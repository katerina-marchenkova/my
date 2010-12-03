using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Control displaying custom rules page.
	/// </summary>
	public partial class CustomRulesPage : UserControl
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CustomRulesPage()
		{
			InitializeComponent();

			listRules.SmallImageList = Pictures.GetList();
			listRules.Items[0].ImageKey = Pictures.RuleEnabled;
			listRules.Items[1].ImageKey = Pictures.RuleEnabled;
		}

		public void XXX()
		{
			displayExample.Display(Resources.ExampleSP1000, "Validates the spacing at the end of the each code line.", "http://www.google.com");
		}

		public void XXX2()
		{
			displayExample.Clear();
		}
	}
}
