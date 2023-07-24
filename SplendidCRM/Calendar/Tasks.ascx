<%@ Control CodeBehind="Tasks.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.Tasks" %>
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
<SplendidCRM:ListHeader SubPanel="divCalendarTasks" Title="Tasks.LBL_LIST_FORM_TITLE" Runat="Server" />

<div id="divCalendarTasks" style='<%= "display:" + (CookieValue("divCalendarTasks") != "1" ? "inline" : "none") %>'>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:HyperLinkColumn HeaderText="Tasks.LBL_LIST_STATUS"     DataTextField="STATUS"       SortExpression="STATUS"       ItemStyle-Width="30%" ItemStyle-Wrap="false" />
			<asp:HyperLinkColumn HeaderText="Tasks.LBL_LIST_SUBJECT"    DataTextField="NAME"         SortExpression="NAME"         ItemStyle-Width="60%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="ID"         DataNavigateUrlFormatString="~/Tasks/view.aspx?id={0}" />
			<asp:TemplateColumn  HeaderText="Tasks.LBL_LIST_DUE_DATE"                                SortExpression="DATE_DUE"     ItemStyle-Width="10%">
				<ItemTemplate>
					<%# Sql.ToDateString(T10n.FromServerTime(DataBinder.Eval(Container.DataItem, "DATE_DUE"))) %>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

