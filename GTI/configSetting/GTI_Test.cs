using BLL.Base;
using BLL.MES;
using Frame.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.SqlClient;
using static BLL.MES.WIPInjectServices;
using MDL.MES;
using System.Reflection;
using MDL;
using static Genesis.Gtimes.WIP.LotUtility;
using System.Dynamic;
using static BLL.MVC.ResourceServices;
using Genesis.Common;
using BLL.InterFace;
using MDL.GenesisMVC.Tables;
using System.Collections;
using System.Resources;
using vFile = System.IO.File;
using NetHttp = System.Net.Http;
using Newtonsoft.Json.Linq;
using BLL.MVC;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Threading;
using Genesis.Library.BLL;
using System.Web.Http.Cors;
using Genesis.Web.SwaggeRegister.Common;
using Genesis.Gtimes.Common;
using System.Web.Routing;
using BLL.MES.DataViews;
using Genesis.Library.BLL.MES.DataViews;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using Genesis;
using Genesis.Gtimes.WIP;
using System.Data;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.Transaction.CAR;

namespace Genesis
{
    public interface IGTI_Test
    {
        string Debug { get; set; }
        IHtmlString methods { get; set; }
        IHtmlString mounted { get; set; }
        IHtmlString param_test { get; }

        IHtmlString Test(string code, int mode = 0);
    }


    public class Ext
    {
        public static IResult Add_Item(string PARAMETERGROUP_SID, List<AD_PARAMETER> list, bool isTest = false)
        => WIPInjectServices.TxnBase.LzDBTrans(null, Txn =>
        {
            var chk_list = list.Select(c => c.PARAMETER_NO).ToList();
            var err_list = Txn.EFQuery_MES.AD_PARAMETER
                .Where(c => chk_list.Contains(c.PARAMETER_NO))
                .ToList();
            Check.Invalid("PARAMETER_NO 已存在!", err_list.Count != 0, err_list);

            var _max = Txn.EFQuery_MES.AD_PARAMETERGROUP_LIST
                .Where(c => c.PARAMETERGROUP_SID == PARAMETERGROUP_SID)
                .Max(c => c.LIST_SEQ);
            if (_max != 0) _max++;

            List<AD_PARAMETERGROUP_LIST> groupLists = new List<AD_PARAMETERGROUP_LIST>();
            for (var idx = 0; idx < list.Count; idx++)
            {
                var _val = list[idx];
                Txn.EntityCommonSetVal(_val);
                var _obj = new AD_PARAMETERGROUP_LIST
                {
                    PARAMETERGROUP_LIST_SID = Txn.GetSID(),
                    PARAMETERGROUP_SID = PARAMETERGROUP_SID,
                    PARAMETER_SID = _val.PARA_SID,
                    LIST_SEQ = _max + idx,
                    CREATE_USER = Txn.UserNo,
                    CREATE_DATE = Txn.ExeTime,
                    UPDATE_USER = Txn.UserNo,
                    UPDATE_DATE = Txn.ExeTime,
                };
                groupLists.Add(_obj);
            }
            Txn.EFQuery_MES.AD_PARAMETER.AddRange(list);
            Txn.EFQuery_MES.AD_PARAMETERGROUP_LIST.AddRange(groupLists);
            Txn.EFQuery_MES.SaveChanges();
            return Txn.result;
        }, isTest);

        public static bool isEnable(int? val)
        {
            if (val == null) return false;
            return val != 0;
        }
        public static bool isAction(object val, bool isPass0 = true)
        => val != null && (isPass0 && (int)val != 0);

    }

    public static class ActionExtensions
    {
        public static dynamic Dispatch(this object actionContainer, object handler, ITxnBase txn, object data)
        {
            var fields = actionContainer.GetType().GetFields();
            System.Reflection.FieldInfo activeField = null;
            foreach (var f in fields)
            {
                var val = f.GetValue(actionContainer);
                if (val != null && val is int && (int)val != 0)
                {
                    activeField = f;
                    break;
                }
            }

            if (activeField == null) return null;

            var method = handler.GetType().GetMethod(activeField.Name);
            if (method == null) return null;

            var ps = method.GetParameters();
            if (ps.Length == 2) return method.Invoke(handler, new object[] { txn, data });
            if (ps.Length == 1) return method.Invoke(handler, new object[] { txn });
            return method.Invoke(handler, null);
        }
    }


    public partial class GTI_Test : IGTI_Test
    {
        public class g_path {
            public static string t_Process = @"P:\t_Process.json";
        }
        public static bool isTest { get; set; }

        static bool? _IsDebuggingEnabled;
        public static bool IsDebuggingEnabled
        {
            get
            {
                try
                {
                    if (_IsDebuggingEnabled == null)
                        _IsDebuggingEnabled = HttpContext.Current.IsDebuggingEnabled;
                }
                catch
                {
                    _IsDebuggingEnabled = false;
                }
                return (bool)_IsDebuggingEnabled;
            }
        }
        HtmlHelper htm;
        HttpRequestBase _Req;
        public GTI_Test(HtmlHelper htm, HttpContextBase HttpCon, string[] list = null)
        {
            init(htm, GTI_Test.IsDebuggingEnabled, HttpCon.Request, list);
        }
        public GTI_Test(HtmlHelper htm, bool IsDebuggingEnabled, HttpRequestBase Req = null, string[] list = null)
        {
            init(htm, IsDebuggingEnabled, Req, list);
        }

        private void init(HtmlHelper htm, bool IsDebuggingEnabled, HttpRequestBase Req, string[] list)
        {
            this.htm = htm;
            this._Req = Req;
            if (IsDebuggingEnabled)
            {
                List<string> _base = new List<string>() { "__gt_test:Vue.prototype.$GTI_Test.__gt_test," };
                this.mounted = htm.Raw("window.__vm = this;");
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        _base.Add($"{item}:Vue.prototype.$GTI_Test.{item},");
                    }
                }
                var _methods = string.Join("", _base.ToArray());
                this.methods = htm.Raw(_methods);
            }
            else
            {
                this.mounted
                    = this.methods
                    = htm.Raw("");
            }

