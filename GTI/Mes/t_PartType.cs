using Genesis.Gtimes.ADM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	[TestClass]
	public class t_PartType : _testBase
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
			PartUtility.PartTypeFunction partType = new PartUtility.PartTypeFunction(this.DBC);
			var dt  = partType.GetpartType("F");
		}


	}


}
