using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.Transaction;
using Genesis.Gtimes.Transaction.CAR;
using Genesis.Gtimes.Transaction.EQP;
using Genesis.Gtimes.Transaction.TOL;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.MES.WIP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.Transaction.TransactionUtility;
using Resources = RES.BLL;


namespace UnitTestProject.Mes
{
	public class 基礎進站 : BaseFun, IX_MesInOut
	{
		WIPFormSendParameter data;
		TxnDoItemInfo txnInfo;
		decimal scrapTotalQty;

        public DBController DBC
		{
			get { return Txn.DBC; }
		}
		public 基礎進站(WIPFormSendParameter data, bool isTest = false, bool isFlowTest = false)
		{
			this.data = data;
			this.isTest = isTest;
			this.isFlowTest = isFlowTest;
		}

		public void 資料初始化()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 機台載入物料批號
		/// </summary>
		public virtual 基礎進站 EqpLoadMLot
		{
			get
			{
				if (FlowTest(nameof(EqpLoadMLot))) return this;
				return this;
				var operinfo = Txn.GetOperationInfo();
				//TODO
				var Mlot = Txn.GetMLotInfo("");
				Txn.DoTransaction(new EQPTransaction.EquipmentLoadMtrLotTxn(Catch_Current.Eqp, Catch_InitRec.Oper, Mlot));
				Catch_Current.Eqp = null;
				return this;
			}
		}

