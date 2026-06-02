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

namespace Genesis
{
    public partial class GTI_Test
    {
        public static void t_Process_SJ(ITxnBase txn, string key)
        {
            var WP_LOT_HIST = txn.EFQuery_MES.WP_LOT_HIST.IQueryable_ACTION_LINK_SID(key);
            var WP_LOT = (from a in txn.EFQuery_MES.WP_LOT
                .Where(c => WP_LOT_HIST.Any(c1 => c1.LOT == c.LOT))
                          select a
                ).ToList();
            var LOT = new
            {
                WP_LOT,
                WP_LOT_HIST = WP_LOT_HIST.AsNoTracking().ToList(),
                WP_LOT_SPLIT = txn.EFQuery_MES.WP_LOT_SPLIT.GetData_ACTION_LINK_SID(key),
                ZZ_LOT_MOVE = txn.EFQuery_MES.ZZ_LOT_MOVE.GetData_ACTION_LINK_SID(key),
            };
            var USER = new
            {
                WP_USER_TRACE = txn.EFQuery_MES.WP_USER_TRACE.GetData_ACTION_LINK_SID(key),
                WP_USER_TRACE_IN = (from a in txn.EFQuery_MES.WP_USER_TRACE_IN
                                    where (WP_LOT_HIST.Any(c1 => c1.LOT == a.LOT) && a.CREATE_DATE == txn.ExeTime)
                                        || a.ACTION_LINK_SID == key
                                    select a
                                ).AsNoTracking().ToList(),
                WP_USER_TRACE_IN_EQP = txn.EFQuery_MES.WP_USER_TRACE_IN_EQP.GetData_ACTION_LINK_SID(key),
                ZZ_OPER_WORKT_SUMMARY = txn.EFQuery_MES.ZZ_OPER_WORKT_SUMMARY.GetData_ACTION_LINK_SID(key),
            };

            var DEFECT = new
            {
                WP_LOT_DEFECT = txn.EFQuery_MES.WP_LOT_DEFECT.GetData_ACTION_LINK_SID(key),
            };

            var SCRAP = new
            {
                WP_LOT_SCRAP = txn.EFQuery_MES.WP_LOT_SCRAP.GetData_ACTION_LINK_SID(key),
            };


            var r = new
            {
                LOT,
                USER,
                SCRAP,
                DEFECT,
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


    }
}