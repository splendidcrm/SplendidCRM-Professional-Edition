<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ChartHeader.ascx.cs" Inherits="SplendidCRM._controls.ChartHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="h3Row" runat="server">
	<asp:TableRow>
		<asp:TableCell Wrap="false">
			<h3><asp:Image SkinID="h3Arrow" Runat="server" />&nbsp;<asp:Label Text='<%# L10n.Term(sTitle) %>' runat="server" /></h3>
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right" Wrap="false">
			<asp:ImageButton CommandName="Refresh" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Dashboard.LBL_REFRESH") %>' SkinID="refresh" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
			<asp:LinkButton  CommandName="Refresh" OnCommand="Page_Command" CssClass="chartToolsLink"          Text='<%# L10n.Term("Dashboard.LBL_REFRESH") %>' Runat="server" />&nbsp;
			<span onclick="toggleDisplay('<%= DivEditName %>'); return false;">
				<asp:ImageButton CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Dashboard.LBL_EDIT"  ) %>' SkinID="edit"    ImageAlign="AbsMiddle" Runat="server" />&nbsp;
				<asp:LinkButton  CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink"          Text='<%# L10n.Term("Dashboard.LBL_EDIT"  ) %>' Runat="server" />
			</span>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

