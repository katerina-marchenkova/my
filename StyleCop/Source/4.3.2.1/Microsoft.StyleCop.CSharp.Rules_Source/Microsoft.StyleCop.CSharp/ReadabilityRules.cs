namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [SourceAnalyzer(typeof(CsParser))]
    public class ReadabilityRules : SourceAnalyzer
    {
        private readonly string[][] builtInTypes = new string[][] { new string[] { "Boolean", "System.Boolean", "bool" }, new string[] { "Object", "System.Object", "object" }, new string[] { "String", "System.String", "string" }, new string[] { "Int16", "System.Int16", "short" }, new string[] { "UInt16", "System.UInt16", "ushort" }, new string[] { "Int32", "System.Int32", "int" }, new string[] { "UInt32", "System.UInt32", "uint" }, new string[] { "Int64", "System.Int64", "long" }, new string[] { "UInt64", "System.UInt64", "ulong" }, new string[] { "Double", "System.Double", "double" }, new string[] { "Single", "System.Single", "float" }, new string[] { "Byte", "System.Byte", "byte" }, new string[] { "SByte", "System.SByte", "sbyte" }, new string[] { "Char", "System.Char", "char" }, new string[] { "Decimal", "System.Decimal", "decimal" } };

        private static void AddClassMember(Dictionary<string, List<CsElement>> members, CsElement child, string name)
        {
            List<CsElement> list = null;
            if (!members.TryGetValue(name, out list))
            {
                list = new List<CsElement>(1);
                members.Add(name, list);
            }
            list.Add(child);
        }

        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            CsDocument document2 = (CsDocument) document;
            Settings context = new Settings();
            context.DoNotUseRegions = this.IsRuleEnabled(document, Microsoft.StyleCop.CSharp.Rules.DoNotUseRegions.ToString());
            context.DoNotPlaceRegionsWithinElements = this.IsRuleEnabled(document, Microsoft.StyleCop.CSharp.Rules.DoNotPlaceRegionsWithinElements.ToString());
            if ((document2.RootElement != null) && !document2.RootElement.Generated)
            {
                document2.WalkDocument<object>(new CodeWalkerElementVisitor<object>(this.ProcessElement), null, new CodeWalkerExpressionVisitor<object>(this.ProcessExpression), context);
                this.CheckStatementFormattingRulesForElement(document2.RootElement);
                this.CheckClassMemberRulesForElements(document2.RootElement, null, null);
                this.CheckForEmptyComments(document2.RootElement);
                this.IterateTokenList(document2, context);
            }
        }

        private void CheckBlockStatementsCurlyBracketPlacement(CsElement element, Statement statement)
        {
            Microsoft.StyleCop.Node<CsToken> openingCurlyBracketFromStatement = GetOpeningCurlyBracketFromStatement(statement);
            if (openingCurlyBracketFromStatement != null)
            {
                CsToken previousToken = GetPreviousToken(openingCurlyBracketFromStatement.Previous, statement.Tokens.MasterList);
                if (previousToken != null)
                {
                    this.CheckTokenPrecedingOrFollowingCurlyBracket(element, previousToken);
                }
            }
        }

        private void CheckBuiltInType(Microsoft.StyleCop.Node<CsToken> type, CsDocument document)
        {
            TypeToken token = (TypeToken) type.Value;
            if (type.Value.CsTokenClass != CsTokenClass.GenericType)
            {
                for (int i = 0; i < this.builtInTypes.Length; i++)
                {
                    string[] strArray = this.builtInTypes[i];
                    if (CsTokenList.MatchTokens(token.ChildTokens.First, new string[] { strArray[0] }) || CsTokenList.MatchTokens(token.ChildTokens.First, new string[] { "System", ".", strArray[0] }))
                    {
                        bool flag = false;
                        for (Microsoft.StyleCop.Node<CsToken> node = type.Previous; node != null; node = node.Previous)
                        {
                            if (((node.Value.CsTokenType != CsTokenType.EndOfLine) && (node.Value.CsTokenType != CsTokenType.MultiLineComment)) && ((node.Value.CsTokenType != CsTokenType.SingleLineComment) && (node.Value.CsTokenType != CsTokenType.WhiteSpace)))
                            {
                                if (node.Value.Text == "=")
                                {
                                    flag = true;
                                }
                                break;
                            }
                        }
                        if (!flag)
                        {
                            base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.UseBuiltInTypeAlias, new object[] { strArray[2], strArray[0], strArray[1] });
                        }
                        break;
                    }
                }
            }
            for (Microsoft.StyleCop.Node<CsToken> node2 = token.ChildTokens.First; node2 != null; node2 = node2.Next)
            {
                if ((node2.Value.CsTokenClass == CsTokenClass.Type) || (node2.Value.CsTokenClass == CsTokenClass.GenericType))
                {
                    this.CheckBuiltInType(node2, document);
                }
            }
        }

        private void CheckChainedStatementCurlyBracketPlacement(CsElement element, Statement statement)
        {
            if (statement.Tokens.First != null)
            {
                Microsoft.StyleCop.Node<CsToken> openingCurlyBracketFromStatement = GetOpeningCurlyBracketFromStatement(statement);
                if (openingCurlyBracketFromStatement != null)
                {
                    CsToken previousToken = GetPreviousToken(openingCurlyBracketFromStatement.Previous, statement.Tokens.MasterList);
                    if (previousToken != null)
                    {
                        this.CheckTokenPrecedingOrFollowingCurlyBracket(element, previousToken);
                    }
                }
            }
        }

        private bool CheckClassMemberRulesForElements(CsElement element, ClassBase parentClass, Dictionary<string, List<CsElement>> members)
        {
            if (base.Cancel)
            {
                return false;
            }
            if (((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Struct)) || (element.ElementType == ElementType.Interface))
            {
                parentClass = element as ClassBase;
                members = CollectClassMembers(parentClass);
            }
            foreach (CsElement element2 in element.ChildElements)
            {
                if (element2.Generated)
                {
                    continue;
                }
                if (((element2.ElementType == ElementType.Method) || (element2.ElementType == ElementType.Constructor)) || ((element2.ElementType == ElementType.Destructor) || (element2.ElementType == ElementType.Accessor)))
                {
                    if (parentClass != null)
                    {
                        this.CheckClassMemberRulesForStatements(element2.ChildStatements, element2, parentClass, members);
                    }
                    continue;
                }
                if ((element2.ElementType == ElementType.Class) || (element2.ElementType == ElementType.Struct))
                {
                    ClassBase base2 = element2 as ClassBase;
                    this.CheckClassMemberRulesForElements(element2, base2, members);
                    continue;
                }
                if (!this.CheckClassMemberRulesForElements(element2, parentClass, members))
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckClassMemberRulesForExpression(Expression expression, Expression parentExpression, CsElement parentElement, ClassBase parentClass, Dictionary<string, List<CsElement>> members)
        {
            if (expression.ExpressionType == ExpressionType.Literal)
            {
                if (!IsLiteralTokenPrecededByMemberAccessSymbol(((LiteralExpression) expression).TokenNode, expression.Tokens.MasterList))
                {
                    this.CheckClassMemberRulesForLiteralToken(((LiteralExpression) expression).TokenNode, expression, parentExpression, parentElement, parentClass, members);
                }
            }
            else
            {
                if (((expression.ExpressionType == ExpressionType.Assignment) && (parentExpression != null)) && (parentExpression.ExpressionType == ExpressionType.CollectionInitializer))
                {
                    this.CheckClassMemberRulesForExpression(((AssignmentExpression) expression).RightHandSide, expression, parentElement, parentClass, members);
                }
                else if (expression.ChildExpressions.Count > 0)
                {
                    this.CheckClassMemberRulesForExpressions(expression.ChildExpressions, expression, parentElement, parentClass, members);
                }
                if (expression.ExpressionType == ExpressionType.AnonymousMethod)
                {
                    this.CheckClassMemberRulesForStatements(expression.ChildStatements, parentElement, parentClass, members);
                }
                else if (expression.ExpressionType == ExpressionType.MethodInvocation)
                {
                    MethodInvocationExpression expression2 = (MethodInvocationExpression) expression;
                    foreach (Expression expression3 in expression2.Arguments)
                    {
                        this.CheckClassMemberRulesForExpression(expression3, null, parentElement, parentClass, members);
                    }
                }
            }
        }

        private void CheckClassMemberRulesForExpressions(ICollection<Expression> expressions, Expression parentExpression, CsElement parentElement, ClassBase parentClass, Dictionary<string, List<CsElement>> members)
        {
            foreach (Expression expression in expressions)
            {
                if (expression.ExpressionType == ExpressionType.VariableDeclarator)
                {
                    VariableDeclaratorExpression expression2 = expression as VariableDeclaratorExpression;
                    if (expression2.Initializer != null)
                    {
                        this.CheckClassMemberRulesForExpression(expression2.Initializer, parentExpression, parentElement, parentClass, members);
                    }
                }
                else
                {
                    this.CheckClassMemberRulesForExpression(expression, parentExpression, parentElement, parentClass, members);
                }
            }
        }

        private void CheckClassMemberRulesForLiteralToken(Microsoft.StyleCop.Node<CsToken> tokenNode, Expression expression, Expression parentExpression, CsElement parentElement, ClassBase parentClass, Dictionary<string, List<CsElement>> members)
        {
            if (!(tokenNode.Value is TypeToken) && !tokenNode.Value.Text.StartsWith(".", StringComparison.Ordinal))
            {
                if (!(tokenNode.Value.Text == "base") || (parentExpression == null))
                {
                    if (tokenNode.Value.Text != "this")
                    {
                        this.CheckWordUsageAgainstClassMemberRules(tokenNode.Value.Text, tokenNode.Value, tokenNode.Value.LineNumber, expression, parentExpression, parentElement, parentClass, members);
                    }
                }
                else
                {
                    CsToken token = ExtractBaseClassMemberName(parentExpression, tokenNode);
                    if (token != null)
                    {
                        ICollection<CsElement> is2 = FindClassMember(token.Text, parentClass, members, true);
                        bool flag = false;
                        if (is2 != null)
                        {
                            foreach (CsElement element in is2)
                            {
                                if (!element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            base.AddViolation(parentElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.DoNotPrefixCallsWithBaseUnlessLocalImplementationExists, new object[] { token });
                        }
                    }
                }
            }
        }

        private void CheckClassMemberRulesForStatements(ICollection<Statement> statements, CsElement parentElement, ClassBase parentClass, Dictionary<string, List<CsElement>> members)
        {
            foreach (Statement statement in statements)
            {
                if (statement.ChildStatements.Count > 0)
                {
                    this.CheckClassMemberRulesForStatements(statement.ChildStatements, parentElement, parentClass, members);
                }
                this.CheckClassMemberRulesForExpressions(statement.ChildExpressions, null, parentElement, parentClass, members);
            }
        }

        private void CheckEmptyString(Microsoft.StyleCop.Node<CsToken> stringNode, CsDocument document)
        {
            CsToken token = stringNode.Value;
            if (string.Equals(token.Text, "\"\"", StringComparison.Ordinal) || string.Equals(token.Text, "@\"\"", StringComparison.Ordinal))
            {
                Microsoft.StyleCop.Node<CsToken> assignmentOperator = null;
                for (Microsoft.StyleCop.Node<CsToken> node2 = stringNode.Previous; node2 != null; node2 = node2.Previous)
                {
                    if (((node2.Value.CsTokenType != CsTokenType.WhiteSpace) && (node2.Value.CsTokenType != CsTokenType.EndOfLine)) && ((node2.Value.CsTokenType != CsTokenType.SingleLineComment) && (node2.Value.CsTokenType != CsTokenType.MultiLineComment)))
                    {
                        assignmentOperator = node2;
                        break;
                    }
                }
                if ((assignmentOperator == null) || ((assignmentOperator.Value.CsTokenType != CsTokenType.Case) && !IsConstVariableDeclaration(assignmentOperator)))
                {
                    base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.UseStringEmptyForEmptyStrings, new object[0]);
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckForEmptyComments(DocumentRoot element)
        {
            for (Microsoft.StyleCop.Node<CsToken> node = element.Tokens.First; !element.Tokens.OutOfBounds(node); node = node.Next)
            {
                if (base.Cancel)
                {
                    return;
                }
                CsToken token = node.Value;
                if (!token.Generated)
                {
                    if (token.CsTokenType == CsTokenType.SingleLineComment)
                    {
                        int num = 0;
                        bool flag = false;
                        for (int i = 0; i < token.Text.Length; i++)
                        {
                            if (num == 2)
                            {
                                if (((token.Text[i] == ' ') || (token.Text[i] == '\t')) || ((token.Text[i] == '\r') || (token.Text[i] == '\n')))
                                {
                                    continue;
                                }
                                flag = true;
                                break;
                            }
                            if (token.Text[i] == '/')
                            {
                                num++;
                            }
                        }
                        if (flag)
                        {
                            continue;
                        }
                        bool flag2 = false;
                        int num3 = 0;
                        foreach (CsToken token2 in element.Tokens.ReverseIterator(node.Previous))
                        {
                            if (token2.Text == "\n")
                            {
                                num3++;
                                if (num3 <= 1)
                                {
                                    continue;
                                }
                                break;
                            }
                            if (token2.CsTokenType == CsTokenType.SingleLineComment)
                            {
                                flag2 = true;
                                break;
                            }
                            if (token2.CsTokenType != CsTokenType.WhiteSpace)
                            {
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            base.AddViolation(element, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.CommentsMustContainText, new object[0]);
                            continue;
                        }
                        flag2 = false;
                        num3 = 0;
                        foreach (CsToken token3 in element.Tokens.ForwardIterator(node.Next))
                        {
                            if (token3.Text == "\n")
                            {
                                num3++;
                                if (num3 <= 1)
                                {
                                    continue;
                                }
                                break;
                            }
                            if (token3.CsTokenType == CsTokenType.SingleLineComment)
                            {
                                flag2 = true;
                                break;
                            }
                            if (token3.CsTokenType != CsTokenType.WhiteSpace)
                            {
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            base.AddViolation(element, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.CommentsMustContainText, new object[0]);
                        }
                        continue;
                    }
                    if (token.CsTokenType == CsTokenType.MultiLineComment)
                    {
                        int index = token.Text.IndexOf("/*", StringComparison.Ordinal);
                        if (index > -1)
                        {
                            int num5 = token.Text.IndexOf("*/", index + 2, StringComparison.Ordinal);
                            if (num5 > -1)
                            {
                                bool flag3 = false;
                                for (int j = index + 2; j < num5; j++)
                                {
                                    if (((token.Text[j] != ' ') && (token.Text[j] != '\t')) && ((token.Text[j] != '\r') && (token.Text[j] != '\n')))
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }
                                if (!flag3)
                                {
                                    base.AddViolation(element, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.CommentsMustContainText, new object[0]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CheckForRegionsInElement(CsElement element, Settings settings)
        {
            if (((settings.DoNotPlaceRegionsWithinElements && !settings.DoNotUseRegions) && !element.Generated) && (((element.ElementType == ElementType.Method) || (element.ElementType == ElementType.Accessor)) || (((element.ElementType == ElementType.Constructor) || (element.ElementType == ElementType.Destructor)) || (element.ElementType == ElementType.Field))))
            {
                for (Microsoft.StyleCop.Node<CsToken> node = element.Tokens.First; node != element.Tokens.Last.Next; node = node.Next)
                {
                    if (node.Value.CsTokenClass == CsTokenClass.RegionDirective)
                    {
                        Region region = (Region) node.Value;
                        if ((region.Beginning && !region.Generated) && !region.IsGeneratedCodeRegion)
                        {
                            base.AddViolation(element, node.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.DoNotPlaceRegionsWithinElements, new object[0]);
                        }
                    }
                }
            }
        }

        private void CheckMethodArgumentList(CsElement element, IArgumentList arguments, Microsoft.StyleCop.Node<CsToken> openingBracketNode, int methodLineNumber)
        {
            bool flag;
            bool flag2;
            DetermineMethodParameterPlacementScheme(arguments, out flag, out flag2);
            if (flag && flag2)
            {
                base.AddViolation(element, methodLineNumber, Microsoft.StyleCop.CSharp.Rules.ParametersMustBeOnSameLineOrSeparateLines, new object[] { element.FriendlyTypeText });
            }
            if (flag2)
            {
                this.CheckSplitMethodArgumentList(element, arguments, openingBracketNode);
            }
            else if (arguments.Count > 0)
            {
                int lineNumber = arguments.Location(0).StartPoint.LineNumber;
                if ((lineNumber != openingBracketNode.Value.LineNumber) && (lineNumber != (openingBracketNode.Value.LineNumber + 1)))
                {
                    int num2 = MeasureCommentLinesAfter(openingBracketNode);
                    if (lineNumber != ((openingBracketNode.Value.LineNumber + num2) + 1))
                    {
                        base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.ParameterListMustFollowDeclaration, new object[0]);
                    }
                }
            }
        }

        private void CheckMethodClosingBracket(CsElement element, CsTokenList parameterListTokens, Microsoft.StyleCop.Node<CsToken> openingBracketNode, CsTokenType closingBracketType, IArgumentList arguments)
        {
            Microsoft.StyleCop.Node<CsToken> end = null;
            for (Microsoft.StyleCop.Node<CsToken> node2 = parameterListTokens.Last; node2 != null; node2 = node2.Previous)
            {
                if (node2.Value.CsTokenType == closingBracketType)
                {
                    end = node2;
                    break;
                }
            }
            if (end != null)
            {
                if (arguments.Count == 0)
                {
                    if (openingBracketNode.Value.LineNumber != end.Value.LineNumber)
                    {
                        int num = MeasureCommentLinesBetween(openingBracketNode, end, false);
                        if ((openingBracketNode.Value.LineNumber + num) != end.Value.LineNumber)
                        {
                            base.AddViolation(element, end.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingParenthesisMustBeOnLineOfOpeningParenthesis, new object[0]);
                        }
                    }
                }
                else
                {
                    int lineNumber = arguments.Location(arguments.Count - 1).EndPoint.LineNumber;
                    if (lineNumber != end.Value.LineNumber)
                    {
                        int num3 = MeasureCommentLinesBetween(arguments.Tokens(arguments.Count - 1).Last, end, false);
                        if ((lineNumber + num3) != end.Value.LineNumber)
                        {
                            base.AddViolation(element, end.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.ClosingParenthesisMustBeOnLineOfLastParameter, new object[0]);
                        }
                    }
                }
            }
        }

        private void CheckMethodInvocationParameters(CsElement element, MethodInvocationExpression expression)
        {
            if ((expression.Tokens.First != null) && !expression.Tokens.First.Value.Generated)
            {
                ArgumentList methodArguments = new ArgumentList(expression.Arguments);
                CsTokenList parameterListTokens = GetArgumentListTokens(expression.Tokens, expression.Name.Tokens.Last, CsTokenType.OpenParenthesis, CsTokenType.CloseParenthesis);
                if (parameterListTokens != null)
                {
                    this.CheckParameters(element, parameterListTokens, methodArguments, expression.LineNumber, CsTokenType.OpenParenthesis, CsTokenType.CloseParenthesis);
                }
            }
        }

        private Microsoft.StyleCop.Node<CsToken> CheckMethodOpeningBracket(CsElement element, CsTokenList parameterListTokens, CsTokenType openingBracketType)
        {
            Microsoft.StyleCop.Node<CsToken> node = null;
            for (Microsoft.StyleCop.Node<CsToken> node2 = parameterListTokens.First; node2 != null; node2 = node2.Next)
            {
                if (node2.Value.CsTokenType == openingBracketType)
                {
                    node = node2;
                    break;
                }
            }
            CsToken token = null;
            if (node != null)
            {
                for (Microsoft.StyleCop.Node<CsToken> node3 = node.Previous; node3 != null; node3 = node3.Previous)
                {
                    if (((node3.Value.CsTokenType != CsTokenType.WhiteSpace) && (node3.Value.CsTokenType != CsTokenType.EndOfLine)) && ((node3.Value.CsTokenType != CsTokenType.SingleLineComment) && (node3.Value.CsTokenType != CsTokenType.MultiLineComment)))
                    {
                        token = node3.Value;
                        break;
                    }
                }
            }
            if ((token != null) && (node.Value.LineNumber != token.LineNumber))
            {
                base.AddViolation(element, node.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.OpeningParenthesisMustBeOnDeclarationLine, new object[] { element.FriendlyTypeText });
            }
            return node;
        }

        private void CheckMethodParameters(CsElement element)
        {
            IList<Parameter> parameters = null;
            CsTokenType openParenthesis = CsTokenType.OpenParenthesis;
            CsTokenType closeParenthesis = CsTokenType.CloseParenthesis;
            if (element.ElementType == ElementType.Constructor)
            {
                parameters = ((Constructor) element).Parameters;
            }
            else if (element.ElementType == ElementType.Delegate)
            {
                parameters = ((Microsoft.StyleCop.CSharp.Delegate) element).Parameters;
            }
            else if (element.ElementType == ElementType.Method)
            {
                parameters = ((Method) element).Parameters;
            }
            else if (element.ElementType == ElementType.Indexer)
            {
                parameters = ((Indexer) element).Parameters;
                openParenthesis = CsTokenType.OpenSquareBracket;
                closeParenthesis = CsTokenType.CloseSquareBracket;
            }
            if (parameters != null)
            {
                ParameterList methodArguments = new ParameterList(parameters);
                CsTokenList parameterListTokens = GetParameterListTokens(element.Declaration.Tokens, openParenthesis, closeParenthesis);
                if (parameterListTokens != null)
                {
                    this.CheckParameters(element, parameterListTokens, methodArguments, element.LineNumber, openParenthesis, closeParenthesis);
                }
            }
        }

        private void CheckParameters(CsElement element, CsTokenList parameterListTokens, IArgumentList methodArguments, int methodStartLineNumber, CsTokenType openBracketType, CsTokenType closeBracketType)
        {
            Microsoft.StyleCop.Node<CsToken> openingBracketNode = this.CheckMethodOpeningBracket(element, parameterListTokens, openBracketType);
            if (openingBracketNode != null)
            {
                this.CheckMethodClosingBracket(element, parameterListTokens, openingBracketNode, closeBracketType, methodArguments);
                if (methodArguments.Count > 0)
                {
                    this.CheckMethodArgumentList(element, methodArguments, openingBracketNode, methodStartLineNumber);
                }
            }
        }

        private void CheckQueryExpression(CsElement element, QueryExpression queryExpression)
        {
            QueryClause previousClause = null;
            bool clauseOnSameLine = false;
            bool clauseOnSeparateLine = false;
            this.ProcessQueryClauses(element, queryExpression, queryExpression.ChildClauses, ref previousClause, ref clauseOnSameLine, ref clauseOnSeparateLine);
        }

        private void CheckSplitMethodArgumentList(CsElement element, IArgumentList arguments, Microsoft.StyleCop.Node<CsToken> openingBracketNode)
        {
            Microsoft.StyleCop.Node<CsToken> start = null;
            bool flag = false;
            for (int i = 0; i < arguments.Count; i++)
            {
                CodeLocation location = arguments.Location(i);
                int lineNumber = location.StartPoint.LineNumber;
                CsTokenList list = arguments.Tokens(i);
                if ((location.LineSpan > 1) && !arguments.MaySpanMultipleLines(i))
                {
                    base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.ParameterMustNotSpanMultipleLines, new object[0]);
                }
                if (i == 0)
                {
                    if (lineNumber != (openingBracketNode.Value.LineNumber + 1))
                    {
                        int num3 = MeasureCommentLinesAfter(openingBracketNode);
                        if (lineNumber != ((openingBracketNode.Value.LineNumber + num3) + 1))
                        {
                            base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.SplitParametersMustStartOnLineAfterDeclaration, new object[] { element.FriendlyTypeText });
                        }
                    }
                }
                else if (!flag && (lineNumber != (start.Value.LineNumber + 1)))
                {
                    int num4 = MeasureCommentLinesAfter(start);
                    if (lineNumber != ((start.Value.LineNumber + num4) + 1))
                    {
                        base.AddViolation(element, lineNumber, Microsoft.StyleCop.CSharp.Rules.ParameterMustFollowComma, new object[0]);
                    }
                }
                flag = false;
                if (i < (arguments.Count - 1))
                {
                    for (Microsoft.StyleCop.Node<CsToken> node2 = list.Last.Next; node2 != null; node2 = node2.Next)
                    {
                        if (node2.Value.CsTokenType == CsTokenType.Comma)
                        {
                            start = node2;
                            if (start.Value.LineNumber != location.EndPoint.LineNumber)
                            {
                                int num5 = MeasureCommentLinesBetween(list.Last, start, false);
                                if (start.Value.LineNumber != (location.EndPoint.LineNumber + num5))
                                {
                                    base.AddViolation(element, node2.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.CommaMustBeOnSameLineAsPreviousParameter, new object[0]);
                                    flag = true;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="Minimizing refactoring before release.")]
        private void CheckStatementCurlyBracketPlacement(CsElement element, Statement statement)
        {
            switch (statement.StatementType)
            {
                case StatementType.Catch:
                case StatementType.Finally:
                    this.CheckChainedStatementCurlyBracketPlacement(element, statement);
                    this.CheckBlockStatementsCurlyBracketPlacement(element, statement);
                    return;

                case StatementType.Checked:
                case StatementType.Fixed:
                case StatementType.Foreach:
                case StatementType.For:
                case StatementType.Lock:
                case StatementType.Switch:
                case StatementType.Unchecked:
                case StatementType.Unsafe:
                case StatementType.Using:
                case StatementType.While:
                    this.CheckBlockStatementsCurlyBracketPlacement(element, statement);
                    return;

                case StatementType.ConstructorInitializer:
                case StatementType.Continue:
                case StatementType.Empty:
                case StatementType.Expression:
                case StatementType.Goto:
                case StatementType.Label:
                case StatementType.Return:
                case StatementType.SwitchCase:
                case StatementType.SwitchDefault:
                case StatementType.Throw:
                case StatementType.VariableDeclaration:
                    break;

                case StatementType.DoWhile:
                    this.CheckBlockStatementsCurlyBracketPlacement(element, statement);
                    this.CheckTrailingStatementCurlyBracketPlacement(element, statement);
                    break;

                case StatementType.Else:
                {
                    this.CheckChainedStatementCurlyBracketPlacement(element, statement);
                    this.CheckBlockStatementsCurlyBracketPlacement(element, statement);
                    ElseStatement statement2 = (ElseStatement) statement;
                    if (statement2.AttachedElseStatement == null)
                    {
                        break;
                    }
                    this.CheckTrailingStatementCurlyBracketPlacement(element, statement);
                    return;
                }
                case StatementType.If:
                {
                    this.CheckBlockStatementsCurlyBracketPlacement(element, statement);
                    IfStatement statement3 = (IfStatement) statement;
                    if (statement3.AttachedElseStatement == null)
                    {
                        break;
                    }
                    this.CheckTrailingStatementCurlyBracketPlacement(element, statement);
                    return;
                }
                case StatementType.Try:
                {
                    this.CheckBlockStatementsCurlyBracketPlacement(element, statement);
                    TryStatement statement4 = (TryStatement) statement;
                    if ((statement4.FinallyStatement != null) || ((statement4.CatchStatements != null) && (statement4.CatchStatements.Count > 0)))
                    {
                        this.CheckTrailingStatementCurlyBracketPlacement(element, statement4);
                    }
                    if ((statement4.CatchStatements != null) && (statement4.CatchStatements.Count > 0))
                    {
                        CatchStatement[] array = new CatchStatement[statement4.CatchStatements.Count];
                        statement4.CatchStatements.CopyTo(array, 0);
                        for (int i = 0; i < array.Length; i++)
                        {
                            if ((array.Length > (i + 1)) || (statement4.FinallyStatement != null))
                            {
                                this.CheckTrailingStatementCurlyBracketPlacement(element, array[i]);
                            }
                        }
                        return;
                    }
                    break;
                }
                default:
                    return;
            }
        }

        private void CheckStatementFormattingRulesForElement(CsElement element)
        {
            if (!element.Generated)
            {
                if (element.ElementType == ElementType.EmptyElement)
                {
                    base.AddViolation(element, element.LineNumber, Microsoft.StyleCop.CSharp.Rules.CodeMustNotContainEmptyStatements, new object[0]);
                }
                else
                {
                    this.CheckStatementFormattingRulesForStatements(element, element.ChildStatements);
                    foreach (CsElement element2 in element.ChildElements)
                    {
                        this.CheckStatementFormattingRulesForElement(element2);
                    }
                }
            }
        }

        private void CheckStatementFormattingRulesForExpressions(CsElement element, ICollection<Expression> expressions)
        {
            foreach (Expression expression in expressions)
            {
                if (expression.ExpressionType == ExpressionType.AnonymousMethod)
                {
                    AnonymousMethodExpression expression2 = expression as AnonymousMethodExpression;
                    this.CheckStatementFormattingRulesForStatements(element, expression2.ChildStatements);
                }
                else
                {
                    this.CheckStatementFormattingRulesForExpressions(element, expression.ChildExpressions);
                }
            }
        }

        private void CheckStatementFormattingRulesForStatement(CsElement element, Statement statement, Statement previousStatement)
        {
            if (statement.StatementType == StatementType.Empty)
            {
                base.AddViolation(element, statement.LineNumber, Microsoft.StyleCop.CSharp.Rules.CodeMustNotContainEmptyStatements, new object[0]);
            }
            else if (previousStatement != null)
            {
                Microsoft.StyleCop.Node<CsToken> first = statement.Tokens.First;
                Microsoft.StyleCop.Node<CsToken> last = previousStatement.Tokens.Last;
                if (first.Value.Location.StartPoint.LineNumber == last.Value.Location.EndPoint.LineNumber)
                {
                    base.AddViolation(element, first.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.CodeMustNotContainMultipleStatementsOnOneLine, new object[0]);
                }
            }
            this.CheckStatementCurlyBracketPlacement(element, statement);
            this.CheckStatementFormattingRulesForStatements(element, statement.ChildStatements);
            this.CheckStatementFormattingRulesForExpressions(element, statement.ChildExpressions);
        }

        private void CheckStatementFormattingRulesForStatements(CsElement element, ICollection<Statement> statements)
        {
            Statement previousStatement = null;
            foreach (Statement statement2 in statements)
            {
                this.CheckStatementFormattingRulesForStatement(element, statement2, previousStatement);
                previousStatement = statement2;
            }
        }

        private void CheckTokenPrecedingOrFollowingCurlyBracket(CsElement element, CsToken previousOrNextToken)
        {
            if (((previousOrNextToken.CsTokenType == CsTokenType.MultiLineComment) || (previousOrNextToken.CsTokenType == CsTokenType.SingleLineComment)) || ((previousOrNextToken.CsTokenType == CsTokenType.XmlHeader) || (previousOrNextToken.CsTokenType == CsTokenType.XmlHeaderLine)))
            {
                base.AddViolation(element, previousOrNextToken.LineNumber, Microsoft.StyleCop.CSharp.Rules.BlockStatementsMustNotContainEmbeddedComments, new object[0]);
            }
            else if ((previousOrNextToken.CsTokenType == CsTokenType.PreprocessorDirective) && (previousOrNextToken is Region))
            {
                base.AddViolation(element, previousOrNextToken.LineNumber, Microsoft.StyleCop.CSharp.Rules.BlockStatementsMustNotContainEmbeddedRegions, new object[0]);
            }
        }

        private void CheckTrailingStatementCurlyBracketPlacement(CsElement element, Statement statement)
        {
            Microsoft.StyleCop.Node<CsToken> closingBracketFromStatement = GetClosingBracketFromStatement(statement);
            if (closingBracketFromStatement != null)
            {
                CsToken nextToken = GetNextToken(closingBracketFromStatement.Next, statement.Tokens.MasterList);
                if (nextToken != null)
                {
                    this.CheckTokenPrecedingOrFollowingCurlyBracket(element, nextToken);
                }
            }
        }

        private void CheckWordUsageAgainstClassMemberRules(string word, CsToken item, int line, Expression expression, Expression parentExpression, CsElement parentElement, ClassBase parentClass, Dictionary<string, List<CsElement>> members)
        {
            if (!IsLocalMember(word, item, expression) && !IsObjectInitializerLeftHandSideExpression(expression))
            {
                CsElement element = null;
                ICollection<CsElement> is2 = FindClassMember(word, parentClass, members, false);
                if (is2 != null)
                {
                    if (is2 != null)
                    {
                        foreach (CsElement element2 in is2)
                        {
                            if (element2.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }) || ((element2.ElementType == ElementType.Field) && ((Field) element2).Const))
                            {
                                element = null;
                                break;
                            }
                            if ((((element2.ElementType != ElementType.Class) && (element2.ElementType != ElementType.Struct)) && ((element2.ElementType != ElementType.Delegate) && (element2.ElementType != ElementType.Enum))) && (element == null))
                            {
                                element = element2;
                            }
                        }
                    }
                    if (element != null)
                    {
                        if (element.ElementType == ElementType.Property)
                        {
                            Property property = (Property) element;
                            if (property.ReturnType.Text != property.Declaration.Name)
                            {
                                base.AddViolation(parentElement, line, Microsoft.StyleCop.CSharp.Rules.PrefixLocalCallsWithThis, new object[] { word });
                            }
                        }
                        else
                        {
                            base.AddViolation(parentElement, line, Microsoft.StyleCop.CSharp.Rules.PrefixLocalCallsWithThis, new object[] { word });
                        }
                    }
                }
            }
        }

        private static Dictionary<string, List<CsElement>> CollectClassMembers(ClassBase parentClass)
        {
            Dictionary<string, List<CsElement>> members = new Dictionary<string, List<CsElement>>();
            if (parentClass.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial }))
            {
                foreach (ClassBase base2 in parentClass.PartialElementList)
                {
                    CollectClassMembersAux(base2, members);
                }
                return members;
            }
            CollectClassMembersAux(parentClass, members);
            return members;
        }

        private static void CollectClassMembersAux(ClassBase @class, Dictionary<string, List<CsElement>> members)
        {
            foreach (CsElement element in @class.ChildElements)
            {
                if (element.ElementType == ElementType.Field)
                {
                    Field field = element as Field;
                    foreach (VariableDeclaratorExpression expression in field.VariableDeclarationStatement.Declarators)
                    {
                        AddClassMember(members, element, expression.Identifier.Text);
                    }
                    continue;
                }
                if (element.ElementType != ElementType.EmptyElement)
                {
                    AddClassMember(members, element, element.Declaration.Name);
                }
            }
        }

        private bool ContainsPartialMembers(CsElement element)
        {
            if ((((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Struct)) || (element.ElementType == ElementType.Interface)) && element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial }))
            {
                return true;
            }
            if (((element.ElementType == ElementType.Root) || (element.ElementType == ElementType.Namespace)) || ((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Struct)))
            {
                foreach (CsElement element2 in element.ChildElements)
                {
                    if (this.ContainsPartialMembers(element2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool ContainsVariable(VariableCollection variables, string word, CsToken item)
        {
            Variable variable = variables[word];
            if (variable != null)
            {
                if (variable.Location.LineNumber < item.LineNumber)
                {
                    return true;
                }
                if ((variable.Location.LineNumber == item.LineNumber) && (variable.Location.IndexOnLine < item.Location.StartPoint.IndexOnLine))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool DelayAnalysis(CodeDocument document, int passNumber)
        {
            Param.RequireNotNull(document, "document");
            bool flag = false;
            if (passNumber == 0)
            {
                CsDocument document2 = document as CsDocument;
                if (document2.RootElement != null)
                {
                    flag = this.ContainsPartialMembers(document2.RootElement);
                }
            }
            return flag;
        }

        private static void DetermineMethodParameterPlacementScheme(IArgumentList arguments, out bool someParametersShareLine, out bool someParameterOnDifferentLines)
        {
            someParametersShareLine = false;
            someParameterOnDifferentLines = false;
            CodeLocation location = null;
            for (int i = 0; i < arguments.Count; i++)
            {
                CodeLocation location2 = arguments.Location(i);
                if (i > 0)
                {
                    if (location.StartPoint.LineNumber == location2.EndPoint.LineNumber)
                    {
                        someParametersShareLine = true;
                    }
                    else
                    {
                        someParameterOnDifferentLines = true;
                    }
                }
                location = location2;
            }
        }

        private static CsToken ExtractBaseClassMemberName(Expression parentExpression, Microsoft.StyleCop.Node<CsToken> baseTokenNode)
        {
            bool flag = false;
            foreach (CsToken token in parentExpression.Tokens.ForwardIterator(baseTokenNode.Next))
            {
                if ((((token.CsTokenType != CsTokenType.WhiteSpace) && (token.CsTokenType != CsTokenType.EndOfLine)) && ((token.CsTokenType != CsTokenType.SingleLineComment) && (token.CsTokenType != CsTokenType.MultiLineComment))) && (token.CsTokenType != CsTokenType.PreprocessorDirective))
                {
                    if (flag)
                    {
                        if (token.CsTokenType == CsTokenType.Other)
                        {
                            return token;
                        }
                        break;
                    }
                    if ((token.CsTokenType != CsTokenType.OperatorSymbol) || (((OperatorSymbol) token).SymbolType != OperatorType.MemberAccess))
                    {
                        break;
                    }
                    flag = true;
                }
            }
            return null;
        }

        private static ICollection<CsElement> FindClassMember(string word, ClassBase parentClass, Dictionary<string, List<CsElement>> members, bool interfaces)
        {
            if (word != parentClass.Declaration.Name)
            {
                ICollection<CsElement> is2 = MatchClassMember(word, members, interfaces);
                if ((is2 != null) && (is2.Count > 0))
                {
                    return is2;
                }
            }
            return null;
        }

        private static CsTokenList GetArgumentListTokens(CsTokenList tokens, Microsoft.StyleCop.Node<CsToken> methodNameLastToken, CsTokenType openBracketType, CsTokenType closeBracketType)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = null;
            Microsoft.StyleCop.Node<CsToken> lastItemNode = null;
            int num = 0;
            for (Microsoft.StyleCop.Node<CsToken> node3 = methodNameLastToken.Next; node3 != null; node3 = node3.Next)
            {
                if (node3.Value.CsTokenType == openBracketType)
                {
                    num++;
                    if (num == 1)
                    {
                        firstItemNode = node3;
                    }
                }
                else if (node3.Value.CsTokenType == closeBracketType)
                {
                    num--;
                    if (num == 0)
                    {
                        lastItemNode = node3;
                        break;
                    }
                }
            }
            if ((firstItemNode != null) && (lastItemNode != null))
            {
                return new CsTokenList(tokens.MasterList, firstItemNode, lastItemNode);
            }
            return null;
        }

        private static BlockStatement GetChildBlockStatement(Statement statement)
        {
            BlockStatement statement2 = null;
            if (statement.StatementType == StatementType.Block)
            {
                statement2 = statement as BlockStatement;
            }
            if (statement2 == null)
            {
                foreach (Statement statement3 in statement.ChildStatements)
                {
                    if (statement3.StatementType == StatementType.Block)
                    {
                        return (statement3 as BlockStatement);
                    }
                }
            }
            return statement2;
        }

        private static Microsoft.StyleCop.Node<CsToken> GetClosingBracketFromStatement(Statement statement)
        {
            BlockStatement childBlockStatement = GetChildBlockStatement(statement);
            if (childBlockStatement != null)
            {
                for (Microsoft.StyleCop.Node<CsToken> node = childBlockStatement.Tokens.Last; node != null; node = node.Previous)
                {
                    if (node.Value.CsTokenType == CsTokenType.CloseCurlyBracket)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        private static CsToken GetNextToken(Microsoft.StyleCop.Node<CsToken> tokenNode, MasterList<CsToken> tokenList)
        {
            foreach (CsToken token in tokenList.ForwardIterator(tokenNode))
            {
                if ((token.CsTokenType != CsTokenType.EndOfLine) && (token.CsTokenType != CsTokenType.WhiteSpace))
                {
                    return token;
                }
            }
            return null;
        }

        private static Microsoft.StyleCop.Node<CsToken> GetOpeningCurlyBracketFromStatement(Statement statement)
        {
            Statement childBlockStatement = null;
            if (statement.StatementType == StatementType.Switch)
            {
                childBlockStatement = statement;
            }
            else
            {
                childBlockStatement = GetChildBlockStatement(statement);
            }
            if (childBlockStatement != null)
            {
                for (Microsoft.StyleCop.Node<CsToken> node = childBlockStatement.Tokens.First; node != null; node = node.Next)
                {
                    if (node.Value.CsTokenType == CsTokenType.OpenCurlyBracket)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        private static CsTokenList GetParameterListTokens(CsTokenList tokens, CsTokenType openBracketType, CsTokenType closeBracketType)
        {
            return GetArgumentListTokens(tokens, tokens.First, openBracketType, closeBracketType);
        }

        private static CsToken GetPreviousToken(Microsoft.StyleCop.Node<CsToken> tokenNode, MasterList<CsToken> tokenList)
        {
            foreach (CsToken token in tokenList.ReverseIterator(tokenNode))
            {
                if ((token.CsTokenType != CsTokenType.EndOfLine) && (token.CsTokenType != CsTokenType.WhiteSpace))
                {
                    return token;
                }
            }
            return null;
        }

        private static bool IsConstVariableDeclaration(Microsoft.StyleCop.Node<CsToken> assignmentOperator)
        {
            if ((assignmentOperator != null) && (assignmentOperator.Value.Text == "="))
            {
                for (Microsoft.StyleCop.Node<CsToken> node = assignmentOperator.Previous; node != null; node = node.Previous)
                {
                    if ((((node.Value.CsTokenType == CsTokenType.CloseParenthesis) || (node.Value.CsTokenType == CsTokenType.OpenParenthesis)) || ((node.Value.CsTokenType == CsTokenType.OpenCurlyBracket) || (node.Value.CsTokenType == CsTokenType.CloseCurlyBracket))) || (node.Value.CsTokenType == CsTokenType.Semicolon))
                    {
                        break;
                    }
                    if (node.Value.CsTokenType == CsTokenType.Const)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsLiteralTokenPrecededByMemberAccessSymbol(Microsoft.StyleCop.Node<CsToken> literalTokenNode, MasterList<CsToken> masterList)
        {
            CsToken previousToken = GetPreviousToken(literalTokenNode.Previous, masterList);
            if ((previousToken != null) && (previousToken.CsTokenType == CsTokenType.OperatorSymbol))
            {
                OperatorSymbol symbol = (OperatorSymbol) previousToken;
                if (((symbol.SymbolType == OperatorType.MemberAccess) || (symbol.SymbolType == OperatorType.Pointer)) || (symbol.SymbolType == OperatorType.QualifiedAlias))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsLocalMember(string word, CsToken item, ICodeUnit parent)
        {
            while (parent != null)
            {
                if (ContainsVariable(parent.Variables, word, item))
                {
                    return true;
                }
                if (parent.CodeUnitType == CodeUnitType.Element)
                {
                    break;
                }
                parent = parent.Parent;
            }
            return false;
        }

        private static bool IsObjectInitializerLeftHandSideExpression(Expression expression)
        {
            if (expression.ExpressionType == ExpressionType.Literal)
            {
                AssignmentExpression parent = expression.Parent as AssignmentExpression;
                if (((parent != null) && (parent.LeftHandSide == expression)) && (parent.Parent is ObjectInitializerExpression))
                {
                    return true;
                }
            }
            return false;
        }

        private void IterateTokenList(CsDocument document, Settings settings)
        {
            for (Microsoft.StyleCop.Node<CsToken> node = document.Tokens.First; node != null; node = node.Next)
            {
                CsToken token = node.Value;
                if ((token.CsTokenClass == CsTokenClass.Type) || (token.CsTokenClass == CsTokenClass.GenericType))
                {
                    this.CheckBuiltInType(node, document);
                }
                else if (token.CsTokenType == CsTokenType.String)
                {
                    this.CheckEmptyString(node, document);
                }
                else if ((token.CsTokenClass == CsTokenClass.RegionDirective) && settings.DoNotUseRegions)
                {
                    Region region = (Region) token;
                    if ((region.Beginning && !region.Generated) && !region.IsGeneratedCodeRegion)
                    {
                        base.AddViolation(document.RootElement, token.LineNumber, Microsoft.StyleCop.CSharp.Rules.DoNotUseRegions, new object[0]);
                    }
                }
            }
        }

        private static ICollection<CsElement> MatchClassMember(string word, Dictionary<string, List<CsElement>> members, bool interfaces)
        {
            List<CsElement> list = null;
            List<CsElement> list2 = null;
            if (members.TryGetValue(word, out list2))
            {
                foreach (CsElement element in list2)
                {
                    if (((element.ElementType == ElementType.Field) || (element.Declaration.Name == word)) || (interfaces && element.Declaration.Name.EndsWith("." + word, StringComparison.Ordinal)))
                    {
                        if (list == null)
                        {
                            list = new List<CsElement>();
                        }
                        list.Add(element);
                    }
                }
            }
            return list;
        }

        private static int MeasureCommentLinesAfter(Microsoft.StyleCop.Node<CsToken> start)
        {
            int num = 0;
            int num2 = -1;
            int lineNumber = -1;
            for (Microsoft.StyleCop.Node<CsToken> node = start.Next; node != null; node = node.Next)
            {
                if (((node.Value.CsTokenType == CsTokenType.SingleLineComment) || (node.Value.CsTokenType == CsTokenType.MultiLineComment)) || (node.Value.CsTokenType == CsTokenType.Attribute))
                {
                    int num4 = ParameterPrewordOffset(node);
                    if (((lineNumber > 0) && (node.Value.LineNumber == lineNumber)) && (num2 > 0))
                    {
                        num4--;
                    }
                    num += num4;
                    num2 = num4;
                    lineNumber = node.Value.Location.EndPoint.LineNumber;
                }
                else if ((node.Value.CsTokenType != CsTokenType.WhiteSpace) && (node.Value.CsTokenType != CsTokenType.EndOfLine))
                {
                    return num;
                }
            }
            return num;
        }

        private static int MeasureCommentLinesBetween(Microsoft.StyleCop.Node<CsToken> start, Microsoft.StyleCop.Node<CsToken> end, bool includeAttributes)
        {
            int num = 0;
            int num2 = -1;
            int lineNumber = -1;
            for (Microsoft.StyleCop.Node<CsToken> node = start.Next; (node != null) && (node != end); node = node.Next)
            {
                if (((node.Value.CsTokenType == CsTokenType.SingleLineComment) || (node.Value.CsTokenType == CsTokenType.MultiLineComment)) || ((node.Value.CsTokenType == CsTokenType.Attribute) && includeAttributes))
                {
                    int num4 = ParameterPrewordOffset(node);
                    if (((lineNumber > 0) && (node.Value.LineNumber == lineNumber)) && (num2 > 0))
                    {
                        num4--;
                    }
                    num += num4;
                    num2 = num4;
                    lineNumber = node.Value.Location.EndPoint.LineNumber;
                }
            }
            return num;
        }

        private static int ParameterPrewordOffset(Microsoft.StyleCop.Node<CsToken> tokenNode)
        {
            for (Microsoft.StyleCop.Node<CsToken> node = tokenNode.Next; node != null; node = node.Next)
            {
                CsTokenType csTokenType = node.Value.CsTokenType;
                if (csTokenType == CsTokenType.EndOfLine)
                {
                    return tokenNode.Value.Location.LineSpan;
                }
                if (((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.MultiLineComment)) && ((csTokenType != CsTokenType.SingleLineComment) && (csTokenType != CsTokenType.Attribute)))
                {
                    return Math.Max(0, node.Value.Location.StartPoint.LineNumber - tokenNode.Value.Location.StartPoint.LineNumber);
                }
            }
            return 0;
        }

        private bool ProcessElement(CsElement element, CsElement parentElement, object context)
        {
            this.CheckMethodParameters(element);
            this.CheckForRegionsInElement(element, (Settings) context);
            return true;
        }

        private bool ProcessExpression(Expression expression, Expression parentExpression, Statement parentStatement, CsElement parentElement, object context)
        {
            if (!parentElement.Generated)
            {
                if (expression.ExpressionType == ExpressionType.Query)
                {
                    this.CheckQueryExpression(parentElement, (QueryExpression) expression);
                }
                else if (expression.ExpressionType == ExpressionType.MethodInvocation)
                {
                    this.CheckMethodInvocationParameters(parentElement, (MethodInvocationExpression) expression);
                }
            }
            return true;
        }

        private bool ProcessQueryClauses(CsElement element, QueryExpression expression, ICollection<QueryClause> clauses, ref QueryClause previousClause, ref bool clauseOnSameLine, ref bool clauseOnSeparateLine)
        {
            foreach (QueryClause clause in clauses)
            {
                if (previousClause != null)
                {
                    int lineNumber = previousClause.Location.EndPoint.LineNumber;
                    if (previousClause.QueryClauseType == QueryClauseType.Continuation)
                    {
                        lineNumber = ((QueryContinuationClause) previousClause).Variable.Location.LineNumber;
                    }
                    if (clause.LineNumber == lineNumber)
                    {
                        if (previousClause.Location.LineSpan > 1)
                        {
                            base.AddViolation(element, clause.LineNumber, Microsoft.StyleCop.CSharp.Rules.QueryClauseMustBeginOnNewLineWhenPreviousClauseSpansMultipleLines, new object[0]);
                            return false;
                        }
                        if (clause.QueryClauseType != QueryClauseType.Continuation)
                        {
                            if (clauseOnSeparateLine)
                            {
                                base.AddViolation(element, clause.LineNumber, Microsoft.StyleCop.CSharp.Rules.QueryClausesMustBeOnSeparateLinesOrAllOnOneLine, new object[0]);
                                return false;
                            }
                            if (clause.Location.LineSpan > 1)
                            {
                                base.AddViolation(element, clause.LineNumber, Microsoft.StyleCop.CSharp.Rules.QueryClausesSpanningMultipleLinesMustBeginOnOwnLine, new object[0]);
                                return false;
                            }
                            clauseOnSameLine = true;
                        }
                    }
                    else if (clause.LineNumber == (lineNumber + 1))
                    {
                        if (clauseOnSameLine)
                        {
                            base.AddViolation(element, clause.LineNumber, Microsoft.StyleCop.CSharp.Rules.QueryClausesMustBeOnSeparateLinesOrAllOnOneLine, new object[0]);
                            return false;
                        }
                        clauseOnSeparateLine = true;
                    }
                    else if (clause.LineNumber > (lineNumber + 1))
                    {
                        base.AddViolation(element, clause.LineNumber, Microsoft.StyleCop.CSharp.Rules.QueryClauseMustFollowPreviousClause, new object[0]);
                        return false;
                    }
                }
                previousClause = clause;
                if (clause.QueryClauseType == QueryClauseType.Continuation)
                {
                    QueryContinuationClause clause2 = (QueryContinuationClause) clause;
                    if (!this.ProcessQueryClauses(element, expression, clause2.ChildClauses, ref previousClause, ref clauseOnSameLine, ref clauseOnSeparateLine))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private class ArgumentList : ReadabilityRules.IArgumentList
        {
            private IList<Expression> arguments;

            public ArgumentList(IList<Expression> arguments)
            {
                this.arguments = arguments;
            }

            public CodeLocation Location(int index)
            {
                CsTokenList tokens = this.arguments[index].Tokens;
                CsToken token = null;
                for (Microsoft.StyleCop.Node<CsToken> node = tokens.First.Previous; node != null; node = node.Previous)
                {
                    if (((node.Value.CsTokenType == CsTokenType.Comma) || (node.Value.CsTokenType == CsTokenType.OpenSquareBracket)) || (node.Value.CsTokenType == CsTokenType.OpenParenthesis))
                    {
                        for (node = node.Next; node != null; node = node.Next)
                        {
                            if (((node.Value.CsTokenType != CsTokenType.WhiteSpace) && (node.Value.CsTokenType != CsTokenType.EndOfLine)) && ((node.Value.CsTokenType != CsTokenType.SingleLineComment) && (node.Value.CsTokenType != CsTokenType.MultiLineComment)))
                            {
                                token = node.Value;
                                break;
                            }
                        }
                        break;
                    }
                }
                if (token != null)
                {
                    return CodeLocation.Join(token.Location, tokens.Last.Value.Location);
                }
                return this.arguments[index].Location;
            }

            public bool MaySpanMultipleLines(int index)
            {
                if (index != 0)
                {
                    Expression expression = this.arguments[index];
                    if (expression.ExpressionType != ExpressionType.Lambda)
                    {
                        return (expression.ExpressionType == ExpressionType.AnonymousMethod);
                    }
                }
                return true;
            }

            public CsTokenList Tokens(int index)
            {
                return this.arguments[index].Tokens;
            }

            public int Count
            {
                get
                {
                    return this.arguments.Count;
                }
            }
        }

        private interface IArgumentList
        {
            CodeLocation Location(int index);
            bool MaySpanMultipleLines(int index);
            CsTokenList Tokens(int index);

            int Count { get; }
        }

        private class ParameterList : ReadabilityRules.IArgumentList
        {
            private IList<Parameter> parameters;

            public ParameterList(IList<Parameter> parameters)
            {
                this.parameters = parameters;
            }

            public CodeLocation Location(int index)
            {
                CsTokenList tokens = this.parameters[index].Tokens;
                CsToken token = null;
                for (Microsoft.StyleCop.Node<CsToken> node = tokens.First.Previous; node != null; node = node.Previous)
                {
                    if (((node.Value.CsTokenType == CsTokenType.Comma) || (node.Value.CsTokenType == CsTokenType.OpenSquareBracket)) || (node.Value.CsTokenType == CsTokenType.OpenParenthesis))
                    {
                        for (node = node.Next; node != null; node = node.Next)
                        {
                            if ((((node.Value.CsTokenType != CsTokenType.Attribute) && (node.Value.CsTokenType != CsTokenType.WhiteSpace)) && ((node.Value.CsTokenType != CsTokenType.EndOfLine) && (node.Value.CsTokenType != CsTokenType.SingleLineComment))) && (node.Value.CsTokenType != CsTokenType.MultiLineComment))
                            {
                                token = node.Value;
                                break;
                            }
                        }
                        break;
                    }
                }
                if (token != null)
                {
                    return CodeLocation.Join(token.Location, tokens.Last.Value.Location);
                }
                return this.parameters[index].Location;
            }

            public bool MaySpanMultipleLines(int index)
            {
                return false;
            }

            public CsTokenList Tokens(int index)
            {
                return this.parameters[index].Tokens;
            }

            public int Count
            {
                get
                {
                    return this.parameters.Count;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Settings
        {
            public bool DoNotUseRegions;
            public bool DoNotPlaceRegionsWithinElements;
        }
    }
}

