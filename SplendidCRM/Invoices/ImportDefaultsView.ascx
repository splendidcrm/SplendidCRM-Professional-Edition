<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImportDefaultsView.ascx.cs" Inherits="SplendidCRM.Invoices.ImportDefaultsView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
// 11/23/2012 Paul.  Must place within InlineScript to prevent error. The Controls collection cannot be modified because the control contains code blocks (i.e. ).
function BillingAccountPopup()
{
	// 07/21/2010 Paul.  Don't post back after selection. 
	return ModulePopup('Accounts', '<%= new SplendidCRM.DynamicControl(this, "BILLING_ACCOUNT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "BILLING_ACCOUNT_NAME").ClientID %>', null, false, null);
}
function ShippingAccountPopup()
{
	// 07/21/2010 Paul.  Don't post back after selection. 
	return ModulePopup('Accounts', '<%= new SplendidCRM.DynamicControl(this, "SHIPPING_ACCOUNT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "SHIPPING_ACCOUNT_NAME").ClientID %>', null, false, null);
}
function BillingContactPopup()
{
	var sACCOUNT_NAME = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "BILLING_ACCOUNT_NAME").ClientID %>').value;
	return ModulePopup('Contacts', '<%= new SplendidCRM.DynamicControl(this, "BILLING_CONTACT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "BILLING_CONTACT_NAME").ClientID %>', 'ACCOUNT_NAME=' + escape(sACCOUNT_NAME), false, null);
}
function ShippingContactPopup()
{
	var sACCOUNT_NAME = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "SHIPPING_ACCOUNT_NAME").ClientID %>').value;
	return ModulePopup('Contacts', '<%= new SplendidCRM.DynamicControl(this, "SHIPPING_CONTACT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "SHIPPING_CONTACT_NAME").ClientID %>', 'ACCOUNT_NAME=' + escape(sACCOUNT_NAME), false, null);
}
</script>
</SplendidCRM:InlineScript>
<div id="divDefaultsView">
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblDescription" class="tabEditView" runat="server">
					<tr>
						<th colspan="2"><h4><asp:Label Text='<%# L10n.Term("Invoices.LBL_DESCRIPTION_TITLE") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
</div>
