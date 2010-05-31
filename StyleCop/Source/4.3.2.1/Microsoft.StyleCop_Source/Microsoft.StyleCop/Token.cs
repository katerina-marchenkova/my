namespace Microsoft.StyleCop
{
    using System;

    public class Token
    {
        private bool generated;
        private CodeLocation location;
        private string text;

        public Token(CodeLocation location, bool generated)
        {
            Param.RequireNotNull(location, "location");
            this.location = location;
            this.generated = generated;
        }

        public Token(string text, CodeLocation location, bool generated) : this(location, generated)
        {
            Param.RequireNotNull(text, "text");
            this.text = text;
        }

        protected virtual void CreateTextString()
        {
        }

        public override string ToString()
        {
            return this.Text;
        }

        public bool Generated
        {
            get
            {
                return this.generated;
            }
        }

        public virtual int LineNumber
        {
            get
            {
                return this.location.StartPoint.LineNumber;
            }
        }

        public virtual CodeLocation Location
        {
            get
            {
                return this.location;
            }
        }

        public virtual string Text
        {
            get
            {
                if (this.text == null)
                {
                    this.CreateTextString();
                    if (this.text == null)
                    {
                        this.text = string.Empty;
                    }
                }
                return this.text;
            }
            protected set
            {
                this.text = value;
            }
        }
    }
}

