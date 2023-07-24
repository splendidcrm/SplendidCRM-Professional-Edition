<%@ Control CodeBehind="Mailbox.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.InboundEmail.Mailbox" %>
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
<div id="divProductTemplatesProductTemplates">
	<br />
	<%-- 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="SubPanelButtons" Src="~/_controls/SubPanelButtons.ascx" %>
	<SplendidCRM:SubPanelButtons ID="ctlDynamicButtons" Module="InboundEmail" Title="InboundEmail.LBL_MAILBOX_DEFAULT" Runat="Server" />

	<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="true" AllowSorting="true" EnableViewState="true" AutoGenerateColumns="false" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="EmailClient.LBL_LIST_FROM" SortExpression="From" ItemStyle-VerticalAlign="Top" ItemStyle-Width="15%" >
				<ItemTemplate>
					<%# Eval("From") %>
					<span Visible='<%# Sql.ToString(Eval("From")) != Sql.ToString(Eval("Sender")) %>' runat="server"><br /><%# Eval("Sender") %></span>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_TO"            DataField="To"           SortExpression="To"           ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_CC"            DataField="CC"           SortExpression="CC"           ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_SUBJECT"       DataField="Subject"      SortExpression="Subject"      ItemStyle-VerticalAlign="Top" ItemStyle-Width="25%" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_DATE_RECEIVED" DataField="DeliveryDate" SortExpression="DeliveryDate" ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_SIZE"          DataField="Size"         SortExpression="Size"         ItemStyle-VerticalAlign="Top" ItemStyle-Width="5%" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_HEADERS"       DataField="Headers"      SortExpression="Headers"      ItemStyle-VerticalAlign="Top" ItemStyle-Width="25%" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_PRIORITY"      DataField="Priority"     SortExpression="Priority"     ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%" Visible="false" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_BCC"           DataField="Bcc"          SortExpression="Bcc"          ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%" Visible="false" />
			<asp:BoundColumn HeaderText="EmailClient.LBL_LIST_MESSAGEID"     DataField="MessageID"    SortExpression="MessageID"    ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%" Visible="false" />
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

