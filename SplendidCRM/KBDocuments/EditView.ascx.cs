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

namespace SplendidCRM.KBDocuments
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

		protected Guid            gID                       ;
		protected HtmlTable       tblMain                   ;
		protected Repeater        ctlAttachments            ;
		protected Repeater        ctlImages                 ;

		// 10/18/2009 Paul.  Move blob logic to LoadFile. 
		public static void LoadAttachmentFile(Guid gID, Stream stm, IDbTransaction trn)
		{
			if ( Sql.StreamBlobs(trn.Connection) )
			{
				const int BUFFER_LENGTH = 4*1024;
				byte[] binFILE_POINTER = new byte[16];
				SqlProcs.spKBDOCUMENTS_ATTACHMENTS_InitPointer(gID, ref binFILE_POINTER, trn);
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					int nFILE_OFFSET = 0 ;
					byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					while ( binBYTES.Length > 0 )
					{
						SqlProcs.spKBDOCUMENTS_ATTACHMENTS_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
						nFILE_OFFSET += binBYTES.Length;
						binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					}
				}
			}
			else
			{
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					byte[] binBYTES = reader.ReadBytes((int) stm.Length);
					SqlProcs.spKBDOCUMENTS_ATTACHMENTS_CONTENT_Update(gID, binBYTES, trn);
				}
			}
		}

		public static void LoadImageFile(Guid gID, Stream stm, IDbTransaction trn)
		{
			if ( Sql.StreamBlobs(trn.Connection) )
			{
				const int BUFFER_LENGTH = 4*1024;
				byte[] binFILE_POINTER = new byte[16];
				SqlProcs.spKBDOCUMENTS_IMAGES_InitPointer(gID, ref binFILE_POINTER, trn);
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					int nFILE_OFFSET = 0 ;
					byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					while ( binBYTES.Length > 0 )
					{
						SqlProcs.spKBDOCUMENTS_IMAGES_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
						nFILE_OFFSET += binBYTES.Length;
						binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					}
				}
			}
			else
			{
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					byte[] binBYTES = reader.ReadBytes((int) stm.Length);
					SqlProcs.spKBDOCUMENTS_IMAGES_CONTENT_Update(gID, binBYTES, trn);
				}
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveDuplicate" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					// 11/10/2010 Paul.  Apply Business Rules. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + LayoutEditView);
					if ( Page.IsValid )
					{
						// 09/09/2009 Paul.  Use the new function to get the table name. 
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *"               + ControlChars.CrLf
								     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, m_sMODULE, "edit");
									Sql.AppendParameter(cmd, gID, "ID", false);
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

							// 11/10/2010 Paul.  Apply Business Rules. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
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
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									SqlProcs.spKBDOCUMENTS_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID"   ).ID
										, new DynamicControl(this, rowCurrent, "NAME"               ).Text
										, new DynamicControl(this, rowCurrent, "KBDOC_APPROVER_ID"  ).ID
										, new DynamicControl(this, rowCurrent, "IS_EXTERNAL_ARTICLE").Checked
										, new DynamicControl(this, rowCurrent, "ACTIVE_DATE"        ).DateValue
										, new DynamicControl(this, rowCurrent, "EXP_DATE"           ).DateValue
										, new DynamicControl(this, rowCurrent, "STATUS"             ).SelectedValue
										, new DynamicControl(this, rowCurrent, "REVISION"           ).Text
										, new DynamicControl(this, rowCurrent, "DESCRIPTION"        ).Text
										, new DynamicControl(this, rowCurrent, "TEAM_ID"            ).ID
										, new DynamicControl(this, rowCurrent, "TEAM_SET_LIST"      ).Text
										, new DynamicControl(this, rowCurrent, "KBTAG_SET_LIST"     ).Text
										// 05/12/2016 Paul.  Add Tags module. 
										, new DynamicControl(this, rowCurrent, "TAG_SET_NAME"       ).Text
										// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
										, new DynamicControl(this, rowCurrent, "ASSIGNED_SET_LIST"  ).Text
										, trn
										);
									foreach ( string sHTML_FIELD_NAME in Request.Files.AllKeys )
									{
										if ( sHTML_FIELD_NAME.StartsWith("attachment") )
										{
											HttpPostedFile pstATTACHMENT = Request.Files[sHTML_FIELD_NAME];
											if ( pstATTACHMENT != null )
											{
												long lFileSize      = pstATTACHMENT.ContentLength;
												long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
												if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
												{
													throw(new Exception("ERROR: uploaded file was too big: max filesize: " + lUploadMaxSize.ToString()));
												}
												// 08/20/2005 Paul.  File may not have been provided. 
												if ( pstATTACHMENT.FileName.Length > 0 )
												{
													string sFILENAME       = Path.GetFileName (pstATTACHMENT.FileName);
													string sFILE_EXT       = Path.GetExtension(sFILENAME);
													string sFILE_MIME_TYPE = pstATTACHMENT.ContentType;
													
													Guid gATTACHMENT_ID = Guid.Empty;
													SqlProcs.spKBDOCUMENTS_ATTACHMENTS_Insert(ref gATTACHMENT_ID, gID, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
													// 10/26/2009 Paul.  Move blob logic to LoadFile. 
													LoadAttachmentFile(gATTACHMENT_ID, pstATTACHMENT.InputStream, trn);
												}
											}
										}
										else if ( sHTML_FIELD_NAME.StartsWith("image") )
										{
											HttpPostedFile pstIMAGE = Request.Files[sHTML_FIELD_NAME];
											if ( pstIMAGE != null )
											{
												long lFileSize      = pstIMAGE.ContentLength;
												long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
												if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
												{
													throw(new Exception("ERROR: uploaded file was too big: max filesize: " + lUploadMaxSize.ToString()));
												}
												// 08/20/2005 Paul.  File may not have been provided. 
												if ( pstIMAGE.FileName.Length > 0 )
												{
													string sFILENAME       = Path.GetFileName (pstIMAGE.FileName);
													string sFILE_EXT       = Path.GetExtension(sFILENAME);
													string sFILE_MIME_TYPE = pstIMAGE.ContentType;
												
													Guid gIMAGE_ID = Guid.Empty;
													SqlProcs.spKBDOCUMENTS_IMAGES_Insert(ref gIMAGE_ID, gID, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
													// 10/26/2009 Paul.  Move blob logic to LoadFile. 
													LoadImageFile(gIMAGE_ID, pstIMAGE.InputStream, trn);
												}
											}
										}
									}

									// 09/13/2011 Paul.  Deleted attachments need to have their relationship removed. 
									DataTable dtAttachments = ViewState["Attachments"] as DataTable;
									if ( dtAttachments != null )
									{
										foreach ( DataRow row in dtAttachments.Rows )
										{
											if ( row.RowState == DataRowState.Deleted )
											{
												// 09/13/2011 Paul.  Deleted row information cannot be accessed through the row.
												// Need to get the Original version. 
												Guid gNOTE_ATTACHMENT_ID = Sql.ToGuid(row["ID", DataRowVersion.Original]);
												SqlProcs.spNOTE_ATTACHMENTS_Delete(gNOTE_ATTACHMENT_ID, trn);
											}
										}
									}

									// 09/13/2011 Paul.  Deleted images need to have their relationship removed. 
									DataTable dtImages = ViewState["Images"] as DataTable;
									if ( dtImages != null )
									{
										foreach ( DataRow row in dtImages.Rows )
										{
											if ( row.RowState == DataRowState.Deleted )
											{
												// 09/13/2011 Paul.  Deleted row information cannot be accessed through the row.
												// Need to get the Original version. 
												Guid gEMAIL_IMAGE_ID = Sql.ToGuid(row["ID", DataRowVersion.Original]);
												SqlProcs.spEMAIL_IMAGES_Delete(gEMAIL_IMAGE_ID, trn);
											}
										}
									}

									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									// 08/26/2010 Paul.  Add new record to tracker. 
									// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
									SqlProcs.spTRACKER_Update
										( Security.USER_ID
										, m_sMODULE
										, gID
										, new DynamicControl(this, rowCurrent, "NAME").Text
										, "save"
										, trn
										);
									trn.Commit();
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
							// 11/10/2010 Paul.  Apply Business Rules. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
						}
						
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
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
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
			// 09/13/2011 Paul.  Store the attachments in ViewState so that we can manipulate the table. 
			else if ( e.CommandName == "Attachments.Delete" )
			{
				Guid gNOTE_ATTACHMENT_ID = Sql.ToGuid(e.CommandArgument);
				DataTable dt = ViewState["Attachments"] as DataTable;
				if ( dt != null && !Sql.IsEmptyGuid(gNOTE_ATTACHMENT_ID) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						if ( gNOTE_ATTACHMENT_ID == Sql.ToGuid(row["ID"]) )
						{
							row.Delete();
						}
					}
					// 09/13/2011 Paul.  Do not accept changes so that we can use the deleted flag to update the relationships. 
					ctlAttachments.DataSource = dt.DefaultView;
					ctlAttachments.DataBind();
					ViewState["Attachments"] = dt;
				}
			}
			// 09/13/2011 Paul.  Store the attachments in ViewState so that we can manipulate the table. 
			else if ( e.CommandName == "Images.Delete" )
			{
				Guid gNOTE_ATTACHMENT_ID = Sql.ToGuid(e.CommandArgument);
				DataTable dt = ViewState["Images"] as DataTable;
				if ( dt != null && !Sql.IsEmptyGuid(gNOTE_ATTACHMENT_ID) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						if ( gNOTE_ATTACHMENT_ID == Sql.ToGuid(row["ID"]) )
						{
							row.Delete();
						}
					}
					// 09/13/2011 Paul.  Do not accept changes so that we can use the deleted flag to update the relationships. 
					ctlImages.DataSource = dt.DefaultView;
					ctlImages.DataBind();
					ViewState["Images"] = dt;
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

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
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
								Security.Filter(cmd, m_sMODULE, "edit");
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
											// 11/11/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											TextBox txtNAME = this.FindControl("NAME") as TextBox;
											if ( txtNAME != null )
												txtNAME.Focus();

											// 03/04/2006 Paul.  Revision is editable if we are duplicating the document. 
											/*
											if ( Sql.IsEmptyGuid(gDuplicateID) )
											{
												// 12/06/2005 Paul.  The Revision is not editable.  SugarCRM 3.5 allows editing, but does not honor any changes. 
												TextBox txtREVISION = FindControl("REVISION") as TextBox;
												if ( txtREVISION != null )
												{
													HtmlTableCell td = txtREVISION.Parent as HtmlTableCell;
													if ( td != null )
													{
														txtREVISION.ReadOnly = true;
													}
												}
											}
											*/
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
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
							sSQL = "select *                             " + ControlChars.CrLf
							     + "  from vwKBDOCUMENTS_ATTACHMENTS     " + ControlChars.CrLf
							     + " where KBDOCUMENT_ID = @KBDOCUMENT_ID" + ControlChars.CrLf
							     + " order by DATE_ENTERED               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@KBDOCUMENT_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwKBDOCUMENTS_ATTACHMENTS", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										ctlAttachments.DataSource = dt.DefaultView;
										ctlAttachments.DataBind();
										// 09/13/2011 Paul.  Store the attachments in ViewState so that we can manipulate the table. 
										ViewState["Attachments"] = dt;
									}
								}
							}
							sSQL = "select *                             " + ControlChars.CrLf
							     + "  from vwKBDOCUMENTS_IMAGES          " + ControlChars.CrLf
							     + " where KBDOCUMENT_ID = @KBDOCUMENT_ID" + ControlChars.CrLf
							     + " order by DATE_ENTERED               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@KBDOCUMENT_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwKBDOCUMENTS_IMAGES", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										ctlImages.DataSource = dt.DefaultView;
										ctlImages.DataBind();
										// 09/13/2011 Paul.  Store the attachments in ViewState so that we can manipulate the table. 
										ViewState["Images"] = dt;
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
						TextBox txtNAME = this.FindControl("NAME") as TextBox;
						if ( txtNAME != null )
							txtNAME.Focus();
						TextBox txtREVISION = this.FindControl("REVISION") as TextBox;
						if ( txtREVISION != null )
							txtREVISION.Text = "1";
						Guid gCASE_ID = Sql.ToGuid(Request["CASE_ID"]);
						if ( !Sql.IsEmptyGuid(gCASE_ID) )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									sSQL = "select *      " + ControlChars.CrLf
									     + "  from vwCASES" + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Security.Filter(cmd, m_sMODULE, "view");
									Sql.AppendParameter(cmd, gCASE_ID, "ID");

									if ( bDebug )
										RegisterClientScriptBlock("vwCASES", Sql.ClientScriptBlock(cmd));

									using ( IDataReader rdr = cmd.ExecuteReader() )
									{
										if ( rdr.Read() )
										{
											txtNAME.Text = Sql.ToString(rdr["NAME"]);
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
			m_sMODULE = "KBDocuments";
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
