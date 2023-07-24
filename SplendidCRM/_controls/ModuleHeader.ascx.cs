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
	///		Summary description for ModuleHeader.
	/// </summary>
	public class ModuleHeader : SplendidControl
	{
		protected SplendidCRM.Themes.Sugar.ModuleHeader ctlModuleHeader;
		protected Panel     pnlHeader;
		// 01/02/2020 Paul.  Provide a way to set the module name. 
		protected string    sModuleTitle       = String.Empty;
		protected string    sModule            = String.Empty;
		protected string    sTitle             = String.Empty;
		protected string    sHelpName          = String.Empty;
		protected string    sTitleText         = String.Empty;
		protected bool      bEnableModuleLabel = true;
		protected bool      bEnablePrint       = false;
		protected bool      bEnableHelp        = false;
		// 03/31/2012 Paul.  Add support for favorites. 
		protected bool      bEnableFavorites   = false;

		// 01/02/2020 Paul.  Provide a way to set the module name. 
		public string ModuleTitle
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.ModuleTitle;
				else
					return sModuleTitle;
			}
			set
			{
				sModuleTitle = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.ModuleTitle = value;
			}
		}

		public string Module
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.Module;
				else
					return sModule;
			}
			set
			{
				sModule = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.Module = value;
			}
		}

		public string Title
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.Title;
				else
					return sTitle;
			}
			set
			{
				sTitle = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.Title = value;
			}
		}

		public string HelpName
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.HelpName;
				else
					return sHelpName;
			}
			set
			{
				sHelpName = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.HelpName = value;
			}
		}

		public string TitleText
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.TitleText;
				else
					return sTitleText;
			}
			set
			{
				sTitleText = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.TitleText = value;
			}
		}

		public bool EnableModuleLabel
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.EnableModuleLabel;
				else
					return bEnableModuleLabel;
			}
			set
			{
				bEnableModuleLabel = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.EnableModuleLabel = value;
			}
		}

		public bool EnablePrint
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.EnablePrint;
				else
					return bEnablePrint;
			}
			set
			{
				bEnablePrint = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.EnablePrint = value;
			}
		}

		public bool EnableHelp
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.EnableHelp;
				else
					return bEnableHelp;
			}
			set
			{
				bEnableHelp = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.EnableHelp = value;
			}
		}

		// 03/31/2012 Paul.  Add support for favorites. 
		public bool EnableFavorites
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.EnableFavorites;
				else
					return bEnableFavorites;
			}
			set
			{
				bEnableFavorites = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.EnableFavorites = value;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
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
			string sTheme = Page.Theme;
			// 10/16/2015 Paul.  Change default theme to our newest theme. 
			if ( String.IsNullOrEmpty(sTheme) )
				sTheme = SplendidDefaults.Theme();
			string sModuleHeaderPath = "~/App_MasterPages/" + Page.Theme + "/ModuleHeader.ascx";
			// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
			if ( Utils.CachedFileExists(Context, sModuleHeaderPath) )
			{
				ctlModuleHeader = LoadControl(sModuleHeaderPath) as SplendidCRM.Themes.Sugar.ModuleHeader;
				if ( ctlModuleHeader != null )
				{
					ctlModuleHeader.Module            = sModule           ;
					// 01/02/2020 Paul.  Provide a way to set the module name. 
					ctlModuleHeader.ModuleTitle       = sModuleTitle      ;
					ctlModuleHeader.Title             = sTitle            ;
					ctlModuleHeader.HelpName          = sHelpName         ;
					ctlModuleHeader.TitleText         = sTitleText        ;
					ctlModuleHeader.EnableModuleLabel = bEnableModuleLabel;
					ctlModuleHeader.EnablePrint       = bEnablePrint      ;
					ctlModuleHeader.EnableHelp        = bEnableHelp       ;
					ctlModuleHeader.EnableFavorites   = bEnableFavorites  ;
					pnlHeader.Controls.Add(ctlModuleHeader);
				}
			}
		}
		#endregion
	}
}

