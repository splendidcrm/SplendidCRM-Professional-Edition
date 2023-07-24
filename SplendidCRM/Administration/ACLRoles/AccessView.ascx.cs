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

namespace SplendidCRM.Administration.ACLRoles
{
	/// <summary>
	///		Summary description for AccessView.
	/// </summary>
	public class AccessView : SplendidControl
	{
		protected Guid          gID            ;
		protected DataView      vwMain         ;
		// 10/07/2014 Paul.  Need to use a separate view for the admin table, otherwise results from the main get lost. 
		protected DataView      vwMainAdmin    ;
		protected ACLGrid       grdACL         ;
		protected ACLGrid       grdACL_Admin   ;
		protected Label         lblError       ;
		protected Guid          gUSER_ID       ;
		protected Panel         pnlAdmin       ;

		public bool EnableACLEditing
		{
			get
			{
				return grdACL.EnableACLEditing;
			}
			set
			{
				grdACL.EnableACLEditing = value;
				grdACL_Admin.EnableACLEditing = value;
			}
		}

		public Guid USER_ID
		{
			get { return gUSER_ID; }
			set { gUSER_ID = value; }
		}

		// 04/25/2006 Paul.  FindControl needs to be executed on the DataGridItem.  I'm not sure why.
		public DropDownList FindACLControl(string sMODULE_NAME, string sACCESS_TYPE)
		{
			return grdACL.FindACLControl(sMODULE_NAME, sACCESS_TYPE);
		}

		// 03/09/2010 Paul.  Admin roles are managed separately. 
		public DropDownList FindACLControl_Admin(string sMODULE_NAME, string sACCESS_TYPE)
		{
			return grdACL_Admin.FindACLControl(sMODULE_NAME, sACCESS_TYPE);
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		public void BindGrid()
		{
			// 12/07/2006 Paul.  We need to be able to force the grid to be rebound when its data has changed. 
			Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
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
					     + "  from vwACL_ACCESS_ByUser  " + ControlChars.CrLf
					     + " where USER_ID = @USER_ID   " + ControlChars.CrLf
					     + " order by MODULE_NAME       " + ControlChars.CrLf;
				}
				else if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
				{
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
					     + "  from vwACL_ACCESS_ByRole  " + ControlChars.CrLf
					     + " where ROLE_ID = @ROLE_ID   " + ControlChars.CrLf
					     + " order by MODULE_NAME       " + ControlChars.CrLf;
				}
				else
				{
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
				}
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					if ( !Sql.IsEmptyGuid(gUSER_ID) )
					{
						Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
						gID = Guid.Empty;
					}
					else if ( !Sql.IsEmptyGuid(gDuplicateID) )
					{
						Sql.AddParameter(cmd, "@ROLE_ID", gDuplicateID);
						gID = Guid.Empty;
					}
					else if ( !Sql.IsEmptyGuid(gID) )
					{
						Sql.AddParameter(cmd, "@ROLE_ID", gID);
					}

					if ( bDebug )
						RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							vwMain = dt.DefaultView;
							grdACL.DataSource = vwMain ;
							// 04/26/2006 Paul.  Normally, we would only bind if not a postback, 
							// but the ACL grid knows how to handle the postback state, so we must always bind. 
							vwMain.RowFilter = "IS_ADMIN = 0 and MODULE_NAME <> 'Teams'";
							grdACL.DataBind();
							// 03/09/2010 Paul.  Admin roles are managed separately. 
							bool bAllowAdminRoles = Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]);
							if ( bAllowAdminRoles )
							{
								// 10/07/2014 Paul.  Need to use a separate view for the admin table, otherwise results from the main get lost. 
								vwMainAdmin = new DataView(dt);
								vwMainAdmin.RowFilter = "IS_ADMIN = 1 or MODULE_NAME = 'Teams'";
								grdACL_Admin.DataSource = vwMainAdmin ;
								grdACL_Admin.DataBind();
							}
						}
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					pnlAdmin.Visible = Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]);
					
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gUSER_ID) )
					{
						if ( pnlAdmin.Visible )
						{
							pnlAdmin.Visible = Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]) && (SplendidCRM.Security.AdminUserAccess("Users"   , grdACL_Admin.EnableACLEditing ? "edit" : "view") >= 0);
							// 03/17/2010 Paul.  Only display admin roles if the user is an Admin Delegate. 
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL;
								sSQL = "select IS_ADMIN_DELEGATE" + ControlChars.CrLf
								     + "  from vwUSERS          " + ControlChars.CrLf
								     + " where ID = @USER_ID    " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
									bool bIS_ADMIN_DELEGATE = Sql.ToBoolean(cmd.ExecuteScalar());
									pnlAdmin.Visible = bIS_ADMIN_DELEGATE;
								}
							}
						}
					}
					else if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						pnlAdmin.Visible = Sql.ToBoolean(Application["CONFIG.allow_admin_roles"]) && (SplendidCRM.Security.AdminUserAccess("ACLRoles", grdACL_Admin.EnableACLEditing ? "edit" : "view") >= 0);
					}
				}
				BindGrid();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
			// 05/03/2006 Paul.  Remove the page data binding as it clears the binding on UserRolesView.ascx. 
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
		}
		#endregion
	}
}

