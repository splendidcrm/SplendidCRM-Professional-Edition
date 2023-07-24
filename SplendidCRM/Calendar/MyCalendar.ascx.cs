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

namespace SplendidCRM.Calendar
{
	/// <summary>
	///		Summary description for MyCalendar.
	/// </summary>
	public class MyCalendar : DashletControl
	{
		protected System.Web.UI.WebControls.Calendar ctlCalendar;

		protected void ctlCalendar_SelectionChanged(Object sender, EventArgs e) 
		{
			// 08/31/2006 Paul.  The date needs to be separated into day, month, year fields to avoid localization issues. 
			Response.Redirect("~/Calendar/default.aspx?" + CalendarControl.CalendarQueryString(ctlCalendar.SelectedDate));
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 09/09/2007 Paul.  We are having trouble dynamically adding user controls to the WebPartZone. 
			// Instead, control visibility manually here.  This approach as the added benefit of hiding the 
			// control even if the WebPartManager has moved it to an alternate zone. 
			// 07/10/2009 Paul.  The end-user will be able to hide or show the Dashlet controls. 
			/*
			if ( this.Visible && !Sql.IsEmptyString(sDetailView) )
			{
				// 01/17/2008 Paul.  We need to use the sDetailView property and not the hard-coded view name. 
				DataView vwFields = new DataView(SplendidCache.DetailViewRelationships(sDetailView));
				vwFields.RowFilter = "CONTROL_NAME = '~/Calendar/MyCalendar'";
				this.Visible = vwFields.Count > 0;
			}
			*/
			if ( !this.Visible )
				return;

			ctlCalendar.NextPrevFormat = NextPrevFormat.CustomText ;
			ctlCalendar.PrevMonthText  = "<div class=\"monthFooterPrev\"><img src=\"" + Session["themeURL"] + "images/calendar_previous.gif\" width=\"6\" height=\"9\" alt=\"" + L10n.Term("Calendar.LBL_PREVIOUS_MONTH") + "\" align=\"absmiddle\" border=\"0\">&nbsp;&nbsp;" + L10n.Term("Calendar.LBL_PREVIOUS_MONTH").Replace(" ", "&nbsp;") + "</div>";
			ctlCalendar.NextMonthText  = "<div class=\"monthFooterNext\">" + L10n.Term("Calendar.LBL_NEXT_MONTH").Replace(" ", "&nbsp;") + "&nbsp;&nbsp;<img src=\"" + Session["themeURL"] + "images/calendar_next.gif\" width=\"6\" height=\"9\" alt=\"" + L10n.Term("Calendar.LBL_NEXT_MONTH") + "\" align=\"absmiddle\" border=\"0\"></div>";
			if ( Information.IsDate(Request["Date"]) )
			{
				ctlCalendar.VisibleDate  = Sql.ToDateTime(Request["Date"]);
				ctlCalendar.SelectedDate = Sql.ToDateTime(Request["Date"]);
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

