using BLL.InterFace;
using BLL.MES;
using Dapper;
using Frame.Code.Web.Select;
using Genesis;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.ADM;
using Genesis.Library.BLL.MES.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnitTestProject.TestUT;
using Genesis.Library.BLL.DTC;
using static BLL.MES.Wafer_Services;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.WIP.LotUtility;
using static Genesis.Library.BLL.DTC.Wafer;
using Frame.Code;

namespace UnitTestProject
{
	[TestClass]
	public class t_Wafer : _testBase
	{
		static class _log
		{
            internal static string t_wafer_shipinig_package_CHECKOUT
			{
				get
				{
					return FileApp.ts_Log(@"Wafer\t_wafer_shipinig_package_CHECKOUT.json");
				}
			}

			internal static string t_Hold_WaferInfo
			{
				get
				{
					return FileApp.ts_Log(@"Wafer\t_Hold_WaferInfo.json");
				}
			}

			internal static string t_ModifyGrade_Query(string extName="") 
			{
				return FileApp.ts_Log($@"Wafer\t_ModifyGrade_Query{extName}.json");
			}

			public static string _parse_WAFER_MAPPING { get { 
					return FileApp.ts_Log(@"Wafer\_parse_WAFER_MAPPING.json");
				}
			}

            public static string t_UnHold_CarrierInfo { get
				{
					return FileApp.ts_Log(@"Wafer\t_UnHold_CarrierInfo.json");
				}
			}

			internal static string t_Process_PosiMap_cmd_Grade
			{
				get
				{
					return FileApp.ts_Log(@"Wafer\t_Process_PosiMap_cmd_Grade.json");
				}
			}

			internal static string t_Process_PosiMap_cmd_Hold
			{
				get
				{
					return FileApp.ts_Log(@"Wafer\t_Process_PosiMap_cmd_Hold.json");
				}
			}

            public static string t_ShipCassetteInfo {
				get
				{
					return FileApp.ts_Log(@"Wafer\t_ShipCassetteInfo.json");
				}
			}

            public static string t_wafer_shipinig_package {
				get
				{
					return FileApp.ts_Log(@"Wafer\t_wafer_shipinig_package.json");
				}
			}

            public static string t_WaferPackage(string Ext="")
			{
				return FileApp.ts_Log($@"Wafer\t_WaferPackage{Ext}.json");
			}
		}
		public Wafer_Services serv = new Wafer_Services();
		public WIPServices serv_WIP = new WIPServices();

		[TestMethod]
        public void _CarrierLoad_CarrierInfo()
        {
			var r = serv.CarrierLoad_CarrierInfo("Carrier_001");
			FileApp.WriteSerializeJson(r, FileApp.ts_Log(@"Wafer\_CarrierLoad_CarrierInfo.json"));
        }

		[TestMethod]
		public void _CarrierLoad_WaferInfo()
		{
			//var r = serv.CarrierLoad_WaferInfo("B4N420510113W");
			//FileApp.WriteSerializeJson(r, FileApp.ts_Log(@"Wafer\_CarrierLoad_CarrierInfo.json"));
		}






		[TestMethod]
		public void t_UnHold_CarrierInfo()
		{
			TxnBase.LzDBQuery((tx) =>
			{
				var CarrierNo = "Carrier_001";
				var r = new Wafer_Services().UnHold_CarrierInfo(CarrierNo,false,1);
				new FileApp(false).Write_SerializeJson(r, _log.t_UnHold_CarrierInfo);
				return tx.result;
			});

		}





		[TestMethod]
        public void _CheckNotHoldByList()
        {
			new TxnBase("TEST", this.DBC)
				.Exec((_txn) =>
				{
					var _list = new List<string>() { "GTI22050513264580556", "GTI22050513264580554" };
					var _r = Wafer_Services.CheckNotHoldByList(_txn, _list);
				}, true);
		}


        [TestMethod]
        public void _ModifyGrade_Query()
        {
            //var r = serv.ModifyGrade_Query("CrystalGrowth_WO01-01.06.8").parseData<d_CommandWaferInfo>();
            //new FileApp(false).Write_SerializeJson(r, _log.t_ModifyGrade_Query());

            var _d = FileApp.Read_SerializeJson<d_CommandWaferInfo>(_log.t_ModifyGrade_Query());

			var sn = _d.Wafers[0].SERIAL_NUMBER_ID;
			var _r1 = serv.ModifyGrade_Query(sn,test:_d);
			Assert.IsTrue(_r1.Success
				, "正常測試,應回傳 true");

			_d.WP_LOT.STATUS = "Hold";
			var _r2 = serv.ModifyGrade_Query(sn, test: _d);
			Assert.IsTrue(_r2.Success == false 
				, "當批號狀態不為Wait,應回傳 false");

			_d.Wafers[0].SERIAL_STATUS = "Hold";
			var _r3 = serv.ModifyGrade_Query(sn, test: _d);
			Assert.IsTrue(_r3.Code == ErrCode.DB.NoData
				, "當 Wafer 狀態不為 Normal,應回傳 NoData");

		 
		}

