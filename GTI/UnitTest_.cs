﻿using BLL.DataViews.Res;
using BLL.MES;
using Frame.Code;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static Genesis.Areas.Label.Controllers.PrintController;
using static Genesis.Library.BLL.Label.LabelBaseService;
using static Genesis.Library.BLL.Label.PrintServices;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_
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
		public void t_1()
		{
			var s = "A,B,C";
			var data = s.Split(',').Select(x =>
			{
				return new Dictionary<string, string>() { { x, "" } };
			}).ToList();

			var z = new Dictionary<string, string>() { { "A", "" } };

			var _data = new List<Dictionary<string, string>>
			{
				new Dictionary<string, string>() { { "Cat", "" } },
				new Dictionary<string, string>() { { "Owl", "" } },
				new Dictionary<string, string>() { { "Rat", "" } },
				new Dictionary<string, string>() { { "Bat", "" } },
			};
			new FileApp().Write_SerializeJson(data, FileApp.ts_Log(@"t_.json"));

		}

		[TestMethod]
		public void t_2()
		{
			//string id = "";
			//bool query = false;
			//var _x = query & string.IsNullOrEmpty(id);
			var x = new DateTime(2020, 1, 1, 16, 30, 30);
			var x1 = x.ToString("YYYY/");
		}

		/// <summary>
		/// 
		/// </summary>
		/// https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/arrays/implicitly-typed-arrays
		[TestMethod]
		public void t_取得料號單位()
		{
			//string[,] a = { { "A", "B" }, { "C", "D" } };
			var z = new string[] { "A", "B" };
			//var z1 = { { "A", "B" } };
			var z2 = new string[,] { { "A", "B" } };
			var a = new List<string[,]> { new string[,] { { "A", "B" }, { "C", "D" }, { "C", "D" } } };
			foreach (var i in a)
			{
				Console.WriteLine(i);
			}
			x_fn(new[] { "A", "B" }, new[] { "A", "B" });
		}

		void x_fn(params string[][] list)
		{
			foreach (var i in list)
			{
				Console.WriteLine(i);
			}
		}

		[TestMethod]
		public void t_3()
		{
			var _list = new List<QtyItem>() {
				new QtyItem(){ Qty = 100},
				new QtyItem(){ Qty = 101}
			};
			var z = QtyItem.Check(_list, 200,null,null,false);
			_list.Add(new QtyItem() { Qty = -1 });
			var z1 = QtyItem.Check(_list, null,null, null, false);
		}

		[TestMethod]
		public void t_4()
		{
			var s = "Lot";
			var s1 = Enum.Parse(typeof(BarCodeSrc), s);
			switch (s1) {
				case BarCodeSrc.Lot:
					break;
			}
		}

		[TestMethod]
		public void t_5()
		{
			int x = 1;
			bool _t = true;
			var _val = ((object)null).ToString();
			if (string.IsNullOrEmpty(_val) == false)
			{
				_t = int.TryParse(null, out x);
				_t = int.TryParse("A", out x);
				_t = int.TryParse("", out x);
				_t = int.TryParse("1", out x);
			}
			
		}

		[TestMethod]
		public void t_6()
		{
			decimal x ;
			bool _t = true;
			_t = decimal.TryParse(null, out x);
			_t = decimal.TryParse("A", out x);
			_t = decimal.TryParse("", out x);
			_t = decimal.TryParse("1.5555", out x);
		}


		[TestMethod]
		public void t_取得版本序號()
		{
			var FileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)
				.FileVersion.ToString();

			//AssemblyName assName = Assembly.GetExecutingAssembly().GetName();
			//string version = assName.Version.ToString();
			Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			string ver = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision.ToString());
		}


		[TestMethod]
		public void t_GTiMES_saltkey()
		{
			var GTiMES_loginkey = "EGAk1l624BeDmFrEoTuG6GXUwnCbdfouiIGjpZaZxjRpC89ybH2iVIYA2cYhIWIrFm2kdfXoKEst3khRYca/qy6OVixxO042kRftIki+eE41pQTHZlnTUKphMk4ML6FimX6KMMywpYay86Sy3fo6Qa6Wj98VLEiDzx1BoeShQPXnSv51gKqkiEDLvJZDrJXWpcd1AUBjQtR0FWKl1TVnmrMex5YBPJECX1zlkdeFFdrtuF61e3imvwJVdj3wBRBOA222w8+DKhGxb2mRNEMcQi3dN4f9i0OFGHk+nv5p8fDf+GmgkTaYFUceKpY7VdCqoxxs8BUNr/+afCrEmxmuBaDpwf2ewQszIDFrbIq6Cn3tHucG82nB5Rwl3ZwugCBDoe1Q5LkiG9oiKvEu7EhRP1vCeph1XUAiYkHquTr2S6FfWjSUzl8I6Bifzwxi6q9i2pETJQ+k4hbWLmZkhLk3vgDZ3xiWUBVgrcbYyUna4PanQI6Ei243u0FYsWA9ofY+v4yc5SdcNRfqwVeVqIXDsD3JwpPg8jhHlTZ9UX9r7m4jxMjVt2f8g882iqqcd1XQktyrV3jl+l9cY3tCNKsHyDPyNl92lIhMtB4DOYeNZx4=";
			var GTiMES_saltkey = "DYWPNt7Co";
			var User = Encrypter.DecryptAES(GTiMES_loginkey, GTiMES_saltkey).ToObject<CurrentLoginUserModel>();
			
		}

		[TestMethod]
		public void t_GTiMES_saltkey1()
		{
			var A = "Ui9PSO0YBlrQ47RuXDzT6A==";
			var B = "P3wRbcoQHxFlvgTT";
			var T = Encrypter.DecryptAES(A,B);
			//
			//var T = pwd.EncryptAES(A, B);

		}

		[TestMethod]
		public void t_x()
		{
			var UItest = "RollCheckIn_Case1";
			var _code = "";
			if (UItest != null)
			{
				string Baseurl = $"http://localhost:59394/GenesisNewMes/Example/Self/UITest?name={UItest}";
				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				var r = client.GetAsync(Baseurl).Result;
				var res = r.Content.ReadAsStringAsync().Result;
				var _o = JsonConvert.DeserializeObject(res);// res.Replace("\"", "");
				_code = $" x = JSON.parse({_o.ToJson()});";
			}
		}

 



		public decimal? t_6__(string val) {
			decimal r;
			if (string.IsNullOrWhiteSpace(val) == false) { 
				if (decimal.TryParse(val, out r)){
					return  decimal.Round(r, 1);
				}
			}
			return null;
		}


	}



}
