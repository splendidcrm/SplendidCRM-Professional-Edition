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
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SplendidCRM.Themes.Mobile
{
	// 12/22/2015 Paul.  All master pages should inherit SplendidMaster. 
	public partial class DefaultView : SplendidMaster
	{
		protected L10N         L10n;

		protected bool          bDebug       = false;
		// 04/19/2013 Paul.  LinkButton is not working on Windows Phone 8.  Not sure why, but changing to Button solves the problem.
		protected Button        lnkFullSite;

		public L10N GetL10n()
		{
			// 08/30/2005 Paul.  Attempt to get the L10n & T10n objects from the parent page. 
			// If that fails, then just create them because they are required. 
			if ( L10n == null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				// A port to DNN prompted this approach. 
				L10n = Context.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					string sCULTURE  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
					L10n = new L10N(sCULTURE);
				}
			}
			return L10n;
		}

		protected override void OnInit(EventArgs e)
		{
			GetL10n();
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}

		// 12/22/2015 Paul.  All master pages should inherit SplendidMaster. 
		public override void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "FullSite" )
			{
				// 11/30/2012 Paul.  Save the default them for the user, as specified in the preferences. 
				// This is to allow the user to go from the Mobile theme to the full site. 
				string sTheme = Sql.ToString(Session["USER_SETTINGS/DEFAULT_THEME"]);
				if ( String.IsNullOrEmpty(sTheme) )
					sTheme = SplendidDefaults.Theme();
				string sApplicationPath = Sql.ToString(HttpContext.Current.Application["rootURL"]);
				HttpContext.Current.Session["USER_SETTINGS/THEME"] = sTheme;
				HttpContext.Current.Session["themeURL"           ] = sApplicationPath + "App_Themes/" + sTheme + "/";
				Response.Redirect(Request.RawUrl);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
#if DEBUG
			bDebug = true;
#endif
			if ( !IsPostBack )
			{
				try
				{
					// http://www.i18nguy.com/temp/rtl.html
					// 09/04/2013 Paul.  ASP.NET 4.5 is enforcing a rule that the root be an HtmlElement and not an HtmlGenericControl. 
					HtmlContainerControl htmlRoot = FindControl("htmlRoot") as HtmlContainerControl;
					if ( htmlRoot != null )
					{
						if ( L10n.IsLanguageRTL() )
						{
							htmlRoot.Attributes.Add("dir", "rtl");
						}
					}
				}
				catch
				{
				}
			}
		}
	}
}

