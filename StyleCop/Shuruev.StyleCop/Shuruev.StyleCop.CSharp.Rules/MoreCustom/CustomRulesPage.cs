using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying custom rules page.
	/// </summary>
	public partial class CustomRulesPage : UserControl
	{
		private Font m_regular;
		private Font m_bold;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRulesPage()
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

		private void CustomRulesPage_Load(object sender, EventArgs e)
		{
			m_regular = new Font(listRules.Font, FontStyle.Regular);
			m_bold = new Font(listRules.Font, FontStyle.Bold);

			UpdateControls();
		}

		private void CustomRulesPage_VisibleChanged(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			if (!Visible)
				return;

			if (!SettingsGrabber.Initialized)
				return;

			UpdateWarnings();
			UpdateAllListItems();
		}

		private void listRules_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		/*xxxprivate void listRules_MouseDoubleClick(object sender, MouseEventArgs e)
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
		}*/

		#endregion

		#region Property page methods

		/// <summary>
		/// Initializes the specified property control with the StyleCop settings file.
		/// </summary>
		public void Initialize()
		{
			UpdateWarnings();
			RebuildRuleList();
		}

		/// <summary>
		/// Apply the modifications to the StyleCop settings file.
		/// </summary>
		public bool Apply()
		{
			/*xxxforeach (ListViewItem lvi in listRules.Items)
			{
				SettingTag tag = (SettingTag)lvi.Tag;
				if (tag.Modified)
				{
					SettingsManager.SetLocalValue(Page, tag.SettingName, tag.MergedValue);
				}
				else
				{
					SettingsManager.ClearLocalValue(Page, tag.SettingName);
				}
			}*/

			return true;
		}

		/// <summary>
		/// Refreshes the state of the settings override.
		/// </summary>
		public void RefreshSettingsOverrideState()
		{
			/*xxxforeach (ListViewItem lvi in listRules.Items)
			{
				SettingTag tag = (SettingTag)lvi.Tag;
				tag.InheritedValue = SettingsManager.GetInheritedValue(Page, tag.SettingName);

				if (tag.Modified)
				{
					Page.Dirty = true;
				}
				else
				{
					tag.MergedValue = SettingsManager.GetMergedValue(Page, tag.SettingName);
				}

				UpdateListItem(lvi);
			}

			UpdateControls();*/
		}

		#endregion

		#region Displaying warnings

		/// <summary>
		/// Updates page warnings.
		/// </summary>
		public void UpdateWarnings()
		{
			warningArea.Clear();
		}

		#endregion

		#region User interface

		/// <summary>
		/// Rebuilds rule list.
		/// </summary>
		private void RebuildRuleList()
		{
			listRules.BeginUpdate();
			listRules.Groups.Clear();
			listRules.Items.Clear();

			foreach (string group in CustomRules.GetGroups())
			{
				ListViewGroup lvg = new ListViewGroup(group);
				listRules.Groups.Add(lvg);

				foreach (CustomRule rule in CustomRules.GetByGroup(group))
				{
					ListViewItem lvi = new ListViewItem();
					lvi.Group = lvg;
					lvi.UseItemStyleForSubItems = false;
					lvi.Text = rule.RuleName;
					lvi.Tag = rule;

					ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
					lvi.SubItems.Add(sub);

					UpdateListItem(lvi);

					listRules.Items.Add(lvi);
				}
			}

			listRules.EndUpdate();
		}

		/// <summary>
		/// Updates list item depending on specified properties.
		/// </summary>
		private void UpdateListItem(ListViewItem lvi)
		{
			CustomRule rule = (CustomRule)lvi.Tag;
			ListViewItem.ListViewSubItem sub = lvi.SubItems[1];

			bool enabled = SettingsGrabber.IsRuleEnabled(Page.Analyzer.Id, rule.RuleName);

			sub.Text = GetOptionsText(enabled);
			lvi.ImageKey = enabled ? Pictures.RuleEnabled : Pictures.RuleDisabled;

			if (SettingsGrabber.IsRuleBold(Page.Analyzer.Id, rule.RuleName))
			{
				sub.Font = m_bold;
			}
			else
			{
				sub.Font = SettingsGrabber.IsRuleBold(Page.Analyzer.Id, rule.RuleName) ? m_bold : m_regular;
				return;
			}

			/*xxxif (String.IsNullOrEmpty(tag.MergedValue))
			{
				lvi.ImageKey = Pictures.RuleDisabled;
				sub.Text = Resources.DoNotCheck;
			}
			else
			{
				lvi.ImageKey = Pictures.RuleEnabled;
				sub.Text = NamingMacro.BuildExample(tag.MergedValue);
			}*/
		}

		/// <summary>
		/// Updates all list items.
		/// </summary>
		private void UpdateAllListItems()
		{
			foreach (ListViewItem lvi in listRules.Items)
			{
				UpdateListItem(lvi);
			}
		}

		/// <summary>
		/// Gets options text for specified custom rule.
		/// </summary>
		private string GetOptionsText(bool enabled)
		{
			if (!enabled)
				return Resources.Disabled;

			return "asdasxxxx";
		}

		/// <summary>
		/// Updates controls for all actions.
		/// </summary>
		private void UpdateControls()
		{
			/*xxxbtnEdit.Enabled = Action_Edit_IsAvailable();
			btnReset.Enabled = Action_Reset_IsAvailable();*/

			UpdateOptions();
		}

		/// <summary>
		/// Updates option panel.
		/// </summary>
		private void UpdateOptions()
		{
			if (listRules.SelectedItems.Count != 1)
			{
				panelOptions.Controls.Clear();
				return;
			}

			ListViewItem lvi = listRules.SelectedItems[0];
			CustomRule rule = (CustomRule)lvi.Tag;

			//xxx
		}

		#endregion

		#region Actions

		/*xxx/// <summary>
		/// Does action "Edit".
		/// </summary>
		private void Action_Edit_Do()
		{
			if (!Action_Edit_IsAvailable())
				return;

			ListViewItem lvi = listRules.SelectedItems[0];
			SettingTag tag = (SettingTag)lvi.Tag;

			using (IAdvancedNamingEditor dialog = NamingSettings.GetEditor(tag.SettingName))
			{
				dialog.ObjectName = lvi.Text;
				dialog.TargetRule = tag.MergedValue;
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					tag.MergedValue = dialog.TargetRule;
					UpdateListItem(lvi);
					Page.Dirty = true;

					UpdateControls();
				}
			}
		}*/

		/*xxx/// <summary>
		/// Checks whether action "Edit" is available.
		/// </summary>
		private bool Action_Edit_IsAvailable()
		{
			if (listRules.SelectedItems.Count == 1)
				return true;

			return false;
		}*/

		/*xxx/// <summary>
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
			Page.Dirty = true;

			UpdateControls();
		}*/

		/*xxx/// <summary>
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
		}*/

		#endregion

		public void XXX()
		{
			displayExample.Display(CustomRulesResources.ExampleSP2000, "Validates the spacing at the end of the each code line.", "http://www.google.com");
		}

		public void XXX2()
		{
			displayExample.Clear();
		}
	}
}
