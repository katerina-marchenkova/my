using System;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace SourceTracker
{
	/// <summary>
	/// Represents a version in Team Foundation Server.
	/// </summary>
	public class TeamFoundationVersion : ISourceVersion
	{
		public Changeset Changeset { get; private set; }

		public TeamFoundationVersion(Changeset changeset)
		{
			Changeset = changeset;
		}

		#region Implementation of ISourceVersion

		/// <summary>
		/// Gets version key.
		/// </summary>
		public string VersionKey
		{
			get { return Changeset.ChangesetId.ToString(); }
		}

		/// <summary>
		/// Gets version date.
		/// </summary>
		public DateTime VersionDate
		{
			get { return Changeset.CreationDate; }
		}

		/// <summary>
		/// Gets user name.
		/// </summary>
		public string UserName
		{
			get { return Changeset.Committer; }
		}

		#endregion
	}
}
