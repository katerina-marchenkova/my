namespace Shuruev.StyleCop.CSharp
{
	partial class PropertyPage
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.tabPages = new System.Windows.Forms.TabControl();
			this.pageNaming = new System.Windows.Forms.TabPage();
			this.namingRulesPage = new Shuruev.StyleCop.CSharp.AdvancedNaming.NamingRulesPage();
			this.tabPages.SuspendLayout();
			this.pageNaming.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(44, 3);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(151, 20);
			this.textBox1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(485, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Reset";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(201, 5);
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(258, 20);
			this.textBox2.TabIndex = 3;
			// 
			// tabPages
			// 
			this.tabPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabPages.Controls.Add(this.pageNaming);
			this.tabPages.Location = new System.Drawing.Point(3, 43);
			this.tabPages.Name = "tabPages";
			this.tabPages.SelectedIndex = 0;
			this.tabPages.Size = new System.Drawing.Size(634, 434);
			this.tabPages.TabIndex = 4;
			// 
			// pageNaming
			// 
			this.pageNaming.Controls.Add(this.namingRulesPage);
			this.pageNaming.Location = new System.Drawing.Point(4, 22);
			this.pageNaming.Name = "pageNaming";
			this.pageNaming.Padding = new System.Windows.Forms.Padding(3);
			this.pageNaming.Size = new System.Drawing.Size(626, 408);
			this.pageNaming.TabIndex = 1;
			this.pageNaming.Text = "Advanced Naming Rules";
			this.pageNaming.UseVisualStyleBackColor = true;
			// 
			// namingRulesPage
			// 
			this.namingRulesPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.namingRulesPage.Location = new System.Drawing.Point(3, 3);
			this.namingRulesPage.Name = "namingRulesPage";
			this.namingRulesPage.Size = new System.Drawing.Size(620, 402);
			this.namingRulesPage.TabIndex = 0;
			// 
			// PropertyPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabPages);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Name = "PropertyPage";
			this.Size = new System.Drawing.Size(640, 480);
			this.tabPages.ResumeLayout(false);
			this.pageNaming.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TabControl tabPages;
		private System.Windows.Forms.TabPage pageNaming;
		private Shuruev.StyleCop.CSharp.AdvancedNaming.NamingRulesPage namingRulesPage;
	}
}
