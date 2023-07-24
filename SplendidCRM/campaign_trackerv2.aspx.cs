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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for campaign_trackerv2.
	/// </summary>
	public class campaign_trackerv2 : SplendidPage
	{
		// 01/25/2008 Paul.  This page must be accessible without authentication. 
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 04/11/2008 Paul.  Expire immediately so that all clicks are counted. 
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);

			SplendidError.SystemMessage("Log", new StackTrace(true).GetFrame(0), "Campaign Tracker v2 " + Request["identifier"] + ", " + Request["track"]);
			Guid gID      = Sql.ToGuid(Request["identifier"]);
			Guid gTrackID = Sql.ToGuid(Request["track"     ]);
			try
			{
				if ( !Sql.IsEmptyGuid(gID) )
				{
					Guid   gTARGET_ID   = Guid.Empty;
					string sTARGET_TYPE = string.Empty;
					SqlProcs.spCAMPAIGN_LOG_UpdateTracker(gID, "link", gTrackID, ref gTARGET_ID, ref sTARGET_TYPE);
				}
				else
				{
					// 09/10/2007 Paul.  Web campaigns will not have an identifier. 
					SqlProcs.spCAMPAIGN_LOG_BannerTracker("link", gTrackID, Request.UserHostAddress);
				}
				if ( !Sql.IsEmptyGuid(gTrackID) )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						sSQL = "select TRACKER_URL     " + ControlChars.CrLf
						     + "  from vwCAMPAIGN_TRKRS" + ControlChars.CrLf
						     + " where ID = @ID        " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gTrackID);
							string sTRACKER_URL = Sql.ToString(cmd.ExecuteScalar());
							if ( !Sql.IsEmptyString(sTRACKER_URL) )
								Response.Redirect(sTRACKER_URL);
						}
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

