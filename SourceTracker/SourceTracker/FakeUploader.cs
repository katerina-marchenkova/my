using System;
using System.Collections.Generic;
using System.Linq;
using SourceTracker.Properties;

namespace SourceTracker
{
	public static class FakeUploader
	{
		public static void UploadFakeFile(int userId)
		{
			string uid = Guid.NewGuid().ToString().ToUpperInvariant();
			int fileId = SourceTrackerDb.ResolveFile(uid, String.Empty);
			int versionId = SourceTrackerDb.ResolveVersion(fileId, uid, userId, new DateTime(1900, 1, 1));

			Dictionary<Guid, LineRow> lineRows = new Dictionary<Guid, LineRow>();

			string[] lines = Resources.FakeFile.Split('\r', '\n');
			for (int i = 0; i < lines.Count(); i++)
			{
				string text = lines[i];
				Guid crc = SourceProcessor.CalculateCrc(text);

				if (lineRows.ContainsKey(crc))
					continue;

				lineRows.Add(
					crc,
					new LineRow
					{
						VersionId = versionId,
						LineCrc = crc,
						LineNumber = i,
						LineText = text
					});
			}

			SourceTrackerDb.UploadLines(lineRows.Values);
		}
	}
}
