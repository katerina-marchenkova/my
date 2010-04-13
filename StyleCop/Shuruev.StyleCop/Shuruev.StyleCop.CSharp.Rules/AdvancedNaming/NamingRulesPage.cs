using System;
using System.Drawing;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying naming rules page.
	/// </summary>
	public partial class NamingRulesPage : UserControl
	{
		private Font m_regular;
		private Font m_bold;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public NamingRulesPage()
		{
			InitializeComponent();

			listRules.SmallImageList = Pictures.GetList();
		}

		#region Properties

		/// <summary>
		/// Gets or sets parent property page.
		/// </summary>
		public PropertyPage Page { get; set; }

		#endregion

		#region Event handlers

		private void NamingRulesPage_Load(object sender, EventArgs e)
		{
			m_regular = new Font(listRules.Font, FontStyle.Regular);
			m_bold = new Font(listRules.Font, FontStyle.Bold);

			UpdateControls();
		}

		private void listRules_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void listRules_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			Action_Edit_Do();
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			Action_Edit_Do();
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			Action_Reset_Do();
		}

		#endregion

		/// <summary>
		/// Initializes the specified property control with the StyleCop settings file.
		/// </summary>
		public void Initialize()
		{
			UpdateRuleList();
		}

		/// <summary>
		/// Apply the modifications to the StyleCop settings file.
		/// </summary>
		public bool Apply()
		{
			return true;
		}

		/// <summary>
		/// Refreshes the state of the settings override.
		/// </summary>
		public void RefreshSettingsOverrideState()
		{
		}

		#region User interface

		/// <summary>
		/// Updates rule list.
		/// </summary>
		private void UpdateRuleList()
		{
			listRules.BeginUpdate();
			listRules.Items.Clear();

			foreach (string setting in NamingSettings.All)
			{
				string friendlyName = SettingsManager.GetFriendlyName(Page, setting);
				string mergedValue = SettingsManager.GetMergedValue(Page, setting);
				string inheritedValue = SettingsManager.GetInheritedValue(Page, setting);

				SettingTag tag = new SettingTag();
				tag.SettingName = setting;
				tag.MergedValue = mergedValue;
				tag.InheritedValue = inheritedValue;

				ListViewItem lvi = new ListViewItem();
				lvi.UseItemStyleForSubItems = false;
				lvi.Text = friendlyName;
				lvi.Tag = tag;

				ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
				lvi.SubItems.Add(sub);

				UpdateListItem(lvi);

				listRules.Items.Add(lvi);
			}

			listRules.EndUpdate();
		}

		/// <summary>
		/// Updates list item depending on specified properties.
		/// </summary>
		private void UpdateListItem(ListViewItem lvi)
		{
			SettingTag tag = (SettingTag)lvi.Tag;
			ListViewItem.ListViewSubItem sub = lvi.SubItems[1];

			sub.Font = tag.Modified ?
				m_bold :
				m_regular;

			if (String.IsNullOrEmpty(tag.MergedValue))
			{
				lvi.ImageKey = Pictures.RuleDisabled;
				sub.Text = Resources.DoNotCheck;
			}
			else
			{
				lvi.ImageKey = Pictures.RuleEnabled;
				sub.Text = NamingMacro.BuildExample(tag.MergedValue);
			}
		}

		/// <summary>
		/// Updates controls for all actions.
		/// </summary>
		private void UpdateControls()
		{
			btnEdit.Enabled = Action_Edit_IsAvailable();
			btnReset.Enabled = Action_Reset_IsAvailable();
		}

		#endregion

		#region Actions

		/// <summary>
		/// Does action "Edit".
		/// </summary>
		private void Action_Edit_Do()
		{
			if (!Action_Edit_IsAvailable())
				return;

			ListViewItem lvi = listRules.SelectedItems[0];
			SettingTag tag = (SettingTag)lvi.Tag;

			using (NamingRuleEditor dialog = new NamingRuleEditor())
			{
				dialog.ObjectName = lvi.Text;
				dialog.RuleDefinition = tag.MergedValue;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					tag.MergedValue = dialog.RuleDefinition;
					UpdateListItem(lvi);
				}
			}
		}

		/// <summary>
		/// Checks whether action "Edit" is available.
		/// </summary>
		private bool Action_Edit_IsAvailable()
		{
			if (listRules.SelectedItems.Count == 1)
				return true;

			return false;
		}

		/// <summary>
		/// Does action "Reset".
		/// </summary>
		private void Action_Reset_Do()
		{
			if (!Action_Reset_IsAvailable())
				return;

			ListViewItem lvi = listRules.SelectedItems[0];
			SettingTag tag = (SettingTag)lvi.Tag;

			string example = NamingMacro.BuildExample(tag.InheritedValue);
			if (Messages.ShowWarningYesNo(this, Resources.ResetSettingQuestion, example) != DialogResult.Yes)
				return;

			tag.MergedValue = tag.InheritedValue;
			UpdateListItem(lvi);

			UpdateControls();
		}

		/// <summary>
		/// Checks whether action "Reset" is available.
		/// </summary>
		private bool Action_Reset_IsAvailable()
		{
			if (listRules.SelectedItems.Count != 1)
				return false;

			ListViewItem lvi = listRules.SelectedItems[0];
			SettingTag tag = (SettingTag)lvi.Tag;
			if (tag.Modified)
				return true;

			return false;
		}

		#endregion
	}
}
