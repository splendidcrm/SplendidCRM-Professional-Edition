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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SplendidCRM.Themes.Sugar
{
	/// <summary>
	///		Summary description for TabMenu.
	/// </summary>
	public class TabMenu : SplendidControl
	{
		// 10/20/2010 Paul.  Restore sActiveTab in order to avoid the old Version 2.1 Sugar2006 them from crashing. 
		protected string sActiveTab      ;
		protected string sApplicationPath;
		protected DataTable   dtMenu;
		protected PlaceHolder phHover;
		protected AjaxControlToolkit.HoverMenuExtender hovMore;

		private void Page_Load(object sender, System.EventArgs e)
		{
			sApplicationPath = Request.ApplicationPath;
			if ( !sApplicationPath.EndsWith("/") )
				sApplicationPath += "/";
			// 09/12/2010 Paul.  Need to use the Portal menu. 
			// 10/20/2015 Paul.  Share code with Portal. 
			dtMenu = PortalCache.IsPortal() ? PortalCache.TabMenu() : SplendidCache.TabMenu();
			// 04/28/2006 Paul.  Hide the tab menu if there is no menu to display. 
			// This should only occur during login. 
			// 02/25/2010 Paul.  This control is not visible if group tabs is enabled. 
			// 02/26/2010 Paul.  The SubPanel Tabs flag has been moved to the Session so that it would be per-user. 
			bool bGroupTabs = Sql.ToBoolean(Session["USER_SETTINGS/GROUP_TABS"]);
			if ( dtMenu.Rows.Count == 0 || bGroupTabs )
				this.Visible = false;
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
			
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			// 11/23/2009 Paul.  SplendidCRM 4.0 is very slow on Blackberry devices.  Lets try and turn off AJAX AutoComplete. 
			bool bAjaxAutoComplete = (mgrAjax != null);
			if ( this.IsMobile )
			{
				// 11/24/2010 Paul.  .NET 4 has broken the compatibility of the browser file system. 
				// We are going to minimize our reliance on browser files in order to reduce deployment issues. 
				bAjaxAutoComplete = Utils.AllowAutoComplete && (mgrAjax != null);
			}
			if ( bAjaxAutoComplete && mgrAjax != null )
			{
				// 10/20/2010 Paul.  phHover will not exist on the old Sugar2006 theme. 
				if ( phHover != null )
				{
					// <ajaxToolkit:HoverMenuExtender TargetControlID="imgTabMenuMore" PopupControlID="pnlTabMenuMore" PopupPosition="Bottom" PopDelay="50" OffsetX="-12" OffsetY="-3" runat="server" />
					hovMore = new AjaxControlToolkit.HoverMenuExtender();
					hovMore.TargetControlID = "imgTabMenuMore";
					hovMore.PopupControlID  = "pnlTabMenuMore";
					hovMore.PopupPosition   = AjaxControlToolkit.HoverMenuPopupPosition.Bottom;
					hovMore.PopDelay        =  50;
					hovMore.OffsetX         = -12;
					hovMore.OffsetY         =  -3;
					phHover.Controls.Add(hovMore);
				}
			}
		}
		#endregion
	}
}

