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
using System.Collections;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for ImportDefaultsView.
	/// </summary>
	public class ImportDefaultsView : SplendidControl
	{
		protected Guid            gID                             ;
		protected HtmlTable       tblTeam                         ;
		protected HtmlTable       tblMain                         ;
		protected HtmlTable       tblAddress                      ;

		protected DropDownList    THEME                           ;
		protected DropDownList    LANGUAGE                        ;
		protected DropDownList    DATE_FORMAT                     ;
		protected DropDownList    TIME_FORMAT                     ;
		protected DropDownList    TIMEZONE_ID                     ;
		protected DropDownList    CURRENCY_ID                     ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
		}

		protected void lstLANGUAGE_Changed(Object sender, EventArgs e)
		{
			if ( LANGUAGE.SelectedValue.Length > 0 )
			{
				CultureInfo oldCulture   = Thread.CurrentThread.CurrentCulture   ;
				CultureInfo oldUICulture = Thread.CurrentThread.CurrentUICulture ;
				Thread.CurrentThread.CurrentCulture   = CultureInfo.CreateSpecificCulture(LANGUAGE.SelectedValue);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(LANGUAGE.SelectedValue);

				DateTime dtNow = T10n.FromServerTime(DateTime.Now);
				DateTimeFormatInfo oDateInfo   = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
				NumberFormatInfo   oNumberInfo = Thread.CurrentThread.CurrentCulture.NumberFormat  ;

				String[] aDateTimePatterns = oDateInfo.GetAllDateTimePatterns();

				DATE_FORMAT.Items.Clear();
				TIME_FORMAT.Items.Clear();
				foreach ( string sPattern in aDateTimePatterns )
				{
					// 11/12/2005 Paul.  Only allow patterns that have a full year. 
					// 10/15/2013 Paul.  Allow 2-digit year. 
					if ( sPattern.IndexOf("yy") >= 0 && sPattern.IndexOf("dd") >= 0 && sPattern.IndexOf("mm") <  0 )
						DATE_FORMAT.Items.Add(new ListItem(sPattern + "   " + dtNow.ToString(sPattern), sPattern));
					if ( sPattern.IndexOf("yy") <  0 && sPattern.IndexOf("mm") >= 0 )
						TIME_FORMAT.Items.Add(new ListItem(sPattern + "   " + dtNow.ToString(sPattern), sPattern));
				}
				Thread.CurrentThread.CurrentCulture = oldCulture  ;
				Thread.CurrentThread.CurrentCulture = oldUICulture;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				TIMEZONE_ID.DataSource = SplendidCache.TimezonesListbox();
				TIMEZONE_ID.DataBind();
				CURRENCY_ID.DataSource = SplendidCache.Currencies();
				CURRENCY_ID.DataBind();
				LANGUAGE   .DataSource = SplendidCache.Languages();
				LANGUAGE   .DataBind();
				THEME      .DataSource = SplendidCache.Themes();
				THEME      .DataBind();

				try
				{
					// 08/19/2010 Paul.  Check the list before assigning the value. 
					Utils.SetSelectedValue(THEME, SplendidDefaults.Theme());
				}
				catch(Exception ex)
				{
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
				}
				try
				{
					string sDefaultLanguage = Sql.ToString(Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"]);
					if ( Sql.IsEmptyString(sDefaultLanguage) )
						sDefaultLanguage = "en-US";
					// 08/19/2010 Paul.  Check the list before assigning the value. 
					Utils.SetSelectedValue(LANGUAGE, sDefaultLanguage);
				}
				catch(Exception ex)
				{
					SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
				}
				lstLANGUAGE_Changed(null, null);
				new DynamicControl(this, "TEAM_ID"  ).Text = String.Empty;
				new DynamicControl(this, "TEAM_NAME").Text = String.Empty;
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
			this.Load += new System.EventHandler(this.Page_Load);
			m_sMODULE = "Users";
			this.AppendEditViewFields(m_sMODULE + ".TeamView"       , tblTeam       , null);
			this.AppendEditViewFields(m_sMODULE + ".EditView"       , tblMain       , null);
			this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
		}
		#endregion
	}
}
