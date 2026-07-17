using BLL.DataViews.Edc;
using BLL.MES;
using Genesis.Gtimes.ADM;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject
{
	public partial class t_EDC : _testBase
	{

        [TestMethod]
        public void t_20260717_()
        => _DBTest((txn) =>
        {
			EDCUtility.EDCFunctions edcFun = new EDCUtility.EDCFunctions(txn.DBC);
			var _lotInfo = txn.GetLotInfo("WO-20260713-001-02", isQueryByLotNO: true);
			var edcData = edcFun.GetEdcOperParaData_OperSid(_lotInfo.WO, _lotInfo.ROUTE_VER_SID, _lotInfo.ROUTE_VER_OPER_SID, _lotInfo.PARTNO, _lotInfo.OPER_SID);

		}, true);
	}


}
