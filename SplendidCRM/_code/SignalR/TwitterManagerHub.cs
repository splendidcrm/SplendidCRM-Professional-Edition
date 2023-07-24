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
	public interface ITwitterServer
	{
		string JoinGroup(string sConnectionId, string sGroupName);
	}

	/// <summary>
	/// Summary description for TwitterManagerHub.
	/// </summary>
	[HubName("TwitterManagerHub")]
	public class TwitterManagerHub : Hub<ITwitterServer>
	{
		private readonly TwitterManager _TwitterManager;

		public TwitterManagerHub() : this(TwitterManager.Instance)
		{
		}

		public TwitterManagerHub(TwitterManager TwitterManager)
		{
			_TwitterManager = TwitterManager;
		}

		// 11/15/2014 Paul.  Hub method should require authorization. 
		// http://eworldproblems.mbaynton.com/2012/12/signalr-hub-authorization/
		[SplendidHubAuthorize]
		public string JoinGroup(string sConnectionId, string sGroupName)
		{
			if ( !Sql.IsEmptyString(sGroupName) )
			{
				// 10/26/2013 Paul.  Each track is a separate group. 
				// 10/27/2013 Paul.  The group string is already expected to be in lowercase so that we don't have to waste time doing it now. 
				string[] arrTracks = sGroupName.Split(',');
				foreach ( string sTrack in arrTracks )
					Groups.Add(sConnectionId, sTrack).Wait();
				return sConnectionId + " joined " + sGroupName;
			}
			return "Group not specified.";
		}
	}
}

