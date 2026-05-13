using BLL.Base;
using BLL.Base.Wrapper;
using BLL.InterFace;
using Frame.Code;
using Genesis.Library.BLL.AMS;
using Genesis.Library.BLL.MES.AutoGenerate;
using Genesis.Library.BLL.PMS.Definition;
using Genesis.Library.Frame.Code.Web.TableQuery;
using MDL;
using MDL.MES;
using Newtonsoft.Json;
using RES.BLL;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BLL.MES.WIPInjectServices;

namespace BLL.PMS
{
    public partial class PMSPlanCreateServices : ServicesBase
    {
        /*
        這個版本 ,主要的目的是讓計劃可以先跑出日期清單,再依據日期清單來跑計劃,
        但因為執行測試上仍有一些問題 ,故先保留 ,日後有空再改
        */
        public IResult RollingPlanV2(PM_PLAN model, DateTime[] rollMonthPeriod, DateTime firstPlanDate)
        {
            try
            {
                //pm_plan checking
                List<dynamic> objectList = GetPMPlanObjectList(model.PLAN_SID);
                List<PM_ITEM> itemList = GetPMPlanItemList(model.PLAN_SID);
                List<AD_USER> userList = GetPMPlanUserList(model.PLAN_SID);

                int beforRollCnt = BasePmPlanimplementService.GetAllListIQueryable().Count();
                string msgMonthPeriod = rollMonthPeriod[0].ToString("yyyy-MM") + ((rollMonthPeriod[0] == rollMonthPeriod[1]) ? "" : (" ~ " + rollMonthPeriod[1].ToString("yyyy-MM")));
                DateTime EndRollDate = rollMonthPeriod[1];
                DateTime dbNow = NUnitServiceBase.GetSysDBTime();
                //run all period
                do
                {
                    if (firstPlanDate.ToString("yyyyMM").Equals(rollMonthPeriod[0].ToString("yyyyMM")))
                    {
                        DateTime rollMonth = firstPlanDate;
                        string HolidaySet = EFQuery_MESContext.PM_PARAMETER
                            .Where(x => x.CALCULATION == "Plan")
                            .FirstOrDefault()
                            .HOLIDAY_SET;

                        // 載入假日清單
                        List<DateTime> holidays = EFQuery_MESContext.AD_CALENDAR_ITEM
                            .Where(x => x.CALENDAR_SID.Equals("PMSHoliday")
                                     && x.CALENDAR_DATE >= rollMonth
                                     && x.CALENDAR_DATE < EndRollDate)
                            .Select(x => x.CALENDAR_DATE)
                            .ToList();
                        var r = CalculateRollingDates(model, rollMonth, firstPlanDate, dbNow, HolidaySet, holidays);
                        firstPlanDate = RollingPlanV2(model, r, objectList, itemList, userList);
                        //firstPlanDate = RollingPlan(model, rollMonthPeriod[0], firstPlanDate, objectList, itemList, userList);
                        //UOW.Save(); 改於RollingPlan內進行Save
                    }
                    else continue;
                } while (DateTime.Compare(rollMonthPeriod[1], (rollMonthPeriod[0] = rollMonthPeriod[0].AddMonths(1))) >= 0);

                int afterRollCnt = BasePmPlanimplementService.GetAllListIQueryable().Count();
                if (afterRollCnt > beforRollCnt)
                    return new Result(true, string.Format(Message.RollSuccessful, model.PLAN_NAME, msgMonthPeriod, (afterRollCnt - beforRollCnt)));
                else return new Result(true, Message.NORollData);

            }
            catch (Exception ex)
            {
                return new Result(ex.Message, ErrCode.False);
            }
        }


