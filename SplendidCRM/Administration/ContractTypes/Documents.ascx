<%@ Control CodeBehind="Documents.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ContractTypes.Documents" %>
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
function DocumentPopup()
{
	return ModulePopup('Documents', '<%= txtDOCUMENT_ID.ClientID %>', null, 'ClearDisabled=1', true, null);
}
</script>
<input ID="txtDOCUMENT_ID" type="hidden" Runat="server" />
<%-- 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. --%>
<%@ Register TagPrefix="SplendidCRM" Tagname="SubPanelButtons" Src="~/_controls/SubPanelButtons.ascx" %>
<SplendidCRM:SubPanelButtons ID="ctlDynamicButtons" Module="Documents" SubPanel="divContractTypesDocuments" Title="Documents.LBL_MODULE_NAME" Runat="Server" />

<div id="divContractTypesDocuments" style='<%= "display:" + (CookieValue("divContractTypesDocuments") != "1" ? "inline" : "none") %>'>
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn  HeaderText="Documents.LBL_LIST_DOCUMENT_NAME" SortExpression="DOCUMENT_NAME" ItemStyle-Width="40%" ItemStyle-CssClass="listViewTdLinkS1">
				<ItemTemplate>
					<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_NAME") %>' NavigateUrl='<%# "~/Documents/view.aspx?ID=" + DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' CssClass="listViewTdLinkS1" Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Documents.LBL_LIST_IS_TEMPLATE" ItemStyle-Width="20%">
				<ItemTemplate>
					<input name="chkMain" disabled="true" class="checkbox" type="checkbox" checked="<%# DataBinder.Eval(Container.DataItem, "IS_TEMPLATE") %>" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Documents.LBL_LIST_TEMPLATE_TYPE"  DataField="TEMPLATE_TYPE"  SortExpression="TEMPLATE_TYPE"  ItemStyle-Width="20%" />
			<asp:BoundColumn     HeaderText="Documents.LBL_LIST_REVISION"       DataField="REVISION"       SortExpression="REVISION"       ItemStyle-Width="20%" />
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit", Sql.ToGuid(Eval("ASSIGNED_USER_ID"))) >= 0 && !Sql.IsProcessPending(Container) %>' CommandName="Documents.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit", Sql.ToGuid(Eval("ASSIGNED_USER_ID"))) >= 0 && !Sql.IsProcessPending(Container) %>' CommandName="Documents.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.AdminUserAccess("Contracts", "edit") >= 0 %>' CommandName="Documents.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.AdminUserAccess("Contracts", "edit") >= 0 %>' CommandName="Documents.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
