<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Reminders.ascx.cs" Inherits="SplendidCRM.Themes.Six.Reminders" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<asp:UpdatePanel UpdateMode="Conditional" runat="server">
	<ContentTemplate>
		<asp:Button ID="btnREMINDER_UPDATE" CommandName="Update" OnCommand="Page_Command" CssClass="button" style="display: none" Text="Update" runat="server" />
		<asp:Label ID="lblScripts" runat="server" />
		<div id="divReminders" style="padding: 8px 8px 5px 8px;" runat="server">
			<h3>
				<asp:HyperLink ID="lnkShowSubPanel" NavigateUrl=<%# "javascript:ShowSubPanel(\'" + lnkShowSubPanel.ClientID + "\',\'" + lnkHideSubPanel.ClientID + "\',\'divReminderPopdown\');"    %> style=<%# "display:" + (CookieValue("divReminderPopdown") == "1" ? "inline" : "none") %> runat="server"><asp:Image SkinID="advanced_search" runat="server" /></asp:HyperLink>
				<asp:HyperLink ID="lnkHideSubPanel" NavigateUrl=<%# "javascript:HideSubPanel(\'" + lnkShowSubPanel.ClientID + "\',\'" + lnkHideSubPanel.ClientID + "\',\'divReminderPopdown\', 1);" %> style=<%# "display:" + (CookieValue("divReminderPopdown") != "1" ? "inline" : "none") %> runat="server"><asp:Image SkinID="basic_search"    runat="server" /></asp:HyperLink>
				&nbsp;<asp:Label Text='<%# L10n.Term("Users.LBL_REMINDER") %>' runat="server" />
			</h3>
			<div id="divReminderPopdown" style='<%# "display:" + (CookieValue("divReminderPopdown") != "1" ? "inline" : "none") %>'>
				<asp:Repeater id="ctlRepeater" runat="server">
					<HeaderTemplate />
					<ItemTemplate>
						<asp:Panel Visible='<%# DateTime.Now > Sql.ToDateTime(Eval("DATE_START")).AddSeconds(-Sql.ToInteger(Eval("REMINDER_TIME"))) %>' runat="server">
							<asp:Table SkinID="tabEditViewButtons" Visible="<%# !PrintView %>" runat="server">
								<asp:TableRow>
									<asp:TableCell>
										<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
										<asp:Button Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, Sql.ToString(Eval("ACTIVITY_TYPE")), "edit", "ASSIGNED_USER_ID") >= 0 %>' CommandName="Edit"    CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_EDIT_BUTTON_LABEL"   ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_EDIT_BUTTON_TITLE"    ) %>' Runat="server" />
										<asp:Button CommandName="Dismiss" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_DISMISS_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_DISMISS_BUTTON_TITLE" ) %>' Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabDetailView" runat="server">
								<asp:TableRow>
									<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%# L10n.Term(".moduleListSingular." + Sql.ToString(Eval("ACTIVITY_TYPE"))) + L10n.Term("Calls.LBL_COLON") %></asp:TableCell>
									<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><%# Eval("NAME") %></asp:TableCell>
									<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%# L10n.Term(Sql.ToString(Eval("ACTIVITY_TYPE")) + ".LBL_STATUS") %></asp:TableCell>
									<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><%# Eval("DIRECTION") + " " + Eval("STATUS") %></asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%# L10n.Term(Sql.ToString(Eval("ACTIVITY_TYPE")) + ".LBL_DATE_TIME") %></asp:TableCell>
									<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><%# T10n.FromServerTime(Sql.ToDateTime(Eval("DATE_START"))) %></asp:TableCell>
									<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%# Sql.ToString(Eval("ACTIVITY_TYPE")) != "Tasks" ? L10n.Term(Sql.ToString(Eval("ACTIVITY_TYPE")) + ".LBL_DURATION") : String.Empty %></asp:TableCell>
									<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><%# Sql.ToString(Eval("ACTIVITY_TYPE")) != "Tasks" ? Sql.ToString(Eval("DURATION_HOURS")) + " " + L10n.Term("Calls.LBL_HOURS_ABBREV") + " " + Sql.ToString(Eval("DURATION_MINUTES")) + " " + L10n.Term("Calls.LBL_MINSS_ABBREV") : String.Empty %></asp:TableCell>
								</asp:TableRow>
								<asp:TableRow Visible='<%# !Sql.IsEmptyString(Eval("DESCRIPTION")) %>'>
									<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%# L10n.Term(Sql.ToString(Eval("ACTIVITY_TYPE")) + ".LBL_DESCRIPTION") %></asp:TableCell>
									<asp:TableCell Width="85%" VerticalAlign="Top" CssClass="tabDetailViewDF" ColumnSpan="3"><%# Eval("DESCRIPTION") %></asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:Panel>
					</ItemTemplate>
					<FooterTemplate />
				</asp:Repeater>
			</div>
		</div>
	</ContentTemplate>
</asp:UpdatePanel>
