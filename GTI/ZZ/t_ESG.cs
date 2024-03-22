using Frame.Code.Web.Select;
using Genesis.Library.BLL.MES.DataViews;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject.TestUT;
using Maintain = Genesis.Library.BLL.ESG.Maintain;

namespace UnitTestProject
{
	[TestClass]
	public class t_ESG : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string ESG_EMISSION_PARTNO_RELATION_Add
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\ESG\ESG_EMISSION_PARTNO_RELATION_Add.json");
				}
			}
			internal static string ESG_RELATION_GATEWAY_Save
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\ESG\ESG_RELATION_GATEWAY_Save.json");
				}
			}

			internal static string tt
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\ESG\tt.json");
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


		struct _d_Transfer
		{
			public string PART_NO { get; set; }
			public SelectModel item { get; set; }
		}

		[TestMethod]
		public void t_ESG_EMISSION_PARTNO_RELATION_Add()
		{
			var _r = FileApp.Read_SerializeJson<_d_Transfer>(_log.ESG_EMISSION_PARTNO_RELATION_Add);
			var z = Maintain.ESG_EMISSION_PARTNO_RELATION_Add(_r.PART_NO,_r.item, true);
		}



		[TestMethod]
		public void t_ESG_CARBON_EMISSION_FACTORY()
		{
			var 新增 = Maintain.ESG_CARBON_EMISSION_FACTORY(null);
			var 查詢bySID = Maintain.ESG_CARBON_EMISSION_FACTORY(82);
			var 查詢byEMISSION_SOURCE = Maintain.ESG_CARBON_EMISSION_FACTORY(-1, "臺北自來水(2020)");
			var 查詢byEMISSION_SOURCE_1 = Maintain.ESG_CARBON_EMISSION_FACTORY(-1, "臺北自來水(2020)--");
		}



		[TestMethod]
		public void t_ESG_CARBON_EMISSION_CooyItem()
		{
			//var 新增 = Maintain.ESG_CARBON_EMISSION_FACTORY(null);
			//var 查詢bySID = Maintain.ESG_CARBON_EMISSION_FACTORY(82);
			//var 查詢byEMISSION_SOURCE = Maintain.ESG_CARBON_EMISSION_FACTORY(-1, "臺北自來水(2020)");
			//var 查詢byEMISSION_SOURCE_1 = Maintain.ESG_CARBON_EMISSION_FACTORY(-1, "臺北自來水(2020)--");

			var CooyItem_己存在 = Maintain.ESG_CARBON_EMISSION_CooyItem(2225);
			var CooyItem_未存在  = Maintain.ESG_CARBON_EMISSION_CooyItem(2111);


		}

		public class x_ESG_CARBON_EMISSION_FACTORY
		{
			public int EMISSION_SID { get; set; }
			public string EMISSION_SOURCE { get; set; }
			public decimal? CARBON_EMISSION { get; set; }
			public string CARBON_EMISSION_UNIT { get; set; }
			public string CREATE_USER { get; set; }
			public DateTime CREATE_DATE { get; set; }
			public string UPDATE_USER { get; set; }
			public DateTime UPDATE_DATE { get; set; }
			public string PARAMETER_NO { get; set; }
			public string PARAMETER_NAME { get; set; }
		}

		[TestMethod]
		public void xx_view_自廠係數()
		=> _DBTest((Txn) =>
		{
			var _repo = new
			{
				ESG_CARBON_EMISSION_FACTORY = Txn.EFQuery<ESG_CARBON_EMISSION_FACTORY>(),
				AD_PARAMETER = Txn.EFQuery<AD_PARAMETER>(),
			};
		var z = (from a0 in _repo.ESG_CARBON_EMISSION_FACTORY.Reads()
				 join a1 in _repo.AD_PARAMETER.Reads(c => c.PARAMETER_VALUE == "ESG" && c.PARAMETER_TYPE == "SystemCode")
					 on a0.EMISSION_SID equals a1.ATTRIBUTE_11 into parameterGroup
				 from a2 in parameterGroup.DefaultIfEmpty()
				 select new ESG_CARBON_EMISSION_FACTORY
				 {
					 EMISSION_SID = a0.EMISSION_SID,
					 EMISSION_SOURCE = a0.EMISSION_SOURCE,
					 CARBON_EMISSION = a0.CARBON_EMISSION,
					 CARBON_EMISSION_UNIT = a0.CARBON_EMISSION_UNIT,
					 CREATE_USER = a0.CREATE_USER,
					 CREATE_DATE = a0.CREATE_DATE,
					 UPDATE_USER = a0.UPDATE_USER,
					 UPDATE_DATE = a0.UPDATE_DATE,
					 //PARAMETER_NO = a2 == null ? null : a2.PARAMETER_NO,
					 //PARAMETER_NAME = a2 == null ? null : a2.PARAMETER_NAME
				 })
				 //select new { a0,a2 })

				 ;
 
				//   ;


            //var r = new Genesis.Library.BLL.ESG.Maintain().view_自廠係數(txn).ToList();
            var r = z.ToList();

		});



		/// <summary>
		/// 使用 join()  但沒作用
		/// </summary>
		[TestMethod]
		public void xx_view_自廠係數1()
		=> _DBTest((Txn) =>
		{
			var _repo = new
			{
				ESG_CARBON_EMISSION_FACTORY = Txn.EFQuery<ESG_CARBON_EMISSION_FACTORY>(),
				AD_PARAMETER = Txn.EFQuery<AD_PARAMETER>(),
			};
			var z = _repo.ESG_CARBON_EMISSION_FACTORY.Reads()
					.Join(_repo.AD_PARAMETER.Reads(c => c.PARAMETER_VALUE == "ESG" && c.PARAMETER_TYPE == "SystemCode")
						, a => a.EMISSION_SID
						, b => b.ATTRIBUTE_11
						, (a, b) => new ESG_CARBON_EMISSION_FACTORY
						{
							EMISSION_SID = a.EMISSION_SID,
							EMISSION_SOURCE = a.EMISSION_SOURCE,
							CARBON_EMISSION = a.CARBON_EMISSION,
							CARBON_EMISSION_UNIT = a.CARBON_EMISSION_UNIT,
							CREATE_USER = a.CREATE_USER,
							CREATE_DATE = a.CREATE_DATE,
							UPDATE_USER = a.UPDATE_USER,
							UPDATE_DATE = a.UPDATE_DATE,
							//PARAMETER_NO = b.PARAMETER_NO,
							//PARAMETER_NAME = b.PARAMETER_NAME
						});
						 


					//var r = new Genesis.Library.BLL.ESG.Maintain().view_自廠係數(txn).ToList();
					var r = z.ToList();

			});


		/// <summary>
		/// 使用 join()  但沒作用
		/// </summary>
		[TestMethod]
		public void xx_view_自廠係數2()
		=> _DBTest((Txn) =>
		{
			var _repo = new
			{
				WP_WO_HIST = Txn.EFQuery<WP_WO_HIST>(),
				AD_USER = Txn.EFQuery<AD_USER>(),
			};
			var queryable = from F in _repo.WP_WO_HIST.Reads()
							join E in _repo.AD_USER.Reads() 
								on F.CREATE_USER equals E.ACCOUNT_NO into group1
							from g1 in group1.DefaultIfEmpty()
							select new WP_WO_HIST_V
							{
								ACTION = F.ACTION,
								APPLICATION_NAME = F.APPLICATION_NAME,
								OLD_STATUS = F.OLD_STATUS,
								NEW_STATUS = F.NEW_STATUS,
								OLD_QUANTITY = F.OLD_QUANTITY,
								NEW_QUANTITY = F.NEW_QUANTITY,
								OLD_UNIT = F.OLD_UNIT,
								NEW_UNIT = F.NEW_UNIT,
								CREATE_USER = F.CREATE_USER,
								CREATE_DATE = F.CREATE_DATE,
								USER_NAME = g1.USER_NAME ?? ""
							};


			//var r = new Genesis.Library.BLL.ESG.Maintain().view_自廠係數(txn).ToList();
			var r = queryable.ToList();

		});


		[TestMethod]
		public void t_view_自廠係數()
		=> _DBTest((Txn) =>
		{
			var r = Genesis.Library.BLL.ESG.Maintain.view_自廠係數(Txn).ToList();
	 

		});

		struct d_x {
			public ESG_RELATION_GATEWAY form;
			public List<ESG_RELATION_GATEWAY_EQP> list_EQP;
		}

		[TestMethod]
		public void t_ESG_RELATION_GATEWAY_Save()
		{
			var _r = FileApp.Read_SerializeJson<d_x>(_log.ESG_RELATION_GATEWAY_Save);
			Maintain.ESG_RELATION_GATEWAY_Save(_r.form, _r.list_EQP, true);
		}

		struct d_x1
		{
			public Dictionary<string,string[]> line;
			public List<d_x1_z> gatewaies;
		}

		struct d_x1_z{
			public string GATEWAY_SID;
			public string WO_LINE_NO;

			public string GATEWAY_USED;

			public string GATEWAY_USED_LINE;
        }


	[TestMethod]
		public void t_tt()
		=> _DBTest((Txn) =>
		{
			var _r = FileApp.Read_SerializeJson<d_x1>(_log.tt);

			var _r1 = _r.gatewaies.GroupBy(item => item.WO_LINE_NO)
						.ToDictionary(
							group => group.Key,
							group => group.Select(item => item.GATEWAY_SID).ToList());

			var gatewayIds = _r1["ESG_DEMO"];
			var _repo = new
			{
				//ZZ_BLUTECH_METER_ANT = Txn.EFQuery<ZZ_BLUTECH_METER_ANT>()
				ZZ_BLUTECH_METER_ANT = Txn.EFQuery<ZZ_BLUTECH_METER_ANT_tmp>()
			};

			//       var z = App.Timer(() =>
			//       {
			//           var x1 = _repo.ZZ_BLUTECH_METER_ANT
			//               .Reads(x => x.CREATE_DATE.Year == 2023 && gatewayIds.Contains(x.GATEWAY_ID))
			//               //.Reads(x => x.year == 2023)
			//               //.OrderBy(x => x.CREATE_DATE)
			//               .GroupBy(x => new { x.GATEWAY_ID, x.month, x.day })
			//               .Select(x => new
			//{
			//	GatewayID = x.Key.GATEWAY_ID,
			//	Month = x.Key.month,
			//	Day = x.Key.day,
			//	TotalKWH = x.Sum(entry => entry.KWH_TTL)
			//})
			//               //.Where(x => gatewayIds.Contains(x.GATEWAY_ID))
			//               ;
			//           return x1.ToList();
			//       });
			decimal? ATTRIBUTE_11 = 0;
			decimal? ATTRIBUTE_12 = 1;

			var z = App.Timer(() =>
			{
				var q1 = (from item in _repo.ZZ_BLUTECH_METER_ANT.Reads()
						  where item.year == 2023 && gatewayIds.Contains(item.GATEWAY_ID)
						  group item by new { item.GATEWAY_ID, item.month, item.day, item.DEVICE_ADDRESS } into grouped
						  select new
						  {
							  grouped.Key.GATEWAY_ID,
							  grouped.Key.month,
							  grouped.Key.day,
							  KWH_TTL_Min = grouped.Min(g => g.KWH_TTL),
							  KWH_TTL_Max = grouped.Max(g => g.KWH_TTL)
						  });
				var q2 = (from a in q1
						  group a by new { a.GATEWAY_ID, a.month, a.day } into finalGroup
						  select new
						  {
							  GATEWAY_ID = finalGroup.Key.GATEWAY_ID,
							  Month = finalGroup.Key.month,
							  Day = finalGroup.Key.day,
							  KWH_TTL = finalGroup.Sum(a => a.KWH_TTL_Max) - finalGroup.Sum(a => a.KWH_TTL_Min)
						  }) 
					.ToDictionary(x => new { x.GATEWAY_ID, x.Month, x.Day }, x => {
						// 將數值乘以倍數變成字典.
						decimal kwh = (decimal)((x.KWH_TTL + ATTRIBUTE_11 ?? 0) * ATTRIBUTE_12 ?? 1);
						// 除以共用數量做分攤計算.
						decimal result = kwh;// / (decimal)(lineGateways.FirstOrDefault(y => y.GATEWAY_SID == x.GATEWAY_ID)?.GATEWAY_USED ?? 1);
						return result;
					})
					//.Where(x => gatewayIds.Contains(x.GATEWAY_ID))
					;
				return q2;
			});
			//var z = App.Timer(() =>
			//{
			//	var q1 = (from item in _repo.ZZ_BLUTECH_METER_ANT.Reads()
			//			  where item.year == 2023 && gatewayIds.Contains(item.GATEWAY_ID)
			//			  group item by new { item.GATEWAY_ID, item.month, item.day, item.DEVICE_ADDRESS } into grouped
			//			  select new
			//			  {
			//				  grouped.Key.GATEWAY_ID,
			//				  grouped.Key.month,
			//				  grouped.Key.day,
			//				  KWH_TTL_Min = grouped.Min(g => g.KWH_TTL),
			//				  KWH_TTL_Max = grouped.Max(g => g.KWH_TTL)
			//			  });

			//	var x1 = _repo.ZZ_BLUTECH_METER_ANT
			//		.Reads(x => x.CREATE_DATE.Year == 2023 && gatewayIds.Contains(x.GATEWAY_ID))
			//		//.Reads(x => x.year == 2023)
			//		//.OrderBy(x => x.CREATE_DATE)
			//		.GroupBy(x => new { x.GATEWAY_ID, x.CREATE_DATE.Month, x.CREATE_DATE.Day })
   //                 .Select(x => new
   //                 {
   //                     GATEWAY_ID = x.Key.GATEWAY_ID,
   //                     Month = x.Key.Month,
   //                     Day = x.Key.Day,
   //                     KWH_S = x.OrderBy(t=>t.CREATE_DATE).Select(entry => entry.KWH_TTL).FirstOrDefault(),

			//			KWH_E = x.OrderByDescending(t => t.CREATE_DATE).Select(entry => entry.KWH_TTL).FirstOrDefault(),
			//		})
   //                 .ToDictionary(x => new { x.GATEWAY_ID, x.Month, x.Day }, x => {
			//			// 將數值乘以倍數變成字典.
			//			decimal kwh = (decimal)(((x?.KWH_E ?? 0 - x?.KWH_S ?? 0) + ATTRIBUTE_11 ?? 0) * ATTRIBUTE_12 ?? 1);
			//			// 除以共用數量做分攤計算.
			//			decimal result = kwh;// / (decimal)(lineGateways.FirstOrDefault(y => y.GATEWAY_SID == x.GATEWAY_ID)?.GATEWAY_USED ?? 1);
			//			return result;
			//		})
			//		//.Where(x => gatewayIds.Contains(x.GATEWAY_ID))
			//		;
			//	return x1.ToList();
			//});

			//         var z1 = App.Timer(() => {
			//	var x1 = _repo.ZZ_BLUTECH_METER_ANT
			//		//.Reads(x => x.CREATE_DATE.Year == 2023 && gatewayIds.Contains(x.GATEWAY_ID))
			//		.Reads(x => x.year == 2023 && gatewayIds.Contains(x.GATEWAY_ID))
			//		//.OrderBy(x => x.CREATE_DATE)
			//		.Select(x => new { x.GATEWAY_ID, x.month, x.day })
			//		;

			//	var result = from entry2 in x1
			//				 join entry1 in _repo.ZZ_BLUTECH_METER_ANT.Reads()
			//					 on new { entry2.GATEWAY_ID, entry2.month, entry2.day } equals new { entry1.GATEWAY_ID, entry1.month, entry1.day }
			//				into joined
			//				 from entry1 in joined.DefaultIfEmpty()
			//				 select new
			//				 {
			//					 entry2.GATEWAY_ID,
			//					 entry2.month,
			//					 entry2.day,
			//					 C1 = 1,
			//					 //entry1.GATEWAY_ID,
			//					 entry1.KWH_TTL,
			//					 entry1.year,
			//					 //entry1.month1,
			//					 //entry1.day1,
			//					 //C2 = entry1.year == null ? (int?)null : 1
			//				 }
			//		;
			//	return result.ToList();
			//});

			return;
		});

	}
}

