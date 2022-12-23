using BLL.Base;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Genesis.Common;
//using Genesis.Hubs;
using Genesis.WebApi;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Genesis
{
AA
    public class GTI_Test : IGTI_Test
    {
        public static bool isTest { get; set; }

        static bool? _IsDebuggingEnabled;
        public static bool IsDebuggingEnabled
        {
            get
            {
                try
                {
                    if (_IsDebuggingEnabled == null)
                        _IsDebuggingEnabled = HttpContext.Current.IsDebuggingEnabled;
                }
                catch
                {
                    _IsDebuggingEnabled = false;
                }
                return (bool)_IsDebuggingEnabled;
            }
        }
        HtmlHelper htm;
        public GTI_Test(HtmlHelper htm, HttpContextBase HttpCon, string[] list = null)
        {
            init(htm, GTI_Test.IsDebuggingEnabled, HttpCon.Request, list);
        }
        public GTI_Test(HtmlHelper htm, bool IsDebuggingEnabled, HttpRequestBase Req = null, string[] list = null)
        {
            init(htm, IsDebuggingEnabled, Req, list);
        }

        private void init(HtmlHelper htm, bool IsDebuggingEnabled, HttpRequestBase Req, string[] list)
        {
            this.htm = htm;
            if (IsDebuggingEnabled)
            {
                List<string> _base = new List<string>() { "__gt_test:Vue.prototype.$GTI_Test.__gt_test," };
                this.mounted = htm.Raw("window.__vm = this;");
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        _base.Add($"{item}:Vue.prototype.$GTI_Test.{item},");
                    }
                }
                var _methods = string.Join("", _base.ToArray());
                this.methods = htm.Raw(_methods);

                if (Req != null)
                {
                    isTest = Req.Params["Test"] == "T";
                    if (isTest)
                    {
                        ServicesBase.isTest = true;
                        Debug = "debugger;";
                    }
                }
            }
            else
            {
                this.mounted
                    = this.methods
                    = htm.Raw("");
            }
        }

        public static string UItest = null;

        internal static bool isUItest(HttpRequestBase Req)
        {
            if (Req != null)
            {
                UItest = Req.Params["UItest"] ?? "";
            }
            return UItest != "";
        }
        public IHtmlString param_UItest(string key = "dataModel")
        {
            var _code = "";
            if (UItest != "")
            {
                string Baseurl = $"http://localhost:59394/GenesisNewMes/Example/Self/UITest?name={UItest}";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var r = client.GetAsync(Baseurl).Result;
                var res = r.Content.ReadAsStringAsync().Result;
                //var _o = JsonConvert.DeserializeObject(res);// res.Replace("\"", "");
                _code = $"{key} = {res}";//JSON.parse({_o.ToJson()});
            }
            return Test(_code, 1);
        }
        public static T param_UItest1<T>()
        {
            T _obj = default(T);
            var _code = "";
            if (UItest != "")
            {
                string Baseurl = $"http://localhost:59394/GenesisNewMes/Example/Self/UITest?name={UItest}";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var r = client.GetAsync(Baseurl).Result;
                var res = r.Content.ReadAsStringAsync().Result;
                _obj = res.ToObject<T>();
            }
            return _obj;
        }

        public IHtmlString mounted { get; set; }
        public IHtmlString methods { get; set; }


        public string Debug { get; set; } = "";
        public IHtmlString param_test { get { return Test("param.isTest='T';"); } }


        public IHtmlString Test(string code, int mode = 0)
        {
            if (mode != 0 && GTI_Test.IsDebuggingEnabled) return htm.Raw(code);
            return htm.Raw(isTest ? code : "");
        }


    }
    //todo-bk
    /*
    MapFunName
    Connection_PD_GTIMES
    CarrierCheckIn.cshtml
    CarrierCheckOut.cshtml
    */

    public struct GT_Client
    {
        //public static string basePath(string key) { return $"~/Areas/Example/Views/Code/{key}.cshtml"; }

        public struct comm_Vue
        {
            /* mixins: [reason_obj],
            v_rootEl , v_model
            mixins: [Vue.prototype.$UT.Mixins.Info()],
            */
            public static int mixins;
            // Vue.config.devtools = true;
            public static int devtools;

            /*
            父物件) H:\HM_Dev\Genesis_MVC\Areas\Example\Views\Self\InOut\useOperPartSplitSubLot.cshtml
                provide() {
                    return {
                        rootEl: this,
                    };
                },
            子物件) H:\HM_Dev\Genesis_MVC\Areas\MES\Views\WIP\Partial2\_useOperPartSplitSubLot.cshtml
                let { v_model, v_rootEl } = Vue.prototype.$UT.Mixins;
                mixins: [v_rootEl()],
            */
            public static int v_rootEl;

            /*
            value: {
                deep: true,
                immediate: false,
                handler(val) {
                    this.table_list = val;
                    var total = 0;
                    $.each(this.table_list , function (index, value) {
                        total += parseFloat(value.INum);
                    });
                    this.defectTotal = total.toFixed(this.set_model.systemConfig.DecimalPoint);
                }
            }, 
            */
            public static int watch;

            /*
             <component:is="currentView"></component>
             */
            public static int component;
        }

        public struct Reazer
        {
            /*
            let { Data,Success,Message } = @Html.Raw(ViewData["result"] ?? "{}");
            let { Wafers = [], LotInfo = {} } = Data || {};

            --for _View 版本的方法
            let { Data,Success,Message } = @Html.Raw(ViewData["result"])|| {};
            var ddl_reason = @Html.Raw(ViewData["ddl_reason"] ?? "{}");
            var ddl_reason = @Html.Raw(Json.Encode(BLL.Base.MESServices.GetFunReason("LotBonus")));
            var srcGrade = @Html.Raw(ViewData["srcGrade"] ?? "[]");

            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\Example\Views\Act\InOut\~posiMapEdcGrade.cshtml
            @using Frame.Code
            @Html.Raw(BLL.MES.DDLServices.ddl_Parameter("Grade").ToJson(true))
            var srcData = @Html.Raw(((object)BLL.MES.Wafer_Services.QueryLotInfo(null, "test001-01.03").Data).ToJson(true));
            */
            public static int result;


            /*
            @{
                var action = ViewContext.RouteData.Values["action"].ToString();
                var controller = ViewContext.RouteData.Values["controller"].ToString().ToUpper();
                GTI_Test GTest = new GTI_Test(Html, ViewContext.HttpContext);
                ViewBag.Title = Genesis.Common.PageBaseInfo.getCurrentResource(action.ToString()).RESOURCE_NAME;
                bool isSingleModel = (bool)ViewData["SingleModel"];
                if (isSingleModel)
                {
                    Layout = "~/Views/Shared/_LayoutSingle.cshtml";
                }
                else
                {
                    <h3>@ViewBag.Title</h3>
                }
            }
            @Html.AntiForgeryToken()
            @eBundle.QRender_CSS()
            @eBundle.QRender_JS()
            @Scripts.Render("~/bundles/parsley")

                                
            */
            public static int Head;

            /*
            //Ajax 語法 
            url: "@Url.Action($"Exec{action}")",
            [Get]
                Query() {
                    let { search, ROOT_SHIP_CASSETTE } = this;
                    var param = { search, ROOT_SHIP_CASSETTE};
                    var _ajax = {
                        url: "@Genesis.Common.PageBaseInfo.GetBaseUrl()/api/info/wafer_shipinig_package",
                        type: 'get',
                        automessage: false,
                        param,
                        success: this.QuerySuccess
                    };
                    $.submitForm(_ajax);
                }, 
            [Post]
                var param = { WafersPackage };
                param.isTest = 'T';
                var _ajax = {
                    url: "@Genesis.Common.PageBaseInfo.GetBaseUrl()/api/info/SHIP_CASSETTE",
                    type: 'post',
                    automessage: true,
                    param,
                    success(res) {
                        let { Data } = res;
                        _.each(Data,el => {
                            let { SERIAL_NUMBER_ID, SERIAL_NUMBER, ROOT_LOT_SID, PARENT_LOT_SID } = el;
                            var _map = _.filter(_self.WafersPackage, { SERIAL_NUMBER_ID });
                            if (_map) {
                                _map[0].ROOT_LOT_SID = SERIAL_NUMBER;
                                _map[0].PARENT_LOT_SID = PARENT_LOT_SID;
                                    
                            }
                        })
                        console.log(Data);
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        debugger;
                        alert(XMLHttpRequest.status);
                        alert(XMLHttpRequest.readyState);
                        alert(textStatus);
                    },
                };
                $.submitForm(_ajax);

             */
            public static int QueryAPI;

            /*
            H:\HM_Dev\Genesis_MVC\Areas\MES\Views\WIP\Partial2\_useOperPartSplitSubLot.cshtml
            title: i18next.t("DataHaveExistError", { ns: "message" }).format(checkNo),
             */
            public static int I18n;

            public static int Test;



        }

        public struct Reazer_Partial
        {
            /*
            H:\HM_Dev\Genesis_MVC\Areas\WIP\Views\Lot\_Common.cshtml
            @Html.Partial("~/Areas/WIP/Views/Shared/_lotinfo.cshtml")
            
            <el-tab-pane :label="c_LotInfo" v-if="!isBatchMode">
                <x-lot-info ref="lotinfo" :lotinfo="CurrentLot" :hide="c_hide"></x-lot-info>
            </el-tab-pane>

            c_hide() {
                return ['OWNER', 'PRIORITY', 'LOT_TYPE'];
            },
             */
            public static int lotinfo;


            /*
               注意: render_sty="GTIMES_SID" 不會有作用
               @Html.Partial("~/Areas/WIP/Views/Shared/_reason.cshtml")
               <div class="col-lg-6 col-md-12 adj">
                   <x-reason v-model="reason"
                               check_options="true"
                               :description.sync="description"
                               :options="ddl_reason"
                               :selectize_ops="selectize_ops"
                               >
                   </x-reason>
               </div>

               reason: '',
               description: '',
               ddl_reason: '',

               selectize_ops: {
                   valueField: 'SID',
                   searchField: ['Display', 'No'],
               }
           */
            public static int Reason;
        }

        public struct comm_Html
        {
            /*
            <div class="panel">
                <div class="panel-body form-horizontal gt-form">
                </div>
            </div>
             */
            public static int gt_form;

            /*
            <gt-hr content="" :active="false" />
            <gt-hr content="" :active="false" ><br /></gt-hr>

             */
            public static int gt_hr;


            /*
            H:\HM_Dev\Genesis_MVC\Areas\Example\Views\Act\UI_SPEC_02.cshtml
            <el-table-column label="@RES.BLL.Face.UPDATE_DATE">
			    <template slot-scope="scope">
				    <el-tag type="success" size="mini">{{scope.row.UPDATE_USER}}</el-tag>
				    <gt-split-data v-model="scope.row.UPDATE_DATE"></gt-split-data>
			    </template>
		    </el-table-column>
             */
            public static int gt_split_data;
        }
        public struct lodash
        {
            /*
            H:\HM_Dev\Genesis_MVC\Areas\MES\Views\WIP\Partial2\_posiMapEDC.cshtml
                _.get(this, '$store.state.isChange', false);
             */
            public static int get;

        }
        public struct comm_JS
        {
            /*
			https://social.msdn.microsoft.com/Forums/en-US/e39f487a-a70e-44ec-860c-9666a7581a4e/how-to-send-a-dictionaryltstringmodelgt-via-ajax-to-controller-in-asp-mvc?forum=aspmvc
			url: 'testButton',
            type: 'POST',
            data: $("#createEventForm").serialize() + "&" + jQuery.param({ giftList: giftList }),$("#createEventForm").serialize() + "&" + jQuery.param({ giftList: giftList }),
			 
			QueryLotInfo(LOT) {
                var _self = this;
                var url = `@Url.Action(nameof(WaferController.Exchange_LotInfo))`;
                if (url == null) return;
                var _ajax = {
                    url,
                    type: 'get',
                    param: {LOT},
                    automessage: false,
                    success: _self.QueryLotInfo_Success
                };
                $.submitForm(_ajax);
            },
			QueryLotInfo_Success(res){
			let { Success, Message, Data } = res
                if (Success) {
                    let { lotinfo } = Data;
                    this.GradeList = list;
                } else {
                    this.$Alert.Err(Message).then(() => {
                        this.LOT = "";
                        this.CarrierNo = "";
                    });
                }
			} 
			*/
            public static string submitForm;

            /*
            Object.defineProperty(exports, "__esModule", { value: true });
            Object.defineProperty(src, 'sts', {
                get() {
                    let { STATUS } = this;
                    STATUS = STATUS ?? "Normal";
                    return STATUS == "Normal"?"0":"1";
                },
                set(val) {
                    var def = "Normal";
                    if (val == "1") def = "Scrap";
                    this.STATUS = def;
                }
            });
            */
            public static int defineProperty;

            /*
            取消執行
            e.preventDefault 
             */
            public static int Event;
        }
        public struct elUI
        {
            public struct Table
            {
                /*
                <el-table empty-text=" "
                          style="width: 100%"
                          :data="grid.Lots">
                    <el-table-column prop="LOT"
                                     label="@Face.Lot"
                                     width="200">
                    </el-table-column>
                    <el-table-column prop="STATUS"
                                     label="@RES.BLL.Face.Status">
                    </el-table-column>
                    <el-table-column prop="PARTNO"
                                     label="@RES.BLL.Face.PartNo"
                                     width="180">
                    </el-table-column>
                    <el-table-column prop="OPERATION"
                                     label="@RES.BLL.Face.OPERATION"
                                     width="180">
                    </el-table-column>
                    <el-table-column prop="WO"
                                     label="@RES.BLL.Face.WO"
                                     width="180">
                    </el-table-column>
                    <el-table-column prop="PRODUCT"
                                     label="@RES.BLL.Face.PRODUCT"
                                     width="180">
                    </el-table-column>

                    <el-table-column prop="QUANTITY"
                                     label="@RES.BLL.Face.QUANTITY">
                    </el-table-column>
                </el-table> 
                */
                public static string html_LOT = "el_Table/Html_LOT";
                /*
                grid: {
                    data: [],
					pageSize: 10,
					pageIdx: 1,
					row_count: 0,
					Sort: {
						Name: 'CREATE_DATE',
						isAsc: true
					},
					Conditions: {},
					Page: {
						Index: 1,
						Size: 10,
					},
					get query_rule() {
						{
							let { Conditions, Sort, Page } = this;
							return {
								Conditions, Sort, Page
							}
						}
					}
				}, 
                 
                */
                public static string html_LOT_vData = "el_Table/html_LOT_vData";
                /*
                DelRow(idx, row) {
                    var _self = this;
                    _self.tableData.splice(idx, 1);
                },
                */
                public static int DelRow;
                /*
                <el-table :data="tableData">
                    <el-table-column label="@RES.BLL.Face.gvAction"
                                     width="60">
                        <template slot-scope="scope">
                            <el-button size="small" circle
                                       type="danger"
                                       icon="el-icon-delete"
                                       @@click="DelRow(scope.$index, scope.row)"></el-button>
                        </template>
                    </el-table-column>
                    <el-table-column prop="SERIAL_NUMBER_ID"
                                     label="WaferSN"></el-table-column>
                    <el-table-column prop="LOT"
                                     label="@Face.Lot"></el-table-column>
                </el-table>
                tableData:''
                */
                public static int Html_Del;

                /*
                <template slot="header" slot-scope="scope">
                    <el-input
                      v-model="search"
                      size="mini"
                      placeholder="输入关键字搜索"/>
                </template>
                */
                public static int ColHead;

                /*
                H:\GTiMES5.1_Dev\Genesis_MVC\Areas\MES\Views\WIP\Partial2\_zz_hm_package.cshtml
                <el-table
                    :data="c_val"
                    style="width: 100%;margin-bottom: 20px;"
                    :row-key="GetRowKeys"
                    border
                    default-expand-all
                    :tree-props="{children: 'items'}">
                    <template slot-scope="scope" >
                        <!--第一筆  和 子層不顯示 -->
                        <el-button size="small" circle v-if="scope.row.isRoot && scope.$index!=0"
                                    type="danger"
                                    icon="el-icon-delete"
                                    @@click="DelRow(scope.$index, scope.row)"></el-button>
                    </template>
                </el-table>

                GetRowKeys(row) {
                    let { SHIP_CASSETTE='',LOT } = row;
                    return `${SHIP_CASSETTE}${LOT}`;
                },
                DelRow(idx, row) {
                    var isRoot = row.LOT == '';
                    if (isRoot) {
                        this.c_val.splice(idx, 1);
                    } else {
                        let { SHIP_CASSETTE,LOT } = row;
                        var target = null;
                        _.some(this.value, el => {
                            var isSome = el.SHIP_CASSETTE === SHIP_CASSETTE;
                            if (isSome) target = el;
                            return isSome;
                        })
                        if (target) {
                            target.QUANTITY -= row.QUANTITY;
                            _.remove(target.children, function (n) {
                                return n.LOT == LOT;
                            })
                            //必須得加這一段 ,解決畫面沒有即時刷新的問題
                            $(event.srcElement).parents('tr').hide();
                        } 
                    }
                },


                //折疊符號 放大並往右浮動
                .el-table__expand-icon {
                    margin: 5px 7px !important;
                    color: #409EFF;
                    font-size: 2.5rem;
                    float: right;
                }
                */
                public static int case_階層;
            }

            public struct Tabs
            {
                /*
                <el-tabs type="border-card">
                    <el-tab-pane label="@Face.LotInfo">
                    </el-tab-pane>
                </el-tabs>
                */
                public static int Base;
            }

            /*
            this.$confirm('是否一併刪除批號記錄?', '提示', {
                type: 'warning'
            }).then(() => {
                
            }).catch(() => {
                
            });
            */
            public static int confirm;
        }

        public struct gt_form_col
        {
            /*
            <div class="panel b">
                <div class="panel-body form-horizontal gt-form">
                    <div class="col-lg-6 col-md-12 adj">
                        <gt-form-col label="@Face.OPERATION" required col_sty="col-lg-12">
                        </gt-form-col>
                    </div>
                </div>
            </div>
            */
            public static int 雙欄佈局;

            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\WIP\Views\Lot\_Common.cshtml
            col_sty="col-lg-12" -- 單邊一欄模式
            <gt-form-col label="@Face.OPERATION" required >
                <input class="form-control" v-if="c_SN_readonly"
                        v-model="c_SN"  
                        v-on:keyup.enter="act_query(c_val)"
                        />
                <div class="form-control" readonly v-if="!c_SN_readonly">
                    <span class="label label-primary">{{Lot_0.ROUTE_VER_OPER_SID}}</span> {{Lot_0.OPERATION}}
                </div>
            </gt-form-col>
            */
            public static int 擬selectize;


            /*
             <span class="label label-primary">{{Lot_0.ROUTE_VER_OPER_SID}}</span> {{Lot_0.OPERATION}}
             */
            public static int 唯讀Tag;

            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\SPC\Views\ControlChart\Data.cshtml
            <gt-form-col label="@Face.Keyword"
                                     @@icon_search_click="fnSearchUsers"
                                     :readonly="c_switch_enable">
            */
            public static string Case1 = "";

            /*
            <gt-form-col label="@RES.BLL.Face.TotalReleaseQty" readonly>
                <div class="form-control" readonly v-if="!c_SN_readonly">
                    <gt-tag-version v-model="operInfo"></gt-tag-version>
                </div>
            </gt-form-col> 
             
            */
            public static string gt_tag_version = "";
        }
        public struct v_selectize
        {
            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\WIP\Views\FutureHold\HoldData.cshtml
            <vue-selectize v-model="ddl_demo.val" required
	            :options="ddl_demo.src" render_sty="GTIMES_SID"
	            :rows.sync="ddl_demo.rows"
	            :auto_drowdown="true">
	            </vue-selectize> 
             
            ddl_demo: {
                val: '',
                src: [
                    { SID: 'A', No: 'A', Display: 'A規' },
                    { SID: 'B', No: 'B', Display: 'B規' },
                    { SID: 'C', No: 'C', Display: 'C規' },
                    { SID: 'G', No: 'G', Display: 'Golden' },
                    { SID: 'S', No: 'S', Display: '報廢品' },
                ],
                rows:[],
                get readonly() {
                    let [Row0 = {}] = this.src;
                    let { No = "", Display = "" } = Row0;
                    return { No, Display };
                }
            },
             */
            public static string basic = "v_selectize/basic";
            public static string basic_vData = @"v_selectize/basic_vData";

            /*
            onChange(_new, _old) {
                debugger
            },
            onFocus() {
                debugger
            } 
            */
            public static int ops_evnet;

            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\WIP\Views\FutureHold\HoldData.cshtml
            reason_group: {
                            onChange(val) {
                                _self.form.REASONGROUP_SID = val;
                                if (val == "") {
                                    _self.reason.value = "";
                                    _self.reason.rows = [];
                                }
                            },
                            value: form.REASONGROUP_SID,
                            ops: ddl_reason_group,

                        },
                        reason: {
                            value: form.REASON_NO,
                            ops: ddl_reason,
                            rows:[]
                        },
                        description:'',
             */
            public static int vue_selectize_reason;
            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\WIP\Views\Lot\ChangeRoute.cshtml
            vue-selectize-router 
             vue-selectize-dynquery
            gt-query-lot 

             */

            public static int vue_selectize_Case;
            /*
            搭配 parsley 的應用案例
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\MES\Views\WIP\Partial2\_selectUsers.cshtml
            data-parsley-error-message
            */

        }

        public struct gt_toolbar
        {
            /*
			<gt-toolbar :e_exec="Exec" :e_clear="Clear_t"> </gt-toolbar>

            <gt-toolbar :e_save="c_save"
                    :e_del="c_delete"
                    :e_copy="c_copy"
                    :enable.sync="c_switch_enable"
                    :visible="!c_isAddMode"
                    fixed="top"
                    :is_change="c_is_change">
                    </gt-toolbar>

            c_delete() {
                return {
                    visable: !this.c_isAddMode,
                    enable: !this.c_switch_enable,
                    fn: this.fnDelete
                }
            },
			*/
            public static int Html;
            /*
            
            c_Exec() {
                return {
                    enable: !this.c_Exec,
                    fn: this.Exec
                }
            },
			Exec() {
                var _self = this;
                var data = _self.parse_Data();
                var param = {
                    WaferSN: _self.SN,
                    NewGarde: _self.ddl_Grade.val,
                    ReasonNo: _self.reason,
                    Desc: _self.description
                }
                var _ajax = {
                    url: "@Url.Action(nameof(WaferController.ExecExchange))",
                    type:'post',
                    param,
                    success(RES) {
                        let { Success, Data = {}, Message} = res;
                        if (Success) {
                            var msg = '@RES.BLL.Message.NormalSuccessful'.replace('{0}', "@ViewBag.Title");
                            _self.$Alert.msg_ExecSuccessful(msg);
                        } else {
                            _self.$Alert.Err(Message);
                        }
                    }
                };
                $.submitForm(_ajax);
            },
			parse_Data() {
                var arr = [];
                _.each(this.list, (el) => {
                    let { ChangeTo } = el;
                    if (ChangeTo != null) {
                        var _obj = _.merge({}, el);
                        arr.push(_obj);
                    }
                })
                return arr;
            },
			*/
            public static int Exec;
            /*
            Clear() {
                location.replace('@Url.Action(nameof(WaferController.Exchange))');
            },
            Clear_t() {
                    setTimeout(() => {
                        var d = [
                            {
                                "element": ".gt-form",
                                "popover": {
                                    "title": "輸入方式",
                                    "description": "可以選擇 [載具編號] 或 [批號] 執行查詢,查詢方式為 輸入完整資訊後,按 Enter "
                                },
                            },
                            {
                                "element": ".fa.fa-repeat",
                                "popover": {
                                    "title": "輸入方式",
                                    "description": "可以選擇 [載具編號] 或 [批號] 執行查詢,查詢方式為 輸入完整資訊後,按 Enter "
                                },
                            }];
                        var driver = new Driver();
                        driver.defineSteps(d);
                        driver.start();
                    }, 500);
                },
            */
            public static int Clear;
        }

        /*
        <gt-input-number label="gt-form 模式"></gt-input-number>
		<gt-form-col label="非 gt-form 模式">
			<gt-input-number root_tag="div"></gt-input-number>
		</gt-form-col> 
         
        */
        public static int gt_input_number;

        public struct gt_query
        {
            /*
			<gt-query label="@Face.Lot" class="col-lg-6 col-md-12 adj"
                    placeholder="@RES.BLL.Message.PleaseScanTheBarcodeOrUseKeywords"
                    v-model="LotNo"
                    :query_func="QueryLotInfo"
                    query_mode=1
                    :readonly="c_readonly"
                    prompt_sty="has-req"
                    :check_repeat=""
                    :check_repeat_key=""
                    ></gt-query>
			LotNo:''
			c_LotNo(){
				get(){ return this.LotNo}
				set(val){
					this.LotNo = val;
				}
			}
			c_readonly() {
                return true;
            },
			*/
            public static int M1;
        }
    }

    public struct GT_Server
    {
        public struct API
        {
            /*
            http://localhost:59394/GenesisNewMes/api/ddl/Reason?funName=LotChangeRoute
            {
                "RefKey": null,
                "SID": "GTI11110115550180956",
                "No": "other",
                "Value": "GTI11110115550180956",
                "Display": "其他",
                "StatusSid": null,
                "Status": null,
                "FromNum": 0,
                "INum": 0,
                "Attr01": null,
                "Attr02": null,
                "Attr03": null
            }
            */
            public static int Reason;


            /*
            http://localhost:59394/GenesisNewMes/api/ddl/Parameter?ParameterNo=grade
            [Route("api/ddl/Parameter")]
		    [HttpGet]
		    public List<SelectModel> Parameter(string ParameterNo)
		    => DDLServices.ddl_Parameter(ParameterNo);
            */
            public static int Parameter;

        }
        public struct Controller
        {
            /*
            =>_View(SingleModel, "WaferModifyGrade",()=>{ 
                if (WaferSN != null) return serv.ModifyGrade_Query(WaferSN);
                return null;
            }) 
            //直接寫在 subFun 段內也可以
            ViewData["ddl_reason"] = MESServices.GetFunReason(ReasonFunction)
            ViewData["srcGrade"] = DDLServices.ddl_Parameter("Grade").ToJson(true);
            */
            public static int _View;

            /*
            =>_Content(o=>o);
            =>_Content(Func);
            =>_Content(Func,{文字類型參數});
		    =>_Content((o) => new Result(true)); 
            */
            public static int _Content;

            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\ADM\Controllers\ShipCassetteController.cs

            */
            public static int MasterDetail;

            /*
            [HttpPost]
            [HandlerAjaxOnly]
            public ActionResult QueryList(PagerQuery pager)
            => _Content(o => serv.QueryList(pager));
            */
            public static int QueryList;


            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\WIP\Controllers\StandadHourController.cs
            AbsCommonController<IStandardHour>
            */
            public static int AbsCommonController;


            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\App_Start\BundleConfig~.cs
            [RoutePrefix("DDD/ADM")]
            [Route("Reason")]
		    [Route("Reason/{FUN_NAME}")]
		    [Route("Reason/{FUN_NAME}/{FUN_SID}")]


            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\MES\Controllers\DashboardController.cs
            [AllowAnonymous]

            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\MES\Controllers\WIPController.cs
            [Common.HandlerAjaxOnly]
		    [ValidateAntiForgeryToken]

            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\WIP\Controllers\CarrierLoadLot_1Controller.cs
            [LotFilterAction]
            */
            public static int attrib;

            public struct Case
            {
                /*
                 H:\HM_Dev\Library\BLL\Base\ServicesBase.cs
                GetCustomerView
                 */
                /// <summary>
                /// 根據專案名稱取得客製頁面
                /// </summary>
                public static int GetCustomerView;

                /*
                H:\HM_Dev\Library\BLL\MES\WIPInfoServices.cs
                GetEdcHist
                var dataRange = new Base.DataRange(edcHistDate);
                 */
                /// <summary>
                /// 取得日期區間
                /// </summary>
                public static int DataRange;
            }
        }
        public struct Service
        {
            public struct Txn
            {
                /*
                => TxnBase.DBQuery(Txn => {
                }); 

                public static List<SelectModel> ddl_Parameter(string ParameterNo) 
                =>TxnBase.DBQuery(Txn =>{
			        var _d = new List<SelectModel>();
                    Txn.result.Data = _d;
	        		return Txn.result;
                }).Data;
                */
                public static int DBQuery;

                /*
                適用 交易寫入相關程序
                A)基本型
                => TxnBase.Process("WaferCarrierLoad", Txn =>
                {});
                
                A)check_fn 為檢核程序 , 如果不檢查 直接傳入 null , 如果檢核失敗就直接回傳,不會再進入交易程序 
                    檢核程序的回傳值如果需要取用,可參考 _d 的處理方法
                => TxnBase.Process("WaferCarrierLoad", check_fn(CarrierNo), Txn => {
                    var _d = txn.result.parseData<d_ModifyGrade_Query>();
                })

                B)增加一個 處理 Reason 的相關程序
                => TxnBase.Process("WaferCarrierLoad",TxnReason.n(ReasonNo, Desc), check_fn(CarrierNo), Txn =>{})
                */
                public static int Process;

                /*
                P:\MyLab\UnitTest\GTI\t_DB.cs
                Txn.EFQuery<WP_LOT_WAFER_MAPPING>().GetByID("GTI22031515092145349");
                
                txn.EFQuery<WP_LOT_WAFER_MAPPING>().Reads(c =>
                         c.SERIAL_STATUS == "Normal"
                         //&& string.IsNullOrEmpty(c.GRADE) == false
                         && ((c.SERIAL_NUMBER_ID == WaferSN)
                             || c.LOT == LotNo)
                    ).ToList()
                */
                public static int EFQuery;

                /*
                TxnBase.isTest(test?.Wafers
                , txn.EFQuery<WP_LOT_WAFER_MAPPING>().Reads(c =>
                         c.SERIAL_STATUS == "Normal"
                         //&& string.IsNullOrEmpty(c.GRADE) == false
                         && ((c.SERIAL_NUMBER_ID == WaferSN)
                             || c.LOT == LotNo)
                    ).ToList()) 
                */
                public static int isTest;

                /*
                InsertCommandBuilder insert = new InsertCommandBuilder(DBC, "WP_LOT_WAFER_MAPPING")
                .InsertColumn("WAFER_MAPPING_SID", DBC.GetSID())
                ;
                 */
                public static int InsertCommandBuilder;
            }
            public struct Txn過站
            {
                /*
                **因為遷就於 LotUtility.LotScrapCreateInfo 的處理方式 , 所以一定只能使用 Reason_SID 做處理,不能使用 ReasonNo
                H:\GTiMES5.1_Dev\Library\BLL\MES\WIP\LOT_Services\LOT_Services.Scrap.cs
                var _list = new List<QtyItem>() { new QtyItem() { Qty = 1, Reason_SID = item.Value.REASON_SID } };
                var SplitLots = LOT_Services.Txn_Scrap(Txn, _list);
                */
                public static int Scrap;

                /*
                H:\GTiMES5.1_Dev\Library\BLL\MES\WIPServies.WAFER_MAPING.cs
                var _lot = new LotInfo(txnInfo.DBC, SplitInfoList[0].LOT_SID, LotUtility.IndexType.SID);
                Txn.DoTransaction
                    (new WIPTransaction.HoldLotTxn(_lot)
                    , new WIPTransaction.EndOfLotTxn(_lot)); 
                 */
                public static int Hold;

                /*
                H:\GTiMES5.1_Dev\Library\BLL\MES\WIPServies.cs
                SplitLots 

                H:\GTiMES5.1_Dev\Library\BLL\MES\WIP\LOT_Services\LOT_Services.Split.cs
                Txn_Split
                var Txn_Split(_TxnBase, item)
                */
                public static int Split;

            }

            /*
            public IResult QueryList(PagerQuery pagerQuery)
            {
                return QueryList(pagerQuery, zzShipCassettleServices.GetAllListIQueryable());
            } 

            public IResult QueryList(PagerQuery pagerQuery)
            => TxnBase.DBQuery(Txn => {
                return QueryList(pagerQuery, Txn.EFQuery<ZZ_SHIP_CASSETTE>().Reads());
            });
            */
            public static int QueryList;
        }

        public struct MES
        {
            /*
            GetLotInfoCheck
            WIPOperConfigServices.GetOperEdc(dbc, _lotInfo, RouteVerOperInfo);
            var RouteVerOperInfo = new RouteUtility.RouteVerOperationInfo(dbc, _lotInfo.ROUTE_VER_OPER_SID, RouteUtility.IndexType.SID);
            */
            public static int EDC;

            /*
            H:\GTiMES5.1_Dev\Library\BLL\MES\WIP\LOT_Services\LOT_Services.cs
            public static bool Hold_STATUS(string STATUS)
            {
                var status = WIP_Exten.Trans_STATUS
                    (LotStatus.Wait,
                    LotStatus.Create,
                    LotStatus.Hold);
                return status.Contains(STATUS);

            }
             
             */
            public static int Status;
        }

        public struct Linq
        {
            public struct GroupBy_ToDictionary
            {

                /*
                 
                */
                public static int basic;
            }
        }

        /*
        _DBTest 
        */
        public struct UTest
        {
            /*
            一般 DB 查詢 不需要加上 true
            但如果是需要 Rollback 的交易測試,就加上 true 
            [TestMethod]
            public void t_()
            => _DBTest(Txn =>{

		    },true);   
             */
            public static int _DBTest;

            /*
            FileApp.WriteSerializeJson(r, _log.t_CarrierInfo);
            */
            public static int WriteLog;

            /*
            var _d = FileApp.Read_SerializeJson<d_CommandWaferInfo>(_log.t_ModifyGrade_Query());

			var sn = _d.Wafers[0].SERIAL_NUMBER_ID;
			var _r1 = serv.ModifyGrade_Query(sn,test:_d);
			Assert.IsTrue(_r1.Success
				, "正常測試,應回傳 true");

			_d.WP_LOT.STATUS = "Hold";
			var _r2 = serv.ModifyGrade_Query(sn, test: _d);
			Assert.IsTrue(_r2.Success == false 
				, "當批號狀態不為Wait,應回傳 false"); 
            */
            public static int Case;
        }
    }

    public static class GTCode
    {
        public static MvcHtmlString GTICode(this HtmlHelper Html, string path)
        {
            var _path = $"~/Areas/Example/Views/GTICode/{path}.cshtml";
            return Html.Partial(_path);
        }
        /// <summary>
        /// 以文字 方式呈現
        /// </summary>
        /// <param name="src"></param>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static IHtmlString t(this string src, HtmlHelper Html)
        {
            return Html.Raw(src);
        }

        /// <summary>
        /// 以路徑 做 Partial
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static IHtmlString p(this string path, HtmlHelper Html)
        {
            var _path = $"~/Areas/Example/Views/GTICode/{path}.cshtml";
            return Html.Partial(_path);
        }

        /// <summary>
        /// 單純只是 Mark
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string m(this int src, HtmlHelper Html = null)
        {
            return "";
        }
        public static string m(this string src, HtmlHelper Html = null)
        {
            return "";
        }
    }

}
namespace Genesis.Areas.Example.Controllers
{
    public partial class SelfController : BaseController
    {
        [AllowAnonymous]
        public ActionResult test(string name, bool SingleModel = true)
        {
            dynamic data = new ExpandoObject();
            ViewData["SingleModel"] = SingleModel;
            return View(name);
        }

