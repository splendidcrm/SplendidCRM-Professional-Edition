<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CalendarHeader.ascx.cs" Inherits="SplendidCRM.Calendar.CalendarHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
	<asp:Button ID="btnDay"    CommandName="Day.Current"    OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_DAY"   ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_DAY"   ) %>' Runat="server" />
	<asp:Button ID="btnWeek"   CommandName="Week.Current"   OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_WEEK"  ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_WEEK"  ) %>' Runat="server" />
	<asp:Button ID="btnMonth"  CommandName="Month.Current"  OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_MONTH" ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_MONTH" ) %>' Runat="server" />
	<asp:Button ID="btnYear"   CommandName="Year.Current"   OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_YEAR"  ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_YEAR"  ) %>' Runat="server" />
	<asp:Button ID="btnShared" CommandName="Shared.Current" OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_SHARED") + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_SHARED") %>' Runat="server" />
</asp:Panel>