        public class RollingDateResult
        {
            public DateTime PlanDate { get; set; }
            public DateTime PrePlanDate { get; set; }
            public DateTime RollMonth { get; set; }
            public int offset { get; set; }
            public DateTime RealExecData
            {
                get
                {
                    return PlanDate.AddDays(offset);
                }
            }
            public DateTime NextPlanDate { get; set; }
        }
        private DateTime RollingPlanV2(PM_PLAN model, List<RollingDateResult> rollingDateList, List<dynamic> objectList, List<PM_ITEM> itemList, List<AD_USER> userList)
        {
            string loginUser = NUnitServiceBase.GetLoginUser().UserNo;
            DateTime dbNow = NUnitServiceBase.GetSysDBTime();
            DateTime dbToday = new DateTime(dbNow.Year, dbNow.Month, dbNow.Day);
            string Calculation = "Plan";
            string HolidaySet = BasePmParameterService.GetAllListIQueryable().Where(x => x.CALCULATION == Calculation).FirstOrDefault().HOLIDAY_SET;

            string sidPrefix = GetSidPrefix();
            string sMaxSid = BasePmPlanimplementService.GetAllListIQueryable()
                .Where(x => x.PLANIMPLEMENT_SID.StartsWith(sidPrefix))
                .Max(x => x.PLANIMPLEMENT_SID);
            if (sMaxSid != null)
                sMaxSid = sMaxSid.Substring(sidPrefix.Length);
            else sMaxSid = "0";
            int sidNum = int.Parse(sMaxSid);

            List<string> implSidList = new List<string>();
            HashSet<DateTime> keys = new HashSet<DateTime>();

            foreach (var rollItem in rollingDateList)
            {
                DateTime PlanDate = rollItem.PlanDate;
                DateTime PrePlanDate = rollItem.PrePlanDate;
                DateTime rollYearAndMonth = rollItem.RollMonth;
                DateTime firstPlanDate = rollItem.PrePlanDate;

                if (BasePmRollService.GetAllListIQueryable().Any(x => x.PLAN_SID == model.PLAN_SID && x.ROLL_DATE == rollYearAndMonth))
                    throw new Exception(string.Format(Message.RollMonthHave, rollYearAndMonth.ToString("yyyy-MM")));

                if (DateTime.Compare(PlanDate, dbToday) >= 0)
                {
                    foreach (dynamic objitem in objectList)
                    {
                        string itemsid = "";
                        string itemName = "";
                        if (MaintenanceObject.Equipment.ToString().Equals(model.ITEM_CLASS))
                        {
                            itemsid = ((FC_EQUIPMENT)objitem).EQP_SID;
                            itemName = ((FC_EQUIPMENT)objitem).EQP_NAME;
                        }
                        else if (MaintenanceObject.Carrier.ToString().Equals(model.ITEM_CLASS))
                        {
                            itemsid = ((FC_CARRIER)objitem).CARRIER_SID;
                            itemName = ((FC_CARRIER)objitem).CARRIER_NAME;
                        }
                        else if (MaintenanceObject.Tool.ToString().Equals(model.ITEM_CLASS))
                        {
                            itemsid = ((FC_TOOL)objitem).TOOL_SID;
                            itemName = ((FC_TOOL)objitem).TOOL_NAME;
                        }
                        else if (MaintenanceObject.Instrument.ToString().Equals(model.ITEM_CLASS))
                        {
                            itemsid = ((QC_INSTRUMENTS)objitem).INSTRUMENT_SID;
                            itemName = ((QC_INSTRUMENTS)objitem).INSTRUMENT_NAME;
                        }

                        var updateImplmentModel = BasePmPlanimplementService.GetAllListIQueryable()
                            .Where(x => x.PLANITEM_SID.Equals(itemsid) && x.PLAN_DATE == PlanDate && !x.STATUS.Equals(MaintenancePlanStatus.OK.ToString())).FirstOrDefault();

                        string implementSid;
                        var existUserSIDs = new List<string>();
                        if (updateImplmentModel == null)
                        {
                            sidNum++;
                            implementSid = sidPrefix + sidNum.ToString("00000000");
                            PM_PLANIMPLEMENT implementModel = new PM_PLANIMPLEMENT()
                            {
                                PLANIMPLEMENT_SID = implementSid,
                                PLAN_SID = model.PLAN_SID,
                                PARAMETER_SID = model.PARAMETER_SID,
                                PLAN_UNIT = model.PLAN_UNIT,
                                UNIT_NUMBER = model.UNIT_NUMBER,
                                START_DATE = model.START_DATE,
                                CANCEL_DATE = model.CANCEL_DATE,
                                HOLIDAY_SET = HolidaySet,
                                CALLOLATION = Calculation,
                                PLAN_DATE = PlanDate,
                                PRIPLAN_DATE = PrePlanDate,
                                UPDATE_DATE = dbNow,
                                UPDATE_USER = loginUser,
                                ROLL_DATE = rollYearAndMonth,
                                PSTARTDATE = firstPlanDate,
                                STATUS = MaintenancePlanStatus.WAIT.ToString(),
                                DESCRIPTION = model.DESCRIPTION,
                                PLANIMPLEMENT_NAME = model.PLAN_NAME,
                                CREATE_DATE = dbNow,
                                CREATE_USER = loginUser,
                                ITEM_CLASS = model.ITEM_CLASS,
                                PLANITEM_SID = itemsid
                            };
                            var tmpResult = BasePmPlanimplementService.InsertData(implementModel, false);
                            if (!tmpResult.Success) throw new Exception(tmpResult.Message);
                        }
                        else
                        {
                            implementSid = updateImplmentModel.PLANIMPLEMENT_SID;
                            var tmpResult = BasePmPlanimplementService.UpdateData(updateImplmentModel);
                            if (!tmpResult.Success) throw new Exception(tmpResult.Message);

                            existUserSIDs = BasePmPlanimplementuserService.GetAllListIQueryable()
                                .Where(x => x.PLANIMPLEMENT_SID.Equals(updateImplmentModel.PLANIMPLEMENT_SID))
                                .Select(x => x.USER_SID).ToList();
                        }

                        implSidList.Add(implementSid);

                        foreach (PM_ITEM item in itemList)
                        {
                            if (updateImplmentModel != null &&
                                BasePmPlanimplementitemService.GetAllListIQueryable()
                                .Any(x => x.PLANIMPLEMENT_SID.Equals(updateImplmentModel.PLANIMPLEMENT_SID) && x.ITEM_SID.Equals(item.ITEM_SID))) continue;

                            string mergeMsg = null;
                            if (updateImplmentModel != null)
                                mergeMsg = "+[" + model.PLAN_NAME + "] Item by Rolling @ " + dbNow.ToString("yyyy-MM-dd");
                            PM_PLANIMPLEMENTITEM implementItem = new PM_PLANIMPLEMENTITEM()
                            {
                                PLANIMPLEMENTITEM_SID = GetSID(GetCodeType.SID),
                                PLANIMPLEMENT_SID = implementSid,
                                ITEM_SID = item.ITEM_SID,
                                EDC_SID = item.EDC_SID,
                                EDC_VER_SID = item.EDC_VER_SID,
                                CREATE_DATE = dbNow,
                                CREATE_USER = loginUser,
                                UPDATE_DATE = dbNow,
                                UPDATE_USER = loginUser,
                                DESCRIPTION = mergeMsg
                            };

                            var tmpResult = BasePmPlanimplementitemService.InsertData(implementItem, false);
                            if (!tmpResult.Success) throw new Exception(tmpResult.Message);
                        }

                        foreach (AD_USER user in userList)
                        {
                            if (existUserSIDs.Contains(user.USER_SID)) continue;
                            PM_PLANIMPLEMENTUSER implementUser = new PM_PLANIMPLEMENTUSER()
                            {
                                PLANIMPLEMENTUSER_SID = GetSID(GetCodeType.SID),
                                USER_SID = user.USER_SID,
                                PLANIMPLEMENT_SID = implementSid,
                                CREATE_DATE = dbNow,
                                CREATE_USER = loginUser,
                                UPDATE_DATE = dbNow,
                                UPDATE_USER = loginUser
                            };

                            var tmpResult = BasePmPlanimplementuserService.InsertData(implementUser, false);
                            if (!tmpResult.Success) throw new Exception(tmpResult.Message);
                        }
                    }
                }

                if (sidNum > int.Parse(sMaxSid))
                {
                    PM_ROLL rollModel = new PM_ROLL()
                    {
                        ROLL_SID = GetSID(GetCodeType.SID),
                        FIRSTPLANDATE = rollItem.PrePlanDate,
                        PLAN_SID = model.PLAN_SID,
                        ROLL_DATE = rollItem.RollMonth,
                        PLANCREATE_NUM = (sidNum - int.Parse(sMaxSid)),
                        CREATE_DATE = dbNow,
                        CREATE_USER = loginUser
                    };

                    var tmpResult = BasePmRollService.InsertData(rollModel, false);
                    if (!tmpResult.Success) throw new Exception(tmpResult.Message);

                    bool isUpdate = false;
                    if (model.QUOTE_ONCE.Equals("F"))
                    {
                        model.QUOTE_ONCE = "T";
                        isUpdate = true;
                    }

                    if (DateTime.Compare(rollItem.RollMonth, (model.ROLL_DATE != null) ? (DateTime)model.ROLL_DATE : DateTime.MinValue) > 0)
                    {
                        model.ROLL_DATE = rollItem.RollMonth;
                        isUpdate = true;
                    }

                    if (isUpdate)
                    {
                        model.UPDATE_USER = loginUser;
                        model.UPDATE_DATE = dbNow;
                        BasePmPlanService.IsDataImport = true;
                        BasePmPlanService.UpdateData(model, false);
                    }
                }
            }

            UOW.Save();

            foreach (var implSid in implSidList)
                doMailNotifyCheck(implSid);

            return rollingDateList.Last().NextPlanDate;
        }





