<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.Google.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Google" Title="Google.LBL_MANAGE_GOOGLE_TITLE" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="FormatDateJavaScript" Src="~/_controls/FormatDateJavaScript.ascx" %>
	<SplendidCRM:FormatDateJavaScript ID="FormatDateJavaScript1" Runat="Server" />
	
	<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
	function OAuthTokenUpdate(access_token, oauth_verifier, realmId, refresh_token, expires_in)
	{
		//var expiresDate = new Date();
		//expiresDate = new Date(expiresDate.valueOf() + expires_in * 1000);
		//var sStatus = 'access_token = '   + access_token   + '\r\n'
		//            + 'oauth_verifier = ' + oauth_verifier + '\r\n'
		//            + 'realmId = '        + realmId        + '\r\n'
		//            + 'refresh_token = '  + refresh_token  + '\r\n'
		//            + 'expires_in = '     + expires_in     + '\r\n';
		//alert(sStatus);
		var divTestStatus = document.getElementById('divTestStatus');
		divTestStatus.innerHTML = '<%= L10n.Term("Google.LBL_TEST_SUCCESSFUL") %>';
	}

	function OAuthTokenError(error)
	{
		var divTestStatus = document.getElementById('divTestStatus');
		divTestStatus.innerHTML = '<%= L10n.Term("Google.LBL_TEST_FAILED") %>';
	}

	// https://console.developers.google.com
	// https://developers.google.com/oauthplayground/
	// https://developers.google.com/identity/protocols/OAuth2
	// https://developers.google.com/identity/protocols/OAuth2WebServer
	// https://developers.google.com/identity/protocols/OAuth2InstalledApp
	// https://developers.google.com/identity/protocols/OpenIDConnect#createxsrftoken
	function Authorize()
	{
		var divTestStatus = document.getElementById('divTestStatus');
		divTestStatus.innerHTML = '';
		var OAUTH_CLIENT_ID = document.getElementById('<%= OAUTH_CLIENT_ID.ClientID %>').value;
		window.open('<%= Application["rootURL"] %>GoogleOAuth/default.aspx?client_id=' + OAUTH_CLIENT_ID, 'GooglePopup', 'width=830,height=830,status=1,toolbar=0,location=0,resizable=1');
		return false;
	}

	function PUSH_NOTIFICATIONS_Clicked()
	{
		var fldPUSH_NOTIFICATIONS    = document.getElementById('<%= PUSH_NOTIFICATIONS   .ClientID %>');
		var fldPUSH_NOTIFICATION_URL = document.getElementById('<%= PUSH_NOTIFICATION_URL.ClientID %>');
		if ( fldPUSH_NOTIFICATIONS != null && fldPUSH_NOTIFICATION_URL != null )
		{
			if ( fldPUSH_NOTIFICATIONS.checked )
			{
				if ( fldPUSH_NOTIFICATION_URL.value == '' )
					fldPUSH_NOTIFICATION_URL.value = '<%= SplendidCRM.Crm.Config.SiteURL(Application) %>' + 'GoogleOAuth/Google_Webhook.aspx';
			}
		}
	}
	function TestPushURL()
	{
		var fldPUSH_NOTIFICATION_URL = document.getElementById('<%= PUSH_NOTIFICATION_URL.ClientID %>');
		if ( fldPUSH_NOTIFICATION_URL.value.length > 0 )
		{
			window.open(fldPUSH_NOTIFICATION_URL.value, 'ExchangePushURL');
		}
		return false;
	}
	</script>
	</SplendidCRM:InlineScript>

	<p></p>
	<asp:HiddenField ID="OAUTH_EXPIRES_IN" runat="server" />
	<div id="divTestStatus" class="error"></div>
	
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Label Text='<%# L10n.Term("Google.LBL_OAUTH_INSTRUCTIONS") %>' runat="server" />
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Google.LBL_GOOGLE_APPS_ENABLED") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="ENABLED" CssClass="checkbox" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Google.LBL_VERBOSE_STATUS") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="VERBOSE_STATUS" CssClass="checkbox" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Google.LBL_OAUTH_API_KEY") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="API_KEY" Size="50" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Google.LBL_OAUTH_CLIENT_ID") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_ID" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_PUSH_NOTIFICATIONS") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="PUSH_NOTIFICATIONS" CssClass="checkbox" onclick='PUSH_NOTIFICATIONS_Clicked();' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Google.LBL_OAUTH_CLIENT_SECRET") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_SECRET" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label ID="Label1" Text='<%# L10n.Term("Exchange.LBL_PUSH_NOTIFICATION_URL") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top" ColumnSpan="3">
							<asp:TextBox ID="PUSH_NOTIFICATION_URL" Size="80" Runat="server" />
							&nbsp;
							<asp:Button ID="Button1" Text='<%# L10n.Term("Exchange.LBL_TEST_URL") %>' ToolTip='<%# L10n.Term("Exchange.LBL_TEST_URL") %>' OnClientClick="return TestPushURL();" CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<p></p>
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>
