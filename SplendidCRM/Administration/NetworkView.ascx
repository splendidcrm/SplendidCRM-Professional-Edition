<%@ Control CodeBehind="NetworkView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.NetworkView" %>
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
<div id="divNetworkView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_SPLENDIDCRM_NETWORK_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Wrap="false" Visible='<%# SplendidCRM.Security.IS_ADMIN %>'>
				<asp:Image SkinID="sugarupdate" AlternateText='<%# L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE_TITLE") %>' NavigateUrl="~/Administration/Updater/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.IS_ADMIN %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image SkinID="OnlineDocumentation" AlternateText='<%# L10n.Term("Administration.LBL_DOCUMENTATION_TITLE") %>' Visible='<%# !Sql.ToBoolean(Application["CONFIG.hide_training"]) %>' Runat="server" />
				&nbsp;
				<asp:LinkButton Text='<%# L10n.Term("Administration.LBL_DOCUMENTATION_TITLE") %>' Visible='<%# !Sql.ToBoolean(Application["CONFIG.hide_training"]) %>' OnClientClick=<%# "window.open('https://www.splendidcrm.com/Documentation.aspx', 'helpwin', 'status=0,resizable=1,scrollbars=1,toolbar=1,location=1'); return false;" %> CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2">
				<asp:Label Text='<%# L10n.Term("Administration.LBL_DOCUMENTATION") %>' Visible='<%# !Sql.ToBoolean(Application["CONFIG.hide_training"]) %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

