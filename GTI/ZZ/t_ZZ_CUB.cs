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
using _zzAPI = Genesis.Library.BLL.ZZ.CUB.ApiService;
using _zzCodeRule = Genesis.Library.BLL.ZZ.CUB.CodeRule;
using static Genesis.Library.BLL.ZZ.CUB.OperInfo.Basic;
using Newtonsoft.Json.Linq;
using Genesis.Gtimes.Transaction.EQP;
using Genesis.Gtimes.Transaction.CAR;
using Dal.Repository;
using Microsoft.EntityFrameworkCore;
using Genesis.Library.BLL;
using Genesis.Library.BLL.MES.AutoGenerate;
using Frame.Code.Web.Select;

using Genesis.Library.BLL.ZZ.CUB.CodeRule;
using Genesis.Library.BLL.PMS.Definition;
using BLL.PMS;

namespace UnitTestProject
{
	/// <summary>
	/// </summary>
	[TestClass]
	public partial class t_ZZ_CUB : _testBase
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

			internal static string t_allFeeders
			{
				get
				{
					return FileApp.ts_Log($@"ZZ/CUB\t_allFeeders.json");
				}
			}

			internal static string t_EQP_Tool_計算
			{
				get
				{
					return FileApp.ts_Log($@"ZZ/CUB\t_EQP_Tool_計算.json");
				}
			}

			internal static string t_Portal_ListData
			{
				get
				{
					return FileApp.ts_Log($@"ZZ/CUB\t_Portal_ListData.json");
				}
			}

			internal static string t_基本檢核_站別檢驗單卡控
			{
				get
				{
					return FileApp.ts_Log($@"ZZ/CUB\t_基本檢核_站別檢驗單卡控.json");
				}
			}

			internal static string t_EqpToolList
			{
				get
				{
					return FileApp.ts_Log(@"Operation\t_EqpToolList.json");
				}
			}

