<%@ Control CodeBehind="Teams.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Users.Teams" %>
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
function TeamMultiSelect()
{
	return ModulePopup('Teams', '<%= txtTEAM_ID.ClientID %>', null, null, true, 'PopupMultiSelect.aspx');
}
</script>
<input ID="txtTEAM_ID" type="hidden" Runat="server" />
<%-- 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. --%>
<%@ Register TagPrefix="SplendidCRM" Tagname="SubPanelButtons" Src="~/_controls/SubPanelButtons.ascx" %>
<SplendidCRM:SubPanelButtons ID="ctlDynamicButtons" Module="Teams" SubPanel="divUsersTeams" Title="Users.LBL_MY_TEAMS" Runat="Server" />

<div id="divUsersTeams" style='<%= "display:" + (CookieValue("divUsersTeams") != "1" ? "inline" : "none") %>'>
	<%-- 05/05/2016 Paul.  Use UpdatePanel for better performance. --%>
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
				<Columns>
					<asp:TemplateColumn  HeaderText="Teams.LBL_LIST_NAME" ItemStyle-Width="30%">
						<ItemTemplate>
							<%-- 12/13/2017 Paul.  Provide link to admin. --%>
							<asp:HyperLink Visible='<%# SplendidCRM.Security.AdminUserAccess("Users", "edit") >= 0 %>' Text='<%# Eval("TEAM_NAME") %>' NavigateUrl='<%# "~/Administration/Teams/view.aspx?ID=" + Sql.ToString(Eval("TEAM_ID")) %>' runat="server" />
							<asp:Label     Visible='<%# SplendidCRM.Security.AdminUserAccess("Users", "edit")  < 0 %>' Text='<%# Eval("TEAM_NAME") %>' runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn     HeaderText="Teams.LBL_LIST_DESCRIPTION" DataField="DESCRIPTION"   ItemStyle-Width="60%" />
					<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
						<ItemTemplate>
							<div visible='<%# (SplendidCRM.Security.AdminUserAccess("Users", "edit") >= 0) && Sql.ToBoolean(Eval("EXPLICIT_ASSIGN")) %>' runat="server">
								<asp:ImageButton CommandName="Teams.Edit"   CommandArgument='<%# Eval("TEAM_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" SkinID="edit_inline" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
								<asp:LinkButton  CommandName="Teams.Edit"   CommandArgument='<%# Eval("TEAM_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Runat="server"><%# L10n.Term(".LNK_EDIT") %></asp:LinkButton>
								&nbsp;
								<asp:ImageButton CommandName="Teams.Remove" CommandArgument='<%# Eval("TEAM_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
								<asp:LinkButton  CommandName="Teams.Remove" CommandArgument='<%# Eval("TEAM_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
							</div>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</SplendidCRM:SplendidGrid>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>
