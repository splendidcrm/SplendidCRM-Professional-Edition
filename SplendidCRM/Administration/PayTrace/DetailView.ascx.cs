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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using SplendidCRM;

namespace SplendidCRM.Administration.PayTrace
{
	/// <summary>
	///		Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;

		protected string      sTransactionID     ;
		protected HtmlTable   tblMain            ;
		protected PlaceHolder plcSubPanel        ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Import" )
				{
					DataSet ds = PayTraceUtils.Transaction(Application, sTransactionID);
					PayTraceUtils.ImportTransaction(Application, ds);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Refund" )
				{
					if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) )
					{
						PayTraceUtils.Refund(Application, sTransactionID);
						HttpRuntime.Cache.Remove("PayTrace.Transaction." + sTransactionID);
					}
					Response.Redirect("view.aspx?TRANXID=" + sTransactionID);
				}
			}
			catch(Exception ex)
			{
				ctlDynamicButtons.ErrorText = ex.Message;
				return;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				sTransactionID = Sql.ToString(Request["TRANXID"]);
				// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
				ctlDynamicButtons.Title = sTransactionID;
				SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);

				if ( !IsPostBack )
				{
					if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) && !Sql.IsEmptyString(sTransactionID) )
					{
						DataSet ds = PayTraceUtils.Transaction(Application, sTransactionID);
						DataTable dt = ds.Tables["TRANSACTIONS"];
						if ( dt.Rows.Count > 0 )
						{
							DataRow row = dt.Rows[0];
							this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
							this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, row);
							ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, row);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "PayTrace";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
				this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			}
		}
		#endregion
	}
}
