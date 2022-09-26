//P:\MyLab\UnitTest\GTI\Mes/MesInOut.cs
//P:\MyLab\UnitTest\GTI\Log/MesInOut/
using BLL.Base;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.Transaction.EQP;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.MES.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnitTestProject.TestUT;
using static BLL.MES.DataViews.PartData;
using static BLL.MES.WIPInjectServices;
using static Genesis.Gtimes.Transaction.TransactionUtility;
using Console = System.Diagnostics.Debug;

namespace UnitTestProject
{
 

	public abstract class BaseFun {
		public bool isFlowTest { get; set; }

		public List<string> _fun_flow = new List<string>();
		public bool FlowTest(string fun_name) {
			if (isFlowTest) _fun_flow.Add(fun_name);
			return isFlowTest;
		}
	}

	public class 基礎出站:BaseFun
	{
		WIPFormSendParameter data;
		ITxnBase Txn;
		TxnDoItemInfo txnInfo;
		bool isTest;
		decimal scrapTotalQty;
		public LotUtility.LotInfo CurrentLot {
			get { return data.Current.LotInfo; }
			set { data.Current.LotInfo = value; }
		}
		public DBController DBC
		{
			get { return Txn.DBC; }
		}
		

		public 基礎出站() { }
		public 基礎出站(WIPFormSendParameter data, bool isTest = false,bool isFlowTest = false) {
			this.data = data;
			this.isTest = isTest;
			this.isFlowTest = isFlowTest;
		}

		public virtual 基礎出站 資料初始化()
		{
			if (FlowTest(nameof(資料初始化))) return this;

			var LotInfo 
				= data.Old.LotInfo 
				= Txn.GetLotInfo(data.Lot, isQueryByLotNO: true);
			if (LotInfo == null  || LotInfo.IsExist == false)
				Result.NotExist(data.Lot).ThrowException();

			if (LotInfo.STATUS.ToUpper() != LotStatus.Run.ToString().ToUpper())
				new Result(string.Format(RES.BLL.Message.LotStatusError, data.Lot))
				{
					Data = new { LotInfo }
				}
				.ThrowException();

			data.Current = new Catch()
			{
				LotInfo = Txn.GetLotInfo(data.Lot,isQueryByLotNO:true),
				Equipment = LotInfo.GetCurrentEquipmentInfo()
			};

			return this;
		}

		public virtual 基礎出站 處理人員
		{
			get
			{
				if (FlowTest(nameof(處理人員))) return this;
				
				string userNO = string.Empty;
				if (data.User_list == null && string.IsNullOrEmpty(data.Select_User))
					new Result(RES.BLL.Message.SetsuccessfullyNo)
					.ThrowException();

				//預設值
				Txn.UserNo = data.Select_User;

				if (data.User_list != null)
				{
					//選擇多User處理
					//使用 登入的使用者當交易對像
					var currentUser = ServicesBase.GetLoginUser(isTest);
					Txn.UserNo = currentUser.UserNo;

					var mutilUsercommands = WIPServices.GetMutilUserCommands(txnInfo);
					Txn.DoTransaction(mutilUsercommands.ToArray());
					CurrentLot.ReLoad();
				}
				return this;
			}
		}

		public virtual 基礎出站 QTimeCheck {
		get{
			if (FlowTest(nameof(QTimeCheck))) return this;

			WIPServices.QTimeCheck(txnInfo);
			return this;
		}}

		public virtual 基礎出站 寫入批號備註
		{
			get
			{
				if (FlowTest(nameof(寫入批號備註))) return this;

				if (data.LotModifyData != null && txnInfo.LotInfo.NOTE != data.LotModifyData.Note)
				{
					var oAttr = new WIPTransaction.LotChangeAttributeTxn
						( CurrentLot
						, "NOTE"
						, CurrentLot.NOTE
						, data.LotModifyData.Note);
					oAttr.TransactionName = "LotChangeNote";
					Txn.DoTransaction(oAttr);
					CurrentLot.ReLoad();
				}
				return this;
			}
		}

