<%@ Page language="c#" Codebehind="ApplicationVars.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.ApplicationVars" %>
<script runat="server">
/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2023 SplendidCRM Software, Inc. All rights reserved.
 *
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License 
 * Agreement, or other written agreement between you and SplendidCRM ("License"). By installing or 
 * using this file, you have unconditionally agreed to the terms and conditions of the License, 
 * including but not limited to restrictions on the number of users therein, and you may not use this 
 * file except in compliance with the License. 
 * 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and 
 * trademarks, in and to the contents of this file.  You will not link to or in any way combine the 
 * contents of this file or any derivatives with any Open Source Code in any manner that would require 
 * the contents of this file to be made available to any third party. 
 * 
 * IN NO EVENT SHALL SPLENDIDCRM BE RESPONSIBLE FOR ANY DAMAGES OF ANY KIND, INCLUDING ANY DIRECT, 
 * SPECIAL, PUNITIVE, INDIRECT, INCIDENTAL OR CONSEQUENTIAL DAMAGES.  Other limitations of liability 
 * and disclaimers set forth in the License. 
 * 
 *********************************************************************************************************************/
</script>
<!DOCTYPE HTML>
<html>
<head runat="server">
	<title>Application Variables</title>
</head>
<body>
<h1>Session</h1>
<table border="1" cellpadding="0" cellspacing="2">
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
if ( SplendidCRM.Security.IS_ADMIN && Request.ServerVariables["SERVER_NAME"] == "localhost" )
{
	foreach(string key in Session.Keys)
	{
		Response.Write("<tr>");
		Response.Write("<td>");
		Response.Write(key);
		Response.Write("</td>");
		Response.Write("<td>");
		Response.Write(Session[key].ToString());
		Response.Write("</td>");
		Response.Write("</tr>");
	}
}
%>
</table>

<h1>Application</h1>
<table border="1" cellpadding="0" cellspacing="2">
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
if ( SplendidCRM.Security.IS_ADMIN && Request.ServerVariables["SERVER_NAME"] == "localhost" )
{
	foreach(string key in Application.Keys)
	{
		Response.Write("<tr>");
		Response.Write("<td>");
		Response.Write(key);
		Response.Write("</td>");
		Response.Write("<td>");
		Response.Write(Application[key].ToString());
		Response.Write("</td>");
		Response.Write("</tr>");
	}
}
%>
</table>

<h1>Server Variables</h1>
<table border="1" cellpadding="0" cellspacing="2">
<%
if ( SplendidCRM.Security.IS_ADMIN )
{
	foreach(string key in Request.ServerVariables.Keys)
	{
		Response.Write("<tr>");
		Response.Write("<td>");
		Response.Write(key);
		Response.Write("</td>");
		Response.Write("<td>");
		Response.Write(Request.ServerVariables[key].ToString());
		Response.Write("</td>");
		Response.Write("</tr>");
	}
}
%>
</table>
</body>
</html>
