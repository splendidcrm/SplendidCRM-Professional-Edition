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
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Diagnostics;
using SplendidCRM.ReportDesigner;

namespace SplendidCRM.Reports
{
	/// <summary>
	/// Summary description for ViewEmbedded.
	/// </summary>
	public class ViewEmbedded : SplendidPage
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected ParameterView            ctlParameterView ;
		protected ReportView               ctlReportView    ;

		protected Guid             gID               ;
		protected string           m_sMODULE         ;

		protected DataRow          rdr               ;
		protected string           sRDL              ;
		protected string           sMODULE_NAME      ;
		protected string           sREPORT_NAME      ;
		protected Guid             gASSIGNED_USER_ID ;
		protected DataTable        dtReportParameters;
		// 02/12/2021 Paul.  Provide a way to require submit before running report. 
		protected bool             bRequireSubmit = false;
		// 03/09/2021 Paul.  Multi-selection listboxes are not displaying their selected values, so manually correct. 
		protected DataTable        dtCorrected       ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Attachment" )
				{
					if ( !Sql.IsEmptyGuid(gID) && !Sql.IsEmptyString(sRDL) )
					{
						RdlDocument rdl = new RdlDocument();
						rdl.LoadRdl(sRDL);
						
						Guid gNOTE_ID = Guid.Empty;
						string sDESCRIPTION = sREPORT_NAME + " " + DateTime.Now.ToString();
						AttachmentView.SendAsAttachment(this.Context, ctlParameterView, L10n, T10n, gID, sRDL, "PDF", sMODULE_NAME, Guid.Empty, sREPORT_NAME, sDESCRIPTION, ref gNOTE_ID);
						Response.Redirect("~/Emails/edit.aspx?NOTE_ID=" + gNOTE_ID.ToString() );
					}
				}
				else if ( e.CommandName == "Submit" )
				{
					if ( !Sql.IsEmptyString(sRDL) )
					{
						ctlReportView.RunReport(gID, sRDL, sMODULE_NAME, ctlParameterView);
					}
				}
				else if ( e.CommandName == "Refresh" )
				{
					if ( !Sql.IsEmptyString(sRDL) )
					{
						ctlReportView.RunReport(gID, sRDL, sMODULE_NAME, ctlParameterView);
					}
				}
				// 02/24/2021 Paul.  Add the clear button. 
				else if ( e.CommandName == "Clear" )
				{
					Response.Redirect("view_embedded.aspx?ID=" + gID.ToString() );
				}
			}
			catch(Exception ex)
			{
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void LoadReport()
		{
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !Sql.IsEmptyGuid(gID) )
				{
					DataTable dtReport = SplendidCache.Report(gID);
					if ( dtReport != null && dtReport.Rows.Count > 0 )
					{
						DataRow rdr = dtReport.Rows[0];
						sRDL              = Sql.ToString(rdr["RDL"             ]);
						sMODULE_NAME      = Sql.ToString(rdr["MODULE_NAME"     ]);
						sREPORT_NAME      = Sql.ToString(rdr["NAME"            ]);
						gASSIGNED_USER_ID = Sql.ToGuid  (rdr["ASSIGNED_USER_ID"]);
						dtReportParameters = SplendidCache.ReportParameters(gID, Security.USER_ID);
						if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
						{
							DataTable dt = SplendidCache.ReportParametersEditView(gID, Security.USER_ID);
							ctlParameterView.AppendEditViewFields(dt);
						}
					}
				}
				else if ( !Sql.IsEmptyString(Request["ReportDesign"]) )
				{
					JavaScriptSerializer json = new JavaScriptSerializer();
					// 12/12/2014 Paul.  No reason to limit the Json result. 
					json.MaxJsonLength = int.MaxValue;
					
					bool bDesignChart = false;
					string sReportDesign = Sql.ToString(Request["ReportDesign"]);
					RdlDocument rdl = new RdlDocument(String.Empty, String.Empty, bDesignChart);
					bool bPrimaryKeyOnly   = false;
					bool bUseSQLParameters = true ;
					bool bUserSpecific     = false;
					int  nSelectedColumns  = 0    ;
					ReportDesign rd = json.Deserialize<ReportDesign>(sReportDesign);
					QueryDesigner.DisplayColumnsUpdate(rdl, rd);
					QueryDesigner.UpdateDataTable(rdl);
					string sReportSQL = QueryDesigner.BuildReportSQL(Application, L10n, rdl, rd, bPrimaryKeyOnly, bUseSQLParameters, bUserSpecific, ref nSelectedColumns);
					sRDL = rdl.OuterXml;
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
				dtCorrected = new DataTable();
				dtCorrected.Columns.Add("NAME" );
				dtCorrected.Columns.Add("VALUE");
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyString(sRDL) )
					{
						// 02/24/2021 Paul.  Add the clear button. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + ".DetailViewWithClear", gASSIGNED_USER_ID, rdr);
						ctlDynamicButtons.ShowButton("Attachment"      , false);
						ctlDynamicButtons.ShowButton("Attachment-PDF"  , false);
						ctlDynamicButtons.ShowButton("Attachment-Excel", false);
						ctlDynamicButtons.ShowButton("Attachment-Word" , false);
						ctlDynamicButtons.ShowButton("Attachment-Image", false);
						
						if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
						{
							foreach ( DataRow rowParameter in dtReportParameters.Rows )
							{
								string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]);
								string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
								// 02/12/2021 Paul.  Provide a way to require submit before running report. 
								if ( sDATA_FIELD == "RequireSubmit" )
								{
									bRequireSubmit = Sql.ToBoolean(sDEFAULT_VALUE);
								}
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
										ListControl lst = ctlParameterView.FindControl(sDATA_FIELD) as ListControl;
										if ( lst != null )
										{
											// 08/09/2014 Paul.  If we have a listbox and a collection of values, then set them individually. 
											if ( sDEFAULT_VALUE.StartsWith("<?xml") )
											{
												XmlDocument xml = new XmlDocument();
												// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
												// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
												// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
												xml.XmlResolver = null;
												xml.LoadXml(sDEFAULT_VALUE);
												XmlNodeList nlValues = xml.DocumentElement.SelectNodes("Value");
												foreach ( XmlNode xValue in nlValues )
												{
													ListItem itm = lst.Items.FindByValue(xValue.InnerText);
													if ( itm != null )
													{
														itm.Selected = true;
														// 03/09/2021 Paul.  Multi-selection listboxes are not displaying their selected values, so manually correct. 
														DataRow rowCorrected = dtCorrected.NewRow();
														dtCorrected.Rows.Add(rowCorrected);
														rowCorrected["NAME" ] = lst.ClientID;
														rowCorrected["VALUE"] = xValue.InnerText;
													}
												}
											}
											else
											{
												Utils.SetSelectedValue(lst, sDEFAULT_VALUE);
											}
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
						if ( !Sql.IsEmptyString(sRDL) )
						{
							// 02/12/2021 Paul.  Provide a way to require submit before running report. 
							if ( !bRequireSubmit )
								ctlReportView.RunReport(gID, sRDL, sMODULE_NAME, ctlParameterView);
							else
								ctlDynamicButtons.ErrorText = L10n.Term("Reports.LBL_PRESS_SUBMIT_TO_RUN");
						}
					}
					else
					{
						// 02/24/2021 Paul.  Add the clear button. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + ".DetailViewWithClear", Guid.Empty, null);
						ctlDynamicButtons.HideAll();
					}
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
			ctlReportView.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Reports";
			LoadReport();
			if ( IsPostBack )
			{
				// 02/24/2021 Paul.  Add the clear button. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".DetailViewWithClear", Guid.Empty, null);
			}
		}
		#endregion
	}
}
