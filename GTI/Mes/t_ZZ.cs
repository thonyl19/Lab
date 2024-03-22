using BLL.MES;
using BLL.MES.DataViews;
using Dapper;
using Frame.Code;
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

namespace UnitTestProject
{
	[TestClass]
	public class t_ZZ : _testBase
	{
		static class _log
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
				File.WriteAllText(FileApp.ts_Log(@"ZZ\test_sum.json"), main.ToJson(true));

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
	}


}


//namespace MyNamespace {
//	public class MyClass
//	{

//		public void MyMethod() { 
//		}
//	}
//}
