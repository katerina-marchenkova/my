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
			LocalDeclaration declaration)
		{
			Assert.AreEqual(expectedName, declaration.Name);
			Assert.AreEqual(expectedIsConstant, declaration.IsConstant);
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
		}

		#endregion

		#region Working with local declarations

		[TestMethod]
		public void Get_Local_Declarations()
		{
			CsDocument document = BuildCodeDocument(Source.LocalDeclarations);
			CsElement element = GetElementByName(document, "constructor Class1");
			List<LocalDeclaration> declarations = CodeHelper.GetLocalDeclarations(element);
			Assert.AreEqual(50, declarations.Count);
			AssertLocalDeclaration("a1", false, declarations[0]);
			AssertLocalDeclaration("a2", false, declarations[1]);
			AssertLocalDeclaration("a3", false, declarations[2]);
			AssertLocalDeclaration("b1", true, declarations[3]);
			AssertLocalDeclaration("b2", true, declarations[4]);
			AssertLocalDeclaration("ex1", false, declarations[5]);
			AssertLocalDeclaration("ex2", false, declarations[6]);
			AssertLocalDeclaration("i1", false, declarations[7]);
			AssertLocalDeclaration("i2", false, declarations[8]);
			AssertLocalDeclaration("i3", false, declarations[9]);
			AssertLocalDeclaration("j1", false, declarations[10]);
			AssertLocalDeclaration("j2", false, declarations[11]);
			AssertLocalDeclaration("j3", false, declarations[12]);
			AssertLocalDeclaration("a1", false, declarations[13]);
			AssertLocalDeclaration("a2", false, declarations[14]);
			AssertLocalDeclaration("a3", false, declarations[15]);
			AssertLocalDeclaration("b1", true, declarations[16]);
			AssertLocalDeclaration("b2", true, declarations[17]);
			AssertLocalDeclaration("s1", false, declarations[18]);
			AssertLocalDeclaration("s2", false, declarations[19]);
			AssertLocalDeclaration("a1", false, declarations[20]);
			AssertLocalDeclaration("a2", false, declarations[21]);
			AssertLocalDeclaration("a3", false, declarations[22]);
			AssertLocalDeclaration("b1", true, declarations[23]);
			AssertLocalDeclaration("b2", true, declarations[24]);
			AssertLocalDeclaration("ms1", false, declarations[25]);
			AssertLocalDeclaration("ms2", false, declarations[26]);
			AssertLocalDeclaration("ms3", false, declarations[27]);
			AssertLocalDeclaration("a1", false, declarations[28]);
			AssertLocalDeclaration("a2", false, declarations[29]);
			AssertLocalDeclaration("a3", false, declarations[30]);
			AssertLocalDeclaration("b1", true, declarations[31]);
			AssertLocalDeclaration("b2", true, declarations[32]);
			AssertLocalDeclaration("c1", false, declarations[33]);
			AssertLocalDeclaration("c2", false, declarations[34]);
			AssertLocalDeclaration("c3", false, declarations[35]);
			AssertLocalDeclaration("d1", true, declarations[36]);
			AssertLocalDeclaration("d2", true, declarations[37]);
			AssertLocalDeclaration("thread1", false, declarations[38]);
			AssertLocalDeclaration("a1", false, declarations[39]);
			AssertLocalDeclaration("a2", false, declarations[40]);
			AssertLocalDeclaration("a3", false, declarations[41]);
			AssertLocalDeclaration("b1", true, declarations[42]);
			AssertLocalDeclaration("b2", true, declarations[43]);
			AssertLocalDeclaration("thread2", false, declarations[44]);
			AssertLocalDeclaration("a1", false, declarations[45]);
			AssertLocalDeclaration("a2", false, declarations[46]);
			AssertLocalDeclaration("a3", false, declarations[47]);
			AssertLocalDeclaration("b1", true, declarations[48]);
			AssertLocalDeclaration("b2", true, declarations[49]);
		}

		#endregion
	}
}
