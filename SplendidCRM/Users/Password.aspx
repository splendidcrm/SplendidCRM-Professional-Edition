<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Password.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Users.Password" %>
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
<script type="text/javascript">
function ChangePassword()
{
	var txtIS_ADMIN         = document.getElementById('<%= txtIS_ADMIN.ClientID         %>');
	var txtOLD_PASSWORD     = document.getElementById('<%= txtOLD_PASSWORD.ClientID     %>');
	var txtNEW_PASSWORD     = document.getElementById('<%= txtNEW_PASSWORD.ClientID     %>');
	var txtCONFIRM_PASSWORD = document.getElementById('<%= txtCONFIRM_PASSWORD.ClientID %>');
	/*
	// 02/13/2009 Paul.  We need to allow a user with a blank password to change his password. 
	if ( txtIS_ADMIN.value == 0 && txtOLD_PASSWORD.value == '' )
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_ENTER_OLD_PASSWORD") %>');
		return false;
	}
	*/
	if ( txtNEW_PASSWORD.value == '' )
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_ENTER_NEW_PASSWORD") %>');
		return false;
	}
	if ( txtCONFIRM_PASSWORD.value == '' )
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_ENTER_CONFIRMATION_PASSWORD") %>');
		return false;
	}

	if ( txtNEW_PASSWORD.value == txtCONFIRM_PASSWORD.value )
	{
		if ( window.opener != null && window.opener.ChangePassword != null )
		{
			window.opener.ChangePassword(txtOLD_PASSWORD.value, txtNEW_PASSWORD.value, txtCONFIRM_PASSWORD.value);
			window.close();
		}
		return true;
	}
	else
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_REENTER_PASSWORDS") %>');
		return false;
	}
	return false;
}
function Cancel()
{
	window.close();
	return false;
}

// 08/30/2006 Paul.  Fix onload to support Firefox. 
window.onload = function()
{
	var txtIS_ADMIN         = document.getElementById('<%= txtIS_ADMIN.ClientID         %>');
	var txtOLD_PASSWORD     = document.getElementById('<%= txtOLD_PASSWORD.ClientID     %>');
	var txtNEW_PASSWORD     = document.getElementById('<%= txtNEW_PASSWORD.ClientID     %>');
	if ( txtIS_ADMIN.value == 0 )
		txtOLD_PASSWORD.focus();
	else
		txtNEW_PASSWORD.focus();
}
</script>
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader Title="Users.LBL_CHANGE_PASSWORD" Runat="Server" />
<br />
<asp:HiddenField ID="txtIS_ADMIN" Value='<%# (SplendidCRM.Security.AdminUserAccess("Users", "edit") >= 0) ? "1" : "0" %>' runat="server" />
<asp:Table Width="100%" runat="server">
	<asp:TableRow>
		<asp:TableCell></asp:TableCell>
		<asp:TableCell><asp:Label ID="lblPasswordHelp" CssClass="error" EnableViewState="false" runat="server" />&nbsp;</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow style='<%# (SplendidCRM.Security.AdminUserAccess("Users", "edit") >= 0) ? "display:none" : "display:inline" %>'>
		<asp:TableCell Width="40%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Users.LBL_OLD_PASSWORD") %>' runat="server" /></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField"><asp:TextBox ID="txtOLD_PASSWORD"     TextMode="Password" TabIndex="1" size="25" MaxLength="50" CssClass="dataField" Runat="server" /></asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Width="40%" CssClass="dataLabel" Wrap="false"><asp:Label Text='<%# L10n.Term("Users.LBL_NEW_PASSWORD") %>' runat="server" /></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField">
			<asp:TextBox ID="txtNEW_PASSWORD" TextMode="Password" TabIndex="2" size="25" MaxLength="50" CssClass="dataField" autocomplete="off" Runat="server" />
			<SplendidCRM:SplendidPassword ID="ctlNEW_PASSWORD_STRENGTH" TargetControlID="txtNEW_PASSWORD" HelpStatusLabelID="lblPasswordHelp" 
				DisplayPosition="RightSide" StrengthIndicatorType="Text" TextCssClass="error" HelpHandleCssClass="PasswordHelp" HelpHandlePosition="LeftSide" runat="server" />
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Width="40%" CssClass="dataLabel" Wrap="false"><asp:Label Text='<%# L10n.Term("Users.LBL_CONFIRM_PASSWORD") %>' runat="server" /></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField">
			<asp:TextBox ID="txtCONFIRM_PASSWORD" TextMode="Password" TabIndex="3" size="25" MaxLength="50" CssClass="dataField" Runat="server" />
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Width="40%" CssClass="dataLabel"></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField"></asp:TableCell>
	</asp:TableRow>
</asp:Table>
<br />
<asp:Table Width="100%" runat="server">
	<asp:TableRow>
		<asp:TableCell HorizontalAlign="Center">
			<asp:Button OnClientClick="return ChangePassword();" CssClass="button" TabIndex="4" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"  ) %>' runat="server" />
			&nbsp;
			<asp:Button OnClientClick="return Cancel(); "        CssClass="button" TabIndex="5" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
</asp:Content>

