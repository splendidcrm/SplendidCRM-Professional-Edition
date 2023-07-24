<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LoginView.ascx.cs" Inherits="SplendidCRM.Users.LoginView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	var user_name     = document.getElementById('<%= txtUSER_NAME.ClientID %>');
	var user_password = document.getElementById('<%= txtPASSWORD.ClientID  %>');
	if ( user_name != null )
	{
		try
		{
			if ( user_name.value != '' && user_password != null )
			{
				user_password.focus();
				user_password.select();
			}
			else
			{
				user_name.focus();
			}
		}
		catch(e)
		{
		}
	}
}
</script>
<div id="divLoginView" class="loginForm">
	<div Visible="<%# !this.IsMobile %>" style="height: 80px;" runat="server" />
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
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" CssClass="loginAppName" runat="server">
								<asp:TableRow runat="server">
									<asp:TableCell style="font-family: Arial; font-size: 14pt; font-weight: bold;">
										SplendidCRM <%# Application["CONFIG.service_level"] %>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table ID="tblUser" Visible="<%# !Security.IsWindowsAuthentication() %>" Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" Runat="server">
								<asp:TableRow>
									<asp:TableCell ColumnSpan="2" style="font-size: 12px; padding-top: 5px;">
										<asp:Label ID="lblInstructions" Visible="<%# !Security.IsWindowsAuthentication() %>" Text='<%# L10n.Term(".NTC_LOGIN_MESSAGE") %>' CssClass="loginInstructions" Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow ID="trError" Visible="false" runat="server">
									<asp:TableCell ColumnSpan="2">
										<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow ID="trUserName" runat="server">
									<asp:TableCell Width="30%" CssClass="dataLabel"><%# L10n.Term("Users.LBL_USER_NAME") %></asp:TableCell>
									<asp:TableCell Width="70%" CssClass="loginField" Wrap="true">
										<asp:TextBox ID="txtUSER_NAME" placeholder='<%# (sTheme == "Arctic" || sTheme == "Pacific") ? L10n.Term("Users.LBL_USER_NAME").Replace(":", "") : String.Empty %>' Runat="server" />
									</asp:TableCell>
									<asp:TableCell Wrap="false" style="color: black" HorizontalAlign="Right" Visible='<%# !Sql.IsEmptyString(Application["CONFIG.default_user_name"]) %>'>
										<%# (Sql.IsEmptyString(Application["CONFIG.default_user_name"]) ? String.Empty : "(" + Sql.ToString(Application["CONFIG.default_user_name"]) + ")") %>
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow ID="trPassword" runat="server">
									<asp:TableCell Width="30%" CssClass="dataLabel"><%# L10n.Term("Users.LBL_PASSWORD") %></asp:TableCell>
									<asp:TableCell Width="70%" CssClass="loginField" Wrap="true">
										<asp:TextBox ID="txtPASSWORD" TextMode="Password" placeholder='<%# (sTheme == "Arctic" || sTheme == "Pacific") ? L10n.Term("Users.LBL_PASSWORD").Replace(":", "") : String.Empty %>' Runat="server" />
									</asp:TableCell>
									<asp:TableCell Wrap="false" style="color: black" HorizontalAlign="Right" Visible='<%# !Sql.IsEmptyString(Application["CONFIG.default_password"]) %>'>
										<%# (Sql.IsEmptyString(Application["CONFIG.default_password"]) ? String.Empty : "(" + Sql.ToString(Application["CONFIG.default_password"]) + ")") %>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" runat="server">
								<asp:TableRow>
									<asp:TableCell Width="30%" Visible='<%# (sTheme != "Arctic" && sTheme != "Pacific") %>'>&nbsp;</asp:TableCell>
									<asp:TableCell Width="70%" Wrap="false">
										<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" runat="server">
											<asp:TableRow>
												<asp:TableCell HorizontalAlign="Left">
													<asp:Button ID="btnLogin" CommandName="Login" OnCommand="Page_Command" CssClass="button" Text='<%# " "  + L10n.Term("Users.LBL_LOGIN_BUTTON_LABEL") + " "  %>' ToolTip='<%# L10n.Term("Users.LBL_LOGIN_BUTTON_TITLE") %>' Runat="server" />
												</asp:TableCell>
												<asp:TableCell>
													<%@ Register TagPrefix="SplendidCRM" Tagname="FacebookLogin" Src="FacebookLogin.ascx" %>
													<SplendidCRM:FacebookLogin ID="ctlFacebookLogin" Runat="Server" />
												</asp:TableCell>
											</asp:TableRow>
										</asp:Table>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" style="padding-top: 10px;" runat="server">
								<asp:TableRow Visible=<%# !Security.IsWindowsAuthentication() && !Utils.CachedFileExists(Context, "~/Users/ClientLogin.aspx") %> runat="server">
									<asp:TableCell ID="trShowForgotPassword">
										<asp:HyperLink NavigateUrl=<%# "javascript:document.getElementById('" + txtFORGOT_USER_NAME.ClientID + "').value = document.getElementById('" + txtUSER_NAME.ClientID + "').value; toggleDisplay('" + pnlForgotPassword.ClientID + "');" %> CssClass="utilsLink" runat="server">
											<asp:Image SkinID="advanced_search" runat="server" />&nbsp;<asp:Label Text='<%# L10n.Term("Users.LBL_FORGOT_PASSWORD") %>' runat="server" />
										</asp:HyperLink>
									</asp:TableCell>
									<asp:TableCell HorizontalAlign="Right">
										<asp:HyperLink ID="lnkWorkOnline"  Text='<%# L10n.Term("Offline.LNK_WORK_ONLINE"  ) %>' NavigateUrl="~/Users/ClientLogin.aspx" Visible="false" runat="server" />
										<asp:HyperLink ID="lnkHTML5Client" Text='<%# L10n.Term(".LNK_MOBILE_CLIENT"       ) %>' NavigateUrl="~/html5/default.aspx"     Visible="false" runat="server" />
										<asp:HyperLink ID="lnkReactClient" Text='<%# L10n.Term(".LNK_REACT_CLIENT"        ) %>' NavigateUrl="~/React/default.aspx"     Visible="false" runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Panel ID="pnlForgotPassword" Visible=<%# !Security.IsWindowsAuthentication() && !Utils.CachedFileExists(Context, "~/Users/ClientLogin.aspx") %> style="display:none" runat="server">
								<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" Runat="server">
									<asp:TableRow ID="trForgotError" Visible="false" runat="server">
										<asp:TableCell ColumnSpan="2">
											<asp:Label ID="lblForgotError" CssClass="error" EnableViewState="false" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Width="30%" CssClass="dataLabel"><%# L10n.Term("Users.LBL_USER_NAME") %></asp:TableCell>
										<asp:TableCell Width="70%" CssClass="loginField">
											<asp:TextBox ID="txtFORGOT_USER_NAME" placeholder='<%# (sTheme == "Arctic" || sTheme == "Pacific") ? L10n.Term("Users.LBL_USER_NAME").Replace(":", "") : String.Empty %>' Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Width="30%" CssClass="dataLabel"><%# L10n.Term("Users.LBL_EMAIL") %></asp:TableCell>
										<asp:TableCell Width="70%" CssClass="loginField">
											<asp:TextBox ID="txtFORGOT_EMAIL" placeholder='<%# (sTheme == "Arctic" || sTheme == "Pacific") ? L10n.Term("Users.LBL_EMAIL").Replace(":", "") : String.Empty %>' Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
								<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" runat="server">
									<asp:TableRow>
										<asp:TableCell Width="30%" Visible='<%# (sTheme != "Arctic" && sTheme != "Pacific") %>'>&nbsp;</asp:TableCell>
										<asp:TableCell Width="70%">
											<asp:Button ID="btnForgotPassword" CommandName="ForgotPassword" OnCommand="Page_Command" CssClass="button" Text='<%# " "  + L10n.Term(".LBL_SUBMIT_BUTTON_LABEL") + " "  %>' ToolTip='<%# L10n.Term(".LBL_SUBMIT_BUTTON_TITLE") %>' Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
							</asp:Panel>
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
if ( tblUser.Visible )
{
	Response.Write(Utils.RegisterEnterKeyPress(txtUSER_NAME.ClientID, btnLogin.ClientID));
	Response.Write(Utils.RegisterEnterKeyPress(txtPASSWORD.ClientID , btnLogin.ClientID));
}
if ( pnlForgotPassword.Visible )
{
	Response.Write(Utils.RegisterEnterKeyPress(txtFORGOT_USER_NAME.ClientID, btnForgotPassword.ClientID));
	Response.Write(Utils.RegisterEnterKeyPress(txtFORGOT_EMAIL.ClientID , btnForgotPassword.ClientID));
}
%>
</div>

