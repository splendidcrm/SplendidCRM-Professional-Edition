<%@ Control Language="c#" AutoEventWireup="false" Codebehind="AllocationsView.ascx.cs" Inherits="SplendidCRM.Payments.AllocationsView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
var sSelectNameUserContext = '';
function ChangeInvoice(sPARENT_ID, sPARENT_NAME)
{
	var fldINVOICE_IDS = document.getElementById('<%# INVOICE_IDS.ClientID %>');
	var fldINVOICE_NAME = document.getElementById(sSelectNameUserContext + 'INVOICE_NAME');
	// 08/16/2010 Paul.  Use a button instead of form submit so that the UpdatePanel will do its job. 
	var btnINVOICES_CHANGED = document.getElementById('<%# btnINVOICES_CHANGED.ClientID %>');
	if ( fldINVOICE_IDS != null && sPARENT_ID.indexOf(',') > 0 )
	{
		fldINVOICE_IDS.value = sPARENT_ID;
		//document.forms[0].submit();
		btnINVOICES_CHANGED.click();
	}
	// 08/16/2010 Paul.  If the name is not provided, then we will need to look it up. 
	else if ( sPARENT_NAME == '' && sPARENT_ID != '' )
	{
		fldINVOICE_IDS.value = sPARENT_ID;
		//document.forms[0].submit();
		btnINVOICES_CHANGED.click();
	}
	else if ( fldINVOICE_NAME != null )
	{
		fldINVOICE_IDS.value = sPARENT_ID;
		fldINVOICE_NAME.value = sPARENT_NAME;
		ItemNameChanged('<%= GetCurrencyControl() != null ? CURRENCY_ID.ClientID : "" %>', fldINVOICE_NAME);
	}
}
function InvoicePopup(fldSELECT_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	sSelectNameUserContext = fldSELECT_NAME.id.replace('SELECT_NAME', '');
	var sINVOICE_NAME = '';
	var fldINVOICE_NAME = document.getElementById(sSelectNameUserContext + 'INVOICE_NAME');
	if ( fldINVOICE_NAME != null )
	{
		sINVOICE_NAME = fldINVOICE_NAME.value;
	}
	// 04/23/2008 Paul.  We need to retrive the account from the form as the control variable will not be valid during a postback. 
	// 08/05/2010 Paul.  Now that we are using InlineScript, we can use the local gACCOUNT_ID. 
	var gACCOUNT_ID = '<%= gACCOUNT_ID %>';
	window.open('../Invoices/PopupMultiSelect.aspx?ACCOUNT_ID=' + gACCOUNT_ID + '&INVOICE_STAGE=&ClearDisabled=1&NAME=' + escape(sINVOICE_NAME), 'InvoicePopup', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
	return false;
}
</script>
</SplendidCRM:InlineScript>
<div id="divAllocationsView">
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<asp:Button ID="btnINVOICES_CHANGED" Text="Insert" style="display: none" runat="server" />
			<asp:HiddenField ID="INVOICE_IDS" runat="server" />
			<asp:GridView ID="grdMain" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" 
				AutoGenerateEditButton="false" AutoGenerateDeleteButton="false" 
				Width="100%" runat="server">
				<RowStyle            CssClass="oddListRowS1"  VerticalAlign="Top" />
				<AlternatingRowStyle CssClass="evenListRowS1" VerticalAlign="Top" />
				<HeaderStyle         CssClass="listViewThS1"  />
				<Columns>
					<asp:TemplateField HeaderText="Payments.LBL_LIST_INVOICE_NAME" ItemStyle-Width="30%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Eval("INVOICE_NAME") %></ItemTemplate>
						<EditItemTemplate>
							<asp:HiddenField ID="ID"                  value='<%# Eval("ID"          ) %>' runat="server" />
							<asp:HiddenField ID="INVOICE_ID"          value='<%# Eval("INVOICE_ID"  ) %>' runat="server" />
							<asp:HiddenField ID="PREVIOUS_NAME"       value='<%# Eval("INVOICE_NAME") %>' runat="server" />
							<asp:HiddenField ID="AMOUNT_DUE_USDOLLAR" value='<%# Sql.ToDecimal(Eval("AMOUNT_DUE_USDOLLAR")).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="AMOUNT_USDOLLAR"     value='<%# Sql.ToDecimal(Eval("AMOUNT_USDOLLAR"    )).ToString("0.000") %>' runat="server" />
							<nobr>
							<asp:TextBox ID="INVOICE_NAME" Text='<%# Eval("INVOICE_NAME") %>' TabIndex="12" onblur=<%# "ItemNameChanged('" + (GetCurrencyControl() != null ? CURRENCY_ID.ClientID : "") + "', this);" %> autocomplete="off" runat="server" />
							<asp:Button ID="SELECT_NAME" UseSubmitBehavior="false" OnClientClick="return InvoicePopup(this);" CssClass="button" Text='<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' Runat="server" />
							</nobr>
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Invoices.LBL_LIST_AMOUNT_DUE" ItemStyle-Width="25%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Sql.ToDecimal(Eval("AMOUNT_DUE")).ToString("c") %></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="AMOUNT_DUE" Text='<%# Sql.ToDecimal(Eval("AMOUNT_DUE")).ToString("0.00") %>' Width="60" TabIndex="17" ReadOnly="true" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Payments.LBL_LIST_ALLOCATED" ItemStyle-Width="25%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Sql.ToDecimal(Eval("AMOUNT")).ToString("c") %></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="AMOUNT" Text='<%# Sql.ToDecimal(Eval("AMOUNT")).ToString("0.00") %>' Width="60" TabIndex="17" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:Button ID="btnUpdate" CommandName="Edit"   OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_EDIT_BUTTON_LABEL"  ) %>' CssClass="button" runat="server" />
							<asp:Button ID="btnCancel" CommandName="Delete" OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_DELETE_BUTTON_LABEL") %>' CssClass="button" runat="server" />
						</ItemTemplate>
						<EditItemTemplate>
							<asp:Button ID="btnUpdate" CommandName="Update" OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>' CssClass="button" runat="server" />
							<asp:Button ID="btnCancel" CommandName="Cancel" OnCommand="LINE_ITEM_Command" CommandArgument='<%# Container.DataItemIndex %>' Text='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' CssClass="button" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
				</Columns>
			</asp:GridView>
			<asp:Label ID="lblLineItemError" ForeColor="Red" EnableViewState="false" runat="server" />
			
			<span id="AjaxErrors" style="color:Red"></span>
	
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:HiddenField ID="ALLOCATED_USDOLLAR" runat="server" />
						<asp:HiddenField ID="EXCHANGE_RATE"      runat="server" />
						<asp:Table ID="tblSummary" SkinID="tabDetailView" runat="server">
							<asp:TableRow>
								<asp:TableCell Width="65%">&nbsp;</asp:TableCell>
								<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label ID="LBL_ALLOCATED" Text='<%# L10n.Term("Payments.LBL_ALLOCATED") %>' runat="server" /></asp:TableCell>
								<asp:TableCell Width="20%" CssClass="dataField"><asp:TextBox ID="ALLOCATED" ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>