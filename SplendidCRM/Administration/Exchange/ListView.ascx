<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Exchange.ListView" %>
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
function ChangeUser(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= txtASSIGNED_USER_ID.ClientID %>').value = sPARENT_ID;
	document.getElementById('<%= btnEnableAssigned.ClientID %>').click();
}
function UserPopup(sEXCHANGE_ALIAS, sEXCHANGE_EMAIL, sIMPERSONATED_TYPE)
{
	document.getElementById('<%= txtEXCHANGE_ALIAS.ClientID %>').value = sEXCHANGE_ALIAS;
	document.getElementById('<%= txtEXCHANGE_EMAIL.ClientID %>').value = sEXCHANGE_EMAIL;
	var sParameters = '';
	if ( sIMPERSONATED_TYPE == 'SmtpAddress' )
		sParameters = 'EMAIL1='    + escape(sEXCHANGE_EMAIL);
	else
		sParameters = 'USER_NAME=' + escape(sEXCHANGE_ALIAS);
	return window.open('../../Users/Popup.aspx?' + sParameters,'UserPopup','<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
}
</script>
<div id="divListView">
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
			<%-- 03/16/2016 Paul.  HeaderButtons must be inside UpdatePanel in order to display errors. --%>
			<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
			<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Exchange" Title="Exchange.LBL_EXCHANGE_USERS" EnableModuleLabel="false" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	
			<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
			<SplendidCRM:SearchBasic ID="ctlSearchBasic" Visible="<%# !PrintView %>" Runat="Server" />
			
			<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
			<SplendidCRM:ListHeader Module="Exchange" Title="Exchange.LBL_LIST_FORM_TITLE" Runat="Server" />
			
			<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
				<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
			</asp:Panel>
		
			<asp:HiddenField ID="txtEXCHANGE_ALIAS"   Runat="server" />
			<asp:HiddenField ID="txtEXCHANGE_EMAIL"   Runat="server" />
			<asp:HiddenField ID="txtASSIGNED_USER_ID" Runat="server" />
			<asp:Button ID="btnEnableAssigned" CommandName="Exchange.EnableAssigned" OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Exchange.LBL_ENABLE") %>' style="display: none" Runat="server" />

			<%-- 06/23/2015 Paul.  MassUpdate hover needs to be in column 0. --%>
			<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" MassUpdateHoverColumn="0" runat="server">
				<Columns>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%">
						<ItemTemplate><%# grdMain.InputCheckbox(!PrintView, ctlCheckAll.FieldName, Sql.ToGuid(Eval("USER_ID")), ctlCheckAll.SelectedItems) %></ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Users.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-Width="23%">
						<ItemTemplate>
							<asp:Label     Visible='<%# !(!Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# Eval("NAME") %>' runat="server" />
							<asp:HyperLink Visible='<%#  (!Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# Eval("NAME") %>' NavigateUrl='<%# "view.aspx?USER_ID=" + Sql.ToString(Eval("ASSIGNED_USER_ID")) %>' runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Users.LBL_LIST_USER_NAME" SortExpression="USER_NAME" ItemStyle-Width="23%">
						<ItemTemplate>
							<asp:Label     Visible='<%# !(!Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# Eval("USER_NAME") %>' runat="server" />
							<asp:HyperLink Visible='<%#  (!Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# Eval("USER_NAME") %>' NavigateUrl='<%# "view.aspx?USER_ID=" + Sql.ToString(Eval("ASSIGNED_USER_ID")) %>' runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Users.LBL_LIST_EMAIL" SortExpression="EMAIL1" ItemStyle-Width="23%">
						<ItemTemplate>
							<asp:Label     Visible='<%# !(!Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# Eval("EMAIL1") %>' runat="server" />
							<asp:HyperLink Visible='<%#  (!Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# Eval("EMAIL1") %>' NavigateUrl='<%# "view.aspx?USER_ID=" + Sql.ToString(Eval("ASSIGNED_USER_ID")) %>' runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Exchange.LBL_LIST_EXCHANGE_STATUS" SortExpression="STATUS" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:Label Visible='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# L10n.Term("Exchange.LBL_ENABLED" ) %>' runat="server" />
							<asp:Label Visible='<%#  Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Text='<%# L10n.Term("Exchange.LBL_DISABLED") %>' runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton Visible='<%#  Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Exchange.Enable"  CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Exchange.LBL_ENABLE") %>' SkinID="plus_inline" Runat="server" />
							<asp:LinkButton  Visible='<%#  Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Exchange.Enable"  CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Exchange.LBL_ENABLE") %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Runat="server" />
							
							<asp:ImageButton Visible='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Exchange.Disable" CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Exchange.LBL_DISABLE") %>' SkinID="minus_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Exchange.Disable" CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Exchange.LBL_DISABLE") %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Runat="server" />
							
							<asp:Panel Visible="false" runat="server">
								&nbsp;
								<asp:ImageButton Visible='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0 %>' CommandName="Exchange.Sync"    CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Exchange.LBL_SYNC") %>' SkinID="rightarrow_inline" Runat="server" />
								<asp:LinkButton  Visible='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0 %>' CommandName="Exchange.Sync"    CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Exchange.LBL_SYNC") %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Runat="server" />
								
								&nbsp;
								<asp:ImageButton Visible='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0 %>' CommandName="Exchange.SyncAll" CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Exchange.LBL_SYNC_ALL") %>' SkinID="rightarrow_inline" Runat="server" />
								<asp:LinkButton  Visible='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0 %>' CommandName="Exchange.SyncAll" CommandArgument='<%# Eval("USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Exchange.LBL_SYNC_ALL") %>' Font-Bold='<%# !Sql.IsEmptyGuid(Eval("ASSIGNED_USER_ID")) %>' Runat="server" />
							</asp:Panel>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</SplendidCRM:SplendidGrid>
		</ContentTemplate>
	</asp:UpdatePanel>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView %>" Runat="Server" />
	<%-- 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. --%>
	<asp:Panel ID="pnlMassUpdateSeven" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="MassUpdate" Src="MassUpdate.ascx" %>
		<SplendidCRM:MassUpdate ID="ctlMassUpdate" Runat="Server" />
	</asp:Panel>
	<br />
</div>
