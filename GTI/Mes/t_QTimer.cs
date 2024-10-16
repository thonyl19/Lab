using BLL.MES;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.MES.AutoGenerate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using UnitTestProject.TestUT;
using mdl = MDL.MES;
using vDbCtx = MDL.MESContext;

namespace UnitTestProject
{
	[TestClass]
	public class t_QTimer : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_GetWpChecktimeData
			{
				get
				{
					return FileApp.ts_Log(@"QTimer\t_GetWpChecktimeData.json");
				}
			}
		}



		[TestMethod]
		public void t_GetWpChecktimeData()
		{
			
			using (var cnn = new MDL.MESContext())
			{
				var example1 = cnn.WP_LOT_CHECKTIME
					.Where("LOT_SID == @0 ", "GTI20021013194301437")
					.ToList();

				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}
		[TestMethod]
		public void t_() {
			//var x = new LotUtility.LotInfo(mes.dbc(), "WO_T059-004", LotUtility.IndexType.NO);
			//var x1 = x.GetRouteVersionOperationInfo();
			var _r = TableQueryService.WP_LOT_CHECKTIME("WO_T059-004", "PSI-R002").ToList();


		}


		[TestMethod]
		public void t_GetCheckQTimeOperParaData()
		{
			using (var dbc = mes.dbc())
			{
				var _lotInfo = new LotUtility.LotInfo(dbc, "5A0AS27400-240515-01", LotUtility.IndexType.NO);

				var chkFun = new CheckTimeUtility.CheckTimeFunctions(dbc);
				var dvOperSet = chkFun.GetCheckQTimeOperParaData
					(_lotInfo.WO
					, _lotInfo.ROUTE_VER_SID
					, _lotInfo.ROUTE_VER_OPER_SID
					, _lotInfo.PARTNO
					, _lotInfo.OPERATION);
				//new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}

        [TestMethod]
        public void t_fn()
		=> _DBTest((txn) =>
		{
			//var _d = txn.DapperQuery<FC_CARRIER>("SELECT * from FC_CARRIER WHERE STATE_NO = 'Idle'")
			//	.FirstOrDefault();
			var LotNo = "WO_T059-008";
			var _lotInfo = new LotUtility.LotInfo(txn.DBC, LotNo, LotUtility.IndexType.NO);
			//WIPServices.LotCheckWithQTime(LotNo,"", txn.DBC, _lotInfo, true);
			var FunctionRightName = "";

			DateTime NowTime = txn.ExeTime;
			var currentUser = txn.UserNo;
			var userNO = txn.UserNo;
			CheckTimeUtility.CheckTimeFunctions chkFun = new CheckTimeUtility.CheckTimeFunctions(txn.DBC);
			//var routeInfo = _lotInfo.GetRouteVersionInfo();
			var No
				//= _lotInfo.GetRouteVersionOperationInfo().OPERATION_NO;
				= "PSI-R002";
			DataView dvCheckTime = chkFun.GetWpChecktimeData(_lotInfo.LOT, No, "T");

			if (dvCheckTime != null)
			{
				dvCheckTime.RowFilter = "CHECK_TYPE='MaxQTime'";

				if (dvCheckTime.Count > 0)
				{
					for (int i = 0; i < dvCheckTime.Count; i++)
					{
						if (NowTime > Convert.ToDateTime(dvCheckTime[i]["END_TIME"].ToString()))
						{
							throw new Exception(string.Format(RES.BLL.Message.LotIsOverTheTimeToCheckIn,
								LotNo, Convert.ToDateTime(dvCheckTime[i]["END_TIME"]).ToString("yyyy/MM/dd HH:mm:ss")));
						}
					}
				}
			}


			
			List<IDbCommand> commands = new List<IDbCommand>();

			if (dvCheckTime != null)
			{
				dvCheckTime.RowFilter = "CHECK_TYPE='MinQTime'";

				if (dvCheckTime.Count > 0)
				{
					for (int i = 0; i < dvCheckTime.Count; i++)
					{
						if (NowTime > Convert.ToDateTime(dvCheckTime[i]["END_TIME"].ToString()))
						{
							List<Column> modifyColumns = new List<Column>();
							Column ENABLE_FLAG = new Column("ENABLE_FLAG", "T", "F");
							modifyColumns.Add(ENABLE_FLAG);
							//
							CheckTimeUtility.CheckTimeTransaction tran = new CheckTimeUtility.CheckTimeTransaction();
							var _cmds = tran.ModifyTransaction
								(txn.DBC
								, FunctionRightName
								, dvCheckTime[i]["LOT_TIMECONTROL_SID"].ToString()
								, _lotInfo.LOT
								, modifyColumns
								, userNO, NowTime);
							commands.AddRange(_cmds);
						}
						else
						{
							throw new Exception(string.Format(RES.BLL.Message.LotNotTheTimeToCheckIn,
								LotNo, Convert.ToDateTime(dvCheckTime[i]["END_TIME"]).ToString("yyyy/MM/dd HH:mm:ss")));
						}
					}

					//if (!isTest) dbc.DoTransaction(commands);
				}
			}
			

		}, true);


	}


}
