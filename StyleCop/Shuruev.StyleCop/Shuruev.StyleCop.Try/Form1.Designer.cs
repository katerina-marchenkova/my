namespace Shuruev.StyleCop.Try
{
	partial class Form1
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
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnEntity = new System.Windows.Forms.ColumnHeader();
			this.columnPreview = new System.Windows.Forms.ColumnHeader();
			this.highlightTextBox1 = new Shuruev.StyleCop.CSharp.HighlightTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEntity,
            this.columnPreview});
			this.listView1.Location = new System.Drawing.Point(397, 37);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(255, 198);
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnEntity
			// 
			this.columnEntity.Text = "Entity";
			this.columnEntity.Width = 97;
			// 
			// columnPreview
			// 
			this.columnPreview.Text = "Preview";
			this.columnPreview.Width = 122;
			// 
			// highlightTextBox1
			// 
			this.highlightTextBox1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.highlightTextBox1.Location = new System.Drawing.Point(72, 159);
			this.highlightTextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.highlightTextBox1.Name = "highlightTextBox1";
			this.highlightTextBox1.Size = new System.Drawing.Size(261, 150);
			this.highlightTextBox1.TabIndex = 1;
			this.highlightTextBox1.Highlight += new System.Windows.Forms.ControlEventHandler(this.highlightTextBox1_Highlight);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(48, 57);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(905, 520);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.highlightTextBox1);
			this.Controls.Add(this.listView1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnEntity;
		private System.Windows.Forms.ColumnHeader columnPreview;
		private Shuruev.StyleCop.CSharp.HighlightTextBox highlightTextBox1;
		private System.Windows.Forms.Button button1;

	}
}

