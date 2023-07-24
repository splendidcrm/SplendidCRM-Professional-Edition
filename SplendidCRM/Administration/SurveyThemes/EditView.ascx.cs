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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.SurveyThemes
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					if ( Page.IsValid )
					{
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									Guid gImageID = Guid.Empty;
									HtmlInputFile PAGE_BACKGROUND_IMAGE_File = this.FindControl("PAGE_BACKGROUND_IMAGE_File") as HtmlInputFile;
									if ( PAGE_BACKGROUND_IMAGE_File != null )
									{
										HttpPostedFile pstATTACHMENT = PAGE_BACKGROUND_IMAGE_File.PostedFile;
										if ( pstATTACHMENT != null )
										{
											if ( pstATTACHMENT.FileName.Length > 0 )
											{
												string sFILENAME = String.Empty;
												SplendidCRM.FileBrowser.FileWorkerUtils.LoadImage(ref gImageID, ref sFILENAME, PAGE_BACKGROUND_IMAGE_File.UniqueID, trn);
												if ( !Sql.IsEmptyGuid(gImageID) )
												{
													new DynamicControl(this, "PAGE_BACKGROUND_IMAGE").Text = "~/Images/EmailImage.aspx?ID=" + gImageID.ToString();
												}
											}
										}
									}
									SqlProcs.spSURVEY_THEMES_Update
										( ref gID
										, new DynamicControl(this, "NAME"                        ).Text
										, new DynamicControl(this, "SURVEY_FONT_FAMILY"          ).Text
										, new DynamicControl(this, "LOGO_BACKGROUND"             ).Text
										, new DynamicControl(this, "SURVEY_BACKGROUND"           ).Text
										, new DynamicControl(this, "SURVEY_TITLE_TEXT_COLOR"     ).Text
										, new DynamicControl(this, "SURVEY_TITLE_FONT_SIZE"      ).SelectedValue
										, new DynamicControl(this, "SURVEY_TITLE_FONT_STYLE"     ).SelectedValue
										, new DynamicControl(this, "SURVEY_TITLE_FONT_WEIGHT"    ).SelectedValue
										, new DynamicControl(this, "SURVEY_TITLE_DECORATION"     ).SelectedValue
										, new DynamicControl(this, "SURVEY_TITLE_BACKGROUND"     ).Text
										, new DynamicControl(this, "PAGE_TITLE_TEXT_COLOR"       ).Text
										, new DynamicControl(this, "PAGE_TITLE_FONT_SIZE"        ).SelectedValue
										, new DynamicControl(this, "PAGE_TITLE_FONT_STYLE"       ).SelectedValue
										, new DynamicControl(this, "PAGE_TITLE_FONT_WEIGHT"      ).SelectedValue
										, new DynamicControl(this, "PAGE_TITLE_DECORATION"       ).SelectedValue
										, new DynamicControl(this, "PAGE_TITLE_BACKGROUND"       ).Text
										, new DynamicControl(this, "PAGE_DESCRIPTION_TEXT_COLOR" ).Text
										, new DynamicControl(this, "PAGE_DESCRIPTION_FONT_SIZE"  ).SelectedValue
										, new DynamicControl(this, "PAGE_DESCRIPTION_FONT_STYLE" ).SelectedValue
										, new DynamicControl(this, "PAGE_DESCRIPTION_FONT_WEIGHT").SelectedValue
										, new DynamicControl(this, "PAGE_DESCRIPTION_DECORATION" ).SelectedValue
										, new DynamicControl(this, "PAGE_DESCRIPTION_BACKGROUND" ).Text
										, new DynamicControl(this, "QUESTION_HEADING_TEXT_COLOR" ).Text
										, new DynamicControl(this, "QUESTION_HEADING_FONT_SIZE"  ).SelectedValue
										, new DynamicControl(this, "QUESTION_HEADING_FONT_STYLE" ).SelectedValue
										, new DynamicControl(this, "QUESTION_HEADING_FONT_WEIGHT").SelectedValue
										, new DynamicControl(this, "QUESTION_HEADING_DECORATION" ).SelectedValue
										, new DynamicControl(this, "QUESTION_HEADING_BACKGROUND" ).Text
										, new DynamicControl(this, "QUESTION_CHOICE_TEXT_COLOR"  ).Text
										, new DynamicControl(this, "QUESTION_CHOICE_FONT_SIZE"   ).SelectedValue
										, new DynamicControl(this, "QUESTION_CHOICE_FONT_STYLE"  ).SelectedValue
										, new DynamicControl(this, "QUESTION_CHOICE_FONT_WEIGHT" ).SelectedValue
										, new DynamicControl(this, "QUESTION_CHOICE_DECORATION"  ).SelectedValue
										, new DynamicControl(this, "QUESTION_CHOICE_BACKGROUND"  ).Text
										, new DynamicControl(this, "PROGRESS_BAR_PAGE_WIDTH"     ).Text
										, new DynamicControl(this, "PROGRESS_BAR_COLOR"          ).Text
										, new DynamicControl(this, "PROGRESS_BAR_BORDER_COLOR"   ).Text
										, new DynamicControl(this, "PROGRESS_BAR_BORDER_WIDTH"   ).Text
										, new DynamicControl(this, "PROGRESS_BAR_TEXT_COLOR"     ).Text
										, new DynamicControl(this, "PROGRESS_BAR_FONT_SIZE"      ).SelectedValue
										, new DynamicControl(this, "PROGRESS_BAR_FONT_STYLE"     ).SelectedValue
										, new DynamicControl(this, "PROGRESS_BAR_FONT_WEIGHT"    ).SelectedValue
										, new DynamicControl(this, "PROGRESS_BAR_DECORATION"     ).SelectedValue
										, new DynamicControl(this, "PROGRESS_BAR_BACKGROUND"     ).Text
										, new DynamicControl(this, "ERROR_TEXT_COLOR"            ).Text
										, new DynamicControl(this, "ERROR_FONT_SIZE"             ).SelectedValue
										, new DynamicControl(this, "ERROR_FONT_STYLE"            ).SelectedValue
										, new DynamicControl(this, "ERROR_FONT_WEIGHT"           ).SelectedValue
										, new DynamicControl(this, "ERROR_DECORATION"            ).SelectedValue
										, new DynamicControl(this, "ERROR_BACKGROUND"            ).Text
										, new DynamicControl(this, "EXIT_LINK_TEXT_COLOR"        ).Text
										, new DynamicControl(this, "EXIT_LINK_FONT_SIZE"         ).SelectedValue
										, new DynamicControl(this, "EXIT_LINK_FONT_STYLE"        ).SelectedValue
										, new DynamicControl(this, "EXIT_LINK_FONT_WEIGHT"       ).SelectedValue
										, new DynamicControl(this, "EXIT_LINK_DECORATION"        ).SelectedValue
										, new DynamicControl(this, "EXIT_LINK_BACKGROUND"        ).Text
										, new DynamicControl(this, "REQUIRED_TEXT_COLOR"         ).Text
										, new DynamicControl(this, "DESCRIPTION"                 ).Text
										// 11/12/2018 Paul.  Add custom styles field to allow any style change. 
										, new DynamicControl(this, "CUSTOM_STYLES"               ).Text
										// 04/09/2019 Paul.  Add Survey Theme Page Background. 
										, new DynamicControl(this, "PAGE_BACKGROUND_IMAGE"       ).Text
										, new DynamicControl(this, "PAGE_BACKGROUND_POSITION"    ).Text
										, new DynamicControl(this, "PAGE_BACKGROUND_REPEAT"      ).Text
										, new DynamicControl(this, "PAGE_BACKGROUND_SIZE"        ).Text
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
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
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwSURVEY_THEMES" + ControlChars.CrLf
							     + " where ID = @ID       " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
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
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
					}
				}
				else
				{
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
			m_sMODULE = "SurveyThemes";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}
