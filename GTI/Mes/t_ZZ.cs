using BLL.MES;
using BLL.MES.DataViews;
using Dapper;
using Genesis.Gtimes.ADM;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnitTestProject.TestUT;
using mdl = MDL.MES;
using System.Resources;
using System.Globalization;
using System.Collections;
using System.Reflection;
using Genesis.Areas.ADM.Controllers;
using BLL.DataViews.Res;
using Frame.Code;
using System.Linq.Dynamic.Core;

namespace UnitTestProject
{
	[TestClass]
	public class t_ZZ : _testBase
	{
		public static class _log
		{
			internal static string t_GetRunCardHistoryByLot
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\t_GetRunCardHistoryByLot.json");
				}
			}

			/// <summary>
			/// 工單過站測試用的資料
			/// </summary>
			internal static string t_ZZ_OPER_WORKT_SUMMARY_工單過站
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\t_ZZ_OPER_WORKT_SUMMARY_工單過站.json");
				}
			}

			/// <summary>
			/// _item 頁面 post 的測試資料
			/// </summary>
			internal static string t_ZZ_OPER_WORKT_SUMMARY
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\t_ZZ_OPER_WORKT_SUMMARY.json");
				}
			}

			internal static string t_ZZ_OPER_WORKT_SUMMARY_UPDATE
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\t_ZZ_OPER_WORKT_SUMMARY_UPDATE.json");
				}
			}

			public static string t_PagerQuery
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\t_PagerQuery.json");
				}
			}
			internal static string t_PagerQuery_RouteOperStage
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\t_PagerQuery_RouteOperStage.json");
				}
			}

		}


		[TestMethod]
		public void t_ZZ_OPER_WORKT_SUMMARY()
		{
			var _data = FileApp
				.Read_SerializeJson<MDL.MES.ZZ_OPER_WORKT_SUMMARY>
					(_log.t_ZZ_OPER_WORKT_SUMMARY_UPDATE);

			var _crt = new ZZServices().ZZ_OPER_WORKT_SUMMARY_Create(_data, true);
		}


		[TestMethod]
		public void t_ZZ_OPER_WORKT_SUMMARY_DEL()
		{
			var _crt = new ZZServices().ZZ_OPER_WORKT_SUMMARY_DEL("12fe6fec-4f73-4a20-9e58-8095eb748dfc", true);
		}

		[TestMethod]
		public void t_ZZ_OPER_WORKT_SUMMARY_UpData()
		{
			var _data = FileApp
				.Read_SerializeJson<MDL.MES.ZZ_OPER_WORKT_SUMMARY>
					(_log.t_ZZ_OPER_WORKT_SUMMARY_UPDATE);

			var _crt = new ZZServices().ZZ_OPER_WORKT_SUMMARY_UpData(_data, true);
		}

		[TestMethod]
		public void t_chk_ZZ_OPER_WORKT_SUMMARY()
		{
			var _data = FileApp
				.Read_SerializeJson<MDL.MES.ZZ_OPER_WORKT_SUMMARY>
					(_log.t_ZZ_OPER_WORKT_SUMMARY_UPDATE);

			var _crt = new ZZServices().ZZ_OPER_WORKT_SUMMARY_UpData(_data, true);
		}


		[TestMethod]
		public void t_confirm_VirtualLot()
		{
			var _data = FileApp
				.Read_SerializeJson<MDL.MES.ZZ_OPER_WORKT_SUMMARY>
					(_log.t_ZZ_OPER_WORKT_SUMMARY_UPDATE);
			var expression = ExtLinq.True<mdl.WP_LOT>()
									.And(t => t.WO_SID == _data.WO_SID);
			var _list = new WIPInfoServices().WP_LOT(expression);
			new FileApp()
				.Write_SerializeJson
					(_list, _log.t_ZZ_OPER_WORKT_SUMMARY_工單過站);
			var _crt = new ZZServices().confirm_VirtualLot(_data);
		}


		//[TestMethod]
		//public void t_STATS_BatchUpdata()
		//{
		//	DateTime WORK_DATE = DateTime.Parse("2020-08-19");
		//	var _list = new ZZServices().STATS_BatchUpdata(WORK_DATE,
		//										  null,
		//										  "I");

		//	var A = 1;
		//	//new FileApp()
		//	//	.Write_SerializeJson
		//	//		(_list, _log.t_ZZ_OPER_WORKT_SUMMARY_工單過站);
		//}

		[TestMethod]
		public void t_流程卡_生產歷程()
		{
			using (var dbc = this.DBC)
			{
				var arg = new { LOT = "SLHJK5" };
				var _sql_head = @"
					SELECT 
							A.WO    
							,A.PARTNO
							,P.PART_NAME
							,A.Lot
							,A.CUSTOMER
		
							,WO.ERP_COMMENT
							,A.ATTRIBUTE_03	    
							,A.ROUTE	    
							,ISNULL(A.QUANTITY,0) AS QUANTITY
							,A.UNIT
						
							,A.ROUTE_VER_SID
					FROM 	WP_LOT A
 							INNER JOIN WP_WO WO ON A.WO_SID=WO.WO_SID
							INNER JOIN  PF_PARTNO P ON (A.PARTNO = P.PARTNO)
					WHERE	A.LOT = @LOT
				";
				var main = dbc.GetConnection()
					.Query<RunCard_Standard>(_sql_head, arg)
					.FirstOrDefault();


				var _sql_body = $@"
					SELECT 	
							--A1.*,
							--A1.oper_category,A1.OPER_SEQ
							--,A0.WO
							A0.OPER_SID,
							A0.OPERATION,
		
							--,
							A0.ROUTE_VER_OPER_SID
							,A0.USER_LIST --負責人
							,A0.OPER_START_TIME --投入日期 ,投入時間
							,A0.TX_TIME -- 產出日期 產出時間
							,ISNULL(A0.GOOD_QTY,0) AS GOOD_QTY --良品數量
							,ISNULL(A0.SCRAP_QTY,0) AS SCRAP_QTY --報廢數量
							,A0.EQP_NO --機台
							,A0.EQP_NAME --機台
							
					FROM 	WR_OPERATION A0
							INNER JOIN PF_ROUTE_VER_OPER A1 
								ON A1.ROUTE_VER_OPER_SID = A0.ROUTE_VER_OPER_SID
					WHERE	A0.LOT = @LOT
					ORDER	BY A0.OPER_START_TIME
				";
				main.Details = dbc.GetConnection()
					.Query<RunCard_Standard_Details>(_sql_body, arg)
					.ToList();

				var _sql_BIN = $@"
					SELECT 	ROUTE_VER_OPER_SID
							,BIN
							,BIN_QTY
							,BIN + ': ' + CAST(BIN_QTY as nvarchar(255)) AS ACTION
					FROM 	ZZ_LOT_BIN
					WHERE 	LOT = @LOT
				";
				var BIN = dbc.GetConnection()
					.Query<ZZ_LOT_BIN>(_sql_BIN, arg)
					.ToList();

				var _sql_edc = $@"
					SELECT 	A1.ROUTE_VER_OPER_SID
							,A1.EDC_NO 
							,A0.EDC_DATA AS PARAMETER
							,A1.EDC_NO + ': ' + A0.EDC_DATA　AS　ACTION
					FROM  	WP_LOT_EDC_ROW A0
							INNER JOIN  WP_LOT_EDC A1 
								ON A1.LOT_EDC_SID = A0.LOT_EDC_SID
					WHERE 	A0.LOT = @LOT
					ORDER	BY A0.EDC_SEQ
				";

				var EDC = dbc.GetConnection()
					.Query<WP_LOT_EDC>(_sql_edc, arg)
					.ToList();

				var funRecipe = new RecipeUtility.RecipeVersionFunction3(dbc);
				var _dc = new Dictionary<string, List<KeyValueExt>>();
				var _sql_ProduceCondition = $@"
					SELECT 	A0.PARAMETER AS Name
							,A0.VALUE AS　Value
					FROM  	FC_RECIPE_VER_PARAMETER A0
					WHERE 	A0.RECIPE_VER_SID IN @RECIPE_VER_SID
					
				";
				Func<string, string, string, string, string, List<KeyValueExt>> _fn_RecipeData = (WO, RouteVerSid, RouteVerOperSid, PARTNO, OperSid) =>
				{
					var _key = $@"{WO}_{RouteVerSid}_{RouteVerOperSid}_{PARTNO}_{OperSid}";
					var _result = new List<KeyValueExt>();
					if (_dc.TryGetValue(_key, out _result) == false)
					{
						var dvOperRecipe = funRecipe.GetPartNoOperRecipeData_OperSid
							(WO
							, RouteVerSid
							, RouteVerOperSid
							, PARTNO
							, OperSid);
						if (dvOperRecipe?.Count > 0)
						{
							var RECIPE_VER_SID = dvOperRecipe.ToTable()
								.Rows.OfType<DataRow>()
								.Select(r => r["RECIPE_VER_SID"].ToString())
								.ToArray<string>();
							_result = dbc.GetConnection()
								.Query<KeyValueExt>(_sql_ProduceCondition, new { RECIPE_VER_SID })
								.ToList();
						}
						_dc.Add(_key, _result);
					}
					return _result;
				};

				main.Details.ForEach(el =>
				{
					el.EDC = EDC.FindAll(i => i.ROUTE_VER_OPER_SID == el.ROUTE_VER_OPER_SID).Select(i => new KeyValueExt() { Name = i.EDC_NO, Value = i.PARAMETER }).ToList();
					el.BIN = BIN.FindAll(i => i.ROUTE_VER_OPER_SID == el.ROUTE_VER_OPER_SID).Select(i => new KeyValueExt() { Name = i.BIN, Value = i.BIN_QTY?.ToString() }).ToList();
					el.ProduceCondition = _fn_RecipeData
						(main.WO
						, main.ROUTE_VER_SID
						, el.ROUTE_VER_OPER_SID
						, main.PARTNO
						, el.OPER_SID
						);
				});


				//string docPath = Environment.GetFolderPath(Environment.CurrentDirectory.MyDocuments);
				File.WriteAllText(FileApp.ts_Log(@"ZZ\test_sum.json"), main.ToJson());

				var _r = new
				{
					main,
					BIN,
					EDC
				};
				FileApp.WriteSerializeJson(_r, FileApp.ts_Log(@"ZZ\test_sum.json"));
			}

		}


		[TestMethod]
		public void t_chk_ListCount()
		{
			List<string> t = new List<string>() { "A", "B", "C" };

			//var _r = chk_Lot.chk_ListCount(t, -2, "併批的批號數量")();

			List<QtyItem> x = new List<QtyItem>() {
				new QtyItem(){ Reason = "A"},
				new QtyItem(){ Reason = "B"},
				new QtyItem(){ Reason = "A"},
			};

			var linqStament = (from p in x
							   group p by new { p.Reason } into g
							   select new
							   {
								   Name = g.Key.Reason,
								   Counts = g.Count()
							   })
						.Any(c=> c.Counts>1);


		}

		[TestMethod]
		public void t_chk_ListCount1()
		{
			List<string> t = new List<string>() { "A", "B", "C" };

			//var _r = chk_Lot.chk_ListCount(t, -2, "併批的批號數量")();
			foreach (var x in t) {
				switch (x) {
					case "B":
						return;
						break;
				}
			}


		}

 

		[TestMethod]
		public void TestLanguageResource()
		{
			var enUsCulture = new CultureInfo("en-US");
			var resManager = new ResourceManager(typeof(RES.BLL.Face));
			var enUsResourceSet = resManager.GetResourceSet(enUsCulture, true, true);

			// Create a new resource writer for the en-US resource file
			using (var resourceWriter = new ResourceWriter("Face.en-US.resx"))
			{
				// Copy the existing resources to the new writer
				foreach (DictionaryEntry entry in enUsResourceSet)
				{
					resourceWriter.AddResource(entry.Key.ToString(), entry.Value.ToString());
				}

				// Add new resources
				resourceWriter.AddResource("hello_world", "Hello World");

				// Save the changes to the resource file
				resourceWriter.Generate();
			}
		}


		[TestMethod]
		public void TestLanguageResource1()
		{
			var resxPath = @"M:\Prd_Dev\Library\RES\BLL\resources.resx";
			//var resxSet = new ResXResourceSet(resxPath);

			// 新增資料
			//resxSet.Add("hello_world", "Hello World");

			// 寫入 .resx 檔案
			using (var writer = new ResXResourceWriter(resxPath))
			{
				writer.AddResource("test", "test");
				writer.Generate();
			}
		}

		[TestMethod]
		public void _test_cae1() {
			var zz = DDLServices.ddl_Carrier();
		}


		[TestMethod]
		public void _zz() {
			string className = "MyNamespace.MyClass"; // 目標類別的完整名稱
			string methodName = "MyMethod"; // 目標方法名稱

			Type type = Type.GetType(className);

			if (type != null)
			{
				MethodInfo method = type.GetMethod(methodName);

				if (method != null)
				{
					object instance = Activator.CreateInstance(type);
					method.Invoke(instance, null);
				}
				else
				{
					Console.WriteLine("方法不存在");
				}
			}
			else
			{
				Console.WriteLine("類別不存在");
			}
		}


		[TestMethod]
		public void t_()
		=> _DBTest(Txn => {
			var r = Txn.GetParameterInfo("PACK_LINE");
		},false,true);


		[TestMethod]
		public void t_PagerQuery()
		{ 
			var obj = FileApp.Read_SerializeJson<PagerQuery>(_log.t_PagerQuery);
			var _r = QMSService.Search_QCResult_Query(obj);
		}

		struct d_Search_QCResult_Query_ILot
		{
			public string qcNo;
			public string operation;
			public string Lot;
		}

		[TestMethod]
		public void t_1()
		=> _DBTest(Txn => {
			//string qcNo = null;// "0516";
			//string operation = null;
			//string Lot 
			//	//= "TWO-240515B-01"
			//	;
			//var arg = new d_Search_QCResult_Query_ILot();
			////arg.Lot = "JTest0717_7-01";
			//var q1 = Txn.EFQuery_MES.WP_IPQC_LOT
			//	.Join(Txn.EFQuery_MES.WP_LOT,
			//			ipqcLot => ipqcLot.LOT_SID,
			//			lot => lot.LOT_SID,
			//			(ipqcLot, lot) => new { ipqcLot, lot })
			//	.Where(joined=>joined.lot.LOT.StartsWith(arg.Lot));
			//var q1_ = q1.ToList();
			
			//var q2 = Txn.EFQuery_MES.WP_IPQC_LOT
			//	.Join(Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL,
			//			ipqcLot => ipqcLot.LOT_SID,
			//			lot => lot.LOT_SID,
			//			(ipqcLot, lot) => new { ipqcLot, lot })
			//	.Where(joined => joined.lot.LOT.StartsWith(arg.Lot));
			//var q2_ = q2.ToList();

			//var query = Txn.EFQuery_MES.WP_IPQC
			//	.Where(x => (arg.qcNo == null || x.QC_NO.Contains(arg.qcNo)) &&
			//		(arg.operation == null || x.OPERATION == arg.operation ) &&
			//		(arg.Lot == null 
			//			|| q1.Any(joined => joined.ipqcLot.QC_NO == x.QC_NO)
			//			|| q2.Any(joined => joined.ipqcLot.QC_NO == x.QC_NO)
			//		))
			//	.Select(x => x);
			//var r = query.ToList();
		}, false, true);



        [TestMethod]
        public void t_fn()
		=> _DBTest((txn) =>
		{
			var q1 = (from p in txn.EFQuery_MES.PF_PARTNO_CATEGORY
						 where p.CATEGORY_FLAG_1 == "T" || p.CATEGORY_FLAG_2 == "T"
						 select p
			).ToList();

			var q2 = q1.GroupBy(p => getGroupKey(p))
				.ToDictionary(p=>p.Key,p=> p.ToList());


			
			
		},false, true);

		Func<PF_PARTNO_CATEGORY, string> getGroupKey = p =>
		{
			// 根據您的邏輯返回分組鍵
			if (p.CATEGORY_FLAG_1 == "T")
			{
				return "Group1";
			}
			else if (p.CATEGORY_FLAG_2 == "T")
			{
				return "Group2";
			}
			else
			{
				return "Other";
			}
		};


		public class d_Query_RouteOperStage
		{
			public string ROUTE_NO { get; set; }
			public string ROUTE { get; set; }
			public string VERSION { get; set; }
			public string OPER_NO { get; set; }
			public string OPERATION { get; set; }
			public string STAGE_NO { get; set; }
			public string STAGE_NAME { get; set; }

			public decimal? d_VERSION {
				get {
					decimal decimalNumber;
					if (decimal.TryParse(this.VERSION, out decimalNumber)) {
						return decimalNumber;
					}
					return null;
				}
			}

			public bool isQueryStage {
				get{
					return !string.IsNullOrWhiteSpace(this.STAGE_NO) 
						|| !string.IsNullOrWhiteSpace(this.STAGE_NAME);
				}
			}

			public bool isQueryRoute{
				get
				{
					return !string.IsNullOrWhiteSpace(this.ROUTE)
						|| !string.IsNullOrWhiteSpace(this.ROUTE_NO)
						|| this.d_VERSION != null;
				}
			}
			public bool isQueryOper
			{
				get
				{
					return !string.IsNullOrWhiteSpace(this.OPERATION)
						|| !string.IsNullOrWhiteSpace(this.OPER_NO);
				}
			}
			public void SetQueryAll()
			{
				this.STAGE_NO = null;
				this.STAGE_NAME = null;
				this.OPERATION = null;
				this.OPER_NO = null;
			}



			public d_Query_RouteOperStage(PagerQuery PQuery)
			{
				if (PQuery.Conditions.Rules != null)
				{
					foreach (var x in PQuery.Conditions.Rules)
					{
						if (x.Value.Length != 0)
						{
							if (x.Value.Length != 0)
							{
								// 使用反射來動態設定屬性值
								var property = this.GetType().GetProperty(x.Field);
								if (property != null)
								{
									property.SetValue(this, x.Value[0]);
								}
							}
						}
					}
				}
			}
		}

		public class d_PF_ROUTE_VER : PF_ROUTE_VER{ 
			public List<d_PF_ROUTE_VER_OPER> child { get; set; }
		}

		public class d_PF_ROUTE_VER_OPER : PF_ROUTE_VER_OPER
		{
			public string STAGE_SID { get; set; }
		}

		[TestMethod]
		public void t_RouteOperStage()
		=> _DBTest((txn) =>
		{
		var PQuery = FileApp.Read_SerializeJson<PagerQuery>(_log.t_PagerQuery_RouteOperStage);
		var arg = new d_Query_RouteOperStage(PQuery);
		var stage = txn.EFQuery_MES.PF_STAGE;
		var routeOperStages = txn.EFQuery_MES.PF_ROUTE_VER_OPER_STAGE;
		var routeVerOper = txn.EFQuery_MES.PF_ROUTE_VER_OPER;
		var route = txn.EFQuery_MES.PF_ROUTE;

			arg.STAGE_NO = "**M0711";
			//arg.ROUTE_NO = "102B4014_02_CM_23";
			arg.OPERATION = null;
			//var t = _queryBase.ToList();
			var zz = 1;


			var q_stage = (from a in stage
					   where (arg.isQueryStage == false
						   || (arg.isQueryStage
								&& (arg.STAGE_NO == null || a.STAGE_NO.Contains(arg.STAGE_NO))
								&& (arg.STAGE_NAME == null || a.STAGE_NAME.Contains(arg.STAGE_NAME))
						   ))
					   select a);

		var q_stage1 = (from a in routeOperStages
						join b in q_stage
							on a.STAGE_SID equals b.STAGE_SID
						//into grp_b
						//from b in grp_b.DefaultIfEmpty()
						select new { a, b });
		//var t1 =q_stage1.ToList();


								


		var q_VerOper = (from a in routeVerOper
						 where arg.isQueryOper == false
								|| (arg.isQueryOper
									&& (arg.OPER_NO == null || a.OPERATION_NO.Contains(arg.OPER_NO))
									&& (arg.OPERATION == null || a.OPERATION.Contains(arg.OPERATION))
									)
						 select a);
		var _q1 = (from a in q_VerOper
				   join b in q_stage1
					   on a.ROUTE_VER_OPER_SID equals b.a.ROUTE_VER_OPER_SID
					//into grp_b
				 //  from b in grp_b.DefaultIfEmpty()
				 //  where b !=null
				   select new d_PF_ROUTE_VER_OPER
				   {
					   ROUTE_VER_OPER_SID = a.ROUTE_VER_OPER_SID,
					   ROUTE_VER_SID = a.ROUTE_VER_SID,
					   ROUTE_SID = a.ROUTE_SID,
					   ROUTE_NO = a.ROUTE_NO,
					   ROUTE = a.ROUTE,
					   VERSION = a.VERSION,
					   //OPER_CATEGORY = a.OPER_CATEGORY,
					   OPER_SEQ = a.OPER_SEQ,
					   OPER_SID = a.OPER_SID,
					   OPERATION_NO = a.OPERATION_NO,
					   OPERATION = a.OPERATION,
					   //IS_START = a.IS_START,
					   //IS_END = a.IS_END,
					   //CREATE_USER = a.CREATE_USER,
					   //CREATE_DATE = a.CREATE_DATE,
					   //UPDATE_USER = a.UPDATE_USER,
					   //UPDATE_DATE = a.UPDATE_DATE,
					   STAGE_SID = b.a.STAGE_SID
				   });
			var t2 = _q1.ToList();

		var q_route = (from a in txn.EFQuery_MES.PF_ROUTE_VER
					   where arg.isQueryRoute == false
							|| (arg.isQueryRoute
								&& (arg.ROUTE == null || a.ROUTE.Contains(arg.ROUTE))
								&& (arg.ROUTE_NO == null || a.ROUTE.Contains(arg.ROUTE_NO))
								&& (arg.d_VERSION == null || a.VERSION == arg.d_VERSION)
								)
					   select a);




		var _queryBase = (from a in q_route
						  where (arg.isQueryOper == false && arg.isQueryStage == false)
							  || _q1.Any(b => b.ROUTE_VER_SID == a.ROUTE_VER_SID)
						  select new d_PF_ROUTE_VER
						  {
							  ROUTE_VER_SID = a.ROUTE_VER_SID,
							  ROUTE_SID = a.ROUTE_SID,
							  ROUTE_NO = a.ROUTE_NO,
							  ROUTE = a.ROUTE,
							  //ROUTE_CATEGORY = a.ROUTE_CATEGORY,
							  VERSION = a.VERSION,
							  VERSION_STATE = a.VERSION_STATE,
							  DEFAULT_FLAG = a.DEFAULT_FLAG,
							  //DESCRIPTION = a.DESCRIPTION,
							  //CREATE_USER = a.CREATE_USER,
							  //CREATE_DATE = a.CREATE_DATE,
							  //UPDATE_USER = a.UPDATE_USER,
							  //UPDATE_DATE = a.UPDATE_DATE,
							  //QUOTE_ONCE = a.QUOTE_ONCE,
							  START_OPER_SID = a.START_OPER_SID,
							  START_OPERATION_NO = a.START_OPERATION_NO,
							  START_OPERATION = a.START_OPERATION,
							  END_OPER_SID = a.END_OPER_SID,
							  END_OPERATION_NO = a.END_OPERATION_NO,
							  END_OPERATION = a.END_OPERATION,
						  }
						);
		

		/*
		 select );

		 */
		if (PQuery?.Sort?.Code != "")
		{
			_queryBase = _queryBase.OrderBy(c => PQuery.Sort.Code);
		}
		if (PQuery.Page == null)
		{
			txn.result.Data = new { Queryable = _queryBase.ToList() };
		}
		else {
			var PageInfo = _queryBase.PageResult(PQuery.Page.Index, PQuery.Page.Size);
			var Queryable = PageInfo.Queryable.ToList();
			//arg.SetQueryAll();
			foreach (var item in Queryable) {
				var x = q_stage1.Where(c => c.a.ROUTE_VER_SID == item.ROUTE_VER_SID).ToList();
				var x1 = q_stage.ToList();
				item.child = _q1.Where(c => c.ROUTE_VER_SID == item.ROUTE_VER_SID).ToList();
			}

			txn.result.Data = new { Queryable, PageInfo = PQuery.parsePagedResult(PageInfo) };
		}
		}, false, true);
	}


}


//namespace MyNamespace {
//	public class MyClass
//	{

//		public void MyMethod() { 
//		}
//	}
//}
