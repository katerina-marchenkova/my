﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CCNet.Common.Properties;

namespace CCNet.Common
{
	/// <summary>
	/// Common methods for working with project.
	/// </summary>
	public static class ProjectHelper
	{
		private static XmlDocument s_projectDocument;
		private static XmlNamespaceManager s_namespaceManager;

		#region Loading project

		/// <summary>
		/// Loads project document.
		/// </summary>
		public static void LoadProject(string projectFilePath)
		{
			string xml = File.ReadAllText(projectFilePath);
			s_projectDocument = new XmlDocument();
			s_projectDocument.LoadXml(xml);

			s_namespaceManager = new XmlNamespaceManager(s_projectDocument.NameTable);
			s_namespaceManager.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");
		}

		#endregion

		#region Service methods

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		private static XmlNode SelectSingleNode(string xpath)
		{
			if (s_projectDocument == null)
				throw new InvalidOperationException("Project document is not loaded.");

			return s_projectDocument.SelectSingleNode(xpath, s_namespaceManager);
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		private static XmlNodeList SelectNodes(string xpath)
		{
			if (s_projectDocument == null)
				throw new InvalidOperationException("Project document is not loaded.");

			return s_projectDocument.SelectNodes(xpath, s_namespaceManager);
		}

		#endregion

		#region Visual Studio version

		/// <summary>
		/// Gets Visual Studio version.
		/// </summary>
		public static string GetVisualStudioVersion()
		{
			string toolsVersion = GetToolsVersion();
			return ResolveVisualStudioVersion(toolsVersion);
		}

		/// <summary>
		/// Gets the value of "Tools Version" project attribute.
		/// </summary>
		private static string GetToolsVersion()
		{
			XmlNode node = SelectSingleNode("/ms:Project");
			if (node == null)
				return null;

			XmlAttribute attr = node.Attributes["ToolsVersion"];
			if (attr == null)
				return null;

			return attr.Value;
		}

		/// <summary>
		/// Converts tools version string to Visual Studio version.
		/// </summary>
		private static string ResolveVisualStudioVersion(string toolsVersion)
		{
			switch (toolsVersion)
			{
				case "3.5":
					return "2008";
				case "4.0":
					return "2010";
				default:
					return Resources.Unknown;
			}
		}

		#endregion

		#region Platform and configuration

		/// <summary>
		/// Gets a list of used conditions.
		/// </summary>
		public static List<string> GetUsedConditions()
		{
			List<string> result = new List<string>();
			foreach (XmlNode node in SelectNodes("/ms:Project/ms:PropertyGroup[@Condition]"))
			{
				string condition = node.Attributes["Condition"].Value;
				string value = condition
					.Replace("'$(Configuration)|$(Platform)' == ", String.Empty)
					.Trim('\'', ' ');

				result.Add(value);
			}

			return result;
		}

		/// <summary>
		/// Gets a list of used configurations.
		/// </summary>
		public static List<string> GetUsedConfigurations()
		{
			return GetUsedConditions()
				.Select(condition => condition.Split('|'))
				.Where(parts => parts.Length > 0)
				.Select(parts => parts[0])
				.Distinct()
				.ToList();
		}

		/// <summary>
		/// Gets a list of used platforms.
		/// </summary>
		public static List<string> GetUsedPlatforms()
		{
			return GetUsedConditions()
				.Select(condition => condition.Split('|'))
				.Where(parts => parts.Length > 1)
				.Select(parts => parts[1])
				.Distinct()
				.ToList();
		}

		#endregion

		#region Project properties

		/// <summary>
		/// Gets all common properties.
		/// </summary>
		public static Dictionary<string, string> GetCommonProperties()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			foreach (XmlNode node in SelectNodes("/ms:Project/ms:PropertyGroup[not(@Condition)]"))
			{
				foreach (XmlNode child in node.ChildNodes)
				{
					result.Add(child.Name, child.InnerText);
				}
			}

			return result;
		}