        public static List<RollingDateResult> CalculateRollingDates(
            PM_PLAN model,
            DateTime rollMonth,
            DateTime firstPlanDate,
            DateTime dbNow,
            string HolidaySet,
            List<DateTime> holidays
        )
        {
            DateTime dbToday = new DateTime(dbNow.Year, dbNow.Month, dbNow.Day);
            DateTime EndRollDate = rollMonth.AddMonths(1);

            string Calculation = "Plan";

            // 結果清單
            List<RollingDateResult> rollingDates = new List<RollingDateResult>();
            HashSet<DateTime> keys = new HashSet<DateTime>();

            DateTime PlanDate = firstPlanDate;
            DateTime PrePlanDate = firstPlanDate;

            // 日期滾算迴圈
            while (DateTime.Compare(EndRollDate, PlanDate) > 0)
            {
                // === 假日調整 ===
                DateTime tmpPlanDate = PlanDate;

                // 先向前調整
                var offset = CheckHolidayAndCorrection(PlanDate, holidays, PrePlanDate, HolidaySet, false);

                // 若無法向前,則向後調整
                if (DateTime.Compare(PlanDate, PrePlanDate) <= 0)
                {
                    PlanDate = tmpPlanDate;
                    offset = CheckHolidayAndCorrection(PlanDate, holidays, EndRollDate, HolidaySet, true);
                }

                // 檢查是否超出滾算範圍
                if (DateTime.Compare(EndRollDate, PlanDate) <= 0)
                    break;

                // 去重檢查
                if (keys.Add(PlanDate) == false)
                {
                    PlanDate = PlanDate.AddDays(1);
                    continue;
                }

                var NextPlanDate = CalculateNextPlanDate(model, tmpPlanDate, firstPlanDate);

                // 只處理未來日期(含今天)
                if (DateTime.Compare(PlanDate, dbToday) >= 0)
                {
                    rollingDates.Add(new RollingDateResult
                    {
                        PlanDate = PlanDate,
                        PrePlanDate = PrePlanDate,
                        RollMonth = rollMonth,
                        offset = offset,
                        NextPlanDate = NextPlanDate,
                    });
                }

                // === 計算下次計劃日期 ===
                PrePlanDate = PlanDate;
                PlanDate = NextPlanDate;
            }

            return rollingDates;
        }


