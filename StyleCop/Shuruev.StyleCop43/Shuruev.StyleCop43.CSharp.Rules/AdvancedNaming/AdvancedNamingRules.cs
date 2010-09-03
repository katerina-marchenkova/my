using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Advanced naming rules.
	/// </summary>
	public class AdvancedNamingRules
	{
		private readonly StyleCopPlus m_parent;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public AdvancedNamingRules(StyleCopPlus parent)
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
			AnalyzeElements(doc.RootElement.ChildElements, settings);
		}

		/// <summary>
		/// Analyzes a collection of elements.
		/// </summary>
		public void AnalyzeElements(IEnumerable<CsElement> elements, CurrentNamingSettings settings)
		{
			foreach (CsElement element in elements)
			{
				if (CodeHelper.IsGenerated(element))
					continue;

				AnalyzeElement(element, settings);
				AnalyzeElements(element.ChildElements, settings);
			}
		}

		/// <summary>
		/// Analyzes specified element.
		/// </summary>
		public void AnalyzeElement(CsElement element, CurrentNamingSettings settings)
		{
			switch (element.ElementType)
			{
				case ElementType.Class:
					AnalyzeClass(element, settings);
					break;
				case ElementType.Constructor:
					AnalyzeConstructor(element, settings);
					break;
				case ElementType.Delegate:
					AnalyzeDelegate(element, settings);
					break;
				case ElementType.Enum:
					AnalyzeEnum(element, settings);
					break;
				case ElementType.EnumItem:
					AnalyzeEnumItem(element, settings);
					break;
				case ElementType.Event:
					AnalyzeEvent(element, settings);
					break;
				case ElementType.Field:
					AnalyzeField(element, settings);
					break;
				case ElementType.Indexer:
					AnalyzeIndexer(element, settings);
					break;
				case ElementType.Interface:
					AnalyzeInterface(element, settings);
					break;
				case ElementType.Method:
					AnalyzeMethod(element, settings);
					break;
				case ElementType.Namespace:
					AnalyzeNamespace(element, settings);
					break;
				case ElementType.Property:
					AnalyzeProperty(element, settings);
					break;
				case ElementType.Struct:
					AnalyzeStruct(element, settings);
					break;
			}
		}

		/// <summary>
		/// Analyzes class element.
		/// </summary>
		private void AnalyzeClass(CsElement element, CurrentNamingSettings settings)
		{
			CheckDerivings(element, settings);

			if (CodeHelper.IsInternal(element))
			{
				CheckDeclaration(element, settings, NamingSettings.ClassInternal);
			}
			else
			{
				CheckDeclaration(element, settings, NamingSettings.ClassNotInternal);
			}

			CheckTypeParameters(element, settings);
		}

		/// <summary>
		/// Analyzes constructor element.
		/// </summary>
		private void AnalyzeConstructor(CsElement element, CurrentNamingSettings settings)
		{
			CheckParameters(element, settings);
			CheckLocalDeclarations(element, settings);
		}

		/// <summary>
		/// Analyzes delegate element.
		/// </summary>
		private void AnalyzeDelegate(CsElement element, CurrentNamingSettings settings)
		{
			CheckDeclaration(element, settings, NamingSettings.Delegate);
			CheckTypeParameters(element, settings);
			CheckParameters(element, settings);
		}

		/// <summary>
		/// Analyzes enum element.
		/// </summary>
		private void AnalyzeEnum(CsElement element, CurrentNamingSettings settings)
		{
			CheckDeclaration(element, settings, NamingSettings.Enum);
		}

		/// <summary>
		/// Analyzes enum item element.
		/// </summary>
		private void AnalyzeEnumItem(CsElement element, CurrentNamingSettings settings)
		{
			CheckDeclaration(element, settings, NamingSettings.EnumItem);
		}

		/// <summary>
		/// Analyzes event element.
		/// </summary>
		private void AnalyzeEvent(CsElement element, CurrentNamingSettings settings)
		{
			CheckDeclaration(element, settings, NamingSettings.Event);
		}

		/// <summary>
		/// Analyzes field element.
		/// </summary>
		private void AnalyzeField(CsElement element, CurrentNamingSettings settings)
		{
			Field field = (Field)element;
			if (field.Const)
			{
				CheckDeclarationAccess(
					field,
					settings,
					NamingSettings.PublicConst,
					NamingSettings.ProtectedConst,
					NamingSettings.PrivateConst,
					NamingSettings.InternalConst);
			}
			else if (CodeHelper.IsStatic(field))
			{
				CheckDeclarationAccess(
					field,
					settings,
					NamingSettings.PublicStaticField,
					NamingSettings.ProtectedStaticField,
					NamingSettings.PrivateStaticField,
					NamingSettings.InternalStaticField);
			}
			else
			{
				CheckDeclarationAccess(
					field,
					settings,
					NamingSettings.PublicInstanceField,
					NamingSettings.ProtectedInstanceField,
					NamingSettings.PrivateInstanceField,
					NamingSettings.InternalInstanceField);
			}
		}

		/// <summary>
		/// Analyzes indexer element.
		/// </summary>
		private void AnalyzeIndexer(CsElement element, CurrentNamingSettings settings)
		{
			CheckParameters(element, settings);
			CheckLocalDeclarations(element, settings);
		}

		/// <summary>
		/// Analyzes interface element.
		/// </summary>
		private void AnalyzeInterface(CsElement element, CurrentNamingSettings settings)
		{
			CheckDeclaration(element, settings, NamingSettings.Interface);
		}

		/// <summary>
		/// Analyzes method element.
		/// </summary>
		private void AnalyzeMethod(CsElement element, CurrentNamingSettings settings)
		{
			if (!CodeHelper.IsOperator(element))
			{
				if (CodeHelper.IsPrivateEventHandler(element))
				{
					CheckDeclaration(element, settings, NamingSettings.MethodPrivateEventHandler);
				}
				else if (CodeHelper.IsTestMethod(element))
				{
					CheckDeclaration(element, settings, NamingSettings.MethodTest);
				}
				else
				{
					CheckDeclaration(element, settings, NamingSettings.MethodGeneral);
				}
			}

			CheckTypeParameters(element, settings);
			CheckParameters(element, settings);
			CheckLocalDeclarations(element, settings);
		}

		/// <summary>
		/// Analyzes namespace element.
		/// </summary>
		private void AnalyzeNamespace(CsElement element, CurrentNamingSettings settings)
		{
			string[] parts = element.Declaration.Name.Split('.');
			foreach (string part in parts)
			{
				if (!CheckName(element, null, settings, NamingSettings.Namespace, part))
					break;
			}
		}

		/// <summary>
		/// Analyzes property element.
		/// </summary>
		private void AnalyzeProperty(CsElement element, CurrentNamingSettings settings)
		{
			CheckDeclaration(element, settings, NamingSettings.Property);
			CheckLocalDeclarations(element, settings);
		}

		/// <summary>
		/// Analyzes struct element.
		/// </summary>
		private void AnalyzeStruct(CsElement element, CurrentNamingSettings settings)
		{
			if (CodeHelper.IsInternal(element))
			{
				CheckDeclaration(element, settings, NamingSettings.StructInternal);
			}
			else
			{
				CheckDeclaration(element, settings, NamingSettings.StructNotInternal);
			}
		}

		#endregion

		#region Checking entity names

		/// <summary>
		/// Checks declaration naming for common access modifiers.
		/// </summary>
		private void CheckDeclarationAccess(
			CsElement element,
			CurrentNamingSettings settings,
			string publicSettingName,
			string protectedSettingName,
			string privateSettingName,
			string internalSettingName)
		{
			if (CodeHelper.IsPublic(element))
			{
				CheckDeclaration(element, settings, publicSettingName);
			}
			else if (CodeHelper.IsProtected(element))
			{
				CheckDeclaration(element, settings, protectedSettingName);
			}
			else if (CodeHelper.IsPrivate(element))
			{
				CheckDeclaration(element, settings, privateSettingName);
			}
			else if (CodeHelper.IsInternal(element))
			{
				CheckDeclaration(element, settings, internalSettingName);
			}
		}

		/// <summary>
		/// Checks declaration naming.
		/// </summary>
		private void CheckDeclaration(CsElement element, CurrentNamingSettings settings, string settingName)
		{
			CheckName(element, null, settings, settingName, element.Declaration.Name);
		}

		/// <summary>
		/// Checks parameters naming.
		/// </summary>
		private void CheckParameters(CsElement element, CurrentNamingSettings settings)
		{
			foreach (Parameter parameter in CodeHelper.GetParameters(element))
			{
				string name = parameter.Name;
				if (!CheckName(element, parameter.LineNumber, settings, NamingSettings.Parameter, name))
					break;
			}
		}

		/// <summary>
		/// Checks type parameters naming.
		/// </summary>
		private void CheckTypeParameters(CsElement element, CurrentNamingSettings settings)
		{
			foreach (string name in CodeHelper.GetTypeParameters(element))
			{
				if (!CheckName(element, null, settings, NamingSettings.TypeParameter, name))
					break;
			}
		}

		/// <summary>
		/// Checks local declarations naming.
		/// </summary>
		private void CheckLocalDeclarations(CsElement element, CurrentNamingSettings settings)
		{
			foreach (LocalDeclarationItem declaration in CodeHelper.GetLocalDeclarations(element))
			{
				if (declaration.IsConstant)
				{
					CheckName(
						element,
						declaration.LineNumber,
						settings,
						NamingSettings.LocalConstant,
						declaration.Name);
				}
				else
				{
					CheckName(
						element,
						declaration.LineNumber,
						settings,
						NamingSettings.LocalVariable,
						declaration.Name);
				}
			}
		}

		/// <summary>
		/// Checks specified name.
		/// </summary>
		private bool CheckName(
			CsElement element,
			int? lineNumber,
			CurrentNamingSettings settings,
			string settingName,
			string nameToCheck)
		{
			Regex regex = settings.GetRegex(settingName);
			if (regex == null)
				return true;

			nameToCheck = CodeHelper.ExtractPureName(nameToCheck);
			if (regex.IsMatch(nameToCheck))
				return true;

			AddViolation(element, lineNumber, settings, settingName, nameToCheck);
			return false;
		}

		/// <summary>
		/// Checks derivings condition.
		/// </summary>
		private void CheckDerivings(CsElement element, CurrentNamingSettings settings)
		{
			Class @class = (Class)element;
			if (String.IsNullOrEmpty(@class.BaseClass))
				return;

			string name = CodeHelper.ExtractPureName(@class.Declaration.Name);
			string baseName = CodeHelper.ExtractPureName(@class.BaseClass);

			string deriving;
			if (settings.CheckDerivedName(baseName, name, out deriving))
				return;

			string friendlyName = Resources.DerivedClassFriendlyName;
			string example = String.Format(Resources.DerivingExample, deriving);

			AddViolation(
				element,
				null,
				friendlyName,
				name,
				example);
		}

		#endregion

		#region Firing violations

		/// <summary>
		/// Fires violation.
		/// </summary>
		private void AddViolation(
			CsElement element,
			int? lineNumber,
			CurrentNamingSettings settings,
			string settingName,
			string currentName)
		{
			AddViolation(
				element,
				lineNumber,
				settings.GetFriendlyName(settingName),
				currentName,
				settings.GetExample(settingName));
		}

		/// <summary>
		/// Fires low-level violation.
		/// </summary>
		private void AddViolation(
			CsElement element,
			int? lineNumber,
			string friendlyName,
			string currentName,
			string example)
		{
			if (lineNumber.HasValue)
			{
				m_parent.AddViolation(
					element,
					lineNumber.Value,
					Rules.AdvancedNamingRules,
					friendlyName,
					currentName,
					example);
			}
			else
			{
				m_parent.AddViolation(
					element,
					Rules.AdvancedNamingRules,
					friendlyName,
					currentName,
					example);
			}
		}

		#endregion
	}
}