		/// <summary>
		/// Gets all properties specific to condition.
		/// </summary>
		private static Dictionary<string, string> GetConditionProperties(string condition)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			XmlNode node = SelectSingleNode(
				"/ms:Project/ms:PropertyGroup[contains(@Condition, '{0}')]"
				.Display(condition));

			if (node == null)
				return result;

			foreach (XmlNode child in node.ChildNodes)
			{
				result.Add(child.Name, child.InnerText);
			}

			return result;
		}

		/// <summary>
		/// Gets all properties specific to Debug configuration.
		/// </summary>
		public static Dictionary<string, string> GetDebugProperties()
		{
			return GetConditionProperties("Debug");
		}

		/// <summary>
		/// Gets all properties specific to Release configuration.
		/// </summary>
		public static Dictionary<string, string> GetReleaseProperties()
		{
			return GetConditionProperties("Release");
		}

		#endregion

		#region Project references

		/// <summary>
		/// Gets binary references.
		/// </summary>
		public static List<Reference> GetBinaryReferences()
		{
			List<Reference> references = new List<Reference>();

			foreach (XmlNode node in SelectNodes("/ms:Project/ms:ItemGroup/ms:Reference"))
			{
				Reference reference = new Reference();

				Dictionary<string, string> properties = PropertiesHelper.ParseFromXml(node);
				if (properties.ContainsKey("/Reference[@Include]"))
				{
					string include = "Name={0}".Display(properties["/Reference[@Include]"]);
					properties.Remove("/Reference[@Include]");

					ArgumentProperties args = ArgumentProperties.Parse(include.Split(','));
					reference.Name = GetFromArgument(args, "Name");
					reference.Version = GetFromArgument(args, "Version");
					reference.Culture = GetFromArgument(args, "Culture");
					reference.PublicKeyToken = GetFromArgument(args, "PublicKeyToken");
					reference.ProcessorArchitecture = GetFromArgument(args, "processorArchitecture");

					if (args.Keys.Count > 0)
					{
						throw new InvalidOperationException(
							"Unknown reference include properties: {0}."
							.Display(String.Join(", ", args.Keys)));
					}
				}

				reference.SpecificVersion = GetFromProperty(properties, "/Reference/SpecificVersion");
				reference.HintPath = GetFromProperty(properties, "/Reference/HintPath");
				reference.Private = GetFromProperty(properties, "/Reference/Private");
				reference.Aliases = GetFromProperty(properties, "/Reference/Aliases");
				reference.EmbedInteropTypes = GetFromProperty(properties, "/Reference/EmbedInteropTypes");

				if (properties.Keys.Count > 0)
				{
					throw new InvalidOperationException(
						"Unknown reference child properties: {0}."
						.Display(String.Join(", ", properties.Keys)));
				}

				references.Add(reference);
			}

			return references;
		}

		/// <summary>
		/// Gets reference field from an argument.
		/// </summary>
		private static string GetFromArgument(ArgumentProperties args, string key)
		{
			string result = null;
			if (args.Contains(key))
			{
				result = args.GetValue(key);
				args.Remove(key);
			}

			return result;
		}

		/// <summary>
		/// Gets reference field from a property.
		/// </summary>
		private static string GetFromProperty(IDictionary<string, string> properties, string key)
		{
			string result = null;
			if (properties.ContainsKey(key))
			{
				result = properties[key];
				properties.Remove(key);
			}

			return result;
		}

		/// <summary>
		/// Gets project references.
		/// </summary>
		public static List<string> GetProjectReferences()
		{
			List<string> references = new List<string>();

			foreach (XmlNode node in SelectNodes("/ms:Project/ms:ItemGroup/ms:ProjectReference"))
			{
				string reference = Resources.Unknown;
				if (node.Attributes != null)
				{
					XmlAttribute attr = node.Attributes["Include"];
					if (attr != null)
					{
						reference = attr.Value;
					}
				}

				references.Add(reference);
			}

			return references;
		}

		#endregion
	}
}
