<%@ Control CodeBehind="VoIPView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.VoIPView" %>
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
<div id="divVoipView" visible='<%# 
(  SplendidCRM.Security.AdminUserAccess("Asterisk", "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Avaya"   , "access") >= 0 
) %>' runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_VOIP_SERVICES_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Asterisk", "edit") >= 0 %>'>
				<asp:Image ID="imgAsterisk" ImageUrl='<%# Session["themeURL"] + "images/Asterisk.gif" %>' AlternateText='<%# L10n.Term("Asterisk.LBL_ASTERISK_SETTINGS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkAsterisk" Text='<%# L10n.Term("Asterisk.LBL_ASTERISK_SETTINGS") %>' NavigateUrl="~/Administration/Asterisk/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Asterisk", "edit") >= 0 %>'>
				<asp:Label ID="lblAsterisk" Text='<%# L10n.Term("Asterisk.LBL_ASTERISK_SETTINGS_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Asterisk", "view") >= 0 %>'>
				<asp:Image ID="imgAsteriskCallDataRecords" ImageUrl='<%# Session["themeURL"] + "images/Asterisk.gif" %>' AlternateText='<%# L10n.Term("Asterisk.LBL_CALL_DATA_RECORDS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkAsteriskCallDataRecords" Text='<%# L10n.Term("Asterisk.LBL_CALL_DATA_RECORDS") %>' NavigateUrl="~/Administration/Asterisk/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Asterisk", "view") >= 0 %>'>
				<asp:Label ID="lblAsteriskCallDataRecords" Text='<%# L10n.Term("Asterisk.LBL_CALL_DATA_RECORDS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Avaya", "edit") >= 0 %>'>
				<asp:Image ID="imgAvaya" ImageUrl='<%# Session["themeURL"] + "images/Avaya.gif" %>' AlternateText='<%# L10n.Term("Avaya.LBL_AVAYA_SETTINGS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkAvaya" Text='<%# L10n.Term("Avaya.LBL_AVAYA_SETTINGS") %>' NavigateUrl="~/Administration/Avaya/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Avaya", "edit") >= 0 %>'>
				<asp:Label ID="lblAvaya" Text='<%# L10n.Term("Avaya.LBL_AVAYA_SETTINGS_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Avaya", "view") >= 0 %>'>
				<asp:Image ID="imgAvayaCallDataRecords" ImageUrl='<%# Session["themeURL"] + "images/Avaya.gif" %>' AlternateText='<%# L10n.Term("Avaya.LBL_CALL_DATA_RECORDS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkAvayaCallDataRecords" Text='<%# L10n.Term("Avaya.LBL_CALL_DATA_RECORDS") %>' NavigateUrl="~/Administration/Avaya/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Avaya", "view") >= 0 %>'>
				<asp:Label ID="lblAvayaCallDataRecords" Text='<%# L10n.Term("Avaya.LBL_CALL_DATA_RECORDS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

