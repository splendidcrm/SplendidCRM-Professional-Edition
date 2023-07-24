<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.SystemSyncLog.ListView" %>
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
<div id="divListView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Users" Title="Administration.LBL_SYSTEM_SYNC_LOG" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="SystemSyncLog" ShowSearchTabs="false" ShowSearchViews="false" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ExportHeader" Src="~/_controls/ExportHeader.ascx" %>
	<SplendidCRM:ExportHeader ID="ctlExportHeader" Module="Administration" Title="" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" OnItemCreated="grdMain_OnItemCreated" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:BoundColumn    HeaderText=".LBL_LIST_DATE_ENTERED"              DataField="DATE_ENTERED"     SortExpression="DATE_ENTERED"     ItemStyle-VerticalAlign="Top" ItemStyle-Wrap="false" />
			<asp:BoundColumn    HeaderText="SystemSyncLog.LBL_LIST_USER_ID"          DataField="USER_ID"          SortExpression="USER_ID"          ItemStyle-VerticalAlign="Top" Visible="false" />
			<asp:BoundColumn    HeaderText="SystemSyncLog.LBL_LIST_MACHINE"          DataField="MACHINE"          SortExpression="MACHINE"          ItemStyle-VerticalAlign="Top" Visible="false" />
			<asp:BoundColumn    HeaderText="SystemSyncLog.LBL_LIST_REMOTE_URL"       DataField="REMOTE_URL"       SortExpression="REMOTE_URL"       ItemStyle-VerticalAlign="Top" Visible="false" />
			<asp:TemplateColumn HeaderText="SystemSyncLog.LBL_LIST_ERROR_TYPE"                                    SortExpression="ERROR_TYPE"       ItemStyle-VerticalAlign="Top">
				<ItemTemplate>
					<div class="<%# (Sql.ToString(Eval("ERROR_TYPE")) == "Error" ? "error" : String.Empty) %>"><%# Eval("ERROR_TYPE") %></div>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="SystemSyncLog.LBL_LIST_MESSAGE"                                       SortExpression="MESSAGE"          ItemStyle-VerticalAlign="Top" ItemStyle-Width="20%">
				<ItemTemplate>
					<div class="<%# (Sql.ToString(Eval("ERROR_TYPE")) == "Error" ? "error" : String.Empty) %>"><%# Eval("MESSAGE") %></div>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="SystemSyncLog.LBL_LIST_FILE_NAME"                                     SortExpression="FILE_NAME"        ItemStyle-VerticalAlign="Top">
				<ItemTemplate><%# Sql.ToString(Eval("FILE_NAME")).Replace("/", "/ ") %></ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn    HeaderText="SystemSyncLog.LBL_LIST_METHOD"           DataField="METHOD"           SortExpression="METHOD"           ItemStyle-VerticalAlign="Top" />
			<asp:BoundColumn    HeaderText="SystemSyncLog.LBL_LIST_LINE_NUMBER"      DataField="LINE_NUMBER"      SortExpression="LINE_NUMBER"      ItemStyle-VerticalAlign="Top" />
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
