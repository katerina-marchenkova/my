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
			this.namingRulesPage = new Shuruev.StyleCop.CSharp.AdvancedNaming.NamingRulesPage();
			this.pictureLogo = new System.Windows.Forms.PictureBox();
			this.panelLogoBorder = new System.Windows.Forms.Panel();
			this.panelTitleBorder = new System.Windows.Forms.Panel();
			this.panelTitle = new System.Windows.Forms.Panel();
			this.labelTitle = new System.Windows.Forms.Label();
			this.labelSubtitle = new System.Windows.Forms.Label();
			this.tabPages.SuspendLayout();
			this.pageNaming.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).BeginInit();
			this.panelLogoBorder.SuspendLayout();
			this.panelTitleBorder.SuspendLayout();
			this.panelTitle.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPages
			// 
			this.tabPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabPages.Controls.Add(this.pageNaming);
			this.tabPages.Location = new System.Drawing.Point(3, 75);
			this.tabPages.Name = "tabPages";
			this.tabPages.SelectedIndex = 0;
			this.tabPages.Size = new System.Drawing.Size(634, 402);
			this.tabPages.TabIndex = 4;
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
			this.panelLogoBorder.TabIndex = 6;
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
			this.panelTitleBorder.TabIndex = 7;
			// 
			// panelTitle
			// 
			this.panelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelTitle.BackColor = System.Drawing.Color.White;
			this.panelTitle.Controls.Add(this.labelSubtitle);
			this.panelTitle.Controls.Add(this.labelTitle);
			this.panelTitle.Location = new System.Drawing.Point(1, 1);
			this.panelTitle.Margin = new System.Windows.Forms.Padding(1);
			this.panelTitle.Name = "panelTitle";
			this.panelTitle.Size = new System.Drawing.Size(560, 64);
			this.panelTitle.TabIndex = 0;
			// 
			// labelTitle
			// 
			this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelTitle.Font = new System.Drawing.Font("Cambria", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelTitle.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelTitle.Location = new System.Drawing.Point(3, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(555, 36);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "StyleCop+";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelSubtitle
			// 
			this.labelSubtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelSubtitle.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelSubtitle.ForeColor = System.Drawing.Color.LightSlateGray;
			this.labelSubtitle.Location = new System.Drawing.Point(2, 36);
			this.labelSubtitle.Name = "labelSubtitle";
			this.labelSubtitle.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.labelSubtitle.Size = new System.Drawing.Size(556, 26);
			this.labelSubtitle.TabIndex = 1;
			this.labelSubtitle.Text = "Settings Page";
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
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabPages;
		private System.Windows.Forms.TabPage pageNaming;
		private Shuruev.StyleCop.CSharp.AdvancedNaming.NamingRulesPage namingRulesPage;
		private System.Windows.Forms.PictureBox pictureLogo;
		private System.Windows.Forms.Panel panelLogoBorder;
		private System.Windows.Forms.Panel panelTitleBorder;
		private System.Windows.Forms.Panel panelTitle;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Label labelSubtitle;
	}
}
