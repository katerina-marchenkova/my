<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="ContentAnalyzer._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Content Analyzer</title>
</head>
<body>
	<% Response.Write(Request.UserHostAddress); %>
	<ul>
		<li>
			<a href="Helix.aspx">Helix Page</a>
		</li>
		<li>
			<a href="Mirror.aspx">Mirror Page</a>
		</li>
		<li>
			<a href="Duplicates.aspx">Duplicates Page</a>
		</li>
		<li>
			<a href="Research.aspx">Research Page</a>
		</li>
		<li>
			<a href="Plain.aspx">Plain Page</a>
		</li>
		<li>
			<a href="BadCanvas.aspx">Bad Canvas Page</a>
		</li>
	</ul>
</body>
</html>
