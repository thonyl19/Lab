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
using _bllSvc = Genesis.Library.BLL.ZZ.MWF;
using Genesis.Library.BLL.ZZ.MWF;

namespace UnitTestProject
{
	/// <summary>
    /// 台英帝國
    /// </summary>
    [TestClass]
	public class t_MWF : _testBase
	{
		static class _log
		{
			internal static string list_WP_WO_MTL_BOM
			{
				get
				{
					
					return FileApp.ts_Log(@"ZZ\MWF\WP_WO_MTL_BOM.json");
				}
			}
			internal static string Insp_WoBomMlot
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/MWF\Insp_WoBomMlot.json");
				}
			}

			internal static string t_BuildQuery_ZZ_WO_MTL_BOM_DOC
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/MWF\t_BuildQuery_ZZ_WO_MTL_BOM_DOC.json");
				}
			}
			


		}


		[TestMethod]
		public void t_Insp_WoBomMlot()
        => _DBTest((txn) =>
        {
			var WO = "51B-210100008";
			var _r = FileApp.Read_SerializeJson<List<_bllSvc.Insp_WoBomMlotService.d_WP_WO_MTL_BOM>>(_log.Insp_WoBomMlot);
			_bllSvc.Insp_WoBomMlotService.Create(txn ,WO, _r);

			var chk = (from a in txn.EFQuery_MES.ZZ_WO_MTL_BOM_DOC.Where(c => c.WO == WO)
					   join b in txn.EFQuery_MES.ZZ_WO_MTL_BOM_DOC_CHECKRECORD
						   on a.BOM_DOC_SID equals b.BOM_DOC_SID
						   into bGroup
					   from c in bGroup.DefaultIfEmpty()
					   select new { a, c }
					).AsNoTracking().ToList();
		}, true, true);	 

		[TestMethod]
		public void t_fn()
		{ 
			var WO = "51B-210100008";
			var _r = FileApp.Read_SerializeJson<List<_bllSvc.Insp_WoBomMlotService.d_WP_WO_MTL_BOM>>(_log.list_WP_WO_MTL_BOM);
			_bllSvc.Insp_WoBomMlotService.Create(WO, _r, true);
		}


		[TestMethod]
		public void t_BuildQuery_ZZ_WO_MTL_BOM_DOC()
		{
			var _r = FileApp.Read_SerializeJson<PagerQuery>(_log.t_BuildQuery_ZZ_WO_MTL_BOM_DOC);
			var _r1 = _bllSvc.Insp_WoBomMlotService.ListData(_r);
			FileApp._tmpJson(_r1);
		}

		[TestMethod]
		public void t_fnx()
	   => _DBTest((txn) =>
	   {
				//var lot = txn.GetParameterInfo(Genesis.Library.BLL.ZZ.MWF.CodeRule.系統參數.生產批號數量上下限百分比);
				//var z = txn.生產批號數量上下限百分比();
	   }, false, true);


        [TestMethod]
        public void t_fn1()
		=> _DBTest((txn) =>
		{
		 //var lot = GTI_helper.getLotInfo(txn);
		 //var zz = txn.EFQuery_MES.PF_ROUTE_VER_OPER.FirstOrDefault();
		 //var _d = txn.DapperQuery<FC_CARRIER>("SELECT * from FC_CARRIER WHERE STATE_NO = 'Idle'")
			// .FirstOrDefault();
		 //var CarrierInfo = txn.GetCarrierInfo(_d.CARRIER_NO);
		 //txn.DoTransaction(new DTC_Carrierload(CarrierInfo));
		}, false ,true);


		[TestMethod]
		public void t_PackQCServices_Query()
		=> _DBTest((txn) =>
		{
			var zz = PackQCServices.Query(txn).ToList();
			//var lot = GTI_helper.getLotInfo(txn);
			//var zz = txn.EFQuery_MES.PF_ROUTE_VER_OPER.FirstOrDefault();
			//var _d = txn.DapperQuery<FC_CARRIER>("SELECT * from FC_CARRIER WHERE STATE_NO = 'Idle'")
			// .FirstOrDefault();
			//var CarrierInfo = txn.GetCarrierInfo(_d.CARRIER_NO);
			//txn.DoTransaction(new DTC_Carrierload(CarrierInfo));
		}, false, true);



        [TestMethod]
        public void t_工時維護()
		=> _DBTest((txn) => {
			var cd_1 = new[] { "Terminated", "Finished" };
			var x =  (from zzoper in  txn.EFQuery_MES.ZZ_OPER_WORKT_SUMMARY
			where  txn.EFQuery_MES.WP_LOT.Any(c=>
					c.LOT == zzoper.LOT
					&& cd_1.Contains(c.STATUS)
				)
			 //join lot in txn.EFQuery_MES.WP_LOT
			 //on zzoper.LOT equals lot.LOT
			 //join wo in txn.EFQuery_MES.WP_WO
			 //on zzoper.WO equals wo.WO
			 //join pfrouteveroper in txn.EFQuery_MES.PF_ROUTE_VER_OPER
			 //on zzoper.ROUTE_VER_OPER_SID equals pfrouteveroper.ROUTE_VER_OPER_SID
			 //join pfroutever in txn.EFQuery_MES.PF_ROUTE_VER
			 //on pfrouteveroper.ROUTE_VER_SID equals pfroutever.ROUTE_VER_SID
			 //where new[] { "Terminated", "Finished" }.Contains(lot.STATUS)
			 select new ZZ_OPER_WORKT_ERP_SUMMARY_EXT
			 {
				 SID = zzoper.SID,
				 WO = zzoper.WO,
				 LOT = zzoper.LOT,
				 PART_NO = zzoper.PART_NO,
				 OUTPUT_QTY = zzoper.OUTPUT_QTY,
				 LINE_NO = zzoper.LINE_NO,
				 //ROUTE_NO = pfrouteveroper.ROUTE_NO,
				 //ROUTE_VERSION = pfroutever.VERSION.ToString(),
				 STATUS = zzoper.STATUS,
				 REPORT_USER_NUM = zzoper.REPORT_USER_NUM,
				 WORK_TOTAL = zzoper.WORK_TOTAL,
				 EQUIPMENT_TOTAL = zzoper.EQUIPMENT_TOTAL,
				 REPORT_DATE = zzoper.WORK_DATE,
				 DATA_TYPE = zzoper.DATA_TYPE,
				 NOTE = zzoper.NOTE,
				 ROUTE_VER_OPER_SID = zzoper.ROUTE_VER_OPER_SID,
				 ACTION_LINK_SID = zzoper.ACTION_LINK_SID,
				 CREATE_USER = zzoper.CREATE_USER,
				 CREATE_DATE = zzoper.CREATE_DATE,
				 UPDATE_USER = zzoper.UPDATE_USER,
				 UPDATE_DATE = zzoper.UPDATE_DATE,
				 //ATTRIBUTE_03 = wo.ATTRIBUTE_03,
			 }).ToList();
 

		}, true);

		[TestMethod]
		public void t_工時維護1()
	   => _DBTest((txn) => {
		   var cd_1 = new[] { "Terminated", "Finished" };
		   var _sql = @"
		   select	* 
		   from		ZZ_OPER_WORKT_SUMMARY z0
		   where	exists 
					(select * from WP_LOT w01
					where w01.lot = z0.lot
						and w01.STATUS in ('Terminated','Finished'))
		   
		   ";

		   var x = txn.DapperQuery(_sql);


	   }, true);

	}
}

