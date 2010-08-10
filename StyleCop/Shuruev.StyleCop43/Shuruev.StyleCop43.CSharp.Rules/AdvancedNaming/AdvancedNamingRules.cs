using System;
using System.Collections.Generic;
using System.Text;
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
		private static object s_xxx = new object();
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
			AnalyzeElements(settings, doc.RootElement.ChildElements);
		}

		/// <summary>
		/// Analyzes a collection of elements.
		/// </summary>
		public void AnalyzeElements(CurrentNamingSettings settings, IEnumerable<CsElement> elements)
		{
			foreach (CsElement element in elements)
			{
				if (CodeHelper.IsGenerated(element))
					continue;

				AnalyzeElement(settings, element);
				AnalyzeElements(settings, element.ChildElements);
			}
		}

		/// <summary>
		/// Analyzes specified element.
		/// </summary>
		public void AnalyzeElement(CurrentNamingSettings settings, CsElement element)
		{
			switch (element.ElementType)
			{
				case ElementType.Class:
					AnalyzeClass(settings, element);
					break;
				case ElementType.Constructor:
					AnalyzeConstructor(settings, element);
					break;
				case ElementType.Delegate:
					AnalyzeDelegate(settings, element);
					break;
				case ElementType.Enum:
					AnalyzeEnum(settings, element);
					break;
				case ElementType.EnumItem:
					AnalyzeEnumItem(settings, element);
					break;
				case ElementType.Event:
					AnalyzeEvent(settings, element);
					break;
				case ElementType.Field:
					AnalyzeField(settings, element);
					break;
				case ElementType.Indexer:
					AnalyzeIndexer(settings, element);
					break;
				case ElementType.Interface:
					AnalyzeInterface(settings, element);
					break;
				case ElementType.Method:
					AnalyzeMethod(settings, element);
					break;
				case ElementType.Namespace:
					AnalyzeNamespace(settings, element);
					break;
				case ElementType.Property:
					AnalyzeProperty(settings, element);
					break;
				case ElementType.Struct:
					AnalyzeStruct(settings, element);
					break;
			}
		}

		/// <summary>
		/// Analyzes class element.
		/// </summary>
		private void AnalyzeClass(CurrentNamingSettings settings, CsElement element)
		{
			CheckDerivings(settings, element);

			if (CodeHelper.IsInternal(element))
			{
				CheckDeclaration(settings, NamingSettings.ClassInternal, element);
			}
			else
			{
				CheckDeclaration(settings, NamingSettings.ClassNotInternal, element);
			}

			CheckTypeParameters(settings, element);
		}

		/// <summary>
		/// Analyzes constructor element.
		/// </summary>
		private void AnalyzeConstructor(CurrentNamingSettings settings, CsElement element)
		{
			CheckParameters(settings, element);
		}

		/// <summary>
		/// Analyzes delegate element.
		/// </summary>
		private void AnalyzeDelegate(CurrentNamingSettings settings, CsElement element)
		{
			CheckDeclaration(settings, NamingSettings.Delegate, element);
			CheckTypeParameters(settings, element);
			CheckParameters(settings, element);
		}

		/// <summary>
		/// Analyzes enum element.
		/// </summary>
		private void AnalyzeEnum(CurrentNamingSettings settings, CsElement element)
		{
			CheckDeclaration(settings, NamingSettings.Enum, element);
		}

		/// <summary>
		/// Analyzes enum item element.
		/// </summary>
		private void AnalyzeEnumItem(CurrentNamingSettings settings, CsElement element)
		{
			CheckDeclaration(settings, NamingSettings.EnumItem, element);
		}

		/// <summary>
		/// Analyzes event element.
		/// </summary>
		private void AnalyzeEvent(CurrentNamingSettings settings, CsElement element)
		{
			CheckDeclaration(settings, NamingSettings.Event, element);
		}

		/// <summary>
		/// Analyzes field element.
		/// </summary>
		private void AnalyzeField(CurrentNamingSettings settings, CsElement element)
		{
			Field field = (Field)element;
			if (field.Const)
			{
				CheckDeclarationAccess(
					settings,
					NamingSettings.PublicConst,
					NamingSettings.ProtectedConst,
					NamingSettings.PrivateConst,
					NamingSettings.InternalConst,
					field);
			}
			else if (CodeHelper.IsStatic(field))
			{
				CheckDeclarationAccess(
					settings,
					NamingSettings.PublicStaticField,
					NamingSettings.ProtectedStaticField,
					NamingSettings.PrivateStaticField,
					NamingSettings.InternalStaticField,
					field);
			}
			else
			{
				CheckDeclarationAccess(
					settings,
					NamingSettings.PublicInstanceField,
					NamingSettings.ProtectedInstanceField,
					NamingSettings.PrivateInstanceField,
					NamingSettings.InternalInstanceField,
					field);
			}
		}

		/// <summary>
		/// Analyzes indexer element.
		/// </summary>
		private void AnalyzeIndexer(CurrentNamingSettings settings, CsElement element)
		{
			CheckParameters(settings, element);
		}

		/// <summary>
		/// Analyzes interface element.
		/// </summary>
		private void AnalyzeInterface(CurrentNamingSettings settings, CsElement element)
		{
			CheckDeclaration(settings, NamingSettings.Interface, element);
		}

		/// <summary>
		/// Analyzes method element.
		/// </summary>
		private void AnalyzeMethod(CurrentNamingSettings settings, CsElement element)
		{
			if (!CodeHelper.IsOperator(element))
			{
				if (CodeHelper.IsPrivateEventHandler(element))
				{
					CheckDeclaration(settings, NamingSettings.MethodPrivateEventHandler, element);
				}
				else if (CodeHelper.IsTestMethod(element))
				{
					CheckDeclaration(settings, NamingSettings.MethodTest, element);
				}
				else
				{
					CheckDeclaration(settings, NamingSettings.MethodGeneral, element);
				}
			}

			CheckTypeParameters(settings, element);
			CheckParameters(settings, element);
		}

		/// <summary>
		/// Analyzes namespace element.
		/// </summary>
		private void AnalyzeNamespace(CurrentNamingSettings settings, CsElement element)
		{
			string[] parts = element.Declaration.Name.Split('.');
			foreach (string part in parts)
			{
				if (!CheckName(settings, NamingSettings.Namespace, element, part))
					break;
			}
		}

		/// <summary>
		/// Analyzes property element.
		/// </summary>
		private void AnalyzeProperty(CurrentNamingSettings settings, CsElement element)
		{
			CheckDeclaration(settings, NamingSettings.Property, element);
		}

		/// <summary>
		/// Analyzes struct element.
		/// </summary>
		private void AnalyzeStruct(CurrentNamingSettings settings, CsElement element)
		{
			if (CodeHelper.IsInternal(element))
			{
				CheckDeclaration(settings, NamingSettings.StructInternal, element);
			}
			else
			{
				CheckDeclaration(settings, NamingSettings.StructNotInternal, element);
			}
		}

		#endregion

		#region Checking entity names

		/// <summary>
		/// Checks declaration naming for common access modifiers.
		/// </summary>
		private void CheckDeclarationAccess(
			CurrentNamingSettings settings,
			string publicSettingName,
			string protectedSettingName,
			string privateSettingName,
			string internalSettingName,
			CsElement element)
		{
			if (CodeHelper.IsPublic(element))
			{
				CheckDeclaration(settings, publicSettingName, element);
			}
			else if (CodeHelper.IsProtected(element))
			{
				CheckDeclaration(settings, protectedSettingName, element);
			}
			else if (CodeHelper.IsPrivate(element))
			{
				CheckDeclaration(settings, privateSettingName, element);
			}
			else if (CodeHelper.IsInternal(element))
			{
				CheckDeclaration(settings, internalSettingName, element);
			}
		}

		/// <summary>
		/// Checks declaration naming.
		/// </summary>
		private void CheckDeclaration(
			CurrentNamingSettings settings,
			string settingName,
			CsElement element)
		{
			string name = CodeHelper.ExtractPureName(element.Declaration.Name);
			CheckName(settings, settingName, element, name);
		}

		/// <summary>
		/// Checks parameters naming.
		/// </summary>
		private void CheckParameters(
			CurrentNamingSettings settings,
			CsElement element)
		{
			foreach (Parameter parameter in CodeHelper.GetParameters(element))
			{
				string name = parameter.Name;
				if (!CheckName(settings, NamingSettings.Parameter, element, name))
					break;
			}
		}

		/// <summary>
		/// Checks type parameters naming.
		/// </summary>
		private void CheckTypeParameters(
			CurrentNamingSettings settings,
			CsElement element)
		{
			foreach (string name in CodeHelper.GetTypeParameters(element))
			{
				if (!CheckName(settings, NamingSettings.TypeParameter, element, name))
					break;
			}
		}

		/// <summary>
		/// Checks specified name.
		/// </summary>
		private bool CheckName(
			CurrentNamingSettings settings,
			string settingName,
			CsElement element,
			string nameToCheck)
		{
			//xxx
			/*xxxlock (s_xxx)
			{
				foreach (string word in SplitIntoWords(nameToCheck))
					System.IO.File.AppendAllText(@"D:\Words.txt", word + Environment.NewLine);
			}*/

			Regex regex = settings.GetRegex(settingName);
			if (regex == null)
				return true;

			if (regex.IsMatch(nameToCheck))
				return true;

			AddViolation(settings, element, settingName, nameToCheck);
			return false;
		}

		/// <summary>
		/// Checks derivings condition.
		/// </summary>
		private void CheckDerivings(CurrentNamingSettings settings, CsElement element)
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
				friendlyName,
				name,
				example);
		}

		/// <summary>
		/// Fires violation.
		/// </summary>
		private void AddViolation(
			CurrentNamingSettings settings,
			CsElement element,
			string settingName,
			string currentName)
		{
			AddViolation(
				element,
				settings.GetFriendlyName(settingName),
				currentName,
				settings.GetExample(settingName));
		}

		/// <summary>
		/// Fires violation.
		/// </summary>
		private void AddViolation(
			CsElement element,
			string friendlyName,
			string currentName,
			string example)
		{
			m_parent.AddViolation(
				element,
				Rules.AdvancedNamingRules,
				friendlyName,
				currentName,
				example);
		}

		#endregion

		#region XXX

		/// <summary>
		/// Splits specified name into words.
		/// </summary>
		public static List<string> SplitIntoWords(string name)
		{
			List<string> words = new List<string>();
			StringBuilder word = new StringBuilder();
			bool isLetter = false;
			bool isDigit = false;
			bool isUpper = false;
			foreach (char c in name)
			{
				if (word.Length > 0)
				{
					if (Char.IsLetter(c) != isLetter
						|| Char.IsDigit(c) != isDigit
						|| (Char.IsLetter(c) && Char.IsUpper(c) && !isUpper))
					{
						words.Add(word.ToString());
						word.Length = 0;
					}
				}

				isLetter = Char.IsLetter(c);
				isDigit = Char.IsDigit(c);
				isUpper = Char.IsUpper(c);

				if (isLetter || isDigit)
					word.Append(c);
			}

			if (word.Length > 0)
				words.Add(word.ToString());

			return words;
		}

		#endregion
	}
}
