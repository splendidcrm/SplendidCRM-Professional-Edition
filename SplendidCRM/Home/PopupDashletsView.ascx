<%@ Control CodeBehind="PopupDashletsView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Home.PopupDashletsView" %>
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
<div id="divPopupDashlets">
	<script type="text/javascript">
	function SelectDashlet(sPARENT_ID, sPARENT_NAME)
	{
		if ( window.opener != null && window.opener.ChangeDashlet != null )
		{
			window.opener.ChangeDashlet(sPARENT_ID, sPARENT_NAME);
			window.close();
		}
		else
		{
			alert('Original window has closed.  Dashlet cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
		}
	}
	function SelectChecked()
	{
		if ( window.opener != null && window.opener.ChangeDashlet != null )
		{
			var sSelectedItems = document.getElementById('<%= ctlCheckAll.SelectedItems.ClientID %>').value;
			window.opener.ChangeDashlet(sSelectedItems, '');
			window.close();
		}
		else
		{
			alert('Original window has closed.  Dashlet cannot be assigned.');
		}
	}
	function Clear()
	{
		if ( window.opener != null && window.opener.ChangeDashlet != null )
		{
			window.opener.ChangeDashlet('', '');
			window.close();
		}
		else
		{
			alert('Original window has closed.  Dashlet cannot be assigned.');
		}
	}
	function Cancel()
	{
		window.close();
	}
	</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Home.LBL_ADD_DASHLETS" Runat="Server" />

	<asp:Button Text='<%# L10n.Term(".LBL_SELECT_CHECKED_BUTTON_LABEL") %>' UseSubmitBehavior="false" OnClientClick="SelectChecked(); return false;" runat="server" />
	<asp:Button Text='<%# L10n.Term(".LBL_DONE_BUTTON_LABEL"          ) %>' UseSubmitBehavior="false" OnClientClick="Cancel(); return false;"        runat="server" />

	<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" PageSize="100" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="2%">
				<ItemTemplate><%# grdMain.InputCheckbox(!PrintView && bMultiSelect, ctlCheckAll.FieldName, Sql.ToGuid(Eval("ID")), ctlCheckAll.SelectedItems) %></ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Dashlets.LBL_LIST_TITLE" ItemStyle-Width="25%">
				<ItemTemplate>
					<a id="DASHLET_ID_TITLE_<%# DataBinder.Eval(Container.DataItem, "ID") %>" class="listViewTdLinkS1" href="#" onclick="SelectDashlet('<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "ID"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "MODULE_NAME"))) %>');"><%# L10n.Term(Sql.ToString(DataBinder.Eval(Container.DataItem, "TITLE"))) %></a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Dashlets.LBL_LIST_MODULE_NAME" ItemStyle-Width="30%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%# L10n.Term(".moduleList.", Sql.ToString(DataBinder.Eval(Container.DataItem, "MODULE_NAME"))) %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn DataField="CONTROL_NAME" HeaderText="Dashlets.LBL_LIST_CONTROL_NAME" ItemStyle-Width="25%"></asp:BoundColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && bMultiSelect %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

