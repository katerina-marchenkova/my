namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public abstract class SourceCode
    {
        private IEnumerable<Configuration> configurations;
        private SourceParser parser;
        private CodeProject project;
        private Dictionary<string, Violation> violations;

        protected SourceCode(CodeProject project, SourceParser parser)
        {
            this.violations = new Dictionary<string, Violation>();
            Param.RequireNotNull(project, "project");
            Param.RequireNotNull(parser, "parser");
            this.project = project;
            this.parser = parser;
        }

        protected SourceCode(CodeProject project, SourceParser parser, IEnumerable<Configuration> configurations) : this(project, parser)
        {
            this.configurations = configurations;
        }

        internal bool AddViolation(Violation violation)
        {
            string key = violation.Key;
            if (!this.violations.ContainsKey(key))
            {
                this.violations.Add(violation.Key, violation);
                return true;
            }
            return false;
        }

        public abstract TextReader Read();

        public IEnumerable<Configuration> Configurations
        {
            get
            {
                return this.configurations;
            }
        }

        public abstract bool Exists { get; }

        public abstract string Name { get; }

        public SourceParser Parser
        {
            get
            {
                return this.parser;
            }
        }

        public abstract string Path { get; }

        public CodeProject Project
        {
            get
            {
                return this.project;
            }
        }

        public Microsoft.StyleCop.Settings Settings
        {
            get
            {
                if (this.project != null)
                {
                    return this.project.Settings;
                }
                return null;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="TimeStamp", Justification="API has already been published and should not be changed.")]
        public abstract DateTime TimeStamp { get; }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification="API has already been published and should not be changed.")]
        public abstract string Type { get; }

        public ICollection<Violation> Violations
        {
            get
            {
                return this.violations.Values;
            }
        }
    }
}

