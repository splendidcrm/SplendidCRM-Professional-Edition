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
using SplendidCRM.PayPal;

namespace SplendidCRM.Administration.PayPalTransactions
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
					// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
					DataSet ds = PayPalCache.Transaction(Application, sTransactionID);
					// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
					PayPalUtils.ImportTransaction(Application, ds);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Refund" )
				{
					// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
					// 01/18/2018 Paul.  Username is being deprecated. 
					if ( !Sql.IsEmptyString(PayPalCache.PayPalAPIUsername(Application)) || !Sql.IsEmptyString(PayPalCache.PayPalClientID(Application)) )
					{
						// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
						PayPalAPI api = PayPalCache.CreatePayPalAPI(Application);
						// 01/14/2009 Paul.  Log any success warnings. 
						// 04/30/2016 Paul.  Base currency has been USD, but we should make it easy to allow a different base. 
						AbstractResponseType resp = api.RefundTransaction(sTransactionID, "Full", String.Empty, Decimal.Zero, SplendidDefaults.BaseCurrencyISO(Application));
						if ( resp.Ack == AckCodeType.SuccessWithWarning )
						{
							SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), api.ExpandErrors(resp.Errors));
						}
					
						PayPalCache.ClearTransaction(sTransactionID);
					}
					Response.Redirect("view.aspx?TransactionID=" + sTransactionID);
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
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				sTransactionID = Sql.ToString(Request["TransactionID"]);
				this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
				DynamicControl TRANSACTION_ID                        = new DynamicControl(this, "TRANSACTION_ID"                       );
				DynamicControl PARENT_TRANSACTION_ID                 = new DynamicControl(this, "PARENT_TRANSACTION_ID"                );
				DynamicControl TRANSACTION_TYPE                      = new DynamicControl(this, "TRANSACTION_TYPE"                     );
				DynamicControl RECEIPT_ID                            = new DynamicControl(this, "RECEIPT_ID"                           );
				DynamicControl PAYMENT_TYPE                          = new DynamicControl(this, "PAYMENT_TYPE"                         );
				DynamicControl PAYMENT_DATE                          = new DynamicControl(this, "PAYMENT_DATE"                         );
				DynamicControl GROSS_AMOUNT                          = new DynamicControl(this, "GROSS_AMOUNT_CURRENCY GROSS_AMOUNT"   );
				DynamicControl FEE_AMOUNT                            = new DynamicControl(this, "FEE_AMOUNT_CURRENCY FEE_AMOUNT"       );
				DynamicControl SETTLE_AMOUNT                         = new DynamicControl(this, "SETTLE_AMOUNT_CURRENCY SETTLE_AMOUNT" );
				DynamicControl TAX_AMOUNT                            = new DynamicControl(this, "TAX_AMOUNT_CURRENCY TAX_AMOUNT"       );
				DynamicControl EXCHANGE_RATE                         = new DynamicControl(this, "EXCHANGE_RATE"                        );
				DynamicControl SALES_TAX                             = new DynamicControl(this, "SALES_TAX"                            );
				DynamicControl PAYMENT_STATUS                        = new DynamicControl(this, "PAYMENT_STATUS"                       );
				DynamicControl REASON_CODE                           = new DynamicControl(this, "REASON_CODE"                          );
				DynamicControl PENDING_REASON                        = new DynamicControl(this, "PENDING_REASON"                       );
				DynamicControl INVOICE_ID                            = new DynamicControl(this, "INVOICE_ID"                           );
				DynamicControl MEMO                                  = new DynamicControl(this, "MEMO"                                 );
				DynamicControl CUSTOM                                = new DynamicControl(this, "CUSTOM"                               );
				DynamicControl AUCTION_BUYER_ID                      = new DynamicControl(this, "AUCTION_BUYER_ID"                     );
				DynamicControl AUCTION_CLOSING_DATE                  = new DynamicControl(this, "AUCTION_CLOSING_DATE"                 );
				DynamicControl AUCTION_MULTI_ITEM                    = new DynamicControl(this, "AUCTION_MULTI_ITEM"                   );

				DynamicControl PAYER                                 = new DynamicControl(this, "PAYER"                                );
				DynamicControl PAYER_ID                              = new DynamicControl(this, "PAYER_ID"                             );
				DynamicControl PAYER_STATUS                          = new DynamicControl(this, "PAYER_STATUS"                         );
				DynamicControl PAYER_SALUATION                       = new DynamicControl(this, "PAYER_SALUATION"                      );
				DynamicControl PAYER_FIRST_NAME                      = new DynamicControl(this, "PAYER_FIRST_NAME"                     );
				DynamicControl PAYER_LAST_NAME                       = new DynamicControl(this, "PAYER_LAST_NAME"                      );
				DynamicControl PAYER_MIDDLE_NAME                     = new DynamicControl(this, "PAYER_MIDDLE_NAME"                    );
				DynamicControl PAYER_SUFFIX                          = new DynamicControl(this, "PAYER_SUFFIX"                         );
				DynamicControl PAYER_BUSINESS                        = new DynamicControl(this, "PAYER_BUSINESS"                       );
				DynamicControl PAYER_PHONE                           = new DynamicControl(this, "PAYER_PHONE"                          );
				DynamicControl PAYER_ADDRESS_OWNER                   = new DynamicControl(this, "PAYER_ADDRESS_OWNER"                  );
				DynamicControl PAYER_ADDRESS_STATUS                  = new DynamicControl(this, "PAYER_ADDRESS_STATUS"                 );
				DynamicControl PAYER_ADDRESS_NAME                    = new DynamicControl(this, "PAYER_ADDRESS_NAME"                   );

				DynamicControl PAYER_ADDRESS_STREET1                 = new DynamicControl(this, "PAYER_ADDRESS_STREET1"                );
				DynamicControl PAYER_ADDRESS_STREET2                 = new DynamicControl(this, "PAYER_ADDRESS_STREET2"                );
				DynamicControl PAYER_ADDRESS_CITY                    = new DynamicControl(this, "PAYER_ADDRESS_CITY"                   );
				DynamicControl PAYER_ADDRESS_STATE                   = new DynamicControl(this, "PAYER_ADDRESS_STATE"                  );
				DynamicControl PAYER_ADDRESS_COUNTRY                 = new DynamicControl(this, "PAYER_ADDRESS_COUNTRY"                );
				DynamicControl PAYER_ADDRESS_COUNTRY_NAME            = new DynamicControl(this, "PAYER_ADDRESS_COUNTRY_NAME"           );
				DynamicControl PAYER_ADDRESS_POSTAL_CODE             = new DynamicControl(this, "PAYER_ADDRESS_POSTAL_CODE"            );
				DynamicControl PAYER_ADDRESS_PHONE                   = new DynamicControl(this, "PAYER_ADDRESS_PHONE"                  );

				DynamicControl PAYER_ADDRESS_INTL_NAME               = new DynamicControl(this, "PAYER_ADDRESS_INTL_NAME"              );
				DynamicControl PAYER_ADDRESS_INTL_STATE              = new DynamicControl(this, "PAYER_ADDRESS_INTL_STATE"             );
				DynamicControl PAYER_ADDRESS_INTL_STREET             = new DynamicControl(this, "PAYER_ADDRESS_INTL_STREET"            );

				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				//if ( !IsPostBack )
				{
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = sTransactionID;
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);

					// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
					// 02/28/2012 Paul.  Don't query if no transaction is provided. 
					// 01/18/2018 Paul.  Convert to new OAuth. 
					if ( !Sql.IsEmptyString(Context.Application["CONFIG.PayPal.ClientID"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayPal.ClientSecret"]) && !Sql.IsEmptyString(sTransactionID) )
					{
						// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
						DataSet ds = PayPalRest.PaymentDetails(Application, sTransactionID);
						DataTable dt = ds.Tables["TRANSACTIONS"];
						if ( dt.Rows.Count > 0 )
						{
							DataRow row = dt.Rows[0];
							if ( row["PAYMENT_DATE"] != DBNull.Value )
								row["PAYMENT_DATE"] = T10n.FromServerTime(Sql.ToDateTime(row["PAYMENT_DATE"]));
							if ( row["AUCTION_CLOSING_DATE"] != DBNull.Value )
								row["AUCTION_CLOSING_DATE"] = T10n.FromServerTime(Sql.ToDateTime(row["AUCTION_CLOSING_DATE"]));

							TRANSACTION_ID             .Text = Sql.ToString(row["TRANSACTION_ID"                       ]);
							PARENT_TRANSACTION_ID      .Text = Sql.ToString(row["PARENT_TRANSACTION_ID"                ]);
							TRANSACTION_TYPE           .Text = Sql.ToString(row["TRANSACTION_TYPE"                     ]);
							RECEIPT_ID                 .Text = Sql.ToString(row["RECEIPT_ID"                           ]);
							PAYMENT_TYPE               .Text = Sql.ToString(row["PAYMENT_TYPE"                         ]);
							PAYMENT_DATE               .Text = Sql.ToString(row["PAYMENT_DATE"                         ]);
							GROSS_AMOUNT               .Text = Sql.ToString(row["GROSS_AMOUNT_CURRENCY" ]) + " " + Sql.ToString(row["GROSS_AMOUNT" ]);
							FEE_AMOUNT                 .Text = Sql.ToString(row["FEE_AMOUNT_CURRENCY"   ]) + " " + Sql.ToString(row["FEE_AMOUNT"   ]);
							SETTLE_AMOUNT              .Text = Sql.ToString(row["SETTLE_AMOUNT_CURRENCY"]) + " " + Sql.ToString(row["SETTLE_AMOUNT"]);
							TAX_AMOUNT                 .Text = Sql.ToString(row["TAX_AMOUNT_CURRENCY"   ]) + " " + Sql.ToString(row["TAX_AMOUNT"   ]);
							EXCHANGE_RATE              .Text = Sql.ToString(row["EXCHANGE_RATE"                        ]);
							SALES_TAX                  .Text = Sql.ToString(row["SALES_TAX"                            ]);
							PAYMENT_STATUS             .Text = Sql.ToString(row["PAYMENT_STATUS"                       ]);
							REASON_CODE                .Text = Sql.ToString(row["REASON_CODE"                          ]);
							PENDING_REASON             .Text = Sql.ToString(row["PENDING_REASON"                       ]);
							INVOICE_ID                 .Text = Sql.ToString(row["INVOICE_ID"                           ]);
							MEMO                       .Text = Sql.ToString(row["MEMO"                                 ]);
							CUSTOM                     .Text = Sql.ToString(row["CUSTOM"                               ]);
							AUCTION_BUYER_ID           .Text = Sql.ToString(row["AUCTION_BUYER_ID"                     ]);
							AUCTION_CLOSING_DATE       .Text = Sql.ToString(row["AUCTION_CLOSING_DATE"                 ]);
							AUCTION_MULTI_ITEM         .Text = Sql.ToString(row["AUCTION_MULTI_ITEM"                   ]);

							PAYER                      .Text = Sql.ToString(row["PAYER"                                ]);
							PAYER_ID                   .Text = Sql.ToString(row["PAYER_ID"                             ]);
							PAYER_STATUS               .Text = Sql.ToString(row["PAYER_STATUS"                         ]);
							PAYER_SALUATION            .Text = Sql.ToString(row["PAYER_SALUATION"                      ]);
							PAYER_FIRST_NAME           .Text = Sql.ToString(row["PAYER_FIRST_NAME"                     ]);
							PAYER_LAST_NAME            .Text = Sql.ToString(row["PAYER_LAST_NAME"                      ]);
							PAYER_MIDDLE_NAME          .Text = Sql.ToString(row["PAYER_MIDDLE_NAME"                    ]);
							PAYER_SUFFIX               .Text = Sql.ToString(row["PAYER_SUFFIX"                         ]);
							PAYER_BUSINESS             .Text = Sql.ToString(row["PAYER_BUSINESS"                       ]);
							PAYER_PHONE                .Text = Sql.ToString(row["PAYER_PHONE"                          ]);
							PAYER_ADDRESS_OWNER        .Text = Sql.ToString(row["PAYER_ADDRESS_OWNER"                  ]);
							PAYER_ADDRESS_STATUS       .Text = Sql.ToString(row["PAYER_ADDRESS_STATUS"                 ]);
							PAYER_ADDRESS_NAME         .Text = Sql.ToString(row["PAYER_ADDRESS_NAME"                   ]);

							PAYER_ADDRESS_STREET1      .Text = Sql.ToString(row["PAYER_ADDRESS_STREET1"                ]);
							PAYER_ADDRESS_STREET2      .Text = Sql.ToString(row["PAYER_ADDRESS_STREET2"                ]);
							PAYER_ADDRESS_CITY         .Text = Sql.ToString(row["PAYER_ADDRESS_CITY"                   ]);
							PAYER_ADDRESS_STATE        .Text = Sql.ToString(row["PAYER_ADDRESS_STATE"                  ]);
							PAYER_ADDRESS_COUNTRY      .Text = Sql.ToString(row["PAYER_ADDRESS_COUNTRY"                ]);
							PAYER_ADDRESS_COUNTRY_NAME .Text = Sql.ToString(row["PAYER_ADDRESS_COUNTRY_NAME"           ]);
							PAYER_ADDRESS_POSTAL_CODE  .Text = Sql.ToString(row["PAYER_ADDRESS_POSTAL_CODE"            ]);
							PAYER_ADDRESS_PHONE        .Text = Sql.ToString(row["PAYER_ADDRESS_PHONE"                  ]);

							PAYER_ADDRESS_INTL_NAME    .Text = Sql.ToString(row["PAYER_ADDRESS_INTL_NAME"              ]);
							PAYER_ADDRESS_INTL_STATE   .Text = Sql.ToString(row["PAYER_ADDRESS_INTL_STATE"             ]);
							PAYER_ADDRESS_INTL_STREET  .Text = Sql.ToString(row["PAYER_ADDRESS_INTL_STREET"            ]);

							//ctlDynamicButtons.EnableButton("Retry" , false);
							//ctlDynamicButtons.EnableButton("Settle", (OPERATION.Text == "Reserve"));
							//ctlDynamicButtons.EnableButton("Refund", (OPERATION.Text == "Pay" || OPERATION.Text == "Settle"));
						}
					}
					else if ( !Sql.IsEmptyString(PayPalCache.PayPalAPIUsername(Application)) && !Sql.IsEmptyString(sTransactionID) )
					{
						// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
						DataSet ds = PayPalCache.Transaction(Application, sTransactionID);
						DataTable dt = ds.Tables["TRANSACTIONS"];
						if ( dt.Rows.Count > 0 )
						{
							DataRow row = dt.Rows[0];
							if ( row["PAYMENT_DATE"] != DBNull.Value )
								row["PAYMENT_DATE"] = T10n.FromServerTime(Sql.ToDateTime(row["PAYMENT_DATE"]));
							if ( row["AUCTION_CLOSING_DATE"] != DBNull.Value )
								row["AUCTION_CLOSING_DATE"] = T10n.FromServerTime(Sql.ToDateTime(row["AUCTION_CLOSING_DATE"]));

							TRANSACTION_ID             .Text = Sql.ToString(row["TRANSACTION_ID"                       ]);
							PARENT_TRANSACTION_ID      .Text = Sql.ToString(row["PARENT_TRANSACTION_ID"                ]);
							TRANSACTION_TYPE           .Text = Sql.ToString(row["TRANSACTION_TYPE"                     ]);
							RECEIPT_ID                 .Text = Sql.ToString(row["RECEIPT_ID"                           ]);
							PAYMENT_TYPE               .Text = Sql.ToString(row["PAYMENT_TYPE"                         ]);
							PAYMENT_DATE               .Text = Sql.ToString(row["PAYMENT_DATE"                         ]);
							GROSS_AMOUNT               .Text = Sql.ToString(row["GROSS_AMOUNT_CURRENCY" ]) + " " + Sql.ToString(row["GROSS_AMOUNT" ]);
							FEE_AMOUNT                 .Text = Sql.ToString(row["FEE_AMOUNT_CURRENCY"   ]) + " " + Sql.ToString(row["FEE_AMOUNT"   ]);
							SETTLE_AMOUNT              .Text = Sql.ToString(row["SETTLE_AMOUNT_CURRENCY"]) + " " + Sql.ToString(row["SETTLE_AMOUNT"]);
							TAX_AMOUNT                 .Text = Sql.ToString(row["TAX_AMOUNT_CURRENCY"   ]) + " " + Sql.ToString(row["TAX_AMOUNT"   ]);
							EXCHANGE_RATE              .Text = Sql.ToString(row["EXCHANGE_RATE"                        ]);
							SALES_TAX                  .Text = Sql.ToString(row["SALES_TAX"                            ]);
							PAYMENT_STATUS             .Text = Sql.ToString(row["PAYMENT_STATUS"                       ]);
							REASON_CODE                .Text = Sql.ToString(row["REASON_CODE"                          ]);
							PENDING_REASON             .Text = Sql.ToString(row["PENDING_REASON"                       ]);
							INVOICE_ID                 .Text = Sql.ToString(row["INVOICE_ID"                           ]);
							MEMO                       .Text = Sql.ToString(row["MEMO"                                 ]);
							CUSTOM                     .Text = Sql.ToString(row["CUSTOM"                               ]);
							AUCTION_BUYER_ID           .Text = Sql.ToString(row["AUCTION_BUYER_ID"                     ]);
							AUCTION_CLOSING_DATE       .Text = Sql.ToString(row["AUCTION_CLOSING_DATE"                 ]);
							AUCTION_MULTI_ITEM         .Text = Sql.ToString(row["AUCTION_MULTI_ITEM"                   ]);

							PAYER                      .Text = Sql.ToString(row["PAYER"                                ]);
							PAYER_ID                   .Text = Sql.ToString(row["PAYER_ID"                             ]);
							PAYER_STATUS               .Text = Sql.ToString(row["PAYER_STATUS"                         ]);
							PAYER_SALUATION            .Text = Sql.ToString(row["PAYER_SALUATION"                      ]);
							PAYER_FIRST_NAME           .Text = Sql.ToString(row["PAYER_FIRST_NAME"                     ]);
							PAYER_LAST_NAME            .Text = Sql.ToString(row["PAYER_LAST_NAME"                      ]);
							PAYER_MIDDLE_NAME          .Text = Sql.ToString(row["PAYER_MIDDLE_NAME"                    ]);
							PAYER_SUFFIX               .Text = Sql.ToString(row["PAYER_SUFFIX"                         ]);
							PAYER_BUSINESS             .Text = Sql.ToString(row["PAYER_BUSINESS"                       ]);
							PAYER_PHONE                .Text = Sql.ToString(row["PAYER_PHONE"                          ]);
							PAYER_ADDRESS_OWNER        .Text = Sql.ToString(row["PAYER_ADDRESS_OWNER"                  ]);
							PAYER_ADDRESS_STATUS       .Text = Sql.ToString(row["PAYER_ADDRESS_STATUS"                 ]);
							PAYER_ADDRESS_NAME         .Text = Sql.ToString(row["PAYER_ADDRESS_NAME"                   ]);

							PAYER_ADDRESS_STREET1      .Text = Sql.ToString(row["PAYER_ADDRESS_STREET1"                ]);
							PAYER_ADDRESS_STREET2      .Text = Sql.ToString(row["PAYER_ADDRESS_STREET2"                ]);
							PAYER_ADDRESS_CITY         .Text = Sql.ToString(row["PAYER_ADDRESS_CITY"                   ]);
							PAYER_ADDRESS_STATE        .Text = Sql.ToString(row["PAYER_ADDRESS_STATE"                  ]);
							PAYER_ADDRESS_COUNTRY      .Text = Sql.ToString(row["PAYER_ADDRESS_COUNTRY"                ]);
							PAYER_ADDRESS_COUNTRY_NAME .Text = Sql.ToString(row["PAYER_ADDRESS_COUNTRY_NAME"           ]);
							PAYER_ADDRESS_POSTAL_CODE  .Text = Sql.ToString(row["PAYER_ADDRESS_POSTAL_CODE"            ]);
							PAYER_ADDRESS_PHONE        .Text = Sql.ToString(row["PAYER_ADDRESS_PHONE"                  ]);

							PAYER_ADDRESS_INTL_NAME    .Text = Sql.ToString(row["PAYER_ADDRESS_INTL_NAME"              ]);
							PAYER_ADDRESS_INTL_STATE   .Text = Sql.ToString(row["PAYER_ADDRESS_INTL_STATE"             ]);
							PAYER_ADDRESS_INTL_STREET  .Text = Sql.ToString(row["PAYER_ADDRESS_INTL_STREET"            ]);

							//ctlDynamicButtons.EnableButton("Retry" , false);
							//ctlDynamicButtons.EnableButton("Settle", (OPERATION.Text == "Reserve"));
							//ctlDynamicButtons.EnableButton("Refund", (OPERATION.Text == "Pay" || OPERATION.Text == "Settle"));
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
			m_sMODULE = "PayPal";
			SetMenu(m_sMODULE);
			this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
			// 06/10/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
			ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
		}
		#endregion
	}
}
