<%@ Control CodeBehind="DetailView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Exchange.DetailView" %>
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
<div id="divSyncView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Exchange" Title="Exchange.LBL_EXCHANGE_SYNC" EnableModuleLabel="false" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:BoundColumn    HeaderText="Exchange.LBL_LIST_WELL_KNOWN_FOLDER" DataField="WELL_KNOWN_FOLDER" ItemStyle-Width="10%" />
			<asp:TemplateColumn HeaderText="Exchange.LBL_LIST_MODULE_NAME" ItemStyle-Width="30%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:Label Text='<%# Sql.IsEmptyString(Eval("MODULE_NAME")) ? "SplendidCRM Root" : Sql.ToString(Eval("MODULE_NAME")) %>' runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn    HeaderText="Exchange.LBL_LIST_PARENT_NAME" DataField="PARENT_NAME" ItemStyle-Width="40%" />
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton CommandName="Exchange.SyncFolder" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Exchange.LBL_SYNC") %>' SkinID="rightarrow_inline" Runat="server" />
					<asp:LinkButton  CommandName="Exchange.SyncFolder" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Exchange.LBL_SYNC") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<br />
</div>
