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

namespace SplendidCRM.Administration.Marketo
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		public const string MODULE_NAME = "Marketo";

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
				
				string sOAUTH_ENDPOINT_URL   = String.Empty;
				string sOAUTH_IDENTITY_URL   = String.Empty;
				string sOAUTH_CLIENT_ID      = String.Empty;
				string sOAUTH_CLIENT_SECRET  = String.Empty;
				string sOAUTH_ACCESS_TOKEN   = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "Marketo.EndpointURL"     :  sOAUTH_ENDPOINT_URL  = Sql.ToString (dict[sKey]);  break;
						case "Marketo.IdentityURL"     :  sOAUTH_IDENTITY_URL  = Sql.ToString (dict[sKey]);  break;
						case "Marketo.ClientID"        :  sOAUTH_CLIENT_ID     = Sql.ToString (dict[sKey]);  break;
						case "Marketo.ClientSecret"    :  sOAUTH_CLIENT_SECRET = Sql.ToString (dict[sKey]);  break;
						case "Marketo.OAuthAccessToken":  sOAUTH_ACCESS_TOKEN  = Sql.ToString (dict[sKey]);  break;
					}
				}
				// 03/10/2021 Paul.  Sensitive fields will not be sent to React client, so check for empty string. 
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_ID) || sOAUTH_CLIENT_ID == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_ID = Sql.ToString(Application["CONFIG.Marketo.ClientID"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_SECRET) || sOAUTH_CLIENT_SECRET == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_SECRET = Sql.ToString(Application["CONFIG.Marketo.ClientSecret"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_ACCESS_TOKEN) || sOAUTH_ACCESS_TOKEN == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_ACCESS_TOKEN = Sql.ToString(Application["CONFIG.Marketo.OAuthAccessToken"]);
				}
				Spring.Social.Marketo.MarketoSync.ValidateMarketo(Application, sOAUTH_CLIENT_ID, sOAUTH_CLIENT_SECRET, sOAUTH_ENDPOINT_URL, sOAUTH_IDENTITY_URL, sOAUTH_ACCESS_TOKEN, sbErrors);
				if ( sbErrors.Length == 0 )
				{
					sbErrors.Append(L10n.Term("Marketo.LBL_TEST_SUCCESSFUL"));
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
				Spring.Social.Marketo.MarketoSync.Sync(Context);
#else
				System.Threading.Thread t = new System.Threading.Thread(Spring.Social.Marketo.MarketoSync.Sync);
				t.Start(Context);
				sbErrors.Append(L10n.Term("Marketo.LBL_SYNC_BACKGROUND"));
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
				Spring.Social.Marketo.MarketoSync.SyncAll(Context);
#else
				System.Threading.Thread t = new System.Threading.Thread(Spring.Social.Marketo.MarketoSync.SyncAll);
				t.Start(Context);
				sbErrors.Append(L10n.Term("Marketo.LBL_SYNC_BACKGROUND"));
#endif
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				sbErrors.Append(ex.Message);
			}
			return sbErrors.ToString();
		}

		public class MarketoToken
		{
			public string access_token;
			public string expires_in  ;
			public string scope       ;
		}

		[OperationContract]
		public Stream Authorize(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			StringBuilder sbErrors = new StringBuilder();
			JavaScriptSerializer json = new JavaScriptSerializer();
			json.MaxJsonLength = int.MaxValue;
			MarketoToken token = new MarketoToken();
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
				Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
				
				string sOAUTH_ENDPOINT_URL   = String.Empty;
				string sOAUTH_IDENTITY_URL   = String.Empty;
				string sOAUTH_CLIENT_ID      = String.Empty;
				string sOAUTH_CLIENT_SECRET  = String.Empty;
				string sOAUTH_ACCESS_TOKEN   = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "Marketo.EndpointURL"     :  sOAUTH_ENDPOINT_URL  = Sql.ToString (dict[sKey]);  break;
						case "Marketo.IdentityURL"     :  sOAUTH_IDENTITY_URL  = Sql.ToString (dict[sKey]);  break;
						case "Marketo.ClientID"        :  sOAUTH_CLIENT_ID     = Sql.ToString (dict[sKey]);  break;
						case "Marketo.ClientSecret"    :  sOAUTH_CLIENT_SECRET = Sql.ToString (dict[sKey]);  break;
						case "Marketo.OAuthAccessToken":  sOAUTH_ACCESS_TOKEN  = Sql.ToString (dict[sKey]);  break;
					}
				}
				// 03/10/2021 Paul.  Sensitive fields will not be sent to React client, so check for empty string. 
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_ID) || sOAUTH_CLIENT_ID == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_ID = Sql.ToString(Application["CONFIG.Marketo.ClientID"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_SECRET) || sOAUTH_CLIENT_SECRET == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_SECRET = Sql.ToString(Application["CONFIG.Marketo.ClientSecret"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_ACCESS_TOKEN) || sOAUTH_ACCESS_TOKEN == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_ACCESS_TOKEN = Sql.ToString(Application["CONFIG.Marketo.OAuthAccessToken"]);
				}
				string sACCESS_TOKEN = String.Empty;
				string sEXPIRES_AT   = String.Empty;
				string sSCOPE        = String.Empty;
				Spring.Social.Marketo.MarketoSync.AuthorizeMarketo(Application, sOAUTH_CLIENT_ID, sOAUTH_CLIENT_SECRET, sOAUTH_ENDPOINT_URL, sOAUTH_IDENTITY_URL, ref sACCESS_TOKEN, ref sEXPIRES_AT, ref sSCOPE, sbErrors);
				token.access_token = sACCESS_TOKEN;
				token.expires_in   = sEXPIRES_AT  ;
				token.scope        = sSCOPE       ;
				//sbErrors.Append(L10n.Term("Marketo.LBL_AUTHORIZE_SUCCESSFUL"));
			}
			catch(Exception ex)
			{
				// 03/20/2019 Paul.  Catch and log all failures, including insufficient access. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw( new Exception(ex.Message));
			}
			Dictionary<string, object> d = new Dictionary<string, object>();
			d.Add("d", token);
			string sResponse = json.Serialize(d);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}
	}
}
