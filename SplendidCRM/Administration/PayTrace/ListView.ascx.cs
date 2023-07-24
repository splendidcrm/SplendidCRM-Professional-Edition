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
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.PayTrace
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected _controls.CheckAll     ctlCheckAll    ;
		protected SearchBasic   ctlSearchBasic                     ;

		protected HtmlTable     tblMain                            ;
		protected DataView      vwMain                             ;
		protected SplendidGrid  grdMain                            ;
		protected Label         lblError                           ;
		protected PlaceHolder   plcSearch                          ;
		protected MassUpdate    ctlMassUpdate                      ;
		// 06/06/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected Panel         pnlMassUpdateSeven;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Clear" )
				{
					ctlSearchBasic.ClearForm();
				}
				else if ( e.CommandName == "Search" )
				{
					Cache.Remove("PayTrace.Transactions");
					grdMain.CurrentPageIndex = 0;
				}
				else if ( e.CommandName == "SelectAll" )
				{
					ctlCheckAll.SelectAll(vwMain, "TRANXID");
					grdMain.DataBind();
				}
				// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
				else if ( e.CommandName == "ToggleMassUpdate" )
				{
					pnlMassUpdateSeven.Visible = !pnlMassUpdateSeven.Visible;
				}
				else if ( e.CommandName == "Import" )
				{
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						foreach ( string sTransactionID in arrID )
						{
							try
							{
								DataSet ds = PayTraceUtils.Transaction(Application, sTransactionID);
								PayTraceUtils.ImportTransaction(Application, ds);
							}
							catch(Exception ex)
							{
								string sMESSAGE = "Error importing PayTrace transaction " + sTransactionID + ".  " + Utils.ExpandException(ex);
								lblError.Text += sMESSAGE + "<br>";
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text += ex.Message;
			}
			try
			{
				Bind(true);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text += ex.Message;
			}
		}

		private void Bind(bool bBind)
		{
			if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) )
			{
				DataSet ds = Cache.Get("PayTrace.Transactions") as DataSet;
				if ( ds == null )
				{
					ds = PayTraceUtils.Transactions(Application, ctlSearchBasic.START_DATE, ctlSearchBasic.END_DATE, ctlSearchBasic.TRANSACTION_TYPE, ctlSearchBasic.SEARCH_TEXT, String.Empty);
					Cache.Insert("PayTrace.Transactions", ds, null, DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
				}
				
				vwMain = ds.Tables[0].DefaultView;
				grdMain.DataSource = vwMain ;
				if ( bBind )
				{
					grdMain.SortColumn = "WHEN";
					grdMain.SortOrder  = "desc" ;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				if ( !IsPostBack )
				{
					Cache.Remove("PayTrace.Transactions");
					ctlSearchBasic.START_DATE = DateTime.Today.AddMonths(-1);
					ctlSearchBasic.END_DATE   = DateTime.Today;
				}
				Bind(!IsPostBack);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = Utils.ExpandException(ex);
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
			ctlSearchBasic.Command += new CommandEventHandler(Page_Command);
			ctlMassUpdate .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll   .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "PayTrace";
			SetMenu(m_sMODULE);
			this.AppendGridColumns(grdMain, m_sMODULE + "." + LayoutListView);
			
			// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
			if ( SplendidDynamic.StackedLayout(Page.Theme) )
			{
				// 06/05/2015 Paul.  Move MassUpdate buttons to the SplendidGrid. 
				grdMain.IsMobile       = this.IsMobile;
				grdMain.MassUpdateView = m_sMODULE + ".MassUpdate";
				grdMain.Command       += new CommandEventHandler(Page_Command);
				if ( !IsPostBack )
					pnlMassUpdateSeven.Visible = false;
			}
		}
		#endregion
	}
}
