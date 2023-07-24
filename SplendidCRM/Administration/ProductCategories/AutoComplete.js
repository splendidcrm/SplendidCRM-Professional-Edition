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

function PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Changed(fldPRODUCT_CATEGORY_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldPRODUCT_CATEGORY_NAME.id.substring(0, fldPRODUCT_CATEGORY_NAME.id.length - 'PRODUCT_CATEGORY_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'PRODUCT_CATEGORY_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_PRODUCT_CATEGORY_NAME = document.getElementById(userContext + 'PREV_PRODUCT_CATEGORY_NAME');
	if ( fldPREV_PRODUCT_CATEGORY_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_PRODUCT_CATEGORY_NAME');
	}
	else if ( fldPREV_PRODUCT_CATEGORY_NAME.value != fldPRODUCT_CATEGORY_NAME.value )
	{
		if ( fldPRODUCT_CATEGORY_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.ProductCategories.AutoComplete.PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Get(fldPRODUCT_CATEGORY_NAME.value, PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Changed_OnSucceededWithContext, PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors                 = document.getElementById(userContext + 'PRODUCT_CATEGORY_NAME_AjaxErrors');
		var fldPRODUCT_CATEGORY_ID        = document.getElementById(userContext + 'PRODUCT_CATEGORY_ID'       );
		var fldPRODUCT_CATEGORY_NAME      = document.getElementById(userContext + 'PRODUCT_CATEGORY_NAME'     );
		var fldPREV_PRODUCT_CATEGORY_NAME = document.getElementById(userContext + 'PREV_PRODUCT_CATEGORY_NAME');
		if ( fldPRODUCT_CATEGORY_ID        != null ) fldPRODUCT_CATEGORY_ID.value        = sID  ;
		if ( fldPRODUCT_CATEGORY_NAME      != null ) fldPRODUCT_CATEGORY_NAME.value      = sNAME;
		if ( fldPREV_PRODUCT_CATEGORY_NAME != null ) fldPREV_PRODUCT_CATEGORY_NAME.value = sNAME;
	}
	else
	{
		alert('result from ProductCategories.AutoComplete service is null');
	}
}

function PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'PRODUCT_CATEGORY_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldPRODUCT_CATEGORY_ID        = document.getElementById(userContext + 'PRODUCT_CATEGORY_ID'       );
	var fldPRODUCT_CATEGORY_NAME      = document.getElementById(userContext + 'PRODUCT_CATEGORY_NAME'     );
	var fldPREV_PRODUCT_CATEGORY_NAME = document.getElementById(userContext + 'PREV_PRODUCT_CATEGORY_NAME');
	if ( fldPRODUCT_CATEGORY_ID        != null ) fldPRODUCT_CATEGORY_ID.value        = '';
	if ( fldPRODUCT_CATEGORY_NAME      != null ) fldPRODUCT_CATEGORY_NAME.value      = '';
	if ( fldPREV_PRODUCT_CATEGORY_NAME != null ) fldPREV_PRODUCT_CATEGORY_NAME.value = '';
}

// 08/05/2012 Paul.  The ProductTemplates.EditView uses PRODUCT_CATEGORIES_CATEGORY_NAME. 
function PRODUCT_CATEGORIES_CATEGORY_NAME_Changed(fldCATEGORY_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	var userContext = fldCATEGORY_NAME.id.substring(0, fldCATEGORY_NAME.id.length - 'CATEGORY_NAME'.length)
	var fldAjaxErrors = document.getElementById(userContext + 'CATEGORY_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_CATEGORY_NAME = document.getElementById(userContext + 'PREV_CATEGORY_NAME');
	if ( fldPREV_CATEGORY_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_CATEGORY_NAME');
	}
	else if ( fldPREV_CATEGORY_NAME.value != fldCATEGORY_NAME.value )
	{
		if ( fldCATEGORY_NAME.value.length > 0 )
		{
			try
			{
				SplendidCRM.ProductCategories.AutoComplete.PRODUCT_CATEGORIES_PRODUCT_CATEGORY_NAME_Get(fldCATEGORY_NAME.value, PRODUCT_CATEGORIES_CATEGORY_NAME_Changed_OnSucceededWithContext, PRODUCT_CATEGORIES_CATEGORY_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('PRODUCT_CATEGORIES_CATEGORY_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			PRODUCT_CATEGORIES_CATEGORY_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function PRODUCT_CATEGORIES_CATEGORY_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		var fldAjaxErrors                 = document.getElementById(userContext + 'CATEGORY_NAME_AjaxErrors');
		var fldCATEGORY_ID        = document.getElementById(userContext + 'CATEGORY_ID'       );
		var fldCATEGORY_NAME      = document.getElementById(userContext + 'CATEGORY_NAME'     );
		var fldPREV_CATEGORY_NAME = document.getElementById(userContext + 'PREV_CATEGORY_NAME');
		if ( fldCATEGORY_ID        != null ) fldCATEGORY_ID.value        = sID  ;
		if ( fldCATEGORY_NAME      != null ) fldCATEGORY_NAME.value      = sNAME;
		if ( fldPREV_CATEGORY_NAME != null ) fldPREV_CATEGORY_NAME.value = sNAME;
	}
	else
	{
		alert('result from ProductCategories.AutoComplete service is null');
	}
}

function PRODUCT_CATEGORIES_CATEGORY_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'CATEGORY_NAME_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldCATEGORY_ID        = document.getElementById(userContext + 'CATEGORY_ID'       );
	var fldCATEGORY_NAME      = document.getElementById(userContext + 'CATEGORY_NAME'     );
	var fldPREV_CATEGORY_NAME = document.getElementById(userContext + 'PREV_CATEGORY_NAME');
	if ( fldCATEGORY_ID        != null ) fldCATEGORY_ID.value        = '';
	if ( fldCATEGORY_NAME      != null ) fldCATEGORY_NAME.value      = '';
	if ( fldPREV_CATEGORY_NAME != null ) fldPREV_CATEGORY_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

