using Genesis.Library.BLL.ICM.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.ICM.Maintain;
using Dapper;
using BLL.DataViews.Edc;

namespace UnitTestProject
{
	[TestClass]
	public class t_ICM : _testBase
	{
		static class _log
		{
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
			internal static string t_QC_INSTRUMENTS_STATE
			{
				get
				{
					return FileApp.ts_Log(@"ICM\QC_INSTRUMENTS_STATE.json");
				}
			}
			internal static string t_QC_INSTRUMENTS
			{
				get
				{
					return FileApp.ts_Log(@"ICM\QC_INSTRUMENTS.json");
				}
			}
			internal static string QC_INSTRUMENTS_CALIBRATION_RECORDS_Save
			{
				get
				{
					return FileApp.ts_Log(@"ICM\QC_INSTRUMENTS_CALIBRATION_RECORDS_Save.json");
				}
			}
		}


		[TestMethod]
		public void t_SQL_查詢範例()
		{

			using (var dbc = this.DBC)
			{
				var _sql = $@"
                SELECT  top 1 *
                FROM    PF_PARTNO_VER

                         ";

				var dt = dbc.Select(_sql);

				new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"DB\ZZ_LOT_BIN.json"));

			}
		}
		[TestMethod]
		public void t_QC_INSTRUMENTS_STATE()
		{
			var _r = FileApp.Read_SerializeJson<QC_INSTRUMENTS_STATE>(_log.t_QC_INSTRUMENTS_STATE);
			Maintain.QC_INSTRUMENTS_STATE_Save(_r,true);
		}

		[TestMethod]
		public void t_QC_INSTRUMENTS()
		{
			var z = new {
				edcData = new List<QC_INSTRUMENTS_EDC>(),
				data = new QC_INSTRUMENTS(),
				isNewEdc=false,
			};
			var _r = new FileApp().Read_SerializeJson(_log.t_QC_INSTRUMENTS,z);
			Maintain.QC_INSTRUMENTS_Save(_r.data,_r.edcData, _r.isNewEdc,true);

		}

		[TestMethod]
		public void t_QC_INSTRUMENTS_1()
		=> _DBTest((Txn) => {
			var SID = "GTI23062015431043743";
			var _sql = @"
					SELECT 	A.*,B.STATE_NAME
					FROM	QC_INSTRUMENTS A
							LEFT JOIN QC_INSTRUMENTS_STATE B
								ON A.STATE_SID = B.STATE_SID
					WHERE	A.INSTRUMENT_SID = @SID 
				";
			dynamic arg = new { SID };// parseArg(SearchType, Search, ref _sql);
			List<QC_INSTRUMENTS_V> form = Txn.DapperQuery<QC_INSTRUMENTS_V>(_sql, arg);
			//;.FirstOrDefault();
		}, true);


		struct d_QC_INSTRUMENTS_CALIBRATION_RECORDS_Save {
			public QC_INSTRUMENTS_CALIBRATION_RECORDS form ;
			public List<EdcModel> edcData;
		}

		[TestMethod]
		public void t_QC_INSTRUMENTS_CALIBRATION_RECORDS_Save()
		{
			var _r = FileApp.Read_SerializeJson<d_QC_INSTRUMENTS_CALIBRATION_RECORDS_Save>
				(_log.QC_INSTRUMENTS_CALIBRATION_RECORDS_Save);
			Maintain.QC_INSTRUMENTS_CALIBRATION_RECORDS_Save(_r.form,_r.edcData,true);

		}

		[TestMethod]
		public void t_QC_INSTRUMENTS_STATE_Delete()
		{
			//Maintain.QC_INSTRUMENTS_STATE_Delete("GTI23061909503543676", true);

		}


		

	}


}
