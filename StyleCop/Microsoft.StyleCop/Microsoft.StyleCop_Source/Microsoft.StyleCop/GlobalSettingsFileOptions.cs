namespace Microsoft.StyleCop
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Windows.Forms;
    using System.Xml;

    internal class GlobalSettingsFileOptions : UserControl, IPropertyControlPage
    {
        private Button browse;
        private Container components = null;
        private bool dirty;
        private bool disableLinking;
        private Button editLinkedSettingsFile;
        private Button editParentSettingsFile;
        private Label label1;
        private TextBox linkedFilePath;
        private Label locationLabel;
        private RadioButton mergeWithLinkedFile;
        private RadioButton mergeWithParents;
        private RadioButton noMerge;
        private PropertyControl tabControl;

        public GlobalSettingsFileOptions()
        {
            this.InitializeComponent();
        }

        public void Activate(bool activated)
        {
        }

        public bool Apply()
        {
            if (this.mergeWithLinkedFile.Checked)
            {
                bool flag = false;
                try
                {
                    if (File.Exists(this.linkedFilePath.Text))
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(this.linkedFilePath.Text);
                        if ((document.DocumentElement.Name == "StyleCopSettings") || (document.DocumentElement.Name == "SourceAnalysisSettings"))
                        {
                            flag = true;
                        }
                    }
                }
                catch (ArgumentException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
                catch (XmlException)
                {
                }
                if (!flag)
                {
                    AlertDialog.Show(this.tabControl.Core, this, Strings.NoLinkedSettingsFile, Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return false;
                }
                string text = this.linkedFilePath.Text;
                if (!text.StartsWith(".", StringComparison.Ordinal))
                {
                    string location = this.tabControl.LocalSettings.Location;
                    if (!location.EndsWith(@"\", StringComparison.Ordinal))
                    {
                        location = location + @"\";
                    }
                    Uri uri = new Uri(location);
                    text = ConvertBackslashes(uri.MakeRelativeUri(new Uri(this.linkedFilePath.Text)).OriginalString);
                }
                this.tabControl.LocalSettings.GlobalSettings.SetProperty(new StringProperty(this.tabControl.Core, "LinkedSettingsFile", text));
                this.tabControl.LocalSettings.GlobalSettings.SetProperty(new StringProperty(this.tabControl.Core, "MergeSettingsFiles", "Linked"));
            }
            else
            {
                this.tabControl.LocalSettings.GlobalSettings.SetProperty(new StringProperty(this.tabControl.Core, "MergeSettingsFiles", this.noMerge.Checked ? "NoMerge" : "Parent"));
                this.tabControl.LocalSettings.GlobalSettings.Remove("LinkedSettingsFile");
            }
            this.dirty = false;
            this.tabControl.DirtyChanged();
            return true;
        }

        private void BrowseClick(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.CreatePrompt = false;
            dialog.DefaultExt = "StyleCop";
            dialog.FileName = (this.linkedFilePath.Text.Length == 0) ? "Settings.StyleCop" : this.linkedFilePath.Text;
            dialog.Filter = string.Format(CultureInfo.CurrentUICulture, "{0} (*.StyleCop)|*.StyleCop|{1} (*.*)|*.*", new object[] { Strings.SettingsFiles, Strings.AllFiles });
            dialog.InitialDirectory = this.linkedFilePath.Text;
            dialog.OverwritePrompt = false;
            dialog.ShowHelp = false;
            dialog.Title = Strings.GlobalSettingsFile;
            dialog.ValidateNames = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.linkedFilePath.Text = dialog.FileName;
            }
        }

        private static string ConvertBackslashes(string path)
        {
            char[] chArray = new char[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                char ch = path[i];
                if (ch == '/')
                {
                    chArray[i] = '\\';
                }
                else
                {
                    chArray[i] = ch;
                }
            }
            return new string(chArray);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EditLinkedSettingsFileClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.linkedFilePath.Text))
            {
                AlertDialog.Show(this.tabControl.Core, this, Strings.EmptySettingsFilePath, Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                Exception exception;
                string relativePath = Environment.ExpandEnvironmentVariables(this.linkedFilePath.Text);
                if (relativePath.StartsWith(".", StringComparison.Ordinal) || !relativePath.Contains(@"\"))
                {
                    relativePath = StyleCopCore.MakeAbsolutePath(this.tabControl.LocalSettings.Location, relativePath);
                }
                if (!File.Exists(relativePath) && (this.tabControl.Core.Environment.GetWritableSettings(relativePath, out exception) == null))
                {
                    AlertDialog.Show(this.tabControl.Core, this, string.Format(CultureInfo.CurrentUICulture, Strings.CannotLoadSettingsFilePath, new object[] { (exception == null) ? string.Empty : exception.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    relativePath = null;
                }
                if (!string.IsNullOrEmpty(relativePath))
                {
                    this.EditParentSettings(relativePath, false);
                }
            }
        }

        private void EditParentSettings(string settingsFile, bool defaultSettings)
        {
            this.tabControl.Core.ShowSettings(settingsFile, "StyleCopLocalProperties", defaultSettings);
            this.tabControl.RefreshMergedSettings();
        }

        private void EditParentSettingsFileClicked(object sender, EventArgs e)
        {
            bool flag = false;
            string parentSettingsPath = this.tabControl.Core.Environment.GetParentSettingsPath(this.tabControl.LocalSettings.Location);
            if (string.IsNullOrEmpty(parentSettingsPath))
            {
                flag = true;
                parentSettingsPath = this.tabControl.Core.Environment.GetDefaultSettingsPath();
            }
            if (string.IsNullOrEmpty(parentSettingsPath))
            {
                AlertDialog.Show(this.tabControl.Core, this, Strings.NoParentSettingsFile, Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else if (flag)
            {
                if (DialogResult.Yes == AlertDialog.Show(this.tabControl.Core, this, Strings.EditDefaultSettingsWarning, Strings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    this.EditParentSettings(parentSettingsPath, true);
                }
            }
            else
            {
                this.EditParentSettings(parentSettingsPath, false);
            }
        }

        private void EnableDisable()
        {
            this.locationLabel.Enabled = this.mergeWithLinkedFile.Checked;
            this.linkedFilePath.Enabled = this.mergeWithLinkedFile.Checked && !this.disableLinking;
            this.browse.Enabled = this.mergeWithLinkedFile.Checked && !this.disableLinking;
            this.editLinkedSettingsFile.Enabled = (this.mergeWithLinkedFile.Checked && (this.linkedFilePath.Text.Length > 0)) && !this.disableLinking;
            this.editParentSettingsFile.Enabled = this.mergeWithParents.Checked;
            this.linkedFilePath.Visible = !this.disableLinking;
            this.browse.Visible = !this.disableLinking;
            this.editLinkedSettingsFile.Visible = !this.disableLinking;
        }

        public void Initialize(PropertyControl propertyControl)
        {
            this.tabControl = propertyControl;
            StringProperty property = this.tabControl.LocalSettings.GlobalSettings.GetProperty("MergeSettingsFiles") as StringProperty;
            string strA = (property == null) ? "Parent" : property.Value;
            if (!this.tabControl.Core.Environment.SupportsLinkedSettings && (string.CompareOrdinal(strA, "Linked") == 0))
            {
                strA = "Parent";
                this.disableLinking = true;
            }
            if (string.CompareOrdinal(strA, "NoMerge") == 0)
            {
                this.noMerge.Checked = true;
            }
            else if (string.CompareOrdinal(strA, "Linked") == 0)
            {
                this.mergeWithLinkedFile.Checked = true;
                StringProperty property2 = this.tabControl.LocalSettings.GlobalSettings.GetProperty("LinkedSettingsFile") as StringProperty;
                if ((property2 != null) && !string.IsNullOrEmpty(property2.Value))
                {
                    string relativePath = Environment.ExpandEnvironmentVariables(property2.Value);
                    if (relativePath.StartsWith(".", StringComparison.Ordinal))
                    {
                        relativePath = StyleCopCore.MakeAbsolutePath(this.tabControl.LocalSettings.Location, relativePath);
                    }
                    this.linkedFilePath.Text = relativePath;
                }
            }
            else
            {
                this.mergeWithParents.Checked = true;
            }
            this.EnableDisable();
            bool defaultSettings = this.tabControl.LocalSettings.DefaultSettings;
            this.mergeWithParents.Enabled = !defaultSettings;
            this.editParentSettingsFile.Enabled = !defaultSettings;
            this.mergeWithLinkedFile.Enabled = !defaultSettings;
            this.locationLabel.Enabled = !defaultSettings;
            this.linkedFilePath.Enabled = !defaultSettings;
            this.browse.Enabled = !defaultSettings;
            this.editLinkedSettingsFile.Enabled = !defaultSettings;
            if (!this.noMerge.Checked && defaultSettings)
            {
                this.noMerge.Checked = true;
            }
            this.dirty = false;
            this.tabControl.DirtyChanged();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(GlobalSettingsFileOptions));
            this.browse = new Button();
            this.linkedFilePath = new TextBox();
            this.locationLabel = new Label();
            this.editLinkedSettingsFile = new Button();
            this.mergeWithParents = new RadioButton();
            this.noMerge = new RadioButton();
            this.mergeWithLinkedFile = new RadioButton();
            this.label1 = new Label();
            this.editParentSettingsFile = new Button();
            base.SuspendLayout();
            manager.ApplyResources(this.browse, "browse");
            this.browse.Name = "browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new EventHandler(this.BrowseClick);
            manager.ApplyResources(this.linkedFilePath, "linkedFilePath");
            this.linkedFilePath.Name = "linkedFilePath";
            this.linkedFilePath.TextChanged += new EventHandler(this.LinkedFilePathTextChanged);
            manager.ApplyResources(this.locationLabel, "locationLabel");
            this.locationLabel.Name = "locationLabel";
            manager.ApplyResources(this.editLinkedSettingsFile, "editLinkedSettingsFile");
            this.editLinkedSettingsFile.Name = "editLinkedSettingsFile";
            this.editLinkedSettingsFile.UseVisualStyleBackColor = true;
            this.editLinkedSettingsFile.Click += new EventHandler(this.EditLinkedSettingsFileClicked);
            manager.ApplyResources(this.mergeWithParents, "mergeWithParents");
            this.mergeWithParents.Name = "mergeWithParents";
            this.mergeWithParents.TabStop = true;
            this.mergeWithParents.UseVisualStyleBackColor = true;
            this.mergeWithParents.CheckedChanged += new EventHandler(this.MergeWithParentsCheckedChanged);
            manager.ApplyResources(this.noMerge, "noMerge");
            this.noMerge.Name = "noMerge";
            this.noMerge.TabStop = true;
            this.noMerge.UseVisualStyleBackColor = true;
            this.noMerge.CheckedChanged += new EventHandler(this.NoMergeCheckedChanged);
            manager.ApplyResources(this.mergeWithLinkedFile, "mergeWithLinkedFile");
            this.mergeWithLinkedFile.Name = "mergeWithLinkedFile";
            this.mergeWithLinkedFile.TabStop = true;
            this.mergeWithLinkedFile.UseVisualStyleBackColor = true;
            this.mergeWithLinkedFile.CheckedChanged += new EventHandler(this.MergeWithLinkedFileCheckedChanged);
            manager.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            manager.ApplyResources(this.editParentSettingsFile, "editParentSettingsFile");
            this.editParentSettingsFile.Name = "editParentSettingsFile";
            this.editParentSettingsFile.UseVisualStyleBackColor = true;
            this.editParentSettingsFile.Click += new EventHandler(this.EditParentSettingsFileClicked);
            base.Controls.Add(this.editParentSettingsFile);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.mergeWithLinkedFile);
            base.Controls.Add(this.noMerge);
            base.Controls.Add(this.mergeWithParents);
            base.Controls.Add(this.editLinkedSettingsFile);
            base.Controls.Add(this.browse);
            base.Controls.Add(this.linkedFilePath);
            base.Controls.Add(this.locationLabel);
            this.MinimumSize = new Size(0xf6, 80);
            base.Name = "GlobalSettingsFileOptions";
            manager.ApplyResources(this, "$this");
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LinkedFilePathTextChanged(object sender, EventArgs e)
        {
            if (!this.dirty)
            {
                this.dirty = true;
                this.tabControl.DirtyChanged();
            }
            this.editLinkedSettingsFile.Enabled = this.mergeWithLinkedFile.Checked && (this.linkedFilePath.Text.Length > 0);
        }

        private void MergeWithLinkedFileCheckedChanged(object sender, EventArgs e)
        {
            this.EnableDisable();
            if (!this.dirty)
            {
                this.dirty = true;
                this.tabControl.DirtyChanged();
            }
        }

        private void MergeWithParentsCheckedChanged(object sender, EventArgs e)
        {
            this.EnableDisable();
            if (!this.dirty)
            {
                this.dirty = true;
                this.tabControl.DirtyChanged();
            }
        }

        private void NoMergeCheckedChanged(object sender, EventArgs e)
        {
            this.EnableDisable();
            if (!this.dirty)
            {
                this.dirty = true;
                this.tabControl.DirtyChanged();
            }
        }

        public void PostApply(bool wasDirty)
        {
            if (wasDirty)
            {
                this.tabControl.RefreshMergedSettings();
            }
        }

        public bool PreApply()
        {
            return true;
        }

        public void RefreshSettingsOverrideState()
        {
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
                return Strings.SettingsFilesTab;
            }
        }
    }
}

