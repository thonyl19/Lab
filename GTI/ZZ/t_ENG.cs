using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Genesis.Gtimes.ADM;
using Genesis.Library.BLL.DTC;
using Genesis.Library.BLL.MES.OperTask;
using Genesis.Library.BLL.ZZ.LIO;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.ZZ.ENG.Maintain;



namespace UnitTestProject
{
	/// <summary>
    /// 台英帝國
    /// </summary>
    [TestClass]
	public class t_ENG : _testBase
	{
		static class _log
		{
			internal static string ZZ_MAKER_ENG_DATA
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\ENG\ZZ_MAKER_ENG_DATA.json");
				}
			}

		}


		[TestMethod]
		public void t_ZZ_MAKER_ENG_DATA()
		{
			var _r = FileApp.Read_SerializeJson<ZZ_MAKER_ENG_DATA>(_log.ZZ_MAKER_ENG_DATA);
			Maintain.ZZ_MAKER_ENG_DATA_ITEM_Save(_r, true);
		}
	}
}

