using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.Releaser.Interfaces;

namespace Shuruev.Releaser.Test
{
	/// <summary>
	/// Testing interface classes.
	/// </summary>
	[TestClass]
	public class ReleaserInterfacesTest
	{
		public List<string> EmptyWords = new List<string>();

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ReleaserInterfacesTest()
		{
			EmptyWords.Add("   ");
			EmptyWords.Add("\r\n\r\n");
			EmptyWords.Add("\t\t\v\v");
			EmptyWords.Add("   \t\t   \r\n   \v\v   ");
			EmptyWords.Add(null);
		}

		#region Container classes

		[TestMethod]
		public void ProjectDataContainer()
		{
			foreach (string word in EmptyWords)
			{
				TestHelper.Throws(delegate
				{
					new ProjectData(
						word,
						"Default",
						"None",
						"None");
				});

				TestHelper.Throws(delegate
				{
					new ProjectData(
						"New Project",
						word,
						"None",
						"None");
				});

				TestHelper.Throws(delegate
				{
					new ProjectData(
						"New Project",
						"Default",
						word,
						"None");
				});

				TestHelper.Throws(delegate
				{
					new ProjectData(
						"New Project",
						"Default",
						"None",
						word);
				});
			}
		}

		[TestMethod]
		public void PropertyValuesContainer()
		{
			PropertyValues properties = new PropertyValues();
			properties.Add("key1", "oleg");

			Assert.AreEqual(1, properties.GetCodes().Count);
			Assert.AreEqual(1, properties.GetValues("key1").Count);
			Assert.AreEqual("oleg", properties.GetValues("key1")[0]);
			Assert.AreEqual(0, properties.GetValues("key2").Count);

			properties.Add("key1", "shuruev");
			properties.Add("key2", "hello");

			Assert.AreEqual(2, properties.GetCodes().Count);
			Assert.AreEqual(2, properties.GetValues("key1").Count);
			Assert.AreEqual("oleg", properties.GetValues("key1")[0]);
			Assert.AreEqual("shuruev", properties.GetValues("key1")[1]);
			Assert.AreEqual(1, properties.GetValues("key2").Count);
			Assert.AreEqual("hello", properties.GetValues("key2")[0]);

			properties.Clear("key1");
			properties.Clear("key3");

			Assert.AreEqual(1, properties.GetCodes().Count);
			Assert.AreEqual(1, properties.GetValues("key2").Count);
			Assert.AreEqual("hello", properties.GetValues("key2")[0]);

			foreach (string word in EmptyWords)
			{
				TestHelper.Throws(delegate
				{
					properties.Add(word, "hello");
				});

				TestHelper.Throws(delegate
				{
					properties.Add("key1", word);
				});
			}
		}

		#endregion
	}
}
