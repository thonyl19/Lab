using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using WMS.BLL.Carrier;
using WMS.BLL.Enum;
using _DTC = WMS.BLL.DTC;

namespace UnitTestProject
{
	[TestClass]
	public class t_GRF_WMS : _testBase
	{
		static class _log
		{
			/// <summary>
			/// 分條進站 的 執行
			/// </summary>
			internal static string ExecSlittingCheckIn
			{
				get
				{
					return FileApp.ts_Log(@"GRF\ExecSlittingCheckIn.json");
				}
			}
			internal static string t_WH_CARRIER
			{
				get
				{
					return FileApp.ts_Log(@"GRF\t_WH_CARRIER.json");
				}
			}
			internal static string CarrierData_Transfer
			{
				get
				{
					return FileApp.ts_Log(@"GRF\CarrierData_Transfer.json");
				}
			}
		}


		#region [ Sample ] 
		/*
		[TestMethod]
		public void _Sample()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo("GTI22060711213497371");
			var c = lot.GetCurrentCarrierInfo();
			var exp = c.CURRENT_CAPACITY - 1;
			Txn.DoTransaction(new Carrier.DTC_AdjustmentCapacity(lot, c, -1));
			var act = lot.GetCurrentCarrierInfo().CURRENT_CAPACITY;
			Assert.AreEqual(exp, act,$"扣數後, 數值應為 {exp}");

			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));
			new FileApp().Write_SerializeJson(dt, _log.t_splitBIN);

			var _r = new FileApp().Read_SerializeJson(_log.t_splitBIN);
		}, true);
		*/
		#endregion
		[TestMethod]
		public void t_DTC_EmptyCarrier()
		=> _DBTest(Txn => {
			Txn.DoTransaction(new _DTC.Carrier.EmptyCarrier("100008-084"));
		}, true);


		[TestMethod]
		public void t_DTC_ChangeCapacity()
		=> _DBTest(Txn => {
			Txn.DoTransaction(new _DTC.Carrier.ChangeCapacity("BILL_NO", "720006-356", "PARTNO", "BATCH_NO",5));
		}, true);


		[TestMethod]
		public void t_DTC_ChangeStatus()
		=> _DBTest(Txn => {
			Txn.DoTransaction(new _DTC.Bill.ChangeStatus("MO202305021140", EBillStatus.FINISHED));
		}, true);


		[TestMethod]
		public void t_DTC_ChangeStatus_Lock測試()
		=> _DBTest(Txn => {
			var BILL_NO = "MO202305021140";
			Txn.DoTransaction(new _DTC.Bill.ChangeStatus(BILL_NO, EBillStatus.FINISHED));
			var t = Txn.EFQuery<WH_BILL>().Read(w => w.BILL_NO == BILL_NO);
		}, true);

		[TestMethod]
		public void t_DTC_CreateCarrier()
		=> _DBTest(Txn => {
			//var x = Txn.EFQuery<WH_CARRIER>().Read(c =>c.BILL_NO == "P202304191713");
			var x = new FileApp().Read_SerializeJson<WH_CARRIER>(_log.t_WH_CARRIER);
			Txn.DoTransaction(new _DTC.Carrier.CreateCarrier(x, "InsertCarrier"));
		}, true);


		struct _d_CarrierData_Transfer {
			public List<WH_CARRIER> CarrierData { get; set; }
			public WH_BILL form { get; set; }
		}

		[TestMethod]
		public void t_CarrierData_Transfer()
		{
			var _r = FileApp.Read_SerializeJson<_d_CarrierData_Transfer>(_log.CarrierData_Transfer);
			new CarrierServices().CarrierTransfer(_r.CarrierData,_r.form,true);
		}

	}


}

