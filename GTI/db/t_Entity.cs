using BLL.DataViews.Res;
using BLL.InterFace;
using Dal.Repository;
using Frame.Code;
using Frame.Code.Web.Select;
using MDL;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


		[TestMethod]
		public void t_測試MODEL是否有特定屬性()
		{
			var property = new WP_LOT()
				.GetType()
				.GetProperty("LOT_x");

		}


		[TestMethod]
		public void t_測試MODEL是否有特定屬性1()
		{
			var StaticMethod = "CartonCheckIn";
			MethodInfo methodInfo = typeof(BLL.MES.WIPInfoServices).GetMethod
				(StaticMethod, BindingFlags.Public | BindingFlags.Static);
			if (methodInfo != null)
			{
				var Search = "20230226001-01";
				var isTest = true;
				object[] methodParameters = new object[] { Search , isTest }; // 传递给静态方法的参数
				IResult Result = (IResult)methodInfo.Invoke(null, methodParameters);
				if (Result.Success) {
					var x = Result.parseData<List< d_CartonCheckIn>>();
					var t = x[0].LOT;
				}
			}
		}

		[TestMethod]
		public void t_測試MODEL是否有特定屬性2()
		{
			var z = BLL.MES.WIPInfoServices.CartonCheckIn("20230226001-01",true);
			
		}


 


	[TestMethod]
		public void _DapperQuery()
		=> _DBTest((Txn) =>
		{
			//var _repo = new
			//{
			//	ESG_CARBON_EMISSION_FACTORY = Txn.EFQuery<ESG_CARBON_EMISSION_FACTORY>(),
			//	AD_PARAMETER = Txn.EFQuery<AD_PARAMETER>(),
			//};
			//var query = from a0 in _repo.ESG_CARBON_EMISSION_FACTORY.Reads()
			//			join a1 in _repo.AD_PARAMETER.Reads()//c=>c.PARAMETER_VALUE == "ESG" && c.PARAMETER_TYPE == "SystemCode")
			//				on a0.EMISSION_SID equals a1.ATTRIBUTE_11 into tmpGrp
			//			from a2 in tmpGrp.DefaultIfEmpty()
			//			select new ESG_CARBON_EMISSION_FACTORY
			//			{
			//				//a0.EMISSION_SID,
			//				//a0.EMISSION_SOURCE,
			//				//a0.CARBON_EMISSION,
			//				//a0.CARBON_EMISSION_UNIT,
			//				//a0.CREATE_USER,
			//				//a0.CREATE_DATE,
			//				//a0.UPDATE_USER,
			//				//a0.UPDATE_DATE,
			//				EMISSION_SID = a0.EMISSION_SID,
			//				 EMISSION_SOURCE = a0.EMISSION_SOURCE,
			//				 CARBON_EMISSION = a0.CARBON_EMISSION,
			//				 CARBON_EMISSION_UNIT = a0.CARBON_EMISSION_UNIT,
			//				 CREATE_USER = a0.CREATE_USER,
			//				 CREATE_DATE = a0.CREATE_DATE,
			//				 UPDATE_USER = a0.UPDATE_USER,
			//				 UPDATE_DATE = a0.UPDATE_DATE,
			//				//a1.PARAMETER_NO,
			//				//a1.PARAMETER_NAME,
			//				//PARAMETER_NO = a2 == null ? null : a2.PARAMETER_NO,
			//				//PARAMETER_NAME = a2 == null ? null : a2.PARAMETER_NAME
			//			};

			//var r = query.ToList();
		},false, true);


	}
}
