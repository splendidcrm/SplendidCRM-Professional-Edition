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

namespace SplendidCRM.Surveys
{
	/// <summary>
	/// Summary description for Preview.
	/// </summary>
	public class Preview : SplendidPage
	{
		protected Guid   gID;
		protected string m_sMODULE;
		
		public Preview()
		{
			this.PreInit += new EventHandler(Preview_PreInit);
		}

		protected void Preview_PreInit(object sender, EventArgs e)
		{
			this.Theme = "";
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;
			
			gID = Sql.ToGuid(Request["ID"]);
			
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			// 08/25/2013 Paul.  jQuery now registered in the master pages, except for Surveys since they do not use the master pages. 
			// 09/07/2013 Paul.  The jQuery UI stylesheet will now be manually embedded in the page. 
			//HtmlLink cssJQuery = new HtmlLink();
			//cssJQuery.Attributes.Add("href" , "~/Include/javascript/jquery-ui-1.9.1.custom.css");
			//cssJQuery.Attributes.Add("type" , "text/css"  );
			//cssJQuery.Attributes.Add("rel"  , "stylesheet");
			//Page.Header.Controls.Add(cssJQuery);
			
			// 08/28/2013 Paul.  json2.js now registered in the master pages, except for Surveys since they do not use the master pages. 
			ScriptReference scrJQuery         = new ScriptReference ("~/Include/javascript/jquery-1.9.1.min.js"      );
			ScriptReference scrJQueryUI       = new ScriptReference ("~/Include/javascript/jquery-ui-1.9.1.custom.js");
			ScriptReference scrJSON2          = new ScriptReference ("~/Include/javascript/json2.min.js"             );
			if ( !mgrAjax.Scripts.Contains(scrJQuery  ) ) mgrAjax.Scripts.Add(scrJQuery        );
			if ( !mgrAjax.Scripts.Contains(scrJQueryUI) ) mgrAjax.Scripts.Add(scrJQueryUI      );
			if ( !mgrAjax.Scripts.Contains(scrJSON2   ) ) mgrAjax.Scripts.Add(scrJSON2         );
			
			// 06/11/2013 Paul.  Register all the Survey JavaScript files. 
			SurveyUtil.RegisterScripts(this.Page);
			
			// 06/15/2013 Paul.  Start with the default theme so that the page does not look ugly while loading the theme. 
			Guid gSURVEY_THEME_ID = Sql.ToGuid(Application["CONFIG.Surveys.DefaultTheme"]);
			HtmlLink cssSurveyStylesheet = new HtmlLink();
			cssSurveyStylesheet.Attributes.Add("id"   , gSURVEY_THEME_ID.ToString().Replace("-", "_"));
			cssSurveyStylesheet.Attributes.Add("href" , "~/Surveys/stylesheet.aspx?ID=" + gSURVEY_THEME_ID.ToString());
			cssSurveyStylesheet.Attributes.Add("type" , "text/css"  );
			cssSurveyStylesheet.Attributes.Add("rel"  , "stylesheet");
			Page.Header.Controls.Add(cssSurveyStylesheet);
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
			this.Load += new System.EventHandler(this.Page_Load);
			m_sMODULE = "Surveys";
		}
		#endregion
	}
}

