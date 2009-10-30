using System;
using System.Data;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Provides reading for reader for standard types.
	/// </summary>
	public static class Read
	{
		/// <summary>
		/// Reads non-nullable Guid.
		/// </summary>
		public static Guid Guid(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (Guid)value;
		}

		/// <summary>
		/// Reads nullable Guid.
		/// </summary>
		public static Guid? GuidOrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (Guid?)value;
		}

		/// <summary>
		/// Reads non-nullable String.
		/// </summary>
		public static string String(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (string)value;
		}

		/// <summary>
		/// Reads nullable String.
		/// </summary>
		public static string StringOrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (string)value;
		}

		/// <summary>
		/// Reads non-nullable Boolean.
		/// </summary>
		public static bool Boolean(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (bool)value;
		}

		/// <summary>
		/// Reads nullable Boolean.
		/// </summary>
		public static bool? BooleanOrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (bool?)value;
		}

		/// <summary>
		/// Reads non-nullable DateTime.
		/// </summary>
		public static DateTime DateTime(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (DateTime)value;
		}

		/// <summary>
		/// Reads nullable DateTime.
		/// </summary>
		public static DateTime? DateTimeOrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (DateTime?)value;
		}

		/// <summary>
		/// Reads non-nullable Byte.
		/// </summary>
		public static byte Byte(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (byte)value;
		}

		/// <summary>
		/// Reads nullable Byte.
		/// </summary>
		public static byte? ByteOrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (byte?)value;
		}

		/// <summary>
		/// Reads non-nullable Int16.
		/// </summary>
		public static short Int16(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (short)value;
		}

		/// <summary>
		/// Reads nullable Int16.
		/// </summary>
		public static short? Int16OrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (short?)value;
		}

		/// <summary>
		/// Reads non-nullable Int32.
		/// </summary>
		public static int Int32(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (int)value;
		}

		/// <summary>
		/// Reads nullable Int32.
		/// </summary>
		public static int? Int32OrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (int?)value;
		}

		/// <summary>
		/// Reads non-nullable Int64.
		/// </summary>
		public static long Int64(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return (long)value;
		}

		/// <summary>
		/// Reads nullable Int64.
		/// </summary>
		public static long? Int64OrNull(IDataRecord reader, string columnName)
		{
			object value = reader[columnName];
			return value == DBNull.Value ? null : (long?)value;
		}
	}
}
