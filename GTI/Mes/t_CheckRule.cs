using Genesis.Library.BLL.ICM.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.ICM.Maintain;
using Dapper;
using BLL.DataViews.Edc;
using BLL.MES.DataViews;
using Genesis.Library.BLL.MES.OperTask;
using System;

namespace UnitTestProject
{
	[TestClass]
	public class t_CheckRule : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_ParallelCheckOut
			{
				get
				{
					return FileApp.ts_Log(@"MesInOut\t_ParallelCheckOut.json");
				}
			}
		}


		[TestMethod]
		public void t_良品數量()
		{ 
			var r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_ParallelCheckOut);
			try
			{
				r.GoodList = null;
				CheckRule.良品數量(r);
			}
			catch (Exception Ex) {
				Assert.AreEqual(Ex.Message, "良品資訊不得為空");
			}
			r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_ParallelCheckOut);
			try
			{
				r.GoodList[0].Qty = 0;
				r.ScrapList = null;
				CheckRule.良品數量(r);
			}
			catch (Exception Ex)
			{
				Assert.AreEqual(Ex.Message, "良品數為零,報廢資訊不得為空");
			}

			r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_ParallelCheckOut);
			try
			{
				r.GoodList[0].Qty = 0;
				var x = new Frame.Code.Web.Select.CustomerList() { Qty = 0 };
				r.ScrapList = new List<Frame.Code.Web.Select.CustomerList>() { x } ;
				CheckRule.良品數量(r);
			}
			catch (Exception Ex)
			{
				Assert.AreEqual(Ex.Message, "良品數+報廢數不得為零");
			}
		}
		

	}
}
