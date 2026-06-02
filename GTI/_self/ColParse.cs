namespace Genesis.Library.BLL.Helper
{
    using Genesis.Library.Frame.Code.Web.TableQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Web;

	public interface IRowAccessor
	{
		object GetValue(object row, string columnName);
	}

	public class DynamicRowAccessor : IRowAccessor
	{
		public object GetValue(object row, string columnName)
		{
			if (row == null) return null;

			// ExpandoObject / Dapper
			if (row is IDictionary<string, object> dict)
			{
				if (dict.TryGetValue(columnName, out var val))
					return Normalize(val);

				// 忽略大小寫
				foreach (var kv in dict)
				{
					if (string.Equals(kv.Key, columnName, StringComparison.OrdinalIgnoreCase))
						return Normalize(kv.Value);
				}

				return null;
			}

			// fallback：dynamic 但不是 IDictionary
			return GetByReflection(row, columnName);
		}

		protected object GetByReflection(object row, string columnName)
		{
			var prop = row.GetType().GetProperty(
				columnName,
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

			return prop != null ? Normalize(prop.GetValue(row)) : null;
		}

		protected object Normalize(object value)
			=> value == DBNull.Value ? null : value;
	}


	public class StrongTypeRowAccessor : IRowAccessor
	{
		public object GetValue(object row, string columnName)
		{
			if (row == null) return null;

			var prop = row.GetType().GetProperty(
				columnName,
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

			return prop != null ? Normalize(prop.GetValue(row)) : null;
		}

		protected object Normalize(object value)
			=> value == DBNull.Value ? null : value;
	}

	public static class RowAccessorFactory
	{
		public static IRowAccessor Create<T>()
		{
			var t = typeof(T);

			// 明確 dynamic / object
			if (t == typeof(object))
				return new DynamicRowAccessor();

			return new StrongTypeRowAccessor();
		}
	}

	public class ColParse<T>
	{
		public string Header { get; set; }
		public string ColumnName { get; }
		public Dictionary<string, string> ValueMap { get; set; }
		public Func<T, object> DynFunc { get; set; }

		private readonly IRowAccessor _accessor;

		#region Constructor

		public ColParse(string columnName, string header = null)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException(nameof(columnName));

			ColumnName = columnName;
			Header = header ?? columnName;

			// ⭐ 關鍵：策略在建構時就決定
			_accessor = RowAccessorFactory.Create<T>();
		}

		#endregion

		#region 核心 API

		public object GetValue(T row)
		{
			if (row == null) return null;

			// 最高優先權
			if (DynFunc != null)
				return DynFunc(row);

			// ⭐ 委派給 Strategy
			object value = _accessor.GetValue(row, ColumnName);

			// ValueMap
			if (value != null && ValueMap != null &&
				ValueMap.TryGetValue(value.ToString(), out var mapped))
			{
				value = mapped;
			}

			return value;
		}

		#endregion

		#region Factory / DSL（原封不動）

		public static ColParse<T> n(object sel)
		{
			if (sel is string col)
				return new ColParse<T>(col);

			if (sel is ColParse<T> cp)
				return cp;

			throw new InvalidOperationException("Unsupported selector type");
		}

		public static ColParse<T> n(
			string columnName,
			string header,
			Func<T, object> dynFunc,
			params (string key, string value)[] mappings)
		{
			var r = new ColParse<T>(columnName, header)
			{
				DynFunc = dynFunc
			};

			if (mappings != null && mappings.Length > 0)
				r.ValueMap = mappings.ToDictionary(t => t.key, t => t.value);

			return r;
		}

		public static ColParse<T> n(
			string columnName,
			params (string key, string value)[] mappings)
			=> n(columnName, string.Empty, null, mappings);

		public static ColParse<T> n(
			string columnName,
			string header,
			params (string key, string value)[] mappings)
			=> n(columnName, header, null, mappings);

		public static ColParse<T> n(Func<T, object> DynFunc)
			=> n("-", string.Empty, DynFunc);

		public static ColParse<T> n(
			string columnName,
			string header,
			Dictionary<string, string> valueMap)
		{
			var r = new ColParse<T>(columnName, header)
			{
				ValueMap = valueMap
			};
			return r;
		}

		public static ColParse<T> n(
			string columnName,
			Dictionary<string, string> valueMap)
			=> n(columnName, columnName, valueMap);

		#endregion
	}
}
