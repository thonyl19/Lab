using BLL.MES;
using MDL.MES;
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

			internal static string t_SaveAPI
			{
				get
				{
					return FileApp.ts_Log(@"EDC\t_SaveAPI.json");
				}
			}
		}


		[TestMethod]
		public void t_GetOperEdc()
		=> _DBTest(txn => { 
			txn.GetLotInfo("EB1N4B2B2006-01", true,true);
			var _list = WIPOperConfigServices.GetOperEdc(txn.DBC, txn.LotInfo, txn.GetRouteVerOper());
			new FileApp().Write_SerializeJson(_list, _log.t_GetOperEdc);
		}, true);


		[TestMethod]
		public void t_QC_INSP_EDC()
		{
			var r = QMSService.QC_INSP_EDC_seq("6CD91018-54FC-495F-9BEE-DAECA975E8F1");

			//Genesis.Library.BLL.ICM.Definition.Status.VerifyPlaning 
		}



	}


}
