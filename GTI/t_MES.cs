using BLL.MES;
using BLL.MES.DataViews;
using Genesis.Gtimes.ADM;
using Genesis.Library.BLL.MES.AutoGenerate;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.ADM.CarrierUtility;
using static Genesis.Gtimes.WIP.LotUtility;

namespace UnitTestProject
{
    [TestClass]
	public class t_MES : _testBase
	{
		static class _log
		{

            /// <summary>
            /// splitBIN 前端傳入的資料範例 
            /// </summary>
            internal static string t_OperationExpand_Update
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_OperationExpand_Update.json");
				}
			}

			internal static string t_GetOperLotAttributeData_OperSid
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_GetOperLotAttributeData_OperSid.json");
				}
			}
			internal static string t_GetCarrier
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_GetCarrier.json");
				}
			}

			internal static string t_Exchange_Carrier
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_Exchange_Carrier.json");
				}
			}

			/// <summary>
			///	因應 漢民專案,處理 載具進站 by Wefer 收 EDC 的樣例 
			/// </summary>
			internal static string t_DoCarrierCheckIn
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_DoCarrierCheckIn.json");
				}
			}

			

		}



		[TestMethod]
		public void t_() {
			var _data = FileApp.Read_SerializeJson<PF_OPERATION_EXPAND>(_log.t_OperationExpand_Update);
			var _svc = new OperationExpandServices();
			_svc.UpdateData(_data);
		}

 

		[TestMethod]
		public void t_GetRouteVerData()
		{
			//GTI20101517562109108
			var ROUTE_SID = "GTI20101517555209104";
			RouteUtility.RouteFunction fun = new RouteUtility.RouteFunction(this.DBC);
			DataTable dt = fun.GetRouteVersionDataTable(ROUTE_SID);
			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"MES\t_GetRouteVerData.json"));

		}


		[TestMethod]
		public void t_GetRouteOperData()
		{
			//GTI20101517562109108
			var routeversid = "GTI20101517562109108";
			RouteUtility.RouteFunction fun = new RouteUtility.RouteFunction(this.DBC);
			DataTable dt = fun.GetRouteVersionAllOperationAndSubRoute(routeversid);
			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"MES\t_GetRouteOperData.json"),isMult:false);

		}

        [TestMethod]
        public void t_x()
        {
			//BasePfRouteVerServices svcRouteVer = new BasePfRouteVerServices();

			//var ddl_RouteVer = (from t in svcRouteVer.GetAllListIQueryable()
			//                    where t.ROUTE_SID == Form.ROUTE_SID
			//                    select new
			//                    {
			//                        SID = t.ROUTE_SID,
			//                        No = t.ROUTE_NO,
			//                        Display = t.ROUTE,
			//                        Value = t.ROUTE_SID,
			//                        DEFAULT_FLAG = t.DEFAULT_FLAG
			//                    }).ToList();

			var x = BLL.MES.FluentValidation.PF_ROUTE.CheckOldRecord("GTI20030417271302053");

		}

		[TestMethod]
		public void t_GetOperLotAttributeData_OperSid()
		{
			//此範例 無法直接轉出資料,必須逐步控制
			var oper_sid = "GTI20101309004108383";
			LotAttributeUtility.LotAttributeFunction fun = new LotAttributeUtility.LotAttributeFunction(this.DBC);
			var dt = fun.GetOperLotAttributeData_OperSid (null,null, null, null, oper_sid);
			new FileApp().Write_SerializeJson(dt.Table,  _log.t_GetOperLotAttributeData_OperSid );


		}


		[TestMethod]
		public void t_GetCarrier()
		{
			var _no = "Cassette04";
			
			var _fn = new CarrierFunction(this.DBC);
			var CarrierInfo = _fn.GetCarrierByNo(_no);
			var Lots = _fn.GetCarrierLotTable(_no);
			new FileApp().Write_SerializeJson(Lots, _log.t_GetCarrier);

		}

		[TestMethod]
		public void t_parse_WAFER_MAPPING()
		{
			using (var dbc = new MDL.MESContext())
			{
				var _svc = new WIPServices();
				_svc.svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices () { DbContext = dbc };
				var lot = "CrystalGrowth_WO01-01.24";
				var _list = _svc.parse_WAFER_MAPPING(lot,"Scrap");
			}
		}

		[TestMethod]
		public void t_Exchange_Carrier()
		{
			using (var dbc = new MDL.MESContext())
			{
				var _svc = new Wafer_Services() { 
					svc_WIP = new WIPServices()
					{
						svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = dbc }
					}
				};
				var CarrierNo = "Cassette04";
				var _list = _svc.Exchange_CarrierInfo(CarrierNo);
				new FileApp().Write_SerializeJson(_list, _log.t_Exchange_Carrier);

			}

		}



        [TestMethod]
        public void t_DoCarrierCheckIn()
        {
			var svc_WIP = new WIPServices()
			{
				svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = new MDL.MESContext() }
			};
			var obj  = FileApp.Read_SerializeJson< WIPFormSendParameter>(_log.t_DoCarrierCheckIn);
			var txnInfo = new TxnDoItemInfo("TEST", this.DBC)
			{
				Data = obj,
			};
			txnInfo.LotInfo = new LotInfo(this.DBC, obj.Lot,IndexType.NO);

			txnInfo.Exec((_txn) =>
			{
				svc_WIP.Process_WaferPosiMap((TxnDoItemInfo)_txn);
			},true);
		}



	}


}
