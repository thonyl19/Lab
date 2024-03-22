using Autofac;
using Autofac.Extras.Moq;
using BLL.Base;
using BLL.Base.Wrapper;
using BLL.DataImport.Rule;
using BLL.DataViews.Res;
using Dal.Repository;
using Genesis.Library.BLL.Base;
using Genesis.Library.BLL.WRP;
using MDL;
using MDL.MES;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using UnitTestProject.db;
using static Genesis.Library.BLL.Base.BulkHelper;

namespace GTiMES5.NUnit.Test
{
    class UnitTestEdc
    {
        private void Register(ContainerBuilder c)
        {
            //c.RegisterInstance<MockMES>(new MockMES("Data Source=10.96.0.217;Initial Catalog=SS_GTIMES5;Persist Security Info=True;User ID=GTIMES;Password=GTIMES;MultipleActiveResultSets=True"))
            // .AsSelf().As<DbContext>();
            var cfg = ConfigurationManager.ConnectionStrings["sql.mes"].ToString();
            c.RegisterInstance<MockMES>(new MockMES(cfg))
             .AsSelf().As<DbContext>();
            c.Register<EFUnitOfWork>(c1 => new EFUnitOfWork(c1.Resolve<MockMES>())).AsImplementedInterfaces();

            Assembly assembly = Assembly.GetAssembly(typeof(AbsDataImportProcess));
            c.RegisterAssemblyTypes(assembly).As<AbsDataImportProcess>().PropertiesAutowired();

            c.RegisterType<DataImportFactory>();

            assembly = Assembly.GetAssembly(typeof(ServicesBase));
            c.RegisterAssemblyTypes(assembly).AsSelf().AsImplementedInterfaces().PropertiesAutowired();
            c.Register<List<AbsDataImportProcess>>(c1 =>
            {
                return c1.Resolve<IEnumerable<AbsDataImportProcess>>().ToList();
            });


            CurrentLoginUserModel mockUser = new CurrentLoginUserModel() { UserName = "admin_test_mock", UserNo = "admin_test_mock" };
            #region mock behavior
            Mock<INUnitServiceBase> mockA = new Mock<INUnitServiceBase>();
            mockA.Setup(x => x.GetLoginUser()).Returns(mockUser);
            mockA.Setup(x => x.GetSysDBTime()).Returns(DateTime.Now);
            #endregion

            c.RegisterMock(mockA);
        }

        [Test]
        public void TestBatch()
        {
            using (var mock = AutoMock.GetLoose(Register))
            {
                var process = mock.Create<UserProcess>();
                process.setDBList();
                process.delete();
            }
        }

        [Test]
        public void TestConvertEntity2Sql()
        {
            string sql = BulkHelper.ConvertEntity2Sql(typeof(AD_IMPORT_DELETE_DATA), "System.Data.SqlClient", SQLAction.INSERT);
            Console.WriteLine(sql);

            sql = BulkHelper.ConvertEntity2Sql(typeof(AD_IMPORT_DELETE_DATA), "System.Data.SqlClient", SQLAction.UPDATE);
            Console.WriteLine(sql);

            sql = BulkHelper.ConvertEntity2Sql(typeof(AD_IMPORT_DELETE_DATA), "System.Data.SqlClient", SQLAction.DELETE);
            Console.WriteLine(sql);
        }

        //[Test]
        //public void TestConvetExl2DataTable()
        //{
        //    EDAServices serv = new EDAServices();
        //    dynamic d = serv.ConvertExl2Struct(new string[] { "生產者", "1.对角线差值", "1.铜箔厚度", "1.IMS宽度", "1.IMS厚度", "1.IMS长度" });
        //    Console.WriteLine(d);
        //}

        [Test]
        [TestCase(1, 0.5)]
        [TestCase(1, 0.49)]
        public void TestDefineLevelStruct(double max, double min)
        {
            var m = typeof(EDAServices).GetMethod("DefineLevelStruct", BindingFlags.NonPublic | BindingFlags.Instance);
            if (m == null)
            {
                Assert.Fail("No found method:DefineLevelStruct!!");
            }

            Dictionary<string, Tuple<double, double>> level = (Dictionary<string, Tuple<double, double>>)m.Invoke(new EDAServices(), new object[] { max, min });

            var answer = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
            CollectionAssert.AreEqual(answer, level.Keys);

            double limit = max;
            double range = (max - min) / 10;
            foreach (var v in level.Values)
            {
                Assert.AreEqual(limit, v.Item1);
                limit = Math.Round(limit - range, 3);
                Assert.AreEqual(limit, v.Item2);
            }
        }
    }
}
