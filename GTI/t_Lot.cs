using BLL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

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




	}
}
