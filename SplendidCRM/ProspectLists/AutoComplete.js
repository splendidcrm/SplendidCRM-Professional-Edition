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

function PROSPECT_LISTS_PROSPECT_LIST_NAME_Changed(fldPROSPECT_LIST_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldPROSPECT_LIST_NAME.id.substring(0, fldPROSPECT_LIST_NAME.id.length - 'PROSPECT_LIST_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'PROSPECT_LIST_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_PROSPECT_LIST_NAME = document.getElementById(userContext + 'PREV_PROSPECT_LIST_NAME');
	if ( fldPREV_PROSPECT_LIST_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_PROSPECT_LIST_NAME');
	}
	else if ( fldPREV_PROSPECT_LIST_NAME.value != fldPROSPECT_LIST_NAME.value )
	{
		if ( fldPROSPECT_LIST_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.ProspectLists.AutoComplete.PROSPECT_LISTS_PROSPECT_LIST_NAME_Get(fldPROSPECT_LIST_NAME.value, PROSPECT_LISTS_PROSPECT_LIST_NAME_Changed_OnSucceededWithContext, PROSPECT_LISTS_PROSPECT_LIST_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('PROSPECT_LISTS_PROSPECT_LIST_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			PROSPECT_LISTS_PROSPECT_LIST_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function PROSPECT_LISTS_PROSPECT_LIST_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors              = document.getElementById(userContext + 'PROSPECT_LIST_NAME_AjaxErrors');
		var fldPROSPECT_LIST_ID        = document.getElementById(userContext + 'PROSPECT_LIST_ID'       );
		var fldPROSPECT_LIST_NAME      = document.getElementById(userContext + 'PROSPECT_LIST_NAME'     );
		var fldPREV_PROSPECT_LIST_NAME = document.getElementById(userContext + 'PREV_PROSPECT_LIST_NAME');
		if ( fldPROSPECT_LIST_ID        != null ) fldPROSPECT_LIST_ID.value        = sID  ;
		if ( fldPROSPECT_LIST_NAME      != null ) fldPROSPECT_LIST_NAME.value      = sNAME;
		if ( fldPREV_PROSPECT_LIST_NAME != null ) fldPREV_PROSPECT_LIST_NAME.value = sNAME;
	}
	else
	{
		alert('result from ProspectLists.AutoComplete service is null');
	}
}

function PROSPECT_LISTS_PROSPECT_LIST_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'PROSPECT_LIST_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldPROSPECT_LIST_ID        = document.getElementById(userContext + 'PROSPECT_LIST_ID'       );
	var fldPROSPECT_LIST_NAME      = document.getElementById(userContext + 'PROSPECT_LIST_NAME'     );
	var fldPREV_PROSPECT_LIST_NAME = document.getElementById(userContext + 'PREV_PROSPECT_LIST_NAME');
	if ( fldPROSPECT_LIST_ID        != null ) fldPROSPECT_LIST_ID.value        = '';
	if ( fldPROSPECT_LIST_NAME      != null ) fldPROSPECT_LIST_NAME.value      = '';
	if ( fldPREV_PROSPECT_LIST_NAME != null ) fldPREV_PROSPECT_LIST_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


