namespace SourceTracker
{
	/// <summary>
	/// Represents source control file.
	/// </summary>
	public interface ISourceFile
	{
		/// <summary>
		/// Gets file full path.
		/// </summary>
		string FullPath { get; }
	}
}