		public virtual 基礎出站 寫入批號等級
		{
			get
			{
				if (FlowTest(nameof(寫入批號等級))) return this;

				if (data.SelectGrade != null)
				{
					var oAttr = new WIPTransaction.LotChangeAttributeTxn
						( CurrentLot
						, "Grade"
						, CurrentLot.GRADE
						, data.SelectGrade.No);
					oAttr.TransactionName = "LotChangeGrade";
					txnInfo.DoTransaction(oAttr);
					CurrentLot.ReLoad();
				}
				return this;
			}
		}


		/// <summary>
		/// 考慮到 scrapTotalQty 是全域變數 , 如無必要 ,需避免重覆執行此段程序
		/// </summary>
		public virtual 基礎出站 處理報廢 
		{ 
			get{
				if (FlowTest(nameof(處理報廢))) return this;

				scrapTotalQty = data.ScrapList == null ? 0 : data.ScrapList.Sum(x => x.INum);

				//總報廢數要加上良品數的報廢
				if (data.GoodComponent == null)
					data.GoodComponent = new GoodComponentStruct();

				scrapTotalQty += data.GoodComponent.LotScrapQty;
			
				txnInfo.OutputQty = CurrentLot.QUANTITY - scrapTotalQty;
				return this;
			}
		}
		
		public virtual 基礎出站 批號出站_重工
		{
			get
			{
				if (FlowTest(nameof(批號出站_重工))) return this;

				if (data.Setting.CheckOutSet.IsReWork &&  data.ReWorkOperList != null)
				{
					var update = new UpdateCommandBuilder(txnInfo.DBC, "WP_LOT");
					update.UpdateColumn("ATTRIBUTE_10", data.ReWorkOperList.Status);
					update.WhereAnd("LOT_SID", txnInfo.OldLotinfo.SID);
					Txn.DoTransaction(update.GetCommand());
					CurrentLot.ReLoad();
				}
				return this;
			}
		}
		public virtual 基礎出站 處理物料批
		{
			get
			{
				if (FlowTest(nameof(批號出站_重工))) return this;

				var DBC = Txn.DBC;
				var operFunction = new OperationUtility.OperationFunction(DBC);
				var operPartSetting = operFunction.GetOperPartSetting(CurrentLot.OPER_SID)?.AsEnumerable().Select(s => s.Field<string>("NO"));
				data.OperationPartUseList = data.OperationPartUseList ?? new List<OperationMTRLotList>();
				var operPartError = operPartSetting?.Except(data.OperationPartUseList.Select(s => s.PARTNO));
				if (operPartError?.Count() > 0)
				{
					throw new Exception($"料號{string.Join(", ", operPartError.Select(s => s))}未刷進物料批");
				}

				if (data.OperationPartUseList != null)
				{
					List<IGtimesTransaction> txnCmds = new List<IGtimesTransaction>();
					foreach (var item in data.OperationPartUseList)
					{
						//耗用批號記錄
						//if (string.IsNullOrEmpty(item.EQPNo))
						//{
						if (item == null)
						{
							continue;
						}
						if (item.UseQty > 0)
						{
							var mLot = new Genesis.Gtimes.MTR.MtrLotUtility.MtrLotInfo(DBC, item.MTR_LOT
								, Genesis.Gtimes.MTR.MtrLotUtility.MtrLotInfo.IndexType.No);
							LotUtility.ILotConsumption consumpMLot = new LotUtility.LotConsumptionMlotQuantity(mLot, (decimal)item.UseQty, 0, 0);
							WIPTransaction.LotConsumptionTxn lotConstxn = new WIPTransaction.LotConsumptionTxn(CurrentLot, consumpMLot);
							txnCmds.Add(lotConstxn);
						}

					}
					txnInfo.DoTransaction(txnCmds.ToArray());
					CurrentLot.ReLoad();
				}
				return this;
			}
		}

