//P:\MyLab\UnitTest\GTI\Mes/MesInOut.cs
//P:\MyLab\UnitTest\GTI\Log/MesInOut/
using BLL.Base;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject.TestUT;
using static BLL.MES.DataViews.PartData;
using static BLL.MES.WIPInjectServices;
using Console = System.Diagnostics.Debug;

namespace UnitTestProject
{
	[TestClass]
	public class t_MesInOut : _testBase
	{
		static class _log
		{
            internal static string t_基礎出站Flow{
				get
				{
					return FileApp.ts_Log(@"MesInOut\t_基礎出站Flow.json");
				}
			}

            /// <summary>
            /// splitBIN 前端傳入的資料範例 
            /// </summary>
            internal static string t_DoRollCheckOut
			{
				get
				{
					return FileApp.ts_Log(@"MesInOut\t_DoRollCheckOut.json");
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
		public void _T01() {
			var r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_DoRollCheckOut);
			new 基礎出站(r,true).Process();
		}


		[TestMethod]
		public void _基礎出站Flow()
		{
			var r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.t_DoRollCheckOut);
			var x = new 基礎出站(r, true, true);
			x.Process();
			FileApp.WriteSerializeJson(x._fun_flow, _log.t_基礎出站Flow);
			//var t = x._fun_flow;
			//Assert.
		}
	}
 
 
}

