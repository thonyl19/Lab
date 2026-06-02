using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Frame.Code.Excel;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction.TOL;
using Genesis.Library.BLL.DTC;
using Genesis.Library.BLL.MES.OperTask;
using Genesis.Library.BLL.WRP;
using Genesis.Library.Frame.Code.Web.TableQuery;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static Genesis.Library.BLL.ADM.BomServices;
using _Prd = Genesis.Library.BLL.MES.OperTask;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;
using BLL.DataViews.Edc;
using static Genesis.Library.BLL.DTC.Lot;
using static Genesis.Gtimes.WIP.LotUtility;
using Genesis.Library.BLL.ADM;
using Genesis.Gtimes.Transaction.WIP;
using System.Data.Entity;
using _bllSvc = Genesis.Library.BLL.ZZ_ZAC;

namespace UnitTestProject
{
	/// <summary>
    /// </summary>
    [TestClass]
	public class t_ZZ_ZAC : _testBase
	{
		static class _log
		{
			internal static string Example
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\ZZ_ZAC\Example.json");
				}
			}
		}

		[TestMethod]
		public void t_20260525_GetOperMaterials_含主要物料與替代物料()
		=> _DBTest((txn) =>
		{
			var db = txn.EFQuery_MES;
			string testRouteVerSid = "TEST_ROUTE_VER_001";
			string testProductPartNo = "TEST_PRODUCT_001";
			string testMainMaterialSid = "MAT_MAIN_SID_001";
			string testSubMaterialSid = "MAT_SUB_SID_001";

			// 1. Arrange: 於交易內建立隔離的測試資料
			// 建立主料資訊 (PF_PARTNO)
			db.PF_PARTNO.Add(new PF_PARTNO
			{
				PARTNO_SID = testMainMaterialSid,
				PARTNO = "MAIN_RESISTOR",
				PART_NAME = "精密電阻",
				CREATE_USER = "UnitTest",
				CREATE_DATE = DateTime.Now,
				UPDATE_USER = "UnitTest",
				UPDATE_DATE = DateTime.Now
			});

			// 建立主要物料設定
			var mainPart = new PF_PARTNO_ROUTEVER_OPER_PARTNO
			{
				P_SID = txn.GetSID(),
				PARTNO_PARTNO_SID = "MAIN_ASSIGN_001",
				ROUTE_VER_SID = testRouteVerSid,
				PARTNO = testProductPartNo,
				PARTNO_SID = "TEST_PRODUCT_SID_001",
				PART_NAME = "測試產品",
				OPER_PARTNO_SID = testMainMaterialSid,
				DISPLAY_NAME = "精密電阻",
				USE_RATE = 2.0m,
				ROUTE_VER_OPER_SID = "TEST_ROUTE_OPER_SID_001",
				CONFIG_FLAG = "P",
				CREATE_USER = "UnitTest",
				CREATE_DATE = DateTime.Now,
				UPDATE_USER = "UnitTest",
				UPDATE_DATE = DateTime.Now
			};
			db.PF_PARTNO_ROUTEVER_OPER_PARTNO.Add(mainPart);

			// 建立替代物料設定
			db.PF_PARTNO_ROUTEVER_OPER_SUBPN.Add(new PF_PARTNO_ROUTEVER_OPER_SUBPN
			{
				P_SID = txn.GetSID().ToString(),
				MAIN_PK = "MAIN_ASSIGN_001", // 精確關聯至主料的 PARTNO_PARTNO_SID
				SUB_PARTNO_SID = testSubMaterialSid,
				SUB_PARTNO = "SUB_RESISTOR",
				SUB_PARTNO_NAME = "替代精密電阻",
				ROUTE_VER_OPER_SID = mainPart.ROUTE_VER_OPER_SID,
				ROUTE_VER_SID = testRouteVerSid,
				PARTNO = testProductPartNo,
				PARTNO_SID = "TEST_PRODUCT_SID_001",
				PARTNO_NAME = "測試產品",
				CONFIG_FLAG = "P",
				CREATE_USER = "UnitTest",
				CREATE_DATE = DateTime.Now,
				UPDATE_USER = "UnitTest",
				UPDATE_DATE = DateTime.Now
			});

			db.SaveChanges();

			// 2. Act: 執行受測邏輯
			var resultList = Genesis.Library.BLL.MES.OperTask.Custom.ZAC_LotInfoDecorator.GetOperMaterials(testRouteVerSid, testProductPartNo);

			// 3. Assert: 驗證輸出結果與結構
			Assert.IsNotNull(resultList);
			// 預期回傳 2 筆 (1 筆主料 + 1 筆替代料)
			Assert.AreEqual(2, resultList.Count);

			var mainDto = resultList.FirstOrDefault(x => x.IS_MAIN);
			var subDto = resultList.FirstOrDefault(x => !x.IS_MAIN);

			Assert.IsNotNull(mainDto);
			Assert.AreEqual("MAIN_RESISTOR", mainDto.PARTNO); // 驗證是否已非產品料號
			Assert.AreEqual("精密電阻", mainDto.PART_NAME);

			Assert.IsNotNull(subDto);
			Assert.AreEqual("SUB_RESISTOR", subDto.PARTNO);
			Assert.AreEqual(mainDto.GROUP_ID, subDto.GROUP_ID); // 驗證是否正確群組化

		}, isTransMode: true, isTest: true); // 強制回滾
		 
	}
}

