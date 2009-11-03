namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Windows.Forms;
    using System.Xml;

    internal class PropertyDialog : Form, IPropertyControlHost
    {
        private Button apply;
        private Button cancel;
        private object[] context;
        private StyleCopCore core;
        private Button help;
        private Help helpCallback;
        private string id;
        private Button ok;
        private IList<IPropertyControlPage> pages;
        private PropertyControl properties;
        private bool settingsChanged;
        private WritableSettings settingsFile;

        public PropertyDialog(IList<IPropertyControlPage> pages, WritableSettings settingsFile, string id, StyleCopCore core, Help helpCallback, params object[] context)
        {
            this.pages = pages;
            this.settingsFile = settingsFile;
            this.id = id;
            this.core = core;
            this.helpCallback = helpCallback;
            this.context = context;
            this.InitializeComponent();
            this.core.Registry.RestoreWindowPosition(this.id, this, base.Location, base.Size);
        }

        private void ApplyClick(object sender, EventArgs e)
        {
            bool flag;
            PropertyControlSaveResult result = this.properties.Apply(out flag);
            if (flag)
            {
                this.settingsChanged = true;
            }
            if (result == PropertyControlSaveResult.SaveError)
            {
                base.Close();
            }
        }

        public void Cancel()
        {
            base.Close();
        }

        private void CancelClick(object sender, EventArgs e)
        {
            base.Close();
        }

        public void Dirty(bool isDirty)
        {
            this.apply.Enabled = isDirty;
        }

        private void HelpClick(object sender, EventArgs e)
        {
            if (this.helpCallback != null)
            {
                this.helpCallback(this.properties.ActivePage);
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(PropertyDialog));
            this.help = new Button();
            this.cancel = new Button();
            this.ok = new Button();
            this.apply = new Button();
            this.properties = new PropertyControl();
            base.SuspendLayout();
            manager.ApplyResources(this.help, "help");
            this.help.Name = "help";
            this.help.Click += new EventHandler(this.HelpClick);
            manager.ApplyResources(this.cancel, "cancel");
            this.cancel.DialogResult = DialogResult.Cancel;
            this.cancel.Name = "cancel";
            this.cancel.Click += new EventHandler(this.CancelClick);
            manager.ApplyResources(this.ok, "ok");
            this.ok.Name = "ok";
            this.ok.Click += new EventHandler(this.OkClick);
            manager.ApplyResources(this.apply, "apply");
            this.apply.Name = "apply";
            this.apply.Click += new EventHandler(this.ApplyClick);
            manager.ApplyResources(this.properties, "properties");
            this.properties.HotTrack = true;
            this.properties.Name = "properties";
            this.properties.SelectedIndex = 0;
            base.AcceptButton = this.ok;
            base.CancelButton = this.cancel;
            manager.ApplyResources(this, "$this");
            base.Controls.Add(this.properties);
            base.Controls.Add(this.apply);
            base.Controls.Add(this.ok);
            base.Controls.Add(this.cancel);
            base.Controls.Add(this.help);
            base.Name = "PropertyDialog";
            base.ResumeLayout(false);
        }

        private static void MoveButton(Button source, Button dest)
        {
            source.Top = dest.Top;
            source.Left = dest.Left;
        }

        private void OkClick(object sender, EventArgs e)
        {
            bool flag;
            PropertyControlSaveResult result = this.properties.Apply(out flag);
            if (flag)
            {
                this.settingsChanged = true;
            }
            if (result == PropertyControlSaveResult.Success)
            {
                base.Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.core.Registry.SaveWindowPositionByForm(this.id, this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.helpCallback == null)
            {
                MoveButton(this.ok, this.cancel);
                MoveButton(this.cancel, this.apply);
                MoveButton(this.apply, this.help);
                this.help.Visible = false;
            }
            this.apply.Enabled = false;
            try
            {
                this.properties.Initialize(this, this.pages, this.settingsFile, this.core, this.context);
            }
            catch (IOException exception)
            {
                AlertDialog.Show(this.core, this, string.Format(CultureInfo.CurrentUICulture, Strings.LocalSettingsNotOpenedOrCreated, new object[] { exception.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
            catch (SecurityException exception2)
            {
                AlertDialog.Show(this.core, this, string.Format(CultureInfo.CurrentUICulture, Strings.LocalSettingsNotOpenedOrCreated, new object[] { exception2.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
            catch (UnauthorizedAccessException exception3)
            {
                AlertDialog.Show(this.core, this, string.Format(CultureInfo.CurrentUICulture, Strings.LocalSettingsNotOpenedOrCreated, new object[] { exception3.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
            catch (XmlException exception4)
            {
                AlertDialog.Show(this.core, this, string.Format(CultureInfo.CurrentUICulture, Strings.LocalSettingsNotOpenedOrCreated, new object[] { exception4.Message }), Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
        }

        public bool SettingsChanged
        {
            get
            {
                return this.settingsChanged;
            }
        }

        public delegate void Help(IPropertyControlPage activePage);
    }
}

