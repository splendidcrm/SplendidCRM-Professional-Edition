<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LastViewed.ascx.cs" Inherits="SplendidCRM._controls.LastViewed" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<div id="divLastViewed" width="100%" class="lastView">
	<b><%= L10n.Term(".LBL_LAST_VIEWED") %>:&nbsp;&nbsp;</b>
	<asp:Repeater id="ctlRepeater" runat="server">
		<HeaderTemplate />
		<ItemTemplate>
			<nobr>
				<asp:HyperLink NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "RELATIVE_PATH") + "view.aspx?ID=" + DataBinder.Eval(Container.DataItem, "ITEM_ID") %>' 
					ToolTip='<%# "[" + L10n.Term(".LBL_ALT_HOT_KEY") + "+" + DataBinder.Eval(Container.DataItem, "ROW_NUMBER") + "]" %>' AccessKey='<%# DataBinder.Eval(Container.DataItem, "ROW_NUMBER") %>' CssClass="lastViewLink" Runat="server">
					<SplendidCRM:DynamicImage ImageSkinID='<%# DataBinder.Eval(Container.DataItem, "IMAGE_NAME") %>' AlternateText='<%# HttpUtility.HtmlEncode(Sql.ToString(Eval("ITEM_SUMMARY"))) %>' runat="server" />
					&nbsp;<%# HttpUtility.HtmlEncode(Sql.ToString(Eval("ITEM_SUMMARY"))) %></asp:HyperLink>&nbsp;
			</nobr>
		</ItemTemplate>
		<FooterTemplate />
	</asp:Repeater>
	<div style="DISPLAY: <%= vwLastViewed != null && vwLastViewed.Count > 0 ? "NONE" : "INLINE" %>">
		<%= L10n.Term(".NTC_NO_ITEMS_DISPLAY") %>
	</div>
</div>