        [TestMethod]
        public void t_ShipCassetteInfo()
        {
            var r = ShipCassettleServices.ShipCassetteInfo("Cassettle02");
			new FileApp(false).Write_SerializeJson(r, _log.t_ShipCassetteInfo, null);
		}

		[TestMethod]
		public void t_ExecHoldWafer()
		{
			var waferSN = "B4N404B26315W";
			var r = serv.ExecHoldWafer(waferSN, "other", "test", true);
		}

		[TestMethod]
		public void t_ExecScrapWafer()
		{
			var r = serv.ExecScrapWafer("Test_SplitWafer-01.09.12", "other", "test", true);
		}

		[TestMethod]
		public void _Hold_WaferInfo()
		{
			//var r = serv.Hold_WaferInfo("Test_SplitWafer-01.09.12");
			//new FileApp(false).Write_SerializeJson(r, _log.t_Hold_WaferInfo);
			var _d = FileApp.Read_SerializeJson<d_CommandWaferInfo>(_log.t_Hold_WaferInfo);

			
			var r1 = serv.Hold_WaferInfo("Test_SplitWafer-01.09.12", test: _d);
			Assert.IsTrue(r1.Success,"正常讀取測試,回傳值應為 True");

			_d.WP_LOT.STATUS = "Hold";
			var r2 = serv.ModifyGrade_Query("", test: _d);
			Assert.IsTrue(r2.Success==false,"批號狀態不為 Wait, 回傳值應為 false");
		}

		


		[TestMethod]
		public void t_ExecModifyGrade()
		{
            var r = new Wafer_Services().ExecModifyGrade
				("Test_SplitWafer-01.09.4"
				, "B"
				, "other"
				, ""
				, true)
				;
   //         new FileApp(false).Write_SerializeJson(r, _log.t_ModifyGrade_Query());

   //         var _d = FileApp.Read_SerializeJson<d_ModifyGrade_Query>(_log.t_ModifyGrade_Query());

			////var _d = src1.parseData<d_ModifyGrade_Query>();

			//var _正常測試 = serv.ModifyGrade_Query("CrystalGrowth_WO01-01.06.8", test: _d);
			//Assert.IsTrue(_正常測試.Success);

			//_d.LotInfo.STATUS = "Hold";
			//var _非Wait = serv.ModifyGrade_Query("", test: _d);
			//var __d = _非Wait.parseData<WP_LOT>();
			//Assert.IsTrue(_d.LotInfo.LOT_SID == __d.LOT_SID);

		}


		[TestMethod]

		public void t_Process_PosiMap_cmd_Hold()
		=>_DBTest(Txn => {
			var posi = FileApp.Read_SerializeJson<WAFER_MAPPING>(_log.t_Process_PosiMap_cmd_Hold);
			var Lot = Txn.GetLotInfo(posi.LOT_SID);
			var cmd = WIPServices.Process_PosiMap_cmd_Hold(Txn, Lot, posi);
			Txn.DoTransaction(cmd);
		},true);

		[TestMethod]

		public void t_Process_PosiMap_cmd_Scrap()
		=> _DBTest(Txn => {
			var posi = FileApp.Read_SerializeJson<WAFER_MAPPING>(_log.t_Process_PosiMap_cmd_Hold);
			var Lot = Txn.GetLotInfo(posi.LOT_SID);
			var cmd = WIPServices.Process_PosiMap_cmd_Scrap(Txn, Lot, posi);
			Txn.DoTransaction(cmd);
		}, true);


		[TestMethod]
		public void t_Process_PosiMap_cmd_Grade()
		=> _DBTest(Txn => {
			var Lot = Txn.GetLotInfo("GTI22050513264480527", true);
			var posi = FileApp.Read_SerializeJson<WAFER_MAPPING>(_log.t_Process_PosiMap_cmd_Grade);
			var cmd = WIPServices.Process_PosiMap_cmd_Grade
				(Txn
				, posi
				);
			Txn.DoTransaction(cmd);
		}, true);

		[TestMethod]
		public void t_DTC_SetGrade()
		=> _DBTest(Txn => {
			var posi = FileApp.Read_SerializeJson<WAFER_MAPPING>(_log.t_Process_PosiMap_cmd_Grade);
			var e = posi.parse_WP_LOT_WAFER_MAPPING();
			Txn.DoTransaction
				(new DTC_SetGrade(e)
				, new DTC_SetShipCassette(e)
				);
		}, true);

		[TestMethod]
		public void _parse_WAFER_MAPPING()
		=> _DBTest(Txn => {
			var c = Txn.GetCarrierInfo("MIRLE_FOUP_005");
			var r = serv_WIP.parse_WAFER_MAPPING(c);
			//new FileApp(true).Write_SerializeJson(r,_)
		});

