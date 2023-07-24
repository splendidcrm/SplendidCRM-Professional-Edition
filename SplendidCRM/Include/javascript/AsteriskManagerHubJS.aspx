<%@ Page language="c#" Codebehind="AsteriskManagerHubJS.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.JavaScript.AsteriskManagerHubJS" %>
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
var sLNK_VIEW                            = '<%# Sql.EscapeJavaScript(L10n.Term(".LNK_VIEW"                                   )) %>';
var sLNK_EDIT                            = '<%# Sql.EscapeJavaScript(L10n.Term(".LNK_EDIT"                                   )) %>';
var sLBL_CREATE_BUTTON_LABEL             = '<%# Sql.EscapeJavaScript(L10n.Term(".LBL_CREATE_BUTTON_LABEL"                    )) %>';
var sLBL_CALLING                         = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_CALLING"                        )) %>';
var sLBL_NEW_OUTGOING_CALL_TEMPLATE      = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_NEW_OUTGOING_CALL_TEMPLATE"     )) %>';
var sLBL_NEW_INCOMING_CALL_TEMPLATE      = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_NEW_INCOMING_CALL_TEMPLATE"     )) %>';
var sLBL_ANSWERED_OUTGOING_CALL_TEMPLATE = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ANSWERED_OUTGOING_CALL_TEMPLATE")) %>';
var sLBL_MISSED_OUTGOING_CALL_TEMPLATE   = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_MISSED_OUTGOING_CALL_TEMPLATE"  )) %>';
var sLBL_ANSWERED_INCOMING_CALL_TEMPLATE = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ANSWERED_INCOMING_CALL_TEMPLATE")) %>';
var sLBL_MISSED_INCOMING_CALL_TEMPLATE   = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_MISSED_INCOMING_CALL_TEMPLATE"  )) %>';
var sLBL_ASTERISK_CREATE_CALL            = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ASTERISK_CREATE_CALL"           )) %>';
var sLBL_ASTERISK_OUTGOING_CALL          = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ASTERISK_OUTGOING_CALL"         )) %>';
var sLBL_ASTERISK_INCOMING_CALL          = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ASTERISK_INCOMING_CALL"         )) %>';
var sLBL_ASTERISK_OUTGOING_COMPLETE      = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ASTERISK_OUTGOING_COMPLETE"     )) %>';
var sLBL_ASTERISK_INCOMING_COMPLETE      = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ASTERISK_INCOMING_COMPLETE"     )) %>';
var sLBL_ASTERISK_OUTGOING_INCOMPLETE    = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ASTERISK_OUTGOING_INCOMPLETE"   )) %>';
var sLBL_ASTERISK_INCOMING_INCOMPLETE    = '<%# Sql.EscapeJavaScript(L10n.Term("Asterisk.LBL_ASTERISK_INCOMING_INCOMPLETE"   )) %>';

function AsteriskStatusDialog(sTitle, sStatus)
{
	var divAsteriskStatus = document.getElementById('divAsteriskStatus');
	if ( divAsteriskStatus == null )
	{
		var $dialog = $('<div id="divAsteriskStatus"></div>');
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
				divAsteriskStatus = document.getElementById('divAsteriskStatus');
				divAsteriskStatus.innerHTML = sStatus;
			}
			, close: function(event, ui)
			{
				$dialog.dialog('destroy');
				var divAsteriskStatus = document.getElementById('divAsteriskStatus');
				divAsteriskStatus.parentNode.removeChild(divAsteriskStatus);
			}
		});
	}
	else
	{
		divAsteriskStatus.innerHTML = sStatus;
		$('#divAsteriskStatus').dialog('option', 'title', sTitle);
	}
}

function AsteriskCreateCall(sUniqueId)
{
	asteriskManager.server.createCall(sUniqueId).done(function(result)
	{
		window.location.href = sREMOTE_SERVER + 'Calls/edit.aspx?ID=' + result;
	})
	.fail(function(e)
	{
		AsteriskStatusDialog(sLBL_ASTERISK_CREATE_CALL, 'AsteriskCreateCall error: ' + e.message);
	});
}

function BuildCallEditLinks(sCALL_ID)
{
	var sCallLinks = '';
	if ( sCALL_ID != null && sCALL_ID.length > 0 )
		sCallLinks = ' &nbsp; <a href="' + sREMOTE_SERVER + 'Calls/view.aspx?ID=' + sCALL_ID + '">' + sLNK_VIEW + '</a> &nbsp; <a href="' + sREMOTE_SERVER + 'Calls/edit.aspx?ID=' + sCALL_ID + '">' + sLNK_EDIT + '</a>';
	return sCallLinks;
}

