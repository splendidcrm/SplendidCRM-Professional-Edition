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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
// 09/18/2011 Paul.  Upgrade to CKEditor 3.6.2. 
using CKEditor.NET;

namespace SplendidCRM.Posts
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		// 01/13/2010 Paul.  Add footer buttons. 
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;
		protected HiddenField     THREAD_ID                    ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			Guid gTHREAD_ID = Sql.ToGuid(THREAD_ID.Value);
			if ( e.CommandName == "Save" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					// 11/10/2010 Paul.  Apply Business Rules. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + LayoutEditView);
					if ( Page.IsValid )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 11/10/2010 Paul.  Apply Business Rules. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							DataRow rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
							
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									SqlProcs.spPOSTS_Update(ref gID
										, gTHREAD_ID
										, new DynamicControl(this, "TITLE"           ).Text
										, new DynamicControl(this, "DESCRIPTION_HTML").Text
										, trn
										);
									// 08/26/2010 Paul.  Add new record to tracker. 
									// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
									SqlProcs.spTRACKER_Update
										( Security.USER_ID
										, m_sMODULE
										, gID
										, new DynamicControl(this, "TITLE").Text
										, "save"
										, trn
										);
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
							// 11/10/2010 Paul.  Apply Business Rules. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
						}
						
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						else if ( !Sql.IsEmptyGuid(gTHREAD_ID) )
							Response.Redirect("~/Threads/view.aspx?ID=" + gTHREAD_ID.ToString());
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
				if ( !Sql.IsEmptyGuid(gTHREAD_ID) )
					Response.Redirect("~/Threads/view.aspx?ID=" + gTHREAD_ID.ToString());
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
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
							// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
							sSQL = "select *"               + ControlChars.CrLf
							     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
							     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
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
											// 11/11/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["TITLE"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											THREAD_ID.Value = Sql.ToString(rdr["THREAD_ID"]);
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											// 02/18/2009 Paul.  On load, the focus should be set to the NAME field. 
											TextBox txtTITLE = this.FindControl("TITLE") as TextBox;
											if ( txtTITLE != null )
												txtTITLE.Focus();
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
										}
										else
										{
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlFooterButtons .DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						// 02/18/2009 Paul.  On load, the focus should be set to the NAME field. 
						// 09/18/2011 Paul.  Upgrade to CKEditor 3.6.2. 
						CKEditorControl txtDESCRIPTION = this.FindControl("DESCRIPTION") as CKEditorControl;
						TextBox txtTITLE = this.FindControl("TITLE") as TextBox;
						if ( txtTITLE != null )
							txtTITLE.Focus();

						Guid gREPLY_ID  = Sql.ToGuid(Request["REPLY_ID" ]);
						Guid gTHREAD_ID = Sql.ToGuid(Request["THREAD_ID"]);
						THREAD_ID.Value = gTHREAD_ID.ToString();

						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							if ( !Sql.IsEmptyGuid(gTHREAD_ID) )
							{
								sSQL = "select *              " + ControlChars.CrLf
								     + "  from vwTHREADS_Edit " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
									Security.Filter(cmd, "Threads", "view");
									Sql.AppendParameter(cmd, gTHREAD_ID, "ID", false);
									con.Open();

									if ( bDebug )
										RegisterClientScriptBlock("vwTHREADS_Edit", Sql.ClientScriptBlock(cmd));

									// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
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
												ctlDynamicButtons.Title = Sql.ToString(rdr["TITLE"]);
												SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
												Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
												ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

												THREAD_ID.Value = gTHREAD_ID.ToString();
												txtTITLE.Text = L10n.Term("Posts.LBL_REPLY_PREFIX") + Sql.ToString (rdr["TITLE"]);
												if ( Sql.ToBoolean(Request["QUOTE"]) )
												{
													string sCREATED_BY    = Sql.ToString(rdr["CREATED_BY"      ]);
													string sORIGINAL_TEXT = Sql.ToString(rdr["DESCRIPTION_HTML"]);
													string sQUOTE_FORMAT  = L10n.Term("Posts.QUOTE_FORMAT");
													StringBuilder sb = new StringBuilder();
													sb.AppendLine("<br />"          );
													sb.AppendLine("<br />"          );
													sb.AppendLine("<strong><em>" + String.Format(sQUOTE_FORMAT, sCREATED_BY) + "</em></strong> <br />");
													sb.AppendLine("<table width=\"90%\" border=\"1\" cellspacing=\"0\" cellpadding=\"4\" style=\"FONT-STYLE: italic\" align=\"center\">");
													sb.AppendLine("    <tr>"    );
													sb.Append("        <td>");
													sb.Append(sORIGINAL_TEXT);
													sb.AppendLine("</td>"           );
													sb.AppendLine("    </tr>"       );
													sb.AppendLine("</table>"        );
													sb.AppendLine("<br />"          );
													sb.AppendLine("<br />"          );
													txtDESCRIPTION.Text = sb.ToString();
												}
											}
										}
									}
								}
							}
							else
							{
								sSQL = "select *              " + ControlChars.CrLf
								     + "  from vwPOSTS_Edit   " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
									Security.Filter(cmd, "Posts", "view");
									Sql.AppendParameter(cmd, gREPLY_ID, "ID", false);
									con.Open();

									if ( bDebug )
										RegisterClientScriptBlock("vwPOSTS_Edit", Sql.ClientScriptBlock(cmd));

									// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
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
												ctlDynamicButtons.Title = Sql.ToString(rdr["TITLE"]);
												SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
												Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
												ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

												THREAD_ID.Value = Sql.ToString(rdr["THREAD_ID"]);
												txtTITLE.Text = L10n.Term("Posts.LBL_REPLY_PREFIX") + Sql.ToString (rdr["TITLE"]);
												if ( Sql.ToBoolean(Request["QUOTE"]) )
												{
													string sCREATED_BY    = Sql.ToString(rdr["CREATED_BY"      ]);
													string sORIGINAL_TEXT = Sql.ToString(rdr["DESCRIPTION_HTML"]);
													string sQUOTE_FORMAT  = L10n.Term("Posts.QUOTE_FORMAT");
													StringBuilder sb = new StringBuilder();
													sb.AppendLine("<br />"          );
													sb.AppendLine("<br />"          );
													sb.AppendLine("<strong><em>" + String.Format(sQUOTE_FORMAT, sCREATED_BY) + "</em></strong> <br />");
													sb.AppendLine("<table width=\"90%\" border=\"1\" cellspacing=\"0\" cellpadding=\"4\" style=\"FONT-STYLE: italic\" align=\"center\">");
													sb.AppendLine("    <tr>"    );
													sb.Append("        <td>");
													sb.Append(sORIGINAL_TEXT);
													sb.AppendLine("</td>"           );
													sb.AppendLine("    </tr>"       );
													sb.AppendLine("</table>"        );
													sb.AppendLine("<br />"          );
													sb.AppendLine("<br />"          );
													txtDESCRIPTION.Text = sb.ToString();
												}
											}
										}
									}
								}
							}
						}
						// 11/10/2010 Paul.  Apply Business Rules. 
						this.ApplyEditViewNewEventRules(m_sMODULE + "." + LayoutEditView);
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
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
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			m_sMODULE = "Posts";
			// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
			m_sVIEW_NAME = "vw" + Crm.Modules.TableName(m_sMODULE) + "_Edit";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				// 11/10/2010 Paul.  Make sure to add the RulesValidator early in the pipeline. 
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}
