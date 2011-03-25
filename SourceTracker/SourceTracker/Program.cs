using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

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
			s_options = new TrackerOptions(new DateTime(1983, 5, 25));
			s_engine = new TeamFoundationEngine("http://rufrt-vxbuild:8080/tfs/sandbox");

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

			Dictionary<Guid, DateTime> dates = new Dictionary<Guid, DateTime>();
			Dictionary<Guid, string> originals = new Dictionary<Guid, string>();

			List<ISourceVersion> versions = s_engine.GetVersions(file, s_options);
			foreach (ISourceVersion version in versions)
			{
				int userId = ResolveUser(version);

				List<string> lines = s_engine.GetLines(version);
				foreach (string line in lines)
				{
				}
			}
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

			return SourceTrackerDb.ResolveUser(userName);
		}
	}
}
