using BLL.MES.DataViews;
using Genesis.Gtimes.Transaction.MTR;
using Genesis.Library.BLL.MES.OperTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.WIP.LotUtility;
using MTR = Genesis.Gtimes.MTR;
using MES = Genesis.Library.BLL.MES.OperTask;
using BLL.MES;
using Genesis.Gtimes.Transaction.EQP;
using static BLL.MES.WIPInjectServices;
using MDL.MES;
using System.Linq;
using Genesis.Gtimes.Common;
using Frame.Code.Web.Select;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.MTR;

namespace UnitTestProject
{
	[TestClass]
	public class t_GRF : _testBase
	{
		static class _log
		{
			/// <summary>
			/// 分條進站 的 執行
			/// </summary>
			internal static string ExecSlittingCheckIn
			{
				get
				{
					return FileApp.ts_Log(@"GRF\ExecSlittingCheckIn.json");
				}
			}
			internal static string 完工轉物料批_CheckOut
			{
				get
				{
					return FileApp.ts_Log(@"GRF\完工轉物料批_CheckOut.json");
				}
			}

			/// <summary>
			/// 貼合摺景 出站資訊 
			/// </summary>
			internal static string ExecPleatingSlittingCheckOut
			{
				get
				{
					return FileApp.ts_Log(@"GRF\ExecPleatingSlittingCheckOut.json");
				}
			}

			/// <summary>
			/// 分條出站- 物料批 報廢測試
			/// </summary>
			internal static string SlittingCheckOut_Case3
			{
				get
				{
					return FileApp.ts_Log(@"GRF\SlittingCheckOut_Case3.json");
				}
			}

		}


		#region [ Sample ] 
		/*
		[TestMethod]
		public void _Sample()
		=> _DBTest(Txn => {
			var lot = Txn.GetLotInfo("GTI22060711213497371");
			var c = lot.GetCurrentCarrierInfo();
			var exp = c.CURRENT_CAPACITY - 1;
			Txn.DoTransaction(new Carrier.DTC_AdjustmentCapacity(lot, c, -1));
			var act = lot.GetCurrentCarrierInfo().CURRENT_CAPACITY;
			Assert.AreEqual(exp, act,$"扣數後, 數值應為 {exp}");

			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));
			new FileApp().Write_SerializeJson(dt, _log.t_splitBIN);

			var _r = new FileApp().Read_SerializeJson(_log.t_splitBIN);
		}, true);
		*/
		#endregion
		[TestMethod]
		public void t_GRF_SlittingCheckIn()
		{ 
			var _r = new FileApp().Read_SerializeJson<WIPFormSendParameter>(_log.ExecSlittingCheckIn);
			new GRF_SlittingCheckIn().Process(_r, true);
		}


		[TestMethod]
		public void t_SlittingCheckOut_Case3()
		{
			var _r = new FileApp().Read_SerializeJson<WIPFormSendParameter>(_log.SlittingCheckOut_Case3);
			new SlittingCheckOut().Process(_r, true);
		}
		


		[TestMethod]
		public void t_生產批轉物料批()
		=> _DBTest((Txn) => {
			List<LotInfo> LotInfos = new List<LotInfo>() {
						Txn.GetLotInfo("分條_20221121-03.02",false,true)
						,Txn.GetLotInfo("分條_20221121-03.03",false,true)
			};
			var mlotCreateList = new List<MTR.MtrLotUtility.MtrLotCreateInfo>();
			var mlotInfoList = new List<MTR.MtrLotUtility.MtrLotInfo>();
			for (int i = 0; i < LotInfos.Count; i++)
			{
				var LotInfo = LotInfos[i];
				var MlotCreate = MES.Func.LotFinishedTransfromMLot(Txn, LotInfo);
				mlotCreateList.Add(MlotCreate);
			}
			Txn.DoTransaction(new MTRTransaction.CreateStartMtrLotTxn(mlotCreateList));
		}, true);


		[TestMethod]
		public void t_SlittingCheckIn_WOInfo()
		{
			new WO_Services().SlittingCheckIn_WOInfo("A_20021121002");
		}

		[TestMethod]
		public void t_LotFinlishTransfromMLot()
		{

			new LotFinlishTransfromMLot().Process(new WIPFormSendParameter(),true);
		}

