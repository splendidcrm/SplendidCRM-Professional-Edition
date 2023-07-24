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

function SURVEYS_SURVEY_NAME_Changed(fldSURVEY_NAME)
{
	var userContext = fldSURVEY_NAME.id.substring(0, fldSURVEY_NAME.id.length - 'SURVEY_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'SURVEY_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_SURVEY_NAME = document.getElementById(userContext + 'PREV_SURVEY_NAME');
	if ( fldPREV_SURVEY_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_SURVEY_NAME');
	}
	else if ( fldPREV_SURVEY_NAME.value != fldSURVEY_NAME.value )
	{
		if ( fldSURVEY_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Surveys.AutoComplete.SURVEYS_SURVEY_NAME_Get(fldSURVEY_NAME.value, SURVEYS_SURVEY_NAME_Changed_OnSucceededWithContext, SURVEYS_SURVEY_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('SURVEYS_SURVEY_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			var result = { 'ID' : '', 'NAME' : '' };
			SURVEYS_SURVEY_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function SURVEYS_SURVEY_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors       = document.getElementById(userContext + 'SURVEY_NAME_AjaxErrors');
		var fldSURVEY_ID        = document.getElementById(userContext + 'SURVEY_ID'       );
		var fldSURVEY_NAME      = document.getElementById(userContext + 'SURVEY_NAME'     );
		var fldPREV_SURVEY_NAME = document.getElementById(userContext + 'PREV_SURVEY_NAME');
		if ( fldSURVEY_ID        != null ) fldSURVEY_ID.value        = sID  ;
		if ( fldSURVEY_NAME      != null ) fldSURVEY_NAME.value      = sNAME;
		if ( fldPREV_SURVEY_NAME != null ) fldPREV_SURVEY_NAME.value = sNAME;
	}
	else
	{
		alert('result from Surveys.AutoComplete service is null');
	}
}

function SURVEYS_SURVEY_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'SURVEY_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldSURVEY_ID        = document.getElementById(userContext + 'SURVEY_ID'       );
	var fldSURVEY_NAME      = document.getElementById(userContext + 'SURVEY_NAME'     );
	var fldPREV_SURVEY_NAME = document.getElementById(userContext + 'PREV_SURVEY_NAME');
	if ( fldSURVEY_ID        != null ) fldSURVEY_ID.value        = '';
	if ( fldSURVEY_NAME      != null ) fldSURVEY_NAME.value      = '';
	if ( fldPREV_SURVEY_NAME != null ) fldPREV_SURVEY_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


