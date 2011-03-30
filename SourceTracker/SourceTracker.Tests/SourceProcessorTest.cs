using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourceTracker.Tests
{
	[TestClass]
	public class SourceProcessorTest
	{
		[TestMethod]
		public void Crc_Should_Ignore_Whitespaces()
		{
			Assert.AreEqual(
				SourceProcessor.CalculateCrc("   class Class1   "),
				SourceProcessor.CalculateCrc("class Class1"));

			Assert.AreEqual(
				SourceProcessor.CalculateCrc("\tclass Class1\v"),
				SourceProcessor.CalculateCrc("class Class1"));

			Assert.AreEqual(
				SourceProcessor.CalculateCrc("   "),
				SourceProcessor.CalculateCrc(String.Empty));
		}

		[TestMethod]
		public void Crc_Should_Be_Case_Insensitive()
		{
			Assert.AreEqual(
				SourceProcessor.CalculateCrc("class Class1"),
				SourceProcessor.CalculateCrc("class class1"));

			Assert.AreEqual(
				SourceProcessor.CalculateCrc("class Class1"),
				SourceProcessor.CalculateCrc("class CLASS1"));
		}
	}
}
