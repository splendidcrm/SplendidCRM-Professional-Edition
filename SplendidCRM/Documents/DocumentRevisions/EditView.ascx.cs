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

namespace SplendidCRM.DocumentRevisions
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
		protected HyperLink       lnkFILENAME                  ;
		protected Label           txtCURRENT_REVISION          ;
		protected HtmlInputFile   fileCONTENT                  ;
		protected TextBox         txtREVISION                  ;
		protected TextBox         txtCHANGE_LOG                ;
		protected RequiredFieldValidator reqFILENAME     ;
		protected RequiredFieldValidator reqREVISION     ;

		// 10/18/2009 Paul.  Move blob logic to LoadFile. 
		// 04/24/2011 Paul.  Move LoadFile() to Crm.DocumentRevisions. 

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				if ( Page.IsValid )
				{
					try
					{
						HttpPostedFile pstCONTENT  = fileCONTENT.PostedFile;
						//die("ERROR: uploaded file was too big: max filesize: {$sugar_config['upload_maxsize']}");
						if ( pstCONTENT != null )
						{
							long lFileSize      = pstCONTENT.ContentLength;
							long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
							if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
							{
								throw(new Exception("ERROR: uploaded file was too big, max filesize: " + lUploadMaxSize.ToString()));
							}
						}
						if ( pstCONTENT != null )
						{
							// 08/20/2005 Paul.  File may not have been provided. 
							if ( pstCONTENT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstCONTENT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstCONTENT.ContentType;
								
								// 01/20/2006 Paul.  Use a transaction as multiple operations occur. 
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									string sSQL ;
									// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
									DataRow   rowCurrent = null;
									DataTable dtCurrent  = new DataTable();
									if ( !Sql.IsEmptyGuid(gID) )
									{
										sSQL = "select *               " + ControlChars.CrLf
										     + "  from vwDOCUMENTS_Edit" + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Security.Filter(cmd, m_sMODULE, "edit");
											Sql.AppendParameter(cmd, gID, "ID", false);
											// 09/04/2022 Paul.  ID may be the latest revision. 
											cmd.CommandText += "    or DOCUMENT_REVISION_ID = @DOCUMENT_REVISION_ID" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@DOCUMENT_REVISION_ID", gID);
											using ( DbDataAdapter da = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)da).SelectCommand = cmd;
												da.Fill(dtCurrent);
												if ( dtCurrent.Rows.Count > 0 )
												{
													rowCurrent = dtCurrent.Rows[0];
													// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
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
													// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
													// It is possible that the record exists, but that ACL rules prevent it from being selected. 
													gID = Guid.Empty;
												}
											}
										}
									}

									// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
									using ( IDbTransaction trn = Sql.BeginTransaction(con) )
									{
										try
										{
											Guid gRevisionID = Guid.Empty;
											SqlProcs.spDOCUMENT_REVISIONS_Insert
												( ref gRevisionID
												, gID
												, txtREVISION.Text
												, txtCHANGE_LOG.Text
												, sFILENAME
												, sFILE_EXT
												, sFILE_MIME_TYPE
												, trn
												);
											// 09/06/2008 Paul.  PostgreSQL does not require that we stream the bytes, so lets explore doing this for all platforms. 
											// 10/18/2009 Paul.  Move blob logic to LoadFile. 
											// 04/24/2011 Paul.  Move LoadFile() to Crm.DocumentRevisions. 
											Crm.DocumentRevisions.LoadFile(gRevisionID, pstCONTENT.InputStream, trn);
											trn.Commit();
										}
										catch(Exception ex)
										{
											trn.Rollback();
											throw(new Exception(ex.Message));
										}
									}
									// 08/26/2010 Paul.  Add new record to tracker. 
									sSQL = "select DOCUMENT_NAME   " + ControlChars.CrLf
									     + "  from vwDOCUMENTS_Edit" + ControlChars.CrLf
									     + " where ID = @ID        " + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@ID", gID);
										string sNAME = Sql.ToString(cmd.ExecuteScalar());
										// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
										SqlProcs.spTRACKER_Update
											( Security.USER_ID
											, m_sMODULE
											, gID
											, sNAME
											, "save"
											);
									}
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
					Response.Redirect("~/Documents/view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				// 09/21/2008 Paul.  Mono is case significant and all default pages are lower case. 
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("~/Documents/default.aspx");
				else
					Response.Redirect("~/Documents/view.aspx?ID=" + gID.ToString());
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
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				// 10/18/2010 Paul.  The required fields need to be bound manually. 
				reqFILENAME.DataBind();
				reqREVISION.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *               " + ControlChars.CrLf
							     + "  from vwDOCUMENTS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
								// 09/04/2022 Paul.  ID may be the latest revision. 
								cmd.CommandText += "    or DOCUMENT_REVISION_ID = @DOCUMENT_REVISION_ID" + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@DOCUMENT_REVISION_ID", gID);
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
											ctlDynamicButtons.Title = Sql.ToString(rdr["DOCUMENT_NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

											lnkFILENAME.Text        = Sql.ToString(rdr["DOCUMENT_NAME"]);
											lnkFILENAME.NavigateUrl = "~/Documents/Document.aspx?ID=" + Sql.ToString(rdr["DOCUMENT_REVISION_ID"]);
											txtCURRENT_REVISION.Text = Sql.ToString(rdr["REVISION"     ]);
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											// 03/03/2010 Paul.  Document Revisions needs the Cancel to be a post-back so that it can redirect to the parent document. 
											ctlDynamicButtons.AppendButtons("DocumentRevisions.EditView", Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlFooterButtons .AppendButtons("DocumentRevisions.EditView", Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
										}
										else
										{
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											// 03/03/2010 Paul.  Document Revisions needs the Cancel to be a post-back so that it can redirect to the parent document. 
											ctlDynamicButtons.AppendButtons("DocumentRevisions.EditView", Guid.Empty, null);
											ctlFooterButtons .AppendButtons("DocumentRevisions.EditView", Guid.Empty, null);
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
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						// 03/03/2010 Paul.  Document Revisions needs the Cancel to be a post-back so that it can redirect to the parent document. 
						ctlDynamicButtons.AppendButtons("DocumentRevisions.EditView", Guid.Empty, null);
						ctlFooterButtons .AppendButtons("DocumentRevisions.EditView", Guid.Empty, null);
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
			m_sMODULE = "Documents";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				// 03/03/2010 Paul.  Document Revisions needs the Cancel to be a post-back so that it can redirect to the parent document. 
				ctlDynamicButtons.AppendButtons("DocumentRevisions.EditView", Guid.Empty, null);
				ctlFooterButtons .AppendButtons("DocumentRevisions.EditView", Guid.Empty, null);
			}
		}
		#endregion
	}
}

