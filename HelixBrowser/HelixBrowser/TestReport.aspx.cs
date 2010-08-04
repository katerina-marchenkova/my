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
using System.Data.SqlClient;

namespace HelixBrowser
{
	public partial class TestReport : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string vxkConnection = "data source=db-vxk.svc.cnprod.cnwk; initial catalog=vxk; user id=vxkuser; password=#vxkuser#;";
			TimeSpan vxkTimeout = TimeSpan.Parse("00:01:00");

			VXStorageConnection conn = new VXSqlStorageConnection();
			conn.Parse(vxkConnection);
			VXKManager vxk = new VXKSqlManager(conn);
			vxk.CommandTimeout = vxkTimeout;

			using (SqlConnection conn1 = new SqlConnection(@"Data Source=RUFRW-OSHU\SQL2005; Initial Catalog=Work; Integrated Security=SSPI;"))
			{
				conn1.Open();

				using (SqlCommand cmd = conn1.CreateCommand())
				{
					cmd.CommandText = "SELECT * FROM [Result] ORDER BY [ItemUid], [ParentTree]";

					Guid lastItemUid = Guid.Empty;
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							Guid itemUid = (Guid)reader["ItemUid"];
							if (itemUid != lastItemUid)
							{
								VXInfoItem item = KB.Template.Multipleimages.ReadItem(vxk, (VXGuid)itemUid);
								RenderedItem render = new RenderedItem();
								render.InfoItem = item;
								string html = render.Render();
								html = html.Replace("50px;", "200px;");
								html = html.Replace("37px;", "150px;");
								Response.Write(html + Environment.NewLine);

								lastItemUid = itemUid;
							}

							bool original = (bool)reader["Original"];
							int skuId = (int)reader["SkuId"];
							string pn = (string)reader["Pn"];

							string parentTree = (string)reader["ParentTree"];
							parentTree = parentTree.Replace("%%%", @"\");

							string stdDesc = (string)reader["StandardDescription"];

							string part = String.Format(
								"<table class=\"Item {0}\"><tr><td><div class=\"Content2 Fourfold\"><div class=\"Mark {0}\">{1}</div><div class=\"Title\"><b>SKU ID: {2}</b><br />{4}<br /></div></div></td><td><div class=\"Content2 Fourfold\"><div class=\"Text\">{5}</div></div></td></tr></table>",
								original ? "Mkt" : "Attribute",
								original ? "BOX" : "LICENSE",
								skuId,
								pn,
								parentTree,
								stdDesc);

							Response.Write(part + Environment.NewLine);
						}
					}
				}
			}
		}
	}
}