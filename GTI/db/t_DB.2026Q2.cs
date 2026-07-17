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
using Moq;

namespace UnitTestProject
{

	public partial class t_DB : _testBase
	{

		[TestMethod]
		public void t_20260628_PagerQuery_IQueryable()
		=> _DBTest((txn) =>
		{
			// 1. 先建立基礎查詢（此時還不會真正去資料庫查資料）
			var _q = from A in txn.EFQuery_MES.WP_IPQC
					select A;

			// 2. 在 C# 端判斷 _q_WP_IPQC 是否有值
			IQueryable<WP_IPQC> _q_WP_IPQC = null; // 或是您的條件來源

			if (_q_WP_IPQC != null){
					// 如果 _q_WP_IPQC 有資料，才把過濾條件加上去
					// 這裡使用內聯的 Subquery，或者將其轉成清單（視資料量而定）
					_q = _q.Where(A => _q_WP_IPQC.Any(c => c.QC_NO == A.QC_NO));
			}

			// 3. 最後真正執行 ToList()，這時送出的 SQL 就會是最乾淨、最優化的結果
			var t = _q.ToList();

		}, true);
	}
}

 
