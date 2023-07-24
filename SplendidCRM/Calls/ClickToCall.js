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

// 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
// 09/20/2013 Paul.  Move EXTENSION to the main table. 
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
		img.src    = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAACHUlEQVR42mNkAIKEpHimgsKCED5ePl0GRkZGBgLg69cvV2fOnLlyysSp/8CK125dOcXc0iybkEZkcP7M+Vm+bkHpjBpqqrwLts16w8zCzEaKAf/+/vuTHpQrxqipqSbVsbLuKUKKkYGDlZPhx+9vBA2pjW1XZtTQUJUqmp8JN8Be2Y1BVUSLYfPVlQwvvzzHa8CktNkQA+KnhT1lBNoMCr9iuzoGZiYWhmsvLjJsvr4GtxeAcGn+WmVGdQ0VKZtO46dczJwMrEysDLUOHQwCnEIM229tYDhwbyeKpv9A/Pf/X4Zf/34xfP/7g+F49XklsAHWnYZPQX4HARclT4ZInQSGNdeWMuy8uxlJMxD+R3XFieoLUAO6jJ7CIh/kjTzTcgYtUT2GOecnM5x6dpxBiEOIQVVIg+H08xNAQ/7BXQM3wKbbCBwGMMDBwslQZF7NoCCgzHAGaIA4jxSDHL8Cw+OPDxhaj1Yz/P33F2zA8SqoF2x6jJ4yoCVADmYOhmjtJAYLaVt44jz17BjDrAuT4GqOV8AM6DNGcQGyv8W5JBhk+OQZPv/8xHD7/Q0UuePlQANUlBXFHCabPWNgYmQmJSWCAuNw4VlZRjkpaQ6DDO31YuZCHqTof33m/Z6L06/5gd2trKigJOsgWcwtw6kB5DIRsvvr0+83nxx40Xvn3v07cI/LSkmxMzEBQ46BgVB2/v/v37+fj589/QHiAADmJsE+kwiJAAAAAABJRU5ErkJggg%3D%3D';
		lnk.appendChild(img);
	}
}

function AsteriskCall(sSpanID, sMODULE_NAME, sID, sPHONE)
{
	try
	{
		var userContext = new Object();
		userContext.SpanID      = sSpanID     ;
		userContext.MODULE_NAME = sMODULE_NAME;
		userContext.ID          = sID         ;
		userContext.PHONE       = sPHONE      ;
		
		var sLBL_CALLS_CALLING = 'Calling . . .';
		var sLNK_NEW_CALL      = 'Create Call';
		var $dialog = $('<div id="' + sSpanID + '_divCall"></div>');
		$dialog.dialog(
		{
			  modal    : true
			, resizable: true
			, width    : 200
			, height   : 100
			, position : { my: 'left top', at: 'right top-8', of: '#' + sSpanID }
			, title    : sLBL_CALLS_CALLING
			, create   : function(event, ui)
			{
				try
				{
					var divCall = document.getElementById(sSpanID + '_divCall');
					var divStatus = document.createElement('div');
					divCall.appendChild(divStatus);
					divStatus.id = sSpanID + '_divCall_divStatus';
					divStatus.innerHTML = sPHONE;
					
					var divCreateCall = document.createElement('div');
					divCall.appendChild(divCreateCall);
					divCreateCall.id = sSpanID + '_divCall_divCreateCall';
					var aCreateCall = document.createElement('a');
					divCreateCall.appendChild(aCreateCall);
					aCreateCall.id = sSpanID + '_divCall_aCreateCall';
					aCreateCall.href = sREMOTE_SERVER + 'Calls/edit.aspx?PARENT_ID=' + sID + '&DIRECTION=Outbound';
					aCreateCall.innerHTML = sLNK_NEW_CALL;
				}
				catch(e)
				{
				}
			}
			, close: function(event, ui)
			{
				$dialog.dialog('destroy');
				var divCall = document.getElementById(sSpanID + '_divCall');
				divCall.parentNode.removeChild(divCall);
			}
		});
		SplendidCRM.Calls.ClickToCall.AsteriskCall(sPHONE, AsteriskCall_OnSucceededWithContext, AsteriskCall_OnFailed, userContext);
	}
	catch(e)
	{
		alert('AsteriskCall: ' + e.message);
	}
}

function AsteriskCall_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var divStatus = document.getElementById(userContext.SpanID + '_divCall_divStatus');
		if ( divStatus != null )
			divStatus.innerHTML = result;
		var aCreateCall = document.getElementById(userContext.SpanID + '_divCall_aCreateCall');
		if ( aCreateCall != null )
			aCreateCall.href = sREMOTE_SERVER + 'Calls/edit.aspx?PARENT_ID=' + userContext.ID + '&DIRECTION=Outbound&STATUS=Held';
	}
	else
	{
		alert('result from ClickToCall.AsteriskCall service is null');
	}
}

function AsteriskCall_OnFailed(error, userContext)
{
	var divStatus = document.getElementById(userContext.SpanID + '_divCall_divStatus');
	if ( divStatus != null )
		divStatus.innerHTML = error.get_message();
	var aCreateCall = document.getElementById(userContext.SpanID + '_divCall_aCreateCall');
	if ( aCreateCall != null )
		aCreateCall.href = sREMOTE_SERVER + 'Calls/edit.aspx?PARENT_ID=' + userContext.ID + '&DIRECTION=Outbound&STATUS=Not%20Held';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


