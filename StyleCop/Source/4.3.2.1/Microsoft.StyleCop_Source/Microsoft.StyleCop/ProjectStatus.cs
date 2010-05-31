namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;

    internal class ProjectStatus
    {
        private Dictionary<string, ICollection<SourceAnalyzer>> analyzerLists = new Dictionary<string, ICollection<SourceAnalyzer>>();
        private bool ignoreResultsCache;

        internal Dictionary<string, ICollection<SourceAnalyzer>> AnalyzerLists
        {
            get
            {
                return this.analyzerLists;
            }
        }

        public bool IgnoreResultsCache
        {
            get
            {
                return this.ignoreResultsCache;
            }
            set
            {
                this.ignoreResultsCache = value;
            }
        }
    }
}

