<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SixToolbar.ascx.cs" Inherits="SplendidCRM.Themes.Atlantic.SixToolbar" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<div id="divSixToolbar">
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<table cellspacing="0" cellpadding="0" border="0" class="SixToolbar">
				<tr>
					<td width="0px">
						<script runat="server">
						// 05/08/2010 Paul.  We need an image button at the top be a sink for the ENTER key.
						// This will prevent the first toolbar button from getting selected inadvertantly. 
						</script>
						<asp:ImageButton SkinID="blank" Width="0" Height="0" OnClientClick="return false;" runat="server" />
					</td>
					<td nowrap>
						<%@ Register TagPrefix="SplendidCRM" Tagname="TabMenu" Src="TabMenu.ascx" %>
						<SplendidCRM:TabMenu ID="ctlTabMenu" Visible='<%# !PrintView %>' Runat="Server" />
					</td>
					<td align="right" valign="middle">
						<asp:Panel ID="cntUnifiedSearch" runat="server">
							<div id="divUnifiedSearch">
								<script type="text/javascript">
								function UnifiedSearch()
								{
									var frm = document.forms[0];
									// 01/21/2014 Paul.  Need to escape the query value to allow for symbols in the query. 
									var sUrl = '<%= Application["rootURL"] %>Home/UnifiedSearch.aspx?txtUnifiedSearch=' + escape(frm['<%= txtUnifiedSearch.ClientID %>'].value);
									window.location.href = sUrl;
									return false;
								}
								</script>
								<nobr>
								&nbsp;<asp:TextBox ID="txtUnifiedSearch" CssClass="searchField" Text='<%# Request["txtUnifiedSearch"] %>' runat="server" />
								<asp:ImageButton ID="btnUnifiedSearch" SkinID="searchButton" AlternateText='<%# L10n.Term(".LBL_SEARCH") %>' OnClientClick="return UnifiedSearch();" CssClass="searchButton" runat="server" />
								&nbsp;
								</nobr>
							</div>
						</asp:Panel>
					</td>
					<td width="100%" class="tabRow"><asp:Image SkinID="blank" Width="1" Height="1" runat="server" /></td>
				</tr>
			</table>
			<table cellspacing="0" cellpadding="0" border="0" class="<%# L10n.IsLanguageRTL() ? "SixToolbarUserRTL" : "SixToolbarUser" %>">
				<tr>
					<td valign="bottom" style="border-left: 1px solid #003564">
						<table id="tabToolbarUser" class="tabToolbarFrame" cellspacing="0" cellpadding="0" height="100%" runat="server">
							<tr>
								<td class="otherUser" nowrap="1">
									<span class="otherTabLink" visible="<%# SplendidCRM.Security.IsImpersonating() %>" runat="server"><%# L10n.Term("Users.LBL_IMPERSONATING") %><br /></span>
									<span class="otherTabLink" style="padding-right:6px;"><%# SplendidCRM.Security.FULL_NAME %></span>
									<asp:Image SkinID="more" class="otherTabMoreArrow" runat="server" /><br />
									<asp:HyperLink NavigateUrl="javascript:void(0);" valign="bottom" runat="server">
										<asp:Image SkinID="blank" Width="100%" Height="4" BorderWidth="0" runat="server" />
									</asp:HyperLink>
								</td>
							</tr>
						</table>
					</td>
					<td valign="bottom" style="border-left: 1px solid #003564" width="32">
						<table id="tabToolbarQuickCreate" class="tabToolbarFrame" cellspacing="0" cellpadding="0" height="100%" runat="server">
							<tr>
								<td class="otherQuickCreate">
									<asp:Image SkinID="ToolbarQuickCreate" class="otherTabMoreArrow" runat="server" /><br />
									<asp:HyperLink NavigateUrl="javascript:void(0);" valign="bottom" runat="server">
										<asp:Image SkinID="blank" Width="100%" Height="4" BorderWidth="0" runat="server" />
									</asp:HyperLink>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<div style="height: 43px; width: 100%"></div>
			<!-- 05/18/2013 Paul.  Moving the hidden panels outside the table solves a Chrome problem with z-index. -->
			<asp:Panel ID="pnlToolbarUserHover" CssClass="PanelHoverHidden" runat="server">
				<table cellpadding="0" cellspacing="0" class="MoreActionsInnerTable">
					<tr>
						<td class="MoreActionsInnerCell">
							<asp:HyperLink  ID="lnkMyAccount" Text='<%# L10n.Term(".LBL_MY_ACCOUNT") %>' NavigateUrl="~/Users/MyAccount.aspx"                CssClass="ModuleActionsMenuItems" Runat="server" />
							<asp:HyperLink  ID="lnkEmployees" Text='<%# L10n.Term(".LBL_EMPLOYEES" ) %>' NavigateUrl="~/Employees/default.aspx"              CssClass="ModuleActionsMenuItems" Visible='<%# !PortalCache.IsPortal() && SplendidCRM.Security.GetUserAccess("Employees", "access") >= 0                              %>' Runat="server" />
							<asp:HyperLink  ID="lnkAdmin"     Text='<%# L10n.Term(".LBL_ADMIN"     ) %>' NavigateUrl="~/Administration/default.aspx"         CssClass="ModuleActionsMenuItems" Visible='<%# !PortalCache.IsPortal() && SplendidCRM.Security.IS_ADMIN || SplendidCRM.Security.IS_ADMIN_DELEGATE                     %>' Runat="server" />
							<asp:HyperLink  ID="lnkTraining"  Text='<%# L10n.Term(".LBL_TRAINING"  ) %>' NavigateUrl="~/Home/TrainingPortal.aspx"            CssClass="ModuleActionsMenuItems" Visible='<%# !PortalCache.IsPortal() && !Sql.ToBoolean(Application["CONFIG.hide_training"])                                         %>' Runat="server" />
							<asp:HyperLink  ID="lnkAbout"     Text='<%# L10n.Term(".LNK_ABOUT"     ) %>' NavigateUrl="~/Home/About.aspx"                     CssClass="ModuleActionsMenuItems" Runat="server" />
							<asp:HyperLink  ID="lnkLogout"    Text='<%# L10n.Term(".LBL_LOGOUT"    ) %>' NavigateUrl="~/Users/Logout.aspx"                   CssClass="ModuleActionsMenuItems" Visible='<%# (!SplendidCRM.Security.IsWindowsAuthentication() || SplendidCRM.Security.IsImpersonating()) && SplendidCRM.Security.IsAuthenticated() %>' Runat="server" />
							<asp:LinkButton ID="lnkReload"    Text='<%# L10n.Term(".LBL_RELOAD"    ) %>' CommandName="Admin.Reload" OnCommand="Page_Command" CssClass="ModuleActionsMenuItems" Visible='<%# !PortalCache.IsPortal() && bDebug && (SplendidCRM.Security.IS_ADMIN || SplendidCRM.Security.IS_ADMIN_DELEGATE) %>' Runat="server" />
						</td>
					</tr>
				</table>
			</asp:Panel>
			<ajaxToolkit:HoverMenuExtender TargetControlID="tabToolbarUser" PopupControlID="pnlToolbarUserHover" PopupPosition="Bottom" PopDelay="250" HoverDelay="500" runat="server" />
			<asp:Panel ID="pnlToolbarQuickCreateHover" CssClass="PanelHoverHidden" runat="server">
				<table cellpadding="0" cellspacing="0" class="MoreActionsInnerTable">
					<tr>
						<td class="MoreActionsInnerCell">
							<asp:PlaceHolder ID="plcSubPanel" runat="server" />
							<asp:HiddenField ID="hidDynamicNewRecord" Value="" runat="server" />
						</td>
					</tr>
				</table>
			</asp:Panel>
			<ajaxToolkit:HoverMenuExtender TargetControlID="tabToolbarQuickCreate" PopupControlID="pnlToolbarQuickCreateHover" PopupPosition="Bottom" PopDelay="250" HoverDelay="500" OffsetX="<%#  L10n.IsLanguageRTL() ? 0 : -160 %>" runat="server" />
			<asp:PlaceHolder ID="plcDynamicNewRecords" runat="server" />
		</ContentTemplate>
	</asp:UpdatePanel>
</div>

