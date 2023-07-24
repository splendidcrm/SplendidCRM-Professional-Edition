<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.SurveyResults.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="SurveyResults" EnablePrint="false" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailNavigation" Src="~/_controls/DetailNavigation.ascx" %>
	<SplendidCRM:DetailNavigation ID="ctlDetailNavigation" Module="<%# m_sMODULE %>" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:HiddenField ID="LAYOUT_DETAIL_VIEW" Runat="server" />
	<table class="tabDetailView">
		<tr>
			<td width="15%"  valign="top" class="tabDetailViewDL"></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><%= bIS_COMPLETE ? "<font color=green><b>" + L10n.Term("SurveyResults.LBL_COMPLETE") + "</b></font>": "<font color=red>" + L10n.Term("SurveyResults.LBL_INCOMPLETE") + "</font>" %></td>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%# L10n.Term("SurveyResults.LBL_RESPONDANT") %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><asp:HyperLink ID="lnkRESPONDANT" Visible="false" CssClass="tabDetailViewDFLink" runat="server" /></td>
		</tr>
		<tr>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%= L10n.Term("SurveyResults.LBL_START_DATE") %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><%= dtSTART_DATE %></td>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%= L10n.Term(".LBL_DATE_MODIFIED") %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><%= dtDATE_MODIFIED %></td>
		</tr>
		<tr>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%= L10n.Term("SurveyResults.LBL_TIME_SPENT") %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><%= TimeSpent(dtSUBMIT_DATE, dtSTART_DATE) %></td>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%= L10n.Term("SurveyResults.LBL_IP_ADDRESS") %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><%= sIP_ADDRESS %></td>
		</tr>
		<tr>
			<td width="15%"  valign="top" class="tabDetailViewDL"><%= L10n.Term("SurveyResults.LBL_SURVEY_NAME") %></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"><asp:HyperLink ID="lnkSURVEY" CssClass="tabDetailViewDFLink" runat="server" /></td>
			<td width="15%"  valign="top" class="tabDetailViewDL"></td>
			<td width="35%"  valign="top" class="tabDetailViewDF"></td>
		</tr>
	</table>

	<div class="SurveyBody">
		<div id="divSurveyTitle" class="SurveyTitle"></div>
		<div id="divSurveyPages"></div>
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />

<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
// 08/25/2013 Paul.  Move sREMOTE_SERVER definition to the master pages. 
//var sREMOTE_SERVER  = '<%= Application["rootURL"] %>';
var sAUTHENTICATION = '';
// 11/10/2018 Paul.  bREMOTE_ENABLED is used by SurveyQuestion_Date. 
var bREMOTE_ENABLED = false;
// 10/09/2018 Paul.  We need a way to turn off bootstrap for BPMN, ReportDesigner and ChatDashboard. 
var bDESKTOP_LAYOUT = true;
// 10/09/2018 Paul.  Date format is needed for date validation. 
var sUSER_DATE_FORMAT    = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"])) %>';

$(document).ready(function()
{
	// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
	var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
	try
	{
		var sSURVEY_RESULT_ID = '<%= sSURVEY_RESULT_ID %>';
		if ( !Sql.IsEmptyString(sSURVEY_RESULT_ID) )
		{
			lblError.innerHTML = 'Retrieving data...';
			var sTABLE_NAME     = 'SURVEY_QUESTIONS_RESULTS';
			var sSORT_FIELD     = 'DATE_ENTERED';
			var sSORT_DIRECTION = 'asc';
			var sSELECT         = '';
			var sFILTER         = "SURVEY_RESULT_ID eq '" + sSURVEY_RESULT_ID + "'";
			SurveyResults_LoadTable(sTABLE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSELECT, sFILTER, function(status, message)
			{
				var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
				if ( status == 1 )
				{
					var rowRESULTS = message;
					lblError.innerHTML = 'Retrieving results...';
					
					var sSURVEY_ID = rowRESULTS[0].SURVEY_ID;
					Survey_LoadItem('Surveys', sSURVEY_ID, function(status, message)
					{
						var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
						if ( status == 1 )
						{
							try
							{
								var rowSURVEY = message;
								lblError.innerHTML = 'Rendering data...';
								
								var survey = new Survey(rowSURVEY);
								survey.Report(rowRESULTS);
								lblError.innerHTML = '';
							}
							catch(e)
							{
								lblError.innerHTML = 'Results Render: ' + e.message;
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
					lblError.innerHTML = 'Survey_LoadItem Response Error: ' + message;
				}
			});
		}
		else
		{
			//lblError.innerHTML = 'Survey ID not specified.';
		}
	}
	catch(e)
	{
		lblError.innerHTML = e.message;
	}
});
	</script>
</SplendidCRM:InlineScript>

