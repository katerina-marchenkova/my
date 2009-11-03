namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    [SourceAnalyzer(typeof(CsParser))]
    public class OrderingRules : SourceAnalyzer
    {
        internal const bool GeneratedCodeElementOrderDefaultValueProperty = true;
        internal const string GeneratedCodeElementOrderProperty = "GeneratedCodeElementOrder";

        private static string AccessModifierTypeString(AccessModifierType type)
        {
            switch (type)
            {
                case AccessModifierType.Public:
                    return "public";

                case AccessModifierType.Internal:
                    return "internal";

                case AccessModifierType.ProtectedInternal:
                    return "protected internal";

                case AccessModifierType.Protected:
                    return "protected";

                case AccessModifierType.Private:
                    return "private";
            }
            throw new InvalidOperationException();
        }

        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            CsDocument document2 = (CsDocument) document;
            if (document2.RootElement != null)
            {
                bool checkGeneratedCode = true;
                if (document.Settings != null)
                {
                    BooleanProperty setting = base.GetSetting(document.Settings, "GeneratedCodeElementOrder") as BooleanProperty;
                    if (setting != null)
                    {
                        checkGeneratedCode = setting.Value;
                    }
                    this.ProcessElements(document2.RootElement, checkGeneratedCode);
                }
                this.CheckUsingDirectiveOrder(document2.RootElement);
            }
        }

        private void CheckChildElementOrdering(CsElement element, bool checkGeneratedCode)
        {
            if (element.ChildElements.Count > 0)
            {
                bool flag = true;
                CsElement[] array = new CsElement[element.ChildElements.Count];
                element.ChildElements.CopyTo(array, 0);
                for (int i = 0; i < array.Length; i++)
                {
                    CsElement first = array[i];
                    if (first.AnalyzerTag == null)
                    {
                        for (int j = i + 1; j < array.Length; j++)
                        {
                            CsElement second = array[j];
                            if ((second.AnalyzerTag == null) && ((checkGeneratedCode && (!first.Generated || !second.Generated)) || ((!checkGeneratedCode && !first.Generated) && !second.Generated)))
                            {
                                if (!this.CompareItems(first, second, !flag))
                                {
                                    if (flag)
                                    {
                                        first.AnalyzerTag = false;
                                    }
                                    else
                                    {
                                        second.AnalyzerTag = false;
                                    }
                                }
                                else if (flag)
                                {
                                    flag = false;
                                }
                                if ((first.ElementType == ElementType.Accessor) && (second.ElementType == ElementType.Accessor))
                                {
                                    Accessor accessor = (Accessor) first;
                                    Accessor accessor2 = (Accessor) second;
                                    if ((accessor.AccessorType == AccessorType.Set) && (accessor2.AccessorType == AccessorType.Get))
                                    {
                                        base.AddViolation(first, Microsoft.StyleCop.CSharp.Rules.PropertyAccessorsMustFollowOrder, new object[0]);
                                    }
                                    else if ((accessor.AccessorType == AccessorType.Remove) && (accessor2.AccessorType == AccessorType.Add))
                                    {
                                        base.AddViolation(first, Microsoft.StyleCop.CSharp.Rules.EventAccessorsMustFollowOrder, new object[0]);
                                    }
                                }
                            }
                        }
                    }
                    this.CheckElementOrder(first, checkGeneratedCode);
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckDeclarationKeywordOrder(CsElement element)
        {
            int num = -1;
            int num2 = -1;
            int num3 = -1;
            int num4 = 0;
            foreach (CsToken token in element.Declaration.Tokens)
            {
                CsTokenType csTokenType = token.CsTokenType;
                switch (csTokenType)
                {
                    case CsTokenType.Private:
                    case CsTokenType.Public:
                    case CsTokenType.Protected:
                    case CsTokenType.Internal:
                    {
                        if (num == -1)
                        {
                            num = num4++;
                        }
                        continue;
                    }
                }
                if (csTokenType == CsTokenType.Static)
                {
                    if (num2 == -1)
                    {
                        num2 = num4++;
                    }
                }
                else if ((((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.SingleLineComment) && (csTokenType != CsTokenType.MultiLineComment))) && (num3 == -1))
                {
                    num3 = num4++;
                }
            }
            if (num != -1)
            {
                if ((num2 > -1) && (num2 < num))
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.DeclarationKeywordsMustFollowOrder, new object[] { Microsoft.StyleCop.CSharp.Strings.AccessModifier, string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[] { Microsoft.StyleCop.CSharp.Strings.Static }) });
                }
                if ((num3 > -1) && (num3 < num))
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.DeclarationKeywordsMustFollowOrder, new object[] { Microsoft.StyleCop.CSharp.Strings.AccessModifier, string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[] { Microsoft.StyleCop.CSharp.Strings.Other }) });
                }
            }
            if (((num2 > -1) && (num3 > -1)) && (num3 < num2))
            {
                base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.DeclarationKeywordsMustFollowOrder, new object[] { string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[] { Microsoft.StyleCop.CSharp.Strings.Static }), string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[] { Microsoft.StyleCop.CSharp.Strings.Other }) });
            }
            if (element.Declaration.AccessModifierType == AccessModifierType.ProtectedInternal)
            {
                bool flag = false;
                foreach (CsToken token2 in element.Declaration.Tokens)
                {
                    if (flag)
                    {
                        if (token2.CsTokenType != CsTokenType.Internal)
                        {
                            if (token2.CsTokenType == CsTokenType.WhiteSpace)
                            {
                                continue;
                            }
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ProtectedMustComeBeforeInternal, new object[0]);
                        }
                        return;
                    }
                    if (token2.CsTokenType == CsTokenType.Protected)
                    {
                        flag = true;
                    }
                    else if (token2.CsTokenType == CsTokenType.Internal)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.ProtectedMustComeBeforeInternal, new object[0]);
                        return;
                    }
                }
            }
        }

        private void CheckElementOrder(CsElement element, bool checkGeneratedCode)
        {
            if (!element.Generated && (((((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Field)) || ((element.ElementType == ElementType.Enum) || (element.ElementType == ElementType.Struct))) || (((element.ElementType == ElementType.Interface) || (element.ElementType == ElementType.Delegate)) || ((element.ElementType == ElementType.Event) || (element.ElementType == ElementType.Property)))) || (((element.ElementType == ElementType.Indexer) || (element.ElementType == ElementType.Method)) || ((element.ElementType == ElementType.Constructor) || (element.ElementType == ElementType.Accessor)))))
            {
                this.CheckDeclarationKeywordOrder(element);
            }
            this.CheckUsingDirectivePlacement(element);
            this.CheckChildElementOrdering(element, checkGeneratedCode);
        }

        private void CheckOrderOfUsingDirectivesInList(List<UsingDirective> usings)
        {
            for (int i = 0; i < usings.Count; i++)
            {
                UsingDirective firstUsing = usings[i];
                for (int j = i + 1; j < usings.Count; j++)
                {
                    UsingDirective secondUsing = usings[j];
                    if (!this.CompareOrderOfUsingDirectives(firstUsing, secondUsing))
                    {
                        break;
                    }
                }
            }
        }

        private void CheckOrderOfUsingDirectivesUnderElement(CsElement element)
        {
            List<UsingDirective> usings = null;
            foreach (CsElement element2 in element.ChildElements)
            {
                if (element2.ElementType == ElementType.UsingDirective)
                {
                    if (usings == null)
                    {
                        usings = new List<UsingDirective>();
                    }
                    usings.Add((UsingDirective) element2);
                    continue;
                }
                if (element2.ElementType != ElementType.ExternAliasDirective)
                {
                    break;
                }
            }
            if (usings != null)
            {
                this.CheckOrderOfUsingDirectivesInList(usings);
            }
        }

        private void CheckUsingDirectiveOrder(CsElement rootElement)
        {
            if (!rootElement.Generated)
            {
                this.CheckOrderOfUsingDirectivesUnderElement(rootElement);
                foreach (CsElement element in rootElement.ChildElements)
                {
                    if (element.ElementType == ElementType.Namespace)
                    {
                        this.CheckUsingDirectiveOrder(element);
                    }
                }
            }
        }

        private void CheckUsingDirectivePlacement(CsElement element)
        {
            if (!element.Generated && (element.ElementType == ElementType.UsingDirective))
            {
                CsElement parentElement = element.ParentElement;
                if ((parentElement != null) && (parentElement.ElementType != ElementType.Namespace))
                {
                    bool flag = false;
                    if (parentElement.ElementType == ElementType.Root)
                    {
                        foreach (CsElement element3 in parentElement.ChildElements)
                        {
                            if (element3.ElementType == ElementType.Namespace)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.UsingDirectivesMustBePlacedWithinNamespace, new object[0]);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private bool CompareItems(CsElement first, CsElement second, bool foundFirst)
        {
            if (((first.ElementType != ElementType.EmptyElement) && (second.ElementType != ElementType.EmptyElement)) && ((first.ElementType != ElementType.Accessor) || (second.ElementType != ElementType.Accessor)))
            {
                CsElement element = second;
                if (!foundFirst)
                {
                    element = first;
                }
                if (first.ElementType > second.ElementType)
                {
                    base.AddViolation(first, element.LineNumber, Microsoft.StyleCop.CSharp.Rules.ElementsMustAppearInTheCorrectOrder, new object[] { first.FriendlyPluralTypeText, second.FriendlyPluralTypeText });
                    return false;
                }
                if (((first.ElementType == second.ElementType) && (first.Declaration != null)) && (second.Declaration != null))
                {
                    if (first.Declaration.AccessModifierType > second.Declaration.AccessModifierType)
                    {
                        if (((!first.Declaration.AccessModifier && (first.ElementType != ElementType.Method)) && first.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial })) || ((!second.Declaration.AccessModifier && (second.ElementType != ElementType.Method)) && second.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial })))
                        {
                            CsElement element2 = first;
                            if (first.Declaration.AccessModifier || !first.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial }))
                            {
                                element2 = second;
                            }
                            base.AddViolation(element2, Microsoft.StyleCop.CSharp.Rules.PartialElementsMustDeclareAccess, new object[] { element2.FriendlyTypeText, element2.FriendlyPluralTypeText });
                        }
                        else
                        {
                            base.AddViolation(first, element.LineNumber, Microsoft.StyleCop.CSharp.Rules.ElementsMustBeOrderedByAccess, new object[] { AccessModifierTypeString(first.Declaration.AccessModifierType), first.FriendlyPluralTypeText, AccessModifierTypeString(second.Declaration.AccessModifierType), second.FriendlyPluralTypeText });
                        }
                        return false;
                    }
                    if (first.Declaration.AccessModifierType == second.Declaration.AccessModifierType)
                    {
                        bool @const = false;
                        bool @readonly = false;
                        Field field = first as Field;
                        Field field2 = second as Field;
                        if ((field != null) && (field2 != null))
                        {
                            @const = field.Const;
                            @readonly = field.Readonly;
                            if ((field2.Const || field2.Readonly) && (!field.Const && !field.Readonly))
                            {
                                base.AddViolation(first, element.LineNumber, Microsoft.StyleCop.CSharp.Rules.ConstantsMustAppearBeforeFields, new object[] { AccessModifierTypeString(first.Declaration.AccessModifierType), first.FriendlyPluralTypeText, AccessModifierTypeString(second.Declaration.AccessModifierType), second.FriendlyPluralTypeText });
                                return false;
                            }
                        }
                        if ((second.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }) && !first.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static })) && (!@const && !@readonly))
                        {
                            base.AddViolation(first, element.LineNumber, Microsoft.StyleCop.CSharp.Rules.StaticElementsMustAppearBeforeInstanceElements, new object[] { AccessModifierTypeString(first.Declaration.AccessModifierType), first.FriendlyPluralTypeText, AccessModifierTypeString(second.Declaration.AccessModifierType), second.FriendlyPluralTypeText });
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CompareOrderOfUsingDirectives(UsingDirective firstUsing, UsingDirective secondUsing)
        {
            if (string.IsNullOrEmpty(firstUsing.Alias))
            {
                if (string.IsNullOrEmpty(secondUsing.Alias))
                {
                    bool flag = firstUsing.NamespaceType.StartsWith("System", StringComparison.Ordinal) || firstUsing.NamespaceType.StartsWith("System.", StringComparison.Ordinal);
                    bool flag2 = secondUsing.NamespaceType.Equals("System", StringComparison.Ordinal) || secondUsing.NamespaceType.StartsWith("System.", StringComparison.Ordinal);
                    if (flag2 && !flag)
                    {
                        base.AddViolation(secondUsing, Microsoft.StyleCop.CSharp.Rules.SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives, new object[0]);
                        return false;
                    }
                    if (((flag && flag2) || (!flag && !flag2)) && (string.Compare(firstUsing.NamespaceType, secondUsing.NamespaceType, StringComparison.InvariantCultureIgnoreCase) > 0))
                    {
                        base.AddViolation(firstUsing, Microsoft.StyleCop.CSharp.Rules.UsingDirectivesMustBeOrderedAlphabeticallyByNamespace, new object[0]);
                        return false;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(secondUsing.Alias))
                {
                    base.AddViolation(firstUsing, Microsoft.StyleCop.CSharp.Rules.UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives, new object[0]);
                    return false;
                }
                if (string.Compare(firstUsing.Alias, secondUsing.Alias, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    base.AddViolation(firstUsing, Microsoft.StyleCop.CSharp.Rules.UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName, new object[0]);
                    return false;
                }
            }
            return true;
        }

        private bool ProcessElements(CsElement element, bool checkGeneratedCode)
        {
            if (base.Cancel)
            {
                return false;
            }
            this.CheckElementOrder(element, checkGeneratedCode);
            return true;
        }
    }
}

