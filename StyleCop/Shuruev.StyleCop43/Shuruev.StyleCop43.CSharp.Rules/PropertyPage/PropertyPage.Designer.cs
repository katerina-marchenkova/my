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
			this.tabPages = new System.Windows.Forms.TabControl();
			this.pageNaming = new System.Windows.Forms.TabPage();
			this.namingRulesPage = new Shuruev.StyleCop.CSharp.NamingRulesPage();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.pictureLogo = new System.Windows.Forms.PictureBox();
			this.panelLogoBorder = new System.Windows.Forms.Panel();
			this.panelTitleBorder = new System.Windows.Forms.Panel();
			this.panelTitle = new System.Windows.Forms.Panel();
			this.pictureTitle = new System.Windows.Forms.PictureBox();
			this.tabPages.SuspendLayout();
			this.pageNaming.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).BeginInit();
			this.panelLogoBorder.SuspendLayout();
			this.panelTitleBorder.SuspendLayout();
			this.panelTitle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureTitle)).BeginInit();
			this.SuspendLayout();
			// 
			// tabPages
			// 
			this.tabPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabPages.Controls.Add(this.pageNaming);
			this.tabPages.Controls.Add(this.tabPage1);
			this.tabPages.Location = new System.Drawing.Point(3, 75);
			this.tabPages.Name = "tabPages";
			this.tabPages.SelectedIndex = 0;
			this.tabPages.Size = new System.Drawing.Size(634, 402);
			this.tabPages.TabIndex = 2;
			// 
			// pageNaming
			// 
			this.pageNaming.Controls.Add(this.namingRulesPage);
			this.pageNaming.Location = new System.Drawing.Point(4, 22);
			this.pageNaming.Name = "pageNaming";
			this.pageNaming.Padding = new System.Windows.Forms.Padding(3);
			this.pageNaming.Size = new System.Drawing.Size(626, 376);
			this.pageNaming.TabIndex = 1;
			this.pageNaming.Text = "Advanced Naming Rules";
			this.pageNaming.UseVisualStyleBackColor = true;
			// 
			// namingRulesPage
			// 
			this.namingRulesPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.namingRulesPage.Location = new System.Drawing.Point(3, 3);
			this.namingRulesPage.Name = "namingRulesPage";
			this.namingRulesPage.Page = null;
			this.namingRulesPage.Size = new System.Drawing.Size(620, 370);
			this.namingRulesPage.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(626, 376);
			this.tabPage1.TabIndex = 2;
			this.tabPage1.Text = "tabPage1xxx";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// pictureLogo
			// 
			this.pictureLogo.Image = global::Shuruev.StyleCop.CSharp.Properties.Resources.StyleCopPlusLogo;
			this.pictureLogo.Location = new System.Drawing.Point(1, 1);
			this.pictureLogo.Margin = new System.Windows.Forms.Padding(1);
			this.pictureLogo.Name = "pictureLogo";
			this.pictureLogo.Size = new System.Drawing.Size(64, 64);
			this.pictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureLogo.TabIndex = 5;
			this.pictureLogo.TabStop = false;
			// 
			// panelLogoBorder
			// 
			this.panelLogoBorder.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelLogoBorder.Controls.Add(this.pictureLogo);
			this.panelLogoBorder.Location = new System.Drawing.Point(3, 3);
			this.panelLogoBorder.Name = "panelLogoBorder";
			this.panelLogoBorder.Size = new System.Drawing.Size(66, 66);
			this.panelLogoBorder.TabIndex = 0;
			// 
			// panelTitleBorder
			// 
			this.panelTitleBorder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelTitleBorder.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelTitleBorder.Controls.Add(this.panelTitle);
			this.panelTitleBorder.Location = new System.Drawing.Point(75, 3);
			this.panelTitleBorder.Name = "panelTitleBorder";
			this.panelTitleBorder.Size = new System.Drawing.Size(562, 66);
			this.panelTitleBorder.TabIndex = 1;
			// 
			// panelTitle
			// 
			this.panelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelTitle.BackColor = System.Drawing.Color.White;
			this.panelTitle.Controls.Add(this.pictureTitle);
			this.panelTitle.Location = new System.Drawing.Point(1, 1);
			this.panelTitle.Margin = new System.Windows.Forms.Padding(1);
			this.panelTitle.Name = "panelTitle";
			this.panelTitle.Size = new System.Drawing.Size(560, 64);
			this.panelTitle.TabIndex = 0;
			// 
			// pictureTitle
			// 
			this.pictureTitle.Image = global::Shuruev.StyleCop.CSharp.Properties.Resources.StyleCopPlusTitle;
			this.pictureTitle.Location = new System.Drawing.Point(0, 0);
			this.pictureTitle.Name = "pictureTitle";
			this.pictureTitle.Size = new System.Drawing.Size(250, 64);
			this.pictureTitle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureTitle.TabIndex = 0;
			this.pictureTitle.TabStop = false;
			// 
			// PropertyPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelTitleBorder);
			this.Controls.Add(this.panelLogoBorder);
			this.Controls.Add(this.tabPages);
			this.Name = "PropertyPage";
			this.Size = new System.Drawing.Size(640, 480);
			this.tabPages.ResumeLayout(false);
			this.pageNaming.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).EndInit();
			this.panelLogoBorder.ResumeLayout(false);
			this.panelLogoBorder.PerformLayout();
			this.panelTitleBorder.ResumeLayout(false);
			this.panelTitle.ResumeLayout(false);
			this.panelTitle.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureTitle)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabPages;
		private System.Windows.Forms.TabPage pageNaming;
		private Shuruev.StyleCop.CSharp.NamingRulesPage namingRulesPage;
		private System.Windows.Forms.PictureBox pictureLogo;
		private System.Windows.Forms.Panel panelLogoBorder;
		private System.Windows.Forms.Panel panelTitleBorder;
		private System.Windows.Forms.Panel panelTitle;
		private System.Windows.Forms.PictureBox pictureTitle;
		private System.Windows.Forms.TabPage tabPage1;
	}
}
