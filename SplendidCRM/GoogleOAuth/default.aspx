<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.GoogleOAuth.Default" %>
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
var access_token   = '<%# sAccessToken      %>';
var oauth_verifier = '<%# sTokenType        %>';
var realmId        = null;
var refresh_token  = '<%# sRefreshToken     %>';
var expires_in     = '<%# sExpiresInSeconds %>';

function getQuerystring(key, default_)
{
	if ( default_ == null || typeof(default_) == 'undefined' )
		default_ = '';
	key = key.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
	// 04/13/2012 Paul.  For some odd reason, facebook is using # and not ? as the separator. 
	var regex = new RegExp("[\\?&#]"+key+"=([^&#]*)");
	var qs = regex.exec(window.location.href);
	if ( qs == null )
		return default_;
	else
		return qs[1];
}

// https://console.developers.google.com
// https://developers.google.com/oauthplayground/
// https://developers.google.com/identity/protocols/OAuth2
// https://developers.google.com/identity/protocols/OAuth2WebServer
// https://developers.google.com/identity/protocols/OAuth2InstalledApp
// https://developers.google.com/identity/protocols/OpenIDConnect#createxsrftoken
window.onload = function()
{
	var divDebug = document.getElementById('divDebug');
	//divDebug.innerHTML += window.location.href;
	try
	{
		var client_id = getQuerystring('client_id');
		var error     = getQuerystring('error');
		var code      = getQuerystring('code');
		if ( client_id.length > 0 )
		{
			var redirect_url    = '<%= Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "GoogleOAuth/" %>';
			var access_type     = 'offline';
			var approval_prompt = 'force';
			var response_type   = 'code';
			var scope           = 'profile';
			scope              += escape(' https://www.googleapis.com/auth/calendar');
			scope              += escape(' https://www.googleapis.com/auth/tasks'   );
			scope              += escape(' https://mail.google.com/'                );
			scope              += escape(' https://www.google.com/m8/feeds'         );
			var authenticateUrl = 'https://accounts.google.com/o/oauth2/auth'
			                    + '?client_id='       + client_id
			                    + '&access_type='     + access_type
			                    + '&approval_prompt=' + approval_prompt
			                    + '&response_type='   + response_type
			                    + '&redirect_uri='    + redirect_url
			                    + '&scope='           + scope
			                    ;
			window.location.href = authenticateUrl;
		}
		else if ( error.length > 0 )
		{
			divDebug.innerHTML = error;
			window.opener.OAuthTokenError(error);
			window.close();
		}
		else if ( code.length > 0 && access_token.length > 0 )
		{
			//divDebug.innerHTML = code;
			// 02/03/2017 Paul.  IE11 is getting stuck due to Protected Mode for Security / Internet service. window.opener === undefined after return from Google URL. 
			if ( window.opener === undefined || window.opener == null )
			{
				divDebug.innerHTML += 'window.opener is undefined.  OAuth protocol will not work.  This could be a problem with IE11 and Protected Mode.';
			}
			else
			{
				window.opener.OAuthTokenUpdate(access_token, oauth_verifier, realmId, refresh_token, expires_in);
				window.close();
			}
		}
		//var oauth_token    = getQuerystring('oauth_token'   );
		//var oauth_verifier = getQuerystring('oauth_verifier');
		//var access_token   = getQuerystring('access_token'  );
		//var instance_url   = getQuerystring('instance_url'  );
		//if ( access_token != '' )
		//	oauth_token = decodeURIComponent(access_token);
		//if ( instance_url != '' )
		//	oauth_verifier = decodeURIComponent(instance_url);
		//// 06/03/2014 Paul.  Extract the QuickBooks realmId (same as Company ID). 
		//var realmId   = getQuerystring('realmId');
		//// 04/23/2015 Paul.  HubSpot has more data. 
		//var refresh_token  = getQuerystring('refresh_token' );
		//var expires_in     = getQuerystring('expires_in'    );
		//if ( refresh_token != '' )
		//	refresh_token = decodeURIComponent(refresh_token);
		//
		//window.opener.OAuthTokenUpdate(oauth_token, oauth_verifier, realmId, refresh_token, expires_in);
		//window.close();
	}
	catch(e)
	{
		divDebug.innerHTML += '<p>' + e.message;
	}
}
</script>
<asp:Label ID="lblError" CssClass="error" runat="server" />
<div id="divDebug"></div>
</asp:Content>
