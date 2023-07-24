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
	///		Summary description for CallMarketing.
	/// </summary>
	public class CallMarketing : SubPanelControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID                ;
		protected DataView        vwMain             ;
		protected SplendidGrid    grdMain            ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "CallMarketing.Create":
						Response.Redirect("~/CallMarketing/edit.aspx?CAMPAIGN_ID=" + gID.ToString());
						break;
					case "CallMarketing.Edit":
					{
						Guid gCALL_MARKETING_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/CallMarketing/edit.aspx?ID=" + gCALL_MARKETING_ID.ToString() + "&CAMPAIGN_ID=" + gID.ToString());
						break;
					}
					case "CallMarketing.Remove":
					{
						Guid gCALL_MARKETING_ID = Sql.ToGuid(e.CommandArgument);
						if ( bEditView )
						{
							this.DeleteEditViewRelationship(gCALL_MARKETING_ID);
						}
						else
						{
							SqlProcs.spCALL_MARKETING_Delete(gCALL_MARKETING_ID);
						}
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
				// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
				m_sVIEW_NAME = "vwCAMPAIGNS_CALL_MARKETING";
				sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
				     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
				     + "  from " + m_sVIEW_NAME + ControlChars.CrLf
				     + " where 1 = 1"           + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AppendParameter(cmd, gID, "CAMPAIGN_ID");
					cmd.CommandText += grdMain.OrderByClause("DATE_START", "desc");

					if ( bDebug )
						RegisterClientScriptBlock("vwCAMPAIGNS_CALL_MARKETING", Sql.ClientScriptBlock(cmd));

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
								grdMain.DataBind();
								if ( this.Visible )
								{
									if ( bEditView && !IsPostBack )
									{
										this.CreateEditViewRelationships(dt, "ID");
									}
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

		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			if ( this.Visible )
			{
				UniqueGuidCollection arrDELETED = this.GetDeletedEditViewRelationships();
				foreach ( Guid gDELETE_ID in arrDELETED )
				{
					if ( !Sql.IsEmptyGuid(gDELETE_ID) )
						SqlProcs.spCALL_MARKETING_Delete(gDELETE_ID, trn);
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			try
			{
				string sCAMPAIGN_TYPE = Sql.ToString(Page.Items["CAMPAIGN_TYPE"]);
				this.Visible = (sCAMPAIGN_TYPE == "Telesales");
				BindGrid();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}

			if ( !IsPostBack )
			{
				Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
				ctlDynamicButtons.AppendButtons("Campaigns." + m_sMODULE, gASSIGNED_USER_ID, gID);
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
			m_sMODULE = "CallMarketing";
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("ID"                       );
			arrSelectFields.Add("DATE_ENTERED"             );
			arrSelectFields.Add("CAMPAIGN_ASSIGNED_USER_ID");
			this.AppendGridColumns(grdMain, "Campaigns." + m_sMODULE, arrSelectFields);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Campaigns." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}

