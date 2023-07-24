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

namespace SplendidCRM.Administration.ProductTemplates
{
	/// <summary>
	///		Summary description for Notes.
	/// </summary>
	public class Notes : SplendidControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected _controls.SearchView     ctlSearchView    ;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected Label           lblError       ;
		protected HtmlInputHidden txtNOTE_ID  ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "Notes.Create":
						Response.Redirect("~/Notes/edit.aspx?PARENT_ID=" + gID.ToString());
						break;
					case "Notes.Edit":
					{
						Guid gNOTE_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/Notes/edit.aspx?ID=" + gNOTE_ID.ToString());
						break;
					}
					case "Notes.Remove":
					{
						Guid gNOTE_ID = Sql.ToGuid(e.CommandArgument);
						//SqlProcs.spPRODUCT_TEMPLATES_NOTES_Delete(gID, gNOTE_ID);
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					// 06/20/2010 Paul.  Add support for SearchView events. Need to rebind inside the clear event. 
					case "Notes.Search":
						ctlSearchView.Visible = !ctlSearchView.Visible;
						break;
					case "Search":
						break;
					case "Clear":
						BindGrid();
						break;
					case "SortGrid":
						break;
					// 06/07/2015 Paul.  Add support for Preview button. 
					case "Preview":
						if ( Page.Master is SplendidMaster )
						{
							CommandEventArgs ePreview = new CommandEventArgs(e.CommandName, new PreviewData(m_sMODULE, Sql.ToGuid(e.CommandArgument)));
							(Page.Master as SplendidMaster).Page_Command(sender, ePreview);
						}
						break;
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
				     + "  from vwPRODUCT_TEMPLATES_NOTES" + ControlChars.CrLf
				     + " where 1 = 1                    " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@PRODUCT_TEMPLATE_ID", gID);
					Sql.AppendParameter(cmd, gID, "PRODUCT_TEMPLATE_ID");
					// 06/20/2010 Paul.  Allow searching of the subpanel. 
					ctlSearchView.SqlSearchClause(cmd);
					// 04/26/2008 Paul.  Move Last Sort to the database.
					cmd.CommandText += grdMain.OrderByClause("DATE_MODIFIED", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwPRODUCT_TEMPLATES_NOTES", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
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
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			if ( SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 )
			{
				Guid gNOTE_ID = Sql.ToGuid(txtNOTE_ID.Value);
				if ( !Sql.IsEmptyGuid(gNOTE_ID) )
				{
					try
					{
						//SqlProcs.spPRODUCT_TEMPLATES_NOTES_Update(gID, gNOTE_ID);
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  If we are not going to redirect,then we must clear the value. 
						txtNOTE_ID.Value = String.Empty;
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
				ctlDynamicButtons.AppendButtons("ProductTemplates.Notes", Guid.Empty, gID);
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
			// 06/20/2010 Paul.  We need to connect the SearchView command handler, otherwise it will throw an exception. 
			ctlSearchView.Command     += new CommandEventHandler(Page_Command);
			m_sMODULE = "Notes";
			// 04/26/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_MODIFIED");
			arrSelectFields.Add("NOTE_ID"      );
			arrSelectFields.Add("ASSIGNED_USER_ID");
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			// 06/07/2015 Paul.  Must include Page_Command in order for Preview to fire. 
			this.AppendGridColumns(grdMain, "ProductTemplates." + m_sMODULE, arrSelectFields, Page_Command);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("ProductTemplates.Notes", Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
