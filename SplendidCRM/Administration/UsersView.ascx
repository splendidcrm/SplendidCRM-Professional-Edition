<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.UsersView" %>
<%@ Import Namespace="SplendidCRM.Crm" %>
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
<div id="divUsersView" visible='<%# 
(  SplendidCRM.Security.AdminUserAccess("Users"      , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("ACLRoles"   , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("UserLogins" , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Teams"      , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("AuditEvents", "access") >= 0 
) %>' runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_USERS_TITLE" Runat="Server" />
	<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />

	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Users", "access") >= 0 %>'>
				<asp:Image SkinID="Users" AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_USERS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_USERS_TITLE") %>' NavigateUrl="~/Users/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton ID="btnUserRequired" Visible='<%# !Config.require_user_assignment() %>' Text='<%# L10n.Term("Users.LBL_REQUIRE"  ) %>'  CommandName="UserAssignement.Require"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnUserOptional" Visible='<%#  Config.require_user_assignment() %>' Text='<%# L10n.Term("Users.LBL_OPTIONAL") %>' CommandName="UserAssignement.Optional" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				<span id="spnDynamicAssignmentButtons" runat="server">
					&nbsp;
					<asp:LinkButton ID="btnAssignmentDynamic"  Visible='<%# !Config.enable_dynamic_assignment() %>' Text='<%# L10n.Term("Users.LBL_DYNAMIC" ) %>' CommandName="UserAssignement.Dynamic"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
					&nbsp;
					<asp:LinkButton ID="btnAssignmentSingular" Visible='<%#  Config.enable_dynamic_assignment() %>' Text='<%# L10n.Term("Users.LBL_SINGULAR") %>' CommandName="UserAssignement.Singular" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				</span>
				)
				</div>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Users", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MANAGE_USERS") %>' runat="server" /><br />
				<%# Config.require_user_assignment() ? L10n.Term("Users.LBL_USER_ASSIGNMENT_REQUIRED") : L10n.Term("Users.LBL_USER_ASSIGNMENT_OPTIONAL") %>
				<span id="spnDynamicAssignmentMessage" runat="server">
					<%# Config.enable_dynamic_assignment() ? L10n.Term("Users.LBL_ASSIGNMENT_DYNAMIC") : L10n.Term("Users.LBL_ASSIGNMENT_NOT_DYNAMIC") %>
				</span>
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ACLRoles", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/ACLRoles/default.aspx") %>'>
				<asp:Image SkinID="Roles" AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_ROLES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_ROLES_TITLE") %>' NavigateUrl="~/Administration/ACLRoles/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton ID="btnAdminDelegationEnable"  Visible='<%# !Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]) && (SplendidCRM.Security.AdminUserAccess("ACLRoles", "edit") >= 0) %>' Text='<%# L10n.Term("ACLRoles.LBL_ENABLE_ADMIN_DELEGATION" ) %>' CommandName="AdminDelegation.Enable"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnAdminDelegationDisable" Visible='<%#  Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]) && (SplendidCRM.Security.AdminUserAccess("ACLRoles", "edit") >= 0) %>' Text='<%# L10n.Term("ACLRoles.LBL_DISABLE_ADMIN_DELEGATION") %>' CommandName="AdminDelegation.Disable" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ACLRoles", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/ACLRoles/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MANAGE_ROLES") %>' runat="server" /><br />
				<%# Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]) ? L10n.Term("ACLRoles.LBL_ADMIN_DELEGATION_ENABLED") : L10n.Term("ACLRoles.LBL_ADMIN_DELEGATION_DISABLED") %>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("UserLogins", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/UserLogins/default.aspx") %>'>
				<asp:Image SkinID="UserLogins" AlternateText='<%# L10n.Term("Administration.LBL_USERS_LOGINS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_USERS_LOGINS_TITLE") %>' NavigateUrl="~/Administration/UserLogins/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("UserLogins", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/UserLogins/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_USERS_LOGINS") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Teams", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Teams/default.aspx") %>'>
				<asp:Image SkinID="Teams" AlternateText='<%# L10n.Term("Administration.LBL_TEAMS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_TEAMS_TITLE") %>' NavigateUrl="~/Administration/Teams/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton ID="btnTeamsEnable"   Visible='<%# !Config.enable_team_management()                                     %>' Text='<%# L10n.Term("Teams.LBL_ENABLE"  ) %>' CommandName="Teams.Enable"   OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnTeamsDisable"  Visible='<%#  Config.enable_team_management()                                     %>' Text='<%# L10n.Term("Teams.LBL_DISABLE" ) %>' CommandName="Teams.Disable"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnTeamsRequire"  Visible='<%# !Config.require_team_management() && Config.enable_team_management() %>' Text='<%# L10n.Term("Teams.LBL_REQUIRE" ) %>' CommandName="Teams.Require"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnTeamsOptional" Visible='<%#  Config.require_team_management() && Config.enable_team_management() %>' Text='<%# L10n.Term("Teams.LBL_OPTIONAL") %>' CommandName="Teams.Optional" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				
				<span id="spnDynamicTeamsButtons" runat="server">
					&nbsp;
					<asp:LinkButton ID="btnTeamsDynamic"  Visible='<%# !Config.enable_dynamic_teams()    && Config.enable_team_management() %>' Text='<%# L10n.Term("Teams.LBL_DYNAMIC" ) %>' CommandName="Teams.Dynamic"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
					&nbsp;
					<asp:LinkButton ID="btnTeamsSingular" Visible='<%#  Config.enable_dynamic_teams()    && Config.enable_team_management() %>' Text='<%# L10n.Term("Teams.LBL_SINGULAR") %>' CommandName="Teams.Singular" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				</span>
				&nbsp;
				<asp:LinkButton ID="btnTeamsHierarchical"    Visible='<%# !Config.enable_team_hierarchy() && Config.enable_team_management() %>' Text='<%# L10n.Term("Teams.LBL_HIERARCHICAL"    ) %>' CommandName="Teams.Hierarchical"    OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnTeamsNonHierarchical" Visible='<%#  Config.enable_team_hierarchy() && Config.enable_team_management() %>' Text='<%# L10n.Term("Teams.LBL_NON_HIERARCHICAL") %>' CommandName="Teams.NonHierarchical" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" style="white-space: nowrap" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Teams", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Teams/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_TEAMS_DESC") %>' runat="server" /><br />
				<%# Config.enable_team_management() ? L10n.Term("Teams.LBL_TEAMS_ENABLED") : L10n.Term("Teams.LBL_TEAMS_DISABLED") %>
				<%# Config.enable_team_management() && Config.require_team_management() ? L10n.Term("Teams.LBL_TEAMS_REQUIRED") : L10n.Term("Teams.LBL_TEAMS_NOT_REQUIRED") %>
				<span id="spnDynamicTeamsMessage" runat="server">
					<%# Config.enable_team_management() && Config.enable_dynamic_teams() ? L10n.Term("Teams.LBL_TEAMS_DYNAMIC") : L10n.Term("Teams.LBL_TEAMS_NOT_DYNAMIC") %>
				</span>
				<%# Config.enable_team_management() && Config.enable_team_hierarchy() ? L10n.Term("Teams.LBL_TEAMS_HIERARCHICAL") : L10n.Term("Teams.LBL_TEAMS_NON_HIERARCHICAL") %>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuditEvents", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/AuditEvents/default.aspx") %>'>
				<asp:Image SkinID="UserLogins" AlternateText='<%# L10n.Term("Administration.LBL_AUDIT_EVENTS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_AUDIT_EVENTS_TITLE") %>' NavigateUrl="~/Administration/AuditEvents/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("AuditEvents", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/AuditEvents/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_AUDIT_EVENTS") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Config", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/PasswordManager/default.aspx") %>'>
				<asp:Image SkinID="Administration" AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_PASSWORD_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_PASSWORD_TITLE") %>' NavigateUrl="~/Administration/PasswordManager/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Config", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/PasswordManager/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MANAGE_PASSWORD") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
