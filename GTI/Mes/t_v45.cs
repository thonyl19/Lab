using Genesis.Library.BLL.ICM.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.ICM.Maintain;
using Dapper;
using BLL.DataViews.Edc;
using Genesis.Gtimes.Transaction.EQP;
using System.Linq;
using static BLL.MES.WIPInjectServices;
using BLL.MES;
using static Genesis.Gtimes.Transaction.TransactionUtility;
using System.Data;
using Genesis.Gtimes.Common;
using System;
using Genesis.Gtimes.WIP;
using Genesis.Gtimes.Transaction;
using BLL.MES.DataViews;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.ADM;
using Genesis;

namespace UnitTestProject
{
	/// <summary>
	/// 測試 4.5 Txn 相關修正
	/// </summary>
	[TestClass]
	public class t_v45 : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_splitBIN
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_splitBIN.json");
				}
			}

            internal static string t_WIPFormSendParameter
            {
                get
                {
                    return FileApp.ts_Log(@"WIP\t_WIPFormSendParameter.json");
                }
            }

            internal static string t_WIPFormSendParameter_平行工站
            {
                get
                {
                    return FileApp.ts_Log(@"WIP\t_WIPFormSendParameter_平行工站.json");
                }
            }
            

        }


        [TestMethod]
        public void t_EquipmentLoadLotTxn()
		=> _DBTest((txn) =>
		{
 

            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var _eqp = txn.EFQuery_MES.FC_EQUIPMENT.FirstOrDefault(c => c.ENABLE_FLAG == "T");
			var EqpInfo = txn.GetEquipmentInfo(_eqp.EQP_SID);
			var _lot = txn.EFQuery_MES.WP_LOT.FirstOrDefault(c => c.STATUS == "Wait");
			var _lot1 = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.STATUS == "Wait");
			var lotInfo = txn.GetLotInfo(_lot.LOT_SID);
			txn.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(EqpInfo, lotInfo));
		}, true,true);


        [TestMethod]
        public void _EquipmentLoadLotTxn1()
        => _DBTest((txn) =>
        {
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            var _eqp = txn.EFQuery_MES.FC_EQUIPMENT.FirstOrDefault(c => c.ENABLE_FLAG == "T");
            var EqpInfo = txn.GetEquipmentInfo(_eqp.EQP_SID);
            ILotInfo lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.STATUS == "Wait");
            var _txn = new EQPTransaction.EquipmentLoadLotTxn(EqpInfo, lot);
            txn.DoTransaction(_txn);
        }, true, true);

		        [TestMethod]
        public void t_EquipmentUnloadLotTxn()
        => _DBTest((txn) =>
        {
            var _lot = "JTest0716-01";
            var EQP_SID = "GTI24051309005595218";
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            
            var EqpInfo = txn.GetEquipmentInfo(EQP_SID);
            var lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT == _lot);
            var _txn = new EQPTransaction.EquipmentUnloadLotTxn(EqpInfo, lot);
            txn.DoTransaction(_txn);
        }, true, true);



        [TestMethod]
        public void _RollBack()
		=> _DBTest((txn) =>
		{
			var _lot1 = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.STATUS == "Wait");
            _lot1.BeginTransaction();
            _lot1.CARRIER_LINK_SID = "Test";
            txn.EFQuery_MES.SaveChanges();

            _lot1.RollBack();
            Assert.AreNotEqual(_lot1.CARRIER_LINK_SID, "Test");
		}, true,true);




        [TestMethod]
        public void t_Defec_v45()
        => _DBTest((txn) =>
        {
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            var data = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_WIPFormSendParameter);
            var lot = txn.GetLotInfo(data.Lot, isQueryByLotNO: true);
            _Func.Defect缺點處理(txn, lot,data.DefectList);
        }, true, true);

        [TestMethod]
        public void t_Defect1()
        => _DBTest((txn) =>
        {
            txn.ActionReason = TxnACTION.n("Test", "other");
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            var svcWIP = txn.LzQuery.WIP;
            var data = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_WIPFormSendParameter_平行工站);
            var lot = svcWIP.f批號平行工作站_是否存在(data.Lot,data.RouterVerOperSid);
            var eqp = txn.GetEquipmentInfo("GTI24051515281998218");
            var _tx = new Genesis.Library.BLL.DTC.Lot.Insert_PARALLEL_DEFECT(lot, data.DefectList, eqp,true);
            txn.DoTransaction(_tx);
        }, true, true);


        [TestMethod]
        public void t_GoToNextParallelTask()
        => _DBTest((txn) =>
        {
            txn.ActionReason = TxnACTION.n("Test", "other");
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            var svcWIP = txn.LzQuery.WIP;
            var data = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_WIPFormSendParameter_平行工站);
            var lot = svcWIP.f批號平行工作站_是否存在(data.Lot, data.RouterVerOperSid);
            txn.ILotInfo = lot;
            var _tx = new Genesis.Library.BLL.DTC.Lot.GoToNextParallelTask(lot);
            txn.DoTransaction(_tx);
        }, true, true);

        


        [TestMethod]
        public void t_Scrap_v45()
        => _DBTest((txn) =>
        {
            txn.ActionReason = TxnACTION.n("Test", "other");
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            var svcWIP = txn.LzQuery.WIP;
            var data = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_WIPFormSendParameter);
            var _CurrentLot_實體 = txn.GetLotInfo(data.Lot, isQueryByLotNO: true);
            var lotScrapCreateInfoList = new List<LotUtility.LotScrapCreateInfo>();
            foreach (var item in data.ScrapList)
            {
                var reasonInfo = txn.GetReasonCodeInfo(item.No, ReasonUtility.IndexType.No);
                if (reasonInfo.IsExist)
                {
                    var lotScrapCreateInfo = new LotUtility.LotScrapCreateInfo(reasonInfo, item.INum);
                    lotScrapCreateInfoList.Add(lotScrapCreateInfo);
                }
            }

            if (!(lotScrapCreateInfoList == null || lotScrapCreateInfoList.Count == 0))
            {
                var oScrap = new WIPTransaction.LotScrapTxn(_CurrentLot_實體, lotScrapCreateInfoList)
                {
                    //報廢到零自動結批                            
                    TerminateLot = true
                };
                txn.DoTransaction(oScrap);
            }
        }, true, true);

        [TestMethod]
        public void t_Scrap()
        => _DBTest((txn) =>
        {
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            var svcWIP = txn.LzQuery.WIP;
            var data = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_WIPFormSendParameter_平行工站);
            var _CurrentLot_實體 = svcWIP.f批號平行工作站_是否存在(data.Lot,data.RouterVerOperSid);
            var eqp = txn.GetEquipmentInfo("GTI24051515281998218");
            var _tx = new Genesis.Library.BLL.DTC.Lot.Insert_PARALLEL_SCRAP(_CurrentLot_實體, _CurrentLot_實體, data.ScrapList, eqp);
            txn.DoTransaction(_tx);
        }, true, true);

        [TestMethod]
        public void t_EDC()
        => _DBTest((txn) =>
        {
            //txn.ActionReason = TxnACTION.n(nameof(GTI_Test.t_Process_PARALLEL), "other");
            TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
            var svcWIP = txn.LzQuery.WIP;
            var data = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_WIPFormSendParameter_平行工站);
            var _CurrentLot_實體 = svcWIP.f批號平行工作站_是否存在(data.Lot,data.RouterVerOperSid);
            var eqp = txn.GetEquipmentInfo("GTI24051515281998218");
            var _tx = new Genesis.Library.BLL.DTC.Lot.Insert_PARALLEL_EDC(_CurrentLot_實體, data.SerialEdcList, eqp) {
                //指定使用者 
                userInfo = txn.GetUserInfo()
            };
            txn.DoTransaction(_tx);
        }, true, true);



 




        [TestMethod]
		public void t_GetOperEquipment()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var _lot = txn.EFQuery_MES.WP_LOT.FirstOrDefault(c => c.STATUS == "Wait");
			var lotInfo = txn.GetLotInfo(_lot.LOT_SID);
			var RouteVerOperInfo = txn.GetRouteVerOper(lotInfo.ROUTE_VER_OPER_SID);
			WIPOperConfigServices.GetOperEquipment(txn.DBC, lotInfo, RouteVerOperInfo);
		}, false, true);





	}


}
