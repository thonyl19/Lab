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
using System.Data.Entity;
using System.Linq;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
    [TestClass]
	public class t_LIO : _testBase
	{
		static class _log
		{
			/// <summary>
			/// 雷切出站
			/// </summary>
			internal static string LaserCuttingCheckOut
			{
				get
				{
					return FileApp.ts_Log(@"LIO\LaserCuttingCheckOut.json");
				}
			}
			internal static string BB_Tesing
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\LIO\BB_Tesing.json");
				}
			}
		}

 

		[TestMethod]
		public void _Sample()
		=> _DBTest(Txn => {
			//Txn.DoTransaction(new Wafer.Rec_NormalWafer_When_CheckOut("JK_WO_001-04.01.02"));
		}, true);


		[TestMethod]
		public void t_LaserCuttingCheckOut()
		{
			var _r = new FileApp().Read_SerializeJson<WIPFormSendParameter>(_log.LaserCuttingCheckOut);
			new LIO_LaserCuttingCheckOut().Process(_r, true);
		}


		/// <summary>
		/// 編碼處理原型 
		/// </summary>
		[TestMethod]
		public void t_SplitModuleLot_簡易版()
		=> _DBTest(Txn => {
			
				var EnInfo = new EncodeFormatUtility.EncodeFormatInfo
					(Txn.DBC, "SplitModuleLot"
					, EncodeFormatUtility.IndexType.No);
			
				var LotNo = "LWO23010901-07";
				var lot = Txn.GetLotInfo(LotNo, isQueryByLotNO: true);
				var qty = (int)lot.QUANTITY;
				var _startIdx = LotNo.Right(2).ToInt()-1;
				_startIdx = (qty * _startIdx) ;

				var _rpo = Txn.EFQuery<AD_ENCODE_FORMAT_ITEM>()
					.Reads(c => c.ENCODE_FORMAT_SID == EnInfo.ENCODE_FORMAT_SID)
					.OrderByDescending(c=>c.ITEM_SEQ)
					.ToList();
				var rule = _rpo[0];

				if (rule.ITEM_TYPE != "SERIAL") Result.Invalid("編碼規則最後一項必須為 SERIAL", rule).ThrowException();
				 
				var _SN_len = (int)rule.ITEM_VALUE_LENGTH;
				var Code = EncodeFormatUtility.Coder.GetCodes
					(Txn.DBC
					, "AdminTest"
					, EnInfo
					, 1
					, new Dictionary<EncodeFormatUtility.ParameterType, object>() {
						{ EncodeFormatUtility.ParameterType.WO, lot.WO },
					}
					//是否自動 commit , T)則會直接把 code 寫入 AD_ENCODE_FORMAT_CONTROL
					, false);
			if (Code.Codes.Count == 0) Result.Invalid("編碼規則取號結果為0筆", Code).ThrowException();

			var _root = Code.Codes[0].Substring(0, Code.Codes[0].Length - _SN_len);
			var list = new List<string>();
			for (int i = 1; i < lot.QUANTITY+1 ; i++) {
				list.Add($"{_root}{(i + _startIdx).ToString().PadLeft(_SN_len,'0')}");
			}

			Txn.result.Data = new { list };


		}, true);

		[TestMethod]
		public void t_SplitModuleLot()
		{
			var LotNo = "LWO23010901-01";
			var r = new ApiService().SplitModuleLot(LotNo);
		}

		[TestMethod]
		public void t_Case()
		=> _DBTest(Txn => {
			var _src = FileApp.Read_SerializeJson<ZZ_BB_TESTING_HIST>(_log.BB_Tesing);
			var _repo = Txn.EFQuery<ZZ_BB_TESTING_HIST>();

			var _item_0 = _repo.Reads
				(c => c.SID == _src.SID && c.STATE == "Pass")
				.FirstOrDefault();

			var _item_1 = _repo.Reads
				(c => c.SID == _src.SID
					&& c.OPERATION == _src.OPERATION
					&& c.CREATE_DATE == _src.CREATE_DATE
					&& c.STATE == "Pass")
				.FirstOrDefault();

			var _item_2 = _repo.Reads
				(c => c.SN == _src.SN
					&& c.OPERATION == _src.OPERATION
					&& DbFunctions.TruncateTime(c.CREATE_DATE) == DbFunctions.TruncateTime(_src.CREATE_DATE)
					&& c.STATE == "Pass")
				.FirstOrDefault();

			_item_0.ACTION_LINK_SID = Txn.LinkSID;
			_repo.Update(_item_0);
			_repo.SaveChanges();



		}, true);

		[TestMethod]
		public void t_ZZ_BB_TESTING_HIST() {
			var z = ApiService.ZZ_BB_TESTING_HIST("WM3-N5V001-000110","GTI23052510515961128");
		}

		[TestMethod]
		public void t_StationEQP()
		{
			var NEXT_OPER_SID = "GTI23010917340392788";
			var r = WIPOperConfigServices.StationEQP(NEXT_OPER_SID);
		}
	}
}

