<%@ Control CodeBehind="SummaryView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Surveys.SummaryView" %>
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
<script type="text/javascript">
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
</script>
<div id="divSummaryView">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="Surveys" EnablePrint="true" HelpName="DetailView" EnableHelp="false" EnableFavorites="false" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="RestUtils" Src="~/_controls/RestUtils.ascx" %>
	<SplendidCRM:RestUtils Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Surveys" ShowSearchViews="false" Visible="<%# false && !PrintView %>" Runat="Server" />

	<div class="SurveyBody">
		<div id="divSurveyTitle" class="SurveyTitle"></div>
		<div id="divSurveyPages"></div>
	</div>

	<SplendidCRM:InlineScript runat="server">
		<script type="text/javascript">
// 11/10/2018 Paul.  bREMOTE_ENABLED is used by SurveyQuestion_Date. 
var bREMOTE_ENABLED = false;
TERMINOLOGY['.LNK_LIST_FIRST'                 ] = '<%= L10n.Term(".LNK_LIST_FIRST"                 ) %>';
TERMINOLOGY['.LNK_LIST_PREVIOUS'              ] = '<%= L10n.Term(".LNK_LIST_PREVIOUS"              ) %>';
TERMINOLOGY['.LNK_LIST_NEXT'                  ] = '<%= L10n.Term(".LNK_LIST_NEXT"                  ) %>';
TERMINOLOGY['.LNK_LIST_LAST'                  ] = '<%= L10n.Term(".LNK_LIST_LAST"                  ) %>';
TERMINOLOGY['.LBL_LIST_OF'                    ] = '<%= L10n.Term(".LBL_LIST_OF"                    ) %>';
TERMINOLOGY['Surveys.LBL_DETAILS'             ] = '<%= L10n.Term("Surveys.LBL_DETAILS"             ) %>';
TERMINOLOGY['SurveyResults.LBL_ANSWERED'      ] = '<%= L10n.Term("SurveyResults.LBL_ANSWERED"      ) %>';
TERMINOLOGY['SurveyResults.LBL_SKIPPED'       ] = '<%= L10n.Term("SurveyResults.LBL_SKIPPED"       ) %>';
TERMINOLOGY['SurveyResults.LBL_ANSWER_CHOICES'] = '<%= L10n.Term("SurveyResults.LBL_ANSWER_CHOICES") %>';
TERMINOLOGY['SurveyResults.LBL_RESPONSES'     ] = '<%= L10n.Term("SurveyResults.LBL_RESPONSES"     ) %>';
TERMINOLOGY['SurveyResults.LBL_AVERAGE'       ] = '<%= L10n.Term("SurveyResults.LBL_AVERAGE"       ) %>';
TERMINOLOGY['SurveyResults.LBL_TOTAL'         ] = '<%= L10n.Term("SurveyResults.LBL_TOTAL"         ) %>';
TERMINOLOGY['SurveyResults.LBL_AVERAGE_RATING'] = '<%= L10n.Term("SurveyResults.LBL_AVERAGE_RATING") %>';
TERMINOLOGY['SurveyResults.LBL_NA'            ] = '<%= L10n.Term("SurveyResults.LBL_NA"            ) %>';
// 12/23/2015 Paul.  Add Export to Summary. 
TERMINOLOGY['SurveyResults.LBL_EXPORT'        ] = '<%= L10n.Term("SurveyResults.LBL_EXPORT"        ) %>';
TERMINOLOGY_LISTS['month_names_dom'           ] = ['<%# String.Join("', '", DateTimeFormat.MonthNames           ) %>'];
TERMINOLOGY_LISTS['short_month_names_dom'     ] = ['<%# String.Join("', '", DateTimeFormat.AbbreviatedMonthNames) %>'];
TERMINOLOGY_LISTS['day_names_dom'             ] = ['<%# String.Join("', '", DateTimeFormat.DayNames             ) %>'];
TERMINOLOGY_LISTS['short_day_names_dom'       ] = ['<%# String.Join("', '", DateTimeFormat.AbbreviatedDayNames  ) %>'];

CONFIG['list_max_entries_per_page'            ] = <%= Sql.ToInteger(Application["CONFIG.list_max_entries_per_page"]) %>;

$(document).ready(function()
{
	// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
	var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
	try
	{
		var sID = '<%= gID %>';
		if ( !Sql.IsEmptyString(sID) )
		{
			lblError.innerHTML = 'Retrieving data...';
			Survey_LoadItem('Surveys', sID, function(status, message)
			{
				// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
				var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
				if ( status == 1 )
				{
					var rowSURVEY = message;
					lblError.innerHTML = 'Retrieving results...';
					try
					{
						var survey = new Survey(rowSURVEY);
						survey.Summary();
						lblError.innerHTML = '';
					}
					catch(e)
					{
						lblError.innerHTML = 'Summary Render: ' + e.message;
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
		lblError.innerHTML = e.message;
	}
});
		</script>
	</SplendidCRM:InlineScript>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

