using System;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Allows to setup derivings setting.
	/// </summary>
	public class DerivingsSpecialSetting : SimpleSpecialSetting
	{
		/// <summary>
		/// Gets help text for current setting.
		/// </summary>
		public override string HelpText
		{
			get { return Resources.SpecialSettingEditorHelpDerivings; }
		}

		/// <summary>
		/// Gets warning text for invalid definition string.
		/// </summary>
		public override string WarningText
		{
			get { return Resources.SpecialSettingEditorWarningDerivings; }
		}

		/// <summary>
		/// Checks if specified character is valid.
		/// </summary>
		public override bool IsValidChar(char c)
		{
			if (Char.IsWhiteSpace(c))
				return true;

			if (Char.IsLetterOrDigit(c))
				return true;

			if (c == '_')
				return true;

			return false;
		}
	}
}
