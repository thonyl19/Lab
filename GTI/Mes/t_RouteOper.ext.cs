using BLL.MES;
using Genesis.Gtimes.ADM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.RouteUtility;

namespace UnitTestProject
{
	public partial class t_RouteOper : _testBase
	{
        [TestMethod]
        public void t_取得下一個流程站點()
        => _DBTest((Txn) =>
        {
            var _lotInfo = GTI_helper.getLotInfo("SELECT * from WP_LOT WHERE STATUS = 'Run'");
            var routeVerOper = Txn.LzQuery.WIP.f取得RouteVersionOperationInfo_是否存在(_lotInfo.ROUTE_VER_OPER_SID);
            var operation = routeVerOper.GetOperationInfo();
            var operTypeVerRule = operation.GetOperationStartRuleInfo();
            var nextOperTypeVerRule = operTypeVerRule.GetNextRouteVersionOperationRuleInfo();
            FileApp.WriteSerializeJson(nextOperTypeVerRule, _log.t_GetNextRouteVersionOperationRuleInfo);
            var RouteVerOperInfo = Txn.GetRouteVerOper(_lotInfo.ROUTE_VER_OPER_SID);
            FileApp._tmpJson(RouteVerOperInfo);
        }, false, true);

    }
}