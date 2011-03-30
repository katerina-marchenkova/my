using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VX.Sys;
using VX.Sys.DbHelper;

namespace SourceTracker
{
	/// <summary>
	/// Accessing Source Tracker DB.
	/// </summary>
	public static class SourceTrackerDb
	{
		private static readonly DbHelper s_db;

		static SourceTrackerDb()
		{
			s_db = new DbHelper(
				ConfigurationHelper.ReadConnectionString("SourceTracker"),
				ConfigurationHelper.ReadTimeSpan("SourceTracker.Timeout"));
		}

		/// <summary>
		/// Returns existing file or creates a new one.
		/// </summary>
		public static int ResolveFile(string fullPath, string extension)
		{
			using (SqlConnection conn = s_db.OpenConnection())
			{
				SqlParameter fileId = new SqlParameter("@fileId", SqlDbType.Int) { Direction = ParameterDirection.Output };

				s_db.ExecuteNonQuery(
					conn,
					"ResolveFile",
					new SqlParameter("@fullPath", fullPath),
					new SqlParameter("@extension", extension),
					fileId);

				return (int)fileId.Value;
			}
		}

		/// <summary>
		/// Returns existing user or creates a new one.
		/// </summary>
		public static int ResolveUser(string userName)
		{
			using (SqlConnection conn = s_db.OpenConnection())
			{
				SqlParameter userId = new SqlParameter("@userId", SqlDbType.Int) { Direction = ParameterDirection.Output };

				s_db.ExecuteNonQuery(
					conn,
					"ResolveUser",
					new SqlParameter("@userName", userName),
					userId);

				return (int)userId.Value;
			}
		}

		/// <summary>
		/// Returns existing version or creates a new one.
		/// </summary>
		public static int ResolveVersion(
			int fileId,
			string versionKey,
			int userId,
			DateTime versionDate)
		{
			using (SqlConnection conn = s_db.OpenConnection())
			{
				SqlParameter versionId = new SqlParameter("@versionId", SqlDbType.Int) { Direction = ParameterDirection.Output };

				s_db.ExecuteNonQuery(
					conn,
					"ResolveVersion",
					new SqlParameter("@fileId", fileId),
					new SqlParameter("@versionKey", versionKey),
					new SqlParameter("@userId", userId),
					new SqlParameter("@versionDate", versionDate),
					versionId);

				return (int)versionId.Value;
			}
		}

		/// <summary>
		/// Uploads new lines.
		/// </summary>
		public static void UploadLines(IEnumerable<LineRow> lines)
		{
			using (SqlConnection conn = s_db.OpenConnection())
			{
				s_db.ExecuteNonQuery(
					conn,
					@"
						CREATE TABLE #UploadedLine (
							LineCrc UNIQUEIDENTIFIER PRIMARY KEY,
							VersionId INT NOT NULL,
							LineNumber INT NOT NULL,
							LineText NVARCHAR(MAX) NOT NULL)
					");

				s_db.ExecuteBulkCopy(
					conn,
					"#UploadedLine",
					s_db.CreateBulkTable(lines));

				s_db.ExecuteNonQuery(conn, "UploadLines");

				s_db.ExecuteNonQuery(
					conn,
					@"
						DROP TABLE #UploadedLine
					");
			}
		}
	}
}
