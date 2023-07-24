<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConvertViewAccount.ascx.cs" Inherits="SplendidCRM.Leads.ConvertViewAccount" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	function AccountPopup()
	{
		return ModulePopup('Accounts', '<%= txtSELECT_ACCOUNT_ID.ClientID %>', '<%= txtSELECT_ACCOUNT_NAME.ClientID %>', null, false, null);
	}
	function ToggleCreateAccount()
	{
		var divCreateAccount = document.getElementById('divCreateAccount');
		var divSelectAccount = document.getElementById('divSelectAccount');
		if( divCreateAccount.style.display == 'none' )
		{
			divCreateAccount.style.display = 'inline';
			divSelectAccount.style.display = 'none'  ;
			// 12/15/2013 Paul.  Stop clearing to allow creation toggle. 
			//ClearSelectAccount();
		}
		else
		{
			divCreateAccount.style.display = 'none'  ;
			divSelectAccount.style.display = 'inline';
		}
	}
	function ClearSelectAccount()
	{
		// 03/04/2009 Paul.  Must use ClientID to access the controls. 
		// 07/27/2010 Paul.  Add the ability to submit after clear. 
		ClearModuleType('Accounts', '<%= txtSELECT_ACCOUNT_ID.ClientID %>', '<%= txtSELECT_ACCOUNT_NAME.ClientID %>', false);
		return false;
	}
</script>
<div id="divConvertViewAccount">
	<h5 CssClass="dataLabel" style="display:<%= sBusinessMode == "B2C" ? "none" : "inline" %>">
		<asp:CheckBox ID="chkCreateAccount" CssClass="checkbox" Runat="server" />
		<%= L10n.Term("Leads.LNK_NEW_ACCOUNT") %>
		&nbsp;<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</h5>
	<div id="divSelectAccount" style="display:<%= (sBusinessMode == "B2C" || chkCreateAccount.Checked) ? "none" : "inline" %>">
		<b><%= L10n.Term(".LBL_OR") %></b>
		<b><%= L10n.Term("Leads.LNK_SELECT_ACCOUNT") %></b>&nbsp;
		<asp:TextBox ID="txtSELECT_ACCOUNT_NAME" ReadOnly="True" Runat="server" />
		<input ID="txtSELECT_ACCOUNT_ID" type="hidden" runat="server" />
		<input ID="btnChangeSelectAccount" type="button" CssClass="button" onclick="return AccountPopup();"       title="<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>" />
		<input ID="btnClearSelectAccount"  type="button" CssClass="button" onclick="return ClearSelectAccount();" title="<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>" AccessKey="<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>" value="<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) %>" />
		<SplendidCRM:RequiredFieldValidatorForHiddenInputs ID="reqSELECT_ACCOUNT_ID" ControlToValidate="txtSELECT_ACCOUNT_ID" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
	</div>
	<div id="divCreateAccount" style="display:<%= chkCreateAccount.Checked ? "inline" : "none" %>">
		<table ID="tblMain" class="tabEditView" runat="server">
		</table>
		
		<div id="divCreateAccountNoteLink">
			&nbsp;<asp:CheckBox ID="chkCreateNote" CssClass="checkbox" Runat="server" />
			&nbsp;<%= L10n.Term("Leads.LNK_NEW_NOTE") %>
		</div>
		<div id="divCreateAccountNote" style="display:<%= chkCreateNote.Checked ? "inline" : "none" %>">
			<p></p>
			<%@ Register TagPrefix="SplendidCRM" Tagname="ConvertViewNote" Src="ConvertViewNote.ascx" %>
			<SplendidCRM:ConvertViewNote ID="ctlConvertViewNote" Runat="Server" />
		</div>
	</div>
</div>
