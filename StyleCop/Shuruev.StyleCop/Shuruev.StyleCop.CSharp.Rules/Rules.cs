namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Enumerates all rules used in add-in.
	/// </summary>
	internal enum Rules
	{
		#region Limited Default Rules

		/// <summary>
		/// Validates that names of certain types of elements begin with an upper-case letter.
		/// Based on SA1300 behaviour, but allows windows forms event handlers to be named with a lower-case letter.
		/// </summary>
		ElementMustBeginWithUpperCaseLetter,

		/// <summary>
		/// Validates that an element contains a properly formatted documentation header.
		/// Based on SA1600 behaviour, but allows windows forms event handlers to be undocumented.
		/// </summary>
		ElementsMustBeDocumented,

		#endregion

		#region Advanced Spacing Rules

		/// <summary>
		/// Validates the spacing at the end of the each code line.
		/// </summary>
		CodeLineMustNotEndWithWhitespace,

		/// <summary>
		/// Validates the spacing at the beginning of the each code line.
		/// </summary>
		CodeLineMustNotBeginWithWhitespace

		#endregion
	}
}
