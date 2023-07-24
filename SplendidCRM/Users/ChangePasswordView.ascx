<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ChangePasswordView.ascx.cs" Inherits="SplendidCRM.Users.ChangePasswordView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
window.onload = function()
{
	set_focus();
	try
	{
		// 01/08/2008 Paul.  showLeftCol does not exist on the mobile master page. 
		showLeftCol(false, false);
	}
	catch(e)
	{
	}
}
function set_focus()
{
	var txtUSER_NAME = document.getElementById('<%= txtUSER_NAME.ClientID %>');
	txtUSER_NAME.focus();
}
</script>
<div id="divChangePassword" class="loginForm">
	<div style="height: 80px;" runat="server" />
	<asp:Table HorizontalAlign="Center" CellPadding="0" CellSpacing="0" CssClass="LoginActionsShadingTable" style="width: 450px;" runat="server">
		<asp:TableRow>
			<asp:TableCell ColumnSpan="3" CssClass="LoginActionsShadingHorizontal" />
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="LoginActionsShadingVertical" />
			<asp:TableCell>
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" HorizontalAlign="Center" CssClass="LoginActionsInnerTable" runat="server">
					<asp:TableRow>
						<asp:TableCell style="padding-top: 20px; padding-bottom: 20px; padding-left: 40px; padding-right: 40px;">
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" runat="server">
								<asp:TableRow runat="server">
									<asp:TableCell style="font-family: Arial; font-size: 14pt; font-weight: bold; color: #003564;">
										SplendidCRM <%# Application["CONFIG.service_level"] %>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" Runat="server">
								<asp:TableRow>
									<asp:TableCell style="font-size: 12px; padding-top: 5px;"><asp:Label ID="lblInstructions" Text='<%# L10n.Term(".NTC_LOGIN_MESSAGE") %>' CssClass="loginInstructions" Runat="server" /></asp:TableCell>
									<asp:TableCell><asp:Label ID="lblPasswordHelp" CssClass="error" EnableViewState="false" runat="server" />&nbsp;</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow ID="trError" Visible="false" runat="server">
									<asp:TableCell ColumnSpan="2">
										<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell Width="40%" CssClass="dataLabel"><%# L10n.Term("Users.LBL_USER_NAME") %></asp:TableCell>
									<asp:TableCell Width="60%" CssClass="loginField">
										<asp:TextBox ID="txtUSER_NAME" size="20" style="width: 140px" placeholder='<%# Sql.ToString(Application["CONFIG.default_theme"]) == "Arctic" ? L10n.Term("Users.LBL_USER_NAME").Replace(":", "") : String.Empty %>' Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell Width="40%" CssClass="dataLabel"><%# L10n.Term("Users.LBL_NEW_PASSWORD") %></asp:TableCell>
									<asp:TableCell Width="60%" CssClass="loginField">
										<asp:TextBox ID="txtNEW_PASSWORD" size="20" style="width: 140px" TextMode="Password" placeholder='<%# Sql.ToString(Application["CONFIG.default_theme"]) == "Arctic" ? L10n.Term("Users.LBL_NEW_PASSWORD").Replace(":", "") : String.Empty %>' Runat="server" />
										<SplendidCRM:SplendidPassword ID="ctlNEW_PASSWORD_STRENGTH" TargetControlID="txtNEW_PASSWORD" HelpStatusLabelID="lblPasswordHelp" 
											DisplayPosition="RightSide" StrengthIndicatorType="Text" TextCssClass="error" HelpHandleCssClass="PasswordHelp" HelpHandlePosition="LeftSide" runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell Width="40%" CssClass="dataLabel"><%# L10n.Term("Users.LBL_CONFIRM_PASSWORD") %></asp:TableCell>
									<asp:TableCell Width="60%" CssClass="loginField">
										<asp:TextBox ID="txtCONFIRM_PASSWORD" size="20" style="width: 140px" TextMode="Password" placeholder='<%# Sql.ToString(Application["CONFIG.default_theme"]) == "Arctic" ? L10n.Term("Users.LBL_CONFIRM_PASSWORD").Replace(":", "") : String.Empty %>' Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" Runat="server">
								<asp:TableRow>
									<asp:TableCell Width="40%">&nbsp;</asp:TableCell>
									<asp:TableCell Width="60%">
										<asp:Button ID="btnLogin" CommandName="Login" OnCommand="Page_Command" CssClass="button" Text='<%# " "  + L10n.Term("Users.LBL_LOGIN_BUTTON_LABEL") + " "  %>' ToolTip='<%# L10n.Term("Users.LBL_LOGIN_BUTTON_TITLE") %>' Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
			<asp:TableCell CssClass="LoginActionsShadingVertical" />
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell ColumnSpan="3" CssClass="LoginActionsShadingHorizontal" />
		</asp:TableRow>
	</asp:Table>
	<br />
	<br />
<%
Response.Write(Utils.RegisterEnterKeyPress(txtUSER_NAME.ClientID       , btnLogin.ClientID));
Response.Write(Utils.RegisterEnterKeyPress(txtNEW_PASSWORD.ClientID    , btnLogin.ClientID));
Response.Write(Utils.RegisterEnterKeyPress(txtCONFIRM_PASSWORD.ClientID, btnLogin.ClientID));
%>
</div>

