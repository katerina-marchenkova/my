namespace Shuruev.StyleCop.Try
{
	partial class UserControl1
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.warningArea1 = new Shuruev.StyleCop.CSharp.WarningArea();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 24);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(610, 330);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// warningArea1
			// 
			this.warningArea1.AutoSize = true;
			this.warningArea1.Dock = System.Windows.Forms.DockStyle.Top;
			this.warningArea1.Location = new System.Drawing.Point(0, 0);
			this.warningArea1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
			this.warningArea1.Name = "warningArea1";
			this.warningArea1.Size = new System.Drawing.Size(610, 24);
			this.warningArea1.TabIndex = 0;
			// 
			// UserControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.warningArea1);
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(610, 354);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Shuruev.StyleCop.CSharp.WarningArea warningArea1;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
