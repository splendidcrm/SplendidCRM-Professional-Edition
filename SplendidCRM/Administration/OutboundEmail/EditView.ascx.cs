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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.OutboundEmail
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
		//private const string      sEMPTY_PASSWORD = "**********"  ;
		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;
		protected HtmlTable       tblSmtp                         ;
		// 01/15/2017 Paul.  Add support for Office 365 OAuth. 
		protected HiddenField     NEW_ID                          ;
		protected TextBox         OAUTH_ACCESS_TOKEN              ;
		protected TextBox         OAUTH_REFRESH_TOKEN             ;
		protected TextBox         OAUTH_EXPIRES_IN                ;
		protected TextBox         OAUTH_CODE                      ;
		protected Table           tblSmtpPanel                    ;
		protected Table           tblOffice365Panel               ;
		protected HtmlTable       tblOffice365Options             ;
		protected Button          btnOffice365Authorize           ;
		protected Button          btnOffice365Delete              ;
		protected Button          btnOffice365Test                ;
		protected Button          btnOffice365Authorized          ;
		protected Button          btnOffice365RefreshToken        ;
		protected Label           lblOffice365Authorized          ;
		protected Label           lblOffice365AuthorizedStatus    ;
		protected Table           tblGoogleAppsPanel              ;
		protected HtmlTable       tblGoogleAppsOptions            ;
		protected Button          btnGoogleAppsAuthorize          ;
		protected Button          btnGoogleAppsDelete             ;
		protected Button          btnGoogleAppsTest               ;
		protected Button          btnGoogleAuthorized             ;
		protected Button          btnGoogleAppsRefreshToken       ;
		protected Label           lblGoogleAppsAuthorized         ;
		protected Label           lblGoogleAuthorizedStatus       ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView   );

					// 01/15/2017 Paul.  Add support for Office 365 OAuth. 
					string sMAIL_SENDTYPE = new DynamicControl(this, "MAIL_SENDTYPE").SelectedValue;
					if ( Sql.IsEmptyString(sMAIL_SENDTYPE) )
						sMAIL_SENDTYPE = "smtp";
					if ( String.Compare(sMAIL_SENDTYPE, "smtp", true) == 0 )
					{
						this.ValidateEditViewFields(m_sMODULE + ".SmtpView");
					}
					// 01/31/2017 Paul.  Add support for Exchange using Username/Password. 
					else if ( String.Compare(sMAIL_SENDTYPE, "Exchange-Password", true) == 0 )
					{
						this.ValidateEditViewFields(m_sMODULE + ".ExchangeView");
						new DynamicControl(this, "MAIL_SMTPSERVER"  ).Text    = String.Empty;
						new DynamicControl(this, "MAIL_SMTPPORT"    ).Text    = String.Empty;
						new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Checked = false;
						new DynamicControl(this, "MAIL_SMTPSSL"     ).Checked = false;
					}
					else
					{
						new DynamicControl(this, "MAIL_SMTPSERVER"  ).Text    = String.Empty;
						new DynamicControl(this, "MAIL_SMTPPORT"    ).Text    = String.Empty;
						new DynamicControl(this, "MAIL_SMTPUSER"    ).Text    = String.Empty;
						new DynamicControl(this, "MAIL_SMTPPASS"    ).Text    = String.Empty;
						new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Checked = false;
						new DynamicControl(this, "MAIL_SMTPSSL"     ).Checked = false;
					}
					if ( Page.IsValid )
					{
						string sMAIL_SMTPPASS = String.Empty;
						// 01/31/2017 Paul.  Add support for Exchange using Username/Password. 
						if ( String.Compare(sMAIL_SENDTYPE, "smtp", true) == 0 || String.Compare(sMAIL_SENDTYPE, "Exchange-Password", true) == 0 )
						{
							Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
							Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
							// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
							sMAIL_SMTPPASS = Sql.sEMPTY_PASSWORD;
							TextBox MAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
							if ( MAIL_SMTPPASS != null )
								sMAIL_SMTPPASS = MAIL_SMTPPASS.Text;
							if ( sMAIL_SMTPPASS == Sql.sEMPTY_PASSWORD )
							{
								sMAIL_SMTPPASS = Sql.ToString(ViewState["smtppass"]);
							}
							else
							{
								string sENCRYPTED_MAIL_SMTPPASS = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
								if ( Security.DecryptPassword(sENCRYPTED_MAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sMAIL_SMTPPASS )
									throw(new Exception("Decryption failed"));
								sMAIL_SMTPPASS = sENCRYPTED_MAIL_SMTPPASS;
								if ( MAIL_SMTPPASS != null )
									MAIL_SMTPPASS.Attributes.Add("value", sMAIL_SMTPPASS);
							}
						}
						else if ( String.Compare(sMAIL_SENDTYPE, "Office365", true) == 0 )
						{
							string sOAuthClientID     = Sql.ToString(Application["CONFIG.Exchange.ClientID"    ]);
							string sOAuthClientSecret = Sql.ToString(Application["CONFIG.Exchange.ClientSecret"]);
							// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
							string sOAuthDirectoryTenatID = Sql.ToString(Application["CONFIG.Exchange.DirectoryTenantID"]);
							SplendidCRM.ActiveDirectory.Office365RefreshAccessToken(Application, sOAuthDirectoryTenatID, sOAuthClientID, sOAuthClientSecret, gID, false);
						}
						else if ( String.Compare(sMAIL_SENDTYPE, "GoogleApps", true) == 0 )
						{
							SplendidCRM.GoogleApps.RefreshAccessToken(Application, gID, false);
						}
						
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *                     " + ControlChars.CrLf
								     + "  from vwOUTBOUND_EMAILS_Edit" + ControlChars.CrLf
								     + " where ID = @ID              " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											rowCurrent = dtCurrent.Rows[0];
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											// 01/17/2017 Paul.  We do not want to clear the ID because we want to use the NEW_ID value. 
											//gID = Guid.Empty;
										}
									}
								}
							}
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									bool bMAIL_SMTPSSL = new DynamicControl(this, rowCurrent, "MAIL_SMTPSSL").Checked && (sMAIL_SENDTYPE == "smtp");
									SqlProcs.spOUTBOUND_EMAILS_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "NAME"             ).Text
										, new DynamicControl(this, rowCurrent, "TYPE"             ).SelectedValue
										// 02/04/2017 Paul.  USER_ID must be null for global outbound records. 
										, Guid.Empty   // USER_ID
										, new DynamicControl(this, rowCurrent, "MAIL_SENDTYPE"    ).SelectedValue
										, new DynamicControl(this, rowCurrent, "MAIL_SMTPTYPE"    ).SelectedValue
										, new DynamicControl(this, rowCurrent, "MAIL_SMTPSERVER"  ).Text
										, new DynamicControl(this, rowCurrent, "MAIL_SMTPPORT"    ).IntegerValue
										, new DynamicControl(this, rowCurrent, "MAIL_SMTPUSER"    ).Text
										, sMAIL_SMTPPASS
										, new DynamicControl(this, rowCurrent, "MAIL_SMTPAUTH_REQ").Checked
										, (bMAIL_SMTPSSL ? 1 : 0)
										, new DynamicControl(this, rowCurrent, "FROM_NAME"        ).Text
										, new DynamicControl(this, rowCurrent, "FROM_ADDR"        ).Text
										// 04/20/2016 Paul.  Add team management to Outbound Emails. 
										, new DynamicControl(this, rowCurrent, "TEAM_ID"          ).ID
										, new DynamicControl(this, rowCurrent, "TEAM_SET_LIST"    ).Text
										, trn
										);
									SqlProcs.spTRACKER_Update
										( Security.USER_ID
										, m_sMODULE
										, gID
										, new DynamicControl(this, rowCurrent, "NAME").Text
										, "save"
										, trn
										);
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									ctlDynamicButtons.ErrorText = ex.Message;
									return;
								}
							}
						}
						SplendidCache.ClearOutboundMail();
						Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Test" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					string sMAIL_SENDTYPE = new DynamicControl(this, "MAIL_SENDTYPE").SelectedValue;
					if ( Sql.IsEmptyString(sMAIL_SENDTYPE) )
						sMAIL_SENDTYPE = "smtp";
					if ( String.Compare(sMAIL_SENDTYPE, "smtp", true) == 0 )
					{
						this.ValidateEditViewFields(m_sMODULE + ".SmtpView"    );
					}
					// 01/31/2017 Paul.  Add support for Exchange using Username/Password. 
					else if ( String.Compare(sMAIL_SENDTYPE, "Exchange-Password", true) == 0 )
					{
						this.ValidateEditViewFields(m_sMODULE + ".ExchangeView");
					}

					if ( Page.IsValid )
					{
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
						// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
						string sMAIL_SMTPPASS = Sql.sEMPTY_PASSWORD;
						TextBox MAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
						if ( MAIL_SMTPPASS != null )
							sMAIL_SMTPPASS = MAIL_SMTPPASS.Text;
						// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
						if ( sMAIL_SMTPPASS == Sql.sEMPTY_PASSWORD )
						{
							sMAIL_SMTPPASS = Sql.ToString(ViewState["smtppass"]);
							if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
								sMAIL_SMTPPASS = Security.DecryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						}
						else
						{
							string sENCRYPTED_MAIL_SMTPPASS = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							if ( Security.DecryptPassword(sENCRYPTED_MAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sMAIL_SMTPPASS )
								throw(new Exception("Decryption failed"));
							if ( MAIL_SMTPPASS != null )
							{
								// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
								MAIL_SMTPPASS.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
							}
						}
						string sMAIL_SMTPUSER = new DynamicControl(this, "MAIL_SMTPUSER").Text;
						string sFROM_NAME     = new DynamicControl(this, "FROM_NAME"    ).Text;
						string sFROM_ADDR     = new DynamicControl(this, "FROM_ADDR"    ).Text;
						if ( String.Compare(sMAIL_SENDTYPE, "smtp", true) == 0 )
						{
							string sMAIL_SMTPSERVER   = new DynamicControl(this, "MAIL_SMTPSERVER"   ).Text;
							int    nMAIL_SMTPPORT     = new DynamicControl(this, "MAIL_SMTPPORT"     ).IntegerValue;
							bool   bMAIL_SMTPAUTH_REQ = new DynamicControl(this, "MAIL_SMTPAUTH_REQ" ).Checked;
							bool   bMAIL_SMTPSSL      = new DynamicControl(this, "MAIL_SMTPSSL"      ).Checked;
							EmailUtils.SendTestMessage(Application, sMAIL_SMTPSERVER, nMAIL_SMTPPORT, bMAIL_SMTPAUTH_REQ, bMAIL_SMTPSSL, sMAIL_SMTPUSER, sMAIL_SMTPPASS, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
							ctlDynamicButtons.ErrorText = "Send was successful.";
						}
						// 01/31/2017 Paul.  Add support for Exchange using Username/Password. 
						else if ( String.Compare(sMAIL_SENDTYPE, "Exchange-Password", true) == 0 )
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							string sIMPERSONATED_TYPE        = Sql.ToString (Application["CONFIG.Exchange.ImpersonatedType"]);
							string sSERVER_URL               = Sql.ToString (Application["CONFIG.Exchange.ServerURL"       ]);
							ExchangeUtils.SendTestMessage(Application, sSERVER_URL, sMAIL_SMTPUSER, sENCRYPTED_EMAIL_PASSWORD, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
							ctlDynamicButtons.ErrorText = "Send was successful.";
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			// 01/17/2017 Paul.  Google now uses OAuth 2.0. 
			else if ( e.CommandName == "GoogleApps.Test" )
			{
				try
				{
					//StringBuilder sbErrors = new StringBuilder();
					//SplendidCRM.GoogleApps.TestAccessToken(Application, gID, sbErrors);
					//lblGoogleAuthorizedStatus.Text = sbErrors.ToString();
					string sFROM_NAME = new DynamicControl(this, "FROM_NAME").Text;
					string sFROM_ADDR = new DynamicControl(this, "FROM_ADDR").Text;
					GoogleApps.SendTestMessage(Application, gID, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
					lblGoogleAuthorizedStatus.Text = "Send was successful.";
				}
				catch(Exception ex)
				{
					lblGoogleAuthorizedStatus.Text = ex.Message;
				}
			}
			else if ( e.CommandName == "GoogleApps.Authorize" )
			{
				try
				{
					DateTime dtOAUTH_EXPIRES_AT = DateTime.Now.AddSeconds(Sql.ToInteger(OAUTH_EXPIRES_IN.Text));
					// 01/19/2017 Paul.  Name must match SEND_TYPE. 
					SqlProcs.spOAUTH_TOKENS_Update(gID, "GoogleApps", OAUTH_ACCESS_TOKEN.Text, String.Empty, dtOAUTH_EXPIRES_AT, OAUTH_REFRESH_TOKEN.Text);
					Application["CONFIG.GoogleApps." + gID.ToString() + ".OAuthAccessToken" ] = OAUTH_ACCESS_TOKEN.Text ;
					Application["CONFIG.GoogleApps." + gID.ToString() + ".OAuthRefreshToken"] = OAUTH_REFRESH_TOKEN.Text;
					Application["CONFIG.GoogleApps." + gID.ToString() + ".OAuthExpiresAt"   ] = dtOAUTH_EXPIRES_AT.ToShortDateString() + " " + dtOAUTH_EXPIRES_AT.ToShortTimeString();
					lblGoogleAuthorizedStatus.Text = L10n.Term("OAuth.LBL_TEST_SUCCESSFUL");
					btnGoogleAppsAuthorize   .Visible = false;
					btnGoogleAppsDelete      .Visible = true ;
					btnGoogleAppsTest        .Visible = true ;
					btnGoogleAppsRefreshToken.Visible = true && bDebug;
					lblGoogleAppsAuthorized  .Visible = true ;
					// 02/09/2017 Paul.  Update the email address. 
					StringBuilder sbErrors = new StringBuilder();
					new DynamicControl(this, "FROM_ADDR").Text = SplendidCRM.GoogleApps.GetEmailAddress(Application, gID, sbErrors);
				}
				catch(Exception ex)
				{
					lblGoogleAuthorizedStatus.Text = ex.Message;
				}
			}
			else if ( e.CommandName == "GoogleApps.Delete" )
			{
				try
				{
					// 01/19/2017 Paul.  Name must match SEND_TYPE. 
					SqlProcs.spOAUTH_TOKENS_Delete(gID, "GoogleApps");
					btnGoogleAppsAuthorize   .Visible = true ;
					btnGoogleAppsDelete      .Visible = false;
					btnGoogleAppsTest        .Visible = false;
					btnGoogleAppsRefreshToken.Visible = false;
					lblGoogleAppsAuthorized  .Visible = false;
				}
				catch(Exception ex)
				{
					lblOffice365AuthorizedStatus.Text =  Utils.ExpandException(ex);
				}
			}
			else if ( e.CommandName == "GoogleApps.RefreshToken" )
			{
				try
				{
					SplendidCRM.GoogleApps.RefreshAccessToken(Application, gID, true);
					lblGoogleAuthorizedStatus.Text = L10n.Term("OAuth.LBL_TEST_SUCCESSFUL");
				}
				catch(Exception ex)
				{
					lblGoogleAuthorizedStatus.Text =  Utils.ExpandException(ex);
				}
			}
			// 01/16/2017 Paul.  Add support for Office 365 OAuth. 
			else if ( e.CommandName == "Office365.Authorize" )
			{
				try
				{
					string sOAuthClientID     = Sql.ToString(Application["CONFIG.Exchange.ClientID"    ]);
					string sOAuthClientSecret = Sql.ToString(Application["CONFIG.Exchange.ClientSecret"]);
					// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
					string sOAuthDirectoryTenatID = Sql.ToString(Application["CONFIG.Exchange.DirectoryTenantID"]);
					// 11/09/2019 Paul.  Pass the RedirectURL so that we can call from the React client. 
					Office365AccessToken token = SplendidCRM.ActiveDirectory.Office365AcquireAccessToken(Context, sOAuthDirectoryTenatID, sOAuthClientID, sOAuthClientSecret, gID, OAUTH_CODE.Text, String.Empty);
					lblOffice365AuthorizedStatus.Text = L10n.Term("OAuth.LBL_TEST_SUCCESSFUL");
					btnOffice365Authorize   .Visible = false;
					btnOffice365Delete      .Visible = true ;
					btnOffice365Test        .Visible = true ;
					btnOffice365RefreshToken.Visible = true && bDebug;
					lblOffice365Authorized  .Visible = true ;
					// 02/09/2017 Paul.  Use Microsoft Graph REST API to get email. 
					MicrosoftGraphProfile profile = SplendidCRM.ActiveDirectory.GetProfile(Application, token.AccessToken);
					if ( profile != null )
					{
						new DynamicControl(this, "FROM_NAME").Text = Sql.ToString(profile.Name        );
						new DynamicControl(this, "FROM_ADDR").Text = Sql.ToString(profile.EmailAddress);
					}
				}
				catch(Exception ex)
				{
					lblOffice365AuthorizedStatus.Text =  Utils.ExpandException(ex);
				}
			}
			else if ( e.CommandName == "Office365.RefreshToken" )
			{
				try
				{
					string sOAuthClientID     = Sql.ToString(Application["CONFIG.Exchange.ClientID"    ]);
					string sOAuthClientSecret = Sql.ToString(Application["CONFIG.Exchange.ClientSecret"]);
					// 02/04/2023 Paul.  Directory Tenant is now required for single tenant app registrations. 
					string sOAuthDirectoryTenatID = Sql.ToString(Application["CONFIG.Exchange.DirectoryTenantID"]);
					SplendidCRM.ActiveDirectory.Office365RefreshAccessToken(Application, sOAuthDirectoryTenatID, sOAuthClientID, sOAuthClientSecret, gID, true);
					lblOffice365AuthorizedStatus.Text = L10n.Term("OAuth.LBL_TEST_SUCCESSFUL");
				}
				catch(Exception ex)
				{
					lblOffice365AuthorizedStatus.Text =  Utils.ExpandException(ex);
				}
			}
			else if ( e.CommandName == "Office365.Delete" )
			{
				try
				{
					SqlProcs.spOAUTH_TOKENS_Delete(gID, "Office365");
					btnOffice365Authorize   .Visible = true ;
					btnOffice365Delete      .Visible = false;
					btnOffice365Test        .Visible = false;
					btnOffice365RefreshToken.Visible = false;
					lblOffice365Authorized  .Visible = false;
				}
				catch(Exception ex)
				{
					lblOffice365AuthorizedStatus.Text =  Utils.ExpandException(ex);
				}
			}
			else if ( e.CommandName == "Office365.Test" )
			{
				try
				{
					//StringBuilder sbErrors = new StringBuilder();
					//string sOAuthClientID     = Sql.ToString(Application["CONFIG.Exchange.ClientID"    ]);
					//string sOAuthClientSecret = Sql.ToString(Application["CONFIG.Exchange.ClientSecret"]);
					//SplendidCRM.ActiveDirectory.Office365TestAccessToken(Application, sOAuthClientID, sOAuthClientSecret, gID, sbErrors);
					//lblOffice365AuthorizedStatus.Text = sbErrors.ToString();
					string sFROM_NAME = new DynamicControl(this, "FROM_NAME").Text;
					string sFROM_ADDR = new DynamicControl(this, "FROM_ADDR").Text;
					// 12/13/2020 Paul.  Move Office365 methods to Office365utils. 
					Office365Utils.SendTestMessage(Application, gID, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
					lblOffice365AuthorizedStatus.Text = "Send was successful.";
				}
				catch(Exception ex)
				{
					lblOffice365AuthorizedStatus.Text =  Utils.ExpandException(ex);
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		protected void MAIL_SENDTYPE_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownList lstMAIL_SENDTYPE = FindControl("MAIL_SENDTYPE") as DropDownList;
			if ( lstMAIL_SENDTYPE != null )
			{
				string sMAIL_SENDTYPE = lstMAIL_SENDTYPE.SelectedValue;
				bool bSmtp = (String.Compare(sMAIL_SENDTYPE, "smtp", true) == 0 || sMAIL_SENDTYPE == String.Empty);
				// 01/31/2017 Paul.  Add support for Exchange using Username/Password. 
				bool bExchange = (String.Compare(sMAIL_SENDTYPE, "Exchange-Password", true) == 0);
				tblSmtpPanel.Visible = bSmtp || bExchange;
				new DynamicControl(this, "MAIL_SMTPSERVER"                ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPSERVER_LABEL"          ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPSERVER_REQUIRED_SYMBOL").Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPPORT"                  ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPPORT_LABEL"            ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPPORT_REQUIRED_SYMBOL"  ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPAUTH_REQ"              ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPAUTH_REQ_LABEL"        ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPSSL"                   ).Visible = bSmtp;
				new DynamicControl(this, "MAIL_SMTPSSL_LABEL"             ).Visible = bSmtp;
				RequiredFieldValidator reqSERVER_URL = FindControl("MAIL_SMTPSERVER_REQUIRED") as RequiredFieldValidator;
				if ( reqSERVER_URL != null )
				{
					reqSERVER_URL.Enabled = bSmtp;
					reqSERVER_URL.EnableClientScript = bSmtp;
				}
				RequiredFieldValidator reqPORT = FindControl("MAIL_SMTPPORT_REQUIRED") as RequiredFieldValidator;
				if ( reqPORT != null )
				{
					reqPORT.Enabled = bSmtp;
					reqPORT.EnableClientScript = bSmtp;
				}

				tblOffice365Panel .Visible = (String.Compare(sMAIL_SENDTYPE, "Office365" , true) == 0);
				tblGoogleAppsPanel.Visible = (String.Compare(sMAIL_SENDTYPE, "GoogleApps", true) == 0);
				ctlDynamicButtons.ShowButton("Test", bSmtp || bExchange);
				ctlFooterButtons .ShowButton("Test", bSmtp || bExchange);

				if ( Sql.IsEmptyString(Application["CONFIG.Exchange.ClientID"]) || Sql.IsEmptyString(Application["CONFIG.Exchange.ClientSecret"]) )
				{
					lblOffice365AuthorizedStatus.Text = L10n.Term("OutboundEmail.LBL_OFFICE365_NOT_ENABLED");
					lblOffice365AuthorizedStatus.CssClass = "error";
				}
				if ( !Sql.ToBoolean(Context.Application["CONFIG.GoogleApps.Enabled"]) )
				{
					lblGoogleAuthorizedStatus.Text = L10n.Term("OutboundEmail.LBL_GOOGLEAPPS_NOT_ENABLED");
					lblGoogleAuthorizedStatus.CssClass = "error";
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					// 01/17/2017 Paul.  The NEW_ID field is used in case we need to save OAuth for a new record. 
					if ( !Sql.IsEmptyGuid(gID) )
						NEW_ID.Value = gID.ToString();
					else
						NEW_ID.Value = Guid.NewGuid().ToString();
					//Debug.WriteLine("NEW_ID = " + NEW_ID.Value);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwOUTBOUND_EMAILS_Edit" + ControlChars.CrLf
							     + " where ID = @ID              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
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
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);

											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											this.AppendEditViewFields(m_sMODULE + ".SmtpView"         , tblSmtp, rdr);
											// 01/17/2017 Paul.  Separate view for Smtp values
											DropDownList lstMAIL_SENDTYPE = FindControl("MAIL_SENDTYPE") as DropDownList;
											if ( lstMAIL_SENDTYPE != null )
											{
												lstMAIL_SENDTYPE.AutoPostBack = true;
												lstMAIL_SENDTYPE.SelectedIndexChanged += new EventHandler(MAIL_SENDTYPE_SelectedIndexChanged);
												MAIL_SENDTYPE_SelectedIndexChanged(null, null);
											}
											string sMAIL_SMTPPASS = Sql.ToString(rdr["MAIL_SMTPPASS"]);
											TextBox MAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
											if ( MAIL_SMTPPASS != null )
												MAIL_SMTPPASS.TextMode = TextBoxMode.Password;
											if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
											{
												ViewState["smtppass"] = sMAIL_SMTPPASS;
												if ( MAIL_SMTPPASS != null )
												{
													// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
													MAIL_SMTPPASS.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
												}
											}
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											// 01/15/2017 Paul.  Add support for Office 365 OAuth. 
											try
											{
												bool bOFFICE365_OAUTH_ENABLED = Sql.ToBoolean(rdr["OFFICE365_OAUTH_ENABLED"]);
												btnOffice365Authorize   .Visible = !bOFFICE365_OAUTH_ENABLED && !Sql.IsEmptyString(Application["CONFIG.Exchange.ClientID"]) && !Sql.IsEmptyString(Application["CONFIG.Exchange.ClientSecret"]);
												btnOffice365Delete      .Visible =  bOFFICE365_OAUTH_ENABLED;
												btnOffice365Test        .Visible =  bOFFICE365_OAUTH_ENABLED;
												btnOffice365RefreshToken.Visible =  bOFFICE365_OAUTH_ENABLED && bDebug;
												lblOffice365Authorized  .Visible =  bOFFICE365_OAUTH_ENABLED;
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), "OFFICE365_OAUTH_ENABLED is not defined. " + ex.Message);
											}
											try
											{
												bool bGOOGLEAPPS_OAUTH_ENABLED = Sql.ToBoolean(rdr["GOOGLEAPPS_OAUTH_ENABLED"]);
												btnGoogleAppsAuthorize   .Visible = !bGOOGLEAPPS_OAUTH_ENABLED && Sql.ToBoolean(Context.Application["CONFIG.GoogleApps.Enabled"]);;
												btnGoogleAppsDelete      .Visible =  bGOOGLEAPPS_OAUTH_ENABLED;
												btnGoogleAppsTest        .Visible =  bGOOGLEAPPS_OAUTH_ENABLED;
												btnGoogleAppsRefreshToken.Visible =  bGOOGLEAPPS_OAUTH_ENABLED && bDebug;
												lblGoogleAppsAuthorized  .Visible =  bGOOGLEAPPS_OAUTH_ENABLED;
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), "GOOGLEAPPS_OAUTH_ENABLED is not defined. " + ex.Message);
											}
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
						this.AppendEditViewFields(m_sMODULE + ".SmtpView"         , tblSmtp, null);
						// 01/17/2017 Paul.  Separate view for Smtp values
						DropDownList lstMAIL_SENDTYPE = FindControl("MAIL_SENDTYPE") as DropDownList;
						if ( lstMAIL_SENDTYPE != null )
						{
							lstMAIL_SENDTYPE.AutoPostBack = true;
							lstMAIL_SENDTYPE.SelectedIndexChanged += new EventHandler(MAIL_SENDTYPE_SelectedIndexChanged);
							MAIL_SENDTYPE_SelectedIndexChanged(null, null);
						}
						TextBox MAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
						if ( MAIL_SMTPPASS != null )
							MAIL_SMTPPASS.TextMode = TextBoxMode.Password;
						btnOffice365Authorize .Visible = true;
						bool bOFFICE365_OAUTH_ENABLED = false;
						btnOffice365Authorize   .Visible = !bOFFICE365_OAUTH_ENABLED && !Sql.IsEmptyString(Application["CONFIG.Exchange.ClientID"]) && !Sql.IsEmptyString(Application["CONFIG.Exchange.ClientSecret"]);
						btnOffice365Delete      .Visible =  bOFFICE365_OAUTH_ENABLED;
						btnOffice365Test        .Visible =  bOFFICE365_OAUTH_ENABLED;
						btnOffice365RefreshToken.Visible =  bOFFICE365_OAUTH_ENABLED && bDebug;
						lblOffice365Authorized  .Visible =  bOFFICE365_OAUTH_ENABLED;
						bool bGOOGLEAPPS_OAUTH_ENABLED = false;
						btnGoogleAppsAuthorize   .Visible = !bGOOGLEAPPS_OAUTH_ENABLED && Sql.ToBoolean(Context.Application["CONFIG.GoogleApps.Enabled"]);;
						btnGoogleAppsDelete      .Visible =  bGOOGLEAPPS_OAUTH_ENABLED;
						btnGoogleAppsTest        .Visible =  bGOOGLEAPPS_OAUTH_ENABLED;
						btnGoogleAppsRefreshToken.Visible =  bGOOGLEAPPS_OAUTH_ENABLED && bDebug;
						lblGoogleAppsAuthorized  .Visible =  bGOOGLEAPPS_OAUTH_ENABLED;
					}
				}
				else
				{
					if ( Sql.IsEmptyGuid(gID) )
						gID = Sql.ToGuid(NEW_ID.Value);
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
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
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			m_sMODULE = "OutboundEmail";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				this.AppendEditViewFields(m_sMODULE + ".SmtpView"         , tblSmtp, null);
				// 01/17/2017 Paul.  Separate view for Smtp values
				DropDownList lstMAIL_SENDTYPE = FindControl("MAIL_SENDTYPE") as DropDownList;
				if ( lstMAIL_SENDTYPE != null )
				{
					lstMAIL_SENDTYPE.AutoPostBack = true;
					lstMAIL_SENDTYPE.SelectedIndexChanged += new EventHandler(MAIL_SENDTYPE_SelectedIndexChanged);
				}
				TextBox MAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
				if ( MAIL_SMTPPASS != null )
					MAIL_SMTPPASS.TextMode = TextBoxMode.Password;
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}

