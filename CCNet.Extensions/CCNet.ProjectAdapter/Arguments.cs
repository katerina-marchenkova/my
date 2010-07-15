using CCNet.Common;

namespace CCNet.ProjectAdapter
{
	/// <summary>
	/// Command line properties for current project.
	/// </summary>
	public static class Arguments
	{
		/// <summary>
		/// Default instance.
		/// </summary>
		public static ArgumentProperties Default;

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
		/// Gets external path.
		/// </summary>
		public static string ExternalPath
		{
			get { return Default.GetValue("ExternalPath"); }
		}

		/// <summary>
		/// Gets current version.
		/// </summary>
		public static string CurrentVersion
		{
			get { return Default.GetValue("CurrentVersion"); }
		}
	}
}
