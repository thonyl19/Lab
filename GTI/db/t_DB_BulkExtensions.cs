using BLL.MES;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using EntityFramework.BulkExtensions;
using EntityFramework.BulkExtensions.Operations;

namespace UnitTestProject
{
	[TestClass]
	public class t_DB_BulkExtensions : _testBase
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
		/*
		Ef6.BulkExtensions
		https://github.com/mamift/BulkExtensions


		** 以下有試用期限的問題
		https://github.com/zzzprojects/EntityFramework-Plus?tab=readme-ov-file
		NuGet\Install-Package Z.EntityFramework.Plus.EF6 -Version 8.102.2.4


		https://www.nuget.org/packages/Z.EntityFramework.Extensions/8.102.2.3?_src=template
		NuGet\Install-Package Z.EntityFramework.Extensions -Version 8.102.2.3
		 */



		[TestMethod]
        public void t_批次新增()
        => _DBTest((txn) =>
        {
			//var r = new AD_AREA();
			List<AD_AREA> entities = new List<AD_AREA>();
			txn.EFQuery_MES.BulkInsert(entities);
			txn.EFQuery_MES.BulkDelete(entities);
			//txn.DapperQuery<FC_CARRIER>("SELECT * from FC_CARRIER WHERE STATE_NO = 'Idle'")
			//             .FirstOrDefault();
			//         var CarrierInfo = txn.GetCarrierInfo(_d.CARRIER_NO);
			//         txn.DoTransaction(new DTC_Carrierload(CarrierInfo));
		}, true, true);


    }


}
