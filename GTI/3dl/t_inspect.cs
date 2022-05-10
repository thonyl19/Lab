using cBLL.MES;
using Genesis.Gtimes.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Helpers;
using UnitTestProject.TestUT;
using static cBLL.MES.inspect;
using cls = cDAL.MES.inspect;
using cls_layering = cDAL.MES.layering;
using mdl_layering = MDL.MES.Views.layering;
using view = MDL.MES.Views.inspect;


namespace UnitTestProject1
{
    [TestClass]
	public class t_inspect : _testBase
	{
 
		static class _log
		{
            internal static string test_xls
			{
				get
				{
					return FileApp.ts_Log(@"3DL\~test.xls");
				}
			}

			internal static string 正式測試_test_內料檢_xls
			{
				get
				{
					return FileApp.ts_Log(@"3DL\正式測試_test_內料檢.xls");
				}
			}

			

			internal static string t_SQL
			{
				get
				{
					return FileApp.ts_Log(@"3DL\t_SQL.json");
				}
			}

			internal static string t_layering_load
			{
				get
				{
					return FileApp.ts_Log(@"3DL\t_layering_load.json");
				}
			}

 
			internal static string t_para_load
			{
				get
				{
					return FileApp.ts_Log(@"3DL\t_para_load.json");
				}
			}

			/// <summary>
			/// 此為正式測試資料,
			/// </summary>
			internal static string t_para_load_內料檢
			{
				get
				{
					return FileApp.ts_Log(@"3DL\t_para_load_內料檢.json");
				}
			}

 
			internal static string 正式測試_para_load_內料檢
			{
				get
				{
					return FileApp.ts_Log(@"3DL\正式測試_para_load_內料檢.json");
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
					ConnectionStringSettings SmartQueryConn = ConfigurationManager.ConnectionStrings["sql.mes"];
					this._dbc = new DBController(SmartQueryConn);
				}
				return this._dbc;
			}
		}

		[TestMethod]
		public void t_para_load()
		{
			var t  = cls.para.load("QC-W58-01", null);
			new FileApp().Write_SerializeJson(t,_log.t_para_load);
		}

		[TestMethod]
		public void t_para_load_內料檢()
		{
			/*
			 科毅特有的 工程參數設定方式  , 是用{科毅檢驗單號}.{料號} 的格式 組成,
				所以需要把 兩個資料都傳入
			 */
			var t = cls.para.load("QC-W008-003", "30249CO001M001");
			new FileApp().Write_SerializeJson(t, _log.t_para_load);
		}
		[TestMethod]
		public void t_() {
			var _d = DateTime.Now;
			var g_sid = DBC.GetSID();
			MDL.MES.ZZ_INSP_MASTER master = new MDL.MES.ZZ_INSP_MASTER()
            {
                SID = g_sid,
                INSP_TYPE = "3-QC-W034-001",
				INSP_NO = "3-QC-W034-001-20210714-010",
				STATUS = "Normal",
                QTY = 0,
				CREATE_DATE = _d,
                CREATE_USER = "mes",
				UPDATE_DATE = _d,
                UPDATE_USER = "mes",
				S01 = "E-1-1",
			};
			var t = cls.para.load("QC-W58-01", null);

			List<MDL.MES.ZZ_INSP_DATA> data = new List<MDL.MES.ZZ_INSP_DATA>();
			foreach (var d in t.OrderBy(m => m.seq))
				{
					//未填原因不進資料庫
					if (d.category.ToLower() == "reason" && string.IsNullOrEmpty(d.value)) continue;

					data.Add(new MDL.MES.ZZ_INSP_DATA()
					{
						SID = DBC.GetSID(),
						INSP_MASTER_SID = g_sid,
						CATEGORY = d.category,
						EDC_VER_SID = d.parentKey,
						EDC_SID = d.parentSid,
						EDC_NO = d.parentNo,
						EDC_NAME = d.parentName,
						VERSION = d.version,
						EDC_VER_PARA_SID = d.paraKey,
						SEQ = d.seq,
						PARA_SID = d.paraSid,
						PARA_NO = d.paraNo,
						PARA_NAME = d.paraName,
						DATATYPE = d.type,
						USL = d.USL,
						UCL = d.UCL,
						TL = d.TL,
						LCL = d.LCL,
						LSL = d.LSL,
						SAMPLESIZE = d.sampleSize,
						THROW_SPC = d.toSPC ? "T" : "F",
						MUST_INPUT = d.must ? "T" : "F",
						DISPLAY_POINT_NAME = JsonConvert.SerializeObject(d.mDisplay),
						EDC_VALUE = d.value,
						CREATE_DATE = _d,
						CREATE_USER = "mes",
						UPDATE_DATE = _d,
						UPDATE_USER = "mes"
					});
				}
			 

			cls.add(master, data);
		}


