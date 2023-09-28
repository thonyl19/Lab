using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	[TestClass]
	public class t_Linq : _testBase
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


		[TestMethod]
		public void t_將文字型數字做排序()
		{
			//請教一下 以下程式語法的功能 是? 
			var Cq_items = new string[] { "1", "11", "2" };
			var z = Cq_items
				.OrderBy(x =>
				{
					return x.Length;
				})
				.ThenBy(x => x).ToList();
		}

	}


}