		[TestMethod]
		public void t_wafer_shipinig_package_CHECKOUT()
		=> _DBTest(Txn => {
			var search = "Blade-Wafer002-01.01";
			var r = wafer_shipinig_package_CHECKOUT(search).Data;
			new FileApp(false).Write_SerializeJson(r, _log.t_wafer_shipinig_package_CHECKOUT);
		});

		[TestMethod]
		public void t_wafer_shipinig_package()
		=> _DBTest(Txn => {
			var search = "EB3N4B2B4505-01.01";
			var root = "TestA";
			var r = wafer_shipinig_package(search, root).Data;
			var r1 = wafer_shipinig_package(search, null).Data;
			new FileApp(false).Write_SerializeJson(r1, _log.t_wafer_shipinig_package);


			var sql_第一版_只取到批 = @"
					SELECT 	T0.*
							,T1.UNIT
							,T1.WO
							,T1.ROUTE_VER_OPER_SID
							,T1.QUANTITY
					FROM 	(SELECT		
									T00.LOT_SID
									,T00.LOT
									,T00.ROOT_LOT_SID
									,T0.SHIP_CASSETTE AS SHIP_CASSETTE
									--,ISNULL(T00.SHIP_CASSETTE,T00.LOT_SID) AS SHIP_CASSETTE
									,T0.GRADE
									--,ISNULL(T00.GRADE,'A') AS GRADE
									,MIN (T00.SERIAL_NUMBER) AS SERIAL_NUMBER
									,(SELECT 	TOP 1 T001.SERIAL_NUMBER_ID 
										FROM 	WP_LOT_WAFER_MAPPING as T001 
										WHERE 	T001.SERIAL_NUMBER  = MIN (T00.SERIAL_NUMBER)
												AND T001.LOT = T00.LOT
												) AS SERIAL_NUMBER_ID
							FROM 	WP_LOT_WAFER_MAPPING T00
							WHERE	T00.SERIAL_STATUS = 'Normal'
									AND (T00.LOT = @search OR T00.SHIP_CASSETTE = @search)
							GROUP	BY T00.LOT_SID,T00.LOT,T00.ROOT_LOT_SID,SHIP_CASSETTE,GRADE
							) AS T0
							INNER JOIN  WP_LOT T1
								ON T0.LOT_SID = T1.LOT_SID

			";
		});

		


		[TestMethod]
		public void t_Wafer入庫編碼()
		=> _DBTest(Txn => {
			var WO="";
			var MTR_IngotID="";
		});


		[TestMethod]
		public void t_process_漢民處理新增長晶站記錄設備的邏輯()
		=> _DBTest(Txn => {
			Txn.GetLotInfo("EB1N4B2B2002-02", true, true);
			var c = WIPServices.process_漢民處理新增長晶站記錄設備的邏輯(Txn);
		},true);

		

		[TestMethod]
        public void t_DTC()
        => _DBTest(Txn =>
        {
            var _lot = Txn.GetLotInfo("GTI22062810124839435");
			var r = new WP_LOT_WAFER_MAPPING() {
				SERIAL_NUMBER = 7,
				SERIAL_NUMBER_ID = "B4N410W04B262"
			};

			//Txn.DoTransaction(
   //             new DTC_InsertWaferTRace(_lot, "Test_SplitWafer-01.09.4", "test", "")
   //             //,new DTC_UpdateWaferStatusAndSN("Test_SplitWafer-01.09.4", "Hold")
			//	,new DTC_UpdateWaferStatusAndSN("Test_SplitWafer-01.09.4", null, 7)
   //             );
			Txn.DoTransaction(
				new DTC_InsertWaferTrace
							(r
							, TxnACTION.n("test", "", "")
							, _lot)
				, new DTC_UpdateWaferStatusAndSN(r)
				);

			Txn.DoTransaction(
				new DTC_SetShipCassetteGrade("Cassettle02","A"),
				new DTC_ShipCassetteAddCapacity("Cassettle02",50)
				);

		}, true);

		[TestMethod]
		public void _DTC_UpdateWaferStatusAndSN()
		=> _DBTest(Txn =>
        {
            var t = Txn.EFQuery<WP_LOT_WAFER_MAPPING>()
                .Reads().FirstOrDefault();
            var SERIAL_NUMBER_ID = t.SERIAL_NUMBER_ID;
            var exp_SERIAL_NUMBER = t.SERIAL_NUMBER;
            
			//測試只有變更狀態
			t.SERIAL_STATUS = "Test";
            t.SERIAL_NUMBER = DTC_Mark.未變更.ToInt();
            Txn.DoTransaction(new DTC_UpdateWaferStatusAndSN(t));
			var t1 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
            Assert.IsTrue(exp_SERIAL_NUMBER == t1.SERIAL_NUMBER);
            Assert.IsTrue(t.SERIAL_STATUS == t1.SERIAL_STATUS);

			//測試變更狀態和序號
			t.SERIAL_STATUS = "Test1";
            t.SERIAL_NUMBER = 99;
            Txn.DoTransaction(new DTC_UpdateWaferStatusAndSN(t));
			var t2 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
            Assert.IsTrue(t.SERIAL_NUMBER == t2.SERIAL_NUMBER);
            Assert.IsTrue(t.SERIAL_STATUS == t2.SERIAL_STATUS);

			t.SERIAL_STATUS = "Test2";
			t.SERIAL_NUMBER = 999;
			var _lot = Txn.GetLotInfo(t.ROOT_LOT_SID);
			Txn.DoTransaction(new DTC_UpdateWaferStatusAndSN(t, _lot));
			var t3 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
			Assert.IsTrue(t3.SERIAL_NUMBER == t.SERIAL_NUMBER);
			Assert.IsTrue(t3.SERIAL_STATUS == t.SERIAL_STATUS);
			Assert.IsTrue(t3.PARENT_LOT_SID == t.LOT_SID);
			Assert.IsTrue(t3.LOT_SID == _lot.SID);
			Assert.IsTrue(t3.LOT == _lot.LOT);

		}, true);

