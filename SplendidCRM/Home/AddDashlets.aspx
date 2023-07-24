<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="AddDashlets.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Home.AddDashlets" %>
<%@ Register TagPrefix="SplendidCRM" Tagname="ArrangeDashlets" Src="ArrangeDashlets.ascx" %>
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
	<script type="text/javascript">
	var ChangeDashlet = null;
	</script>

	<asp:Label ID="lblUpgradeWarning" CssClass="error" Visible="false" runat="server" />
	<asp:Button CommandName="CloseDashlets" Text='<%# L10n.Term("Home.LBL_CLOSE_DASHLETS") %>' OnCommand="Page_Command" style="margin-top: 6px;" runat="server" />
	<table border="0" cellpadding="0" cellspacing="0" width="100%" Visible="<%# !this.IsMobile %>" style="margin-top: 4px;" runat="server">
		<tr>
			<td width="50%" valign="top">
				<SplendidCRM:ArrangeDashlets ID="ctlDashletsBody" DetailView="Home.DetailView.Body" Category="My Dashlets" Runat="Server" />
			</td>
			<td style="padding-left: 10px; vertical-align: top;">
				<SplendidCRM:ArrangeDashlets ID="ctlDashletsRight" DetailView="Home.DetailView.Right" Category="My Dashlets" Runat="Server" />
			</td>
		</tr>
	</table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>

