using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using UnitTestProject.TestUT;
//using _bllSvc = Genesis.Library.BLL.ZZ_ZAC;

namespace UnitTestProject
{
    public partial class t_ZAC : _testBase
	{
		[TestMethod]
		public void t_20260525_GetOperMaterials_含主要物料與替代物料()
		=> _DBTest((txn) =>
		{
			var db = txn.EFQuery_MES;
			var testRouteVerSid = "GTI26012313075963710";
			var testProductPartNo = "MTR_100001";
			var _lot = txn.GetLotInfo("DevTest_20260114-03", isQueryByLotNO: true);

			// 2. Act: 執行受測邏輯
			var resultList = Genesis.Library.BLL.MES.OperTask.Custom.ZAC_LotInfoDecorator.GetOperMaterials(_lot, txn);
			FileApp._tmpJson(resultList);

		}, isTransMode: true, isTest: true); // 強制回滾
 

	}
}
