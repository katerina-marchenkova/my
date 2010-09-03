using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.StyleCop;
using Microsoft.StyleCop.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.StyleCop.CSharp;

namespace Shuruev.StyleCop.Test.CodeHelperTests
{
	/// <summary>
	/// Testing code helper work.
	/// </summary>
	[TestClass]
	public class CodeHelperTest
	{
		#region Service methods

		/// <summary>
		/// Builds code document from specified source code.
		/// </summary>
		private static CsDocument BuildCodeDocument(string sourceCode)
		{
			string tempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CodeHelperTest.cs");
			File.WriteAllText(tempFile, sourceCode);

			CodeProject project = new CodeProject(0, string.Empty, new Configuration(null));
			CsParser parser = new CsParser();
			CodeFile file = new CodeFile(tempFile, project, parser);

			CodeDocument doc = null;
			parser.ParseFile(file, 0, ref doc);

			File.Delete(tempFile);

			return (CsDocument)doc;
		}

		/// <summary>
		/// Gets element by name.
		/// </summary>
		private static CsElement GetElementByName(CsDocument document, string name)
		{
			return GetElementByName(document.RootElement.ChildElements, name);
		}

		/// <summary>
		/// Gets element by name.
		/// </summary>
		private static CsElement GetElementByName(IEnumerable<CsElement> elements, string name)
		{
			foreach (CsElement element in elements)
			{
				if (element.Name == name)
					return element;

				CsElement result = GetElementByName(element.ChildElements, name);
				if (result != null)
					return result;
			}

			return null;
		}

		/// <summary>
		/// Checks specified local declaration.
		/// </summary>
		private static void AssertLocalDeclaration(
			string expectedName,
			bool expectedIsConstant,
			int expectedLineNumber,
			LocalDeclarationItem declaration)
		{
			Assert.AreEqual(expectedName, declaration.Name);
			Assert.AreEqual(expectedIsConstant, declaration.IsConstant);

			if (!StyleCop43Compatibility.IsStyleCop43())
			{
				Assert.IsTrue(declaration.LineNumber.HasValue);
				Assert.AreEqual(expectedLineNumber, declaration.LineNumber);
			}
		}

		/// <summary>
		/// Checks specified labels.
		/// </summary>
		private static void AssertLabel(
			string expectedName,
			int expectedLineNumber,
			LabelItem label)
		{
			Assert.AreEqual(expectedName, label.Name);
			Assert.IsTrue(label.LineNumber.HasValue);
			Assert.AreEqual(expectedLineNumber, label.LineNumber);
		}

		#endregion

		#region Identifying elements

