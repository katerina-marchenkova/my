﻿namespace Shuruev.StyleCop.CSharp
{
	partial class CustomRulesPage
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("CodeLineMustNotEndWithWhitespace");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("CodeLineMustBeginWithIdenticalWhitespaces");
			this.panelMain = new System.Windows.Forms.Panel();
			this.listRules = new System.Windows.Forms.ListView();
			this.columnRule = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnOptions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupOptions = new System.Windows.Forms.GroupBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.displayExample = new Shuruev.StyleCop.CSharp.DisplayExample();
			this.warningArea = new Shuruev.StyleCop.CSharp.WarningArea();
			this.panelMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Controls.Add(this.groupOptions);
			this.panelMain.Controls.Add(this.displayExample);
			this.panelMain.Controls.Add(this.listRules);
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 24);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(640, 456);
			this.panelMain.TabIndex = 0;
			// 
			// listRules
			// 
			this.listRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listRules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnRule,
            this.columnOptions});
			this.listRules.FullRowSelect = true;
			this.listRules.HideSelection = false;
			this.listRules.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
			this.listRules.Location = new System.Drawing.Point(3, 3);
			this.listRules.Name = "listRules";
			this.listRules.Size = new System.Drawing.Size(400, 255);
			this.listRules.TabIndex = 0;
			this.listRules.UseCompatibleStateImageBehavior = false;
			this.listRules.View = System.Windows.Forms.View.Details;
			// 
			// columnRule
			// 
			this.columnRule.Text = "Rule";
			this.columnRule.Width = 250;
			// 
			// columnOptions
			// 
			this.columnOptions.Text = "Options";
			this.columnOptions.Width = 100;
			// 
			// groupOptions
			// 
			this.groupOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupOptions.Location = new System.Drawing.Point(409, 0);
			this.groupOptions.Name = "groupOptions";
			this.groupOptions.Size = new System.Drawing.Size(221, 258);
			this.groupOptions.TabIndex = 1;
			this.groupOptions.TabStop = false;
			this.groupOptions.Text = "Rule Options";
			// 
			// btnReset
			// 
			this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReset.Location = new System.Drawing.Point(549, 253);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(75, 23);
			this.btnReset.TabIndex = 3;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			// 
			// displayExample
			// 
			this.displayExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.displayExample.Location = new System.Drawing.Point(3, 264);
			this.displayExample.Name = "displayExample";
			this.displayExample.Size = new System.Drawing.Size(637, 192);
			this.displayExample.TabIndex = 2;
			// 
			// warningArea
			// 
			this.warningArea.AutoSize = true;
			this.warningArea.Dock = System.Windows.Forms.DockStyle.Top;
			this.warningArea.Location = new System.Drawing.Point(0, 0);
			this.warningArea.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
			this.warningArea.Name = "warningArea";
			this.warningArea.Size = new System.Drawing.Size(640, 24);
			this.warningArea.TabIndex = 1;
			// 
			// CustomRulesPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.warningArea);
			this.Name = "CustomRulesPage";
			this.Size = new System.Drawing.Size(640, 480);
			this.panelMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private WarningArea warningArea;
		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.ListView listRules;
		private System.Windows.Forms.ColumnHeader columnRule;
		private System.Windows.Forms.ColumnHeader columnOptions;
		private DisplayExample displayExample;
		private System.Windows.Forms.GroupBox groupOptions;
		private System.Windows.Forms.Button btnReset;
	}
}
