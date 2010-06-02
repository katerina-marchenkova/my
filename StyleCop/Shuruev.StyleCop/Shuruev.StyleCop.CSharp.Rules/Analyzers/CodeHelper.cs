using System;
using System.Collections.Generic;
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

			for (Node<CsToken> node = element.Tokens.First; node != null && node.Value.Parent == element; node = node.Next)
			{
				if (node.Value.CsTokenClass == CsTokenClass.GenericType)
				{
					GenericType type = (GenericType)node.Value;
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
		/// Gets first node by specified line number.
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