            if (Req != null)
            {
                isTest = Req.Params["Test"] == "T";
                if (isTest)
                {
                    ServicesBase.isTest = true;
                    Debug = "debugger;";

                }
                UItest = Req.Params["UItest"];
            }

        }

        //bk
        public static void TxnBase_T(ITxnBase Txn, string ActionName, string Link_SID)
        {
            switch (ActionName) {
                case "":
                    break;
                //case "Process":
                default:
                    var key = $"t_Process_{ServicesBase.ProjectCustomer}";
                    var runDef = dyn_Process(key, new object[] { Txn, Link_SID });
                    if (runDef) t_Process(Txn, Link_SID);
                    break;
            }
        }


        public static void dyn_TxnBase_T(string ActionName) {
            TxnBase.Test = GTI_Test.TxnBase_T;
            if (string.IsNullOrEmpty(ActionName) == false)
            {
                var method = typeof(GTI_Test).GetMethod(ActionName, BindingFlags.Static | BindingFlags.Public);
                if (method != null)
                {
                    TxnBase.Test = (Action<ITxnBase, string, string>)Delegate.CreateDelegate(typeof(Action<ITxnBase, string, string>), method);
                }
            }
        }



        public static bool dyn_Process(string StaticMethod, object[] methodParameters)
        {

            MethodInfo methodInfo = typeof(Genesis.GTI_Test).GetMethod
                (StaticMethod, BindingFlags.Public | BindingFlags.Static);
            if (methodInfo != null) {
                methodInfo.Invoke(null, methodParameters);
                return false;
            }
            return true;
        }
        public static dynamic dyn_Process_echo(string StaticMethod, object[] methodParameters)
        {

            MethodInfo methodInfo = typeof(Genesis.GTI_Test).GetMethod
                (StaticMethod, BindingFlags.Public | BindingFlags.Static);
            if (methodInfo != null)
            {
                return methodInfo.Invoke(null, methodParameters);
            }
            return null;
        }


        public static void t_Process(ITxnBase txn, string key)
        {
            var WP_LOT_HIST = txn.EFQuery_MES.WP_LOT_HIST.IQueryable_ACTION_LINK_SID(key);
            var WP_LOT = (from a in txn.EFQuery_MES.WP_LOT
                .Where(c => WP_LOT_HIST.Any(c1 => c1.LOT == c.LOT))
                          select a
                ).ToList();
            var LOT = new
            {
                WP_LOT,
                WP_LOT_HIST = WP_LOT_HIST.ToList(),
                WP_LOT_SPLIT = txn.EFQuery_MES.WP_LOT_SPLIT.GetData_ACTION_LINK_SID(key),
                ZZ_LOT_MOVE = txn.EFQuery_MES.ZZ_LOT_MOVE.GetData_ACTION_LINK_SID(key),
            };
            var USER = new
            {
                WP_USER_TRACE = txn.EFQuery_MES.WP_USER_TRACE.GetData_ACTION_LINK_SID(key),
                //WP_USER_TRACE_IN = (from a in txn.EFQuery_MES.WP_USER_TRACE_IN
                //        .Where(c => WP_LOT_HIST.Any(c1 => c1.LOT == c.LOT) && c.CREATE_DATE == txn.ExeTime)
                //                    select a
                //    ).ToList(),
                ZZ_OPER_WORKT_SUMMARY = txn.EFQuery_MES.ZZ_OPER_WORKT_SUMMARY.GetData_ACTION_LINK_SID(key),
            };

            var DEFECT = new {
                WP_LOT_DEFECT = txn.EFQuery_MES.WP_LOT_DEFECT.GetData_ACTION_LINK_SID(key),
            };

            var SCRAP = new
            {
                WP_LOT_SCRAP = txn.EFQuery_MES.WP_LOT_SCRAP.GetData_ACTION_LINK_SID(key),
            };


            var r = new { LOT, USER, SCRAP, DEFECT,
                EQP = trc_EQP(txn, key),
                TOOL = trc_TOOL(txn, key),
                EDC = trc_EDC(txn, key),
                //CHECKLIST = trc_CHECKLIST(txn,key)
                //UserTraceIn = trc_UserTraceIn(txn,key)
            };

            string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }

        public static void TxnBase_T_IPQC(ITxnBase Txn, string ActionName, string Link_SID)
        {
            WP_IPQC form = Txn.result.Data;
            var WP_IPQC_LOT = Txn.EFQuery_MES.WP_IPQC_LOT
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
                WP_IPQC_LOT,
                WP_IPQC_CHECKITEM = WP_IPQC_CHECKITEM.ToList(),
                WP_IPQC_CHECKITEM_RAW,
            };
            string json = JsonConvert.SerializeObject(IPQC, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }

        public static void TxnBase_T_EDC(ITxnBase Txn, string ActionName, string Link_SID)
        {
            var result = new {
                TxnResultData = Txn.result.Data,
                EDC = trc_EDC(Txn, Link_SID)
            }
            
            ;
            string json = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }


        public static dynamic trc_USER(ITxnBase txn, string key)
        {
            //var WP_USER_TRACE_IN = txn.EFQuery_MES.WP_USER_TRACE_IN.IQueryable_ACTION_LINK_SID(key);
            //var WP_USER_TRACE_IN_MASTER =
            //    (from a in txn.EFQuery_MES.WP_USER_TRACE_IN_MASTER
            //        .Where(c => WP_USER_TRACE_IN.Any(c1 => c1.IN_MASTER_SID == c.IN_MASTER_SID))
            //     select a
            //    ).ToList();
            return new
            {
                WP_USER_TRACE = txn.EFQuery_MES.WP_USER_TRACE.GetData_ACTION_LINK_SID(key),
                //WP_USER_TRACE_IN_MASTER,
                //WP_USER_TRACE_IN = WP_USER_TRACE_IN.ToList(),
                ZZ_OPER_WORKT_SUMMARY = txn.EFQuery_MES.ZZ_OPER_WORKT_SUMMARY.GetData_ACTION_LINK_SID(key),
            };
        }
        /*
        public static dynamic trc_CHECKLIST(ITxnBase txn, string key) {
            var WP_LOT_CHECKLIST_ITEM = txn.EFQuery_MES.WP_LOT_CHECKLIST_ITEM.IQueryable_ACTION_LINK_SID(key);
            var WP_LOT_CHECKLIST_TAG_ITEM = (from a in txn.EFQuery_MES.WP_LOT_CHECKLIST_TAG_ITEM
                .Where(c => WP_LOT_CHECKLIST_ITEM.Any(c1 => c1.WP_CHECKLIST_ITEM_SID == c.WP_CHECKLIST_ITEM_SID))
                                             select a
                ).ToList();

            var CHECKLIST = new
            {
                WP_LOT_CHECKLIST_ITEM = WP_LOT_CHECKLIST_ITEM.ToList(),
                WP_LOT_CHECKLIST_TAG_ITEM,
                //FC_CHECKLIST_EDC_ROW = txn.EFQuery_MES.FC_CHECKLIST_EDC_ROW.GetData_ACTION_LINK_SID(key),
            };
            return CHECKLIST;
        }


   
        public static dynamic trc_UserTraceIn(ITxnBase txn, string key)
        {
            var WP_USER_TRACE_IN = txn.EFQuery_MES.WP_USER_TRACE_IN.IQueryable_ACTION_LINK_SID(key);
            var WP_USER_TRACE_IN_EQP = (from a in txn.EFQuery_MES.WP_USER_TRACE_IN_EQP
                .Where(c => WP_USER_TRACE_IN.Any(c1 => c1.IN_SUPPORT_SID == c.WP_USER_TRACE_IN_SID))
                        select a
                ).ToList();

            var CHECKLIST = new
            {
                WP_USER_TRACE_IN = WP_USER_TRACE_IN.ToList(),
                WP_USER_TRACE_IN_EQP,
                //FC_CHECKLIST_EDC_ROW = txn.EFQuery_MES.FC_CHECKLIST_EDC_ROW.GetData_ACTION_LINK_SID(key),
            };
            return CHECKLIST;
        }
             */
        public static dynamic trc_TOOL(ITxnBase txn, string key)
        {
            var WP_TOOL_TRACE = txn.EFQuery_MES.WP_TOOL_TRACE.IQueryable_ACTION_LINK_SID(key);
            return new
            {
                FC_TOOL = txn.EFQuery_MES.FC_TOOL.Where(c => WP_TOOL_TRACE.Any(c1 => c1.TOOL_SID == c.TOOL_SID)).AsNoTracking().ToList(),
                WP_TOOL_TRACE = WP_TOOL_TRACE.ToList(),
                WP_EQP_TOOL_LIST = txn.EFQuery_MES.WP_EQP_TOOL_LIST
                         .Where(c => c.UPDATE_DATE == txn.ExeTime)
                         .ToList()
            };
        }
        

        public static dynamic trc_EQP(ITxnBase txn, string key)
        =>new {
            //WP_EQP = txn.EFQuery_MES.view_EqpExt().Where(c=>c.Main.UPDATE_DATE == txn.ExeTime).ToList(),
            WP_LOT_EQP_TRACE = txn.EFQuery_MES.WP_LOT_EQP_TRACE.Where(c => c.UPDATE_DATE == txn.ExeTime).ToList(),
            WP_EQP_TRACE = txn.EFQuery_MES.WP_EQP_TRACE.GetData_ACTION_LINK_SID(key)
        };


        public static dynamic trc_EDC(ITxnBase txn, string key)
        {
            var _WP_LOT_EDC = txn.EFQuery_MES.WP_LOT_EDC.IQueryable_ACTION_LINK_SID(key);
            
            return new {
                WP_LOT_EDC = _WP_LOT_EDC.ToList(),
                WP_LOT_EDC_ROW = txn.EFQuery_MES.WP_LOT_EDC_ROW.Where(c => _WP_LOT_EDC.Any(c1=>c1.LOT_EDC_SID == c.LOT_EDC_SID)).ToList(),
                //ZZ_MPI_EDC_LOG = txn.EFQuery_MES.ZZ_MPI_EDC_LOG.IQueryable_ACTION_LINK_SID(key),
            };
        }

        public static dynamic trc_ADLog(ITxnBase txn)
        =>new {
            AD_LOG = txn.EFQuery_MES.AD_LOG.Where(c => c.VALUE_LINK_SID == txn.LinkSID || c.CREATE_DATE == txn.ExeTime).ToList(),
            AD_LOG_VALUE = txn.EFQuery_MES.AD_LOG_VALUE.Where(c => c.VALUE_LINK_SID == txn.LinkSID || c.CREATE_DATE == txn.ExeTime).ToList()
        };


        public string UItest = null;

        internal static bool isUItest(HttpRequestBase Req)
        {
            if (Req != null) {
                var _UItest = Req.Params["UItest"] ?? "";
                var _path = Req.RequestContext.HttpContext.Server.MapPath($"~/Areas/Example/Views/Self/UITest/{_UItest}.json");
                return System.IO.File.Exists(_path);
            }
            return false;
        }

        public IHtmlString param_UItest(string key = "dataModel")
        {
            var _code = "";
            if (!string.IsNullOrWhiteSpace(UItest)) {
                var _path = this._Req.RequestContext.HttpContext.Server.MapPath($"~/Areas/Example/Views/Self/UITest/{UItest}.json");
                if (System.IO.File.Exists(_path)) {
                    var UItest_Code = System.IO.File.ReadAllText(_path);
                    if (!string.IsNullOrWhiteSpace(UItest_Code)) {
                        _code = $"{key} = {UItest_Code};";
                    }
                }
            }
            return Test(_code);
        }

        //改用
        //public IHtmlString param_UItest_20240310(string key = "dataModel")
        //{
        //    var _code = "";
        //    if (string.IsNullOrWhiteSpace(_UItest) == false)
        //    {
        //        string Baseurl = $"http://localhost:59394/GenesisNewMes/Example/Self/UITest?name={_UItest}";
        //        HttpClient client = new HttpClient();
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var r = client.GetAsync(Baseurl).Result;
        //        var res = r.Content.ReadAsStringAsync().Result;
        //        if (r.IsSuccessStatusCode) { 
        //            //var _o = JsonConvert.DeserializeObject(res);// res.Replace("\"", "");
        //            _code = $"{key} = {res};";//JSON.parse({_o.ToJson()});
        //        }
        //    }
        //    return Test(_code, 1);
        //}

        public static T param_UItest1<T>(HttpRequestBase Req = null)
        {
            T _obj = default(T);
            if (Req != null) {
                var _UItest = Req.Params["UItest"] ?? "";
                if (_UItest != "") {
                    var _path = Req.RequestContext.HttpContext.Server.MapPath($"~/Areas/Example/Views/Self/UITest/{_UItest}.json");
                    if (System.IO.File.Exists(_path)) {
                        var UItest_Code = System.IO.File.ReadAllText(_path);
                        if (!string.IsNullOrWhiteSpace(UItest_Code)) {
                            _obj = UItest_Code.ToObject<T>();
                        }
                    }
                }
            }
            return _obj;
        }

        public IHtmlString mounted { get; set; }
        public IHtmlString methods { get; set; }


        public string Debug { get; set; } = "";
        public IHtmlString param_test { get { return Test(param_test_code); } }
        public static string param_test_code = @"
            param.isTest = 'T';
            var _obj = $('.red_mark');
            if (_obj.length >= 1 && _obj.css('color') != 'rgb(255, 0, 0)'){
                param.isTest = 'F'
            }
            console.log(param);
        ";

        /// <summary>
        /// 千萬不要再用@GTest.Test(GTI_Test.code_新增料號) , 轉成正式時會出錯
        /// </summary>
        public static string code_新增料號 = @"
            param.PARTNO_SID = null;
            param.PARTNO_VER_SID = null;
        ";

        public IHtmlString Test(string code, int mode = 0)
        =>GTI_Test.Test(htm, code, mode);

        public static IHtmlString Test(HtmlHelper htm ,string code, int mode = 0)
        {
            switch (mode) {
                case 1:
                    code = $@"
                    var _obj = $('.red_mark.pass');
                    if (_obj.length == 0 ){{
                    {code}
                    }}
                    ";
                    break;
                case 2:
                    code = $@"<script src='{VirtualPathUtility.ToAbsolute("~/MSW/msw-loader.js")}' data-page='{code}'></script>";
                    break;
                case 3:
                    code = $@"
                    var _re = confirm('{code}');
                    if (_re == '' ){{
                        alert('success!');
                        return;
                    }}";
                    break;
                case 9:
                    code = red_mark(code);
                    break;

                //千萬不要再用@GTest.Test(GTI_Test.code_新增料號) , 轉成正式時會出錯
                case 9001:
                    code = code_新增料號;
                    break;
            }
            
            //if (mode != 0 && GTI_Test.IsDebuggingEnabled) return htm.Raw(code);
            return htm.Raw(isTest ? code : "");
        }

        public static string red_mark(string tar)
        {
            if (tar == "") tar = "i.fa.fa-play";
            return  $@"
            $('<style>')
              .prop('type', 'text/css')
              .html(`
                {tar}.red_mark {{
                    color: rgb(255, 0, 0);
                }}
                {tar}.red_mark.pass {{
                    color: inherit;
                }}
                `).appendTo('head');

            $('{tar}').addClass('red_mark')
                .on('click', function (e) {{
                    var t = $('{tar}'), k = 'pass';
                    var z = t.hasClass(k)
                        ? t.removeClass(k)
                        : t.addClass(k)
                        ;
                    e.stopPropagation();
                }});
            ";
        }

        public static void Exec(string ActName, string Key) { }


        public static void SendMail(string html){
            var message = new MailMessage();
            message.From = new MailAddress("a0982830615@gmail.com", "thony");
            message.To.Add("anthony_lin@genesis.com.tw");
            message.Subject = "Test";
            message.Body = html;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true; // ✅ 必開
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new NetworkCredential("a0982830615@gmail.com", "bhcp etwd yybg qkqb");
                smtp.Timeout = 10000; // 設定 timeout，避免卡死
                smtp.Send(message);
            }
        }
    }
    //todo-bk
    /*
    MapFunName
    Connection_PD_GTIMES
    CarrierCheckIn.cshtml
    CarrierCheckOut.cshtml
    */
    

    public class DatabaseHelper
    {
        private static readonly TraceSource traceSource = new TraceSource("DatabaseTraceSource");

        public void ExecuteDatabaseOperation()
        {
            var ConStr = ConfigurationManager.ConnectionStrings["sql.mes"].ConnectionString;
            using (var connection = new SqlConnection(ConStr))
            {
                try
                {
                    traceSource.TraceEvent(TraceEventType.Information, 0, "Opening connection...");
                    connection.Open();
                    // Perform database operations
                }
                catch (Exception ex)
                {
                    traceSource.TraceEvent(TraceEventType.Error, 0, $"Exception: {ex.Message}");
                }
                finally
                {
                    traceSource.TraceEvent(TraceEventType.Information, 0, "Closing connection...");
                }

                using (var connection1 = new SqlConnection(ConStr))
                {
                    try
                    {
                        traceSource.TraceEvent(TraceEventType.Information, 0, "Opening connection...");
                        connection1.Open();
                        // Perform database operations
                    }
                    catch (Exception ex)
                    {
                        traceSource.TraceEvent(TraceEventType.Error, 0, $"Exception: {ex.Message}");
                    }
                    finally
                    {
                        traceSource.TraceEvent(TraceEventType.Information, 0, "Closing connection...");
                    }
                }
            }
        }
    }


    public static partial class ext {
        //public static bool? ts_BoolNull(this int? _self)
        //{
        //    if (_self == null) return null;
        //    return _self != 0;
        //}

        public static bool isEnable(this int _self)
        => _self != 0;


        public static IQueryable<T> IQueryable_ACTION_LINK_SID<T>(this IQueryable<T> queryable, string key) where T : class
        => IQueryable_Col(queryable, "ACTION_LINK_SID", key);

        public static IQueryable<T> IQueryable_Col<T>(this IQueryable<T> queryable, string ColName , string key) where T : class
        {
            // 獲取要查詢的類型
            var entityType = typeof(T);

            // 獲取 ACTION_LINK_SID 屬性
            var property = entityType.GetProperty(ColName);
            if (property == null)
            {
                throw new InvalidOperationException($"Type {entityType.Name} does not contain a property named ACTION_LINK_SID");
            }

            // 生成表達式樹
            var parameter = Expression.Parameter(entityType, "c");
            var propertyAccess = Expression.Property(parameter, property);
            var constant = Expression.Constant(key);
            var equal = Expression.Equal(propertyAccess, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            // 執行查詢
            return queryable.Where(lambda).AsNoTracking();
        }

        public static List<T> GetData_ACTION_LINK_SID<T>(this IQueryable<T> queryable, string key) where T : class
        => queryable.IQueryable_Col<T>("ACTION_LINK_SID",key).ToList();
        //=> queryable.IQueryable_ACTION_LINK_SID(key).ToList();

        //public static IQueryable<object> SelectLotInfo<T>(this IQueryable<T> query, params string[] properties)
        //    where T : class
        //{
        //    var selectClause = string.Join(", ", properties);
        //    return (IQueryable<object>)query.Select($"new ({selectClause})");
        //}


        public static IEnumerable<Dictionary<string, object>> SelectLotInfo_1<T>
            (this IQueryable<T> query, Dictionary<string, string> propertyMappings)
        where T : class
        {
            return query.AsEnumerable().Select(entity =>
            {
                var result = new Dictionary<string, object>();
                var entityType = typeof(T);

                foreach (var mapping in propertyMappings)
                {
                    var sourcePropertyName = mapping.Value;
                    var targetPropertyName = mapping.Key;

                    var propertyInfo = entityType.GetProperty(sourcePropertyName);
                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(entity);
                        result[targetPropertyName] = value;
                    }
                    // 如果找不到對應的欄位，則直接略過
                }

                return result;
            });
        }

        public static IEnumerable<Dictionary<string, object>> SelectLotInfo_2<T>(this IQueryable<T> query, string[] propertyNames, params string[] fixedProperties)
                where T : class
        {
            
            return query.AsEnumerable().Select(entity =>
            {
                var result = new Dictionary<string, object>();
                var entityType = typeof(T);

                // 添加动态选择的字段
                foreach (var propertyName in propertyNames)
                {
                    var propertyInfo = entityType.GetProperty(propertyName);
                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(entity);
                        result[propertyName] = value;
                    }
                    // 如果找不到对应的字段，则直接跳过
                }

                // 添加固定字段
                foreach (var propertyName in fixedProperties)
                {
                    var propertyInfo = entityType.GetProperty(propertyName);
                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(entity);
                        result[propertyName] = value;
                    }
                    // 如果找不到对应的字段，则直接跳过
                }

                return result;
            });
        }

        public static List<Dictionary<string, object>> SelectLotInfo_3<T>(
       this IQueryable<T> query,
       string[] propertyNames,
       params string[] fixedProperties) where T : class
        {
            // 先將資料取回記憶體 (避免 EF 無法解析 Dictionary)
            var list = query.ToList();

            // 取得所有需要的欄位 (去除重複)
            var selectedProperties = propertyNames.Concat(fixedProperties).Distinct();
            var entityType = typeof(T);

            return list.Select(entity =>
            {
                var result = new Dictionary<string, object>();

                foreach (var propertyName in selectedProperties)
                {
                    var propertyInfo = entityType.GetProperty(propertyName);
                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(entity);
                        result[propertyName] = value;
                    }
                }

                return result;
            }).ToList();
        }

 

    public static List<dynamic> SelectLotInfo_4<T>(this IQueryable<T> query,
    string[] propertyNames,
    params string[] fixedProperties) where T : class
        {
            // 獲取要查詢的類型
            var entityType = typeof(T);

            // 構建選擇表達式
            var parameter = Expression.Parameter(entityType, "c");
            var bindings = new List<MemberBinding>();

            // 動態構建匿名類型的屬性
            var anonymousTypeProperties = new List<MemberBinding>();

            foreach (var propertyName in propertyNames)
            {
                var property = entityType.GetProperty(propertyName);
                if (property == null)
                {
                    throw new InvalidOperationException($"Type {entityType.Name} does not contain a property named {propertyName}");
                }

                var propertyAccess = Expression.Property(parameter, property);
                var binding = Expression.Bind(property, propertyAccess);
                anonymousTypeProperties.Add(binding);
            }

            // 創建匿名類型的選擇器
            var selector = Expression.Lambda<Func<T, object>>(
                Expression.MemberInit(Expression.New(typeof(object)), anonymousTypeProperties),
                parameter
            );

            // 執行 select 操作
            return query.Select(selector).AsNoTracking().ToList();
        }













        public static IQueryable<PF_ROUTE_VER_OPER> f_Oper_找出關聯流程
           (this MESContext _self
           , string OperNo)
        => from a in _self.PF_ROUTE_VER_OPER
           where a.OPERATION_NO == OperNo
           select a;

        public static IQueryable<WP_LOT> f_Oper_找出一般批號
           (this MESContext _self
           , string OperNo)
        => from a in _self.WP_LOT
           where _self.PF_ROUTE_VER_OPER
               .Any(c => c.OPERATION_NO == OperNo
               && a.OPER_SID == c.OPER_SID)
           select a;

        public static LotInfo to_LotInfo
           (this WP_LOT _self
           , ITxnBase _Txn)
        => _Txn.GetLotInfo(_self.LOT_SID);

       
    }


 
    public static class GTCode
    {
        public static MvcHtmlString GTICode(this HtmlHelper Html, string path)
        {
            var _path = $"~/Areas/Example/Views/GTICode/{path}.cshtml";
            return Html.Partial(_path);
        }
        /// <summary>
        /// 以文字 方式呈現
        /// </summary>
        /// <param name="src"></param>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static IHtmlString t(this string src, HtmlHelper Html)
        {
            return Html.Raw(src);
        }

        /// <summary>
        /// 以路徑 做 Partial
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static IHtmlString p(this string path, HtmlHelper Html)
        {
            var _path = $"~/Areas/Example/Views/GTICode/{path}.cshtml";
            return Html.Partial(_path);
        }

        /// <summary>
        /// 單純只是 Mark
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string m(this int src, HtmlHelper Html = null)
        {
            return "";
        }
        public static string m(this string src, HtmlHelper Html = null)
        {
            return "";
        }

        /// <summary>
        /// 0)null  1)true 2)false
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool? ts_NullBool(this int? src)
        {
            if (src != null && src != 0) return (int)src == 1;
            return (bool?)null;
        }

        /// <summary>
        /// 只有 1 才會為 true , 其餘全為 false
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool ts_Bool(this int? src)
        => src == 1;

        /// <summary>
        /// 只要不為 0
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool isAction(this int src)
        => src != 0;

        public static string ts_NullString(this string src)
        {
            if (src == null || src == "string") return null;
            return src;
        }

        public static bool ts_Bool(this int src)
        => src == 1;

        public static int? ts_NullInt(this int src)
        => src == 0 ? null : (int?)src;

        public static Nullable<T> ts_NullEnum<T>(this int src) where T : struct
        {
            if (src == 0) return null;
            var z = (T)Enum.Parse(typeof(T), src.ToString());
            return (Nullable<T>)z;
        }

        public static T ts_Enum<T>(this int src) where T : struct
        => (T)Enum.Parse(typeof(T), src.ToString());
    }



    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class GTI_TestAPIAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public string DynamicProcessName { get; set; } // 新增屬性

        public GTI_TestAPIAttribute(string dynamicProcessName = null) //新增建構子
        {
            DynamicProcessName = dynamicProcessName;
        }
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext filterContext)
        => GTI_Test.dyn_TxnBase_T(DynamicProcessName);
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class GTI_TestAttribute : ActionFilterAttribute
    {
        public string DynamicProcessName { get; set; } // 新增屬性

        public GTI_TestAttribute(string dynamicProcessName = null) //新增建構子
        {
            DynamicProcessName = dynamicProcessName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        => GTI_Test.dyn_TxnBase_T(DynamicProcessName);

    }

    public static class GTI_Code
    {

        /*
        var action = ViewContext.RouteData.Values["action"].ToString();
        var controller = ViewContext.RouteData.Values["controller"].ToString().ToUpper();
    
        GTI_Test GTest = new GTI_Test
	        (Html, HttpContext.Current.IsDebuggingEnabled
	        , ViewContext.HttpContext.Request);
    
        ViewBag.Title = Genesis.Common.PageBaseInfo.getCurrentResource(action).RESOURCE_NAME;

        bool isSingleModel = (bool)(ViewData["SingleModel"]??false);
	    @GTest.param_test
	    @GTest.Test("console.log({ param });")
	    
        @GTest.Test(".el-icon-caret-right", 9) 
        */
        public static int _前置頁面;
    }
}


