<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MetaHeader.ascx.cs" Inherits="SplendidCRM._controls.MetaHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	<!-- 02/01/2018 Paul.  Prevent IE compatibility mode from disabling HTML5. -->
	<!-- 02/14/2018 Paul.  Moved to MetaHeader control so that it is used everywhere. -->
	<meta charset="utf-8">
	<meta http-equiv="X-UA-Compatible" content="IE=edge">
	<meta name="keywords" content="<%= Application["CONFIG.header_keywords"] %>" />
	<link rel="SHORTCUT ICON" href="<%= Application["imageURL"] %>SplendidCRM_Icon.ico" />
	<!-- 01/24/2018 Paul.  Include version in url to ensure updates of combined files. -->
	<script type="text/javascript" src="<%= Application["rootURL"] %>HeaderScriptsCombined<%= "_" + Sql.ToString(Application["SplendidVersion"]) %>"></script>

<script type="text/javascript">
// 05/08/2010 Paul.  Move onkeypress to SplendidCRM.js. 
// 05/08/2010 Paul.  Create rootURL javascript variable for use by the CalendarPopup. 
// This will allow us to remove the CalendarPopup definitions in the Admin area. 
// 08/25/2013 Paul.  Move sREMOTE_SERVER definition to the master pages. 
// 09/07/2013 Paul.  Change rootURL to sREMOTE_SERVER to match Survey module. 
// 09/20/2013 Paul.  Move EXTENSION to the main table. 
// 09/27/2013 Paul.  SMS messages need to be opt-in. 
// 11/10/2014 Paul.  Add Chat Channels. 
// 06/29/2017 Paul.  AssemblyVersion is needed for HTML5 Dashboard. 
var sAssemblyVersion     = '<%# Sql.ToString(Application["SplendidVersion"]) %>';
var sREMOTE_SERVER       = '<%# Application["rootURL"] %>';
// 04/23/2018 Paul.  Build in javascript to allow proxy handling. 
sREMOTE_SERVER           = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port: '') + '<%# Sql.ToString(Application["rootURL"]) %>';
var sUSER_EXTENSION      = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["EXTENSION"   ])) %>';
var sUSER_FULL_NAME      = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["FULL_NAME"   ])) %>';
var sUSER_PHONE_WORK     = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["PHONE_WORK"  ])) %>';
var sUSER_SMS_OPT_IN     = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["SMS_OPT_IN"  ])) %>';
var sUSER_PHONE_MOBILE   = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["PHONE_MOBILE"])) %>';
var sUSER_TWITTER_TRACKS = '';
var sUSER_CHAT_CHANNELS  = '';
// 09/09/2020 Paul.  Add PhoneBurner SignalR support. 
var sUSER_PHONE_BURNER_GROUP = '<%# (Security.IsAuthenticated() && Sql.ToBoolean(Application["CONFIG.PhoneBurner.Enabled"]) && !Sql.IsEmptyString(Application["CONFIG.PhoneBurner." + Security.USER_ID.ToString() + ".OAuthAccessToken"])) ? Security.USER_ID.ToString() : String.Empty %>';
// 03/28/2018 Paul.  The User Primary Role is used with role-based views. 
var sPRIMARY_ROLE_NAME   = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["PRIMARY_ROLE_NAME"])) %>';
// 03/28/2018 Paul.  The team may be needed for rules. 
var sTEAM_ID             = '<%# Security.TEAM_ID   %>';
var sTEAM_NAME           = '<%# Security.TEAM_NAME %>';

</script>

