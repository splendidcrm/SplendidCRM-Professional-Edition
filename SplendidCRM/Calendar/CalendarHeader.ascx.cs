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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SplendidCRM.Calendar
{
	/// <summary>
	///		Summary description for CalendarHeader.
	/// </summary>
	public class CalendarHeader : SplendidControl
	{
		public    CommandEventHandler Command ;
		
		protected Button btnDay    ;
		protected Button btnWeek   ;
		protected Button btnMonth  ;
		protected Button btnYear   ;
		protected Button btnShared ;
		protected string sActiveTab;

		public string ActiveTab
		{
			get
			{
				return sActiveTab;
			}
			set
			{
				sActiveTab = value;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e) ;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/16/2007 Paul.  If calls are not visible on only visible to owners, then hide the Shared button. 
			btnShared.Visible = (SplendidCRM.Security.GetUserAccess("Calls", "list") >= ACL_ACCESS.OWNER);
			switch(sActiveTab)
			{
				case "Day"   :  btnDay   .CssClass = "buttonOn" ;  break;
				case "Week"  :  btnWeek  .CssClass = "buttonOn" ;  break;
				case "Month" :  btnMonth .CssClass = "buttonOn" ;  break;
				case "Year"  :  btnYear  .CssClass = "buttonOn" ;  break;
				case "Shared":  btnShared.CssClass = "buttonOn" ;  break;
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

