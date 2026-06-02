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
using static Genesis.WebApi.SelfInfoController;

namespace Genesis
{
    public partial class GTI_Test
    {

        public static dynamic trc_ZAC(ITxnBase txn, string key)
        {
            var _ZZ_ZAC_MATERIAL_TEMP = txn.EFQuery_MES.ZZ_ZAC_MATERIAL_TEMP.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            return new{
                _ZZ_ZAC_MATERIAL_TEMP,
            };
        }

    }
}
namespace Genesis.WebApi
{
     
}