namespace UnitTestProject
{
    using BLL.DataViews.Res;
    using BLL.MES.DataViews;
    using Frame.Code;
    using Genesis.Library.BLL.MVC.AutoGenerate;
    using MDL;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Reflection;
    using System.Resources;
    using UnitTestProject.TestUT;
    using static Genesis.Library.BLL.MES.DataViews.System;
    using _v8n = BLL.MES.FluentValidation;

    //[assembly:NeutralResourcesLanguage("en")]
    /// <summary>
    /// Defines the <see cref="t_AD" />.
    /// </summary>
    [TestClass]
    public class t_AD : _testBase
    {
        /// <summary>
        /// Defines the UserModel.
        /// </summary>
        public static CurrentLoginUserModel UserModel = new CurrentLoginUserModel()
        {
            UserName = "test"
        };

        /// <summary>
        /// Defines the <see cref="_log" />.
        /// </summary>
        internal static class _log
        {
            /// <summary>
            /// Gets the t_MESSystemConfig.
            /// </summary>
            internal static string t_MESSystemConfig
            {
                get
                {
                    return FileApp.ts_Log(@"AD\t_MESSystemConfig.json");
                }
            }

            //測試檢核程序的正確性
            /// <summary>
            /// Gets the t_AD_ENCODE_FORMAT_Check.
            /// </summary>
            internal static string t_AD_ENCODE_FORMAT_Check
            {
                get
                {
                    return FileApp.ts_Log(@"AD\t_AD_ENCODE_FORMAT_Check.json");
                }
            }
        }

        /// <summary>
        /// The T_AD_ENCODE_FORMAT_Check.
        /// </summary>
        [TestMethod]
        public void T_AD_ENCODE_FORMAT_Check()
        {
            var _r = FileApp.Read_SerializeJson<SequenceNum_PostData>(_log.t_AD_ENCODE_FORMAT_Check);
            var result = _v8n.AD_ENCODE_FORMAT.Check
                    (t_AD.UserModel, _r, true);
            Assert.IsTrue(result.Success);
            Assert.IsFalse(string.IsNullOrEmpty(_r.main.ENCODE_FORMAT_SID), "新增模式,ENCODE_FORMAT_SID 不應為空值");
            Assert.AreEqual(_r.main.CREATE_USER, "test", "新增模式,CREATE_USER 應為 test");
            Assert.IsNotNull(_r.main.CREATE_DATE, "新增模式,CREATE_DATE 不應為空值");

            _r.items = new List<MDL.MES.AD_ENCODE_FORMAT_ITEM>();
            var result1 = _v8n.AD_ENCODE_FORMAT.Check
                    (t_AD.UserModel, _r, true);
            Assert.IsFalse(result1.Success);
            var msg = string.Format(RES.BLL.Message.MustInput, RES.BLL.Face.SequenceDetail);
            Assert.AreEqual(result1.Message, msg, "測試身檔明細檢核-應該要有符合的錯誤訊息");
        }

        /// <summary>
        /// The t_MESSystemConfig.
        /// </summary>
        [TestMethod]
        public void t_MESSystemConfig()
        {
            MESSystemConfig systemConfig;
            DbContext dbContext = MVCContext.Create();
            /*
			 這裡的資料表 跟 MES 不是同一個資料庫, 所以必須轉用如下的方法 
			 */
            //using (var dbc = new MDL.MESContext())
            using (var dbc = MVCContext.Create())
            {
                using (BaseAdSystemConfigServices _svc = new BaseAdSystemConfigServices())
                {
                    _svc.DbContext = dbc;
                    //var readEntity = new SystemConfigServices().GetDataEntity("1");
                    systemConfig = _svc.GetEntityByKey("1").SystemconfigJson.ToObject<MESSystemConfig>();
                }

            }
            new FileApp().Write_SerializeJson(systemConfig, _log.t_MESSystemConfig);
        }

        /// <summary>
        /// The t_.
        /// </summary>
        [TestMethod]
        public void t_()
        {
            Dictionary<string, string> _dc = new Dictionary<string, string>();


            var systemConfig = FileApp.Read_SerializeJson<MESSystemConfig>(_log.t_MESSystemConfig);
            var dataType = systemConfig.GetType();
            var _list = dataType.GetProperties();

            var z = typeof(RES.BLL.Face);
            var _res = new ResourceManager(z);
            foreach (var _item in _list)
            {
                var _val = _res.GetString(_item.Name)?.ToString().Trim();
                _dc.Add(_item.Name, _val ?? _item.Name);
            }
        }

        /// <summary>
        /// The t_LDAP_AD驗証.
        /// </summary>
        [TestMethod]
        public void t_LDAP_AD驗証()
        {

            //using (PrincipalContext context = new PrincipalContext(ContextType.Domain, "genesis.com.tw"))
            //{
            //    // 驗證使用者名稱和密碼
            //    // XXX@genesis.com.tw 
            //    bool isValid = context.ValidateCredentials("username", "password");

            //    if (isValid)
            //    {
            //        Console.WriteLine("身份驗證成功！");
            //    }
            //    else
            //    {
            //        Console.WriteLine("身份驗證失敗！");
            //    }
            //}
        }

        /// <summary>
        /// The t_1.
        /// </summary>
        [TestMethod]
        public void t_1()
        {

            // Try to load the assembly.
            Assembly assem = typeof(RES.BLL.Face).Assembly;

            // Enumerate the resource files.
            string[] resNames = assem.GetManifestResourceNames();
            if (resNames.Length == 0)
                Console.WriteLine("   No resources found.");

            //foreach (var resName in resNames)
            //	Console.WriteLine("   Resource: {0}", resName.Replace(".resources", ""));



            var _val = new ResourceManager("RES.BLL.Face", typeof(RES.BLL.Face).Assembly).GetString("QuoteOnce1")?.ToString().Trim();
            var z = typeof(RES.BLL.Face);
            var _val1 = new ResourceManager(z).GetString("QuoteOnce")?.ToString().Trim();
        }

        /// <summary>
        /// The t_2.
        /// </summary>
        [TestMethod]
        public void t_2()
        {
            var json = new { machineID = "5179228C5B676460DEB32391544E018B490F89699", state = "1" };
            var machineID = Convert.ToString(json.machineID);
            //var regCode = Convert.ToString(json.regCode);
            var state = Convert.ToString(json.state); // 0:驗證註冊碼  1:重新檢查授權
                                                      //var productName = Convert.ToString(obj.productName);

            if (state == "0")
            {
                //RegisterClass reg = new RegisterClass();
                //var rCode = reg.GetCode(machineID);

                //if (rCode != regCode)
                //{
                //	//return null;
                //}
            }

            var registerServices = new BLL.MVC.RegisterServices();
            var dataInfo = (MDL.GenesisMVC.Tables.AD_REGISTER)registerServices.getData(machineID);

            if (dataInfo.SID == null)
            {
                //return null;
            }
        }
    }
}
