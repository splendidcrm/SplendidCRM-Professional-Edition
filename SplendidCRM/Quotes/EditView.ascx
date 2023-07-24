<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Quotes.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
// 07/27/2010 Paul.  Use the DATA_FORMAT field to determine if the ModulePopup will auto-submit. 
// 07/27/2010 Paul.  We need to allow an onclick to override the default ModulePopup behavior. 
// 08/03/2010 Paul.  Include the ACCOUNT_ID so that it can be used when creating a Contact. 
// 10/04/2010 Paul.  Changing a contact should auto-submit. 
// 09/03/2012 Paul.  Must place within InlineScript to prevent error. The Controls collection cannot be modified because the control contains code blocks (i.e. ).
function BillingContactPopup()
{
	// 02/29/2016 Paul.  Account fields will not exist in B2C mode. 
	var sACCOUNT_ID   = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "BILLING_ACCOUNT_ID"  ).ClientID %>') != null ? document.getElementById('<%= new SplendidCRM.DynamicControl(this, "BILLING_ACCOUNT_ID"  ).ClientID %>').value : '';
	var sACCOUNT_NAME = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "BILLING_ACCOUNT_NAME").ClientID %>') != null ? document.getElementById('<%= new SplendidCRM.DynamicControl(this, "BILLING_ACCOUNT_NAME").ClientID %>').value : '';
	return ModulePopup('Contacts', '<%= new SplendidCRM.DynamicControl(this, "BILLING_CONTACT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "BILLING_CONTACT_NAME").ClientID %>', 'ACCOUNT_NAME=' + escape(sACCOUNT_NAME) + '&ACCOUNT_ID=' + escape(sACCOUNT_ID), true, null);
}
function ShippingContactPopup()
{
	// 02/29/2016 Paul.  Account fields will not exist in B2C mode. 
	var sACCOUNT_ID   = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "SHIPPING_ACCOUNT_ID"  ).ClientID %>') != null ? document.getElementById('<%= new SplendidCRM.DynamicControl(this, "SHIPPING_ACCOUNT_ID"  ).ClientID %>').value : '';
	var sACCOUNT_NAME = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "SHIPPING_ACCOUNT_NAME").ClientID %>') != null ? document.getElementById('<%= new SplendidCRM.DynamicControl(this, "SHIPPING_ACCOUNT_NAME").ClientID %>').value : '';
	return ModulePopup('Contacts', '<%= new SplendidCRM.DynamicControl(this, "SHIPPING_CONTACT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "SHIPPING_CONTACT_NAME").ClientID %>', 'ACCOUNT_NAME=' + escape(sACCOUNT_NAME) + '&ACCOUNT_ID=' + escape(sACCOUNT_ID), true, null);
}
</script>
</SplendidCRM:InlineScript>
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Quotes" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

	<asp:HiddenField ID="LAYOUT_EDIT_VIEW" Runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Button ID="BILLING_ACCOUNT_UPDATE"  Text="BILLING_ACCOUNT_UPDATE"  style="display: none" runat="server" />
	<asp:Button ID="SHIPPING_ACCOUNT_UPDATE" Text="SHIPPING_ACCOUNT_UPDATE" style="display: none" runat="server" />
	<asp:Button ID="BILLING_CONTACT_UPDATE"  Text="BILLING_CONTACT_UPDATE"  style="display: none" runat="server" />
	<asp:Button ID="SHIPPING_CONTACT_UPDATE" Text="SHIPPING_CONTACT_UPDATE" style="display: none" runat="server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditLineItemsView" Src="~/_controls/EditLineItemsView.ascx" %>
	<SplendidCRM:EditLineItemsView ID="ctlEditLineItemsView" MODULE="Quotes" MODULE_KEY="QUOTE_ID" Runat="Server" />

	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblDescription" class="tabEditView" runat="server">
					<tr>
						<th colspan="2"><h4><asp:Label Text='<%# L10n.Term("Quotes.LBL_DESCRIPTION_TITLE") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />

	<div id="divEditSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
