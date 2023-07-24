<%@ Control CodeBehind="ProductsView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ProductsView" %>
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
<div id="divProductsView" visible='<%# 
(  SplendidCRM.Security.AdminUserAccess("ProductTemplates" , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Manufacturers"    , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("ProductCategories", "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Shippers"         , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("ProductTypes"     , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("TaxRates"         , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Discounts"        , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Regions"          , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("PaymentTypes"     , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("PaymentTerms"     , "access") >= 0 
) %>' runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_PRODUCTS_QUOTES_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ProductTemplates", "access") >= 0 %>'>
				<asp:Image SkinID="ProductTemplates" AlternateText='<%# L10n.Term("Administration.LBL_PRODUCT_TEMPLATES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_PRODUCT_TEMPLATES_TITLE") %>' NavigateUrl="~/Administration/ProductTemplates/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ProductTemplates", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_PRODUCT_TEMPLATES_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Manufacturers", "access") >= 0 %>'>
				<asp:Image SkinID="Manufacturers" AlternateText='<%# L10n.Term("Administration.LBL_MANUFACTURERS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANUFACTURERS_TITLE") %>' NavigateUrl="~/Administration/Manufacturers/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Manufacturers", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MANUFACTURERS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ProductCategories", "access") >= 0 %>'>
				<asp:Image SkinID="ProductCategories" AlternateText='<%# L10n.Term("Administration.LBL_PRODUCT_CATEGORIES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_PRODUCT_CATEGORIES_TITLE") %>' NavigateUrl="~/Administration/ProductCategories/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ProductCategories", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_PRODUCT_CATEGORIES_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Shippers", "access") >= 0 %>'>
				<asp:Image SkinID="Shippers" AlternateText='<%# L10n.Term("Administration.LBL_SHIPPERS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SHIPPERS_TITLE") %>' NavigateUrl="~/Administration/Shippers/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Shippers", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_SHIPPERS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ProductTypes", "access") >= 0 %>'>
				<asp:Image SkinID="ProductTypes" AlternateText='<%# L10n.Term("Administration.LBL_PRODUCT_TYPES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_PRODUCT_TYPES_TITLE") %>' NavigateUrl="~/Administration/ProductTypes/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ProductTypes", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_PRODUCT_TYPES_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("TaxRates", "access") >= 0 %>'>
				<asp:Image SkinID="TaxRates" AlternateText='<%# L10n.Term("Administration.LBL_TAX_RATES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_TAX_RATES_TITLE") %>' NavigateUrl="~/Administration/TaxRates/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("TaxRates", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_TAX_RATES_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Discounts", "access") >= 0 %>'>
				<asp:Image SkinID="Discounts" AlternateText='<%# L10n.Term("Administration.LBL_DISCOUNTS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_DISCOUNTS_TITLE") %>' NavigateUrl="~/Administration/Discounts/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Discounts", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_DISCOUNTS_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Regions", "access") >= 0 %>'>
				<asp:Image SkinID="Regions" AlternateText='<%# L10n.Term("Administration.LBL_REGIONS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_REGIONS_TITLE") %>' NavigateUrl="~/Administration/Regions/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Regions", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_REGIONS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PaymentTypes", "access") >= 0 %>'>
				<asp:Image ID="imgPaymentTypes" SkinID="ProductTypes" AlternateText='<%# L10n.Term("Administration.LBL_PAYMENT_TYPES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkPaymentTypes" Text='<%# L10n.Term("Administration.LBL_PAYMENT_TYPES_TITLE") %>' NavigateUrl="~/Administration/PaymentTypes/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PaymentTypes", "access") >= 0 %>'>
				<asp:Label ID="lblPaymentTypes" Text='<%# L10n.Term("Administration.LBL_PAYMENT_TYPES_DESC") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PaymentTerms", "access") >= 0 %>'>
				<asp:Image ID="imgPaymentTerms" SkinID="ProductTypes" AlternateText='<%# L10n.Term("Administration.LBL_PAYMENT_TERMS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink ID="lnkPaymentTerms" Text='<%# L10n.Term("Administration.LBL_PAYMENT_TERMS_TITLE") %>' NavigateUrl="~/Administration/PaymentTerms/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PaymentTerms", "access") >= 0 %>'>
				<asp:Label ID="lblPaymentTerms" Text='<%# L10n.Term("Administration.LBL_PAYMENT_TERMS_DESC") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
