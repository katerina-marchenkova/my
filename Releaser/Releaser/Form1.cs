using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shuruev.Releaser
{
	/// <summary>
	/// This is a form.
	/// </summary>
	public partial class Form1 : Form
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Form1"/> class.
		/// </summary>
		public Form1()
		{
			this.InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Type type = Type.GetType("VX.Storage.VXMString, VXStorage", true, false);
			object o = Activator.CreateInstance(type);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			int a = 10;
		}
	}
}
