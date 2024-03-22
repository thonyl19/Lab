
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Frame.Code.Web.Select;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.MES.OperTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using static BLL.MES.LOT_Services;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.WIP.LotUtility;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;
using _check = BLL.InterFace.Check;

namespace UnitTestProject
{
	[TestClass]
	public class t_分批 : _testBase
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

		[TestMethod]
		public void t_分批0() => _DBTest((txn) =>
        {
            var DBC = txn.DBC;
            var _lotInfo = txn.LotInfo;
            var gtimesTxn = txn.GtimesTxn;

            string CodeNo;
            List<CustomerList> SplitList;
            EncodeFormatUtility.CodesInfo CodesInfo;

            //todo
            //var _子批設定 = 產生子批設定(txn, _lotInfo);

            //var CommandTxn = new TransactionUtility.AddSQLCommandTxn();
            //CommandTxn.Commands.AddRange(CodesInfo.Commands);
            //var SplitInfos = 產生子批清單(DBC, _子批設定);

        }, true, true);


        /// <summary>
        /// todo
        /// </summary>
        /// <param name="txn"></param>
        /// <param name="_lotInfo"></param>
        /// <param name="CodeNo"></param>
        /// <param name="SplitList"></param>
        /// <returns></returns>
        private static EncodeFormatUtility.CodesInfo 產生子批設定(ITxnBase txn, LotInfo _lotInfo, string CodeNo = null, List<CustomerList> SplitList = null)
        {
            CodeNo = CodeNo ?? FunctionName.LotSplit;
            SplitList = SplitList ?? new List<CustomerList>() { new CustomerList() { INum = 1 } };

            switch (CodeNo) {
                case FunctionName.人工給號:
                    _check.Invalid("人工給號 ,分批清單必須設定" , SplitList == null, SplitList);
                    break;
                default:
                    break;
            }

            return txn.GetEnCodes(CodeNo, SplitList.Count, _lotInfo, false);
        }
        private static List<LotUtility.LotSplitInfo> 產生子批設定_人工給號(ITxnBase txn, List<CustomerList> SplitList)
        {
            _check.Invalid("人工給號 ,分批清單必須設定", SplitList == null, SplitList);
            var SplitInfos = new List<LotUtility.LotSplitInfo>();
            var i = 0;
            foreach (var item in SplitList) {
                _check.Invalid("人工給號, No 欄位必須有值", string.IsNullOrWhiteSpace(item.No), item);
                var _Lot = txn.GetLotInfo(item.No, isQueryByLotNO: true);
                _check.Invalid($"人工給號-{item.No} ,己存在", _Lot.IsExist, item);
                var splitLot = new LotUtility.LotSplitInfo(txn.GetSID(), item.No, item.INum);
                SplitInfos.Add(splitLot);
                i++;
            }
            return SplitInfos;
        }

        private static List<LotSplitInfo> 產生子批清單(Genesis.Gtimes.Common.DBController DBC, List<CustomerList> SplitSetList)
        {
            var SplitInfos = new List<LotUtility.LotSplitInfo>();
            var i = 0;
            foreach (var item in SplitSetList){
                var splitLot = new LotUtility.LotSplitInfo(DBC.GetSID(), item.No, item.INum);
                SplitInfos.Add(splitLot);
                i++;
            }
            return SplitInfos;
        }

        [TestMethod]
		public void t_分批1()
		=> _DBTest((txn) =>
		{
			//var obj = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_StationCheckIn("~20230317"));
			var lot = txn.GetLotInfo("GTI24022611472828753");
			Genesis.Library.BLL.MES.OperTask.Func.LOT_CHANGE_BATCH(txn, lot, "Test");

		},true, true);


		 
		
	}
}
