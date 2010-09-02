using System;
using Microsoft.StyleCop;

namespace ConsoleApplication1
{
	/// <summary>
	/// Simple example for running StyleCop environment.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Main program entry.
		/// </summary>
		public static void Main(string[] args)
		{
			string projectPath = @"C:\Users\Public\GIT\My\StyleCop\Source\4.4.0.14\ConsoleApplication1";
			string filePath = @"C:\Users\Public\GIT\My\StyleCop\Source\4.4.0.14\ConsoleApplication1\Class1.cs";

			StyleCopConsole console = new StyleCopConsole(null, false, null, null, true);
			CodeProject project = new CodeProject(0, projectPath, new Configuration(null));

			console.Core.Environment.AddSourceCode(project, filePath, null);

			console.OutputGenerated += OnOutputGenerated;
			console.ViolationEncountered += OnViolationEncountered;
			console.Start(new[] { project }, true);
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
			Console.WriteLine("{0}: {1}", e.Violation.Rule.CheckId, e.Message);
		}
	}
}
