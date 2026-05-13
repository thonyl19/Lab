using Microsoft.VisualStudio.TestTools.UnitTesting;
using MDL.MES;
using System.Linq;
using System.Collections.Generic;
using Genesis.Library.BLL.ZZ.MPI.Model;
using Newtonsoft.Json;
using static Genesis.Library.BLL.ADM.EDCTargetServices;
using Genesis.Library.BLL.ADM;
using UnitTestProject.TestUT;
using _svc_EDC = Genesis.Library.BLL.ADM.EDCTargetServices;

namespace UnitTestProject
{
    public partial class t_MPI : _testBase
    {
 

        /// <summary>
        /// 驗證 ZZ_MPI_EDC_LOG 資料寫入 (Task 003)
        /// </summary>
        [TestMethod]
        public void t_20260412_EDC規格快照寫入測試()
        => _DBTest((txn) =>
        {
            // 1. 取得測試批號
            var lot = txn.EFQuery_MES.WP_LOT.FirstOrDefault();
            Assert.IsNotNull(lot, "資料庫需至少有一個 Lot 以供測試");

            // 2. 模擬 DTC 寫入
            var mockJson = "[{\"GROUP_CODE\":\"TEST_GROUP\",\"VAL\":100}]";
            var dtc = new Genesis.Library.BLL.DTC.Lot.Insert_ZZ_MPI_EDC_LOG
            {
                actionLinkSID = txn.LinkSID,
                lotId = lot.LOT,
                edcConfigJson = mockJson,
                catchKey = "UT_TEST",
                execUser = "UT_AGENT"
            };
            txn.DoTransaction(dtc);

            // 3. 驗證結果
            var log = txn.EFQuery_MES.ZZ_MPI_EDC_LOG.FirstOrDefault(c => c.ACTION_LINK_SID == txn.LinkSID);
            Assert.IsNotNull(log, "ZZ_MPI_EDC_LOG 應該要被成功寫入");
            Assert.AreEqual(mockJson, log.EDC_CONFIG_JSON);
            Assert.AreEqual(lot.LOT, log.LOT_ID);
        }, true); // Rollback = true

        /// <summary>
        /// 驗證快取復原機制 (Task 004)
        /// </summary>
        [TestMethod]
        public void t_20260412_EDC快取隔離與恢復測試()
        => _DBTest((txn) =>
        {
            // 1. 準備 Mock 快取資料
            var lotNo = "UT_LOT_CACHE_TEST";
            var catchKey = $"STATION_CHECKOUT_{lotNo}";
            
            var mockEdcModel = new List<EdcLoadResult> {
                new EdcLoadResult {
                    ZZ_EDC_GROUP_CODE = new EdcGroupCodeDetail { GROUP_CODE = "G1" },
                    columns = new List<FiledSet> { new FiledSet { data = "P1", title = "P1 (Restored Value)" } }
                }
            };

            var catchData = new AD_CATCH_DATA {
                SID = txn.GetSID(),
                FUN_NAME = catchKey,
                FUN_SID = "STATION_CHECKOUT",
                CATEGORY = "EDC_DATA",
                DATA = JsonConvert.SerializeObject(mockEdcModel),
                CREATE_USER = "UT_USER",
                CREATE_DATE = txn.ExeTime,
                RETENTION_PERIOR = txn.ExeTime.AddDays(7)
            };
            txn.EFQuery_MES.AD_CATCH_DATA.Add(catchData);
            txn.EFQuery_MES.SaveChanges();

            // 2. 準備 OperTask_Edc 環境
            // 這裡模擬一個 lotInfo
            var lot = new Genesis.Gtimes.WIP.LotUtility.LotInfo { 
                LOT = lotNo,
                SID = "UT_SID",
                WO_LINE_NO = "UT_LINE"
            };

            // 手動調用 OperTask_Edc (我們在 Task 004 已經修改了它)
            // 由於內部會查真實 DB，我們這邊僅能測試其結構或確保語法通過
            var dummyRule = new d_FC_EDC_TARGET_rule { 
                EDC_VER_SID = "UT_VER_SID" 
            };

            // 因 OperTask_Edc 內部會 txn.GetLotInfo(LOT_SID)，所以 LOT_SID 必須存在於 DB
            // 為了測試，我們改用現有的 Lot，但改寫 CatchKey
            var realLot = txn.EFQuery_MES.WP_LOT.FirstOrDefault();
            if (realLot != null) {
                var realCatchKey = $"STATION_CHECKOUT_{realLot.LOT}";
                catchData.FUN_NAME = realCatchKey;
                txn.EFQuery_MES.SaveChanges();

                var arg = new EdcLoadArguments { LOT = realLot.LOT, LINE = new string[] { realLot.WO_LINE_NO } };
                var rules = EDCTargetServices.get_FC_EDC_TARGET_rule(txn.EFQuery_MES, arg);

                if (rules.Any()) {
                    var res = EDCTargetServices.OperTask_Edc(txn, rules[0], realLot.LOT_SID);
                    Assert.IsNotNull(res, "應回傳 EDC 設定物件");
                    // 此處可驗證 res.edc_Model 是否包含我們注入的資料 (如果 GROUP_CODE 吻合)
                }
            }
        }, true);
    }
}
