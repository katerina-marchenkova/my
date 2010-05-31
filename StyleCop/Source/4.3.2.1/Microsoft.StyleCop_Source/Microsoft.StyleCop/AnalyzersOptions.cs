namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class AnalyzersOptions : UserControl, IPropertyControlPage
    {
        private TreeView analyzeTree;
        private IContainer components;
        private TextBox description;
        private TreeView detailsTree;
        private bool dirty;
        private Button findRule;
        private TextBox findRuleId;
        private IButtonControl formAcceptButton;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ImageList nodeImages;
        private Dictionary<StyleCopAddIn, ICollection<BooleanProperty>> properties = new Dictionary<StyleCopAddIn, ICollection<BooleanProperty>>();
        private bool refreshing;
        private SplitContainer splitContainer1;
        private PropertyControl tabControl;

        public AnalyzersOptions()
        {
            this.InitializeComponent();
        }

        public void Activate(bool activated)
        {
        }

        private void AdjustBoldState()
        {
            this.analyzeTree.BeginUpdate();
            try
            {
                foreach (TreeNode node in this.analyzeTree.Nodes)
                {
                    bool bolded = false;
                    foreach (TreeNode node2 in node.Nodes)
                    {
                        bool flag2 = false;
                        foreach (TreeNode node3 in node2.Nodes)
                        {
                            if (node3.Tag == null)
                            {
                                bool flag3 = false;
                                foreach (TreeNode node4 in node3.Nodes)
                                {
                                    flag3 |= this.DetectBoldStateForRule(node4);
                                }
                                flag2 |= flag3;
                                SetBoldState(node3, flag3, this.analyzeTree);
                                continue;
                            }
                            flag2 |= this.DetectBoldStateForRule(node3);
                        }
                        bolded |= flag2;
                        SetBoldState(node2, flag2, this.analyzeTree);
                    }
                    SetBoldState(node, bolded, this.analyzeTree);
                }
            }
            finally
            {
                this.analyzeTree.EndUpdate();
            }
        }

        private void AnalyzeTreeAfterCheck(object sender, TreeViewEventArgs e)
        {
            this.dirty = true;
            this.tabControl.DirtyChanged();
            this.analyzeTree.AfterCheck -= new TreeViewEventHandler(this.AnalyzeTreeAfterCheck);
            this.CheckAllChildNodes(e.Node, e.Node.Checked);
            for (TreeNode node = e.Node.Parent; node != null; node = node.Parent)
            {
                if (e.Node.Checked)
                {
                    node.Checked = true;
                    continue;
                }
                bool flag = false;
                foreach (TreeNode node2 in node.Nodes)
                {
                    if (node2.Checked)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    node.Checked = false;
                }
            }
            this.analyzeTree.AfterCheck += new TreeViewEventHandler(this.AnalyzeTreeAfterCheck);
            if (!this.refreshing)
            {
                this.AdjustBoldState();
                this.analyzeTree.SelectedNode = e.Node;
            }
        }

        private void AnalyzeTreeAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                this.description.Clear();
            }
            else
            {
                StyleCopAddIn tag = e.Node.Tag as StyleCopAddIn;
                if (tag != null)
                {
                    this.description.Text = tag.Description;
                }
                else
                {
                    Rule rule = e.Node.Tag as Rule;
                    if (rule != null)
                    {
                        this.description.Text = rule.Description;
                    }
                    else
                    {
                        this.description.Clear();
                    }
                }
                this.FillDetailsTree();
            }
        }

        private void AnalyzeTreeBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag is SourceParser)
            {
                e.Cancel = true;
            }
        }

        public bool Apply()
        {
            foreach (TreeNode node in this.analyzeTree.Nodes)
            {
                SourceParser tag = (SourceParser) node.Tag;
                this.ApplyProperties(tag);
                foreach (TreeNode node2 in node.Nodes)
                {
                    SourceAnalyzer addIn = (SourceAnalyzer) node2.Tag;
                    this.ApplyProperties(addIn);
                    this.ApplyRules(addIn, node2);
                }
            }
            this.dirty = false;
            this.tabControl.DirtyChanged();
            return true;
        }

        private void ApplyProperties(StyleCopAddIn addIn)
        {
            ICollection<BooleanProperty> is2 = null;
            if (this.properties.TryGetValue(addIn, out is2))
            {
                foreach (BooleanProperty property in is2)
                {
                    addIn.SetSetting(this.tabControl.LocalSettings, property);
                }
            }
        }

        private void ApplyRules(StyleCopAddIn addIn, TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.Nodes)
            {
                Rule tag = node.Tag as Rule;
                if (tag == null)
                {
                    this.ApplyRules(addIn, node);
                }
                else
                {
                    addIn.SetSetting(this.tabControl.LocalSettings, new BooleanProperty(addIn, tag.Name + "#Enabled", node.Checked));
                }
            }
        }

        private void CheckAllChildNodes(TreeNode node, bool @checked)
        {
            foreach (TreeNode node2 in node.Nodes)
            {
                node2.Checked = @checked;
                if (node2.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(node2, @checked);
                }
            }
        }

        private void DetailsTreeAfterCheck(object sender, TreeViewEventArgs e)
        {
            this.dirty = true;
            this.tabControl.DirtyChanged();
            this.DetectBoldStateForDetails(e.Node);
            if (!this.refreshing)
            {
                this.detailsTree.SelectedNode = e.Node;
            }
            PropertyAddInPair tag = (PropertyAddInPair) e.Node.Tag;
            tag.Property.Value = e.Node.Checked;
        }

        private void DetailsTreeAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                this.description.Clear();
            }
            else
            {
                PropertyAddInPair tag = (PropertyAddInPair) e.Node.Tag;
                this.description.Text = tag.Property.Description;
            }
        }

        private void DetectBoldStateForDetails(TreeNode propertyNode)
        {
            PropertyAddInPair tag = (PropertyAddInPair) propertyNode.Tag;
            bool bolded = false;
            BooleanProperty localProperty = new BooleanProperty((PropertyDescriptor<bool>) tag.Property.PropertyDescriptor, propertyNode.Checked);
            bolded = this.tabControl.SettingsComparer.IsAddInSettingOverwritten(tag.AddIn, tag.Property.PropertyName, localProperty);
            SetBoldState(propertyNode, bolded, this.detailsTree);
        }

        private bool DetectBoldStateForRule(TreeNode ruleNode)
        {
            SourceAnalyzer propertyContainer = null;
            for (TreeNode node = ruleNode.Parent; node != null; node = node.Parent)
            {
                propertyContainer = node.Tag as SourceAnalyzer;
                if (propertyContainer != null)
                {
                    break;
                }
            }
            Rule tag = ruleNode.Tag as Rule;
            bool bolded = false;
            if (propertyContainer != null)
            {
                string propertyName = tag.Name + "#Enabled";
                BooleanProperty localProperty = new BooleanProperty(propertyContainer, propertyName, ruleNode.Checked);
                bolded = this.tabControl.SettingsComparer.IsAddInSettingOverwritten(propertyContainer, propertyName, localProperty);
            }
            SetBoldState(ruleNode, bolded, this.analyzeTree);
            return bolded;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FillAnalyzerRules(SourceAnalyzer analyzer, TreeNode analyzerNode)
        {
            foreach (Rule rule in analyzer.AddInRules)
            {
                if (rule.CanDisable)
                {
                    TreeNode nodeToInsert = analyzerNode;
                    if (!string.IsNullOrEmpty(rule.RuleGroup))
                    {
                        nodeToInsert = FindMatchingRuleGroupNode(analyzerNode.Nodes, rule.RuleGroup);
                        if (nodeToInsert == null)
                        {
                            nodeToInsert = new TreeNode(rule.RuleGroup);
                            nodeToInsert.ImageKey = nodeToInsert.SelectedImageKey = "RuleGroupNode";
                            InsertIntoSortedTree(analyzerNode.Nodes, nodeToInsert);
                        }
                    }
                    TreeNode node2 = new TreeNode(rule.CheckId + ": " + rule.Name);
                    node2.ImageKey = node2.SelectedImageKey = "RuleNode";
                    node2.Tag = rule;
                    InsertIntoSortedTree(nodeToInsert.Nodes, node2);
                    this.InitializeRuleCheckedState(rule, node2);
                }
            }
        }

        private void FillAnalyzerTree()
        {
            foreach (SourceParser parser in this.tabControl.Core.Parsers)
            {
                TreeNode nodeToInsert = new TreeNode(parser.Name);
                nodeToInsert.ImageKey = nodeToInsert.SelectedImageKey = "ParserNode";
                nodeToInsert.Tag = parser;
                InsertIntoSortedTree(this.analyzeTree.Nodes, nodeToInsert);
                this.StoreAddinProperties(parser);
                this.refreshing = true;
                try
                {
                    foreach (SourceAnalyzer analyzer in parser.Analyzers)
                    {
                        TreeNode node2 = new TreeNode(analyzer.Name);
                        node2.ImageKey = node2.SelectedImageKey = "AnalyzerNode";
                        node2.Tag = analyzer;
                        InsertIntoSortedTree(nodeToInsert.Nodes, node2);
                        this.StoreAddinProperties(analyzer);
                        this.FillAnalyzerRules(analyzer, node2);
                    }
                }
                finally
                {
                    this.refreshing = false;
                    this.AdjustBoldState();
                }
                nodeToInsert.Expand();
            }
        }

        private void FillDetailsTree()
        {
            this.detailsTree.Nodes.Clear();
            if (this.analyzeTree.SelectedNode != null)
            {
                StyleCopAddIn tag = this.analyzeTree.SelectedNode.Tag as StyleCopAddIn;
                if (tag != null)
                {
                    ICollection<BooleanProperty> is2 = null;
                    if (this.properties.TryGetValue(tag, out is2))
                    {
                        foreach (BooleanProperty property in is2)
                        {
                            PropertyAddInPair pair = new PropertyAddInPair();
                            pair.Property = property;
                            pair.AddIn = tag;
                            TreeNode node = new TreeNode(property.FriendlyName);
                            node.Checked = property.Value;
                            node.Tag = pair;
                            this.detailsTree.Nodes.Add(node);
                            this.DetectBoldStateForDetails(node);
                        }
                    }
                }
            }
        }

        private static TreeNode FindMatchingRuleGroupNode(TreeNodeCollection nodes, string ruleGroup)
        {
            foreach (TreeNode node in nodes)
            {
                if ((node.Tag == null) && string.Equals(node.Text, ruleGroup, StringComparison.Ordinal))
                {
                    return node;
                }
            }
            return null;
        }

        private void FindRuleClick(object sender, EventArgs e)
        {
            if (this.findRuleId.Text.Length > 0)
            {
                this.SearchForRule(this.findRuleId.Text);
            }
        }

        private void FindRuleIdGotFocus(object sender, EventArgs e)
        {
            this.formAcceptButton = base.ParentForm.AcceptButton;
            base.ParentForm.AcceptButton = null;
        }

        private void FindRuleIdKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Return) && (this.findRuleId.Text.Length > 0))
            {
                this.SearchForRule(this.findRuleId.Text);
            }
        }

        private void FindRuleIdLostFocus(object sender, EventArgs e)
        {
            if (this.formAcceptButton != null)
            {
                base.ParentForm.AcceptButton = this.formAcceptButton;
                this.formAcceptButton = null;
            }
        }

        public void Initialize(PropertyControl propertyControl)
        {
            this.tabControl = propertyControl;
            this.FillAnalyzerTree();
            if (this.analyzeTree.Nodes.Count > 0)
            {
                this.analyzeTree.SelectedNode = this.analyzeTree.Nodes[0];
            }
            this.dirty = false;
            this.tabControl.DirtyChanged();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(AnalyzersOptions));
            this.label1 = new Label();
            this.description = new TextBox();
            this.splitContainer1 = new SplitContainer();
            this.label2 = new Label();
            this.analyzeTree = new TreeView();
            this.nodeImages = new ImageList(this.components);
            this.label3 = new Label();
            this.detailsTree = new TreeView();
            this.findRule = new Button();
            this.findRuleId = new TextBox();
            this.label4 = new Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            base.SuspendLayout();
            manager.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            manager.ApplyResources(this.description, "description");
            this.description.Name = "description";
            this.description.ReadOnly = true;
            manager.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.analyzeTree);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.detailsTree);
            manager.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            manager.ApplyResources(this.analyzeTree, "analyzeTree");
            this.analyzeTree.CheckBoxes = true;
            this.analyzeTree.HideSelection = false;
            this.analyzeTree.ImageList = this.nodeImages;
            this.analyzeTree.Name = "analyzeTree";
            this.analyzeTree.ShowRootLines = false;
            this.analyzeTree.AfterCheck += new TreeViewEventHandler(this.AnalyzeTreeAfterCheck);
            this.analyzeTree.BeforeCollapse += new TreeViewCancelEventHandler(this.AnalyzeTreeBeforeCollapse);
            this.analyzeTree.AfterSelect += new TreeViewEventHandler(this.AnalyzeTreeAfterSelect);
            this.nodeImages.ImageStream = (ImageListStreamer) manager.GetObject("nodeImages.ImageStream");
            this.nodeImages.TransparentColor = Color.Magenta;
            this.nodeImages.Images.SetKeyName(0, "AnalyzerNode");
            this.nodeImages.Images.SetKeyName(1, "ParserNode");
            this.nodeImages.Images.SetKeyName(2, "RuleGroupNode");
            this.nodeImages.Images.SetKeyName(3, "RuleNode");
            manager.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            manager.ApplyResources(this.detailsTree, "detailsTree");
            this.detailsTree.CheckBoxes = true;
            this.detailsTree.HideSelection = false;
            this.detailsTree.Name = "detailsTree";
            this.detailsTree.ShowLines = false;
            this.detailsTree.ShowRootLines = false;
            this.detailsTree.AfterCheck += new TreeViewEventHandler(this.DetailsTreeAfterCheck);
            this.detailsTree.AfterSelect += new TreeViewEventHandler(this.DetailsTreeAfterSelect);
            manager.ApplyResources(this.findRule, "findRule");
            this.findRule.Name = "findRule";
            this.findRule.UseVisualStyleBackColor = true;
            this.findRule.Click += new EventHandler(this.FindRuleClick);
            manager.ApplyResources(this.findRuleId, "findRuleId");
            this.findRuleId.Name = "findRuleId";
            this.findRuleId.KeyDown += new KeyEventHandler(this.FindRuleIdKeyDown);
            this.findRuleId.GotFocus += new EventHandler(this.FindRuleIdGotFocus);
            this.findRuleId.LostFocus += new EventHandler(this.FindRuleIdLostFocus);
            manager.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            base.Controls.Add(this.findRule);
            base.Controls.Add(this.findRuleId);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.splitContainer1);
            base.Controls.Add(this.description);
            base.Controls.Add(this.label1);
            base.Name = "AnalyzersOptions";
            manager.ApplyResources(this, "$this");
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitializePropertyState(StyleCopAddIn addIn, BooleanProperty property)
        {
            BooleanProperty setting = addIn.GetSetting(this.tabControl.MergedSettings, property.PropertyName) as BooleanProperty;
            if (setting == null)
            {
                property.Value = property.DefaultValue;
            }
            else
            {
                property.Value = setting.Value;
            }
        }

        private void InitializeRuleCheckedState(Rule rule, TreeNode ruleNode)
        {
            SourceAnalyzer tag = null;
            for (TreeNode node = ruleNode.Parent; node != null; node = node.Parent)
            {
                tag = node.Tag as SourceAnalyzer;
                if (tag != null)
                {
                    break;
                }
            }
            BooleanProperty property = tag.GetRuleSetting(this.tabControl.MergedSettings, rule.Name, "Enabled") as BooleanProperty;
            if (property == null)
            {
                ruleNode.Checked = rule.EnabledByDefault;
            }
            else
            {
                ruleNode.Checked = property.Value;
            }
        }

        private static void InsertIntoSortedTree(TreeNodeCollection nodes, TreeNode nodeToInsert)
        {
            int index = 0;
            while (index < nodes.Count)
            {
                if (string.Compare(nodes[index].Text, nodeToInsert.Text, StringComparison.Ordinal) > 0)
                {
                    break;
                }
                index++;
            }
            nodes.Insert(index, nodeToInsert);
        }

        private TreeNode IterateAndFindRule(TreeNodeCollection nodes, string searchText, MatchRuleHandler matchHandler)
        {
            foreach (TreeNode node in nodes)
            {
                if (matchHandler(node, searchText))
                {
                    return node;
                }
                TreeNode node2 = this.IterateAndFindRule(node.Nodes, searchText, matchHandler);
                if (node2 != null)
                {
                    return node2;
                }
            }
            return null;
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
            this.refreshing = true;
            try
            {
                foreach (TreeNode node in this.analyzeTree.Nodes)
                {
                    foreach (TreeNode node2 in node.Nodes)
                    {
                        foreach (TreeNode node3 in node2.Nodes)
                        {
                            Rule tag = node3.Tag as Rule;
                            if (tag == null)
                            {
                                foreach (TreeNode node4 in node3.Nodes)
                                {
                                    tag = node4.Tag as Rule;
                                    this.InitializeRuleCheckedState(tag, node4);
                                }
                                continue;
                            }
                            this.InitializeRuleCheckedState(tag, node3);
                        }
                    }
                }
                this.FillDetailsTree();
            }
            finally
            {
                this.refreshing = false;
                this.AdjustBoldState();
            }
        }

        private void SearchForRule(string searchText)
        {
            TreeNode node = this.SearchForRuleByCategories(searchText);
            if (node != null)
            {
                node.EnsureVisible();
                this.analyzeTree.SelectedNode = node;
            }
        }

        private TreeNode SearchForRuleByCategories(string searchText)
        {
            TreeNode node1 = this.IterateAndFindRule(this.analyzeTree.Nodes, searchText, delegate (TreeNode node, string text) {
                Rule tag = node.Tag as Rule;
                return (tag != null) && string.Equals(text, tag.CheckId, StringComparison.OrdinalIgnoreCase);
            });
            if (node1 == null)
            {
                node1 = this.IterateAndFindRule(this.analyzeTree.Nodes, searchText, delegate (TreeNode node, string text) {
                    Rule tag = node.Tag as Rule;
                    return (tag != null) && string.Equals(text, tag.Name, StringComparison.OrdinalIgnoreCase);
                });
            }
            if (node1 != null)
            {
                return node1;
            }
            return this.IterateAndFindRule(this.analyzeTree.Nodes, searchText, delegate (TreeNode node, string text) {
                Rule tag = node.Tag as Rule;
                return (tag != null) && tag.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static void SetBoldState(TreeNode item, bool bolded, TreeView tree)
        {
            if (bolded)
            {
                if (item.NodeFont == null)
                {
                    item.NodeFont = new Font(tree.Font, FontStyle.Bold);
                }
                else if (!item.NodeFont.Bold)
                {
                    item.NodeFont = new Font(item.NodeFont, FontStyle.Bold);
                }
            }
            else if ((item.NodeFont != null) && item.NodeFont.Bold)
            {
                item.NodeFont = new Font(item.NodeFont, FontStyle.Regular);
            }
            tree.BeginUpdate();
            tree.EndUpdate();
        }

        private void StoreAddinProperties(StyleCopAddIn addIn)
        {
            ICollection<Microsoft.StyleCop.PropertyDescriptor> propertyDescriptors = addIn.PropertyDescriptors;
            if ((propertyDescriptors != null) && (propertyDescriptors.Count > 0))
            {
                List<BooleanProperty> list = new List<BooleanProperty>(propertyDescriptors.Count);
                foreach (Microsoft.StyleCop.PropertyDescriptor descriptor in propertyDescriptors)
                {
                    if ((descriptor.PropertyType == PropertyType.Boolean) && descriptor.DisplaySettings)
                    {
                        PropertyDescriptor<bool> propertyDescriptor = (PropertyDescriptor<bool>) descriptor;
                        if (string.IsNullOrEmpty(descriptor.FriendlyName))
                        {
                            throw new ArgumentException(Strings.PropertyFriendlyNameNotSet);
                        }
                        if (string.IsNullOrEmpty(descriptor.Description))
                        {
                            throw new ArgumentException(Strings.PropertyDescriptionNotSet);
                        }
                        BooleanProperty property = new BooleanProperty(propertyDescriptor, propertyDescriptor.DefaultValue);
                        this.InitializePropertyState(addIn, property);
                        list.Add(property);
                    }
                }
                this.properties.Add(addIn, list.ToArray());
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
                return Strings.AnalyzersTab;
            }
        }

        private delegate bool MatchRuleHandler(TreeNode node, string searchText);

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyAddInPair
        {
            public BooleanProperty Property;
            public StyleCopAddIn AddIn;
        }
    }
}

