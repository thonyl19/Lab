using BLL.MES;
using Genesis.Gtimes.MTR;
using Genesis.Gtimes.Transaction.MTR;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.MES.WRP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.Transaction.MTR.MTRTransaction;
using _Frame = Genesis.Library.Frame.Code.Web.TableQuery;

namespace UnitTestProject
{

    [TestClass]
	public class t_MTR : _testBase
	{
		public static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_GetMTLotOnEqpOfLot1
			{
				get
				{
					return FileApp.ts_Log(@"Lot\t_GetMTLotOnEqpOfLot.json");
				}
			}

			/// <summary>
			/// Gets the t_Search_QCResult_Query.
			/// </summary>
			internal static string t_Search_QCResult_Query
			{
				get
				{
					return FileApp.ts_Log(@"MTR\t_Search_QCResult_Query.json");
				}
			}

		}

		[TestMethod]
		public void t_LotConsumptionTxn()
		=> _DBTest((Txn) => {
			var CurrentLot = Txn.GetLotInfo("3B0000-231213-01", isQueryByLotNO: true);
			var mLot = Txn.GetMLotInfo("2001-15409-1-1B01");
			var consumpMLot = new LotUtility.LotConsumptionMlotQuantity(mLot, (decimal)50, 0, 0);
			//Txn.DoTransaction(new WIPTransaction.LotConsumptionTxn(CurrentLot, consumpMLot));
		}, true);


		/// <summary>
		/// 變更物料批號數量
		/// </summary>
		[TestMethod]
        public void t_ChangeMtrLotQtyTxn()
		  => _DBTest((Txn) =>
		  {
			  //var CurrentLot = Txn.GetLotInfo("3B0000-231213-01", isQueryByLotNO: true);
			  var mLot = Txn.GetMLotInfo("15127-1-1C01-00001");
			 // Txn.Ac

				//var txnBase = new TransactionUtility.TransactionBase
				//		(DBC.GetSID()
				//		, "TEST"
				//		, DBC.GetDBTime()
				//		, "PackMerge");
			  //var gtimesTxn = new TransactionUtility.GtimesTxn(DBC, txnBase);
			  Txn.DoTransaction(new ChangeMtrLotQtyTxn(mLot, 50, null, null));
		  }, true,true);

        /// <summary>
        /// The t_insp_wo_PagerQuery.
        /// </summary>
        [TestMethod]
		public void t_MTRListServices_DoQuery()
		{
			var _r = FileApp.Read_SerializeJson<_Frame.PagerQuery>(_log.t_Search_QCResult_Query);
			var z = new MTRListServices().DoQuery(_r);
		}


	}
}
