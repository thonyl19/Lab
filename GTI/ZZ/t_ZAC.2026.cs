using BLL.DataViews.Res;
using BLL.MES;
using FluentAssertions;
using Frame.Code.Web.Select;
using Genesis;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL;
using Genesis.Library.BLL.ADM;
using Genesis.Library.BLL.MES.OperTask.Custom;
using Genesis.Library.BLL.ZZ.ZAC.LiquidPreparationJudgingServices;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using UnitTestProject.TestUT;
//using _bllSvc = Genesis.Library.BLL.ZZ_ZAC;

namespace UnitTestProject
{
    public partial class t_ZAC : _testBase
	{
		[TestMethod]
		public void t_20260525_GetOperMaterials_含主要物料與替代物料()
		=> _DBTest((txn) =>
		{
			var db = txn.EFQuery_MES;
			var testRouteVerSid = "GTI26012313075963710";
			var testProductPartNo = "MTR_100001";
			var _lot = txn.GetLotInfo("DevTest_20260114-03", isQueryByLotNO: true);

			// 2. Act: 執行受測邏輯
			var resultList = Genesis.Library.BLL.MES.OperTask.Custom.ZAC_LotInfoDecorator.GetOperMaterials(_lot, txn);
			FileApp._tmpJson(resultList);

		}, isTransMode: true, isTest: true); // 強制回滾



        [TestMethod]
        public void t_20260610_SaveInspInfo_Operation()
        => _DBTest((txn) =>
        {
            // 1. Arrange: 準備測試資料
            string jsonPath = FileApp.ts_Log(@"ZZ\ZAC\t_SaveInspInfo.json");
            var jsonStr = File.ReadAllText(jsonPath);

            // Deserialize directly into PF_OPERATION_EDC list
            var testData = JsonConvert.DeserializeObject<SaveInspInfoTestData>(jsonStr);
            var form = testData.form;
            var pqcList = testData.Edc_List;

            var serv_DynFM = new DynFuncModuleServices();

            // 2. Act: 執行受測邏輯
            var result = serv_DynFM.SaveInspInfo_Operation(txn, form, pqcList, IsAutoGenerateSPCControl: false, isTest: true);

            //GTI_Test.TxnBase_T_ZAC(txn,"", txn.LinkSID);

            // 3. Assert: 驗證結果
            Assert.IsTrue(result.Success, $"SaveInspInfo_Operation failed: {result.Message}");

        }, isTransMode: true, isTest: true);

        [TestMethod]
        public void t_20260610_SaveInspInfo_PartRoute()
        => _DBTest((txn) =>
        {
            // 1. Arrange: 準備測試資料
            string jsonPath = FileApp.ts_Log(@"ZZ\ZAC\t_SaveInspInfo_PartRoute.json");
            var jsonStr = File.ReadAllText(jsonPath);

            // Deserialize directly into PF_OPERATION_EDC list
            var testData = JsonConvert.DeserializeObject<SaveInspInfoTestData_PartRoute>(jsonStr);
            var form = testData.form;
            var pqcList = testData.Edc_List;

            var serv_DynFM = new DynFuncModuleServices();

            // 2. Act: 執行受測邏輯
            var result = serv_DynFM.SaveInspInfo_PartRouteOper(txn, form , pqcList, testData.formExt, IsAutoGenerateSPCControl: false, isTest: true);

            //GTI_Test.TxnBase_T_ZAC(txn, "", txn.LinkSID);

            // 3. Assert: 驗證結果
            Assert.IsTrue(result.Success, $"SaveInspInfo_PartRouteOper failed: {result.Message}");

        }, isTransMode: true, isTest: true);



        private class SaveInspInfoTestData
        {
            public PF_OPERATION form { get; set; }
            public List<PF_OPERATION_EDC> Edc_List { get; set; }
        }

        private class SaveInspInfoTestData_PartRoute
        {
            public PartRouteOperServices.DataStruct form { get; set; }
            public List<PF_PARTNO_ROUTEVER_OPER_EDC> Edc_List { get; set; }
            public d_MainExt<PF_PARTNO_ROUTEVER, PF_PARTNO_ROUTEVER_EXT> formExt { get; set; }
        }




        private Genesis.Library.BLL.ZZ.ZAC.LiquidPreparationJudgingServices.Rules _service;
        private ControlLimits _defaultLimits;

