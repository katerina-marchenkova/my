using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.StyleCop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.StyleCop.Test.Properties;

namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Testing StyleCop extensions.
	/// </summary>
	[TestClass]
	public class UnitTest1
	{
		private readonly string basePath;
		private readonly string currentFilename;
		private readonly List<string> currentViolations;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitTest1"/> class.
		/// </summary>
		public UnitTest1()
		{
			this.basePath = AppDomain.CurrentDomain.BaseDirectory;
			this.currentFilename = Path.Combine(this.basePath, "Test.cs");
			this.currentViolations = new List<string>();
		}

		/// <summary>
		/// Tests EA1600 rule.
		/// </summary>
		[TestMethod]
		public void EA1600()
		{
			TestCollection(Resources.EA1600, "EA1600");
		}

		#region Running tests for custom source code

		/// <summary>
		/// Test a collection of source code examples.
		/// </summary>
		private void TestCollection(string sourceCollection, string checkId)
		{
			string[] lines = sourceCollection.Split(new[] { "\r\n" }, StringSplitOptions.None);
			bool expected = false;

			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
			{
				if (line.StartsWith("//")
					&& line.EndsWith("///////////////////////////////////////////////////"))
				{
					if (sb.Length > 0)
					{
						string sourceCode = sb.ToString();
						sourceCode = sourceCode.Remove(sourceCode.Length - 2, 2);

						this.PerformAnalysis(sourceCode);
						Assert.AreEqual(expected, this.currentViolations.Contains(checkId));

						sb.Length = 0;
					}

					string result = line.Trim('/', ' ');
					expected = result == "ERROR";
					continue;
				}

				sb.AppendLine(line);
			}
		}

		/// <summary>
		/// Performs StyleCop analysis for specified source code.
		/// </summary>
		private void PerformAnalysis(string sourceCode)
		{
			if (File.Exists(this.currentFilename))
			{
				File.Delete(this.currentFilename);
			}

			File.WriteAllText(this.currentFilename, sourceCode);

			this.currentViolations.Clear();

			StyleCopConsole console = new StyleCopConsole(
				null,
				false,
				null,
				new List<string>(new[] { this.basePath }),
				true);

			Configuration configuration = new Configuration(null);
			CodeProject project = new CodeProject(0, this.basePath, configuration);

			console.Core.Environment.AddSourceCode(project, this.currentFilename, null);

			List<CodeProject> projects = new List<CodeProject>();
			projects.Add(project);

			console.ViolationEncountered += this.OnViolationEncountered;
			console.Start(projects, true);
			console.ViolationEncountered -= this.OnViolationEncountered;
			console.Dispose();

			if (File.Exists(this.currentFilename))
			{
				File.Delete(this.currentFilename);
			}
		}

		/// <summary>
		/// Handles encountered violations.
		/// </summary>
		private void OnViolationEncountered(object sender, ViolationEventArgs e)
		{
			this.currentViolations.Add(e.Violation.Rule.CheckId);
		}

		#endregion
	}
}
