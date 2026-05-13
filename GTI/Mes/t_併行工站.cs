using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using Genesis.Gtimes.Transaction.EQP;
using System.Linq;
using static BLL.MES.WIPInjectServices;
using BLL.MES;
using System.Data;
using Genesis.Gtimes.Common;
using System;
using Genesis.Gtimes.WIP;
using BLL.MES.DataViews;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.ADM;
using Genesis;
using static Genesis.Library.BLL.DTC.Lot;
using Newtonsoft.Json;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Frame.Code.Web.Select;
using BLL.InterFace;
using _Check = Genesis.Library.BLL.MES.OperTask.CheckRule;
using Genesis.Gtimes.Transaction.TOL;
using Frame.Code;

namespace UnitTestProject
{
    /// <summary>
    /// 測試 4.5 Txn 相關修正
    /// </summary>
    [TestClass]
	public class t_併行工站 : _testBase
	{
		static class _log
		{

            internal static string t_WIPFormSendParameter_平行工站
            {
                get
                {
                    return FileApp.ts_Log(@"WIP\t_WIPFormSendParameter_平行工站.json");
                }
            }

            internal static string t_GetOperEquipment
            {
                get
                {
                    return FileApp.ts_Log(@"WIP\t_GetOperEquipment.json");
                }
            }

            internal static string t_WIPFormSendParameter
            {
                get
                {
                    return FileApp.ts_Log(@"WIP\t_WIPFormSendParameter.json");
                }
            }
            internal static string t_eqpInfo
            {
                get
                {
                    return FileApp.ts_Log(@"ZZ\CUB\t_eqpInfo.json");
                }
            }

        }


