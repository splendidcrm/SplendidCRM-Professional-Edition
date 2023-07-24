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
using System.IO;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.Collections;
using System.Threading;

namespace SplendidCRM.Reports
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected Reports.QueryBuilder     ctlQueryBuilder         ;
		// 01/13/2010 Paul.  Add footer buttons. 
		protected _controls.DynamicButtons ctlFooterButtons        ;
		protected ReportView               ctlReportView           ;
		protected _controls.TeamSelect     ctlTeamSelect           ;
		// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		protected _controls.UserSelect     ctlUserSelect           ;
		// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
		protected HtmlTable                tblViewEvents           ;

		protected bool            bRun               = false;
		protected Guid            gID                     ;
		protected TextBox         txtNAME                 ;
		protected TextBox         txtASSIGNED_TO          ;
		protected HtmlInputHidden txtASSIGNED_USER_ID     ;

		protected RequiredFieldValidator reqNAME;
		// 06/17/2010 Paul.  Manually manage singular Team field. 
		protected TextBox         TEAM_NAME                    ;
		protected HiddenField     TEAM_ID                      ;

		private string GetReportType()
		{
			string sREPORT_TYPE = "tabular";
			return sREPORT_TYPE;
		}

		#region Page_Command
		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				int nSelectedColumns = ctlQueryBuilder.SelectedColumns;
				// 01/24/2010 Paul.  Clear the Report List on save. 
				if ( e.CommandName == "Save"  || e.CommandName == "SaveDuplicate" || e.CommandName == "SaveConcurrency" )
				{
					SplendidCache.ClearReports();
				}
				// 04/11/2020 Paul.  Starts with Attachment. 
				if ( e.CommandName.StartsWith("Attachment") || e.CommandName == "Save" || e.CommandName == "Run" || e.CommandName == "Print"  || e.CommandName == "SaveDuplicate" || e.CommandName == "SaveConcurrency" )
				{
					//reqNAME.Enabled = true;
					//reqNAME.Validate();
					if ( nSelectedColumns == 0 )
					{
						ctlQueryBuilder.ActiveTab = "2";
						ctlDynamicButtons.ErrorText = L10n.Term("Reports.LBL_DISPLAY_COLUMNS_REQUIRED");
					}
				}
				// 01/14/2010 Paul.  Not sure why Print was not previous supported. 
				// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
				// 03/15/2014 Paul.  Enable override of concurrency error. 
				if ( e.CommandName == "Save" || e.CommandName == "Print" || e.CommandName == "SaveDuplicate" || e.CommandName == "SaveConcurrency" )
				{
					if ( Page.IsValid && nSelectedColumns > 0 )
					{
						try
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								DataRow rowCurrent = null;
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
										string sREPORT_TYPE = GetReportType();
										// 02/11/2010 Paul.  The Report Name is important, so store in the Custom area. 
										ctlQueryBuilder.SetCustomProperty("ReportName"    , txtNAME.Text             );
										ctlQueryBuilder.SetCustomProperty("ReportType"    , sREPORT_TYPE             );
										ctlQueryBuilder.SetCustomProperty("AssignedUserID", txtASSIGNED_USER_ID.Value);
										ctlQueryBuilder.SetSingleNode    ("Author"        , txtASSIGNED_TO.Text      );
										// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
										ctlQueryBuilder.SetCustomProperty("PRE_LOAD_EVENT_ID"   , new DynamicControl(this, "PRE_LOAD_EVENT_ID"   ).Text);
										ctlQueryBuilder.SetCustomProperty("PRE_LOAD_EVENT_NAME" , new DynamicControl(this, "PRE_LOAD_EVENT_NAME" ).Text);
										ctlQueryBuilder.SetCustomProperty("POST_LOAD_EVENT_ID"  , new DynamicControl(this, "POST_LOAD_EVENT_ID"  ).Text);
										ctlQueryBuilder.SetCustomProperty("POST_LOAD_EVENT_NAME", new DynamicControl(this, "POST_LOAD_EVENT_NAME").Text);
										// 10/22/2007 Paul.  Use the Assigned User ID field when saving the record. 
										// 06/17/2010 Paul.  Manually manage singular Team field. 
										Guid gTEAM_ID = Guid.Empty;
										if ( SplendidCRM.Crm.Config.enable_dynamic_teams() )
											gTEAM_ID = ctlTeamSelect.TEAM_ID;
										else
											gTEAM_ID = Sql.ToGuid(TEAM_ID.Value);
										// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
										Guid gASSIGNED_USER_ID = Guid.Empty;
										if ( SplendidCRM.Crm.Config.enable_dynamic_assignment() )
											gASSIGNED_USER_ID = ctlUserSelect.USER_ID;
										else
											gASSIGNED_USER_ID = Sql.ToGuid(txtASSIGNED_USER_ID.Value);
										// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
										SqlProcs.spREPORTS_Update
											( ref gID
											, gASSIGNED_USER_ID
											, txtNAME.Text
											, ctlQueryBuilder.MODULE
											, sREPORT_TYPE
											, ctlQueryBuilder.ReportRDL
											, gTEAM_ID
											, ctlTeamSelect.TEAM_SET_LIST
											, new DynamicControl(this, "PRE_LOAD_EVENT_ID" ).ID
											, new DynamicControl(this, "POST_LOAD_EVENT_ID").ID
											// 05/17/2017 Paul.  Add Tags module. 
											, new DynamicControl(this, "TAG_SET_NAME"      ).Text
											// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
											, ctlUserSelect.ASSIGNED_SET_LIST
											, trn
											);
										// 08/26/2010 Paul.  Add new record to tracker. 
										// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
										SqlProcs.spTRACKER_Update
											( Security.USER_ID
											, m_sMODULE
											, gID
											, txtNAME.Text
											, "save"
											, trn
											);
										trn.Commit();
										// 04/06/2011 Paul.  Cache reports. 
										SplendidCache.ClearReport(gID);
										// 04/03/2012 Paul.  Just in case the name changes, clear the favorites. 
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
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							ctlDynamicButtons.ErrorText = ex.Message;
							return;
						}
						if ( e.CommandName == "Print" )
						{
							Response.Redirect("render.aspx?ID=" + gID.ToString());
						}
						else
						{
							// 10/28/2011 Paul.  When save reverts back to the edit screen, it seems like it did not work. 
							//Response.Redirect("edit.aspx?ID=" + gID.ToString());
							Response.Redirect("default.aspx");
						}
					}
				}
				else if ( e.CommandName == "Run" )
				{
					if ( Page.IsValid && nSelectedColumns > 0 )
					{
						// 02/11/2010 Paul.  The Report Name is important, so store in the Custom area. 
						ctlQueryBuilder.SetCustomProperty("ReportName", txtNAME.Text);
						// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
						ctlQueryBuilder.SetCustomProperty("PRE_LOAD_EVENT_ID"   , new DynamicControl(this, "PRE_LOAD_EVENT_ID"   ).Text);
						ctlQueryBuilder.SetCustomProperty("PRE_LOAD_EVENT_NAME" , new DynamicControl(this, "PRE_LOAD_EVENT_NAME" ).Text);
						ctlQueryBuilder.SetCustomProperty("POST_LOAD_EVENT_ID"  , new DynamicControl(this, "POST_LOAD_EVENT_ID"  ).Text);
						ctlQueryBuilder.SetCustomProperty("POST_LOAD_EVENT_NAME", new DynamicControl(this, "POST_LOAD_EVENT_NAME").Text);
						// 07/09/2006 Paul.  The ReportViewer has a bug that prevents it from reloading a previous RDL.
						// The only solution is to create a new ReportViewer object. 
						// 10/06/2012 Paul.  Use the ID to make sure that the key to the RDL is unique. 
						Session[gID.ToString() + ".rdl"] = ctlQueryBuilder.ReportRDL;
						Response.Redirect("edit.aspx?ID=" + gID.ToString() + "&Run=1");
						//ctlReportView.RunReport(gID, ctlQueryBuilder.ReportRDL);
					}
				}
				// 05/06/2018 Paul.  Allow button argument to specify render format so that we generate Excel attachment. 
				else if ( e.CommandName == "Attachment" || e.CommandName == "Attachment-PDF" || e.CommandName == "Attachment-Excel" || e.CommandName == "Attachment-Word" || e.CommandName == "Attachment-Image" )
				{
					string sRDL           = ctlQueryBuilder.ReportRDL;
					string sMODULE_NAME   = ctlQueryBuilder.MODULE;
					string sREPORT_NAME   = txtNAME.Text;
					string sDESCRIPTION   = sREPORT_NAME + " " + DateTime.Now.ToString();
					Guid gNOTE_ID = Guid.Empty;
					// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
					// 05/06/2018 Paul.  Allow button argument to specify render format so that we generate Excel attachment. 
					string sRENDER_FORMAT = "PDF";
					switch ( e.CommandName )
					{
						case "Attachment"      :  sRENDER_FORMAT = "PDF"  ;  break;
						case "Attachment-PDF"  :  sRENDER_FORMAT = "PDF"  ;  break;
						// 02/05/2021 Paul.  Use EXCELOPENXML instead of EXCEL. 
						case "Attachment-Excel":  sRENDER_FORMAT = "EXCELOPENXML";  break;
						// 02/05/2021 Paul.  Use WORDOPENXML instead of WORD. 
						case "Attachment-Word" :  sRENDER_FORMAT = "WORDOPENXML" ;  break;
						case "Attachment-Image":  sRENDER_FORMAT = "IMAGE";  break;
					}
					AttachmentView.SendAsAttachment(this.Context, L10n, T10n, gID, sRDL, sRENDER_FORMAT, sMODULE_NAME, Guid.Empty, sREPORT_NAME, sDESCRIPTION, ref gNOTE_ID);
					Response.Redirect("~/Emails/edit.aspx?NOTE_ID=" + gNOTE_ID.ToString() );
				}
				else if ( e.CommandName == "Filters.Change" )
				{
					if ( Sql.ToString(e.CommandArgument) == "Module" )
					{
						ctlReportView.ClearReport();
					}
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			reqNAME.DataBind();
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				
				bRun = Sql.ToBoolean(Request["Run"]);
				// 10/06/2012 Paul.  Use the ID to make sure that the key to the RDL is unique. 
				string sRdl = Sql.ToString(Session[gID.ToString() + ".rdl"]);
				// 07/09/2006 Paul.  The ReportViewer has a bug that prevents it from reloading a previous RDL.
				// The only solution is to create a new ReportViewer object. 
				if ( bRun && sRdl.Length > 0 )
				{
					// 07/09/2006 Paul.  Clear the Run parameter on the command line. 
					RegisterClientScriptBlock("frmRedirect", "<script type=\"text/javascript\">document.forms[0].action='edit.aspx?ID=" + gID.ToString() + "';</script>");
					// 07/09/2006 Paul.  Clear the session variable as soon as we are done loading it. 
					//Session.Remove(gID.ToString() + ".rdl");
					ctlQueryBuilder.LoadRdl(sRdl);
					
					// 01/24/2010 Paul.  The Report tag does not support the Name attribute. 
					// 02/11/2010 Paul.  The Report Name is important, so store in the Custom area. 
					//txtNAME.Text              = rdl.SelectNodeAttribute(String.Empty, "Name");
					txtNAME.Text              = ctlQueryBuilder.GetCustomPropertyValue("ReportName");
					ctlQueryBuilder.ActiveTab = "1";
					txtASSIGNED_USER_ID.Value = ctlQueryBuilder.GetCustomPropertyValue("AssignedUserID");
					txtASSIGNED_TO.Text       = ctlQueryBuilder.SelectNodeValue("Author");
					
					// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
					this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, null);
					new DynamicControl(this, "PRE_LOAD_EVENT_ID"   ).ID   = Sql.ToGuid  (ctlQueryBuilder.GetCustomPropertyValue("PRE_LOAD_EVENT_ID"   ));
					new DynamicControl(this, "PRE_LOAD_EVENT_NAME" ).Text = Sql.ToString(ctlQueryBuilder.GetCustomPropertyValue("PRE_LOAD_EVENT_NAME" ));
					new DynamicControl(this, "POST_LOAD_EVENT_ID"  ).ID   = Sql.ToGuid  (ctlQueryBuilder.GetCustomPropertyValue("POST_LOAD_EVENT_ID"  ));
					new DynamicControl(this, "POST_LOAD_EVENT_NAME").Text = Sql.ToString(ctlQueryBuilder.GetCustomPropertyValue("POST_LOAD_EVENT_NAME"));
					
					// 06/17/2008 Paul.  The Run operation is not a post back, it is a direct navigation, so the buttons need to be handled here as well. 
					// At some point we may need to pass a data reader so that report parameters can be used in the buttons. For now, there seems to be little reason to do so. 
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(txtASSIGNED_USER_ID.Value), null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(txtASSIGNED_USER_ID.Value), null);
					// 01/14/2010 Paul.  Disable the Print button until report is saved. 
					ctlDynamicButtons.EnableButton("Print", !Sql.IsEmptyGuid(gID) );
					ctlFooterButtons .EnableButton("Print", !Sql.IsEmptyGuid(gID) );
					// 10/22/2010 Paul.  The Attachment button should be disabled until the report is run for the first time. 
					ctlDynamicButtons.EnableButton("Attachment", !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyString(ctlQueryBuilder.ReportRDL) );
					ctlFooterButtons .EnableButton("Attachment", !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyString(ctlQueryBuilder.ReportRDL) );

					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = ctlQueryBuilder.SelectNodeAttribute(String.Empty, "Name");
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
					ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
					ViewState["ASSIGNED_USER_ID"     ] = txtASSIGNED_USER_ID.Value;
					
					bool bTeamInitialized = false;
					// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
					string sMODULE_NAME = ctlQueryBuilder.GetCustomPropertyValue("Module");
					// 04/06/2011 Paul.  Use cache reports to get the team. . 
					DataTable dtReport = SplendidCache.Report(gID);
					if ( dtReport.Rows.Count > 0 )
					{
						DataRow rdr = dtReport.Rows[0];
						// 10/30/2011 Paul.  An imported report may not have the module as a custom property, so load from the record. 
						if ( Sql.IsEmptyString(sMODULE_NAME) )
							sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
						// 10/28/2011 Paul.  We must initialize the team. 
						TEAM_NAME.Text    = Sql.ToString(rdr["TEAM_NAME"]);
						TEAM_ID.Value     = Sql.ToString(rdr["TEAM_ID"  ]);
						Guid gTEAM_SET_ID = Sql.ToGuid(rdr["TEAM_SET_ID"]);
						ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true);
						bTeamInitialized = true;
						// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
						Guid gASSIGNED_SET_ID = Sql.ToGuid(rdr["ASSIGNED_SET_ID"]);
						ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true);
					}
					// 10/28/2011 Paul.  We must initialize the team. 
					if ( !bTeamInitialized )
					{
						TEAM_NAME.Text    = Security.TEAM_NAME;
						TEAM_ID.Value     = Security.TEAM_ID.ToString();
						ctlTeamSelect.LoadLineItems(Guid.Empty, true);
						// 05/10/2018 Paul.  Initialize dynamic assignment. 
						ctlUserSelect.LoadLineItems(Guid.Empty, true);
					}
					// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
					ctlReportView.RunReport(gID, ctlQueryBuilder.ReportRDL, sMODULE_NAME);
				}
				else if ( !IsPostBack )
				{
					txtNAME.Text              = "untitled";
					ctlQueryBuilder.ActiveTab = "1";
					txtASSIGNED_TO.Text       = Security.USER_NAME;
					txtASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
					ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;
					
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL;
							// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
							sSQL = "select *"               + ControlChars.CrLf
							     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
							     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 06/17/2010 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								// 04/24/2018 Paul.  Provide a way to exclude the SavedSearch for areas that are global in nature. 
								Security.Filter(cmd, m_sMODULE, "edit", "ASSIGNED_USER_ID", true);
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

								// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
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
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											// 06/25/2010 Paul.  Add tracker for the report. 
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

											txtNAME.Text              = Sql.ToString(rdr["NAME"            ]);
											txtASSIGNED_USER_ID.Value = Sql.ToString(rdr["ASSIGNED_USER_ID"]);
											txtASSIGNED_TO.Text       = Sql.ToString(rdr["ASSIGNED_TO"     ]);
											ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;

											string sXML = Sql.ToString(rdr["RDL"]);
											try
											{
												if ( !Sql.IsEmptyString(sXML) )
												{
													// 07/24/2008 Paul.  sXML already has the data. 
													ctlQueryBuilder.LoadRdl(sXML);
													
													// 07/09/2006 Paul.  Update Assigned values as they may have changed externally. 
													ctlQueryBuilder.SetCustomProperty("AssignedUserID", txtASSIGNED_USER_ID.Value);
													ctlQueryBuilder.SetSingleNode("Author", txtASSIGNED_TO.Text);
													// 02/11/2010 Paul.  The Report Name is important, so store in the Custom area. 
													ctlQueryBuilder.SetCustomProperty("ReportName", txtNAME.Text);
												}
											}
											catch
											{
											}
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											// 01/14/2010 Paul.  Disable the Print button until report is saved. 
											ctlDynamicButtons.EnableButton("Print", !Sql.IsEmptyGuid(gID) );
											ctlFooterButtons .EnableButton("Print", !Sql.IsEmptyGuid(gID) );
											// 10/22/2010 Paul.  The Attachment button should be disabled until the report is run for the first time. 
											ctlDynamicButtons.EnableButton("Attachment", !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyString(ctlQueryBuilder.ReportRDL) );
											ctlFooterButtons .EnableButton("Attachment", !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyString(ctlQueryBuilder.ReportRDL) );

											// 06/17/2010 Paul.  Manually manage singular Team field. 
											TEAM_NAME.Text    = Sql.ToString(rdr["TEAM_NAME"]);
											TEAM_ID.Value     = Sql.ToString(rdr["TEAM_ID"  ]);
											Guid gTEAM_SET_ID = Sql.ToGuid(rdr["TEAM_SET_ID"]);
											ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true);
											// 05/10/2018 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
											Guid gASSIGNED_SET_ID = Sql.ToGuid(rdr["ASSIGNED_SET_ID"]);
											ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true);
											
											// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
											this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, rdr);
										}
										else
										{
											// 06/17/2010 Paul.  Manually manage singular Team field. 
											TEAM_NAME.Text    = Security.TEAM_NAME;
											TEAM_ID.Value     = Security.TEAM_ID.ToString();
											ctlTeamSelect.LoadLineItems(Guid.Empty, true);
											// 05/10/2018 Paul.  Initialize dynamic assignment. 
											ctlUserSelect.LoadLineItems(Guid.Empty, true);
											
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlFooterButtons .DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
											ctlQueryBuilder.CreateRdl(txtNAME.Text, txtASSIGNED_TO.Text, txtASSIGNED_USER_ID.Value);
										}
									}
								}
							}
						}
					}
					else
					{
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						// 01/14/2010 Paul.  Disable the Print button until report is saved. 
						ctlDynamicButtons.EnableButton("Print", !Sql.IsEmptyGuid(gID) );
						ctlFooterButtons .EnableButton("Print", !Sql.IsEmptyGuid(gID) );
						// 10/22/2010 Paul.  The Attachment button should be disabled until the report is run for the first time. 
						ctlDynamicButtons.EnableButton("Attachment", !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyString(ctlQueryBuilder.ReportRDL) );
						ctlFooterButtons .EnableButton("Attachment", !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyString(ctlQueryBuilder.ReportRDL) );

						// 06/17/2010 Paul.  Manually manage singular Team field. 
						TEAM_NAME.Text    = Security.TEAM_NAME;
						TEAM_ID.Value     = Security.TEAM_ID.ToString();
						ctlTeamSelect.LoadLineItems(Guid.Empty, true);
						// 05/10/2018 Paul.  Initialize dynamic assignment. 
						ctlUserSelect.LoadLineItems(Guid.Empty, true);
						// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
						this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, null);
						ctlQueryBuilder.CreateRdl(txtNAME.Text, txtASSIGNED_TO.Text, txtASSIGNED_USER_ID.Value);
					}
				}
				else
				{
					// 01/24/2010 Paul.  The Report tag does not support the Name attribute. 
					//ctlQueryBuilder.SetSingleNodeAttribute(rdl.DocumentElement, "Name", txtNAME.Text);
					if ( Sql.ToString(ViewState["ASSIGNED_USER_ID"]) != txtASSIGNED_USER_ID.Value )
					{
						txtASSIGNED_TO.Text = Crm.Users.USER_NAME(Sql.ToGuid(txtASSIGNED_USER_ID.Value));
						ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;
					}
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
				}
#if DEBUG
				//RegisterClientScriptBlock("ReportSQL", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript("\r" + sReportSQL) + "';</script>");
#endif
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			ViewState["rdl"] = ctlQueryBuilder.ReportRDL;
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
			this.PreRender += new System.EventHandler(this.Page_PreRender);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);
			ctlQueryBuilder  .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Reports";
			// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
			m_sVIEW_NAME = "vw" + Crm.Modules.TableName(m_sMODULE) + "_Edit";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
				this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}
