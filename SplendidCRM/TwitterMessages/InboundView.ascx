<%@ Control Language="c#" AutoEventWireup="false" Codebehind="InboundView.ascx.cs" Inherits="SplendidCRM.TwitterMessages.InboundView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divDetailView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="TwitterMessages" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<script type="text/javascript">
	function OAuthTokenUpdate(oauth_token, oauth_verifier)
	{
		document.getElementById('<%= txtOAUTH_TOKEN.ClientID    %>').value = oauth_token   ;
		document.getElementById('<%= txtOAUTH_VERIFIER.ClientID %>').value = oauth_verifier;
		document.getElementById('<%= btnOAuthChanged.ClientID   %>').click();
	}
	</script>

	<asp:HiddenField ID="txtOAUTH_TOKEN"         runat="server" />
	<asp:HiddenField ID="txtOAUTH_SECRET"        runat="server" />
	<asp:HiddenField ID="txtOAUTH_VERIFIER"      runat="server" />
	<asp:HiddenField ID="txtOAUTH_ACCESS_TOKEN"  runat="server" />
	<asp:HiddenField ID="txtOAUTH_ACCESS_SECRET" runat="server" />
	<asp:Button ID="btnOAuthChanged" CommandName="OAuthToken" OnCommand="Page_Command" style="display: none" Runat="server" />
	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

