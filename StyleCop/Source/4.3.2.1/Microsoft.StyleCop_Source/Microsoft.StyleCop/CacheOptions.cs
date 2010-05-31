namespace Microsoft.StyleCop
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    internal class CacheOptions : UserControl, IPropertyControlPage
    {
        private Container components = null;
        private bool dirty;
        private CheckBox enableCache;
        private Label label1;
        private BooleanProperty parentProperty;
        private PropertyControl tabControl;

        public CacheOptions()
        {
            this.InitializeComponent();
        }

        public void Activate(bool activated)
        {
        }

        public bool Apply()
        {
            this.tabControl.LocalSettings.GlobalSettings.SetProperty(new BooleanProperty(this.tabControl.Core, "WriteCache", this.enableCache.Checked));
            this.dirty = false;
            this.tabControl.DirtyChanged();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EnableCacheCheckedChanged(object sender, EventArgs e)
        {
            if (!this.dirty)
            {
                this.dirty = true;
                this.tabControl.DirtyChanged();
            }
            this.SetBoldState();
        }

        public void Initialize(PropertyControl propertyControl)
        {
            this.tabControl = propertyControl;
            PropertyDescriptor<bool> descriptor = this.tabControl.Core.PropertyDescriptors["WriteCache"] as PropertyDescriptor<bool>;
            this.parentProperty = (this.tabControl.ParentSettings == null) ? null : (this.tabControl.ParentSettings.GlobalSettings.GetProperty(descriptor.PropertyName) as BooleanProperty);
            BooleanProperty property = (this.tabControl.MergedSettings == null) ? null : (this.tabControl.MergedSettings.GlobalSettings.GetProperty(descriptor.PropertyName) as BooleanProperty);
            this.enableCache.Checked = (property == null) ? descriptor.DefaultValue : property.Value;
            this.SetBoldState();
            this.dirty = false;
            this.tabControl.DirtyChanged();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(CacheOptions));
            this.label1 = new Label();
            this.enableCache = new CheckBox();
            base.SuspendLayout();
            manager.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            manager.ApplyResources(this.enableCache, "enableCache");
            this.enableCache.Name = "enableCache";
            this.enableCache.UseVisualStyleBackColor = true;
            this.enableCache.CheckedChanged += new EventHandler(this.EnableCacheCheckedChanged);
            base.Controls.Add(this.enableCache);
            base.Controls.Add(this.label1);
            this.MinimumSize = new Size(0xf6, 80);
            base.Name = "CacheOptions";
            manager.ApplyResources(this, "$this");
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public void PostApply(bool wasDirty)
        {
        }

        public bool PreApply()
        {
            return true;
        }

        public void RefreshSettingsOverrideState()
        {
            this.parentProperty = (this.tabControl.ParentSettings == null) ? null : (this.tabControl.ParentSettings.GlobalSettings.GetProperty("WriteCache") as BooleanProperty);
            this.SetBoldState();
        }

        private void SetBoldState()
        {
            if ((this.parentProperty != null) && (this.enableCache.Checked != this.parentProperty.Value))
            {
                this.enableCache.Font = new Font(this.enableCache.Font, FontStyle.Bold);
            }
            else
            {
                this.enableCache.Font = new Font(this.enableCache.Font, FontStyle.Regular);
            }
        }

        public bool Dirty
        {
            get
            {
                return this.dirty;
            }
            set
            {
                if (this.dirty != value)
                {
                    this.dirty = value;
                    this.tabControl.DirtyChanged();
                }
            }
        }

        public string TabName
        {
            get
            {
                return Strings.CacheTab;
            }
        }
    }
}

