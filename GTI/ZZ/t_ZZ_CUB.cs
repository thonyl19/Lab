using AutoMapper;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Frame.Code.Excel;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction.TOL;
using Genesis.Library.BLL.DTC;
using Genesis.Library.BLL.MES.OperTask;
using Genesis.Library.BLL.WRP;
using Genesis.Library.Frame.Code.Web.TableQuery;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static Genesis.Library.BLL.ADM.BomServices;
using _Prd = Genesis.Library.BLL.MES.OperTask;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;
using BLL.DataViews.Edc;
using static Genesis.Library.BLL.DTC.Lot;
using static Genesis.Gtimes.WIP.LotUtility;
using Genesis.Library.BLL.ADM;
using Genesis.Gtimes.Transaction.WIP;
using System.Data.Entity;
using Genesis.Library.BLL.MES.OperInfo;
using Genesis.Library.BLL.ZZ.CUB.OperTask;
using System.Threading;
using System.Globalization;
using Genesis.Gtimes.Common;
using BLL.MVC;
using Genesis.Library.BLL.ZZ.CUB;
using System.IO;
using Genesis;
using Genesis.Library.BLL.QMS;
//using _bllSvc = Genesis.Library.BLL.ZZ.CUB;
using _zz_OperInfo = Genesis.Library.BLL.ZZ.CUB.OperInfo;
using static Genesis.Library.BLL.ZZ.CUB.OperInfo.Basic;
using Newtonsoft.Json.Linq;

