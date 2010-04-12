using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp;

namespace Shuruev.StyleCop.Try
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private string xxx = "I$(AaBb)";

		private void button1_Click(object sender, EventArgs e)
		{
			using (NamingRuleEditor dialog = new NamingRuleEditor())
			{
				dialog.ObjectName = "Interface";
				dialog.RuleDefinition = xxx;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					xxx = dialog.RuleDefinition;
				}
			}
		}
	}
}
