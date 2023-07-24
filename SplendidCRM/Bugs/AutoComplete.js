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

function BUGS_BUG_NAME_Changed(fldBUG_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldBUG_NAME.id.substring(0, fldBUG_NAME.id.length - 'BUG_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'BUG_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_BUG_NAME = document.getElementById(userContext + 'PREV_BUG_NAME');
	if ( fldPREV_BUG_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_BUG_NAME');
	}
	else if ( fldPREV_BUG_NAME.value != fldBUG_NAME.value )
	{
		if ( fldBUG_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Bugs.AutoComplete.BUGS_BUG_NAME_Get(fldBUG_NAME.value, BUGS_BUG_NAME_Changed_OnSucceededWithContext, BUGS_BUG_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('BUGS_BUG_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			BUGS_BUG_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function BUGS_BUG_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors    = document.getElementById(userContext + 'BUG_NAME_AjaxErrors');
		var fldBUG_ID        = document.getElementById(userContext + 'BUG_ID'       );
		var fldBUG_NAME      = document.getElementById(userContext + 'BUG_NAME'     );
		var fldPREV_BUG_NAME = document.getElementById(userContext + 'PREV_BUG_NAME');
		if ( fldBUG_ID        != null ) fldBUG_ID.value        = sID  ;
		if ( fldBUG_NAME      != null ) fldBUG_NAME.value      = sNAME;
		if ( fldPREV_BUG_NAME != null ) fldPREV_BUG_NAME.value = sNAME;
	}
	else
	{
		alert('result from Bugs.AutoComplete service is null');
	}
}

function BUGS_BUG_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'BUG_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldBUG_ID        = document.getElementById(userContext + 'BUG_ID'       );
	var fldBUG_NAME      = document.getElementById(userContext + 'BUG_NAME'     );
	var fldPREV_BUG_NAME = document.getElementById(userContext + 'PREV_BUG_NAME');
	if ( fldBUG_ID        != null ) fldBUG_ID.value        = '';
	if ( fldBUG_NAME      != null ) fldBUG_NAME.value      = '';
	if ( fldPREV_BUG_NAME != null ) fldPREV_BUG_NAME.value = '';
}

function BUGS_BUG_NUMBER_Changed(fldBUG_NUMBER)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldBUG_NUMBER.id.substring(0, fldBUG_NUMBER.id.length - 'BUG_NUMBER'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'BUG_NUMBER_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_BUG_NUMBER = document.getElementById(userContext + 'PREV_BUG_NUMBER');
	if ( fldPREV_BUG_NUMBER == null )
	{
		//alert('Could not find ' + userContext + 'PREV_BUG_NUMBER');
	}
	else if ( fldPREV_BUG_NUMBER.value != fldBUG_NUMBER.value )
	{
		if ( fldBUG_NUMBER.value.length > 0 )
		{
			try
			{
				SplendidCRM.Bugs.AutoComplete.BUGS_BUG_NUMBER_Get(fldBUG_NUMBER.value, BUGS_BUG_NUMBER_Changed_OnSucceededWithContext, BUGS_BUG_NUMBER_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('BUGS_BUG_NUMBER_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			BUGS_BUG_NUMBER_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function BUGS_BUG_NUMBER_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors      = document.getElementById(userContext + 'BUG_NUMBER_AjaxErrors');
		var fldBUG_ID          = document.getElementById(userContext + 'BUG_ID'         );
		var fldBUG_NUMBER      = document.getElementById(userContext + 'BUG_NUMBER'     );
		var fldPREV_BUG_NUMBER = document.getElementById(userContext + 'PREV_BUG_NUMBER');
		if ( fldBUG_ID          != null ) fldBUG_ID.value          = sID  ;
		if ( fldBUG_NUMBER      != null ) fldBUG_NUMBER.value      = sNAME;
		if ( fldPREV_BUG_NUMBER != null ) fldPREV_BUG_NUMBER.value = sNAME;
	}
	else
	{
		alert('result from Bugs.AutoComplete service is null');
	}
}

function BUGS_BUG_NUMBER_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'BUG_NUMBER_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldBUG_ID        = document.getElementById(userContext + 'BUG_ID'       );
	var fldBUG_NUMBER      = document.getElementById(userContext + 'BUG_NUMBER'     );
	var fldPREV_BUG_NUMBER = document.getElementById(userContext + 'PREV_BUG_NUMBER');
	if ( fldBUG_ID        != null ) fldBUG_ID.value        = '';
	if ( fldBUG_NUMBER      != null ) fldBUG_NUMBER.value      = '';
	if ( fldPREV_BUG_NUMBER != null ) fldPREV_BUG_NUMBER.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