		[TestMethod]
		public void t_layering_load()
		{
			mdl_layering.query conditions = new mdl_layering.query() { type = new List<string>() { "A" } };
			var t = cls_layering.load(conditions); ;
			new FileApp().Write_SerializeJson(t, _log.t_layering_load);
		}


		[TestMethod]
		public void t_SQL()
        {
			var day = "2021/07/14";
			var SPC_LAYERING_TYPE = "A";

			DataTable dt = get_LayerByDay(day, SPC_LAYERING_TYPE);
            new FileApp().Write_SerializeJson(dt, _log.t_SQL);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="formDay"></param>
		/// <param name="SPC_LAYERING_TYPE"></param>
		/// <returns></returns>
        private DataTable get_LayerByDay(string formDay,string SPC_LAYERING_TYPE)
        {
            string sql = @"SELECT A.SPC_LAYERING_NO , 
						sum(case when format(B.D01,'yyyy/MM/dd') = :formDay then 1 else 0 end )
					FROM ZZ_SPC_LAYERING A
						LEFT JOIN ZZ_INSP_MASTER B
							ON B.S01 = A.SPC_LAYERING_NO
					where A.ENABLE_FLAG = 'T'
							AND A.SPC_LAYERING_TYPE = :SPC_LAYERING_TYPE  
					group by A.SPC_LAYERING_NO
					 order by  A.SPC_LAYERING_NO
			";
            sql = DBC.GetCommandText(sql, SQLStringType.SqlServerSQLString);
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            DBC.AddCommandParameter(parameters, "SPC_LAYERING_TYPE", SPC_LAYERING_TYPE);
            DBC.AddCommandParameter(parameters, "formDay", formDay);
            var dt = DBC.Select(sql, parameters);
            return dt;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="formDay"></param>
		/// <param name="SPC_LAYERING_TYPE"></param>
		/// <returns></returns>
		private DataTable get_INSP_ByDayAndType(string formDay, string INSP_TYPE ,string SPC_LAYERING_TYPE)
		{
			string sql = @"select B.SPC_LAYERING_NAME,A.INSP_NO
					from	ZZ_INSP_MASTER A
							LEFT JOIN ZZ_SPC_LAYERING B
								ON B.SPC_LAYERING_TYPE= :SPC_LAYERING_TYPE
									AND B.ENABLE_FLAG ='T'
									AND B.SPC_LAYERING_NO = A.S01
					where format(A.D01,'yyyy-MM-dd') = :formDay
							AND A.INSP_TYPE = :INSP_TYPE
					ORDER BY A.S01,INSP_NO 
			";
			sql = DBC.GetCommandText(sql, SQLStringType.OracleSQLString);
			List<IDbDataParameter> parameters = new List<IDbDataParameter>();
			DBC.AddCommandParameter(parameters, "formDay", formDay);
			DBC.AddCommandParameter(parameters, "INSP_TYPE", INSP_TYPE);
			DBC.AddCommandParameter(parameters, "SPC_LAYERING_TYPE", SPC_LAYERING_TYPE);
			var dt = DBC.Select(sql, parameters);
			return dt;
		}


		[TestMethod]
		public void t_水質檢測_批次建立_原型()
        {
            var day = "2021/07/15";
            var SPC_LAYERING_TYPE = "A";
            var type = "3-QC-W034-001";
            var fun_type = "QC-W58-01";
            NewMethod(day, SPC_LAYERING_TYPE, type, fun_type);
        }



		private void NewMethod(string day, string SPC_LAYERING_TYPE, string type, string fun_type)
        {
            DataTable dt = get_LayerByDay(day, SPC_LAYERING_TYPE);
            var t = cls.para.load(fun_type, null);
            foreach (DataRow row in dt.Rows)
            {
                if (row["Column1"].ToString() != "0") continue;
                var _d = DateTime.Now;
                var _INSP_NO = cBLL.MES.inspect.getNewSerialNo(type, "mes", DBC); ;
                MDL.MES.ZZ_INSP_MASTER master = new MDL.MES.ZZ_INSP_MASTER()
                {
                    SID = DBC.GetSID(),
                    INSP_TYPE = type,
                    INSP_NO = _INSP_NO,
                    STATUS = "Normal",
                    QTY = 0,
                    CREATE_DATE = _d,
                    CREATE_USER = "mes",
                    UPDATE_DATE = _d,
                    UPDATE_USER = "mes",
                    S01 = row["SPC_LAYERING_NO"].ToString(),
                    D01 = _d
                };

                List<MDL.MES.ZZ_INSP_DATA> data = new List<MDL.MES.ZZ_INSP_DATA>();
                foreach (var d in t.OrderBy(m => m.seq))
                {
                    //未填原因不進資料庫
                    if (d.category.ToLower() == "reason" && string.IsNullOrEmpty(d.value)) continue;

                    data.Add(new MDL.MES.ZZ_INSP_DATA()
                    {
                        SID = DBC.GetSID(),
                        INSP_MASTER_SID = master.SID,
                        CATEGORY = d.category,
                        EDC_VER_SID = d.parentKey,
                        EDC_SID = d.parentSid,
                        EDC_NO = d.parentNo,
                        EDC_NAME = d.parentName,
                        VERSION = d.version,
                        EDC_VER_PARA_SID = d.paraKey,
                        SEQ = d.seq,
                        PARA_SID = d.paraSid,
                        PARA_NO = d.paraNo,
                        PARA_NAME = d.paraName,
                        DATATYPE = d.type,
                        USL = d.USL,
                        UCL = d.UCL,
                        TL = d.TL,
                        LCL = d.LCL,
                        LSL = d.LSL,
                        SAMPLESIZE = d.sampleSize,
                        THROW_SPC = d.toSPC ? "T" : "F",
                        MUST_INPUT = d.must ? "T" : "F",
                        DISPLAY_POINT_NAME = JsonConvert.SerializeObject(d.mDisplay),
                        EDC_VALUE = d.value,
                        CREATE_DATE = _d,
                        CREATE_USER = "mes",
                        UPDATE_DATE = _d,
                        UPDATE_USER = "mes"
                    });
                }

                cls.add(master, data);
            }
        }

        [TestMethod]
		public void t_匯出_原型()
        {
            var day = "2021-07-20";
            var SPC_LAYERING_TYPE = "A";
            var fun_type = "QC-W58-01";
            匯出程序(day, SPC_LAYERING_TYPE, fun_type);

        }


		[TestMethod]
		public void t_落菌檢驗_批次建立_原型()
		{
			var day = "2021/07/15";
			var SPC_LAYERING_TYPE = "B";
			var type = "1-QC-W039-001";
			var fun_type = "QC-W59-01";
			NewMethod(day, SPC_LAYERING_TYPE, type, fun_type);
		}

		[TestMethod]
		public void t_落菌檢驗_匯出()
		{
			var day = "2021-07-15";
			var SPC_LAYERING_TYPE = "B";
			var fun_type = "QC-W59-01";
			匯出程序(day, SPC_LAYERING_TYPE, fun_type);

		}

		private void 匯出程序(string day, string SPC_LAYERING_TYPE, string fun_type)
        {
            DataTable dt = get_INSP_ByDayAndType(day, fun_type, SPC_LAYERING_TYPE);
            var _fileds = cls.para.load(fun_type, null).Select(e => new { e.paraName, e.display }).ToList();
            _fileds.Insert(0, new { paraName = "INSP_NO", display = string.Empty });
            _fileds.Insert(0, new { paraName = "INSP_LOCATION", display = string.Empty });

            hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet(day);
            IDrawing patr = (HSSFPatriarch)sheet.CreateDrawingPatriarch();

            var _row_title = sheet.CreateRow(0);
            for (var i = 0; i < _fileds.Count; i++)
            {
                var _cell = _row_title.CreateCell(i);
                _cell.SetCellValue(new HSSFRichTextString(_fileds[i].paraName));

                var isNeedComment = string.IsNullOrEmpty(_fileds[i].display) == false;
                if (isNeedComment)
                {
                    IComment comment1 = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, 4, 2, 6, 5));
                    comment1.String = (new HSSFRichTextString(_fileds[i].display));
                    _cell.CellComment = (comment1);
                }
            }

            var row_idx = 1;
            foreach (DataRow row in dt.Rows)
            {
                var _row = sheet.CreateRow(row_idx);
                _row.CreateCell(0).SetCellValue(new HSSFRichTextString(row["SPC_LAYERING_NAME"].ToString()));
                _row.CreateCell(1).SetCellValue(new HSSFRichTextString(row["INSP_NO"].ToString()));
                row_idx++;
            }
            WriteToFile();
        }

