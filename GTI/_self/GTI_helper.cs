using BLL.InterFace;
using BLL.MES;
using Dal.Repository;
using Frame.Code;
using Genesis;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using MDL.MES;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.ADM.CarrierUtility;
using static Genesis.Gtimes.WIP.LotUtility;

namespace UnitTestProject.TestUT
{
    public class GTI_TxnBase
    {
        public static void IPQC(ITxnBase Txn, string ActionName, string Link_SID)
        {
            WP_IPQC form = Txn.result.Data;
            //var WP_IPQC = Txn.EFQuery_MES.WP_IPQC_CHECKITEM.IQueryable_ACTION_LINK_SID(Link_SID);
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
            //File.WriteAllText(GTI_Test.g_path.t_Process, json);
        }
    }
    public class GTI_helper
	{
        public static CarrierInfo getCarrierInfo(string cd_sql=null)
        => TxnBase.LzDBQuery(Txn=>{
            cd_sql = cd_sql ?? "SELECT * from FC_CARRIER WHERE STATE_NO = 'Idle'";
            var _d = Txn.DapperQuery<FC_CARRIER>(cd_sql)
                .FirstOrDefault();
            Txn.result.Data = Txn.GetCarrierInfo(_d.CARRIER_NO);
            return Txn.result;
        }).Data;

        public static EquipmentUtility.EquipmentInfo getEquipmentInfo(ITxnBase Txn, string cd_sql = null)
        {
            cd_sql = cd_sql ?? "SELECT * from FC_EQUIPMENT WHERE STATE_NO = 'Idle'";
            var _d = Txn.DapperQuery<FC_EQUIPMENT>(cd_sql)
                .FirstOrDefault();
            return Txn.GetEquipmentInfo(_d.EQP_SID);
        }

        public static UserUtility.UserInfo getUserInfo(ITxnBase Txn, string cd_sql = null)
        {
            cd_sql = cd_sql ?? "SELECT * from AD_USER WHERE ENABLE_FLAG = 'T'";
            var _d = Txn.DapperQuery<AD_USER>(cd_sql)
                .FirstOrDefault();
            return Txn.GetUserInfo(_d.ACCOUNT_NO);
        }

        public static LotInfo getLotInfo(string cd_sql = null)
        => TxnBase.LzDBQuery(Txn => {
            Txn.result.Data = getLotInfo(Txn, cd_sql);
            return Txn.result;
        }).Data;
        public static LotInfo getLotInfo(ITxnBase Txn, string cd_sql = null){
            cd_sql = cd_sql ?? "SELECT * from WP_LOT WHERE STATUS = 'Wait'";
            var _d = Txn.DapperQuery<WP_LOT>(cd_sql)
                .FirstOrDefault();
            return Txn.GetLotInfo(_d.LOT_SID);
        }

        public static IResult CatchEx(Func<IResult> p)
        {
            IResult r = new Result(false);
            try {
                r = p();
            }
            catch (Exception txnEX){
                if (txnEX.Data["IResult"] != null){
                    r = txnEX.Data["IResult"].ToString().ToObject<Result>();
                }
            }
            return r;
        }
    }
 
 

}
