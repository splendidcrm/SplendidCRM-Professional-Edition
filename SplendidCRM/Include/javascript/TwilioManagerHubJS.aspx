<%@ Page language="c#" Codebehind="TwilioManagerHubJS.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.JavaScript.TwilioManagerHubJS" %>
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
// 11/06/2013 Paul.  Make sure to JavaScript escape the text as the various languages may introduce accents. 
</script>
<head visible="false" runat="server" />
var sLNK_VIEW                          = '<%# Sql.EscapeJavaScript(L10n.Term(".LNK_VIEW"                               )) %>';
var sLNK_EDIT                          = '<%# Sql.EscapeJavaScript(L10n.Term(".LNK_EDIT"                               )) %>';
var sLBL_CREATE_BUTTON_LABEL           = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_CREATE_BUTTON_LABEL"                )) %>';
var sLBL_TWILIO_CREATE_MESSAGE         = '<%# Sql.EscapeJavaScript(L10n.Term("Twilio.LBL_TWILIO_CREATE_MESSAGE"        )) %>';
var sLBL_TWILIO_INCOMING_MESSAGE       = '<%# Sql.EscapeJavaScript(L10n.Term("Twilio.LBL_TWILIO_INCOMING_MESSAGE"      )) %>';
var sLBL_NEW_INCOMING_MESSAGE_TEMPLATE = '<%# Sql.EscapeJavaScript(L10n.Term("Twilio.LBL_NEW_INCOMING_MESSAGE_TEMPLATE")) %>';

function TwilioStatusDialog(sTitle, sStatus)
{
	var divTwilioStatus = document.getElementById('divTwilioStatus');
	if ( divTwilioStatus == null )
	{
		var $dialog = $('<div id="divTwilioStatus"></div>');
		$dialog.dialog(
		{
			  modal    : false
			, resizable: false
			, width    : 300
			, height   : 100
			, position : { my: 'right bottom', at: 'right bottom', of: window }
			, title    : sTitle
			, create   : function(event, ui)
			{
				divTwilioStatus = document.getElementById('divTwilioStatus');
				divTwilioStatus.innerHTML = sStatus;
			}
			, close: function(event, ui)
			{
				$dialog.dialog('destroy');
				var divTwilioStatus = document.getElementById('divTwilioStatus');
				divTwilioStatus.parentNode.removeChild(divTwilioStatus);
			}
		});
	}
	else
	{
		divTwilioStatus.innerHTML = sStatus;
		$('#divTwilioStatus').dialog('option', 'title', sTitle);
	}
}

function TwilioCreateMessage(sMESSAGE_SID, sFROM_NUMBER, sTO_NUMBER, sSUBJECT)
{
	twilioManager.server.createSmsMessage(sMESSAGE_SID, sFROM_NUMBER, sTO_NUMBER, sSUBJECT).done(function(result)
	{
		window.location.href = sREMOTE_SERVER + 'SmsMessages/edit.aspx?ID=' + result;
	})
	.fail(function(e)
	{
		TwilioStatusDialog(sLBL_TWILIO_CREATE_MESSAGE, 'TwilioCreateMessage error: ' + e.message);
	});
}

function BuildMessageEditLinks(sSMS_MESSAGE_ID)
{
	var sMessageLinks = '';
	if ( sSMS_MESSAGE_ID != null && sSMS_MESSAGE_ID.length > 0 )
		sMessageLinks = ' &nbsp; <a href="' + sREMOTE_SERVER + 'SmsMessages/view.aspx?ID=' + sSMS_MESSAGE_ID + '">' + sLNK_VIEW + '</a> &nbsp; <a href="' + sREMOTE_SERVER + 'SmsMessages/edit.aspx?ID=' + sSMS_MESSAGE_ID + '">' + sLNK_EDIT + '</a>';
	return sMessageLinks;
}

function BuildMessageCreateLink(sMESSAGE_SID, sFROM_NUMBER, sTO_NUMBER, sSUBJECT)
{
	var sMessageLinks = '';
	sMessageLinks = ' &nbsp; <a href="#" onclick="TwilioCreateMessage(\'' + sMESSAGE_SID + '\', \'' + sFROM_NUMBER + '\', \'' + sTO_NUMBER + '\', \'' + escape(sSUBJECT) + '\'); return false;">' + sLBL_CREATE_BUTTON_LABEL + '</a>';
	return sMessageLinks;
}

var twilioManager = $.connection.TwilioManagerHub;

twilioManager.client.incomingMessage = function(sMESSAGE_SID, sFROM_NUMBER, sTO_NUMBER, sSUBJECT, sSMS_MESSAGE_ID)
{
	var sSUBJECT = sLBL_NEW_INCOMING_MESSAGE_TEMPLATE.replace('{0}', sFROM_NUMBER).replace('{1}', sSUBJECT);
	if ( sSMS_MESSAGE_ID != null && sSMS_MESSAGE_ID.length > 0 )
	{
		TwilioStatusDialog(sLBL_TWILIO_INCOMING_MESSAGE, sSUBJECT + BuildMessageEditLinks(sSMS_MESSAGE_ID));
	}
	else
	{
		TwilioStatusDialog(sLBL_TWILIO_INCOMING_MESSAGE, sSUBJECT + BuildMessageCreateLink(sMESSAGE_SID, sFROM_NUMBER, sTO_NUMBER, sSUBJECT));
	}
};

