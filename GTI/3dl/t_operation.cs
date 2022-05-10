using cBLL.MES;
using Genesis.Gtimes.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using UnitTestProject.TestUT;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using mdl = MDL.MES.Views.user;
using System.Net;

namespace UnitTestProject
{
	[TestClass]
	public class t_operation
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
		}

		DBController _dbc;

		DBController DBC
		{
			get
			{
				if (_dbc == null)
				{
					ConnectionStringSettings SmartQueryConn = ConfigurationManager.ConnectionStrings["sql.mes"];
					this._dbc = new DBController(SmartQueryConn);
				}
				return this._dbc;
			}
		}

		[TestMethod]
		public void t_()
		{
			var pager = new MDL.Base.State.Pager();
			var r = operation.load(null, null);
		}

		[TestMethod]
		public void t_1()
		{
			var _type = "3-MD-W027-03A";
			var flag = $"{_type}-{DateTime.Today.ToString("yyyyMMdd")}";
			var no = $"{flag}-{(cDAL.MES.inspect.getMaxSerial(_type, flag) + 1).ToString("00")}";

			var r = operation.load(null, null);
		}


		[TestMethod]
		public void t_2()
		{
			//var status = new cDAL.MES.user.status();
			using (var db = new MDL.MESContext())
			{
				var defState = cDAL.MES.user.status.get(no: "Idle");

				var query =
					from u in db.AD_USER
					join s in db.FC_USER_STATE on u.STATE_SID equals s.STATE_SID into j1
					from s in j1.DefaultIfEmpty()
					select new MDL.MES.Views.user
					{
						sid = u.USER_SID,
						account = u.ACCOUNT_NO,
						no = u.EMP_NO,
						name = u.USER_NAME,
						email = u.EMAIL,
						language = u.CULTURE_LANGUAGE,
						isEnabled = u.ENABLE_FLAG == "T" ? true : false,
						effectiveDate = u.ONBOARD_DATE,
						expirationDate = u.LEAVE_DATE,
						countOPER = db.WP_USER.Where(m => m.USER_SID == u.USER_SID & !string.IsNullOrEmpty(m.OPER_SID)).Select(m => m.OPER_NO).Distinct().Count(),
						countWO = db.WP_USER.Where(m => m.USER_SID == u.USER_SID & !string.IsNullOrEmpty(m.OPER_SID)).Select(m => m.WO).Distinct().Count(),
						lastTime = db.WP_USER.Where(m => m.USER_SID == u.USER_SID).Max(m => m.UPDATE_DATE),
						state = s == null ? new mdl.userState() { sid = defState.sid, no = defState.no, name = defState.name } : new MDL.MES.Views.user.userState() { sid = s.STATE_SID, no = s.STATE_NO, name = s.STATE_NAME },
						authType = u.AD_AUTH_FLAG == "T" ? "ldap" : "gtimes"
					};
				var _list = new List<string>() { "3dl" };
				var x = query
						.Where(e=>_list.Contains(e.account))
						.ToList();

			}
		}



		[TestMethod]
		public void t_call_ashx()
		{
			var url = "http://localhost:50550/ashx/test.ashx?ENCODE_FORMAT_NO=3_MD_W027_03A";

			WebRequest request = WebRequest.Create(url);
			System.IO.Stream stream = request.GetResponse().GetResponseStream();
			System.IO.StreamReader reader = new System.IO.StreamReader(stream);
			string contents = reader.ReadToEnd();

			WebClient client = new WebClient();
			var z = client.DownloadString(url);
		}


		[TestMethod]
		public void t_getNewSerialNo()
		{
			//var code = "";
			//string mesDBkey = WebConfigurationManager.AppSettings["csMES"];
			//var defaultMesConnection = ConfigurationManager.ConnectionStrings[mesDBkey];

			//using (var db = new DBController(defaultMesConnection))
				//var x = cBLL.MES.mes.getSID();
			var _type = "3-MD-W027-03A";
			var no = cBLL.MES.inspect.getNewSerialNo(_type,"AdmTest");

			//var r = operation.load(null, null);
		}


		
	}
}
