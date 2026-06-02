using BLL.MES;
using Genesis.Gtimes.ADM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.RouteUtility;

namespace UnitTestProject
{
	public partial class t_RouteOper : _testBase
	{
        [TestMethod]
        public void t_取得耗用物料資訊()
        => _DBTest((Txn) =>
        {
            var _lotInfo = Txn.GetLotInfo("DevTest_20260114-03", true,true);
            var RouteVerOperInfo = Txn.GetRouteVerOper();
            var r = WIPOperConfigServices.GetOperPartUseList(_lotInfo, RouteVerOperInfo);
            FileApp.WriteSerializeJson(r, _log.t_OperationPartUseList);
        }, false, true);


        [TestMethod]

        public void t_GetRouteVerOperPartNoInfo()
        => _DBTest((Txn) =>{
            var _lotInfo = Txn.GetLotInfo("DevTest_20260114-03", true,true);
            var _part = _lotInfo.GetPartNoVersionInfo();
            var  fun = new RouteUtility.PartNoRouteVerOperFunction(mes.dbc());

            var dt = fun.GetRouteVerOperPartNoInfo(
                        _part.SID
                        , _lotInfo.ROUTE_VER_SID
                        , _lotInfo.ROUTE_VER_OPER_SID);
            FileApp.WriteSerializeJson(dt, _log.t_GetRouteVerOperPartNoInfo);
        }, false, true);


        /// <summary>
        /// 取得類 
        /// </summary>
        [TestMethod]
        public void t_GetRouteVerOperPartNoList()
        => _DBTest((Txn) =>{
            var _lotInfo = Txn.GetLotInfo("DevTest_20260114-03", true,true);
            var _part = _lotInfo.GetPartNoVersionInfo();
            //var  fun = new RouteUtility.PartNoRouteVerOperFunction(mes.dbc());

            //var dt = fun.GetRouteVerOperPartNoList(
            //            _part.SID
            //            , _lotInfo.ROUTE_VER_SID
            //            , _lotInfo.ROUTE_VER_OPER_SID);

            var z = DynFuncModuleServices.GetOperPartNOAllPartNoList(Txn,
                        _part.SID
                        , _lotInfo.ROUTE_VER_SID
                        , _lotInfo.ROUTE_VER_OPER_SID);


            FileApp.WriteSerializeJson(z, _log.t_GetRouteVerOperPartNoList);
        }, false, true);
           
        

    }
}