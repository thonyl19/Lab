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
 

        public static void TxnBase_T_ZAC(ITxnBase txn, string ActionName, string Link_SID)
        {
            var _ZZ_ZAC_MATERIAL_TEMP = txn.EFQuery_MES.ZZ_ZAC_MATERIAL_TEMP.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            var _PF_PARTNO_ROUTEVER_EXT = txn.EFQuery_MES.PF_PARTNO_ROUTEVER_EXT.Where(c => c.UPDATE_DATE == txn.ExeTime).ToList();
            var _PF_OPERATION_PQC = txn.EFQuery_MES.PF_OPERATION_PQC.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            var _PF_PARTNO_ROUTEVER_OPER_PQC = txn.EFQuery_MES.PF_PARTNO_ROUTEVER_OPER_PQC.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            var r = new{
                _ZZ_ZAC_MATERIAL_TEMP,
                _PF_OPERATION_PQC,
                _PF_PARTNO_ROUTEVER_OPER_PQC,
                _PF_PARTNO_ROUTEVER_EXT
            };
            string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);

            // 将 JSON 写入文件
            File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }



         
        public static dynamic trc_ZAC(ITxnBase txn, string key)
        {
            var _ZZ_ZAC_MATERIAL_TEMP = txn.EFQuery_MES.ZZ_ZAC_MATERIAL_TEMP.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            var _ZZ_ZAC_MLOT_USED = txn.EFQuery_MES.ZZ_ZAC_MLOT_USED.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            var _PF_PARTNO_ROUTEVER_OPER_PQC = txn.EFQuery_MES.PF_PARTNO_ROUTEVER_OPER_PQC.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            //var _WP_QC = txn.EFQuery_MES.WP_QC.Where(c => c.UPDATE_DATE == txn.ExeTime).ToList();
            //var _QC_NO = _WP_QC.FirstOrDefault().QC_NO;
            //var sql = $@"
            //    SELECT *
            //    FROM WP_QC_LOT
            //    WHERE QC_NO ='{_QC_NO}'";
            //var _WP_QC_LOT = txn.DapperQuery(sql).ToList();
            //var _WP_QC_CHECKITEM = txn.EFQuery_MES.WP_QC_CHECKITEM.Where(c => c.UPDATE_DATE == txn.ExeTime).ToList();
            //var _WP_QC_CHECKITEM_RAW = txn.EFQuery_MES.WP_QC_CHECKITEM_RAW.Where(c => c.CREATE_DATE == txn.ExeTime).ToList();
            return new
            {
                _ZZ_ZAC_MATERIAL_TEMP,
                _ZZ_ZAC_MLOT_USED,
                _PF_PARTNO_ROUTEVER_OPER_PQC,
                //_WP_QC = _WP_QC.ToList(),
                //_WP_QC_LOT,
                //_WP_QC_CHECKITEM,
                //_WP_QC_CHECKITEM_RAW
            };
        }

    }
}
namespace Genesis.WebApi
{
     
}