<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ProcessButtons.ascx.cs" Inherits="SplendidCRM._controls.ProcessButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
function ProcessPopupOptions()
{
	return '<%# SplendidCRM.Crm.Config.PopupWindowOptions() %>';
}
function SelectProcessUserPopup()
{
	var hidPROCESS_TEAM_ID = document.getElementById('<%= hidPROCESS_TEAM_ID.ClientID %>');
	var hidPROCESS_ACTION  = document.getElementById('<%= hidPROCESS_ACTION .ClientID %>');
	hidPROCESS_ACTION.value = 'ChangeProcessUser';
	var sPopupURL = '<%= Sql.ToString(Application["rootURL"]) + "Processes/SelectUserPopup.aspx?ID=" %>' + hidPROCESS_TEAM_ID.value;
	return window.open(sPopupURL, 'SelectProcessUserPopup', ProcessPopupOptions());
}
function SelectAssignedUserPopup()
{
	var hidASSIGNED_TEAM_ID = document.getElementById('<%= hidASSIGNED_TEAM_ID.ClientID %>');
	var hidPROCESS_ACTION   = document.getElementById('<%= hidPROCESS_ACTION  .ClientID %>');
	hidPROCESS_ACTION.value = 'ChangeAssignedUser';
	var sPopupURL = '<%= Sql.ToString(Application["rootURL"]) + "Processes/SelectUserPopup.aspx?ID=" %>' + hidASSIGNED_TEAM_ID.value;
	return window.open(sPopupURL, 'SelectProcessUserPopup', ProcessPopupOptions());
}
function ChangeProcessUser(sPARENT_ID, sPARENT_NAME, sPROCESS_NOTES)
{
	var hidPROCESS_USER_ID   = document.getElementById('<%= hidPROCESS_USER_ID.ClientID   %>');
	var hidPROCESS_NOTES     = document.getElementById('<%= hidPROCESS_NOTES.ClientID     %>');
	var btnChangeProcessUser = document.getElementById('<%= btnChangeProcessUser.ClientID %>');
	hidPROCESS_USER_ID.value = sPARENT_ID;
	hidPROCESS_NOTES.value   = sPROCESS_NOTES;
	btnChangeProcessUser.click();
}
function ProcessHistoryPopup()
{
	var hidPENDING_PROCESS_ID = document.getElementById('<%= hidPENDING_PROCESS_ID.ClientID %>');
	var sPopupURL = '<%= Sql.ToString(Application["rootURL"]) + "Processes/ProcessHistoryPopup.aspx?PROCESS_ID=" %>' + hidPENDING_PROCESS_ID.value;
	return window.open(sPopupURL, 'ProcessHistoryPopup', ProcessPopupOptions());
}
function ProcessNotesPopup()
{
	var hidPENDING_PROCESS_ID = document.getElementById('<%= hidPENDING_PROCESS_ID.ClientID %>');
	var sPopupURL = '<%= Sql.ToString(Application["rootURL"]) + "Processes/ProcessNotesPopup.aspx?PROCESS_ID=" %>' + hidPENDING_PROCESS_ID.value;
	return window.open(sPopupURL, 'ProcessNotesPopup', ProcessPopupOptions());
}
</script>
<asp:HiddenField ID="hidPROCESS_USER_ID"    runat="server" />
<asp:HiddenField ID="hidPROCESS_NOTES"      runat="server" />
<asp:HiddenField ID="hidPROCESS_ACTION"     runat="server" />
<asp:HiddenField ID="hidPENDING_PROCESS_ID" runat="server" />
<asp:HiddenField ID="hidASSIGNED_TEAM_ID"   runat="server" />
<asp:HiddenField ID="hidPROCESS_TEAM_ID"    runat="server" />
<asp:HiddenField ID="hidSTATUS"             runat="server" />
<asp:HiddenField ID="hidERASED_COUNT"       runat="server" />
<asp:Button      ID="btnChangeProcessUser" OnCommand="Page_Command" CommandName="Processes.ChangeUser" Text="Change User" style="display: none;" runat="server" />
<div>
	<asp:Label ID="txtProcessStatus" CssClass="ProcessStatus" Visible="false" runat="server" />
</div>
