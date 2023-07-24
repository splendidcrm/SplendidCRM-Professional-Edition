<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.ConstantContact.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="ConstantContact" Title="ConstantContact.LBL_CONSTANTCONTACT_SETTINGS" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="FormatDateJavaScript" Src="~/_controls/FormatDateJavaScript.ascx" %>
	<SplendidCRM:FormatDateJavaScript Runat="Server" />
	
	<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
	function OAuthTokenUpdate(code)
	{
		var AUTHORIZATION_CODE = document.getElementById('<%= AUTHORIZATION_CODE.ClientID %>');
		AUTHORIZATION_CODE.value = code;
		document.getElementById('<%= btnGetAccessToken.ClientID %>').click();
	}

	function Authorize()
	{
	// https://community.constantcontact.com/t5/Documentation/Authentication-using-OAuth-2-0-Server-and-Client-Flows/ba-p/38313
	// 11/11/2019 Paul.  ConstantContact v3 API. 
	// https://v3.developer.constantcontact.com/api_guide/server_flow.html
		var OAUTH_CLIENT_ID  = document.getElementById('<%= OAUTH_CLIENT_ID.ClientID %>').value;
		var REDIRECT_URL     = '<%= Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Administration/ConstantContact/OAuthLanding.aspx" %>';
		var authenticateUrl  = 'https://api.cc.email/v3/idfed?client_id=' + OAUTH_CLIENT_ID + '&redirect_uri=' + escape(REDIRECT_URL) + '&response_type=code&scope=contact_data+campaign_data';
		window.open(authenticateUrl, 'ConstantContactPopup', 'width=830,height=830,status=1,toolbar=0,location=0,resizable=1');
		return false;
	}
	</script>
	</SplendidCRM:InlineScript>
	<p></p>
	
	<asp:TextBox ID="AUTHORIZATION_CODE" style="display: none;" runat="server" />
	<asp:Button ID="btnGetAccessToken" OnCommand="Page_Command" CommandName="GetAccessToken" style="display: none;" Text="Get Access Token" runat="server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_APP_INSTRUCTIONS").Replace("~/", Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"])) %>' runat="server" />
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_CONSTANTCONTACT_ENABLED") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="ENABLED" CssClass="checkbox" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_VERBOSE_STATUS") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="VERBOSE_STATUS" CssClass="checkbox" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_OAUTH_CLIENT_ID") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_ID" Size="50" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_OAUTH_ACCESS_TOKEN") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_ACCESS_TOKEN" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_OAUTH_CLIENT_SECRET") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_CLIENT_SECRET" Size="50" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_OAUTH_REFRESH_TOKEN") %>' runat="server" />
							<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="OAUTH_REFRESH_TOKEN" Size="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_DIRECTION") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="DIRECTION" DataValueField="NAME" DataTextField="DISPLAY_NAME" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_CONFLICT_RESOLUTION") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="CONFLICT_RESOLUTION" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel" VerticalAlign="top">
							<asp:Label Text='<%# L10n.Term("ConstantContact.LBL_SYNC_MODULES") %>' runat="server" />
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
