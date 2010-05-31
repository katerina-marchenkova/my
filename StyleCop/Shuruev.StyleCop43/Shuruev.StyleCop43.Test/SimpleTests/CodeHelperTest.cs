using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.StyleCop.CSharp;

namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Testing code helper work.
	/// </summary>
	[TestClass]
	public class CodeHelperTest
	{
		/// <summary>
		/// Tests pure name extracting.
		/// </summary>
		[TestMethod]
		public void ExtractPureName()
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
		}
	}
}
