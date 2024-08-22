using Genesis.Library.BLL.ICM.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.ICM.Maintain;
using Dapper;
using BLL.DataViews.Edc;

namespace UnitTestProject
{
	[TestClass]
	public class t_Task : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_splitBIN
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_splitBIN.json");
				}
			}
		}


		public void 任務X(d_data data) { 
			//todo
		}

		public class d_data { 
			public string test { get; set; }
		}

        [TestMethod]
        public void t_fn()
		=> _DBTest((txn) =>
		{
			var _data = new d_data() { test = "A" };
			//var _d = TASK_Services.Init(txn,nameof(任務X), _data, 5);
		},true, true);

	}
}
