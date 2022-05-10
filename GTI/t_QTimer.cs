using BLL.MES;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.MES.AutoGenerate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Linq.Dynamic.Core;
using UnitTestProject.TestUT;
using mdl = MDL.MES;
using vDbCtx = MDL.MESContext;

namespace UnitTestProject
{
	[TestClass]
	public class t_QTimer : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_GetWpChecktimeData
			{
				get
				{
					return FileApp.ts_Log(@"QTimer\t_GetWpChecktimeData.json");
				}
			}
		}



		[TestMethod]
		public void t_GetWpChecktimeData()
		{
			
			using (var cnn = new MDL.MESContext())
			{
				var example1 = cnn.WP_LOT_CHECKTIME
					.Where("LOT_SID == @0 ", "GTI20021013194301437")
					.ToList();

				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}
		[TestMethod]
		public void t_() {
			//var x = new LotUtility.LotInfo(mes.dbc(), "WO_T059-004", LotUtility.IndexType.NO);
			//var x1 = x.GetRouteVersionOperationInfo();
			var _r = TableQueryService.WP_LOT_CHECKTIME("WO_T059-004", "PSI-R002").ToList();


		}


		[TestMethod]
		public void t_GetCheckQTimeOperParaData()
		{
			using (var dbc = mes.dbc())
			{
				var _lotInfo = new LotUtility.LotInfo(dbc, "WO_T059-004", LotUtility.IndexType.NO);

				var chkFun = new CheckTimeUtility.CheckTimeFunctions(dbc);
				var dvOperSet = chkFun.GetCheckQTimeOperParaData
					(_lotInfo.WO
					, _lotInfo.ROUTE_VER_SID
					, _lotInfo.ROUTE_VER_OPER_SID
					, _lotInfo.PARTNO
					, _lotInfo.OPERATION);
				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}


		}


	}


}
