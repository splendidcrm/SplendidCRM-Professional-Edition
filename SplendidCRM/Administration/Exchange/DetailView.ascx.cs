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
using Microsoft.Exchange.WebServices.Data;

namespace SplendidCRM.Administration.Exchange
{
	/// <summary>
	///		Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;

		protected Guid          gUSER_ID       ;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected DataTable     dtUserFolders  ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				string sEXCHANGE_ALIAS     = Sql.ToString(ViewState["EXCHANGE_ALIAS"    ]);
				string sEXCHANGE_EMAIL     = Sql.ToString(ViewState["EXCHANGE_EMAIL"    ]);
				string sEXCHANGE_WATERMARK = Sql.ToString(ViewState["EXCHANGE_WATERMARK"]);
				// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
				string sMAIL_SMTPUSER      = Sql.ToString(ViewState["MAIL_SMTPUSER"     ]);
				string sMAIL_SMTPPASS      = Sql.ToString(ViewState["MAIL_SMTPPASS"     ]);
				// 01/17/2017 Paul.  The gEXCHANGE_ID is to lookup the OAuth credentials. 
				bool   bOFFICE365_OAUTH_ENABLED = Sql.ToBoolean(ViewState["OFFICE365_OAUTH_ENABLED"]);
				bool bSyncAll = (e.CommandName == "Exchange.SyncAll");
				
