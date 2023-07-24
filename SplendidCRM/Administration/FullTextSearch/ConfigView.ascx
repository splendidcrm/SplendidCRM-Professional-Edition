<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.FullTextSearch.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="FullTextSearch" Title="FullTextSearch.LBL_FULLTEXTSEARCH_SETTINGS" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_FULLTEXT_SUPPORTED") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="FULLTEXT_SUPPORTED" Text='<%# L10n.Term(".LBL_NO") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell ColumnSpan="2" CssClass="dataField" VerticalAlign="Middle">
				<asp:Label ID="SUPPORTED_INSTRUCTIONS" Text='<%# L10n.Term("FullTextSearch.LBL_SUPPORTED_INSTRUCTIONS") %>' runat="server" /><br />
				<asp:Label ID="SQL_SERVER_VERSION" runat="server" /><br />
				<asp:Label ID="SQL_SERVER_EDITION" runat="server" /><br />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_FULLTEXT_INSTALLED") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="FULLTEXT_INSTALLED" Text='<%# L10n.Term(".LBL_NO") %>' Runat="server" />
			</asp:TableCell>
			<asp:TableCell ColumnSpan="2" CssClass="dataField" VerticalAlign="Middle">
				<asp:Label ID="INSTALLED_INSTRUCTIONS" Text='<%# L10n.Term("FullTextSearch.LBL_INSTALLED_INSTRUCTIONS") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_OFFICE_SUPPORTED") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="OFFICE_SUPPORTED" Text='<%# L10n.Term(".LBL_NO") %>' Runat="server" />
			</asp:TableCell>
			<asp:TableCell ColumnSpan="2" CssClass="dataField" VerticalAlign="Middle">
				<asp:Label ID="OFFICE_INSTRUCTIONS" Text='<%# L10n.Term("FullTextSearch.LBL_OFFICE_INSTRUCTIONS") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_PDF_SUPPORTED") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="PDF_SUPPORTED" Text='<%# L10n.Term(".LBL_NO") %>' Runat="server" />
			</asp:TableCell>
			<asp:TableCell ColumnSpan="2" CssClass="dataField" VerticalAlign="Middle">
				<asp:Label ID="PDF_INSTRUCTIONS" Text='<%# L10n.Term("FullTextSearch.LBL_PDF_INSTRUCTIONS") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_FULLTEXT_CATALOG_EXISTS") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="FULLTEXT_CATALOG_EXISTS" Text='<%# L10n.Term(".LBL_NO") %>' Runat="server" />
			</asp:TableCell>
			<asp:TableCell ColumnSpan="2" CssClass="dataField" VerticalAlign="Middle">
				<asp:Label ID="CATALOG_INSTRUCTIONS" Text='<%# L10n.Term("FullTextSearch.LBL_CATALOG_INSTRUCTIONS") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_SUPPORTED_DOCUMENT_TYPES") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:TextBox ID="DOCUMENT_TYPES" TextMode="MultiLine" Rows="6" ReadOnly="true" Columns="10" runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_INDEXED_TABLES") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:TextBox ID="INDEXED_TABLES" TextMode="MultiLine" Rows="6" ReadOnly="true" Columns="30" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_POPULATION_STATUS") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="POPULATION_STATUS" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_POPULATION_COUNT") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="POPULATION_COUNT" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("FullTextSearch.LBL_LAST_POPULATION_DATE") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label ID="LAST_POPULATION_DATE" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="dataLabel" VerticalAlign="top">
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="dataField" VerticalAlign="top">
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
</div>
