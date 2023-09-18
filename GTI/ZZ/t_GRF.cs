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
using AHTask = Genesis.Library.BLL.ZZ.GRF.AHTask;
using static Genesis.Library.BLL.ZZ.GRF.AHTask;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Frame.Code;
using System;
using System.Data.Entity;

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


			/// <summary>
			/// 分條出站- 物料批 報廢測試
			/// </summary>
			internal static string WIP_10_工單物料耗用報工
			{
				get
				{
					return FileApp.ts_Log(@"GRF\WIP_10_工單物料耗用報工.json");
				}
			}

			/// <summary>
			/// 工單耗用SAP回報 回拋SAP
			/// </summary>
			internal static string WIP_11_工單耗用SAP回報
			{
				get
				{
					return FileApp.ts_Log(@"GRF\WIP_11_工單耗用SAP回報.json");
				}
			}

			internal static string WIP_11_工單耗用SAP回報_sap
			{
				get
				{
					return FileApp.ts_Log(@"GRF\WIP_11_工單耗用SAP回報_sap2.json");
				}
			}

			internal static string WIP_10_Sap_GetOWOR
			{
				get
				{
					return FileApp.ts_Log(@"GRF\WIP_10_Sap_GetOWOR.json");
				}
			}

			internal static string 分條工單批次開立
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\GRF\分條工單批次開立.json");
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
		=> _DBTest((Txn) =>
		{
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
			WOServices.SlittingCheckIn_WOInfo("A_20021121002");
		}

		[TestMethod]
		public void t_LotFinlishTransfromMLot()
		{

			new LotFinlishTransfromMLot().Process(new WIPFormSendParameter(), true);
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
		/// 以一般出站模式 做測試
		/// </summary>
		[TestMethod]
		public void t_WIP_10_工單物料耗用報工()
		{
			var _r = FileApp.Read_SerializeJson<ZZ_WO_MLOT_CONSUME>(_log.WIP_10_工單物料耗用報工);
			AHTask.WoMtrConsumption(_r, true);
		}

		[TestMethod]
		public void t_WIP_10_工單物料耗用報工_1()
		=> _DBTest(Txn =>
		{
			var _r = FileApp.Read_SerializeJson<ZZ_WO_MLOT_CONSUME>(_log.WIP_10_工單物料耗用報工);
			var mlot = Txn.GetMLotInfo(_r.MTR_LOT);
			var addMtrLotQtyTxn = new MTRTransaction.AddMtrLotQtyTxn
				(mlot
				, -2
				, null, null);
			Txn.DoTransaction(addMtrLotQtyTxn);
			Txn.DoTransaction(new MTRTransaction.TerminateMtrLotTxn(mlot));

			//mlot = Txn.GetMLotInfo(_r.MTR_LOT);


		}, true);

		/// <summary>
		///  
		/// </summary>
		[TestMethod]
		public void t_查詢前階工單()
		{
			var r = WOServices.查詢前階工單("");
		}

		public struct d_WIP_11_工單耗用SAP回報
		{
			public List<ZZ_WO_MLOT_CONSUME> data { get; set; }
			public int DocEntry { get; set; }
			public string SAP { get; set; }
			public string NOTE { get; set; }
		}

		/// <summary>
		///  
		/// </summary>
		[TestMethod]
		public void WIP_11_工單耗用SAP回報()
		{
			var r = FileApp.Read_SerializeJson<d_WIP_11_工單耗用SAP回報>(_log.WIP_11_工單耗用SAP回報);
			AHTask.WoConsumptionToSap(r.data, r.DocEntry, r.SAP, r.NOTE, true);
			//var z = new d_SAP_PostOIGEList();
			//var x = r.GroupBy(c => new {c.PARTNO,c.ERP_WO,c.WAREHOUSE_NO })
			//	.ToDictionary(a => a.Key, b => b.ToList());
			//int idx = 0;
			//foreach (var el in x) {
			//	idx++;
			//	var BatchList = el.Value.Select(c => new BatchItem(c.MTR_LOT, (int)c.MTR_QTY)).ToList();
			//	var _idx = idx.ToString();
			//	var x1 = new Line() {
			//		ItemCode = el.Key.PARTNO,
			//		WhsCode = el.Key.WAREHOUSE_NO,
			//		//TODO:20230331
			//		//BaseEntry = el.Key.ERP_WO,
			//		Quantity = el.Value.Sum(c => {
			//			c.U_MES_LINE_ID = _idx;
			//			return (int)c.MTR_QTY;
			//		}),
			//		BatchList = BatchList,
			//		BaseLine = idx,
			//		//TODO:20230331
			//		//U_MES_LINE_ID = _idx
			//	};
			//	z.Lines.Add(x1);
			//}
			//FileApp.WriteSerializeJson(z, _log.WIP_11_工單耗用SAP回報_sap);
		}


		/// <summary>
		///  
		/// </summary>
		[TestMethod]
		public void WIP_11_工單耗用SAP回報1()
		{
			//	var r = FileApp.Read_SerializeJson<List<ZZ_WO_MLOT_CONSUME>>(_log.WIP_11_工單耗用SAP回報);
			//	var r1 = d_SAP_PostOIGEList.parse(r).parseData<d_SAP_PostOIGEList_z>();
			//	var r2 = AHTask.SendAPI(r1.ApiData);

			var r = FileApp.Read_SerializeJson<d_SAP_PostOIGEList>(_log.WIP_11_工單耗用SAP回報_sap);
			var r2 = AHTask.SendAPI_PostOIGE(r);


			//FileApp.WriteSerializeJson(z, _log.WIP_11_工單耗用SAP回報_sap);
		}

		[TestMethod]
		public void TestMethod1()
		{
			string url = "http://35.79.76.110:8086/api/SapApi/PostOIGEList";
			string json = "{ \r\n   \"Lines\": [ \r\n     { \r\n       \"BatchList\": [ \r\n         { \r\n           \"batchNumber\": \"string\", \r\n           \"Quantity\": 0 \r\n         } \r\n       ], \r\n       \"DocEntry\": \"string\", \r\n       \"ItemCode\": \"string\", \r\n       \"Dscription\": \"string\", \r\n       \"Quantity\": 0, \r\n       \"LineNum\": 0, \r\n       \"unitMsr\": \"string\", \r\n       \"WhsCode\": \"string\", \r\n       \"BaseType\": \"string\", \r\n       \"BaseEntry\": 0, \r\n       \"BaseLine\": 0, \r\n       \"BaseRef\": \"string\", \r\n       \"OcrCode\": \"string\", \r\n       \"OcrName\": \"string\", \r\n       \"U_MESLineID\": \"string\" \r\n     } \r\n   ] \r\n }";
			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			//HttpResponseMessage httpResponseMessage = client.PostAsync(url, content).Result;
			var response = client.PostAsync(url, content).Result;
			string r = response.Content.ReadAsStringAsync().Result;
			var result = r.ToObject<d_SAP_PostOIGEList_result>();

		}


		[TestMethod]
		public void TestMethod2()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			var response = client.GetAsync("http://35.79.76.110:8086/api/GetApi/Sap_GetOWOR?DocNum=960").Result;
			string content = response.Content.ReadAsStringAsync().Result;
			var x = content.ToObject<d_SAP_PostOIGEList_result>();
		}


		[TestMethod]
		public void TestMethod2_1()
		{
			var a = FileApp.Read_SerializeJson<d_SAP_PostOIGEList_result>(_log.WIP_10_Sap_GetOWOR);
			var x = a.Data.ToString().ToObject<List<d_Sap_GetOWOR>>();
		}

		/// <summary>
		///  
		/// </summary>
		[TestMethod]
		public void t_()
		=> _DBTest(Txn =>
		{
			//var Eqp = Txn.GetEquipmentInfo("GTI22120910222003769");
			////var r = WOServices.查詢前階工單("");
			//Txn.DoTransaction
			//	(new EQPTransaction.EquipmentChangeCapacityTxn(Eqp, 0));

			var r = Txn.EFQuery<WP_MTL_TRACE>().Reads().ToList();
		});

		[TestMethod]
		public void t_1()
		=> TxnBase.LzDBTrans("App", TxnACTION.n("MLOT_SCRAP"), Txn =>
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
		 }, isTest: true);

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


		[TestMethod]
		public void t_2()
		{
			List<string> inputList = new List<string>
				{
					"SPC_WO_110",
					"SPC_WO_109",
					"SPC_WO_108",
					"SPC_WO_107",
					"SPC_WO_107-",//應略過
					"SPC_WO_1A7-",//應略過
					"SPC_WO_1100",
					"SPC_WO_1100A",//應略過
				};

			// 使用 LINQ 將每個元素轉換為數字並取得最大值
			int maxValue = inputList
				.Select(item =>
				{
					string numberPart = item.Replace("SPC_WO_1", "");
					int number;
					if (numberPart.Length > 3) return (int?)null;
					if (int.TryParse(numberPart, out number))
					{
						return number;
					}
					else
					{
						return (int?)null;
					}
				})
				.Where(number => number.HasValue)
				.Max(number => number.Value);
		}

		[TestMethod]
		public void t_分條工單批次開立()
		{
			var _r = FileApp.Read_SerializeJson<WP_WO>(_log.分條工單批次開立);
			var _r1 = WOServices.分條工單批次開立(_r,true);
		}

		[TestMethod]
		public void t_分條工單批次開立_Rule_WO()
		=> _DBTest(Txn =>
		{
			var r = WOServices.分條工單批次開立_Rule_WO(Txn, "11111111", 25);

			//var _rep = new
			//{
			//	WP_WO = Txn.EFQuery<WP_WO>()
			//};
			//var _ATTRIBUTE_09 = "11111111";
			//var _list = (from a in _rep.WP_WO.Reads()
			//			 where a.ATTRIBUTE_09 == _ATTRIBUTE_09
			//			 select a.WO).AsNoTracking().ToList();

			//int maxValue = _list
			//	.Select(item =>
			//	{
			//		string numberPart = item.Replace($"{_ATTRIBUTE_09}-", "");
			//		int number;
			//		if (numberPart.Length > 3) return 0;
			//		if (int.TryParse(numberPart, out number))
			//		{
			//			return number;
			//		}
			//		else
			//		{
			//			return 0;
			//		}
			//	})
			//	//.Where(number => number.HasValue)
			//	.Max(number => number);


			//var result = new List<string>();
			//var count = 10;
			//var startValue = 95;
			//for (int i = 0; i < count; i++)
			//{
			//	startValue++; // 遞增起始值
			//	string formattedNumber = startValue.ToString("D2"); // 轉換為格式化後的文字（兩位數，不足補零）
			//	result.Add(formattedNumber);
			//}

		});
	}
}
