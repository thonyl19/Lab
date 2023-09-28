
using BLL.MES.DataViews;
using Frame.Code;
using Genesis.Library.BLL.MES.OperTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

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
    }
}
