using BLL.Base;
using BLL.DataViews.Res;
using BLL.MES;
using Frame.Code;
using Genesis;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.Helper;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using static BLL.MES.WIPInjectServices;
using static Genesis.Library.BLL.Label.LabelBaseService;

namespace UnitTestProject.TestUT
{
	[TestClass]
	public class UnitTest_ : _testBase
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
		public void t_ProjectCustomer()
		{
			string ProjectCustomer = System.Configuration.ConfigurationManager.AppSettings["urlSAP"] != null ?
				System.Configuration.ConfigurationManager.AppSettings["urlSAP"] : "";

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
			var z = QtyItem.Check(_list, 200, null, null, false);
			_list.Add(new QtyItem() { Qty = -1 });
			var z1 = QtyItem.Check(_list, null, null, null, false);
		}

		[TestMethod]
		public void t_4()
		{
			var s = "Lot";
			var s1 = Enum.Parse(typeof(BarCodeSrc), s);
			switch (s1)
			{
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
			decimal x;
			bool _t = true;
			_t = decimal.TryParse(null, out x);
			_t = decimal.TryParse("A", out x);
			_t = decimal.TryParse("", out x);
			_t = decimal.TryParse("1.5555", out x);
		}

		[TestMethod]
		public void t_GetParameterGroupTypes()
		{
			var x = new ServicesBase().GetEnumList("ParameterGroupType");
		}


		[TestMethod]
		public void t_RequiredFieldCheck()
		{
			var d = new QC_CODE_LETTER();
			new ServicesBase().RequiredFieldCheck(d);
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
			var A = "oyx/67HMQms51wSVODquTg==";
			var B = "K1XyUcfDObcmqVob";
			var T = Encrypter.DecryptAES(A, B);
			//
			//var T = pwd.EncryptAES(A, B);

		}



		[TestMethod]
		public void t_GTiMES_saltkey2()
		=> _DBTest(Txn =>
		{
			//var T = Encrypter.DecryptAES(userEntity.PWD, userEntity.SECOND_PWD);
			//
			//Genesis.Mes.Library.Security.Password pwd = new Genesis.Mes.Library.Security.Password();
			//pwd.DecryptAES(password, userEntity.PWD, userEntity.SECOND_PWD);			return Txn.result;
		});



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


		[TestMethod]
		public void t_fn111()
		{
			var chk_NewVer = typeof(WP_IPQC_LOT).GetProperty("LOT") != null;
		}

		[TestMethod]
		public void t_fn()
		{
			int t = 1;
			//Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機? z = t.ts_NullEnum<Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機>();

			//Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機 z1 = t.ts_Enum<Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機>();
			var a = new PF_PARTNO()
			{
				//ATTRIBUTE_36 = 10
			};
			var b = new PF_PARTNO()
			{
				//ATTRIBUTE_36 = (decimal)10.000
			};
			var _AD_LOG = new AD_LOG()
			{
				FUN_NAME = "PartNoServices",
				ACTION = "UpdateEnableFlag",
				TARGET_TABLE = "PF_PARTNO_VER",
				//TARGET_PK = version.PARTNO_VER_SID,
				//VALUE_LINK_SID = GetSID(),
				CREATE_USER = "TEST",
				CREATE_DATE = DateTime.Now,
			};
			var z = a.parseLogValues(b, _AD_LOG);
		}

		[TestMethod]
		public void t_x1()
		{
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo("zh-TW"); // 使用特定的文化，例如 en-US

			//// 建立 ResourceManager
			//ResourceManager resourceManager = new ResourceManager("RES.BLL.Face", typeof(LocalizationExample).Assembly);

			//// 使用 GetString 方法取得特定鍵對應的值
			//string processingLineText = resourceManager.GetString("ProcessingLine", cultureInfo);
			var a = new ResourceManager("RES.BLL.Face", typeof(RES.BLL.Face).Assembly);
			var a1 = typeof(RES.BLL.Face);
			Assembly a2 = a1.Assembly;
			string[] resNames1 = a2.GetManifestResourceNames();
			var ttt = a.GetString("RES.BLL.mes.resources", cultureInfo)?.ToString().Trim();


			Assembly assem = typeof(RES.BLL.Face).Assembly;

			// Enumerate the resource files.
			string[] resNames = assem.GetManifestResourceNames();
			if (resNames.Length == 0)
				Console.WriteLine("   No resources found.");

			//foreach (var resName in resNames)
			//	Console.WriteLine("   Resource: {0}", resName.Replace(".resources", ""));



			var _val = new ResourceManager("RES.BLL.Face", typeof(RES.BLL.Face).Assembly).GetString("QuoteOnce1")?.ToString().Trim();
			var z = typeof(RES.BLL.Face);
			var _val1 = new ResourceManager(z).GetString("QuoteOnce")?.ToString().Trim();
		}







		public decimal? t_6__(string val)
		{
			decimal r;
			if (string.IsNullOrWhiteSpace(val) == false)
			{
				if (decimal.TryParse(val, out r))
				{
					return decimal.Round(r, 1);
				}
			}
			return null;
		}

		private async Task SubFunctionAsync(int delayTime)
		{
			Console.WriteLine($"Sub function will start after {delayTime} ms");

			// 等待指定的時間
			await Task.Delay(delayTime);

			Console.WriteLine("Sub function executed after delay");
		}



		[TestMethod]
		public void t_20251225_col_sel()
		=> _DBTest((txn) =>
		{
			var _list = (from a in txn.EFQuery_MES.FC_EQUIPMENT
						 where a.STATE_NO == "Run"
						 select a)
				.ToList();

			//只有 Head 基礎模式
			var m_Head = new col_sel<FC_EQUIPMENT>("保養單類別");
			object _o = "保養單類別";
			var m_Head_obj = col_sel<FC_EQUIPMENT>.n(_o);

			//直接傳入 Dictionary
			var _dc = new Dictionary<string, string>() {
				{"D", "預防保養單" },
				{"U", "達次保養單" }
			};
			var m_Dec = col_sel<FC_EQUIPMENT>.n(_dc);

			//直接傳入 Dictionary
			Func<FC_EQUIPMENT, object> _Fn = (c)=>{ return c.UPDATE_DATE; };
			var m_Fn = col_sel<FC_EQUIPMENT>.n(_Fn);
			var t = m_Fn.DynFunc(_list.FirstOrDefault());

			//Header + Map
			var m_Header_Map = col_sel<FC_EQUIPMENT>.n(
				"保養單類別",
				("D", "預防保養單"),
				("U", "達次保養單")
			);

			//只有 params Map
			var m_Map = new col_sel<FC_EQUIPMENT>(
				("D", "預防保養單"),
				("U", "達次保養單")
			);


			//Header + Fun
			var colConfirm = col_sel<FC_EQUIPMENT>.n(
				"審核狀態",
				row => row.UPDATE_DATE.ToString("yyyyMMdd")
			);

		}, true);

		[TestMethod]
		public void t_20251225_ColParse()
		=> _DBTest((txn) =>
		{
			var _list = (from a in txn.EFQuery_MES.FC_EQUIPMENT
						 where a.STATE_NO == "Run"
						 select a)
				.ToList();

			var _ent = _list.FirstOrDefault();

			//foreach (var row in _list) {
			//	//var _t = m_Head.GetValue<string>(row);
			//	var _t1 = m_Head.GetValue(row);
			//};

			//只有 基礎模式,只處理欄位 及 欄位抬頭
			var m_Col = ColParse<FC_EQUIPMENT>.n("UPDATE_DATE");
			var m_Col_Head = ColParse<FC_EQUIPMENT>.n("UPDATE_DATE","欄位名稱");



			//ValueMap
			var m_ValueMap = ColParse<FC_EQUIPMENT>.n
				("UPDATE_DATE",
					"欄位名稱", //彈性增加
				("D", "預防保養單"),
                ("U", "達次保養單"));

            //Dictionary
            var _dc = new Dictionary<string, string>() {
                {"D", "預防保養單" },
                {"U", "達次保養單" }
            };
            var m_Dec = ColParse<FC_EQUIPMENT>.n
				("UPDATE_DATE"
					,"欄位名稱" //彈性增加
				, _dc);

			//直接傳入 DynFunc 
			//Func<FC_EQUIPMENT, object> _Fn = (c) => { return c.UPDATE_DATE; };
			//var m_Fn = ColParse<FC_EQUIPMENT>.n((c)=>c.UPDATE_DATE);
			var m_Fn = ColParse<FC_EQUIPMENT>.n((c)=>c.UPDATE_DATE.ToString("yyyyMMdd"));
			var t = m_Fn.GetValue(_ent);

			var _dyn = new{
				UPDATE_DATE = DateTime.Now
			};


			/*
			 這一段的寫法 ,目的是為了 省略 ColParse<FC_EQUIPMENT>.n  重覆的字段 
			 */
			var _src = new FC_EQUIPMENT();
            var m_Fn_t1 = _src.nColParse(c => c.UPDATE_DATE.ToString("yyyyMMdd"));
			var t01 = m_Fn_t1.GetValue(_ent);
			/* 無法實現, 因為底層的物件型別不同 
            var t02 = m_Fn_t1.GetValue(_dyn);
			*/

			/*
			 動態物件 , 只能使用 columnName , 而且能兼容於 Entity

			但像以下語法 ,經確認是無法執行的 
			 */
			var m_Fn_t2 = ColParse<dynamic>.n("UPDATE_DATE");
			var t21 = m_Fn_t2.GetValue(_dyn);
			var t22 = m_Fn_t2.GetValue(_ent);

			var m_Fn_t3 = _dyn.nColParse("UPDATE_DATE");
			var t31 = m_Fn_t3.GetValue(_dyn);
			/*
			var t32 = m_Fn_t3.GetValue(_ent);

			以上這段不可行 , 主要的原因是 ColParse 在初始化是,就需要先決定要,動態要處理的 物件是什麼型別,
				目前的模式--決定後,就無法更改
			之所以會採用這樣模式 , 最根本的原因就是 
				匿名型別 (dynamic / Dapper / object) 
				具名型別 (EF entity / T) 
				上述兩種是完全不同的型別跟處理模式
			為了讓兩者能使用同一個方法執行, 在 ColParse 底層中 , 是使用 IRowAccessor 來做執行模式的切分,
				也因此,無法接受動態變更
			*/
		});

	}

