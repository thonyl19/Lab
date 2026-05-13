using BLL.Base;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using BLL.MVC;
using BundleTransformer.Core.Transformers;
using Frame.Code;
using Frame.Code.Web.Select;
using Genesis.Common;
using Genesis.Gtimes.Common;
using Genesis.Library.BLL;
using Genesis.Library.BLL.MES.DataViews;
using Genesis.Web.SwaggeRegister.Common;
using MDL.MES;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml.Linq;
using static BLL.MVC.ResourceServices;
using vFile = System.IO.File;
using NetHttp = System.Net.Http;
using static BLL.MES.WIPInjectServices;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using static Genesis.Library.BLL.BillServices.BillEnum;
using Genesis.Library.BLL.QMS.Definition;
using System.Data.Entity;


//*
//BundleConfig~.cs 中 , 預設是用 System.Web.Mvc , 
//	所以一定要使用全名 -- System.Web.Http.Route , 
//	不然設定不會產生作用
namespace Genesis.WebApi
{
 	public partial class SelfInfoController : System.Web.Http.ApiController
	{
        

        public class d_人員資訊  
        {
            public d_Base _Base { get; set; }
            public Act_CUB Action_CUB { get; set; }
            public class Act_CUB
            {
                /// <summary>
                /// 0)不執行 1)查詢 2)執行離站 
                /// </summary>
                public int 找尋未離站資訊_離站 { get; set; } 
            }
        }
 

        [System.Web.Http.HttpPost]
		[System.Web.Http.Route("人員資訊")]
        [System.Web.Http.AllowAnonymousAttribute]
		public dynamic 人員資訊(d_人員資訊 data)
		=> TxnBase.LzDBQuery<dynamic>(Txn => {
            var _Test = data._Base._Test;
            var _UserNo = data._Base.UserNo.ts_NullString();
            if (data.Action_CUB == null) return null;
            if (data.Action_CUB.找尋未離站資訊_離站.isAction()) {
                Check.Invalid("UserNo 必須有值", _UserNo == null);
                var 找尋未離站資訊 = Txn.EFQuery_MES.WP_USER_TRACE_IN_MASTER
                        .FirstOrDefault(x => x.USER_NO == _UserNo && x.RUN_END_DATE == null);
                if(data.Action_CUB.找尋未離站資訊_離站 == 2){
                    var svc = new Genesis.Library.BLL.ZZ.CUB.ApiService();
                    var work_log = Txn.EFQuery_MES.WP_USER_TRACE_IN.Where
                        (x => x.LOT == 找尋未離站資訊.LOT
                            && x.USER_NO == _UserNo
                            && x.ACTION_LINK_SID == null && x.END_TIME == null)
                        .FirstOrDefault();
                    if (work_log != null){
                        Txn.ExeTime = DateTime.Now;
                        work_log.ACTION_LINK_SID = Txn.LinkSID;
                        work_log.UPDATE_USER = _UserNo;
                        work_log.UPDATE_DATE = Txn.ExeTime;
                        work_log.END_TIME = Txn.ExeTime;

                        work_log.EXE_OUTPUT_QTY = 0;
                        work_log.TOTAL_TIME = work_log.parse_TOTAL_TIME();
                        Txn.EFQuery_MES.SaveChanges();
                    }
                    return svc.UserTraceLeave(Txn, 找尋未離站資訊.IN_MASTER_SID,false, true);
                }
                return 找尋未離站資訊;
            }
            return null;
		});


        public class d_站別檢驗單 
        {
            public d_Base _Base { get; set; }
            public d_站別檢驗單_Act Action;

            public int? _UserIn { get; set; } 
            public d_卡控檢核 卡控檢核 { get; set; }
            public d_列舉執行類型 列舉執行類型 { get; set; }

            public class d_站別檢驗單_Act
            {
                public int 卡控檢核;
                public int 列舉執行類型;
                public int 產生檢驗單;
                public int 批次刪除檢驗單;
            }

