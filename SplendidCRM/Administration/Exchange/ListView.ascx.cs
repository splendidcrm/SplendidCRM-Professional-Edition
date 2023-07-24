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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;

namespace SplendidCRM.Administration.Exchange
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected SearchBasic            ctlSearchBasic ;
		protected _controls.CheckAll     ctlCheckAll    ;

		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected MassUpdate    ctlMassUpdate  ;
		// 06/06/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected Panel         pnlMassUpdateSeven;
		protected HiddenField   txtEXCHANGE_ALIAS  ;
		protected HiddenField   txtEXCHANGE_EMAIL  ;
		protected HiddenField   txtASSIGNED_USER_ID;
		protected Button        btnEnableAssigned  ;
		
		protected string        sIMPERSONATED_TYPE ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				if ( e.CommandName == "Search" )
				{
					grdMain.CurrentPageIndex = 0;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
				else if ( e.CommandName == "Clear" )
				{
					ctlSearchBasic.ClearForm();
					Bind(false);
				}
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
				}
				// 11/17/2010 Paul.  Populate the hidden Selected field with all IDs. 
				else if ( e.CommandName == "SelectAll" )
				{
					ctlCheckAll.SelectAll(vwMain, "USER_ID");
					grdMain.DataBind();
				}
				// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
				else if ( e.CommandName == "ToggleMassUpdate" )
				{
					pnlMassUpdateSeven.Visible = !pnlMassUpdateSeven.Visible;
				}
				/*
				else if ( e.CommandName == "MassImport" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						string sIDs = Utils.ValidateIDs(arrID);
						if ( !Sql.IsEmptyString(sIDs) )
						{
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								foreach ( string sID in arrID )
								{
									Guid gUSER_ID = Sql.ToGuid(sID);
									vwMain.RowFilter = "USER_ID = '" + gUSER_ID.ToString() + "'";
									if ( vwMain.Count > 0 )
									{
										DataRowView row = vwMain[0];
										Guid   gASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
										if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
										{
											Guid   gID               = Sql.ToGuid  (row["ID"        ]);
											string sEXCHANGE_NAME    = Sql.ToString(row["NAME"      ]);  // Sql.ToString(row["EXCHANGE_NAME" ]);
											string sEXCHANGE_ALIAS   = Sql.ToString(row["USER_NAME" ]);  // Sql.ToString(row["EXCHANGE_ALIAS"]);
											string sEXCHANGE_EMAIL   = Sql.ToString(row["EMAIL1"    ]);  // Sql.ToString(row["EXCHANGE_EMAIL"]);
											string sEXCHANGE_FIRST   = Sql.ToString(row["FIRST_NAME"]);
											string sEXCHANGE_LAST    = Sql.ToString(row["LAST_NAME" ]);
											string sEXCHANGE_PHONE   = String.Empty;
											string sEXCHANGE_TITLE   = String.Empty;

											// 03/29/2010 Paul.  ValidateImpersonation will throw an exception if it cannot access the Contacts folder. 
											// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
											if ( Sql.ToString(Application["CONFIG.Exchange.ImpersonatedType"]) != "NoImpersonation" )
												ExchangeUtils.ValidateImpersonation(Application, sEXCHANGE_ALIAS, sEXCHANGE_EMAIL);
											//string[] arrEXCHANGE_NAME = sEXCHANGE_NAME.Split(' ');
											//if ( arrEXCHANGE_NAME.Length > 1 )
											//{
											//	sEXCHANGE_FIRST = arrEXCHANGE_NAME[0];
											//	sEXCHANGE_LAST  = arrEXCHANGE_NAME[arrEXCHANGE_NAME.Length - 1];
											//}
											//else
											//{
											//	sEXCHANGE_LAST = arrEXCHANGE_NAME[0];
											//}
											SqlProcs.spEXCHANGE_USERS_InsertUser
												( ref gID
												, sEXCHANGE_ALIAS  
												, sEXCHANGE_EMAIL  
												, sEXCHANGE_FIRST  
												, sEXCHANGE_LAST   
												, sEXCHANGE_TITLE  
												, sEXCHANGE_PHONE  
												, sIMPERSONATED_TYPE
												);
										}
									}
								}
							}
							Bind(false);
						}
					}
				}
				else if ( e.CommandName == "Exchange.EnableAssigned" )
				{
					DataRowView row = vwMain[0];
					Guid gASSIGNED_USER_ID = Sql.ToGuid(txtASSIGNED_USER_ID.Value);
					if ( !Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
					{
						string sEXCHANGE_ALIAS = txtEXCHANGE_ALIAS.Value;
						string sEXCHANGE_EMAIL = txtEXCHANGE_EMAIL.Value;
						txtEXCHANGE_ALIAS  .Value = String.Empty;
						txtEXCHANGE_EMAIL  .Value = String.Empty;
						txtASSIGNED_USER_ID.Value = String.Empty;
						
						// 03/29/2010 Paul.  ValidateImpersonation will throw an exception if it cannot access the Contacts folder. 
						// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
						if ( Sql.ToString(Application["CONFIG.Exchange.ImpersonatedType"]) != "NoImpersonation" )
							ExchangeUtils.ValidateImpersonation(Application, sEXCHANGE_ALIAS, sEXCHANGE_EMAIL);
						Guid gID = Guid.Empty;
						SqlProcs.spEXCHANGE_USERS_Update
							( ref gID
							, sEXCHANGE_ALIAS
							, sEXCHANGE_EMAIL
							, sIMPERSONATED_TYPE
							, gASSIGNED_USER_ID
							);
						Bind(false);
					}
				}
				*/
				else if ( e.CommandName == "Exchange.Enable" )
				{
					Guid gUSER_ID = Sql.ToGuid(e.CommandArgument);
					vwMain.RowFilter = "USER_ID = '" + gUSER_ID.ToString() + "'";
					if ( vwMain.Count > 0 )
					{
						DataRowView row = vwMain[0];
						Guid   gASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
						if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
						{
							string sEXCHANGE_ALIAS = Sql.ToString(row["USER_NAME"]);  // Sql.ToString(row["EXCHANGE_ALIAS"]);
							string sEXCHANGE_EMAIL = Sql.ToString(row["EMAIL1"   ]);  // Sql.ToString(row["EXCHANGE_EMAIL"]);
							
							// 03/29/2010 Paul.  ValidateImpersonation will throw an exception if it cannot access the Contacts folder. 
							// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
							if ( Sql.ToString(Application["CONFIG.Exchange.ImpersonatedType"]) != "NoImpersonation" )
								ExchangeUtils.ValidateImpersonation(Application, sEXCHANGE_ALIAS, sEXCHANGE_EMAIL);
							Guid gID = Guid.Empty;
							SqlProcs.spEXCHANGE_USERS_Update
								( ref gID
								, sEXCHANGE_ALIAS
								, sEXCHANGE_EMAIL
								, sIMPERSONATED_TYPE
								, Guid.Empty
								);
							Bind(false);
						}
					}
				}
				else if ( e.CommandName == "MassEnable" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						if ( arrID.Length > 0 )
						{
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								foreach ( string sID in arrID )
								{
									Guid gUSER_ID = Sql.ToGuid(sID);
									vwMain.RowFilter = "USER_ID = '" + gUSER_ID.ToString() + "'";
									if ( vwMain.Count > 0 )
									{
										DataRowView row = vwMain[0];
										Guid   gASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
										if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
										{
											string sEXCHANGE_ALIAS = Sql.ToString(row["USER_NAME"]);  // Sql.ToString(row["EXCHANGE_ALIAS"]);
											string sEXCHANGE_EMAIL = Sql.ToString(row["EMAIL1"   ]);  // Sql.ToString(row["EXCHANGE_EMAIL"]);
											
											try
											{
												// 03/29/2010 Paul.  ValidateImpersonation will throw an exception if it cannot access the Contacts folder. 
												// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
												if ( Sql.ToString(Application["CONFIG.Exchange.ImpersonatedType"]) != "NoImpersonation" )
													ExchangeUtils.ValidateImpersonation(Application, sEXCHANGE_ALIAS, sEXCHANGE_EMAIL);
												Guid gID = Guid.Empty;
												SqlProcs.spEXCHANGE_USERS_Update
													( ref gID
													, sEXCHANGE_ALIAS
													, sEXCHANGE_EMAIL
													, sIMPERSONATED_TYPE
													, Guid.Empty
													);
											}
											catch
											{
											}
										}
									}
								}
							}
							Bind(false);
						}
					}
				}
				else if ( e.CommandName == "Exchange.Disable" )
				{
					Guid gUSER_ID = Sql.ToGuid(e.CommandArgument);
					vwMain.RowFilter = "USER_ID = '" + gUSER_ID.ToString() + "'";
					if ( vwMain.Count > 0 )
					{
						DataRowView row = vwMain[0];
						Guid gID               = Sql.ToGuid(row["ID"              ]);
						Guid gASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
						if ( !Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
						{
							// 04/26/2010 Paul.  We need to stop any subscriptions when disabled. 
							// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
							ExchangeSync.StopPullSubscription(Context, gASSIGNED_USER_ID);
							ExchangeSync.StopPushSubscription(Context, gASSIGNED_USER_ID);
							SqlProcs.spEXCHANGE_USERS_Delete(gID);
							Bind(false);
						}
					}
				}
				else if ( e.CommandName == "MassDisable" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						if ( arrID.Length > 0 )
						{
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										foreach ( string sID in arrID )
										{
											Guid gUSER_ID = Sql.ToGuid(sID);
											vwMain.RowFilter = "USER_ID = '" + gUSER_ID.ToString() + "'";
											if ( vwMain.Count > 0 )
											{
												DataRowView row = vwMain[0];
												Guid gID               = Sql.ToGuid(row["ID"              ]);
												Guid gASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
												if ( !Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
												{
													// 04/26/2010 Paul.  We need to stop any subscriptions when disabled. 
													// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
													ExchangeSync.StopPullSubscription(Context, gASSIGNED_USER_ID);
													ExchangeSync.StopPushSubscription(Context, gASSIGNED_USER_ID);
													SqlProcs.spEXCHANGE_USERS_Delete(gID, trn);
												}
											}
										}
										trn.Commit();
									}
									catch
									{
										trn.Rollback();
										throw;
									}
								}
							}
							Bind(false);
						}
					}
				}
				else if ( e.CommandName == "Exchange.Sync" || e.CommandName == "Exchange.SyncAll" )
				{
					Guid gUSER_ID = Sql.ToGuid(e.CommandArgument);
					vwMain.RowFilter = "USER_ID = '" + gUSER_ID.ToString() + "'";
					if ( vwMain.Count > 0 )
					{
						DataRowView row = vwMain[0];
						string sEXCHANGE_ALIAS     = Sql.ToString(row["EXCHANGE_ALIAS"    ]);
						string sEXCHANGE_EMAIL     = Sql.ToString(row["EXCHANGE_EMAIL"    ]);
						Guid   gASSIGNED_USER_ID   = Sql.ToGuid  (row["ASSIGNED_USER_ID"  ]);
						string sEXCHANGE_WATERMARK = Sql.ToString(row["EXCHANGE_WATERMARK"]);
						string sMAIL_SMTPUSER      = Sql.ToString(row["MAIL_SMTPUSER"     ]);
						string sMAIL_SMTPPASS      = Sql.ToString(row["MAIL_SMTPPASS"     ]);
						bool   bOFFICE365_OAUTH_ENABLED = false;
						// 01/17/2017 Paul.  The gEXCHANGE_ID is to lookup the OAuth credentials. 
						try
						{
							bOFFICE365_OAUTH_ENABLED = Sql.ToBoolean(row["OFFICE365_OAUTH_ENABLED"]);
						}
						catch
						{
						}
						
						StringBuilder sbErrors = new StringBuilder();
						bool bSyncAll = (e.CommandName == "Exchange.SyncAll");
						// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
						ExchangeSync.UserSync User = new ExchangeSync.UserSync(this.Context, sEXCHANGE_ALIAS, sEXCHANGE_EMAIL, sMAIL_SMTPUSER, sMAIL_SMTPPASS, gASSIGNED_USER_ID, sEXCHANGE_WATERMARK, bSyncAll, bOFFICE365_OAUTH_ENABLED);
						if ( bSyncAll )
						{
							lblError.Text = L10n.Term("Users.LBL_SYNC_BACKGROUND");
							// 04/25/2010 Paul.  A SyncAll operation can take a long time, so create inside a thread. 
							// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
							System.Threading.Thread t = new System.Threading.Thread(User.Start);
							t.Start();
						}
						else
						{
							// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
							ExchangeSync.Sync(User, sbErrors);
						}
						if ( sbErrors.Length > 0 )
							lblError.Text = sbErrors.ToString();
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text += ex.Message;
			}
		}

		private void Bind(bool bIsPostBack)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				// 06/06/2018 Paul.  Allow sorting by status. 
				sSQL = "select vwUSERS_EXCHANGE.*" + ControlChars.CrLf
				     + "     , (case when ASSIGNED_USER_ID is not null then 1 else 0 end) as STATUS" + ControlChars.CrLf
				     + "  from vwUSERS_EXCHANGE" + ControlChars.CrLf
				     + " where 1 = 1           " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					if ( sIMPERSONATED_TYPE == "SmtpAddress" )
						sSQL += "   and EMAIL1 is not null" + ControlChars.CrLf;
					else if ( sIMPERSONATED_TYPE == "PrincipalName" )
						sSQL += "   and USER_NAME is not null" + ControlChars.CrLf;
					// 11/23/2011 Paul.  New option not to impersonate Exchange users. 
					else if ( sIMPERSONATED_TYPE == "NoImpersonation" )
					{
						// 06/06/2018 Paul.  Office365 uses EMAIL1, not MAIL_SMTPUSER. 
						if ( (!Sql.IsEmptyString(Application["CONFIG.Exchange.ClientID"]) && !Sql.IsEmptyString(Application["CONFIG.Exchange.ClientSecret"])) )
							sSQL += "   and EMAIL1 is not null" + ControlChars.CrLf;
						else
							sSQL += "   and MAIL_SMTPUSER is not null" + ControlChars.CrLf;
					}
					cmd.CommandText = sSQL;
					ctlSearchBasic.SqlSearchClause(cmd);

					if ( bDebug )
						RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							vwMain = dt.DefaultView;
							grdMain.DataSource = vwMain ;
							if ( !bIsPostBack )
							{
								// 12/14/2007 Paul.  Only set the default sort if it is not already set.  It may have been set by SearchView. 
								if ( String.IsNullOrEmpty(grdMain.SortColumn) )
								{
									grdMain.SortColumn = "USER_NAME";
									grdMain.SortOrder  = "asc" ;
								}
								grdMain.ApplySort();
								grdMain.DataBind();
							}
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
			{
				Parent.DataBind();
				return;
			}
			try
			{
				bool bIsPostBack = IsPostBack;
				sIMPERSONATED_TYPE  = Sql.ToString (Application["CONFIG.Exchange.ImpersonatedType"]);
				// 06/06/2018 Paul.  We need to show list when Office365 is enabled. 
				if ( !Sql.IsEmptyString(Application["CONFIG.Exchange.UserName"]) || (!Sql.IsEmptyString(Application["CONFIG.Exchange.ClientID"]) && !Sql.IsEmptyString(Application["CONFIG.Exchange.ClientSecret"])) )
				{
					// 04/28/2010 Paul.  WebDAV is not enabled by default in Exchange 2010. 
					// There does not seem to be a reliable way to get the list of users using EWS, 
					// so we need to change the way that we manage users. 
					/*
					dtExchange = Cache.Get("Exchange.Users") as DataTable;
					if ( dtExchange == null )
					{
						StringBuilder sbErrors = new StringBuilder();
						dtExchange = ExchangeUtils.ExchangeUsers(Application, sbErrors);
						
						dtExchange.Columns.Add("ASSIGNED_USER_ID"  , typeof(System.Guid  ));
						dtExchange.Columns.Add("USER_NAME"         , typeof(System.String));
						dtExchange.Columns.Add("NAME"              , typeof(System.String));
						dtExchange.Columns.Add("EMAIL1"            , typeof(System.String));
						dtExchange.Columns.Add("EXCHANGE_WATERMARK", typeof(System.String));
						dtExchange.Columns.Add("CRM_CANDIDATE"     , typeof(System.Boolean));
						
						if ( !IsPostBack )
							Cache.Insert("Exchange.Users", dtExchange, null, DateTime.Now.AddMinutes(15),System.Web.Caching.Cache.NoSlidingExpiration);
						
						if ( sbErrors.Length > 0 )
							lblError.Text = sbErrors.ToString();
						// 03/28/2010 Paul.  If we reload the Exchange table, then we also need to reload the Sync information. 
						bIsPostBack = false;
					}
					*/
					Bind(bIsPostBack);
				}
				if ( !IsPostBack )
				{
					ctlCheckAll.Visible = ctlMassUpdate.Visible;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = Utils.ExpandException(ex);
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
			ctlSearchBasic.Command += new CommandEventHandler(Page_Command);
			ctlMassUpdate .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll   .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Exchange";
			// 07/24/2010 Paul.  We need an admin flag for the areas that don't have a record in the Modules table. 
			SetAdminMenu(m_sMODULE);
			// this.AppendGridColumns(grdMain, m_sMODULE + "." + LayoutListView);
			
			// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
			if ( SplendidDynamic.StackedLayout(Page.Theme) )
			{
				// 06/05/2015 Paul.  Move MassUpdate buttons to the SplendidGrid. 
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
