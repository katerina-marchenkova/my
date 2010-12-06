namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Tag object for custom rule setting.
	/// </summary>
	internal class CustomRuleTag : SettingTag
	{
		/// <summary>
		/// Gets or sets rule object.
		/// </summary>
		internal CustomRule Rule { get; set; }
	}
}
