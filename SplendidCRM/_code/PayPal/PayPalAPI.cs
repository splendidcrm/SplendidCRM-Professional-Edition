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
using System.Xml;
using System.Net;
using System.Data;
using System.Text;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using SplendidCRM.PayPal;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for PayPalAPI.
	/// </summary>
	public class PayPalAPI
	{
		private PayPalAPISoapBinding api;
		private PayPalAPIAASoapBinding direct;

		public PayPalAPI(string sAPIUsername, string sAPIPassword, byte[] byPkcs12, string sPkcs12Password, string sWebServiceURL)
		{
			// 07/01/2018 Paul.  As of June 30, 2018, the PayPal system requires TLS 1.2. 
			// https://stackoverflow.com/questions/28286086/default-securityprotocol-in-net-4-5
			// https://github.com/paypal/PayPal-NET-SDK/issues/142
			if ( !ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) )
			{
				ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
			}
			api = new PayPalAPISoapBinding();
			api.Url = sWebServiceURL;

			X509Certificate2 cert = null;
			if ( String.IsNullOrEmpty(sPkcs12Password) )
				cert = new X509Certificate2(byPkcs12);
			else
				cert = new X509Certificate2(byPkcs12, sPkcs12Password);
			api.ClientCertificates.Add(cert);

			UserIdPasswordType usr = new UserIdPasswordType();
			usr.Username = sAPIUsername;
			usr.Password = sAPIPassword;

			api.RequesterCredentials = new CustomSecurityHeaderType();
			api.RequesterCredentials.Credentials = usr;

			// 11/08/2008 Paul.  DirectPay goes through a separate API. 
			direct = new PayPalAPIAASoapBinding();
			direct.Url = sWebServiceURL;

			direct.ClientCertificates.Add(cert);

			direct.RequesterCredentials = new CustomSecurityHeaderType();
			direct.RequesterCredentials.Credentials = usr;
		}

		public string ExpandErrors(ErrorType[] arrErrors)
		{
			StringBuilder sb = new StringBuilder();
			foreach ( ErrorType err in arrErrors )
			{
				sb.Append(err.ErrorCode );
				sb.Append(": ");
				sb.Append(err.LongMessage);
				sb.Append("\r\n");
			}
			return sb.ToString();
		}

		#region Transaction Operations
		public DataSet TransactionSearch(DateTime dtStartDate, DateTime dtEndDate, string sTransactionClass, string sTransactionStatus)
		{
			TransactionSearchReq req = new TransactionSearchReq();
			req.TransactionSearchRequest = new TransactionSearchRequestType();
			req.TransactionSearchRequest.Version = "51.0";
			
			// 10/14/2008 Paul.  Start date is required. 
			req.TransactionSearchRequest.StartDate = dtStartDate;
			if ( dtEndDate != DateTime.MinValue )
			{
				req.TransactionSearchRequest.EndDate = dtEndDate;
				req.TransactionSearchRequest.EndDateSpecified = true;
			}
			if ( !String.IsNullOrEmpty(sTransactionClass) )
			{
				if ( Enum.IsDefined(typeof(PaymentTransactionClassCodeType), sTransactionClass) )
				{
					req.TransactionSearchRequest.TransactionClass = (PaymentTransactionClassCodeType) Enum.Parse(typeof(PaymentTransactionClassCodeType), sTransactionClass);
					req.TransactionSearchRequest.TransactionClassSpecified = true;
				}
			}
			if ( !String.IsNullOrEmpty(sTransactionStatus) )
			{
				if ( Enum.IsDefined(typeof(PaymentTransactionStatusCodeType), sTransactionStatus) )
				{
					req.TransactionSearchRequest.Status = (PaymentTransactionStatusCodeType) Enum.Parse(typeof(PaymentTransactionStatusCodeType), sTransactionStatus);
					req.TransactionSearchRequest.StatusSpecified = true;
				}
			}
			TransactionSearchResponseType resp = api.TransactionSearch(req);
			if ( resp.Ack != AckCodeType.Success && resp.Errors != null && resp.Errors.Length > 0 )
			{
				// 01/14/2008 Paul.  SuccessWithWarning is not an error, so just continue. 
				if ( resp.Ack != AckCodeType.SuccessWithWarning )
					throw(new Exception(ExpandErrors(resp.Errors)));
			}

			DataSet ds = new DataSet();
			DataTable dtTransactions = ds.Tables.Add("TRANSACTIONS");
			dtTransactions.Columns.Add("TRANSACTION_ID"       , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("TRANSACTION_TYPE"     , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("TRANSACTION_STATUS"   , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("TRANSACTION_DATE"     , Type.GetType("System.DateTime"));
			dtTransactions.Columns.Add("TIMEZONE"             , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("GROSS_AMOUNT"         , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("GROSS_AMOUNT_CURRENCY", Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("FEE_AMOUNT"           , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("FEE_AMOUNT_CURRENCY"  , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("NET_AMOUNT"           , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("NET_AMOUNT_CURRENCY"  , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER"                , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_DISPLAY_NAME"   , Type.GetType("System.String"  ));

			if ( resp.PaymentTransactions != null )
			{
				foreach ( PaymentTransactionSearchResultType trn in resp.PaymentTransactions )
				{
					DataRow row = dtTransactions.NewRow();
					dtTransactions.Rows.Add(row);
					row["TRANSACTION_ID"       ] = trn.TransactionID                    ;
					row["TRANSACTION_TYPE"     ] = trn.Type                             ;
					row["TRANSACTION_STATUS"   ] = trn.Status.ToString()                ;
					// 12/14/2008 Paul.  Convert time from GMT. 
					row["TRANSACTION_DATE"     ] = trn.Timestamp.ToLocalTime()          ;
					row["TIMEZONE"             ] = trn.Timezone                         ;
					row["GROSS_AMOUNT"         ] = trn.GrossAmount.Value                ;
					row["GROSS_AMOUNT_CURRENCY"] = trn.GrossAmount.currencyID.ToString();
					row["FEE_AMOUNT"           ] = trn.FeeAmount.Value                  ;
					row["FEE_AMOUNT_CURRENCY"  ] = trn.FeeAmount.currencyID.ToString()  ;
					row["NET_AMOUNT"           ] = trn.NetAmount.Value                  ;
					row["NET_AMOUNT_CURRENCY"  ] = trn.NetAmount.currencyID.ToString()  ;
					row["PAYER"                ] = trn.Payer                            ;
					row["PAYER_DISPLAY_NAME"   ] = trn.PayerDisplayName                 ;
				}
			}
			return ds;
		}

		public DataSet GetTransactionDetails(string sTransactionID)
		{
			GetTransactionDetailsReq req = new GetTransactionDetailsReq();
			req.GetTransactionDetailsRequest = new GetTransactionDetailsRequestType();
			req.GetTransactionDetailsRequest.TransactionID = sTransactionID;
			req.GetTransactionDetailsRequest.Version = "51.0";
			GetTransactionDetailsResponseType resp = api.GetTransactionDetails(req);
			if ( resp.Ack != AckCodeType.Success && resp.Errors != null && resp.Errors.Length > 0 )
			{
				// 01/14/2008 Paul.  SuccessWithWarning is not an error, so just continue. 
				if ( resp.Ack != AckCodeType.SuccessWithWarning )
					throw(new Exception(ExpandErrors(resp.Errors)));
			}

			DataSet ds = new DataSet();
			DataTable dtTransactions = ds.Tables.Add("TRANSACTIONS");
			dtTransactions.Columns.Add("RECEIVER_BUSINESS"             , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("RECEIVER"                      , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("RECEIVER_ID"                   , Type.GetType("System.String"  ));

			dtTransactions.Columns.Add("PAYER"                         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ID"                      , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_STATUS"                  , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_SALUATION"               , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_FIRST_NAME"              , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_LAST_NAME"               , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_MIDDLE_NAME"             , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_SUFFIX"                  , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_PHONE"                   , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_BUSINESS"                , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_COUNTRY"                 , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_OWNER"           , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_STATUS"          , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_NAME"            , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_STREET1"         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_STREET2"         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_CITY"            , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_STATE"           , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_COUNTRY"         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_COUNTRY_NAME"    , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_PHONE"           , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_POSTAL_CODE"     , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_INTL_NAME"       , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_INTL_STATE"      , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYER_ADDRESS_INTL_STREET"     , Type.GetType("System.String"  ));

			dtTransactions.Columns.Add("TRANSACTION_ID"                , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PARENT_TRANSACTION_ID"         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("RECEIPT_ID"                    , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("TRANSACTION_TYPE"              , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYMENT_TYPE"                  , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYMENT_DATE"                  , Type.GetType("System.DateTime"));
			dtTransactions.Columns.Add("GROSS_AMOUNT_CURRENCY"         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("GROSS_AMOUNT"                  , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("FEE_AMOUNT_CURRENCY"           , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("FEE_AMOUNT"                    , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("SETTLE_AMOUNT_CURRENCY"        , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SETTLE_AMOUNT"                 , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("TAX_AMOUNT_CURRENCY"           , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("TAX_AMOUNT"                    , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("EXCHANGE_RATE"                 , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PAYMENT_STATUS"                , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("PENDING_REASON"                , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("REASON_CODE"                   , Type.GetType("System.String"  ));

			dtTransactions.Columns.Add("AUCTION_BUYER_ID"              , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("AUCTION_CLOSING_DATE"          , Type.GetType("System.DateTime"));
			dtTransactions.Columns.Add("AUCTION_MULTI_ITEM"            , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("CUSTOM"                        , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("INVOICE_ID"                    , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("MEMO"                          , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SALES_TAX"                     , Type.GetType("System.Decimal" ));
			dtTransactions.Columns.Add("SUBSCRIPTION_USERNAME"         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SUBSCRIPTION_PASSWORD"         , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SUBSCRIPTION_SUBSCRIPTION_ID"  , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SUBSCRIPTION_SUBSCRIPTION_DATE", Type.GetType("System.DateTime"));
			dtTransactions.Columns.Add("SUBSCRIPTION_EFFECTIVE_DATE"   , Type.GetType("System.DateTime"));
			dtTransactions.Columns.Add("SUBSCRIPTION_REATTEMPT"        , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SUBSCRIPTION_RECURRENCES"      , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SUBSCRIPTION_RECURRING"        , Type.GetType("System.String"  ));
			dtTransactions.Columns.Add("SUBSCRIPTION_RETRY_TIME"       , Type.GetType("System.DateTime"));
			dtTransactions.Columns.Add("SUBSCRIPTION_TERMS"            , Type.GetType("System.String"  ));


			DataTable dtLineItems = ds.Tables.Add("LINE_ITEMS");
			dtLineItems.Columns.Add("TRANSACTION_ID"   , Type.GetType("System.String"  ));
			dtLineItems.Columns.Add("AMOUNT_CURRENCY"  , Type.GetType("System.String"  ));
			dtLineItems.Columns.Add("AMOUNT"           , Type.GetType("System.Decimal" ));
			dtLineItems.Columns.Add("NAME"             , Type.GetType("System.String"  ));
			dtLineItems.Columns.Add("NUMBER"           , Type.GetType("System.String"  ));
			dtLineItems.Columns.Add("OPTIONS"          , Type.GetType("System.String"  ));
			dtLineItems.Columns.Add("QUANTITY"         , Type.GetType("System.Decimal" ));
			dtLineItems.Columns.Add("SALES_TAX"        , Type.GetType("System.Decimal" ));

			PaymentTransactionType pt = resp.PaymentTransactionDetails;
			if ( pt != null )
			{
				ReceiverInfoType    rec   = pt.ReceiverInfo   ;
				PayerInfoType       payer = pt.PayerInfo      ;
				PaymentInfoType     pmt   = pt.PaymentInfo    ;
				PaymentItemInfoType itm   = pt.PaymentItemInfo;

				DataRow row = dtTransactions.NewRow();
				dtTransactions.Rows.Add(row);
				if ( rec != null )
				{
					row["RECEIVER_BUSINESS"          ] = rec.Business                                 ;
					row["RECEIVER"                   ] = rec.Receiver                                 ;
					row["RECEIVER_ID"                ] = rec.ReceiverID                               ;
				}

				if ( payer != null )
				{
					row["PAYER"                      ] = payer.Payer                                  ;
					row["PAYER_ID"                   ] = payer.PayerID                                ;
					row["PAYER_STATUS"               ] = DBNull.Value                                 ;
					if ( payer.PayerName != null )
					{
						row["PAYER_SALUATION"            ] = payer.PayerName.Salutation                   ;
						row["PAYER_FIRST_NAME"           ] = payer.PayerName.FirstName                    ;
						row["PAYER_LAST_NAME"            ] = payer.PayerName.LastName                     ;
						row["PAYER_MIDDLE_NAME"          ] = payer.PayerName.MiddleName                   ;
						row["PAYER_SUFFIX"               ] = payer.PayerName.Suffix                       ;
					}
					row["PAYER_PHONE"                ] = payer.ContactPhone                           ;
					row["PAYER_BUSINESS"             ] = payer.PayerBusiness                          ;
					row["PAYER_COUNTRY"              ] = DBNull.Value                                 ;
					row["PAYER_ADDRESS_OWNER"        ] = DBNull.Value                                 ;
					row["PAYER_ADDRESS_STATUS"       ] = DBNull.Value                                 ;
					if ( payer.PayerStatusSpecified )
						row["PAYER_STATUS"           ] = payer.PayerStatus                            ;
					if ( payer.PayerCountrySpecified )
						row["PAYER_COUNTRY"          ] = payer.PayerCountry                           ;
					if ( payer.Address != null )
					{
						row["PAYER_ADDRESS_NAME"         ] = payer.Address.Name                           ;
						row["PAYER_ADDRESS_STREET1"      ] = payer.Address.Street1                        ;
						row["PAYER_ADDRESS_STREET2"      ] = payer.Address.Street2                        ;
						row["PAYER_ADDRESS_CITY"         ] = payer.Address.CityName                       ;
						row["PAYER_ADDRESS_STATE"        ] = payer.Address.StateOrProvince                ;
						row["PAYER_ADDRESS_COUNTRY"      ] = DBNull.Value                                 ;
						row["PAYER_ADDRESS_COUNTRY_NAME" ] = payer.Address.CountryName                    ;
						row["PAYER_ADDRESS_PHONE"        ] = payer.Address.Phone                          ;
						row["PAYER_ADDRESS_POSTAL_CODE"  ] = payer.Address.PostalCode                     ;
						row["PAYER_ADDRESS_INTL_NAME"    ] = payer.Address.InternationalName              ;
						row["PAYER_ADDRESS_INTL_STATE"   ] = payer.Address.InternationalStateAndCity      ;
						row["PAYER_ADDRESS_INTL_STREET"  ] = payer.Address.InternationalStreet            ;
						if ( payer.Address.AddressOwnerSpecified)
							row["PAYER_ADDRESS_OWNER"    ] = payer.Address.AddressOwner                   ;
						if ( payer.Address.AddressStatusSpecified )
							row["PAYER_ADDRESS_STATUS"   ] = payer.Address.AddressStatus                  ;
						if ( payer.Address.CountrySpecified )
							row["PAYER_ADDRESS_COUNTRY"  ] = payer.Address.Country                        ;
					}
				}

				if ( pmt != null )
				{
					row["TRANSACTION_ID"             ] = pmt.TransactionID                            ;
					row["PARENT_TRANSACTION_ID"      ] = pmt.ParentTransactionID                      ;
					row["RECEIPT_ID"                 ] = pmt.ReceiptID                                ;
					row["TRANSACTION_TYPE"           ] = pmt.TransactionType                          ;
					row["PAYMENT_TYPE"               ] = DBNull.Value                                 ;
					// 12/14/2008 Paul.  Convert time from GMT. 
					row["PAYMENT_DATE"               ] = pmt.PaymentDate.ToLocalTime()                ;
					if ( pmt.GrossAmount != null )
					{
						row["GROSS_AMOUNT_CURRENCY"  ] = pmt.GrossAmount.currencyID.ToString()        ;
						row["GROSS_AMOUNT"           ] = Sql.ToDecimal(pmt.GrossAmount.Value)         ;
					}
					if ( pmt.FeeAmount != null )
					{
						row["FEE_AMOUNT_CURRENCY"    ] = pmt.FeeAmount.currencyID.ToString()          ;
						row["FEE_AMOUNT"             ] = Sql.ToDecimal(pmt.FeeAmount.Value)           ;
					}
					if ( pmt.SettleAmount != null )
					{
						row["SETTLE_AMOUNT_CURRENCY" ] = pmt.SettleAmount.currencyID.ToString()       ;
						row["SETTLE_AMOUNT"          ] = Sql.ToDecimal(pmt.SettleAmount.Value)        ;
					}
					if ( pmt.TaxAmount != null )
					{
						row["TAX_AMOUNT_CURRENCY"    ] = pmt.TaxAmount.currencyID.ToString()          ;
						row["TAX_AMOUNT"             ] = Sql.ToDecimal(pmt.TaxAmount.Value)           ;
					}
					row["EXCHANGE_RATE"              ] = pmt.ExchangeRate                             ;
					row["PAYMENT_STATUS"             ] = pmt.PaymentStatus                            ;
					row["PENDING_REASON"             ] = DBNull.Value                                 ;
					row["REASON_CODE"                ] = DBNull.Value                                 ;
					if ( pmt.PaymentTypeSpecified )
						row["PAYMENT_TYPE"           ] = pmt.PaymentType                              ;
					if ( pmt.PendingReasonSpecified )
						row["PENDING_REASON"         ] = pmt.PendingReason                            ;
					if ( pmt.ReasonCodeSpecified )
						row["REASON_CODE"            ] = pmt.ReasonCode                               ;
				}

				if ( itm != null )
				{
					if ( itm.Auction != null )
					{
						row["AUCTION_BUYER_ID"              ] = itm.Auction.BuyerID              ;
						if ( itm.Auction.ClosingDate != null && itm.Auction.ClosingDate != DateTime.MinValue )
							row["AUCTION_CLOSING_DATE"          ] = itm.Auction.ClosingDate.ToLocalTime();
						row["AUCTION_MULTI_ITEM"            ] = itm.Auction.multiItem            ;
					}
					row["CUSTOM"                        ] = itm.Custom                       ;
					row["INVOICE_ID"                    ] = itm.InvoiceID                    ;
					row["MEMO"                          ] = itm.Memo                         ;
					row["SALES_TAX"                     ] = Sql.ToDecimal(itm.SalesTax)      ;
					if ( itm.Subscription != null )
					{
						row["SUBSCRIPTION_USERNAME"         ] = itm.Subscription.Username        ;
						row["SUBSCRIPTION_PASSWORD"         ] = itm.Subscription.Password        ;
						row["SUBSCRIPTION_SUBSCRIPTION_ID"  ] = itm.Subscription.SubscriptionID  ;
						if ( itm.Subscription.SubscriptionDate != null && itm.Subscription.SubscriptionDate != DateTime.MinValue )
							row["SUBSCRIPTION_SUBSCRIPTION_DATE"] = itm.Subscription.SubscriptionDate.ToLocalTime();
						if ( itm.Subscription.EffectiveDate != null && itm.Subscription.EffectiveDate != DateTime.MinValue )
							row["SUBSCRIPTION_EFFECTIVE_DATE"   ] = itm.Subscription.EffectiveDate.ToLocalTime()   ;
						row["SUBSCRIPTION_REATTEMPT"        ] = itm.Subscription.reattempt       ;
						row["SUBSCRIPTION_RECURRENCES"      ] = itm.Subscription.Recurrences     ;
						row["SUBSCRIPTION_RECURRING"        ] = itm.Subscription.recurring       ;
						row["SUBSCRIPTION_RETRY_TIME"       ] = itm.Subscription.RetryTime       ;
						if ( itm.Subscription.Terms != null )
						{
							//row["SUBSCRIPTION_TERMS"            ] = itm.Subscription.Terms           ;
						}
					}
					if ( itm.PaymentItem != null )
					{
						foreach ( PaymentItemType part in itm.PaymentItem )
						{
							DataRow rowPart = dtLineItems.NewRow();
							dtLineItems.Rows.Add(rowPart);
							rowPart["TRANSACTION_ID"   ] = pmt.TransactionID                ;
							rowPart["QUANTITY"         ] = Sql.ToDecimal(part.Quantity    ) ;
							rowPart["NUMBER"           ] = part.Number                      ;
							rowPart["NAME"             ] = part.Name                        ;
							// 12/07/2008 Paul.  The amount might not exist. 
							if ( part.Amount != null )
							{
								rowPart["AMOUNT_CURRENCY"  ] = part.Amount.currencyID.ToString();
								rowPart["AMOUNT"           ] = Sql.ToDecimal(part.Amount.Value) ;
							}
							rowPart["SALES_TAX"        ] = Sql.ToDecimal(part.SalesTax    ) ;
							if ( part.Options != null )
							{
								//rowPart["OPTIONS"          ] = part.Options          ;
							}
						}
					}
				}
			}
			return ds;
		}

		public AbstractResponseType RefundTransaction(string sTransactionID, string sRefundType, string sMemo, Decimal dAmount, string sCurrencyID)
		{
			RefundTransactionReq req = new RefundTransactionReq();
			req.RefundTransactionRequest = new RefundTransactionRequestType();
			req.RefundTransactionRequest.TransactionID       = sTransactionID;
			req.RefundTransactionRequest.RefundType          = RefundType.Full;
			req.RefundTransactionRequest.RefundTypeSpecified = true;
			req.RefundTransactionRequest.Memo                = sMemo;
			if ( !String.IsNullOrEmpty(sRefundType) )
			{
				if ( Enum.IsDefined(typeof(RefundType), sRefundType) )
				{
					req.RefundTransactionRequest.RefundType = (RefundType) Enum.Parse(typeof(RefundType), sRefundType);
					req.RefundTransactionRequest.RefundTypeSpecified = true;
				}
			}
			if ( req.RefundTransactionRequest.RefundType != RefundType.Full )
			{
				req.RefundTransactionRequest.Amount.currencyID = (CurrencyCodeType) Enum.Parse(typeof(CurrencyCodeType), sCurrencyID);
				req.RefundTransactionRequest.Amount.Value      = dAmount.ToString();
			}

			RefundTransactionResponseType resp = api.RefundTransaction(req);
			if ( resp.Ack != AckCodeType.Success && resp.Errors != null && resp.Errors.Length > 0 )
			{
				// 01/14/2008 Paul.  SuccessWithWarning is not an error, so just continue. 
				if ( resp.Ack != AckCodeType.SuccessWithWarning )
					throw(new Exception(ExpandErrors(resp.Errors)));
			}
			return resp;
		}
		#endregion

		public AbstractResponseType DoDirect(DoDirectPaymentRequestType dpr, ref string sTransactionID, ref string sAvsCode, ref string sPaymentStatus)
		{
			DoDirectPaymentReq req = new DoDirectPaymentReq();
			req.DoDirectPaymentRequest = dpr;
			
			DoDirectPaymentResponseType resp = direct.DoDirectPayment(req);
			sTransactionID = resp.TransactionID;
			sAvsCode       = resp.AVSCode;
			if ( resp.PaymentStatusSpecified )
				sPaymentStatus = resp.PaymentStatus.ToString();
			if ( resp.Ack != AckCodeType.Success && resp.Errors != null && resp.Errors.Length > 0 )
			{
				// 01/14/2008 Paul.  SuccessWithWarning is not an error, so just continue. 
				if ( resp.Ack != AckCodeType.SuccessWithWarning )
					throw(new Exception(ExpandErrors(resp.Errors)));
			}
			return resp;
		}
	}
}

