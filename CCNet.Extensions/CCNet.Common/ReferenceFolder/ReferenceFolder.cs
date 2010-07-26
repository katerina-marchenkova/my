using System.Collections.Generic;
using System.IO;

namespace CCNet.Common
{
	/// <summary>
	/// Common methods for working with reference folders.
	/// </summary>
	public static class ReferenceFolder
	{
		/// <summary>
		/// Gets path for latest reference version.
		/// </summary>
		private static string GetLatestPath(string referenceFolder, string referenceProject)
		{
			string subPath = Path.Combine(referenceFolder, referenceProject);
			return Path.Combine(subPath, "Latest");
		}

		/// <summary>
		/// Gets names for all binary files in reference folder.
		/// </summary>
		public static List<ReferenceFile> GetAllFiles(string referenceFolder)
		{
			List<ReferenceFile> files = new List<ReferenceFile>();

			foreach (string dir in Directory.GetDirectories(referenceFolder))
			{
				string latestPath = GetLatestPath(referenceFolder, dir);
				foreach (string file in Directory.GetFiles(latestPath, "*.dll"))
				{
					files.Add(new ReferenceFile
					{
						ProjectName = Path.GetFileName(dir),
						FilePath = file,
						AssemblyName = Path.GetFileNameWithoutExtension(file)
					});
				}
			}

			return files;
		}
	}
}