	public static partial class AD_ext
	{

		public static ColParse<T> nColParse<T>(this T _self, string columnName)
		=>ColParse<T>.n(columnName);

		public static ColParse<T> nColParse<T>(this T _self, Func<T, object> DynFunc)
		=> ColParse<T>.n(DynFunc);
	}


	public class col_sel<T>
	{
		/// <summary>
		/// Excel 欄位抬頭
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// 欄位值轉換表
		/// </summary>
		public Dictionary<string, string> ValueMap { get; set; }

		/// <summary>
		/// 動態函數，可處理 row 單位，返回最終輸出值
		/// </summary>
		public Func<T, object> DynFunc { get; set; }


		public col_sel(params (string key, string value)[] mappings)
		{
			if (mappings != null && mappings.Length > 0)
				ValueMap = mappings.ToDictionary(t => t.key, t => t.value);
		}


		public col_sel(string header, Func<T, object> dynFunc = null)
		{
			Header = header;
			DynFunc = dynFunc;
		}

		/// <summary>
		/// 取得欄位對應的最終輸出值
		/// </summary>
		/// <param name="row">目前 row</param>
		/// <param name="originalValue">原欄位值</param>
		/// <returns>轉換後值</returns>
		public object GetValue(T row, object originalValue)
		{
			object value = originalValue;

