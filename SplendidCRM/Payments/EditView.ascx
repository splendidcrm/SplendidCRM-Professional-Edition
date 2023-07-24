<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Payments.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
// 01/03/2008 Paul.  num_grp_sep must be defined in order for numbers to be formatted properly. 
// 02/29/2008 Paul.  The config value should only be used as an override.  We should default to the .NET culture value. 
// 03/07/2008 Paul.  We are going to let the ASP.NET and Microsoft AJAX culture engines take care of number formatting. 
// 03/07/2008 Paul.  Prevent Microsoft AJAX from using the currency symbol.  
// The problem is that the culture value may not match the user's selected value, which can cause parsing errors. 
Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol = '';

// 09/03/2012 Paul.  Must place within InlineScript to prevent error. The Controls collection cannot be modified because the control contains code blocks (i.e. ).
// 05/01/2013 Paul.  Add Contacts field to support B2C. 
function CreditCardPopup()
{
	var fldACCOUNT_ID = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "ACCOUNT_ID"    ).ClientID %>');
	var fldCONTACT_ID = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "B2C_CONTACT_ID").ClientID %>');
	var sACCOUNT_ID = (fldACCOUNT_ID != null ? fldACCOUNT_ID.value : '');
	var sCONTACT_ID = (fldCONTACT_ID != null ? fldCONTACT_ID.value : '');
	return ModulePopup('CreditCards', '<%= new SplendidCRM.DynamicControl(this, "CREDIT_CARD_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "CREDIT_CARD_NAME").ClientID %>', 'ACCOUNT_ID=' + sACCOUNT_ID + '&CONTACT_ID=' + sCONTACT_ID, false, null);
}
</script>
</SplendidCRM:InlineScript>
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Payments" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<asp:Button ID="ACCOUNT_UPDATE"  Text="ACCOUNT_UPDATE"  style="display: none" runat="server" />
	<asp:HiddenField ID="LAYOUT_EDIT_VIEW" Runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="AllocationsView" Src="AllocationsView.ascx" %>
	<SplendidCRM:AllocationsView ID="ctlAllocationsView" Runat="Server" />
</div>
