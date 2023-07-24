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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;
using System.Globalization;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for ReassignView.
	/// </summary>
	public class ReassignView : SplendidControl
	{
		protected Label        lblError                  ;

		protected Panel        pnlSelectUser             ;
		protected Panel        pnlFilters                ;
		protected Panel        pnlReassignment           ;
		protected Panel        pnlResults                ;
		protected PlaceHolder  plcPreview                ;
		protected PlaceHolder  plcResults                ;

		protected DropDownList lstFROM                   ;
		protected DropDownList lstTO                     ;
		protected DropDownList lstTEAM                   ;
		protected ListBox      lstMODULES                ;
		protected CheckBox     chkREASSIGN_WORKFLOW      ;
		protected TableRow     trTeams                   ;
		protected TableRow     trReassignWorkflows       ;

		protected Panel        pnlAccountFilters         ;
		protected Panel        pnlBugFilters             ;
		protected Panel        pnlCallFilters            ;
		protected Panel        pnlCaseFilters            ;
		protected Panel        pnlOpportunityFilters     ;
		protected Panel        pnlTaskFilters            ;

		protected _controls.SearchView ctlAccountSearch    ;
		protected _controls.SearchView ctlBugSearch        ;
		protected _controls.SearchView ctlCallSearch       ;
		protected _controls.SearchView ctlCaseSearch       ;
		protected _controls.SearchView ctlOpportunitySearch;
		protected _controls.SearchView ctlTaskSearch       ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Submit" )
				{
					int nSelected = 0;
					foreach ( ListItem item in lstMODULES.Items )
					{
						if ( item.Selected )
							nSelected++;
					}
					if ( nSelected == 0 )
						throw(new Exception(L10n.Term("Users.ERR_REASS_SELECT_MODULE")));
					if ( lstFROM.SelectedValue == lstTO.SelectedValue )
						throw(new Exception(L10n.Term("Users.ERR_REASS_DIFF_USERS")));

					if ( Page.IsValid )
					{
						pnlSelectUser  .Visible = false;
						pnlFilters     .Visible = false;
						pnlReassignment.Visible = true ;
						pnlResults     .Visible = false;
						plcPreview.Controls.Clear();
						plcResults.Controls.Clear();
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							foreach ( ListItem item in lstMODULES.Items )
							{
								if ( item.Selected )
								{
									Literal litModule = new Literal();
									litModule.Text = "<h5>" + item.Text + "</h5>";
									plcPreview.Controls.Add(litModule);

									Table     tbl = new Table();
									TableRow  tr  = new TableRow();
									TableCell td  = new TableCell();
									tbl.CssClass = "tabDetailView";
									tbl.BorderWidth = 0;
									tbl.CellSpacing = 0;
									tbl.CellPadding = 0;
									td.CssClass = "tabDetailViewDF";
									plcPreview.Controls.Add(tbl);
									tbl.Rows.Add(tr);
									tr.Cells.Add(td);

									Literal litUpdated = new Literal();
									td.Controls.Add(litUpdated);
									
									int nUpdatedRecords = 0;
									string sTABLE_NAME = Sql.ToString(Application["Modules." + item.Value + ".TableName"]);
									sSQL = "select count(*)        " + ControlChars.CrLf
									     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
									     + " where ASSIGNED_USER_ID = @ASSIGNED_USER_ID" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", Sql.ToGuid(lstFROM.SelectedValue));
										switch ( item.Value )
										{
											case "Accounts"     :
												ctlAccountSearch.SqlSearchClause(cmd);
												break;
											case "Bugs"         :
												ctlBugSearch.SqlSearchClause(cmd);
												break;
											case "Calls"        :
												ctlCallSearch.SqlSearchClause(cmd);
												break;
											case "Cases"        :
												ctlCaseSearch.SqlSearchClause(cmd);
												break;
											case "Opportunities":
												ctlOpportunitySearch.SqlSearchClause(cmd);
												break;
											case "Tasks"        :
												ctlTaskSearch.SqlSearchClause(cmd);
												break;
										}
										nUpdatedRecords = Sql.ToInteger(cmd.ExecuteScalar());
										try
										{
											// 03/04/2009 Paul.  Perform a check to see if the stored procedure exists. 
											SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_MassAssign");
											litUpdated.Text = String.Format(L10n.Term("Users.LBL_REASS_WILL_BE_UPDATED"), nUpdatedRecords.ToString(), item.Text);
										}
										catch ( Exception ex )
										{
											litUpdated.Text = "<font class=\"error\">" + ex.Message + "</font>";
										}
#if DEBUG
										litUpdated.Text += "<pre>" + Sql.ExpandParameters(cmd) + "</pre>";
#endif
									}
								}
							}
						}
					}
				}
				else if ( e.CommandName == "Continue" )
				{
					int nSelected = 0;
					foreach ( ListItem item in lstMODULES.Items )
					{
						if ( item.Selected )
							nSelected++;
					}
					if ( nSelected == 0 )
						throw(new Exception(L10n.Term("Users.ERR_REASS_SELECT_MODULE")));
					if ( lstFROM.SelectedValue == lstTO.SelectedValue )
						throw(new Exception(L10n.Term("Users.ERR_REASS_DIFF_USERS")));

					if ( Page.IsValid )
					{
						pnlSelectUser  .Visible = false;
						pnlFilters     .Visible = false;
						pnlReassignment.Visible = false;
						pnlResults     .Visible = true ;
						plcPreview.Controls.Clear();
						plcResults.Controls.Clear();
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									foreach ( ListItem item in lstMODULES.Items )
									{
										if ( item.Selected )
										{
											Literal litModule = new Literal();
											litModule.Text = "<h5>" + item.Text + "</h5>";
											plcResults.Controls.Add(litModule);

											Table     tbl = new Table();
											TableRow  tr  = new TableRow();
											TableCell td  = new TableCell();
											tbl.CssClass = "tabDetailView";
											tbl.BorderWidth = 0;
											tbl.CellSpacing = 0;
											tbl.CellPadding = 0;
											td.CssClass = "tabDetailViewDF";
											plcResults.Controls.Add(tbl);
											tbl.Rows.Add(tr);
											tr.Cells.Add(td);

											Literal litUpdated = new Literal();
											td.Controls.Add(litUpdated);
											
											int nUpdatedRecords = 0;
											string sTABLE_NAME = Sql.ToString(Application["Modules." + item.Value + ".TableName"]);
											string sSQL;
											sSQL = "select ID              " + ControlChars.CrLf
											     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
											     + " where ASSIGNED_USER_ID = @ASSIGNED_USER_ID" + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												// 01/10/2011 Paul.  Must include the command in the transaction. 
												cmd.Transaction = trn;
												Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", Sql.ToGuid(lstFROM.SelectedValue));
												switch ( item.Value )
												{
													case "Accounts"     :
														ctlAccountSearch.SqlSearchClause(cmd);
														break;
													case "Bugs"         :
														ctlBugSearch.SqlSearchClause(cmd);
														break;
													case "Calls"        :
														ctlCallSearch.SqlSearchClause(cmd);
														break;
													case "Cases"        :
														ctlCaseSearch.SqlSearchClause(cmd);
														break;
													case "Opportunities":
														ctlOpportunitySearch.SqlSearchClause(cmd);
														break;
													case "Tasks"        :
														ctlTaskSearch.SqlSearchClause(cmd);
														break;
												}
												System.Collections.Stack stk = new System.Collections.Stack();
												using ( DbDataAdapter da = dbf.CreateDataAdapter() )
												{
													((IDbDataAdapter)da).SelectCommand = cmd;
													using ( DataTable dt = new DataTable() )
													{
														da.Fill(dt);
														nUpdatedRecords = dt.Rows.Count;
														foreach ( DataRow row in dt.Rows )
														{
															stk.Push(Sql.ToString(row["ID"]));
														}
													}
												}
												IDbCommand cmdMassAssign = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_MassAssign");
												cmdMassAssign.Transaction = trn;
												Sql.SetParameter(cmdMassAssign, "@MODIFIED_USER_ID",  Security.USER_ID);
												Sql.SetParameter(cmdMassAssign, "@ASSIGNED_USER_ID",  Sql.ToGuid(lstTO  .SelectedValue));
												Sql.SetParameter(cmdMassAssign, "@TEAM_ID"         ,  Sql.ToGuid(lstTEAM.SelectedValue));
												while ( stk.Count > 0 )
												{
													string sIDs = Utils.BuildMassIDs(stk);
													Sql.SetParameter(cmdMassAssign, "@ID_LIST", sIDs);
													cmdMassAssign.ExecuteNonQuery();
													//SqlProcs.spACCOUNTS_MassAssign(sIDs, ctlMassUpdate.ASSIGNED_USER_ID, Sql.ToGuid(lstTEAM.SelectedValue), trn);
												}
												litUpdated.Text = String.Format(L10n.Term("Users.LBL_REASS_SUCCESSFUL"), nUpdatedRecords.ToString(), item.Text);
#if DEBUG
												litUpdated.Text += "<pre>" + Sql.ExpandParameters(cmd) + "</pre>";
#endif
											}
										}
									}
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									throw(new Exception(ex.Message, ex.InnerException));
								}
							}
						}
					}
				}
				else if ( e.CommandName == "Go Back" || e.CommandName == "Return" )
				{
					pnlSelectUser  .Visible = true ;
					pnlFilters     .Visible = true ;
					pnlReassignment.Visible = false;
					pnlResults     .Visible = false;
					plcPreview.Controls.Clear();
					plcResults.Controls.Clear();
				}
				else if ( e.CommandName == "Clear" || e.CommandName == "Restart" )
				{
					pnlSelectUser  .Visible = true ;
					pnlFilters     .Visible = true ;
					pnlReassignment.Visible = false;
					pnlResults     .Visible = false;
					plcPreview.Controls.Clear();
					plcResults.Controls.Clear();

					lstFROM.SelectedIndex = 0;
					lstTO  .SelectedIndex = 0;
					if ( Crm.Config.enable_team_management() )
						lstTEAM.SelectedIndex = 0;
					chkREASSIGN_WORKFLOW.Checked = false;
					
					foreach ( ListItem item in lstMODULES.Items )
					{
						item.Selected = false;
					}
					pnlAccountFilters    .Visible = false;
					pnlBugFilters        .Visible = false;
					pnlCallFilters       .Visible = false;
					pnlCaseFilters       .Visible = false;
					pnlOpportunityFilters.Visible = false;
					pnlTaskFilters       .Visible = false;
					ctlAccountSearch    .ClearForm();
					ctlBugSearch        .ClearForm();
					ctlCallSearch       .ClearForm();
					ctlCaseSearch       .ClearForm();
					ctlOpportunitySearch.ClearForm();
					ctlTaskSearch       .ClearForm();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void lstMODULES_Changed(object sender, System.EventArgs e)
		{
			foreach ( ListItem item in lstMODULES.Items )
			{
				switch ( item.Value )
				{
					case "Accounts"     :  pnlAccountFilters    .Visible = item.Selected;  break;
					case "Bugs"         :  pnlBugFilters        .Visible = item.Selected;  break;
					case "Calls"        :  pnlCallFilters       .Visible = item.Selected;  break;
					case "Cases"        :  pnlCaseFilters       .Visible = item.Selected;  break;
					case "Opportunities":  pnlOpportunityFilters.Visible = item.Selected;  break;
					case "Tasks"        :  pnlTaskFilters       .Visible = item.Selected;  break;
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Users.LBL_REASSIGN_TITLE"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				if ( !IsPostBack )
				{
					pnlSelectUser  .Visible = true ;
					pnlFilters     .Visible = true ;
					pnlReassignment.Visible = false;
					pnlResults     .Visible = false;
					trTeams.Visible = Crm.Config.enable_team_management();
					//trReassignWorkflows.Visible = (Sql.ToString(Application["CONFIG.service_level"]) == "Enterprise");

					pnlAccountFilters    .Visible = false;
					pnlBugFilters        .Visible = false;
					pnlCallFilters       .Visible = false;
					pnlCaseFilters       .Visible = false;
					pnlOpportunityFilters.Visible = false;
					pnlTaskFilters       .Visible = false;

					lstFROM.DataSource = SplendidCache.AssignedUser();
					lstFROM.DataBind();
					lstTO  .DataSource = SplendidCache.AssignedUser();
					lstTO  .DataBind();
					lstTEAM.DataSource = SplendidCache.Teams();
					lstTEAM.DataBind();
					lstTEAM.Items.Insert(0, new ListItem(L10n.Term("Users.LBL_REASS_NO_CHANGE"), ""));
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 04/17/2013 Paul.  BusinessRules and CallMarketing do not have normal ASSIGNED_USER_ID fields. 
						// 06/26/2021 Paul.  Exclude BusinessProcesses. 
						sSQL = "select MODULE_NAME      " + ControlChars.CrLf
						     + "  from vwMODULES_AppVars" + ControlChars.CrLf
						     + " where IS_ASSIGNED = 1  " + ControlChars.CrLf
						     + "   and MODULE_NAME not in (N'Activities', N'BusinessRules', N'CallMarketing', N'RulesWizard', N'ReportRules', N'WorkflowAlertShells', 'BusinessProcesses')" + ControlChars.CrLf
						     + " order by MODULE_NAME   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									string sMODULE_NAME  = Sql.ToString (rdr["MODULE_NAME"]);
									string sDISPLAY_NAME = Sql.ToString(L10n.Term(".moduleList.", sMODULE_NAME));
									lstMODULES.Items.Add(new ListItem(sDISPLAY_NAME, sMODULE_NAME));
								}
							}
						}
					}
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
			m_sMODULE = "Users";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
