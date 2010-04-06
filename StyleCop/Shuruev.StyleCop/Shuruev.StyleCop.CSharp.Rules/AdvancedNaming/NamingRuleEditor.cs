using System;
using System.Drawing;
using System.Windows.Forms;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Dialog for editing naming rule.
	/// </summary>
	public partial class NamingRuleEditor : Form
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public NamingRuleEditor()
		{
			InitializeComponent();
		}

		#region Event handlers

		private void NamingRuleEditor_Load(object sender, EventArgs e)
		{
			UpdateMacroList();
			UpdateControls();
		}

		private void textEditor_Highlight(object sender, ControlEventArgs e)
		{
			NamingMacro.Highlight((RichTextBox)e.Control);
		}

		private void listMacro_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void listMacro_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			Action_Insert_Do();
		}

		private void checkDisable_CheckedChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void btnInsert_Click(object sender, EventArgs e)
		{
			Action_Insert_Do();
		}

		#endregion

		#region User interface

		/// <summary>
		/// Updates macro list.
		/// </summary>
		private void UpdateMacroList()
		{
			listMacro.BeginUpdate();
			listMacro.Items.Clear();

			foreach (string key in NamingMacro.GetKeys())
			{
				string descripion = NamingMacro.GetDescription(key);

				ListViewItem lvi = new ListViewItem();
				lvi.UseItemStyleForSubItems = false;
				lvi.Text = key;

				ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem(lvi, descripion);
				sub.ForeColor = SystemColors.GrayText;
				lvi.SubItems.Add(sub);

				listMacro.Items.Add(lvi);
			}

			listMacro.EndUpdate();
		}

		/// <summary>
		/// Updates controls for all actions.
		/// </summary>
		private void UpdateControls()
		{
			bool enabled = checkDisable.Checked;
			if (enabled)
			{
				textEditor.ReadOnly = true;
				btnInsert.Enabled = false;
			}
			else
			{
				textEditor.ReadOnly = false;
				btnInsert.Enabled = Action_Insert_IsAvailable();
			}
		}

		#endregion

		#region Actions

		/// <summary>
		/// Does action "Insert".
		/// </summary>
		private void Action_Insert_Do()
		{
			if (!Action_Insert_IsAvailable())
				return;

			ListViewItem lvi = listMacro.SelectedItems[0];
			string key = lvi.Text;
			string text = NamingMacro.GetMarkup(key);

			textEditor.RichTextBox.SelectedText = text;
		}

		/// <summary>
		/// Checks whether action "Insert" is available.
		/// </summary>
		private bool Action_Insert_IsAvailable()
		{
			if (listMacro.SelectedItems.Count == 1)
				return true;

			return false;
		}

		#endregion
	}
}
