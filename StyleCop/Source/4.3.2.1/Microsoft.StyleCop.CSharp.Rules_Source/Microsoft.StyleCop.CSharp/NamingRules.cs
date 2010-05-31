namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    [SourceAnalyzer(typeof(CsParser))]
    public class NamingRules : SourceAnalyzer
    {
        internal const string AllowedPrefixesProperty = "Hungarian";

        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            CsDocument document2 = (CsDocument) document;
            if ((document2.RootElement != null) && !document2.RootElement.Generated)
            {
                Dictionary<string, string> prefixes = this.GetPrefixes(document.Settings);
                this.ProcessElement(document2.RootElement, prefixes, false);
            }
        }

        private void CheckCase(CsElement element, string name, int line, bool upper)
        {
            if (name.Length >= 1)
            {
                char c = name[0];
                if (char.IsLetter(c))
                {
                    if (upper)
                    {
                        if (!char.IsUpper(c))
                        {
                            base.AddViolation(element, line, Microsoft.StyleCop.CSharp.Rules.ElementMustBeginWithUpperCaseLetter, new object[] { element.FriendlyTypeText, name });
                        }
                    }
                    else if (!char.IsLower(c))
                    {
                        base.AddViolation(element, line, Microsoft.StyleCop.CSharp.Rules.ElementMustBeginWithLowerCaseLetter, new object[] { element.FriendlyTypeText, name });
                    }
                }
            }
        }

        private void CheckFieldPrefix(Field field, Dictionary<string, string> validPrefixes)
        {
            int index = MovePastPrefix(field.Declaration.Name);
            if (char.IsLower(field.Declaration.Name, index))
            {
                this.CheckHungarian(field.Declaration.Name, index, field.LineNumber, field, validPrefixes);
                if (field.Const)
                {
                    base.AddViolation(field, field.LineNumber, Microsoft.StyleCop.CSharp.Rules.ConstFieldNamesMustBeginWithUpperCaseLetter, new object[] { field.Declaration.Name });
                }
                else if (((field.AccessModifier == AccessModifierType.Public) || (field.AccessModifier == AccessModifierType.Internal)) || (field.AccessModifier == AccessModifierType.ProtectedInternal))
                {
                    base.AddViolation(field, field.LineNumber, Microsoft.StyleCop.CSharp.Rules.AccessibleFieldsMustBeginWithUpperCaseLetter, new object[] { field.Declaration.Name });
                }
                if (field.Readonly && (field.Declaration.AccessModifierType != AccessModifierType.Private))
                {
                    base.AddViolation(field, field.LineNumber, Microsoft.StyleCop.CSharp.Rules.NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter, new object[] { field.Declaration.Name });
                }
            }
            else if (((!field.Const && !field.Readonly) && ((field.AccessModifier != AccessModifierType.Public) && (field.AccessModifier != AccessModifierType.Internal))) && (field.AccessModifier != AccessModifierType.ProtectedInternal))
            {
                base.AddViolation(field, field.LineNumber, Microsoft.StyleCop.CSharp.Rules.FieldNamesMustBeginWithLowerCaseLetter, new object[] { field.Declaration.Name });
            }
        }

        private void CheckFieldUnderscores(CsElement field)
        {
            if (field.Declaration.Name.StartsWith("s_", StringComparison.Ordinal) || field.Declaration.Name.StartsWith("m_", StringComparison.Ordinal))
            {
                base.AddViolation(field, Microsoft.StyleCop.CSharp.Rules.VariableNamesMustNotBePrefixed, new object[0]);
            }
            else if (field.Declaration.Name.StartsWith("_", StringComparison.Ordinal))
            {
                base.AddViolation(field, Microsoft.StyleCop.CSharp.Rules.FieldNamesMustNotBeginWithUnderscore, new object[0]);
            }
            else if (field.Declaration.Name.IndexOf("_", StringComparison.Ordinal) > -1)
            {
                base.AddViolation(field, Microsoft.StyleCop.CSharp.Rules.FieldNamesMustNotContainUnderscore, new object[0]);
            }
        }

        private void CheckHungarian(string name, int startIndex, int line, CsElement element, Dictionary<string, string> validPrefixes)
        {
            if ((name.Length - startIndex) > 3)
            {
                string key = null;
                for (int i = startIndex + 1; i < (3 + startIndex); i++)
                {
                    string str2 = name.Substring(i, 1);
                    if (str2 == str2.ToUpper(CultureInfo.InvariantCulture))
                    {
                        key = name.Substring(startIndex, i - startIndex);
                        break;
                    }
                }
                if (key != null)
                {
                    bool flag = false;
                    if ((validPrefixes != null) && validPrefixes.ContainsKey(key))
                    {
                        flag = true;
                    }
                    if (!flag)
                    {
                        base.AddViolation(element, line, Microsoft.StyleCop.CSharp.Rules.FieldNamesMustNotUseHungarianNotation, new object[] { name });
                    }
                }
            }
        }

        private void CheckMethodVariablePrefix(Variable variable, CsElement element, Dictionary<string, string> validPrefixes)
        {
            int index = MovePastPrefix(variable.Name);
            if (char.IsLower(variable.Name, index))
            {
                this.CheckHungarian(variable.Name, index, variable.Location.LineNumber, element, validPrefixes);
                if ((variable.Modifiers & VariableModifiers.Const) == VariableModifiers.Const)
                {
                    base.AddViolation(element, variable.Location.LineNumber, Microsoft.StyleCop.CSharp.Rules.ConstFieldNamesMustBeginWithUpperCaseLetter, new object[] { variable.Name });
                }
            }
            else if ((variable.Modifiers & VariableModifiers.Const) == VariableModifiers.None)
            {
                base.AddViolation(element, variable.Location.LineNumber, Microsoft.StyleCop.CSharp.Rules.FieldNamesMustBeginWithLowerCaseLetter, new object[] { variable.Name });
            }
        }

        private void CheckUnderscores(CsElement element, VariableCollection variables)
        {
            foreach (Variable variable in variables)
            {
                if (variable.Name.StartsWith("_", StringComparison.Ordinal))
                {
                    base.AddViolation(element, variable.Location.LineNumber, Microsoft.StyleCop.CSharp.Rules.FieldNamesMustNotBeginWithUnderscore, new object[0]);
                }
            }
        }

        private Dictionary<string, string> GetPrefixes(Microsoft.StyleCop.Settings settings)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (settings != null)
            {
                CollectionProperty setting = base.GetSetting(settings, "Hungarian") as CollectionProperty;
                if ((setting == null) || (setting.Count <= 0))
                {
                    return dictionary;
                }
                foreach (string str in setting)
                {
                    if (!string.IsNullOrEmpty(str) && !dictionary.ContainsKey(str))
                    {
                        dictionary.Add(str, str);
                    }
                }
            }
            return dictionary;
        }

        private static int MovePastPrefix(string name)
        {
            if ((name.StartsWith("s_", StringComparison.Ordinal) || name.StartsWith("m_", StringComparison.Ordinal)) || name.StartsWith("__", StringComparison.Ordinal))
            {
                return 2;
            }
            if (!name.StartsWith("_", StringComparison.Ordinal) && !name.StartsWith("@", StringComparison.Ordinal))
            {
                return 0;
            }
            return 1;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private bool ProcessElement(CsElement element, Dictionary<string, string> validPrefixes, bool nativeMethods)
        {
            if (base.Cancel)
            {
                return false;
            }
            if ((!element.Generated && (element.Declaration != null)) && (element.Declaration.Name != null))
            {
                switch (element.ElementType)
                {
                    case ElementType.Namespace:
                    case ElementType.Delegate:
                    case ElementType.Event:
                    case ElementType.Enum:
                    case ElementType.Property:
                    case ElementType.Struct:
                    case ElementType.Class:
                        if (!nativeMethods)
                        {
                            this.CheckCase(element, element.Declaration.Name, element.LineNumber, true);
                        }
                        break;

                    case ElementType.Field:
                        if (!nativeMethods)
                        {
                            this.CheckFieldUnderscores(element);
                            this.CheckFieldPrefix(element as Field, validPrefixes);
                        }
                        break;

                    case ElementType.Interface:
                        if ((element.Declaration.Name.Length < 1) || (element.Declaration.Name[0] != 'I'))
                        {
                            base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.InterfaceNamesMustBeginWithI, new object[] { element.Declaration.Name });
                        }
                        break;

                    case ElementType.Method:
                        if ((!nativeMethods && !element.Declaration.Name.StartsWith("operator", StringComparison.Ordinal)) && (element.Declaration.Name != "foreach"))
                        {
                            this.CheckCase(element, element.Declaration.Name, element.LineNumber, true);
                        }
                        break;
                }
            }
            if ((!nativeMethods && ((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Struct))) && element.Declaration.Name.EndsWith("NativeMethods", StringComparison.Ordinal))
            {
                nativeMethods = true;
            }
            foreach (CsElement element2 in element.ChildElements)
            {
                if (!this.ProcessElement(element2, validPrefixes, nativeMethods))
                {
                    return false;
                }
            }
            if (!nativeMethods)
            {
                this.ProcessStatementContainer(element, validPrefixes);
            }
            return true;
        }

        private void ProcessExpression(Expression expression, CsElement element, Dictionary<string, string> validPrefixes)
        {
            if (expression.ExpressionType == ExpressionType.AnonymousMethod)
            {
                AnonymousMethodExpression expression2 = (AnonymousMethodExpression) expression;
                if (expression2.Variables != null)
                {
                    foreach (Variable variable in expression2.Variables)
                    {
                        this.CheckMethodVariablePrefix(variable, element, validPrefixes);
                    }
                    foreach (Statement statement in expression2.ChildStatements)
                    {
                        this.ProcessStatement(statement, element, validPrefixes);
                    }
                }
            }
            foreach (Expression expression3 in expression.ChildExpressions)
            {
                this.ProcessExpression(expression3, element, validPrefixes);
            }
        }

        private void ProcessStatement(Statement statement, CsElement element, Dictionary<string, string> validPrefixes)
        {
            if (statement.Variables != null)
            {
                foreach (Variable variable in statement.Variables)
                {
                    this.CheckMethodVariablePrefix(variable, element, validPrefixes);
                    this.CheckUnderscores(element, statement.Variables);
                }
            }
            foreach (Expression expression in statement.ChildExpressions)
            {
                this.ProcessExpression(expression, element, validPrefixes);
            }
            foreach (Statement statement2 in statement.ChildStatements)
            {
                this.ProcessStatement(statement2, element, validPrefixes);
            }
        }

        private void ProcessStatementContainer(CsElement element, Dictionary<string, string> validPrefixes)
        {
            if (element.Variables != null)
            {
                foreach (Variable variable in element.Variables)
                {
                    if (!variable.Generated)
                    {
                        this.CheckMethodVariablePrefix(variable, element, validPrefixes);
                        this.CheckUnderscores(element, element.Variables);
                    }
                }
            }
            foreach (Statement statement in element.ChildStatements)
            {
                this.ProcessStatement(statement, element, validPrefixes);
            }
        }

        public override ICollection<IPropertyControlPage> SettingsPages
        {
            get
            {
                return new IPropertyControlPage[] { new ValidPrefixes(this) };
            }
        }
    }
}

