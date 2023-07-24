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
using System.Diagnostics;

namespace SplendidCRM.Themes.Atlantic
{
	// 12/22/2015 Paul.  All master pages should inherit SplendidMaster. 
	public partial class DefaultView : SplendidMaster
	{
		protected L10N         L10n;

		protected HtmlTable          tblLoginHeader;
		protected System.Web.UI.WebControls.Image imgCompanyLogo;
		// 10/02/2016 Paul.  Add support for Arctic theme. 
		protected TableCell     tdShortcuts      ;
		protected bool          bShowLeftCol = true;
		protected Image         imgShowHandle    ;
		protected Image         imgHideHandle    ;
		
		public bool PrintView
		{
			get
			{
				bool bPrintView = Sql.ToBoolean(Context.Items["PrintView"]);
				return bPrintView;
			}
		}
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

		// 10/21/2016 Paul.  Move logic to manage Arctic left panel to method. 
		public virtual bool IsFullScreenPage()
		{
			string sActiveTabMenu = Sql.ToString(Page.Items["ActiveTabMenu"]);
			bool bIsAdmin = Sql.ToBoolean(Application["Modules." + sActiveTabMenu + ".IsAdmin"]);
			// 10/31/2016 Paul.  Login screen has blank menu. 
			bool bIsFullScreenPage = (bIsAdmin || sActiveTabMenu == "" || sActiveTabMenu == "Home" || sActiveTabMenu == "Administration" || sActiveTabMenu == "Dashboard" || sActiveTabMenu == "Calendar" || sActiveTabMenu == "ChatDashboard" || sActiveTabMenu == "Reports" || sActiveTabMenu == "ReportDesigner" || sActiveTabMenu == "Processes");
			return bIsFullScreenPage;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 10/02/2016 Paul.  Add support for Arctic theme. 
			if ( imgHideHandle != null || imgShowHandle != null || tdShortcuts != null )
			{
				if ( Request.Cookies["showLeftCol"] != null )
				{
					bShowLeftCol = Sql.ToBoolean(Request.Cookies["showLeftCol"].Value);
				}
				else
				{
					HttpCookie cShowLeftCol = new HttpCookie("showLeftCol", bShowLeftCol ? "true" : "false");
					cShowLeftCol.Expires = DateTime.Now.AddDays(30);
					cShowLeftCol.Path    = "/";
					Response.Cookies.Add(cShowLeftCol);
				}
			}
			// 10/02/2016 Paul.  The objects may not exist, so catch any errors. 
			try
			{
				if ( imgHideHandle != null )
				{
					imgHideHandle.Style.Remove("display");
					imgHideHandle.Style.Add("display",  bShowLeftCol ? "table-cell" : "none");
				}
				if ( imgShowHandle != null )
				{
					imgShowHandle.Style.Remove("display");
					imgShowHandle.Style.Add("display", !bShowLeftCol ? "table-cell" : "none");
				}
				if ( tdShortcuts != null )
				{
					tdShortcuts  .Style.Remove("display");
					tdShortcuts  .Style.Add("display",  bShowLeftCol ? "table-cell" : "none");
				}
			}
			catch
			{
			}
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
					if ( !Security.IsAuthenticated() )
					{
						if ( !Sql.IsEmptyString(Application["CONFIG.header_logo_image"]) )
						{
							// 02/23/2009 Paul.  Allow the logo to be any URL. 
							string sImageUrl = Sql.ToString(Application["CONFIG.header_logo_image"]);
							if ( sImageUrl.StartsWith("http", true, System.Threading.Thread.CurrentThread.CurrentCulture) )
								imgCompanyLogo.ImageUrl = sImageUrl;
							// 08/09/2009 Paul.  Allow the image to be relative to the application. 
							else if ( sImageUrl.StartsWith("~/") )
								imgCompanyLogo.ImageUrl = sImageUrl;
							else
								imgCompanyLogo.ImageUrl = "~/Include/images/" + sImageUrl;
							
							if ( Sql.ToInteger(Application["CONFIG.header_logo_width"]) > 0 )
								imgCompanyLogo.Width    = Sql.ToInteger(Application["CONFIG.header_logo_width" ]);
							if ( Sql.ToInteger(Application["CONFIG.header_logo_height"]) > 0 )
								imgCompanyLogo.Height   = Sql.ToInteger(Application["CONFIG.header_logo_height"]);
							// 12/31/2017 Paul.  Let Arctic theme have its own style. 
							// 02/17/2020 Paul.  Conditional did not use the correct value. 
							if ( !Sql.IsEmptyString(Application["CONFIG.arctic_header_logo_style"]) )
								imgCompanyLogo.Attributes.Add("style", Sql.ToString(Application["CONFIG.arctic_header_logo_style"]));
							// 11/27/2008 Paul.  Company logo is a config value, not a term. 
							// 07/07/2010 Paul.  Fix company name.  The logo is a URL. 
							// 08/18/2010 Paul.  IE8 does not support alt any more, so we need to use ToolTip instead. 
							imgCompanyLogo.ToolTip = Sql.ToString(Application["CONFIG.company_name"]);
						}
					}
				}
				catch
				{
				}
			}
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			// 08/30/2013 Paul.  Move jQuery registration to Utils class. 
			Utils.RegisterJQuery(Page, mgrAjax);
			// 08/25/2013 Paul.  Register Asterisk and Signal-R scripts. 
			// 09/20/2013 Paul.  Move EXTENSION to the main table. 
			if ( !Sql.IsEmptyString(Session["EXTENSION"]) )
			{
				AsteriskManager.RegisterScripts(Context, mgrAjax);
				// 12/03/2013 Paul.  Add support for Avaya. 
				AvayaManager.RegisterScripts(Context, mgrAjax);
			}
			// 09/27/2013 Paul.  SMS messages need to be opt-in. 
			if ( Sql.ToString(Session["SMS_OPT_IN"]) == "yes" )
				TwilioManager.RegisterScripts(Context, mgrAjax);
			// 09/09/2020 Paul.  Add PhoneBurner SignalR support. 
			PhoneBurnerManager.RegisterScripts(Context, mgrAjax);
		}

		#region Web Form Designer generated code
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			GetL10n();
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}
		#endregion
	}
}

