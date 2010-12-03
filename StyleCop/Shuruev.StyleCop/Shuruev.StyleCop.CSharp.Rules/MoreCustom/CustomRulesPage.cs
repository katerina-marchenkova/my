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
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRulesPage()
		{
			InitializeComponent();

			listRules.SmallImageList = Pictures.GetList();
			listRules.Items[0].ImageKey = Pictures.RuleEnabled;
			listRules.Items[1].ImageKey = Pictures.RuleEnabled;
		}

		#region Properties

		/// <summary>
		/// Gets or sets parent property page.
		/// </summary>
		public PropertyPage Page { get; set; }

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
			/*xxxlistRules.BeginUpdate();
			listRules.Groups.Clear();
			listRules.Items.Clear();

			foreach (string group in NamingSettings.GetGroups())
			{
				ListViewGroup lvg = new ListViewGroup(group);
				listRules.Groups.Add(lvg);

				foreach (string setting in NamingSettings.GetByGroup(group))
				{
					string friendlyName = SettingsManager.GetFriendlyName(Page, setting);
					string mergedValue = SettingsManager.GetMergedValue(Page, setting);
					string inheritedValue = SettingsManager.GetInheritedValue(Page, setting);

					SettingTag tag = new SettingTag();
					tag.SettingName = setting;
					tag.MergedValue = mergedValue;
					tag.InheritedValue = inheritedValue;

					ListViewItem lvi = new ListViewItem();
					lvi.Group = lvg;
					lvi.UseItemStyleForSubItems = false;
					lvi.Text = friendlyName;
					lvi.Tag = tag;

					ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
					lvi.SubItems.Add(sub);

					UpdateListItem(lvi);

					listRules.Items.Add(lvi);
				}
			}

			listRules.EndUpdate();*/
		}

		/// <summary>
		/// Updates list item depending on specified properties.
		/// </summary>
		private void UpdateListItem(ListViewItem lvi)
		{
			/*xxxSettingTag tag = (SettingTag)lvi.Tag;
			ListViewItem.ListViewSubItem sub = lvi.SubItems[1];

			sub.Font = tag.Modified ?
				m_bold :
				m_regular;

			if (tag.SettingName == NamingSettings.Abbreviations)
			{
				lvi.ImageKey = Pictures.CapitalLetter;
				sub.Text = tag.MergedValue;
				return;
			}

			if (tag.SettingName == NamingSettings.Words)
			{
				lvi.ImageKey = Pictures.TwoLetters;
				sub.Text = tag.MergedValue;
				return;
			}

			if (tag.SettingName == NamingSettings.Derivings)
			{
				lvi.ImageKey = Pictures.RightArrow;
				sub.Text = tag.MergedValue;
				return;
			}

			if (String.IsNullOrEmpty(tag.MergedValue))
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
		/// Updates controls for all actions.
		/// </summary>
		private void UpdateControls()
		{
			/*xxxbtnEdit.Enabled = Action_Edit_IsAvailable();
			btnReset.Enabled = Action_Reset_IsAvailable();*/
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
			displayExample.Display(Resources.ExampleSP1000, "Validates the spacing at the end of the each code line.", "http://www.google.com");
		}

		public void XXX2()
		{
			displayExample.Clear();
		}
	}
}
