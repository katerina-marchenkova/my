<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="ContentAnalyzer._Default" %>
<%@ Import Namespace="VX.Knowledge.DataSource"%>
<%@ Import Namespace="ContentAnalyzer"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Content Analyzer</title>
</head>
<body>
	<table>
		<%
			foreach (Guid itemUid in m_items)
			{
		%>
		<tr>
			<%
				foreach (FileRow row in m_files[itemUid])
				{
			%>
			<td>
				<a
					href="http://ws.vx.cnetchannel.com/vxshare/<% Response.Write(KB.Template.Multipleimages.Id); %>/<% Response.Write(row.FileUid.ToString().Substring(0, 2)); %>/<% Response.Write(row.FileUid.ToString().Substring(2, 2)); %>/<% Response.Write(row.FileUid + row.Extension); %>"
					title="<% Response.Write(row.Name); %>">
					<img
						src="http://ws.vx.cnetchannel.com/vxshare/<% Response.Write(KB.Template.Multipleimages.Id); %>/<% Response.Write(row.FileUid.ToString().Substring(0, 2)); %>/<% Response.Write(row.FileUid.ToString().Substring(2, 2)); %>/<% Response.Write(row.FileUid + row.Extension); %>"
						alt="<% Response.Write(row.Name); %>"
						width="120"
						height="90" />
				</a>
			</td>
			<%
				}
			%>
		</tr>
		<%
			}
		%>
	</table>
</body>
</html>
