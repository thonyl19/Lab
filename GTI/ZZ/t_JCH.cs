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
using _bllSvc = Genesis.Library.BLL.ZZ.JOCHU;
using Genesis.Library.BLL.ZZ.JOCHU.CodeRule;
using Genesis.Library.BLL.ZZ.JOCHU;

namespace UnitTestProject
{
	/// <summary>
    /// 台英帝國
    /// </summary>
    [TestClass]
	public class t_JCH : _testBase
	{
		static class _log
		{
			internal static string list_WP_WO_MTL_BOM
			{
				get
				{
					
					return FileApp.ts_Log(@"ZZ\JCH\WP_WO_MTL_BOM.json");
				}
			}
 

		}
 

		[TestMethod]
		public void t_List_線外投入工時()
		{
			var x =  Table.List_線外投入工時().FirstOrDefault();
		}


        [TestMethod]
        public void t_fn1()
		=> _DBTest((txn) =>
		{
			var _d = txn.EFQuery_MES._線外投入工時();
		}, true,true);

		[TestMethod]

		public void t_List_線內停機()
		=> _DBTest((txn) =>
		{
			var _d = Table.List_線內停機(txn.DBC).FirstOrDefault();
		}, false, true);
		

		//[TestMethod]
		//public void t_取得工單機台停線時間()
		//=> _DBTest((txn) =>
		//{
		//	var _d = new EquipWorkingHoursServices().取得工單機台停線時間("","",txn.DBC);
		//}, false, true);

		[TestMethod]
		public void t_StopServices_Query() {
			var _svc = new StopServices();
			_svc.DbContext = new MDL.MESContext();
			var zz = _svc.DbContext.Database;
			var zz1 = ((MDL.MESContext)_svc.DbContext).WP_LOT.FirstOrDefault();
			var x = _svc.Query("");
		}

		[TestMethod]
		public void t_Equipment_DAE()
		=> _DBTest((txn) =>
		{
			var eqp = txn.GetEquipmentInfo("A016", EquipmentUtility.IndexType.No);
			_Prd.Check.Equipment_DAE(txn,eqp);
		}, false, true);




		[TestMethod]
		public void t_x()
		=> _DBTest((txn) =>
		{
			var z = txn.EFQuery_MES.ZZ_ERP_STOP_CODE.FirstOrDefault();
			new StopCodeServices().Insert(z, true);
		}, false, true);

 
	}
}