namespace UnitTestProject
{
	/// <summary>
	/// </summary>
	[TestClass]
	public class t_ZZ_CUB : _testBase
	{
		static class _log
		{
			internal static string t_ParallelOper
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\CUB\t_ParallelOper.json");
				}
			}
			internal static string t_Base
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\CUB\t_Base.json");
				}
			}
			internal static string ParallelCheckIn
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\ParallelCheckIn.json");
				}
			}


			internal static string ParallelCheckOut
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\ParallelCheckOut.json");
				}
			}

			internal static string SIPQC_Paralle_Save
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\SIPQC_Paralle_Save.json");
				}
			}

			internal static string t_list_前站完成清單
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\t_list_前站完成清單.json");
				}
			}

			internal static string t_QcResultService_ListData
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\t_QcResultService_ListData.json");
				}
			}

			internal static string t_進出站檢驗單案例(string CaseName="")
			{
				return FileApp.ts_Log($@"ZZ/CUB\t_進出站檢驗單案例{CaseName}.json");
			}
		}



		[TestMethod]
		public void t_Base()
		{
			var t = new Basic("EMS20240614-001-02");
			t.isTest = true;
			t.Process();
			FileApp.WriteSerializeJson(t, _log.t_Base);
		}

		[TestMethod]
		public void t_ParallelOper()
		{
			var t = new ParallelOper("Jtest0717-01", "GTI24071610203627082");
			t.isTest = true;
			t.Process();

			//var t1 = new ParallelOper("JTest0717_10-01", LotStatus.Wait, "GTI24071610203627083");
			//t1.isTest = true;
			//t1.Process();

			//FileApp.WriteSerializeJson(t, _log.t_ParallelOper);
		}



		[TestMethod]
		public void t_ParallelCheckIn() {
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var _r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.ParallelCheckIn);
			new ParallelCheckIn().Process(_r, true);
		}




		[TestMethod]
		public void t_PARALLEL_History()
		=> _DBTest((txn) =>
		{
			var _d = txn.LzQuery.WIP.t併行工作站在製現況表.FirstOrDefault();
			var tx = new PARALLEL_History(txn.ActionReason, _d, _d);
			tx.lot_old = _d;
			txn.DoTransaction(tx);
		}, true, true);


		[TestMethod]
		public void t_PARALLEL_History1()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;

			var _d = txn.LzQuery.WIP.t併行工作站在製現況表.FirstOrDefault();
			txn.ILotInfo = _d;
			var newQuantity = _d.OPER_EXECUTED_QUANTITY + 10;
			var zz = new Column("OPER_EXECUTED_QUANTITY", _d.OPER_EXECUTED_QUANTITY, newQuantity);
			var cmd = Genesis.Library.BLL.DTC.Lot.UpdateLot(txn, _d, zz);
			txn.DoTransaction(cmd);


			var tx = new PARALLEL_History(txn.ActionReason, _d, _d);
			tx.lot_old = _d;
			txn.DoTransaction(tx);
		}, true, true);



		[TestMethod]
		public void t_LOT_OPER_PARALLEL_GOOD_QTY()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var svcWIP = txn.LzQuery.WIP;
			var _d = svcWIP.t併行工作站在製現況表.FirstOrDefault();
			txn.ILotInfo = _d;
			var _r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.ParallelCheckOut);
			var _eqp = GTI_helper.getEquipmentInfo(txn);

			var tx = new Genesis.Library.BLL.DTC.Lot.PARTIAL_PROGRESS_UPDATE(_d, _d, _r.GoodList[0]) {
				ActionUserInfo = GTI_helper.getUserInfo(txn),
			};
			txn.DoTransaction(tx);
		}, true, true);


		[TestMethod]
		public void t_LOT_OPER_PARALLEL_GOOD_QTY__測試有SN_20241027_old()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var svcWIP = txn.LzQuery.WIP;
			var _d = svcWIP.t併行工作站在製現況表.FirstOrDefault();
			txn.ILotInfo = _d;
			var _r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.ParallelCheckOut);
			var _eqp = GTI_helper.getEquipmentInfo(txn);

			var tx = new Genesis.Library.BLL.DTC.Lot.PARTIAL_PROGRESS_UPDATE(_d, _d, _r.GoodList[0], true)
			{
				ActionUserInfo = GTI_helper.getUserInfo(txn),
				eqp = _eqp
			};
			txn.DoTransaction(tx);
		}, true, true);

		/// <summary>
		/// 因為改了新作法 , 所以押上日期以識別
		/// </summary>
		[TestMethod]
		public void t_LOT_OPER_PARALLEL_GOOD_QTY__測試有SN_20241027()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			//var svcWIP = txn.LzQuery.WIP;
			//var _d = svcWIP.t併行工作站在製現況表.FirstOrDefault();
			//txn.ILotInfo = _d;
			//var _r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.ParallelCheckOut);
			//var _eqp = GTI_helper.getEquipmentInfo(txn);
			var _d = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == "GTI24071715395227807");

			var tx = new Genesis.Library.BLL.DTC.Lot.PARTIAL_PROGRESS_UPDATE(_d, _d, null, true)
			{
				ActionUserInfo = txn.GetUserInfo("Admin"),//.getUserInfo(txn),
														  //eqp = _eqp
			};
			txn.DoTransaction(tx);
		}, true, true);

		[TestMethod]

		public void t_Columns_HistExecutedQty()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var svcWIP = txn.LzQuery.WIP;
			var _d = svcWIP.t併行工作站在製現況表.FirstOrDefault();
			var x = _d.Columns_HistExecutedQty(_d);
		}, true, true);

		[TestMethod]

		public void t_Parallel_InOut()
		=> _DBTest((txn) =>
		{
			var zz = new SearchServices() { _Txn = txn };
			zz.Parallel_InOut("PAR$GTI24071713503027675");
		}, true, true);



		[TestMethod]

		public void t_GetCarrierInfo()
		=> _DBTest((txn) =>
		{
			var z = txn.GetCarrierInfo();
		}, true, true);


		[TestMethod]
		public void t_UserTraceExited()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var ILot = txn.LzQuery.WIP.f批號平行工作站_是否存在("Jtest0717-01", "GTI24071610203627082");
			ApiService.UserTraceExit(txn, "Admin", ILot, null);
		}, true, true);


		[TestMethod]
		public void t_UserTraceExited_1() {
			ApiService.UserTraceExit("Admin", "Jtest0717-01", "GTI24071610203627082", true);
		}

		public class d_SIPQC_Paralle_Save
		{
			public WP_IPQC form { get; set; }
			public List<WP_IPQC_LOT> lot_list { get; set; }
			public List<EdcModel> data_input { get; set; }
		}
		[TestMethod]
		public void t_SIPQC_Paralle_Save()
		{
			TxnBase.Test = GTI_TxnBase.IPQC;
			var _r = FileApp.Read_SerializeJson<d_SIPQC_Paralle_Save>(_log.SIPQC_Paralle_Save);
			QMSService.IPQC_Save(_r.form, _r.lot_list, _r.data_input, true, true);
		}


		[TestMethod]
		public void t_有序工站檢核_有序號項目是否執行過前一站()
		=> _DBTest((txn) =>
		{

			var is有序工單 = true;
			var is有序號項目 = true;
			//var 取得當前站前一站;
			var svcWIP = txn.LzQuery.WIP;
			//var lot = txn.GetLotInfo();
			//var oper = lot.GetRouteVersionOperationInfo();
			var _cur_oper = new RouteUtility.RouteVersionOperationInfo(txn.DBC, "GTI24083010222641572");
			var isNeedCheckPrevOper = _cur_oper.IS_START == "F";
			if (isNeedCheckPrevOper)
			{
				var oper_pre = _cur_oper.GetPrevDefaultRouteVersionOperationInfo();
				List<string> 當前的執行清單 = new List<string>() { "Serial1", "Serial2" };
				List<WP_LOT_OPER_PARALLEL_SN> list_前站完成清單 = svcWIP.t併行工作站_SN過站記錄檔
					.Where(c => c.ROUTE_VER_OPER_SID == oper_pre.ROUTE_VER_OPER_SID && c.LOT == "JTest0717_7")
					.ToList();

				//FileApp.WriteSerializeJson(list_前站完成清單, _log.t_list_前站完成清單);

				//chk_前站未完成項目(當前的執行清單, list_前站完成清單);

			}


		}, true);


		[TestMethod]
		public void t_SequenceWO_Check_SN()
		{
			var lot = "JTest0717_7-01";
			var r = ApiService.SequenceWO_Check_SN("GTI24071610203527080", lot, "Serial2");
			var r1 = ApiService.SequenceWO_Check_SN("GTI24071610203627082", lot, "Serial1");

		}


		[TestMethod]
		public void t_SequenceWO_LotMove_SN()
		{
			var lot = "JTest0717_15-01";
			var SN = "Serial1";
			var ROUTE_VER_OPER_SID = "GTI24071610203527080";
			TxnBase.Test = ((Txn, ActionName, Link_SID) => {
				var WP_LOT_OPER_PARALLEL_SN = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN
					 .Where(c => c.ROUTE_VER_OPER_SID == ROUTE_VER_OPER_SID && c.SN == SN)
					 .ToList();

				var LOT = new
				{
					WP_LOT_OPER_PARALLEL_SN,
					Txn.result.Data
				};
				string json = JsonConvert.SerializeObject(LOT, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText(GTI_Test.g_path.t_Process, json);
			});
			var r = ApiService.SequenceWO_LotMove_OnePicec(ROUTE_VER_OPER_SID, lot, SN, "Admin", null, true);
		}


		[TestMethod]
		public void t_ParallelSN_Del()
		{
			var lot = "JTest0717_7-01";
			var SN = "Serial1";
			var ROUTE_VER_OPER_SID = "GTI24071610203527080";
			TxnBase.Test = ((Txn, ActionName, Link_SID) => {
				var WP_LOT_OPER_PARALLEL_SN = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN
					 .Where(c => c.ROUTE_VER_OPER_SID == ROUTE_VER_OPER_SID && c.SN == SN)
					 .ToList();

				var LOT = new
				{
					WP_LOT_OPER_PARALLEL_SN,
					AD_LOG = Txn.EFQuery_MES.AD_LOG.Where(c => c.ACTION == "DELETE" && c.TARGET_PK == SN)
						.OrderByDescending(c => c.CREATE_DATE).FirstOrDefault(),
				};
				string json = JsonConvert.SerializeObject(LOT, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText(GTI_Test.g_path.t_Process, json);
			});
			var r_刪除併行工站 = ApiService.ParallelSN_Del(ROUTE_VER_OPER_SID, lot, SN, "Admin", false, true);
			var r_刪除循序工單 = ApiService.ParallelSN_Del(ROUTE_VER_OPER_SID, lot, SN, "Admin", true, true);
		}

		[TestMethod]
		public void t_ParallelSN_Creat()
		{
			var lot = "JTest0717_7-01";
			var SN = "Serial1";
			var ROUTE_VER_OPER_SID = "GTI24071610203527080";
			TxnBase.Test = ((Txn, ActionName, Link_SID) => {
				var WP_LOT_OPER_PARALLEL_SN = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN
					 .Where(c => c.ROUTE_VER_OPER_SID == ROUTE_VER_OPER_SID && c.SN == SN)
					 .ToList();

				var LOT = new
				{
					WP_LOT_OPER_PARALLEL_SN,
				};
				string json = JsonConvert.SerializeObject(LOT, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText(GTI_Test.g_path.t_Process, json);
			});
			var r_可新增 = ApiService.ParallelSN_Creat(ROUTE_VER_OPER_SID, lot, $"{SN}5", "Admin", null, true);
			var r_不可新增 = ApiService.ParallelSN_Creat(ROUTE_VER_OPER_SID, lot, SN, "Admin", null, true);
		}


		[TestMethod]
		public void _ParallelSN_Creat()
		=> _DBTest((txn) => {
			//隨便取得一筆 SN 當測試鍵值
			var sn = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.FirstOrDefault();
			if (sn != null) {
				var r_不可新增 = ApiService.ParallelSN_Creat(sn.ROUTE_VER_OPER_SID, sn.LOT, sn.SN, sn.OP, null, true);
				Assert.IsFalse(r_不可新增.Success, "在相同條件下,不應該可以新增");

				var r_可新增 = ApiService.ParallelSN_Creat(txn, sn.ROUTE_VER_OPER_SID, sn.LOT, $"{sn.SN}5", sn.OP, null);
				Assert.IsTrue(r_不可新增.Success, "應該要可以新增");
				txn.Txn.Commit();

				var r_可新增1 = ApiService.ParallelSN_Del(txn, sn.ROUTE_VER_OPER_SID, sn.LOT, $"{sn.SN}5", sn.OP, false);
				Assert.IsTrue(r_不可新增.Success, "應該可以刪除");
			}
		}, true, true);

		[TestMethod]
		public void _有序工站序號刪除_項目已刷過下站_不允許刪除()
		=> _DBTest((txn) => {
			/*
			 取得測試 lot
			 取得 lot 的流程工站
			 試寫一筆 lot_move
			 試寫一站 lot_move
			 試著刪除 第一站記錄 
			 */                //隨便取得一筆 SN 當測試鍵值
			var sn = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.FirstOrDefault();
			if (sn != null)
			{
				var oper = new RouteUtility.RouteVersionOperationInfo(txn.DBC, sn.ROUTE_VER_OPER_SID);
				var opers = oper.GetRouteVersionInfo().GetRouteVersionOperationList();

				var _sn = $"{sn.SN}_T";
				var r_第一筆LotMove = ApiService.SequenceWO_LotMove_OnePicec(txn, opers[0].ROUTE_VER_OPER_SID, sn.LOT, _sn, sn.OP);
				var r_第二筆LotMove = ApiService.SequenceWO_LotMove_OnePicec(txn, opers[1].ROUTE_VER_OPER_SID, sn.LOT, _sn, sn.OP);
				Assert.IsTrue(r_第一筆LotMove.Success && r_第二筆LotMove.Success, "應該新增成功");

				var r_不可刪除 = GTI_helper.CatchEx(() => ApiService.ParallelSN_Del(txn, opers[0].ROUTE_VER_OPER_SID, sn.LOT, _sn, sn.OP, true));
				Assert.IsTrue(r_不可刪除.Message.Contains("已有下一站的過站記錄,不允許刪除"), "應該不可以刪除");
			}
		}, true, true);

		[TestMethod]
		public void _有序工站序號刪除_LotMove_OnePicec()
		=> _DBTest((txn) => {
			var sn = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.FirstOrDefault();
			if (sn != null)
			{
				var oper = new RouteUtility.RouteVersionOperationInfo(txn.DBC, sn.ROUTE_VER_OPER_SID);
				var opers = oper.GetRouteVersionInfo().GetRouteVersionOperationList();

				var _sn = $"{sn.SN}_T";
				var t_查無執行記錄 = GTI_helper.CatchEx(() => ApiService.SequenceWO_LotMove_OnePicec(txn, opers[1].ROUTE_VER_OPER_SID, sn.LOT, _sn, sn.OP));
				Assert.IsTrue(t_查無執行記錄.Message.Contains("查無執行記錄")
					, "故意用新序號從第二站新增,應該有這個錯誤");

				var t_本站已有生產記錄 = GTI_helper.CatchEx(() => ApiService.SequenceWO_LotMove_OnePicec(txn, sn.ROUTE_VER_OPER_SID, sn.LOT, sn.SN, "Admin"));
				Assert.IsTrue(t_本站已有生產記錄.Message.Contains("本站已有生產記錄")
					, "故意用不同OP建立原始記錄 SN,應該有這個錯誤");

				var t_第一筆LotMove = GTI_helper.CatchEx(() => ApiService.SequenceWO_LotMove_OnePicec(txn, opers[0].ROUTE_VER_OPER_SID, sn.LOT, _sn, sn.OP));
				Assert.IsTrue(t_第一筆LotMove.Success, "應該新增成功");

				var t_查不到上一站生產記錄 = GTI_helper.CatchEx(() => ApiService.SequenceWO_LotMove_OnePicec(txn, opers[2].ROUTE_VER_OPER_SID, sn.LOT, _sn, sn.OP));
				Assert.IsTrue(t_查不到上一站生產記錄.Message.Contains("查不到上一站生產記錄")
					, "故意用第三站建立,應該要有這個錯誤");

			}
		}, true, true);



		/* 測試模型
		 
		[TestMethod]
		public void _有序工站檢核_有序號項目是否執行過前一站_() {
			List<string> 當前的執行清單 = new List<string>() { "Serial1", "Serial2" };

			var list_前站完成清單 =  FileApp.Read_SerializeJson<List<WP_LOT_OPER_PARALLEL_SN>>(_log.t_list_前站完成清單);
			var r = chk_前站未完成項目(當前的執行清單, list_前站完成清單);
			Assert.IsTrue(r.Count == 0,"清單檢核結果應該要為零");

			當前的執行清單.Add("Serial3");

			var r1 = chk_前站未完成項目(當前的執行清單, list_前站完成清單);
			Assert.IsTrue(r1.Count == 1 && r1[0] == "Serial3", "清單檢核結果應該要有一筆 -Serial3");
		}

		private static List<string> chk_前站未完成項目(List<string> 當前的執行清單, List<WP_LOT_OPER_PARALLEL_SN> list_前站完成清單)
		{
			return 當前的執行清單
				.Where(c => !list_前站完成清單.Any(e => e.SN == c))
				.ToList();
		}

		
        [TestMethod]
		public void t_有序工站檢核_無序號項目是否執行過前一站()
        {
            var is有序工單 = true;
            var is有序號項目 = false;

            //var 取得流程的前一站;
            var 取得前一站己完成數 = 20;
            var 當前已過數量 = 10;
            var 當前報工數量 = 10;

			Assert.IsTrue(GetIs可報工(取得前一站己完成數, 當前已過數量, 當前報工數量));
			Assert.IsFalse(GetIs可報工(20, 20, 10));
			Assert.IsFalse(GetIs可報工(0, 20, 10));
        }

        private static bool GetIs可報工(int 取得前一站己完成數, int 當前已過數量, int 當前報工數量)
        {
            return 取得前一站己完成數 >= 當前已過數量 + 當前報工數量;
        }
		*/


		[TestMethod]
		public void t_QcResultService_ListData()
		{
			var _PagerQuery = FileApp.Read_SerializeJson<BLL.DataViews.Res.PagerQuery>(_log.t_QcResultService_ListData);
			var r = QcResultService.ListData(_PagerQuery, false);
			FileApp._tmpJson(r);
		}



		[TestMethod]
		public void t_fn()
		=> _DBTest((txn) =>
		{
			var zz = txn.EFQuery_MES.f檢驗單_查詢檢驗和覆確結果("INSP24111002").FirstOrDefault();
		}, false, true);


		[TestMethod]
		public void t_fnA()
		=> _DBTest((_Txn) =>
		{
			/*

			var _qc_form = (from a in _Txn.EFQuery_MES.PF_OPERATION_INSP
							where a.OPER_SID == iLot.OPER_SID
							select a).ToList();
			if (_qc_form.Count == 0) return;


			var _Finished = nameof(RES.BLL.Face.Finished);
			var _Reject = nameof(RES.BLL.Face.Reject);
			var _Accept = nameof(RES.BLL.Face.Accept);
			var _qc_result = (from w0 in _Txn.EFQuery_MES.WP_IPQC
							  where _Txn.EFQuery_MES.WP_IPQC_LOT
									  .Any(w00 => w00.LOT_SID == iLot.SID
										  && w00.ROUTE_VER_OPER_SID == iLot.ROUTE_VER_OPER_SID
										  && w00.QC_NO == w0.QC_NO)
							  join z1 in _Txn.EFQuery_MES.ZZ_INSP_RECHECK on w0.QC_NO equals z1.QC_NO into gj
							  from z1 in gj.DefaultIfEmpty()
							  orderby z1.UPDATE_DATE descending
							  select new InspectionResult
							  {
								  INSP_SID = w0.INSP_SID,
								  QC_NO = w0.QC_NO,
								  QC_INSP_TYPE = w0.QC_INSP_TYPE,
								  INSP_STATUS = w0.INSP_STATUS,
								  QC_RESULT = w0.QC_RESULT,
								  RECHECK_SID = z1.SID,
								  RESULT = z1.RESULT,
								  STATUS = z1.STATUS,
								  UPDATE_USER = z1.UPDATE_USER,
								  UPDATE_DATE = z1.UPDATE_DATE
							  }).ToList();

			var z = new { iLot, _qc_form, _qc_result };
			FileApp.WriteSerializeJson(z, _log.t_進出站檢驗單案例());
			//*/

			var iLot = _Txn.GetLotInfo("EMS20240614-001-01", isQueryByLotNO: true);
			_zz_OperInfo.Basic.基本檢核_站別檢驗單卡控(_Txn, iLot);

			//var _lot1 = _Txn.GetLotInfo("CE201214020-01", isQueryByLotNO: true);
			//_zz_OperInfo.Basic.基本檢核_檢驗單卡控(_Txn, _lot1.SID);


			//         var _lot2 = _Txn.GetLotInfo("EMSWO-20240602-001-01", isQueryByLotNO: true);
			//_zz_OperInfo.Basic.基本檢核_站別檢驗單卡控_old(_Txn, _lot2.SID ,_lot2.ROUTE_VER_OPER_SID ,isPrepairCheck:true);




		}, false, true);

		public class _d_進出站檢驗單案例 {
			//public Genesis.Gtimes.Common.ILotInfo iLot { get; set; }
			public WP_LOT_OPER_PARALLEL iLot { get; set; }
			public List<PF_OPERATION_INSP> _qc_form { get; set; }
			public List<InspectionResult> _qc_result { get; set; }
		}



		[TestMethod]
        public void t_進出站檢驗單案例()
        {

            //var z = FileApp.Read_SerializeJson<_d_進出站檢驗單案例>(_log.t_進出站檢驗單案例());
            //_zz_OperInfo.Basic.NewMethod(z.iLot, z._qc_form, z._qc_result);

			var z1 = FileApp.Read_SerializeJson<_d_進出站檢驗單案例>(_log.t_進出站檢驗單案例("-進站,雙檢驗單"));
			//_zz_OperInfo.Basic.基本檢核_站別檢驗單卡控_檢核邏輯(z1.iLot, z1._qc_form, z1._qc_result);
		}

		//[TestMethod]
		//public void t_站別檢驗單_人員進出站檢核()
  //      => _DBTest((txn) =>
  //      {
		//	var _lot1 = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
		//		.FirstOrDefault(c=>c.LOT_SID == "GTI24110411262869530");

		//	var zz = _zz_OperInfo.Basic.站別檢驗單_人員進出站檢核(txn, _lot1,"Admin",true,true);
		//}, true);


		[TestMethod]
		public void t_UserTraceEnter()
        => _DBTest((txn) =>{
			var t = new ApiService().UserTraceEnter(txn, "Admin", "WORK_CENTER_CHW0_010100", "300036263-0010-01", "GTI24110411121869361",true);
		}, true,true);
		


		[TestMethod]
		public void t_IPQC_LotInfo_CUB()
		{

			//var r = QMSService.IPQC_LotInfo_CUB("EMSWO-20240603-001-01");

		}


        [TestMethod]
        public void t_fnxx()
		=> _DBTest((txn) =>
		{
			var t = WIPOperConfigServices.GetOperTools("GTI24071610151727020",txn.DBC);
		}, false);


		[TestMethod]
		public void t_GetMachineTool()
		{
			var t = Genesis.Library.BLL.ZZ.CUB.ApiService.GetMachineTool
			("JTest0717_14-01","GTI24112418411105058","GTI24112418411105058");
		}


		[TestMethod]
        public void t_PF_OPERATION_EXPAND()
		=> _DBTest((txn) =>
		{
			var x = txn.EFQuery_MES.PF_OPERATION_EXPAND.ToList();
			var g = x.Where(c=>{
				dynamic data = JsonConvert.DeserializeObject(c.SETTING_JSON);
				string propertyName = "IsCheckFeeder";
				if (FindProperty(data.CheckInSet, propertyName) ||
					FindProperty(data.CheckOutSet, propertyName) ||
					FindProperty(data.AnySet, propertyName))
				{
					return true;
				}
				return false;
			}).ToList();
			FileApp._tmpJson(g);
		}, false);

		public bool FindProperty(dynamic obj, string propertyName)
		{
			if (obj is JObject jobj){
				foreach (var property in jobj)
				{
					var isSameName = property.Key.ToString() == propertyName;
					if (isSameName && (bool)property.Value)
					{
						return true;
					}else if (property.Value is JObject){
						if (FindProperty(property.Value, "enable")){
							return true;
						}
					}
				}
			}
			return false;
		}



        [TestMethod]
        public void t_Tool使用數計算()
        {
			var avg_ToolUseCount = (decimal) 6 / 3;
			var int_取得整數 = (int)Math.Floor(avg_ToolUseCount);
			var int_取得餘數 = (int)Math.Ceiling(avg_ToolUseCount - int_取得整數);
		}

		[TestMethod]
		public void t_SequenceWO_Check_Qty()
		=> _DBTest((txn) =>{
			var lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c=>c.LOT_SID == "GTI24121113585207089");
			ApiService.SequenceWO_Check_Qty(txn, lot, 10);
		}, false);
	}



}

