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

function ShowOptionsDialog(cbLoginComplete)
{
	try
	{
		if ( cbLoginComplete === undefined )
		{
			cbLoginComplete = function(status, message)
			{
				if ( status == 1 )
				{
					// 08/26/2014 Paul.  This is not the best place to set the last login value. 
					//localStorage['LastLoginRemote'] = false;
					SplendidError.SystemMessage('');
				}
			};
		}
		LoginViewUI_Load('divMainLayoutPanel', 'divMainActionsPanel', cbLoginComplete, function(status, message)
		{
			if ( status == 1 )
			{
				SplendidError.SystemMessage('');
			}
			else
			{
				SplendidError.SystemMessage(message);
			}
		});
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'Options.js ShowOptionsDialog()');
	}
}

