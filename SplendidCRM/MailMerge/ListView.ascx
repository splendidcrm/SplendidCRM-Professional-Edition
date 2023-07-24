<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.MailMerge.ListView" %>
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
	<SplendidCRM:HeaderButtons ID="ctlModuleHeader" Module="MailMerge" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	
	<script type="text/javascript">
	function AddRecords()
	{
		return ModulePopup('<%= lstPRIMARY_MODULE.SelectedValue %>', '<%= txtADD_RECORDS.ClientID %>', null, 'ClearDisabled=1', false, 'PopupMultiSelect.aspx', '<%= btnADD_RECORDS.ClientID %>');
	}
	function SelectSecondary(sPrimaryID, sPrimaryName)
	{
		document.getElementById('<%= txtPRIMARY_ID.ClientID %>').value = sPrimaryID;
		return ModulePopup('<%= hidSECONDARY_MODULE.Value %>', '<%= txtSECONDARY_ID.ClientID %>', null, 'FilterAccount=1&<%= sSINGULAR_NAME %>_ID=' + sPrimaryID + '&<%= sSINGULAR_NAME %>_NAME=' + escape(sPrimaryName), false, 'PopupMultiSelect.aspx', '<%= btnCHANGE_SECONDARY.ClientID %>');
	}
	</script>

	<asp:HiddenField ID="txtADD_RECORDS"      runat="server" />
	<asp:Button      ID="btnADD_RECORDS"      OnClick="btnADD_RECORDS_Click" style="display: none" runat="server" />
	<asp:HiddenField ID="txtPRIMARY_ID"       runat="server" />
	<asp:HiddenField ID="hidSECONDARY_MODULE" Runat="server" />
	<asp:HiddenField ID="txtSECONDARY_ID"     runat="server" />
	<asp:Button      ID="btnCHANGE_SECONDARY"   OnClick="btnCHANGE_SECONDARY_Click" style="display: none" runat="server" />
	<asp:Table SkinID="tabEditViewButtons" Visible="<%# !PrintView %>" runat="server">
		<asp:TableRow>
			<asp:TableCell ID="tdButtons" Width="10%" VerticalAlign="Top" Wrap="false">
				<asp:Button ID="btnGenerate" OnCommand="Page_Command" CommandName="Generate" CssClass="button" Text='<%# "  " + L10n.Term("MailMerge.LBL_GENERATE_BUTTON") + "  " %>' ToolTip='<%# L10n.Term("MailMerge.LBL_GENERATE_BUTTON") %>' Runat="server" />&nbsp;
			</asp:TableCell>
			<asp:TableCell>
				&nbsp;<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
			</asp:TableCell>
			<asp:TableCell ID="tdRequired" HorizontalAlign="Right" VerticalAlign="Top" Wrap="false" Visible="false">
				<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
				&nbsp;
				<asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableHeaderRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("MailMerge.LBL_INSTRUCTIONS") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableHeaderRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="15%"><asp:Label Text='<%# L10n.Term("MailMerge.LBL_SELECT_TEMPLATE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="Top" Width="35%">
							<asp:DropDownList ID="lstDOCUMENT_TEMPLATE" DataValueField="ID" DataTextField="NAME" OnSelectedIndexChanged="lstDOCUMENT_TEMPLATE_SelectedIndexChanged" AutoPostBack="true" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="50%"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("MailMerge.LBL_SELECTED_MODULE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
							<asp:DropDownList ID="lstPRIMARY_MODULE" DataValueField="MODULE_NAME" DataTextField="TITLE" OnSelectedIndexChanged="lstPRIMARY_MODULE_SelectedIndexChanged" AutoPostBack="true" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="15%"><asp:Label Text='<%# L10n.Term("MailMerge.LBL_SECONDARY_MODULE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="35%">
							<asp:Label       ID="lblSECONDARY_MODULE" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="50%"></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell>
							<asp:Button ID="btnAddRecord" UseSubmitBehavior="false" OnClientClick="return AddRecords();" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_ADD_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_ADD_BUTTON_TITLE") %>' Runat="server" />&nbsp;
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
								<Columns>
									<asp:TemplateColumn HeaderText="MailMerge.LBL_LIST_MODULE_NAME" ItemStyle-Width="16%">
										<ItemTemplate>
											<%# L10n.Term(".moduleList." + Sql.ToString(Eval("MODULE_NAME"))) %>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn    HeaderText="MailMerge.LBL_LIST_NAME" DataField="NAME" ItemStyle-Width="40%" />
									<asp:TemplateColumn HeaderText="" ItemStyle-Width="2%" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
										<ItemTemplate>
											<asp:ImageButton CommandName="Primary.Delete" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
											<asp:LinkButton  CommandName="Primary.Delete" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn    HeaderText="MailMerge.LBL_LIST_SECONDARY_NAME" DataField="SECONDARY_NAME" ItemStyle-Width="40%" />
									<asp:TemplateColumn HeaderText="" ItemStyle-Width="2%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
										<ItemTemplate>
											<asp:ImageButton OnClientClick=<%# "return SelectSecondary('" + Sql.ToString(Eval("ID")) + "', '" + Sql.EscapeJavaScript(Sql.ToString(Eval("NAME"))) + "');" %> CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
											<asp:LinkButton  OnClientClick=<%# "return SelectSecondary('" + Sql.ToString(Eval("ID")) + "', '" + Sql.EscapeJavaScript(Sql.ToString(Eval("NAME"))) + "');" %> CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
											<asp:ImageButton Visible='<%# !Sql.IsEmptyGuid(Eval("SECONDARY_ID")) %>' CommandName="Secondary.Delete" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
											<asp:LinkButton  Visible='<%# !Sql.IsEmptyGuid(Eval("SECONDARY_ID")) %>' CommandName="Secondary.Delete" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</SplendidCRM:SplendidGrid>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

