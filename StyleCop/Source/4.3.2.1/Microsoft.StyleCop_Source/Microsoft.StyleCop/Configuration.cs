namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification="Configuration is an appropriate name for the class.")]
    public class Configuration
    {
        private Dictionary<string, string> conditionalCompilationDefinitions;

        public Configuration(string[] conditionalCompilationDefinitions)
        {
            if ((conditionalCompilationDefinitions != null) && (conditionalCompilationDefinitions.Length > 0))
            {
                this.conditionalCompilationDefinitions = new Dictionary<string, string>();
                foreach (string str in conditionalCompilationDefinitions)
                {
                    if (!this.conditionalCompilationDefinitions.ContainsKey(str))
                    {
                        this.conditionalCompilationDefinitions.Add(str, str);
                    }
                }
            }
        }

        public bool Contains(string definition)
        {
            Param.RequireNotNull(definition, "definition");
            return ((this.conditionalCompilationDefinitions != null) && this.conditionalCompilationDefinitions.ContainsKey(definition));
        }

        public string GetValue(string definition)
        {
            Param.RequireNotNull(definition, "definition");
            if (this.conditionalCompilationDefinitions != null)
            {
                return this.conditionalCompilationDefinitions[definition];
            }
            return null;
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId="Flags", Justification="API has already been published and should not be changed.")]
        public ICollection<string> Flags
        {
            get
            {
                if (this.conditionalCompilationDefinitions != null)
                {
                    return this.conditionalCompilationDefinitions.Keys;
                }
                return new string[0];
            }
        }
    }
}