        static HSSFWorkbook hssfworkbook;


		[TestMethod]
		public void  InitializeWorkbook()
		{
			hssfworkbook = new HSSFWorkbook();
			//创建一个DocumentSummaryInformation条目
			DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
			dsi.Company = "NPOI Team";
			hssfworkbook.DocumentSummaryInformation = dsi;
			//创建一个汇总信息条目
			SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
			si.Subject = "NPOI SDK Example";
			hssfworkbook.SummaryInformation = si;
		}


		/// <summary>
		/// [Ref]https://www.cjavapy.com/article/754/
		/// </summary>
		[TestMethod]
		public void t_讀取XLS()
		{
			var _batch_time = DateTime.Now;
			var _user = "mes";

			using (FileStream file = new FileStream(_log.test_xls, FileMode.Open, FileAccess.Read))
			{
				hssfworkbook = new HSSFWorkbook(file);
			}
			ISheet sheet = hssfworkbook.GetSheetAt(0);
			var _date = sheet.SheetName;
			using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
			{
				using (var db = new MDL.MESContext())
				{
					for (int row = 1; row <= sheet.LastRowNum; row++)
					{
						var _row = new _sheetRow(sheet, row);
						if (_row.Exists == false) continue;

						var insp_no = _row.insp_no;
						var _insp_m = db.ZZ_INSP_MASTER
							.Where(m => m.INSP_NO == _row.insp_no)
							.FirstOrDefault();
						if (_insp_m == null) { 
							_row.setAct("找不到對應單號");
							continue;
						}
						else if (_insp_m.UPDATE_DATE != _insp_m.CREATE_DATE)
						{
							_row.setAct("資料己經被修改過,略過不匯入");
							continue;
						}


						var _insp_d = db.ZZ_INSP_DATA
							.Where(m => m.INSP_MASTER_SID == _insp_m.SID
							&& String.IsNullOrEmpty(m.EDC_VALUE)
							).ToList();
						if (_insp_d == null)
						{
							_row.setAct("找不到需要更新的項目");
							continue;
						}

						List<view.data> model = null;

						List<string> err = new List<string>();
						foreach (var edc_item in _insp_d)
						{
							string[] edc_val;
							_row.edc.TryGetValue(edc_item.PARA_NAME, out edc_val);
							if (edc_item.MUST_INPUT == "T" && edc_val == null)
							{
								err.Add($"[{edc_item.PARA_NAME}] 為必填欄位");
								continue;
							}
							if (edc_item.DATATYPE == "N") {
								var d = edc_item;
								decimal u = decimal.MaxValue;
								decimal l = decimal.MinValue;
								if (d.UCL.HasValue) u = d.UCL.Value; else if (d.USL.HasValue) u = d.USL.Value;
								if (d.LCL.HasValue) l = d.LCL.Value; else if (d.LSL.HasValue) l = d.LSL.Value;
								foreach (var v in edc_val)
								{
									if (string.IsNullOrEmpty(v)) continue;
									decimal _v = 0;
									if (!decimal.TryParse(v, out _v)) continue;
									//if (model == null) model = new List<view.data>();
									//if (_v > u | _v < l) model.Add(d);
								}
							}

							else if (edc_val != null)
							{
								edc_item.EDC_VALUE = JsonConvert.SerializeObject(edc_val);
								edc_item.UPDATE_DATE = _batch_time;
								edc_item.UPDATE_USER = _user;
							}
						}
						if (err.Count == 0)
						{
							_insp_m.UPDATE_DATE = _batch_time;
							_insp_m.UPDATE_USER = _user;
							db.SaveChanges();
							_row.setAct("匯入完成");
						}
						else
						{
							_row.setAct(string.Join(",", err.ToArray()));
						}
					}
					scope.Complete();
				}
			}
			WriteToFile();
		}