		[TestMethod]
		public void Is_Private_Event_Handler()
		{
			CsDocument document = BuildCodeDocument(Source.PrivateEventHandlers);
			Assert.IsFalse(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method FalseMethodIsNotPrivate")));
			Assert.IsFalse(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method FalseFirstArgumentIsNotObject")));
			Assert.IsFalse(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method FalseFirstArgumentHasWrongName")));
			Assert.IsFalse(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method FalseSecondArgumentIsNotEventArgs")));
			Assert.IsFalse(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method FalseSecondArgumentHasWrongName")));
			Assert.IsFalse(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method FalseUseAliasForObject1")));
			Assert.IsFalse(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method FalseUseAliasForObject2")));
			Assert.IsTrue(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method TrueSimple")));
			Assert.IsTrue(CodeHelper.IsPrivateEventHandler(GetElementByName(document, "method TrueComplexEventArgs")));
		}

		#endregion

		#region Working with element names

		[TestMethod]
		public void Extract_Pure_Name()
		{
			string name;

			name = "Exception";
			Assert.AreEqual("Exception", CodeHelper.ExtractPureName(name));

			name = "System.Exception";
			Assert.AreEqual("Exception", CodeHelper.ExtractPureName(name));

			name = "Dictionary<int, bool>";
			Assert.AreEqual("Dictionary", CodeHelper.ExtractPureName(name));

			name = "System.Collections.Dictionary<int, bool>";
			Assert.AreEqual("Dictionary", CodeHelper.ExtractPureName(name));

			name = "IDisposable.Dispose";
			Assert.AreEqual("Dispose", CodeHelper.ExtractPureName(name));

			name = "value";
			Assert.AreEqual("value", CodeHelper.ExtractPureName(name));

			name = "@value";
			Assert.AreEqual("value", CodeHelper.ExtractPureName(name));
		}

		#endregion

		#region Working with local declarations and labels

		[TestMethod]
		public void Get_Local_Declarations()
		{
			CsDocument document = BuildCodeDocument(Source.LocalDeclarations);
			CsElement element = GetElementByName(document, "constructor Class1");
			List<LocalDeclarationItem> declarations = CodeHelper.GetLocalDeclarations(element);
			Assert.AreEqual(50, declarations.Count);
			AssertLocalDeclaration("a1", false, 15, declarations[0]);
			AssertLocalDeclaration("a2", false, 15, declarations[1]);
			AssertLocalDeclaration("a3", false, 15, declarations[2]);
			AssertLocalDeclaration("b1", true, 16, declarations[3]);
			AssertLocalDeclaration("b2", true, 16, declarations[4]);
			AssertLocalDeclaration("ex1", false, 20, declarations[5]);
			AssertLocalDeclaration("ex2", false, 24, declarations[6]);
			AssertLocalDeclaration("i1", false, 30, declarations[7]);
			AssertLocalDeclaration("i2", false, 31, declarations[8]);
			AssertLocalDeclaration("i3", false, 32, declarations[9]);
			AssertLocalDeclaration("j1", false, 35, declarations[10]);
			AssertLocalDeclaration("j2", false, 35, declarations[11]);
			AssertLocalDeclaration("j3", false, 36, declarations[12]);
			AssertLocalDeclaration("a1", false, 40, declarations[13]);
			AssertLocalDeclaration("a2", false, 40, declarations[14]);
			AssertLocalDeclaration("a3", false, 40, declarations[15]);
			AssertLocalDeclaration("b1", true, 41, declarations[16]);
			AssertLocalDeclaration("b2", true, 41, declarations[17]);
			AssertLocalDeclaration("s1", false, 46, declarations[18]);
			AssertLocalDeclaration("s2", false, 47, declarations[19]);
			AssertLocalDeclaration("a1", false, 49, declarations[20]);
			AssertLocalDeclaration("a2", false, 49, declarations[21]);
			AssertLocalDeclaration("a3", false, 49, declarations[22]);
			AssertLocalDeclaration("b1", true, 50, declarations[23]);
			AssertLocalDeclaration("b2", true, 50, declarations[24]);
			AssertLocalDeclaration("ms1", false, 55, declarations[25]);
			AssertLocalDeclaration("ms2", false, 56, declarations[26]);
			AssertLocalDeclaration("ms3", false, 57, declarations[27]);
			AssertLocalDeclaration("a1", false, 59, declarations[28]);
			AssertLocalDeclaration("a2", false, 59, declarations[29]);
			AssertLocalDeclaration("a3", false, 59, declarations[30]);
			AssertLocalDeclaration("b1", true, 60, declarations[31]);
			AssertLocalDeclaration("b2", true, 60, declarations[32]);
			AssertLocalDeclaration("c1", false, 66, declarations[33]);
			AssertLocalDeclaration("c2", false, 66, declarations[34]);
			AssertLocalDeclaration("c3", false, 67, declarations[35]);
			AssertLocalDeclaration("d1", true, 69, declarations[36]);
			AssertLocalDeclaration("d2", true, 70, declarations[37]);
			AssertLocalDeclaration("thread1", false, 74, declarations[38]);
			AssertLocalDeclaration("a1", false, 77, declarations[39]);
			AssertLocalDeclaration("a2", false, 78, declarations[40]);
			AssertLocalDeclaration("a3", false, 79, declarations[41]);
			AssertLocalDeclaration("b1", true, 81, declarations[42]);
			AssertLocalDeclaration("b2", true, 82, declarations[43]);
			AssertLocalDeclaration("thread2", false, 87, declarations[44]);
			AssertLocalDeclaration("a1", false, 89, declarations[45]);
			AssertLocalDeclaration("a2", false, 89, declarations[46]);
			AssertLocalDeclaration("a3", false, 90, declarations[47]);
			AssertLocalDeclaration("b1", true, 91, declarations[48]);
			AssertLocalDeclaration("b2", true, 92, declarations[49]);
		}

		[TestMethod]
		public void Get_Labels()
		{
			CsDocument document = BuildCodeDocument(Source.Labels);
			CsElement element = GetElementByName(document, "constructor Class1");
			List<LabelItem> labels = CodeHelper.GetLabels(element);
			Assert.AreEqual(10, labels.Count);
			AssertLabel("lab1", 15, labels[0]);
			AssertLabel("lab2", 24, labels[1]);
			AssertLabel("lab3", 29, labels[2]);
			AssertLabel("lab4", 34, labels[3]);
			AssertLabel("lab5", 38, labels[4]);
			AssertLabel("lab6", 40, labels[5]);
			AssertLabel("lab7", 41, labels[6]);
			AssertLabel("lab8", 44, labels[7]);
			AssertLabel("lab9", 48, labels[8]);
			AssertLabel("lab10", 53, labels[9]);
		}

		#endregion
	}
}
