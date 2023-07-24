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

function MODULES_MODULE_NAME_Changed(fldMODULE_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldMODULE_NAME.id.substring(0, fldMODULE_NAME.id.length - 'MODULE_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'MODULE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_MODULE_NAME = document.getElementById(userContext + 'PREV_MODULE_NAME');
	if ( fldPREV_MODULE_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_MODULE_NAME');
	}
	else if ( fldPREV_MODULE_NAME.value != fldMODULE_NAME.value )
	{
		if ( fldMODULE_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.Administration.Modules.AutoComplete.MODULES_MODULE_NAME_Get(fldMODULE_NAME.value, MODULES_MODULE_NAME_Changed_OnSucceededWithContext, MODULES_MODULE_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('MODULES_MODULE_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			MODULES_MODULE_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function MODULES_MODULE_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors       = document.getElementById(userContext + 'MODULE_NAME_AjaxErrors');
		var fldMODULE_ID        = document.getElementById(userContext + 'MODULE_ID'       );
		var fldMODULE_NAME      = document.getElementById(userContext + 'MODULE_NAME'     );
		var fldPREV_MODULE_NAME = document.getElementById(userContext + 'PREV_MODULE_NAME');
		if ( fldMODULE_ID        != null ) fldMODULE_ID.value        = sID  ;
		if ( fldMODULE_NAME      != null ) fldMODULE_NAME.value      = sNAME;
		if ( fldPREV_MODULE_NAME != null ) fldPREV_MODULE_NAME.value = sNAME;
		
		// 08/31/2009 Paul.  We want to automatically click the update button. 
		// In ordre for this to work, we must define our own command buttons in the GridView. 
		var btnUpdate = document.getElementById(userContext + 'btnUpdate');
		if ( btnUpdate != null )
		{
			btnUpdate.click();
		}
	}
	else
	{
		alert('result from Modules.AutoComplete service is null');
	}
}

function MODULES_MODULE_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'MODULE_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldMODULE_ID        = document.getElementById(userContext + 'MODULE_ID'       );
	var fldMODULE_NAME      = document.getElementById(userContext + 'MODULE_NAME'     );
	var fldPREV_MODULE_NAME = document.getElementById(userContext + 'PREV_MODULE_NAME');
	if ( fldMODULE_ID        != null ) fldMODULE_ID.value        = '';
	if ( fldMODULE_NAME      != null ) fldMODULE_NAME.value      = '';
	if ( fldPREV_MODULE_NAME != null ) fldPREV_MODULE_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

