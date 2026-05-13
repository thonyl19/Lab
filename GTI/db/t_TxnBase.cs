using BLL.Base;
using BLL.DataViews.Res;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Dal.Repository;
using Dapper;
using Frame.Code;
using Frame.Code.Web.Select;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.Transaction;
using Genesis.Library.BLL.MES.AutoGenerate;
using Genesis.Library.BLL.MES.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static BLL.MES.WIPServices;
using mdl = MDL.MES;
using vDbCtx = MDL.MESContext;
using _mdl_mvc = MDL.GenesisMVC.Tables;
using MDL;
using static BLL.MES.TableQueryService;
using Genesis.Gtimes.Transaction.WIP;

namespace UnitTestProject
{
	//TODO-BK
	[TestClass]

	public partial class t_TxnBase : _testBase
	{
		static class _log
		{
			internal static string t_Process_PosiMap_cmd_CHANGE_SERIAL
			{
				get
				{
					return FileApp.ts_Log(@"DB\t_Process_PosiMap_cmd_CHANGE_SERIAL.json");
				}
			}
		}


        [TestMethod]
        public void t_fn()
		=> _DBTest((txn) =>
		{
			var _d = txn.LzQuery.EQP.取得機台上的治具清單("T-RF-01").ToList();
			
		}, true,true);

		[TestMethod]
		public void t_20251223_Update_ENABLE_FLAG()
		=> _DBTest((txn) =>
		{

			var data1
				= txn.EFQuery_MES.view_EqpExt().FirstOrDefault();
				//= new d_MainExt<FC_EQUIPMENT, FC_EQUIPMENT_EXT>(); // 這樣會報錯,因為 子項的Main/Ext 仍是 null;
			txn.Update_ENABLE_FLAG(data1.Main, "EQP_SID");

			txn.Update_ENABLE_FLAG(data1.Main, "EQP_SID", _ExtCheck: (
				c => Check.Invalid("狀態為 Run,不允許停用", c.ENABLE_FLAG == "F" && c.STATE_NO == "Run", c)
			));


			var model = txn.EFQuery_MES.PF_OPERATION.AsNoTracking().FirstOrDefault();
			model.ENABLE_FLAG = model.ENABLE_FLAG=="F"?"T":"F";

			txn.Update_ENABLE_FLAG(model, "OPER_SID");
			//txn.Update_ENABLE_FLAG(model, model.DynWhereExpression("OPER_SID"));
			txn.Update_ENABLE_FLAG(model, (c=> c.ENABLE_FLAG  == model.ATTRIBUTE_01));

			

		}, true, true);


		[TestMethod]
		public void t_DynWhereExpression()
		=> _DBTest((txn) =>
		{
			var model = txn.EFQuery_MES.PF_OPERATION.AsNoTracking().FirstOrDefault();

			var dynWhere1 = model.DynWhereExpression("OPER_SID");
			var r1 = txn.EFQuery_MES.PF_OPERATION.Where(dynWhere1).FirstOrDefault();

			var dynWhere2 = model.DynWhereExpression("OPERATION_NO", model.OPERATION_NO);
			var r2 = txn.EFQuery_MES.PF_OPERATION.Where(dynWhere2).FirstOrDefault();
		}, true, true);



	}
}

 
