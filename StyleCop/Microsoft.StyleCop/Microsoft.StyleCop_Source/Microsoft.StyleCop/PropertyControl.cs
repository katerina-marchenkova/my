namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class PropertyControl : TabControl
    {
        private object[] context;
        private StyleCopCore core;
        private bool dirty;
        private IPropertyControlHost host;
        private WritableSettings localSettings;
        private Settings mergedSettings;
        private IList<IPropertyControlPage> pageInterfaces;
        private UserControl[] pages;
        private Settings parentSettings;
        private Microsoft.StyleCop.SettingsComparer settingsComparer;
        private TabPage[] tabPages;

        internal PropertyControl()
        {
            this.InitializeComponent();
        }

        internal PropertyControlSaveResult Apply(out bool dirtyPages)
        {
            dirtyPages = false;
            PropertyControlSaveResult success = PropertyControlSaveResult.Success;
            bool flag = false;
            bool[] flagArray = new bool[this.pageInterfaces.Count];
            for (int i = 0; i < this.pageInterfaces.Count; i++)
            {
                if (this.pageInterfaces[i] != null)
                {
                    flagArray[i] = this.pageInterfaces[i].Dirty;
                    if (!this.pageInterfaces[i].PreApply())
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                int num2 = -1;
                for (int j = 0; j < this.pageInterfaces.Count; j++)
                {
                    if ((this.pageInterfaces[j] != null) && this.pageInterfaces[j].Dirty)
                    {
                        dirtyPages = true;
                        if (!this.pageInterfaces[j].Apply())
                        {
                            success = PropertyControlSaveResult.PageAbort;
                            num2 = j;
                            break;
                        }
                    }
                }
                int num4 = (num2 == -1) ? (this.pageInterfaces.Count - 1) : num2;
                for (int k = 0; k <= num4; k++)
                {
                    if (this.pageInterfaces[k] != null)
                    {
                        this.pageInterfaces[k].PostApply(flagArray[k]);
                    }
                }
                if (num2 == -1)
                {
                    Exception exception = null;
                    if (!this.core.Environment.SaveSettings(this.localSettings, out exception))
                    {
                        AlertDialog.Show(this.core, this, string.Format(CultureInfo.CurrentUICulture, Strings.CouldNotSaveSettingsFile, new object[] { exception.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        success = PropertyControlSaveResult.SaveError;
                        for (int m = 0; m < this.pageInterfaces.Count; m++)
                        {
                            if (this.pageInterfaces[m] != null)
                            {
                                this.pageInterfaces[m].Dirty = flagArray[m];
                            }
                        }
                        return success;
                    }
                    this.dirty = false;
                    this.host.Dirty(false);
                }
            }
            return success;
        }

        public void Cancel()
        {
            this.host.Cancel();
        }

        public void DirtyChanged()
        {
            bool isDirty = false;
            foreach (IPropertyControlPage page in this.pageInterfaces)
            {
                if ((page != null) && page.Dirty)
                {
                    isDirty = true;
                    break;
                }
            }
            if (isDirty != this.dirty)
            {
                this.dirty = isDirty;
                this.host.Dirty(isDirty);
            }
        }

        internal void Initialize(IPropertyControlHost hostInstance, IList<IPropertyControlPage> propertyPages, WritableSettings settings, StyleCopCore coreInstance, params object[] contextItem)
        {
            if (this.host != null)
            {
                throw new StyleCopException(Strings.PropertyControlAlreadyInitialized);
            }
            this.host = hostInstance;
            this.pageInterfaces = propertyPages;
            this.localSettings = settings;
            this.core = coreInstance;
            this.context = contextItem;
            SettingsMerger merger = new SettingsMerger(this.localSettings, this.core.Environment);
            this.parentSettings = merger.ParentMergedSettings;
            this.mergedSettings = merger.MergedSettings;
            this.settingsComparer = new Microsoft.StyleCop.SettingsComparer(this.localSettings, this.parentSettings);
            if (this.context == null)
            {
                this.context = new object[0];
            }
            this.tabPages = new TabPage[propertyPages.Count];
            this.pages = new UserControl[propertyPages.Count];
            int index = 0;
            for (int i = 0; i < propertyPages.Count; i++)
            {
                this.pages[index] = (UserControl) this.pageInterfaces[i];
                TabPage page = new TabPage(this.pageInterfaces[i].TabName);
                this.tabPages[index] = page;
                page.Controls.Add(this.pages[i]);
                base.Controls.Add(page);
                this.pages[i].Dock = DockStyle.Fill;
                this.SizePage(i);
                this.pageInterfaces[i].Initialize(this);
                index++;
            }
            if (base.TabPages[0] != null)
            {
                base.SelectedTab = this.tabPages[0];
                this.pageInterfaces[0].Activate(true);
            }
            base.SizeChanged += new EventHandler(this.OnSizeChanged);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.Controls.AddRange(new Control[0]);
            base.Name = "PropertyControl";
            base.Size = new Size(0xf8, 0xd8);
            base.ResumeLayout(false);
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (base.SelectedIndex >= 0)
            {
                this.SizePage(base.SelectedIndex);
            }
        }

        public void RefreshMergedSettings()
        {
            SettingsMerger merger = new SettingsMerger(this.localSettings, this.core.Environment);
            this.parentSettings = merger.ParentMergedSettings;
            this.mergedSettings = merger.MergedSettings;
            this.settingsComparer = new Microsoft.StyleCop.SettingsComparer(this.localSettings, this.parentSettings);
            for (int i = 0; i < this.pageInterfaces.Count; i++)
            {
                if (this.pageInterfaces[i] != null)
                {
                    this.pageInterfaces[i].RefreshSettingsOverrideState();
                }
            }
        }

        private void SizePage(int index)
        {
            Rectangle tabRect = base.GetTabRect(index);
            this.tabPages[index].Height = tabRect.Height;
            this.tabPages[index].Width = tabRect.Width;
        }

        public IPropertyControlPage ActivePage
        {
            get
            {
                if (this.host != null)
                {
                    return this.pageInterfaces[base.SelectedIndex];
                }
                return null;
            }
        }

        public IList<object> Context
        {
            get
            {
                return this.context;
            }
        }

        public StyleCopCore Core
        {
            get
            {
                return this.core;
            }
        }

        public bool IsDirty
        {
            get
            {
                return this.dirty;
            }
        }

        public WritableSettings LocalSettings
        {
            get
            {
                return this.localSettings;
            }
        }

        public Settings MergedSettings
        {
            get
            {
                return this.mergedSettings;
            }
        }

        public IList<IPropertyControlPage> Pages
        {
            get
            {
                return this.pageInterfaces;
            }
        }

        public Settings ParentSettings
        {
            get
            {
                return this.parentSettings;
            }
        }

        public Microsoft.StyleCop.SettingsComparer SettingsComparer
        {
            get
            {
                return this.settingsComparer;
            }
        }
    }
}

