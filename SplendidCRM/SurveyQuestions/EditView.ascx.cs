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
using System;
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.SurveyQuestions
{
	/// <summary>
	///		Summary OBJECTIVE for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		#region Properties
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected Guid               gID                          ;
		protected HtmlTable          tblMain                      ;
		protected PlaceHolder        plcSubPanel                  ;
		
		protected TextBox            NAME                         ;
		protected TextBox            DESCRIPTION                  ;
		// 11/24/2018 Paul.  Place image caption in ANSWER_CHOICES. 
		protected Label              ANSWER_CHOICES_LABEL         ;
		protected TextBox            ANSWER_CHOICES               ;
		protected TextBox            COLUMN_CHOICES               ;
		protected TextBox            NA_LABEL                     ;
		protected CheckBox           FORCED_RANKING               ;
		protected TextBox            INVALID_DATE_MESSAGE         ;
		protected TextBox            INVALID_NUMBER_MESSAGE       ;
		protected DropDownList       QUESTION_TYPE                ;
		protected DropDownList       DISPLAY_FORMAT               ;
		// 09/30/2018 Paul.  Add survey record creation to survey. 
		protected Label              SURVEY_TARGET_LABEL          ;
		protected DropDownList       SURVEY_TARGET_MODULE         ;
		protected Label              TARGET_FIELD_LABEL           ;
		protected DropDownList       TARGET_FIELD_NAME            ;
		
		protected CheckBox           REQUIRED                     ;
		protected DropDownList       REQUIRED_TYPE                ;
		protected TextBox            REQUIRED_RESPONSES_MIN       ;
		protected TextBox            REQUIRED_RESPONSES_MAX       ;
		protected Label              REQUIRED_RESPONSES_RANGE     ;
		protected TextBox            REQUIRED_MESSAGE             ;
		protected RadioButtonList    RANDOMIZE_TYPE               ;
		protected CheckBox           RANDOMIZE_NOT_LAST           ;
		
		protected CheckBox           OTHER_ENABLED                ;
		protected TextBox            OTHER_LABEL                  ;
		protected DropDownList       OTHER_HEIGHT                 ;
		protected DropDownList       OTHER_WIDTH                  ;
		protected CheckBox           OTHER_AS_CHOICE              ;
		protected CheckBox           OTHER_ONE_PER_ROW            ;
		protected DropDownList       OTHER_VALIDATION_TYPE        ;
		protected TextBox            OTHER_VALIDATION_MIN         ;
		protected TextBox            OTHER_VALIDATION_MAX         ;
		protected TextBox            OTHER_VALIDATION_MESSAGE     ;
		protected TextBox            OTHER_REQUIRED_MESSAGE       ;

		protected DropDownList       VALIDATION_TYPE              ;
		protected TextBox            VALIDATION_MIN               ;
		protected TextBox            VALIDATION_MAX               ;
		protected TextBox            VALIDATION_MESSAGE           ;
		protected CheckBox           VALIDATION_SUM_ENABLED       ;
		protected TextBox            VALIDATION_NUMERIC_SUM       ;
		protected TextBox            VALIDATION_SUM_MESSAGE       ;

		protected DropDownList       SIZE_UNITS                   ;
		protected DropDownList       SIZE_HEIGHT                  ;
		protected DropDownList       SIZE_WIDTH                   ;
		protected DropDownList       BOX_HEIGHT                   ;
		protected DropDownList       BOX_WIDTH                    ;
		protected DropDownList       COLUMN_WIDTH                 ;
		protected DropDownList       PLACEMENT                    ;
		protected TextBox            SPACING_LEFT                 ;
		protected TextBox            SPACING_TOP                  ;
		protected TextBox            SPACING_RIGHT                ;
		protected TextBox            SPACING_BOTTOM               ;
		
		protected RadioButton        radIMAGE_UPLOAD              ;
		protected RadioButton        radIMAGE_URL                 ;
		protected FileUpload         UPLOAD_IMAGE                 ;
		protected TextBox            IMAGE_URL                    ;
		protected Image              imgIMAGE                     ;
		// 01/01/2016 Paul.  Add categories. 
		protected TextBox            CATEGORIES                   ;

		// 01/01/2016 Paul.  Move display format to the right of Question Type. 
		//protected Table              tblDisplayFormat             ;
		protected Panel              pnlAnswer                    ;
		// 11/07/2018 Paul.  Provide a way to get a single numerical value for lead population.  Just like textbox but with numeric validation. 
		protected Table              tblAnswerChoices             ;
		protected Table              tblInvalidDate               ;
		protected Table              tblInvalidNumber             ;
		protected Table              tblRankingNA                 ;
		protected Panel              pnlRatingScale               ;
		protected Table              tblColumnChoices             ;
		protected Table              tblForcedRanking             ;
		protected Panel              pnlMenuChoices               ;
		protected Panel              pnlDemographic               ;
		protected Panel              pnlRequired                  ;
		protected Table              tblRequiredType              ;
		protected Panel              pnlValidationEnabled         ;
		protected TableRow           trOtherValidationMessage     ;
		protected TableRow           trOtherRequiredMessage       ;
		protected TableRow           trOtherAsChoice              ;
		protected Panel              pnlValidationSum             ;
		protected Panel              pnlName                      ;
		protected Panel              pnlImage                     ;
		protected Panel              pnlOther                     ;
		protected Panel              pnlRandomize                 ;
		protected Table              tblColumnWidth               ;
		protected DropDownList       lstNumberOfMenus             ;
		protected DropDownList       lstRatingScale               ;
		protected Label              labDisplayFormat             ;
		protected HtmlGenericControl spnOtherValidation           ;
		protected HtmlGenericControl spnValidation                ;
		protected CheckBox           NA_ENABLED                   ;
		protected Table              tblSize                      ;
		protected Table              tblBoxSize                   ;
		protected HtmlGenericControl divAnswerOther               ;
		protected HtmlGenericControl divRequired                  ;
		protected HtmlGenericControl divRandomize                 ;
		protected HtmlGenericControl divValidationEnabled         ;
		protected HtmlGenericControl divValidationSum             ;
		protected HtmlGenericControl divQuestionSize              ;

		protected Table              tblRATING                    ;
		protected Table              tblMENU                      ;
		protected Table              tblDEMOGRAPHIC               ;

		// 10/08/2014 Paul.  Add Range question type. 
		protected Panel              pnlRange                     ;
		protected TextBox            RANGE_MIN                    ;
		protected TextBox            RANGE_MAX                    ;
		protected TextBox            RANGE_STEP                   ;
		#endregion

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			Guid gSURVEY_ID      = Sql.ToGuid(Request["SURVEY_ID"     ]);
			Guid gSURVEY_PAGE_ID = Sql.ToGuid(Request["SURVEY_PAGE_ID"]);
			// 07/31/2013 Paul.  Add SaveNew button to SurveyQuestion. 
			// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveNew" || e.CommandName == "SaveDuplicate" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + LayoutEditView);
					
					if ( plcSubPanel.Visible )
					{
						foreach ( Control ctl in plcSubPanel.Controls )
						{
							InlineEditControl ctlSubPanel = ctl as InlineEditControl;
							if ( ctlSubPanel != null )
							{
								ctlSubPanel.ValidateEditViewFields();
							}
						}
					}
					if ( Page.IsValid )
					{
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *"               + ControlChars.CrLf
								     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, m_sMODULE, "edit");
									Sql.AppendParameter(cmd, gID, "ID", false);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											rowCurrent = dtCurrent.Rows[0];
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											gID = Guid.Empty;
										}
									}
								}
							}
							
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
							
							string sQUESTION_TYPE  = QUESTION_TYPE.SelectedValue;
							string sANSWER_CHOICES = new DynamicControl(this, rowCurrent, "ANSWER_CHOICES").Text.TrimEnd();
							string sCOLUMN_CHOICES = new DynamicControl(this, rowCurrent, "COLUMN_CHOICES").Text.TrimEnd();
							if ( sQUESTION_TYPE == "Rating Scale" )
							{
								XmlDocument xml = new XmlDocument();
								xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
								xml.AppendChild(xml.CreateElement("Ratings"));
								int nRATINGS_SCALE = Sql.ToInteger(lstRatingScale.SelectedValue);
								for ( int n = 1; n <= nRATINGS_SCALE; n++ )
								{
									XmlNode xRating = xml.CreateElement("Rating");
									xml.DocumentElement.AppendChild(xRating);
									XmlNode xLabel  = xml.CreateElement("Label" );
									XmlNode xWeight = xml.CreateElement("Weight");
									xRating.AppendChild(xLabel );
									xRating.AppendChild(xWeight);
									xLabel .InnerText = new DynamicControl(this, "RATING_LABEL_"  + n.ToString()).Text;
									xWeight.InnerText = new DynamicControl(this, "RATING_WEIGHT_" + n.ToString()).IntegerValue.ToString();
								}
								sCOLUMN_CHOICES = xml.OuterXml;
							}
							else if ( sQUESTION_TYPE == "Dropdown Matrix" )
							{
								XmlDocument xml = new XmlDocument();
								xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
								xml.AppendChild(xml.CreateElement("Menus"));
								int nMENU_ITEMS = Sql.ToInteger(lstNumberOfMenus.SelectedValue);
								for ( int n = 1; n <= nMENU_ITEMS; n++ )
								{
									XmlNode xMenu = xml.CreateElement("Menu");
									xml.DocumentElement.AppendChild(xMenu);
									XmlNode xHeading = xml.CreateElement("Heading");
									XmlNode xOptions = xml.CreateElement("Options");
									xMenu.AppendChild(xHeading);
									xMenu.AppendChild(xOptions);
									xHeading.InnerText = new DynamicControl(this, "MENU_HEADING_" + n.ToString()).Text.TrimEnd();
									xOptions.InnerText = new DynamicControl(this, "MENU_OPTIONS_" + n.ToString()).Text.TrimEnd();
								}
								sCOLUMN_CHOICES = xml.OuterXml;
							}
							else if ( sQUESTION_TYPE == "Demographic" )
							{
								REQUIRED.Checked = false;
								XmlDocument xml = new XmlDocument();
								xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
								xml.AppendChild(xml.CreateElement("Demographic"));
								DataTable dtDemographicNames = SplendidCache.List("survey_question_demographic_fields");
								for ( int n = 0; n < dtDemographicNames.Rows.Count; n++ )
								{
									string sNAME         = Sql.ToString(dtDemographicNames.Rows[n]["NAME"        ]);
									string sDISPLAY_NAME = Sql.ToString(dtDemographicNames.Rows[n]["DISPLAY_NAME"]);
									XmlNode xField = xml.CreateElement("Field");
									xml.DocumentElement.AppendChild(xField);
									XmlAttribute attrName     = xml.CreateAttribute("Name"    );
									XmlAttribute attrVisible  = xml.CreateAttribute("Visible" );
									XmlAttribute attrRequired = xml.CreateAttribute("Required");
									xField.Attributes.SetNamedItem(attrName    );
									xField.Attributes.SetNamedItem(attrVisible );
									xField.Attributes.SetNamedItem(attrRequired);
									attrName.Value     = sNAME;
									attrVisible .Value = new DynamicControl(this, "DEMOGRAPHIC_" + sNAME + "_VISIBLE" ).Checked.ToString();
									attrRequired.Value = new DynamicControl(this, "DEMOGRAPHIC_" + sNAME + "_REQUIRED").Checked.ToString();
									xField.InnerText   = new DynamicControl(this, "DEMOGRAPHIC_" + sNAME).Text;
									if ( new DynamicControl(this, "DEMOGRAPHIC_" + sNAME + "_REQUIRED").Checked )
										REQUIRED.Checked = true;
									// 09/30/2018 Paul.  Add survey record creation to survey. 
									if ( !Sql.IsEmptyString(SURVEY_TARGET_MODULE.SelectedValue) )
									{
										XmlAttribute attrTargetField = xml.CreateAttribute("TargetField");
										xField.Attributes.SetNamedItem(attrTargetField);
										attrTargetField.Value = new DynamicControl(this, "DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_NAME").SelectedValue;
									}
								}
								sCOLUMN_CHOICES = xml.OuterXml;
							}
							// 10/08/2014 Paul.  Add Range question type. 
							else if ( sQUESTION_TYPE == "Range" )
							{
								sANSWER_CHOICES = Sql.ToInteger(RANGE_MIN.Text).ToString() + ControlChars.CrLf + Sql.ToInteger(RANGE_MAX.Text).ToString() + ControlChars.CrLf + Sql.ToInteger(RANGE_STEP.Text).ToString();
							}
							// 06/02/2013 Paul.  Clear any unused fields. 
							// 11/07/2018 Paul.  Provide a way to get a single numerical or date value for lead population.
							// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population. 
							// 11/24/2018 Paul.  Place image caption in ANSWER_CHOICES. 
							// 04/18/2019 Paul.  Hidden value does save ANSWER_CHOICES. 
							if ( sQUESTION_TYPE == "Text Area" || sQUESTION_TYPE == "Textbox" || sQUESTION_TYPE == "Plain Text" || sQUESTION_TYPE == "Demographic" || sQUESTION_TYPE == "Single Numerical" || sQUESTION_TYPE == "Single Date" )
								sANSWER_CHOICES = String.Empty;
							if ( sQUESTION_TYPE != "Radio Matrix" && sQUESTION_TYPE != "Checkbox Matrix" && sQUESTION_TYPE != "Dropdown Matrix" && sQUESTION_TYPE != "Rating Scale" && sQUESTION_TYPE != "Demographic" || sQUESTION_TYPE == "Range" )
								sCOLUMN_CHOICES = String.Empty;
							// 11/07/2018 Paul.  Provide a way to get a single numerical or date value for lead population.
							// 11/10/2018 Paul.  Provide a way to get a single checkbox for lead population. 
							// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population. 
							if ( sQUESTION_TYPE == "Text Area" || sQUESTION_TYPE == "Ranking" || sQUESTION_TYPE.StartsWith("Textbox") || sQUESTION_TYPE == "Plain Text" || sQUESTION_TYPE == "Image" || sQUESTION_TYPE == "Date" || sQUESTION_TYPE == "Demographic" || sQUESTION_TYPE == "Range" || sQUESTION_TYPE == "Single Numerical" || sQUESTION_TYPE == "Single Date" || sQUESTION_TYPE == "Single Checkbox" || sQUESTION_TYPE == "Hidden" )
							{
								OTHER_AS_CHOICE         .Checked       = false;
								OTHER_ONE_PER_ROW       .Checked       = false;
								OTHER_REQUIRED_MESSAGE  .Text          = String.Empty;
								OTHER_VALIDATION_TYPE   .SelectedValue = String.Empty;
								OTHER_VALIDATION_MIN    .Text          = String.Empty;
								OTHER_VALIDATION_MAX    .Text          = String.Empty;
								OTHER_VALIDATION_MESSAGE.Text          = String.Empty;
							}
							// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population. 
							if ( sQUESTION_TYPE == "Plain Text" || sQUESTION_TYPE == "Image" || sQUESTION_TYPE == "Hidden" )
								REQUIRED_MESSAGE.Text = String.Empty;
							string sREQUIRED_TYPE = (tblRequiredType.Visible ? REQUIRED_TYPE.SelectedValue : String.Empty);
							// 08/14/2013 Paul.  Don't clear validation for a Textbox. 
							// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population. 
							if ( sQUESTION_TYPE == "Text Area" || sQUESTION_TYPE == "Ranking" || sQUESTION_TYPE == "Plain Text" || sQUESTION_TYPE == "Image" || sQUESTION_TYPE == "Demographic" || sQUESTION_TYPE == "Range" || sQUESTION_TYPE == "Hidden" )
							{
								VALIDATION_TYPE   .SelectedValue = String.Empty;
								VALIDATION_MIN    .Text          = String.Empty;
								VALIDATION_MAX    .Text          = String.Empty;
								VALIDATION_MESSAGE.Text          = String.Empty;
							}
							// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population. 
							// 03/10/2019 Paul.  The Name field will be visible to all. 
							//if ( sQUESTION_TYPE != "Plain Text" && sQUESTION_TYPE != "Image" && sQUESTION_TYPE != "Hidden" )
							//	NAME.Text = String.Empty;
							if ( sQUESTION_TYPE != "Textbox Numerical" )
							{
								VALIDATION_NUMERIC_SUM.Text = String.Empty;
								VALIDATION_SUM_MESSAGE.Text = String.Empty;
							}
							// 11/07/2018 Paul.  Provide a way to get a single numerical or date value for lead population.
							// 11/10/2018 Paul.  Provide a way to get a single checkbox for lead population. 
							// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population. 
							if ( sQUESTION_TYPE == "Text Area" || sQUESTION_TYPE == "Textbox" || sQUESTION_TYPE == "Plain Text" || sQUESTION_TYPE == "Image" || sQUESTION_TYPE == "Demographic" || sQUESTION_TYPE == "Range" || sQUESTION_TYPE == "Single Numerical" || sQUESTION_TYPE == "Single Date" || sQUESTION_TYPE == "Single Checkbox" || sQUESTION_TYPE == "Hidden" )
								RANDOMIZE_TYPE.ClearSelection();
							if ( sQUESTION_TYPE != "Image" )
								IMAGE_URL.Text = String.Empty;
							string sSIZE_WIDTH   = String.Empty;
							string sSIZE_HEIGHT  = String.Empty;
							string sBOX_WIDTH    = String.Empty;
							string sBOX_HEIGHT   = String.Empty;
							string sCOLUMN_WIDTH = String.Empty;
							// 11/07/2018 Paul.  Provide a way to get a single numerical or date value for lead population.
							// 11/10/2018 Paul.  Provide a way to get a single checkbox for lead population. 
							// 11/12/2018 Paul.  Enable size fields. 
							//if ( sQUESTION_TYPE != "Text Area" && sQUESTION_TYPE != "Textbox" && sQUESTION_TYPE != "Image" && sQUESTION_TYPE != "Single Numerical" && sQUESTION_TYPE != "Single Date" && sQUESTION_TYPE != "Single Checkbox" )
								sSIZE_WIDTH   = SIZE_WIDTH .SelectedValue;
							// 11/12/2018 Paul.  This comment is just to confirm that sSIZE_HEIGHT is not used. 
							if ( sQUESTION_TYPE == "" )
								sSIZE_HEIGHT  = SIZE_HEIGHT.SelectedValue;
							// 11/07/2018 Paul.  Provide a way to get a single numerical or date value for lead population.
							// 11/10/2018 Paul.  Provide a way to get a single checkbox for lead population. 
							if ( sQUESTION_TYPE == "Text Area" || sQUESTION_TYPE == "Textbox" || sQUESTION_TYPE == "Textbox Multiple" || sQUESTION_TYPE == "Textbox Numerical" || sQUESTION_TYPE == "Demographic" || sQUESTION_TYPE == "Single Numerical" || sQUESTION_TYPE == "Single Date" || sQUESTION_TYPE == "Single Checkbox" )
								sBOX_WIDTH    = BOX_WIDTH .SelectedValue;
							if ( sQUESTION_TYPE == "Text Area" )
								sBOX_HEIGHT   = BOX_HEIGHT.SelectedValue;
							if ( sQUESTION_TYPE == "Rating Scale" || sQUESTION_TYPE == "Radio Matrix" || sQUESTION_TYPE == "Checkbox Matrix" || sQUESTION_TYPE == "Dropdown Matrix" || sQUESTION_TYPE == "Textbox Multiple" || sQUESTION_TYPE == "Textbox Numerical" || sQUESTION_TYPE == "Date" || sQUESTION_TYPE == "Demographic" || sQUESTION_TYPE == "Range" )
								sCOLUMN_WIDTH = COLUMN_WIDTH.SelectedValue;
							string sDISPLAY_FORMAT = String.Empty;
							// 10/08/2014 Paul.  Add Range question type. 
							// 03/09/2019 Paul.  Single Date also uses DISPLAY_FORMAT. 
							if ( sQUESTION_TYPE == "Radio" || sQUESTION_TYPE == "Checkbox" || sQUESTION_TYPE == "Date" || sQUESTION_TYPE == "Single Date" || sQUESTION_TYPE == "Range" )
								sDISPLAY_FORMAT = DISPLAY_FORMAT.SelectedValue;
							// 08/17/2018 Paul.  For date validation, we need to store time in seconds as the database field is an integer.  Convert to seconds since 1970. 
							int nREQUIRED_RESPONSES_MIN = 0;
							int nREQUIRED_RESPONSES_MAX = 0;
							if ( sQUESTION_TYPE == "Date" )
							{
								DateTime dtREQUIRED_RESPONSES_MIN = Sql.ToDateTime(REQUIRED_RESPONSES_MIN.Text);
								DateTime dtREQUIRED_RESPONSES_MAX = Sql.ToDateTime(REQUIRED_RESPONSES_MAX.Text);
								if ( dtREQUIRED_RESPONSES_MIN != DateTime.MinValue )
									nREQUIRED_RESPONSES_MIN = (int)(dtREQUIRED_RESPONSES_MIN - new DateTime(1970, 1, 1)).TotalSeconds;
								if ( dtREQUIRED_RESPONSES_MAX != DateTime.MinValue )
									nREQUIRED_RESPONSES_MAX = (int)(dtREQUIRED_RESPONSES_MAX - new DateTime(1970, 1, 1)).TotalSeconds;
							}
							else
							{
								nREQUIRED_RESPONSES_MIN = Sql.ToInteger(REQUIRED_RESPONSES_MIN.Text);
								nREQUIRED_RESPONSES_MAX = Sql.ToInteger(REQUIRED_RESPONSES_MAX.Text);
							}
							
							// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
							// Apply duplicate checking after PreSave business rules, but before trasnaction. 
							bool bDUPLICATE_CHECHING_ENABLED = Sql.ToBoolean(Application["CONFIG.enable_duplicate_check"]) && Sql.ToBoolean(Application["Modules." + m_sMODULE + ".DuplicateCheckingEnabled"]) && (e.CommandName != "SaveDuplicate");
							if ( bDUPLICATE_CHECHING_ENABLED )
							{
								if ( Utils.DuplicateCheck(Application, con, m_sMODULE, gID, this, rowCurrent) > 0 )
								{
									ctlDynamicButtons.ShowButton("SaveDuplicate", true);
									ctlFooterButtons .ShowButton("SaveDuplicate", true);
									throw(new Exception(L10n.Term(".ERR_DUPLICATE_EXCEPTION")));
								}
							}
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									Guid gImageID = Guid.Empty;
									if ( radIMAGE_UPLOAD.Checked && UPLOAD_IMAGE.HasFile )
									{
										string sFILENAME = String.Empty;
										SplendidCRM.FileBrowser.FileWorkerUtils.LoadImage(ref gImageID, ref sFILENAME, UPLOAD_IMAGE.UniqueID, trn);
										if ( !Sql.IsEmptyGuid(gImageID) )
										{
											IMAGE_URL.Text = "~/Images/EmailImage.aspx?ID=" + gImageID.ToString();
											radIMAGE_URL.Checked = true;
										}
									}
									// 01/01/2016 Paul.  Add categories. 
									SqlProcs.spSURVEY_QUESTIONS_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID").ID
										, new DynamicControl(this, rowCurrent, "TEAM_ID"         ).ID
										, new DynamicControl(this, rowCurrent, "TEAM_SET_LIST"   ).Text
										, NAME                       .Text
										, DESCRIPTION                .Text
										, QUESTION_TYPE              .SelectedValue
										, sDISPLAY_FORMAT
										, sANSWER_CHOICES
										, sCOLUMN_CHOICES
										, FORCED_RANKING             .Checked
										, (sQUESTION_TYPE == "Date"              ? INVALID_DATE_MESSAGE  .Text : String.Empty)
										, (sQUESTION_TYPE == "Textbox Numerical" ? INVALID_NUMBER_MESSAGE.Text : String.Empty)
										, NA_ENABLED                 .Checked
										, (NA_ENABLED.Checked ? NA_LABEL.Text : String.Empty)
										, OTHER_ENABLED              .Checked
										, (OTHER_ENABLED.Checked ? OTHER_LABEL.Text : String.Empty)
										, Sql.ToInteger(OTHER_HEIGHT.SelectedValue)
										, Sql.ToInteger(OTHER_WIDTH .SelectedValue)
										, OTHER_AS_CHOICE            .Checked
										, OTHER_ONE_PER_ROW          .Checked
										, OTHER_REQUIRED_MESSAGE     .Text
										, OTHER_VALIDATION_TYPE      .SelectedValue
										, OTHER_VALIDATION_MIN       .Text
										, OTHER_VALIDATION_MAX       .Text
										, OTHER_VALIDATION_MESSAGE   .Text
										, REQUIRED                   .Checked
										, sREQUIRED_TYPE
										, nREQUIRED_RESPONSES_MIN
										, nREQUIRED_RESPONSES_MAX
										, REQUIRED_MESSAGE           .Text
										, VALIDATION_TYPE            .SelectedValue
										, VALIDATION_MIN             .Text
										, VALIDATION_MAX             .Text
										, VALIDATION_MESSAGE         .Text
										, VALIDATION_SUM_ENABLED     .Checked
										, Sql.ToInteger(VALIDATION_NUMERIC_SUM.Text)
										, VALIDATION_SUM_MESSAGE     .Text
										, RANDOMIZE_TYPE             .SelectedValue
										, RANDOMIZE_NOT_LAST         .Checked
										, sSIZE_WIDTH
										, sSIZE_HEIGHT
										, sBOX_WIDTH
										, sBOX_HEIGHT
										, sCOLUMN_WIDTH
										, PLACEMENT                  .SelectedValue
										, Sql.ToInteger(SPACING_LEFT  .Text)
										, Sql.ToInteger(SPACING_TOP   .Text)
										, Sql.ToInteger(SPACING_RIGHT .Text)
										, Sql.ToInteger(SPACING_BOTTOM.Text)
										, IMAGE_URL                  .Text
										, CATEGORIES                 .Text.Trim()
										// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
										, new DynamicControl(this, rowCurrent, "ASSIGNED_SET_LIST").Text
										// 09/30/2018 Paul.  Add survey record creation to survey. 
										, SURVEY_TARGET_MODULE.SelectedValue
										, TARGET_FIELD_NAME.SelectedValue
										, trn
										);
									SqlProcs.spTRACKER_Update
										( Security.USER_ID
										, m_sMODULE
										, gID
										, new DynamicControl(this, rowCurrent, "DESCRIPTION").Text
										, "save"
										, trn
										);
									if ( plcSubPanel.Visible )
									{
										foreach ( Control ctl in plcSubPanel.Controls )
										{
											InlineEditControl ctlSubPanel = ctl as InlineEditControl;
											if ( ctlSubPanel != null )
											{
												ctlSubPanel.Save(gID, m_sMODULE, trn);
											}
										}
									}
									if ( !Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
										SqlProcs.spSURVEY_PAGES_QUESTIONS_Update(gSURVEY_PAGE_ID, gID, 0, trn);
									trn.Commit();
									SplendidCache.ClearFavorites();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									ctlDynamicButtons.ErrorText = ex.Message;
									return;
								}
							}
							rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
						}
						
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						// 06/24/2013 Paul.  We want the save operation to go to the view so that we can edit again. 
						//else if ( !Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
						//	Response.Redirect("~/SurveyPages/view.aspx?ID=" + gSURVEY_PAGE_ID.ToString());
						// 07/31/2013 Paul.  Add SaveNew button to SurveyQuestion. 
						else if ( e.CommandName == "SaveNew" )
						{
							string sSaveNewParams = String.Empty;
							if ( !Sql.IsEmptyGuid(gSURVEY_ID) )
							{
								sSaveNewParams += Sql.IsEmptyString(sSaveNewParams) ? "?" : "&";
								sSaveNewParams += "SURVEY_ID=" + gSURVEY_ID.ToString();
							}
							if ( !Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
							{
								sSaveNewParams += Sql.IsEmptyString(sSaveNewParams) ? "?" : "&";
								sSaveNewParams += "SURVEY_PAGE_ID=" + gSURVEY_PAGE_ID.ToString();
							}
							Response.Redirect("edit.aspx" + sSaveNewParams);
						}
						else
							Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				// 06/22/2013 Paul.  Cancel should return to the Survey Page. 
				if ( !Sql.IsEmptyGuid(gSURVEY_ID) )
					Response.Redirect("~/Surveys/view.aspx?ID=" + gSURVEY_ID.ToString());
				else if ( !Sql.IsEmptyGuid(gSURVEY_PAGE_ID) )
					Response.Redirect("~/SurveyPages/view.aspx?ID=" + gSURVEY_PAGE_ID.ToString());
				else if ( !Sql.IsEmptyGuid(gID) )
					Response.Redirect("view.aspx?ID=" + gID.ToString());
				else
					Response.Redirect("default.aspx");
			}
		}

		#region Change Events
		protected void QUESTION_TYPE_SelectedIndexChanged(object sender, EventArgs e)
		{
			// 01/01/2016 Paul.  Move display format to the right of Question Type. 
			//tblDisplayFormat    .Visible = false;
			labDisplayFormat    .Visible = false;
			DISPLAY_FORMAT      .Visible = false;
			pnlAnswer           .Visible = false;
			// 11/07/2018 Paul.  Provide a way to get a single numerical value for lead population.  Just like textbox but with numeric validation. 
			tblAnswerChoices    .Visible = true;
			// 11/24/2018 Paul.  Place image caption in ANSWER_CHOICES. 
			ANSWER_CHOICES_LABEL.Text    = L10n.Term("SurveyQuestions.LBL_ANSWER_CHOICES");
			// 11/10/2018 Paul.  QuestionSize is visible to all but Hidden. 
			divQuestionSize     .Visible = true;
			// 10/08/2014 Paul.  Add Range question type. 
			pnlRange            .Visible = false;
			tblInvalidDate      .Visible = false;
			tblInvalidNumber    .Visible = false;
			tblRankingNA        .Visible = false;
			pnlRatingScale      .Visible = false;
			tblColumnChoices    .Visible = false;
			tblForcedRanking    .Visible = false;
			pnlMenuChoices      .Visible = false;
			pnlDemographic      .Visible = false;
			pnlRequired         .Visible = false;
			REQUIRED            .Visible = true;
			tblRequiredType     .Visible = false;
			pnlValidationEnabled.Visible = false;
			pnlValidationSum    .Visible = false;
			pnlOther            .Visible = false;
			pnlRandomize        .Visible = false;
			// 03/10/2019 Paul.  The Name field will be visible to all. 
			pnlName             .Visible = true;
			pnlImage            .Visible = false;
			OTHER_AS_CHOICE     .Visible = false;
			OTHER_ONE_PER_ROW   .Visible = false;
			trOtherAsChoice     .Visible = false;
			tblSize             .Visible = false;
			SIZE_UNITS          .Visible = false;
			SIZE_HEIGHT         .Visible = false;
			SIZE_WIDTH          .Visible = false;
			tblColumnWidth      .Visible = false;
			RANDOMIZE_NOT_LAST  .Visible = false;
			tblBoxSize          .Visible = false;
			BOX_WIDTH           .Visible = false;
			BOX_HEIGHT          .Visible = false;
			// 09/30/2018 Paul.  Add survey record creation to survey. 
			SURVEY_TARGET_LABEL .Visible = false;
			SURVEY_TARGET_MODULE.Visible = false;
			TARGET_FIELD_LABEL  .Visible = false;
			TARGET_FIELD_NAME   .Visible = false;
			switch ( QUESTION_TYPE.SelectedValue )
			{
				case "Radio"            :
					pnlAnswer           .Visible = true;
					pnlOther            .Visible = true;
					OTHER_AS_CHOICE     .Visible = true;
					trOtherAsChoice     .Visible = true;
					pnlRequired         .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					// 01/01/2016 Paul.  Move display format to the right of Question Type. 
					//tblDisplayFormat    .Visible = true;
					labDisplayFormat    .Visible = true;
					DISPLAY_FORMAT      .Visible = true;
					labDisplayFormat    .Text    = L10n.Term("SurveyQuestions.LBL_DISPLAY_FORMAT");
					DISPLAY_FORMAT      .DataSource = SplendidCache.List("survey_question_format");
					DISPLAY_FORMAT      .DataBind();
					// 09/30/2018 Paul.  Add survey record creation to survey. 
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					break;
				case "Checkbox"         :
					pnlAnswer           .Visible = true;
					pnlOther            .Visible = true;
					OTHER_AS_CHOICE     .Visible = true;
					trOtherAsChoice     .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					// 01/01/2016 Paul.  Move display format to the right of Question Type. 
					//tblDisplayFormat    .Visible = true;
					labDisplayFormat    .Visible = true;
					DISPLAY_FORMAT      .Visible = true;
					labDisplayFormat    .Text    = L10n.Term("SurveyQuestions.LBL_DISPLAY_FORMAT");
					DISPLAY_FORMAT      .DataSource = SplendidCache.List("survey_question_format");
					DISPLAY_FORMAT      .DataBind();
					break;
				// 11/10/2018 Paul.  Provide a way to get a single checkbox for lead population. 
				case "Single Checkbox":
					tblInvalidNumber    .Visible = false;
					pnlAnswer           .Visible = true;
					tblAnswerChoices    .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = false;
					pnlValidationEnabled.Visible = false;
					pnlValidationSum    .Visible = false;
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					if ( sender != null )
						Utils.SetSelectedValue(BOX_WIDTH, "50");
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					break;
				case "Dropdown"         :
					pnlAnswer           .Visible = true;
					pnlOther            .Visible = true;
					OTHER_AS_CHOICE     .Visible = true;
					trOtherAsChoice     .Visible = true;
					pnlRequired         .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					// 09/30/2018 Paul.  Add survey record creation to survey. 
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					break;
				case "Ranking"          :
					pnlAnswer           .Visible = true;
					tblRankingNA        .Visible = true;
					pnlRequired         .Visible = true;
					pnlRandomize        .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					break;
				case "Rating Scale"     :
					pnlAnswer           .Visible = true;
					tblRankingNA        .Visible = true;
					pnlOther            .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					pnlRatingScale      .Visible = true;
					tblForcedRanking    .Visible = true;
					tblColumnWidth      .Visible = true;
					OTHER_ONE_PER_ROW   .Visible = true;
					trOtherAsChoice     .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					lstRatingScale_SelectedIndexChanged(null, null);
					break;
				case "Radio Matrix"     :
					pnlAnswer           .Visible = true;
					tblColumnChoices    .Visible = true;
					pnlOther            .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					tblForcedRanking    .Visible = true;
					tblColumnWidth      .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					break;
				case "Checkbox Matrix"  :
					pnlAnswer           .Visible = true;
					tblColumnChoices    .Visible = true;
					pnlOther            .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					tblColumnWidth      .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					break;
				case "Dropdown Matrix"  :
					pnlAnswer           .Visible = true;
					pnlMenuChoices      .Visible = true;
					pnlOther            .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					lstNumberOfMenus_SelectedIndexChanged(null, null);
					tblColumnWidth      .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					break;
				case "Text Area"        :
					pnlRequired         .Visible = true;
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					BOX_HEIGHT          .Visible = true;
					if ( sender != null )
					{
						Utils.SetSelectedValue(BOX_WIDTH , "50");
						Utils.SetSelectedValue(BOX_HEIGHT, "3" );
					}
					// 09/30/2018 Paul.  Add survey record creation to survey. 
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					break;
				case "Textbox"          :
					pnlRequired         .Visible = true;
					pnlValidationEnabled.Visible = true;
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					if ( sender != null )
						Utils.SetSelectedValue(BOX_WIDTH, "50");
					// 09/30/2018 Paul.  Add survey record creation to survey. 
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					// 11/07/2018 Paul.  The validation type can change its options. 
					VALIDATION_TYPE     .DataSource = SplendidCache.List("survey_question_validation");
					VALIDATION_TYPE     .DataBind();
					break;
				case "Textbox Multiple" :
					pnlAnswer           .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					pnlValidationEnabled.Visible = true;
					tblColumnWidth      .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					if ( sender != null )
						Utils.SetSelectedValue(BOX_WIDTH, "50");
					// 11/07/2018 Paul.  The validation type can change its options. 
					VALIDATION_TYPE     .DataSource = SplendidCache.List("survey_question_validation");
					VALIDATION_TYPE     .DataBind();
					break;
				case "Textbox Numerical":
					tblInvalidNumber    .Visible = true;
					pnlAnswer           .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					pnlValidationSum    .Visible = true;
					tblColumnWidth      .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					if ( sender != null )
						Utils.SetSelectedValue(BOX_WIDTH, "50");
					if ( sender != null && Sql.IsEmptyString(INVALID_NUMBER_MESSAGE.Text) )
						INVALID_NUMBER_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_INVALID_NUMBER_MESSAGE_DEFAULT");
					break;
				// 11/07/2018 Paul.  Provide a way to get a single numerical value for lead population.  Just like textbox but with numeric validation. 
				case "Single Numerical":
					tblInvalidNumber    .Visible = true;
					pnlAnswer           .Visible = true;
					tblAnswerChoices    .Visible = false;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = false;
					pnlValidationEnabled.Visible = true;
					pnlValidationSum    .Visible = false;
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					if ( sender != null )
						Utils.SetSelectedValue(BOX_WIDTH, "50");
					if ( sender != null && Sql.IsEmptyString(INVALID_NUMBER_MESSAGE.Text) )
						INVALID_NUMBER_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_INVALID_NUMBER_MESSAGE_DEFAULT");
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					VALIDATION_TYPE     .DataSource = SplendidCache.List("survey_question_validation_numerical");
					VALIDATION_TYPE     .DataBind();
					break;
				case "Plain Text"       :
					pnlName             .Visible = true;
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					break;
				// 11/10/2018 Paul.  Provide a way to get a hidden value for lead population.
				case "Hidden"       :
					pnlAnswer           .Visible = true;
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					divQuestionSize     .Visible = false;
					break;
				case "Image"            :
					pnlName             .Visible = true;
					pnlImage            .Visible = true;
					// 11/24/2018 Paul.  Place image caption in ANSWER_CHOICES. 
					pnlAnswer           .Visible = true;
					tblAnswerChoices    .Visible = true;
					ANSWER_CHOICES_LABEL.Text    = L10n.Term("SurveyQuestions.LBL_IMAGE_CAPTION");
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					break;
				case "Date"             :
					pnlAnswer           .Visible = true;
					tblInvalidDate      .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = true;
					pnlRandomize        .Visible = true;
					tblColumnWidth      .Visible = true;
					RANDOMIZE_NOT_LAST  .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					// 01/01/2016 Paul.  Move display format to the right of Question Type. 
					//tblDisplayFormat    .Visible = true;
					labDisplayFormat    .Visible = true;
					DISPLAY_FORMAT      .Visible = true;
					labDisplayFormat    .Text    = L10n.Term("SurveyQuestions.LBL_DATE_FORMAT");
					DISPLAY_FORMAT      .DataSource = SplendidCache.List("survey_question_date_format");
					DISPLAY_FORMAT      .DataBind();
					if ( sender != null && Sql.IsEmptyString(INVALID_DATE_MESSAGE.Text) )
						INVALID_DATE_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_INVALID_DATE_MESSAGE_DEFAULT");
					break;
				// 11/07/2018 Paul.  Provide a way to get a single numerical value for lead population.  Just like textbox but with numeric validation. 
				case "Single Date":
					pnlAnswer           .Visible = true;
					tblAnswerChoices    .Visible = false;
					tblInvalidDate      .Visible = true;
					pnlRequired         .Visible = true;
					tblRequiredType     .Visible = false;
					pnlValidationEnabled.Visible = true;
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					// 01/01/2016 Paul.  Move display format to the right of Question Type. 
					//tblDisplayFormat    .Visible = true;
					labDisplayFormat    .Visible = true;
					DISPLAY_FORMAT      .Visible = true;
					labDisplayFormat    .Text    = L10n.Term("SurveyQuestions.LBL_DATE_FORMAT");
					DISPLAY_FORMAT      .DataSource = SplendidCache.List("survey_question_date_format");
					DISPLAY_FORMAT      .DataBind();
					if ( sender != null && Sql.IsEmptyString(INVALID_DATE_MESSAGE.Text) )
						INVALID_DATE_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_INVALID_DATE_MESSAGE_DEFAULT");
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					VALIDATION_TYPE     .DataSource = SplendidCache.List("survey_question_validation_date");
					VALIDATION_TYPE     .DataBind();
					break;
				case "Demographic"      :
					REQUIRED            .Visible = false;
					pnlDemographic      .Visible = true;
					pnlRequired         .Visible = true;
					tblColumnWidth      .Visible = true;
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					tblBoxSize          .Visible = true;
					BOX_WIDTH           .Visible = true;
					if ( sender != null )
						Utils.SetSelectedValue(BOX_WIDTH, "50");
					// 09/30/2018 Paul.  Add survey record creation to survey. 
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					//TARGET_FIELD_LABEL  .Visible = true;
					//TARGET_FIELD_NAME   .Visible = true;
					break;
				// 10/08/2014 Paul.  Add Range question type. 
				case "Range"            :
					pnlRange            .Visible = true;
					// 11/12/2018 Paul.  Enable size fields. 
					tblSize             .Visible = true;
					SIZE_UNITS          .Visible = true;
					SIZE_WIDTH          .Visible = true;
					SIZE_UNITS_SelectedIndexChanged(null, null);
					// 01/01/2016 Paul.  Move display format to the right of Question Type. 
					//tblDisplayFormat    .Visible = true;
					labDisplayFormat    .Visible = true;
					DISPLAY_FORMAT      .Visible = true;
					labDisplayFormat    .Text    = L10n.Term("SurveyQuestions.LBL_DISPLAY_FORMAT");
					DISPLAY_FORMAT      .DataSource = SplendidCache.List("survey_question_range_format");
					DISPLAY_FORMAT      .DataBind();
					pnlRequired         .Visible = true;
					if ( sender != null )
					{
						RANGE_MIN .Text = "0"  ;
						RANGE_MAX .Text = "100";
						RANGE_STEP.Text = "1"  ;
					}
					// 11/07/2018 Paul.  Add survey record creation to survey. 
					SURVEY_TARGET_LABEL .Visible = true;
					SURVEY_TARGET_MODULE.Visible = true;
					TARGET_FIELD_LABEL  .Visible = true;
					TARGET_FIELD_NAME   .Visible = true;
					break;
			}
			if ( sender != null )
				QUESTION_TYPE.Focus();
		}

		protected void REQUIRED_TYPE_SelectedIndexChanged(object sender, EventArgs e)
		{
			REQUIRED_RESPONSES_MIN  .Visible = false;
			REQUIRED_RESPONSES_MAX  .Visible = false;
			REQUIRED_RESPONSES_RANGE.Visible = false;
			switch ( REQUIRED_TYPE.SelectedValue )
			{
				case "All"     :
					break;
				case "At Least":
					REQUIRED_RESPONSES_MIN.Visible = true;
					if ( sender != null )
					{
						REQUIRED_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_REQUIRED_MESSAGE_DEFAULT1");
					}
					break;
				case "At Most" :
					REQUIRED_RESPONSES_MAX.Visible = true;
					if ( sender != null )
					{
						REQUIRED_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_REQUIRED_MESSAGE_DEFAULT1");
					}
					break;
				case "Exactly" :
					REQUIRED_RESPONSES_MIN.Visible = true;
					if ( sender != null )
					{
						REQUIRED_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_REQUIRED_MESSAGE_DEFAULT1");
					}
					break;
				case "Range"   :
					REQUIRED_RESPONSES_MIN  .Visible = true;
					REQUIRED_RESPONSES_MAX  .Visible = true;
					REQUIRED_RESPONSES_RANGE.Visible = true;
					if ( sender != null )
					{
						REQUIRED_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_REQUIRED_MESSAGE_DEFAULT2");
					}
					break;
			}
			if ( sender != null )
				REQUIRED_TYPE.Focus();
		}

		protected void lstNumberOfMenus_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nMenus = Sql.ToInteger(lstNumberOfMenus.SelectedValue);
			for ( int n = 1; n <= 16; n++ )
			{
				Panel pnlMENU = FindControl("pnlMENU" + n.ToString()) as Panel;
				if ( pnlMENU != null )
					pnlMENU.Visible = nMenus >= n;
			}
			if ( sender != null )
				lstNumberOfMenus.Focus();
		}

		protected void lstRatingScale_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nRatings = Sql.ToInteger(lstRatingScale.SelectedValue);
			for ( int n = 1; n <= 16; n++ )
			{
				TableRow trRATING = FindControl("trRATING_" + n.ToString()) as TableRow;
				if ( trRATING != null )
					trRATING.Visible = nRatings >= n;
			}
			if ( sender != null )
				lstRatingScale.Focus();
		}

		protected void OTHER_VALIDATION_TYPE_SelectedIndexChanged(object sender, EventArgs e)
		{
			spnOtherValidation      .Visible = false;
			OTHER_VALIDATION_MIN    .Visible = false;
			OTHER_VALIDATION_MAX    .Visible = false;
			trOtherValidationMessage.Visible = false;
			switch ( OTHER_VALIDATION_TYPE.SelectedValue )
			{
				case ""               :
					break;
				case "Specific Length":
					spnOtherValidation      .Visible = true;
					OTHER_VALIDATION_MIN    .Visible = true;
					OTHER_VALIDATION_MAX    .Visible = true;
					trOtherValidationMessage.Visible = true;
					if ( sender != null )
					{
						OTHER_VALIDATION_MIN    .Text = "0";
						OTHER_VALIDATION_MAX    .Text = "5000";
						OTHER_VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_OTHER_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Integer"        :
					spnOtherValidation      .Visible = true;
					OTHER_VALIDATION_MIN    .Visible = true;
					OTHER_VALIDATION_MAX    .Visible = true;
					trOtherValidationMessage.Visible = true;
					if ( sender != null )
					{
						OTHER_VALIDATION_MIN    .Text = "0";
						OTHER_VALIDATION_MAX    .Text = "100";
						OTHER_VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_OTHER_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Decimal"        :
					spnOtherValidation      .Visible = true;
					OTHER_VALIDATION_MIN    .Visible = true;
					OTHER_VALIDATION_MAX    .Visible = true;
					trOtherValidationMessage.Visible = true;
					if ( sender != null )
					{
						OTHER_VALIDATION_MIN    .Text = "0.0";
						OTHER_VALIDATION_MAX    .Text = "100.0";
						OTHER_VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_OTHER_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Date"           :
					spnOtherValidation      .Visible = true;
					OTHER_VALIDATION_MIN    .Visible = true;
					OTHER_VALIDATION_MAX    .Visible = true;
					trOtherValidationMessage.Visible = true;
					if ( sender != null )
					{
						OTHER_VALIDATION_MIN    .Text = DateTime.Today.ToShortDateString();
						OTHER_VALIDATION_MAX    .Text = DateTime.Today.AddYears(1).ToShortDateString();
						OTHER_VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_OTHER_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Email"          :
					trOtherValidationMessage.Visible = true;
					if ( sender != null )
					{
						OTHER_VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_OTHER_VALIDATION_MESSAGE_DEFAULT");
					}
					break;
			}
			if ( sender != null )
			{
				OTHER_VALIDATION_TYPE.Focus();
			}
		}

		protected void VALIDATION_TYPE_SelectedIndexChanged(object sender, EventArgs e)
		{
			spnValidation     .Visible = false;
			VALIDATION_MIN    .Visible = false;
			VALIDATION_MAX    .Visible = false;
			VALIDATION_MESSAGE.Visible = false;
			switch ( VALIDATION_TYPE.SelectedValue )
			{
				case ""               :
					break;
				case "Specific Length":
					spnValidation     .Visible = true;
					VALIDATION_MIN    .Visible = true;
					VALIDATION_MAX    .Visible = true;
					VALIDATION_MESSAGE.Visible = true;
					if ( sender != null )
					{
						VALIDATION_MIN    .Text = "0";
						VALIDATION_MAX    .Text = "5000";
						VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Integer"        :
					spnValidation     .Visible = true;
					VALIDATION_MIN    .Visible = true;
					VALIDATION_MAX    .Visible = true;
					VALIDATION_MESSAGE.Visible = true;
					if ( sender != null )
					{
						VALIDATION_MIN    .Text = "0";
						VALIDATION_MAX    .Text = "100";
						VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Decimal"        :
					spnValidation     .Visible = true;
					VALIDATION_MIN    .Visible = true;
					VALIDATION_MAX    .Visible = true;
					VALIDATION_MESSAGE.Visible = true;
					if ( sender != null )
					{
						VALIDATION_MIN    .Text = "0.0";
						VALIDATION_MAX    .Text = "100.0";
						VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Date"           :
					spnValidation     .Visible = true;
					VALIDATION_MIN    .Visible = true;
					VALIDATION_MAX    .Visible = true;
					VALIDATION_MESSAGE.Visible = true;
					if ( sender != null )
					{
						VALIDATION_MIN    .Text = DateTime.Today.ToShortDateString();
						VALIDATION_MAX    .Text = DateTime.Today.AddYears(1).ToShortDateString();
						VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_VALIDATION_MESSAGE_DEFAULT2");
					}
					break;
				case "Email"          :
					VALIDATION_MESSAGE.Visible = true;
					if ( sender != null )
					{
						VALIDATION_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_VALIDATION_MESSAGE_DEFAULT");
					}
					break;
			}
			if ( sender != null )
				VALIDATION_TYPE.Focus();
		}

		protected void SIZE_UNITS_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch ( SIZE_UNITS.SelectedValue )
			{
				case "Percent":
					SIZE_WIDTH.DataSource = SplendidCache.List("survey_question_width_percent");
					SIZE_WIDTH.DataBind();
					break;
				case "Fixed"  :
					SIZE_WIDTH.DataSource = SplendidCache.List("survey_question_width_fixed");
					SIZE_WIDTH.DataBind();
					break;
			}
		}

		protected void PLACEMENT_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void NA_ENABLED_CheckedChanged(object sender, EventArgs e)
		{
			if ( NA_ENABLED.Checked )
			{
				if ( sender != null )
					NA_LABEL.Text = L10n.Term("SurveyQuestions.LBL_NA_LABEL_DEFAULT");
			}
		}

		protected void REQUIRED_CheckedChanged(object sender, EventArgs e)
		{
			if ( REQUIRED.Checked )
			{
				if ( sender != null )
					REQUIRED_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_REQUIRED_MESSAGE_DEFAULT");
			}
			tblRequiredType.Visible = REQUIRED.Checked && (QUESTION_TYPE.SelectedValue == "Checkbox" || QUESTION_TYPE.SelectedValue == "Rating Scale" || QUESTION_TYPE.SelectedValue.Contains("Matrix") || QUESTION_TYPE.SelectedValue == "Textbox Multiple" || QUESTION_TYPE.SelectedValue == "Textbox Numerical" || QUESTION_TYPE.SelectedValue == "Date");
			divRequired.DataBind();
		}

		protected void VALIDATION_SUM_ENABLED_CheckedChanged(object sender, EventArgs e)
		{
			if ( VALIDATION_SUM_ENABLED.Checked )
			{
				if ( sender != null )
					VALIDATION_SUM_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_VALIDATION_SUM_MESSAGE_DEFAULT");
			}
			divValidationSum.DataBind();
		}

		protected void OTHER_ENABLED_CheckedChanged(object sender, EventArgs e)
		{
			if ( OTHER_ENABLED.Checked )
			{
				if ( sender != null )
				{
					OTHER_LABEL.Text = L10n.Term("SurveyQuestions.LBL_OTHER_LABEL_DEFAULT");
				}
			}
		}

		protected void OTHER_AS_CHOICE_CheckedChanged(object sender, EventArgs e)
		{
			trOtherRequiredMessage.Visible = OTHER_AS_CHOICE.Checked;
			if ( OTHER_AS_CHOICE.Checked )
			{
				if ( sender != null )
				{
					OTHER_REQUIRED_MESSAGE.Text = L10n.Term("SurveyQuestions.LBL_OTHER_REQUIRED_MESSAGE_DEFAULT");
				}
			}
		}

		// 09/30/2018 Paul.  Add survey record creation to survey. 
		protected void SURVEY_TARGET_MODULE_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataTable dtDemographicNames = SplendidCache.List("survey_question_demographic_fields");
			if ( !Sql.IsEmptyString(SURVEY_TARGET_MODULE.SelectedValue) && SURVEY_TARGET_MODULE.Visible )
			{
				string sSURVEY_TARGET_MODULE = SURVEY_TARGET_MODULE.SelectedValue;
				DataTable dtColumns = SplendidCache.ImportColumns(sSURVEY_TARGET_MODULE).Copy();
				foreach(DataRow row in dtColumns.Rows)
				{
					row["DISPLAY_NAME"] = Utils.TableColumnName(L10n, sSURVEY_TARGET_MODULE, Sql.ToString(row["DISPLAY_NAME"]));
				}
				DataView vwColumns = new DataView(dtColumns);
				vwColumns.Sort = "DISPLAY_NAME";
				// 04/25/2019 Paul.  We want to allow Guids so that custom fields can be set. 
				vwColumns.RowFilter = "NAME not in ('DATE_ENTERED', 'DATE_MODIFIED', 'EXCHANGE_FOLDER', 'INVALID_EMAIL', 'LEAD_NUMBER', 'PROSPECT_NUMBER', 'CONTACT_NUMBER', 'ACCOUNT_NUMBER', 'PICTURE', 'ASSIGNED_SET_LIST', 'TEAM_SET_LIST')";
				TARGET_FIELD_LABEL.Visible = (QUESTION_TYPE.SelectedValue != "Demographic");
				TARGET_FIELD_NAME .Visible = (QUESTION_TYPE.SelectedValue != "Demographic");
				TARGET_FIELD_NAME.Items.Clear();
				TARGET_FIELD_NAME.DataSource = vwColumns;
				TARGET_FIELD_NAME.DataBind();
				TARGET_FIELD_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				for ( int n = 0; n < dtDemographicNames.Rows.Count; n++ )
				{
					string sNAME = Sql.ToString(dtDemographicNames.Rows[n]["NAME"]);
					DropDownList lstTargetField = this.FindControl("DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_NAME") as DropDownList;
					if ( lstTargetField != null )
					{
						lstTargetField.Visible = true;
						lstTargetField.Items.Clear();
						lstTargetField.DataSource = vwColumns;
						lstTargetField.DataBind();
						lstTargetField.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					}
					Label lblTargetField = this.FindControl("DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_LABEL") as Label;
					if ( lblTargetField != null )
					{
						lblTargetField.Visible = true;
					}
				}
			}
			else
			{
				TARGET_FIELD_LABEL.Visible = false;
				TARGET_FIELD_NAME .Visible = false;
				TARGET_FIELD_NAME.Items.Clear();
				for ( int n = 0; n < dtDemographicNames.Rows.Count; n++ )
				{
					string sNAME = Sql.ToString(dtDemographicNames.Rows[n]["NAME"]);
					DropDownList lstTargetField = this.FindControl("DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_NAME") as DropDownList;
					if ( lstTargetField != null )
					{
						lstTargetField.Visible = false;
						lstTargetField.Items.Clear();
					}
					Label lblTargetField = this.FindControl("DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_LABEL") as Label;
					if ( lblTargetField != null )
					{
						lblTargetField.Visible = false;
					}
				}
			}
		}
		#endregion

		#region Build
		protected void BuildMenuChoices()
		{
			TableRow trMENU = null;
			DataTable dtMenuChoices = SplendidCache.List("survey_question_menu_choices");
			for ( int n = 1; n <= dtMenuChoices.Rows.Count; n++ )
			{
				if ( n == 1 || n == 4 || n == 7 )
				{
					trMENU = new TableRow();
					tblMENU.Rows.Add(trMENU);
				}
				TableCell tdMENU = new TableCell();
				tdMENU.Width = new Unit("33%");
				trMENU.Cells.Add(tdMENU);
				Panel pnlMENU = new Panel();
				pnlMENU.ID = "pnlMENU" + n.ToString();
				tdMENU.Controls.Add(pnlMENU);
				
				HtmlGenericControl div = new HtmlGenericControl("div");
				div.Attributes.Add("style", "margin-top: 2px; margin-bottom: 2px;");
				pnlMENU.Controls.Add(div);
				Label lblHeading = new Label();
				lblHeading.Text = String.Format(L10n.Term("SurveyQuestions.LBL_MENU_HEADING"), n);
				div.Controls.Add(lblHeading);
				TextBox MENU_HEADING = new TextBox();
				MENU_HEADING.ID = "MENU_HEADING_" + n.ToString();
				MENU_HEADING.Width = new Unit("95%");
				pnlMENU.Controls.Add(MENU_HEADING);
				
				div = new HtmlGenericControl("div");
				div.Attributes.Add("style", "margin-top: 2px; margin-bottom: 2px;");
				pnlMENU.Controls.Add(div);
				lblHeading = new Label();
				lblHeading.Text = String.Format(L10n.Term("SurveyQuestions.LBL_MENU_OPTIONS"), n);
				div.Controls.Add(lblHeading);
				TextBox MENU_OPTIONS = new TextBox();
				MENU_OPTIONS.ID       = "MENU_OPTIONS_" + n.ToString();
				MENU_OPTIONS.TextMode = TextBoxMode.MultiLine;
				MENU_OPTIONS.Rows     = 3;
				MENU_OPTIONS.Width    = new Unit("95%");
				pnlMENU.Controls.Add(MENU_OPTIONS);
			}
		}

		protected void BuildDemographicInformation()
		{
			DataTable dtDemographicNames = SplendidCache.List("survey_question_demographic_fields");
			for ( int n = 0; n < dtDemographicNames.Rows.Count; n++ )
			{
				string sNAME         = Sql.ToString(dtDemographicNames.Rows[n]["NAME"        ]);
				string sDISPLAY_NAME = Sql.ToString(dtDemographicNames.Rows[n]["DISPLAY_NAME"]);
				TableRow tr = new TableRow();
				tblDEMOGRAPHIC.Rows.Add(tr);
				TableCell tdLABEL      = new TableCell();
				TableCell tdTEXT       = new TableCell();
				TableCell tdVISIBLE    = new TableCell();
				TableCell tdREQUIRED   = new TableCell();
				// 09/30/2018 Paul.  Add survey record creation to survey. 
				TableCell tdFIELD_NAME = new TableCell();
				TableCell tdSPACER     = new TableCell();
				tr.Cells.Add(tdLABEL     );
				tr.Cells.Add(tdTEXT      );
				tr.Cells.Add(tdVISIBLE   );
				tr.Cells.Add(tdREQUIRED  );
				tr.Cells.Add(tdFIELD_NAME);
				tr.Cells.Add(tdSPACER    );
				tdLABEL     .CssClass = "dataLabel";
				tdTEXT      .CssClass = "dataField";
				tdVISIBLE   .CssClass = "dataField";
				tdREQUIRED  .CssClass = "dataField";
				tdFIELD_NAME.CssClass = "dataField";
				tdSPACER    .CssClass = "dataField";
				tdLABEL     .Attributes.Add("style", "width: 15%; white-space: nowrap;");
				tdTEXT      .Attributes.Add("style", "width: 25%; white-space: nowrap;");
				tdVISIBLE   .Attributes.Add("style", "width: 10%; white-space: nowrap;");
				tdREQUIRED  .Attributes.Add("style", "width: 10%; white-space: nowrap;");
				tdFIELD_NAME.Attributes.Add("style", "width: 30%; white-space: nowrap;");
				tdSPACER    .Attributes.Add("style", "width: 10%;");
				tdLABEL.Text = sDISPLAY_NAME;
				TextBox  txtField    = new TextBox();
				CheckBox chkVisible  = new CheckBox();
				CheckBox chkRequired = new CheckBox();
				txtField   .ID       = "DEMOGRAPHIC_" + sNAME;
				chkVisible .ID       = "DEMOGRAPHIC_" + sNAME + "_VISIBLE" ;
				chkRequired.ID       = "DEMOGRAPHIC_" + sNAME + "_REQUIRED";
				chkVisible .CssClass = "checkbox";
				chkRequired.CssClass = "checkbox";
				chkVisible .Text     = L10n.Term("SurveyQuestions.LBL_DEMOGRAPHIC_VISIBLE" );
				chkRequired.Text     = L10n.Term("SurveyQuestions.LBL_DEMOGRAPHIC_REQUIRED");
				tdTEXT    .Controls.Add(txtField   );
				tdVISIBLE .Controls.Add(chkVisible );
				tdREQUIRED.Controls.Add(chkRequired);
				if ( !IsPostBack )
				{
					txtField.Text = sDISPLAY_NAME;
					chkVisible.Checked = true;
				}
				// 09/30/2018 Paul.  Add survey record creation to survey. 
				Label        lblTargetField = new Label();
				DropDownList lstTargetField = new DropDownList();
				tdFIELD_NAME.Controls.Add(lblTargetField);
				tdFIELD_NAME.Controls.Add(lstTargetField);
				lblTargetField.ID = "DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_LABEL";
				lstTargetField.ID = "DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_NAME" ;
				lblTargetField.CssClass = "dataLabel";
				lblTargetField.Attributes.Add("style", "padding-right: 5px;");
				lblTargetField.Text           = L10n.Term("SurveyQuestions.LBL_TARGET_FIELD_NAME");
				lstTargetField.CssClass       = "dataField";
				lstTargetField.DataValueField = "NAME";
				lstTargetField.DataTextField  = "DISPLAY_NAME";

			}
		}

		protected void BuildRatingScaleTable()
		{
			DataTable dtRatingScales = SplendidCache.List("survey_question_ratings_scale");
			for ( int n = 1; n <= dtRatingScales.Rows.Count; n++ )
			{
				TableRow trRATING = new TableRow();
				trRATING.ID = "trRATING_" + n.ToString();
				tblRATING.Rows.Add(trRATING);
				TableCell tdLABEL_LABEL  = new TableCell();
				TableCell tdLABEL_TEXT   = new TableCell();
				TableCell tdWEIGHT_LABEL = new TableCell();
				TableCell tdWEIGHT_TEXT  = new TableCell();
				trRATING.Cells.Add(tdLABEL_LABEL );
				trRATING.Cells.Add(tdLABEL_TEXT  );
				trRATING.Cells.Add(tdWEIGHT_LABEL);
				trRATING.Cells.Add(tdWEIGHT_TEXT );
				tdLABEL_LABEL .CssClass = "dataLabel";
				tdLABEL_TEXT  .CssClass = "dataField";
				tdWEIGHT_LABEL.CssClass = "dataLabel";
				tdWEIGHT_TEXT .CssClass = "dataField";
				tdLABEL_LABEL .Text = L10n.Term("SurveyQuestions.LBL_RATING_SCALE_LABEL");
				tdWEIGHT_LABEL.Text = L10n.Term("SurveyQuestions.LBL_RATING_SCALE_WEIGHT");
				TextBox RATING_LABEL  = new TextBox();
				TextBox RATING_WEIGHT = new TextBox();
				RATING_LABEL .ID = "RATING_LABEL_"  + n.ToString();
				RATING_WEIGHT.ID = "RATING_WEIGHT_" + n.ToString();
				RATING_LABEL .Attributes.Add("style", "width: 200px;");
				RATING_WEIGHT.Attributes.Add("style", "width: 40px;");
				tdLABEL_TEXT .Controls.Add(RATING_LABEL );
				tdWEIGHT_TEXT.Controls.Add(RATING_WEIGHT);
				if ( !IsPostBack )
				{
					RATING_WEIGHT.Text = n.ToString();
				}
			}
		}
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;
			
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					// 05/28/2013 Paul.  Terms need to be binded manually. 
					RANDOMIZE_NOT_LAST      .DataBind();
					radIMAGE_UPLOAD         .DataBind();
					radIMAGE_URL            .DataBind();
					OTHER_AS_CHOICE         .DataBind();
					OTHER_ONE_PER_ROW       .DataBind();
					NA_ENABLED              .DataBind();
					REQUIRED                .DataBind();
					VALIDATION_SUM_ENABLED  .DataBind();
					
					QUESTION_TYPE           .DataSource = SplendidCache.List("survey_question_type");
					QUESTION_TYPE           .DataBind();
					// 09/30/2018 Paul.  Add survey record creation to survey. 
					SURVEY_TARGET_MODULE    .DataSource = SplendidCache.List("survey_target_module_dom");
					SURVEY_TARGET_MODULE    .DataBind();
					SURVEY_TARGET_MODULE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

					OTHER_HEIGHT            .DataSource = SplendidCache.List("survey_question_field_lines");
					OTHER_HEIGHT            .DataBind();
					OTHER_WIDTH             .DataSource = SplendidCache.List("survey_question_field_chars");
					OTHER_WIDTH             .DataBind();
					OTHER_VALIDATION_TYPE   .DataSource = SplendidCache.List("survey_question_validation");
					OTHER_VALIDATION_TYPE   .DataBind();
					VALIDATION_TYPE         .DataSource = SplendidCache.List("survey_question_validation");
					VALIDATION_TYPE         .DataBind();
					REQUIRED_TYPE           .DataSource = SplendidCache.List("survey_question_required_rows");
					REQUIRED_TYPE           .DataBind();
					REQUIRED_RESPONSES_RANGE.DataBind();
					DataTable dtRANDOMIZE_TYPE = SplendidCache.List("survey_answer_randomization").Copy();
					DataRow rowRANDOMIZE_TYPE = dtRANDOMIZE_TYPE.NewRow();
					rowRANDOMIZE_TYPE["NAME"        ] = String.Empty;
					rowRANDOMIZE_TYPE["DISPLAY_NAME"] = L10n.Term("Surveys.LBL_NOT_RANDOMIZED");
					dtRANDOMIZE_TYPE.Rows.InsertAt(rowRANDOMIZE_TYPE, 0);
					RANDOMIZE_TYPE          .DataSource = dtRANDOMIZE_TYPE;
					RANDOMIZE_TYPE          .DataBind();
					SIZE_UNITS              .DataSource = SplendidCache.List("survey_question_width_units");
					SIZE_UNITS              .DataBind();
					SIZE_HEIGHT             .DataSource = SplendidCache.List("survey_question_field_lines");
					SIZE_HEIGHT             .DataBind();
					BOX_WIDTH               .DataSource = SplendidCache.List("survey_question_field_chars");
					BOX_WIDTH               .DataBind();
					BOX_HEIGHT              .DataSource = SplendidCache.List("survey_question_field_lines");
					BOX_HEIGHT              .DataBind();
					COLUMN_WIDTH            .DataSource = SplendidCache.List("survey_question_columns_width");
					COLUMN_WIDTH            .DataBind();
					PLACEMENT               .DataSource = SplendidCache.List("survey_question_placement");
					PLACEMENT               .DataBind();
					lstNumberOfMenus        .DataSource = SplendidCache.List("survey_question_menu_choices");
					lstNumberOfMenus        .DataBind();
					lstRatingScale          .DataSource = SplendidCache.List("survey_question_ratings_scale");
					lstRatingScale          .DataBind();
					
					QUESTION_TYPE_SelectedIndexChanged(null, null);
					REQUIRED_TYPE_SelectedIndexChanged(null, null);
					SIZE_UNITS_SelectedIndexChanged(null, null);
					lstNumberOfMenus_SelectedIndexChanged(null, null);
					lstRatingScale_SelectedIndexChanged(null, null);
					OTHER_VALIDATION_TYPE_SelectedIndexChanged(null, null);
					OTHER_AS_CHOICE_CheckedChanged(null, null);
					VALIDATION_TYPE_SelectedIndexChanged(null, null);
					
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
							sSQL = "select *"               + ControlChars.CrLf
							     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
							     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "edit");
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AppendParameter(cmd, gDuplicateID, "ID", false);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AppendParameter(cmd, gID, "ID", false);
								}
								con.Open();
								
								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										// 10/31/2017 Paul.  Provide a way to inject Record level ACL. 
										if ( dtCurrent.Rows.Count > 0 && (SplendidCRM.Security.GetRecordAccess(dtCurrent.Rows[0], m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0) )
										{
											DataRow rdr = dtCurrent.Rows[0];
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString  (rdr["DESCRIPTION"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											this.AppendEditViewRelationships(m_sMODULE + "." + LayoutEditView, plcSubPanel, Sql.IsEmptyGuid(Request["ID"]));
											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											BuildRatingScaleTable();
											BuildMenuChoices();
											BuildDemographicInformation();

											string sQUESTION_TYPE  = Sql.ToString (rdr["QUESTION_TYPE" ]);
											NAME                       .Text            = Sql.ToString (rdr["NAME"                       ]);
											DESCRIPTION                .Text            = Sql.ToString (rdr["DESCRIPTION"                ]);
											Utils.SetSelectedValue(QUESTION_TYPE        , Sql.ToString (rdr["QUESTION_TYPE"              ]));
											QUESTION_TYPE_SelectedIndexChanged(null, null);
											Utils.SetSelectedValue(DISPLAY_FORMAT       , Sql.ToString (rdr["DISPLAY_FORMAT"             ]));
											// 09/30/2018 Paul.  Add survey record creation to survey. 
											Utils.SetSelectedValue(SURVEY_TARGET_MODULE , Sql.ToString (rdr["SURVEY_TARGET_MODULE"       ]));
											SURVEY_TARGET_MODULE_SelectedIndexChanged(null, null);
											Utils.SetSelectedValue(TARGET_FIELD_NAME    , Sql.ToString (rdr["TARGET_FIELD_NAME"          ]));

											ANSWER_CHOICES             .Text            = Sql.ToString (rdr["ANSWER_CHOICES"             ]);
											COLUMN_CHOICES             .Text            = Sql.ToString (rdr["COLUMN_CHOICES"             ]);
											FORCED_RANKING             .Checked         = Sql.ToBoolean(rdr["FORCED_RANKING"             ]);
											INVALID_DATE_MESSAGE       .Text            = Sql.ToString (rdr["INVALID_DATE_MESSAGE"       ]);
											INVALID_NUMBER_MESSAGE     .Text            = Sql.ToString (rdr["INVALID_NUMBER_MESSAGE"     ]);
											NA_ENABLED                 .Checked         = Sql.ToBoolean(rdr["NA_ENABLED"                 ]);
											NA_LABEL                   .Text            = Sql.ToString (rdr["NA_LABEL"                   ]);
											OTHER_ENABLED              .Checked         = Sql.ToBoolean(rdr["OTHER_ENABLED"              ]);
											OTHER_LABEL                .Text            = Sql.ToString (rdr["OTHER_LABEL"                ]);
											Utils.SetSelectedValue(OTHER_HEIGHT         , Sql.ToString (rdr["OTHER_HEIGHT"               ]));
											Utils.SetSelectedValue(OTHER_WIDTH          , Sql.ToString (rdr["OTHER_WIDTH"                ]));
											OTHER_AS_CHOICE            .Checked         = Sql.ToBoolean(rdr["OTHER_AS_CHOICE"            ]);
											OTHER_AS_CHOICE_CheckedChanged(null, null);
											OTHER_ONE_PER_ROW          .Checked         = Sql.ToBoolean(rdr["OTHER_ONE_PER_ROW"          ]);
											OTHER_REQUIRED_MESSAGE     .Text            = Sql.ToString (rdr["OTHER_REQUIRED_MESSAGE"     ]);
											Utils.SetSelectedValue(OTHER_VALIDATION_TYPE, Sql.ToString (rdr["OTHER_VALIDATION_TYPE"      ]));
											OTHER_VALIDATION_TYPE_SelectedIndexChanged(null, null);
											OTHER_VALIDATION_MIN       .Text            = Sql.ToString (rdr["OTHER_VALIDATION_MIN"       ]);
											OTHER_VALIDATION_MAX       .Text            = Sql.ToString (rdr["OTHER_VALIDATION_MAX"       ]);
											OTHER_VALIDATION_MESSAGE   .Text            = Sql.ToString (rdr["OTHER_VALIDATION_MESSAGE"   ]);
											REQUIRED                   .Checked         = Sql.ToBoolean(rdr["REQUIRED"                   ]);
											Utils.SetSelectedValue(REQUIRED_TYPE        , Sql.ToString (rdr["REQUIRED_TYPE"              ]));
											REQUIRED_TYPE_SelectedIndexChanged(null, null);
											// 08/17/2018 Paul.  For date validation, we need to store time in seconds as the database field is an integer.  Convert to seconds since 1970. 
											if ( sQUESTION_TYPE == "Date" )
											{
												int nREQUIRED_RESPONSES_MIN = Sql.ToInteger(rdr["REQUIRED_RESPONSES_MIN"     ]);
												int nREQUIRED_RESPONSES_MAX = Sql.ToInteger(rdr["REQUIRED_RESPONSES_MAX"     ]);
												if ( nREQUIRED_RESPONSES_MIN > 0 )
												{
													DateTime dtREQUIRED_RESPONSES_MIN = new DateTime(1970, 1, 1).AddSeconds(nREQUIRED_RESPONSES_MIN);
													REQUIRED_RESPONSES_MIN.Text = dtREQUIRED_RESPONSES_MIN.ToShortDateString();
												}
												if ( nREQUIRED_RESPONSES_MAX > 0 )
												{
													DateTime dtREQUIRED_RESPONSES_MAX = new DateTime(1970, 1, 1).AddSeconds(nREQUIRED_RESPONSES_MAX);
													REQUIRED_RESPONSES_MAX.Text = dtREQUIRED_RESPONSES_MAX.ToShortDateString();
												}
											}
											else
											{
												REQUIRED_RESPONSES_MIN     .Text            = Sql.ToString (rdr["REQUIRED_RESPONSES_MIN"     ]);
												REQUIRED_RESPONSES_MAX     .Text            = Sql.ToString (rdr["REQUIRED_RESPONSES_MAX"     ]);
											}
											REQUIRED_CheckedChanged(null, null);
											REQUIRED_MESSAGE           .Text            = Sql.ToString (rdr["REQUIRED_MESSAGE"           ]);
											Utils.SetSelectedValue(VALIDATION_TYPE      , Sql.ToString (rdr["VALIDATION_TYPE"            ]));
											VALIDATION_TYPE_SelectedIndexChanged(null, null);
											VALIDATION_MIN             .Text            = Sql.ToString (rdr["VALIDATION_MIN"             ]);
											VALIDATION_MAX             .Text            = Sql.ToString (rdr["VALIDATION_MAX"             ]);
											VALIDATION_MESSAGE         .Text            = Sql.ToString (rdr["VALIDATION_MESSAGE"         ]);
											VALIDATION_SUM_ENABLED     .Checked         = Sql.ToBoolean(rdr["VALIDATION_SUM_ENABLED"     ]);
											if ( VALIDATION_SUM_ENABLED .Checked )
												VALIDATION_NUMERIC_SUM.Text = Sql.ToString (rdr["VALIDATION_NUMERIC_SUM"]);
											VALIDATION_SUM_MESSAGE     .Text            = Sql.ToString (rdr["VALIDATION_SUM_MESSAGE"     ]);
											Utils.SetValue(RANDOMIZE_TYPE, Sql.ToString (rdr["RANDOMIZE_TYPE"]));
											RANDOMIZE_NOT_LAST         .Checked         = Sql.ToBoolean(rdr["RANDOMIZE_NOT_LAST"         ]);
											string sSIZE_WIDTH = Sql.ToString (rdr["SIZE_WIDTH"]);
											if ( sSIZE_WIDTH.Contains("%") )
											{
												Utils.SetSelectedValue(SIZE_UNITS, "Percent");
											}
											else
											{
												Utils.SetSelectedValue(SIZE_UNITS, "Fixed");
											}
											SIZE_UNITS_SelectedIndexChanged(null, null);
											Utils.SetSelectedValue(SIZE_WIDTH           , Sql.ToString (rdr["SIZE_WIDTH"                 ]));
											Utils.SetSelectedValue(SIZE_HEIGHT          , Sql.ToString (rdr["SIZE_HEIGHT"                ]));
											Utils.SetSelectedValue(BOX_WIDTH            , Sql.ToString (rdr["BOX_WIDTH"                  ]));
											Utils.SetSelectedValue(BOX_HEIGHT           , Sql.ToString (rdr["BOX_HEIGHT"                 ]));
											Utils.SetSelectedValue(COLUMN_WIDTH         , Sql.ToString (rdr["COLUMN_WIDTH"               ]));
											Utils.SetSelectedValue(PLACEMENT            , Sql.ToString (rdr["PLACEMENT"                  ]));
											SPACING_LEFT               .Text            = Sql.ToString (rdr["SPACING_LEFT"               ]);
											SPACING_TOP                .Text            = Sql.ToString (rdr["SPACING_TOP"                ]);
											SPACING_RIGHT              .Text            = Sql.ToString (rdr["SPACING_RIGHT"              ]);
											SPACING_BOTTOM             .Text            = Sql.ToString (rdr["SPACING_BOTTOM"             ]);
											IMAGE_URL                  .Text            = Sql.ToString (rdr["IMAGE_URL"                  ]);
											// 01/01/2016 Paul.  Add categories. 
											CATEGORIES                 .Text            = Sql.ToString (rdr["CATEGORIES"                 ]);
											if ( !Sql.IsEmptyString(IMAGE_URL.Text) )
												radIMAGE_URL.Checked = true;
											
											try
											{
												string sANSWER_CHOICES = Sql.ToString (rdr["ANSWER_CHOICES"]);
												string sCOLUMN_CHOICES = Sql.ToString (rdr["COLUMN_CHOICES"]);
												if ( sQUESTION_TYPE == "Rating Scale" )
												{
													COLUMN_CHOICES.Text = String.Empty;
													XmlDocument xml = new XmlDocument();
													// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
													// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
													// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
													xml.XmlResolver = null;
													xml.LoadXml(sCOLUMN_CHOICES);
													XmlNodeList nl = xml.DocumentElement.SelectNodes("Rating");
													int nRATINGS_SCALE = nl.Count;
													Utils.SetSelectedValue(lstRatingScale, nRATINGS_SCALE.ToString());
													for ( int n = 1; n <= nRATINGS_SCALE; n++ )
													{
														XmlNode xRating = nl[n - 1];
														XmlNode xLabel  = xRating.SelectSingleNode("Label" );
														XmlNode xWeight = xRating.SelectSingleNode("Weight");
														new DynamicControl(this, "RATING_LABEL_"  + n.ToString()).Text = xLabel .InnerText;
														new DynamicControl(this, "RATING_WEIGHT_" + n.ToString()).Text = xWeight.InnerText;
													}
												}
												else if ( sQUESTION_TYPE == "Dropdown Matrix" )
												{
													COLUMN_CHOICES.Text = String.Empty;
													XmlDocument xml = new XmlDocument();
													// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
													// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
													// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
													xml.XmlResolver = null;
													xml.LoadXml(sCOLUMN_CHOICES);
													XmlNodeList nl = xml.DocumentElement.SelectNodes("Menu");
													int nMENU_ITEMS = nl.Count;
													Utils.SetSelectedValue(lstNumberOfMenus, nMENU_ITEMS.ToString());
													for ( int n = 1; n <= nMENU_ITEMS; n++ )
													{
														XmlNode xMenu    = nl[n - 1];
														XmlNode xHeading = xMenu.SelectSingleNode("Heading");
														XmlNode xOptions = xMenu.SelectSingleNode("Options");
														new DynamicControl(this, "MENU_HEADING_" + n.ToString()).Text = xHeading.InnerText;
														new DynamicControl(this, "MENU_OPTIONS_" + n.ToString()).Text = xOptions.InnerText;
													}
												}
												else if ( sQUESTION_TYPE == "Demographic" )
												{
													COLUMN_CHOICES.Text = String.Empty;
													XmlDocument xml = new XmlDocument();
													// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
													// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
													// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
													xml.XmlResolver = null;
													xml.LoadXml(sCOLUMN_CHOICES);
													XmlNodeList nl = xml.DocumentElement.SelectNodes("Field");
													for ( int n = 0; n < nl.Count; n++ )
													{
														XmlNode xField = nl[n];
														string sNAME = XmlUtil.SelectAttribute(xField, "Name");
														new DynamicControl(this, "DEMOGRAPHIC_" + sNAME + "_VISIBLE"          ).Checked       = Sql.ToBoolean(XmlUtil.SelectAttribute(xField, "Visible" ));
														new DynamicControl(this, "DEMOGRAPHIC_" + sNAME + "_REQUIRED"         ).Checked       = Sql.ToBoolean(XmlUtil.SelectAttribute(xField, "Required"));
														new DynamicControl(this, "DEMOGRAPHIC_" + sNAME                       ).Text          = xField.InnerText;
														// 09/30/2018 Paul.  Add survey record creation to survey. 
														if ( !Sql.IsEmptyString(SURVEY_TARGET_MODULE.SelectedValue) )
														{
															new DynamicControl(this, "DEMOGRAPHIC_" + sNAME + "_TARGET_FIELD_NAME").SelectedValue = Sql.ToString(XmlUtil.SelectAttribute(xField, "TargetField"));
														}
													}
												}
												// 10/08/2014 Paul.  Add Range question type. 
												else if ( sQUESTION_TYPE == "Range" )
												{
													string[] arrANSWER_CHOICES = sANSWER_CHOICES.Split(new String[] { ControlChars.CrLf }, StringSplitOptions.None);
													RANGE_MIN.Text = arrANSWER_CHOICES[0];
													if ( arrANSWER_CHOICES.Length > 0 )
														RANGE_MAX.Text = arrANSWER_CHOICES[1];
													if ( arrANSWER_CHOICES.Length > 1 )
														RANGE_STEP.Text = arrANSWER_CHOICES[2];
												}
												string sIMAGE_URL = Sql.ToString(rdr["IMAGE_URL"]);
												imgIMAGE.ImageUrl = sIMAGE_URL;
												imgIMAGE.Visible  = !Sql.IsEmptyString(sIMAGE_URL);
											}
											catch
											{
											}
											lstNumberOfMenus_SelectedIndexChanged(null, null);
											lstRatingScale_SelectedIndexChanged(null, null);
											
											if ( tblRankingNA.Visible && NA_LABEL.Text != L10n.Term("SurveyQuestions.LBL_NA_LABEL_DEFAULT") )
												NA_LABEL.Visible = true;
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											TextBox txtNAME = this.FindControl("NAME") as TextBox;
											if ( txtNAME != null )
												txtNAME.Focus();
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											
											ViewState ["NAME"            ] = Sql.ToString(rdr["NAME"            ]);
											ViewState ["ASSIGNED_USER_ID"] = Sql.ToGuid  (rdr["ASSIGNED_USER_ID"]);
											Page.Items["NAME"            ] = ViewState ["NAME"            ];
											Page.Items["ASSIGNED_USER_ID"] = ViewState ["ASSIGNED_USER_ID"];
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
										}
										else
										{
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlFooterButtons .DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
											plcSubPanel.Visible = false;
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewRelationships(m_sMODULE + "." + LayoutEditView, plcSubPanel, Sql.IsEmptyGuid(Request["ID"]));
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
						BuildRatingScaleTable();
						BuildMenuChoices();
						BuildDemographicInformation();
						RANDOMIZE_TYPE.SelectedIndex = 0;
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						DESCRIPTION.Focus();
						this.ApplyEditViewNewEventRules(m_sMODULE + "." + LayoutEditView);
					}
				}
				else
				{
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
					Page.Items["NAME"            ] = ViewState ["NAME"            ];
					Page.Items["ASSIGNED_USER_ID"] = ViewState ["ASSIGNED_USER_ID"];
				}
				
				// 10/07/2018 Paul.  Register all the Survey JavaScript files. 
				SurveyUtil.RegisterScripts(this.Page);
				
				Guid gSURVEY_THEME_ID = Sql.ToGuid(Application["CONFIG.Surveys.DefaultTheme"]);
				// 10/07/2018 Paul.  The stylesheet needs to be loaded separately. 
				HtmlLink cssSurveyStylesheet = new HtmlLink();
				cssSurveyStylesheet.Attributes.Add("id"   , gSURVEY_THEME_ID.ToString().Replace("-", "_"));
				cssSurveyStylesheet.Attributes.Add("href" , "~/Surveys/stylesheet.aspx?ID=" + gSURVEY_THEME_ID.ToString());
				cssSurveyStylesheet.Attributes.Add("type" , "text/css"  );
				cssSurveyStylesheet.Attributes.Add("rel"  , "stylesheet");
				Page.Header.Controls.Add(cssSurveyStylesheet);
				
				// 10/07/2018 Paul.  Use selectize.js to enhance the dropdown. 
				// https://github.com/selectize/selectize.js
				AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page) as AjaxControlToolkit.ToolkitScriptManager;
				Sql.AddScriptReference(mgrAjax, "~/Include/javascript/selectize.min.js");
				Sql.AddStyleSheet(this.Page, "~/Include/javascript/selectize.css");
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "SurveyQuestions";
			// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
			m_sVIEW_NAME = "vw" + Crm.Modules.TableName(m_sMODULE) + "_Edit";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendEditViewRelationships(m_sMODULE + "." + LayoutEditView, plcSubPanel, Sql.IsEmptyGuid(Request["ID"]));
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				BuildRatingScaleTable();
				BuildMenuChoices();
				BuildDemographicInformation();
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}

