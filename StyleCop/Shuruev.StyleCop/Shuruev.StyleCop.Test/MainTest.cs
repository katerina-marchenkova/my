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
	public class MainTest
	{
		private readonly string m_basePath;
		private readonly string m_currentFilename;
		private readonly List<string> m_currentViolations;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public MainTest()
		{
			m_basePath = AppDomain.CurrentDomain.BaseDirectory;
			m_currentFilename = Path.Combine(m_basePath, "Test.cs");
			m_currentViolations = new List<string>();
		}

		#region Extended original rules

		/// <summary>
		/// Tests SP1300 rule.
		/// </summary>
		[TestMethod]
		public void SP1300()
		{
			TestCollection(Resources.SP1300, "SP1300");
		}

		/// <summary>
		/// Tests SP1509 rule.
		/// </summary>
		[TestMethod]
		public void SP1509()
		{
			TestCollection(Resources.SP1509, "SP1509");
		}

		/// <summary>
		/// Tests SP1600 rule.
		/// </summary>
		[TestMethod]
		public void SP1600()
		{
			TestCollection(Resources.SP1600, "SP1600");
		}

		#endregion

		#region More custom rules

		/// <summary>
		/// Tests SP2000 rule.
		/// </summary>
		[TestMethod]
		public void SP2000()
		{
			TestCollection(Resources.SP2000, "SP2000");
		}

		/// <summary>
		/// Tests SP2001 rule.
		/// </summary>
		[TestMethod]
		public void SP2001()
		{
			TestCollection(Resources.SP2001, "SP2001");
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

						PerformAnalysis(sourceCode);
						Assert.AreEqual(
							expected,
							m_currentViolations.Contains(checkId),
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
			if (File.Exists(m_currentFilename))
			{
				File.Delete(m_currentFilename);
			}

			File.WriteAllText(m_currentFilename, sourceCode);

			m_currentViolations.Clear();

			StyleCopConsole console = new StyleCopConsole(
				null,
				false,
				null,
				new List<string>(new[] { m_basePath }),
				true);

			Configuration configuration = new Configuration(null);
			CodeProject project = new CodeProject(0, m_basePath, configuration);

			console.Core.Environment.AddSourceCode(project, m_currentFilename, null);

			List<CodeProject> projects = new List<CodeProject>();
			projects.Add(project);

			console.ViolationEncountered += OnViolationEncountered;
			console.Start(projects, true);
			console.ViolationEncountered -= OnViolationEncountered;
			console.Dispose();

			if (File.Exists(m_currentFilename))
			{
				File.Delete(m_currentFilename);
			}
		}

		/// <summary>
		/// Handles encountered violations.
		/// </summary>
		private void OnViolationEncountered(object sender, ViolationEventArgs e)
		{
			m_currentViolations.Add(e.Violation.Rule.CheckId);
		}

		#endregion
	}
}
