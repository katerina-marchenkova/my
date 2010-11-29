using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Shuruev.StyleCop.Try
{
	public partial class UserControl1 : UserControl
	{
		public UserControl1()
		{
			InitializeComponent();
		}

		public void Add(string text)
		{
			warningArea1.Add(text, "http://www.ya.ru");
		}
	}
}
