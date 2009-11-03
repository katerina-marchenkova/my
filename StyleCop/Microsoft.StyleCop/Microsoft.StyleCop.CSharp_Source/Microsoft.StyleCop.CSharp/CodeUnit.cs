namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class CodeUnit : IWriteableCodeUnit, ICodeUnit
    {
        private Microsoft.StyleCop.CSharp.CodeUnitType codeUnitType;
        private static Expression[] emptyExpressionArray = new Expression[0];
        private static Statement[] emptyStatementArray = new Statement[0];
        private static CsTokenList emptyTokenList;
        private CodeUnitCollection<Expression> expressions;
        private string friendlyPluralTypeName;
        private string friendlyTypeName;
        private CodeLocation location;
        private ICodeUnit parent;
        private CodeUnitCollection<Statement> statements;
        private CsTokenList tokens;
        private bool trimTokens;
        private VariableCollection variables;

        internal CodeUnit(Microsoft.StyleCop.CSharp.CodeUnitType codeUnitType)
        {
            this.variables = new VariableCollection();
            this.trimTokens = true;
            this.codeUnitType = codeUnitType;
        }

        internal CodeUnit(Microsoft.StyleCop.CSharp.CodeUnitType codeUnitType, CsTokenList tokens) : this(codeUnitType)
        {
            this.tokens = tokens;
            this.tokens.Trim();
        }

        internal void AddExpression(Expression expression)
        {
            if (this.expressions == null)
            {
                this.expressions = new CodeUnitCollection<Expression>(this);
            }
            this.expressions.Add(expression);
        }

        internal void AddExpressions(IEnumerable<Expression> items)
        {
            if (this.expressions == null)
            {
                this.expressions = new CodeUnitCollection<Expression>(this);
            }
            this.expressions.AddRange(items);
        }

        internal void AddStatement(Statement statement)
        {
            if (this.statements == null)
            {
                this.statements = new CodeUnitCollection<Statement>(this);
            }
            this.statements.Add(statement);
        }

        internal void AddStatements(IEnumerable<Statement> items)
        {
            if (this.statements == null)
            {
                this.statements = new CodeUnitCollection<Statement>(this);
            }
            this.statements.AddRange(items);
        }

        internal string GetFriendlyPluralTypeText(string typeName)
        {
            if ((this.friendlyPluralTypeName == null) && (typeName != null))
            {
                this.friendlyPluralTypeName = TypeNames.ResourceManager.GetString(typeName + "Plural", TypeNames.Culture);
            }
            return this.friendlyPluralTypeName;
        }

        internal string GetFriendlyTypeText(string typeName)
        {
            if ((this.friendlyTypeName == null) && (typeName != null))
            {
                this.friendlyTypeName = TypeNames.ResourceManager.GetString(typeName, TypeNames.Culture);
            }
            return this.friendlyTypeName;
        }

        void IWriteableCodeUnit.AddExpression(Expression expression)
        {
            this.AddExpression(expression);
        }

        void IWriteableCodeUnit.AddExpressions(IEnumerable<Expression> items)
        {
            this.AddExpressions(items);
        }

        void IWriteableCodeUnit.AddStatement(Statement statement)
        {
            this.AddStatement(statement);
        }

        void IWriteableCodeUnit.AddStatements(IEnumerable<Statement> items)
        {
            this.AddStatements(items);
        }

        void IWriteableCodeUnit.SetParent(ICodeUnit parentCodeUnit)
        {
            this.parent = parentCodeUnit;
        }

        public ICollection<Expression> ChildExpressions
        {
            get
            {
                if (this.expressions == null)
                {
                    return emptyExpressionArray;
                }
                return this.expressions;
            }
        }

        public ICollection<Statement> ChildStatements
        {
            get
            {
                if (this.statements == null)
                {
                    return emptyStatementArray;
                }
                return this.statements;
            }
        }

        public Microsoft.StyleCop.CSharp.CodeUnitType CodeUnitType
        {
            get
            {
                return this.codeUnitType;
            }
        }

        public string FriendlyPluralTypeText
        {
            get
            {
                string friendlyPluralTypeText = this.GetFriendlyPluralTypeText(null);
                if (friendlyPluralTypeText != null)
                {
                    return friendlyPluralTypeText;
                }
                return this.GetFriendlyPluralTypeText(base.GetType().Name);
            }
        }

        public string FriendlyTypeText
        {
            get
            {
                string friendlyTypeText = this.GetFriendlyTypeText(null);
                if (friendlyTypeText != null)
                {
                    return friendlyTypeText;
                }
                return this.GetFriendlyTypeText(base.GetType().Name);
            }
        }

        public virtual int LineNumber
        {
            get
            {
                return this.Location.StartPoint.LineNumber;
            }
        }

        public virtual CodeLocation Location
        {
            get
            {
                if (this.location == null)
                {
                    this.location = CodeLocation.Join<CsToken>(this.tokens.First, this.tokens.Last);
                }
                return this.location;
            }
            internal set
            {
                this.location = value;
            }
        }

        public ICodeUnit Parent
        {
            get
            {
                return this.parent;
            }
        }

        public CsElement ParentElement
        {
            get
            {
                for (ICodeUnit unit = this.parent; unit != null; unit = unit.Parent)
                {
                    if (unit.CodeUnitType == Microsoft.StyleCop.CSharp.CodeUnitType.Element)
                    {
                        return (CsElement) unit;
                    }
                }
                return null;
            }
        }

        public Expression ParentExpression
        {
            get
            {
                for (ICodeUnit unit = this.parent; unit != null; unit = unit.Parent)
                {
                    if (unit.CodeUnitType == Microsoft.StyleCop.CSharp.CodeUnitType.Expression)
                    {
                        return (Expression) unit;
                    }
                    if (unit.CodeUnitType == Microsoft.StyleCop.CSharp.CodeUnitType.Element)
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        public Statement ParentStatement
        {
            get
            {
                for (ICodeUnit unit = this.parent; unit != null; unit = unit.Parent)
                {
                    if (unit.CodeUnitType == Microsoft.StyleCop.CSharp.CodeUnitType.Statement)
                    {
                        return (Statement) unit;
                    }
                    if (unit.CodeUnitType == Microsoft.StyleCop.CSharp.CodeUnitType.Element)
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        public virtual CsTokenList Tokens
        {
            get
            {
                if (this.tokens != null)
                {
                    return this.tokens;
                }
                if (emptyTokenList == null)
                {
                    emptyTokenList = new CsTokenList(new MasterList<CsToken>());
                }
                return emptyTokenList;
            }
            internal set
            {
                this.tokens = value;
                if (this.trimTokens)
                {
                    this.tokens.Trim();
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="A property should always have a get accessor.")]
        internal bool TrimTokens
        {
            get
            {
                return this.trimTokens;
            }
            set
            {
                this.trimTokens = value;
            }
        }

        public VariableCollection Variables
        {
            get
            {
                return this.variables;
            }
        }
    }
}

