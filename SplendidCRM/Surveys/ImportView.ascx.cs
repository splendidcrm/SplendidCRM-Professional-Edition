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
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Surveys
{
	/// <summary>
	///		Summary description for ImportView.
	/// </summary>
	public class ImportView : SplendidControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons       ;
		protected _controls.TeamSelect     ctlTeamSelect           ;

		protected TextBox                  txtNAME                 ;
		protected TextBox                  txtASSIGNED_TO          ;
		protected HiddenField              txtASSIGNED_USER_ID     ;
		protected HtmlInputFile            fileIMPORT              ;
		protected RequiredFieldValidator   reqNAME                 ;
		protected RequiredFieldValidator   reqFILENAME             ;
		protected TextBox                  TEAM_NAME               ;
		protected HiddenField              TEAM_ID                 ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Import" )
				{
					txtNAME.Text = txtNAME.Text.Trim();
					if ( txtNAME.Text.ToLower().EndsWith(".xml") )
						txtNAME.Text = txtNAME.Text.Substring(0, txtNAME.Text.Length - 4);
					
					reqNAME.Enabled = true;
					reqNAME.Validate();
					reqFILENAME.Enabled = true;
					reqFILENAME.Validate();
					if ( Page.IsValid )
					{
						Guid gSURVEY_ID = Guid.Empty;
						HttpPostedFile pstIMPORT = fileIMPORT.PostedFile;
						if ( pstIMPORT != null )
						{
							if ( pstIMPORT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstIMPORT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstIMPORT.ContentType;
								
								XmlDocument xmlImport = new XmlDocument();
								// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
								// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
								// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
								xmlImport.XmlResolver = null;
								xmlImport.Load(pstIMPORT.InputStream);
								
								string sNAME                 = txtNAME.Text;
								Guid   gTEAM_ID              = Guid.Empty;
								Guid   gASSIGNED_USER_ID     = Sql.ToGuid(txtASSIGNED_USER_ID.Value);
								bool   bEnableTeamManagement = Crm.Config.enable_team_management();
								if ( SplendidCRM.Crm.Config.enable_dynamic_teams() )
									gTEAM_ID = ctlTeamSelect.TEAM_ID;
								else
									gTEAM_ID = Sql.ToGuid(TEAM_ID.Value);
								
								XmlNodeList nlSurveys = xmlImport.DocumentElement.SelectNodes("Surveys".ToLower());
								// 05/23/2020 Paul.  Try non-lower case. 
								if ( nlSurveys.Count == 0 )
									nlSurveys = xmlImport.DocumentElement.SelectNodes("Surveys");
								if ( nlSurveys.Count == 0 )
									throw(new Exception(L10n.Term("Import.LBL_NOTHING")));
								
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									IDbCommand cmdSURVEYS                = SqlProcs.Factory(con, "spSURVEYS_Update"               );
									IDbCommand cmdSURVEY_THEMES          = SqlProcs.Factory(con, "spSURVEY_THEMES_Update"         );
									IDbCommand cmdSURVEY_QUESTIONS       = SqlProcs.Factory(con, "spSURVEY_QUESTIONS_Update"      );
									IDbCommand cmdSURVEY_PAGES           = SqlProcs.Factory(con, "spSURVEY_PAGES_Update"          );
									IDbCommand cmdSURVEY_PAGES_QUESTIONS = SqlProcs.Factory(con, "spSURVEY_PAGES_QUESTIONS_Update");
									// 11/12/2018 Paul.  Export images that are not URLs. 
									IDbCommand cmdEMAIL_IMAGES           = SqlProcs.Factory(con, "spEMAIL_IMAGES_Update"          );
									
									using ( IDbTransaction trn = Sql.BeginTransaction(con) )
									{
										cmdSURVEYS               .Transaction = trn;
										cmdSURVEY_THEMES         .Transaction = trn;
										cmdSURVEY_QUESTIONS      .Transaction = trn;
										cmdSURVEY_PAGES          .Transaction = trn;
										cmdSURVEY_PAGES_QUESTIONS.Transaction = trn;
										cmdEMAIL_IMAGES          .Transaction = trn;
										try
										{
											Dictionary<Guid, Guid> dictSurveys = new Dictionary<Guid, Guid>();
											for ( int i = 0; i < nlSurveys.Count; i++ )
											{
												XmlNode node = nlSurveys[i];
												IDbDataParameter parID = null;
												foreach(IDbDataParameter par in cmdSURVEYS.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdSURVEYS, par.ParameterName).ToUpper();
													if ( sParameterName == "TEAM_ID" && bEnableTeamManagement )
														par.Value = Sql.ToDBGuid(gTEAM_ID);
													else if ( sParameterName == "TEAM_SET_LIST" && bEnableTeamManagement )
														par.Value = ctlTeamSelect.TEAM_SET_LIST;
													else if ( sParameterName == "ASSIGNED_USER_ID" )
														par.Value = Sql.ToDBGuid(gASSIGNED_USER_ID);
													else if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Security.USER_ID;
													else
														par.Value = DBNull.Value;
													if ( sParameterName == "ID" )
														parID = par;
												}
												for ( int j = 0; j < node.ChildNodes.Count; j++ )
												{
													string sName = node.ChildNodes[j].Name     ;
													string sText = node.ChildNodes[j].InnerText;
													Sql.SetParameter(cmdSURVEYS, sName, sText);
												}
												Guid gOLD_ID = Sql.ToGuid(parID.Value);
												// 06/23/2013 Paul.  An imported Survey will always be a new survey. 
												parID.Value = DBNull.Value;
												cmdSURVEYS.ExecuteNonQuery();
												// 06/23/2013 Paul.  Provide mapping from old Survey ID to new Survey ID. 
												Guid gID = Sql.ToGuid(parID.Value);
												if ( !Sql.IsEmptyGuid(gOLD_ID) )
													dictSurveys.Add(gOLD_ID, gID);
												// 06/23/2013 Paul.  We will redirect to first survey if only one survey imported. 
												if ( Sql.IsEmptyGuid(gSURVEY_ID) && nlSurveys.Count == 1 )
													gSURVEY_ID = gID;
											}
											
											XmlNodeList nlSurveyThemes = xmlImport.DocumentElement.SelectNodes("SurveyThemes".ToLower());
											for ( int i = 0; i < nlSurveyThemes.Count; i++ )
											{
												XmlNode node = nlSurveyThemes[i];
												IDbDataParameter parID = null;
												foreach(IDbDataParameter par in cmdSURVEY_THEMES.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdSURVEY_THEMES, par.ParameterName).ToUpper();
													if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Security.USER_ID;
													else
														par.Value = DBNull.Value;
													if ( sParameterName == "ID" )
														parID = par;
												}
												for ( int j = 0; j < node.ChildNodes.Count; j++ )
												{
													string sName = node.ChildNodes[j].Name     ;
													string sText = node.ChildNodes[j].InnerText;
													Sql.SetParameter(cmdSURVEY_THEMES, sName, sText);
												}
												Guid gSURVEY_THEME_ID = Sql.ToGuid(parID.Value);
												
												string sSQL;
												sSQL = "select count(*)       " + ControlChars.CrLf
												     + "  from vwSURVEY_THEMES" + ControlChars.CrLf
												     + " where ID = @ID       " + ControlChars.CrLf;
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.Transaction = trn;
													cmd.CommandText = sSQL;
													Sql.AddParameter(cmd, "@ID", gSURVEY_THEME_ID);
													// 06/23/2013 Paul.  Only import the theme if it does not exist.  Do not overwrite an existing theme. 
													if ( Sql.ToInteger(cmd.ExecuteScalar()) == 0 )
													{
														cmdSURVEY_THEMES.ExecuteNonQuery();
													}
												}
											}
											
											XmlNodeList nlSurveyQuestions = xmlImport.DocumentElement.SelectNodes("SurveyQuestions".ToLower());
											for ( int i = 0; i < nlSurveyQuestions.Count; i++ )
											{
												XmlNode node = nlSurveyQuestions[i];
												IDbDataParameter parID = null;
												foreach(IDbDataParameter par in cmdSURVEY_QUESTIONS.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdSURVEY_QUESTIONS, par.ParameterName).ToUpper();
													if ( sParameterName == "TEAM_ID" && bEnableTeamManagement )
														par.Value = Sql.ToDBGuid(gTEAM_ID);
													else if ( sParameterName == "TEAM_SET_LIST" && bEnableTeamManagement )
														par.Value = ctlTeamSelect.TEAM_SET_LIST;
													else if ( sParameterName == "ASSIGNED_USER_ID" )
														par.Value = Sql.ToDBGuid(gASSIGNED_USER_ID);
													else if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Security.USER_ID;
													else
														par.Value = DBNull.Value;
													if ( sParameterName == "ID" )
														parID = par;
												}
												for ( int j = 0; j < node.ChildNodes.Count; j++ )
												{
													string sName = node.ChildNodes[j].Name     ;
													string sText = node.ChildNodes[j].InnerText;
													Sql.SetParameter(cmdSURVEY_QUESTIONS, sName, sText);
												}
												Guid gSURVEY_QUESTION_ID = Sql.ToGuid(parID.Value);
												
												string sSQL;
												sSQL = "select count(*)          " + ControlChars.CrLf
												     + "  from vwSURVEY_QUESTIONS" + ControlChars.CrLf
												     + " where ID = @ID          " + ControlChars.CrLf;
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.Transaction = trn;
													cmd.CommandText = sSQL;
													Sql.AddParameter(cmd, "@ID", gSURVEY_QUESTION_ID);
													// 06/23/2013 Paul.  Only import the question if it does not exist.  Do not overwrite an existing question. 
													if ( Sql.ToInteger(cmd.ExecuteScalar()) == 0 )
													{
														cmdSURVEY_QUESTIONS.ExecuteNonQuery();
													}
												}
											}
											
											Dictionary<Guid, Guid> dictSurveyPages = new Dictionary<Guid, Guid>();
											XmlNodeList nlSurveyPages = xmlImport.DocumentElement.SelectNodes("SurveyPages".ToLower());
											for ( int i = 0; i < nlSurveyPages.Count; i++ )
											{
												XmlNode node = nlSurveyPages[i];
												IDbDataParameter parID = null;
												IDbDataParameter parSURVEY_ID = null;
												foreach(IDbDataParameter par in cmdSURVEY_PAGES.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdSURVEY_PAGES, par.ParameterName).ToUpper();
													if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Security.USER_ID;
													else
														par.Value = DBNull.Value;
													if ( sParameterName == "ID" )
														parID = par;
													else if ( sParameterName == "SURVEY_ID" )
														parSURVEY_ID = par;
												}
												for ( int j = 0; j < node.ChildNodes.Count; j++ )
												{
													string sName = node.ChildNodes[j].Name     ;
													string sText = node.ChildNodes[j].InnerText;
													Sql.SetParameter(cmdSURVEY_PAGES, sName, sText);
												}
												Guid gOLD_ID        = Sql.ToGuid(parID       .Value);
												Guid gOLD_SURVEY_ID = Sql.ToGuid(parSURVEY_ID.Value);
												// 06/23/2013 Paul.  An imported Survey Page will always be a new. 
												parID.Value = DBNull.Value;
												// 06/23/2013 Paul.  Remap old Survey ID to new Survey ID. 
												if ( dictSurveys.ContainsKey(gOLD_SURVEY_ID) )
													parSURVEY_ID.Value = dictSurveys[gOLD_SURVEY_ID];
												cmdSURVEY_PAGES.ExecuteNonQuery();
												Guid gID = Sql.ToGuid(parID.Value);
												dictSurveyPages.Add(gOLD_ID, gID);
											}
											
											XmlNodeList nlSurveyPagesQuestions = xmlImport.DocumentElement.SelectNodes("SurveyPagesQuestions".ToLower());
											for ( int i = 0; i < nlSurveyPagesQuestions.Count; i++ )
											{
												XmlNode node = nlSurveyPagesQuestions[i];
												IDbDataParameter parSURVEY_PAGE_ID = null;
												foreach(IDbDataParameter par in cmdSURVEY_PAGES_QUESTIONS.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdSURVEY_PAGES_QUESTIONS, par.ParameterName).ToUpper();
													if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Security.USER_ID;
													else
														par.Value = DBNull.Value;
													if ( sParameterName == "SURVEY_PAGE_ID" )
														parSURVEY_PAGE_ID = par;
												}
												for ( int j = 0; j < node.ChildNodes.Count; j++ )
												{
													string sName = node.ChildNodes[j].Name     ;
													string sText = node.ChildNodes[j].InnerText;
													Sql.SetParameter(cmdSURVEY_PAGES_QUESTIONS, sName, sText);
												}
												Guid gOLD_SURVEY_PAGE_ID = Sql.ToGuid(parSURVEY_PAGE_ID.Value);
												// 06/23/2013 Paul.  Remap old Survey Page ID to new Survey Page ID. 
												parSURVEY_PAGE_ID.Value = dictSurveyPages[gOLD_SURVEY_PAGE_ID];
												cmdSURVEY_PAGES_QUESTIONS.ExecuteNonQuery();
											}
											
											// 11/12/2018 Paul.  Export images that are not URLs. 
											XmlNodeList nlImages = xmlImport.DocumentElement.SelectNodes("Images".ToLower());
											for ( int i = 0; i < nlImages.Count; i++ )
											{
												XmlNode node = nlImages[i];
												IDbDataParameter parID = null;
												foreach(IDbDataParameter par in cmdEMAIL_IMAGES.Parameters)
												{
													string sParameterName = Sql.ExtractDbName(cmdEMAIL_IMAGES, par.ParameterName).ToUpper();
													if ( sParameterName == "MODIFIED_USER_ID" )
														par.Value = Security.USER_ID;
													else
														par.Value = DBNull.Value;
													if ( sParameterName == "ID" )
														parID = par;
												}
												for ( int j = 0; j < node.ChildNodes.Count; j++ )
												{
													string sName = node.ChildNodes[j].Name     ;
													string sText = node.ChildNodes[j].InnerText;
													Sql.SetParameter(cmdEMAIL_IMAGES, sName, sText);
												}
												Guid gEMAIL_IMAGE_ID = Sql.ToGuid(parID.Value);
												
												string sSQL;
												sSQL = "select count(*)      " + ControlChars.CrLf
												     + "  from vwEMAIL_IMAGES" + ControlChars.CrLf
												     + " where ID = @ID      " + ControlChars.CrLf;
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.Transaction = trn;
													cmd.CommandText = sSQL;
													Sql.AddParameter(cmd, "@ID", gEMAIL_IMAGE_ID);
													// 11/12/2018 Paul.  Only import the theme if it does not exist.  Do not overwrite an existing theme. 
													if ( Sql.ToInteger(cmd.ExecuteScalar()) == 0 )
													{
														cmdEMAIL_IMAGES.ExecuteNonQuery();
													}
												}
											}
											trn.Commit();
										}
										catch(Exception ex)
										{
											trn.Rollback();
											SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											ctlDynamicButtons.ErrorText = ex.Message;
											return;
										}
									}
								}
							}
						}
						if ( Sql.IsEmptyGuid(gSURVEY_ID) )
							Response.Redirect("default.aspx");
						else
							Response.Redirect("view.aspx?ID=" + gSURVEY_ID.ToString());
					}
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;
			
			reqNAME.DataBind();
			reqFILENAME.DataBind();
			try
			{
				if ( !IsPostBack )
				{
					txtASSIGNED_TO.Text       = Security.USER_NAME;
					txtASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
					ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;
					DataView vwMODULES = new DataView(SplendidCache.ReportingModules());
					vwMODULES.Sort = "DISPLAY_NAME";
					TEAM_NAME.Text    = Security.TEAM_NAME;
					TEAM_ID.Value     = Security.TEAM_ID.ToString();
					ctlTeamSelect.LoadLineItems(Guid.Empty, true);
				}
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
			m_sMODULE = "Surveys";
			SetMenu(m_sMODULE);
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ImportView", Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