			// 先用 ValueMap 轉換（如果存在對應）
			if (value != null && ValueMap != null && ValueMap.TryGetValue(value.ToString(), out string mapped))
				value = mapped;

			// 再用 DynFunc 進一步處理（如果設定）
			if (DynFunc != null)
				value = DynFunc(row);

			return value;
		}

		public static col_sel<T> n(object sel)
		{
			if (sel is string header)
				return new col_sel<T>(header);
			if (sel is col_sel<T> cs)
				return cs;

			var r = new col_sel<T>(string.Empty);
			if (sel is Dictionary<string, string> dc) {
				r.ValueMap = dc;
			}else if (sel is Func<T, object> fn){
				r.DynFunc = fn;
			}
			return r;
		}
		/// <summary>
		/// 建構子：Header + ValueMap + DynFunc
		/// </summary>
		/// <param name="header">欄位抬頭</param>
		/// <param name="dynFunc">動態函數</param>
		/// <param name="mappings">key-value 對應表</param>
		public static col_sel<T> n(string header, Func<T, object> dynFunc, params (string key, string value)[] mappings)
		{
			var r = new col_sel<T>(header, dynFunc);
			if (mappings != null && mappings.Length > 0)
				r.ValueMap = mappings.ToDictionary(t => t.key, t => t.value);
			return r;
		}

		public static col_sel<T> n(string header, params (string key, string value)[] mappings)
		=> col_sel<T>.n(header, null, mappings);

		public static col_sel<T> n(string header, Dictionary<string, string> valueMap)
		{
			var r = new col_sel<T>(header);
			r.ValueMap = valueMap;
			return r;
		}
	}

	public class ColParse_<T>
	{
		/// <summary>
		/// 匯出用欄位抬頭
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// dynamic / DB 欄位名稱
		/// </summary>
		public string ColumnName { get; }

		/// <summary>
		/// 欄位值轉換表（ValueMap）
		/// </summary>
		public Dictionary<string, string> ValueMap { get; set; }

		/// <summary>
		/// 動態計算函數（最高優先權）
		/// </summary>
		public Func<T, object> DynFunc { get; set; }

		#region Constructor

		public ColParse_(string columnName, string header = null)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException(nameof(columnName));

