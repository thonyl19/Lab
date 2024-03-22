using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.SMT.Maintain;

namespace UnitTestProject
{
	[TestClass]
	public class t_SMT : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string SMT_BOM_ITEM_SaveMain
			{
				get
				{
					return FileApp.ts_Log(@"SMT\SMT_BOM_ITEM_SaveMain.json");
				}
			}
			internal static string SMT_BOM_ITEM_SaveItem
			{
				get
				{
					return FileApp.ts_Log(@"SMT\SMT_BOM_ITEM_SaveItem.json");
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
		public void t_SMT_BOM_ITEM_SaveMain(){ 
			var _r = FileApp.Read_SerializeJson<SMT_BOM>(_log.SMT_BOM_ITEM_SaveMain);
			Maintain.SMT_BOM_ITEM_SaveMain(_r, true);
		}


		[TestMethod]
		public void t_SMT_BOM_ITEM_SaveItem()
		{
			var main = FileApp.Read_SerializeJson<SMT_BOM>(_log.SMT_BOM_ITEM_SaveMain);
			var items = FileApp.Read_SerializeJson<List<SMT_BOM_ITEM>>(_log.SMT_BOM_ITEM_SaveItem);
			Maintain.SMT_BOM_ITEM_SaveItem(main, items ,null, true);
		}

	}
}