            public class d_卡控檢核
            {
                public string LOT_SID { get; set; }
                public string UserNo { get; set; }
                public int CheckInOut_UserInOut_BeforeStartWork_AfterEndWork { get; set; }
                /// <summary>
                /// 0)預設 1)產生檢驗單 2)
                /// </summary>
                public int 產生檢驗單_核可;
            }
            public class d_列舉執行類型 : d_Base_工作站 { }

            
        }
        #region [站別檢驗單]
        [System.Web.Http.AllowAnonymousAttribute]
        [System.Web.Http.Route("站別檢驗單")]
        public dynamic 站別檢驗單(d_站別檢驗單 data)
		=> TxnBase.LzDBQuery<dynamic>(Txn => {
            var _Test = data._Base._Test;
            if (data.Action.列舉執行類型.ts_Bool()){
                var oper = data.列舉執行類型.get_PF_OPERATION(Txn);
                return Txn.EFQuery_MES.f站別檢驗單設定(oper.OPER_SID);
            }
            else if (data.Action.卡控檢核.isAction()){
                return 站別檢驗單_卡控檢核(data, Txn, _Test);
            }
            else if (data.Action.批次刪除檢驗單.isAction()){
                return 站別檢驗單_批次刪除檢驗單(data, Txn);
            }
            return data;
        });

        static dynamic 站別檢驗單_卡控檢核(d_站別檢驗單 data, ITxnBase Txn, bool _Test)
        {
            var q2 = data.卡控檢核;
            var ILot = Txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                .Where(c => c.LOT_SID == q2.LOT_SID)
                .FirstOrDefault_CheckExists();
            var 檢驗單檢核時機 = data.卡控檢核.CheckInOut_UserInOut_BeforeStartWork_AfterEndWork
                        .ts_Enum<Genesis.Library.BLL.ZZ.CUB.CodeRule.站別檢驗單檢核時機>();

            var r = Library.BLL.ZZ.CUB.OperInfo.Basic.基本檢核_站別檢驗單卡控
                (Txn, ILot
                , q2.UserNo
                //, data._UserIn.ts_NullBool()
                , 檢驗單檢核時機
                , _Test);

            if (data.卡控檢核.產生檢驗單_核可 != 0)
            {

                var 需要檢核的檢驗表單 = r.站別檢驗單設定.FirstOrDefault().Value;
                var oper = Txn.GetOperationInfo(ILot.OPER_SID);
                Txn.ExeTime = DateTime.Now;

                foreach (var form in 需要檢核的檢驗表單)
                {
                    var OperInsp = form.OperInsp;
                    var QC_INSP = form.QC_INSP;
                    var _WP_IPQC = new WP_IPQC()
                    {
                        QC_NO = Txn.GetSID()
                        ,
                        QC_INSP_METHOD = "NORMAL"
                        ,
                        INSP_STATUS = nameof(RES.BLL.Face.Confirm)
                        ,
                        INSP_SID = QC_INSP.INSP_SID,
                        QC_INSP_TYPE = QC_INSP.INSP_TYPE,
                        FORM_TYPE = QC_INSP.INSP_TYPE
                        ,
                        FORM_TARGET = QC_INSP.INSP_KEY
                        ,
                        OPERATION = oper.Name
                        ,
                        OPERATION_NO = oper.No
                        ,
                        OPER_SID = oper.SID
                        ,
                        CREATE_USER = q2.UserNo
                        ,
                        UPDATE_USER = q2.UserNo
                        ,
                        CREATE_DATE = Txn.ExeTime
                        ,
                        UPDATE_DATE = Txn.ExeTime
                        ,
                        QC_RESULT = QcResult.Accept.ToString(),
                        DESCRIPTION = "TEST"
                    };
                    Txn.EFQuery_MES.WP_IPQC.Add(_WP_IPQC);
                    var obj_ZZ_INSP_RECHECK = new ZZ_INSP_RECHECK()
                    {
                        SID = Txn.GetSID(true),
                        QC_NO = _WP_IPQC.QC_NO,
                        STATUS = nameof(RES.BLL.Face.Confirm),
                        CREATE_USER = q2.UserNo,
                        UPDATE_USER = q2.UserNo,
                        CREATE_DATE = Txn.ExeTime,
                        UPDATE_DATE = Txn.ExeTime
                    };
                    switch (data.卡控檢核.產生檢驗單_核可)
                    {
                        case 2://核可
                            obj_ZZ_INSP_RECHECK.STATUS = nameof(RES.BLL.Face.Finished);
                            obj_ZZ_INSP_RECHECK.RESULT = nameof(RES.BLL.Face.ContinuedProcess);
                            obj_ZZ_INSP_RECHECK.AUTHORIZE_USER = q2.UserNo;
                            obj_ZZ_INSP_RECHECK.AUTHORIZE_DATE = Txn.ExeTime;
                            break;
                    }

                    Txn.EFQuery_MES.ZZ_INSP_RECHECK.Add(obj_ZZ_INSP_RECHECK);

                    var obj_WP_IPQC_LOT = new WP_IPQC_LOT()
                    {
                        SID = Txn.GetSID(true),
                        QC_NO = _WP_IPQC.QC_NO,
                        ROUTE_VER_OPER_SID = ILot.ROUTE_VER_OPER_SID,
                        LOT_SID = ILot.LOT_SID,
                        LOT = ILot.LOT,
                    };
                    Txn.EFQuery_MES.WP_IPQC_LOT.Add(obj_WP_IPQC_LOT);
                    Txn.EFQuery_MES.SaveChanges();
                    return _WP_IPQC;
                }
            }
            return r;
        }

