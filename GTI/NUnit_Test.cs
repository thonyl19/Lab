using Frame.Code;
using Genesis.Gtimes.Common;
using Genesis.Library.BLL.Base;
using MDL;
using MDL.MES;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using static Genesis.Library.BLL.Base.BulkHelper;

namespace UnitTestProject
{
    public class NUnit_Test
    {
        private DbContext dbContext;
        private DBController spcDBC;
        [SetUp]
        public void Setup()
        {
            //自建-模擬 web.config 中的 DBC 節點,
            ConnectionStringSettings spcConn = new ConnectionStringSettings("sql.spc",
                "Data Source=10.96.0.229;Initial Catalog=PD_SPC5_DEMO;Persist Security Info=True;User ID=GTIMES;Password=GTIMES;MultipleActiveResultSets=True",
                "System.Data.SqlClient");
            spcDBC = new DBController(spcConn);

            //直接以純 cmd string 方式,建立  dbContext
            dbContext = new MESContext("Data Source=10.96.0.217;Initial Catalog=PD_GTIMES5;Persist Security Info=True;User ID=GTIMES;Password=GTIMES;MultipleActiveResultSets=True");
        }

        [Test]
        public void Test1()
        {
            string sql = "select a,b,c,d,e,f from TEST";
            sql = sql.ToUpper();
            int startIndex = sql.IndexOf("SELECT");
            startIndex += 7;
            int length = sql.IndexOf("FROM") - startIndex;
            Console.WriteLine(sql.Substring(startIndex, length));
        }


        public interface ICalculator
        {
            int Add(int a, int b);
            string Mode { get; set; }
        }

        public interface ICommand
        {
            void Execute();
            event EventHandler Executed;
        }

        public class SomethingThatNeedsACommand
        {
            ICommand command;
            public SomethingThatNeedsACommand(ICommand command)
            {
                this.command = command;
            }
            public void DoSomething() { command.Execute(); }
            public void DontDoAnything() { }
        }



        [Test]
        public void ConvertTwoArrays2List()
        {
            string[] COMMA = new string[] { "," };
            string[] value = "A,B,C,D,E".Split(COMMA, StringSplitOptions.RemoveEmptyEntries);
            string[] text = "1,2,3".Split(COMMA, StringSplitOptions.RemoveEmptyEntries);
            var options = Enumerable.Zip(value, text, (v, t) => new {
                SID = v,
                No = v,
                Value = v,
                Display = t
            });
            Console.WriteLine(options.ToArray());
        }

        [Test]
        public void TestDateTime()
        {
            DateTime d1 = new DateTime(2022, 6, 15, 3, 0, 0);
            DateTime d2 = new DateTime(2022, 6, 15, 5, 30, 0);
            //decimal diff = Math.Round(d3, 1);
            //Console.WriteLine(diff);

        }


        [Test]
        public void TestDynamicGroup()
        {
            List<dynamic> list = new List<dynamic>();
            list.Add(new { name = "A", age = 18 });
            list.Add(new { name = "B", age = 17 });
            list.Add(new { name = "C", age = 16 });
            list.Add(new { name = "A", age = 18 });
            list.Add(new { name = "C", age = 19 });
            list.Add(new { name = "D", age = 17 });

            int count = list.Where(x => x.name == "A").Count();
            Assert.IsTrue(count == 2);
        }

        [Test]
        public void TestLinq()
        {
            List<WP_LOT> lots = new List<WP_LOT>();
            lots.Add(new WP_LOT() { LOT = "A", STATUS = "Run" });
            lots.Add(new WP_LOT() { LOT = "B", STATUS = "Wait" });
            lots.Add(new WP_LOT() { LOT = "C", STATUS = "Finished" });
            lots.Add(new WP_LOT() { LOT = "D", STATUS = "Hold" });
            lots.Add(new WP_LOT() { LOT = "E", STATUS = "Run" });

            var t = from l in lots
                    let status = l.STATUS = "Use"
                    select l.STATUS;

            var count = t.Where(x => x == "Use").Count();
            Assert.IsTrue(count == 5);
        }

        [Test]
        public void MakeGoldenSample()
        {
            MESContext dbContext = new MESContext("Data Source=10.96.0.217;Initial Catalog=PD_GTIMES5;Persist Security Info=True;User ID=GTIMES;Password=GTIMES;MultipleActiveResultSets=True");
            using (dbContext)
            {
                BulkHelper.Bulk<ZZ_GOLDEN_SAMPLE>(new List<ZZ_GOLDEN_SAMPLE>() { new ZZ_GOLDEN_SAMPLE()
                {
                    GOLDEN_SAMPLE_SID = Guid.NewGuid().ToString(),
                    WO = "Golden_Sample_1",
                    LOT = "Golden_Sample_1",
                    CREATE_DATE = DateTime.Now,
                    CREATE_USER = "EAP",
                    UPDATE_DATE = DateTime.Now,
                    UPDATE_USER = "EAP",
                    ENABLE_FLAG = "T",
                    EQP_NO = "RF1",
                    GOLDEN_SAMPLE_NO = "Golden_Sample",
                    QUOTE_ONCE = "T"
                }}, dbContext, SQLAction.INSERT);
            }
        }

