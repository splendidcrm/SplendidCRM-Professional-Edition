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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for FacebookUtils.
	/// </summary>
	public class FacebookUtils
	{
		protected string   sAppID            ;
		protected string   sAppSecret        ;
		protected string   sAccessToken      ;
		protected string   sBaseDomain       ;
		protected DateTime dtExpires         ;
		protected string   sSecret           ;
		protected string   sSessionKey       ;
		protected string   sSig              ;
		protected string   sUID              ;
		protected string   sComputedSignature;
		protected NameValueCollection arrValues;

		public string UID
		{
			get { return sUID; }
		}

		public DateTime Expires
		{
			get { return dtExpires; }
		}

		public bool FacebookValuesExist
		{
			get { return !Sql.IsEmptyString(sAppID) && (arrValues != null); }
		}

		public FacebookUtils(string sAppID, string sAppSecret, HttpCookieCollection cookies)
		{
			this.sAppID     = sAppID    ;
			this.sAppSecret = sAppSecret;
			
			HttpCookie cFacebook = cookies["fbs_" + sAppID];
			if ( cFacebook != null )
			{
				arrValues = HttpUtility.ParseQueryString(cFacebook.Value.Replace("\"", string.Empty));
			}
		}

		public bool ParseCookie()
		{
			// 03/19/2011 Paul.  We need to reparse the cookie so that the values are properly unescaped. 
			if ( arrValues != null )
			{
				StringBuilder sbPayload = new StringBuilder();
				foreach ( string sKey in arrValues )
				{
					if ( sKey != "sig" )
						sbPayload.AppendFormat("{0}={1}", sKey, arrValues[sKey]);
				}
				sbPayload.Append(sAppSecret);
				// 03/19/2011 Paul.  facebook uses the same MD5 hash that we use for SplendidCRM passwords. 
				sComputedSignature = Security.HashPassword(sbPayload.ToString());
				
				long lExpires;
				DateTime dtUnixEpoch = new DateTime(1970, 1, 1);
				sAccessToken = arrValues["access_token"];
				sBaseDomain  = arrValues["base_domain" ];
				long.TryParse(arrValues["expires"], out lExpires);  // Unix timestamp. 
				dtExpires    = dtUnixEpoch.AddSeconds(lExpires);
				sSecret      = arrValues["secret"      ];
				sSessionKey  = arrValues["session_key" ];
				sSig         = arrValues["sig"         ];
				sUID         = arrValues["uid"         ];  // This is the facebook User ID. 
			}
			return IsValidSignature();
		}

		public bool IsValidSignature()
		{
			return (sSig == sComputedSignature);
		}
	}
}
