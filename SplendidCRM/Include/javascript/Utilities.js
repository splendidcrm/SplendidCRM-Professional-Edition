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

function SplendidCRM_ChangeFavorites(fld, sMODULE, gID)
{
	var fldAdd = document.getElementsByName('favAdd_' + gID);
	var fldRem = document.getElementsByName('favRem_' + gID);
	if ( fldAdd[0].style.display == 'none' )
		SplendidCRM_RemoveFromFavorites(fld, sMODULE, gID);
	else
		SplendidCRM_AddToFavorites(fld, sMODULE, gID);
}

function SplendidCRM_AddToFavorites(fld, sMODULE, gID)
{
	var userContext = gID;
	try
	{
		SplendidCRM.Utilities.Modules.AddToFavorites(sMODULE, gID, SplendidCRM_AddToFavorites_OnSucceededWithContext, SplendidCRM_AddToFavorites_OnFailed, userContext);
	}
	catch(e)
	{
		alert('SplendidCRM_AddToFavorites: ' + e.message);
	}
	return false;
}

function SplendidCRM_AddToFavorites_OnSucceededWithContext(result, userContext)
{
	if ( result )
	{
		var fldAdd = document.getElementsByName('favAdd_' + userContext);
		var fldRem = document.getElementsByName('favRem_' + userContext);
		fldAdd[0].style.display = 'none'  ;
		fldRem[0].style.display = 'inline';
	}
}

function SplendidCRM_AddToFavorites_OnFailed(error, userContext)
{
	alert('SplendidCRM_AddToFavorites_OnFailed: ' + error.Message);
}

function SplendidCRM_RemoveFromFavorites(fld, sMODULE, gID)
{
	var userContext = gID;
	try
	{
		SplendidCRM.Utilities.Modules.RemoveFromFavorites(sMODULE, gID, SplendidCRM_RemoveFromFavorites_OnSucceededWithContext, SplendidCRM_RemoveFromFavorites_OnFailed, userContext);
	}
	catch(e)
	{
		alert('SplendidCRM_RemoveFromFavorites: ' + e.message);
	}
	return false;
}

function SplendidCRM_RemoveFromFavorites_OnSucceededWithContext(result, userContext)
{
	if ( result )
	{
		var fldAdd = document.getElementsByName('favAdd_' + userContext);
		var fldRem = document.getElementsByName('favRem_' + userContext);
		fldAdd[0].style.display = 'inline';
		fldRem[0].style.display = 'none'  ;
	}
}

function SplendidCRM_RemoveFromFavorites_OnFailed(error, userContext)
{
	alert('SplendidCRM_RemoveFromFavorites_OnFailed: ' + error.Message);
}

// 10/09/2015 Paul.  Add methods to manage subscriptions. 
function SplendidCRM_ChangeFollowing(fld, sMODULE, gID)
{
	var fldAdd = document.getElementsByName('follow_'    + gID);
	var fldRem = document.getElementsByName('following_' + gID);
	if ( fldAdd[0].style.display == 'none' )
		SplendidCRM_RemoveSubscription(fld, sMODULE, gID);
	else
		SplendidCRM_AddSubscription(fld, sMODULE, gID);
}

function SplendidCRM_AddSubscription(fld, sMODULE, gID)
{
	var userContext = gID;
	try
	{
		SplendidCRM.Utilities.Modules.AddSubscription(sMODULE, gID, SplendidCRM_AddSubscription_OnSucceededWithContext, SplendidCRM_AddSubscription_OnFailed, userContext);
	}
	catch(e)
	{
		alert('SplendidCRM_AddSubscription: ' + e.message);
	}
	return false;
}

function SplendidCRM_AddSubscription_OnSucceededWithContext(result, userContext)
{
	if ( result )
	{
		var fldAdd = document.getElementsByName('follow_'    + userContext);
		var fldRem = document.getElementsByName('following_' + userContext);
		fldAdd[0].style.display = 'none'  ;
		fldRem[0].style.display = 'inline';
	}
}

function SplendidCRM_AddSubscription_OnFailed(error, userContext)
{
	alert('SplendidCRM_AddSubscription_OnFailed: ' + error.Message);
}

function SplendidCRM_RemoveSubscription(fld, sMODULE, gID)
{
	var userContext = gID;
	try
	{
		SplendidCRM.Utilities.Modules.RemoveSubscription(sMODULE, gID, SplendidCRM_RemoveSubscription_OnSucceededWithContext, SplendidCRM_RemoveSubscription_OnFailed, userContext);
	}
	catch(e)
	{
		alert('SplendidCRM_RemoveSubscription: ' + e.message);
	}
	return false;
}

function SplendidCRM_RemoveSubscription_OnSucceededWithContext(result, userContext)
{
	if ( result )
	{
		var fldAdd = document.getElementsByName('follow_' + userContext);
		var fldRem = document.getElementsByName('following_' + userContext);
		fldAdd[0].style.display = 'inline';
		fldRem[0].style.display = 'none'  ;
	}
}

function SplendidCRM_RemoveSubscription_OnFailed(error, userContext)
{
	alert('SplendidCRM_RemoveSubscription_OnFailed: ' + error.Message);
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();


