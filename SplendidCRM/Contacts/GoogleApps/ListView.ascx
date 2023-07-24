<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Contacts.GoogleApps.ListView" %>
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
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
	function OAuthTokenUpdate(access_token, oauth_verifier, realmId, refresh_token, expires_in)
	{
		//var expiresDate = new Date();
		//expiresDate = new Date(expiresDate.valueOf() + expires_in * 1000);
		//var sStatus = 'access_token = '   + access_token   + '\r\n'
		//            + 'oauth_verifier = ' + oauth_verifier + '\r\n'
		//            + 'realmId = '        + realmId        + '\r\n'
		//            + 'refresh_token = '  + refresh_token  + '\r\n'
		//            + 'expires_in = '     + expires_in     + '\r\n';
		//alert(sStatus);
		document.getElementById('<%= OAUTH_ACCESS_TOKEN .ClientID %>').value = access_token  ;
		document.getElementById('<%= OAUTH_REFRESH_TOKEN.ClientID %>').value = refresh_token ;
		document.getElementById('<%= OAUTH_EXPIRES_IN   .ClientID %>').value = expires_in    ;
		var btnGoogleAuthorized = document.getElementById('<%= btnGoogleAuthorized.ClientID %>');
		btnGoogleAuthorized.click();
	}

	function OAuthTokenError(error)
	{
		var lblError = document.getElementById('<%= lblError.ClientID %>');
		lblError.innerHTML = '<%= L10n.Term("Google.LBL_TEST_FAILED") %>';
	}

	// https://console.developers.google.com
	// https://developers.google.com/oauthplayground/
	// https://developers.google.com/identity/protocols/OAuth2
	// https://developers.google.com/identity/protocols/OAuth2WebServer
	// https://developers.google.com/identity/protocols/OAuth2InstalledApp
	// https://developers.google.com/identity/protocols/OpenIDConnect#createxsrftoken
	function GoogleAppsAuthorize()
	{
		var client_id = '<%= Application["CONFIG.GoogleApps.ClientID"] %>';
		window.open('<%= Application["rootURL"] %>GoogleOAuth/default.aspx?client_id=' + client_id, 'GooglePopup', 'width=830,height=830,status=1,toolbar=0,location=0,resizable=1');
		return false;
	}
</script>
</SplendidCRM:InlineScript>
<div style="display: none;">
	<asp:Label  Text='<%# L10n.Term("Google.LBL_OAUTH_ACCESS_TOKEN" ) %>' runat="server" /> <asp:TextBox ID="OAUTH_ACCESS_TOKEN"  runat="server" /><br />
	<asp:Label  Text='<%# L10n.Term("Google.LBL_OAUTH_REFRESH_TOKEN") %>' runat="server" /> <asp:TextBox ID="OAUTH_REFRESH_TOKEN" runat="server" /><br />
	<asp:Label  Text='<%# L10n.Term("Google.LBL_OAUTH_EXPIRES_IN"   ) %>' runat="server" /> <asp:TextBox ID="OAUTH_EXPIRES_IN"    runat="server" /><br />
	<asp:Button ID="btnGoogleAuthorized"       CommandName="GoogleApps.Authorize"    OnCommand="Page_Command" Text="Authorized" runat="server" />
	<asp:Button ID="btnGoogleAppsRefreshToken" CommandName="GoogleApps.RefreshToken" OnCommand="Page_Command" style="margin-top: 4px;"  CssClass="button" Text='<%# "  " + L10n.Term("Google.LBL_REFRESH_TOKEN_LABEL") + "  " %>' Runat="server" />
</div>
<div id="divListView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Contacts" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Google" SearchMode="Contacts.SearchSubpanel" ShowSearchTabs="false" ShowDuplicateSearch="false" ShowSearchViews="false" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ExportHeader" Src="~/_controls/ExportHeader.ascx" %>
	<SplendidCRM:ExportHeader ID="ctlExportHeader" Module="Contacts" Title="Contacts.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<asp:HiddenField ID="LAYOUT_LIST_VIEW" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%">
				<ItemTemplate><%# grdMain.InputCheckbox(!PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE), ctlCheckAll.FieldName, Sql.ToString(Eval("ID")), ctlCheckAll.SelectedItems) %></ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:HyperLink NavigateUrl='<%# "edit.aspx?GID=" + Eval("ID") %>' ToolTip='<%# L10n.Term(".LNK_EDIT") %>' Runat="server">
						<asp:Image SkinID="edit_inline" Runat="server" />
					</asp:HyperLink>
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton CommandName="Delete" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE) %>" Runat="Server" />
	<%-- 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. --%>
	<asp:Panel ID="pnlMassUpdateSeven" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="MassUpdate" Src="MassUpdate.ascx" %>
		<SplendidCRM:MassUpdate ID="ctlMassUpdate" Visible="<%# !PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE) %>" Runat="Server" />
	</asp:Panel>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

