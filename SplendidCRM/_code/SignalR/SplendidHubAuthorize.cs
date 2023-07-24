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
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;

namespace SplendidCRM
{
	public class SplendidSession
	{
		private static int nSessionTimeout = 20;
		// 11/16/2014 Paul.  Using a local session variable means that this system will not work on a web farm unless sticky sessions are used. 
		// The alternative is to use the Claims approach of OWIN, but that system seems to be CPU intensive with all the encrypting and decrypting of the claim data. 
		// The claim data is just an encrypted package of non-sensitive user information, such as User ID, User Name and Email. 
		// The claim data is effectively session data that is encrypted and stored in a cookie. 
		private static Dictionary<string, SplendidSession> dictSessions;

		public DateTime Expiration;
		public Guid     USER_ID   ;
		public string   USER_NAME ;

		public static void CreateSession(HttpSessionState Session)
		{
			if ( Session != null )
			{
				if ( dictSessions == null )
				{
					dictSessions = new Dictionary<string, SplendidSession>();
					nSessionTimeout = Session.Timeout;
				}
				Guid gUSER_ID = Sql.ToGuid(Session["USER_ID"]);
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
					SplendidSession ss = new SplendidSession();
					ss.Expiration = DateTime.Now.AddMinutes(nSessionTimeout);
					ss.USER_ID   = gUSER_ID;
					ss.USER_NAME = Sql.ToString(Session["USER_NAME"]);
					dictSessions[Session.SessionID] = ss;
				}
				else
				{
					if ( dictSessions.ContainsKey(Session.SessionID) )
						dictSessions.Remove(Session.SessionID);
				}
			}
		}

		public static SplendidSession GetSession(string sSessionID)
		{
			SplendidSession ss = null;
			if ( dictSessions.ContainsKey(sSessionID) )
			{
				ss = dictSessions[sSessionID];
				if ( ss.Expiration < DateTime.Now )
				{
					dictSessions.Remove(sSessionID);
					ss = null;
				}
			}
			return ss;
		}

		public static void PurgeOldSessions(HttpContext Context)
		{
			try
			{
				if ( dictSessions != null )
				{
					DateTime dtNow = DateTime.Now;
					// 11/16/2014 Paul.  We cannot use foreach to remove items from a dictionary, so use a separate list. 
					List<string> arrSessions = new List<string>();
					foreach ( string sSessionID in dictSessions.Keys )
						arrSessions.Add(sSessionID);
					foreach ( string sSessionID in arrSessions )
					{
						SplendidSession ss = dictSessions[sSessionID];
						if ( ss.Expiration < dtNow )
							dictSessions.Remove(sSessionID);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
			}
		}
	}

	// http://eworldproblems.mbaynton.com/2012/12/signalr-hub-authorization/
	public class SplendidHubAuthorize : Attribute, IAuthorizeHubConnection, IAuthorizeHubMethodInvocation
	{
		public virtual bool AuthorizeHubConnection(HubDescriptor hubDescriptor, Microsoft.AspNet.SignalR.IRequest request)
		{
			// 11/14/2014 Paul.  SignalR 1.0 does not have access to the ASP.NET Pipeline, so we cannot identify the user. 
			//System.Security.Principal.IIdentity usr = request.User.Identity;
			SplendidSession ss = null;
			if ( request.Cookies.ContainsKey("ASP.NET_SessionId") )
			{
				Cookie cookie = request.Cookies["ASP.NET_SessionId"];
				string sSessionID = cookie.Value;
				ss = SplendidSession.GetSession(sSessionID);
			}
			return ss != null;
		}

		public virtual bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
		{
			// 11/14/2014 Paul.  SignalR 1.0 does not have access to the ASP.NET Pipeline, so we cannot identify the user. 
			//System.Security.Principal.IIdentity usr = hubIncomingInvokerContext.Hub.Context.User.Identity;
			SplendidSession ss = null;
			if ( hubIncomingInvokerContext.Hub.Context.RequestCookies.ContainsKey("ASP.NET_SessionId") )
			{
				Cookie cookie = hubIncomingInvokerContext.Hub.Context.RequestCookies["ASP.NET_SessionId"];
				string sSessionID = cookie.Value;
				ss = SplendidSession.GetSession(sSessionID);
			}
			return ss != null;
		}
	}
}