        [AllowAnonymous]
        /// <summary>
        /// 針對 進出站的控件做 測試
        /// </summary>
        /// <param name="name"></param>
        /// <param name="SingleModel"></param>
        /// <returns></returns>
        public ActionResult InOut(string name, string Case = "0", bool SingleModel = false)
        {
            ViewData["SingleModel"] = SingleModel;
            //var _data = ControllerContext.HttpContext.Server.MapPath($"../../Areas/example/Views/Act/InOut/~Case{Case}.json");
            //string text = System.IO.File.ReadAllText(_data);
            //ViewData["result"] = JsonConvert.DeserializeObject(text);

            return View($"InOut/{name}", new LotData());
        }

        public ActionResult SignalR_Item(string id = null)
        {
            //ViewData["Count"] = UserCountHub._Users.Count.ToString();
            return View("SignalR/Item");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UITest(string name)
        {
            var _path = Server.MapPath($"~/Areas/Example/Views/Self/UITest/{name}.json");
            var code = System.IO.File.ReadAllText(_path);
            return Content(code);
        }
    }
    public partial class ActController : BaseController
    {


        public ActionResult Code(string name, bool SingleModel = false)
        {
            ViewData["SingleModel"] = SingleModel;
            return View($"Code/{name}");
        }
    }



