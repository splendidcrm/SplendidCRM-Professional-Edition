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

function RELEASES_RELEASE_NAME_Changed(fldRELEASE_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldRELEASE_NAME.id.substring(0, fldRELEASE_NAME.id.length - 'RELEASE_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'RELEASE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_RELEASE_NAME = document.getElementById(userContext + 'PREV_RELEASE_NAME');
	if ( fldPREV_RELEASE_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_RELEASE_NAME');
	}
	else if ( fldPREV_RELEASE_NAME.value != fldRELEASE_NAME.value )
	{
		if ( fldRELEASE_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Releases.AutoComplete.RELEASES_RELEASE_NAME_Get(fldRELEASE_NAME.value, RELEASES_RELEASE_NAME_Changed_OnSucceededWithContext, RELEASES_RELEASE_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('RELEASES_RELEASE_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			RELEASES_RELEASE_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function RELEASES_RELEASE_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors        = document.getElementById(userContext + 'RELEASE_NAME_AjaxErrors');
		var fldRELEASE_ID        = document.getElementById(userContext + 'RELEASE_ID'       );
		var fldRELEASE_NAME      = document.getElementById(userContext + 'RELEASE_NAME'     );
		var fldPREV_RELEASE_NAME = document.getElementById(userContext + 'PREV_RELEASE_NAME');
		if ( fldRELEASE_ID        != null ) fldRELEASE_ID.value        = sID  ;
		if ( fldRELEASE_NAME      != null ) fldRELEASE_NAME.value      = sNAME;
		if ( fldPREV_RELEASE_NAME != null ) fldPREV_RELEASE_NAME.value = sNAME;
	}
	else
	{
		alert('result from Releases.AutoComplete service is null');
	}
}

function RELEASES_RELEASE_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'RELEASE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldRELEASE_ID        = document.getElementById(userContext + 'RELEASE_ID'       );
	var fldRELEASE_NAME      = document.getElementById(userContext + 'RELEASE_NAME'     );
	var fldPREV_RELEASE_NAME = document.getElementById(userContext + 'PREV_RELEASE_NAME');
	if ( fldRELEASE_ID        != null ) fldRELEASE_ID.value        = '';
	if ( fldRELEASE_NAME      != null ) fldRELEASE_NAME.value      = '';
	if ( fldPREV_RELEASE_NAME != null ) fldPREV_RELEASE_NAME.value = '';
}

// 08/05/2010 Paul.  Add support for AutoComplete when editing Bugs. 
function RELEASES_FOUND_IN_RELEASE_Changed(fldFOUND_IN_RELEASE)
{
	var userContext = fldFOUND_IN_RELEASE.id.substring(0, fldFOUND_IN_RELEASE.id.length - 'FOUND_IN_RELEASE'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'FOUND_IN_RELEASE_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_FOUND_IN_RELEASE = document.getElementById(userContext + 'PREV_FOUND_IN_RELEASE');
	if ( fldPREV_FOUND_IN_RELEASE == null )
	{
		//alert('Could not find ' + userContext + 'PREV_FOUND_IN_RELEASE');
	}
	else if ( fldPREV_FOUND_IN_RELEASE.value != fldFOUND_IN_RELEASE.value )
	{
		if ( fldFOUND_IN_RELEASE.value.length > 0 )
		{
			try
			{
				SplendidCRM.Releases.AutoComplete.RELEASES_RELEASE_NAME_Get(fldFOUND_IN_RELEASE.value, RELEASES_FOUND_IN_RELEASE_Changed_OnSucceededWithContext, RELEASES_FOUND_IN_RELEASE_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('RELEASES_FOUND_IN_RELEASE_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			RELEASES_FOUND_IN_RELEASE_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function RELEASES_FOUND_IN_RELEASE_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors            = document.getElementById(userContext + 'FOUND_IN_RELEASE_AjaxErrors');
		var fldFOUND_IN_RELEASE_ID   = document.getElementById(userContext + 'FOUND_IN_RELEASE_ID'  );
		var fldFOUND_IN_RELEASE      = document.getElementById(userContext + 'FOUND_IN_RELEASE'     );
		var fldPREV_FOUND_IN_RELEASE = document.getElementById(userContext + 'PREV_FOUND_IN_RELEASE');
		if ( fldFOUND_IN_RELEASE_ID   != null ) fldFOUND_IN_RELEASE_ID.value   = sID  ;
		if ( fldFOUND_IN_RELEASE      != null ) fldFOUND_IN_RELEASE.value      = sNAME;
		if ( fldPREV_FOUND_IN_RELEASE != null ) fldPREV_FOUND_IN_RELEASE.value = sNAME;
	}
	else
	{
		alert('result from Releases.AutoComplete service is null');
	}
}

function RELEASES_FOUND_IN_RELEASE_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'FOUND_IN_RELEASE_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldFOUND_IN_RELEASE_ID   = document.getElementById(userContext + 'FOUND_IN_RELEASE_ID'  );
	var fldFOUND_IN_RELEASE      = document.getElementById(userContext + 'FOUND_IN_RELEASE'     );
	var fldPREV_FOUND_IN_RELEASE = document.getElementById(userContext + 'PREV_FOUND_IN_RELEASE');
	if ( fldFOUND_IN_RELEASE_ID   != null ) fldFOUND_IN_RELEASE_ID.value   = '';
	if ( fldFOUND_IN_RELEASE      != null ) fldFOUND_IN_RELEASE.value      = '';
	if ( fldPREV_FOUND_IN_RELEASE != null ) fldPREV_FOUND_IN_RELEASE.value = '';
}

function RELEASES_FIXED_IN_RELEASE_Changed(fldFIXED_IN_RELEASE)
{
	var userContext = fldFIXED_IN_RELEASE.id.substring(0, fldFIXED_IN_RELEASE.id.length - 'FIXED_IN_RELEASE'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'FIXED_IN_RELEASE_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_FIXED_IN_RELEASE = document.getElementById(userContext + 'PREV_FIXED_IN_RELEASE');
	if ( fldPREV_FIXED_IN_RELEASE == null )
	{
		//alert('Could not find ' + userContext + 'PREV_FIXED_IN_RELEASE');
	}
	else if ( fldPREV_FIXED_IN_RELEASE.value != fldFIXED_IN_RELEASE.value )
	{
		if ( fldFIXED_IN_RELEASE.value.length > 0 )
		{
			try
			{
				SplendidCRM.Releases.AutoComplete.RELEASES_RELEASE_NAME_Get(fldFIXED_IN_RELEASE.value, RELEASES_FIXED_IN_RELEASE_Changed_OnSucceededWithContext, RELEASES_FIXED_IN_RELEASE_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('RELEASES_FIXED_IN_RELEASE_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			RELEASES_FIXED_IN_RELEASE_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function RELEASES_FIXED_IN_RELEASE_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors            = document.getElementById(userContext + 'FIXED_IN_RELEASE_AjaxErrors');
		var fldFIXED_IN_RELEASE_ID   = document.getElementById(userContext + 'FIXED_IN_RELEASE_ID'  );
		var fldFIXED_IN_RELEASE      = document.getElementById(userContext + 'FIXED_IN_RELEASE'     );
		var fldPREV_FIXED_IN_RELEASE = document.getElementById(userContext + 'PREV_FIXED_IN_RELEASE');
		if ( fldFIXED_IN_RELEASE_ID   != null ) fldFIXED_IN_RELEASE_ID.value   = sID  ;
		if ( fldFIXED_IN_RELEASE      != null ) fldFIXED_IN_RELEASE.value      = sNAME;
		if ( fldPREV_FIXED_IN_RELEASE != null ) fldPREV_FIXED_IN_RELEASE.value = sNAME;
	}
	else
	{
		alert('result from Releases.AutoComplete service is null');
	}
}

function RELEASES_FIXED_IN_RELEASE_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'FIXED_IN_RELEASE_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldFIXED_IN_RELEASE_ID   = document.getElementById(userContext + 'FIXED_IN_RELEASE_ID'  );
	var fldFIXED_IN_RELEASE      = document.getElementById(userContext + 'FIXED_IN_RELEASE'     );
	var fldPREV_FIXED_IN_RELEASE = document.getElementById(userContext + 'PREV_FIXED_IN_RELEASE');
	if ( fldFIXED_IN_RELEASE_ID   != null ) fldFIXED_IN_RELEASE_ID.value   = '';
	if ( fldFIXED_IN_RELEASE      != null ) fldFIXED_IN_RELEASE.value      = '';
	if ( fldPREV_FIXED_IN_RELEASE != null ) fldPREV_FIXED_IN_RELEASE.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


