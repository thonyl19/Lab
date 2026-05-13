using Genesis.Library.BLL.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using UnitTestProject.OO_MES;

namespace UnitTestProject
{
 


	[TestClass]
	public class t_OO_MES
	{

        [TestMethod]
        public void t_批號加帳(){
			var t = new Lot_DB("1234");
			t.AddQty(10, "TEst");
		}

		[TestMethod]
		public void t_進站流程()
		{
			var context = new ProcessContext
			{
				Station = "ST10",
				UserNo = "Anthony"
			};

			context.LotNos.Add("LOT001");
			context.LotNos.Add("LOT002");

			IProcessFlow process = new InStationProcess();
			process.Execute(context);
		}

		[TestMethod]
		public void t_進站流程_DB()
		{
			var context = new ProcessContext
			{
				Station = "ST10",
				UserNo = "Anthony"
			};

			context.LotNos.Add("LOT001");
			context.LotNos.Add("LOT002");

			var factory = new ProcessFlowFactory();
			var processFlow = factory.Create("InStation");
			processFlow.Execute(context);
		}

	}
}