		public virtual 基礎出站 Edc收集處理
		{
			get
			{
				if (FlowTest(nameof(Edc收集處理))) return this;

				if (data.EdcList != null)
				{
					WIPTransactionExtension.RecordEDC(txnInfo);
				}
				return this;
			}
		}

        public 基礎出站 更新FC_TOOL的彈性欄位 
		{ 
			get
			{
				if (FlowTest(nameof(Edc收集處理))) return this;

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

						CurrentLot.ReLoad();
					}

				}
				return this;
			}
		}

        public 基礎出站 暫留線邊倉功能
		{
			get
			{
				if (FlowTest(nameof(暫留線邊倉功能))) return this;

				if (data.Setting.CheckOutSet.IsLotSplitQTY)
				{
					//工時填入Group的ID
					if (string.IsNullOrEmpty(CurrentLot.ATTRIBUTE_04))
					{
						//如是批號拆設備、暫留線邊倉，每次出站時寫入一個GroupID當下次使用識別處理
						UpdateCommandBuilder update = new UpdateCommandBuilder(txnInfo.DBC, "WP_LOT");
						update.UpdateColumn("ATTRIBUTE_04", Txn.GetSID());
						update.WhereAnd("LOT_SID", txnInfo.LotInfo.SID);
 
						Txn.DoTransaction(update.GetCommand());
						CurrentLot.ReLoad();
					}


					if (data.LotSplitQty == 0 && scrapTotalQty == 0)
						new Result("數量不得為零") { 
							Data = new { data.LotSplitQty , scrapTotalQty } 
						}
						.ThrowException();

					#region Check List

					var LotQty = CurrentLot.QUANTITY;
					decimal IsCheckoutQty = 0;
					//取得已出站資料、聚鼎客製
					var sql = "Select SUM(MOVE_QTY) as MOVE_QTY from ZZ_LOT_MOVE where LOT_SID = @LOT_SID AND ROUTE_VER_OPER_SID = @ROUTE_VER_OPER_SID AND GROUP_SID = @GROUP_SID ";
					var parameters = new List<IDbDataParameter>();
					DBC.AddCommandParameter(parameters, "LOT_SID", CurrentLot.SID);
					DBC.AddCommandParameter(parameters, "ROUTE_VER_OPER_SID", CurrentLot.ROUTE_VER_OPER_SID);
					DBC.AddCommandParameter(parameters, "GROUP_SID", CurrentLot.ATTRIBUTE_04);

					var reader = DBC.Select(sql, parameters);
					if (reader != null)
					{
						if (!string.IsNullOrEmpty(reader.Rows[0][0].ToString()))
						{
							IsCheckoutQty = Convert.ToDecimal(reader.Rows[0][0]);
						}
					}


					//過站量
					var CurrentCheckOutQty = data.LotSplitQty;

					//當前過站量加前次過站量
					var CheckOutTotal = IsCheckoutQty + CurrentCheckOutQty;

					//過站量+報廢數
					CheckOutTotal += scrapTotalQty;



					if (CheckOutTotal > CurrentLot.QUANTITY)  
						new Result("過站數量超過原始批號數量，請進行調整").ThrowException();

					//進行變更狀態為：數量全數轉移，批號狀態改成 WipInv ，要由完工作業進行出站
					if (CheckOutTotal >= CurrentLot.QUANTITY)
					{
						var change = new WIPTransaction.LotChangeStatusTxn(CurrentLot);
						change.NewStatus = "WipInv";
						Txn.DoTransaction(change);

						#region 109 11/11 Joseph補充：拆設備必需手動下機台，當數量轉移完成後

						var equip = data.Current.Equipment;
						if (equip.IsExist)
						{
							#region Equip unload lot
							var equipUnloadLot = new EQPTransaction.EquipmentUnloadLotTxn(equip, CurrentLot);
							Txn.DoTransaction(equipUnloadLot);

							CurrentLot.ReLoad();

							equip = new EquipmentUtility.EquipmentInfo(DBC, equip.No, EquipmentUtility.IndexType.No);
							EQPTransaction.EndOfEquipmentTxn endEQPTxn = new EQPTransaction.EndOfEquipmentTxn(equip);
							Txn.DoTransaction(endEQPTxn);
							#endregion

						}

						//重取資料
						CurrentLot.ReLoad();
						#endregion

					}
					#endregion

					var doInserList = new List<IDbCommand>();



					var _oldInfo = data.Old.LotInfo;
					InsertCommandBuilder insert = new InsertCommandBuilder(txnInfo.DBC, "ZZ_LOT_MOVE");
					insert.InsertColumn("LOT_MOVE_SID", Txn.GetSID());
					insert.InsertColumn("LOT_SID", _oldInfo.SID);
					insert.InsertColumn("LOT", _oldInfo.LOT);
					insert.InsertColumn("GROUP_SID", CurrentLot.ATTRIBUTE_04);
					insert.InsertColumn("ROUTE_VER_OPER_SID", _oldInfo.ROUTE_VER_OPER_SID);
					insert.InsertColumn("STATUS", "LotMoveQty");
					insert.InsertColumn("OPERATION", _oldInfo.OPERATION);
					insert.InsertColumn("ACTION", "LotMoveQty");
					insert.InsertColumn("APPLICATION_NAME", txnInfo.ApplicationName);
					insert.InsertColumn("ACTION_LINK_SID", txnInfo.LinkSID);
					insert.InsertColumn("ACTION_LINK_SID_TRACE", txnInfo.LinkSID);
					insert.InsertColumn("MOVE_QTY", data.LotSplitQty);
					insert.InsertColumn("CREATE_USER", txnInfo.UserNo);
					insert.InsertColumn("CREATE_DATE", txnInfo.ExeTime);
					insert.InsertColumn("UPDATE_USER", txnInfo.UserNo);
					insert.InsertColumn("UPDATE_DATE", txnInfo.ExeTime);

					doInserList.Add(insert.GetCommand());

					//分設備後撰寫一筆Hist資料
					var doHist = BLL.MES.ExpanTxn.HistTransaction.AddLotHist(txnInfo, "LOT_STAY_OPERATION", "ZZ_LOT_MOVE", "", "");
					doInserList.Add(doHist);

					txnInfo.DoTransaction(doInserList.ToArray());
					txnInfo.MESWIPCheckItem.IsGoNextTask = false;

					//重取資料
					CurrentLot.ReLoad();
				}

				return this;
			}
		}

		public virtual void End() { Console.WriteLine(nameof(End)); }

		
		public virtual IResult Process()
		=> TxnBase.LzDBTrans(nameof(基礎出站)
			, TxnReason.n("")
            , (txn) => {
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
					LotInfo = data.Current.LotInfo,
					//產出數、預設為出站數-報廢數
					//OutputQty = data.Current.LotInfo.QUANTITY - scrapTotalQty,
					OldLotinfo = data.Old.LotInfo,
					OldEqpInfo = data.Old.Equipment,
					Txn = txn.Txn,
					UserNo = txn.UserNo,
					ApplicationName = txn.ApplicationName,
					MESWIPCheckItem = new WIPCheckItem(),
					Sqlcmd = txn.Sqlcmd
				};
				//todo  SPC 資料打包
				// SpcIntegrateInfo txnInfoBySPC = new SpcIntegrateInfo(_lotInfo: _lotInfo, _SpcHoldLotInfo: _oldInfo, _WIPFormSendParameter: data, _txnTime: txnTime, _linkSID: linkSID, _MesDB: DBC);

				//return this
				//		.處理人員
				//		.處理報廢
				//		.QTimeCheck
				//		.寫入批號備註
				//		.寫入批號等級
				//		.批號出站_重工
				//		.處理物料批
				//		.Edc收集處理
				//		.更新FC_TOOL的彈性欄位
				//		.暫留線邊倉功能

				//		.Txn.result
				//		;


				return this
						.處理人員
						.處理報廢
						.Edc收集處理
						.更新FC_TOOL的彈性欄位
						.處理物料批
						.Txn.result;

			}
			,isTest
		);
	}

 
}

