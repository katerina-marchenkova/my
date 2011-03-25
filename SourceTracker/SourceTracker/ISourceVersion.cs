using System;

namespace SourceTracker
{
	/// <summary>
	/// Represents source control version.
	/// </summary>
	public interface ISourceVersion
	{
		/// <summary>
		/// Gets version ID.
		/// </summary>
		int VersionId { get; }

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