        [TestMethod]
        public void t_EquipmentLoadLotTxn()
		=> _DBTest((txn) => {
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
        => _DBTest((txn) => {
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

            ///主要 進站時使用 , 再者 是 U型站才會使用
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
		public void t_GetOperEquipment_取得工作站設定的機台()
		=> _DBTest((txn) =>
		{
			//TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var _lot = txn.EFQuery_MES.WP_LOT.FirstOrDefault(c => c.STATUS == "Wait");
			var lotInfo = txn.GetLotInfo(_lot.LOT_SID);
			var RouteVerOperInfo = txn.GetRouteVerOper(lotInfo.ROUTE_VER_OPER_SID);
			var x = WIPOperConfigServices.GetOperEquipment(txn.DBC, lotInfo, RouteVerOperInfo);
            FileApp.WriteSerializeJson(x, _log.t_GetOperEquipment);
		}, false, true);


        [TestMethod]
        public void t_GetOperEquipment_UserTraceStart_EqpToolCatch()
        => _DBTest((Txn) =>
        {
            List<SelectModel> eqpInfo = FileApp.Read_SerializeJson<List<SelectModel>>(_log.t_eqpInfo);
            var iLot = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                .Where(c=>c.LOT_SID == "GTI25052211164229261")
                .FirstOrDefault();
            var operinfo = Txn.GetOperationInfo(iLot.OPER_SID);
            var operfun = new OperationUtility.OperationFunction(Txn.DBC);
            DataTable operRule_Eqp = operfun.GetOperEquipAllEquipmentList(operinfo);
            DataTable operRule_Tool = operfun.GetOperToolItemSetting(iLot.OPER_SID);
            Check.Invalid("此工作站有設定機台規則,必須選取機台", operRule_Eqp != null && eqpInfo == null, new { operRule_Eqp ,eqpInfo});

            var _eqp_trace = Txn.EFQuery_MES.WP_LOT_EQP_TRACE
                .Where(c => c.EQP_LINK_SID == iLot.EQP_LINK_SID && c.UNLOAD_LINK_SID == null)
                .ToList();

            var _operRule_Eqp = operRule_Eqp.AsEnumerable();
            var _operRule_Tool = operRule_Tool.AsEnumerable();
            var _dc_ToolStateInfo = new Dictionary<string, ToolUtility.ToolStateInfo>();

            foreach (var eqp in eqpInfo){
                var eqpRule = _operRule_Eqp.FirstOrDefault(x => x.Field<string>("EQP_SID") == eqp.SID);
                Check.Invalid($"機台[{eqp.No}]不是此工作站的可用機台", eqpRule == null, new { eqpRule });

                var _eqp_trace_load = _eqp_trace.FirstOrDefault(c => c.EQP_SID == eqp.SID);
                if(_eqp_trace_load == null){
                    var _eqpInfo = Txn.GetEquipmentInfo(eqp.SID);
                    Txn.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(_eqpInfo, iLot));
                    iLot.ReLoad(Txn);
                }

                /// 取得機台上的治具清單
                var ToolsOfEqp = Txn.EFQuery_MES.f取得機台上的治具清單(eqp.No).ToList();
                Check.Invalid($"機台[{eqp.No}]必須有搭配治具", ToolsOfEqp.Count != 0 && eqp.subItem?.Count == 0 , new { ToolsOfEqp,eqp });

                List<ToolUtility.ToolInfo> List_ToolInfo = new List<ToolUtility.ToolInfo>();
                foreach(var tool in eqp.subItem){
                    var toolRule = _operRule_Tool.FirstOrDefault(x => x.Field<string>("NO") == tool.No);
                    Check.Invalid($"治具[{tool.No}]不是此工作站的可用治具", toolRule == null, new { toolRule });
                    
                    //判定 治具是否己在機台上
                    var _tool = ToolsOfEqp.FirstOrDefault(c => c.TOOL_NO == tool.No);
                    if (_tool == null){
                        //治具上機台
                        var _toolInfo = Txn.GetToolInfo(tool.No);
                        //檢核治具是否可用
                        _Check.Tool(_toolInfo, null, iLot.QUANTITY??0);
                        List_ToolInfo.Add(_toolInfo);
                    }
                }

                if (List_ToolInfo.Count > 0){
                    var _eqpInfo = Txn.GetEquipmentInfo(eqp.SID);
                    Txn.DoTransaction(new EQPTransaction.EquipmentLoadToolTxn(_eqpInfo, List_ToolInfo));
                    foreach (var tool in List_ToolInfo)
                    {
                        Txn.DoTransaction(new TOLTransaction.EndOfToolTxn(tool));
                    }
                    Txn.DoTransaction(new EQPTransaction.EndOfEquipmentTxn(_eqpInfo));
                }
            } 
            

			Txn.EFQuery_MES.SaveChanges();
			//Txn.result.Data = new { d_EqpToolCatch };

        }, true);

 
 


        [TestMethod]
        public void t_查詢機台上的治具()
        => _DBTest((txn) => {
            var SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.Where(c => c.QTY_SN_SID == "GTI25011514234219709");
            var _SN = SN.FirstOrDefault();
            var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                .Where(c => c.LOT == _SN.LOT && c.ROUTE_VER_OPER_SID == _SN.ROUTE_VER_OPER_SID);

            TxnBase.Test = ((Txn, ActionName, Link_SID) => {
                var WP_LOT_OPER_PARALLEL_SN = SN.FirstOrDefault();
                var r = new
                {
                    WP_LOT_OPER_PARALLEL_SN,
                    WP_LOT_OPER_PARALLEL = _lot.FirstOrDefault(),
                    WP_LOT_SN_LIST = txn.EFQuery_MES.WP_LOT_SN_LIST.FirstOrDefault(c => c.SN == WP_LOT_OPER_PARALLEL_SN.SN),
                };

                string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(GTI_Test.g_path.t_Process, json);
            });

            txn.DoTransaction(new PARALLEL_SN_CHANGE_STATUS(_lot.FirstOrDefault(), _SN.SN, nameof(LotStatus.Hold)));
        }, true, true);

        /*
         SELECT LOT.LOT,LOT.QUANTITY,LOT.UNIT,LOT.PRODUCT,LOT.PARTNO,LOT.ROUTE,EQP.CREATE_DATE
                        FROM WP_LOT LOT,WP_LOT_EQP_TRACE EQP
                        WHERE LOT.LOT_SID=EQP.LOT_SID
                        AND LOT.EQP_LINK_SID = EQP.EQP_LINK_SID
                        AND EQP.EQP_NO=:EQP_NO
         
         */
        [TestMethod]
        public void t_查詢機台上有沒有特定批號()
        => _DBTest((Txn) =>
        {
            var _lotA = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == "GTI25052211164229261");
            var _lotB_未上機台批號 = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == "GTI24122615505796905");
            var _eqp_trace = Txn.EFQuery_MES.WP_LOT_EQP_TRACE.FirstOrDefault(c => c.EQP_LINK_SID == _lotA.EQP_LINK_SID);
            //var _eqpInfo = Txn.GetEquipmentInfo(_eqp.EQP_SID);
            //var _lotInfo = Txn.GetLotInfo(_lot.LOT_SID);
            var EqpInfo = Txn.GetEquipmentInfo(_eqp_trace.EQP_SID);
            Txn.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(EqpInfo, _lotA));


        }, true);


