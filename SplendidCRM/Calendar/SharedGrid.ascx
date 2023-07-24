<%@ Control CodeBehind="SharedGrid.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.SharedGrid" %>
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
<div id="SharedGrid">
	<%@ Register TagPrefix="SplendidCRM" Tagname="CalendarHeader" Src="CalendarHeader.ascx" %>
	<SplendidCRM:CalendarHeader ID="ctlCalendarHeader" ActiveTab="Shared" Runat="Server" />

	<p></p>
	<asp:Table SkinID="tabFrame" runat="server">
		<asp:TableRow>
			<asp:TableCell Wrap="false">
				<h3><asp:Image SkinID="h3Arrow" Runat="server" />&nbsp;<%= L10n.Term("Calendar.LBL_SHARED_CAL_TITLE") %></h3>
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right" Wrap="false">
				<span onclick="toggleDisplay('shared_cal_edit'); return false;">
					<asp:ImageButton CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Calendar.LBL_EDIT") %>' SkinID="edit" Runat="server" />&nbsp;
					<asp:LinkButton  CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink" Text='<%# L10n.Term("Calendar.LBL_EDIT") %>' Runat="server" />
				</span>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>

	<div ID="shared_cal_edit" style="DISPLAY: none">
		<asp:Table SkinID="tabFrame" HorizontalAlign="Center" runat="server">
			<asp:TableRow>
				<asp:TableHeaderCell VerticalAlign="Top" HorizontalAlign="Center" ColumnSpan="2"><%= L10n.Term("Calendar.LBL_SELECT_USERS") %></asp:TableHeaderCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table BorderWidth="0" CellPadding="1" CellSpacing="1" HorizontalAlign="Center" CssClass="chartForm" runat="server">
						<asp:TableRow>
							<asp:TableCell VerticalAlign="Top" Wrap="false"><b><%= L10n.Term("Calendar.LBL_USERS") %></b></asp:TableCell>
							<asp:TableCell VerticalAlign="Top">
								<asp:ListBox ID="lstUSERS" DataValueField="ID" DataTextField="USER_NAME" SelectionMode="Multiple" Rows="3" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell HorizontalAlign="Right" ColumnSpan="2">
								<asp:Button ID="btnSubmit" CommandName="Submit" OnCommand="Page_Command"                   CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SELECT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' runat="server" />&nbsp;
								<asp:Button ID="btnCancel" UseSubmitBehavior="false" OnClientClick="toggleDisplay('shared_cal_edit'); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<asp:Table SkinID="tabFrame" CssClass="monthBox" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabFrame" CssClass="monthHeader" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="1%" CssClass="monthHeaderPrevTd" Wrap="false">
							<asp:ImageButton CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' SkinID="calendar_previous" Runat="server" />&nbsp;
							<asp:LinkButton  CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="98%" HorizontalAlign="Center">
							<span class="monthHeaderH3"><%= dtCurrentWeek.ToLongDateString() + " - " + dtCurrentWeek.AddDays(6).ToLongDateString() %></span>
						</asp:TableCell>
						<asp:TableCell Width="1%" HorizontalAlign="Right" CssClass="monthHeaderNextTd" Wrap="false">
							<asp:LinkButton  CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' Runat="server" />
							<asp:ImageButton CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' SkinID="calendar_next" Runat="server" />&nbsp;
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="monthCalBody">
				<asp:PlaceHolder ID="plcWeekRows" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabFrame" CssClass="monthFooter" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="50%" CssClass="monthFooterPrev" Wrap="false">
							<asp:ImageButton CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' SkinID="calendar_previous" Runat="server" />&nbsp;
							<asp:LinkButton  CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="50%" HorizontalAlign="Right" CssClass="monthFooterNext" Wrap="false">
							<asp:LinkButton  CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' Runat="server" />
							<asp:ImageButton CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' SkinID="calendar_next" Runat="server" />&nbsp;
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

