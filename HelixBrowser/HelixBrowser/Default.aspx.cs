using System;
using System.Web.UI;

namespace HelixBrowser
{
	public partial class Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Redirect("TestDemo.aspx?nodeId=00000000-0000-0000-0011-00000000000B");
			//Response.Redirect("TestDemo.aspx?nodeId=3A6773F5-70C7-468D-BA88-0C048338ED4C");
			//Response.Redirect("TestDemo.aspx?nodeId=865EAFC1-CF97-44AD-99BC-AF5B2F804310");
			//Response.Redirect("TestReport.aspx");
		}
	}
}