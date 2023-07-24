<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Administration.SurveyThemes.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" Module="SurveyThemes" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

<div class="SurveyBody" style="margin-bottom: 10px; border: 1px solid black;">
	<table cellspacing="0" cellpadding="0" border="0" width="100%" class="SurveyLogo">
		<tbody>
			<tr>
				<td width="99%">
				</td>
				<td width="1%">
					<div class="SurveyExitLink"><%# L10n.Term("SurveyThemes.LBL_EXIT_LINK") %></div>
				</td>
			</tr>
		</tbody>
	</table>
	<div class="SurveyTitle"><%# L10n.Term("SurveyThemes.LBL_SURVEY_TITLE") %></div>
	<div class="SurveyPageTitle">1. <%# L10n.Term("SurveyThemes.LBL_PAGE_TITLE") %></div>
	<div align="center">
		<div class="SurveyProgressBarFrame" align="left">
			<table class="SurveyProgressBar" width="100%">
				<tbody>
					<tr>
						<td align="center">100%</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
	<div class="SurveyPageDescription"><%# L10n.Term("SurveyThemes.LBL_PAGE_DESCRIPTION") %></div>
	<div class="SurveyQuestionContent">
		<div class="SurveyQuestionValidation"><%# L10n.Term("SurveyThemes.LBL_VALIDATION_ERROR") %></div>
		<div class="SurveyQuestionHeading">
			<span class="SurveyQuestionRequiredAsterisk">*</span><span class="SurveyQuestionNumber">1. </span><span><%# L10n.Term("SurveyThemes.LBL_QUESTION_HEADING") %></span>
		</div>
		<div class="SurveyQuestionBody">
			<table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">
				<tbody>
					<tr>
						<td valign="top" style="width: 100%;">
							<div class="SurveyAnswerChoice">
								<input id="AnswerChoice_1" name="AnswerChoice" type="radio" class="SurveyAnswerChoiceRadio">
								<label for="AnswerChoice_1"><%# L10n.Term("SurveyThemes.LBL_QUESTION_CHOICE") %> 1</label>
							</div>
							<div class="SurveyAnswerChoice">
								<input id="AnswerChoice_2" name="AnswerChoice" type="radio" class="SurveyAnswerChoiceRadio">
								<label for="AnswerChoice_2"><%# L10n.Term("SurveyThemes.LBL_QUESTION_CHOICE") %> 2</label>
							</div>
							<div class="SurveyAnswerChoice">
								<input id="AnswerChoice_3" name="AnswerChoice" type="radio" class="SurveyAnswerChoiceRadio">
								<label for="AnswerChoice_3"><%# L10n.Term("SurveyThemes.LBL_QUESTION_CHOICE") %> 3</label>
							</div>
						</td>
					</tr>
					<tr>
						<td valign="top">
							<div class="SurveyAnswerChoice SurveyAnswerOther">
								<label for="AnswerChoice_Other">Other (please specify)</label>
								<input id="AnswerChoice_Other" type="text" size="10">
							</div>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</div>

	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>
	
	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