        [TestCase(1, 0.5)]
        [TestCase(1, 0.49)]
        public void TestDefineLevelStruct(double max, double min)
        {
            double limit = max;
            double range = (max - min) / 10;
            var arr = new double[] { 100, 200 };
            foreach (var v in arr)
            {
                Assert.AreEqual(limit, v);
                limit = Math.Round(limit - range, 3);
                Assert.AreEqual(limit, v);
            }
        }

        [TestCase(1, 0.49)]
        public void TestDefineLevelStruct1(double[] arr)
        {

        }



        [Test]
        public void MakeFakeData()
        {
            List<ZZ_GOLDEN_SAMPLE_DETAIL> data = new List<ZZ_GOLDEN_SAMPLE_DETAIL>();
            Random random = new Random();
            DateTime startDate = new DateTime(2022, 7, 14);
            for (double i = 1; i <= 14400; i++)
            {
                DateTime date = startDate.AddMinutes(i);
                decimal step = Math.Ceiling((i / 360)).ToDecimal();
                data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                {
                    SID = Guid.NewGuid().ToString(),
                    EQP_NO = "PLC_Beckhoff",
                    WO = "51B-210100055",
                    LOT = "TBA11",
                    CREATE_USER = "EAP",
                    CREATE_DATE = date,
                    PARAMETER = ".Top_Temperature_PV",
                    VALUE = random.Next(0, 2400).ToString(),
                    STEP = step
                });

                data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                {
                    SID = Guid.NewGuid().ToString(),
                    EQP_NO = "PLC_Beckhoff",
                    WO = "51B-210100055",
                    LOT = "TBA11",
                    CREATE_USER = "EAP",
                    CREATE_DATE = date,
                    PARAMETER = ".Power_Output_PV",
                    VALUE = random.Next(0, 600).ToString(),
                    STEP = step
                });

                data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                {
                    SID = Guid.NewGuid().ToString(),
                    EQP_NO = "PLC_Beckhoff",
                    WO = "51B-210100055",
                    LOT = "TBA11",
                    CREATE_USER = "EAP",
                    CREATE_DATE = date,
                    PARAMETER = ".Bottom_Temperature_PV",
                    VALUE = random.Next(0, 1200).ToString(),
                    STEP = step
                });

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".Chamber_Pressure_PV",
                //    VALUE = random.Next(0, 2000).ToString(),
                //    STEP = step
                //});

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".HF_Current_PV",
                //    VALUE = random.Next(0, 2000).ToString(),
                //    STEP = step
                //});

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".DC_Current_PV",
                //    VALUE = random.Next(0, 2000).ToString(),
                //    STEP = step
                //});

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".MFC1_Flow_PV",
                //    VALUE = random.Next(0, 2000).ToString(),
                //    STEP = step
                //});

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".MFC3_Flow_PV",
                //    VALUE = random.Next(0, 2000).ToString(),
                //    STEP = step
                //});

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".MFC4_Flow_PV",
                //    VALUE = random.Next(0, 2000).ToString(),
                //    STEP = step
                //});

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".Coil_Position_PV",
                //    VALUE = random.Next(0, 2000).ToString(),
                //    STEP = step
                //});

                //data.Add(new ZZ_GOLDEN_SAMPLE_DETAIL()
                //{
                //    SID = Guid.NewGuid().ToString(),
                //    EQP_NO = "RF1",
                //    WO = "51B-210100055",
                //    LOT = "TBA11",
                //    CREATE_USER = "EAP",
                //    CREATE_DATE = date,
                //    PARAMETER = ".Rotation_Position_PV",
                //    VALUE = random.Next(0, 200).ToString(),
                //    STEP = step
                //});

                if (data.Count >= 1000)
                {
                    using (MESContext db = new MESContext("Data Source=10.96.0.217;Initial Catalog=PD_GTIMES5;Persist Security Info=True;User ID=GTIMES;Password=GTIMES;MultipleActiveResultSets=True"))
                    {
                        Bulk(data, db, SQLAction.INSERT);
                        data.Clear();
                    }
                }
            }

            using (MESContext db = new MESContext("Data Source=10.96.0.217;Initial Catalog=PD_GTIMES5;Persist Security Info=True;User ID=GTIMES;Password=GTIMES;MultipleActiveResultSets=True"))
            {
                Bulk(data, db, SQLAction.INSERT);
                data.Clear();
            }
        }

    }
}