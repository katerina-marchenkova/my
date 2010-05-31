namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    [SourceParser, SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification="Camel case better serves in this case.")]
    public class CsParser : SourceParser
    {
        internal const string AnalyzeDesignerFilesProperty = "AnalyzeDesignerFiles";
        internal const string AnalyzeGeneratedFilesProperty = "AnalyzeGeneratedFiles";
        private Dictionary<string, List<CsElement>> partialElements;
        private Dictionary<SuppressedRule, List<CsElement>> suppressions;
        private ReaderWriterLock suppressionsLock = new ReaderWriterLock();

        internal void AddRuleSuppression(CsElement element, string ruleId, string ruleName, string ruleNamespace)
        {
            SuppressedRule key = new SuppressedRule();
            key.RuleId = ruleId;
            key.RuleName = ruleName;
            key.RuleNamespace = ruleNamespace;
            this.suppressionsLock.AcquireWriterLock(-1);
            try
            {
                List<CsElement> list = null;
                bool flag = false;
                if (this.suppressions.Count != 0)
                {
                    flag = this.suppressions.TryGetValue(key, out list);
                }
                if (!flag)
                {
                    list = new List<CsElement>();
                    this.suppressions.Add(key, list);
                }
                list.Add(element);
            }
            finally
            {
                this.suppressionsLock.ReleaseWriterLock();
            }
        }

        internal static string GetPreprocessorDirectiveType(Symbol preprocessor, out int bodyIndex)
        {
            bodyIndex = -1;
            int startIndex = -1;
            int num2 = -1;
            for (int i = 1; i < preprocessor.Text.Length; i++)
            {
                if (char.IsLetter(preprocessor.Text[i]))
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex == -1)
            {
                return null;
            }
            num2 = startIndex;
            while (num2 < preprocessor.Text.Length)
            {
                if (!char.IsLetter(preprocessor.Text[num2]))
                {
                    break;
                }
                num2++;
            }
            num2--;
            if (num2 < startIndex)
            {
                return null;
            }
            bodyIndex = num2 + 1;
            return preprocessor.Text.Substring(startIndex, (num2 - startIndex) + 1);
        }

        public override bool IsRuleSuppressed(CodeElement element, string ruleCheckId, string ruleName, string ruleNamespace)
        {
            if (((element != null) && !string.IsNullOrEmpty(ruleCheckId)) && (!string.IsNullOrEmpty(ruleName) && !string.IsNullOrEmpty(ruleNamespace)))
            {
                SuppressedRule key = new SuppressedRule();
                key.RuleId = ruleCheckId;
                key.RuleName = ruleName;
                key.RuleNamespace = ruleNamespace;
                this.suppressionsLock.AcquireReaderLock(-1);
                try
                {
                    List<CsElement> list = null;
                    if ((this.suppressions.Count != 0) && this.suppressions.TryGetValue(key, out list))
                    {
                        return MatchElementWithPossibleElementsTraversingParents((CsElement) element, list);
                    }
                }
                finally
                {
                    this.suppressionsLock.ReleaseReaderLock();
                }
            }
            return false;
        }

        private static bool MatchElementWithPossibleElementsTraversingParents(CsElement element, IEnumerable<CsElement> possibleElements)
        {
            foreach (CsElement element2 in possibleElements)
            {
                for (CsElement element3 = element; element3 != null; element3 = element3.ParentElement)
                {
                    if (element3 == element2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool ParseFile(SourceCode sourceCode, int passNumber, ref CodeDocument document)
        {
            Param.RequireNotNull(sourceCode, "sourceCode");
            Param.RequireGreaterThanOrEqualToZero(passNumber, "passNumber");
            if (passNumber == 0)
            {
                try
                {
                    using (TextReader reader = sourceCode.Read())
                    {
                        if (reader == null)
                        {
                            base.AddViolation(sourceCode, 1, Microsoft.StyleCop.CSharp.Rules.FileMustBeReadable, new object[0]);
                        }
                        else
                        {
                            CodeLexer lexer = new CodeLexer(this, sourceCode, new CodeReader(reader));
                            CodeParser parser = new CodeParser(this, lexer);
                            parser.ParseDocument();
                            document = parser.Document;
                        }
                    }
                }
                catch (SyntaxException exception)
                {
                    base.AddViolation(exception.SourceCode, exception.LineNumber, Microsoft.StyleCop.CSharp.Rules.SyntaxException, new object[] { exception.Message });
                    CsDocument document2 = new CsDocument(sourceCode, this);
                    document2.FileHeader = new FileHeader(string.Empty);
                    document = document2;
                }
            }
            return false;
        }

        public override void PostParse()
        {
            this.partialElements = null;
            this.suppressions = null;
        }

        public override void PreParse()
        {
            this.partialElements = new Dictionary<string, List<CsElement>>();
            this.suppressions = new Dictionary<SuppressedRule, List<CsElement>>();
        }

        public override bool SkipAnalysisForDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            BooleanProperty setting = base.GetSetting(document.Settings, "AnalyzeDesignerFiles") as BooleanProperty;
            bool flag = true;
            if (setting != null)
            {
                flag = setting.Value;
            }
            if (flag || !document.SourceCode.Name.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase))
            {
                BooleanProperty property2 = base.GetSetting(document.Settings, "AnalyzeGeneratedFiles") as BooleanProperty;
                bool flag2 = false;
                if (property2 != null)
                {
                    flag2 = property2.Value;
                }
                if (flag2 || (!document.SourceCode.Name.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) && !document.SourceCode.Name.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }
            return true;
        }

        internal Dictionary<string, List<CsElement>> PartialElements
        {
            get
            {
                return this.partialElements;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SuppressedRule
        {
            public string RuleId;
            public string RuleName;
            public string RuleNamespace;
            public static int GenerateHashCode(string ruleId, string ruleName, string ruleNamespace)
            {
                return (ruleId + ":" + ruleNamespace + "." + ruleName).GetHashCode();
            }

            public override int GetHashCode()
            {
                return GenerateHashCode(this.RuleId, this.RuleName, this.RuleNamespace);
            }
        }
    }
}

