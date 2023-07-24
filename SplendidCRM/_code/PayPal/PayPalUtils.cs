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
using System.Web;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Diagnostics;
using Mono.Security.Cryptography;
using SplendidCRM.PayPal;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for PayPalUtils.
	/// </summary>
	public class PayPalUtils
	{
		public static byte[] MakeCertPKCS12(string sX509PrivateKey, string sX509Certificate, string sPassword)
		{
			Mono.Security.X509.PKCS12 mPKCS12 = new Mono.Security.X509.PKCS12();
			if ( !String.IsNullOrEmpty(sPassword) )
				mPKCS12.Password = sPassword;

			sX509Certificate = sX509Certificate.Trim();
			if ( !String.IsNullOrEmpty(sX509Certificate) )
			{
				const string sCertHeader = "-----BEGIN CERTIFICATE-----";
				const string sCertFooter = "-----END CERTIFICATE-----";
				if (sX509Certificate.StartsWith(sCertHeader) && sX509Certificate.EndsWith(sCertFooter))
				{
					sX509Certificate = sX509Certificate.Substring(sCertHeader.Length, sX509Certificate.Length - sCertHeader.Length - sCertFooter.Length);
					sX509Certificate = sX509Certificate.Trim();
					byte[] byPKS8  = Convert.FromBase64String(sX509Certificate);
					
					Mono.Security.X509.X509Certificate x509 = new Mono.Security.X509.X509Certificate(byPKS8);
					mPKCS12.AddCertificate(x509);
				}
				else
				{
					throw(new Exception("Invalid X509 Certificate.  Missing BEGIN CERTIFICATE or END CERTIFICATE."));
				}
			}

			sX509PrivateKey = sX509PrivateKey.Trim();
			if ( !String.IsNullOrEmpty(sX509PrivateKey) )
			{
				const string sPemHeader = "-----BEGIN RSA PRIVATE KEY-----";
				const string sPemFooter = "-----END RSA PRIVATE KEY-----";
				if (sX509PrivateKey.StartsWith(sPemHeader) && sX509PrivateKey.EndsWith(sPemFooter))
				{
					sX509PrivateKey = sX509PrivateKey.Substring(sPemHeader.Length, sX509PrivateKey.Length - sPemHeader.Length - sPemFooter.Length);
					sX509PrivateKey = sX509PrivateKey.Trim();
					byte[] byKeyPair  = Convert.FromBase64String(sX509PrivateKey);
					
					// 04/05/2013 Paul.  An exception here could be an IIS Application Pool configuration issue. 
					// The system cannot find the file specified.
					// The solution is to edit the Advanced Settings in the Application Pool and enable "Load User Profile". 
					RSA mRSA = PKCS8.PrivateKeyInfo.DecodeRSA(byKeyPair);
					mPKCS12.AddPkcs8ShroudedKeyBag(mRSA);
				}
				else
				{
					throw(new Exception("Invalid X509 Private Key.  Missing BEGIN RSA PRIVATE KEY or END RSA PRIVATE KEY."));
				}
			}
			return mPKCS12.GetBytes();
		}

		// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
		public static void ImportTransaction(HttpApplicationState Application, DataRowView row)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				if ( row != null )
				{
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							string   sTXN_ID                 = Sql.ToString  (row["TRANSACTION_ID"       ]);
							string   sTXN_TYPE               = Sql.ToString  (row["TRANSACTION_TYPE"     ]);
							string   sPAYMENT_STATUS         = Sql.ToString  (row["TRANSACTION_STATUS"   ]);
							DateTime dtPAYMENT_DATE          = Sql.ToDateTime(row["TRANSACTION_DATE"     ]);
							Decimal  dMC_GROSS               = Sql.ToDecimal (row["GROSS_AMOUNT"         ]);
							string   sMC_CURRENCY            = Sql.ToString  (row["GROSS_AMOUNT_CURRENCY"]);
							// 08/26/2010 Paul.  We need a bank fee field to allow for a difference between allocated and received payment. 
							Decimal  dMC_FEE                 = Sql.ToDecimal (row["FEE_AMOUNT"           ]);
							string   sMC_FEE_CURRENCY        = Sql.ToString  (row["FEE_AMOUNT_CURRENCY"  ]);
							string   sPAYER_EMAIL            = Sql.ToString  (row["PAYER"                ]);
							string   sPAYER_BUSINESS_NAME    = Sql.ToString  (row["PAYER_DISPLAY_NAME"   ]);
							string   sADDRESS_STREET         = String.Empty;
							string   sADDRESS_CITY           = String.Empty;
							string   sADDRESS_STATE          = String.Empty;
							string   sADDRESS_ZIP            = String.Empty;
							string   sADDRESS_COUNTRY        = String.Empty;
							string   sCONTACT_PHONE          = String.Empty;

							Guid gCONTACT_ID                     = Guid.Empty;
							Guid gACCOUNT_ID                     = Guid.Empty;
							// 10/16/2008 Paul.  Also create an Order as we already display Order Line Items under Accounts. 
							Guid gORDER_ID                       = Guid.Empty;
							Guid gINVOICE_ID                     = Guid.Empty;
							Guid gPAYMENT_ID                     = Guid.Empty;
							Guid gPAYMENTS_TRANSACTION_ID        = Guid.Empty;
							Guid gPARENT_PAYMENTS_TRANSACTION_ID = Guid.Empty;

							sSQL = "select vwPAYMENTS_TRANSACTIONS.ID              " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.ACCOUNT_ID      " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.CONTACT_ID      " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.PAYMENT_ID      " + ControlChars.CrLf
							     + "     , vwINVOICES_PAYMENTS.INVOICE_ID          " + ControlChars.CrLf
							     + "     , vwINVOICES_PAYMENTS.ORDER_ID            " + ControlChars.CrLf
							     + "  from            vwPAYMENTS_TRANSACTIONS      " + ControlChars.CrLf
							     + "  left outer join vwINVOICES_PAYMENTS          " + ControlChars.CrLf
							     + "               on vwINVOICES_PAYMENTS.PAYMENT_ID = vwPAYMENTS_TRANSACTIONS.PAYMENT_ID" + ControlChars.CrLf
							     + " where vwPAYMENTS_TRANSACTIONS.PAYMENT_GATEWAY    = N'PayPal'          " + ControlChars.CrLf
							     + "   and vwPAYMENTS_TRANSACTIONS.TRANSACTION_NUMBER = @TRANSACTION_NUMBER" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@TRANSACTION_NUMBER", sTXN_ID);
								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										gPAYMENTS_TRANSACTION_ID = Sql.ToGuid(rdr["ID"        ]);
										gPAYMENT_ID              = Sql.ToGuid(rdr["PAYMENT_ID"]);
										// 04/03/2008 Paul.  The Account and Invoice may have been retrieved by the parent transaction. 
										// Don't override the values from the parent transaction. 
										if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
											gACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
										if ( Sql.IsEmptyGuid(gCONTACT_ID) )
										// 09/20/2013 Paul.  In B2C mode, the Contact is not null. 
											gCONTACT_ID = Sql.ToGuid(rdr["CONTACT_ID"]);
										if ( Sql.IsEmptyGuid(gINVOICE_ID) )
											gINVOICE_ID = Sql.ToGuid(rdr["INVOICE_ID"]);
										if ( Sql.IsEmptyGuid(gORDER_ID) )
											gORDER_ID = Sql.ToGuid(rdr["ORDER_ID"  ]);
									}
								}
							}

							if ( Sql.IsEmptyGuid(gCONTACT_ID) )
							{
								// 04/03/2008 Paul. Look for a matching contact. 
								sSQL = "select ID              " + ControlChars.CrLf
								     + "     , ACCOUNT_ID      " + ControlChars.CrLf
								     + "  from vwCONTACTS      " + ControlChars.CrLf
								     + " where EMAIL1 = @EMAIL1" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@EMAIL1", sPAYER_EMAIL);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											gCONTACT_ID = Sql.ToGuid(rdr["ID"        ]);
											gACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
										}
									}
								}
							}
							if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
							{
								// 04/03/2008 Paul.  The Payer Email is more significant than the Business Name. 
								sSQL = "select 1               " + ControlChars.CrLf
								     + "     , ID              " + ControlChars.CrLf
								     + "  from vwACCOUNTS      " + ControlChars.CrLf
								     + " where EMAIL1 = @EMAIL1" + ControlChars.CrLf
								     + "union                  " + ControlChars.CrLf
								     + "select 2               " + ControlChars.CrLf
								     + "     , ID              " + ControlChars.CrLf
								     + "  from vwACCOUNTS      " + ControlChars.CrLf
								     + " where NAME = @NAME    " + ControlChars.CrLf
								     + " order by 1            " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@EMAIL1", sPAYER_EMAIL        );
									Sql.AddParameter(cmd, "@NAME"  , sPAYER_BUSINESS_NAME);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											gACCOUNT_ID = Sql.ToGuid(rdr["ID"]);
										}
									}
								}
							}
							if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
							{
								string sDESCRIPTION = String.Empty;
								// 08/06/2009 Paul.  ACCOUNT_NUMBER now uses our number sequence table. 
								// 04/07/2010 Paul.  Add EXCHANGE_FOLDER. 
								SqlProcs.spACCOUNTS_Update
									( ref gACCOUNT_ID
									, Guid.Empty    // ASSIGNED_USER_ID
									, sPAYER_BUSINESS_NAME
									, "Customer"
									, Guid.Empty    // PARENT_ID
									, String.Empty  // INDUSTRY
									, String.Empty  // ANNUAL_REVENUE
									, String.Empty  // PHONE_FAX
									, sADDRESS_STREET
									, sADDRESS_CITY
									, sADDRESS_STATE
									, sADDRESS_ZIP
									, sADDRESS_COUNTRY
									, sDESCRIPTION
									, String.Empty  // RATING
									, sCONTACT_PHONE
									, String.Empty  // PHONE_ALTERNATE
									, sPAYER_EMAIL  // EMAIL1
									, String.Empty  // EMAIL2
									, String.Empty  // WEBSITE
									, String.Empty  // OWNERSHIP
									, String.Empty  // EMPLOYEES
									, String.Empty  // Store the Payer ID in the SIC_CODE field. 
									, String.Empty  // TICKER_SYMBOL
									, String.Empty  // SHIPPING_ADDRESS_STREET
									, String.Empty  // SHIPPING_ADDRESS_CITY
									, String.Empty  // SHIPPING_ADDRESS_STATE
									, String.Empty  // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty  // SHIPPING_ADDRESS_COUNTRY
									, String.Empty  // ACCOUNT_NUMBER
									, Guid.Empty    // TEAM_ID
									, String.Empty  // TEAM_SET_LIST
									, false         // EXCHANGE_FOLDER
									// 08/07/2015 Paul.  Add picture. 
									, String.Empty  // PICTURE
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty  // TAG_SET_NAME
									// 06/07/2017 Paul.  Add NAICSCodes module. 
									, String.Empty  // NAICS_SET_NAME
									// 10/27/2017 Paul.  Add Accounts as email source. 
									, false         // DO_NOT_CALL
									, false         // EMAIL_OPT_OUT
									, false         // INVALID_EMAIL
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty  // ASSIGNED_SET_LIST
									, trn
									);
								if ( !Sql.IsEmptyGuid(gCONTACT_ID) )
									SqlProcs.spACCOUNTS_CONTACTS_Update(gACCOUNT_ID, gCONTACT_ID, trn);
							}
							if ( Sql.IsEmptyGuid(gCONTACT_ID) )
							{
								string sFIRST_NAME  = String.Empty;
								string sLAST_NAME   = String.Empty  ;
								string[] arrPAYER_BUSINESS_NAME = sPAYER_BUSINESS_NAME.Split(' ');
								if ( arrPAYER_BUSINESS_NAME.Length > 1 )
								{
									sFIRST_NAME  = arrPAYER_BUSINESS_NAME[0];
									sLAST_NAME   = arrPAYER_BUSINESS_NAME[1];
								}
								string sDESCRIPTION = String.Empty;
								SqlProcs.spCONTACTS_Update
									( ref gCONTACT_ID
									, Guid.Empty    // ASSIGNED_USER_ID
									, String.Empty  // SALUTATION
									, sFIRST_NAME
									, sLAST_NAME
									, gACCOUNT_ID
									, String.Empty  // LEAD_SOURCE
									, String.Empty  // TITLE
									, String.Empty  // DEPARTMENT
									, Guid.Empty    // REPORTS_TO_ID
									, DateTime.MinValue  // BIRTHDATE
									, false         // DO_NOT_CALL
									, String.Empty  // PHONE_HOME
									, String.Empty  // PHONE_MOBILE
									, sCONTACT_PHONE // PHONE_WORK
									, String.Empty  // PHONE_OTHER
									, String.Empty  // PHONE_FAX
									, sPAYER_EMAIL  // EMAIL1
									, String.Empty  // EMAIL2
									, String.Empty  // ASSISTANT
									, String.Empty  // ASSISTANT_PHONE
									, false         // EMAIL_OPT_OUT
									, false         // INVALID_EMAIL
									, sADDRESS_STREET
									, sADDRESS_CITY
									, sADDRESS_STATE
									, sADDRESS_ZIP
									, sADDRESS_COUNTRY
									, String.Empty  // ALT_ADDRESS_STREET
									, String.Empty  // ALT_ADDRESS_CITY
									, String.Empty  // ALT_ADDRESS_STATE
									, String.Empty  // ALT_ADDRESS_POSTALCODE
									, String.Empty  // ALT_ADDRESS_COUNTRY
									, sDESCRIPTION
									, String.Empty  // PARENT_TYPE
									, Guid.Empty    // PARENT_ID
									, false         // SYNC_CONTACT
									, Guid.Empty    // TEAM_ID
									, String.Empty  // TEAM_SET_LIST
									// 09/27/2013 Paul.  SMS messages need to be opt-in. 
									, String.Empty  // SMS_OPT_IN
									// 10/22/2013 Paul.  Provide a way to map Tweets to a parent. 
									, String.Empty  // TWITTER_SCREEN_NAME
									// 08/07/2015 Paul.  Add picture. 
									, String.Empty  // PICTURE
									// 08/07/2015 Paul.  Add Leads/Contacts relationship. 
									, Guid.Empty    // LEAD_ID
									// 09/27/2015 Paul.  Separate SYNC_CONTACT and EXCHANGE_FOLDER. 
									, false         // EXCHANGE_FOLDER
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty  // TAG_SET_NAME
									// 06/20/2017 Paul.  Add number fields to Contacts, Leads, Prospects, Opportunities and Campaigns. 
									, String.Empty  // CONTACT_NUMBER
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty  // ASSIGNED_SET_LIST
									// 06/23/2018 Paul.  Add DP_BUSINESS_PURPOSE and DP_CONSENT_LAST_UPDATED for data privacy. 
									, String.Empty       // DP_BUSINESS_PURPOSE
									, DateTime.MinValue  // DP_CONSENT_LAST_UPDATED
									, trn
									);
							}
							// 04/03/2008 Paul.  PayPal always returns USD. 
							// 05/29/2008 Paul.  Use MC_CURRENCY to lookup the currency to prepare for the time when PayPal supports other currencies. 
							// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
							Currency oCurrency = new Currency(Application);
							DataView vwCurrencies = new DataView(SplendidCache.Currencies());
							vwCurrencies.RowFilter = "ISO4217 = '" + Sql.EscapeSQL(sMC_CURRENCY) + "'";
							if ( vwCurrencies.Count > 0 )
								oCurrency = Currency.CreateCurrency(Application, Sql.ToGuid(vwCurrencies[0]["ID"]));
					
							Guid   gSHIPPER_ID = Guid.Empty;
							string sINVOICE    = "PayPal " + sTXN_TYPE + " " + sTXN_ID;
					
							// 10/16/2008 Paul.  Creating an order is nearly identical to creating an invoice. 
							string sORDER       = sINVOICE;
							string sORDER_STAGE = String.Empty;
							switch ( sPAYMENT_STATUS )
							{
								case "Canceled-Reversal":  sORDER_STAGE = "Cancelled" ;  break;
								case "Completed"        :  sORDER_STAGE = "Ordered"   ;  break;
								case "Denied"           :  sORDER_STAGE = "Cancelled" ;  break;
								case "Expired"          :  sORDER_STAGE = "Cancelled" ;  break;
								case "Failed"           :  sORDER_STAGE = "Cancelled" ;  break;
								case "In-Progress"      :  sORDER_STAGE = "Pending"   ;  break;
								case "Pending"          :  sORDER_STAGE = "Pending"   ;  break;
								case "Processed"        :  sORDER_STAGE = "Ordered"   ;  break;
								// 11/11/2008 Paul.  Change stage to Refunded. 
								case "Refunded"         :  sORDER_STAGE = "Refunded"  ;  break;
								case "Reversed"         :  sORDER_STAGE = "Cancelled" ;  break;
							}
							Decimal dSUBTOTAL      = dMC_GROSS;
							Decimal dPAYMENT_GROSS = dMC_GROSS;
							Decimal dTAX           = Decimal.Zero;
							Decimal dMC_SHIPPING   = dMC_GROSS - dSUBTOTAL - dTAX;
							if ( Sql.IsEmptyGuid(gORDER_ID) )
							{
								// 08/06/2009 Paul.  ORDER_NUM now uses our number sequence table. 
								SqlProcs.spORDERS_Update
									( ref gORDER_ID
									, Guid.Empty        // ASSIGNED_USER_ID
									, sORDER            // NAME
									, Guid.Empty        // QUOTE_ID
									, Guid.Empty        // OPPORTUNITY_ID
									, "Due on Receipt"  // PAYMENT_TERMS
									, sORDER_STAGE
									, String.Empty      // PURCHASE_ORDER_NUM
									, dtPAYMENT_DATE    // ORIGINAL_PO_DATE
									, dtPAYMENT_DATE    // DATE_ORDER_DUE
									, DateTime.MinValue // DATE_ORDER_SHIPPED
									, false             // SHOW_LINE_NUMS
									, false             // CALC_GRAND_TOTAL
									, 1.0f              // EXCHANGE_RATE
									, oCurrency.ID      // CURRENCY_ID
									, Guid.Empty        // TAXRATE_ID
									, gSHIPPER_ID       // SHIPPER_ID
									, dSUBTOTAL         // SUBTOTAL
									, Decimal.Zero      // DISCOUNT
									, dMC_SHIPPING      // SHIPPING
									, dTAX              // TAX
									, dMC_GROSS         // TOTAL
									, gACCOUNT_ID       // BILLING_ACCOUNT_ID
									, gCONTACT_ID       // BILLING_CONTACT_ID
									, sADDRESS_STREET   // BILLING_ADDRESS_STREET
									, sADDRESS_CITY     // BILLING_ADDRESS_CITY
									, sADDRESS_STATE    // BILLING_ADDRESS_STATE
									, sADDRESS_ZIP      // BILLING_ADDRESS_POSTALCODE
									, sADDRESS_COUNTRY  // BILLING_ADDRESS_COUNTRY
									, Guid.Empty        // SHIPPING_ACCOUNT_ID
									, Guid.Empty        // SHIPPING_CONTACT_ID
									, String.Empty      // SHIPPING_ADDRESS_STREET
									, String.Empty      // SHIPPING_ADDRESS_CITY
									, String.Empty      // SHIPPING_ADDRESS_STATE
									, String.Empty      // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty      // SHIPPING_ADDRESS_COUNTRY
									, String.Empty      // DESCRIPTION
									, String.Empty      // ORDER_NUM
									, Guid.Empty        // TEAM_ID
									, String.Empty      // TEAM_SET_LIST
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty      // TAG_SET_NAME
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty      // ASSIGNED_SET_LIST
									, trn
									);
							}

							string sINVOICE_STAGE = String.Empty;
							switch ( sPAYMENT_STATUS )
							{
								case "Canceled-Reversal":  sINVOICE_STAGE = "Cancelled" ;  break;
								case "Completed"        :  sINVOICE_STAGE = "Paid"      ;  break;
								case "Denied"           :  sINVOICE_STAGE = "Cancelled" ;  break;
								case "Expired"          :  sINVOICE_STAGE = "Cancelled" ;  break;
								case "Failed"           :  sINVOICE_STAGE = "Cancelled" ;  break;
								case "In-Progress"      :  sINVOICE_STAGE = "Due"       ;  break;
								case "Pending"          :  sINVOICE_STAGE = "Due"       ;  break;
								case "Processed"        :  sINVOICE_STAGE = "Paid"      ;  break;
								// 11/11/2008 Paul.  Change stage to Refunded. 
								case "Refunded"         :  sINVOICE_STAGE = "Refunded"  ;  break;
								case "Reversed"         :  sINVOICE_STAGE = "Cancelled" ;  break;
							}
					
							if ( Sql.IsEmptyGuid(gINVOICE_ID) )
							{
								// 08/06/2009 Paul.  INVOICE_NUM now uses our number sequence table. 
								// 02/27/2015 Paul.  Add SHIP_DATE to sync with QuickBooks. 
								SqlProcs.spINVOICES_Update
									( ref gINVOICE_ID
									, Guid.Empty        // ASSIGNED_USER_ID
									, sINVOICE          // NAME
									, Guid.Empty        // QUOTE_ID
									, gORDER_ID         // ORDER_ID
									, Guid.Empty        // OPPORTUNITY_ID
									, "Due on Receipt"  // PAYMENT_TERMS
									, sINVOICE_STAGE
									, String.Empty      // PURCHASE_ORDER_NUM
									, dtPAYMENT_DATE    // DUE_DATE
									, 1.0f              // EXCHANGE_RATE
									, oCurrency.ID      // CURRENCY_ID
									, Guid.Empty        // TAXRATE_ID
									, gSHIPPER_ID       // SHIPPER_ID
									, dSUBTOTAL         // SUBTOTAL
									, Decimal.Zero      // DISCOUNT
									, dMC_SHIPPING      // SHIPPING
									, dTAX              // TAX
									, dMC_GROSS         // TOTAL
									, Decimal.Zero      // AMOUNT_DUE
									, gACCOUNT_ID       // BILLING_ACCOUNT_ID
									, gCONTACT_ID       // BILLING_CONTACT_ID
									, sADDRESS_STREET   // BILLING_ADDRESS_STREET
									, sADDRESS_CITY     // BILLING_ADDRESS_CITY
									, sADDRESS_STATE    // BILLING_ADDRESS_STATE
									, sADDRESS_ZIP      // BILLING_ADDRESS_POSTALCODE
									, sADDRESS_COUNTRY  // BILLING_ADDRESS_COUNTRY
									, Guid.Empty        // SHIPPING_ACCOUNT_ID
									, Guid.Empty        // SHIPPING_CONTACT_ID
									, String.Empty      // SHIPPING_ADDRESS_STREET
									, String.Empty      // SHIPPING_ADDRESS_CITY
									, String.Empty      // SHIPPING_ADDRESS_STATE
									, String.Empty      // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty      // SHIPPING_ADDRESS_COUNTRY
									, String.Empty      // DESCRIPTION
									, String.Empty      // INVOICE_NUM
									, Guid.Empty        // TEAM_ID
									, String.Empty      // TEAM_SET_LIST
									, DateTime.MinValue // SHIP_DATE
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty      // TAG_SET_NAME
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty      // ASSIGNED_SET_LIST
									, trn
									);
							}

							if ( Sql.IsEmptyGuid(gPAYMENT_ID) )
							{
								// 08/06/2009 Paul.  PAYMENT_NUM now uses our number sequence table. 
								// 08/26/2010 Paul.  We need a bank fee field to allow for a difference between allocated and received payment. 
								// 05/07/2013 Paul.  Add Contacts field to support B2C. 
								Guid gB2C_CONTACT_ID = Guid.Empty;
								SqlProcs.spPAYMENTS_Update
									( ref gPAYMENT_ID
									, Guid.Empty      // ASSIGNED_USER_ID
									, gACCOUNT_ID
									, dtPAYMENT_DATE
									, "PayPal"        // PAYMENT_TYPE
									, String.Empty    // CUSTOMER_REFERENCE
									, 1.0f            // EXCHANGE_RATE
									, oCurrency.ID    // CURRENCY_ID
									, dPAYMENT_GROSS  // AMOUNT
									, String.Empty    // DESCRIPTION
									, Guid.Empty      // CREDIT_CARD_ID
									, String.Empty    // PAYMENT_NUM
									, Guid.Empty      // TEAM_ID
									, String.Empty    // TEAM_SET_LIST
									, dMC_FEE         // BANK_FEE
									, gB2C_CONTACT_ID
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty    // ASSIGNED_SET_LIST
									, trn
									);
								Guid gINVOICES_PAYMENT_ID = Guid.Empty;
								SqlProcs.spINVOICES_PAYMENTS_Update(ref gINVOICES_PAYMENT_ID, gINVOICE_ID, gPAYMENT_ID, dPAYMENT_GROSS, trn);
							}
							if ( Sql.IsEmptyGuid(gPAYMENTS_TRANSACTION_ID) )
							{
								// Canceled-Reversal, Completed, Denied, Expired, Failed, In-Progress, Pending, Processed, Refunded, Reversed, Voided
								// 04/22/2008 Paul.  Change from transaction type of Charge to Sale to match .netCHARGE. 
								string sTRANSACTION_TYPE = sPAYMENT_STATUS;
								switch ( sPAYMENT_STATUS )
								{
									case "Completed":  sTRANSACTION_TYPE = "Sale"  ;  break;
									case "Refunded" :  sTRANSACTION_TYPE = "Refund";  break;
									case "Reversed" :  sTRANSACTION_TYPE = "Refund";  break;
								}
								string sINVOICE_NUMBER = String.Empty;
								sSQL = "select INVOICE_NUM" + ControlChars.CrLf
								     + "  from vwINVOICES " + ControlChars.CrLf
								     + " where ID = @ID   " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gINVOICE_ID);
									sINVOICE_NUMBER = Sql.ToString(cmd.ExecuteScalar());
								}
					
								SqlProcs.spPAYMENTS_TRANSACTIONS_InsertOnly
									( ref gPAYMENTS_TRANSACTION_ID
									, gPAYMENT_ID
									, "PayPal"           // PAYMENT_GATEWAY
									, sTRANSACTION_TYPE  // Sale or Refund. 
									, dPAYMENT_GROSS
									, oCurrency.ID       // CURRENCY_ID
									, sINVOICE_NUMBER    // INVOICE_NUMBER
									, String.Empty       // DESCRIPTION
									, Guid.Empty         // CREDIT_CARD_ID
									, gACCOUNT_ID
									, sPAYMENT_STATUS    // STATUS
									, trn
									);
								SqlProcs.spPAYMENTS_TRANSACTIONS_Update
									( gPAYMENTS_TRANSACTION_ID
									, sPAYMENT_STATUS    // STATUS
									, sTXN_ID            // TRANSACTION_NUMBER
									, String.Empty       // REFERENCE_NUMBER
									, String.Empty       // AUTHORIZATION_CODE
									, String.Empty       // AVS_CODE
									, String.Empty       // ERROR_CODE
									, String.Empty       // ERROR_MESSAGE
									, trn
									);
							}
							trn.Commit();
						}
						catch
						{
							trn.Rollback();
							throw;
						}
					}
				}
			}
		}

		// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
		public static void ImportTransaction(HttpApplicationState Application, DataSet ds)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;

				DataTable dt = ds.Tables["TRANSACTIONS"];
				if ( dt.Rows.Count > 0 )
				{
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							DataRow row = dt.Rows[0];
							
							// https://www.paypal.com/IntegrationCenter/ic_ipn-pdt-variable-reference.html
							// IPN and PDT Variables: Basic Information
							string   sADDRESS_CITY           = Sql.ToString  (row["PAYER_ADDRESS_CITY"        ]);  // City of customer's address.
							string   sADDRESS_COUNTRY        = Sql.ToString  (row["PAYER_ADDRESS_COUNTRY_NAME"]);  // Country of customer's address.
							string   sADDRESS_COUNTRY_CODE   = Sql.ToString  (row["PAYER_ADDRESS_COUNTRY"     ]);  // Two-character ISO 3166 country code
							string   sADDRESS_NAME           = Sql.ToString  (row["PAYER_ADDRESS_NAME"        ]);  // Name used with address (included when the customer provides a Gift Address)
							string   sADDRESS_STATE          = Sql.ToString  (row["PAYER_ADDRESS_STATE"       ]);  // State of customer's address 
							string   sADDRESS_STATUS         = Sql.ToString  (row["PAYER_ADDRESS_STATUS"      ]);  // (confirmed unconfirmed)
							string   sADDRESS_STREET         = Sql.ToString  (row["PAYER_ADDRESS_STREET1"     ]);  // Customer's street address.
							string   sADDRESS_ZIP            = Sql.ToString  (row["PAYER_ADDRESS_POSTAL_CODE" ]);  // ZIP code of customer's address.
							string   sSALUTATION             = Sql.ToString  (row["PAYER_SALUATION"           ]);
							string   sFIRST_NAME             = Sql.ToString  (row["PAYER_FIRST_NAME"          ]);  // Customer's first name
							string   sLAST_NAME              = Sql.ToString  (row["PAYER_LAST_NAME"           ]);  // Customer's last name
							string   sPAYER_BUSINESS_NAME    = Sql.ToString  (row["PAYER_BUSINESS"            ]);  // Customer's company name, if customer represents a business
							string   sPAYER_EMAIL            = Sql.ToString  (row["PAYER"                     ]);  // Customer's primary email address. Use this email to provide any credits.
							string   sPAYER_ID               = Sql.ToString  (row["PAYER_ID"                  ]);  // Unique customer ID.
							string   sPAYER_STATUS           = Sql.ToString  (row["PAYER_STATUS"              ]);  // (verified unverified) Customer has a verified PayPal account.
							string   sRESIDENCE_COUNTRY      = Sql.ToString  (row["PAYER_COUNTRY"             ]);  // Two-character ISO 3166 country code
							string   sCONTACT_PHONE          = Sql.ToString  (row["PAYER_PHONE"               ]);  // Customer's telephone number
					
							// IPN and PDT Variables: Basic Information
							string   sBUSINESS               = Sql.ToString  (row["RECEIVER_BUSINESS"         ]);  // Email address or account ID of the payment recipient (that is, the merchant). Equivalent to the values of receiver_email (if payment is sent to primary account) and business set in the Website Payment HTML.
							//string   sITEM_NAME              = Sql.ToString  (row["item_name"                 ]);  // Item name as passed by you, the merchant. Or, if not passed by you, as entered by your customer. If this is a shopping cart transaction, PayPal will append the number of the item (e.g., item_name1, item_name2).
							//string   sITEM_NUMBER            = Sql.ToString  (row["item_number"               ]);  // Pass-through variable for you to track purchases. It will get passed back to you at the completion of the payment. If omitted, no variable will be passed back to you.
							//int      nQUANTITY               = Sql.ToInteger (row["quantity"                  ]);  // Quantity as entered by your customer or as passed by you, the merchant. If this is a shopping cart transaction, PayPal appends the number of the item (e.g. quantity1, quantity2).  
							string   sRECEIVER_EMAIL         = Sql.ToString  (row["RECEIVER"                  ]);  // Primary email address of the payment recipient (that is, the merchant). If the payment is sent to a non-primary email address on your PayPal account, the receiver_email is still your primary email.
							string   sRECEIVER_ID            = Sql.ToString  (row["RECEIVER_ID"               ]);  // Unique account ID of payment recipient
					
							// IPN and PDT Variables: Advanced and Custom Information
							string   sCUSTOM                 = Sql.ToString  (row["CUSTOM"                    ]);  // Custom value as passed by you, the merchant. These are pass-through variables that are never presented to your customer. 
							string   sINVOICE                = Sql.ToString  (row["INVOICE_ID"                ]);  // Variable you can use to identify your Invoice Number for this purchase. If omitted, no variable is passed back. Must be unique per transaction. PayPal will prevent a payment if the value has been used previously. 
							string   sMEMO                   = Sql.ToString  (row["MEMO"                      ]);  // Memo as entered by your customer in PayPal Website Payments note field. 
							//string   sTAX                   = Sql.ToString  (row["SALES_TAX"                  ]);  // Amount of tax charged on payment. 2 decimal places.
					
							// IPN and PDT Variables: Website Payments Standard, Website Payments Pro, and Refund Information
							//string   sAUTH_ID                = Sql.ToString  (row["auth_id"                   ]);   // Transaction-specific Authorization identification number 
							//string   sAUTH_EXP               = Sql.ToString  (row["auth_exp"                  ]);   // Transaction-specific Authorization expiration date and time 
							//string   sAUTH_STATUS            = Sql.ToString  (row["auth_status"               ]);   // (Completed Pending Voided) Status of authorization 
							//Decimal  dMC_GROSS               = Sql.ToDecimal (row["mc_gross"                  ]);   // Transaction-specific for multiple currencies. The amount is in the currency of mc_currency, where x is the shopping cart detail item number. The sum of mc_gross_x should total mc_gross. 
							//Decimal  dMC_HANDLING            = Sql.ToDecimal (row["mc_handling"               ]);   // Transaction-specific for multiple currencies. This is the combined total of shipping and shipping2 WebsitePayments variables, where x is the shopping cart detail item number. The shippingx variable is only shown when the merchant applies a shipping amount for a specific item. Because profile shipping might apply, the sum of shippingx might not be equal to shipping. 
							string   sPARENT_TXN_ID          = Sql.ToString  (row["PARENT_TRANSACTION_ID"     ]);   // In the case of a refund, reversal, or canceled reversal, this variable contains the txn_id of the original transaction, while txn_id contains a new ID for the new transaction.
							DateTime dtPAYMENT_DATE          = Sql.ToDateTime(row["PAYMENT_DATE"              ]) ;   // Time/Date stamp generated by PayPal [format: "18:30:30 Jan 1, 2000 PST"] 
					
							string   sPAYMENT_STATUS         = Sql.ToString  (row["PAYMENT_STATUS"            ]);   // (Canceled-Reversal Completed Denied Expired Failed In-Progress Pending Processed Refunded Reversed)
							//                        Voided The status of the payment:
							//                        Canceled-Reversal: A reversal has been canceled. For example, you won a dispute with the customer, and the funds for the transaction that was reversed have been returned to you.
							//                        Completed: The payment has been completed, and the funds have been added successfully to your account balance.
							//                        Denied: You denied the payment. This happens only if the payment was previously pending because of possible reasons described for the PendingReason element.
							//                        Expired: This authorization has expired and cannot be captured.
							//                        Failed: The payment has failed. This happens only if the payment was made from your customer's bank account.
							//                        In-Progress: The transaction is in process of authorization and capture.
							//                        Pending: The payment is pending. See pending_ re for more information.
							//                        Refunded: You refunded the payment.
							//                        Reversed: A payment was reversed due to a chargeback or other type of reversal. The funds have been removed from your account balance and returned to the buyer. The reason for the reversal is specified in the ReasonCode element.
							//                        Processed: A payment has been accepted.
							//                        Voided: This authorization has been voided. 
							string   sPAYMENT_TYPE           = Sql.ToString  (row["PAYMENT_TYPE"              ]);   // (echeck instant)
							//                        echeck: This payment was funded with an eCheck.
							//                        instant: This payment was funded with PayPal balance, credit card, or Instant Transfer. 
							string   sPENDING_REASON         = Sql.ToString  (row["PENDING_REASON"            ]);   // (address authorization echeck intl multi-currency unilateral upgrade verify other)
							//                        This variable is set only if payment_status = Pending.
							//                        address: The payment is pending because your customer did not include a confirmed shipping address and your Payment Receiving Preferences is set to allow you to manually accept or deny each of these payments. To change your preference, go to the Preferences section of your Profile.
							//                        authorization: You set <PaymentAction> Authorization</PaymentAction> on SetExpressCheckoutRequest and have not yet captured funds.
							//                        echeck: The payment is pending because it was made by an eCheck that has not yet cleared.
							//                        intl: The payment is pending because you hold a non-U.S. account and do not have a withdrawal mechanism. You must manually accept or deny this payment from your Account Overview.
							//                        multi-currency: You do not have a balance in the currency sent, and you do not have your Payment Receiving Preferences set to automatically convert and accept this payment. You must manually accept or deny this payment.
							//                        unilateral: The payment is pending because it was made to an email address that is not yet registered or confirmed.
							//                        upgrade: The payment is pending because it was made via credit card and you must upgrade your account to Business or Premier status in order to receive the funds. upgrade can also mean that you have reached the monthly limit for transactions on your account.
							//                        verify: The payment is pending because you are not yet verified. You must verify your account before you can accept this payment.
							//                        other: The payment is pending for a reason other than those listed above. For more information, contact PayPal Customer Service.
							string   sREASON_CODE            = Sql.ToString  (row["REASON_CODE"               ]);   // (chargeback guarantee buyer-complaint refund other)
							//                        This variable is only set if payment_status =Reversed or Refunded.
							//                        chargeback: A reversal has occurred on this transaction due to a chargeback by your customer.
							//                        guarantee: A reversal has occurred on this transaction due to your customer triggering a money-back guarantee.
							//                        buyer-complaint: A reversal has occurred on this transaction due to a complaint about the transaction from your customer.
							//                        refund: A reversal has occurred on this transaction because you have given the customer a refund.
							//                        other: A reversal has occurred on this transaction due to a reason not listed above. 
							//string   sSHIPPING_METHOD        = Sql.ToString  (row["shipping_method"           ]);   // Merchant-specific The name of a shipping method from the Shipping Calculations section of the merchant's account profile. The buyer selected the named shipping method for this transaction. 
							// 05/29/2008 Paul.  dSHIPPING is not always used. 
							//Decimal  dSHIPPING               = Sql.ToDecimal (row["shipping"                  ]);   // Transaction-specific Shipping charges associated with this transaction. Format: unsigned, no currency symbol, two decimal places. 
							Decimal  dTAX                    = Sql.ToDecimal (row["SALES_TAX"                 ]);   // PayPal appends the number of the item (e.g., item_name1, item_name2). The tax_x variable is included only if there was a specific tax amount applied to a particular shopping cart item. Because profile tax may apply to other items in the cart, the sum of tax_x might not total to tax. 
							//string   sTRANSACTION_ENTITY     = Sql.ToString  (row["transaction_entity"        ]);   // (auth reauth order payment) Authorization and Capture transaction entity 
							string   sTXN_ID                 = Sql.ToString  (row["TRANSACTION_ID"            ]);   // A unique transaction ID generated by PayPal.Character length and limitations: 17 
							string   sTXN_TYPE               = Sql.ToString  (row["TRANSACTION_TYPE"          ]);   // (cart express_ checkout merch_pmt send_money virtual_terminal web_accept)
							string   sRECEIPT_ID             = Sql.ToString  (row["RECEIPT_ID"                ]);
							//string   sVERIFY_SIGN            = Sql.ToString  (row["verify_sign"               ]);
							//                        cart: Transaction created by a customer:
							//                         * Via the PayPal Shopping Cart feature.
							//                         * Via Express Checkout when the cart contains multiple items.
							//                        express_checkout: Transaction created by Express Checkout when the customer's cart contains a single item.
							//                        merch_pmt: Website Payments Pro monthly billing fee.
							//                        send-money: Transaction created by customer from the Send Money tab on the PayPal website.
							//                        virtual_terminal: Transaction created with Virtual Terminal.
							//                        web_accept: Transaction created by customer via Buy Now, Donation, or Auction Smart Logos. 
							// 05/29/208 Paul.  dAUTH_AMOUNT is not always used. 
							//Decimal  dAUTH_AMOUNT            = Sql.ToDecimal (row["auth_amount"               ]);   // Transaction-specific Authorization amount 
					
							// IPN and PDT Variables: Currency and Currency Exchange Information
							float    fEXCHANGE_RATE          = Sql.ToFloat   (row["EXCHANGE_RATE"             ]);  // Exchange rate used if a currency conversion occurred. 
							string   sMC_CURRENCY            = Sql.ToString  (row["GROSS_AMOUNT_CURRENCY"     ]);  // See table of supported currencies. For payment IPNs, this is the currency of the payment.
							Decimal  dMC_FEE                 = Sql.ToDecimal (row["FEE_AMOUNT"                ]);  // Transaction fee associated with the payment. mc_gross minus mc_fee will equal the amount deposited into the receiver_email account. Equivalent to payment_fee for USD payments. If this amount is negative, it signifies a refund or reversal, and either of those payment statuses can be for the full or partial amount of the original transaction fee. 
							Decimal  dMC_GROSS               = Sql.ToDecimal (row["GROSS_AMOUNT"              ]);  // Full amount of the customer's payment, before transaction fee is subtracted. Equivalent to payment_gross for USD payments. If this amount is negative, it signifies a refund or reversal, and either of those payment statuses can be for the full or partial amount of the original transaction. 
							Decimal  dPAYMENT_FEE            = Sql.ToDecimal (row["FEE_AMOUNT"                ]);  // Transaction-specific for USD payments only USD transaction fee associated with the payment. payment_gross minus payment_fee equals the amount deposited into the receiver email account. Is empty for non-USD payments. This is a legacy field replaced by mc_fee. If this amount is negative, it signifies a refund or reversal, and either of those payment statuses can be for the full or partial amount of the original transaction fee. 
							Decimal  dPAYMENT_GROSS          = Sql.ToDecimal (row["GROSS_AMOUNT"              ]);  // Transaction-specific for USD payments only Full USD amount of the customer's payment, before transaction fee is subtracted. Will be empty for non-USD payments. This is a legacy field replaced by mc_gross. If this amount is negative, it signifies a refund or reversal, and either of those payment statuses can be for the full or partial amount of the original transaction. 
							Decimal  dSETTLE_AMOUNT          = Sql.ToDecimal (row["SETTLE_AMOUNT"             ]);  // Amount that is deposited into the account's primary balance after a currency conversion from automatic conversion (through your Payment Receiving Preferences) or manual conversion (through manually accepting a payment). 
							string   sSETTLE_CURRENCY        = Sql.ToString  (row["SETTLE_AMOUNT_CURRENCY"    ]);  // Currency of settle_amount. 
					
							Guid gCONTACT_ID                     = Guid.Empty;
							Guid gACCOUNT_ID                     = Guid.Empty;
							// 10/16/2008 Paul.  Also create an Order as we already display Order Line Items under Accounts. 
							Guid gORDER_ID                       = Guid.Empty;
							Guid gINVOICE_ID                     = Guid.Empty;
							Guid gPAYMENT_ID                     = Guid.Empty;
							Guid gPAYMENTS_TRANSACTION_ID        = Guid.Empty;
							Guid gPARENT_PAYMENTS_TRANSACTION_ID = Guid.Empty;
							// 04/03/2008 Paul.  First look for a parent transaction. 
							sSQL = "select vwPAYMENTS_TRANSACTIONS.ID              " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.ACCOUNT_ID      " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.CONTACT_ID      " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.PAYMENT_ID      " + ControlChars.CrLf
							     + "     , vwINVOICES_PAYMENTS.INVOICE_ID          " + ControlChars.CrLf
							     + "     , vwINVOICES_PAYMENTS.ORDER_ID            " + ControlChars.CrLf
							     + "  from            vwPAYMENTS_TRANSACTIONS      " + ControlChars.CrLf
							     + "  left outer join vwINVOICES_PAYMENTS          " + ControlChars.CrLf
							     + "               on vwINVOICES_PAYMENTS.PAYMENT_ID = vwPAYMENTS_TRANSACTIONS.PAYMENT_ID" + ControlChars.CrLf
							     + " where vwPAYMENTS_TRANSACTIONS.PAYMENT_GATEWAY    = N'PayPal'          " + ControlChars.CrLf
							     + "   and vwPAYMENTS_TRANSACTIONS.TRANSACTION_NUMBER = @TRANSACTION_NUMBER" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@TRANSACTION_NUMBER", sPARENT_TXN_ID);
								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										gPARENT_PAYMENTS_TRANSACTION_ID = Sql.ToGuid(rdr["ID"        ]);
										gACCOUNT_ID                     = Sql.ToGuid(rdr["ACCOUNT_ID"]);
										// 09/20/2013 Paul.  In B2C mode, the Contact is not null. 
										gCONTACT_ID                     = Sql.ToGuid(rdr["CONTACT_ID"]);
										gINVOICE_ID                     = Sql.ToGuid(rdr["INVOICE_ID"]);
										gORDER_ID                       = Sql.ToGuid(rdr["ORDER_ID"  ]);
									}
								}
							}
							// 04/03/2008 Paul.  Now look for the current transaction.  This is to prevent duplicate data. 
							sSQL = "select vwPAYMENTS_TRANSACTIONS.ID              " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.ACCOUNT_ID      " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.CONTACT_ID      " + ControlChars.CrLf
							     + "     , vwPAYMENTS_TRANSACTIONS.PAYMENT_ID      " + ControlChars.CrLf
							     + "     , vwINVOICES_PAYMENTS.INVOICE_ID          " + ControlChars.CrLf
							     + "     , vwINVOICES_PAYMENTS.ORDER_ID            " + ControlChars.CrLf
							     + "  from            vwPAYMENTS_TRANSACTIONS      " + ControlChars.CrLf
							     + "  left outer join vwINVOICES_PAYMENTS          " + ControlChars.CrLf
							     + "               on vwINVOICES_PAYMENTS.PAYMENT_ID = vwPAYMENTS_TRANSACTIONS.PAYMENT_ID" + ControlChars.CrLf
							     + " where vwPAYMENTS_TRANSACTIONS.PAYMENT_GATEWAY    = N'PayPal'          " + ControlChars.CrLf
							     + "   and vwPAYMENTS_TRANSACTIONS.TRANSACTION_NUMBER = @TRANSACTION_NUMBER" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@TRANSACTION_NUMBER", sTXN_ID);
								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										gPAYMENTS_TRANSACTION_ID = Sql.ToGuid(rdr["ID"        ]);
										gPAYMENT_ID              = Sql.ToGuid(rdr["PAYMENT_ID"]);
										// 04/03/2008 Paul.  The Account and Invoice may have been retrieved by the parent transaction. 
										// Don't override the values from the parent transaction. 
										if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
											gACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
										// 09/20/2013 Paul.  In B2C mode, the Contact is not null. 
										if ( Sql.IsEmptyGuid(gCONTACT_ID) )
											gCONTACT_ID = Sql.ToGuid(rdr["CONTACT_ID"]);
										if ( Sql.IsEmptyGuid(gINVOICE_ID) )
											gINVOICE_ID = Sql.ToGuid(rdr["INVOICE_ID"]);
										if ( Sql.IsEmptyGuid(gORDER_ID) )
											gORDER_ID = Sql.ToGuid(rdr["ORDER_ID"  ]);
									}
								}
							}
					
							if ( Sql.IsEmptyGuid(gCONTACT_ID) )
							{
								// 04/03/2008 Paul. Look for a matching contact. 
								sSQL = "select ID              " + ControlChars.CrLf
								     + "     , ACCOUNT_ID      " + ControlChars.CrLf
								     + "  from vwCONTACTS      " + ControlChars.CrLf
								     + " where EMAIL1 = @EMAIL1" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@EMAIL1", sPAYER_EMAIL);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											gCONTACT_ID = Sql.ToGuid(rdr["ID"        ]);
											gACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
										}
									}
								}
							}
							if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
							{
								// 04/03/2008 Paul.  The Payer Email is more significant than the Business Name. 
								sSQL = "select 1               " + ControlChars.CrLf
								     + "     , ID              " + ControlChars.CrLf
								     + "  from vwACCOUNTS      " + ControlChars.CrLf
								     + " where EMAIL1 = @EMAIL1" + ControlChars.CrLf
								     + "union                  " + ControlChars.CrLf
								     + "select 2               " + ControlChars.CrLf
								     + "     , ID              " + ControlChars.CrLf
								     + "  from vwACCOUNTS      " + ControlChars.CrLf
								     + " where NAME = @NAME    " + ControlChars.CrLf
								     + " order by 1            " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@EMAIL1", sPAYER_EMAIL        );
									Sql.AddParameter(cmd, "@NAME"  , sPAYER_BUSINESS_NAME);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											gACCOUNT_ID = Sql.ToGuid(rdr["ID"]);
										}
									}
								}
							}
							if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
							{
								if ( Sql.IsEmptyString(sPAYER_BUSINESS_NAME) )
									sPAYER_BUSINESS_NAME = sFIRST_NAME + " " + sLAST_NAME;
								string sDESCRIPTION = "PayPal ID: " + sPAYER_ID       + ControlChars.CrLf 
								                    + "Address: "   + sADDRESS_STATUS + ControlChars.CrLf 
								                    + "Payer: "     + sPAYER_STATUS   + ControlChars.CrLf;
								// 08/06/2009 Paul.  ACCOUNT_NUMBER now uses our number sequence table. 
								// 04/07/2010 Paul.  Add EXCHANGE_FOLDER. 
								SqlProcs.spACCOUNTS_Update
									( ref gACCOUNT_ID
									, Guid.Empty    // ASSIGNED_USER_ID
									, sPAYER_BUSINESS_NAME
									, "Customer"
									, Guid.Empty    // PARENT_ID
									, String.Empty  // INDUSTRY
									, String.Empty  // ANNUAL_REVENUE
									, String.Empty  // PHONE_FAX
									, sADDRESS_STREET
									, sADDRESS_CITY
									, sADDRESS_STATE
									, sADDRESS_ZIP
									, sADDRESS_COUNTRY
									, sDESCRIPTION
									, String.Empty  // RATING
									, sCONTACT_PHONE
									, String.Empty  // PHONE_ALTERNATE
									, sPAYER_EMAIL  // EMAIL1
									, String.Empty  // EMAIL2
									, String.Empty  // WEBSITE
									, String.Empty  // OWNERSHIP
									, String.Empty  // EMPLOYEES
									, sPAYER_ID     // Store the Payer ID in the SIC_CODE field. 
									, String.Empty  // TICKER_SYMBOL
									, String.Empty  // SHIPPING_ADDRESS_STREET
									, String.Empty  // SHIPPING_ADDRESS_CITY
									, String.Empty  // SHIPPING_ADDRESS_STATE
									, String.Empty  // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty  // SHIPPING_ADDRESS_COUNTRY
									, String.Empty  // ACCOUNT_NUMBER
									, Guid.Empty    // TEAM_ID
									, String.Empty  // TEAM_SET_LIST
									, false         // EXCHANGE_FOLDER
									// 08/07/2015 Paul.  Add picture. 
									, String.Empty  // PICTURE
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty  // TAG_SET_NAME
									// 06/07/2017 Paul.  Add NAICSCodes module. 
									, String.Empty  // NAICS_SET_NAME
									// 10/27/2017 Paul.  Add Accounts as email source. 
									, false         // DO_NOT_CALL
									, false         // EMAIL_OPT_OUT
									, false         // INVALID_EMAIL
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty  // ASSIGNED_SET_LIST
									, trn
									);
								if ( !Sql.IsEmptyGuid(gCONTACT_ID) )
									SqlProcs.spACCOUNTS_CONTACTS_Update(gACCOUNT_ID, gCONTACT_ID, trn);
							}
							if ( Sql.IsEmptyGuid(gCONTACT_ID) )
							{
								string sDESCRIPTION = "PayPal ID: " + sPAYER_ID       + ControlChars.CrLf 
								                    + "Address: "   + sADDRESS_STATUS + ControlChars.CrLf 
								                    + "Payer: "     + sPAYER_STATUS   + ControlChars.CrLf;
								SqlProcs.spCONTACTS_Update
									( ref gCONTACT_ID
									, Guid.Empty    // ASSIGNED_USER_ID
									, sSALUTATION
									, sFIRST_NAME
									, sLAST_NAME
									, gACCOUNT_ID
									, String.Empty  // LEAD_SOURCE
									, String.Empty  // TITLE
									, String.Empty  // DEPARTMENT
									, Guid.Empty    // REPORTS_TO_ID
									, DateTime.MinValue  // BIRTHDATE
									, false         // DO_NOT_CALL
									, String.Empty  // PHONE_HOME
									, String.Empty  // PHONE_MOBILE
									, sCONTACT_PHONE // PHONE_WORK
									, String.Empty  // PHONE_OTHER
									, String.Empty  // PHONE_FAX
									, sPAYER_EMAIL  // EMAIL1
									, String.Empty  // EMAIL2
									, String.Empty  // ASSISTANT
									, String.Empty  // ASSISTANT_PHONE
									, false         // EMAIL_OPT_OUT
									, false         // INVALID_EMAIL
									, sADDRESS_STREET
									, sADDRESS_CITY
									, sADDRESS_STATE
									, sADDRESS_ZIP
									, sADDRESS_COUNTRY
									, String.Empty  // ALT_ADDRESS_STREET
									, String.Empty  // ALT_ADDRESS_CITY
									, String.Empty  // ALT_ADDRESS_STATE
									, String.Empty  // ALT_ADDRESS_POSTALCODE
									, String.Empty  // ALT_ADDRESS_COUNTRY
									, sDESCRIPTION
									, String.Empty  // PARENT_TYPE
									, Guid.Empty    // PARENT_ID
									, false         // SYNC_CONTACT
									, Guid.Empty    // TEAM_ID
									, String.Empty  // TEAM_SET_LIST
									// 09/27/2013 Paul.  SMS messages need to be opt-in. 
									, String.Empty  // SMS_OPT_IN
									// 10/22/2013 Paul.  Provide a way to map Tweets to a parent. 
									, String.Empty  // TWITTER_SCREEN_NAME
									// 08/07/2015 Paul.  Add picture. 
									, String.Empty  // PICTURE
									// 08/07/2015 Paul.  Add Leads/Contacts relationship. 
									, Guid.Empty    // LEAD_ID
									// 09/27/2015 Paul.  Separate SYNC_CONTACT and EXCHANGE_FOLDER. 
									, false         // EXCHANGE_FOLDER
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty  // TAG_SET_NAME
									// 06/20/2017 Paul.  Add number fields to Contacts, Leads, Prospects, Opportunities and Campaigns. 
									, String.Empty  // CONTACT_NUMBER
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty  // ASSIGNED_SET_LIST
									// 06/23/2018 Paul.  Add DP_BUSINESS_PURPOSE and DP_CONSENT_LAST_UPDATED for data privacy. 
									, String.Empty       // DP_BUSINESS_PURPOSE
									, DateTime.MinValue  // DP_CONSENT_LAST_UPDATED
									);
							}
							// 04/03/2008 Paul.  PayPal always returns USD. 
							// 05/29/2008 Paul.  Use MC_CURRENCY to lookup the currency to prepare for the time when PayPal supports other currencies. 
							// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
							Currency oCurrency = new Currency(Application);
							DataView vwCurrencies = new DataView(SplendidCache.Currencies());
							vwCurrencies.RowFilter = "ISO4217 = '" + Sql.EscapeSQL(sMC_CURRENCY) + "'";
							if ( vwCurrencies.Count > 0 )
								oCurrency = Currency.CreateCurrency(Application, Sql.ToGuid(vwCurrencies[0]["ID"]));
					
							Guid gSHIPPER_ID = Guid.Empty;
							if ( Sql.IsEmptyString(sINVOICE) )
								sINVOICE = "PayPal " + sTXN_TYPE + " " + sTXN_ID;
					
							// 10/16/2008 Paul.  Creating an order is nearly identical to creating an invoice. 
							string sORDER       = sINVOICE;
							string sORDER_STAGE = String.Empty;
							switch ( sPAYMENT_STATUS )
							{
								case "Canceled-Reversal":  sORDER_STAGE = "Cancelled" ;  break;
								case "Completed"        :  sORDER_STAGE = "Ordered"   ;  break;
								case "Denied"           :  sORDER_STAGE = "Cancelled" ;  break;
								case "Expired"          :  sORDER_STAGE = "Cancelled" ;  break;
								case "Failed"           :  sORDER_STAGE = "Cancelled" ;  break;
								case "In-Progress"      :  sORDER_STAGE = "Pending"   ;  break;
								case "Pending"          :  sORDER_STAGE = "Pending"   ;  break;
								case "Processed"        :  sORDER_STAGE = "Ordered"   ;  break;
								// 11/11/2008 Paul.  Change stage to Refunded. 
								case "Refunded"         :  sORDER_STAGE = "Refunded"  ;  break;
								case "Reversed"         :  sORDER_STAGE = "Cancelled" ;  break;
							}
					
							DataTable dtLINE_ITEMS = ds.Tables["LINE_ITEMS"];
							int nNUM_CART_ITEMS = dtLINE_ITEMS.Rows.Count;
							Decimal dSUBTOTAL = Decimal.Zero;
							foreach ( DataRow rowItem in dtLINE_ITEMS.Rows )
							{
								Decimal dUNIT_USDOLLAR = Sql.ToDecimal(rowItem["AMOUNT"]);
								dSUBTOTAL += dUNIT_USDOLLAR;
							}
							Decimal dMC_SHIPPING = dMC_GROSS - dSUBTOTAL - dTAX;
							if ( Sql.IsEmptyGuid(gORDER_ID) )
							{
								// 08/06/2009 Paul.  ORDER_NUM now uses our number sequence table. 
								SqlProcs.spORDERS_Update
									( ref gORDER_ID
									, Guid.Empty        // ASSIGNED_USER_ID
									, sORDER            // NAME
									, Guid.Empty        // QUOTE_ID
									, Guid.Empty        // OPPORTUNITY_ID
									, "Due on Receipt"  // PAYMENT_TERMS
									, sORDER_STAGE
									, sRECEIPT_ID       // PURCHASE_ORDER_NUM
									, dtPAYMENT_DATE    // ORIGINAL_PO_DATE
									, dtPAYMENT_DATE    // DATE_ORDER_DUE
									, DateTime.MinValue // DATE_ORDER_SHIPPED
									, false             // SHOW_LINE_NUMS
									, false             // CALC_GRAND_TOTAL
									, 1.0f              // EXCHANGE_RATE
									, oCurrency.ID      // CURRENCY_ID
									, Guid.Empty        // TAXRATE_ID
									, gSHIPPER_ID       // SHIPPER_ID
									, dSUBTOTAL         // SUBTOTAL
									, Decimal.Zero      // DISCOUNT
									, dMC_SHIPPING      // SHIPPING
									, dTAX              // TAX
									, dMC_GROSS         // TOTAL
									, gACCOUNT_ID       // BILLING_ACCOUNT_ID
									, gCONTACT_ID       // BILLING_CONTACT_ID
									, sADDRESS_STREET   // BILLING_ADDRESS_STREET
									, sADDRESS_CITY     // BILLING_ADDRESS_CITY
									, sADDRESS_STATE    // BILLING_ADDRESS_STATE
									, sADDRESS_ZIP      // BILLING_ADDRESS_POSTALCODE
									, sADDRESS_COUNTRY  // BILLING_ADDRESS_COUNTRY
									, Guid.Empty        // SHIPPING_ACCOUNT_ID
									, Guid.Empty        // SHIPPING_CONTACT_ID
									, String.Empty      // SHIPPING_ADDRESS_STREET
									, String.Empty      // SHIPPING_ADDRESS_CITY
									, String.Empty      // SHIPPING_ADDRESS_STATE
									, String.Empty      // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty      // SHIPPING_ADDRESS_COUNTRY
									, sMEMO             // DESCRIPTION
									, String.Empty      // ORDER_NUM
									, Guid.Empty        // TEAM_ID
									, String.Empty      // TEAM_SET_LIST
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty      // TAG_SET_NAME
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty      // ASSIGNED_SET_LIST
									, trn
									);
					
								if ( nNUM_CART_ITEMS > 0 )
								{
									sSQL = "select *                          " + ControlChars.CrLf
									    + "  from vwPRODUCT_CATALOG           " + ControlChars.CrLf
									    + " where MFT_PART_NUM = @MFT_PART_NUM" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.Transaction = trn;
										cmd.CommandText = sSQL;
										IDbDataParameter parMFT_PART_NUM = Sql.AddParameter(cmd, "@MFT_PART_NUM", String.Empty);
										int nPOSITION = 1;
										foreach ( DataRow rowItem in dtLINE_ITEMS.Rows )
										{
											Guid    gPRODUCT_TEMPLATE_ID = Guid.Empty;
											string  sPRODUCT_NAME        = Sql.ToString (rowItem["NAME"     ]);
											string  sMFT_PART_NUM        = Sql.ToString (rowItem["NUMBER"   ]);
											Decimal dITEM_TAX            = Sql.ToDecimal(rowItem["SALES_TAX"]);
											int     nITEM_QUANTITY       = Sql.ToInteger(rowItem["QUANTITY" ]);
											string  sVENDOR_PART_NUM     = String.Empty;
											string  sTAX_CLASS           = String.Empty;
											Decimal dCOST_PRICE          = Decimal.Zero;
											Decimal dCOST_USDOLLAR       = Decimal.Zero;
											Decimal dLIST_PRICE          = Decimal.Zero;
											Decimal dLIST_USDOLLAR       = Decimal.Zero;
											Decimal dUNIT_PRICE          = Sql.ToDecimal(rowItem["AMOUNT"   ]);
											Decimal dUNIT_USDOLLAR       = Sql.ToDecimal(rowItem["AMOUNT"   ]);
											// 06/01/2008 Paul.  MC_GROSS_ is the sum of all the items.  Convert to unit price by dividing by quantity. 
											if ( nITEM_QUANTITY > 0 )
											{
												dUNIT_PRICE    /= nITEM_QUANTITY;
												dUNIT_USDOLLAR /= nITEM_QUANTITY;
											}
											parMFT_PART_NUM.Value = sMFT_PART_NUM;
											using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
											{
												if ( rdr.Read() )
												{
													gPRODUCT_TEMPLATE_ID = Sql.ToGuid   (rdr["ID"             ]);
													sPRODUCT_NAME        = Sql.ToString (rdr["NAME"           ]);
													sMFT_PART_NUM        = Sql.ToString (rdr["MFT_PART_NUM"   ]);
													sVENDOR_PART_NUM     = Sql.ToString (rdr["VENDOR_PART_NUM"]);
													sTAX_CLASS           = Sql.ToString (rdr["TAX_CLASS"      ]);
													dCOST_PRICE          = Sql.ToDecimal(rdr["COST_PRICE"     ]);
													dCOST_USDOLLAR       = Sql.ToDecimal(rdr["COST_USDOLLAR"  ]);
													dLIST_PRICE          = Sql.ToDecimal(rdr["LIST_PRICE"     ]);
													dLIST_USDOLLAR       = Sql.ToDecimal(rdr["LIST_USDOLLAR"  ]);
													// 05/29/2008 Paul.  Don't replace the actual value charged. 
													//dUNIT_PRICE          = Sql.ToDecimal(rdr["UNIT_PRICE"     ]);
													//dUNIT_USDOLLAR       = Sql.ToDecimal(rdr["UNIT_USDOLLAR"  ]);
													if ( oCurrency.ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
													{
														dCOST_PRICE = oCurrency.ToCurrency(dCOST_USDOLLAR);
														dLIST_PRICE = oCurrency.ToCurrency(dLIST_USDOLLAR);
														//dUNIT_PRICE = oCurrency.ToCurrency(dUNIT_USDOLLAR);
													}
												}
											}
											Guid gLINE_ITEM_ID = Guid.Empty;
											// 04/03/2008 Paul.  Override tax class based on payment. Customer might be out-of-state. 
											sTAX_CLASS = (dITEM_TAX > Decimal.Zero) ? "Taxable" : "Non-Taxable";
											// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
											// 07/15/2010 Paul.  Add GROUP_ID for options management. 
											// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
											// 08/13/2010 Paul.  New discount fields. 
											// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
											// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
											SqlProcs.spORDERS_LINE_ITEMS_Update
												( ref gLINE_ITEM_ID   
												, gORDER_ID         
												, Guid.Empty          // LINE_GROUP_ID
												, String.Empty        // LINE_ITEM_TYPE
												, nPOSITION           
												, sPRODUCT_NAME       // NAME
												, sMFT_PART_NUM
												, sVENDOR_PART_NUM    
												, gPRODUCT_TEMPLATE_ID
												, sTAX_CLASS          
												, nITEM_QUANTITY      // QUANTITY
												, dCOST_PRICE         
												, dLIST_PRICE         
												, dUNIT_PRICE         
												, String.Empty        // DESCRIPTION
												, Guid.Empty          // PARENT_TEMPLATE_ID
												, Guid.Empty          // DISCOUNT_ID
												, Decimal.Zero        // DISCOUNT_PRICE
												, String.Empty        // PRICING_FORMULA
												, 0                   // PRICING_FACTOR
												, Guid.Empty          // TAXRATE_ID
												, trn
												);
											nPOSITION++;
										}
									}
								}
							}
							else
							{
								// 05/28/2008 Paul.  If the order is refunded, we get a Refunded event. 
								// The refunded event does not include the taxes.
								string sDESCRIPTION = String.Empty;
								sSQL = "select TAX            " + ControlChars.CrLf
								     + "     , SUBTOTAL       " + ControlChars.CrLf
								     + "     , SHIPPER_ID     " + ControlChars.CrLf
								     + "     , DESCRIPTION    " + ControlChars.CrLf
								     + "  from vwORDERS_Edit  " + ControlChars.CrLf
								     + " where ID = @ID       " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gORDER_ID);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											dTAX         = Sql.ToDecimal(rdr["TAX"        ]);
											dSUBTOTAL    = Sql.ToDecimal(rdr["SUBTOTAL"   ]);
											gSHIPPER_ID  = Sql.ToGuid   (rdr["SHIPPER_ID" ]);
											sDESCRIPTION = Sql.ToString (rdr["DESCRIPTION"]);
											if ( !Sql.IsEmptyString(sDESCRIPTION) )
											{
												if ( !Sql.IsEmptyString(sMEMO) )
													sMEMO = sDESCRIPTION + ControlChars.CrLf + sMEMO ;
												else
													sMEMO = sDESCRIPTION;
											}
										}
									}
								}
								// 08/06/2009 Paul.  ORDER_NUM now uses our number sequence table. 
								SqlProcs.spORDERS_Update
									( ref gORDER_ID
									, Guid.Empty        // ASSIGNED_USER_ID
									, sORDER            // NAME
									, Guid.Empty        // QUOTE_ID
									, Guid.Empty        // OPPORTUNITY_ID
									, "Due on Receipt"  // PAYMENT_TERMS
									, sORDER_STAGE
									, sRECEIPT_ID       // PURCHASE_ORDER_NUM
									, dtPAYMENT_DATE    // ORIGINAL_PO_DATE
									, dtPAYMENT_DATE    // DATE_ORDER_DUE
									, DateTime.MinValue // DATE_ORDER_SHIPPED
									, false             // SHOW_LINE_NUMS
									, false             // CALC_GRAND_TOTAL
									, 1.0f              // EXCHANGE_RATE
									, oCurrency.ID      // CURRENCY_ID
									, Guid.Empty        // TAXRATE_ID
									, gSHIPPER_ID       // SHIPPER_ID
									, dSUBTOTAL         // SUBTOTAL
									, Decimal.Zero      // DISCOUNT
									, dMC_SHIPPING      // SHIPPING
									, dTAX              // TAX
									, dMC_GROSS         // TOTAL
									, gACCOUNT_ID       // BILLING_ACCOUNT_ID
									, gCONTACT_ID       // BILLING_CONTACT_ID
									, sADDRESS_STREET   // BILLING_ADDRESS_STREET
									, sADDRESS_CITY     // BILLING_ADDRESS_CITY
									, sADDRESS_STATE    // BILLING_ADDRESS_STATE
									, sADDRESS_ZIP      // BILLING_ADDRESS_POSTALCODE
									, sADDRESS_COUNTRY  // BILLING_ADDRESS_COUNTRY
									, Guid.Empty        // SHIPPING_ACCOUNT_ID
									, Guid.Empty        // SHIPPING_CONTACT_ID
									, String.Empty      // SHIPPING_ADDRESS_STREET
									, String.Empty      // SHIPPING_ADDRESS_CITY
									, String.Empty      // SHIPPING_ADDRESS_STATE
									, String.Empty      // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty      // SHIPPING_ADDRESS_COUNTRY
									, sMEMO             // DESCRIPTION
									, String.Empty      // ORDER_NUM
									, Guid.Empty        // TEAM_ID
									, String.Empty      // TEAM_SET_LIST
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty      // TAG_SET_NAME
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty      // ASSIGNED_SET_LIST
									, trn
									);
							}
					
							string sINVOICE_STAGE = String.Empty;
							switch ( sPAYMENT_STATUS )
							{
								case "Canceled-Reversal":  sINVOICE_STAGE = "Cancelled" ;  break;
								case "Completed"        :  sINVOICE_STAGE = "Paid"      ;  break;
								case "Denied"           :  sINVOICE_STAGE = "Cancelled" ;  break;
								case "Expired"          :  sINVOICE_STAGE = "Cancelled" ;  break;
								case "Failed"           :  sINVOICE_STAGE = "Cancelled" ;  break;
								case "In-Progress"      :  sINVOICE_STAGE = "Due"       ;  break;
								case "Pending"          :  sINVOICE_STAGE = "Due"       ;  break;
								case "Processed"        :  sINVOICE_STAGE = "Paid"      ;  break;
								// 11/11/2008 Paul.  Change stage to Refunded. 
								case "Refunded"         :  sINVOICE_STAGE = "Refunded"  ;  break;
								case "Reversed"         :  sINVOICE_STAGE = "Cancelled" ;  break;
							}
					
							if ( Sql.IsEmptyGuid(gINVOICE_ID) )
							{
								// 08/06/2009 Paul.  INVOICE_NUM now uses our number sequence table. 
								// 02/27/2015 Paul.  Add SHIP_DATE to sync with QuickBooks. 
								SqlProcs.spINVOICES_Update
									( ref gINVOICE_ID
									, Guid.Empty        // ASSIGNED_USER_ID
									, sINVOICE          // NAME
									, Guid.Empty        // QUOTE_ID
									, gORDER_ID         // ORDER_ID
									, Guid.Empty        // OPPORTUNITY_ID
									, "Due on Receipt"  // PAYMENT_TERMS
									, sINVOICE_STAGE
									, sRECEIPT_ID       // PURCHASE_ORDER_NUM
									, dtPAYMENT_DATE    // DUE_DATE
									, 1.0f              // EXCHANGE_RATE
									, oCurrency.ID      // CURRENCY_ID
									, Guid.Empty        // TAXRATE_ID
									, gSHIPPER_ID       // SHIPPER_ID
									, dSUBTOTAL         // SUBTOTAL
									, Decimal.Zero      // DISCOUNT
									, dMC_SHIPPING      // SHIPPING
									, dTAX              // TAX
									, dMC_GROSS         // TOTAL
									, Decimal.Zero      // AMOUNT_DUE
									, gACCOUNT_ID       // BILLING_ACCOUNT_ID
									, gCONTACT_ID       // BILLING_CONTACT_ID
									, sADDRESS_STREET   // BILLING_ADDRESS_STREET
									, sADDRESS_CITY     // BILLING_ADDRESS_CITY
									, sADDRESS_STATE    // BILLING_ADDRESS_STATE
									, sADDRESS_ZIP      // BILLING_ADDRESS_POSTALCODE
									, sADDRESS_COUNTRY  // BILLING_ADDRESS_COUNTRY
									, Guid.Empty        // SHIPPING_ACCOUNT_ID
									, Guid.Empty        // SHIPPING_CONTACT_ID
									, String.Empty      // SHIPPING_ADDRESS_STREET
									, String.Empty      // SHIPPING_ADDRESS_CITY
									, String.Empty      // SHIPPING_ADDRESS_STATE
									, String.Empty      // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty      // SHIPPING_ADDRESS_COUNTRY
									, sMEMO             // DESCRIPTION
									, String.Empty      // INVOICE_NUM
									, Guid.Empty        // TEAM_ID
									, String.Empty      // TEAM_SET_LIST
									, DateTime.MinValue // SHIP_DATE
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty      // TAG_SET_NAME
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty      // ASSIGNED_SET_LIST
									, trn
									);
					
								if ( nNUM_CART_ITEMS > 0 )
								{
									sSQL = "select *                          " + ControlChars.CrLf
									    + "  from vwPRODUCT_CATALOG           " + ControlChars.CrLf
									    + " where MFT_PART_NUM = @MFT_PART_NUM" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.Transaction = trn;
										cmd.CommandText = sSQL;
										IDbDataParameter parMFT_PART_NUM = Sql.AddParameter(cmd, "@MFT_PART_NUM", String.Empty);
										int nPOSITION = 1;
										foreach ( DataRow rowItem in dtLINE_ITEMS.Rows )
										{
											Guid    gPRODUCT_TEMPLATE_ID = Guid.Empty;
											string  sPRODUCT_NAME        = Sql.ToString (rowItem["NAME"     ]);
											string  sMFT_PART_NUM        = Sql.ToString (rowItem["NUMBER"   ]);
											Decimal dITEM_TAX            = Sql.ToDecimal(rowItem["SALES_TAX"]);
											int     nITEM_QUANTITY       = Sql.ToInteger(rowItem["QUANTITY" ]);
											string  sVENDOR_PART_NUM     = String.Empty;
											string  sTAX_CLASS           = String.Empty;
											Decimal dCOST_PRICE          = Decimal.Zero;
											Decimal dCOST_USDOLLAR       = Decimal.Zero;
											Decimal dLIST_PRICE          = Decimal.Zero;
											Decimal dLIST_USDOLLAR       = Decimal.Zero;
											Decimal dUNIT_PRICE          = Sql.ToDecimal(rowItem["AMOUNT"   ]);
											Decimal dUNIT_USDOLLAR       = Sql.ToDecimal(rowItem["AMOUNT"   ]);
											// 06/01/2008 Paul.  MC_GROSS_ is the sum of all the items.  Convert to unit price by dividing by quantity. 
											if ( nITEM_QUANTITY > 0 )
											{
												dUNIT_PRICE    /= nITEM_QUANTITY;
												dUNIT_USDOLLAR /= nITEM_QUANTITY;
											}
											parMFT_PART_NUM.Value = sMFT_PART_NUM;
											using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
											{
												if ( rdr.Read() )
												{
													gPRODUCT_TEMPLATE_ID = Sql.ToGuid   (rdr["ID"             ]);
													sPRODUCT_NAME        = Sql.ToString (rdr["NAME"           ]);
													sMFT_PART_NUM        = Sql.ToString (rdr["MFT_PART_NUM"   ]);
													sVENDOR_PART_NUM     = Sql.ToString (rdr["VENDOR_PART_NUM"]);
													sTAX_CLASS           = Sql.ToString (rdr["TAX_CLASS"      ]);
													dCOST_PRICE          = Sql.ToDecimal(rdr["COST_PRICE"     ]);
													dCOST_USDOLLAR       = Sql.ToDecimal(rdr["COST_USDOLLAR"  ]);
													dLIST_PRICE          = Sql.ToDecimal(rdr["LIST_PRICE"     ]);
													dLIST_USDOLLAR       = Sql.ToDecimal(rdr["LIST_USDOLLAR"  ]);
													// 05/29/2008 Paul.  Don't replace the actual value charged. 
													//dUNIT_PRICE          = Sql.ToDecimal(rdr["UNIT_PRICE"     ]);
													//dUNIT_USDOLLAR       = Sql.ToDecimal(rdr["UNIT_USDOLLAR"  ]);
													if ( oCurrency.ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
													{
														dCOST_PRICE = oCurrency.ToCurrency(dCOST_USDOLLAR);
														dLIST_PRICE = oCurrency.ToCurrency(dLIST_USDOLLAR);
														//dUNIT_PRICE = oCurrency.ToCurrency(dUNIT_USDOLLAR);
													}
												}
											}
											Guid gLINE_ITEM_ID = Guid.Empty;
											// 04/03/2008 Paul.  Override tax class based on payment. Customer might be out-of-state. 
											sTAX_CLASS = (dITEM_TAX > Decimal.Zero) ? "Taxable" : "Non-Taxable";
											// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
											// 07/15/2010 Paul.  Add GROUP_ID for options management. 
											// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
											// 08/13/2010 Paul.  New discount fields. 
											// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
											// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
											SqlProcs.spINVOICES_LINE_ITEMS_Update
												( ref gLINE_ITEM_ID   
												, gINVOICE_ID         
												, Guid.Empty          // LINE_GROUP_ID
												, String.Empty        // LINE_ITEM_TYPE
												, nPOSITION           
												, sPRODUCT_NAME       // NAME
												, sMFT_PART_NUM
												, sVENDOR_PART_NUM    
												, gPRODUCT_TEMPLATE_ID
												, sTAX_CLASS          
												, nITEM_QUANTITY      // QUANTITY
												, dCOST_PRICE         
												, dLIST_PRICE         
												, dUNIT_PRICE         
												, String.Empty        // DESCRIPTION
												, Guid.Empty          // PARENT_TEMPLATE_ID
												, Guid.Empty          // DISCOUNT_ID
												, Decimal.Zero        // DISCOUNT_PRICE
												, String.Empty        // PRICING_FORMULA
												, 0                   // PRICING_FACTOR
												, Guid.Empty          // TAXRATE_ID
												, trn
												);
											nPOSITION++;
										}
									}
								}
							}
							else
							{
								// 05/28/2008 Paul.  If the order is refunded, we get a Refunded event. 
								// The refunded event does not include the taxes.
								string sDESCRIPTION = String.Empty;
								sSQL = "select TAX            " + ControlChars.CrLf
								     + "     , SUBTOTAL       " + ControlChars.CrLf
								     + "     , SHIPPER_ID     " + ControlChars.CrLf
								     + "     , DESCRIPTION    " + ControlChars.CrLf
								     + "  from vwINVOICES_Edit" + ControlChars.CrLf
								     + " where ID = @ID       " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gINVOICE_ID);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											dTAX         = Sql.ToDecimal(rdr["TAX"        ]);
											dSUBTOTAL    = Sql.ToDecimal(rdr["SUBTOTAL"   ]);
											gSHIPPER_ID  = Sql.ToGuid   (rdr["SHIPPER_ID" ]);
											sDESCRIPTION = Sql.ToString (rdr["DESCRIPTION"]);
											if ( !Sql.IsEmptyString(sDESCRIPTION) )
											{
												if ( !Sql.IsEmptyString(sMEMO) )
													sMEMO = sDESCRIPTION + ControlChars.CrLf + sMEMO ;
												else
													sMEMO = sDESCRIPTION;
											}
										}
									}
								}
								// 08/06/2009 Paul.  INVOICE_NUM now uses our number sequence table. 
								// 02/27/2015 Paul.  Add SHIP_DATE to sync with QuickBooks. 
								SqlProcs.spINVOICES_Update
									( ref gINVOICE_ID
									, Guid.Empty        // ASSIGNED_USER_ID
									, sINVOICE          // NAME
									, Guid.Empty        // QUOTE_ID
									, gORDER_ID         // ORDER_ID
									, Guid.Empty        // OPPORTUNITY_ID
									, "Due on Receipt"  // PAYMENT_TERMS
									, sINVOICE_STAGE
									, sRECEIPT_ID       // PURCHASE_ORDER_NUM
									, dtPAYMENT_DATE    // DUE_DATE
									, 1.0f              // EXCHANGE_RATE
									, oCurrency.ID      // CURRENCY_ID
									, Guid.Empty        // TAXRATE_ID
									, gSHIPPER_ID       // SHIPPER_ID
									, dSUBTOTAL         // SUBTOTAL
									, Decimal.Zero      // DISCOUNT
									, dMC_SHIPPING      // SHIPPING
									, dTAX              // TAX
									, dMC_GROSS         // TOTAL
									, Decimal.Zero      // AMOUNT_DUE
									, gACCOUNT_ID       // BILLING_ACCOUNT_ID
									, gCONTACT_ID       // BILLING_CONTACT_ID
									, sADDRESS_STREET   // BILLING_ADDRESS_STREET
									, sADDRESS_CITY     // BILLING_ADDRESS_CITY
									, sADDRESS_STATE    // BILLING_ADDRESS_STATE
									, sADDRESS_ZIP      // BILLING_ADDRESS_POSTALCODE
									, sADDRESS_COUNTRY  // BILLING_ADDRESS_COUNTRY
									, Guid.Empty        // SHIPPING_ACCOUNT_ID
									, Guid.Empty        // SHIPPING_CONTACT_ID
									, String.Empty      // SHIPPING_ADDRESS_STREET
									, String.Empty      // SHIPPING_ADDRESS_CITY
									, String.Empty      // SHIPPING_ADDRESS_STATE
									, String.Empty      // SHIPPING_ADDRESS_POSTALCODE
									, String.Empty      // SHIPPING_ADDRESS_COUNTRY
									, sMEMO             // DESCRIPTION
									, String.Empty      // INVOICE_NUM
									, Guid.Empty        // TEAM_ID
									, String.Empty      // TEAM_SET_LIST
									, DateTime.MinValue // SHIP_DATE
									// 05/12/2016 Paul.  Add Tags module. 
									, String.Empty      // TAG_SET_NAME
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty      // ASSIGNED_SET_LIST
									, trn
									);
							}
					
							if ( Sql.IsEmptyGuid(gPAYMENT_ID) )
							{
								// 08/06/2009 Paul.  PAYMENT_NUM now uses our number sequence table. 
								// 08/26/2010 Paul.  We need a bank fee field to allow for a difference between allocated and received payment. 
								// 05/07/2013 Paul.  Add Contacts field to support B2C. 
								Guid gB2C_CONTACT_ID = Guid.Empty;
								SqlProcs.spPAYMENTS_Update
									( ref gPAYMENT_ID
									, Guid.Empty      // ASSIGNED_USER_ID
									, gACCOUNT_ID
									, dtPAYMENT_DATE
									, "PayPal"        // PAYMENT_TYPE
									, sRECEIPT_ID     // CUSTOMER_REFERENCE
									, 1.0f            // EXCHANGE_RATE
									, oCurrency.ID    // CURRENCY_ID
									, dPAYMENT_GROSS  // AMOUNT
									, String.Empty    // DESCRIPTION
									, Guid.Empty      // CREDIT_CARD_ID
									, String.Empty    // PAYMENT_NUM
									, Guid.Empty      // TEAM_ID
									, String.Empty    // TEAM_SET_LIST
									, dPAYMENT_FEE    // BANK_FEE
									, gB2C_CONTACT_ID
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									, String.Empty    // ASSIGNED_SET_LIST
									, trn
									);
								Guid gINVOICES_PAYMENT_ID = Guid.Empty;
								SqlProcs.spINVOICES_PAYMENTS_Update(ref gINVOICES_PAYMENT_ID, gINVOICE_ID, gPAYMENT_ID, dPAYMENT_GROSS, trn);
							}
							if ( Sql.IsEmptyGuid(gPAYMENTS_TRANSACTION_ID) )
							{
								// Canceled-Reversal, Completed, Denied, Expired, Failed, In-Progress, Pending, Processed, Refunded, Reversed, Voided
								// 04/22/2008 Paul.  Change from transaction type of Charge to Sale to match .netCHARGE. 
								string sTRANSACTION_TYPE = sPAYMENT_STATUS;
								switch ( sPAYMENT_STATUS )
								{
									case "Completed":  sTRANSACTION_TYPE = "Sale"  ;  break;
									case "Refunded" :  sTRANSACTION_TYPE = "Refund";  break;
									case "Reversed" :  sTRANSACTION_TYPE = "Refund";  break;
								}
								string sINVOICE_NUMBER = String.Empty;
								sSQL = "select INVOICE_NUM" + ControlChars.CrLf
								     + "  from vwINVOICES " + ControlChars.CrLf
								     + " where ID = @ID   " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.Transaction = trn;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gINVOICE_ID);
									sINVOICE_NUMBER = Sql.ToString(cmd.ExecuteScalar());
								}
					
								SqlProcs.spPAYMENTS_TRANSACTIONS_InsertOnly
									( ref gPAYMENTS_TRANSACTION_ID
									, gPAYMENT_ID
									, "PayPal"           // PAYMENT_GATEWAY
									, sTRANSACTION_TYPE  // Sale or Refund. 
									, dPAYMENT_GROSS
									, oCurrency.ID       // CURRENCY_ID
									, sINVOICE_NUMBER    // INVOICE_NUMBER
									, String.Empty       // DESCRIPTION
									, Guid.Empty         // CREDIT_CARD_ID
									, gACCOUNT_ID
									, sPAYMENT_STATUS    // STATUS
									, trn
									);
								SqlProcs.spPAYMENTS_TRANSACTIONS_Update
									( gPAYMENTS_TRANSACTION_ID
									, sPAYMENT_STATUS    // STATUS
									, sTXN_ID            // TRANSACTION_NUMBER
									, String.Empty       // REFERENCE_NUMBER
									, String.Empty       // AUTHORIZATION_CODE
									, String.Empty       // AVS_CODE
									, sPENDING_REASON    // ERROR_CODE
									, sREASON_CODE       // ERROR_MESSAGE
									, trn
									);
							}
							trn.Commit();
						}
						catch
						{
							trn.Rollback();
							throw;
						}
					}
				}
			}
		}

		public static string Charge(HttpApplicationState Application, Guid gCURRENCY_ID, Guid gCONTACT_ID, Guid gINVOICE_ID, Guid gPAYMENT_ID, Guid gCREDIT_CARD_ID, string sClientIP, string sDESCRIPTION)
		{
			Guid gCREDIT_CARD_KEY = Sql.ToGuid(Application["CONFIG.CreditCardKey"]);
			Guid gCREDIT_CARD_IV  = Sql.ToGuid(Application["CONFIG.CreditCardIV" ]);
			// 11/15/2009 Paul.  Had to fix the Countries function to use the Runtime cache unstead of the context. 
			StringDictionary dictCountries = PayPalCache.Countries();

			string sSTATUS = "Prevalidation";
			// 11/15/2009 Paul.  In order to call this in the background, we need to use the Application object to get to the factory. 
			SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory(Application);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				DoDirectPaymentRequestType dpr = new DoDirectPaymentRequestType();
				dpr.Version = "1.0";
				dpr.DoDirectPaymentRequestDetails = new DoDirectPaymentRequestDetailsType();
				dpr.DoDirectPaymentRequestDetails.IPAddress         = sClientIP;
				//dpr.DoDirectPaymentRequestDetails.MerchantSessionId = sSESSION_ID;
				dpr.DoDirectPaymentRequestDetails.PaymentAction     = PaymentActionCodeType.Sale;
				dpr.DoDirectPaymentRequestDetails.CreditCard        = new CreditCardDetailsType();
				dpr.DoDirectPaymentRequestDetails.PaymentDetails    = new PaymentDetailsType();

				CurrencyCodeType currencyID = CurrencyCodeType.USD;
				// 11/15/2009 Paul.  We need to supply the Application object as this function is called in the background. 
				Currency C10n = Currency.CreateCurrency(Application, gCURRENCY_ID);
				try
				{
					currencyID = (CurrencyCodeType) Enum.Parse(typeof(CurrencyCodeType), C10n.ISO4217, true);
				}
				catch
				{
				}

				string sFULL_NAME   = String.Empty;
				string sFIRST_NAME  = String.Empty;
				string sMIDDLE_NAME = String.Empty;
				string sLAST_NAME   = String.Empty;
				string sEMAIL1      = String.Empty;
				string sPHONE_WORK  = String.Empty;
				string sSQL         = String.Empty;
				sSQL = "select *         " + ControlChars.CrLf
				     + "  from vwCONTACTS" + ControlChars.CrLf
				     + " where ID = @ID  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gCONTACT_ID);
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							sFULL_NAME  = Sql.ToString(rdr["NAME"      ]);
							sFIRST_NAME = Sql.ToString(rdr["FIRST_NAME"]);
							sLAST_NAME  = Sql.ToString(rdr["LAST_NAME" ]);
							sEMAIL1     = Sql.ToString(rdr["EMAIL1"    ]);
							sPHONE_WORK = Sql.ToString(rdr["PHONE_WORK"]);
						}
					}
				}

				sSQL = "select *                  " + ControlChars.CrLf
				     + "  from vwCREDIT_CARDS_Edit" + ControlChars.CrLf
				     + " where ID = @ID           " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gCREDIT_CARD_ID);
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							string sCARD_NUMBER = Sql.ToString(rdr["CARD_NUMBER"]);
							if ( Sql.ToBoolean(rdr["IS_ENCRYPTED"]) )
							{
								sCARD_NUMBER = Security.DecryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
							}
							string sCARD_TYPE = Sql.ToString(rdr["CARD_TYPE"]);
							if ( sCARD_TYPE.StartsWith("Bank Draft") )
							{
								throw(new Exception("Bank Drafts are not supported at this time. "));
							}
							else if ( sCARD_TYPE == "American Express" )
							{
								sCARD_TYPE = "Amex";
							}
							else if ( sCARD_TYPE == "Discover Card" )
							{
								sCARD_TYPE = "Discover";
							}

							DateTime dtEXPIRATION_DATE = Sql.ToDateTime(rdr["EXPIRATION_DATE"]);
							dpr.DoDirectPaymentRequestDetails.CreditCard.CreditCardNumber                  = sCARD_NUMBER;
							dpr.DoDirectPaymentRequestDetails.CreditCard.CVV2                              = Sql.ToString(rdr["SECURITY_CODE"]);
							dpr.DoDirectPaymentRequestDetails.CreditCard.CreditCardType                    = (CreditCardTypeType) Enum.Parse(typeof(CreditCardTypeType), sCARD_TYPE, true);
							dpr.DoDirectPaymentRequestDetails.CreditCard.CreditCardTypeSpecified           = true;
							dpr.DoDirectPaymentRequestDetails.CreditCard.ExpMonth                          = dtEXPIRATION_DATE.Month;
							dpr.DoDirectPaymentRequestDetails.CreditCard.ExpMonthSpecified                 = true;
							dpr.DoDirectPaymentRequestDetails.CreditCard.ExpYear                           = dtEXPIRATION_DATE.Year;
							dpr.DoDirectPaymentRequestDetails.CreditCard.ExpYearSpecified                  = true;

							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner                         = new PayerInfoType();
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.ContactPhone            = sPHONE_WORK;

							string sCREDIT_CARD_NAME = Sql.ToString(rdr["NAME"]);
							string[] arrCREDIT_CARD_NAME = sCREDIT_CARD_NAME.Split(' ');
							if ( arrCREDIT_CARD_NAME.Length == 2 )
							{
								sFIRST_NAME  = arrCREDIT_CARD_NAME[0];
								sLAST_NAME   = arrCREDIT_CARD_NAME[1];
							}
							else if ( arrCREDIT_CARD_NAME.Length == 3 )
							{
								sFIRST_NAME  = arrCREDIT_CARD_NAME[0];
								sMIDDLE_NAME = arrCREDIT_CARD_NAME[1];
								sLAST_NAME   = arrCREDIT_CARD_NAME[2];
							}
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Payer                   = sEMAIL1;
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.PayerName               = new PersonNameType();
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.PayerName.FirstName     = sFIRST_NAME;
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.PayerName.MiddleName    = sMIDDLE_NAME;
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.PayerName.LastName      = sLAST_NAME;
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address                 = new AddressType();
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.Name            = sCREDIT_CARD_NAME;
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.Phone           = sPHONE_WORK;
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.Street1         = Sql.ToString(rdr["ADDRESS_STREET"    ]);
							//dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.Street2       = "";
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CityName        = Sql.ToString(rdr["ADDRESS_CITY"      ]);
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.StateOrProvince = Sql.ToString(rdr["ADDRESS_STATE"     ]);
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.PostalCode      = Sql.ToString(rdr["ADDRESS_POSTALCODE"]);
							dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CountryName     = Sql.ToString(rdr["ADDRESS_COUNTRY"   ]);
							if ( !Sql.IsEmptyString(dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CountryName) )
							{
								try
								{
									// 12/16/2008 Paul.  Use the countries dictionary to convert to the correct country code. 
									if ( dictCountries.ContainsKey(dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CountryName.ToUpper()) )
										dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CountryName = dictCountries[dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CountryName.ToUpper()];
									dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.Country = (CountryCodeType) Enum.Parse(typeof(CountryCodeType), dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CountryName, true);
									dpr.DoDirectPaymentRequestDetails.CreditCard.CardOwner.Address.CountrySpecified = true;
								}
								catch
								{
								}
							}
							else
							{
								// 12/16/2008 Paul.  Always set the country, otherwise PayPal will fail. 
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName = "United States";
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Country = CountryCodeType.US;
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountrySpecified = true;
							}
						}
					}
				}

				string  sInvoiceNumber = String.Empty;
				decimal dAMOUNT        = Decimal.Zero;
				Guid    gACCOUNT_ID    = Guid.Empty;
				sSQL = "select *         " + ControlChars.CrLf
				     + "  from vwINVOICES" + ControlChars.CrLf
				     + " where ID = @ID  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gINVOICE_ID);
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							string sOrderTotal       = String.Empty;
							string sShippingTotal    = String.Empty;
							string sHandlingTotal    = String.Empty;
							string sInsuranceTotal   = String.Empty;
							string sShippingDiscount = String.Empty;
							string sTaxTotal         = String.Empty;
							if ( currencyID == CurrencyCodeType.USD )
							{
								dAMOUNT = Sql.ToDecimal(rdr["TOTAL_USDOLLAR"]);
								if ( rdr["TOTAL_USDOLLAR"   ] != DBNull.Value ) sOrderTotal       = Sql.ToDecimal(rdr["TOTAL_USDOLLAR"   ]).ToString("0.00");
								if ( rdr["SHIPPING_USDOLLAR"] != DBNull.Value ) sShippingTotal    = Sql.ToDecimal(rdr["SHIPPING_USDOLLAR"]).ToString("0.00");
								if ( rdr["TAX_USDOLLAR"     ] != DBNull.Value ) sTaxTotal         = Sql.ToDecimal(rdr["TAX_USDOLLAR"     ]).ToString("0.00");
							}
							else
							{
								dAMOUNT = Sql.ToDecimal(rdr["TOTAL"]);
								if ( rdr["TOTAL"   ] != DBNull.Value ) sOrderTotal       = Sql.ToDecimal(rdr["TOTAL"   ]).ToString("0.00");
								if ( rdr["SHIPPING"] != DBNull.Value ) sShippingTotal    = Sql.ToDecimal(rdr["SHIPPING"]).ToString("0.00");
								if ( rdr["TAX"     ] != DBNull.Value ) sTaxTotal         = Sql.ToDecimal(rdr["TAX"     ]).ToString("0.00");
							}

							gACCOUNT_ID    = Sql.ToGuid(rdr["BILLING_ACCOUNT_ID"]);
							// 08/15/2015 Paul.  B2C will not have an Account ID. 
							if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
								gACCOUNT_ID = Sql.ToGuid(rdr["BILLING_CONTACT_ID"]);
							sInvoiceNumber = Sql.ToString(rdr["INVOICE_NUM"]);
							dpr.DoDirectPaymentRequestDetails.PaymentDetails.InvoiceID                   = sInvoiceNumber;
							dpr.DoDirectPaymentRequestDetails.PaymentDetails.OrderTotal                  = new BasicAmountType();
							dpr.DoDirectPaymentRequestDetails.PaymentDetails.OrderTotal.currencyID       = currencyID;
							dpr.DoDirectPaymentRequestDetails.PaymentDetails.OrderTotal.Value            = sOrderTotal;
							if ( !Sql.IsEmptyString(sShippingTotal) )
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShippingTotal               = new BasicAmountType();
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShippingTotal.currencyID    = currencyID;
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShippingTotal.Value         = sShippingTotal;
							}
							if ( !Sql.IsEmptyString(sHandlingTotal) )
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.HandlingTotal               = new BasicAmountType();
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.HandlingTotal.currencyID    = currencyID;
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.HandlingTotal.Value         = sHandlingTotal;
							}
							if ( !Sql.IsEmptyString(sInsuranceTotal) )
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.InsuranceTotal              = new BasicAmountType();
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.InsuranceTotal.currencyID   = currencyID;
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.InsuranceTotal.Value        = sInsuranceTotal;
							}
							if ( !Sql.IsEmptyString(sShippingDiscount) )
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShippingDiscount            = new BasicAmountType();
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShippingDiscount.currencyID = currencyID;
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShippingDiscount.Value      = sShippingDiscount;
							}
							if ( !Sql.IsEmptyString(sTaxTotal) )
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.TaxTotal                    = new BasicAmountType();
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.TaxTotal.currencyID         = currencyID;
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.TaxTotal.Value              = sTaxTotal;
							}
							dpr.DoDirectPaymentRequestDetails.PaymentDetails.OrderDescription = sDESCRIPTION;
							//dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShippingMethod            = ShippingServiceCodeType.UPSGround;

							if ( !Sql.IsEmptyString(rdr["SHIPPING_ADDRESS_STREET"]) )
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress                 = new AddressType();
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Name            = Sql.ToString(rdr["SHIPPING_ACCOUNT_NAME"      ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Phone           = "";
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Street1         = Sql.ToString(rdr["SHIPPING_ADDRESS_STREET"    ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Street2         = "";
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CityName        = Sql.ToString(rdr["SHIPPING_ADDRESS_CITY"      ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.StateOrProvince = Sql.ToString(rdr["SHIPPING_ADDRESS_STATE"     ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.PostalCode      = Sql.ToString(rdr["SHIPPING_ADDRESS_POSTALCODE"]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName     = Sql.ToString(rdr["SHIPPING_ADDRESS_COUNTRY"   ]);
								if ( !Sql.IsEmptyString(dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName) )
								{
									try
									{
										// 12/16/2008 Paul.  Use the countries dictionary to convert to the correct country code. 
										if ( dictCountries.ContainsKey(dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName.ToUpper()) )
											dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName = dictCountries[dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName.ToUpper()];
										dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Country = (CountryCodeType) Enum.Parse(typeof(CountryCodeType), dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName, true);
										dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountrySpecified = true;
									}
									catch
									{
									}
								}
								else
								{
									// 12/16/2008 Paul.  Always set the country, otherwise PayPal will fail. 
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName = "United States";
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Country = CountryCodeType.US;
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountrySpecified = true;
								}
							}
							else
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress                 = new AddressType();
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Name            = Sql.ToString(rdr["BILLING_ACCOUNT_NAME"      ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Phone           = "";
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Street1         = Sql.ToString(rdr["BILLING_ADDRESS_STREET"    ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Street2         = "";
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CityName        = Sql.ToString(rdr["BILLING_ADDRESS_CITY"      ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.StateOrProvince = Sql.ToString(rdr["BILLING_ADDRESS_STATE"     ]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.PostalCode      = Sql.ToString(rdr["BILLING_ADDRESS_POSTALCODE"]);
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName     = Sql.ToString(rdr["BILLING_ADDRESS_COUNTRY"   ]);
								if ( !Sql.IsEmptyString(dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName) )
								{
									try
									{
										// 12/16/2008 Paul.  Use the countries dictionary to convert to the correct country code. 
										if ( dictCountries.ContainsKey(dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName.ToUpper()) )
											dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName = dictCountries[dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName.ToUpper()];
										dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Country = (CountryCodeType) Enum.Parse(typeof(CountryCodeType), dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName, true);
										dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountrySpecified = true;
									}
									catch
									{
									}
								}
								else
								{
									// 12/16/2008 Paul.  Always set the country, otherwise PayPal will fail. 
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountryName = "United States";
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.Country = CountryCodeType.US;
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.ShipToAddress.CountrySpecified = true;
								}
							}
						}
					}
				}
				sSQL = "select *                       " + ControlChars.CrLf
				     + "  from vwINVOICES_LINE_ITEMS   " + ControlChars.CrLf
				     + " where INVOICE_ID = @INVOICE_ID" + ControlChars.CrLf
				     + " order by POSITION             " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@INVOICE_ID", gINVOICE_ID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							if ( dt.Rows.Count > 0 )
							{
								dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem = new PaymentDetailsItemType[dt.Rows.Count];
								for ( int i = 0; i < dt.Rows.Count; i++ )
								{
									DataRow row = dt.Rows[i];
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i] = new PaymentDetailsItemType();
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Amount            = new BasicAmountType();
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Amount.currencyID = currencyID;
									// 12/14/2008 Paul.  Use UNIT_PRICE and not EXTENDED_PRICE. 
									if ( currencyID == CurrencyCodeType.USD )
										dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Amount.Value  = Sql.ToDecimal(row["UNIT_USDOLLAR"]).ToString("0.00");
									else
										dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Amount.Value  = Sql.ToDecimal(row["UNIT_PRICE"]).ToString("0.00");
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Description       = Sql.ToString(row["DESCRIPTION" ]);
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Name              = Sql.ToString(row["NAME"        ]);
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Number            = Sql.ToString(row["MFT_PART_NUM"]);
									dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Quantity          = Sql.ToString(row["QUANTITY"    ]);
									//dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Tax               = new BasicAmountType();
									//dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Tax.currencyID    = currencyID;
									//dpr.DoDirectPaymentRequestDetails.PaymentDetails.PaymentDetailsItem[i].Tax.Value         = Sql.ToString(row["TAX"]);
								}
							}
						}
					}
				}

				Guid gPAYMENTS_TRANSACTION_ID = Guid.Empty;
				// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
				using ( IDbTransaction trn = Sql.BeginTransaction(con) )
				{
					try
					{
						SqlProcs.spPAYMENTS_TRANSACTIONS_InsertOnly
							( ref gPAYMENTS_TRANSACTION_ID
							, gPAYMENT_ID
							, "PayPal"
							, "Sale"
							, dAMOUNT
							, gCURRENCY_ID
							, sInvoiceNumber
							, sDESCRIPTION
							, gCREDIT_CARD_ID
							, gACCOUNT_ID
							, sSTATUS
							, trn
							);
						trn.Commit();
					}
					catch
					{
						trn.Rollback();
						throw;
					}
				}
				string sTransactionID = String.Empty;
				string sAvsCode       = String.Empty;
				string sErrorMessage  = String.Empty;
				try
				{
					// 11/15/2009 Paul.  We need to pass the application as this function is called in the background. 
					PayPalAPI api = PayPalCache.CreatePayPalAPI(Application);
					// 01/14/2009 Paul.  Log any success warnings. 
					AbstractResponseType resp = api.DoDirect(dpr, ref sTransactionID, ref sAvsCode, ref sSTATUS);
					if ( resp.Ack == AckCodeType.SuccessWithWarning )
					{
						SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), api.ExpandErrors(resp.Errors));
					}
				}
				catch(Exception ex)
				{
					sErrorMessage = ex.Message;
					sSTATUS = "Failed";
				}
				finally
				{
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							SqlProcs.spPAYMENTS_TRANSACTIONS_Update
								( gPAYMENTS_TRANSACTION_ID
								, sSTATUS
								, sTransactionID
								, String.Empty
								, String.Empty
								, sAvsCode
								, String.Empty
								, sErrorMessage
								, trn
								);
							trn.Commit();
						}
						catch
						{
							trn.Rollback();
						}
					}
				}
				if ( sSTATUS == "Failed" )
					throw(new Exception(sErrorMessage));
			}
			return sSTATUS;
		}
	}
}
