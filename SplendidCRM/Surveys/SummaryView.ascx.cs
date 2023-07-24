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
using System.Globalization;
using System.Diagnostics;

namespace SplendidCRM.Surveys
{
	/// <summary>
	///		Summary description for SummaryView.
	/// </summary>
	public class SummaryView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;
		protected _controls.SearchView     ctlSearchView    ;

		protected Guid                     gID              ;

		public DateTimeFormatInfo DateTimeFormat
		{
			get { return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat; }
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
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
					SqlProcs.spSURVEYS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				// 01/28/2015 Paul.  Need to provide a way to delete all survey results. 
				else if ( e.CommandName == "Survey.DeleteResults" )
				{
					SqlProcs.spSURVEYS_DeleteResults(gID);
					Response.Redirect("summary.aspx?ID=" + gID.ToString());
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
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				// 06/11/2013 Paul.  Register all the Survey JavaScript files. 
				ChartUtil.RegisterScripts(this.Page);
				SurveyUtil.RegisterScripts(this.Page);
				AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(Page) as AjaxControlToolkit.ToolkitScriptManager;
				ScriptReference scrFullCalendar = new ScriptReference ("~/html5/FullCalendar/fullcalendar.js");
				ScriptReference scrAppear       = new ScriptReference ("~/html5/jQuery/jquery.appear.js");
				if ( !mgrAjax.Scripts.Contains(scrFullCalendar) ) mgrAjax.Scripts.Add(scrFullCalendar);
				if ( !mgrAjax.Scripts.Contains(scrAppear      ) ) mgrAjax.Scripts.Add(scrAppear      );
				
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *             " + ControlChars.CrLf
							     + "  from vwSURVEYS_List" + ControlChars.CrLf;
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
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											ViewState["SURVEY_THEME_ID"] = Sql.ToGuid(rdr["SURVEY_THEME_ID"]);
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlDynamicButtons.AppendLinks  (m_sMODULE + ".SummaryView", Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
										}
										else
										{
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
											ctlDynamicButtons.AppendLinks  (m_sMODULE + ".SummaryView", Guid.Empty, null);
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
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
						ctlDynamicButtons.AppendLinks  (m_sMODULE + ".SummaryView", Guid.Empty, null);
						ctlDynamicButtons.DisableAll();
						ctlDynamicButtons.HideAllLinks();
					}
				}
				else
				{
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
				}
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
				HtmlLink cssSummary = new HtmlLink();
				cssSummary.Attributes.Add("href" , "~/Surveys/summary.css");
				cssSummary.Attributes.Add("type" , "text/css"  );
				cssSummary.Attributes.Add("rel"  , "stylesheet");
				Page.Header.Controls.Add(cssSummary);
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
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
				ctlDynamicButtons.AppendLinks  (m_sMODULE + ".SummaryView"  , Guid.Empty, null);
			}
		}
		#endregion
	}
}

