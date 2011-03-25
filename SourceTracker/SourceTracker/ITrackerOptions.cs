using System;

namespace SourceTracker
{
	/// <summary>
	/// Provides methods used for specifying tracking options.
	/// </summary>
	public interface ITrackerOptions
	{
		/// <summary>
		/// Checks whether specified file should be skipped by its date.
		/// </summary>
		bool SkipByDate(DateTime fileDate);

		/// <summary>
		/// Checks whether specified file should be skipped by its extension.
		/// Throws exception if extension is unknown yet.
		/// </summary>
		bool SkipByExtension(string fileName);
	}
}