			ColumnName = columnName;
			Header = header ?? columnName;
		}

		#endregion

		#region 核心 API

		/// <summary>
		/// 由 row 取得「最終輸出值」
		/// </summary>
		public object GetValue(T row)
		{
			if (row == null) return null;

			// 最高優先權
			if (DynFunc != null)return DynFunc(row);
			
			// 先取原始欄位值（一定來自 row）
			object value = GetOriginalValue(row);

			// ValueMap 轉換
			if (value != null && ValueMap != null &&
				ValueMap.TryGetValue(value.ToString(), out var mapped)){
				value = mapped;
			}
			return value;
		}

		#endregion

		#region 內部取值邏輯（dynamic / Dapper / Expando）

		private object GetOriginalValue(T row)
		{
			// ExpandoObject / Dapper dynamic
			if (row is IDictionary<string, object> dict)
			{
				if (dict.TryGetValue(ColumnName, out var val))
					return Normalize(val);

				// 容錯：忽略大小寫
				foreach (var kv in dict)
				{
					if (string.Equals(kv.Key, ColumnName, StringComparison.OrdinalIgnoreCase))
						return Normalize(kv.Value);
				}

				return null;
			}

			// Reflection fallback（保險）
			var prop = row.GetType().GetProperty(
				ColumnName,
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

			return prop != null ? Normalize(prop.GetValue(row)) : null;
		}

		private object Normalize(object value)
			=> value == DBNull.Value ? null : value;

		#endregion

		#region Factory / DSL（承襲 col_sel 的使用感）

		public static ColParse<T> n(object sel)
		{
			if (sel is string col)
				return new ColParse<T>(col);

			if (sel is ColParse<T> cp)
				return cp;

			throw new InvalidOperationException("Unsupported selector type");
		}

		public static ColParse<T> n(
			string columnName,
			string header,
			Func<T, object> dynFunc,
			params (string key, string value)[] mappings)
		{
			var r = new ColParse<T>(columnName, header)
			{
				DynFunc = dynFunc
			};

			if (mappings != null && mappings.Length > 0)
				r.ValueMap = mappings.ToDictionary(t => t.key, t => t.value);

			return r;
		}

		public static ColParse<T> n(
			string columnName,
			params (string key, string value)[] mappings)
			=> n(columnName, string.Empty, null, mappings);

		public static ColParse<T> n(
			string columnName,
			string header,
			params (string key, string value)[] mappings)
			=> n(columnName, header, null, mappings);

		public static ColParse<T> n(
			string columnName,
			string header,
			Dictionary<string, string> valueMap)
		{
			var r = new ColParse<T>(columnName, header)
			{
				ValueMap = valueMap
			};
			return r;
		}

		public static ColParse<T> n(
			string columnName,
			Dictionary<string, string> valueMap)
			=> n(columnName, columnName, valueMap);

		/// <summary>
		/// 如果直接定義 DynFunc, 就不需要再 columnName , ValueMap ,直接寫在 DynFunc 即可
		/// </summary>
		/// <param name="DynFunc"></param>
		/// <returns></returns>
		public static ColParse<T> n(Func<T, object> DynFunc)
		=> n("-", string.Empty, DynFunc);

		#endregion
	}


	public class ColParse_01<T>
	{
		private readonly string _columnName;

		public ColParse_01(string columnName) {
			_columnName = columnName;
		}

		/// <summary>
		/// 取得 dynamic row 中指定欄位的值
		/// </summary>
		public object GetValue(dynamic row)
		{
			if (row == null) return null;

			// 1️⃣ ExpandoObject / Dapper dynamic
			if (row is IDictionary<string, object> dict)
			{
				if (dict.TryGetValue(_columnName, out var val))
					return NormalizeDbValue(val);

				// 容錯：忽略大小寫
				foreach (var kv in dict)
				{
					if (string.Equals(kv.Key, _columnName, StringComparison.OrdinalIgnoreCase))
						return NormalizeDbValue(kv.Value);
				}

				return null;
			}

			// 2️⃣ fallback：Reflection（保險機制）
			var prop = row.GetType().GetProperty(
				_columnName,
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
			);

			if (prop != null)
				return NormalizeDbValue(prop.GetValue(row));

			return null;
		}

		/// <summary>
		/// 泛型版本：直接轉型回傳
		/// </summary>
		public TResult GetValue<TResult>(dynamic row)
		{
			var val = GetValue(row);

			if (val == null)
				return default;

			return (TResult)Convert.ChangeType(val, typeof(TResult));
		}

		private object NormalizeDbValue(object value)
		{
			return value == DBNull.Value ? null : value;
		}
	}

}
