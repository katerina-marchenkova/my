using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ContentAnalyzer
{
	/// <summary>
	/// Accessing Work DB.
	/// </summary>
	public static class WorkDb
	{
		private static readonly DbHelper s_db;

		static WorkDb()
		{
			s_db = new DbHelper(
				@"Data Source=RUFRW-OSHU\SQL2005; Initial Catalog=Work; Integrated Security=True;",
				TimeSpan.Parse("00:10:00"));
		}

		/// <summary>
		/// Gets multuple images with bad canvas.
		/// </summary>
		public static List<MultipleImageRow> GetMultipleImagesWithBadCanvas()
		{
			List<MultipleImageRow> rows = new List<MultipleImageRow>();

			using (SqlConnection conn = s_db.OpenConnection())
			{
				s_db.ExecuteReader(
					conn,
					@"
						CREATE TABLE #MaxSize(
							Url VARCHAR(200),
							OriginalId VARCHAR(100),
							[Size] TINYINT)

						INSERT INTO #MaxSize
							SELECT
								Url,
								OriginalId,
								CASE 
									WHEN RealWidthPercent > RealHeightPercent THEN RealWidthPercent
									ELSE RealHeightPercent
								END
							FROM [Canvas]

						SELECT TOP 1000
							Url,
							OriginalId
						FROM #MaxSize
						ORDER BY [Size]

						DROP TABLE #MaxSize
					",
					delegate(IDataRecord reader)
					{
						MultipleImageRow row = new MultipleImageRow(reader);
						rows.Add(row);
					});
			}

			return rows;
		}
	}
}
