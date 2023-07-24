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
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for RestUtils.
	/// </summary>
	public class RestUtils : SplendidControl
	{
		protected DataTable dtModules;

		// 05/09/2016 Paul.  Move AddScriptReference and AddStyleSheet to Sql object. 
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 03/19/2016 Paul.  Move Ajax registration ot RestUtils.ascx. 
			AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(Page) as AjaxControlToolkit.ToolkitScriptManager;
			// 01/28/2018 Paul.  We need to paginate the popup to support large data sets. 
			Sql.AddScriptReference(mgrAjax, "~/html5/jQuery/jquery.paging.min.js");
			// 05/09/2016 Paul.  Move javascript objects to separate file. 
			Sql.AddScriptReference(mgrAjax, "~/include/javascript/RestUtils.js");
			Sql.AddScriptReference(mgrAjax, "~/html5/SplendidUI/Formatting.js");
			Sql.AddScriptReference(mgrAjax, "~/html5/SplendidUI/Sql.js"       );
			// 01/28/2018 Paul.  SearchBuilder is needed for parent popup. 
			Sql.AddScriptReference(mgrAjax, "~/html5/SplendidUI/SearchBuilder.js");
			
			// 01/24/2018 Paul.  The Calendar needs to determine if Calls module is enabled. 
			dtModules = SplendidCache.AccessibleModulesTable(HttpContext.Current, Security.USER_ID);
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

