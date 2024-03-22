using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	/*
    https://dotblogs.com.tw/kinanson/2017/04/27/083741 
         
    */
	class Order
	{
		public int OrderId { get; set; }
		public string Item { get; set; }
		public int Quantity { get; set; }
		public string OrderName { get; set; }
	}

	[TestClass]
	public class t_BOGUS
	{
		public string _path = $@"\Log\";
		[TestMethod]
		public void t_取得AppConfig中ConnectionStringSettings()
		{
			int orderId = 1;
			string _file = $"{_path}test.json";

			var faker = new Faker<Order>("en")
				.RuleFor(u => u.OrderId, f => f.Random.Number(1, 100))
				.RuleFor(u => u.Item, f => f.Lorem.Sentence())
				.RuleFor(u => u.Quantity, f => f.Random.Number(1, 10))
				.RuleFor(u => u.OrderName, f => f.Commerce.Product());
			var order = faker.Generate();

			FileApp.WriteSerializeJson(order, FileApp.ts_Log("test.json"));

		}




	}
}
