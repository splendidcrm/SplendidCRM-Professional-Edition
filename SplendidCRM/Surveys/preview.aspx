<%@ Page language="c#" Codebehind="preview.aspx.cs" AutoEventWireup="false" EnableViewState="false" Inherits="SplendidCRM.Surveys.Preview" %>
<script runat="server">
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
</script>
<!DOCTYPE HTML>
<html id="htmlRoot" runat="server">
<head runat="server">
	<meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=yes, target-densitydpi=device-dpi" />
	<link type="text/css" rel="stylesheet" href="~/Include/javascript/jquery-ui-1.9.1.custom.css" runat="server" />
</head>
<body style="background-color: white;">
<form id="frmMain" method="post" runat="server">
<ajaxToolkit:ToolkitScriptManager ID="mgrAjax" CombineScripts="true" EnableScriptGlobalization="true" EnableScriptLocalization="false" ScriptMode="Release" runat="server" />

<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
var sREMOTE_SERVER       = '<%# Application["rootURL"] %>';
// 04/23/2018 Paul.  Build in javascript to allow proxy handling. 
sREMOTE_SERVER           = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port: '') + '<%# Sql.ToString(Application["rootURL"]) %>';
// 12/01/2014 Paul.  bREMOTE_ENABLED needs to be in the UI page so that it can be quickly accessed by the Formatting functions. 
var bREMOTE_ENABLED      = false;
// 06/24/2017 Paul.  We need a way to turn off bootstrap for BPMN, ReportDesigner and ChatDashboard. 
var bDESKTOP_LAYOUT      = true;
// 06/23/2017 Paul.  sPLATFORM_LAYOUT is required with latest build. 
var sPLATFORM_LAYOUT     = '';
var sAUTHENTICATION      = '';
var sHEADER_LOGO_IMAGE   = '<%# Application["CONFIG.header_logo_image" ] %>';
var sHEADER_LOGO_WIDTH   = '<%# Application["CONFIG.header_logo_width" ] %>';
var sHEADER_LOGO_HEIGHT  = '<%# Application["CONFIG.header_logo_height"] %>';
var sHEADER_LOGO_STYLE   = '<%# Application["CONFIG.header_logo_style" ] %>';
var sCOMPANY_NAME        = '<%# Application["CONFIG.company_name"      ] %>';
var sLBL_EXIT_LINK       = '<%# L10n.Term("Surveys.LBL_EXIT_LINK"      ) %>';
var sLBL_PREV_LINK       = '<%# L10n.Term("Surveys.LBL_PREV_LINK"      ) %>';
var sLBL_NEXT_LINK       = '<%# L10n.Term("Surveys.LBL_NEXT_LINK"      ) %>';
var sLBL_SUBMIT_LINK     = '<%# L10n.Term("Surveys.LBL_SUBMIT_LINK"    ) %>';
var sLBL_SURVEY_COMPLETE = '<%# L10n.Term("Surveys.LBL_SURVEY_COMPLETE") %>';
// 08/17/2018 Paul.  Date format is needed for date validation. 
var sUSER_DATE_FORMAT    = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"])) %>';

$(document).ready(function()
{
	// 12/30/2015 Paul.  Build runtime header manually so that we can adjust for mobile dynamically. 
	var divSurveyBody = document.getElementById('divSurveyBody');
	Survey_CreateRuntimeHeader(divSurveyBody);
	var lblError = document.getElementById('lblError');
	try
	{
		var divSurveyExitLink = document.getElementById('divSurveyExitLink');
		divSurveyExitLink.innerHTML = sLBL_EXIT_LINK;
		divSurveyExitLink.onclick = function()
		{
			window.close();
		};

		if ( !Sql.IsEmptyString(sHEADER_LOGO_IMAGE) )
		{
			var divSurveyLogo = document.getElementById('divSurveyLogo');
			var img = document.createElement('img');
			img.id  = 'imgSurveyLogo';
			if ( StartsWith(sHEADER_LOGO_IMAGE, 'http') )
				img.src    = sHEADER_LOGO_IMAGE;
			else if ( StartsWith(sHEADER_LOGO_IMAGE, '~/') )
				img.src    = sREMOTE_SERVER + sHEADER_LOGO_IMAGE.substring(2, sHEADER_LOGO_IMAGE.length);
			else
				img.src    = sREMOTE_SERVER + 'Include/images/' + sHEADER_LOGO_IMAGE;
			
			if (!Sql.IsEmptyString(sHEADER_LOGO_WIDTH)) img.width = sHEADER_LOGO_WIDTH;
			if ( !Sql.IsEmptyString(sHEADER_LOGO_HEIGHT) ) img.height        = sHEADER_LOGO_HEIGHT;
			if ( !Sql.IsEmptyString(sCOMPANY_NAME      ) ) img.alt           = sCOMPANY_NAME      ;
			if ( !Sql.IsEmptyString(sHEADER_LOGO_STYLE ) ) img.style.cssText = sHEADER_LOGO_STYLE ;
			divSurveyLogo.appendChild(img);
		}
		
		var sID = getUrlParam('id');
		if ( !Sql.IsEmptyString(sID) )
		{
			lblError.innerHTML = 'Retrieving data...';
			Survey_LoadItem('Surveys', sID, function(status, message)
			{
				var lblError = document.getElementById('lblError');
				if ( status == 1 )
				{
					try
					{
						rowSURVEY = message;
						lblError.innerHTML = 'Rendering data...';
					
						var survey = new Survey(rowSURVEY);
						survey.LBL_EXIT_LINK       = sLBL_EXIT_LINK      ;
						survey.LBL_PREV_LINK       = sLBL_PREV_LINK      ;
						survey.LBL_NEXT_LINK       = sLBL_NEXT_LINK      ;
						survey.LBL_SUBMIT_LINK     = sLBL_SUBMIT_LINK    ;
						survey.LBL_SURVEY_COMPLETE = sLBL_SURVEY_COMPLETE;
						survey.Render(null, false);
						survey.SubmitResults = function(page, nPageIndex, bSurveyComplete, callback)
						{
							if ( bSurveyComplete )
								window.close();
							else
								callback(1, null);
						}
						lblError.innerHTML = '';
					}
					catch(e)
					{
						lblError.innerHTML = 'Preview Render: ' + e.message;
					}
				}
				else
				{
					lblError.innerHTML = 'Survey_LoadItem Response Error: ' + message;
				}
			});
		}
		else
		{
			lblError.innerHTML = 'Survey ID not specified.';
		}
	}
	catch(e)
	{
		lblError.innerHTML = 'Ready: ' + e.message;
	}
});
	</script>
</SplendidCRM:InlineScript>
<div id="divSurveyBody" class="SurveyBody">
</div>
</form>
<%@ Register TagPrefix="SplendidCRM" Tagname="Copyright" Src="~/_controls/Copyright.ascx" %>
<SplendidCRM:Copyright ID="ctlCopyright" Runat="Server" />
</body>
</html>
