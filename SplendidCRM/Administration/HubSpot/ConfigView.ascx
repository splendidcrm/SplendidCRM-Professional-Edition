<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.HubSpot.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
function OAuthTokenUpdate(code)
{
	var AUTHORIZATION_CODE = document.getElementById('<%= AUTHORIZATION_CODE.ClientID %>');
	AUTHORIZATION_CODE.value = code;
	document.getElementById('<%= btnGetAccessToken.ClientID %>').click();
}

function Authorize()
{
	var OAUTH_CLIENT_ID = document.getElementById('<%= OAUTH_CLIENT_ID.ClientID %>').value;
	var REDIRECT_URL    = '<%= Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Administration/HubSpot/OAuthLanding.aspx" %>';
	// 11/12/2019 Paul.  Update HubSpot OAuth. 
	// https://developers.hubspot.com/docs/methods/oauth2/initiate-oauth-integration
	// 09/26/2020 Paul.  integration-sync is causing a permissions error. 
	// Couldn't complete the connection  This hub doesn't have access to some HubSpot features that are required by this integration. Please contact the integrator.
	var authenticateUrl = 'https://app.hubspot.com/oauth/authorize?client_id=' + OAUTH_CLIENT_ID + '&redirect_uri=' + REDIRECT_URL + '&scope=contacts%20oauth'; //+'%20integration-sync';
	window.open(authenticateUrl, 'HubSpotPopup', 'width=830,height=830,status=1,toolbar=0,location=0,resizable=1');
	return false;
}
</script>
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="HubSpot" Title="HubSpot.LBL_HUBSPOT_SETTINGS" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />

	<asp:TextBox ID="AUTHORIZATION_CODE" style="display: none;" runat="server" />
	<asp:Button ID="btnGetAccessToken" OnCommand="Page_Command" CommandName="GetAccessToken" style="display: none;" Text="Get Access Token" runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Label Text='<%# L10n.Term("HubSpot.LBL_APP_INSTRUCTIONS").Replace("~/", Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"])) %>' runat="server" />
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_HUBSPOT_ENABLED") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="ENABLED" CssClass="checkbox" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_VERBOSE_STATUS") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="VERBOSE_STATUS" CssClass="checkbox" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_OAUTH_PORTAL_ID") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_PORTAL_ID" Size="15" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_OAUTH_ACCESS_TOKEN") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_ACCESS_TOKEN" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_OAUTH_CLIENT_ID") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_ID" Size="50" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_OAUTH_REFRESH_TOKEN") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_REFRESH_TOKEN" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_OAUTH_CLIENT_SECRET") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_SECRET" Size="50" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_OAUTH_EXPIRES_AT") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_EXPIRES_AT" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_DIRECTION") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="DIRECTION" DataValueField="NAME" DataTextField="DISPLAY_NAME" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_CONFLICT_RESOLUTION") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="CONFLICT_RESOLUTION" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("HubSpot.LBL_SYNC_MODULES") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="SYNC_MODULES" DataValueField="NAME" DataTextField="DISPLAY_NAME" runat="server" />
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
