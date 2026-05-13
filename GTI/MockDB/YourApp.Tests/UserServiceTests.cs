using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using md = YourApp.Entities;
using YourApp.Repository;
using YourApp.Service;

namespace YourApp.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public void GetUserName_Should_Return_Name()
        {
            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(r => r.GetUser(1))
                    .Returns(new md.User { Id = 1, Name = "TestUser" });

            var service = new UserService(mockRepo.Object);

            var result = service.GetUserName(1);

            Assert.AreEqual("TestUser", result);
        }
    }
}
