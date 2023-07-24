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

function SURVEY_QUESTIONS_DESCRIPTION_Changed(fldSURVEY_QUESTIONS_DESCRIPTION)
{
	var userContext = fldSURVEY_QUESTIONS_DESCRIPTION.id.substring(0, fldSURVEY_QUESTIONS_DESCRIPTION.id.length - 'SURVEY_QUESTIONS_DESCRIPTION'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'SURVEY_QUESTIONS_DESCRIPTION_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_SURVEY_QUESTIONS_DESCRIPTION = document.getElementById(userContext + 'PREV_SURVEY_QUESTIONS_DESCRIPTION');
	if ( fldPREV_SURVEY_QUESTIONS_DESCRIPTION == null )
	{
		//alert('Could not find ' + userContext + 'PREV_SURVEY_QUESTIONS_DESCRIPTION');
	}
	else if ( fldPREV_SURVEY_QUESTIONS_DESCRIPTION.value != fldSURVEY_QUESTIONS_DESCRIPTION.value )
	{
		if ( fldSURVEY_QUESTIONS_DESCRIPTION.value.length > 0 )
		{
			try
			{
				SplendidCRM.SurveyQuestions.AutoComplete.SURVEY_QUESTIONS_DESCRIPTION_Get(fldSURVEY_QUESTIONS_DESCRIPTION.value, SURVEY_QUESTIONS_DESCRIPTION_Changed_OnSucceededWithContext, SURVEY_QUESTIONS_DESCRIPTION_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('SURVEY_QUESTIONS_DESCRIPTION_Changed: ' + e.message);
			}
		}
		else
		{
			var result = { 'ID' : '', 'NAME' : '' };
			SURVEY_QUESTIONS_DESCRIPTION_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function SURVEY_QUESTIONS_DESCRIPTION_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors                        = document.getElementById(userContext + 'SURVEY_QUESTIONS_DESCRIPTION_AjaxErrors');
		var fldSURVEY_QUESTIONS_DESCRIPTION      = document.getElementById(userContext + 'SURVEY_QUESTIONS_DESCRIPTION'           );
		var fldPREV_SURVEY_QUESTIONS_DESCRIPTION = document.getElementById(userContext + 'PREV_SURVEY_QUESTIONS_DESCRIPTION'      );
		if ( fldSURVEY_QUESTIONS_DESCRIPTION      != null ) fldSURVEY_QUESTIONS_DESCRIPTION.value      = sNAME;
		if ( fldPREV_SURVEY_QUESTIONS_DESCRIPTION != null ) fldPREV_SURVEY_QUESTIONS_DESCRIPTION.value = sNAME;
	}
	else
	{
		alert('result from SurveyQuestions.AutoComplete service is null');
	}
}

function SURVEY_QUESTIONS_DESCRIPTION_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'SURVEY_QUESTIONS_DESCRIPTION_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldSURVEY_QUESTIONS_DESCRIPTION      = document.getElementById(userContext + 'SURVEY_QUESTIONS_DESCRIPTION'     );
	var fldPREV_SURVEY_QUESTIONS_DESCRIPTION = document.getElementById(userContext + 'PREV_SURVEY_QUESTIONS_DESCRIPTION');
	if ( fldSURVEY_QUESTIONS_DESCRIPTION      != null ) fldSURVEY_QUESTIONS_DESCRIPTION.value      = '';
	if ( fldPREV_SURVEY_QUESTIONS_DESCRIPTION != null ) fldPREV_SURVEY_QUESTIONS_DESCRIPTION.value = '';
}


function SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Changed(fldSURVEY_QUESTION_NAME)
{
	var userContext = fldSURVEY_QUESTION_NAME.id.substring(0, fldSURVEY_QUESTION_NAME.id.length - 'SURVEY_QUESTION_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'SURVEY_QUESTION_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_SURVEY_QUESTION_NAME = document.getElementById(userContext + 'PREV_SURVEY_QUESTION_NAME');
	if ( fldPREV_SURVEY_QUESTION_NAME == null )
	{
		alert('Could not find ' + userContext + 'PREV_SURVEY_QUESTION_NAME');
	}
	else if ( fldPREV_SURVEY_QUESTION_NAME.value != fldSURVEY_QUESTION_NAME.value )
	{
		if ( fldSURVEY_QUESTION_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.SurveyQuestions.AutoComplete.SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Get(fldSURVEY_QUESTION_NAME.value, SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Changed_OnSucceededWithContext, SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			var result = { 'ID' : '', 'NAME' : '' };
			SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors                = document.getElementById(userContext + 'SURVEY_QUESTION_NAME_AjaxErrors');
		var fldSURVEY_QUESTION_ID        = document.getElementById(userContext + 'SURVEY_QUESTION_ID'             );
		var fldSURVEY_QUESTION_NAME      = document.getElementById(userContext + 'SURVEY_QUESTION_NAME'           );
		var fldPREV_SURVEY_QUESTION_NAME = document.getElementById(userContext + 'PREV_SURVEY_QUESTION_NAME'      );
		if ( fldSURVEY_QUESTION_ID        != null ) fldSURVEY_QUESTION_ID.value        = sID  ;
		if ( fldSURVEY_QUESTION_NAME      != null ) fldSURVEY_QUESTION_NAME.value      = sNAME;
		if ( fldPREV_SURVEY_QUESTION_NAME != null ) fldPREV_SURVEY_QUESTION_NAME.value = sNAME;
	}
	else
	{
		alert('result from SurveyQuestions.AutoComplete service is null');
	}
}

function SURVEY_QUESTIONS_SURVEY_QUESTION_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'SURVEY_QUESTION_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldSURVEY_QUESTION_ID        = document.getElementById(userContext + 'SURVEY_QUESTION_ID'       );
	var fldSURVEY_QUESTION_NAME      = document.getElementById(userContext + 'SURVEY_QUESTION_NAME'     );
	var fldPREV_SURVEY_QUESTION_NAME = document.getElementById(userContext + 'PREV_SURVEY_QUESTION_NAME');
	if ( fldSURVEY_QUESTION_ID        != null ) fldSURVEY_QUESTION_ID.value        = '';
	if ( fldSURVEY_QUESTION_NAME      != null ) fldSURVEY_QUESTION_NAME.value      = '';
	if ( fldPREV_SURVEY_QUESTION_NAME != null ) fldPREV_SURVEY_QUESTION_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


