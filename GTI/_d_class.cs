using BLL.MES;
using Genesis.Gtimes.WIP;
using System.Collections.Generic;

namespace UnitTestProject
{
    internal class _d_class
    {
        public bool is單批多包;
        public bool isGradeInvalid;
        public LotUtility.LotInfo lotInfo;
        public Dictionary<string, Wafer_Services.d_Process_SHIP_CASSETTE.Lot_Wafers> GrpWaferByShipCassette;
        public Dictionary<string, List<string>> GRADEs;

        public _d_class(bool p, bool isGradeInvalid, LotUtility.LotInfo lotInfo, Dictionary<string, Wafer_Services.d_Process_SHIP_CASSETTE.Lot_Wafers> sHIP_CASSETTE, List<KeyValuePair<string, Wafer_Services.d_WP_LOT_WAFER_MAPPING>> sHIP_CASSETTEs, Dictionary<string, List<string>> gRADEs)
        {
            this.is單批多包 = p;
            this.isGradeInvalid = isGradeInvalid;
            this.lotInfo = lotInfo;
            this.GrpWaferByShipCassette = sHIP_CASSETTE;
            this.GRADEs = gRADEs;
        }
    }
}