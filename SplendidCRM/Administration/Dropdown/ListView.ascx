<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Dropdown.ListView" %>
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
<div id="divListView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="Dropdown" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="Dropdown.LBL_LIST_FORM_TITLE" Visible="false" Runat="Server" />
	
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
				<asp:HiddenField ID="txtINDEX" Runat="server" />
				<asp:Button ID="btnINDEX_MOVE" style="display: none" runat="server" />
				<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
			</asp:Panel>
			
			<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="false" AllowSorting="false" EnableViewState="true" ShowFooter='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' runat="server">
				<Columns>
					<asp:TemplateColumn ItemStyle-CssClass="dragHandle">
						<ItemTemplate><asp:Image SkinID="blank" Width="14px" runat="server" /></ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Dropdown.LBL_KEY"   ItemStyle-Width="29%">
						<ItemTemplate><%# Eval("NAME") %></ItemTemplate>
						<EditItemTemplate><asp:TextBox ID="txtNAME" Text='<%# Eval("NAME") %>' runat="server" /></EditItemTemplate>
						<FooterTemplate><asp:TextBox ID="txtNAME" Text='<%# Eval("NAME") %>' runat="server" /></FooterTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Dropdown.LBL_VALUE" ItemStyle-Width="50%">
						<ItemTemplate><%# Server.HtmlEncode(Eval("DISPLAY_NAME") as string) %></ItemTemplate>
						<EditItemTemplate><asp:TextBox ID="txtDISPLAY_NAME" Text='<%# Eval("DISPLAY_NAME") %>' size="40" runat="server" /></EditItemTemplate>
						<FooterTemplate><asp:TextBox ID="txtDISPLAY_NAME" Text='<%# Eval("DISPLAY_NAME") %>' size="40" runat="server" /></FooterTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" Visible="false">
						<ItemTemplate>
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveUp"   CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_UP") %>' SkinID="uparrow_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveUp"   CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_UP") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveDown" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' SkinID="downarrow_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveDown" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_DOWN") %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
						<ItemTemplate><%# Eval("LIST_ORDER") %></ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Update"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>' SkinID="accept_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Update"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Cancel"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' SkinID="decline_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Cancel"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Edit"     CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Edit"     CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Delete"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Delete"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
						</ItemTemplate>
						<FooterTemplate>
							<asp:Button ID="btnInsert" CommandName="Insert" Text='<%# L10n.Term(".LBL_ADD_BUTTON_LABEL") %>' Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' runat="server" />
						</FooterTemplate>
					</asp:TemplateColumn>
				</Columns>
			</SplendidCRM:SplendidGrid>
			
			<SplendidCRM:InlineScript runat="server">
				<script type="text/javascript">
				// http://www.isocra.com/2008/02/table-drag-and-drop-jquery-plugin/
				$(document).ready(function()
				{
					$("#<%= grdMain.ClientID %>").tableDnD
					({
						dragHandle: "dragHandle",
						onDragClass: "jQueryDragBorder",
						onDragStart: function(table, row)
						{
							var txtINDEX = document.getElementById('<%= txtINDEX.ClientID %>');
							txtINDEX.value = (row.parentNode.rowIndex-1);
						},
						onDrop: function(table, row)
						{
							var txtINDEX = document.getElementById('<%= txtINDEX.ClientID %>');
							txtINDEX.value += ',' + (row.rowIndex-1); 
							document.getElementById('<%= btnINDEX_MOVE.ClientID %>').click();
						}
					});
					$("#<%= grdMain.ClientID %> tr").hover
					(
						function()
						{
							if ( !$(this).hasClass("nodrag") )
								$(this.cells[0]).addClass('jQueryDragHandle');
						},
						function()
						{
							if ( !$(this).hasClass("nodrag") )
								$(this.cells[0]).removeClass('jQueryDragHandle');
						}
					);
				});
				</script>
			</SplendidCRM:InlineScript>
		</ContentTemplate>
	</asp:UpdatePanel>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

