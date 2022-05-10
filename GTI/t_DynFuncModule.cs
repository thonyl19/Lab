using BLL.MES;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	[TestClass]
	public class t_DynFuncModule : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_OperationData
			{
				get
				{
					return FileApp.ts_Log(@"DynFuncModule\t_OperationData.json");
				}
			}
		}


		[TestMethod]
		public void t_SQL_查詢範例()
		{
			var _r = FileApp.Read_SerializeJson<PF_OPERATION>(_log.t_OperationData);

			var svc = new DynFuncModuleServices();

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
