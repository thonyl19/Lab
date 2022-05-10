using BLL.Base;
using BLL.MES;
using Dal.Repository;
using Frame.Code;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using UnitTestProject.TestUT;
using mdl = MDL.MES;
using _v8n = BLL.MES.FluentValidation;

namespace UnitTestProject
{
	[TestClass]
	public class t_IPQC : _testBase
	{


		public static class _log
		{
			internal static string t_QCResult
			{
				get
				{
					return FileApp.ts_Log(@"IPQC\t_QCResult.json");
				}
			}

			/// <summary>
			///  
			/// </summary>
			internal static string t_Updata_QC_INSP
			{
				get
				{
					return FileApp.ts_Log(@"IPQC\t_Updata_QC_INSP.json");
				}
			}


			internal static string t_GetEDC_Data
			{
				get
				{
					return FileApp.ts_Log(@"IPQC\t_GetEDC_Data.json");
				}
			}

			internal static string t_無抽樣巡檢_server取值樣本
			{
				get
				{
					return FileApp.ts_Log(@"IPQC\t_無抽樣巡檢_server取值樣本.json");
				}
			}

			internal static string t_Search_QCResult_Query
			{
				get
				{
					return FileApp.ts_Log(@"IPQC\t_Search_QCResult_Query.json");
				}
			}

			internal static string t_Search_QCResult_Query_result
			{
				get
				{
					return FileApp.ts_Log(@"IPQC\t_Search_QCResult_Query_result.json");
				}
			}


		}



