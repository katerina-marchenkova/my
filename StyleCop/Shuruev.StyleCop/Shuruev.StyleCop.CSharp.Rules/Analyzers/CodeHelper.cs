using System.Text;
using System.Xml;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Helper methods for analyzing code.
	/// </summary>
	public static class CodeHelper
	{
		#region Identifying elements

		/// <summary>
		/// Checks whether specified element describes windows forms event handler.
		/// </summary>
		public static bool IsWindowsFormsEventHandler(CsElement element)
		{
			if (element.ElementType == ElementType.Method)
			{
				if (element.AccessModifier == AccessModifierType.Private)
				{
					if (element.Declaration.Name.Contains("_"))
					{
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
					}
				}
			}

			return false;
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