        [TestInitialize]
        public void Setup()
        {
            // 每個測試案例執行前都會初始化服務與標準管制上下限
            _service = new Genesis.Library.BLL.ZZ.ZAC.LiquidPreparationJudgingServices.Rules();

            _defaultLimits = new ControlLimits
            {
                ViscosityLowerLimit = 50.0m,
                ViscosityUpperLimit = 100.0m,
                SolidContentLowerLimit = 20.0m,
                SolidContentUpperLimit = 30.0m
            };
        }

        /// <summary>
        /// 測試情境：當所有工程參數（溫度、黏度、固形分）皆在合格範圍內時，系統應判定總檢驗結果為合格。
        /// </summary>
        [TestMethod]
        public void ValidateInspection_AllParametersValid_ShouldReturnTotalPassed()
        {
            // -------------------------------------------------------------------------
            // 1. Arrange (準備階段): 建構完全符合規格的測試資料
            // -------------------------------------------------------------------------
            var request = new FluidInspectionRequest
            {
                // 塗料溫度：只要不為空值(null)即符合基本卡控
                CoatingTemperature = 25.5m,

                // 黏度資料：轉速必須與設定值一致，且實測黏度要在管制範圍內 (50.0 ~ 100.0)
                Viscosity = new ViscosityData
                {
                    TargetRpm = 1200m,     // 設定轉速
                    ActualRpm = 1200m,     // 實測轉速 (相符 -> 合格)
                    ActualViscosity = 75.0m // 實測黏度值 (在區間內 -> 合格)
                },

                // 固形分濃度：抽樣數量固定為 2 組
                SolidContentSamples = new List<SolidContentSample>
                {
                    // 第一組：((45烘烤 - 20皿) / 100塗料) * 100 = 25.0% (在 20%~30% 內 -> 合格)
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 45.0m },
                    
                    // 第二組：((47烘烤 - 20皿) / 100塗料) * 100 = 27.0% (在 20%~30% 內 -> 合格)
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 47.0m }
                    
                    // 平均值預期為: (25.0% + 27.0%) / 2 = 26.0% (在 20%~30% 內 -> 合格)
                },
                ControlLimits = _defaultLimits
            };

            // -------------------------------------------------------------------------
            // 2. Act (執行階段): 呼叫待測的核心業務邏輯
            // -------------------------------------------------------------------------
            var result = _service.ValidateInspection(request);

