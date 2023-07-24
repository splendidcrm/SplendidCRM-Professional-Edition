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

function QUOTES_QUOTE_NAME_Changed(fldQUOTE_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldQUOTE_NAME.id.substring(0, fldQUOTE_NAME.id.length - 'QUOTE_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'QUOTE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_QUOTE_NAME = document.getElementById(userContext + 'PREV_QUOTE_NAME');
	if ( fldPREV_QUOTE_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_QUOTE_NAME');
	}
	else if ( fldPREV_QUOTE_NAME.value != fldQUOTE_NAME.value )
	{
		if ( fldQUOTE_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Quotes.AutoComplete.QUOTES_QUOTE_NAME_Get(fldQUOTE_NAME.value, QUOTES_QUOTE_NAME_Changed_OnSucceededWithContext, QUOTES_QUOTE_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('QUOTES_QUOTE_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			QUOTES_QUOTE_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function QUOTES_QUOTE_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors      = document.getElementById(userContext + 'QUOTE_NAME_AjaxErrors');
		var fldQUOTE_ID        = document.getElementById(userContext + 'QUOTE_ID'       );
		var fldQUOTE_NAME      = document.getElementById(userContext + 'QUOTE_NAME'     );
		var fldPREV_QUOTE_NAME = document.getElementById(userContext + 'PREV_QUOTE_NAME');
		if ( fldQUOTE_ID        != null ) fldQUOTE_ID.value        = sID  ;
		if ( fldQUOTE_NAME      != null ) fldQUOTE_NAME.value      = sNAME;
		if ( fldPREV_QUOTE_NAME != null ) fldPREV_QUOTE_NAME.value = sNAME;
	}
	else
	{
		alert('result from Quotes.AutoComplete service is null');
	}
}

function QUOTES_QUOTE_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'QUOTE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldQUOTE_ID        = document.getElementById(userContext + 'QUOTE_ID'       );
	var fldQUOTE_NAME      = document.getElementById(userContext + 'QUOTE_NAME'     );
	var fldPREV_QUOTE_NAME = document.getElementById(userContext + 'PREV_QUOTE_NAME');
	if ( fldQUOTE_ID        != null ) fldQUOTE_ID.value        = '';
	if ( fldQUOTE_NAME      != null ) fldQUOTE_NAME.value      = '';
	if ( fldPREV_QUOTE_NAME != null ) fldPREV_QUOTE_NAME.value = '';
}

function QUOTES_QUOTE_NUM_Changed(fldQUOTE_NUM)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldQUOTE_NUM.id.substring(0, fldQUOTE_NUM.id.length - 'QUOTE_NUM'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'QUOTE_NUM_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_QUOTE_NUM = document.getElementById(userContext + 'PREV_QUOTE_NUM');
	if ( fldPREV_QUOTE_NUM == null )
	{
		//alert('Could not find ' + userContext + 'PREV_QUOTE_NUM');
	}
	else if ( fldPREV_QUOTE_NUM.value != fldQUOTE_NUM.value )
	{
		if ( fldQUOTE_NUM.value.length > 0 )
		{
			try
			{
				SplendidCRM.Quotes.AutoComplete.QUOTES_QUOTE_NUM_Get(fldQUOTE_NUM.value, QUOTES_QUOTE_NUM_Changed_OnSucceededWithContext, QUOTES_QUOTE_NUM_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('QUOTES_QUOTE_NUM_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			QUOTES_QUOTE_NUM_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function QUOTES_QUOTE_NUM_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors     = document.getElementById(userContext + 'QUOTE_NUM_AjaxErrors');
		var fldQUOTE_ID       = document.getElementById(userContext + 'QUOTE_ID'      );
		var fldQUOTE_NUM      = document.getElementById(userContext + 'QUOTE_NUM'     );
		var fldPREV_QUOTE_NUM = document.getElementById(userContext + 'PREV_QUOTE_NUM');
		if ( fldQUOTE_ID       != null ) fldQUOTE_ID.value       = sID  ;
		if ( fldQUOTE_NUM      != null ) fldQUOTE_NUM.value      = sNAME;
		if ( fldPREV_QUOTE_NUM != null ) fldPREV_QUOTE_NUM.value = sNAME;
	}
	else
	{
		alert('result from Quotes.AutoComplete service is null');
	}
}

function QUOTES_QUOTE_NUM_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'QUOTE_NUM_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldQUOTE_ID        = document.getElementById(userContext + 'QUOTE_ID'       );
	var fldQUOTE_NUM      = document.getElementById(userContext + 'QUOTE_NUM'     );
	var fldPREV_QUOTE_NUM = document.getElementById(userContext + 'PREV_QUOTE_NUM');
	if ( fldQUOTE_ID        != null ) fldQUOTE_ID.value        = '';
	if ( fldQUOTE_NUM      != null ) fldQUOTE_NUM.value      = '';
	if ( fldPREV_QUOTE_NUM != null ) fldPREV_QUOTE_NUM.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

