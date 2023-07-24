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
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for TwiML.
	/// </summary>
	public class TwiML : SplendidPage
	{
		// 09/25/2013 Paul.  This page must be accessible without authentication. 
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			string sFormBody = String.Empty;
			if (Request.RequestType == "POST")
			{
				// AccountSid=ACd956f8185bce3b8c1178ff2b23459197&SmsStatus=sent&Body=Sent+from+your+Twilio+trial+account+-+PayTrace+logo&SmsSid=SMc6f69d092635f5e83adb39869d303aae&To=%2B19196041258&From=%2B19032252332&ApiVersion=2010-04-01
				using ( StreamReader rdr = new StreamReader(Request.InputStream) )
				{
					sFormBody = rdr.ReadToEnd();
				}
			}
#if DEBUG
			Debug.WriteLine("TwiML.QueryString: " + Request.QueryString);
			if ( !Sql.IsEmptyString(sFormBody) )
				Debug.WriteLine("TwiML.Body: " + sFormBody);
#endif
			try
			{
				Guid   gID          = Sql.ToGuid  (Request["ID"        ]);
				string sAccountSid  = Sql.ToString(Request["AccountSid"]);
				string sSmsStatus   = Sql.ToString(Request["SmsStatus" ]);
				string sApiVersion  = Sql.ToString(Request["ApiVersion"]);
				string sSUBJECT     = Sql.ToString(Request["Body"      ]);
				string sMESSAGE_SID = Sql.ToString(Request["SmsSid"    ]);
				string sTO_NUMBER   = Sql.ToString(Request["To"        ]);
				string sFROM_NUMBER = Sql.ToString(Request["From"      ]);
				// 09/29/2013 Paul.  Received messages have more data. 
				string sToCity      = Sql.ToString(Request["ToCity"     ]);
				string sToState     = Sql.ToString(Request["ToState"    ]);
				string sToZip       = Sql.ToString(Request["ToZip"      ]);
				string sToCountry   = Sql.ToString(Request["ToCountry"  ]);
				string sFromCity    = Sql.ToString(Request["FromCity"   ]);
				string sFromState   = Sql.ToString(Request["FromState"  ]);
				string sFromZip     = Sql.ToString(Request["FromZip"    ]);
				string sFromCountry = Sql.ToString(Request["FromCountry"]);
				string sFROM_LOCATION = String.Empty;
				string sTO_LOCATION   = String.Empty;
				// 09/26/2013 Paul.  In order to ensure the integrity of the post, the submitted ID must match the configuration value. 
				if ( sAccountSid == Sql.ToString(Context.Application["CONFIG.Twilio.AccountSID"]) )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						SqlProcs.spSMS_MESSAGES_UpdateStatus(gID, sSmsStatus, sMESSAGE_SID);
					}
					else if ( sSmsStatus == "received" )
					{
						if ( !Sql.IsEmptyString(sFromCity) ) sFromCity += ",";
						if ( !Sql.IsEmptyString(sToCity  ) ) sToCity   += ",";
						sFROM_LOCATION = (sFromCity + " " + sFromState + " " + sFromZip + " " + sFromCountry).Trim();
						sTO_LOCATION   = (sToCity   + " " + sToState   + " " + sToZip   + " " + sToCountry  ).Trim();
						// 11/27/2022 Paul.  If SignalR is disabled, we need to manually initialize twilio manager. 
						if ( TwilioManager.Instance == null )
						{
							TwilioManager.InitApp(this.Context);
						}
						TwilioManager.Instance.NewSmsMessage(sMESSAGE_SID, sFROM_NUMBER, sTO_NUMBER, sSUBJECT, sFROM_LOCATION, sTO_LOCATION);
					}
				}
				else
				{
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Unknown Twilio event: " + Request.RawUrl + ControlChars.CrLf + sFormBody);
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