		/// <summary>
 
		/// </summary>
		[TestMethod]
		public void t_讀取XLS1()
		{
			var _batch_time = DateTime.Now;
			var _user = "mes";

			MemoryStream stream1 = inspect.Import_QC_W5n(_log.test_xls);
			using (FileStream file = new FileStream(_log.test_xls, FileMode.Create, System.IO.FileAccess.Write)) { 
				stream1.CopyTo(file);
			}
		}
		/// <summary>
		/// [Ref]https://www.cjavapy.com/article/754/
		/// </summary>
		[TestMethod]
		public void t_1()
		{
			//using (var _DbCtx = DbCtx ?? new MDL.MESContext())
			//{
			//using (var db = new MDL.MESContext())
			//{
			//	var e = db.ZZ_INSP_MASTER
			//		.Where(m => m.INSP_NO == "1-QC-W039-001-20210715-011" 
			//		&& m.CREATE_DATE == m.UPDATE_DATE
			//		)
			//		.FirstOrDefault();
			//}
			using (var db = new MDL.MESContext())
			{
				var e = db.ZZ_INSP_MASTER
					.Where(m => m.PARTNO == "30249CO001M001"
					&& m.D01 == m.UPDATE_DATE
					&& m.S01 == m.S01
					)
					.FirstOrDefault();
			}
		}
		/// <summary>
		/// [Ref]https://www.cjavapy.com/article/754/
		/// </summary>
		[TestMethod]
		public void t_第一個基本測例()
		{
			_Base();
			WriteToFile();
		}


