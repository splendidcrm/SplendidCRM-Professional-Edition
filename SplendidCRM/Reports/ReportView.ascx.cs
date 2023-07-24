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
using System.IO;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using Microsoft.Reporting.WebForms;

namespace SplendidCRM.Reports
{
	/// <summary>
	///		Summary description for ReportView.
	/// </summary>
	public class ReportView : SplendidControl
	{
		protected Label        lblError ;
		protected ReportViewer rdlViewer;
		protected string       sReportSQL;
		protected HtmlGenericControl divReportView;
		// 01/16/2016 Paul.   Handle the Refresh button. 
		public CommandEventHandler Command;

		public void ClearReport()
		{
			// 07/09/2006 Paul.  There is no way to to reset the ReportViewer. 
			// All we can do is hide the division. 
			divReportView.Visible = false;
		}

		public string ReportSQL
		{
			get { return sReportSQL; }
		}

		// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
		// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
		public void RunReport(Guid gREPORT_ID, string sRDL, string sMODULE_NAME)
		{
			try
			{
				// 01/24/2010 Paul.  Pass the context so that it can be used in the Validation call. 
				// 12/04/2010 Paul.  L10n is needed by the Rules Engine to allow translation of list terms. 
				// 04/13/2011 Paul.  A scheduled report does not have a Session, so we need to create a session using the same approach used for ExchangeSync. 
				// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
				// 03/24/2016 Paul.  We need an alternate way to provide parameters to render a report with a signature. 
				RdlUtil.LocalLoadReportDefinition(this.Context, null, null, L10n, T10n, rdlViewer, gREPORT_ID, sRDL, sMODULE_NAME, Guid.Empty, out sReportSQL);

				if ( bDebug )
					RegisterClientScriptBlock("SQLCode", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript(sReportSQL) + "';</script>");
				// 06/25/2006 Paul.  Refresh did not work, clear the data sources instead. 
				//rdlViewer.LocalReport.Refresh();
				rdlViewer.DataBind();
			}
			catch ( Exception ex )
			{
				lblError.Text = Utils.ExpandException(ex);
			}
		}

		// 04/06/2011 Paul.  We need a way to pull data from the Parameters form. 
		// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
		public void RunReport(Guid gREPORT_ID, string sRDL, string sMODULE_NAME, SplendidControl ctlParameterView)
		{
			try
			{
				// 04/13/2011 Paul.  A scheduled report does not have a Session, so we need to create a session using the same approach used for ExchangeSync. 
				// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
				// 03/24/2016 Paul.  We need an alternate way to provide parameters to render a report with a signature. 
				RdlUtil.LocalLoadReportDefinition(this.Context, null, ctlParameterView, L10n, T10n, rdlViewer, gREPORT_ID, sRDL, sMODULE_NAME, Guid.Empty, out sReportSQL);

				if ( bDebug )
					RegisterClientScriptBlock("SQLCode", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript(sReportSQL) + "';</script>");
				rdlViewer.DataBind();
			}
			catch ( Exception ex )
			{
				lblError.Text = Utils.ExpandException(ex);
			}
		}

		// 01/16/2016 Paul.   Handle the Refresh button. 
		private void rdlViewer_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if ( this.Command != null )
				this.Command(sender, new CommandEventArgs("Refresh", null));
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
			rdlViewer.ReportRefresh += rdlViewer_ReportRefresh;
		}
		#endregion
	}
}
