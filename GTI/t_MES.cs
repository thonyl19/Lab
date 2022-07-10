using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code.Web.Select;
using Genesis;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction;
using Genesis.Gtimes.Transaction.CAR;
using Genesis.Gtimes.Transaction.EQP;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.DTC;
using Genesis.Library.BLL.MES.AutoGenerate;
using Genesis.Library.BLL.MES.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
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
			public static string t_依包材做拆批
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_依包材做拆批.json");
				}
			}


			/// <summary>
			/// CarrierInfo 的 資料結構
			/// </summary>
			public static string t_CarrierInfo
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_CarrierInfo.json");
				}
			}

			public static string t_LotSplit
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_LotSplit.json");
				}
			}

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

			internal static string t_DoCarrierCheckOut(string ext_name) { 
				return FileApp.ts_Log($@"MES\t_DoCarrierCheckOut{ext_name}.json");
			}

			/// <summary>
			/// 測試載具出站 - 包材分級的程序
			/// </summary>
			internal static string t_DoCarrierCheckOut_包材分級
			{
				get
				{
					return FileApp.ts_Log(@"MES\t_DoCarrierCheckOut_包材分級.json");
				}
			}



			/// <summary>
			///	因應 漢民專案,處理 載具進站 by Wefer 收 EDC 的樣例 
			/// </summary>
			internal static string t_DoCarrierCheckIn(string ext="")
			{
				return FileApp.ts_Log($@"MES\t_DoCarrierCheckIn{ext}.json");
			}



		}



		[TestMethod]
		public void t_()
		{
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
			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"MES\t_GetRouteOperData.json"), isMult: false);

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
			var dt = fun.GetOperLotAttributeData_OperSid(null, null, null, null, oper_sid);
			new FileApp().Write_SerializeJson(dt.Table, _log.t_GetOperLotAttributeData_OperSid);


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
				_svc.svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = dbc };
				var lot = "CrystalGrowth_WO01-01.24";
				//備忘:如果 SERIAL_NUMBER 有重覆就會出現如下錯誤 -- 已經加入含有相同索引鍵的項目 
				var _list = _svc.parse_WAFER_MAPPING(lot, "Scrap");
			}
		}

		[TestMethod]
		public void t_Exchange_Carrier()
		{
			using (var dbc = new MDL.MESContext())
			{
				var _svc = new Wafer_Services()
				{
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
		public void t_DoCarrierCheckIn_GRADE()
		{
			var svc_WIP = new WIPServices()
			{
				svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = new MDL.MESContext() }
			};
			var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_DoCarrierCheckIn("_GRADE"));

			var txnInfo = new TxnDoItemInfo("TEST", this.DBC)
			{
				Data = obj,
			};
			txnInfo.LotInfo = new LotInfo(this.DBC, obj.Lot, IndexType.NO);

			txnInfo.Exec((_txn) =>
			{
				svc_WIP.Process_WaferPosiMap((TxnDoItemInfo)_txn);
			}, true);
		}


		[TestMethod]
		public void t_DoCarrierCheckIn()
		{
			var svc_WIP = new WIPServices()
			{
				svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = new MDL.MESContext() }
			};
			var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_DoCarrierCheckIn());


			var txnInfo = new TxnDoItemInfo("TEST", this.DBC)
			{
				Data = obj,
			};
			txnInfo.LotInfo = new LotInfo(this.DBC, obj.Lot, IndexType.NO);

			txnInfo.Exec((_txn) =>
			{
				svc_WIP.Process_WaferPosiMap((TxnDoItemInfo)_txn);
			}, true);
		}

		[TestMethod]
		public void t_依包材做拆批11()
		{
			var svc_WIP = new WIPServices()
			{
				svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = new MDL.MESContext() }
			};
			//var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_DoCarrierCheckIn);
			var inputSrc = FileApp.Read_SerializeJson<Dictionary<string, WAFER_MAPPING>>(_log.t_依包材做拆批);
			var sqlcmd = new TransactionUtility.AddSQLCommandTxn();

			var grp_SHIP_CASSETTE = inputSrc.Select(t => t.Value)
				.GroupBy(o => o.SHIP_CASSETTE)
				.ToDictionary(o => o.Key, o =>
				{
					return new
					{
						list = o.ToList(),
						isGradValid = o.GroupBy(c => c.GRADE).Count() == 1,
						grpLot = o.GroupBy(c => c.LOT).ToDictionary(e => e.Key, e => e.ToList())
					};
				});

		}

		[TestMethod]
		public void t_process_Wafer_RangeTo_ShipCassette()
		{
			var svc_WIP = new WIPServices()
			{
				svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = new MDL.MESContext() }
			};
			var inputSrc = FileApp.Read_SerializeJson<Dictionary<string, WAFER_MAPPING>>(_log.t_依包材做拆批);
			new TxnBase("TEST", this.DBC)
				.Exec((_txn) =>
				{
					svc_WIP.process_Wafer_RangeTo_ShipCassette(_txn, inputSrc);
				}, true);
		}




		[TestMethod]
		public void _Txn_Scrap()
		{
			var _list = new List<QtyItem>() { new QtyItem() { Qty = 1, Reason_SID = "---" } };
			new TxnBase("TEST", this.DBC)
				.Exec((_txn) =>
				{
					_txn.GetLotInfo("WO_T074_001", true);
					LOT_Services.Txn_Scrap(_txn, _list);
				}, true);
		}




		[TestMethod]
		public void t_Txn_UnLoadLot()
		=> _DBTest((Txn) =>
		{
			var SplitList = new List<QtyItem>() { new QtyItem() { Qty = 1 } };
			EncodeFormatUtility.CodesInfo codes = null;
			Txn.GetLotInfo("GTI22050513264480527", true);

			var carrierip = Txn.LotInfo.GetCurrentCarrierInfo();


			var r = LOT_Services.Txn_LotSplit(Txn, SplitList);

			var lotInfo = Txn.GetLotInfo(r[0].LOT_SID);
			_Txn_UnLoadLot(Txn, lotInfo);

		}, true);

		public void _Txn_UnLoadLot(ITxnBase Txn, LotInfo lotInfo = null)
		{
			lotInfo = lotInfo ?? Txn.LotInfo;

			var equip = lotInfo.GetCurrentEquipmentInfo();
			if (equip.IsExist)
			{
				Txn.DoTransaction
					(new EQPTransaction.EquipmentUnloadLotTxn(equip, lotInfo)
					, new EQPTransaction.EndOfEquipmentTxn(equip));
			}

			var carrierip = lotInfo.GetCurrentCarrierInfo();
			if (carrierip.IsExist)
			{
				//var lot_Qty = int.Parse(lotInfo.QUANTITY.ToString());
				//int nowQtyp = int.Parse(carrierip.CURRENT_CAPACITY.ToString()) - lot_Qty;

				//if ((_Qty - lot_Qty) == 0)
				//{
				var carrierUnLoad = new CARTransaction.CarrierUnloadLotTxn(carrierip, lotInfo);
				Txn.DoTransaction(carrierUnLoad);

				carrierip = Txn.GetCarrierInfo(carrierip.No);
				if (carrierip.CURRENT_CAPACITY == 0)
				{
					var stateinfo = new CarrierStateInfo(Txn.DBC, "Idle", CarrierStateInfo.IndexType.No);
					var carrierState = new CARTransaction.CarrierChangeStateTxn(carrierip, stateinfo);
					Txn.DoTransaction(carrierState);
				}
				Txn.DoTransaction(new CARTransaction.EndOfCarrierTxn(carrierip));
				//}
				//else
				//{
				//	Txn.DoTransaction(new CARTransaction.CarrierChangeCapacityTxn(carrierip, nowQtyp));
				//}
			}
		}

		[TestMethod]
		public void t_Txn_載具出站_WaferGrade()
		=> _DBTest((Txn) => { 
			var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_DoCarrierCheckOut("_GRADE"));
			var posi = new Dictionary<string, WAFER_MAPPING>();
			foreach (var item in obj.PosiMap)
			{
				var point = JsonConvert.DeserializeObject<WAFER_MAPPING>(item.Value);
				posi.Add(item.Key, point);
				if (point.STATUS == "Scrap" && string.IsNullOrWhiteSpace(point.REASON)){
					throw new Exception($"Wafer(SN:{ point.SN }) ,{RES.BLL.Message.PleaseSelectReason}");
				}
			}
			Txn.DoTransaction(new Wafer.DTC_Process_WaferGrade(posi));

			

		},true);

		[TestMethod]
		public void _Txn_LotSplit()
		=> _DBTest((Txn) =>
		{
			var EnCode = "";
			//List<CustomerList> SplitList = new List<CustomerList>();
			var SplitList = new List<QtyItem>() { new QtyItem() { Qty = 1 } };
			EncodeFormatUtility.CodesInfo codes = null;
			Txn.GetLotInfo("GTI22050513264480527", true);

			var r = LOT_Services.Txn_LotSplit(Txn, SplitList);
			FileApp.WriteSerializeJson(r, _log.t_LotSplit);
			//var gtimesTxn = Txn.GtimesTxn;
			//var DBC = Txn.DBC;
			//var _lotInfo = Txn.LotInfo;
			//var SplitInfos = new List<LotSplitInfo>();

			//if (codes == null) {
			//	codes = Txn.GetEnCodes(LOT_Services.FunctionName.LotSplit, SplitList.Count);
			//}

			//var i = 0;
			//foreach (var item in SplitList)
			//{
			//	var splitLot = new LotSplitInfo(DBC.GetSID()
			//		, codes.Codes[i].ToString(), item.d_Qty);
			//	SplitInfos.Add(splitLot);
			//	i++;
			//}
			//WIPTransaction.LotSplitTxn oSplit = new WIPTransaction.LotSplitTxn(_lotInfo, SplitInfos);
			//oSplit.TerminateParentLot = true;//分批到零自動結批
			//Txn.DoTransaction(oSplit);
			//Txn.result.Data = new { SplitInfos };
		}, true);



		/// <summary>
		/// process_Wafer_RangeTo_ShipCassette 的原型
		/// </summary>
		public void t_依包材做拆批()
		{
			var svc_WIP = new WIPServices()
			{
				svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = new MDL.MESContext() }
			};
			//var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_DoCarrierCheckIn);
			var inputSrc = FileApp.Read_SerializeJson<Dictionary<string, WAFER_MAPPING>>(_log.t_依包材做拆批);
			var sqlcmd = new TransactionUtility.AddSQLCommandTxn();

			var grp_SHIP_CASSETTE = inputSrc.Select(t => t.Value)
				.GroupBy(o => o.SHIP_CASSETTE)
				.ToDictionary(o => o.Key, o =>
				{
					return new
					{
						list = o.ToList(),
						isGradValid = o.GroupBy(c => c.GRADE).Count() == 1,
						grpLot = o.GroupBy(c => c.LOT).ToDictionary(e => e.Key, e => e.ToList())
					};
				});
			//var txnInfo = ;
			new TxnBase("TEST", this.DBC)
				.Exec((_txn) =>
				{
					grp_SHIP_CASSETTE.Any(c =>
					{
						if (c.Value.isGradValid == false) throw new System.Exception($"單一包材只允許一個等級(SHIP_CASSETTE_NO:{c.Key})");
						return false;
					});
					var dc_SHIP_CASSETTE = WIPServices.CountWaferBy_SHIP_CASSETTE(grp_SHIP_CASSETTE.Keys.ToList());
					var SHIP_CASSETTE_0 = grp_SHIP_CASSETTE.ElementAt(0);
					var is單批同包 = grp_SHIP_CASSETTE.Count == 1
						&& SHIP_CASSETTE_0.Value.grpLot.Count == 1;
					if (is單批同包)
					{
							//WIPInfoServices.process_SHIP_CASSETTE
							//		(_txn, inputSrc, dc_SHIP_CASSETTE
							//		, SHIP_CASSETTE_0.Key
							//		, SHIP_CASSETTE_0.Value.list);
						}
					else
					{
						foreach (var SHIP_CASSETTE in grp_SHIP_CASSETTE)
						{
							var grpByLOT = SHIP_CASSETTE.Value.grpLot;
							foreach (var grpLot in grpByLOT)
							{
									//拆批前,先檢查 當前的 是 單wafer/批 , 還是 多wafer/批
									var is單批多Wafer = WIPServices.CountWaferByLot(grpLot.Key) > 1;
								if (is單批多Wafer)
								{
									SHIP_CASSETTE curSHIP_CASSETTE = dc_SHIP_CASSETTE[SHIP_CASSETTE.Key];
									var srcUseCounts = grpLot.Value.Count;
									var isOutOfLimit = curSHIP_CASSETTE.Limit < (curSHIP_CASSETTE.Used + srcUseCounts);
									if (isOutOfLimit) throw new System.Exception($"包材({SHIP_CASSETTE.Key})超過可使用數量.");

									var SplitList = new List<CustomerList>() { new CustomerList() { INum = grpLot.Value.Count } }; ;
									var _lot = new LotInfo(_txn.DBC, grpLot.Key, IndexType.NO);
									_txn.LotInfo = _lot;
									List<LotSplitInfo> SplitInfoList = WIPServices.SplitLots(_txn, SplitList);
									var _newLot = _txn.GetLotInfo(SplitInfoList[0].LOT_SID);

										// 處理 子批註記
										foreach (var posi in grpLot.Value)
									{
										var _posi = inputSrc[posi.SN.ToString()];
										_posi.LOT = _newLot.LOT;
										_posi.LOT_SID = _newLot.SID;
										_posi.REASON = "單批多包材--多wafer/批";
									}
								}
									//else
									//{
									//	var _posi = inputSrc[grpLot.Value[0].SN.ToString()];
									//	_posi.REASON = "單批多包材--單wafer/批";
									//}
								}

								//WIPInfoServices.process_SHIP_CASSETTE
								//	(_txn
								//	, inputSrc
								//	, dc_SHIP_CASSETTE
								//	, SHIP_CASSETTE.Key
								//	, SHIP_CASSETTE.Value.list);
							}

					}
				}, true);



		}


		[TestMethod]
		public void t_CarrierInfo()
		{
			var CarrierInfo = new CarrierInfo
				(mes.dbc()
				, "Cassette04"
				, CarrierUtility.CarrierInfo.IndexType.No);
			FileApp.WriteSerializeJson(CarrierInfo, _log.t_CarrierInfo);
		}



		[TestMethod]
		public void t_fn()
		=> _DBTest((Txn) => {
			//var r = DDLServices.Reason("Hold");
		});

	}
}
