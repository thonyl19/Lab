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
		public void _測試組合表()
		=> _DBTest((txn) =>
		{
			var _d = txn.LzQuery.WIP.v批號平行工作站_SN過站記錄
				.Where(c => c.WP_LOT_OPER_PARALLEL.LOT == "JTest0716-01");


			FileApp._tmpJson(_d.ToList());
		}, false, true);

		/// <summary>
		/// 測試 GTI_Txn 4.5 語法  與 EF 併行 , 基本上己經成功完成 ,但需要注意一點的地方就是 ,
		/// EF 要讀取 GTI_Txn 4.5 執行後的資料 , 必須一定要使用 AsNoTracking() ,
		///		否則取到的 會是 GTI_Txn 4.5 執行前的資料
		///	但測試時發現  GTI_Txn 4.5 在讀取 EF 的異動資料 ,並沒有 前述的情形
		///	
		/// 20240709) 後來再優化 EFQuery_MES 後 , AsNoTracking 已經非必要 , 確定可以取得最新資料
		/// </summary>
		[TestMethod]
		public void _EFQuery_MES_Transaction測試()
		=> _DBTest((txn) => {
			/*
			 select  db_name(dbid) as dbname , count(*) 'connections count'
				from master..sysprocesses
				where spid > 50 and db_name(dbid) = 'JOCHUXM15_GTIMES5'
				group by  db_name(dbid)
				order by count(*) desc
			 
			 */
			var rr1 = App.Timer(() => {
				txn.EFQuery_MES.Configuration.LazyLoadingEnabled = true;
				var zz1 = txn.EFQuery_MES.WP_LOT.FirstOrDefault(c => c.STATUS == "Run");
				return zz1;
			});

			var x = txn.EFQuery_MES.WP_LOT.FirstOrDefault(c => c.STATUS == "Run");

			var _lot = txn.GetLotInfo(x.LOT_SID);

			txn.DoTransaction(new WIPTransaction.HoldLotTxn(_lot)
				, new WIPTransaction.EndOfLotTxn(_lot));

			x.ATTRIBUTE_01 = "TEST2";
			txn.EFQuery_MES.SaveChanges();

			var x1 = txn.EFQuery<WP_LOT>().Read(c => c.LOT_SID == x.LOT_SID);
			/*
			此說明前的程序, 都只會共用一條連線 
			*/



			//Check.Invalid("", true);
			_lot = _lot.ReLoad(txn.DBC);
			var r = TxnBase.LzDBQuery(ttx => {
				var zz = ttx.EFQuery_MES.WP_LOT.FirstOrDefault();
				return ttx.result;
			});

			var r1 = TxnBase.LzDBQuery(ttx => {
				var zz = ttx.EFQuery_MES.WP_LOT.FirstOrDefault();
				return ttx.result;
			});


			/// 這段 語法一定會掛掉 , 因為 在 Transaction 機制下, 沒辦法再做 read()
			var r2 = TxnBase.LzDBQuery(ttx => {
				var zz = ttx.EFQuery_MES.WP_LOT.FirstOrDefault();
				return ttx.result;
			}, txn.DBC);


		}, true, true);

		[TestMethod]
		public void _測試組合表1()
		=> _DBTest((txn) =>
		{
			var mockSet = new Mock<DbSet<WP_LOT_OPER_PARALLEL_SN>>();

			// 模擬資料
			var mockData = new List<WP_LOT_OPER_PARALLEL_SN>
			{
				new WP_LOT_OPER_PARALLEL_SN { LOT = "TestLot", SN = "TestSN" }
			};
			//var zz = mockData.ToList();

			// 設定 DbSet 的行為
			mockSet.As<IQueryable<WP_LOT_OPER_PARALLEL_SN>>()
				.Setup(m => m.Provider)
				.Returns(mockData.AsQueryable().Provider);
			mockSet.As<IQueryable<WP_LOT_OPER_PARALLEL_SN>>()
				   .Setup(m => m.Expression)
				   .Returns(mockData.AsQueryable().Expression);
			mockSet.As<IQueryable<WP_LOT_OPER_PARALLEL_SN>>()
				   .Setup(m => m.ElementType)
				   .Returns(mockData.AsQueryable().ElementType);
			//mockSet.Setup(m => m.ToList())
			//	   .Returns(mockData);
			// 模擬 DbContext
			
			var mockContext = new Mock<MESContext>(); // YourDbContext 為你的實際 DbContext
			mockContext.Setup(m => m.WP_LOT_OPER_PARALLEL_SN).Returns(mockSet.Object);

			//txn.DbContext = mockContext;

			// 測試查詢邏輯
			var rows_過站記錄 = mockContext.Object.WP_LOT_OPER_PARALLEL_SN
				.Where(c => c.LOT == "TestLot" && c.SN == "TestSN")
				.ToList();

			Assert.AreEqual(1, rows_過站記錄.Count);
			//FileApp._tmpJson(_d.ToList());
		}, false, true);
	} 
}

 
