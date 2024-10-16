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
//using _bllSvc = Genesis.Library.BLL.ZZ.CUB;

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
		public void t_LOT_OPER_PARALLEL_GOOD_QTY__測試有SN()
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
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T_IPQC;
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

				chk_前站未完成項目(當前的執行清單, list_前站完成清單);

            }


        }, true);


        [TestMethod]
        public void t_fn()
        {
			var r = ApiService.SequenceWO_Check_SN("GTI24071610203527080","Serial1");

		}

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

    }



}