        static dynamic 站別檢驗單_批次刪除檢驗單(d_站別檢驗單 data, ITxnBase Txn){
            Check.MustInput("SID", data._Base.SID);
            var _list = data._Base.SID.Split(',').ToList();
            var q1 = (from a in Txn.EFQuery_MES.WP_IPQC
                      where _list.Contains(a.QC_NO)
                      select a);
            var q2 = (from a in Txn.EFQuery_MES.ZZ_INSP_RECHECK
                      where q1.Any(c => c.QC_NO == a.QC_NO)
                      select a);
            var q3 = (from a in Txn.EFQuery_MES.WP_IPQC_LOT
                      where q1.Any(c => c.QC_NO == a.QC_NO)
                      select a);
            var q4 = (from a in Txn.EFQuery_MES.WP_IPQC_CHECKITEM
                      where q1.Any(c => c.QC_NO == a.ACTION_LINK_SID)
                      select a);
            var q5 = (from a in Txn.EFQuery_MES.WP_IPQC_CHECKITEM_RAW
                      where q4.Any(c => c.WP_IPQC_CHECKITEM_SID == a.ACTION_LINK_SID)
                      select a);
            Txn.EFQuery_MES
                ._BulkDelete(q5)
                ._BulkDelete(q4)
                ._BulkDelete(q3)
                ._BulkDelete(q2)
                ._BulkDelete(q1);

            return _list;

        }
        #endregion [站別檢驗單]

        #region [檢驗單]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Insp/{UserNo}")]
        [System.Web.Http.AllowAnonymousAttribute]
        public dynamic 檢驗單(string UserNo, string Action = null)
        => TxnBase.LzDBQuery<dynamic>(Txn => {
            switch (Action)
            {
                case "站別檢驗單卡控":
                case "併行工站_離站":
                    var 找尋未離站資訊 = Txn.EFQuery_MES.WP_USER_TRACE_IN_MASTER
                        .FirstOrDefault(x => x.USER_NO == UserNo && x.RUN_END_DATE == null);
                    if (找尋未離站資訊 != null && Action == "併行工站_離站")
                    {
                        var svc = new Genesis.Library.BLL.ZZ.CUB.ApiService();
                        var work_log = Txn.EFQuery_MES.WP_USER_TRACE_IN.Where
                            (x => x.LOT == 找尋未離站資訊.LOT
                                && x.USER_NO == UserNo
                                && x.ACTION_LINK_SID == null && x.END_TIME == null)
                            .FirstOrDefault();
                        if (work_log != null)
                        {
                            Txn.ExeTime = DateTime.Now;
                            work_log.ACTION_LINK_SID = Txn.LinkSID;
                            work_log.UPDATE_USER = UserNo;
                            work_log.UPDATE_DATE = Txn.ExeTime;
                            work_log.END_TIME = Txn.ExeTime;

                            work_log.EXE_OUTPUT_QTY = 0;
                            work_log.TOTAL_TIME = work_log.parse_TOTAL_TIME();
                            Txn.EFQuery_MES.SaveChanges();
                        }
                        return svc.UserTraceLeave(Txn, 找尋未離站資訊.IN_MASTER_SID,false, true);
                    }
                    return 找尋未離站資訊;
                    break;
                default:
                    //return Txn.EFQuery_MES.f_Oper_找出關聯流程(OperNo).ToList();
                    break;
            }
            return null;
        });

