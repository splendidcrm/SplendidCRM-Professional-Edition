<%@ Control CodeBehind="InviteesView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calls.InviteesView" %>
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
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchInvitees" Src="SearchInvitees.ascx" %>
	<SplendidCRM:SearchInvitees ID="ctlSearch" Runat="Server" />
	<br />
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<div id="divInvitees" visible="false" runat="server">
		<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="true" AllowSorting="true" EnableViewState="true" runat="server">
			<Columns>
				<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
					<ItemTemplate>
						<SplendidCRM:DynamicImage ID="DynamicImage1" ImageSkinID='<%# Eval("INVITEE_TYPE") %>' Runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn    HeaderText="Users.LBL_LIST_NAME"  DataField="NAME"  SortExpression="NAME"  ItemStyle-Width="15%" />
				<asp:TemplateColumn HeaderText="" ItemStyle-Width="50%">
					<ItemTemplate>
						<%@ Register TagPrefix="SplendidCRM" Tagname="UserSchedule" Src="~/Activities/UserSchedule.ascx" %>
						<SplendidCRM:UserSchedule ID="ctlUserSchedule" DATE_START='<%# dtDATE_START %>' DATE_END='<%# dtDATE_END %>' USER_ID='<%# Sql.ToString(Eval("INVITEE_TYPE")) == "Users" ? Sql.ToGuid(Eval("ID")) : Guid.Empty %>' Runat="Server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn    HeaderText="Users.LBL_LIST_EMAIL" DataField="EMAIL" SortExpression="EMAIL" ItemStyle-Width="15%" />
				<asp:BoundColumn    HeaderText="Users.LBL_LIST_PHONE_WORK" DataField="PHONE" SortExpression="PHONE" ItemStyle-Width="15%" />
				<asp:TemplateColumn HeaderText="" ItemStyle-Width="4%" ItemStyle-HorizontalAlign="Center">
					<ItemTemplate>
						<asp:Button ID="Button1" CommandName="Invitees.Add" OnCommand="Page_Command" CommandArgument='<%# Eval("ID") %>' CssClass="button" Text='<%# " " + L10n.Term("Meetings.LBL_ADD_BUTTON") + " " %>' ToolTip='<%# L10n.Term("Meetings.LBL_ADD_BUTTON") %>' Enabled='<%# !IsExistingInvitee(Sql.ToString(Eval("ID"))) %>' Runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</SplendidCRM:SplendidGrid>
	</div>
</div>

