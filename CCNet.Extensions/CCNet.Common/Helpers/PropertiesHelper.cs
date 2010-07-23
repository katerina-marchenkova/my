using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;

namespace CCNet.Common
{
	/// <summary>
	/// Common methods for working with properties collections.
	/// </summary>
	public static class PropertiesHelper
	{
		#region Parsing from XML

		/// <summary>
		/// Gets properties collection from an XML node.
		/// </summary>
		public static Dictionary<string, string> ParseFromXml(XmlNode node)
		{
			Contract.Requires(node != null);

			Dictionary<string, string> properties = new Dictionary<string, string>();
			ResearchNode(properties, String.Empty, node);

			return properties;
		}

		/// <summary>
		/// Gets properties collection from an XML document.
		/// </summary>
		public static Dictionary<string, string> ParseFromXml(XmlDocument document)
		{
			Contract.Requires(document != null);

			return ParseFromXml(document.DocumentElement);
		}

		/// <summary>
		/// Reseraches specified node.
		/// </summary>
		private static void ResearchNode(Dictionary<string, string> properties, string prefix, XmlNode node)
		{
			RemoveXmlnsAttributes(node);
			RemoveCommentNodes(node);

			if (node.NodeType == XmlNodeType.Text)
			{
				properties.Add(
					prefix,
					node.Value.Trim());
				return;
			}

			string subPrefix = "{0}/{1}".Display(prefix, node.Name);
			if (node.ChildNodes.Count == 0 && node.Attributes.Count == 0)
			{
				properties.Add(
					subPrefix,
					String.Empty);
				return;
			}

			foreach (XmlAttribute attr in node.Attributes)
			{
				properties.Add(
					"{0}/{1}[@{2}]".Display(prefix, node.Name, attr.Name),
					attr.Value);
			}

			foreach (XmlNode child in node.ChildNodes)
			{
				ResearchNode(properties, subPrefix, child);
			}
		}

		/// <summary>
		/// Removes all namespace attributes from specified node.
		/// </summary>
		private static void RemoveXmlnsAttributes(XmlNode node)
		{
			if (node.Attributes == null)
				return;

			foreach (XmlAttribute attr in node.Attributes
				.Cast<XmlAttribute>()
				.Where(attr => attr.Name == "xmlns" || attr.Name.StartsWith("xmlns:", StringComparison.Ordinal))
				.ToList())
			{
				node.Attributes.Remove(attr);
			}
		}

		/// <summary>
		/// Removes all comment nodes from specified node.
		/// </summary>
		private static void RemoveCommentNodes(XmlNode node)
		{
			if (node.ChildNodes.Count == 0)
				return;

			foreach (XmlNode child in node.ChildNodes
				.Cast<XmlNode>()
				.Where(child => child.NodeType == XmlNodeType.Comment)
				.ToList())
			{
				node.RemoveChild(child);
			}
		}

		#endregion
	}
}
