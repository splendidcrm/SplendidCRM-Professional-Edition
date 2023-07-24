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
	///		Summary description for Shortcuts.
	/// </summary>
	public class Shortcuts : SplendidControl
	{
		protected SplendidCRM.Themes.Sugar.Shortcuts ctlShortcuts;
		protected Panel  pnlShortcuts;
		protected string sSubMenu;

		public string SubMenu
		{
			get
			{
				if ( ctlShortcuts != null )
					return ctlShortcuts.SubMenu;
				else
					return sSubMenu;
			}
			set
			{
				sSubMenu = value;
				if ( ctlShortcuts != null )
					ctlShortcuts.SubMenu = value;
			}
		}

		public bool AdminShortcuts
		{
			get
			{
				// 01/20/2007 Paul.  IsAdminPage is now a property of the SplendidPage. 
				SplendidPage oPage = Page as SplendidPage;
				if ( oPage != null )
					return oPage.IsAdminPage;
				return false;
			}
			set
			{
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
			string sShortcutsPath = "~/App_MasterPages/" + Page.Theme + "/Shortcuts.ascx";
			// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
			if ( Utils.CachedFileExists(Context, sShortcutsPath) )
			{
				ctlShortcuts = LoadControl(sShortcutsPath) as SplendidCRM.Themes.Sugar.Shortcuts;
				if ( ctlShortcuts != null )
				{
					ctlShortcuts.SubMenu = sSubMenu;
					pnlShortcuts.Controls.Add(ctlShortcuts);
				}
			}
		}
		#endregion
	}
}

