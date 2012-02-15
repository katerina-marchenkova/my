using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using EnvDTE;
using EnvDTE80;
using SolutionHelper.Properties;
using VSLangProj;

namespace SolutionHelper
{
	/// <summary>
	/// Researches and modifies active projects.
	/// </summary>
	public class ProjectAdapter
	{
		private Solution m_solution;
		private Dictionary<string, Project> m_projects;
		private Dictionary<string, List<Reference>> m_references;
		private List<string> m_referencedAssemblies;

		#region Researching projects

		/// <summary>
		/// Researches project collection.
		/// </summary>
		private void Initialize(DTE2 applicationObject)
		{
			m_solution = applicationObject.Solution;

			m_projects = new Dictionary<string, Project>();
			m_references = new Dictionary<string, List<Reference>>();
			m_referencedAssemblies = new List<string>();

			foreach (Project project in m_solution.Projects)
			{
				if (project.ConfigurationManager == null)
					continue;

				if (project.Kind.ToLowerInvariant() == "{54435603-dbb4-11d2-8724-00a0c9a8b90c}")
					continue;

				if (project.Kind.ToLowerInvariant() == "{cc5fd16d-436d-48ad-a40c-5a424c6e3e79}")
					continue;

				string assemblyName = project.GetAssemblyName();
				if (m_projects.ContainsKey(assemblyName))
				{
					throw new InvalidOperationException(
						String.Format(
							Resources.SharedAssemblyName,
							m_projects[assemblyName].Name,
							project.Name,
							assemblyName));
				}

				m_projects.Add(assemblyName, project);
				m_references.Add(assemblyName, new List<Reference>());
			}

			foreach (Project project in m_projects.Values)
			{
				string assemblyName = project.GetAssemblyName();

				foreach (Reference reference in project.GetReferences())
				{
					if (reference.SourceProject != null)
					{
						throw new InvalidOperationException(
							String.Format(
								Resources.DontUseProjectReferences,
								project.Name,
								reference.Name));
					}

					if (!m_projects.ContainsKey(reference.Name))
					{
						if (!m_referencedAssemblies.Contains(reference.Name))
						{
							m_referencedAssemblies.Add(reference.Name);
						}

						continue;
					}

					m_references[assemblyName].Add(reference);
				}
			}
		}

		#endregion

		#region Modifying projects

		/// <summary>
		/// Analyzes and adjusts all solution projects.
		/// </summary>
		public void Adjust(DTE2 applicationObject, string baseReferencePaths)
		{
			Initialize(applicationObject);

			foreach (string assemblyName in m_projects.Keys)
			{
				SetupReferencePaths(assemblyName, baseReferencePaths);
				SetupDependencies(assemblyName);
				ClearHintPaths(assemblyName);
			}
		}

		/// <summary>
		/// Downloads the latest references.
		/// </summary>
		public void DownloadLatestReferences(
			DTE2 applicationObject,
			string internalRefPath,
			string externalRefPath,
			string internalRefTargetPath,
			string externalRefTargetPath)
		{
			Initialize(applicationObject);

			IDictionary<string, string> internalAssemblyPaths =
				GetDownloadPathsForInternalAssemblies(
				internalRefPath,
				m_referencedAssemblies);

			IDictionary<string, string> externalAssemblyPaths =
				GetDownloadPathsForExternalAssemblies(
				externalRefPath,
				m_referencedAssemblies);

			PerformDownload(
				internalRefPath,
				externalRefPath,
				internalAssemblyPaths.Values,
				externalAssemblyPaths.Values,
				internalRefTargetPath,
				externalRefTargetPath);
		}

		/// <summary>
		/// Setups reference paths for specified project.
		/// </summary>
		private void SetupReferencePaths(string assemblyName, string baseReferencePaths)
		{
			Project project = m_projects[assemblyName];

			List<string> paths = new List<string>();
			paths.AddRange(
				m_references[assemblyName]
				.Select(reference => m_projects[reference.Name])
				.Select(refProj => refProj.GetDebugOutputPath()));

			paths.Add(baseReferencePaths);

			Property property = project.Properties.Item("ReferencePath");
			property.Value = String.Join(";", paths.ToArray());
		}

		/// <summary>
		/// Setups hint paths for specified project.
		/// </summary>
		private void ClearHintPaths(string assemblyName)
		{
			Project project = m_projects[assemblyName];
			project.Save();
			string text = File.ReadAllText(project.FileName);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text);

			XmlNamespaceManager xnm = new XmlNamespaceManager(doc.NameTable);
			xnm.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");

			bool changed = false;
			foreach (XmlNode node in doc.SelectNodes("/ms:Project/ms:ItemGroup/ms:Reference/ms:HintPath", xnm))
			{
				node.ParentNode.RemoveChild(node);
				changed = true;
			}

			if (!changed)
				return;

			// raise source control check-out notification
			ProjectItem temp = project.ProjectItems.AddFolder("Temp");
			temp.Remove();
			Directory.Delete(temp.FileNames[0]);
			project.Save();

			using (XmlTextWriter xtw = new XmlTextWriter(project.FileName, Encoding.UTF8))
			{
				xtw.Formatting = Formatting.Indented;
				doc.WriteTo(xtw);
			}
		}

