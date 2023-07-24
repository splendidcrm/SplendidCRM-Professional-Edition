<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Office365Landing.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.OAuth.Office365Landing" %>
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


window.onload = function()
{
	var divDebug = document.getElementById('divDebug');
	divDebug.innerHTML = window.location.href;
	try
	{
		var code  = getQuerystring('code' );
		var error = getQuerystring('error');
		if ( window.opener == null || window.opener.Office365OCodeUpdate == null )
		{
			// 02/09/2017 Paul.  May need to disable the IE Protected Mode.  It is on Security panel and for both Internet and Local Intranet. 
			divDebug.innerHTML += '<p>Office365OCodeUpdate does not exist.  Please close window and try again.';
		}
		else if ( error !== undefined && error !== '' )
		{
			var error_description = getQuerystring('error_description');
			window.opener.Office365OAuthTokenError(error + ': ' + error_description);
			window.close();
		}
		else if ( code !== undefined )
		{
			window.opener.Office365OCodeUpdate(code);
			window.close();
		}
		else
		{
			window.opener.Office365OAuthTokenError('Missing code parameter in query string: ' + window.location.href);
			window.close();
		}
	}
	catch(e)
	{
		divDebug.innerHTML += '<p>' + e.message;
	}
}
</script>
<div id="divDebug"></div>
</asp:Content>
