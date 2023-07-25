using Dal.Repository;
using Genesis.Gtimes.Common;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static BLL.MES.WIPInjectServices;

namespace UnitTestProject.TestUT
{
	public class App
	{


		public static dynamic Timer(Func<dynamic> fn)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			var result = fn();
			sw.Stop();
			TimeSpan ts2 = sw.Elapsed;
			//Console.WriteLine($"Stopwatch總共花費{ts2.TotalMilliseconds}ms.");
			return new { result , Cost = $"Stopwatch總共花費{ts2.TotalMilliseconds}ms." };
		}
	}

	public class FileApp
	{
		public System.IO.TextWriter go_TextWriter;
		public FileMode _FileMode = FileMode.CreateNew;
		public Encoding EncodingType;

		bool IsAppend
		{
			get
			{
				return this._FileMode == FileMode.Append;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isAppend">T)自動附加,F)強制複寫</param>
		public FileApp(bool isAppend)
		{
			this._FileMode
				= isAppend
				? FileMode.Append
				: FileMode.Create;
			this.SetEncoding();
		}

		/// <summary>
		/// FileMode.CreateNew -- 己存在的檔案,自動加日期新建
		/// </summary>
		public FileApp()
		{
			this.SetEncoding();
		}

		//public IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
		public static JsonSerializerSettings json_options = new JsonSerializerSettings()
		{
			DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
			DateFormatString = "yyyy-MM-dd HH:mm:ss",
			NullValueHandling = NullValueHandling.Include,
			Error = (serializer, err) =>
			{
				err.ErrorContext.Handled = true;
			}
		};

 

        public static string ts_Log(string PathName)
		{
			string _path = System.AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug", "");
			string _filePath = $@"{_path}Log\{PathName}";
			return _filePath;
		}

		/// <summary>
		/// 在檔案名稱和延伸名稱之間加上標記 ,標記預設為 yyyyMMdd_HHmmss
		/// </summary>
		/// <param name="PathFile"></param>
		/// <returns></returns>
		public static string ts_FileMark(string PathFile, string Mark = null)
		{
			if (Mark == null) Mark = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			string _ext = Path.GetExtension(PathFile);
			Regex _reg = new Regex($"{_ext}\\b",
					RegexOptions.Compiled | RegexOptions.IgnoreCase);
			return _reg.Replace(PathFile, $"_{Mark}{_ext}");
		}




		/// <summary>
		/// 設置 EncodingType(預設為中文 GetEncoding(950))
		/// </summary>
		/// <returns></returns>
		public virtual FileApp SetEncoding()
		{
			this.EncodingType = Encoding.GetEncoding(950);
			return this;
		}

		/// <summary>
		/// 寫檔規則
		/// </summary>
		/// <param name="as_PathFile"></param>
		/// <returns></returns>
		string rule_WriteFile(string as_PathFile)
		{
			switch (this._FileMode)
			{
				case FileMode.Append:
					break;
				case FileMode.Create:
					if (File.Exists(as_PathFile))
					{
						File.Delete(as_PathFile);
					}
					break;
				case FileMode.CreateNew:
					if (File.Exists(as_PathFile))
					{
						as_PathFile = FileApp.ts_FileMark(as_PathFile);
					}
					break;
			}
			return as_PathFile;
		}

		/// <summary>
		/// 主要是避免,檔案若是己存在,被覆寫的問題(T:可寫檔)
		/// </summary>
		/// <param name="as_PathFile"></param>
		/// <returns></returns>
		public bool chk_寫檔確認(string as_PathFile)
		{
			bool lb_ = false;
			switch (this._FileMode)
			{
				case FileMode.Create:
					if (File.Exists(as_PathFile))
					{
						File.Delete(as_PathFile);
					}
					break;
				case FileMode.CreateNew:
					if (File.Exists(as_PathFile))
					{
						as_PathFile = FileApp.ts_FileMark(as_PathFile);
					}
					break;
				case FileMode.Append:
					lb_ = true;
					break;
			}
			return lb_;
		}
 
        public static bool Write_SerializeXml(object ao_obj, string as_FullFileName, bool isMult = true)
		{
			return new FileApp().Write_SerializeXml(ao_obj, as_FullFileName);
		}
		/// <summary>
		/// 執行序列化寫檔
		/// </summary>
		/// <returns></returns>
		public bool Write_SerializeXml(object ao_obj, string as_FullFileName)
		{
			try
			{
				as_FullFileName = rule_WriteFile(as_FullFileName);
				//若不是 CreateNew 狀態,則直接完成寫檔
				go_TextWriter = new StreamWriter(as_FullFileName);
				XmlSerializer lo_XS = new XmlSerializer(ao_obj.GetType());
				lo_XS.Serialize(go_TextWriter, ao_obj);
				return true;
			}
			catch (Exception Ex)
			{
				//_global.go_PR.Set(Ex, "執行序列化寫檔失敗!!");
			}
			finally
			{
				this.Close();
			}
			return false;
		}

		public static bool WriteSerializeJson(object ao_obj, string as_FullFileName, JsonSerializerSettings json_options = null)
		{
			return new FileApp().Write_SerializeJson(ao_obj, as_FullFileName, json_options);
		}

		public bool Write_SerializeJson(object ao_obj, string as_FullFileName, JsonSerializerSettings json_options = null, bool isMult = true)
		{
			json_options = json_options ?? FileApp.json_options;
			try
			{
				as_FullFileName = rule_WriteFile(as_FullFileName);
				string _json = JsonConvert.SerializeObject(ao_obj, json_options);
				this.Write(_json, as_FullFileName);
				return true;
			}
			catch (Exception Ex)
			{
				//_global.go_PR.Set(Ex, "執行序列化寫檔失敗!!");
			}
			finally
			{
				this.Close();
			}
			return false;
		}

		public static object Read_SerializeXml(object ao_obj, string as_FullFileName, bool isMult = true)
		{
			return new FileApp().Read_SerializeXml(ao_obj, as_FullFileName);
		}
		/// <summary>
		/// 讀取 序列化讀檔
		/// </summary>
		/// <returns></returns>
		public object Read_SerializeXml(object ao_obj, string as_FullFileName)
		{
			try
			{
				XmlSerializer lo_XS = new XmlSerializer(ao_obj.GetType());
				System.IO.TextReader lo_TextReader = this.ts_StreamReader(as_FullFileName);
				ao_obj = lo_XS.Deserialize(lo_TextReader);
			}
			catch (Exception Ex)
			{
				//throw new _DBex(Ex, "Read_SerializeFile()執行序列讀檔失敗!!");
			}
			finally
			{
				this.Close();
			}
			return ao_obj;
		}


		public static T Read_SerializeJson<T>(string as_FullFileName, JsonSerializerSettings json_options = null, bool isMult = true)
		{
			return new FileApp().Read_SerializeJson<T>(as_FullFileName, json_options);
		}


		/// <summary>
		/// 讀取 序列化讀檔,支援 anonymousTypeObject
		/// </summary>
		/// <returns></returns>
		public T Read_SerializeJson<T>(string as_FullFileName, T anonymousTypeObject, JsonSerializerSettings json_options = null)
		{
			json_options = json_options ?? FileApp.json_options;
			T _obj = default(T);
			try
			{
				TextReader lo_TextReader = this.ts_StreamReader(as_FullFileName);
				string _s = lo_TextReader.ReadToEnd();
				_obj = JsonConvert.DeserializeAnonymousType(_s, anonymousTypeObject, json_options);
			}
			catch (Exception Ex)
			{
				throw new Exception($"Read_SerializeJson 序列讀檔({as_FullFileName})");
			}
			finally
			{
				this.Close();
			}
			return _obj;
		}


		/// <summary>
		/// 讀取 序列化讀檔
		/// </summary>
		/// <returns></returns>
		public T Read_SerializeJson<T>(string as_FullFileName, JsonSerializerSettings json_options = null)
		{
			json_options = json_options ?? FileApp.json_options;
			T _obj = default(T);
			try
			{
				TextReader lo_TextReader = this.ts_StreamReader(as_FullFileName);
				string _s = lo_TextReader.ReadToEnd();
				_obj = JsonConvert.DeserializeObject<T>(_s, json_options);
			}
			catch (Exception Ex)
			{
				throw new Exception($"Read_SerializeJson 序列讀檔({as_FullFileName})");
			}
			finally
			{
				this.Close();
			}
			return _obj;
		}

		/// <summary>
		/// 此程序 是將 檢核檔案是否存及 StreamReader 做合併處理
		/// </summary>
		/// <param name="FilePathName"></param>
		/// <returns></returns>
		public StreamReader ts_StreamReader(string as_PathFile)
		{
			this.chk_File(as_PathFile, true);
			return new StreamReader(as_PathFile, EncodingType);
		}

		/// <summary>
		/// 檢核檔案是否存在(True:存在)(依參數決定,若不存在是否丟出 Exception)
		/// </summary>
		/// <param name="FilePathName"></param>
		/// <returns></returns>
		public bool chk_File(string FilePathName, bool ab_丟出Exception)
		{
			if (File.Exists(FilePathName) == false)
			{
				//_global.go_PR.Set(new Exception("檔案不存在,\n[FilePathName]" + FilePathName));
				if (ab_丟出Exception)
				{
					throw new Exception("檔案不存在,\n[FilePathName]" + FilePathName);
					//_global.go_PR.ThrowEx();
				}
				return false;
			}
			return true;
		}

		public void Close()
		{
			try
			{
				go_TextWriter?.Close();
			}
			catch
			{
			}
		}

		public static string Read(string as_FullFileName, bool isMult = true)
		{
			return new FileApp().Read(as_FullFileName);
		}

		public string Read(string FilePathName)
		{
			string ls_Return = "";
			try
			{
				StringBuilder GetDate = new StringBuilder();
				StreamReader SR = this.ts_StreamReader(FilePathName);
				String input;
				while ((input = SR.ReadLine()) != null)
				{
					GetDate.Append(input + "\n");
				}
				SR.Close();
				ls_Return = GetDate.ToString();
			}
			catch (Exception e)
			{
				//_global.go_PR.Set
				//(e
				//, "Read()發生異常 "
				//, "[FilePathName]", FilePathName
				//, "[SetFileCode]", EncodingType.ToString()
				//).ThrowEx();
			}
			return ls_Return;
		}

		/// <summary>
		/// 將指定檔案 讀成 二進位陣列
		/// </summary>
		/// <param name="FilePathName"></param>
		/// <returns></returns>
		public byte[] Binary_Read(string FilePathName)
		{
			byte[] lb_Data = new byte[0];
			try
			{
				StreamReader SR = this.ts_StreamReader(FilePathName);
				long ll_ = SR.BaseStream.Length;
				lb_Data = new byte[ll_];
				for (long i = 0; i < ll_; i++)
				{
					lb_Data[i] = (byte)SR.BaseStream.ReadByte();
				}
				SR.Close();
			}
			catch (Exception Ex)
			{
				//throw new _DBex(Ex, "Binary_Read() 處理發生異常!!");
			}



			return lb_Data;
		}

		/// <summary>
		/// 文字檔寫檔程序
		/// </summary>
		/// <param name="strMsg"></param>
		/// <param name="FilePathName"></param>
		/// <returns></returns>
		public bool Write(string[] strMsg, string as_PathFile)
		{
			try
			{
				as_PathFile = rule_WriteFile(as_PathFile);
				StreamWriter SW = new StreamWriter
					(as_PathFile
					, this.IsAppend
					, this.EncodingType);
				for (int i = 0; i < strMsg.Length; i++)
				{
					SW.WriteLine(strMsg[i]);
				}
				SW.Flush();
				SW.Close();
				return true;
			}
			catch (Exception e)
			{
				//_global.go_PR.Set(e, "Write() 文字檔寫檔程序:發生異常"
				//, "[strMsg]", string.Join("\n", strMsg)
				//, "[FilePathName]", as_PathFile
				//, "[ae_Mode]", this._FileMode.ToString()
				//, "[SetFileCode]", EncodingType.ToString()
				//);
			}
			return false;
		}


		/// <summary>
		/// 文字檔寫檔程序(預設為串接)
		/// </summary>
		/// <param name="strMsg"></param>
		/// <param name="FilePathName"></param>
		/// <returns></returns>
		public bool Write(string strMsg, string FilePathName, bool isSplitLine = true)
		{
			if (isSplitLine)
			{
				string[] tmpArr = strMsg.Split(new char[] { '\n' });
				return this.Write(tmpArr, FilePathName);
			}
			return this.Write(new string[] { strMsg }, FilePathName);
		}

        public static string _tmpTxt(bool isRead=false)
        {
			var _file = ts_Log("_tmp.txt");
			if (isRead == false) return _file;
			return Read(_file);
		}

		public static string _tmpJson(object data = null)
		{
			return _tmpJson<string>(data);
		}
		public static T _tmpJson<T>(object data =null)
		{
			var _file = ts_Log("_tmp.json");
			var isWrite = data != null;
			if (isWrite){
				new FileApp() { _FileMode = FileMode.Create }.Write_SerializeJson(data, _file);
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				return (T)converter.ConvertFrom(_file);
			}
			return  Read_SerializeJson<T>(_file);
		}
		public static T _tmpTxtToJson<T>(string AutoWriteTo = null)
		{
			var _data = _tmpTxt(true);
			T _obj = JsonConvert.DeserializeObject<T>(_data, FileApp.json_options);
			if (AutoWriteTo!=null && _obj != null) {
				FileApp.WriteSerializeJson(_obj, AutoWriteTo);
			}
			return _obj;

		}
	}


	public class _testBase
	{
		internal DBController _dbc;

		internal DBController DBC
		{
			get
			{
				if (_dbc == null)
				{

					var Conn = ConfigurationManager.ConnectionStrings["sql.mes"];
					//var Conn = ConfigurationManager.ConnectionStrings["SRDSR.SqlServer.GTIMES"];
					//var a = WebConfigurationManager.AppSettings["csMES"];
					//var g = WebConfigurationManager.ConnectionStrings;
					//var Conn = WebConfigurationManager.ConnectionStrings[a];
					_dbc = new DBController(Conn);
				}
				return this._dbc;
			}
		}

		internal void _DBTest(Action<ITxnBase> fn, bool isTransMode = false)
		{
			if (isTransMode)
			{
				TxnBase.LzDBTrans_t(txn =>
				{
					fn(txn);
					return txn.result;
				});
			}
			else {
				TxnBase.LzDBQuery(txn =>
				{
					fn(txn);
					return txn.result;
				});
			}
		}
		
 

		internal FileApp _file = new FileApp();
	}
}