    public class ChatConnection : PersistentConnection
    {
        private static int _connections = 0;

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            Interlocked.Increment(ref _connections);
            //廣播訊息
            Connection.Broadcast("新的連線加入，連線ID：" + connectionId + ",已有連線數：" + _connections);
            return Connection.Send(connectionId, "雙向連線成功，連線ID：" + connectionId);
        }

        /// <summary>
        /// 連線斷開 
        /// </summary>
        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            Interlocked.Decrement(ref _connections);
            return Connection.Broadcast(connectionId + "退出連線，已有連線數：" + _connections);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            var message = connectionId + "傳送內容>>" + data;
            return Connection.Broadcast(message);
        }
    }
}
namespace Genesis.Areas.DDD.Controllers
{
    public class DDDAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DDD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DDD_default",
                "DDD/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
    [RoutePrefix("DDD/Wafer")]
    public class WaferController : BaseController
    {
        [Route("LotInfo")]
        [Route("LotInfo/{LotSID}")]
        public ActionResult LotInfo(string LotSID = null, string Lot = null)
        => _Content((o) => Wafer_Services.QueryLotInfo(LotSID, Lot));

        ///*
        //因為無法處理 SN_ID 有帶 小數 - _ 等字符的問題,所以只採用這種方式 
        //*/
        public ActionResult ID(string SN_ID)
        => _Content(o => Wafer_Services.WaferInfo(SN_ID));
    }

