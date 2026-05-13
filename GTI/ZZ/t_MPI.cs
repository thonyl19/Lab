using System;
using System.Collections.Generic;
using System.Linq;
using BLL.DataViews.Res;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Frame.Code.Web.Select;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction.WIP;
using Genesis.Gtimes.WIP;
using Genesis.Library.BLL.ZZ.MPI;
using Genesis.Library.BLL.ZZ.MPI.CaseRule;
using Genesis.Library.BLL.ZZ.MPI.EDC;
using Genesis.Library.BLL.ZZ.MPI.Model;
using Genesis.WebApi;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.EDCUtility;
using static Genesis.Library.BLL.ADM.EDCTargetServices;
using static Genesis.Library.BLL.ADM.ESopCheckListTargetServices;
using _Frame = Genesis.Library.Frame.Code.Web.TableQuery;
using _MPIApiServices = Genesis.Library.BLL.ZZ.MPI.MPIApiServices;
using _svc_EDC = Genesis.Library.BLL.ADM.EDCTargetServices;
using _svc_ESop = Genesis.Library.BLL.ADM.ESopCheckListTargetServices;

//todo-MPI
namespace UnitTestProject
{
    [TestClass]
    public partial class t_MPI : _testBase
    {
        static class _log
        {

            internal static string t_EDC對象維護
            {
                get
                {

                    return FileApp.ts_Log(@"ZZ\MPI\t_EDC對象維護.json");
                }
            }


            internal static string t_StationCheckOut(string ext = "")
            {
                return FileApp.ts_Log($@"ZZ\MPI\t_StationCheckOut{ext}.json");
            }

            internal static string t_排除關連對象條件重覆的問題
            {
                get
                {
                    return FileApp.ts_Log(@"ZZ\MPI\t_排除關連對象條件重覆的問題.json");
                }
            }
            internal static string t_OperEdcCfg
            {
                get
                {
                    return FileApp.ts_Log(@"ZZ\MPI\t_OperEdcCfg.json");
                }
            }

            public static string t_20260309_ListLotEDC_MPI
            {
                get
                {
                    return FileApp.ts_Log(@"ZZ\MPI\t_20260309_ListLotEDC_MPI.json");
                }
            }

            public static string t_EdcLoadArguments
            {
                get
                {
                    return FileApp.ts_Log(@"ZZ\MPI\t_EdcLoadArguments.json");
                }
            }

        }


        [TestMethod]
        public void t_SubRouteVer()
        {
            var z = DDLServices.SubRouteVer(null, MDL.SearchKey.All)
                .Select(c => new CustomerList()
                {
                    SID = c.SID,
                    No = c.No,
                    Display = c.Display,
                    Value = c.Value,
                    INum1 = c.INum1,
                    Attr05 = $"{c.Display} (第{c.INum1}版)"
                })
                .ToList();


            FileApp._tmpJson(z);
        }


        [TestMethod]
        public void t_Update_ENABLE_FLAG()
        => _DBTest((Txn) =>
        {
            var t = Txn.EFQuery_MES.FC_CHECKLIST_TARGET.FirstOrDefault();
            Txn.Update_ENABLE_FLAG(t, c => c.CHECKLIST_TARGET_SID == t.CHECKLIST_TARGET_SID, true);

            //FileApp._tmpJson(z);
        }, true, true);

        [TestMethod]
        public void t_V_LOOKUP_TABLE()
        => _DBTest((Txn) =>
        {
            var z = Txn.EFQuery_MES.f_V_LOOKUP_TABLE("TF", "Linda Jones");
            FileApp._tmpJson(z);
        }, true);



        [TestMethod]
        public void t_CallAPI()
        {
            var _api = new Genesis.Library.BLL.ZZ.MPI.APISetting()
            {
                URL = "http://localhost:59394/MPIMES_DEV/posting-trigger/get-edc-limitset"
            };
            var requestSet = _MPIApiServices.GenerateRequestSet(_MPIApiServices.ApiName.GetEdcLimit, new { }, _api);
            var r = _MPIApiServices.CallAPI(requestSet);
            var r1 = Json.ToObject<ApiResponse_Edc>(r.Result.Data);

        }