function BuildCallCreateLink(sUniqueId)
{
	var sCallLinks = '';
	sCallLinks = ' &nbsp; <a href="#" onclick="AsteriskCreateCall(\'' + sUniqueId + '\'); return false;">' + sLBL_CREATE_BUTTON_LABEL + '</a>';
	return sCallLinks;
}

var asteriskManager = $.connection.AsteriskManagerHub;
asteriskManager.client.newState = function(sStatus)
{
	//AsteriskStatusDialog('Asterisk New State', sStatus);
};

asteriskManager.client.outgoingCall = function(sUniqueId, sConnectedLineName, sCallerID, sCALL_ID)
{
	var sSUBJECT = sLBL_NEW_OUTGOING_CALL_TEMPLATE.replace('{0}', sConnectedLineName).replace('{1}', sCallerID);
	if ( sCALL_ID != null && sCALL_ID.length > 0 )
	{
		AsteriskStatusDialog(sLBL_ASTERISK_OUTGOING_CALL, sSUBJECT + BuildCallEditLinks(sCALL_ID));
	}
	else
	{
		AsteriskStatusDialog(sLBL_ASTERISK_OUTGOING_CALL, sSUBJECT + BuildCallCreateLink(sUniqueId));
	}
};

asteriskManager.client.incomingCall = function(sUniqueId, sConnectedLineName, sCallerID, sCALL_ID)
{
	var sSUBJECT = sLBL_NEW_INCOMING_CALL_TEMPLATE.replace('{0}', sCallerID).replace('{1}', sConnectedLineName);
	if ( sCALL_ID != null && sCALL_ID.length > 0 )
	{
		AsteriskStatusDialog(sLBL_ASTERISK_INCOMING_CALL, sSUBJECT + BuildCallEditLinks(sCALL_ID));
	}
	else
	{
		AsteriskStatusDialog(sLBL_ASTERISK_INCOMING_CALL, sSUBJECT + BuildCallCreateLink(sUniqueId));
	}
};

asteriskManager.client.outgoingComplete = function(sUniqueId, sConnectedLineName, sCallerID, sCALL_ID, nDURATION_HOURS, nDURATION_MINUTES)
{
	if ( getUrlParam('ID') != sCALL_ID )
	{
		var sSUBJECT = sLBL_ANSWERED_OUTGOING_CALL_TEMPLATE.replace('{0}', sConnectedLineName).replace('{1}', sCallerID);
		AsteriskStatusDialog(sLBL_ASTERISK_OUTGOING_COMPLETE, sSUBJECT + BuildCallEditLinks(sCALL_ID));
	}
	else
	{
		//AsteriskStatusDialog(sLBL_ASTERISK_OUTGOING_COMPLETE, nDURATION_HOURS + ':' + nDURATION_MINUTES);
		SelectOption   ('ctl00_cntBody_ctlEditView_DIRECTION'       , 'Outbound'    );
		SelectOption   ('ctl00_cntBody_ctlEditView_STATUS'          , 'Held'        );
		SelectAddOption('ctl00_cntBody_ctlEditView_DURATION_MINUTES', nDURATION_MINUTES.toString());
		var ctl00_cntBody_ctlEditView_DURATION_HOURS = document.getElementById('ctl00_cntBody_ctlEditView_DURATION_HOURS');
		if ( ctl00_cntBody_ctlEditView_DURATION_HOURS != null )
			ctl00_cntBody_ctlEditView_DURATION_HOURS.value = nDURATION_HOURS.toString();
	}
};

asteriskManager.client.incomingComplete = function(sUniqueId, sConnectedLineName, sCallerID, sCALL_ID, nDURATION_HOURS, nDURATION_MINUTES)
{
	if ( getUrlParam('ID') != sCALL_ID )
	{
		var sSUBJECT = sLBL_ANSWERED_INCOMING_CALL_TEMPLATE.replace('{0}', sConnectedLineName).replace('{1}', sCallerID);
		AsteriskStatusDialog(sLBL_ASTERISK_INCOMING_COMPLETE, sSUBJECT + BuildCallEditLinks(sCALL_ID));
	}
	else
	{
		//AsteriskStatusDialog(sLBL_ASTERISK_INCOMING_COMPLETE, nDURATION_HOURS + ':' + nDURATION_MINUTES);
		SelectOption   ('ctl00_cntBody_ctlEditView_DIRECTION'       , 'Inbound'     );
		SelectOption   ('ctl00_cntBody_ctlEditView_STATUS'          , 'Held'        );
		SelectAddOption('ctl00_cntBody_ctlEditView_DURATION_MINUTES', nDURATION_MINUTES.toString());
		var ctl00_cntBody_ctlEditView_DURATION_HOURS = document.getElementById('ctl00_cntBody_ctlEditView_DURATION_HOURS');
		if ( ctl00_cntBody_ctlEditView_DURATION_HOURS != null )
			ctl00_cntBody_ctlEditView_DURATION_HOURS.value = nDURATION_HOURS.toString();
	}
};