    [RoutePrefix("DDD/ADM")]
    public class ADMController : BaseController
    {
        [Route("Test")]
        public ActionResult Test()
        {
            //var context = GlobalHost.ConnectionManager.GetHubContext<GTiHub>();
            //if (string.IsNullOrWhiteSpace(connectionIds))
            //{
            //    context.Clients
            //           .All
            //           .ShowMessage(name, country);
            //}
            //else
            //{
            //    //不支援多筆
            //    context.Clients
            //           .Clients(new List<string>
            //           {
            //       connectionIds
            //           })
            //           .ShowMessage(name, country);
            //}
            IResult result = new Result(true);
            return Content(result.ToJson());
        }

        /// <summary>
        /// Fix 程序,只要 帶入 AD_FUNCTION.FUN_SID 即可
        /// </summary>
        /// <param name="FUN_NAME"></param>
        /// <param name="FUN_SID"></param>
        /// <param name="FUN_URL"></param>
        /// <param name="FUN_FILE_NAME"></param>
        /// <returns></returns>
        [Route("Reason")]
        [Route("Reason/{FUN_NAME}")]
        [Route("Reason/{FUN_NAME}/{FUN_SID}")]
        public ActionResult Reason(string FUN_NAME, string FUN_SID, string FUN_URL = null, string FUN_FILE_NAME = null)
        => _Content((o) => DDLServices.Reason(FUN_NAME, FUN_SID, FUN_URL, FUN_FILE_NAME));

        [Route("Lot")]
        [Route("Lot/{LOT_SID}")]
        [Route("Lot/{LOT_SID}/info/{ActName}")]
        [Route("Lot/info/{ActName}")]
        public ActionResult LotInfo(string LOT_SID, string LOT = null, string ActName = null)
        => _Content((o) => LOT_Services.QueryLotInfo(LOT_SID, LOT, ActName));

        //TODO-tmp 用某個流程 ,直接 查出現下有那些站,站內有那些批號
    }
}

//bk