		/// <summary>
		/// Setups dependencies for specified project.
		/// </summary>
		private void SetupDependencies(string assemblyName)
		{
			Project project = m_projects[assemblyName];
			BuildDependency dependency = m_solution.SolutionBuild.BuildDependencies.Item(project);

			bool same = Enumerable.SequenceEqual(
				(dependency.RequiredProjects == null ? new object[] { } : (object[])dependency.RequiredProjects)
					.Cast<Project>()
					.Select(proj => proj.UniqueName)
					.OrderBy(name => name),
				m_references[assemblyName]
					.Select(reference => m_projects[reference.Name].UniqueName)
					.OrderBy(name => name));

			if (same)
				return;

			dependency.RemoveAllProjects();
			foreach (Reference reference in m_references[assemblyName])
			{
				Project refProj = m_projects[reference.Name];
				dependency.AddProject(refProj.UniqueName);
			}
		}

		#endregion

		#region Download Latest References

		/// <summary>
		/// Performs downloading referenced libraries from server.
		/// </summary>
		private void PerformDownload(
			string internalSourcePath,
			string externalSourcePath,
			IEnumerable<string> internalAssemblyPaths,
			IEnumerable<string> externalAssemblyPaths,
			string internalTargetPath,
			string externalTargetPath)
		{

			ProcessStartInfo si = new ProcessStartInfo("cmd.exe");

			StringBuilder commandString = new StringBuilder();
			commandString.Append(" /c ");
			commandString.Append(" @ECHO OFF ");

			string internalReferencesDownloadCommand =
				CreateAssemblyDownloadCommand(internalTargetPath, internalAssemblyPaths.ToArray());
			string externalReferencesDownloadCommand =
				CreateAssemblyDownloadCommand(externalTargetPath, externalAssemblyPaths.ToArray());

			if (!string.IsNullOrEmpty(internalReferencesDownloadCommand))
			{
				AppendDownloadCommand(commandString, internalReferencesDownloadCommand, internalSourcePath);
			}

			if (!string.IsNullOrEmpty(externalReferencesDownloadCommand))
			{
				AppendDownloadCommand(commandString, externalReferencesDownloadCommand, externalSourcePath);
			}

			si.Arguments = commandString.ToString();

			System.Diagnostics.Process console =
				System.Diagnostics.Process.Start(si);

			console.WaitForExit();
		}

		/// <summary>
		/// Appends the download references command.
		/// </summary>
		private void AppendDownloadCommand(
			StringBuilder commandString,
			string internalReferencesDownloadCommand,
			string internalSourcePath)
		{
			if (!string.IsNullOrEmpty(internalReferencesDownloadCommand))
			{
				commandString.AppendFormat(" & ( PUSHD {0} ", internalSourcePath);
				commandString.AppendFormat(" & {0} ", internalReferencesDownloadCommand);
				commandString.AppendFormat(" & POPD ) ");
			}
		}

		/// <summary>
		/// Gets the download paths for internal assemblies.
		/// </summary>
		private static IDictionary<string, string> GetDownloadPathsForInternalAssemblies(
			string sourceInternalPath,
			List<string> assemblyNames)
		{
			Dictionary<string, string> internalAssemblyPaths = new Dictionary<string, string>();

			DirectoryInfo dirInfo = new DirectoryInfo(sourceInternalPath);
			DirectoryInfo[] subDirInfos = dirInfo.GetDirectories();
			foreach (DirectoryInfo subdirectory in subDirInfos)
			{
				if (assemblyNames.Contains(subdirectory.Name))
				{
					internalAssemblyPaths[subdirectory.Name] = subdirectory.FullName;
				}
			}

			return internalAssemblyPaths;
		}

		/// <summary>
		/// Gets the download paths for external assemblies.
		/// </summary>
		private static IDictionary<string, string> GetDownloadPathsForExternalAssemblies(
			string sourceExternalPath,
			List<string> assemblyNames)
		{
			Dictionary<string, string> externalAssemblyPaths = new Dictionary<string, string>();

			DirectoryInfo dirInfo = new DirectoryInfo(sourceExternalPath);
			DirectoryInfo[] subDirInfos = dirInfo.GetDirectories();
			foreach (DirectoryInfo subdirectory in subDirInfos)
			{
				FileInfo[] libraries = subdirectory.GetFiles("*.dll", SearchOption.AllDirectories);
				foreach (FileInfo fileInfo in libraries)
				{
					string fileAssemblyName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
					if (assemblyNames.Contains(fileAssemblyName))
					{
						externalAssemblyPaths[fileAssemblyName] = subdirectory.FullName;
						break;
					}
				}
			}

			return externalAssemblyPaths;
		}

		/// <summary>
		/// Creates the download command for assemblies.
		/// </summary>
		private static string CreateAssemblyDownloadCommand(string targetPath, string[] assemblyPaths)
		{
			if (assemblyPaths.Length == 0)
			{
				return string.Empty;
			}

			string conjunction = string.Join(" ", assemblyPaths);
			return string.Format("FOR /D %G IN ({0}) DO XCOPY \"%G\\Latest\" \"{1}\" /R /S /Y", conjunction, targetPath);
		}

		#endregion
	}
}
