/*
 * Copyright (C) 2019-2020 SplendidCRM Software, Inc. All Rights Reserved. 
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
 */
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SplendidCRM.Administration.HubSpot
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		public const string MODULE_NAME = "HubSpot";

		[OperationContract]
		public string Test(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			StringBuilder sbErrors = new StringBuilder();
			try
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				// 03/09/2019 Paul.  Allow admin delegate to access admin api. 
				if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SplendidSession.CreateSession(HttpContext.Current.Session);
				
				string sRequest = String.Empty;
				using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
				{
					sRequest = stmRequest.ReadToEnd();
				}
				JavaScriptSerializer json = new JavaScriptSerializer();
				json.MaxJsonLength = int.MaxValue;
				Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
				
				string sOAUTH_PORTAL_ID      = String.Empty;
				string sOAUTH_CLIENT_ID      = String.Empty;
				string sOAUTH_CLIENT_SECRET  = String.Empty;
				string sOAUTH_ACCESS_TOKEN   = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "HubSpot.PortalID"        :  sOAUTH_PORTAL_ID     = Sql.ToString (dict[sKey]);  break;
						case "HubSpot.ClientID"        :  sOAUTH_CLIENT_ID     = Sql.ToString (dict[sKey]);  break;
						case "HubSpot.ClientSecret"    :  sOAUTH_CLIENT_SECRET = Sql.ToString (dict[sKey]);  break;
						case "HubSpot.OAuthAccessToken":  sOAUTH_ACCESS_TOKEN  = Sql.ToString (dict[sKey]);  break;
					}
				}
				// 03/10/2021 Paul.  Sensitive fields will not be sent to React client, so check for empty string. 
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_ID) || sOAUTH_CLIENT_ID == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_ID = Sql.ToString(Application["CONFIG.HubSpot.ClientID"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_SECRET) || sOAUTH_CLIENT_SECRET == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_SECRET = Sql.ToString(Application["CONFIG.HubSpot.ClientSecret"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_ACCESS_TOKEN) || sOAUTH_ACCESS_TOKEN == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_ACCESS_TOKEN = Sql.ToString(Application["CONFIG.HubSpot.OAuthAccessToken"]);
				}
				Spring.Social.HubSpot.HubSpotSync.ValidateHubSpot(Application, sOAUTH_PORTAL_ID, sOAUTH_CLIENT_ID, sOAUTH_CLIENT_SECRET, sOAUTH_ACCESS_TOKEN, sbErrors);
				if ( sbErrors.Length == 0 )
				{
					sbErrors.Append(L10n.Term("HubSpot.LBL_TEST_SUCCESSFUL"));
				}
			}
			catch(Exception ex)
			{
				// 03/20/2019 Paul.  Catch and log all failures, including insufficient access. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				sbErrors.Append(ex.Message);
			}
			return sbErrors.ToString();
		}

		[OperationContract]
		public string Sync(Stream input)
		{
			HttpContext          Context     = HttpContext.Current            ;
			HttpApplicationState Application = HttpContext.Current.Application;
			StringBuilder sbErrors = new StringBuilder();
			try
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SplendidSession.CreateSession(HttpContext.Current.Session);
				
				string sRequest = String.Empty;
				using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
				{
					sRequest = stmRequest.ReadToEnd();
				}
				JavaScriptSerializer json = new JavaScriptSerializer();
				json.MaxJsonLength = int.MaxValue;
				Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
				
#if false
				Spring.Social.HubSpot.HubSpotSync.Sync(Context);
#else
				System.Threading.Thread t = new System.Threading.Thread(Spring.Social.HubSpot.HubSpotSync.Sync);
				t.Start(Context);
				sbErrors.Append(L10n.Term("HubSpot.LBL_SYNC_BACKGROUND"));
#endif
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				sbErrors.Append(ex.Message);
			}
			return sbErrors.ToString();
		}

		[OperationContract]
		public string SyncAll(Stream input)
		{
			HttpContext          Context     = HttpContext.Current            ;
			HttpApplicationState Application = HttpContext.Current.Application;
			StringBuilder sbErrors = new StringBuilder();
			try
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SplendidSession.CreateSession(HttpContext.Current.Session);
				
				string sRequest = String.Empty;
				using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
				{
					sRequest = stmRequest.ReadToEnd();
				}
				JavaScriptSerializer json = new JavaScriptSerializer();
				json.MaxJsonLength = int.MaxValue;
				Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
				
#if false
				Spring.Social.HubSpot.HubSpotSync.SyncAll(Context);
#else
				System.Threading.Thread t = new System.Threading.Thread(Spring.Social.HubSpot.HubSpotSync.SyncAll);
				t.Start(Context);
				sbErrors.Append(L10n.Term("HubSpot.LBL_SYNC_BACKGROUND"));
#endif
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				sbErrors.Append(ex.Message);
			}
			return sbErrors.ToString();
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Spring.Social.HubSpot.RefreshToken GetAccessToken(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			Spring.Social.HubSpot.RefreshToken token = null;
			StringBuilder sbErrors = new StringBuilder();
			try
			{
				string sRequest = String.Empty;
				using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
				{
					sRequest = stmRequest.ReadToEnd();
				}
				JavaScriptSerializer json = new JavaScriptSerializer();
				json.MaxJsonLength = int.MaxValue;
				Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
			
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				// 03/09/2019 Paul.  Allow admin delegate to access admin api. 
				if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SplendidSession.CreateSession(HttpContext.Current.Session);
				
				string sCode          = String.Empty;
				string sRedirectURL   = String.Empty;
				foreach ( string sColumnName in dict.Keys )
				{
					switch ( sColumnName )
					{
						case "code"         :  sCode         = Sql.ToString (dict[sColumnName]);  break;
						case "redirect_url" :  sRedirectURL  = Sql.ToString (dict[sColumnName]);  break;
					}
				}
				
				string sOAUTH_CLIENT_ID     = Sql.ToString(Application["CONFIG.HubSpot.ClientID"        ]);
				string sOAUTH_CLIENT_SECRET = Sql.ToString(Application["CONFIG.HubSpot.ClientSecret"    ]);
				// http://developer.HubSpot.com/documentation/HubSpot/guides/how-to-use-oauth2/
				HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create("https://login.HubSpot.com/oauth2/token");
				objRequest.Headers.Add("cache-control", "no-cache");
				objRequest.KeepAlive         = false;
				objRequest.AllowAutoRedirect = false;
				objRequest.Timeout           = 120000;  // 120 seconds
				objRequest.ContentType       = "application/x-www-form-urlencoded";
				objRequest.Method            = "POST";
				
				if ( Sql.IsEmptyString(sRedirectURL) )
					sRedirectURL = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Administration/HubSpot/OAuthLanding.aspx";
				string sData = "grant_type=authorization_code&client_id=" + sOAUTH_CLIENT_ID + "&client_secret=" + sOAUTH_CLIENT_SECRET + "&code=" + sCode + "&redirect_uri=" + HttpUtility.UrlEncode(sRedirectURL);
				objRequest.ContentLength = sData.Length;
				using ( StreamWriter stm = new StreamWriter(objRequest.GetRequestStream(), System.Text.Encoding.ASCII) )
				{
					stm.Write(sData);
				}
				
				string sResponse = String.Empty;
				using ( HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse() )
				{
					if ( objResponse != null )
					{
						if ( objResponse.StatusCode != HttpStatusCode.OK && objResponse.StatusCode != HttpStatusCode.Found )
						{
							throw(new Exception(objResponse.StatusCode + " " + objResponse.StatusDescription));
						}
						else
						{
							using ( StreamReader stm = new StreamReader(objResponse.GetResponseStream()) )
							{
								sResponse = stm.ReadToEnd();
								token = json.Deserialize<Spring.Social.HubSpot.RefreshToken>(sResponse);
								DateTime dtOAuthExpiresAt = DateTime.Now.AddHours(6);
								token.expires_at = dtOAuthExpiresAt.ToShortDateString() + " " + dtOAuthExpiresAt.ToShortTimeString();
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				// 03/20/2019 Paul.  Catch and log all failures, including insufficient access. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw(new Exception(ex.Message));
			}
			return token;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Spring.Social.HubSpot.RefreshToken RefreshToken()
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			Spring.Social.HubSpot.RefreshToken token = null;
			StringBuilder sbErrors = new StringBuilder();
			try
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SplendidSession.CreateSession(HttpContext.Current.Session);
			
				Spring.Social.HubSpot.HubSpotSync.RefreshAccessToken(Application, sbErrors);
				if ( sbErrors.Length == 0 )
				{
					token = new Spring.Social.HubSpot.RefreshToken();
					token.access_token  = Sql.ToString (Application["CONFIG.HubSpot.OAuthAccessToken" ]);
					token.refresh_token = Sql.ToString (Application["CONFIG.HubSpot.OAuthRefreshToken"]);
					token.expires_at    = Sql.ToString (Application["CONFIG.HubSpot.OAuthExpiresAt"   ]);
				}
				else
				{
					throw(new Exception(sbErrors.ToString()));
				}
			}
			catch(Exception ex)
			{
				// 03/20/2019 Paul.  Catch and log all failures, including insufficient access. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw(new Exception(ex.Message));
			}
			return token;
		}
	}
}
