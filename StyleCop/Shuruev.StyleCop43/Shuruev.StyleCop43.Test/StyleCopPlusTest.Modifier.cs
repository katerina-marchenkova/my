namespace Shuruev.StyleCop.Test
{
	/// <summary>
	/// Testing StyleCop+ plug-in.
	/// </summary>
	public partial class StyleCopPlusTest
	{
		/// <summary>
		/// Modifies source code before running a test.
		/// </summary>
		public string ModifySource(string sourceCode)
		{
			sourceCode = sourceCode.Replace("in TInput", "TInput");
			sourceCode = sourceCode.Replace("out TOutput", "TOutput");
			sourceCode = sourceCode.Replace("in Input", "Input");
			sourceCode = sourceCode.Replace("out Output", "Output");
			return sourceCode;
		}
	}
}
