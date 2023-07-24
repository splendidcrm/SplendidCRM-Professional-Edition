<%@ Page language="c#" Codebehind="TwitterManagerHubJS.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.JavaScript.TwitterManagerHubJS" %>
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
// 09/07/2013 Paul.  Put the labels in the javascript file because they will only change based on the language. 
</script>
<head visible="false" runat="server" />
var twitterManager = $.connection.TwitterManagerHub;

twitterManager.client.newTweet = function(sTRACK, sNAME, sDESCRIPTION, sDATE_START, lTWITTER_ID, lTWITTER_USER_ID, sTWITTER_FULL_NAME, sTWITTER_SCREEN_NAME, sTWITTER_AVATAR, gTWITTER_MESSAGE_ID)
{
	if ( sNAME != null && sNAME.length > 0 )
	{
		var olMyTwitterTracks = document.getElementById('olMyTwitterTracks');
		var liTweet = document.createElement('li');
		if ( olMyTwitterTracks.firstChild == null )
			olMyTwitterTracks.appendChild(liTweet);
		else
			olMyTwitterTracks.insertBefore(liTweet, olMyTwitterTracks.firstChild);
		
		var divTweetContent = document.createElement('div');
		liTweet.appendChild(divTweetContent);
			var divTweetHeader = document.createElement('div');
			divTweetContent.appendChild(divTweetHeader);
				var spnTweetHeadUserName = document.createElement('span');
				divTweetHeader.appendChild(spnTweetHeadUserName);
					var aTweetHeadAvatar = document.createElement('a');
					spnTweetHeadUserName.appendChild(aTweetHeadAvatar);
						var imgTweetHeadAvatar = document.createElement('img');
						aTweetHeadAvatar.appendChild(imgTweetHeadAvatar);
					var aTweetHeadFullName = document.createElement('a');
					spnTweetHeadUserName.appendChild(aTweetHeadFullName);
					var aTweetHeadScreenName = document.createElement('a');
					spnTweetHeadUserName.appendChild(aTweetHeadScreenName);
				var spnTweetHeadTime = document.createElement('span');
				divTweetHeader.appendChild(spnTweetHeadTime);
					var aTweetHeadTime = document.createElement('a');
					spnTweetHeadTime.appendChild(aTweetHeadTime);
			var divTweetText = document.createElement('div');
			divTweetContent.appendChild(divTweetText);
			var divTweetFooter = document.createElement('div');
			divTweetContent.appendChild(divTweetFooter);
		
		//olMyTwitterTracks.calssName      = 'twitter-stream-items';
		liTweet.className                = 'twitter-stream-item';
		divTweetContent.className        = 'twitter-stream-item-tweet';
		divTweetHeader.className         = 'twitter-stream-item-header';
		spnTweetHeadUserName.className   = 'twitter-stream-item-header-username';
		imgTweetHeadAvatar.className     = 'twitter-stream-item-header-avatar';
		aTweetHeadFullName.className     = 'twitter-stream-item-header-fullname';
		aTweetHeadScreenName.className   = 'twitter-stream-item-header-screenname';
		spnTweetHeadTime.className       = 'twitter-stream-item-header-time';
		aTweetHeadTime.className         = 'twitter-stream-item-header-timestamp';
		divTweetText.className           = 'twitter-stream-item-tweet-text';
		divTweetFooter.className         = 'twitter-stream-item-footer';
		
		aTweetHeadAvatar.href            = 'https://twitter.com/' + sTWITTER_SCREEN_NAME;
		aTweetHeadAvatar.target          = 'TwitterSite';
		imgTweetHeadAvatar.src           = sTWITTER_AVATAR;
		imgTweetHeadAvatar.border        = 0;
		aTweetHeadFullName.href          = 'https://twitter.com/' + sTWITTER_SCREEN_NAME;
		aTweetHeadFullName.target        = 'TwitterSite';
		aTweetHeadFullName.innerHTML     = sTWITTER_FULL_NAME;
		aTweetHeadScreenName.href        = 'https://twitter.com/' + sTWITTER_SCREEN_NAME;
		aTweetHeadScreenName.target      = 'TwitterSite';
		aTweetHeadScreenName.innerHTML   = '@' + sTWITTER_SCREEN_NAME;
		aTweetHeadTime.href              = 'https://twitter.com/' + sTWITTER_SCREEN_NAME + '/status/' + lTWITTER_ID;
		aTweetHeadTime.target            = 'TwitterSite';
		aTweetHeadTime.innerHTML         = sDATE_START;
		divTweetText.innerHTML           = sDESCRIPTION;
	}
};