        #endregion [檢驗單]


        #region [保養單]

        [System.Web.Http.AllowAnonymousAttribute]
        [System.Web.Http.Route("保養單")]
        public dynamic 保養單(d_保養單 data)
        => TxnBase.LzDBQuery<dynamic>(Txn => {
            if (data.Action.刪除保養單.isAction()) {
                return 保養單_刪除保養單(data, Txn);
            }
            return data;
        });

        static dynamic 保養單_刪除保養單(d_保養單 data, ITxnBase Txn)
        {
            Check.MustInput("SID", data._Base.SID);
            var _list = data._Base.SID.Split(',').ToList();
            var q1 = (from a in Txn.EFQuery_MES.PM_PLANIMPLEMENT
                      where _list.Contains(a.PLANIMPLEMENT_SID)
                      select a);
            var q2 = (from a in Txn.EFQuery_MES.PM_PLANIMPLEMENT_PARTNO
                      where q1.Any(c => c.PLANIMPLEMENT_SID == a.PLANIMPLEMENT_SID)
                      select a);
            var q3 = (from a in Txn.EFQuery_MES.PM_PLANIMPLEMENTDEFECT
                      where q1.Any(c => c.PLANIMPLEMENT_SID == a.PLANIMPLEMENT_SID)
                      select a);
            var q4 = (from a in Txn.EFQuery_MES.PM_PLANIMPLEMENTITEM
                      where q1.Any(c => c.PLANIMPLEMENT_SID == a.PLANIMPLEMENT_SID)
                      select a);
            var q5 = (from a in Txn.EFQuery_MES.PM_PLANIMPLEMENTUSER
                      where q1.Any(c => c.PLANIMPLEMENT_SID == a.PLANIMPLEMENT_SID)
                      select a);

            Txn.EFQuery_MES
                ._BulkDelete(q5)
                ._BulkDelete(q4)
                ._BulkDelete(q3)
                ._BulkDelete(q2)
                ._BulkDelete(q1);

            return new { 刪除清單=_list };

        }

        #endregion [保養單]
    }



}

