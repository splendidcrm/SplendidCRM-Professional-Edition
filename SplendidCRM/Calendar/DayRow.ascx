<%@ Control CodeBehind="DayRow.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.DayRow" %>
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
				<tr>
					<td width="1%" class="dailyCalBodyTime" nowrap>
						<span onclick="<%= !PrintView ? "toggleDisplay('" + dtDATE_START.Hour + "_appt'); return false;" : String.Empty %>">
							<a href="#" class="weekCalBodyDayLink"><%= Sql.ToTimeString(dtDATE_START) %></a>
						</span>
					</td>
					<td width="99%" class="dailyCalBodyItems">
						<div style="display:none;" id="<%= dtDATE_START.Hour %>_appt">
							<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="1" runat="server">
								<asp:TableRow>
									<asp:TableCell ColumnSpan="2">
										<asp:RadioButton ID="radScheduleCall"    GroupName="grpAppointment" class="radio" Checked="true" Runat="server" /><asp:Label CssClass="dataLabel" Text='<%# L10n.Term("Calendar.LNK_NEW_CALL"   ) %>' runat="server" />
										&nbsp; &nbsp;
										<asp:RadioButton ID="radScheduleMeeting" GroupName="grpAppointment" class="radio"                Runat="server" /><asp:Label CssClass="dataLabel" Text='<%# L10n.Term("Calendar.LNK_NEW_MEETING") %>' runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell ColumnSpan="2"><asp:Label CssClass="dataLabel" Text='<%# L10n.Term("Meetings.LBL_SUBJECT") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell VerticalAlign="top"><asp:TextBox ID="txtNAME" size="30" MaxLength="255" Runat="server" /></asp:TableCell>
									<asp:TableCell VerticalAlign="top"><asp:Button ID="btnSave" CommandName="Save" OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term(".LBL_SAVE_BUTTON_LABEL") + " " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /></asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<br />
						</div>
						
						<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
						<asp:DataList ID="lstMain" Width="100%"  BorderWidth="0" CellPadding="0" CellSpacing="0" ShowBorder="False"
							RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="0" Runat="server">
							<ItemTemplate>
								<div style="margin-top: 1px;">
								<asp:Table SkinID="tabFrame" CssClass="monthCalBodyDayItem" runat="server">
									<asp:TableRow>
										<asp:TableCell CssClass="monthCalBodyDayIconTd">
											<SplendidCRM:DynamicImage ImageSkinID='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") %>' AlternateText='<%# L10n.Term(Sql.ToString(DataBinder.Eval(Container.DataItem, "STATUS"))) + ": " + DataBinder.Eval(Container.DataItem, "NAME") %>' Runat="server" />
										</asp:TableCell>
										<asp:TableCell CssClass="monthCalBodyDayItemTd" width="100%">
											<asp:HyperLink Text='<%# L10n.Term(Sql.ToString(DataBinder.Eval(Container.DataItem, "STATUS"))) + ": " + DataBinder.Eval(Container.DataItem, "NAME") %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/view.aspx?id=" + DataBinder.Eval(Container.DataItem, "ID") %>' CssClass="monthCalBodyDayItemLink" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
								<div>
							</ItemTemplate>
							<SeparatorStyle Height="1px" />
						</asp:DataList>
					</td>
				</tr>

