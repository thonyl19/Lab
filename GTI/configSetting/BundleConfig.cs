using BundleTransformer.Core.Transformers;
using System;
using System.Linq;
using System.Web;
using System.Web.Optimization;

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

	public interface IGTI_Test
	{
		string Debug { get; set; }
		IHtmlString methods { get; set; }
		IHtmlString mounted { get; set; }
		IHtmlString param_test { get; }

		IHtmlString Test(string code, int mode = 0);
	}

}
