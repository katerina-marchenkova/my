<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Research.aspx.cs" Inherits="ContentAnalyzer.Research" %>
<%@ Import Namespace="VX.Knowledge.DataSource"%>
<%@ Import Namespace="ContentAnalyzer"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Research Page</title>
</head>
<body>
	<table>
		<%
			foreach (PairContentRow row in m_rows)
			{
		%>
		<tr>
			<td>
				<b><% Response.Write(row.MagicNumber); %></b>
			</td>
			<td>
				<img
					src="https://cdn.cnetcontent.com/<% Response.Write(row.ContentGuidA.ToString().Substring(0, 2)); %>/<% Response.Write(row.ContentGuidA.ToString().Substring(2, 2)); %>/<% Response.Write(row.ContentGuidA + ".jpg"); %>" />
			</td>
			<td>
				<img
					src="https://cdn.cnetcontent.com/<% Response.Write(row.ContentGuidB.ToString().Substring(0, 2)); %>/<% Response.Write(row.ContentGuidB.ToString().Substring(2, 2)); %>/<% Response.Write(row.ContentGuidB + ".jpg"); %>" />
			</td>
		</tr>
		<%
			}
		%>
	</table>
</body>
</html>
