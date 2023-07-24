<%@ Control CodeBehind="EmailsView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.EmailsView" %>
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
<div id="divEmailsView" visible='<%# 
(  SplendidCRM.Security.AdminUserAccess("EmailMan"     , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("InboundEmail" , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("OutboundEmail", "access") >= 0 
) %>' runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_EMAIL_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("EmailMan", "edit") >= 0 %>'>
				<asp:Image SkinID="EmailMan" AlternateText='<%# L10n.Term("Administration.LBL_MASS_EMAIL_CONFIG_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MASS_EMAIL_CONFIG_TITLE") %>' NavigateUrl="~/Administration/EmailMan/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("EmailMan", "edit") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MASS_EMAIL_CONFIG_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("EmailMan", "list") >= 0 %>'>
				<asp:Image SkinID="EmailMan" AlternateText='<%# L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_TITLE") %>' NavigateUrl="~/Administration/EmailMan/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("EmailMan", "list") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("InboundEmail", "access") >= 0 %>'>
				<asp:Image SkinID="InboundEmail" AlternateText='<%# L10n.Term("Administration.LBL_INBOUND_EMAIL_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_INBOUND_EMAIL_TITLE") %>' NavigateUrl="~/Administration/InboundEmail/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("InboundEmail", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MAILBOX_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("EmailMan", "edit") >= 0 %>'>
				<asp:Image SkinID="Campaigns" AlternateText='<%# L10n.Term("Administration.LBL_CAMPAIGN_EMAIL_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_CAMPAIGN_EMAIL_TITLE") %>' NavigateUrl="~/Administration/EmailMan/edit.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("EmailMan", "edit") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_CAMPAIGN_EMAIL_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("OutboundEmail", "access") >= 0 %>'>
				<asp:Image ID="imgOutboundEmail" SkinID="OutboundEmail" AlternateText='<%# L10n.Term("Administration.LBL_OUTBOUND_EMAIL_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkOutboundEmail" Text='<%# L10n.Term("Administration.LBL_OUTBOUND_EMAIL_TITLE") %>' NavigateUrl="~/Administration/OutboundEmail/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("OutboundEmail", "access") >= 0 %>'>
				<asp:Label ID="lblOutboundEmail" Text='<%# L10n.Term("Administration.LBL_OUTBOUND_EMAIL_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2"></asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

