/*
 * Copyright (C) 2013-2021 SplendidCRM Software, Inc. All Rights Reserved. 
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
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Threading;
using System.Diagnostics;

namespace SplendidCRM.Administration.ReplicationTables
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		[OperationContract]
		public void RunSync(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 12/12/2014 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);

			string sModuleName = "ReplicationTables";
			L10N L10n       = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			int  nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sModuleName));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			try
			{
				DbProviderFactory dbfReplication = DbReplicationFactories.GetFactory();
				if ( !Sql.IsEmptyString(Application["ReplicationConnectionString"]) )
				{
					string sTABLE_NAME = Sql.ToString(Request["TABLE_NAME"]);
#if DEBUG
					ReplicationExternalDB.RunReplication(HttpContext.Current, sTABLE_NAME);
#else
					ReplicationBackground background = new ReplicationBackground(HttpContext.Current, sTABLE_NAME);
					System.Threading.Thread t = new System.Threading.Thread(background.Start);
					t.Start();
#endif
				}
				else
				{
					throw(new Exception(L10n.Term("ReplicationTables.ERR_NOT_CONFIGURED")));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		[OperationContract]
		public void BulkRunSync(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 12/12/2014 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
			ArrayList arrTABLE_NAME = dict["NAME_LIST"] as ArrayList;

			string sModuleName = "ReplicationTables";
			L10N L10n       = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			int  nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sModuleName));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			try
			{
				DbProviderFactory dbfReplication = DbReplicationFactories.GetFactory();
				if ( !Sql.IsEmptyString(Application["ReplicationConnectionString"]) )
				{
					foreach ( string sTABLE_NAME in arrTABLE_NAME )
					{
						ReplicationBackground background = new ReplicationBackground(HttpContext.Current, sTABLE_NAME);
						System.Threading.Thread t = new System.Threading.Thread(background.Start);
						t.Start();
					}
				}
				else
				{
					throw(new Exception(L10n.Term("ReplicationTables.ERR_NOT_CONFIGURED")));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		[OperationContract]
		public void UpdateStats(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 12/12/2014 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);

			string sModuleName = "ReplicationTables";
			L10N L10n       = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			int  nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sModuleName));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			try
			{
				DbProviderFactory dbfReplication = DbReplicationFactories.GetFactory();
				if ( !Sql.IsEmptyString(Application["ReplicationConnectionString"]) )
				{
					string sTABLE_NAME = Sql.ToString(Request["TABLE_NAME"]);
					ReplicationExternalDB.UpdateStats(HttpContext.Current, sTABLE_NAME);
				}
				else
				{
					throw(new Exception(L10n.Term("ReplicationTables.ERR_NOT_CONFIGURED")));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		[OperationContract]
		public void BulkUpdateStats(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 12/12/2014 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
			ArrayList arrTABLE_NAME = dict["NAME_LIST"] as ArrayList;

			string sModuleName = "ReplicationTables";
			L10N L10n       = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			int  nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				// 09/06/2017 Paul.  Include module name in error. 
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + ": " + sModuleName));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			try
			{
				DbProviderFactory dbfReplication = DbReplicationFactories.GetFactory();
				if ( !Sql.IsEmptyString(Application["ReplicationConnectionString"]) )
				{
					StringBuilder sbErrors = new StringBuilder();
					foreach ( string sTABLE_NAME in arrTABLE_NAME )
					{
						try
						{
							ReplicationExternalDB.UpdateStats(HttpContext.Current, sTABLE_NAME);
						}
						catch(Exception ex)
						{
							sbErrors.AppendLine(ex.Message);
						}
					}
					if ( sbErrors.Length > 0 )
					{
						throw(new Exception(sbErrors.ToString()));
					}
				}
				else
				{
					throw(new Exception(L10n.Term("ReplicationTables.ERR_NOT_CONFIGURED")));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetByTableName(string TABLE_NAME)
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpContext          Context     = HttpContext.Current;
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;

			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			// 03/09/2019 Paul.  Allow admin delegate to access admin api. 
			if ( !Security.IsAuthenticated() || !(Security.IS_ADMIN || SplendidCRM.Security.IS_ADMIN_DELEGATE) )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			string ModuleName = "ReplicationTables";
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( !Sql.IsEmptyString(ModuleName) )
			{
				bool bIsAdmin = Sql.ToBoolean(Application["Modules." + ModuleName + ".IsAdmin"]);
				if ( bIsAdmin && SplendidCRM.Security.AdminUserAccess(ModuleName, "access") >= 0 )
				{
					string sFILTER = "TABLE_NAME eq '" + Sql.EscapeSQL(TABLE_NAME) + "'";
					long lTotalCount = 0;
					// 10/26/2019 Paul.  Return the SQL to the React Client. 
					StringBuilder sbDumpSQL = new StringBuilder();
					// 12/16/2019 Paul.  Moved GetTable to ~/_code/RestUtil.cs
					// 10/16/2020 Paul.  Use AccessMode.list so that we use the _List view if available. 
					DataTable dt = RestUtil.GetAdminTable(Context, sTABLE_NAME, 0, 1, sFILTER, String.Empty, String.Empty, null, null, ref lTotalCount, null, AccessMode.edit, sbDumpSQL);
					if ( dt == null || dt.Rows.Count == 0 )
						throw(new Exception("Table not found: " + ModuleName + " " + TABLE_NAME));
			
					string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath;
					JavaScriptSerializer json = new JavaScriptSerializer();
					json.MaxJsonLength = int.MaxValue;
			
					Guid     gTIMEZONE         = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
					TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
					// 04/01/2020 Paul.  Move json utils to RestUtil. 
					Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, ModuleName, dt.Rows[0], T10n);
					
					// 10/26/2019 Paul.  Return the SQL to the React Client. 
					if ( Sql.ToBoolean(Application["CONFIG.show_sql"]) )
					{
						dict.Add("__sql", sbDumpSQL.ToString());
					}
					string sResponse = json.Serialize(dict);
					byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
					return new MemoryStream(byResponse);
				}
				else
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
			}
			else
			{
				throw(new Exception("Unsupported table: " + sTABLE_NAME));
			}
		}

	}
}
