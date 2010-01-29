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

		#region Extended original rules

		/// <summary>
		/// Tests EX1300 rule.
		/// </summary>
		[TestMethod]
		public void EX1300()
		{
			TestCollection(Resources.EX1300, "EX1300");
		}

		/// <summary>
		/// Tests EX1600 rule.
		/// </summary>
		[TestMethod]
		public void EX1600()
		{
			TestCollection(Resources.EX1600, "EX1600");
		}

		#endregion

		#region More custom rules

		/// <summary>
		/// Tests CR2000 rule.
		/// </summary>
		[TestMethod]
		public void CR2000()
		{
			TestCollection(Resources.CR2000, "CR2000");
		}

		/// <summary>
		/// Tests CR2001 rule.
		/// </summary>
		[TestMethod]
		public void CR2001()
		{
			TestCollection(Resources.CR2001, "CR2001");
		}

		#endregion

		#region Running tests for custom source code

		/// <summary>
		/// Test a collection of source code examples.
		/// </summary>
		private void TestCollection(string sourceCollection, string checkId)
		{
			string[] lines = sourceCollection.Split(new[] { "\r\n" }, StringSplitOptions.None);
			bool expected = false;
			string message = null;

			if (lines.Length == 0)
				throw new InvalidDataException("Test definition file doesn't contain any data.");

			string firstLine = lines[0];
			if (!firstLine.StartsWith("//# "))
				throw new InvalidDataException("Test definition file has wrong format.");

			string lastLine = lines[lines.Length - 1];
			if (lastLine != "//# [END OF FILE]")
				throw new InvalidDataException("The last line in test definition file doesn't contain valid EOF sign.");

			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
			{
				if (line.StartsWith("//# "))
				{
					if (sb.Length > 0)
					{
						if (String.IsNullOrEmpty(message))
							throw new InvalidDataException("Every test in file should contain describing message.");

						string sourceCode = sb.ToString();
						sourceCode = sourceCode.Remove(sourceCode.Length - 2, 2);

						this.PerformAnalysis(sourceCode);
						Assert.AreEqual(
							expected,
							this.currentViolations.Contains(checkId),
							String.Format(
								"Failed test description is: {0}",
								message));

						sb.Length = 0;
						expected = false;
						message = null;
					}

					if (line == "//# [END OF FILE]")
						return;

					if (line.StartsWith("//# ["))
					{
						string result = line.Trim('/', '#', ' ', '[', ']');
						if (result == "ERROR")
						{
							expected = true;
						}
						else if (result == "OK")
						{
							expected = false;
						}
						else
						{
							throw new InvalidDataException(
								String.Format(
									"{0} is wrong string for expected result.",
									result));
						}
					}
					else
					{
						string text = line.Trim('/', '#', ' ');
						if (String.IsNullOrEmpty(text))
							throw new InvalidDataException("Describing message can not be empty.");

						message = text;
					}

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
