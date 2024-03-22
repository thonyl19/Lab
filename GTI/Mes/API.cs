using BLL.MES;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using vFile = System.IO.File;

namespace UnitTestProject
{
	[TestClass]
	public partial class t_API : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_SaveAPI
			{
				get
				{
					return FileApp.ts_Log(@"API\t_SaveAPI.json");
				}
			}

			internal static string t_GetAPI
			{
				get
				{
					return FileApp.ts_Log(@"API\t_GetAPI.json");
				}
			}

			internal static string t_API_Param
			{
				get
				{
					return FileApp.ts_Log(@"API\t_API_Param.json");
				}
			}
			
		}


		[TestMethod]
		public void t_GetAsync() { 
			var  httpClient = new HttpClient();
			HttpResponseMessage httpResponseMessage = httpClient.GetAsync("http://127.0.0.1:3000/api/issue").Result;

			int statusCode = (int)httpResponseMessage.StatusCode;
			//Console.WriteLine($"Http 狀態碼: {statusCode}");

			string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
			//Console.WriteLine($"Http 回應內容: {content}");
		}

		[TestMethod]
		public void PostAsync()
		{
			var httpClient = new HttpClient();
			var payload = "{\"CustomerId\": 5,\"CustomerName\": \"Pepsi\"}";

			HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
			HttpResponseMessage httpResponseMessage = httpClient.PostAsync("http://127.0.0.1:3000/api/test",c).Result;

			int statusCode = (int)httpResponseMessage.StatusCode;
			//Console.WriteLine($"Http 狀態碼: {statusCode}");

			string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
			//Console.WriteLine($"Http 回應內容: {content}");
		}
		#region [ Sample ] 
		/*
		[TestMethod]
		public void _Sample()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo("GTI22060711213497371");
			var c = lot.GetCurrentCarrierInfo();
			var exp = c.CURRENT_CAPACITY - 1;
			Txn.DoTransaction(new Carrier.DTC_AdjustmentCapacity(lot, c, -1));
			var act = lot.GetCurrentCarrierInfo().CURRENT_CAPACITY;
			Assert.AreEqual(exp, act,$"扣數後, 數值應為 {exp}");

			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));
			new FileApp().Write_SerializeJson(dt, _log.t_splitBIN);

			var _r = new FileApp().Read_SerializeJson(_log.t_splitBIN);
		}, true);
		*/
		#endregion


		[TestMethod]
		public void t_APIList()
		{
			var x = DDLServices.APIList();
		}

		[TestMethod]
		public void t_Add_PF_OPERATION_EXPAND()
		=> TxnBase.LzDBQuery(txn =>
		{
			var r = new PF_OPERATION_EXPAND()
			{
				OPER_SID = "GTI23010917340392787",
				OPER_CODE = "C02-0090",
				OPER_NAME = "雷切",
				FUN_SID = txn.GetSID(),
				FUN_CODE = "API",
				SETTING_JSON = "",

			};
			//txn.UserNo = "Admin";
			txn.EntityCommonSetVal(r, isNeedInit: true);
			//new OperationExpandServices().UpdateData(r);

			var _ef = txn.EFQuery<PF_OPERATION_EXPAND>();
			_ef.Create(r);
			_ef.SaveChanges();

			return txn.result;
		});


		[TestMethod]
		public void t_SaveAPI()
		{
			var s = new FileApp().Read_SerializeJson<PF_OPERATION_API>(_log.t_SaveAPI);
			new DynFuncModuleServices().SaveAPI(s);
		}


		[TestMethod]
		public void t_GetAPI()
		{
			var s = WIPOperConfigServices.GetAPI(true,true, "GTI23010916485492704");
			FileApp.WriteSerializeJson(s, _log.t_GetAPI);
		}


		[TestMethod]
		public void t_API_Param()
		{
			var s = DDLServices.API_Param();
			FileApp.WriteSerializeJson(s, _log.t_API_Param);
		}


		[TestMethod]
		public void t_API_i18n()
		{
			//new APPController().i18nAdd("", "", "");
            var resxFilePath = @"G:\LIO_Dev\Library\RES\BLL\Face.resx";

			//using (ResXResourceWriter resxWriter = new ResXResourceWriter(resxFilePath))
			//{
			//	resxWriter.AddResource("Test", "t"); // 新增繁體中文
			//}

            var resxSet = new ResXResourceSet(resxFilePath);

			////新增資料
			//resxSet.Add("hello_world", "Hello World");

			//寫入.resx 檔案
			//using (var writer = new ResXResourceWriter(resxFilePath))
			//{
			//    writer.AddResource("test", "test");
			//    writer.Generate();
			//}
			var tarFile1 = @"G:\LIO_Dev\Library\RES\BLL\Face.zh-TW.resx";
			string key = "TEST", val = "TTTT";
//Server.MapPath($@"~/..\Library\RES\BLL\{res}{res_tp}.resx");
			string fileContent = vFile.ReadAllText(tarFile1);
			// 使用正則表達式進行置換
			string pattern = $@"</data>\s*</root>";
			string replacement = $@"</data>
	<data name=""{key}"" xml:space=""preserve"">
		<value>{val}</value>
	</data>
</root>";
			string newContent = Regex.Replace(fileContent, pattern, replacement);
			//vFile.WriteAllText(tarFile, newContent);

		}

	}
}

