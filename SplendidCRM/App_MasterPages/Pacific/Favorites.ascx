<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Favorites.ascx.cs" Inherits="SplendidCRM.SplendidControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<div id="divFavorites" width="100%" class="lastView">
	<h1><asp:Label Text='<%# L10n.Term(".LBL_FAVORITES") %>' Visible='<%# SplendidCache.Favorites(Sql.ToString(Page.Items["ActiveTabMenu"])).Count > 0 %>' runat="server" /></h1>
	<asp:Repeater id="ctlRepeater" DataSource='<%# SplendidCache.Favorites(Sql.ToString(Page.Items["ActiveTabMenu"])) %>' runat="server">
		<HeaderTemplate />
		<ItemTemplate>
			<div class="lastViewRecentViewed" onclick="window.location.href='<%# Sql.ToString(Eval("RELATIVE_PATH")).Replace("~/", Sql.ToString(Application["rootURL"])) + "view.aspx?ID=" + Eval("ITEM_ID") %>'" style="cursor: pointer;">
				<asp:HyperLink NavigateUrl='<%# Eval("RELATIVE_PATH") + "view.aspx?ID=" + Eval("ITEM_ID") %>' ToolTip='<%# Eval("ITEM_SUMMARY") %>' CssClass="lastViewLink" Runat="server">
					<%# HttpUtility.HtmlEncode(Sql.ToString(Eval("ITEM_SUMMARY"))) %>
				</asp:HyperLink>
			</div>
		</ItemTemplate>
		<FooterTemplate />
	</asp:Repeater>
</div>

