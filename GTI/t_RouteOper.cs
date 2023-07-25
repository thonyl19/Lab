using BLL.MES;
using Genesis.Gtimes.ADM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.RouteUtility;

namespace t_RouteOper
{
	[TestClass]
	public class t_RouteOper : _testBase
	{
		static class _log
		{
			/// <summary>
			/// 取得流程版本中所有工作站與子流程
			/// </summary>
			public static string t_GetRouteVersionAllOperationAndSubRoute
			{
				get
				{
					return FileApp.ts_Log(@"Route\t_GetRouteVersionAllOperationAndSubRoute.json");
				}
			}
			/// <summary>
			/// 取得所有由本站出發經由 Judge 路線可到達的下一站工作站物件集合
			/// </summary>
			public static string t_GetAllNextJudgePathRouteVersionOperationList
			{
				get
				{
					return FileApp.ts_Log(@"Route\t_GetAllNextJudgePathRouteVersionOperationList.json");
				}
			}

			public static string t_GetRouteVerOperPathJudge
			{
				get
				{
					return FileApp.ts_Log(@"Route\t_GetRouteVerOperPathJudge.json");

				}
			}

			/// <summary>
			/// 取得流程版本工作站第一個功能規則物件
			/// </summary>
			public static string t_GetRouteVersionOperationStartRule
			{
				get
				{
					return FileApp.ts_Log(@"Route\t_GetRouteVersionOperationStartRule.json");

				}
			}

			public static string t_ReWork
			{
				get
				{
					return FileApp.ts_Log(@"Route\t_ReWork.json");

				}
			}

			public static string t_RouteVerOperationInfo
			{
				get
				{
					return FileApp.ts_Log(@"Route\t_RouteVerOperationInfo.json");

				}
			}
			
		}

		/// <summary>
		/// 取得所有由本站出發經由 Judge 路線可到達的下一站工作站物件集合
		/// </summary>
		[TestMethod]
		public void t_GetAllNextJudgePathRouteVersionOperationList()
		{
			var ROUTE_VER_OPER_SID = "GTI20072312154043044";
			var routeVerOper = new RouteVersionOperationInfo
				(DBC
				, ROUTE_VER_OPER_SID);
			var _r = routeVerOper.GetAllNextJudgePathRouteVersionOperationList();
			_file.Write_SerializeJson(_r, _log.t_GetAllNextJudgePathRouteVersionOperationList);
		}


		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void t_GetRouteVerOperPathJudge()
		{
			var ROUTE_VER_SID = "GTI20072309533742755";
			var jfun = new RouteUtility.RouteVerOperPathJudgeFunction(DBC);
			var _r = jfun.GetRouteVerOperPathJudge(ROUTE_VER_SID);
			_file.Write_SerializeJson(_r, _log.t_GetRouteVerOperPathJudge);
		}


		/// <summary>
		/// 取得流程版本工作站第一個功能規則物件
		/// </summary>
		[TestMethod]
		public void t_GetRouteVersionOperationStartRule()
		{
			var ROUTE_VER_SID = "GTI19121911041909580";
			var _fn = new RouteVersionOperationInfo(DBC, ROUTE_VER_SID);
			var _r = _fn.GetRouteVersionOperationStartRule();
			_file.Write_SerializeJson(_r, _log.t_GetRouteVersionOperationStartRule);
		}


		[TestMethod]
		public void t_GetRouteVersionAllOperationAndSubRoute()
		{
			var routeversid = "GTI20091610074707007";
			var _fn = new RouteUtility.RouteFunction(this.DBC);
			var _r = _fn.GetRouteVersionAllOperationAndSubRoute(routeversid);
			_file.Write_SerializeJson(_r, _log.t_GetRouteVersionAllOperationAndSubRoute);
		}

