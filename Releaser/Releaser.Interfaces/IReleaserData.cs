using System.Collections.Specialized;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Interface for accessing Releaser data.
	/// </summary>
	public interface IReleaserData
	{
		/// <summary>
		/// Initialize data engine.
		/// </summary>
		void Initialize(NameValueCollection config);
	}
}
