<%@ Control CodeBehind="SystemView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.SystemView" %>
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
<div id="divSystemView" visible='<%# 
(  SplendidCRM.Security.AdminUserAccess("Config"        , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Currencies"    , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("SystemLog"     , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Administration", "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Import"        , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Schedulers"    , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("PaymentGateway", "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("Undelete"      , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("ZipCodes"      , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("NAICSCodes"    , "access") >= 0 
|| SplendidCRM.Security.AdminUserAccess("ModulesArchiveRules", "access") >= 0 
) %>' runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_ADMINISTRATION_HOME_TITLE" Runat="Server" />
	<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />

	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Wrap="false" Visible='<%# SplendidCRM.Security.AdminUserAccess("Config", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Config/default.aspx") %>'>
				<asp:Image SkinID="Administration" AlternateText='<%# L10n.Term("Administration.LBL_CONFIGURE_SETTINGS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_CONFIGURE_SETTINGS_TITLE") %>' NavigateUrl="~/Administration/Config/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
					<asp:LinkButton ID="btnShowSQL" Visible='<%# !Sql.ToBoolean(Application["CONFIG.show_sql"]) %>' Text='<%# L10n.Term("Administration.LBL_SHOW_SQL") %>' CommandName="System.ShowSQL"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
					<asp:LinkButton ID="btnHideSQL" Visible='<%#  Sql.ToBoolean(Application["CONFIG.show_sql"]) %>' Text='<%# L10n.Term("Administration.LBL_HIDE_SQL") %>' CommandName="System.HideSQL" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Config", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Config/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_CONFIGURE_SETTINGS") %>' runat="server" /><br />
				<%# bDebug ? L10n.Term("Administration.LBL_SHOW_SQL_DEBUG_BUILD") : (Sql.ToBoolean(Application["CONFIG.show_sql"]) ? L10n.Term("Administration.LBL_SHOW_SQL_ENABLED") : L10n.Term("Administration.LBL_SHOW_SQL_DISABLED")) %>
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Administration", "access") >= 0 %>'>
				<asp:Image SkinID="SystemCheck" AlternateText='<%# L10n.Term("Administration.LBL_SYSTEM_CHECK_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SYSTEM_CHECK_TITLE") %>' NavigateUrl="~/SystemCheck.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton Text="Reload"     CommandName="System.Reload" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:HyperLink  Text='Precompile' NavigateUrl="~/_devtools/Precompile.aspx" CssClass="tabDetailViewDL2Link" Target="PrecompileSplendidCRM" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Administration", "access") >= 0 %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_SYSTEM_CHECK") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Currencies", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Currencies/default.aspx") %>'>
				<asp:Image SkinID="Currencies" AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_CURRENCIES") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_CURRENCIES") %>' NavigateUrl="~/Administration/Currencies/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Currencies", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Currencies/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_CURRENCY") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("SystemLog", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/SystemLog/default.aspx") %>'>
				<asp:Image SkinID="SystemLog" AlternateText='<%# L10n.Term("Administration.LBL_SYSTEM_LOG_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SYSTEM_LOG_TITLE") %>' NavigateUrl="~/Administration/SystemLog/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("SystemLog", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/SystemLog/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_SYSTEM_LOG") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Import", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Import/default.aspx") %>'>
				<asp:Image SkinID="Import" AlternateText='<%# L10n.Term("Administration.LBL_IMPORT_DATABASE_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_IMPORT_DATABASE_TITLE") %>' NavigateUrl="~/Administration/Import/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Import", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Import/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_IMPORT_DATABASE") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Import", "export") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Export/default.aspx") %>'>
				<asp:Image SkinID="export" AlternateText='<%# L10n.Term("Administration.LBL_EXPORT_DATABASE_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_EXPORT_DATABASE_TITLE") %>' NavigateUrl="~/Administration/Export/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Import", "export") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Export/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_EXPORT_DATABASE") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Schedulers", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Schedulers/default.aspx") %>'>
				<asp:Image SkinID="Schedulers" AlternateText='<%# L10n.Term("Administration.LBL_SUGAR_SCHEDULER_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SUGAR_SCHEDULER_TITLE") %>' NavigateUrl="~/Administration/Schedulers/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Schedulers", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Schedulers/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_SUGAR_SCHEDULER") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/Backups/default.aspx") %>'>
				<asp:Image SkinID="Backups" AlternateText='<%# L10n.Term("Administration.LBL_BACKUPS_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_BACKUPS_TITLE") %>' NavigateUrl="~/Administration/Backups/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<div align="center">
				(
				<asp:LinkButton Text='<%# L10n.Term("Administration.LBL_PURGE_DEMO") %>' CommandName="System.PurgeDemo" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/Backups/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_BACKUPS") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PaymentGateway", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/PaymentGateway/default.aspx") %>'>
				<asp:Image SkinID="PaymentGateway" AlternateText='<%# L10n.Term("Administration.LBL_PAYMENT_GATEWAY_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_PAYMENT_GATEWAY_TITLE") %>' NavigateUrl="~/Administration/PaymentGateway/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("PaymentGateway", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/PaymentGateway/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_PAYMENT_GATEWAY") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/BusinessMode/default.aspx") %>'>
				<asp:Image SkinID="Schema" AlternateText='<%# L10n.Term("Administration.LBL_BUSINESS_MODE_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_BUSINESS_MODE_TITLE") %>' NavigateUrl="~/Administration/BusinessMode/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/BusinessMode/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_BUSINESS_MODE") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Config", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Configurator/default.aspx") %>'>
				<asp:Image SkinID="Administration" AlternateText='<%# L10n.Term("Administration.LBL_CONFIGURATOR_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_CONFIGURATOR_TITLE") %>' NavigateUrl="~/Administration/Configurator/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Config", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Configurator/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_CONFIGURATOR") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Undelete", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Undelete/default.aspx") %>'>
				<asp:Image SkinID="Undelete" AlternateText='<%# L10n.Term("Administration.LBL_UNDELETE_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_UNDELETE_TITLE") %>' NavigateUrl="~/Administration/Undelete/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("Undelete", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/Undelete/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_UNDELETE") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ZipCodes", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/ZipCodes/default.aspx") %>'>
				<asp:Image SkinID="Administration" AlternateText='<%# L10n.Term("Administration.LBL_ZIPCODES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_ZIPCODES_TITLE") %>' NavigateUrl="~/Administration/ZipCodes/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("ZipCodes", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/ZipCodes/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_ZIPCODES") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.AdminUserAccess("NAICSCodes", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/NAICSCodes/default.aspx") %>'>
				<asp:Image SkinID="Administration" AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_NAICS_CODES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_NAICS_CODES_TITLE") %>' NavigateUrl="~/Administration/NAICSCodes/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.AdminUserAccess("NAICSCodes", "access") >= 0 && Utils.CachedFileExists(Context, "~/Administration/NAICSCodes/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MANAGE_NAICS_CODES") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<%-- 02/18/2018 Paul.  ModulesArchiveRules module to Professional.  --%>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/ModulesArchiveRules/default.aspx") %>'>
				<asp:Image SkinID="Backups" AlternateText='<%# L10n.Term("Administration.LBL_MODULE_ARCHIVE_RULES_TITLE") %>' Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MODULE_ARCHIVE_RULES_TITLE") %>' NavigateUrl="~/Administration/ModulesArchiveRules/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<asp:Panel Visible='<%# Sql.IsEmptyString(Context.Application["ArchiveConnectionString"]) %>' runat="server">
					<br />
					<div align="center">
					(
					<asp:LinkButton Text="Rebuild Archive" CommandName="System.RebuildArchive"   OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
					)
					</div>
				</asp:Panel>
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/ModulesArchiveRules/default.aspx") %>'>
				<asp:Label Text='<%# L10n.Term("Administration.LBL_MODULE_ARCHIVE_RULES") %>' runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/ModulesArchiveRules/default.aspx") %>'></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2" Visible='<%# SplendidCRM.Security.IS_ADMIN && Utils.CachedFileExists(Context, "~/Administration/ModulesArchiveRules/default.aspx") %>'></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
