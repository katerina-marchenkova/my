using System;
using System.IO;
using EnvDTE;
using VSLangProj;

namespace SolutionHelper
{
	/// <summary>
	/// Coontains extension methods for project.
	/// </summary>
	public static class ProjectExtensions
	{
		/// <summary>
		/// Gets assembly name.
		/// </summary>
		public static string GetAssemblyName(this Project project)
		{
			return (string)project.Properties.Item("AssemblyName").Value;
		}

		/// <summary>
		/// Gets project debug output path.
		/// </summary>
		public static string GetDebugOutputPath(this Project project)
		{
			string path = Path.GetDirectoryName(project.FileName);
			return String.Format(@"{0}\bin\Debug\", path);
		}

		/// <summary>
		/// Gets project references.
		/// </summary>
		public static References GetReferences(this Project project)
		{
			VSProject vsproj = (VSProject)project.Object;
			return vsproj.References;
		}
	}
}
