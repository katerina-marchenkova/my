using System;
using System.Collections.Generic;
using System.IO;

namespace SourceTracker
{
	/// <summary>
	/// Tracks source code.
	/// </summary>
	public class Program
	{
		private static ITrackerOptions s_options;
		private static ISourceEngine s_engine;

		/// <summary>
		/// Main program entry.
		/// </summary>
		public static void Main(string[] args)
		{
			s_options = new TrackerOptions(new DateTime(2011, 3, 28));
			s_engine = new TeamFoundationEngine("http://rufrt-vxbuild:8080/tfs/sandbox");

			// TODO: fake uploading for garbage parts
			// FakeUploader.UploadFakeFile(1);
			// return;

			DateTime start = DateTime.Now;
			Console.WriteLine("Started at {0:G}.", start);

			List<ISourceFile> files = s_engine.GetFiles(s_options);
			foreach (ISourceFile file in files)
			{
				Console.WriteLine("[{0:HH:mm:ss}] {1}", DateTime.Now, file.FullPath);
				ProcessFile(file);
			}

			DateTime end = DateTime.Now;
			Console.WriteLine("Finished at {0:G}.", end);

			TimeSpan delta = end.Subtract(start);
			int seconds = Convert.ToInt32(delta.TotalSeconds);
			delta = TimeSpan.FromSeconds(seconds);
			Console.WriteLine("Done in {0}.", delta);
			Console.ReadKey();
		}

		/// <summary>
		/// Processes single file.
		/// </summary>
		private static void ProcessFile(ISourceFile file)
		{
			int fileId = ResolveFile(file);

			Dictionary<Guid, LineRow> lineRows = new Dictionary<Guid, LineRow>();
			Dictionary<Guid, DateTime> lineDates = new Dictionary<Guid, DateTime>();

			List<ISourceVersion> versions = s_engine.GetVersions(file, s_options);
			foreach (ISourceVersion version in versions)
			{
				int userId = ResolveUser(version);
				int versionId = ResolveVersion(fileId, userId, version);

				List<string> lines = s_engine.GetLines(version);
				for (int i = 0; i < lines.Count; i++)
				{
					string text = lines[i];

					Guid crc = SourceProcessor.CalculateCrc(text);
					LineRow lineRow = new LineRow
					{
						VersionId = versionId,
						LineCrc = crc,
						LineNumber = i,
						LineText = text
					};

					if (!lineRows.ContainsKey(crc))
					{
						lineRows.Add(crc, lineRow);
						lineDates.Add(crc, version.VersionDate);
					}
					else
					{
						if (version.VersionDate < lineDates[crc])
						{
							lineRows[crc] = lineRow;
							lineDates[crc] = version.VersionDate;
						}
					}
				}
			}

			SourceTrackerDb.UploadLines(lineRows.Values);
		}

		/// <summary>
		/// Returns existing file or creates a new one.
		/// </summary>
		private static int ResolveFile(ISourceFile file)
		{
			string extension = Path.GetExtension(file.FullPath).ToLowerInvariant().TrimStart('.');
			return SourceTrackerDb.ResolveFile(file.FullPath, extension);
		}

		/// <summary>
		/// Returns existing user or creates a new one.
		/// </summary>
		private static int ResolveUser(ISourceVersion version)
		{
			string userName = version.UserName.ToUpperInvariant();

			if (userName.StartsWith(@"CNEU\"))
				userName = userName.Substring(5);

			if (userName.StartsWith(@"CNET\"))
				userName = userName.Substring(5);

			return SourceTrackerDb.ResolveUser(userName);
		}

		/// <summary>
		/// Returns existing version or creates a new one.
		/// </summary>
		private static int ResolveVersion(int fileId, int userId, ISourceVersion version)
		{
			return SourceTrackerDb.ResolveVersion(
				fileId,
				version.VersionKey,
				userId,
				version.VersionDate);
		}
	}
}
