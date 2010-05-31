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
		private readonly AdvancedNamingRules m_advancedNamingRules;
		private readonly ExtendedOriginalRules m_extendedOriginalRules;
		private readonly MoreCustomRules m_moreCustomRules;

		private readonly List<string> m_disableAllRulesExcept;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public StyleCopPlus()
		{
			m_advancedNamingRules = new AdvancedNamingRules(this);
			m_extendedOriginalRules = new ExtendedOriginalRules(this);
			m_moreCustomRules = new MoreCustomRules(this);

			m_disableAllRulesExcept = new List<string>();
		}

		#region Properties

		/// <summary>
		/// Gets a collection of own settings pages.
		/// </summary>
		public override ICollection<IPropertyControlPage> SettingsPages
		{
			get
			{
				return new IPropertyControlPage[] { new PropertyPage(this) };
			}
		}

		/// <summary>
		/// Gets a set of rules that are enabled in the current instance.
		/// </summary>
		public List<string> DisableAllRulesExcept
		{
			get
			{
				return m_disableAllRulesExcept;
			}
		}

		#endregion

		#region Analyzer methods

		/// <summary>
		/// Checks whether specified rule is enabled.
		/// </summary>
		public override bool IsRuleEnabled(CodeDocument document, string ruleName)
		{
			if (m_disableAllRulesExcept.Count == 0)
				return base.IsRuleEnabled(document, ruleName);

			return m_disableAllRulesExcept.Contains(ruleName);
		}

		/// <summary>
		/// Initializes an add-in.
		/// </summary>
		public override void InitializeAddIn()
		{
			m_extendedOriginalRules.InitializeAddIn();
		}

		/// <summary>
		/// Analyzes source document.
		/// </summary>
		public override void AnalyzeDocument(CodeDocument document)
		{
			if (IsRuleEnabled(document, Rules.AdvancedNamingRules.ToString()))
				m_advancedNamingRules.AnalyzeDocument(document);

			m_extendedOriginalRules.AnalyzeDocument(document);
			m_moreCustomRules.AnalyzeDocument(document);
		}

		#endregion
	}
}
