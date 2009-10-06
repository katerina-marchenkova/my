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
								ItemUid UNIQUEIDENTIFIER PRIMARY KEY,
								Total INT)

							INSERT INTO #Item
								SELECT
									CC.ItemUid,
									COUNT(*) AS Total
								FROM Catalog.[File] CF WITH(NOLOCK)
									INNER JOIN Catalog.Content CC WITH(NOLOCK)
									ON CC.TemplateUid = CF.TemplateUid
										AND CC.ItemUid = CF.ItemUid
								WHERE
									CC.TemplateUid = '{0}'
									AND CC.OwnerUid <> 'AE16042A-32F6-47B2-AB09-3BEC75D1815C'
								GROUP BY CC.ItemUid
								HAVING COUNT(*) >= {1}

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
							ORDER BY I.Total DESC, F.ItemUid

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
