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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Surveys
{
	/// <summary>
	///		Summary description for ResultsView.
	/// </summary>
	public class ResultsView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons ;
		protected _controls.SearchView    ctlSearchView     ;
		protected _controls.CheckAll      ctlCheckAll       ;

		protected Guid                     gID              ;
		protected DataView                 vwMain           ;
		protected SplendidGrid             grdMain          ;
		protected string                   sNAME            ;

		protected string TimeSpent(object oSUBMIT_DATE, object oSTART_DATE)
		{
			string sTimeSpent = String.Empty;
			if ( Sql.ToDateTime(oSUBMIT_DATE) != DateTime.MinValue )
			{
				TimeSpan ts = Sql.ToDateTime(oSUBMIT_DATE) - Sql.ToDateTime(oSTART_DATE);
				sTimeSpent = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
			}
			return sTimeSpent;
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				// 12/30/2015 Paul.  We do not want the standard Clear behavior, so set IsSubpanelSearch flag.  
				if ( e.CommandName == "Clear" )
				{
					ctlSearchView.IsSubpanelSearch = true;
					grdMain.CurrentPageIndex = 0;
					BindGrid(true);
				}
				else if ( e.CommandName == "Search" )
				{
					grdMain.CurrentPageIndex = 0;
					grdMain.DataBind();
				}
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
				}
				else if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Duplicate" )
				{
					Guid gDUPLICATE_ID = Guid.Empty;
					string sCOPY_OF = L10n.Term("Surveys.LBL_COPY_OF");
					if ( sCOPY_OF == "Surveys.LBL_COPY_OF" )
						sCOPY_OF = String.Empty;
					SqlProcs.spSURVEYS_Duplicate(ref gDUPLICATE_ID, gID, L10n.Term("Surveys.LBL_COPY_OF"));
					Response.Redirect("view.aspx?ID=" + gDUPLICATE_ID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					if ( vwMain.Count > 0 )
					{
						Guid gSURVEY_RESULT_ID = Sql.ToGuid(vwMain[grdMain.CurrentPageIndex]["SURVEY_RESULT_ID"]);
						SqlProcs.spSURVEY_RESULTS_Delete(gSURVEY_RESULT_ID);
						if ( grdMain.CurrentPageIndex == vwMain.Count - 1 )
							grdMain.CurrentPageIndex--;
					}
					BindGrid(true);
				}
				// 01/28/2015 Paul.  Need to provide a way to delete all survey results. 
				else if ( e.CommandName == "Survey.DeleteResults" )
				{
					SqlProcs.spSURVEYS_DeleteResults(gID);
					Response.Redirect("results.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Export" )
				{
					//SplendidExport.Export(vwMain, m_sMODULE, ctlExportHeader.ExportFormat, ctlExportHeader.ExportRange, grdMain.CurrentPageIndex, grdMain.PageSize, arrID, grdMain.AllowCustomPaging);
				}
				// 01/05/2016 Paul.  Provide a way to run a report from mass update. 
				else if ( e.CommandName == "Report" || e.CommandName == "ReportAll" )
				{
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( e.CommandName == "ReportAll" )
					{
						if ( vwMain == null )
							grdMain.DataBind();
						if ( vwMain != null )
						{
							arrID = new string[vwMain.Count];
							for ( int i = 0; i < vwMain.Count; i++ )
							{
								arrID[i] = Sql.ToString(vwMain[i]["ID"]);
							}
						}
					}
					if ( arrID != null && arrID.Length > 0 )
					{
						if ( Sql.IsEmptyString(e.CommandArgument) )
						{
							throw(new Exception("Button CommandArgument is empty."));
						}
						string sRenderURL = "~/Reports/render.aspx?" + Sql.ToString(e.CommandArgument).Trim();
						if ( !sRenderURL.EndsWith("&") )
							sRenderURL += "&";
						string sTABLE_NAME         = Crm.Modules.TableName(m_sMODULE);
						string sMODULE_FIELD_NAME  = Crm.Modules.SingularTableName(sTABLE_NAME) + "_ID";
						sRenderURL += sMODULE_FIELD_NAME + "=" + String.Join("&" + sMODULE_FIELD_NAME + "=", arrID);
						Server.Transfer(sRenderURL);
					}
					else
					{
						throw(new Exception(L10n.Term(".LBL_NOTHING_SELECTED")));
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void BindGrid(bool bBind)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					sSQL = "select *                     " + ControlChars.CrLf
					     + "  from vwSURVEY_RESULTS_Edit " + ControlChars.CrLf
					     + " where SURVEY_ID = @SURVEY_ID" + ControlChars.CrLf;
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@SURVEY_ID", gID);
					
					ctlSearchView.SqlSearchClause(cmd);
					cmd.CommandText += " order by DATE_MODIFIED asc";

					if ( bDebug )
						RegisterClientScriptBlock("vwSURVEY_RESULTS_Edit", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( bBind )
								{
									grdMain.DataBind();
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *        " + ControlChars.CrLf
							     + "  from vwSURVEYS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "view");
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											
											// 12/29/2015 Paul.  Enable respondant searching. 
											sNAME = Sql.ToString(rdr["NAME"]);
											ViewState["Survey.Name"] = sNAME;
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											//this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
											Guid gSURVEY_THEME_ID = Sql.ToGuid(rdr["SURVEY_THEME_ID"]);
											HtmlLink cssSurveyStylesheet = new HtmlLink();
											cssSurveyStylesheet.Attributes.Add("id"   , gSURVEY_THEME_ID.ToString().Replace("-", "_"));
											cssSurveyStylesheet.Attributes.Add("href" , "~/Surveys/stylesheet.aspx?ID=" + gSURVEY_THEME_ID.ToString());
											cssSurveyStylesheet.Attributes.Add("type" , "text/css"  );
											cssSurveyStylesheet.Attributes.Add("rel"  , "stylesheet");
											Page.Header.Controls.Add(cssSurveyStylesheet);
											ViewState["SURVEY_THEME_ID"] = gSURVEY_THEME_ID;
											
											Page.Items["ASSIGNED_USER_ID"] = Sql.ToGuid(rdr["ASSIGNED_USER_ID"]);
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											// 11/01/2015 Paul.  Separate buttons for ResultsDetailView.  Put Test first because we don't want
											ctlDynamicButtons.AppendButtons(m_sMODULE + ".ResultsDetailView", Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlDynamicButtons.AppendLinks  (m_sMODULE + ".ResultsView"      , Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
										}
										else
										{
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											// 11/01/2015 Paul.  Separate buttons for ResultsDetailView.  Put Test first because we don't want
											ctlDynamicButtons.AppendButtons(m_sMODULE + ".ResultsDetailView", Guid.Empty, null);
											ctlDynamicButtons.AppendLinks  (m_sMODULE + ".ResultsView"      , Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.HideAllLinks();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
					}
					else
					{
						// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + ".ResultsDetailView", Guid.Empty, null);
						ctlDynamicButtons.AppendLinks  (m_sMODULE + ".ResultsView", Guid.Empty, null);
						ctlDynamicButtons.DisableAll();
						ctlDynamicButtons.HideAllLinks();
					}
				}
				else
				{
					// 12/29/2015 Paul.  Enable respondant searching. 
					sNAME = Sql.ToString(ViewState["Survey.Name"]);
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
				}
				BindGrid(true);
				if ( !Sql.IsEmptyGuid(ViewState["SURVEY_THEME_ID"]) )
				{
					Guid gSURVEY_THEME_ID = Sql.ToGuid(ViewState["SURVEY_THEME_ID"]);
					HtmlLink cssSurveyStylesheet = new HtmlLink();
					cssSurveyStylesheet.Attributes.Add("id"   , gSURVEY_THEME_ID.ToString().Replace("-", "_"));
					cssSurveyStylesheet.Attributes.Add("href" , "~/Surveys/stylesheet.aspx?ID=" + gSURVEY_THEME_ID.ToString());
					cssSurveyStylesheet.Attributes.Add("type" , "text/css"  );
					cssSurveyStylesheet.Attributes.Add("rel"  , "stylesheet");
					Page.Header.Controls.Add(cssSurveyStylesheet);
				}
				// 06/11/2013 Paul.  Register all the Survey JavaScript files. 
				SurveyUtil.RegisterScripts(this.Page);
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
			ctlSearchView.Command     += new CommandEventHandler(Page_Command);
			m_sMODULE = "Surveys";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				//this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
				// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
				// 11/01/2015 Paul.  Separate buttons for ResultsDetailView.  Put Test first because we don't want
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".ResultsDetailView", Guid.Empty, null);
				ctlDynamicButtons.AppendLinks  (m_sMODULE + ".ResultsView"  , Guid.Empty, null);
			}
		}
		#endregion
	}
}

