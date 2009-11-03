namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SourceAnalyzer(typeof(CsParser))]
    public class SpacingRules : SourceAnalyzer
    {
        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            CsDocument document2 = (CsDocument) document;
            if ((document2.RootElement != null) && !document2.RootElement.Generated)
            {
                this.CheckSpacing(document2.RootElement, document2.Tokens, false);
            }
        }

        private void CheckAttributeTokenCloseBracket(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && ((previous.Value.CsTokenType == CsTokenType.WhiteSpace) || (previous.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingAttributeBracketsMustBeSpacedCorrectly, new object[0]);
            }
        }

        private void CheckAttributeTokenOpenBracket(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> next = tokenNode.Next;
            if ((next != null) && ((next.Value.CsTokenType == CsTokenType.WhiteSpace) || (next.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningAttributeBracketsMustBeSpacedCorrectly, new object[0]);
            }
        }

        private void CheckCloseCurlyBracket(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if (((previous != null) && (previous.Value.CsTokenType != CsTokenType.WhiteSpace)) && (previous.Value.CsTokenType != CsTokenType.EndOfLine))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingCurlyBracketsMustBeSpacedCorrectly, new object[0]);
            }
            Node<CsToken> next = tokenNode.Next;
            if (next != null)
            {
                CsTokenType csTokenType = next.Value.CsTokenType;
                if ((((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.CloseParenthesis) && (csTokenType != CsTokenType.Semicolon))) && (csTokenType != CsTokenType.Comma))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingCurlyBracketsMustBeSpacedCorrectly, new object[0]);
                }
                if (csTokenType == CsTokenType.WhiteSpace)
                {
                    foreach (CsToken token in tokens.ForwardIterator(tokenNode.Next.Next))
                    {
                        CsTokenType type2 = token.CsTokenType;
                        switch (type2)
                        {
                            case CsTokenType.CloseParenthesis:
                            case CsTokenType.Semicolon:
                            case CsTokenType.Comma:
                            {
                                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingCurlyBracketsMustBeSpacedCorrectly, new object[0]);
                                continue;
                            }
                        }
                        if (type2 != CsTokenType.WhiteSpace)
                        {
                            return;
                        }
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckCloseParen(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && ((previous.Value.CsTokenType == CsTokenType.WhiteSpace) || (previous.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingParenthesisMustBeSpacedCorrectly, new object[0]);
            }
            Node<CsToken> next = tokenNode.Next;
            if (next == null)
            {
                return;
            }
            CsTokenType csTokenType = next.Value.CsTokenType;
            if (((((((csTokenType == CsTokenType.WhiteSpace) || (csTokenType == CsTokenType.EndOfLine)) || ((csTokenType == CsTokenType.CloseParenthesis) || (csTokenType == CsTokenType.OpenParenthesis))) || (((csTokenType == CsTokenType.CloseSquareBracket) || (csTokenType == CsTokenType.OpenSquareBracket)) || ((csTokenType == CsTokenType.CloseAttributeBracket) || (csTokenType == CsTokenType.Semicolon)))) || ((((csTokenType == CsTokenType.Comma) || (csTokenType == CsTokenType.Other)) || ((csTokenType == CsTokenType.Base) || (csTokenType == CsTokenType.This))) || (((csTokenType == CsTokenType.Null) || (csTokenType == CsTokenType.New)) || ((csTokenType == CsTokenType.Number) || (csTokenType == CsTokenType.String))))) || ((csTokenType == CsTokenType.Delegate) || ((csTokenType == CsTokenType.OperatorSymbol) && (((OperatorSymbol) next.Value).SymbolType == OperatorType.AddressOf)))) || next.Value.Text.StartsWith(".", StringComparison.Ordinal))
            {
                goto Label_01C1;
            }
            bool flag = false;
            if (csTokenType == CsTokenType.OperatorSymbol)
            {
                OperatorSymbol symbol = next.Value as OperatorSymbol;
                if ((symbol.SymbolType == OperatorType.Negative) || (symbol.SymbolType == OperatorType.Positive))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                goto Label_01C1;
            }
            bool flag2 = false;
            if (csTokenType == CsTokenType.LabelColon)
            {
                foreach (CsToken token in tokens.ReverseIterator(tokenNode.Previous))
                {
                    switch (token.CsTokenType)
                    {
                        case CsTokenType.EndOfLine:
                            goto Label_019E;

                        case CsTokenType.Case:
                            flag2 = true;
                            goto Label_019E;
                    }
                }
            }
        Label_019E:
            if (!flag2)
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingParenthesisMustBeSpacedCorrectly, new object[0]);
            }
        Label_01C1:
            if (csTokenType == CsTokenType.WhiteSpace)
            {
                foreach (CsToken token2 in tokens.ForwardIterator(tokenNode.Next.Next))
                {
                    CsTokenType type3 = token2.CsTokenType;
                    switch (type3)
                    {
                        case CsTokenType.CloseParenthesis:
                        case CsTokenType.OpenParenthesis:
                        case CsTokenType.CloseSquareBracket:
                        case CsTokenType.OpenSquareBracket:
                        case CsTokenType.Semicolon:
                        case CsTokenType.Comma:
                        {
                            base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingParenthesisMustBeSpacedCorrectly, new object[0]);
                            continue;
                        }
                    }
                    if (type3 != CsTokenType.WhiteSpace)
                    {
                        return;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckCloseSquareBracket(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && ((previous.Value.CsTokenType == CsTokenType.WhiteSpace) || (previous.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingSquareBracketsMustBeSpacedCorrectly, new object[0]);
            }
            Node<CsToken> next = tokenNode.Next;
            if (next != null)
            {
                CsTokenType csTokenType = next.Value.CsTokenType;
                if (((((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.CloseParenthesis) && (csTokenType != CsTokenType.OpenParenthesis))) && (((csTokenType != CsTokenType.CloseSquareBracket) && (csTokenType != CsTokenType.OpenSquareBracket)) && ((csTokenType != CsTokenType.Semicolon) && (csTokenType != CsTokenType.Comma)))) && (((csTokenType != CsTokenType.CloseGenericBracket) && (next.Value.Text != "++")) && ((next.Value.Text != "--") && !next.Value.Text.StartsWith(".", StringComparison.Ordinal))))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingSquareBracketsMustBeSpacedCorrectly, new object[0]);
                }
                if (csTokenType == CsTokenType.WhiteSpace)
                {
                    foreach (CsToken token in tokens.ForwardIterator(tokenNode.Next.Next))
                    {
                        CsTokenType type2 = token.CsTokenType;
                        switch (type2)
                        {
                            case CsTokenType.CloseParenthesis:
                            case CsTokenType.OpenParenthesis:
                            case CsTokenType.CloseSquareBracket:
                            case CsTokenType.OpenSquareBracket:
                            case CsTokenType.Semicolon:
                            case CsTokenType.Comma:
                            {
                                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingSquareBracketsMustBeSpacedCorrectly, new object[0]);
                                continue;
                            }
                        }
                        if (type2 != CsTokenType.WhiteSpace)
                        {
                            return;
                        }
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckGenericSpacing(DocumentRoot root, GenericType generic)
        {
            if (generic.ChildTokens.Count > 0)
            {
                for (Node<CsToken> node = generic.ChildTokens.First; node != null; node = node.Next)
                {
                    OperatorSymbol symbol;
                    if (base.Cancel)
                    {
                        return;
                    }
                    if (node.Value.CsTokenClass == CsTokenClass.GenericType)
                    {
                        this.CheckGenericSpacing(root, node.Value as GenericType);
                    }
                    if (!node.Value.Generated)
                    {
                        switch (node.Value.CsTokenType)
                        {
                            case CsTokenType.OpenParenthesis:
                                this.CheckOpenParen(root, generic.ChildTokens, node);
                                goto Label_0169;

                            case CsTokenType.CloseParenthesis:
                                this.CheckCloseParen(root, generic.ChildTokens, node);
                                goto Label_0169;

                            case CsTokenType.OpenCurlyBracket:
                            case CsTokenType.CloseCurlyBracket:
                            case CsTokenType.BaseColon:
                            case CsTokenType.WhereColon:
                            case CsTokenType.AttributeColon:
                            case CsTokenType.LabelColon:
                            case CsTokenType.Other:
                            case CsTokenType.EndOfLine:
                                goto Label_0169;

                            case CsTokenType.OpenSquareBracket:
                                this.CheckOpenSquareBracket(root, node);
                                goto Label_0169;

                            case CsTokenType.CloseSquareBracket:
                                this.CheckCloseSquareBracket(root, generic.ChildTokens, node);
                                goto Label_0169;

                            case CsTokenType.OpenGenericBracket:
                                this.CheckGenericTokenOpenBracket(root, node);
                                goto Label_0169;

                            case CsTokenType.CloseGenericBracket:
                                this.CheckGenericTokenCloseBracket(root, node);
                                goto Label_0169;

                            case CsTokenType.OperatorSymbol:
                                goto Label_013B;

                            case CsTokenType.Comma:
                                this.CheckSemicolonAndComma(root, node);
                                goto Label_0169;

                            case CsTokenType.WhiteSpace:
                                this.CheckWhitespace(root, node);
                                goto Label_0169;

                            case CsTokenType.PreprocessorDirective:
                                goto Label_012C;
                        }
                    }
                    goto Label_0169;
                Label_012C:
                    this.CheckPreprocessorSpacing(root, node.Value);
                    goto Label_0169;
                Label_013B:
                    symbol = node.Value as OperatorSymbol;
                    if ((symbol.SymbolType == OperatorType.MemberAccess) || (symbol.SymbolType == OperatorType.QualifiedAlias))
                    {
                        this.CheckMemberAccessSymbol(root, generic.ChildTokens, node);
                    }
                Label_0169:;
                }
            }
        }

        private void CheckGenericTokenCloseBracket(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && ((previous.Value.CsTokenType == CsTokenType.WhiteSpace) || (previous.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingGenericBracketsMustBeSpacedCorrectly, new object[0]);
            }
        }

        private void CheckGenericTokenOpenBracket(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && ((previous.Value.CsTokenType == CsTokenType.WhiteSpace) || (previous.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningGenericBracketsMustBeSpacedCorrectly, new object[0]);
            }
            Node<CsToken> next = tokenNode.Next;
            if ((next != null) && ((next.Value.CsTokenType == CsTokenType.WhiteSpace) || (next.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningGenericBracketsMustBeSpacedCorrectly, new object[0]);
            }
        }

        private void CheckIncrementDecrement(DocumentRoot root, Node<CsToken> tokenNode)
        {
            bool flag = false;
            bool flag2 = false;
            Node<CsToken> previous = tokenNode.Previous;
            if (previous == null)
            {
                flag = true;
            }
            else
            {
                switch (previous.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        flag = true;
                        break;
                }
            }
            Node<CsToken> next = tokenNode.Next;
            if (next == null)
            {
                flag2 = true;
            }
            else
            {
                switch (next.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        flag2 = true;
                        break;
                }
            }
            if (!flag && !flag2)
            {
                if ((previous == null) || ((previous.Value.CsTokenType != CsTokenType.OpenSquareBracket) && (previous.Value.CsTokenType != CsTokenType.OpenParenthesis)))
                {
                    if (next != null)
                    {
                        switch (next.Value.CsTokenType)
                        {
                            case CsTokenType.CloseSquareBracket:
                            case CsTokenType.CloseParenthesis:
                            case CsTokenType.Comma:
                            case CsTokenType.Semicolon:
                                return;
                        }
                    }
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.IncrementDecrementSymbolsMustBeSpacedCorrectly, new object[0]);
                }
            }
            else if (flag && flag2)
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.IncrementDecrementSymbolsMustBeSpacedCorrectly, new object[0]);
            }
        }

        private void CheckKeywordWithoutSpace(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> next = tokenNode.Next;
            if ((next != null) && ((next.Value.CsTokenType == CsTokenType.WhiteSpace) || (next.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                foreach (CsToken token in tokens.ForwardIterator(next.Next))
                {
                    if (token.CsTokenType == CsTokenType.OpenParenthesis)
                    {
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.KeywordsMustBeSpacedCorrectly, new object[] { tokenNode.Value.Text });
                        break;
                    }
                    if ((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine))
                    {
                        break;
                    }
                }
            }
        }

        private void CheckKeywordWithSpace(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> next = tokenNode.Next;
            if ((next == null) || (((next.Value.CsTokenType != CsTokenType.WhiteSpace) && (next.Value.CsTokenType != CsTokenType.EndOfLine)) && ((next.Value.CsTokenType != CsTokenType.Semicolon) && (next.Value.CsTokenType != CsTokenType.AttributeColon))))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.KeywordsMustBeSpacedCorrectly, new object[] { tokenNode.Value.Text });
            }
        }

        private void CheckLabelColon(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> next = tokenNode.Next;
            if (next == null)
            {
                CsTokenType csTokenType = next.Value.CsTokenType;
                if ((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ColonsMustBeSpacedCorrectly, new object[0]);
                }
            }
            Node<CsToken> previous = tokenNode.Previous;
            if (previous != null)
            {
                switch (previous.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ColonsMustBeSpacedCorrectly, new object[0]);
                        break;
                }
            }
        }

        private void CheckMemberAccessSymbol(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if (previous == null)
            {
                switch (previous.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.MemberAccessSymbolsMustBeSpacedCorrectly, new object[0]);
                        break;
                }
            }
            Node<CsToken> next = tokenNode.Next;
            if (next == null)
            {
                switch (next.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        if (previous != null)
                        {
                            foreach (CsToken token in tokens.ReverseIterator(previous))
                            {
                                CsTokenType csTokenType = token.CsTokenType;
                                if (csTokenType == CsTokenType.Operator)
                                {
                                    return;
                                }
                                if (((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.SingleLineComment) && (csTokenType != CsTokenType.MultiLineComment)))
                                {
                                    break;
                                }
                            }
                        }
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.MemberAccessSymbolsMustBeSpacedCorrectly, new object[0]);
                        break;
                }
            }
        }

        private void CheckNegativeSign(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if (previous != null)
            {
                CsTokenType csTokenType = previous.Value.CsTokenType;
                if ((((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.OpenParenthesis) && (csTokenType != CsTokenType.OpenSquareBracket))) && (csTokenType != CsTokenType.CloseParenthesis))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.NegativeSignsMustBeSpacedCorrectly, new object[0]);
                }
            }
            Node<CsToken> next = tokenNode.Next;
            if (next != null)
            {
                switch (next.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.NegativeSignsMustBeSpacedCorrectly, new object[0]);
                        break;
                }
            }
        }

        private void CheckNewKeywordSpacing(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> next = tokenNode.Next;
            if (next != null)
            {
                if ((next.Value.CsTokenType == CsTokenType.WhiteSpace) || (next.Value.CsTokenType == CsTokenType.EndOfLine))
                {
                    foreach (CsToken token in tokens.ForwardIterator(next.Next))
                    {
                        if (token.CsTokenType == CsTokenType.OpenSquareBracket)
                        {
                            base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation, new object[0]);
                            break;
                        }
                        if ((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine))
                        {
                            break;
                        }
                    }
                }
                else if (next.Value.CsTokenType != CsTokenType.OpenSquareBracket)
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.KeywordsMustBeSpacedCorrectly, new object[] { tokenNode.Value.Text });
                }
            }
        }

        private void CheckNullableTypeSymbol(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && ((previous.Value.CsTokenType == CsTokenType.WhiteSpace) || (previous.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.NullableTypeSymbolsMustNotBePrecededBySpace, new object[0]);
            }
        }

        private void CheckOpenCurlyBracket(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if (previous != null)
            {
                CsTokenType csTokenType = previous.Value.CsTokenType;
                if (((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && (csTokenType != CsTokenType.OpenParenthesis))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningCurlyBracketsMustBeSpacedCorrectly, new object[0]);
                }
                if (csTokenType == CsTokenType.WhiteSpace)
                {
                    foreach (CsToken token in tokens.ReverseIterator(previous))
                    {
                        CsTokenType type2 = token.CsTokenType;
                        if (type2 == CsTokenType.OpenParenthesis)
                        {
                            base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningCurlyBracketsMustBeSpacedCorrectly, new object[0]);
                        }
                        else if (type2 != CsTokenType.WhiteSpace)
                        {
                            break;
                        }
                    }
                }
            }
            Node<CsToken> next = tokenNode.Next;
            if (((next != null) && (next.Value.CsTokenType != CsTokenType.WhiteSpace)) && (next.Value.CsTokenType != CsTokenType.EndOfLine))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningCurlyBracketsMustBeSpacedCorrectly, new object[0]);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckOpenParen(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> node2;
            bool flag = false;
            bool flag2 = false;
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && (previous.Value.CsTokenType == CsTokenType.WhiteSpace))
            {
                foreach (CsToken token in tokens.ReverseIterator(previous))
                {
                    switch (token.CsTokenType)
                    {
                        case CsTokenType.WhiteSpace:
                        {
                            continue;
                        }
                        case CsTokenType.EndOfLine:
                            flag = true;
                            goto Label_017C;

                        case CsTokenType.Case:
                        case CsTokenType.Catch:
                        case CsTokenType.CloseSquareBracket:
                        case CsTokenType.Comma:
                        case CsTokenType.Equals:
                        case CsTokenType.Fixed:
                        case CsTokenType.For:
                        case CsTokenType.Foreach:
                        case CsTokenType.From:
                        case CsTokenType.Group:
                        case CsTokenType.If:
                        case CsTokenType.In:
                        case CsTokenType.Into:
                        case CsTokenType.Join:
                        case CsTokenType.Let:
                        case CsTokenType.Lock:
                        case CsTokenType.MultiLineComment:
                        case CsTokenType.Number:
                        case CsTokenType.OperatorSymbol:
                        case CsTokenType.OpenCurlyBracket:
                        case CsTokenType.OrderBy:
                        case CsTokenType.Return:
                        case CsTokenType.Select:
                        case CsTokenType.Semicolon:
                        case CsTokenType.Switch:
                        case CsTokenType.Throw:
                        case CsTokenType.Using:
                        case CsTokenType.Where:
                        case CsTokenType.While:
                        case CsTokenType.WhileDo:
                        case CsTokenType.Yield:
                            goto Label_017C;
                    }
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningParenthesisMustBeSpacedCorrectly, new object[0]);
                }
            }
        Label_017C:
            node2 = tokenNode.Next;
            if ((node2 != null) && ((node2.Value.CsTokenType == CsTokenType.WhiteSpace) || (node2.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                foreach (CsToken token2 in tokens.ForwardIterator(node2))
                {
                    CsTokenType csTokenType = token2.CsTokenType;
                    if (csTokenType == CsTokenType.EndOfLine)
                    {
                        flag2 = true;
                        break;
                    }
                    if (((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.SingleLineComment)) && (csTokenType != CsTokenType.MultiLineComment))
                    {
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningParenthesisMustBeSpacedCorrectly, new object[0]);
                        break;
                    }
                }
            }
            if (flag && flag2)
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningParenthesisMustBeSpacedCorrectly, new object[0]);
            }
        }

        private void CheckOpenSquareBracket(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if ((previous != null) && ((previous.Value.CsTokenType == CsTokenType.WhiteSpace) || (previous.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningSquareBracketsMustBeSpacedCorrectly, new object[0]);
            }
            Node<CsToken> next = tokenNode.Next;
            if ((next != null) && ((next.Value.CsTokenType == CsTokenType.WhiteSpace) || (next.Value.CsTokenType == CsTokenType.EndOfLine)))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningSquareBracketsMustBeSpacedCorrectly, new object[0]);
            }
        }

        private void CheckOperatorKeyword(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> next = tokenNode.Next;
            if ((next != null) && (next.Value.CsTokenType != CsTokenType.WhiteSpace))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OperatorKeywordMustBeFollowedBySpace, new object[0]);
            }
        }

        private void CheckPositiveSign(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if (previous != null)
            {
                CsTokenType csTokenType = previous.Value.CsTokenType;
                if ((((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.OpenParenthesis) && (csTokenType != CsTokenType.OpenSquareBracket))) && (csTokenType != CsTokenType.CloseParenthesis))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.PositiveSignsMustBeSpacedCorrectly, new object[0]);
                }
            }
            Node<CsToken> next = tokenNode.Next;
            if (next != null)
            {
                switch (next.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.PositiveSignsMustBeSpacedCorrectly, new object[0]);
                        break;
                }
            }
        }

        private void CheckPreprocessorSpacing(DocumentRoot root, CsToken preprocessor)
        {
            if (((preprocessor.Text.Length > 1) && (preprocessor.Text[0] == '#')) && ((preprocessor.Text[1] == ' ') || (preprocessor.Text[1] == '\t')))
            {
                base.AddViolation(root, preprocessor.LineNumber, Microsoft.StyleCop.CSharp.Rules.PreprocessorKeywordsMustNotBePrecededBySpace, new object[0]);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckSemicolonAndComma(DocumentRoot root, Node<CsToken> tokenNode)
        {
            bool flag = false;
            if (tokenNode.Value.Text == ",")
            {
                flag = true;
            }
            string[] strArray = new string[] { "[", "<" };
            string[] strArray2 = new string[] { "]", ">" };
            if (!flag)
            {
                strArray = new string[] { "(" };
                strArray2 = new string[] { ")" };
            }
            bool flag2 = true;
            bool flag3 = true;
            bool flag4 = false;
            Node<CsToken> previous = tokenNode.Previous;
            if (previous != null)
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (previous.Value.Text == strArray[i])
                    {
                        flag4 = true;
                        break;
                    }
                }
                if (!flag4)
                {
                    if (previous.Value.Text == tokenNode.Value.Text)
                    {
                        flag4 = true;
                    }
                    else
                    {
                        flag2 = false;
                    }
                }
            }
            if (!flag4)
            {
                flag2 = false;
            }
            flag4 = false;
            previous = tokenNode.Next;
            if (previous != null)
            {
                for (int j = 0; j < strArray2.Length; j++)
                {
                    if (previous.Value.Text == strArray2[j])
                    {
                        flag4 = true;
                        break;
                    }
                }
                if (!flag4)
                {
                    if (previous.Value.Text == tokenNode.Value.Text)
                    {
                        flag4 = true;
                    }
                    else
                    {
                        flag3 = false;
                    }
                }
            }
            if (!flag4)
            {
                flag3 = false;
            }
            if (!flag2)
            {
                Node<CsToken> node2 = tokenNode.Previous;
                if ((node2 != null) && ((node2.Value.CsTokenType == CsTokenType.WhiteSpace) || (node2.Value.CsTokenType == CsTokenType.EndOfLine)))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, flag ? Microsoft.StyleCop.CSharp.Rules.CommasMustBeSpacedCorrectly : Microsoft.StyleCop.CSharp.Rules.SemicolonsMustBeSpacedCorrectly, new object[0]);
                }
            }
            if (!flag3)
            {
                Node<CsToken> next = tokenNode.Next;
                if (((next != null) && (next.Value.CsTokenType != CsTokenType.WhiteSpace)) && ((next.Value.CsTokenType != CsTokenType.EndOfLine) && (next.Value.CsTokenType != CsTokenType.CloseParenthesis)))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, flag ? Microsoft.StyleCop.CSharp.Rules.CommasMustBeSpacedCorrectly : Microsoft.StyleCop.CSharp.Rules.SemicolonsMustBeSpacedCorrectly, new object[0]);
                }
            }
        }

        private void CheckSingleLineComment(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            if (tokenNode.Value.Text.Length > 2)
            {
                string text = tokenNode.Value.Text;
                if ((((text[2] != ' ') && (text[2] != '\t')) && ((text[2] != '/') && (text[2] != '\\'))) && (((text[1] != '\n') && (text[1] != '\r')) && (((text.Length < 4) || (text[2] != '-')) || (text[3] != '-'))))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.SingleLineCommentsMustBeginWithSingleSpace, new object[0]);
                }
                else if (((text.Length > 3) && ((text[3] == ' ') || (text[3] == '\t'))) && (text[2] != '\\'))
                {
                    bool flag = true;
                    int num = 0;
                    foreach (CsToken token in tokens.ReverseIterator(tokenNode.Previous))
                    {
                        if (token.CsTokenType == CsTokenType.EndOfLine)
                        {
                            if (++num != 2)
                            {
                                continue;
                            }
                            break;
                        }
                        if (token.CsTokenType == CsTokenType.SingleLineComment)
                        {
                            flag = false;
                            break;
                        }
                        if (token.CsTokenType != CsTokenType.WhiteSpace)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.SingleLineCommentsMustBeginWithSingleSpace, new object[0]);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release."), SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification="Minimizing refactoring before release.")]
        private void CheckSpacing(DocumentRoot root, MasterList<CsToken> tokens, bool type)
        {
            if (tokens.Count > 0)
            {
                for (Node<CsToken> node = tokens.First; node != null; node = node.Next)
                {
                    OperatorSymbol symbol;
                    CsTokenClass class2;
                    if (base.Cancel)
                    {
                        return;
                    }
                    if (node.Value.Generated)
                    {
                        goto Label_049F;
                    }
                    switch (node.Value.CsTokenType)
                    {
                        case CsTokenType.OpenParenthesis:
                            this.CheckOpenParen(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.CloseParenthesis:
                            this.CheckCloseParen(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.OpenCurlyBracket:
                            this.CheckOpenCurlyBracket(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.CloseCurlyBracket:
                            this.CheckCloseCurlyBracket(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.OpenSquareBracket:
                            this.CheckOpenSquareBracket(root, node);
                            goto Label_043E;

                        case CsTokenType.CloseSquareBracket:
                            this.CheckCloseSquareBracket(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.OperatorSymbol:
                            symbol = node.Value as OperatorSymbol;
                            switch (symbol.Category)
                            {
                                case OperatorCategory.Relational:
                                case OperatorCategory.Logical:
                                case OperatorCategory.Assignment:
                                case OperatorCategory.Arithmetic:
                                case OperatorCategory.Shift:
                                case OperatorCategory.Conditional:
                                case OperatorCategory.Lambda:
                                    goto Label_03F7;

                                case OperatorCategory.IncrementDecrement:
                                    goto Label_0402;

                                case OperatorCategory.Unary:
                                    goto Label_040C;
                            }
                            goto Label_043E;

                        case CsTokenType.BaseColon:
                        case CsTokenType.WhereColon:
                            this.CheckSymbol(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.AttributeColon:
                        case CsTokenType.LabelColon:
                            this.CheckLabelColon(root, node);
                            goto Label_043E;

                        case CsTokenType.Comma:
                        case CsTokenType.Semicolon:
                            this.CheckSemicolonAndComma(root, node);
                            goto Label_043E;

                        case CsTokenType.NullableTypeSymbol:
                            this.CheckNullableTypeSymbol(root, node);
                            goto Label_043E;

                        case CsTokenType.Catch:
                        case CsTokenType.Fixed:
                        case CsTokenType.For:
                        case CsTokenType.Foreach:
                        case CsTokenType.From:
                        case CsTokenType.Group:
                        case CsTokenType.If:
                        case CsTokenType.In:
                        case CsTokenType.Into:
                        case CsTokenType.Join:
                        case CsTokenType.Let:
                        case CsTokenType.Lock:
                        case CsTokenType.OrderBy:
                        case CsTokenType.Return:
                        case CsTokenType.Select:
                        case CsTokenType.Stackalloc:
                        case CsTokenType.Switch:
                        case CsTokenType.Throw:
                        case CsTokenType.Using:
                        case CsTokenType.Where:
                        case CsTokenType.While:
                        case CsTokenType.WhileDo:
                        case CsTokenType.Yield:
                            this.CheckKeywordWithSpace(root, node);
                            goto Label_043E;

                        case CsTokenType.Checked:
                        case CsTokenType.DefaultValue:
                        case CsTokenType.Sizeof:
                        case CsTokenType.Typeof:
                        case CsTokenType.Unchecked:
                            this.CheckKeywordWithoutSpace(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.New:
                            this.CheckNewKeywordSpacing(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.Operator:
                            this.CheckOperatorKeyword(root, node);
                            goto Label_043E;

                        case CsTokenType.WhiteSpace:
                            this.CheckWhitespace(root, node);
                            goto Label_043E;

                        case CsTokenType.SingleLineComment:
                            this.CheckTabsInComment(root, node.Value);
                            this.CheckSingleLineComment(root, tokens, node);
                            goto Label_043E;

                        case CsTokenType.MultiLineComment:
                            this.CheckTabsInComment(root, node.Value);
                            goto Label_043E;

                        case CsTokenType.PreprocessorDirective:
                            this.CheckPreprocessorSpacing(root, node.Value as Preprocessor);
                            goto Label_043E;

                        case CsTokenType.Attribute:
                        {
                            Microsoft.StyleCop.CSharp.Attribute attribute = node.Value as Microsoft.StyleCop.CSharp.Attribute;
                            this.CheckSpacing(root, attribute.ChildTokens, false);
                            goto Label_043E;
                        }
                        case CsTokenType.OpenAttributeBracket:
                            this.CheckAttributeTokenOpenBracket(root, node);
                            goto Label_043E;

                        case CsTokenType.CloseAttributeBracket:
                            this.CheckAttributeTokenCloseBracket(root, node);
                            goto Label_043E;

                        case CsTokenType.XmlHeader:
                        {
                            XmlHeader header = (XmlHeader) node.Value;
                            this.CheckXmlHeaderComment(root, header);
                            for (Node<CsToken> node2 = header.ChildTokens.First; node2 != null; node2 = node2.Next)
                            {
                                this.CheckTabsInComment(root, node2.Value);
                            }
                            goto Label_043E;
                        }
                        default:
                            goto Label_043E;
                    }
                Label_03F7:
                    this.CheckSymbol(root, tokens, node);
                    goto Label_043E;
                Label_0402:
                    this.CheckIncrementDecrement(root, node);
                    goto Label_043E;
                Label_040C:
                    if (symbol.SymbolType == OperatorType.Negative)
                    {
                        this.CheckNegativeSign(root, node);
                    }
                    else if (symbol.SymbolType == OperatorType.Positive)
                    {
                        this.CheckPositiveSign(root, node);
                    }
                    else
                    {
                        this.CheckUnarySymbol(root, node);
                    }
                Label_043E:
                    class2 = node.Value.CsTokenClass;
                    if (class2 != CsTokenClass.GenericType)
                    {
                        if (class2 == CsTokenClass.Type)
                        {
                            goto Label_0487;
                        }
                        if (class2 == CsTokenClass.ConstructorConstraint)
                        {
                            this.CheckSpacing(root, ((ConstructorConstraint) node.Value).ChildTokens, false);
                        }
                        goto Label_049F;
                    }
                    this.CheckGenericSpacing(root, node.Value as GenericType);
                Label_0487:
                    this.CheckSpacing(root, ((TypeToken) node.Value).ChildTokens, true);
                Label_049F:;
                }
            }
        }

        private void CheckSymbol(DocumentRoot root, MasterList<CsToken> tokens, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if (((previous != null) && (previous.Value.CsTokenType != CsTokenType.WhiteSpace)) && (previous.Value.CsTokenType != CsTokenType.EndOfLine))
            {
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.SymbolsMustBeSpacedCorrectly, new object[] { tokenNode.Value.Text });
            }
            Node<CsToken> next = tokenNode.Next;
            if (((next != null) && (next.Value.CsTokenType != CsTokenType.WhiteSpace)) && (next.Value.CsTokenType != CsTokenType.EndOfLine))
            {
                if (previous != null)
                {
                    foreach (CsToken token in tokens.ReverseIterator(previous))
                    {
                        if (token.CsTokenType == CsTokenType.Operator)
                        {
                            return;
                        }
                        if (((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine)) && (((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment)) && (token.CsTokenType != CsTokenType.PreprocessorDirective)))
                        {
                            break;
                        }
                    }
                }
                base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.SymbolsMustBeSpacedCorrectly, new object[] { tokenNode.Value.Text });
            }
        }

        private void CheckTabsInComment(CsElement element, CsToken comment)
        {
            int num = 0;
            for (int i = 0; i < comment.Text.Length; i++)
            {
                if (comment.Text[i] == '\t')
                {
                    base.AddViolation(element, comment.LineNumber + num, Microsoft.StyleCop.CSharp.Rules.TabsMustNotBeUsed, new object[0]);
                }
                else if (comment.Text[i] == '\n')
                {
                    num++;
                }
            }
        }

        private void CheckUnarySymbol(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Node<CsToken> previous = tokenNode.Previous;
            if (previous != null)
            {
                CsTokenType csTokenType = previous.Value.CsTokenType;
                if (((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.OpenParenthesis) && (csTokenType != CsTokenType.OpenSquareBracket)))
                {
                    base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.SymbolsMustBeSpacedCorrectly, new object[] { tokenNode.Value.Text });
                }
            }
            Node<CsToken> next = tokenNode.Next;
            if (next == null)
            {
                switch (next.Value.CsTokenType)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.SymbolsMustBeSpacedCorrectly, new object[] { tokenNode.Value.Text });
                        break;
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckUnsafeAccessSymbols(DocumentRoot root, Node<CsToken> tokenNode, bool type)
        {
            if (type)
            {
                Node<CsToken> next = tokenNode.Next;
                if (next != null)
                {
                    CsTokenType csTokenType = next.Value.CsTokenType;
                    if ((((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.OpenParenthesis) && (csTokenType != CsTokenType.OpenSquareBracket))) && ((csTokenType != CsTokenType.CloseParenthesis) && (csTokenType != tokenNode.Value.CsTokenType)))
                    {
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly, new object[0]);
                    }
                }
                Node<CsToken> previous = tokenNode.Previous;
                if (previous != null)
                {
                    switch (previous.Value.CsTokenType)
                    {
                        case CsTokenType.WhiteSpace:
                        case CsTokenType.EndOfLine:
                        case CsTokenType.SingleLineComment:
                        case CsTokenType.MultiLineComment:
                            base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly, new object[0]);
                            return;
                    }
                }
            }
            else
            {
                Node<CsToken> node3 = tokenNode.Previous;
                if (node3 != null)
                {
                    CsTokenType type4 = node3.Value.CsTokenType;
                    if ((((type4 != CsTokenType.WhiteSpace) && (type4 != CsTokenType.EndOfLine)) && ((type4 != CsTokenType.OpenParenthesis) && (type4 != CsTokenType.OpenSquareBracket))) && ((type4 != CsTokenType.CloseParenthesis) && (type4 != tokenNode.Value.CsTokenType)))
                    {
                        base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly, new object[0]);
                    }
                }
                Node<CsToken> node4 = tokenNode.Next;
                if (node4 == null)
                {
                    switch (node4.Value.CsTokenType)
                    {
                        case CsTokenType.WhiteSpace:
                        case CsTokenType.EndOfLine:
                        case CsTokenType.SingleLineComment:
                        case CsTokenType.MultiLineComment:
                            base.AddViolation(root, tokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly, new object[0]);
                            break;
                    }
                }
            }
        }

        private void CheckWhitespace(DocumentRoot root, Node<CsToken> tokenNode)
        {
            Whitespace whitespace = (Whitespace) tokenNode.Value;
            if (whitespace.TabCount > 0)
            {
                base.AddViolation(root, whitespace.LineNumber, Microsoft.StyleCop.CSharp.Rules.TabsMustNotBeUsed, new object[0]);
            }
            else if ((whitespace.TabCount == 0) && (whitespace.SpaceCount > 1))
            {
                Node<CsToken> next = tokenNode.Next;
                Node<CsToken> previous = tokenNode.Previous;
                if (((((previous != null) && (previous.Value.CsTokenType != CsTokenType.EndOfLine)) && ((previous.Value.CsTokenType != CsTokenType.Comma) && (previous.Value.CsTokenType != CsTokenType.Semicolon))) && (((next != null) && (next.Value.CsTokenType != CsTokenType.OperatorSymbol)) && ((next.Value.CsTokenType != CsTokenType.EndOfLine) && (next.Value.CsTokenType != CsTokenType.SingleLineComment)))) && (next.Value.CsTokenType != CsTokenType.MultiLineComment))
                {
                    base.AddViolation(root, whitespace.LineNumber, Microsoft.StyleCop.CSharp.Rules.CodeMustNotContainMultipleWhitespaceInARow, new object[0]);
                }
            }
        }

        private void CheckXmlHeaderComment(DocumentRoot root, XmlHeader header)
        {
            for (Node<CsToken> node = header.ChildTokens.First; node != null; node = node.Next)
            {
                CsToken token = node.Value;
                if ((token.CsTokenType == CsTokenType.XmlHeaderLine) && (token.Text.Length > 3))
                {
                    if ((((token.Text[3] != ' ') && (token.Text[3] != '\t')) && ((token.Text[3] != '/') && (token.Text[2] != '\n'))) && (token.Text[2] != '\r'))
                    {
                        base.AddViolation(root, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationLinesMustBeginWithSingleSpace, new object[0]);
                        continue;
                    }
                    if ((token.Text.Length > 4) && ((token.Text[4] == ' ') || (token.Text[4] == '\t')))
                    {
                        bool flag = true;
                        for (Node<CsToken> node2 = node.Previous; node2 != null; node2 = node2.Previous)
                        {
                            if (node2.Value.CsTokenType == CsTokenType.XmlHeaderLine)
                            {
                                for (Node<CsToken> node3 = node.Next; node3 != null; node3 = node3.Next)
                                {
                                    if (node3.Value.CsTokenType == CsTokenType.XmlHeaderLine)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        if (flag)
                        {
                            base.AddViolation(root, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.DocumentationLinesMustBeginWithSingleSpace, new object[0]);
                        }
                    }
                }
            }
        }
    }
}