		[TestMethod]
		public void t_sheetRow()
		{
			//讀取 內料檢的 EDC 格式範本 
			var _edcBase = new FileApp().Read_SerializeJson<List<view.data>>(_log.正式測試_para_load_內料檢);

			FileStream file = new FileStream(_log.正式測試_test_內料檢_xls, FileMode.Open, FileAccess.Read);
			var hssfworkbook = new HSSFWorkbook(file);
			ISheet sheet = hssfworkbook.GetSheetAt(0);
			var _row = new sheetRow(sheet, 2, "D01", "PART_NO", "LOT");

			var act = _edcBase[1];
			_row.edcItem[act.paraNo] = new string[] {"" };
			{
				var _t = _row.check(act);
				Assert.AreEqual($"[{act.paraNo} - 內料半成品-pH值]為必填欄位", _t, "欄位為空值,應該要有檢核訊息");
			}
			
			_row.edcItem[act.paraNo] = new string[] { "1" };
			Assert.AreEqual("", _row.check(act), "把欄位設任意值,應該要檢核通過");
			
			act.must = false;
			_row.edcItem[act.paraNo] = new string[] {""};
			_row.check(act);
			Assert.AreEqual(act.value, null, "內容應該為 null");


			act = _edcBase[0];
			_row.edcItem[act.paraNo] = new string[] { "A" };
			{
				var _t = _row.check(act) ;
				Assert.AreEqual("[內料半成品-黏度]為必填欄位,且資料數量必須為 2", _t, "欄位為空值,應該要有檢核訊息");
			}

			//因為此案例為字串格式,所以應該要順利通過
			_row.edcItem[act.paraNo] = new string[] { "A,B" };
			Assert.AreEqual("", _row.check(act), "改為逗號分隔的值,應該要檢核通過");

			act.type = "N";
			Assert.AreEqual("資料型別為數值,填寫值為數字.", _row.check(act), "型別改為數字,應該要有檢核訊息");

			_row.edcItem[act.paraNo] = new string[] { "1,1.5" };
			Assert.AreEqual("", _row.check(act), "把資料改為數值,應該要檢核通過");

			act.type = "B";
			Assert.AreEqual("資料型別為布林,填寫值必須為 T 或 F.", _row.check(act), "型別改為布林,應該要有檢核訊息");

			_row.edcItem[act.paraNo] = new string[] { "F,T" };
			Assert.AreEqual("", _row.check(act), "把資料改為布林,應該要檢核通過");

			Assert.AreEqual(act.value,Json.Encode(new List<string> { "F","T" }), "內容應該要一致");

		}


