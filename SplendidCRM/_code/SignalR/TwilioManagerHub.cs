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
	public interface ITwilioServer
	{
		string JoinGroup(string sConnectionId, string sGroupName);
		Guid CreateSmsMessage(string sMESSAGE_SID, string sFROM_NUMBER, string sTO_NUMBER, string sSUBJECT);
	}

	/// <summary>
	/// Summary description for TwilioManagerHub.
	/// </summary>
	[HubName("TwilioManagerHub")]
	public class TwilioManagerHub : Hub<ITwilioServer>
	{
		private readonly TwilioManager _twilioManager;

		public TwilioManagerHub() : this(TwilioManager.Instance)
		{
		}

		public TwilioManagerHub(TwilioManager TwilioManager)
		{
			_twilioManager = TwilioManager;
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
				sGroupName = Utils.NormalizePhone(TwilioManager.RemoveCountryCode(sGroupName));
				Groups.Add(sConnectionId, sGroupName).Wait();
				return sConnectionId + " joined " + sGroupName;
			}
			return "Group not specified.";
		}

		// 11/15/2014 Paul.  Hub method should require authorization. 
		// http://eworldproblems.mbaynton.com/2012/12/signalr-hub-authorization/
		[SplendidHubAuthorize]
		public Guid CreateSmsMessage(string sMESSAGE_SID, string sFROM_NUMBER, string sTO_NUMBER, string sSUBJECT)
		{
			return _twilioManager.CreateSmsMessage(sMESSAGE_SID, sFROM_NUMBER, sTO_NUMBER, sSUBJECT, String.Empty, String.Empty);
		}
	}
}

