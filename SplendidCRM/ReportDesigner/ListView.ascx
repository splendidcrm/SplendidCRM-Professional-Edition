<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.ReportDesigner.ListView" %>
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
function ChangeDashboard(sREPORT_ID, sDASHBOARD_ID, sCATEGORY)
{
	document.getElementById('<%= txtREPORT_ID.ClientID    %>').value = sREPORT_ID   ;
	document.getElementById('<%= txtDASHBOARD_ID.ClientID %>').value = sDASHBOARD_ID;
	document.getElementById('<%= txtCATEGORY.ClientID     %>').value = sCATEGORY    ;
	document.forms[0].submit();
}
function DashboardPopup(sREPORT_ID)
{
	return window.open('../Dashboard/Popup.aspx?REPORT_ID=' + sREPORT_ID, 'DashboardPopup', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
}
</script>
<input ID="txtREPORT_ID"    type="hidden" Runat="server" />
<input ID="txtDASHBOARD_ID" type="hidden" Runat="server" />
<input ID="txtCATEGORY"     type="hidden" Runat="server" />
<asp:UpdatePanel runat="server">
	<ContentTemplate>
		<asp:Button ID="btnAddDashlet" CommandName="Reports.AddDashlet" OnCommand="Page_Command" Text="Add Dashlet" style="display: none;" runat="server" />
	</ContentTemplate>
</asp:UpdatePanel>
<div id="divListView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Reports" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Reports" ShowSearchTabs="false" ShowDuplicateSearch="false" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="Reports.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%">
				<ItemTemplate><%# grdMain.InputCheckbox(!PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE), ctlCheckAll.FieldName, Sql.ToGuid(Eval("ID")), ctlCheckAll.SelectedItems) %></ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:HyperLink onclick=<%# "return SplendidCRM_ChangeFavorites(this, \'" + m_sMODULE + "\', \'" + Sql.ToString(Eval("ID")) + "\')" %> Visible="<%# !this.IsMobile && !this.DisableFavorites() %>" Runat="server">
						<asp:Image name='<%# "favAdd_" + Sql.ToString(Eval("ID")) %>' SkinID="favorites_add"    style='<%# "display:" + ( Sql.IsEmptyGuid(Eval("FAVORITE_RECORD_ID")) ? "inline" : "none") %>' ToolTip='<%# L10n.Term(".LBL_ADD_TO_FAVORITES"     ) %>' Runat="server" />
						<asp:Image name='<%# "favRem_" + Sql.ToString(Eval("ID")) %>' SkinID="favorites_remove" style='<%# "display:" + (!Sql.IsEmptyGuid(Eval("FAVORITE_RECORD_ID")) ? "inline" : "none") %>' ToolTip='<%# L10n.Term(".LBL_REMOVE_FROM_FAVORITES") %>' Runat="server" />
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:HyperLinkColumn HeaderText="Reports.LBL_LIST_REPORT_NAME"    DataTextField="NAME"  SortExpression="NAME" ItemStyle-Width="35%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="ID" DataNavigateUrlFormatString="edit.aspx?id={0}" Visible="false" />
			<asp:TemplateColumn  HeaderText="Reports.LBL_LIST_REPORT_NAME"                          SortExpression="NAME" ItemStyle-Width="35%" ItemStyle-CssClass="listViewTdLinkS1">
				<ItemTemplate>
					<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
					<%-- 05/06/2018 Paul.  Remove edit by default and add Edit button. --%>
					<asp:HyperLink Visible='<%# false && Sql.ToString(Eval("REPORT_TYPE")) == L10n.Term(".dom_report_types.", "tabular") && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 %>' NavigateUrl='<%# "~/ReportDesigner/edit.aspx?id=" + Eval("ID") %>' Text='<%# Eval("NAME") %>' Runat="server" />
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "view", "ASSIGNED_USER_ID") >= 0 %>' NavigateUrl='<%# "~/Reports/view.aspx?id=" + Eval("ID") %>' Text='<%# Eval("NAME") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Reports.LBL_LIST_MODULE_NAME"    DataField="MODULE_NAME"                     ItemStyle-Width="15%" />
			<asp:BoundColumn     HeaderText="Reports.LBL_LIST_REPORT_TYPE"    DataField="REPORT_TYPE"                     ItemStyle-Width="25%" />
			<asp:TemplateColumn  HeaderText="Reports.LBL_LIST_SCHEDULE_REPORT">
				<ItemTemplate>
					<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
					<%-- 12/03/2017 Paul.  Correct path to scheduler. --%>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit") >= 0 %>' NavigateUrl=<%# "~/Reports/Schedule.aspx?ReportDesigner=1&REPORT_ID=" + Eval("ID") %> Text='<%# Sql.IsEmptyString(Eval("JOB_INTERVAL")) ? L10n.Term("Reports.LNK_SCHEDULE") : Sql.ToString(Eval("JOB_INTERVAL")) %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Reports.LBL_LIST_SCHEDULE_LAST_RUN" DataField="LAST_RUN" ItemStyle-Wrap="false" ItemStyle-Width="10%" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%-- 05/06/2018 Paul.  Remove edit by default and add Edit button. --%>
					<asp:HyperLink Visible='<%# Sql.ToString(Eval("REPORT_TYPE")) == L10n.Term(".dom_report_types.", "tabular") && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0  %>' NavigateUrl='<%# "~/ReportDesigner/edit.aspx?id=" + Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' Runat="server">
						<asp:Image SkinID="edit_inline" Runat="server" />&nbsp;<%# L10n.Term(".LNK_EDIT") %>
					</asp:HyperLink>
					&nbsp;
					<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
					<asp:HyperLink Visible='<%# Sql.ToString(Eval("REPORT_TYPE")) != L10n.Term(".dom_report_types.", "Freeform") && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 %>' NavigateUrl='<%# "~/ReportDesigner/edit.aspx?DuplicateID=" + Eval("ID") %>' Text='<%# L10n.Term(".LBL_DUPLICATE_BUTTON_LABEL") %>' CssClass="listViewTdToolsS1" Runat="server" />
					&nbsp;
					<asp:LinkButton OnClientClick=<%# "DashboardPopup(\'" + Eval("ID")  + "\'); return false;" %> CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Reports.LNK_ADD_DASHLET") %>' Runat="server" />
					&nbsp;
					<asp:HyperLink Visible='<%# SplendidCRM.Security.IS_ADMIN && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "export") >= 0 %>' NavigateUrl='<%# "~/Reports/ExportRDL.aspx?id=" + Eval("ID") %>' Target="_blank" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LBL_EXPORT_RDL") %>' Runat="server">
						<asp:Image SkinID="export" Runat="server" />&nbsp;<%# L10n.Term("Reports.LBL_EXPORT_RDL") %>
					</asp:HyperLink>
					&nbsp;
					<asp:HyperLink Visible='<%# SplendidCRM.Security.IS_ADMIN && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "export") >= 0 %>' NavigateUrl='<%# "~/Reports/ExportSQL.aspx?id=" + Eval("ID") %>' Target="_blank" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LBL_EXPORT_SQL") %>' Runat="server">
						<asp:Image SkinID="export" Runat="server" />&nbsp;<%# L10n.Term("Reports.LBL_EXPORT_SQL") %>
					</asp:HyperLink>
					&nbsp;
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "view", "ASSIGNED_USER_ID") >= 0  %>' NavigateUrl='<%# "~/Reports/view.aspx?id=" + Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LNK_VIEW") %>' Runat="server">
						<asp:Image SkinID="view_inline" Runat="server" />&nbsp;<%# L10n.Term("Reports.LNK_VIEW") %>
					</asp:HyperLink>
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "delete", "ASSIGNED_USER_ID") >= 0 %>' CommandName="Reports.Delete" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "delete", "ASSIGNED_USER_ID") >= 0 %>' CommandName="Reports.Delete" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
					&nbsp;
					<asp:ImageButton Visible='<%# false && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 %>' CommandName="Reports.Publish" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LNK_PUBLISH") %>' SkinID="publish_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# false && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 %>' CommandName="Reports.Publish" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Reports.LNK_PUBLISH") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE) %>" Runat="Server" />
	<%-- 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. --%>
	<asp:Panel ID="pnlMassUpdateSeven" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="MassUpdate" Src="~/Reports/MassUpdate.ascx" %>
		<SplendidCRM:MassUpdate ID="ctlMassUpdate" Visible="<%# !PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE) %>" Runat="Server" />
	</asp:Panel>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
