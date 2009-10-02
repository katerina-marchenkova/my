using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VX.Knowledge.DataSource;
using VX.Sys;

namespace ContentAnalyzer
{
	/// <summary>
	/// Accessing Aggregator DB.
	/// </summary>
	public static class AggregatorDb
	{
		private static readonly string s_connectionString = String.Empty;
		private static readonly TimeSpan s_commandTimeout = TimeSpan.FromSeconds(30);
		private static readonly int s_commandTimeoutInSeconds = 30;

		static AggregatorDb()
		{
			s_connectionString = ConfigurationHelper.ReadConnectionString("Aggregator");
			s_commandTimeout = ConfigurationHelper.ReadTimeSpan("Aggregator.Timeout");
			s_commandTimeoutInSeconds = Convert.ToInt32(s_commandTimeout.TotalSeconds);
		}

		/// <summary>
		/// Gets multiple images with many files.
		/// </summary>
		public static List<FileRow> GetMultipleImagesByFileCount(int fileCount)
		{
			List<FileRow> rows = new List<FileRow>();

			using (SqlConnection conn = new SqlConnection(s_connectionString))
			{
				conn.Open();

				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandTimeout = s_commandTimeoutInSeconds;
					cmd.CommandText = String.Format(
						@"
							CREATE TABLE #Item (
								ItemUid UNIQUEIDENTIFIER PRIMARY KEY)

							INSERT INTO #Item
								SELECT ItemUid
								FROM Catalog.[File] WITH(NOLOCK)
								WHERE TemplateUid = '{0}'
								GROUP BY ItemUid
								HAVING COUNT(*) >= {1}
								ORDER BY COUNT(*) DESC

							SELECT
								F.TemplateUid,
								F.ItemUid,
								F.FileUid,
								F.Name,
								F.Extension
							FROM #Item I
								INNER JOIN Catalog.[File] F WITH(NOLOCK)
								ON F.TemplateUid = '{0}'
									AND F.ItemUid = I.ItemUid
							ORDER BY F.ItemUid

							DROP TABLE #Item
						",
						KB.Template.Multipleimages.Id,
						fileCount);

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							FileRow row = new FileRow(reader);
							rows.Add(row);
						}
					}
				}
			}

			return rows;
		}
	}
}
