<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchBasic.ascx.cs" Inherits="SplendidCRM.Administration.Terminology.SearchBasic" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader Title="Terminology.LBL_SEARCH_FORM_TITLE" Runat="Server" />
<div id="divSearch">
	<asp:Table SkinID="tabSearchForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_NAME"        ) %></asp:TableCell>
						<asp:TableCell Width="25%" CssClass="dataField"><asp:TextBox ID="txtNAME"         TabIndex="1" Size="25" MaxLength="50" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_LANG"       ) %></asp:TableCell>
						<asp:TableCell Width="25%" CssClass="dataField"><asp:DropDownList ID="lstLANGUAGE" TabIndex="2" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_MODULE_NAME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstMODULE_NAME" TabIndex="4" DataValueField="MODULE_NAME" DataTextField="MODULE_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:CheckBox ID="chkGLOBAL_TERMS" TabIndex="5" OnCheckedChanged="chkGLOBAL_TERMS_CheckedChanged" AutoPostBack="True" CssClass="checkbox" Runat="server" />&nbsp;<%= L10n.Term("Terminology.LBL_GLOBAL_TERMS") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_LIST_NAME_LABEL") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstLIST_NAME" TabIndex="6" DataValueField="LIST_NAME" DataTextField="LIST_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:CheckBox ID="chkINCLUDE_LISTS" TabIndex="7" OnCheckedChanged="chkINCLUDE_LISTS_CheckedChanged" AutoPostBack="True" CssClass="checkbox" Runat="server" />&nbsp;<%= L10n.Term("Terminology.LBL_INCLUDE_LISTS") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_DISPLAY_NAME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3"><asp:TextBox ID="txtDISPLAY_NAME" TabIndex="3" TextMode="MultiLine" Columns="90" Rows="2" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
				<%@ Register TagPrefix="SplendidCRM" Tagname="SearchButtons" Src="~/_controls/SearchButtons.ascx" %>
				<SplendidCRM:SearchButtons ID="ctlSearchButtons" Visible="<%# !PrintView %>" Runat="Server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID        , ctlSearchButtons.SearchClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtDISPLAY_NAME.ClientID, ctlSearchButtons.SearchClientID) %>
</div>

