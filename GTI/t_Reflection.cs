using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.RouteUtility;

namespace UnitTestProject
{
	class ClassSample
	{
		public void test1(string para1)
		{
			Console.WriteLine("方式1 {0}________test111", para1);
		}

		public void test1(string para1, string para2)
		{
			Console.WriteLine("方式2 {0}________test111________{1}", para1, para2);
		}

		public void test2(string para1, string para2)
		{
			Console.WriteLine("方式3 {0}________test222________{1}", para1, para2);
		}
	}

	[TestClass]
	public class t_Reflection : _testBase
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
		}

		/// <summary>
		/// 使用反射來取得已載入組件的完整名稱
		/// https://docs.microsoft.com/zh-cn/previous-versions/ms173183(v=vs.80)?redirectedfrom=MSDN
		/// </summary>
		[TestMethod]
		public void t_Assembly_Load()
		{

			System.Reflection.Assembly o = System.Reflection.Assembly.Load("mscorlib.dll");
			System.Console.WriteLine(o.GetName());
		}

		[TestMethod]
		public void t_1()
		{
			//取得類別型別
			//法一
			Type _type = typeof(RouteVersionOperationInfo);
			var _r
				//= _type.GetMethods();
				= _type.GetMethod("GetRouteInfo");

		}

		/// <summary>
		/// 
		/// [Ref]https://blog.csdn.net/smartsmile2012/article/details/79551176
		/// </summary>
		[TestMethod]
		public void t_2()
		{
			//反射获取 命名空间+类名
			string className = "UnitTestProject.ClassSample";
			string methodName = "test1";
			//传递参数
			Object[] paras = new Object[] { "我的", "电脑" };
			var t = Type.GetType(className);
			object obj = Activator.CreateInstance(t);

			try
			{
				#region 方法一
				//直接调用
				MethodInfo method = t.GetMethod("test2");
				method.Invoke(obj, paras);
				#endregion

				#region 方法二
				MethodInfo[] info = t.GetMethods();
				for (int i = 0; i < info.Length; i++)
				{
					var md = info[i];
					//方法名
					string mothodName = md.Name;
					//参数集合
					ParameterInfo[] paramInfos = md.GetParameters();
					//方法名相同且参数个数一样
					if (mothodName == methodName && paramInfos.Length == paras.Length)
					{
						md.Invoke(obj, paras);
					}
				}
				#endregion
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		[TestMethod]
		public void t_處理動態欄位新增和合併() {
			var a = new { A = "A" };
			dynamic zzA = zz(a);
			zzA.X = "A";
			zzA.Y = "Y";
		}


		public ExpandoObject zz(object x) {
			//dynamic zz1 = new ExpandoObject();
			//var booDict = new IDictionary<string, object>;
			var expando = new ExpandoObject();
			var dictionary = (IDictionary<string, object>)expando;

			foreach (var property in x.GetType().GetProperties())
				dictionary.Add(property.Name, property.GetValue(x));
			return expando;
		}

	}


}
