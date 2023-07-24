<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.FullTextSearch.ListView" %>
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
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="FullTextSearch" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="FullTextSearch.LBL_SEARCH_FORM_TITLE" Runat="Server" />
	<div id="divSearch">
		<asp:Table SkinID="tabSearchForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table SkinID="tabSearchView" Width="100%" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_TABLE_NAME") %>' runat="server" /></asp:TableCell>
							<asp:TableCell Width="35%" CssClass="dataField"><asp:ListBox ID="lstTABLES" Rows="4" Width="250" runat="server" /></asp:TableCell>
							<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_SEARCH_TEXT") %>' runat="server" /></asp:TableCell>
							<asp:TableCell Width="35%" CssClass="dataField"><asp:TextBox ID="txtSEARCH_TEXT" CssClass="dataField" size="55" Runat="server" /></asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					<%@ Register TagPrefix="SplendidCRM" Tagname="SearchButtons" Src="~/_controls/SearchButtons.ascx" %>
					<SplendidCRM:SearchButtons ID="ctlSearchButtons" Visible="<%# !PrintView %>" Runat="Server" />
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
		<%= Utils.RegisterEnterKeyPress(txtSEARCH_TEXT.ClientID, ctlSearchButtons.SearchClientID) %>
	</div>

	<SplendidCRM:ListHeader Title="FullTextSearch.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<SplendidCRM:DynamicImage ImageSkinID='<%# Eval("MODULE_NAME") %>' runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="FullTextSearch.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-CssClass="listViewTdLinkS1">
				<ItemTemplate>
					<asp:HyperLink Text='<%# Eval("NAME") %>' NavigateUrl='<%# "~/" + Eval("MODULE_NAME") + "/view.aspx?ID=" + Eval("ID") %>' CssClass="listViewTdLinkS1" Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

