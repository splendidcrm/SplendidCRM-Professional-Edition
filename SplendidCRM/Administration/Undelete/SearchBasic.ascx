<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchBasic.ascx.cs" Inherits="SplendidCRM.Administration.Undelete.SearchBasic" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divSearch">
	<asp:Table SkinID="tabSearchForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Wrap="false"><%= L10n.Term("Undelete.LBL_NAME"  ) %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="Top" Wrap="false"><asp:TextBox ID="txtNAME" CssClass="dataField" size="30" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top"><%= L10n.Term("Undelete.LBL_MODULE_NAME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="Top"><asp:DropDownList ID="lstMODULE_NAME" DataValueField="MODULE_NAME" DataTextField="MODULE_NAME" AutoPostBack="true" OnSelectedIndexChanged="lstMODULE_NAME_Changed" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Wrap="false"><%= L10n.Term("Undelete.LBL_AUDIT_DATE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="Top" Wrap="false" RowSpan="2">
							<asp:Table runat="server">
								<asp:TableRow>
									<asp:TableCell><%= L10n.Term("SavedSearch.LBL_SEARCH_AFTER" ) %></asp:TableCell>
									<asp:TableCell><SplendidCRM:DatePicker ID="ctlSTART_DATE" Runat="Server" /></asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell><%= L10n.Term("SavedSearch.LBL_SEARCH_BEFORE") %></asp:TableCell>
									<asp:TableCell><SplendidCRM:DatePicker ID="ctlEND_DATE"   Runat="Server" /></asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Wrap="false"><%= L10n.Term("Undelete.LBL_AUDIT_TOKEN") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="Top" Wrap="false"><asp:TextBox ID="txtAUDIT_TOKEN" CssClass="dataField" size="30" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top"><%= L10n.Term("Undelete.LBL_MODIFIED_BY") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="Top"><asp:DropDownList ID="lstUSERS" DataValueField="ID" DataTextField="USER_NAME" AutoPostBack="true" OnSelectedIndexChanged="lstUSERS_Changed" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
				<asp:Panel ID="pnlSearchButtons" CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
					<asp:Table ID="tblSearchButtons" Width="100%" CellPadding="0" CellSpacing="1" style="padding-top: 4px;" runat="server">
						<asp:TableRow>
							<asp:TableCell>
								<asp:Button ID="btnSearch"   CommandName="Search"   OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_SEARCH_BUTTON_LABEL"          ) %>' ToolTip='<%# L10n.Term(".LBL_SEARCH_BUTTON_TITLE"             ) %>' Runat="server" />&nbsp;
								<asp:Button ID="btnClear"    CommandName="Clear"    OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL"           ) %>' ToolTip='<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE"              ) %>' Runat="server" />&nbsp;
								<asp:Button ID="btnUndelete" CommandName="Undelete" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Undelete.LBL_UNDELETE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term("Undelete.LBL_UNDELETE_BUTTON_UNDELETE") %>' Runat="server" />&nbsp;
								<asp:CheckBox ID="chkBackground" Text='<%# L10n.Term("Undelete.LBL_BACKGROUND_OPERATION") %>' CssClass="checkbox" runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:Panel>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID , btnSearch.ClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtAUDIT_TOKEN.ClientID , btnSearch.ClientID) %>
</div>
