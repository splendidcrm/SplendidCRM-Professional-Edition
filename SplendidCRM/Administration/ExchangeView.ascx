<%@ Control CodeBehind="ExchangeView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ExchangeView" %>
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
<div id="divExchangeView" visible='<%# 
(  SplendidCRM.Security.AdminUserAccess("Exchange"     , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("SystemSyncLog", "access") >= 0 
) %>' runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Exchange.LBL_EXCHANGE_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Exchange", "edit") >= 0 %>'>
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Exchange.gif" %>' AlternateText='<%# L10n.Term("Exchange.LBL_EXCHANGE_USERS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Exchange.LBL_EXCHANGE_USERS") %>' NavigateUrl="~/Administration/Exchange/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Exchange", "edit") >= 0 %>'>
				<%= L10n.Term("Exchange.LBL_EXCHANGE_USERS_DESC") %>
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Exchange", "edit") >= 0 %>'>
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Exchange.gif" %>' AlternateText='<%# L10n.Term("Exchange.LBL_EXCHANGE_SETTINGS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Exchange.LBL_EXCHANGE_SETTINGS") %>' NavigateUrl="~/Administration/Exchange/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Exchange", "edit") >= 0 %>'>
				<%= L10n.Term("Exchange.LBL_EXCHANGE_SETTINGS_DESC") %>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("SystemSyncLog", "access") >= 0 %>'>
				<asp:Image SkinID="SystemLog" AlternateText='<%# L10n.Term("Exchange.LBL_SYSTEM_SYNC_LOG") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Exchange.LBL_SYSTEM_SYNC_LOG") %>' NavigateUrl="~/Administration/SystemSyncLog/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Exchange", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Exchange.LBL_SYSTEM_SYNC_LOG_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2">
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2">
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
