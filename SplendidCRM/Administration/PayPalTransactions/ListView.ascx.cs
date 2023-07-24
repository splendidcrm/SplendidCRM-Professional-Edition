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

namespace SplendidCRM.Administration.PayPalTransactions
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
				if ( e.CommandName == "Search" )
				{
					// 10/15/2008 Paul.  Remove the list from the cache so that it will search again. 
					Cache.Remove("PayPal.Transactions");
					grdMain.CurrentPageIndex = 0;
				}
				// 11/17/2010 Paul.  Populate the hidden Selected field with all IDs. 
				else if ( e.CommandName == "SelectAll" )
				{
					ctlCheckAll.SelectAll(vwMain, "TRANSACTION_ID");
					grdMain.DataBind();
				}
				// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
				else if ( e.CommandName == "ToggleMassUpdate" )
				{
					pnlMassUpdateSeven.Visible = !pnlMassUpdateSeven.Visible;
				}
				else if ( e.CommandName == "Import" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						foreach ( string sTransactionID in arrID )
						{
							try
							{
								// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
								DataSet ds = PayPalCache.Transaction(Application, sTransactionID);
								// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
								PayPalUtils.ImportTransaction(Application, ds);
							}
							catch(Exception ex)
							{
								// 12/07/2008 Paul.  PayPal is having trouble retrieving records prior to 09/01/2005.  Import the minimum. 
								string sMESSAGE = "Error importing PayPal transaction " + sTransactionID + ".  " + Utils.ExpandException(ex);
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), sMESSAGE);
								if ( ex.Message.StartsWith("10001: Internal Error") )
								{
									vwMain.RowFilter = "TRANSACTION_ID = '" + sTransactionID + "'";
									try
									{
										if ( vwMain.Count > 0 )
										{
											// 12/07/2008 Paul.  We probably don't want to import from the transaction search 
											// unless the record is very old. 
											DateTime dtPAYMENT_DATE = Sql.ToDateTime(vwMain[0]["TRANSACTION_DATE"]);
											if ( dtPAYMENT_DATE.Year < 2006 )
											{
												lblError.Text += "Importing the minimal information for " + sTransactionID + "<br>";
												// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
												PayPalUtils.ImportTransaction(Application, vwMain[0]);
											}
										}
									}
									catch(Exception ex1)
									{
										sMESSAGE = "Error creating minimal records for " + sTransactionID + ".  " + Utils.ExpandException(ex1);
										SplendidError.SystemError(new StackTrace(true).GetFrame(0), sMESSAGE);
										lblError.Text += sMESSAGE + "<br>";
									}
									vwMain.RowFilter = String.Empty;
								}
								else
								{
									lblError.Text += sMESSAGE + "<br>";
								}
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
				// 12/07/2008 Paul.  If we request too many records, PayPal will complain. 
				// 11002: The number of results were truncated. Please change your search parameters if you wish to see all your results.
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
			// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
			// 01/18/2018 Paul.  Convert to new OAuth. 
			if ( !Sql.IsEmptyString(Context.Application["CONFIG.PayPal.ClientID"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayPal.ClientSecret"]) )
			{
				DataSet ds = Cache.Get("PayPal.Transactions") as DataSet;
				if ( ds == null )
				{
					ds = PayPalRest.PaymentSearch(Application, ctlSearchBasic.START_DATE, ctlSearchBasic.END_DATE, ctlSearchBasic.EMAIL);
					foreach ( DataRow row in ds.Tables["TRANSACTIONS"].Rows )
					{
						row["TRANSACTION_DATE"] = T10n.FromServerTime(Sql.ToDateTime(row["TRANSACTION_DATE"]));
					}
					Cache.Insert("PayPal.Transactions", ds, null, PayPalCache.DefaultCacheExpiration(),System.Web.Caching.Cache.NoSlidingExpiration);
				}
				vwMain = ds.Tables[0].DefaultView;
				grdMain.DataSource = vwMain ;
				if ( bBind )
				{
					grdMain.SortColumn = "TRANSACTION_DATE";
					grdMain.SortOrder  = "desc" ;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
			}
			else if ( !Sql.IsEmptyString(PayPalCache.PayPalAPIUsername(Application)) )
			{
				// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
				PayPalAPI api = PayPalCache.CreatePayPalAPI(Application);
				DataSet ds = Cache.Get("PayPal.Transactions") as DataSet;
				if ( ds == null )
				{
					ds = api.TransactionSearch(ctlSearchBasic.START_DATE, ctlSearchBasic.END_DATE, ctlSearchBasic.TRANSACTION_CLASS, ctlSearchBasic.TRANSACTION_STATUS);
					// 10/14/2008 Paul.  We need to convert the time to the user's specified timezone. 
					foreach ( DataRow row in ds.Tables["TRANSACTIONS"].Rows )
					{
						row["TRANSACTION_DATE"] = T10n.FromServerTime(Sql.ToDateTime(row["TRANSACTION_DATE"]));
					}
					Cache.Insert("PayPal.Transactions", ds, null, PayPalCache.DefaultCacheExpiration(),System.Web.Caching.Cache.NoSlidingExpiration);
				}
				
				vwMain = ds.Tables[0].DefaultView;
				grdMain.DataSource = vwMain ;
				if ( bBind )
				{
					grdMain.SortColumn = "TRANSACTION_DATE";
					grdMain.SortOrder  = "desc" ;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				if ( !IsPostBack )
				{
					// 10/14/2008 Paul.  Remove the list from the cache so that it will search again. 
					Cache.Remove("PayPal.Transactions");
					ctlSearchBasic.START_DATE = DateTime.Today.AddMonths(-1);
				}
				//this.AppendDetailViewFields(m_sMODULE + ".AccountBalance", tblMain, null);
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
			m_sMODULE = "PayPal";
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
