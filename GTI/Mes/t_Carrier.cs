using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using Genesis.Library.BLL.DTC;
using BLL.MES;

namespace UnitTestProject
{
	[TestClass]
	public class t_Carrier : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			//internal static string t_splitBIN
			//{
			//	get
			//	{
			//		return FileApp.ts_Log(@"Carrier\t_splitBIN.json");
			//	}
			//}
		}
		 
		[TestMethod]
		public void _DTC_AdjustmentCapacity()
		=> _DBTest(Txn => {
			var r = Carrier_Services.Check_CarrierCanUse(Txn , Txn.GetCarrierInfo("2-0001-18"));

		}, false);



		[TestMethod]
		public void _ddl_Carrier()
		{
			var x = DDLServices.ddl_Carrier();
		}


	}


}
