using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code.Web.Select;
using Genesis.Areas.MES.Controllers;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction.EQP;
using Genesis.Gtimes.Transaction.TOL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.RouteUtility;

namespace UnitTestProject
{


	[TestClass]
	public class t_Operation : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_splitBIN
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_splitBIN.json");
				}
			}

			internal static string t_工作站的機台治具設定
			{
				get
				{
					return FileApp.ts_Log(@"Operation\t_工作站的機台治具設定.json");
				}
			}


			
		}


		[TestMethod]
		public void t_chk_Lot()
		{
			string LotNo = "201-20121129-34";
			var _svc = new OffLineLotServices();
			var result = chk_Lot.Condition_ChangeOperation(LotNo);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_chk_Lot.json"));

		}

		[TestMethod]
		public void t_ChangeOperation_LotInfo()
		{
			string LotNo = "Sub-20200220-20";
			var _ctr = new OperationController();
			//var result = _ctr.ChangeOperation_LotInfo(LotNo);
			//var _json = ((ContentResult)result).Content;
			//new FileApp().Write(_json, FileApp.ts_Log(@"Operation\t_ChangeOperation_LotInfo.json"));

		}

		/// <summary>
		/// 取得 子流程 的工作站清單
		/// </summary>
		[TestMethod]
		public void t_t_SubRouteOperList()
		{
			string ROUTE_VER_SID = "GTI20022014223001921";
			string OPER_SID = "GTI20022014210401910";
			var _ctr = new OperationController();
			//var result = _ctr.SubRouteOperList(ROUTE_VER_SID, OPER_SID);
			//var _json = ((ContentResult)result).Content;
			//new FileApp().Write(_json, FileApp.ts_Log(@"Operation\t_SubRouteOperList.json"));

		}


		/// <summary>
		/// 以 ROUTE_VER_OPER_SID  取得 工作站點資訊
		/// </summary>
		[TestMethod]
		public void t_RouteVersionOperationInfo()
		{
			string ROUTE_VER_OPER_SID = "GTI20022015012701968";
			var result = new RouteUtility.RouteVersionOperationInfo
				(this.DBC
				, ROUTE_VER_OPER_SID);

			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_RouteVersionOperationInfo.json"));

		}

		/// <summary>
		/// 以 ROUTE_VER_OPER_SID  取得 工作站點資訊
		/// </summary>
		[TestMethod]
		public void t_ChkChangeOperationInfo()
		{
			string ROUTE_VER_SID = "GTI20022014223001921";
			string ROUTE_VER_OPER_SID = "GTI20022015012701968";
			string ROUTE_VER_OPER_SID_new = "GTI20022015012701969";
			string ROUTE_VER_OPER_SID_sub = "GTI20022014215601918";
			var RouteVer = new RouteUtility.RouteVersionInfo(this.DBC, ROUTE_VER_SID);
			var MainRouteOperList = RouteVer.GetRouteVersionOperationList();

			var oldOper = MainRouteOperList.FirstOrDefault(el => { return el.ROUTE_VER_OPER_SID == ROUTE_VER_OPER_SID; });
			if (oldOper == null)
			{
				//throw 站點不存在 
			}

			var NewOper = MainRouteOperList.FirstOrDefault(el => { return el.ROUTE_VER_OPER_SID == ROUTE_VER_OPER_SID_new; });
			if (oldOper.SID == NewOper.SID)
			{
				//throw new Exception(Resources.Message.CanNotToThisOperation);
			}



			var isSubRoute = NewOper.OPER_CATEGORY != "O";
			if (isSubRoute)
			{
				var SubRouteOperList = RouteVer.GetRouteVersionOperationListOnlySubRoutes(NewOper.OPER_SID);
				var subOper = SubRouteOperList.FirstOrDefault(el => { return el.ROUTE_VER_OPER_SID == ROUTE_VER_OPER_SID_sub; });
				if (subOper == null)
				{
					//throw 子站點不存在 
				}
				if (oldOper.SID == subOper.SID)
				{
					//throw new Exception(Resources.Message.CanNotToThisOperation);
				}
			}

			var result = new RouteUtility.RouteVersionOperationInfo
					(this.DBC
					, ROUTE_VER_OPER_SID);

			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_RouteVersionOperationInfo.json"));

		}



		[TestMethod]
		public void t_DoChangeOperation()
		{
			//{"ROUTE_VER_SID":"GTI20022014223001921","ROUTE_VER_OPER_SID":"GTI20022014215601918","OPER_SID_new":"GTI20022014210401910","OPER_SID_sub":"GTI19041123494300933","List":[{"LOT":"Sub-20200220-20","ROUTE_VER_OPER_SID":"GTI20022014215601918"}],"CurrentLot":null,"Description":null,"Reason":["other"]}
			var data = new ChangeOperationFrom()
			{
				Reason = new string[] { "test" },
				ROUTE_VER_SID = "GTI20022014223001921",
				ROUTE_VER_OPER_SID = "GTI20022015012701968",
				OPER_SID_new = "GTI20022014210401910",
				OPER_SID_sub = "GTI19041123494300933",
				List = new List<ChangeOperationLot>() {
					new ChangeOperationLot(){ LOT = "Sub-20200220-16",ROUTE_VER_OPER_SID = "GTI20022015012701968" },
					new ChangeOperationLot(){ LOT = "Sub-20200220-17",ROUTE_VER_OPER_SID = "GTI20022015012701968" }
				}
			};
			var _svc = new OffLineLotServices();
			var result = _svc.DoChangeOperation(data);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_DoChangeOperation.json"));

		}

		[TestMethod]
		public void t_DoChangeOperation_1()
		{
			var data = FileApp.Read_SerializeJson<ChangeOperationFrom>(FileApp.ts_Log(@"Operation\x.json"));

			var _svc = new OffLineLotServices();
			//var result = _svc.DoChangeOperation(data, false, true);
			var result = _svc.DoChangeOperation(data, false, true);
			new FileApp(true).Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_DoChangeOperation.json"));

		}

		[TestMethod]
		public void t_RouteVersionInfo()
		{
			var ROUTE_VER_SID = "GTI12090317461000029";
			var RouteVerinfo = new RouteUtility.RouteVersionInfo(this.DBC, ROUTE_VER_SID);
			string _null = null;
			var result = new
			{
				SID = RouteVerinfo.SID,
				No = RouteVerinfo.ROUTE_NO,
				Value = RouteVerinfo.VERSION,
				Display = RouteVerinfo.ROUTE,
				StatusSid = _null,
				Status = RouteVerinfo.VERSION_STATE,
			};

			new FileApp(false).Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_RouteVersionInfo.json"));
		}

		[TestMethod]
		public void t_ChangeVerStatus()
		{
			var ROUTE_VER_SID = "GTI19041017063400250";
			var result = new RouteVerServices().ChangeVerStatus(ROUTE_VER_SID, true);
			new FileApp(false).Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_ChangeVerStatus.json"));
		}


		/// <summary>
		/// 取得 流程中 , Judge 的參數設定
		/// </summary>
		[TestMethod]
		public void t_GetRouteVerOperPathJudge()
		{
			string ROUTE_VER_SID = "GTI20070210443098502";
			RouteUtility.RouteVerOperPathJudgeFunction jfun = new RouteUtility.RouteVerOperPathJudgeFunction(DBC);
			var result = jfun.GetRouteVerOperPathJudge(ROUTE_VER_SID);
			new FileApp(false).Write_SerializeJson(result, FileApp.ts_Log(@"Operation\t_GetRouteVerOperPathJudge.json"));

		}

		/// <summary>
		/// 取得 流程中 , Judge 的參數設定
		/// </summary>
		[TestMethod]
		public void t_取得所有由本站出發經由Judge路線可到達的下一站工作站物件集合()
		=> _DBTest((txn) =>
        {
			var lot = txn.GetLotInfo("TestFurance2025021801-01", isQueryByLotNO: true);
			
			var routeVerOper = new RouteVersionOperationInfo
				(txn.DBC
				, lot.ROUTE_VER_OPER_SID);

			var zz = DDLServices.ReWork(routeVerOper.ROUTE_VER_SID, routeVerOper.ROUTE_VER_OPER_SID);

			var t = routeVerOper
				.GetAllNextJudgePathRouteVersionOperationList();
			var _list_Judge = zz
				.Select(c=> new SelectModel{
						SID = c.OPER_SID,
						No = c.OPERATION_NO,
						Display = c.OPERATION,
						Value = c.OPER_SID,
						Attr01 = c.ROUTE_VER_OPER_SID,
						INum = 0,
						Status = c.Status
				}).ToList();

			var is有Judge = _list_Judge.Count != 0;
			if (is有Judge) {
				var _r1 = routeVerOper.GetNextDefaultRouteVersionOperationInfo();
				_list_Judge.Add(new SelectModel { 
					SID = _r1.OPER_SID,
					No = _r1.OPERATION_NO,
					Display = _r1.OPERATION,
					Value = _r1.OPER_SID,
					Attr01 = _r1.ROUTE_VER_OPER_SID,
					INum = 1,
				});
			}

			new FileApp(false).Write_SerializeJson(_list_Judge, FileApp.ts_Log(@"Operation\t_NextJudgePath_1.json"));

		}, true);



		[TestMethod]
		public void t_ZZ_ReWorkOperList()
		{
			using (var dbc = this.DBC)
			{
				//var _sql = $@"
				//            SELECT	A.TO_OPER_SID AS SID,
				//		C.OPERATION_NO AS No,
				//		C.OPERATION AS Display,
				//		B.JUDGE_VALUE AS Status,
				//		0 AS INum
				//            FROM    PF_ROUTE_VER_OPER_PATH AS A WITH (NOLOCK)
				//		INNER JOIN PF_ROUTE_VER_OPER_PATH_JUDGE B WITH (NOLOCK)
				//			ON B.JUDGE_PARAMETER = 'ATTRIBUTE_10'
				//				AND B.ROUTE_VER_OPER_PATH_SID = A.ROUTE_VER_OPER_PATH_SID 
				//		LEFT JOIN PF_OPERATION C WITH (NOLOCK)
				//			ON C.OPER_SID =  A.TO_OPER_SID
				//WHERE	A.ROUTE_VER_SID = :ROUTE_VER_SID
				//                     ";

				//List<IDbDataParameter> parameters = new List<IDbDataParameter>();
				//dbc.AddCommandParameter(parameters, "ROUTE_VER_SID", "GTI20070210443098502");
				//_sql = dbc.GetCommandText(_sql, SQLStringType.SqlServerSQLString);

				//var dt = dbc.Select(_sql, parameters);

				//var dt = new WIPServices().ZZ_ReWorkOperList("GTI20072309533742755", this.DBC);

				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"Operation\t_ZZ_ReWorkOperList.json"));

			}

		}
 

		[TestMethod]
		public void _20260624_取得所有的站點()
		=> _DBTest((txn) =>
		{
			var _lotInfo = TestUT.GTI_helper.getLotInfo(txn);
			var routeVer = _lotInfo.GetRouteVersionInfo();
			var routeList = routeVer.GetRouteVersionOperationInfoList();
			var currentOperIndex = routeList.FindIndex(x => x.OPER_SID == _lotInfo.OPER_SID);
			var OperRouteList = new OperRouteInfo() { OperRouteList = routeList, CurrentOperKey = (currentOperIndex + 1).ToString() };
			var SubRouteList = OperRouteList.OperRouteList.Where(x => x.OPER_CATEGORY == "R").ToList();
		}, true);


		[TestMethod]
		public void _取最後一站()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo();
			var oper = lot.GetLotNextDefaultRouteVersionNextOperationInfo();
			
			var r = oper.IS_END;
		}, true);

		[TestMethod]
		public void _取上一站()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo();
			var oper = lot.GetRouteVersionOperationInfo();
			var oper_pre = oper.GetPrevDefaultRouteVersionOperationInfo();
			var r = oper.IS_END;
		}, true);


		[TestMethod]
		public void _取得工站設定的機台治具()
		=> _DBTest(Txn => {

			//var oper = Txn.GetOperationInfo("C01-0020", OperationUtility.IndexType.No);
			var _lotInfo = Txn.GetLotInfo("TWO-240725A-04", isQueryByLotNO: true);
			var Fn_Eqp = new EquipmentUtility.EquipmentFunction(Txn.DBC);
			var dt = Fn_Eqp.GetPartNoOperEquipmentData_OperSid(_lotInfo.WO, _lotInfo.ROUTE_VER_SID, _lotInfo.ROUTE_VER_OPER_SID
				, _lotInfo.PARTNO, _lotInfo.OPER_SID);

			var Fn_Tool = new ToolUtility.ToolFunction(Txn.DBC);
			var dt1 = Fn_Tool.GetPartNoOperToolData_OperSid(_lotInfo.WO, _lotInfo.ROUTE_VER_SID, _lotInfo.ROUTE_VER_OPER_SID
				, _lotInfo.PARTNO, _lotInfo.OPER_SID);


			//另一種取法 
			var operinfo = Txn.GetOperationInfo(_lotInfo.OPER_SID);
			var operfun = new OperationUtility.OperationFunction(Txn.DBC);
			var operRule_Eqp = operfun.GetOperEquipAllEquipmentList(operinfo);



		}, true);

		[TestMethod]
		public void _取得工站所有流程()
		=> _DBTest(Txn => {

			//var oper = Txn.GetOperationInfo("C01-0020", OperationUtility.IndexType.No);
			var _lotInfo = Txn.GetLotInfo("TWO-240725A-04", isQueryByLotNO: true);
			var Fn_Eqp = new EquipmentUtility.EquipmentFunction(Txn.DBC);
			var dt = Fn_Eqp.GetPartNoOperEquipmentData_OperSid(_lotInfo.WO, _lotInfo.ROUTE_VER_SID, _lotInfo.ROUTE_VER_OPER_SID
				, _lotInfo.PARTNO, _lotInfo.OPER_SID);

			var Fn_Tool = new ToolUtility.ToolFunction(Txn.DBC);
			var dt1 = Fn_Tool.GetPartNoOperToolData_OperSid(_lotInfo.WO, _lotInfo.ROUTE_VER_SID, _lotInfo.ROUTE_VER_OPER_SID
				, _lotInfo.PARTNO, _lotInfo.OPER_SID);


			//另一種取法 
			var operinfo = Txn.GetOperationInfo(_lotInfo.OPER_SID);
			var operfun = new OperationUtility.OperationFunction(Txn.DBC);
			var operRule_Eqp = operfun.GetOperEquipAllEquipmentList(operinfo);
		}, true);


	}
}
