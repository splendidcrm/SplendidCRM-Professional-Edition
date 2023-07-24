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

namespace SplendidCRM.Administration.iContact
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		public const string MODULE_NAME = "iContact";

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
				
				string sAPI_APP_ID                = String.Empty;
				string sAPI_USERNAME              = String.Empty;
				string sAPI_PASSWORD              = String.Empty;
				string sICONTACT_ACCOUNT_ID       = String.Empty;
				string sICONTACT_CLIENT_FOLDER_ID = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "iContact.ApiAppId"              :  sAPI_APP_ID                = Sql.ToString (dict[sKey]);  break;
						case "iContact.ApiUsername"           :  sAPI_USERNAME              = Sql.ToString (dict[sKey]);  break;
						case "iContact.ApiPassword"           :  sAPI_PASSWORD              = Sql.ToString (dict[sKey]);  break;
						case "iContact.iContactAccountId"     :  sICONTACT_ACCOUNT_ID       = Sql.ToString (dict[sKey]);  break;
						case "iContact.iContactClientFolderId":  sICONTACT_CLIENT_FOLDER_ID = Sql.ToString (dict[sKey]);  break;
					}
				}
				// 03/10/2021 Paul.  Sensitive fields will not be sent to React client, so check for empty string. 
				if ( Sql.IsEmptyString(sAPI_APP_ID) || sAPI_APP_ID == Sql.sEMPTY_PASSWORD )
				{
					sAPI_APP_ID = Sql.ToString(Application["CONFIG.iContact.ApiAppId"]);
				}
				if ( Sql.IsEmptyString(sAPI_USERNAME) || sAPI_USERNAME == Sql.sEMPTY_PASSWORD )
				{
					sAPI_USERNAME = Sql.ToString(Application["CONFIG.iContact.ApiUsername"]);
				}
				if ( Sql.IsEmptyString(sAPI_PASSWORD) || sAPI_PASSWORD == Sql.sEMPTY_PASSWORD )
				{
					sAPI_PASSWORD = Sql.ToString(Application["CONFIG.iContact.ApiPassword"]);
				}
				Spring.Social.iContact.iContactSync.ValidateiContact(Application, sAPI_APP_ID, sAPI_USERNAME, sAPI_PASSWORD, sICONTACT_ACCOUNT_ID, sICONTACT_CLIENT_FOLDER_ID, sbErrors);
				if ( sbErrors.Length == 0 )
				{
					sbErrors.Append(L10n.Term("iContact.LBL_TEST_SUCCESSFUL"));
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
				Spring.Social.iContact.iContactSync.Sync(Context);
#else
				System.Threading.Thread t = new System.Threading.Thread(Spring.Social.iContact.iContactSync.Sync);
				t.Start(Context);
				sbErrors.Append(L10n.Term("iContact.LBL_SYNC_BACKGROUND"));
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
				Spring.Social.iContact.iContactSync.SyncAll(Context);
#else
				System.Threading.Thread t = new System.Threading.Thread(Spring.Social.iContact.iContactSync.SyncAll);
				t.Start(Context);
				sbErrors.Append(L10n.Term("iContact.LBL_SYNC_BACKGROUND"));
#endif
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				sbErrors.Append(ex.Message);
			}
			return sbErrors.ToString();
		}

		public class iContactAccount
		{
			public string AccountID     ;
			public string ClientFolderID;
		}

		[OperationContract]
		public iContactAccount GetAccount(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			
			iContactAccount account = null;
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
				
				string sAPI_APP_ID   = String.Empty;
				string sAPI_USERNAME = String.Empty;
				string sAPI_PASSWORD = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "iContact.ApiAppId"   :  sAPI_APP_ID   = Sql.ToString (dict[sKey]);  break;
						case "iContact.ApiUsername":  sAPI_USERNAME = Sql.ToString (dict[sKey]);  break;
						case "iContact.ApiPassword":  sAPI_PASSWORD = Sql.ToString (dict[sKey]);  break;
					}
				}
				// 03/10/2021 Paul.  Sensitive fields will not be sent to React client, so check for empty string. 
				if ( Sql.IsEmptyString(sAPI_APP_ID) || sAPI_APP_ID == Sql.sEMPTY_PASSWORD )
				{
					sAPI_APP_ID = Sql.ToString(Application["CONFIG.iContact.ApiAppId"]);
				}
				if ( Sql.IsEmptyString(sAPI_USERNAME) || sAPI_USERNAME == Sql.sEMPTY_PASSWORD )
				{
					sAPI_USERNAME = Sql.ToString(Application["CONFIG.iContact.ApiUsername"]);
				}
				if ( Sql.IsEmptyString(sAPI_PASSWORD) || sAPI_PASSWORD == Sql.sEMPTY_PASSWORD )
				{
					sAPI_PASSWORD = Sql.ToString(Application["CONFIG.iContact.ApiPassword"]);
				}
				string sAccountID      = String.Empty;
				string sClientFolderID = String.Empty;
				Spring.Social.iContact.iContactSync.GetAccount(Application, sAPI_APP_ID, sAPI_USERNAME, sAPI_PASSWORD, ref sAccountID, ref sClientFolderID, sbErrors);
				if ( sbErrors.Length == 0 )
				{
					account = new iContactAccount();
					account.AccountID      = sAccountID     ;
					account.ClientFolderID = sClientFolderID;
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
			return account;
		}

	}
}
