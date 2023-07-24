<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Contacts.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Contacts" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />

	<asp:HiddenField ID="LAYOUT_EDIT_VIEW" Runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%-- 03/19/2020 Paul.  Move header to layout. --%>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Button ID="ACCOUNT_UPDATE" Text="ACCOUNT_UPDATE" style="display: none" runat="server" />
	<asp:Panel visible='<%# Sql.ToBoolean(Application["CONFIG.portal_on"]) %>' runat="server">
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table ID="tblPortal" class="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Contacts.LBL_PORTAL_INFORMATION") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Contacts.LBL_PORTAL_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="PORTAL_NAME" size="32" MaxLength="255" runat="server" />
							&nbsp;<asp:RequiredFieldValidator ID="PORTAL_NAME_REQUIRED" ControlToValidate="PORTAL_NAME" CssClass="required" Display="Dynamic" Enabled="false" EnableClientScript="false" EnableViewState="false" runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Contacts.LBL_PORTAL_ACTIVE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top"><asp:CheckBox ID="PORTAL_ACTIVE" CssClass="checkbox" runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Contacts.LBL_PORTAL_PASSWORD") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="PORTAL_PASSWORD" size="32" MaxLength="32" TextMode="Password" runat="server" />
							&nbsp;<asp:RequiredFieldValidator ID="PORTAL_PASSWORD_REQUIRED" ControlToValidate="PORTAL_PASSWORD" CssClass="required" Display="Dynamic" Enabled="false" EnableClientScript="false" EnableViewState="false" runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Contacts.LBL_CONFIRM_PORTAL_PASSWORD") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="PORTAL_PASSWORD_CONFIRM" size="32" MaxLength="32" TextMode="Password" runat="server" />
							&nbsp;<asp:RequiredFieldValidator ID="PORTAL_PASSWORD_CONFIRM_REQUIRED" ControlToValidate="PORTAL_PASSWORD_CONFIRM" CssClass="required" Display="Dynamic" Enabled="false" EnableClientScript="false" EnableViewState="false" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</asp:Panel>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />

	<div id="divEditSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

