using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace SourceTracker
{
	/// <summary>
	/// Provides methods for getting data from Team Foundation Server.
	/// </summary>
	public class TeamFoundationEngine : ISourceEngine
	{
		private readonly VersionControlServer m_server;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public TeamFoundationEngine(string uri)
		{
			var collection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(uri));
			m_server = (VersionControlServer)collection.GetService(typeof(VersionControlServer));
		}

		#region Implementation of ISourceEngine

		/// <summary>
		/// Returns a list of files that should be processed.
		/// </summary>
		public List<ISourceFile> GetFiles(ITrackerOptions options)
		{
			ItemSet set = m_server.GetItems(
				//"$/",
				"$/DataThrow/ContentCast/CC.Core/Properties/",
				VersionSpec.Latest,
				RecursionType.Full,
				DeletedState.Any,
				ItemType.File,
				false);

			return set.Items
				.Where(i => !options.SkipByExtension(i.ServerItem))
				.Where(i => !options.SkipByDate(i.CheckinDate))
				.Select(i => new TeamFoundationFile(i))
				.Cast<ISourceFile>()
				.ToList();
		}

		/// <summary>
		/// Returns a list of versions that should be processed.
		/// </summary>
		public List<ISourceVersion> GetVersions(ISourceFile file, ITrackerOptions options)
		{
			Item item = ((TeamFoundationFile)file).Item;

			var query = m_server.QueryHistory(
				item.ServerItem,
				VersionSpec.Latest,
				item.DeletionId,
				RecursionType.Full,
				null,
				null,
				null,
				Int32.MaxValue,
				true,
				false,
				false,
				false);

			return query
				.Cast<Changeset>()
				.Where(c => !options.SkipByDate(c.CreationDate))
				.Select(c => new TeamFoundationVersion(c))
				.Cast<ISourceVersion>()
				.ToList();
		}

		/// <summary>
		/// Returns a list of lines for specified version.
		/// </summary>
		public List<string> GetLines(ISourceVersion version)
		{
			List<string> lines = new List<string>();

			Changeset changeset = ((TeamFoundationVersion)version).Changeset;
			foreach (Change change in changeset.Changes)
			{
				using (StreamReader reader = new StreamReader(change.Item.DownloadFile()))
				{
					while (!reader.EndOfStream)
					{
						lines.Add(reader.ReadLine());
					}
				}
			}

			return lines;
		}

		#endregion
	}
}
