using BLL.DataViews.Edc;
using BLL.MES;
using MDL.MES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitTestProject.TestUT;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject
{
	public partial class t_EDC : _testBase
	{
		
		[TestMethod]
		public void t_NumCheckStyle()
		=> _DBTest(txn => {
			var src = FileApp.Read_SerializeJson<List<EdcModel>>(_log.t_edcData);
			var t_obj = src[0];
			Assert.IsTrue(t_obj.CheckPass(), "必填,符合上下限");
			t_obj.mustInput = "F";
			Assert.IsTrue(EdcModel.CheckNum(t_obj), "非必填,符合上下限");

			t_obj.mustInput = "T";
			t_obj.InputValueList = new List<Dictionary<string, string>>() {
				new Dictionary<string, string>(){ { "1", "5" } },
			};
			Assert.IsFalse(EdcModel.CheckNum(t_obj), "必填,不符合上下限");
			t_obj.mustInput = "F";
			Assert.IsFalse(EdcModel.CheckNum(t_obj), "非必填,不符合上下限");


			t_obj.mustInput = "T";
			t_obj.InputValueList = new List<Dictionary<string, string>>() {
				new Dictionary<string, string>(){ { "1", "" } },
			};
			Assert.IsFalse(EdcModel.CheckNum(t_obj), "必填,傳入空值");
			t_obj.mustInput = "F";
			Assert.IsTrue(EdcModel.CheckNum(t_obj), "非必填,傳入空值");

			t_obj.mustInput = "T";
			t_obj.InputValueList = new List<Dictionary<string, string>>() {
				new Dictionary<string, string>(){ { "1", "1" } },
				new Dictionary<string, string>(){ { "2", "" } }
			};
			Assert.IsFalse(EdcModel.CheckString(t_obj), "文字必填,應該要檢核出有空值");


			t_obj.mustInput = "F";
			Assert.IsTrue(EdcModel.CheckString(t_obj), "文字非必填,應該要通過");

			t_obj.mustInput = "T";
			t_obj.DataType = "B";
			t_obj.InputValueList = new List<Dictionary<string, string>>() {
				new Dictionary<string, string>(){ { "1", "" } },
			};
			Assert.IsFalse(t_obj.CheckPass(), "文字必填,應該要檢核出錯誤");

			t_obj.InputValueList = new List<Dictionary<string, string>>() {
				new Dictionary<string, string>(){ { "1", "N" } },
				new Dictionary<string, string>(){ { "2", "Y" } },
			};
			Assert.IsTrue(t_obj.CheckPass(), "文字必填,應該要檢核通過");

			t_obj.mustInput = "F";
			t_obj.InputValueList = new List<Dictionary<string, string>>() {
				new Dictionary<string, string>(){ { "1", "-" } },
				new Dictionary<string, string>(){ { "2", "" } },
			};
			Assert.IsFalse(t_obj.CheckPass(), "文字非必填,應該要檢核出不合法字完");

		}, true, true);


		public bool NumCheckStyle(EdcModel rowData, string itemVal)
		{
			if (string.IsNullOrEmpty(itemVal))
			{
				string mustInput = rowData.mustInput;
				rowData.pass = mustInput == "F";
				return true;
			}

			decimal? chkU = rowData.USL;
			decimal? chkL = rowData.LSL;

			// Determine the check type
			string chkType = $"{(chkU.HasValue ? 'T' : 'F')}{(chkL.HasValue ? 'T' : 'F')}";

			// Determine if the value passes based on the check type
			bool isPass = true;
			decimal numVal;
			switch (chkType)
			{
				case "FF":  // No limits
					break;
				case "TF":  // Upper limit only
					isPass = decimal.TryParse(itemVal, out numVal) && numVal <= chkU.Value && numVal > 0;
					break;
				case "FT":  // Lower limit only
					isPass = decimal.TryParse(itemVal, out numVal) && numVal >= chkL.Value && numVal > 0;
					break;
				case "TT":  // Both upper and lower limits
					isPass = decimal.TryParse(itemVal, out numVal) && numVal <= chkU.Value && numVal >= chkL.Value;
					break;
				default:
					break;
			}

			rowData.pass = isPass;
			return isPass;
		}


	}


}