asteriskManager.client.outgoingIncomplete = function(sUniqueId, sConnectedLineName, sCallerID, sCALL_ID)
{
	if ( getUrlParam('ID') != sCALL_ID )
	{
		// 12/26/2013 Paul.  First parameter is Extension and second parameter is the called number. 
		var sSUBJECT = sLBL_MISSED_OUTGOING_CALL_TEMPLATE.replace('{0}', sConnectedLineName).replace('{1}', sCallerID);
		AsteriskStatusDialog(sLBL_ASTERISK_OUTGOING_INCOMPLETE, sSUBJECT + BuildCallEditLinks(sCALL_ID));
	}
	else
	{
		SelectOption('ctl00_cntBody_ctlEditView_DIRECTION'       , 'Outbound'    );
		SelectOption('ctl00_cntBody_ctlEditView_STATUS'          , 'Not Answered');
		SelectOption('ctl00_cntBody_ctlEditView_DURATION_MINUTES', '0'           );
		var ctl00_cntBody_ctlEditView_DURATION_HOURS = document.getElementById('ctl00_cntBody_ctlEditView_DURATION_HOURS');
		if ( ctl00_cntBody_ctlEditView_DURATION_HOURS != null )
			ctl00_cntBody_ctlEditView_DURATION_HOURS.value = '0';
	}
};

asteriskManager.client.incomingIncomplete = function(sUniqueId, sConnectedLineName, sCallerID, sCALL_ID)
{
	if ( getUrlParam('ID') != sCALL_ID )
	{
		var sSUBJECT = sLBL_MISSED_INCOMING_CALL_TEMPLATE.replace('{0}', sCallerID).replace('{1}', sConnectedLineName);
		AsteriskStatusDialog(sLBL_ASTERISK_INCOMING_INCOMPLETE, sSUBJECT + BuildCallEditLinks(sCALL_ID));
	}
	else
	{
		SelectOption('ctl00_cntBody_ctlEditView_DIRECTION'       , 'Inbound'     );
		SelectOption('ctl00_cntBody_ctlEditView_STATUS'          , 'Not Answered');
		SelectOption('ctl00_cntBody_ctlEditView_DURATION_MINUTES', '0'           );
		var ctl00_cntBody_ctlEditView_DURATION_HOURS = document.getElementById('ctl00_cntBody_ctlEditView_DURATION_HOURS');
		if ( ctl00_cntBody_ctlEditView_DURATION_HOURS != null )
			ctl00_cntBody_ctlEditView_DURATION_HOURS.value = '0';
	}
};

function CreateClickToCall(sSpanID, sMODULE_NAME, sID, sPHONE)
{
	var spn = document.getElementById(sSpanID);
	if ( spn != null && sPHONE.length > 0 )
	{
		var lnk = document.createElement('a');
		lnk.href    = '#';
		lnk.onclick = function()
		{
			AsteriskCall(sSpanID, sMODULE_NAME, sID, sPHONE);
			return false;
		};
		spn.appendChild(lnk);
		var img = document.createElement('img');
		img.border = 0;
		img.width  = 16;
		img.height = 16;
		img.src    = sREMOTE_SERVER + 'App_Themes/Atlantic/images/phone.gif';
		lnk.appendChild(img);
	}
}

function AsteriskCall(sSpanID, sMODULE_NAME, sID, sPHONE)
{
	try
	{
		//var userContext = new Object();
		//userContext.SpanID      = sSpanID     ;
		//userContext.MODULE_NAME = sMODULE_NAME;
		//userContext.ID          = sID         ;
		//userContext.PHONE       = sPHONE      ;
		//SplendidCRM.Calls.ClickToCall.AsteriskCall(sPHONE, sID, sMODULE_NAME, AsteriskCall_OnSucceededWithContext, AsteriskCall_OnFailed, userContext);
		
		var sSUBJECT = sLBL_CALLING.replace('{0}', sPHONE);
		AsteriskStatusDialog(sLBL_ASTERISK_CREATE_CALL, sSUBJECT);
		
		// 09/20/2013 Paul.  Move EXTENSION to the main table. 
		asteriskManager.server.originateCall(sUSER_EXTENSION, sUSER_FULL_NAME, sUSER_PHONE_WORK, sPHONE, sID, sMODULE_NAME).done(function(result)
		{
			//AsteriskStatusDialog('Asterisk Originate', result);
		})
		.fail(function(e)
		{
		});
	}
	catch(e)
	{
		alert('AsteriskCall: ' + e.message);
	}
}

