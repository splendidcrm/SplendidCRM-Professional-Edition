<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Accounts.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
// 09/03/2012 Paul.  Must place within InlineScript to prevent error. The Controls collection cannot be modified because the control contains code blocks (i.e. ).
function CopyBillingToContact()
{
	var txtADDRESS_TYPE = document.getElementById('<%= txtADDRESS_TYPE.ClientID %>');
	txtADDRESS_TYPE.value = "Billing";
	return ModulePopup('Contacts', '<%= txtCONTACT_ID.ClientID %>', null, 'ClearDisabled=1&FilterAccount=1&ACCOUNT_ID=<%= gID %>', true, 'PopupMultiSelect.aspx');
	return false;
}
function CopyShippingToContact()
{
	var txtADDRESS_TYPE = document.getElementById('<%= txtADDRESS_TYPE.ClientID %>');
	txtADDRESS_TYPE.value = "Shipping";
	return ModulePopup('Contacts', '<%= txtCONTACT_ID.ClientID %>', null, 'ClearDisabled=1&FilterAccount=1&ACCOUNT_ID=<%= gID %>', true, 'PopupMultiSelect.aspx');
	return false;
}
</script>
</SplendidCRM:InlineScript>
<div id="divDetailView" runat="server">
	<asp:HiddenField ID="txtADDRESS_TYPE" Runat="server" />
	<asp:HiddenField ID="txtCONTACT_ID"     Runat="server" />

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Accounts" EnablePrint="true" HelpName="DetailView" EnableHelp="true" EnableFavorites="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailNavigation" Src="~/_controls/DetailNavigation.ascx" %>
	<SplendidCRM:DetailNavigation ID="ctlDetailNavigation" Module="<%# m_sMODULE %>" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:HiddenField ID="LAYOUT_DETAIL_VIEW" Runat="server" />
	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
