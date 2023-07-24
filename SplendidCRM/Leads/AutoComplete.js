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

function LEADS_LEAD_LAST_NAME_Changed(fldLEAD_LAST_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain LAST_NAME in the text, so just get the length minus 4. 
	var userContext = fldLEAD_LAST_NAME.id.substring(0, fldLEAD_LAST_NAME.id.length - 'LEAD_LAST_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'LEAD_LAST_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_LEAD_LAST_NAME = document.getElementById(userContext + 'PREV_LEAD_LAST_NAME');
	if ( fldPREV_LEAD_LAST_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_LEAD_LAST_NAME');
	}
	else if ( fldPREV_LEAD_LAST_NAME.value != fldLEAD_LAST_NAME.value )
	{
		if ( fldLEAD_LAST_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Leads.AutoComplete.LEADS_LEAD_LAST_NAME_Get(fldLEAD_LAST_NAME.value, LEADS_LEAD_LAST_NAME_Changed_OnSucceededWithContext, LEADS_LEAD_LAST_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('LEADS_LEAD_LAST_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			LEADS_LEAD_LAST_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function LEADS_LEAD_LAST_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sLAST_NAME = result.LAST_NAME;
		
		var fldAjaxErrors          = document.getElementById(userContext + 'LEAD_LAST_NAME_AjaxErrors');
		var fldLEAD_ID             = document.getElementById(userContext + 'LEAD_ID'            );
		var fldLEAD_LAST_NAME      = document.getElementById(userContext + 'LEAD_LAST_NAME'     );
		var fldPREV_LEAD_LAST_NAME = document.getElementById(userContext + 'PREV_LEAD_LAST_NAME');
		if ( fldLEAD_ID             != null ) fldLEAD_ID.value             = sID       ;
		if ( fldLEAD_LAST_NAME      != null ) fldLEAD_LAST_NAME.value      = sLAST_NAME;
		if ( fldPREV_LEAD_LAST_NAME != null ) fldPREV_LEAD_LAST_NAME.value = sLAST_NAME;
	}
	else
	{
		alert('result from Leads.AutoComplete service is null');
	}
}

function LEADS_LEAD_LAST_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'LEAD_LAST_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldLEAD_ID        = document.getElementById(userContext + 'LEAD_ID'       );
	var fldLEAD_LAST_NAME      = document.getElementById(userContext + 'LEAD_LAST_NAME'     );
	var fldPREV_LEAD_LAST_NAME = document.getElementById(userContext + 'PREV_LEAD_LAST_NAME');
	if ( fldLEAD_ID        != null ) fldLEAD_ID.value        = '';
	if ( fldLEAD_LAST_NAME      != null ) fldLEAD_LAST_NAME.value      = '';
	if ( fldPREV_LEAD_LAST_NAME != null ) fldPREV_LEAD_LAST_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


