<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FacebookButtons.ascx.cs" Inherits="SplendidCRM.Users.FacebookButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="fb-root"></div>
<script type="text/javascript">
	window.fbAsyncInit = function()
	{
		FB.init(
		{
			appId: '<%= Application["CONFIG.facebook.AppID"] %>',
			status: true,
			cookie: true,
			xfbml: true
		});
	};
	
	(function()
	{
		var e = document.createElement('script');
		e.type = 'text/javascript';
		e.src = document.location.protocol + '//connect.facebook.net/en_US/all.js';
		e.async = true;
		document.getElementById('fb-root').appendChild(e);
	}());
	
	function FBlogin()
	{
		// http://developers.facebook.com/docs/authentication/permissions/
		FB.login
		(
			function(response)
			{
				FB.api('/me', function(response)
				{
					if ( response.id !== undefined )
					{
						var sID        = response.id;
						var sName      = (response.name       !== undefined) ? response.name       : '';
						var sFirstName = (response.first_name !== undefined) ? response.first_name : '';
						var sLastName  = (response.last_name  !== undefined) ? response.last_name  : '';
						var sLink      = (response.link       !== undefined) ? response.link       : '';
						var sBirthday  = (response.birthday   !== undefined) ? response.birthday   : '';
						var sGender    = (response.gender     !== undefined) ? response.gender     : '';
						var sEmail     = (response.email      !== undefined) ? response.email      : '';
						var sTimezone  = (response.timezone   !== undefined) ? response.timezone   : '';
						var sLocale    = (response.locale     !== undefined) ? response.locale     : '';
						var fldFACEBOOK_ID = document.getElementById('<%= sFACEBOOK_ID %>');
						if ( fldFACEBOOK_ID!= null )
						{
							fldFACEBOOK_ID.value = sID;
							fldFACEBOOK_ID.focus();
						}
					}
				});
			}
			, perms='email'
		);
	}
</script>
