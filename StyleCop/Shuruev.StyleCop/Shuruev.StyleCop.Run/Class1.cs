using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Shuruev.StyleCop.Test
{
	[SuppressMessage(
		"Microsoft.StyleCop.CSharp.LayoutRules",
		"SA1505:OpeningCurlyBracketsMustNotBeFollowedByBlankLine",
		Justification = "Some justification here.")]
	[SuppressMessage(
		"Microsoft.StyleCop.CSharp.LayoutRules",
		"SA1507:CodeMustNotContainMultipleBlankLinesInARow",
		Justification = "Some justification here.")]
	[SuppressMessage(
		"Shuruev.StyleCop.CSharp.StyleCopPlus",
		"SP0100:AdvancedNamingRules",
		Justification = "Some justification here.")]
	public class Class1
	{
		public Class1()
		{


			int AAA = 10;
		}

		public Class1(int z)
			: this()
		{
		zzz:
			int AAA = 10;
		}

		~Class1()
		{


		ZZZ:
			int AAA = 10;
		}

		public string P
		{
			get
			{
				return null;
			}
			set
			{
				int a = 10;
				int b = 10;
			}
		}

		public void A()
		{
			int a = 10;
		}
	}
}