		/// <summary>
		/// 機台載入批號
		/// </summary>
		public virtual 基礎進站 EqpLoadLot
		{
			get
			{
				if (FlowTest(nameof(EqpLoadLot))) return this;

				if (!string.IsNullOrEmpty(data.Select_Eqp))
				{
					var DBC = txnInfo.DBC;
					var data = txnInfo.Data;
					var lotInfo = txnInfo.LotInfo;

					var EqpInfo = Txn.GetEquipmentInfo(data.Select_Eqp);
					Check.Equipment(txnInfo, EqpInfo);
					Txn.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(EqpInfo, lotInfo));

					#region Tool 累計使用次數
					var equip = Txn.GetEquipmentInfo(data.Select_Eqp);
					ToolUtility.ToolFunction ToolFun = new ToolUtility.ToolFunction(DBC);
					DataTable dtTool = ToolFun.GetEqpOnToolList(equip.No);

					if (dtTool != null && dtTool.Rows.Count != 0)
					{
						List<IGtimesTransaction> txnCmds = new List<IGtimesTransaction>();
						foreach (DataRow row in dtTool.Rows)
						{
							ToolUtility.ToolInfo toolinfo = new ToolUtility.ToolInfo(DBC, row["TOOL_SID"].ToString(), ToolUtility.ToolInfo.IndexType.SID);
							decimal AddUseCount = ToolFun.GetToolAddUseCount(equip.No, toolinfo, lotInfo.QUANTITY);
							txnCmds.Add(new TOLTransaction.ToolAddUseCountTxn(toolinfo, AddUseCount));
						}
						Txn.DoTransaction(txnCmds.ToArray());
					}
					#endregion
				}
				return this;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// Ref)WIPTransactionExtension.RecordEDC
		public virtual 基礎進站 RecordEDC { 
			get {
				if (FlowTest(nameof(RecordEDC))) return this;

				if (data.EdcList != null)
				{
					var DBC = txnInfo.DBC;
					var _lotInfo = txnInfo.LotInfo;
					var sendData = txnInfo.Data;
					#region SetValue 

					var edcUser = Txn.GetUserInfo(txnInfo.UserNo);

					var tmpEquipment = _lotInfo.GetCurrentEquipmentInfo();

					if (!tmpEquipment.IsExist)
					{
						if (sendData.UseEqumentList != null && sendData.UseEqumentList.Count > 0)
						{
							tmpEquipment = new EquipmentUtility.EquipmentInfo(DBC, sendData.UseEqumentList[0].No, EquipmentUtility.IndexType.No);
						}
					}

					var edcinfoList = new List<LotUtility.LotEDCCreateInfo>();
					foreach (var item in sendData.EdcList)
					{
						//針對Type D的進行特殊處理
						if (item.DataType.ToUpper() == "D")
						{
							WIPTransactionExtension.RecordEDCForDataReference(txnInfo, tmpEquipment, item);
							continue;
						}

						string edcVersionSID = Convert.ToString(item.EDCVerSid);
						string edcParameterSID = Convert.ToString(item.EdcParameterSid);
						var parameterInfo = new EDCUtility.EDCVersionParameterInfo(DBC, edcVersionSID, edcParameterSID);

						List<object> valueList = WIPServices.parse_InputValueList_4Input(item, txnInfo);

						string ParaName = item.Display;
						string DataType = item.DataType;

						string MustInput = item.MustInput; //NEED_INPUT
						int TextPoint = item.InputValueList.Count;

						var edc = new LotUtility.LotEDCCreateInfo(_lotInfo, parameterInfo, valueList);
						edc.EDCRecordUser = edcUser;
						edc.EDCRecordEquipment = tmpEquipment;

						if (valueList.Count > 0)
							edcinfoList.Add(edc);
					}
					#endregion

					Txn.DoTransaction(new WIPTransaction.LotEDCTxn(edcinfoList));
				}
				return this;
			} 
		}
		public 基礎進站 更新FC_TOOL的彈性欄位
		{
			get
			{
				if (FlowTest(nameof(更新FC_TOOL的彈性欄位))) return this;
				if (data.EqpUseToolList != null)
				{
					foreach (var item in data.EqpUseToolList)
					{
						UpdateCommandBuilder update = new UpdateCommandBuilder(DBC, "FC_TOOL");
						update.UpdateColumn("ATTRIBUTE_01", item.ATTRIBUTE_01);
						update.UpdateColumn("ATTRIBUTE_02", item.ATTRIBUTE_02);
						update.UpdateColumn("ATTRIBUTE_03", item.ATTRIBUTE_03);
						update.UpdateColumn("ATTRIBUTE_04", item.ATTRIBUTE_04);
						update.UpdateColumn("ATTRIBUTE_05", item.ATTRIBUTE_05);
						update.UpdateColumn("ATTRIBUTE_06", item.ATTRIBUTE_06);
						update.UpdateColumn("ATTRIBUTE_07", item.ATTRIBUTE_07);
						update.UpdateColumn("ATTRIBUTE_08", item.ATTRIBUTE_08);
						update.UpdateColumn("ATTRIBUTE_09", item.ATTRIBUTE_09);
						update.UpdateColumn("ATTRIBUTE_10", item.ATTRIBUTE_10);
						update.UpdateColumn("ATTRIBUTE_11", item.ATTRIBUTE_11);
						update.UpdateColumn("ATTRIBUTE_12", item.ATTRIBUTE_12);
						update.UpdateColumn("ATTRIBUTE_13", item.ATTRIBUTE_13);
						update.UpdateColumn("ATTRIBUTE_14", item.ATTRIBUTE_14);
						update.UpdateColumn("ATTRIBUTE_15", item.ATTRIBUTE_15);
						update.UpdateColumn("ATTRIBUTE_16", item.ATTRIBUTE_16);
						update.UpdateColumn("ATTRIBUTE_17", item.ATTRIBUTE_17);
						update.UpdateColumn("ATTRIBUTE_18", item.ATTRIBUTE_18);
						update.UpdateColumn("ATTRIBUTE_19", item.ATTRIBUTE_19);
						update.UpdateColumn("ATTRIBUTE_20", item.ATTRIBUTE_20);
						update.UpdateColumn("ATTRIBUTE_21", item.ATTRIBUTE_21);
						update.UpdateColumn("ATTRIBUTE_22", item.ATTRIBUTE_22);
						update.UpdateColumn("ATTRIBUTE_23", item.ATTRIBUTE_23);
						update.UpdateColumn("ATTRIBUTE_24", item.ATTRIBUTE_24);
						update.UpdateColumn("ATTRIBUTE_25", item.ATTRIBUTE_25);
						update.UpdateColumn("ATTRIBUTE_26", item.ATTRIBUTE_26);
						update.UpdateColumn("ATTRIBUTE_27", item.ATTRIBUTE_27);
						update.UpdateColumn("ATTRIBUTE_28", item.ATTRIBUTE_28);
						update.UpdateColumn("ATTRIBUTE_29", item.ATTRIBUTE_29);
						update.UpdateColumn("ATTRIBUTE_30", item.ATTRIBUTE_30);
						update.UpdateColumn("ATTRIBUTE_31", item.ATTRIBUTE_31);
						update.UpdateColumn("ATTRIBUTE_32", item.ATTRIBUTE_32);
						update.UpdateColumn("ATTRIBUTE_33", item.ATTRIBUTE_33);
						update.UpdateColumn("ATTRIBUTE_34", item.ATTRIBUTE_34);
						update.UpdateColumn("ATTRIBUTE_35", item.ATTRIBUTE_35);
						update.UpdateColumn("ATTRIBUTE_36", item.ATTRIBUTE_36);
						update.UpdateColumn("ATTRIBUTE_37", item.ATTRIBUTE_37);
						update.UpdateColumn("ATTRIBUTE_38", item.ATTRIBUTE_38);
						update.UpdateColumn("ATTRIBUTE_39", item.ATTRIBUTE_39);
						update.UpdateColumn("ATTRIBUTE_40", item.ATTRIBUTE_40);
						update.UpdateColumn("UPDATE_USER", txnInfo.UserNo);
						update.UpdateColumn("UPDATE_DATE", txnInfo.ExeTime);

						update.WhereAnd("TOOL_SID", item.TOOL_SID);
						txnInfo.DoTransaction(update.GetCommand());
					}

				}
				Catch_Current.Lot.ReLoad(DBC);
				return this;
			}
		}

		/// <summary>
		/// 跟 基礎出站 站大同小異 , 權衡後,還是決定分開各自寫
		/// </summary>
		public virtual 基礎進站 MutilUser處理
		{
			get
			{
				if (FlowTest(nameof(MutilUser處理))) return this;
				
				if (data.User_list != null)
				{
					var mutilUsercommands = WIPServices.GetMutilUserCommands(txnInfo);
					Txn.DoTransaction(mutilUsercommands);
				}
				return this;
			}
		}
 


		public virtual 基礎進站 批號進站
		{
			get
			{
				if (FlowTest(nameof(批號進站))) return this;
				if (Catch_Current.Lot == null) Catch_Current.Lot = Txn.GetLotInfo();
				
				txnInfo.DoTransaction(new WIPTransaction.LotCheckInTxn(Catch_Current.Lot));
				Catch_Current.Lot = null;
				return this;
			}
		}

        public virtual 基礎進站 寫入當下的Recipe
		{
			get
			{
				if (FlowTest(nameof(寫入當下的Recipe))) return this;
				if (SystemConfig.IsRecordOperationRecipeHist)
				{
					var RouteVerOperInfo = new RouteUtility.RouteVerOperationInfo(txnInfo.DBC, Catch_InitRec.Lot.ROUTE_VER_OPER_SID, RouteUtility.IndexType.SID);
					var OperRecipe = WIPOperConfigServices.GetOperRecipe(txnInfo.DBC, Catch_InitRec.Lot, RouteVerOperInfo);
					if (OperRecipe != null)
					{
						var cmds = new List<IDbCommand>();
						foreach (var item in OperRecipe)
						{
							var sid = txnInfo.DBC.GetSID();

							InsertCommandBuilder insert = new InsertCommandBuilder(txnInfo.DBC, "WP_LOT_RECIPE");
							insert.InsertColumn("LOT_RECIPE_SID", sid);
							insert.InsertColumn("LOT_SID", txnInfo.LotInfo.SID);
							insert.InsertColumn("LOT", txnInfo.LotInfo.LOT);
							insert.InsertColumn("ROUTE_VER_OPER_SID", RouteVerOperInfo.RouteVerOperSid);
							insert.InsertColumn("OPERATION", txnInfo.LotInfo.OPERATION);
							insert.InsertColumn("ACTION", "RecipeRecord");
							insert.InsertColumn("APPLICATION_NAME", txnInfo.ApplicationName);
							insert.InsertColumn("ACTION_LINK_SID", txnInfo.LinkSID);

							insert.InsertColumn("RECIPE_VER_PARA_SID", item.RECIPE_VER_PARA_SID);
							insert.InsertColumn("RECIPE_SID", item.RECIPE_SID);
							insert.InsertColumn("RECIPE_NO", item.RECIPE_NO);
							insert.InsertColumn("RECIPE_NAME", item.RECIPE_NAME);
							insert.InsertColumn("RECIPE_VALUE", item.VALUE);

							insert.InsertColumn("CREATE_USER", txnInfo.UserNo);
							insert.InsertColumn("CREATE_DATE", txnInfo.ExeTime);

							insert.InsertColumn("UPDATE_USER", txnInfo.UserNo);
							insert.InsertColumn("UPDATE_DATE", txnInfo.ExeTime);
							cmds.Add(insert.GetCommand());
						}
						txnInfo.DoTransaction(cmds.ToArray());
					}
				}
				return this;
			}
		}

		public virtual 基礎進站 批號數量分設備投產
		{
			get
			{
				if (FlowTest(nameof(批號數量分設備投產))) return this;
				if (data.Setting.AnySet.IsMultiLotSpitEqps)
				{
					if (data.UseEqumentList == null || data.UseEqumentList.Count == 0)  throw new Exception("請選擇設備");
					#region 進站不進行拆數量的處理

					//var LotOnEqpTotal = data.Lot_Eqps_List.Sum(x => Convert.ToDecimal(x.Value));
					////表示不過站，在當站至所有批號完成後才作過站的相關交易
					//if(LotOnEqpTotal != _lotInfo.QUANTITY)
					//{
					//    _wipCheckItem.IsGoNextTask = false;
					//    //throw new Exception($"批號數量必需全數分配至{RES.BLL.Face.EQP}");
					//}

					#endregion
					if (Catch_Current.Lot == null) Catch_Current.Lot = Txn.GetLotInfo();
					foreach (var item in data.UseEqumentList)
					{
						//批號減去數量
						//var changeLotQty = new WIPTransaction.LotChangeQuantityTxn(_lotInfo);
						//changeLotQty.NewQuantity = _lotInfo.QUANTITY -(Convert.ToDecimal(item.Value));
						//gtimesTxn.Add(changeLotQty);
						//var commands = gtimesTxn.GetTransactionCommands();
						//gtimesTxn.DoTransaction(commands, tx);
						//_lotInfo = new LotUtility.LotInfo(DBC, data.Lot, IndexType.NO);

						//gtimesTxn.GetTransactionCommands()


						#region 檢查 Tool的可用性

						#region Load Eqp Tool

						ToolUtility.ToolFunction ToolFunction = new ToolUtility.ToolFunction(DBC);
						var dtToolList = ToolFunction.GetEqpOnToolList(item.No);

						if (dtToolList != null)
						{
							for (int i = 0; i < dtToolList.Rows.Count; i++)
							{
								string TOOL_NO = dtToolList.Rows[i]["TOOL_NO"].ToString();
								ToolUtility.ToolFunction fun = new ToolUtility.ToolFunction(DBC);
								ToolUtility.ToolInfo toolinfo = new ToolUtility.ToolInfo(DBC, TOOL_NO, ToolUtility.ToolInfo.IndexType.No);
								ToolUtility.ToolStateInfo Toolstateinfo = new ToolUtility.ToolStateInfo(DBC, dtToolList.Rows[i]["STATE_NO"].ToString(), ToolUtility.ToolStateInfo.IndexType.No);

								decimal AddUseCount = fun.GetToolAddUseCount(item.No, toolinfo, Txn.LotInfo.QUANTITY);

								if (Toolstateinfo.RUN_FLAG == "F")
								{
									//零配件{0}狀態為{1}，禁止使用!
									throw new Exception(string.Format(Resources.Message.ToolStateNotAllowUser, TOOL_NO, Toolstateinfo.STATE_NO));
								}
								if (toolinfo.MAX_USE_COUNT <= toolinfo.USE_COUNT)
								{
									//零配件{0}最大使用次數已經達到{1}!
									throw new Exception(string.Format(Resources.Message.ToolMaxUseError, TOOL_NO, toolinfo.MAX_USE_COUNT.ToString()));
								}
								if (toolinfo.MAX_USE_COUNT < toolinfo.USE_COUNT + AddUseCount)
								{
									//零配件{0}剩餘使用次數不足，剩餘{1}次。
									throw new Exception(string.Format(Resources.Message.ToolRemainUseNotEnough, TOOL_NO, (toolinfo.MAX_USE_COUNT - toolinfo.USE_COUNT).ToString()));
								}
							}
						}
						 


						#endregion


						#endregion


						//各別上機台
						var EqpInfo = new EquipmentUtility.EquipmentInfo(DBC, item.SID, EquipmentUtility.IndexType.SID);
						txnInfo.DoTransaction(new EQPTransaction.EquipmentLoadLotTxn(EqpInfo, Txn.LotInfo));


						#region Tool 累計使用次數
						EqpInfo.ReLoad(DBC);
						ToolUtility.ToolFunction ToolFun = new ToolUtility.ToolFunction(DBC);
						DataTable dtTool = ToolFun.GetEqpOnToolList(EqpInfo.No);

						//TODO:效能優化
						if (dtTool != null && dtTool.Rows.Count != 0)
						{
							var _cmd = new List<IGtimesTransaction>();
							foreach (DataRow row in dtTool.Rows) {
								ToolUtility.ToolInfo toolinfo = new ToolUtility.ToolInfo(DBC, row["TOOL_SID"].ToString(), ToolUtility.ToolInfo.IndexType.SID);
								decimal AddUseCount = ToolFun.GetToolAddUseCount(EqpInfo.No, toolinfo, Txn.LotInfo.QUANTITY);
								_cmd.Add(new TOLTransaction.ToolAddUseCountTxn(toolinfo, AddUseCount));
							}
							Txn.DoTransaction(_cmd);
						}
						#endregion
					}
					 
				}
				return this;
			}
		}

		public virtual 基礎進站 處理混工單處理
		{
			get
			{
				if (FlowTest(nameof(處理混工單處理))) return this;
				var _lotInfo = Txn.LotInfo;
				if (data.InputMixWOLots != null)
				{
					var subLotList = new List<LotUtility.LotInfo>();
					foreach (var item in data.InputMixWOLots)
					{
						LotUtility.LotInfo sublot = new LotUtility.LotInfo(DBC, item.Lot, LotUtility.IndexType.NO);
						List<IDbCommand> commands = new List<IDbCommand>();
						CheckTimeUtility.CheckTimeFunctions chkFun = new CheckTimeUtility.CheckTimeFunctions(DBC);
						System.Data.DataView dvCheckTime;
						var sqlcmd = new TransactionUtility.AddSQLCommandTxn();



						#region Max QTiem 時間控管

						dvCheckTime = chkFun.GetWpChecktimeData(sublot.LOT, sublot.GetRouteVersionOperationInfo().OPERATION_NO, "T");
						if (dvCheckTime != null)
						{
							dvCheckTime.RowFilter = "CHECK_TYPE='MaxQTime'";

							if (dvCheckTime.Count > 0)
							{
								for (int k = 0; k < dvCheckTime.Count; k++)
								{
									List<Column> modifyColumns = new List<Column>();
									Column ENABLE_FLAG = new Column("ENABLE_FLAG", "T", "F");
									modifyColumns.Add(ENABLE_FLAG);
									//
									CheckTimeUtility.CheckTimeTransaction tran = new CheckTimeUtility.CheckTimeTransaction();
									commands.AddRange(tran.ModifyTransaction(DBC, "MargeLot", dvCheckTime[k]["LOT_TIMECONTROL_SID"].ToString(), sublot.LOT,
										modifyColumns, Txn.UserNo, Txn.ExeTime));
								}

								sqlcmd.Commands = commands;
								Txn.DoTransaction(sqlcmd);
							}
						}
						#endregion

						#region Min QTiem 時間控管

						dvCheckTime = chkFun.GetWpChecktimeData(sublot.LOT, sublot.GetRouteVersionOperationInfo().OPERATION_NO, "T");
						if (dvCheckTime != null)
						{
							dvCheckTime.RowFilter = "CHECK_TYPE='MinQTime'";

							if (dvCheckTime.Count > 0)
							{
								for (int k = 0; k < dvCheckTime.Count; k++)
								{
									List<Column> modifyColumns = new List<Column>();
									Column ENABLE_FLAG = new Column("ENABLE_FLAG", "T", "F");
									modifyColumns.Add(ENABLE_FLAG);
									//
									CheckTimeUtility.CheckTimeTransaction tran = new CheckTimeUtility.CheckTimeTransaction();
									commands.AddRange(tran.ModifyTransaction(DBC, "MergeLot", dvCheckTime[k]["LOT_TIMECONTROL_SID"].ToString(), sublot.LOT,
										modifyColumns, Txn.UserNo, Txn.ExeTime));
							}

								sqlcmd.Commands = commands;
								Txn.DoTransaction(sqlcmd);

							}
						}
						#endregion


						if (Txn.LotInfo.ROUTE_VER_OPER_SID != sublot.ROUTE_VER_OPER_SID)
						{
							throw new Exception(Resources.Message.MustTheSameOperation);
						}

						subLotList.Add(sublot);

						#region 母、併批載具處理

						var carrierInfo = _lotInfo.GetCurrentCarrierInfo();
						//母批有載具資訊
						if (carrierInfo.IsExist)
						{
							CarrierUtility.CarrierInfo subCarrier = sublot.GetCurrentCarrierInfo(); //併批載具資訊

							if (subCarrier.IsExist)
							{
								//下併批載具資訊
								Txn.DoTransaction(new CARTransaction.CarrierUnloadLotTxn(carrierInfo, sublot));
								carrierInfo = new CarrierUtility.CarrierInfo(DBC, carrierInfo.SID, CarrierUtility.CarrierInfo.IndexType.SID);
							}

							//併批數量加回母批
							Txn.DoTransaction(new CARTransaction.CarrierChangeCapacityTxn(carrierInfo, carrierInfo.CURRENT_CAPACITY + sublot.QUANTITY));
							carrierInfo = new CarrierUtility.CarrierInfo(DBC, carrierInfo.No, CarrierUtility.CarrierInfo.IndexType.No);
						}

						#endregion
					}
					Txn.DoTransaction(new WIPTransaction.LotMergeTxn(_lotInfo, subLotList));
					Txn.LotInfo.ReLoad(DBC);
				}
				return this;
			}
		}

 
		public virtual IResult Process()
		=> TxnBase.LzDBTrans(nameof(基礎出站)
			, TxnReason.n("")
			, (txn) =>
			{
				Txn = txn;
				//過濾性用法

				this.資料初始化();

				txnInfo = new TxnDoItemInfo()
				{

					DBC = txn.DBC,
					Data = data,
					GtimesTxn = txn.GtimesTxn,
					LinkSID = txn.LinkSID,
					ExeTime = txn.ExeTime,
					LotInfo = Catch_Current.Lot,
					//產出數、預設為出站數-報廢數
					//OutputQty = data.Current.LotInfo.QUANTITY - scrapTotalQty,
					OldLotinfo = Catch_InitRec.Lot,
					OldEqpInfo = Catch_InitRec.Eqp,
					Txn = txn.Txn,
					UserNo = txn.UserNo,
					ApplicationName = txn.ApplicationName,
					MESWIPCheckItem = new WIPCheckItem(),
					Sqlcmd = txn.Sqlcmd
				};



				return this
						.RecordEDC
						.更新FC_TOOL的彈性欄位
						.批號進站
						.寫入當下的Recipe
						.EqpLoadLot
						.EqpLoadMLot
						.MutilUser處理
						.批號數量分設備投產
						.處理混工單處理
						.End();

			}
			,false
		);

        public virtual IResult End()
        {
 			txnInfo.DoTransaction
				(new WIPTransaction.GoToNextTaskTxn(Txn.LotInfo)
				, new WIPTransaction.EndOfLotTxn(Txn.LotInfo)
				);
			Txn.LotInfo.ReLoad(DBC);
			return Txn.result;
        }
    }
}
