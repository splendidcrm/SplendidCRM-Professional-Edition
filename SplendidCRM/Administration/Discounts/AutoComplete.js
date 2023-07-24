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

function DISCOUNTS_DISCOUNT_NAME_Changed(fldDISCOUNT_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldDISCOUNT_NAME.id.substring(0, fldDISCOUNT_NAME.id.length - 'DISCOUNT_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'DISCOUNT_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_DISCOUNT_NAME = document.getElementById(userContext + 'PREV_DISCOUNT_NAME');
	if ( fldPREV_DISCOUNT_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_DISCOUNT_NAME');
	}
	else if ( fldPREV_DISCOUNT_NAME.value != fldDISCOUNT_NAME.value )
	{
		if ( fldDISCOUNT_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Discounts.AutoComplete.DISCOUNTS_DISCOUNT_NAME_Get(fldDISCOUNT_NAME.value, DISCOUNTS_DISCOUNT_NAME_Changed_OnSucceededWithContext, DISCOUNTS_DISCOUNT_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('DISCOUNTS_DISCOUNT_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			DISCOUNTS_DISCOUNT_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function DISCOUNTS_DISCOUNT_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors         = document.getElementById(userContext + 'DISCOUNT_NAME_AjaxErrors');
		var fldDISCOUNT_ID        = document.getElementById(userContext + 'DISCOUNT_ID'       );
		var fldDISCOUNT_NAME      = document.getElementById(userContext + 'DISCOUNT_NAME'     );
		var fldPREV_DISCOUNT_NAME = document.getElementById(userContext + 'PREV_DISCOUNT_NAME');
		if ( fldDISCOUNT_ID        != null ) fldDISCOUNT_ID.value        = sID  ;
		if ( fldDISCOUNT_NAME      != null ) fldDISCOUNT_NAME.value      = sNAME;
		if ( fldPREV_DISCOUNT_NAME != null ) fldPREV_DISCOUNT_NAME.value = sNAME;
		// 08/15/2010 Paul.  We typically submit the form when the account changes so that we can load the address. 
		document.forms[0].submit();
	}
	else
	{
		alert('result from Discounts.AutoComplete service is null');
	}
}

function DISCOUNTS_DISCOUNT_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'DISCOUNT_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldDISCOUNT_ID        = document.getElementById(userContext + 'DISCOUNT_ID'       );
	var fldDISCOUNT_NAME      = document.getElementById(userContext + 'DISCOUNT_NAME'     );
	var fldPREV_DISCOUNT_NAME = document.getElementById(userContext + 'PREV_DISCOUNT_NAME');
	if ( fldDISCOUNT_ID        != null ) fldDISCOUNT_ID.value        = '';
	if ( fldDISCOUNT_NAME      != null ) fldDISCOUNT_NAME.value      = '';
	if ( fldPREV_DISCOUNT_NAME != null ) fldPREV_DISCOUNT_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

