﻿namespace Shuruev.StyleCop.CSharp
{
	partial class CustomRuleIndentOptions
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.radioTabs = new System.Windows.Forms.RadioButton();
			this.radioSpaces = new System.Windows.Forms.RadioButton();
			this.radioBoth = new System.Windows.Forms.RadioButton();
			this.labelTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// radioTabs
			// 
			this.radioTabs.Location = new System.Drawing.Point(0, 26);
			this.radioTabs.Name = "radioTabs";
			this.radioTabs.Size = new System.Drawing.Size(180, 23);
			this.radioTabs.TabIndex = 1;
			this.radioTabs.TabStop = true;
			this.radioTabs.Text = "All lines must use tabs";
			this.radioTabs.UseVisualStyleBackColor = true;
			this.radioTabs.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
			// 
			// radioSpaces
			// 
			this.radioSpaces.Location = new System.Drawing.Point(0, 55);
			this.radioSpaces.Name = "radioSpaces";
			this.radioSpaces.Size = new System.Drawing.Size(180, 23);
			this.radioSpaces.TabIndex = 2;
			this.radioSpaces.TabStop = true;
			this.radioSpaces.Text = "All lines must use spaces";
			this.radioSpaces.UseVisualStyleBackColor = true;
			this.radioSpaces.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
			// 
			// radioBoth
			// 
			this.radioBoth.Location = new System.Drawing.Point(0, 84);
			this.radioBoth.Name = "radioBoth";
			this.radioBoth.Size = new System.Drawing.Size(180, 48);
			this.radioBoth.TabIndex = 3;
			this.radioBoth.TabStop = true;
			this.radioBoth.Text = "Some lines can use spaces, some can use tabs\r\n(supporting legacy code)";
			this.radioBoth.UseVisualStyleBackColor = true;
			this.radioBoth.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(0, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(180, 23);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "Indentation character:";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// CustomRuleIndentOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.radioBoth);
			this.Controls.Add(this.radioSpaces);
			this.Controls.Add(this.radioTabs);
			this.Name = "CustomRuleIndentOptions";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RadioButton radioTabs;
		private System.Windows.Forms.RadioButton radioSpaces;
		private System.Windows.Forms.RadioButton radioBoth;
		private System.Windows.Forms.Label labelTitle;

	}
}
