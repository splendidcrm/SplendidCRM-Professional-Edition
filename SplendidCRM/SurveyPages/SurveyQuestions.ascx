<%@ Control CodeBehind="SurveyQuestions.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.SurveyPages.SurveyQuestions" %>
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
// 11/09/2018 Paul.  Render actual question. 
//var sREMOTE_SERVER  = '<%# Application["rootURL"] %>';
var sAUTHENTICATION = '';
var rowQUESTION     = null;
// 11/10/2018 Paul.  bREMOTE_ENABLED is used by SurveyQuestion_Date. 
var bREMOTE_ENABLED = false;
var bDESKTOP_LAYOUT = true;
var sUSER_DATE_FORMAT    = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"])) %>';

function SurveyQuestionPopup()
{
	return ModulePopup('SurveyQuestions', '<%= txtSURVEY_QUESTION_ID.ClientID %>', null, 'ClearDisabled=1&SURVEY_PAGE_ID<%= gID %>', true, 'PopupMultiSelect.aspx');
}
</script>
<input ID="txtSURVEY_QUESTION_ID" type="hidden" Runat="server" />
<%-- 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. --%>
<%@ Register TagPrefix="SplendidCRM" Tagname="SubPanelButtons" Src="~/_controls/SubPanelButtons.ascx" %>
<SplendidCRM:SubPanelButtons ID="ctlDynamicButtons" Module="SurveyQuestions" SubPanel="divSurveyPagesSurveyQuestions" Title="SurveyQuestions.LBL_MODULE_NAME" Runat="Server" />

<div id="divSurveyPagesSurveyQuestions" style='<%= "display:" + (CookieValue("divSurveyPagesSurveyQuestions") != "1" ? "inline" : "none") %>'>
	<asp:Panel ID="pnlNewRecordInline" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>' Style="display:none" runat="server">
		<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="~/SurveyQuestions/NewRecord.ascx" %>
		<SplendidCRM:NewRecord ID="ctlNewRecord" Width="100%" EditView="EditView.Inline" ShowCancel="true" ShowHeader="false" ShowFullForm="true" ShowTopButtons="true" Runat="Server" />
	</asp:Panel>
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="SurveyQuestions" SearchMode="SearchSubpanel" IsSubpanelSearch="true" ShowSearchTabs="false" ShowDuplicateSearch="false" ShowSearchViews="false" Visible="false" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:HiddenField ID="txtINDEX" Runat="server" />
		<asp:Button ID="btnINDEX_MOVE" ValidationGroup="move" style="display: none" runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="false" AllowSorting="false" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn ItemStyle-CssClass="dragHandle">
				<ItemTemplate><asp:Image SkinID="blank" Width="14px" runat="server" /></ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-Wrap="false" Visible="false">
				<ItemTemplate>
					<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
					<asp:ImageButton CommandName="SurveQuestions.MoveUp"   Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "SURVEY_ASSIGNED_USER_ID") >= 0 %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_UP"  ) %>' SkinID="uparrow_inline" Runat="server" />
					<asp:LinkButton  CommandName="SurveQuestions.MoveUp"   Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "SURVEY_ASSIGNED_USER_ID") >= 0 %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_UP") %>' Runat="server" />
					&nbsp;
					<asp:ImageButton CommandName="SurveQuestions.MoveDown" Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "SURVEY_ASSIGNED_USER_ID") >= 0 %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' SkinID="downarrow_inline" Runat="server" />
					<asp:LinkButton  CommandName="SurveQuestions.MoveDown" Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "SURVEY_ASSIGNED_USER_ID") >= 0 %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_DOWN") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn>
				<ItemTemplate>
					<div id="grdMain_Question_<%# Eval("ID") %>" class="SurveyQuestionDesignFrame SurveyQuestionFrame" style="width: 400px"></div>
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
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%-- 10/31/2017 Paul.  Provide a way to inject Record level ACL. --%>
					<asp:ImageButton Visible='<%# !bEditView && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 %>' CommandName="SurveyQuestions.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SURVEY_QUESTION_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# !bEditView && SplendidCRM.Security.GetRecordAccess(Container, m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0 %>' CommandName="SurveyQuestions.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SURVEY_QUESTION_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<asp:ImageButton Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Surveys", "remove", "SURVEY_ASSIGNED_USER_ID") >= 0 %>' CommandName="SurveyQuestions.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SURVEY_QUESTION_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
					<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetRecordAccess(Container, "Surveys", "remove", "SURVEY_ASSIGNED_USER_ID") >= 0 %>' CommandName="SurveyQuestions.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SURVEY_QUESTION_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<SplendidCRM:InlineScript runat="server">
		<script type="text/javascript">
		// http://www.isocra.com/2008/02/table-drag-and-drop-jquery-plugin/
		$(document).ready(function()
		{
			$("#<%= grdMain.ClientID %>").tableDnD
			({
				dragHandle: "dragHandle",
				onDragClass: "jQueryDragBorder",
				onDragStart: function(table, row)
				{
					var txtINDEX = document.getElementById('<%= txtINDEX.ClientID %>');
					txtINDEX.value = (row.parentNode.rowIndex-1);
				},
				onDrop: function(table, row)
				{
					var txtINDEX = document.getElementById('<%= txtINDEX.ClientID %>');
					txtINDEX.value += ',' + (row.rowIndex-1); 
					document.getElementById('<%= btnINDEX_MOVE.ClientID %>').click();
				}
			});
			$("#<%= grdMain.ClientID %> tr").hover
			(
				function()
				{
					if ( !$(this).hasClass("nodrag") )
						$(this.cells[0]).addClass('jQueryDragHandle');
				},
				function()
				{
					if ( !$(this).hasClass("nodrag") )
						$(this.cells[0]).removeClass('jQueryDragHandle');
				}
			);
		});
		</script>
	</SplendidCRM:InlineScript>
</div>

