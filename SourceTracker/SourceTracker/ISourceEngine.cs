using System.Collections.Generic;

namespace SourceTracker
{
	/// <summary>
	/// Provides methods for getting data from source control.
	/// </summary>
	public interface ISourceEngine
	{
		/// <summary>
		/// Returns a list of files that should be processed.
		/// </summary>
		List<ISourceFile> GetFiles(ITrackerOptions options);

		/// <summary>
		/// Returns a list of versions that should be processed.
		/// </summary>
		List<ISourceVersion> GetVersions(ISourceFile file, ITrackerOptions options);

		/// <summary>
		/// Returns a list of lines for specified version.
		/// </summary>
		List<string> GetLines(ISourceVersion version);
	}
}
