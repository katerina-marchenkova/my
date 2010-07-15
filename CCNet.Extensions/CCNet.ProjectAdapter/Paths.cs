using System.IO;
using CCNet.Common;

namespace CCNet.ProjectAdapter
{
	/// <summary>
	/// Generates environment paths based on arguments.
	/// </summary>
	public static class Paths
	{
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
		/// Gets assembly information file path.
		/// </summary>
		public static string AssemblyInfoFile
		{
			get
			{
				string propertiesPath = Path.Combine(Arguments.ProjectPath, "Properties");
				return Path.Combine(propertiesPath, "AssemblyInfo.cs");
			}
		}
	}
}
