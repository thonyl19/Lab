using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.T4.Htm;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	[TestClass]
	public class t_T4 : _testBase
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
		public void t_() {
			var data = new MyData() { Items = new List<MyDataItem>() { 
				new MyDataItem("Name","Value") 
			} };
			var page = new MyWebPage1(data);
			string pageContent = page.TransformText();
			System.IO.File.WriteAllText(FileApp.ts_Log("tt.html") , pageContent);
		}
		

	}


}
