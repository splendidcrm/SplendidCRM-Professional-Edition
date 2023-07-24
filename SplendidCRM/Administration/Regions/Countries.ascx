<%@ Control CodeBehind="Countries.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Regions.Countries" %>
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
function ChangeCountry(sCOUNTRY_NAME)
{
	document.getElementById('<%= txtCOUNTRY.ClientID %>').value = sCOUNTRY_NAME;
	document.forms[0].submit();
}
function CountriesPopup()
{
	// 01/01/2017 Paul.  Change popup so that we can use the standard definition of PopupMultiSelect for Region selection. 
	return window.open('PopupCountries.aspx', 'CountriesPopup', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
}
</script>
<input ID="txtCOUNTRY" type="hidden" Runat="server" />
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divRegionsCountries" Title="Regions.LBL_COUNTRIES" Runat="Server" />

<div id="divRegionsCountries" style='<%= "display:" + (CookieValue("divRegionsCountries") != "1" ? "inline" : "none") %>'>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:BoundColumn     HeaderText="Regions.LBL_LIST_COUNTRY"  DataField="COUNTRY" SortExpression="COUNTRY" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton Visible='<%# SplendidCRM.Security.AdminUserAccess("Regions", "edit") >= 0 %>' CommandName="Countries.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "COUNTRY") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# SplendidCRM.Security.AdminUserAccess("Regions", "edit") >= 0 %>' CommandName="Countries.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "COUNTRY") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
