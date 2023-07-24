<%@ Control CodeBehind="Contacts.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Opportunities.Contacts" %>
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
<script type="text/javascript">
function ContactPopup()
{
	// 12/27/2012 Paul.  The contacts popup should filter on the account. 
	var sACCOUNT_ID   = '<%= Page.Items["ACCOUNT_ID"  ] %>';
	// 07/15/2014 Paul.  Need to escape the name in case it includes quote characters. 
	var sACCOUNT_NAME = '<%= Sql.EscapeJavaScript(Sql.ToString(Page.Items["ACCOUNT_NAME"])) %>';
	return ModulePopup('Contacts', '<%= txtCONTACT_ID.ClientID %>', null, 'ACCOUNT_NAME=' + escape(sACCOUNT_NAME) + '&ACCOUNT_ID=' + escape(sACCOUNT_ID) + '&ClearDisabled=1', true, null);
}
</script>
<input ID="txtCONTACT_ID" type="hidden" Runat="server" />
<%-- 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. --%>
<%@ Register TagPrefix="SplendidCRM" Tagname="SubPanelButtons" Src="~/_controls/SubPanelButtons.ascx" %>
<SplendidCRM:SubPanelButtons ID="ctlDynamicButtons" Module="Contacts" SubPanel="divOpportunitiesContacts" Title="Contacts.LBL_MODULE_NAME" Runat="Server" />

<div id="divOpportunitiesContacts" style='<%= "display:" + (CookieValue("divOpportunitiesContacts") != "1" ? "inline" : "none") %>'>
	<asp:Panel ID="pnlNewRecordInline" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="~/Contacts/NewRecord.ascx" %>
		<SplendidCRM:NewRecord ID="ctlNewRecord" Width="100%" EditView="EditView.Inline" ShowCancel="true" ShowHeader="false" ShowFullForm="true" ShowTopButtons="true" Runat="Server" />
	</asp:Panel>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Contacts" SearchMode="SearchSubpanel" IsSubpanelSearch="true" ShowSearchTabs="false" ShowDuplicateSearch="false" ShowSearchViews="false" Visible="false" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%-- 10/08/2017 Paul.  Editing relationships is not allowed in ArchiveView.  --%>
					<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
					<asp:ImageButton Visible='<%# !bEditView && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 && !Sql.IsProcessPending(Container) && !ArchiveViewExists() %>' CommandName="Contacts.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CONTACT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# !bEditView && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 && !Sql.IsProcessPending(Container) && !ArchiveViewExists() %>' CommandName="Contacts.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CONTACT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript("Opportunities.NTC_REMOVE_OPP_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Opportunities", "remove", "OPPORTUNITY_ASSIGNED_USER_ID") >= 0 && !ArchiveViewExists() %>' CommandName="Contacts.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CONTACT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Opportunities", "remove", "OPPORTUNITY_ASSIGNED_USER_ID") >= 0 && !ArchiveViewExists() %>' CommandName="Contacts.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CONTACT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