namespace Genesis.Areas.Example.Controllers
{
    public partial class SelfController : BaseController
    {
        [AllowAnonymous]
        public ActionResult test(string name, bool SingleModel = true)
        {
            dynamic data = new ExpandoObject();
            ViewData["SingleModel"] = SingleModel;
            return View(name);
        }
         
        [HttpPost]
        [HandlerAjaxOnly]
        //public ActionResult ListLotEDC_MPI([ModelBinder(typeof(JsonModelBinder<PagerQuery>))] PagerQuery pager)
        public ActionResult PagerQuery(BLL.DataViews.Res.PagerQuery pager)
        => _Content(o => new Result("測試資料送出") { Code = "991", Data = pager });
        

        [AllowAnonymous]
        public async Task<ActionResult> SendEmail(string name)
        {
            //var url = "http://localhost:59394/CUBMES/Example/Self/test?name=_Mail";
            string html = await RenderViewToStringAsync(ControllerContext, name, new { });
            using (var httpClient = new HttpClient())
            {
                //var html = await httpClient.GetStringAsync(url);

                var message = new MailMessage();
                message.From = new MailAddress("anthony_lin@genesis.com.tw", "thony");
                message.To.Add("a0982830615@gmail.com");
                message.Subject = "Order Confirmation";
                message.Body = html;
                //message.Body = "<h3>這是測試信</h3><p>使用 Gmail + App Password 寄送成功！</p>";
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true; // ✅ 必開
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;

                    smtp.Credentials = new NetworkCredential("a0982830615@gmail.com", "bhcp etwd yybg qkqb");
                    smtp.Timeout = 10000; // 設定 timeout，避免卡死
                    await smtp.SendMailAsync(message).ConfigureAwait(false);
                }

            }
            return Content($"Mail sented <br>{html}");
        }
         



