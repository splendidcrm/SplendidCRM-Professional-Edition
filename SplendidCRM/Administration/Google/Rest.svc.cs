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

using Google.Apis.Auth.OAuth2.Responses;

namespace SplendidCRM.Administration.GoogleConfig
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		public const string MODULE_NAME = "Google";

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
				/*
				bool   bENABLED               = false;
				bool   bVERBOSE_STATUS        = false;
				string sAPI_KEY               = String.Empty;
				string sOAUTH_CLIENT_ID       = String.Empty;
				string sOAUTH_CLIENT_SECRET   = String.Empty;
				bool   bPUSH_NOTIFICATIONS    = false;
				string sPUSH_NOTIFICATION_URL = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "GoogleApps.Enabled"            :  bENABLED               = Sql.ToBoolean(dict[sKey]);  break;
						case "GoogleApps.VerboseStatus"      :  bVERBOSE_STATUS        = Sql.ToBoolean(dict[sKey]);  break;
						case "GoogleApps.ApiKey"             :  sAPI_KEY               = Sql.ToString (dict[sKey]);  break;
						case "GoogleApps.ClientID"           :  sOAUTH_CLIENT_ID       = Sql.ToString (dict[sKey]);  break;
						case "GoogleApps.ClientSecret"       :  sOAUTH_CLIENT_SECRET   = Sql.ToString (dict[sKey]);  break;
						case "GoogleApps.PushNotifications"  :  bPUSH_NOTIFICATIONS    = Sql.ToBoolean(dict[sKey]);  break;
						case "GoogleApps.PushNotificationURL":  sPUSH_NOTIFICATION_URL = Sql.ToString (dict[sKey]);  break;
					}
				}
				if ( Sql.IsEmptyString(sAPI_KEY) || sAPI_KEY == Sql.sEMPTY_PASSWORD )
				{
					sAPI_KEY = Sql.ToString(Application["CONFIG.GoogleApps.ApiKey"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_ID) || sOAUTH_CLIENT_ID == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_ID = Sql.ToString(Application["CONFIG.GoogleApps.ClientID"]);
				}
				if ( Sql.IsEmptyString(sOAUTH_CLIENT_SECRET) || sOAUTH_CLIENT_SECRET == Sql.sEMPTY_PASSWORD )
				{
					sOAUTH_CLIENT_SECRET = Sql.ToString(Application["CONFIG.GoogleApps.ClientSecret"]);
				}
				*/
				bool bValid = GoogleApps.TestAccessToken(Application, Security.USER_ID, sbErrors);
				if ( bValid )
					sbErrors.Append(L10n.Term("Google.LBL_TEST_SUCCESSFUL"));
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
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public TokenResponse RefreshToken()
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			TokenResponse token = null;
			StringBuilder sbErrors = new StringBuilder();
			try
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SplendidSession.CreateSession(HttpContext.Current.Session);
			
				token = GoogleApps.RefreshAccessToken(Application, Security.USER_ID, true);
				sbErrors.Append(L10n.Term("Google.LBL_TEST_SUCCESSFUL"));
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
