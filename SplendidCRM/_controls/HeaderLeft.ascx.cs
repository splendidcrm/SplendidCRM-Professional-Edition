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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for HeaderLeft.
	/// </summary>
	public class HeaderLeft : SplendidControl
	{
		protected SplendidCRM.Themes.Sugar.HeaderLeft ctlHeaderLeft;
		protected Panel  pnlHeader;
		protected string sTitle;
		protected Unit   uWidth;

		public string Title
		{
			get
			{
				if ( ctlHeaderLeft != null )
					return ctlHeaderLeft.Title;
				else
					return sTitle;
			}
			set
			{
				sTitle = value;
				if ( ctlHeaderLeft != null )
					ctlHeaderLeft.Title = value;
			}
		}

		public Unit Width
		{
			get
			{
				if ( ctlHeaderLeft != null )
					return ctlHeaderLeft.Width;
				else
					return uWidth;
			}
			set
			{
				uWidth = value;
				if ( ctlHeaderLeft != null )
					ctlHeaderLeft.Width = value;
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
			string sTheme = Page.Theme;
			// 10/16/2015 Paul.  Change default theme to our newest theme. 
			if ( String.IsNullOrEmpty(sTheme) )
				sTheme = SplendidDefaults.Theme();
			string sHeaderLeftPath = "~/App_MasterPages/" + Page.Theme + "/HeaderLeft.ascx";
			// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
			if ( Utils.CachedFileExists(Context, sHeaderLeftPath) )
			{
				ctlHeaderLeft = LoadControl(sHeaderLeftPath) as SplendidCRM.Themes.Sugar.HeaderLeft;
				if ( ctlHeaderLeft != null )
				{
					ctlHeaderLeft.Title = sTitle;
					if ( !uWidth.IsEmpty )
						ctlHeaderLeft.Width = uWidth;
					pnlHeader.Controls.Add(ctlHeaderLeft);
				}
			}
		}
		#endregion
	}
}