namespace Genesis
{
    public partial class GTI_Test
    {
        //*/
        public static void t_Process_CUB(ITxnBase txn, string key)
        {
            var WP_LOT_OPER_PARALLEL_HIST = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_HIST.IQueryable_ACTION_LINK_SID(key);
            var WP_LOT_OPER_PARALLEL =
                (from a in txn.EFQuery_MES.WP_LOT_OPER_PARALLEL
                    .Where(c => WP_LOT_OPER_PARALLEL_HIST.Any(c1 => c1.LOT == c.LOT && c1.OLD_ROUTE_VER_OPER_SID == c.ROUTE_VER_OPER_SID))
                 select a
                ).ToList();
            var _lot = WP_LOT_OPER_PARALLEL.FirstOrDefault();
            var LOT = new
            {
                WP_LOT_OPER_PARALLEL,
                WP_LOT_OPER_PARALLEL_SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SN.GetData_ACTION_LINK_SID(key),
                WP_LOT_OPER_PARALLEL_HIST = WP_LOT_OPER_PARALLEL_HIST.ToList(),
                WO = _lot == null ? null : txn.EFQuery_MES.WP_WO.FirstOrDefault(c => c.WO == _lot.WO),
                WP_LOT_REMARK = _lot == null ? null : txn.EFQuery_MES.WP_LOT_REMARK.GetData_ACTION_LINK_SID(key),
            };


            var ZZ_WP_LOT_OPER_PARALLEL_CONSUM = txn.EFQuery_MES.ZZ_WP_LOT_OPER_PARALLEL_CONSUM.IQueryable_ACTION_LINK_SID(key);
            var Else = new
            {
                ZZ_WP_LOT_OPER_PARALLEL_CONSUM,
                ZZ_WP_LOT_OPER_PARALLEL_CONSUM_TMPDATA =
                    (from a in txn.EFQuery_MES.ZZ_WP_LOT_OPER_PARALLEL_CONSUM_TMPDATA
                        .Where(c => ZZ_WP_LOT_OPER_PARALLEL_CONSUM.Any(c1 => c1.LOT_CONSUM_SID == c.LOT_CONSUM_SID))
                     select a).ToList(),
                ZZ_WP_PACKAGE_ITEM = txn.EFQuery_MES.ZZ_WP_PACKAGE_ITEM.GetData_ACTION_LINK_SID(key)
            };


            var r = new
            {
                key,
                ExeTime = txn.ExeTime,
                LOT,
                USER = trc_USER_CUB(txn, key),
                TOOL = trc_TOOL(txn, key),
                EQP = trc_EQP_CUB(txn, key),
                HOLD = trc_HOLD(txn, key),
                SCRAP = trc_SCRAP(txn, key),
                DEFECT = trc_DEFECTC(txn, key),
                EDC = trc_EDC_CUB(txn, key),
                SMT = trc_SMT(txn, key),
                ADLog = trc_ADLog(txn),
                站別檢驗單 = trc_站別檢驗單(txn, key),
                Else
            };

            string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }

        
        public static void TxnBase_T_OperTask_人員上崗(ITxnBase Txn, string ActionName, string Link_SID)
        { 
            var AD_CATCH_DATA = (from a in Txn.EFQuery_MES.AD_CATCH_DATA
                                    where  a.UPDATE_DATE == Txn.ExeTime
                            select a
                            )
                            .AsNoTracking()
                            .ToList();
            var ZZ_PICKING_LIST_HIST = (from a in Txn.EFQuery_MES.ZZ_PICKING_LIST_HIST
                        where  a.CREATE_DATE == Txn.ExeTime
                select a
                )
                .AsNoTracking()
                .ToList();

            var data = new
            {
                AD_CATCH_DATA,
                ZZ_PICKING_LIST_HIST
            };
            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);

        }

        public static void TxnBase_T_PMSRepairV2(ITxnBase Txn, string ActionName, string Link_SID)
        {
            var form = Txn.result.Data as PM_REPAIR;
            if (form == null) {
                form = Txn.EFQuery_MES.PM_REPAIR
                    .Where(c => c.UPDATE_DATE == Txn.ExeTime)
                    .AsNoTracking()
                    .FirstOrDefault();
            }
            var TARGET_SID = form?.TARGET_SID;
            var view_EqpExt = (from a in  Txn.EFQuery_MES.view_EqpExt()
                            where a.Main.EQP_SID == TARGET_SID
                                && a.Main.UPDATE_DATE == Txn.ExeTime
                               select a).ToList();

            var PM_REPAIR_PARTNO = (from a in Txn.EFQuery_MES.PM_REPAIR_PARTNO
                                    where a.TARGET_SID == TARGET_SID
                                && a.UPDATE_DATE == Txn.ExeTime
                            select a
                            )
                            .AsNoTracking()
                            .ToList();
            var FC_TOOL = (from a in Txn.EFQuery_MES.FC_TOOL
                            where a.TOOL_SID == TARGET_SID
                                && a.UPDATE_DATE == Txn.ExeTime
                            select a
                            )
                            .AsNoTracking()
                            .ToList();

            var data = new
            {
                TARGET_SID,
                PM_REPAIR = form,
                PM_REPAIR_PARTNO,
                view_EqpExt,
                FC_TOOL,
                ADLog = trc_ADLog(Txn),
            };
            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }


        public static dynamic trc_EQP_CUB(ITxnBase txn, string key)
        => new{
            WP_EQP = txn.EFQuery_MES.view_EqpExt().Where(c=>c.Main.UPDATE_DATE == txn.ExeTime).ToList(),
            WP_LOT_EQP_TRACE = txn.EFQuery_MES.WP_LOT_EQP_TRACE.Where(c => c.UPDATE_DATE == txn.ExeTime).ToList(),
            WP_EQP_TRACE = txn.EFQuery_MES.WP_EQP_TRACE.GetData_ACTION_LINK_SID(key)
        };


