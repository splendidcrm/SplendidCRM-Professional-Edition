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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using SplendidCRM._controls;

namespace SplendidCRM.Calendar
{
	/// <summary>
	///		Summary description for NewRecord.
	/// </summary>
	public class NewRecord : SplendidControl
	{
		protected Label                      lblError          ;
		protected RadioButton                radScheduleCall   ;
		protected RadioButton                radScheduleMeeting;
		protected TextBox                    txtNAME           ;
		protected Label                      lblDATEFORMAT     ;
		protected Label                      lblTIMEFORMAT     ;
		protected DatePicker                 ctlDATE_START     ;
		protected TextBox                    txtTIME_START     ;
		protected RequiredFieldValidator     reqNAME           ;
		protected RequiredFieldValidator     reqTIME_START     ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "NewRecord" )
			{
				reqNAME      .Enabled = true;
				reqTIME_START.Enabled = true;
				reqNAME      .Validate();
				reqTIME_START.Validate();
				if ( Page.IsValid )
				{
					Guid gID = Guid.Empty;
					try
					{
						// 02/28/2006 Paul.  The easiest way to parse the two separate date/time fields is to combine the text. 
						DateTime dtDATE_START = T10n.ToServerTime(Sql.ToDateTime(ctlDATE_START.DateText + " " + txtTIME_START.Text));
						// 01/16/2012 Paul.  Assigned User ID and Team ID are now parameters. 
						if ( radScheduleCall.Checked )
						{
							SqlProcs.spCALLS_New
								( ref gID
								, txtNAME.Text
								, dtDATE_START
								, Security.USER_ID
								, Security.TEAM_ID
								, String.Empty
								// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
								, String.Empty      // ASSIGNED_SET_LIST
								);
						}
						else
						{
							SqlProcs.spMEETINGS_New
								( ref gID
								, txtNAME.Text
								, dtDATE_START
								, Security.USER_ID
								, Security.TEAM_ID
								, String.Empty
								// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
								, String.Empty      // ASSIGNED_SET_LIST
								);
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
					if ( !Sql.IsEmptyGuid(gID) )
					{
						if ( radScheduleCall.Checked )
							Response.Redirect("~/Calls/view.aspx?ID=" + gID.ToString());
						else
							Response.Redirect("~/Meetings/view.aspx?ID=" + gID.ToString());
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			radScheduleCall   .Visible = (SplendidCRM.Security.GetUserAccess("Calls"   , "edit") >= 0);
			radScheduleMeeting.Visible = (SplendidCRM.Security.GetUserAccess("Meetings", "edit") >= 0);

			// 06/04/2006 Paul.  NewRecord should not be displayed if the user does not have edit rights. 
			// 01/02/2020 Paul.  Allow the NewRecord to be disabled per module using config table. 
			this.Visible = !Sql.ToBoolean(Application["CONFIG." + m_sMODULE + ".DisableNewRecord"]) && (radScheduleCall.Visible || radScheduleMeeting.Visible);
			if ( !this.Visible )
				return;

			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();  // Need to bind so that Text of the Button gets updated. 
			reqNAME      .ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Calls.LBL_LIST_SUBJECT") + "<br>";
			reqTIME_START.ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Calls.LBL_LIST_TIME"   ) + "<br>";
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( !bIsPostBack )
			{
				// 05/06/2010 Paul.  When the control is created out-of-band, we need to manually bind the controls. 
				if ( NotPostBack )
					this.DataBind();
				DateTime dt1100PM = DateTime.Today.AddHours(23);
				lblDATEFORMAT.Text = "(" + Session["USER_SETTINGS/DATEFORMAT"] + ")";
				lblTIMEFORMAT.Text = "(" + dt1100PM.ToShortTimeString() + ")";

				DateTime dtNow = T10n.FromServerTime(DateTime.Now);
				ctlDATE_START.Value = dtNow;
				txtTIME_START.Text  = Sql.ToTimeString(dtNow);
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}

