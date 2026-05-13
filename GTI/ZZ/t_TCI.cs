using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
    /// <summary>
    /// 台英帝國
    /// </summary>
    [TestClass]
	public class t_TCI : _testBase
	{
		static class _log
		{
			internal static string list_WP_WO_MTL_BOM
			{
				get
				{
					
					return FileApp.ts_Log(@"ZZ\TCI\WP_WO_MTL_BOM.json");
				}
			}
 

		}


	}
}

