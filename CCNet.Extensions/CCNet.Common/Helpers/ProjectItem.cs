namespace CCNet.Common
{
	/// <summary>
	/// Project item.
	/// </summary>
	public class ProjectItem
	{
		/// <summary>
		/// Gets or sets full item name.
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// Gets or sets assembly name.
		/// </summary>
		public ProjectItemType Type { get; set; }
	}
}
