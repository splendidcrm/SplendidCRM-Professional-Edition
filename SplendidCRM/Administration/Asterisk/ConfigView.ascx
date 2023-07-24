<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.Asterisk.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Asterisk" Title="Asterisk.LBL_ASTERISK_SETTINGS" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_HOST_SERVER") %>' runat="server" />
				<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="HOST_SERVER" Size="30" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_HOST_PORT") %>' runat="server" />
				<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="HOST_PORT" Size="30" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_USER_NAME") %>' runat="server" />
				<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="USER_NAME" Size="30" Runat="server" />
				<asp:RequiredFieldValidator ID="reqUSER_NAME" ControlToValidate="USER_NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_PASSWORD") %>' runat="server" />
				<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="PASSWORD" Size="30" TextMode="Password" Runat="server" />
				<asp:RequiredFieldValidator ID="reqPASSWORD" ControlToValidate="PASSWORD" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_FROM_TRUNK") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="FROM_TRUNK" Size="30" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_FROM_CONTEXT") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:TextBox ID="FROM_CONTEXT" Size="30" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_LOG_MISSED_INCOMING_CALLS") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:CheckBox ID="LOG_MISSED_INCOMING_CALLS" CssClass="checkbox" runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_LOG_MISSED_OUTGOING_CALLS") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:CheckBox ID="LOG_MISSED_OUTGOING_CALLS" CssClass="checkbox" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_LOG_CALL_DETAILS") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:CheckBox ID="LOG_CALL_DETAILS" CssClass="checkbox" runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
				<asp:Label Text='<%# L10n.Term("Asterisk.LBL_ORIGINATE_EXTENSION_FIRST") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
				<asp:CheckBox ID="ORIGINATE_EXTENSION_FIRST" CssClass="checkbox" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>
