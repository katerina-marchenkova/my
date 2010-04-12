using System.Collections.Generic;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// StyleCop+ plug-in.
	/// </summary>
	[SourceAnalyzer(typeof(CsParser))]
	public class StyleCopPlus : SourceAnalyzer
	{
		public override ICollection<IPropertyControlPage> SettingsPages
		{
			get
			{
				return new IPropertyControlPage[] { new PropertyPage(this) };
			}
		}
	}
}
