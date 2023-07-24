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

namespace SplendidCRM.Campaigns
{
	/// <summary>
	///		Summary description for ProspectLists.
	/// </summary>
	public class ProspectLists : SubPanelControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID                ;
		protected DataView        vwMain             ;
		protected SplendidGrid    grdMain            ;
		protected HtmlInputHidden txtPROSPECT_LIST_ID;
		// 02/21/2010 Paul.  Controls to manage inline create. 
		protected Button          btnCreateInline   ;
		protected Panel           pnlNewRecordInline;
		// 02/21/2010 Paul.  Make sure to point to the Contacts.NewRecord. 
		protected SplendidCRM.ProspectLists.NewRecord ctlNewRecord   ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					//case "ProspectLists.Create":
					//	Response.Redirect("~/ProspectLists/edit.aspx?PARENT_ID=" + gID.ToString());
					//	break;
					case "ProspectLists.Edit":
					{
						Guid gPROSPECT_LIST_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/ProspectLists/edit.aspx?ID=" + gPROSPECT_LIST_ID.ToString());
						break;
					}
					case "ProspectLists.Remove":
					{
						Guid gPROSPECT_LIST_ID = Sql.ToGuid(e.CommandArgument);
						if ( bEditView )
						{
							this.DeleteEditViewRelationship(gPROSPECT_LIST_ID);
						}
						else
						{
							SqlProcs.spPROSPECT_LIST_CAMPAIGNS_Delete(gPROSPECT_LIST_ID, gID);
						}
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					// 02/21/2010 Paul.  Handle new events that hide and show the NewRecord panel. 
					//case "NewRecord.Show":
					case "ProspectLists.Create":
						// 02/21/2010 Paul.  We are not going to allow inline create on a mobile device. 
						// 02/22/2010 Paul.  We should have a way to turn-off inline editing as it is a performance issue. 
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/" + m_sMODULE + "/edit.aspx?PARENT_ID=" + gID.ToString());
						else
						{
							pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "inline");
							ctlDynamicButtons.HideAll();
						}
						break;
					case "NewRecord.Cancel":
						pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "none");
						ctlDynamicButtons.ShowAll();
						break;
					case "NewRecord.FullForm":
						Response.Redirect("~/" + m_sMODULE + "/edit.aspx?PARENT_ID=" + gID.ToString());
						break;
					case "NewRecord":
						//BindGrid();
						// 02/21/2010 Paul.  Redirect instead of rebind so that the NewRecord form will get cleared. 
						Response.Redirect(Request.RawUrl);
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
						arrSelectFields.Remove("PROSPECT_LIST_ID"         );
						arrSelectFields.Remove("PROSPECT_LIST_NAME"       );
						arrSelectFields.Remove("CAMPAIGN_ID"              );
						arrSelectFields.Remove("CAMPAIGN_NAME"            );
						arrSelectFields.Remove("CAMPAIGN_ASSIGNED_USER_ID");
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "     , ID                         as PROSPECT_LIST_ID         " + ControlChars.CrLf
						     + "     , NAME                       as PROSPECT_LIST_NAME       " + ControlChars.CrLf
						     + "     , @CAMPAIGN_ID               as CAMPAIGN_ID              " + ControlChars.CrLf
						     + "     , @CAMPAIGN_NAME             as CAMPAIGN_NAME            " + ControlChars.CrLf
						     + "     , @CAMPAIGN_ASSIGNED_USER_ID as CAMPAIGN_ASSIGNED_USER_ID" + ControlChars.CrLf
						     + "  from vwPROSPECT_LISTS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@CAMPAIGN_ID"              , gID);
						Sql.AddParameter(cmd, "@CAMPAIGN_NAME"            , Sql.ToString(Page.Items["NAME"            ]));
						Sql.AddParameter(cmd, "@CAMPAIGN_ASSIGNED_USER_ID", Sql.ToGuid  (Page.Items["ASSIGNED_USER_ID"]));
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, arrUPDATED.ToArray(), "ID");
					}
					else
					{
						// 04/26/2008 Paul.  Build the list of fields to use in the select clause.
						// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
						m_sVIEW_NAME = "vwCAMPAIGNS_PROSPECT_LISTS";
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
						     + "  from " + m_sVIEW_NAME + ControlChars.CrLf
						     + " where 1 = 1"           + ControlChars.CrLf;
						cmd.CommandText = sSQL;
					}
					Sql.AppendParameter(cmd, gID, "CAMPAIGN_ID");
					// 04/26/2008 Paul.  Move Last Sort to the database.
					cmd.CommandText += grdMain.OrderByClause("DATE_ENTERED", "desc");

					if ( bDebug )
						RegisterClientScriptBlock("vwCAMPAIGNS_PROSPECT_LISTS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								this.ApplyGridViewRules("Campaigns." + m_sMODULE, dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								// 09/05/2005 Paul.  LinkButton controls will not fire an event unless the the grid is bound. 
								// 04/25/2008 Paul.  Enable sorting of sub panel. 
								// 04/26/2008 Paul.  Move Last Sort to the database.
								grdMain.DataBind();
								// 01/27/2010 Paul.  In EditView mode, we need a list of existing relationships. 
								if ( bEditView && !IsPostBack )
								{
									this.CreateEditViewRelationships(dt, "PROSPECT_LIST_ID");
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
					SqlProcs.spPROSPECT_LIST_CAMPAIGNS_Delete(gDELETE_ID, gPARENT_ID, trn);
			}

			UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
			foreach ( Guid gUPDATE_ID in arrUPDATED )
			{
				if ( !Sql.IsEmptyGuid(gUPDATE_ID) )
					SqlProcs.spPROSPECT_LIST_CAMPAIGNS_Update(gUPDATE_ID, gPARENT_ID, trn);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			Guid gPROSPECT_LIST_ID = Sql.ToGuid(txtPROSPECT_LIST_ID.Value);
			if ( !Sql.IsEmptyGuid(gPROSPECT_LIST_ID) )
			{
				try
				{
					if ( bEditView )
					{
						this.UpdateEditViewRelationship(gPROSPECT_LIST_ID);
					}
					else
					{
						SqlProcs.spPROSPECT_LIST_CAMPAIGNS_Update(gPROSPECT_LIST_ID, gID);
					}
					// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
					//Response.Redirect("view.aspx?ID=" + gID.ToString());
					// 05/16/2008 Paul.  If we are not going to redirect,then we must clear the value. 
					txtPROSPECT_LIST_ID.Value = String.Empty;
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
				// 04/28/2008 Paul.  Make use of dynamic buttons. 
				Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
				ctlDynamicButtons.AppendButtons("Campaigns." + m_sMODULE, gASSIGNED_USER_ID, gID);
				// 10/21/2010 Paul.  Make sure to initialize the parent. 
				ctlNewRecord.PARENT_ID   = gID;
				ctlNewRecord.PARENT_TYPE = "Campaigns";
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
			ctlNewRecord.Command      += new CommandEventHandler(Page_Command);
			m_sMODULE = "ProspectLists";
			// 04/26/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_ENTERED"             );
			arrSelectFields.Add("PROSPECT_LIST_ID"         );
			arrSelectFields.Add("ASSIGNED_USER_ID"         );
			arrSelectFields.Add("CAMPAIGN_ASSIGNED_USER_ID");
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			this.AppendGridColumns(grdMain, "Campaigns." + m_sMODULE, arrSelectFields);
			// 04/28/2008 Paul.  Make use of dynamic buttons. 
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Campaigns." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}

