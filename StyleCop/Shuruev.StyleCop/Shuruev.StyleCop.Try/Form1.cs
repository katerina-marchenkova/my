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
		private string yyy = null;

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

		private void button2_Click(object sender, EventArgs e)
		{
			using (AbbreviationsEditor dialog = new AbbreviationsEditor())
			{
				dialog.Abbreviations = yyy;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					yyy = dialog.Abbreviations;
				}
			}
		}
	}
}
