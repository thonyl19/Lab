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
using Frame.Code.Web.Select;

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
			//var _d = txn.EFQuery_MES._線外投入工時();
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
		public void t_BindPallet()
		{
			string[] lots = new string[] { "1ABZ080004420-24C11AN024" };
			string[] unbind = null;
			string palletNo; int boxCount;
			//var r = new OfflineWorkingServices().BindPallet
			//	(lots
			//	, unbind

			//	,true);
		}


		
		[TestMethod]
		public void t__CheckRole()
		=> _DBTest((txn) =>{
			var z2 = App.Timer(() => RunStopServices._CheckRole(txn, "F3", "Admin"));

			var z1 = App.Timer(()=>{
				var user = txn.EFQuery_MES.AD_USER.FirstOrDefault(c => c.ACCOUNT_NO == "Admin");
				
				 var F3Users = (from aur in txn.EFQuery_MVC.AD_USER_ROLE
				 where aur.ROLE_SID == (from r in txn.EFQuery_MVC.AD_ROLE
										where r.ROLE_NO == "ROLE-070"
										select r.SID).FirstOrDefault()
				 select aur.USER_SID).Distinct().AsQueryable();

				var z = F3Users.Contains(user.USER_SID);

				return new { z };
			});
		}, false, true);


		[TestMethod]
		public void t_x()
		=> _DBTest((txn) =>
		{
			var z = txn.EFQuery_MES.ZZ_ERP_STOP_CODE.FirstOrDefault();
			new StopCodeServices().Insert(z, true);
		}, false, true);


        [TestMethod]
        public void t_查詢同工單內低於滿包數的批號清單()
        {
			var z = _bllSvc.ApiService
				.查詢同工單內低於滿包數的批號清單("0ABC126000410-25306AN076", 48);

		}

		[TestMethod]
		public void t_f_查詢未離站和使用的設備()
		=> _DBTest((txn) =>
		{
			var _EFQuery = txn.EFQuery_MES;

			var z = _EFQuery.f_查詢未離站記錄和使用的設備("X080213").ToList();
			FileApp._tmpJson(z);

		}, false, true);

		[TestMethod]
		public void t_同工單同站不同批號人員()
	=> _DBTest((txn) =>
	{
		var _lotInfo = txn.EFQuery_MES.WP_LOT.FirstOrDefault(c => c.LOT == "45105-2501170013-01.013");
		var _eqp_no = txn.EFQuery_MES.WP_LOT_EQP_TRACE.FirstOrDefault(c => c.LOT == _lotInfo.LOT && c.UNLOAD_LINK_SID == null)?.EQP_NO;
		/*
         select new SelectModel
                             {
                                 No = userTrace.USER_NO,
                                 SID = b.USER_SID,
                                 Display = userTrace.USER_NAME,
                                 Attr01 = b.EMP_NO,
                                 //todo-JCH:人員重覆登入
                                 Attr03 = "Test",
                             }

         */
		var _EFQuery = txn.EFQuery_MES;
		var is贴附包装 = _EFQuery.PF_OPERATION
				.FirstOrDefault(c => c.OPER_SID == _lotInfo.OPER_SID)
				.OPERATION_NO == "PCS06-";

		var 人員清單 = (from userTrace in _EFQuery.WP_USER_TRACE_IN
				join b in _EFQuery.AD_USER
				   on userTrace.USER_NO equals b.ACCOUNT_NO
				where userTrace.END_TIME == null
					&& userTrace.WO == _lotInfo.WO
					&& userTrace.ROUTE_VER_OPER_SID == _lotInfo.ROUTE_VER_OPER_SID
				group userTrace by new { userTrace.USER_NO, userTrace.USER_NAME, b.USER_SID, b.EMP_NO } into grouped
				select new
				{
					No = grouped.Key.USER_NO,
					SID = grouped.Key.USER_SID,
					Display = grouped.Key.USER_NAME,
					Attr01 = grouped.Key.EMP_NO,
					subItem = (from a in _EFQuery.WP_USER_TRACE_IN
							.Where(c => is贴附包装 && c.USER_NO == grouped.Key.USER_NO && c.END_TIME == null)
							  join b in _EFQuery.WP_LOT_EQP_TRACE.Where(c => c.UNLOAD_LINK_SID == null)
								  on a.LOT equals b.LOT
							  group a by new { a.LOT, b.EQP_NO, b.EQP_NAME } into Grp1
							  select new
							  {
								  EQP_NO = Grp1.Key.EQP_NO,
								  EQP_NAME = Grp1.Key.EQP_NAME,
								  LOT = Grp1.Key.LOT,
								  zz = Grp1.ToList()
							  }).ToList()
				}).ToList();

        FileApp._tmpJson(人員清單);


	}, true);

		[TestMethod]
        public void t_同工單同站不同批號人員_tmp()
		=> _DBTest((txn) =>
		{
			var _lotInfo = txn.EFQuery_MES.WP_LOT.FirstOrDefault(c => c.LOT == "45105-2503050002-01.006");
			var _eqp_no = txn.EFQuery_MES.WP_LOT_EQP_TRACE.FirstOrDefault(c => c.LOT == _lotInfo.LOT && c.UNLOAD_LINK_SID == null)?.EQP_NO;
		/*
		 select new SelectModel
							 {
								 No = userTrace.USER_NO,
								 SID = b.USER_SID,
								 Display = userTrace.USER_NAME,
								 Attr01 = b.EMP_NO,
								 //todo-JCH:人員重覆登入
								 Attr03 = "Test",
							 }

		 */
		//var r = (from userTrace in txn.EFQuery_MES.WP_USER_TRACE_IN
		//		//join w1 in txn.EFQuery_MES.WP_LOT_EQP_TRACE
		//		//	on userTrace.LOT equals w1.LOT into w1LeftJoin
		//		// from w1 in w1LeftJoin.DefaultIfEmpty()
		//		 join b in txn.EFQuery_MES.AD_USER
		//			on userTrace.USER_NO equals b.ACCOUNT_NO
					
		//		where userTrace.END_TIME == null
		//			&& userTrace.WO == _lotInfo.WO
		//			&& userTrace.ROUTE_VER_OPER_SID == _lotInfo.ROUTE_VER_OPER_SID
		//		group userTrace by new { userTrace.USER_NO, userTrace.USER_NAME, b.USER_SID , b.EMP_NO } into grouped
		//		select new SelectModel
		//		{
		//			No = grouped.Key.USER_NO,
		//			SID = grouped.Key.USER_SID,
		//			Display = grouped.Key.USER_NAME,
		//			Attr01 = grouped.Key.EMP_NO,
		//			//UserTraces = grouped.ToList(),
		//			subItem = (from a in txn.EFQuery_MES.WP_LOT_EQP_TRACE
		//				where grouped.Any(c=>c.LOT == a.LOT) 
		//					&& a.UNLOAD_LINK_SID == null
		//					&& (_eqp_no !=null && a.EQP_NO != _eqp_no)
		//				select new SelectModel {
		//					No = a.EQP_NO,
		//					SID = null,
		//					Display = null,
		//					Attr01 = null,
		//					//subItem = null,
		//				})
 	//					.ToList() ?? new List<SelectModel>()
		//			// 將群組內的筆數放到 List 中
		//		}).ToList();
			var queryResult = txn.EFQuery_MES.WP_USER_TRACE_IN
				.Where(userTrace => userTrace.END_TIME == null
					&& userTrace.WO == _lotInfo.WO
					&& userTrace.ROUTE_VER_OPER_SID == _lotInfo.ROUTE_VER_OPER_SID)
				.Join(txn.EFQuery_MES.AD_USER,
					userTrace => userTrace.USER_NO,
					user => user.ACCOUNT_NO,
					(userTrace, user) => new
					{
						userTrace.USER_NO,
						userTrace.USER_NAME,
						user.USER_SID,
						user.EMP_NO
					})
				.GroupJoin(txn.EFQuery_MES.WP_LOT_EQP_TRACE
					.Where(a => a.UNLOAD_LINK_SID == null
						&& (_eqp_no != null && a.EQP_NO != _eqp_no)),
					user => user.USER_NO,
					eqpTrace => eqpTrace.LOT,
					(user, eqpTraces) => new SelectModel
					{
						No = user.USER_NO,
						SID = user.USER_SID,
						Display = user.USER_NAME,
						Attr01 = user.EMP_NO,
						//subItem = eqpTraces.Select(a => new SelectModel
						//{
						//	No = a.EQP_NO,
						//	SID = null,
						//	Display = null,
						//	Attr01 = null,
						//	subItem = null
						//}).ToList()??new List<SelectModel>()
					})
				.ToList();

			FileApp._tmpJson(queryResult);

			
		}, true);

	}
}

