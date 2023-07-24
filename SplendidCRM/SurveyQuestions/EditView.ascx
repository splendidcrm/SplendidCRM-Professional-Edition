<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.SurveyQuestions.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView" runat="server">
	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderButtons" Src="~/_controls/HeaderButtons.ascx" %>
	<SplendidCRM:HeaderButtons ID="ctlDynamicButtons" ShowRequired="true" EditView="true" Module="SurveyQuestions" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>

	<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_QUESTION") %>' runat="server" /></h3>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="50%" VerticalAlign="Top">
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" ColumnSpan="2">
							<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_DESCRIPTION") %>' runat="server" />&nbsp;<asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2">
							<asp:TextBox ID="DESCRIPTION" TextMode="MultiLine" style="width: 100%" Rows="4" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel">
							<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_QUESTION_TYPE") %>' runat="server" />&nbsp;<asp:Label ID="Label2" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel" style="padding-left: 20px">
							<asp:Label ID="labDisplayFormat" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:DropDownList ID="QUESTION_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="QUESTION_TYPE_SelectedIndexChanged" AutoPostBack="true" Runat="server" />
						</asp:TableCell>
						<asp:TableCell style="padding-left: 20px">
							<asp:DropDownList ID="DISPLAY_FORMAT" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2">
							<h3 style="margin-top: 20px"><%# L10n.Term("SurveyQuestions.LBL_SAMPLE") %></h3>
							<div id="divQuestionEditViewSample" style="margin-top: 10px"></div>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
			<asp:TableCell Width="50%" VerticalAlign="Top" style="padding-left: 10px;">
				<asp:HiddenField ID="LAYOUT_EDIT_VIEW" Runat="server" />
				<table ID="tblMain" class="tabEditView" runat="server" />
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" Width="30%">
							<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_CATEGORIES") %>' runat="server" />
						</asp:TableCell>
						<asp:TableCell>
							<asp:TextBox ID="CATEGORIES" TextMode="MultiLine" style="width: 100%" Rows="3" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" Width="35%">
							<asp:Label ID="SURVEY_TARGET_LABEL" Text='<%# L10n.Term("SurveyQuestions.LBL_SURVEY_TARGET_MODULE") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell>
							<asp:DropDownList ID="SURVEY_TARGET_MODULE" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="SURVEY_TARGET_MODULE_SelectedIndexChanged" AutoPostBack="true" CssClass="dataField" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel">
							<asp:Label ID="TARGET_FIELD_LABEL" Text='<%# L10n.Term("SurveyQuestions.LBL_TARGET_FIELD_NAME") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell>
							<asp:DropDownList ID="TARGET_FIELD_NAME" DataValueField="NAME" DataTextField="DISPLAY_NAME" CssClass="dataField" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<asp:Panel ID="pnlAnswer" runat="server">
		<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_ANSWER_OPTIONS") %>' runat="server" /></h3>
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table ID="tblAnswerChoices" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel">
								<asp:Label ID="ANSWER_CHOICES_LABEL" Text='<%# L10n.Term("SurveyQuestions.LBL_ANSWER_CHOICES") %>' runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataField" style="padding-right: 10px;">
								<asp:TextBox ID="ANSWER_CHOICES" TextMode="MultiLine" style="width: 100%" Rows="4" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					
					<asp:Table ID="tblInvalidDate" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_INVALID_DATE_MESSAGE") %>' runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataField" style="padding-right: 10px;">
								<asp:TextBox ID="INVALID_DATE_MESSAGE" style="width: 100%" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					
					<asp:Table ID="tblInvalidNumber" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_INVALID_NUMBER_MESSAGE") %>' runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataField" style="padding-right: 10px;">
								<asp:TextBox ID="INVALID_NUMBER_MESSAGE" style="width: 100%" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					
					<asp:Table ID="tblRankingNA" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:CheckBox ID="NA_ENABLED" Text='<%# L10n.Term("SurveyQuestions.LBL_NA_ENABLED") %>' CssClass="checkbox" OnCheckedChanged="NA_ENABLED_CheckedChanged" AutoPostBack="true" runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField">
								<asp:TextBox ID="NA_LABEL" Size="40" style="margin-left: 10px;" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					
					<asp:Panel ID="pnlRatingScale" style="width: 100%;" runat="server">
						<asp:Table SkinID="tabEditView" CellPadding="3" runat="server">
							<asp:TableRow>
								<asp:TableCell CssClass="dataField" Width="15%" VerticalAlign="Top">
									<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RATING_SCALE") %>' runat="server" /><br />
									<asp:DropDownList ID="lstRatingScale" DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-left: 4px;" OnSelectedIndexChanged="lstRatingScale_SelectedIndexChanged" AutoPostBack="true" runat="server" />
								</asp:TableCell>
								<asp:TableCell CssClass="dataField" Width="50%" VerticalAlign="Top">
									<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RATING_SCALE_CHOICES") %>' runat="server" />
									<asp:Table ID="tblRATING" SkinID="tabEditView" style="margin-left: 4px;" runat="server" />
								</asp:TableCell>
								<asp:TableCell CssClass="dataField" Width="35%" VerticalAlign="Top">
								</asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:Panel>
					
					<asp:Table ID="tblColumnChoices" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_COLUMN_CHOICES") %>' runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataField" style="padding-right: 10px;">
								<asp:TextBox ID="COLUMN_CHOICES" TextMode="MultiLine" style="width: 100%" Rows="4" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					
					<asp:Table ID="tblForcedRanking" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel">
								<asp:CheckBox ID="FORCED_RANKING" Text='<%# L10n.Term("SurveyQuestions.LBL_FORCED_RANKING") %>' CssClass="checkbox" runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					
					<asp:Panel ID="pnlMenuChoices" runat="server">
						<asp:Table SkinID="tabEditView" runat="server">
							<asp:TableRow>
								<asp:TableCell CssClass="dataLabel" Width="15%" VerticalAlign="Top">
									<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_MENU_CHOICES") %>' runat="server" />
								</asp:TableCell>
								<asp:TableCell CssClass="dataField" Width="85%" VerticalAlign="Top">
									<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_NUMBER_OF_MENUS") %>' runat="server" />
									<asp:DropDownList ID="lstNumberOfMenus" DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-left: 4px" OnSelectedIndexChanged="lstNumberOfMenus_SelectedIndexChanged" AutoPostBack="true" runat="server" />
								</asp:TableCell>
							</asp:TableRow>
						</asp:Table>

						<asp:Table ID="tblMENU" SkinID="tabEditView" runat="server">
						</asp:Table>
					</asp:Panel>
					
					<asp:Panel ID="pnlOther" CssClass="dataLabel" runat="server">
						<asp:Table SkinID="tabEditView" runat="server">
							<asp:TableRow>
								<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top">
									<asp:CheckBox ID="OTHER_ENABLED" Text='<%# L10n.Term("SurveyQuestions.LBL_OTHER_ENABLED") %>' CssClass="checkbox" OnCheckedChanged="OTHER_ENABLED_CheckedChanged" AutoPostBack="true" runat="server" />
								</asp:TableCell>
								<asp:TableCell Width="85%" CssClass="dataField" VerticalAlign="Top">
									<div id="divAnswerOther" runat="server">
										<asp:Table SkinID="tabEditView" runat="server">
											<asp:TableRow>
												<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_OTHER_LABEL") %>' runat="server" /></asp:TableCell>
												<asp:TableCell Width="85%" CssClass="dataField"><asp:TextBox ID="OTHER_LABEL" Size="40" MaxLength="200" Runat="server" /></asp:TableCell>
											</asp:TableRow>
											<asp:TableRow>
												<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_OTHER_SIZE") %>' runat="server" /></asp:TableCell>
												<asp:TableCell Width="85%" CssClass="dataField">
													<asp:DropDownList ID="OTHER_HEIGHT" DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-right: 10px" Runat="server" />
													<asp:DropDownList ID="OTHER_WIDTH"  DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-right: 10px" Runat="server" />
												</asp:TableCell>
											</asp:TableRow>
											<asp:TableRow>
												<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_OTHER_VALIDATION_TYPE") %>' runat="server" /></asp:TableCell>
												<asp:TableCell Width="85%" CssClass="dataField">
													<asp:DropDownList ID="OTHER_VALIDATION_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="OTHER_VALIDATION_TYPE_SelectedIndexChanged" AutoPostBack="true" style="margin-right: 10px" Runat="server" />
													<span ID="spnOtherValidation" runat="server">
														<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_VALIDATION_BETWEEN") %>' runat="server" />
														<asp:TextBox ID="OTHER_VALIDATION_MIN" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
														<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_VALIDATION_AND"    ) %>' runat="server" />
														<asp:TextBox ID="OTHER_VALIDATION_MAX" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
													</span>
												</asp:TableCell>
											</asp:TableRow>
											<asp:TableRow ID="trOtherValidationMessage">
												<asp:TableCell CssClass="dataLabel" />
												<asp:TableCell CssClass="dataField" style="padding-right: 10px;">
													<asp:TextBox ID="OTHER_VALIDATION_MESSAGE" TextMode="MultiLine" style="width: 100%" Rows="2" Runat="server" />
												</asp:TableCell>
											</asp:TableRow>
											<asp:TableRow ID="trOtherAsChoice">
												<asp:TableCell CssClass="dataLabel" />
												<asp:TableCell CssClass="dataField">
													<asp:CheckBox ID="OTHER_AS_CHOICE"   Text='<%# L10n.Term("SurveyQuestions.LBL_OTHER_AS_CHOICE"  ) %>' CssClass="checkbox" OnCheckedChanged="OTHER_AS_CHOICE_CheckedChanged" AutoPostBack="true" Runat="server" />
													<asp:CheckBox ID="OTHER_ONE_PER_ROW" Text='<%# L10n.Term("SurveyQuestions.LBL_OTHER_ONE_PER_ROW") %>' CssClass="checkbox" Runat="server" />
												</asp:TableCell>
											</asp:TableRow>
											<asp:TableRow ID="trOtherRequiredMessage">
												<asp:TableCell CssClass="dataLabel" />
												<asp:TableCell CssClass="dataField" style="padding-right: 10px;">
													<asp:TextBox ID="OTHER_REQUIRED_MESSAGE" TextMode="MultiLine" style="width: 100%" Rows="2" Runat="server" />
												</asp:TableCell>
											</asp:TableRow>
										</asp:Table>
									</div>
								</asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:Panel>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</asp:Panel>

	<asp:Panel ID="pnlRange" runat="server">
		<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RANGE_OPTIONS") %>' runat="server" /></h3>
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RANGE_VALUES") %>' runat="server" />&nbsp;<asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField" style="padding-right: 10px;">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RANGE_BETWEEN") %>' runat="server" />
								<asp:TextBox ID="RANGE_MIN" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RANGE_AND"    ) %>' runat="server" />
								<asp:TextBox ID="RANGE_MAX" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RANGE_STEP") %>' runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField" style="padding-right: 10px;">
								<asp:TextBox ID="RANGE_STEP" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</asp:Panel>

	<asp:Panel ID="pnlDemographic" runat="server">
		<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_DEMOGRAPHIC_INFORMATION") %>' runat="server" /></h3>
		<asp:Table ID="tblDEMOGRAPHIC" SkinID="tabForm" Width="100%" runat="server" />
	</asp:Panel>

	<asp:Panel ID="pnlRequired" runat="server">
		<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_REQUIRED_OPTIONS") %>' runat="server" /></h3>
		<div id="divRequired" runat="server">
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:Table SkinID="tabEditView" runat="server">
							<asp:TableRow>
								<asp:TableCell CssClass="dataLabel" Width="15%" VerticalAlign="Top">
									<asp:CheckBox ID="REQUIRED" Text='<%# L10n.Term("SurveyQuestions.LBL_REQUIRED") %>' CssClass="checkbox" OnCheckedChanged="REQUIRED_CheckedChanged" AutoPostBack="true" runat="server" />
								</asp:TableCell>
								<asp:TableCell CssClass="dataField" Width="85%" VerticalAlign="Top" style="padding-right: 10px;">
									<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_REQUIRED_MESSAGE") %>' runat="server" /><br />
									<asp:TextBox ID="REQUIRED_MESSAGE"  TextMode="MultiLine" style="width: 100%" Rows="2" Runat="server" />
									<asp:Table ID="tblRequiredType" SkinID="tabEditView" runat="server">
										<asp:TableRow>
											<asp:TableCell Width="20%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_REQUIRED_TYPE") %>' runat="server" /></asp:TableCell>
											<asp:TableCell Width="80%" CssClass="dataField">
												<asp:DropDownList ID="REQUIRED_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="REQUIRED_TYPE_SelectedIndexChanged" AutoPostBack="true" Runat="server" />
												<asp:TextBox ID="REQUIRED_RESPONSES_MIN" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
												<asp:Label ID="REQUIRED_RESPONSES_RANGE" Text='<%# L10n.Term("SurveyQuestions.LBL_REQUIRED_RESPONSES_RANGE") %>' runat="server" />
												<asp:TextBox ID="REQUIRED_RESPONSES_MAX" style="width: 100px; margin-left: 10px; margin-right: 10px" Runat="server" />
											</asp:TableCell>
										</asp:TableRow>
									</asp:Table>
								</asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</div>
	</asp:Panel>

	<asp:Panel ID="pnlRandomize" runat="server">
		<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RANDOMIZE") %>' runat="server" /></h3>
		<div id="divRandomize" runat="server">
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:Table runat="server">
							<asp:TableRow>
								<asp:TableCell CssClass="dataLabel">
									<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_RANDOMIZE_TYPE") %>' runat="server" />
								</asp:TableCell>
								<asp:TableCell CssClass="dataField">
									<asp:RadioButtonList ID="RANDOMIZE_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" RepeatDirection="Horizontal" CssClass="radio" Runat="server" />
								</asp:TableCell>
								<asp:TableCell CssClass="dataField" style="padding-left: 20px;">
									<asp:CheckBox ID="RANDOMIZE_NOT_LAST" Text='<%# L10n.Term("SurveyQuestions.LBL_RANDOMIZE_NOT_LAST") %>' CssClass="checkbox" runat="server" />
								</asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</div>
	</asp:Panel>

	<asp:Panel ID="pnlValidationEnabled" runat="server">
		<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_VALIDATION_ENABLED") %>' runat="server" /></h3>
		<div id="divValidationEnabled" runat="server">
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell Width="15%" CssClass="dataLabel"><%# L10n.Term("SurveyQuestions.LBL_VALIDATION_TYPE") %></asp:TableCell>
					<asp:TableCell Width="85%" CssClass="dataField">
						<asp:DropDownList ID="VALIDATION_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="VALIDATION_TYPE_SelectedIndexChanged" AutoPostBack="true" style="margin-right: 10px" Runat="server" />
						<span ID="spnValidation" runat="server">
							<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_VALIDATION_BETWEEN") %>' runat="server" />
							<asp:TextBox ID="VALIDATION_MIN" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
							<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_VALIDATION_AND"    ) %>' runat="server" />
							<asp:TextBox ID="VALIDATION_MAX" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
						</span>
					</asp:TableCell>
				</asp:TableRow>
				<asp:TableRow>
					<asp:TableCell CssClass="dataField" ColumnSpan="2" style="padding-right: 10px;">
						<asp:TextBox ID="VALIDATION_MESSAGE" TextMode="MultiLine" style="width: 100%" Rows="2" Runat="server" />
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</div>
	</asp:Panel>

	<asp:Panel ID="pnlValidationSum" runat="server">
		<h3></h3>
		<div id="divValidationSum" runat="server">
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell Width="15%" CssClass="dataField" VerticalAlign="Top">
						<asp:CheckBox ID="VALIDATION_SUM_ENABLED" Text='<%# L10n.Term("SurveyQuestions.LBL_VALIDATION_SUM_ENABLED") %>' CssClass="checkbox" OnCheckedChanged="VALIDATION_SUM_ENABLED_CheckedChanged" AutoPostBack="true" runat="server" />
					</asp:TableCell>
					<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_VALIDATION_SUM") %>' runat="server" /></asp:TableCell>
					<asp:TableCell Width="70%" CssClass="dataField" VerticalAlign="Top">
						<asp:TextBox ID="VALIDATION_NUMERIC_SUM" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
					</asp:TableCell>
				</asp:TableRow>
				<asp:TableRow>
					<asp:TableCell CssClass="dataField" ColumnSpan="4" style="padding-right: 10px;">
						<asp:TextBox ID="VALIDATION_SUM_MESSAGE" TextMode="MultiLine" style="width: 100%" Rows="2" Runat="server" />
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</div>
	</asp:Panel>

	<asp:Panel ID="pnlName" runat="server">
		<h3></h3>
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_NAME") %>' runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataField" ColumnSpan="2">
								<asp:TextBox ID="NAME" Size="80" MaxLength="150" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</asp:Panel>

	<asp:Panel ID="pnlImage" runat="server">
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel" Width="15%" style="white-space: nowrap">
								<asp:RadioButton ID="radIMAGE_UPLOAD" GroupName="IMAGE" Text='<%# L10n.Term("SurveyQuestions.LBL_IMAGE_UPLOAD") %>' Checked="true" CssClass="radio" runat="server" />
							</asp:TableCell>
							<asp:TableCell CssClass="dataField" Width="85%">
								<asp:FileUpload ID="UPLOAD_IMAGE" size="30" runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel" style="white-space: nowrap">
								<asp:RadioButton ID="radIMAGE_URL" GroupName="IMAGE" Text='<%# L10n.Term("SurveyQuestions.LBL_IMAGE_URL") %>' CssClass="radio" runat="server" />
							</asp:TableCell>
							<asp:TableCell CssClass="dataField">
								<asp:TextBox ID="IMAGE_URL" style="width: 100%;" runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_EXISTING_IMAGE") %>' runat="server" />
							</asp:TableCell>
							<asp:TableCell CssClass="dataField">
								<asp:Image ID="imgIMAGE" runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</asp:Panel>

	<h3><asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_QUESTION_SIZE") %>' runat="server" /></h3>
	<div id="divQuestionSize" runat="server">
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table ID="tblSize" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_SIZE") %>' runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField">
								<asp:DropDownList ID="SIZE_UNITS"  DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-right: 10px;" OnSelectedIndexChanged="SIZE_UNITS_SelectedIndexChanged" AutoPostBack="true" Runat="server" />
								<asp:DropDownList ID="SIZE_HEIGHT" DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-right: 10px;" Runat="server" />
								<asp:DropDownList ID="SIZE_WIDTH"  DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-right: 10px;" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					<asp:Table ID="tblBoxSize" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_BOX_SIZE") %>' runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField">
								<asp:DropDownList ID="BOX_HEIGHT" DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-right: 10px;" Runat="server" />
								<asp:DropDownList ID="BOX_WIDTH"  DataValueField="NAME" DataTextField="DISPLAY_NAME" style="margin-right: 10px;" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					<asp:Table ID="tblColumnWidth" SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_COLUMN_WIDTH") %>' runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField">
								<asp:DropDownList ID="COLUMN_WIDTH" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					<asp:Table SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_PLACEMENT") %>' runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField">
								<asp:DropDownList ID="PLACEMENT" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="PLACEMENT_SelectedIndexChanged" AutoPostBack="true" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					<asp:Table SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_SPACING") %>' runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="85%" CssClass="dataField">
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_SPACING_LEFT"  ) %>' runat="server" /> <asp:TextBox ID="SPACING_LEFT"   style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_SPACING_TOP"   ) %>' runat="server" /> <asp:TextBox ID="SPACING_TOP"    style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_SPACING_RIGHT" ) %>' runat="server" /> <asp:TextBox ID="SPACING_RIGHT"  style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
								<asp:Label Text='<%# L10n.Term("SurveyQuestions.LBL_SPACING_BOTTOM") %>' runat="server" /> <asp:TextBox ID="SPACING_BOTTOM" style="width: 100px; margin-left: 4px; margin-right: 10px" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>

	<%-- 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. --%>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !SplendidDynamic.StackedLayout(this.Page.Theme) && !PrintView %>" ShowRequired="false" Runat="Server" />

	<div id="divEditSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<style type="text/css">
