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

		internal const string PublicInstanceField = "PublicInstanceFieldNamingRule";
		internal const string ProtectedInstanceField = "ProtectedInstanceFieldNamingRule";
		internal const string PrivateInstanceField = "PrivateInstanceFieldNamingRule";
		internal const string InternalInstanceField = "InternalInstanceFieldNamingRule";
		internal const string PublicStaticField = "PublicStaticFieldNamingRule";
		internal const string ProtectedStaticField = "ProtectedStaticFieldNamingRule";
		internal const string PrivateStaticField = "PrivateStaticFieldNamingRule";
		internal const string InternalStaticField = "InternalStaticFieldNamingRule";
		internal const string PublicConst = "PublicConstNamingRule";
		internal const string ProtectedConst = "ProtectedConstNamingRule";
		internal const string PrivateConst = "PrivateConstNamingRule";
		internal const string InternalConst = "InternalConstNamingRule";

		internal const string MethodGeneral = "MethodGeneralNamingRule";
		internal const string MethodPrivateEventHandler = "MethodPrivateEventHandlerNamingRule";

		internal const string Delegate = "DelegateNamingRule";
		internal const string Event = "EventNamingRule";
		internal const string Property = "PropertyNamingRule";
		internal const string Enum = "EnumNamingRule";
		internal const string EnumItem = "EnumItemNamingRule";

		internal const string Parameter = "ParameterNamingRule";
		internal const string TypeParameter = "TypeParameterNamingRule";
		internal const string LocalVariable = "LocalVariableNamingRule";
		internal const string LocalConstant = "LocalConstantNamingRule";

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
				Interface,
				PublicInstanceField,
				ProtectedInstanceField,
				PrivateInstanceField,
				InternalInstanceField,
				PublicStaticField,
				ProtectedStaticField,
				PrivateStaticField,
				InternalStaticField,
				PublicConst,
				ProtectedConst,
				PrivateConst,
				InternalConst,
				MethodGeneral,
				MethodPrivateEventHandler,
				Delegate,
				Event,
				Property,
				Enum,
				EnumItem,
				Parameter,
				TypeParameter,
				LocalVariable,
				LocalConstant
			});
	}
}
