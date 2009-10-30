using System;
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
		private static readonly string s_connectionString = String.Empty;
		private static readonly TimeSpan s_commandTimeout = TimeSpan.FromSeconds(30);
		private static readonly int s_commandTimeoutInSeconds = 30;

		static TPD()
		{
			s_connectionString = ConfigurationHelper.ReadConnectionString("TPD");
			s_commandTimeout = ConfigurationHelper.ReadTimeSpan("TPD.Timeout");
			s_commandTimeoutInSeconds = Convert.ToInt32(s_commandTimeout.TotalSeconds);
		}

		/// <summary>
		/// Gets multiple images with many files.
		/// </summary>
		public static List<DigitalContentRow> GetMultipleImagesByFileCount()
		{
			List<DigitalContentRow> rows = new List<DigitalContentRow>();

			using (SqlConnection conn = new SqlConnection(s_connectionString))
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
			}

			return rows;
		}
	}
}
