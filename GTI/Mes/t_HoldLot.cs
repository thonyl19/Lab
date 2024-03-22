using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Genesis.Areas.WIP.Controllers;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Dynamic;
using System.Web.Mvc;
using UnitTestProject.TestUT;



namespace UnitTestProject
{

	public class t1
	{
		public string _public { get; set; }
		private string _private { get; set; }
		internal string _internal { get; set; }
		protected string _protected { get; set; }
		private protected string _pprotected { get; set; }
	}

	public static class t2
	{
	}
	//public static class x
	//{
	//	public static T TransTo<T>(this object src) where T : new()
	//	{
	//		Type _src = src.GetType();
	//		T targetObj = new T();
	//		Type _target = targetObj.GetType();

	//		//PropertyInfo[] _sourcProp =
	//		Dictionary<string, object> _srcDC
	//				= _src
	//				.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
	//				.ToLookup(t => t.Name, t => t)
	//				.ToDictionary(p => p.Key, p =>
	//				{

	//					try
	//					{
	//						return p.First().GetValue(src);
	//					}
	//					catch (Exception Ex)
	//					{
	//						var ex = Ex;
	//					}
	//					return null;
	//				});

	//		PropertyInfo[] _targetFileds
	//				= _target
	//				.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	//		Parallel.ForEach(_targetFileds, (field) =>
	//		//foreach (PropertyInfo field in _targetFileds)
	//		{
	//			object _val = null;
	//			if (_srcDC.TryGetValue(field.Name, out _val))
	//			{
	//				try
	//				{
	//					field.SetValue(targetObj, _val);
	//				}
	//				catch
	//				{
	//				}
	//			}
	//		});
	//		return targetObj;
	//		//new { _srcDC, targetObj };
	//	}



	//	/// <summary>
	//	/// 因應 2015 版的處理需求,需要將以下欄位 Operation ,Product ,Part 的欄位呈現 [No]~Name 格式
	//	/// </summary>
	//	/// <param name="Lot"></param>
	//	/// <param name="dbc"></param>
	//	/// <returns></returns>
	//	public static MDL.MES.WP_LOT TransDetail(this LotUtility.LotInfo Lot, DBController dbc)
	//	{
	//		var _lot = Lot.TransTo<MDL.MES.WP_LOT>();
	//		//using (var dbc = this.DBC) { 
	//		var oper = new OperationUtility.OperationInfo(dbc, Lot.OPER_SID, OperationUtility.IndexType.SID);
	//		var pro = new ProductUtility.ProductInfo(dbc, Lot.PRODUCT_SID, ProductUtility.IndexType.SID);
	//		var part = new PartUtility.PartInfo(dbc, Lot.PARTNO, PartUtility.IndexType.No);
	//		_lot.PARTNO = $"[{part.No}]~{part.Name}";
	//		_lot.PRODUCT = $"[{pro.No}]~{pro.Name}";
	//		_lot.OPERATION = $"[{oper.No}]~{oper.Name}";
	//		//}
	//		return _lot;
	//	}
	//}

	[TestClass]
	public class t_HoldLot
	{
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
		public void t_GetFromHoldLotController()
		{
			string LotNo = "201-20121129-36";
			var _ctl = new HoldLotController();
			//var result = _ctl.GetLotInfo(LotNo);
			//var _json = ((ContentResult)result).Content;

			//new FileApp().Write(_json, FileApp.ts_Log(@"HoldLot\LotInfo_t.json"));
		}


		[TestMethod]
		public void t_GetFromHoldLotController_1()
		{
			string LotNo = "201-20121129-34";

			var LotInfo = new LotUtility.LotInfo(this.DBC, LotNo, LotUtility.IndexType.NO);

			var _r = LotInfo.TransTo<MDL.MES.WP_LOT>();


			new FileApp().Write(_r.ToJson(), FileApp.ts_Log(@"HoldLot\TransTo_t.json"));

		}



		[TestMethod]
		public void t_Hold_LotInfo()
		{
			string LotNo = "201-20121129-37";
			var _chk = new chk_Lot(LotNo);

			dynamic result = new ExpandoObject();

			using (var dbc = mes.dbc())
			{
				result = _chk.LotInfo.ReBinDBC(dbc).GetLotHoldInfoList();

			}
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"HoldLot\t_Hold_LotInfo.json"));

		}

		[TestMethod]
		public void t_chk_Lot()
		{
			string LotNo = "201-20121129-34";
			var _svc = new OffLineLotServices();
			var result = chk_Lot.Condition_HoldLot(LotNo);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"HoldLot\LotInfo_t.json"));

		}

		[TestMethod]
		public void t_DoHoldLot()
		{
			string LotNo = "201-20121129-36";
			var _svc = new OffLineLotServices();
			var result = _svc.DoHoldLot(LotNo, "1", "", true);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"HoldLot\LotInfo_t.json"));

		}


		[TestMethod]
		public void t_DoHoldLot_1()
		{
			string LotNo = "201-20121129-39";
			var _svc = new OffLineLotServices();
			var obj = new HoldLotForm()
			{
				Reason = new string[] { "test" },
				Description = "",
				//HoldLotList = new System.Collections.Generic.List<HoldLotInfo>()
			};
			//obj.HoldLotList.Add(new HoldLotInfo() { Lot = LotNo });

			var result = _svc.DoHoldLotMulti(obj, false, true);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"HoldLot\t_DoHoldLot_1.json"));

		}

		[TestMethod]
		public void t_DoHoldLotMulti()
		{
			string LotNo = "201-20121129-38";
			var _svc = new OffLineLotServices();
			var obj = new HoldLotForm()
			{
				Reason = new string[] { "test" },
				Description = "",
				//HoldLotList = new System.Collections.Generic.List<HoldLotInfo>()
			};
			//obj.HoldLotList.Add(new HoldLotInfo() { Lot = LotNo });
			//obj.HoldLotList.Add(new HoldLotInfo() { Lot = "201-20121129-39" });

			var result = _svc.DoHoldLotMulti(obj, true, true);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"HoldLot\t_DoHoldLotMulti.json"));

		}


		[TestMethod]
		public void t_getFromWIPServices()
		{
			string LotNo = "201-20121129-34";
			var wipServices = new BLL.MES.WIPServices();
			var data = wipServices.OffLineLotInfo(LotNo);
			LotUtility.LotInfo LotInfo = data.CurrentLot;

			FileApp.WriteSerializeJson(LotInfo, FileApp.ts_Log(@"HoldLot\LotInfo_t.json"));

		}

		[TestMethod]
		public void t_()
		{
			string LotNo = "201-20121129-34";
			var wipServices = new BLL.MES.WIPServices();
			var data = wipServices.OffLineLotInfo(LotNo);
			LotUtility.LotInfo LotInfo = data.CurrentLot;
			//var _r = LotInfo.chk_Status_Is(LotStatus.Create, LotStatus.Hold)();
			//LotUtility.LotInfo LotInfo
			//	= FileApp.Read_SerializeJson<LotUtility.LotInfo>(FileApp.ts_Log(@"HoldLot\LotInfo_t.json"));

		}

	}
}
