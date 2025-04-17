using BLL.MES;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using QueryableQMS = Genesis.Library.BLL.LzQuery.QMS;

namespace UnitTestProject
{
	[TestClass]
	public class t_QMS : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_by料號查詢特定檢驗單
			{
				get
				{
					return FileApp.ts_Log(@"QMS\t_by料號查詢特定檢驗單.json");
				}
			}

			internal static string t_by檢驗單號取得EDC資料_轉換為前端格式
			{
				get
				{
					return FileApp.ts_Log(@"QMS\t_by檢驗單號取得EDC資料_轉換為前端格式.json");
				}
			}
			
		}



        [TestMethod]
        public void t_by料號查詢特定檢驗單()
		=> _DBTest((txn) =>
		{
			List<QC_INSP> _t =  FileApp.Read_SerializeJson<List<QC_INSP>>(_log.t_by料號查詢特定檢驗單);

			var _d = TxnBase.isDBTest(_t, QueryableQMS.by料號查詢特定檢驗單(txn, "120001","FIPQC")).ToList();
			FileApp.WriteSerializeJson(_d, _log.t_by料號查詢特定檢驗單);
		}, false);

		[TestMethod]
        public void t_by檢驗單號取得EDC資料_轉換為前端格式()
		=> _DBTest((txn) =>
		{
			//var z = txn.QueryableQMS().by檢驗單號取得EDC資料_轉換為前端格式("SIPQC_02",5);
			//FileApp.WriteSerializeJson(z, _log.t_by檢驗單號取得EDC資料_轉換為前端格式);
		}, false);

		[TestMethod]
		public void _查詢檢驗單()
		=> _DBTest((txn) =>
		{
			var _QMS = txn.QueryableQMS();
			var _src = txn.EFQuery_MES.QC_INSP.FirstOrDefault();
			var _反向條件 = _src.ENABLE_FLAG == "T"?"F":"T";
			var 測試正向 = _QMS.查詢檢驗單(_src.INSP_NO,ENABLE_FLAG: _src.ENABLE_FLAG).FirstOrDefault();
			Assert.IsTrue(測試正向!=null, $"測試正向 - 應該要有值");

			var 測試反向 = _QMS.查詢檢驗單(_src.INSP_NO, ENABLE_FLAG: _反向條件).FirstOrDefault();
			Assert.IsTrue(測試反向 == null, $"測試反向 - 應該要沒有值");

		}, false);


		[TestMethod]
		public void t_QCResult_linq()
		{
			//var r = QMSService.QCResult_linq("INSP24100603", "SIPQC");
		}
		
		


	}


}
