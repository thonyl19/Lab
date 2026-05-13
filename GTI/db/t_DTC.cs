using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using Genesis.Library.BLL.DTC;
using Genesis.Gtimes.Transaction.EQP;
using Genesis.Gtimes.Transaction.WIP;
using System.Linq;

namespace UnitTestProject
{
	[TestClass]
	public class t_DTC : _testBase
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
		}

 

		[TestMethod]
		public void t_SQL_查詢範例()
		{

			using (var dbc = this.DBC)
			{
				var _sql = $@"
                SELECT  top 1 *
                FROM    PF_PARTNO_VER

                         ";

				var dt = dbc.Select(_sql);

				new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));
				new FileApp().Write_SerializeJson(dt, _log.t_splitBIN);
			}
		}
		[TestMethod]
		public void _DTC_AdjustmentCapacity()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo("GTI22060711213497371");
			var c = lot.GetCurrentCarrierInfo();
			var exp = c.CURRENT_CAPACITY - 1;
			Txn.DoTransaction(new Carrier.DTC_AdjustmentCapacity(lot, c, -1));
			var act = lot.GetCurrentCarrierInfo().CURRENT_CAPACITY;
			Assert.AreEqual(exp, act,$"扣數後, 數值應為 {exp}");
		}, true);


		[TestMethod]
		public void _DTC_WoCheckIn()
		=> _DBTest(Txn => {
			Txn.GetLotInfo("GRF_WO_ProductMA02-01", true,true);
			Txn.DoTransaction(new WO.DTC_WoCheckIn("GRF_WO_ProductMA02-01","GTI22090513500294503"));
			//var act = lot.GetCurrentCarrierInfo().CURRENT_CAPACITY;
			//Assert.AreEqual(exp, act, $"扣數後, 數值應為 {exp}");
		}, true);



		[TestMethod]
		public void _DTC_SendMail()
		=> _DBTest(Txn => {
			//Txn.DoTransaction(new AL.SendMail("anthony_lin@genesis.com.tw", "Test"));
		},true,true);

		[TestMethod]
		public void _DTC_修改ReasonNo()
		=> _DBTest(Txn => {
			var _lotInfo = GTI_helper.getLotInfo();
			var EqpInfo = GTI_helper.getEquipmentInfo(Txn);// Txn.GetEquipmentInfo("GTI23121315360556348");
			Txn.GtimesTxn.GetCurrentTxnBase().ReasonNo = "AAAAAA";
			var oHold = new WIPTransaction.HoldLotTxn(_lotInfo);
			//oHold.TransactionName

			//Txn.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(EqpInfo, _lotInfo));
			//Txn.DoTransaction(new AL.SendMail("anthony_lin@genesis.com.tw", "Test"));
		}, true, true);
	}


}
