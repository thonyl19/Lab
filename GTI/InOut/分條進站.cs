using BLL.MES.DataViews;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject.Mes
{
    public class 分條進站 : BaseFun{
		WIPFormSendParameter data;
		ITxnBase Txn;
		TxnDoItemInfo txnInfo;
		bool isTest;
		decimal scrapTotalQty;
		public LotUtility.LotInfo CurrentLot
		{
			get { return data.Current.LotInfo; }
			set { data.Current.LotInfo = value; }
		}
		public DBController DBC
		{
			get { return Txn.DBC; }
		}
		public 分條進站(WIPFormSendParameter data, bool isTest = false, bool isFlowTest = false)
		{
			this.data = data;
			this.isTest = isTest;
			this.isFlowTest = isFlowTest;
		}

		/// <summary>
		/// 設備上批號
		/// </summary>
		public virtual 分條進站 EqpLoadLot
		{
			get
			{
				if (FlowTest(nameof(EqpLoadLot))) return this;

				//if (data.LotModifyData != null && txnInfo.LotInfo.NOTE != data.LotModifyData.Note)
				//{
				//	var oAttr = new WIPTransaction.LotChangeAttributeTxn
				//		(CurrentLot
				//		, "NOTE"
				//		, CurrentLot.NOTE
				//		, data.LotModifyData.Note);
				//	oAttr.TransactionName = "LotChangeNote";
				//	Txn.DoTransaction(oAttr);
				//	CurrentLot.ReLoad();
				//}
				return this;
			}
		}

		/// <summary>
		/// 設備上物料批
		/// </summary>
		public virtual 分條進站 EqpLoadMLot
		{
			get
			{
				if (FlowTest(nameof(EqpLoadMLot))) return this;

				//if (data.LotModifyData != null && txnInfo.LotInfo.NOTE != data.LotModifyData.Note)
				//{
				//	var oAttr = new WIPTransaction.LotChangeAttributeTxn
				//		(CurrentLot
				//		, "NOTE"
				//		, CurrentLot.NOTE
				//		, data.LotModifyData.Note);
				//	oAttr.TransactionName = "LotChangeNote";
				//	Txn.DoTransaction(oAttr);
				//	CurrentLot.ReLoad();
				//}
				return this;
			}
		}

		
	}
}
