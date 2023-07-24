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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Users
{
	/// <summary>
	/// Summary description for Logout.
	/// </summary>
	public class Logout : SplendidPage
	{
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 02/28/2007 Paul.  Centralize session reset to prepare for WebParts. 
			//Security.Clear();
			//SplendidInit.InitSession();
			// 01/08/2008 Paul.  Clear and InitSession are not working.  The user remains authenticated.
			// Abandon works much better. 
			// 04/15/2008 Paul.  Although we are going to continue to use Abandon, the original problem 
			// was likely due to the identity being set inside Global.assx Application_AcquireRequestState
			// whereby we were setting the HttpContext.Current.User = GenericPrincipal().
			// Resetting the identity very likely would have fixed the issue with the user remaining authenticated. 
			try
			{
				Guid gUSER_LOGIN_ID = Security.USER_LOGIN_ID;
				if ( !Sql.IsEmptyGuid(gUSER_LOGIN_ID) )
					SqlProcs.spUSERS_LOGINS_Logout(gUSER_LOGIN_ID);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}

			Session.Abandon();
			// 11/15/2014 Paul.  Prevent resuse of SessionID. 
			// http://support.microsoft.com/kb/899918
			Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
			// 12/25/2018 Paul.  Logout should perform Azure or ADFS logout. 
			if ( Sql.ToBoolean(Application["CONFIG.ADFS.SingleSignOn.Enabled"]) )
			{
				string sRequestURL = ActiveDirectory.FederationServicesLogout(Context);
				Response.Redirect(sRequestURL);
			}
			else if ( Sql.ToBoolean(Application["CONFIG.Azure.SingleSignOn.Enabled"]) )
			{
				string sRequestURL = ActiveDirectory.AzureLogout(Context);
				Response.Redirect(sRequestURL);
			}
			else
			{
				Response.Redirect("Login.aspx");
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

