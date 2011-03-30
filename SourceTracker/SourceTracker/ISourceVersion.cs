using System;

namespace SourceTracker
{
	/// <summary>
	/// Represents source control version.
	/// </summary>
	public interface ISourceVersion
	{
		/// <summary>
		/// Gets version key.
		/// </summary>
		string VersionKey { get; }

		/// <summary>
		/// Gets version date.
		/// </summary>
		DateTime VersionDate { get; }

		/// <summary>
		/// Gets user name.
		/// </summary>
		string UserName { get; }
	}
}
