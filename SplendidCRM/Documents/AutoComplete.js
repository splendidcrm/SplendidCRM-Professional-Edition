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

function DOCUMENTS_DOCUMENT_NAME_Changed(fldDOCUMENT_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain DOCUMENT_NAME in the text, so just get the length minus 4. 
	var userContext = fldDOCUMENT_NAME.id.substring(0, fldDOCUMENT_NAME.id.length - 'DOCUMENT_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'DOCUMENT_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_DOCUMENT_NAME = document.getElementById(userContext + 'PREV_DOCUMENT_NAME');
	if ( fldPREV_DOCUMENT_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_DOCUMENT_NAME');
	}
	else if ( fldPREV_DOCUMENT_NAME.value != fldDOCUMENT_NAME.value )
	{
		if ( fldDOCUMENT_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Documents.AutoComplete.DOCUMENTS_DOCUMENT_NAME_Get(fldDOCUMENT_NAME.value, DOCUMENTS_DOCUMENT_NAME_Changed_OnSucceededWithContext, DOCUMENTS_DOCUMENT_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('DOCUMENTS_DOCUMENT_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'DOCUMENT_NAME' : '' };
			DOCUMENTS_DOCUMENT_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function DOCUMENTS_DOCUMENT_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sDOCUMENT_NAME = result.DOCUMENT_NAME;
		
		var fldAjaxErrors         = document.getElementById(userContext + 'DOCUMENT_NAME_AjaxErrors');
		var fldDOCUMENT_ID        = document.getElementById(userContext + 'DOCUMENT_ID'       );
		var fldDOCUMENT_NAME      = document.getElementById(userContext + 'DOCUMENT_NAME'     );
		var fldPREV_DOCUMENT_NAME = document.getElementById(userContext + 'PREV_DOCUMENT_NAME');
		if ( fldDOCUMENT_ID        != null ) fldDOCUMENT_ID.value        = sID           ;
		if ( fldDOCUMENT_NAME      != null ) fldDOCUMENT_NAME.value      = sDOCUMENT_NAME;
		if ( fldPREV_DOCUMENT_NAME != null ) fldPREV_DOCUMENT_NAME.value = sDOCUMENT_NAME;
	}
	else
	{
		alert('result from Documents.AutoComplete service is null');
	}
}

function DOCUMENTS_DOCUMENT_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'DOCUMENT_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldDOCUMENT_ID        = document.getElementById(userContext + 'DOCUMENT_ID'       );
	var fldDOCUMENT_NAME      = document.getElementById(userContext + 'DOCUMENT_NAME'     );
	var fldPREV_DOCUMENT_NAME = document.getElementById(userContext + 'PREV_DOCUMENT_NAME');
	if ( fldDOCUMENT_ID        != null ) fldDOCUMENT_ID.value        = '';
	if ( fldDOCUMENT_NAME      != null ) fldDOCUMENT_NAME.value      = '';
	if ( fldPREV_DOCUMENT_NAME != null ) fldPREV_DOCUMENT_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


