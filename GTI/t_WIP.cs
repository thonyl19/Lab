using AutoMapper;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject
{
	[TestClass]
	public class t_WIP : _testBase
	{
		public string _path = @"C:\Code\GTIMES_2015\UnitTestProject\Log\";

		static class _log
		{

            internal static string t_PackMerge_LotInfo
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_PackMerge_LotInfo.json");
				}
			}

			internal static string t_Process_CollectLotAttr_cmd
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_Process_CollectLotAttr_cmd.json");
				}
			}


			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_splitBIN
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_splitBIN.json");
				}
			}

			/// <summary>
			/// GetLotInfoCheck 程序最終回傳值,但有個問題是,
			/// 反序列化後,CurrentLot , PartInfo 等 ,原本舊 MES 的資料物件, 都無法正常的取到值.
			/// </summary>
			public static string t_GetLotInfoCheck { get { return FileApp.ts_Log(@"WIP\t_GetLotInfoCheck.json"); } }

			/// <summary>
			/// 依據 LOT_SID 取得 ZZ_LOT_BIN 的範例 資料
			/// </summary>
			public static string t_ZZ_LOT_BIN { get { return FileApp.ts_Log(@"WIP\t_ZZ_LOT_BIN.json"); } }

			/// <summary>
			/// 測試 取得 BIN for 包裝站
			/// </summary>
			public static string t_ZZ_BIN_Package { get { return FileApp.ts_Log(@"WIP\t_ZZ_BIN_Package.json"); } }

			public static string t_ZZ_ReWork { get { return FileApp.ts_Log(@"WIP\t_ZZ_ReWork.json"); } }

			public static string t_Release { get { return FileApp.ts_Log(@"WIP\t_Release.json"); } }
			public static string t_GetOperLotAttributeData_OperSid { get { return FileApp.ts_Log(@"WIP\t_GetOperLotAttributeData_OperSid.json"); } }
			public static string t_WP_LOT { get { return FileApp.ts_Log(@"WIP\t_WP_LOT.json"); } }
			public static string t_CollectLotAttr_parse { get { return FileApp.ts_Log(@"WIP\t_CollectLotAttr_parse.json"); } }

			
		}

		[TestMethod]
		public void t_Commands_InsBIN_實測寫入()
		{
			string LOT_SID = "LOT_SID"
			, LOT = "LOT"
			, ROUTE_VER_OPER_SID = "ROUTE_VER_OPER_SID"
			, OPERATION = "OPERATION";
			var z = FileApp.Read_SerializeJson<List<ZZ_BIN>>
				(_log.t_splitBIN);
			try
			{
				string linkSID = DBC.GetSID();
				DateTime txnTime = DBC.GetDBTime();
				TransactionUtility.TransactionBase txnBase = new TransactionUtility.TransactionBase
					(linkSID, "test", txnTime, "FunctionRightName");
				TransactionUtility.GtimesTxn gtimesTxn = new TransactionUtility.GtimesTxn(DBC, txnBase);
				TransactionUtility.AddSQLCommandTxn sqlcmd = new TransactionUtility.AddSQLCommandTxn();

				IDbTransaction tx = DBC.GetTransaction();
				var inser_commands = new BLL.MES.WIPServices().ZZ_Commands_InsBIN
						(z
						, DBC
						, "Test"
						, "FunctionRightName"
						, "linkSID"
						, DateTime.Now
						, 20
						, LOT_SID
						, LOT
						, ROUTE_VER_OPER_SID
						, OPERATION);
				sqlcmd.Commands.AddRange(inser_commands);
				gtimesTxn.Add(sqlcmd);
				gtimesTxn.DoTransaction(gtimesTxn.GetTransactionCommands(), tx);
				sqlcmd.Commands.Clear();
				gtimesTxn.Clear();
				tx.Commit();
			}
			catch (Exception Ex)
			{

			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// 這個測試有個問題,就是 因為 LotInfo 特殊性的關係,
		///     使得它無法使用反序列化來做固定模擬,
		/// 所以 LotInfo 只能使用確實存在的資料來做測試模擬 
		[TestMethod]
		public void t_Commands_InsBIN()
		{
			string LOT_SID = "LOT_SID"
			, LOT = "LOT"
			, ROUTE_VER_OPER_SID = "ROUTE_VER_OPER_SID"
			, OPERATION = "OPERATION";

			var z = FileApp.Read_SerializeJson<List<ZZ_BIN>>
				(_log.t_splitBIN);

			try
			{
				t_Commands_InsBIN_fn(LOT_SID, LOT, ROUTE_VER_OPER_SID, OPERATION, z);
			}
			catch (Exception Ex)
			{
				Assert.AreEqual
					("分BIN 合計數超過限制(批號數量-報廢量)."
					, Ex.Message
					, "應該檢核出 合計數超過限制");
			}
			try
			{
				z[0].BIN = "";
				t_Commands_InsBIN_fn(LOT_SID, LOT, ROUTE_VER_OPER_SID, OPERATION, z);
			}
			catch (Exception Ex)
			{
				Assert.AreEqual
					("BIN 和 阻值範圍 不得為空值."
					, Ex.Message
					, "應該檢核出 不得為空值");
			}

			try
			{
				z[0].BIN = "1111";
				z[0].BIN_QTY = null;
				t_Commands_InsBIN_fn(LOT_SID, LOT, ROUTE_VER_OPER_SID, OPERATION, z);
			}
			catch (Exception Ex)
			{
				Assert.AreEqual
					("BIN 值不得為空值."
					, Ex.Message
					, "應該檢核出 不得為空值");
			}

			try
			{
				z[0].BIN = "1111";
				z[0].BIN_QTY = -1;
				t_Commands_InsBIN_fn(LOT_SID, LOT, ROUTE_VER_OPER_SID, OPERATION, z);
			}
			catch (Exception Ex)
			{
				Assert.AreEqual
					("BIN 值不得為負值."
					, Ex.Message
					, "應該檢核出 BIN 值不得為負值.");
			}
		}

		public List<IDbCommand> t_Commands_InsBIN_fn(string LOT_SID, string LOT, string ROUTE_VER_OPER_SID, string OPERATION, List<ZZ_BIN> z)
		{
			var _r = new BLL.MES.WIPServices().ZZ_Commands_InsBIN
				(z
				, DBC
				, "Test"
				, "FunctionRightName"
				, "linkSID"
				, DateTime.Now
				, 10
				, LOT_SID
				, LOT
				, ROUTE_VER_OPER_SID
				, OPERATION);
			return _r;
		}

		[TestMethod]
		public void t_splitBIN()
		{

			var z = FileApp.Read_SerializeJson<List<ZZ_BIN>>
				(FileApp.ts_Log(@"WIP\t_splitBIN.json"));
		}








		[TestMethod]
		public void t_ZZ_FilterEDC_ByEQP()
		{
			var t_GetLotInfoCheck = _log.t_GetLotInfoCheck;
			//var _r = new WIPServices().GetLotInfoCheck("New-20190411-20", "StationCheckout");
			//new FileApp().Write_SerializeJson(_r, _file);

			var _LotData = new FileApp().Read_SerializeJson<LotData>(t_GetLotInfoCheck);
			var _all = _LotData.EdcList.Count;
			var _list_1 = new WIPServices().ZZ_FilterEDC_ByEQP(_LotData, "");
			Assert.AreEqual(_all, _list_1.Count, "批號拆設備測試->EDC 應全數列出");

			//_LotData.Setting.CheckOutSet.IsUseLotSplitToEqp = false;
			//var _list_2 = new WIPServices().ZZ_FilterEDC_ByEQP(_LotData, "");
			//Assert.AreEqual(2, _list_2.Count, "單批號測試,EQP 未設定->EDC 應只列出 ALL");

			var _list_3 = new WIPServices().ZZ_FilterEDC_ByEQP(_LotData, "V1");
			Assert.AreEqual(4, _list_3.Count, "單批號測試,EQP 未設定->EDC 應全數列出 ALL和 A1");


			//new FileApp().Write_SerializeJson(_LotData.EdcList, FileApp.ts_Log(@"WIP\t_ZZ_FilterEDC_ByEQP.json"));


		}

		public List<Dictionary<string, string>> parse_x(string _s, decimal DATA_COUNT)
		{
			List<Dictionary<string, string>> _list = new List<Dictionary<string, string>>();
			if (string.IsNullOrWhiteSpace(_s))
			{
				for (int i = 0; i < DATA_COUNT; i++)
				{
					_list.Add(new Dictionary<string, string>() { { (i + 1).ToString(), "" } });
				}
			}
			else
			{
				_list = _s.Split(',')
					.Select(x =>
					{
						return new Dictionary<string, string>() { { x, "" } };
					}).ToList();
			}
			return _list;
		}

		[TestMethod]
		public void t_GetEDC_Info()
		{
			var _lotInfo = new LotUtility.LotInfo(DBC, "SAFXJJ101", LotUtility.IndexType.NO);
			EDCUtility.EDCFunctions edcFun = new EDCUtility.EDCFunctions(DBC);
			var edcData = edcFun.GetEdcOperParaData(_lotInfo.ECN_SID, _lotInfo.WO, _lotInfo.ROUTE_VER_SID, _lotInfo.ROUTE_VER_OPER_SID, _lotInfo.PARTNO,
				_lotInfo.OPERATION);



			if (edcData != null && edcData.Count != 0)
			{
				var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DataRow, BLL.DataViews.Edc.EdcModel>()
						.ForMember(d => d.EdcVerSid, o => o.MapFrom(s => s["EDC_VER_SID"]))
						.ForMember(d => d.EdcParameterSid, o => o.MapFrom(s => s["EDC_PARA_SID"]))
						.ForMember(d => d.QCItemSID, o => o.MapFrom(s => s["QC_ITEM_SID"]))
						.ForMember(d => d.DataType, o => o.MapFrom(s => s["DATATYPE"]))
						.ForMember(d => d.DataCount, o => o.MapFrom(s => s["DATA_COUNT"]))
						.ForMember(d => d.DispayPointName, o => o.MapFrom(s => s["DISPLAY_POINT_NAME"]))
						.ForMember(d => d.DispayPointNameList, o => o.MapFrom(s => s["DISPLAY_POINT_NAME"].ToString().Split(',').ToList()))
						.ForMember(d => d.InputValueList, o => o.MapFrom(s => parse_x(s["DISPLAY_POINT_NAME"].ToString(), (decimal)s["DATA_COUNT"])))
						.ForMember(d => d.ItemSid, o => o.MapFrom(s => s["QC_ITEM_SID"]))
						.ForMember(d => d.ItemNo, o => o.MapFrom(s => s["ITEM_NO"]))
						.ForMember(d => d.ItemName, o => o.MapFrom(s => s["ITEM"]))
						.ForMember(d => d.mustInput, o => o.MapFrom(s => s["MUST_INPUT"]))
						.ForMember(d => d.UCL, o => o.MapFrom(s => s["UCL"]))
						.ForMember(d => d.USL, o => o.MapFrom(s => s["USL"]))
						.ForMember(d => d.LCL, o => o.MapFrom(s => s["LCL"]))
						.ForMember(d => d.LSL, o => o.MapFrom(s => s["LSL"]))
						.ForMember(d => d.TL, o => o.MapFrom(s => s["TL"]))
						.ForMember(d => d.EDCSid, o => o.MapFrom(s => s["EDC_SID"]))
						.ForMember(d => d.EDCThrowSpc, o => o.MapFrom(s => s["EDC_THROW_SPC"]))
						.ForMember(d => d.EDCVerParaSid, o => o.MapFrom(s => s["EDC_VER_PARA_SID"]))
						).CreateMapper();

				var z = mapper.Map<List<BLL.DataViews.Edc.EdcModel>>(edcData.ToTable().Rows);
				//new FileApp().Write_SerializeJson(z, FileApp.ts_Log(@"WIP\t_GetEDC_Info_EdcModel.json"));

				new FileApp().Write_SerializeJson(z, FileApp.ts_Log(@"WIP\t_GetEDC_Info_EdcModel.json"));

			}

		}



		[TestMethod]
		public void t_ZZ_BIN_Package()
		{
			//輸入要測試的 LOT_SID
			var LOT_SID = "GTI19041123581800978";
			var _r = new WIPServices().ZZ_BIN_Package(true, LOT_SID);
			new FileApp().Write_SerializeJson(_r, _log.t_ZZ_BIN_Package);
		}

		[TestMethod]
		public void t_ZZ_Commands_PackageUpdate()
		{
			var txnTime = this.DBC.GetDBTime();
			var linkSID = this.DBC.GetSID();
			var _l = new FileApp().Read_SerializeJson<List<ZZ_BIN>>(_log.t_ZZ_BIN_Package);
			var _r = new WIPServices().ZZ_Commands_PackageUpdate
				(_l,
				this.DBC,
				"test",
				linkSID,
				txnTime
				);

			/*實際測試寫入
            
            TransactionUtility.TransactionBase txnBase = new TransactionUtility.TransactionBase(linkSID, "test", txnTime, "FunctionRightName");
            TransactionUtility.GtimesTxn gtimesTxn = new TransactionUtility.GtimesTxn(this.DBC, txnBase);
            TransactionUtility.AddSQLCommandTxn sqlcmd = new TransactionUtility.AddSQLCommandTxn();

            using (IDbTransaction tx = DBC.GetTransaction())
            {

                sqlcmd.Commands.AddRange(_r);
                gtimesTxn.Add(sqlcmd);
                gtimesTxn.DoTransaction(gtimesTxn.GetTransactionCommands(), tx);
                sqlcmd.Commands.Clear();
                gtimesTxn.Clear();
                tx.Commit();
            }
            */
		}



		/// <summary>
		/// 主要是 前端 post 回來的資料測試程序執行結果,主要應測出
		/// 1.如果 ReWorkOperList 為空時,應略過往後執行
		/// 2.如果 ReWorkOperList 有資料,則將 TxnDoItemInfo.LotInfo  的資料做寫檔並重取
		///		並確認 TxnDoItemInfo.LotInfo.ATTRIBUTE_10 的值有正確更新 
		///	目前測試程序主要只針對 第2點實作
		/// </summary>
		[TestMethod]
		public void t_ZZ_ReWork()
		{

			var _r = new FileApp().Read_SerializeJson<WIPFormSendParameter>(_log.t_ZZ_ReWork);
			using (var DBC = mes.dbc())
			{
				try
				{
					var _lot = "New-20190411-03";
					var _lotInfo = new LotUtility.LotInfo(DBC, _lot, LotUtility.IndexType.NO);
					var _oldInfo = new LotUtility.LotInfo(DBC, _lot, LotUtility.IndexType.NO);
					var UserNo = "test";
					string linkSID = DBC.GetSID();
					DateTime txnTime = DBC.GetDBTime();
					TransactionUtility.TransactionBase txnBase = new TransactionUtility.TransactionBase(linkSID, UserNo, txnTime, _lotInfo.FUN_CODE);
					TransactionUtility.GtimesTxn gtimesTxn = new TransactionUtility.GtimesTxn(DBC, txnBase);
					TransactionUtility.AddSQLCommandTxn sqlcmd = new TransactionUtility.AddSQLCommandTxn();
					using (IDbTransaction tx = DBC.GetTransaction())
					{
						var txnInfo = new TxnDoItemInfo()
						{

							DBC = DBC,
							//Data = data,
							GtimesTxn = gtimesTxn,
							LinkSID = linkSID,
							ExeTime = txnTime,
							LotInfo = _lotInfo,
							OldLotinfo = _oldInfo,
							Txn = tx,
							UserNo = UserNo,
							ApplicationName = _lotInfo.FUN_CODE
						};

						var old_attr10 = txnInfo.LotInfo.ATTRIBUTE_10;
						WIPServices.ZZ_Process_ReWork(_r.ReWorkOperList, txnInfo);
						var new_attr10 = txnInfo.LotInfo.ATTRIBUTE_10;
						tx.Rollback();
						Assert.AreNotEqual(old_attr10, new_attr10, "兩者的資料不應相符！");
					}
				}
				catch (Exception Ex)
				{

					throw Ex;
				}
			}
		}

		/// <summary>
		/// 產生 批號完工 所需要的測試
		/// </summary>
		/// 這個測試有個問題,就是 因為 LotInfo 特殊性的關係,
		///     使得它無法使用反序列化來做固定模擬,
		/// 所以 LotInfo 只能使用確實存在的資料來做測試模擬 
		[TestMethod]
		public void _批號完工_產生測試資料()
		{
			/*
			https://genesisoffice365-my.sharepoint.com/personal/anthony_lin_genesis_com_tw/_layouts/15/Doc.aspx?sourcedoc={45739d42-e95b-4e08-b009-54d89cdc2ef0}&action=edit&wd=target%280_%E5%B0%88%E6%A1%88%2Fworklog%2F%E6%89%B9%E8%99%9F%E5%AE%8C%E5%B7%A5.one%7C80651539-8171-4438-b247-4920be0e12ab%2F%E6%B8%AC%E8%A9%A6%E7%A8%8B%E5%BA%8F%7C32841c19-8ca6-4a09-8810-4f2e3e180572%2F%29
			*/
		}
		[TestMethod]
		public void t_PackMerge_LotInfo()
		{
			var _r = WIPServices.PackMerge_LotInfo("T0572.01");
			FileApp.WriteSerializeJson(_r, _log.t_PackMerge_LotInfo);
		}

		/// <summary>
		/// 測試原則,隨便找一筆 WP_LOT.ROOT_LOT_SID 不為空值的來測即可 ,
		/// 測試結果,取得的 單號最後一碼,應為 Z ,重覆執行第二次則應為 Y
		/// </summary>
		[TestMethod]
		public void t_PackMerge_NewLotNo()
		{
			using (var DBC = mes.dbc())
			{
				using (IDbTransaction tx = DBC.GetTransaction())
				{

					var txnBase = new TransactionUtility.TransactionBase
						(DBC.GetSID()
						, "TEST"
						, DBC.GetDBTime()
						, "PackMerge");
					var gtimesTxn = new TransactionUtility.GtimesTxn(DBC, txnBase);

					var lotNo = "SEGJ11-01";
					var EnInfo = new EncodeFormatUtility.EncodeFormatInfo(DBC, "LotNoRepack", EncodeFormatUtility.IndexType.No);
					var Lot = new LotUtility.LotInfo(DBC, lotNo, LotUtility.IndexType.NO);
					var codes = WIPServices.PackMerge_NewLotNo
						(DBC
						, "test"
						, EnInfo
						, Lot
						);
					var CommandTxn = new TransactionUtility.AddSQLCommandTxn();

					CommandTxn.Commands.AddRange(codes.Commands);
					gtimesTxn.Add(CommandTxn);
					gtimesTxn.DoTransaction(gtimesTxn.GetTransactionCommands(), tx);
					tx.Commit();
				}
			}
		}


		[TestMethod]
		public void t_CollectLotAttr_parse()
		{
			var dt_WP_LOT = new FileApp().Read_SerializeJson<DataTable>( _log.t_WP_LOT);
			var dv_LotAttr = new FileApp().Read_SerializeJson<DataTable>( _log.t_GetOperLotAttributeData_OperSid);
			var _r = WIPOperConfigServices.CollectLotAttr_parse(dv_LotAttr, dt_WP_LOT);
			var _src = _r[0];

			Assert.AreEqual<string>( "F" , _r[0].MUST_INPUT, "MUST_INPUT 原始值為 null ,應自動轉為 F");
			Assert.AreEqual<string>("MIRLE_Zip_STK1", _r[0].Value,  "ATTRIBUTE_08  應為 MIRLE_Zip_STK1");
			Assert.AreEqual<string>("el-input", _r[0].VIEW_TYPE,  "VIEW_TYPE  應為 el-input");
			Assert.AreEqual<string>("批號彈性欄位08", _r[0].FACE);

			Assert.AreEqual<string>(null, _r[1].FACE);
			Assert.AreEqual<string>("包裝,SPR-P175-Y", _r[1].Value);
			Assert.AreEqual<string>("el-input", _r[1].VIEW_TYPE);

			Assert.AreEqual<string>("----", _r[2].FACE,  "測試 FACE_ID 找不到對應值 , 回傳應為 '----'");
			Assert.AreEqual<string>("gt-input-bool", _r[2].VIEW_TYPE);

			_src = _r[3];
			Assert.AreEqual<string>("2022/2/16 下午 02:51:49", _src.Value);
			Assert.AreEqual<string>("gt-el-date-picker", _src.VIEW_TYPE);

			_src = _r[4];
			Assert.AreEqual<string>("1", _src.Value);
			Assert.AreEqual<string>("gt-input-number", _src.VIEW_TYPE);

			_src = _r[5];
			Assert.AreEqual<string>("1.1234", _src.Value);
			Assert.AreEqual<string>("gt-input-number", _src.VIEW_TYPE);

			new FileApp().Write_SerializeJson(_r, _log.t_CollectLotAttr_parse,isMult:false);

		}



		[TestMethod]
		public void t_Process_CollectLotAttr_cmd()
		{
			var dt_OperLotAttribute = new FileApp().Read_SerializeJson<DataTable>(_log.t_GetOperLotAttributeData_OperSid);
			var collectLotAttr = new FileApp().Read_SerializeJson<List<CollectLotAttr>>(_log.t_CollectLotAttr_parse);
            var _r = WIPServices.Process_CollectLotAttr_cmd
				( this.DBC
                , dt_OperLotAttribute
                , collectLotAttr
                , "SUX979.01");

            new FileApp().Write_SerializeJson(_r, _log.t_Process_CollectLotAttr_cmd, isMult: false);

        }

		[TestMethod]
		public void t_LoadCheckToolByEquipment()
		{
			//ToolUtility.ToolFunction func = new ToolUtility.ToolFunction(dbc);
			//DataView dvOperTool = func.GetPartNoOperToolData_OperSid(lotInfo.WO, lotInfo.ROUTE_VER_SID, lotInfo.ROUTE_VER_OPER_SID, lotInfo.PARTNO, lotInfo.OPER_SID);

			var x = new WIPServices()
				//.LoadCheckToolByEquipment("CUT-003", "WO-T010-01");
				.LoadCheckToolByEquipment("E-C02-001-01", "LWO23010901-07");

		}
	}
}
