using System;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Dialog for editing abbreviations.
	/// </summary>
	public partial class AbbreviationsEditor : Form
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public AbbreviationsEditor()
		{
			InitializeComponent();
		}

		#region Properties

		/// <summary>
		/// Gets or sets object name.
		/// </summary>
		public string ObjectName { get; set; }

		/// <summary>
		/// Gets or sets abbreviations string.
		/// </summary>
		public string Abbreviations { get; set; }

		#endregion

		#region Event handlers

		private void AbbreviationsEditor_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(ObjectName))
				throw new InvalidOperationException("ObjectName is not set.");

			Text = String.Format(Resources.CommonEditorCaption, ObjectName);

			if (!String.IsNullOrEmpty(Abbreviations))
			{
				textEditor.RichTextBox.Text = NamingMacro.ConvertAbbreviationsToText(Abbreviations);
			}
		}

		private void textEditor_Highlight(object sender, ControlEventArgs e)
		{
			NamingMacro.HighlightAbbreviations((RichTextBox)e.Control);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (!NamingMacro.CheckAbbreviations(textEditor.RichTextBox.Text))
			{
				Messages.ShowWarningOk(this, Resources.AbbreviationsEditorWarning);
				textEditor.Focus();
				return;
			}

			Abbreviations = NamingMacro.ParseAbbreviationsFromText(textEditor.RichTextBox.Text);
			DialogResult = DialogResult.OK;
		}

		#endregion
	}
}
