namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class CompanyInformation : UserControl, IPropertyControlPage
    {
        private SourceAnalyzer analyzer;
        private CheckBox checkBox;
        private TextBox companyName;
        private Label companyNameLabel;
        private IContainer components = null;
        private TextBox copyright;
        private Label copyrightLabel;
        private bool dirty;
        private PropertyControl tabControl;

        public CompanyInformation()
        {
            this.InitializeComponent();
        }

        public CompanyInformation(DocumentationRules analyzer) : this()
        {
            Param.RequireNotNull(analyzer, "analyzer");
            this.analyzer = analyzer;
        }

        public void Activate(bool activated)
        {
        }

        public bool Apply()
        {
            if (this.analyzer != null)
            {
                if (!this.checkBox.Checked)
                {
                    this.analyzer.ClearSetting(this.tabControl.LocalSettings, "CompanyName");
                    this.analyzer.ClearSetting(this.tabControl.LocalSettings, "Copyright");
                }
                else
                {
                    if ((this.companyName.Text.Length == 0) || (this.copyright.Text.Length == 0))
                    {
                        AlertDialog.Show(this.tabControl.Core, this, Microsoft.StyleCop.CSharp.Strings.MissingCompanyOrCopyright, Microsoft.StyleCop.CSharp.Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return false;
                    }
                    this.analyzer.SetSetting(this.tabControl.LocalSettings, new StringProperty(this.analyzer, "CompanyName", this.companyName.Text));
                    this.analyzer.SetSetting(this.tabControl.LocalSettings, new StringProperty(this.analyzer, "Copyright", this.copyright.Text));
                }
            }
            this.dirty = false;
            this.tabControl.DirtyChanged();
            return true;
        }

        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this.dirty = true;
            this.tabControl.DirtyChanged();
            this.companyName.Enabled = this.checkBox.Checked;
            this.copyright.Enabled = this.checkBox.Checked;
        }

        private void CompanyNameTextChanged(object sender, EventArgs e)
        {
            this.DetectCompanyNameBoldState();
            this.dirty = true;
            this.tabControl.DirtyChanged();
        }

        private void CopyrightTextChanged(object sender, EventArgs e)
        {
            this.DetectCopyrightBoldState();
            this.dirty = true;
            this.tabControl.DirtyChanged();
        }

        private void DetectBoldState()
        {
            this.DetectCompanyNameBoldState();
            this.DetectCopyrightBoldState();
        }

        private void DetectCompanyNameBoldState()
        {
            if (this.analyzer != null)
            {
                StringProperty localProperty = new StringProperty(this.analyzer, "CompanyName", this.companyName.Text);
                this.SetBoldState(this.companyName, this.tabControl.SettingsComparer.IsAddInSettingOverwritten(this.analyzer, "CompanyName", localProperty));
            }
        }

        private void DetectCopyrightBoldState()
        {
            StringProperty localProperty = new StringProperty(this.analyzer, "Copyright", this.copyright.Text);
            this.SetBoldState(this.copyright, this.tabControl.SettingsComparer.IsAddInSettingOverwritten(this.analyzer, "Copyright", localProperty));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void Initialize(PropertyControl propertyControl)
        {
            Param.RequireNotNull(propertyControl, "propertyControl");
            this.tabControl = propertyControl;
            this.InitializeSettings();
            this.DetectBoldState();
            this.dirty = false;
            this.tabControl.DirtyChanged();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(CompanyInformation));
            this.checkBox = new CheckBox();
            this.companyNameLabel = new Label();
            this.copyrightLabel = new Label();
            this.companyName = new TextBox();
            this.copyright = new TextBox();
            base.SuspendLayout();
            manager.ApplyResources(this.checkBox, "checkBox");
            this.checkBox.Name = "checkBox";
            this.checkBox.UseVisualStyleBackColor = true;
            this.checkBox.CheckedChanged += new EventHandler(this.CheckBoxCheckedChanged);
            manager.ApplyResources(this.companyNameLabel, "companyNameLabel");
            this.companyNameLabel.Name = "companyNameLabel";
            manager.ApplyResources(this.copyrightLabel, "copyrightLabel");
            this.copyrightLabel.Name = "copyrightLabel";
            manager.ApplyResources(this.companyName, "companyName");
            this.companyName.Name = "companyName";
            this.companyName.TextChanged += new EventHandler(this.CompanyNameTextChanged);
            manager.ApplyResources(this.copyright, "copyright");
            this.copyright.Name = "copyright";
            this.copyright.TextChanged += new EventHandler(this.CopyrightTextChanged);
            manager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.copyright);
            base.Controls.Add(this.companyName);
            base.Controls.Add(this.copyrightLabel);
            base.Controls.Add(this.companyNameLabel);
            base.Controls.Add(this.checkBox);
            base.Name = "CompanyInformation";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitializeSettings()
        {
            if (this.analyzer != null)
            {
                StringProperty setting = this.analyzer.GetSetting(this.tabControl.MergedSettings, "CompanyName") as StringProperty;
                if (setting != null)
                {
                    this.companyName.Text = setting.Value;
                }
                StringProperty property2 = this.analyzer.GetSetting(this.tabControl.MergedSettings, "Copyright") as StringProperty;
                if (property2 != null)
                {
                    this.copyright.Text = property2.Value;
                }
                this.checkBox.Checked = (setting != null) || (property2 != null);
                this.CheckBoxCheckedChanged(this.checkBox, new EventArgs());
            }
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
            if (!this.dirty)
            {
                this.InitializeSettings();
            }
            this.DetectBoldState();
        }

        private void SetBoldState(TextBox item, bool bold)
        {
            if ((item.Font != this.Font) && (item.Font != null))
            {
                item.Font.Dispose();
            }
            if (bold)
            {
                item.Font = new Font(this.Font, FontStyle.Bold);
            }
            else
            {
                item.Font = new Font(this.Font, FontStyle.Regular);
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
                return Microsoft.StyleCop.CSharp.Strings.CompanyInformationTab;
            }
        }
    }
}