            // -------------------------------------------------------------------------
            // 3. Assert (驗證階段): 斷言最終產出的結果必須全數合格
            // -------------------------------------------------------------------------
            result.IsTotalPassed.Should().BeTrue();         // 總判定必須為合格
            result.IsTemperaturePassed.Should().BeTrue();   // 溫度項目必須合格
            result.IsViscosityPassed.Should().BeTrue();     // 黏度項目必須合格
            result.IsSolidContentPassed.Should().BeTrue();   // 固形分項目必須合格
            result.AverageSolidContent.Should().Be(26.0m);  // 驗證內部計算的平均值是否精確
        }

        /// <summary>
        /// 測試情境：當漏填塗料溫度（值為 null）時，系統應判定溫度不合格，進而導致總判定不合格。
        /// </summary>
        [TestMethod]
        public void ValidateInspection_TemperatureMissing_ShouldReturnTotalFailed()
        {
            // -------------------------------------------------------------------------
            // 1. Arrange (準備階段): 故意將溫度設為 null
            // -------------------------------------------------------------------------
            var request = new FluidInspectionRequest
            {
                CoatingTemperature = null, // 觸發「不可空白」的卡控規則
                Viscosity = new ViscosityData { TargetRpm = 1200m, ActualRpm = 1200m, ActualViscosity = 75.0m },
                SolidContentSamples = new List<SolidContentSample>
                {
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 45.0m },
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 45.0m }
                },
                ControlLimits = _defaultLimits
            };

            // -------------------------------------------------------------------------
            // 2. Act (執行階段)
            // -------------------------------------------------------------------------
            var result = _service.ValidateInspection(request);

            // -------------------------------------------------------------------------
            // 3. Assert (驗證階段)
            // -------------------------------------------------------------------------
            result.IsTotalPassed.Should().BeFalse();         // 最終判定必須是不合格
            result.IsTemperaturePassed.Should().BeFalse();   // 明確指出是溫度卡控失敗
        }

        /// <summary>
        /// 測試情境：當量測棒的「實測轉速」與「設定轉速」不一致時，黏度項目應判定不合格。
        /// </summary>
        [TestMethod]
        public void ValidateInspection_ViscosityRpmMismatch_ShouldReturnViscosityFailed()
        {
            // -------------------------------------------------------------------------
            // 1. Arrange (準備階段): 製造轉速不相符的情境
            // -------------------------------------------------------------------------
            var request = new FluidInspectionRequest
            {
                CoatingTemperature = 25.0m,
                Viscosity = new ViscosityData
                {
                    TargetRpm = 1200m, // 設定值為 1200
                    ActualRpm = 1150m, // 實測值為 1150 (現場轉速異常 -> 應觸發不合格)
                    ActualViscosity = 75.0m
                },
                SolidContentSamples = new List<SolidContentSample>
                {
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 45.0m },
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 45.0m }
                },
                ControlLimits = _defaultLimits
            };

            // -------------------------------------------------------------------------
            // 2. Act (執行階段)
            // -------------------------------------------------------------------------
            var result = _service.ValidateInspection(request);

            // -------------------------------------------------------------------------
            // 3. Assert (驗證階段)
            // -------------------------------------------------------------------------
            result.IsTotalPassed.Should().BeFalse();       // 總判定不合格
            result.IsViscosityPassed.Should().BeFalse();   // 明確指出是黏度卡控失敗
        }

        /// <summary>
        /// 測試情境：當固形分抽樣的單組計算值超出管制範圍時，即使平均值合格，整體固形分仍應判定不合格。
        /// </summary>
        [TestMethod]
        public void ValidateInspection_SolidContentAverageOutofBounds_ShouldReturnSolidContentFailed()
        {
            // -------------------------------------------------------------------------
            // 1. Arrange (準備階段): 透過調整管制下限，刻意讓其中一組樣品超標
            // -------------------------------------------------------------------------
            var request = new FluidInspectionRequest
            {
                CoatingTemperature = 25.0m,
                Viscosity = new ViscosityData { TargetRpm = 1200m, ActualRpm = 1200m, ActualViscosity = 75.0m },
                SolidContentSamples = new List<SolidContentSample>
                {
                    // 第一組濃度 = ((41 - 20) / 100) * 100 = 21.0%  (低於下限 22.0% -> 此組超標！)
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 41.0m },
                    
                    // 第二組濃度 = ((49 - 20) / 100) * 100 = 29.0%  (在 22.0% ~ 30.0% 內 -> 合格)
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 49.0m }
                    
                    // 兩組平均值 = (21.0% + 29.0%) / 2 = 25.0%   (平均值雖在 22.0% ~ 30.0% 內，但單組超標仍算不合格)
                },
                ControlLimits = new ControlLimits
                {
                    ViscosityLowerLimit = 50.0m,
                    ViscosityUpperLimit = 100.0m,
                    SolidContentLowerLimit = 22.0m, // 故意將管制下限由 20.0 提高到 22.0
                    SolidContentUpperLimit = 30.0m
                }
            };

            // -------------------------------------------------------------------------
            // 2. Act (執行階段)
            // -------------------------------------------------------------------------
            var result = _service.ValidateInspection(request);

            // -------------------------------------------------------------------------
            // 3. Assert (驗證階段)
            // -------------------------------------------------------------------------
            result.IsTotalPassed.Should().BeFalse();          // 總判定不合格
            result.IsSolidContentPassed.Should().BeFalse();   // 固形分濃度判定失敗
        }

        /// <summary>
        /// 測試情境：後端映射應將 EDC 資料正確轉成調液檢查 request，並帶出 2 組固形分樣本與上下限。
        /// </summary>
        [TestMethod]
        public void ToFluidInspectionRequest_ShouldMapLiquidInspectionFieldsCorrectly()
        {
            var edcData = new List<EdcFormInfo>
            {
                new EdcFormInfo
                {
                    ItemNo = LiquidPreparationParameterCodes.Rpm,
                    ItemName = "RPM",
                    TL = "1200m",
                    LSL = 1100m,
                    LCL = 1150m,
                    USL = 1250m,
                    UCL = 1300m,
                    InputValueList = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "1200" } }
                    }
                },
                new EdcFormInfo
                {
                    ItemNo = LiquidPreparationParameterCodes.Viscosity,
                    ItemName = "黏度",
                    InputValueList = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "75.5" } }
                    },
                    LSL = 50m,
                    LCL = 55m,
                    USL = 90m,
                    UCL = 95m
                },
                new EdcFormInfo
                {
                    ItemNo = LiquidPreparationParameterCodes.Dish,
                    ItemName = "測量皿",
                    InputValueList = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "20" } },
                        new Dictionary<string, string> { { "2", "20" } }
                    }
                },
                new EdcFormInfo
                {
                    ItemNo = LiquidPreparationParameterCodes.Coating,
                    ItemName = "塗料克重",
                    InputValueList = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "100" } },
                        new Dictionary<string, string> { { "2", "100" } }
                    },
                    LSL = 20m,
                    LCL = 22m,
                    USL = 30m,
                    UCL = 32m
                },
                new EdcFormInfo
                {
                    ItemNo = LiquidPreparationParameterCodes.Baked,
                    ItemName = "烘烤克重",
                    InputValueList = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "45" } },
                        new Dictionary<string, string> { { "2", "47" } }
                    }
                },
                new EdcFormInfo
                {
                    ItemNo = LiquidPreparationParameterCodes.Temperature,
                    ItemName = "塗料溫度",
                    InputValueList = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "25.5" } }
                    }
                }
            };

            var request = EdcFormInfoLiquidPreparationMapper.ToFluidInspectionRequest(edcData);

            request.CoatingTemperature.Should().Be(25.5m);
            request.Viscosity.TargetRpm.Should().Be(1200m);
            request.Viscosity.ActualRpm.Should().Be(1200m);
            request.Viscosity.ActualViscosity.Should().Be(75.5m);
            request.ControlLimits.ViscosityLowerLimit.Should().Be(50m);
            request.ControlLimits.ViscosityUpperLimit.Should().Be(90m);
            request.ControlLimits.SolidContentLowerLimit.Should().Be(20m);
            request.ControlLimits.SolidContentUpperLimit.Should().Be(30m);
            request.SolidContentSamples.Should().HaveCount(2);
            request.SolidContentSamples[0].DishWeight.Should().Be(20m);
            request.SolidContentSamples[0].CoatingWeight.Should().Be(100m);
            request.SolidContentSamples[0].BakedWeight.Should().Be(45m);
            request.SolidContentSamples[1].DishWeight.Should().Be(20m);
            request.SolidContentSamples[1].CoatingWeight.Should().Be(100m);
            request.SolidContentSamples[1].BakedWeight.Should().Be(47m);
        }

        /// <summary>
        /// 測試情境：固形分抽樣未滿 2 組時，應判定不合格並回傳對應訊息。
        /// </summary>
        [TestMethod]
        public void ValidateInspection_SolidContentSamplesLessThanTwo_ShouldReturnSolidContentFailed()
        {
            var request = new FluidInspectionRequest
            {
                CoatingTemperature = 25.0m,
                Viscosity = new ViscosityData { TargetRpm = 1200m, ActualRpm = 1200m, ActualViscosity = 75.0m },
                SolidContentSamples = new List<SolidContentSample>
                {
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 45.0m }
                },
                ControlLimits = _defaultLimits
            };

            var result = _service.ValidateInspection(request);

            result.IsSolidContentPassed.Should().BeFalse();
            result.IsTotalPassed.Should().BeFalse();
            result.ValidationMessages.Should().Contain(x => x.Contains("抽樣數量必須為 2"));
        }

        /// <summary>
        /// 測試情境：測量皿空白時，固形分應判定不合格，且應帶出缺漏代碼。
        /// </summary>
        [TestMethod]
        public void ValidateInspection_DishMissing_ShouldReturnDishMissingMessage()
        {
            var request = new FluidInspectionRequest
            {
                CoatingTemperature = 25.0m,
                Viscosity = new ViscosityData { TargetRpm = 1200m, ActualRpm = 1200m, ActualViscosity = 75.0m },
                SolidContentSamples = new List<SolidContentSample>
                {
                    new SolidContentSample { DishWeight = null, CoatingWeight = 100.0m, BakedWeight = 45.0m },
                    new SolidContentSample { DishWeight = 20.0m, CoatingWeight = 100.0m, BakedWeight = 47.0m }
                },
                ControlLimits = _defaultLimits
            };

            var result = _service.ValidateInspection(request);

            result.IsSolidContentPassed.Should().BeFalse();
            result.IsTotalPassed.Should().BeFalse();
            result.MissingParameterCodes.Should().Contain(LiquidPreparationParameterCodes.Dish);
            result.ValidationMessages.Should().Contain(x => x.Contains("測量皿重量不可空白"));
        }


        [TestMethod]
        public void t_20260617_OperInspInfo_查詢檢驗資訊_應回傳正確擴充資料()
        => _DBTest((txn) => {
            // ----------------------------------------------------
            // 1. Arrange: 準備測試資料
            // ----------------------------------------------------
            //var testPartnoSid = "TEST_PART_001";
            //var testRouteSid = "TEST_ROUTE_001";
            //var testPartRouteSid = "TEST_PR_SID_001";
            //// 建立主表資料 PF_PARTNO_ROUTEVER
            //var mainRecord = new PF_PARTNO_ROUTEVER
            //{
            //    PART_ROUTE_SID = testPartRouteSid,
            //    PARTNO_SID = testPartnoSid,
            //    ROUTE_SID = testRouteSid,
            //    CREATE_USER = "UT_USER",
            //    CREATE_DATE = DateTime.Now,
            //    UPDATE_USER = "UT_USER",
            //    UPDATE_DATE = DateTime.Now
            //};
            //txn.EFQuery_MES.PF_PARTNO_ROUTEVER.Add(mainRecord);
            //// 建立擴充表資料 PF_PARTNO_ROUTEVER_EXT
            //var extRecord = new PF_PARTNO_ROUTEVER_EXT
            //{
            //    PART_ROUTE_SID = testPartRouteSid,
            //    SYSTEM_JUDGMENT = "Y",
            //    UPDATE_USER = "UT_USER",
            //    UPDATE_DATE = DateTime.Now
            //};
            //txn.EFQuery_MES.PF_PARTNO_ROUTEVER_EXT.Add(extRecord);

            // 寫入變更 (此變更會在測試結束後自動 Rollback)
            //txn.SaveChanges();
            // 模擬 LotInfo (設定基本屬性以確保 GetPartNoVersionInfo 與 GetRouteVersionInfo 能正確運作)
            var lotInfo = txn.GetLotInfo("DevTest_20260114-03-03",isQueryByLotNO:true);
                
            // ----------------------------------------------------
            // 2. Act: 執行業務邏輯
            // ----------------------------------------------------
            var result = ZAC_LotInfoDecorator.OperInspInfo_CheckIn(lotInfo, txn);
            // ----------------------------------------------------
            // 3. Assert: 驗證結果
            // ----------------------------------------------------
            Assert.IsNotNull(result, "回傳之 d_MainExt 物件不應為 null");
            //Assert.AreEqual(testPartRouteSid, result.Main.PART_ROUTE_SID, "主表 SID 應相符");
            //Assert.AreEqual("Y", result.Ext.SYSTEM_JUDGMENT, "系統判定值應為 'Y'");
        }, isTransMode: true, isTest: true); // isTest: true 確保測試資料自動回滾


        [TestMethod]
        public void t_20260626_InspOperMaintain_ListData()
        => _DBTest((txn) =>
        {
            var r = FileApp.Read_SerializeJson<PagerQuery>(_log.t_20260626_InspOperMaintain_ListData);
            var Result = InspOperMaintainQueryServices.QueryList(r,false);
        }, true);


        [TestMethod]
        public void t_20260626_parseEdcInput()
        => _DBTest((txn) =>
        {
            //var r = FileApp.Read_SerializeJson<PagerQuery>(_log.t_20260626_InspOperMaintain_ListData);
            var Result = QMSService.parseEdcInput(txn.DBC, "GTI26062820061795738");
        }, true);



        [TestMethod]
        public void t_20260717_LoadInspInfo_PartRouteOper()
        => _DBTest((txn) =>
        {
            var _lotInfo = txn.GetLotInfo("WO-20260713-001-02", isQueryByLotNO: true);
            //var edcData = edcFun.GetEdcOperParaData_OperSid(_lotInfo.WO, _lotInfo.ROUTE_VER_SID, _lotInfo.ROUTE_VER_OPER_SID, _lotInfo.PARTNO, _lotInfo.OPER_SID);
            var _part = _lotInfo.GetPartVersionInfo();
            var _route = _lotInfo.GetRouteVersionInfo();
            var arg = new PartRouteOperServices.DataStruct()
            {
                PARTNO_SID = _part.PARTNO_SID,
                ROUTE_VER_SID = _lotInfo.ROUTE_VER_SID,
                ROUTE_VER_OPER_SID = _lotInfo.ROUTE_VER_OPER_SID,
                ROUTE_SID = _route.ROUTE_SID,
                OPER_SID = _lotInfo.OPER_SID,
            };
            var r = new DynFuncModuleServices().LoadInspInfo_PartRouteOper(arg);

            //var r = FileApp.Read_SerializeJson<PagerQuery>(_log.t_20260626_InspOperMaintain_ListData);
            //var Result = QMSService.parseEdcInput(txn.DBC, "GTI26062820061795738");
        }, true);
    }
}

