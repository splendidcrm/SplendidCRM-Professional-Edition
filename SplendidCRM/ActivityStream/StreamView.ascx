<%@ Control CodeBehind="StreamView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.ActivityStream.StreamView" %>
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
<div id="divListView">
	<div style="DISPLAY: <%= bShowHeader ? "inline" : "none" %>">
		<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
		<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Title=".LBL_ACTIVITY_STREAM" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	</div>

	<div ID="my_activitystream_edit" style="DISPLAY: <%= bShowSearchDialog ? "inline" : "none" %>">
		<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
		<SplendidCRM:SearchBasic ID="ctlSearchBasic" Visible="<%# !PrintView %>" Runat="Server" />
	</div>

	<asp:Panel ID="pnlNewRecordInline" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="NewRecord.ascx" %>
		<SplendidCRM:NewRecord ID="ctlNewRecord" Visible="<%# !PrintView %>" Runat="Server" />
	</asp:Panel>

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<asp:HiddenField ID="LAYOUT_LIST_VIEW" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="<%# !PrintView %>" EnableViewState="true" ShowHeader="false" runat="server">
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
									<asp:Panel CssClass='<%# "ModuleHeaderModule ModuleHeaderModule" + Sql.ToString(Eval("MODULE_NAME")) + " ListHeaderModule" %>' Visible='<%# Sql.IsEmptyGuid(Eval("CREATED_BY_ID")) %>' runat="server"><%# L10n.Term(Sql.ToString(Eval("MODULE_NAME")) + ".LBL_MODULE_ABBREVIATION") %></asp:Panel>
								</div>
							</td>
							<td>
								<div class="ActivityStreamDescription"><%# StreamFormatDescription(Sql.ToString(Eval("MODULE_NAME")), L10n, T10n, Container.DataItem) %></div>
								<div class="ActivityStreamIdentity">
									<span class="ActivityStreamCreatedBy"><%# Eval("CREATED_BY") %></span>
									<span class="ActivityStreamDateEntered"><%# Eval("STREAM_DATE") %></span>
								</div>
							</td>
							<td width="20px">
								<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(Eval("MODULE_NAME")), "view") >= 0 && (Sql.ToString(Eval("STREAM_ACTION")) != "Deleted") %>' CommandName="Preview" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_PREVIEW") %>' SkinID="preview_inline" Runat="server" />
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

