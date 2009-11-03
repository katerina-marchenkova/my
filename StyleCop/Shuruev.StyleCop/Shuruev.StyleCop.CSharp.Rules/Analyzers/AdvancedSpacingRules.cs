using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	[SourceAnalyzer(typeof(CsParser))]
	public class AdvancedSpacingRules : SourceAnalyzer
	{
		public override void AnalyzeDocument(CodeDocument document)
		{
			CsDocument doc = (CsDocument)document;
			for (Node<CsToken> node = doc.Tokens.First; node != null; node = node.Next)
			{
				if (node.Next == null
					|| node.Next.Value.CsTokenType == CsTokenType.EndOfLine)
				{
					if (node.Value.CsTokenType == CsTokenType.WhiteSpace)
					{
							AddViolation(
								doc.RootElement,
								node.Value.LineNumber,
								Rules.CodeLineMustNotEndWithWhitespace,
								new object[0]);
					}
				}

				if (node.Previous != null
					&& node.Previous.Value.CsTokenType == CsTokenType.EndOfLine)
				{
					if (node.Value.CsTokenType == CsTokenType.WhiteSpace
						&& node.Value.Text.Contains(" ")
						&& node.Value.Text.Contains("\t"))
					{
						AddViolation(
							doc.RootElement,
							node.Value.LineNumber,
							Rules.CodeLineMustNotBeginWithWhitespace,
							new object[0]);
					}
				}
			}
		}
	}
}
