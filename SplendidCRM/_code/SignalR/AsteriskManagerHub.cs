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
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;

namespace SplendidCRM
{
	public interface IAsteriskServer
	{
		string JoinGroup(string sConnectionId, string sGroupName);
		string OriginateCall(string sUSER_EXTENSION, string sUSER_FULL_NAME, string sUSER_PHONE_WORK, string sPHONE, string sPARENT_ID, string sPARENT_TYPE);
		Guid CreateCall(string sUniqueId);
	}

	/// <summary>
	/// Summary description for AsteriskManagerHub.
	/// </summary>
	[HubName("AsteriskManagerHub")]
	public class AsteriskManagerHub : Hub<IAsteriskServer>
	{
		private readonly AsteriskManager _asteriskManager;

		public AsteriskManagerHub() : this(AsteriskManager.Instance)
		{
		}

		public AsteriskManagerHub(AsteriskManager asteriskManager)
		{
			_asteriskManager = asteriskManager;
		}

		// 11/15/2014 Paul.  Hub method should require authorization. 
		// http://eworldproblems.mbaynton.com/2012/12/signalr-hub-authorization/
		[SplendidHubAuthorize]
		public string JoinGroup(string sConnectionId, string sGroupName)
		{
			// 09/02/2013 Paul.  The the.Context.User.Identity value is not the same as HttpContext.Current.User, so we don't know who this is. 
			//if ( this.Context.User != null && this.Context.User.Identity != null )
			//	Debug.WriteLine(this.Context.User.Identity.Name);
			if ( !Sql.IsEmptyString(sGroupName) )
			{
				Groups.Add(sConnectionId, sGroupName).Wait();
				return sConnectionId + " joined " + sGroupName;
			}
			return "Group not specified.";
		}

		// 11/15/2014 Paul.  Hub method should require authorization. 
		// http://eworldproblems.mbaynton.com/2012/12/signalr-hub-authorization/
		[SplendidHubAuthorize]
		public string OriginateCall(string sUSER_EXTENSION, string sUSER_FULL_NAME, string sUSER_PHONE_WORK, string sPHONE, string sPARENT_ID, string sPARENT_TYPE)
		{
			// 08/28/2013 Paul.  Session is not available in SignalR. 
			/*
			if ( !Security.IsAuthenticated() )
				throw(new Exception("Authentication required"));
			
			string sUSER_EXTENSION = Sql.ToString(HttpContext.Current.Session["EXTENSION"]);
			string sUSER_PHONE_WORK  = Sql.ToString(HttpContext.Current.Session["PHONE_WORK" ]);
			if ( Sql.IsEmptyString(sUSER_EXTENSION) )
				throw(new Exception("Phone Extension not specified in user profile."));
			*/
			return _asteriskManager.OriginateCall(sUSER_EXTENSION, sUSER_FULL_NAME, sUSER_PHONE_WORK, sPHONE, sPARENT_ID, sPARENT_TYPE);
		}

		// 11/15/2014 Paul.  Hub method should require authorization. 
		// http://eworldproblems.mbaynton.com/2012/12/signalr-hub-authorization/
		[SplendidHubAuthorize]
		public Guid CreateCall(string sUniqueId)
		{
			return _asteriskManager.CreateCall(sUniqueId);
		}
	}
}

