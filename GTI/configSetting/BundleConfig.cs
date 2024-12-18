using BLL.Base;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using BLL.MVC;
using BundleTransformer.Core.Transformers;
using Frame.Code;
using Frame.Code.Web.Select;
using Genesis.Common;
using Genesis.Gtimes.Common;
using Genesis.Library.BLL;
using Genesis.Library.BLL.MES.DataViews;
using Genesis.Web.SwaggeRegister.Common;
using MDL.MES;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml.Linq;
using static BLL.MVC.ResourceServices;
using vFile = System.IO.File;
using NetHttp = System.Net.Http;
using static BLL.MES.WIPInjectServices;
using Newtonsoft.Json.Linq;
using System.Web.Http.Description;
using MDL.GenesisMVC.Tables;
using Genesis.Gtimes.WIP;

namespace Genesis
{
    public static class eBundle
	{
		public static string bootstrap_Css = "~/Content/bootstrapCss";
		public static string jqGrid_CSS = "~/bundles/jqGridCss";
		public static string jqGrid_JS = "~/bundles/jqGrid";
		public static string Vue = "~/bundles/VueJS";
		public static string JQuery = "~/bundles/jquery";
		public static string JQueryDrag_JS = "~/Vendor/JQueryDragJS";
		public static string JQueryDrag_CSS = "~/Vendor/JQueryDragCSS";
		public static string Storage = "~/bundles/storage";
		
		public static string Genesis = "~/bundles/Genesis";
		public static string elUI_JS = "~/Vendor/elUI_JS";
		public static string elUI_CSS = "~/Vendor/elUI_CSS";
		public static string app_el_CSS = "~/Content/app/css/app_el.css";
        public static string app_component = "~/Content/app/css/app_component.css";
		public static string jqDataTables_JS = "~/Vendor/jqDataTablesJs";
		public static string jqDataTables_CSS = "~/Vendor/jqDataTablesCss";

		public static string artTemplate = "~/bundles/artTemplate";
		public static string parsley = "~/bundles/parsley";
		public static string localize = "~/bundles/localize";
		/// <summary>
		/// 備註：這裡不包括載入 elUI 相關套件
		/// </summary>
		public static string Vue_MES = "~/bundles/vue_mes";
		public static string selectize_JS = "~/bundles/selectizeJS";
		public static string selectize_CSS = "~/Vendor/selectizeCSS";
		public static string barcode = "~/Vendor/VueBarcode";
		public static string c3_CSS = "~/Vendor/c3Css";
		public static string c3_JS = "~/Vendor/c3Js";
		public static string char_JS = "~/Vendor/charJs";
		public static string ECharts_JS = "~/Vendor/EChartsJs";


		//public static IHtmlString QRender_JS()
		//{
		//	return Scripts.Render
		//		(eBundle.selectize_JS
		//		, eBundle.Vue_MES
		//		, eBundle.elUI_JS);

		//}
		public static IHtmlString QRender_JS(params string[] list)
		{
			var _list = new string[] { 
				eBundle.selectize_JS
				, eBundle.Vue_MES
				, eBundle.elUI_JS
			}.Concat(list).ToArray();
			return Scripts.Render(_list);
		}

		public static IHtmlString QRender_CSS(params string[] list)
		{
            var _list = new string[] { eBundle.selectize_CSS
                , eBundle.elUI_CSS 
                , eBundle.app_el_CSS
				, eBundle.app_component
			}.Concat(list).ToArray();
            return Styles.Render (_list);
		}

		
	}

	public class BundleConfig
	{

		// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			dynamic styleBundle;

			// App Styles
			//bundles.Add(new StyleBundle("~/Content/appCss").Include(
			//	"~/Content/app/css/app.css",
			//	"~/Content/app/css/print.css",
			//	"~/Content/mvc-override.css"
			//));


			styleBundle = new StyleBundle("~/bundle/appCss")
						 .Include("~/Content/app/css/app.css")
						 .Include("~/Content/app/css/print.css")
						 .Include("~/Content/mvc-override.css");

			styleBundle.Transforms.Add(new StyleTransformer());//修正CSS檔裡的URL錯誤問題
			/***再呼叫這行執行檔案最小化***/
			styleBundle.Transforms.Add(new CssMinify());
			bundles.Add(styleBundle);


			// Bootstrap Styles
			bundles.Add(new StyleBundle(eBundle.bootstrap_Css).Include(
				"~/Content/app/css/bootstrap.css", new CssRewriteUrlTransform()
			));

			bundles.Add(new ScriptBundle(eBundle.Genesis).Include(
				// App init
				"~/Scripts/app/app.init.js",
				// GTI
				"~/Scripts/GtiFramework.js",
				//fastLoadCSS
				"~/Scripts/app/modules/load-css.js",
				// Modules
				"~/Scripts/app/modules/bootstrap-start.js",
				"~/Scripts/app/modules/calendar.js",
				"~/Scripts/app/modules/easypiechart.js",
				"~/Scripts/app/modules/clear-storage.js",
				"~/Scripts/app/modules/constants.js",
				"~/Scripts/app/modules/flatdoc.js",
				"~/Scripts/app/modules/trigger-resize.js",
				"~/Scripts/app/modules/fullscreen.js",
				"~/Scripts/app/modules/gmap.js",
				
				"~/Scripts/app/modules/i18next.js",
				"~/Scripts/app/modules/maps-vector.js",
				"~/Scripts/app/modules/navbar-search.js",
				"~/Scripts/app/modules/notify.js",
				"~/Scripts/app/modules/now.js",
				"~/Scripts/app/modules/panel-tools.js",
				"~/Scripts/app/modules/play-animation.js",
				"~/Scripts/app/modules/porlets.js",
				"~/Scripts/app/modules/sidebar.js",
				"~/Scripts/app/modules/skycons.js",
				"~/Scripts/app/modules/slimscroll.js",
				"~/Scripts/app/modules/sparkline.js",
				"~/Scripts/app/modules/table-checkall.js",
				"~/Scripts/app/modules/toggle-state.js",
				"~/Scripts/app/modules/utils.js",
				"~/Scripts/app/modules/morris.js",
				"~/Scripts/app/modules/rickshaw.js",
				"~/Scripts/app/modules/chartist.js",
				"~/Scripts/app/modules/tour.js",
				"~/Scripts/app/modules/sweetalert.js",
				"~/Scripts/app/modules/color-picker.js",
				"~/Scripts/app/modules/imagecrop.js",
				"~/Scripts/app/modules/chart-knob.js",
				"~/Scripts/app/modules/chart-easypie.js",
				"~/Scripts/app/modules/select2.js",
				"~/Vendor/jquery.blockUI/jquery.blockUI.js",
				"~/Vendor/cookie/jquery.cookie.js"
			));

			//"~/Scripts/app/modules/chart.js",


			// Demos
			bundles.Add(new ScriptBundle("~/bundles/demoDashboard").Include(
			   "~/Scripts/demo/demo-dashboard.js"
		   ));

			bundles.Add(new ScriptBundle("~/bundles/demoDatatable").Include(
				"~/Scripts/demo/demo-datatable.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoFlot").Include(
				"~/Scripts/demo/demo-flot.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoForms").Include(
				"~/Scripts/demo/demo-forms.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoPanels").Include(
				"~/Scripts/demo/demo-panels.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoRTL").Include(
				"~/Scripts/demo/demo-rtl.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoSearch").Include(
				"~/Scripts/demo/demo-search.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoVectormap").Include(
				"~/Scripts/demo/demo-vector-map.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoJQGrid").Include(
				"~/Scripts/demo/demo-jqgrid.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoSortable").Include(
				"~/Scripts/demo/demo-sortable.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoNestable").Include(
				"~/Scripts/demo/demo-nestable.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoUpload").Include(
				"~/Scripts/demo/demo-upload.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoWizard").Include(
				"~/Scripts/demo/demo-wizard.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoXEditable").Include(
				"~/Scripts/demo/demo-xeditable.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/demoJQCloud").Include(
				"~/Scripts/demo/demo-jqcloud.js"
			));

