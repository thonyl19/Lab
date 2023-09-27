using BLL.Base;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Genesis.Common;
using Genesis.WebApi;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Resources;
using Genesis.Gtimes.Common;
using System.Configuration;
using System.Web.Http.Cors;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using vFile = System.IO.File;
using MDL.OracleMES.Tables;

namespace Genesis
{
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
                    isUItest(Req);
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
            if (string.IsNullOrWhiteSpace(UItest)==false)
            {
                string Baseurl = $"http://localhost:59394/GenesisNewMes/Example/Self/UITest?name={UItest}";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var r = client.GetAsync(Baseurl).Result;
                var res = r.Content.ReadAsStringAsync().Result;
                //var _o = JsonConvert.DeserializeObject(res);// res.Replace("\"", "");
                _code = $"{key} = {res};";//JSON.parse({_o.ToJson()});
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

                //Type _type = typeof(T);
                //if (_type == typeof(LotData)) {
                //    (_obj as LotData).LOT_Ext.Add("UItest_Src", res);
                //}
            }
            return _obj;
        }

        public IHtmlString mounted { get; set; }
        public IHtmlString methods { get; set; }


        public string Debug { get; set; } = "";
        public IHtmlString param_test { get { return Test("param.isTest='T'; console.log({ param });"); } }


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
        
        public struct GT_FormCol
        {
            /*
        	<template slot-scope="scope">
                <gt-no-name v-model="scope.row.Display" :no="scope.row.No"  :inline="false" ></gt-no-name>
			</template>
             */
            public int gt_no_name;

            /*
            搭配 i18n 處理, readonly 時的顯示模式
            <gt-form-col :label="i18n.校正結果" 校正結果 required col_sty="col-lg-12" v-model="i18n.CALIBRATION_RESULT[form.CALIBRATION_RESULT]" 
                :readonly="!isAddMode">
                <el-radio-group v-model="form.CALIBRATION_RESULT" required v-if="isAddMode">
                    <el-radio label="T">{{i18n.CALIBRATION_RESULT.T}}</el-radio>
                    <el-radio label="F">{{i18n.CALIBRATION_RESULT.F}}</el-radio>
                </el-radio-group>
            </gt-form-col> 
            */
            public int 搭配RadioGroup;
        }
        //public static string basePath(string key) { return $"~/Areas/Example/Views/Code/{key}.cshtml"; }

        public struct 進出站 {
            /*
            ##CheckPartials
            \Genesis_MVC\Areas\MES\Views\OperTask\StationCheckOut.cshtml 
            @foreach (var c in BLL.Base.ServicesBase.CheckPartials(Html, "GRF", "_LotFinlishTransfromMLot.cshtml"))
            {
                @Html.Raw(c)
            }
            */
            public static int CheckPartials;


            /*
            \Genesis_MVC\Areas\MES\Views\WIP\Partial2\_selectEqument.cshtml
            c_EqpMLOTList: {
                get() {
                    var r = [];
                    let { $store } = this;
                    if ($store) {
                        return  _.get($store,'state.globalvar.OperationPartUseList', []);
                    }
                    return r; 
                },
                set(val) {
                    this.EqpMLOTList = val;
                    let { $store } = this;
                    if ($store) this.$set($store.state.globalvar,'OperationPartUseList', val);
                }
            }
             
             */
            public static int _子物件往主物件做賦值;
        }

        public struct comm_Vue
        {
            /* mixins: [reason_obj],
            v_rootEl , v_model
            mixins: [Vue.prototype.$UT.Mixins.Info()],
            $nextTick 

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
                {{c_Src.Setting}}
                let { v_model, v_rootEl } = Vue.prototype.$UT.Mixins;
                mixins: [v_rootEl()],

            賦值程序)
            this.$set(this.c_Src, 'ErrGroup', {});
                   
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
                GTI_Test GTest = new GTI_Test(Html, HttpContext.Current.IsDebuggingEnabled, ViewContext.HttpContext.Request);
                
                ViewBag.Title = Model.ViewBag_Title(Face.CHECKOUT);
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

                var param = $.extend(submitData, VmSendData);
                @GTest.param_test
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
            url: "@Url.Action("DoFutureHold",{controll}, new { area = "WIP" })"
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

            /*
            QueryFunc(search) {
                var _self = this;
                let { query_mode } = this;
                var url = _self.$URL.chg_Path(_self.c_API, { search, query_mode });
                if (url == null) return;
                var _ajax = {
                    url,
                    type: 'get',
                    automessage: false,
                    success(res) {
                        let { Success, Message,Data} = res
                        if (Success) {
                            if (_self.clear_type == 1) _self.c_val = "";
                            if (_self.CheckRepeat(Data) == false) {
                                _self.callback(Data);
                            }
                        } else {
                            _self.$Alert.Err(Message).then(() => {
                                if (_self.clear_type == 2) _self.c_val = "";
                            });
                        }
                    }
                };
                $.submitForm(_ajax);
            } 
             */
            public static int API;

            /*
            M:\Prd_Dev\Genesis_MVC\Areas\ADM\Views\SequenceNum\SequenceNum_Item.cshtml
            this.$confirm(_self.i18n.ConfirmAdd, '提示', _self.confirm_arg).then(() => {
                _self.$URL.chgSearchParam({ SID: null }, true);
            }); 
             */
            public static int e_Add;

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


                public static int ViewRow;
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

                /*
                <el-table-column :label='i18n.ENABLE_FLAG' prop='ENABLE_FLAG' 啟用停用>
                    <template slot-scope="scope">
                        {{ENABLE_FLAG[scope.row.ENABLE_FLAG]}}
                    </template>
                </el-table-column>
                
                ENABLE_FLAG: {
					T:'@Face.rdoEnable',
					F:'@Face.rdoDisable',
				},
                */
                public static int ENABLE_FLAG;
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


            /// <summary>
            /// http://localhost:59394/GenesisNewMes/Example/Self/test?name=_el_DatePicker
            /// </summary>
            public struct DatePicker {
                /*
                <el-date-picker v-model="form.日期" style="width: 100% !important;"
                                type="daterange"
                                align="right"
                                unlink-panels
                                value-format="yyyy-MM-dd"
                                range-separator="@Face.DateQueryFlag"
                                start-placeholder="@Face.Start_Date"
                                end-placeholder="@Face.End_Date"
                                :picker-options="pickerOptions">
                </el-date-picker>
                data() {
                    form: {
                        日期:['2022-1-1','2022-1-31']
                        // 預填入當天日期
                        //日期:new Date()
                    },
                    pickerOptions: {
                        shortcuts: [
                            {
                                text: '最近一週',
                                onClick(picker) {
                                    const end = new Date();
                                    const start = new Date();
                                    start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
                                    picker.$emit('pick', [start, end]);
                                }
                            }, {
                                text: '最近一個月',
                                onClick(picker) {
                                    const end = new Date();
                                    const start = new Date();
                                    start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
                                    picker.$emit('pick', [start, end]);
                                }
                            }, {
                                text: '最近三個月',
                                onClick(picker) {
                                    const end = new Date();
                                    const start = new Date();
                                    start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
                                    picker.$emit('pick', [start, end]);
                                }
                            }
                        ]
                    },
                }
                computed: {
                    c_Conditions() {
                        var conditions = { condition: "AND", rules: [] };
                        _.each(this.form,
                            (value, field) => {
                                if (value != "" && value != null) {
                                    var _rule = { field, type: "string", operator: "contains", value };
                                    switch (field) {
                                        case "日期":
                                            //_rule.operator = 'greater_or_equal';
                                            //_rule.value = moment(this.form.日期).format('YYYY/MM/DD') + " 00:00:00";
                                            if (_.isString(value)) {
                                                _rule.operator = "Contains";
                                                _rule.value = [value];
                                            } else if (_.isArray(value)) {
                                                _rule.operator = "between";
                                                _rule.type = "date";
                                                var format = 'YYYY-MM-DD HH:mm:ss';
                                                var start = moment(value[0]).startOf('day').format(format);
                                                let end = moment(value[1]).endOf('day').format(format);
                                                _rule.value = [start, end];
                                            }
                                            conditions.rules.push(_rule);
                                            break;
                                        default:
                                            conditions.rules.push(_rule);
                                            break;
                                    }
                                }
                            });
                        return conditions;
                    },
                },
                click_Query() {
                    this.grid.Conditions = this.c_Conditions;
                    this.query(1);
                },
                // 查詢處理程序
                query(pageIdx) {
                    var _self = this;
                    _self.grid.Page.Index = pageIdx;
                    var param = _self.grid.query_rule;
                    console.log({ query: param });
                    var _ajax = {
                        url: '@Url.Action($"{action}_ListData")',
                        type: 'post',
                        param,
                        success: this.query_success
                    };
                    $.submitForm(_ajax);
                },
                // 查詢成功處理程序
                query_success(res) {
                    let { Data, Code = null, Message, Success = false } = res;
                    if (Success) {
                        let { PageInfo, gridData } = Data || {};
                        this.grid.data = gridData;
                        this.grid.row_count = PageInfo.RowCount;
                    } else {
                        var msg = Message != null
                            ? Message
                            : '@BLL.InterFace.ErrCode.GenericErrorMessage'
                            ;
                        this.$Alert.Err(msg);
                    }
                },

                 */
                public static int 日期起迄區間;

                /*
                L:\Prd_Dev\Genesis_MVC\Areas\PMS\Views\PMSJobExecute\PMSJobExecuteMaster.cshtml
                delay_query() {
                    var conditions = { condition: "AND", rules: [] };
                    var _rule_Data = { field: 'STATUS', operator: "Contains", type: "string", value: ["WAIT", "PROCESSING"] };
                    conditions.rules.push(_rule_Data);
                    var _rule_STATUS = { field: 'PLAN_DATE', operator: "less_or_equal", type: "date", value: moment().format("YYYY/MM/DD") };
                    conditions.rules.push(_rule_STATUS);
                    this.grid.Conditions = conditions;
                    this.query(1);
                },
                */
                public static int case_查詢今日以前的特定狀態;

                /*
                因為 MVC 接收時 , 無法直接接收 UTC 的時間格式 , 所以 需要用類似下面的程序 ,
                    先把日期資料轉成文字格式後 ,後端才能正確讀取並解析 
                param.START_DATE = moment(param.START_DATE).format('YYYY/MM/DD 00:00:00');
                param.END_DATE = moment(param.END_DATE).format('YYYY/MM/DD 23:59:59');
                 */
                public static int MVC_傳送資料的前置處理;

                /*
                    <el-date-picker v-model="form.CALIBRATION_DATE" style="width: 100% !important;"
                        type="date"
                        align="right"
                        unlink-panels
                        value-format="yyyy-MM-dd"
                        :picker-options="pickerOptions"
                        >
                    </el-date-picker>

                    pickerOptions: {
                        disabledDate(time) {
                            return time.getTime() > Date.now();
                        },
                    },
                */
                public static int 限定只能選取今天以前的日期;
            }
            /*
            更省事的用法 
            :e_clear="$Alert.Clear"

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
            col_sty="col-lg-12" -- 單邊一欄模式

            <div class="panel b">
                <div class="panel-body form-horizontal gt-form">
                    <div class="col-lg-6 col-md-12 adj">
                        <gt-form-col label="@Face.OPERATION" required readonly col_sty="col-lg-12">
                        </gt-form-col>
                    </div>
                </div>
            </div>
            */
            public static int 雙欄佈局;

            /*
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\WIP\Views\Lot\_Common.cshtml
            
            <div class="col-lg-6 col-md-6 form-group">
                <gt-form-col label="@Face.OPERATION" required >
                    <input class="form-control" v-if="c_SN_readonly"
                            v-model="c_SN"  
                            v-on:keyup.enter="act_query(c_val)"
                            />
                    <div class="form-control" readonly v-if="!c_SN_readonly">
                        <span class="label label-primary">{{Lot_0.ROUTE_VER_OPER_SID}}</span> {{Lot_0.OPERATION}}
                    </div>
                </gt-form-col>
            </div>
            
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

            /*
            <div class="col-lg-12 col-md-12 adj-center">
                <gt-query label="鋼版編號/名稱" 
                            v-model="query.Top"
                            placeholder="@RES.BLL.Message.PleaseScanTheBarcodeOrUseKeywords"
                            :callback="callback"
                            :query_mode=1
                            api="api/ddl/Tool"
                            >
                    <el-button type='success' icon="fa fa-save"   style="margin-left:3rem;" @@click="Save">
                        儲存
                    </el-button>
                </gt-query>
            </div> 
             
             */
            public static int _單欄置中配置;


            /*
            <gt-form-col :label='i18n.UPDATE_DATE' 資料更新日期 col_sty="col-lg-12"
                            v-trim>
                <div style="padding: 6px 0px">
                    <el-tag type="success" size="mini">{{form.UPDATE_USER}}</el-tag>
                    {{form.UPDATE_DATE}}
                </div>
            </gt-form-col>
            */
            public static int _人員日期配置;


            /*
             <gt-form-col :label="c_special_reason_label" col_sty="col-lg-12">
                <el-input type="textarea" v-model="qc_data.@nameof(IQC_QC_1916.SPECIAL_REASON)" :readonly="c_status_is_create">
            </gt-form-col> 

            <gt-form-col :label="i18n.DESCRIPTION" col_sty="col-lg-12">
                <el-input type="textarea" v-model="form.DESCRIPTION" :autosize="{ minRows: 3, maxRows: 4}" >
            </gt-form-col>
             */
            public static int _備註;

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
            public static int vue_selectize_班別;
            /*
            H:\GRF_Dev\Genesis_MVC\Areas\MES\Views\WIP\Partial2\_multiSelectUsers.cshtml
            <vue-selectize ref="selectShiftItem" placeholder="@RES.BLL.Message.PleaseScanTheBarcodeOrUseKeywords"
							   v-if="set_model.UserShiftList"
							   v-model="selected_Shiftitem"
							   :options="set_model.UserShiftList"
							   :selectize_ops="selectize_ops"
							   render_sty="GTIMES"
							   autocomplete="off"
                               required
							   >
				</vue-selectize>
             */


            public static int vue_selectize_Case;
            /*
            搭配 parsley 的應用案例
            H:\GTiMES5.1_Dev\Genesis_MVC\Areas\MES\Views\WIP\Partial2\_selectUsers.cshtml
            data-parsley-error-message
            */

            public static int vue_selectize_api;
            /*
            <gt-form-col :label="i18n.料號" col_sty="col-lg-12"
                                 v-model="form.PART_NO"
                                 :readonly="isViewMode">
                <vue-selectize-ddlapi v-model="form.PARTNO"  required
                            :options="ddl_PARTNO.src"
                            action="PartNo"
                            render_sty="GTIMES" 
						    :readonly="!isAddMode">
					    <gt-tagformat v-model="form.PART_NAME" :tag="form.PARTNO" readonly></gt-tagformat>
			    </vue-selectize-ddlapi>
            </gt-form-col>

            //使用 readonly_view
            <gt-form-col :label="i18n.料號" col_sty="col-lg-12"
                                 v-model="form.PART_NO"
                                 :readonly="isViewMode">
                <vue-selectize-ddlapi v-model="form.PARTNO"  required
                            :options="ddl_PARTNO.src"
                            action="PartNo"
                            render_sty="GTIMES" 
						    :readonly="!isAddMode"
                            :readonly_view="[form.INSTRUMENT_NAME,form.INSTRUMENT_NO]">
					    
			    </vue-selectize-ddlapi>
            </gt-form-col>

            */
        }

        public struct gt_toolbar
        {
            /*
			<gt-toolbar :e_exec="Exec" :e_clear="Clear_t"> </gt-toolbar>

            <gt-toolbar :e_save="c_save"
                    :e_del="c_delete"
                    :e_copy="c_copy"
                    :enable="c_enable"
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
            c_enable: {
                get() {
                    if (this.c_is_change)
                        this.form.@nameof(FC_EQUIPMENT_PORT.ENABLE_FLAG) = 'F';
                    return this.form.@nameof(FC_EQUIPMENT_PORT.ENABLE_FLAG) === 'T' ? true : false;
                },
                set(val) {
                    this.$set(this.form, '@nameof(FC_EQUIPMENT_PORT.ENABLE_FLAG)', val ? 'T' : 'F');

                    if (!this.fnCheckIsChange()) {
                        return false;
                    }

                    if (!this.c_isAddMode && !this.c_is_change)
                        this.fnUpdateEnable(val);
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

            /*
            e_enable(Enable) {
                debugger
                var _self = this;
                let param = {
                    SID: this.form.STATE_SID,
                    Enable
                }
                var url = '@Url.Action($"{action}_Enable")';
                var _ajax = {
                    url,
                    param,
                    type: 'post',
                    success(res) {
                        let { Success, Data, Message = ""} = res;
                        if (Success) {
                        } else {
                            _self.form.ENABLE_FLAG = !Enable ? 'T' : 'F';
                        }
                        _self.$UT.parent_reload();
                    }
                };
                return $.submitForm(_ajax);
            },
             
             */
            public static int Enable;

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


            public static int gt_query_lot;
            /*
                             <gt-query-lot label="@RES.BLL.Face.Lot" prompt_sty="has-req" col_sty="col-lg-12"
                              v-model="CurrentLot.LOT"
                              :query_func="Query"
                              :check_repeat="grid.data"
                              :readonly="c_LOT_input"
                              placeholder="@RES.BLL.Message.PleaseInputLot"
                              query_mode=1>
             
             */
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


            /*
            http://localhost:59394/GenesisNewMes/api/ddl/Parameter?ParameterNo=grade
            var _list = TableQueryService.GetRouteOperData(RouteVerSID);
			if (_list == null) return null;
			var _ddl = _list
				.Select(s => new SelectModel
				{
					SID = s.Field<string>("OPER_SID"),
					No = s.Field<string>("OPERATION_NO"),
					Display = s.Field<string>("OPERATION"),
					Value = s.Field<string>("OPER_SID"),
					Attr01 = s.Field<string>("ROUTE_VER_OPER_SID")
				}).ToList();
			return _ddl;
            */
            public static int SelectModel;

            /*
            http://localhost:59394/GenesisNewMes/Example/Self/test?name=_Upload

            [Route("api/Import/Upload")]
            [HttpPost]
            public IResult Upload()
            {
                var httpRequest = HttpContext.Current.Request;
                // 檢查是否有上傳的檔案
                if (httpRequest.Files.Count == 0)
                {
                    return new Result("沒有上傳的檔案。");
                }
                var postedFile = httpRequest.Files[0];
                string fileName = httpRequest["fileName"];
                if (fileName == null) { 
                    fileName = Path.GetFileName(postedFile.FileName);
                }
                var uploadPath = HttpContext.Current.Server.MapPath("~/App_Data/" + fileName);
                if (File.Exists(uploadPath)) File.Delete(uploadPath);

                postedFile.SaveAs(uploadPath);
                return new Result(true, "圖片上傳成功。") { Data = new { fileName } };
            } 
            
             */
            public static int upload;

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
            =>_Content(o => new Result("測試資料送出") { Code = "991", Data = data });
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
                [Ref] H:\GRF_Dev\Genesis_MVC\Areas\ZZ_LIO\Views\OperTask\LaserCuttingCheckOut.cshtml

                @foreach (var c in BLL.Base.ServicesBase.CheckPartials(Html, nameof(mes.LIO), Model.Setting.CheckOutSet?.SplitCfg))
                {
                    @Html.Raw(c)
                }
                    
                @BLL.Base.ServicesBase.CheckPartials(Server,Html, nameof(mes.GRF), "_woInfo_ext.cshtml")
        
                /// <summary>
		        /// 
		        /// </summary>
		        /// <param name="server"></param>
		        /// <param name="htm"></param>
		        /// <param name="Key"></param>
		        /// <param name="list"></param>
		        /// <returns></returns>
		        /// Anthony-20230719)更懶的作法,以及加上找不到檔案的防呆機制
		        public static IHtmlString CheckPartials(HttpServerUtilityBase server, HtmlHelper htm, string Key, params string[] list)
		        {
			        var r = new List<MvcHtmlString>();
			        bool isMatch = System.Configuration.ConfigurationManager.AppSettings["ProjectCustomer"] == Key;
			        if (!isMatch) return htm.Raw("");
			        var area_name = _Rule(Key);
			        foreach (string item in list)
			        {
				        var viewPath = $"~/Areas/{area_name}/Views/{item}";
				        if (System.IO.File.Exists(server.MapPath(viewPath)))
				        {
					        r.Add(htm.Partial(viewPath));
				        }
			        }
			        return htm.Raw(string.Join("", r));
		        }
                 */
                /// <summary>
                /// 處理多檔載入需求 
                /// </summary>
                public static int CheckPartials;

                /*
                H:\HM_Dev\Library\BLL\MES\WIPInfoServices.cs
                GetEdcHist
                var dataRange = new Base.DataRange(edcHistDate);
                 */
                /// <summary>
                /// 取得日期區間
                /// </summary>
                public static int DataRange;


                /*
                M:\Prd_zz\Genesis_MVC\Common\PageBaseInfo.cs

                public class PF_PARTNO : iCommPage
                { 
				    public string path { get; } = "~/Views/Maintain/_PF_PARTNO.cshtml"; 
				    public string Query { get; set; } 

                    /// <summary>
                    /// 子視窗程序
                    /// </summary>
                    public string SubForm = "";
                } 
                */
                public static int CommPage;
            }


            /*
            public Dictionary<string,object> ExtenParam { get; set; } = new Dictionary<string, object>();
             {
                "ExtenParam": {
                    "NextStationEQP": "[{\"RefKey\":null,\"SID\":\"GTI23032016184294144\",\"No\":\"DBM-001\",\"Value\":\"DBM-001\",\"Display\":\"DBM-001\",\"StatusSid\":null,\"Status\":null,\"FromNum\":0,\"INum\":0,\"Attr01\":null,\"Attr02\":null,\"Attr03\":null,\"$order\":1}]",
			        "X":"ABC"
                },
        
            }
             */
            public static int _可接收的Dictionary架構;
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

            /*
             
             
             */
            public struct PagerQuery {
                /*
                L:\Prd_Dev\Library\BLL\MES\WRP\ReportFormServices.cs
                
                var PageInfo = merge.PageResult<dynamic>(pagerQuery.Page.Index, pagerQuery.Page.Size);
                var gridData = PageInfo.Queryable.ToList();
                foreach (var d in gridData)
                {
                    for (int i = 0; i < param.Columns.Count; i++)
                    {
                        var col = param.Columns[i];
                        if (string.IsNullOrEmpty(col.DATA_FORMAT_STRING))
                            continue;

                        IDictionary<string,object> rawData = (IDictionary<string, object>)d;
                        rawData[col.COLUMN_NAME] = string.Format(col.DATA_FORMAT_STRING, rawData[col.COLUMN_NAME]);
                    }
                }
                data.gridData = gridData;
                data.PageInfo = pagerQuery.parsePagedResult(PageInfo);
                 */

                public static int 在查詢結果之後補上必要欄位_case1;

                /*
                L:\Prd_Dev\Library\BLL\MES\ADMServices.cs
                L:\Prd_Dev\Library\BLL\MES\WOServices.cs
                var PageInfo = _query.PageResult(PQuery.Page.Index, PQuery.Page.Size);
							var Queryable = (
									from wo in PageInfo.Queryable
									join part in dbContext.PF_PARTNO_VER
										on wo.PARTNO_VER_SID equals part.PARTNO_VER_SID
									select new
									{
										#region [ wo.* ] 
										wo.WO_SID,
										wo.WO,
										wo.SO,
										wo.SO_TYPE,
										wo.SO_SEQ,
										wo.PO,
										wo.PO_TYPE,
										wo.PO_SEQ,
										wo.WO_TYPE,
										wo.WO_TYPE2,
										wo.WO_TYPE3,
										wo.STATUS,
										wo.ERP_STATUS,
										wo.QUANTITY,
										wo.ERP_QUANTITY,
										wo.UNIT,
										wo.QTY_1,
										wo.UNIT_1,
										wo.QTY_2,
										wo.UNIT_2,
										wo.UNRELEASE_QUANTITY,
										wo.UNRELEASE_QTY_1,
										wo.UNRELEASE_QTY_2,
										wo.LOT_SIZE,
										wo.ROUTE_VER_SID,
										wo.ROUTE,
										wo.ROUTE_VERSION,
										wo.PRODUCT_SID,
										wo.PRODUCT,
										wo.PARTNO_VER_SID,
										wo.PARTNO,
										wo.PARTNO_VERSION,
										wo.PRIORITY,
										wo.OWNER,
										wo.CUSTOMER,
										wo.BONDED_FLAG,
										wo.BONDED_NO,
										wo.SCHEDULEDATE,
										wo.DUEDATE,
										wo.ERP_CREATE_DATE,
										wo.ERP_FINISH_DATE,
										wo.FINISH_DATE,
										wo.CREATEDATE,
										wo.MIN_START_DATE,
										wo.PRE_START_DATE,
										wo.ROOT_WO_SID,
										wo.PARENT_WO_SID,
										wo.PARENT_WO_TYPE,
										wo.FACTORY,
										wo.MODIFY_FLAG,
										wo.BOM_SID,
										wo.BOM_VERSION,
										wo.BOM_CREATE_DATE,
										wo.CONFIRM_FLAG,
										wo.SCHEDULE_QTY,
										wo.ERP_RECEIVE_QTY,
										wo.ERP_WIP_QTY,
										wo.AREA_SID,
										wo.AREA_NO,
										wo.INVENTORY_TYPE,
										wo.LINE_NAME,
										wo.CONFIRM_DATE,
										wo.CONFIRM_USER,
										wo.PACKAGE_UNIT_QTY,
										wo.PACKAGE_UNIT,
										wo.SIGN_STATUS,
										wo.CUST_PART_NO,
										wo.CUST_PART_NO_SID,
										wo.CUST_PART_NO_VER_SID,
										wo.GROSS_DIE_QTY,
										wo.PM_USER,
										wo.SHIPPING_ADDRESS,
										wo.SHIPPING_ADDRESS1,
										wo.URGENT_FLAG,
										wo.WAFER_THICKNESS,
										wo.WAFER_SIZE,
										wo.RECEIVE_NO,
										wo.RECEIVE_NO_TYPE,
										wo.ERP_COMMENT,
										wo.MES_COMMENT,
										wo.ERP_COMMENT1,
										wo.ERP_COMMENT2,
										wo.ERP_COMMENT3,
										wo.ERP_COMMENT4,
										wo.ATTRIBUTE_01,
										wo.ATTRIBUTE_02,
										wo.ATTRIBUTE_03,
										wo.ATTRIBUTE_04,
										wo.ATTRIBUTE_05,
										wo.ATTRIBUTE_06,
										wo.ATTRIBUTE_07,
										wo.ATTRIBUTE_08,
										wo.ATTRIBUTE_09,
										wo.ATTRIBUTE_10,
										wo.ATTRIBUTE_11,
										wo.ATTRIBUTE_12,
										wo.ATTRIBUTE_13,
										wo.ATTRIBUTE_14,
										wo.ATTRIBUTE_15,
										wo.ATTRIBUTE_16,
										wo.CREATE_USER,
										wo.CREATE_DATE,
										wo.UPDATE_USER,
										wo.UPDATE_DATE,
										wo.RELEASE_QUANTITY,
										wo.RELEASE_QTY_1,
										wo.RELEASE_QTY_2,
										wo.YIELD,
										wo.SCARP_QUANTITY,
										wo.SCARP_QTY1,
										wo.SCARP_QTY2,
										wo.TERMINATED_QUANTITY,
										wo.TERMINATED_QTY1,
										wo.TERMINATED_QTY2,
										wo.ECN,
										wo.STOCK_QTY,
										wo.RELEASE_DATE,
										wo.ECN_SID,
										wo.ECN_NO,
										wo.ECN_NAME,
										wo.WO_LINE_SID,
										wo.WO_LINE_NO,
										wo.WO_LINE,
										#endregion
										part.PART_NAME
									}
								)
								.ToList();
                */
                public static int 在查詢結果之後補上必要欄位_case2;

                /*
                L:\Prd_Dev\Library\BLL\MES\QMSService.cs
                這一版作法的概念是
                1.先把 排序,分頁的結果 取出為 tmp_list
                2.從  tmp_list 取得 list_key
                3.用 list_key 再去 db 取得需要 子項欄位
                4.使用 linq join 的方式 , 把欄位填回到  tmp_list

                var PageInfo = _queryBase.PageResult(PQuery.Page.Index, PQuery.Page.Size);
				var tmp_list = PageInfo.Queryable.ToList();
				var list_LotSid = tmp_list.Select(c => c.LOT_SID).ToList();
				var list_part = (from a in dbContext.WP_LOT
							where list_LotSid.Contains(a.LOT_SID)
							join b in dbContext.PF_PARTNO
								on a.PARTNO equals b.PARTNO
							select new {
								a.LOT_SID,
								a.PARTNO,
								a.WO_LINE,
								a.WO_LINE_NO,
								b.PART_NAME
							}).ToList();
				var Queryable = tmp_list.Join(
								list_part,
								a=>a.LOT_SID,
								b=>b.LOT_SID,
								(a,b)=> {
									a.PART_NAME = b.PART_NAME;
									a.PARTNO = b.PARTNO;
									a.WO_LINE = b.WO_LINE;
									a.WO_LINE_NO = b.WO_LINE_NO;
									return a;
								}).ToList();
                 */
                public static int 在查詢結果之後補上必要欄位_case3;

            }

        }

        public struct MES
        {
            public struct EDC { 
                /*
                GetLotInfoCheck
                WIPOperConfigServices.GetOperEdc(dbc, _lotInfo, RouteVerOperInfo);
                var RouteVerOperInfo = new RouteUtility.RouteVerOperationInfo(dbc, _lotInfo.ROUTE_VER_OPER_SID, RouteUtility.IndexType.SID);
                */
                public static int by工站取得EDC;

                /*
                M:\Prd_zz\Library\BLL\ICM\Maintain.QC_INSTRUMENTS_CALIBRATION_RECORDS.cs

                 */
                public static int 讀取EDC記錄_儀校;
            }

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
                 var listA = (from a in svcAD_RESOURCE.GetAllListIQueryable()
						 where a.RESOURCE_TYPE == 9
						 select a)
						 .ToDictionary(p => p.RESOURCE_NO, p => p);
                */
                public static int basic;

                /*
                var query = from a in context.A join b in context.B on a.Id equals b.AId 
                into bGroup from b in bGroup.DefaultIfEmpty() 
                where b != null 
                select new { a, b };

                */
                public static int left_join;

                /*
                Txn.result.Data = Txn.EFQuery<PF_OPERATION_API>()
					.Reads(c=>c.FUN_TYPE == FUN_TYPE && c.OPER_SID == OPER_SID)
                    .Select(c => new
					{
						c.MOUDLE,
						c.RESOURCE_URL,
						c.CHECK_MODE
					})
					.GroupBy(c=>c.MOUDLE)
					.ToDictionary(c=>c.Key,c => (dynamic)c.ToList());

                
                var _repo = new
			    {
				    QC_INSTRUMENTS = Txn.EFQuery<QC_INSTRUMENTS>(),
				    QC_INSTRUMENTS_EDC = Txn.EFQuery<QC_INSTRUMENTS_EDC>(),
				    QC_INSTRUMENTS_STATE = Txn.EFQuery<QC_INSTRUMENTS_STATE>(),
			    };
			    var form =  (from a in _repo.QC_INSTRUMENTS.Reads()
				     join b in _repo.QC_INSTRUMENTS_STATE.Reads() 
					    on a.STATE_SID equals b.STATE_SID into bGroup
				     where a.INSTRUMENT_SID == SID
			
				     from b in bGroup.DefaultIfEmpty()
				     select new
				     {
					     A = a,
					     STATE_NAME = b != null ? b.STATE_NAME : null
				     }).FirstOrDefault();

                */
                public static int case1;

                /*
                var data_input = _repo.QC_INSP_EDC.Reads(c => c.INSP_NO == form.IPQC_NO)
                        .Select(q1 => new BLL.DataViews.Edc.EdcModel
						{
							QCItemSID = q1.INSP_EDC_SID,
							DispayPointName = q1.DISPLAY_POINT_NAME,
							ItemName = q1.PARAMETER,
							ItemNo = q1.PARA_NO,
							EdcVerSid = q1.EDC_VER_SID,
							DataType = q1.DATATYPE,
							TL = q1.TL,
							UCL = q1.UCL,
							LCL = q1.LCL,
							USL = q1.USL,
							LSL = q1.LSL,
							mustInput = q1.MUST_INPUT,
						}).ToList();
                */
                public static int EDC;


                /*
                context.YourEntities.AsNoTracking().ToList();
                 */
                public static int i排解相同類型的實體已經有相同的主索引鍵值;
            }

            /*
             NotMapped
             */
            public static string 擴充欄位 = new MDL.MES.QC_AQL_DATA().AQL_NO;
        }

        public struct commm {
            /*
            string ProjectCustomer = System.Configuration.ConfigurationManager.AppSettings["ProjectCustomer"] != null ?
				System.Configuration.ConfigurationManager.AppSettings["ProjectCustomer"] : "";
             */
            public static int ConfigurationManager;


            /*
            var svcAD_RESOURCE = new BaseAdResourceServices() { DbContext = new MVCContext() }; 
             */
            public static int MVCContext;


            /*
            D:\GRF_Dev\Genesis_MVC\Common\BaseController.cs
            _OperTaskView()

            TempData["Err"] = err;
            return Redirect($"~/Pages/Error?Message=執行程式定義錯誤，請聯絡管理人員");
             
             */
            public static int ErrorMessage;

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

            /*
            測試案例中, 其資料中有多個集合 , 如果沒有既有物件可以承接讀取 ,
                就可利用下例中的方式 ,簡單的建一個資料集合來承接 
            
            struct d_QC_INSTRUMENTS_CALIBRATION_RECORDS_Save {
			    public QC_INSTRUMENTS_CALIBRATION_RECORDS form ;
			    public List<EdcModel> edc_list;
		    } 
             */
            public static int 讀取集合性質的Log;
        }

        /*
        檢核相關參考 
        */
        public struct Check {
            /*
            H:\GRF_Dev\Library\BLL\ZZ\GRF\AHTask.WoMtrConsumption.cs 
            struct d_WoMtrConsumption {
			    public WOInfo WO ;
			    public MtrLotUtility.MtrLotInfo MTR_LOT;
		    }
		    static IResult Check_WoMtrConsumption(ITxnBase Txn, ZZ_WO_MLOT_CONSUME data) {
			    if (string.IsNullOrEmpty(data.WO)) {
				    Result.FieldRequired(RES.BLL.Face.WO).ThrowException();
			    }
			    if (string.IsNullOrEmpty(data.MTR_LOT)) {
				    Result.FieldRequired(RES.BLL.Face.MLOT).ThrowException();
			    }
                if (data.MTR_QTY == null)
                {
                    Result.FieldRequired("物料耗用數量").ThrowException();
                }

			    var _d = new d_WoMtrConsumption();
			    _d.WO = Txn.GetWOInfo(data.WO);
			    if (_d.WO.IsExist == false) Result.NotExist(RES.BLL.Face.WO);

			    _d.MTR_LOT = Txn.GetMLotInfo(data.MTR_LOT);
			    if (_d.MTR_LOT.IsExist == false) Result.NotExist(RES.BLL.Face.MLOT);

			    Txn.result.Data = _d;
			    return Txn.result;
		    }
            */
            public static int Case1;

            /*
             
            BLL.InterFace.Check
            */
            public static int Case_InterFace;
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult TestEDC(string file = "EDC_API")
        {
            var _data = ControllerContext.HttpContext.Server.MapPath($"../../Areas/example/Views/Self/UITest/{file}.json");
            string text = System.IO.File.ReadAllText(_data);

            var EdcLog = JsonConvert.DeserializeObject<object>(text);
            var _r = new Result(true)
            {
                Data = EdcLog
            };
            return Content(_r.ToJson(true));
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

    [RoutePrefix("DDD/DBA")]
    public class DBAController : BaseController {
        DBController _dbc;
        internal DBController DBC
        {
            get
            {
                if (_dbc == null)
                {

                    var Conn = ConfigurationManager.ConnectionStrings["sql.mes"];
                    _dbc = new DBController(Conn);
                }
                return this._dbc;
            }
        }

        [Route("IP/{IP}")]
        public ActionResult LotInfo(string IP = "226")
        => _Content((o) =>{
            using (_dbc ?? DBC){ 

            }
            return null;
        });

        //[EnableCors(origins: "http://allowed-origin.com", headers: "*", methods: "GET")]
        [AllowAnonymous]
        [Route("Table")]
        [Route("Table/{Table}")]
        public ActionResult TableInfo(string Table = null) {
            using (_dbc ?? DBC)
            {
                var sql_table_list = @"
                SELECT name
                FROM sys.tables;
                ";

                var sql_table_schema = $@"
                    SELECT 
		                    c.name AS Filed,
		                    ISNULL(p.value, '') AS [Desc],
		                    t.Name AS Type,
		                    c.max_length/2 AS Length,
		                    IIF(c.is_nullable=0,'N','Y') AS abeNull
                    FROM  sys.columns c
			                    INNER JOIN  sys.types t 
				                    ON c.user_type_id = t.user_type_id
			                    LEFT OUTER JOIN sys.extended_properties p 
				                    ON p.major_id = c.object_id AND p.minor_id = c.column_id
                    WHERE 
		                    OBJECT_NAME(c.object_id) = '{Table}'
                ";
                var _sql = Table == null ? sql_table_list : sql_table_schema;
                return Content(_dbc.Select(_sql).ToJson(true));

            }
            return Content("");
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

    [RoutePrefix("DDD/APP")]
    public class APPController : BaseController
    {
        private readonly string[] _localizationFiles = new[]
        {
            "Face.zh-TW.resx",
            "Message.zh-TW.resx"
        };

        [Route("i18n/{keyword}")]
        public ActionResult i18nSearch(string keyword)
        => _Content(o =>
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            foreach (var file in _localizationFiles)
            {
                //var results = new Dictionary<string, string>();
                var _path = System.Web.HttpContext.Current.Server.MapPath("~/");
                var fileName = $"{_path}/../../Library/RES/BLL/{file}";
                XDocument doc = XDocument.Load(fileName);
                var query = from elem in doc.Descendants("data")
                            where elem.Value.Contains(keyword) 
                                || elem.Attribute("name").Value.Contains(keyword)
                            select new
                            {
                                Key = elem.Attribute("name").Value,
                                Value = elem.Element("value").Value
                            };
                var results = new Dictionary<string, string>();
                foreach (var item in query)
                {
                    if (results.ContainsKey(item.Key))
                    {
                        results.Add($"{item.Key}~${item.Value}", item.Value);
                    }
                    else { 
                        results.Add(item.Key, item.Value);
                    }
                }
                var mainkey = file.Replace(".zh-TW.resx", "");
                result.Add(mainkey, results);
            }
            return new Result(true) { Data = result };
        });


        [HttpPost]
        [Route("i18n/Add/{res}/{key}/")]
        public ActionResult i18nAdd(string res, string key, string en, string tw, string cn)
        => _Content(o =>
        {
            Type _t = null;
            switch (res.ToUpper()) {
                case "FACE":
                    _t = typeof(RES.BLL.Face);
                    break;
                case "MESSAGE":
                    _t = typeof(RES.BLL.Message);
                    break;
            }
            if (_t == null) return new Result("查無符合的 BLL.res");
            var rm = new ResourceManager(_t);
            var MatchItem = rm.GetObject(key);
            if (MatchItem != null) {
                var r = new Result("Key值己存在");
                r.Data = new { key , MatchItem };
                return r;
            }

            var root = Server.MapPath("~/");
            var tarFile = $@"{root}..\Library\RES\BLL\{res}.resx";
            if (FileHelper.IsExistFile(tarFile) == false) {
                return Result.NotExist("語系檔").Data = new { tarFile };
            }

            ResXResourceSet resxSet = new ResXResourceSet(tarFile);
            using (ResXResourceWriter resxWriter = new ResXResourceWriter(tarFile)){
                foreach (DictionaryEntry entry in resxSet){
                    var _key = entry.Key.ToString();
                    resxWriter.AddResource(_key, entry.Value);
                }

                resxWriter.AddResource(key, en);
                resxWriter.Generate();
            }

            for (var i = 0; i < 2; i++) {
                string val = tw, res_tp = ".zh-TW";
                if (i == 1) { 
                    val = cn;
                    res_tp = ".zh-CN";
                }
                var tarFile1 = $@"{root}..\Library\RES\BLL\{res}{res_tp}.resx";
                string fileContent = vFile.ReadAllText(tarFile1);
                // 使用正則表達式進行置換
                string pattern = $@"</data>\s*</root>";
                string replacement = $@"</data>
	<data name=""{key}"" xml:space=""preserve"">
		<value>{val}</value>
	</data>
</root>";
                string newContent = Regex.Replace(fileContent, pattern, replacement);
                vFile.WriteAllText(tarFile1, newContent);
            }
            return o;
        });


        [Route("i18n/parseTxt")]
        public ActionResult parseTxt()
        => _Content(o =>
        {
            Type _t = null;
            var list = new string[] { "Face", "Message" };
            var root = Server.MapPath("~/");
            List<string> result;
            foreach (var res in list) { 
                var tarFile = $@"{root}..\Library\RES\BLL\{res}.zh-TW.resx";
                if (FileHelper.IsExistFile(tarFile) == false){
                    return Result.NotExist("語系檔").Data = new { res };
                }
                var resxSet = new ResXResourceReader(tarFile);
                
                result = new List<string>();
                foreach (DictionaryEntry entry in resxSet)
                {
                    result.Add(entry.Value.ToString());
                }
                
                var data = string.Join("\n", result);
                var tarFile1 = $@"{root}..\Library\RES\BLL\{res}.txt";
                vFile.WriteAllText(tarFile1, data);
            }
            return o;
        });
        [Route("i18n/parseTxt/callback")]
        public ActionResult callback()
        => _Content(o =>
        {
            Type _t = null;
            var list = new string[] { "Face", "Message" };
            var root = Server.MapPath("~/");
            List<string> result;
            foreach (var res in list)
            {
                var tarFile = $@"{root}..\Library\RES\BLL\{res}-zh-TW.resx";
                var src = $@"{root}..\Library\RES\BLL\{res}.txt";
                if (FileHelper.IsExistFile(tarFile) == false)
                {
                    return Result.NotExist("語系檔").Data = new { res };
                }
                var resxSet = new ResXResourceReader(tarFile);

                result = new List<string>();
                foreach (DictionaryEntry entry in resxSet)
                {
                    result.Add(entry.Value.ToString());
                }

                var data = string.Join("\n", result);
                var tarFile1 = $@"{root}..\Library\RES\BLL\{res}.txt";
                vFile.WriteAllText(tarFile1, data);
            }
            return o;
        });
    }
}
//bk