        public static dynamic trc_USER_CUB(ITxnBase txn, string key)
        {
            var WP_USER_TRACE_IN = txn.EFQuery_MES.WP_USER_TRACE_IN.IQueryable_ACTION_LINK_SID(key);
            var WP_USER_TRACE_IN_MASTER =
                (from a in txn.EFQuery_MES.WP_USER_TRACE_IN_MASTER
                    .Where(c => WP_USER_TRACE_IN.Any(c1 => c1.IN_MASTER_SID == c.IN_MASTER_SID))
                 select a
                ).ToList();

            var EqpToolCatch = (from a in txn.EFQuery_MES.AD_CATCH_DATA
                    where a.UPDATE_DATE == txn.ExeTime
                    select a
                ).ToList();

            return new
            {
                WP_USER_TRACE = txn.EFQuery_MES.WP_USER_TRACE.GetData_ACTION_LINK_SID(key),
                WP_USER_TRACE_IN_MASTER,
                WP_USER_TRACE_IN = WP_USER_TRACE_IN.ToList(),
                ZZ_OPER_WORKT_SUMMARY = txn.EFQuery_MES.ZZ_OPER_WORKT_SUMMARY.GetData_ACTION_LINK_SID(key),
                EqpToolCatch,
            };
        }

        public static dynamic trc_SMT(ITxnBase txn, string key)
        {
            var WP_LOT_CARRIER_TRACE = txn.EFQuery_MES.WP_LOT_CARRIER_TRACE.IQueryable_ACTION_LINK_SID(key);
            var WP_CARRIER_TRACE = txn.EFQuery_MES.WP_CARRIER_TRACE.GetData_ACTION_LINK_SID(key);
            var FC_CARRIER = (from a in txn.EFQuery_MES.FC_CARRIER
                    .Where(c => WP_LOT_CARRIER_TRACE.Any(c1 => c1.CARRIER_SID == c.CARRIER_SID))
                              select a
                ).ToList();

            //var SMT_CARRIER_TOOL_HIST = txn.EFQuery_MES.SMT_CARRIER_TOOL_HIST
            //        .Where(c => c.UPDATE_DATE == txn.ExeTime).ToList();

            var SMT_TOOL_MTLOT_HIST = txn.EFQuery_MES.SMT_TOOL_MTLOT_HIST
                    .Where(c => c.UPDATE_DATE == txn.ExeTime).ToList();
            return new
            {
                WP_LOT_CARRIER_TRACE = WP_LOT_CARRIER_TRACE.ToList(),
                WP_CARRIER_TRACE,
                FC_CARRIER,
                //SMT_CARRIER_TOOL_HIST,
                SMT_TOOL_MTLOT_HIST
            };
        }


        public static dynamic trc_EDC_CUB(ITxnBase txn, string key)
        {
            var WP_LOT_OPER_PARALLEL_EDC = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_EDC.IQueryable_ACTION_LINK_SID(key);
            var WP_LOT_OPER_PARALLEL_EDC_ROW =
                (from a in txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_EDC_ROW
                    .Where(c => WP_LOT_OPER_PARALLEL_EDC.Any(c1 => c1.LOT_EDC_SID == c.LOT_EDC_SID))
                 select a
                ).ToList();
            return new
            {
                WP_LOT_OPER_PARALLEL_EDC = WP_LOT_OPER_PARALLEL_EDC.ToList(),
                WP_LOT_OPER_PARALLEL_EDC_ROW,
                WP_LOT_OPER_PARALLEL_EDC_SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_EDC_SN.GetData_ACTION_LINK_SID(key)
            };
        }

        public static dynamic trc_DEFECTC(ITxnBase txn, string key)
        => new
        {
            WP_LOT_OPER_PARALLEL_DEFECT = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_DEFECT.GetData_ACTION_LINK_SID(key),
            WP_LOT_OPER_PARALLEL_DEFECT_SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_DEFECT_SN.GetData_ACTION_LINK_SID(key),
        };