				// 05/09/2018 Paul.  We need to also check for ClientID for Office 365. 
				string sSERVER_URL          = Sql.ToString (Context.Application["CONFIG.Exchange.ServerURL"   ]);
				string sOAUTH_CLIENT_ID     = Sql.ToString (Context.Application["CONFIG.Exchange.ClientID"    ]);
				string sOAUTH_CLIENT_SECRET = Sql.ToString (Context.Application["CONFIG.Exchange.ClientSecret"]);
				// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
				string sOAuthDirectoryTenatID = Sql.ToString(Application["CONFIG.Exchange.DirectoryTenantID"]);
				if ( !Sql.IsEmptyString(sSERVER_URL) && !Sql.IsEmptyString(sOAUTH_CLIENT_ID) && !Sql.IsEmptyString(sOAUTH_CLIENT_SECRET) )
				{
					Spring.Social.Office365.Office365Sync.UserSync User = new Spring.Social.Office365.Office365Sync.UserSync(this.Context, sEXCHANGE_ALIAS, sEXCHANGE_EMAIL, sMAIL_SMTPUSER, sMAIL_SMTPPASS, gUSER_ID, bSyncAll, bOFFICE365_OAUTH_ENABLED);
					if ( e.CommandName == "Exchange.Sync" || e.CommandName == "Exchange.SyncAll" )
					{
						StringBuilder sbErrors = new StringBuilder();
						if ( bSyncAll )
						{
							ctlDynamicButtons.ErrorText = L10n.Term("Users.LBL_SYNC_BACKGROUND");
							// 04/25/2010 Paul.  A SyncAll operation can take a long time, so create inside a thread. 
							// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
							System.Threading.Thread t = new System.Threading.Thread(User.Start);
							t.Start();
						}
						else
						{
							// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
							Spring.Social.Office365.Office365Sync.Sync(User, sbErrors);
						}
						if ( sbErrors.Length > 0 )
							ctlDynamicButtons.ErrorText = sbErrors.ToString();
					}
					else if ( e.CommandName == "Exchange.SyncFolder" )
					{
						Guid gID = Sql.ToGuid(e.CommandArgument);
						vwMain.RowFilter = "ID = '" + gID.ToString() + "'";
						if ( vwMain.Count > 0 )
						{
							DataRowView rowUserFolders = vwMain[0];
							string sREMOTE_KEY        = Sql.ToString (rowUserFolders["REMOTE_KEY"       ]);
							string sMODULE_NAME       = Sql.ToString (rowUserFolders["MODULE_NAME"      ]);
							Guid   gPARENT_ID         = Sql.ToGuid   (rowUserFolders["PARENT_ID"        ]);
							string sPARENT_NAME       = Sql.ToString (rowUserFolders["PARENT_NAME"      ]);
							bool   bWELL_KNOWN_FOLDER = Sql.ToBoolean(rowUserFolders["WELL_KNOWN_FOLDER"]);
						
							StringBuilder sbErrors = new StringBuilder();
							ExchangeSession Session = ExchangeSecurity.LoadUserACL(Application, gUSER_ID);
							Office365AccessToken token = ActiveDirectory.Office365RefreshAccessToken(Context.Application, sOAuthDirectoryTenatID, sOAUTH_CLIENT_ID, sOAUTH_CLIENT_SECRET, gUSER_ID, false);
							Spring.Social.Office365.Api.IOffice365 service = Spring.Social.Office365.Office365Sync.CreateApi(Context.Application, token.access_token);
						
							if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Contacts" )
							{
								// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
								// 02/12/2021 Paul.  Allow disable contacts. 
								bool bDisableContacts = Sql.ToBoolean(User.Context.Application["CONFIG.Exchange.DisableContacts"]);
								if ( !bDisableContacts )
									Spring.Social.Office365.Office365Sync.SyncContacts    (Context, Session, service, sEXCHANGE_ALIAS, gUSER_ID, false, sbErrors);
							}
							else if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Calendar" )
							{
								// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
								bool bDisableAppointments = Sql.ToBoolean(User.Context.Application["CONFIG.Exchange.DisableAppointments"]);
								if ( !bDisableAppointments )
									Spring.Social.Office365.Office365Sync.SyncAppointments(Context, Session, service, sEXCHANGE_ALIAS, gUSER_ID, false, sbErrors);
							}
							// 03/11/2012 Paul.  Import messages from Sent Items if the TO email exists in the CRM. 
							else if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Sent Items" )
							{
								bool bSentItemsSync = Sql.ToBoolean(Application["CONFIG.Exchange.SentItemsSync"]);
								if ( bSentItemsSync )
								{
									// 03/13/2012 Paul.  Move SyncSentItems into a thread as it can take a long time to work. 
									//ExchangeSync.SyncSentItems(Context, Session, service, con, sEXCHANGE_ALIAS, gUSER_ID, true, DateTime.MinValue, sbErrors);
									ctlDynamicButtons.ErrorText = L10n.Term("Users.LBL_SYNC_BACKGROUND");
									System.Threading.Thread t = new System.Threading.Thread(User.SyncSentItems);
									t.Start();
								}
							}
							// 07/05/2017 Paul.  Import messages from Inbox if the FROM email exists in the CRM. 
							else if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Inbox" )
							{
								bool bSentItemsSync = Sql.ToBoolean(Application["CONFIG.Exchange.InboxSync"]);
								if ( bSentItemsSync )
								{
									ctlDynamicButtons.ErrorText = L10n.Term("Users.LBL_SYNC_BACKGROUND");
									System.Threading.Thread t = new System.Threading.Thread(User.SyncInbox);
									t.Start();
								}
							}
							else
							{
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
								
									bool bInboxRoot     = Sql.ToBoolean(Application["CONFIG.Exchange.InboxRoot"    ]);
									bool bSentItemsRoot = Sql.ToBoolean(Application["CONFIG.Exchange.SentItemsRoot"]);
									Spring.Social.Office365.Api.MailFolder fldExchangeRoot = service.FolderOperations.GetWellKnownFolder("msgfolderroot");
									Spring.Social.Office365.Api.MailFolder fldSplendidRoot = null;
									Spring.Social.Office365.Api.MailFolder fldModuleFolder = null;
									if ( bInboxRoot )
										fldExchangeRoot = service.FolderOperations.GetWellKnownFolder("inbox");
									if ( Sql.IsEmptyString(sMODULE_NAME) )
									{
										// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
										Spring.Social.Office365.Office365Sync.SyncModuleFolders(Context, Session, service, con, fldExchangeRoot, ref fldSplendidRoot, ref fldModuleFolder, String.Empty, Guid.Empty, String.Empty, sEXCHANGE_ALIAS, gUSER_ID, true, sbErrors);
									}
									else
									{
										// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
										Spring.Social.Office365.Office365Sync.SyncModuleFolders(Context, Session, service, con, fldExchangeRoot, ref fldSplendidRoot, ref fldModuleFolder, sMODULE_NAME, gPARENT_ID, sPARENT_NAME, sEXCHANGE_ALIAS, gUSER_ID, true, sbErrors);
										if ( bSentItemsRoot )
										{
											fldExchangeRoot = service.FolderOperations.GetWellKnownFolder("sentitems");
											fldSplendidRoot = null;
											fldModuleFolder = null;
											// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
											Spring.Social.Office365.Office365Sync.SyncModuleFolders(Context, Session, service, con, fldExchangeRoot, ref fldSplendidRoot, ref fldModuleFolder, sMODULE_NAME, gPARENT_ID, sPARENT_NAME, sEXCHANGE_ALIAS, gUSER_ID, true, sbErrors);
										}
									}
								}
							}
							ctlDynamicButtons.ErrorText = sbErrors.ToString();
							// 04/25/2010 Paul.  We need to bind the grid, otherwise the pagination will disappear. 
							vwMain.RowFilter = String.Empty;
							grdMain.DataBind();
						}
					}
				}
				else
				{
					// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
					ExchangeSync.UserSync User = new ExchangeSync.UserSync(this.Context, sEXCHANGE_ALIAS, sEXCHANGE_EMAIL, sMAIL_SMTPUSER, sMAIL_SMTPPASS, gUSER_ID, sEXCHANGE_WATERMARK, bSyncAll, bOFFICE365_OAUTH_ENABLED);
					if ( e.CommandName == "Exchange.Sync" || e.CommandName == "Exchange.SyncAll" )
					{
						StringBuilder sbErrors = new StringBuilder();
						if ( bSyncAll )
						{
							ctlDynamicButtons.ErrorText = L10n.Term("Users.LBL_SYNC_BACKGROUND");
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
							ctlDynamicButtons.ErrorText = sbErrors.ToString();
					}
					else if ( e.CommandName == "Exchange.SyncFolder" )
					{
						Guid gID = Sql.ToGuid(e.CommandArgument);
						vwMain.RowFilter = "ID = '" + gID.ToString() + "'";
						if ( vwMain.Count > 0 )
						{
							DataRowView rowUserFolders = vwMain[0];
							string sREMOTE_KEY        = Sql.ToString (rowUserFolders["REMOTE_KEY"       ]);
							string sMODULE_NAME       = Sql.ToString (rowUserFolders["MODULE_NAME"      ]);
							Guid   gPARENT_ID         = Sql.ToGuid   (rowUserFolders["PARENT_ID"        ]);
							string sPARENT_NAME       = Sql.ToString (rowUserFolders["PARENT_NAME"      ]);
							bool   bWELL_KNOWN_FOLDER = Sql.ToBoolean(rowUserFolders["WELL_KNOWN_FOLDER"]);
						
							StringBuilder sbErrors = new StringBuilder();
							ExchangeSession Session = ExchangeSecurity.LoadUserACL(Application, gUSER_ID);
							// 11/23/2011 Paul.  Add MAIL_SMTPUSER and MAIL_SMTPPASS so that we can avoid impersonation. 
							ExchangeService service = ExchangeUtils.CreateExchangeService(User);
						
							if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Contacts" )
							{
								// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
								// 02/12/2021 Paul.  Allow disable contacts. 
								bool bDisableContacts = Sql.ToBoolean(User.Context.Application["CONFIG.Exchange.DisableContacts"]);
								if ( !bDisableContacts )
									ExchangeSync.SyncContacts    (Context, Session, service, sEXCHANGE_ALIAS, gUSER_ID, false, sbErrors);
							}
							else if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Calendar" )
							{
								// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
								// 02/12/2021 Paul.  Should have been disabled here long ago. 
								bool bDisableAppointments = Sql.ToBoolean(User.Context.Application["CONFIG.Exchange.DisableAppointments"]);
								if ( !bDisableAppointments )
									ExchangeSync.SyncAppointments(Context, Session, service, sEXCHANGE_ALIAS, gUSER_ID, false, sbErrors);
							}
							// 03/11/2012 Paul.  Import messages from Sent Items if the TO email exists in the CRM. 
							else if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Sent Items" )
							{
								bool bSentItemsSync = Sql.ToBoolean(Application["CONFIG.Exchange.SentItemsSync"]);
								if ( bSentItemsSync )
								{
									// 03/13/2012 Paul.  Move SyncSentItems into a thread as it can take a long time to work. 
									//ExchangeSync.SyncSentItems(Context, Session, service, con, sEXCHANGE_ALIAS, gUSER_ID, true, DateTime.MinValue, sbErrors);
									ctlDynamicButtons.ErrorText = L10n.Term("Users.LBL_SYNC_BACKGROUND");
									System.Threading.Thread t = new System.Threading.Thread(User.SyncSentItems);
									t.Start();
								}
							}
							// 07/05/2017 Paul.  Import messages from Inbox if the FROM email exists in the CRM. 
							else if ( bWELL_KNOWN_FOLDER && sMODULE_NAME == "Inbox" )
							{
								bool bSentItemsSync = Sql.ToBoolean(Application["CONFIG.Exchange.InboxSync"]);
								if ( bSentItemsSync )
								{
									ctlDynamicButtons.ErrorText = L10n.Term("Users.LBL_SYNC_BACKGROUND");
									System.Threading.Thread t = new System.Threading.Thread(User.SyncInbox);
									t.Start();
								}
							}
							else
							{
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
								
									bool bInboxRoot     = Sql.ToBoolean(Application["CONFIG.Exchange.InboxRoot"    ]);
									bool bSentItemsRoot = Sql.ToBoolean(Application["CONFIG.Exchange.SentItemsRoot"]);
									WellKnownFolderName fldExchangeRoot = WellKnownFolderName.MsgFolderRoot;
									Folder fldSplendidRoot = null;
									Folder fldModuleFolder = null;
									if ( bInboxRoot )
										fldExchangeRoot = WellKnownFolderName.Inbox;
									if ( Sql.IsEmptyString(sMODULE_NAME) )
									{
										// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
										ExchangeSync.SyncModuleFolders(Context, Session, service, con, fldExchangeRoot, ref fldSplendidRoot, ref fldModuleFolder, String.Empty, Guid.Empty, String.Empty, sEXCHANGE_ALIAS, gUSER_ID, true, sbErrors);
									}
									else
									{
										// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
										ExchangeSync.SyncModuleFolders(Context, Session, service, con, fldExchangeRoot, ref fldSplendidRoot, ref fldModuleFolder, sMODULE_NAME, gPARENT_ID, sPARENT_NAME, sEXCHANGE_ALIAS, gUSER_ID, true, sbErrors);
										if ( bSentItemsRoot )
										{
											fldExchangeRoot = WellKnownFolderName.SentItems;
											fldSplendidRoot = null;
											fldModuleFolder = null;
											// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
											ExchangeSync.SyncModuleFolders(Context, Session, service, con, fldExchangeRoot, ref fldSplendidRoot, ref fldModuleFolder, sMODULE_NAME, gPARENT_ID, sPARENT_NAME, sEXCHANGE_ALIAS, gUSER_ID, true, sbErrors);
										}
									}
								}
							}
							ctlDynamicButtons.ErrorText = sbErrors.ToString();
							// 04/25/2010 Paul.  We need to bind the grid, otherwise the pagination will disappear. 
							vwMain.RowFilter = String.Empty;
							grdMain.DataBind();
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText += ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Exchange.LBL_EXCHANGE_SYNC"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "access") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}
			try
			{
				gUSER_ID = Sql.ToGuid(Request["USER_ID"]);
				// 06/23/2015 Paul.  We always need to get the records, otherwise we cannot Sync one folder at a time.
				//if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gUSER_ID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                                   " + ControlChars.CrLf
							     + "  from vwUSERS_EXCHANGE                    " + ControlChars.CrLf
							     + " where ASSIGNED_USER_ID = @ASSIGNED_USER_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gUSER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											ViewState["EXCHANGE_ALIAS"    ] = Sql.ToString(rdr["EXCHANGE_ALIAS"    ]);
											ViewState["EXCHANGE_EMAIL"    ] = Sql.ToString(rdr["EXCHANGE_EMAIL"    ]);
											ViewState["EXCHANGE_WATERMARK"] = Sql.ToString(rdr["EXCHANGE_WATERMARK"]);
											// 11/23/2011 Paul.  New option not to impersonate Exchange users. 
											ViewState["MAIL_SMTPUSER"     ] = Sql.ToString(rdr["MAIL_SMTPUSER"     ]);
											ViewState["MAIL_SMTPPASS"     ] = Sql.ToString(rdr["MAIL_SMTPPASS"     ]);
											// 01/17/2017 Paul.  The gEXCHANGE_ID is to lookup the OAuth credentials. 
											try
											{
												ViewState["OFFICE365_OAUTH_ENABLED"] = Sql.ToBoolean(rdr["OFFICE365_OAUTH_ENABLED"]);
											}
											catch
											{
											}
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, rdr);
										}
										else
										{
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
						// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
						ctlDynamicButtons.Title = L10n.Term("Exchange.LBL_EXCHANGE_SYNC") + Sql.ToString(ViewState["EXCHANGE_ALIAS"]);
						SetPageTitle(ctlDynamicButtons.Title);
						
						dtUserFolders = SplendidCache.ExchangeFolders(Context, gUSER_ID);
						vwMain = new DataView(dtUserFolders);
						grdMain.DataSource = vwMain;
						if ( !IsPostBack )
						{
							grdMain.DataBind();
						}
					}
					else
					{
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
						ctlDynamicButtons.DisableAll();
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = Utils.ExpandException(ex);
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
			m_sMODULE = "Exchange";
			// 07/24/2010 Paul.  We need an admin flag for the areas that don't have a record in the Modules table. 
			SetAdminMenu(m_sMODULE);
			// this.AppendGridColumns(grdMain, m_sMODULE + ".ListView");
			// 06/23/2015 Paul.  We are always getting the records, so we will always create the buttons in Page_Load. 
			//if ( IsPostBack )
			//{
			//	ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			//}
		}
		#endregion
	}
}
