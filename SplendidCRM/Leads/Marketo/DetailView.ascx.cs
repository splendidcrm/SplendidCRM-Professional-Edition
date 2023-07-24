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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Leads.Marketo
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;

		protected Guid        gID              ;
		protected int         nMID             ;
		protected HtmlTable   tblMain          ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = Spring.Social.Marketo.MarketoSync.MarketoEnabled(Application) && (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0) && (SplendidCRM.Security.GetUserAccess("Marketo", "view") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID  = Sql.ToGuid   (Request["ID" ]);
				nMID = Sql.ToInteger(Request["MID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select (select SYNC_REMOTE_KEY from vwLEADS_SYNC where SYNC_SERVICE_NAME = N'Marketo' and SYNC_LOCAL_ID = vwLEADS.ID) as SYNC_REMOTE_KEY" + ControlChars.CrLf
							     + "  from vwLEADS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "view");
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											nMID = Sql.ToInteger(rdr["SYNC_REMOTE_KEY"]);
										}
										else
										{
										}
									}
								}
							}
						}
					}
					if ( nMID > 0 )
					{
						StringBuilder sbErrors = new StringBuilder();
						Spring.Social.Marketo.MarketoSync.RefreshAccessToken(Application, sbErrors);
						if ( sbErrors.Length == 0 )
						{
							Spring.Social.Marketo.Api.IMarketo marketo = Spring.Social.Marketo.MarketoSync.CreateApi(Context.Application);
							string sMarketoFields = Spring.Social.Marketo.MarketoSync.MarketoFields(Application, marketo);
							Spring.Social.Marketo.Api.Lead obj = marketo.LeadOperations.GetById(nMID, sMarketoFields);
							DataRow rdr = Spring.Social.Marketo.Api.Lead.ConvertToRow(obj);
							this.ApplyDetailViewPreLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
							
							this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
							ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, rdr);
							ViewState["MID"] = nMID;
							
							this.ApplyDetailViewPostLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
						}
					}
					else
					{
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
						ctlDynamicButtons.DisableAll();
					}
				}
				else
				{
					// 04/29/2015 Paul.  We need to make sure not to populate the DetailView even if there was an error or if disabled. Otherwise the other subpanels might fail. 
					nMID = Sql.ToInteger(ViewState["MID"]);
					if ( nMID > 0 )
					{
						this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
					}
				}
				this.Visible = (nMID > 0);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Leads";
			this.LayoutDetailView = "DetailView.Marketo";
			if ( IsPostBack )
			{
				// 04/29/2015 Paul.  We need to make sure not to populate the DetailView even if there was an error or if disabled. Otherwise the other subpanels might fail. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			}
		}
		#endregion
	}
}

