namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Interface for checking character validity.
	/// </summary>
	public interface ICharValidator
	{
		/// <summary>
		/// Checks if specified character is valid.
		/// </summary>
		bool IsValidChar(char c);
	}
}
