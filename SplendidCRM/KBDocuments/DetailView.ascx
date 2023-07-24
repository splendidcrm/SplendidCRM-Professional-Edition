<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.KBDocuments.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divDetailView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="KBDocuments" EnablePrint="true" HelpName="DetailView" EnableHelp="true" EnableFavorites="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailNavigation" Src="~/_controls/DetailNavigation.ascx" %>
	<SplendidCRM:DetailNavigation ID="ctlDetailNavigation" Module="<%# m_sMODULE %>" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:HiddenField ID="LAYOUT_DETAIL_VIEW" Runat="server" />
	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>
	<table ID="tblAttachments" class="tabDetailView" runat="server">
		<tr>
			<td width="15%" class="tabDetailViewDL" valign="top">
				<%= L10n.Term("KBDocuments.LBL_ATTACHMENTS") %>
			</td>
			<td class="tabDetailViewDF" valign="top">
				<asp:Repeater id="ctlAttachments" runat="server">
					<HeaderTemplate />
					<ItemTemplate>
						<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "FILENAME") %>' NavigateUrl='<%# "~/KBDocuments/Attachment.aspx?ID=" + DataBinder.Eval(Container.DataItem, "ID") %>' Target="_blank" Runat="server" /><br />
					</ItemTemplate>
					<FooterTemplate />
				</asp:Repeater>
			</td>
		</tr>
		<tr>
			<td width="15%" class="tabDetailViewDL" valign="top">
				<%= L10n.Term("KBDocuments.LBL_IMAGES") %>
			</td>
			<td class="tabDetailViewDF" valign="top">
				<asp:Repeater id="ctlImages" runat="server">
					<HeaderTemplate />
					<ItemTemplate>
						<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "FILENAME") %>' NavigateUrl='<%# "~/KBDocuments/Image.aspx?ID=" + DataBinder.Eval(Container.DataItem, "ID") %>' Target="_blank" Runat="server" /><br />
					</ItemTemplate>
					<FooterTemplate />
				</asp:Repeater>
			</td>
		</tr>
	</table>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
