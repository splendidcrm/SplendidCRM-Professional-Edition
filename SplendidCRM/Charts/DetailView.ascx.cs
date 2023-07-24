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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Charts
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;
		protected Reports.ParameterView    ctlParameterView ;
		protected ChartView                ctlChartView     ;

		protected Guid             gID               ;
		protected string           sRDL              ;
		protected string           sMODULE_NAME      ;
		protected string           sCHART_NAME       ;
		protected Guid             gASSIGNED_USER_ID ;
		protected DataTable        dtReportParameters;
		protected PlaceHolder      plcSubPanel       ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Duplicate" )
				{
					Response.Redirect("edit.aspx?DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spACCOUNTS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Submit" )
				{
					if ( !Sql.IsEmptyString(sRDL) )
					{
						ctlChartView.RunReport(sRDL, sMODULE_NAME, ctlParameterView);
					}
				}
				else
				{
					throw(new Exception("Unknown command: " + e.CommandName));
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
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DataTable dtChart = SplendidCache.Chart(gID);
						if ( dtChart != null && dtChart.Rows.Count > 0 )
						{
							DataRow rdr = dtChart.Rows[0];
							sRDL              = Sql.ToString(rdr["RDL"             ]);
							sMODULE_NAME      = Sql.ToString(rdr["MODULE_NAME"     ]);
							sCHART_NAME       = Sql.ToString(rdr["NAME"            ]);
							gASSIGNED_USER_ID = Sql.ToGuid  (rdr["ASSIGNED_USER_ID"]);

							// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
							ctlDynamicButtons.Title = sCHART_NAME;
							SetPageTitle(L10n.Term(".moduleList.Charts") + " - " + ctlDynamicButtons.Title);
							Utils.UpdateTracker(Page, m_sMODULE, gID, sCHART_NAME);
							
							ctlDynamicButtons.AppendButtons(m_sMODULE + ".DetailView", gASSIGNED_USER_ID, rdr);
							ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
							ViewState["MODULE_NAME"] = sMODULE_NAME;
							
							// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
							dtReportParameters = SplendidCache.ChartParameters(gID, Security.USER_ID);
							if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
							{
								// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
								DataTable dt = SplendidCache.ChartParametersEditView(gID, Security.USER_ID);
								ctlParameterView.AppendEditViewFields(dt);
							}
							if ( !Sql.IsEmptyString(sRDL) )
							{
								if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
								{
									foreach ( DataRow rowParameter in dtReportParameters.Rows )
									{
										string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]);
										string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
										DynamicControl ctl = new DynamicControl(ctlParameterView, sDATA_FIELD);
										if ( ctl.Exists )
										{
											if ( Request.QueryString[sDATA_FIELD] != null )
											{
												if ( sDATA_FIELD.EndsWith("_ID") )
												{
													try
													{
														Guid gDATA_FIELD = Sql.ToGuid(Request.QueryString[sDATA_FIELD]);
														ctl.ID = gDATA_FIELD;
														string sDISPLAY_FIELD = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 2) + "NAME";
														new DynamicControl(ctlParameterView, sDISPLAY_FIELD).Text = Crm.Modules.ItemName(Application, sMODULE_NAME, gDATA_FIELD);
													}
													catch
													{
													}
												}
												else
												{
													ctl.Text = Sql.ToString(Request[sDATA_FIELD]);
												}
											}
											else if ( !Sql.IsEmptyString(sDEFAULT_VALUE) )
											{
												// 04/16/2011 Paul.  When the report first loads, use the default value to populate. 
												// This should work with multi-selection listboxes because we convert multiple default values to XML. 
												// 07/26/2012 Paul.  Setting a list control by index is not working due to a problem with SelectedIndex. 
												// Just for reports and charts, change to selection by value. 
												DropDownList lst = ctlParameterView.FindControl(sDATA_FIELD) as DropDownList;
												if ( lst != null )
												{
													Utils.SetSelectedValue(lst, sDEFAULT_VALUE);
												}
												else
												{
													ctl.Text = sDEFAULT_VALUE;
												}
											}
										}
									}
								}
								else
								{
									ctlDynamicButtons.ShowButton("Submit", false);
									ctlParameterView.Visible = false;
								}
								// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
								if ( !Sql.IsEmptyString(sRDL) )
								{
									// 07/24/2008 Paul.  sXML already has the data. 
									// 09/27/2010 Paul.  Don't need to load the RDL here as it will be done in RunReport. 
									//rdl.LoadRdl(sRDL);
									ctlChartView.RunReport(sRDL, sMODULE_NAME, ctlParameterView);
								}
							}
						}
					}
					else
					{
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
						ctlDynamicButtons.DisableAll();
					}
				}
				else
				{
					sMODULE_NAME = Sql.ToString(ViewState["MODULE_NAME"]);
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList.Charts") + " - " + ctlDynamicButtons.Title);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Charts";
			SetMenu(m_sMODULE);
			this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
			if ( IsPostBack )
			{
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			}
		}
		#endregion
	}
}

