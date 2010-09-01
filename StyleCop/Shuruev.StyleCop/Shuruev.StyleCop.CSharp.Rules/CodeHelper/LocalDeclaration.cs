namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Local declaration object.
	/// </summary>
	public class LocalDeclaration
	{
		/// <summary>
		/// Gets or sets local declaration name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether local declaration is a constant.
		/// </summary>
		public bool IsConstant { get; set; }
	}
}
