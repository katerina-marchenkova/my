<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Plain.aspx.cs" Inherits="ContentAnalyzer.Plain" %>
<%@ Import Namespace="ContentAnalyzer"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Plain Page</title>
	<style type="text/css">
		body { font-family: Cambria, Tahoma, Arial, Helvetica, Sans-Serif; }
		h1 { font-size: 160%; color: #365f91; border-bottom: solid 1px #365f91; }
		table { font-family: Calibri, Verdana, Arial, Helvetica, Sans-Serif; }
		td { white-space: nowrap; }
		img { border: solid 1px #666666; }
		.title { color: #267f00; font-weight: bold; }
		.important { color: #ff0000; font-weight: bold; }
		.information { color: #0066cc; font-size: 80%; }
		.note { color: #666666; font-size: 80%; }
		.micro { color: #666666; font-size: 50%; }
	</style>
</head>
<body>
	<h1>Browse SKUs</h1>
	<table>
		<%
			foreach (int skuId in m_skuIds)
			{
		%>
		<tr style="height: 150px;">
			<td style="padding-right: 20px;">
				<span class="title">SKU <% Response.Write(skuId); %></span><br />
				<%
					if (m_infos.ContainsKey(skuId))
					{
						SkuInfoRow info = m_infos[skuId];
				%>
				<span class="important"><% Response.Write(info.ManufacturerName); %></span><br />
				<span class="information"><% Response.Write(info.ParentTree); %></span>
				<%
					}
					else
					{
				%>
				<span class="note">NO PRODUCT INFO</span>
				<%
					}
				%>
			</td>
			<%
				if (m_standardImages.ContainsKey(skuId))
				{
			%>
				<%
					foreach (DigitalContentRow row in m_standardImages[skuId])
					{
				%>
					<td>
						<span class="micro">STANDARD IMAGE</span><br />
						<img src="<% Response.Write(row.Url); %>" />
					</td>
				<%
					}
				%>
			<%
				}
				else
				{
			%>
			<td>
				<span class="note">NO STANDARD IMAGE</span>
			</td>
			<%
				}
			%>
			<%
				if (m_multipleImages.ContainsKey(skuId))
				{
			%>
				<%
					foreach (DigitalContentRow row in m_multipleImages[skuId])
					{
				%>
					<td>
						<span class="micro">MULTIPLE IMAGE</span><br />
						<img src="<% Response.Write(row.Url); %>" />
					</td>
				<%
					}
				%>
			<%
				}
				else
				{
			%>
			<td>
				<span class="note">NO MULTIPLE IMAGES</span>
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
