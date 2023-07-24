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

namespace SplendidCRM.Administration.ACLRoles
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
		protected AccessView ctlAccessView;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					if ( Page.IsValid )
					{
						DataTable dtACLACCESS = null;
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 03/09/2010 Paul.  Admin roles are managed separately. 
							// 09/26/2017 Paul.  Add Archive access right. 
							sSQL = "select MODULE_NAME          " + ControlChars.CrLf
							     + "     , DISPLAY_NAME         " + ControlChars.CrLf
							     + "     , ACLACCESS_ADMIN      " + ControlChars.CrLf
							     + "     , ACLACCESS_ACCESS     " + ControlChars.CrLf
							     + "     , ACLACCESS_VIEW       " + ControlChars.CrLf
							     + "     , ACLACCESS_LIST       " + ControlChars.CrLf
							     + "     , ACLACCESS_EDIT       " + ControlChars.CrLf
							     + "     , ACLACCESS_DELETE     " + ControlChars.CrLf
							     + "     , ACLACCESS_IMPORT     " + ControlChars.CrLf
							     + "     , ACLACCESS_EXPORT     " + ControlChars.CrLf
							     + "     , ACLACCESS_ARCHIVE    " + ControlChars.CrLf
							     + "     , IS_ADMIN             " + ControlChars.CrLf
							     + "  from vwACL_ACCESS_ByModule" + ControlChars.CrLf
							     + " order by MODULE_NAME       " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dtACLACCESS = new DataTable();
									da.Fill(dtACLACCESS);
								}
							}

							// 09/09/2009 Paul.  Use the new function to get the table name. 
							string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
							DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									SqlProcs.spACL_ROLES_Update
										( ref gID
										, new DynamicControl(this, "NAME"       ).Text
										, new DynamicControl(this, "DESCRIPTION").Text
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									
									// 03/09/2010 Paul.  Admin roles are managed separately. 
									DataView vwMain = new DataView(dtACLACCESS);
									vwMain.RowFilter = "IS_ADMIN = 0 and MODULE_NAME <> 'Teams'";
									foreach(DataRowView row in vwMain)
									{
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										// 04/25/2006 Paul.  FindControl needs to be executed on the DataGridItem.  I'm not sure why.
										DropDownList lstAccess = ctlAccessView.FindACLControl(sMODULE_NAME, "access");
										DropDownList lstView   = ctlAccessView.FindACLControl(sMODULE_NAME, "view"  );
										DropDownList lstList   = ctlAccessView.FindACLControl(sMODULE_NAME, "list"  );
										DropDownList lstEdit   = ctlAccessView.FindACLControl(sMODULE_NAME, "edit"  );
										DropDownList lstDelete = ctlAccessView.FindACLControl(sMODULE_NAME, "delete");
										DropDownList lstImport = ctlAccessView.FindACLControl(sMODULE_NAME, "import");
										DropDownList lstExport = ctlAccessView.FindACLControl(sMODULE_NAME, "export");
										// 09/26/2017 Paul.  Add Archive access right. 
										DropDownList lstArchive = ctlAccessView.FindACLControl(sMODULE_NAME, "archive");
										Guid gActionAccessID = Guid.Empty;
										Guid gActionViewID   = Guid.Empty;
										Guid gActionListID   = Guid.Empty;
										Guid gActionEditID   = Guid.Empty;
										Guid gActionDeleteID = Guid.Empty;
										Guid gActionImportID = Guid.Empty;
										Guid gActionExportID = Guid.Empty;
										// 09/26/2017 Paul.  Add Archive access right. 
										Guid gActionArchiveID = Guid.Empty;
										if ( lstAccess != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionAccessID, gID, "access", sMODULE_NAME, Sql.ToInteger(lstAccess.SelectedValue), trn);
										if ( lstView   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionViewID  , gID, "view"  , sMODULE_NAME, Sql.ToInteger(lstView  .SelectedValue), trn);
										if ( lstList   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionListID  , gID, "list"  , sMODULE_NAME, Sql.ToInteger(lstList  .SelectedValue), trn);
										if ( lstEdit   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionEditID  , gID, "edit"  , sMODULE_NAME, Sql.ToInteger(lstEdit  .SelectedValue), trn);
										if ( lstDelete != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionDeleteID, gID, "delete", sMODULE_NAME, Sql.ToInteger(lstDelete.SelectedValue), trn);
										if ( lstImport != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionImportID, gID, "import", sMODULE_NAME, Sql.ToInteger(lstImport.SelectedValue), trn);
										if ( lstExport != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionExportID, gID, "export", sMODULE_NAME, Sql.ToInteger(lstExport.SelectedValue), trn);
										// 09/26/2017 Paul.  Add Archive access right. 
										if ( lstArchive != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionArchiveID, gID, "archive", sMODULE_NAME, Sql.ToInteger(lstArchive.SelectedValue), trn);
										//break;
									}
									// 03/09/2010 Paul.  Admin roles are managed separately. 
									bool bAllowAdminRoles = Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]);
									if ( bAllowAdminRoles )
									{
										vwMain.RowFilter = "IS_ADMIN = 1 or MODULE_NAME = 'Teams'";
										foreach(DataRowView row in vwMain)
										{
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											// 04/25/2006 Paul.  FindControl needs to be executed on the DataGridItem.  I'm not sure why.
											DropDownList lstAccess = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "access");
											DropDownList lstView   = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "view"  );
											DropDownList lstList   = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "list"  );
											DropDownList lstEdit   = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "edit"  );
											DropDownList lstDelete = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "delete");
											DropDownList lstImport = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "import");
											DropDownList lstExport = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "export");
											// 09/26/2017 Paul.  Add Archive access right. 
											DropDownList lstArchive = ctlAccessView.FindACLControl_Admin(sMODULE_NAME, "archive");
											Guid gActionAccessID = Guid.Empty;
											Guid gActionViewID   = Guid.Empty;
											Guid gActionListID   = Guid.Empty;
											Guid gActionEditID   = Guid.Empty;
											Guid gActionDeleteID = Guid.Empty;
											Guid gActionImportID = Guid.Empty;
											Guid gActionExportID = Guid.Empty;
											// 09/26/2017 Paul.  Add Archive access right. 
											Guid gActionArchiveID = Guid.Empty;
											if ( lstAccess != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionAccessID, gID, "access", sMODULE_NAME, Sql.ToInteger(lstAccess.SelectedValue), trn);
											if ( lstView   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionViewID  , gID, "view"  , sMODULE_NAME, Sql.ToInteger(lstView  .SelectedValue), trn);
											if ( lstList   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionListID  , gID, "list"  , sMODULE_NAME, Sql.ToInteger(lstList  .SelectedValue), trn);
											if ( lstEdit   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionEditID  , gID, "edit"  , sMODULE_NAME, Sql.ToInteger(lstEdit  .SelectedValue), trn);
											if ( lstDelete != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionDeleteID, gID, "delete", sMODULE_NAME, Sql.ToInteger(lstDelete.SelectedValue), trn);
											if ( lstImport != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionImportID, gID, "import", sMODULE_NAME, Sql.ToInteger(lstImport.SelectedValue), trn);
											if ( lstExport != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionExportID, gID, "export", sMODULE_NAME, Sql.ToInteger(lstExport.SelectedValue), trn);
											// 09/26/2017 Paul.  Add Archive access right. 
											if ( lstArchive != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionArchiveID, gID, "archive", sMODULE_NAME, Sql.ToInteger(lstArchive.SelectedValue), trn);
											//break;
										}
									}
									// 07/16/2023 Paul.  Also duplicte Field Level Security settings. 
									Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
									if ( !Sql.IsEmptyGuid(gDuplicateID) )
										SqlProcs.spACL_FIELDS_Duplicate(gID, gDuplicateID, trn);
									trn.Commit();
									// 03/17/2010 Paul.  We can only reset the current user. 
									SplendidInit.ClearUserACL();
									SplendidInit.LoadUserACL(Security.USER_ID);
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
			SetPageTitle(L10n.Term("ACLRoles.LBL_ROLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *               " + ControlChars.CrLf
							     + "  from vwACL_ROLES_Edit" + ControlChars.CrLf
							     + " where ID = @ID        " + ControlChars.CrLf;
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
											SetPageTitle(L10n.Term("ACLRoles.LBL_ROLE") + " - " + ctlDynamicButtons.Title);
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
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term("ACLRoles.LBL_ROLE") + " - " + ctlDynamicButtons.Title);
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
			m_sMODULE = "ACLRoles";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}

