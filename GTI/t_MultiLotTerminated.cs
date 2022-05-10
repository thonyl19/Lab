using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.Transaction;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.WIP.LotUtility;

namespace UnitTestProject
{
	[TestClass]
	public class t_MultiLotTerminated
	{
		public string _path = @"C:\Code\GTIMES_2015\UnitTestProject\Log\";
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
		DBController _dbc;

		DBController DBC
		{
			get
			{
				if (_dbc == null)
				{
					ConnectionStringSettings SmartQueryConn = ConfigurationManager.ConnectionStrings["SRDSR.SqlServer.GTIMES"];
					this._dbc = new DBController(SmartQueryConn);
				}
				return this._dbc;
			}
		}

		[TestMethod]
		[Obsolete]
		public void t_取得AppConfig中指定的Connection()
		{
			string linkSID = this.DBC.GetSID();

		}

		[TestMethod]
		public void t_LotInfo序列化()
		{
			string LotNo = "201-20121129-34";
			string _file = $"{_path}test.json";
			string _xml = $"{_path}test.xml";
			var LotInfo = new LotUtility.LotInfo(this.DBC, LotNo, LotUtility.IndexType.NO);

			//FileApp.Write_SerializeJson<LotUtility.LotInfo>(LotInfo, _file );

			//LotInfo = FileApp.Read_SerializeJson<LotUtility.LotInfo>(_file);


		}

		[TestMethod]
		public void t_GetPartNoOperEquipmentData_OperSid()
		{
			string LotNo = "201-20121129-34";
			string _file = $"{_path}GetPartNoOperEquipmentData_OperSid.json";
			var LotInfo = new LotUtility.LotInfo(this.DBC, LotNo, LotUtility.IndexType.NO);
			EquipmentUtility.EquipmentFunction uf = new EquipmentUtility.EquipmentFunction(this.DBC);

			DataView dt = uf.GetPartNoOperEquipmentData_OperSid
				(LotInfo.WO
				, LotInfo.ROUTE_VER_SID
				, LotInfo.ROUTE_VER_OPER_SID
				, LotInfo.PARTNO
				, LotInfo.OPER_SID);
			//FileApp.Write_SerializeJson<LotUtility.LotInfo>(LotInfo, _file );

			//LotInfo = FileApp.Read_SerializeJson<LotUtility.LotInfo>(_file);
		}

		[TestMethod]
		public void t_()
		{

			string LotNo = "201-20121129-34";
			LotUtility.LotInfo lotinfo = new LotUtility.LotInfo(this.DBC, LotNo, LotUtility.IndexType.NO);

			dynamic boo = new ExpandoObject();
			//直接寫boo.Name加上新的Property
			boo.Name = "Jeffrey";

			FileApp.Write_SerializeJson(boo, FileApp.ts_Log(@"MultiLotTerminated\lotinfo.json"));

			return;
			string linkSID = this.DBC.GetSID();
			DateTime txnTime = this.DBC.GetDBTime();

			TransactionUtility.TransactionBase txnBase = new TransactionUtility.TransactionBase
				(linkSID, "TETS", txnTime, "MultiLotTerminated");
			TransactionUtility.GtimesTxn gtimesTxn = new TransactionUtility.GtimesTxn(this.DBC, txnBase);

			var oTerminate = new WIPTransaction.TerminateLotTxn(lotinfo);
			gtimesTxn.Add(oTerminate);
			//var oEnd = new WIPTransaction.EndOfLotTxn(LotInfo);
			//gtimesTxn.Add(oEnd);
			List<IDbCommand> TxnComds = gtimesTxn.GetTransactionCommands();
			this.g(TxnComds);
			//gtimesTxn.DoTransaction(TxnComds, tx);
			//gtimesTxn.Clear();
		}


		dynamic g(List<IDbCommand> TxnComds)
		{
			foreach (IDbCommand Comd in TxnComds)
			{
				dynamic boo = new ExpandoObject();
				foreach (var x in Comd.Parameters)
				{
					var z = x;
				}
			}
			return "";
		}


	}
}
