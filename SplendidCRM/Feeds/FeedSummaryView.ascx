<%@ Control CodeBehind="FeedSummaryView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Feeds.FeedSummaryView" %>
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
<div id="divDetailView" runat="server">
	<p></p>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<asp:Table SkinID="tabFrame" runat="server">
		<asp:TableRow>
			<asp:TableCell HorizontalAlign="Right">
				<nobr>
				<asp:ImageButton CommandName="MoveUp"   CommandArgument='<%# gID.ToString() %>' OnCommand="Page_Command" AlternateText='<%# L10n.Term("Feeds.LBL_MOVE_UP"                ) %>' SkinID="uparrow"   Runat="server" />
				<asp:ImageButton CommandName="MoveDown" CommandArgument='<%# gID.ToString() %>' OnCommand="Page_Command" AlternateText='<%# L10n.Term("Feeds.LBL_MOVE_DOWN"              ) %>' SkinID="downarrow" Runat="server" />
				<asp:ImageButton CommandName="Delete"   CommandArgument='<%# gID.ToString() %>' OnCommand="Page_Command" AlternateText='<%# L10n.Term("Feeds.LBL_DELETE_FAV_BUTTON_LABEL") %>' SkinID="delete"    CssClass="listViewTdToolsS1" Runat="server" />
				</nobr>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Table SkinID="tabFrame" CssClass="listView" runat="server">
		<asp:TableRow Height="20">
			<asp:TableCell Width="100%" CssClass="listViewThS1">
				<asp:HyperLink Text='<%# sChannelTitle %>' NavigateUrl='<%# "view.aspx?id=" + gID.ToString() %>' CssClass="listViewThLinkS1" Runat="server" />
				-
				<asp:HyperLink Text='<%# "(" + L10n.Term("Feeds.LBL_VISIT_WEBSITE") + ")" %>' NavigateUrl='<%# sChannelLink %>' CssClass="listViewThLinkS1" Target="_new" Runat="server" />
				&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblLastBuildDate" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell BackColor="#f1f1f1" CssClass="evenListRowS1" ColumnSpan="10">
				<asp:Repeater ID="rpFeed" Runat="server">
					<ItemTemplate>
						<li><asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "title") %>' NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "link") %>' CssClass="listViewTdLinkS1" Target="_new" Runat="server" />
						&nbsp;&nbsp;<asp:Label Text='<%# DataBinder.Eval(Container.DataItem, "pubDate") %>' CssClass="rssItemDate" runat="server" />
					</ItemTemplate>
				</asp:Repeater>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
</div>

