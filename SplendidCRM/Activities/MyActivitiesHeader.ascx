<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MyActivitiesHeader.ascx.cs" Inherits="SplendidCRM.Activities.MyActivitiesHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
		<asp:TableCell Wrap="false">
			&nbsp;&nbsp;
			<%= L10n.Term("Activities.LBL_TODAY") %><asp:DropDownList ID="lstTHROUGH" DataValueField="NAME" DataTextField="DISPLAY_NAME" SelectedIndexChanged="Page_Command" AutoPostBack="true" Runat="server" />
			&nbsp;
			<asp:Label ID="txtTHROUGH" Runat="server" />
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right" Wrap="false">
			<asp:Table  BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server">
				<asp:TableRow>
					<asp:TableCell Wrap="false">
						<asp:ImageButton ID="imgRefresh" CommandName="Refresh" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Dashboard.LBL_REFRESH") %>' SkinID="refresh" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  ID="bntRefresh" CommandName="Refresh" OnCommand="Page_Command" CssClass="chartToolsLink"          Text='<%# L10n.Term("Dashboard.LBL_REFRESH") %>' Visible="false" Runat="server" />
					</asp:TableCell>
					<asp:TableCell Wrap="false" Visible="<%# ShowEdit %>">
						&nbsp;
						<span onclick="toggleDisplay('<%= DivEditName %>'); return false;">
							<asp:ImageButton ID="imgEdit" CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Dashboard.LBL_EDIT"  ) %>' SkinID="edit"    ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  ID="bntEdit" CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink"          Text='<%# L10n.Term("Dashboard.LBL_EDIT"  ) %>' Visible="false" Runat="server" />
						</span>
					</asp:TableCell>
					<asp:TableCell Wrap="false">
						&nbsp;
						<span onclick="return confirm('<%= L10n.TermJavaScript("Home.LBL_REMOVE_DASHLET_CONFIRM") %>')">
							<asp:ImageButton ID="imgRemove" CommandName="Remove" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term(".LBL_REMOVE") %>' SkinID="delete" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  ID="bntRemove" CommandName="Remove" OnCommand="Page_Command" CssClass="chartToolsLink"          Text='<%# L10n.Term(".LBL_REMOVE") %>' Visible="false" Runat="server" />
						</span>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