		/// <summary>
		/// 特殊個案測試
		/// </summary>
		[TestMethod]
		public void t_sheetRow_byCase() {
			//讀取 內料檢的 EDC 格式範本 
			var _edcBase = new FileApp().Read_SerializeJson<List<view.data>>(_log.正式測試_para_load_內料檢);

			FileStream file = new FileStream(_log.正式測試_test_內料檢_xls, FileMode.Open, FileAccess.Read);
			var hssfworkbook = new HSSFWorkbook(file);
			ISheet sheet = hssfworkbook.GetSheetAt(0);
			var _row = new sheetRow(sheet, 2, "D01", "PART_NO", "LOT");

			var act = _edcBase[1];
			_row.edcItem[act.paraNo] = new string[] { "" };
			{
				var _t = _row.check(act);
				Assert.AreEqual($"[{act.paraNo} - 內料半成品-pH值]為必填欄位", _t, "欄位為空值,應該要有檢核訊息");
			}

		}


		[TestMethod]
		public void __Import_QC_W008_003()
		{
			cBLL.MES.inspect.Import_QC_W008_003(_log.正式測試_test_內料檢_xls,this.DBC);
		}

 
		
		

		public pack _Base()
		{
			InitializeWorkbook();
			ISheet sheet = hssfworkbook.CreateSheet("ICell comments in POI HSSF");
			//创建绘图族长。这是所有形状(包括单元格注释)的顶级容器。
			IDrawing patr = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
			//在第3行创建一个单元格
			ICell cell1 = sheet.CreateRow(3).CreateCell(1);
			cell1.SetCellValue(new HSSFRichTextString("Hello, World"));
			return new pack(patr, cell1);

		}

		static void WriteToFile()
		{
			//将工作簿的流数据写入根目录
			FileStream file = new FileStream(_log.test_xls, FileMode.Create);
			hssfworkbook.Write(file);
			file.Close();
		}
	}

    internal class _sheetRow
    {
        private ISheet sheet;
		IRow _row;
		IRow _title;
		public string insp_no;
		public Dictionary<string, string[]> edc = new Dictionary<string, string[]>();

		public bool Exists {
			get {
				return _row != null;
			}
		}

		public _sheetRow(ISheet sheet, int row)
        {
            this.sheet = sheet;
            _row = sheet.GetRow(row);
			if (this.Exists) { 
				_title = sheet.GetRow(0);
				insp_no = _row.Cells[1].StringCellValue;
				for (int _idx = 2; _idx < _title.Cells.Count; _idx++) {
					var _key = _title.Cells[_idx].StringCellValue;
					var _val = _row.GetCell(_idx, MissingCellPolicy.RETURN_NULL_AND_BLANK);
					if (_val != null) { 
						//var _val = new string[] { _row.GetCell(_idx,MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString()};
						edc.Add(_key, new string[] { _val.ToString() });
					}
				}
			}
		}

		public void setAct(string msg) {
			_row.CreateCell(_title.Cells.Count).SetCellValue(msg);
		}
	}

    public class pack
	{
		public pack(IDrawing patr, ICell cell)
		{
			this.patr = patr;
			this.cell = cell;

		}
		public IDrawing patr { get; set; }
		public ICell cell { get; set; }
		public IComment comment { get; set; }
	}

	public class pack_1
	{
		public pack_1(IDrawing patr, ICell cell)
		{
			this.patr = patr;
			this.cell = cell;

		}
		public IDrawing patr { get; set; }
		public ICell cell { get; set; }
		public IComment comment { get; set; }
	}
}
