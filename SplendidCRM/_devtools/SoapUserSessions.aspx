<%@ Page Language="C#" CodeBehind="SoapUserSessions.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._code.SoapUserSessions" %>
<%@ Import Namespace="SplendidCRM" %>
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
<html>
<head id="Head1" runat="server">
	<title>SOAP User Sessions</title>
	<link href="<%= Session["themeURL"] %>style.css" type="text/css" rel="stylesheet">
</head>
<body>
<br>
<h1>SOAP User Sessions</h1>
<table border="1" cellspacing="0" cellpadding="4">
	<tr>
		<td>SESSION_ID</td>
		<td>Expiration</td>
		<td>USER_ID   </td>
		<td>USER_NAME </td>
		<td>CURRENCY  </td>
		<td>TIMEZONE  </td>
	</tr>
<%
if ( SplendidCRM.Security.IS_ADMIN )
{
	foreach ( DictionaryEntry entry in Cache )
	{
		if ( entry.Key.ToString().StartsWith("soap.session.user.") )
		{
			Guid     gSessionID   = Sql.ToGuid    (entry.Key.ToString().Substring("soap.session.user.".Length));
			DateTime dtExpiration = Sql.ToDateTime(Cache.Get("soap.user.expiration."  + gSessionID.ToString()));
			Guid     gUSER_ID     = Sql.ToGuid    (entry.Value);
			string   user_name    = Sql.ToString  (Cache.Get("soap.user.username."    + gUSER_ID.ToString()));
			string   sCurrencyID  = Sql.ToString  (Cache.Get("soap.user.currency."    + gUSER_ID.ToString()));
			string   sTimeZone    = Sql.ToString  (Cache.Get("soap.user.timezone."    + gUSER_ID.ToString()));
			%>
	<tr>
		<td><%= gSessionID   %>&nbsp;</td>
		<td><%= dtExpiration %>&nbsp;</td>
		<td><%= gUSER_ID     %>&nbsp;</td>
		<td><%= user_name    %>&nbsp;</td>
		<td><%= sCurrencyID  %>&nbsp;</td>
		<td><%= sTimeZone    %>&nbsp;</td>
	</tr>
			<%
		}
	}
}
%>
</table>
</body>
</html>
