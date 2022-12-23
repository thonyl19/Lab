using BLL.MES;
using BLL.MES.FluentValidation;
using Dal.Repository;
using Frame.Code;
using Genesis.Areas.MES.Controllers;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using UnitTestProject.TestUT;
using static Genesis.Gtimes.ADM.PartUtility;
using mdl = MDL.MES;

namespace UnitTestProject
{
	[TestClass]
	public class t_WO : _testBase
	{
		static class _log
		{
			internal static string t_Search_QCResult_Query
			{
				get
				{
					return FileApp.ts_Log(@"WO\t_Search_QCResult_Query.json");
				}
			}

			/// <summary>
			/// 測試工單查詢 join 的程序  
			/// </summary>
			internal static string t_WorkOrder_List
			{
				get
				{
					return FileApp.ts_Log(@"WO\t_WorkOrder_List.json");
				}
			}
		}


		[TestMethod]
		public void t_WOInfo()
		{
			string SID = "20121129-0002";
			var result = new WOUtility.WOInfo(this.DBC, SID, WOUtility.IndexType.SID);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_WOInfo.json"));

		}


		/// <summary>
		/// 根據傳入的工單號碼返回工單需要下線的所有批次信息 (WP_LOT)
		/// 主要是依據 stats = Create
		/// </summary>
		[TestMethod]
		public void t_取得工單內需要下線的Lot()
		{
			string SID = "GTI20091714413100061";
			var wo = new WOUtility.WOInfo(this.DBC, SID, WOUtility.IndexType.SID);
			var Lot = new LotUtility.LotFunction(DBC);
			var result = Lot.GetWOStartLotList(wo.SID);
			//new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_WOInfo.json"));
		}


		/// <summary>
		/// 根據傳入的工單號碼返回工單己下線的所有批次信息 (WP_LOT) 
		/// 主要是依據 stats = wait
		/// </summary>
		[TestMethod]
		public void t_取得工單內己下線的Lot()
		{
			string SID = "TestWOforRelease001";
			var wo = new WOUtility.WOInfo(this.DBC, SID, WOUtility.IndexType.SID);
			var Lot = new LotUtility.LotFunction(DBC);
			var result = Lot.GetWOWaitLotList(wo.SID);
			//new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_WOInfo.json"));
		}





		/// <summary>
		/// 取得 SO 訂單資訊
		/// </summary>
		[TestMethod]
		public void t_GetSOConditionSQL()
		{
			string SO = "TEST";
			var function = new SOUtility.SOFunction(this.DBC);
			Command comm = function.GetSOConditionSQL("Enable", SO, "", "");
			var result = function.GetSOLikeCondition(comm);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_GetSOConditionSQL.json"));

		}

		[TestMethod]
		public void t_SOTransaction()
		{
			string SO = "TEST";
			var dct = new SOUtility.SOTransaction();
			List<IDbCommand> commands = new List<IDbCommand>();

			List<Column> insertColumns = new List<Column>();
			Column clm1 = new Column("SO", "", "TEST_SO");
			insertColumns.Add(clm1);
			Column clm2 = new Column("SO_TYPE_SID", "", "--");
			insertColumns.Add(clm2);
			Column clm3 = new Column("CUSTOMER_SID", "", "--");
			insertColumns.Add(clm3);
			Column clm4 = new Column("DESCRIPTION", "", "--");
			insertColumns.Add(clm4);
			Column clm5 = new Column("ENABLE_FLAG", "", "T");
			insertColumns.Add(clm5);

			commands.AddRange(
				dct.AddSOTransaction(this.DBC
				, "SOData.aspx"
				, this.DBC.GetSID()
				, "TEST_SO"
				, insertColumns
				, "Admin"
				, this.DBC.GetDBTime())
			);
			this.DBC.DoTransaction(commands);
		}

		/// <summary>
		/// 取得 PO 採購單資訊
		/// </summary>
		[TestMethod]
		public void t_GetPOConditionSQL()
		{
			string PO = "GTI20040912254321421";
			var function = new POUtility.POFunction(this.DBC);
			Command comm = function.GetPoConditionSQL("Enable", PO, "", "");
			var result = function.GetPOLikeCondition(comm);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_GetPOConditionSQL.json"));

		}

