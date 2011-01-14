using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;
using Shuruev.StyleCop.CSharp;

namespace Shuruev.StyleCop.Run
{
	/// <summary>
	/// Running StyleCop environment.
	/// </summary>
	public class Program
	{
		private static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
		private static readonly string sourceFile = @"C:\Users\Public\GIT\GitHub\My\StyleCop\Shuruev.StyleCop\Shuruev.StyleCop.Run\Class1.cs";

		/// <summary>
		/// Main program entry.
		/// </summary>
		public static void Main(string[] args)
		{
			StyleCopConsole console = new StyleCopConsole(
				null,
				false,
				null,
				new List<string>(new[] { basePath }),
				true);

			/*xxxStyleCopPlus styleCopPlus = null;
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

			if (styleCopPlus == null)
			{
				throw new InvalidOperationException("StyleCopPlus was not found.");
			}*/

			/*xxxstyleCopPlus.DisableAllRulesExcept.Clear();
			styleCopPlus.DisableAllRulesExcept.Add("AdvancedNamingRules");*/

			CodeProject project = new CodeProject(
				0,
				basePath,
				new Configuration(null));

			console.Core.Environment.AddSourceCode(project, sourceFile, null);

			console.OutputGenerated += OnOutputGenerated;
			console.ViolationEncountered += OnViolationEncountered;
			console.Start(new[] { project }, true);

			foreach (SourceParser parser in console.Core.Parsers)
			{
				Console.WriteLine("Parser: {0}", parser.Name);
				foreach (SourceAnalyzer analyzer in parser.Analyzers)
				{
					Console.WriteLine("Analyzer: {0}", analyzer.Name);
				}
			}

			console.OutputGenerated -= OnOutputGenerated;
			console.ViolationEncountered -= OnViolationEncountered;
			console.Dispose();

			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}

		/// <summary>
		/// Handles generated output.
		/// </summary>
		private static void OnOutputGenerated(object sender, OutputEventArgs e)
		{
			Console.WriteLine(e.Output);
		}

		/// <summary>
		/// Handles encountered violations.
		/// </summary>
		private static void OnViolationEncountered(object sender, ViolationEventArgs e)
		{
			Console.WriteLine("{0} ({1}): {2}", e.Violation.Rule.CheckId, e.LineNumber, e.Message);
		}
	}
}
