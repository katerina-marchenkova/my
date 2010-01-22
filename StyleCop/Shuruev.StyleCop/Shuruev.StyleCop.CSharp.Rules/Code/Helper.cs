using System.Text;
using System.Xml;
using Microsoft.StyleCop.CSharp;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Helper methods.
	/// </summary>
	public static class Helper
	{
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
	}
}
