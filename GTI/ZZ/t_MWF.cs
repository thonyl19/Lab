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


		


		//=> _DBTest((txn) =>
		//{
		//	var _d = txn.EFQuery_MES.WP_WO_MTL_BOM
		//		.Where(c => c.WO == WO)
		//		.ToList();


		//}, true, true);	 
	}
}

