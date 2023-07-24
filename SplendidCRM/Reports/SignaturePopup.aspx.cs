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
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Reports
{
	/// <summary>
	/// Summary description for SignaturePopup.
	/// </summary>
	public class SignaturePopup : SplendidPopup
	{
		protected Label            lblError          ;
		protected Button           btnSubmit         ;
		protected Button           btnClear          ;
		protected Button           btnCancel         ;
		protected ReportView       ctlReportView     ;
		protected Guid             gID               ;
		protected string           sRDL              ;
		protected string           sMODULE_NAME      ;
		protected string           sREPORT_NAME      ;

		// 05/09/2016 Paul.  Move AddScriptReference and AddStyleSheet to Sql object. 

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Refresh" )
				{
					if ( !Sql.IsEmptyString(sRDL) )
					{
						ctlReportView.RunReport(gID, sRDL, sMODULE_NAME, null);
					}
				}
			}
			catch(Exception ex)
			{
				lblError.Text = ex.Message;
			}
		}

		private void LoadReport()
		{
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !Sql.IsEmptyGuid(gID) )
				{
					DataTable dtReport = SplendidCache.Report(gID);
					if ( dtReport != null && dtReport.Rows.Count > 0 )
					{
						DataRow rdr = dtReport.Rows[0];
						sRDL              = Sql.ToString(rdr["RDL"             ]);
						sMODULE_NAME      = Sql.ToString(rdr["MODULE_NAME"     ]);
						sREPORT_NAME      = Sql.ToString(rdr["NAME"            ]);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(Page) as AjaxControlToolkit.ToolkitScriptManager;
				Utils.RegisterJQuery(Page, mgrAjax);
				// 05/09/2016 Paul.  Move AddScriptReference and AddStyleSheet to Sql object. 
				Sql.AddScriptReference(mgrAjax, "~/Include/jqPlot/excanvas.min.js");
				Sql.AddScriptReference(mgrAjax, "~/Include/Signature/jquery.signature.min.js");
				string sBrowser = Sql.ToString(Session["Browser"]);
				if ( sBrowser == "iPhone" || sBrowser == "iPad" || sBrowser.StartsWith("Android") )
				{
					Sql.AddScriptReference(mgrAjax, "~/Include/Signature/jquery.ui.touch-punch.min.js");
				}
				Sql.AddStyleSheet(this, "~/Include/javascript/jquery-ui-1.9.1.custom.css");
				Sql.AddStyleSheet(this, "~/Include/Signature/jquery.signature.css");
				if ( !IsPostBack )
				{
					btnSubmit.Text    = L10n.Term(".LBL_SUBMIT_BUTTON_LABEL");
					btnSubmit.ToolTip = L10n.Term(".LBL_SUBMIT_BUTTON_TITLE");
					btnClear .Text    = L10n.Term(".LBL_CLEAR_BUTTON_LABEL" );
					btnClear .ToolTip = L10n.Term(".LBL_CLEAR_BUTTON_TITLE" );
					btnCancel.Text    = L10n.Term(".LBL_CANCEL_BUTTON_LABEL");
					btnCancel.ToolTip = L10n.Term(".LBL_CANCEL_BUTTON_TITLE");
					if ( !Sql.IsEmptyString(sRDL) )
					{
						SetPageTitle(L10n.Term(".moduleList.Reports") + " - " + sREPORT_NAME);
						ViewState["ctlDynamicButtons.Title"] = sREPORT_NAME;
						if ( !Sql.IsEmptyString(sRDL) )
						{
							ctlReportView.RunReport(gID, sRDL, sMODULE_NAME, null);
						}
					}
				}
				else
				{
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					string sTitle = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList.Reports") + " - " + sTitle);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
			this.Load += new System.EventHandler(this.Page_Load);
			ctlReportView.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Reports";
			SetMenu(m_sMODULE);
			LoadReport();
		}
		#endregion
	}
}