        public static int CheckHolidayAndCorrection(DateTime planDate, List<DateTime> holidays, DateTime chkEndDate,
            string HolidaySet,
            bool isForeward = true)
        {
            string Calculation = "Plan";
            int r = 0;
            switch (HolidaySet)
            {
                case "Weekly":
                    if ((planDate.DayOfWeek).Equals(DayOfWeek.Saturday))
                        r = isForeward ? 2 : -1;
                    else if (planDate.DayOfWeek.Equals(DayOfWeek.Sunday))
                        r = isForeward ? 1 : -2;
                    break;
                case "Calendar":
                    while (holidays.Contains(planDate)
                        && DateTime.Compare(
                            isForeward ? chkEndDate : planDate,
                            isForeward ? planDate : chkEndDate) > 0)
                    {
                        r = 1;
                    }
                    break;
            }
            return r;
        }

        // =====================================================
        // 3. 計算下次計劃日期 (獨立方法)
        // =====================================================
        public static DateTime CalculateNextPlanDate(PM_PLAN model, DateTime currentPlanDate, DateTime firstPlanDate)
        {
            DateTime nextDate = currentPlanDate;

            if (MaintenanceUnit.Day.ToString().Equals(model.PLAN_UNIT))
            {
                // 天: 加上指定天數
                nextDate = currentPlanDate.AddDays(decimal.ToInt16(model.UNIT_NUMBER));
            }
            else if (MaintenanceUnit.Week.ToString().Equals(model.PLAN_UNIT))
            {
                // 週: 找下一個指定星期幾
                nextDate = currentPlanDate.AddDays(1);
                while (((int)nextDate.DayOfWeek) != decimal.ToInt16(model.UNIT_NUMBER))
                {
                    nextDate = nextDate.AddDays(1);
                }
            }
            else if (MaintenanceUnit.Month.ToString().Equals(model.PLAN_UNIT))
            {
                // 月: 找下個月的指定日期
                nextDate = currentPlanDate.AddDays(1);
                int days = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);

                if (nextDate.Day > decimal.ToInt16(model.UNIT_NUMBER))
                {
                    int addDays = (days - nextDate.Day) + decimal.ToInt16(model.UNIT_NUMBER);
                    nextDate = nextDate.AddDays(addDays);
                }
                else
                {
                    int addDays = decimal.ToInt16(model.UNIT_NUMBER) - nextDate.Day;
                    nextDate = nextDate.AddDays(addDays);
                }
            }
            else if (MaintenanceUnit.SixMonth.ToString().Equals(model.PLAN_UNIT))
            {
                // 半年: 從首次計劃日期開始,每 6 個月
                DateTime tmpDate = firstPlanDate;
                while (DateTime.Compare(currentPlanDate, tmpDate) >= 0)
                {
                    tmpDate = tmpDate.AddMonths(6);
                }
                nextDate = tmpDate;
            }
            else if (MaintenanceUnit.Year.ToString().Equals(model.PLAN_UNIT))
            {
                // 年: 從首次計劃日期開始,每年同一天
                DateTime tmpDate = firstPlanDate;
                while (DateTime.Compare(currentPlanDate, tmpDate) >= 0)
                {
                    tmpDate = tmpDate.AddYears(1);
                }
                nextDate = tmpDate;
            }

            return nextDate;
        }
    }
}
