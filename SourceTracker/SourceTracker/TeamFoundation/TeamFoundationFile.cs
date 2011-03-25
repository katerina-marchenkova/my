using Microsoft.TeamFoundation.VersionControl.Client;

namespace SourceTracker
{
	/// <summary>
	/// Represents a file in Team Foundation Server.
	/// </summary>
	public class TeamFoundationFile : ISourceFile
	{
		public Item Item { get; private set; }

		public TeamFoundationFile(Item item)
		{
			Item = item;
		}

		#region Implementation of ISourceFile

		/// <summary>
		/// Gets file full path.
		/// </summary>
		public string FullPath
		{
			get { return Item.ServerItem; }
		}

		#endregion
	}
}
