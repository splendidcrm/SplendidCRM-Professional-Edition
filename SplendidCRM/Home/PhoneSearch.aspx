<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="PhoneSearch.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Home.PhoneSearch" %>
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
<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="Shortcuts" Src="~/_controls/Shortcuts.ascx" %>
	<SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="Home" Title=".LBL_SHORTCUTS" Runat="Server" />
	<asp:PlaceHolder ID="plcSubPanelLeft" Runat="server" />
</asp:Content>

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<script type="text/javascript">
	// 08/26/2010 Paul.  We need to count the visible search panels in JavaScript as we do not have an easy way to get the visible count in the code-behind. 
	var nPhoneSearchVisibleCount = 0;
	var sPhoneSearchURL = '';
	</script>
	<div id="divListView">
		<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
		<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Search" Title="Home.LBL_SEARCH_RESULTS" EnableModuleLabel="false" EnablePrint="true" EnableHelp="true" Runat="Server" />
		
		<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
			<asp:Label ID="lblWarning" CssClass="error" EnableViewState="false" Runat="server" />
		</asp:Panel>
		
		<div id="divDetailSubPanel">
			<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
		</div>
		<asp:Label ID="lblNoResults" Text='<%# L10n.Term(".LBL_EMAIL_SEARCH_NO_RESULTS") %>' CssClass="error" style="display:none" Runat="server" />
		<script type="text/javascript">
		if ( nPhoneSearchVisibleCount == 0 )
		{
			var lblNoResults = document.getElementById('<%# lblNoResults.ClientID %>');
			lblNoResults.style.display = 'inline';
		}
		// 07/08/2012 Paul.  If there is only one record found, then navigate to that record. 
		else if ( nPhoneSearchVisibleCount == 1 )
		{
			window.location.href = sPhoneSearchURL;
		}
		</script>

		<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
		<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
	</div>
</asp:Content>

