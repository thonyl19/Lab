using AutoMapper;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Frame.Code.Excel;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction.TOL;
using Genesis.Library.BLL.DTC;
using Genesis.Library.BLL.MES.OperTask;
using Genesis.Library.BLL.WRP;
using Genesis.Library.Frame.Code.Web.TableQuery;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static Genesis.Library.BLL.ADM.BomServices;
using _Prd = Genesis.Library.BLL.MES.OperTask;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;
using BLL.DataViews.Edc;
using static Genesis.Library.BLL.DTC.Lot;
using static Genesis.Gtimes.WIP.LotUtility;
using Genesis.Library.BLL.ADM;
using Genesis.Gtimes.Transaction.WIP;
using System.Data.Entity;
using Genesis.Library.BLL.MES.OperInfo;
using Genesis.Library.BLL.ZZ.CUB.OperTask;
using System.Threading;
using System.Globalization;
//using _bllSvc = Genesis.Library.BLL.ZZ.CUB;

namespace UnitTestProject
{
	/// <summary>
    /// </summary>
    [TestClass]
	public class t_ZZ_CUB : _testBase
	{
		static class _log
		{
			internal static string t_ParallelOper
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\CUB\t_ParallelOper.json");
				}
			}
			internal static string t_Base
			{
				get
				{
					return FileApp.ts_Log(@"ZZ\CUB\t_Base.json");
				}
			}
			internal static string ParallelCheckIn
			{
				get
				{
					return FileApp.ts_Log(@"ZZ/CUB\ParallelCheckIn.json");
				}
			}




		}

 

		[TestMethod]
		public void t_Base()
		{
			var t = new Basic("EMS20240614-001-02");
			t.isTest = true;
			t.Process();
			FileApp.WriteSerializeJson(t, _log.t_Base);
		}

		[TestMethod]
		public void t_ParallelOper()
		{
			var t = new ParallelOper("Jtest0717-01", "GTI24071610203627082");
			t.isTest = true;
			t.Process();

			//var t1 = new ParallelOper("JTest0717_10-01", LotStatus.Wait, "GTI24071610203627083");
			//t1.isTest = true;
			//t1.Process();

			//FileApp.WriteSerializeJson(t, _log.t_ParallelOper);
		}

		[TestMethod]
		public void t_ParallelCheckIn() {
			TxnBase.Test = Genesis.GTI_Test.TxnBase_T;
			var _r = FileApp.Read_SerializeJson<WIPFormSendParameter>(_log.ParallelCheckIn);
			new ParallelCheckIn().Process(_r,true);
		}
	}


    
}

