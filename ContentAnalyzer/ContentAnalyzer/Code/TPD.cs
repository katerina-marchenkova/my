using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VX.Sys;

namespace ContentAnalyzer
{
	/// <summary>
	/// Accessing TPD.
	/// </summary>
	public static class TPD
	{
		private static readonly DbHelper s_db;

		static TPD()
		{
			s_db = new DbHelper(
				ConfigurationHelper.ReadConnectionString("TPD"),
				ConfigurationHelper.ReadTimeSpan("TPD.Timeout"));
		}

		/// <summary>
		/// Gets multiple images with many files.
		/// </summary>
		public static List<DigitalContentRow> GetMultipleImagesByFileCount()
		{
			List<DigitalContentRow> rows = new List<DigitalContentRow>();

			/*using (SqlConnection conn = new SqlConnection(s_connectionString))
			{
				conn.Open();

				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandTimeout = s_commandTimeoutInSeconds;
					cmd.CommandText = String.Format(
						@"
							SELECT
								DCL.sku_id,
								DCL.content_guid,
								DC.original_file_name,
								DC.original_file_extension
							FROM tpd_digital_content DC WITH(NOLOCK)
								INNER JOIN tpd_digital_content_meta_link DCML WITH(NOLOCK)
								ON DCML.content_guid = DC.content_guid
									AND DCML.meta_value_id = 899018
								INNER JOIN tpd_digital_content_link DCL WITH(NOLOCK)
								ON DCL.content_guid = DC.content_guid
							WHERE
								media_type_id = 15
								AND sku_id IN
								(
									6238340,
									5408055,
									5221661,
									5394367,
									5001945,
									4827247,
									4778156,
									4976441,
									5215413,
									4687179,
									5380883,
									4687079,
									4824139,
									5231532,
									4946927,
									5070774,
									4976455,
									4824121,
									4778115,
									5041535
								)
						");

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							DigitalContentRow row = new DigitalContentRow(reader);
							rows.Add(row);
						}
					}
				}
			}*/

			return rows;
		}

		/// <summary>
		/// Gets license multiple images.
		/// </summary>
		public static List<DigitalContentRow> GetLicenseMultipleImages()
		{
			List<DigitalContentRow> rows = new List<DigitalContentRow>();

			/*using (SqlConnection conn = new SqlConnection(s_connectionString))
			{
				conn.Open();

				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandTimeout = s_commandTimeoutInSeconds;
					cmd.CommandText = String.Format(
						@"
							SELECT
								A.content_guid,
								A.original_file_name,
								A.original_file_extension,
								(SELECT TOP 1 sku_id FROM tpd_digital_content_link WHERE content_guid = A.content_guid) AS sku_id
							FROM tpd_digital_content A
								INNER JOIN tpd_digital_content_meta_link B
								ON B.content_guid = A.content_guid
								INNER JOIN tpd_digital_content_meta_link C
								ON C.content_guid = A.content_guid
							WHERE
								B.meta_value_id = 1314536
								AND C.meta_value_id = 2683
						");

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							DigitalContentRow row = new DigitalContentRow(reader);
							rows.Add(row);
						}
					}
				}
			}*/

			return rows;
		}

		/// <summary>
		/// Gets multiple images.
		/// </summary>
		public static List<DigitalContentRow> GetMultipleImages(int skuId)
		{
			List<DigitalContentRow> rows = new List<DigitalContentRow>();

			/*using (SqlConnection conn = new SqlConnection(s_connectionString))
			{
				conn.Open();

				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandTimeout = s_commandTimeoutInSeconds;
					cmd.CommandText = String.Format(
						@"
							SELECT
								DCL.sku_id,
								DCL.content_guid,
								DC.original_file_name,
								DC.original_file_extension
							FROM tpd_digital_content DC WITH(NOLOCK)
								INNER JOIN tpd_digital_content_meta_link DCML WITH(NOLOCK)
								ON DCML.content_guid = DC.content_guid
									AND DCML.meta_value_id = 2683
								INNER JOIN tpd_digital_content_link DCL WITH(NOLOCK)
								ON DCL.content_guid = DC.content_guid
							WHERE
								media_type_id = 15
								AND sku_id = @skuId
						");

					cmd.Parameters.AddWithValue("@skuId", skuId);

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							DigitalContentRow row = new DigitalContentRow(reader);
							rows.Add(row);
						}
					}
				}
			}*/

			return rows;
		}

		/// <summary>
		/// Gets all multiple images by category.
		/// </summary>
		public static List<MultipleImageRow> GetAllMultipleImages(string categoryCode)
		{
			List<MultipleImageRow> rows = new List<MultipleImageRow>();

			using (SqlConnection conn = s_db.OpenConnection())
			{
				s_db.ExecuteReader(
					conn,
					@"
						CREATE TABLE #Temp (
							ContentUid UNIQUEIDENTIFIER,
							ItemUid UNIQUEIDENTIFIER,
							FileUid UNIQUEIDENTIFIER,
							MetaValueId INT)

						INSERT INTO #Temp
							SELECT DISTINCT TOP 500
								TDC.content_guid,
								CAST(LEFT(TDC.original_id, 36) AS UNIQUEIDENTIFIER),
								CAST(SUBSTRING(TDC.original_id, 38, 36) AS UNIQUEIDENTIFIER),
								TDCML.meta_value_id
							FROM [tpd_digital_content] TDC WITH(NOLOCK)
								INNER JOIN [tpd_digital_content_link] TDCL WITH(NOLOCK)
								ON TDCL.content_guid = TDC.content_guid
								INNER JOIN [tpd_digital_content_meta_link] TDCML WITH(NOLOCK)
								ON TDCML.content_guid = TDC.content_guid
								INNER JOIN [tpd_sku] TS WITH(NOLOCK)
								ON TS.sku_id = TDCL.sku_id
							WHERE
								TDC.media_type_id = 15
								AND TDCML.meta_value_id IN (2683, 2689) -- 200 x 150 or 400 x 300
								AND TDCL.priority BETWEEN 500000 AND 509999
								AND TS.category_code = @categoryCode

						SELECT DISTINCT
							ItemUid,
							FileUid,
							(
								SELECT TOP 1 ContentUid
								FROM #Temp A
								WHERE
									A.ItemUid = T.ItemUid
									AND A.FileUid = T.FileUid
								ORDER BY MetaValueId DESC
							) AS ContentUid
						FROM #Temp T
						ORDER BY ItemUid

						DROP TABLE #Temp
					",
					delegate(IDataRecord reader)
					{
						MultipleImageRow row = new MultipleImageRow(reader);
						rows.Add(row);
					},
					new SqlParameter("@categoryCode", categoryCode));
			}

			return rows;
		}
	}
}
