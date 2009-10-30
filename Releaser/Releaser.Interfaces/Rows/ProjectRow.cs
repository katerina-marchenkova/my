using System.Text;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Project in database.
	/// </summary>
	public class ProjectRow
	{
		/// <summary>
		/// Project ID.
		/// </summary>
		public int Id;

		/// <summary>
		/// Project data.
		/// </summary>
		public ProjectData Data;

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			sb.Append(Id);
			sb.Append("] ");
			sb.Append(Data);
			return sb.ToString();
		}
	}
}