		/// <summary>
		/// 需求
		/// 1. 用 ROUTE_VER_SID 取得 所有路徑
		/// 2. 而路徑中,屬於 JUDGE 且 JUDGE_PARAMETER = 'ATTRIBUTE_10' 者
		/// 3. 列出前述項目所對應的 工作站 和 子流程(第一個站)
		/// </summary>
		[TestMethod]
		public void t_()
		{
			var ROUTE_VER_SID = "GTI20091610074707007";
			//條件1,2
			var _sql_12 = @"
			select  B.ROUTE_VER_OPER_PATH_JUDGE_SID  ,
					B.JUDGE_VALUE ,
					B.JUDGE_PARAMETER
			FROM 	PF_ROUTE_VER_OPER_PATH AS A WITH (NOLOCK)
					INNER JOIN PF_ROUTE_VER_OPER_PATH_JUDGE B WITH (NOLOCK)
						ON B.JUDGE_PARAMETER = 'ATTRIBUTE_10'
							AND B.ROUTE_VER_OPER_PATH_SID = A.ROUTE_VER_OPER_PATH_SID 
			where  A.ROUTE_VER_SID = :ROUTE_VER_SID
			";

			//條件1~3
			var _sql_all = @"
			select  B.ROUTE_VER_OPER_PATH_JUDGE_SID  ,
					B.JUDGE_VALUE ,
					B.JUDGE_PARAMETER,
					C.OPERATION,
					C.OPERATION_NO,
					C.OPER_SID,
					--工作站類型不需要帶出流程,原意 要在查詢端直接排除掉,但考慮到要處理的不只這個欄位,所以索性直接給前端做處理
					--CASE WHEN B.JUDGE_VALUE = 'R' THEN C.ROUTE ELSE '' END ROUTE,
					C.ROUTE,
					C.ROUTE_VER_SID,
					C.ROUTE_NO
			FROM 	PF_ROUTE_VER_OPER_PATH AS A WITH (NOLOCK)
					INNER JOIN PF_ROUTE_VER_OPER_PATH_JUDGE B WITH (NOLOCK)
						ON B.JUDGE_PARAMETER = 'ATTRIBUTE_10'
							AND B.ROUTE_VER_OPER_PATH_SID = A.ROUTE_VER_OPER_PATH_SID 
					INNER JOIN PF_ROUTE_VER_OPER C WITH (NOLOCK)
						ON	--子流程的處理條件
							(B.JUDGE_VALUE = 'R' AND C.IS_START = 'T'
								AND C.ROUTE_VER_SID = A.TO_OPER_SID)
							--工作站的對應條件
							OR (B.JUDGE_VALUE = 'O' AND C.ROUTE_VER_OPER_SID = A.TO_ROUTE_VER_OPER_SID )
			WHERE	A.ROUTE_VER_SID = :ROUTE_VER_SID
			";

			//var _fn = new RouteUtility.RouteFunction(this.DBC);
			//var _r = _fn.GetRouteVersionAllOperationAndSubRoute(routeversid);
			//_file.Write_SerializeJson(_r, _log.t_GetRouteVersionAllOperationAndSubRoute);
		}

		[TestMethod]
		public void t_ReWork()
		{
			var ROUTE_VER_SID = "GTI20101318275702518";
			var _r = DDLServices.ReWork(ROUTE_VER_SID);
			_file.Write_SerializeJson(_r, _log.t_ReWork);
		}

		[TestMethod]
		public void t_RouteVerOperationInfo()
		{
			var FROM_ROUTE_VER_OPER_SID = "GTI21050518344132210";
			var _r = new RouteUtility.RouteVerOperationInfo
				(this.DBC
				, FROM_ROUTE_VER_OPER_SID
				, RouteUtility.IndexType.SID);
			_file.Write_SerializeJson(_r, _log.t_RouteVerOperationInfo);

		}


		[TestMethod]
		public void t_StationEQP()
		{
			var NEXT_OPER_SID = "GTI23010917340392788";
			var r = WIPOperConfigServices.StationEQP(NEXT_OPER_SID);
		}
	}



}
