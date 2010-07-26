using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CCNet.Common;
using CCNet.ProjectAdapter.Properties;

namespace CCNet.ProjectAdapter
{
	/// <summary>
	/// Updates project during build process.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Main program.
		/// </summary>
		public static void Main(string[] args)
		{
			/*xxxargs = new[]
			{
				@"ProjectName=VortexCommander",
				@"CurrentVersion=1.2.3",
				@"ReferencesDirectory=\\rufrt-vxbuild\d$\VSS\CCNET\VortexCommander\References",
				@"WorkingDirectorySource=\\rufrt-vxbuild\d$\VSS\CCNET\VortexCommander\WorkingDirectory\Source",
				@"ExternalReferencesPath=\\rufrt-vxbuild\ExternalReferences",
				@"InternalReferencesPath=\\rufrt-vxbuild\InternalReferences"
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return;
			}

			Arguments.Default = ArgumentProperties.Parse(args);

			UpdateAssemblyInfo();
			UpdatePublishVersions();
			UpdateReferences();
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}

		#region Performing update

		/// <summary>
		/// Updates assembly information file.
		/// </summary>
		private static void UpdateAssemblyInfo()
		{
			string text = File.ReadAllText(Paths.AssemblyInfoFile);

			Regex regex = new Regex("\\[assembly: AssemblyVersion\\(\"[0-9\\.\\*]+\"\\)]");
			text = regex.Replace(text, "[assembly: AssemblyVersion(\"" + Arguments.CurrentVersion + "\")]");

			File.WriteAllText(Paths.AssemblyInfoFile, text, Encoding.UTF8);

			Console.WriteLine(
				Resources.UpdateAssemblyInfoDone,
				Paths.AssemblyInfoFile,
				Arguments.CurrentVersion);
		}

		/// <summary>
		/// Updates publish versions.
		/// </summary>
		private static void UpdatePublishVersions()
		{
			string text = File.ReadAllText(Paths.ProjectFile);

			Regex regex = new Regex("<MinimumRequiredVersion>[0-9\\.\\*]+</MinimumRequiredVersion>");
			text = regex.Replace(text, "<MinimumRequiredVersion>" + Arguments.CurrentVersion + "</MinimumRequiredVersion>");

			regex = new Regex("<ApplicationVersion>[0-9\\.\\*]+</ApplicationVersion>");
			text = regex.Replace(text, "<ApplicationVersion>" + Arguments.CurrentVersion + "</ApplicationVersion>");

			File.WriteAllText(Paths.ProjectFile, text, Encoding.UTF8);

			Console.WriteLine(
				Resources.UpdatePublishVersionsDone,
				Paths.ProjectFile,
				Arguments.CurrentVersion);
		}

		/// <summary>
		/// Updates hint paths for references.
		/// </summary>
		private static void UpdateReferences()
		{
			string text = File.ReadAllText(Paths.ProjectFile);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text);

			XmlNamespaceManager xnm = new XmlNamespaceManager(doc.NameTable);
			xnm.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");

			List<ReferenceFile> allExternals = ReferenceFolder.GetAllFiles(Arguments.ExternalReferencesPath);
			List<ReferenceFile> allInternals = ReferenceFolder.GetAllFiles(Arguments.InternalReferencesPath);

			UpdateHints(doc, xnm, allExternals);
			UpdateHints(doc, xnm, allInternals);

			using (XmlTextWriter xtw = new XmlTextWriter(Paths.ProjectFile, Encoding.UTF8))
			{
				xtw.Formatting = Formatting.Indented;
				doc.WriteTo(xtw);
			}
		}

		/// <summary>
		/// Updates hint paths for resolved references.
		/// </summary>
		private static void UpdateHints(
			XmlDocument doc,
			XmlNamespaceManager xnm,
			IEnumerable<ReferenceFile> references)
		{
			foreach (ReferenceFile reference in references)
			{
				XmlNode node = doc.SelectSingleNode(
					"/ms:Project/ms:ItemGroup/ms:Reference[starts-with(@Include, '{0},')]"
					.Display(reference.AssemblyName),
					xnm);

				if (node == null)
					continue;

				XmlNode hint = node.SelectSingleNode("ms:HintPath", xnm);
				if (hint != null)
					node.RemoveChild(hint);

				hint = doc.CreateElement("HintPath", xnm.LookupNamespace("ms"));
				hint.InnerXml = reference.FilePath;

				node.AppendChild(hint);

				Console.WriteLine(
					Resources.UpdateReferencesDone,
					reference.AssemblyName,
					reference.FilePath);
			}
		}

		#endregion
	}
}