        public static dynamic trc_HOLD(ITxnBase txn, string key)
        => new
        {
            WP_LOT_OPER_PARALLEL_HOLD = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_HOLD.Where(c => c.RELEASE_LINK_SID == key).ToList(),
        };

        public static dynamic trc_SCRAP(ITxnBase txn, string key)
        => new
        {
            WP_LOT_OPER_PARALLEL_SCRAP = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SCRAP.GetData_ACTION_LINK_SID(key),
            WP_LOT_OPER_PARALLEL_SCRAPT_SN = txn.EFQuery_MES.WP_LOT_OPER_PARALLEL_SCRAP_SN.GetData_ACTION_LINK_SID(key),
        };

        public static dynamic trc_站別檢驗單(ITxnBase txn, string key)
        {
            var WP_USER_TRACE_IN = txn.EFQuery_MES.WP_USER_TRACE_IN
                    .Where(c => c.UPDATE_DATE == txn.ExeTime);
            var WP_USER_TRACE_IN_MASTER = txn.EFQuery_MES.WP_USER_TRACE_IN_MASTER
                    .Where(c => c.UPDATE_DATE == txn.ExeTime);
            var ZZ_INSP_RECHECK = txn.EFQuery_MES.ZZ_INSP_RECHECK
                    .Where(c => WP_USER_TRACE_IN.Any(c1 => c1.IN_SUPPORT_SID == c.LINK_SID)
                        || WP_USER_TRACE_IN_MASTER.Any(c1 => c1.IN_MASTER_SID == c.LINK_SID)
                    )
                    .ToList();

            return new
            {
                ZZ_INSP_RECHECK,
                WP_USER_TRACE_IN_MASTER = WP_USER_TRACE_IN_MASTER.ToList(),
                WP_USER_TRACE_IN = WP_USER_TRACE_IN.ToList(),
            };
        }


