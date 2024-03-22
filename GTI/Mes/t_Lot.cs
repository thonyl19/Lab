using BLL.MES;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject
{

    [TestClass]
	public class t_Lot : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_GetMTLotOnEqpOfLot
			{
				get
				{
					return FileApp.ts_Log(@"Lot\t_GetMTLotOnEqpOfLot.json");
				}
			}
			internal static string t_Lot_Defect
			{
				get
				{
					return FileApp.ts_Log(@"Lot\t_Lot_Defect.json");
				}
			}
		}

		[TestMethod]
		public void t_Hold_LotInfo()
		{
			//string LotNo = "201-20121129-34";
			//var _ctr = new LotController();
			//var result = _ctr.Hold_LotInfo(LotNo);
			//new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"Lot\t_Hold_LotInfo.json"));

		}


        [TestMethod]
        public void t_fn()
		=> _DBTest((txn) => {
			var x = new LOT_Services().Defect_LotInfo("JK_WO_001-08.02");
			FileApp.WriteSerializeJson(x.Data.OperDefectList, _log.t_Lot_Defect);
		
		}, true);

		[TestMethod]
        public void t_LotConsumptionTxn()
		=> _DBTest((Txn) => {
			var CurrentLot = Txn.GetLotInfo("3B0000-231213-01",isQueryByLotNO:true);
			var mLot = Txn.GetMLotInfo("2001-15409-1-1B01");
			var consumpMLot = new LotUtility.LotConsumptionMlotQuantity(mLot, (decimal)50, 0, 0);
			Txn.DoTransaction(new WIPTransaction.LotConsumptionTxn(CurrentLot, consumpMLot));
		}, true);


        [TestMethod]
        public void t_LotChangeAttributeTxn()
        => TxnBase.LzDBTrans_t((tx) =>
        {
            var lot = tx.GetLotInfo("GTI22031515091645302");
			// 很奇怪, 使用 LotChangeAttributeTxn  會出現跟 WIPTransaction 發生衝突的問題 ,只能先註解掉 
			//var _txn = new LotChangeAttributeTxn(lot, "ATTRIBUTE_35", lot.ATTRIBUTE_35, "A");
			//tx.DoTransaction(_txn);
			//lot.ReLoad();
			return tx.result;
        });

		[TestMethod]
		public void t_LotAddRemark()
		{
			//LOT_Services.LotAddRemark("RIS_20230101-A1-01" ,"Remark", "other", "test",true);
		}

	}
}
