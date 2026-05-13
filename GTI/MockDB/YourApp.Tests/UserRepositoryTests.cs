using Microsoft.VisualStudio.TestTools.UnitTesting;
using YourApp.Db;
using md = YourApp.Entities;
using YourApp.Repository;
using System.Linq;
using System.Linq.Dynamic;

namespace YourApp.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        [TestMethod]
        public void GetAll_Should_Return_All_Users()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();

            using (var context = new AppDbContext(connection))
            {
                context.Users.Add(new md.User { Name = "Test" });
                context.SaveChanges();

                //var repo = new UserRepository(context);

                var result = context.Users.Where(c=>c.Name == "Test").ToList();

                Assert.AreEqual(1, result.Count);
            }
        }
    }
}
