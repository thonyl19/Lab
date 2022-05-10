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
using static BLL.MES.WIPServices;
using mdl = MDL.MES;
using vDbCtx = MDL.MESContext;

namespace UnitTestProject
{
	//TODO-BK
	[TestClass]
	public class t_DB : _testBase
	{
		public string _path = @"C:\Code\GTIMES_2015\UnitTestProject\Log\";
		static class _log
		{
            internal static  string t_Process_PosiMap_cmd_CHANGE_SERIAL
			{
				get
				{
					return FileApp.ts_Log(@"DB\t_Process_PosiMap_cmd_CHANGE_SERIAL.json");
				}
			}

			internal static string t_BuildQuery
			{
				get
				{
					return FileApp.ts_Log(@"DB\t_BuildQuery.json");
				}
			}
			internal static string t_Linq_ToDictionary
			{
				get
				{
					return FileApp.ts_Log(@"DB\t_Linq_ToDictionary.json");
				}
			}


		}

		[TestMethod]
		public void t_取得AppConfig中ConnectionStringSettings()
		{
			ConnectionStringSettingsCollection settings =
			   ConfigurationManager.ConnectionStrings;

			if (settings != null)
			{
				foreach (ConnectionStringSettings cs in settings)
				{
					Console.WriteLine(cs.Name);
					Console.WriteLine(cs.ProviderName);
					Console.WriteLine(cs.ConnectionString);
					//DBController db = new DBController(cs);
				}
			}
			//Assert.IsTrue(CheckInfo.T("s"));
		}