        public static void TxnBase_T_保養單(ITxnBase Txn, string ActionName, string Link_SID)
        {
            PM_PLANIMPLEMENT srcform = Txn.result.Data;
            if (srcform == null) return;
            var PLANIMPLEMENT_SID = srcform.PLANIMPLEMENT_SID;

            var PM_PLANIMPLEMENT = Txn.EFQuery_MES.PM_PLANIMPLEMENT
                .Where(c => c.PLANIMPLEMENT_SID == srcform.PLANIMPLEMENT_SID)
                .AsNoTracking()
                .FirstOrDefault();

            var PM_PLANIMPLEMENTITEM = Txn.EFQuery_MES.PM_PLANIMPLEMENTITEM
                .Where(c => c.PLANIMPLEMENT_SID == srcform.PLANIMPLEMENT_SID)
                .AsNoTracking()
                .ToList();

            var PM_PLANIMPLEMENTUSER = Txn.EFQuery_MES.PM_PLANIMPLEMENTUSER
                .Where(c => c.PLANIMPLEMENT_SID == srcform.PLANIMPLEMENT_SID)
                .AsNoTracking()
                .ToList();

            var PM_PLANIMPLEMENT_PARTNO = Txn.EFQuery_MES.PM_PLANIMPLEMENT_PARTNO
                .Where(c => c.PLANIMPLEMENT_SID == srcform.PLANIMPLEMENT_SID)
                .AsNoTracking()
                .ToList();

            var PM_PLANIMPLEMENTDEFECT = Txn.EFQuery_MES.PM_PLANIMPLEMENTDEFECT
                .Where(c => c.PLANIMPLEMENT_SID == srcform.PLANIMPLEMENT_SID)
                .AsNoTracking()
                .ToList();

            //var WP_IPQC_CHECKITEM_RAW =
            //    (from a in Txn.EFQuery_MES.WP_IPQC_CHECKITEM_RAW
            //        .Where(c => WP_IPQC_CHECKITEM.Any(c1 => c1.WP_IPQC_CHECKITEM_SID == c.ACTION_LINK_SID))
            //     select a
            //    ).ToList();

            var 保養單 = new {
                srcform,
                PM_PLANIMPLEMENT,
                PM_PLANIMPLEMENTITEM,
                PM_PLANIMPLEMENTUSER,
                PM_PLANIMPLEMENT_PARTNO,
                PM_PLANIMPLEMENTDEFECT,
            };

            string json = JsonConvert.SerializeObject(保養單, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }





        public static void TxnBase_T_站別檢驗單(ITxnBase Txn, string ActionName, string Link_SID)
        {
            WP_IPQC form = Txn.result.Data;
            if (form == null) return;
            var WP_IPQC_LOT = Txn.EFQuery_MES.WP_IPQC_LOT
                .Where(c => c.QC_NO == form.QC_NO)
                .ToList();

            var ZZ_INSP_RECHECK = Txn.EFQuery_MES.ZZ_INSP_RECHECK
                .Where(c => c.QC_NO == form.QC_NO)
                .ToList();

            var WP_IPQC_CHECKITEM = Txn.EFQuery_MES.WP_IPQC_CHECKITEM
                .Where(c => c.ACTION_LINK_SID == form.QC_NO)
                ;

            var WP_IPQC_CHECKITEM_RAW =
                (from a in Txn.EFQuery_MES.WP_IPQC_CHECKITEM_RAW
                    .Where(c => WP_IPQC_CHECKITEM.Any(c1 => c1.WP_IPQC_CHECKITEM_SID == c.ACTION_LINK_SID))
                 select a
                ).ToList();

            var IPQC = new
            {
                WP_IPQC = form,
                ZZ_INSP_RECHECK,
                WP_IPQC_LOT,
                WP_IPQC_CHECKITEM = WP_IPQC_CHECKITEM.ToList(),
                WP_IPQC_CHECKITEM_RAW,
            };


            string json = JsonConvert.SerializeObject(IPQC, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }


    }

    public static partial class ext
    {
        //WP_LOT_OPER_PARALLEL
        //public static IQueryable<WP_LOT_OPER_PARALLEL> f_Oper_找出併行工站批號
        //   (this MESContext _self
        //   , string OperNo)
        //=> from a in _self.WP_LOT_OPER_PARALLEL
        //   where _self.PF_ROUTE_VER_OPER
        //       .Any(c => c.OPERATION_NO == OperNo
        //       && a.ROUTE_VER_OPER_SID == c.ROUTE_VER_OPER_SID)
        //   select a;

        static string[] filed_WP_LOT_OPER_PARALLEL = new string[] { "LOT_SID", "LOT", "WO", "QUANTITY", "OPER_EXECUTED_QUANTITY", "STATUS", "ROUTE_VER_OPER_SID", "OPERATION" };

        //public static List<dynamic>
        public static List<Dictionary<string, object>> Select_Info(this IQueryable<WP_LOT_OPER_PARALLEL> query, params string[] fixedProperties)
        => query.SelectLotInfo_3(filed_WP_LOT_OPER_PARALLEL, fixedProperties);


        static string[] filed_WP_LOT_OPER_PARALLEL_HIST = new string[]{
            "LOT_HIST_SID", "LOT_SID", "ACTION",
            "APPLICATION_NAME", "ACTION_LINK_TABLE", "ACTION_REASON",
            "ACTION_DESCRIPTION", "OLD_STATUS", "NEW_STATUS",
            "OLD_OPER_EXECUTED_QUANTITY", "NEW_OPER_EXECUTED_QUANTITY", "OLD_QUANTITY",
            "NEW_QUANTITY", "OLD_OPERATION", "NEW_OPERATION",
            "OLD_RULE", "NEW_RULE", "OLD_EQP_LINK_SID",
            "NEW_EQP_LINK_SID", "OLD_PARTNO_LINK_SID", "NEW_PARTNO_LINK_SID",
            "OLD_CARRIER_LINK_SID", "NEW_CARRIER_LINK_SID", "OLD_TOOL_LINK_SID",
            "NEW_TOOL_LINK_SID", "EQP_LINK_SID", "TOOL_LINK_SID",
            "CARRIER_LINK_SID"
        };
        public static List<Dictionary<string, object>> Select_Info(this IQueryable<WP_LOT_OPER_PARALLEL_HIST> query, params string[] fixedProperties)
        => query.SelectLotInfo_3(filed_WP_LOT_OPER_PARALLEL_HIST, fixedProperties);
    }

}
 