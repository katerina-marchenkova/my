﻿namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Enumerates all rules used in add-in.
	/// </summary>
	internal enum Rules
	{
		/// <summary>
		/// Provides wide and flexible variety of naming rules.
		/// </summary>
		AdvancedNamingRules,

		#region Extended original rules

		/// <summary>
		/// Extends original SA1502 rule.
		/// </summary>
		ElementMustNotBeOnSingleLine,

		/// <summary>
		/// Extends original SA1509 rule.
		/// </summary>
		OpeningCurlyBracketsMustNotBePrecededByBlankLine,

		/// <summary>
		/// Extends original SA1513 rule.
		/// </summary>
		ClosingCurlyBracketMustBeFollowedByBlankLine,

		/// <summary>
		/// Extends original SA1516 rule.
		/// </summary>
		ElementsMustBeSeparatedByBlankLine,

		/// <summary>
		/// Extends original SA1642 rule. Ensure that you have disabled SA1642 when you enable this rule.
		/// Allows any constructor to have the following summary text: "Initializes a new instance.".
		/// </summary>
		ConstructorSummaryDocumentationMustBeginWithStandardText,

		/// <summary>
		/// Extends original SA1643 rule. Ensure that you have disabled SA1643 when you enable this rule.
		/// Allows any destructor to have the following summary text: "Finalizes an instance.".
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