        private async Task<string> RenderViewToStringAsync(ControllerContext context, string viewName, object model = null)
        {
            context.Controller.ViewData.Model = model;
            using (var sw = new StringWriter()){
                var viewResult = ViewEngines.Engines.FindView(context, viewName, null);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return await Task.FromResult(sw.ToString());
            }
        }


        //public ActionResult QC_INSTRUMENTS_CALIBRATION_RECORDS_PMS_Save(QC_INSTRUMENTS_CALIBRATION_RECORDS form, List<EdcModel> edcData, string isTest = null)
        //=> _Content1(o => Maintain.QC_INSTRUMENTS_CALIBRATION_RECORDS_Save(form, edcData, isTest == "T"));

        public static ExpandoObject GetCurrentMethodParameters(int index = 1)
        {
            // 取得當前執行緒的堆疊框架
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(index); // 1 表示取得呼叫者的堆疊框架

            // 取得呼叫方法的方法資訊
            MethodBase method = stackFrame.GetMethod();

            // 取得傳入參數集合
            ParameterInfo[] parameters = method.GetParameters();

            dynamic parameterValues = new ExpandoObject();

            // 將參數值加入動態物件
            foreach (var parameter in parameters)
            {
                ((IDictionary<string, object>)parameterValues)[parameter.Name] = parameter.DefaultValue;
            }

            return parameterValues;
        }

        public static ExpandoObject GetCurrentMethodParameters(string ActionName)
        {
            StackTrace stackTrace = new StackTrace();
            dynamic parameterValues = new ExpandoObject();
            // 從堆疊中尋找目標呼叫者
            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                StackFrame stackFrame = stackTrace.GetFrame(i);
                MethodBase method = stackFrame.GetMethod();

                // 檢查呼叫者的類型和方法名稱
                if (method.ReflectedType != null &&
                    method.Name == ActionName)
                {
                    ParameterInfo[] parameters = method.GetParameters();


                    // 將參數值加入動態物件
                    foreach (var parameter in parameters)
                    {
                        ((IDictionary<string, object>)parameterValues)[parameter.Name] = parameter.DefaultValue;
                    }

                    break;
                }
            }
            // 取得傳入參數集合
            return parameterValues;
        }

        public dynamic Check_RedirectToCustomAction(bool isNeedExec)
        {
            if (!isNeedExec) return null;

            var controllerContext = ControllerContext;
            string ProjectCustomer = "DAE";// ServicesBase.ProjectCustomer ?? "";
            if (ProjectCustomer == "") return null;

            dynamic Arg = new ExpandoObject();
            string ActionName = controllerContext.RouteData.Values["action"].ToString();
            Arg.CusActionName = $"{ActionName}_{ProjectCustomer}";
            var actionMethod = controllerContext.Controller
                .GetType()
                .GetMethod(Arg.CusActionName);
            if (actionMethod == null) return null;

            var queryParameter = HttpContext.Request.QueryString;
            var routeValues = new RouteValueDictionary();
            foreach (string key in queryParameter)
            {
                routeValues.Add(key, queryParameter[key]);
            }
            Arg.RouteParam = routeValues;
            return Arg;
        }


        public ActionResult _Content1(Func<IResult, IResult> func, bool isCus = false)
        {
            IResult result = new Result(true);
            try
            {
                var Arg = Check_RedirectToCustomAction(isCus);
                if (Arg != null)
                {
                    //return Content(((object) Arg).ToJson(true));
                    return RedirectToAction(Arg.CusActionName, Arg.RouteParam);
                }
                result = func(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.Data = ex.Data;
                Logger.Error(ex.Message, ex);
            }

            return Content(result.ToJson(true));
        }

        [AllowAnonymous]
        public ActionResult test1(string name, bool SingleModel = true)
        => _Content1(o => new Result(true) { Data = "Test1" }, true);

        [AllowAnonymous]
        public ActionResult test1_DAE(string name, bool SingleModel = true)
        => _Content(o => new Result(true) { Data = new { name, SingleModel } });

        [AllowAnonymous]
        /// <summary>
        /// 針對 進出站的控件做 測試
        /// </summary>
        /// <param name="name"></param>
        /// <param name="SingleModel"></param>
        /// <returns></returns>
        public ActionResult InOut(string name, string Case = "0", bool SingleModel = true)
        {
            ViewData["SingleModel"] = SingleModel;
            //var _data = ControllerContext.HttpContext.Server.MapPath($"../../Areas/example/Views/Act/InOut/~Case{Case}.json");
            //string text = System.IO.File.ReadAllText(_data);
            //ViewData["result"] = JsonConvert.DeserializeObject(text);

            return View($"InOut/{name}", new LotData());
        }

        public ActionResult SignalR_Item(string id = null)
        {
            //ViewData["Count"] = Hubs.UserCountHub._Users.Count.ToString();
            return View("SignalR/Item");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UITest(string name)
        {
            var _path = Server.MapPath($"~/Areas/Example/Views/Self/UITest/{name}.json");
            var code = System.IO.File.ReadAllText(_path);
            return Content(code);
        }
    }
    public partial class ActController : BaseController
    {


        public ActionResult Code(string name, bool SingleModel = false)
        {
            ViewData["SingleModel"] = SingleModel;
            return View($"Code/{name}");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult TestEDC(string file = "EDC_API")
        {
            var _data = ControllerContext.HttpContext.Server.MapPath($"../../Areas/example/Views/Self/UITest/{file}.json");
            string text = System.IO.File.ReadAllText(_data);

            var EdcLog = JsonConvert.DeserializeObject<object>(text);
            var _r = new Result(true)
            {
                Data = EdcLog
            };
            return Content(_r.ToJson(true));
        }

    }

    public class ChatConnection : PersistentConnection
    {
        private static int _connections = 0;

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            Interlocked.Increment(ref _connections);
            //廣播訊息
            Connection.Broadcast("新的連線加入，連線ID：" + connectionId + ",已有連線數：" + _connections);
            return Connection.Send(connectionId, "雙向連線成功，連線ID：" + connectionId);
        }

        /// <summary>
        /// 連線斷開 
        /// </summary>
        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            Interlocked.Decrement(ref _connections);
            return Connection.Broadcast(connectionId + "退出連線，已有連線數：" + _connections);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            var message = connectionId + "傳送內容>>" + data;
            return Connection.Broadcast(message);
        }
    }
}
namespace Genesis.Areas.DDD.Controllers
{

