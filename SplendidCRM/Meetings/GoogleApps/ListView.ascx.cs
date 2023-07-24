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
using System.Data;
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace SplendidCRM.Meetings.GoogleApps
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected _controls.HeaderButtons ctlModuleHeader;
		protected _controls.ExportHeader ctlExportHeader;
		protected _controls.SearchView   ctlSearchView  ;
		protected _controls.CheckAll     ctlCheckAll    ;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected MassUpdate    ctlMassUpdate  ;
		// 09/05/2015 Paul.  Google now uses OAuth 2.0. 
		protected TextBox       OAUTH_ACCESS_TOKEN       ;
		protected TextBox       OAUTH_REFRESH_TOKEN      ;
		protected TextBox       OAUTH_EXPIRES_IN         ;
		protected Button        btnGoogleAuthorized      ;
		// 06/06/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected Panel         pnlMassUpdateSeven;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					// 03/17/2011 Paul.  We need to treat a comma-separated list of fields as an array. 
					arrSelectFields.AddFields(grdMain.SortColumn);
				}
				// 11/17/2010 Paul.  Populate the hidden Selected field with all IDs. 
				else if ( e.CommandName == "SelectAll" )
				{
					// 05/22/2011 Paul.  When using custom paging, vwMain may not be defined. 
					if ( vwMain == null )
						grdMain.DataBind();
					ctlCheckAll.SelectAll(vwMain, "ID");
					grdMain.DataBind();
				}
				// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
				else if ( e.CommandName == "ToggleMassUpdate" )
				{
					pnlMassUpdateSeven.Visible = !pnlMassUpdateSeven.Visible;
				}
				else if ( e.CommandName == "MassDelete" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						//Response.Redirect("default.aspx");
					}
				}
				else if ( e.CommandName == "Export" )
				{
					// 11/03/2006 Paul.  Apply ACL rules to Export. 
					int nACLACCESS = SplendidCRM.Security.GetUserAccess(m_sMODULE, "export");
					if ( nACLACCESS  >= 0 )
					{
						// 10/05/2009 Paul.  When exporting, we may need to manually bind.  Custom paging should be disabled when exporting all. 
						if ( vwMain == null )
							grdMain.DataBind();
						if ( nACLACCESS == ACL_ACCESS.OWNER )
							vwMain.RowFilter = "ASSIGNED_USER_ID = '" + Security.USER_ID.ToString() + "'";
						// 11/27/2010 Paul.  Use new selected items. 
						string[] arrID = ctlCheckAll.SelectedItemsArray;
						SplendidExport.Export(vwMain, m_sMODULE, ctlExportHeader.ExportFormat, ctlExportHeader.ExportRange, grdMain.CurrentPageIndex, grdMain.PageSize, arrID, grdMain.AllowCustomPaging);
					}
				}
				// 09/05/2015 Paul.  Google now uses OAuth 2.0. 
				else if ( e.CommandName == "GoogleApps.Authorize" )
				{
					try
					{
						DateTime dtOAUTH_EXPIRES_AT = DateTime.Now.AddSeconds(Sql.ToInteger(OAUTH_EXPIRES_IN.Text));
						SqlProcs.spOAUTH_TOKENS_Update(Security.USER_ID, "GoogleApps", OAUTH_ACCESS_TOKEN.Text, String.Empty, dtOAUTH_EXPIRES_AT, OAUTH_REFRESH_TOKEN.Text);
						lblError.Text = L10n.Term("Google.LBL_TEST_SUCCESSFUL");
					}
					catch(Exception ex)
					{
						lblError.Text = ex.Message;
					}
				}
				else if ( e.CommandName == "GoogleApps.RefreshToken" )
				{
					try
					{
						SplendidCRM.GoogleApps.RefreshAccessToken(Application, Security.USER_ID, true);
						lblError.Text = L10n.Term("Google.LBL_TEST_SUCCESSFUL");
						Bind();
					}
					catch(Exception ex)
					{
						lblError.Text =  Utils.ExpandException(ex);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Bind()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ID"                  , typeof(String  ));
			dt.Columns.Add("NAME"                , typeof(String  ));
			dt.Columns.Add("DATE_ENTERED"        , typeof(DateTime));
			dt.Columns.Add("DATE_MODIFIED"       , typeof(DateTime));
			dt.Columns.Add("ASSIGNED_TO_NAME"    , typeof(String  ));
			dt.Columns.Add("CREATED_BY_NAME"     , typeof(String  ));
			dt.Columns.Add("MODIFIED_BY_NAME"    , typeof(String  ));
			dt.Columns.Add("MODIFIED_USER_ID"    , typeof(Guid    ));
			dt.Columns.Add("TEAM_ID"             , typeof(Guid    ));
			dt.Columns.Add("TEAM_NAME"           , typeof(String  ));
			dt.Columns.Add("TEAM_SET_NAME"       , typeof(String  ));
			dt.Columns.Add("TEAM_SET_LIST"       , typeof(String  ));
			dt.Columns.Add("LOCATION"            , typeof(String  ));
			dt.Columns.Add("DURATION_HOURS"      , typeof(int     ));
			dt.Columns.Add("DURATION_MINUTES"    , typeof(int     ));
			dt.Columns.Add("DATE_TIME"           , typeof(DateTime));
			dt.Columns.Add("PARENT_TYPE"         , typeof(String  ));
			dt.Columns.Add("PARENT_ID"           , typeof(Guid    ));
			dt.Columns.Add("STATUS"              , typeof(String  ));
			dt.Columns.Add("DIRECTION"           , typeof(String  ));
			dt.Columns.Add("REMINDER_TIME"       , typeof(int     ));
			dt.Columns.Add("DESCRIPTION"         , typeof(String  ));
			dt.Columns.Add("INVITEE_LIST"        , typeof(String  ));
			dt.Columns.Add("EMAIL_REMINDER_TIME" , typeof(int     ));
			dt.Columns.Add("ALL_DAY_EVENT"       , typeof(bool    ));
			dt.Columns.Add("REPEAT_TYPE"         , typeof(String  ));
			dt.Columns.Add("REPEAT_INTERVAL"     , typeof(int     ));
			dt.Columns.Add("REPEAT_DOW"          , typeof(String  ));
			dt.Columns.Add("REPEAT_UNTIL"        , typeof(DateTime));
			dt.Columns.Add("REPEAT_COUNT"        , typeof(int     ));
			dt.Columns.Add("SMS_REMINDER_TIME"   , typeof(int     ));
			
			Google.Apis.Services.BaseClientService.Initializer initializer = SplendidCRM.GoogleApps.GetUserCredentialInitializer(Application, Security.USER_ID, CalendarService.Scope.Calendar);
			CalendarService service = new CalendarService(initializer);
			
			string sCALENDAR_ID = "primary";
			string sGROUP_NAME  = Sql.ToString(Application["CONFIG.GoogleApps.GroupName"]).Trim();
			if ( Sql.IsEmptyString(sGROUP_NAME) )
				sGROUP_NAME = "SplendidCRM";
			if ( sGROUP_NAME.ToLower() != "primary" )
			{
				sCALENDAR_ID = Sql.ToString(Application["CONFIG.GoogleApps." + Security.USER_ID.ToString() + ".CalendarID"]);
				if ( Sql.IsEmptyString(sCALENDAR_ID) )
				{
					try
					{
						CalendarListResource.ListRequest reqCalendars = service.CalendarList.List();
						CalendarList calendars = reqCalendars.Execute();
						if ( calendars.Items != null && calendars.Items.Count > 0 )
						{
							foreach ( CalendarListEntry calendar in calendars.Items )
							{
								//Debug.WriteLine(calendar.Id + " " + calendar.Summary);
								if ( calendar.Summary == sGROUP_NAME )
								{
									sCALENDAR_ID = calendar.Id;
									break;
								}
							}
						}
					}
					catch(Exception ex)
					{
						throw(new Exception("Could not find calendar " + sGROUP_NAME, ex));
					}
					if ( Sql.IsEmptyString(sCALENDAR_ID) )
					{
						try
						{
							Google.Apis.Calendar.v3.Data.Calendar calSplendidCRM = new Google.Apis.Calendar.v3.Data.Calendar();
							calSplendidCRM.Summary = sGROUP_NAME;
							CalendarsResource.InsertRequest reqInsert = service.Calendars.Insert(calSplendidCRM);
							Google.Apis.Calendar.v3.Data.Calendar entSplendidCRM = reqInsert.Execute();
							sCALENDAR_ID = entSplendidCRM.Id;
						}
						catch(Exception ex)
						{
							throw(new Exception("Could not create calendar " + sGROUP_NAME, ex));
						}
					}
					Application["CONFIG.GoogleApps." + Security.USER_ID.ToString() + ".CalendarID"] = sCALENDAR_ID;
				}
			}
			
			EventsResource.ListRequest request = service.Events.List(sCALENDAR_ID);
			request.TimeMin      = DateTime.Now.AddMonths(-1);
			request.ShowDeleted  = false;
			request.SingleEvents = false;
			request.MaxResults   = 200  ;
			//request.OrderBy      = EventsResource.ListRequest.OrderByEnum.StartTime;
			Events events = request.Execute();
			if ( events.Items != null && events.Items.Count > 0 )
			{
				foreach ( Event appointment in events.Items )
				{
					//EventsResource.GetRequest reqGet = service.Events.Get(sCalendarID, appointment.Id);
					//Event evt = reqGet.Execute();
					// 09/14/2015 Paul.  Appointments do not have a deleted flag.  They have cancelled and not many other fields. 
					if ( appointment.Status == "cancelled" && !appointment.Created.HasValue )
						continue;
					DataRow row = dt.NewRow();
					row["ID"              ] = appointment.Id           ;
					row["NAME"            ] = appointment.Summary      ;
					row["DATE_ENTERED"    ] = appointment.Created.Value;
					row["DATE_MODIFIED"   ] = (appointment.Updated.HasValue ? appointment.Updated.Value : appointment.Created.Value);
					row["ASSIGNED_TO_NAME"] = Security.USER_NAME;
					row["CREATED_BY_NAME" ] = Security.USER_NAME;
					row["MODIFIED_BY_NAME"] = Security.USER_NAME;
					row["MODIFIED_USER_ID"] = Security.USER_ID  ;
					row["TEAM_NAME"       ] = Security.TEAM_NAME;
					row["TEAM_ID"         ] = Security.TEAM_ID  ;
					row["TEAM_SET_NAME"   ] = Security.TEAM_NAME;
					if ( !appointment.Start.DateTime.HasValue )
					{
						row["ALL_DAY_EVENT"   ] = true;
						row["DATE_TIME"       ] = Sql.ToDateTime(appointment.Start.Date);
						TimeSpan tsDURATION = Sql.ToDateTime(appointment.End.Date) - Sql.ToDateTime(appointment.Start.Date);
						row["DURATION_MINUTES"] = 0;
						row["DURATION_HOURS"  ] = tsDURATION.TotalHours;
					}
					else
					{
						row["ALL_DAY_EVENT"   ] = false;
						row["DATE_TIME"       ] = (appointment.Start.DateTime.HasValue ? appointment.Start.DateTime.Value : DateTime.MinValue);
						if ( !appointment.End.DateTime.HasValue )
							appointment.End.DateTime = appointment.Start.DateTime;
						TimeSpan tsDURATION = appointment.End.DateTime.Value - appointment.Start.DateTime.Value;
						row["DURATION_MINUTES"] = tsDURATION.Minutes;
						row["DURATION_HOURS"  ] = tsDURATION.Hours  ;
					}
					row["DESCRIPTION"     ] = appointment.Description;
					row["LOCATION"        ] = appointment.Location   ;
					row["STATUS"          ] = appointment.Status     ;
					if ( appointment.Status == SplendidCRM.GoogleApps.EventStatus.CANCELLED )
						row["STATUS"] = "Not held";
					else if ( appointment.Status == SplendidCRM.GoogleApps.EventStatus.CONFIRMED )
						row["STATUS"] = "Confirmed";
					else if ( appointment.Status == SplendidCRM.GoogleApps.EventStatus.TENTATIVE )
						row["STATUS"] = "Planned";
					if ( appointment.Reminders != null )
					{
						if ( appointment.Reminders.Overrides != null )
						{
							foreach ( EventReminder reminder in appointment.Reminders.Overrides )
							{
								if      ( reminder.Method == "popup" && reminder.Minutes.HasValue ) row["REMINDER_TIME"      ] = reminder.Minutes.Value * 60;
								else if ( reminder.Method == "email" && reminder.Minutes.HasValue ) row["EMAIL_REMINDER_TIME"] = reminder.Minutes.Value * 60;
							}
						}
						else if ( appointment.Reminders.UseDefault.HasValue && appointment.Reminders.UseDefault.Value )
						{
							row["REMINDER_TIME"      ] = 30 * 60;
							row["EMAIL_REMINDER_TIME"] = 10 * 60;
						}
					}
					if ( appointment.Recurrence != null && appointment.Recurrence.Count > 0 )
					{
						string   sRRULE           = appointment.Recurrence[0];
						string   sREPEAT_TYPE     = String.Empty     ;
						int      nREPEAT_INTERVAL = 0                ;
						string   sREPEAT_DOW      = String.Empty     ;
						DateTime dtREPEAT_UNTIL   = DateTime.MinValue;
						int      nREPEAT_COUNT    = 0                ;
						
						try
						{
							Utils.CalDAV_ParseRule(sRRULE, ref sREPEAT_TYPE, ref nREPEAT_INTERVAL, ref sREPEAT_DOW, ref dtREPEAT_UNTIL, ref nREPEAT_COUNT);
							row["REPEAT_TYPE"    ] = sREPEAT_TYPE    ;
							row["REPEAT_INTERVAL"] = nREPEAT_INTERVAL;
							row["REPEAT_DOW"     ] = sREPEAT_DOW     ;
							row["REPEAT_UNTIL"   ] = dtREPEAT_UNTIL  ;
							row["REPEAT_COUNT"   ] = nREPEAT_COUNT   ;
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Failed to parse Google Calendar event rule " + sRRULE + ". " + ex.Message);
						}
					}
					dt.Rows.Add(row);
				}
			}
			//this.ApplyGridViewRules(m_sMODULE + "." + LayoutListView, dt);
			vwMain = dt.DefaultView;
			grdMain.DataSource = vwMain ;
			ViewState["Meetings"] = dt;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible || !SplendidCRM.GoogleApps.GoogleAppsEnabled(Application) )
				return;
			// 12/06/2018 Paul.  Skip during precompile. 
			if ( Sql.ToBoolean(Request["PrecompileOnly"]) )
				return;
			
			try
			{
				if ( !IsPostBack )
				{
					grdMain.OrderByClause("NAME", "asc");
					ctlExportHeader.Visible = true;
					ctlMassUpdate.Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
					// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
					ctlCheckAll  .Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
					
					Bind();
				}
				else
				{
					DataTable dt = ViewState["Meetings"] as DataTable;
					if ( dt != null )
					{
						vwMain = dt.DefaultView;
						grdMain.DataSource = vwMain ;
					}
				}
				if ( !IsPostBack )
				{
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This Contact is required by the ASP.NET Web Form Designer.
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
			ctlSearchView  .Command += new CommandEventHandler(Page_Command);
			ctlExportHeader.Command += new CommandEventHandler(Page_Command);
			ctlMassUpdate  .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll    .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Meetings";
			SetMenu(m_sMODULE);
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("NAME");
			this.AppendGridColumns(grdMain, m_sMODULE + "." + LayoutListView, arrSelectFields);
			if ( Security.GetUserAccess(m_sMODULE, "delete") < 0 && Security.GetUserAccess(m_sMODULE, "edit") < 0 )
				ctlMassUpdate.Visible = false;
			
			// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
			if ( SplendidDynamic.StackedLayout(Page.Theme) )
			{
				ctlModuleHeader.Command += new CommandEventHandler(Page_Command);
				ctlModuleHeader.AppendButtons(m_sMODULE + "." + LayoutListView, Guid.Empty, null);
				// 06/05/2015 Paul.  Move MassUpdate buttons to the SplendidGrid. 
				grdMain.IsMobile       = this.IsMobile;
				grdMain.MassUpdateView = m_sMODULE + ".MassUpdate";
				grdMain.Command       += new CommandEventHandler(Page_Command);
				if ( !IsPostBack )
					pnlMassUpdateSeven.Visible = false;
			}
		}
		#endregion
	}
}

