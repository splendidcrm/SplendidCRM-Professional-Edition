<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Home.Default" %>
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
<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="Shortcuts" Src="~/_controls/Shortcuts.ascx" %>
	<SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="Home" Title=".LBL_SHORTCUTS" Runat="Server" />
	<asp:PlaceHolder ID="plcSubPanelLeft" Runat="server" />
	<%= Application["CONFIG.home_left_banner"] %>
</asp:Content>

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<asp:Table Width="100%" BorderStyle="None" BorderWidth="0" CellPadding="2" CellSpacing="0" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="90%">
				<asp:Button ID="btnAddDashlets" CommandName="AddDashlets" Text='<%# L10n.Term("Home.LBL_ADD_DASHLETS") %>' OnCommand="Page_Command" Visible='<%# !this.IsMobile && !Sql.ToBoolean(Application["CONFIG.disable_add_dashlets"]) %>' runat="server" />
				&nbsp;
				<asp:PlaceHolder Visible='<%# !Sql.ToBoolean(Application["CONFIG.hide_upgrade_warning"]) %>' runat="server">
					<asp:Label ID="lblUpgradeWarning" CssClass="error" Visible="false" runat="server" />
				</asp:PlaceHolder>
			</asp:TableCell>
			<asp:TableCell Width="10%" HorizontalAlign="Right" Wrap="false">
				<script type="text/javascript">
				function PopupHelp()
				{
					var url = document.getElementById('<%= lnkHelpText.ClientID %>').href;
					window.open(url,'helpwin','width=600,height=600,status=0,resizable=1,scrollbars=1,toolbar=0,location=1');
				}
				</script>
				<asp:PlaceHolder Visible='<%# !Sql.ToBoolean(Application["CONFIG.hide_help"]) %>' runat="server">
					<asp:HyperLink ID="lnkHelpImage" onclick="PopupHelp(); return false;" CssClass="utilsLink" Target="_blank" Runat="server">
						<asp:Image AlternateText='<%# L10n.Term(".LNK_HELP") %>' SkinID="help" Runat="server" />
					</asp:HyperLink>
					<asp:HyperLink ID="lnkHelpText" onclick="PopupHelp(); return false;" CssClass="utilsLink" Target="_blank" Runat="server"><%# L10n.Term(".LNK_HELP") %></asp:HyperLink>
				</asp:PlaceHolder>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<table border="0" cellpadding="0" cellspacing="0" width="100%" Visible="<%# !this.IsMobile %>" runat="server">
		<tr>
			<td width="60%" valign="top">
				<asp:PlaceHolder ID="plcSubPanelBody" Runat="server" />
			</td>
			<td style="padding-left: 10px; vertical-align: top;">
				<asp:PlaceHolder ID="plcSubPanelRight" Runat="server" />
				<%= Application["CONFIG.home_right_banner"] %>
			</td>
		</tr>
	</table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>

