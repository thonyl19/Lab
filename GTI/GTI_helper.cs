using Dal.Repository;
using Genesis.Gtimes.Common;
using MDL.MES;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.ADM.CarrierUtility;
using static Genesis.Gtimes.WIP.LotUtility;

namespace UnitTestProject.TestUT
{
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
    }
 
}