		/// <summary>
		/// 壓合摺景出站
		/// </summary>
		[TestMethod]
		public void t_ExecPleatingSlittingCheckOut()
		{
			var _r = new FileApp().Read_SerializeJson<WIPFormSendParameter>(_log.ExecPleatingSlittingCheckOut);
			new GRF_PleatingSlittingCheckOut().Process(_r, true);
		}

		/// <summary>
		/// 以一般出站模式 做測試
		/// </summary>
		[TestMethod]
		public void t_完工轉物料批_CheckOut()
		{
			var _r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.完工轉物料批_CheckOut);
			new WIPServices().DoCheckOut(_r, true);
		}

		/// <summary>
		///  
		/// </summary>
		[TestMethod]
		public void t_查詢前階工單()
		{ 
			var r = WOServices.查詢前階工單("");
		}

		/// <summary>
		///  
		/// </summary>
		[TestMethod]
		public void t_()
		=> _DBTest(Txn => {
			//var Eqp = Txn.GetEquipmentInfo("GTI22120910222003769");
			////var r = WOServices.查詢前階工單("");
			//Txn.DoTransaction
			//	(new EQPTransaction.EquipmentChangeCapacityTxn(Eqp, 0));

			var r = Txn.EFQuery<WP_MTL_TRACE>().Reads().ToList();
		});

		[TestMethod]
		public void t_1()
		=> TxnBase.LzDBTrans("App", TxnACTION.n("MLOT_SCRAP"),Txn=>
        {

            var lot = Txn.GetLotInfo("分條_20221121-03", isQueryByLotNO: true);
            var operation = lot.GetRouteVersionOperationInfo();
            var equipment = lot.GetCurrentEquipmentInfo();
            var mlot = Txn.GetMLotInfo("GTI22112117405494215");
            var reason = new SelectModel();// Txn.GetReasonCodeInfo("GTI11110115550180956");
            var dbc = Txn.DBC;

            NewMethod(Txn, mlot, operation, equipment, reason);
            //var r = Txn.EFQuery<WP_MTL_TRACE>().Reads().ToList();
            return Txn.result;
        }, isTest:true);

        private static void NewMethod
			(ITxnBase Txn 
			, MtrLotUtility.MtrLotInfo mlot
			, RouteUtility.RouteVersionOperationInfo operation
			, EquipmentUtility.EquipmentInfo equipment
			, SelectModel reason)
        {
			var dbc = Txn.DBC;
			InsertCommandBuilder insert = new InsertCommandBuilder(dbc, "MT_LOT_SCRAP");
            insert.InsertColumn("MTL_SCRAP_SID", dbc.GetSID());
            insert.InsertColumn("MTL_SID", mlot.SID);
            insert.InsertColumn("LOT", mlot.LOT);
            insert.InsertColumn("MTL_LOT", mlot.MTR_LOT);
            insert.InsertColumn("ROUTE_VER_OPER_SID", operation.ROUTE_VER_OPER_SID);
            insert.InsertColumn("OPERATION", operation.OPERATION);
            insert.InsertColumn("ACTION", "MTL_SCRAP");
            insert.InsertColumn("APPLICATION_NAME", Txn.ApplicationName);
            insert.InsertColumn("ACTION_LINK_SID", Txn.LinkSID);
            insert.InsertColumn("ACTION_REASON", Txn?.ActionReason.ReasonNo);
            insert.InsertColumn("ACTION_DESCRIPTION", Txn?.ActionReason.Desc);
            if (!(equipment == null || equipment.IsExist == false))
            {
                insert.InsertColumn("EQP_SID", equipment.SID);
                insert.InsertColumn("EQP_NO", equipment.No);
                insert.InsertColumn("EQP_NAME", equipment.Name);
            }
            insert.InsertColumn("REASON_SID", reason.SID);
            insert.InsertColumn("REASON_NO", reason.No);
            insert.InsertColumn("REASON", reason.Display);
            //TODO:不確定,先且設 Attr3
            insert.InsertColumn("SCRAP_DESCRIPTION", reason.Attr03);
            insert.InsertColumn("SCRAP_QUNATITY", reason.INum);
            insert.InsertColumn("CANCEL_FLAG", "F");
            insert.InsertColumn("CREATE_USER", Txn.UserNo);
            insert.InsertColumn("CREATE_DATE", Txn.ExeTime);
            insert.InsertColumn("UPDATE_USER", Txn.UserNo);
            insert.InsertColumn("UPDATE_DATE", Txn.ExeTime);

            Txn.DoTransaction(insert.GetCommand());
        }

 
	}
}

