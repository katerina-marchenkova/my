using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;
using Attribute = Microsoft.StyleCop.CSharp.Attribute;
using Delegate = Microsoft.StyleCop.CSharp.Delegate;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Helper methods for analyzing code.
	/// </summary>
	public static class CodeHelper
	{
		#region Compatibility helpers

		/// <summary>
		/// Checks whether we are working with old StyleCop version.
		/// </summary>
		public static bool IsStyleCop43()
		{
			Assembly assembly = typeof(StyleCopCore).Assembly;
			Version version = assembly.GetName().Version;
			return version.Major == 4 && version.Minor == 3;
		}

		#endregion

		#region Identifying elements

		/// <summary>
		/// Checks whether specified element describes private event handler.
		/// </summary>
		public static bool IsPrivateEventHandler(CsElement element)
		{
			if (element.ElementType != ElementType.Method)
				return false;

			if (!IsPrivate(element))
				return false;

			Method method = (Method)element;
			if (method.Parameters.Count == 2)
			{
				Parameter sender = method.Parameters[0];
				Parameter args = method.Parameters[1];
				if (sender.Name == "sender"
					&& sender.Type.Text == "object"
					&& args.Name == "e"
					&& args.Type.Text.EndsWith("EventArgs"))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks whether specified element describes test method.
		/// </summary>
		public static bool IsTestMethod(CsElement element)
		{
			if (element.ElementType != ElementType.Method)
				return false;

			if (element.Attributes != null)
			{
				foreach (Attribute attr in element.Attributes)
				{
					for (Node<CsToken> node = attr.ChildTokens.First; node != null; node = node.Next)
					{
						if (node.Value.Text == "TestMethod")
							return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Checks whether specified element has public access modifier.
		/// </summary>
		public static bool IsPublic(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Public)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element has protected access modifier.
		/// </summary>
		public static bool IsProtected(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Protected)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element has private access modifier.
		/// </summary>
		public static bool IsPrivate(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Private)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element has internal access modifier.
		/// </summary>
		public static bool IsInternal(CsElement element)
		{
			AccessModifierType modifier = element.AccessModifier;

			if (modifier == AccessModifierType.Internal)
				return true;

			if (modifier == AccessModifierType.ProtectedInternal)
				return true;

			return false;
		}

		/// <summary>
		/// Checks whether specified element is static.
		/// </summary>
		public static bool IsStatic(CsElement element)
		{
			return element.Declaration.ContainsModifier(CsTokenType.Static);
		}

		/// <summary>
		/// Checks whether specified element is operator.
		/// </summary>
		public static bool IsOperator(CsElement element)
		{
			return element.Declaration.Name.StartsWith("operator ");
		}

		/// <summary>
		/// Checks whether specified element is generated.
		/// </summary>
		public static bool IsGenerated(CsElement element)
		{
			if (element.Attributes != null)
			{
				foreach (Attribute attr in element.Attributes)
				{
					for (Node<CsToken> node = attr.ChildTokens.First; node != null; node = node.Next)
					{
						if (node.Value.Text == "GeneratedCodeAttribute")
							return true;
					}
				}
			}

			return false;
		}

		#endregion

		#region Working with element names

		/// <summary>
		/// Extracts pure name from the declaration.
		/// </summary>
		public static string ExtractPureName(string declarationName)
		{
			string text = declarationName;

			string[] parts = text.Split('.');
			text = parts[parts.Length - 1];

			int index = text.IndexOf('<');
			if (index < 0)
				return text;

			return text.Substring(0, index);
		}

		#endregion

		#region Working with parameters

		/// <summary>
		/// Gets a list of parameters for an element.
		/// </summary>
		public static IList<Parameter> GetParameters(CsElement element)
		{
			switch (element.ElementType)
			{
				case ElementType.Constructor:
					return ((Constructor)element).Parameters;
				case ElementType.Delegate:
					return ((Delegate)element).Parameters;
				case ElementType.Indexer:
					return ((Indexer)element).Parameters;
				case ElementType.Method:
					return ((Method)element).Parameters;
				default:
					throw new InvalidOperationException(
						String.Format(
							"Can't find parameters for a {0} element.",
							element.FriendlyTypeText));
			}
		}

		/// <summary>
		/// Gets a list of type parameters for an element.
		/// </summary>
		public static List<string> GetTypeParameters(CsElement element)
		{
			List<string> names = new List<string>();

			for (Node<CsToken> node = element.Tokens.First; node != null; node = node.Next)
			{
				if (node.Value.CsTokenType == CsTokenType.OpenCurlyBracket
					|| node.Value.CsTokenType == CsTokenType.OpenParenthesis
					|| node.Value.CsTokenType == CsTokenType.BaseColon
					|| node.Value.CsTokenType == CsTokenType.Where)
					break;

				if (IsStyleCop43())
				{
					if (node.Value.Text == "where")
						break;
				}

				if (node.Value.CsTokenClass == CsTokenClass.GenericType)
				{
					GenericType type = (GenericType)node.Value;

					if (element.ElementType == ElementType.Method)
					{
						Method method = (Method)element;
						if (type == method.ReturnType)
							continue;
					}

					if (element.ElementType == ElementType.Delegate)
					{
						Delegate @delegate = (Delegate)element;
						if (type == @delegate.ReturnType)
							continue;
					}

					for (Node<CsToken> inner = type.ChildTokens.First; inner != null; inner = inner.Next)
					{
						if (inner.Value.CsTokenClass == CsTokenClass.Type)
						{
							string name = inner.Value.Text;
							names.Add(name);
						}
					}
				}
			}

			return names;
		}

		#endregion

		#region Working with local declarations

		/// <summary>
		/// Gets local declarations.
		/// </summary>
		public static List<LocalDeclaration> GetLocalDeclarations(CsElement element)
		{
			List<LocalDeclaration> result = new List<LocalDeclaration>();
			element.WalkElement(null, GetLocalDeclarationsStatementVisitor, result);
			return result;
		}

		/// <summary>
		/// Analyzes statements for getting local declarations.
		/// </summary>
		private static bool GetLocalDeclarationsStatementVisitor(
			Statement statement,
			Expression parentExpression,
			Statement parentStatement,
			CsElement parentElement,
			List<LocalDeclaration> declarations)
		{
			if (statement.StatementType == StatementType.Block)
				return true;

			if (statement.Variables.Count > 0)
			{
				foreach (Variable variable in statement.Variables)
				{
					declarations.Add(new LocalDeclaration
					{
						Name = variable.Name
					});
				}

				return true;
			}

			if (statement.StatementType == StatementType.VariableDeclaration)
			{
				VariableDeclarationStatement declaration = (VariableDeclarationStatement)statement;
				bool isConstant = declaration.Tokens.First.Value.CsTokenType == CsTokenType.Const;
				foreach (VariableDeclaratorExpression declarator in declaration.Declarators)
				{
					declarations.Add(new LocalDeclaration
					{
						Name = declarator.Identifier.Text,
						IsConstant = isConstant
					});
				}
			}
			return true;
		}

		#endregion

		#region Working with node tree

		/// <summary>
		/// Gets first node by specified line number.
		/// </summary>
		public static Node<CsToken> GetNodeByLine(CsDocument document, int lineNumber)
		{
			for (Node<CsToken> node = document.Tokens.First; node != null; node = node.Next)
			{
				if (node.Value.LineNumber == lineNumber)
					return node;
			}

			return null;
		}

		/// <summary>
		/// Finds previous valuable node.
		/// </summary>
		public static Node<CsToken> FindPreviousValueableNode(Node<CsToken> target)
		{
			for (Node<CsToken> node = target.Previous; node != null; node = node.Previous)
			{
				if (node.Value.CsTokenType == CsTokenType.WhiteSpace
					|| node.Value.CsTokenType == CsTokenType.EndOfLine)
					continue;

				return node;
			}

			return null;
		}

		/// <summary>
		/// Finds next valuable node.
		/// </summary>
		public static Node<CsToken> FindNextValueableNode(Node<CsToken> target)
		{
			for (Node<CsToken> node = target.Next; node != null; node = node.Next)
			{
				if (node.Value.CsTokenType == CsTokenType.WhiteSpace
					|| node.Value.CsTokenType == CsTokenType.EndOfLine)
					continue;

				return node;
			}

			return null;
		}

		#endregion

		#region Working with documentation

		/// <summary>
		/// Gets summary text for specified element.
		/// </summary>
		public static string GetSummaryText(CsElement element)
		{
			XmlDocument header = MakeXml(element.Header.Text);
			if (header != null)
			{
				XmlNode node = header.SelectSingleNode("root/summary");
				if (node != null)
				{
					return node.InnerXml.Trim();
				}
			}

			return null;
		}

		/// <summary>
		/// Creates XML document from its inner text.
		/// </summary>
		public static XmlDocument MakeXml(string text)
		{
			XmlDocument document = new XmlDocument();

			try
			{
				StringBuilder xml = new StringBuilder();
				xml.Append("<root>");
				xml.Append(text);
				xml.Append("</root>");
				document.LoadXml(xml.ToString());
			}
			catch (XmlException)
			{
				return null;
			}

			return document;
		}

		#endregion
	}
}