        [TestMethod]
        public void t_測試已經有上到A機台的批號再上B機台的問題()
        => _DBTest((Txn) =>
        {
            var q_lot = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.Where(c => c.LOT_SID == "GTI25052211164229261");
            var _lotA = q_lot.FirstOrDefault();
            var eqp_link_sid = _lotA.EQP_LINK_SID;
            //var _lotB_未上機台批號 = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == "GTI24122615505796905");
            var q_eqp_trace = Txn.EFQuery_MES.WP_LOT_EQP_TRACE.Where(c => c.EQP_LINK_SID == eqp_link_sid);
            var _eqp_trace = q_eqp_trace.FirstOrDefault();


            var _eqpB = Txn.EFQuery_MES.FC_EQUIPMENT.FirstOrDefault();
            var EqpInfo_A = Txn.GetEquipmentInfo(_eqp_trace.EQP_SID);
            var EqpInfo_B = Txn.GetEquipmentInfo(_eqpB.EQP_SID);

            TxnBase.Test = ((txn, ActionName, Link_SID) => {
                var r = new{
                    eqp_link_sid,
                    原始的Lot = _lotA,
                    上過另一個機台後的Lot = q_lot.FirstOrDefault(),
                    異動前的eqp_trace = _eqp_trace,
                    異動後的eqp_trace = txn.EFQuery_MES.WP_LOT_EQP_TRACE.Where(c=>c.CREATE_DATE == txn.ExeTime || c.EQP_LINK_SID == eqp_link_sid).ToList(),
                    WP_LOT_OPER_PARALLEL_HIST = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_HIST
                        .Where(c => c.CREATE_DATE == txn.ExeTime).ToList(),
                };

                string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(GTI_Test.g_path.t_Process, json);
            });


            Txn.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(EqpInfo_B, _lotA));

            _lotA = q_lot.FirstOrDefault();
            Txn.DoTransaction(new EQPTransaction.EquipmentUnloadLotTxn(EqpInfo_A, _lotA));

        },true);

        [TestMethod]
		public void t_PARALLEL_SN_CHANGE_STATUS()
		=> _DBTest((txn) =>{
            var SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.Where(c => c.QTY_SN_SID == "GTI25011514234219709");
            var _SN = SN.FirstOrDefault();
            var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                .Where(c => c.LOT == _SN.LOT && c.ROUTE_VER_OPER_SID == _SN.ROUTE_VER_OPER_SID);
            
            TxnBase.Test =((Txn, ActionName, Link_SID) => {
                var WP_LOT_OPER_PARALLEL_SN = SN.FirstOrDefault();
                var r = new
                {
                    WP_LOT_OPER_PARALLEL_SN,
                    WP_LOT_OPER_PARALLEL = _lot.FirstOrDefault(),
                    WP_LOT_SN_LIST = txn.EFQuery_MES.WP_LOT_SN_LIST.FirstOrDefault(c => c.SN == WP_LOT_OPER_PARALLEL_SN.SN),
                };

                string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(GTI_Test.g_path.t_Process, json);
            }); 

            txn.DoTransaction(new PARALLEL_SN_CHANGE_STATUS(_lot.FirstOrDefault(), _SN.SN, nameof(LotStatus.Hold)));
        }, true, true);

