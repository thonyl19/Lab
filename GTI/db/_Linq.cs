using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	[TestClass]
	public class t_Linq : _testBase
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


		[TestMethod]
		public void t_將文字型數字做排序()
		{
			//請教一下 以下程式語法的功能 是? 
			var Cq_items = new string[] { "1", "11", "2" };
			var z = Cq_items
				.OrderBy(x =>
				{
					return x.Length;
				})
				.ThenBy(x => x).ToList();

		}

		[TestMethod]
		public void t_Equals()
		{
			string root = @"C:\users";
			string root2 = @"C:\Users";
			var t = new
			{
				T1 = root == root2,
				T2 = root.Equals(root2),
				T3 = root.Equals(root2, StringComparison.OrdinalIgnoreCase),
			};
		}



		[TestMethod]
		public void t_Compile()
		{
			// 定義一個參數 (int x)
			ParameterExpression x = Expression.Parameter(typeof(int), "x");

			// 定義一個常數 (int 5)
			ConstantExpression five = Expression.Constant(5);

			// 建立一個二元運算式 (x + 5)
			BinaryExpression add = Expression.Add(x, five);

			// 建立一個 Lambda 運算式 (int x) => x + 5
			Expression<Func<int, int>> lambdaExpression = Expression.Lambda<Func<int, int>>(add, x);

			// 編譯 Lambda 運算式以產生委派
			Func<int, int> compiledDelegate = lambdaExpression.Compile();

			// 呼叫編譯後的委派
			int result = compiledDelegate(10);
			Console.WriteLine($"結果: {result}"); // 輸出: 結果: 15
		}

		[TestMethod]
		public void t_Compile_1()
		{
			List<User> users = new List<User>
		{
			new User { Age = 25, City = "Taipei" },
			new User { Age = 30, City = "Kaohsiung" },
			new User { Age = 25, City = "Kaohsiung" }
		};

			// 動態建立篩選條件：Age == 25
			Func<User, bool> ageFilter = DynamicQuery.CreateFilter("Age", 25);
			var youngUsers = users.Where(ageFilter);
			Console.WriteLine("年齡為 25 的使用者:");
			foreach (var user in youngUsers)
			{
				Console.WriteLine($"  年齡: {user.Age}, 城市: {user.City}");
			}

			// 動態建立篩選條件：City == "Kaohsiung"
			Func<User, bool> cityFilter = DynamicQuery.CreateFilter("City", "Kaohsiung");
			var kaohsiungUsers = users.Where(cityFilter);
			Console.WriteLine("\n居住在 Kaohsiung 的使用者:");
			foreach (var user in kaohsiungUsers)
			{
				Console.WriteLine($"  年齡: {user.Age}, 城市: {user.City}");
			}
		}

	}
}
 
public class User
{
	public int Age { get; set; }
	public string City { get; set; }
}

public class DynamicQuery
{
	public static Func<User, bool> CreateFilter(string propertyName, object value)
	{
		ParameterExpression userParam = Expression.Parameter(typeof(User), "user");
		MemberExpression propertyAccess = Expression.Property(userParam, propertyName);
		ConstantExpression valueConst = Expression.Constant(value);
		BinaryExpression equality = Expression.Equal(propertyAccess, valueConst);
		Expression<Func<User, bool>> lambda = Expression.Lambda<Func<User, bool>>(equality, userParam);
		return lambda.Compile();
	}
 
}
