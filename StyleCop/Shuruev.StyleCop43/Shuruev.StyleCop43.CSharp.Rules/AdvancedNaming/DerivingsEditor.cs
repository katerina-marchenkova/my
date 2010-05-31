using System;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Dialog for editing derivings.
	/// </summary>
	public partial class DerivingsEditor : Form
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public DerivingsEditor()
		{
			InitializeComponent();
		}

		#region Properties

		/// <summary>
		/// Gets or sets object name.
		/// </summary>
		public string ObjectName { get; set; }

		/// <summary>
		/// Gets or sets derivings string.
		/// </summary>
		public string Derivings { get; set; }

		#endregion

		#region Event handlers

		private void DerivingsEditor_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(ObjectName))
				throw new InvalidOperationException("ObjectName is not set.");

			Text = String.Format(Resources.CommonEditorCaption, ObjectName);

			if (!String.IsNullOrEmpty(Derivings))
			{
				textEditor.RichTextBox.Text = NamingMacro.ConvertDerivingsToText(Derivings);
			}
		}

		private void textEditor_Highlight(object sender, ControlEventArgs e)
		{
			NamingMacro.HighlightDerivings((RichTextBox)e.Control);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (!NamingMacro.CheckDerivings(textEditor.RichTextBox.Text))
			{
				Messages.ShowWarningOk(this, Resources.DerivingsEditorWarning);
				textEditor.Focus();
				return;
			}

			Derivings = NamingMacro.ParseDerivingsFromText(textEditor.RichTextBox.Text);
			DialogResult = DialogResult.OK;
		}

		#endregion
	}
}
