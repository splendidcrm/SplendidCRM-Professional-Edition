<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NAICSCodeSelect.ascx.cs" Inherits="SplendidCRM._controls.NAICSCodeSelect" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<asp:UpdatePanel UpdateMode="Conditional" runat="server">
	<ContentTemplate>
		<asp:Panel ID="pnlAddReplace" Visible="<%# bShowAddReplace %>" CssClass="setAddReplace" runat="server">
			<asp:RadioButton ID="radNaicsSetReplace" Enabled="<%# bEnabled %>" GroupName="NaicsSetAddReplace" Text='<%# L10n.Term("NAICSCodes.LBL_REPLACE_NAICS_SET") %>' CssClass="radio" Checked="true" runat="server" />
			&nbsp;
			<asp:RadioButton ID="radNaicsSetAdd"     Enabled="<%# bEnabled %>" GroupName="NaicsSetAddReplace" Text='<%# L10n.Term("NAICSCodes.LBL_ADD_NAICS_SET"      ) %>' CssClass="radio" runat="server" />
		</asp:Panel>
		<asp:GridView ID="grdMain" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"
			AutoGenerateEditButton="false" AutoGenerateDeleteButton="false" OnRowCreated="grdMain_RowCreated" OnRowDataBound="grdMain_RowDataBound"
			OnRowEditing="grdMain_RowEditing" OnRowDeleting="grdMain_RowDeleting" OnRowUpdating="grdMain_RowUpdating" OnRowCancelingEdit="grdMain_RowCancelingEdit"
			CssClass="listView setListView" runat="server">
			<RowStyle            CssClass="oddListRowS1"  VerticalAlign="Top" />
			<AlternatingRowStyle CssClass="evenListRowS1" VerticalAlign="Top" />
			<HeaderStyle         CssClass="listViewThS1"  />
			<Columns>
				<asp:TemplateField>
					<ItemTemplate><%# Eval("NAICS_CODE_NAME") %></ItemTemplate>
					<EditItemTemplate>
						<asp:DropDownList ID="lstNAICS_CODE_ID" Enabled="<%# bEnabled %>" DataValueField="ID" DataTextField="NAME" Visible="false" runat="server" />
						<span style="display: <%# bSupportsPopups ? "inline" : "none" %>">
						<asp:HiddenField ID="NAICS_CODE_ID"        value='<%# Eval("NAICS_CODE_ID"  ) %>' runat="server" />
						<asp:HiddenField ID="PREV_NAICS_CODE_NAME" value='<%# Eval("NAICS_CODE_NAME") %>' runat="server" />
						<%-- 12/24/2017 Paul.  Use a div tag so that we can manage spacing with Arctic theme. --%>
						<div class="modulePopupText" style="white-space: nowrap;">
							<asp:TextBox  ID="NAICS_CODE_NAME" Enabled="<%# bEnabled %>" Text='<%# Eval("NAICS_CODE_NAME") %>' TabIndex="<%# nTagIndex %>" onblur="NAICS_CODES_NAICS_CODE_NAME_Changed(this);" autocomplete="off" runat="server" />
							<asp:Button ID="SELECT_NAME" Enabled="<%# bEnabled %>" UseSubmitBehavior="false" OnClientClick="return NAICSCodeSelectPopup(this);" CssClass="button" Text='<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' Runat="server" />
						</div>
						</span>
						<span id="NAICS_CODE_NAME_AjaxErrors" style="color:Red" EnableViewState="false" runat="server" />
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ItemStyle-Wrap="false">
					<ItemTemplate>
						<asp:ImageButton ID="btnEdit"   Enabled="<%# bEnabled %>" CommandName="Edit"   SkinID="set_edit_inline"   runat="server" />
						<asp:ImageButton ID="btnDelete" Enabled="<%# bEnabled %>" CommandName="Delete" SkinID="set_delete_inline" runat="server" />
					</ItemTemplate>
					<EditItemTemplate>
						<asp:ImageButton ID="btnUpdate" Enabled="<%# bEnabled %>" CommandName="Update" SkinID="set_update_inline" runat="server" />
						<asp:ImageButton ID="btnCancel" Enabled="<%# bEnabled %>" CommandName="Cancel" SkinID="set_cancel_inline" runat="server" />
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:CommandField Visible="false" ButtonType="Image" ShowEditButton="true" ShowDeleteButton="true" ControlStyle-CssClass="button" ItemStyle-Width="10%" ItemStyle-Wrap="false" 
					EditText=".LBL_EDIT_BUTTON_LABEL"     EditImageUrl="~/App_Themes/Sugar/images/set_edit_inline.gif" 
					DeleteText=".LBL_REMOVE"              DeleteImageUrl="~/App_Themes/Sugar/images/set_delete_inline.gif" 
					UpdateText=".LBL_UPDATE_BUTTON_LABEL" UpdateImageUrl="~/App_Themes/Sugar/images/set_update_inline.gif" 
					CancelText=".LBL_CANCEL_BUTTON_LABEL" CancelImageUrl="~/App_Themes/Sugar/images/set_cancel_inline.gif" 
					/>
			</Columns>
		</asp:GridView>
		<SplendidCRM:RequiredFieldValidatorForNAICSCodeSelect ID="valNAICSCodeSelect" ControlToValidate="grdMain" CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Runat="server" />
	</ContentTemplate>
</asp:UpdatePanel>