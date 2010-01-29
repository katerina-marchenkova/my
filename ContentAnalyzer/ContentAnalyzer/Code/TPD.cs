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
									/*6238340,
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
									5041535*/
3314140,
5627780,
4517555,
6846303,
7181021,
5160831,
4773421,
3753243,
6446324,
6386493,
5473073,
6334800,
6380124,
5814472,
7213610,
5740303,
4123089,
5661139,
5015032,
7483983,
3385853,
6439789,
5995087,
5665281,
6723750,
6408116,
5806938,
3527656,
3750100,
6726976,
5805481,
1541532,
5009954,
5663512,
3523514,
6626398,
5209215,
1214016,
2295044,
2317643,
1942679,
2447564,
2420906,
1936061,
2124293,
1360668,
5721053,
6224212,
2917135,
6275052,
2218336,
4785137,
2099072,
3623096,
1744334,
5090286,
2634279,
1432092,
6925782,
2803636,
5021089,
6085303,
6606172,
1732223,
5238624,
6684254,
4520781,
7176736,
6256575,
5263158,
5948847,
5497175,
2489708,
6750368,
3771122,
5880818,
5965641,
5694292,
5441257,
4403827,
6396005,
5425545,
6174497,
4280793,
1825954
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
