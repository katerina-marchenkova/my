<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Mirror.aspx.cs" Inherits="ContentAnalyzer.Mirror" %>
<%@ Import Namespace="VX.Knowledge.DataSource"%>
<%@ Import Namespace="ContentAnalyzer"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Mirror Page</title>
</head>
<body>
	<table>
		<%
			foreach (int skuId in m_skuIds)
			{
		%>
		<tr>
			<%
				foreach (DigitalContentRow row in m_rows[skuId])
				{
			%>
			<td>
				<img
					src="https://cdn.cnetcontent.com/<% Response.Write(row.ContentGuid.ToString().Substring(0, 2)); %>/<% Response.Write(row.ContentGuid.ToString().Substring(2, 2)); %>/<% Response.Write(row.ContentGuid + row.Extension); %>"
					alt="<% Response.Write(row.Name); %>" />
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
