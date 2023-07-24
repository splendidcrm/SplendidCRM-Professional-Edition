<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Twilio.ListView" %>
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
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Twilio" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	
	<table ID="tblMain" width="100%" border="0" cellspacing="0" cellpadding="0" class="tabDetailView" runat="server">
	</table>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearchBasic" Runat="Server" />
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Module="Twilio" Title="Twilio.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:DataGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="<%# !PrintView %>" AllowSorting="false" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NumericPages" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText="Previous" NextPageText="Next" />
		<Columns>
			<asp:BoundColumn HeaderText="Date Sent" DataField="DateSent" SortExpression="DateSent" ItemStyle-Width="15%" />
			<asp:BoundColumn HeaderText="From"      DataField="From"     SortExpression="From"     ItemStyle-Width="15%" />
			<asp:BoundColumn HeaderText="To"        DataField="To"       SortExpression="To"       ItemStyle-Width="15%" />
			<asp:BoundColumn HeaderText="Status"    DataField="Status"   SortExpression="Status"   ItemStyle-Width="15%" />
			<asp:BoundColumn HeaderText="Body"      DataField="Body"     SortExpression="Body"     ItemStyle-Width="40%" />
		</Columns>
	</asp:DataGrid>
	<br />
</div>
