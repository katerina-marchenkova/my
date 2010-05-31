using System;
using System.Collections.Generic;
using Microsoft.StyleCop;
using Shuruev.StyleCop.CSharp;

namespace Shuruev.StyleCop.Run
{
	/// <summary>
	/// Runs StyleCop+ with specified settings.
	/// </summary>
	public class StyleCopPlusRunner
	{
		public List<string> Violations { get; set; }

		/// <summary>
		/// Runs StyleCop+ for specified file.
		/// </summary>
		public void Run(string sourceFile, string targetRule)
		{
			Violations = new List<string>();

			string basePath = AppDomain.CurrentDomain.BaseDirectory;

			StyleCopConsole console = new StyleCopConsole(
				null,
				false,
				null,
				new List<string>(new[] { basePath }),
				true);

			StyleCopPlus styleCopPlus = ExtractStyleCopPlus(console);
			if (styleCopPlus == null)
			{
				throw new InvalidOperationException("StyleCopPlus was not found.");
			}

			styleCopPlus.DisableAllRulesExcept.Clear();
			styleCopPlus.DisableAllRulesExcept.Add(targetRule);

			CodeProject project = new CodeProject(
				0,
				basePath,
				new Configuration(null));

			console.Core.Environment.AddSourceCode(project, sourceFile, null);

			console.ViolationEncountered += OnViolationEncountered;
			console.Start(new[] { project }, true);
			console.ViolationEncountered -= OnViolationEncountered;
			console.Dispose();
		}

		/// <summary>
		/// Handles encountered violations.
		/// </summary>
		private void OnViolationEncountered(object sender, ViolationEventArgs e)
		{
			Violations.Add(e.Violation.Rule.Name);
		}

		/// <summary>
		/// Finds StyleCop+ analyzer and removes all other analyzers.
		/// </summary>
		private static StyleCopPlus ExtractStyleCopPlus(StyleCopConsole console)
		{
			StyleCopPlus styleCopPlus = null;
			foreach (SourceParser parser in console.Core.Parsers)
			{
				List<SourceAnalyzer> analyzersToRemove = new List<SourceAnalyzer>();
				foreach (SourceAnalyzer analyzer in parser.Analyzers)
				{
					if (analyzer.GetType() == typeof(StyleCopPlus))
					{
						styleCopPlus = (StyleCopPlus)analyzer;
						break;
					}

					analyzersToRemove.Add(analyzer);
				}

				foreach (SourceAnalyzer analyzer in analyzersToRemove)
				{
					parser.Analyzers.Remove(analyzer);
				}
			}

			return styleCopPlus;
		}
	}
}
