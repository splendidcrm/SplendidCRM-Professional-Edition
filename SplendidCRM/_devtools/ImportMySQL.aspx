<%@ Page language="c#" Codebehind="ImportMySQL.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.ImportMySQL" %>
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
	<title>ImportMySQL</title>
</head>
<body>
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
// 12/22/2007 Paul.  Allow an admin to import data. 
if ( SplendidCRM.Security.IS_ADMIN )
{
	%>
	<form id="frm" method="post" runat="server">
		<input id="fileUNC" type="file" MaxLength="511" runat="server" />
		<asp:Button ID="btnUpload" Text="Go" CommandName="Upload" OnCommand="Page_ItemCommand" CssClass="btn" Runat="server" />
	</form>
	<%
}
%>
</body>
</html>
