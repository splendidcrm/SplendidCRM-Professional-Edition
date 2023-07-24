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
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.Exchange
{
	/// <summary>
	///		Summary description for ExchangeLogin.
	/// </summary>
	public class ExchangeLogin : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected TextBox      SERVER_URL            ;
		protected TextBox      USER_NAME             ;
		protected TextBox      PASSWORD              ;
		protected CheckBox     IGNORE_CERTIFICATE    ;
		// 12/13/2017 Paul.  Allow version to be changed. 
		protected DropDownList EXCHANGE_VERSION      ;
		protected DropDownList IMPERSONATED_TYPE     ;
		protected CheckBox     INBOX_ROOT            ;
		protected CheckBox     SENT_ITEMS_ROOT       ;
		// 03/11/2012 Paul.  Import messages from Sent Items if the TO email exists in the CRM. 
		protected CheckBox     SENT_ITEMS_SYNC       ;
		// 07/05/2017 Paul.  Import messages from Inbox if the FROM email exists in the CRM. 
		protected CheckBox     INBOX_SYNC            ;
		protected CheckBox     PUSH_NOTIFICATIONS    ;
		protected TextBox      PUSH_FREQUENCY        ;
		protected TextBox      PUSH_NOTIFICATION_URL ;
		//protected RequiredFieldValidator reqSERVER_URL;
		protected RequiredFieldValidator reqUSER_NAME ;
		protected RequiredFieldValidator reqPASSWORD  ;
		// 01/17/2017 Paul.  Add support for OAuth. 
		protected TableRow     trAUTHENTICATION_METHOD_OAUTH   ;
		protected TableRow     trAUTHENTICATION_METHOD_USERNAME;
		protected RadioButton  AUTHENTICATION_METHOD_OAUTH     ;
		protected RadioButton  AUTHENTICATION_METHOD_USERNAME  ;
		protected TextBox      OAUTH_CLIENT_ID                 ;
		protected TextBox      OAUTH_CLIENT_SECRET             ;
		// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
		protected TextBox      OAUTH_DIRECTORY_TENANT_ID       ;
		protected Label        LBL_OAUTH_DIRECTORY_TENANT_ID   ;
		protected RequiredFieldValidator reqOAUTH_CLIENT_ID    ;
		protected RequiredFieldValidator reqOAUTH_CLIENT_SECRET;
		protected HiddenField  OAUTH_CODE                      ;
		protected Button       btnOffice365Authorized          ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" )
			{
				try
				{
					SERVER_URL.Text = SERVER_URL.Text.Trim();
					USER_NAME .Text = USER_NAME .Text.Trim();
					PASSWORD  .Text = PASSWORD  .Text.Trim();
					// 01/17/2017 Paul.  Add support for OAuth. 
					OAUTH_CLIENT_ID    .Text = OAUTH_CLIENT_ID    .Text.Trim();
					OAUTH_CLIENT_SECRET.Text = OAUTH_CLIENT_SECRET.Text.Trim();
					OAUTH_DIRECTORY_TENANT_ID.Text = OAUTH_DIRECTORY_TENANT_ID.Text.Trim();
					if ( SERVER_URL.Text.EndsWith("/") )
						SERVER_URL.Text = SERVER_URL.Text.Substring(0, SERVER_URL.Text.Length-1);
					// 08/30/2013 Paul.  Office365 requires that we use auto-discover to get the server URL. 
					if ( !SERVER_URL.Text.ToLower().StartsWith("autodiscover") && !SERVER_URL.Text.ToLower().EndsWith("/exchange.asmx") )
					{
						if ( !SERVER_URL.Text.ToUpper().EndsWith("/EWS") )
						{
							SERVER_URL.Text += "/EWS";
						}
						SERVER_URL.Text += "/Exchange.asmx";
					}
					if ( PUSH_NOTIFICATIONS.Checked )
					{
						if ( PUSH_NOTIFICATION_URL.Text.EndsWith("/") )
							PUSH_NOTIFICATION_URL.Text = PUSH_NOTIFICATION_URL.Text.Substring(0, PUSH_NOTIFICATION_URL.Text.Length - 1);
						if ( !PUSH_NOTIFICATION_URL.Text.ToLower().EndsWith("/exchangeservice2007.asmx") )
							PUSH_NOTIFICATION_URL.Text += "/ExchangeService2007.asmx";
					}
					
					string sPASSWORD = PASSWORD.Text;
					// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
					if ( sPASSWORD == Sql.sEMPTY_PASSWORD )
					{
						sPASSWORD = Sql.ToString (Application["CONFIG.Exchange.Password"]);
					}
					else
					{
						// 09/13/2013 Paul.  Make sure that the encryption keys have been created. 
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_KEY) )
						{
							gINBOUND_EMAIL_KEY = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("mail", "InboundEmailKey", gINBOUND_EMAIL_KEY.ToString());
							Application["CONFIG.InboundEmailKey"] = gINBOUND_EMAIL_KEY;
						}
						Guid gINBOUND_EMAIL_IV = Sql.ToGuid(Application["CONFIG.InboundEmailIV"]);
						if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_IV) )
						{
							gINBOUND_EMAIL_IV = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("mail", "InboundEmailIV", gINBOUND_EMAIL_IV.ToString());
							Application["CONFIG.InboundEmailIV"] = gINBOUND_EMAIL_IV;
						}
						string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sPASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sPASSWORD )
							throw(new Exception("Decryption failed"));
						sPASSWORD = sENCRYPTED_EMAIL_PASSWORD;
					}
					// 05/23/2012 Paul.  Don't validate the Server URL because it prevents us from disabling Exchange Sync. 
					//reqSERVER_URL.Enabled = true;
					//reqSERVER_URL.Validate();
					if ( AUTHENTICATION_METHOD_OAUTH.Checked )
					{
						// 02/10/2017 Paul.  https://outlook.office.com/EWS/Exchange.asmx does not work. 
						if ( !SERVER_URL.Text.StartsWith("https://outlook.office365.com") && !SERVER_URL.Text.StartsWith("https://outlook.office.com") )
							throw(new Exception("OAuth is only supported with Office 365."));
						reqOAUTH_CLIENT_ID    .Enabled = true ;
						reqOAUTH_CLIENT_SECRET.Enabled = true ;
						reqUSER_NAME          .Enabled = false;
						reqPASSWORD           .Enabled = false;
						reqOAUTH_CLIENT_ID    .Validate();
						reqOAUTH_CLIENT_SECRET.Validate();
						USER_NAME.Text = String.Empty;
						PASSWORD .Text = String.Empty;
						sPASSWORD      = String.Empty;
					}
					else
					{
						reqOAUTH_CLIENT_ID    .Enabled = false;
						reqOAUTH_CLIENT_SECRET.Enabled = false;
						reqUSER_NAME          .Enabled = true ;
						reqPASSWORD           .Enabled = true ;
						reqUSER_NAME .Validate();
						reqPASSWORD  .Validate();
						OAUTH_CLIENT_ID    .Text = String.Empty;
						OAUTH_CLIENT_SECRET.Text = String.Empty;
						OAUTH_DIRECTORY_TENANT_ID.Text = String.Empty;
					}
					if ( Page.IsValid )
					{
						// 12/13/2017 Paul.  Allow version to be changed. 
						// 02/05/2023 Paul.  Version is now optional. 
						string sEXCHANGE_VERSION = "Exchange2013_SP1";
						if ( EXCHANGE_VERSION != null )
						{
							sEXCHANGE_VERSION = EXCHANGE_VERSION.SelectedValue;
						}
						if ( SERVER_URL.Text.ToLower().Contains("outlook.office365.com") )
							sEXCHANGE_VERSION = "Exchange2013_SP1";

						if ( e.CommandName == "Test" )
						{
							if ( AUTHENTICATION_METHOD_OAUTH.Checked )
							{
								StringBuilder sbErrors = new StringBuilder();
								// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
								SplendidCRM.ActiveDirectory.Office365TestAccessToken(Application, OAUTH_DIRECTORY_TENANT_ID.Text, OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text, ExchangeUtils.EXCHANGE_ID, sbErrors);
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							}
							else
							{
								StringBuilder sbErrors = new StringBuilder();
								ExchangeUtils.ValidateExchange(Application, SERVER_URL.Text, USER_NAME.Text, sPASSWORD, IGNORE_CERTIFICATE.Checked, IMPERSONATED_TYPE.SelectedValue, sEXCHANGE_VERSION, sbErrors);
								if ( sbErrors.Length > 0 )
									ctlDynamicButtons.ErrorText = sbErrors.ToString();
							}
						}
						else if ( e.CommandName == "Save" )
						{
							// 04/23/2010 Paul.  We need to stop any active subscriptions before we change the values. 
							// 07/18/2010 Paul.  Move Exchange Sync functions to a separate class. 
							ExchangeSync.StopSubscriptions(Context);
							Application["CONFIG.Exchange.ServerURL"          ] = SERVER_URL.Text;
							Application["CONFIG.Exchange.UserName"           ] = USER_NAME .Text;
							Application["CONFIG.Exchange.Password"           ] = sPASSWORD      ;
							Application["CONFIG.Exchange.IgnoreCertificate"  ] = IGNORE_CERTIFICATE.Checked;
							Application["CONFIG.Exchange.Version"            ] = sEXCHANGE_VERSION;
							Application["CONFIG.Exchange.ImpersonatedType"   ] = IMPERSONATED_TYPE.SelectedValue;
							Application["CONFIG.Exchange.InboxRoot"          ] = INBOX_ROOT        .Checked;
							Application["CONFIG.Exchange.SentItemsRoot"      ] = SENT_ITEMS_ROOT   .Checked;
							// 03/11/2012 Paul.  Import messages from Sent Items if the TO email exists in the CRM. 
							Application["CONFIG.Exchange.SentItemsSync"      ] = SENT_ITEMS_SYNC   .Checked;
							// 07/05/2017 Paul.  Import messages from Inbox if the FROM email exists in the CRM. 
							Application["CONFIG.Exchange.InboxSync"          ] = INBOX_SYNC        .Checked;
							Application["CONFIG.Exchange.PushNotifications"  ] = PUSH_NOTIFICATIONS.Checked;
							Application["CONFIG.Exchange.PushFrequency"      ] = PUSH_FREQUENCY.Text;
							Application["CONFIG.Exchange.PushNotificationURL"] = PUSH_NOTIFICATION_URL.Text;
							// 01/17/2017 Paul.  Add support for OAuth. 
							Application["CONFIG.Exchange.ClientID"           ] = OAUTH_CLIENT_ID    .Text;
							Application["CONFIG.Exchange.ClientSecret"       ] = OAUTH_CLIENT_SECRET.Text;
							// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
							Application["CONFIG.Exchange.DirectoryTenantID"  ] = OAUTH_DIRECTORY_TENANT_ID.Text;
							
							SqlProcs.spCONFIG_Update("mail", "Exchange.ServerURL"          , Sql.ToString(Application["CONFIG.Exchange.ServerURL"          ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.UserName"           , Sql.ToString(Application["CONFIG.Exchange.UserName"           ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.Password"           , Sql.ToString(Application["CONFIG.Exchange.Password"           ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.IgnoreCertificate"  , Sql.ToString(Application["CONFIG.Exchange.IgnoreCertificate"  ]));
							// 12/13/2017 Paul.  Allow version to be changed. 
							SqlProcs.spCONFIG_Update("mail", "Exchange.Version"            , Sql.ToString(Application["CONFIG.Exchange.Version"            ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.ImpersonatedType"   , Sql.ToString(Application["CONFIG.Exchange.ImpersonatedType"   ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.InboxRoot"          , Sql.ToString(Application["CONFIG.Exchange.InboxRoot"          ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.SentItemsRoot"      , Sql.ToString(Application["CONFIG.Exchange.SentItemsRoot"      ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.SentItemsSync"      , Sql.ToString(Application["CONFIG.Exchange.SentItemsSync"      ]));
							// 07/05/2017 Paul.  Import messages from Inbox if the FROM email exists in the CRM. 
							SqlProcs.spCONFIG_Update("mail", "Exchange.InboxSync"          , Sql.ToString(Application["CONFIG.Exchange.InboxSync"          ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.PushNotifications"  , Sql.ToString(Application["CONFIG.Exchange.PushNotifications"  ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.PushFrequency"      , Sql.ToString(Application["CONFIG.Exchange.PushFrequency"      ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.PushNotificationURL", Sql.ToString(Application["CONFIG.Exchange.PushNotificationURL"]));
							// 01/17/2017 Paul.  Add support for OAuth. 
							SqlProcs.spCONFIG_Update("mail", "Exchange.ClientID"           , Sql.ToString(Application["CONFIG.Exchange.ClientID"           ]));
							SqlProcs.spCONFIG_Update("mail", "Exchange.ClientSecret"       , Sql.ToString(Application["CONFIG.Exchange.ClientSecret"       ]));
							// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
							SqlProcs.spCONFIG_Update("mail", "Exchange.DirectoryTenantID"  , Sql.ToString(Application["CONFIG.Exchange.DirectoryTenantID"  ]));
							
							// 08/24/2010 Paul.  Enable the ExchangeSync scheduled job. 
							SqlProcs.spSCHEDULERS_UpdateStatus("function::pollExchangeSync", "Active");
							Response.Redirect("../default.aspx");
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
					return;
				}
			}
			else if ( e.CommandName == "Exchange.Authorize" )
			{
				try
				{
					// 11/09/2019 Paul.  Pass the RedirectURL so that we can call from the React client. 
					// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
					SplendidCRM.ActiveDirectory.Office365AcquireAccessToken(Context, OAUTH_DIRECTORY_TENANT_ID.Text, OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text, ExchangeUtils.EXCHANGE_ID, OAUTH_CODE.Value, String.Empty);
					ctlDynamicButtons.ErrorText = L10n.Term("OAuth.LBL_TEST_SUCCESSFUL");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
					return;
				}
			}
			// 12/09/2020 Paul.  Add Sync buttons. 
			else if ( e.CommandName == "Exchange.SyncAll" )
			{
				System.Threading.Thread t = new System.Threading.Thread(ExchangeSync.SyncAllUsers);
				t.Start(Context);
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("../default.aspx");
			}
		}

		// 01/17/2017 Paul.  Add support for OAuth. 
		protected void AUTHENTICATION_METHOD_OAUTH_CheckedChanged(object sender, EventArgs e)
		{
			bool bOAuthEnabled = true;
			trAUTHENTICATION_METHOD_OAUTH   .Visible =  bOAuthEnabled;
			trAUTHENTICATION_METHOD_USERNAME.Visible = !bOAuthEnabled ;
			ctlDynamicButtons.ShowButton("Authorize", bOAuthEnabled);
			ctlFooterButtons .ShowButton("Authorize", bOAuthEnabled);
			OAUTH_DIRECTORY_TENANT_ID    .Visible = bOAuthEnabled;
			LBL_OAUTH_DIRECTORY_TENANT_ID.Visible = bOAuthEnabled;
			Page.DataBind();
		}

		protected void AUTHENTICATION_METHOD_USERNAME_CheckedChanged(object sender, EventArgs e)
		{
			bool bOAuthEnabled = false;
			trAUTHENTICATION_METHOD_OAUTH   .Visible =  bOAuthEnabled;
			trAUTHENTICATION_METHOD_USERNAME.Visible = !bOAuthEnabled ;
			ctlDynamicButtons.ShowButton("Authorize", bOAuthEnabled);
			ctlFooterButtons .ShowButton("Authorize", bOAuthEnabled);
			OAUTH_DIRECTORY_TENANT_ID    .Visible = bOAuthEnabled;
			LBL_OAUTH_DIRECTORY_TENANT_ID.Visible = bOAuthEnabled;
			Page.DataBind();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Exchange.LBL_EXCHANGE_SETTINGS"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				//reqSERVER_URL.DataBind();
				reqUSER_NAME .DataBind();
				reqPASSWORD  .DataBind();
				reqOAUTH_CLIENT_ID    .DataBind();
				reqOAUTH_CLIENT_SECRET.DataBind();
				if ( !IsPostBack )
				{
					// 12/13/2017 Paul.  Allow version to be changed. 
					// 02/05/2023 Paul.  Version is now optional. 
					if ( EXCHANGE_VERSION != null )
					{
						EXCHANGE_VERSION.Items.Add(new ListItem("Exchange 2007 SP1", "Exchange2007_SP1"));
						EXCHANGE_VERSION.Items.Add(new ListItem("Exchange 2010"    , "Exchange2010"    ));
						EXCHANGE_VERSION.Items.Add(new ListItem("Exchange 2010 SP1", "Exchange2010_SP1"));
						EXCHANGE_VERSION.Items.Add(new ListItem("Exchange 2010 SP2", "Exchange2010_SP2"));
						EXCHANGE_VERSION.Items.Add(new ListItem("Exchange 2013"    , "Exchange2013"    ));
						EXCHANGE_VERSION.Items.Add(new ListItem("Exchange 2013 SP1", "Exchange2013_SP1"));
					}

					IMPERSONATED_TYPE.Items.Add(new ListItem(L10n.Term("Exchange.LBL_PRINCIPAL_NAME"  ), "PrincipalName"  ));
					IMPERSONATED_TYPE.Items.Add(new ListItem(L10n.Term("Exchange.LBL_SMTP_ADDRESS"    ), "SmtpAddress"    ));
					// 11/23/2011 Paul.  New option not to impersonate Exchange users. 
					IMPERSONATED_TYPE.Items.Add(new ListItem(L10n.Term("Exchange.LBL_NO_IMPERSONATION"), "NoImpersonation"));

					ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);

					SERVER_URL.Text    = Sql.ToString(Application["CONFIG.Exchange.ServerURL"]);
					USER_NAME .Text    = Sql.ToString(Application["CONFIG.Exchange.UserName" ]);
					string sPASSWORD   = Sql.ToString(Application["CONFIG.Exchange.Password" ]);
					if ( !Sql.IsEmptyString(sPASSWORD) )
					{
						// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
						//MAIL_SMTPPASS.Text = Sql.sEMPTY_PASSWORD;
						PASSWORD.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
					}
					IGNORE_CERTIFICATE.Checked = Sql.ToBoolean(Application["CONFIG.Exchange.IgnoreCertificate"  ]);
					INBOX_ROOT        .Checked = Sql.ToBoolean(Application["CONFIG.Exchange.InboxRoot"          ]);
					SENT_ITEMS_ROOT   .Checked = Sql.ToBoolean(Application["CONFIG.Exchange.SentItemsRoot"      ]);
					// 03/11/2012 Paul.  Import messages from Sent Items if the TO email exists in the CRM. 
					SENT_ITEMS_SYNC   .Checked = Sql.ToBoolean(Application["CONFIG.Exchange.SentItemsSync"      ]);
					// 03/11/2012 Paul.  Default to enabling Sent Items Sync. 
					if ( Sql.IsEmptyString(Application["CONFIG.Exchange.SentItemsSync"]) )
						SENT_ITEMS_SYNC.Checked = true;
					// 07/05/2017 Paul.  Import messages from Inbox if the FROM email exists in the CRM. 
					// 07/05/2017 Paul.  We will not default to importing the inbox as we have not done this for many years. 
					INBOX_SYNC        .Checked = Sql.ToBoolean(Application["CONFIG.Exchange.InboxSync"          ]);
					PUSH_NOTIFICATIONS.Checked = Sql.ToBoolean(Application["CONFIG.Exchange.PushNotifications"  ]);
					PUSH_FREQUENCY       .Text = Sql.ToString (Application["CONFIG.Exchange.PushFrequency"      ]);
					PUSH_NOTIFICATION_URL.Text = Sql.ToString (Application["CONFIG.Exchange.PushNotificationURL"]);
					try
					{
						// 12/13/2017 Paul.  Allow version to be changed. 
						Utils.SetSelectedValue(EXCHANGE_VERSION, Sql.ToString(Application["CONFIG.Exchange.Version"]));
					}
					catch
					{
					}
					try
					{
						// 08/19/2010 Paul.  Check the list before assigning the value. 
						Utils.SetSelectedValue(IMPERSONATED_TYPE, Sql.ToString(Application["CONFIG.Exchange.ImpersonatedType"]));
					}
					catch
					{
					}
					// 01/17/2017 Paul.  Add support for OAuth. 
					bool bOAuthEnabled = !Sql.IsEmptyString(Application["CONFIG.Exchange.ClientID"]);
					if ( Sql.IsEmptyString(Application["CONFIG.Exchange.ClientID"]) && Sql.IsEmptyString(Application["CONFIG.Exchange.UserName"]) )
						bOAuthEnabled = true;
					AUTHENTICATION_METHOD_OAUTH     .Checked =  bOAuthEnabled;
					AUTHENTICATION_METHOD_USERNAME  .Checked = !bOAuthEnabled;
					trAUTHENTICATION_METHOD_OAUTH   .Visible =  bOAuthEnabled;
					trAUTHENTICATION_METHOD_USERNAME.Visible = !bOAuthEnabled;
					OAUTH_CLIENT_ID    .Text = Sql.ToString (Application["CONFIG.Exchange.ClientID"    ]);
					OAUTH_CLIENT_SECRET.Text = Sql.ToString (Application["CONFIG.Exchange.ClientSecret"]);
					// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
					OAUTH_DIRECTORY_TENANT_ID.Text = Sql.ToString(Application["CONFIG.Exchange.DirectoryTenantID"]);
					OAUTH_DIRECTORY_TENANT_ID    .Visible = bOAuthEnabled;
					LBL_OAUTH_DIRECTORY_TENANT_ID.Visible = bOAuthEnabled;

					ctlDynamicButtons.ShowButton("Authorize", bOAuthEnabled);
					ctlFooterButtons .ShowButton("Authorize", bOAuthEnabled);
				}
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Exchange";
			// 07/24/2010 Paul.  We need an admin flag for the areas that don't have a record in the Modules table. 
			SetAdminMenu(m_sMODULE);
			if ( IsPostBack )
			{
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
			}
		}
		#endregion
	}
}
