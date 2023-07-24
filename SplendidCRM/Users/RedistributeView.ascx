<%@ Control Language="c#" AutoEventWireup="false" Codebehind="RedistributeView.ascx.cs" Inherits="SplendidCRM.Users.RedistributeView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="Users.LBL_REDISTRIBUTE_TITLE" Runat="Server" />
	<p></p>
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" /><br />
			<asp:Panel ID="pnlSelectUser" runat="server">
				<asp:Label Text='<%# L10n.Term("Users.LBL_REDISTRIBUTE_DESCRIPTION") %>' Runat="server" />
				<br />
				<br />
				<asp:Table SkinID="tabFrame" runat="server">
					<asp:TableRow>
						<asp:TableCell>
							<asp:Label Text='<%# L10n.Term("Users.LBL_REDISTRIBUTE_SELECT_USERS") %>' Runat="server" /><br />
							<asp:Table SkinID="tabEditView" CellPadding="6" runat="server">
								<asp:TableRow>
									<asp:TableCell>
										<asp:Label Text='<%# L10n.Term("Users.LBL_REASS_USER_FROM") %>' Runat="server" /><br />
										<asp:DropDownList ID="lstFROM"    DataValueField="ID"          DataTextField="USER_NAME"    runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell>
										<asp:Label Text='<%# L10n.Term("Users.LBL_REASS_USER_TO") %>' Runat="server" /><br />
										<asp:ListBox ID="lstTO"      DataValueField="ID"          DataTextField="USER_NAME"    Rows="6" SelectionMode="Multiple" runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow ID="trTeams" Visible="false">
									<asp:TableCell>
										<asp:Label Text='<%# L10n.Term("Users.LBL_REASS_USER_TEAM") %>' Runat="server" /><br />
										<asp:DropDownList ID="lstTEAM"    DataValueField="ID"          DataTextField="NAME"         runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell>
										<asp:Label Text='<%# L10n.Term("Users.LBL_REASS_MOD_REASSIGN") %>' Runat="server" /><br />
										<asp:DropDownList ID="lstMODULES" DataValueField="MODULE_NAME" DataTextField="MODULE_NAME" OnSelectedIndexChanged="lstMODULES_Changed" AutoPostBack="true" runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow ID="trReassignWorkflows" Visible="false">
									<asp:TableCell VerticalAlign="Top">
										<asp:CheckBox ID="chkREASSIGN_WORKFLOW" CssClass="checkbox" runat="server" />&nbsp;
										<asp:Label Text='<%# L10n.Term("Users.LBL_REASS_WORK_NOTIF_AUDIT") %>' runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<br />
							<asp:Button CommandName="Submit" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Users.LBL_REASS_BUTTON_SUBMIT") %>' Runat="server" />&nbsp;
							<asp:Button CommandName="Clear"  OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Users.LBL_REASS_BUTTON_CLEAR" ) %>' Runat="server" /><br />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:Panel>

			<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
			<asp:Panel ID="pnlFilters" runat="server">
				<asp:Table SkinID="tabFrame" runat="server">
					<asp:TableRow>
						<asp:TableCell>
							<asp:Panel ID="pnlAccountFilters" Visible="false" runat="server">
								<asp:Label Text='<%# String.Format(L10n.Term("Users.LBL_REASS_FILTERS"), L10n.Term(".moduleList.", "Accounts")) %>' Font-Bold="true" Runat="server" /><br />
								<SplendidCRM:SearchView ID="ctlAccountSearch" Module="Accounts" SearchMode="Reassign" ShowSearchTabs="false" ShowSearchButtons="false" ShowSearchViews="false" ShowDuplicateSearch="false" Runat="Server" />
							</asp:Panel>
							<asp:Panel ID="pnlBugFilters" Visible="false" runat="server">
								<asp:Label Text='<%# String.Format(L10n.Term("Users.LBL_REASS_FILTERS"), L10n.Term(".moduleList.", "Bugs")) %>' Font-Bold="true" Runat="server" /><br />
								<SplendidCRM:SearchView ID="ctlBugSearch" Module="Bugs" SearchMode="Reassign" ShowSearchTabs="false" ShowSearchButtons="false" ShowSearchViews="false" ShowDuplicateSearch="false" Runat="Server" />
							</asp:Panel>
							<asp:Panel ID="pnlCallFilters" Visible="false" runat="server">
								<asp:Label Text='<%# String.Format(L10n.Term("Users.LBL_REASS_FILTERS"), L10n.Term(".moduleList.", "Calls")) %>' Font-Bold="true" Runat="server" /><br />
								<SplendidCRM:SearchView ID="ctlCallSearch" Module="Calls" SearchMode="Reassign" ShowSearchTabs="false" ShowSearchButtons="false" ShowSearchViews="false" ShowDuplicateSearch="false" Runat="Server" />
							</asp:Panel>
							<asp:Panel ID="pnlCaseFilters" Visible="false" runat="server">
								<asp:Label Text='<%# String.Format(L10n.Term("Users.LBL_REASS_FILTERS"), L10n.Term(".moduleList.", "Cases")) %>' Font-Bold="true" Runat="server" /><br />
								<SplendidCRM:SearchView ID="ctlCaseSearch" Module="Cases" SearchMode="Reassign" ShowSearchTabs="false" ShowSearchButtons="false" ShowSearchViews="false" ShowDuplicateSearch="false" Runat="Server" />
							</asp:Panel>
							<asp:Panel ID="pnlOpportunityFilters" Visible="false" runat="server">
								<asp:Label Text='<%# String.Format(L10n.Term("Users.LBL_REASS_FILTERS"), L10n.Term(".moduleList.", "Opportunities")) %>' Font-Bold="true" Runat="server" /><br />
								<SplendidCRM:SearchView ID="ctlOpportunitySearch" Module="Opportunities" SearchMode="Reassign" ShowSearchTabs="false" ShowSearchButtons="false" ShowSearchViews="false" ShowDuplicateSearch="false" Runat="Server" />
							</asp:Panel>
							<asp:Panel ID="pnlTaskFilters" Visible="false" runat="server">
								<asp:Label Text='<%# String.Format(L10n.Term("Users.LBL_REASS_FILTERS"), L10n.Term(".moduleList.", "Tasks")) %>' Font-Bold="true" Runat="server" /><br />
								<SplendidCRM:SearchView ID="ctlTaskSearch" Module="Tasks" SearchMode="Reassign" ShowSearchTabs="false" ShowSearchButtons="false" ShowSearchViews="false" ShowDuplicateSearch="false" Runat="Server" />
							</asp:Panel>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:Panel>

			<asp:Panel ID="pnlReassignment" runat="server">
				<asp:Label Text='<%# L10n.Term("Users.LBL_REASS_NOTES_TITLE") %>' Runat="server" /><br />
				<asp:Table SkinID="tabFrame" runat="server">
					<asp:TableRow>
						<asp:TableCell>
							<asp:PlaceHolder ID="plcPreview" runat="server" />
							<br />
							<asp:Button CommandName="Continue" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Users.LBL_REASS_BUTTON_CONTINUE") %>' Runat="server" />&nbsp;
							<asp:Button CommandName="Go Back"  OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Users.LBL_REASS_BUTTON_GO_BACK" ) %>' Runat="server" />&nbsp;
							<asp:Button CommandName="Restart"  OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Users.LBL_REASS_BUTTON_RESTART" ) %>' Runat="server" /><br />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:Panel>

			<asp:Panel ID="pnlResults" runat="server">
				<asp:Table SkinID="tabFrame" runat="server">
					<asp:TableRow>
						<asp:TableCell>
							<asp:PlaceHolder ID="plcResults" runat="server" />
							<br />
							<asp:Button CommandName="Return"  OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Users.LBL_REASS_BUTTON_RETURN" ) %>' Runat="server" /><br />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:Panel>
		</ContentTemplate>
	</asp:UpdatePanel>
	<p></p>
</div>
