using BLL.MES;
using Genesis.Gtimes.ADM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
    /// <summary>
    /// 賽諾世
    /// </summary>
    [TestClass]
	public class t_ZAC : _testBase
	{
		static class _log
		{
			internal static string list_WP_WO_MTL_BOM
			{
				get
				{
					
					return FileApp.ts_Log(@"ZZ\TCI\WP_WO_MTL_BOM.json");
				}
			}
 

		}


        [TestMethod]
        public void t_20260114_取得批號子批數量()
        => _DBTest((txn) =>
        {
             
        }, true);

        /// <summary>
        /// 分段報工最後一筆數量完成出站後_該母批狀態為Finished
        /// </summary>
        [TestMethod]
        public void t_20260114_數量分完後_母批狀態為Terminated()
        => _DBTest((txn) =>  {
            var _lot = txn.GetLotInfo("DevTest_20260114-01",true, isQueryByLotNO: true);
            var SplitList = new List<QtyItem>() { new QtyItem() { Qty = 301 } };
            EncodeFormatUtility.CodesInfo codes = null;
            var _old_Qty = _lot.QUANTITY;
            
            var r = LOT_Services.Txn_LotSplit(txn, SplitList,null,true);
            _lot = _lot.ReLoad(txn.DBC);


            var subLots = _lot.getLotSubLotListByLotSid(); 

            FileApp._tmpJson(new { _old_Qty  ,_lot.QUANTITY});

        }, true,true);


        /// <summary>
        /// 分段報工最後一筆數量完成出站後_該母批狀態為Finished
        /// </summary>
        [TestMethod]
        public void t_20260114_良品加報廢歸給子批_母批扣數()
        => _DBTest((txn) =>  {

        }, true);


        /// <summary>
        /// 沒有良品數,只有 報廢數時 ,要切成子批並完成 Terminated
        /// </summary>
        [TestMethod]
        public void t_20260114_單獨報廢_子批Terminated_留在當站()
        => _DBTest((txn) =>{

        }, true);
    }
}

