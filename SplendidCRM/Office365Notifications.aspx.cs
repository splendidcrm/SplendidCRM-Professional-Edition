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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Spring.Json;
using Spring.Social.Office365;
using Spring.Social.Office365.Api;
using Spring.Social.Office365.Api.Impl.Json;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for TwiML.
	/// </summary>
	public class Office365Notifications : SplendidPage
	{
		// 12/24/2020 Paul.  This page must be accessible without authentication. 
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			string sFormBody = String.Empty;
			if ( Request.RequestType == "POST" )
			{
				using ( StreamReader rdr = new StreamReader(Request.InputStream) )
				{
					sFormBody = rdr.ReadToEnd();
				}
			}
			try
			{
				string validationToken = Sql.ToString(Request.QueryString["validationToken"]);
				if ( !Sql.IsEmptyString(validationToken) )
				{
					//Debug.WriteLine("Office365Notifications validationToken: " + validationToken);
					Response.ContentType = Request.ContentType;
					Response.Write(validationToken);
				}
				else if ( Request.ContentType.StartsWith("application/json") )
				{
#if DEBUG
					Debug.WriteLine("Office365Notifications.ContentType: " + Request.ContentType);
					Debug.WriteLine("Office365Notifications.QueryString: " + Request.QueryString);
					if ( !Sql.IsEmptyString(sFormBody) )
						Debug.WriteLine("Office365Notifications.Body: " + sFormBody);
#endif
					JsonValue json = JsonValue.Parse(sFormBody);
					JsonMapper jsonMapper = new JsonMapper();
					jsonMapper.RegisterDeserializer(typeof(ResourceData                   ), new ResourceDataDeserializer                ());
					jsonMapper.RegisterDeserializer(typeof(SubscriptionNotification       ), new SubscriptionNotificationDeserializer    ());
					jsonMapper.RegisterDeserializer(typeof(IList<SubscriptionNotification>), new SubscriptionNotificationListDeserializer());
					jsonMapper.RegisterDeserializer(typeof(SubscriptionNotificationBody   ), new SubscriptionNotificationBodyDeserializer());
					SubscriptionNotificationBody body = jsonMapper.Deserialize<SubscriptionNotificationBody>(json);
					foreach ( SubscriptionNotification notification in body.values )
					{
						//Debug.WriteLine(notification.ToString());
						Guid gUSER_ID = Sql.ToGuid(notification.ClientState);
						if ( !Sql.IsEmptyGuid(gUSER_ID) )
						{
							bool bVERBOSE_STATUS = Sql.ToBoolean(Context.Application["CONFIG.Exchange.VerboseStatus"      ]);
							if ( bVERBOSE_STATUS )
								SyncError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "Office365Notifications: " + Sql.ToString(notification.ChangeType) + " " + Sql.ToString(notification.LifecycleEvent) + " for " + gUSER_ID.ToString() + ", " + notification.Resource);
							// 12/25/2020 Paul.  Use a queue to prevent hitting concurrency limit. 
							// Application is over its MailboxConcurrency limit.
							Spring.Social.Office365.Office365Sync.AddNotificationToQueue(Context, notification);
						}
						Response.StatusCode = 202;
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}