		[TestMethod]
		public void _DTC_Hold()
		=> _DBTest(Txn =>
		{
			var t = Txn.EFQuery<WP_LOT_WAFER_MAPPING>()
				.Reads().FirstOrDefault();
			var SERIAL_NUMBER_ID = t.SERIAL_NUMBER_ID;
			var SERIAL_STATUS = t.SERIAL_STATUS;
			////測試變更狀態和序號
			var _lot = Txn.GetLotInfo(t.ROOT_LOT_SID);
			var _dtc = new DTC_Hold(t, _lot, "test-hold","DESC");
			Txn.DoTransaction(_dtc);
            var t2 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
            Assert.IsTrue(t2.SERIAL_NUMBER == -1);
            Assert.IsTrue(t2.SERIAL_STATUS == STATUS.Hold.ToString());
            Assert.IsTrue(t2.PARENT_LOT_SID == t.LOT_SID);
            Assert.IsTrue(t2.LOT_SID == _lot.SID);
            Assert.IsTrue(t2.LOT == _lot.LOT);

			var t2_trc = _WP_LOT_WAFER_MAPPING_TRACE(Txn);
			Assert.IsTrue(t2_trc.OLD_LOT_SID == t.LOT_SID);
			Assert.IsTrue(t2_trc.LOT_SID == _lot.SID);
			Assert.IsTrue(t2_trc.LOT == _lot.LOT);
			Assert.IsTrue(t2_trc.OPERATION == _lot.OPERATION);
			Assert.IsTrue(t2_trc.ROUTE_VER_OPER_SID == _lot.ROUTE_VER_OPER_SID);

			Assert.IsTrue(t2_trc.SERIAL_NUMBER_ID == t.SERIAL_NUMBER_ID);
			Assert.IsTrue(t2_trc.OLD_STATUS == SERIAL_STATUS);
			Assert.IsTrue(t2_trc.NEW_STATUS == STATUS.Hold.ToString());
			Assert.IsTrue(t2_trc.OLD_SERIAL_NUMBER == t2.PREV_SEAIAL);
			Assert.IsTrue(t2_trc.NEW_SERIAL_NUMBER == -1);

			Assert.IsTrue(t2_trc.ACTION == ACTION.WAFER_HOLD.ToString());
			Assert.IsTrue(t2_trc.ACTION_REASON == _dtc.txnACTION.ReasonNo);
			Assert.IsTrue(t2_trc.ACTION_DESCRIPTION == _dtc.txnACTION.Desc);

		}, true);


		[TestMethod]
		public void _DTC_InsertWaferTrace()
		=> _DBTest(Txn => {
	//		var t = Txn.DapperQuery("select * from WP_LOT_WAFER_MAPPING")
	//.FirstOrDefault();
			var t = Txn.EFQuery<WP_LOT_WAFER_MAPPING>()
				.Reads().FirstOrDefault();
			var _lot = Txn.GetLotInfo(t.ROOT_LOT_SID);

			var SERIAL_NUMBER_ID = t.SERIAL_NUMBER_ID;
			var SERIAL_STATUS = t.SERIAL_STATUS;
			var act = TxnACTION.n
					(ACTION.WAFER_EXCHANGE.ToString()
					, "");
			var _dtc = new DTC_InsertWaferTrace(t, act,true);
			Txn.DoTransaction(_dtc);
		}, true);

