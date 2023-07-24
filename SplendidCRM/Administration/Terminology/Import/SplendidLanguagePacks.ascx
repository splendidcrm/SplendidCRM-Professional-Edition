<%@ Control CodeBehind="SplendidLanguagePacks.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Terminology.Import.SplendidLanguagePacks" %>
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
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divImportSplendidLanguagePacks" Title="SplendidCRM Language Packs" Runat="Server" />

<div id="divImportSplendidLanguagePacks" style='<%= "display:" + (CookieValue("divImportSplendidLanguagePacks") != "1" ? "inline" : "none") %>'>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn  HeaderText="Name"                                SortExpression="Name"        ItemStyle-Width="25%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton CommandName="LanguagePack.Import" CommandArgument='<%# Eval("URL") %>' CausesValidation="false" OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Import.LBL_MODULE_NAME") %>' SkinID="Import" Runat="server" />
					<asp:LinkButton  CommandName="LanguagePack.Import" CommandArgument='<%# Eval("URL") %>' CausesValidation="false" OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# Eval("Name") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Date"        DataField="Date"        SortExpression="Date"        ItemStyle-Width="15%" ItemStyle-Wrap="false" />
			<asp:BoundColumn     HeaderText="Description" DataField="Description" SortExpression="Description" ItemStyle-Width="60%" ItemStyle-Wrap="true" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%">
				<ItemTemplate>
					<asp:HyperLink  NavigateUrl='<%# Eval("URL") %>' SkinID="Backup" runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

