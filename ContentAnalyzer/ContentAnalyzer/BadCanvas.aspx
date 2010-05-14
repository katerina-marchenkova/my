<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Plain.aspx.cs" Inherits="ContentAnalyzer.Plain" %>
<%@ Import Namespace="ContentAnalyzer"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Bad Canvas Page</title>
	<style type="text/css">
		body { font-family: Cambria, Tahoma, Arial, Helvetica, Sans-Serif; }
		h1 { font-size: 160%; color: #365f91; border-bottom: solid 1px #365f91; }
		table { font-family: Calibri, Verdana, Arial, Helvetica, Sans-Serif; }
		td { white-space: nowrap; }
		img { border: solid 1px #666666; }
		.link { color: #0066cc; }
	</style>
</head>
<body>
	<h1>Bad Canvas Top 1000</h1>
	<table>
		<%
			foreach (MultipleImageRow row in WorkDb.GetMultipleImagesWithBadCanvas())
			{
		%>
		<tr>
			<td>
				<img src="<% Response.Write(row.Url); %>" />
			</td>
			<td>
				Vortex Browser URL:<br />
				<span class="link"><% Response.Write(row.VXStorageLink); %></span>
			</td>
		</tr>
		<%
			}
		%>
	</table>
</body>
</html>
