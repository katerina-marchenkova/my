using System;
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

		private string m_test1 = "I$(AaBb)";
		private string m_test2 = null;

		private void button1_Click(object sender, EventArgs e)
		{
			using (NamingRuleEditor dialog = new NamingRuleEditor())
			{
				dialog.ObjectName = "Interface";
				dialog.TargetRule = m_test1;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					m_test1 = dialog.TargetRule;
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			using (SpecialSettingEditor dialog = new SpecialSettingEditor())
			{
				dialog.SpecialSetting = new DerivingsSpecialSetting();
				dialog.ObjectName = "Test";
				dialog.TargetRule = m_test2;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					m_test2 = dialog.TargetRule;
				}
			}
		}
	}
}
