namespace CCNet.Common
{
	/// <summary>
	/// Reference item.
	/// </summary>
	public class Reference
	{
		/// <summary>
		/// Assembly name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Assembly version.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Assembly culture.
		/// </summary>
		public string Culture { get; set; }

		/// <summary>
		/// Public key token.
		/// </summary>
		public string PublicKeyToken { get; set; }

		/// <summary>
		/// Processor architecture.
		/// </summary>
		public string ProcessorArchitecture { get; set; }

		/// <summary>
		/// Specific version.
		/// </summary>
		public string SpecificVersion { get; set; }

		/// <summary>
		/// Hint path.
		/// </summary>
		public string HintPath { get; set; }

		/// <summary>
		/// Private property.
		/// </summary>
		public string Private { get; set; }

		/// <summary>
		/// Aliases property.
		/// </summary>
		public string Aliases { get; set; }

		/// <summary>
		/// Embedding options.
		/// </summary>
		public string EmbedInteropTypes { get; set; }
	}
}
