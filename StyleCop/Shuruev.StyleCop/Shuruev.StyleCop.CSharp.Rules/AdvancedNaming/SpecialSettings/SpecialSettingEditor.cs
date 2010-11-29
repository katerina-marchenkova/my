﻿using System;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Dialog for editing special settings.
	/// </summary>
	public partial class SpecialSettingEditor : Form, IAdvancedNamingEditor
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public SpecialSettingEditor()
		{
			InitializeComponent();
		}

		#region Properties

		/// <summary>
		/// Gets or sets special setting to work with.
		/// </summary>
		public ISpecialSetting SpecialSetting { get; set; }

		/// <summary>
		/// Gets or sets object name.
		/// </summary>
		public string ObjectName { get; set; }

		/// <summary>
		/// Gets or sets target rule string.
		/// </summary>
		public string TargetRule { get; set; }

		#endregion

		#region Event handlers

		private void SpecialSettingEditor_Load(object sender, EventArgs e)
		{
			if (SpecialSetting == null)
				throw new InvalidOperationException("SpecialSetting is not set.");

			if (String.IsNullOrEmpty(ObjectName))
				throw new InvalidOperationException("ObjectName is not set.");

			Text = String.Format(Resources.SpecialSettingEditorCaption, ObjectName);

			labelHelp.Text = SpecialSetting.HelpText;

			if (!String.IsNullOrEmpty(TargetRule))
			{
				textEditor.RichTextBox.Text = SpecialSetting.ConvertToText(TargetRule);
			}
		}

		private void textEditor_Highlight(object sender, ControlEventArgs e)
		{
			SpecialSetting.Highlight((RichTextBox)e.Control);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (!SpecialSetting.IsValidText(textEditor.RichTextBox.Text))
			{
				Messages.ShowWarningOk(this, SpecialSetting.WarningText);
				textEditor.Focus();
				return;
			}

			TargetRule = SpecialSetting.ParseFromText(textEditor.RichTextBox.Text);
			DialogResult = DialogResult.OK;
		}

		#endregion
	}
}
