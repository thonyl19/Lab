using BLL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject
{
	[TestClass]
	public class t_EDC : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_GetOperEdc
			{
				get
				{
					return FileApp.ts_Log(@"EDC\t_GetOperEdc.json");
				}
			}
		}


		[TestMethod]
		public void t_GetOperEdc()
		{
			var txn = new TxnBase("test", this.DBC);
			txn.GetLotInfo("GTI21011209000088768", true);
			var _list = WIPOperConfigServices.GetOperEdc(txn.DBC, txn.LotInfo, txn.GetRouteVerOper());
			new FileApp().Write_SerializeJson(_list, _log.t_GetOperEdc);

		}



	}


}
