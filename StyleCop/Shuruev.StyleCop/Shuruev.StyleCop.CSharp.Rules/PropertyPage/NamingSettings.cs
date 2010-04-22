using System.Collections.Generic;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// All naming settings.
	/// </summary>
	internal static class NamingSettings
	{
		internal const string Abbreviations = "NamingAbbreviations";
		internal const string Derivings = "NamingDerivings";

		internal const string Namespace = "NamespaceNamingRule";
		internal const string ClassNotInternal = "ClassNotInternalNamingRule";
		internal const string ClassInternal = "ClassInternalNamingRule";
		internal const string StructNotInternal = "StructNotInternalNamingRule";
		internal const string StructInternal = "StructInternalNamingRule";
		internal const string Interface = "InterfaceNamingRule";
		//ClassException
		//ClassAttribute
		//Field (static, readonly, accessor)
		// - static / instance
		// - read-only / read-write
		//
		// Constant
		//Delegate
		//Event
		//Enum
		//EnumItem
		//Property
		//Method
		//operator?
		//Windows forms event handlers
		//Type parameters
		//Local variables (constant?)
		//Parameters (value, reference, output, params array)

		internal static readonly List<string> All = new List<string>(
			new[]
			{
				Abbreviations,
				Namespace,
				ClassNotInternal,
				ClassInternal,
				Derivings,
				StructNotInternal,
				StructInternal,
				Interface
			});
	}
}
