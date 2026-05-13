using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using Genesis.Library.BLL.DTC;
using BLL.MES;
using Genesis.Gtimes.ADM;
using System.Linq;
using Genesis.Gtimes.Transaction.EQP;
using Frame.Code.Web.Select;
using System.Collections.Generic;
using System.Data;
using Genesis.Library.BLL.FC;
using Genesis.Gtimes.Transaction.TOL;

namespace UnitTestProject
{
    [TestClass]
    public class t_EQP : _testBase
    {
        static class _log
        {
            internal static string t_GetOperEquipAllEquipmentList
            {
                get
                {
                    return FileApp.ts_Log(@"EQP\t_GetOperEquipAllEquipmentList.json");
                }
            }
            internal static string t_GetOperToolSetting
            {
                get
                {
                    return FileApp.ts_Log(@"EQP\t_GetOperToolSetting.json");
                }
            }



            internal static string t_過站設備
            {
                get
                {
                    return FileApp.ts_Log(@"EQP\t_過站設備.json");
                }
            }

            internal static string t_SetUpTool_info
            {
                get
                {
                    return FileApp.ts_Log(@"EQP\t_SetUpTool_info.json");
                }
            }
        }



        /// <summary>
        /// 取得工站設定的設備
        /// </summary>
        [TestMethod]
        public void t_GetOperEquipAllEquipmentList()
        => _DBTest((txn) =>
        {
            //var oper = txn.EFQuery_MES.PF_OPERATION_EQUIPMENT.FirstOrDefault();

            var operinfo = new OperationUtility.OperationInfo
                (txn.DBC
                , "C01-0020"
                , OperationUtility.IndexType.No);
            OperationUtility.OperationFunction operfun = new OperationUtility.OperationFunction(txn.DBC);
            var dt = operfun.GetOperEquipAllEquipmentList(operinfo);
            //FileApp.WriteSerializeJson(dt, _log.t_GetOperEquipAllEquipmentList);
        }, true);


        /// <summary>
        /// 取得工站設定的治具
        /// </summary>
        [TestMethod]
        public void t_GetOperToolSetting()
         => _DBTest((txn) =>
         {
            //var oper = txn.EFQuery_MES.PF_OPERATION_EQUIPMENT.FirstOrDefault();
            var operinfo = new OperationUtility.OperationInfo(txn.DBC, "B121-019BA4-139_1000-0040", OperationUtility.IndexType.No);
             OperationUtility.OperationFunction operfun = new OperationUtility.OperationFunction(txn.DBC);
             var dt = operfun.GetOperToolSetting(operinfo.SID);
             FileApp.WriteSerializeJson(dt, _log.t_GetOperToolSetting);
         }, false);

        /*
         			EquipmentUtility.EquipmentFunction uf = new EquipmentUtility.EquipmentFunction(dbc);
			var dt = uf.GetPartNoOperEquipmentData_OperSid(_lotInfo.WO, RouteVerOperInfo.RouteVerSid, RouteVerOperInfo.RouteVerOperSid
				, _lotInfo.PARTNO, RouteVerOperInfo.OperSid);
         
         */

        /*
         
         
         */
        [TestMethod]
        public void t_下機台()
        => _DBTest((txn) =>
        {
            var lotInfo = txn.GetLotInfo("24I049-UG-CD802", isQueryByLotNO: true);
            var equip = txn.GetEquipmentInfo("store_clear_002", IndexType: EquipmentUtility.IndexType.No);
            EQPTransaction.EquipmentUnloadLotTxn unloadLot = new EQPTransaction.EquipmentUnloadLotTxn(equip, lotInfo);
            txn.DoTransaction(unloadLot);

            EQPTransaction.EndOfEquipmentTxn endEquip = new EQPTransaction.EndOfEquipmentTxn(equip);
            txn.DoTransaction(endEquip);
        }, true, true);


        //改移到 t_CUB
        public void job_重設機台並下治具_平行工站(){}


        [TestMethod]
        public void t_檢核過站時的機台是否符合工作站的設定()
        => _DBTest((txn) =>
        {
            var _過站設備 = FileApp.Read_SerializeJson<List<CustomerList>>(_log.t_過站設備);
            var oper_no = "B121-019BA4-139_1000-0040";
            var operinfo 
                //= new OperationUtility.OperationInfo(txn.DBC, oper_no, OperationUtility.IndexType.No);
                = txn.GetOperationInfo(oper_no,IndexType:OperationUtility.IndexType.No);
            OperationUtility.OperationFunction operfun = new OperationUtility.OperationFunction(txn.DBC);
            var dt_Eqp = operfun.GetOperEquipAllEquipmentList(operinfo);
            if (dt_Eqp != null && dt_Eqp.Rows.Count > 0) {
                var _dt_Eqp = dt_Eqp.AsEnumerable();
                var chkEqp = _過站設備.Any(c => _dt_Eqp.Any(row => row.Field<string>("EQP_NO") == c.No)==false);
            }

        }, false, true);







        [TestMethod]
        public void t_fn()
        {
            var EQP_SID = "GTI24072816351496353";
            //var z = EquipmentService.SetUpTool_info(EQP_SID);
            //FileApp.WriteSerializeJson(z, _log.t_SetUpTool_info);
        }

        [TestMethod]
        public void t_f治具清單()
        
        => _DBTest((txn) =>
        {
            //var lot = GTI_helper.getLotInfo(txn);
            //var zz = txn.EFQuery_MES.f治具清單("CTT01",排除已被使用:false).ToList();
        }, true);


        [TestMethod]
        public void t_GetEquipmentLotList()
        => _DBTest((Txn) =>
        {
            var EqpFn = new EquipmentUtility.EquipmentFunction(Txn.DBC);
            var EqpInfo = Txn.GetEquipmentInfo("CWH1-K004-0000002-01", IndexType:EquipmentUtility.IndexType.No);
            var lots = EqpFn.GetEquipmentLotList(EqpInfo.No);
        }, true);



        [TestMethod]
        public void _20260115_批號取得進站後綁定的機台()
        => _DBTest((Txn) =>{
            var _lotInfo = Txn.GetLotInfo("JK_GENERAL_TEST-08", isQueryByLotNO: true);
            var eqps = _lotInfo.GetLotProductionEquipmentInfoList();
        }, true);

        
    }


}