		[TestMethod]
		public void _DTC_Scrapt()
		=> _DBTest(Txn =>
		{
			var t = Txn.EFQuery<WP_LOT_WAFER_MAPPING>()
				.Reads().FirstOrDefault();
			var SERIAL_NUMBER_ID = t.SERIAL_NUMBER_ID;
			var SERIAL_STATUS = t.SERIAL_STATUS;
			////測試變更狀態和序號
			var _lot = Txn.GetLotInfo(t.ROOT_LOT_SID);
			var _dtc = new DTC_Scrapt(t, _lot, "test-Scrap", "DESC");
			Txn.DoTransaction(_dtc);
			var t2 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
			Assert.IsTrue(t2.SERIAL_NUMBER == -1);
			Assert.IsTrue(t2.SERIAL_STATUS == STATUS.Scrap.ToString());
			//報廢不會變更 Lot
			Assert.IsTrue(t2.PARENT_LOT_SID == t.PARENT_LOT_SID);
			Assert.IsTrue(t2.LOT_SID == t.LOT_SID);
			Assert.IsTrue(t2.LOT == t.LOT);

			var t2_trc = _WP_LOT_WAFER_MAPPING_TRACE(Txn);
			Assert.IsTrue(t2_trc.OLD_LOT_SID == t.LOT_SID);
			Assert.IsTrue(t2_trc.LOT_SID == _lot.SID);
			Assert.IsTrue(t2_trc.LOT == _lot.LOT);
			Assert.IsTrue(t2_trc.OPERATION == _lot.OPERATION);
			Assert.IsTrue(t2_trc.ROUTE_VER_OPER_SID == _lot.ROUTE_VER_OPER_SID);

			Assert.IsTrue(t2_trc.SERIAL_NUMBER_ID == t.SERIAL_NUMBER_ID);
			Assert.IsTrue(t2_trc.OLD_STATUS == SERIAL_STATUS);
			Assert.IsTrue(t2_trc.NEW_STATUS == t2.SERIAL_STATUS);
			Assert.IsTrue(t2_trc.OLD_SERIAL_NUMBER == t2.PREV_SEAIAL);
			Assert.IsTrue(t2_trc.NEW_SERIAL_NUMBER == -1);

			Assert.IsTrue(t2_trc.ACTION == ACTION.WAFER_SCRAP.ToString());
			Assert.IsTrue(t2_trc.ACTION_REASON == _dtc.txnACTION.ReasonNo);
			Assert.IsTrue(t2_trc.ACTION_DESCRIPTION == _dtc.txnACTION.Desc);
		}, true);


		[TestMethod]
		public void _DTC_Exchange()
		=> _DBTest(Txn =>
		{
			var t = Txn.EFQuery<WP_LOT_WAFER_MAPPING>()
				.Reads().FirstOrDefault();
			var SERIAL_NUMBER_ID = t.SERIAL_NUMBER_ID;
			var SERIAL_NUMBER = t.SERIAL_NUMBER;
			var SERIAL_STATUS = t.SERIAL_STATUS;
			////測試變更狀態和序號

			t.SERIAL_NUMBER = 99;
			var _lot = Txn.GetLotInfo(t.ROOT_LOT_SID);
			var _dtc = new DTC_Exchange(t, _lot, "test-Exchange", "DESC");
			Txn.DoTransaction(_dtc);
			var t2 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
			Assert.IsTrue(t2.PREV_SEAIAL == SERIAL_NUMBER);
			Assert.IsTrue(t2.SERIAL_NUMBER == t.SERIAL_NUMBER);
			Assert.IsTrue(t2.SERIAL_STATUS == t.SERIAL_STATUS);
			//交換不會變更 Lot
			Assert.IsTrue(t2.PARENT_LOT_SID == t.PARENT_LOT_SID);
			Assert.IsTrue(t2.LOT_SID == t.LOT_SID);
			Assert.IsTrue(t2.LOT == t.LOT);

			var t2_trc = _WP_LOT_WAFER_MAPPING_TRACE(Txn);
            Assert.IsTrue(t2_trc.OLD_LOT_SID == t.LOT_SID);
            Assert.IsTrue(t2_trc.LOT_SID == _lot.SID);
            Assert.IsTrue(t2_trc.LOT == _lot.LOT);
            Assert.IsTrue(t2_trc.OPERATION == _lot.OPERATION);
            Assert.IsTrue(t2_trc.ROUTE_VER_OPER_SID == _lot.ROUTE_VER_OPER_SID);

            Assert.IsTrue(t2_trc.SERIAL_NUMBER_ID == t.SERIAL_NUMBER_ID);
            Assert.IsTrue(t2_trc.OLD_STATUS == SERIAL_STATUS);
            Assert.IsTrue(t2_trc.NEW_STATUS == t2.SERIAL_STATUS);
            Assert.IsTrue(t2_trc.OLD_SERIAL_NUMBER == SERIAL_NUMBER);
            Assert.IsTrue(t2_trc.NEW_SERIAL_NUMBER == t.SERIAL_NUMBER);

            Assert.IsTrue(t2_trc.ACTION == ACTION.WAFER_EXCHANGE.ToString());
			Assert.IsTrue(t2_trc.ACTION_REASON == _dtc.txnACTION.ReasonNo);
			Assert.IsTrue(t2_trc.ACTION_DESCRIPTION == _dtc.txnACTION.Desc);
		}, true);

        
		[TestMethod]
		public void _DTC_UpdateWaferShipCassette()
		=> _DBTest(Txn =>
		{
			var t = Txn.EFQuery<WP_LOT_WAFER_MAPPING>()
				.Reads().FirstOrDefault();
			var SERIAL_NUMBER_ID = t.SERIAL_NUMBER_ID;
			var SERIAL_NUMBER = t.SERIAL_NUMBER;
			var SERIAL_STATUS = t.SERIAL_STATUS;
			////測試變更狀態和序號

			t.SERIAL_NUMBER = DTC_Mark.未變更.ToInt();
			t.SHIP_CASSETTE = "test-SHIP_CASSETTE";
			Txn.DoTransaction(new DTC_UpdateWaferShipCassette(t));
			var t2 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
			Assert.IsTrue(t2.SERIAL_NUMBER == SERIAL_NUMBER);
			Assert.IsTrue(t2.SHIP_CASSETTE == t.SHIP_CASSETTE);

			t.SERIAL_NUMBER = 99;
			t.SHIP_CASSETTE = "test-SHIP_CASSETTE2";
			Txn.DoTransaction(new DTC_UpdateWaferShipCassette(t));
			var t3 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
			Assert.IsTrue(t3.PREV_SEAIAL == t2.SERIAL_NUMBER);
			Assert.IsTrue(t3.SERIAL_NUMBER == t.SERIAL_NUMBER);
			Assert.IsTrue(t3.SHIP_CASSETTE == t.SHIP_CASSETTE);

			t.SERIAL_NUMBER = 999;
			t.LOT = "test-LOT";
			t.LOT_SID = "test-LOT_SID";
			t.PARENT_LOT_SID = DTC_Mark.SplitLot.ToString();
			Txn.DoTransaction(new DTC_UpdateWaferShipCassette(t));
			var t4 = _WP_LOT_WAFER_MAPPING(Txn, SERIAL_NUMBER_ID);
			Assert.IsTrue(t4.PREV_SEAIAL == t3.SERIAL_NUMBER);
			Assert.IsTrue(t4.SERIAL_NUMBER == t.SERIAL_NUMBER);
			Assert.IsTrue(t4.SHIP_CASSETTE == t.SHIP_CASSETTE);
			Assert.IsTrue(t4.LOT == t.LOT);
			Assert.IsTrue(t4.LOT_SID == t.LOT_SID);
			Assert.IsTrue(t4.PARENT_LOT_SID == t3.LOT_SID);
		}, true);


