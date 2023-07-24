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
	/// Summary description for AddDashlets.
	/// </summary>
	public class AddDashlets : SplendidPage
	{
		// 03/06/2014 Paul.  Allow AddDashlets to be used by other modules. 
		protected ArrangeDashlets ctlDashletsBody ;
		protected ArrangeDashlets ctlDashletsRight;
		protected string sModule;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "CloseDashlets" )
			{
				// 03/06/2014 Paul.  Allow AddDashlets to be used by other modules. 
				if ( !Sql.IsEmptyString(sModule) && Sql.ToBoolean(Application["Modules." + sModule + ".Valid"]) )
					Response.Redirect("~/" + sModule + "/");
				else
					// 06/15/2017 Paul.  Add support for HTML5 Home Page. 
					Response.Redirect(Sql.ToString(Application["Modules.Home.RelativePath"]));
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".LBL_BROWSER_TITLE"));
			if ( !IsPostBack )
			{
				// 03/06/2014 Paul.  Allow AddDashlets to be used by other modules. 
				if ( !Sql.IsEmptyString(sModule) && Sql.ToBoolean(Application["Modules." + sModule + ".Valid"]) )
					ctlDashletsRight.Visible = false;
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
			sModule = Sql.ToString(Request["Module"]);
			// 03/06/2014 Paul.  Allow AddDashlets to be used by other modules. 
			if ( !Sql.IsEmptyString(sModule) && Sql.ToBoolean(Application["Modules." + sModule + ".Valid"]) )
				ctlDashletsBody.DetailView = sModule + ".Dashboard";
		}
		#endregion
	}
}

