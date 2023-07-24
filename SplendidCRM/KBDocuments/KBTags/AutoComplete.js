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

// 05/06/2010 Paul.  Move the scripts using in KBTagSelect here so that they will be outside of the UpdatePanel. 
// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
var sKBTagSelectNameUserContext = '';
var sKBTagSelectNameUserSuffix  = '';
function ChangeKBTagSelect(sPARENT_ID, sPARENT_NAME)
{
	var fldKBTAG_NAME = document.getElementById(sKBTagSelectNameUserContext + 'KBTAG_NAME' + sKBTagSelectNameUserSuffix);
	if ( fldKBTAG_NAME != null )
	{
		fldKBTAG_NAME.value = sPARENT_NAME;
		try
		{
			KBTAGS_KBTAG_NAME_Changed(fldKBTAG_NAME);
		}
		catch(e)
		{
			alert('KBTagSelect - ChangeKBTagSelect: ' + e.message);
		}
	}
}

function NamePrefix(s, sSeparator)
{
	var nSeparatorIndex = s.lastIndexOf(sSeparator);
	if ( nSeparatorIndex > 0 )
	{
		return s.substring(0, nSeparatorIndex);
	}
}

function NameSuffix(s, sSeparator)
{
	var nSeparatorIndex = s.lastIndexOf(sSeparator);
	if ( nSeparatorIndex > 0 )
	{
		return s.substring(nSeparatorIndex + sSeparator.length, s.length);
	}
	return '';
}

function KBTagSelectPopup(fldSELECT_NAME)
{
	// 08/29/2009 Paul.  Use a different name for our KBTagSelect callback to prevent a collision with the ModulePopup code. 
	ChangeKBTag = ChangeKBTagSelect;
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	sKBTagSelectNameUserContext = fldSELECT_NAME.id.replace('SELECT_NAME', '');
	// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	sKBTagSelectNameUserContext = NamePrefix(fldSELECT_NAME.id, 'SELECT_NAME');
	sKBTagSelectNameUserSuffix  = NameSuffix(fldSELECT_NAME.id, 'SELECT_NAME');
	// 05/16/2010 Paul.  Make sure that our rootURL global javascript variable is always available. 
	// 01/27/2012 Paul.  Cannot use ASP.NET code here.  Must embed the string or use a public. 
	// 09/07/2013 Paul.  Change rootURL to sREMOTE_SERVER to match Survey module. 
	window.open(sREMOTE_SERVER + 'KBDocuments/KBTags/Popup.aspx', 'KBTagSelectPopup', 'width=600,height=400,resizable=1,scrollbars=1');
	return false;
}

function KBTAGS_KBTAG_NAME_ItemSelected(sender, e)
{
	KBTAGS_KBTAG_NAME_Changed(sender.get_element());
}

function KBTAGS_KBTAG_NAME_Changed(fldKBTAG_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	//var userContext = fldKBTAG_NAME.id.substring(0, fldKBTAG_NAME.id.length - 'KBTAG_NAME'.length)
	// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	var userContext = NamePrefix(fldKBTAG_NAME.id, 'KBTAG_NAME');
	var userSuffix  = NameSuffix(fldKBTAG_NAME.id, 'KBTAG_NAME');
	
	var fldAjaxErrors = document.getElementById(userContext + 'KBTAG_NAME_AjaxErrors' + userSuffix);
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_KBTAG_NAME = document.getElementById(userContext + 'PREV_KBTAG_NAME' + userSuffix);
	if ( fldPREV_KBTAG_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_KBTAG_NAME');
	}
	else if ( fldPREV_KBTAG_NAME.value != fldKBTAG_NAME.value )
	{
		if ( fldKBTAG_NAME.value.length > 0 )
		{
			try
			{
				sKBTagSelectNameUserContext = userContext;
				sKBTagSelectNameUserSuffix  = userSuffix ;
				SplendidCRM.KBDocuments.KBTags.AutoComplete.KBTAGS_KBTAG_NAME_Get(fldKBTAG_NAME.value, KBTAGS_KBTAG_NAME_Changed_OnSucceededWithContext, KBTAGS_KBTAG_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('KBTAGS_KBTAG_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			KBTAGS_KBTAG_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function KBTAGS_KBTAG_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
		var fldAjaxErrors      = document.getElementById(userContext + 'KBTAG_NAME_AjaxErrors' + sKBTagSelectNameUserSuffix);
		var fldKBTAG_ID        = document.getElementById(userContext + 'KBTAG_ID'        + sKBTagSelectNameUserSuffix);
		var fldKBTAG_NAME      = document.getElementById(userContext + 'KBTAG_NAME'      + sKBTagSelectNameUserSuffix);
		var fldPREV_KBTAG_NAME = document.getElementById(userContext + 'PREV_KBTAG_NAME' + sKBTagSelectNameUserSuffix);
		if ( fldKBTAG_ID        != null ) fldKBTAG_ID.value        = sID  ;
		if ( fldKBTAG_NAME      != null ) fldKBTAG_NAME.value      = sNAME;
		if ( fldPREV_KBTAG_NAME != null ) fldPREV_KBTAG_NAME.value = sNAME;
		
		// 08/31/2009 Paul.  We want to automatically click the update button. 
		// In ordre for this to work, we must define our own command buttons in the GridView. 
		var btnUpdate = document.getElementById(userContext + 'btnUpdate' + sKBTagSelectNameUserSuffix);
		if ( btnUpdate != null )
		{
			btnUpdate.click();
		}
	}
	else
	{
		alert('result from KBTags.AutoComplete service is null');
	}
}

function KBTAGS_KBTAG_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById(userContext + 'KBTAG_NAME_AjaxErrors' + sKBTagSelectNameUserSuffix);
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

		// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	var fldKBTAG_ID        = document.getElementById(userContext + 'KBTAG_ID'        + sKBTagSelectNameUserSuffix);
	var fldKBTAG_NAME      = document.getElementById(userContext + 'KBTAG_NAME'      + sKBTagSelectNameUserSuffix);
	var fldPREV_KBTAG_NAME = document.getElementById(userContext + 'PREV_KBTAG_NAME' + sKBTagSelectNameUserSuffix);
	if ( fldKBTAG_ID        != null ) fldKBTAG_ID.value        = '';
	if ( fldKBTAG_NAME      != null ) fldKBTAG_NAME.value      = '';
	if ( fldPREV_KBTAG_NAME != null ) fldPREV_KBTAG_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

