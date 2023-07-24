<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdate.ascx.cs" Inherits="SplendidCRM.Quotes.MassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="SplendidCRM" Tagname="DatePicker" Src="~/_controls/DatePicker.ascx" %>
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
function ShippingAccountPopup()
{
	return ModulePopup('Accounts', '<%= txtSHIPPING_ACCOUNT_ID.ClientID %>', '<%= txtSHIPPING_ACCOUNT_NAME.ClientID %>', null, false, null);
}
function BillingAccountPopup()
{
	return ModulePopup('Accounts', '<%= txtBILLING_ACCOUNT_ID.ClientID %>', '<%= txtBILLING_ACCOUNT_NAME.ClientID %>', null, false, null);
}
function ShippingContactPopup()
{
	return ModulePopup('Contacts', '<%= txtSHIPPING_CONTACT_ID.ClientID %>', '<%= txtSHIPPING_CONTACT_NAME.ClientID %>', null, false, null);
}
function BillingContactPopup()
{
	return ModulePopup('Contacts', '<%= txtBILLING_CONTACT_ID.ClientID %>', '<%= txtBILLING_CONTACT_NAME.ClientID %>', null, false, null);
}
</script>
<%-- 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. --%>
<%@ Register TagPrefix="SplendidCRM" Tagname="MassUpdateButtons" Src="~/_controls/MassUpdateButtons.ascx" %>
<SplendidCRM:MassUpdateButtons ID="ctlDynamicButtons" SubPanel="divQuotesMassUpdate" Title=".LBL_MASS_UPDATE_TITLE" Runat="Server" />

<%-- 03/26/2018 Paul.  Hide mass update fields in ArchiveView. --%>
<div id="divQuotesMassUpdate" style='<%= "display:" + (CookieValue("divQuotesMassUpdate") != "1" && !ArchiveView() ? "inline" : "none") %>'>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm tabMassUpdate" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedMassUpdate" Src="~/_controls/TeamAssignedMassUpdate.ascx" %>
				<SplendidCRM:TeamAssignedMassUpdate ID="ctlTeamAssignedMassUpdate" Runat="Server" />
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_DATE_VALID_UNTIL") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlDATE_QUOTE_EXPECTED_CLOSED" Runat="Server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_ORIGINAL_PO_DATE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlORIGINAL_PO_DATE" Runat="Server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_PAYMENT_TERMS") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstPAYMENT_TERMS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_QUOTE_STAGE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstQUOTE_STAGE" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_SHIPPING_ACCOUNT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtSHIPPING_ACCOUNT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtSHIPPING_ACCOUNT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnSHIPPING_ACCOUNT_ID" UseSubmitBehavior="false" OnClientClick="return ShippingAccountPopup();" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_SHIPPING_CONTACT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtSHIPPING_CONTACT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtSHIPPING_CONTACT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnSHIPPING_CONTACT_ID" UseSubmitBehavior="false" OnClientClick="return ShippingContactPopup();" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_BILLING_ACCOUNT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtBILLING_ACCOUNT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtBILLING_ACCOUNT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnBILLING_ACCOUNT_ID" UseSubmitBehavior="false" OnClientClick="return BillingAccountPopup();" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_BILLING_CONTACT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtBILLING_CONTACT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtBILLING_CONTACT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnBILLING_CONTACT_ID" UseSubmitBehavior="false" OnClientClick="return BillingContactPopup();" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term(".LBL_TAG_SET_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<%@ Register TagPrefix="SplendidCRM" Tagname="TagMassUpdate" Src="~/_controls/TagMassUpdate.ascx" %>
							<SplendidCRM:TagMassUpdate ID="ctlTagMassUpdate" Runat="Server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
