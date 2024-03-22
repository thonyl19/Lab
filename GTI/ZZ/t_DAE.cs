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
using Genesis.Library.BLL.ZZ.DAE;
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
using static Genesis.Library.BLL.WRP.CrewServices;
using _Prd = Genesis.Library.BLL.MES.OperTask;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;

namespace UnitTestProject
{
	/// <summary>
    /// 台英帝國
    /// </summary>
    [TestClass]
	public class t_DAE : _testBase
	{
		static class _log
		{
			internal static string ZZ_MAKER_ENG_DATA
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\ENG\ZZ_MAKER_ENG_DATA.json");
				}
			}

			internal static string Productivity_ListData
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\DAE\Productivity_ListData.json");
				}
			}

			internal static string Productivity_ListData_1
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\DAE\Productivity_ListData_1.json");
				}
			}

			internal static string Productivity_ListData_SA
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\DAE\Productivity_ListData_SA.json");
				}
			}

			internal static string Productivity_ListData_case
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\DAE\Productivity_ListData_cc.json");
				}
			}

			internal static string 按Bom表檢查物料清單
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\DAE\按Bom表檢查物料清單.json");
				}
			}
 
		}


		[TestMethod]
		public void t_SuportRec_List()
		{
			//ApiService.SuportRec_In("D5101-230831001-02", "Admin",true);
			//ApiService.SuportRec_List("5D0AS27500-240103-01",true);
		}

		[TestMethod]
		public void t_SuportRec_In()
		{
			//ApiService.SuportRec_In("D5101-230831001-02", "Admin",true);
			ApiService.SuportRec_In("5D0AS27500-240103-01", "Admin", true);
		}


		[TestMethod]
		public void t_SuportRec_Exit()
		{
			//ApiService.SuportRec_Exit("5D0AS27500-240103-01", "USER_003", true);
		}

		[TestMethod]
		public void t_編碼測試()
		=> _DBTest(Txn => {
			var MESDBC = Txn.DBC;
			var EnInfo = new EncodeFormatUtility.EncodeFormatInfo
						  (MESDBC, "DAE_MTR_LOT", EncodeFormatUtility.IndexType.No);
			Dictionary<EncodeFormatUtility.ParameterType, object> args = new Dictionary<EncodeFormatUtility.ParameterType, object>();
			args.Add(EncodeFormatUtility.ParameterType.INPUT_DICT_STR_STR,
                    new Dictionary<string, string>(){ {"KEY1", "12345"} }
            );
			var Code = EncodeFormatUtility.Coder.GetCodes(MESDBC, "EIS", EnInfo, 1, args,false);
			//return Code.Codes[0].ToString();
		},false, true);


		[TestMethod]
		public void t_RemoveSuportRec()
		=> _DBTest(Txn => {
			var dbc = Txn.DBC;
			var lot = Txn.GetLotInfo("5D0AS27400-231004-02", isQueryByLotNO:true);
			//var cmd = Genesis.Library.BLL.DTC.Lot.RemoveSuportRec(dbc, lot);

			//var T = TxnBase.CheckClass("MDL.MES", "WP_USER_TRACE_IN");
			var StaticMethod = "RemoveSuportRec";
			MethodInfo methodInfo = typeof(Genesis.Library.BLL.DTC.Lot).GetMethod
				(StaticMethod, BindingFlags.Public | BindingFlags.Static);
			if (methodInfo != null) {
				object[] methodParameters = new object[] { dbc, lot }; // 传递给静态方法的参数
				List<IDbCommand> cmd =  (List< IDbCommand>)methodInfo.Invoke(null, methodParameters);
				if (cmd!=null) Txn.DoTransaction(cmd);
			}
 


		},true, true);

		[TestMethod]
		public void t_取得工單指定治具的單模數量()
		=> _DBTest(Txn => {
			var dbc = Txn.DBC;
			var lot = Txn.GetLotInfo("5D0AS27400-231004-01", isQueryByLotNO: true);
			var oper = Txn.GetOperationInfo(lot.OPER_SID);
			ApiService._取得工單指定治具的單模數量(Txn, lot, oper);

		}, true, true);



		[TestMethod]
		public void t_x()
		=> _DBTest(Txn => {
			var _repo = new
			{
				PF_ROUTE_VER_OPER = Txn.EFQuery<PF_ROUTE_VER_OPER>(),
				ZZ_STD_HOUR = Txn.EFQuery<ZZ_STD_HOUR>(),
			};
			var ROUTE_VER_SID = "GTI23091108350055568";
			var OperList = (from P0 in _repo.PF_ROUTE_VER_OPER.Reads(c => c.ROUTE_VER_SID == ROUTE_VER_SID)
							join Z1 in _repo.ZZ_STD_HOUR.Reads() on P0.OPERATION_NO equals Z1.OPERATION_NO
							orderby P0.OPER_SEQ
							select new d_OperList()
							{
								OPER_SEQ = P0.OPER_SEQ,
								OPERATION_NO = P0.OPERATION_NO,
								OPERATION = P0.OPERATION,
								STD_HOUR = Z1.STD_HOUR
							}
				 ).ToList();

		}, true, true);

		[TestMethod]
		public void t_x1()
		=> _DBTest(Txn => {
			var z = FileApp.Read_SerializeJson<PagerQuery>(_log.Productivity_ListData);
			var z1 = CrewServices.Productivity_ListData_取得基本資料(z, Txn);
			FileApp.WriteSerializeJson(z1,_log.Productivity_ListData_case);
		},false, true);


		[TestMethod]
		public void t_x2()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo("D5101-230831001-04" ,isQueryByLotNO:true);

			var _repo = new
			{
				WP_LOT_HIST = Txn.EFQuery<WP_LOT_HIST>(),
				WP_TOOL_TRACE = Txn.EFQuery<WP_TOOL_TRACE>(),
			};
			var z = _repo.WP_LOT_HIST.Reads(c =>
				c.LOT == lot.LOT
				&& c.NEW_ROUTE_VER_OPER_SID == lot.ROUTE_VER_OPER_SID
				&& c.ACTION_LINK_TABLE == "WP_LOT_TOOL_TRACE"
			).OrderByDescending(c=>c.CREATE_DATE)
			.FirstOrDefault();

			if (z != null) { 
				var tools = _repo.WP_TOOL_TRACE.Reads(c => c.ACTION_LINK_SID == z.ACTION_LINK_SID).ToList();
				foreach (var tool in tools) { 
					var toolinfo = new ToolUtility.ToolInfo(Txn.DBC, tool.TOOL_SID, ToolUtility.ToolInfo.IndexType.SID);
					var ReCallUseCount = tool.OLD_USE_COUNT - tool.NEW_USE_COUNT;
					Txn.DoTransaction(new TOLTransaction.ToolAddUseCountTxn(toolinfo, (decimal) ReCallUseCount));
				}
			}


		}, true, true);


		struct d_按Bom表檢查物料清單 {
			public List<PartData.OperationMTRLotList> Mlot;
			public List<WP_BOM> list_bom;
			public List<ZZ_WO_MT_LOT_LIST> list_WO_MLOT;
		}

		[TestMethod]
		public void t_按Bom表檢查物料清單()
		=> _DBTest(Txn => {
			var bomFormNo = "5101-150519001";
			//var _repo = new
			//{
			//	WP_BOM = Txn.EFQuery<WP_BOM>(),
			//	ZZ_WO_MT_LOT_LIST = Txn.EFQuery<ZZ_WO_MT_LOT_LIST>(),
			//};
			//var list_bom = _repo.WP_BOM
			//	.Reads(x => x.CATEGORY.Equals(BomCategory.Leaf) && x.BOM_FORM_NO.Equals(bomFormNo))
			//	.ToList();

			//var list_WO_MLOT = _repo.ZZ_WO_MT_LOT_LIST
			//	.Reads(c => c.WO == bomFormNo)
			//	.ToList();

			//FileApp.WriteSerializeJson(new { list_bom, list_WO_MLOT }, _log.按Bom表檢查物料清單);

			var x  = FileApp.Read_SerializeJson<d_按Bom表檢查物料清單>(_log.按Bom表檢查物料清單);
			_Prd.Check.按Bom表檢查物料清單(x.Mlot, x.list_bom, x.list_WO_MLOT);
		}, false, true);


		[TestMethod]
		public void t_ZZ_WO_MT_LOT_LIST()
		=> _DBTest(Txn => {
			var z1 = ApiService.ZZ_WO_MT_LOT_LIST("5101-150519001", "15127-1-1C01");
		},false, true);

		[TestMethod]
		public void t_ZZ_MT_LOT_USED()
		=> _DBTest(Txn => {
			var z1 = ApiService.ZZ_MT_LOT_USED("5D2NW50000-231128-01");
			FileApp._tmpJson(z1);
		}, false, true);
		

		[TestMethod]
		public void t_Productivity_ListData()
		{
			var _sql = @"
	                    SELECT [LOT]
						FROM	[dbo].[WP_LOT_HIST]
						WHERE	1=1 
								AND ACTION  = 'END_OPERATION'
								AND OLD_ROUTE_VER_SID = @ROUTE_VER_SID
								--[STARTDATE]-- AND OPER_END_TIME >= @STARTDATE
								--[ENDDATE]-- AND OPER_END_TIME <= @ENDDATE
								--[RANGE]-- AND OPER_END_TIME BETWEEN @RANGE_S AND @RANGE_E
						GROUP BY LOT
                         
					";
			//
			var z = FileApp.Read_SerializeJson<PagerQuery>(_log.Productivity_ListData);
			//var z1 = CrewServices.parseArg_Productivity_ListData(z, _sql);
			var z2 = CrewServices.Productivity_ListData(z,true);
			//var z3 = CrewServices.Productivity_ExportExcel(z);
			FileApp._tmpJson(z2);
		}

		public struct d_x {
			public List<d_Productivity_ListData> query;
			public List<d_OperList> operList;
		}
		[TestMethod]
		public void t_Productivity_ListData1()
		{
			var z
				//= FileApp.Read_SerializeJson<d_x>(_log.Productivity_ListData_SA);
				= FileApp.Read_SerializeJson<CrewServices.d_基本資料>(_log.Productivity_ListData_case);
			var z1 = CrewServices.Productivity_ListData_分類計算(z.query,z.OperList,false, isDebug: true);
            FileApp._tmpJson(z1);
        }


		[TestMethod]
		public void t_取得USER_NO並轉成列名()
		{
			var z = FileApp.Read_SerializeJson<List<CrewServices.d_Productivity_ListData>>(_log.Productivity_ListData_SA);
			var allCols = z.Select(o => o.USER_NO).Distinct().OrderBy(o => o).ToList();
			var res = z.GroupBy(o => o.OPER_NO)
				.Select(o =>
				{
					dynamic d = new ExpandoObject();
					d.LOT = o.Key;
					var dict = d as IDictionary<string, object>;
					allCols.ForEach(c =>
					{
						dict["S" + c] = o.Where(p => p.USER_NO == c).Sum(p => p.TOTAL_TIME);
					});
					return d;
				}).ToList();
			FileApp._tmpJson(res);
		}


		[TestMethod]
		public void t_11()
		=> _DBTest(Txn => {
			//MDL.MESContext()
			var _repo = new {
				AD_USERGROUP_OPERATION_LIST = Txn.EFQuery<AD_USERGROUP_OPERATION_LIST>(),
				AD_USERGROUP = Txn.EFQuery<AD_USERGROUP>(),
				AD_USERGROUP_USER_LIST = Txn.EFQuery<AD_USERGROUP_USER_LIST>(),
				AD_USER = Txn.EFQuery<AD_USER>(),
				WP_USER_TRACE_IN = Txn.EFQuery<WP_USER_TRACE_IN>(),
				PF_ROUTE_VER_OPER = Txn.EFQuery<PF_ROUTE_VER_OPER>(),
				ZZ_STD_HOUR = Txn.EFQuery<ZZ_STD_HOUR>(),
			};
			//var ROUTE_VER_SID = "GTI23080211431452319";
			//var OperList = (from P0 in _repo.PF_ROUTE_VER_OPER.Reads(c => c.ROUTE_VER_SID == ROUTE_VER_SID)
			//				join Z1 in _repo.ZZ_STD_HOUR.Reads() on P0.OPERATION_NO equals Z1.OPERATION_NO
			//				orderby P0.OPER_SEQ
			//				select new d_OperList()
			//				{
			//					OPER_SEQ = P0.OPER_SEQ,
			//					OPERATION_NO = P0.OPERATION_NO,
			//					OPERATION = P0.OPERATION,
			//					STD_HOUR = Z1.STD_HOUR
			//				}
			//).ToList();
			//FileApp._tmpJson(OperList);

			var query = (from U in (
				from UO in _repo.AD_USERGROUP_OPERATION_LIST.Reads()
				join UG in _repo.AD_USERGROUP.Reads() on UO.GROUP_SID equals UG.GROUP_SID
				join UU in _repo.AD_USERGROUP_USER_LIST.Reads() on UG.GROUP_SID equals UU.GROUP_SID
				join user in _repo.AD_USER.Reads() on UU.USER_SID equals user.USER_SID
				select user
				).Distinct()
				join trace in _repo.WP_USER_TRACE_IN.Reads().Where(w => w.END_TIME == null)
				on U.ACCOUNT_NO equals trace.USER_NO into joinedTrace
				from W1 in joinedTrace.DefaultIfEmpty()
				select new 
				{
					USER_NO =  U.ACCOUNT_NO,
					USER_NAME = U.USER_NAME,
					IN_SUPPORT_SID = W1.IN_SUPPORT_SID
					, IS_SUPPORT = W1.IS_SUPPORT
					//,W1.ACTION_LINK_SID
					,ROUTE_VER_OPER_SID = W1.ROUTE_VER_OPER_SID
					,OPER_SID = W1.OPER_SID
					,OPER_NO = W1.OPER_NO
					,WO = W1.WO
					,PARTNO = W1.PARTNO
					,LOT = W1.LOT
					,START_TIME = W1.START_TIME != null ? W1.START_TIME : DateTime.MinValue
					,END_TIME = W1.END_TIME

				}).ToList();

			DataTable dt = (DataTable)JsonConvert.DeserializeObject(((object)query).ToJson(true), (typeof(DataTable)));

			dt = WebHelper.GTI_TranslateDataTableHeader(dt,
				new Dictionary<string, string>(){});

			//var x = EpplusHelper.ExportDataTable2ByteArr(dt);

		}, false, true);



		[TestMethod]
		public void t_Tool累計使用次數_DAE()
		{
			
			var r = _Func.Tool累計使用次數_DAE_公式(25, 5);
			//var r = _Func.Tool累計使用次數_DAE_公式(25,10);
		}


		[TestMethod]
		public void t_CheckInBatchiLot()
		{
			//var r = WIPServices.CheckBatchLot("D5101-230831001-05", "D5101-230831001-04",true) ;
			//var r = WIPServices.CheckBatchLot("D5101-230831001-05", "D5101-230831001-04", true);
		}
	}
}

