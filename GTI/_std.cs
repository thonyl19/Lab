using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	[TestClass]
	public class t_STD : _testBase
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
		public void t_SQL_查詢範例()
		{

			using (var dbc = this.DBC)
			{
				var _sql = $@"
                SELECT  top 1 *
                FROM    PF_PARTNO_VER

                         ";

				var dt = dbc.Select(_sql);

				new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}
		[TestMethod]
		public void t_() { }


	}


}
