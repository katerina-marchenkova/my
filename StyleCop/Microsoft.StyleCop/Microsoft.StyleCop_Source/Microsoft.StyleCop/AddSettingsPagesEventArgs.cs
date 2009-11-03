namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;

    public class AddSettingsPagesEventArgs : EventArgs
    {
        private List<IPropertyControlPage> pages = new List<IPropertyControlPage>();
        private string settingsPath;

        internal AddSettingsPagesEventArgs(string settingsPath)
        {
            this.settingsPath = settingsPath;
        }

        public void Add(IPropertyControlPage page)
        {
            Param.RequireNotNull(page, "page");
            if (page != null)
            {
                this.pages.Add(page);
            }
        }

        public IEnumerable<IPropertyControlPage> Pages
        {
            get
            {
                return this.pages;
            }
        }

        public string SettingsPath
        {
            get
            {
                return this.settingsPath;
            }
        }
    }
}

