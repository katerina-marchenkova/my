namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class CodeLexer
    {
        private CodeReader codeReader;
        private Stack<bool> conditionalDirectives = new Stack<bool>();
        private Dictionary<string, string> defines;
        private MarkerData marker = new MarkerData();
        private CsParser parser;
        private Microsoft.StyleCop.SourceCode source;
        private Dictionary<string, string> undefines;

        internal CodeLexer(CsParser parser, Microsoft.StyleCop.SourceCode source, CodeReader codeReader)
        {
            this.parser = parser;
            this.source = source;
            this.codeReader = codeReader;
        }

        private void AdvanceToEndOfLine(StringBuilder text)
        {
            int count = this.FindNextEndOfLine();
            if (count != -1)
            {
                text.Append(this.codeReader.ReadString(count));
            }
        }

        private void CheckForConditionalCompilationDirective(Microsoft.StyleCop.SourceCode sourceCode, Symbol preprocessorSymbol, Configuration configuration)
        {
            int num;
            int num2;
            Symbol symbol;
            string preprocessorDirectiveType = CsParser.GetPreprocessorDirectiveType(preprocessorSymbol, out num);
            switch (preprocessorDirectiveType)
            {
                case "define":
                    this.GetDefinePreprocessorDirective(sourceCode, preprocessorSymbol, num);
                    return;

                case "undef":
                    this.GetUndefinePreprocessorDirective(sourceCode, preprocessorSymbol, num);
                    return;

                case "endif":
                    if (this.conditionalDirectives.Count == 0)
                    {
                        throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
                    }
                    this.conditionalDirectives.Pop();
                    return;

                default:
                    bool flag;
                    if (this.GetIfElsePreprocessorDirectives(sourceCode, preprocessorSymbol, configuration, num, preprocessorDirectiveType, out flag))
                    {
                        return;
                    }
                    if (!flag)
                    {
                        if (this.conditionalDirectives.Count == 0)
                        {
                            throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
                        }
                        this.conditionalDirectives.Pop();
                        this.conditionalDirectives.Push(true);
                        return;
                    }
                    num2 = 0;
                    goto Label_0085;
            }
        Label_0085:
            do
            {
                symbol = this.GetSymbol(sourceCode, configuration, false);
                if (symbol == null)
                {
                    throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
                }
            }
            while (symbol.SymbolType != SymbolType.PreprocessorDirective);
            preprocessorDirectiveType = CsParser.GetPreprocessorDirectiveType(symbol, out num);
            if (preprocessorDirectiveType == "if")
            {
                num2++;
                goto Label_0085;
            }
            if ((num2 > 0) && (preprocessorDirectiveType == "endif"))
            {
                num2--;
                goto Label_0085;
            }
            if ((num2 != 0) || ((!(preprocessorDirectiveType == "elif") && !(preprocessorDirectiveType == "else")) && !(preprocessorDirectiveType == "endif")))
            {
                goto Label_0085;
            }
            this.marker.Index = symbol.Location.StartPoint.Index;
            this.marker.IndexOnLine = symbol.Location.StartPoint.IndexOnLine;
            this.marker.LineNumber = symbol.Location.StartPoint.LineNumber;
        }

        private static CodeLocation CodeLocationFromMarker(MarkerData marker)
        {
            return new CodeLocation(marker.Index, marker.Index, marker.IndexOnLine, marker.IndexOnLine, marker.LineNumber, marker.LineNumber);
        }

        private Symbol CreateAndMovePastSymbol(string text, SymbolType type)
        {
            this.codeReader.ReadNext();
            Symbol symbol = new Symbol(text, type, CodeLocationFromMarker(this.marker));
            this.marker.Index++;
            this.marker.IndexOnLine++;
            return symbol;
        }

        private bool EvaluateConditionalDirectiveExpression(Microsoft.StyleCop.SourceCode sourceCode, Expression expression, Configuration configuration)
        {
            bool flag2;
            bool flag3;
            switch (expression.ExpressionType)
            {
                case ExpressionType.Parenthesized:
                {
                    ParenthesizedExpression expression6 = expression as ParenthesizedExpression;
                    return this.EvaluateConditionalDirectiveExpression(sourceCode, expression6.InnerExpression, configuration);
                }
                case ExpressionType.Relational:
                {
                    RelationalExpression expression4 = expression as RelationalExpression;
                    flag2 = this.EvaluateConditionalDirectiveExpression(sourceCode, expression4.LeftHandSide, configuration);
                    flag3 = this.EvaluateConditionalDirectiveExpression(sourceCode, expression4.RightHandSide, configuration);
                    if (expression4.OperatorType != RelationalExpression.Operator.EqualTo)
                    {
                        if (expression4.OperatorType != RelationalExpression.Operator.NotEqualTo)
                        {
                            throw new SyntaxException(sourceCode, expression.Tokens.First.Value.LineNumber);
                        }
                        return (flag2 != flag3);
                    }
                    return (flag2 == flag3);
                }
                case ExpressionType.Unary:
                {
                    UnaryExpression expression5 = expression as UnaryExpression;
                    if (expression5.OperatorType != UnaryExpression.Operator.Not)
                    {
                        throw new SyntaxException(sourceCode, expression.Tokens.First.Value.LineNumber);
                    }
                    return !this.EvaluateConditionalDirectiveExpression(sourceCode, expression5.Value, configuration);
                }
                case ExpressionType.ConditionalLogical:
                {
                    ConditionalLogicalExpression expression3 = expression as ConditionalLogicalExpression;
                    flag2 = this.EvaluateConditionalDirectiveExpression(sourceCode, expression3.LeftHandSide, configuration);
                    flag3 = this.EvaluateConditionalDirectiveExpression(sourceCode, expression3.RightHandSide, configuration);
                    if (expression3.OperatorType == ConditionalLogicalExpression.Operator.And)
                    {
                        return (flag2 && flag3);
                    }
                    return (flag2 || flag3);
                }
                case ExpressionType.Literal:
                {
                    LiteralExpression expression2 = expression as LiteralExpression;
                    if (expression2.Text == "false")
                    {
                        return false;
                    }
                    if (expression2.Text == "true")
                    {
                        return true;
                    }
                    if ((this.undefines != null) && this.undefines.ContainsKey(expression2.Text))
                    {
                        return false;
                    }
                    return (((this.defines != null) && this.defines.ContainsKey(expression2.Text)) || ((configuration != null) && configuration.Contains(expression2.Text)));
                }
            }
            throw new SyntaxException(sourceCode, expression.Tokens.First.Value.LineNumber);
        }

        private int FindNextEndOfLine()
        {
            int num = -1;
            int num2 = -1;
            int index = 0;
            while (true)
            {
                switch (this.codeReader.Peek(index))
                {
                    case '\0':
                        goto Label_003C;

                    case '\r':
                        if (num2 == -1)
                        {
                            num2 = index;
                            if (num == -1)
                            {
                                break;
                            }
                        }
                        goto Label_003C;

                    case '\n':
                        if (num != -1)
                        {
                            goto Label_003C;
                        }
                        num = index;
                        if (num2 != -1)
                        {
                            goto Label_003C;
                        }
                        break;
                }
                index++;
            }
        Label_003C:
            if ((num != -1) && (num2 != -1))
            {
                return Math.Min(num, num2);
            }
            if (num2 != -1)
            {
                return num2;
            }
            if (num != -1)
            {
                return num;
            }
            return index;
        }

        private Symbol GetComment()
        {
            Symbol symbol = null;
            StringBuilder text = new StringBuilder();
            char ch = this.codeReader.Peek(1);
            switch (ch)
            {
                case '\0':
                    return symbol;

                case '*':
                    text.Append(this.codeReader.ReadNext());
                    return this.GetMultiLineComment(text);
            }
            if (ch != '/')
            {
                return symbol;
            }
            text.Append(this.codeReader.ReadNext());
            text.Append(this.codeReader.ReadNext());
            if (this.codeReader.Peek() == '/')
            {
                text.Append(this.codeReader.ReadNext());
                if (this.codeReader.Peek() != '/')
                {
                    return this.GetXmlHeaderLine(text);
                }
                return this.GetSingleLineComment(text);
            }
            return this.GetSingleLineComment(text);
        }

        private int GetDecimalFraction(int index)
        {
            int num = index;
            while (true)
            {
                char ch = this.codeReader.Peek(index - this.marker.Index);
                if ((ch < '0') || (ch > '9'))
                {
                    break;
                }
                index++;
            }
            index--;
            if (index < num)
            {
                index = -1;
            }
            return index;
        }

        private int GetDecimalLiteral(int index)
        {
            int num = index;
            while (true)
            {
                char ch = this.codeReader.Peek(index - this.marker.Index);
                if ((ch < '0') || (ch > '9'))
                {
                    break;
                }
                index++;
            }
            index--;
            if (index >= num)
            {
                int integerTypeSuffix = this.GetIntegerTypeSuffix(index + 1);
                if (integerTypeSuffix != -1)
                {
                    index = integerTypeSuffix;
                    return index;
                }
                integerTypeSuffix = this.GetRealLiteralTrailingCharacters(index, false);
                if (integerTypeSuffix != -1)
                {
                    index = integerTypeSuffix;
                }
                return index;
            }
            int realLiteralTrailingCharacters = this.GetRealLiteralTrailingCharacters(index, true);
            if (realLiteralTrailingCharacters != -1)
            {
                index = realLiteralTrailingCharacters;
            }
            if (index < num)
            {
                index = -1;
            }
            return index;
        }

        private void GetDefinePreprocessorDirective(Microsoft.StyleCop.SourceCode sourceCode, Symbol preprocessorSymbol, int startIndex)
        {
            LiteralExpression expression = CodeParser.GetConditionalPreprocessorBodyExpression(this.parser, sourceCode, preprocessorSymbol, startIndex) as LiteralExpression;
            if (expression == null)
            {
                throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
            }
            if (this.defines == null)
            {
                this.defines = new Dictionary<string, string>();
            }
            this.defines.Add(expression.Text, expression.Text);
            if (this.undefines != null)
            {
                this.undefines.Remove(expression.Text);
            }
        }

        private int GetHexidecimalIntegerLiteral(int index)
        {
            int num = index;
            while (true)
            {
                char ch = this.codeReader.Peek(index - this.marker.Index);
                if ((((ch < '0') || (ch > '9')) && ((ch < 'a') || (ch > 'f'))) && ((ch < 'A') || (ch > 'F')))
                {
                    break;
                }
                index++;
            }
            index--;
            if (index >= num)
            {
                int integerTypeSuffix = this.GetIntegerTypeSuffix(index + 1);
                if (integerTypeSuffix != -1)
                {
                    index = integerTypeSuffix;
                }
                return index;
            }
            index = -1;
            return index;
        }

        private bool GetIfElsePreprocessorDirectives(Microsoft.StyleCop.SourceCode sourceCode, Symbol preprocessorSymbol, Configuration configuration, int startIndex, string type, out bool skip)
        {
            bool flag = false;
            skip = false;
            if (type == "if")
            {
                this.conditionalDirectives.Push(false);
                Expression expression = CodeParser.GetConditionalPreprocessorBodyExpression(this.parser, sourceCode, preprocessorSymbol, startIndex);
                if (expression == null)
                {
                    throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
                }
                skip = !this.EvaluateConditionalDirectiveExpression(sourceCode, expression, configuration);
                return flag;
            }
            if (type == "elif")
            {
                if (this.conditionalDirectives.Count == 0)
                {
                    throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
                }
                if (this.conditionalDirectives.Peek())
                {
                    skip = true;
                    return flag;
                }
                Expression expression2 = CodeParser.GetConditionalPreprocessorBodyExpression(this.parser, sourceCode, preprocessorSymbol, startIndex);
                if (expression2 != null)
                {
                    skip = !this.EvaluateConditionalDirectiveExpression(sourceCode, expression2, configuration);
                }
                return flag;
            }
            if (type == "else")
            {
                if (this.conditionalDirectives.Count == 0)
                {
                    throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
                }
                if (this.conditionalDirectives.Peek())
                {
                    skip = true;
                }
                return flag;
            }
            return true;
        }

        private int GetIntegerTypeSuffix(int index)
        {
            int num = -1;
            char ch = this.codeReader.Peek(index - this.marker.Index);
            switch (ch)
            {
                case 'u':
                case 'U':
                    num = index;
                    ch = this.codeReader.Peek((index + 1) - this.marker.Index);
                    if ((ch != 'l') && (ch != 'L'))
                    {
                        return num;
                    }
                    return (index + 1);
            }
            if ((ch != 'l') && (ch != 'L'))
            {
                return num;
            }
            num = index;
            ch = this.codeReader.Peek((index + 1) - this.marker.Index);
            if ((ch != 'u') && (ch != 'U'))
            {
                return num;
            }
            return (index + 1);
        }

        private Symbol GetLiteral()
        {
            StringBuilder text = new StringBuilder();
            char ch = this.codeReader.ReadNext();
            text.Append(ch);
            ch = this.codeReader.Peek();
            switch (ch)
            {
                case '\0':
                    throw new SyntaxException(this.source, this.marker.LineNumber);

                case '\'':
                case '"':
                    return this.GetLiteralString(text);
            }
            if (!char.IsLetter(ch) && (ch != '_'))
            {
                throw new SyntaxException(this.source, this.marker.LineNumber);
            }
            return this.GetLiteralKeyword(text);
        }

        private Symbol GetLiteralKeyword(StringBuilder text)
        {
            this.ReadToEndOfOtherSymbol(text);
            if (text.Length == 1)
            {
                throw new SyntaxException(this.source, this.marker.LineNumber);
            }
            CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + text.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + text.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol symbol = new Symbol(text.ToString(), SymbolType.Other, location);
            this.marker.Index += text.Length;
            this.marker.IndexOnLine += text.Length;
            return symbol;
        }

        private Symbol GetLiteralString(StringBuilder text)
        {
            char ch2;
            int index = this.marker.Index;
            int endIndex = this.marker.Index;
            int indexOnLine = this.marker.IndexOnLine;
            int endIndexOnLine = this.marker.IndexOnLine;
            int lineNumber = this.marker.LineNumber;
            int endLineNumber = this.marker.LineNumber;
            char ch = this.codeReader.Peek();
            text.Append(ch);
            this.codeReader.ReadNext();
            endIndex += 2;
            endIndexOnLine += 2;
        Label_0074:
            ch2 = this.codeReader.Peek();
            if (ch2 == '\0')
            {
                goto Label_0152;
            }
            if (ch2 == ch)
            {
                this.codeReader.ReadNext();
                text.Append(ch2);
                endIndex++;
                endIndexOnLine++;
                ch2 = this.codeReader.Peek();
                if (ch2 != ch)
                {
                    goto Label_0152;
                }
                this.codeReader.ReadNext();
                text.Append(ch2);
                endIndex++;
                endIndexOnLine++;
            }
            else
            {
                if ((ch2 == '\r') || (ch2 == '\n'))
                {
                    if (ch == '\'')
                    {
                        throw new SyntaxException(this.source, this.marker.LineNumber);
                    }
                    switch (ch2)
                    {
                        case '\n':
                            endLineNumber++;
                            endIndexOnLine = -1;
                            break;

                        case '\r':
                            this.codeReader.ReadNext();
                            goto Label_0074;
                    }
                }
                this.codeReader.ReadNext();
                text.Append(ch2);
                endIndex++;
                endIndexOnLine++;
            }
            goto Label_0074;
        Label_0152:
            if ((text.Length <= 2) || (text[text.Length - 1] != ch))
            {
                throw new SyntaxException(this.source, this.marker.LineNumber);
            }
            CodeLocation location = new CodeLocation(index, endIndex, indexOnLine, endIndexOnLine, lineNumber, endLineNumber);
            Symbol symbol = new Symbol(text.ToString(), SymbolType.String, location);
            this.marker.Index = endIndex + 1;
            this.marker.IndexOnLine = endIndexOnLine + 1;
            this.marker.LineNumber = endLineNumber;
            return symbol;
        }

        private Symbol GetMultiLineComment(StringBuilder text)
        {
            int index = this.marker.Index;
            int endIndex = this.marker.Index + 1;
            int indexOnLine = this.marker.IndexOnLine;
            int endIndexOnLine = this.marker.IndexOnLine + 1;
            int lineNumber = this.marker.LineNumber;
            int endLineNumber = this.marker.LineNumber;
            bool flag = false;
            bool flag2 = false;
            while (true)
            {
                char ch = this.codeReader.Peek();
                if (ch == '\0')
                {
                    break;
                }
                text.Append(this.codeReader.ReadNext());
                if (flag && (ch == '/'))
                {
                    break;
                }
                if (ch == '*')
                {
                    if (flag2)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag2 = true;
                    }
                }
                else
                {
                    flag = false;
                    if (ch == '\n')
                    {
                        endLineNumber++;
                        endIndexOnLine = -1;
                    }
                    else if ((ch == '\r') && (this.codeReader.Peek() != '\n'))
                    {
                        endLineNumber++;
                        endIndexOnLine = -1;
                    }
                }
                endIndex++;
                endIndexOnLine++;
            }
            CodeLocation location = new CodeLocation(index, endIndex, indexOnLine, endIndexOnLine, lineNumber, endLineNumber);
            Symbol symbol = new Symbol(text.ToString(), SymbolType.MultiLineComment, location);
            this.marker.Index = endIndex + 1;
            this.marker.IndexOnLine = endIndexOnLine + 1;
            this.marker.LineNumber = endLineNumber;
            return symbol;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId="Microsoft.StyleCop.CSharp.Symbol.#ctor(System.String,Microsoft.StyleCop.CSharp.SymbolType,Microsoft.StyleCop.CodeLocation)", Justification="The literal is a non-localizable newline character")]
        private Symbol GetNewLine()
        {
            Symbol symbol = null;
            char ch = this.codeReader.Peek();
            if (ch != '\0')
            {
                this.codeReader.ReadNext();
                int index = this.marker.Index;
                int endIndex = this.marker.Index;
                if ((ch == '\r') && (this.codeReader.Peek() == '\n'))
                {
                    this.codeReader.ReadNext();
                    this.marker.Index++;
                    endIndex++;
                }
                CodeLocation location = new CodeLocation(index, endIndex, this.marker.IndexOnLine, this.marker.IndexOnLine + (endIndex - index), this.marker.LineNumber, this.marker.LineNumber);
                symbol = new Symbol("\n", SymbolType.EndOfLine, location);
                this.marker.Index++;
                this.marker.LineNumber++;
                this.marker.IndexOnLine = 0;
            }
            return symbol;
        }

        private Symbol GetNumber()
        {
            int positiveNumber = -1;
            char ch = this.codeReader.Peek();
            switch (ch)
            {
                case '-':
                case '+':
                    ch = this.codeReader.Peek(1);
                    if ((ch >= '0') && (ch <= '9'))
                    {
                        positiveNumber = this.GetPositiveNumber(this.marker.Index + 1);
                    }
                    break;

                default:
                    positiveNumber = this.GetPositiveNumber(this.marker.Index);
                    break;
            }
            Symbol symbol = null;
            if (positiveNumber >= this.marker.Index)
            {
                int count = (positiveNumber - this.marker.Index) + 1;
                string text = this.codeReader.ReadString(count);
                CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + count) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + count) - 1, this.marker.LineNumber, this.marker.LineNumber);
                symbol = new Symbol(text, SymbolType.Number, location);
                this.marker.Index += count;
                this.marker.IndexOnLine += count;
            }
            return symbol;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification="The method long, but simple."), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method long, but simple.")]
        private Symbol GetOperatorSymbol(char character)
        {
            SymbolType other = SymbolType.Other;
            StringBuilder builder = new StringBuilder();
            if (character == '.')
            {
                builder.Append(".");
                other = SymbolType.Dot;
                this.codeReader.ReadNext();
            }
            else if (character == '~')
            {
                builder.Append("~");
                other = SymbolType.Tilde;
                this.codeReader.ReadNext();
            }
            else if (character == '+')
            {
                builder.Append("+");
                other = SymbolType.Plus;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '+')
                {
                    builder.Append("+");
                    other = SymbolType.Increment;
                    this.codeReader.ReadNext();
                }
                else if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.PlusEquals;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '-')
            {
                builder.Append("-");
                other = SymbolType.Minus;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '-')
                {
                    builder.Append("-");
                    other = SymbolType.Decrement;
                    this.codeReader.ReadNext();
                }
                else if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.MinusEquals;
                    this.codeReader.ReadNext();
                }
                else if (character == '>')
                {
                    builder.Append(">");
                    other = SymbolType.Pointer;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '*')
            {
                builder.Append("*");
                other = SymbolType.Multiplication;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("*");
                    other = SymbolType.MultiplicationEquals;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '/')
            {
                builder.Append("/");
                other = SymbolType.Division;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.DivisionEquals;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '|')
            {
                builder.Append("|");
                other = SymbolType.LogicalOr;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.OrEquals;
                    this.codeReader.ReadNext();
                }
                else if (character == '|')
                {
                    builder.Append("|");
                    other = SymbolType.ConditionalOr;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '&')
            {
                builder.Append("&");
                other = SymbolType.LogicalAnd;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.AndEquals;
                    this.codeReader.ReadNext();
                }
                else if (character == '&')
                {
                    builder.Append("&");
                    other = SymbolType.ConditionalAnd;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '^')
            {
                builder.Append("^");
                other = SymbolType.LogicalXor;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.XorEquals;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '!')
            {
                builder.Append("!");
                other = SymbolType.Not;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.NotEquals;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '%')
            {
                builder.Append("%");
                other = SymbolType.Mod;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.ModEquals;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '=')
            {
                builder.Append("=");
                other = SymbolType.Equals;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.ConditionalEquals;
                    this.codeReader.ReadNext();
                }
                else if (character == '>')
                {
                    builder.Append(">");
                    other = SymbolType.Lambda;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '<')
            {
                builder.Append("<");
                other = SymbolType.LessThan;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.LessThanOrEquals;
                    this.codeReader.ReadNext();
                }
                else if (character == '<')
                {
                    builder.Append("<");
                    other = SymbolType.LeftShift;
                    this.codeReader.ReadNext();
                    character = this.codeReader.Peek();
                    if (character == '=')
                    {
                        builder.Append("=");
                        other = SymbolType.LeftShiftEquals;
                        this.codeReader.ReadNext();
                    }
                }
            }
            else if (character == '>')
            {
                builder.Append(">");
                other = SymbolType.GreaterThan;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '=')
                {
                    builder.Append("=");
                    other = SymbolType.GreaterThanOrEquals;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == '?')
            {
                builder.Append("?");
                other = SymbolType.QuestionMark;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == '?')
                {
                    builder.Append("?");
                    other = SymbolType.NullCoalescingSymbol;
                    this.codeReader.ReadNext();
                }
            }
            else if (character == ':')
            {
                builder.Append(":");
                other = SymbolType.Colon;
                this.codeReader.ReadNext();
                character = this.codeReader.Peek();
                if (character == ':')
                {
                    builder.Append(":");
                    other = SymbolType.QualifiedAlias;
                    this.codeReader.ReadNext();
                }
            }
            if ((builder == null) || (builder.Length == 0))
            {
                throw new SyntaxException(this.source, this.marker.LineNumber);
            }
            CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + builder.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + builder.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol symbol = new Symbol(builder.ToString(), other, location);
            this.marker.Index += builder.Length;
            this.marker.IndexOnLine += builder.Length;
            return symbol;
        }

        private Symbol GetOtherSymbol(Microsoft.StyleCop.SourceCode sourceCode)
        {
            StringBuilder text = new StringBuilder();
            this.ReadToEndOfOtherSymbol(text);
            if (text.Length == 0)
            {
                throw new SyntaxException(sourceCode, 1);
            }
            string str = text.ToString();
            CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + text.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + text.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol symbol = new Symbol(str, GetOtherSymbolType(str), location);
            this.marker.Index += text.Length;
            this.marker.IndexOnLine += text.Length;
            return symbol;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method consists of a simple switch statement.")]
        private static SymbolType GetOtherSymbolType(string text)
        {
            switch (text)
            {
                case "abstract":
                    return SymbolType.Abstract;

                case "as":
                    return SymbolType.As;

                case "base":
                    return SymbolType.Base;

                case "break":
                    return SymbolType.Break;

                case "case":
                    return SymbolType.Case;

                case "catch":
                    return SymbolType.Catch;

                case "checked":
                    return SymbolType.Checked;

                case "class":
                    return SymbolType.Class;

                case "const":
                    return SymbolType.Const;

                case "continue":
                    return SymbolType.Continue;

                case "default":
                    return SymbolType.Default;

                case "delegate":
                    return SymbolType.Delegate;

                case "do":
                    return SymbolType.Do;

                case "else":
                    return SymbolType.Else;

                case "enum":
                    return SymbolType.Enum;

                case "event":
                    return SymbolType.Event;

                case "explicit":
                    return SymbolType.Explicit;

                case "extern":
                    return SymbolType.Extern;

                case "false":
                    return SymbolType.False;

                case "finally":
                    return SymbolType.Finally;

                case "fixed":
                    return SymbolType.Fixed;

                case "for":
                    return SymbolType.For;

                case "foreach":
                    return SymbolType.Foreach;

                case "goto":
                    return SymbolType.Goto;

                case "if":
                    return SymbolType.If;

                case "implicit":
                    return SymbolType.Implicit;

                case "in":
                    return SymbolType.In;

                case "interface":
                    return SymbolType.Interface;

                case "internal":
                    return SymbolType.Internal;

                case "is":
                    return SymbolType.Is;

                case "lock":
                    return SymbolType.Lock;

                case "namespace":
                    return SymbolType.Namespace;

                case "new":
                    return SymbolType.New;

                case "null":
                    return SymbolType.Null;

                case "operator":
                    return SymbolType.Operator;

                case "out":
                    return SymbolType.Out;

                case "override":
                    return SymbolType.Override;

                case "params":
                    return SymbolType.Params;

                case "private":
                    return SymbolType.Private;

                case "protected":
                    return SymbolType.Protected;

                case "public":
                    return SymbolType.Public;

                case "readonly":
                    return SymbolType.Readonly;

                case "ref":
                    return SymbolType.Ref;

                case "return":
                    return SymbolType.Return;

                case "sealed":
                    return SymbolType.Sealed;

                case "sizeof":
                    return SymbolType.Sizeof;

                case "stackalloc":
                    return SymbolType.Stackalloc;

                case "static":
                    return SymbolType.Static;

                case "struct":
                    return SymbolType.Struct;

                case "switch":
                    return SymbolType.Switch;

                case "this":
                    return SymbolType.This;

                case "throw":
                    return SymbolType.Throw;

                case "true":
                    return SymbolType.True;

                case "try":
                    return SymbolType.Try;

                case "typeof":
                    return SymbolType.Typeof;

                case "unchecked":
                    return SymbolType.Unchecked;

                case "unsafe":
                    return SymbolType.Unsafe;

                case "using":
                    return SymbolType.Using;

                case "virtual":
                    return SymbolType.Virtual;

                case "volatile":
                    return SymbolType.Volatile;

                case "while":
                    return SymbolType.While;
            }
            return SymbolType.Other;
        }

        private int GetPositiveNumber(int index)
        {
            if (this.codeReader.Peek() == '0')
            {
                switch (this.codeReader.Peek(1))
                {
                    case 'x':
                    case 'X':
                        return this.GetHexidecimalIntegerLiteral(index + 2);
                }
            }
            return this.GetDecimalLiteral(index);
        }

        private Symbol GetPreprocessorDirectiveSymbol(Microsoft.StyleCop.SourceCode sourceCode, Configuration configuration, bool evaluate)
        {
            StringBuilder text = new StringBuilder();
            this.AdvanceToEndOfLine(text);
            if (text.Length == 1)
            {
                throw new SyntaxException(Microsoft.StyleCop.CSharp.Strings.UnexpectedEndOfFile);
            }
            CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + text.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + text.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol preprocessorSymbol = new Symbol(text.ToString(), SymbolType.PreprocessorDirective, location);
            this.marker.Index += text.Length;
            this.marker.IndexOnLine += text.Length;
            if (evaluate && (configuration != null))
            {
                this.CheckForConditionalCompilationDirective(sourceCode, preprocessorSymbol, configuration);
            }
            return preprocessorSymbol;
        }

        private int GetRealLiteralExponent(int index)
        {
            int num = -1;
            char ch = this.codeReader.Peek(index - this.marker.Index);
            if ((ch != 'e') && (ch != 'E'))
            {
                return num;
            }
            index++;
            switch (this.codeReader.Peek(index - this.marker.Index))
            {
                case '-':
                case '+':
                    index++;
                    break;
            }
        Label_0052:
            ch = this.codeReader.Peek(index - this.marker.Index);
            if ((ch >= '0') && (ch <= '9'))
            {
                num = index;
                index++;
                goto Label_0052;
            }
            return num;
        }

        private int GetRealLiteralTrailingCharacters(int index, bool requiresDecimalPoint)
        {
            bool flag = false;
            bool flag2 = false;
            if (this.codeReader.Peek((index - this.marker.Index) + 1) == '.')
            {
                int decimalFraction = this.GetDecimalFraction(index + 2);
                if (decimalFraction != -1)
                {
                    index = decimalFraction;
                    flag2 = true;
                    flag = true;
                }
            }
            if (!requiresDecimalPoint || flag2)
            {
                switch (this.codeReader.Peek((index - this.marker.Index) + 1))
                {
                    case 'e':
                    case 'E':
                    {
                        int realLiteralExponent = this.GetRealLiteralExponent(index + 1);
                        if (realLiteralExponent != -1)
                        {
                            index = realLiteralExponent;
                            flag = true;
                        }
                        break;
                    }
                }
                char ch = this.codeReader.Peek((index - this.marker.Index) + 1);
                if ((((ch == 'm') || (ch == 'M')) || ((ch == 'd') || (ch == 'D'))) || ((ch == 'f') || (ch == 'F')))
                {
                    index++;
                    flag = true;
                }
            }
            if (!flag)
            {
                index = -1;
            }
            return index;
        }

        private Symbol GetSingleLineComment(StringBuilder text)
        {
            this.AdvanceToEndOfLine(text);
            CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + text.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + text.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol symbol = new Symbol(text.ToString(), SymbolType.SingleLineComment, location);
            this.marker.Index += text.Length;
            this.marker.IndexOnLine += text.Length;
            return symbol;
        }

        private Symbol GetString()
        {
            CodeLocation location;
            StringBuilder builder = new StringBuilder();
            char ch = this.codeReader.ReadNext();
            builder.Append(ch);
            bool flag = false;
            while (true)
            {
                char ch2 = this.codeReader.Peek();
                if ((ch2 == '\0') || ((ch2 == ch) && !flag))
                {
                    builder.Append(ch2);
                    this.codeReader.ReadNext();
                    break;
                }
                if (ch2 == '\\')
                {
                    flag = !flag;
                }
                else
                {
                    flag = false;
                    switch (ch2)
                    {
                        case '\r':
                        case '\n':
                            goto Label_0076;
                    }
                }
                builder.Append(ch2);
                this.codeReader.ReadNext();
            }
        Label_0076:
            location = new CodeLocation(this.marker.Index, (this.marker.Index + builder.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + builder.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol symbol = new Symbol(builder.ToString(), SymbolType.String, location);
            this.marker.Index += builder.Length;
            this.marker.IndexOnLine += builder.Length;
            return symbol;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification="The literals represent non-localizable C# operators."), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="The method is not overly complex.")]
        internal Symbol GetSymbol(Microsoft.StyleCop.SourceCode sourceCode, Configuration configuration, bool evaluatePreprocessors)
        {
            Symbol number = null;
            char character = this.codeReader.Peek();
            switch (character)
            {
                case '\t':
                case ' ':
                    return this.GetWhitespace();

                case '\n':
                case '\r':
                    return this.GetNewLine();

                case '!':
                case '%':
                case '&':
                case '*':
                case '+':
                case '-':
                case ':':
                case '<':
                case '=':
                case '>':
                case '?':
                case '^':
                case '|':
                case '~':
                    return this.GetOperatorSymbol(character);

                case '"':
                case '\'':
                    return this.GetString();

                case '#':
                    return this.GetPreprocessorDirectiveSymbol(sourceCode, configuration, evaluatePreprocessors);

                case '(':
                    return this.CreateAndMovePastSymbol("(", SymbolType.OpenParenthesis);

                case ')':
                    return this.CreateAndMovePastSymbol(")", SymbolType.CloseParenthesis);

                case ',':
                    return this.CreateAndMovePastSymbol(",", SymbolType.Comma);

                case '.':
                    number = this.GetNumber();
                    if (number == null)
                    {
                        number = this.GetOperatorSymbol('.');
                    }
                    return number;

                case '/':
                    number = this.GetComment();
                    if (number == null)
                    {
                        number = this.GetOperatorSymbol('/');
                    }
                    return number;

                case ';':
                    return this.CreateAndMovePastSymbol(";", SymbolType.Semicolon);

                case '@':
                    return this.GetLiteral();

                case '[':
                    return this.CreateAndMovePastSymbol("[", SymbolType.OpenSquareBracket);

                case ']':
                    return this.CreateAndMovePastSymbol("]", SymbolType.CloseSquareBracket);

                case '{':
                    return this.CreateAndMovePastSymbol("{", SymbolType.OpenCurlyBracket);

                case '}':
                    return this.CreateAndMovePastSymbol("}", SymbolType.CloseCurlyBracket);

                case '\0':
                    return number;
            }
            UnicodeCategory unicodeCategory = char.GetUnicodeCategory(character);
            if ((unicodeCategory != UnicodeCategory.Format) && (unicodeCategory != UnicodeCategory.OtherNotAssigned))
            {
                number = this.GetNumber();
                if (number == null)
                {
                    number = this.GetOtherSymbol(this.source);
                }
            }
            return number;
        }

        internal List<Symbol> GetSymbols(Microsoft.StyleCop.SourceCode sourceCode, Configuration configuration)
        {
            List<Symbol> list = new List<Symbol>();
            while (true)
            {
                Symbol item = this.GetSymbol(sourceCode, configuration, true);
                if (item == null)
                {
                    return list;
                }
                list.Add(item);
            }
        }

        private void GetUndefinePreprocessorDirective(Microsoft.StyleCop.SourceCode sourceCode, Symbol preprocessorSymbol, int startIndex)
        {
            LiteralExpression expression = CodeParser.GetConditionalPreprocessorBodyExpression(this.parser, sourceCode, preprocessorSymbol, startIndex) as LiteralExpression;
            if (expression == null)
            {
                throw new SyntaxException(sourceCode, preprocessorSymbol.LineNumber);
            }
            if (this.undefines == null)
            {
                this.undefines = new Dictionary<string, string>();
            }
            this.undefines.Add(expression.Text, expression.Text);
            if (this.defines != null)
            {
                this.defines.Remove(expression.Text);
            }
        }

        private Symbol GetWhitespace()
        {
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                char ch = this.codeReader.Peek();
                if ((ch == '\0') || ((ch != ' ') && (ch != '\t')))
                {
                    break;
                }
                builder.Append(ch);
                this.codeReader.ReadNext();
            }
            CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + builder.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + builder.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol symbol = new Symbol(builder.ToString(), SymbolType.WhiteSpace, location);
            this.marker.Index += builder.Length;
            this.marker.IndexOnLine += builder.Length;
            return symbol;
        }

        private Symbol GetXmlHeaderLine(StringBuilder text)
        {
            this.AdvanceToEndOfLine(text);
            CodeLocation location = new CodeLocation(this.marker.Index, (this.marker.Index + text.Length) - 1, this.marker.IndexOnLine, (this.marker.IndexOnLine + text.Length) - 1, this.marker.LineNumber, this.marker.LineNumber);
            Symbol symbol = new Symbol(text.ToString(), SymbolType.XmlHeaderLine, location);
            this.marker.Index += text.Length;
            this.marker.IndexOnLine += text.Length;
            return symbol;
        }

        private void ReadToEndOfOtherSymbol(StringBuilder text)
        {
            bool flag = false;
            while (true)
            {
                char c = this.codeReader.Peek();
                if (c == '\0')
                {
                    return;
                }
                if ((char.IsLetter(c) || (c == '_')) || (char.GetUnicodeCategory(c) == UnicodeCategory.OtherSymbol))
                {
                    flag = true;
                }
                else if (!flag || !char.IsNumber(c))
                {
                    return;
                }
                text.Append(c);
                this.codeReader.ReadNext();
            }
        }

        internal Microsoft.StyleCop.SourceCode SourceCode
        {
            get
            {
                return this.source;
            }
        }

        internal class MarkerData
        {
            private int index;
            private int indexOnLine;
            private int lineNumber = 1;

            public int Index
            {
                get
                {
                    return this.index;
                }
                set
                {
                    Param.RequireGreaterThanOrEqualToZero(value, "Index");
                    this.index = value;
                }
            }

            public int IndexOnLine
            {
                get
                {
                    return this.indexOnLine;
                }
                set
                {
                    Param.RequireGreaterThanOrEqualToZero(value, "IndexOnLine");
                    this.indexOnLine = value;
                }
            }

            public int LineNumber
            {
                get
                {
                    return this.lineNumber;
                }
                set
                {
                    Param.RequireGreaterThanZero(value, "LineNumber");
                    this.lineNumber = value;
                }
            }
        }
    }
}

