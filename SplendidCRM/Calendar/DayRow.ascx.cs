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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Globalization;

namespace SplendidCRM.Calendar
{
	/// <summary>
	///		Summary description for DayRow.
	/// </summary>
	public class DayRow : SplendidControl
	{
		protected DateTime      dtDATE_START = DateTime.MinValue;
		//protected DateTime      dtDATE_END   = DateTime.MaxValue;
		protected DataView      vwMain             ;
		protected DataList      lstMain            ;
		protected Label         lblError           ;

		protected RadioButton   radScheduleCall    ;
		protected RadioButton   radScheduleMeeting ;
		protected TextBox       txtNAME            ;
		// 06/14/2006 Paul.  The Italian problem was that it was using the culture separator, but DataView only supports the en-US format. 
		protected Button        btnSave            ;
		protected CultureInfo   ciEnglish          ;

		public CommandEventHandler Command;

		public DateTime DATE_START
		{
			get
			{
				return dtDATE_START;
			}
			set
			{
				dtDATE_START = value;
				// 06/15/2006 Paul.  Instead of binding the command argument at render time, we must bind 
				// here to ensure that ciEnglish has been created.  This is normally not a problem, but it is with DayRow
				// because we are manually loading the control during the rendering of DayGrid. 
				if ( ciEnglish == null )
					ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
				btnSave.CommandArgument = dtDATE_START.ToString(CalendarControl.SqlDateTimeFormat);
			}
		}
		/*
		public DateTime DATE_END
		{
			get
			{
				return dtDATE_END;
			}
			set
			{
				dtDATE_END = value;
			}
		}
		*/
		public DataView DataSource
		{
			set
			{
				vwMain = value;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( !Sql.IsEmptyString(txtNAME.Text) && Information.IsDate(e.CommandArgument) )
				{
					// 06/09/2006 Paul.  Add code to create call or meeting. This code did not make the 1.0 release. 
					dtDATE_START = Sql.ToDateTime(e.CommandArgument);
					if ( radScheduleCall.Checked )
					{
						Guid gID = Guid.Empty;
						// 01/16/2012 Paul.  Assigned User ID and Team ID are now parameters. 
						//SqlProcs.spCALLS_New(ref gID, txtNAME.Text, T10n.ToServerTime(dtDATE_START), Security.USER_ID, Security.TEAM_ID, String.Empty);
						// 01/27/2011 Paul.  In order to honor team management, we need to use the base procedure. 
						// 12/26/2012 Paul.  Add EMAIL_REMINDER_TIME. 
						// 03/07/2013 Paul.  Add ALL_DAY_EVENT. 
						// 03/20/2013 Paul.  Add REPEAT fiels. 
						// 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
						// 09/14/2015 Paul.  Default for reminders should be 0. 
						SqlProcs.spCALLS_Update
							( ref gID
							, Security.USER_ID
							, txtNAME.Text
							, 1
							, 0
							, T10n.ToServerTime(dtDATE_START)
							, String.Empty
							, Guid.Empty
							, "Planned"
							, "Outbound"
							, 0                  // REMINDER_TIME
							, String.Empty
							, String.Empty
							, Security.TEAM_ID
							, String.Empty
							, 0                  // EMAIL_REMINDER_TIME
							, false              // ALL_DAY_EVENT
							, String.Empty       // REPEAT_TYPE
							, 0                  // REPEAT_INTERVAL
							, String.Empty       // REPEAT_DOW
							, DateTime.MinValue  // REPEAT_UNTIL
							, 0                  // REPEAT_COUNT
							, 0                  // SMS_REMINDER_TIME
							// 05/17/2017 Paul.  Add Tags module. 
							, String.Empty       // TAG_SET_NAME
							// 11/07/2017 Paul.  Add IS_PRIVATE for use by a large customer. 
							, false              // IS_PRIVATE
							// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
							, String.Empty       // ASSIGNED_SET_LIST
							);
					}
					else if ( radScheduleMeeting.Checked )
					{
						Guid gID = Guid.Empty;
						// 01/16/2012 Paul.  Assigned User ID and Team ID are now parameters. 
						//SqlProcs.spMEETINGS_New(ref gID, txtNAME.Text, T10n.ToServerTime(dtDATE_START), Security.USER_ID, Security.TEAM_ID, String.Empty);
						// 01/27/2011 Paul.  In order to honor team management, we need to use the base procedure. 
						// 12/26/2012 Paul.  Add EMAIL_REMINDER_TIME. 
						// 03/07/2013 Paul.  Add ALL_DAY_EVENT. 
						// 03/20/2013 Paul.  Add REPEAT fields. 
						// 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
						// 09/14/2015 Paul.  Default for reminders should be 0. 
						SqlProcs.spMEETINGS_Update
							( ref gID
							, Security.USER_ID
							, txtNAME.Text
							, String.Empty
							, 1
							, 0
							, T10n.ToServerTime(dtDATE_START)
							, "Planned"
							, String.Empty
							, Guid.Empty
							, 0                  // REMINDER_TIME
							, String.Empty
							, String.Empty
							, Security.TEAM_ID
							, String.Empty
							, 0                  // EMAIL_REMINDER_TIME
							, false              // ALL_DAY_EVENT
							, String.Empty       // REPEAT_TYPE
							, 0                  // REPEAT_INTERVAL
							, String.Empty       // REPEAT_DOW
							, DateTime.MinValue  // REPEAT_UNTIL
							, 0                  // REPEAT_COUNT
							, 0                  // SMS_REMINDER_TIME
							// 05/17/2017 Paul.  Add Tags module. 
							, String.Empty       // TAG_SET_NAME
							// 11/07/2017 Paul.  Add IS_PRIVATE for use by a large customer. 
							, false              // IS_PRIVATE
							// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
							, String.Empty       // ASSIGNED_SET_LIST
							);
					}
				}
			}
			if ( Command != null )
				Command(this, e);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			/*
			// 09/27/2005 Paul.  Instead of performing a query for each cell, just do one query for the entire range and filter before each cell. 
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                                                       " + ControlChars.CrLf
					     + "  from vwACTIVITIES_List                                       " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
						Security.Filter(cmd, "Calls", "list");
						// 01/16/2007 Paul.  Use AppendParameter so that duplicate ASSIGNED_USER_ID can be avoided. 
						// 01/19/2007 Paul.  Fix AppendParamenter.  @ should not be used in field name. 
						Sql.AppendParameter(cmd, Security.USER_ID, "ASSIGNED_USER_ID");
						cmd.CommandText += "   and (   DATE_START >= @DATE_START and DATE_START < @DATE_END" + ControlChars.CrLf;
						cmd.CommandText += "        or DATE_END   >= @DATE_START and DATE_END   < @DATE_END" + ControlChars.CrLf;
						cmd.CommandText += "        or DATE_START <  @DATE_START and DATE_END   > @DATE_END" + ControlChars.CrLf;
						cmd.CommandText += "       )                                                       " + ControlChars.CrLf;
						cmd.CommandText += " order by DATE_START asc, NAME asc                             " + ControlChars.CrLf;
						// 03/19/2007 Paul.  Need to query activities based on server time. 
						Sql.AddParameter(cmd, "@DATE_START", T10n.ToServerTime(dtDATE_START));
						Sql.AddParameter(cmd, "@DATE_END"  , T10n.ToServerTime(dtDATE_END  ));

						if ( bDebug )
							RegisterClientScriptBlock("vwACTIVITIES_List" + dtDATE_START.ToOADate().ToString(), Sql.ClientScriptBlock(cmd));

						try
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									vwMain = dt.DefaultView;
									lstMain.DataSource = vwMain ;
									lstMain.DataBind();
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							lblError.Text = ex.Message;
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
			*/
			//lstMain.DataSource = vwMain ;
			//lstMain.DataBind();
			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//Page.DataBind();
		}

		private void Page_DataBind(object sender, System.EventArgs e)
		{
			// 03/19/2007 Paul.  We were having a problem with the calendar data appearing during print view.  We needed to rebind the data. 
			lstMain.DataSource = vwMain ;
			lstMain.DataBind();
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
			this.Load += new System.EventHandler(this.Page_Load);
			this.DataBinding += new System.EventHandler(this.Page_DataBind);
		}
		#endregion
	}
}

