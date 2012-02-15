namespace SolutionHelper
{
	partial class RunForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunForm));
			this.pictureLogo = new System.Windows.Forms.PictureBox();
			this.panelInfoBorder = new System.Windows.Forms.Panel();
			this.panelInfo = new System.Windows.Forms.Panel();
			this.labelInfo = new System.Windows.Forms.Label();
			this.textBaseReferencePaths = new System.Windows.Forms.TextBox();
			this.labelBaseReferencePaths = new System.Windows.Forms.Label();
			this.btnAdjust = new System.Windows.Forms.Button();
			this.btnDownloadLatestLibraries = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblLocalPathInternal = new System.Windows.Forms.Label();
			this.txtLocalPathInternal = new System.Windows.Forms.TextBox();
			this.lblLocalPathExternal = new System.Windows.Forms.Label();
			this.txtLocalPathExternal = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).BeginInit();
			this.panelInfoBorder.SuspendLayout();
			this.panelInfo.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureLogo
			// 
			this.pictureLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureLogo.Image")));
			this.pictureLogo.Location = new System.Drawing.Point(0, 0);
			this.pictureLogo.Name = "pictureLogo";
			this.pictureLogo.Size = new System.Drawing.Size(400, 60);
			this.pictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureLogo.TabIndex = 0;
			this.pictureLogo.TabStop = false;
			// 
			// panelInfoBorder
			// 
			this.panelInfoBorder.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.panelInfoBorder.Controls.Add(this.panelInfo);
			this.panelInfoBorder.Location = new System.Drawing.Point(6, 66);
			this.panelInfoBorder.Name = "panelInfoBorder";
			this.panelInfoBorder.Size = new System.Drawing.Size(382, 84);
			this.panelInfoBorder.TabIndex = 0;
			// 
			// panelInfo
			// 
			this.panelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelInfo.BackColor = System.Drawing.SystemColors.Info;
			this.panelInfo.Controls.Add(this.labelInfo);
			this.panelInfo.Location = new System.Drawing.Point(1, 1);
			this.panelInfo.Name = "panelInfo";
			this.panelInfo.Size = new System.Drawing.Size(380, 82);
			this.panelInfo.TabIndex = 0;
			// 
			// labelInfo
			// 
			this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInfo.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.labelInfo.Location = new System.Drawing.Point(8, 8);
			this.labelInfo.Margin = new System.Windows.Forms.Padding(8);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(364, 66);
			this.labelInfo.TabIndex = 0;
			this.labelInfo.Text = "This tool will adjust your solution to the build process by updating:\r\n   - Refer" +
    "ence Paths\r\n   - Project Dependencies\r\n\r\nSet base reference paths and press \"Adj" +
    "ust Solution\" button.";
			// 
			// textBaseReferencePaths
			// 
			this.textBaseReferencePaths.Location = new System.Drawing.Point(12, 177);
			this.textBaseReferencePaths.Name = "textBaseReferencePaths";
			this.textBaseReferencePaths.Size = new System.Drawing.Size(370, 20);
			this.textBaseReferencePaths.TabIndex = 2;
			// 
			// labelBaseReferencePaths
			// 
			this.labelBaseReferencePaths.AutoSize = true;
			this.labelBaseReferencePaths.Location = new System.Drawing.Point(9, 161);
			this.labelBaseReferencePaths.Name = "labelBaseReferencePaths";
			this.labelBaseReferencePaths.Size = new System.Drawing.Size(335, 13);
			this.labelBaseReferencePaths.TabIndex = 1;
			this.labelBaseReferencePaths.Text = "Base reference paths (your local folders containing binary references):";
			// 
			// btnAdjust
			// 
			this.btnAdjust.Location = new System.Drawing.Point(254, 203);
			this.btnAdjust.Name = "btnAdjust";
			this.btnAdjust.Size = new System.Drawing.Size(128, 23);
			this.btnAdjust.TabIndex = 3;
			this.btnAdjust.Text = "Adjust Solution";
			this.btnAdjust.UseVisualStyleBackColor = true;
			this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);
			// 
			// btnDownloadLatestLibraries
			// 
			this.btnDownloadLatestLibraries.Location = new System.Drawing.Point(207, 86);
			this.btnDownloadLatestLibraries.Name = "btnDownloadLatestLibraries";
			this.btnDownloadLatestLibraries.Size = new System.Drawing.Size(175, 23);
			this.btnDownloadLatestLibraries.TabIndex = 4;
			this.btnDownloadLatestLibraries.Text = "Download Latest References";
			this.btnDownloadLatestLibraries.UseVisualStyleBackColor = true;
			this.btnDownloadLatestLibraries.Click += new System.EventHandler(this.btnDownloadLatestLibraries_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.txtLocalPathExternal);
			this.panel1.Controls.Add(this.lblLocalPathExternal);
			this.panel1.Controls.Add(this.txtLocalPathInternal);
			this.panel1.Controls.Add(this.lblLocalPathInternal);
			this.panel1.Controls.Add(this.btnDownloadLatestLibraries);
			this.panel1.Location = new System.Drawing.Point(0, 232);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(393, 119);
			this.panel1.TabIndex = 5;
			// 
			// lblLocalPathInternal
			// 
			this.lblLocalPathInternal.AutoSize = true;
			this.lblLocalPathInternal.Location = new System.Drawing.Point(9, 0);
			this.lblLocalPathInternal.Name = "lblLocalPathInternal";
			this.lblLocalPathInternal.Size = new System.Drawing.Size(247, 13);
			this.lblLocalPathInternal.TabIndex = 5;
			this.lblLocalPathInternal.Text = "Please enter your local path for Internal references:";
			// 
			// txtLocalPathInternal
			// 
			this.txtLocalPathInternal.Location = new System.Drawing.Point(12, 16);
			this.txtLocalPathInternal.Name = "txtLocalPathInternal";
			this.txtLocalPathInternal.Size = new System.Drawing.Size(370, 20);
			this.txtLocalPathInternal.TabIndex = 6;
			// 
			// lblLocalPathExternal
			// 
			this.lblLocalPathExternal.AutoSize = true;
			this.lblLocalPathExternal.Location = new System.Drawing.Point(12, 43);
			this.lblLocalPathExternal.Name = "lblLocalPathExternal";
			this.lblLocalPathExternal.Size = new System.Drawing.Size(250, 13);
			this.lblLocalPathExternal.TabIndex = 7;
			this.lblLocalPathExternal.Text = "Please enter your local path for External references:";
			// 
			// txtLocalPathExternal
			// 
			this.txtLocalPathExternal.Location = new System.Drawing.Point(12, 60);
			this.txtLocalPathExternal.Name = "txtLocalPathExternal";
			this.txtLocalPathExternal.Size = new System.Drawing.Size(370, 20);
			this.txtLocalPathExternal.TabIndex = 8;
			// 
			// RunForm
			// 
			this.AcceptButton = this.btnAdjust;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(394, 354);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnAdjust);
			this.Controls.Add(this.labelBaseReferencePaths);
			this.Controls.Add(this.textBaseReferencePaths);
			this.Controls.Add(this.panelInfoBorder);
			this.Controls.Add(this.pictureLogo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RunForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "[AddIn Name]";
			this.Load += new System.EventHandler(this.RunForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RunForm_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).EndInit();
			this.panelInfoBorder.ResumeLayout(false);
			this.panelInfo.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureLogo;
		private System.Windows.Forms.Panel panelInfoBorder;
		private System.Windows.Forms.Panel panelInfo;
		private System.Windows.Forms.Label labelInfo;
		private System.Windows.Forms.TextBox textBaseReferencePaths;
		private System.Windows.Forms.Label labelBaseReferencePaths;
		private System.Windows.Forms.Button btnAdjust;
		private System.Windows.Forms.Button btnDownloadLatestLibraries;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox txtLocalPathExternal;
		private System.Windows.Forms.Label lblLocalPathExternal;
		private System.Windows.Forms.TextBox txtLocalPathInternal;
		private System.Windows.Forms.Label lblLocalPathInternal;
	}
}