			// Main Vendor

			bundles.Add(new ScriptBundle(eBundle.JQuery).Include(
				"~/Vendor/jquery/dist/jquery.js"
			));

			bundles.Add(new ScriptBundle(eBundle.c3_JS).Include(
				"~/Vendor/d3/d3.min.js",
				"~/Vendor/d3/d3-time-format.js",
				"~/Vendor/c3/c3.js"
			));

			bundles.Add(new ScriptBundle(eBundle.char_JS).Include(
				"~/Vendor/Chart.js/2.7.0/Chart.js",
				"~/Vendor/Chart.js/2.7.0/vue-chartjs.3.5.min.js"
			));

			bundles.Add(new ScriptBundle(eBundle.ECharts_JS).Include(
				"~/Vendor/echarts/5.3.0/dist/echarts.js"
				//"~/Vendor/echarts/4.1.0/vue-echarts.min.js"
			));
			

			bundles.Add(new StyleBundle(eBundle.c3_CSS)
			  .Include("~/c3/c3.css", new CssRewriteUrlTransform())
			);


			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Vendor/modernizr/modernizr.custom.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Vendor/bootstrap/dist/js/bootstrap.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jsbarcode").Include(
					  "~/Vendor/jsbarcode/JsBarcode.all.min.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/pagedjs").Include(
					  "~/Vendor/pagedjs-0.3.5/paged.polyfill-0.3.5.min.js"
			));

			// Vendor Plugins

			bundles.Add(new ScriptBundle("~/bundles/matchMedia").Include(
					"~/Vendor/matchMedia/matchMedia.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/sparklines").Include(
				"~/Vendor/sparkline/index.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/ChartJS").Include(
				 "~/Vendor/Chart.js/dist/Chart.js"
			));

			#region simpleLineIcons css 路徑問題進行處理

			//bundles.Add(new StyleBundle("~/bundles/simpleLineIcons").Include(
			//  "~/Vendor/simple-line-icons/css/simple-line-icons.css", new CssRewriteUrlTransform()
			//));
			styleBundle = new StyleBundle("~/bundles/simpleLineIcons")
			.Include("~/Vendor/simple-line-icons/css/simple-line-icons.css");
			styleBundle.Transforms.Add(new StyleTransformer());
			bundles.Add(styleBundle);

			#endregion


			bundles.Add(new ScriptBundle(eBundle.Storage).Include(
			  "~/Vendor/jQuery-Storage-API/jquery.storageapi.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jqueryEasing").Include(
			  "~/Vendor/jquery.easing/js/jquery.easing.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
			  "~/Vendor/datatables/media/js/jquery.dataTables.min.js",
			  "~/Vendor/datatables-colvis/js/dataTables.colVis.js",
			  "~/Vendor/datatables/media/js/dataTables.bootstrap.js",
			  // Buttons
			  "~/Vendor/datatables-buttons/js/dataTables.buttons.js",
			  //"~/Vendor/datatables-buttons/css/buttons.bootstrap.css",
			  "~/Vendor/datatables-buttons/js/buttons.bootstrap.js",
			  "~/Vendor/datatables-buttons/js/buttons.colVis.js",
			  "~/Vendor/datatables-buttons/js/buttons.flash.js",
			  "~/Vendor/datatables-buttons/js/buttons.html5.js",
			  "~/Vendor/datatables-buttons/js/buttons.print.js",
			  "~/Vendor/datatables-responsive/js/dataTables.responsive.js",
			  "~/Vendor/datatables-responsive/js/responsive.bootstrap.js"
			));

			bundles.Add(new StyleBundle("~/bundles/datatablesCss")
			  .Include("~/Vendor/datatables-colvis/css/dataTables.colVis.css", new CssRewriteUrlTransform())
			  .Include("~/Vendor/datatables/media/css/dataTables.bootstrap.css", new CssRewriteUrlTransform())
			  .Include("~/Vendor/dataTables.fontAwesome/index.css", new CssRewriteUrlTransform())
			);

			bundles.Add(new ScriptBundle("~/bundles/parsley").Include(
			  "~/Vendor/parsleyjs/2.9.2/dist/parsley.min.js",
			  "~/Vendor/parsleyjs/2.9.2/dist/i18n/zh_tw.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/filestyle").Include(
			  "~/Vendor/bootstrap-filestyle/src/bootstrap-filestyle.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/tagsinput").Include(
			  "~/Vendor/bootstrap-tagsinput/dist/bootstrap-tagsinput.min.js"
			));

			bundles.Add(new StyleBundle("~/bundles/tagsinputCss").Include(
			  "~/Vendor/bootstrap-tagsinput/dist/bootstrap-tagsinput.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/gmap").Include(
			  "~/Vendor/jQuery-gMap/jquery.gmap.min.js"
			));

			bundles.Add(new StyleBundle("~/bundles/weatherIcons").Include(
			  "~/Vendor/weather-icons/css/weather-icons.min.css", new CssRewriteUrlTransform()
			));

			bundles.Add(new StyleBundle("~/bundles/weatherIconsWind").Include(
			  "~/Vendor/weather-icons/css/weather-icons-wind.min.css", new CssRewriteUrlTransform()
			));

			bundles.Add(new ScriptBundle("~/bundles/skycons").Include(
			  "~/Vendor/skycons/skycons.js"
			));

			bundles.Add(new StyleBundle("~/bundles/whirl").Include(
			  "~/Vendor/whirl/dist/whirl.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/animo").Include(
			  "~/Vendor/animo.js/animo.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/fastclick").Include(
			  "~/Vendor/fastclick/lib/fastclick.js"
			));

			#region fontawesome css 路徑問題進行處理

			styleBundle = new StyleBundle("~/bundles/fontawesome")
			.Include("~/Vendor/fontawesome/css/font-awesome.min.css");
			styleBundle.Transforms.Add(new StyleTransformer());

			bundles.Add(styleBundle);

			//bundles.Add(new StyleBundle("~/bundles/fontawesome").Include(
			//  "~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform()
			//));


			#endregion

			bundles.Add(new ScriptBundle("~/bundles/sliderCtrl").Include(
			  "~/Vendor/seiyria-bootstrap-slider/dist/bootstrap-slider.min.js"
			));

			bundles.Add(new StyleBundle("~/bundles/sliderCtrlCss").Include(
			  "~/Vendor/seiyria-bootstrap-slider/dist/css/bootstrap-slider.min.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/wysiwyg").Include(
			  "~/Vendor/bootstrap-wysiwyg/bootstrap-wysiwyg.js",
			  "~/Vendor/bootstrap-wysiwyg/external/jquery.hotkeys.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/slimscroll").Include(
			  "~/Vendor/slimscroll/jquery.slimscroll.min.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/screenfull").Include(
			  "~/Vendor/screenfull/dist/screenfull.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jvectormap").Include(
			  "~/Vendor/ika.jvectormap/jquery-jvectormap-1.2.2.min.js",
			  "~/Vendor/ika.jvectormap/jquery-jvectormap-world-mill-en.js",
			  "~/Vendor/ika.jvectormap/jquery-jvectormap-us-mill-en.js"
			));

			bundles.Add(new StyleBundle("~/bundles/jvectormapCss").Include(
			  "~/Vendor/ika.jvectormap/jquery-jvectormap-1.2.2.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/flot").Include(
			  "~/Vendor/flot/jquery.flot.js",
			  "~/Vendor/flot.tooltip/js/jquery.flot.tooltip.min.js",
			  "~/Vendor/flot/jquery.flot.resize.js",
			  "~/Vendor/flot/jquery.flot.pie.js",
			  "~/Vendor/flot/jquery.flot.time.js",
			  "~/Vendor/flot/jquery.flot.categories.js",
			  "~/Vendor/flot-spline/js/jquery.flot.spline.min.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jqueryUi").Include(
			  "~/Vendor/jquery-ui/jquery-ui.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jqueryUiTouchPunch").Include(
			  "~/Vendor/jqueryui-touch-punch/jquery.ui.touch-punch.min.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/moment").Include(
			  "~/Vendor/moment/min/moment-with-locales.min.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
			  "~/Vendor/jquery.inputmask/dist/jquery.inputmask.bundle.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/flatdoc").Include(
			  "~/Vendor/flatdoc/flatdoc.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
			  "~/Vendor/chosen_v1.2.0/chosen.jquery.min.js"
			));

			bundles.Add(new StyleBundle("~/bundles/chosenCss").Include(
			  "~/Vendor/chosen_v1.2.0/chosen.min.css", new CssRewriteUrlTransform()
			));

			bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
			  "~/Vendor/fullcalendar/dist/fullcalendar.min.js",
			  "~/Vendor/fullcalendar/dist/gcal.js"
			));

			bundles.Add(new StyleBundle("~/bundles/fullcalendarCss").Include(
			  "~/Vendor/fullcalendar/dist/fullcalendar.css"
			));

			bundles.Add(new StyleBundle("~/bundles/animatecss").Include(
			  "~/Vendor/animate.css/animate.min.css"
			));

			bundles.Add(new ScriptBundle(eBundle.barcode).Include(
				"~/Scripts/vue-barcode.min.js"
			));

			//改成i18next
			//bundles.Add(new ScriptBundle("~/bundles/localize").Include(
			//  "~/Vendor/jquery-localize-i18n/dist/jquery.localize.js"
			//));

			//i18next
			bundles.Add(new ScriptBundle("~/bundles/localize").Include(
			  "~/Vendor/i18next/i18next.js",
			  "~/Vendor/i18next/i18nextBrowserLanguageDetector.js",
			  "~/Vendor/i18next/i18nextHttpBackend.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/nestable").Include(
			  "~/Vendor/nestable/jquery.nestable.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/sortable").Include(
			  "~/Vendor/html.sortable/dist/html.sortable.js"
			));


			bundles.Add(new ScriptBundle(eBundle.jqGrid_JS).Include(
			  "~/Vendor/jqgrid/5.4.0/js/jquery.jqGrid.min.js",
			  "~/Vendor/jqgrid/5.4.0/js/i18n/grid.locale-tw.js",
			  "~/Vendor/jqTtableExport/tableExport.min.js"

			));


			//"~/Vendor/jqTtableExport/libs/es6-promise/es6-promise.auto.min.js", //png
			//  "~/Vendor/jqTtableExport/libs/html2canvas/html2canvas.min.js", //png
			//  "~/Vendor/jqTtableExport/libs/jsPDF/jspdf.min.js", //PDF
			//  "~/Vendor/jqTtableExport/libs/jsPDF/jspdf.plugin.autotable.js", //PDF
			//  "~/Vendor/jqTtableExport/libs/js-xlsx/xlsx.core.min.js", //Excel2007
			//  "~/Vendor/jqTtableExport/libs/FileSaver/FileSaver.min.js", //Files
			//  "~/Vendor/jqTtableExport/libs/bootstrap-table/bootstrap-table.min.js", //bootstrap
			//  "~/Vendor/jqTtableExport/libs/bootstrap-table/bootstrap-table-export.js", //bootstrap



			bundles.Add(new StyleBundle(eBundle.jqGrid_CSS).Include(
		  "~/Vendor/jqgrid/css/ui.jqgrid.css",
		  "~/Vendor/jquery-ui/themes/smoothness/jquery-ui.css"
			));

			bundles.Add(new StyleBundle("~/bundles/fileUploadCss").Include(
				"~/Vendor/blueimp-file-upload9.31.0/css/jquery.fileupload.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/fileUpload").Include(
				"~/Vendor/jquery-ui/ui/widget.js",
				"~/Vendor/blueimp-tmpl/js/tmpl.js",
				"~/Vendor/blueimp-load-image/js/load-image.all.min.js",
				"~/Vendor/blueimp-canvas-to-blob/js/canvas-to-blob.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.iframe-transport.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.fileupload.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.fileupload-process.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.fileupload-image.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.fileupload-audio.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.fileupload-video.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.fileupload-validate.js",
				"~/Vendor/blueimp-file-upload9.31.0/js/jquery.fileupload-ui.js"
			));

			bundles.Add(new StyleBundle("~/bundles/xEditableCss").Include(
				"~/Vendor/x-editable/dist/bootstrap3-editable/css/bootstrap-editable.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/xEditable").Include(
			  "~/Vendor/x-editable/dist/bootstrap3-editable/js/bootstrap-editable.min.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jqueryValidate").Include(
			  "~/Vendor/jquery-validation/dist/jquery.validate.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jquerySteps").Include(
			  "~/Vendor/jquery.steps/build/jquery.steps.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/datetimePicker").Include(
			  "~/Vendor/eonasdan-bootstrap-datetimepicker/build/js/bootstrap-datetimepicker.min.js"
			));

			bundles.Add(new StyleBundle("~/bundles/datetimePickerCss").Include(
				"~/Vendor/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css"
			));

			bundles.Add(new StyleBundle("~/bundles/RickshawCss").Include(
				"~/Vendor/rickshaw/rickshaw.min.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/Rickshaw").Include(
			  "~/Vendor/d3/d3.min.js",
			  "~/Vendor/rickshaw/rickshaw.js"
			));

			bundles.Add(new StyleBundle("~/bundles/ChartistCss").Include(
				"~/Vendor/chartist/dist/chartist.min.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/Chartist").Include(
			  "~/Vendor/chartist/dist/chartist.js"
			));

			bundles.Add(new StyleBundle("~/bundles/MorrisCss").Include(
				"~/Vendor/morris.js/morris.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/Morris").Include(
			  "~/Vendor/raphael/raphael.js",
			  "~/Vendor/morris.js/morris.js"
			));

			bundles.Add(new StyleBundle("~/bundles/Spinkit").Include(
				"~/Vendor/spinkit/css/spinkit.css"
			));

			bundles.Add(new StyleBundle("~/bundles/LoadersCss").Include(
				"~/Vendor/loaders.css/loaders.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/JQCloud").Include(
			  "~/Vendor/jqcloud2/dist/jqcloud.js"
			));

			bundles.Add(new StyleBundle("~/bundles/JQCloudCss").Include(
				"~/Vendor/jqcloud2/dist/jqcloud.css"
			));

			bundles.Add(new StyleBundle("~/bundles/SweetAlertCss").Include(
				"~/Vendor/sweetalert/dist/sweetalert.css"
			));

			bundles.Add(new StyleBundle("~/bundles/SweetAlert").Include(
				"~/Vendor/sweetalert/dist/sweetalert.min.js"
			));



			bundles.Add(new StyleBundle("~/bundles/BootstrapTourCss").Include(
				"~/Vendor/bootstrap-tour/build/css/bootstrap-tour-standalone.css"
			));
			bundles.Add(new StyleBundle("~/bundles/BootstrapTour").Include(
				"~/Vendor/bootstrap-tour/build/js/bootstrap-tour-standalone.js"
			));

			bundles.Add(new StyleBundle("~/bundles/ColorPickerCss").Include(
				"~/Vendor/mjolnic-bootstrap-colorpicker/dist/css/bootstrap-colorpicker.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/ColorPicker").Include(
			  "~/Vendor/mjolnic-bootstrap-colorpicker/dist/js/bootstrap-colorpicker.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Knob").Include(
			  "~/Vendor/jquery-knob/js/jquery.knob.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/EasyPie").Include(
			  "~/Vendor/jquery.easy-pie-chart/dist/jquery.easypiechart.js"
			));

			bundles.Add(new StyleBundle("~/bundles/CropperCss").Include(
				"~/Vendor/cropper/dist/cropper.css"
			));

			bundles.Add(new ScriptBundle("~/bundles/Cropper").Include(
			  "~/Vendor/cropper/dist/cropper.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/select2").Include(
			  "~/Vendor/select2/dist/js/select2.js"
			));

			bundles.Add(new StyleBundle("~/bundles/select2Css").Include(
			  "~/Vendor/select2/dist/css/select2.css",
			  "~/Vendor/select2-bootstrap-theme/dist/select2-bootstrap.css"
			));


			//knockout
			bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
			 "~/Scripts/knockout-3.1.0.js",
			  "~/Scripts/knockout.mapping-latest.js",
			  "~/Scripts/perpetuum.knockout.js"
				));

			//"~/Scripts/moment.js"
			bundles.Add(new ScriptBundle("~/bundles/SweetAlert_2").Include(
			   "~/Vendor/sweetalert2/sweetalert2.all.min.js",
			   "~/Vendor/sweetalert2/IeFix/es6-promise.min.js",
			   "~/Vendor/sweetalert2/IeFix/es6-promise.auto.min.js",
			   "~/Vendor/sweetalert2/IeFix/polyfill.min.js"
		   ));

			//bootstrap-treeview.min
			bundles.Add(new StyleBundle("~/bundles/bootstrapTreeviewCss").Include(
			   "~/Vendor/bootstrap-treeview/bootstrap-treeview.min.css"
		   ));
			bundles.Add(new ScriptBundle("~/bundles/bootstrapTreeview").Include(
			"~/Vendor/bootstrap-treeview/bootstrap-treeview.min.js"
			));

			//bootstrap-treeview.min
			bundles.Add(new StyleBundle("~/bundles/doubleboxBootstrapCss").Include(
			   "~/Vendor/doublebox-bootstrap/doublebox-bootstrap.css"
		   ));

			bundles.Add(new ScriptBundle("~/bundles/doubleboxBootstrap").Include(
			"~/Vendor/doublebox-bootstrap/doublebox-bootstrap.js"
			));

			//selectize
			bundles.Add(new ScriptBundle(eBundle.selectize_JS).Include(
				"~/Vendor/selectize.js-0.12.4/js/microplugin.js",
				"~/Vendor/selectize.js-0.12.4/js/sifter.js",
				"~/Vendor/selectize.js-0.12.4/js/selectize.js"
				));

			bundles.Add(new StyleBundle(eBundle.selectize_CSS).Include(
				"~/Vendor/selectize.js-0.12.4/css/selectize.bootstrap3.css"));

			bundles.Add(new StyleBundle(eBundle.elUI_CSS).Include(
				"~/Vendor/element-ui/lib/theme-chalk/index.css"));

			bundles.Add(new ScriptBundle(eBundle.elUI_JS).Include(
				"~/Vendor/element-ui/lib/index.js"
				));

			//GoJS-2.0.6
			bundles.Add(new ScriptBundle("~/bundles/GoJS").Include(
			"~/Vendor/GoJS-2.0.6/go.min.js"
			));


			//Vis
			bundles.Add(new ScriptBundle("~/bundles/visNetworkJS").Include(
			"~/Vendor/vis/dist/vis.min.js"
			));

			bundles.Add(new StyleBundle("~/bundles/visNetworkCss").Include(
			"~/Vendor/vis/dist/vis-network.min.css"));


			//signalR
			bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
				"~/Scripts/json2.js",
				//"~/signalr/hubs",
				"~/Scripts/jquery-3.5.1.min.js",
				"~/Scripts/jquery.signalR-2.4.1.min.js"
			));

			//echarts
			bundles.Add(new ScriptBundle("~/bundles/echartsJS").Include(
			"~/Vendor/echarts/dist/echarts.min.js",
			"~/Vendor/echarts/dist/infographic.js"

			));

			//jqueryLayoutJS
			bundles.Add(new ScriptBundle("~/bundles/jqueryLayoutJS").Include(
			"~/Vendor/jquery.layout/jquery.layout-latest.js"

			));
			//jqueryLayoutCSS
			bundles.Add(new StyleBundle("~/bundles/jqueryLayoutCss").Include(
				"~/Vendor/jquery.layout/layout-default-latest.css"));


			//flowUI
			bundles.Add(new ScriptBundle("~/bundles/flowUIJS").Include(
			"~/Vendor/flow-ui/flow.js"
			));

			bundles.Add(new StyleBundle("~/bundles/flowUICss").Include(
				"~/Vendor/flow-ui/flow.css"));

			//gooFlow
			bundles.Add(new ScriptBundle("~/bundles/gooFlowJS").Include(
			"~/Vendor/gooflow/codebase/GooFlow.js",
			"~/Vendor/gooflow/codebase/GooFunc.js"

			));


			styleBundle = new StyleBundle("~/bundles/gooFlowCss")
			.Include("~/Vendor/gooflow/codebase/GooFlow.css");
			styleBundle.Transforms.Add(new StyleTransformer());
			bundles.Add(styleBundle);

			//        bundles.Add(new StyleBundle("~/bundles/gooFlowCss").Include(
			//"~/Vendor/gooflow/codebase/GooFlow.css", new CssRewriteUrlTransform()));

			//Vue
			bundles.Add(new ScriptBundle(eBundle.Vue).Include(
			  "~/Scripts/vue.js",
			  "~/Scripts/lodash.min.js"
			));
			bundles.Add(new ScriptBundle(eBundle.Vue_MES).Include(
				"~/Scripts/vue.js",
				"~/Scripts/vuex.js",
				"~/Scripts/numeral.min.js",
				"~/Scripts/vue_Utility.js",
				"~/Scripts/lodash.min.js",
				"~/Scripts/vue_mes/*.js"
			));


			styleBundle = new StyleBundle(eBundle.jqDataTables_CSS)
			.Include(
				//"~/Vendor/DataTablesPackages/DataTables-1.10.20/css/dataTables.bootstrap.min.css",
				"~/Vendor/DataTablesPackages/DataTables-1.10.20/css/jquery.dataTables.min.css",
				"~/Vendor/DataTablesPackages/Extensions/RowReorder-1.2.6/css/rowReorder.dataTables.min.css",
				"~/Vendor/DataTablesPackages/Extensions/Responsive-2.2.3/css/responsive.dataTables.min.css"
			);
			styleBundle.Transforms.Add(new StyleTransformer());
			bundles.Add(styleBundle);

			bundles.Add(new ScriptBundle(eBundle.jqDataTables_JS).Include(
			  //"~/Vendor/DataTablesPackages/DataTables-1.10.20/js/dataTables.bootstrap.min.js",
			  "~/Vendor/DataTablesPackages/DataTables-1.10.20/js/jquery.dataTables.min.js",
			  "~/Vendor/DataTablesPackages/Extensions/RowReorder-1.2.6/js/dataTables.rowReorder.js",
			  "~/Vendor/DataTablesPackages/Extensions/Responsive-2.2.3/js/dataTables.responsive.js"

			));

			bundles.Add(new ScriptBundle(eBundle.JQueryDrag_JS).Include(
			  "~/Vendor/jquery-drag/context/context.js",
			  "~/Vendor/jquery-drag/drag.js",
			  "~/Vendor/jquery-drag/jquery.grumble/js/jquery.grumble.min.js"
			));

			styleBundle = new StyleBundle(eBundle.JQueryDrag_CSS)
			.Include(
				"~/Vendor/jquery-drag/context/context.standalone.css",
				"~/Vendor/jquery-drag/jquery.grumble/css/grumble.min.css"
			);
			styleBundle.Transforms.Add(new StyleTransformer());
			bundles.Add(styleBundle);

			//bootstrap-duallistbox
			bundles.Add(new StyleBundle("~/bundles/bootstrapDuallistboxCss").Include(
			   "~/Vendor/bootstrap-duallistbox/3.0.6/dist/bootstrap-duallistbox.css"
		   ));
			bundles.Add(new ScriptBundle("~/bundles/bootstrapDuallistboxJs").Include(
			"~/Vendor/bootstrap-duallistbox/3.0.6/dist/jquery.bootstrap-duallistbox.js"
			));

			//artTemplate
			bundles.Add(new ScriptBundle(eBundle.artTemplate).Include(
			"~/Scripts/art-template-web.js"
			));

			//當設定為true時,輸出則會顯示壓縮後的檔案
			//當為false,輸出則顯示個別檔案
			BundleTable.EnableOptimizations = false;


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

		//public ActionResult QC_INSTRUMENTS_CALIBRATION_RECORDS_PMS_Save(QC_INSTRUMENTS_CALIBRATION_RECORDS form, List<EdcModel> edcData, string isTest = null)
		//=> _Content1(o => Maintain.QC_INSTRUMENTS_CALIBRATION_RECORDS_Save(form, edcData, isTest == "T"));

		public static ExpandoObject GetCurrentMethodParameters(int index = 1)
		{
			// 取得當前執行緒的堆疊框架
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(index); // 1 表示取得呼叫者的堆疊框架

			// 取得呼叫方法的方法資訊
			MethodBase method = stackFrame.GetMethod();

			// 取得傳入參數集合
			ParameterInfo[] parameters = method.GetParameters();

			dynamic parameterValues = new ExpandoObject();

			// 將參數值加入動態物件
			foreach (var parameter in parameters)
			{
				((IDictionary<string, object>)parameterValues)[parameter.Name] = parameter.DefaultValue;
			}

			return parameterValues;
		}

		public static ExpandoObject GetCurrentMethodParameters(string ActionName)
		{
			StackTrace stackTrace = new StackTrace();
			dynamic parameterValues = new ExpandoObject();
			// 從堆疊中尋找目標呼叫者
			for (int i = 1; i < stackTrace.FrameCount; i++)
			{
				StackFrame stackFrame = stackTrace.GetFrame(i);
				MethodBase method = stackFrame.GetMethod();

				// 檢查呼叫者的類型和方法名稱
				if (method.ReflectedType != null &&
					method.Name == ActionName)
				{
					ParameterInfo[] parameters = method.GetParameters();


					// 將參數值加入動態物件
					foreach (var parameter in parameters)
					{
						((IDictionary<string, object>)parameterValues)[parameter.Name] = parameter.DefaultValue;
					}

					break;
				}
			}
			// 取得傳入參數集合
			return parameterValues;
		}

		public dynamic Check_RedirectToCustomAction(bool isNeedExec)
		{
			if (!isNeedExec) return null;

			var controllerContext = ControllerContext;
			string ProjectCustomer = "DAE";// ServicesBase.ProjectCustomer ?? "";
			if (ProjectCustomer == "") return null;

			dynamic Arg = new ExpandoObject();
			string ActionName = controllerContext.RouteData.Values["action"].ToString();
			Arg.CusActionName = $"{ActionName}_{ProjectCustomer}";
			var actionMethod = controllerContext.Controller
				.GetType()
				.GetMethod(Arg.CusActionName);
			if (actionMethod == null) return null;

			var queryParameter = HttpContext.Request.QueryString;
			var routeValues = new RouteValueDictionary();
			foreach (string key in queryParameter)
			{
				routeValues.Add(key, queryParameter[key]);
			}
			Arg.RouteParam = routeValues;
			return Arg;
		}


		public ActionResult _Content1(Func<IResult, IResult> func, bool isCus = false)
		{
			IResult result = new Result(true);
			try
			{
				var Arg = Check_RedirectToCustomAction(isCus);
				if (Arg != null)
				{
					//return Content(((object) Arg).ToJson(true));
					return RedirectToAction(Arg.CusActionName, Arg.RouteParam);
				}
				result = func(result);
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
				result.Data = ex.Data;
				Logger.Error(ex.Message, ex);
			}

			return Content(result.ToJson(true));
		}

		[AllowAnonymous]
		public ActionResult test1(string name, bool SingleModel = true)
		=> _Content1(o => new Result(true) { Data = "Test1" }, true);

		[AllowAnonymous]
		public ActionResult test1_DAE(string name, bool SingleModel = true)
		=> _Content(o => new Result(true) { Data = new { name, SingleModel } });

		[AllowAnonymous]
		/// <summary>
		/// 針對 進出站的控件做 測試
		/// </summary>
		/// <param name="name"></param>
		/// <param name="SingleModel"></param>
		/// <returns></returns>
		public ActionResult InOut(string name, string Case = "0", bool SingleModel = true)
		{
			ViewData["SingleModel"] = SingleModel;
			//var _data = ControllerContext.HttpContext.Server.MapPath($"../../Areas/example/Views/Act/InOut/~Case{Case}.json");
			//string text = System.IO.File.ReadAllText(_data);
			//ViewData["result"] = JsonConvert.DeserializeObject(text);

			return View($"InOut/{name}", new LotData());
		}

		public ActionResult SignalR_Item(string id = null)
		{
			//ViewData["Count"] = Hubs.UserCountHub._Users.Count.ToString();
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

	//*
	// RoutePrefix 在 DDDAreaRegistration 下,沒有作用
	//[RoutePrefix("DDD/ApiTest")]
	//[Route("DDD/ApiTest")]
	[SwaggerAuthorizationFilterAttribute]
	public class CaseController : System.Web.Http.ApiController
	{


        [Route("DDD/ApiTest/T001")]
        [System.Web.Http.HttpGet]
        public List<string> T001()
        => new List<string>() { "T01" };

        //      [Route("DDD/ApiTest/T003/{T003}/Test")]
        //      [System.Web.Http.HttpGet]
        //      public List<string> T003(string T003)
        //      => new List<string>() { T003 };


        //[HttpGet]
        //public List<SelectModel> T002()
        //{
        //    throw new Exception();
        //    return new List<SelectModel>();
        //}

    }

	public class CaseAPIController : System.Web.Http.ApiController
	{
		[System.Web.Http.HttpGet]
		[System.Web.Http.Route("T002")]
		public dynamic T002()
		{ 
			throw new Exception("test");
			return "";
		}

		/// <summary>
		/// 測試自定義的 Route
		/// </summary>
		/// <returns></returns>
		/// 一但設定 Route ,程序就會改套用 Route 做為 Url
		[System.Web.Http.Route("T001")]
		public List<string> T001()
		=> new List<string>() { "T01" };

		public List<string> Get()
		=> new List<string>() { "T01" };

		public string Get(string id)
		=> id;

		// POST: api/Products
		public NetHttp.HttpResponseMessage Post(string id)
		{
			var obj = new { id = id };
			var response = new NetHttp.HttpResponseMessage(System.Net.HttpStatusCode.Created);
			/*
			在 RESTful API 中，資源的狀態是由 URI 定義的。當成功創建一個資源後，
			伺服器應該返回一個 201 Created 的狀態碼，以及一個 Location 標頭，
			指向新創建的資源的 URI ,
			如果要完成前述的需求 ,可搭配如下程序
			 */
			//response.Headers.Location = new Uri(Url.Link("DefaultApi"
			//	, obj
			//	));
			response.Content = new NetHttp.StringContent(obj.ToJson(true), Encoding.UTF8, "application/json");
			return response;
		}
	}
	
	
	[SwaggerAuthorizationFilterAttribute]
	[RoutePrefix("DDD/DBA")]
	public class DBAController : BaseController
	{
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
		=> _Content((o) => {
			using (_dbc ?? DBC)
			{

			}
			return null;
		});

		/*
		~\Genesis_MVC\Common\LogActionFilterAttribute.cs 
			skipAction.Add("NeedUpdateAuthMenus");
			skipAction.Add("GetResource");
			skipAction.Add("Dashboard_vue");
		 */
		//[EnableCors(origins: "http://allowed-origin.com", headers: "*", methods: "GET")]
		[AllowAnonymous]
		[Route("Table")]
		[Route("Table/{Table}")]
		public ActionResult GetResource(string Table = null)
		{
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
	}

	[RoutePrefix("DDD/APP")]
	public class APPController : BaseController
	{
		private readonly string[] _localizationFiles = new[]
		{
			"Face.zh-TW.resx",
			"Message.zh-TW.resx"
		};

		[Route("i18n/Search/{keyword}")]
		[Route("i18n/Search/{keyword}/{project}")]
		public ActionResult i18nSearch(string keyword,string project = null)
		=> _Content(o =>
		{
			var result = new Dictionary<string, Dictionary<string, string>>();
			foreach (var file in _localizationFiles)
			{
					//var results = new Dictionary<string, string>();
					var _path = System.Web.HttpContext.Current.Server.MapPath("~/");
				var fileName = $"{_path}/../../Library/RES/BLL/{file}";

				if (project != null) {
					var new_dir = project.Replace("_", @":\");
					var pattern = @"^(.*?):\\(\w*)_";
					fileName = Regex.Replace(fileName, pattern, $"{new_dir}_");
				}
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
					else
					{
						results.Add(item.Key, item.Value);
					}
				}
				var mainkey = file.Replace(".zh-TW.resx", "");
				result.Add(mainkey, results);
			}
			return new Result(true) { Data = new { result, project } };
		});

		[AllowAnonymous]
		[HttpPost]
		[Route("i18n/Add/{res}/{key}/")]
		public ActionResult i18nAdd(string res, string key, string en, string tw, string cn)
		=> _Content(o => I18nAdd(res, key, en, tw, cn));

		IResult I18nAdd(string res, string key, string en, string tw, string cn)
		{
			Type _t = null;
			switch (res.ToUpper())
			{
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
			if (MatchItem != null)
			{
				var r = new Result("Key值己存在");
				r.Data = new { key, MatchItem };
				return r;
			}

			var root = Server.MapPath("~/");
			var tarFile = $@"{root}..\Library\RES\BLL\{res}.resx";
			if (FileHelper.IsExistFile(tarFile) == false)
			{
				return Result.NotExist("語系檔").Data = new { tarFile };
			}

			var is產品語系檔 = root.Substring(0, 2) == "M:";
			if (is產品語系檔 == false)
			{
				var tmp = new { en, tw, cn };
				vFile.WriteAllText($@"P:\MyLab\GTI_Sample\~i18n\{res}_{key}.json", tmp.ToJson(true));
			}

			ResXResourceSet resxSet = new ResXResourceSet(tarFile);
			using (ResXResourceWriter resxWriter = new ResXResourceWriter(tarFile))
			{
				foreach (DictionaryEntry entry in resxSet)
				{
					var _key = entry.Key.ToString();
					resxWriter.AddResource(_key, entry.Value);
				}

				resxWriter.AddResource(key, en);
				resxWriter.Generate();
			}




			for (var i = 0; i < 2; i++)
			{
				string val = tw, res_tp = ".zh-TW";
				if (i == 1)
				{
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
			return new Result(true);
		}

		public struct d_i18n
		{
			public string en;
			public string tw;
			public string cn;
		}

		[Route("GTI_Test/t_Process")]
		public ActionResult t_Process()
		=> Content(vFile.ReadAllText(GTI_Test.g_path.t_Process));


		[Route("i18n/AutoAdd")]
		public ActionResult AutoAdd()
		=> _Content(o =>
		{
			var root = Server.MapPath("~/");
			var is產品語系檔 = root.Substring(0, 2) == "M:";
			if (is產品語系檔 == false) return new Result("目前不是在產品環境");
			string directoryPath = $@"{root}Areas\Example\Views\Self\~i18n\";

			if (Directory.Exists(directoryPath))
			{
					// 取得目錄中的 JSON 檔案清單
					string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

				foreach (string jsonFile in jsonFiles)
				{
					try
					{
						var _fileName = Path.GetFileNameWithoutExtension(jsonFile);
						var arr = _fileName.Split('_');
							//var res = _fileName[0];
							//var key = _fileName[1];

							//$@"{directoryPath}{jsonzFile}"
							string jsonContent = vFile.ReadAllText(jsonFile);
							// 解析 JSON 內容到物件
							var data = JsonConvert.DeserializeObject<d_i18n>(jsonContent);
						var r = I18nAdd(arr[0], arr[1], data.en, data.tw, data.cn);
						if (r.Success)
						{
							vFile.Move(jsonFile, $@"{directoryPath}{_fileName}.---");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Error processing {jsonFile}: {ex.Message}");
					}
				}
			}
			else
			{
				Console.WriteLine("Directory does not exist.");
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
			foreach (var res in list)
			{
				var tarFile = $@"{root}..\Library\RES\BLL\{res}.zh-TW.resx";
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

namespace Genesis.Areas.SYSAdmin.Controllers
{
	public partial class ResourceController : BaseController
	{
		public ActionResult ResourceData_t(string keyVal)
		{
			ViewData["result"] = ResourceServices.Query(keyVal).ToJson(true);
			ViewData["SingleModel"] = true;
			ViewData["mode"] = string.IsNullOrEmpty(keyVal) ? "Add" : "Edit";
			var _view = "~/Areas/Example/Views/Self/ADM/ResourceData.cshtml";
			return View(_view);
		}

		[HttpPost]
		[HandlerAjaxOnly]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateExt(DataModel model)
		{
			if (string.IsNullOrEmpty(model.form.SID))
			{
				return Content(Insert(model).ToJson());
			}
			else
			{
				return Content(ResourceServices.Update(model).ToJson());
			}
		}

		[HttpPost]
		[HandlerAjaxOnly]
		[ValidateAntiForgeryToken]
		public ActionResult Add_ROLE(string RESOURCE_SID)
		=> _Content(c => f_Add_ROLE(RESOURCE_SID));

		public ActionResult Insert(DataModel model)
		=> _Content((c) => ResourceServices.Insert(model));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Txn"></param>
		/// <param name="RESOURCE_SID"></param>
		/// <returns></returns>
		public static IResult f_Add_ROLE(string RESOURCE_SID)
		=> WIPInjectServices.TxnBase.LzDBTrans(null, Txn=>{
				/* 因為專案編譯的需求 先 mark 掉
				var role = Txn.EFQuery_MVC.AD_ROLE.Where(c => c.ROLE_NO == "Admin").FirstOrDefault();
				Check.Invalid("AD_ROLE 查無 Admin 帳號", role == null);

				var ROLE_res = new AD_ROLE_RESOURCE()
				{
					SID = Txn.GetSID(),
					ROLE_SID = role.SID,
					RESOURCE_SID = RESOURCE_SID,
					RESOURCE_TYPE = "0"
				};
				var chk = Txn.EFQuery_MVC.AD_ROLE_RESOURCE
					.Where(c => c.RESOURCE_SID == RESOURCE_SID
						&& c.ROLE_SID == role.SID)
					.Any();
				if (chk == false)
				{
					Txn.EFQuery_MVC.AD_ROLE_RESOURCE.Add(ROLE_res);
					Txn.EFQuery_MVC.SaveChanges();
				}
				//*/
			return Txn.result;
		});
	}
}

namespace Genesis.Areas.ADM.Controllers
{
	/// <summary>
	/// 系統參數群組維護 
	/// ADM/Parameter
	/// </summary>
	public partial class ParameterGroupController : BaseController
	{
		private ParameterServices _service;
		public ActionResult ParameterGroupData_t(string keyVal)
		{
			dynamic data = new ExpandoObject();
			ParameterGroupViewModel model = null;
			if (keyVal != null) model = _groupService.GetParameterGroupData(keyVal);
			data.form = model ?? new ParameterGroupViewModel() { PARAMETERGROUP_TYPE = "Custom" };
			data.ParameterGroupTypes = _groupService.GetParameterGroupTypes();
			data.AllParameters = _paramService.GetTransferUIParameters();
			var result = new Result(true) { Data = data };

			ViewData["Model"] = result.ToJson(true);
			ViewData["SingleModel"] = true;
			var _view = "~/Areas/Example/Views/Self/ADM/ParameterGroupData.cshtml";
			return View(_view);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="PARAMETERGROUP_SID"></param>
		/// <param name="isTest"></param>
		/// <returns></returns>
		/// Ref 
		/// N:\CUB_Dev\Library\BLL\ADM\ParameterGroupServices.cs
		public ActionResult Add_Item(string PARAMETERGROUP_SID, string Data, string isTest = "F")
		=> _Content((c)=>{
			var arr = Data.Split('\n');
			var _list = new List<AD_PARAMETER>();
			foreach (string s in arr) {
				var _obj = new AD_PARAMETER() { 
					PARA_SID = Guid.NewGuid().ToString(), 
					PARAMETER_NO = s,
					PARAMETER_VALUE = s,
					PARAMETER_NAME = s,
					PARAMETER_TYPE = "SystemCode",
				};
				_list.Add(_obj);
			}
			if (_list.Count != 0) return Ext.Add_Item(PARAMETERGROUP_SID, _list, isTest == "T");
			return null;
		});

		
	}
}

namespace Genesis
{
	public class Ext
	{
		public static IResult Add_Item(string PARAMETERGROUP_SID, List<AD_PARAMETER> list, bool isTest = false)
		=> WIPInjectServices.TxnBase.LzDBTrans(null, Txn =>
		{
			var chk_list = list.Select(c => c.PARAMETER_NO).ToList();
			var err_list = Txn.EFQuery_MES.AD_PARAMETER
				.Where(c => chk_list.Contains(c.PARAMETER_NO))
				.ToList();
			Check.Invalid("PARAMETER_NO 已存在!", err_list.Count != 0, err_list);

			var _max = Txn.EFQuery_MES.AD_PARAMETERGROUP_LIST
				.Where(c => c.PARAMETERGROUP_SID == PARAMETERGROUP_SID)
				.Max(c => c.LIST_SEQ);
			if (_max != 0) _max++;

			List<AD_PARAMETERGROUP_LIST> groupLists = new List<AD_PARAMETERGROUP_LIST>();
			for (var idx = 0; idx < list.Count; idx++)
			{
				var _val = list[idx];
				Txn.EntityCommonSetVal(_val);
				var _obj = new AD_PARAMETERGROUP_LIST
				{
					PARAMETERGROUP_LIST_SID = Txn.GetSID(),
					PARAMETERGROUP_SID = PARAMETERGROUP_SID,
					PARAMETER_SID = _val.PARA_SID,
					LIST_SEQ = _max + idx,
					CREATE_USER = Txn.UserNo,
					CREATE_DATE = Txn.ExeTime,
					UPDATE_USER = Txn.UserNo,
					UPDATE_DATE = Txn.ExeTime,
				};
				groupLists.Add(_obj);
			}
			Txn.EFQuery_MES.AD_PARAMETER.AddRange(list);
			Txn.EFQuery_MES.AD_PARAMETERGROUP_LIST.AddRange(groupLists);
			Txn.EFQuery_MES.SaveChanges();
			return Txn.result;
		}, isTest);
 
		public static bool isEnable(int? val){
			if (val == null) return false;
			return val != 0;
		}
		public static bool isEnable(object val)
		=> val != null;

	}
}

//*
//BundleConfig~.cs 中 , 預設是用 System.Web.Mvc , 
//	所以一定要使用全名 -- System.Web.Http.Route , 
//	不然設定不會產生作用
namespace Genesis.WebApi
{
	[System.Web.Http.RoutePrefix("api")]
	public partial class SelfInfoController : System.Web.Http.ApiController
	{
		public class d_批號 {
			public int isTest;
			public int? CheckIn;
			public string LOT;
			public string SID;
			public d_批號_Act Action;
		}
		public class d_批號_Act {
			public int _全部流程的工作站擴展;
			public int _依據料號流程工站設定取得原因碼;
			public int _測試再製品查詢;
		}

		/*
		[System.Web.Http.AllowAnonymousAttribute]
		[System.Web.Http.Route("批號")]
		public dynamic 批號(d_批號 data)
		=>TxnBase.LzDBQuery<dynamic>(Txn => {
			var lot = (from a in Txn.EFQuery_MES.WP_LOT
					   where a.LOT == data.LOT || a.LOT_SID == data.SID
					   select a).FirstOrDefault_CheckExists();
			if (Ext.isEnable(data.Action?._全部流程的工作站擴展)){
				dynamic r1 = new ExpandoObject();
				var RouteVerInfo = lot.to_LotInfo(Txn).GetRouteVersionInfo();
				var Opers = RouteVerInfo.GetRouteVersionOperationList();
				r1.Opers = Opers;
				r1.Settings = (from a in Opers
							   select new
							   {
								   a.OPER_SID,
								   a.OPERATION,
								   a.OPER_SEQ,
								   Setting = ts_OperExpandSetting(Txn, a.OPER_SID, data.Action._全部流程的工作站擴展),
							   })
							   .Where(c=>c.Setting !=null)
							   .ToList();
				r1.lot = lot;
				return r1;
			}else if (Ext.isEnable(data.Action?._依據料號流程工站設定取得原因碼)){
				return DDLServices.GetPartNoOperReasonCodeData_OperSid(lot.LOT, lot.to_LotInfo(Txn));
			}else if (Ext.isEnable(data.Action?._測試再製品查詢)){
				return (from w0 in Txn.EFQuery_MES.PF_PARTNO
						where w0.PARTNO == data.SID
						select w0
					).ToList();
				//return (from w0 in Txn.EFQuery_MES.WP_LOT
				//	join p1 in Txn.EFQuery_MES.PF_PARTNO 
				//		on w0.PARTNO equals p1.PARTNO
				//		into bGroup
				//		from p1 in bGroup.DefaultIfEmpty()
				//		select new { w0, bGroup }
				//	).ToList();
			}
			return lot;
			//return Txn.result;
   //         var isQueryByLotNo = LOT != null;
			//var queryKey = isQueryByLotNo ? LOT : LOT_SID;
			//var LotInfo = Txn.GetLotInfo(queryKey, false, isQueryByLotNo);
			//dynamic ROUTE = new ExpandoObject();
			//dynamic OPER = new ExpandoObject();
			//var key = ActName?.ToUpper();
			//switch (key) {
			//	case "ROUTE":
			//		var RouteVerInfo = LotInfo.GetRouteVersionInfo();
			//		var Opers = RouteVerInfo.GetRouteVersionOperationList();
			//		var Settings = (from a in Opers
			//						select new
			//						{
			//							a.OPER_SID,
			//							a.Name,
			//							Setting = WIPOperConfigServices.GetOperSetting(a.OPER_SID, false)
			//						}
			//		).ToList();
			//		ROUTE = new { RouteVerInfo, Opers, Settings};
			//		break;
			//	default:
			//		var Setting = WIPOperConfigServices.GetOperSetting(LotInfo.OPER_SID, false);
			//		var Reason = DDLServices.GetPartNoOperReasonCodeData_OperSid(LotInfo.SID, LotInfo);
			//		OPER = new { Setting, Reason};
			//		break;
			//}
			//Txn.result.Data = new
			//{
			//	LotInfo,
			//	ROUTE,
			//	OPER,
			//};
			//return Txn.result;
        });
		//*/

		dynamic ts_OperExpandSetting(ITxnBase Txn, string OPER_SID,int mode) {
			var r = Txn.EFQuery_MES.PF_OPERATION_EXPAND.Where(t => t.OPER_SID == OPER_SID).FirstOrDefault();
			if (r != null) {
				var root  = r.SETTING_JSON.ToObject<OperExpandSetting>();
				switch (mode) {
					case 9://按原始全部顯示
						return root;
						break;
					case 1://過濾出只有設定的部份
						var r2 = new Dictionary<string, Dictionary<string, dynamic>>();
						foreach (var Sub1 in root.GetType().GetProperties()){
							var key = Sub1.Name;
							var value = Sub1.GetValue(root);
							if (value != null){
								var r3 = new Dictionary<string, dynamic>();
								foreach (var Sub2 in value.GetType().GetProperties()) {
									var key1 = Sub2.Name;
									var val1 = Sub2.GetValue(value);
									if (checkIsEable(val1)) { 
										r3.Add(key1,val1);
									}
								}
								r2.Add(key, r3);
							}
						}
						return r2;
						break;
				}
			} 
			return null;
		}

        private bool checkIsEable(object val1){
			if (val1 is bool ) return (bool)val1;

			Type type = val1.GetType();
			// 嘗試取得名為 "enable" 的屬性
			PropertyInfo enableProperty = type.GetProperty("enable");
			if (enableProperty != null){
				var v = (bool)enableProperty.GetValue(val1);
				return v;
			}
			return false;
		}

        /// <summary>
        /// 測試
        /// </summary>
        /// <param name="Test"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
		[System.Web.Http.Route("Lot1")]
		public string LotInfox(string Test)
		=>TxnBase.LzDBQuery<string>(Txn=>{
			Check.Invalid("test-Invalid", Test == "Invalid");
			return Test;
		});

		[System.Web.Http.AllowAnonymousAttribute]
		[System.Web.Http.HttpGet]
		[System.Web.Http.Route("Oper/{OperNo}")]
		public dynamic 依工作站代碼找找出關聯流程(string OperNo, string Action = null)
		=> TxnBase.LzDBQuery<dynamic>(Txn =>{
			switch (Action) {
				case "Lots":
					return Txn.EFQuery_MES.f_Oper_找出一般批號(OperNo).ToList();
					break;
				//case "ParallelLots":
				//	return Txn.EFQuery_MES.f_Oper_找出併行工站批號(OperNo).ToList();
					break;
				case "關聯流程":
					return Txn.EFQuery_MES.f_Oper_找出關聯流程(OperNo).ToList();
					break;
			}
			return null;
		});
 
		[System.Web.Http.HttpGet]
		[System.Web.Http.Route("OperExtend")]
		[System.Web.Http.Route("OperExtend/{ExtendName}")]
		public dynamic 工作站擴展_相對有開啟的站(string ExtendName = null)
		=> TxnBase.LzDBQuery<dynamic>(Txn => {
			var x = Txn.EFQuery_MES.PF_OPERATION_EXPAND.ToList();
			if (string.IsNullOrWhiteSpace(ExtendName)) return x;
			return x.Where(c => {
				dynamic data = JsonConvert.DeserializeObject(c.SETTING_JSON);
				if (FindProperty(data.CheckInSet, ExtendName) ||
					FindProperty(data.CheckOutSet, ExtendName) ||
					FindProperty(data.AnySet, ExtendName)){
					return true;
				}
				return false;
			}).ToList();
		});

		public bool FindProperty(dynamic obj, string propertyName){
			JObject jobj = obj as JObject;
			if (jobj !=null){
				foreach (var property in jobj){
					var isSameName = property.Key.ToString() == propertyName;
					if (isSameName && (bool)property.Value) return true;
					if (property.Value is JObject){
						if (FindProperty(property.Value, "enable"))
							return true;
					}
				}
			}
			return false;
		}

 

		/*
		[System.Web.Http.Route("ROUTE_NO/{ROUTE_NO}")]
		[System.Web.Http.Route("ROUTE_VER_SID/{ROUTE_VER_SID}")]
		[System.Web.Http.Route("ROUTE_NO/{ROUTE_NO}/info/{info}")]
		[System.Web.Http.Route("ROUTE_VER_SID/{ROUTE_VER_SID}/info/{info}")]
		[System.Web.Http.HttpGet]

		public dynamic ROUTE(string ROUTE_NO = null, string ROUTE_VER_SID = null, string info = null)
        => TxnBase.LzDBQuery<dynamic>(Txn => {
			dynamic data = new ExpandoObject();
			data.ROUTE =  Txn.EFQuery_MES.PF_ROUTE_VER
				.Where(c=>c.ROUTE_NO == ROUTE_NO || c.ROUTE_VER_SID == ROUTE_VER_SID)
				.ToList();
			switch (info?.ToUpper()) {
				case "OPER":
					data.OPERs = (from a in Txn.EFQuery_MES.PF_ROUTE_VER_OPER
								  where a.ROUTE_VER_SID == ROUTE_VER_SID
								  orderby a.OPER_SEQ
								  select a).ToList();
					break;
			}
			return data;
        });
		*/


	}

	//TODO-tmp 用某個流程 ,直接 查出現下有那些站,站內有那些批號

	//[System.Web.Http.RoutePrefix("api/v1")]
	public class ApiTestController : System.Web.Http.ApiController
	{

		[System.Web.Http.Route("T003")]
		[System.Web.Http.HttpGet]
		public List<string> T003(string T003)
		=> new List<string>() { T003 };

		//[System.Web.Http.HttpGet]
		//public List<string> T002(string T003)
		//=> new List<string>() { T003 };
	}
}

 


//*/