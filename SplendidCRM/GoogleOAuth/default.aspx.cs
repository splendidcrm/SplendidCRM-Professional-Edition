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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Auth.OAuth2.Flows;
using System.Threading;

namespace SplendidCRM.GoogleOAuth
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : SplendidPopup
	{
		protected Label  lblError         ;
		protected string sAccessToken     ;
		protected string sTokenType       ;
		protected string sRefreshToken    ;
		protected string sExpiresInSeconds;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			try
			{
				string sCode = Sql.ToString(Request["code"]);
				if ( !Sql.IsEmptyString(sCode) )
				{
					string[] arrScopes = new string[]
					{
						"https://www.googleapis.com/auth/calendar",
						"https://www.googleapis.com/auth/tasks",
						"https://mail.google.com/",
						"https://www.google.com/m8/feeds"
					};
					string sOAuthClientID     = Sql.ToString(Application["CONFIG.GoogleApps.ClientID"    ]);
					string sOAuthClientSecret = Sql.ToString(Application["CONFIG.GoogleApps.ClientSecret"]);
					GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
					{
						//DataStore = new SessionDataStore(Session),
						ClientSecrets = new ClientSecrets
						{
							ClientId     = sOAuthClientID,
							ClientSecret = sOAuthClientSecret
						},
						Scopes = arrScopes
					});
					string sUserID = Security.USER_ID.ToString();
					// 09/25/2015 Paul.  Redirect URL must match those allowed in Google Developer Console. https://console.developers.google.com/project/_/apiui/credential
					string sRedirectURL = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "GoogleOAuth/";
					Google.Apis.Auth.OAuth2.Responses.TokenResponse token = flow.ExchangeCodeForTokenAsync(sUserID, sCode, sRedirectURL, CancellationToken.None).Result;
					// 02/03/2017 Paul.  IE11 is getting stuck due to Protected Mode for Security / Internet service. window.opener === undefined after return from Google URL. 
					sAccessToken      = token.AccessToken           ;
					sTokenType        = token.TokenType             ;
					sRefreshToken     = token.RefreshToken          ;
					sExpiresInSeconds = token.ExpiresInSeconds.Value.ToString();
					//Page.ClientScript.RegisterStartupScript(System.Type.GetType("System.String"), "TokenUpdate", "if ( window.opener !== undefined && window.opener != null ) { window.opener.OAuthTokenUpdate('" + token.AccessToken + "', '" + token.TokenType + "', null, '" + token.RefreshToken + "', '" + token.ExpiresInSeconds.Value + "'); window.close(); }", true);
				}
			}
			catch(Exception ex)
			{
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
