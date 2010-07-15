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
	}
}
