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
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for CalendarControl.
	/// </summary>
	public class CalendarControl : SplendidControl
	{
		public static string SqlDateTimeFormat = "yyyy/MM/dd HH:mm:ss";

// 10/30/2021 Paul.  Convert CalendarControl to simple class as the control has been removed.
#if !ReactOnlyUI
		protected DateTime dtCurrentDate  = DateTime.MinValue;

		public static string CalendarQueryString(DateTime dt)
		{
			return "day=" + dt.Day + "&month=" + dt.Month + "&year=" + dt.Year;
		}

		// 09/30/2005 Paul.  Can't initialize in OnInit because ViewState is not ready. 
		// Can't initialize in a Page_Load because it will not get called in the correct sequence. 
		protected void CalendarInitDate()
		{
			if ( !IsPostBack )
			{
				int nYear  = Sql.ToInteger(Request["year" ]);
				int nMonth = Sql.ToInteger(Request["month"]);
				int nDay   = Sql.ToInteger(Request["day"  ]);
				try
				{
					if ( nYear < 1753 || nYear > 9999 || nMonth < 1 || nMonth > 12 || nDay < 1 || nDay > 31 )
						dtCurrentDate = DateTime.Today;
					else
						dtCurrentDate = new DateTime(nYear, nMonth, nDay);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					dtCurrentDate = DateTime.Today;
				}
				// 09/30/2005 Paul.  ViewState is not available in OnInit.  Must wait for the Page_Load event. 
				ViewState["CurrentDate"] = dtCurrentDate;
			}
			else
			{
				dtCurrentDate = Sql.ToDateTime(ViewState["CurrentDate"]);
			}
		}
#endif
	}
}

