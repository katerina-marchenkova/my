namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification="Camel case better serves in this case.")]
    public abstract class CsElement : CodeElement, IWriteableCodeUnit, ICodeUnit
    {
        private AccessModifierType actualAccess;
        private ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes;
        private CodeUnit codeUnit;
        private Microsoft.StyleCop.CSharp.Declaration declaration;
        private CsDocument document;
        private CodeUnitCollection<CsElement> elements;
        private static CsElement[] emptyElementArray = new CsElement[0];
        private string fullNamespaceName;
        private string fullyQualifiedBase;
        private string fullyQualifiedName;
        private XmlHeader header;
        private Microsoft.StyleCop.CSharp.ElementType type;
        private bool unsafeCode;

        internal CsElement(CsDocument document, CsElement parent, Microsoft.StyleCop.CSharp.ElementType type, string name, XmlHeader header, ICollection<Microsoft.StyleCop.CSharp.Attribute> attributes, Microsoft.StyleCop.CSharp.Declaration declaration, bool unsafeCode, bool generated) : base(document, name, generated)
        {
            this.codeUnit = new CodeUnit(Microsoft.StyleCop.CSharp.CodeUnitType.Element);
            this.document = document;
            if (this.document == null)
            {
                throw new ArgumentException(Microsoft.StyleCop.CSharp.Strings.DocumentMustBeCsDocument, "document");
            }
            if ((parent != null) && (parent.Document != document))
            {
                throw new ArgumentException(Microsoft.StyleCop.CSharp.Strings.ElementMustBeInParentsDocument, "parent");
            }
            this.type = type;
            this.header = header;
            this.attributes = attributes;
            this.declaration = declaration;
            this.unsafeCode = unsafeCode;
            if (!unsafeCode && this.declaration.ContainsModifier(new CsTokenType[] { CsTokenType.Unsafe }))
            {
                this.unsafeCode = true;
            }
            if (this.header != null)
            {
                this.header.Element = this;
            }
            if (this.attributes != null)
            {
                foreach (Microsoft.StyleCop.CSharp.Attribute attribute in this.attributes)
                {
                    attribute.Element = this;
                }
            }
            if (parent != null)
            {
                this.fullyQualifiedBase = parent.FullyQualifiedName;
                this.MergeAccess(parent.ActualAccess);
            }
            else if (this.declaration != null)
            {
                this.actualAccess = this.declaration.AccessModifierType;
            }
            else
            {
                this.actualAccess = AccessModifierType.Public;
            }
            this.fullyQualifiedName = this.fullyQualifiedBase;
            if (((this.declaration != null) && (this.declaration.Name != null)) && (this.declaration.Name.Length > 0))
            {
                if ((this.fullyQualifiedBase != null) && (this.fullyQualifiedBase.Length > 0))
                {
                    this.fullyQualifiedName = this.fullyQualifiedName + ".";
                }
                int index = this.declaration.Name.LastIndexOf(@"\", StringComparison.Ordinal);
                if (index != -1)
                {
                    this.fullyQualifiedName = this.fullyQualifiedName + this.declaration.Name.Substring(index + 1, (this.declaration.Name.Length - index) - 1);
                }
                else
                {
                    this.fullyQualifiedName = this.fullyQualifiedName + this.declaration.Name;
                }
                index = this.fullyQualifiedName.IndexOf(".cs.", StringComparison.OrdinalIgnoreCase);
                if (-1 == index)
                {
                    this.fullNamespaceName = this.fullyQualifiedName;
                }
                else
                {
                    this.fullNamespaceName = this.fullyQualifiedName.Substring(index + 4, (this.fullyQualifiedName.Length - index) - 4);
                }
            }
            if (type == Microsoft.StyleCop.CSharp.ElementType.Root)
            {
                this.codeUnit.TrimTokens = false;
            }
        }

        internal virtual void AddElement(CsElement element)
        {
            if (this.elements == null)
            {
                this.elements = new CodeUnitCollection<CsElement>(this);
            }
            this.elements.Add(element);
        }

        internal virtual void AddExpression(Expression expression)
        {
            this.codeUnit.AddExpression(expression);
        }

        internal virtual void AddStatement(Statement statement)
        {
            this.codeUnit.AddStatement(statement);
        }

        internal virtual void AddStatements(IEnumerable<Statement> statements)
        {
            this.codeUnit.AddStatements(statements);
        }

        internal virtual void Initialize()
        {
        }

        private void MergeAccess(AccessModifierType parentAccess)
        {
            AccessModifierType accessModifierType = this.declaration.AccessModifierType;
            if (parentAccess == AccessModifierType.Public)
            {
                this.actualAccess = accessModifierType;
            }
            else if (parentAccess == AccessModifierType.ProtectedInternal)
            {
                if (accessModifierType == AccessModifierType.Public)
                {
                    this.actualAccess = AccessModifierType.ProtectedInternal;
                }
                else
                {
                    this.actualAccess = accessModifierType;
                }
            }
            else if (parentAccess == AccessModifierType.Protected)
            {
                switch (accessModifierType)
                {
                    case AccessModifierType.Public:
                    case AccessModifierType.ProtectedInternal:
                        this.actualAccess = AccessModifierType.Protected;
                        return;

                    case AccessModifierType.Internal:
                        this.actualAccess = AccessModifierType.ProtectedAndInternal;
                        return;
                }
                this.actualAccess = accessModifierType;
            }
            else if (parentAccess == AccessModifierType.Internal)
            {
                switch (accessModifierType)
                {
                    case AccessModifierType.Public:
                    case AccessModifierType.ProtectedInternal:
                        this.actualAccess = AccessModifierType.Internal;
                        return;

                    case AccessModifierType.Protected:
                        this.actualAccess = AccessModifierType.ProtectedAndInternal;
                        return;
                }
                this.actualAccess = accessModifierType;
            }
            else if (parentAccess == AccessModifierType.ProtectedAndInternal)
            {
                switch (accessModifierType)
                {
                    case AccessModifierType.Public:
                    case AccessModifierType.ProtectedInternal:
                    case AccessModifierType.Protected:
                    case AccessModifierType.Internal:
                        this.actualAccess = AccessModifierType.ProtectedAndInternal;
                        return;
                }
                this.actualAccess = accessModifierType;
            }
            else
            {
                this.actualAccess = AccessModifierType.Private;
            }
        }

        void IWriteableCodeUnit.AddExpression(Expression expression)
        {
            throw new NotSupportedException();
        }

        void IWriteableCodeUnit.AddExpressions(IEnumerable<Expression> expressions)
        {
            throw new NotSupportedException();
        }

        void IWriteableCodeUnit.AddStatement(Statement statement)
        {
            this.AddStatement(statement);
        }

        void IWriteableCodeUnit.AddStatements(IEnumerable<Statement> statements)
        {
            this.AddStatements(statements);
        }

        void IWriteableCodeUnit.SetParent(ICodeUnit parent)
        {
            ((IWriteableCodeUnit) this.codeUnit).SetParent(parent);
        }

        public void WalkElement(CodeWalkerElementVisitor<object> elementCallback)
        {
            this.WalkElement<object>(elementCallback, null, null, null, null);
        }

        public void WalkElement<T>(CodeWalkerElementVisitor<T> elementCallback, T context)
        {
            this.WalkElement<T>(elementCallback, null, null, null, context);
        }

        public void WalkElement(CodeWalkerElementVisitor<object> elementCallback, CodeWalkerStatementVisitor<object> statementCallback)
        {
            this.WalkElement<object>(elementCallback, statementCallback, null, null, null);
        }

        public void WalkElement(CodeWalkerElementVisitor<object> elementCallback, CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback)
        {
            this.WalkElement<object>(elementCallback, statementCallback, expressionCallback, null, null);
        }

        public void WalkElement<T>(CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, T context)
        {
            this.WalkElement<T>(elementCallback, statementCallback, null, null, context);
        }

        public void WalkElement<T>(CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, T context)
        {
            this.WalkElement<T>(elementCallback, statementCallback, expressionCallback, null, context);
        }

        public void WalkElement(CodeWalkerElementVisitor<object> elementCallback, CodeWalkerStatementVisitor<object> statementCallback, CodeWalkerExpressionVisitor<object> expressionCallback, CodeWalkerQueryClauseVisitor<object> queryClauseCallback)
        {
            CodeWalker<object>.Start(this, elementCallback, statementCallback, expressionCallback, queryClauseCallback, null);
        }

        public void WalkElement<T>(CodeWalkerElementVisitor<T> elementCallback, CodeWalkerStatementVisitor<T> statementCallback, CodeWalkerExpressionVisitor<T> expressionCallback, CodeWalkerQueryClauseVisitor<T> queryClauseCallback, T context)
        {
            CodeWalker<T>.Start(this, elementCallback, statementCallback, expressionCallback, queryClauseCallback, context);
        }

        public AccessModifierType AccessModifier
        {
            get
            {
                if (this.declaration != null)
                {
                    return this.declaration.AccessModifierType;
                }
                return this.actualAccess;
            }
        }

        public AccessModifierType ActualAccess
        {
            get
            {
                return this.actualAccess;
            }
        }

        public ICollection<Microsoft.StyleCop.CSharp.Attribute> Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        public override IEnumerable<CodeElement> ChildCodeElements
        {
            get
            {
                return new EnumerableAdapter<CsElement, CodeElement>(this.elements, delegate (CsElement item) {
                    return item;
                });
            }
        }

        public ICollection<CsElement> ChildElements
        {
            get
            {
                if (this.elements == null)
                {
                    return emptyElementArray;
                }
                return this.elements;
            }
        }

        public ICollection<Expression> ChildExpressions
        {
            get
            {
                return this.codeUnit.ChildExpressions;
            }
        }

        public ICollection<Statement> ChildStatements
        {
            get
            {
                return this.codeUnit.ChildStatements;
            }
        }

        public Microsoft.StyleCop.CSharp.CodeUnitType CodeUnitType
        {
            get
            {
                return Microsoft.StyleCop.CSharp.CodeUnitType.Element;
            }
        }

        public Microsoft.StyleCop.CSharp.Declaration Declaration
        {
            get
            {
                return this.declaration;
            }
        }

        public new CsDocument Document
        {
            get
            {
                return this.document;
            }
        }

        public override IEnumerable<Token> ElementTokens
        {
            get
            {
                return new EnumerableAdapter<CsToken, Token>(this.codeUnit.Tokens, delegate (CsToken token) {
                    return token;
                });
            }
        }

        public Microsoft.StyleCop.CSharp.ElementType ElementType
        {
            get
            {
                return this.type;
            }
        }

        public string FriendlyPluralTypeText
        {
            get
            {
                string friendlyPluralTypeText = this.codeUnit.GetFriendlyPluralTypeText(null);
                if (friendlyPluralTypeText != null)
                {
                    return friendlyPluralTypeText;
                }
                return this.codeUnit.GetFriendlyPluralTypeText(base.GetType().Name);
            }
        }

        public string FriendlyTypeText
        {
            get
            {
                string friendlyTypeText = this.codeUnit.GetFriendlyTypeText(null);
                if (friendlyTypeText != null)
                {
                    return friendlyTypeText;
                }
                return this.codeUnit.GetFriendlyTypeText(base.GetType().Name);
            }
        }

        public string FullNamespaceName
        {
            get
            {
                return this.fullNamespaceName;
            }
        }

        public override string FullyQualifiedName
        {
            get
            {
                return this.fullyQualifiedName;
            }
        }

        public XmlHeader Header
        {
            get
            {
                return this.header;
            }
        }

        public override int LineNumber
        {
            get
            {
                if ((this.codeUnit.Location != null) && (this.codeUnit.Location.StartPoint.LineNumber >= 1))
                {
                    return this.codeUnit.Location.StartPoint.LineNumber;
                }
                return 0;
            }
        }

        public CodeLocation Location
        {
            get
            {
                return this.codeUnit.Location;
            }
            internal set
            {
                this.codeUnit.Location = value;
            }
        }

        public ICodeUnit Parent
        {
            get
            {
                return this.codeUnit.Parent;
            }
        }

        public CsElement ParentElement
        {
            get
            {
                return this.codeUnit.ParentElement;
            }
        }

        public Expression ParentExpression
        {
            get
            {
                return null;
            }
        }

        public Statement ParentStatement
        {
            get
            {
                return null;
            }
        }

        protected string QualifiedName
        {
            get
            {
                return this.fullyQualifiedName;
            }
            set
            {
                Param.RequireValidString(value, "QualifiedName");
                this.fullyQualifiedName = value;
            }
        }

        public CsTokenList Tokens
        {
            get
            {
                return this.codeUnit.Tokens;
            }
            internal set
            {
                this.codeUnit.Tokens = value;
            }
        }

        public bool Unsafe
        {
            get
            {
                return this.unsafeCode;
            }
        }

        public VariableCollection Variables
        {
            get
            {
                return this.codeUnit.Variables;
            }
        }
    }
}

