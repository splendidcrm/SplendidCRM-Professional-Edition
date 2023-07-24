<%@ Control CodeBehind="ActivityStream.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Opportunities.ActivityStream" %>
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
<%@ Register TagPrefix="SplendidCRM" Tagname="SubPanelButtons" Src="~/_controls/SubPanelButtons.ascx" %>
<SplendidCRM:SubPanelButtons ID="ctlDynamicButtons" Module="ActivityStream" SubPanel="divOpportunitiesActivityStream" Title=".LBL_ACTIVITY_STREAM" Runat="Server" />

<div id="divOpportunitiesActivityStream" style='<%= "display:" + (CookieValue("divOpportunitiesActivityStream") != "1" ? "inline" : "none") %>'>
	<asp:Panel ID="pnlNewRecordInline" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="~/ActivityStream/NewRecord.ascx" %>
		<SplendidCRM:NewRecord ID="ctlNewRecord" Width="100%" ShowCancel="true" ShowHeader="false" Runat="Server" />
	</asp:Panel>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/ActivityStream/SearchBasic.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Visible="false" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" ShowHeader="false" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="100%" ItemStyle-HorizontalAlign="Left">
				<ItemTemplate>
					<table cellpadding="2" cellspacing="0" border="0" width="100%">
						<tr>
							<td width="50px">
								<div class="ActivityStreamPicture" >
									<%-- 01/17/2018 Paul.  Use CREATED_BY_ID to determine of person created the event. --%>
									<asp:Image CssClass="ActivityStreamPicture" SkinID="ActivityStreamUser"                                Visible='<%# !Sql.IsEmptyGuid(Eval("CREATED_BY_ID")) &&  Sql.IsEmptyString(Eval("CREATED_BY_PICTURE")) %>' runat="server" />
									<asp:Image CssClass="ActivityStreamPicture" src='<%# Eval("CREATED_BY_PICTURE") %>'                    Visible='<%# !Sql.IsEmptyGuid(Eval("CREATED_BY_ID")) && !Sql.IsEmptyString(Eval("CREATED_BY_PICTURE")) %>' runat="server" />
									<asp:Panel CssClass='<%# "ModuleHeaderModule ModuleHeaderModule" + m_sMODULE + " ListHeaderModule" %>' Visible='<%#  Sql.IsEmptyGuid(Eval("CREATED_BY_ID")) %>' runat="server"><%# L10n.Term(m_sMODULE + ".LBL_MODULE_ABBREVIATION") %></asp:Panel>
								</div>
							</td>
							<td>
								<div class="ActivityStreamDescription"><%# SplendidCRM.ActivityStream.StreamView.StreamFormatDescription(m_sMODULE, L10n, T10n, Container.DataItem) %></div>
								<div class="ActivityStreamIdentity">
									<span class="ActivityStreamCreatedBy"><%# Eval("CREATED_BY") %></span>
									<span class="ActivityStreamDateEntered"><%# Eval("STREAM_DATE") %></span>
								</div>
							</td>
							<td width="20px">
								<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0 && (Sql.ToString(Eval("STREAM_ACTION")) != "Deleted") %>' CommandName="Preview" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_PREVIEW") %>' SkinID="preview_inline" Runat="server" />
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

