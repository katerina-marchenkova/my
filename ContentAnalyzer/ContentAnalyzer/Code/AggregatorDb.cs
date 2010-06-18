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
		private static readonly DbHelper s_db;

		static AggregatorDb()
		{
			s_db = new DbHelper(
				ConfigurationHelper.ReadConnectionString("Aggregator"),
				ConfigurationHelper.ReadTimeSpan("Aggregator.Timeout"));
		}

		/// <summary>
		/// Gets multiple images with many files.
		/// </summary>
		public static List<HelixFileRow> GetMultipleImagesByFileCount(int fileCount)
		{
			List<HelixFileRow> rows = new List<HelixFileRow>();

			/*using (SqlConnection conn = new SqlConnection(s_connectionString))
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
							HelixFileRow row = new HelixFileRow(reader);
							rows.Add(row);
						}
					}
				}
			}*/

			return rows;
		}

		/// <summary>
		/// Gets information for specified products.
		/// </summary>
		public static Dictionary<int, SkuInfoRow> GetSkuInfo(IEnumerable<int> skuIds)
		{
			Dictionary<int, SkuInfoRow> map = new Dictionary<int, SkuInfoRow>();

			using (SqlConnection conn = s_db.OpenConnection())
			{
				s_db.ExecuteNonQuery(
					conn,
					@"
						CREATE TABLE #Sku (
							SkuId INT PRIMARY KEY)
					");

				s_db.ExecuteBulkCopy(
					conn,
					"#Sku",
					s_db.CreateBulkTable(new HashSet<int>(skuIds), "SkuId"));

				s_db.ExecuteReader(
					conn,
					@"
						SELECT
							CN.SkuId,
							CM.ManufacturerName,
							CN.ParentTree
						FROM [Catalog].[Node] CN WITH(NOLOCK)
							INNER JOIN [Catalog].[Manufacturer] CM WITH(NOLOCK)
							ON CM.ManufacturerUid = CN.ManufacturerUid
							INNER JOIN #Sku S WITH(NOLOCK)
							ON S.SkuId = CN.SkuId
					",
					delegate(IDataRecord reader)
					{
						int skuId = (int)reader["SkuId"];
						SkuInfoRow row = new SkuInfoRow(reader);
						map[skuId] = row;
					});

				s_db.ExecuteNonQuery(
					conn,
					@"
						DROP TABLE #Sku
					");
			}

			return map;
		}
	}
}
