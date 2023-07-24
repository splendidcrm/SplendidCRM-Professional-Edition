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

// 05/06/2010 Paul.  Move the scripts using in TeamSelect here so that they will be outside of the UpdatePanel. 
// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
var sTeamSelectNameUserContext = '';
var sTeamSelectNameUserSuffix  = '';
function ChangeTeamSelect(sPARENT_ID, sPARENT_NAME)
{
	var fldTEAM_NAME = document.getElementById(sTeamSelectNameUserContext + 'TEAM_NAME' + sTeamSelectNameUserSuffix);
	if ( fldTEAM_NAME != null )
	{
		fldTEAM_NAME.value = sPARENT_NAME;
		try
		{
			TEAMS_TEAM_NAME_Changed(fldTEAM_NAME);
		}
		catch(e)
		{
			alert('TeamSelect - ChangeTeamSelect: ' + e.message);
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

function TeamSelectPopup(fldSELECT_NAME)
{
	// 08/29/2009 Paul.  Use a different name for our TeamSelect callback to prevent a collision with the ModulePopup code. 
	ChangeTeam = ChangeTeamSelect;
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	//sTeamSelectNameUserContext = fldSELECT_NAME.id.replace('SELECT_NAME', '');
	// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	sTeamSelectNameUserContext = NamePrefix(fldSELECT_NAME.id, 'SELECT_NAME');
	sTeamSelectNameUserSuffix  = NameSuffix(fldSELECT_NAME.id, 'SELECT_NAME');
	// 05/16/2010 Paul.  Make sure that our rootURL global javascript variable is always available. 
	// 01/27/2012 Paul.  Cannot use ASP.NET code here.  Must embed the string or use a public. 
	// 09/07/2013 Paul.  Change rootURL to sREMOTE_SERVER to match Survey module. 
	// 05/12/2016 Paul.  Increase default popup size. 
	window.open(sREMOTE_SERVER + 'Administration/Teams/Popup.aspx', 'TeamSelectPopup', 'width=900,height=900,resizable=1,scrollbars=1');
	return false;
}

function TEAMS_TEAM_NAME_ItemSelected(sender, e)
{
	TEAMS_TEAM_NAME_Changed(sender.get_element());
}

function TEAMS_TEAM_NAME_Changed(fldTEAM_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	// 08/24/2009 Paul.  One of the base controls can contain NAME in the text, so just get the length minus 4. 
	//var userContext = fldTEAM_NAME.id.substring(0, fldTEAM_NAME.id.length - 'TEAM_NAME'.length)
	// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	var userContext = NamePrefix(fldTEAM_NAME.id, 'TEAM_NAME');
	var userSuffix  = NameSuffix(fldTEAM_NAME.id, 'TEAM_NAME');

	var fldAjaxErrors = document.getElementById(userContext + 'TEAM_NAME_AjaxErrors' + userSuffix);
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_TEAM_NAME = document.getElementById(userContext + 'PREV_TEAM_NAME' + userSuffix);
	if ( fldPREV_TEAM_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_TEAM_NAME');
	}
	else if ( fldPREV_TEAM_NAME.value != fldTEAM_NAME.value )
	{
		if ( fldTEAM_NAME.value.length > 0 )
		{
			try
			{
				sTeamSelectNameUserContext = userContext;
				sTeamSelectNameUserSuffix  = userSuffix ;
				SplendidCRM.Administration.Teams.AutoComplete.TEAMS_TEAM_NAME_Get(fldTEAM_NAME.value, TEAMS_TEAM_NAME_Changed_OnSucceededWithContext, TEAMS_TEAM_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('TEAMS_TEAM_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			TEAMS_TEAM_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function TEAMS_TEAM_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
		var fldAjaxErrors     = document.getElementById(userContext + 'TEAM_NAME_AjaxErrors' + sTeamSelectNameUserSuffix);
		var fldTEAM_ID        = document.getElementById(userContext + 'TEAM_ID'        + sTeamSelectNameUserSuffix);
		var fldTEAM_NAME      = document.getElementById(userContext + 'TEAM_NAME'      + sTeamSelectNameUserSuffix);
		var fldPREV_TEAM_NAME = document.getElementById(userContext + 'PREV_TEAM_NAME' + sTeamSelectNameUserSuffix);
		if ( fldTEAM_ID        != null ) fldTEAM_ID.value        = sID  ;
		if ( fldTEAM_NAME      != null ) fldTEAM_NAME.value      = sNAME;
		if ( fldPREV_TEAM_NAME != null ) fldPREV_TEAM_NAME.value = sNAME;
		
		// 08/31/2009 Paul.  We want to automatically click the update button. 
		// In order for this to work, we must define our own command buttons in the GridView. 
		var btnUpdate = document.getElementById(userContext + 'btnUpdate' + sTeamSelectNameUserSuffix);
		if ( btnUpdate != null )
		{
			btnUpdate.click();
		}
	}
	else
	{
		alert('result from Teams.AutoComplete service is null');
	}
}

function TEAMS_TEAM_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	var fldAjaxErrors = document.getElementById(userContext + 'TEAM_NAME_AjaxErrors' + sTeamSelectNameUserSuffix);
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldTEAM_ID        = document.getElementById(userContext + 'TEAM_ID'        + sTeamSelectNameUserSuffix);
	var fldTEAM_NAME      = document.getElementById(userContext + 'TEAM_NAME'      + sTeamSelectNameUserSuffix);
	var fldPREV_TEAM_NAME = document.getElementById(userContext + 'PREV_TEAM_NAME' + sTeamSelectNameUserSuffix);
	if ( fldTEAM_ID        != null ) fldTEAM_ID.value        = '';
	if ( fldTEAM_NAME      != null ) fldTEAM_NAME.value      = '';
	if ( fldPREV_TEAM_NAME != null ) fldPREV_TEAM_NAME.value = '';
}

function TEAMS_PARENT_NAME_Changed(fldPARENT_NAME)
{
	var userContext = NamePrefix(fldPARENT_NAME.id, 'PARENT_NAME');
	var userSuffix  = NameSuffix(fldPARENT_NAME.id, 'PARENT_NAME');

	var fldAjaxErrors = document.getElementById(userContext + 'PARENT_NAME_AjaxErrors' + userSuffix);
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';
	
	var fldPREV_PARENT_NAME = document.getElementById(userContext + 'PREV_PARENT_NAME' + userSuffix);
	if ( fldPREV_PARENT_NAME == null )
	{
		//alert('Could not find ' + userContext + 'PREV_PARENT_NAME');
	}
	else if ( fldPREV_PARENT_NAME.value != fldPARENT_NAME.value )
	{
		if ( fldPARENT_NAME.value.length > 0 )
		{
			try
			{
				sTeamSelectNameUserContext = userContext;
				sTeamSelectNameUserSuffix  = userSuffix ;
				SplendidCRM.Administration.Teams.AutoComplete.TEAMS_TEAM_NAME_Get(fldPARENT_NAME.value, TEAMS_PARENT_NAME_Changed_OnSucceededWithContext, TEAMS_PARENT_NAME_Changed_OnFailed, userContext);
			}
			catch(e)
			{
				alert('TEAMS_PARENT_NAME_Changed: ' + e.message);
			}
		}
		else
		{
			// 08/30/2010 Paul.  If the name was cleared, then we must also clear the hidden ID field. 
			var result = { 'ID' : '', 'NAME' : '' };
			TEAMS_PARENT_NAME_Changed_OnSucceededWithContext(result, userContext, null);
		}
	}
}

function TEAMS_PARENT_NAME_Changed_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID   = result.ID  ;
		var sNAME = result.NAME;
		
		// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
		var fldAjaxErrors       = document.getElementById(userContext + 'PARENT_NAME_AjaxErrors' + sTeamSelectNameUserSuffix);
		var fldPARENT_ID        = document.getElementById(userContext + 'PARENT_ID'        + sTeamSelectNameUserSuffix);
		var fldPARENT_NAME      = document.getElementById(userContext + 'PARENT_NAME'      + sTeamSelectNameUserSuffix);
		var fldPREV_PARENT_NAME = document.getElementById(userContext + 'PREV_PARENT_NAME' + sTeamSelectNameUserSuffix);
		if ( fldPARENT_ID        != null ) fldPARENT_ID.value        = sID  ;
		if ( fldPARENT_NAME      != null ) fldPARENT_NAME.value      = sNAME;
		if ( fldPREV_PARENT_NAME != null ) fldPREV_PARENT_NAME.value = sNAME;
		
		// 08/31/2009 Paul.  We want to automatically click the update button. 
		// In order for this to work, we must define our own command buttons in the GridView. 
		var btnUpdate = document.getElementById(userContext + 'btnUpdate' + sTeamSelectNameUserSuffix);
		if ( btnUpdate != null )
		{
			btnUpdate.click();
		}
	}
	else
	{
		alert('result from Teams.AutoComplete service is null');
	}
}

function TEAMS_PARENT_NAME_Changed_OnFailed(error, userContext)
{
	// Display the error.
	// 03/17/2011 Paul.  .NET 4.0 appends a row index to the field IDs. 
	var fldAjaxErrors = document.getElementById(userContext + 'PARENT_NAME_AjaxErrors' + sTeamSelectNameUserSuffix);
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '<br />' + error.get_message();

	var fldPARENT_ID        = document.getElementById(userContext + 'PARENT_ID'        + sTeamSelectNameUserSuffix);
	var fldPARENT_NAME      = document.getElementById(userContext + 'PARENT_NAME'      + sTeamSelectNameUserSuffix);
	var fldPREV_PARENT_NAME = document.getElementById(userContext + 'PREV_PARENT_NAME' + sTeamSelectNameUserSuffix);
	if ( fldPARENT_ID        != null ) fldPARENT_ID.value        = '';
	if ( fldPARENT_NAME      != null ) fldPARENT_NAME.value      = '';
	if ( fldPREV_PARENT_NAME != null ) fldPREV_PARENT_NAME.value = '';
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

