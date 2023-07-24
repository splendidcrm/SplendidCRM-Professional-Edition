<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdate.ascx.cs" Inherits="SplendidCRM.Administration.ProductTemplates.MassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<%-- 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. --%>
<%@ Register TagPrefix="SplendidCRM" Tagname="MassUpdateButtons" Src="~/_controls/MassUpdateButtons.ascx" %>
<SplendidCRM:MassUpdateButtons ID="ctlDynamicButtons" SubPanel="divProductTemplatesMassUpdate" Title=".LBL_MASS_UPDATE_TITLE" Runat="Server" />

<div id="divProductTemplatesMassUpdate" style='<%= "display:" + (CookieValue("divProductTemplatesMassUpdate") != "1" ? "inline" : "none") %>'>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm tabMassUpdate" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_DATE_COST_PRICE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlDATE_COST_PRICE" Runat="Server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_STATUS") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstSTATUS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_TAX_CLASS") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstTAX_CLASS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_DATE_AVAILABLE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlDATE_AVAILABLE" Runat="Server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_SUPPORT_TERM") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstSUPPORT_TERM" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_TYPE_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtTYPE_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtTYPE_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnTYPE_ID" UseSubmitBehavior="false" OnClientClick=<%# "return ModulePopup('ProductTypes', '" + txtTYPE_ID.ClientID + "', '" + txtTYPE_NAME.ClientID + "', null, false, null);" %> Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_QUICKBOOKS_ACCOUNT") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:TextBox ID="txtQUICKBOOKS_ACCOUNT" MaxLength="41" Size="25" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" Visible="false">
							<asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_ACCOUNT_NAME") %>' Visible='<%# Sql.ToString(Application["CONFIG.BusinessMode"]) != "B2C" %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" Visible="false">
							<asp:TextBox     ID="txtACCOUNT_NAME" ReadOnly="True" Visible='<%# Sql.ToString(Application["CONFIG.BusinessMode"]) != "B2C" %>' Runat="server" />
							<asp:HiddenField ID="txtACCOUNT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnACCOUNT_ID" UseSubmitBehavior="false" OnClientClick=<%# "return ModulePopup('Accounts', '" + txtACCOUNT_ID.ClientID + "', '" + txtACCOUNT_NAME.ClientID + "', null, false, null);" %> Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" Visible='<%# Sql.ToString(Application["CONFIG.BusinessMode"]) != "B2C" %>' runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
				<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedMassUpdate" Src="~/_controls/TeamAssignedMassUpdate.ascx" %>
				<SplendidCRM:TeamAssignedMassUpdate ID="ctlTeamAssignedMassUpdate" ShowAssigned="false" Runat="Server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
