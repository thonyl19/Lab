
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Frame.Code.Web.Select;
using Genesis.Library.BLL.MES.OperTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.WIP.LotUtility;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;

namespace UnitTestProject
{
	[TestClass]
	public class t_Fun : _testBase
	{
		static class _log
		{
			/// <summary>
			///	因應 漢民專案,處理 載具進站 by Wefer 收 EDC 的樣例 
			/// </summary>
			internal static string t_StationCheckIn(string ext = "")
			{
				return FileApp.ts_Log($@"MES\t_StationCheckIn{ext}.json");
			}
		}


        #region [ Sample ] 
        /*
		[TestMethod]
		public void _Sample()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo("GTI22060711213497371");
			var c = lot.GetCurrentCarrierInfo();
			var exp = c.CURRENT_CAPACITY - 1;
			Txn.DoTransaction(new Carrier.DTC_AdjustmentCapacity(lot, c, -1));
			var act = lot.GetCurrentCarrierInfo().CURRENT_CAPACITY;
			Assert.AreEqual(exp, act,$"扣數後, 數值應為 {exp}");

			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));
			new FileApp().Write_SerializeJson(dt, _log.t_splitBIN);

			var _r = new FileApp().Read_SerializeJson(_log.t_splitBIN);
		}, true);
		*/
        #endregion

        [TestMethod]
        public void t_物料批檢核序()
        => _DBTest((txn) =>
        {
            var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_StationCheckIn("~20230317"));
            Genesis.Library.BLL.MES.OperTask.Check.物料批檢核(txn, obj);
        }, true);

		[TestMethod]
        public void t_ExtenParam()
        => _DBTest((txn) =>
		{
			var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_StationCheckIn("~20230317"));
			//obj.ExtenParam.Add("AAA", (new { AAA="test"}).ToJson());
			FileApp._tmpJson(obj);
		}, true);


		[TestMethod]
		public void t_LOT_CHANGE_BATCH()
		=> _DBTest((txn) =>
		{
			//var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_StationCheckIn("~20230317"));
			var lot = txn.GetLotInfo("GTI24022611472828753");
			Genesis.Library.BLL.MES.OperTask.Func.LOT_CHANGE_BATCH(txn, lot, "Test");

		},true, true);



		[TestMethod]
		public void t_SplitLots()
		=> _DBTest((txn) =>
		{
			//var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_StationCheckIn("~20230317"));
			var lot = txn.GetLotInfo("GTI24022611472828753",isKeep:true);
			var SplitList = new List<CustomerList>() { new CustomerList() { INum = 10 } }; ;

			//var g = txn.GetEnCodes("BagNo", 1, lot, false);
		//var EnInfo = new EncodeFormatUtility.EncodeFormatInfo(DBC, "BagNo", EncodeFormatUtility.IndexType.No);
		//var SplitInfos = new List<LotUtility.LotSplitInfo>();
		//var CommandTxn = new TransactionUtility.AddSQLCommandTxn();
		//if (EnInfo.IsExist == true)
		//{
		//	EncodeFormatUtility.CodesInfo codes = EncodeFormatUtility.Coder.GetCodes(DBC, "Admin", EnInfo,
		//		SplitList.Count, new Dictionary<EncodeFormatUtility.ParameterType, object>
		//		{
		//				{ EncodeFormatUtility.ParameterType.LOT, _lotInfo.LOT },
		//				{ EncodeFormatUtility.ParameterType.WO, _lotInfo.WO },
		//				{ EncodeFormatUtility.ParameterType.PARTNO, _lotInfo.PARTNO }
		//		}, false);

			List<LotSplitInfo> SplitInfoList = _Func.SplitLots(txn, SplitList, "BagNo");
			var _newLot = txn.GetLotInfo(SplitInfoList[0].LOT_SID);


		}, true, true);

		[TestMethod]
		public void t_SplitLots_人工給號()
		=> _DBTest((txn) =>
		{
			//var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_StationCheckIn("~20230317"));
			var lot = txn.GetLotInfo("GTI24022611472828753", isKeep: true);
			var SplitList = new List<CustomerList>() { new CustomerList() { INum = 10 ,No = "test"} }; ;
 
			List<LotSplitInfo> SplitInfoList = _Func.SplitLots_人工給號(txn, SplitList);
			var _newLot = txn.GetLotInfo(SplitInfoList[0].LOT_SID);
		}, true, true);
		
	}
}
