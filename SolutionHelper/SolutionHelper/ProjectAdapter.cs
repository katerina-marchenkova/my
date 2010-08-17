using System;
using System.Collections.Generic;
using System.Linq;
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

		#region Researching projects

		/// <summary>
		/// Researches project collection.
		/// </summary>
		private void Initialize(DTE2 applicationObject)
		{
			m_solution = applicationObject.Solution;

			m_projects = new Dictionary<string, Project>();
			m_references = new Dictionary<string, List<Reference>>();

			foreach (Project project in m_solution.Projects)
			{
				if (project.ConfigurationManager == null)
					continue;

				if (project.Kind == "{54435603-DBB4-11D2-8724-00A0C9A8B90C}")
					continue;

				string assemblyName = project.GetAssemblyName();

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
						continue;

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
			}
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
		/// Setups dependencies for specified project.
		/// </summary>
		private void SetupDependencies(string assemblyName)
		{
			Project project = m_projects[assemblyName];
			BuildDependency dependency = m_solution.SolutionBuild.BuildDependencies.Item(project);
			dependency.RemoveAllProjects();
			foreach (Reference reference in m_references[assemblyName])
			{
				Project refProj = m_projects[reference.Name];
				dependency.AddProject(refProj.UniqueName);
			}
		}

		#endregion
	}
}
