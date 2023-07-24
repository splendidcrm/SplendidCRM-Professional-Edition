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

namespace SplendidCRM.Administration.Teams
{
	/// <summary>
	///		Summary description for Users.
	/// </summary>
	public class Users : SplendidControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected Label           lblError       ;
		protected HtmlInputHidden txtUSER_ID     ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "Users.Remove":
					{
						Guid gUSER_ID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spTEAM_MEMBERSHIPS_Delete(gID, gUSER_ID);
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					default:
						throw(new Exception("Unknown command: " + e.CommandName));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void BindGrid()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				// 04/26/2008 Paul.  Build the list of fields to use in the select clause.
				sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
				     + "  from vwTEAM_MEMBERSHIPS_List" + ControlChars.CrLf
				     + " where 1 = 1                  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AppendParameter(cmd, gID, "TEAM_ID");
					// 04/26/2008 Paul.  Move Last Sort to the database.
					cmd.CommandText += grdMain.OrderByClause("FULL_NAME", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwTEAM_MEMBERSHIPS_List", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								dt.Columns.Add("MEMBERSHIP");
								string sMember          = L10n.Term(".team_membership_dom.Member");
								string sMemberReportsTo = L10n.Term(".team_membership_dom.Member Reports-to");
								foreach ( DataRow row in dt.Rows )
								{
									if ( Sql.ToBoolean(row["EXPLICIT_ASSIGN"]) )
										row["MEMBERSHIP"] = sMember;
									else
										row["MEMBERSHIP"] = sMemberReportsTo;
								}
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								// 09/05/2005 Paul.  LinkButton controls will not fire an event unless the the grid is bound. 
								// 04/25/2008 Paul.  Enable sorting of sub panel. 
								// 04/26/2008 Paul.  Move Last Sort to the database.
								grdMain.DataBind();
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
			gID = Sql.ToGuid(Request["ID"]);
			// 04/28/2016 Paul.  Make sure the user can view the teams. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess("Teams", "view") >= 0);
			if ( !this.Visible )
				return;
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			if ( SplendidCRM.Security.AdminUserAccess("Teams", "edit") >= 0 )
			{
				if ( !Sql.IsEmptyString(txtUSER_ID.Value) )
				{
					try
					{
						SqlProcs.spTEAM_MEMBERSHIPS_MassUpdate(gID, txtUSER_ID.Value);
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  If we are not going to redirect,then we must clear the value. 
						txtUSER_ID.Value = String.Empty;
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
					}
				}
			}
			BindGrid();

			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				ctlDynamicButtons.AppendButtons("Teams." + m_sMODULE, Guid.Empty, gID);
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
			m_sMODULE = "Users";
			// 04/26/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_ENTERED"   );
			arrSelectFields.Add("USER_ID"        );
			arrSelectFields.Add("USER_NAME"      );
			arrSelectFields.Add("FULL_NAME"      );
			// 05/09/2008 Paul.  MEMBERSHIP is not returned from the database. 
			//arrSelectFields.Add("MEMBERSHIP"     );
			arrSelectFields.Add("EMAIL1"         );
			arrSelectFields.Add("PHONE_WORK"     );
			arrSelectFields.Add("EXPLICIT_ASSIGN");
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Teams." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
