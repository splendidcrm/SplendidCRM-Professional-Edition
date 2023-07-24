<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditLineItemsView.ascx.cs" Inherits="SplendidCRM._controls.EditLineItemsView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Import Namespace="System.Globalization" %>
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
<script type="text/javascript">
// 02/29/2008 Paul.  The config value should only be used as an override.  We should default to the .NET culture value. 
// 03/07/2008 Paul.  We are going to let the ASP.NET and Microsoft AJAX culture engines take care of number formatting. 
// 03/07/2008 Paul.  Prevent Microsoft AJAX from using the currency symbol.  
// The problem is that the culture value may not match the user's selected value, which can cause parsing errors. 
Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol = '';

var sSelectNameUserContext = '';
// 05/14/2012 Paul.  ChangeProductCatalog to match ModulePopupScripts generated code. 
function ChangeProductCatalog(sPARENT_ID, sPARENT_NAME)
{
	var bEnableOptions = '<%= bEnableOptions ? 1 : 0 %>';
	if ( bEnableOptions == '1' || sPARENT_NAME.length == 0 )
	{
		var hidPRODUCTS        = document.getElementById('<%= hidPRODUCTS.ClientID %>');
		var btnPRODUCT_OPTIONS = document.getElementById('<%= btnPRODUCT_OPTIONS.ClientID %>');
		hidPRODUCTS.value = sPARENT_ID;
		btnPRODUCT_OPTIONS.click();
	}
	else
	{
		var fldNAME = document.getElementById(sSelectNameUserContext + 'NAME');
		if ( fldNAME != null )
		{
			fldNAME.value = sPARENT_NAME;
			ItemNameChanged('<%= CURRENCY_ID.ClientID %>', fldNAME);
		}
	}
}
function ProductPopup(fldSELECT_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	sSelectNameUserContext = fldSELECT_NAME.id.replace('SELECT_NAME', '');
	var sNAME = '';
	var fldNAME = document.getElementById(sSelectNameUserContext + 'NAME');
	if ( fldNAME != null )
	{
		sNAME = fldNAME.value;
	}
	window.open('../Products/ProductCatalog/Popup.aspx?ClearDisabled=1&NAME=' + escape(sNAME), 'ProductPopup', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
	return false;
}
</script>
<div id="divEditLineItemsView">
	<asp:UpdatePanel ID="ctlLineHeaderPanel" runat="server">
		<ContentTemplate>
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:Table SkinID="tabFrame" runat="server">
							<asp:TableRow Visible="false">
								<asp:TableHeaderCell ColumnSpan="10"><asp:Label Text='<%# L10n.Term(m_sMODULE + ".LBL_LINE_ITEMS_TITLE") %>' runat="server" /></asp:TableHeaderCell>
							</asp:TableRow>
							<asp:TableRow>
								<asp:TableCell>
									<asp:Label ID="LBL_CURRENCY"         Text='<%# L10n.Term(m_sMODULE + ".LBL_CURRENCY"        ) %>' runat="server" />&nbsp;<asp:DropDownList ID="CURRENCY_ID" DataValueField="ID" DataTextField="NAME_SYMBOL" OnSelectedIndexChanged="CURRENCY_ID_Changed" AutoPostBack="true" Runat="server" />
									&nbsp;
									<asp:Label ID="LBL_CONVERSION_RATE"  Text='<%# L10n.Term(m_sMODULE + ".LBL_CONVERSION_RATE" ) %>' runat="server" />&nbsp;<asp:TextBox ID="EXCHANGE_RATE" Size="4" MaxLength="10" runat="server" />&nbsp;
								</asp:TableCell>
								<asp:TableCell>
									<asp:Label ID="LBL_TAXRATE"          Text='<%# L10n.Term(m_sMODULE + ".LBL_TAXRATE"         ) %>' runat="server" />&nbsp;
									<asp:DropDownList ID="TAXRATE_ID"  DataValueField="ID" DataTextField="NAME" OnSelectedIndexChanged="TAXRATE_ID_Changed" AutoPostBack="true" Runat="server" />
									<asp:Button ID="btnTAXRATE_LOOKUP" Text='<%# L10n.Term("Orders.LBL_LOOKUP_TAX_LABEL") %>' ToolTip='<%# L10n.Term("Orders.LBL_LOOKUP_TAX_TITLE") %>' Visible='<%# true || !Sql.IsEmptyString(sZIPTAX_KEY) %>' OnCommand="Page_Command" CommandName="Tax.Lookup" CssClass="button" runat="server" />&nbsp;
									<asp:Label ID="lblTaxError" CssClass="error" EnableViewState="false" runat="server" />
								</asp:TableCell>
								<asp:TableCell>
									<asp:Label ID="LBL_SHIPPER"          Text='<%# L10n.Term(m_sMODULE + ".LBL_SHIPPER"         ) %>' runat="server" />&nbsp;
									<asp:DropDownList ID="SHIPPER_ID"  DataValueField="ID" DataTextField="NAME" Runat="server" />
								</asp:TableCell>
								<asp:TableCell>
									<asp:Label ID="LBL_SHOW_LINE_NUMS"   Text='<%# L10n.Term(m_sMODULE + ".LBL_SHOW_LINE_NUMS"  ) %>' Visible="false" runat="server" />&nbsp;
									<asp:CheckBox     ID="SHOW_LINE_NUMS"   CssClass="checkbox" Visible="false" Runat="server" />
								</asp:TableCell>
								<asp:TableCell>
									<asp:Label ID="LBL_CALC_GRAND_TOTAL" Text='<%# L10n.Term(m_sMODULE + ".LBL_CALC_GRAND_TOTAL") %>' Visible="false" runat="server" />&nbsp;
									<asp:CheckBox     ID="CALC_GRAND_TOTAL" CssClass="checkbox" Visible="false" Runat="server" />
								</asp:TableCell>
								<asp:TableCell>&nbsp;</asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</ContentTemplate>
	</asp:UpdatePanel>
	<asp:UpdatePanel ID="ctlLineItemsPanel" runat="server">
		<ContentTemplate>
			<asp:HiddenField ID="hidPRODUCTS" runat="server" />
			<asp:Button ID="btnPRODUCT_OPTIONS" OnCommand="btnPRODUCT_OPTIONS_Clicked" Text="Insert" style="display: none" runat="server" />
			<asp:GridView ID="grdMain" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" 
				AutoGenerateEditButton="false" AutoGenerateDeleteButton="false" OnRowCreated="grdMain_RowCreated" OnRowDataBound="grdMain_RowDataBound"
				OnRowEditing="grdMain_RowEditing" OnRowDeleting="grdMain_RowDeleting" OnRowUpdating="grdMain_RowUpdating" OnRowCancelingEdit="grdMain_RowCancelingEdit" 
				Width="100%" runat="server">
				<RowStyle            CssClass="oddListRowS1"  VerticalAlign="Top" />
				<AlternatingRowStyle CssClass="evenListRowS1" VerticalAlign="Top" />
				<HeaderStyle         CssClass="listViewThS1"  />
				<Columns>
					<asp:TemplateField HeaderText="" ItemStyle-Width="1%" HeaderStyle-Wrap="false">
						<ItemTemplate><asp:Label Text='<%# Eval("LINE_ITEM_TYPE") %>' runat="server" /></ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_QUANTITY" ItemStyle-Width="10%" HeaderStyle-Wrap="false">
						<ItemTemplate><asp:Label Text='<%# Eval("QUANTITY") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "QUANTITY") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:HiddenField ID="ID"                    value='<%# Eval("ID"                 ) %>' runat="server" />
							<asp:HiddenField ID="LINE_GROUP_ID"         value='<%# Eval("LINE_GROUP_ID"      ) %>' runat="server" />
							<asp:HiddenField ID="LINE_ITEM_TYPE"        value='<%# Eval("LINE_ITEM_TYPE"     ) %>' runat="server" />
							<asp:HiddenField ID="PRODUCT_TEMPLATE_ID"   value='<%# Eval("PRODUCT_TEMPLATE_ID") %>' runat="server" />
							<asp:HiddenField ID="PARENT_TEMPLATE_ID"    value='<%# Eval("PARENT_TEMPLATE_ID" ) %>' runat="server" />
							<asp:HiddenField ID="PREVIOUS_NAME"         value='<%# Eval("NAME"               ) %>' runat="server" />
							<asp:HiddenField ID="VENDOR_PART_NUM"       value='<%# Eval("VENDOR_PART_NUM"    ) %>' runat="server" />
							<asp:HiddenField ID="PREVIOUS_MFT_PART_NUM" value='<%# Eval("MFT_PART_NUM"       ) %>' runat="server" />
							<asp:HiddenField ID="COST_USDOLLAR"         value='<%# Sql.ToDecimal(Eval("COST_USDOLLAR"    )).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="LIST_USDOLLAR"         value='<%# Sql.ToDecimal(Eval("LIST_USDOLLAR"    )).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="UNIT_USDOLLAR"         value='<%# Sql.ToDecimal(Eval("UNIT_USDOLLAR"    )).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="EXTENDED_USDOLLAR"     value='<%# Sql.ToDecimal(Eval("EXTENDED_USDOLLAR")).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="DISCOUNT_USDOLLAR"     value='<%# Sql.ToDecimal(Eval("DISCOUNT_USDOLLAR")).ToString("0.000") %>' runat="server" />

							<asp:TextBox ID="QUANTITY" Text='<%# Eval("QUANTITY") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "QUANTITY") %>' Width="50" TabIndex="11" onblur="ItemQuantityChanged(this);" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_NAME" ItemStyle-Width="20%" HeaderStyle-Wrap="false">
						<ItemTemplate><asp:Label Text='<%# Eval("NAME") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "NAME") %>' runat="server" /><asp:Literal Text="<br />" Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "NAME") %>' runat="server" /><asp:Label Text='<%# Sql.ToString(Eval("DESCRIPTION")).Replace(ControlChars.CrLf, "<br />" + ControlChars.CrLf) %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "DESCRIPTION") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<nobr>
							<asp:TextBox ID="NAME" Text='<%# Eval("NAME") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "NAME") %>' TabIndex="12" onblur=<%# "ItemNameChanged('" + CURRENCY_ID.ClientID + "', this);" %> autocomplete="off" runat="server" />
							<asp:Button ID="SELECT_NAME" Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "SELECT_NAME") %>' UseSubmitBehavior="false" OnClientClick="return ProductPopup(this);" CssClass="button" Text='<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' Runat="server" />
							<asp:Literal Text="<br />" Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "NAME") %>' runat="server" /></nobr>
							<asp:TextBox ID="DESCRIPTION" Text='<%# Eval("DESCRIPTION") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "DESCRIPTION") %>' TabIndex="24" TextMode="MultiLine" Rows="3" Width="180px" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_MFT_PART_NUM" ItemStyle-Width="20%" HeaderStyle-Wrap="false">
						<ItemTemplate><asp:Label Text='<%# Eval("MFT_PART_NUM") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "MFT_PART_NUM") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="MFT_PART_NUM" Text='<%# Eval("MFT_PART_NUM") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "MFT_PART_NUM") %>' TabIndex="14" onblur=<%# "ItemPartNumberChanged('" + CURRENCY_ID.ClientID + "', this);" %> autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_TAX_CLASS" ItemStyle-Width="10%" HeaderStyle-Wrap="false">
						<ItemTemplate><asp:Label Text='<%# Eval("TAX_CLASS") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "TAX_CLASS") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList ID="TAX_CLASS" Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "TAX_CLASS") %>' DataValueField="NAME" DataTextField="DISPLAY_NAME" TabIndex="15" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_TAX_RATE" ItemStyle-Width="10%" HeaderStyle-Wrap="false">
						<ItemTemplate><asp:Label Text='<%# TaxRateName(Eval("TAXRATE_ID")) %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "TAXRATE_ID") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList ID="lstTAXRATE_ID" Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "TAXRATE_ID") %>' DataValueField="ID" DataTextField="NAME" TabIndex="15" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_COST_PRICE" ItemStyle-Width="10%" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
						<ItemTemplate><asp:Label Text='<%# Sql.ToDecimal(Eval("COST_PRICE")).ToString("c") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "COST_PRICE") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="COST_PRICE" Text='<%# Sql.ToDecimal(Eval("COST_PRICE")).ToString("#,###.##") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "COST_PRICE") %>' Width="60" TabIndex="16" autocomplete="off" style="text-align: right" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_LIST_PRICE" ItemStyle-Width="10%" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
						<ItemTemplate><asp:Label Text='<%# Sql.ToDecimal(Eval("LIST_PRICE")).ToString("c") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "LIST_PRICE") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="LIST_PRICE" Text='<%# Sql.ToDecimal(Eval("LIST_PRICE")).ToString("#,###.##") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "LIST_PRICE") %>' Width="60" TabIndex="17" autocomplete="off" style="text-align: right" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_UNIT_PRICE" ItemStyle-Width="10%" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
						<ItemTemplate><asp:Label Text='<%# Sql.ToDecimal(Eval("UNIT_PRICE")).ToString("c") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "UNIT_PRICE") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="UNIT_PRICE" Text='<%# Sql.ToDecimal(Eval("UNIT_PRICE")).ToString("#,###.##") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "UNIT_PRICE") %>' Width="60" TabIndex="18" onblur="ItemUnitPriceChanged(this);" autocomplete="off" style="text-align: right" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_EXTENDED_PRICE" ItemStyle-Width="10%" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
						<ItemTemplate><asp:Label Text='<%# Sql.ToDecimal(Eval("EXTENDED_PRICE")).ToString("c") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "EXTENDED_PRICE") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="EXTENDED_PRICE" Text='<%# Sql.ToDecimal(Eval("EXTENDED_PRICE")).ToString("#,###.##") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "EXTENDED_PRICE") %>' Width="60" TabIndex="19" ReadOnly="true" autocomplete="off" style="text-align: right" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_TAX" ItemStyle-Width="5%" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
						<ItemTemplate><asp:Label Text='<%# Sql.ToDecimal(Eval("TAX")).ToString("c") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "TAX") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="TAX" Text='<%# Sql.ToDecimal(Eval("TAX")).ToString("#,###.##") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "TAX") %>' Width="30" TabIndex="20" ReadOnly="true" autocomplete="off" style="text-align: right" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="" ItemStyle-Width="10%" HeaderStyle-Wrap="false">
						<ItemTemplate>
							<asp:Label Text='<%# Eval("DISCOUNT_NAME") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "DISCOUNT_NAME") %>' runat="server" />
							<asp:Label Text='<%# L10n.Term(".pricing_formula_dom.", Eval("PRICING_FORMULA")) %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "PRICING_FORMULA") && !Sql.IsEmptyString(Eval("PRICING_FORMULA")) && Sql.IsEmptyString(Eval("DISCOUNT_NAME")) %>' runat="server" />
							<asp:Label Text='<%# Eval("PRICING_FACTOR" ) %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "PRICING_FACTOR" ) && !Sql.IsEmptyString(Eval("PRICING_FORMULA")) && Sql.IsEmptyString(Eval("DISCOUNT_NAME")) %>' runat="server" />
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList ID="DISCOUNT_ID" Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "DISCOUNT_ID") %>' DataValueField="ID" DataTextField="NAME" TabIndex="21" OnSelectedIndexChanged="DISCOUNT_ID_Changed" AutoPostBack="true" runat="server" /><br />
							<nobr>
								<asp:DropDownList ID="PRICING_FORMULA" Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "PRICING_FORMULA") %>' DataValueField="NAME" DataTextField="DISPLAY_NAME" TabIndex="21" OnSelectedIndexChanged="PRICING_FORMULA_Changed" AutoPostBack="true" runat="server" />
								<asp:TextBox ID="PRICING_FACTOR" Text='<%# Eval("PRICING_FACTOR") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "PRICING_FACTOR") %>' Width="30" TabIndex="22" autocomplete="off" runat="server" />
							</nobr>
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_DISCOUNT_NAME" ItemStyle-Width="10%" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
						<ItemTemplate><asp:Label Text='<%# Sql.ToDecimal(Eval("DISCOUNT_PRICE")).ToString("c") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "DISCOUNT_PRICE") %>' runat="server" /></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="DISCOUNT_PRICE" Text='<%# Sql.ToDecimal(Eval("DISCOUNT_PRICE")).ToString("#,###.##") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "DISCOUNT_PRICE") %>' Width="60" TabIndex="23" ReadOnly="true" autocomplete="off" style="text-align: right" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:CommandField Visible="false" ButtonType="Button" ShowEditButton="true" ShowDeleteButton="true" ControlStyle-CssClass="button" EditText=".LBL_EDIT_BUTTON_LABEL" DeleteText=".LBL_DELETE_BUTTON_LABEL" UpdateText=".LBL_UPDATE_BUTTON_LABEL" CancelText=".LBL_CANCEL_BUTTON_LABEL" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
					<asp:TemplateField ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton ID="btnMoveUp"   CommandName="MoveUp"   OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' AlternateText='<%# L10n.Term("Dropdown.LNK_UP"  ) %>' SkinID="uparrow_inline"   CssClass="listViewTdToolsS1" Runat="server" />
							<asp:ImageButton ID="btnMoveDown" CommandName="MoveDown" OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' SkinID="downarrow_inline" CssClass="listViewTdToolsS1" Runat="server" />&nbsp;
							<asp:Button      ID="btnUpdate"   CommandName="Edit"     OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_EDIT_BUTTON_LABEL"  ) %>'   CssClass="button" runat="server" />
							<asp:Button      ID="btnCancel"   CommandName="Delete"   OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_DELETE_BUTTON_LABEL") %>'   CssClass="button" runat="server" />
						</ItemTemplate>
						<EditItemTemplate>
							<asp:ImageButton ID="btnMoveUp"   CommandName="MoveUp"   OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' AlternateText='<%# L10n.Term("Dropdown.LNK_UP"  ) %>' SkinID="uparrow_inline"   Runat="server" />
							<asp:ImageButton ID="btnMoveDown" CommandName="MoveDown" OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' SkinID="downarrow_inline" Runat="server" />&nbsp;
							<asp:Button      ID="btnUpdate"   CommandName="Update"   OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>'   CssClass="button" runat="server" />
							<asp:Button      ID="btnCancel"   CommandName="Cancel"   OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>'   CssClass="button" runat="server" /><br />
							<asp:ImageButton ID="imgComment"  CommandName="Comment"  OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' AlternateText='<%# L10n.Term("Orders.LBL_ADD_COMMENT") %>' Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "btnComment") %>' SkinID="plus_inline" runat="server" />
							<asp:LinkButton  ID="btnComment"  CommandName="Comment"  OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term("Orders.LBL_ADD_COMMENT") %>'          Visible='<%# FieldVisibility(Sql.ToString(Eval("LINE_ITEM_TYPE")), "btnComment") %>' runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
				</Columns>
			</asp:GridView>
			<asp:Label ID="lblLineItemError" ForeColor="Red" EnableViewState="false" runat="server" />
		</ContentTemplate>
	</asp:UpdatePanel>
	
	<span id="AjaxErrors" style="color:Red"></span>
	
	<asp:UpdatePanel ID="ctlSummaryPanel" runat="server">
		<ContentTemplate>
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:HiddenField ID="DISCOUNT_USDOLLAR" runat="server" />
						<asp:HiddenField ID="SHIPPING_USDOLLAR" runat="server" />
						<asp:Table SkinID="tabFrame" runat="server">
							<asp:TableRow><asp:TableCell Width="65%"></asp:TableCell><asp:TableCell CssClass="dataLabel" Width="15%"><asp:Label ID="LBL_SUBTOTAL" Text='<%# L10n.Term(m_sMODULE + ".LBL_SUBTOTAL") %>' runat="server" /></asp:TableCell><asp:TableCell CssClass="dataField" width="20%"><asp:TextBox ID="SUBTOTAL" ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_DISCOUNT" Text='<%# L10n.Term(m_sMODULE + ".LBL_DISCOUNT") %>' runat="server" /></asp:TableCell><asp:TableCell CssClass="dataField"            ><asp:TextBox ID="DISCOUNT" ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_SHIPPING" Text='<%# L10n.Term(m_sMODULE + ".LBL_SHIPPING") %>' runat="server" /></asp:TableCell><asp:TableCell CssClass="dataField"            ><asp:TextBox ID="SHIPPING"                                     runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow ID="trSUMMARY_TAX"><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_TAX"      Text='<%# L10n.Term(m_sMODULE + ".LBL_TAX"     ) %>' runat="server" /></asp:TableCell><asp:TableCell CssClass="dataField"            ><asp:TextBox ID="TAX"      ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_TOTAL"    Text='<%# L10n.Term(m_sMODULE + ".LBL_TOTAL"   ) %>' runat="server" /></asp:TableCell><asp:TableCell CssClass="dataField"            ><asp:TextBox ID="TOTAL"    ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell></asp:TableRow>
						</asp:Table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>

