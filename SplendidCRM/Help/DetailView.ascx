<%@ Control CodeBehind="DetailView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Help.DetailView" %>
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
	<script type="text/javascript">
	function createBookmarkLink()
	{
		var sTitle = '<%= SplendidCRM.Sql.EscapeJavaScript(sPageTitle) %>';
		var sURL = window.location.href;
		if ( document.all )
			window.external.AddFavorite(sURL, sTitle);
		else if ( window.sidebar )
			window.sidebar.addPanel(sTitle, sURL, '');
	}
	function Cancel()
	{
		window.close();
	}

	var sApplicationSiteURL = location.protocol + '//' + location.host + '<%= Application["rootURL"] %>';
	</script>
	<!-- 01/29/2011 Paul.  Allow the help javascript to be customized. -->
	<%= Application["CONFIG.help_scripts"] %>
	<asp:Table width="100%" border="0" cellspacing="2" cellpadding="0" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="25%">
				<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
				<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" Runat="Server" />
			</asp:TableCell>
			<asp:TableCell Width="50%" HorizontalAlign="Center">
				<asp:HyperLink ID="lnkTOPICS" NavigateUrl="topics.aspx" Text='<%# L10n.Term("Help.LBL_TOPICS") %>' runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkTEST"   NavigateUrl="~/_devtools/TestHelp.aspx" Text='<%# L10n.Term("Help.LBL_TEST") %>' Visible='<%# Security.IS_ADMIN %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="25%" HorizontalAlign="Right">
				<asp:HyperLink ID="lnkPRINT" NavigateUrl="javascript:window.print();" Text='<%# L10n.Term("Help.LBL_HELP_PRINT") %>' runat="server" />
				-
				<asp:HyperLink ID="lnkEMAIL" NavigateUrl="#" Text='<%# L10n.Term("Help.LBL_HELP_EMAIL") %>' runat="server" />
				-
				<asp:HyperLink ID="lnkBOOKMARK" NavigateUrl="#" onmousedown="createBookmarkLink()" Text='<%# L10n.Term("Help.LBL_HELP_BOOKMARK") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Literal ID="lblDISPLAY_TEXT" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<script type="text/javascript">
		document.getElementById('<%= new SplendidCRM.DynamicControl(this, "lnkEMAIL").ClientID %>').href = 'mailto:?subject=<%= Server.HtmlEncode(sPageTitle) %>&body=' + escape(window.location.href);
	</script>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