		[TestMethod]
		public void t_IPQC_1()
		{
			using (var dbContext = new MDL.MESContext())
			{
				var _refServices = new EFRepository<mdl.QC_INSP>(dbContext);

				var item1 = _refServices.ReadsList("SELECT * from QC_INSP");
				new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"IPQC\t_IPQC.json"));
			}
		}



		[TestMethod]
		public void t_v8n_QC_INSP()
		{
			var _QC_INSP = new QC_INSP()
			{
				INSP_NO = "INSP_NO",
				INSP_NAME = "INSP_NAME"
			};

			var result = _v8n.QC_INSP.Check(ServicesBase.GetLoginUser(true), _QC_INSP);

		}


		[TestMethod]
		public void t_Updata_QC_INSP_NewEdcMode()
		{
			var x = new
			{
				formData = new QC_INSP(),
				edcData = new List<QC_INSP_EDC>(),
				isNewEdc = true
			};

			using (var dbContext = new MDL.MESContext())
			{

				var _QC_INSP_EDC = new EFRepository<mdl.QC_INSP_EDC>(dbContext);
				var _r = new FileApp().Read_SerializeJson(_log.t_Updata_QC_INSP, x);
				var _OldData = QMSService.QC_INSP_EDC(_r.formData.INSP_NO, MDL.SearchKey.No);

				QMSService.Updata_QC_INSP_NewEdcMode
					(_r.formData
					, _QC_INSP_EDC
					, _OldData
					, _r.edcData
					);
				dbContext.SaveChanges();
			}
		}




		/// <summary>
		/// 取得 工程參數 EDC群組
		/// </summary>
		/// \GTIMES_2015\GTiMES\ADM\EquipmentData.aspx.EDC.cs - txtEDC_TextChanged
		[TestMethod]
		public void t_GetOperToolSourceList()
		{
			SelectCommandBuilder builder = new SelectCommandBuilder(DBC, "FC_EDC");
			builder.SelectColumns("EDC_SID,EDC_NO,EDC_NAME");
			//builder.SelectColumn("EDC_SID");
			//builder.SelectExpressColumn("'['||EDC_NO ||']'||  '~' || EDC_NAME", "EDC_NO");
			builder.WhereAnd("EDC_TYPE", "EDC");
			builder.WhereAnd("EDC_NO", SQLOperator.StartWith, "%");

			builder.OrderByColumn("EDC_NO");
			var item1 = builder.DoQuery();

			new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"IPQC\t_GetOperToolSourceList.json"));

		}
		/// <summary>
		/// 取得 工程參數 EDC群組
		/// </summary>
		/// \GTIMES_2015\GTiMES\ADM\EquipmentData.aspx.EDC.cs - txtEDC_TextChanged
		[TestMethod]
		public void t_GetOperToolSourceList_svc()
		{
			var item1 = EDCServices.Search("T");
			new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"IPQC\t_GetOperToolSourceList_svc.json"));
		}

		/// <summary>
		/// 取得 工程參數 的版本
		/// </summary>
		/// \GTIMES_2015\GTiMES\ADM\EquipmentData.aspx.EDC.cs - ddlTabEQP_EDC_SelectedIndexChanged
		[TestMethod]
		public void t_GetEDC_Ver()
		{
			EDCUtility.EDCInfo EDCInfo = new EDCUtility.EDCInfo(DBC, "GTI19040815091400008");
			SelectCommandBuilder builder = new SelectCommandBuilder(DBC, "FC_EDC_VER");
			builder.SelectColumns("VERSION,EDC_VER_SID,DEFAULT_FLAG");
			builder.WhereAnd("EDC_SID", EDCInfo.SID);
			builder.WhereAnd("VERSION_STATE", "Enable");
			builder.OrderByColumn("VERSION");
			var item1 = builder.DoQuery();

			new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"IPQC\t_GetEDC_Ver.json"));

		}

		/// <summary>
		/// 工程參數 版本 的對應資料
		/// </summary>
		/// \GTIMES_2015\GTiMES\ADM\EquipmentData.aspx.EDC.cs - ddlTabEQP_EDCVer_SelectedIndexChanged
		[TestMethod]
		public void t_GetEDCinfo_ByVer()
		{
			EDCUtility.EDCVersionParameterFunction function = new EDCUtility.EDCVersionParameterFunction(this.DBC);
			var item1 = function.GetQuery("EDC_VER_SID", "GTI19121913522509676");
			item1.Columns.Add("DISPLAY_POINT_NAME", typeof(string));

			new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"IPQC\t_GetEDCinfo_ByVer.json"));

		}

		/// <summary>
		/// 工程參數 版本 的對應資料
		/// </summary>
		/// \GTIMES_2015\GTiMES\ADM\EquipmentData.aspx.EDC.cs - ddlTabEQP_EDCVer_SelectedIndexChanged
		[TestMethod]
		public void t_GetLotBaseInfo_ByLOT()
		{
			EDCUtility.EDCVersionParameterFunction function = new EDCUtility.EDCVersionParameterFunction(this.DBC);
			var item1 = function.GetQuery("EDC_VER_SID", "GTI19121913522509676");
			item1.Columns.Add("DISPLAY_POINT_NAME", typeof(string));

			new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"IPQC\t_GetEDCinfo_ByVer.json"));

		}


		[TestMethod]
		public void t_GetEDC_Data()
		{
			var _r = QMSService.GetEDC_Data("AA");
			new FileApp().Write_SerializeJson(_r, _log.t_GetEDC_Data);

		}

		[TestMethod]
		public void t_NoSampling_Save()
		{
			var x = new
			{
				form = new WP_IPQC(),
				lot_list = new List<WP_IPQC_LOT>(),
				data_input = new List<BLL.DataViews.Edc.EdcModel>()
			};
			var _r = new FileApp().Read_SerializeJson(_log.t_無抽樣巡檢_server取值樣本, x);

			var _r1 = QMSService.NoSampling_Save(_r.form, _r.lot_list, _r.data_input, true);
			//new FileApp().Write_SerializeJson(_r, _log.t_GetEDC_Data);
		}

		[TestMethod]
		public void t_Search_QCResult_Query()
		{

			var _r = new FileApp().Read_SerializeJson<Pagination>(_log.t_Search_QCResult_Query);

			var _r1 = QMSService.Search_QCResult_Query(_r);
			new FileApp().Write_SerializeJson(_r1, _log.t_Search_QCResult_Query_result);
		}

		[TestMethod]
		public void t_QCResult()
		{
			var zzz = new string[] { };
			var ggx = new List<KeyValuePair<string, object>>();

			dynamic x1 = new ExpandoObject();//= new { F = "FK" };
											 //var _r1 = new QMSService().QCResult("GTI20091817340364397");
			using (var dbContext = new MDL.MESContext())
			{
				x1 = (from t in dbContext.WP_IPQC_CHECKITEM
					  where t.ACTION_LINK_SID == "GTI20092116354270600"
					  group t by new
					  {
						  t.WP_IPQC_CHECKITEM_SID,
						  t.EDC_VER_SID,
						  t.ROUTE_VER_OPER_SID,
						  t.OPERATION,
						  t.ITEM_NO,
						  t.ITEM,
						  t.DATATYPE,
						  DISPLAY_POINT_NAME = t.DISPLAY_POINT_NAME ?? "",

					  } into grp
					  select new BLL.DataViews.Edc.EdcModel()
					  {
						  ItemSid = grp.Key.WP_IPQC_CHECKITEM_SID,
						  EdcVerSid = grp.Key.EDC_VER_SID,
						  //grp.Key.ROUTE_VER_OPER_SID,
						  //grp.Key.OPERATION,
						  //grp.Key.ITEM_NO,
						  //grp.Key.ITEM,
						  //MustInput = "F",
						  //UCL = 0,
						  //USL = 0,
						  //TL = "F",
						  //LCL = 0,
						  //LSL = 0,
						  DataType = grp.Key.DATATYPE,
						  DispayPointName = grp.Key.DISPLAY_POINT_NAME,
						  // DispayPointNameList = (string.IsNullOrEmpty(grp.Key.DISPLAY_POINT_NAME)
						  //? new List<string>()
						  //: grp.Key.DISPLAY_POINT_NAME.Split(',').ToList()),
						  //InputValueList = (from dd in ggx select dd).ToList(),
						  //InputValueList =
						  //(from zz in dbContext.WP_IPQC_CHECKITEM_RAW
						  // where zz.ACTION_LINK_SID == grp.Key.WP_IPQC_CHECKITEM_SID
						  // //select new <KeyValuePair<string, string>>() { new KeyValuePair<string, string>(zz.QC_SEQ.ToString(), zz.QC_DATA) }
						  // //select new KeyValuePair<string, string>(zz.QC_SEQ.ToString(), zz.QC_DATA)
						  // //as Dictionary<string, string>

						  // //.Select(p =>{
						  // // return new Dictionary<string, string> { { p.QC_SEQ.ToString(), p.QC_DATA } }
						  // //})

						  // select (Dictionary<string, string>)new
						  // {
						  //  zz.QC_SEQ.ToString(),
						  //  zz.QC_DATA,
						  // }
						  //)
						  ////.ToList<Dictionary<string, string>>()
						  //.ToList()
						  //
					  }).ToList();
			}





			FileApp.WriteSerializeJson(x1, _log.t_QCResult, null);

		}


		[TestMethod]
		public void t_QCResult_1()
		{
			dynamic f = new ExpandoObject();
			using (var dbc = mes.dbc())
			{
				var QC_NO = "GTI20092116354270600";
				List<IDbDataParameter> parameters = new List<IDbDataParameter>();
				dbc.AddCommandParameter(parameters, "QC_NO", QC_NO);

				f = QMSService.parseEdcInput(dbc, QC_NO);
			}

			FileApp.WriteSerializeJson(f, _log.t_QCResult, null);

		}



		private static dynamic NewMethod(DBController dbc)
		{
			dynamic f;
			var WP_IPQC_CHECKITEM = new QMSService().WP_IPQC_CHECKITEM("GTI20092116354270600");

			var _sql1 = @"
					SELECT	W1.*
					FROM	WP_IPQC_CHECKITEM AS W0 
							INNER JOIN WP_IPQC_CHECKITEM_RAW AS W1 
								ON W1.ACTION_LINK_SID = W0.WP_IPQC_CHECKITEM_SID
					WHERE	W0.ACTION_LINK_SID = :QC_NO
				";

			List<IDbDataParameter> parameters = new List<IDbDataParameter>();
			dbc.AddCommandParameter(parameters, "QC_NO", "GTI20092116354270600");
			_sql1 = dbc.GetCommandText(_sql1, SQLStringType.OracleSQLString);
			var WP_IPQC_CHECKITEM_RAW = dbc.Select(_sql1, parameters);


			f = (from z in WP_IPQC_CHECKITEM
				 select new BLL.DataViews.Edc.EdcModel()
				 {
					 QCItemSID = z.WP_IPQC_CHECKITEM_SID,
					 ItemSid = z.ACTION_LINK_SID,
					 EdcVerSid = z.EDC_VER_SID,
					 DataType = z.DATATYPE,
					 DispayPointName = z.DISPLAY_POINT_NAME,
				 })
					.ToList();

			foreach (BLL.DataViews.Edc.EdcModel z in f)
			{
				var _list = (from g in WP_IPQC_CHECKITEM_RAW.AsEnumerable()
							 where g.Field<string>("ACTION_LINK_SID") == z.QCItemSID
							 select new
							 {
								 QC_SEQ = (int)g.Field<decimal>("QC_SEQ"),
								 QC_DATA = g.Field<string>("QC_DATA")
							 }
							).ToList();
				var isDef = string.IsNullOrEmpty(z.DispayPointName);
				var _arr = isDef ? new string[] { } : z.DispayPointName.Split(',');
				z.InputValueList = new List<Dictionary<string, string>>();
				foreach (var row in _list)
				{
					var _key = isDef
						? row.QC_SEQ.ToString()
						: _arr[row.QC_SEQ - 1]
						;
					var _dc = new Dictionary<string, string> { { _key, row.QC_DATA } };
					z.InputValueList.Add(_dc);
				}
			}

			return f;
		}


		[TestMethod]
		public void t_ZZZ()
		{
			//
			var Encode = new EncodeFormatUtility.EncodeFormatInfo
								(mes.dbc(), "LotScrap_NO", EncodeFormatUtility.IndexType.No);
			if (Encode.IsExist == true)
			{
				var Code = EncodeFormatUtility.Coder.GetCodes
					(mes.dbc(), "AdminTest", Encode, 1, "", ""
					, "LotNo", "", false);

			}

		}
	}
}
