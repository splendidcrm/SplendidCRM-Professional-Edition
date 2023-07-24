<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="PopupSmsNumbers.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.SmsMessages.PopupSmsNumbers" %>
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
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="SmsMessages" IsPopupSearch="true" ShowSearchTabs="false" Visible="<%# !PrintView %>" Runat="Server" />

<script type="text/javascript">
function SelectContact(sCONTACT_ID, sCONTACT_PHONE_MOBILE)
{
	if ( window.opener != null && window.opener.ChangeContactSmsNumber != null )
	{
		window.opener.ChangeContactSmsNumber(sCONTACT_ID, sCONTACT_PHONE_MOBILE);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Contact cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
	}
}
function Clear()
{
	// 11/20/2005 Paul.  Clear does nothing on SugarCRM 3.5.1. 
	if ( window.opener != null && window.opener.ChangeContactSmsNumber != null )
	{
		window.opener.ChangeContactSmsNumber('', '', '');
		window.close();
	}
	else
	{
		alert('Original window has closed.  Contact cannot be assigned.');
	}
}
function Cancel()
{
	window.close();
}
</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Contacts.LBL_LIST_FORM_TITLE" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="Contacts.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-Width="35%">
				<ItemTemplate>
					<a name="CONTACT_ID" id="CONTACT_ID_<%# Eval("ID") %>" class="listViewTdLinkS1" href="#" onclick="SelectContact('<%# Sql.EscapeJavaScript(Sql.ToString(Eval("ID"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(Eval("PHONE_MOBILE"))) %>');"><%# Eval("NAME") %></a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Contacts.LBL_LIST_PHONE_MOBILE" SortExpression="PHONE_MOBILE" ItemStyle-Width="25%">
				<ItemTemplate>
					<a name="CONTACT_ID_PHONE_MOBILE" id="CONTACT_ID_PHONE_MOBILE_<%# Eval("ID") %>" class="listViewTdLinkS1" href="#" onclick="SelectContact('<%# Sql.EscapeJavaScript(Sql.ToString(Eval("ID"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(Eval("PHONE_MOBILE"))) %>');"><%# Eval("PHONE_MOBILE") %></a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn    HeaderText="Contacts.LBL_LIST_ACCOUNT_NAME"  DataField="ACCOUNT_NAME"     SortExpression="ACCOUNT_NAME" ItemStyle-Width="25%" />
			<asp:TemplateColumn HeaderText="SmsMessages.LBL_LIST_TYPE"                                    SortExpression="MODULE_TYPE" ItemStyle-Width="15%">
				<ItemTemplate>
					<%# L10n.Term(".moduleListSingular.", Eval("MODULE_TYPE")) %>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>

