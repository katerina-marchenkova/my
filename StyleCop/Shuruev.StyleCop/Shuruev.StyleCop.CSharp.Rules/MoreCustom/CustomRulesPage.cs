using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp.MoreCustom
{
	public partial class CustomRulesPage : UserControl
	{
		public CustomRulesPage()
		{
			InitializeComponent();
		}

		public void XXX()
		{
			displayExample.Display(Resources.ExampleSP1000, "http://www.google.com");
		}

		public void XXX2()
		{
			displayExample.Clear();
		}
	}
}
