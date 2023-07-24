<%@ Control CodeBehind="PaymentView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.PaymentView" %>
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
<div id="divPaymentView" visible='<%# 
(  SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("PayPal"      , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("PayTrace"    , "access") >= 0 
) %>' runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_PAYMENT_SERVICES_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
				<asp:Image ID="imgAuthorizeNet" SkinID="AuthorizeNet" AlternateText='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_SETTINGS") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkAuthorizeNet" Text='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_SETTINGS") %>' NavigateUrl="~/Administration/AuthorizeNet/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
				<asp:Label ID="lblAuthorizeNet" Text='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_SETTINGS_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
				<asp:Image ID="imgAuthorizeNetTransactions" SkinID="AuthorizeNet" AlternateText='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_TRANSACTIONS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkAuthorizeNetTransactions" Text='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_TRANSACTIONS") %>' NavigateUrl="~/Administration/AuthorizeNet/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
				<asp:Label ID="lblAuthorizeNetTransactions" Text='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_TRANSACTIONS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
				<asp:Image ID="imgAuthorizeNetCustomerProfiles" SkinID="AuthorizeNet" AlternateText='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_CUSTOMER_PROFILES") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkAuthorizeNetCustomerProfiles" Text='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_CUSTOMER_PROFILES") %>' NavigateUrl="~/Administration/AuthorizeNet/CustomerProfiles/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuthorizeNet", "edit") >= 0 %>'>
				<asp:Label ID="lblAuthorizeNetCustomerProfiles" Text='<%# L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_CUSTOMER_PROFILES_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayPal", "edit") >= 0 %>'>
				<asp:Image ID="imgPayPal" SkinID="PayPal" AlternateText='<%# L10n.Term("PayPal.LBL_PAYPAL_SETTINGS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkPayPal" Text='<%# L10n.Term("PayPal.LBL_PAYPAL_SETTINGS") %>' NavigateUrl="~/Administration/PayPalTransactions/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayPal", "edit") >= 0 %>'>
				<asp:Label ID="lblPayPal" Text='<%# L10n.Term("PayPal.LBL_PAYPAL_SETTINGS_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayPal", "edit") >= 0 %>'>
				<asp:Image ID="imgPayPalTransactions" SkinID="PayPal" AlternateText='<%# L10n.Term("PayPal.LBL_PAYPAL_TRANSACTIONS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkPayPalTransactions" Text='<%# L10n.Term("PayPal.LBL_PAYPAL_TRANSACTIONS") %>' NavigateUrl="~/Administration/PayPalTransactions/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayPal", "edit") >= 0 %>'>
				<asp:Label ID="lblPayPalTransactions" Text='<%# L10n.Term("Administration.LBL_PAYPAL_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayTrace", "edit") >= 0 %>'>
				<asp:Image ID="imgPayTrace" SkinID="PayTrace" AlternateText='<%# L10n.Term("PayTrace.LBL_PAYTRACE_SETTINGS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkPayTrace" Text='<%# L10n.Term("PayTrace.LBL_PAYTRACE_SETTINGS") %>' NavigateUrl="~/Administration/PayTrace/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayTrace", "edit") >= 0 %>'>
				<asp:Label ID="lblPayTrace" Text='<%# L10n.Term("PayTrace.LBL_PAYTRACE_SETTINGS_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayTrace", "edit") >= 0 %>'>
				<asp:Image ID="imgPayTraceTransactions" SkinID="PayTrace" AlternateText='<%# L10n.Term("PayTrace.LBL_PAYTRACE_TRANSACTIONS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkPayTraceTransactions" Text='<%# L10n.Term("PayTrace.LBL_PAYTRACE_TRANSACTIONS") %>' NavigateUrl="~/Administration/PayTrace/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PayTrace", "edit") >= 0 %>'>
				<asp:Label ID="lblPayTraceTransactions" Text='<%# L10n.Term("PayTrace.LBL_PAYTRACE_TRANSACTIONS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

