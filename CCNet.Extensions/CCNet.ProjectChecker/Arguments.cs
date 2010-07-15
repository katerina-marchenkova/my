using CCNet.Common;

namespace CCNet.ProjectChecker
{
	/// <summary>
	/// Command line properties for current project.
	/// </summary>
	public static class Arguments
	{
		/// <summary>
		/// Default instance.
		/// </summary>
		public static ArgumentProperties Default { get; set; }

		/// <summary>
		/// Gets project name.
		/// </summary>
		public static string ProjectName
		{
			get { return Default.GetValue("ProjectName"); }
		}

		/// <summary>
		/// Gets project path.
		/// </summary>
		public static string ProjectPath
		{
			get { return Default.GetValue("ProjectPath"); }
		}

		/// <summary>
		/// Gets project type.
		/// </summary>
		public static string ProjectType
		{
			get { return Default.GetValue("ProjectType"); }
		}

		/// <summary>
		/// Gets Visual Studio version.
		/// </summary>
		public static string VisualStudioVersion
		{
			get { return Default.GetValue("VisualStudioVersion"); }
		}

		/// <summary>
		/// Gets target framework.
		/// </summary>
		public static string TargetFramework
		{
			get { return Default.GetValue("TargetFramework"); }
		}

		/// <summary>
		/// Gets target platform.
		/// </summary>
		public static string TargetPlatform
		{
			get { return Default.GetValue("TargetPlatform"); }
		}

		/// <summary>
		/// Gets root namespace.
		/// </summary>
		public static string RootNamespace
		{
			get { return Default.GetValue("RootNamespace"); }
		}
	}
}
