using System;
using System.Text;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Project in database.
	/// </summary>
	public class ProjectRow
	{
		private readonly int id;
		private readonly ProjectData data;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectRow"/> class.
		/// </summary>
		public ProjectRow(int id, ProjectData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			this.id = id;
			this.data = data;
		}

		#region Properties

		/// <summary>
		/// Gets project ID.
		/// </summary>
		public int Id
		{
			get { return this.id; }
		}

		/// <summary>
		/// Gets project data.
		/// </summary>
		public ProjectData Data
		{
			get { return this.data; }
		}

		#endregion

		#region Service methods

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			sb.Append(this.Id);
			sb.Append("] ");
			sb.Append(this.Data);
			return sb.ToString();
		}

		#endregion
	}
}
