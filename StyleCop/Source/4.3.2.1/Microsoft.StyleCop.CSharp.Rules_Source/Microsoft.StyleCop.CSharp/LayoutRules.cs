namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SourceAnalyzer(typeof(CsParser))]
    public class LayoutRules : SourceAnalyzer
    {
        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            CsDocument document2 = (CsDocument) document;
            if ((document2.RootElement != null) && !document2.RootElement.Generated)
            {
                document2.WalkDocument(new CodeWalkerElementVisitor<object>(this.VisitElement), new CodeWalkerStatementVisitor<object>(this.CheckStatementCurlyBracketPlacement), new CodeWalkerExpressionVisitor<object>(this.CheckExpressionCurlyBracketPlacement));
                this.CheckLineSpacing(document2);
            }
        }

        private static bool BracketSharesLine(Microsoft.StyleCop.Node<CsToken> bracketNode, bool allowTrailingCharacters)
        {
            bool flag = false;
            CsToken token = null;
            for (Microsoft.StyleCop.Node<CsToken> node = bracketNode.Next; node != null; node = node.Next)
            {
                CsToken token2 = node.Value;
                if (token2.CsTokenType == CsTokenType.EndOfLine)
                {
                    break;
                }
                if (((token2.CsTokenType != CsTokenType.WhiteSpace) && (token2.CsTokenType != CsTokenType.SingleLineComment)) && (token2.CsTokenType != CsTokenType.MultiLineComment))
                {
                    token = token2;
                    break;
                }
            }
            if ((token != null) && (!allowTrailingCharacters || (((token.CsTokenType != CsTokenType.Semicolon) && (token.CsTokenType != CsTokenType.Comma)) && ((token.CsTokenType != CsTokenType.CloseParenthesis) && (token.CsTokenType != CsTokenType.CloseSquareBracket)))))
            {
                flag = true;
            }
            if (!flag)
            {
                for (Microsoft.StyleCop.Node<CsToken> node2 = bracketNode.Previous; node2 != null; node2 = node2.Previous)
                {
                    CsToken token3 = node2.Value;
                    if (token3.CsTokenType == CsTokenType.EndOfLine)
                    {
                        return flag;
                    }
                    if ((token3.CsTokenType != CsTokenType.WhiteSpace) && (token3.CsTokenType != CsTokenType.SingleLineComment))
                    {
                        return true;
                    }
                }
            }
            return flag;
        }

        private void CheckBracketPlacement(CsElement parentElement, Statement parentStatement, CsTokenList tokens, Microsoft.StyleCop.Node<CsToken> openBracketNode, bool allowAllOnOneLine)
        {
            Bracket bracket = (Bracket) openBracketNode.Value;
            if (((bracket.MatchingBracket != null) && !bracket.Generated) && !bracket.MatchingBracket.Generated)
            {
                if (bracket.LineNumber == bracket.MatchingBracket.LineNumber)
                {
                    if (allowAllOnOneLine)
                    {
                        if (tokens.First.Value.LineNumber != tokens.Last.Value.LineNumber)
                        {
                            base.AddViolation(parentElement, bracket.LineNumber, Microsoft.StyleCop.CSharp.Rules.CurlyBracketsForMultiLineStatementsMustNotShareLine, new object[] { GetOpeningOrClosingBracketText(bracket) });
                        }
                    }
                    else if (parentStatement == null)
                    {
                        base.AddViolation(parentElement, bracket.LineNumber, Microsoft.StyleCop.CSharp.Rules.ElementMustNotBeOnSingleLine, new object[] { parentElement.FriendlyTypeText });
                    }
                    else if (parentStatement.StatementType != StatementType.ConstructorInitializer)
                    {
                        base.AddViolation(parentElement, bracket.LineNumber, Microsoft.StyleCop.CSharp.Rules.StatementMustNotBeOnSingleLine, new object[0]);
                    }
                }
                else
                {
                    if (BracketSharesLine(openBracketNode, false))
                    {
                        base.AddViolation(parentElement, bracket.LineNumber, Microsoft.StyleCop.CSharp.Rules.CurlyBracketsForMultiLineStatementsMustNotShareLine, new object[] { GetOpeningOrClosingBracketText(bracket) });
                    }
                    if (BracketSharesLine(bracket.MatchingBracketNode, true))
                    {
                        base.AddViolation(parentElement, bracket.MatchingBracket.LineNumber, Microsoft.StyleCop.CSharp.Rules.CurlyBracketsForMultiLineStatementsMustNotShareLine, new object[] { GetOpeningOrClosingBracketText(bracket.MatchingBracket) });
                    }
                }
            }
        }

        private void CheckChildElementSpacing(CsElement element)
        {
            CsElement element2 = null;
            if ((element.ChildElements != null) && (element.ChildElements.Count > 0))
            {
                foreach (CsElement element3 in element.ChildElements)
                {
                    if ((((element2 != null) && !element2.Generated) && !element3.Generated) && ((((element2.ElementType != element3.ElementType) || (element3.Header != null)) || (element2.Location.LineSpan > 1)) || ((((element3.ElementType != ElementType.UsingDirective) && (element3.ElementType != ElementType.ExternAliasDirective)) && ((element3.ElementType != ElementType.Accessor) && (element3.ElementType != ElementType.EnumItem))) && (element3.ElementType != ElementType.Field))))
                    {
                        int lineNumber = element3.LineNumber;
                        if (element3.Header != null)
                        {
                            lineNumber = element3.Header.LineNumber;
                        }
                        if ((lineNumber == element2.Location.EndPoint.LineNumber) || (lineNumber == (element2.Location.EndPoint.LineNumber + 1)))
                        {
                            base.AddViolation(element3, Microsoft.StyleCop.CSharp.Rules.ElementsMustBeSeparatedByBlankLine, new object[0]);
                        }
                    }
                    element2 = element3;
                }
            }
        }

        private void CheckElementBracketPlacement(CsElement element, bool allowAllOnOneLine)
        {
            CsToken token = null;
            if (element.Declaration.Tokens.First != null)
            {
                token = element.Declaration.Tokens.Last.Value;
            }
            bool flag = false;
            for (Microsoft.StyleCop.Node<CsToken> node = element.Tokens.First; !element.Tokens.OutOfBounds(node); node = node.Next)
            {
                if ((token == null) || flag)
                {
                    if (node.Value.CsTokenType == CsTokenType.Equals)
                    {
                        allowAllOnOneLine = true;
                    }
                    else if (node.Value.CsTokenType == CsTokenType.OpenCurlyBracket)
                    {
                        this.CheckBracketPlacement(element, null, element.Tokens, node, allowAllOnOneLine);
                        if (allowAllOnOneLine && (element.ElementType == ElementType.Accessor))
                        {
                            this.CheckSiblingAccessors(element, node);
                            return;
                        }
                        break;
                    }
                }
                if (!flag)
                {
                    flag = node.Value == token;
                }
            }
        }

        private void CheckElementCurlyBracketPlacement(CsElement element)
        {
            if (!element.Generated)
            {
                if (element.ElementType == ElementType.Accessor)
                {
                    this.CheckElementBracketPlacement(element, true);
                }
                else if ((((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Constructor)) || ((element.ElementType == ElementType.Destructor) || (element.ElementType == ElementType.Enum))) || ((((element.ElementType == ElementType.Event) || (element.ElementType == ElementType.Indexer)) || ((element.ElementType == ElementType.Interface) || (element.ElementType == ElementType.Method))) || ((element.ElementType == ElementType.Namespace) || (element.ElementType == ElementType.Struct))))
                {
                    this.CheckElementBracketPlacement(element, false);
                }
                else if (element.ElementType == ElementType.Property)
                {
                    this.CheckElementBracketPlacement(element, IsAutomaticProperty((Property) element));
                }
            }
        }

        private bool CheckExpressionCurlyBracketPlacement(Expression expression, Expression parentExpression, Statement parentStatement, CsElement parentElement, object context)
        {
            switch (expression.ExpressionType)
            {
                case ExpressionType.CollectionInitializer:
                case ExpressionType.ObjectInitializer:
                case ExpressionType.ArrayInitializer:
                {
                    CsTokenList tokens = expression.Tokens;
                    Expression parent = expression.Parent as Expression;
                    if (parent != null)
                    {
                        tokens = parent.Tokens;
                    }
                    this.CheckBracketPlacement(parentElement, parentStatement, tokens, GetOpenBracket(tokens), true);
                    break;
                }
                case ExpressionType.Lambda:
                case ExpressionType.AnonymousMethod:
                {
                    Microsoft.StyleCop.Node<CsToken> openBracket = GetOpenBracket(expression.Tokens);
                    if (openBracket != null)
                    {
                        bool allowAllOnOneLine = expression.Location.StartPoint.LineNumber == expression.Location.EndPoint.LineNumber;
                        this.CheckBracketPlacement(parentElement, parentStatement, expression.Tokens, openBracket, allowAllOnOneLine);
                    }
                    break;
                }
            }
            return true;
        }

        private void CheckLineSpacing(CsDocument document)
        {
            int count = 0;
            Microsoft.StyleCop.Node<CsToken> precedingTokenNode = null;
            bool fileHeader = true;
            bool firstTokenOnLine = true;
            for (Microsoft.StyleCop.Node<CsToken> node2 = document.Tokens.First; node2 != null; node2 = node2.Next)
            {
                if (base.Cancel)
                {
                    return;
                }
                CsToken token = node2.Value;
                if (((fileHeader && (token.CsTokenType != CsTokenType.EndOfLine)) && ((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.SingleLineComment))) && (token.CsTokenType != CsTokenType.MultiLineComment))
                {
                    fileHeader = false;
                }
                if (token.Text == "\n")
                {
                    count++;
                    firstTokenOnLine = true;
                    this.CheckLineSpacingNewline(document, precedingTokenNode, token, count);
                }
                else if (token.CsTokenType != CsTokenType.WhiteSpace)
                {
                    this.CheckLineSpacingNonWhitespace(document, precedingTokenNode, token, fileHeader, firstTokenOnLine, count);
                    count = 0;
                    precedingTokenNode = node2;
                    if ((firstTokenOnLine && (token.CsTokenType != CsTokenType.SingleLineComment)) && (token.CsTokenType != CsTokenType.MultiLineComment))
                    {
                        firstTokenOnLine = false;
                    }
                }
            }
        }

        private void CheckLineSpacingNewline(CsDocument document, Microsoft.StyleCop.Node<CsToken> precedingTokenNode, CsToken token, int count)
        {
            if (count == 2)
            {
                if ((precedingTokenNode != null) && !precedingTokenNode.Value.Generated)
                {
                    if (precedingTokenNode.Value.CsTokenType == CsTokenType.OpenCurlyBracket)
                    {
                        base.AddViolation(document.RootElement, precedingTokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningCurlyBracketsMustNotBeFollowedByBlankLine, new object[0]);
                    }
                    else if (precedingTokenNode.Value.CsTokenType == CsTokenType.XmlHeader)
                    {
                        base.AddViolation(document.RootElement, precedingTokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ElementDocumentationHeadersMustNotBeFollowedByBlankLine, new object[0]);
                    }
                }
            }
            else if ((count == 3) && !token.Generated)
            {
                base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.CodeMustNotContainMultipleBlankLinesInARow, new object[0]);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckLineSpacingNonWhitespace(CsDocument document, Microsoft.StyleCop.Node<CsToken> precedingTokenNode, CsToken token, bool fileHeader, bool firstTokenOnLine, int count)
        {
            if (!token.Generated)
            {
                if (count <= 1)
                {
                    if (count == 1)
                    {
                        if (token.CsTokenType == CsTokenType.XmlHeader)
                        {
                            if (((precedingTokenNode != null) && (precedingTokenNode.Value.CsTokenType != CsTokenType.XmlHeader)) && ((precedingTokenNode.Value.CsTokenType != CsTokenType.OpenCurlyBracket) && (precedingTokenNode.Value.CsTokenType != CsTokenType.PreprocessorDirective)))
                            {
                                base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.ElementDocumentationHeaderMustBePrecededByBlankLine, new object[0]);
                            }
                        }
                        else if (token.CsTokenType == CsTokenType.SingleLineComment)
                        {
                            if ((((precedingTokenNode != null) && (precedingTokenNode.Value.CsTokenType != CsTokenType.SingleLineComment)) && ((precedingTokenNode.Value.CsTokenType != CsTokenType.OpenCurlyBracket) && (precedingTokenNode.Value.CsTokenType != CsTokenType.LabelColon))) && ((precedingTokenNode.Value.CsTokenType != CsTokenType.PreprocessorDirective) && !token.Text.Trim().StartsWith("////", StringComparison.Ordinal)))
                            {
                                base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.SingleLineCommentMustBePrecededByBlankLine, new object[0]);
                            }
                        }
                        else if ((precedingTokenNode != null) && (precedingTokenNode.Value.CsTokenType == CsTokenType.CloseCurlyBracket))
                        {
                            Bracket bracket = precedingTokenNode.Value as Bracket;
                            if (((((bracket.MatchingBracket != null) && (bracket.MatchingBracket.LineNumber != bracket.LineNumber)) && (firstTokenOnLine && (token.CsTokenType != CsTokenType.CloseCurlyBracket))) && (((token.CsTokenType != CsTokenType.Finally) && (token.CsTokenType != CsTokenType.Catch)) && ((token.CsTokenType != CsTokenType.WhileDo) && (token.CsTokenType != CsTokenType.Else)))) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
                            {
                                base.AddViolation(document.RootElement, bracket.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingCurlyBracketMustBeFollowedByBlankLine, new object[0]);
                            }
                        }
                    }
                }
                else
                {
                    if (token.CsTokenType == CsTokenType.CloseCurlyBracket)
                    {
                        base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingCurlyBracketsMustNotBePrecededByBlankLine, new object[0]);
                    }
                    else if (token.CsTokenType == CsTokenType.OpenCurlyBracket)
                    {
                        base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningCurlyBracketsMustNotBePrecededByBlankLine, new object[0]);
                    }
                    else if (((token.CsTokenType == CsTokenType.Else) || (token.CsTokenType == CsTokenType.Catch)) || (token.CsTokenType == CsTokenType.Finally))
                    {
                        base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.ChainedStatementBlocksMustNotBePrecededByBlankLine, new object[0]);
                    }
                    else if (token.CsTokenType == CsTokenType.WhileDo)
                    {
                        base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.WhileDoFooterMustNotBePrecededByBlankLine, new object[0]);
                    }
                    if (((!fileHeader && (precedingTokenNode != null)) && ((precedingTokenNode.Value.CsTokenType == CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.SingleLineComment))) && ((token.CsTokenType != CsTokenType.MultiLineComment) && (token.CsTokenType != CsTokenType.XmlHeader)))
                    {
                        bool flag = false;
                        if (precedingTokenNode != null)
                        {
                            foreach (CsToken token2 in document.Tokens.ReverseIterator(precedingTokenNode.Previous))
                            {
                                if (token2.CsTokenType == CsTokenType.EndOfLine)
                                {
                                    break;
                                }
                                if (token2.CsTokenType != CsTokenType.WhiteSpace)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        string str = precedingTokenNode.Value.Text.Trim();
                        if ((!flag && !str.StartsWith("////", StringComparison.Ordinal)) && !IsCommentInFileHeader(precedingTokenNode))
                        {
                            base.AddViolation(document.RootElement, precedingTokenNode.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.SingleLineCommentsMustNotBeFollowedByBlankLine, new object[0]);
                        }
                    }
                }
            }
        }

        private void CheckMissingBlock(CsElement parentElement, Statement statement, Statement embeddedStatement, string statementType, bool allowStacks)
        {
            if (((embeddedStatement != null) && (embeddedStatement.StatementType != StatementType.Block)) && ((!allowStacks || (statement.ChildStatements == null)) || ((statement.ChildStatements.Count == 0) || (GetFirstChildStatement(statement).StatementType != statement.StatementType))))
            {
                base.AddViolation(parentElement, embeddedStatement.LineNumber, Microsoft.StyleCop.CSharp.Rules.CurlyBracketsMustNotBeOmitted, new object[] { statementType });
            }
        }

        private void CheckSiblingAccessors(CsElement accessor, Microsoft.StyleCop.Node<CsToken> openingBracketNode)
        {
            Bracket bracket = (Bracket) openingBracketNode.Value;
            if ((bracket.MatchingBracket != null) && (bracket.LineNumber == bracket.MatchingBracket.LineNumber))
            {
                foreach (CsElement element in accessor.ParentElement.ChildElements)
                {
                    if (element != accessor)
                    {
                        Microsoft.StyleCop.Node<CsToken> openBracket = GetOpenBracket(element.Tokens);
                        if (openBracket != null)
                        {
                            bracket = (Bracket) openBracket.Value;
                            if (((bracket != null) && (bracket.MatchingBracket != null)) && (bracket.LineNumber != bracket.MatchingBracket.LineNumber))
                            {
                                base.AddViolation(accessor, accessor.LineNumber, Microsoft.StyleCop.CSharp.Rules.AllAccessorsMustBeMultiLineOrSingleLine, new object[] { accessor.ParentElement.FriendlyTypeText });
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool CheckStatementCurlyBracketPlacement(Statement statement, Expression parentExpression, Statement parentStatement, CsElement parentElement, object context)
        {
            switch (statement.StatementType)
            {
                case StatementType.DoWhile:
                    this.CheckMissingBlock(parentElement, statement, ((DoWhileStatement) statement).EmbeddedStatement, statement.FriendlyTypeText, false);
                    break;

                case StatementType.Else:
                    this.CheckMissingBlock(parentElement, statement, ((ElseStatement) statement).EmbeddedStatement, statement.FriendlyTypeText, false);
                    break;

                case StatementType.Block:
                {
                    bool allowAllOnOneLine = false;
                    LambdaExpression parent = statement.Parent as LambdaExpression;
                    if ((parent != null) && (parent.Location.StartPoint.LineNumber == parent.Location.EndPoint.LineNumber))
                    {
                        allowAllOnOneLine = true;
                    }
                    this.CheckBracketPlacement(parentElement, statement, statement.Tokens, GetOpenBracket(statement.Tokens), allowAllOnOneLine);
                    break;
                }
                case StatementType.Foreach:
                    this.CheckMissingBlock(parentElement, statement, ((ForeachStatement) statement).EmbeddedStatement, statement.FriendlyTypeText, false);
                    break;

                case StatementType.For:
                    this.CheckMissingBlock(parentElement, statement, ((ForStatement) statement).EmbeddedStatement, statement.FriendlyTypeText, false);
                    break;

                case StatementType.If:
                    this.CheckMissingBlock(parentElement, statement, ((IfStatement) statement).EmbeddedStatement, statement.FriendlyTypeText, false);
                    break;

                case StatementType.Switch:
                    this.CheckBracketPlacement(parentElement, statement, statement.Tokens, GetOpenBracket(statement.Tokens), false);
                    break;

                case StatementType.Using:
                    this.CheckMissingBlock(parentElement, statement, ((UsingStatement) statement).EmbeddedStatement, statement.FriendlyTypeText, true);
                    break;

                case StatementType.While:
                    this.CheckMissingBlock(parentElement, statement, ((WhileStatement) statement).EmbeddedStatement, statement.FriendlyTypeText, false);
                    break;
            }
            return true;
        }

        private static bool DoesAccessorHaveBody(Accessor accessor)
        {
            for (Microsoft.StyleCop.Node<CsToken> node = accessor.Tokens.First; node != accessor.Tokens.Last; node = node.Next)
            {
                if (node.Value.CsTokenType == CsTokenType.OpenCurlyBracket)
                {
                    return true;
                }
            }
            return false;
        }

        private static Statement GetFirstChildStatement(Statement statement)
        {
            if ((statement.ChildStatements != null) && (statement.ChildStatements.Count > 0))
            {
                using (IEnumerator<Statement> enumerator = statement.ChildStatements.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }
            }
            return null;
        }

        private static Microsoft.StyleCop.Node<CsToken> GetOpenBracket(CsTokenList tokens)
        {
            for (Microsoft.StyleCop.Node<CsToken> node = tokens.First; !tokens.OutOfBounds(node); node = node.Next)
            {
                if (node.Value.CsTokenType == CsTokenType.OpenCurlyBracket)
                {
                    return node;
                }
            }
            return null;
        }

        private static string GetOpeningOrClosingBracketText(Bracket bracket)
        {
            switch (bracket.CsTokenType)
            {
                case CsTokenType.OpenParenthesis:
                case CsTokenType.OpenCurlyBracket:
                case CsTokenType.OpenSquareBracket:
                case CsTokenType.OpenGenericBracket:
                case CsTokenType.OpenAttributeBracket:
                    return Microsoft.StyleCop.CSharp.Strings.Opening;

                case CsTokenType.CloseParenthesis:
                case CsTokenType.CloseCurlyBracket:
                case CsTokenType.CloseSquareBracket:
                case CsTokenType.CloseGenericBracket:
                case CsTokenType.CloseAttributeBracket:
                    return Microsoft.StyleCop.CSharp.Strings.Closing;
            }
            return string.Empty;
        }

        private static bool IsAutomaticProperty(Property property)
        {
            if (property.GetAccessor != null)
            {
                return !DoesAccessorHaveBody(property.GetAccessor);
            }
            return ((property.SetAccessor != null) && !DoesAccessorHaveBody(property.SetAccessor));
        }

        private static bool IsCommentInFileHeader(Microsoft.StyleCop.Node<CsToken> comment)
        {
            for (Microsoft.StyleCop.Node<CsToken> node = comment; node != null; node = node.Previous)
            {
                if (((node.Value.CsTokenType != CsTokenType.SingleLineComment) && (node.Value.CsTokenType != CsTokenType.WhiteSpace)) && (node.Value.CsTokenType != CsTokenType.EndOfLine))
                {
                    return false;
                }
            }
            return true;
        }

        private bool VisitElement(CsElement element, CsElement parentElement, object context)
        {
            this.CheckElementCurlyBracketPlacement(element);
            this.CheckChildElementSpacing(element);
            return true;
        }
    }
}

