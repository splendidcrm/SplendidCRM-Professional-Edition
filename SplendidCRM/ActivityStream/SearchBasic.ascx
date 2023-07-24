<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchBasic.ascx.cs" Inherits="SplendidCRM.ActivityStream.SearchBasic" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
				<asp:Table SkinID="tabSearchView" Width="100%" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="10%" CssClass="dataLabel" style="padding: 8px;">
							<asp:ListBox ID="lstSTREAM_ACTION" Rows="3" SelectionMode="Multiple" DataTextField="DISPLAY_NAME" DataValueField="NAME" runat="server" />
						</asp:TableCell>
						<asp:TableCell ID="tdMODULES" Width="10%" CssClass="dataLabel" style="padding: 8px;">
							<asp:ListBox ID="lstMODULES" Rows="3" SelectionMode="Multiple" DataTextField="DISPLAY_NAME" DataValueField="MODULE_NAME" runat="server" />
						</asp:TableCell>
						<asp:TableCell ID="tdNAME" Width="75%" CssClass="dataLabel" style="padding: 8px;" runat="server">
							<asp:TextBox ID="txtNAME" CssClass="dataField" Width="97%" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="5%" style="padding: 8px;">
							<asp:Button ID="btnSearch" CommandName="Search" OnCommand="Page_Command" CssClass="EditHeaderOtherButton" Text='<%# L10n.Term(".LBL_SEARCH_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SEARCH_BUTTON_TITLE") %>' Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID, btnSearch.ClientID) %>
</div>
