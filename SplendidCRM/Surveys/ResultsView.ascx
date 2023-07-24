<%@ Control CodeBehind="ResultsView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Surveys.ResultsView" %>
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
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
// 07/27/2018 Paul.  We need a way to turn off bootstrap for BPMN, ReportDesigner and ChatDashboard. 
var bDESKTOP_LAYOUT      = true;
// 11/10/2018 Paul.  bREMOTE_ENABLED is used by SurveyQuestion_Date. 
var bREMOTE_ENABLED = false;

function Preview()
{
	window.open('preview.aspx?ID=<%= gID %>', 'SurveyPreview_<%= gID.ToString().Replace("-", "_") %>');
	return false;
}
function Test()
{
	window.open('run.aspx?ID=<%= gID %>', 'SurveyRun_<%= gID.ToString().Replace("-", "_") %>');
	return false;
}
function ChangeRespondant(sPARENT_ID, sPARENT_NAME)
{
	var sCHANGE_MODULE_NAME = 'SurveyRespondants';
	var sCHANGE_MODULE_ID   = '<%= new SplendidCRM.DynamicControl(ctlSearchView, "PARENT_ID"  ).ClientID %>';
	var sCHANGE_MODULE_NAME = '<%= new SplendidCRM.DynamicControl(ctlSearchView, "PARENT_NAME").ClientID %>';
	var fldAjaxErrors = document.getElementById(sCHANGE_MODULE_NAME + '_AjaxErrors');
	if ( fldAjaxErrors != null )
		fldAjaxErrors.innerHTML = '';

	if ( sCHANGE_MODULE_NAME != null )
	{
		var fldCHANGE_MODULE_NAME = document.getElementById(sCHANGE_MODULE_NAME);
		if ( fldCHANGE_MODULE_NAME != null )
		{
			fldCHANGE_MODULE_NAME.value = sPARENT_NAME;
		}
	}
	var fldCHANGE_MODULE_ID   = document.getElementById(sCHANGE_MODULE_ID  );
	if ( fldCHANGE_MODULE_ID != null )
	{
		fldCHANGE_MODULE_ID.value   = sPARENT_ID  ;
	}
	else
	{
		alert('Could not find ' + sCHANGE_MODULE_ID + ' in the form.');
	}
}
function RespondantPopup()
{
	var sSURVEY_ID   = '<%= gID %>';
	var sPopupURL    = '../SurveyResults/RespondantsPopup.aspx?SURVEY_ID=' + escape(sSURVEY_ID);
	return window.open(sPopupURL, 'SurveyRespondantPopup', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
}
</script>
</SplendidCRM:InlineScript>
<div id="divResultsView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Surveys" EnablePrint="true" HelpName="DetailView" EnableHelp="false" EnableFavorites="false" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Surveys" SearchMode="SearchResultsView" ShowSearchTabs="false" ShowSearchViews="false" Visible="<%# !PrintView %>" Runat="Server" />

	<br />
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="true" AllowSorting="false" EnableViewState="true" ShowHeader="false" PageSize="1" runat="server">
		<Columns>
			<asp:TemplateColumn ItemStyle-VerticalAlign="Top">
				<ItemTemplate>
					<table class="tabDetailView">
						<tr>
							<td width="15%"  valign="top" class="tabDetailViewDL"></td>
							<td width="35%"  valign="top" class="tabDetailViewDF"><%# Sql.ToBoolean(Eval("IS_COMPLETE")) ? "<font color=green><b>" + L10n.Term("SurveyResults.LBL_COMPLETE") + "</b></font>": "<font color=red>" + L10n.Term("SurveyResults.LBL_INCOMPLETE") + "</font>" %></td>
							<td width="15%"  valign="top" class="tabDetailViewDL"><%# L10n.Term("SurveyResults.LBL_RESPONDANT") %></td>
							<td width="35%"  valign="top" class="tabDetailViewDF"><asp:HyperLink NavigateUrl='<%# "~/" + Eval("PARENT_TYPE") + "/view.aspx?ID=" + Eval("PARENT_ID") %>' Text='<%# Eval("PARENT_NAME") %>' Visible='<%# !Sql.IsEmptyGuid(Eval("PARENT_ID")) %>' CssClass="tabDetailViewDFLink" runat="server" /></td>
						</tr>
						<tr>
							<td width="15%"  valign="top" class="tabDetailViewDL"><%# L10n.Term("SurveyResults.LBL_START_DATE") %></td>
							<td width="35%"  valign="top" class="tabDetailViewDF"><%# Eval("START_DATE") %></td>
							<td width="15%"  valign="top" class="tabDetailViewDL"><%# L10n.Term(".LBL_DATE_MODIFIED") %></td>
							<td width="35%"  valign="top" class="tabDetailViewDF"><%# Eval("DATE_MODIFIED") %></td>
						</tr>
						<tr>
							<td width="15%"  valign="top" class="tabDetailViewDL"><%# L10n.Term("SurveyResults.LBL_TIME_SPENT") %></td>
							<td width="35%"  valign="top" class="tabDetailViewDF"><%# TimeSpent(Eval("SUBMIT_DATE"), Eval("START_DATE")) %></td>
							<td width="15%"  valign="top" class="tabDetailViewDL"><%# L10n.Term("SurveyResults.LBL_IP_ADDRESS") %></td>
							<td width="35%"  valign="top" class="tabDetailViewDF"><%# Eval("IP_ADDRESS"  ) %></td>
						</tr>
					</table>
					<br />

<div class="SurveyBody">
	<div id="divSurveyTitle" class="SurveyTitle"></div>
	<div id="divSurveyPages"></div>
</div>

<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
// 08/25/2013 Paul.  Move sREMOTE_SERVER definition to the master pages. 
//var sREMOTE_SERVER  = '<%# Application["rootURL"] %>';
var sAUTHENTICATION = '';

$(document).ready(function()
{
	// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
	var lblError = document.getElementById('<%# ctlDynamicButtons.ErrorClientID %>');
	try
	{
		var sID               = '<%# Eval("SURVEY_ID"       ) %>';
		var sSURVEY_RESULT_ID = '<%# Eval("SURVEY_RESULT_ID") %>';
		if ( !Sql.IsEmptyString(sID) && !Sql.IsEmptyString(sSURVEY_RESULT_ID) )
		{
			lblError.innerHTML = 'Retrieving data...';
			Survey_LoadItem('Surveys', sID, function(status, message)
			{
				// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
				var lblError = document.getElementById('<%# ctlDynamicButtons.ErrorClientID %>');
				if ( status == 1 )
				{
					var rowSURVEY = message;
					lblError.innerHTML = 'Retrieving results...';
					
					var sTABLE_NAME     = 'SURVEY_QUESTIONS_RESULTS';
					var sSORT_FIELD     = 'DATE_ENTERED';
					var sSORT_DIRECTION = 'asc';
					var sSELECT         = '';
					var sFILTER         = "SURVEY_RESULT_ID eq '" + sSURVEY_RESULT_ID + "'";
					SurveyResults_LoadTable(sTABLE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSELECT, sFILTER, function(status, message)
					{
						// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
						var lblError = document.getElementById('<%# ctlDynamicButtons.ErrorClientID %>');
						if ( status == 1 )
						{
							try
							{
								var rowRESULTS = message;
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
			lblError.innerHTML = 'Survey ID not specified.';
		}
	}
	catch(e)
	{
		lblError.innerHTML = e.message;
	}
});
	</script>
</SplendidCRM:InlineScript>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE) %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

