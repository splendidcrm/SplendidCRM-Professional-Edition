<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ExchangeLogin.ascx.cs" Inherits="SplendidCRM.Administration.Exchange.ExchangeLogin" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
function PUSH_NOTIFICATIONS_Clicked()
{
	var fldPUSH_NOTIFICATIONS    = document.getElementById('<%= PUSH_NOTIFICATIONS   .ClientID %>');
	var fldPUSH_NOTIFICATION_URL = document.getElementById('<%= PUSH_NOTIFICATION_URL.ClientID %>');
	if ( fldPUSH_NOTIFICATIONS != null && fldPUSH_NOTIFICATION_URL != null )
	{
		if ( fldPUSH_NOTIFICATIONS.checked )
		{
			if ( fldPUSH_NOTIFICATION_URL.value == '' )
				fldPUSH_NOTIFICATION_URL.value = '<%= SplendidCRM.Crm.Config.SiteURL(Application) %>' + 'ExchangeService2007.asmx';
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
function UseOffice365()
{
	var fldSERVER_URL = document.getElementById('<%= SERVER_URL.ClientID %>');
	// 02/10/2017 Paul.  https://outlook.office.com/EWS/Exchange.asmx does not work. 
	fldSERVER_URL.value = 'https://outlook.office365.com/EWS/Exchange.asmx';
	var fldAUTHENTICATION_METHOD_OAUTH = document.getElementById('<%= AUTHENTICATION_METHOD_OAUTH.ClientID %>');
	fldAUTHENTICATION_METHOD_OAUTH.click();
	return false;
}

function Office365OCodeUpdate(code)
{
	document.getElementById('<%= OAUTH_CODE.ClientID %>').value = code;
	var btnOffice365Authorized = document.getElementById('<%= btnOffice365Authorized.ClientID %>');
	btnOffice365Authorized.click();
}

function Office365OAuthTokenError(error)
{
	var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
	lblError.innerHTML = error;
}

// https://blogs.msdn.microsoft.com/exchangedev/2014/03/25/using-oauth2-to-access-calendar-contact-and-mail-api-in-office-365-exchange-online/
function Office365Authorize()
{
	var fldOAUTH_CLIENT_ID = document.getElementById('<%= OAUTH_CLIENT_ID.ClientID %>');
	// 02/10/2017 Paul.  One endpoint to rule them all does not work with ExchangeService. https://graph.microsoft.io/en-us/
	// 02/10/2017 Paul.  Use new endpoint. https://msdn.microsoft.com/en-us/office/office365/api/use-outlook-rest-api
	// 11/28/2020 Paul.  Outlook REST API has been deprecated.  Use Microsoft Graph instead. https://docs.microsoft.com/en-us/outlook/rest/compare-graph
	var client_id       = fldOAUTH_CLIENT_ID.value;
	var state           = '<%= Guid.NewGuid().ToString() %>';
	var redirect_url    = '<%= Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "OAuth/Office365Landing.aspx" %>';
	// 12/29/2020 Paul.  New set of resources for Office365 sync. 
	var response_type   = 'code';
	var scope           = '<%= Spring.Social.Office365.Office365Sync.scope %>';
	// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
	var fldOAUTH_DIRECTORY_TENANT_ID = document.getElementById('<%= OAUTH_DIRECTORY_TENANT_ID.ClientID %>');
	var tenant          = fldOAUTH_DIRECTORY_TENANT_ID.value;
	if ( tenant == '' )
		tenant = 'common';
	var authenticateUrl = 'https://login.microsoftonline.com/'+ tenant + '/oauth2/v2.0/authorize'
	                    + '?response_type=' + response_type
	                    + '&client_id='     + client_id
	                    + '&redirect_uri='  + encodeURIComponent(redirect_url)
	                    + '&scope='         + escape(scope)
	                    + '&state='         + state
	                    + '&response_mode=query';
	if ( client_id == '' )
	{
		Office365OAuthTokenError('<%= L10n.Term("Exchange.ERR_OAUTH_CLIENT_ID_REQUIRED") %>');
	}
	else
	{
		window.open(authenticateUrl, 'Office365AuthorizePopup', 'width=830,height=830,status=1,toolbar=0,location=0,resizable=1');
	}
	return false;
}
</script>
<asp:HiddenField ID="OAUTH_CODE" runat="server" />
<asp:Button ID="btnOffice365Authorized" CommandName="Exchange.Authorize" OnCommand="Page_Command" Text="Exchange Authorized"  style="display: none;" runat="server" />

<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="Exchange" Title="Exchange.LBL_EXCHANGE_SETTINGS" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" Width="100%" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_SERVER_URL") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top" ColumnSpan="3">
							<asp:TextBox ID="SERVER_URL" Size="80" Runat="server" />
							&nbsp;
							<asp:Button Text='<%# L10n.Term("Exchange.LBL_USE_OFFICE365") %>' ToolTip='<%# L10n.Term("Exchange.LBL_USE_OFFICE365") %>' OnClientClick="return UseOffice365();" CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_AUTHENTICATION_METHOD") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:RadioButton ID="AUTHENTICATION_METHOD_OAUTH"    Text='<%# L10n.Term("Exchange.LBL_AUTHENTICATION_METHOD_OAUTH") %>'    GroupName="AUTHENTICATION_METHOD" OnCheckedChanged="AUTHENTICATION_METHOD_OAUTH_CheckedChanged" AutoPostBack="true" runat="server" />
							&nbsp;
							<asp:RadioButton ID="AUTHENTICATION_METHOD_USERNAME" Text='<%# L10n.Term("Exchange.LBL_AUTHENTICATION_METHOD_USERNAME") %>' GroupName="AUTHENTICATION_METHOD" OnCheckedChanged="AUTHENTICATION_METHOD_USERNAME_CheckedChanged" AutoPostBack="true" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label ID="LBL_OAUTH_DIRECTORY_TENANT_ID" Text='<%# L10n.Term("Exchange.LBL_OAUTH_DIRECTORY_TENANT_ID") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_DIRECTORY_TENANT_ID" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow ID="trAUTHENTICATION_METHOD_OAUTH">
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_OAUTH_CLIENT_ID") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_ID" Size="50" Runat="server" />
							<asp:RequiredFieldValidator ID="reqOAUTH_CLIENT_ID" ControlToValidate="OAUTH_CLIENT_ID" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_OAUTH_CLIENT_SECRET") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_SECRET" Size="50" Runat="server" />
							<asp:RequiredFieldValidator ID="reqOAUTH_CLIENT_SECRET" ControlToValidate="OAUTH_CLIENT_SECRET" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow ID="trAUTHENTICATION_METHOD_USERNAME">
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_USER_NAME") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="USER_NAME" Size="50" Runat="server" />
							<asp:RequiredFieldValidator ID="reqUSER_NAME" ControlToValidate="USER_NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_PASSWORD") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="PASSWORD" Size="50" TextMode="Password" Runat="server" />
							<asp:RequiredFieldValidator ID="reqPASSWORD" ControlToValidate="PASSWORD" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_IGNORE_CERTIFICATE") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="IGNORE_CERTIFICATE" CssClass="checkbox" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_IMPERSONATED_TYPE") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="IMPERSONATED_TYPE" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_INBOX_ROOT") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="INBOX_ROOT" CssClass="checkbox" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_SENT_ITEMS_ROOT") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="SENT_ITEMS_ROOT" CssClass="checkbox" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_PUSH_NOTIFICATIONS") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="PUSH_NOTIFICATIONS" CssClass="checkbox" onclick='PUSH_NOTIFICATIONS_Clicked();' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_SENT_ITEMS_SYNC") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="SENT_ITEMS_SYNC" CssClass="checkbox" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_PUSH_FREQUENCY") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="PUSH_FREQUENCY" Size="10" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_INBOX_SYNC") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="INBOX_SYNC" CssClass="checkbox" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("Exchange.LBL_PUSH_NOTIFICATION_URL") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top" ColumnSpan="3">
							<asp:TextBox ID="PUSH_NOTIFICATION_URL" Size="80" Runat="server" />
							&nbsp;
							<asp:Button Text='<%# L10n.Term("Exchange.LBL_TEST_URL") %>' ToolTip='<%# L10n.Term("Exchange.LBL_TEST_URL") %>' OnClientClick="return TestPushURL();" CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />
</div>
