/*
 * Copyright (C) 2019-2021 SplendidCRM Software, Inc. All Rights Reserved. 
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
using System.Web.Caching;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SplendidCRM.Administration.PayTrace
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		public const string MODULE_NAME = "PayTrace";

		[OperationContract]
		public string Test(Stream input)
		{
			HttpContext          Context     = HttpContext.Current;
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			Cache                Cache       = HttpRuntime.Cache;
			
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
				
				bool   bENABLED    = false;
				string sUSER_NAME  = String.Empty;
				string sPASSWORD   = String.Empty;
				foreach ( string sKey in dict.Keys )
				{
					switch ( sKey )
					{
						case "PayTrace.Enabled" :  bENABLED   = Sql.ToBoolean(dict[sKey]);  break;
						case "PayTrace.UserName":  sUSER_NAME = Sql.ToString (dict[sKey]);  break;
						case "PayTrace.Password":  sPASSWORD  = Sql.ToString (dict[sKey]);  break;
					}
				}
				// 02/23/2021 Paul.  The React client will not get the user name or the transaction key, so if blank, then not changed. 
				if ( Sql.IsEmptyString(sUSER_NAME) || sUSER_NAME == Sql.sEMPTY_PASSWORD )
				{
					sUSER_NAME = Sql.ToString (Application["CONFIG.PayTrace.UserName"]);
					sPASSWORD  = Sql.ToString (Application["CONFIG.PayTrace.Password"]);
				}
				// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
				// 03/10/2021 Paul.  Sensitive fields will not be sent to React client, so check for empty string. 
				if ( Sql.IsEmptyString(sPASSWORD) || sPASSWORD == Sql.sEMPTY_PASSWORD )
				{
					sPASSWORD = Sql.ToString (Application["CONFIG.PayTrace.TransactionKey"]);
				}
				else
				{
					Guid gCREDIT_CARD_KEY = Sql.ToGuid(Application["CONFIG.CreditCardKey"]);
					Guid gCREDIT_CARD_IV  = Sql.ToGuid(Application["CONFIG.CreditCardIV" ]);
					string sENCRYPTED_PASSWORD = Security.EncryptPassword(sPASSWORD, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
					if ( Security.DecryptPassword(sENCRYPTED_PASSWORD, gCREDIT_CARD_KEY, gCREDIT_CARD_IV) != sPASSWORD )
						throw(new Exception("Decryption failed"));
					sPASSWORD = sENCRYPTED_PASSWORD;
				}
				string sResult = PayTraceUtils.ValidateLogin(Application, sUSER_NAME, sPASSWORD);
				if ( Sql.IsEmptyString(sResult) )
				{
					sbErrors.Append(L10n.Term("PayTrace.LBL_CONNECTION_SUCCESSFUL"));
				}
				else
				{
					sbErrors.Append(sResult);
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
		public Stream Transactions(Stream input)
		{
			HttpContext          Context     = HttpContext.Current;
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			Cache                Cache       = HttpRuntime.Cache;
			
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

			Guid      gTIMEZONE   = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone  T10n        = TimeZone.CreateTimeZone(gTIMEZONE);
			DataTable dtPaginated = new DataTable();
			long      lTotalCount = 0;
			try
			{
				int    nSKIP     = Sql.ToInteger(Request["$skip"    ]);
				int    nTOP      = Sql.ToInteger(Request["$top"     ]);
				string sFILTER   = Sql.ToString (Request["$filter"  ]);
				string sORDER_BY = Sql.ToString (Request["$orderby" ]);
				// 06/17/2013 Paul.  Add support for GROUP BY. 
				string sGROUP_BY = Sql.ToString (Request["$groupby" ]);
				// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
				string sSELECT   = Sql.ToString (Request["$select"  ]);
				
				DateTime dtSTART_DATE      = DateTime.MinValue;
				DateTime dtEND_DATE        = DateTime.MinValue;
				string   sTRANSACTION_TYPE = String.Empty;
				string   sSEARCH_TEXT      = String.Empty;
				Dictionary<string, object> dictSearchValues = null;
				try
				{
					foreach ( string sName in dict.Keys )
					{
						switch ( sName )
						{
							case "$skip"           :  nSKIP            = Sql.ToInteger(dict[sName]);  break;
							case "$top"            :  nTOP             = Sql.ToInteger(dict[sName]);  break;
							case "$filter"         :  sFILTER          = Sql.ToString (dict[sName]);  break;
							case "$orderby"        :  sORDER_BY        = Sql.ToString (dict[sName]);  break;
							case "$groupby"        :  sGROUP_BY        = Sql.ToString (dict[sName]);  break;
							case "$select"         :  sSELECT          = Sql.ToString (dict[sName]);  break;
							case "$searchvalues"   :  dictSearchValues = dict[sName] as Dictionary<string, object>;  break;
						}
					}
					if ( dictSearchValues != null )
					{
						if ( dictSearchValues.ContainsKey("START_DATE") )
						{
							Dictionary<string, object> dictValue = dictSearchValues["START_DATE"] as Dictionary<string, object>;
							if ( dictValue.ContainsKey("value") )
								dtSTART_DATE = T10n.ToServerTime(RestUtil.FromJsonDate(Sql.ToString(dictValue["value"])));
						}
						if ( dictSearchValues.ContainsKey("END_DATE") )
						{
							Dictionary<string, object> dictValue = dictSearchValues["END_DATE"] as Dictionary<string, object>;
							if ( dictValue.ContainsKey("value") )
								dtEND_DATE = T10n.ToServerTime(RestUtil.FromJsonDate(Sql.ToString(dictValue["value"])));
						}
						if ( dictSearchValues.ContainsKey("TRANSACTION_TYPE") )
						{
							Dictionary<string, object> dictValue = dictSearchValues["TRANSACTION_TYPE"] as Dictionary<string, object>;
							if ( dictValue.ContainsKey("value") )
								sTRANSACTION_TYPE = Sql.ToString(dictValue["value"]);
						}
						if ( dictSearchValues.ContainsKey("SEARCH_TEXT") )
						{
							Dictionary<string, object> dictValue = dictSearchValues["SEARCH_TEXT"] as Dictionary<string, object>;
							if ( dictValue.ContainsKey("value") )
								sSEARCH_TEXT = Sql.ToString(dictValue["value"]);
						}
					}
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex.Message);
					throw;
				}
				
				// 02/26/2021 Paul.  Include the date in the cache key so that we can speed pagination but still have live data. 
				string sCacheKey = dtSTART_DATE.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + dtEND_DATE.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
				DataSet ds = Cache.Get(MODULE_NAME + ".Transactions_" + sCacheKey) as DataSet;
				if ( ds == null )
				{
					if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) )
					{
						ds = PayTraceUtils.Transactions(Application, dtSTART_DATE, dtEND_DATE, sTRANSACTION_TYPE, sSEARCH_TEXT, String.Empty);
						DataTable dtTransactions = ds.Tables["TRANSACTIONS"];
						dtTransactions.Columns.Add("NAME");
						foreach ( DataRow row in dtTransactions.Rows )
						{
							row["NAME"] = row["TRANXID"];
						}
						Cache.Insert(MODULE_NAME + ".Transactions_" + sCacheKey, ds, null, PayPalCache.DefaultCacheExpiration(), System.Web.Caching.Cache.NoSlidingExpiration);
					}
					else
					{
						throw(new Exception(MODULE_NAME + " is not enabled or configured."));
					}
				}
				DataTable dt = ds.Tables["TRANSACTIONS"];
				DataView vw = new DataView(dt);
				if ( Sql.IsEmptyString(sORDER_BY) )
					sORDER_BY = "TRANXID desc";
				vw.Sort = sORDER_BY;

				dtPaginated = dt.Clone();
				for ( int i = nSKIP; i < vw.Count; i++ )
				{
					DataRow row = dtPaginated.NewRow();
					foreach ( DataColumn col in dtPaginated.Columns )
					{
						row[col.ColumnName] = vw[i].Row[col.ColumnName];
					}
					dtPaginated.Rows.Add(row);
					if ( dtPaginated.Rows.Count >= nTOP )
						break;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath;
			// 04/01/2020 Paul.  Move json utils to RestUtil. 
			Dictionary<string, object> dictResponse = RestUtil.ToJson(sBaseURI, String.Empty, dtPaginated, T10n);
			dictResponse.Add("__total", lTotalCount);
			string sResponse = json.Serialize(dictResponse);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetTransaction(string ID)
		{
			HttpContext          Context     = HttpContext.Current;
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			Cache                Cache       = HttpRuntime.Cache;
			
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			DataSet   ds        = null;
			DataTable dt        = null;
			Guid      gTIMEZONE = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone  T10n      = TimeZone.CreateTimeZone(gTIMEZONE);
			string    sTransactionID = ID;
			if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) )
			{
				ds = PayTraceUtils.Transaction(Application, sTransactionID);
				dt = ds.Tables["TRANSACTIONS"];
			}
			else
			{
				throw(new Exception(MODULE_NAME + " is not enabled or configured."));
			}
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath;
			JavaScriptSerializer json = new JavaScriptSerializer();
			json.MaxJsonLength = int.MaxValue;
			
			Dictionary<string, object> dict = RestUtil.ToJson(sBaseURI, MODULE_NAME, dt.Rows[0], T10n);
			string sResponse = json.Serialize(dict);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		public string Refund(Stream input)
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

				string sTransactionID = (dict.ContainsKey("TransactionID") ? Sql.ToString(dict["TransactionID"]) : String.Empty);
				if ( Sql.IsEmptyString(sTransactionID ) )
				{
					throw(new Exception("Missing TransactionID"));
				}
				
				if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) )
				{
					PayTraceUtils.Refund(Application, sTransactionID);
					HttpRuntime.Cache.Remove("PayTrace.Transaction." + sTransactionID);
				}
				else
				{
					throw(new Exception(MODULE_NAME + " is not enabled or configured."));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				sbErrors.Append(ex.Message);
			}
			return sbErrors.ToString();
		}

	}
}
