<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Products.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
function ContactPopup()
{
	var sACCOUNT_NAME = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "ACCOUNT_NAME").ClientID %>').value;
	return ModulePopup('Contacts', '<%= new SplendidCRM.DynamicControl(this, "CONTACT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "CONTACT_NAME").ClientID %>', 'ACCOUNT_NAME=' + escape(sACCOUNT_NAME), false, null);
}
// 05/14/2012 Paul.  ChangeProductCatalog to match ModulePopupScripts generated code. 
function ChangeProductCatalog(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= new SplendidCRM.DynamicControl(this, "PRODUCT_TEMPLATE_ID").ClientID %>').value = sPARENT_ID  ;
	document.getElementById('<%= new SplendidCRM.DynamicControl(this, "NAME"               ).ClientID %>').value = sPARENT_NAME;
	var btnProductChanged = document.getElementById('<%= btnProductChanged.ClientID %>');
	btnProductChanged.click();
}
function ProductCatalogPopup()
{
	window.open('../Products/ProductCatalog/Popup.aspx', 'ProductCatalogPopup', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
	return false;
}
</script>
</SplendidCRM:InlineScript>
<div id="divNewRecord">
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderLeft" Src="~/_controls/HeaderLeft.ascx" %>
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="Products.LBL_NEW_FORM_TITLE" Width=<%# uWidth %> Visible="<%# ShowHeader %>" Runat="Server" />

	<asp:Button ID="btnProductChanged" Text="Product Changed" OnClick="btnProductChanged_Clicked" style="DISPLAY:none" runat="server" />
	<asp:Panel ID="pnlMain" Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
		<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# ShowTopButtons && !PrintView %>" Runat="server" />

		<asp:Panel ID="pnlEdit" CssClass="" style="margin-bottom: 4px;" Width=<%# uWidth %> runat="server">
			<asp:Literal Text='<%# "<h4>" + L10n.Term("Products.LBL_NEW_FORM_TITLE") + "</h4>" %>' Visible="<%# ShowInlineHeader %>" runat="server" />
			<table ID="tblMain" class="tabEditView" runat="server">
			</table>
		</asp:Panel>

		<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# ShowBottomButtons && !PrintView %>" Runat="server" />
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
</div>
