<%@ Control CodeBehind="SystemCurrencyLog.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.CurrencyLayer.SystemCurrencyLog" %>
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
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divCurrencyLayerSystemCurrencyLog" Title="CurrencyLayer.LBL_SYSTEM_CURRENCY_LOG" Runat="Server" />

<div id="divCurrencyLayerSystemCurrencyLog" style='<%= "display:" + (CookieValue("divCurrencyLayerSystemCurrencyLog") != "1" ? "inline" : "none") %>'>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:BoundColumn     HeaderText=".LBL_LIST_DATE_ENTERED"                     DataField="DATE_ENTERED"        SortExpression="DATE_ENTERED"        DataFormatString="{0:G}" />
			<asp:BoundColumn     HeaderText=".LBL_LIST_CREATED_BY"                       DataField="CREATED_BY"          SortExpression="CREATED_BY"          />
			<asp:BoundColumn     HeaderText="CurrencyLayer.LBL_LIST_SOURCE_ISO4217"      DataField="SOURCE_ISO4217"      SortExpression="SOURCE_ISO4217"      />
			<asp:BoundColumn     HeaderText="CurrencyLayer.LBL_LIST_DESTINATION_ISO4217" DataField="DESTINATION_ISO4217" SortExpression="DESTINATION_ISO4217" />
			<asp:BoundColumn     HeaderText="CurrencyLayer.LBL_LIST_CONVERSION_RATE"     DataField="CONVERSION_RATE"     SortExpression="CONVERSION_RATE"     DataFormatString="{0:F3}" />
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
