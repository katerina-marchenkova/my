namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    internal class ValidPrefixes : UserControl, IPropertyControlPage
    {
        private Button addButton;
        private TextBox addPrefix;
        private SourceAnalyzer analyzer;
        private ColumnHeader columnHeader1;
        private bool dirty;
        private IButtonControl formAcceptButton;
        private Label label1;
        private Label label2;
        private Label label3;
        private ListView prefixList;
        private Button removeButton;
        private bool settingsAreMerged;
        private PropertyControl tabControl;

        public ValidPrefixes()
        {
            this.InitializeComponent();
        }

        public ValidPrefixes(NamingRules analyzer) : this()
        {
            this.analyzer = analyzer;
        }

        public void Activate(bool activated)
        {
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            if ((this.addPrefix.Text.Length == 0) || (this.addPrefix.Text.Length > 2))
            {
                AlertDialog.Show(this.tabControl.Core, this, Microsoft.StyleCop.CSharp.Strings.EnterValidPrefix, Microsoft.StyleCop.CSharp.Strings.Title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                foreach (ListViewItem item in this.prefixList.Items)
                {
                    if (item.Text == this.addPrefix.Text)
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                        this.addPrefix.Clear();
                        return;
                    }
                }
                ListViewItem item2 = this.prefixList.Items.Add(this.addPrefix.Text);
                if (item2 != null)
                {
                    item2.Tag = true;
                    item2.Selected = true;
                    this.prefixList.EnsureVisible(item2.Index);
                    this.SetBoldState(item2);
                }
                this.addPrefix.Clear();
                this.dirty = true;
                this.tabControl.DirtyChanged();
            }
        }

        private void AddParentPrefixes()
        {
            CollectionProperty addInSetting = null;
            if (this.tabControl.ParentSettings != null)
            {
                addInSetting = this.tabControl.ParentSettings.GetAddInSetting(this.analyzer, "Hungarian") as CollectionProperty;
                if (addInSetting != null)
                {
                    this.settingsAreMerged = true;
                    if (addInSetting.Values.Count > 0)
                    {
                        foreach (string str in addInSetting)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                ListViewItem item = this.prefixList.Items.Add(str);
                                if (item != null)
                                {
                                    item.Tag = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddPrefixGotFocus(object sender, EventArgs e)
        {
            this.formAcceptButton = base.ParentForm.AcceptButton;
            base.ParentForm.AcceptButton = null;
        }

        private void AddPrefixKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Return) && (this.addPrefix.Text.Length > 0))
            {
                this.AddButtonClick(sender, e);
            }
        }

        private void AddPrefixLostFocus(object sender, EventArgs e)
        {
            if (this.formAcceptButton != null)
            {
                base.ParentForm.AcceptButton = this.formAcceptButton;
                this.formAcceptButton = null;
            }
        }

        public bool Apply()
        {
            if (this.analyzer != null)
            {
                List<string> innerCollection = new List<string>(this.prefixList.Items.Count);
                foreach (ListViewItem item in this.prefixList.Items)
                {
                    if ((bool) item.Tag)
                    {
                        innerCollection.Add(item.Text);
                    }
                }
                CollectionProperty property = new CollectionProperty(this.analyzer, "Hungarian", innerCollection);
                this.tabControl.LocalSettings.SetAddInSetting(this.analyzer, property);
            }
            this.dirty = false;
            this.tabControl.DirtyChanged();
            return true;
        }

        private void EnableDisableRemoveButton()
        {
            if (this.prefixList.SelectedItems.Count > 0)
            {
                ListViewItem item = this.prefixList.SelectedItems[0];
                this.removeButton.Enabled = (bool) item.Tag;
            }
            else
            {
                this.removeButton.Enabled = false;
            }
        }

        public void Initialize(PropertyControl propertyControl)
        {
            this.tabControl = propertyControl;
            if (this.analyzer != null)
            {
                this.AddParentPrefixes();
                CollectionProperty addInSetting = this.tabControl.LocalSettings.GetAddInSetting(this.analyzer, "Hungarian") as CollectionProperty;
                if ((addInSetting != null) && (addInSetting.Values.Count > 0))
                {
                    foreach (string str in addInSetting)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            ListViewItem item = this.prefixList.Items.Add(str);
                            if (item != null)
                            {
                                item.Tag = true;
                                this.SetBoldState(item);
                            }
                        }
                    }
                }
            }
            if (this.prefixList.Items.Count > 0)
            {
                this.prefixList.Items[0].Selected = true;
            }
            this.EnableDisableRemoveButton();
            this.dirty = false;
            this.tabControl.DirtyChanged();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(ValidPrefixes));
            this.removeButton = new Button();
            this.addButton = new Button();
            this.label2 = new Label();
            this.addPrefix = new TextBox();
            this.label1 = new Label();
            this.prefixList = new ListView();
            this.columnHeader1 = new ColumnHeader();
            this.label3 = new Label();
            base.SuspendLayout();
            manager.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.Name = "removeButton";
            this.removeButton.Click += new EventHandler(this.RemoveButtonClick);
            manager.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new EventHandler(this.AddButtonClick);
            manager.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            manager.ApplyResources(this.addPrefix, "addPrefix");
            this.addPrefix.Name = "addPrefix";
            this.addPrefix.KeyDown += new KeyEventHandler(this.AddPrefixKeyDown);
            this.addPrefix.GotFocus += new EventHandler(this.AddPrefixGotFocus);
            this.addPrefix.LostFocus += new EventHandler(this.AddPrefixLostFocus);
            manager.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            manager.ApplyResources(this.prefixList, "prefixList");
            this.prefixList.Columns.AddRange(new ColumnHeader[] { this.columnHeader1 });
            this.prefixList.HeaderStyle = ColumnHeaderStyle.None;
            this.prefixList.HideSelection = false;
            this.prefixList.MultiSelect = false;
            this.prefixList.Name = "prefixList";
            this.prefixList.Sorting = SortOrder.Ascending;
            this.prefixList.UseCompatibleStateImageBehavior = false;
            this.prefixList.View = View.Details;
            this.prefixList.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.PrefixListItemSelectionChanged);
            this.prefixList.KeyDown += new KeyEventHandler(this.PrefixListKeyDown);
            manager.ApplyResources(this.columnHeader1, "columnHeader1");
            manager.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            base.Controls.Add(this.label3);
            base.Controls.Add(this.prefixList);
            base.Controls.Add(this.removeButton);
            base.Controls.Add(this.addButton);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.addPrefix);
            base.Controls.Add(this.label1);
            base.Name = "ValidPrefixes";
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

        private void PrefixListItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.EnableDisableRemoveButton();
        }

        private void PrefixListKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Delete) && (this.addPrefix.Text.Length > 0))
            {
                this.RemoveButtonClick(sender, e);
            }
        }

        public void RefreshSettingsOverrideState()
        {
            List<ListViewItem> list = new List<ListViewItem>();
            foreach (ListViewItem item in this.prefixList.Items)
            {
                if (!((bool) item.Tag))
                {
                    list.Add(item);
                }
            }
            foreach (ListViewItem item2 in list)
            {
                this.prefixList.Items.Remove(item2);
            }
            this.AddParentPrefixes();
            foreach (ListViewItem item3 in this.prefixList.Items)
            {
                if ((bool) item3.Tag)
                {
                    this.SetBoldState(item3);
                }
            }
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            if (this.prefixList.SelectedItems.Count > 0)
            {
                int index = this.prefixList.SelectedIndices[0];
                this.prefixList.Items.RemoveAt(index);
                this.EnableDisableRemoveButton();
                if (this.prefixList.Items.Count > index)
                {
                    this.prefixList.Items[index].Selected = true;
                }
                else if (this.prefixList.Items.Count > 0)
                {
                    this.prefixList.Items[this.prefixList.Items.Count - 1].Selected = true;
                }
                this.dirty = true;
                this.tabControl.DirtyChanged();
            }
        }

        private void SetBoldState(ListViewItem item)
        {
            if ((item.Font != this.prefixList.Font) && (item.Font != null))
            {
                item.Font.Dispose();
            }
            if (this.settingsAreMerged)
            {
                item.Font = new Font(this.prefixList.Font, FontStyle.Bold);
            }
            else
            {
                item.Font = new Font(this.prefixList.Font, FontStyle.Regular);
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
                return Microsoft.StyleCop.CSharp.Strings.HungarianTab;
            }
        }
    }
}

