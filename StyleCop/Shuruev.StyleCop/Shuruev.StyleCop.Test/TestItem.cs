namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Test in test definition file.
	/// </summary>
	public struct TestItem
	{
		/// <summary>
		/// A number of expected errors.
		/// </summary>
		public int ErrorCount;

		/// <summary>
		/// A list of expected violations.
		/// </summary>
		public string Description;

		/// <summary>
		/// Source code.
		/// </summary>
		public string SourceCode;
	}
}
