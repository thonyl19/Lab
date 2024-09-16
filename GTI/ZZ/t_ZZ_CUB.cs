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
using static Genesis.GT_Server.Service;
using BLL.MVC;
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
			new ParallelCheckIn().Process(_r,true);
		}

 


        [TestMethod]
        public void t_PARALLEL_History()
		=> _DBTest((txn) =>
		{
			var _d = txn.LzQuery.WIP.t併行工作站在製現況表.FirstOrDefault();
			var tx = new PARALLEL_History(txn.ActionReason, _d,_d);
			tx.lot_old = _d;
			txn.DoTransaction(tx);
		}, true,true);


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


			var tx = new PARALLEL_History(txn.ActionReason, _d,_d);
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

			var tx = new Genesis.Library.BLL.DTC.Lot.PARTIAL_PROGRESS_UPDATE(_d, _d, _r.GoodList[0],true)
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
			var zz = new SearchServices() { _Txn = txn};
			zz.Parallel_InOut("PAR$GTI24071713503027675");
		}, true, true);



		[TestMethod]

		public void t_GetCarrierInfo()
		=> _DBTest((txn) =>
		{
			var z = txn.GetCarrierInfo();
		}, true, true);

		
	}


    
}

