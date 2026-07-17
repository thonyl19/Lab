using BLL.DataViews.Edc;
using BLL.MES;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject
{
	[TestClass]
	public partial class t_EDC : _testBase
	{
		static class _log
		{
			
			internal static string t_GetOperEdc
			{
				get
				{
					return FileApp.ts_Log(@"EDC\t_GetOperEdc.json");
				}
			}
			internal static string t_GetEDC_Data
			{
				get
				{
					return FileApp.ts_Log(@"EDC\t_GetEDC_Data.json");
				}
			}
			internal static string t_edcData
			{
				get
				{
					return FileApp.ts_Log(@"EDC\t_edcData.json");
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
			//var r = QMSService.QC_INSP_EDC_seq("6CD91018-54FC-495F-9BEE-DAECA975E8F1");

			//Genesis.Library.BLL.ICM.Definition.Status.VerifyPlaning 
		}

		[TestMethod]
		public void t_GetEDC_Data()
		=> _DBTest(txn => {
			var QC_NO = "22";
			var x = QMSService.GetEDC_Data(QC_NO);
			new FileApp().Write_SerializeJson(x, _log.t_GetEDC_Data);
		}, true,true);


		[TestMethod]
		public void t_edcData_check()
		=> _DBTest(txn => {
			var src = FileApp.Read_SerializeJson<List<EdcModel>>(_log.t_edcData);
			//var x = QMSService.GetEDC_Data(QC_NO);
		}, true, true);

	}


}
