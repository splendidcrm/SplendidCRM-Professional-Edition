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

function ZIPCODES_POSTALCODE_Changed(fldPOSTALCODE)
{
	var userContext = fldPOSTALCODE.id.substring(0, fldPOSTALCODE.id.length - 'POSTALCODE'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'POSTALCODE_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_POSTALCODE = document.getElementById(userContext + 'POSTALCODE_PREV');
	if ( fldPREV_POSTALCODE == null )
	{
		//alert('Could not find ' + userContext + 'PREV_POSTALCODE');
	}
	else if ( fldPREV_POSTALCODE.value != fldPOSTALCODE.value )
	{
		if ( fldPOSTALCODE.value.length > 0 )
		{
			try
			{
				var fldCOUNTRY = document.getElementById(userContext + 'COUNTRY');
				var sCOUNTRY = '';
				// 11/19/2017 Paul.  Field will be null, not undefined. 
				if ( fldCOUNTRY != null )
				{
					if ( fldCOUNTRY.options !== undefined )
						sCOUNTRY = fldCOUNTRY.options[fldCOUNTRY.selectedIndex].value;
					else
						sCOUNTRY = fldCOUNTRY.value;
				}
				SplendidCRM.Administration.ZipCodes.AutoComplete.ZIPCODES_POSTALCODE_Get(fldPOSTALCODE.value, sCOUNTRY, ZIPCODES_POSTALCODE_Changed_OnSucceededWithContext, ZIPCODES_POSTALCODE_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('ZIPCODES_POSTALCODE_Changed: ' + e.message);
			}
		}
		else
		{
			fldPREV_POSTALCODE.value = '';
		}
	}
}

function ZIPCODES_POSTALCODE_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sPOSTALCODE = result.POSTALCODE;
		var sCITY       = result.CITY      ;
		var sSTATE      = result.STATE     ;
		var sCOUNTRY    = result.COUNTRY   ;
		
		var fldAjaxErrors      = document.getElementById(userContext + 'POSTALCODE_AjaxErrors');
		var fldPOSTALCODE      = document.getElementById(userContext + 'POSTALCODE'     );
		var fldCITY            = document.getElementById(userContext + 'CITY'           );
		var fldSTATE           = document.getElementById(userContext + 'STATE'          );
		var fldCOUNTRY         = document.getElementById(userContext + 'COUNTRY'        );
		var fldPREV_POSTALCODE = document.getElementById(userContext + 'POSTALCODE_PREV');
		if ( fldPOSTALCODE      != null ) fldPOSTALCODE     .value = sPOSTALCODE;
		if ( fldCITY            != null ) fldCITY           .value = sCITY      ;
		if ( fldSTATE           != null ) fldSTATE          .value = sSTATE     ;
		if ( fldCOUNTRY         != null ) fldCOUNTRY        .value = sCOUNTRY   ;
		if ( fldPREV_POSTALCODE != null ) fldPREV_POSTALCODE.value = sPOSTALCODE;
	}
	else
	{
		alert('result from ZipCodes.AutoComplete service is null');
	}
}

function ZIPCODES_POSTALCODE_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'POSTALCODE_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	// 04/13/2016 Paul.  If postal code not found, then do nothing.  Don't clear any field. 
	//var fldPOSTALCODE      = document.getElementById(userContext + 'POSTALCODE'     );
	//var fldPREV_POSTALCODE = document.getElementById(userContext + 'POSTALCODE_PREV');
	//if ( fldPOSTALCODE      != null ) fldPOSTALCODE.value      = '';
	//if ( fldPREV_POSTALCODE != null ) fldPREV_POSTALCODE.value = '';
}

function ZipCodes_SetContextKey(sEXTENDER_ID, sPOSTALCODE_ID)
{
	try
	{
		var userContext = sPOSTALCODE_ID.substring(0, sPOSTALCODE_ID.length - 'POSTALCODE'.length)
		var fldCOUNTRY = document.getElementById(userContext + 'COUNTRY');
		if ( fldCOUNTRY != null )
		{
			fldCOUNTRY.onkeyup = function()
			{
				var sCOUNTRY = '';
				if ( fldCOUNTRY.options !== undefined )
					sCOUNTRY = fldCOUNTRY.options[fldCOUNTRY.selectedIndex].value;
				else
					sCOUNTRY = fldCOUNTRY.value;
				var fldPOSTALCODE = document.getElementById(sPOSTALCODE_ID);
				if ( fldPOSTALCODE != null )
				{
					$find(sEXTENDER_ID).set_contextKey(sCOUNTRY);
				}
			}
		}
	}
	catch(e)
	{
		console.log('ZipCodes_SetContextKey: ' + e.message);
	}
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

