using System.Collections.Generic;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// All naming settings.
	/// </summary>
	internal static class NamingSettings
	{
		internal const string Namespace = "NamespaceNamingRule";
		internal const string Interface = "InterfaceNamingRule";
		internal const string Class = "ClassNamingRule";
		internal const string Struct = "StructNamingRule";

		internal static readonly List<string> All = new List<string>(
			new[]
			{
				Namespace,
				Interface,
				Class,
				Struct
			});
	}
}
