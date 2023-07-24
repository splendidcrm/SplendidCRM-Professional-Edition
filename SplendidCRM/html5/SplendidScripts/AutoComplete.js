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

function AutoComplete_ModuleMethod(sMODULE_NAME, sMETHOD, sREQUEST, callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	if ( sMODULE_NAME == 'Teams' )
		sMODULE_NAME = 'Administration/Teams';
	else if ( sMODULE_NAME == 'Tags' )
		sMODULE_NAME = 'Administration/Tags';
	// 06/07/2017 Paul.  Add NAICSCodes module. 
	else if ( sMODULE_NAME == 'NAICSCodes' )
		sMODULE_NAME = 'Administration/NAICSCodes';
	var xhr = CreateSplendidRequest(sMODULE_NAME + '/AutoComplete.asmx/' + sMETHOD);
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
						callback.call(context||this, 1, result.d);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'AutoComplete_ModuleMethod'));
				}
			});
		}
	}
	try
	{
		xhr.send(sREQUEST);
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'AutoComplete_ModuleMethod'));
	}
}

