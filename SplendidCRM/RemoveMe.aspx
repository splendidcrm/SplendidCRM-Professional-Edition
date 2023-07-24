<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="RemoveMe.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.RemoveMe" %>
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
<asp:Content ID="cntLastViewed" ContentPlaceHolderID="cntLastViewed" runat="server" />
<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server" />

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<div style="padding-top: 50px;"></div>
	<asp:Literal ID="litREMOVE_ME_HEADER" runat="server" />
	
	<asp:RadioButtonList ID="radREASON" Visible="false" DataValueField="NAME" DataTextField="DISPLAY_NAME" CssClass="radio" runat="server" />
	<asp:Button ID="btnSubmit" Visible="false" UseSubmitBehavior="false" CommandName="Submit" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SUBMIT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SUBMIT_BUTTON_TITLE") %>' Runat="server" />
	&nbsp;<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	&nbsp;<asp:Label ID="lblWarning" CssClass="error" EnableViewState="false" Runat="server" />
	
	<asp:Literal ID="litREMOVE_ME_FOOTER" runat="server" />
</asp:Content>

