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

namespace SplendidCRM.Contracts
{
	/// <summary>
	///		Summary description for Documents.
	/// </summary>
	public class Documents : SubPanelControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		// 10/05/2017 Paul.  We need to build a list of the fields used by the search clause. 
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected Label           lblError       ;
		protected HtmlInputHidden txtDOCUMENT_ID ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "Documents.Create":
						Response.Redirect("~/Documents/edit.aspx?PARENT_ID=" + gID.ToString());
						break;
					case "Documents.Edit":
					{
						Guid gDOCUMENT_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/Documents/view.aspx?ID=" + gDOCUMENT_ID.ToString());
						break;
					}
					case "Documents.Remove":
					{
						Guid gDOCUMENT_ID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spCONTRACTS_DOCUMENTS_Delete(gID, gDOCUMENT_ID);
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					case "Documents.GetLatest":
					{
						Guid gDOCUMENT_ID = Sql.ToGuid(e.CommandArgument);
						if ( bEditView )
						{
							this.DeleteEditViewRelationship(gDOCUMENT_ID);
						}
						else
						{
							SqlProcs.spCONTRACTS_DOCUMENTS_GetLatest(gID, gDOCUMENT_ID);
						}
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					// 10/05/2017 Paul.  Add support for Preview button. 
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
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
					if ( bEditView && IsPostBack && arrUPDATED.Count > 0 )
					{
						// 10/05/2017 Paul.  We need to build a list of the fields used by the search clause. 
						arrSelectFields.Remove("DOCUMENT_ID"                  );
						arrSelectFields.Remove("SELECTED_REVISION"            );
						arrSelectFields.Remove("SELECTED_DOCUMENT_REVISION_ID");
						arrSelectFields.Remove("CONTRACT_ID"                  );
						arrSelectFields.Remove("CONTRACT_NAME"                );
						arrSelectFields.Remove("CONTRACT_ASSIGNED_USER_ID"    );
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "     , ID                         as DOCUMENT_ID                  " + ControlChars.CrLf
						     + "     , null                       as SELECTED_REVISION            " + ControlChars.CrLf
						     + "     , null                       as SELECTED_DOCUMENT_REVISION_ID" + ControlChars.CrLf
						     + "     , @CONTRACT_ID               as CONTRACT_ID                  " + ControlChars.CrLf
						     + "     , @CONTRACT_NAME             as CONTRACT_NAME                " + ControlChars.CrLf
						     + "     , @CONTRACT_ASSIGNED_USER_ID as CONTRACT_ASSIGNED_USER_ID    " + ControlChars.CrLf
						     + "  from vwDOCUMENTS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@CONTRACT_ID"              , gID);
						Sql.AddParameter(cmd, "@CONTRACT_NAME"            , Sql.ToString(Page.Items["NAME"            ]));
						Sql.AddParameter(cmd, "@CONTRACT_ASSIGNED_USER_ID", Sql.ToGuid  (Page.Items["ASSIGNED_USER_ID"]));
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, arrUPDATED.ToArray(), "ID");
					}
					else
					{
						// 10/05/2017 Paul.  Add Archive relationship view. 
						// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
						     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						// 10/06/2017 Paul.  Make sure to filter relationship data based on team access rights. 
						// 10/08/2017 Paul.  Add Archive access right. 
						Security.Filter(cmd, m_sMODULE, (ArchiveViewEnabled() ? "archive" : "list"));
						Sql.AppendParameter(cmd, gID, "CONTRACT_ID");
					}
					// 10/05/2017 Paul.  Move Last Sort to the database.
					cmd.CommandText += grdMain.OrderByClause("DOCUMENT_NAME", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwCONTRACTS_DOCUMENTS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								// 10/05/2017 Paul.  Add Archive relationship view. 
								this.ApplyGridViewRules("Contracts." + this.LayoutListView, dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								// 09/05/2005 Paul. LinkButton controls will not fire an event unless the the grid is bound. 
								grdMain.DataBind();
								// 01/27/2010 Paul.  In EditView mode, we need a list of existing relationships. 
								if ( bEditView && !IsPostBack )
								{
									this.CreateEditViewRelationships(dt, "DOCUMENT_ID");
								}
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

		// 01/27/2010 Paul.  This method is only calld when in EditMode. 
		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			UniqueGuidCollection arrDELETED = this.GetDeletedEditViewRelationships();
			foreach ( Guid gDELETE_ID in arrDELETED )
			{
				if ( !Sql.IsEmptyGuid(gDELETE_ID) )
					SqlProcs.spCONTRACTS_DOCUMENTS_Delete(gPARENT_ID, gDELETE_ID, trn);
			}

			UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
			foreach ( Guid gUPDATE_ID in arrUPDATED )
			{
				if ( !Sql.IsEmptyGuid(gUPDATE_ID) )
					SqlProcs.spCONTRACTS_DOCUMENTS_Update(gPARENT_ID, gUPDATE_ID, trn);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			Guid gDOCUMENT_ID = Sql.ToGuid(txtDOCUMENT_ID.Value);
			if ( !Sql.IsEmptyGuid(gDOCUMENT_ID) )
			{
				try
				{
					if ( bEditView )
					{
						this.UpdateEditViewRelationship(gDOCUMENT_ID);
					}
					else
					{
						SqlProcs.spCONTRACTS_DOCUMENTS_Update(gID, gDOCUMENT_ID);
					}
					// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
					//Response.Redirect("view.aspx?ID=" + gID.ToString());
					// 05/16/2008 Paul.  If we are not going to redirect,then we must clear the value. 
					txtDOCUMENT_ID.Value = String.Empty;
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			try
			{
				BindGrid();
				// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
				if ( !IsPostBack )
				{
					Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
					// 10/05/2017 Paul.  Add Archive relationship view. 
					ctlDynamicButtons.AppendButtons("Contracts." + this.LayoutListView, gASSIGNED_USER_ID, gID);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}

			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
			m_sMODULE = "Documents";
			// 10/05/2017 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DOCUMENT_ID"              );
			arrSelectFields.Add("DOCUMENT_NAME"            );
			arrSelectFields.Add("SELECTED_REVISION"        );
			arrSelectFields.Add("REVISION"                 );
			arrSelectFields.Add("ASSIGNED_USER_ID"         );
			arrSelectFields.Add("CONTRACT_ASSIGNED_USER_ID");
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			// 10/05/2017 Paul.  Add Archive relationship view. 
			m_sVIEW_NAME = "vwCONTRACTS_DOCUMENTS";
			if ( ArchiveViewExists() )
				m_sVIEW_NAME = m_sVIEW_NAME + "_ARCHIVE";
			this.LayoutListView = m_sMODULE + (ArchiveView() ? ".ArchiveView" : String.Empty);
			this.AppendGridColumns(grdMain, "Contracts." + this.LayoutListView, arrSelectFields, Page_Command);
			// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Contracts." + this.LayoutListView, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
