/*
 * Copyright (C) 2005-2023 SplendidCRM Software, Inc. All rights reserved.
 *
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License 
 * Agreement, or other written agreement between you and SplendidCRM ("License"). By installing or 
 * using this file, you have unconditionally agreed to the terms and conditions of the License, 
 * including but not limited to restrictions on the number of users therein, and you may not use this 
 * file except in compliance with the License. 
 * 
 */

function ProcessButtons_GetProcessStatus(gPENDING_PROCESS_ID, callback, context)
{
	var xhr = CreateSplendidRequest('Processes/Rest.svc/GetProcessStatus?ID=' + gPENDING_PROCESS_ID, 'GET');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							// 10/04/2011 Paul.  DetailViewUI.LoadItem returns the row. 
							callback.call(context||this, 1, result.d.results);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ProcessButtons_GetProcessStatus'));
				}
			});
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'ProcessButtons_GetProcessStatus'));
	}
}

function ProcessButtons_ProcessAction(sACTION, gPENDING_PROCESS_ID, gPROCESS_USER_ID, sPROCESS_NOTES, callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	var xhr = CreateSplendidRequest('Processes/Rest.svc/ProcessAction', 'POST', 'application/octet-stream');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						callback.call(context||this, 1, null);
					}
					else if ( result.status == 0 )
					{
						callback.call(context||this, -1, 'A record cannot be processed when offline.');
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ProcessButtons_ProcessAction'));
				}
			});
		}
	}
	try
	{
		var obj = new Object();
		obj.ACTION             = sACTION            ;
		obj.PENDING_PROCESS_ID = gPENDING_PROCESS_ID;
		obj.PROCESS_USER_ID    = gPROCESS_USER_ID   ;
		obj.PROCESS_NOTES      = sPROCESS_NOTES     ;
		xhr.send(JSON.stringify(obj));
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'ProcessButtons_ProcessAction'));
	}
}

function ProcessButtons_ProcessUsers(gTEAM_ID, sSORT_FIELD, sSORT_DIRECTION, sSELECT, sFILTER, callback, context)
{
	// 03/01/2013 Paul.  If sSORT_FIELD is not provided, then clear sSORT_DIRECTION. 
	if ( sSORT_FIELD === undefined || sSORT_FIELD == null || sSORT_FIELD == '' )
	{
		sSORT_FIELD     = '';
		sSORT_DIRECTION = '';
	}
	var xhr = CreateSplendidRequest('Processes/Rest.svc/ProcessUsers?TEAM_ID=' + gTEAM_ID + '&$orderby=' + encodeURIComponent(sSORT_FIELD + ' ' + sSORT_DIRECTION) + '&$select=' + escape(sSELECT) + '&$filter=' + escape(sFILTER), 'GET');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							// 10/04/2011 Paul.  ListView_LoadModule returns the rows. 
							callback.call(context||this, 1, result.d.results);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadModule'));
				}
			}, context||this);
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadModule'));
	}
}

