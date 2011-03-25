using System;
using System.IO;

namespace SourceTracker
{
	/// <summary>
	/// Provides methods used for specifying tracking options.
	/// </summary>
	public class TrackerOptions : ITrackerOptions
	{
		private readonly DateTime m_threshold;

		public TrackerOptions(DateTime lastRun)
		{
			m_threshold = lastRun.Subtract(TimeSpan.FromDays(1));
		}

		#region Implementation of ITrackerOptions

		/// <summary>
		/// Checks whether specified file should be skipped by its date.
		/// </summary>
		public bool SkipByDate(DateTime fileDate)
		{
			if (fileDate < m_threshold)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified file should be skipped by its extension.
		/// Throws exception if extension is unknown yet.
		/// </summary>
		public bool SkipByExtension(string fileName)
		{
			string ext = Path.GetExtension(fileName).ToLowerInvariant();
			switch (ext)
			{
				case ".asax":
				case ".ascx":
				case ".aspx":
				case ".cs":
				case ".cshtml":
				case ".css":
				case ".htm":
				case ".js":
				case ".master":
				case ".sql":
				case ".xaml":
					return false;

				case "":
				case ".ccproj":
				case ".config":
				case ".cscfg":
				case ".csdef":
				case ".csproj":
				case ".dll":
				case ".docx":
				case ".gif":
				case ".ico":
				case ".jpg":
				case ".jss":
				case ".pdb":
				case ".png":
				case ".pptx":
				case ".psess":
				case ".resx":
				case ".sln":
				case ".swf":
				case ".testsettings":
				case ".txt":
				case ".vsmdi":
				case ".vspscc":
				case ".vssscc":
				case ".xml":
					return true;

				default:
					throw new InvalidDataException(
						String.Format("Extension {0} is unknown yet.", ext));
			}
		}

		#endregion
	}
}
