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
		public void _MockDB_單一張表()
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


		[TestMethod]
		public void t_測試d_MasterDetail的應用()
		=> _DBTest((txn) => {
			var query = txn.EFQuery_MES
				.view_UserTrace()
				.FirstOrDefault(c => c.Main.IN_MASTER_SID == "GTI25031809251667011");

		}, false);

		/*
		這個功能當無法實現,因為 在 c#7 版本中 , 兩表 join 時的  equals 表示式 ,
			無法使用 Expression 來完成 ,
		 */
		[TestMethod]
		public void x_QueryMainExt()
		=> _DBTest((txn) =>
		{
			//var t = txn.EFQuery_MES.view_EqpExt().FirstOrDefault();
			//var t1 = t.Ext?.MAX_USE_COUNT;

			var query = txn.EFQuery_MES.QueryMainExt(
					txn.EFQuery_MES.FC_EQUIPMENT,         // 主表
					txn.EFQuery_MES.FC_EQUIPMENT_EXT,     // 擴展表
					eq => eq.EQP_SID,           // 連接條件 (主表)
					ext => ext.EQP_SID          // 連接條件 (擴展表)
				).FirstOrDefault();

		}, false);




		



	}

	public class d_MasterDetail<TMain, TExt>
	{
		public TMain Main { get; set; }
		public List<TExt> Exts { get; set; } = new List<TExt>();
	}


	public static class t_DB_Ext {

		public static IQueryable<d_MainExt<TMain, TExt>> QueryMainExt<TMain, TExt, TKey>(
			this MDL.MESContext _self,
			IQueryable<TMain> mainTable,
			IQueryable<TExt> extTable,
			Expression<Func<TMain, TKey>> mainKeySelector,
			Expression<Func<TExt, TKey>> extKeySelector
		) where TMain : class where TExt : class
		{
			return from a in mainTable
				   join b in extTable
						//on mainKeyFunc(a) equals extKeyFunc(b) into aJoin
						on mainKeySelector.Compile()(a) equals extKeySelector.Compile()(b) into aJoin
						//on mainKeySelector.Body equals extKeySelector.Body into aJoin
				   from b in aJoin.DefaultIfEmpty()
				   select new d_MainExt<TMain, TExt>
				   {
					   Main = a,
					   Ext = b
				   };
		}


		public static IQueryable<d_MasterDetail<WP_USER_TRACE_IN_MASTER, WP_USER_TRACE_IN>> view_UserTrace(this MDL.MESContext _self)
		{
			return from a in _self.WP_USER_TRACE_IN_MASTER
				   join b in _self.WP_USER_TRACE_IN
					   on a.IN_MASTER_SID equals b.IN_MASTER_SID into aGroup
				   select new d_MasterDetail<WP_USER_TRACE_IN_MASTER, WP_USER_TRACE_IN>
				   {
					   Main = a,
					   Exts = aGroup.ToList() // 將分組的 Ext 記錄轉換為 List
				   };
		}
	}

}

 
