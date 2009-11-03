namespace Microsoft.StyleCop
{
    using Microsoft.Build.Framework;
    using System;
    using System.Runtime.CompilerServices;

    public class OutputEventArgs : EventArgs
    {
        [CompilerGenerated]
        private MessageImportance _Importance_k__BackingField;
        [CompilerGenerated]
        private string _Output_k__BackingField;

        public OutputEventArgs(string text) : this(text, MessageImportance.Normal)
        {
            Param.RequireNotNull(text, "text");
        }

        public OutputEventArgs(string text, MessageImportance importance)
        {
            Param.RequireNotNull(text, "text");
            this.Output = text;
            this.Importance = importance;
        }

        public MessageImportance Importance
        {
            [CompilerGenerated]
            get
            {
                return this._Importance_k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this._Importance_k__BackingField = value;
            }
        }

        public string Output
        {
            [CompilerGenerated]
            get
            {
                return this._Output_k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this._Output_k__BackingField = value;
            }
        }
    }
}