		[TestMethod]
		public void t_SQL_查詢範例()
		{

			using (var dbc = this.DBC)
			{
				var _sql = $@"
                SELECT  top 1 *
                FROM    PF_PARTNO_VER

                         ";

				var dt = dbc.Select(_sql);

				new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}

		[TestMethod]
		public void t_SQL_查詢範例1()
		{

			using (var dbc = this.DBC)
			{
				var _sql = $@"
                
				select  count(*)
				FROM	ZZ_OPER_WORKT_SUMMARY
				where TX_DATE = @TX_DATE

                         ";
				var arg = new { TX_DATE = "2020/11/10" };
				var dt = dbc.GetConnection().Query(_sql, arg).ToList();

				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}

		[TestMethod]
		public void t_SQL_查詢範例_帶參數()
		{
			var LOT_SID = "LOT_SID";

			using (var dbc = this.DBC)
			{
				var _sql = $@"
                SELECT  *
                FROM    ZZ_LOT_BIN
                WHERE   LOT_SID = :LOT_SID

                         ";

				List<IDbDataParameter> parameters = new List<IDbDataParameter>();
				dbc.AddCommandParameter(parameters, "LOT_SID", LOT_SID);
				_sql = dbc.GetCommandText(_sql, SQLStringType.SqlServerSQLString);

				var dt = dbc.Select(_sql, parameters);

				new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}
		[TestMethod]
		public void t_SQL_查詢範例_帶參數_使用Dapper()
		{

			using (var dbc = this.DBC)
			{
				string FROM_ROUTE_VER_OPER_SID = null;
				var _sql = @"
					SELECT  ISNULL(@FROM_ROUTE_VER_OPER_SID,'') AS ｘ
					FROM	PF_ROUTE_VER_OPER_PATH
					WHERE	(ISNULL(@FROM_ROUTE_VER_OPER_SID,'') ='' 
								OR FROM_ROUTE_VER_OPER_SID = @FROM_ROUTE_VER_OPER_SID)
				";
				var _re = dbc.GetConnection()
					.Query(_sql, new { FROM_ROUTE_VER_OPER_SID })
					.ToList();

				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}

		[TestMethod]
		public void t_使用BuildQuery_Group()
		{
			var obj = new FileApp().Read_SerializeJson<PagerQuery>(_log.t_BuildQuery);
			var Result = ZZServices.QueryWORKT(obj);
		}



		[TestMethod]
		public void t_SQL_Dapper()
		{
			var r = DDLServices.get_PARAMETERS("WorkingTimeType");
		}

		[TestMethod]
		public void t_SQL_Dapper_1()
		{
			var x = TableQueryService.AD_PARAMETER_ByGroupNo("AttendanceType")
					.Where(c => c.PARAMETER_NO == "Absencehours")
					.FirstOrDefault();

		}

		[TestMethod]
		public void t_SQL_Dapper_3()
		{
			var _list = TableQueryService
					.AD_SHIFT("%", MDL.SearchKey.All)
					.Where(s => s.ENABLE_FLAG == "T")
					.Select(s => new SelectModel
					{
						SID = s.SHIFT_SID,
						No = s.SHIFT_NO,
						Display = s.SHIFT,
						Value = s.SHIFT_SID,
					})
					.ToList();

		}

		[TestMethod]
		public void t_x()
		{
			//var _REPORT = TableQueryService.AD_PARAMETER_ByGroupNo("WorkingTimeType");
			//var is實際工時 = false;
			//var r = _REPORT
			//	.Where(c => (c.PARAMETER_NO == "ActualWorkingHours") == is實際工時)
			//	.ToList();

			//var _r1 = TableQueryService.AD_SHIFT("A", MDL.SearchKey.All)
			//		.ToList();

			//var r2 = v8n_ZZ_EMP_ATTEND.chk_OldRec("1234");
			//var rr1 = App.Timer(() =>
			//{
			//	var arr = new string[] { "EQP-023", "EQP-019" };
			//	var r = TableQueryService
			//				.FC_EQUIPMENT(null, MDL.SearchKey.FullTable)
			//				.Where(c => arr.Contains(c.EQP_NO))
			//				//.ToList()
			//				;
			//	return r;
			//});
			var arg = new ExpandoObject() as IDictionary<string, Object>;
			//var _arg = arg as IDictionary<string, Object>;
			var x = "AAA";
			arg[x] = "test";
		}


		/// <summary>
		/// 由 MDL.MESContext 轉型態為 IDbConnection 再調用  純 SqlCmd 執行
		/// </summary>
		/// <param name="Search"></param>
		/// <param name="SearchType"></param>
		/// <param name="DbCtx"></param>
		/// <returns></returns>
		public static IQueryable<FC_EQUIPMENT> FC_EQUIPMENT(string Search = null, MDL.SearchKey SearchType = MDL.SearchKey.SID
			, DbContext DbCtx = null)
		{
			return FC_EQUIPMENT(Search, SearchType, DbCtx.Database.Connection);
		}

		/// <summary>
		/// 純 SqlCmd 執行範本 
		/// </summary>
		/// <param name="Search"></param>
		/// <param name="SearchType"></param>
		/// <param name="cnn"></param>
		/// <returns></returns>
		public static IQueryable<FC_EQUIPMENT> FC_EQUIPMENT(string Search = null, MDL.SearchKey SearchType = MDL.SearchKey.SID
			, IDbConnection cnn = null)
		{
			if (Search == "%") Search = "";
			using (var _cnn = cnn ?? mes.dbc().GetConnection())
			{
				var _sql = @"
					SELECT 	*
					FROM	FC_EQUIPMENT WITH(nolock) 
					WHERE	1=1
							--[SID]-- AND EQP_SID = @SID 
							--[Name]-- AND EQP_NAME = @Name 
							--[No]-- AND EQP_NO = @No
							--[All]-- AND (EQP_SID LIKE @ALL OR EQP_NAME LIKE @ALL OR EQP_NO LIKE @ALL)
									
				";
				dynamic arg = new { };// parseArg(SearchType, Search, ref _sql);
				var _re = _cnn
						.Query<FC_EQUIPMENT>(_sql, (object)arg)
						.AsQueryable()
						;
				return _re;
			}
		}

		/// <summary>
		///	主要是以 MESContext ,無法以 mes.dbc 做傳入 ,因為物件型態不同 ,
		///		mes.dbc 內根本就沒有 DbSet 相關的物件
		/// </summary>
		/// <param name="Search"></param>
		/// <param name="SearchType"></param>
		/// <param name="DbCtx"></param>
		/// <returns></returns>

		public IQueryable<FC_EQUIPMENT> FC_EQUIPMENT(string Search = null, MDL.SearchKey SearchType = MDL.SearchKey.SID
			, vDbCtx DbCtx = null)
		{
			if (Search == "%") Search = "";
			using (var _DbCtx = DbCtx ?? new MDL.MESContext())
			{
				return (from t in _DbCtx.FC_EQUIPMENT
						where
					   (SearchType == MDL.SearchKey.SID && t.EQP_SID == Search)
					   || (SearchType == MDL.SearchKey.Name && t.EQP_NAME == Search)
					   || (SearchType == MDL.SearchKey.No && t.EQP_NO == Search)
					   || (SearchType == MDL.SearchKey.All
						   && (t.EQP_SID.Contains(Search)
						   || t.EQP_NAME.Contains(Search)
						   || t.EQP_NO.Contains(Search)))
						select t)
					.AsQueryable();
			}

		}

		public static IQueryable<PF_OPERATION> PF_OPERATION(string Search, MDL.SearchKey SearchType = MDL.SearchKey.SID, IDbConnection cnn = null)
		{
			using (var _cnn = mes.dbc())
			{
				//var z = new MDL.MESContext();
				//z.Database.Connection
				var _re = (from t in new MDL.MESContext().PF_OPERATION
						   where
						   (SearchType == MDL.SearchKey.FullTable
						   || SearchType == MDL.SearchKey.SID && t.OPER_SID == Search)
						   || (SearchType == MDL.SearchKey.Name && t.OPERATION == Search)
						   || (SearchType == MDL.SearchKey.No && t.OPERATION_NO == Search)
						   || (SearchType == MDL.SearchKey.All
							   && (t.OPER_SID.Contains(Search)
							   || t.OPERATION.Contains(Search)
							   || t.OPERATION_NO.Contains(Search)))
						   select t).AsQueryable()
					;
				return _re;
			}
		}

		[TestMethod]
		public void t_效能測試()
		{
			var rr3_A = App.Timer(() =>
			 {
				 var arr = new string[]
					 //{ "SMD", "ALD" };
					 {"1003-1","1003-2","1003-3"};
				 var r = PF_OPERATION("", MDL.SearchKey.FullTable)
					 .Where(c => arr.Contains(c.OPERATION_NO))
					 .ToList()
					 ;
				 return r;

			 });
			//var rr3_1 = App.Timer(() =>
			//{
			//	var arr = new string[]
			//		//{ "SMD", "ALD" };
			//		{"SMD 白班","OVP夜班","OVP夜班"};
			//	var r = TableQueryService
			//		.AD_SHIFT("", MDL.SearchKey.FullTable)
			//		.Where(c => arr.Contains(c.SHIFT))
			//		.ToList()
			//		;
			//	return r;

			//});

			//var rr3 = App.Timer(() =>
			//{
			//	var arr = new string[]
			//		//{ "SMD", "ALD" };
			//		{"SMD 白班","OVP夜班","OVP夜班"};
			//	var r = TableQueryService
			//		.AD_SHIFT(arr, MDL.SearchKey.Name)
			//		//.Where(c => arr.Contains(c.EQP_NO))
			//		.ToList()
			//		;
			//	return r;

			//});

		}


		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void t_效能測試1()
		{
			/*
			//原始語法 
			TableQueryService.WP_USER_TRACE()
				.Where(x =>

					x.ACTION_LINK_SID == "GTI21011112223380094"
				//&& x.LOT == 'WB127C21011PE'
				)
				.Select(x => new { x.USER_NO, x.USER_NAME })
				.ToList()
			*/

			var rr3_B = App.Timer(() =>
			{
				return null;
				var _sql = @"
					SELECT 	*
					FROM	WP_USER_TRACE WITH(nolock) 
				";
				var _re = this.DBC.GetConnection()
						.Query<WP_USER_TRACE>(_sql)
						.AsQueryable()
						;
				/*
				經過實測發現 , 這裡的  AsQueryable 並不是如預期的只先產生語法 ,
					而是真的就直接把資料 query 回來了 ,所以 很慢的原因點就是緣於此
				 */

				var z1 = _re.Where(x =>

						x.ACTION_LINK_SID == "GTI21011112223380094"
					//&& x.LOT == 'WB127C21011PE'
					)
					.Select(x => new { x.USER_NO, x.USER_NAME })
					;
				return z1;
			});



			var rr3_C = App.Timer(() =>
			{
				//return null;
				var z1 = TableQueryService.WP_USER_TRACE(null, MDL.SearchKey.FullTable, new MDL.MESContext())
				.Where(x =>

					x.ACTION_LINK_SID == "GTI21011112223380094"
				//&& x.LOT == 'WB127C21011PE'
				)
				.Select(x => new { x.USER_NO, x.USER_NAME });
				var z2 = z1.ToList();
				return z2;

			});

			var rr3_D = App.Timer(() =>
			{
				BaseWpUserTraceServices svc_WP_USER_TRACE = new BaseWpUserTraceServices();
				svc_WP_USER_TRACE.DbContext = new MDL.MESContext();
				var z1 = (from t in svc_WP_USER_TRACE.GetAllListIQueryable()
						  select t
						  ).AsQueryable<WP_USER_TRACE>();
				var z2 = z1.Where(t => t.ACTION_LINK_SID == "GTI21011112223380094")
						//&& (t.LOT == LOT)
						.Select(t => new
						{
							t.USER_NO,
							t.USER_NAME
						}
						);
				var z3 = z2.ToList<dynamic>();
				return z3;

			});
		}

		[TestMethod]
		public void t_SQL_Update_帶參數()
		{
			using (var dbc = this.DBC)
			{
				var _sql = @"
					
					INSERT	INTO AD_LOG 
							([FUN_NAME], [ACTION], [TARGET_TABLE]
							, [TARGET_PK], [TARGET_IDENTITY], [VALUE_LINK_SID]
							, [CREATE_USER], [CREATE_DATE])
					SELECT	'WorkingTime' , 'BatchUpdateStatus' , 'ZZ_OPER_WORKT_SUMMARY' 
							, 'Batch' , :LINK_SID AS  TARGET_PK , :LINK_SID AS VALUE_LINK_SID 
							, :CREATE_USER AS CREATE_USER , :CREATE_DATE AS CREATE_DATE;
					
				";
				//var currentUser = true
				//		? new CurrentLoginUserModel() { UserName = "admin_test" }
				//		: GetLoginUser();
				//string starDate = Convert.ToDateTime(WORK_DATE).ToString("yyyy-MM-dd") + " 00:00:00";
				//string endDate = Convert.ToDateTime(WORK_DATE).ToString("yyyy-MM-dd") + " 23:59:59";
				List<IDbDataParameter> parameters = new List<IDbDataParameter>();
				//parameters.Add(dbc.CreateCommandParameter(":LINK_SID", GetSID()));
				//parameters.Add(dbc.CreateCommandParameter(":CREATE_USER", currentUser.UserName));
				//parameters.Add(dbc.CreateCommandParameter(":CREATE_DATE", GetSysDBTime()));
				_sql = dbc.GetCommandText(_sql, SQLStringType.OracleSQLString);
				try
				{
					dbc.Execute(_sql, parameters);
					//var dt = dbc.Select(_sql, parameters);
				}
				catch (Exception Ex)
				{
					//throw;
				}
				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}

		/// <summary>
		/// EFRepository 範例
		/// </summary>
		[TestMethod]
		public void t_WO_check()
		{
			using (var dbContext = new MDL.MESContext())
			{
				var _refServices = new EFRepository<mdl.WP_WO>(dbContext);

				Func<string, Expression<Func<mdl.WP_WO, bool>>> expression = (string WO) =>
				{
					return ExtLinq.True<mdl.WP_WO>()
						.And(t => t.WO == WO);
				};

				//Expression<Func<mdl.WP_WO, bool> expression_A => <T>()
				//{
				//	return ExtLinq.True<mdl.WP_WO>()
				//		.And(t => t.WO == WO);
				//};

				/* 另一等價的寫法 
				 
				var expression1 = ExtLinq.True<mdl.WP_WO>();
				var keyword = "keyword";
				if (!string.IsNullOrEmpty(keyword))
				{
					expression1 = expression1
						.And(t => t.WO.Contains(keyword))
						.Or(t => t.WO.Contains(keyword));
				}
				*/
				var item1 = _refServices.Read(expression("20120903-0001"));
				new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"WO\t_WP_WO.json"));
				var item2 = _refServices.Read(expression("20120903-0001x"));
			}
		}

		/// <summary>
		/// ReadsList
		/// </summary>
		[TestMethod]
		public void t_ReadsList()
		{
			using (var dbContext = new MDL.MESContext())
			{
				var _sql = $@"SELECT * from WP_WO where WO = @WO";
				var _refServices = new EFRepository<mdl.WP_WO>(dbContext);

				//var _r = _refServices.ReadsList(_sql);
				// 使用參數

				var p1 = new DbParameter[] {
					new SqlParameter(){ ParameterName="@WO",SqlDbType=SqlDbType.NVarChar,Value="20100430-0001"}
				};

				//{"SqlParameterCollection 只接受非 Null 的 SqlParameter 型別物件，不接受 EntityParameter 物件。"}
				var p2 = new EntityParameter[] {
					new EntityParameter(){ParameterName = "WO" ,Value = "20100430-0001" }
				};

				//var p3 = (DbParameter[])new IDbDataParameter[] {
				//	_refServices.CreateCommandParameter("@WO","20100430-0001")
				//};
				var p4 = new SqlParameter("@WO", "20100430-0001");
				var p5 = _refServices.CreateCommandParameter("@WO", "20100430-0001");
				var _r1 = _refServices.ReadsList(_sql, p5);


				//System.Data.SqlClient.SqlException: '必須宣告純量變數 "WO"。'
				//var _r1 = _refServices.ReadsList(_sql, new object[] { "WO", "20100430-0001" });
				new FileApp().Write_SerializeJson(_r1, FileApp.ts_Log(@"DB\t_ReadsList.json"));

			}
		}

		/// <summary>
		/// EFRepository 範例
		/// </summary>
		[TestMethod]
		public void t_EFRepository_Query()
		{
			using (var dbContext = new MDL.MESContext())
			{
				var _sql = $@"SELECT top 1 * from WP_WO";
				var _r = dbContext.Database
						// EF 原生用法
						//.SqlQuery<mdl.WP_WO>(_sql)
						// Dapper 用法
						.Connection.Query(_sql)
						.ToList();

				new FileApp().Write_SerializeJson(_r, FileApp.ts_Log(@"DB\t_SqlQuery.json"));

			}
		}

		/// <summary>
		/// EFRepository 新增範例
		/// </summary>
		[TestMethod]
		public void t_EFRepository新增範例()
		{
			using (var dbContext = new MDL.MESContext())
			{
				var _AD_LOG = new mdl.AD_LOG()
				{
					FUN_NAME = "t1",
					ACTION = "ACTION",
					TARGET_TABLE = "ZZ_OPER_WORKT_SUMMARY",
					TARGET_PK = "TARGET_PK",
					TARGET_IDENTITY = "TARGET_IDENTITY",
					CREATE_USER = "CREATE_USER",
					CREATE_DATE = DateTime.Now,
				};
				dbContext.AD_LOG.Add(_AD_LOG);
				dbContext.SaveChanges();
			}
		}



		/// <summary>
		///  SqlCommandBuilder 實作範例 
		/// </summary>
		[TestMethod]
		public void t_Txn_SqlCommandBuilder()
		{
			var commands = new List<IDbCommand>();
			var _sql = $@"
                    UPDATE  ZZ_LOT_BIN
                    SET     STATUS = :STATUS ,
                            ACTION_LINK_SID = :ACTION_LINK_SID ,
                            ACTION_LINK_SID_TRACE = ACTION_LINK_SID_TRACE + ';' + :ACTION_LINK_SID 
                    WHERE   LOT_BIN_SID = :LOT_BIN_SID
                ";
			using (var DBC = this.DBC)
			{
				string linkSID = DBC.GetSID();
				DateTime txnTime = DBC.GetDBTime();

				TransactionUtility.TransactionBase txnBase = new TransactionUtility.TransactionBase(linkSID, "test", txnTime, "FunctionRightName");
				TransactionUtility.GtimesTxn gtimesTxn = new TransactionUtility.GtimesTxn(this.DBC, txnBase);
				TransactionUtility.AddSQLCommandTxn sqlcmd = new TransactionUtility.AddSQLCommandTxn();

				using (IDbTransaction tx = DBC.GetTransaction())
				{
					List<IDbDataParameter> parameters = new List<IDbDataParameter>();
					DBC.AddCommandParameter(parameters, "LOT_BIN_SID", "372cd879-b988-4fa9-965e-2d607e898f9d");
					DBC.AddCommandParameter(parameters, "ACTION_LINK_SID", "ACTION_LINK_SID");
					DBC.AddCommandParameter(parameters, "STATUS", true);
					_sql = DBC.GetCommandText(_sql, SQLStringType.SqlServerSQLString);

					var cmd = DBC.CreateCommand(_sql, parameters);
					commands.Add(cmd);

					sqlcmd.Commands.AddRange(commands);
					gtimesTxn.Add(sqlcmd);
					gtimesTxn.DoTransaction(gtimesTxn.GetTransactionCommands(), tx);
					sqlcmd.Commands.Clear();
					gtimesTxn.Clear();
					tx.Commit();

				}
			}
		}
		/// <summary>
		/// SelectCommandBuilder IsNull 的正確用法 
		/// </summary>
		[TestMethod]
		public void t_SelectCommandBuilder_Isnull()
		{
			SelectCommandBuilder select = new SelectCommandBuilder(this.DBC, "WP_EQP_TOOL_LIST");
			select.JoinTable("FC_TOOL");
			select.On("WP_EQP_TOOL_LIST", "TOOL_NO", "FC_TOOL", "TOOL_NO");

			//模製具資料
			select.SelectColumn("WP_EQP_TOOL_LIST", "TOOL_SID");
			select.SelectColumn("WP_EQP_TOOL_LIST", "TOOL_NO");
			select.SelectColumn("WP_EQP_TOOL_LIST", "TOOL_NAME");

			//下機台時間
			select.SelectColumn("WP_EQP_TOOL_LIST", "UNLOAD_TIME");
			//下機台連結SID
			select.SelectColumn("WP_EQP_TOOL_LIST", "UNLOAD_LINK_SID");

			//設備資料
			select.SelectColumn("WP_EQP_TOOL_LIST", "EQP_SID");
			select.SelectColumn("WP_EQP_TOOL_LIST", "EQP_NO");
			select.SelectColumn("WP_EQP_TOOL_LIST", "EQP_NAME");

			//使用次數
			select.SelectColumn("FC_TOOL", "USE_COUNT");
			//最大使用次數
			select.SelectColumn("FC_TOOL", "MAX_USE_COUNT");


			//Null 查詢的正確用法
			select.WhereAnd("UNLOAD_LINK_SID?", null);

			DataTable dt = select.DoQuery();
			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\t_SelectCommandBuilder_Isnull.json"));

		}


		[TestMethod]
		public void t_Select_DataTable()
		{
			//var select = new SelectCommandBuilder(this.DBC);
			//select.FromTable("WP_LOT", "A")
			//	.SelectColumn("*")
			//	.WhereAnd("A","LOT", SQLOperator.Equal,"SUX979.01");

			var select = new SelectCommandBuilder(this.DBC, "WP_LOT");
			select.SelectColumn("*")
				.WhereAnd("LOT", "SUX979.01");
			DataTable dt = select.DoQuery();
			//var Row = dt.Rows[0];
			//var _t = Row["A"].ToString();
			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\t_Select_DataTable.json"));

		}
		/// <summary>
		/// ReadsList
		/// </summary>
		//[TestMethod]
		//public void tt() {
		//	using (var dbContext = new MDL.MESContext())
		//	{
		//		var _refServices = new EFRepository<mdl.WP_WO>(dbContext);

		//		Func<string, Expression<Func<mdl.WP_WO, bool>>> expression = (string WO) =>
		//		{
		//			return ExtLinq.True<mdl.WP_WO>()
		//				.And(t => t.WO == WO);
		//		};


		//		var item1 = _refServices.Read(expression("20120903-0001"));
		//		new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"WO\t_WP_WO.json"));
		//		var item2 = _refServices.Read(expression("20120903-0001x"));
		//	}
		//	//ExtLinq.True<mdl.WP_WO>()
		//	//var _svc = _pubSVC<mdl.WP_WO>.Search((t => { t.WO = "20120903-0001"});
		//	ADMServices.AD_LINE(mdl.AD_LINE.SID("abcd"));
		//}

		/// <summary>
		/// ReadsList
		/// </summary>
		[TestMethod]
		public void t_AD_LINE()
		{
			//var _r = ADMServices.AD_LINE("a線別", mdl.AD_LINE.Key.Name);
			var _r1 = ADMServices.AD_LINE();
			//new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"WO\t_WP_WO.json"));

		}

		[TestMethod]
		public void __Pager()
		{

			using (var dbc = this.DBC)
			{
				string FROM_ROUTE_VER_OPER_SID = null;
				var _sql = @"
					SELECT   
					FROM	WP_WO AS A WIDTH (nolock)
					WHERE	1=1 
				";
				var _re = dbc.GetConnection()
					.Query(_sql, new { FROM_ROUTE_VER_OPER_SID })


					;
				var records = _re.Count();
				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}

		[TestMethod]
		public void t_DynLinq()
		{

			using (var cnn = new MDL.MESContext())
			{
				var example1 = cnn.WP_LOT
					.Where("LOT_SID == @0 ", "GTI20021013194301437")
					.ToList();

				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}



		[TestMethod]
		public void t_()
		{

			using (var cnn = new MDL.MESContext())
			{
				var _sql = @"
SELECT 	LOT.ROUTE_VER_SID,
			WO.CUSTOMER,
			LOT.LOT,		
			LOT.WO,
			LOT.LINE,
			LOT.QUANTITY
	
	FROM	WP_LOT LOT
			INNER JOIN WP_WO WO
				ON  WO.WO = LOT.WO
			INNER JOIN PF_PARTNO PART
				ON PART.PARTNO = LOT.PARTNO
	WHERE	LOT.LOT = 'T056A'
				";
				var example1 = cnn.Database.Connection.Query<RunCardHistoryHTMLView>(_sql)
						.AsQueryable()
					//.Join(c=>)
					//.Where("LOT_SID == @0 ", "GTI20021013194301437")
					//.ToList()
					;

				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}

		[TestMethod]
		public void t_IQueryable()
		{
			//程序內自己啟用 Connection
			var x = TableQueryService.AD_PARAMETER_ByGroupNo("WorkingTimeType").ToList();
			using (var cnn = new MDL.MESContext())
			{
				//使用外部傳入的 Connection
				var x1 = TableQueryService.AD_PARAMETER_ByGroupNo
					("WorkingTimeType"
					, cnn.Database.Connection
					).ToList();
			}
		}

		[TestMethod]
		public void t_IQueryable_2()
		{
			//程序內自己啟用 Connection
			using (var cnn = new MDL.MESContext())
			{
				//使用外部傳入的 Connection
				var _sql = @"
					--工單主要資訊
					SELECT 	LOT.ROUTE_VER_SID,
							WO.CUSTOMER,
							LOT.LOT,		
							LOT.WO,
							ISNULL(LOT.LINE,'LINE') AS LINE,
							LOT.QUANTITY,
							WO.PARTNO,
							WO.ROUTE,
							WO.ERP_COMMENT,
							ISNULL(WO.ATTRIBUTE_03,'ATTRIBUTE_03') AS MAINUSE_PARTNO
					FROM	WP_LOT LOT
							INNER JOIN WP_WO WO
								ON  WO.WO = LOT.WO
							LEFT JOIN PF_PARTNO PART
								ON PART.PARTNO = LOT.PARTNO
					WHERE	LOT.LOT = @LOT;

					--主流程資料
					SELECT	B0.* ,
							B2.PARAMETER AS PRD_CONDITION,
							B2.VALUE  AS PRD_CONDITION_VAL
					FROM	(SELECT 	*
							FROM	PF_ROUTE_VER_OPER A0
							WHERE	EXISTS(
										SELECT 	*
										FROM	WP_LOT LOT
												INNER JOIN WP_WO WO
													ON  WO.WO = LOT.WO
													AND LOT.LOT = @LOT
										WHERE	WO.ROUTE_VER_SID =  A0.ROUTE_VER_SID
										)
							) B0
							LEFT JOIN PF_OPERATION_RECIPE B1
								ON B1.OPER_SID = B0.OPER_SID
							LEFT JOIN FC_RECIPE_VER_PARAMETER B2
								ON B2.RECIPE_VER_SID = B1.RECIPE_VER_SID
								AND B2.RECIPE_SID = B1.RECIPE_SID

					ORDER	BY B0.OPER_SEQ;
					
					--子流程資訊
					SELECT	B0.*,
							B2.PARAMETER AS PRD_CONDITION,
							B2.VALUE  AS PRD_CONDITION_VAL
					FROM	(SELECT	A0.ROUTE AS MAIN_ROUTE ,
									A1.*
							FROM	PF_ROUTE_VER_OPER A0
									LEFT JOIN PF_ROUTE_VER_OPER A1
										ON A1.ROUTE_VER_SID = A0.OPER_SID
							WHERE	A0.OPER_CATEGORY = 'R'	
									AND EXISTS(
										SELECT 	*
										FROM	WP_LOT LOT
												INNER JOIN WP_WO WO
													ON  WO.WO = LOT.WO
													AND LOT.LOT = @LOT
										WHERE	WO.ROUTE_VER_SID =  A0.ROUTE_VER_SID)
							) B0
							LEFT JOIN PF_OPERATION_RECIPE B1
								ON B1.OPER_SID = B0.OPER_SID
							LEFT JOIN FC_RECIPE_VER_PARAMETER B2
								ON B2.RECIPE_VER_SID = B1.RECIPE_VER_SID
								AND B2.RECIPE_SID = B1.RECIPE_SID
					ORDER	BY B0.OPER_SEQ;
				";
				var parameter = new DynamicParameters();
				parameter.Add("@LOT", "SQQL5J1", DbType.String, ParameterDirection.Input);
				var _re = cnn.Database.Connection
					.QueryMultiple(_sql, parameter)
					;
				var A1 = _re.Read();
				var A2 = _re.Read();
				var A3 = _re.Read();


			}
		}




		[TestMethod]
		public void t_Group()
		{
			string SHIFT_SID = "GTI20071015443415664";
			List<Object> _r = null;
			using (var dbContext = new MDL.MESContext())
			{
				_r = (from t in dbContext.ZZ_OPER_WORKT_SUMMARY
					  where
						  (!string.IsNullOrEmpty(SHIFT_SID) && t.SHIFT_SID == SHIFT_SID)
					  //|| (!string.IsNullOrEmpty(LINE_SID) && t.LINE_SID == LINE_SID)
					  //|| (!string.IsNullOrEmpty(OPERATION_SID) && t.OPERATION_SID == OPERATION_SID)
					  group t by new
					  {
						  t.WO,
						  t.REPORT_TYPE,
						  t.OUTPUT_QTY,
						  t.WORK_TOTAL,
						  t.EQUIPMENT_TOTAL
					  }
					  into t1
					  select new
					  {
						  t1.Key.WO,
						  t1.Key.REPORT_TYPE,
						  OUTPUT_SUM = t1.Sum(x => x.OUTPUT_QTY),
						  WORK_SUM = t1.Sum(x => x.WORK_TOTAL),
						  EQU_SUM = t1.Sum(x => x.EQUIPMENT_TOTAL),
					  })
					.ToList<Object>();
			}
			new FileApp().Write_SerializeJson(_r, FileApp.ts_Log(@"DB\t_Group.json"));

		}

		[TestMethod]
		public void t_CreateUnitWork()
		{
			using (var dbContext = new MDL.MESContext())
			{
				var unWork = new EFUnitOfWork(dbContext);
				var _sid = mes.getSID();
				var _AD_LOG = new mdl.AD_LOG()
				{
					FUN_NAME = "Maintain",
					ACTION = "UPDATE",
					TARGET_PK = _sid,
					TARGET_TABLE = "ZZ_EMP_ATTEND",
					VALUE_LINK_SID = _sid,
					CREATE_USER = "TEST",
					CREATE_DATE = mes.getTime(),
				};

				unWork.Repository<AD_LOG>().Create(_AD_LOG);

				/*
				多表作業的語法與上雷同,重點是在於 save 時, UnitWork 會確保 所有的資料都寫入成功才算完成
					,如果有一筆寫入失敗,就會全部取消 ,等同於 transcation 的程序功能
				 */
				unWork.Save();
				//new FileApp().Write_SerializeJson(_r, FileApp.ts_Log(@"DB\t_Group.json"));
			}
		}


		public IResult ZZ_OPER_WORKT_SUMMARY_UpData(mdl.ZZ_OPER_WORKT_SUMMARY entity, bool isTest = false)
		{
			Result result = new Result(true);

			using (var dbContext = new MDL.MESContext())
			{

				var _svc = new EFRepository<mdl.ZZ_OPER_WORKT_SUMMARY>(dbContext);
				try
				{
					//crud
					_svc.Update(entity);
					_svc.SaveChanges();
				}
				catch (Exception ex)
				{

					result.Success = false;
					result.Message = ex.Message;
				}
				return result;

			}
		}


		[TestMethod]
		public void t_Xls_getSheets()
		{
			OleDbConnection objConn = null;
			System.Data.DataTable dt = null;
			// Connection String. Change the excel file to the file you
			// will search.
			string FileName = $"{Directory.GetCurrentDirectory()}\\..\\..\\t_DB_GTiMES_REASON.xlsx";
			String connString =
				$"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = \"{FileName}\";Extended Properties=\"Excel 12.0 Xml;HDR=YES\";";
			objConn = new OleDbConnection(connString);
			// Open connection with the database.
			objConn.Open();
			// Get the data table containg the schema guid.
			dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

			List<string> excelSheets = new List<string>();
			int i = 0;
			foreach (DataRow row in dt.Rows)
			{
				var sheetName = row["TABLE_NAME"].ToString();
				var isSrcSheet = sheetName.Right(1) == "$";
				if (isSrcSheet) excelSheets.Add(sheetName);
			}



		}

		[TestMethod]
		public void t_Xls_getSheets_1()
		{
			OleDbConnection objConn = null;
			System.Data.DataTable dt = null;
			// Connection String. Change the excel file to the file you
			// will search.
			string FileName = $"{Directory.GetCurrentDirectory()}\\..\\..\\t_DB_GTiMES_REASON.xlsx";
			var ds = ReadExcelToDataSet(FileName);
			new FileApp().Write_SerializeJson(ds, FileApp.ts_Log(@"DB\t_Xls_getSheets_1.json"));
		}

		public void x(string SheetName, OleDbConnection conn = null)
		{
			OleDbCommand comm = new OleDbCommand();
			comm.Connection = conn;
			comm.CommandText = $"SELECT * FROM [{SheetName}]";
			OleDbDataReader reader = null;
			DataTable dtData = new DataTable();
			try
			{
				reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
				dtData.Load(reader);

			}
			catch (Exception ex)
			{
				throw new Exception("查無工作表名");
			}
			if (dtData == null || dtData.Columns == null || dtData.Rows == null || dtData.Rows.Count == 0)
				throw new Exception(RES.BLL.Message.UploadFileError);
			//去除末端列空白
			while (1 == 1)
			{
				if (dtData.Rows.Count == 0)
					break;
				if (dtData.Rows[dtData.Rows.Count - 1][1].ToString().Trim() == "")
					dtData.Rows.RemoveAt(dtData.Rows.Count - 1);
				else
					break;
			}
			if (dtData.Rows.Count < 3)
				throw new Exception(RES.BLL.Message.UploadFileError);

			//去除空白列
			DataTable dtTmp = dtData.Clone();
			for (int i = 0; i < dtData.Rows.Count; i++)
			{
				bool NotEmpty = false;
				foreach (DataColumn dc in dtData.Columns)
				{
					if (dtData.Rows[i][dc.ColumnName].ToString().Trim().Length > 0)
						NotEmpty = true;
				}
				if (NotEmpty)
					dtTmp.ImportRow(dtData.Rows[i]);
			}
			//2017.11.14 因為選取指定的Column所以不會有空白的Column，故不須再刪除第一個Column
			//dtTmp.Columns.RemoveAt(0);
			dtTmp.AcceptChanges();
			dtData = dtTmp;

			//DB欄位與GRIDVIEW顯示文字
			Dictionary<string, string> DBColumns = new Dictionary<string, string>();
			for (int i = 0; i < dtData.Columns.Count; i++)
			{
				DBColumns.Add(dtData.Columns[i].ToString(), dtData.Rows[0][i].ToString());
			}

			//移除中文標題
			dtData.Rows.RemoveAt(0);
			//移除範例
			dtData.Rows.RemoveAt(0);

			//設定起始列、末端列
			int RowStart = 4;
			int RowEnd = dtData.Rows.Count;
			//try
			//{
			//	RowStart = Convert.ToInt32(txtRowStart.Text) - 4;
			//	if (RowStart < 0)
			//		RowStart = 0;
			//	RowEnd = Convert.ToInt32(txtRowEnd.Text) - 4;
			//	if (RowEnd > dtData.Rows.Count)
			//		RowEnd = dtData.Rows.Count;
			//}
			//catch
			//{
			//	txtRowStart.Text = "4";
			//	txtRowEnd.Text = (dtData.Rows.Count + 4).ToString();
			//}

			//增加錯誤訊息顯示欄位
			dtData.Columns.Add("MSG");
			//依據啟始終端列抓取要匯入的資料顯示
			dtData = dtData.Select().Skip(RowStart).Take(RowEnd - RowStart + 1).CopyToDataTable();

			//資料列單行檢查，可複寫
			foreach (DataRow row in dtData.Rows)
			{
				try
				{
					//CheckRowData(row);
				}
				catch (Exception ex)
				{
					row["MSG"] = ex.Message;
				}
			}

			//重建GRIDVIEW
			//gvData.Columns.Clear();
			GridView gvData = new GridView();
			foreach (string col in DBColumns.Keys)
			{
				BoundField column = new BoundField();
				column.HeaderText = DBColumns[col];
				column.DataField = col; ;
				column.HeaderStyle.Wrap = false;
				column.ItemStyle.Wrap = false;
				gvData.Columns.Add(column);

			}

			BoundField column2 = new BoundField();
			column2.HeaderText = "錯誤";
			column2.DataField = "MSG"; ;
			column2.HeaderStyle.Wrap = false;
			gvData.Columns.Add(column2);

			gvData.DataSource = dtData;
		}


		public static DataSet ReadExcelToDataSet(string filePath)
		{
			DataSet ds = new DataSet("ds");
			using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
			using (ExcelPackage package = new ExcelPackage())
			{
				package.Load(fs);
				foreach (var sheet in package.Workbook.Worksheets)
				{
					switch (sheet.Name)
					{
						case "注意事項":
						case "勿動（設定資料）":
							continue;
							break;
					}
					if (sheet.Dimension == null) continue;
					var dt = GTI_WorksheetToDataTable(sheet);
					if (dt != null) ds.Tables.Add(dt);
				}
			}
			return ds;

		}

		private static DataTable GTI_WorksheetToDataTable(ExcelWorksheet Sheet, int DataRowStartIdx = 4)
		{

			DataTable dt = new DataTable(Sheet.Name);
			var columnCount = Sheet.Dimension.End.Column + 1;
			var rowCount = Sheet.Dimension.End.Row;
			var colIdx = 2;
			if (rowCount == 0 || rowCount < DataRowStartIdx) return null;
			List<string> colNames = new List<string>();
			for (int col = colIdx; col < columnCount; col++)//設置DataTable列名
			{
				var objCellValue = Sheet.Cells[1, col].Value;
				var cellValue = objCellValue == null ? "" : objCellValue.ToString();
				dt.Columns.Add(cellValue, typeof(string));
				colNames.Add(cellValue);
			}
			for (int i = DataRowStartIdx; i <= rowCount; i++)
			{
				var dr = dt.NewRow();
				var _colIdx = colIdx;
				foreach (var colName in colNames)
				{
					var objCellValue = Sheet.Cells[i, _colIdx].Value;
					var cellValue = objCellValue == null ? "" : objCellValue.ToString();
					dr[colName] = cellValue;
					_colIdx++;
				}
				dt.Rows.Add(dr);
			}
			return dt;
		}

		[TestMethod]
		public void t_ReadExcelToDataSet()
		{
			string FileName = $"{Directory.GetCurrentDirectory()}\\..\\..\\t_DB_GTiMES_REASON.xlsx";

			var dt = Frame.Code.Excel.EpplusHelper.ReadExcelToDataSet(FileName);
		}


		[TestMethod]
		public void t_新Service用法()
		{
			var keyword = "B";
			using (var dbc = new MDL.MESContext())
			{
				BasePfPartnoServices svc_PF_PARTNO = new BasePfPartnoServices();
				svc_PF_PARTNO.DbContext = dbc;
				var _r = new Result(true);
				var _list = (from t in svc_PF_PARTNO.GetAllListIQueryable()
							 where (t.PARTNO == keyword
									|| (t.PARTNO.Contains(keyword)
										|| t.PARTNO_SID.Contains(keyword)
										|| t.PART_NAME.Contains(keyword)
										))
							 select new SelectModel
							 {
								 SID = t.PARTNO_SID,
								 No = t.PARTNO,
								 Display = t.PART_NAME,
								 Value = t.PARTNO,
							 }
							).ToList();


			}
		}

		[TestMethod]
		public void t_取得料號流程工作站資料()
		{
			var keyword = "K1210400281                             ";
			string OPER_SID = "GTI20101310305408436";
			string OPERATION_NO = "";
			using (var dbc = this.DBC)
			{
				//var partInfo = new PartUtility.PartNoVersionInfo(dbc, keyword, PartUtility.IndexType.No);
				//var oper = new OperationUtility.OperationInfo(DBC, OPER_SID, OperationUtility.IndexType.No);
				var fun = new OperationUtility.OperationTypeVerRuleFunction(dbc);
				string OperUITabString = fun.OperUITabString(OPER_SID);
			}
		}


		[TestMethod]
		public void t_取得料號流程工作站_耗用物料()
		{
			var keyword = "K1210400281                             ";
			string PARTNO_SID = "00106D3C-36CD-42B6-AD16-A104A12FA30D";
			string ROUTE_VER_SID = "GTI20101609022009701";
			string ROUTE_VER_OPER_SID = "GTI20101609130109761";
			using (var dbc = this.DBC)
			{
				RouteUtility.PartNoRouteVerOperFunction fun = new RouteUtility.PartNoRouteVerOperFunction(this.DBC);
				DataTable dt = fun.GetRouteVerOperPartNoInfo
					(PARTNO_SID
					, ROUTE_VER_SID
					, ROUTE_VER_OPER_SID);
				new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\test.json"));
			}
		}

		[TestMethod]
		public void t_取得料號流程工作站_x1()
		{
			var keyword = "K1210400281                             ";
			string PARTNO_SID = "00106D3C-36CD-42B6-AD16-A104A12FA30D";
			string ROUTE_VER_SID = "GTI20101609022009701";
			string ROUTE_VER_OPER_SID = "GTI20101609130109761";
			using (var dbc = this.DBC)
			{


				var uf = new PartUtility.PartNoVersionFunction(dbc);
				Command conditionCommand = uf.GetPartNoVersionConditionSQL(keyword, "", VersionSate.Enable.ToString(), "", "", "", "", "", "", "", "", "", "", "");
				var dt = uf.GetPartNoVersionData(conditionCommand);
				if (dt == null || dt.Rows.Count == 0)
				{
					//WriteClientMessage(this.Page, MessageType.Normal, Resources.Message.NoDataFound);
				}
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string sText = string.Format("[{0}]~{1}", Convert.ToString(dt.Rows[i]["PARTNO"] + "]"), Convert.ToString(dt.Rows[i]["PART_NAME"]));
					string sValue = Convert.ToString(dt.Rows[i]["PARTNO_SID"]);
					//ddlTabOperPartSource.Items.Add(new ListItem(sText, sValue));

					//ddlTabOperPartSource.Items.Add(new ListItem(Convert.ToString(dt.Rows[i]["PARTNO"]) + SplitStringWord + Convert.ToString(dt.Rows[i]["PART_NAME"])
					//    , Convert.ToString(dt.Rows[i]["PARTNO_SID"])));
				}
				var _list = (from g in dt.AsEnumerable()
							 select new SelectModel
							 {
								 SID = g.Field<string>("PARTNO_SID"),
								 No = g.Field<string>("PARTNO"),
								 Display = g.Field<string>("PART_NAME"),
								 Value = g.Field<string>("PARTNO_SID"),
							 }
							).ToList();
				new FileApp().Write_SerializeJson(_list, FileApp.ts_Log(@"DB\test.json"));
			}
		}


		[TestMethod]
		public void t_x1()
		{
			using (var dbc = new MDL.MESContext())
			{
				var svc_QC_ITEMGROUP_ITEM = new BaseQcItemgroupItemServices() { DbContext = dbc };
				var svc_QC_ITEMGROUP = new BaseQcItemgroupServices() { DbContext = dbc };
				var svc_QC_ITEM = new BaseQcItemServices() { DbContext = dbc };
				var _list = (from a in svc_QC_ITEMGROUP_ITEM.GetAllListIQueryable()
							 join b in svc_QC_ITEM.GetAllListIQueryable()
								on a.QC_ITEM_SID equals b.QC_ITEM_SID
							 join c in svc_QC_ITEMGROUP.GetAllListIQueryable()
								on a.QC_ITEMGROUP_SID equals c.QC_ITEMGROUP_SID
							 where b.ENABLE_FLAG == nameof(EnableFlag.T)
							 //orderby $"{c.QC_ITEMGROUP_NO}-{c.QC_ITEMGROUP_NAME}"
							 select new SelectModel
							 {
								 SID = c.QC_ITEMGROUP_SID,
								 No = c.QC_ITEMGROUP_NO,
								 Display = c.QC_ITEMGROUP_NAME,
							 }
							)
							.Distinct()
							.AsQueryable()
							.ToList();
			}


		}


		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void t_GetLotQTimeConditionSQL()
		{
			using (var dbc = new MDL.MESContext())
			{
				bool IsQtimeRelease = true;
				string[] strArray = new string[] { "MaxQTime", "MaxProcessTime" };
				string[] arrENABLE_FLAG = new string[] { "T", "N" };
				var NOW_TIME = DateTime.Now;
				var result = (from t in dbc.WP_LOT_CHECKTIME
							  where strArray.Contains(t.CHECK_TYPE)
								&& t.ENABLE_FLAG != "C"
									&& ((IsQtimeRelease && t.END_TIME < NOW_TIME)
									|| (!IsQtimeRelease && t.END_TIME > NOW_TIME))
							  select t).Concat(
					from t in dbc.WP_LOT_CHECKTIME
					where t.CHECK_TYPE == "MinQTime"
							&& ((IsQtimeRelease
									&& t.END_TIME > NOW_TIME
									&& t.ENABLE_FLAG == "T")
								|| (!IsQtimeRelease
									&& t.ENABLE_FLAG != "C"))
					select t
				);
				var _query = (from t in dbc.WP_LOT_CHECKTIME
							  where result.Any(e => e.LOT_TIMECONTROL_SID == t.LOT_TIMECONTROL_SID)
							  select t);
				var z = _query.ToList();
			}


		}

		public class zzzz
		{
			public string SID { get; set; }
			public string ROOT_SID { get; set; }
			public string PARENT_SID { get; set; }
			public int? PREV_SEAIAL { get; set; }
			public int SN { get; set; }
			public string SN_ID { get; set; }

			public string STATUS { get; set; }
			public zzzz()
			{
			}
			public zzzz(WP_LOT_WAFER_MAPPING entity)
			{
				this.SID = entity.WAFER_MAPPING_SID;
				this.ROOT_SID = entity.ROOT_LOT_SID;
				this.PARENT_SID = entity.PARENT_LOT_SID;
				this.PREV_SEAIAL = entity.PREV_SEAIAL;
				this.SN = entity.SERIAL_NUMBER;
				this.SN_ID = entity.SERIAL_NUMBER_ID;
			}
		}

		[TestMethod]
		public void t_Linq_ToDictionary()
		{
			using (var dbc = new MDL.MESContext())
			{
				var svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = dbc };
				// 這樣的寫法會發生錯誤 , 主要 是 linq 內,不允許使用 new class(....) 的模式 ,
				//		只能接收 new class(){ .... } 式的寫法 
				//var _list = (from a in svc_WP_LOT_WAFER_MAPPING.GetAllListIQueryable()
				//			 select new KeyValuePair<int, WP_LOT_WAFER_MAPPING>(a.SERIAL_NUMBER, a)
				//			)
				//			//.ToDictionary<int, zzzz>(a=>a.Key,a=>a.Value)
				//			.ToDictionary(a=>a.Key,a=>new zzzz(a.Value))
				//			//.ToList()
				//			;

				var _list = (from a in svc_WP_LOT_WAFER_MAPPING.GetAllListIQueryable()
							 where a.LOT == "STFGT1" && a.SERIAL_STATUS == "Wait"
							 select new
							 {
								 Key = a.SERIAL_NUMBER,
								 Value = a
							 }
							)
							.ToDictionary(a => a.Key, a => new zzzz(a.Value))
							;

				var obj = FileApp.WriteSerializeJson(_list, _log.t_Linq_ToDictionary);

			}

		}


		[TestMethod]
		public void t_Process_PosiMap_cmd_CHANGE_SERIAL()
		{
			using (var dbc = new MDL.MESContext())
			{

				var obj = FileApp.Read_SerializeJson<Dictionary<int, WAFER_MAPPING>> ( _log.t_Process_PosiMap_cmd_CHANGE_SERIAL);

				TransactionUtility.TransactionBase txnBase = new TransactionUtility.TransactionBase("----", "test", DateTime.Now, "FunctionRightName");
				TransactionUtility.GtimesTxn gtimesTxn = new TransactionUtility.GtimesTxn(this.DBC, txnBase);
				TransactionUtility.AddSQLCommandTxn sqlcmd = new TransactionUtility.AddSQLCommandTxn();

                try
                {
					using (IDbTransaction tx = DBC.GetTransaction())
					{
						var cmd = WIPServices.Process_PosiMap_cmd_UpdateStatus
						(this.DBC
						, "STFGT1"
						, "GTI20123116390095188"
						, obj[5]
						, "TT1"
						, DateTime.Now
						, "-----"
						, WAFER_MAPPING_STATUS.Scrap
						, "ACTION"
						, "ACTION"
						);

						sqlcmd.Commands.Add(cmd);
						gtimesTxn.Add(sqlcmd);
						gtimesTxn.DoTransaction(gtimesTxn.GetTransactionCommands(), tx);
						sqlcmd.Commands.Clear();
						gtimesTxn.Clear();
						tx.Rollback();
					}
				}
				catch (Exception ex)
                {

                    throw ex;
                }

				
            }

		}


		[TestMethod]
		public void t_Process_PosiMap_cmd_CHANGE_SERIAL1()
		{
			using (var dbc = new MDL.MESContext())
			{
				var svc_WP_LOT_WAFER_MAPPING = new BaseWpLotWaferMappingServices() { DbContext = dbc };
				var search = "WO_T074_001";

				var _list = (from a in svc_WP_LOT_WAFER_MAPPING.GetAllListIQueryable()
							 where a.SERIAL_STATUS == "Scrap" &&
									(a.LOT == search
										|| a.SERIAL_NUMBER_ID == search)
								 && !(from b in dbc.WP_LOT_WAFER_MAPPING
									  where b.SERIAL_STATUS == "Normal"
									  select new { b.LOT_SID, b.SERIAL_NUMBER }
								 ).Contains(new { a.LOT_SID, a.SERIAL_NUMBER })
							 select new
							 {
								 LOT = a.LOT,
								 SID = a.WAFER_MAPPING_SID,
								 ROOT_SID = a.ROOT_LOT_SID,
								 PARENT_SID = a.PARENT_LOT_SID,
								 PREV_SEAIAL = a.PREV_SEAIAL,
								 SN = a.SERIAL_NUMBER,
								 SN_ID = a.SERIAL_NUMBER_ID,
							 }
							).ToList();

			}

		}
	}
}

 
