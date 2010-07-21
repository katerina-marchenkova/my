using System.IO;
using CCNet.Common;

namespace CCNet.ProjectChecker
{
	/// <summary>
	/// Generates environment paths based on arguments.
	/// </summary>
	public static class Paths
	{
		/// <summary>
		/// Gets properties folder path.
		/// </summary>
		private static string PropertiesFolder
		{
			get { return Path.Combine(Arguments.ProjectPath, "Properties"); }
		}

		/// <summary>
		/// Gets project file path.
		/// </summary>
		public static string ProjectFile
		{
			get
			{
				string projectFile = "{0}.csproj".Display(Arguments.ProjectName);
				return Path.Combine(Arguments.ProjectPath, projectFile);
			}
		}

		/// <summary>
		/// Gets manifest file path.
		/// </summary>
		public static string ManifestFile
		{
			get
			{
				string manifestFile = "App.manifest";
				return Path.Combine(PropertiesFolder, manifestFile);
			}
		}
	}
}