		[TestMethod]
		public void t_POTransaction()
		{
			string SO = "TEST";
			var dct = new POUtility.POTransaction();
			List<IDbCommand> commands = new List<IDbCommand>();
			List<Column> insertColumns = new List<Column>();

			Column PO = new Column("PO", "", this.DBC.GetSID());
			insertColumns.Add(PO);
			//
			Column DESCRIPTION = new Column("DESCRIPTION", "", "DESCRIPTION");
			insertColumns.Add(DESCRIPTION);
			//
			Column ENABLE_FLAG = new Column("ENABLE_FLAG", "", "T");
			insertColumns.Add(ENABLE_FLAG);


			Column PO_TYPE_SID = new Column("PO_TYPE_SID", "", "PO_TYPE_SID");
			insertColumns.Add(PO_TYPE_SID);

			Column CUSTOMER_SID = new Column("CUSTOMER_SID", "", "CUSTOMER_SID");
			insertColumns.Add(CUSTOMER_SID);

			commands.AddRange(
				dct.AddTransaction(this.DBC
				, "POData.aspx"
				, this.DBC.GetSID()
				, "TEST_PO"
				, insertColumns
				, "Admin"
				, this.DBC.GetDBTime())
			);
			this.DBC.DoTransaction(commands);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void t_取得料號單位()
		{
			/*
        sql = @"SELECT '['||PARA.PARAMETER_NO||']~'||PARA.PARAMETER_NAME AS PARA_DSER,
            PARA.PARAMETER_NO,PARA.PARAMETER_NAME,PARA.PARAMETER_TYPE,PARA.PARAMETER_VALUE,PLIST.LIST_SEQ,PARA.DESCRIPTION 
            FROM    AD_PARAMETERGROUP PGROUP 
                    ,AD_PARAMETERGROUP_LIST PLIST
                    ,AD_PARAMETER PARA
            WHERE  PGROUP.PARAMETERGROUP_SID=PLIST.PARAMETERGROUP_SID 
            AND PLIST.PARAMETER_SID=PARA.PARA_SID 
            AND PGROUP.PARAMETERGROUP_NO=:GROUPNO 
            ORDER BY PARA.PARAMETER_NO ";
             */
			ParameterUtility.ParameterGroupFunction para = new ParameterUtility.ParameterGroupFunction(DBC);
			DataTable dt = para.GetGroupParaListByGroupNo("PartNo-Unit");

			var z = dt.Select("PARAMETER_NO = 'g'");

			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"WO\t_PartNo_Unit.json"));

		}

		[TestMethod]
		public void t_WO_Priority()
		{
			ParameterUtility.ParameterGroupFunction para = new ParameterUtility.ParameterGroupFunction(DBC);
			DataTable result = para.GetGroupParaListByGroupNo("WO-Priority");
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_WO_Priority.json"));

		}

		[TestMethod]
		public void t_Product()
		{
			SelectCommandBuilder builder = new SelectCommandBuilder(this.DBC, "PF_PRODUCT");
			builder.SelectColumns("PRODUCT,PRODUCT_SID,PRODUCT_NO");
			builder.OrderByColumn("PRODUCT_NO");
			DataTable result = builder.DoQuery();
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_Product.json"));

		}

		[TestMethod]
		public void t_GetPartNoVersionConditionSQL()
		{
			var PRODUCT_SID = "GTI12090317332700014";
			PartUtility.PartNoVersionFunction uf = new PartUtility.PartNoVersionFunction(this.DBC);
			Command conditionCommand = uf.GetPartNoVersionConditionSQL("%", "", VersionSate.Enable.ToString(), "", "", "", "", "", "", "", "", "", "", PRODUCT_SID);
			DataTable result = uf.GetPartNoVersionData(conditionCommand);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_GetPartNoVersionConditionSQL.json"));

		}

		[TestMethod]
		public void t_PartVersionInfo()
		{
			var PARTNO_VER_SID = "GTI19091710030205371";
			var _partVer = new PartVersionInfo(this.DBC, PARTNO_VER_SID);
		}



