using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using BoxUT = Genesis.Gtimes.ADM.BoxInfoUtility;

namespace UnitTestProject
{
	[TestClass]
	public class t_BoxInfo
	{
		public string _path = @"C:\Code\GTIMES_2015\UnitTestProject\Log\BoxInfo\";
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
		public void t_x()
		{
			string LotNo = "201-20121129-34";
			string _file = $"{_path}GetPartNoOperEquipmentData_OperSid.json";
			var LotInfo = new BoxUT.BoxInfo(this.DBC, LotNo, BoxUT.IndexType.No);
			var _ut = new BoxUT.BoxInfoFunction(this.DBC);

			var x = _ut.IsBoxCodeExist("1234");
			//FileApp.Write_SerializeJson<LotUtility.LotInfo>(LotInfo, _file );

			//LotInfo = FileApp.Read_SerializeJson<LotUtility.LotInfo>(_file);


		}

		[TestMethod]
		public void t_Ins_x()
		{
			//string LotNo = "201-20121129-34";
			//string _file = $"{_path}GetPartNoOperEquipmentData_OperSid.json";
			//var _Obj = new BoxUT.BoxInfo(this.DBC, LotNo, BoxUT.IndexType.No);
			//var _ut = new BoxUT.BoxInfoFunction(this.DBC);

			//var _SID = this.DBC.GetSID();


			//string user = "test";
			//DateTime updateTime = this.DBC.GetDBTime();
			////string Description = txtDescription.Text.Trim();
			//// Insert Data

			//InsertCommandBuilder insBuild = new InsertCommandBuilder(this.DBC, "ZZ_BOXINFO");

			//List<Column> insertColumns = new List<Column>();
			////
			//Column EQP_NO = new Column("EQP_NO", "", txtNO.Text.Trim());
			//insertColumns.Add(EQP_NO);

			//List<IDbCommand> commands = new List<IDbCommand>();
			//EquipmentUtility.EquipmentTransaction tran = new EquipmentUtility.EquipmentTransaction();
			//commands.AddRange(tran.AddTransaction(this.DBC, FunctionRightName + "Data.aspx", _SID, sEQP_NO, insertColumns, user, updateTime));

			//this.DBC.DoTransaction(commands);



		}

		[TestMethod]
		public void t_Ins()
		{
			string LotNo = "201-20121129-34";
			string _file = $"{_path}GetPartNoOperEquipmentData_OperSid.json";
			var _Obj = new BoxUT.BoxInfo(this.DBC, LotNo, BoxUT.IndexType.No);
			var _ut = new BoxUT.BoxInfoFunction(this.DBC);

			var _SID = this.DBC.GetSID();
			var _sql = $@"
                INSERT INTO ZZ_BOXINFO
                    ([BOX_SID]     
                    ,[BOX_CODE]    
                    ,[BOX_ALIAS]   
                    ,[BOX_COLS]    
                    ,[BOX_ROWS]    
                    ,[DESCRIPTION] 
                    ,[COL_TYPE]    
                    ,[ROW_TYPE]    
                    ,[ENABLE_FLAG] )
                SELECT '{_SID}' AS　BOX_SID
                    ,'Spacer' AS BOX_CODE    
                    ,'Spacer' AS BOX_ALIAS   
                    ,10 AS BOX_COLS    
                    ,10 AS BOX_ROWS    
                    ,'' AS DESCRIPTION 
                    ,0 AS COL_TYPE    
                    ,1 AS ROW_TYPE    
                    ,'F' AS ENABLE_FLAG
                    
                    
            ";
			this.DBC.Execute(_sql);




		}

	}
}
