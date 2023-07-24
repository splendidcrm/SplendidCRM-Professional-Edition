<%@ Page language="c#" Codebehind="Popup.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.Popup" %>
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
<!DOCTYPE HTML>
<html>
<head runat="server">
	<title>Calendar</title>
	<base target="_self" />
</head>
<script type="text/javascript">
function SelectDate(sDATE)
{
	if ( window.opener != null && window.opener.ChangeDate != null )
	{
		window.opener.ChangeDate(sDATE);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Date cannot be set.');
	}
}
</script>
<body leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
	<form id="frm" method="post" runat="server">
		<asp:Calendar ID="ctlCalendar" OnSelectionChanged="ctlCalendar_SelectionChanged" CssClass="Calendar" Runat="server">
			<TitleStyle         CssClass="CalendarTitle" />
			<DayHeaderStyle     CssClass="CalendarDayHeader" />
			<DayStyle           CssClass="CalendarDay" />
			<OtherMonthDayStyle CssClass="CalendarOtherMonthDay" />
			<NextPrevStyle      CssClass="" />
			<SelectedDayStyle   CssClass="" />
			<SelectorStyle      CssClass="" />
			<TodayDayStyle      CssClass="CalendarToday" />
			<WeekendDayStyle    CssClass="CalendarWeekendDay" />
		</asp:Calendar>
	</form>
</body>
</html>

