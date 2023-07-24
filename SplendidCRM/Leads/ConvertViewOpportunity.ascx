<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConvertViewOpportunity.ascx.cs" Inherits="SplendidCRM.Leads.ConvertViewOpportunity" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	function OpportunityPopup()
	{
		return ModulePopup('Opportunities', '<%= txtSELECT_OPPORTUNITY_ID.ClientID %>', '<%= txtSELECT_OPPORTUNITY_NAME.ClientID %>', null, false, null);
	}
	function ToggleCreateOpportunity()
	{
		var divCreateOpportunity = document.getElementById('divCreateOpportunity');
		var divSelectOpportunity = document.getElementById('divSelectOpportunity');
		if( divCreateOpportunity.style.display == 'none' )
		{
			divCreateOpportunity.style.display = 'inline';
			divSelectOpportunity.style.display = 'none'  ;
			ClearSelectOpportunity();
		}
		else
		{
			divCreateOpportunity.style.display = 'none'  ;
			divSelectOpportunity.style.display = 'inline';
		}
	}
	function ClearSelectOpportunity()
	{
		// 03/04/2009 Paul.  Must use ClientID to access the controls. 
		// 07/27/2010 Paul.  Add the ability to submit after clear. 
		ClearModuleType('Opportunities', '<%= txtSELECT_OPPORTUNITY_ID.ClientID %>', '<%= txtSELECT_OPPORTUNITY_NAME.ClientID %>', false);
		return false;
	}
</script>
<div id="divConvertViewOpportunity">
	<h5 CssClass="dataLabel" style="display:inline">
		<asp:CheckBox ID="chkCreateOpportunity" CssClass="checkbox" Runat="server" />
		<%= L10n.Term("Leads.LNK_NEW_OPPORTUNITY") %>
		&nbsp;<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</h5>
	<div id="divSelectOpportunity" style="display:<%= chkCreateOpportunity.Checked ? "none" : "inline" %>">
		<b><%= L10n.Term(".LBL_OR") %></b>
		<b><%= L10n.Term("Leads.LNK_SELECT_OPPORTUNITY") %></b>&nbsp;
		<asp:TextBox ID="txtSELECT_OPPORTUNITY_NAME" ReadOnly="True" Runat="server" />
		<input ID="txtSELECT_OPPORTUNITY_ID" type="hidden" runat="server" />
		<input ID="btnChangeSelectOpportunity" type="button" CssClass="button" onclick="return OpportunityPopup();"       title="<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>" />
		<input ID="btnClearSelectOpportunity"  type="button" CssClass="button" onclick="return ClearSelectOpportunity();" title="<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>" AccessKey="<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>" value="<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) %>" />
		<SplendidCRM:RequiredFieldValidatorForHiddenInputs ID="reqSELECT_OPPORTUNITY_ID" ControlToValidate="txtSELECT_OPPORTUNITY_ID" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
	</div>
	<div id="divCreateOpportunity" style="display:<%= chkCreateOpportunity.Checked ? "inline" : "none" %>">
		<table ID="tblMain" class="tabEditView" runat="server">
		</table>
		
		<div id="divCreateOpportunityNoteLink">
			&nbsp;<asp:CheckBox ID="chkCreateNote" CssClass="checkbox" Runat="server" />
			&nbsp;<%= L10n.Term("Leads.LNK_NEW_NOTE") %>
		</div>
		<div id="divCreateOpportunityNote" style="display:<%= chkCreateNote.Checked ? "inline" : "none" %>">
			<p></p>
			<%@ Register TagPrefix="SplendidCRM" Tagname="ConvertViewNote" Src="ConvertViewNote.ascx" %>
			<SplendidCRM:ConvertViewNote ID="ctlConvertViewNote" Runat="Server" />
		</div>
	</div>
</div>
