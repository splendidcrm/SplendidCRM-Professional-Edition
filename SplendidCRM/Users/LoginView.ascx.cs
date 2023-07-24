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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Threading;
using System.Net.Mail;
using System.Diagnostics;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for LoginView.
	/// </summary>
	public class LoginView : SplendidControl
	{
		#region Properties
		protected _controls.ModuleHeader ctlModuleHeader;

		protected Label           lblError                        ;
		protected TextBox         txtUSER_NAME                    ;
		protected TextBox         txtPASSWORD                     ;
		protected Table           tblUser                         ;
		protected TableRow        trError                         ;
		protected TableRow        trUserName                      ;
		protected TableRow        trPassword                      ;
		protected HyperLink       lnkWorkOnline                   ;
		protected HyperLink       lnkHTML5Client                  ;
		protected HyperLink       lnkReactClient                  ;
		
		protected TextBox         txtFORGOT_USER_NAME             ;
		protected TextBox         txtFORGOT_EMAIL                 ;
		protected Panel           pnlForgotPassword               ;
		protected Table           tblForgotUser                   ;
		protected TableRow        trForgotError                   ;
		protected Label           lblForgotError                  ;
		protected FacebookLogin   ctlFacebookLogin                ;
		// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout.
		protected TableCell       trShowForgotPassword            ;
		protected string          sTheme                          ;
		#endregion

		// 02/18/2020 Paul.  Allow React Client to forget password. 
		// 10/30/2021 Paul.  Move SendForgotPasswordNotice to ModuleUtils. 

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Login" && Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]) )
			{
				// 01/13/2017 Paul.  This event only applies after an initial login failure.  It is for a second login attempt. 
				string sRequestURL = ActiveDirectory.AzureLogin(Context);
				Response.Redirect(sRequestURL);
			}
			else if ( e.CommandName == "Login" && Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled"]) )
			{
				// 01/13/2017 Paul.  This event only applies after an initial login failure.  It is for a second login attempt. 
				string sRequestURL = ActiveDirectory.FederationServicesLogin(Context);
				Response.Redirect(sRequestURL);
			}
			else if ( e.CommandName == "Login" || e.CommandName == "Login.Facebook" )
			{
				trForgotError.Visible = false;
				lblForgotError.Text = String.Empty;
				if ( Page.IsValid )
				{
					bool bValidUser  = false;
					try
					{
						if ( Security.IsWindowsAuthentication() )
						{
							bValidUser = true;
						}
						else
						{
							// 03/19/2011 Paul.  If the facebook user has been authenticated, then all we will have is the user name. 
							if ( e.CommandName == "Login.Facebook" )
							{
								string sAppID     = Sql.ToString(Application["CONFIG.facebook.AppID"    ]);
								string sAppSecret = Sql.ToString(Application["CONFIG.facebook.AppSecret"]);
								
								FacebookUtils fbUtils = new FacebookUtils(sAppID, sAppSecret, Request.Cookies);
								if ( fbUtils.FacebookValuesExist )
								{
									fbUtils.ParseCookie();
									if ( !fbUtils.IsValidSignature() )
									{
										throw(new Exception(L10n.Term("Users.ERR_FACEBOOK_SIGNATURE")));
									}
									bValidUser = SplendidInit.FacebookLoginUser(fbUtils.UID);
									if ( !bValidUser  )
									{
										throw(new Exception(L10n.Term("Users.ERR_FACEBOOK_LOGIN")));
									}
								}
								else
								{
									throw(new Exception(L10n.Term("Users.ERR_FACEBOOK_COOKIE")));
								}
							}
							else
							{
								// 02/20/2011 Paul.  Skip the login if the user has been locked. 
								// 04/16/2013 Paul.  Throw an exception so that we can track lockout count failures in the error log. 
								if ( SplendidInit.LoginFailures(Application, txtUSER_NAME.Text) >= Crm.Password.LoginLockoutCount(Application) )
								{
									L10N L10n = new L10N("en-US");
									throw(new Exception(L10n.Term("Users.ERR_USER_LOCKED_OUT")));
								}
								// 04/16/2013 Paul.  Allow system to be restricted by IP Address. 
								if ( SplendidInit.InvalidIPAddress(Application, Request.UserHostAddress) )
								{
									L10N L10n = new L10N("en-US");
									throw(new Exception(L10n.Term("Users.ERR_INVALID_IP_ADDRESS")));
								}
								bValidUser = SplendidInit.LoginUser(txtUSER_NAME.Text, txtPASSWORD.Text, String.Empty, String.Empty);
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						trError.Visible = true;
						lblError.Text = ex.Message;
						return;
					}
					// 09/12/2006 Paul.  Move redirect outside try/catch to avoid catching "Thread was being aborted" exception. 
					if ( bValidUser )
					{
						// 02/22/2011 Paul.  The login redirect is also needed after the change password. 
						LoginRedirect();
						return;
					}
					else
					{
						trError.Visible = true;
						lblError.Text = L10n.Term("Users.ERR_INVALID_PASSWORD");
					}
				}
			}
			else if ( e.CommandName == "ForgotPassword" )
			{
				trError.Visible = false;
				lblError.Text = String.Empty;
				pnlForgotPassword.Style.Remove("display");
				try
				{
					txtFORGOT_USER_NAME.Text = txtFORGOT_USER_NAME.Text.Trim();
					txtFORGOT_EMAIL    .Text = txtFORGOT_EMAIL    .Text.Trim();
					// 10/30/2021 Paul.  Move SendForgotPasswordNotice to ModuleUtils. 
					lblForgotError.Text = ModuleUtils.Login.SendForgotPasswordNotice(Application, txtFORGOT_USER_NAME.Text, txtFORGOT_EMAIL.Text);
					trForgotError.Visible = true;
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					trForgotError.Visible = true;
					lblForgotError.Text = ex.Message;
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 12/27/2008 Paul.  Set the page title so that a bookmark will not default to "Login". 
			// 08/18/2011 Paul.  Make sure to use the terminology table for the browser title. 
			SetPageTitle(L10n.Term(".LBL_BROWSER_TITLE"));
			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				if ( !IsPostBack )
				{
					string sDefaultUserName = Sql.ToString(Application["CONFIG.default_user_name"]);
					string sDefaultPassword = Sql.ToString(Application["CONFIG.default_password" ]);
					string sDefaultTheme    = Sql.ToString(Application["CONFIG.default_theme"    ]);
					string sDefaultLanguage = Sql.ToString(Application["CONFIG.default_language" ]);
					txtUSER_NAME.Text = sDefaultUserName;
					txtPASSWORD.Text  = sDefaultPassword;

					// 11/19/2009 Paul.  File IO is expensive, so cache the results of the Exists test. 
					// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
					lnkWorkOnline .Visible = Utils.CachedFileExists(Context, "~/Users/ClientLogin.aspx");
					// 05/25/2020 Paul.  Disable easy access to HTML5 client as the React Client will replace it. 
					lnkHTML5Client.Visible = false; // Utils.CachedFileExists(Context, "~/html5/default.aspx"    );
					// 05/25/2020 Paul.  React Client link on old login page. 
					lnkReactClient.Visible = Utils.CachedFileExists(Context, "~/React/default.aspx"    );
					// 03/19/2011 Paul.  Facebook login does not make sense on the offline client. 
					ctlFacebookLogin.Visible = !lnkWorkOnline.Visible && Sql.ToBoolean(Application["CONFIG.facebook.EnableLogin"]) && !Sql.IsEmptyString(Application["CONFIG.facebook.AppID"]);
					// 04/08/2016 Paul.  Provide way to force HTTPS. 
					if ( Sql.ToBoolean(Application["CONFIG.Site.Https"]) && Request.Url.Scheme.ToLower() == "http" )
						Response.Redirect("https://" + Request.Url.Host + Request.Url.PathAndQuery);

					// 01/08/2017 Paul.  Add support for ADFS Single-Sign-On.  Using WS-Federation Passive authentication (browser redirect). 
					if ( Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled"]) )
					{
						txtUSER_NAME     .Visible = false;
						txtPASSWORD      .Visible = false;
						lnkWorkOnline    .Visible = false;
						lnkHTML5Client   .Visible = false;
						ctlFacebookLogin .Visible = false;
						pnlForgotPassword.Visible = false;
						trUserName       .Visible = false;
						trPassword       .Visible = false;
						if ( trShowForgotPassword != null )
							trShowForgotPassword.Visible = false;
						if ( Sql.IsEmptyString(Request["error"]) )
						{
							if ( Sql.ToString(Request.Form["wa"]) == "wsignin1.0" && !Sql.IsEmptyString(Request.Form["wresult"]) )
							{
								string sError = String.Empty;
								string sToken = Sql.ToString(Request.Form["wresult"]);
								Guid gUSER_ID = ActiveDirectory.FederationServicesValidate(Context, sToken, ref sError);
								if ( !Sql.IsEmptyGuid(gUSER_ID) )
								{
									SplendidInit.LoginUser(gUSER_ID, "ASDF");
									LoginRedirect();
								}
								else
								{
									trError.Visible = true;
									lblError.Text = L10n.Term("Users.ERR_INVALID_USER") + "<br />" + sError;
								}
							}
							// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout.  Signout is in the URL, not the form. 
							else if (  Sql.ToString(Request["wa"]) == "wsignoutcleanup1.0" )
							{
								// 12/25/2018 Paul.  After logout, leave the login screen with the login button. 
							}
							else
							{
								string sRequestURL = ActiveDirectory.FederationServicesLogin(Context);
								Response.Redirect(sRequestURL);
							}
						}
						else
						{
							trError.Visible = true;
							lblError.Text = Sql.ToString(Request["error_description"]);
						}
					}
					else if ( Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]) )
					{
						txtUSER_NAME     .Visible = false;
						txtPASSWORD      .Visible = false;
						lnkWorkOnline    .Visible = false;
						lnkHTML5Client   .Visible = false;
						ctlFacebookLogin .Visible = false;
						pnlForgotPassword.Visible = false;
						trUserName       .Visible = false;
						trPassword       .Visible = false;
						if ( trShowForgotPassword != null )
							trShowForgotPassword.Visible = false;
						if ( Sql.IsEmptyString(Request["error"]) )
						{
							if ( Sql.ToString(Request.Form["wa"]) == "wsignin1.0" && !Sql.IsEmptyString(Request.Form["wresult"]) )
							{
								string sError = String.Empty;
								string sToken = Sql.ToString(Request.Form["wresult"]);
								Guid gUSER_ID = ActiveDirectory.AzureValidate(Application, sToken, ref sError);
								if ( !Sql.IsEmptyGuid(gUSER_ID) )
								{
									SplendidInit.LoginUser(gUSER_ID, "Azure AD");
									LoginRedirect();
								}
								else
								{
									trError.Visible = true;
									lblError.Text = L10n.Term("Users.ERR_INVALID_USER") + "<br />" + sError;
								}
							}
							// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout.  Signout is in the URL, not the form. 
							else if (  Sql.ToString(Request["wa"]) == "wsignoutcleanup1.0" )
							{
								// 12/25/2018 Paul.  After logout, leave the login screen with the login button. 
							}
							else
							{
								string sRequestURL = ActiveDirectory.AzureLogin(Context);
								Response.Redirect(sRequestURL);
							}
						}
						else
						{
							trError.Visible = true;
							lblError.Text = Sql.ToString(Request["error_description"]);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				trError.Visible = true;
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
			ctlFacebookLogin.Command += new CommandEventHandler(Page_Command);
			sTheme = Sql.ToString(Application["CONFIG.default_theme"]);
		}
		#endregion
	}
}

