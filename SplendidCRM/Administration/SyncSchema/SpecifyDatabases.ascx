<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SpecifyDatabases.ascx.cs" Inherits="SplendidCRM.Administration.SyncSchema.SpecifyDatabases" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_PROVIDER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="lstSOURCE_PROVIDER" TabIndex="1" Runat="server">
								<asp:ListItem Value="System.Data.SqlClient"   >SQL Server</asp:ListItem>
								<asp:ListItem Value="Oracle.DataAccess.Client">Oracle</asp:ListItem>
								<asp:ListItem Value="IBM.Data.DB2"            >DB2</asp:ListItem>
								<asp:ListItem Value="MySql.Data"              >MySQL</asp:ListItem>
								<asp:ListItem Value="iAnywhere.Data.AsaClient">SQL Anywhere</asp:ListItem>
								<asp:ListItem Value="Sybase.Data.AseClient"   >Sybase ASE</asp:ListItem>
							</asp:DropDownList>
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_PROVIDER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="lstDESTINATION_PROVIDER" TabIndex="2" Runat="server">
								<asp:ListItem Value="System.Data.SqlClient"   >SQL Server</asp:ListItem>
								<asp:ListItem Value="Oracle.DataAccess.Client">Oracle</asp:ListItem>
								<asp:ListItem Value="IBM.Data.DB2"            >DB2</asp:ListItem>
								<asp:ListItem Value="MySql.Data"              >MySQL</asp:ListItem>
								<asp:ListItem Value="iAnywhere.Data.AsaClient">SQL Anywhere</asp:ListItem>
								<asp:ListItem Value="Sybase.Data.AseClient"   >Sybase ASE</asp:ListItem>
							</asp:DropDownList>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_CONNECTION") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="txtSOURCE_CONNECTION" TabIndex="1" TextMode="MultiLine" Rows="4" Columns="50" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_CONNECTION") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="txtDESTINATION_CONNECTION" TabIndex="2" TextMode="MultiLine" Rows="4" Columns="50" Runat="server" />
						</asp:TableCell>
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
</div>
<%
#if DEBUG
	XmlUtil.Dump(GetXml());
#endif
%>
