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
using System.Net;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SplendidCRM.GoogleOAuth
{
	/// <summary>
	/// Summary description for Google_Webhook.
	/// </summary>
	public class Google_Webhook : SplendidPage
	{
		// 09/14/2015 Paul.  This page must be accessible without authentication. 
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// User-Agent: APIs-Google; (+https://developers.google.com/webmasters/APIs-Google.html)
				// X-Goog-Channel-ID: 735ad244-5132-4f5d-8fb6-9aa883697c77
				// X-Goog-Channel-Expiration: Tue, 22 Sep 2015 09:51:46 GMT
				// X-Goog-Resource-State: exists
				// X-Goog-Message-Number: 165139
				// X-Goog-Resource-ID: M9-5cvkI1ybZzvVLAyMHMHMa_VI
				// X-Goog-Resource-URI: https://www.googleapis.com/calendar/v3/calendars/g0hr9n0oa09eikbm4rakh9huq0@group.calendar.google.com/events?alt=json
				// X-Goog-Channel-Token: 4836884c-5d8a-4d6b-8689-bfbeb33e902e
				string sChannelId         = String.Empty;
				string sChannelExpiration = String.Empty;
				string sResourceState     = String.Empty;
				string sMessageNumber     = String.Empty;
				string sResourceId        = String.Empty;
				string sResourceURI       = String.Empty;
				string sChannelToken      = String.Empty;
				
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Google_Webhook.aspx " + DateTime.Now.ToString());
				foreach ( string sHeaderName in Request.Headers.AllKeys )
				{
					sb.AppendLine(sHeaderName + ": " + Request.Headers[sHeaderName]);
					switch ( sHeaderName )
					{
						case "X-Goog-Channel-ID"        :  sChannelId         = Request.Headers[sHeaderName];  break;
						case "X-Goog-Channel-Expiration":  sChannelExpiration = Request.Headers[sHeaderName];  break;
						case "X-Goog-Resource-State"    :  sResourceState     = Request.Headers[sHeaderName];  break;
						case "X-Goog-Message-Number"    :  sMessageNumber     = Request.Headers[sHeaderName];  break;
						case "X-Goog-Resource-ID"       :  sResourceId        = Request.Headers[sHeaderName];  break;
						case "X-Goog-Resource-URI"      :  sResourceURI       = Request.Headers[sHeaderName];  break;
						case "X-Goog-Channel-Token"     :  sChannelToken      = Request.Headers[sHeaderName];  break;
					}
				}
				Debug.WriteLine(sb.ToString());
				if ( !Sql.IsEmptyString(sChannelId) && !Sql.IsEmptyString(sChannelToken) )
					GoogleSync.GoogleWebhook.AddWebhook(Context, sChannelId, sChannelExpiration, sResourceState, sMessageNumber, sResourceId, sResourceURI, sChannelToken);
				//Response.StatusCode = GoogleSync.ProcessNotification(Context, sChannelID, sChannelExpiration, sResourceState, sMessageNumber, sResourceID, sResourceURI, sChannelToken);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Clear();
				Response.StatusCode = 404;
				Response.Status = "404 Page Not Found";
				Response.End();
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
