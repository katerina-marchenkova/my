namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Interface for checking text validity.
	/// </summary>
	public interface ITextValidator
	{
		/// <summary>
		/// Checks if specified text is valid.
		/// </summary>
		bool IsValidText(string text);
	}
}