		[TestMethod]
		public void t_SQL_PARTNO_VER_SID()
		{
			string PARTNO_VER_SID = "GTI19091710030205371";

			var z = new WorkOrderController().PratNoVerRoute(PARTNO_VER_SID);
			new FileApp().Write_SerializeJson(z, FileApp.ts_Log(@"WO\t_SQL_PARTNO_VER_SID_1.json"));

			using (var dbc = this.DBC)
			{
				var _sql = $@"
                SELECT  A2.DEFAULT_LOT_SIZE
                FROM    PF_PARTNO_VER A0
                        INNER JOIN PF_PARTNO A1
                            ON A0.PARTNO_SID = A1.PARTNO_SID
                        INNER JOIN PF_PARTNO_TYPE  A2
                            ON A1.PARTNO_TYPE_SID = A2.PARTNO_TYPE_SID
                WHERE   A0.PARTNO_VER_SID = :PARTNO_VER_SID
                         ";
				List<IDbDataParameter> parameters = new List<IDbDataParameter>();

				dbc.AddCommandParameter(parameters, "PARTNO_VER_SID", PARTNO_VER_SID);
				_sql = dbc.GetCommandText(_sql, SQLStringType.SqlServerSQLString);

				var dt = dbc.Select(_sql, parameters);

				new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"WO\t_SQL_PARTNO_VER_SID.json"));

			}
		}

		/// <summary>
		/// 2015 - ddlPart_SelectedIndexChanged
		/// 以料號帶 流程
		/// </summary>
		[TestMethod]
		public void t_由料號取得流程和版本完整程序()
		{
			var PARTNO_SID = "GTI20022014053001905";
			PartUtility.PartInfo Partinfo = new PartUtility.PartInfo(this.DBC, PARTNO_SID, PartUtility.IndexType.SID);
			/*
             這個料號版本 在 2015 上是沒有顯示的(Visible="False"),
             經請教 Ivy 得到的答案是,系統以前是有 料號版本的規劃,
             但後來基於簡化的考量,後來己經沒有在用,所以直接帶預設值即可 
             */
			SelectCommandBuilder builder = new SelectCommandBuilder(this.DBC, "PF_PARTNO_VER");
			builder.SelectColumns("VERSION,PARTNO_VER_SID");
			builder.WhereAnd("PARTNO_SID", PARTNO_SID);
			builder.WhereAnd("VERSION_STATE", VersionSate.Enable.ToString());
			DataTable ddlPartVer = builder.DoQuery();

			//取得 DEFAULT_VERSION
			//ListItem lt = ddlPartVer.Items.FindByText(Convert.ToString(Partinfo.DEFAULT_VERSION));

			/*
                dtLotSize
             */

			/*
                由 PF_PARTNO
             */
			builder = new SelectCommandBuilder(DBC, "PF_PARTNO_TYPE");
			builder.SelectColumns("PARTNO_TYPE_SID,DEFAULT_LOT_SIZE");
			builder.WhereAnd("PARTNO_TYPE_SID", Partinfo.PARTNO_TYPE_SID);
			DataTable dtLotSize = builder.DoQuery();


			//用料號NO 取得 流程版本清單
			var PARTNO_VER_SID = "GTI20022014053001906";
			PartUtility.PartFunction PartFun = new PartUtility.PartFunction(this.DBC);
			DataTable dt = PartFun.GetPratNoVerRouteList(PARTNO_VER_SID);

			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"WO\t_GetPratNoVerRouteList.json"));

			var ROUTE_SID = "GTI20022014001501891";
			var rows = dt.Select("DEFAULT_FLAG = 'T'");
			if (rows.Length != 0)
			{
				ROUTE_SID = rows[0]["ROUTE_SID"].ToString();
			}

			//用  流程版本清單.ROUTE_SID   取得 版號清單
			/* 中間這段可以不必要
            */
			var routeinfo = new RouteUtility.RouteInfo(this.DBC, ROUTE_SID, RouteUtility.IndexType.SID);
			new FileApp().Write_SerializeJson(routeinfo, FileApp.ts_Log(@"WO\t_GetRouteVersionDataTable_routeinfo.json"));

			RouteUtility.RouteFunction fun = new RouteUtility.RouteFunction(this.DBC);
			DataTable dt2 = fun.GetRouteVersionDataTable(ROUTE_SID, VersionSate.Enable.ToString());
			new FileApp().Write_SerializeJson(dt2, FileApp.ts_Log(@"WO\t_GetRouteVersionDataTable.json"));

		}


		/// <summary>
		/// 2015 - ddlPart_SelectedIndexChanged
		/// 以料號帶 流程
		/// </summary>
		[TestMethod]
		public void t_由料號類別取得流程和版本完整程序()
		{
			//Table - PF_PARTNO_TYPE
			// 由此程序 查詢出 ROUTE_SID
			PartTypeUtility.PartTypeFunction parttype = new PartTypeUtility.PartTypeFunction(this.DBC);
			Command conditionCommand = parttype.GetPartTypeConditionSQL("Enable", "", "%");
			DataTable dt = parttype.GetPartTypeLikeCondition(conditionCommand);
			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"WO\t_由料號類別取得流程和版本完整程序_dt.json"));

			//以下這一段可以略過不用處理 
			var PARTNO_TYPE_SID = dt.Rows[0]["PARTNO_TYPE_SID"].ToString();
			RouteUtility.RouteFunction routeFun = new RouteUtility.RouteFunction(DBC);
			var parttype1 = new PartTypeUtility.PartTypeInfo(this.DBC, PARTNO_TYPE_SID, PartTypeUtility.IndexType.SID);
			new FileApp().Write_SerializeJson(parttype1, FileApp.ts_Log(@"WO\t_由料號類別取得流程和版本完整程序_parttype1.json"));

			//資料內容不足以使用
			//var DataSource = routeFun.GetEnableAndDefRouteVerAndSelectedIndex(parttype1.ROUTE_SID);
			var routeinfo = new RouteUtility.RouteInfo(DBC, parttype1.ROUTE_SID, RouteUtility.IndexType.SID);
			new FileApp().Write_SerializeJson(routeinfo, FileApp.ts_Log(@"WO\t_由料號類別取得流程和版本完整程序_routeinfo.json"));

			//var _svc = new EFRepository<mdl.PF_ROUTE>(dbContext);
			//var x =  from resList in dbContext.PF_ROUTE.Where(i => i.ROUTE_SID == "%")

			//資料內容不足以使用
			var _svc = new BLL.MES.RouteServices();
			var routeinfo1 = _svc.getData("GTI19050800123202206");
			new FileApp().Write_SerializeJson(routeinfo1, FileApp.ts_Log(@"WO\t_由料號類別取得流程和版本完整程序_routeinfo1.json"));

			//最後改使用 t_RouteVerServices 程序

		}
		[TestMethod]
		public void t_RouteVerServices()
		{
			//不合用,因為只會取一筆回來而己
			//var RouteVerServices = new RouteVerServices().getData("GTI12090317455300027");
			//new FileApp().Write_SerializeJson(RouteVerServices, FileApp.ts_Log(@"WO\t_RouteVerServices.json"));




			var ROUTE_SID = "GTI12090317455300027";
			var dbc = mes.dbc();

			List<IDbDataParameter> parameters_1 = new List<IDbDataParameter>();
			dbc.AddCommandParameter(parameters_1, "search", "%");

			var _sql1 = $@"
                        SELECT  A0.PARTNO_TYPE_SID,
                                A0.PARTNO_TYPE_NO  ,
                                A0.PARTNO_TYPE_NAME ,
                                A0.ROUTE_SID,
                                A0.DEFAULT_LOT_SIZE
                        FROM    PF_PARTNO_TYPE A0
                        WHERE   A0.ENABLE_FLAG = 'T'
                                AND (A0.PARTNO_TYPE_SID like :search
                                    OR A0.PARTNO_TYPE_NO like :search
                                    OR A0.PARTNO_TYPE_NAME like :search)
                         ";
			_sql1 = dbc.GetCommandText(_sql1, SQLStringType.SqlServerSQLString);
			var dt1 = dbc.Select(_sql1, parameters_1);
			new FileApp().Write_SerializeJson(dt1, FileApp.ts_Log(@"WO\t_RouteVerServices_PF_PARTNO_TYPE.json"));



			var _sql = $@"
                        SELECT  A0.ROUTE_SID,
                                A0.ROUTE AS ROUTE_NAME,
                                A0.DEFAULT_FLAG
                        FROM    PF_ROUTE_VER A0
                        WHERE   A0.ROUTE_SID = :ROUTE_SID
                                AND A0.VERSION_STATE = 'Enable'
                         ";
			List<IDbDataParameter> parameters = new List<IDbDataParameter>();
			dbc.AddCommandParameter(parameters, "ROUTE_SID", ROUTE_SID);
			_sql = dbc.GetCommandText(_sql, SQLStringType.SqlServerSQLString);
			var dt = dbc.Select(_sql, parameters);
			new FileApp().Write_SerializeJson(dt, FileApp.ts_Log(@"WO\t_RouteVerServices_1.json"));

		}
		/// <summary>
		/// 2015 - ddlRoute_SelectedIndexChanged
		/// 以流程帶相應的版本資訊
		/// </summary>
		[TestMethod]
		public void t_GetPartNoVersionConditionSQL_()
		{
			var ROUTE_SID = "GTI19121911040809578";
			PartUtility.PartNoVersionFunction uf = new PartUtility.PartNoVersionFunction(this.DBC);
			Command conditionCommand = uf.GetPartNoVersionConditionSQL("%", "", VersionSate.Enable.ToString(), "", "", "", "", "", "", "", "", "", "", ROUTE_SID);
			DataTable result = uf.GetPartNoVersionData(conditionCommand);
			new FileApp().Write_SerializeJson(result, FileApp.ts_Log(@"WO\t_GetPartNoVersionConditionSQL.json"));

			/*
             routeinfo = new RouteUtility.RouteInfo(DBC, ddlRoute.SelectedValue, RouteUtility.IndexType.SID);
                RouteUtility.RouteFunction fun = new RouteUtility.RouteFunction(this.DBC);
                DataTable dt = fun.GetRouteVersionDataTable(routeinfo.SID, VersionSate.Enable.ToString());
                if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                {
                    throw new Exception(string.Format(Resources.Message.SelectFieldRelateFieldIsNull, lblRoute.Text, lblRouteVer.Text));
                }
                dt = CommUtility.DataTableTrimDecimalZero(dt);
                ddlRouteVer.DataSource = dt;
                ddlRouteVer.DataTextField = "VERSION";
                ddlRouteVer.DataValueField = "ROUTE_VER_SID";
                ddlRouteVer.DataBind();
             
             */
		}

		/// <summary>
		/// 提供 取得前端 post 資料,做單元測試的程序 
		/// </summary>
		[TestMethod]
		public void t_WO_Add_1()
		{
			var item = FileApp.Read_SerializeJson<mdl.WP_WO>(FileApp.ts_Log(@"WO\t_WP_WO_x.json"));
			var r = new WOServices().Create(item, true);
			new FileApp(false).Write_SerializeJson(r, FileApp.ts_Log(@"WO\t_WP_WO_x1.json"));
		}

		[TestMethod]
		public void t_WO_Update()
		{
			var item = FileApp.Read_SerializeJson<mdl.WP_WO>(FileApp.ts_Log(@"WO\t_WP_WO.json"));
			var r = new WOServices().Update(item, true);
			new FileApp(false).Write_SerializeJson(r, FileApp.ts_Log(@"WO\t_WP_WO_2.json"));
		}


		[TestMethod]
		public void t_WO_Add()
		{
			var _wo = new MDL.MES.WP_WO()
			{
				WO_SID = "",
				PRODUCT_SID = "GTI12090317332700014",
				PARTNO = "140-DBR1394A",

			};
			WOUtility.WOInfo woInfo = new WOUtility.WOInfo(DBC, _wo.WO_SID, WOUtility.IndexType.WO);
			if (woInfo.IsExist == true)
			{
				//throw new Exception(string.Format(Resources.Message.InputDataDuplicate, lblNO.Text));
			}

			InsertCommandBuilder builder;
			string user = ""; //this.User.Identity.Name;
			DateTime updateTime = this.DBC.GetDBTime();
			builder = new InsertCommandBuilder(this.DBC, "WP_WO");
			List<IDbCommand> commands = new List<IDbCommand>();
			if (_wo.WO_SID.Trim().Length == 0)
			{
				EncodeFormatUtility.EncodeFormatInfo Encode = new EncodeFormatUtility.EncodeFormatInfo
				(DBC, "WorkOrderNo", EncodeFormatUtility.IndexType.No);
				if (Encode.IsExist == true)
				{
					//EncodeFormatUtility.CodesInfo Code = EncodeFormatUtility.Coder.GetCodes(DBC, user, Encode, 1, "", "", _wo.PARTNO, ddlPart.SelectedItem.Text, "", false);
					var Code = EncodeFormatUtility.Coder.GetCodes
						(DBC, user, Encode, 1, "", ""
						, _wo.PARTNO, "", false);
					_wo.WO_SID = Code.Codes[0];
					commands.AddRange(Code.Commands);
				}
				else
				{
					_wo.WO_SID = this.DBC.GetSID();
				}
			}
			builder.InsertColumn("WO_SID", _wo.WO_SID);
			builder.InsertColumn("WO", _wo.WO_SID);
			builder.InsertColumn("SO", _wo.SO);
			builder.InsertColumn("PO", _wo.PO);
			builder.InsertColumn("WO_TYPE", _wo.WO_TYPE);
			builder.InsertColumn("STATUS", _wo.STATUS);
			builder.InsertColumn("ERP_STATUS", "");
			//builder.InsertColumn("QUANTITY",  _wo.QUANTITY == null ? (decimal?)null : Convert.ToDecimal(txtQuantity.Text.Trim()));

			//builder.InsertColumn("UNRELEASE_QUANTITY", txtQuantity.Text.Trim());
			//builder.InsertColumn("UNIT", ddlUnit.SelectedValue.Trim());
			//builder.InsertColumn("QTY_1", txtQTY1.Text.Trim() == "" ? (decimal?)null : Convert.ToDecimal(txtQTY1.Text.Trim()));
			//builder.InsertColumn("UNRELEASE_QTY_1", txtQTY1.Text.Trim() == "" ? (decimal?)null : Convert.ToDecimal(txtQTY1.Text.Trim()));
			//builder.InsertColumn("UNIT_1", ddlUnit1.SelectedValue.Trim());
			//builder.InsertColumn("QTY_2", txtQTY2.Text.Trim() == "" ? (decimal?)null : Convert.ToDecimal(txtQTY2.Text.Trim()));
			//builder.InsertColumn("UNRELEASE_QTY_2", txtQTY2.Text.Trim() == "" ? (decimal?)null : Convert.ToDecimal(txtQTY2.Text.Trim()));
			//builder.InsertColumn("UNIT_2", ddlUnit2.SelectedValue.Trim());
			builder.InsertColumn("LOT_SIZE", _wo.LOT_SIZE);
			builder.InsertColumn("ROUTE_VER_SID", _wo.ROUTE_VER_SID);
			builder.InsertColumn("ROUTE", _wo.ROUTE);
			builder.InsertColumn("ROUTE_VERSION", _wo.ROUTE_VERSION);


			if (!String.IsNullOrEmpty(_wo.FACTORY))
			{
				builder.InsertColumn("FACTORY", _wo.FACTORY);
				UpdateCommandBuilder ucb = new UpdateCommandBuilder(DBC, "pf_factory");
				ucb.UpdateColumn("QUOTE_ONCE", "T");
				ucb.WhereAnd("FACTORY_SID", _wo.FACTORY);
				commands.AddRange(ucb.GetCommands());
			}

			if (!String.IsNullOrEmpty(_wo.SO))
			{
				UpdateCommandBuilder ucb = new UpdateCommandBuilder(DBC, "wp_so");
				ucb.UpdateColumn("QUOTE_ONCE", "T");
				ucb.WhereAnd("SO", _wo.SO);

				commands.AddRange(ucb.GetCommands());
			}
			if (!String.IsNullOrEmpty(_wo.PO))
			{
				UpdateCommandBuilder ucb = new UpdateCommandBuilder(DBC, "wp_po");
				ucb.UpdateColumn("QUOTE_ONCE", "T");
				ucb.WhereAnd("PO", _wo.PO);

				commands.AddRange(ucb.GetCommands());
			}

			if (_wo.ERP_QUANTITY != null)
			{
				builder.InsertColumn("ERP_QUANTITY", _wo.ERP_QUANTITY);
			}
			if (!String.IsNullOrEmpty(_wo.ECN_NO))
			{
				ECNUtility.ECNInfo ecnInfo = new ECNUtility.ECNInfo(DBC, _wo.ECN_NO, ECNUtility.IndexType.SID);
				builder.InsertColumn("ECN_SID", ecnInfo.SID);
				builder.InsertColumn("ECN_NO", ecnInfo.No);
				builder.InsertColumn("ECN_NAME", ecnInfo.Name);

				UpdateCommandBuilder ucb = new UpdateCommandBuilder(DBC, "FC_ECN");
				ucb.UpdateColumn("QUOTE_ONCE", "T");
				ucb.WhereAnd("ECN_SID", ecnInfo.SID);

				commands.AddRange(ucb.GetCommands());
			}
			if (!String.IsNullOrEmpty(_wo.WO_LINE))
			{
				LineUtility.LineInfo olineInfo = new LineUtility.LineInfo(DBC, _wo.WO_LINE, LineUtility.IndexType.SID);
				builder.InsertColumn("WO_LINE_SID", olineInfo.SID);
				builder.InsertColumn("WO_LINE_NO", olineInfo.No);
				builder.InsertColumn("WO_LINE", olineInfo.Name);

				UpdateCommandBuilder ucb = new UpdateCommandBuilder(DBC, "AD_LINE");
				ucb.UpdateColumn("QUOTE_ONCE", "T");
				ucb.WhereAnd("LINE_SID", olineInfo.SID);

				commands.AddRange(ucb.GetCommands());
			}
			ProductUtility.ProductInfo prodinfo = new ProductUtility.ProductInfo(DBC, _wo.PRODUCT_SID, ProductUtility.IndexType.SID);
			builder.InsertColumn("PRODUCT_SID", prodinfo.SID);
			builder.InsertColumn("PRODUCT", prodinfo.Name);
			builder.InsertColumn("PARTNO_VER_SID", _wo.PARTNO_VER_SID);
			//string[] s = ddlPart.SelectedItem.Text.Split('~');
			PartUtility.PartInfo partInfo = new PartUtility.PartInfo(DBC, _wo.PARTNO, PartUtility.IndexType.SID);
			builder.InsertColumn("PARTNO", partInfo.No);
			builder.InsertColumn("PARTNO_VERSION", _wo.PARTNO_VERSION);
			builder.InsertColumn("PRIORITY", _wo.PRIORITY);
			builder.InsertColumn("OWNER", _wo.OWNER);
			//builder.InsertColumn("CUSTOMER", txtCustomer.Text);
			builder.InsertColumn("CUSTOMER", _wo.CUSTOMER);
			builder.InsertColumn("BONDED_FLAG", _wo.BONDED_FLAG);
			builder.InsertColumn("BONDED_NO", _wo.BONDED_NO);
			if (_wo.SCHEDULEDATE != null)
			{
				builder.InsertColumn("SCHEDULEDATE", _wo.SCHEDULEDATE);
			}
			if (_wo.DUEDATE != null)
			{
				builder.InsertColumn("DUEDATE", _wo.DUEDATE);
			}


			builder.InsertColumn("ATTRIBUTE_01", _wo.ATTRIBUTE_01);
			builder.InsertColumn("ATTRIBUTE_02", _wo.ATTRIBUTE_02);
			builder.InsertColumn("ATTRIBUTE_03", _wo.ATTRIBUTE_03);
			builder.InsertColumn("ATTRIBUTE_04", _wo.ATTRIBUTE_04);
			builder.InsertColumn("ATTRIBUTE_05", _wo.ATTRIBUTE_05);
			builder.InsertColumn("ATTRIBUTE_06", _wo.ATTRIBUTE_06);
			builder.InsertColumn("ATTRIBUTE_07", _wo.ATTRIBUTE_07);
			builder.InsertColumn("ATTRIBUTE_08", _wo.ATTRIBUTE_08);
			builder.InsertColumn("ATTRIBUTE_09", _wo.ATTRIBUTE_09);
			builder.InsertColumn("ATTRIBUTE_10", _wo.ATTRIBUTE_10);
			builder.InsertColumn("ATTRIBUTE_11", _wo.ATTRIBUTE_11);
			builder.InsertColumn("ATTRIBUTE_12", _wo.ATTRIBUTE_12);
			builder.InsertColumn("ATTRIBUTE_13", _wo.ATTRIBUTE_13);
			builder.InsertColumn("ATTRIBUTE_14", _wo.ATTRIBUTE_14);
			builder.InsertColumn("ATTRIBUTE_15", _wo.ATTRIBUTE_15);
			builder.InsertColumn("ATTRIBUTE_16", _wo.ATTRIBUTE_16);
			builder.InsertColumn("CREATE_USER", user);
			builder.InsertColumn("CREATE_DATE", updateTime);
			builder.InsertColumn("UPDATE_USER", user);
			builder.InsertColumn("UPDATE_DATE", updateTime);

			builder.SetLogInfo("WorkOrderData.aspx", user, _wo.WO_SID, user, updateTime);

			commands.AddRange(builder.GetCommands());

			//DBC.DoTransaction(commands);
		}


		/// <summary>
		/// 檢核 WO 是否存在 
		/// </summary>
		[TestMethod]
		public void t_WO_check()
		{
			using (var dbContext = new MDL.MESContext())
			{
				var _refServices = new EFRepository<mdl.WP_WO>(dbContext);
				Func<string, Expression<Func<mdl.WP_WO, bool>>> expression = (string WO) =>
				{
					return ExtLinq.True<mdl.WP_WO>()
						.And(t => t.WO == WO);
				};
				var item1 = _refServices.Read(expression("20120903-0001"));
				new FileApp().Write_SerializeJson(item1, FileApp.ts_Log(@"WO\t_WP_WO.json"));
				var item2 = _refServices.Read(expression("20120903-0001x"));
			}
		}

		/// <summary>
		/// 檢核 WO 是否存在 
		/// </summary>
		[TestMethod]
		public void t_GetEnableCustomerNoNameNew()
		{
			var _r = ADMServices.GetEnableCustomerNoNameNew();
			new FileApp().Write_SerializeJson(_r, FileApp.ts_Log(@"WO\t_GetEnableCustomerNoNameNew.json"));

		}


		[TestMethod]
		public void t_()
		{
			chkDateInput
				(new[] { null, "A" }
				, new object[] { DateTime.Now, "B" }
				, new object[] { DateTime.Now, "B" }
				);

		}

		[TestMethod]
		public void t_WorkOrder_List()
		{
			var _x = new FileApp().Read_SerializeJson<Pagination>(_log.t_Search_QCResult_Query);

			var _x1 = new WOServices().getList(_x, "");
			new FileApp().Write_SerializeJson(_x1, FileApp.ts_Log(@"WO\t_WorkOrder_List_1.json"));

			//var _r = new FileApp().Read_SerializeJson<List<mdl.WP_WO>>(_log.t_WorkOrder_List);
			//using (var dbContext = new MDL.MESContext())
			//{
			//	var _r2 = _r.GroupBy(i => i.PARTNO_VER_SID)
			//		.ToList();
			//	var _r1 = (from t in dbContext.PF_PARTNO_VER
			//			   join A in _r on t.PARTNO_VER_SID equals A.PARTNO_VER_SID
			//			   //where (!string.IsNullOrEmpty(SHIFT_SID) && t.SHIFT_SID == SHIFT_SID)
			//			   //|| (!string.IsNullOrEmpty(LINE_SID) && t.LINE_SID == LINE_SID)
			//			   //|| (!string.IsNullOrEmpty(OPERATION_SID) && t.OPERATION_SID == OPERATION_SID)

			//			   select A
			//		)
			//		.ToList<Object>();

			//}



		}

		[TestMethod]
		public void t_WOValidation()
		{
			var entity = new mdl.WP_WO()
			{
				WO_TYPE = "x",
				PRIORITY = "x",
				QUANTITY = 10,
				LOT_SIZE = 11,
				PRODUCT_SID = "x",
				PARTNO_VER_SID = "x",
				ROUTE_VER_SID = "x",
				QTY_1 = 100,
				UNIT_1 = "XX"
			};
			var validationResult = new WOValidation().Validate(entity);
			if (!validationResult.IsValid)
			{
				var _r = string.Join("\r\n", validationResult.Errors.Select(e => e.ErrorMessage));
			}
		}

		void chkDateInput(params Object[][] lists)
		{
			foreach (var item in lists)
			{
				Object src = item[0],
					lab = item[1];
				if (src == null) continue;
				try
				{
					Convert.ToDateTime(src);
				}
				catch
				{
					var zz = 1;
					//throw new Exception(string.Format(RES.BLL.Message.InputFormatError, (string)lab));
				}
			}
		}

	}
}
