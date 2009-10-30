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
		private readonly List<string> emptyWords = new List<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ReleaserInterfacesTest"/> class.
		/// </summary>
		public ReleaserInterfacesTest()
		{
			this.emptyWords.Add("   ");
			this.emptyWords.Add("\r\n\r\n");
			this.emptyWords.Add("\t\t\v\v");
			this.emptyWords.Add("   \t\t   \r\n   \v\v   ");
			this.emptyWords.Add(null);
		}

		#region Container classes

		/// <summary>
		/// Tests project data container.
		/// </summary>
		[TestMethod]
		public void ProjectDataContainer()
		{
			foreach (string word in this.emptyWords)
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

		/// <summary>
		/// Tests property values container.
		/// </summary>
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

			foreach (string word in this.emptyWords)
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