    public class DDDAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DDD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DDD_default",
                "DDD/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
            //context.EnableCors();
        }
    }

    //*
    // RoutePrefix 在 DDDAreaRegistration 下,沒有作用
    //[RoutePrefix("DDD/ApiTest")]
    //[Route("DDD/ApiTest")]
    [SwaggerAuthorizationFilterAttribute]
    public class CaseController : System.Web.Http.ApiController
    {


        [Route("DDD/ApiTest/T001")]
        [System.Web.Http.HttpGet]
        public List<string> T001()
        => new List<string>() { "T01" };

        //      [Route("DDD/ApiTest/T003/{T003}/Test")]
        //      [System.Web.Http.HttpGet]
        //      public List<string> T003(string T003)
        //      => new List<string>() { T003 };


        //[HttpGet]
        //public List<SelectModel> T002()
        //{
        //    throw new Exception();
        //    return new List<SelectModel>();
        //}

    }

    public class CaseAPIController : System.Web.Http.ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("T002")]
        public dynamic T002()
        {
            throw new Exception("test");
            return "";
        }

        /// <summary>
        /// 測試自定義的 Route
        /// </summary>
        /// <returns></returns>
        /// 一但設定 Route ,程序就會改套用 Route 做為 Url
        [System.Web.Http.Route("T001")]
        public List<string> T001()
        => new List<string>() { "T01" };

        public List<string> Get()
        => new List<string>() { "T01" };

        public string Get(string id)
        => id;

        // POST: api/Products
        public NetHttp.HttpResponseMessage Post(string id)
        {
            var obj = new { id = id };
            var response = new NetHttp.HttpResponseMessage(System.Net.HttpStatusCode.Created);
            /*
            在 RESTful API 中，資源的狀態是由 URI 定義的。當成功創建一個資源後，
            伺服器應該返回一個 201 Created 的狀態碼，以及一個 Location 標頭，
            指向新創建的資源的 URI ,
            如果要完成前述的需求 ,可搭配如下程序
             */
            //response.Headers.Location = new Uri(Url.Link("DefaultApi"
            //	, obj
            //	));
            response.Content = new NetHttp.StringContent(obj.ToJson(true), Encoding.UTF8, "application/json");
            return response;
        }
    }

    [EnableCors(origins: "http://example.com, http://localhost:3000", headers: "*", methods: "GET, POST")]
    [SwaggerAuthorizationFilterAttribute]
    [RoutePrefix("DDD/DBA")]
    public class DBAController : System.Web.Http.ApiController
    {
        DBController _dbc;
        internal DBController DBC
        {
            get
            {
                if (_dbc == null)
                {

                    var Conn = ConfigurationManager.ConnectionStrings["sql.mes"];
                    _dbc = new DBController(Conn);
                }
                return this._dbc;
            }
        }

        //[Route("IP/{IP}")]
        //public ActionResult LotInfo(string IP = "51")
        //=> _Content((o) => {
        //	//var connectionString = $"Server=10.96.1.{IP};Database=myDatabase;User Id=myUser;Password=myPassword;";

        //	//using (var connection = new SqlConnection(connectionString)){
        //	//	// 查詢所有資料庫
        //	//	var sql = "SELECT name FROM sys.databases;";
        //	//	var databases = connection.Query<string>(sql).ToList();

        //	//	// 查詢特定資料庫中的表
        //	//	sql = "SELECT name FROM sys.tables WHERE SCHEMA_NAME(schema_id) = 'dbo'";
        //	//	var tables = connection.Query<string>(sql, new { database = "myDatabase" }, commandType: CommandType.Text).ToList();
        //	//}
        //	return null;
        //});

        /*
        ~\Genesis_MVC\Common\LogActionFilterAttribute.cs 
            skipAction.Add("NeedUpdateAuthMenus");
            skipAction.Add("GetResource");
            skipAction.Add("Dashboard_vue");
         */
        //[EnableCors(origins: "http://allowed-origin.com", headers: "*", methods: "GET")]
        [EnableCors(origins: "http://example.com, http://localhost:3000, http://127.0.0.1:3000", headers: "*", methods: "GET, POST")]
        [System.Web.Http.AllowAnonymousAttribute]
        [AllowAnonymous]
        [System.Web.Http.Route("DDD/DBA/Table")]
        [System.Web.Http.Route("DDD/DBA/Table/{Table}")]
        public dynamic GetResource(string Table = null,string format = null)
        {
            using (_dbc ?? DBC)
            {
                var sql_table_list = @"
                SELECT name
                FROM sys.tables;
                ";

                var sql_table_schema = $@"
                    SELECT 
		                    c.name AS Filed,
		                    ISNULL(p.value, '') AS [Desc],
		                    t.Name AS Type,
		                    c.max_length/2 AS Length,
		                    IIF(c.is_nullable=0,'N','Y') AS abeNull
                    FROM  sys.columns c
			                    INNER JOIN  sys.types t 
				                    ON c.user_type_id = t.user_type_id
			                    LEFT OUTER JOIN sys.extended_properties p 
				                    ON p.major_id = c.object_id AND p.minor_id = c.column_id
                    WHERE 
		                    OBJECT_NAME(c.object_id) = '{Table}'
                ";
                var _sql = Table == null ? sql_table_list : sql_table_schema;
                switch(format){ 
                    case "ai":
                        _sql = $@"
                        SELECT '| 欄位名稱 (Column) | 資料型態 (Type) | NULL | 說明 (Description) |' AS [Markdown_Output]
                            UNION ALL
                            SELECT '| :--- | :--- | :--- | :--- |'

                            UNION ALL

                            -- 2. 產生 Markdown 表格 Rows
                            SELECT 
                                '| `' + c.name + '` |    `' + 
                                ty.name + 
                                CASE 
                                    WHEN ty.name IN ('varchar', 'nvarchar', 'char', 'nchar') THEN '(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE CAST(c.max_length AS VARCHAR) END + ')'
                                    WHEN ty.name IN ('decimal', 'numeric') THEN '(' + CAST(c.precision AS VARCHAR) + ',' + CAST(c.scale AS VARCHAR) + ')'
                                    ELSE ''
                                END + '` | ' + 
                                CASE WHEN c.is_nullable = 1 THEN 'Yes' ELSE 'No' END + ' | ' + 
                                ISNULL(CAST(ep.value AS NVARCHAR(MAX)), '') + 
                                CASE WHEN i.is_primary_key = 1 THEN ' (PK)' ELSE '' END + ' |'
                            FROM sys.tables t
                            INNER JOIN sys.columns c ON t.object_id = c.object_id
                            INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
                            LEFT JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                            LEFT JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id AND i.is_primary_key = 1
                            LEFT JOIN sys.extended_properties ep ON ep.major_id = t.object_id AND ep.minor_id = c.column_id AND ep.name = 'MS_Description'
                            WHERE t.name = '{Table}'
                        ";
                        DataTable _dt = _dbc.Select(_sql);
                        //把_dt 的資料 join 成字串
                        var _md = _dt.Rows.Cast<DataRow>().Select(r => string.Join(" ", r.ItemArray)).ToArray();
                        var _res = string.Join("\n", _md);

                        var response = new NetHttp.HttpResponseMessage(System.Net.HttpStatusCode.OK);
                        response.Content = new NetHttp.StringContent(_res, Encoding.UTF8, "text/plain");
                        return response;

                        break;
                    default:
                        break;
                }

                return _dbc.Select(_sql);

            }
        }
    }
    [RoutePrefix("DDD/Wafer")]
    public class WaferController : BaseController
    {
        [Route("LotInfo")]
        [Route("LotInfo/{LotSID}")]
        public ActionResult LotInfo(string LotSID = null, string Lot = null)
        => _Content((o) => Wafer_Services.QueryLotInfo(LotSID, Lot));

        ///*
        //因為無法處理 SN_ID 有帶 小數 - _ 等字符的問題,所以只採用這種方式 
        //*/
        public ActionResult ID(string SN_ID)
        => _Content(o => Wafer_Services.WaferInfo(SN_ID));
    }

    [RoutePrefix("DDD/ADM")]
    public class ADMController : BaseController
    {
        [Route("Test")]
        public ActionResult Test()
        {
            //var context = GlobalHost.ConnectionManager.GetHubContext<GTiHub>();
            //if (string.IsNullOrWhiteSpace(connectionIds))
            //{
            //    context.Clients
            //           .All
            //           .ShowMessage(name, country);
            //}
            //else
            //{
            //    //不支援多筆
            //    context.Clients
            //           .Clients(new List<string>
            //           {
            //       connectionIds
            //           })
            //           .ShowMessage(name, country);
            //}
            IResult result = new Result(true);
            return Content(result.ToJson());
        }

        /// <summary>
        /// Fix 程序,只要 帶入 AD_FUNCTION.FUN_SID 即可
        /// </summary>
        /// <param name="FUN_NAME"></param>
        /// <param name="FUN_SID"></param>
        /// <param name="FUN_URL"></param>
        /// <param name="FUN_FILE_NAME"></param>
        /// <returns></returns>
        [Route("Reason")]
        [Route("Reason/{FUN_NAME}")]
        [Route("Reason/{FUN_NAME}/{FUN_SID}")]
        public ActionResult Reason(string FUN_NAME, string FUN_SID, string FUN_URL = null, string FUN_FILE_NAME = null)
        => _Content((o) => DDLServices.Reason(FUN_NAME, FUN_SID, FUN_URL, FUN_FILE_NAME));
    }

    [RoutePrefix("DDD/APP")]
    public class APPController : BaseController
    {
        private readonly string[] _localizationFiles = new[]
        {
            "Face.zh-TW.resx",
            "Message.zh-TW.resx"
        };

        [Route("i18n/Search/{keyword}")]
        [Route("i18n/Search/{keyword}/{project}")]
        public ActionResult i18nSearch(string keyword, string project = null)
        => _Content(o =>
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            foreach (var file in _localizationFiles)
            {
                    //var results = new Dictionary<string, string>();
                    var _path = System.Web.HttpContext.Current.Server.MapPath("~/");
                var fileName = $"{_path}/../../Library/RES/BLL/{file}";

                if (project != null)
                {
                    var new_dir = project.Replace("_", @":\");
                    var pattern = @"^(.*?):\\(\w*)_";
                    fileName = Regex.Replace(fileName, pattern, $"{new_dir}_");
                }
                XDocument doc = XDocument.Load(fileName);
                var query = from elem in doc.Descendants("data")
                            where elem.Value.Contains(keyword)
                                || elem.Attribute("name").Value.Contains(keyword)
                            select new
                            {
                                Key = elem.Attribute("name").Value,
                                Value = elem.Element("value").Value
                            };
                var results = new Dictionary<string, string>();
                foreach (var item in query)
                {
                    if (results.ContainsKey(item.Key))
                    {
                        results.Add($"{item.Key}~${item.Value}", item.Value);
                    }
                    else
                    {
                        results.Add(item.Key, item.Value);
                    }
                }
                var mainkey = file.Replace(".zh-TW.resx", "");
                result.Add(mainkey, results);
            }
            return new Result(true) { Data = new { result, project } };
        });

        [AllowAnonymous]
        [HttpPost]
        [Route("i18n/Add/{res}/{key}/")]
        public ActionResult i18nAdd(string res, string key, string en, string tw, string cn)
        => _Content(o => I18nAdd(res, key, en, tw, cn));

        IResult I18nAdd(string res, string key, string en, string tw, string cn)
        {
            Type _t = null;
            switch (res.ToUpper())
            {
                case "FACE":
                    _t = typeof(RES.BLL.Face);
                    break;
                case "MESSAGE":
                    _t = typeof(RES.BLL.Message);
                    break;
            }
            if (_t == null) return new Result("查無符合的 BLL.res");
            var rm = new ResourceManager(_t);
            var MatchItem = rm.GetObject(key);
            if (MatchItem != null)
            {
                var r = new Result("Key值己存在");
                r.Data = new { key, MatchItem };
                return r;
            }

            var root = Server.MapPath("~/");
            var tarFile = $@"{root}..\Library\RES\BLL\{res}.resx";
            if (FileHelper.IsExistFile(tarFile) == false)
            {
                return Result.NotExist("語系檔").Data = new { tarFile };
            }

            var is產品語系檔 = root.Substring(0, 2) == "M:";
            if (is產品語系檔 == false)
            {
                var tmp = new { en, tw, cn };
                vFile.WriteAllText($@"P:\MyLab\GTI_Sample\~i18n\{res}_{key}.json", tmp.ToJson(true));
            }

            ResXResourceSet resxSet = new ResXResourceSet(tarFile);
            using (ResXResourceWriter resxWriter = new ResXResourceWriter(tarFile))
            {
                foreach (DictionaryEntry entry in resxSet)
                {
                    var _key = entry.Key.ToString();
                    resxWriter.AddResource(_key, entry.Value);
                }

                resxWriter.AddResource(key, en);
                resxWriter.Generate();
            }




            for (var i = 0; i < 2; i++)
            {
                string val = tw, res_tp = ".zh-TW";
                if (i == 1)
                {
                    val = cn;
                    res_tp = ".zh-CN";
                }
                var tarFile1 = $@"{root}..\Library\RES\BLL\{res}{res_tp}.resx";
                string fileContent = vFile.ReadAllText(tarFile1);
                // 使用正則表達式進行置換
                string pattern = $@"</data>\s*</root>";
                string replacement = $@"</data>
	<data name=""{key}"" xml:space=""preserve"">
		<value>{val}</value>
	</data>
</root>";
                string newContent = Regex.Replace(fileContent, pattern, replacement);
                vFile.WriteAllText(tarFile1, newContent);
            }
            return new Result(true);
        }

        public struct d_i18n
        {
            public string en;
            public string tw;
            public string cn;
        }

        [Route("GTI_Test/t_Process")]
        public ActionResult t_Process()
        => Content(vFile.ReadAllText(GTI_Test.g_path.t_Process));


        [Route("i18n/AutoAdd")]
        public ActionResult AutoAdd()
        => _Content(o =>
        {
            var root = Server.MapPath("~/");
            var is產品語系檔 = root.Substring(0, 2) == "M:";
            if (is產品語系檔 == false) return new Result("目前不是在產品環境");
            string directoryPath = $@"{root}Areas\Example\Views\Self\~i18n\";

            if (Directory.Exists(directoryPath))
            {
                    // 取得目錄中的 JSON 檔案清單
                    string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

                foreach (string jsonFile in jsonFiles)
                {
                    try
                    {
                        var _fileName = Path.GetFileNameWithoutExtension(jsonFile);
                        var arr = _fileName.Split('_');
                            //var res = _fileName[0];
                            //var key = _fileName[1];

                            //$@"{directoryPath}{jsonzFile}"
                            string jsonContent = vFile.ReadAllText(jsonFile);
                            // 解析 JSON 內容到物件
                            var data = JsonConvert.DeserializeObject<d_i18n>(jsonContent);
                        var r = I18nAdd(arr[0], arr[1], data.en, data.tw, data.cn);
                        if (r.Success)
                        {
                            vFile.Move(jsonFile, $@"{directoryPath}{_fileName}.---");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing {jsonFile}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist.");
            }
            return o;
        });

        [Route("i18n/parseTxt")]
        public ActionResult parseTxt()
        => _Content(o =>
        {
            Type _t = null;
            var list = new string[] { "Face", "Message" };
            var root = Server.MapPath("~/");
            List<string> result;
            foreach (var res in list)
            {
                var tarFile = $@"{root}..\Library\RES\BLL\{res}.zh-TW.resx";
                if (FileHelper.IsExistFile(tarFile) == false)
                {
                    return Result.NotExist("語系檔").Data = new { res };
                }
                var resxSet = new ResXResourceReader(tarFile);

                result = new List<string>();
                foreach (DictionaryEntry entry in resxSet)
                {
                    result.Add(entry.Value.ToString());
                }

                var data = string.Join("\n", result);
                var tarFile1 = $@"{root}..\Library\RES\BLL\{res}.txt";
                vFile.WriteAllText(tarFile1, data);
            }
            return o;
        });
        [Route("i18n/parseTxt/callback")]
        public ActionResult callback()
        => _Content(o =>
        {
            Type _t = null;
            var list = new string[] { "Face", "Message" };
            var root = Server.MapPath("~/");
            List<string> result;
            foreach (var res in list)
            {
                var tarFile = $@"{root}..\Library\RES\BLL\{res}-zh-TW.resx";
                var src = $@"{root}..\Library\RES\BLL\{res}.txt";
                if (FileHelper.IsExistFile(tarFile) == false)
                {
                    return Result.NotExist("語系檔").Data = new { res };
                }
                var resxSet = new ResXResourceReader(tarFile);

                result = new List<string>();
                foreach (DictionaryEntry entry in resxSet)
                {
                    result.Add(entry.Value.ToString());
                }

                var data = string.Join("\n", result);
                var tarFile1 = $@"{root}..\Library\RES\BLL\{res}.txt";
                vFile.WriteAllText(tarFile1, data);
            }
            return o;
        });
    }
}
namespace Genesis.Areas.ADM.Controllers
{
    /// <summary>
    /// 系統參數群組維護 
    /// ADM/Parameter
    /// </summary>
    public partial class ParameterGroupController : BaseController
    {
        private ParameterServices _service;
        public ActionResult ParameterGroupData_t(string keyVal)
        {
            dynamic data = new ExpandoObject();
            ParameterGroupViewModel model = null;
            if (keyVal != null) model = _groupService.GetParameterGroupData(keyVal);
            data.form = model ?? new ParameterGroupViewModel() { PARAMETERGROUP_TYPE = "Custom" };
            data.ParameterGroupTypes = _groupService.GetParameterGroupTypes();
            data.AllParameters = _paramService.GetTransferUIParameters();
            var result = new Result(true) { Data = data };

            ViewData["Model"] = result.ToJson(true);
            ViewData["SingleModel"] = true;
            var _view = "~/Areas/Example/Views/Self/ADM/ParameterGroupData.cshtml";
            return View(_view);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PARAMETERGROUP_SID"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        /// Ref 
        /// N:\CUB_Dev\Library\BLL\ADM\ParameterGroupServices.cs
        public ActionResult Add_Item(string PARAMETERGROUP_SID, string Data, string isTest = "F")
        => _Content((c) => {
            var arr = Data.Split('\n');
            var _list = new List<AD_PARAMETER>();
            foreach (string s in arr)
            {
                var _obj = new AD_PARAMETER()
                {
                    PARA_SID = Guid.NewGuid().ToString(),
                    PARAMETER_NO = s,
                    PARAMETER_VALUE = s,
                    PARAMETER_NAME = s,
                    PARAMETER_TYPE = "SystemCode",
                };
                _list.Add(_obj);
            }
            if (_list.Count != 0) return Ext.Add_Item(PARAMETERGROUP_SID, _list, isTest == "T");
            return null;
        });


    }


    public partial class I18NItemController : BaseController
    {
        public ActionResult I18NItemMaster_t(string keyVal)
        {
            var _view = "~/Areas/Example/Views/Self/ADM/I18NItemMaster_T.cshtml";
            return View(_view);
        }
    }

    //public partial class OperationController : AbsDynFuncModuleController
    //{
    //    [Route("ADM/Operation/OperationData/{NO}")]
    //    public ActionResult OperationData_(string NO)
    //    => TxnBase.LzDBQuery(Txn =>
    //    {
    //        var row = Txn.EFQuery_MES.PF_OPERATION
    //            .Where(c => c.OPERATION_NO == NO)
    //            .FirstOrDefault_CheckExists();
    //        return Redirect($"~/ADM/Operation/OperationData?keyVal={row.OPER_SID}");
    //        //return RedirectToAction("OperationData", "Operation", new { keyVal = row.OPER_SID });
    //    });
    //}
}

namespace Genesis.Areas.SYSAdmin.Controllers
{
    public partial class ResourceController : BaseController
    {
        public ActionResult ResourceData_t(string keyVal)
        {
            ViewData["result"] = ResourceServices.Query(keyVal).ToJson(true);
            ViewData["SingleModel"] = true;
            ViewData["mode"] = string.IsNullOrEmpty(keyVal) ? "Add" : "Edit";
            var _view = "~/Areas/Example/Views/Self/ADM/ResourceData_T.cshtml";
            return View(_view);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public ActionResult UpdateExt(DataModel model)
        {
            if (string.IsNullOrEmpty(model.form.SID))
            {
                return Content(ResourceServices.Insert(model).ToJson());
            }
            else
            {
                return Content(ResourceServices.Update(model).ToJson());
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [AllowAnonymous]

        public ActionResult Add_ROLE(string RESOURCE_SID)
        => _Content(c => f_Add_ROLE(RESOURCE_SID));



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Txn"></param>
        /// <param name="RESOURCE_SID"></param>
        /// <returns></returns>
        public static IResult f_Add_ROLE(string RESOURCE_SID)
        => WIPInjectServices.TxnBase.LzDBTrans(null, Txn =>
        {
            //* 因為專案編譯的需求 先 mark 掉
            var role = Txn.EFQuery_MVC.AD_ROLE.Where(c => c.ROLE_NO == "Admin").FirstOrDefault();
            Check.Invalid("AD_ROLE 查無 Admin 帳號", role == null);

            var ROLE_res = new AD_ROLE_RESOURCE()
            {
                SID = Txn.GetSID(),
                ROLE_SID = role.SID,
                RESOURCE_SID = RESOURCE_SID,
                RESOURCE_TYPE = "0"
            };
            var chk = Txn.EFQuery_MVC.AD_ROLE_RESOURCE
            .Where(c => c.RESOURCE_SID == RESOURCE_SID
                && c.ROLE_SID == role.SID)
            .Any();
            if (chk == false)
            {
                Txn.EFQuery_MVC.AD_ROLE_RESOURCE.Add(ROLE_res);
                Txn.EFQuery_MVC.SaveChanges();
            }
            //*/
            return Txn.result;
        });
    }

}
namespace System.Linq
{
    public static partial class _ext
    {
        public static List<T> t_Process<T>
              (this List<T> _self)
        {
            File.WriteAllText(GTI_Test.g_path.t_Process, _self.ToJson());
            return _self;
        }

        public static Dictionary<TKey, TSource> t_Process<TKey,TSource>(this Dictionary<TKey, TSource> _self)
        {
            File.WriteAllText(GTI_Test.g_path.t_Process, _self.ToJson());
            return _self;
        }


        public static IQueryable<T> f_有包含指定工站的流程<T>
          (this IQueryable<T> _self, string Search) where T : MDL.MES.PF_ROUTE_VER_OPER
        => from t in _self
           where (Search == null
                             || t.OPER_SID == Search
                             || t.ROUTE_VER_OPER_SID == Search
                             || t.OPERATION_NO == Search
                             || t.OPERATION == Search
                          )
           select t;



    }
}

//*
//BundleConfig~.cs 中 , 預設是用 System.Web.Mvc , 
//	所以一定要使用全名 -- System.Web.Http.Route , 
//	不然設定不會產生作用
namespace Genesis.WebApi
{
    [System.Web.Http.RoutePrefix("api")]
    public partial class SelfInfoController : System.Web.Http.ApiController
    {
        [HttpPost]
        [System.Web.Http.Route("proxy/splunk")]
        public async Task<System.Web.Http.IHttpActionResult> Forward(string url)
        {
            // 讀取原始請求的 body
            string body = await Request.Content.ReadAsStringAsync();

            using (var client = new NetHttp.HttpClient())
            {
                var content = new NetHttp.StringContent(body, System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", "Splunk 924d5d0a-b863-4023-9769-7df031052e2d");

                var response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
        }


        /*
            因為 swagger 資料格式中 null 只會自動識別為 0 
            所以 為了滿足使用 null 的情形 , 目前設置為 ,參數的前置為 _ 者,
                預設就是 0 為 null , 而 1 為 T , 2 為 F ,
                然後 ,都應使用 ts_NullBool 做為標準轉換,

             */

        public class d_Base
        {
            public int isTest { get; set; } = 1;
            public int? CheckIn { get; set; }
            public string UserNo { get; set; }
            public decimal? Qty { get; set; }
            public string SID { get; set; }
            public string NAME { get; set; }
            public string NO { get; set; }
            public string LOT { get; set; }

            [JsonIgnore]
            public bool _Test
            {
                get { return isTest == 1; }
            }

            public object 查詢EDC群組(ITxnBase txn)
            {
                return null;
                //t_Process();
            }
        }


        public class d_Base_工作站
        {
            public string OPER_NO { get; set; }
            public string OPER_SID { get; set; }


            public PF_OPERATION get_PF_OPERATION(ITxnBase Txn)
            {
                PF_OPERATION oper = null;
                if (string.IsNullOrWhiteSpace(OPER_NO.ts_NullString()) == false)
                {
                    oper = Txn.EFQuery_MES.PF_OPERATION
                        .Where(c => c.OPERATION_NO == OPER_NO)
                        .FirstOrDefault()
                        //.FirstOrDefault_CheckExists(RES.BLL.Face.OPERATION_NO)
                        ;
                    OPER_SID = oper.OPER_SID;
                }
                Check.Invalid("OPER_NO 必須有值", OPER_NO.ts_NullString() == null);
                return oper;
            }

            public dynamic 列舉有開啟的擴展_全部(ITxnBase Txn, d_工作站 data)
            {
                var oper = get_PF_OPERATION(Txn);
                return ts_OperExpandSetting(Txn, oper.OPER_SID, data.Action.列舉有開啟的擴展_全部);
            }

            public List<PF_ROUTE_VER_OPER> 查詢含有此站的流程(ITxnBase Txn){
                var search = OPER_SID ?? OPER_NO;
                return Txn.EFQuery_MES.PF_ROUTE_VER_OPER
                            .f_有包含指定工站的流程(search)
                            .ToList();
            }

            public List<WP_WO> 查詢含有此站的工單(ITxnBase Txn)
            {
                var search = OPER_SID ?? OPER_NO;
                var q = Txn.EFQuery_MES.PF_ROUTE_VER_OPER
                        .f_有包含指定工站的流程(search);
                return (from a in Txn.EFQuery_MES.WP_WO
                    where q.Any(c=>c.ROUTE_VER_SID == a.ROUTE_VER_SID)
                    select a
                    ).ToList();
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="Txn"></param>
            /// <param name="OPER_SID"></param>
            /// <param name="mode">1)過濾出只有設定的部份 2)全部顯示</param>
            /// <returns></returns>
            public dynamic ts_OperExpandSetting(ITxnBase Txn, string OPER_SID, int mode)
            {
                var r = Txn.EFQuery_MES.PF_OPERATION_EXPAND.Where(t => t.OPER_SID == OPER_SID).FirstOrDefault();
                if (r != null)
                {
                    var root = r.SETTING_JSON.ToObject<OperExpandSetting>();
                    switch (mode)
                    {
                        case 2://按原始全部顯示
                            return root;
                            break;
                        case 1://過濾出只有設定的部份
                            var r2 = new Dictionary<string, Dictionary<string, dynamic>>();
                            foreach (var Sub1 in root.GetType().GetProperties())
                            {
                                var key = Sub1.Name;
                                var value = Sub1.GetValue(root);
                                if (value != null)
                                {
                                    var r3 = new Dictionary<string, dynamic>();
                                    foreach (var Sub2 in value.GetType().GetProperties())
                                    {
                                        var key1 = Sub2.Name;
                                        var val1 = Sub2.GetValue(value);
                                        if (checkIsEable(val1))
                                        {
                                            r3.Add(key1, val1);
                                        }
                                    }
                                    r2.Add(key, r3);
                                }
                            }
                            return r2;
                            break;
                    }
                }
                return null;
            }

            private bool checkIsEable(object val1)
            {
                if (val1 is bool) return (bool)val1;

                Type type = val1.GetType();
                // 嘗試取得名為 "enable" 的屬性
                PropertyInfo enableProperty = type.GetProperty("enable");
                if (enableProperty != null)
                {
                    var v = (bool)enableProperty.GetValue(val1);
                    return v;
                }
                return false;
            }

        }
        public class d_批號:d_Base
        {
            public d_Base_批號 ArgLot { get; set; }
            public Act Action;

            public class Act
            {
                public int 查詢過站記錄;
                public int 變更數量;
                public int 拆1成100;
                public int 批次結批;
            }
            public class d_Base_批號
            {
                public void 查詢過站記錄(ITxnBase Txn, d_批號 data)
                {
                    Txn.EFQuery_MES.WP_LOT_HIST
                        .Where(c => c.LOT == data.LOT).ToList()
                        .t_Process();
                }

                public void 變更數量(ITxnBase Txn, d_批號 data)
                {
                    var _obj = Txn.EFQuery_MES.WP_LOT
                        .FirstOrDefault(c => c.LOT == data.LOT);
                    _obj.QUANTITY = data.Qty;
                    //Txn.EFQuery_MES.Update(_obj);
                    Txn.EFQuery_MES.SaveChanges();
                }

                public void 拆1成100(ITxnBase Txn, d_批號 data)
                {
                    var LotInfo = Txn.GetLotInfo(data.LOT, true, isQueryByLotNO: true);
                    var items = new List<QtyItem>() { new QtyItem { Qty = 1 } };
                    var splitLots = LOT_Services.Txn_LotSplit(Txn, items);
                    var _lot = splitLots[0].LOT;
                    var _obj = Txn.EFQuery_MES.WP_LOT
                        .FirstOrDefault(c => c.LOT == _lot);
                    _obj.QUANTITY = data.Qty ?? 100;
                    //Txn.EFQuery_MES.Update(_obj);
                    Txn.EFQuery_MES.SaveChanges();
                    Txn.result.Data = _obj;
                }

                public void 批次結批(ITxnBase Txn, d_批號 data)
                {
                    var lots = data.LOT.Split(';');
                    foreach (var lot in lots)
                    {
                        var lotInfo = Txn.GetLotInfo(lot, isQueryByLotNO: true);
                        if (lotInfo.BATCH_NO != string.Empty)
                        {
                            var oChangeAttr = new WIPTransaction.LotChangeAttributeTxn(lotInfo, "BATCH_NO", lotInfo.BATCH_NO, "");
                            Txn.DoTransaction(oChangeAttr);
                        }

                        var CarrierInfo = lotInfo.GetCurrentCarrierInfo(); //載具資訊
                                                                           //檢查母批是否有載具資訊
                        if (CarrierInfo.IsExist)
                        {
                            var carrierLoad = new CARTransaction.CarrierUnloadLotTxn(CarrierInfo, lotInfo);
                            Txn.DoTransaction(carrierLoad);
                        }
                        var terminateMtrLotTxn = new WIPTransaction.TerminateLotTxn(lotInfo);
                        Txn.DoTransaction(terminateMtrLotTxn);
                        var oEnd = new WIPTransaction.EndOfLotTxn(lotInfo);
                        Txn.DoTransaction(oEnd);
                        process_時間控管(Txn, lotInfo);
                    }
                }

                public void process_時間控管(ITxnBase Txn, LotUtility.LotInfo LotInfo)
                {
                    var userNo = Txn.UserNo;

                    List<IDbCommand> commands = new List<IDbCommand>();
                    var chkFun = new CheckTimeUtility.CheckTimeFunctions(Txn.DBC);

                    var dvCheckTime = chkFun.GetWpChecktimeData(LotInfo.LOT, LotInfo.GetRouteVersionOperationInfo().OPERATION_NO, "T");
                    if (dvCheckTime != null)
                    {
                        var FilterList = new string[] { "CHECK_TYPE='MaxQTime'", "CHECK_TYPE='MinQTime'" };
                        foreach (string filter in FilterList)
                        {

                            dvCheckTime.RowFilter = filter;
                            if (dvCheckTime.Count > 0)
                            {
                                for (int k = 0; k < dvCheckTime.Count; k++)
                                {
                                    List<Column> modifyColumns = new List<Column>();
                                    Column ENABLE_FLAG = new Column("ENABLE_FLAG", "T", "F");
                                    modifyColumns.Add(ENABLE_FLAG);
                                    var tran = new CheckTimeUtility.CheckTimeTransaction();
                                    //commands.AddRange(tran.ModifyTransaction
                                    //    (Txn.DBC, Txn.ApplicationName, dvCheckTime[k]["LOT_TIMECONTROL_SID"].ToString()
                                    //    , LotInfo.LOT, modifyColumns
                                    //    , userNo, Txn.ActionTime));
                                }
                                Txn.DoTransaction(commands);
                            }
                        }
                    }
                }

            }
        }
        [System.Web.Http.AllowAnonymousAttribute]
        [System.Web.Http.Route("批號")]
        public dynamic 批號(d_批號 data)
        => TxnBase.LzDBTrans("Test", Txn => {
            var result = data.Action.Dispatch(data.ArgLot, Txn, data);
            return (result != null) ? (IResult)result : Txn.result;
        });


        public class d_保養單
        {
            public d_Base _Base { get; set; }
            public Act Action;

            public class Act
            {
                public int 刪除保養單;
            }
        }

        



        /*
        [System.Web.Http.AllowAnonymousAttribute]
        [System.Web.Http.Route("批號")]
        public dynamic 批號(d_批號 data)
        =>TxnBase.LzDBQuery<dynamic>(Txn => {
            var lot = (from a in Txn.EFQuery_MES.WP_LOT
                       where a.LOT == data.LOT || a.LOT_SID == data.SID
                       select a).FirstOrDefault_CheckExists();

            if (data.Action.列舉流程版本工站中有開啟的擴展_全部.isAction()){
                dynamic r1 = new ExpandoObject();
                var RouteVerInfo = lot.to_LotInfo(Txn).GetRouteVersionInfo();
                var Opers = RouteVerInfo.GetRouteVersionOperationList();
                var _base_Oper = new d_Base_工作站();
                r1.Opers = Opers;
                r1.Settings = (from a in Opers
                               select new {
                                   a.OPER_SID,
                                   a.OPERATION,
                                   a.OPER_SEQ,
                                   Setting = _base_Oper.ts_OperExpandSetting(Txn, a.OPER_SID, data.Action.列舉流程版本工站中有開啟的擴展_全部),
                               })
                               .Where(c=>c.Setting !=null)
                               .ToList();
                r1.lot = lot;
                return r1;
            }
            else if (data.Action.依據料號流程工站設定取得原因碼.isAction()){
                return DDLServices.GetPartNoOperReasonCodeData_OperSid(lot.LOT, lot.to_LotInfo(Txn));
            }
            else if (data.Action.測試再製品查詢.isAction()){
                return (from w0 in Txn.EFQuery_MES.PF_PARTNO
                        where w0.PARTNO == data.SID
                        select w0
                    ).ToList();
                //return (from w0 in Txn.EFQuery_MES.WP_LOT
                //	join p1 in Txn.EFQuery_MES.PF_PARTNO 
                //		on w0.PARTNO equals p1.PARTNO
                //		into bGroup
                //		from p1 in bGroup.DefaultIfEmpty()
                //		select new { w0, bGroup }
                //	).ToList();
            }
            return lot;
            //return Txn.result;
   //         var isQueryByLotNo = LOT != null;
            //var queryKey = isQueryByLotNo ? LOT : LOT_SID;
            //var LotInfo = Txn.GetLotInfo(queryKey, false, isQueryByLotNo);
            //dynamic ROUTE = new ExpandoObject();
            //dynamic OPER = new ExpandoObject();
            //var key = ActName?.ToUpper();
            //switch (key) {
            //	case "ROUTE":
            //		var RouteVerInfo = LotInfo.GetRouteVersionInfo();
            //		var Opers = RouteVerInfo.GetRouteVersionOperationList();
            //		var Settings = (from a in Opers
            //						select new
            //						{
            //							a.OPER_SID,
            //							a.Name,
            //							Setting = WIPOperConfigServices.GetOperSetting(a.OPER_SID, false)
            //						}
            //		).ToList();
            //		ROUTE = new { RouteVerInfo, Opers, Settings};
            //		break;
            //	default:
            //		var Setting = WIPOperConfigServices.GetOperSetting(LotInfo.OPER_SID, false);
            //		var Reason = DDLServices.GetPartNoOperReasonCodeData_OperSid(LotInfo.SID, LotInfo);
            //		OPER = new { Setting, Reason};
            //		break;
            //}
            //Txn.result.Data = new
            //{
            //	LotInfo,
            //	ROUTE,
            //	OPER,
            //};
            //return Txn.result;
        });

        */

        public class d_工作站
        {
            public d_Base _Base { get; set; }
            public d_Base_工作站 _Base_工作站 { get; set; }
            public Act Action;

            public class Act
            {
                public int 列舉有開啟的擴展_全部;
                public int 查詢含有此站的流程;
                public int 查詢含有此站的工單;
            }
        }

        [System.Web.Http.AllowAnonymousAttribute]
        [System.Web.Http.Route("工作站")]
        public dynamic 工作站(d_工作站 data)
        => TxnBase.LzDBQuery<dynamic>(Txn => {
            var result = data.Action.Dispatch(data._Base_工作站, Txn, data);
            return (result != null) ? result : data;
        });
        /*
        [System.Web.Http.AllowAnonymousAttribute]
        [System.Web.Http.Route("RouteOperStage")]
        public dynamic RouteOperStage(Library.BLL.ADM.RouteOperStageServices.d_Query_RouteOperStage data)
        =>TxnBase.LzDBQuery<dynamic>(Txn =>{
            dynamic _r = new ExpandoObject();
            var list = new List<dynamic>();
            if (data.isQueryStage){
                var PF_STAGEs= (from a in Txn.EFQuery_MES.PF_STAGE.f_STAG_Query(data)
                            select a).ToList();

                foreach (var PF_STAGE in PF_STAGEs) {
                var Info = (from a in Txn.EFQuery_MES.PF_ROUTE_VER_OPER_STAGE
                            join b in Txn.EFQuery_MES.PF_ROUTE_VER_OPER
                                on a.ROUTE_VER_OPER_SID equals b.ROUTE_VER_OPER_SID
                                into b_grp
                            from b in b_grp.DefaultIfEmpty()
                            where a.STAGE_SID == PF_STAGE.STAGE_SID
                            select new {
                                a.ROUTE_VER_OPER_STAGE_SID,
                                a.STAGE_SID,
                                a.ROUTE_VER_SID,
                                b.OPER_SEQ,
                                b.OPERATION,
                                b.OPERATION_NO,
                                b.ROUTE,
                                b.ROUTE_NO,
                                b.VERSION,
                            })
                            .ToList();
                    list.Add(new { PF_STAGE, Info });
                }
                _r.r = list;
                return _r;
            }
            if (data.isQueryOper) { 
                var Opers = (from a in Txn.EFQuery_MES.PF_ROUTE_VER_OPER.f_STAG_QueryRouteOper(data)
                             select a);
                var STAGE_Info = (from a in Txn.EFQuery_MES.PF_ROUTE_VER_OPER_STAGE
                                        .Where(c1 => Opers.Any(c2 => c2.ROUTE_VER_OPER_SID == c1.ROUTE_VER_OPER_SID))
                        join b in Txn.EFQuery_MES.PF_STAGE
                            on a.STAGE_SID equals b.STAGE_SID
                            into b_grp
                        from b in b_grp.DefaultIfEmpty()
                        select new
                        {
                            a.ROUTE_VER_OPER_STAGE_SID,
                            a.ROUTE_VER_OPER_SID,
                            a.STAGE_SID,
                            b.STAGE_NO,
                            b.STAGE_NAME,
                        })
                        .ToList();
                _r.r = new { Opers, STAGE_Info };
                return _r;
            }

            return data;	 
        });

        */



        //      /// <summary>
        //      /// 測試
        //      /// </summary>
        //      /// <param name="Test"></param>
        //      /// <returns></returns>
        //      [System.Web.Http.HttpGet]
        //[System.Web.Http.Route("Lot1")]
        //public string LotInfox(string Test)
        //=>TxnBase.LzDBQuery<string>(Txn=>{
        //	Check.Invalid("test-Invalid", Test == "Invalid");
        //	return Test;
        //});

        [System.Web.Http.AllowAnonymousAttribute]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Oper/{OperNo}")]
        public dynamic 依工作站代碼找找出關聯流程(string OperNo, string Action = null)
        => TxnBase.LzDBQuery<dynamic>(Txn => {
            switch (Action)
            {
                case "Lots":
                    return Txn.EFQuery_MES.f_Oper_找出一般批號(OperNo).ToList();
                    break;
                        //case "ParallelLots":
                        //	return Txn.EFQuery_MES.f_Oper_找出併行工站批號(OperNo).ToList();
                        break;
                case "關聯流程":
                    return Txn.EFQuery_MES.f_Oper_找出關聯流程(OperNo).ToList();
                    break;
            }
            return null;
        });

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("OperExtend")]
        [System.Web.Http.Route("OperExtend/{ExtendName}")]
        public dynamic 工作站擴展_相對有開啟的站(string ExtendName = null)
        => TxnBase.LzDBQuery<dynamic>(Txn => {
            var x = Txn.EFQuery_MES.PF_OPERATION_EXPAND.ToList();
            if (string.IsNullOrWhiteSpace(ExtendName)) return x;
            return x.Where(c => {
                dynamic data = JsonConvert.DeserializeObject(c.SETTING_JSON);
                if (FindProperty(data.CheckInSet, ExtendName) ||
                    FindProperty(data.CheckOutSet, ExtendName) ||
                    FindProperty(data.AnySet, ExtendName))
                {
                    return true;
                }
                return false;
            }).ToList();
        });

        public bool FindProperty(dynamic obj, string propertyName)
        {
            JObject jobj = obj as JObject;
            if (jobj != null)
            {
                foreach (var property in jobj)
                {
                    var isSameName = property.Key.ToString() == propertyName;
                    if (isSameName && (bool)property.Value) return true;
                    if (property.Value is JObject)
                    {
                        if (FindProperty(property.Value, "enable"))
                            return true;
                    }
                }
            }
            return false;
        }



        /*
        [System.Web.Http.Route("ROUTE_NO/{ROUTE_NO}")]
        [System.Web.Http.Route("ROUTE_VER_SID/{ROUTE_VER_SID}")]
        [System.Web.Http.Route("ROUTE_NO/{ROUTE_NO}/info/{info}")]
        [System.Web.Http.Route("ROUTE_VER_SID/{ROUTE_VER_SID}/info/{info}")]
        [System.Web.Http.HttpGet]

        public dynamic ROUTE(string ROUTE_NO = null, string ROUTE_VER_SID = null, string info = null)
        => TxnBase.LzDBQuery<dynamic>(Txn => {
            dynamic data = new ExpandoObject();
            data.ROUTE =  Txn.EFQuery_MES.PF_ROUTE_VER
                .Where(c=>c.ROUTE_NO == ROUTE_NO || c.ROUTE_VER_SID == ROUTE_VER_SID)
                .ToList();
            switch (info?.ToUpper()) {
                case "OPER":
                    data.OPERs = (from a in Txn.EFQuery_MES.PF_ROUTE_VER_OPER
                                  where a.ROUTE_VER_SID == ROUTE_VER_SID
                                  orderby a.OPER_SEQ
                                  select a).ToList();
                    break;
            }
            return data;
        });
        */


    }

    //TODO-tmp 用某個流程 ,直接 查出現下有那些站,站內有那些批號

    //[System.Web.Http.RoutePrefix("api/v1")]
    public class ApiTestController : System.Web.Http.ApiController
    {

        [System.Web.Http.Route("T003")]
        [System.Web.Http.HttpGet]
        public List<string> T003(string T003)
        => new List<string>() { T003 };


        [System.Web.Http.Route("PagerQuery_test")]
        [System.Web.Http.HttpPost]
        public dynamic T004(BLL.DataViews.Res.PagerQuery pager)
        => pager;
        

        //[System.Web.Http.HttpGet]
        //public List<string> T002(string T003)
        //=> new List<string>() { T003 };
    }



}

//bk