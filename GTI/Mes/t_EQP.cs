using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using Genesis.Library.BLL.DTC;
using BLL.MES;
using Genesis.Gtimes.ADM;
using System.Linq;
using Genesis.Gtimes.Transaction.EQP;

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
        }

         


		[TestMethod]
		public void t_GetOperEquipAllEquipmentList()
        => _DBTest((txn) =>
        {
            var oper = txn.EFQuery_MES.PF_OPERATION_EQUIPMENT.FirstOrDefault();
            var operinfo = new OperationUtility.OperationInfo(txn.DBC, oper.OPERATION, OperationUtility.IndexType.SID);
            OperationUtility.OperationFunction operfun = new OperationUtility.OperationFunction(txn.DBC);
            var dt = operfun.GetOperEquipAllEquipmentList(operinfo);
            FileApp.WriteSerializeJson(dt, _log.t_GetOperEquipAllEquipmentList);
        }, true);


        [TestMethod]
		public void t_下機台()
        => _DBTest((txn) =>
        {
            var lotInfo = txn.GetLotInfo("24I049-UG-CD802",isQueryByLotNO:true);
            var equip = txn.GetEquipmentInfo("store_clear_002",IndexType:EquipmentUtility.IndexType.No);
            EQPTransaction.EquipmentUnloadLotTxn unloadLot = new EQPTransaction.EquipmentUnloadLotTxn(equip, lotInfo);
            txn.DoTransaction(unloadLot);

            EQPTransaction.EndOfEquipmentTxn endEquip = new EQPTransaction.EndOfEquipmentTxn(equip);
            txn.DoTransaction(endEquip);
        }, true,true);

	}


}