        [TestMethod]
        public void t_併行工站批號_SPC_Hold()
        => _DBTest((txn) =>
        {
            var SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.Where(c => c.QTY_SN_SID == "GTI25011514234219709");
            var _SN = SN.FirstOrDefault();
            var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                .Where(c => c.LOT == _SN.LOT && c.ROUTE_VER_OPER_SID == _SN.ROUTE_VER_OPER_SID)
                //.Select(c=>new {c.WO,c.LOT_SID, c.LOT, c.STATUS ,c.LAST_STATUS_CHANGE_TIME })
                ;

            var q_需要扣留的清單 = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                .Where(c => _lot.Any(z=>z.WO == c.WO && z.LOT == c.LOT))
                ;

                    TxnBase.Test = ((Txn, ActionName, Link_SID) => {
                        var WP_LOT_OPER_PARALLEL_SN = SN.FirstOrDefault();
                        var r = new{
                            WP_LOT_OPER_PARALLEL = q_需要扣留的清單
                                .Select_Info("STATUS")
                                ,
                            WP_LOT_OPER_PARALLEL_HIST = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_HIST
                                .IQueryable_ACTION_LINK_SID(Link_SID)
                                .Select_Info(),
                            WP_LOT_OPER_PARALLEL_SN,
                            WP_LOT_SN_LIST = Txn.EFQuery_MES.WP_LOT_SN_LIST
                                .FirstOrDefault(c => c.SN == WP_LOT_OPER_PARALLEL_SN.SN),
                            WP_LOT_OPER_PARALLEL_HOLD = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_HOLD
                                .IQueryable_Col("HOLD_LINK_SID", Link_SID).ToList(),
                        };
                

                string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(GTI_Test.g_path.t_Process, json);
            });
            

            //限制只有 Run/Wait
            var sts = new string[] { LotStatus.Run.ToString(), LotStatus.Wait.ToString() };
            var _需要扣留的清單 = q_需要扣留的清單
                .Where(c=> sts.Contains(c.STATUS))
                .ToList();
 
            var SPCReasonCodeInfo = new ReasonUtility.ReasonCodeInfo(DBC, "SPCHold", ReasonUtility.IndexType.No);
            string HoldDesc = $"管制圖xxx 第[xxx點違反規則:xxxx";

            var _dtc = new PARALLEL_HOLD(_需要扣留的清單, "SPCHold", HoldDesc) {
                reason = SPCReasonCodeInfo
            };
            txn.DoTransaction(_dtc);
            

        }, true, true);

        [TestMethod]
        public void t_查詢未完工的併行站點()
        => _DBTest((txn) =>{
            var sts = new string[] {LotStatus.Run.ToString(), LotStatus.Wait.ToString() };
            var SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.Where(c => c.QTY_SN_SID == "GTI25011514234219709");
            var _SN = SN.FirstOrDefault();
            var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                .FirstOrDefault(c => c.LOT == _SN.LOT && c.ROUTE_VER_OPER_SID == _SN.ROUTE_VER_OPER_SID);

            var _需要扣留的站 = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.Where(c =>
                c.WO == _lot.WO
                && c.LOT == _lot.LOT
                && c.LOT_SID != _lot.LOT_SID
                && sts.Contains(c.STATUS)
                ).ToList();


        }, false);


    }


}
