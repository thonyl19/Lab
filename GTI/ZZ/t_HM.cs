using Genesis.Library.BLL.DTC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
    [TestClass]
	public class t_GRF : _testBase
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
		public void _Sample()
		=> _DBTest(Txn => {
			Txn.DoTransaction(new Wafer.Rec_NormalWafer_When_CheckOut("JK_WO_001-04.01.02"));
		}, true);


		//[TestMethod]
		//public void _Sample1()
		//=> _DBTest(Txn => {
		//	var lot = Txn.GetLotInfo();
		//	var Wafer = null;
		//	Txn.DoTransaction
		//	(new Wafer.Process_WAFER_REWORK(Wafer, lot, (int)100)
		//	);
		//}, true);

	}
}