.selectize-dropdown .active
{
	background-color: #dddddd;
}
.SurveyQuestionHeading
{
	background-color: inherit;
}
.SurveyQuestionBody
{
	background-color: inherit;
}
.SurveyAnswerChoice
{
	background-color: inherit;
}
</style>

<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
// 12/08/2017 Paul.  We need a way to turn off bootstrap for BPMN, ReportDesigner and ChatDashboard. 
var bDESKTOP_LAYOUT = true;
// 11/10/2018 Paul.  bREMOTE_ENABLED is used by SurveyQuestion_Date. 
var bREMOTE_ENABLED = false;
// 08/17/2018 Paul.  Date format is needed for date validation. 
var sUSER_DATE_FORMAT    = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"])) %>';

$(document).ready(function()
{
	var lblError = document.getElementById('<%= ctlDynamicButtons.ErrorClientID %>');
	var sQUESTION_TYPE = '<%= QUESTION_TYPE.ClientID %>';
	try
	{
		// http://selectize.github.io/selectize.js/
		$('#' + sQUESTION_TYPE).selectize(
		{ render:
			{
				option: function(item, escape)
				{
					var divQuestionFrame = document.createElement('div');
					// 11/09/2018 Paul.  Question Types can have a space. 
					divQuestionFrame.id = 'divQuestionEditView' + item.value.replace(' ', '');
					//divQuestionFrame.innerText = escape(item.value) + ' - ' + escape(item.text);
					divQuestionFrame.className = 'SurveyQuestionDesignFrame SurveyQuestionFrame';
					divQuestionFrame.innerHTML = 'Rendering data...';
					var rowQUESTION = new Object();
					rowQUESTION.ID               = item.value.replace(' ', '');
					rowQUESTION.QUESTION_TYPE    = item.value;
					rowQUESTION.DESCRIPTION      = item.text;
					rowQUESTION.QUESTION_NUMBER  = 1;
					try
					{
						SurveyQuestion_RenderSample(rowQUESTION, divQuestionFrame);
						return divQuestionFrame;
					}
					catch(e)
					{
						return '<div>' + escape(e.message) + '</div>';
					}
				}
			}
		});
	}
	catch(e)
	{
		lblError.innerHTML = e.message;
	}
	var lstQUESTION_TYPE = document.getElementById(sQUESTION_TYPE);
	var divQuestionEditViewSample = document.getElementById('divQuestionEditViewSample');
	divQuestionEditViewSample.className = 'SurveyQuestionDesignFrame SurveyQuestionFrame';
	divQuestionEditViewSample.innerHTML = 'Rendering data...';
	var rowQUESTION = new Object();
	rowQUESTION.ID               = 'divQuestionEditViewSample' + lstQUESTION_TYPE.options[lstQUESTION_TYPE.options.selectedIndex].value;
	rowQUESTION.QUESTION_TYPE    = lstQUESTION_TYPE.options[lstQUESTION_TYPE.options.selectedIndex].value;
	rowQUESTION.DESCRIPTION      = lstQUESTION_TYPE.options[lstQUESTION_TYPE.options.selectedIndex].text;
	rowQUESTION.QUESTION_NUMBER  = 1;
	try
	{
		SurveyQuestion_RenderSample(rowQUESTION, divQuestionEditViewSample);
	}
	catch(e)
	{
		divQuestionEditViewSample.innerHTML = e.message;
	}
});
	</script>
</SplendidCRM:InlineScript>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# false && !PrintView %>" Runat="Server" />

