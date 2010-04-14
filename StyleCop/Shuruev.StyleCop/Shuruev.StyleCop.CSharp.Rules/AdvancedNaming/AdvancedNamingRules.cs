using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Advanced naming rules.
	/// </summary>
	public class AdvancedNamingRules
	{
		private readonly SourceAnalyzer m_parent;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public AdvancedNamingRules(SourceAnalyzer parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			m_parent = parent;
		}

		#region Analyzing document

		/// <summary>
		/// Analyzes source document.
		/// </summary>
		public void AnalyzeDocument(CodeDocument document)
		{
			CurrentNamingSettings settings = new CurrentNamingSettings();
			settings.Initialize(m_parent, document);

			CsDocument doc = (CsDocument)document;
			AnalyzeElements(settings, doc.RootElement.ChildElements);
		}

		/// <summary>
		/// Analyzes a collection of elements.
		/// </summary>
		public void AnalyzeElements(CurrentNamingSettings settings, IEnumerable<CsElement> elements)
		{
			foreach (CsElement element in elements)
			{
				AnalyzeElement(settings, element);
				AnalyzeElements(settings, element.ChildElements);
			}
		}

		/// <summary>
		/// Analyzes specified element.
		/// </summary>
		public void AnalyzeElement(CurrentNamingSettings settings, CsElement element)
		{
			if (element.ElementType == ElementType.Namespace)
			{
				CheckNamespace(settings, element);
			}

			if (element.ElementType == ElementType.Class)
			{
				CheckClass(settings, element);
			}
		}

		/// <summary>
		/// Fires violation.
		/// </summary>
		private void AddViolation(CurrentNamingSettings settings, CsElement element, string settingName)
		{
			m_parent.AddViolation(
				element,
				Rules.AdvancedNamingRules,
				settings.GetFriendlyName(settingName),
				settings.GetExample(settingName));
		}

		#endregion

		#region Checking entity names

		/// <summary>
		/// Checks namespace name.
		/// </summary>
		private void CheckNamespace(CurrentNamingSettings settings, CsElement element)
		{
			Regex regex = settings.GetRegex(NamingSettings.Namespace);
			if (regex == null)
				return;

			foreach (string name in element.Declaration.Name.Split('.'))
			{
				if (!regex.IsMatch(name))
				{
					AddViolation(settings, element, NamingSettings.Namespace);
					return;
				}
			}
		}

		/// <summary>
		/// Checks class name.
		/// </summary>
		private void CheckClass(CurrentNamingSettings settings, CsElement element)
		{
			Regex regex = settings.GetRegex(NamingSettings.Class);
			if (regex == null)
				return;

			if (!regex.IsMatch(element.Declaration.Name))
				AddViolation(settings, element, NamingSettings.Class);
		}

		#endregion
	}
}
