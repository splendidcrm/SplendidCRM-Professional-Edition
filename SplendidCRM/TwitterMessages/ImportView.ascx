<%@ Control CodeBehind="ImportView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.TwitterMessages.ImportView" %>
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
<div id="divImportView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="TwitterMessages" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

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

	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="PayTrace.LBL_SEARCH_FORM_TITLE" Runat="Server" />
	<div id="divImportSearch">
		<asp:Table SkinID="tabSearchForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel" Wrap="false" Width="15%"><%= L10n.Term("TwitterMessages.LBL_SEARCH_TEXT") %></asp:TableCell>
							<asp:TableCell CssClass="dataField" Wrap="false" Width="85%"><asp:TextBox ID="txtSEARCH_TEXT" Width="400" runat="server" /></asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
					<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" ShowRequired="true" Runat="Server" />
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	<%= Utils.RegisterEnterKeyPress(txtSEARCH_TEXT.ClientID, ctlDynamicButtons.ButtonClientID("Search")) %>

	<SplendidCRM:ListHeader Module="TwitterMessages" Title="TwitterMessages.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%# grdMain.InputCheckbox(!PrintView && !IsMobile, ctlCheckAll.FieldName, Sql.ToString(Eval("TWITTER_ID")), ctlCheckAll.SelectedItems) %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && !IsMobile %>" FieldName="TWITTER_ID" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

