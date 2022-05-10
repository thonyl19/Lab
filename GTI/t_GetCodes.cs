//using Genesis.Gtimes.Common;
using BLL.MES;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.Transaction;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using UnitTestProject.TestUT;
//using Genesis.Gtimes.ADM.EncodeFormatUtility;

namespace UnitTestProject
{
	/// <summary>
	/// 編碼原則
	/// </summary>
	[TestClass]
	public class t_GetCodes
	{
		static class _log
		{
			internal static string t_GetRunCardHistoryByLot
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\t_GetRunCardHistoryByLot.json");
				}
			}
		}


		DBController _dbc;

		DBController DBC
		{
			get
			{
				if (_dbc == null)
				{
					ConnectionStringSettings SmartQueryConn = ConfigurationManager.ConnectionStrings["SRDSR.SqlServer.GTIMES"];
					this._dbc = new DBController(SmartQueryConn);
				}
				return this._dbc;
			}
		}

		[TestMethod]
		public void t_取得編碼原則資料()
		{

			using (var DBC = mes.dbc())
			{
				var EnInfo = new EncodeFormatUtility.EncodeFormatInfo
					(DBC, "LotNoRepack"
					, EncodeFormatUtility.IndexType.No);
			}
		}

		/// <summary>
		/// 編碼處理原型 
		/// </summary>
		[TestMethod]
		public void t_取得編碼原則_簡易版() {
			using (var DBC = mes.dbc())
			{
				var EnInfo = new EncodeFormatUtility.EncodeFormatInfo
					(DBC, "LotNoRepack"
					, EncodeFormatUtility.IndexType.No);
                var lotNo = "ZEGJ11-01";
                var Code = EncodeFormatUtility.Coder.GetCodes
					(DBC
					, "AdminTest"
					, EnInfo
					, 1 
					,new Dictionary<EncodeFormatUtility.ParameterType, object>() {
						{ EncodeFormatUtility.ParameterType.LOT, lotNo },
					}
					//是否自動 commit , T)則會直接把 code 寫入 AD_ENCODE_FORMAT_CONTROL
					, false);
			}
		}

		/// <summary>
		/// 這個測試,是需要先在 編碼原則維護 ,先建立一個  Test 的 編碼,
		/// 該原則主要 是 FIX:{4,'Test'} SERIAL:{3} , 產生的Code 會形成 Test001
		/// </summary>
		[TestMethod]
		public void t_取得編碼原則_Test()
		{
			using (var DBC = mes.dbc())
			{
				var EnInfo = new EncodeFormatUtility.EncodeFormatInfo
					(DBC, "G-MD-W003-010"
					, EncodeFormatUtility.IndexType.No);
				var Code = EncodeFormatUtility.Coder.GetCodes
					(DBC
					, "AdminTest"
					, EnInfo
					, 10
					, new Dictionary<EncodeFormatUtility.ParameterType, object>() {
						//{ EncodeFormatUtility.ParameterType.LOT, lotNo },
					}
					//是否自動 commit , T)則會直接把 code 寫入 AD_ENCODE_FORMAT_CONTROL
					, true);
			}
		}


		

		/// <summary>
		/// 編碼處理原型 
		/// </summary>
		[TestMethod]
		public void t_PackMerge_LotInfo_1()
		{
			using (var DBC = mes.dbc())
			{
				using (IDbTransaction tx = DBC.GetTransaction())
				{
					try
					{
						//使用 gti 交易程序
						var txnBase = new TransactionUtility.TransactionBase
							(DBC.GetSID()
							, "TEST"  // UserNo
							, DBC.GetDBTime()
							, "PackMerge" //程序名稱
							);
						var gtimesTxn = new TransactionUtility.GtimesTxn(DBC, txnBase);

						var lotNo = "SEGJ11-01";
						var EnInfo = new EncodeFormatUtility.EncodeFormatInfo(DBC, "LotNoRepack", EncodeFormatUtility.IndexType.No);

						//在這個個案中,需求是要取得 Lot 的母批 ,並依其 LotNo 重設最後一碼做為新 LotNo
						var Lot = new LotUtility.LotInfo(DBC, lotNo, LotUtility.IndexType.NO);
						var RootLot = new LotUtility.LotInfo(DBC, Lot.ROOT_LOT_SID, LotUtility.IndexType.SID);
						//這裡是截取出基本的保留的 Key(去最後一碼)
						var key = RootLot.LOT.Substring(0, RootLot.LOT.Length - 1);

						var codes = EncodeFormatUtility.Coder.GetCodes
							(DBC
							, "test"
							, EnInfo
							, 1 //只產生一筆資料
								//TODO:這一段 的功能作用不是很清楚,先比照辦理
							, new Dictionary<EncodeFormatUtility.ParameterType, object>
							{
								{ EncodeFormatUtility.ParameterType.LOT, key },
								//{ EncodeFormatUtility.ParameterType.WO, RootLot.WO },
								//{ EncodeFormatUtility.ParameterType.PARTNO, RootLot.PARTNO }
							}, false);
						tx.Commit();
					}
					catch (Exception Ex)
					{


					}

				}
			}
		}




		/*

		/// <summary>
		/// 預取/實取編碼,返回編碼信息物件
		/// </summary>
		/// <param name="dbc">DBController</param>
		/// <param name="userId">使用者id</param>
		/// <param name="encodeFormatInfo">格式編碼主檔Info物件</param>
		/// <param name="firstCode">起始編碼,如果有起始編碼,則不再控制流水號控制表</param>
		/// <param name="count">預取的編碼數量</param>
		/// <param name="args">獲取編碼需要的參數字典</param>
		/// <param name="doTransaction">獲取編碼同時是否更新已取號碼表</param>
		/// <returns>返回一個預取編碼信息物件</returns>
		public static CodesInfo GetCodes(DBController dbc, string userId, EncodeFormatInfo encodeFormatInfo, string firstCode, int count, Dictionary<ParameterType, object> args, bool doTransaction)
		{
			Dictionary<string, string> inputArgs = null;
			StringBuilder strFormatMask = new StringBuilder();
			StringBuilder strSearialInitValues = new StringBuilder();
			List<string> itemStrings = new List<string>();
			DateTime baseDate;
			DateTime updateDate;
			var info = new CodesInfo(dbc);

			if (args != null && args.ContainsKey(ParameterType.BASE_DATE))
			{
				baseDate = (DateTime)args[ParameterType.BASE_DATE];
				updateDate = dbc.GetDBTime();
			}
			else
			{
				baseDate = dbc.GetDBTime();
				updateDate = baseDate;
			}

			foreach (var v in args)
			{
				if (v.Key == ParameterType.INPUT_DICT_STR_STR)
				{
					inputArgs = (Dictionary<string, string>)v.Value;
				}
			}

			EncodeFormatItemFunction itemFunction = new EncodeFormatItemFunction(dbc);
			DataTable dtItems = itemFunction.GetByEncodeFormatSid(encodeFormatInfo.ENCODE_FORMAT_SID);
			if (dtItems == null || dtItems.Rows == null || dtItems.Rows.Count == 0)
			{
				for (int i = 1; i <= count; i++)
				{
					info.Codes.Add(string.Format("{0}-ERROR-{1}", encodeFormatInfo.ENCODE_FORMAT_NO, i));
				}
				return info;
			}
			Dictionary<int, EncodeFormatItemInfo> serialItemInfos = new Dictionary<int, EncodeFormatItemInfo>();

			bool hasWeekItem = false;
			foreach (DataRow dr in dtItems.Rows)
			{
				if (dr["ITEM_TYPE"].ToString() == ItemTypeEnum.WEEK.ToString())
				{
					hasWeekItem = true;
					break;
				}
			}

			//以下獲取重置條件部份
			for (int i = 0; i < dtItems.Rows.Count; i++)
			{
				EncodeFormatItemInfo item = new EncodeFormatItemInfo(dtItems.Rows[i]);
				ItemTypeEnum itemType = (ItemTypeEnum)(System.Enum.Parse(typeof(ItemTypeEnum), item.ITEM_TYPE, true));
				string tmpString = string.Empty;
				if (item.RESET_ON_CHANGED == "T" && itemType != ItemTypeEnum.SERIAL) // 序號類別的重置條件設定無效
				{
					switch (itemType)
					{
						case ItemTypeEnum.FIX:
							tmpString = item.ITEM_VALUE;
							break;
						case ItemTypeEnum.INPUT:
							if (inputArgs == null || !inputArgs.ContainsKey(item.ITEM_VALUE))
								break;
							tmpString = inputArgs[item.ITEM_VALUE];
							if (item.ITEM_VALUE_LENGTH > 0 && tmpString.Length != item.ITEM_VALUE_LENGTH)
							{
								tmpString = string.Empty;
								break;
							}
							break;
						case ItemTypeEnum.YEAR:
							if (hasWeekItem)
							{
								int tmpWeek = GetWeekOfYear(baseDate);
								if (tmpWeek >= 52 && baseDate.Month <= 1)
								{
									tmpString = FormatNumber(baseDate.Year - 1, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
								}
								else
								{
									tmpString = FormatNumber(baseDate.Year, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
								}
							}
							else
							{
								tmpString = FormatNumber(baseDate.Year, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							}
							break;
						case ItemTypeEnum.MONTH:
							if (item.ITEM_VALUE_LENGTH == 1 && item.SERIAL_CHARS == string.Empty)
								tmpString = string.Format("{0:X}", baseDate.Month);
							else if (item.ITEM_VALUE_LENGTH == 2 && item.SERIAL_CHARS == string.Empty)
								tmpString = string.Format("{0:D2}", baseDate.Month);
							else
								tmpString = FormatNumber(baseDate.Month, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.DAY:
							tmpString = FormatNumber(baseDate.Day, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.QUARTER:
							tmpString = FormatNumber((baseDate.Month - 1) / 3 + 1, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.WEEK:
							tmpString = FormatNumber(GetWeekOfYear(baseDate), item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.COLUMN:
							tmpString = GetColumnValue(dbc, item.TABLE_NAME, item.COLUMN_NAME, args);
							if (tmpString == string.Empty) break;

							int itemValueStart = item.COLUMN_VALUE_START != null ? item.COLUMN_VALUE_START.Value : 1;
							int itemValueLength = item.ITEM_VALUE_LENGTH;
							if (itemValueLength == 0) itemValueLength = tmpString.Length;

							if (itemValueStart == 0)
							{
								tmpString = string.Empty;
								break;
							}
							else if (itemValueStart < 0)
							{
								itemValueStart = itemValueLength + itemValueStart;
							}
							else if (itemValueStart > 0)
							{
								itemValueStart--;
							}

							if (itemValueStart < 0 || itemValueStart >= itemValueLength)
							{
								tmpString = string.Empty;
								break;
							}

							tmpString = tmpString.Substring(itemValueStart, itemValueLength);

							if (item.SERIAL_CHARS != string.Empty)
							{
								int num;
								if (int.TryParse(tmpString, out num))
								{
									tmpString = FormatNumber(num, itemValueLength, item.SERIAL_CHARS);
								}
							}
							break;
						case ItemTypeEnum.CUSTOM:
							List<IDbCommand> commands = new List<IDbCommand>();
							tmpString = GetCustomCode(dbc, encodeFormatInfo, item, args, ref commands);
							info.Commands.AddRange(commands);
							break;
						default:
							tmpString = string.Empty;
							break;
					}
					if (tmpString == string.Empty && item.ITEM_TYPE != ItemTypeEnum.SERIAL.ToString())
					{
						throw new Exception(string.Format("無法正確獲取格式編碼{0}的序號為{1}的格式項值.", encodeFormatInfo.ENCODE_FORMAT_NO, item.ITEM_SEQ));
					}
					strFormatMask.Append(tmpString);
					strFormatMask.Append('.');
					itemStrings.Add(tmpString);
				}
				else
				{
					switch (itemType)
					{
						case ItemTypeEnum.FIX:
							tmpString = item.ITEM_VALUE;
							break;
						case ItemTypeEnum.INPUT:
							if (inputArgs == null || !inputArgs.ContainsKey(item.ITEM_VALUE))
								break;
							tmpString = inputArgs[item.ITEM_VALUE];
							if (item.ITEM_VALUE_LENGTH > 0 && tmpString.Length != item.ITEM_VALUE_LENGTH)
							{
								tmpString = string.Empty;
								break;
							}
							break;
						case ItemTypeEnum.YEAR:
							if (hasWeekItem)
							{
								int tmpWeek = GetWeekOfYear(baseDate);
								if (tmpWeek >= 52 && baseDate.Month <= 1)
								{
									tmpString = FormatNumber(baseDate.Year - 1, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
								}
								else
								{
									tmpString = FormatNumber(baseDate.Year, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
								}
							}
							else
							{
								tmpString = FormatNumber(baseDate.Year, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							}
							break;
						case ItemTypeEnum.MONTH:
							if (item.ITEM_VALUE_LENGTH == 1 && item.SERIAL_CHARS == string.Empty)
								tmpString = string.Format("{0:X}", baseDate.Month);
							else if (item.ITEM_VALUE_LENGTH == 2 && item.SERIAL_CHARS == string.Empty)
								tmpString = string.Format("{0:D2}", baseDate.Month);
							else
								tmpString = FormatNumber(baseDate.Month, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.DAY:
							tmpString = FormatNumber(baseDate.Day, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.QUARTER:
							tmpString = FormatNumber((baseDate.Month - 1) / 3 + 1, item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.WEEK:
							tmpString = FormatNumber(GetWeekOfYear(baseDate), item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS);
							break;
						case ItemTypeEnum.COLUMN:
							tmpString = GetColumnValue(dbc, item.TABLE_NAME, item.COLUMN_NAME, args);
							if (tmpString == string.Empty) break;

							int itemValueStart = item.COLUMN_VALUE_START != null ? item.COLUMN_VALUE_START.Value : 1;
							int itemValueLength = item.ITEM_VALUE_LENGTH;
							if (itemValueLength == 0) itemValueLength = tmpString.Length;

							if (itemValueStart == 0)
							{
								tmpString = string.Empty;
								break;
							}
							else if (itemValueStart < 0)
							{
								itemValueStart = itemValueLength + itemValueStart;
							}
							else if (itemValueStart > 0)
							{
								itemValueStart--;
							}

							if (itemValueStart < 0 || itemValueStart >= itemValueLength)
							{
								tmpString = string.Empty;
								break;
							}

							tmpString = tmpString.Substring(itemValueStart, itemValueLength);

							if (item.SERIAL_CHARS != string.Empty)
							{
								int num;
								if (int.TryParse(tmpString, out num))
								{
									tmpString = FormatNumber(num, itemValueLength, item.SERIAL_CHARS);
								}
							}
							break;
						case ItemTypeEnum.SERIAL:
							tmpString = string.Empty;
							serialItemInfos.Add(i, item);
							if (item.SERIAL_START_VALUE != string.Empty)
							{
								if (item.SERIAL_CHARS == string.Empty)
									strSearialInitValues.AppendFormat(",{0}", item.SERIAL_START_VALUE.PadLeft(item.ITEM_VALUE_LENGTH, '0'));
								else
									strSearialInitValues.AppendFormat(",{0}", item.SERIAL_START_VALUE.PadLeft(item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS[0]));
							}
							else if (item.SERIAL_CHARS == string.Empty)
								strSearialInitValues.AppendFormat(",{0}", "1".PadLeft(item.ITEM_VALUE_LENGTH, '0'));
							else
								strSearialInitValues.AppendFormat(",{0}", item.SERIAL_CHARS[0].ToString().PadLeft(item.ITEM_VALUE_LENGTH, item.SERIAL_CHARS[0]));
							break;
						case ItemTypeEnum.CUSTOM:
							List<IDbCommand> commands = new List<IDbCommand>();
							tmpString = GetCustomCode(dbc, encodeFormatInfo, item, args, ref commands);
							info.Commands.AddRange(commands);
							break;
						default:
							tmpString = string.Empty;
							break;
					}
					if (tmpString == string.Empty && item.ITEM_TYPE != ItemTypeEnum.SERIAL.ToString())
					{
						throw new Exception(string.Format("無法正確獲取格式編碼{0}的序號為{1}的格式項值.", encodeFormatInfo.ENCODE_FORMAT_NO, item.ITEM_SEQ));
					}
					strFormatMask.Append('.');
					itemStrings.Add(tmpString);
				}
			}

			// 提供起始編碼,則不再考慮編碼控制表
			if (!string.IsNullOrEmpty(firstCode))
			{
				strSearialInitValues.Remove(0, strSearialInitValues.Length);
				strSearialInitValues.Append(GetSerialInitialValues(dtItems, itemStrings, firstCode));
			}

			if (strSearialInitValues.Length > 0)
			{
				EncodeFormatControlFunction controlFunction = new EncodeFormatControlFunction(dbc);
				controlFunction.SetCanReturnEmptyDataTable(true);
				EncodeFormatControlInfo controlInfo;
				DataTable dtControl = controlFunction.GetQuery("ENCODE_FORMAT_SID", encodeFormatInfo.ENCODE_FORMAT_SID, "RESET_FORMAT_MASK", strFormatMask.ToString());

				// 提供起始編碼,則不再考慮編碼控制表
				if (!string.IsNullOrEmpty(firstCode))
				{
					dtControl.Rows[0]["SERIAL_LAST_VALUE"] = strSearialInitValues.ToString().Substring(1);
					controlInfo = new EncodeFormatControlInfo(dtControl.Rows[0]);
				}
				else if ((dtControl.Rows == null || dtControl.Rows.Count == 0))
				{
					DataRow dr = dtControl.NewRow();
					controlInfo = new EncodeFormatControlInfo(dr);

					dr["ENCODE_FORMAT_CONTROL_SID"] = dbc.GetSID();
					dr["ENCODE_FORMAT_SID"] = encodeFormatInfo.ENCODE_FORMAT_SID;
					dr["ENCODE_FORMAT_NO"] = encodeFormatInfo.ENCODE_FORMAT_NO;
					dr["ENCODE_FORMAT_NAME"] = encodeFormatInfo.ENCODE_FORMAT_NAME;
					dr["ENCODE_FORMAT_TYPE"] = encodeFormatInfo.ENCODE_FORMAT_TYPE;
					dr["RESET_FORMAT_MASK"] = strFormatMask.ToString();
					dr["SERIAL_LAST_VALUE"] = strSearialInitValues.ToString().Substring(1);
					dr["CREATE_USER"] = userId;
					dr["CREATE_DATE"] = updateDate;
					dr["UPDATE_USER"] = userId;
					dr["UPDATE_DATE"] = updateDate;
				}
				else
				{
					controlInfo = new EncodeFormatControlInfo(dtControl.Rows[0]);
				}

				string[] serialInitValues = controlInfo.SERIAL_LAST_VALUE.Split(',');
				if (serialInitValues.Length != serialItemInfos.Count)
					throw new Exception("編碼格式在使用後被修改了定義!");

				for (int cnt = 1; cnt <= count; cnt++)
				{
					if (serialItemInfos.Count > 0)
					{
						bool overflow = (dtControl.Rows.Count > 0 && cnt == 1) || cnt > 1;
						int startIndex, endIndex, step, itemIndex;
						if (encodeFormatInfo.MULTI_SERIAL_DIRECTION != "1")
						{
							startIndex = 0;
							endIndex = serialItemInfos.Count;
							step = 1;
						}
						else
						{
							startIndex = serialItemInfos.Count - 1;
							endIndex = -1;
							step = -1;
						}
						for (int i = startIndex; i != endIndex; i += step)
						{
							itemIndex = serialItemInfos.ToArray()[i].Key;

							if (overflow)
							{
								itemStrings[itemIndex] = IncreaseSerial(serialInitValues[i], serialItemInfos[itemIndex].SERIAL_CHARS, out overflow);
								serialInitValues[i] = itemStrings[itemIndex];
							}
							else
								itemStrings[itemIndex] = serialInitValues[i];
						}
					}

					info.Codes.Add(string.Join(string.Empty, itemStrings.ToArray()));
				}

				if (!string.IsNullOrEmpty(firstCode))
				{
					// 提供起始編碼,則不再考慮編碼控制表
				}
				if ((dtControl.Rows == null || dtControl.Rows.Count == 0))
				{
					InsertCommandBuilder insert = new InsertCommandBuilder(dbc, "AD_ENCODE_FORMAT_CONTROL");
					insert.InsertColumn("ENCODE_FORMAT_CONTROL_SID", controlInfo.ENCODE_FORMAT_CONTROL_SID);
					insert.InsertColumn("ENCODE_FORMAT_SID", controlInfo.ENCODE_FORMAT_SID);
					insert.InsertColumn("ENCODE_FORMAT_NO", controlInfo.ENCODE_FORMAT_NO);
					insert.InsertColumn("ENCODE_FORMAT_NAME", controlInfo.ENCODE_FORMAT_NAME);
					insert.InsertColumn("ENCODE_FORMAT_TYPE", controlInfo.ENCODE_FORMAT_TYPE);
					insert.InsertColumn("RESET_FORMAT_MASK", controlInfo.RESET_FORMAT_MASK);
					insert.InsertColumn("SERIAL_LAST_VALUE", string.Join(",", serialInitValues));
					insert.InsertColumn("CREATE_USER", controlInfo.CREATE_USER);
					insert.InsertColumn("CREATE_DATE", controlInfo.CREATE_DATE);
					insert.InsertColumn("UPDATE_USER", controlInfo.UPDATE_USER);
					insert.InsertColumn("UPDATE_DATE", controlInfo.UPDATE_DATE);

					if (doTransaction)
						dbc.DoTransaction(insert.GetCommands());
					else
						info.Commands.AddRange(insert.GetCommands());
				}
				else if (serialInitValues.Length > 0)
				{
					UpdateCommandBuilder update = new UpdateCommandBuilder(dbc, "AD_ENCODE_FORMAT_CONTROL");
					update.UpdateColumn("SERIAL_LAST_VALUE", string.Join(",", serialInitValues));
					update.UpdateColumn("UPDATE_USER", userId);
					update.UpdateColumn("UPDATE_DATE", updateDate);
					update.WhereAnd("ENCODE_FORMAT_CONTROL_SID", controlInfo.ENCODE_FORMAT_CONTROL_SID);
					update.WhereAnd("UPDATE_DATE", controlInfo.UPDATE_DATE);

					if (doTransaction)
						dbc.DoTransaction(update.GetCommands());
					else
						info.Commands.AddRange(update.GetCommands());
				}
			}
			else
			{
				for (int i = 1; i <= count; i++)
					info.Codes.Add(string.Join(string.Empty, itemStrings.ToArray()));
			}

			return info;
		}
		*/
	}
}
