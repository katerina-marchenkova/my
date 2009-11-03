namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;

    [SourceAnalyzer(typeof(CsParser))]
    public class MaintainabilityRules : SourceAnalyzer
    {
        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            CsDocument document2 = (CsDocument) document;
            if ((document2.RootElement != null) && !document2.RootElement.Generated)
            {
                TopLevelElements context = new TopLevelElements();
                document2.WalkDocument<TopLevelElements>(new CodeWalkerElementVisitor<TopLevelElements>(this.ProcessElement), new CodeWalkerStatementVisitor<TopLevelElements>(this.ProcessStatement), new CodeWalkerExpressionVisitor<TopLevelElements>(this.ProcessExpression), context);
                if (context.Classes.Count > 1)
                {
                    string fullNamespaceName = string.Empty;
                    foreach (Class class2 in context.Classes)
                    {
                        if (!class2.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial }) || (!string.IsNullOrEmpty(fullNamespaceName) && (string.Compare(fullNamespaceName, class2.FullNamespaceName, StringComparison.Ordinal) != 0)))
                        {
                            base.AddViolation(class2, 1, Microsoft.StyleCop.CSharp.Rules.FileMayOnlyContainASingleClass, new object[0]);
                            break;
                        }
                        fullNamespaceName = class2.FullNamespaceName;
                    }
                }
                if (context.Namespaces.Count > 1)
                {
                    base.AddViolation(document2.RootElement, Microsoft.StyleCop.CSharp.Rules.FileMayOnlyContainASingleNamespace, new object[0]);
                }
            }
        }

        private void CheckAccessModifierRulesForElement(CsElement element)
        {
            if (!element.Generated)
            {
                CsElement parentElement = element.ParentElement;
                if ((parentElement == null) || (parentElement.ElementType != ElementType.Interface))
                {
                    this.CheckForAccessModifier(element);
                    this.CheckFieldAccessModifiers(element);
                }
            }
        }

        private void CheckAnonymousMethodParenthesis(CsElement element, AnonymousMethodExpression expression)
        {
            if ((expression.Parameters == null) || (expression.Parameters.Count == 0))
            {
                for (Microsoft.StyleCop.Node<CsToken> node = expression.Tokens.First; node != expression.Tokens.Last; node = node.Next)
                {
                    if (node.Value.CsTokenType == CsTokenType.OpenCurlyBracket)
                    {
                        return;
                    }
                    if (node.Value.CsTokenType == CsTokenType.OpenParenthesis)
                    {
                        base.AddViolation(element, node.Value.LineNumber, Microsoft.StyleCop.CSharp.Rules.RemoveDelegateParenthesisWhenPossible, new object[0]);
                        return;
                    }
                }
            }
        }

        private void CheckArithmeticExpressionParenthesis(CsElement element, ArithmeticExpression expression)
        {
            if (((expression.LeftHandSide.ExpressionType != ExpressionType.Arithmetic) || this.CheckArithmeticParenthesisForExpressionAndChild(element, expression, (ArithmeticExpression) expression.LeftHandSide)) && (expression.RightHandSide.ExpressionType == ExpressionType.Arithmetic))
            {
                this.CheckArithmeticParenthesisForExpressionAndChild(element, expression, (ArithmeticExpression) expression.RightHandSide);
            }
        }

        private bool CheckArithmeticParenthesisForExpressionAndChild(CsElement element, ArithmeticExpression expression, ArithmeticExpression childExpression)
        {
            if ((expression.OperatorType != childExpression.OperatorType) && (((((expression.OperatorType != ArithmeticExpression.Operator.Addition) && (expression.OperatorType != ArithmeticExpression.Operator.Subtraction)) || ((childExpression.OperatorType != ArithmeticExpression.Operator.Addition) && (childExpression.OperatorType != ArithmeticExpression.Operator.Subtraction))) && (((expression.OperatorType != ArithmeticExpression.Operator.Multiplication) && (expression.OperatorType != ArithmeticExpression.Operator.Division)) || ((childExpression.OperatorType != ArithmeticExpression.Operator.Multiplication) && (childExpression.OperatorType != ArithmeticExpression.Operator.Division)))) && (((expression.OperatorType != ArithmeticExpression.Operator.LeftShift) && (expression.OperatorType != ArithmeticExpression.Operator.RightShift)) || ((childExpression.OperatorType != ArithmeticExpression.Operator.LeftShift) && (childExpression.OperatorType != ArithmeticExpression.Operator.RightShift)))))
            {
                base.AddViolation(element, expression.LineNumber, Microsoft.StyleCop.CSharp.Rules.ArithmeticExpressionsMustDeclarePrecedence, new object[0]);
                return false;
            }
            return true;
        }

        private void CheckCodeAnalysisAttributeJustifications(CsElement element)
        {
            if (!element.Generated && (element.Attributes != null))
            {
                foreach (Microsoft.StyleCop.CSharp.Attribute attribute in element.Attributes)
                {
                    foreach (AttributeExpression expression in attribute.AttributeExpressions)
                    {
                        foreach (Expression expression2 in expression.ChildExpressions)
                        {
                            if (expression2.ExpressionType == ExpressionType.MethodInvocation)
                            {
                                MethodInvocationExpression expression3 = (MethodInvocationExpression) expression2;
                                if (IsSuppressMessage(expression3))
                                {
                                    this.CheckCodeAnalysisSuppressionForJustification(element, expression3);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CheckCodeAnalysisSuppressionForJustification(CsElement element, MethodInvocationExpression suppression)
        {
            bool flag = false;
            foreach (Expression expression in suppression.Arguments)
            {
                if (expression.ExpressionType == ExpressionType.Assignment)
                {
                    AssignmentExpression expression2 = (AssignmentExpression) expression;
                    if (expression2.LeftHandSide.Tokens.First.Value.Text.Equals("Justification", StringComparison.Ordinal))
                    {
                        Microsoft.StyleCop.Node<CsToken> first = expression2.RightHandSide.Tokens.First;
                        if (((first != null) && (first.Value.CsTokenType == CsTokenType.String)) && ((first.Value.Text != null) && !IsEmptyString(first.Value.Text)))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            if (!flag)
            {
                base.AddViolation(element, suppression.LineNumber, Microsoft.StyleCop.CSharp.Rules.CodeAnalysisSuppressionMustHaveJustification, new object[0]);
            }
        }

        private void CheckConditionalLogicalExpressionParenthesis(CsElement element, ConditionalLogicalExpression expression)
        {
            if (((expression.LeftHandSide.ExpressionType != ExpressionType.ConditionalLogical) || this.CheckConditionalLogicalParenthesisForExpressionAndChild(element, expression, (ConditionalLogicalExpression) expression.LeftHandSide)) && (expression.RightHandSide.ExpressionType == ExpressionType.ConditionalLogical))
            {
                this.CheckConditionalLogicalParenthesisForExpressionAndChild(element, expression, (ConditionalLogicalExpression) expression.RightHandSide);
            }
        }

        private bool CheckConditionalLogicalParenthesisForExpressionAndChild(CsElement element, ConditionalLogicalExpression expression, ConditionalLogicalExpression childExpression)
        {
            if (expression.OperatorType != childExpression.OperatorType)
            {
                base.AddViolation(element, expression.LineNumber, Microsoft.StyleCop.CSharp.Rules.ConditionalExpressionsMustDeclarePrecedence, new object[0]);
                return false;
            }
            return true;
        }

        private void CheckDebugAssertMessage(CsElement element, MethodInvocationExpression debugAssertMethodCall)
        {
            Expression expression = null;
            int num = 0;
            foreach (Expression expression2 in debugAssertMethodCall.Arguments)
            {
                if (num == 1)
                {
                    expression = expression2;
                    break;
                }
                num++;
            }
            if ((expression == null) || (expression.Tokens.First == null))
            {
                base.AddViolation(element, debugAssertMethodCall.LineNumber, Microsoft.StyleCop.CSharp.Rules.DebugAssertMustProvideMessageText, new object[0]);
            }
            else if ((expression.Tokens.First.Value.CsTokenType == CsTokenType.String) && IsEmptyString(expression.Tokens.First.Value.Text))
            {
                base.AddViolation(element, debugAssertMethodCall.LineNumber, Microsoft.StyleCop.CSharp.Rules.DebugAssertMustProvideMessageText, new object[0]);
            }
        }

        private void CheckDebugFailMessage(CsElement element, MethodInvocationExpression debugFailMethodCall)
        {
            Expression expression = null;
            foreach (Expression expression2 in debugFailMethodCall.Arguments)
            {
                expression = expression2;
                break;
            }
            if ((expression == null) || (expression.Tokens.First == null))
            {
                base.AddViolation(element, debugFailMethodCall.LineNumber, Microsoft.StyleCop.CSharp.Rules.DebugFailMustProvideMessageText, new object[0]);
            }
            else if ((expression.Tokens.First.Value.CsTokenType == CsTokenType.String) && IsEmptyString(expression.Tokens.First.Value.Text))
            {
                base.AddViolation(element, debugFailMethodCall.LineNumber, Microsoft.StyleCop.CSharp.Rules.DebugFailMustProvideMessageText, new object[0]);
            }
        }

        private void CheckFieldAccessModifiers(CsElement element)
        {
            if (((element.ElementType == ElementType.Field) && (element.Declaration.AccessModifierType != AccessModifierType.Private)) && (element.ParentElement.ElementType != ElementType.Struct))
            {
                CsElement parentElement = element.ParentElement;
                bool flag = false;
                bool flag2 = false;
                while (parentElement != null)
                {
                    if ((parentElement.ElementType != ElementType.Class) && (parentElement.ElementType != ElementType.Struct))
                    {
                        break;
                    }
                    if ((parentElement.ActualAccess == AccessModifierType.Private) || (parentElement.ActualAccess == AccessModifierType.Internal))
                    {
                        flag2 = true;
                    }
                    if (parentElement.Declaration.Name.EndsWith("NativeMethods", StringComparison.Ordinal))
                    {
                        flag = true;
                        break;
                    }
                    parentElement = parentElement.ParentElement;
                }
                if (!flag || !flag2)
                {
                    Field field = (Field) element;
                    if ((!field.Const && !field.Readonly) && !field.Generated)
                    {
                        base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.FieldsMustBePrivate, new object[0]);
                    }
                }
            }
        }

        private static void CheckFileContents(CsElement element, CsElement parentElement, TopLevelElements topLevelElements)
        {
            if (element.ElementType == ElementType.Class)
            {
                if (((parentElement == null) || (parentElement.ElementType == ElementType.Root)) || (parentElement.ElementType == ElementType.Namespace))
                {
                    topLevelElements.Classes.Add((Class) element);
                }
            }
            else if (element.ElementType == ElementType.Namespace)
            {
                topLevelElements.Namespaces.Add((Namespace) element);
            }
        }

        private void CheckForAccessModifier(CsElement element)
        {
            if (((element.ElementType == ElementType.Method) || (element.ElementType == ElementType.Property)) || ((element.ElementType == ElementType.Indexer) || (element.ElementType == ElementType.Event)))
            {
                if ((!element.Declaration.AccessModifier && !element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Partial })) && ((element.Name.IndexOf(".", StringComparison.Ordinal) == -1) || element.Name.StartsWith("this.", StringComparison.Ordinal)))
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.AccessModifierMustBeDeclared, new object[] { element.FriendlyTypeText });
                }
            }
            else if ((((element.ElementType == ElementType.Class) || (element.ElementType == ElementType.Field)) || ((element.ElementType == ElementType.Enum) || (element.ElementType == ElementType.Struct))) || ((element.ElementType == ElementType.Interface) || (element.ElementType == ElementType.Delegate)))
            {
                if (!element.Declaration.AccessModifier)
                {
                    base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.AccessModifierMustBeDeclared, new object[] { element.FriendlyTypeText });
                }
            }
            else if (((element.ElementType == ElementType.Constructor) && !element.Declaration.AccessModifier) && !element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }))
            {
                base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.AccessModifierMustBeDeclared, new object[] { element.FriendlyTypeText });
            }
        }

        private void CheckForEmptyElements(CsElement element)
        {
            if ((!element.Generated && (element.ElementType == ElementType.Constructor)) && (element.Declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Static }) && IsEmptyElement(element)))
            {
                base.AddViolation(element, Microsoft.StyleCop.CSharp.Rules.RemoveUnnecessaryCode, new object[] { element.FriendlyTypeText });
            }
        }

        private void CheckForUnnecessaryStatements(Statement statement, CsElement parentElement)
        {
            if (!parentElement.Generated)
            {
                if (((statement.StatementType == StatementType.Finally) || (statement.StatementType == StatementType.Checked)) || (((statement.StatementType == StatementType.Unchecked) || (statement.StatementType == StatementType.Lock)) || (statement.StatementType == StatementType.Unsafe)))
                {
                    if (IsEmptyParentOfBlockStatement(statement))
                    {
                        base.AddViolation(parentElement, statement.LineNumber, Microsoft.StyleCop.CSharp.Rules.RemoveUnnecessaryCode, new object[] { statement.FriendlyTypeText });
                    }
                }
                else if ((statement.StatementType == StatementType.Try) && IsUnnecessaryTryStatement((TryStatement) statement))
                {
                    base.AddViolation(parentElement, statement.LineNumber, Microsoft.StyleCop.CSharp.Rules.RemoveUnnecessaryCode, new object[] { statement.FriendlyTypeText });
                }
            }
        }

        private void CheckParenthesizedExpression(CsElement element, ParenthesizedExpression parenthesizedExpression)
        {
            if (parenthesizedExpression.InnerExpression != null)
            {
                Expression innerExpression = parenthesizedExpression.InnerExpression;
                if ((((((innerExpression.ExpressionType != ExpressionType.Arithmetic) && (innerExpression.ExpressionType != ExpressionType.As)) && ((innerExpression.ExpressionType != ExpressionType.Assignment) && (innerExpression.ExpressionType != ExpressionType.Cast))) && (((innerExpression.ExpressionType != ExpressionType.Conditional) && (innerExpression.ExpressionType != ExpressionType.ConditionalLogical)) && ((innerExpression.ExpressionType != ExpressionType.Decrement) && (innerExpression.ExpressionType != ExpressionType.Increment)))) && ((((innerExpression.ExpressionType != ExpressionType.Is) && (innerExpression.ExpressionType != ExpressionType.Lambda)) && ((innerExpression.ExpressionType != ExpressionType.Logical) && (innerExpression.ExpressionType != ExpressionType.New))) && (((innerExpression.ExpressionType != ExpressionType.NewArray) && (innerExpression.ExpressionType != ExpressionType.NullCoalescing)) && ((innerExpression.ExpressionType != ExpressionType.Query) && (innerExpression.ExpressionType != ExpressionType.Relational))))) && (innerExpression.ExpressionType != ExpressionType.Unary))
                {
                    base.AddViolation(element, parenthesizedExpression.LineNumber, Microsoft.StyleCop.CSharp.Rules.StatementMustNotUseUnnecessaryParenthesis, new object[0]);
                }
                else if (!(parenthesizedExpression.Parent is Expression) || (parenthesizedExpression.Parent is VariableDeclaratorExpression))
                {
                    base.AddViolation(element, parenthesizedExpression.LineNumber, Microsoft.StyleCop.CSharp.Rules.StatementMustNotUseUnnecessaryParenthesis, new object[0]);
                }
                else
                {
                    AssignmentExpression parent = parenthesizedExpression.Parent as AssignmentExpression;
                    if ((parent != null) && (parent.RightHandSide == parenthesizedExpression))
                    {
                        base.AddViolation(element, parenthesizedExpression.LineNumber, Microsoft.StyleCop.CSharp.Rules.StatementMustNotUseUnnecessaryParenthesis, new object[0]);
                    }
                }
            }
        }

        private static bool IsEmptyElement(CsElement element)
        {
            return (((element.ChildElements == null) || (element.ChildElements.Count <= 0)) && ((element.ChildStatements == null) || (element.ChildStatements.Count <= 0)));
        }

        private static bool IsEmptyParentOfBlockStatement(Statement statement)
        {
            foreach (Statement statement2 in statement.ChildStatements)
            {
                if (statement2.StatementType == StatementType.Block)
                {
                    if ((statement2.ChildStatements != null) && (statement2.ChildStatements.Count != 0))
                    {
                        break;
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool IsEmptyString(string text)
        {
            if (text.Length > 2)
            {
                if (text[0] != '@')
                {
                    return false;
                }
                if (text.Length > 3)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsSuppressMessage(MethodInvocationExpression expression)
        {
            Microsoft.StyleCop.Node<CsToken> first = expression.Name.Tokens.First;
            if (first != null)
            {
                if (first.Value.Text.Equals("SuppressMessage", StringComparison.Ordinal))
                {
                    return true;
                }
                if (first.Value.Text.Equals("System"))
                {
                    return expression.Name.Tokens.MatchTokens(new string[] { "System", ".", "Diagnostics", ".", "CodeAnalysis", ".", "SuppressMessage" });
                }
            }
            return false;
        }

        private static bool IsUnnecessaryTryStatement(TryStatement tryStatement)
        {
            return (IsEmptyParentOfBlockStatement(tryStatement) || (((tryStatement.CatchStatements == null) || (tryStatement.CatchStatements.Count == 0)) && ((tryStatement.FinallyStatement == null) || IsEmptyParentOfBlockStatement(tryStatement.FinallyStatement))));
        }

        private bool ProcessElement(CsElement element, CsElement parentElement, TopLevelElements topLevelElements)
        {
            this.CheckAccessModifierRulesForElement(element);
            this.CheckCodeAnalysisAttributeJustifications(element);
            this.CheckForEmptyElements(element);
            CheckFileContents(element, parentElement, topLevelElements);
            return true;
        }

        private bool ProcessExpression(Expression expression, Expression parentExpression, Statement parentStatement, CsElement parentElement, TopLevelElements context)
        {
            if (!parentElement.Generated)
            {
                if (expression.ExpressionType == ExpressionType.MethodInvocation)
                {
                    MethodInvocationExpression debugAssertMethodCall = (MethodInvocationExpression) expression;
                    if (debugAssertMethodCall.Name.Tokens.MatchTokens(new string[] { "Debug", ".", "Assert" }) || debugAssertMethodCall.Name.Tokens.MatchTokens(new string[] { "System", ".", "Diagnostics", ".", "Debug", ".", "Assert" }))
                    {
                        this.CheckDebugAssertMessage(parentElement, debugAssertMethodCall);
                    }
                    else if (debugAssertMethodCall.Name.Tokens.MatchTokens(new string[] { "Debug", ".", "Fail" }) || debugAssertMethodCall.Name.Tokens.MatchTokens(new string[] { "System", ".", "Diagnostics", ".", "Debug", ".", "Fail" }))
                    {
                        this.CheckDebugFailMessage(parentElement, debugAssertMethodCall);
                    }
                }
                else if (expression.ExpressionType == ExpressionType.Parenthesized)
                {
                    this.CheckParenthesizedExpression(parentElement, (ParenthesizedExpression) expression);
                }
                else if (expression.ExpressionType == ExpressionType.Arithmetic)
                {
                    this.CheckArithmeticExpressionParenthesis(parentElement, (ArithmeticExpression) expression);
                }
                else if (expression.ExpressionType == ExpressionType.ConditionalLogical)
                {
                    this.CheckConditionalLogicalExpressionParenthesis(parentElement, (ConditionalLogicalExpression) expression);
                }
                else if (expression.ExpressionType == ExpressionType.AnonymousMethod)
                {
                    this.CheckAnonymousMethodParenthesis(parentElement, (AnonymousMethodExpression) expression);
                }
            }
            return true;
        }

        private bool ProcessStatement(Statement statement, Expression parentExpression, Statement parentStatement, CsElement parentElement, TopLevelElements context)
        {
            this.CheckForUnnecessaryStatements(statement, parentElement);
            return true;
        }

        private class TopLevelElements
        {
            private List<Class> classes = new List<Class>();
            private List<Namespace> namespaces = new List<Namespace>();

            public ICollection<Class> Classes
            {
                get
                {
                    return this.classes;
                }
            }

            public ICollection<Namespace> Namespaces
            {
                get
                {
                    return this.namespaces;
                }
            }
        }
    }
}

