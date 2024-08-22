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
using static Genesis.Gtimes.Transaction.WIP.WIPTransaction;
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

	}
}

 
