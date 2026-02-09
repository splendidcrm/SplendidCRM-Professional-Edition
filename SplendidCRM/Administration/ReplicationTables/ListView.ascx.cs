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

namespace SplendidCRM.Administration.ReplicationTables
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected _controls.HeaderButtons ctlModuleHeader;
		protected _controls.SearchView   ctlSearchView  ;
		protected _controls.CheckAll     ctlCheckAll    ;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected MassUpdate    ctlMassUpdate  ;
		protected Panel         pnlMassUpdateSeven;
		// 04/29/2018 Paul.  Disable run if External Archive is enabled. 
		protected bool          bExternalArchiveActive = false;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Clear" )
				{
					// 01/01/2026 Paul.  Need to manually save clear as bShowSearchViews is false. 
					ctlSearchView.SaveDefaultView();
					Server.Transfer("default.aspx");
				}
				else if ( e.CommandName == "Search" )
				{
					grdMain.CurrentPageIndex = 0;
					grdMain.DataBind();
				}
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
					arrSelectFields.AddFields(grdMain.SortColumn);
				}
				else if ( e.CommandName == "SelectAll" )
				{
					// 01/05/2026 Paul.  Bind before and after due to custom paging. 
					grdMain.DataBind();
					ctlCheckAll.SelectAll(vwMain, "TABLE_NAME");
					grdMain.DataBind();
				}
				else if ( e.CommandName == "UpdateStats" )
				{
					DbProviderFactory dbfReplication = DbReplicationFactories.GetFactory();
					if ( !Sql.IsEmptyString(Context.Application["ReplicationConnectionString"]) )
					{
						string[] arrTABLE_NAME = ctlCheckAll.SelectedItemsArray;
						if ( arrTABLE_NAME != null )
						{
							if ( arrTABLE_NAME.Length > 0 )
							{
								for ( int i = 0; i < arrTABLE_NAME.Length && Response.IsClientConnected; i++ )
								{
									string sTABLE_NAME = arrTABLE_NAME[i];
									ReplicationExternalDB.UpdateStats(Context, sTABLE_NAME);
								}
								ctlCheckAll.ClearAll();
								grdMain.DataBind();
							}
						}
					}
					else
					{
						lblError.Text = L10n.Term("ReplicationTables.ERR_NOT_CONFIGURED");
					}
				}
				else if ( e.CommandName == "RunSync" )
				{
					DbProviderFactory dbfReplication = DbReplicationFactories.GetFactory();
					if ( !Sql.IsEmptyString(Context.Application["ReplicationConnectionString"]) )
					{
						string[] arrTABLE_NAME = ctlCheckAll.SelectedItemsArray;
						if ( arrTABLE_NAME != null )
						{
							if ( arrTABLE_NAME.Length > 0 )
							{
								for ( int i = 0; i < arrTABLE_NAME.Length && Response.IsClientConnected; i++ )
								{
									string sTABLE_NAME = arrTABLE_NAME[i];
#if DEBUG
									ReplicationExternalDB.RunReplication(Context, sTABLE_NAME);
#else
									ReplicationBackground background = new ReplicationBackground(Context, sTABLE_NAME);
									System.Threading.Thread t = new System.Threading.Thread(background.Start);
									t.Start();
									lblError.Text = L10n.Term(".LBL_BACKGROUND_OPERATION");
#endif
								}
#if DEBUG
								ctlCheckAll.ClearAll();
								grdMain.DataBind();
#endif
							}
						}
					}
					else
					{
						lblError.Text = L10n.Term("ReplicationTables.ERR_NOT_CONFIGURED");
					}
				}
				else if ( e.CommandName == "ToggleMassUpdate" )
				{
					pnlMassUpdateSeven.Visible = !pnlMassUpdateSeven.Visible;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void grdMain_OnSelectMethod(int nCurrentPageIndex, int nPageSize)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					string sTABLE_NAME = "REPLICATION_TABLES";
					sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
					     + "  from " + Sql.MetadataName(cmd, "vw" + sTABLE_NAME) + ControlChars.CrLf;
					cmd.CommandText = sSQL;
					Security.Filter(cmd, m_sMODULE, "list");
					ctlSearchView.SqlSearchClause(cmd);

					if ( nPageSize > 0 )
					{
						Sql.PageResults(cmd, "vw" + sTABLE_NAME, grdMain.OrderByClause(), nCurrentPageIndex, nPageSize);
					}
					else
					{
						cmd.CommandText += grdMain.OrderByClause();
					}

					if ( bDebug )
						RegisterClientScriptBlock("SQLPaged", Sql.ClientScriptBlock(cmd));

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							vwMain = dt.DefaultView;
							grdMain.DataSource = vwMain;
						}
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				grdMain.AllowCustomPaging = true;
				grdMain.SelectMethod     += new SelectMethodHandler(grdMain_OnSelectMethod);

				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
						string sSQL;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						grdMain.OrderByClause("LOCAL_COUNT", "desc");

						string sTABLE_NAME = "REPLICATION_TABLES";
						sSQL = "select count(*)" + ControlChars.CrLf
						     + "  from " + Sql.MetadataName(cmd, "vw" + sTABLE_NAME) + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Security.Filter(cmd, m_sMODULE, "list");
						ctlSearchView.SqlSearchClause(cmd);

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
						
						grdMain.VirtualItemCount = Sql.ToInteger(cmd.ExecuteScalar());
						
						ctlMassUpdate.Visible = !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
						ctlCheckAll  .Visible = !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
					}
				}
				if ( !IsPostBack )
				{
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
			ctlSearchView  .Command += new CommandEventHandler(Page_Command);
			ctlMassUpdate  .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll    .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "ReplicationTables";
			SetMenu(m_sMODULE);
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("ID"  );
			arrSelectFields.Add("NAME");
			arrSelectFields.Add("TABLE_NAME");
			this.AppendGridColumns(grdMain, m_sMODULE + "." + LayoutListView, arrSelectFields);
			if ( Security.GetUserAccess(m_sMODULE, "delete") < 0 && Security.GetUserAccess(m_sMODULE, "edit") < 0 )
				ctlMassUpdate.Visible = false;
			
			if ( SplendidDynamic.StackedLayout(Page.Theme) )
			{
				ctlModuleHeader.Command += new CommandEventHandler(Page_Command);
				ctlModuleHeader.AppendButtons(m_sMODULE + "." + LayoutListView, Guid.Empty, null);
				grdMain.IsMobile       = this.IsMobile;
				grdMain.MassUpdateView = m_sMODULE + ".MassUpdate";
				grdMain.Command       += new CommandEventHandler(Page_Command);
				if ( !IsPostBack )
					pnlMassUpdateSeven.Visible = false;
			}
		}
#endregion
	}
}
