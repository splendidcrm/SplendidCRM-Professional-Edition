<%@ Control Language="c#" AutoEventWireup="false" Codebehind="VerifyFunctions.ascx.cs" Inherits="SplendidCRM.Administration.SyncSchema.VerifyFunctions" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Import Namespace="System.Xml" %>
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
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" EnablePrint="false" EnableHelp="true" Runat="Server" />
	<p></p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="WizardButtons" Src="WizardButtons.ascx" %>
	<SplendidCRM:WizardButtons ID="ctlWizardButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_PROVIDER") %></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblSOURCE_PROVIDER" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_PROVIDER") %></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblDESTINATION_PROVIDER" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_CONNECTION") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblSOURCE_CONNECTION" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_CONNECTION") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblDESTINATION_CONNECTION" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" ColumnSpan="2"><asp:Label ID="lblSourceError"      ForeColor="Red" EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" ColumnSpan="2"><asp:Label ID="lblDestinationError" ForeColor="Red" EnableViewState="False" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_UNIQUE_FUNCTIONS") %></asp:TableHeaderCell>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_UNIQUE_FUNCTIONS") %></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litSOURCE_UNIQUE"      Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litDESTINATION_UNIQUE" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_ALL_FUNCTIONS") %></asp:TableHeaderCell>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_ALL_FUNCTIONS") %></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litSOURCE_LIST"      Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litDESTINATION_LIST" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
</div>
<%
#if DEBUG
	XmlUtil.Dump(GetXml());
#endif
%>