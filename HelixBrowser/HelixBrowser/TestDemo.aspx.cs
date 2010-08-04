using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using VX.Storage;
using VX.Storage.Adapter;
using VX.Storage.Adapter.Sql;
using VX.Sys;
using VX.Knowledge.DataSource;
using VX.Knowledge.DataSource.Model;

namespace HelixBrowser
{
	public partial class TestDemo : System.Web.UI.Page
	{
		protected VXGuid m_nodeId;
		protected List<RenderedItem> m_renderedItems;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!ReadNodeId())
				Response.Redirect("Error.aspx");

			string vxkConnection = ConfigurationHelper.ReadString("VXK.Connection");
			TimeSpan vxkTimeout = ConfigurationHelper.ReadTimeSpan("VXK.Timeout");

			VXStorageConnection conn = new VXSqlStorageConnection();
			conn.Parse(vxkConnection);
			VXKManager vxk = new VXKSqlManager(conn);
			vxk.CommandTimeout = vxkTimeout;

			DataMap map = new DataMap();
			map.Load(vxk, m_nodeId);

			m_renderedItems = map.BuildRenderedItems();
		}

		private bool ReadNodeId()
		{
			string nodeId = Request["nodeId"];

			if (String.IsNullOrEmpty(nodeId))
				return false;

			try
			{
				m_nodeId = new VXGuid(nodeId);
			}
			catch
			{
				return false;
			}

			return true;
		}
	}
}