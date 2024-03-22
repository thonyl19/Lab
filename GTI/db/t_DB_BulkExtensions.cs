using BLL.MES;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

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


		//[TestMethod]
		//public void t_批次新增()
  //      => _DBTest((txn) =>
  //      {
  //          var _d = txn.DapperQuery<FC_CARRIER>("SELECT * from FC_CARRIER WHERE STATE_NO = 'Idle'")
  //              .FirstOrDefault();
  //          var CarrierInfo = txn.GetCarrierInfo(_d.CARRIER_NO);
  //          txn.DoTransaction(new DTC_Carrierload(CarrierInfo));
  //      }, true);


	}


}