		private static WP_LOT_WAFER_MAPPING_TRACE _WP_LOT_WAFER_MAPPING_TRACE(ITxnBase Txn)
		{
			return Txn.DapperQuery<WP_LOT_WAFER_MAPPING_TRACE>(@"select * from WP_LOT_WAFER_MAPPING_TRACE with(nolock) 
					where ACTION_LINK_SID = @ACTION_LINK_SID"
								, new { ACTION_LINK_SID=Txn.LinkSID })
							.FirstOrDefault();
		}

		private static WP_LOT_WAFER_MAPPING _WP_LOT_WAFER_MAPPING(ITxnBase Txn, string SERIAL_NUMBER_ID)
        {
            return Txn.DapperQuery<WP_LOT_WAFER_MAPPING>(@"select * from WP_LOT_WAFER_MAPPING with(nolock) 
					where SERIAL_NUMBER_ID = @SERIAL_NUMBER_ID"
                                , new { SERIAL_NUMBER_ID })
                            .FirstOrDefault();
        }

 


		[TestMethod]
		public void t_WaferPackage()
		=> _DBTest(Txn => {
			var inputSrc = FileApp.Read_SerializeJson<List<WP_LOT_WAFER_MAPPING>>(_log.t_WaferPackage("_tmp"));
			
			//檢視分群結果
			var grp_SHIP_CASSETTE = inputSrc
				.GroupBy(o => o.SHIP_CASSETTE)
				.ToDictionary(o => o.Key, o => {
					return new
					{
						SHIP_CASSETTE = Txn.EFQuery<ZZ_SHIP_CASSETTE>().Read(c => c.SHIP_CASSETTE_NO == o.Key),
						list_wafer = o.ToList(),
						isPackageHaveMultiGrade = o.GroupBy(c => c.GRADE).Count() > 1,
						grpLot = o.GroupBy(c => c.LOT_SID).ToDictionary(e => e.Key, e => e.ToList()),
					};
				});


			new FileApp(false).Write_SerializeJson(grp_SHIP_CASSETTE, _log.t_WaferPackage("_tmp"));

			//WIPServices.Process_WaferPackage(Txn, inputSrc,true);

			/* 原型
			grp_SHIP_CASSETTE.Any(c => {
				if (c.Value.isPackageHaveMultiGrade) 
					throw new System.Exception($"單一包材只允許一個等級(SHIP_CASSETTE_NO:{c.Key})");
				if (c.Value.SHIP_CASSETTE == null) 
					throw new System.Exception($"查無此包材號(SHIP_CASSETTE_NO:{c.Key})");
				var GRADE = c.Value.SHIP_CASSETTE.GRADE;
				var Wafer = c.Value.list_wafer[0];
				if (GRADE != null && GRADE != Wafer.GRADE) 
					throw new System.Exception($"WaferSN({Wafer.SERIAL_NUMBER_ID})設定的等級 與 包材(SHIP_CASSETTE_NO:{c.Key})等級不符");
				return false;
			});

			var grp_0 = grp_SHIP_CASSETTE.ElementAt(0);
			var is單批同包 = grp_SHIP_CASSETTE.Count == 1
				&& grp_0.Value.grpLot.Count == 1;

			if (is單批同包) {
				Txn.DoTransaction(new DTC_PackageWafer 
						(grp_0.Value.SHIP_CASSETTE
						, grp_0.Value.list_wafer
						));
            } else {
				foreach (var grp in grp_SHIP_CASSETTE){
					var grpByLOT = grp.Value.grpLot;
                    foreach (var grpLot in grpByLOT) {
						//拆批前,先檢查 當前的 是否為 單批多Wafer Y)需做拆批 N)不需再拆
						var is單批多Wafer = WIPServices.CountWaferByLot(grpLot.Key) > 1;
                        if (is單批多Wafer) {
                            var curSHIP_CASSETTE = grp.Value.SHIP_CASSETTE;
                            var srcUseCounts = grpLot.Value.Count;
                            var isOutOfLimit = curSHIP_CASSETTE.MAX_CAPACITY < (curSHIP_CASSETTE.CURRENT_CAPACITY + srcUseCounts);
                            if (isOutOfLimit) throw new System.Exception($"包材({grp.Key})超過可使用數量.");

                            var SplitList = new List<CustomerList>() { new CustomerList() { INum = grpLot.Value.Count } }; ;
                            Txn.GetLotInfo(grpLot.Key,true);
                            List<LotSplitInfo> SplitInfoList = WIPServices.SplitLots(Txn, SplitList);
                            var _newLot = Txn.GetLotInfo(SplitInfoList[0].LOT_SID);

                            //// 處理 子批註記
                            foreach (var posi in grpLot.Value) {
								posi.LOT = _newLot.LOT;
								posi.LOT_SID = _newLot.SID;
								posi.PARENT_LOT_SID = DTC_Mark.SplitLot.ToString();
							}
						}
                    }
					Txn.DoTransaction(new DTC_PackageWafer
						( grp.Value.SHIP_CASSETTE
						, grp.Value.list_wafer
						));
                }
			}*/
		}, true);

