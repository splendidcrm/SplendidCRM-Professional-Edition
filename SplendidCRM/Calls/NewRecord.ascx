<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Calls.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<script type="text/javascript">
function <%= this.ClientID %>_UserCalendarPopup()
{
	try
	{
		var sDateFormat         = '<%= Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"]) %>';
		var sTimeFormat         = '<%= Sql.ToString(Session["USER_SETTINGS/TIMEFORMAT"]) %>';
		var sASSIGNED_USER_ID   = '<%= new SplendidCRM.DynamicControl(this, "ASSIGNED_USER_ID").ClientID %>';
		var sASSIGNED_TO_NAME   = '<%= new SplendidCRM.DynamicControl(this, "ASSIGNED_TO_NAME").ClientID %>';
		var txtDATE             = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "DATE_START"      ).ClientID + "_txtDATE"     %>');
		var txtTIME             = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "DATE_START"      ).ClientID + "_txtTIME"     %>');
		var lstHOUR             = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "DATE_START"      ).ClientID + "_lstHOUR"     %>');
		var lstMINUTE           = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "DATE_START"      ).ClientID + "_lstMINUTE"   %>');
		var lstMERIDIEM         = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "DATE_START"      ).ClientID + "_lstMERIDIEM" %>');
		var txtDURATION_HOURS   = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "DURATION_HOURS"  ).ClientID %>');
		var lstDURATION_MINUTES = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "DURATION_MINUTES").ClientID %>');
		var dtDATE_START        = new Date();
		var dtDATE_END          = new Date();
		var nHOUR               = dtDATE_START.getHours();
		var nMINUTE             = 0;
		var nDURATION_HOURS     = 1;
		var nDURATION_MINUTES   = 0;
		var sAMPM               = 'AM';
		if ( txtDATE != null )
		{
			// http://api.jqueryui.com/datepicker/
			dtDATE_START = $.datepicker.parseDate(sDateFormat.replace('MM', 'mm').replace('yyyy', 'yy'), txtDATE.value);
			dtDATE_END   = $.datepicker.parseDate(sDateFormat.replace('MM', 'mm').replace('yyyy', 'yy'), txtDATE.value);
		}
		if ( txtTIME != null )
		{
			var time = txtTIME.value.toLowerCase().match(/(\d+)(?::(\d\d))?\s*(p?)/);
			if ( time != null )
			{
				nHOUR   = parseInt(time[1]) + (time[3] ? 12 : 0);
				nMINUTE = parseInt(time[2]) || 0;
			}
			alert(nHOUR + ':' + nMINUTE);
		}
		if ( lstHOUR             != null ) nHOUR             = parseInt(lstHOUR            .options[lstHOUR            .selectedIndex].value);
		if ( lstMINUTE           != null ) nMINUTE           = parseInt(lstMINUTE          .options[lstMINUTE          .selectedIndex].value);
		if ( lstMERIDIEM         != null ) sAMPM             = lstMERIDIEM                 .options[lstMERIDIEM        .selectedIndex].value
		if ( txtDURATION_HOURS   != null ) nDURATION_HOURS   = parseInt(txtDURATION_HOURS.value);
		if ( lstDURATION_MINUTES != null ) nDURATION_MINUTES = parseInt(lstDURATION_MINUTES.options[lstDURATION_MINUTES.selectedIndex].value);
		if ( sAMPM == 'PM' )
			nHOUR += 12;
		dtDATE_START.setHours  (nHOUR  );
		dtDATE_START.setMinutes(nMINUTE);
		dtDATE_END  .setHours  (nHOUR   + nDURATION_HOURS  );
		dtDATE_END.  setMinutes(nMINUTE + nDURATION_MINUTES);
		var sDATE_START = $.fullCalendar.formatDate(dtDATE_START, sDateFormat + ' ' + sTimeFormat);
		var sDATE_END   = $.fullCalendar.formatDate(dtDATE_END  , sDateFormat + ' ' + sTimeFormat);
		return ModulePopup('Users', sASSIGNED_USER_ID, sASSIGNED_TO_NAME, 'FULL_NAME=1&DATE_START=' + escape(sDATE_START) + '&DATE_END=' + escape(sDATE_END), false, 'UserCalendarPopup.aspx');
	}
	catch(e)
	{
		alert(e.message);
	}
}
</script>
<div id="divNewRecord">
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderLeft" Src="~/_controls/HeaderLeft.ascx" %>
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="Calls.LBL_NEW_FORM_TITLE" Width=<%# uWidth %> Visible="<%# ShowHeader %>" Runat="Server" />

	<asp:Panel ID="pnlMain" Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
		<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# ShowTopButtons && !PrintView %>" Runat="server" />

		<asp:Panel ID="pnlEdit" CssClass="" style="margin-bottom: 4px;" Width=<%# uWidth %> runat="server">
			<asp:Literal Text='<%# "<h4>" + L10n.Term("Calls.LBL_NEW_FORM_TITLE") + "</h4>" %>' Visible="<%# ShowInlineHeader %>" runat="server" />
			<table ID="tblMain" class="tabEditView" runat="server">
			</table>
		</asp:Panel>

		<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# ShowBottomButtons && !PrintView %>" Runat="server" />
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
</div>