        [TestMethod]
        public void t_()
        {
            var fromRuleString = _svc_ESop.TargetFromRule.CHECKLIST.ToString();// fromRule.ToString();

            var _r = FileApp.Read_SerializeJson<_Frame.PagerQuery>(t_MTR._log.t_Search_QCResult_Query);

            FC_TARGET targetFilter = null;

            //var query = from checklist in Txn.EFQuery_MES.FC_CHECKLIST_TARGET
            //    join target in Txn.EFQuery_MES.FC_TARGET
            //        on checklist.CHECKLIST_TARGET_SID equals target.FROM_RULE_SID
            //    where target.FROM_RULE == fromRuleString
            //    group target by checklist into g
            //    select new _svc_ESop.DataViewModel
            //    {
            //        CHECKLIST_TARGET_SID = g.Key.CHECKLIST_TARGET_SID,
            //        CHECKLIST_VER_ITEM_SID = g.Key.CHECKLIST_VER_ITEM_SID,
            //        CHECKLIST_VER_SID = g.Key.CHECKLIST_VER_SID,
            //        CHECKLIST_VER_NO = g.Key.CHECKLIST_VER_NO,
            //        CHECKLIST_VER_NAME = g.Key.CHECKLIST_VER_NAME,
            //        VERSION = g.Key.VERSION,
            //        ENABLE_FLAG = g.Key.ENABLE_FLAG,
            //        CREATE_USER = g.Key.CREATE_USER,
            //        CREATE_DATE = g.Key.CREATE_DATE,
            //        WIP_SCOPE = g.Key.WIP_SCOPE,
            //        TARGET_LIST = g.ToList()  // 已分組的目標列表
            //    };

            //// 動態過濾條件
            //query = ApplyTargetFilters(query, targetFilter);

            //var r = _svc_EDC.ListData(targetFilter, _r.Page).Data;

            //FileApp.WriteSerializeJson(r,_log.t_EDC對象維護);

        }

        public class PagedResult<DataViewModel>
        {
            public List<DataViewModel> Data { get; set; }
            public int TotalCount { get; set; }
            public int PageSize { get; set; }
            public int CurrentPage { get; set; }
        }
        // 過濾條件參數類
        public class TargetFilter
        {
            public string TARGET_OBJECT { get; set; }
            public string TARGET_NO { get; set; }
            public string TARGET_NAME { get; set; }
        }
        private IQueryable<_svc_ESop.DataViewModel> ApplyTargetFilters(IQueryable<_svc_ESop.DataViewModel> query, TargetFilter targetFilter)
        {
            if (targetFilter == null) return query;

            if (!string.IsNullOrEmpty(targetFilter.TARGET_OBJECT))
            {
                query = query.Where(x => x.TargetList.Any(t => t.TARGET_OBJECT == targetFilter.TARGET_OBJECT));
            }

            if (!string.IsNullOrEmpty(targetFilter.TARGET_NO))
            {
                query = query.Where(x => x.TargetList.Any(t => t.TARGET_NO.Contains(targetFilter.TARGET_NO)));
            }

            if (!string.IsNullOrEmpty(targetFilter.TARGET_NAME))
            {
                query = query.Where(x => x.TargetList.Any(t => t.TARGET_NAME.Contains(targetFilter.TARGET_NAME)));
            }

            return query;
        }


        [TestMethod]
        public void t_fn()
        {
            //var t = Genesis.Library.BLL.ADM.EDCTargetServices.API_GetEdcLimit();
        }

