namespace Microsoft.StyleCop
{
    using System;

    public class Rule
    {
        private bool canDisable;
        private string checkId;
        private string context;
        private string description;
        private bool enabledByDefault;
        private string name;
        private string @namespace;
        private string ruleGroup;
        private bool warning;

        internal Rule(string name, string @namespace, string checkId, string context, bool warning) : this(name, @namespace, checkId, context, warning, string.Empty, null, true, false)
        {
        }

        internal Rule(string name, string @namespace, string checkId, string context, bool warning, string description, string ruleGroup, bool enabledByDefault, bool canDisable)
        {
            this.enabledByDefault = true;
            if (!enabledByDefault && !canDisable)
            {
                throw new ArgumentException(Strings.RuleDisabledByDefaultAndNotCanDisable);
            }
            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]))
                {
                    throw new ArgumentException(Strings.RuleNameInvalid, name);
                }
            }
            char c = name[0];
            if (!char.IsUpper(c))
            {
                throw new ArgumentException(Strings.RuleNameInvalid, name);
            }
            if ((((checkId.Length != 6) || !char.IsLetter(checkId[0])) || (!char.IsUpper(checkId[0]) || !char.IsLetter(checkId[1]))) || ((!char.IsUpper(checkId[1]) || !char.IsDigit(checkId[2])) || ((!char.IsDigit(checkId[3]) || !char.IsDigit(checkId[4])) || !char.IsDigit(checkId[5]))))
            {
                throw new ArgumentException(Strings.RuleCheckIdInvalid, checkId);
            }
            this.name = name;
            this.@namespace = @namespace;
            this.checkId = checkId;
            this.context = context;
            this.warning = warning;
            this.description = description;
            this.ruleGroup = ruleGroup;
            this.enabledByDefault = enabledByDefault;
            this.canDisable = canDisable;
        }

        public bool CanDisable
        {
            get
            {
                return this.canDisable;
            }
        }

        public string CheckId
        {
            get
            {
                return this.checkId;
            }
        }

        public string Context
        {
            get
            {
                return this.context;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public bool EnabledByDefault
        {
            get
            {
                return this.enabledByDefault;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Namespace
        {
            get
            {
                return this.@namespace;
            }
        }

        public string RuleGroup
        {
            get
            {
                return this.ruleGroup;
            }
        }

        public bool Warning
        {
            get
            {
                return this.warning;
            }
        }
    }
}

