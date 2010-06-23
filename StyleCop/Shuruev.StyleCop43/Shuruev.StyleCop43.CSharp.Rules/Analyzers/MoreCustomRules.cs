using System;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// More custom rules represented by StyleCop+ plug-in.
	/// </summary>
	public class MoreCustomRules
	{
		private readonly StyleCopPlus m_parent;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public MoreCustomRules(StyleCopPlus parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			m_parent = parent;
		}

		/// <summary>
		/// Analyzes source document.
		/// </summary>
		public void AnalyzeDocument(CodeDocument document)
		{
			CsDocument doc = (CsDocument)document;
			for (Node<CsToken> node = doc.Tokens.First; node != null; node = node.Next)
			{
				if (node.Next == null
					|| node.Next.Value.CsTokenType == CsTokenType.EndOfLine)
				{
					if (node.Value.CsTokenType == CsTokenType.WhiteSpace)
					{
						m_parent.AddViolation(
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
						m_parent.AddViolation(
							doc.RootElement,
							node.Value.LineNumber,
							Rules.CodeLineMustBeginWithIdenticalWhitespaces,
							new object[0]);
					}
				}
			}
		}
	}
}
