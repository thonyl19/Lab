using Dal.Repository;
using Frame.Code;
using Frame.Code.Web.Select;
using MDL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject.TestUT;
using mdl = MDL.MES;

namespace UnitTestProject
{
	[TestClass]
	public class t_Entity : _testBase
	{

		MESContext _dbContext;
		MESContext dbContext
		{
			get
			{
				if (_dbContext == null)
				{
					this._dbContext = new MESContext();
				}
				return this._dbContext;
			}
		}

		[TestMethod]
		public void t_基本連接測試()
		{
			//在站中批號
			var _lotServices = new EFRepository<mdl.WP_LOT>(dbContext);
			var lotExpression = ExtLinq.True<mdl.WP_LOT>();
			lotExpression = lotExpression.And(t => t.STATUS == "Run");
			var x = _lotServices.Reads(lotExpression).Count();
			new FileApp().Write_SerializeJson(x, FileApp.ts_Log(@"Entity\t_WOInfo.json"));

		}

		[TestMethod]
		public void t_基本連接測試1()
		{
			var queryGroup = from resList in dbContext.PF_REASONGROUP_FUN_LIST.Where(i => i.FUN_SID == "%")
							 join resListDetail in dbContext.PF_REASONGROUP on resList.REASONGROUP_SID equals resListDetail.REASONGROUP_SID
							 select new SelectModel
							 {
								 SID = resListDetail.REASONGROUP_SID,
								 No = resListDetail.REASONGROUP_NO,
								 Display = resListDetail.REASONGROUP,
								 Value = resListDetail.REASONGROUP_SID
							 };
			var retModel = queryGroup.Distinct().ToList();
			new FileApp().Write_SerializeJson(retModel, FileApp.ts_Log(@"Entity\t_基本連接測試1.json"));


		}

		/// <summary>
		/// $("#btnSave").on("click", function () {
		/// C:\Code\Genesis-MVC5\Genesis_MVC\Areas\SYSAdmin\Views\User\Form.cshtml
		/// 後端 - SubmitData
		/// C:\Code\Genesis-MVC5\Genesis_MVC\Areas\SYSAdmin\Controllers\UserController.cs
		/// 
		/// </summary>
		[TestMethod]
		public void t_頭身寫檔範例()
		{
			/*
			Updata_QC_INSP
			*/
		}



	}
}
