<%@ Control CodeBehind="PopupView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.SurveyQuestions.PopupView" %>
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
<div id="divPopupView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="SurveyQuestions" IsPopupSearch="true" ShowSearchTabs="false" Visible="<%# !PrintView %>" Runat="Server" />

	<script type="text/javascript">
// 11/09/2018 Paul.  Render actual question. 
//var sREMOTE_SERVER  = '<%# Application["rootURL"] %>';
var sAUTHENTICATION = '';
var rowQUESTION     = null;
// 11/10/2018 Paul.  bREMOTE_ENABLED is used by SurveyQuestion_Date. 
var bREMOTE_ENABLED = false;
var bDESKTOP_LAYOUT = true;
var sUSER_DATE_FORMAT    = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"])) %>';

	function SelectSurveyQuestion(sPARENT_ID, sPARENT_NAME)
	{
		if ( window.opener != null && window.opener.ChangeSurveyQuestion != null )
		{
			window.opener.ChangeSurveyQuestion(sPARENT_ID, sPARENT_NAME);
			window.close();
		}
		else
		{
			alert('Original window has closed.  SurveyQuestion cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
		}
	}
	function SelectChecked()
	{
		if ( window.opener != null && window.opener.ChangeSurveyQuestion != null )
		{
			var sSelectedItems = document.getElementById('<%= ctlCheckAll.SelectedItems.ClientID %>').value;
			window.opener.ChangeSurveyQuestion(sSelectedItems, '');
			window.close();
		}
		else
		{
			alert('Original window has closed.  SurveyQuestion cannot be assigned.');
		}
	}
	function Clear()
	{
		if ( window.opener != null && window.opener.ChangeSurveyQuestion != null )
		{
			window.opener.ChangeSurveyQuestion('', '');
			window.close();
		}
		else
		{
			alert('Original window has closed.  SurveyQuestion cannot be assigned.');
		}
	}
	function Cancel()
	{
		window.close();
	}
	</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="SurveyQuestions.LBL_LIST_FORM_TITLE" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />

	<asp:UpdatePanel UpdateMode="Conditional" Visible='<%# false && !Sql.ToBoolean(Application["CONFIG.disable_popupview_inline"]) %>' runat="server">
		<ContentTemplate>
			<asp:Button ID="btnCreateInline" CommandName="NewRecord.Show" OnCommand="Page_Command" Text='<%# L10n.Term(m_sMODULE + ".LNK_NEW_SURVEY_QUESTION") %>' CssClass="button" style="margin-bottom: 4px;" Visible="<%# !this.IsMobile %>" runat="server" />
			<asp:Panel ID="pnlNewRecordInline" Style="display:none" runat="server">
				<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="NewRecord.ascx" %>
				<SplendidCRM:NewRecord ID="ctlNewRecord" Width="100%" EditView="PopupView.Inline" ShowCancel="true" Runat="Server" />
			</asp:Panel>
		</ContentTemplate>
	</asp:UpdatePanel>

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-VerticalAlign="Top">
				<ItemTemplate><%# grdMain.InputCheckbox(!PrintView && bMultiSelect, ctlCheckAll.FieldName, Sql.ToGuid(Eval("ID")), ctlCheckAll.SelectedItems) %></ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn>
				<ItemTemplate>
					<div id="grdMain_Question_<%# Eval("ID") %>" class="SurveyQuestionDesignFrame SurveyQuestionFrame"></div>
					<SplendidCRM:InlineScript runat="server">
						<script type="text/javascript">
						// 11/09/2018 Paul.  Render actual question. 
						try
						{
							var sSURVEY_QUESTION_ID = '<%# Eval("ID") %>';
							SurveyQuestion_LoadItem(null, sSURVEY_QUESTION_ID, function(status, row)
							{
								var divQuestionFrame = document.getElementById('grdMain_Question_<%# Eval("ID") %>');
								// 11/09/2018 Paul.  Question Types can have a space. 
								divQuestionFrame.className = 'SurveyQuestionDesignFrame SurveyQuestionFrame';
								divQuestionFrame.innerHTML = '<%# L10n.Term("SurveyQuestions.LBL_RENDERING_DATA") %>';
								SurveyQuestion_Render(row, divQuestionFrame, null, true);
							});
						}
						catch(e)
						{
							var divQuestionFrame = document.getElementById('grdMain_Question_<%# Eval("ID") %>');
							divQuestionFrame.innerHTML = '<div>' + escape(e.message) + '</div>';
						}
						</script>
					</SplendidCRM:InlineScript>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && bMultiSelect %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>

