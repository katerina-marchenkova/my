using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.StyleCop;

namespace Shuruev.StyleCop.Run
{
	/// <summary>
	/// Running StyleCop environment.
	/// </summary>
	public class Program
	{
		private static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
		private static readonly string sourceFile = @"C:\Users\oshuruev\Documents\Visual Studio 2010\Projects\Microsoft.StyleCop\ConsoleApplication1\Program.cs";

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

			Configuration configuration = new Configuration(null);
			CodeProject project = new CodeProject(0, basePath, configuration);

			console.Core.Environment.AddSourceCode(project, sourceFile, null);

			List<CodeProject> projects = new List<CodeProject>();
			projects.Add(project);

			console.OutputGenerated += OnOutputGenerated;
			console.ViolationEncountered += OnViolationEncountered;
			console.Start(projects, true);
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

		/// <summary>
		/// Test method.
		/// </summary>
		private static void Test1()
		{
			bool flag = (bool)typeof(Uri).InvokeMember(null, BindingFlags.Default, null, null, null);
		}
	}
}