		[TestMethod]
		public void t_ExecCarrierLoad()
		=> _DBTest(Txn => {
			List<WP_LOT_WAFER_MAPPING> data = new List<WP_LOT_WAFER_MAPPING>();
			serv.ExecCarrierLoad("2-0002-02", data, true);
		}, true);


		[TestMethod]
		public void t_info_SHIP_CASSETTE()
		=> _DBTest(Txn => {
			d_ZZ_SHIP_CASSETTE info_SHIP_CASSETTE = Wafer_Services.info_SHIP_CASSETTE(Txn,"TestA")
				.parseData<d_ZZ_SHIP_CASSETTE>();
		}, false);

		[TestMethod]
		public void _DTC_SHIP_CASSETTE_setFinished()
		=> _DBTest(Txn => {
			Txn.DoTransaction(new Wafer.DTC_SHIP_CASSETTE_setFinished("TestA"));
		}, true);


		[TestMethod()]
		public void _Process_SHIP_CASSETTE()
		=> _DBTest(Txn => {
			var isTest = true;
			//if (true) { 
			//	var inputSrc = FileApp.Read_SerializeJson<List<WP_LOT_WAFER_MAPPING>>(_log.t_WaferPackage("_單包同批"));
			//	var r = Wafer_Services.Process_SHIP_CASSETTE(Txn,inputSrc, true);
			//	var x = r.parseData<d_Process_SHIP_CASSETTE>();
			//	var t1 = x.list.All(c => c.ROOT_LOT_SID == "單包同批");
			//	Assert.IsTrue(t1, "必須全部都為-單包單批");
			//}
			//if (false)
			//{
			//	var inputSrc = FileApp.Read_SerializeJson<List<WP_LOT_WAFER_MAPPING>>(_log.t_WaferPackage("_單包多批"));
				
			//	var r = Wafer_Services.Process_SHIP_CASSETTE(Txn, inputSrc, true);
			//	//var t1 = r.parseData<List<WP_LOT_WAFER_MAPPING>>().All(c => c.ROOT_LOT_SID == "單批同包");
			//	//new FileApp(false).Write_SerializeJson(r.Data, _log.t_WaferPackage("_r"));
			//	//new FileApp(false).Write_SerializeJson(r, _log.t_WaferPackage("_r"));
			//	//Assert.IsTrue(t1, "必須全部都為-單批同包");
			//}
            if (false)
            {
				// 程式原型 
    //            var inputSrc = FileApp.Read_SerializeJson<List<WP_LOT_WAFER_MAPPING>>(_log.t_WaferPackage("_單批多包"));

				//var srcSHIP_CASSETTE = inputSrc.GroupBy(c => c.SHIP_CASSETTE).ToDictionary(o => o.Key, o => {
				//	var Wafers = o.ToList();
				//	var info_SHIP_CASSETTE = Wafer_Services.info_SHIP_CASSETTE(o.Key);
				//	var isOver = info_SHIP_CASSETTE==null
				//		?true
				//		: info_SHIP_CASSETTE.MAX_CAPACITY < (info_SHIP_CASSETTE.CURRENT_CAPACITY + Wafers.Count)
				//		;
				//	var GRADEs = o.GroupBy(c => c.GRADE)
				//				.ToDictionary(e => e.Key, e => e.Select(c => c.SERIAL_NUMBER_ID)
				//			   .ToList());
				//	var isMultGrade = GRADEs.Count > 1;
				//	return new { isMultGrade, isOver, Wafers, info_SHIP_CASSETTE, GRADEs };
				//});

				//srcSHIP_CASSETTE.Any(c => {
				//	var _this = c.Value;
				//	if (!isTest && _this.info_SHIP_CASSETTE == null)
				//		Result.Invalid($"查無此包材號(SHIP_CASSETTE_NO:{c.Key})", null).ThrowException();
				//	if (_this.isMultGrade)
				//		Result.Invalid($"單一包材只允許一個等級(SHIP_CASSETTE_NO:{c.Key})", _this.GRADEs).ThrowException();
				//	return false;
				//});
				//new FileApp(false).Write_SerializeJson(srcSHIP_CASSETTE, _log.t_WaferPackage("_r1"));

				//var chk_Split = inputSrc.GroupBy(o => o.LOT_SID)
				//	.ToDictionary(o => o.Key
				//		, o => {
				//			var _key = o.ElementAt(0).SHIP_CASSETTE;
				//			//var g = o.Select(c=>c.SHIP_CASSETTE).ToList();
				//			//var SHIP_CASSETTEs = srcSHIP_CASSETTE.Where(c => g.Contains(c.Key)).ToList();
				//			var LotInfo = Txn.GetLotInfo(o.Key);
				//			var GrpWaferByShipCassette  = o.GroupBy(c => c.SHIP_CASSETTE)
				//				.ToDictionary(q => q.Key, q =>
				//				{
				//					var _wafers = q.ToList();
				//					return new d_Process_SHIP_CASSETTE.Lot_Wafers
				//					{
				//						isRow0 = _wafers[0].SHIP_CASSETTE == _key,
				//						wafers = _wafers
				//					};
				//				});
				//			var is單批多包 = GrpWaferByShipCassette.Count > 1;

				//			return new {is單批多包
				//				, LotInfo
				//				, GrpWaferByShipCassette };
				//		});
				//new FileApp(false).Write_SerializeJson(chk_Split, _log.t_WaferPackage("_r"));
 
				//var list_split = chk_Split.Where(c => c.Value.is單批多包).ToList();
    //            list_split.ForEach(grpLot => {
				//	var lotInfo = grpLot.Value.LotInfo;
				//	foreach (var x in grpLot.Value.GrpWaferByShipCassette) {
				//		if (x.Value.isRow0 == false) {
				//			Txn.LotInfo = lotInfo;
				//			LotSplitInfo _LotSplitInfo = null;
				//			if (!isTest){	
				//				var SplitList = new List<CustomerList>() { new CustomerList() { INum = x.Value.wafers.Count } }; ;
				//				List<LotSplitInfo> SplitInfoList = WIPServices.SplitLots(Txn, SplitList);
				//				_LotSplitInfo = SplitInfoList[0];
				//			}
				//			x.Value.wafers.ForEach(el => {
				//				var wafer = inputSrc.Find(c => c.SERIAL_NUMBER_ID == el.SERIAL_NUMBER_ID);
				//				if (isTest) { 
				//					wafer.PARENT_LOT_SID = DTC_Mark.SplitLot.ToString();
				//				}
				//				else { 
				//					wafer.LOT = _LotSplitInfo.LOT;
				//					wafer.LOT_SID = _LotSplitInfo.LOT_SID;
				//					wafer.PARENT_LOT_SID = DTC_Mark.SplitLot.ToString();
				//				}
				//			});
				//		}
				//	}
				//});

				//var txnACTION = TxnACTION.n(ACTION.WAFER_PACKAGE.ToString(), "", "");
				//foreach (var grp in srcSHIP_CASSETTE) {
					 
                
				


				/*
				先處理拆批 的問題 

				再處上包材的問題
					逐包材設定 wafer 
				 */


				//new FileApp(false).Write_SerializeJson(chk_Split, _log.t_WaferPackage("_r"));
                //Assert.IsTrue(t1, "必須全部都為-單批同包");
            }



            //List<WP_LOT_WAFER_MAPPING> data = new List<WP_LOT_WAFER_MAPPING>();
            //serv.ExecCarrierLoad("2-0002-02", data, true);
        }, true);


	}


}
