namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Enumerates all rules used in add-in.
	/// </summary>
	internal enum Rules
	{
		#region Extended original rules

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

		/// <summary>
		/// Verifies that a constructor's summary text begins with the appropriate wording.
		/// Based on SA1642 behaviour, but allows any constructor to have the following summary text: "Initializes a new instance.".
		/// </summary>
		ConstructorSummaryDocumentationMustBeginWithStandardText,

		/// <summary>
		/// Verifies that a destructor's summary text begins with the appropriate wording.
		/// Based on SA1643 behaviour, but allows any destructor to have the following summary text: "Finalizes an instance.".
		/// </summary>
		DestructorSummaryDocumentationMustBeginWithStandardText,

		#endregion

		#region More custom rules

		/// <summary>
		/// Validates the spacing at the end of the each code line.
		/// </summary>
		CodeLineMustNotEndWithWhitespace,

		/// <summary>
		/// Validates that spacing at the beginning of the each code line uses identical characters.
		/// </summary>
		CodeLineMustBeginWithIdenticalWhitespaces

		#endregion
	}
}
