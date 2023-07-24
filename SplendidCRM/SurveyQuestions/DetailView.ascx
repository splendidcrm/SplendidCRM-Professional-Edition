<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.SurveyQuestions.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divDetailView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="SurveyQuestions" EnablePrint="false" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailNavigation" Src="~/_controls/DetailNavigation.ascx" %>
	<SplendidCRM:DetailNavigation ID="ctlDetailNavigation" Module="<%# m_sMODULE %>" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:HiddenField ID="LAYOUT_DETAIL_VIEW" Runat="server" />
	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>

	<div id="divQuestionDetailView"></div>
	<div align="center">
		<asp:Button Text='<%# L10n.Term("SurveyQuestions.LBL_TEST_VALIDATION") %>' OnClientClick="return TestValidation();" CssClass="button" style="margin-top: 6px;" runat="server" />
	</div>

	<div id="divEditSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />


	<SplendidCRM:InlineScript runat="server">
		<script type="text/javascript">
// 08/25/2013 Paul.  Move sREMOTE_SERVER definition to the master pages. 
//var sREMOTE_SERVER  = '<%# Application["rootURL"] %>';
var sAUTHENTICATION = '';
var rowQUESTION     = null;
// 11/10/2018 Paul.  bREMOTE_ENABLED is used by SurveyQuestion_Date. 
var bREMOTE_ENABLED = false;
// 12/08/2017 Paul.  We need a way to turn off bootstrap for BPMN, ReportDesigner and ChatDashboard. 
var bDESKTOP_LAYOUT = true;
// 08/17/2018 Paul.  Date format is needed for date validation. 
var sUSER_DATE_FORMAT    = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"])) %>';

$(document).ready(function()
{
	// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
	var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
	try
	{
		var sID = '<%= gID %>';
		var divQuestionDetailView = document.getElementById('divQuestionDetailView');
		divQuestionDetailView.innerHTML = 'Retrieving data...';
		SurveyQuestion_LoadItem('SurveyQuestions', sID, function(status, message)
		{
			var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
			if ( status == 1 )
			{
				try
				{
					rowQUESTION = message;
					rowQUESTION.QUESTION_NUMBER = 1;
					
					var divQuestionDetailView = document.getElementById('divQuestionDetailView');
					SurveyQuestion_Helper_Clear(divQuestionDetailView);
					
					var divQuestionFrame = document.createElement('div');
					divQuestionFrame.className = 'SurveyQuestionDesignFrame SurveyQuestionFrame';
					divQuestionDetailView.appendChild(divQuestionFrame);
					divQuestionFrame.innerHTML = 'Rendering data...';
					SurveyQuestion_Render(rowQUESTION, divQuestionFrame, null, false);
				}
				catch(e)
				{
					lblError.innerHTML = 'DetailView Render: ' + e.message;
				}
			}
			else
			{
				lblError.innerHTML = message;
			}
		});
	}
	catch(e)
	{
		lblError.innerHTML = e.message;
	}
});

function TestValidation()
{
	// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
	var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
	try
	{
		if ( SurveyQuestion_Validate(rowQUESTION) )
		{
			if ( lblError != null )
			{
				// 11/30/2019 Paul.  Add term for success. 
				lblError.innerHTML = '<%= L10n.Term("SurveyQuestions.LBL_SUCCESS") %>' + ' ' + SurveyQuestion_Value(rowQUESTION);
			}
		}
		else
		{
			if ( lblError != null )
			{
				// 11/30/2019 Paul.  Add term for failure. 
				lblError.innerHTML = '<%= L10n.Term("SurveyQuestions.LBL_FAILURE") %>';
			}
		}
	}
	catch(e)
	{
		lblError.innerHTML = e.message;
	}
	return false;
}

		</script>
	</SplendidCRM:InlineScript>
