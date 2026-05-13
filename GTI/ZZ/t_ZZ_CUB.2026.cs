using AutoMapper;
using BLL.InterFace;
using BLL.MES;
using BLL.MES.DataViews;
using Frame.Code;
using Frame.Code.Excel;
using Genesis.Gtimes.ADM;
using Genesis.Gtimes.Transaction.TOL;
using Genesis.Library.BLL.DTC;
using Genesis.Library.BLL.MES.OperTask;
using Genesis.Library.BLL.WRP;
using Genesis.Library.Frame.Code.Web.TableQuery;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;
using static Genesis.Library.BLL.ADM.BomServices;
using _Prd = Genesis.Library.BLL.MES.OperTask;
using _Func = Genesis.Library.BLL.MES.OperTask.Func;
using BLL.DataViews.Edc;
using static Genesis.Library.BLL.DTC.Lot;
using static Genesis.Gtimes.WIP.LotUtility;
using Genesis.Library.BLL.ADM;
using Genesis.Gtimes.Transaction.WIP;
using System.Data.Entity;
using Genesis.Library.BLL.MES.OperInfo;
using Genesis.Library.BLL.ZZ.CUB.OperTask;
using System.Threading;
using System.Globalization;
using Genesis.Gtimes.Common;
using BLL.MVC;
using Genesis.Library.BLL.ZZ.CUB;
using System.IO;
using Genesis;
using Genesis.Library.BLL.QMS;
//using _bllSvc = Genesis.Library.BLL.ZZ.CUB;
using _zz_OperInfo = Genesis.Library.BLL.ZZ.CUB.OperInfo;
using _zzAPI = Genesis.Library.BLL.ZZ.CUB.ApiService;
using _zzCodeRule = Genesis.Library.BLL.ZZ.CUB.CodeRule;
using static Genesis.Library.BLL.ZZ.CUB.OperInfo.Basic;
using Newtonsoft.Json.Linq;
using Genesis.Gtimes.Transaction.EQP;
using Genesis.Gtimes.Transaction.CAR;
using Dal.Repository;
using Microsoft.EntityFrameworkCore;
using Genesis.Library.BLL;
using Genesis.Library.BLL.MES.AutoGenerate;
using Frame.Code.Web.Select;

using Genesis.Library.BLL.ZZ.CUB.CodeRule;
using Genesis.Library.BLL.PMS.Definition;
using BLL.PMS;

namespace UnitTestProject
{
	/// <summary>
	/// </summary>
	public partial class t_ZZ_CUB : _testBase
	{


		[TestMethod]
		public void t_20260507_consumedMaterialInfo()
        => _DBTest((txn) =>
        {
            var _src = FileApp.Read_SerializeJson<List<ZZ_WP_LOT_OPER_PARALLEL_CONSUM>>(_log.t_consumedMaterialInfo);

            // 1. 設定測試資料以對齊 WH_BILL_DATA 屬性
            var testWo = "WO-TEST-20260507";
            var testPartNo = "PART-TEST-20260507";
            var testBatchNo = "BATCH-TEST-20260507";
            var testSn = "SN-TEST-20260507";
            var testQty = 8.5m;

            _src[0].WO = testWo;
            _src[0].PARTNO = testPartNo;
            _src[0].TARGET_NO = testBatchNo;
            _src[0].TARGET_CUST_ONLYKEY = testSn;
            _src[0].CONSUM_QUANTITY = testQty;

            // 2. Mock 建立 WH_BILL (工單領料單)
            var mockBill = new WH_BILL()
            {
                BILL_NO = "BILL-TEST-20260507",
                MES_WO = testWo,
                DEPT_CODE = "FACTORY-TEST",
                CREATE_USER = "SYS-TEST",
                CREATE_DATE = txn.ExeTime,
                UPDATE_USER = "SYS-TEST",
                UPDATE_DATE = txn.ExeTime
            };
            txn.EFQuery_MES.WH_BILL.Add(mockBill);

            // 3. Mock 建立 WH_BILL_DATA (工單領料單明細 - 包含 ITEM_CODE 對應批號以進行方案 B 匹配)
            var mockBillData = new WH_BILL_DATA()
            {
                BILL_DATA_SID = "DATA-SID-20260507",
                BILL_NO = "BILL-TEST-20260507",
                PART_NO = testPartNo,
                ITEM_CODE = testBatchNo, // 批號對齊 TARGET_NO
                PART_NAME = "測試燈罩線組",
                DESCRIPTION = "測試規格描述",
                ITEM_QTY = 15.0m,
                ITEM_UNIT = "PCS",
                ITEM_SEQ = 1,
                CREATE_USER = "SYS-TEST",
                CREATE_DATE = txn.ExeTime,
                UPDATE_USER = "SYS-TEST",
                UPDATE_DATE = txn.ExeTime
            };
            txn.EFQuery_MES.WH_BILL_DATA.Add(mockBillData);
            txn.EFQuery_MES.SaveChanges();

            // 4. 執行首次領料自動勾選寫檔
            _zzAPI.consumedMaterialInfo_領料勾選(txn, "HEADER-SID-TEST", "USER-TEST", _src);

            // 5. 驗證首筆是否正確寫入 ZZ_PICKING_LIST_HIST
            var insertedRecord = txn.EFQuery_MES.ZZ_PICKING_LIST_HIST.FirstOrDefault(x => x.SN == testSn);
            Assert.IsNotNull(insertedRecord, "首次寫入應成功且產生 ZZ_PICKING_LIST_HIST 紀錄");
            Assert.AreEqual(testWo, insertedRecord.WO);
            Assert.AreEqual(testPartNo, insertedRecord.PART_NO);
            Assert.AreEqual(testBatchNo, insertedRecord.MLOT);
            Assert.AreEqual(testQty, insertedRecord.QTY);
            Assert.AreEqual("T", insertedRecord.STATUS);
            Assert.AreEqual("OK", insertedRecord.PICKING_RESULT);
            Assert.AreEqual("DATA-SID-20260507", insertedRecord.BILL_DATA_SID);

            // 6. 再次執行以驗證重複防寫機制 (避開重複寫入同一 SN)
            _zzAPI.consumedMaterialInfo_領料勾選(txn, "HEADER-SID-TEST", "USER-TEST", _src);

            var totalRecords = txn.EFQuery_MES.ZZ_PICKING_LIST_HIST.Count(x => x.SN == testSn);
            Assert.AreEqual(1, totalRecords, "重複呼叫時應直接略過，不應新增重複 SN 的紀錄");
        }, true, true);
	}



}
//todo-CUB
