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

namespace SplendidCRM.ReportDesigner
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected QueryDesigner            ctlQueryBuilder         ;
		// 01/13/2010 Paul.  Add footer buttons. 
		protected _controls.DynamicButtons ctlFooterButtons        ;
		protected Reports.ParameterView    ctlParameterView        ;
		protected Reports.ReportView       ctlReportView           ;
		//protected _controls.TeamSelect     ctlTeamSelect           ;
		// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		//protected _controls.UserSelect     ctlUserSelect           ;
		// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
		// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
		//protected HtmlTable                tblViewEvents           ;
		protected HtmlTable                tblMain                 ;

		protected bool            bRun               = false;
		protected Guid            gID                     ;
		// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
		//protected TextBox         txtNAME                 ;
		//protected TextBox         txtASSIGNED_TO          ;
		//protected HtmlInputHidden txtASSIGNED_USER_ID     ;
		//protected CheckBox        chkSHOW_QUERY           ;
		//protected TextBox         txtPAGE_WIDTH           ;
		//protected TextBox         txtPAGE_HEIGHT          ;
		protected Literal         litREPORT_QUERY         ;
		protected Literal         litREPORT_RDL           ;
		protected HiddenField     hidPARAMETERS_EDITVIEW  ;
		protected Button          btnSubmitParameters     ;

		// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
		//protected RequiredFieldValidator reqNAME       ;
		//protected RequiredFieldValidator reqPAGE_WIDTH ;
		//protected RequiredFieldValidator reqPAGE_HEIGHT;
		// 06/17/2010 Paul.  Manually manage singular Team field. 
		// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
		//protected TextBox         TEAM_NAME                    ;
		//protected HiddenField     TEAM_ID                      ;

		private string GetReportType()
		{
			string sREPORT_TYPE = "tabular";
			return sREPORT_TYPE;
		}

		private void UpdateReportProperties()
		{
			string sREPORT_TYPE = GetReportType();
			// 02/11/2010 Paul.  The Report Name is important, so store in the Custom area. 
			// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
			ctlQueryBuilder.SetCustomProperty("ReportName"          , new DynamicControl(this, "NAME"                ).Text);
			ctlQueryBuilder.SetCustomProperty("ReportType"          , sREPORT_TYPE                                         );
			ctlQueryBuilder.SetCustomProperty("AssignedUserID"      , new DynamicControl(this, "ASSIGNED_USER_ID"    ).Text);
			ctlQueryBuilder.SetSingleNode    ("Author"              , new DynamicControl(this, "ASSIGNED_TO_NAME"    ).Text);
			ctlQueryBuilder.SetSingleNode    ("Width"               , new DynamicControl(this, "PAGE_WIDTH"          ).Text);
			ctlQueryBuilder.SetSingleNode    ("PageWidth"           , new DynamicControl(this, "PAGE_WIDTH"          ).Text);
			ctlQueryBuilder.SetSingleNode    ("PageHeight"          , new DynamicControl(this, "PAGE_HEIGHT"         ).Text);
			// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
			ctlQueryBuilder.SetCustomProperty("PRE_LOAD_EVENT_ID"   , new DynamicControl(this, "PRE_LOAD_EVENT_ID"   ).Text);
			ctlQueryBuilder.SetCustomProperty("PRE_LOAD_EVENT_NAME" , new DynamicControl(this, "PRE_LOAD_EVENT_NAME" ).Text);
			ctlQueryBuilder.SetCustomProperty("POST_LOAD_EVENT_ID"  , new DynamicControl(this, "POST_LOAD_EVENT_ID"  ).Text);
			ctlQueryBuilder.SetCustomProperty("POST_LOAD_EVENT_NAME", new DynamicControl(this, "POST_LOAD_EVENT_NAME").Text);
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
								// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
								string sPAGE_WIDTH  = new DynamicControl(this, "PAGE_WIDTH" ).Text.ToLower();
								string sPAGE_HEIGHT = new DynamicControl(this, "PAGE_HEIGHT").Text.ToLower();
								bool bValidWidthUnits  = false;
								bool bValidHeightUnits = false;
								if ( sPAGE_WIDTH.EndsWith("in") || sPAGE_WIDTH.EndsWith("cm") || sPAGE_WIDTH.EndsWith("mm") || sPAGE_WIDTH.EndsWith("pt") || sPAGE_WIDTH.EndsWith("pc") )
									bValidWidthUnits = true;
								if ( sPAGE_HEIGHT.EndsWith("in") || sPAGE_HEIGHT.EndsWith("cm") || sPAGE_HEIGHT.EndsWith("mm") || sPAGE_HEIGHT.EndsWith("pt") || sPAGE_HEIGHT.EndsWith("pc") )
									bValidHeightUnits = true;
								if ( !bValidWidthUnits || !bValidHeightUnits )
									throw(new Exception(L10n.Term("ReportDesigner.ERR_INVALID_REPORT_UNITS")));
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										string sREPORT_TYPE = GetReportType();
										UpdateReportProperties();
										// 08/10/2014 Paul.  We need to update the column widths after setting the page width. 
										ctlQueryBuilder.UpdateDataTable();
										// 10/22/2007 Paul.  Use the Assigned User ID field when saving the record. 
										// 06/17/2010 Paul.  Manually manage singular Team field. 
										// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
										// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
										SqlProcs.spREPORTS_Update
											( ref gID
											, new DynamicControl(this, "ASSIGNED_USER_ID"  ).ID
											, new DynamicControl(this, "NAME"              ).Text
											, ctlQueryBuilder.MODULE
											, sREPORT_TYPE
											, ctlQueryBuilder.ReportRDL
											, new DynamicControl(this, "TEAM_ID"           ).ID
											, new DynamicControl(this, "TEAM_SET_LIST"     ).Text
											, new DynamicControl(this, "PRE_LOAD_EVENT_ID" ).ID
											, new DynamicControl(this, "POST_LOAD_EVENT_ID").ID
											// 05/17/2017 Paul.  Add Tags module. 
											, new DynamicControl(this, "TAG_SET_NAME"      ).Text
											// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
											, new DynamicControl(this, "ASSIGNED_SET_LIST" ).Text
											, trn
											);
										// 08/26/2010 Paul.  Add new record to tracker. 
										// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
										SqlProcs.spTRACKER_Update
											( Security.USER_ID
											, m_sMODULE
											, gID
											, new DynamicControl(this, "NAME").Text
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
							// 09/15/2014 Paul.  Render needs to be hard-coded to the Reports folder. 
							Response.Redirect("~/Reports/render.aspx?ID=" + gID.ToString());
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
						UpdateReportProperties();
						// 08/10/2014 Paul.  We need to update the column widths after setting the page width. 
						ctlQueryBuilder.UpdateDataTable();
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
					string sREPORT_NAME   = new DynamicControl(this, "NAME").Text;
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
					Reports.AttachmentView.SendAsAttachment(this.Context, ctlParameterView, L10n, T10n, gID, sRDL, sRENDER_FORMAT, sMODULE_NAME, Guid.Empty, sREPORT_NAME, sDESCRIPTION, ref gNOTE_ID);
					Response.Redirect("~/Emails/edit.aspx?NOTE_ID=" + gNOTE_ID.ToString() );
				}
				else if ( e.CommandName == "Submit" )
				{
					if ( Page.IsValid && nSelectedColumns > 0 )
					{
						UpdateReportProperties();
						// 08/10/2014 Paul.  We need to update the column widths after setting the page width. 
						ctlQueryBuilder.UpdateDataTable();
						// 08/11/2014 Paul.  The ParametersEditView is only updated during the Run operation. 
						ctlReportView.RunReport(gID, ctlQueryBuilder.ReportRDL, ctlQueryBuilder.MODULE, ctlParameterView);
					}
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

		protected void SaveParametersEditView(DataTable dt)
		{
			hidPARAMETERS_EDITVIEW.Value = String.Empty;
			if ( dt != null )
			{
				DataSet ds = new DataSet();
				dt.TableName = "EDITVIEWS_FIELDS";
				ds.Tables.Add(dt);
				using ( MemoryStream mem = new MemoryStream() )
				{
					using ( TextWriter stm = new StreamWriter(mem) )
					{
						ds.WriteXml(stm, XmlWriteMode.WriteSchema);
						stm.Flush();
					}
					hidPARAMETERS_EDITVIEW.Value = Encoding.UTF8.GetString(mem.ToArray());
				}
			}
		}

		protected void UpdateParametersEditView()
		{
			using ( DataTable dtParametersEditView = SplendidCache.ReportParametersEditView(ctlQueryBuilder.MODULE, ctlQueryBuilder.ReportRDL) )
			{
				btnSubmitParameters.Visible = (dtParametersEditView.Rows.Count > 0);
				ctlParameterView.ClearEditViewFields();
				if ( dtParametersEditView.Rows.Count > 0 )
				{
					ctlParameterView.AppendEditViewFields(dtParametersEditView);
					SaveParametersEditView(dtParametersEditView);
				}
				else
				{
					SaveParametersEditView(null);
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			//reqNAME       .DataBind();
			//reqPAGE_WIDTH .DataBind();
			//reqPAGE_HEIGHT.DataBind();
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				
				bRun = Sql.ToBoolean(Request["Run"]);
				// 10/06/2012 Paul.  Use the ID to make sure that the key to the RDL is unique. 
				string sRdl = Sql.ToString(Session[gID.ToString() + ".rdl"]);
				// 03/27/2020 Paul. SHOW_QUERY is not available until after we append the fields. 
				//if ( !IsPostBack || Sql.IsEmptyString(sRdl) )
				//{
				//	if ( Session["QueryBuilder.SHOW_QUERY"] == null )
				//		Session["QueryBuilder.SHOW_QUERY"] = Sql.ToBoolean(Application["CONFIG.show_sql"]);
				//	// 07/13/2006 Paul.  We don't store the SHOW_QUERY value in the RDL, so we must retrieve it from the session. 
				//	new DynamicControl(this, "SHOW_QUERY").Checked = Sql.ToBoolean(Session["QueryBuilder.SHOW_QUERY"]);
				//}
				//else
				//{
				//	// 07/13/2006 Paul.  Save the SHOW_QUERY flag in the Session so that it will be available across redirects. 
				//	Session["QueryBuilder.SHOW_QUERY"] = new DynamicControl(this, "SHOW_QUERY").Checked;
				//}
				// 07/09/2006 Paul.  The ReportViewer has a bug that prevents it from reloading a previous RDL.
				// The only solution is to create a new ReportViewer object. 
				// 08/09/2014 Paul.  Only render when not a postback. 
				if ( bRun && sRdl.Length > 0 && !IsPostBack )
				{
					// 07/09/2006 Paul.  Clear the Run parameter on the command line. 
					RegisterClientScriptBlock("frmRedirect", "<script type=\"text/javascript\">document.forms[0].action='edit.aspx?ID=" + gID.ToString() + "';</script>");
					// 07/09/2006 Paul.  Clear the session variable as soon as we are done loading it. 
					//Session.Remove(gID.ToString() + ".rdl");
					ctlQueryBuilder.LoadRdl(sRdl);
					
					// 01/24/2010 Paul.  The Report tag does not support the Name attribute. 
					// 02/11/2010 Paul.  The Report Name is important, so store in the Custom area. 
					//txtNAME.Text              = rdl.SelectNodeAttribute(String.Empty, "Name");
					// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
					this.AppendEditViewFields("Reports.EditView", tblMain, null);
					CheckBox SHOW_QUERY = FindControl("SHOW_QUERY") as CheckBox;
					if ( SHOW_QUERY != null )
					{
						SHOW_QUERY.AutoPostBack = true;
						if ( Sql.IsEmptyString(sRdl) )
						{
							if ( Session["QueryBuilder.SHOW_QUERY"] == null )
								Session["QueryBuilder.SHOW_QUERY"] = Sql.ToBoolean(Application["CONFIG.show_sql"]);
							// 07/13/2006 Paul.  We don't store the SHOW_QUERY value in the RDL, so we must retrieve it from the session. 
							SHOW_QUERY.Checked = Sql.ToBoolean(Session["QueryBuilder.SHOW_QUERY"]);
						}
					}

					new DynamicControl(this, "NAME"            ).Text = ctlQueryBuilder.GetCustomPropertyValue("ReportName"    );
					new DynamicControl(this, "ASSIGNED_USER_ID").Text = ctlQueryBuilder.GetCustomPropertyValue("AssignedUserID");
					new DynamicControl(this, "ASSIGNED_TO_NAME").Text = ctlQueryBuilder.SelectNodeValue("Author"    );
					// http://technet.microsoft.com/en-us/library/ms152813(v=sql.90).aspx
					new DynamicControl(this, "PAGE_WIDTH"      ).Text  = ctlQueryBuilder.SelectNodeValue("PageWidth" );
					new DynamicControl(this, "PAGE_HEIGHT"     ).Text  = ctlQueryBuilder.SelectNodeValue("PageHeight");
					
					// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
					// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
					//this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, null);
					new DynamicControl(this, "PRE_LOAD_EVENT_ID"   ).ID   = Sql.ToGuid  (ctlQueryBuilder.GetCustomPropertyValue("PRE_LOAD_EVENT_ID"   ));
					new DynamicControl(this, "PRE_LOAD_EVENT_NAME" ).Text = Sql.ToString(ctlQueryBuilder.GetCustomPropertyValue("PRE_LOAD_EVENT_NAME" ));
					new DynamicControl(this, "POST_LOAD_EVENT_ID"  ).ID   = Sql.ToGuid  (ctlQueryBuilder.GetCustomPropertyValue("POST_LOAD_EVENT_ID"  ));
					new DynamicControl(this, "POST_LOAD_EVENT_NAME").Text = Sql.ToString(ctlQueryBuilder.GetCustomPropertyValue("POST_LOAD_EVENT_NAME"));
					
					// 06/17/2008 Paul.  The Run operation is not a post back, it is a direct navigation, so the buttons need to be handled here as well. 
					// At some point we may need to pass a data reader so that report parameters can be used in the buttons. For now, there seems to be little reason to do so. 
					// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, new DynamicControl(this, "ASSIGNED_USER_ID").ID, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, new DynamicControl(this, "ASSIGNED_USER_ID").ID, null);
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
					ViewState["ASSIGNED_USER_ID"     ] = new DynamicControl(this, "ASSIGNED_USER_ID").Text;
					
					bool bTeamInitialized = false;
					// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
					string sMODULE_NAME = ctlQueryBuilder.MODULE;
					// 04/06/2011 Paul.  Use cache reports to get the team. . 
					DataTable dtReport = SplendidCache.Report(gID);
					if ( dtReport.Rows.Count > 0 )
					{
						DataRow rdr = dtReport.Rows[0];
						// 10/30/2011 Paul.  An imported report may not have the module as a custom property, so load from the record. 
						if ( Sql.IsEmptyString(sMODULE_NAME) )
							sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
						// 10/28/2011 Paul.  We must initialize the team. 
						new DynamicControl(this, "TEAM_NAME").Text = Sql.ToString(rdr["TEAM_NAME"]);
						new DynamicControl(this, "TEAM_ID"  ).Text = Sql.ToString(rdr["TEAM_ID"  ]);
						Guid gTEAM_SET_ID = Sql.ToGuid(rdr["TEAM_SET_ID"]);
						SplendidCRM._controls.TeamSelect ctlTeamSelect = FindControl("TEAM_SET_NAME") as SplendidCRM._controls.TeamSelect;
						if ( ctlTeamSelect != null )
							ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true);
						bTeamInitialized = true;
						// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
						Guid gASSIGNED_SET_ID = Sql.ToGuid(rdr["ASSIGNED_SET_ID"]);
						SplendidCRM._controls.UserSelect ctlUserSelect = FindControl("ASSIGNED_SET_NAME") as SplendidCRM._controls.UserSelect;
						if ( ctlUserSelect != null )
							ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true);
					}
					// 10/28/2011 Paul.  We must initialize the team. 
					if ( !bTeamInitialized )
					{
						new DynamicControl(this, "TEAM_NAME").Text = Security.TEAM_NAME;
						new DynamicControl(this, "TEAM_ID"  ).Text = Security.TEAM_ID.ToString();
						SplendidCRM._controls.TeamSelect ctlTeamSelect = FindControl("TEAM_SET_NAME") as SplendidCRM._controls.TeamSelect;
						if ( ctlTeamSelect != null )
							ctlTeamSelect.LoadLineItems(Guid.Empty, true);
						// 05/10/2018 Paul.  Initialize dynamic assignment. 
						SplendidCRM._controls.UserSelect ctlUserSelect = FindControl("ASSIGNED_SET_NAME") as SplendidCRM._controls.UserSelect;
						if ( ctlUserSelect != null )
							ctlUserSelect.LoadLineItems(Guid.Empty, true);
					}
					UpdateParametersEditView();
					// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
					ctlReportView.RunReport(gID, ctlQueryBuilder.ReportRDL, sMODULE_NAME, ctlParameterView);
				}
				else if ( !IsPostBack )
				{
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL;
							// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
							sSQL = "select *"               + ControlChars.CrLf
							     + "     , 0 as SHOW_QUERY" + ControlChars.CrLf
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
											
											// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
											this.AppendEditViewFields("Reports.EditView", tblMain, rdr);
											CheckBox SHOW_QUERY = FindControl("SHOW_QUERY") as CheckBox;
											if ( SHOW_QUERY != null )
											{
												SHOW_QUERY.AutoPostBack = true;
												if ( Sql.IsEmptyString(sRdl) )
												{
													if ( Session["QueryBuilder.SHOW_QUERY"] == null )
														Session["QueryBuilder.SHOW_QUERY"] = Sql.ToBoolean(Application["CONFIG.show_sql"]);
													// 07/13/2006 Paul.  We don't store the SHOW_QUERY value in the RDL, so we must retrieve it from the session. 
													SHOW_QUERY.Checked = Sql.ToBoolean(Session["QueryBuilder.SHOW_QUERY"]);
												}
											}
											//new DynamicControl(this, "NAME"            ).Text = Sql.ToString(rdr["NAME"            ]);
											//new DynamicControl(this, "ASSIGNED_USER_ID").Text = Sql.ToString(rdr["ASSIGNED_USER_ID"]);
											//new DynamicControl(this, "ASSIGNED_TO_NAME").Text = Sql.ToString(rdr["ASSIGNED_TO_NAME"]);
											ViewState["ASSIGNED_USER_ID"] = new DynamicControl(this, "ASSIGNED_USER_ID").Text;

											string sXML = Sql.ToString(rdr["RDL"]);
											try
											{
												if ( !Sql.IsEmptyString(sXML) )
												{
													// 07/24/2008 Paul.  sXML already has the data. 
													ctlQueryBuilder.LoadRdl(sXML);
													
													// 07/09/2006 Paul.  Update Assigned values as they may have changed externally. 
													// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
													ctlQueryBuilder.SetCustomProperty("AssignedUserID", new DynamicControl(this, "ASSIGNED_USER_ID").Text);
													ctlQueryBuilder.SetSingleNode("Author", new DynamicControl(this, "ASSIGNED_USER_ID").Text);
													// 02/11/2010 Paul.  The Report Name is important, so store in the Custom area. 
													ctlQueryBuilder.SetCustomProperty("ReportName", new DynamicControl(this, "NAME").Text);
													// 01/16/2015 Paul.  Need to load page size properties. 
													// http://technet.microsoft.com/en-us/library/ms152813(v=sql.90).aspx
													// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
													new DynamicControl(this, "PAGE_WIDTH") .Text  = ctlQueryBuilder.SelectNodeValue("PageWidth" );
													new DynamicControl(this, "PAGE_HEIGHT").Text  = ctlQueryBuilder.SelectNodeValue("PageHeight");
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

											// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
											// 06/17/2010 Paul.  Manually manage singular Team field. 
											//new DynamicControl(this, "TEAM_NAME").Text = Sql.ToString(rdr["TEAM_NAME"]);
											//new DynamicControl(this, "TEAM_ID"  ).Text = Sql.ToString(rdr["TEAM_ID"  ]);
											//Guid gTEAM_SET_ID = Sql.ToGuid(rdr["TEAM_SET_ID"]);
											//ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true);
											// 05/10/2018 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
											//Guid gASSIGNED_SET_ID = Sql.ToGuid(rdr["ASSIGNED_SET_ID"]);
											//ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true);
											
											// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
											//this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, rdr);
											// 01/16/2015 Paul.  Need to load report rules properties. 
											new DynamicControl(this, "PRE_LOAD_EVENT_ID"   ).ID   = Sql.ToGuid  (ctlQueryBuilder.GetCustomPropertyValue("PRE_LOAD_EVENT_ID"   ));
											new DynamicControl(this, "PRE_LOAD_EVENT_NAME" ).Text = Sql.ToString(ctlQueryBuilder.GetCustomPropertyValue("PRE_LOAD_EVENT_NAME" ));
											new DynamicControl(this, "POST_LOAD_EVENT_ID"  ).ID   = Sql.ToGuid  (ctlQueryBuilder.GetCustomPropertyValue("POST_LOAD_EVENT_ID"  ));
											new DynamicControl(this, "POST_LOAD_EVENT_NAME").Text = Sql.ToString(ctlQueryBuilder.GetCustomPropertyValue("POST_LOAD_EVENT_NAME"));
										}
										else
										{
											// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
											this.AppendEditViewFields("Reports.EditView", tblMain, null);
											CheckBox SHOW_QUERY = FindControl("SHOW_QUERY") as CheckBox;
											if ( SHOW_QUERY != null )
											{
												SHOW_QUERY.AutoPostBack = true;
												if ( Sql.IsEmptyString(sRdl) )
												{
													if ( Session["QueryBuilder.SHOW_QUERY"] == null )
														Session["QueryBuilder.SHOW_QUERY"] = Sql.ToBoolean(Application["CONFIG.show_sql"]);
													// 07/13/2006 Paul.  We don't store the SHOW_QUERY value in the RDL, so we must retrieve it from the session. 
													SHOW_QUERY.Checked = Sql.ToBoolean(Session["QueryBuilder.SHOW_QUERY"]);
												}
											}
											// 06/17/2010 Paul.  Manually manage singular Team field. 
											//new DynamicControl(this, "TEAM_NAME").Text = Security.TEAM_NAME;
											//new DynamicControl(this, "TEAM_ID"  ).Text = Security.TEAM_ID.ToString();
											//ctlTeamSelect.LoadLineItems(Guid.Empty, true);
											// 05/10/2018 Paul.  Initialize dynamic assignment. 
											//ctlUserSelect.LoadLineItems(Guid.Empty, true);
											
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlFooterButtons .DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
											// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
											ctlQueryBuilder.CreateRdl(new DynamicControl(this, "NAME").Text, new DynamicControl(this, "ASSIGNED_TO_NAME").Text, new DynamicControl(this, "ASSIGNED_USER_ID").Text);
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

						this.AppendEditViewFields("Reports.EditView", tblMain, null);
						CheckBox SHOW_QUERY = FindControl("SHOW_QUERY") as CheckBox;
						if ( SHOW_QUERY != null )
						{
							SHOW_QUERY.AutoPostBack = true;
							if ( Sql.IsEmptyString(sRdl) )
							{
								if ( Session["QueryBuilder.SHOW_QUERY"] == null )
									Session["QueryBuilder.SHOW_QUERY"] = Sql.ToBoolean(Application["CONFIG.show_sql"]);
								// 07/13/2006 Paul.  We don't store the SHOW_QUERY value in the RDL, so we must retrieve it from the session. 
								SHOW_QUERY.Checked = Sql.ToBoolean(Session["QueryBuilder.SHOW_QUERY"]);
							}
						}
						// 06/17/2010 Paul.  Manually manage singular Team field. 
						//TEAM_NAME.Text    = Security.TEAM_NAME;
						//TEAM_ID.Value     = Security.TEAM_ID.ToString();
						//ctlTeamSelect.LoadLineItems(Guid.Empty, true);
						// 05/10/2018 Paul.  Initialize dynamic assignment. 
						//ctlUserSelect.LoadLineItems(Guid.Empty, true);
						// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
						// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
						//this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, null);
						new DynamicControl(this, "NAME"            ).Text = "untitled";
						//new DynamicControl(this, "ASSIGNED_TO_NAME").Text = Security.USER_NAME;
						//new DynamicControl(this, "ASSIGNED_USER_ID").Text = Security.USER_ID.ToString();
						new DynamicControl(this, "PAGE_WIDTH"      ).Text = "11in";
						new DynamicControl(this, "PAGE_HEIGHT"     ).Text = "8.5in";
						ViewState["ASSIGNED_USER_ID"] = new DynamicControl(this, "ASSIGNED_USER_ID").Text;
					
						ctlQueryBuilder.CreateRdl(new DynamicControl(this, "NAME").Text, new DynamicControl(this, "ASSIGNED_TO_NAME").Text, new DynamicControl(this, "ASSIGNED_USER_ID").Text);
					}
					UpdateParametersEditView();
				}
				else
				{
					Session["QueryBuilder.SHOW_QUERY"] = new DynamicControl(this, "SHOW_QUERY").Checked;
					// 01/24/2010 Paul.  The Report tag does not support the Name attribute. 
					//ctlQueryBuilder.SetSingleNodeAttribute(rdl.DocumentElement, "Name", txtNAME.Text);
					// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
					if ( Sql.ToString(ViewState["ASSIGNED_USER_ID"]) != new DynamicControl(this, "ASSIGNED_USER_ID").Text )
					{
						new DynamicControl(this, "ASSIGNED_USER_ID").Text = Crm.Users.USER_NAME(new DynamicControl(this, "ASSIGNED_USER_ID").ID);
						ViewState["ASSIGNED_USER_ID"] = new DynamicControl(this, "ASSIGNED_USER_ID").Text;
					}
					// 08/10/2014 Paul.  We need to update the Report PageWidth before the Page_Load of the QueryDesigner, othterwise the columns calculations will be wrong. 
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
					
					// 08/11/2014 Paul.  We would typically rebuild the ParametersEditView here, but we need to wait until the QueryBuilder is done loaded. 
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
			// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
			if ( new DynamicControl(this, "SHOW_QUERY").Checked )
			{
				litREPORT_QUERY.Text = "<br /><table border=\"1\" cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" bgcolor=\"LightGrey\"><tr><td>";
				litREPORT_QUERY.Text += "<pre><b>" + ctlQueryBuilder.ReportSQL + "</b></pre>";
				litREPORT_QUERY.Text += "</td></tr></table><br />";
#if DEBUG
				// 07/15/2010 Paul.  Use new function to format Rdl. 
				if ( ctlQueryBuilder.RDL != null && ctlQueryBuilder.RDL.DocumentElement != null)
					litREPORT_RDL.Text = RdlUtil.RdlEncode(ctlQueryBuilder.RDL);
#endif
			}
			else
			{
				// 07/15/2010 Paul.  If not checked, we must clear the literal. 
				litREPORT_QUERY.Text = String.Empty;
			}
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
			// 12/21/2014 Paul.  Treat report designer as a separate module. 
			m_sMODULE = "ReportDesigner";
			// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
			m_sVIEW_NAME = "vw" + Crm.Modules.TableName(m_sMODULE) + "_Edit";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/04/2010 Paul.  Add support for Business Rules Framework to Reports. 
				// 03/27/2020 Paul.  Convert to dynamic layout to support React Client. 
				//this.AppendEditViewFields("ReportRules.EventsEditView", tblViewEvents, null);
				this.AppendEditViewFields("Reports.EditView", tblMain, null);
				CheckBox SHOW_QUERY = FindControl("SHOW_QUERY") as CheckBox;
				if ( SHOW_QUERY != null )
					SHOW_QUERY.AutoPostBack = true;
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				
				string sPARAMETERS_EDITVIEW = Sql.ToString(Request[hidPARAMETERS_EDITVIEW.UniqueID]);
				if ( !Sql.IsEmptyString(sPARAMETERS_EDITVIEW) )
				{
					using ( DataSet ds = new DataSet() )
					{
						using ( StringReader stm = new StringReader(sPARAMETERS_EDITVIEW) )
						{
							ds.ReadXml(stm);
						}
						if ( ds.Tables.Count > 0 )
						{
							DataTable dtParametersEditView = ds.Tables[0];
							if ( dtParametersEditView.Rows.Count > 0 )
							{
								ctlParameterView.AppendEditViewFields(dtParametersEditView);
							}
						}
					}
				}
			}
		}
		#endregion
	}
}
