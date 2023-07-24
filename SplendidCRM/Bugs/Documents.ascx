<%@ Control CodeBehind="Documents.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Bugs.Documents" %>
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
<SplendidCRM:SubPanelButtons ID="ctlDynamicButtons" Module="Documents" SubPanel="divBugsDocuments" Title="Documents.LBL_MODULE_NAME" Runat="Server" />

<div id="divBugsDocuments" style='<%= "display:" + (CookieValue("divBugsDocuments") != "1" ? "inline" : "none") %>'>
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%-- 10/08/2017 Paul.  Editing relationships is not allowed in ArchiveView.  --%>
					<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
					<div style="DISPLAY: <%# Sql.ToString(DataBinder.Eval(Container.DataItem, "REVISION")) != Sql.ToString(DataBinder.Eval(Container.DataItem, "SELECTED_REVISION")) ? "inline" : "none" %>">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Bugs", "edit", "BUG_ASSIGNED_USER_ID") >= 0 && !ArchiveViewExists() %>' CommandName="Documents.GetLatest" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_GET_LATEST") %>' SkinID="getLatestDocument" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Bugs", "edit", "BUG_ASSIGNED_USER_ID") >= 0 && !ArchiveViewExists() %>' CommandName="Documents.GetLatest" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_GET_LATEST") %>' Runat="server" />
					</div>
					<asp:ImageButton Visible='<%# !bEditView && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 && !Sql.IsProcessPending(Container) && !ArchiveViewExists() %>' CommandName="Documents.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# !bEditView && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 && !Sql.IsProcessPending(Container) && !ArchiveViewExists() %>' CommandName="Documents.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Bugs", "remove", "BUG_ASSIGNED_USER_ID") >= 0 && !ArchiveViewExists() %>' CommandName="Documents.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Bugs", "remove", "BUG_ASSIGNED_USER_ID") >= 0 && !ArchiveViewExists() %>' CommandName="Documents.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
