using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.StyleCop.Run;
using Shuruev.StyleCop.Test.Properties;

namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Testing StyleCop+ plug-in.
	/// </summary>
	[TestClass]
	public class StyleCopPlusTest
	{
		private readonly string m_tempFileName;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public StyleCopPlusTest()
		{
			m_tempFileName = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				"Test.cs");
		}

		/// <summary>
		/// Tests rules from StyleCop+ plug-in.
		/// </summary>
		[TestMethod]
		public void StyleCopPlus()
		{
			List<BlockItem> blocks = ParseBlocks(Resources.StyleCopPlusTest);
			foreach (BlockItem block in blocks)
			{
				List<TestItem> tests = ParseTests(block.Content);
				foreach (TestItem test in tests)
				{
					RunTest(
						block.Rule,
						test.ErrorCount,
						test.Description,
						test.SourceCode);
				}
			}
		}

		#region Reading test definition file

		/// <summary>
		/// Reads a list of blocks.
		/// </summary>
		private static List<BlockItem> ParseBlocks(string allText)
		{
			List<BlockItem> blocks = new List<BlockItem>();

			StringBuilder sb = new StringBuilder();
			string rule = null;

			string[] lines = allText.Split(new[] { "\r\n" }, StringSplitOptions.None);

			foreach (string line in lines)
			{
				if (line.StartsWith("#region "))
				{
					rule = line.Substring(8);
					if (String.IsNullOrEmpty(rule))
						ThrowWrongTestFile();

					continue;
				}

				if (line.StartsWith("#endregion"))
				{
					BlockItem block = new BlockItem
					{
						Rule = rule,
						Content = sb.ToString()
					};
					blocks.Add(block);

					rule = null;
					sb.Length = 0;
					continue;
				}

				sb.AppendLine(line);
			}

			return blocks;
		}

		/// <summary>
		/// Reads a list of tests.
		/// </summary>
		private static List<TestItem> ParseTests(string blockText)
		{
			List<TestItem> tests = new List<TestItem>();

			int index = 0;
			while (index < blockText.Length)
			{
				int start = blockText.IndexOf("//# [", index);
				if (start < 0)
					break;

				int end = blockText.IndexOf("//# [END]", start);
				if (end < 0)
					break;

				string testText = blockText.Substring(start, end - start);
				TestItem test = ParseTest(testText);
				tests.Add(test);

				index = end + 1;
			}

			return tests;
		}

		/// <summary>
		/// Reads a test.
		/// </summary>
		private static TestItem ParseTest(string testText)
		{
			TestItem test = new TestItem();

			List<string> lines = new List<string>(
				testText.Split(
					new[] { "\r\n" },
					StringSplitOptions.None));

			if (lines.Count < 3)
				ThrowWrongTestFile();

			string headerLine = lines[0];
			if (!headerLine.StartsWith("//# ["))
				ThrowWrongTestFile();

			if (headerLine == "//# [OK]")
			{
				test.ErrorCount = 0;
			}
			else
			{
				if (headerLine == "//# [ERROR]")
				{
					test.ErrorCount = 1;
				}
				else
				{
					if (!headerLine.StartsWith("//# [ERROR:"))
						ThrowWrongTestFile();

					string[] parts = headerLine.Split(
						new[] { "//# [ERROR:", "]" },
						StringSplitOptions.RemoveEmptyEntries);

					if (parts.Length != 1)
						ThrowWrongTestFile();

					if (!Int32.TryParse(parts[0], out test.ErrorCount))
						ThrowWrongTestFile();
				}
			}

			string descriptionLine = lines[1];
			if (!descriptionLine.StartsWith("//# "))
				ThrowWrongTestFile();

			test.Description = descriptionLine.Substring(4);
			if (String.IsNullOrEmpty(test.Description))
				ThrowWrongTestFile();

			lines.RemoveAt(0);
			lines.RemoveAt(0);
			test.SourceCode = String.Join("\r\n", lines.ToArray());

			return test;
		}

		/// <summary>
		/// Throws exception about wrong test definition file.
		/// </summary>
		private static void ThrowWrongTestFile()
		{
			throw new InvalidDataException("Test definition file appears to have wrong format.");
		}

		#endregion

		#region Running tests

		/// <summary>
		/// Runs specified test.
		/// </summary>
		private void RunTest(
			string targetRule,
			int errorCount,
			string description,
			string sourceCode)
		{
			if (File.Exists(m_tempFileName))
			{
				File.Delete(m_tempFileName);
			}

			File.WriteAllText(m_tempFileName, sourceCode);

			StyleCopPlusRunner runner = new StyleCopPlusRunner();
			runner.Run(m_tempFileName, targetRule);

			string message = String.Format(
				"{0}: {1}",
				targetRule, description);

			Assert.AreEqual(
				errorCount,
				runner.Violations.Count,
				message);

			for (int i = 0; i < errorCount; i++)
			{
				runner.Violations.Remove(targetRule);
			}

			Assert.AreEqual(
				0,
				runner.Violations.Count,
				message);
		}

		#endregion
	}
}
