<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TabMenu.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.TabMenu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="System.Data" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
						<div id="divTabMenu">
							<table class="tabFrame" cellspacing="0" cellpadding="0">
								<tr>
									<td style="padding-left:14px;" class="otherTabRight">&nbsp;</td>
<%
string sActiveTab = Sql.ToString(Page.Items["ActiveTabMenu"]);
int nRow = 0;
int nDisplayedTabs = 0;
int nMaxTabs = Sql.ToInteger(Session["max_tabs"]);
// 09/24/2007 Paul.  Max tabs is a config variable and needs the CONFIG in front of the name. 
if ( nMaxTabs == 0 )
	nMaxTabs = Sql.ToInteger(Application["CONFIG.default_max_tabs"]);
if ( nMaxTabs == 0 )
	nMaxTabs = 12;
for ( ; nRow < dtMenu.Rows.Count; nRow++ )
{
	DataRow row = dtMenu.Rows[nRow];
	string sMODULE_NAME   = Sql.ToString(row["MODULE_NAME"  ]);
	string sRELATIVE_PATH = Sql.ToString(row["RELATIVE_PATH"]);
	string sDISPLAY_NAME  = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
	string sTAB_CLASS     = (sMODULE_NAME == sActiveTab) ? "currentTab" : "otherTab";
	// 12/05/2006 Paul.  The TabMenu view does not filter the Calendar or activities tabs as they are virtual. 
	if ( SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "access") >= 0 )
	{
		if ( nDisplayedTabs < nMaxTabs || hovMore == null )
		{
			nDisplayedTabs++;
				%>
									<td valign="bottom">
										<table class="tabFrame" cellspacing="0" cellpadding="0" height="25">
											<tr>
												<td class="<%= sTAB_CLASS %>Left"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
												<td class="<%= sTAB_CLASS %>" nowrap><a class="<%= sTAB_CLASS %>Link"  href="<%= sRELATIVE_PATH.Replace("~/", sApplicationPath) %>"><%= sDISPLAY_NAME %></a></td>
												<td class="<%= sTAB_CLASS %>Right"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
											</tr>
										</table>
									</td>
				<%
		}
		else
		{
			HyperLink lnk = new HyperLink();
			lnk.Text        = sDISPLAY_NAME;
			lnk.NavigateUrl = sRELATIVE_PATH.Replace("~/", sApplicationPath);
			lnk.CssClass    = "menuItem";
			pnlTabMenuMore.Controls.Add(lnk);
		}
	}
}
// 01/05/2017 Paul.  Adding Feeds to the tab menu is a configuration option. 
if ( Sql.ToBoolean(Application["CONFIG.add_feeds_to_menu"]) )
{
	DataTable dtFeeds = SplendidCache.TabFeeds();
	foreach ( DataRow row in dtFeeds.Rows )
	{
		string sTITLE = Sql.ToString(row["TITLE"]);
		string sURL   = Sql.ToString(row["URL"  ]);
		HyperLink lnk = new HyperLink();
		lnk.Text        = sTITLE;
		lnk.NavigateUrl = sURL;
		lnk.CssClass    = "menuItem";
		lnk.Target      = "_blank";
		pnlTabMenuMore.Controls.Add(lnk);
	}
}
%>
									<td valign="bottom" style="DISPLAY: <%= (pnlTabMenuMore.Controls.Count > 0) ? "inline" : "none" %>">
										<table class="tabFrame" cellspacing="0" cellpadding="0">
											<tr>
												<td class="otherTabLeft"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
												<td class="otherTab"><asp:Image ID="imgTabMenuMore" SkinID="more" runat="server" /></td>
												<td class="otherTabRight"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
											</tr>
										</table>
									</td>

									<td width="100%" class="tabRow"><asp:Image SkinID="blank" Width="1" Height="1" runat="server" /></td>
								</tr>
							</table>
							<table class="tabFrame" cellspacing="0" cellpadding="0" height="20">
								<tr>
									<td id="subtabs"><asp:Image SkinID="blank" Width="1" Height="1" runat="server" /></td>
								</tr>
							</table>
						</div>
<asp:PlaceHolder ID="phHover" runat="server" />
<asp:Panel ID="pnlTabMenuMore" CssClass="menu" runat="server" />

