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

 namespace Genesis
{
    public partial class GTI_Test
    {
        public static void TxnBase_EDCTarget(ITxnBase Txn, string ActionName, string Link_SID)
        {
            var FC_EDC_TARGET = Txn.EFQuery_MES.FC_EDC_TARGET
                .Where(c => c.UPDATE_DATE == Txn.ExeTime)
                .ToList();
            var FC_TARGET = Txn.EFQuery_MES.FC_TARGET
                .Where(c => c.CREATE_DATE == Txn.ExeTime)
                .ToList();
 

            var R = new
            {
                FC_EDC_TARGET,
                FC_TARGET
            };
            string json = JsonConvert.SerializeObject(R, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }
    }
}

namespace Genesis.WebApi
{
    //[System.Web.Http.RoutePrefix("api")]
    public partial class SelfInfoController : System.Web.Http.ApiController
        {

            public class d_EDC
            {
                public d_Base _Base { get; set; }
                public Act Action;

                public class Act
                {
                    public int ByEdc查詢符合關聯條件;
                    public int 查詢EDC群組;
                }
            }
            [System.Web.Http.AllowAnonymousAttribute]
            [System.Web.Http.Route("EDC")]
            public dynamic EDC(d_EDC data)
            => TxnBase.LzDBQuery<dynamic>(Txn => {
                if (data.Action.ByEdc查詢符合關聯條件.isAction()){
                    var q0 = (from a in Txn.EFQuery_MES.FC_EDC_TARGET
                        where a.ENABLE_FLAG == "T"
                            &&  (data._Base.NO !=null &&  a.EDC_NO == data._Base.NO)
                            || (data._Base.SID !=null &&  a.EDC_SID == data._Base.SID)
                            || (data._Base.NAME !=null &&  a.EDC_NAME == data._Base.NAME)
                        select a
                    );
                    var q1 = (from a in Txn.EFQuery_MES.FC_TARGET
                              where a.FROM_RULE_SID != null
                                  && q0.Any(c=>a.FROM_RULE_SID == c.EDC_TARGET_SID)
                              select a
                            ).ToList();

 
                    var q2 = q1.GroupBy(p => p.FROM_RULE_SID)
                        .Where(g => g.Key != null)
                        .ToDictionary(p => p.Key, p => p)
                        .t_Process()
                        ;
                    return q2;
                }if (data.Action.查詢EDC群組.isAction())
                {
                    return data._Base.查詢EDC群組(Txn);
                }
                return data;
            });

        
    }
}

