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

define(function()
{
	return {
		Render: function(sLayoutPanel, sActionsPanel, sSCRIPT_URL, sSETTINGS_EDITVIEW, sDEFAULT_SETTINGS)
		{
			var divDashboardPanel = document.getElementById(sLayoutPanel);
			if ( divDashboardPanel != null )
			{
				var divDashletBody = document.createElement('div');
				divDashletBody.id = sLayoutPanel + '_divDashletBody';
				divDashletBody.align = 'center';
				divDashboardPanel.appendChild(divDashletBody);
				var divDashletError = document.createElement('div');
				divDashletError.id = sLayoutPanel + '_divDashletError';
				divDashletError.className = 'error';
				divDashletBody.appendChild(divDashletError);
				var divDashletHTML5 = document.createElement('div');
				divDashletHTML5.id = sLayoutPanel + '_divDashletHTML5';
				divDashletHTML5.style.width = '100%';
				divDashletBody.appendChild(divDashletHTML5);
				
				var rowDefaultSearch = Sql.ParseFormData(sDEFAULT_SETTINGS);
				var sCHART_ID = rowDefaultSearch.CHART_ID;
				var sSCRIPT_URL = '~/Charts/view_embedded.aspx?ID=' + sCHART_ID;
				// 06/21/2017 Paul.  On a mobile device, we cannot access the parent frame, so hardcode the height to 600px. 
				if ( !bMOBILE_CLIENT )
					sSCRIPT_URL += '&ParentFrame=' + sLayoutPanel + '_divDashletHTML5_frame';
				sSCRIPT_URL = sSCRIPT_URL.replace('~/', sREMOTE_SERVER);
				try
				{
					var divDashletHTML5_frame = document.createElement('div');
					divDashletHTML5_frame.className    = 'embed-responsive';
					// 06/21/2017 Paul.  This is where we set the initial height of the frame.  It becomes the final height on a mobile device. 
					divDashletHTML5_frame.style.height = '600px';
					divDashletHTML5_frame.id           = sLayoutPanel + '_divDashletHTML5_frame';
					divDashletHTML5.appendChild(divDashletHTML5_frame);
					var frame = document.createElement('iframe');
					frame.className = 'embed-responsive-item';  // embed-responsive-4by3, embed-responsive-1by1
					frame.src       = sSCRIPT_URL;
					frame.width     = '100%';
					frame.height    = '100%';
					divDashletHTML5_frame.appendChild(frame);
				}
				catch(e)
				{
					$('#' + sLayoutPanel + '_divDashletError').text(e.message);
				}
			}
		},
	};
});
