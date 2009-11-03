namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification="Class is split across multiple files for added maintainability."), SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification="The class does not create anything that it should dispose.")]
    internal class CodeParser
    {
        private static readonly string[] ClassModifiers = new string[] { "new", "unsafe", "abstract", "sealed", "static", "partial" };
        private static readonly string[] ConstructorModifiers = new string[] { "unsafe", "static", "extern" };
        private static readonly string[] DelegateModifiers = new string[] { "new", "unsafe" };
        private static readonly string[] DestructorModifiers = new string[] { "unsafe", "extern", "static" };
        private CsDocument document;
        private static readonly string[] EnumModifiers = new string[] { "new" };
        private static readonly string[] EventModifiers = new string[] { "new", "unsafe", "static", "virtual", "sealed", "override", "abstract", "extern" };
        private static readonly string[] FieldModifiers = new string[] { "new", "unsafe", "const", "readonly", "static", "volatile", "fixed" };
        private static readonly string[] IndexerModifiers = new string[] { "new", "unsafe", "virtual", "sealed", "override", "abstract", "extern" };
        private CodeLexer lexer;
        private static readonly string[] MethodModifiers = new string[] { "new", "unsafe", "static", "virtual", "sealed", "override", "abstract", "extern", "partial", "implicit", "explicit" };
        private CsParser parser;
        private static readonly string[] PropertyModifiers = new string[] { "new", "unsafe", "static", "virtual", "sealed", "override", "abstract", "extern" };
        private SymbolManager symbols;
        private MasterList<CsToken> tokens;

        public CodeParser(CsParser parser, CodeLexer lexer)
        {
            this.tokens = new MasterList<CsToken>();
            this.parser = parser;
            this.lexer = lexer;
        }

        public CodeParser(CsParser parser, SymbolManager symbols)
        {
            this.tokens = new MasterList<CsToken>();
            this.parser = parser;
            this.symbols = symbols;
        }

        private static void AddElementToPartialElementsList(CsElement element, Dictionary<string, List<CsElement>> partialElements)
        {
            if (element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial }) && (element is ClassBase))
            {
                if (partialElements == null)
                {
                    throw new SyntaxException(element.Document.SourceCode, element.LineNumber);
                }
                List<CsElement> list = null;
                lock (partialElements)
                {
                    partialElements.TryGetValue(element.FullNamespaceName, out list);
                    if (list == null)
                    {
                        list = new List<CsElement>();
                        partialElements.Add(element.FullNamespaceName, list);
                    }
                    else if ((list.Count > 0) && (list[0].ElementType != element.ElementType))
                    {
                        throw new SyntaxException(element.Document.SourceCode, element.Declaration.Tokens.First.Value.LineNumber);
                    }
                }
                list.Add(element);
            }
        }

        internal static string AddQualifications(ICollection<Parameter> parameters, string fullyQualifiedName)
        {
            foreach (Parameter parameter in parameters)
            {
                fullyQualifiedName = fullyQualifiedName + "%" + parameter.Type;
            }
            return fullyQualifiedName;
        }

        private void AddRuleSuppressionsForElement(CsElement element)
        {
            if (((element != null) && (element.Attributes != null)) && (element.Attributes.Count > 0))
            {
                foreach (Microsoft.StyleCop.CSharp.Attribute attribute in element.Attributes)
                {
                    if (attribute.AttributeExpressions != null)
                    {
                        foreach (AttributeExpression expression in attribute.AttributeExpressions)
                        {
                            if (expression.Initialization != null)
                            {
                                string str;
                                string str2;
                                string str3;
                                MethodInvocationExpression initialization = expression.Initialization as MethodInvocationExpression;
                                if (((initialization != null) && IsCodeAnalysisSuppression(initialization.Name)) && TryCrackCodeAnalysisSuppression(initialization, out str, out str2, out str3))
                                {
                                    this.parser.AddRuleSuppression(element, str, str2, str3);
                                }
                            }
                        }
                        continue;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="May be simplified later.")]
        private int AdvanceToClosingGenericSymbol(int startIndex)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(startIndex);
            while (nextCodeSymbolIndex != -1)
            {
                Symbol symbol = this.symbols.Peek(nextCodeSymbolIndex);
                if (symbol.SymbolType == SymbolType.GreaterThan)
                {
                    if (!flag2 && !flag3)
                    {
                        return nextCodeSymbolIndex;
                    }
                    return -1;
                }
                if (symbol.SymbolType == SymbolType.Comma)
                {
                    if ((flag2 && !flag4) || (flag3 || (!flag && !flag4)))
                    {
                        return -1;
                    }
                    flag2 = true;
                    flag = false;
                }
                else if ((symbol.SymbolType == SymbolType.Dot) || (symbol.SymbolType == SymbolType.QualifiedAlias))
                {
                    if ((!flag || flag2) || flag3)
                    {
                        return -1;
                    }
                    flag3 = true;
                    flag = false;
                }
                else if (symbol.SymbolType == SymbolType.OpenSquareBracket)
                {
                    if (flag4 || flag2)
                    {
                        return -1;
                    }
                    flag4 = true;
                    flag = false;
                }
                else if (symbol.SymbolType == SymbolType.CloseSquareBracket)
                {
                    if (!flag4)
                    {
                        return -1;
                    }
                    flag2 = false;
                    flag4 = false;
                    flag = true;
                }
                else if (symbol.SymbolType == SymbolType.QuestionMark)
                {
                    if (!flag)
                    {
                        return -1;
                    }
                }
                else if (symbol.SymbolType == SymbolType.Other)
                {
                    if (flag || flag4)
                    {
                        return -1;
                    }
                    flag = true;
                    flag2 = false;
                    flag3 = false;
                }
                else if (symbol.SymbolType == SymbolType.LessThan)
                {
                    if (!flag)
                    {
                        return -1;
                    }
                    nextCodeSymbolIndex = this.AdvanceToClosingGenericSymbol(nextCodeSymbolIndex + 1);
                    if (nextCodeSymbolIndex == -1)
                    {
                        return nextCodeSymbolIndex;
                    }
                }
                else
                {
                    return -1;
                }
                nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex + 1);
            }
            return nextCodeSymbolIndex;
        }

        private int AdvanceToEndOfName(int startIndex)
        {
            bool flag;
            return this.AdvanceToEndOfName(startIndex, out flag);
        }

        private int AdvanceToEndOfName(int startIndex, out bool generic)
        {
            generic = false;
            Symbol symbol = this.symbols.Peek(startIndex);
            int count = startIndex;
            int num2 = startIndex;
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(count + 1);
            if ((nextCodeSymbolIndex != -1) && (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.LessThan))
            {
                nextCodeSymbolIndex = this.AdvanceToClosingGenericSymbol(nextCodeSymbolIndex + 1);
                if (nextCodeSymbolIndex != -1)
                {
                    count = nextCodeSymbolIndex;
                    num2 = count;
                    generic = true;
                }
            }
            bool flag = false;
        Label_0051:
            count = this.GetNextCodeSymbolIndex(count + 1);
            if (count != -1)
            {
                symbol = this.symbols.Peek(count);
                if (symbol.SymbolType == SymbolType.Other)
                {
                    if (!flag)
                    {
                        return num2;
                    }
                    int num4 = this.GetNextCodeSymbolIndex(count + 1);
                    if ((num4 != -1) && (this.symbols.Peek(num4).SymbolType == SymbolType.LessThan))
                    {
                        num4 = this.AdvanceToClosingGenericSymbol(num4 + 1);
                        if (num4 == -1)
                        {
                            return num2;
                        }
                        count = num4;
                    }
                    num2 = count;
                    flag = false;
                    goto Label_0051;
                }
                if ((symbol.SymbolType != SymbolType.Dot) && (symbol.SymbolType != SymbolType.QualifiedAlias))
                {
                    return num2;
                }
                if (!flag)
                {
                    flag = true;
                    goto Label_0051;
                }
            }
            return num2;
        }

        private void AdvanceToNextCodeSymbol()
        {
            this.AdvanceToNextCodeSymbol(SkipSymbols.All);
        }

        private void AdvanceToNextCodeSymbol(SkipSymbols skip)
        {
            for (Symbol symbol = this.symbols.Peek(1); symbol != null; symbol = this.symbols.Peek(1))
            {
                if ((symbol.SymbolType == SymbolType.WhiteSpace) && ((skip & SkipSymbols.WhiteSpace) != SkipSymbols.None))
                {
                    this.tokens.Add(new Whitespace(symbol.Text, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else if ((symbol.SymbolType == SymbolType.EndOfLine) && ((skip & SkipSymbols.EndOfLine) != SkipSymbols.None))
                {
                    this.tokens.Add(new CsToken(symbol.Text, CsTokenType.EndOfLine, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else if ((symbol.SymbolType == SymbolType.SingleLineComment) && ((skip & SkipSymbols.SingleLineComment) != SkipSymbols.None))
                {
                    this.tokens.Add(new CsToken(symbol.Text, CsTokenType.SingleLineComment, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else if ((symbol.SymbolType == SymbolType.MultiLineComment) && ((skip & SkipSymbols.MultiLineComment) != SkipSymbols.None))
                {
                    this.tokens.Add(new CsToken(symbol.Text, CsTokenType.MultiLineComment, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else if ((symbol.SymbolType == SymbolType.PreprocessorDirective) && ((skip & SkipSymbols.Preprocessor) != SkipSymbols.None))
                {
                    this.tokens.Add(this.GetPreprocessorDirectiveToken(symbol, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else
                {
                    if ((symbol.SymbolType != SymbolType.XmlHeaderLine) || ((skip & SkipSymbols.XmlHeader) == SkipSymbols.None))
                    {
                        break;
                    }
                    this.tokens.Add(new CsToken(symbol.Text, CsTokenType.XmlHeaderLine, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
            }
        }

        private void AdvanceToNextConditionalDirectiveCodeSymbol()
        {
            for (Symbol symbol = this.symbols.Peek(1); symbol != null; symbol = this.symbols.Peek(1))
            {
                if (symbol.SymbolType == SymbolType.WhiteSpace)
                {
                    this.tokens.Add(new Whitespace(symbol.Text, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else if (symbol.SymbolType == SymbolType.EndOfLine)
                {
                    this.tokens.Add(new CsToken(symbol.Text, CsTokenType.EndOfLine, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else if (symbol.SymbolType == SymbolType.SingleLineComment)
                {
                    this.tokens.Add(new CsToken(symbol.Text, CsTokenType.SingleLineComment, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
                else
                {
                    if (symbol.SymbolType != SymbolType.MultiLineComment)
                    {
                        break;
                    }
                    this.tokens.Add(new CsToken(symbol.Text, CsTokenType.MultiLineComment, symbol.Location, this.symbols.Generated));
                    this.symbols.Advance();
                }
            }
        }

        private static bool CheckPrecedence(ExpressionPrecedence previousPrecedence, ExpressionPrecedence nextPrecedence)
        {
            if (((previousPrecedence != ExpressionPrecedence.None) || (nextPrecedence != ExpressionPrecedence.None)) && ((previousPrecedence != ExpressionPrecedence.Conditional) || (nextPrecedence != ExpressionPrecedence.Conditional)))
            {
                return (previousPrecedence > nextPrecedence);
            }
            return true;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId="Microsoft.StyleCop.CSharp.SymbolManager.Combine(System.Int32,System.Int32,System.String,Microsoft.StyleCop.CSharp.SymbolType)", Justification="The literal represents a C# operator and is not localizable.")]
        private CsToken ConvertOperatorOverloadSymbol()
        {
            CsToken token = null;
            Symbol symbol = this.symbols.Peek(1);
            if (symbol != null)
            {
                if (symbol.SymbolType == SymbolType.GreaterThan)
                {
                    Symbol symbol2 = this.symbols.Peek(2);
                    if ((symbol2 != null) && (symbol2.SymbolType == SymbolType.GreaterThan))
                    {
                        symbol2 = this.symbols.Peek(3);
                        if ((symbol2 != null) && (symbol2.SymbolType == SymbolType.Equals))
                        {
                            this.symbols.Combine(1, 3, ">>=", SymbolType.RightShiftEquals);
                        }
                        else
                        {
                            this.symbols.Combine(1, 2, ">>", SymbolType.RightShift);
                        }
                    }
                    symbol = this.symbols.Peek(1);
                    token = new CsToken(symbol.Text, CsTokenType.Other, symbol.Location, this.symbols.Generated);
                    this.symbols.Advance();
                    return token;
                }
                token = new CsToken(symbol.Text, CsTokenType.Other, symbol.Location, this.symbols.Generated);
                this.symbols.Advance();
            }
            return token;
        }

        private CsToken ConvertSymbol(Symbol symbol, CsTokenType tokenType)
        {
            if (symbol.SymbolType == SymbolType.WhiteSpace)
            {
                return new Whitespace(symbol.Text, symbol.Location, this.symbols.Generated);
            }
            if (symbol.SymbolType == SymbolType.Number)
            {
                return new Microsoft.StyleCop.CSharp.Number(symbol.Text, symbol.Location, this.symbols.Generated);
            }
            if (symbol.SymbolType == SymbolType.PreprocessorDirective)
            {
                return this.GetPreprocessorDirectiveToken(symbol, this.symbols.Generated);
            }
            return new CsToken(symbol.Text, tokenType, CsTokenClass.Token, symbol.Location, this.symbols.Generated);
        }

        private LiteralExpression ConvertTypeExpression(Expression expression)
        {
            Microsoft.StyleCop.Node<CsToken> first = expression.Tokens.First;
            Microsoft.StyleCop.Node<CsToken> last = expression.Tokens.Last;
            List<CsToken> items = new List<CsToken>();
            foreach (CsToken token in expression.Tokens)
            {
                items.Add(token);
            }
            if ((first != null) && (expression.Tokens.First != null))
            {
                Microsoft.StyleCop.Node<CsToken> next = first.Next;
                if (!expression.Tokens.OutOfBounds(next))
                {
                    this.tokens.RemoveRange(next, expression.Tokens.Last);
                }
            }
            MasterList<CsToken> childTokens = new MasterList<CsToken>(items);
            TypeToken newItem = new TypeToken(childTokens, CodeLocation.Join<CsToken>(first, last), first.Value.Generated);
            return new LiteralExpression(new CsTokenList(this.tokens, first, first), this.tokens.Replace(first, newItem));
        }

        private static OperatorSymbol CreateOperatorToken(Symbol symbol, bool generated)
        {
            OperatorType type;
            OperatorCategory category;
            if (!GetOperatorType(symbol, out type, out category))
            {
                throw new InvalidOperationException();
            }
            return new OperatorSymbol(symbol.Text, category, type, symbol.Location, generated);
        }

        private SyntaxException CreateSyntaxException()
        {
            throw new SyntaxException(this.document.SourceCode, this.GetBestLineNumber());
        }

        private static string ExtractStringFromAttributeExpression(Expression expression)
        {
            if ((expression == null) || (expression.ExpressionType != ExpressionType.Literal))
            {
                return null;
            }
            LiteralExpression expression2 = (LiteralExpression) expression;
            if (expression2.Token.CsTokenType != CsTokenType.String)
            {
                return null;
            }
            string text = expression2.Token.Text;
            if ((text.StartsWith("\"", StringComparison.Ordinal) && text.EndsWith("\"", StringComparison.Ordinal)) && (text.Length >= 2))
            {
                return text.Substring(1, text.Length - 2);
            }
            return text;
        }

        internal static TypeToken ExtractTypeTokenFromLiteralExpression(LiteralExpression literal)
        {
            return (TypeToken) literal.TokenNode.Value;
        }

        internal static Microsoft.StyleCop.Node<CsToken> FindEndOfName(CsDocument document, CsTokenList tokens, Microsoft.StyleCop.Node<CsToken> startTokenNode)
        {
            Microsoft.StyleCop.Node<CsToken> node = startTokenNode;
            Microsoft.StyleCop.Node<CsToken> node2 = startTokenNode;
            if (node2 == null)
            {
                CsTokenType csTokenType = node2.Value.CsTokenType;
                if ((((csTokenType != CsTokenType.Other) && (csTokenType != CsTokenType.Get)) && ((csTokenType != CsTokenType.Set) && (csTokenType != CsTokenType.Add))) && (csTokenType != CsTokenType.Remove))
                {
                    throw new SyntaxException(document.SourceCode, (node2 == null) ? document.MasterTokenList.Last.Value.LineNumber : node2.Value.LineNumber);
                }
            }
            bool flag = false;
            for (Microsoft.StyleCop.Node<CsToken> node3 = tokens.First; !tokens.OutOfBounds(node3); node3 = node3.Next)
            {
                CsTokenType type2 = node3.Value.CsTokenType;
                switch (type2)
                {
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                    case CsTokenType.PreprocessorDirective:
                        break;

                    default:
                        if (flag)
                        {
                            if (type2 != CsTokenType.Other)
                            {
                                throw new SyntaxException(document.SourceCode, node3.Value.LineNumber);
                            }
                            node = node2;
                            flag = false;
                        }
                        else
                        {
                            if (!(node3.Value.Text == ".") && !(node3.Value.Text == "::"))
                            {
                                return node;
                            }
                            flag = true;
                        }
                        break;
                }
            }
            return node;
        }

        private AnonymousMethodExpression GetAnonymousMethodExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Delegate, SymbolType.Delegate));
            Symbol nextSymbol = this.GetNextSymbol();
            ICollection<Parameter> items = null;
            if (nextSymbol.SymbolType == SymbolType.OpenParenthesis)
            {
                items = this.ParseAnonymousMethodParameterList(unsafeCode);
            }
            AnonymousMethodExpression parent = new AnonymousMethodExpression();
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Microsoft.StyleCop.Node<CsToken> node3 = this.ParseStatementScope(parent, unsafeCode);
            if (node3 == null)
            {
                throw this.CreateSyntaxException();
            }
            bracketToken.MatchingBracketNode = node3;
            ((Bracket) node3.Value).MatchingBracketNode = node2;
            parent.Tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            if ((items != null) && (items.Count > 0))
            {
                parent.AddParameters(items);
            }
            if ((parent.Parameters != null) && (parent.Parameters.Count > 0))
            {
                foreach (Parameter parameter in parent.Parameters)
                {
                    parent.Variables.Add(new Variable(parameter.Type, parameter.Name, VariableModifiers.None, parameter.Location.StartPoint, parameter.Generated));
                }
            }
            return parent;
        }

        private CollectionInitializerExpression GetAnonymousTypeInitializerExpression(bool unsafeCode)
        {
            this.GetNextSymbol(SymbolType.OpenCurlyBracket);
            CollectionInitializerExpression collectionInitializerExpression = this.GetCollectionInitializerExpression(unsafeCode);
            if ((collectionInitializerExpression == null) || (collectionInitializerExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            foreach (Expression expression2 in collectionInitializerExpression.ChildExpressions)
            {
                if (((expression2.ExpressionType != ExpressionType.Literal) && (expression2.ExpressionType != ExpressionType.MemberAccess)) && (expression2.ExpressionType != ExpressionType.Assignment))
                {
                    throw this.CreateSyntaxException();
                }
            }
            return collectionInitializerExpression;
        }

        private IList<Expression> GetArgumentList(SymbolType closingSymbol, bool unsafeCode)
        {
            List<Expression> list = new List<Expression>();
            while (true)
            {
                Symbol nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType == closingSymbol)
                {
                    return list.ToArray();
                }
                if (nextSymbol.SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                }
                else if (nextSymbol.SymbolType == SymbolType.Ref)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Ref, SymbolType.Ref));
                }
                else if (nextSymbol.SymbolType == SymbolType.Out)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Out, SymbolType.Out));
                }
                else if (nextSymbol.SymbolType == SymbolType.Params)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Params, SymbolType.Params));
                }
                else
                {
                    list.Add(this.GetNextExpression(ExpressionPrecedence.None, unsafeCode));
                }
            }
        }

        private ArithmeticExpression GetArithmeticExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            ArithmeticExpression expression = null;
            ArithmeticExpression.Operator addition;
            OperatorSymbol item = this.PeekOperatorToken();
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (!CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                return expression;
            }
            this.symbols.Advance();
            this.tokens.Add(item);
            Expression operatorRightHandExpression = this.GetOperatorRightHandExpression(operatorPrecedence, unsafeCode);
            CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
            switch (item.SymbolType)
            {
                case OperatorType.Plus:
                    addition = ArithmeticExpression.Operator.Addition;
                    break;

                case OperatorType.Minus:
                    addition = ArithmeticExpression.Operator.Subtraction;
                    break;

                case OperatorType.Multiplication:
                    addition = ArithmeticExpression.Operator.Multiplication;
                    break;

                case OperatorType.Division:
                    addition = ArithmeticExpression.Operator.Division;
                    break;

                case OperatorType.Mod:
                    addition = ArithmeticExpression.Operator.Mod;
                    break;

                case OperatorType.LeftShift:
                    addition = ArithmeticExpression.Operator.LeftShift;
                    break;

                case OperatorType.RightShift:
                    addition = ArithmeticExpression.Operator.RightShift;
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return new ArithmeticExpression(tokens, addition, leftHandSide, operatorRightHandExpression);
        }

        private ArrayAccessExpression GetArrayAccessExpression(Expression array, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            ArrayAccessExpression expression = null;
            if (CheckPrecedence(previousPrecedence, ExpressionPrecedence.Primary))
            {
                Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenSquareBracket, SymbolType.OpenSquareBracket);
                Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
                ICollection<Expression> argumentList = this.GetArgumentList(SymbolType.CloseSquareBracket, unsafeCode);
                Bracket item = this.GetBracketToken(CsTokenType.CloseSquareBracket, SymbolType.CloseSquareBracket);
                Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(item);
                bracketToken.MatchingBracketNode = node2;
                item.MatchingBracketNode = node;
                Microsoft.StyleCop.Node<CsToken> first = array.Tokens.First;
                CsTokenList tokens = new CsTokenList(this.tokens, first, this.tokens.Last);
                expression = new ArrayAccessExpression(tokens, array, argumentList);
            }
            return expression;
        }

        private ArrayInitializerExpression GetArrayInitializerExpression(bool unsafeCode)
        {
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(bracketToken);
            List<Expression> list = new List<Expression>();
            while (true)
            {
                Symbol nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType == SymbolType.OpenCurlyBracket)
                {
                    list.Add(this.GetArrayInitializerExpression(unsafeCode));
                }
                else
                {
                    if (nextSymbol.SymbolType == SymbolType.CloseCurlyBracket)
                    {
                        break;
                    }
                    list.Add(this.GetNextExpression(ExpressionPrecedence.None, unsafeCode));
                }
                if (this.GetNextSymbol().SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                }
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseCurlyBracket, SymbolType.CloseCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node2;
            item.MatchingBracketNode = firstItemNode;
            return new ArrayInitializerExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), list.ToArray());
        }

        private AsExpression GetAsExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            AsExpression expression = null;
            if (!CheckPrecedence(previousPrecedence, ExpressionPrecedence.Relational))
            {
                return expression;
            }
            this.tokens.Add(this.GetToken(CsTokenType.As, SymbolType.As));
            this.GetNextSymbol(SymbolType.Other);
            LiteralExpression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, true);
            if ((typeTokenExpression == null) || (typeTokenExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            return new AsExpression(new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last), leftHandSide, typeTokenExpression);
        }

        private AssignmentExpression GetAssignmentExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            AssignmentExpression expression = null;
            AssignmentExpression.Operator equals;
            OperatorSymbol item = this.PeekOperatorToken();
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (!CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                return expression;
            }
            this.symbols.Advance();
            this.tokens.Add(item);
            Expression operatorRightHandExpression = this.GetOperatorRightHandExpression(operatorPrecedence, unsafeCode);
            CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
            switch (item.SymbolType)
            {
                case OperatorType.Equals:
                    equals = AssignmentExpression.Operator.Equals;
                    break;

                case OperatorType.PlusEquals:
                    equals = AssignmentExpression.Operator.PlusEquals;
                    break;

                case OperatorType.MinusEquals:
                    equals = AssignmentExpression.Operator.MinusEquals;
                    break;

                case OperatorType.MultiplicationEquals:
                    equals = AssignmentExpression.Operator.MultiplicationEquals;
                    break;

                case OperatorType.DivisionEquals:
                    equals = AssignmentExpression.Operator.DivisionEquals;
                    break;

                case OperatorType.LeftShiftEquals:
                    equals = AssignmentExpression.Operator.LeftShiftEquals;
                    break;

                case OperatorType.RightShiftEquals:
                    equals = AssignmentExpression.Operator.RightShiftEquals;
                    break;

                case OperatorType.AndEquals:
                    equals = AssignmentExpression.Operator.AndEquals;
                    break;

                case OperatorType.OrEquals:
                    equals = AssignmentExpression.Operator.OrEquals;
                    break;

                case OperatorType.XorEquals:
                    equals = AssignmentExpression.Operator.XorEquals;
                    break;

                case OperatorType.ModEquals:
                    equals = AssignmentExpression.Operator.ModEquals;
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return new AssignmentExpression(tokens, equals, leftHandSide, operatorRightHandExpression);
        }

        private CatchStatement GetAttachedCatchStatement(TryStatement tryStatement, bool unsafeCode)
        {
            CatchStatement statement = null;
            if (this.GetNextSymbol().SymbolType == SymbolType.Catch)
            {
                CsToken item = this.GetToken(CsTokenType.Catch, SymbolType.Catch);
                Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
                Expression classExpression = null;
                if (this.GetNextSymbol().SymbolType == SymbolType.OpenParenthesis)
                {
                    Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
                    Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
                    if (this.GetNextSymbol().SymbolType == SymbolType.Other)
                    {
                        classExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode, true, true);
                    }
                    Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
                    Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
                    bracketToken.MatchingBracketNode = node3;
                    bracket2.MatchingBracketNode = node2;
                }
                BlockStatement nextStatement = this.GetNextStatement(unsafeCode) as BlockStatement;
                if (nextStatement == null)
                {
                    throw new SyntaxException(this.document.SourceCode, item.LineNumber);
                }
                CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
                statement = new CatchStatement(tokens, tryStatement, classExpression, nextStatement);
                if ((statement.ClassType != null) && !string.IsNullOrEmpty(statement.Identifier))
                {
                    Variable variable = new Variable(statement.ClassType, statement.Identifier, VariableModifiers.None, statement.ClassType.Location.StartPoint, statement.ClassType.Generated);
                    if (!statement.Variables.Contains(statement.Identifier))
                    {
                        statement.Variables.Add(variable);
                    }
                }
            }
            return statement;
        }

        private ElseStatement GetAttachedElseStatement(bool unsafeCode)
        {
            ElseStatement statement = null;
            if (this.GetNextSymbol().SymbolType == SymbolType.Else)
            {
                Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Else, SymbolType.Else));
                Expression conditionExpression = null;
                Symbol nextSymbol = this.GetNextSymbol();
                if ((nextSymbol != null) && (nextSymbol.SymbolType == SymbolType.If))
                {
                    this.tokens.Add(this.GetToken(CsTokenType.If, SymbolType.If));
                    Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
                    Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
                    conditionExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                    if (conditionExpression == null)
                    {
                        throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                    }
                    Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
                    Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
                    bracketToken.MatchingBracketNode = node3;
                    item.MatchingBracketNode = node2;
                }
                Statement nextStatement = this.GetNextStatement(unsafeCode);
                if (nextStatement == null)
                {
                    throw this.CreateSyntaxException();
                }
                CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
                statement = new ElseStatement(tokens, conditionExpression);
                statement.EmbeddedStatement = nextStatement;
                ElseStatement attachedElseStatement = this.GetAttachedElseStatement(unsafeCode);
                if (attachedElseStatement != null)
                {
                    statement.AttachedElseStatement = attachedElseStatement;
                }
            }
            return statement;
        }

        private FinallyStatement GetAttachedFinallyStatement(TryStatement tryStatement, bool unsafeCode)
        {
            FinallyStatement statement = null;
            if (this.GetNextSymbol().SymbolType != SymbolType.Finally)
            {
                return statement;
            }
            CsToken item = this.GetToken(CsTokenType.Finally, SymbolType.Finally);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            BlockStatement nextStatement = this.GetNextStatement(unsafeCode) as BlockStatement;
            if (nextStatement == null)
            {
                throw new SyntaxException(this.document.SourceCode, item.LineNumber);
            }
            return new FinallyStatement(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), tryStatement, nextStatement);
        }

        private Microsoft.StyleCop.CSharp.Attribute GetAttribute(bool unsafeCode)
        {
            CodeParser parser = new CodeParser(this.parser, this.symbols);
            return parser.ParseAttribute(unsafeCode);
        }

        private int GetBestLineNumber()
        {
            int lineNumber = 1;
            if (this.symbols.Current != null)
            {
                return this.symbols.Current.LineNumber;
            }
            if (this.tokens.Count > 1)
            {
                lineNumber = this.tokens.Last.Value.LineNumber;
            }
            return lineNumber;
        }

        private Bracket GetBracketToken(CsTokenType tokenType, SymbolType symbolType)
        {
            Symbol nextSymbol = this.GetNextSymbol(symbolType);
            this.symbols.Advance();
            return new Bracket(nextSymbol.Text, tokenType, nextSymbol.Location, this.symbols.Generated);
        }

        private CastExpression GetCastExpression(bool unsafeCode)
        {
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(bracketToken);
            this.GetNextSymbol(SymbolType.Other);
            LiteralExpression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, true);
            if ((typeTokenExpression == null) || (typeTokenExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node2;
            item.MatchingBracketNode = firstItemNode;
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Unary, unsafeCode);
            if ((nextExpression == null) || (nextExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            return new CastExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), typeTokenExpression, nextExpression);
        }

        private CheckedExpression GetCheckedExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Checked, SymbolType.Checked));
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node2;
            return new CheckedExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression);
        }

        private CollectionInitializerExpression GetCollectionInitializerExpression(bool unsafeCode)
        {
            Symbol symbol;
            List<Expression> initializers = new List<Expression>();
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(bracketToken);
        Label_001C:
            symbol = this.GetNextSymbol();
            if (symbol.SymbolType != SymbolType.CloseCurlyBracket)
            {
                Expression collectionInitializerExpression = null;
                if (symbol.SymbolType == SymbolType.OpenCurlyBracket)
                {
                    collectionInitializerExpression = this.GetCollectionInitializerExpression(unsafeCode);
                }
                else
                {
                    collectionInitializerExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                }
                initializers.Add(collectionInitializerExpression);
                if (this.GetNextSymbol().SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                    if (this.GetNextSymbol().SymbolType != SymbolType.CloseCurlyBracket)
                    {
                        goto Label_001C;
                    }
                }
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseCurlyBracket, SymbolType.CloseCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> lastItemNode = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = lastItemNode;
            item.MatchingBracketNode = firstItemNode;
            return new CollectionInitializerExpression(new CsTokenList(this.tokens, firstItemNode, lastItemNode), initializers);
        }

        private ConditionalCompilationDirective GetConditionalCompilationDirective(Symbol preprocessorSymbol, ConditionalCompilationDirectiveType type, int startIndex, bool generated)
        {
            Expression body = null;
            if ((type != ConditionalCompilationDirectiveType.Endif) && (startIndex < preprocessorSymbol.Text.Length))
            {
                body = GetConditionalPreprocessorBodyExpression(this.parser, this.document.SourceCode, preprocessorSymbol, startIndex);
            }
            return new ConditionalCompilationDirective(preprocessorSymbol.Text, type, body, preprocessorSymbol.Location, generated);
        }

        private ConditionalExpression GetConditionalExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            ConditionalExpression expression = null;
            if (CheckPrecedence(previousPrecedence, ExpressionPrecedence.Conditional))
            {
                this.tokens.Add(this.GetOperatorToken(OperatorType.ConditionalQuestionMark));
                Expression operatorRightHandExpression = this.GetOperatorRightHandExpression(ExpressionPrecedence.Conditional, unsafeCode);
                this.tokens.Add(this.GetOperatorToken(OperatorType.ConditionalColon));
                Expression falseValue = this.GetOperatorRightHandExpression(ExpressionPrecedence.None, unsafeCode);
                CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
                expression = new ConditionalExpression(tokens, leftHandSide, operatorRightHandExpression, falseValue);
            }
            return expression;
        }

        private ConditionalLogicalExpression GetConditionalLogicalExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            ConditionalLogicalExpression expression = null;
            ConditionalLogicalExpression.Operator and;
            OperatorSymbol item = this.PeekOperatorToken();
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (!CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                return expression;
            }
            this.symbols.Advance();
            this.tokens.Add(item);
            Expression operatorRightHandExpression = this.GetOperatorRightHandExpression(operatorPrecedence, unsafeCode);
            CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
            switch (item.SymbolType)
            {
                case OperatorType.ConditionalAnd:
                    and = ConditionalLogicalExpression.Operator.And;
                    break;

                case OperatorType.ConditionalOr:
                    and = ConditionalLogicalExpression.Operator.Or;
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return new ConditionalLogicalExpression(tokens, and, leftHandSide, operatorRightHandExpression);
        }

        private ConditionalLogicalExpression GetConditionalPreprocessorAndOrExpression(SourceCode sourceCode, Expression leftHandSide, ExpressionPrecedence previousPrecedence)
        {
            ConditionalLogicalExpression expression = null;
            OperatorType type;
            OperatorCategory category;
            ConditionalLogicalExpression.Operator and;
            this.AdvanceToNextConditionalDirectiveCodeSymbol();
            Symbol symbol = this.symbols.Peek(1);
            if (symbol == null)
            {
                throw new SyntaxException(sourceCode, symbol.LineNumber);
            }
            GetOperatorType(symbol, out type, out category);
            OperatorSymbol item = new OperatorSymbol(symbol.Text, category, type, symbol.Location, this.symbols.Generated);
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (!CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                return expression;
            }
            this.symbols.Advance();
            this.tokens.Add(item);
            Expression nextConditionalPreprocessorExpression = this.GetNextConditionalPreprocessorExpression(sourceCode, operatorPrecedence);
            if (nextConditionalPreprocessorExpression == null)
            {
                throw new SyntaxException(sourceCode, item.LineNumber);
            }
            CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
            switch (item.SymbolType)
            {
                case OperatorType.ConditionalAnd:
                    and = ConditionalLogicalExpression.Operator.And;
                    break;

                case OperatorType.ConditionalOr:
                    and = ConditionalLogicalExpression.Operator.Or;
                    break;

                default:
                    throw new SyntaxException(sourceCode, item.LineNumber);
            }
            return new ConditionalLogicalExpression(tokens, and, leftHandSide, nextConditionalPreprocessorExpression);
        }

        internal static Expression GetConditionalPreprocessorBodyExpression(CsParser parser, SourceCode sourceCode, Symbol preprocessorSymbol, int startIndex)
        {
            string s = preprocessorSymbol.Text.Substring(startIndex, preprocessorSymbol.Text.Length - startIndex).Trim();
            if (s.Length > 0)
            {
                StringReader code = new StringReader(s);
                CodeLexer lexer = new CodeLexer(parser, sourceCode, new CodeReader(code));
                SymbolManager symbols = new SymbolManager(lexer.GetSymbols(sourceCode, null));
                CodeParser parser2 = new CodeParser(parser, symbols);
                return parser2.GetNextConditionalPreprocessorExpression(sourceCode);
            }
            return null;
        }

        private LiteralExpression GetConditionalPreprocessorConstantExpression()
        {
            Symbol symbol = this.symbols.Peek(1);
            this.symbols.Advance();
            CsToken item = new CsToken(symbol.Text, CsTokenType.Other, symbol.Location, this.symbols.Generated);
            return new LiteralExpression(this.tokens, this.tokens.InsertLast(item));
        }

        private RelationalExpression GetConditionalPreprocessorEqualityExpression(SourceCode sourceCode, Expression leftHandSide, ExpressionPrecedence previousPrecedence)
        {
            RelationalExpression expression = null;
            OperatorType type;
            OperatorCategory category;
            RelationalExpression.Operator equalTo;
            this.AdvanceToNextConditionalDirectiveCodeSymbol();
            Symbol symbol = this.symbols.Peek(1);
            if (symbol == null)
            {
                throw new SyntaxException(sourceCode, symbol.LineNumber);
            }
            GetOperatorType(symbol, out type, out category);
            OperatorSymbol item = new OperatorSymbol(symbol.Text, category, type, symbol.Location, this.symbols.Generated);
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (!CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                return expression;
            }
            this.symbols.Advance();
            this.tokens.Add(item);
            Expression nextConditionalPreprocessorExpression = this.GetNextConditionalPreprocessorExpression(sourceCode, operatorPrecedence);
            if (nextConditionalPreprocessorExpression == null)
            {
                throw new SyntaxException(sourceCode, item.LineNumber);
            }
            CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
            switch (item.SymbolType)
            {
                case OperatorType.ConditionalEquals:
                    equalTo = RelationalExpression.Operator.EqualTo;
                    break;

                case OperatorType.NotEquals:
                    equalTo = RelationalExpression.Operator.NotEqualTo;
                    break;

                default:
                    throw new SyntaxException(sourceCode, item.LineNumber);
            }
            return new RelationalExpression(tokens, equalTo, leftHandSide, nextConditionalPreprocessorExpression);
        }

        private Expression GetConditionalPreprocessorExpressionExtension(SourceCode sourceCode, Expression leftSide, ExpressionPrecedence previousPrecedence)
        {
            OperatorType type;
            OperatorCategory category;
            this.AdvanceToNextConditionalDirectiveCodeSymbol();
            Symbol symbol = this.symbols.Peek(1);
            if (((symbol != null) && (symbol.SymbolType != SymbolType.CloseParenthesis)) && GetOperatorType(symbol, out type, out category))
            {
                switch (type)
                {
                    case OperatorType.ConditionalEquals:
                    case OperatorType.NotEquals:
                        return this.GetConditionalPreprocessorEqualityExpression(sourceCode, leftSide, previousPrecedence);

                    case OperatorType.ConditionalAnd:
                    case OperatorType.ConditionalOr:
                        return this.GetConditionalPreprocessorAndOrExpression(sourceCode, leftSide, previousPrecedence);
                }
            }
            return null;
        }

        private UnaryExpression GetConditionalPreprocessorNotExpression(SourceCode sourceCode)
        {
            this.AdvanceToNextConditionalDirectiveCodeSymbol();
            Symbol symbol = this.symbols.Peek(1);
            OperatorSymbol item = new OperatorSymbol(symbol.Text, OperatorCategory.Unary, OperatorType.Not, symbol.Location, this.symbols.Generated);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            this.symbols.Advance();
            Expression nextConditionalPreprocessorExpression = this.GetNextConditionalPreprocessorExpression(sourceCode, ExpressionPrecedence.Unary);
            if ((nextConditionalPreprocessorExpression == null) || (nextConditionalPreprocessorExpression.Tokens.First == null))
            {
                throw new SyntaxException(sourceCode, symbol.LineNumber);
            }
            return new UnaryExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), UnaryExpression.Operator.Not, nextConditionalPreprocessorExpression);
        }

        private ParenthesizedExpression GetConditionalPreprocessorParenthesizedExpression(SourceCode sourceCode)
        {
            this.AdvanceToNextConditionalDirectiveCodeSymbol();
            Symbol symbol = this.symbols.Peek(1);
            if ((symbol == null) || (symbol.SymbolType != SymbolType.OpenParenthesis))
            {
                throw new SyntaxException(sourceCode, symbol.LineNumber);
            }
            this.symbols.Advance();
            Bracket item = new Bracket(symbol.Text, CsTokenType.OpenParenthesis, symbol.Location, this.symbols.Generated);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Expression nextConditionalPreprocessorExpression = this.GetNextConditionalPreprocessorExpression(sourceCode, ExpressionPrecedence.None);
            if (nextConditionalPreprocessorExpression == null)
            {
                throw new SyntaxException(sourceCode, symbol.LineNumber);
            }
            this.AdvanceToNextConditionalDirectiveCodeSymbol();
            Symbol symbol2 = this.symbols.Peek(1);
            if ((symbol2 == null) || (symbol2.SymbolType != SymbolType.CloseParenthesis))
            {
                throw new SyntaxException(sourceCode, symbol.LineNumber);
            }
            this.symbols.Advance();
            Bracket bracket2 = new Bracket(symbol2.Text, CsTokenType.CloseParenthesis, symbol2.Location, this.symbols.Generated);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracket2);
            item.MatchingBracketNode = node2;
            bracket2.MatchingBracketNode = firstItemNode;
            return new ParenthesizedExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextConditionalPreprocessorExpression);
        }

        private DefaultValueExpression GetDefaultValueExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.DefaultValue, SymbolType.Default));
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            LiteralExpression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, true);
            if (typeTokenExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node2;
            return new DefaultValueExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), typeTokenExpression);
        }

        private Dictionary<CsTokenType, CsToken> GetElementModifiers(ref AccessModifierType accessModifier, string[] allowedModifiers)
        {
            Symbol symbol = null;
            Dictionary<CsTokenType, CsToken> modifiers = new Dictionary<CsTokenType, CsToken>();
            Symbol nextSymbol = this.GetNextSymbol();
            while (true)
            {
                if (nextSymbol.SymbolType == SymbolType.Public)
                {
                    if (symbol != null)
                    {
                        throw this.CreateSyntaxException();
                    }
                    accessModifier = AccessModifierType.Public;
                    symbol = nextSymbol;
                    CsToken item = this.GetToken(CsTokenType.Public, SymbolType.Public);
                    this.tokens.Add(item);
                    modifiers.Add(CsTokenType.Public, item);
                }
                else if (nextSymbol.SymbolType == SymbolType.Private)
                {
                    if (symbol != null)
                    {
                        throw this.CreateSyntaxException();
                    }
                    accessModifier = AccessModifierType.Private;
                    symbol = nextSymbol;
                    CsToken token = this.GetToken(CsTokenType.Private, SymbolType.Private);
                    this.tokens.Add(token);
                    modifiers.Add(CsTokenType.Private, token);
                }
                else if (nextSymbol.SymbolType == SymbolType.Internal)
                {
                    if (symbol == null)
                    {
                        accessModifier = AccessModifierType.Internal;
                    }
                    else
                    {
                        if (symbol.SymbolType != SymbolType.Protected)
                        {
                            throw this.CreateSyntaxException();
                        }
                        accessModifier = AccessModifierType.ProtectedInternal;
                    }
                    symbol = nextSymbol;
                    CsToken token3 = this.GetToken(CsTokenType.Internal, SymbolType.Internal);
                    this.tokens.Add(token3);
                    modifiers.Add(CsTokenType.Internal, token3);
                }
                else if (nextSymbol.SymbolType == SymbolType.Protected)
                {
                    if (symbol == null)
                    {
                        accessModifier = AccessModifierType.Protected;
                    }
                    else
                    {
                        if (symbol.SymbolType != SymbolType.Internal)
                        {
                            throw this.CreateSyntaxException();
                        }
                        accessModifier = AccessModifierType.ProtectedInternal;
                    }
                    symbol = nextSymbol;
                    CsToken token4 = this.GetToken(CsTokenType.Protected, SymbolType.Protected);
                    this.tokens.Add(token4);
                    modifiers.Add(CsTokenType.Protected, token4);
                }
                else if (!this.GetOtherElementModifier(allowedModifiers, modifiers, nextSymbol))
                {
                    return modifiers;
                }
                nextSymbol = this.GetNextSymbol();
            }
        }

        private CsToken GetElementNameToken(bool unsafeCode)
        {
            return this.GetElementNameToken(unsafeCode, false);
        }

        private CsToken GetElementNameToken(bool unsafeCode, bool allowArrayBrackets)
        {
            if (this.GetNextSymbol().SymbolType == SymbolType.This)
            {
                return this.GetToken(CsTokenType.Other, SymbolType.This);
            }
            TypeToken typeToken = this.GetTypeToken(unsafeCode, allowArrayBrackets);
            if (typeToken.ChildTokens.Count == 1)
            {
                return typeToken.ChildTokens.First.Value;
            }
            return typeToken;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification="May be simplified later."), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="May be simplified later.")]
        private ElementType? GetElementType(CsElement parent, bool unsafeCode)
        {
            int count = 1;
            Symbol symbol = this.symbols.Peek(count);
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = true;
            ElementType? nullable = null;
            while ((symbol != null) && flag4)
            {
                switch (symbol.SymbolType)
                {
                    case SymbolType.OpenParenthesis:
                        nullable = new ElementType?(ElementType.Method);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.OpenCurlyBracket:
                        flag = true;
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.OpenSquareBracket:
                    case SymbolType.CloseSquareBracket:
                    case SymbolType.LessThan:
                    case SymbolType.GreaterThan:
                    case SymbolType.Dot:
                    case SymbolType.QualifiedAlias:
                    case SymbolType.QuestionMark:
                    case SymbolType.Comma:
                    case SymbolType.Abstract:
                    case SymbolType.Const:
                    case SymbolType.Explicit:
                    case SymbolType.Fixed:
                    case SymbolType.Implicit:
                    case SymbolType.Internal:
                    case SymbolType.New:
                    case SymbolType.Override:
                    case SymbolType.Private:
                    case SymbolType.Protected:
                    case SymbolType.Public:
                    case SymbolType.Readonly:
                    case SymbolType.Sealed:
                    case SymbolType.Static:
                    case SymbolType.Virtual:
                    case SymbolType.Volatile:
                    case SymbolType.WhiteSpace:
                    case SymbolType.EndOfLine:
                    case SymbolType.SingleLineComment:
                    case SymbolType.MultiLineComment:
                    case SymbolType.PreprocessorDirective:
                        goto Label_0368;

                    case SymbolType.Multiplication:
                    case SymbolType.LogicalAnd:
                        if (unsafeCode)
                        {
                            goto Label_0368;
                        }
                        goto Label_0365;

                    case SymbolType.Tilde:
                        nullable = new ElementType?(ElementType.Destructor);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Class:
                        nullable = new ElementType?(ElementType.Class);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Delegate:
                        nullable = new ElementType?(ElementType.Delegate);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Enum:
                        nullable = new ElementType?(ElementType.Enum);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Event:
                        nullable = new ElementType?(ElementType.Event);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Extern:
                    {
                        int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(count + 1);
                        if ((nextCodeSymbolIndex != -1) && (this.symbols.Peek(nextCodeSymbolIndex).Text == "alias"))
                        {
                            flag2 = true;
                        }
                        goto Label_0368;
                    }
                    case SymbolType.Interface:
                        nullable = new ElementType?(ElementType.Interface);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Namespace:
                        nullable = new ElementType?(ElementType.Namespace);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Operator:
                        nullable = new ElementType?(ElementType.Method);
                        flag3 = false;
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Struct:
                        nullable = new ElementType?(ElementType.Struct);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.This:
                        nullable = new ElementType?(ElementType.Indexer);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Unsafe:
                        unsafeCode = true;
                        goto Label_0368;

                    case SymbolType.Using:
                        nullable = new ElementType?(ElementType.UsingDirective);
                        flag4 = false;
                        goto Label_0368;

                    case SymbolType.Other:
                        if (parent != null)
                        {
                            if (!(symbol.Text == "get") && !(symbol.Text == "set"))
                            {
                                break;
                            }
                            if ((parent is Property) || (parent is Indexer))
                            {
                                nullable = new ElementType?(ElementType.Accessor);
                                flag4 = false;
                            }
                        }
                        goto Label_0368;

                    default:
                        goto Label_0365;
                }
                if (((symbol.Text == "add") || (symbol.Text == "remove")) && (parent is Event))
                {
                    nullable = new ElementType?(ElementType.Accessor);
                    flag4 = false;
                }
                goto Label_0368;
            Label_0365:
                flag4 = false;
            Label_0368:
                symbol = this.symbols.Peek(++count);
            }
            if (!nullable.HasValue)
            {
                if (flag)
                {
                    return new ElementType?(ElementType.Property);
                }
                if (flag2)
                {
                    return new ElementType?(ElementType.ExternAliasDirective);
                }
                if (count == 2)
                {
                    symbol = this.symbols.Peek(1);
                    if ((symbol != null) && (symbol.SymbolType == SymbolType.Semicolon))
                    {
                        nullable = new ElementType?(ElementType.EmptyElement);
                    }
                    return nullable;
                }
                return new ElementType?(ElementType.Field);
            }
            if (((((ElementType) nullable) == ElementType.Method) && (parent != null)) && !flag3)
            {
                count = 1;
                for (symbol = this.symbols.Peek(count); symbol != null; symbol = this.symbols.Peek(++count))
                {
                    if (symbol.SymbolType == SymbolType.Other)
                    {
                        int num3 = this.GetNextCodeSymbolIndex(count + 1);
                        if ((num3 != -1) && (this.symbols.Peek(num3).SymbolType == SymbolType.OpenParenthesis))
                        {
                            if (!parent.Declaration.Name.StartsWith(symbol.Text, StringComparison.Ordinal))
                            {
                                if (symbol.Text.StartsWith("~", StringComparison.Ordinal))
                                {
                                    string str = symbol.Text.Substring(1, symbol.Text.Length - 1);
                                    if (parent.Declaration.Name.StartsWith(str, StringComparison.Ordinal) && (((parent.Declaration.Name.Length == str.Length) || (parent.Declaration.Name[str.Length] == ' ')) || (parent.Declaration.Name[str.Length] == '<')))
                                    {
                                        return new ElementType?(ElementType.Destructor);
                                    }
                                }
                            }
                            else if (((parent.Declaration.Name.Length == symbol.Text.Length) || (parent.Declaration.Name[symbol.Text.Length] == ' ')) || (parent.Declaration.Name[symbol.Text.Length] == '<'))
                            {
                                return new ElementType?(ElementType.Constructor);
                            }
                        }
                    }
                    else if (((symbol.SymbolType == SymbolType.OpenParenthesis) || (symbol.SymbolType == SymbolType.Dot)) || (symbol.SymbolType == SymbolType.Operator))
                    {
                        return nullable;
                    }
                }
            }
            return nullable;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="May be simplified later."), SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId="Microsoft.StyleCop.CSharp.SymbolManager.Combine(System.Int32,System.Int32,System.String,Microsoft.StyleCop.CSharp.SymbolType)", Justification="The literal represents a non-localizable C# operator symbol")]
        private Expression GetExpressionExtension(Expression leftSide, ExpressionPrecedence previousPrecedence, bool unsafeCode, bool typeExpression, bool allowVariableDeclaration)
        {
            Expression expression = null;
            Symbol nextSymbol = this.GetNextSymbol();
            if (typeExpression)
            {
                if ((nextSymbol.SymbolType != SymbolType.Dot) && (nextSymbol.SymbolType != SymbolType.QualifiedAlias))
                {
                    return expression;
                }
                return this.GetMemberAccessExpression(leftSide, previousPrecedence, unsafeCode);
            }
            switch (nextSymbol.SymbolType)
            {
                case SymbolType.OpenParenthesis:
                    return this.GetMethodInvocationExpression(leftSide, previousPrecedence, unsafeCode);

                case SymbolType.CloseParenthesis:
                case SymbolType.CloseSquareBracket:
                    return expression;

                case SymbolType.OpenSquareBracket:
                    return this.GetArrayAccessExpression(leftSide, previousPrecedence, unsafeCode);

                case SymbolType.Increment:
                    return this.GetPrimaryIncrementExpression(leftSide, previousPrecedence);

                case SymbolType.Decrement:
                    return this.GetPrimaryDecrementExpression(leftSide, previousPrecedence);

                case SymbolType.Dot:
                case SymbolType.Pointer:
                case SymbolType.QualifiedAlias:
                    return this.GetMemberAccessExpression(leftSide, previousPrecedence, unsafeCode);

                case SymbolType.Comma:
                case SymbolType.Semicolon:
                    return expression;

                case SymbolType.As:
                    return this.GetAsExpression(leftSide, previousPrecedence, unsafeCode);

                case SymbolType.Is:
                    return this.GetIsExpression(leftSide, previousPrecedence, unsafeCode);

                case SymbolType.Other:
                    if (!allowVariableDeclaration || ((leftSide.ExpressionType != ExpressionType.Literal) && (leftSide.ExpressionType != ExpressionType.MemberAccess)))
                    {
                        return expression;
                    }
                    return this.GetVariableDeclarationExpression(leftSide, previousPrecedence, unsafeCode);

                default:
                    OperatorType type;
                    OperatorCategory category;
                    if (GetOperatorType(nextSymbol, out type, out category))
                    {
                        switch (category)
                        {
                            case OperatorCategory.Relational:
                            {
                                if (type != OperatorType.GreaterThan)
                                {
                                    goto Label_024C;
                                }
                                Symbol symbol2 = this.symbols.Peek(2);
                                if (symbol2 == null)
                                {
                                    goto Label_024C;
                                }
                                if (symbol2.SymbolType != SymbolType.GreaterThanOrEquals)
                                {
                                    if (symbol2.SymbolType == SymbolType.GreaterThan)
                                    {
                                        this.symbols.Combine(1, 2, ">>", SymbolType.RightShift);
                                        break;
                                    }
                                    goto Label_024C;
                                }
                                this.symbols.Combine(1, 2, ">>=", SymbolType.RightShiftEquals);
                                goto Label_01E5;
                            }
                            case OperatorCategory.Logical:
                                switch (type)
                                {
                                    case OperatorType.LogicalAnd:
                                    case OperatorType.LogicalOr:
                                    case OperatorType.LogicalXor:
                                        return this.GetLogicalExpression(leftSide, previousPrecedence, unsafeCode);

                                    case OperatorType.ConditionalAnd:
                                    case OperatorType.ConditionalOr:
                                        return this.GetConditionalLogicalExpression(leftSide, previousPrecedence, unsafeCode);

                                    case OperatorType.NullCoalescingSymbol:
                                        return this.GetNullCoalescingExpression(leftSide, previousPrecedence, unsafeCode);
                                }
                                return expression;

                            case OperatorCategory.Assignment:
                                goto Label_01E5;

                            case OperatorCategory.Arithmetic:
                                if (!unsafeCode || (type != OperatorType.Multiplication))
                                {
                                    return this.GetArithmeticExpression(leftSide, previousPrecedence, unsafeCode);
                                }
                                if (!this.IsDereferenceExpression(leftSide))
                                {
                                    return this.GetArithmeticExpression(leftSide, previousPrecedence, unsafeCode);
                                }
                                return this.GetUnsafeTypeExpression(leftSide, previousPrecedence);

                            case OperatorCategory.Conditional:
                                if (type == OperatorType.ConditionalQuestionMark)
                                {
                                    expression = this.GetConditionalExpression(leftSide, previousPrecedence, unsafeCode);
                                }
                                return expression;
                        }
                    }
                    return expression;
            }
        Label_01E5:
            return this.GetAssignmentExpression(leftSide, previousPrecedence, unsafeCode);
        Label_024C:
            return this.GetRelationalExpression(leftSide, previousPrecedence, unsafeCode);
        }

        private FileHeader GetFileHeader()
        {
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                Symbol symbol = this.symbols.Peek(1);
                if ((symbol == null) || ((symbol.SymbolType != SymbolType.WhiteSpace) && (symbol.SymbolType != SymbolType.EndOfLine)))
                {
                    break;
                }
                this.document.MasterTokenList.Add(this.GetToken(TokenTypeFromSymbolType(symbol.SymbolType), symbol.SymbolType));
            }
            bool flag = false;
            int num = 0;
            while (!flag)
            {
                Symbol symbol2 = this.symbols.Peek(1);
                if (symbol2 == null)
                {
                    break;
                }
                if (symbol2.SymbolType == SymbolType.SingleLineComment)
                {
                    num = 0;
                    if (!symbol2.Text.StartsWith("//-", StringComparison.Ordinal))
                    {
                        builder.Append(symbol2.Text.Substring(2, symbol2.Text.Length - 2));
                    }
                    this.document.MasterTokenList.Add(this.GetToken(CsTokenType.SingleLineComment, SymbolType.SingleLineComment));
                }
                else if (symbol2.SymbolType == SymbolType.WhiteSpace)
                {
                    this.document.MasterTokenList.Add(this.GetToken(CsTokenType.WhiteSpace, SymbolType.WhiteSpace));
                }
                else
                {
                    if ((symbol2.SymbolType != SymbolType.EndOfLine) || (++num > 1))
                    {
                        break;
                    }
                    this.document.MasterTokenList.Add(this.GetToken(CsTokenType.EndOfLine, SymbolType.EndOfLine));
                }
            }
            return new FileHeader(builder.ToString());
        }

        internal static string GetFullName(CsDocument document, CsTokenList tokens, Microsoft.StyleCop.Node<CsToken> startTokenNode, out Microsoft.StyleCop.Node<CsToken> endTokenNode)
        {
            endTokenNode = FindEndOfName(document, tokens, startTokenNode);
            StringBuilder builder = new StringBuilder();
            for (Microsoft.StyleCop.Node<CsToken> node = startTokenNode; !tokens.OutOfBounds(node); node = node.Next)
            {
                CsTokenType csTokenType = node.Value.CsTokenType;
                if ((((csTokenType != CsTokenType.WhiteSpace) && (csTokenType != CsTokenType.EndOfLine)) && ((csTokenType != CsTokenType.SingleLineComment) && (csTokenType != CsTokenType.MultiLineComment))) && (csTokenType != CsTokenType.PreprocessorDirective))
                {
                    builder.Append(node.Value.Text);
                }
                if (node == endTokenNode)
                {
                    break;
                }
            }
            return builder.ToString();
        }

        private MasterList<CsToken> GetGenericArgumentList(bool unsafeCode, CsToken name, int startIndex, out int endIndex)
        {
            endIndex = -1;
            MasterList<CsToken> list = null;
            int count = startIndex;
            while (true)
            {
                Symbol symbol = this.symbols.Peek(count);
                if ((symbol == null) || (((symbol.SymbolType != SymbolType.WhiteSpace) && (symbol.SymbolType != SymbolType.EndOfLine)) && (((symbol.SymbolType != SymbolType.SingleLineComment) && (symbol.SymbolType != SymbolType.MultiLineComment)) && (symbol.SymbolType != SymbolType.PreprocessorDirective))))
                {
                    break;
                }
                count++;
            }
            Symbol symbol2 = this.symbols.Peek(count);
            if ((symbol2 == null) || (symbol2.SymbolType != SymbolType.LessThan))
            {
                return list;
            }
            list = new MasterList<CsToken>();
            if (name != null)
            {
                list.Add(name);
            }
            Microsoft.StyleCop.Node<CsToken> node = null;
            for (int i = startIndex; i <= count; i++)
            {
                symbol2 = this.symbols.Peek(i);
                if (symbol2.SymbolType == SymbolType.LessThan)
                {
                    if (node != null)
                    {
                        return null;
                    }
                    Bracket item = new Bracket(symbol2.Text, CsTokenType.OpenGenericBracket, symbol2.Location, this.symbols.Generated);
                    node = list.InsertLast(item);
                }
                else
                {
                    list.Add(this.ConvertSymbol(symbol2, TokenTypeFromSymbolType(symbol2.SymbolType)));
                }
            }
        Label_00F4:
            symbol2 = this.symbols.Peek(++count);
            if (symbol2 == null)
            {
                throw new SyntaxException(this.document.SourceCode, name.LineNumber);
            }
            if (symbol2.SymbolType == SymbolType.GreaterThan)
            {
                if (node == null)
                {
                    return null;
                }
                Bracket bracket2 = new Bracket(symbol2.Text, CsTokenType.CloseGenericBracket, symbol2.Location, this.symbols.Generated);
                Microsoft.StyleCop.Node<CsToken> node2 = list.InsertLast(bracket2);
                ((Bracket) node.Value).MatchingBracketNode = node2;
                bracket2.MatchingBracketNode = node;
                endIndex = count;
                return list;
            }
            if (symbol2.SymbolType == SymbolType.Other)
            {
                int num3 = 0;
                CsToken token = this.GetTypeTokenAux(unsafeCode, true, false, count, out num3);
                if (token == null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol2.LineNumber);
                }
                count = num3;
                list.Add(token);
                goto Label_00F4;
            }
            if (((symbol2.SymbolType == SymbolType.WhiteSpace) || (symbol2.SymbolType == SymbolType.EndOfLine)) || (((symbol2.SymbolType == SymbolType.SingleLineComment) || (symbol2.SymbolType == SymbolType.MultiLineComment)) || (symbol2.SymbolType == SymbolType.PreprocessorDirective)))
            {
                list.Add(this.ConvertSymbol(symbol2, TokenTypeFromSymbolType(symbol2.SymbolType)));
                goto Label_00F4;
            }
            if (symbol2.SymbolType == SymbolType.Comma)
            {
                list.Add(this.ConvertSymbol(symbol2, CsTokenType.Comma));
                goto Label_00F4;
            }
            return null;
        }

        private GenericType GetGenericToken(bool unsafeCode)
        {
            int lastIndex = -1;
            GenericType type = this.GetGenericTokenAux(unsafeCode, 1, out lastIndex);
            if (type != null)
            {
                this.symbols.CurrentIndex += lastIndex;
            }
            return type;
        }

        private GenericType GetGenericTokenAux(bool unsafeCode, int startIndex, out int lastIndex)
        {
            lastIndex = -1;
            Symbol symbol = this.symbols.Peek(startIndex);
            GenericType type = null;
            CsToken name = new CsToken(symbol.Text, CsTokenType.Other, symbol.Location, this.symbols.Generated);
            MasterList<CsToken> childTokens = this.GetGenericArgumentList(unsafeCode, name, startIndex + 1, out lastIndex);
            if (childTokens != null)
            {
                type = new GenericType(childTokens, CodeLocation.Join<CsToken>(symbol.Location, childTokens.Last), this.symbols.Generated);
            }
            return type;
        }

        private IsExpression GetIsExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            IsExpression expression = null;
            if (!CheckPrecedence(previousPrecedence, ExpressionPrecedence.Relational))
            {
                return expression;
            }
            this.tokens.Add(this.GetToken(CsTokenType.Is, SymbolType.Is));
            this.GetNextSymbol(SymbolType.Other);
            LiteralExpression type = this.GetTypeTokenExpression(unsafeCode, true, true);
            if ((type == null) || (type.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            return new IsExpression(new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last), leftHandSide, type);
        }

        private LambdaExpression GetLambdaExpression(bool unsafeCode)
        {
            LambdaExpression expression = new LambdaExpression();
            Symbol nextSymbol = this.GetNextSymbol();
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            ICollection<Parameter> items = null;
            if (nextSymbol.SymbolType == SymbolType.OpenParenthesis)
            {
                items = this.ParseAnonymousMethodParameterList(unsafeCode);
            }
            else
            {
                if (nextSymbol.SymbolType != SymbolType.Other)
                {
                    throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                }
                CsToken item = this.GetToken(CsTokenType.Other, SymbolType.Other);
                this.tokens.Add(item);
                expression.AddParameter(new Parameter(null, item.Text, ParameterModifiers.None, item.Location, new CsTokenList(this.tokens, this.tokens.Last, this.tokens.Last), item.Generated));
            }
            this.tokens.Add(this.GetOperatorToken(OperatorType.Lambda));
            if (this.GetNextSymbol().SymbolType == SymbolType.OpenCurlyBracket)
            {
                expression.AnonymousFunctionBody = this.GetNextStatement(unsafeCode);
            }
            else
            {
                expression.AnonymousFunctionBody = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            }
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            expression.Tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            if ((items != null) && (items.Count > 0))
            {
                expression.AddParameters(items);
            }
            if ((expression.Parameters != null) && (expression.Parameters.Count > 0))
            {
                foreach (Parameter parameter in expression.Parameters)
                {
                    expression.Variables.Add(new Variable(parameter.Type, parameter.Name, VariableModifiers.None, parameter.Location.StartPoint, parameter.Generated));
                }
            }
            return expression;
        }

        private LiteralExpression GetLiteralExpression(bool unsafeCode)
        {
            int num;
            Symbol nextSymbol = this.GetNextSymbol();
            CsToken item = null;
            bool generic = false;
            if (((nextSymbol.SymbolType == SymbolType.Other) && this.HasTypeSignature(1, false, out num, out generic)) && generic)
            {
                item = this.GetGenericToken(unsafeCode);
            }
            if (item == null)
            {
                item = this.GetToken(CsTokenType.Other, SymbolType.Other);
            }
            return new LiteralExpression(this.tokens, this.tokens.InsertLast(item));
        }

        private LogicalExpression GetLogicalExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            LogicalExpression expression = null;
            LogicalExpression.Operator and;
            OperatorSymbol item = this.PeekOperatorToken();
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (!CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                return expression;
            }
            this.symbols.Advance();
            this.tokens.Add(item);
            Expression operatorRightHandExpression = this.GetOperatorRightHandExpression(operatorPrecedence, unsafeCode);
            CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
            switch (item.SymbolType)
            {
                case OperatorType.LogicalAnd:
                    and = LogicalExpression.Operator.And;
                    break;

                case OperatorType.LogicalOr:
                    and = LogicalExpression.Operator.Or;
                    break;

                case OperatorType.LogicalXor:
                    and = LogicalExpression.Operator.Xor;
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return new LogicalExpression(tokens, and, leftHandSide, operatorRightHandExpression);
        }

        private MemberAccessExpression GetMemberAccessExpression(Expression leftSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            MemberAccessExpression expression = null;
            OperatorType memberAccess;
            MemberAccessExpression.Operator dot;
            ExpressionPrecedence primary;
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol.SymbolType == SymbolType.Dot)
            {
                memberAccess = OperatorType.MemberAccess;
                dot = MemberAccessExpression.Operator.Dot;
                primary = ExpressionPrecedence.Primary;
            }
            else if (nextSymbol.SymbolType == SymbolType.Pointer)
            {
                memberAccess = OperatorType.Pointer;
                dot = MemberAccessExpression.Operator.Pointer;
                primary = ExpressionPrecedence.Primary;
            }
            else
            {
                if (nextSymbol.SymbolType != SymbolType.QualifiedAlias)
                {
                    throw new InvalidOperationException();
                }
                memberAccess = OperatorType.QualifiedAlias;
                dot = MemberAccessExpression.Operator.QualifiedAlias;
                primary = ExpressionPrecedence.Global;
            }
            if (!CheckPrecedence(previousPrecedence, primary))
            {
                return expression;
            }
            this.tokens.Add(this.GetOperatorToken(memberAccess));
            LiteralExpression literalExpression = this.GetLiteralExpression(unsafeCode);
            if (literalExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            return new MemberAccessExpression(new CsTokenList(this.tokens, leftSide.Tokens.First, this.tokens.Last), dot, leftSide, literalExpression);
        }

        private MethodInvocationExpression GetMethodInvocationExpression(Expression methodName, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            MethodInvocationExpression expression = null;
            if (CheckPrecedence(previousPrecedence, ExpressionPrecedence.Primary))
            {
                Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
                Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
                IList<Expression> argumentList = this.GetArgumentList(SymbolType.CloseParenthesis, unsafeCode);
                Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
                Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(item);
                bracketToken.MatchingBracketNode = node2;
                item.MatchingBracketNode = node;
                Microsoft.StyleCop.Node<CsToken> first = methodName.Tokens.First;
                CsTokenList tokens = new CsTokenList(this.tokens, first, this.tokens.Last);
                expression = new MethodInvocationExpression(tokens, methodName, argumentList);
            }
            return expression;
        }

        private Expression GetNewAllocationExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstTokenNode = this.tokens.InsertLast(this.GetToken(CsTokenType.New, SymbolType.New));
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol.SymbolType == SymbolType.OpenCurlyBracket)
            {
                return this.GetNewAnonymousTypeExpression(unsafeCode, firstTokenNode);
            }
            if (nextSymbol.SymbolType == SymbolType.OpenSquareBracket)
            {
                return this.GetNewArrayTypeExpression(unsafeCode, firstTokenNode, null);
            }
            if (nextSymbol.SymbolType != SymbolType.Other)
            {
                throw this.CreateSyntaxException();
            }
            Expression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, false);
            if ((typeTokenExpression == null) || (typeTokenExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            if (this.GetNextSymbol().SymbolType == SymbolType.OpenSquareBracket)
            {
                return this.GetNewArrayTypeExpression(unsafeCode, firstTokenNode, typeTokenExpression);
            }
            return this.GetNewNonArrayTypeExpression(unsafeCode, firstTokenNode, typeTokenExpression);
        }

        private NewExpression GetNewAnonymousTypeExpression(bool unsafeCode, Microsoft.StyleCop.Node<CsToken> firstTokenNode)
        {
            CollectionInitializerExpression anonymousTypeInitializerExpression = this.GetAnonymousTypeInitializerExpression(unsafeCode);
            return new NewExpression(new CsTokenList(this.tokens, firstTokenNode, anonymousTypeInitializerExpression.Tokens.Last), null, anonymousTypeInitializerExpression);
        }

        private NewArrayExpression GetNewArrayTypeExpression(bool unsafeCode, Microsoft.StyleCop.Node<CsToken> firstTokenNode, Expression type)
        {
            Symbol nextSymbol = this.GetNextSymbol(SymbolType.OpenSquareBracket);
            if (type != null)
            {
                while (nextSymbol.SymbolType == SymbolType.OpenSquareBracket)
                {
                    type = this.GetArrayAccessExpression(type, ExpressionPrecedence.None, unsafeCode);
                    if ((type == null) || (type.Tokens.First == null))
                    {
                        throw this.CreateSyntaxException();
                    }
                    nextSymbol = this.GetNextSymbol();
                }
            }
            else
            {
                this.MovePastArrayBrackets();
            }
            if ((type != null) && (type.ExpressionType != ExpressionType.ArrayAccess))
            {
                throw this.CreateSyntaxException();
            }
            nextSymbol = this.GetNextSymbol();
            ArrayInitializerExpression initializer = null;
            if (nextSymbol.SymbolType == SymbolType.OpenCurlyBracket)
            {
                initializer = this.GetArrayInitializerExpression(unsafeCode);
            }
            if ((type == null) && (initializer == null))
            {
                throw this.CreateSyntaxException();
            }
            return new NewArrayExpression(new CsTokenList(this.tokens, firstTokenNode, this.tokens.Last), type as ArrayAccessExpression, initializer);
        }

        private NewExpression GetNewNonArrayTypeExpression(bool unsafeCode, Microsoft.StyleCop.Node<CsToken> firstTokenNode, Expression type)
        {
            Expression typeCreationExpression = type;
            if (this.GetNextSymbol().SymbolType == SymbolType.OpenParenthesis)
            {
                typeCreationExpression = this.GetMethodInvocationExpression(type, ExpressionPrecedence.None, unsafeCode);
                if ((typeCreationExpression == null) || (typeCreationExpression.Tokens.First == null))
                {
                    throw this.CreateSyntaxException();
                }
            }
            Symbol nextSymbol = this.GetNextSymbol();
            Expression initializerExpression = null;
            if (nextSymbol.SymbolType == SymbolType.OpenCurlyBracket)
            {
                initializerExpression = this.GetObjectOrCollectionInitializerExpression(unsafeCode);
                if ((initializerExpression == null) || (initializerExpression.Tokens.First == null))
                {
                    throw this.CreateSyntaxException();
                }
            }
            return new NewExpression(new CsTokenList(this.tokens, firstTokenNode, this.tokens.Last), typeCreationExpression, initializerExpression);
        }

        private int GetNextCodeSymbolIndex(int startIndex)
        {
            int num = -1;
            while (true)
            {
                Symbol symbol = this.symbols.Peek(startIndex);
                if (symbol == null)
                {
                    return num;
                }
                if ((((symbol.SymbolType != SymbolType.WhiteSpace) && (symbol.SymbolType != SymbolType.EndOfLine)) && ((symbol.SymbolType != SymbolType.SingleLineComment) && (symbol.SymbolType != SymbolType.MultiLineComment))) && (symbol.SymbolType != SymbolType.PreprocessorDirective))
                {
                    return startIndex;
                }
                startIndex++;
            }
        }

        internal Expression GetNextConditionalPreprocessorExpression(SourceCode sourceCode)
        {
            return this.GetNextConditionalPreprocessorExpression(sourceCode, ExpressionPrecedence.None);
        }

        private Expression GetNextConditionalPreprocessorExpression(SourceCode sourceCode, ExpressionPrecedence previousPrecedence)
        {
            this.AdvanceToNextConditionalDirectiveCodeSymbol();
            Expression leftSide = null;
            Symbol symbol = this.symbols.Peek(1);
            if (symbol != null)
            {
                CsToken token;
                Microsoft.StyleCop.Node<CsToken> node;
                SymbolType symbolType = symbol.SymbolType;
                if (symbolType <= SymbolType.Not)
                {
                    switch (symbolType)
                    {
                        case SymbolType.OpenParenthesis:
                            leftSide = this.GetConditionalPreprocessorParenthesizedExpression(sourceCode);
                            goto Label_012D;

                        case SymbolType.Not:
                            leftSide = this.GetConditionalPreprocessorNotExpression(sourceCode);
                            goto Label_012D;
                    }
                    goto Label_010E;
                }
                switch (symbolType)
                {
                    case SymbolType.False:
                        this.symbols.Advance();
                        token = new CsToken(symbol.Text, CsTokenType.False, symbol.Location, this.symbols.Generated);
                        node = this.tokens.InsertLast(token);
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_012D;

                    case SymbolType.True:
                        this.symbols.Advance();
                        token = new CsToken(symbol.Text, CsTokenType.True, symbol.Location, this.symbols.Generated);
                        node = this.tokens.InsertLast(token);
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_012D;
                }
                if (symbolType != SymbolType.Other)
                {
                    goto Label_010E;
                }
                leftSide = this.GetConditionalPreprocessorConstantExpression();
            }
            goto Label_012D;
        Label_010E:
            throw new SyntaxException(sourceCode, symbol.LineNumber);
        Label_012D:
            while (leftSide != null)
            {
                Expression expression2 = this.GetConditionalPreprocessorExpressionExtension(sourceCode, leftSide, previousPrecedence);
                if (expression2 == null)
                {
                    return leftSide;
                }
                leftSide = expression2;
            }
            return leftSide;
        }

        private Expression GetNextExpression(ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            return this.GetNextExpression(previousPrecedence, unsafeCode, false, false);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="May be simplified later.")]
        private Expression GetNextExpression(ExpressionPrecedence previousPrecedence, bool unsafeCode, bool allowVariableDeclaration, bool typeExpression)
        {
            Expression leftSide = null;
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol != null)
            {
                Microsoft.StyleCop.Node<CsToken> node;
                switch (nextSymbol.SymbolType)
                {
                    case SymbolType.Not:
                    case SymbolType.Tilde:
                        leftSide = this.GetUnaryExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.Base:
                        node = this.tokens.InsertLast(this.GetToken(CsTokenType.Base, SymbolType.Base));
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_03B9;

                    case SymbolType.Plus:
                    case SymbolType.Minus:
                        if (this.IsUnaryExpression())
                        {
                            leftSide = this.GetUnaryExpression(unsafeCode);
                        }
                        goto Label_03B9;

                    case SymbolType.PlusEquals:
                    case SymbolType.MinusEquals:
                    case SymbolType.Static:
                    case SymbolType.Struct:
                    case SymbolType.Switch:
                    case SymbolType.Throw:
                    case SymbolType.Try:
                    case SymbolType.WhiteSpace:
                    case SymbolType.EndOfLine:
                        goto Label_0390;

                    case SymbolType.Multiplication:
                        if (!unsafeCode)
                        {
                            goto Label_0390;
                        }
                        leftSide = this.GetUnsafeAccessExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.OpenParenthesis:
                        if (this.IsLambdaExpression())
                        {
                            leftSide = this.GetLambdaExpression(unsafeCode);
                        }
                        else
                        {
                            leftSide = this.GetOpenParenthesisExpression(unsafeCode);
                        }
                        goto Label_03B9;

                    case SymbolType.Increment:
                        if (this.IsUnaryExpression())
                        {
                            leftSide = this.GetUnaryIncrementExpression(unsafeCode);
                        }
                        goto Label_03B9;

                    case SymbolType.Decrement:
                        if (this.IsUnaryExpression())
                        {
                            leftSide = this.GetUnaryDecrementExpression(unsafeCode);
                        }
                        goto Label_03B9;

                    case SymbolType.LogicalAnd:
                        if (!unsafeCode)
                        {
                            goto Label_0390;
                        }
                        leftSide = this.GetUnsafeAccessExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.Default:
                        leftSide = this.GetDefaultValueExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.Delegate:
                        leftSide = this.GetAnonymousMethodExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.False:
                        node = this.tokens.InsertLast(this.GetToken(CsTokenType.False, SymbolType.False));
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_03B9;

                    case SymbolType.Checked:
                        leftSide = this.GetCheckedExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.New:
                        leftSide = this.GetNewAllocationExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.Null:
                        node = this.tokens.InsertLast(this.GetToken(CsTokenType.Null, SymbolType.Null));
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_03B9;

                    case SymbolType.Sizeof:
                        leftSide = this.GetSizeofExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.Stackalloc:
                        leftSide = this.GetStackallocExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.This:
                        node = this.tokens.InsertLast(this.GetToken(CsTokenType.This, SymbolType.This));
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_03B9;

                    case SymbolType.True:
                        node = this.tokens.InsertLast(this.GetToken(CsTokenType.True, SymbolType.True));
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_03B9;

                    case SymbolType.Typeof:
                        leftSide = this.GetTypeofExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.Unchecked:
                        leftSide = this.GetUncheckedExpression(unsafeCode);
                        goto Label_03B9;

                    case SymbolType.Other:
                        if (!this.IsLambdaExpression())
                        {
                            if (this.IsQueryExpression(unsafeCode))
                            {
                                leftSide = this.GetQueryExpression(unsafeCode);
                            }
                            break;
                        }
                        leftSide = this.GetLambdaExpression(unsafeCode);
                        break;

                    case SymbolType.String:
                        node = this.tokens.InsertLast(this.GetToken(CsTokenType.String, SymbolType.String));
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_03B9;

                    case SymbolType.Number:
                        node = this.tokens.InsertLast(this.GetToken(CsTokenType.Number, SymbolType.Number));
                        leftSide = new LiteralExpression(new CsTokenList(this.tokens, node, node), node);
                        goto Label_03B9;

                    default:
                        goto Label_0390;
                }
                if (leftSide == null)
                {
                    leftSide = this.GetOtherExpression(allowVariableDeclaration, unsafeCode);
                }
            }
            goto Label_03B9;
        Label_0390:
            throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
        Label_03B9:
            while (leftSide != null)
            {
                Expression expression2 = this.GetExpressionExtension(leftSide, previousPrecedence, unsafeCode, typeExpression, allowVariableDeclaration);
                if (expression2 == null)
                {
                    return leftSide;
                }
                leftSide = expression2;
            }
            return leftSide;
        }

        private Statement GetNextStatement(bool unsafeCode)
        {
            return this.GetNextStatement(unsafeCode, null);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="May be simplified later.")]
        private Statement GetNextStatement(bool unsafeCode, VariableCollection variables)
        {
            Statement statement = null;
            if (!this.MoveToStatement())
            {
                return statement;
            }
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol == null)
            {
                return statement;
            }
            switch (nextSymbol.SymbolType)
            {
                case SymbolType.OpenParenthesis:
                case SymbolType.Increment:
                case SymbolType.Decrement:
                case SymbolType.Base:
                case SymbolType.New:
                case SymbolType.This:
                    return this.ParseExpressionStatement(unsafeCode);

                case SymbolType.OpenCurlyBracket:
                    return this.ParseBlockStatement(unsafeCode);

                case SymbolType.Multiplication:
                    if (unsafeCode)
                    {
                        return this.ParseExpressionStatement(unsafeCode);
                    }
                    break;

                case SymbolType.LogicalAnd:
                    if (!unsafeCode)
                    {
                        break;
                    }
                    return this.ParseExpressionStatement(unsafeCode);

                case SymbolType.Break:
                    return this.ParseBreakStatement();

                case SymbolType.Checked:
                    return this.ParseCheckedStatement(unsafeCode);

                case SymbolType.Const:
                    return this.ParseVariableDeclarationStatement(unsafeCode, variables);

                case SymbolType.Continue:
                    return this.ParseContinueStatement();

                case SymbolType.Do:
                    return this.ParseDoWhileStatement(unsafeCode);

                case SymbolType.Semicolon:
                {
                    Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
                    return new EmptyStatement(new CsTokenList(this.tokens, firstItemNode, firstItemNode));
                }
                case SymbolType.Fixed:
                    return this.ParseFixedStatement(unsafeCode);

                case SymbolType.For:
                    return this.ParseForStatement(unsafeCode);

                case SymbolType.Foreach:
                    return this.ParseForeachStatement(unsafeCode);

                case SymbolType.Goto:
                    return this.ParseGotoStatement(unsafeCode);

                case SymbolType.If:
                    return this.ParseIfStatement(unsafeCode);

                case SymbolType.Lock:
                    return this.ParseLockStatement(unsafeCode);

                case SymbolType.Return:
                    return this.ParseReturnStatement(unsafeCode);

                case SymbolType.Sizeof:
                case SymbolType.Typeof:
                    return this.ParseExpressionStatement(unsafeCode);

                case SymbolType.Switch:
                    return this.ParseSwitchStatement(unsafeCode);

                case SymbolType.Throw:
                    return this.ParseThrowStatement(unsafeCode);

                case SymbolType.Try:
                    return this.ParseTryStatement(unsafeCode);

                case SymbolType.Unchecked:
                    return this.ParseUncheckedStatement(unsafeCode);

                case SymbolType.Unsafe:
                    return this.ParseUnsafeStatement();

                case SymbolType.Using:
                    return this.ParseUsingStatement(unsafeCode);

                case SymbolType.While:
                    return this.ParseWhileStatement(unsafeCode);

                case SymbolType.Other:
                    if (nextSymbol.Text == "yield")
                    {
                        statement = this.ParseYieldStatement(unsafeCode);
                        if (statement != null)
                        {
                            return statement;
                        }
                    }
                    return this.ParseOtherStatement(unsafeCode, variables);
            }
            throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
        }

        private Symbol GetNextSymbol()
        {
            return this.GetNextSymbol(SkipSymbols.All);
        }

        private Symbol GetNextSymbol(SkipSymbols skip)
        {
            return this.GetNextSymbol(skip, false);
        }

        private Symbol GetNextSymbol(SymbolType symbolType)
        {
            return this.GetNextSymbol(symbolType, SkipSymbols.All);
        }

        private Symbol GetNextSymbol(SkipSymbols skip, bool allowNull)
        {
            this.AdvanceToNextCodeSymbol(skip);
            Symbol symbol = this.symbols.Peek(1);
            if ((symbol == null) && !allowNull)
            {
                throw this.CreateSyntaxException();
            }
            return symbol;
        }

        private Symbol GetNextSymbol(SymbolType symbolType, SkipSymbols skip)
        {
            Symbol nextSymbol = this.GetNextSymbol(skip);
            if (nextSymbol.SymbolType != symbolType)
            {
                throw this.CreateSyntaxException();
            }
            return nextSymbol;
        }

        private NullCoalescingExpression GetNullCoalescingExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            NullCoalescingExpression expression = null;
            OperatorSymbol item = this.PeekOperatorToken();
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                this.symbols.Advance();
                this.tokens.Add(item);
                Expression operatorRightHandExpression = this.GetOperatorRightHandExpression(operatorPrecedence, unsafeCode);
                CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
                expression = new NullCoalescingExpression(tokens, leftHandSide, operatorRightHandExpression);
            }
            return expression;
        }

        private ObjectInitializerExpression GetObjectInitializerExpression(bool unsafeCode)
        {
            List<AssignmentExpression> list = new List<AssignmentExpression>();
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(bracketToken);
        Label_001C:
            if (this.GetNextSymbol().SymbolType != SymbolType.CloseCurlyBracket)
            {
                LiteralExpression literalExpression = this.GetLiteralExpression(unsafeCode);
                if (this.GetNextSymbol().SymbolType != SymbolType.Equals)
                {
                    throw this.CreateSyntaxException();
                }
                this.tokens.Add(this.GetOperatorToken(OperatorType.Equals));
                Expression rightHandSide = null;
                if (this.GetNextSymbol().SymbolType == SymbolType.OpenCurlyBracket)
                {
                    rightHandSide = this.GetObjectOrCollectionInitializerExpression(unsafeCode);
                }
                else
                {
                    rightHandSide = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                }
                CsTokenList tokens = new CsTokenList(this.tokens, literalExpression.Tokens.First, rightHandSide.Tokens.Last);
                list.Add(new AssignmentExpression(tokens, AssignmentExpression.Operator.Equals, literalExpression, rightHandSide));
                if (this.GetNextSymbol().SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                    if (this.GetNextSymbol().SymbolType != SymbolType.CloseCurlyBracket)
                    {
                        goto Label_001C;
                    }
                }
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseCurlyBracket, SymbolType.CloseCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> lastItemNode = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = lastItemNode;
            item.MatchingBracketNode = firstItemNode;
            return new ObjectInitializerExpression(new CsTokenList(this.tokens, firstItemNode, lastItemNode), list.ToArray());
        }

        private Expression GetObjectOrCollectionInitializerExpression(bool unsafeCode)
        {
            Symbol nextSymbol = this.GetNextSymbol(SymbolType.OpenCurlyBracket);
            bool flag = false;
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(2);
            if (nextCodeSymbolIndex == -1)
            {
                throw this.CreateSyntaxException();
            }
            if (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.Other)
            {
                nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex + 1);
                if ((nextCodeSymbolIndex != -1) && (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.Equals))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                return this.GetObjectInitializerExpression(unsafeCode);
            }
            return this.GetCollectionInitializerExpression(unsafeCode);
        }

        private Expression GetOpenParenthesisExpression(bool unsafeCode)
        {
            if (this.IsCastExpression(unsafeCode))
            {
                return this.GetCastExpression(unsafeCode);
            }
            return this.GetParenthesizedExpression(unsafeCode);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method is not complex.")]
        private static ExpressionPrecedence GetOperatorPrecedence(OperatorType type)
        {
            ExpressionPrecedence none = ExpressionPrecedence.None;
            switch (type)
            {
                case OperatorType.ConditionalEquals:
                case OperatorType.NotEquals:
                    return ExpressionPrecedence.Equality;

                case OperatorType.LessThan:
                case OperatorType.GreaterThan:
                case OperatorType.LessThanOrEquals:
                case OperatorType.GreaterThanOrEquals:
                    return ExpressionPrecedence.Relational;

                case OperatorType.LogicalAnd:
                    return ExpressionPrecedence.LogicalAnd;

                case OperatorType.LogicalOr:
                    return ExpressionPrecedence.LogicalOr;

                case OperatorType.LogicalXor:
                    return ExpressionPrecedence.LogicalXor;

                case OperatorType.ConditionalAnd:
                    return ExpressionPrecedence.ConditionalAnd;

                case OperatorType.ConditionalOr:
                    return ExpressionPrecedence.ConditionalOr;

                case OperatorType.NullCoalescingSymbol:
                    return ExpressionPrecedence.NullCoalescing;

                case OperatorType.Equals:
                case OperatorType.PlusEquals:
                case OperatorType.MinusEquals:
                case OperatorType.MultiplicationEquals:
                case OperatorType.DivisionEquals:
                case OperatorType.LeftShiftEquals:
                case OperatorType.RightShiftEquals:
                case OperatorType.AndEquals:
                case OperatorType.OrEquals:
                case OperatorType.XorEquals:
                case OperatorType.ModEquals:
                    return ExpressionPrecedence.Assignment;

                case OperatorType.Plus:
                case OperatorType.Minus:
                    return ExpressionPrecedence.Additive;

                case OperatorType.Multiplication:
                case OperatorType.Division:
                case OperatorType.Mod:
                    return ExpressionPrecedence.Multiplicative;

                case OperatorType.LeftShift:
                case OperatorType.RightShift:
                    return ExpressionPrecedence.Shift;

                case OperatorType.ConditionalColon:
                case OperatorType.ConditionalQuestionMark:
                    return ExpressionPrecedence.Conditional;

                case OperatorType.Increment:
                case OperatorType.Decrement:
                case OperatorType.Not:
                case OperatorType.BitwiseCompliment:
                    return ExpressionPrecedence.Unary;
            }
            return none;
        }

        private Expression GetOperatorRightHandExpression(ExpressionPrecedence precedence, bool unsafeCode)
        {
            Expression nextExpression = this.GetNextExpression(precedence, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            return nextExpression;
        }

        private OperatorSymbol GetOperatorToken(OperatorType operatorType)
        {
            SymbolType symbolType = SymbolTypeFromOperatorType(operatorType);
            OperatorSymbol symbol2 = CreateOperatorToken(this.GetNextSymbol(symbolType), this.symbols.Generated);
            if ((symbol2 == null) || (symbol2.SymbolType != operatorType))
            {
                throw this.CreateSyntaxException();
            }
            this.symbols.Advance();
            return symbol2;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method is not complex.")]
        private static bool GetOperatorType(Symbol symbol, out OperatorType type, out OperatorCategory category)
        {
            bool flag = true;
            switch (symbol.SymbolType)
            {
                case SymbolType.Equals:
                    type = OperatorType.Equals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.ConditionalEquals:
                    type = OperatorType.ConditionalEquals;
                    category = OperatorCategory.Relational;
                    return flag;

                case SymbolType.Plus:
                    type = OperatorType.Plus;
                    category = OperatorCategory.Arithmetic;
                    return flag;

                case SymbolType.PlusEquals:
                    type = OperatorType.PlusEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.Minus:
                    type = OperatorType.Minus;
                    category = OperatorCategory.Arithmetic;
                    return flag;

                case SymbolType.MinusEquals:
                    type = OperatorType.MinusEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.Multiplication:
                    type = OperatorType.Multiplication;
                    category = OperatorCategory.Arithmetic;
                    return flag;

                case SymbolType.MultiplicationEquals:
                    type = OperatorType.MultiplicationEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.Division:
                    type = OperatorType.Division;
                    category = OperatorCategory.Arithmetic;
                    return flag;

                case SymbolType.DivisionEquals:
                    type = OperatorType.DivisionEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.LessThan:
                    type = OperatorType.LessThan;
                    category = OperatorCategory.Relational;
                    return flag;

                case SymbolType.LessThanOrEquals:
                    type = OperatorType.LessThanOrEquals;
                    category = OperatorCategory.Relational;
                    return flag;

                case SymbolType.LeftShift:
                    type = OperatorType.LeftShift;
                    category = OperatorCategory.Shift;
                    return flag;

                case SymbolType.LeftShiftEquals:
                    type = OperatorType.LeftShiftEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.GreaterThan:
                    type = OperatorType.GreaterThan;
                    category = OperatorCategory.Relational;
                    return flag;

                case SymbolType.GreaterThanOrEquals:
                    type = OperatorType.GreaterThanOrEquals;
                    category = OperatorCategory.Relational;
                    return flag;

                case SymbolType.RightShift:
                    type = OperatorType.RightShift;
                    category = OperatorCategory.Shift;
                    return flag;

                case SymbolType.RightShiftEquals:
                    type = OperatorType.RightShiftEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.Increment:
                    type = OperatorType.Increment;
                    category = OperatorCategory.IncrementDecrement;
                    return flag;

                case SymbolType.Decrement:
                    type = OperatorType.Decrement;
                    category = OperatorCategory.IncrementDecrement;
                    return flag;

                case SymbolType.LogicalAnd:
                    type = OperatorType.LogicalAnd;
                    category = OperatorCategory.Logical;
                    return flag;

                case SymbolType.AndEquals:
                    type = OperatorType.AndEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.ConditionalAnd:
                    type = OperatorType.ConditionalAnd;
                    category = OperatorCategory.Logical;
                    return flag;

                case SymbolType.LogicalOr:
                    type = OperatorType.LogicalOr;
                    category = OperatorCategory.Logical;
                    return flag;

                case SymbolType.OrEquals:
                    type = OperatorType.OrEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.ConditionalOr:
                    type = OperatorType.ConditionalOr;
                    category = OperatorCategory.Logical;
                    return flag;

                case SymbolType.LogicalXor:
                    type = OperatorType.LogicalXor;
                    category = OperatorCategory.Logical;
                    return flag;

                case SymbolType.XorEquals:
                    type = OperatorType.XorEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.Not:
                    type = OperatorType.Not;
                    category = OperatorCategory.Unary;
                    return flag;

                case SymbolType.NotEquals:
                    type = OperatorType.NotEquals;
                    category = OperatorCategory.Relational;
                    return flag;

                case SymbolType.Mod:
                    type = OperatorType.Mod;
                    category = OperatorCategory.Arithmetic;
                    return flag;

                case SymbolType.ModEquals:
                    type = OperatorType.ModEquals;
                    category = OperatorCategory.Assignment;
                    return flag;

                case SymbolType.Dot:
                    type = OperatorType.MemberAccess;
                    category = OperatorCategory.Reference;
                    return flag;

                case SymbolType.Pointer:
                    type = OperatorType.Pointer;
                    category = OperatorCategory.Reference;
                    return flag;

                case SymbolType.Colon:
                    type = OperatorType.ConditionalColon;
                    category = OperatorCategory.Conditional;
                    return flag;

                case SymbolType.QualifiedAlias:
                    type = OperatorType.QualifiedAlias;
                    category = OperatorCategory.Reference;
                    return flag;

                case SymbolType.QuestionMark:
                    type = OperatorType.ConditionalQuestionMark;
                    category = OperatorCategory.Conditional;
                    return flag;

                case SymbolType.NullCoalescingSymbol:
                    type = OperatorType.NullCoalescingSymbol;
                    category = OperatorCategory.Logical;
                    return flag;

                case SymbolType.Tilde:
                    type = OperatorType.BitwiseCompliment;
                    category = OperatorCategory.Unary;
                    return flag;

                case SymbolType.Lambda:
                    type = OperatorType.Lambda;
                    category = OperatorCategory.Lambda;
                    return flag;
            }
            type = OperatorType.AddressOf;
            category = OperatorCategory.Arithmetic;
            return false;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method contains a switch statement, but it not complex.")]
        private bool GetOtherElementModifier(string[] allowedModifiers, Dictionary<CsTokenType, CsToken> modifiers, Symbol symbol)
        {
            bool flag = true;
            if (allowedModifiers != null)
            {
                for (int i = 0; i < allowedModifiers.Length; i++)
                {
                    CsTokenType @explicit;
                    CsToken token;
                    if (!string.Equals(symbol.Text, allowedModifiers[i], StringComparison.Ordinal))
                    {
                        goto Label_014E;
                    }
                    switch (symbol.SymbolType)
                    {
                        case SymbolType.Explicit:
                            @explicit = CsTokenType.Explicit;
                            goto Label_0128;

                        case SymbolType.Extern:
                            @explicit = CsTokenType.Extern;
                            goto Label_0128;

                        case SymbolType.Fixed:
                            @explicit = CsTokenType.Fixed;
                            goto Label_0128;

                        case SymbolType.Implicit:
                            @explicit = CsTokenType.Implicit;
                            goto Label_0128;

                        case SymbolType.New:
                            @explicit = CsTokenType.New;
                            goto Label_0128;

                        case SymbolType.Abstract:
                            @explicit = CsTokenType.Abstract;
                            goto Label_0128;

                        case SymbolType.Const:
                            @explicit = CsTokenType.Const;
                            goto Label_0128;

                        case SymbolType.Override:
                            @explicit = CsTokenType.Override;
                            goto Label_0128;

                        case SymbolType.Readonly:
                            @explicit = CsTokenType.Readonly;
                            goto Label_0128;

                        case SymbolType.Unsafe:
                            @explicit = CsTokenType.Unsafe;
                            goto Label_0128;

                        case SymbolType.Virtual:
                            @explicit = CsTokenType.Virtual;
                            goto Label_0128;

                        case SymbolType.Volatile:
                            @explicit = CsTokenType.Volatile;
                            goto Label_0128;

                        case SymbolType.Other:
                            if (symbol.Text != "partial")
                            {
                                break;
                            }
                            @explicit = CsTokenType.Partial;
                            goto Label_0128;

                        case SymbolType.Static:
                            @explicit = CsTokenType.Static;
                            goto Label_0128;

                        case SymbolType.Sealed:
                            @explicit = CsTokenType.Sealed;
                            goto Label_0128;
                    }
                    throw this.CreateSyntaxException();
                Label_0128:
                    token = this.GetToken(@explicit, symbol.SymbolType);
                    modifiers.Add(@explicit, token);
                    this.tokens.Add(token);
                    flag = false;
                    break;
                Label_014E:;
                }
            }
            return !flag;
        }

        private Expression GetOtherExpression(bool allowVariableDeclaration, bool unsafeCode)
        {
            int num;
            bool flag = false;
            if (allowVariableDeclaration && this.HasTypeSignature(1, unsafeCode, out num))
            {
                int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(num + 1);
                if ((nextCodeSymbolIndex != -1) && (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.Other))
                {
                    nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex + 1);
                    if (nextCodeSymbolIndex != -1)
                    {
                        Symbol symbol = this.symbols.Peek(nextCodeSymbolIndex);
                        if (((symbol.SymbolType == SymbolType.Equals) || (symbol.SymbolType == SymbolType.Semicolon)) || (((symbol.SymbolType == SymbolType.CloseParenthesis) || (symbol.SymbolType == SymbolType.Comma)) || (symbol.SymbolType == SymbolType.In)))
                        {
                            flag = true;
                        }
                    }
                }
            }
            if (flag)
            {
                LiteralExpression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, true);
                if ((typeTokenExpression == null) || (typeTokenExpression.Tokens.First == null))
                {
                    throw this.CreateSyntaxException();
                }
                return this.GetVariableDeclarationExpression(typeTokenExpression, ExpressionPrecedence.None, unsafeCode);
            }
            return this.GetLiteralExpression(unsafeCode);
        }

        private ParenthesizedExpression GetParenthesizedExpression(bool unsafeCode)
        {
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node2;
            item.MatchingBracketNode = firstItemNode;
            return new ParenthesizedExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression);
        }

        private Preprocessor GetPreprocessorDirectiveToken(Symbol preprocessorSymbol, bool generated)
        {
            int num;
            string preprocessorDirectiveType = CsParser.GetPreprocessorDirectiveType(preprocessorSymbol, out num);
            if (preprocessorDirectiveType == null)
            {
                throw new SyntaxException(this.document.SourceCode, preprocessorSymbol.LineNumber);
            }
            switch (preprocessorDirectiveType)
            {
                case "region":
                {
                    Region region = new Region(preprocessorSymbol.Text, preprocessorSymbol.Location, true, generated);
                    this.symbols.PushRegion(region);
                    return region;
                }
                case "endregion":
                {
                    Region region2 = new Region(preprocessorSymbol.Text, preprocessorSymbol.Location, false, generated);
                    Region region3 = this.symbols.PopRegion();
                    if (region3 == null)
                    {
                        throw new SyntaxException(this.document.SourceCode, preprocessorSymbol.LineNumber);
                    }
                    region3.Partner = region2;
                    region2.Partner = region3;
                    return region2;
                }
                case "if":
                    return this.GetConditionalCompilationDirective(preprocessorSymbol, ConditionalCompilationDirectiveType.If, num, generated);

                case "elif":
                    return this.GetConditionalCompilationDirective(preprocessorSymbol, ConditionalCompilationDirectiveType.Elif, num, generated);

                case "else":
                    return this.GetConditionalCompilationDirective(preprocessorSymbol, ConditionalCompilationDirectiveType.Else, num, generated);

                case "endif":
                    return this.GetConditionalCompilationDirective(preprocessorSymbol, ConditionalCompilationDirectiveType.Endif, num, generated);
            }
            return new Preprocessor(preprocessorSymbol.Text, preprocessorSymbol.Location, generated);
        }

        private DecrementExpression GetPrimaryDecrementExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence)
        {
            DecrementExpression expression = null;
            if (CheckPrecedence(previousPrecedence, ExpressionPrecedence.Primary))
            {
                this.tokens.Add(this.GetOperatorToken(OperatorType.Decrement));
                CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
                expression = new DecrementExpression(tokens, leftHandSide, DecrementExpression.DecrementType.Postfix);
            }
            return expression;
        }

        private IncrementExpression GetPrimaryIncrementExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence)
        {
            IncrementExpression expression = null;
            if (CheckPrecedence(previousPrecedence, ExpressionPrecedence.Primary))
            {
                this.tokens.Add(this.GetOperatorToken(OperatorType.Increment));
                CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
                expression = new IncrementExpression(tokens, leftHandSide, IncrementExpression.IncrementType.Postfix);
            }
            return expression;
        }

        private QueryContinuationClause GetQueryContinuationClause(bool unsafeCode)
        {
            this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Into, SymbolType.Other));
            Variable variable = this.GetVariable(unsafeCode, true, true);
            if (variable == null)
            {
                throw this.CreateSyntaxException();
            }
            List<QueryClause> clauses = new List<QueryClause>();
            List<Variable> variables = new List<Variable>();
            CsTokenList list3 = this.GetQueryExpressionClauses(unsafeCode, clauses, variables);
            if ((clauses.Count == 0) || (list3.First == null))
            {
                throw this.CreateSyntaxException();
            }
            QueryContinuationClause clause = new QueryContinuationClause(new CsTokenList(this.tokens, firstItemNode, list3.Last), variable, clauses.ToArray());
            clause.Variables.AddRange(variables);
            return clause;
        }

        private QueryExpression GetQueryExpression(bool unsafeCode)
        {
            this.GetNextSymbol();
            List<QueryClause> clauses = new List<QueryClause>();
            List<Variable> variables = new List<Variable>();
            CsTokenList tokens = this.GetQueryExpressionClauses(unsafeCode, clauses, variables);
            if ((clauses.Count == 0) || (tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            QueryExpression expression = new QueryExpression(tokens, clauses.ToArray());
            expression.Variables.AddRange(variables);
            return expression;
        }

        private CsTokenList GetQueryExpressionClauses(bool unsafeCode, List<QueryClause> clauses, List<Variable> variables)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = null;
            Symbol symbol;
        Label_0002:
            symbol = this.GetNextSymbol();
            if (symbol.SymbolType == SymbolType.Other)
            {
                QueryClause item = null;
                switch (symbol.Text)
                {
                    case "from":
                    {
                        QueryFromClause queryFromClause = this.GetQueryFromClause(unsafeCode);
                        variables.Add(queryFromClause.RangeVariable);
                        item = queryFromClause;
                        break;
                    }
                    case "let":
                    {
                        QueryLetClause queryLetClause = this.GetQueryLetClause(unsafeCode);
                        variables.Add(queryLetClause.RangeVariable);
                        item = queryLetClause;
                        break;
                    }
                    case "where":
                        item = this.GetQueryWhereClause(unsafeCode);
                        break;

                    case "join":
                    {
                        QueryJoinClause queryJoinClause = this.GetQueryJoinClause(unsafeCode);
                        variables.Add(queryJoinClause.RangeVariable);
                        if (queryJoinClause.IntoVariable != null)
                        {
                            variables.Add(queryJoinClause.IntoVariable);
                        }
                        item = queryJoinClause;
                        break;
                    }
                    case "orderby":
                        item = this.GetQueryOrderByClause(unsafeCode);
                        break;

                    case "select":
                        item = this.GetQuerySelectClause(unsafeCode);
                        break;

                    case "group":
                        item = this.GetQueryGroupClause(unsafeCode);
                        break;

                    case "into":
                    {
                        QueryContinuationClause queryContinuationClause = this.GetQueryContinuationClause(unsafeCode);
                        variables.Add(queryContinuationClause.Variable);
                        item = queryContinuationClause;
                        break;
                    }
                }
                if (item != null)
                {
                    if (firstItemNode == null)
                    {
                        firstItemNode = item.Tokens.First;
                    }
                    clauses.Add(item);
                    goto Label_0002;
                }
            }
            if (firstItemNode == null)
            {
                throw this.CreateSyntaxException();
            }
            return new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
        }

        private QueryFromClause GetQueryFromClause(bool unsafeCode)
        {
            this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.From, SymbolType.Other));
            Variable rangeVariable = this.GetVariable(unsafeCode, true, false);
            if (rangeVariable == null)
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(this.GetToken(CsTokenType.In, SymbolType.In));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            return new QueryFromClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), rangeVariable, nextExpression);
        }

        private QueryGroupClause GetQueryGroupClause(bool unsafeCode)
        {
            Symbol nextSymbol = this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Group, SymbolType.Other));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            if (this.GetNextSymbol(SymbolType.Other).Text != "by")
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(this.GetToken(CsTokenType.By, SymbolType.Other));
            Expression groupByExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (groupByExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            return new QueryGroupClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression, groupByExpression);
        }

        private QueryJoinClause GetQueryJoinClause(bool unsafeCode)
        {
            Symbol nextSymbol = this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Join, SymbolType.Other));
            Variable rangeVariable = this.GetVariable(unsafeCode, true, false);
            if (rangeVariable == null)
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(this.GetToken(CsTokenType.In, SymbolType.In));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            CsToken item = this.GetToken(CsTokenType.On, SymbolType.Other);
            if (item.Text != "on")
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(item);
            Expression onKeyExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (onKeyExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            item = this.GetToken(CsTokenType.Equals, SymbolType.Other);
            if (item.Text != "equals")
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(item);
            Expression equalsKeyExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (equalsKeyExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Variable intoVariable = null;
            nextSymbol = this.GetNextSymbol();
            if ((nextSymbol.SymbolType == SymbolType.Other) && (nextSymbol.Text == "into"))
            {
                this.tokens.Add(this.GetToken(CsTokenType.Into, SymbolType.Other));
                intoVariable = this.GetVariable(unsafeCode, true, true);
            }
            return new QueryJoinClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), rangeVariable, nextExpression, onKeyExpression, equalsKeyExpression, intoVariable);
        }

        private QueryLetClause GetQueryLetClause(bool unsafeCode)
        {
            this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Let, SymbolType.Other));
            Variable rangeVariable = this.GetVariable(unsafeCode, true, true);
            if (rangeVariable == null)
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(this.GetOperatorToken(OperatorType.Equals));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            return new QueryLetClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), rangeVariable, nextExpression);
        }

        private QueryOrderByClause GetQueryOrderByClause(bool unsafeCode)
        {
            QueryOrderByOrdering ordering;
            Symbol nextSymbol = this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.OrderBy, SymbolType.Other));
            List<QueryOrderByOrdering> orderings = new List<QueryOrderByOrdering>();
        Label_0025:
            ordering = new QueryOrderByOrdering();
            ordering.Expression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (ordering.Expression == null)
            {
                throw this.CreateSyntaxException();
            }
            nextSymbol = this.GetNextSymbol();
            ordering.Direction = QueryOrderByDirection.Undefined;
            if (nextSymbol.Text == "ascending")
            {
                ordering.Direction = QueryOrderByDirection.Ascending;
                this.tokens.Add(this.GetToken(CsTokenType.Ascending, SymbolType.Other));
            }
            else if (nextSymbol.Text == "descending")
            {
                ordering.Direction = QueryOrderByDirection.Descending;
                this.tokens.Add(this.GetToken(CsTokenType.Descending, SymbolType.Other));
            }
            orderings.Add(ordering);
            if (this.GetNextSymbol().SymbolType == SymbolType.Comma)
            {
                this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                goto Label_0025;
            }
            return new QueryOrderByClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), orderings);
        }

        private QuerySelectClause GetQuerySelectClause(bool unsafeCode)
        {
            this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Select, SymbolType.Other));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            return new QuerySelectClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression);
        }

        private QueryWhereClause GetQueryWhereClause(bool unsafeCode)
        {
            this.GetNextSymbol(SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Where, SymbolType.Other));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Query, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            return new QueryWhereClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression);
        }

        private RelationalExpression GetRelationalExpression(Expression leftHandSide, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            RelationalExpression expression = null;
            RelationalExpression.Operator equalTo;
            OperatorSymbol item = this.PeekOperatorToken();
            ExpressionPrecedence operatorPrecedence = GetOperatorPrecedence(item.SymbolType);
            if (!CheckPrecedence(previousPrecedence, operatorPrecedence))
            {
                return expression;
            }
            this.symbols.Advance();
            this.tokens.Add(item);
            Expression operatorRightHandExpression = this.GetOperatorRightHandExpression(operatorPrecedence, unsafeCode);
            CsTokenList tokens = new CsTokenList(this.tokens, leftHandSide.Tokens.First, this.tokens.Last);
            switch (item.SymbolType)
            {
                case OperatorType.ConditionalEquals:
                    equalTo = RelationalExpression.Operator.EqualTo;
                    break;

                case OperatorType.NotEquals:
                    equalTo = RelationalExpression.Operator.NotEqualTo;
                    break;

                case OperatorType.LessThan:
                    equalTo = RelationalExpression.Operator.LessThan;
                    break;

                case OperatorType.GreaterThan:
                    equalTo = RelationalExpression.Operator.GreaterThan;
                    break;

                case OperatorType.LessThanOrEquals:
                    equalTo = RelationalExpression.Operator.LessThanOrEqualTo;
                    break;

                case OperatorType.GreaterThanOrEquals:
                    equalTo = RelationalExpression.Operator.GreaterThanOrEqualTo;
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return new RelationalExpression(tokens, equalTo, leftHandSide, operatorRightHandExpression);
        }

        private SizeofExpression GetSizeofExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Sizeof, SymbolType.Sizeof));
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, true);
            if (typeTokenExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node2;
            return new SizeofExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), typeTokenExpression);
        }

        private StackallocExpression GetStackallocExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Stackalloc, SymbolType.Stackalloc));
            this.GetNextSymbol(SymbolType.Other);
            Expression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, false);
            if ((typeTokenExpression == null) || (typeTokenExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            ArrayAccessExpression type = this.GetArrayAccessExpression(typeTokenExpression, ExpressionPrecedence.None, unsafeCode);
            if ((type == null) || (type.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            return new StackallocExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), type);
        }

        private CsToken GetToken(CsTokenType tokenType, SymbolType symbolType)
        {
            SkipSymbols all = SkipSymbols.All;
            if (symbolType == SymbolType.WhiteSpace)
            {
                all &= ~SkipSymbols.WhiteSpace;
            }
            else if (symbolType == SymbolType.EndOfLine)
            {
                all &= ~SkipSymbols.EndOfLine;
            }
            else if (symbolType == SymbolType.SingleLineComment)
            {
                all &= ~SkipSymbols.SingleLineComment;
            }
            else if (symbolType == SymbolType.MultiLineComment)
            {
                all &= ~SkipSymbols.MultiLineComment;
            }
            else if (symbolType == SymbolType.PreprocessorDirective)
            {
                all &= ~SkipSymbols.Preprocessor;
            }
            else if (symbolType == SymbolType.XmlHeaderLine)
            {
                all &= ~SkipSymbols.XmlHeader;
            }
            Symbol nextSymbol = this.GetNextSymbol(symbolType, all);
            this.symbols.Advance();
            return this.ConvertSymbol(nextSymbol, tokenType);
        }

        private TypeofExpression GetTypeofExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Typeof, SymbolType.Typeof));
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            LiteralExpression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, true);
            if (typeTokenExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node2;
            return new TypeofExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), typeTokenExpression);
        }

        private TypeToken GetTypeToken(bool unsafeCode, bool includeArrayBrackets)
        {
            return this.GetTypeToken(unsafeCode, includeArrayBrackets, false);
        }

        private TypeToken GetTypeToken(bool unsafeCode, bool includeArrayBrackets, bool isExpression)
        {
            int num;
            this.GetNextSymbol(SymbolType.Other);
            TypeToken token = this.GetTypeTokenAux(unsafeCode, includeArrayBrackets, isExpression, 1, out num);
            if (token != null)
            {
                this.symbols.CurrentIndex += num;
            }
            return token;
        }

        private void GetTypeTokenArrayBrackets(MasterList<CsToken> typeTokens, ref int startIndex)
        {
            Symbol symbol;
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(startIndex);
            if ((nextCodeSymbolIndex == -1) || (this.symbols.Peek(nextCodeSymbolIndex).SymbolType != SymbolType.OpenSquareBracket))
            {
                return;
            }
            for (int i = startIndex; i <= (nextCodeSymbolIndex - 1); i++)
            {
                Symbol symbol2 = this.symbols.Peek(startIndex);
                typeTokens.Add(this.ConvertSymbol(symbol2, TokenTypeFromSymbolType(symbol2.SymbolType)));
                startIndex++;
            }
            Microsoft.StyleCop.Node<CsToken> node = null;
        Label_0067:
            symbol = this.symbols.Peek(startIndex);
            if (((symbol.SymbolType == SymbolType.WhiteSpace) || (symbol.SymbolType == SymbolType.EndOfLine)) || (((symbol.SymbolType == SymbolType.SingleLineComment) || (symbol.SymbolType == SymbolType.MultiLineComment)) || (symbol.SymbolType == SymbolType.PreprocessorDirective)))
            {
                typeTokens.Add(this.ConvertSymbol(symbol, TokenTypeFromSymbolType(symbol.SymbolType)));
                startIndex++;
                goto Label_0067;
            }
            if (symbol.SymbolType == SymbolType.Number)
            {
                typeTokens.Add(this.ConvertSymbol(symbol, CsTokenType.Number));
                startIndex++;
                goto Label_0067;
            }
            if (symbol.SymbolType == SymbolType.Other)
            {
                typeTokens.Add(this.ConvertSymbol(symbol, CsTokenType.Other));
                startIndex++;
                goto Label_0067;
            }
            if (symbol.SymbolType == SymbolType.Comma)
            {
                typeTokens.Add(this.ConvertSymbol(symbol, CsTokenType.Comma));
                startIndex++;
                goto Label_0067;
            }
            if (symbol.SymbolType == SymbolType.OpenSquareBracket)
            {
                if (node != null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                }
                Bracket item = new Bracket(symbol.Text, CsTokenType.OpenSquareBracket, symbol.Location, this.symbols.Generated);
                node = typeTokens.InsertLast(item);
                startIndex++;
                goto Label_0067;
            }
            if (symbol.SymbolType == SymbolType.CloseSquareBracket)
            {
                if (node == null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                }
                Bracket bracket2 = new Bracket(symbol.Text, CsTokenType.CloseSquareBracket, symbol.Location, this.symbols.Generated);
                Microsoft.StyleCop.Node<CsToken> node2 = typeTokens.InsertLast(bracket2);
                startIndex++;
                ((Bracket) node.Value).MatchingBracketNode = node2;
                bracket2.MatchingBracketNode = node;
                node = null;
                int count = this.GetNextCodeSymbolIndex(startIndex);
                if ((count != -1) && (this.symbols.Peek(count).SymbolType != SymbolType.OpenSquareBracket))
                {
                    return;
                }
                goto Label_0067;
            }
            if (node != null)
            {
                throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
            }
        }

        private TypeToken GetTypeTokenAux(bool unsafeCode, bool includeArrayBrackets, bool isExpression, int startIndex, out int endIndex)
        {
            GenericType type;
            this.symbols.Peek(startIndex);
            MasterList<CsToken> typeTokens = new MasterList<CsToken>();
            this.GetTypeTokenBaseName(ref typeTokens, ref startIndex, out type, unsafeCode);
            bool flag = true;
            if (unsafeCode && this.GetTypeTokenDereferenceSymbols(typeTokens, ref startIndex))
            {
                flag = false;
            }
            if (flag)
            {
                this.GetTypeTokenNullableTypeSymbol(typeTokens, isExpression, ref startIndex);
            }
            if (includeArrayBrackets)
            {
                this.GetTypeTokenArrayBrackets(typeTokens, ref startIndex);
            }
            if (typeTokens.Count == 0)
            {
                throw this.CreateSyntaxException();
            }
            endIndex = startIndex - 1;
            if ((type != null) && (typeTokens.Count == 1))
            {
                return type;
            }
            return new TypeToken(typeTokens, CodeLocation.Join<CsToken>(typeTokens.First, typeTokens.Last), this.symbols.Generated);
        }

        private void GetTypeTokenBaseName(ref MasterList<CsToken> typeTokens, ref int startIndex, out GenericType generic, bool unsafeCode)
        {
            generic = null;
            Symbol symbol = this.symbols.Peek(startIndex);
            int count = -1;
        Label_0045:
            while ((symbol != null) && ((((symbol.SymbolType == SymbolType.WhiteSpace) || (symbol.SymbolType == SymbolType.EndOfLine)) || ((symbol.SymbolType == SymbolType.SingleLineComment) || (symbol.SymbolType == SymbolType.MultiLineComment))) || (symbol.SymbolType == SymbolType.PreprocessorDirective)))
            {
                int num3;
                typeTokens.Add(this.ConvertSymbol(symbol, TokenTypeFromSymbolType(symbol.SymbolType)));
                startIndex = num3 = startIndex + 1;
                symbol = this.symbols.Peek(num3);
            }
            if ((symbol.SymbolType != SymbolType.Other) && (symbol.SymbolType != SymbolType.This))
            {
                throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
            }
            typeTokens.Add(new CsToken(symbol.Text, CsTokenType.Other, symbol.Location, this.symbols.Generated));
            startIndex++;
            count = this.GetNextCodeSymbolIndex(startIndex);
            if (count != -1)
            {
                if (this.symbols.Peek(count).SymbolType == SymbolType.LessThan)
                {
                    int num2;
                    MasterList<CsToken> items = this.GetGenericArgumentList(unsafeCode, null, startIndex, out num2);
                    if (items != null)
                    {
                        typeTokens.AddRange(items);
                        CodeLocation location = CodeLocation.Join<CsToken>(typeTokens.First, typeTokens.Last);
                        generic = new GenericType(typeTokens, location, this.symbols.Generated);
                        typeTokens = new MasterList<CsToken>();
                        typeTokens.Add(generic);
                        startIndex = num2 + 1;
                        count = this.GetNextCodeSymbolIndex(startIndex);
                        if (count == -1)
                        {
                            return;
                        }
                    }
                }
                symbol = this.symbols.Peek(count);
                if ((symbol.SymbolType == SymbolType.Dot) || (symbol.SymbolType == SymbolType.QualifiedAlias))
                {
                    int num5;
                    symbol = this.symbols.Peek(startIndex);
                    while ((symbol != null) && ((((symbol.SymbolType == SymbolType.WhiteSpace) || (symbol.SymbolType == SymbolType.EndOfLine)) || ((symbol.SymbolType == SymbolType.SingleLineComment) || (symbol.SymbolType == SymbolType.MultiLineComment))) || (symbol.SymbolType == SymbolType.PreprocessorDirective)))
                    {
                        int num4;
                        typeTokens.Add(this.ConvertSymbol(symbol, TokenTypeFromSymbolType(symbol.SymbolType)));
                        startIndex = num4 = startIndex + 1;
                        symbol = this.symbols.Peek(num4);
                    }
                    if (symbol.SymbolType == SymbolType.Dot)
                    {
                        typeTokens.Add(new OperatorSymbol(symbol.Text, OperatorCategory.Reference, OperatorType.MemberAccess, symbol.Location, this.symbols.Generated));
                    }
                    else
                    {
                        typeTokens.Add(new OperatorSymbol(symbol.Text, OperatorCategory.Reference, OperatorType.QualifiedAlias, symbol.Location, this.symbols.Generated));
                    }
                    startIndex = num5 = startIndex + 1;
                    symbol = this.symbols.Peek(num5);
                    goto Label_0045;
                }
            }
        }

        private bool GetTypeTokenDereferenceSymbols(MasterList<CsToken> typeTokens, ref int startIndex)
        {
            bool flag = false;
            while (true)
            {
                int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(startIndex);
                if ((nextCodeSymbolIndex == -1) || (this.symbols.Peek(nextCodeSymbolIndex).SymbolType != SymbolType.Multiplication))
                {
                    return flag;
                }
                Symbol symbol = this.symbols.Peek(startIndex);
                while ((symbol != null) && ((((symbol.SymbolType == SymbolType.WhiteSpace) || (symbol.SymbolType == SymbolType.EndOfLine)) || ((symbol.SymbolType == SymbolType.SingleLineComment) || (symbol.SymbolType == SymbolType.MultiLineComment))) || (symbol.SymbolType == SymbolType.PreprocessorDirective)))
                {
                    int num2;
                    typeTokens.Add(this.ConvertSymbol(symbol, TokenTypeFromSymbolType(symbol.SymbolType)));
                    startIndex = num2 = startIndex + 1;
                    symbol = this.symbols.Peek(num2);
                }
                typeTokens.Add(new OperatorSymbol(symbol.Text, OperatorCategory.Reference, OperatorType.Dereference, symbol.Location, this.symbols.Generated));
                startIndex++;
                flag = true;
            }
        }

        private LiteralExpression GetTypeTokenExpression(bool unsafeCode, bool includeArrayBrackets)
        {
            return this.GetTypeTokenExpression(unsafeCode, includeArrayBrackets, false);
        }

        private LiteralExpression GetTypeTokenExpression(bool unsafeCode, bool includeArrayBrackets, bool isExpression)
        {
            TypeToken item = this.GetTypeToken(unsafeCode, includeArrayBrackets, isExpression);
            Microsoft.StyleCop.Node<CsToken> tokenNode = this.tokens.InsertLast(item);
            return new LiteralExpression(new CsTokenList(this.tokens, tokenNode, tokenNode), tokenNode);
        }

        private void GetTypeTokenNullableTypeSymbol(MasterList<CsToken> typeTokens, bool isExpression, ref int startIndex)
        {
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(startIndex);
            if (nextCodeSymbolIndex == -1)
            {
                throw this.CreateSyntaxException();
            }
            if ((this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.QuestionMark) && (!isExpression || this.IsNullableTypeSymbolFromIsExpression(nextCodeSymbolIndex)))
            {
                Symbol symbol = this.symbols.Peek(startIndex);
                while ((symbol != null) && ((((symbol.SymbolType == SymbolType.WhiteSpace) || (symbol.SymbolType == SymbolType.EndOfLine)) || ((symbol.SymbolType == SymbolType.SingleLineComment) || (symbol.SymbolType == SymbolType.MultiLineComment))) || (symbol.SymbolType == SymbolType.PreprocessorDirective)))
                {
                    int num2;
                    typeTokens.Add(this.ConvertSymbol(symbol, TokenTypeFromSymbolType(symbol.SymbolType)));
                    startIndex = num2 = startIndex + 1;
                    symbol = this.symbols.Peek(num2);
                }
                typeTokens.Add(new CsToken(symbol.Text, CsTokenType.NullableTypeSymbol, symbol.Location, this.symbols.Generated));
                startIndex++;
            }
        }

        private DecrementExpression GetUnaryDecrementExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetOperatorToken(OperatorType.Decrement));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Unary, unsafeCode);
            if ((nextExpression == null) || (nextExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            return new DecrementExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression, DecrementExpression.DecrementType.Prefix);
        }

        private UnaryExpression GetUnaryExpression(bool unsafeCode)
        {
            OperatorSymbol symbol2;
            UnaryExpression.Operator positive;
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol.SymbolType == SymbolType.Plus)
            {
                positive = UnaryExpression.Operator.Positive;
                symbol2 = new OperatorSymbol(nextSymbol.Text, OperatorCategory.Unary, OperatorType.Positive, nextSymbol.Location, this.symbols.Generated);
            }
            else if (nextSymbol.SymbolType == SymbolType.Minus)
            {
                positive = UnaryExpression.Operator.Negative;
                symbol2 = new OperatorSymbol(nextSymbol.Text, OperatorCategory.Unary, OperatorType.Negative, nextSymbol.Location, this.symbols.Generated);
            }
            else if (nextSymbol.SymbolType == SymbolType.Not)
            {
                positive = UnaryExpression.Operator.Not;
                symbol2 = new OperatorSymbol(nextSymbol.Text, OperatorCategory.Unary, OperatorType.Not, nextSymbol.Location, this.symbols.Generated);
            }
            else
            {
                if (nextSymbol.SymbolType != SymbolType.Tilde)
                {
                    throw new StyleCopException();
                }
                positive = UnaryExpression.Operator.BitwiseCompliment;
                symbol2 = new OperatorSymbol(nextSymbol.Text, OperatorCategory.Unary, OperatorType.BitwiseCompliment, nextSymbol.Location, this.symbols.Generated);
            }
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(symbol2);
            this.symbols.Advance();
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Unary, unsafeCode);
            if ((nextExpression == null) || (nextExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            return new UnaryExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), positive, nextExpression);
        }

        private IncrementExpression GetUnaryIncrementExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetOperatorToken(OperatorType.Increment));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Unary, unsafeCode);
            if ((nextExpression == null) || (nextExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            return new IncrementExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression, IncrementExpression.IncrementType.Prefix);
        }

        private UncheckedExpression GetUncheckedExpression(bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Unchecked, SymbolType.Unchecked));
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node2;
            return new UncheckedExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression);
        }

        private UnsafeAccessExpression GetUnsafeAccessExpression(bool unsafeCode)
        {
            OperatorType addressOf;
            UnsafeAccessExpression.Operator dereference;
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol.SymbolType == SymbolType.LogicalAnd)
            {
                addressOf = OperatorType.AddressOf;
                dereference = UnsafeAccessExpression.Operator.AddressOf;
            }
            else
            {
                if (nextSymbol.SymbolType != SymbolType.Multiplication)
                {
                    throw new InvalidOperationException();
                }
                addressOf = OperatorType.Dereference;
                dereference = UnsafeAccessExpression.Operator.Dereference;
            }
            this.symbols.Advance();
            OperatorSymbol item = new OperatorSymbol(nextSymbol.Text, OperatorCategory.Reference, addressOf, nextSymbol.Location, this.symbols.Generated);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.Unary, unsafeCode);
            if ((nextExpression == null) || (nextExpression.Tokens.First == null))
            {
                throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
            }
            return new UnsafeAccessExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), dereference, nextExpression);
        }

        private UnsafeAccessExpression GetUnsafeTypeExpression(Expression type, ExpressionPrecedence previousPrecedence)
        {
            UnsafeAccessExpression expression = null;
            OperatorType addressOf;
            UnsafeAccessExpression.Operator dereference;
            if (!CheckPrecedence(previousPrecedence, ExpressionPrecedence.Unary))
            {
                return expression;
            }
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol.SymbolType == SymbolType.LogicalAnd)
            {
                addressOf = OperatorType.AddressOf;
                dereference = UnsafeAccessExpression.Operator.AddressOf;
            }
            else
            {
                if (nextSymbol.SymbolType != SymbolType.Multiplication)
                {
                    throw new InvalidOperationException();
                }
                addressOf = OperatorType.Dereference;
                dereference = UnsafeAccessExpression.Operator.Dereference;
            }
            this.symbols.Advance();
            OperatorSymbol item = new OperatorSymbol(nextSymbol.Text, OperatorCategory.Reference, addressOf, nextSymbol.Location, this.symbols.Generated);
            this.tokens.Add(item);
            return new UnsafeAccessExpression(new CsTokenList(this.tokens, type.Tokens.First, this.tokens.Last), dereference, type);
        }

        private Variable GetVariable(bool unsafeCode, bool allowTypelessVariable, bool onlyTypelessVariable)
        {
            this.AdvanceToNextCodeSymbol();
            TypeToken item = this.GetTypeToken(unsafeCode, true, false);
            if (item == null)
            {
                throw this.CreateSyntaxException();
            }
            if (onlyTypelessVariable)
            {
                this.tokens.Add(item.ChildTokens.First.Value);
                return new Variable(null, item.Text, VariableModifiers.None, item.Location.StartPoint, item.Generated);
            }
            this.AdvanceToNextCodeSymbol();
            Symbol symbol = this.symbols.Peek(1);
            if ((symbol == null) || (symbol.SymbolType != SymbolType.Other))
            {
                if (!allowTypelessVariable)
                {
                    throw this.CreateSyntaxException();
                }
                this.tokens.Add(item.ChildTokens.First.Value);
                return new Variable(null, item.Text, VariableModifiers.None, item.Location.StartPoint, item.Generated);
            }
            this.tokens.Add(item);
            CsToken token2 = new CsToken(symbol.Text, CsTokenType.Other, CsTokenClass.Token, symbol.Location, this.symbols.Generated);
            this.tokens.Add(token2);
            this.symbols.Advance();
            return new Variable(item, token2.Text, VariableModifiers.None, item.Location.StartPoint, item.Generated || token2.Generated);
        }

        private VariableDeclarationExpression GetVariableDeclarationExpression(Expression type, ExpressionPrecedence previousPrecedence, bool unsafeCode)
        {
            VariableDeclarationExpression expression = null;
            Symbol symbol;
            if (!CheckPrecedence(previousPrecedence, ExpressionPrecedence.None))
            {
                return expression;
            }
            LiteralExpression expression2 = null;
            if (type.ExpressionType == ExpressionType.Literal)
            {
                expression2 = type as LiteralExpression;
                if (!(expression2.Token is TypeToken))
                {
                    expression2 = null;
                }
            }
            if (expression2 == null)
            {
                expression2 = this.ConvertTypeExpression(type);
            }
            List<VariableDeclaratorExpression> list = new List<VariableDeclaratorExpression>();
        Label_0042:
            symbol = this.GetNextSymbol(SymbolType.Other);
            LiteralExpression literalExpression = this.GetLiteralExpression(unsafeCode);
            if ((literalExpression == null) || (literalExpression.Tokens.First == null))
            {
                throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
            }
            Expression initializer = null;
            if (this.GetNextSymbol().SymbolType == SymbolType.Equals)
            {
                this.tokens.Add(this.GetOperatorToken(OperatorType.Equals));
                if (this.GetNextSymbol().SymbolType == SymbolType.OpenCurlyBracket)
                {
                    initializer = this.GetArrayInitializerExpression(unsafeCode);
                }
                else
                {
                    initializer = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                }
            }
            CsTokenList tokens = new CsTokenList(this.tokens, literalExpression.Tokens.First, this.tokens.Last);
            list.Add(new VariableDeclaratorExpression(tokens, literalExpression, initializer));
            if (this.GetNextSymbol().SymbolType == SymbolType.Comma)
            {
                this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                goto Label_0042;
            }
            return new VariableDeclarationExpression(new CsTokenList(this.tokens, type.Tokens.First, this.tokens.Last), expression2, list.ToArray());
        }

        private XmlHeader GetXmlHeader()
        {
            int count = 1;
            Symbol symbol = this.symbols.Peek(count);
            int num2 = -1;
            int num3 = 0;
            for (Symbol symbol2 = symbol; symbol2 != null; symbol2 = this.symbols.Peek(++count))
            {
                if (symbol2.SymbolType == SymbolType.XmlHeaderLine)
                {
                    num2 = count;
                    num3 = 0;
                }
                else if (symbol2.SymbolType == SymbolType.EndOfLine)
                {
                    if (++num3 > 1)
                    {
                        break;
                    }
                }
                else
                {
                    if ((symbol2.SymbolType != SymbolType.WhiteSpace) && (symbol2.SymbolType != SymbolType.SingleLineComment))
                    {
                        break;
                    }
                    num3 = 0;
                }
            }
            MasterList<CsToken> childTokens = new MasterList<CsToken>();
            for (int i = 1; i <= num2; i++)
            {
                this.symbols.Advance();
                childTokens.Add(this.ConvertSymbol(this.symbols.Current, TokenTypeFromSymbolType(this.symbols.Current.SymbolType)));
            }
            return new XmlHeader(childTokens, CodeLocation.Join(symbol.Location, this.symbols.Current.Location), this.symbols.Generated);
        }

        private bool HasTypeSignature(int startIndex, bool unsafeCode, out int endIndex)
        {
            bool flag;
            return this.HasTypeSignature(startIndex, unsafeCode, out endIndex, out flag);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="May be simplified later.")]
        private bool HasTypeSignature(int startIndex, bool unsafeCode, out int endIndex, out bool generic)
        {
            endIndex = -1;
            generic = false;
            bool flag = false;
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(startIndex);
            if (nextCodeSymbolIndex != -1)
            {
                bool flag2 = true;
                if (this.symbols.Peek(nextCodeSymbolIndex).SymbolType != SymbolType.Other)
                {
                    return flag;
                }
                flag = true;
                nextCodeSymbolIndex = this.AdvanceToEndOfName(nextCodeSymbolIndex, out generic);
                endIndex = nextCodeSymbolIndex;
                bool flag3 = true;
                while (flag3)
                {
                    flag3 = false;
                    if (nextCodeSymbolIndex != -1)
                    {
                        nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex + 1);
                        if (((nextCodeSymbolIndex != -1) && (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.Multiplication)) && unsafeCode)
                        {
                            flag2 = false;
                            endIndex = nextCodeSymbolIndex;
                            nextCodeSymbolIndex++;
                            flag3 = true;
                        }
                    }
                }
                if (flag2 && (nextCodeSymbolIndex != -1))
                {
                    nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex);
                    if ((nextCodeSymbolIndex != -1) && (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.QuestionMark))
                    {
                        endIndex = nextCodeSymbolIndex;
                        nextCodeSymbolIndex++;
                    }
                }
                bool flag4 = false;
                while (nextCodeSymbolIndex != -1)
                {
                    nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex);
                    if (nextCodeSymbolIndex != -1)
                    {
                        Symbol symbol = this.symbols.Peek(nextCodeSymbolIndex);
                        if (flag4)
                        {
                            if ((((symbol.SymbolType == SymbolType.WhiteSpace) || (symbol.SymbolType == SymbolType.EndOfLine)) || ((symbol.SymbolType == SymbolType.SingleLineComment) || (symbol.SymbolType == SymbolType.MultiLineComment))) || (((symbol.SymbolType == SymbolType.PreprocessorDirective) || (symbol.SymbolType == SymbolType.Number)) || (symbol.SymbolType == SymbolType.Comma)))
                            {
                                nextCodeSymbolIndex++;
                            }
                            else
                            {
                                if (symbol.SymbolType != SymbolType.CloseSquareBracket)
                                {
                                    return flag;
                                }
                                endIndex = nextCodeSymbolIndex;
                                nextCodeSymbolIndex++;
                                flag4 = false;
                            }
                        }
                        else
                        {
                            if (symbol.SymbolType != SymbolType.OpenSquareBracket)
                            {
                                return flag;
                            }
                            flag2 = false;
                            nextCodeSymbolIndex++;
                            flag4 = true;
                        }
                    }
                }
            }
            return flag;
        }

        private void InitializeElement(CsElement element)
        {
            Microsoft.StyleCop.Node<CsToken> first = element.Declaration.Tokens.First;
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            element.Tokens = new CsTokenList(element.Declaration.Tokens.MasterList, first, last);
            element.Location = CodeLocation.Join<CsToken>(first, last);
            element.Initialize();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="May be simplified later.")]
        private bool IsCastExpression(bool unsafeCode)
        {
            bool flag = false;
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(1);
            if (((nextCodeSymbolIndex != -1) && (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.OpenParenthesis)) && (this.HasTypeSignature(nextCodeSymbolIndex + 1, unsafeCode, out nextCodeSymbolIndex) && (nextCodeSymbolIndex != -1)))
            {
                if (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.Other)
                {
                    nextCodeSymbolIndex = this.AdvanceToEndOfName(nextCodeSymbolIndex);
                }
                if (nextCodeSymbolIndex == -1)
                {
                    return flag;
                }
                nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex + 1);
                if ((nextCodeSymbolIndex == -1) || (this.symbols.Peek(nextCodeSymbolIndex).SymbolType != SymbolType.CloseParenthesis))
                {
                    return flag;
                }
                nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(nextCodeSymbolIndex + 1);
                if (nextCodeSymbolIndex == -1)
                {
                    return flag;
                }
                SymbolType symbolType = this.symbols.Peek(nextCodeSymbolIndex).SymbolType;
                switch (symbolType)
                {
                    case SymbolType.Other:
                    case SymbolType.OpenParenthesis:
                    case SymbolType.Number:
                    case SymbolType.Tilde:
                    case SymbolType.Not:
                    case SymbolType.New:
                    case SymbolType.Sizeof:
                    case SymbolType.Typeof:
                    case SymbolType.Default:
                    case SymbolType.Checked:
                    case SymbolType.Unchecked:
                    case SymbolType.This:
                    case SymbolType.Base:
                    case SymbolType.Null:
                    case SymbolType.True:
                    case SymbolType.False:
                    case SymbolType.Plus:
                    case SymbolType.Minus:
                    case SymbolType.String:
                    case SymbolType.Delegate:
                        return true;
                }
                if ((symbolType == SymbolType.LogicalAnd) && unsafeCode)
                {
                    flag = true;
                }
            }
            return flag;
        }

        private static bool IsCodeAnalysisSuppression(Expression name)
        {
            if (name.ExpressionType == ExpressionType.Literal)
            {
                string text = name.Text;
                if (string.Equals(text, "SuppressMessage", StringComparison.Ordinal) || string.Equals(text, "SuppressMessageAttribute"))
                {
                    return true;
                }
            }
            else if ((name.ExpressionType == ExpressionType.MemberAccess) && (name.Tokens.MatchTokens(new string[] { "System", ".", "Diagnostics", ".", "CodeAnalysis", ".", "SuppressMessage" }) || name.Tokens.MatchTokens(new string[] { "System", ".", "Diagnostics", ".", "CodeAnalysis", ".", "SuppressMessageAttribute" })))
            {
                return true;
            }
            return false;
        }

        private bool IsDereferenceExpression(Expression leftSide)
        {
            Symbol symbol = this.symbols.Peek(1);
            bool flag = false;
            if (leftSide != null)
            {
                if ((!(leftSide is UnsafeAccessExpression) || (this.GetNextCodeSymbolIndex(2) == -1)) || ((symbol.SymbolType != SymbolType.CloseParenthesis) && (symbol.SymbolType != SymbolType.Multiplication)))
                {
                    return flag;
                }
                return true;
            }
            return true;
        }

        private bool IsLambdaExpression()
        {
            int count = 1;
            Symbol symbol = this.symbols.Peek(count);
            if (symbol.SymbolType == SymbolType.OpenParenthesis)
            {
                int num2 = 0;
                while (true)
                {
                    symbol = this.symbols.Peek(count);
                    if (symbol == null)
                    {
                        goto Label_0057;
                    }
                    if (symbol.SymbolType == SymbolType.OpenParenthesis)
                    {
                        num2++;
                    }
                    else if (symbol.SymbolType == SymbolType.CloseParenthesis)
                    {
                        num2--;
                        if (num2 == 0)
                        {
                            goto Label_0057;
                        }
                    }
                    count++;
                }
            }
            SymbolType symbolType = symbol.SymbolType;
        Label_0057:
            count++;
            while (true)
            {
                symbol = this.symbols.Peek(count);
                if (symbol == null)
                {
                    break;
                }
                if (symbol.SymbolType == SymbolType.Lambda)
                {
                    return true;
                }
                if (((symbol.SymbolType != SymbolType.EndOfLine) && (symbol.SymbolType != SymbolType.WhiteSpace)) && ((symbol.SymbolType != SymbolType.MultiLineComment) && (symbol.SymbolType != SymbolType.SingleLineComment)))
                {
                    break;
                }
                count++;
            }
            return false;
        }

        private bool IsNullableTypeSymbolFromIsExpression(int index)
        {
            index = this.GetNextCodeSymbolIndex(index + 1);
            Symbol symbol = this.symbols.Peek(index);
            if (symbol != null)
            {
                SymbolType symbolType = symbol.SymbolType;
                if ((((symbolType != SymbolType.CloseCurlyBracket) && (symbolType != SymbolType.CloseParenthesis)) && ((symbolType != SymbolType.CloseSquareBracket) && (symbolType != SymbolType.Comma))) && ((((symbolType != SymbolType.Semicolon) && (symbolType != SymbolType.ConditionalAnd)) && ((symbolType != SymbolType.ConditionalOr) && (symbolType != SymbolType.ConditionalEquals))) && (((symbolType != SymbolType.NotEquals) && (symbolType != SymbolType.QuestionMark)) && (symbolType != SymbolType.OpenSquareBracket))))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsQueryExpression(bool unsafeCode)
        {
            int count = 1;
            Symbol symbol = this.symbols.Peek(count);
            if ((symbol != null) && ((symbol.SymbolType == SymbolType.Other) && (string.CompareOrdinal(symbol.Text, "from") == 0)))
            {
                count = this.GetNextCodeSymbolIndex(count + 1);
                int endIndex = -1;
                if (this.HasTypeSignature(count, unsafeCode, out endIndex))
                {
                    count = this.GetNextCodeSymbolIndex(endIndex + 1);
                    symbol = this.symbols.Peek(count);
                    if (symbol != null)
                    {
                        if (symbol.SymbolType == SymbolType.In)
                        {
                            return true;
                        }
                        if (symbol.SymbolType == SymbolType.Other)
                        {
                            count = this.GetNextCodeSymbolIndex(count + 1);
                            symbol = this.symbols.Peek(count);
                            if ((symbol != null) && (symbol.SymbolType == SymbolType.In))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method is not complex.")]
        private bool IsUnaryExpression()
        {
            bool flag = false;
            int currentIndex = this.symbols.CurrentIndex;
            Symbol symbol = null;
            while (true)
            {
                symbol = this.symbols[currentIndex];
                if ((symbol == null) || (((symbol.SymbolType != SymbolType.WhiteSpace) && (symbol.SymbolType != SymbolType.EndOfLine)) && (((symbol.SymbolType != SymbolType.SingleLineComment) && (symbol.SymbolType != SymbolType.MultiLineComment)) && (symbol.SymbolType != SymbolType.PreprocessorDirective))))
                {
                    break;
                }
                currentIndex--;
            }
            if ((symbol == null) || ((((((symbol.SymbolType != SymbolType.Equals) && (symbol.SymbolType != SymbolType.PlusEquals)) && ((symbol.SymbolType != SymbolType.MinusEquals) && (symbol.SymbolType != SymbolType.MultiplicationEquals))) && (((symbol.SymbolType != SymbolType.DivisionEquals) && (symbol.SymbolType != SymbolType.OrEquals)) && ((symbol.SymbolType != SymbolType.AndEquals) && (symbol.SymbolType != SymbolType.XorEquals)))) && ((((symbol.SymbolType != SymbolType.LeftShiftEquals) && (symbol.SymbolType != SymbolType.RightShiftEquals)) && ((symbol.SymbolType != SymbolType.ModEquals) && (symbol.SymbolType != SymbolType.ConditionalEquals))) && (((symbol.SymbolType != SymbolType.NotEquals) && (symbol.SymbolType != SymbolType.GreaterThan)) && ((symbol.SymbolType != SymbolType.GreaterThanOrEquals) && (symbol.SymbolType != SymbolType.LessThan))))) && ((((((symbol.SymbolType != SymbolType.LessThanOrEquals) && (symbol.SymbolType != SymbolType.OpenCurlyBracket)) && ((symbol.SymbolType != SymbolType.CloseCurlyBracket) && (symbol.SymbolType != SymbolType.OpenParenthesis))) && (((symbol.SymbolType != SymbolType.CloseParenthesis) && (symbol.SymbolType != SymbolType.OpenSquareBracket)) && ((symbol.SymbolType != SymbolType.LogicalAnd) && (symbol.SymbolType != SymbolType.LogicalOr)))) && ((((symbol.SymbolType != SymbolType.LogicalXor) && (symbol.SymbolType != SymbolType.ConditionalAnd)) && ((symbol.SymbolType != SymbolType.ConditionalOr) && (symbol.SymbolType != SymbolType.Plus))) && (((symbol.SymbolType != SymbolType.Minus) && (symbol.SymbolType != SymbolType.Multiplication)) && ((symbol.SymbolType != SymbolType.Division) && (symbol.SymbolType != SymbolType.LeftShift))))) && (((((symbol.SymbolType != SymbolType.RightShift) && (symbol.SymbolType != SymbolType.Mod)) && ((symbol.SymbolType != SymbolType.Tilde) && (symbol.SymbolType != SymbolType.Case))) && (((symbol.SymbolType != SymbolType.QuestionMark) && (symbol.SymbolType != SymbolType.Colon)) && ((symbol.SymbolType != SymbolType.NullCoalescingSymbol) && (symbol.SymbolType != SymbolType.Comma)))) && (((symbol.SymbolType != SymbolType.Semicolon) && (symbol.SymbolType != SymbolType.Return)) && ((symbol.SymbolType != SymbolType.Throw) && (symbol.SymbolType != SymbolType.Else)))))))
            {
                return flag;
            }
            return true;
        }

        private void MovePastArrayBrackets()
        {
            for (Symbol symbol = this.GetNextSymbol(SymbolType.OpenSquareBracket); symbol.SymbolType == SymbolType.OpenSquareBracket; symbol = this.GetNextSymbol())
            {
                Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenSquareBracket, SymbolType.OpenSquareBracket);
                Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
                for (symbol = this.GetNextSymbol(); symbol.SymbolType == SymbolType.Comma; symbol = this.GetNextSymbol())
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                }
                Bracket item = this.GetBracketToken(CsTokenType.CloseSquareBracket, SymbolType.CloseSquareBracket);
                Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(item);
                bracketToken.MatchingBracketNode = node2;
                item.MatchingBracketNode = node;
            }
        }

        private static bool MovePastTokens(CsTokenList tokens, ref Microsoft.StyleCop.Node<CsToken> start, params CsTokenType[] movePastTypes)
        {
            for (Microsoft.StyleCop.Node<CsToken> node = start; !tokens.OutOfBounds(node); node = node.Next)
            {
                bool flag = false;
                for (int i = 0; i < movePastTypes.Length; i++)
                {
                    if (node.Value.CsTokenType == movePastTypes[i])
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    start = node;
                    return true;
                }
            }
            return false;
        }

        private void MoveToElementDeclaration(bool unsafeCode, out XmlHeader xmlHeader, out ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            xmlHeader = null;
            List<Microsoft.StyleCop.CSharp.Attribute> list = new List<Microsoft.StyleCop.CSharp.Attribute>();
            SkipSymbols all = SkipSymbols.All;
            all &= ~SkipSymbols.XmlHeader;
            bool flag = true;
            for (Symbol symbol = this.GetNextSymbol(all, true); (symbol != null) && flag; symbol = this.GetNextSymbol(all, true))
            {
                SymbolType symbolType = symbol.SymbolType;
                if (symbolType != SymbolType.OpenSquareBracket)
                {
                    if (symbolType != SymbolType.XmlHeaderLine)
                    {
                        goto Label_00EF;
                    }
                    xmlHeader = this.GetXmlHeader();
                    if (xmlHeader == null)
                    {
                        throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                    }
                    this.tokens.Add(xmlHeader);
                    continue;
                }
                Microsoft.StyleCop.CSharp.Attribute item = this.GetAttribute(unsafeCode);
                if (item == null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                }
                bool flag2 = false;
                foreach (AttributeExpression expression in item.AttributeExpressions)
                {
                    if (expression.IsAssemblyAttribute)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    list.Add(item);
                }
                this.tokens.Add(item);
                continue;
            Label_00EF:
                flag = false;
            }
            if (list == null)
            {
                attributes = null;
            }
            else
            {
                attributes = list.ToArray();
            }
        }

        internal static bool MoveToNextCodeToken(CsTokenList tokens, ref Microsoft.StyleCop.Node<CsToken> start)
        {
            return MovePastTokens(tokens, ref start, new CsTokenType[] { CsTokenType.WhiteSpace, CsTokenType.EndOfLine, CsTokenType.SingleLineComment, CsTokenType.MultiLineComment, CsTokenType.PreprocessorDirective, CsTokenType.Attribute });
        }

        private bool MoveToStatement()
        {
            bool flag = false;
            for (Symbol symbol = this.GetNextSymbol(); symbol != null; symbol = this.GetNextSymbol())
            {
                if (symbol.SymbolType == SymbolType.PreprocessorDirective)
                {
                    Preprocessor preprocessorDirectiveToken = this.GetPreprocessorDirectiveToken(symbol, this.symbols.Generated);
                    if (preprocessorDirectiveToken == null)
                    {
                        throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                    }
                    this.symbols.Advance();
                    this.tokens.Add(preprocessorDirectiveToken);
                }
                else if (symbol.SymbolType == SymbolType.XmlHeaderLine)
                {
                    XmlHeader xmlHeader = this.GetXmlHeader();
                    if (xmlHeader == null)
                    {
                        throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                    }
                    this.tokens.Add(xmlHeader);
                }
                else
                {
                    if (symbol.SymbolType == SymbolType.CloseCurlyBracket)
                    {
                        flag = true;
                    }
                    break;
                }
            }
            return !flag;
        }

        private Accessor ParseAccessor(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, null);
            AccessorType get = AccessorType.Get;
            CsToken item = null;
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol.Text == "get")
            {
                item = this.GetToken(CsTokenType.Get, SymbolType.Other);
                if ((parent.ElementType != ElementType.Property) && (parent.ElementType != ElementType.Indexer))
                {
                    throw this.CreateSyntaxException();
                }
            }
            else if (nextSymbol.Text == "set")
            {
                get = AccessorType.Set;
                item = this.GetToken(CsTokenType.Set, SymbolType.Other);
                if ((parent.ElementType != ElementType.Property) && (parent.ElementType != ElementType.Indexer))
                {
                    throw this.CreateSyntaxException();
                }
            }
            else if (nextSymbol.Text == "add")
            {
                get = AccessorType.Add;
                item = this.GetToken(CsTokenType.Add, SymbolType.Other);
                if (parent.ElementType != ElementType.Event)
                {
                    throw this.CreateSyntaxException();
                }
            }
            else
            {
                if (nextSymbol.Text != "remove")
                {
                    throw this.CreateSyntaxException();
                }
                get = AccessorType.Remove;
                item = this.GetToken(CsTokenType.Remove, SymbolType.Other);
                if (parent.ElementType != ElementType.Event)
                {
                    throw this.CreateSyntaxException();
                }
            }
            this.tokens.Add(item);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, item.Text, ElementType.Accessor, @private, elementModifiers);
            Accessor element = new Accessor(this.document, parent, get, xmlHeader, attributes, declaration, unsafeCode, generated);
            this.ParseStatementContainer(element, true, unsafeCode);
            return element;
        }

        private ICollection<Parameter> ParseAnonymousMethodParameterList(bool unsafeCode)
        {
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
            List<Parameter> list = new List<Parameter>();
            Symbol nextSymbol = this.GetNextSymbol();
            while (nextSymbol.SymbolType != SymbolType.CloseParenthesis)
            {
                Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
                ParameterModifiers none = ParameterModifiers.None;
                if (nextSymbol.SymbolType == SymbolType.Ref)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Ref, SymbolType.Ref));
                    none |= ParameterModifiers.Ref;
                }
                else if (nextSymbol.SymbolType == SymbolType.Out)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Out, SymbolType.Out));
                    none |= ParameterModifiers.Out;
                }
                else if (nextSymbol.SymbolType == SymbolType.Params)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Params, SymbolType.Params));
                    none |= ParameterModifiers.Params;
                }
                TypeToken typeToken = this.GetTypeToken(unsafeCode, true);
                int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(1);
                if (nextCodeSymbolIndex == -1)
                {
                    throw this.CreateSyntaxException();
                }
                nextSymbol = this.symbols.Peek(nextCodeSymbolIndex);
                if ((nextSymbol.SymbolType == SymbolType.Comma) || (nextSymbol.SymbolType == SymbolType.CloseParenthesis))
                {
                    if (typeToken.ChildTokens.Count > 1)
                    {
                        throw this.CreateSyntaxException();
                    }
                    CsToken token2 = typeToken.ChildTokens.First.Value;
                    this.tokens.Add(token2);
                    list.Add(new Parameter(null, token2.Text, none, token2.Location, new CsTokenList(this.tokens, last.Next, this.tokens.Last), token2.Generated));
                }
                else
                {
                    this.tokens.Add(typeToken);
                    CsToken token = this.GetToken(CsTokenType.Other, SymbolType.Other);
                    this.tokens.Add(token);
                    list.Add(new Parameter(typeToken, token.Text, none, CodeLocation.Join(typeToken.Location, token.Location), new CsTokenList(this.tokens, last.Next, this.tokens.Last), typeToken.Generated || token.Generated));
                }
                nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                    nextSymbol = this.GetNextSymbol();
                }
            }
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node;
            return list;
        }

        internal Microsoft.StyleCop.CSharp.Attribute ParseAttribute(bool unsafeCode)
        {
            Symbol symbol = this.symbols.Peek(1);
            List<AttributeExpression> list = new List<AttributeExpression>();
            Bracket item = new Bracket(symbol.Text, CsTokenType.OpenAttributeBracket, symbol.Location, this.symbols.Generated);
            Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(item);
            this.symbols.Advance();
        Label_004A:
            this.AdvanceToNextCodeSymbol();
            Symbol symbol2 = this.symbols.Peek(1);
            if (symbol2 == null)
            {
                throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
            }
            if (symbol2.SymbolType == SymbolType.CloseSquareBracket)
            {
                Bracket bracket2 = new Bracket(symbol2.Text, CsTokenType.CloseAttributeBracket, symbol2.Location, this.symbols.Generated);
                Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracket2);
                this.symbols.Advance();
                item.MatchingBracketNode = node2;
                bracket2.MatchingBracketNode = node;
            }
            else
            {
                LiteralExpression target = null;
                this.AdvanceToNextCodeSymbol();
                symbol2 = this.symbols.Peek(1);
                if (symbol2 == null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                }
                Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
                if ((symbol2.SymbolType == SymbolType.Other) || (symbol2.SymbolType == SymbolType.Return))
                {
                    int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(2);
                    if (nextCodeSymbolIndex != -1)
                    {
                        Symbol symbol3 = this.symbols.Peek(nextCodeSymbolIndex);
                        if (symbol3.SymbolType == SymbolType.Colon)
                        {
                            symbol2.SymbolType = SymbolType.Other;
                            target = this.GetLiteralExpression(unsafeCode);
                            this.AdvanceToNextCodeSymbol();
                            this.tokens.Add(new CsToken(symbol3.Text, CsTokenType.AttributeColon, symbol2.Location, this.symbols.Generated));
                            this.symbols.Advance();
                            this.AdvanceToNextCodeSymbol();
                        }
                    }
                }
                Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                if (nextExpression == null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                }
                list.Add(new AttributeExpression(new CsTokenList(this.tokens, last.Next, this.tokens.Last), target, nextExpression));
                this.AdvanceToNextCodeSymbol();
                symbol2 = this.symbols.Peek(1);
                if (symbol2 == null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                }
                if (symbol2.SymbolType != SymbolType.Comma)
                {
                    if (symbol2.SymbolType != SymbolType.CloseSquareBracket)
                    {
                        throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                    }
                }
                else
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                }
                goto Label_004A;
            }
            return new Microsoft.StyleCop.CSharp.Attribute(this.tokens, CodeLocation.Join<CsToken>(this.tokens.First, this.tokens.Last), list.ToArray(), this.tokens.First.Value.Generated || this.tokens.Last.Value.Generated);
        }

        private BlockStatement ParseBlockStatement(bool unsafeCode)
        {
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(bracketToken);
            BlockStatement parent = new BlockStatement();
            Microsoft.StyleCop.Node<CsToken> node2 = this.ParseStatementScope(parent, unsafeCode);
            if (node2 == null)
            {
                throw this.CreateSyntaxException();
            }
            bracketToken.MatchingBracketNode = node2;
            ((Bracket) node2.Value).MatchingBracketNode = firstItemNode;
            CsTokenList list = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            parent.Tokens = list;
            return parent;
        }

        private BreakStatement ParseBreakStatement()
        {
            CsToken item = this.GetToken(CsTokenType.Break, SymbolType.Break);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new BreakStatement(new CsTokenList(this.tokens, firstItemNode, firstItemNode));
        }

        private CheckedStatement ParseCheckedStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Checked, SymbolType.Checked);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            BlockStatement nextStatement = this.GetNextStatement(unsafeCode) as BlockStatement;
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            return new CheckedStatement(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextStatement);
        }

        private ClassBase ParseClass(ElementType elementType, CsElement parent, Dictionary<string, List<CsElement>> partialElements, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType accessModifier = AccessModifierType.Internal;
            if (parent.ElementType == ElementType.Class)
            {
                accessModifier = AccessModifierType.Private;
            }
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref accessModifier, ClassModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            CsTokenType tokenType = CsTokenType.Class;
            SymbolType symbolType = SymbolType.Class;
            if (elementType == ElementType.Struct)
            {
                tokenType = CsTokenType.Struct;
                symbolType = SymbolType.Struct;
            }
            else if (elementType == ElementType.Interface)
            {
                tokenType = CsTokenType.Interface;
                symbolType = SymbolType.Interface;
            }
            this.tokens.Add(this.GetToken(tokenType, symbolType));
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            if (this.GetNextSymbol().SymbolType == SymbolType.Colon)
            {
                this.tokens.Add(this.GetToken(CsTokenType.BaseColon, SymbolType.Colon));
                while (true)
                {
                    this.tokens.Add(this.GetTypeToken(unsafeCode, false));
                    if (this.GetNextSymbol().SymbolType != SymbolType.Comma)
                    {
                        break;
                    }
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                }
            }
            ICollection<TypeParameterConstraintClause> typeConstraints = null;
            if (this.GetNextSymbol().Text == "where")
            {
                typeConstraints = this.ParseTypeConstraintClauses(unsafeCode);
            }
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, elementType, accessModifier, elementModifiers);
            ClassBase element = null;
            switch (tokenType)
            {
                case CsTokenType.Class:
                    element = new Class(this.document, parent, xmlHeader, attributes, declaration, typeConstraints, unsafeCode, generated);
                    break;

                case CsTokenType.Struct:
                    element = new Struct(this.document, parent, xmlHeader, attributes, declaration, typeConstraints, unsafeCode, generated);
                    break;

                default:
                    element = new Interface(this.document, parent, xmlHeader, attributes, declaration, typeConstraints, unsafeCode, generated);
                    break;
            }
            this.ParseElementContainer(element, partialElements, unsafeCode);
            return element;
        }

        private Constructor ParseConstructor(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, ConstructorModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            IList<Parameter> parameters = this.ParseParameterList(unsafeCode, SymbolType.OpenParenthesis);
            MethodInvocationExpression initializerExpression = null;
            if (this.GetNextSymbol().SymbolType == SymbolType.Colon)
            {
                this.tokens.Add(this.GetToken(CsTokenType.BaseColon, SymbolType.Colon));
                Symbol nextSymbol = this.GetNextSymbol();
                if ((nextSymbol.SymbolType != SymbolType.This) && (nextSymbol.SymbolType != SymbolType.Base))
                {
                    throw this.CreateSyntaxException();
                }
                Microsoft.StyleCop.Node<CsToken> tokenNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Other, nextSymbol.SymbolType));
                LiteralExpression methodName = new LiteralExpression(this.tokens, tokenNode);
                initializerExpression = this.GetMethodInvocationExpression(methodName, ExpressionPrecedence.None, unsafeCode);
            }
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, ElementType.Constructor, @private, elementModifiers);
            Constructor element = new Constructor(this.document, parent, xmlHeader, attributes, declaration, parameters, initializerExpression, unsafeCode, generated);
            if (elementModifiers.ContainsKey(CsTokenType.Extern))
            {
                this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
                return element;
            }
            this.ParseStatementContainer(element, true, unsafeCode);
            return element;
        }

        private ContinueStatement ParseContinueStatement()
        {
            CsToken item = this.GetToken(CsTokenType.Continue, SymbolType.Continue);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new ContinueStatement(new CsTokenList(this.tokens, firstItemNode, firstItemNode));
        }

        private Microsoft.StyleCop.CSharp.Delegate ParseDelegate(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @public = AccessModifierType.Public;
            if ((parent.ElementType == ElementType.Class) || (parent.ElementType == ElementType.Struct))
            {
                @public = AccessModifierType.Private;
            }
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @public, DelegateModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            this.tokens.Add(this.GetToken(CsTokenType.Delegate, SymbolType.Delegate));
            TypeToken typeToken = this.GetTypeToken(unsafeCode, true);
            this.tokens.Add(typeToken);
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            IList<Parameter> parameters = this.ParseParameterList(unsafeCode, SymbolType.OpenParenthesis);
            ICollection<TypeParameterConstraintClause> typeConstraints = null;
            if (this.GetNextSymbol().Text == "where")
            {
                typeConstraints = this.ParseTypeConstraintClauses(unsafeCode);
            }
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, ElementType.Delegate, @public, elementModifiers);
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new Microsoft.StyleCop.CSharp.Delegate(this.document, parent, xmlHeader, attributes, declaration, typeToken, parameters, typeConstraints, unsafeCode, generated);
        }

        private Destructor ParseDestructor(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, DestructorModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            this.tokens.Add(this.GetToken(CsTokenType.DestructorTilde, SymbolType.Tilde));
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            string name = "~" + elementNameToken.Text;
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Bracket item = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node2;
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, name, ElementType.Destructor, @private, elementModifiers);
            Destructor element = new Destructor(this.document, parent, xmlHeader, attributes, declaration, unsafeCode, generated);
            if (elementModifiers.ContainsKey(CsTokenType.Extern))
            {
                this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
                return element;
            }
            this.ParseStatementContainer(element, true, unsafeCode);
            return element;
        }

        internal void ParseDocument()
        {
            List<Symbol> symbols = this.lexer.GetSymbols(this.lexer.SourceCode, this.lexer.SourceCode.Project.Configuration);
            this.symbols = new SymbolManager(symbols);
            this.document = new CsDocument(this.lexer.SourceCode, this.parser, this.tokens);
            FileHeader fileHeader = this.GetFileHeader();
            if (fileHeader.Generated)
            {
                this.symbols.IncrementGeneratedCodeBlocks();
            }
            this.document.FileHeader = fileHeader;
            Declaration declaration = new Declaration(new CsTokenList(this.document.Tokens), Microsoft.StyleCop.CSharp.Strings.Root, ElementType.Root, AccessModifierType.Public, new Dictionary<CsTokenType, CsToken>());
            DocumentRoot element = new DocumentRoot(this.document, declaration, fileHeader.Generated);
            this.ParseElementContainerBody(element, this.parser.PartialElements, false);
            if (this.document.Tokens.Count > 0)
            {
                element.Tokens = new CsTokenList(this.document.Tokens, this.document.Tokens.First, this.document.Tokens.Last);
                element.Location = CodeLocation.Join<CsToken>(this.document.Tokens.First, this.document.Tokens.Last);
            }
            this.document.RootElement = element;
        }

        private DoWhileStatement ParseDoWhileStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Do, SymbolType.Do);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if ((nextStatement == null) || (nextStatement.Tokens.First == null))
            {
                throw new SyntaxException(this.document.SourceCode, item.LineNumber);
            }
            this.tokens.Add(this.GetToken(CsTokenType.WhileDo, SymbolType.While));
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new DoWhileStatement(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression, nextStatement);
        }

        private CsElement ParseElement(ElementType elementType, CsElement parent, Dictionary<string, List<CsElement>> partialElements, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            switch (elementType)
            {
                case ElementType.ExternAliasDirective:
                    return this.ParseExternAliasDirective(parent, generated);

                case ElementType.UsingDirective:
                    return this.ParseUsingDirective(parent, unsafeCode, generated);

                case ElementType.Namespace:
                    return this.ParseNamespace(parent, partialElements, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Field:
                    return this.ParseField(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Constructor:
                    return this.ParseConstructor(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Destructor:
                    return this.ParseDestructor(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Delegate:
                    return this.ParseDelegate(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Event:
                    return this.ParseEvent(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Enum:
                    return this.ParseEnum(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Interface:
                case ElementType.Struct:
                case ElementType.Class:
                    return this.ParseClass(elementType, parent, partialElements, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Property:
                    return this.ParseProperty(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Accessor:
                    return this.ParseAccessor(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Indexer:
                    return this.ParseIndexer(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.Method:
                    return this.ParseMethod(parent, unsafeCode, generated, xmlHeader, attributes);

                case ElementType.EmptyElement:
                    return this.ParseEmptyElement(parent, unsafeCode, generated);
            }
            throw new StyleCopException();
        }

        private void ParseElementContainer(CsElement element, Dictionary<string, List<CsElement>> partialElements, bool unsafeCode)
        {
            if (!unsafeCode)
            {
                unsafeCode = element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Unsafe });
            }
            if (this.GetNextSymbol().SymbolType != SymbolType.OpenCurlyBracket)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
            Microsoft.StyleCop.Node<CsToken> node2 = this.ParseElementContainerBody(element, partialElements, unsafeCode);
            if (node2 == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = (Bracket) node2.Value;
            bracketToken.MatchingBracketNode = node2;
            bracket2.MatchingBracketNode = node;
        }

        private Microsoft.StyleCop.Node<CsToken> ParseElementContainerBody(CsElement element, Dictionary<string, List<CsElement>> partialElements, bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> node = null;
            while (true)
            {
                ICollection<Microsoft.StyleCop.CSharp.Attribute> is2;
                XmlHeader xmlHeader = null;
                this.MoveToElementDeclaration(unsafeCode, out xmlHeader, out is2);
                bool generated = this.symbols.Generated;
                Symbol symbol = this.symbols.Peek(1);
                if (symbol == null)
                {
                    return node;
                }
                if (symbol.SymbolType == SymbolType.CloseCurlyBracket)
                {
                    Bracket bracketToken = this.GetBracketToken(CsTokenType.CloseCurlyBracket, SymbolType.CloseCurlyBracket);
                    return this.tokens.InsertLast(bracketToken);
                }
                ElementType? elementType = this.GetElementType(element, unsafeCode);
                if (!elementType.HasValue || !SanityCheckElementTypeAgainstParent(elementType.Value, element))
                {
                    throw this.CreateSyntaxException();
                }
                CsElement element2 = this.ParseElement(elementType.Value, element, partialElements, unsafeCode, generated, xmlHeader, is2);
                this.AddRuleSuppressionsForElement(element2);
                element.AddElement(element2);
                this.InitializeElement(element2);
                AddElementToPartialElementsList(element2, partialElements);
            }
        }

        private EmptyElement ParseEmptyElement(CsElement parent, bool unsafeCode, bool generated)
        {
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            CsTokenList tokens = new CsTokenList(this.tokens, this.tokens.Last, this.tokens.Last);
            return new EmptyElement(this.document, parent, new Declaration(tokens, string.Empty, ElementType.EmptyElement, AccessModifierType.Public), unsafeCode, generated);
        }

        private Microsoft.StyleCop.CSharp.Enum ParseEnum(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @public = AccessModifierType.Public;
            if ((parent.ElementType == ElementType.Class) || (parent.ElementType == ElementType.Struct))
            {
                @public = AccessModifierType.Private;
            }
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @public, EnumModifiers);
            this.tokens.Add(this.GetToken(CsTokenType.Enum, SymbolType.Enum));
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            if (this.GetNextSymbol().SymbolType == SymbolType.Colon)
            {
                this.tokens.Add(this.GetToken(CsTokenType.BaseColon, SymbolType.Colon));
                this.tokens.Add(this.GetTypeToken(unsafeCode, false));
            }
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, ElementType.Enum, @public, elementModifiers);
            Microsoft.StyleCop.CSharp.Enum enum2 = new Microsoft.StyleCop.CSharp.Enum(this.document, parent, xmlHeader, attributes, declaration, unsafeCode, generated);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracketToken);
            enum2.Items = this.ParseEnumItems(enum2, unsafeCode);
            Bracket item = this.GetBracketToken(CsTokenType.CloseCurlyBracket, SymbolType.CloseCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> node4 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node4;
            item.MatchingBracketNode = node3;
            return enum2;
        }

        private ICollection<EnumItem> ParseEnumItems(Microsoft.StyleCop.CSharp.Enum parent, bool unsafeCode)
        {
            List<EnumItem> list = new List<EnumItem>();
            SkipSymbols all = SkipSymbols.All;
            all &= ~SkipSymbols.XmlHeader;
            for (Symbol symbol = this.GetNextSymbol(all); symbol.SymbolType != SymbolType.CloseCurlyBracket; symbol = this.GetNextSymbol(all))
            {
                ICollection<Microsoft.StyleCop.CSharp.Attribute> is2;
                XmlHeader xmlHeader = null;
                this.MoveToElementDeclaration(unsafeCode, out xmlHeader, out is2);
                if (this.GetNextSymbol().SymbolType == SymbolType.CloseCurlyBracket)
                {
                    break;
                }
                Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Other, SymbolType.Other));
                Expression initialization = null;
                if (this.GetNextSymbol().SymbolType == SymbolType.Equals)
                {
                    this.tokens.Add(this.GetOperatorToken(OperatorType.Equals));
                    initialization = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                }
                CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
                Declaration declaration = new Declaration(tokens, firstItemNode.Value.Text, ElementType.EnumItem, AccessModifierType.Public);
                EnumItem item = new EnumItem(this.document, parent, xmlHeader, is2, declaration, initialization, unsafeCode, this.symbols.Generated);
                item.Tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
                list.Add(item);
                parent.AddElement(item);
                if (this.GetNextSymbol().SymbolType != SymbolType.Comma)
                {
                    break;
                }
                this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
            }
            return list.ToArray();
        }

        private Event ParseEvent(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Interface interface2 = parent as Interface;
            if (interface2 != null)
            {
                @private = interface2.AccessModifier;
            }
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, EventModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            this.tokens.Add(this.GetToken(CsTokenType.Event, SymbolType.Event));
            TypeToken typeToken = this.GetTypeToken(unsafeCode, true);
            this.tokens.Add(typeToken);
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, ElementType.Event, @private, elementModifiers);
            Event element = new Event(this.document, parent, xmlHeader, attributes, declaration, typeToken, unsafeCode, generated);
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol.SymbolType == SymbolType.Equals)
            {
                this.tokens.Add(this.GetToken(CsTokenType.Equals, SymbolType.Equals));
                element.InitializationExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
                return element;
            }
            if (nextSymbol.SymbolType == SymbolType.Semicolon)
            {
                this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
                return element;
            }
            this.ParseElementContainer(element, null, unsafeCode);
            return element;
        }

        private ExpressionStatement ParseExpressionStatement(bool unsafeCode)
        {
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if ((nextExpression == null) || (nextExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new ExpressionStatement(new CsTokenList(this.tokens, nextExpression.Tokens.First, this.tokens.Last), nextExpression);
        }

        private ExternAliasDirective ParseExternAliasDirective(CsElement parent, bool generated)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Extern, SymbolType.Extern));
            this.tokens.Add(this.GetToken(CsTokenType.Alias, SymbolType.Other));
            CsToken item = this.GetToken(CsTokenType.Other, SymbolType.Other);
            this.tokens.Add(item);
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, item.Text, ElementType.ExternAliasDirective, AccessModifierType.Public);
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new ExternAliasDirective(this.document, parent, declaration, generated);
        }

        private Field ParseField(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, FieldModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            TypeToken typeToken = this.GetTypeToken(unsafeCode, true);
            Microsoft.StyleCop.Node<CsToken> tokenNode = this.tokens.InsertLast(typeToken);
            IList<VariableDeclaratorExpression> declarators = this.ParseFieldDeclarators(unsafeCode, typeToken);
            if (declarators.Count == 0)
            {
                throw this.CreateSyntaxException();
            }
            VariableDeclarationExpression expression = new VariableDeclarationExpression(new CsTokenList(this.tokens, declarators[0].Tokens.First, this.tokens.Last), new LiteralExpression(this.tokens, tokenNode), declarators);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, declarators[0].Identifier.Text, ElementType.Field, @private, elementModifiers);
            Field field = new Field(this.document, parent, xmlHeader, attributes, declaration, typeToken, unsafeCode, generated);
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            field.VariableDeclarationStatement = new VariableDeclarationStatement(new CsTokenList(this.tokens, declarators[0].Tokens.First, this.tokens.Last), field.Const, expression);
            return field;
        }

        private IList<VariableDeclaratorExpression> ParseFieldDeclarators(bool unsafeCode, TypeToken fieldType)
        {
            List<VariableDeclaratorExpression> list = new List<VariableDeclaratorExpression>();
            Symbol nextSymbol = this.GetNextSymbol();
            while (nextSymbol.SymbolType != SymbolType.Semicolon)
            {
                CsToken elementNameToken = this.GetElementNameToken(unsafeCode, true);
                Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(elementNameToken);
                Expression initializer = null;
                if (this.GetNextSymbol().SymbolType == SymbolType.Equals)
                {
                    this.tokens.Add(this.GetOperatorToken(OperatorType.Equals));
                    if (this.GetNextSymbol().SymbolType == SymbolType.OpenCurlyBracket)
                    {
                        if ((fieldType.Text == "var") || (((fieldType.Text != "Array") && (fieldType.Text != "System.Array")) && !fieldType.Text.Contains("[")))
                        {
                            initializer = this.GetAnonymousTypeInitializerExpression(unsafeCode);
                        }
                        else
                        {
                            initializer = this.GetArrayInitializerExpression(unsafeCode);
                        }
                    }
                    else
                    {
                        initializer = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                    }
                    if (initializer == null)
                    {
                        throw this.CreateSyntaxException();
                    }
                }
                list.Add(new VariableDeclaratorExpression(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), new LiteralExpression(this.tokens, firstItemNode), initializer));
                nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                    nextSymbol = this.GetNextSymbol();
                }
            }
            return list.ToArray();
        }

        private FixedStatement ParseFixedStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Fixed, SymbolType.Fixed);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            VariableDeclarationExpression fixedVariable = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode, true, false) as VariableDeclarationExpression;
            if (fixedVariable == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            FixedStatement statement2 = new FixedStatement(tokens, fixedVariable);
            statement2.EmbeddedStatement = nextStatement;
            foreach (VariableDeclaratorExpression expression2 in fixedVariable.Declarators)
            {
                Variable variable = new Variable(fixedVariable.Type, expression2.Identifier.Token.Text, VariableModifiers.None, expression2.Tokens.First.Value.Location.StartPoint, fixedVariable.Type.Generated || expression2.Identifier.Token.Generated);
                if (!statement2.Variables.Contains(expression2.Identifier.Token.Text))
                {
                    statement2.Variables.Add(variable);
                }
            }
            return statement2;
        }

        private ForeachStatement ParseForeachStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Foreach, SymbolType.Foreach);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            VariableDeclarationExpression expression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode, true, false) as VariableDeclarationExpression;
            if (expression == null)
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(this.GetToken(CsTokenType.In, SymbolType.In));
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            ForeachStatement statement2 = new ForeachStatement(tokens, expression, nextExpression);
            statement2.EmbeddedStatement = nextStatement;
            foreach (VariableDeclaratorExpression expression3 in expression.Declarators)
            {
                Variable variable = new Variable(expression.Type, expression3.Identifier.Token.Text, VariableModifiers.None, expression3.Tokens.First.Value.Location.StartPoint, expression.Type.Generated);
                if (!statement2.Variables.Contains(expression3.Identifier.Token.Text))
                {
                    statement2.Variables.Add(variable);
                }
            }
            return statement2;
        }

        private ForStatement ParseForStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.For, SymbolType.For);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> openParenthesisNode = this.tokens.InsertLast(bracketToken);
            List<Expression> list = this.ParseForStatementInitializers(unsafeCode);
            Expression condition = this.ParseForStatementCondition(unsafeCode);
            List<Expression> list2 = this.ParseForStatementIterators(unsafeCode, bracketToken, openParenthesisNode);
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if ((nextStatement == null) || (nextStatement.Tokens.First == null))
            {
                throw new SyntaxException(this.document.SourceCode, item.LineNumber);
            }
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            ForStatement statement2 = new ForStatement(tokens, list.ToArray(), condition, list2.ToArray());
            statement2.EmbeddedStatement = nextStatement;
            foreach (Expression expression2 in list)
            {
                VariableDeclarationExpression expression3 = expression2 as VariableDeclarationExpression;
                if (expression3 != null)
                {
                    foreach (VariableDeclaratorExpression expression4 in expression3.Declarators)
                    {
                        Variable variable = new Variable(expression3.Type, expression4.Identifier.Token.Text, VariableModifiers.None, expression4.Tokens.First.Value.Location.StartPoint, expression3.Type.Generated || expression4.Identifier.Token.Generated);
                        if (!statement2.Variables.Contains(expression4.Identifier.Token.Text))
                        {
                            statement2.Variables.Add(variable);
                        }
                    }
                    continue;
                }
            }
            return statement2;
        }

        private Expression ParseForStatementCondition(bool unsafeCode)
        {
            Symbol nextSymbol = this.GetNextSymbol();
            Expression nextExpression = null;
            if (nextSymbol.SymbolType != SymbolType.Semicolon)
            {
                nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                if ((nextExpression == null) || (nextExpression.Tokens.First == null))
                {
                    throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                }
                nextSymbol = this.GetNextSymbol();
            }
            if (nextSymbol.SymbolType != SymbolType.Semicolon)
            {
                throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
            }
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return nextExpression;
        }

        private List<Expression> ParseForStatementInitializers(bool unsafeCode)
        {
            List<Expression> list = new List<Expression>();
            while (true)
            {
                Symbol nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType == SymbolType.Semicolon)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
                    return list;
                }
                Expression item = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode, true, false);
                if ((item == null) || (item.Tokens.First == null))
                {
                    throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                }
                list.Add(item);
                nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType != SymbolType.Comma)
                {
                    if (nextSymbol.SymbolType != SymbolType.Semicolon)
                    {
                        throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                    }
                }
                else
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                }
            }
        }

        private List<Expression> ParseForStatementIterators(bool unsafeCode, Bracket openParenthesis, Microsoft.StyleCop.Node<CsToken> openParenthesisNode)
        {
            List<Expression> list = new List<Expression>();
            while (true)
            {
                Symbol nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType == SymbolType.CloseParenthesis)
                {
                    Bracket bracketToken = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
                    Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
                    openParenthesis.MatchingBracketNode = node;
                    bracketToken.MatchingBracketNode = openParenthesisNode;
                    return list;
                }
                Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                if ((nextExpression == null) || (nextExpression.Tokens.First == null))
                {
                    throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                }
                list.Add(nextExpression);
                nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType != SymbolType.Comma)
                {
                    if (nextSymbol.SymbolType != SymbolType.CloseParenthesis)
                    {
                        throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                    }
                }
                else
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                }
            }
        }

        private GotoStatement ParseGotoStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Goto, SymbolType.Goto);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Symbol nextSymbol = this.GetNextSymbol();
            Expression identifier = null;
            if (nextSymbol.SymbolType == SymbolType.Default)
            {
                Microsoft.StyleCop.Node<CsToken> tokenNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Other, SymbolType.Default));
                identifier = new LiteralExpression(new CsTokenList(this.tokens, tokenNode, tokenNode), tokenNode);
            }
            else if (nextSymbol.SymbolType == SymbolType.Case)
            {
                this.tokens.Add(this.GetToken(CsTokenType.Other, SymbolType.Case));
                identifier = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            }
            else
            {
                identifier = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            }
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new GotoStatement(new CsTokenList(this.tokens, firstItemNode, firstItemNode), identifier);
        }

        private IfStatement ParseIfStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.If, SymbolType.If);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if (nextStatement == null)
            {
                throw new SyntaxException(this.document.SourceCode, item.LineNumber);
            }
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            IfStatement statement2 = new IfStatement(tokens, nextExpression);
            statement2.EmbeddedStatement = nextStatement;
            ElseStatement attachedElseStatement = this.GetAttachedElseStatement(unsafeCode);
            if (attachedElseStatement != null)
            {
                statement2.AttachedElseStatement = attachedElseStatement;
            }
            return statement2;
        }

        private Indexer ParseIndexer(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Interface interface2 = parent as Interface;
            if (interface2 != null)
            {
                @private = interface2.AccessModifier;
            }
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, IndexerModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            TypeToken typeToken = this.GetTypeToken(unsafeCode, true);
            this.tokens.Add(typeToken);
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            IList<Parameter> parameters = this.ParseParameterList(unsafeCode, SymbolType.OpenSquareBracket);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, ElementType.Indexer, @private, elementModifiers);
            Indexer element = new Indexer(this.document, parent, xmlHeader, attributes, declaration, typeToken, parameters, unsafeCode, generated);
            this.ParseElementContainer(element, null, unsafeCode);
            return element;
        }

        private LabelStatement ParseLabelStatement(bool unsafeCode)
        {
            this.GetNextSymbol(SymbolType.Other);
            LiteralExpression literalExpression = this.GetLiteralExpression(unsafeCode);
            if ((literalExpression == null) || (literalExpression.Tokens.First == null))
            {
                throw this.CreateSyntaxException();
            }
            this.tokens.Add(this.GetToken(CsTokenType.LabelColon, SymbolType.Colon));
            return new LabelStatement(new CsTokenList(this.tokens, literalExpression.Tokens.First, this.tokens.Last), literalExpression);
        }

        private LockStatement ParseLockStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Lock, SymbolType.Lock);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            LockStatement statement2 = new LockStatement(tokens, nextExpression);
            statement2.EmbeddedStatement = nextStatement;
            VariableDeclarationExpression expression2 = nextExpression as VariableDeclarationExpression;
            if (expression2 != null)
            {
                foreach (VariableDeclaratorExpression expression3 in expression2.Declarators)
                {
                    Variable variable = new Variable(expression2.Type, expression3.Identifier.Token.Text, VariableModifiers.None, expression3.Tokens.First.Value.Location.StartPoint, expression2.Type.Generated || expression3.Identifier.Token.Generated);
                    if (!statement2.Variables.Contains(expression3.Identifier.Token.Text))
                    {
                        statement2.Variables.Add(variable);
                    }
                }
            }
            return statement2;
        }

        private Method ParseMethod(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Interface interface2 = parent as Interface;
            if (interface2 != null)
            {
                @private = interface2.AccessModifier;
            }
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, MethodModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            TypeToken item = null;
            if (!elementModifiers.ContainsKey(CsTokenType.Implicit) && !elementModifiers.ContainsKey(CsTokenType.Explicit))
            {
                item = this.GetTypeToken(unsafeCode, true);
                this.tokens.Add(item);
            }
            string name = null;
            if (this.GetNextSymbol().SymbolType == SymbolType.Operator)
            {
                this.tokens.Add(this.GetToken(CsTokenType.Operator, SymbolType.Operator));
                this.AdvanceToNextCodeSymbol();
                int endIndex = -1;
                CsToken typeToken = null;
                if (this.HasTypeSignature(1, unsafeCode, out endIndex))
                {
                    typeToken = this.GetTypeToken(unsafeCode, true);
                }
                else
                {
                    typeToken = this.ConvertOperatorOverloadSymbol();
                }
                this.tokens.Add(typeToken);
                name = "operator " + typeToken.Text;
            }
            else
            {
                CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
                name = elementNameToken.Text;
                this.tokens.Add(elementNameToken);
            }
            IList<Parameter> parameters = this.ParseParameterList(unsafeCode, SymbolType.OpenParenthesis, elementModifiers.ContainsKey(CsTokenType.Static));
            ICollection<TypeParameterConstraintClause> typeConstraints = null;
            if (this.GetNextSymbol().Text == "where")
            {
                typeConstraints = this.ParseTypeConstraintClauses(unsafeCode);
            }
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, name, ElementType.Method, @private, elementModifiers);
            Method element = new Method(this.document, parent, xmlHeader, attributes, declaration, item, parameters, typeConstraints, unsafeCode, generated);
            if ((elementModifiers.ContainsKey(CsTokenType.Abstract) || elementModifiers.ContainsKey(CsTokenType.Extern)) || (parent.ElementType == ElementType.Interface))
            {
                this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
                return element;
            }
            this.ParseStatementContainer(element, true, unsafeCode);
            return element;
        }

        private Namespace ParseNamespace(CsElement parent, Dictionary<string, List<CsElement>> partialElements, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Namespace, SymbolType.Namespace));
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, ElementType.Namespace, AccessModifierType.Public);
            Namespace element = new Namespace(this.document, parent, xmlHeader, attributes, declaration, unsafeCode, generated);
            this.ParseElementContainer(element, partialElements, unsafeCode);
            return element;
        }

        private Statement ParseOtherStatement(bool unsafeCode, VariableCollection variables)
        {
            int num;
            bool flag = false;
            if (this.HasTypeSignature(1, unsafeCode, out num))
            {
                int count = this.GetNextCodeSymbolIndex(num + 1);
                if ((count != -1) && (this.symbols.Peek(count).SymbolType == SymbolType.Other))
                {
                    count = this.GetNextCodeSymbolIndex(count + 1);
                    if (count != -1)
                    {
                        Symbol symbol = this.symbols.Peek(count);
                        if (((symbol.SymbolType == SymbolType.Equals) || (symbol.SymbolType == SymbolType.Semicolon)) || (symbol.SymbolType == SymbolType.Comma))
                        {
                            flag = true;
                        }
                    }
                }
            }
            if (flag)
            {
                return this.ParseVariableDeclarationStatement(unsafeCode, variables);
            }
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(2);
            if ((nextCodeSymbolIndex == -1) || (this.symbols.Peek(nextCodeSymbolIndex).SymbolType != SymbolType.Colon))
            {
                return this.ParseExpressionStatement(unsafeCode);
            }
            return this.ParseLabelStatement(unsafeCode);
        }

        private IList<Parameter> ParseParameterList(bool unsafeCode, SymbolType openingBracketType)
        {
            return this.ParseParameterList(unsafeCode, openingBracketType, false);
        }

        private IList<Parameter> ParseParameterList(bool unsafeCode, SymbolType openingBracketType, bool staticMethod)
        {
            CsTokenType openParenthesis = CsTokenType.OpenParenthesis;
            CsTokenType closeParenthesis = CsTokenType.CloseParenthesis;
            SymbolType symbolType = SymbolType.CloseParenthesis;
            if (openingBracketType == SymbolType.OpenSquareBracket)
            {
                openParenthesis = CsTokenType.OpenSquareBracket;
                closeParenthesis = CsTokenType.CloseSquareBracket;
                symbolType = SymbolType.CloseSquareBracket;
            }
            Bracket bracketToken = this.GetBracketToken(openParenthesis, openingBracketType);
            Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
            Symbol nextSymbol = this.GetNextSymbol();
            List<Parameter> list = new List<Parameter>();
            while (nextSymbol.SymbolType != symbolType)
            {
                Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
                while (nextSymbol.SymbolType == SymbolType.OpenSquareBracket)
                {
                    Microsoft.StyleCop.CSharp.Attribute attribute = this.GetAttribute(unsafeCode);
                    if (attribute == null)
                    {
                        throw this.CreateSyntaxException();
                    }
                    this.tokens.Add(attribute);
                    nextSymbol = this.GetNextSymbol();
                }
                ParameterModifiers none = ParameterModifiers.None;
                if (nextSymbol.SymbolType == SymbolType.Ref)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Ref, SymbolType.Ref));
                    none |= ParameterModifiers.Ref;
                }
                else if (nextSymbol.SymbolType == SymbolType.Out)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Out, SymbolType.Out));
                    none |= ParameterModifiers.Out;
                }
                else if (nextSymbol.SymbolType == SymbolType.Params)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Params, SymbolType.Params));
                    none |= ParameterModifiers.Params;
                }
                else if (((nextSymbol.SymbolType == SymbolType.This) && (list.Count == 0)) && staticMethod)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.This, SymbolType.This));
                    none |= ParameterModifiers.This;
                }
                TypeToken typeToken = this.GetTypeToken(unsafeCode, true);
                CsToken token = null;
                if (typeToken.Text.Equals("__arglist", StringComparison.Ordinal))
                {
                    token = typeToken.ChildTokens.First.Value;
                    typeToken = null;
                    this.tokens.Add(token);
                }
                else
                {
                    this.tokens.Add(typeToken);
                    token = this.GetToken(CsTokenType.Other, SymbolType.Other);
                    this.tokens.Add(token);
                }
                list.Add(new Parameter(typeToken, token.Text, none, (typeToken == null) ? token.Location : CodeLocation.Join(typeToken.Location, token.Location), new CsTokenList(this.tokens, last.Next, this.tokens.Last), (typeToken == null) ? token.Generated : (typeToken.Generated || token.Generated)));
                nextSymbol = this.GetNextSymbol();
                if (nextSymbol.SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                    nextSymbol = this.GetNextSymbol();
                }
            }
            Bracket item = this.GetBracketToken(closeParenthesis, symbolType);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(item);
            bracketToken.MatchingBracketNode = node3;
            item.MatchingBracketNode = node;
            return list.ToArray();
        }

        private Property ParseProperty(CsElement parent, bool unsafeCode, bool generated, XmlHeader xmlHeader, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes)
        {
            Microsoft.StyleCop.Node<CsToken> last = this.tokens.Last;
            AccessModifierType @private = AccessModifierType.Private;
            Interface interface2 = parent as Interface;
            if (interface2 != null)
            {
                @private = interface2.AccessModifier;
            }
            Dictionary<CsTokenType, CsToken> elementModifiers = this.GetElementModifiers(ref @private, PropertyModifiers);
            unsafeCode |= elementModifiers.ContainsKey(CsTokenType.Unsafe);
            TypeToken typeToken = this.GetTypeToken(unsafeCode, true);
            this.tokens.Add(typeToken);
            CsToken elementNameToken = this.GetElementNameToken(unsafeCode);
            this.tokens.Add(elementNameToken);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = (last == null) ? this.tokens.First : last.Next;
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, elementNameToken.Text, ElementType.Property, @private, elementModifiers);
            Property element = new Property(this.document, parent, xmlHeader, attributes, declaration, typeToken, unsafeCode, generated);
            this.ParseElementContainer(element, null, unsafeCode);
            return element;
        }

        private ReturnStatement ParseReturnStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Return, SymbolType.Return);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Symbol nextSymbol = this.GetNextSymbol();
            Expression returnValue = null;
            if (nextSymbol.SymbolType != SymbolType.Semicolon)
            {
                returnValue = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                if (returnValue == null)
                {
                    throw this.CreateSyntaxException();
                }
            }
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new ReturnStatement(new CsTokenList(this.tokens, firstItemNode, firstItemNode), returnValue);
        }

        private void ParseStatementContainer(CsElement element, bool interfaceType, bool unsafeCode)
        {
            unsafeCode |= element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Unsafe });
            Symbol nextSymbol = this.GetNextSymbol();
            if (nextSymbol == null)
            {
                throw this.CreateSyntaxException();
            }
            if (nextSymbol.SymbolType == SymbolType.OpenCurlyBracket)
            {
                Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
                Microsoft.StyleCop.Node<CsToken> node = this.tokens.InsertLast(bracketToken);
                Microsoft.StyleCop.Node<CsToken> node2 = this.ParseStatementScope(element, unsafeCode);
                if (node2 == null)
                {
                    throw this.CreateSyntaxException();
                }
                bracketToken.MatchingBracketNode = node2;
                ((Bracket) node2.Value).MatchingBracketNode = node;
            }
            else
            {
                if (!interfaceType || (nextSymbol.SymbolType != SymbolType.Semicolon))
                {
                    throw new SyntaxException(this.document.SourceCode, nextSymbol.LineNumber);
                }
                this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            }
        }

        private Microsoft.StyleCop.Node<CsToken> ParseStatementScope(IWriteableCodeUnit parent, bool unsafeCode)
        {
            Microsoft.StyleCop.Node<CsToken> node = null;
            while (true)
            {
                Statement nextStatement;
                do
                {
                    Symbol nextSymbol = this.GetNextSymbol();
                    if (nextSymbol == null)
                    {
                        return node;
                    }
                    if (nextSymbol.SymbolType == SymbolType.CloseCurlyBracket)
                    {
                        Bracket bracketToken = this.GetBracketToken(CsTokenType.CloseCurlyBracket, SymbolType.CloseCurlyBracket);
                        return this.tokens.InsertLast(bracketToken);
                    }
                    nextStatement = this.GetNextStatement(unsafeCode, parent.Variables);
                }
                while (nextStatement == null);
                parent.AddStatement(nextStatement);
                foreach (Statement statement2 in nextStatement.AttachedStatements)
                {
                    parent.AddStatement(statement2);
                }
            }
        }

        private SwitchCaseStatement ParseSwitchCaseStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Case, SymbolType.Case);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Symbol nextSymbol = this.GetNextSymbol();
            if (((((nextSymbol.SymbolType != SymbolType.Other) && (nextSymbol.SymbolType != SymbolType.String)) && ((nextSymbol.SymbolType != SymbolType.Number) && (nextSymbol.SymbolType != SymbolType.Null))) && (((nextSymbol.SymbolType != SymbolType.OpenParenthesis) && (nextSymbol.SymbolType != SymbolType.Minus)) && ((nextSymbol.SymbolType != SymbolType.Plus) && (nextSymbol.SymbolType != SymbolType.True)))) && (nextSymbol.SymbolType != SymbolType.False))
            {
                throw this.CreateSyntaxException();
            }
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            this.tokens.Add(this.GetToken(CsTokenType.LabelColon, SymbolType.Colon));
            SwitchCaseStatement statement = new SwitchCaseStatement(nextExpression);
            while (true)
            {
                nextSymbol = this.GetNextSymbol();
                if (((nextSymbol.SymbolType == SymbolType.CloseCurlyBracket) || (nextSymbol.SymbolType == SymbolType.Case)) || (nextSymbol.SymbolType == SymbolType.Default))
                {
                    break;
                }
                Statement nextStatement = this.GetNextStatement(unsafeCode, statement.Variables);
                if (nextStatement == null)
                {
                    throw this.CreateSyntaxException();
                }
                statement.AddStatement(nextStatement);
            }
            statement.Tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            return statement;
        }

        private SwitchDefaultStatement ParseSwitchDefaultStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Default, SymbolType.Default);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            this.tokens.Add(this.GetToken(CsTokenType.LabelColon, SymbolType.Colon));
            SwitchDefaultStatement statement = new SwitchDefaultStatement();
            while (true)
            {
                Symbol nextSymbol = this.GetNextSymbol();
                if (((nextSymbol.SymbolType == SymbolType.CloseCurlyBracket) || (nextSymbol.SymbolType == SymbolType.Case)) || (nextSymbol.SymbolType == SymbolType.Default))
                {
                    break;
                }
                Statement nextStatement = this.GetNextStatement(unsafeCode, statement.Variables);
                if (nextStatement == null)
                {
                    throw this.CreateSyntaxException();
                }
                statement.AddStatement(nextStatement);
            }
            statement.Tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            return statement;
        }

        private SwitchStatement ParseSwitchStatement(bool unsafeCode)
        {
            SwitchDefaultStatement statement;
            CsToken item = this.GetToken(CsTokenType.Switch, SymbolType.Switch);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            Bracket bracket3 = this.GetBracketToken(CsTokenType.OpenCurlyBracket, SymbolType.OpenCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> node4 = this.tokens.InsertLast(bracket3);
            List<SwitchCaseStatement> list = this.ParseSwitchStatementCaseStatements(unsafeCode, out statement);
            Bracket bracket4 = this.GetBracketToken(CsTokenType.CloseCurlyBracket, SymbolType.CloseCurlyBracket);
            Microsoft.StyleCop.Node<CsToken> node5 = this.tokens.InsertLast(bracket4);
            bracket3.MatchingBracketNode = node5;
            bracket4.MatchingBracketNode = node4;
            return new SwitchStatement(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextExpression, list.ToArray(), statement);
        }

        private List<SwitchCaseStatement> ParseSwitchStatementCaseStatements(bool unsafeCode, out SwitchDefaultStatement defaultStatement)
        {
            Symbol symbol;
            defaultStatement = null;
            List<SwitchCaseStatement> list = new List<SwitchCaseStatement>();
        Label_0009:
            symbol = this.GetNextSymbol();
            if (symbol.SymbolType == SymbolType.Case)
            {
                list.Add(this.ParseSwitchCaseStatement(unsafeCode));
                goto Label_0009;
            }
            if (symbol.SymbolType == SymbolType.Default)
            {
                if (defaultStatement != null)
                {
                    throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
                }
                defaultStatement = this.ParseSwitchDefaultStatement(unsafeCode);
                goto Label_0009;
            }
            if (symbol.SymbolType != SymbolType.CloseCurlyBracket)
            {
                throw new SyntaxException(this.document.SourceCode, symbol.LineNumber);
            }
            return list;
        }

        private ThrowStatement ParseThrowStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Throw, SymbolType.Throw);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Symbol nextSymbol = this.GetNextSymbol();
            Expression thrownExpression = null;
            if (nextSymbol.SymbolType != SymbolType.Semicolon)
            {
                thrownExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                if (thrownExpression == null)
                {
                    throw this.CreateSyntaxException();
                }
            }
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new ThrowStatement(new CsTokenList(this.tokens, firstItemNode, firstItemNode), thrownExpression);
        }

        private TryStatement ParseTryStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Try, SymbolType.Try);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            BlockStatement nextStatement = this.GetNextStatement(unsafeCode) as BlockStatement;
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            TryStatement tryStatement = new TryStatement(nextStatement);
            List<CatchStatement> list = new List<CatchStatement>();
            while (true)
            {
                CatchStatement attachedCatchStatement = this.GetAttachedCatchStatement(tryStatement, unsafeCode);
                if (attachedCatchStatement == null)
                {
                    break;
                }
                list.Add(attachedCatchStatement);
            }
            FinallyStatement attachedFinallyStatement = this.GetAttachedFinallyStatement(tryStatement, unsafeCode);
            tryStatement.Tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            tryStatement.CatchStatements = list.ToArray();
            tryStatement.FinallyStatement = attachedFinallyStatement;
            return tryStatement;
        }

        private ICollection<TypeParameterConstraintClause> ParseTypeConstraintClauses(bool unsafeCode)
        {
            List<TypeParameterConstraintClause> list = new List<TypeParameterConstraintClause>();
            for (Symbol symbol = this.GetNextSymbol(); symbol.Text == "where"; symbol = this.GetNextSymbol())
            {
                Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Other, SymbolType.Other));
                Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(this.GetToken(CsTokenType.Other, SymbolType.Other));
                this.tokens.Add(this.GetToken(CsTokenType.WhereColon, SymbolType.Colon));
                List<CsToken> list2 = new List<CsToken>();
            Label_005A:
                symbol = this.GetNextSymbol();
                CsToken item = null;
                if ((symbol.SymbolType == SymbolType.Class) || (symbol.SymbolType == SymbolType.Struct))
                {
                    item = this.GetToken(CsTokenType.Other, symbol.SymbolType);
                }
                else if (symbol.SymbolType == SymbolType.New)
                {
                    MasterList<CsToken> childTokens = new MasterList<CsToken>();
                    childTokens.Add(this.GetToken(CsTokenType.Other, SymbolType.New));
                    Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
                    Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
                    Microsoft.StyleCop.Node<CsToken> node3 = childTokens.InsertLast(bracketToken);
                    Microsoft.StyleCop.Node<CsToken> node4 = childTokens.InsertLast(bracket2);
                    bracketToken.MatchingBracketNode = node4;
                    bracket2.MatchingBracketNode = node3;
                    item = new ConstructorConstraint(childTokens, CodeLocation.Join<CsToken>(childTokens.First, childTokens.Last), childTokens.First.Value.Generated);
                }
                else
                {
                    item = this.GetTypeToken(unsafeCode, true);
                }
                this.tokens.Add(item);
                list2.Add(item);
                if (this.GetNextSymbol().SymbolType == SymbolType.Comma)
                {
                    this.tokens.Add(this.GetToken(CsTokenType.Comma, SymbolType.Comma));
                    goto Label_005A;
                }
                list.Add(new TypeParameterConstraintClause(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), node2.Value, list2.ToArray()));
            }
            return list.ToArray();
        }

        private UncheckedStatement ParseUncheckedStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Unchecked, SymbolType.Unchecked);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            BlockStatement nextStatement = this.GetNextStatement(unsafeCode) as BlockStatement;
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            return new UncheckedStatement(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextStatement);
        }

        private UnsafeStatement ParseUnsafeStatement()
        {
            CsToken item = this.GetToken(CsTokenType.Unsafe, SymbolType.Unsafe);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            BlockStatement nextStatement = this.GetNextStatement(true) as BlockStatement;
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            return new UnsafeStatement(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), nextStatement);
        }

        private UsingDirective ParseUsingDirective(CsElement parent, bool unsafeCode, bool generated)
        {
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Using, SymbolType.Using));
            Symbol nextSymbol = this.GetNextSymbol(SymbolType.Other);
            int nextCodeSymbolIndex = this.GetNextCodeSymbolIndex(2);
            if (nextCodeSymbolIndex == -1)
            {
                throw this.CreateSyntaxException();
            }
            CsToken item = null;
            if (this.symbols.Peek(nextCodeSymbolIndex).SymbolType == SymbolType.Equals)
            {
                item = this.GetToken(CsTokenType.Other, SymbolType.Other);
                this.tokens.Add(item);
                this.tokens.Add(this.GetOperatorToken(OperatorType.Equals));
            }
            TypeToken typeToken = this.GetTypeToken(unsafeCode, false);
            this.tokens.Add(typeToken);
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            Declaration declaration = new Declaration(tokens, (item == null) ? typeToken.Text : item.Text, ElementType.UsingDirective, AccessModifierType.Public);
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new UsingDirective(this.document, parent, declaration, generated, typeToken.Text, (item == null) ? null : item.Text);
        }

        private UsingStatement ParseUsingStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.Using, SymbolType.Using);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression resource = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode, true, false);
            if (resource == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if (nextStatement == null)
            {
                throw this.CreateSyntaxException();
            }
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            UsingStatement statement2 = new UsingStatement(tokens, resource);
            statement2.EmbeddedStatement = nextStatement;
            VariableDeclarationExpression expression2 = resource as VariableDeclarationExpression;
            if (expression2 != null)
            {
                foreach (VariableDeclaratorExpression expression3 in expression2.Declarators)
                {
                    Variable variable = new Variable(expression2.Type, expression3.Identifier.Token.Text, VariableModifiers.None, expression3.Tokens.First.Value.Location.StartPoint, expression2.Type.Generated || expression3.Identifier.Token.Generated);
                    if (!statement2.Variables.Contains(expression3.Identifier.Token.Text))
                    {
                        statement2.Variables.Add(variable);
                    }
                }
            }
            return statement2;
        }

        private VariableDeclarationStatement ParseVariableDeclarationStatement(bool unsafeCode, VariableCollection variables)
        {
            bool constant = false;
            Symbol nextSymbol = this.GetNextSymbol();
            CsToken token = null;
            Microsoft.StyleCop.Node<CsToken> firstItemNode = null;
            if (nextSymbol.SymbolType == SymbolType.Const)
            {
                constant = true;
                token = new CsToken(nextSymbol.Text, CsTokenType.Const, nextSymbol.Location, this.symbols.Generated);
                firstItemNode = this.tokens.InsertLast(this.GetToken(CsTokenType.Const, SymbolType.Const));
                nextSymbol = this.GetNextSymbol();
            }
            if (nextSymbol.SymbolType != SymbolType.Other)
            {
                throw this.CreateSyntaxException();
            }
            LiteralExpression typeTokenExpression = this.GetTypeTokenExpression(unsafeCode, true);
            if ((typeTokenExpression == null) || (typeTokenExpression.Tokens.First == null))
            {
                throw new SyntaxException(this.document.SourceCode, token.LineNumber);
            }
            if (firstItemNode == null)
            {
                firstItemNode = typeTokenExpression.Tokens.First;
            }
            VariableDeclarationExpression expression = this.GetVariableDeclarationExpression(typeTokenExpression, ExpressionPrecedence.None, unsafeCode);
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            if (variables != null)
            {
                VariableModifiers modifiers = constant ? VariableModifiers.Const : VariableModifiers.None;
                foreach (VariableDeclaratorExpression expression3 in expression.Declarators)
                {
                    Variable variable = new Variable(expression.Type, expression3.Identifier.Token.Text, modifiers, expression3.Tokens.First.Value.Location.StartPoint, expression.Tokens.First.Value.Generated || expression3.Identifier.Token.Generated);
                    if (!variables.Contains(expression3.Identifier.Token.Text))
                    {
                        variables.Add(variable);
                    }
                }
            }
            return new VariableDeclarationStatement(new CsTokenList(this.tokens, firstItemNode, this.tokens.Last), constant, expression);
        }

        private WhileStatement ParseWhileStatement(bool unsafeCode)
        {
            CsToken item = this.GetToken(CsTokenType.While, SymbolType.While);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Bracket bracketToken = this.GetBracketToken(CsTokenType.OpenParenthesis, SymbolType.OpenParenthesis);
            Microsoft.StyleCop.Node<CsToken> node2 = this.tokens.InsertLast(bracketToken);
            Expression nextExpression = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
            if (nextExpression == null)
            {
                throw this.CreateSyntaxException();
            }
            Bracket bracket2 = this.GetBracketToken(CsTokenType.CloseParenthesis, SymbolType.CloseParenthesis);
            Microsoft.StyleCop.Node<CsToken> node3 = this.tokens.InsertLast(bracket2);
            bracketToken.MatchingBracketNode = node3;
            bracket2.MatchingBracketNode = node2;
            Statement nextStatement = this.GetNextStatement(unsafeCode);
            if (nextStatement == null)
            {
                throw new SyntaxException(this.document.SourceCode, item.LineNumber);
            }
            CsTokenList tokens = new CsTokenList(this.tokens, firstItemNode, this.tokens.Last);
            WhileStatement statement2 = new WhileStatement(tokens, nextExpression);
            statement2.EmbeddedStatement = nextStatement;
            return statement2;
        }

        private YieldStatement ParseYieldStatement(bool unsafeCode)
        {
            YieldStatement.Type @break;
            CsToken item = this.GetToken(CsTokenType.Yield, SymbolType.Other);
            Microsoft.StyleCop.Node<CsToken> firstItemNode = this.tokens.InsertLast(item);
            Symbol nextSymbol = this.GetNextSymbol();
            Expression returnValue = null;
            if (nextSymbol.SymbolType == SymbolType.Return)
            {
                @break = YieldStatement.Type.Return;
                this.tokens.Add(this.GetToken(CsTokenType.Return, SymbolType.Return));
                returnValue = this.GetNextExpression(ExpressionPrecedence.None, unsafeCode);
                if (returnValue == null)
                {
                    throw this.CreateSyntaxException();
                }
            }
            else
            {
                if (nextSymbol.SymbolType != SymbolType.Break)
                {
                    throw this.CreateSyntaxException();
                }
                @break = YieldStatement.Type.Break;
                this.tokens.Add(this.GetToken(CsTokenType.Break, SymbolType.Break));
            }
            this.tokens.Add(this.GetToken(CsTokenType.Semicolon, SymbolType.Semicolon));
            return new YieldStatement(new CsTokenList(this.tokens, firstItemNode, firstItemNode), @break, returnValue);
        }

        private OperatorSymbol PeekOperatorToken()
        {
            OperatorSymbol symbol2 = CreateOperatorToken(this.GetNextSymbol(), this.symbols.Generated);
            if (symbol2 == null)
            {
                throw this.CreateSyntaxException();
            }
            return symbol2;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method is not complex.")]
        private static bool SanityCheckElementTypeAgainstParent(ElementType elementType, CsElement parent)
        {
            switch (elementType)
            {
                case ElementType.File:
                case ElementType.Root:
                    return (parent == null);

                case ElementType.ExternAliasDirective:
                    if (parent == null)
                    {
                        return false;
                    }
                    return ((parent.ElementType == ElementType.Namespace) || (parent.ElementType == ElementType.Root));

                case ElementType.UsingDirective:
                case ElementType.Namespace:
                    if (parent == null)
                    {
                        return false;
                    }
                    if (parent.ElementType != ElementType.Root)
                    {
                        return (parent.ElementType == ElementType.Namespace);
                    }
                    return true;

                case ElementType.Field:
                case ElementType.Constructor:
                case ElementType.Destructor:
                    if (parent == null)
                    {
                        return false;
                    }
                    return ((parent.ElementType == ElementType.Class) || (parent.ElementType == ElementType.Struct));

                case ElementType.Delegate:
                case ElementType.Enum:
                case ElementType.Interface:
                case ElementType.Struct:
                case ElementType.Class:
                    if (parent == null)
                    {
                        return false;
                    }
                    return ((((parent.ElementType == ElementType.Root) || (parent.ElementType == ElementType.Namespace)) || (parent.ElementType == ElementType.Class)) || (parent.ElementType == ElementType.Struct));

                case ElementType.Event:
                case ElementType.Property:
                case ElementType.Indexer:
                case ElementType.Method:
                    if (parent == null)
                    {
                        return false;
                    }
                    return (((parent.ElementType == ElementType.Class) || (parent.ElementType == ElementType.Struct)) || (parent.ElementType == ElementType.Interface));

                case ElementType.Accessor:
                    if (parent == null)
                    {
                        return false;
                    }
                    return (((parent.ElementType == ElementType.Property) || (parent.ElementType == ElementType.Indexer)) || (parent.ElementType == ElementType.Event));

                case ElementType.EnumItem:
                    if (parent == null)
                    {
                        return false;
                    }
                    return (parent.ElementType == ElementType.Enum);

                case ElementType.ConstructorInitializer:
                    if (parent == null)
                    {
                        return false;
                    }
                    return (parent.ElementType == ElementType.Constructor);
            }
            return true;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method is not complex.")]
        private static SymbolType SymbolTypeFromOperatorType(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.ConditionalEquals:
                    return SymbolType.ConditionalEquals;

                case OperatorType.NotEquals:
                    return SymbolType.NotEquals;

                case OperatorType.LessThan:
                    return SymbolType.LessThan;

                case OperatorType.GreaterThan:
                    return SymbolType.GreaterThan;

                case OperatorType.LessThanOrEquals:
                    return SymbolType.LessThanOrEquals;

                case OperatorType.GreaterThanOrEquals:
                    return SymbolType.GreaterThanOrEquals;

                case OperatorType.LogicalAnd:
                    return SymbolType.LogicalAnd;

                case OperatorType.LogicalOr:
                    return SymbolType.LogicalOr;

                case OperatorType.LogicalXor:
                    return SymbolType.LogicalXor;

                case OperatorType.ConditionalAnd:
                    return SymbolType.ConditionalAnd;

                case OperatorType.ConditionalOr:
                    return SymbolType.ConditionalOr;

                case OperatorType.NullCoalescingSymbol:
                    return SymbolType.NullCoalescingSymbol;

                case OperatorType.Equals:
                    return SymbolType.Equals;

                case OperatorType.PlusEquals:
                    return SymbolType.PlusEquals;

                case OperatorType.MinusEquals:
                    return SymbolType.MinusEquals;

                case OperatorType.MultiplicationEquals:
                    return SymbolType.MultiplicationEquals;

                case OperatorType.DivisionEquals:
                    return SymbolType.DivisionEquals;

                case OperatorType.LeftShiftEquals:
                    return SymbolType.LeftShiftEquals;

                case OperatorType.RightShiftEquals:
                    return SymbolType.RightShiftEquals;

                case OperatorType.AndEquals:
                    return SymbolType.AndEquals;

                case OperatorType.OrEquals:
                    return SymbolType.OrEquals;

                case OperatorType.XorEquals:
                    return SymbolType.XorEquals;

                case OperatorType.Plus:
                    return SymbolType.Plus;

                case OperatorType.Minus:
                    return SymbolType.Minus;

                case OperatorType.Multiplication:
                    return SymbolType.Multiplication;

                case OperatorType.Division:
                    return SymbolType.Division;

                case OperatorType.Mod:
                    return SymbolType.Mod;

                case OperatorType.ModEquals:
                    return SymbolType.ModEquals;

                case OperatorType.LeftShift:
                    return SymbolType.LeftShift;

                case OperatorType.RightShift:
                    return SymbolType.RightShift;

                case OperatorType.ConditionalColon:
                    return SymbolType.Colon;

                case OperatorType.ConditionalQuestionMark:
                    return SymbolType.QuestionMark;

                case OperatorType.Increment:
                    return SymbolType.Increment;

                case OperatorType.Decrement:
                    return SymbolType.Decrement;

                case OperatorType.Not:
                    return SymbolType.Not;

                case OperatorType.BitwiseCompliment:
                    return SymbolType.Tilde;

                case OperatorType.Positive:
                    return SymbolType.Plus;

                case OperatorType.Negative:
                    return SymbolType.Minus;

                case OperatorType.Dereference:
                    return SymbolType.Multiplication;

                case OperatorType.AddressOf:
                    return SymbolType.LogicalAnd;

                case OperatorType.Pointer:
                    return SymbolType.Pointer;

                case OperatorType.MemberAccess:
                    return SymbolType.Dot;

                case OperatorType.QualifiedAlias:
                    return SymbolType.QualifiedAlias;

                case OperatorType.Lambda:
                    return SymbolType.Lambda;
            }
            throw new StyleCopException();
        }

        private static CsTokenType TokenTypeFromSymbolType(SymbolType symbolType)
        {
            switch (symbolType)
            {
                case SymbolType.WhiteSpace:
                    return CsTokenType.WhiteSpace;

                case SymbolType.EndOfLine:
                    return CsTokenType.EndOfLine;

                case SymbolType.SingleLineComment:
                    return CsTokenType.SingleLineComment;

                case SymbolType.MultiLineComment:
                    return CsTokenType.MultiLineComment;

                case SymbolType.PreprocessorDirective:
                    return CsTokenType.PreprocessorDirective;

                case SymbolType.XmlHeaderLine:
                    return CsTokenType.XmlHeaderLine;
            }
            throw new StyleCopException();
        }

        internal static string TrimType(string type)
        {
            if (type == null)
            {
                return null;
            }
            int length = 0;
            char[] chArray = new char[type.Length];
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < type.Length; i++)
            {
                char c = type[i];
                if (flag2)
                {
                    switch (c)
                    {
                        case '\r':
                        case '\n':
                            flag2 = false;
                            break;
                    }
                }
                else if (flag)
                {
                    if (((c == '*') && (i < (type.Length - 1))) && (type[i + 1] == '/'))
                    {
                        i++;
                        flag = false;
                    }
                }
                else if (!char.IsWhiteSpace(c))
                {
                    if ((c == '/') && (i < (type.Length - 1)))
                    {
                        if (type[i + 1] == '/')
                        {
                            flag2 = true;
                        }
                        else if (type[i + 1] == '*')
                        {
                            flag = true;
                        }
                    }
                    if (!flag2 && !flag)
                    {
                        chArray[length++] = c;
                    }
                }
            }
            if (length == type.Length)
            {
                return type;
            }
            return new string(chArray, 0, length);
        }

        private static bool TryCrackCodeAnalysisSuppression(MethodInvocationExpression codeAnalysisAttributeExpression, out string ruleId, out string ruleName, out string ruleNamespace)
        {
            string str2;
            ruleNamespace = (string) (str2 = null);
            ruleId = ruleName = str2;
            if ((codeAnalysisAttributeExpression.Arguments == null) || (codeAnalysisAttributeExpression.Arguments.Count < 2))
            {
                return false;
            }
            ruleNamespace = ExtractStringFromAttributeExpression(codeAnalysisAttributeExpression.Arguments[0]);
            if (string.IsNullOrEmpty(ruleNamespace))
            {
                return false;
            }
            string str = ExtractStringFromAttributeExpression(codeAnalysisAttributeExpression.Arguments[1]);
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            int index = str.IndexOf(':');
            if (index == -1)
            {
                return false;
            }
            ruleId = str.Substring(0, index);
            ruleName = str.Substring(index + 1, (str.Length - index) - 1);
            return ((ruleId.Length > 0) && (ruleName.Length > 0));
        }

        public CsDocument Document
        {
            get
            {
                return this.document;
            }
        }

        private enum ExpressionPrecedence
        {
            Global,
            Primary,
            Unary,
            Multiplicative,
            Additive,
            Shift,
            Relational,
            Equality,
            LogicalAnd,
            LogicalXor,
            LogicalOr,
            ConditionalAnd,
            ConditionalOr,
            NullCoalescing,
            Conditional,
            Assignment,
            Query,
            None
        }

        [Flags]
        private enum SkipSymbols
        {
            All = 0xff,
            EndOfLine = 2,
            MultiLineComment = 8,
            None = 0,
            Preprocessor = 0x20,
            SingleLineComment = 4,
            WhiteSpace = 1,
            XmlHeader = 0x10
        }
    }
}