        [TestMethod]
        public void t_EDC過站載入()
        => _DBTest((txn) =>
        {
            var arg = new
            {
                LOT_STATUS = "",
                EQP_SID = "",
                OPER_SID = "GTI25080812465111012",
                PARTNO_SID = "GTI25082009555936699",
                PART_NO = "PKZ022000022",
                ROUTE_VER_SID = "",
                LINE = new string[] { "JK_LINE", "TF" },
            };
            var db = txn.EFQuery_MES;
            //var t1 = q_Part.ToList();
            var q_Oper = (from a in db.PF_OPERATION where a.OPER_SID == arg.OPER_SID && arg.LINE.Contains(a.LINE_NO) select a);
            var q_Part = (from a in db.PF_PARTNO where a.PARTNO_SID == arg.PARTNO_SID && arg.LINE.Contains(a.LINE_NO) select a);
            var q_EQP = (from a in db.FC_EQUIPMENT where a.EQP_SID == arg.EQP_SID && arg.LINE.Contains(a.LINE_NO) select a);
            var q_SubRouteVer = (from a in db.PF_ROUTE_VER where a.ROUTE_VER_SID == arg.ROUTE_VER_SID && arg.LINE.Contains(a.LINE_NO) select a);
            var main = (from a in db.FC_TARGET
                        join b in db.FC_EDC_TARGET
                            on a.FROM_RULE_SID equals b.EDC_TARGET_SID
                        where a.FROM_RULE == "EDC"
                            && (
                                (a.TARGET_OBJECT == TargetObject.Operation.ToString()
                                    && q_Oper.Any(c => c.OPER_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                                || (a.TARGET_OBJECT == TargetObject.Part.ToString()
                                    && q_Part.Any(c => c.PARTNO_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                                || (a.TARGET_OBJECT == TargetObject.Equipment.ToString()
                                    && q_EQP.Any(c => c.EQP_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                                || (a.TARGET_OBJECT == TargetObject.Part.ToString()
                                    && q_SubRouteVer.Any(c => c.ROUTE_VER_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                            )
                        select new
                        {
                            a,
                            b,
                            Weight = a.TARGET_OBJECT == TargetObject.Operation.ToString() ? 1.1
                                : a.TARGET_OBJECT == TargetObject.Part.ToString() ? 1.01
                                : a.TARGET_OBJECT == TargetObject.Equipment.ToString() ? 1.001
                                : 1.0

                        }
                        );

            var result = (from m in main
                          group m by m.a.FROM_RULE_SID into g
                          select new
                          {
                              FROM_RULE_SID = g.Key,
                              TotalWeight = g.Sum(x => x.Weight),
                              //FC_EDC_TARGET = db.FC_EDC_TARGET.Where(c => c.EDC_TARGET_SID == g.Key).FirstOrDefault()
                              FC_EDC_TARGET = g.Select(x => x.b).FirstOrDefault()
                          })
                        .OrderByDescending(x => x.TotalWeight)
                        .FirstOrDefault();

            var edc_ver = (from a in db.FC_EDC_VER
                           where a.EDC_VER_SID == result.FC_EDC_TARGET.EDC_VER_SID
                           select a).ToList();

            var q_edc_para = (from a in db.FC_EDC_VER_PARAMETER
                              where a.EDC_VER_SID == result.FC_EDC_TARGET.EDC_VER_SID
                              select a
                        );



            var group_code = (from a in db.ZZ_EDC_GROUP_CODE
                              where q_edc_para.Any(c => c.GROUP_CODE_SID == a.SID)
                              select a).ToList();
            var edc_para = q_edc_para.ToList();

            var q1 = from a in db.FC_EDC_VER_PARAMETER
                     join b in db.ZZ_EDC_GROUP_CODE
                         on a.GROUP_CODE_SID equals b.SID
                     where a.EDC_VER_SID == result.FC_EDC_TARGET.EDC_VER_SID
                     group new { a, b } by a.GROUP_CODE_SID into g
                     select new _q1
                     {
                         GROUP_CODE_SID = g.Key,
                         GroupCodeList = g.Select(x => x.b).FirstOrDefault(),
                         //  GroupCodeList = g.Select(x => new ZZ_EDC_GROUP_CODE
                         //  {
                         //      //x.b.SID,
                         //       x.b.GROUP_CODE,
                         //      x.b.SAMPLE_TYPE,
                         //      x.b.ITEM_RESOURCE,
                         //      x.b.MAXIMUM,
                         //      x.b.API_ROUTE
                         //  }).Distinct(),
                         ParameterList = g.Select(x => x.a).OrderBy(x => x.ITEM_SEQ).ToList()

                         // 這裡 x.a 即為 FC_EDC_VER_PARAMETER 實體

                         // 2. FC_EDC_VER_PARAMETER 依 ITEM_SEQ 排序的清單
                         //  ParameterList = g.Select(x => new FC_EDC_VER_PARAMETER
                         //  {
                         //      x.a.EDC_VER_PARA_SID,
                         //      //x.a.EDC_VER_SID,
                         //      //x.a.EDC_SID,
                         //      //x.a.EDC_NO,
                         //      //x.a.EDC_NAME,
                         //      //x.a.VERSION,
                         //      //x.a.EDC_PARA_SID,
                         //      x.a.PARA_NO,
                         //      x.a.PARAMETER,
                         //      x.a.DATATYPE,
                         //      x.a.TL,
                         //      x.a.UCL,
                         //      x.a.LCL,
                         //      x.a.USL,
                         //      x.a.LSL,
                         //      //x.a.SAMPLESIZE,
                         //      //x.a.THROW_SPC,
                         //      x.a.MUST_INPUT,
                         //      x.a.ITEM_SEQ,
                         //     // x.a.DB_TABLE,
                         //      //x.a.DISPLAY_POINT_NAME,
                         //      //x.a.DB_COLUMN_NAME,
                         //      x.a.DEFAULT_TL
                         //  }).OrderBy(x => x.ITEM_SEQ)
                     };
            var q1_r = q1.ToList();
            foreach (var i in q1_r)
            {
                i.parse();
            }
            FileApp._tmpJson(new { q1_r });

            // 將 ParameterList 轉換為目標格式
            //var result1 = new
            //{
            //    data = new List<Dictionary<string, object>>{},
            //    columns = new List<FiledSet>()
            //};

            /*
            // 取得 ParameterList (假設您已經有這個列表)
            var parameterList = q1.FirstOrDefault()?.ParameterList.ToList();

            var dataRow = new Dictionary<string, object>();
            if (parameterList != null && parameterList.Any())
            {
            
                // 處理 columns 部分
                foreach (var param in parameterList.OrderBy(p => p.ITEM_SEQ))
                {
                    string sid = param.EDC_VER_PARA_SID;
                    // 根據 DATATYPE 設定預設值
                    object defaultValue = param.DATATYPE;
                    dataRow[sid] = defaultValue;
                    var column = new Dictionary<string, object>
                    {
                        { "title", param.PARAMETER },
                        { "field", param.EDC_VER_PARA_SID }
                    };

                    // 根據 DATATYPE 設定對應的 type 和相關屬性
                    switch (param.DATATYPE)
                    {
                        case "N": // 數值型
                            column["type"] = "numeric";
                            break;
                        case "B": // 布林型
                            column["type"] = "checkbox";
                            break;
                        case "L": // 下拉選單
                            column["type"] = "dropdown";
                            column["source"] = new
                            {
                                values = new List<string> { "台北", "台中", "高雄", "台南", "其他" }
                            };
                            break;
                        case "S": // 字串型 - 使用預設的 text 類型
                        default:
                            break;
                    }
                    result1.columns.Add(column);
                }
                result1.data.Add(dataRow);

                FileApp._tmpJson(new {  q1 , result1 });

            }
            */
            // 回傳結果




            /*
             * var q1 = (from a in db.FC_EDC_VER_PARAMETER
                      join b in db.ZZ_EDC_GROUP_CODE
                          on a.GROUP_CODE_SID equals b.SID
                      where a.EDC_VER_SID == result.FC_EDC_TARGET.EDC_VER_SID
                      select new _FC_EDC_VER_PARAMETER
                      {
                          EDC_VER_PARA_SID = a.EDC_VER_PARA_SID,
                          EDC_VER_SID = a.EDC_VER_SID,
                          EDC_SID = a.EDC_SID,
                          EDC_NO = a.EDC_NO,
                          EDC_NAME = a.EDC_NAME,
                          VERSION = a.VERSION,
                          EDC_PARA_SID = a.EDC_PARA_SID,
                          PARA_NO = a.PARA_NO,
                          PARAMETER = a.PARAMETER,
                          DATATYPE = a.DATATYPE,
                          TL = a.TL,
                          UCL = a.UCL,
                          LCL = a.LCL,
                          USL = a.USL,
                          LSL = a.LSL,
                          CREATE_USER = a.CREATE_USER,
                          CREATE_DATE = a.CREATE_DATE,
                          UPDATE_USER = a.UPDATE_USER,
                          UPDATE_DATE = a.UPDATE_DATE,
                          SAMPLESIZE = a.SAMPLESIZE,
                          THROW_SPC = a.THROW_SPC,
                          MUST_INPUT = a.MUST_INPUT,
                          ITEM_SEQ = a.ITEM_SEQ,
                          DISPLAY_POINT_NAME = a.DISPLAY_POINT_NAME,
                          DB_TABLE = a.DB_TABLE,
                          DB_COLUMN_NAME = a.DB_COLUMN_NAME,
                          GROUP_CODE_SID = a.GROUP_CODE_SID,
                          DEFAULT_TL = a.DEFAULT_TL,
                          GROUP_CODE = b.GROUP_CODE,
                          SAMPLE_TYPE = b.SAMPLE_TYPE,
                          ITEM_RESOURCE = b.ITEM_RESOURCE,
                          MAXIMUM = b.MAXIMUM,
                          API_ROUTE = b.API_ROUTE
                      }).ToList();
       */



        }, true);

        [TestMethod]
        public void t_EDC過站載入_1()
=> _DBTest((txn) =>
{
    var arg = new
    {
        LOT_STATUS = "",
        EQP_SID = "",
        OPER_SID = "GTI25080812465111012",
        PARTNO_SID = "GTI25082009555936699",
        PART_NO = "PKZ022000022",
        ROUTE_VER_SID = "",
        LINE = new string[] { "JK_LINE", "TF" },
    };
    var db = txn.EFQuery_MES;
    //var t1 = q_Part.ToList();
    var q_Oper = (from a in db.PF_OPERATION where a.OPER_SID == arg.OPER_SID && arg.LINE.Contains(a.LINE_NO) select a);
    var q_Part = (from a in db.PF_PARTNO where a.PARTNO_SID == arg.PARTNO_SID && arg.LINE.Contains(a.LINE_NO) select a);
    var q_EQP = (from a in db.FC_EQUIPMENT where a.EQP_SID == arg.EQP_SID && arg.LINE.Contains(a.LINE_NO) select a);
    var q_SubRouteVer = (from a in db.PF_ROUTE_VER where a.ROUTE_VER_SID == arg.ROUTE_VER_SID && arg.LINE.Contains(a.LINE_NO) select a);
    var main = (from a in db.FC_TARGET
                join b in db.FC_EDC_TARGET
                    on a.FROM_RULE_SID equals b.EDC_TARGET_SID
                where a.FROM_RULE == "EDC"
                    && (
                        (a.TARGET_OBJECT == TargetObject.Operation.ToString()
                            && q_Oper.Any(c => c.OPER_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                        || (a.TARGET_OBJECT == TargetObject.Part.ToString()
                            && q_Part.Any(c => c.PARTNO_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                        || (a.TARGET_OBJECT == TargetObject.Equipment.ToString()
                            && q_EQP.Any(c => c.EQP_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                        || (a.TARGET_OBJECT == TargetObject.Part.ToString()
                            && q_SubRouteVer.Any(c => c.ROUTE_VER_SID == a.TARGET_SID && c.LINE_NO == b.LINE_NO))
                    )
                select new
                {
                    a,
                    b,
                    Weight = a.TARGET_OBJECT == TargetObject.Operation.ToString() ? 1.1
                        : a.TARGET_OBJECT == TargetObject.Part.ToString() ? 1.01
                        : a.TARGET_OBJECT == TargetObject.Equipment.ToString() ? 1.001
                        : 1.0

                }
                );

    var result = (from m in main
                  group m by m.a.FROM_RULE_SID into g
                  select new
                  {
                      FROM_RULE_SID = g.Key,
                      TotalWeight = g.Sum(x => x.Weight),
                      //FC_EDC_TARGET = db.FC_EDC_TARGET.Where(c => c.EDC_TARGET_SID == g.Key).FirstOrDefault()
                      FC_EDC_TARGET = g.Select(x => x.b).FirstOrDefault()
                  })
                .OrderByDescending(x => x.TotalWeight)
                .FirstOrDefault();

    var q0 = from a in db.FC_EDC_VER_PARAMETER
             join b in db.ZZ_EDC_GROUP_CODE
                 on a.GROUP_CODE_SID equals b.SID

             where a.EDC_VER_SID == result.FC_EDC_TARGET.EDC_VER_SID
             select new { a, b };

    var _V_LOOKUP_TABLE = from a in db.V_LOOKUP_TABLE
                          where arg.LINE.Contains(a.LUT_LINE)
                              && q0.Any(q =>
                                  (q.a.DATATYPE == "L" && a.LUT_NO == q.a.DB_COLUMN_NAME)
                                  || (q.b.SAMPLE_TYPE == "清單" && a.LUT_NO == q.b.ITEM_RESOURCE)
                              )
                          group new { a } by a.LUT_NO into g
                          select new
                          {
                              Key = g.Key,
                              List = g.Select(x => x.a.LUT_VALUE).ToList()
                          }
        ;


    var q1 = from _q0 in q0
             group new { _q0.a, _q0.b } by _q0.a.GROUP_CODE_SID into g
             select new _q1
             {
                 GROUP_CODE_SID = g.Key,
                 //GroupCodeList = g.Select(x => x.b).FirstOrDefault(),
                 GroupCodeList1 = g.Select(x => new EdcGroupCodeDetail
                 {
                     SID = x.b.SID,
                     EDC_SID = x.b.EDC_SID,
                     GROUP_CODE = x.b.GROUP_CODE,
                     ITEM_RESOURCE = x.b.ITEM_RESOURCE,
                     SAMPLE_TYPE = x.b.SAMPLE_TYPE,
                     MAXIMUM = x.b.MAXIMUM,
                 }).FirstOrDefault(),
                 ParameterList = g.Select(x => x.a)
                    .OrderBy(x => x.ITEM_SEQ).ToList()

             };
    var q1_r = q1.ToList();
    var q2_r = _V_LOOKUP_TABLE.ToDictionary(c => c.Key, c => c.List);
    foreach (var i in q1_r)
    {
        i.parse(q2_r);
    }




    FileApp._tmpJson(new { q2_r, q1_r });


}, true);



        [TestMethod]
        public void t_20260224_OperTask_Edc()
        => _DBTest((txn) =>
        {
            /*
            依批號 / 機台 / 工站 取得 EDC 
             
            */

            var lot = txn.GetLotInfo("JK_GENERAL_TEST-08", isQueryByLotNO: true);
            //var z = lot.GetCurrentEquipmentInfo
            var eqp = txn.GetEquipmentInfo("JK_LINE-EQP-00000004 ", EquipmentUtility.IndexType.No);
            var eqps = (from a in lot.GetLotProductionEquipmentInfoList().AsEnumerable()
                        select a.SID).ToArray();

            var arg = new EdcLoadArguments
            {
                LOT_STATUS = "",
                EQP_SID = eqps,
                OPER_SID = "GTI25110510554254406",
                //PARTNO_SID = "GTI25082009555936699",
                //PART_NO = "JK_API_CREATE_03",
                //SUBROUTE_VER_SID
                ROUTE_VER_SID = "",
                LINE = new string[] { "JK_LINE", "TF" },
            };
            var r1 = _svc_EDC.get_FC_EDC_TARGET_rule(txn.EFQuery_MES, arg);
            FileApp._tmpJson(r1);

            var r1_x = r1[0].FC_EDC_TARGET;

            // 打 API 調修 EDC 內容
            var _api = new Genesis.Library.BLL.ZZ.MPI.APISetting()
            {
                URL = "http://localhost:59394/MPIMES_DEV/posting-trigger/get-edc-limitset"
            };
            var r2 = _svc_EDC.OperTask_Edc(txn, r1[0], lot.SID, _api, true);
            FileApp.WriteSerializeJson(r2, _log.t_OperEdcCfg);
        }, true);



        [TestMethod]
        public void t_20260210_from_current_sub_route_ver_oper_sid()
        => _DBTest((txn) =>
        {
            var lot = txn.GetLotInfo("GTI25100910461295868");
            var z = lot.from_current_sub_route_ver_oper_sid();

        }, true);


        [TestMethod]
        public void t_20260309_ListLotEDC_MPI()
        {
            var obj =FileApp.Read_SerializeJson<PagerQuery>(_log.t_20260309_ListLotEDC_MPI);
            var _r = Genesis.Library.BLL.ZZ.MPI.EDC. MPIEdcServices.ListLotEDC_MPI(obj,false,true);
        }


        [TestMethod]
        public void t_20260305_Data_Query()
        {
            var link_SID = "GTI25091715193569491"
                //"GTI26030311120735182"
                ;


            var z = Genesis.Library.BLL.ZZ.MPI.EDC.MPIEdcServices.Data_Query(link_SID, true);
            FileApp._tmpJson(z);
        }


        [TestMethod]
        public void t_20260223_edc_offline_pre()
        {
            //z.URL = _MPIApiServices.ApiName.EdcOffline.Pre;
            //var requestSet = _MPIApiServices.GenerateRequestSet(_MPIApiServices.ApiName.EdcOffline, new { }, z);
            //var r = _MPIApiServices.CallAPI(requestSet);
            //var r1 = Json.ToObject<ApiResponse_Edc>(r.Result.Data);
        }


        public class z1
        {
            public string SampleId { get; set; }
            public string Data { get; set; }
        }
        Dictionary<string, List<z1>> z2;


        [TestMethod]
        public void t_20251222_dbLock測試()
        => _DBTest((txn) =>
        {
            var _lot =
            //"JK_TEST_LAYER-01"
            "JK_GENERAL_TEST_03-03"
            ;
            var lot = txn.GetLotInfo(_lot, isQueryByLotNO: true);
            var z = lot.SUB_ROUTE_KEY_PATH;
            var _wp_lot = txn.EFQuery_MES.WP_LOT.Where(c => c.LOT == _lot).FirstOrDefault();


            var zz = txn.EFQuery_MES.WP_LOT.Where(c => c.LOT == "20251211001-02").FirstOrDefault();
            zz.QUANTITY++;
            txn.EFQuery_MES.SaveChanges();
            var x = 0;
        }, true);


        [TestMethod]
        public void t_20260108_Get_EdcInfo()
        {
            var t = _svc_EDC.Get_EdcInfo("JK_GENERAL_TEST_03-03", true);
            FileApp._tmpJson(t);
        }

        /// <summary>
        /// 驗證 ZZ_MPI_EDC_LOG 資料寫入 (Task 003)
        /// </summary>
        [TestMethod]
        public void t_20260505_opertask_edc_with_catch()
        => _DBTest((txn) =>
        {
            var data = FileApp.Read_SerializeJson<_svc_EDC.EdcLoadArguments>(_log.t_EdcLoadArguments);
            if (!string.IsNullOrEmpty(data.LOT))
            {
                var _lot = txn.EFQuery_MES.WP_LOT.Where(c => c.LOT == data.LOT).FirstOrDefault_CheckExists();
                data.LOT_SID = _lot.LOT_SID;
                if (data.LINE == null)
                {
                    var _wo = (from a in txn.EFQuery_MES.WP_WO
                               where a.WO_SID == _lot.WO_SID
                               select a).FirstOrDefault();
                    data.LINE = new string[] { _wo.WO_LINE_NO };
                }
                data.PART_NO = data.PART_NO ?? _lot.PARTNO;
                if (data.PARTNO_SID == null)
                {
                    var _part = txn.EFQuery_MES.PF_PARTNO.Where(c => c.PARTNO == _lot.PARTNO)
                        .FirstOrDefault_CheckExists();
                    data.PARTNO_SID = _part.PARTNO_SID;
                }
                data.ROUTE_VER_SID = data.ROUTE_VER_SID ?? _lot.ROUTE_VER_SID;

                if (!string.IsNullOrEmpty(data.ROUTE_VER_OPER_SID) && string.IsNullOrEmpty(data.OPER_SID))
                {
                    var _oper = txn.EFQuery_MES.PF_ROUTE_VER_OPER.Where(c => c.ROUTE_VER_OPER_SID == data.ROUTE_VER_OPER_SID)
                        .FirstOrDefault_CheckExists($"找不到這個 ROUTE_VER_OPER_SID ({data.ROUTE_VER_OPER_SID})", true);
                    data.OPER_SID = _oper.OPER_SID;
                }
                data.OPER_SID = data.OPER_SID ?? _lot.OPER_SID;
            }

            var rules = _svc_EDC.get_FC_EDC_TARGET_rule(txn.EFQuery_MES, data);
            if (rules.Count == 0)
            {
                txn.result.Data = new { data, rules, OperEdcCfg = new { } };
            }

            //260421 arg增加一個isApi，判斷是否為API CALL
            //260421 arg增加AttrCollection 用於產生EDC_ROUTE_TREE相關資訊
            var OperEdcCfg = _svc_EDC.OperTask_Edc(txn, rules[0], data.LOT_SID, isTest: true, attrCollections: data.AttrCollection);
            

            var _catch_sid = "GTI26042309065653135";
            var _catch = txn.EFQuery_MES.AD_CATCH_DATA.FirstOrDefault(c => c.SID == _catch_sid);

            FileApp._tmpJson(_catch);
        });

        [TestMethod]
        public void t_20250505_parse_dc_map()
        {
            var z  = FileApp._tmpJson<AD_CATCH_DATA>();

            var z1 = z.DATA.ToObject<OperEdcCfg>();

            var _idx = MPIEdcServices.parse_dc_map(z1);

            FileApp.WriteSerializeJson(_idx, FileApp.ts_Log("t_20250504_x.json"));
        }

        [TestMethod]
        public void t_20250505_bind_dc_map()
        {
            var B_Catch = FileApp._tmpJson<AD_CATCH_DATA>();
            var B_Catch_obj = B_Catch.DATA.ToObject<OperEdcCfg>();
            var B_idx = MPIEdcServices.parse_dc_map(B_Catch_obj);
            FileApp.WriteSerializeJson(B_idx, FileApp.ts_Log("t_20250505_B_idx.json"), isMult: false);

            var A = FileApp.Read_SerializeJson<OperEdcCfg>(FileApp.ts_Log("t_20250504_x.json"));
            MPIEdcServices.bind_dc_map(A, B_idx);

            FileApp.WriteSerializeJson(A, FileApp.ts_Log("t_20250505_測試寫入.json"),isMult:false);

        }

        [TestMethod]
        public void t_20251209_parseOperEdcCfg()
        {
            var r = FileApp.Read_SerializeJson<List<EdcLoadResult>>(_log.t_OperEdcCfg);
            z2 = new Dictionary<string, List<z1>>();
            foreach (var r1 in r)
            {
                foreach (var r2 in r1.data)
                {
                    var _key = r2["SampleId"];
                    foreach (var key in r2.Keys)
                    {
                        if (key != "SampleId")
                        {
                            var zz = new z1();
                            if (z2.ContainsKey(key))
                            {
                                z2[key].Add(zz);
                            }
                            else
                            {
                                z2[key] = new List<z1>() { zz };
                            }
                        }
                    }
                }
            }
            //foreach (var r1 in r) {
            //    foreach (var r2 in r1.EdcSrcs) {
            //        var edc_ver = r2.edc_ver;
            //        var _主檔 = new WP_LOT_EDC();

            //        foreach () ;
            //        var _主檔 = new WP_LOT_EDC();
            //    }
            //}
        }


        [TestMethod]
        public void t_20260226_查詢lotinfo_FROM_CURRENT_SUB_ROUTE_VER_OPER_SID()
        => _DBTest((txn) => {
            var lot = txn.GetLotInfo("BWO001", false, true);
            var z = lot.from_current_sub_route_ver_oper_sid();
        }, true);

        [TestMethod]
        public void t_排除關連對象條件重覆的問題()
        => _DBTest((txn) =>
        {
            var _r = FileApp.Read_SerializeJson<d_FC_EDC_TARGET>(_log.t_排除關連對象條件重覆的問題);

            var EDC_VER_SID = "";
            var r1 = (from a in txn.EFQuery_MES.FC_EDC_TARGET
                      where a.EDC_VER_SID == _r.EDC_VER_SID
                      select a);

            var r2 = (from a in txn.EFQuery_MES.FC_TARGET
                      where a.FROM_RULE == "EDC"
                      && r1.Any(c => c.EDC_TARGET_SID == a.FROM_RULE_SID)
                      group a by a.FROM_RULE_SID into g
                      select new
                      {
                          FROM_RULE_SID = g.Key,
                          Items = g.ToList()
                      }
            ).ToList();


            var r3 = r2.Where(t =>
                t.Items.All(c =>
                 _r.TARGET_LIST.Any(r =>
                     r.TARGET_OBJECT == c.TARGET_OBJECT &&
                     r.TARGET_SID == c.TARGET_SID
                 ))
            ).ToList();






        }, true);




        public class _q1
        {
            // 對應 g.Key (即 a.GROUP_CODE_SID)
            public string GROUP_CODE_SID { get; set; }

            // 對應 GroupCodeList = g.Select(...)
            public ZZ_EDC_GROUP_CODE GroupCodeList { get; set; }
            public EdcGroupCodeDetail GroupCodeList1 { get; set; }

            // 對應 ParameterList = g.Select(...).OrderBy(...)
            public List<FC_EDC_VER_PARAMETER> ParameterList { get; set; }
            public Dictionary<string, FC_EDC_VER_PARAMETER> ParameterList_1 { get; set; } = new Dictionary<string, FC_EDC_VER_PARAMETER>();
            public List<Dictionary<string, object>> data { get; set; } = new List<Dictionary<string, object>>();
            public List<FiledSet> columns { get; set; } = new List<FiledSet>();

            public void parse(Dictionary<string, List<string>> vLookUpTable = null)
            {
                Action _fn = () => { };
                var dataRow = new Dictionary<string, object>();
                switch (this.GroupCodeList1.SAMPLE_TYPE)
                {
                    case "生產批":
                        dataRow["Sample Id"] = "Lot";
                        break;
                    case "FreeText":
                        dataRow["Sample Id"] = "Sample001";
                        _fn = () =>
                        {
                            int i = 1;
                            int.TryParse(this.GroupCodeList1.MAXIMUM, out i);
                            for (var x = 2; x < i; x++)
                            {
                                var dataRowCopy = new Dictionary<string, object>(dataRow);
                                dataRowCopy["Sample Id"] = string.Format("Sample{0:000}", x); //"Sample001";
                                this.data.Add(dataRowCopy);
                            }
                        };
                        break;
                    case "清單":
                        dataRow["Sample Id"] = null;
                        _fn = () =>
                        {
                            var _list = new List<string>();
                            if (vLookUpTable.TryGetValue(this.GroupCodeList1.ITEM_RESOURCE, out _list))
                            {
                                for (var x = 1; x < _list.Count; x++)
                                {
                                    var dataRowCopy = new Dictionary<string, object>(dataRow);
                                    dataRowCopy["Sample Id"] = _list[x]; //"Sample001";
                                    this.data.Add(dataRowCopy);
                                }
                                this.data[0]["Sample Id"] = _list[0];
                            }
                        };


                        break;
                }

                if (this.ParameterList != null && this.ParameterList.Any())
                {
                    foreach (var param in ParameterList)
                    {
                        string sid = param.EDC_VER_PARA_SID;
                        ParameterList_1.Add(sid, param);
                        dataRow[sid] = null;
                        // 根據 DATATYPE 設定預設值
                        var _DATATYPE = param.DATATYPE;
                        var column = new FiledSet
                        {
                            title = param.PARAMETER,
                            data = sid,
                        };
                        // var column =  new Dictionary<string, object>
                        // {
                        //     { "title", param.PARAMETER },
                        //     { "field", param.EDC_VER_PARA_SID }
                        // };

                        // 根據 DATATYPE 設定對應的 type 和相關屬性
                        switch (_DATATYPE)
                        {
                            case "N": // 數值型
                                column.type = "numeric";
                                break;
                            case "B": // 布林型
                                column.type = "checkbox";
                                break;
                            case "L": // 下拉選單
                                column.type = "dropdown";
                                var _list = new List<string>();
                                if (vLookUpTable.TryGetValue(param.DB_COLUMN_NAME, out _list))
                                {
                                    column.source = _list;
                                }
                                break;
                            default:
                                break;
                        }
                        this.columns.Add(column);
                    }
                    this.data.Add(dataRow);
                }

                _fn();


            }

        }

        public class FiledSet
        {
            // 對應 "data" 欄位，通常是顯示給使用者的名稱或內部資料欄位的名稱
            public string title { get; set; }
            //public string field  { get; set; }
            public string data { get; set; }

            // 對應 "type" 欄位，指定組件類型，這裡固定為 "text"
            public string type { get; set; }

            // 對應 "source" 欄位，這是下拉式選單的選項來源清單
            public List<string> source { get; set; }
        }

        public class EdcGroupCodeDetail
        {
            // 唯一識別碼
            public string SID { get; set; }

            // EDC 主檔的 SID
            public string EDC_SID { get; set; }

            // 群組代碼名稱 (例如: 鑽孔孔徑)
            public string GROUP_CODE { get; set; }

            // 採樣類型 (例如: 生產批)
            public string SAMPLE_TYPE { get; set; }

            // 項目資源 (通常用於下拉式選單或其他資料源)
            public string ITEM_RESOURCE { get; set; } // 可能是 string 或 Nullable<int>/Nullable<decimal>，這裡假設為 string

            // 最大值或最大數量
            public string MAXIMUM { get; set; } // 假設是數字，且為 Nullable

            // EDC 版本 SID
            public string EDC_VER_SID { get; set; }
        }


        public class _FC_EDC_VER_PARAMETER : FC_EDC_VER_PARAMETER
        {
            public string SAMPLE_TYPE { get; set; }
            public string ITEM_RESOURCE { get; set; }
            public string MAXIMUM { get; set; }
            public string API_ROUTE { get; set; }
        }

    }
}

