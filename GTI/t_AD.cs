using BLL.DataViews.Res;
using BLL.MES;
using BLL.MES.DataViews;
using BLL.MVC;
using Dal.Repository;
using Frame.Code;
using Genesis.Library.BLL.MES;
using Genesis.Library.BLL.MVC.AutoGenerate;
using MDL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Resources;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.ParameterUtility;
using static Genesis.Library.BLL.MES.DataViews.System;
using _v8n = BLL.MES.FluentValidation;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace UnitTestProject
{
	//[assembly:NeutralResourcesLanguage("en")]
	[TestClass]
	public class t_AD : _testBase
	{
		public static CurrentLoginUserModel UserModel = new CurrentLoginUserModel()
		{
			UserName = "test"
		};
		static class _log
		{
			internal static string t_MESSystemConfig
			{
				get
				{
					return FileApp.ts_Log(@"AD\t_MESSystemConfig.json");
				}
			}

			//測試檢核程序的正確性
			internal static string t_AD_ENCODE_FORMAT_Check
			{
				get
				{
					return FileApp.ts_Log(@"AD\t_AD_ENCODE_FORMAT_Check.json");
				}
			}

			//測試 資料 Update 
			internal static string t_SequenceNum_Item_Update
			{
				get {
					return FileApp.ts_Log(@"AD\t_SequenceNum_Item_Update.json");
				}
			}


		}




		[TestMethod]
		public void t_GetParameterGroupByNo()
		{
			var _fn = new ParameterGroupFunction(DBC);
			var _r = _fn.GetParameterGroupByNo("WorkingTimeType");
			new FileApp().Write_SerializeJson(_r, FileApp.ts_Log(@"AD\t_GetParameterGroupByNo.json"));

		}


		/// <summary>
		/// 依照傳入参数編號取得参数資料清單
		/// </summary>
		[TestMethod]
		public void t_GetParametersByGroupNo()
		{
			var _fn = new ParameterGroupItemFunction(DBC);
			var _r = _fn.GetParametersByGroupNo("WorkingTimeType");

			new FileApp().Write_SerializeJson(_r, FileApp.ts_Log(@"AD\t_GetParametersByGroupNo.json"));

		}

		/// <summary>
		/// 依照傳入参数編號取得参数資料清單
		/// </summary>
		[TestMethod]
		public void t_SequenceNum_Read()
		{
			var _r = new ADMServices().SequenceNum_Read("GTI20102115385063472");

			new FileApp().Write_SerializeJson(_r, FileApp.ts_Log(@"AD\t_SequenceNum_Read.json"));

		}

		/// <summary>
		/// 測試新增
		/// </summary>
		[TestMethod]
		public void t_SequenceNum_Add()
		{
			/*
			載入測試樣本 
			執行新增
			判斷是否成功 
			讀入 db 現有資料值,
			跟回傳值比對是否正確
			*/
			var _svc = new ADMServices();
			var _data = FileApp.Read_SerializeJson<SequenceNum_PostData>(_log.t_SequenceNum_Item_Update);

			//NO 不能重覆
			_data.main.ENCODE_FORMAT_SID = null;
			_data.main.ENCODE_FORMAT_NO += DateTime.Now.ToString("_yyyyMMdd");
			var _r1 = _svc.SequenceNum_Save(_data, true, true);
			Assert.IsTrue(_r1.Success);
			var _data_A = (SequenceNum_PostData)_r1.Data;

			var _data_B = _svc.SequenceNum_Read(_data_A.main.ENCODE_FORMAT_SID);
			var A = _data_A.ToJson();
			var B = _data_B.ToJson();
			Assert.AreEqual(A, B);

			//回寫,以便測試 Update
			FileApp.WriteSerializeJson(_data_B, _log.t_SequenceNum_Item_Update);
		}

		/// <summary>
		/// 測試新增
		/// </summary>
		[TestMethod]
		public void T_SequenceNum_Update()
		{
			/*
			載入測試樣本 
			執行新增
			判斷是否成功 
			讀入 db 現有資料值,
			跟回傳值比對是否正確
			*/
			var _svc = new ADMServices();
			var _data = FileApp.Read_SerializeJson<SequenceNum_PostData>(_log.t_SequenceNum_Item_Update);
			var _mark = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			var _chkTKey = _data.items[0].ENCODE_FORMAT_ITEM_SID;
			_data.main.DESCRIPTION = _mark;
			var _r1 = _svc.SequenceNum_Save(_data, false, true);
			Assert.IsTrue(_r1.Success);

			var _data_A = (SequenceNum_PostData)_r1.Data;
			Assert.AreEqual(_data_A.main.DESCRIPTION, _mark, "檢核更新-資料應該一致.");
			Assert.AreNotEqual(_data_A.items[0].ENCODE_FORMAT_ITEM_SID, _chkTKey, "更新後 SID應該不一樣.");

			var _data_B = _svc.SequenceNum_Read(_data_A.main.ENCODE_FORMAT_SID);
			var A = _data_A.ToJson();
			var B = _data_B.ToJson();
			Assert.AreEqual(A, B);


		}

		[TestMethod]
		public void T_AD_ENCODE_FORMAT_Check()
		{
			var _r = FileApp.Read_SerializeJson<SequenceNum_PostData>(_log.t_AD_ENCODE_FORMAT_Check);
			var result = _v8n.AD_ENCODE_FORMAT.Check
					(t_AD.UserModel, _r, true);
			Assert.IsTrue(result.Success);
			Assert.IsFalse(string.IsNullOrEmpty(_r.main.ENCODE_FORMAT_SID), "新增模式,ENCODE_FORMAT_SID 不應為空值");
			Assert.AreEqual(_r.main.CREATE_USER, "test", "新增模式,CREATE_USER 應為 test");
			Assert.IsNotNull(_r.main.CREATE_DATE, "新增模式,CREATE_DATE 不應為空值");

			_r.items = new List<MDL.MES.AD_ENCODE_FORMAT_ITEM>();
			var result1 = _v8n.AD_ENCODE_FORMAT.Check
					(t_AD.UserModel, _r, true);
			Assert.IsFalse(result1.Success);
			var msg = string.Format(RES.BLL.Message.MustInput, RES.BLL.Face.SequenceDetail);
			Assert.AreEqual(result1.Message, msg, "測試身檔明細檢核-應該要有符合的錯誤訊息");


		}
		[TestMethod]
		public void t_MESSystemConfig()
		{
			MESSystemConfig systemConfig;
			DbContext dbContext = MVCContext.Create();
			/*
			 這裡的資料表 跟 MES 不是同一個資料庫, 所以必須轉用如下的方法 
			 */
			//using (var dbc = new MDL.MESContext())
			using (var dbc = MVCContext.Create())
			{
				using (BaseAdSystemConfigServices _svc = new BaseAdSystemConfigServices())
				{
					_svc.DbContext = dbc;
					//var readEntity = new SystemConfigServices().GetDataEntity("1");
					systemConfig = _svc.GetEntityByKey("1").SystemconfigJson.ToObject<MESSystemConfig>();
				}

			}
			new FileApp().Write_SerializeJson(systemConfig, _log.t_MESSystemConfig);
		}

		[TestMethod]
		public void t_()
		{
			Dictionary<string, string> _dc = new Dictionary<string, string>();


			var systemConfig = FileApp.Read_SerializeJson<MESSystemConfig>(_log.t_MESSystemConfig);
			var dataType = systemConfig.GetType();
			var _list = dataType.GetProperties();

			var z = typeof(RES.BLL.Face);
			var _res = new ResourceManager(z);
			foreach (var _item in _list) {
				var _val = _res.GetString(_item.Name)?.ToString().Trim();
				_dc.Add(_item.Name, _val ?? _item.Name);
			}

		}


		[TestMethod]
		public void t_1(){
			 
			// Try to load the assembly.
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
	}


}