			internal static string t_UserGroupStepLicenseData
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\t_UserGroupStepLicenseData.json");
				}
			}

			internal static string t_consumedMaterialInfo
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\t_consumedMaterialInfo.json");
				}
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
			//QMSService.IPQC_Save(_r.form, _r.lot_list, _r.data_input, true, null,true);
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
			var r_可新增 = ApiService.ParallelSN_Creat(ROUTE_VER_OPER_SID, lot, $"{SN}5", "Admin", null ,null, true);
			var r_不可新增 = ApiService.ParallelSN_Creat(ROUTE_VER_OPER_SID, lot, SN, "Admin", null,null, true);
		}


		[TestMethod]
		public void _ParallelSN_Creat()
		=> _DBTest((txn) => {
			//隨便取得一筆 SN 當測試鍵值
			var sn = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.FirstOrDefault();
			if (sn != null) {
				var r_不可新增 = ApiService.ParallelSN_Creat(sn.ROUTE_VER_OPER_SID, sn.LOT, sn.SN, sn.OP, null, null,true);
				Assert.IsFalse(r_不可新增.Success, "在相同條件下,不應該可以新增");

				var r_可新增 = ApiService.ParallelSN_Creat(txn, sn.ROUTE_VER_OPER_SID, sn.LOT, $"{sn.SN}5", sn.OP, null);
				Assert.IsTrue(r_不可新增.Success, "應該要可以新增");
				txn.Txn.Commit();

				var r_可新增1 = ApiService.ParallelSN_Del(txn, sn.ROUTE_VER_OPER_SID, sn.LOT, $"{sn.SN}5", sn.OP, false);
				Assert.IsTrue(r_不可新增.Success, "應該可以刪除");
			}
		}, true, true);

		[TestMethod]
		public void _f站別檢驗單設定()
		=> _DBTest((txn) => {
			int z = 1;
			var z1 = z.ts_NullEnum<Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機>();

			//隨便取得一筆 SN 當測試鍵值
			var sn = txn.EFQuery_MES.f站別檢驗單設定
			("GTI24122418180688120"
			, z1);
		}, false, true);

		

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
		public void t_Portal_ListData()
		{
			var _PagerQuery = FileApp.Read_SerializeJson<BLL.DataViews.Res.PagerQuery>(_log.t_Portal_ListData);
			var r = PMSJobImplementServices.Portal_ListData(_PagerQuery);
			FileApp._tmpJson(r);
		}

		[TestMethod]
		public void t_TxnBase_T_保養單()
		=> _DBTest((txn) =>
		{
			GTI_Test.TxnBase_T_保養單(txn, null, "PMS2025103000000011");
		}, false, true);


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
			//_zz_OperInfo.Basic.基本檢核_站別檢驗單卡控(_Txn, iLot);

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
        public void t_GetOperTools()
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
		public void job_重設機台並下治具()
		=> _DBTest((txn) =>
		{
			var _eqpInfo = txn.GetEquipmentInfo("Test01", IndexType: EquipmentUtility.IndexType.No);

			// 取得機台上的治具
			var ToolsOfEqp = _eqpInfo.GetEquipmentLoadToolList();
			//var ToolsOfEqp = txn.EFQuery_MES.f取得機台上的治具清單(equip.No).ToList();
			if (ToolsOfEqp.Count != 0)
			{
				// 治具下機台
				txn.DoTransaction(new EQPTransaction.EquipmentUnloadToolTxn(_eqpInfo, ToolsOfEqp));
				foreach (var tool in ToolsOfEqp)
				{
					txn.DoTransaction(new TOLTransaction.EndOfToolTxn(tool));
				}
				txn.DoTransaction(new EQPTransaction.EndOfEquipmentTxn(_eqpInfo));
			}

			var EqpFn = new EquipmentUtility.EquipmentFunction(txn.DBC);
			var lots = txn.EFQuery_MES.f取得機台上的批號清單(_eqpInfo.SID).ToList();
			//DataTable lots = EqpFn.GetEquipmentLotList(_eqpInfo.No);

			foreach (var lot in lots)
			{
				_eqpInfo = _eqpInfo.ReLoad(txn.DBC);
				var _lotInfo = txn.GetLotInfo(lot.LOT_SID);
				txn.DoTransaction(
					new EQPTransaction.EquipmentUnloadLotTxn(_eqpInfo, _lotInfo),
					new EQPTransaction.EndOfEquipmentTxn(_eqpInfo)
				);
			}

			if (_eqpInfo.CURRENT_LOT_SIZE != 0 || _eqpInfo.CURRENT_CAPACITY != 0)
			{
				_eqpInfo = _eqpInfo.ReLoad(txn.DBC);
				txn.DoTransaction(
					new EQPTransaction.EquipmentChangeCapacityAndLotSizeTxn(_eqpInfo, 0, 0)
					{
						ChangeStatus = true
					}
				); ;
			}
		}, true);


		[TestMethod]
		public void t_SequenceWO_Check_Qty()
		=> _DBTest((txn) =>{
			var lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c=>c.LOT_SID == "GTI25010713253810209");
			var _qty = new CustomerList() { Qty = 1 , subItem = new List<SelectModel>() { 
				new SelectModel(){No = "20250203001"},
				new SelectModel(){No = "20250203004"},
				
			} };
			//ApiService.SequenceWO_Check_Qty(txn, lot, _qty);
			ApiService.SequenceWO_Check_Qty(txn, lot, _qty);
		}, false);

		[TestMethod]
		public void t_TTTT()
		=> _DBTest((Txn) => {

			var chk_Query = Txn.EFQuery_MES.ZZ_WP_PACKAGE_ITEM.Where(c => c.WP_LOT_SID == "GTI24071715395327809" && c.BARCODE == null && c.ACTION_LINK_SID == null);
			var is有空包裝資料 = chk_Query.Any();
			if (is有空包裝資料)
			{
				Txn.EFQuery_MES.ZZ_WP_PACKAGE_ITEM.RemoveRange(chk_Query);
				Txn.EFQuery_MES.SaveChanges();
			}
		}, true,true);


		[TestMethod]
		public void t_註消非U型站_上崗前檢驗單()
		=> _DBTest((Txn) => {
			var lot = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(x => x.LOT_SID == "GTI24122513471294962");
			_zzAPI._註消進站後上崗前檢驗單(Txn, lot, "Admin", "Test");
		}, true,true);

		public static void TxnBase_T_批號治具(ITxnBase Txn, string ActionName, string Link_SID)
		{
			var WP_LOT_OPER_PARALLEL = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
					.Where(c => c.LOT_SID == "GTI24122615505096899");

			var WP_TOOL_TRACE = Txn.EFQuery_MES.WP_TOOL_TRACE.GetData_ACTION_LINK_SID(Link_SID);
			var WP_EQP_TRACE = Txn.EFQuery_MES.WP_EQP_TRACE.GetData_ACTION_LINK_SID(Link_SID);
			var WP_CARRIER_TRACE = Txn.EFQuery_MES.WP_CARRIER_TRACE.GetData_ACTION_LINK_SID(Link_SID);

			var WP_LOT_TOOL_TRACE = Txn.EFQuery_MES.WP_LOT_TOOL_TRACE
				.Where(c => WP_LOT_OPER_PARALLEL.Any(c1 => c1.LOT_SID == c.LOT_SID));

			var WP_LOT_CARRIER_TRACE = Txn.EFQuery_MES.WP_LOT_CARRIER_TRACE
				.Where(c => WP_LOT_OPER_PARALLEL.Any(c1 => c1.LOT_SID == c.LOT_SID));

			var FC_TOOL = Txn.EFQuery_MES.FC_TOOL
					.Where(c => WP_LOT_TOOL_TRACE.Any(c1=>c1.TOOL_SID == c.TOOL_SID))
					.ToList();
			var FC_EQUIPMENT = Txn.EFQuery_MES.FC_EQUIPMENT
				.Where(c => WP_LOT_TOOL_TRACE.Any(c1 => c1.EQP_SID == c.EQP_SID))
				.ToList();

			var FC_CARRIER = Txn.EFQuery_MES.FC_CARRIER
				.Where(c => WP_LOT_CARRIER_TRACE.Any(c1 => c1.CARRIER_SID == c.CARRIER_SID))
				.ToList();

			var WP_EQP_TOOL_LIST = Txn.EFQuery_MES.WP_EQP_TOOL_LIST
				.Where(c => WP_LOT_TOOL_TRACE.Any(c1 => c1.EQP_SID == c.EQP_SID && c1.TOOL_SID == c.TOOL_SID))
				.ToList();
			//SMT_CARRIER_TOOL

			var LOT = new
			{
				Link_SID,
				WP_LOT_OPER_PARALLEL = WP_LOT_OPER_PARALLEL.ToList(),
				WP_TOOL_TRACE = WP_TOOL_TRACE.ToList(),
				WP_EQP_TRACE = WP_EQP_TRACE.ToList(),
				WP_CARRIER_TRACE = WP_CARRIER_TRACE.ToList(),
				FC_TOOL,
				FC_EQUIPMENT,
				FC_CARRIER,
				WP_LOT_TOOL_TRACE =WP_LOT_TOOL_TRACE.ToList(),
				WP_EQP_TOOL_LIST,
				Txn.result.Data
			};
			string json = JsonConvert.SerializeObject(LOT, Newtonsoft.Json.Formatting.Indented);
			File.WriteAllText(GTI_Test.g_path.t_Process, json);
		}

		[TestMethod]
		public void t_針對批號下設備和載具()
		=> _DBTest((Txn) => {
			TxnBase.Test = TxnBase_T_批號治具;
			var lot = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(x => x.LOT_SID == "GTI24122615505096899");
			var _取得批號身上的機台 = Txn.f取得批號身上的治具(lot)
				.GroupBy(c=>c.EQP_SID)
				.ToDictionary(c=>c.Key,c=>c.ToList());
			foreach (var EQP_Tool in _取得批號身上的機台) {
				var EqpInfo = Txn.GetEquipmentInfo(EQP_Tool.Key);
				foreach (var Tool in EQP_Tool.Value) { 
					var toolInfo = new ToolUtility.ToolInfo(Txn.DBC, Tool.TOOL_NO, ToolUtility.ToolInfo.IndexType.No);
					Txn.DoTransaction(
						new EQPTransaction.EquipmentUnloadToolTxn(EqpInfo, toolInfo),
						new TOLTransaction.EndOfToolTxn(toolInfo)
						,new EQPTransaction.EndOfEquipmentTxn(EqpInfo)
					);
				}
				EqpInfo = EqpInfo.ReLoad(Txn.DBC);
				Txn.DoTransaction(
					new EQPTransaction.EquipmentUnloadLotTxn(EqpInfo, lot),
					new EQPTransaction.EndOfEquipmentTxn(EqpInfo)
				);
			}
		}, true, true);



        [TestMethod]
        public void t_進站上料槍()
		=> _DBTest((txn) =>
		{
			var wo = "100031194";
			var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault();
			var _Used = nameof(Genesis.Library.BLL.ICM.Definition.Status.Used);
			var _Idle = nameof(Genesis.Library.BLL.ICM.Definition.Status.Idle);
			//WP_WO woInfo = BaseWpWoServices.GetEntityByExpression(x => x.WO == wo);
			var list_carrier = (from a in txn.EFQuery_MES.SMT_CARRIER_TOOL
						   join b in txn.EFQuery_MES.FC_CARRIER on a.CARRIER_NO equals b.CARRIER_NO
						   where b.STATE_NO == "Idle" && a.WO == wo
						   select b.CARRIER_NO).Distinct().ToList();
			var stateinfo = new CarrierUtility.CarrierStateInfo(txn.DBC, _Used, CarrierUtility.CarrierStateInfo.IndexType.No);
			Decimal useCount = 1;

			foreach (var carrier in list_carrier) {
				var carrierInfo = txn.GetCarrierInfo(carrier);
				Check.Invalid(string.Format(RES.BLL.Message.FeederShiftIsNotIdle, carrier), carrierInfo.STATE_NO != _Idle, carrierInfo);

				txn.DoTransaction
					( new CARTransaction.CarrierLoadLotTxn(carrierInfo, _lot)
					, new CARTransaction.CarrierChangeStateTxn(carrierInfo, stateinfo)
					, new CARTransaction.EndOfCarrierTxn(carrierInfo)
					);

				carrierInfo = txn.GetCarrierInfo(carrier);
				txn.DoTransaction(new CARTransaction.CarrierAddUseCountTxn(carrierInfo, useCount));

				//txn.DoTransaction(new SMT.InsertHistory(_lot, "T", _lot.SID));
			}
			txn.EFQuery_MES.SaveChanges();
		}, true);


        [TestMethod]
        public void t_批號下載具()
		=> _DBTest((txn) =>
		{
			var list = (from a in txn.EFQuery_MES.WP_LOT_CARRIER_TRACE
						where a.ACTION_LINK_SID == "GTI25020614353133998"
						select a
				).ToList();

			var _Idle = nameof(Genesis.Library.BLL.ICM.Definition.Status.Idle);
			var stateinfo = new CarrierUtility.CarrierStateInfo(txn.DBC, _Idle, CarrierUtility.CarrierStateInfo.IndexType.No);

			foreach (var a in list) {
				var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == a.LOT_SID);
				var carrierInfo = txn.GetCarrierInfo(a.CARRIER_NO);
				txn.DoTransaction
					(
					//new CARTransaction.CarrierUnloadLotTxn(carrierInfo, _lot)
					new CARTransaction.CarrierChangeStateTxn(carrierInfo, stateinfo)
					//, new CARTransaction.EndOfCarrierTxn(carrierInfo)
					);
			}


		}, true);

		[TestMethod]
		public void t_進站下料槍()
		=> _DBTest((txn) =>
		{

//		100031207-0010
//Rack-03
//Rack-04
			var wo = "100031194";
			var _Used = nameof(Genesis.Library.BLL.ICM.Definition.Status.Used);
			var _Idle = nameof(Genesis.Library.BLL.ICM.Definition.Status.Idle);
			var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault();
			//WP_WO woInfo = BaseWpWoServices.GetEntityByExpression(x => x.WO == wo);
			var list_carrier = (from a in txn.EFQuery_MES.SMT_CARRIER_TOOL
								join b in txn.EFQuery_MES.FC_CARRIER on a.CARRIER_NO equals b.CARRIER_NO
								where b.STATE_NO == _Used && a.WO == wo
								select b.CARRIER_NO).Distinct().ToList();
			var stateinfo = new CarrierUtility.CarrierStateInfo(txn.DBC, _Idle, CarrierUtility.CarrierStateInfo.IndexType.No);
			 
 
			foreach (var carrier in list_carrier)
			{
				var carrierInfo = txn.GetCarrierInfo(carrier);
				Check.Invalid("載具必須是使用中", carrierInfo.STATE_NO != _Used, carrierInfo);

				txn.DoTransaction
					(new CARTransaction.CarrierUnloadLotTxn(carrierInfo, _lot)
					, new CARTransaction.CarrierChangeStateTxn(carrierInfo, stateinfo)
					, new CARTransaction.EndOfCarrierTxn(carrierInfo)
					);

				//serv.InsertDataToHistory(_lot.WO, _lot.LOT, "T", actionLinkSid: _lot.SID, true);
			}
 
			//serv.UOW.Save();

		}, true);


		[TestMethod]
		public void t_InsertHistory()
		=> _DBTest((txn) =>
		{
			TxnBase.Test = (_txn, key, b)=>{ 
				var r = new {
					//SMT = GTI_Test.trc_SMT(_txn, key)
				};
				string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText(GTI_Test.g_path.t_Process, json);
			};
			var x = FileApp.Read_SerializeJson<List<Feeder>>(_log.t_allFeeders);
			var _lot = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == "GTI25010709475209508");
			txn.DoTransaction(new SMT.InsertHistory(_lot, x));
		}, true);


        [TestMethod]
        public void t_EQP_Tool_計算()
		=> _DBTest((txn) =>
		{
			var t = FileApp.Read_SerializeJson< DataStructure>(_log.t_EQP_Tool_計算);
		}, false);

		[TestMethod]
        public void t_EQP_EXT()
		=> _DBTest((txn) =>
		{
            var t = txn.EFQuery_MES.view_EqpExt().Where(c=>c.Main.EQP_SID == "GTI24122419462090669");
            //var t1 = t.Ext?.MAX_USE_COUNT;

		}, false);


        [TestMethod]
        public void t_使用次數計算公式()
        {
			decimal z = 0.5m;
			var t = Genesis.Library.BLL.ZZ.CUB.CodeRule.EQP_Tool.使用次數計算公式(1, z);

		}

		[TestMethod]
		public void t_EnforceFinished()
		=> _DBTest((Txn) =>
		{
            var LOT = "100031174-0010";
			var _dbc = Txn.EFQuery_MES;
			//var q_lot = _dbc.WP_LOT_OPER_PARALLEL.Where(c => c.LOT == "100031174-0080");
			//var _lot = q_lot.ToList_CheckExists();
			var _lot = (from lot in _dbc.WP_LOT_OPER_PARALLEL
							join oper in _dbc.PF_OPERATION
							on lot.OPER_SID equals oper.OPER_SID into grp_oper
							from oper in grp_oper.DefaultIfEmpty()

                        where lot.LOT == LOT
						select new
						{
							lot,
							oper,
							is人員離站 = !_dbc.WP_USER_TRACE_IN_MASTER.Any(c=>c.LOT == lot.LOT && c.ROUTE_VER_OPER_SID == lot.ROUTE_VER_OPER_SID && c.RUN_END_DATE == null)
						}).ToList();
                    
			var s_wo = _lot[0].lot.WO;
			var _wo = _dbc.WP_WO.Where(c => c.WO == s_wo).ToList_CheckExists();
            var z = _wo
					.Select(wo=>new {
                            WO= wo.WO,
                            PARTNO_VER_SID = wo.PARTNO_VER_SID,
                            PARTNO = wo.PARTNO,
                            QUANTITY = wo.QUANTITY,
                            LOT = LOT,
                            LotInfo = _lot.Select(c=>new {
                                STATUS = c.lot.STATUS,
                                OPER_NO= c.oper.OPERATION_NO,
                                OPERATION = c.lot.OPERATION,
                                QUANTITY = c.lot.QUANTITY,
                                is全部離站 = c.is人員離站,
                            }).ToList()
                        }).ToList();
			FileApp._tmpJson(z);


		}, false);


		[TestMethod]
		public void t_EnforceFinished_Case1()
		=> _DBTest((Txn) =>
		{
            /*
            			return from a in _self.WP_USER_TRACE_IN_MASTER
				   join b in _self.WP_USER_TRACE_IN
					   on a.IN_MASTER_SID equals b.IN_MASTER_SID into aGroup
				   select new d_MasterDetail<WP_USER_TRACE_IN_MASTER, WP_USER_TRACE_IN>
				   {
					   Main = a,
					   Exts = aGroup.ToList() // 將分組的 Ext 記錄轉換為 List
				   };
                var t = (from lot in _dbc.WP_LOT_OPER_PARALLEL.Where(c=>c.LOT == "100031174-0080")
                        join user_m in _dbc.WP_USER_TRACE_IN_MASTER on lot.LOT_SID equals user_m.IN_MASTER_SID into user_mGroup
                        from user_m in user_mGroup.DefaultIfEmpty()
                        join user_in in _dbc.WP_USER_TRACE_IN on user_m.IN_MASTER_SID equals user_in.IN_MASTER_SID into user_list
                        from user_in in user_list.DefaultIfEmpty()
                select new d_MasterDetail<WP_LOT_OPER_PARALLEL, d_MasterDetail<WP_USER_TRACE_IN_MASTER, WP_USER_TRACE_IN>>
                {
                    Main = lot,
                    Exts = user_m
					new d_MasterDetail<WP_USER_TRACE_IN_MASTER, WP_USER_TRACE_IN>(){
                            Main = user_m,
                            Exts = user_list.ToList()
                        }
                    }
                }).ToList();
            */
			var _dbc = Txn.EFQuery_MES;
            var q_lot = _dbc.WP_LOT_OPER_PARALLEL.Where(c=>c.LOT == "100031174-0080");
            var _lot = q_lot.ToList_CheckExists();
			var s_wo = _lot[0].WO;
			var _wo = _dbc.WP_WO.Where(c=>c.WO == s_wo).ToList_CheckExists();
            var user = (from user_m in _dbc.WP_USER_TRACE_IN_MASTER.Where(c=>q_lot.Any(c1=>c1.LOT == c.LOT && c1.ROUTE_VER_OPER_SID == c.ROUTE_VER_OPER_SID))
                        join user_in in _dbc.WP_USER_TRACE_IN on user_m.IN_MASTER_SID equals user_in.IN_MASTER_SID into user_list
                        from user_in in user_list.DefaultIfEmpty()
                        select new 
                        {
                            user_m,
							user_list = user_list.ToList()
                        }).ToList();
 
            var z = _wo
					.Select(wo=>new {
                            WO= wo.WO,
                            PARTNO_VER_SID = wo.PARTNO_VER_SID,
                            PARTNO = wo.PARTNO,
                            QUANTITY = wo.QUANTITY,
                            LotInfo = _lot.Select(lot=>new {
                                    STATUS = lot.STATUS,
                                    OPER_NO= user.Where(c=>c.user_m.LOT == lot.LOT && c.user_m.ROUTE_VER_OPER_SID == lot.ROUTE_VER_OPER_SID).FirstOrDefault().user_m.OPER_NO,
                                    OPERATION = lot.OPERATION,
                                    QUANTITY = lot.QUANTITY,
                                    UserInfo = user.Where(c=>c.user_m.LOT == lot.LOT && c.user_m.ROUTE_VER_OPER_SID == lot.ROUTE_VER_OPER_SID)
										.Select(c=> new{
											USER_NO = c.user_m.USER_NO,
											USER_NAME = c.user_m.USER_NAME,
                                            RUN_START_DATE = c.user_m.RUN_START_DATE,
                                            RUN_END_DATE = c.user_m.RUN_END_DATE,
                                            WorkInfo = c.user_list.Select(c1=>new {
												USER_NO = c1.USER_NO,
												USER_NAME = c1.USER_NAME,
                                                START_TIME = c1.START_TIME,
                                                END_TIME = c1.END_TIME,
											}).ToList()
										})
										.ToList()
                            }).ToList()
                }).ToList();


			//var groupedResult = flatList
			//	.GroupBy(x => x.lot)
			//	.Select(g => new d_MasterDetail<WP_LOT_OPER_PARALLEL, d_MasterDetail<WP_USER_TRACE_IN_MASTER, WP_USER_TRACE_IN>>
			//	{
			//		Main = g.Key,
			//		Exts = g.GroupBy(x => x.grp_user_m)
			//				.Select(mg => new d_MasterDetail<WP_USER_TRACE_IN_MASTER, WP_USER_TRACE_IN>
			//				{
			//					Main = mg.Key,
			//					Exts = mg.Select(x => x.grp_user_in).Where(x => x != null).ToList()
			//				}).ToList()
			//	}).ToList();
			FileApp._tmpJson( z );
		}, false);


		[TestMethod]
		public void t_查詢未離線人員(){
            //var _list = _zzAPI.query_online_user("100031174-0010","GTI24122418180688152");
            //FileApp._tmpJson(_list);
        }


		[TestMethod]
		public void t_GetMachineTool_1()
		{
            var _list = _zzAPI.GetMachineTool_1("GTI25052909345834007");
            FileApp._tmpJson(_list);
        }

		[TestMethod]
		public void t_保養提醒通知判定(){

			var T01 = _zzCodeRule.EQP_Tool.保養作業判定(100, 0, null);
			Assert.IsTrue
				(T01.Check() == -1
				&& (T01.Check_EqulMax() == -1)
				&& (T01.Check_Equl() == -1
				&& (T01.Check_EqulMin() == -1)
				)
				, "未設最大使用量/浮動值, 相符/超過 判定模式");

			var T02 = _zzCodeRule.EQP_Tool.保養作業判定(100, 0, 100);
			Assert.IsTrue
				(T02.Check() == -1
				&& (T02.Check_EqulMax() == 1)
				&& (T02.Check_Equl() == 1)
				&& (T02.Check_EqulMin() == 0)
				, "最大使用量 100 ,未設 , 最大值 相符/超過 判定模式");

			var T03 = _zzCodeRule.EQP_Tool.保養作業判定(100, 0, 100, (decimal?)1.1);
			Assert.IsTrue
				(T03.Check() == -1
				&& (T03.Check_EqulMax() == -1)
				&& (T03.Check_Equl() == 0)
				&& (T03.Check_EqulMin() == 0)
				, "最大使用量 100 ,設 浮動值 1.1 , 最大值 相符/超過 判定模式");

			T03.CheckVal = 110;
			Assert.IsTrue
				(T03.Check() == 0
				&& (T03.Check_EqulMax() == 1)
				&& (T03.Check_Equl() == 1)
				&& (T03.Check_EqulMin() == 0)
				, "最大使用量 100 ,設 浮動值 1.1 , 最大值 相符/超過 判定模式");


			var T04 = _zzCodeRule.EQP_Tool.保養作業判定(90, 0, 100, (decimal ?)0.9);
			Assert.IsTrue
				(T04.Check() == -1
				&& (T04.Check_EqulMax() == -1)
				&& (T04.Check_Equl() == 0)
				&& (T04.Check_EqulMin() == 0)
				, "最大使用量 100 ,設 浮動值 0.9 , 最大值 相符/超過 判定模式");

			T04.CheckVal = 100;
			Assert.IsTrue
				(T04.Check() == 0
				&& (T04.Check_EqulMax() == 1)
				&& (T04.Check_Equl() == 1)
				&& (T04.Check_EqulMin() == 0)
				, "最大使用量 100 ,設 浮動值 1.1 , 最大值 相符/超過 判定模式");

			/*

			Assert.IsTrue
				(_zzCodeRule.EQP_Tool.保養提醒通知判定(90, 5, null, (decimal?)0.9) == false
				, "未設置最大使用量 , 恆為不超過 ");

			Assert.IsTrue
				(_zzCodeRule.EQP_Tool.保養提醒通知判定(100, 0, 100, null) == false
				&& (_zzCodeRule.EQP_Tool.保養提醒通知判定(100, 1, 100, null))
				, "未設置 FLEX_RATE , 不得超過 MAX_USE_COUNT ");

			Assert.IsTrue
				(_zzCodeRule.EQP_Tool.保養提醒通知判定(100, 1, 100, (decimal?)1.1) == false
				, "100+1 < 110 應為未超過 ");

			Assert.IsTrue
				(_zzCodeRule.EQP_Tool.保養提醒通知判定(90, 1, 100, (decimal?)0.9)
				, "90+1 > 90 應為超過 ");
				*/
		}

        [TestMethod]
		public void t_取得併行批號包裝站的設定()
		=> _DBTest((txn) =>
		{
            var _dbc = txn.EFQuery_MES;
            var _q =(from a in _dbc.WP_LOT_OPER_PARALLEL
                    join b in _dbc.PF_OPERATION_EXPAND on a.OPER_SID equals b.OPER_SID into grp_oper
                    from b in grp_oper.DefaultIfEmpty()
                    where a.LOT == "100031207" //&& b.ENABLE_FLAG == "T"
					 select new {
                        Lot = a,OperSet = b
                    }).ToList();     

            var _r = _q.Where(c => {
                dynamic data = JsonConvert.DeserializeObject(c.OperSet.SETTING_JSON);
                if (FindProperty(data.CheckOutSet, "IsTreePacking")) return data.CheckOutSet.IsTreePacking == true;
                return false;
                })
                .ToList();


			var _lot = _r.FirstOrDefault().Lot;
			//取得包裝的大項資訊
			var pkgInfo = _dbc.PF_PARTNO_PACKAGE.FirstOrDefault(x => x.PARTNO_VER_SID == _lot.PARTNO_VER_SID && x.DEFAULT_FLAG == "T");

			//var PKG_TYPE_NO = pkgInfo.PACKAGE_TYPE_NO ?? _lot.Attr01_替代包裝類別No();
			//var pkgNodes = (from a in _dbc.ZZ_PF_PACKAGE_ITEM
			//				where a.PKG_TYPE_NO == PKG_TYPE_NO
			//					&& a.UPPER_LAYER_SID != null
			//				select a
			//					).ToList();

		}, false);


		[TestMethod]
		public void t_查詢機台上有沒有特定批號()
		=> _DBTest((Txn) =>
		{
					/*
					原本的需求其實是查批號有沒有上過這個機台 , 但以下的查法是不正確的 
					 ,但還是先保留以做參考 
					 */
			var _lotA = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == "GTI25052211164229261");
			var _eqp_trace = Txn.EFQuery_MES.WP_LOT_EQP_TRACE.FirstOrDefault(c => c.EQP_LINK_SID == _lotA.EQP_LINK_SID);

		}, true);

		[TestMethod]
		public void t_取得併行批號包裝站的設定_1() {
			var t = LOT_Services.ChangePackagingRule_LotInfo("100031207");
			FileApp._tmpJson(t);
		}


		[TestMethod]
		public void t_達次保養()
		=> _DBTest((Txn) =>
		{
            try
            {
				var zz =  Txn.GetParameterInfo(_zzCodeRule.系統參數.達次保養通知群組)
					.CheckExist($"達次保養通知群組[{_zzCodeRule.系統參數.達次保養通知群組}]");
            }
            catch (Exception ex)
            {
				var z = ex;
            }

		});


		[TestMethod]
		public void t_20251205_Mail_達次()
		=> _DBTest((Txn) =>
		{
			var mail_TO = (from a in Txn.f取得角色人員清單(系統參數.角色_保養單核准)
						   where a.EMAIL != null && a.EMAIL != ""
						   select a.EMAIL).ToList();

			var _list = (from P0 in Txn.EFQuery_MES.f保養單查詢基礎結構()
					  where P0.STATUS != "OK" && P0.PLAN_TYPE == "U"
						 && P0.CONFIRM == "N"
					  && (P0.DEAD_LINE == null)
					  select  P0 
					  ).ToList();
			var _list_SID = _list.Select(c => c.PLANIMPLEMENT_SID).ToList();

			var _user = (from a in Txn.EFQuery_MES.PM_PLANIMPLEMENTUSER
							join b in Txn.EFQuery_MES.AD_USER
								on a.USER_SID equals b.USER_SID
						 where _list_SID.Contains(a.PLANIMPLEMENT_SID)
						 select new {
							 PLANIMPLEMENT_SID = a.PLANIMPLEMENT_SID,
							 USER = b.USER_NAME,
							 MAIL = b.EMAIL
						 }).ToList();

			var _mail = _user.Where(c => !string.IsNullOrEmpty(c.MAIL))
						.Select(c => c.MAIL)
						.Distinct()
						.Except(mail_TO)
                .ToList(); 
			mail_TO.AddRange(_mail);

			foreach (var a in _list){
				var __list = _user
					.Where(b => b.PLANIMPLEMENT_SID == a.PLANIMPLEMENT_SID)
					.Select(b => b.USER)
					.ToList();
				a.USERs = string.Join(",", _list);
			}

			FileApp._tmpJson(new { _list, mail_TO , _user });
		});



		[TestMethod]
        public void t_f依據sid取得AD_USER()
		=> _DBTest((Txn) =>
		{
			var q_userSid = Txn.EFQuery_MES.f取得PlanUserList("GTI25100917380340153")
					.Select(c => c.USER_SID);
			var list_user = Txn.EFQuery_MES.AD_USER
				.f依據sid_list取得AD_USER(q_userSid)
				.ToList();
		}, true);

		[TestMethod]
		public void t_建立達次保養單_原型()
		=> _DBTest((Txn) =>
        {
			var _eqp = Txn.EFQuery_MES.FC_EQUIPMENT
			   .Where(c => c.EQP_NO == "161600000313")
			   .FirstOrDefault_CheckExists();

			var _plan = Txn.EFQuery_MES.f依據OBJECT_SID取得Plan(_eqp.EQP_SID ,"U" )
				.FirstOrDefault_CheckExists("查無對應的 PM Plan 記錄", true);

			string Calculation = "Plan";
			string HolidaySet = Txn.EFQuery_MES.PM_PARAMETER
				.Where(x => x.CALCULATION == Calculation)
				.FirstOrDefault().HOLIDAY_SET;


			var obj = new PM_PLANIMPLEMENT(){
                PLANIMPLEMENT_SID = Txn.GetSID(true),
                PLAN_SID = _plan.PLAN_SID,
				PLAN_DATE = Txn.ExeTime.AddDays(-1) ,
				PARAMETER_SID = _plan.PARAMETER_SID,
				//PLAN_NO = _plan.PLAN_NO,
				STATUS = MaintenancePlanStatus.WAIT.ToString(),
				PLANIMPLEMENT_NAME = _plan.PLAN_NAME,
				ITEM_CLASS = MaintenanceObject.Equipment.ToString(),
				//ITEM_CLASS = ModuleNames.UCO.ToString(),
				//ITEM_NUMBER = 1,
				//ENABLE_FLAG = "T",
				//QUOTE_ONCE = "F",
				PLAN_UNIT = _plan.PLAN_UNIT,
				UNIT_NUMBER = 1,
				//START_DATE = Txn.ExeTime,
				CALLOLATION = Calculation,
				HOLIDAY_SET = HolidaySet,
				PLANITEM_SID = _eqp.EQP_SID,
				//CANCEL_DATE = "",
				DESCRIPTION = "",
				//LASTPLAN_DATE = "",
				//LASTACTUAL_DATE = "",
				//PARAMETER_SID = "",
				//ROLL_DATE = "",
				//PLAN_TYPE = _plan.PLAN_TYPE,
			};
			Txn.EntityCommonSetVal(obj, isNeedInit: true);


			var q_userSid = Txn.EFQuery_MES.PM_PLANUSER.f取得userSIDs(_plan.PLAN_SID);
			var list_user = Txn.EFQuery_MES.AD_USER
				.f依據sid_list取得AD_USER(q_userSid)
				.ToList();
 
			var list_PM_PLANIMPLEMENTUSER = new List<PM_PLANIMPLEMENTUSER>();
			foreach (AD_USER user in list_user){
				var _obj = new PM_PLANIMPLEMENTUSER()
				{
					PLANIMPLEMENTUSER_SID = Txn.GetSID(true),
					USER_SID = user.USER_SID,
					PLANIMPLEMENT_SID = obj.PLANIMPLEMENT_SID,
					CREATE_DATE = Txn.ExeTime,
					CREATE_USER = Txn.UserNo,
					UPDATE_DATE = Txn.ExeTime,
					UPDATE_USER = Txn.UserNo,
				};
				//Txn.EntityCommonSetVal(_obj, isNeedInit: true);
				list_PM_PLANIMPLEMENTUSER.Add(_obj);
			}

			var itemList = (from a in Txn.EFQuery_MES.PM_PLANITEM
							where a.PLAN_SID == _plan.PLAN_SID
							select a
				).ToList();

			var list_PM_PLANIMPLEMENTITEM = new List<PM_PLANIMPLEMENTITEM>();
			foreach (var item in itemList){
				var _obj = new PM_PLANIMPLEMENTITEM()
				{
					PLANIMPLEMENTITEM_SID = Txn.GetSID(true),
					PLANIMPLEMENT_SID = obj.PLANIMPLEMENT_SID,
					ITEM_SID = item.ITEM_SID,
					CREATE_DATE = Txn.ExeTime,
					CREATE_USER = Txn.UserNo,
					UPDATE_DATE = Txn.ExeTime,
					UPDATE_USER = Txn.UserNo,
					//EDC_SID = item.EDC_SID,
					//EDC_VER_SID = item.EDC_VER_SID,
				};
				//Txn.EntityCommonSetVal(_obj, isNeedInit: true);
				list_PM_PLANIMPLEMENTITEM.Add(_obj);
			}

			Txn.EFQuery_MES.PM_PLANIMPLEMENT.Add(obj);
			Txn.EFQuery_MES._BulkInsert(list_PM_PLANIMPLEMENTUSER);
			Txn.EFQuery_MES._BulkInsert(list_PM_PLANIMPLEMENTITEM);

			Txn.EFQuery_MES.SaveChanges();



            ParallelCheckOut.SendMail(Txn, obj, list_user
				, _eqp.EQP_NAME
				, MaintenanceObject.Equipment.ToDisplayName()
				, t_MES.GMail_Test);

		},true,true);


		
        [TestMethod]
        public void t_SendMail_找不對應的計劃單()
        => _DBTest((txn) =>{
			ParallelCheckOut.SendMail_找不對應的計劃單(txn, "機台Name", "機台", t_MES.GMail_Test);
		}, true);

		[TestMethod]
        public void t_達次保養確認_EQP()
		=> _DBTest((Txn) =>
		{
			var T01 = _zzCodeRule.EQP_Tool.保養作業判定(100, 1, 100);
			var z = T01.Check();
			var _eqp = Txn.EFQuery_MES.view_EqpExt()
				.Where(c => c.Main.EQP_NO == "161600000313")
				.FirstOrDefault_CheckExists();
			ParallelCheckOut.達次保養作業_Eqp(Txn, _eqp, T01);

		}, true,true);

		[TestMethod]
		public void t_達次保養確認_TOOL()
		=> _DBTest((Txn) =>
		{
			var T01 = _zzCodeRule.EQP_Tool.保養作業判定(100, 1, 100);
			var z = T01.Check();
			var _Tool = Txn.EFQuery_MES.view_ToolExt()
				.Where(c => c.Main.TOOL_NO == "CTT01")
				.FirstOrDefault_CheckExists();
			ParallelCheckOut.達次保養確認_Tool(Txn, _Tool, T01);

		}, true, true);


		[TestMethod]
		public void t_保養單審核人員判定()
		{
			var r = _zzCodeRule.系統參數.保養單審核人員判定("thony");
		}
		

		[TestMethod]
		public void t_依據EQP_NO查對應計劃()
		=> _DBTest((Txn) =>
		{
					//目的是取得機台 SID
			var _eqp = Txn.EFQuery_MES.FC_EQUIPMENT
				.Where(c => c.EQP_NO == "161600000313")
				.FirstOrDefault_CheckExists();

			var _plan = Txn.EFQuery_MES.f依據OBJECT_SID取得Plan(_eqp.EQP_SID)
				.FirstOrDefault_CheckExists("查無對應的 PM Plan 記錄", true);



			//原型)請用上述程序 組合出 使用 Eqp_SID 直接查出 plan
			//	var _plan1 = (from planObject in Txn.EFQuery_MES.PM_PLANOBJECT
			//					  // 步驟 1: 查詢符合此機台 SID 的 PM_PLANOBJECT 記錄
			//				  where planObject.OBJECT_SID == _eqp.EQP_SID
			//				  // 步驟 2: 透過 PLAN_SID 欄位，連接 (JOIN) 到 PM_PLAN 表
			//				  join plan in Txn.EFQuery_MES.PM_PLAN
			//			on planObject.PLAN_SID equals plan.PLAN_SID
			//				  // 步驟 3: 選取最終的 PM_PLAN 實體
			//				  select plan)
			//// 獲取第一筆符合的記錄，並使用 CheckExists 檢查結果
			//.FirstOrDefault_CheckExists("查無對應的 PM Plan 記錄", true);


		});


        [TestMethod]
        public void t_查詢機台上的批號中未下崗的人員()
		=> _DBTest((txn) =>
		{
			var _list1 = _zzCodeRule.LOT.查詢機台上的批號中未下崗的人員(txn.EFQuery_MES, "GTI24122419462090560", false)
						.ToList();


			var EFQuery_MESContext = txn.EFQuery_MES;

			var _未機台的批號清單 = (from a in EFQuery_MESContext.WP_LOT_EQP_TRACE
				join b in EFQuery_MESContext.WP_LOT_OPER_PARALLEL 
					on a.LOT_SID equals b.LOT_SID
						  where a.UNLOAD_LINK_SID == null
						  select new { trc_EQP = a , Lot = b });
			var t1 = _未機台的批號清單
				.Where(c=>c.Lot.LOT == "100031177-0010")
				.FirstOrDefault();
			var _list = (from w0 in EFQuery_MESContext.WP_USER_TRACE_IN_MASTER
						 where  _未機台的批號清單.Any(x => x.Lot.LOT == w0.LOT && x.Lot.ROUTE_VER_SID == w0.ROUTE_VER_OPER_SID)
								&&  w0.RUN_END_DATE == null
						 select new {
							 KEY = w0.IN_MASTER_SID,
							 USER_NO = w0.USER_NO,
							 USER_NAME = w0.USER_NAME,
							 START_TIME = w0.RUN_START_DATE,
							 END_TIME = w0.RUN_END_DATE,
							 LOT = w0.LOT,
							 ROUTE_VER_OPER_SID = w0.ROUTE_VER_OPER_SID,
						 }).ToList();
		}, true);


		[TestMethod]
		public void job_重設機台並下治具_平行工站()
		=> _DBTest((txn) =>
		{
			var _eqpInfo = txn.GetEquipmentInfo("Test03", IndexType: EquipmentUtility.IndexType.No);

			// 取得機台上的治具
			var ToolsOfEqp = _eqpInfo.GetEquipmentLoadToolList();
			//var ToolsOfEqp = txn.EFQuery_MES.f取得機台上的治具清單(equip.No).ToList();
			if (ToolsOfEqp.Count != 0)
			{
				// 治具下機台
				txn.DoTransaction(new EQPTransaction.EquipmentUnloadToolTxn(_eqpInfo, ToolsOfEqp));
				foreach (var tool in ToolsOfEqp)
				{
					txn.DoTransaction(new TOLTransaction.EndOfToolTxn(tool));
				}
				txn.DoTransaction(new EQPTransaction.EndOfEquipmentTxn(_eqpInfo));
			}

			var EqpFn = new EquipmentUtility.EquipmentFunction(txn.DBC);
			var lots = txn.EFQuery_MES.f取得機台上的平行工站批號清單(_eqpInfo.SID);
			//DataTable lots = EqpFn.GetEquipmentLotList(_eqpInfo.No);

			foreach (var lot in lots)
			{
				_eqpInfo = _eqpInfo.ReLoad(txn.DBC);
				var _lotInfo = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL.FirstOrDefault(c => c.LOT_SID == lot.LOT_SID);
				txn.DoTransaction(
					new EQPTransaction.EquipmentUnloadLotTxn(_eqpInfo, _lotInfo),
					new EQPTransaction.EndOfEquipmentTxn(_eqpInfo)
				);
			}
			_eqpInfo = _eqpInfo.ReLoad(txn.DBC);
			if (_eqpInfo.CURRENT_LOT_SIZE != 0 || _eqpInfo.CURRENT_CAPACITY != 0)
			{
				_eqpInfo = _eqpInfo.ReLoad(txn.DBC);
				txn.DoTransaction(
					new EQPTransaction.EquipmentChangeCapacityAndLotSizeTxn(_eqpInfo, 0, 0)
					{
						ChangeStatus = true
					}
				); ;
			}
		}, true);



		[TestMethod]
		public void t_20251203_報修_保養時限()
		=> _DBTest((Txn) =>
		{

			var day = "2025/11/22";
			DateTime start = DateTime.Parse(day).Date;
			
			// 當日的開始（00:00:00）
			//DateTime start = targetDate.Date;

			// 當日的結束（23:59:59.997）
			DateTime end = start.AddDays(1).AddMilliseconds(-1);

			var _list_報修 = (from r in Txn.EFQuery_MES.PM_REPAIR
					where r.DEAD_LINE != null
							&& r.DEAD_LINE >= start
							&& r.DEAD_LINE <= end
					select r).ToList();

			var _list_保養 =(from r in Txn.EFQuery_MES.PM_PLANIMPLEMENT
						   where r.DEAD_LINE != null
							&& r.DEAD_LINE >= start
							&& r.DEAD_LINE <= end
					select r).ToList();

			FileApp._tmpJson(new { _list_報修, _list_保養 });

		}, true);


		[TestMethod]
		public void t_20251225_TransToByte()
		=> _DBTest((Txn) =>{
			var day = "2025/11/22";
			DateTime start = DateTime.Parse(day).Date;
			DateTime end = start.AddDays(1).AddMilliseconds(-1);

			var _list_報修 = (from r in Txn.EFQuery_MES.PM_REPAIR
							where r.DEAD_LINE != null
									&& r.DEAD_LINE >= start
									&& r.DEAD_LINE <= end
							select r).ToList();

			//需要安裝 OfficeOpenXml ,先作罷
			//var t = EpplusHelper.TransToByte(_list_報修);

		}, true);



		[TestMethod]
		public void t_20251222_isPassChk()
		{
			string lot_sid = "GTI24122513471294964";
			//string UserNo = "thony";
			_zzAPI.Check_IQPC(lot_sid, Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機.BeforeUserStart ,null, true);
		}




		[TestMethod]
		public void t_20260114_工作站的機台治具設定()
		=> _DBTest((Txn) =>
		{
			var lotInfo = Txn.GetLotInfo("");
			var oper_no = "JK_LINE-OPR-00000009";


			var operinfo = Txn.GetOperationInfo(oper_no, IndexType: OperationUtility.IndexType.No);
			OperationUtility.OperationFunction operfun = new OperationUtility.OperationFunction(Txn.DBC);
			var dt_Eqp = operfun.GetOperEquipAllEquipmentList(operinfo);
			//var dt_Tool = operfun.GetOperToolSetting(operinfo.SID);
			var dt_Tool = Txn.EFQuery_MES.f工作站_治具清單(operinfo.SID).ToList();
			//FileApp.WriteSerializeJson(new { dt_Eqp, dt_Tool }, _log.t_工作站的機台治具設定);

			var _過站機台治具資訊 = FileApp.Read_SerializeJson<List<CustomerList>>(_log.t_EqpToolList);


			if (dt_Eqp == null || dt_Eqp.Rows.Count == 0) return;
			Check.Invalid($"必須選取的機台", _過站機台治具資訊.Count == 0, _過站機台治具資訊);
			var _dt_Eqp = dt_Eqp.AsEnumerable();
			var chk_不合規機台 = _過站機台治具資訊
				.Where(c => _dt_Eqp.Any(row => row.Field<string>("EQP_NO") == c.No) == false)
				.ToList();
			Check.Invalid($"所選取的機台必須符合工作站設定", chk_不合規機台.Count != 0, chk_不合規機台);

			if (dt_Tool != null && dt_Tool.Count != 0)
			{
				var chk_未設定治具 = _過站機台治具資訊.Where(c => c.subItem == null || c.subItem.Count == 0)
					.Select(c => c.No)
					.ToList();
				Check.Invalid($"機台({string.Join(",", chk_未設定治具)})必須選取治具", chk_未設定治具.Count != 0, chk_未設定治具);
				/*
				 檢查所選取的治具是否有符合工站設定 
				 但如果 規則不設限(只限符合) , 視同不卡不合規 , 可以略過以下檢核
				 */
				var chk_不合規治具 = _過站機台治具資訊
					.Where(c => c.subItem.Any(c1 => !dt_Tool.Any(c2 => c2.TOOL_NO == c1.No)))
					.ToList();
				Check.Invalid($"機台({string.Join(",", chk_不合規治具.Select(c => c.No))})所選取的治具不符合工作站設定", chk_不合規治具.Count != 0, chk_未設定治具);

			}


			var Used = nameof(Genesis.Library.BLL.ICM.Definition.Status.Used);

			foreach (var eqp in _過站機台治具資訊)
			{
				var _eqpInfo = Txn.GetEquipmentInfo(eqp.SID);
				Check.isExist($"機台({_eqpInfo.No})", _eqpInfo.IsExist);
				Check.Invalid($"機台({_eqpInfo.No})狀態不能為 Used", _eqpInfo.STATE_NO == Used);

				if (eqp.subItem != null)
				{
					foreach (var tool in eqp.subItem)
					{
						var _tool = Txn.GetToolInfo(tool.No);
						Check.isExist($"治具({tool.No})", _tool.IsExist);

						var isUsed = _tool.STATE_NO == Used;
						//檢核治具是否已上機台
						if (isUsed)
						{
							Check.Invalid($"治具({_tool.No})狀態不能為 Used", _tool.STATE_NO == Used);

							//檢核治具是否上在對應的機台上
							var usedEqp = _tool.GetCurrentEquipmentInfo();
							var isNotSameEqp = usedEqp.SID != eqp.SID;
							Check.Invalid($"治具({_tool.No})已上在({usedEqp.No})", isNotSameEqp, usedEqp);
						}
						else
						{
							Txn.DoTransaction(
								new EQPTransaction.EquipmentLoadToolTxn(_eqpInfo, _tool),
								new TOLTransaction.EndOfToolTxn(_tool),
								new EQPTransaction.EndOfEquipmentTxn(_eqpInfo)
							);
							_eqpInfo = _eqpInfo.ReLoad(DBC);
						}
					}
				}

				Txn.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(_eqpInfo, lotInfo));
			}
		}, false, true);


		[TestMethod]
		public void t_20251219_iqpc_form_start() {
			string lot_sid = "GTI24122513471294964";
			//string UserNo = "thony";
			_zzAPI.Check_IQPC(lot_sid,  Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機.BeforeUserStart, null, true);
		}


		[TestMethod]
		public void t_20260316_UserGroupStepLicenseData()
		{
			App.Timer(()=>{ 
				var _r = FileApp.Read_SerializeJson<UserGroupStepLicenseServices.DataStruct>(_log.t_UserGroupStepLicenseData);
				return UserGroupStepLicenseServices.UpdateV2(_r, true);
			});
		}


		/*
		=> _DBTest((Txn) => {
			string lot_sid = "GTI24122513471294964";
			string UserNo = "thony";

			var ILot = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
				.Where(c => c.LOT_SID == lot_sid)
				.FirstOrDefault_CheckExists();
 
			var r = 基本檢核_站別檢驗單卡控
				(Txn, ILot
				, UserNo
				, Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機.BeforeUserStart
				, true);

			var 需要檢核的檢驗表單 = r.站別檢驗單設定.FirstOrDefault().Value;
			var _Forms = new List<string>();
			if (需要檢核的檢驗表單 != null) {
				var oper = Txn.GetOperationInfo(ILot.OPER_SID);
				foreach (var form in 需要檢核的檢驗表單){
					//GTI25020414304730705
					var isHasForm = r.符合的檢驗單.Any(c => c.INSP_SID == form.QC_INSP.INSP_SID);
					if (isHasForm) continue;
					var OperInsp = form.OperInsp;
					var QC_INSP = form.QC_INSP;
					string _QC_NO;
					var _svcWP_IPQC = Txn.EFQuery_MES.WP_IPQC;
					var Encode = new EncodeFormatUtility.EncodeFormatInfo
								(Txn.DBC, "InspNoByLot", EncodeFormatUtility.IndexType.No);
					if (Encode.IsExist && Encode.ENABLE_FLAG == "T")
					{
						var Code = EncodeFormatUtility.Coder.GetCodes
							(Txn.DBC, Txn.UserNo, Encode, 1, "", ""
							, ILot.LOT_SID, "", false);
						_QC_NO = Code.Codes[0];
						Txn.DoTransaction(Code.Commands);
					}
					else
					{
						_QC_NO = Txn.GetSID();
					}
					var _WP_IPQC = new WP_IPQC() {
						QC_NO = _QC_NO,
						QC_INSP_METHOD = "NORMAL",
						INSP_STATUS = nameof(RES.BLL.Face.Confirm),
						INSP_SID = QC_INSP.INSP_SID,
						QC_INSP_TYPE = QC_INSP.INSP_TYPE,
						FORM_TYPE = QC_INSP.INSP_TYPE,
						FORM_TARGET = QC_INSP.INSP_KEY,
						OPERATION = oper.Name,
						OPERATION_NO = oper.No,
						OPER_SID = oper.SID,
						CREATE_USER = UserNo,
						UPDATE_USER = UserNo,
						CREATE_DATE = Txn.ExeTime,
						UPDATE_DATE = Txn.ExeTime,
						QC_RESULT = "-"
					};
					Txn.EFQuery_MES.WP_IPQC.Add(_WP_IPQC);
					var obj_ZZ_INSP_RECHECK = new ZZ_INSP_RECHECK()
					{
						SID = Txn.GetSID(true),
						QC_NO = _WP_IPQC.QC_NO,
						STATUS = nameof(RES.BLL.Face.Create),
						CREATE_USER = UserNo,
						UPDATE_USER = UserNo,
						CREATE_DATE = Txn.ExeTime,
						UPDATE_DATE = Txn.ExeTime
					};
					Txn.EFQuery_MES.ZZ_INSP_RECHECK.Add(obj_ZZ_INSP_RECHECK);

					var obj_WP_IPQC_LOT = new WP_IPQC_LOT(){
						SID = Txn.GetSID(true),
						QC_NO = _WP_IPQC.QC_NO,
						ROUTE_VER_OPER_SID = ILot.ROUTE_VER_OPER_SID,
						LOT_SID = ILot.LOT_SID,
						LOT = ILot.LOT,
					};
					Txn.EFQuery_MES.WP_IPQC_LOT.Add(obj_WP_IPQC_LOT);
					Txn.EFQuery_MES.SaveChanges();
					_Forms.Add(_WP_IPQC.QC_NO);
				}
			}
		}, true,true);
		*/


		public class DataStructure
		{
			public int 良品數 { get; set; }
			public Dictionary<string, Equipment> EQP { get; set; } // 使用 Dictionary 來表示動態的 EQP
		}

		public class Equipment: Item{
			public Dictionary<string, Item> TOOL { get; set; }
		}

		public class Item{
			public double 轉換率 { get; set; }
		}
	}



}
//todo-CUB
