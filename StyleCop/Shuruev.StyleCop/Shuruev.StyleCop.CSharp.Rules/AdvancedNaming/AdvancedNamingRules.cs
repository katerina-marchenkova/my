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
				Check(settings, element, NamingSettings.Namespace);
			}

			if (element.ElementType == ElementType.Class)
			{
				if (CodeHelper.IsInternal(element))
				{
					Check(settings, element, NamingSettings.ClassInternal);
				}
				else
				{
					Check(settings, element, NamingSettings.ClassNotInternal);
				}
			}

			if (element.ElementType == ElementType.Struct)
			{
				if (CodeHelper.IsInternal(element))
				{
					Check(settings, element, NamingSettings.StructInternal);
				}
				else
				{
					Check(settings, element, NamingSettings.StructNotInternal);
				}
			}

			if (element.ElementType == ElementType.Interface)
			{
				Check(settings, element, NamingSettings.Interface);
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
		/// Checks common naming setting.
		/// </summary>
		private void Check(CurrentNamingSettings settings, CsElement element, string settingName)
		{
			Regex regex = settings.GetRegex(settingName);
			if (regex == null)
				return;

			string name = CodeHelper.ExtractPureName(element.Declaration.Name);

			string[] parts = name.Split('.');
			foreach (string part in parts)
			{
				if (!regex.IsMatch(part))
				{
					AddViolation(settings, element, settingName);
					return;
				}
			}
		}

		#endregion
	}
}
