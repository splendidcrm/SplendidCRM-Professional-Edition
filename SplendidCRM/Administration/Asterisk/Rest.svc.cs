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
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SplendidCRM.Administration.Asterisk
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		public const string MODULE_NAME = "Asterisk";

		[OperationContract]
		public string Test(Stream input)
		{
			StringBuilder sbErrors = new StringBuilder();
			HttpApplicationState Application = HttpContext.Current.Application;
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
				
				string sHOST_SERVER   = String.Empty;
				int    nHOST_PORT     = 0;
				string sUSER_NAME     = String.Empty;
				string sPASSWORD      = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "Asterisk.Host"        :  sHOST_SERVER   = Sql.ToString (dict[sKey]);  break;
						case "Asterisk.Port"        :  nHOST_PORT     = Sql.ToInteger(dict[sKey]);  break;
						case "Asterisk.UserName"    :  sUSER_NAME     = Sql.ToString (dict[sKey]);  break;
						case "Asterisk.Password"    :  sPASSWORD      = Sql.ToString (dict[sKey]);  break;
					}
				}
				// 03/10/2021 Paul.  Sensitive fields will not be sent to React client, so check for empty string. 
				if ( Sql.IsEmptyString(sPASSWORD) || sPASSWORD == Sql.sEMPTY_PASSWORD )
				{
					sPASSWORD = Sql.ToString(Application["CONFIG.Avaya.Password"]);
				}
				else
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					sPASSWORD = Security.EncryptPassword(sPASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
				}
				if ( AsteriskManager.Instance != null )
				{
					string sResult = AsteriskManager.Instance.ValidateLogin(sHOST_SERVER, nHOST_PORT, sUSER_NAME, sPASSWORD);
					if ( Sql.IsEmptyString(sResult) )
						sbErrors.Append(L10n.Term("Asterisk.LBL_CONNECTION_SUCCESSFUL"));
					else
						sbErrors.Append(String.Format(L10n.Term("Asterisk.ERR_FAILED_TO_CONNECT"), sResult));
				}
				else if ( Sql.ToBoolean(Application["CONFIG.SignalR.Disabled"]) )
				{
					sbErrors.Append("SignalR is Disabled");
				}
				else
				{
					sbErrors.Append("AsteriskManager.Instance is null");
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

	}
}
