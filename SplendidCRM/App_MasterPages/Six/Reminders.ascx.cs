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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Themes.Six
{
	/// <summary>
	///		Summary description for Reminders.
	/// </summary>
	public class Reminders : SplendidControl
	{
		protected HtmlGenericControl divReminders      ;
		protected Repeater           ctlRepeater       ;
		protected Button             btnREMINDER_UPDATE;
		protected HyperLink          lnkShowSubPanel   ;
		protected HyperLink          lnkHideSubPanel   ;
		protected Label              lblScripts        ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Edit" )
			{
				Response.Redirect("~/Activities/edit.aspx?ID=" + Sql.ToString(e.CommandArgument));
			}
			else if ( e.CommandName == "Dismiss" )
			{
				try
				{
					Guid gID = Sql.ToGuid(e.CommandArgument);
					SqlProcs.spACTIVITIES_UpdateDismiss(gID, Security.USER_ID, true);
					SplendidCache.ClearUserReminders();
					Bind();
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			else if ( e.CommandName == "Update" )
			{
				// 12/24/2012 Paul.  We only need to clear the reminders when debugging. 
				//SplendidCache.ClearUserReminders();
				//Bind();
			}
		}

		private void Bind()
		{
			DataTable dtReminders = SplendidCache.UserReminders();
			ctlRepeater.DataSource = dtReminders;
			ctlRepeater.DataBind();
			// 12/24/2012 Paul.  Get the smallest seconds 
			int nVisibleCount = 0;
			DateTime dtFIRST_REMINDER = DateTime.MaxValue;
			foreach ( DataRow row in dtReminders.Rows )
			{
				int      nREMINDER_TIME   = Sql.ToInteger (row["REMINDER_TIME"]);
				DateTime dtDATE_START     = Sql.ToDateTime(row["DATE_START"   ]);
				DateTime dtREMINDER_START = dtDATE_START.AddSeconds(-nREMINDER_TIME);
				if ( DateTime.Now > dtREMINDER_START )
					nVisibleCount++;
				// 12/24/2012 Paul.  We only need to activate the timer for records that are not visible. 
				else if ( dtREMINDER_START < dtFIRST_REMINDER )
					dtFIRST_REMINDER = dtREMINDER_START;
			}
			divReminders.Visible = (nVisibleCount > 0);
			if ( dtFIRST_REMINDER != DateTime.MaxValue )
			{
				TimeSpan ts = dtFIRST_REMINDER - DateTime.Now;
				if ( ts.TotalMilliseconds > 0 )
				{
					string sTimeoutScript = "setTimeout(function(){document.getElementById('" + btnREMINDER_UPDATE.ClientID + "').click();}, " + Convert.ToInt32(ts.TotalMilliseconds).ToString() + "); // " + ts.TotalMinutes.ToString() + " minutes";
					// 03/24/2013 Paul.  Disable debug code. 
					//lblScripts.Text += sTimeoutScript;
					// 12/27/2012 Paul.  To register a script block every time that an asynchronous postback occurs, use the RegisterClientScriptBlock(Page, Type, String, String, Boolean) overload of this method. 
					// http://msdn.microsoft.com/en-us/library/bb338357.aspx
					ScriptManager.RegisterClientScriptBlock(Page, typeof(Reminders), UniqueID, sTimeoutScript, true);
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if ( Sql.ToBoolean(Application["CONFIG.enable_reminder_popdowns"]) )
				{
					Bind();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			GetL10n();
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}

