using BLL.MES;
using Genesis.Gtimes.ADM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject.TestUT;
using zz_API = Genesis.Library.BLL.ZZ.SJ.ApiService;
namespace UnitTestProject
{
    /// <summary>
    /// 台英帝國
    /// </summary>
    [TestClass]
	public class t_SJ : _testBase
	{
		static class _log
		{
			internal static string t_User_JobReport
            {
				get
				{
					
					return FileApp.ts_Log(@"ZZ\sj\t_User_JobReport.json");
				}
			}

            internal static string process_治具扣數
            {
                get
                {
                    return FileApp.ts_Log(@"ZZ/SJ\process_治具扣數.json");
                }
            }
        }


        [TestMethod]
        public void t_fn()
        => _DBTest((txn) =>
        {
            var query = txn.EFQuery_MES.WP_USER_TRACE_IN
                .Where(x=>x.LOT == "100031179")
                .GroupBy(x => new { x.LOT, x.USER_NO })
                .Select(g => new
                {
                    LOT = g.Key.LOT,
                    USER_NO = g.Key.USER_NO,
                    //LatestStartTime = g.OrderByDescending(x => x.START_TIME).FirstOrDefault().START_TIME,
                    LatestRec = g.OrderByDescending(x => x.START_TIME).FirstOrDefault(),
                    Count = g.Count()
                }).ToList();
        }, false);

        [TestMethod]
        public void t_User_CheckIn(){
            var r = zz_API.User_CheckIn("Jordan", "TWO-240725A-04",null, true);
        }


        [TestMethod]
        public void t_User_JobReport()
        => _DBTest((txn) =>{
            var _r = FileApp.Read_SerializeJson<zz_API.d_人員報工>(_log.t_User_JobReport);
            zz_API.User_JobReport(txn, _r);
        }, true, true);


        [TestMethod]
        public void t_ddl_Parameter()
        => _DBTest((txn) =>
        {
            var t2 = Genesis.Library.BLL.ZZ.SJ.CaseRule.PartNo.Check_高週波料號(txn, "45");
        }, false, true);


        [TestMethod]
        public void t_CancelCheckIn_LotInfo_檢核支援人員與設備()
        => _DBTest((txn) =>
        {
            LOT_Services.CancelCheckIn_LotInfo_檢核支援人員與設備(txn, "TWO-240725A-06");
        }, false, true);



        public class d_治具扣數
        {
            public dynamic EqpInfo;
            public bool is機台有治具;
            public List<dynamic> _機台上的治具;
        }

        [TestMethod]
        public void SJ_高週波站()
        => _DBTest(Txn =>
        {
            var WOinfo = Txn.GetWOInfo("TestFurance2025021801-Test");
            Genesis.Library.BLL.ZZ.SJ.CaseRule.WO.高週波站設定(Txn, WOinfo, null);

        });


        [TestMethod]
        public void t_編碼原則問題()
        => _DBTest((Txn) =>
        {
            var CODE_NO = "WMS_BILL_NO_LOCATION_TRANSFER";
            var codeInfo = Txn.GetEnCodeInfo(CODE_NO);

            if (!codeInfo.IsExist) throw new Exception(string.Format(RES.BLL.Message.SystemParamaterIsNull, CODE_NO));

            var args = new Dictionary<EncodeFormatUtility.ParameterType, object>
            {
                { EncodeFormatUtility.ParameterType.BASE_DATE, Txn.DBC.GetDBTime() }
            };
            var codes = EncodeFormatUtility.Coder.GetCodes(Txn.DBC, Txn.UserNo, codeInfo, 1, args, false);
            Txn.DoTransaction(codes.Commands);
        }, true);


        [TestMethod]
        public void t_f_依據流程版本取得Judge可選用的站點()
        => _DBTest((Txn) =>{
            var r1 = Txn.EFQuery_MES.f_依據流程版本取得Judge可選用的站點("GTI25031815581792003").ToList();
            var r2 = r1.Select(c => new  {
                    SID = c.OPER_SID,
                    No = c.OPERATION_NO,
                    Display = c.OPERATION,
                    Value = c.OPER_SID,
                    Attr01 = c.ROUTE_VER_OPER_SID,
                    INum = 0,
                    Status = c.JUDGE_VALUE,
                }).ToList();
            FileApp._tmpJson(new { r1,r2});
        }, true);


        
        [TestMethod]
        public void t_f_查詢下一站()
        => _DBTest((Txn) => {
            var r1 = Txn.EFQuery_MES.f_查詢下一站("T01", "GTI25042511513938127").ToList();
            var r2 = Txn.EFQuery_MES.f_查詢下一站("T01", "GTI25042511513938127", true).ToList();
            FileApp._tmpJson(new { r1,r2});
        }, true);
    }
}

