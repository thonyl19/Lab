using Genesis.Library.BLL.ICM.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.ICM.Maintain;
using Dapper;
using BLL.DataViews.Edc;
using System;
using System.Linq;
using Genesis.Library.BLL.PMS.Definition;
using BLL.PMS;

namespace UnitTestProject
{
	[TestClass]
	public class t_PMS : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_splitBIN
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_splitBIN.json");
				}
			}
		}
        

        // =====================================================
        // 1. 滾算日期結果 DTO
        // =====================================================
        public class RollingDateResult
        {
            public DateTime PlanDate { get; set; }
            public DateTime PrePlanDate { get; set; }
            public DateTime RollMonth { get; set; }
            public int offset { get; set; }
            public DateTime RealExecData { 
                get {
                    return PlanDate.AddDays(offset);
                } 
            }
        }


        [TestMethod]
        public void t_fn()
        => _DBTest((txn) =>
        {
            string Calculation = "Plan";
            DateTime dbNow = txn.ExeTime;
            DateTime dbToday = new DateTime(dbNow.Year, dbNow.Month, dbNow.Day);
            DateTime firstPlanDate = dbToday;
            DateTime rollMonth = dbToday;
            DateTime EndRollDate = rollMonth.AddMonths(1);

            string HolidaySet = txn.EFQuery_MES.PM_PARAMETER
                .Where(x => x.CALCULATION == Calculation)
                .FirstOrDefault()
                .HOLIDAY_SET;

            // 載入假日清單
            List<DateTime> holidays = txn.EFQuery_MES.AD_CALENDAR_ITEM
                .Where(x => x.CALENDAR_SID.Equals("PMSHoliday")
                         && x.CALENDAR_DATE >= rollMonth
                         && x.CALENDAR_DATE < EndRollDate)
                .Select(x => x.CALENDAR_DATE)
                .ToList();

            var main = txn.EFQuery_MES.PM_PLAN
                .Where(c=>c.PLAN_NO =="Yearly MA-02")
                .FirstOrDefault();
            //var r = PMSPlanCreateServices.CalculateRollingDates(main, rollMonth, firstPlanDate, dbNow, HolidaySet, holidays);
            //FileApp._tmpJson(r);

        }, true);



    }
}
