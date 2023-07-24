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

namespace SplendidCRM.Home
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : SplendidPage
	{
		protected Label       lblUpgradeWarning;
		protected PlaceHolder plcSubPanelLeft  ;
		protected PlaceHolder plcSubPanelBody  ;
		protected PlaceHolder plcSubPanelRight ;
		protected HyperLink   lnkHelpImage     ;
		protected HyperLink   lnkHelpText      ;
		// 04/23/2016 Paul.  How dashlets to be disabled. 
		protected Button      btnAddDashlets   ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "AddDashlets" )
			{
				Response.Redirect("AddDashlets.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".LBL_BROWSER_TITLE"));
			if ( !IsPostBack )
			{
				// 04/23/2016 Paul.  How dashlets to be disabled. 
				btnAddDashlets.Visible = !Sql.ToBoolean(Application["CONFIG.disable_add_dashlets"]);
				// 10/07/2010 Paul.  Add Help Wiki to the home page. 
				if ( lnkHelpImage != null )
				{
					// 01/26/2011 Paul.  Don't show the Help Wiki on a mobile device. 
					lnkHelpImage.Visible     = !this.IsMobile;
					lnkHelpImage.NavigateUrl = "~/Help/view.aspx?MODULE=Home&NAME=index";
				}
				if ( lnkHelpText != null )
				{
					// 01/26/2011 Paul.  Don't show the Help Wiki on a mobile device. 
					lnkHelpText.Visible      = !this.IsMobile;
					// 02/07/2010 Paul.  Defensive programming, build URL just in case lnkHelpImage is null. 
					lnkHelpText.NavigateUrl = "~/Help/view.aspx?MODULE=Home&NAME=index";
					// 10/25/2006 Paul.  There is a config flag to disable the wiki entirely. 
					if ( (SplendidCRM.Security.GetUserAccess("Help", "edit") >= 0) && Sql.ToBoolean(Application["CONFIG.enable_help_wiki"]) )
						lnkHelpText.Text = L10n.Term(".LNK_HELP_WIKI");
					else
						lnkHelpText.Text = L10n.Term(".LNK_HELP");
				}
				// 01/15/2008 Paul.  Move the Upgrade warning to an exclusive location on the home page. 
				// We want to reduce the performance hit to a single page. 
				if ( Security.IS_ADMIN )
				{
					try
					{
						string sAvailableVersion = Sql.ToString(Application["available_version"]);
						if ( !Sql.IsEmptyString(sAvailableVersion) )
						{
							// 04/21/2009 Paul.  Show the current version and the available version. 
							string  sSplendidVersion  = Sql.ToString(Application["SplendidVersion"]);
							Version vSplendidVersion  = new Version(sSplendidVersion);
							Version vAvailableVersion = new Version(sAvailableVersion);
							if ( vSplendidVersion < vAvailableVersion )
							{
								lblUpgradeWarning.Text = L10n.Term("Administration.WARN_UPGRADE") + sSplendidVersion + ": " + Sql.ToString(Application["available_version_description"]);
								lblUpgradeWarning.Visible = true;
							}
						}
					}
					catch
					{
					}
				}
				// 09/09/2007 Paul.  We are having trouble dynamically adding user controls to the WebPartZone. 
				// Instead, control visibility in the user control.  This approach as the added benefit of hiding the 
				// control even if the WebPartManager has moved it to an alternate zone. 
				// This approach is far from idea as it leaves web part frames in tact. 
				/*
				WebPartManager mgrWebPart = Page.Master.FindControl("mgrWebPart") as WebPartManager;
				Control ctl = LoadControl("~/Leads/MyLeads.ascx");
				GenericWebPart part = mgrWebPart.CreateWebPart(ctl);
				mgrWebPart.AddWebPart(part, zBody, 0);
				*/

				// 02/11/2007 Paul.  Only show team notices if team management enabled. 
				// 01/17/2008 Paul.  The MyTeamNotices control will hide itself. 
				// ctlMyTeamNotices.Visible = Crm.Config.enable_team_management();
				// 01/22/2007 Paul.  The ZoneTemplate does not seem to allow data binding to the title, so do it manually. 
				// 01/11/2008 Paul.  Remove the side bar. 
				/*
				foreach ( WebPart part in zSidebar.WebParts )
				{
					part.Title = L10n.Term(part.Title);
				}
				*/
				// 01/22/2007 Paul.  The ZoneTemplate does not seem to allow data binding to the title, so do it manually. 
				// 01/17/2008 Paul.  We are replacing WebParts with a DetailView relationship. 
				/*
				foreach ( WebPart part in zBody.WebParts )
				{
					part.Title = L10n.Term(part.Title);
				}
				// 01/22/2007 Paul.  The ZoneTemplate does not seem to allow data binding to the title, so do it manually. 
				foreach ( WebPart part in zRightbar.WebParts )
				{
					part.Title = L10n.Term(part.Title);
				}
				*/
				// 06/09/2006 Paul.  The primary data binding will now only occur in the ASPX pages so that this is only one per cycle. 
				// 03/11/2008 Paul.  Move the primary binding to SplendidPage. 
				//Page DataBind();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			SetMenu("Home");
			this.Load += new System.EventHandler(this.Page_Load);
			// 01/20/2007 Paul.  Use WebParts to display the relationships. 
			// 01/17/2008 Paul.  We are replacing WebParts with a DetailView relationship. 
			// 07/10/2009 Paul.  We are now allowing relationships to be user-specific. 
			this.AppendDetailViewRelationships("Home.DetailView.Left" , plcSubPanelLeft , Security.USER_ID);
			this.AppendDetailViewRelationships("Home.DetailView.Body" , plcSubPanelBody , Security.USER_ID);
			this.AppendDetailViewRelationships("Home.DetailView.Right", plcSubPanelRight, Security.USER_ID);
		}
		#endregion
	}
}

