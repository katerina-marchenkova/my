using System.Collections.Generic;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// All naming settings.
	/// </summary>
	internal static class NamingSettings
	{
		internal const string Abbreviations = "NamingAbbreviations";
		internal const string Namespace = "NamespaceNamingRule";
		internal const string ClassNotInternal = "ClassNotInternalNamingRule";
		internal const string ClassInternal = "ClassInternalNamingRule";
		internal const string StructNotInternal = "StructNotInternalNamingRule";
		internal const string StructInternal = "StructInternalNamingRule";
		internal const string Interface = "InterfaceNamingRule";

		internal static readonly List<string> All = new List<string>(
			new[]
			{
				Abbreviations,
				Namespace,
				ClassNotInternal,
				ClassInternal,
				StructNotInternal,
				StructInternal,
				Interface
			});
	}
}
