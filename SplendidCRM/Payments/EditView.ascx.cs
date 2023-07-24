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
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Diagnostics;

namespace SplendidCRM.Payments
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected AllocationsView ctlAllocationsView;

		// 10/28/2010 Paul.  Create the buttons manually so that we can create a Charge button for each available gateway. 
		// 06/01/2015 Paul.  Buttons created dynamically. 
		//protected Button          btnSave               ;
		//protected Button          btnCancel             ;
		//protected Button          btnCharge             ;
		//protected Repeater        ctlGatewayRepeater    ;
		//protected Label           lblError              ;
		//protected TableCell       tdRequired            ;

		protected Guid            gID                   ;
		protected HtmlTable       tblMain               ;
		// 10/12/2010 Paul.  These fields are used a lot. 
		protected DropDownList    CURRENCY_ID           ;
		protected TextBox         BANK_FEE              ;
		protected TextBox         AMOUNT                ;
		protected DropDownList    PAYMENT_TYPE          ;
		protected DataTable       dtPaymentGateways     ;

		// 05/08/2022 Paul.  Move to static method so that we can call from Rest API. 
		public static void Charge(HttpContext Context, Guid gID, string sPAYMENT_GATEWAY_ID)
		{
			HttpApplicationState Application = Context.Application;
			HttpSessionState     Session     = Context.Session    ;
			HttpRequest          Request     = Context.Request    ;
			DataRow   rowCurrent = null;
			DataTable dtCurrent  = new DataTable();
			DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					string sSQL ;
					sSQL = "select *"               + ControlChars.CrLf
					     + "  from vwPAYMENTS_Edit" + ControlChars.CrLf;
					cmd.CommandText = sSQL;
					Security.Filter(cmd, "Payments", "edit");
					Sql.AppendParameter(cmd, gID, "ID", false);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtCurrent);
						if ( dtCurrent.Rows.Count > 0 )
						{
							rowCurrent = dtCurrent.Rows[0];
						}
					}
				}
				L10N L10n = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
				if ( rowCurrent == null )
				{
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				Decimal  dAMOUNT             = Sql.ToDecimal (rowCurrent["AMOUNT"            ]);
				Guid     gACCOUNT_ID         = Sql.ToGuid    (rowCurrent["ACCOUNT_ID"        ]);
				Guid     gB2C_CONTACT_ID     = Sql.ToGuid    (rowCurrent["B2C_CONTACT_ID"    ]);
				Guid     gCURRENCY_ID        = Sql.ToGuid    (rowCurrent["CURRENCY_ID"       ]);
				string   sDESCRIPTION        = Sql.ToString  (rowCurrent["DESCRIPTION"       ]);
				Guid     gCREDIT_CARD_ID     = Sql.ToGuid    (rowCurrent["CREDIT_CARD_ID"    ]);
				if ( Sql.IsEmptyGuid(gCURRENCY_ID) )
				{
					gCURRENCY_ID = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
				}
				StringBuilder sbINVOICE_NUMBER = new StringBuilder();
				{
					string sSQL;
					sSQL = "select *                  " + ControlChars.CrLf
					     + "  from vwPAYMENTS_INVOICES" + ControlChars.CrLf
					     + " where 1 = 1              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID", false);
						cmd.CommandText += " order by DATE_MODIFIED asc" + ControlChars.CrLf;
						using ( DataTable dtLineItems = new DataTable() )
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dtLineItems);
							}
							foreach ( DataRow row in dtLineItems.Rows )
							{
								Guid    gITEM_ID        = Sql.ToGuid   (row["ID"             ]);
								Guid    gINVOICE_ID     = Sql.ToGuid   (row["INVOICE_ID"     ]);
								Decimal dINVOICE_AMOUNT = Sql.ToDecimal(row["AMOUNT"         ]);
								// 09/29/2008 Paul.  Include the invoice number in the gateway transaction id. 
								// 10/01/2008 Paul.  Invoice Name works better. 
								string  sINVOICE_NAME   = Sql.ToString (row["INVOICE_NAME"   ]);
								// 03/27/2007 Paul.  Only add if product is defined.  This will exclude the blank row. 
								if ( !Sql.IsEmptyGuid(gINVOICE_ID) )
								{
									if ( sbINVOICE_NUMBER.Length > 0 )
										sbINVOICE_NUMBER.Append(",");
									// 09/29/2008 Paul.  Place the invoice number in front of the guid. 
									sbINVOICE_NUMBER.Append("[" + sINVOICE_NAME + "] ");
									sbINVOICE_NUMBER.Append(gINVOICE_ID.ToString());
								}
							}
						}
					}
				}
				if ( Sql.IsEmptyGuid(gCREDIT_CARD_ID) )
				{
					throw(new Exception(L10n.Term("CreditCards.ERR_CREDIT_CARD_REQUIRED")));
				}
				
				// 09/13/2013 Paul.  Add support for PayTrace. 
				if ( Sql.ToBoolean(Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Application["CONFIG.PayTrace.UserName"]) )
				{
					string sSQL;
					sSQL = "select vwPAYMENTS_INVOICES.INVOICE_ID                     " + ControlChars.CrLf
					     + "     , vwPAYMENTS_INVOICES.INVOICE_NAME                   " + ControlChars.CrLf
					     + "     , vwINVOICES.BILLING_CONTACT_ID                      " + ControlChars.CrLf
					     + "  from      vwPAYMENTS_INVOICES                           " + ControlChars.CrLf
					     + " inner join vwINVOICES                                    " + ControlChars.CrLf
					     + "         on vwINVOICES.ID = vwPAYMENTS_INVOICES.INVOICE_ID" + ControlChars.CrLf
					     + " where 1 = 1                                              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID");
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string sINVOICE_NAME   = Sql.ToString(row["INVOICE_NAME"      ]);
									Guid   gINVOICE_ID     = Sql.ToGuid  (row["INVOICE_ID"        ]);
									SplendidCRM.PayTraceUtils.Charge(Application, gCURRENCY_ID, gINVOICE_ID, gID, gCREDIT_CARD_ID, Request.UserHostAddress, sINVOICE_NAME);
								}
							}
						}
					}
				}
				// 12/16/2013 Paul.  Add support for Authorize.Net
				else if ( Sql.ToBoolean(Application["CONFIG.AuthorizeNet.Enabled"]) && !Sql.IsEmptyString(Application["CONFIG.AuthorizeNet.UserName"]) )
				{
					string sSQL;
					sSQL = "select vwPAYMENTS_INVOICES.INVOICE_ID                     " + ControlChars.CrLf
					     + "     , vwPAYMENTS_INVOICES.INVOICE_NAME                   " + ControlChars.CrLf
					     + "     , vwINVOICES.BILLING_CONTACT_ID                      " + ControlChars.CrLf
					     + "  from      vwPAYMENTS_INVOICES                           " + ControlChars.CrLf
					     + " inner join vwINVOICES                                    " + ControlChars.CrLf
					     + "         on vwINVOICES.ID = vwPAYMENTS_INVOICES.INVOICE_ID" + ControlChars.CrLf
					     + " where 1 = 1                                              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID");
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string sINVOICE_NAME   = Sql.ToString(row["INVOICE_NAME"      ]);
									Guid   gINVOICE_ID     = Sql.ToGuid  (row["INVOICE_ID"        ]);
									SplendidCRM.AuthorizeNetUtils.Charge(Application, gCURRENCY_ID, gINVOICE_ID, gID, gCREDIT_CARD_ID, Request.UserHostAddress, sINVOICE_NAME);
								}
							}
						}
					}
				}
				// 12/16/2008 Paul.  If the PayPal certificate is defined, then process via PayPal. 
				// 11/15/2009 Paul.  We need a version of the certificate function that accepts the application. 
				// 10/27/2010 Paul.  Skip PayPal if a credit card was selected. 
				else if ( Sql.IsEmptyString(sPAYMENT_GATEWAY_ID) && !Sql.IsEmptyString(PayPalCache.PayPalX509Certificate(Application)) )
				{
					// 12/16/2008 Paul.  When multiple invoices are used, we charge them separately so that the invoices on PayPal would match. 
					string sSQL;
					sSQL = "select vwPAYMENTS_INVOICES.INVOICE_ID                     " + ControlChars.CrLf
					     + "     , vwPAYMENTS_INVOICES.INVOICE_NAME                   " + ControlChars.CrLf
					     + "     , vwINVOICES.BILLING_CONTACT_ID                      " + ControlChars.CrLf
					     + "  from      vwPAYMENTS_INVOICES                           " + ControlChars.CrLf
					     + " inner join vwINVOICES                                    " + ControlChars.CrLf
					     + "         on vwINVOICES.ID = vwPAYMENTS_INVOICES.INVOICE_ID" + ControlChars.CrLf
					     + " where 1 = 1                                              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID");
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string sINVOICE_NAME   = Sql.ToString(row["INVOICE_NAME"      ]);
									Guid   gINVOICE_ID     = Sql.ToGuid  (row["INVOICE_ID"        ]);
									       gB2C_CONTACT_ID = Sql.ToGuid  (row["BILLING_CONTACT_ID"]);
									SplendidCRM.PayPalUtils.Charge(Application, gCURRENCY_ID, gB2C_CONTACT_ID, gINVOICE_ID, gID, gCREDIT_CARD_ID, Request.UserHostAddress, sINVOICE_NAME);
								}
							}
						}
					}
				}
				// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
				else if ( Sql.IsEmptyString(sPAYMENT_GATEWAY_ID) && !Sql.IsEmptyString(PayPalCache.PayPalClientID(Application)) && !Sql.IsEmptyString(PayPalCache.PayPalClientSecret(Application)) )
				{
					// 12/16/2008 Paul.  When multiple invoices are used, we charge them separately so that the invoices on PayPal would match. 
					string sSQL;
					sSQL = "select vwPAYMENTS_INVOICES.INVOICE_ID                     " + ControlChars.CrLf
					     + "     , vwPAYMENTS_INVOICES.INVOICE_NAME                   " + ControlChars.CrLf
					     + "     , vwINVOICES.BILLING_CONTACT_ID                      " + ControlChars.CrLf
					     + "  from      vwPAYMENTS_INVOICES                           " + ControlChars.CrLf
					     + " inner join vwINVOICES                                    " + ControlChars.CrLf
					     + "         on vwINVOICES.ID = vwPAYMENTS_INVOICES.INVOICE_ID" + ControlChars.CrLf
					     + " where 1 = 1                                              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID");
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string sINVOICE_NAME   = Sql.ToString(row["INVOICE_NAME"      ]);
									Guid   gINVOICE_ID     = Sql.ToGuid  (row["INVOICE_ID"        ]);
									       gB2C_CONTACT_ID = Sql.ToGuid  (row["BILLING_CONTACT_ID"]);
									SplendidCRM.PayPalRest.Charge(Application, gCURRENCY_ID, gB2C_CONTACT_ID, gINVOICE_ID, gID, gCREDIT_CARD_ID);
								}
							}
						}
					}
				}
				else
				{
					// 10/19/2010 Paul.  Assume the default gateway, but use the argument to select the gateway. 
					string sPaymentGateway          = Sql.ToString (Application["CONFIG.PaymentGateway"         ]);
					string sPaymentGateway_Login    = Sql.ToString (Application["CONFIG.PaymentGateway_Login"   ]);
					string sPaymentGateway_Password = Sql.ToString (Application["CONFIG.PaymentGateway_Password"]);
					bool   bPaymentGateway_TestMode = Sql.ToBoolean(Application["CONFIG.PaymentGateway_TestMode"]);
					if ( !Sql.IsEmptyString(sPAYMENT_GATEWAY_ID) )
					{
						DataTable dtPaymentGateways = SplendidCache.PaymentGateways(Context);
						DataView vwPaymentGateways = new DataView(dtPaymentGateways);
						// 10/27/2010 Paul.  The argument must be the ID as there can be multiple payment gateways with the same gateway name. 
						vwPaymentGateways.RowFilter = "ID = '" + sPAYMENT_GATEWAY_ID + "'";
						if ( vwPaymentGateways.Count > 0 )
						{
							sPaymentGateway          = Sql.ToString (vwPaymentGateways[0]["GATEWAY"  ]);
							sPaymentGateway_Login    = Sql.ToString (vwPaymentGateways[0]["LOGIN"    ]);
							sPaymentGateway_Password = Sql.ToString (vwPaymentGateways[0]["PASSWORD" ]);
							bPaymentGateway_TestMode = Sql.ToBoolean(vwPaymentGateways[0]["TEST_MODE"]);
						}
					}
					// 04/22/2008 Paul.  We need a unique OrderID for the charge, so add a timestamp. 
					sbINVOICE_NUMBER.Append(" " + DateTime.UtcNow.ToString());
					// 05/01/2013 Paul.  Add Contacts field to support B2C. 
					if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
						SplendidCharge.CC.Charge(Application, gID, gCURRENCY_ID, gB2C_CONTACT_ID, gCREDIT_CARD_ID, Request.UserHostAddress, sbINVOICE_NUMBER.ToString(), sDESCRIPTION, "Sale", dAMOUNT, String.Empty, sPaymentGateway, sPaymentGateway_Login, sPaymentGateway_Password, bPaymentGateway_TestMode);
					else
						SplendidCharge.CC.Charge(Application, gID, gCURRENCY_ID, gACCOUNT_ID, gCREDIT_CARD_ID, Request.UserHostAddress, sbINVOICE_NUMBER.ToString(), sDESCRIPTION, "Sale", dAMOUNT, String.Empty, sPaymentGateway, sPaymentGateway_Login, sPaymentGateway_Password, bPaymentGateway_TestMode);
				}
			}
		}
		
		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 04/22/2008 Paul.  Move the Refund button to the Payment Transactions grid. '
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" || e.CommandName == "Charge" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					// 11/10/2010 Paul.  Apply Business Rules. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + LayoutEditView);
					bool bIsValid = Page.IsValid;

					Decimal dAMOUNT = new DynamicControl(this, "AMOUNT").DecimalValue;
					// 08/26/2010 Paul.  We need a bank fee field to allow for a difference between allocated and received payment. 
					Decimal dBANK_FEE = new DynamicControl(this, "BANK_FEE").DecimalValue;
					if ( (dAMOUNT + dBANK_FEE) != ctlAllocationsView.ALLOCATED_TOTAL )
					{
						// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
						ctlDynamicButtons.ErrorText = L10n.Term("Payments.ERR_AMOUNT_MUST_MATCH_ALLOCATION");
						bIsValid = false;
					}
					if ( bIsValid )
					{
						// 09/09/2009 Paul.  Use the new function to get the table name. 
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *"               + ControlChars.CrLf
								     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, m_sMODULE, "edit");
									Sql.AppendParameter(cmd, gID, "ID", false);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											rowCurrent = dtCurrent.Rows[0];
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												// 03/15/2014 Paul.  DynamicButtons is not used in this area. 
												//ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												//ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
											// It is possible that the record exists, but that ACL rules prevent it from being selected. 
											gID = Guid.Empty;
										}
									}
								}
							}

							Guid     gASSIGNED_USER_ID   = new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID"  ).ID;
							Guid     gACCOUNT_ID         = new DynamicControl(this, rowCurrent, "ACCOUNT_ID"        ).ID;
							// 05/01/2013 Paul.  Add Contacts field to support B2C. 
							Guid     gB2C_CONTACT_ID     = new DynamicControl(this, rowCurrent, "B2C_CONTACT_ID"    ).ID;
							DateTime dtPAYMENT_DATE      = new DynamicControl(this, rowCurrent, "PAYMENT_DATE"      ).DateValue;
							string   sPAYMENT_TYPE       = new DynamicControl(this, rowCurrent, "PAYMENT_TYPE"      ).SelectedValue;
							string   sCUSTOMER_REFERENCE = new DynamicControl(this, rowCurrent, "CUSTOMER_REFERENCE").Text;
							Guid     gCURRENCY_ID        = new DynamicControl(this, rowCurrent, "CURRENCY_ID"       ).ID;
							string   sDESCRIPTION        = new DynamicControl(this, rowCurrent, "DESCRIPTION"       ).Text;
							Guid     gCREDIT_CARD_ID     = new DynamicControl(this, rowCurrent, "CREDIT_CARD_ID"    ).ID;
							string   sPAYMENT_NUM        = new DynamicControl(this, rowCurrent, "PAYMENT_NUM"       ).Text;
							Guid     gTEAM_ID            = new DynamicControl(this, rowCurrent, "TEAM_ID"           ).ID;
							string   sTEAM_SET_LIST      = new DynamicControl(this, rowCurrent, "TEAM_SET_LIST"     ).Text;
							// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
							string   sASSIGNED_SET_LIST  = new DynamicControl(this, rowCurrent, "ASSIGNED_SET_LIST" ).Text;
							// 10/12/2010 Paul.  If the currency field is not displayed, then use the default. 
							if ( Sql.IsEmptyGuid(gCURRENCY_ID) )
								gCURRENCY_ID = C10n.ID;
							// 03/12/2008 Paul.  There might be a temporary credit card value that needs to be cleared. 
							if ( sPAYMENT_TYPE != "Credit Card" )
								gCREDIT_CARD_ID = Guid.Empty;
							// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
							float fEXCHANGE_RATE         = new DynamicControl(ctlAllocationsView, rowCurrent, "EXCHANGE_RATE").FloatValue;
							StringBuilder sbINVOICE_NUMBER = new StringBuilder();
							if ( dtPAYMENT_DATE == DateTime.MinValue || e.CommandName == "Charge" )
								dtPAYMENT_DATE = DateTime.Now;
							
							// 11/10/2010 Paul.  Apply Business Rules. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
									// 08/06/2009 Paul.  PAYMENT_NUM now uses our number sequence table. 
									// 08/26/2010 Paul.  We need a bank fee field to allow for a difference between allocated and received payment. 
									// 05/01/2013 Paul.  Add Contacts field to support B2C. 
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									SqlProcs.spPAYMENTS_Update
										( ref gID
										, gASSIGNED_USER_ID
										, gACCOUNT_ID
										, dtPAYMENT_DATE
										, sPAYMENT_TYPE
										, sCUSTOMER_REFERENCE
										// 05/26/2007 Paul.  Exchange rate is stored in the AllocationsView. 
										, fEXCHANGE_RATE
										, gCURRENCY_ID
										, dAMOUNT
										, sDESCRIPTION
										, gCREDIT_CARD_ID
										, sPAYMENT_NUM
										, gTEAM_ID
										, sTEAM_SET_LIST
										, dBANK_FEE
										, gB2C_CONTACT_ID
										, sASSIGNED_SET_LIST
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									
									DataTable dtLineItems = ctlAllocationsView.LineItems;
									// 03/15/2011 Paul.  Some customers want to be able to save a payment without applying to an invoice. 
									if ( dtLineItems != null )
									{
										// 03/27/2007 Paul.  Delete records before performing inserts or updates.
										// 06/17/2008 Paul.  If this was a duplication operation, then don't delete the original line items. 
										Guid gDuplicateID = Sql.ToGuid(ViewState["DuplicateID"]);
										if ( Sql.IsEmptyGuid(gDuplicateID) )
										{
											foreach ( DataRow row in dtLineItems.Rows )
											{
												if ( row.RowState == DataRowState.Deleted )
												{
													// 05/26/2007 Paul.  In order to access values from deleted row, use DataRowVersion.Original, 
													// otherwise accessing the data will throw an exception "Deleted row information cannot be accessed through the row."
													Guid gITEM_ID = Sql.ToGuid(row["ID", DataRowVersion.Original]);
													if ( !Sql.IsEmptyGuid(gITEM_ID) )
														SqlProcs.spINVOICES_PAYMENTS_Delete(gITEM_ID, trn);
												}
											}
										}
										foreach ( DataRow row in dtLineItems.Rows )
										{
											if ( row.RowState != DataRowState.Deleted )
											{
												Guid    gITEM_ID        = Sql.ToGuid   (row["ID"             ]);
												Guid    gINVOICE_ID     = Sql.ToGuid   (row["INVOICE_ID"     ]);
												Decimal dINVOICE_AMOUNT = Sql.ToDecimal(row["AMOUNT"         ]);
												// 09/29/2008 Paul.  Include the invoice number in the gateway transaction id. 
												// 10/01/2008 Paul.  Invoice Name works better. 
												string  sINVOICE_NAME   = Sql.ToString (row["INVOICE_NAME"   ]);

												// 03/27/2007 Paul.  Only add if product is defined.  This will exclude the blank row. 
												if ( !Sql.IsEmptyGuid(gINVOICE_ID) )
												{
													SqlProcs.spINVOICES_PAYMENTS_Update
														( ref gITEM_ID
														, gINVOICE_ID
														, gID
														, dINVOICE_AMOUNT
														, trn
														);
													if ( sbINVOICE_NUMBER.Length > 0 )
														sbINVOICE_NUMBER.Append(",");
													// 09/29/2008 Paul.  Place the invoice number in front of the guid. 
													sbINVOICE_NUMBER.Append("[" + sINVOICE_NAME + "] ");
													sbINVOICE_NUMBER.Append(gINVOICE_ID.ToString());
												}
											}
										}
									}
									trn.Commit();
									// 02/11/2008 Paul.  We need to save the ID so that a new record would not get created if the Charge Now operation fails. 
									ViewState["ID"] = gID;
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
									ctlDynamicButtons.ErrorText = ex.Message;
									return;
								}
								string sSQL;
								// 08/26/2010 Paul.  Add new record to tracker. 
								sSQL = "select PAYMENT_NUM    " + ControlChars.CrLf
								     + "  from vwPAYMENTS_Edit" + ControlChars.CrLf
								     + " where ID = @ID       " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
									string sNAME = Sql.ToString(cmd.ExecuteScalar());
									// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
									SqlProcs.spTRACKER_Update
										( Security.USER_ID
										, m_sMODULE
										, gID
										, sNAME
										, "save"
										);
								}
								// 10/27/2010 Paul.  We need to update the last modified date after a save to prevent a concurrency error ifthe charge fails. 
								sSQL = "select DATE_MODIFIED  " + ControlChars.CrLf
								     + "  from vwPAYMENTS_Edit" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, m_sMODULE, "edit");
									Sql.AppendParameter(cmd, gID, "ID", false);
									ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(cmd.ExecuteScalar());
								}
							}
							// 04/22/2008 Paul.  Move the Refund button to the Payment Transactions grid. 
							if ( e.CommandName == "Charge" )
							{
								// 05/08/2022 Paul.  Move to static method so that we can call from Rest API. 
								Charge(Context, gID, Sql.ToString(e.CommandArgument));
							}
							// 11/10/2010 Paul.  Apply Business Rules. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
						}
						
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						else
							Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		protected void CURRENCY_ID_Changed(object sender, System.EventArgs e)
		{
			ctlAllocationsView.CURRENCY_ID_Changed(sender, e);
		}

		protected void PAYMENT_TYPE_Changed(object sender, System.EventArgs e)
		{
			if ( PAYMENT_TYPE != null )
			{
				bool bMultipleGateways = (dtPaymentGateways != null && dtPaymentGateways.Rows.Count > 1);
				
				bool bCreditCard = PAYMENT_TYPE.SelectedValue == "Credit Card";
				// 02/11/2008 Paul.  We need to add more logic to the show functions. 
				// Charge should only apply if no charge was successful or is pending. 
				// Refund should only apply if charge was successful and not previous refunds. 
				//btnCharge.Visible = bCreditCard && !bMultipleGateways;
				// 10/19/2010 Paul.  Only show the gateway selection if there are multiple gateways. 
				//ctlGatewayRepeater.Visible = bCreditCard &&  bMultipleGateways;
				// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons.  Disable all buttons. 
				ctlDynamicButtons.ShowButton("Charge", bCreditCard);

				// 04/22/2008 Paul.  Move the Refund button to the Payment Transactions grid. 
				//ctlDynamicButtons.ShowButton("Refund", bCreditCard);
				new DynamicControl(this, "CREDIT_CARD_ID_LABEL"    ).Visible = bCreditCard;
				new DynamicControl(this, "CREDIT_CARD_NAME"        ).Visible = bCreditCard;
				new DynamicControl(this, "CREDIT_CARD_ID_btnChange").Visible = bCreditCard;
				new DynamicControl(this, "CREDIT_CARD_ID_btnClear" ).Visible = bCreditCard;
				// 03/12/2008 Paul.  If the payment type has changed, then lookup the primary card. 
				if ( bCreditCard && Sql.IsEmptyGuid(new DynamicControl(this, "CREDIT_CARD_ID").ID) )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						// 05/01/2013 Paul.  Add Contacts field to support B2C. 
						sSQL = "select *             " + ControlChars.CrLf
						     + "  from vwCREDIT_CARDS" + ControlChars.CrLf
						     + " where IS_PRIMARY = 1" + ControlChars.CrLf
						     + "   and (ACCOUNT_ID = @ACCOUNT_ID or CONTACT_ID = @CONTACT_ID)" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 05/01/2013 Paul.  Add Contacts field to support B2C. 
							Guid gACCOUNT_ID     = new DynamicControl(this, "ACCOUNT_ID"    ).ID;
							Guid gB2C_CONTACT_ID = new DynamicControl(this, "B2C_CONTACT_ID").ID;
							Sql.AddParameter(cmd, "ACCOUNT_ID", gACCOUNT_ID    );
							Sql.AddParameter(cmd, "CONTACT_ID", gB2C_CONTACT_ID);
							con.Open();

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									new DynamicControl(this, "CREDIT_CARD_ID"  ).ID   = Sql.ToGuid  (rdr["ID"  ]);
									new DynamicControl(this, "CREDIT_CARD_NAME").Text = Sql.ToString(rdr["NAME"]);
								}
							}
						}
					}
				}
			}
		}

		protected void BANK_FEE_TextChanged(object sender, EventArgs e)
		{
			Decimal dAMOUNT   = new DynamicControl(this, "AMOUNT"  ).DecimalValue;
			Decimal dBANK_FEE = new DynamicControl(this, "BANK_FEE").DecimalValue;
			dAMOUNT = ctlAllocationsView.ALLOCATED_TOTAL - dBANK_FEE;
			new DynamicControl(this, "AMOUNT").Text = dAMOUNT.ToString("0.00");
		}

		protected void AMOUNT_TextChanged(object sender, EventArgs e)
		{
			Decimal dAMOUNT   = new DynamicControl(this, "AMOUNT"  ).DecimalValue;
			Decimal dBANK_FEE = new DynamicControl(this, "BANK_FEE").DecimalValue;
			dBANK_FEE = ctlAllocationsView.ALLOCATED_TOTAL - dAMOUNT;
			new DynamicControl(this, "BANK_FEE").Text = dBANK_FEE.ToString("0.00");
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				//tdRequired.DataBind();
				//dtPaymentGateways = SplendidCache.PaymentGateways(Context);
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( Sql.IsEmptyGuid(gID) )
					gID = Sql.ToGuid(ViewState["ID"]);
				if ( !IsPostBack )
				{
					// 10/28/2010 Paul.  Create the buttons manually so that we can create a Charge button for each available gateway. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons.  Disable all buttons. 
					//ctlGatewayRepeater.DataSource = dtPaymentGateways;
					//ctlGatewayRepeater.DataBind();
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					// 06/17/2008 Paul.  We need to save the DuplicateID so that we can make sure not to delete original line items. 
					ViewState["DuplicateID"] = gDuplicateID;
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
							sSQL = "select *"               + ControlChars.CrLf
							     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
							     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AppendParameter(cmd, gDuplicateID, "ID", false);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AppendParameter(cmd, gID, "ID", false);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										// 10/31/2017 Paul.  Provide a way to inject Record level ACL. 
										if ( dtCurrent.Rows.Count > 0 && (SplendidCRM.Security.GetRecordAccess(dtCurrent.Rows[0], m_sMODULE, "edit", "ASSIGNED_USER_ID") >= 0) )
										{
											DataRow rdr = dtCurrent.Rows[0];
											// 11/11/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["PAYMENT_NUM"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);

											InitControls();
											PAYMENT_TYPE_Changed(null, null);
											// 10/02/2017 Paul.  We needed to make sure that the number gets reset when copying a record. 
											if ( !Sql.IsEmptyGuid(ViewState["DuplicateID"]) )
											{
												new DynamicControl(this, "PAYMENT_NUM").Text = String.Empty;
											}
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											// 12/16/2008 Paul.  LoadLineItems will close the rdr, so get the date before loading the items. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											
											Guid   gACCOUNT_ID   = Sql.ToGuid  (rdr["ACCOUNT_ID"  ]);
											string sACCOUNT_NAME = Sql.ToString(rdr["ACCOUNT_NAME"]);
											// 08/04/2010 Paul.  Need to detect an Account ID change. 
											ViewState["LAST_ACCOUNT_ID"  ] = gACCOUNT_ID  ;
											ViewState["LAST_ACCOUNT_NAME"] = sACCOUNT_NAME;
											// 05/01/2013 Paul.  Add Contacts field to support B2C. 
											Guid   gB2C_CONTACT_ID   = Sql.ToGuid  (rdr["B2C_CONTACT_ID"  ]);
											string sB2C_CONTACT_NAME = Sql.ToString(rdr["B2C_CONTACT_NAME"]);
											ViewState["LAST_B2C_CONTACT_ID"  ] = gB2C_CONTACT_ID  ;
											ViewState["LAST_B2C_CONTACT_NAME"] = sB2C_CONTACT_NAME;
											
											ctlAllocationsView.LoadLineItems(gID, gACCOUNT_ID, gB2C_CONTACT_ID, gDuplicateID, con, rdr);
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
										}
										else
										{
											// 05/01/2013 Paul.  Add Contacts field to support B2C. 
											Guid gACCOUNT_ID     = Guid.Empty;
											Guid gB2C_CONTACT_ID = Guid.Empty;
											Guid gPARENT_ID      = Sql.ToGuid(Request["PARENT_ID"]);
											string sMODULE       = String.Empty;
											string sPARENT_TYPE  = String.Empty;
											string sPARENT_NAME  = String.Empty;
											SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
											// 08/04/2010 Paul.  Need to detect an Account ID change. 
											if ( sMODULE == "Accounts" )
												ViewState["LAST_ACCOUNT_ID"] = gPARENT_ID;
											else if ( sMODULE == "Contacts" )
												ViewState["LAST_B2C_CONTACT_ID"] = gPARENT_ID;
											ctlAllocationsView.LoadLineItems(gID, gACCOUNT_ID, gB2C_CONTACT_ID, gDuplicateID, con, null);
											
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons.  Disable all buttons. 
											//btnSave  .Enabled = false;
											//btnCancel.Enabled = false;
											//btnCharge.Enabled = false;
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);

						InitControls();
						// 03/17/2011 Paul.  Set default value for bank fee. 
						new DynamicControl(this, "BANK_FEE").Text = "0.00";
						// 05/28/2007 Paul.  Prepopulate the Account. 
						Guid gACCOUNT_ID     = Guid.Empty;
						Guid gB2C_CONTACT_ID = Guid.Empty;
						Guid gPARENT_ID      = Sql.ToGuid(Request["PARENT_ID"]);
						new DynamicControl(this, "PAYMENT_DATE").DateValue = DateTime.Today;
						Guid gINVOICE_ID = gPARENT_ID;
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							// 04/14/2016 Paul.  New spPARENT_GetWithTeam procedure so that we can inherit Assigned To and Team values. 
							string sMODULE           = String.Empty;
							string sPARENT_TYPE      = String.Empty;
							string sPARENT_NAME      = String.Empty;
							Guid   gASSIGNED_USER_ID = Guid.Empty;
							string sASSIGNED_TO      = String.Empty;
							string sASSIGNED_TO_NAME = String.Empty;
							Guid   gTEAM_ID          = Guid.Empty;
							string sTEAM_NAME        = String.Empty;
							Guid   gTEAM_SET_ID      = Guid.Empty;
							// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
							Guid   gASSIGNED_SET_ID  = Guid.Empty;
							SqlProcs.spPARENT_GetWithTeam(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME, ref gASSIGNED_USER_ID, ref sASSIGNED_TO, ref sASSIGNED_TO_NAME, ref gTEAM_ID, ref sTEAM_NAME, ref gTEAM_SET_ID, ref gASSIGNED_SET_ID);
							if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Accounts" )
							{
								gACCOUNT_ID = gPARENT_ID;
								new DynamicControl(this, "ACCOUNT_ID"  ).ID   = gPARENT_ID;
								new DynamicControl(this, "ACCOUNT_NAME").Text = sPARENT_NAME;
								// 04/14/2016 Paul.  New spPARENT_GetWithTeam procedure so that we can inherit Assigned To and Team values. 
								if ( Sql.ToBoolean(Application["CONFIG.inherit_assigned_user"]) )
								{
									new DynamicControl(this, "ASSIGNED_USER_ID").ID   = gASSIGNED_USER_ID;
									new DynamicControl(this, "ASSIGNED_TO"     ).Text = sASSIGNED_TO     ;
									new DynamicControl(this, "ASSIGNED_TO_NAME").Text = sASSIGNED_TO_NAME;
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									if ( Crm.Config.enable_dynamic_assignment() )
									{
										SplendidCRM._controls.UserSelect ctlUserSelect = FindControl("ASSIGNED_SET_NAME") as SplendidCRM._controls.UserSelect;
										if ( ctlUserSelect != null )
											ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true, true);
									}
								}
								if ( Sql.ToBoolean(Application["CONFIG.inherit_team"]) )
								{
									new DynamicControl(this, "TEAM_ID"  ).ID   = gTEAM_ID  ;
									new DynamicControl(this, "TEAM_NAME").Text = sTEAM_NAME;
									SplendidCRM._controls.TeamSelect ctlTeamSelect = FindControl("TEAM_SET_NAME") as SplendidCRM._controls.TeamSelect;
									if ( ctlTeamSelect != null )
										ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true, true);
								}
								// 08/04/2010 Paul.  Need to detect an Account ID change. 
								ViewState["LAST_ACCOUNT_ID"  ] = gACCOUNT_ID ;
								ViewState["LAST_ACCOUNT_NAME"] = sPARENT_NAME;
								// 02/25/2008 Paul.  We need to pass the Account ID, not the Invoice ID. 
								ctlAllocationsView.LoadLineItems(gID, gACCOUNT_ID, gB2C_CONTACT_ID, gDuplicateID, null, null);
							}
							// 05/01/2013 Paul.  Add Contacts field to support B2C. 
							else if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Contacts" )
							{
								gB2C_CONTACT_ID = gPARENT_ID;
								new DynamicControl(this, "B2C_CONTACT_ID"  ).ID   = gPARENT_ID;
								new DynamicControl(this, "B2C_CONTACT_NAME").Text = sPARENT_NAME;
								// 04/14/2016 Paul.  New spPARENT_GetWithTeam procedure so that we can inherit Assigned To and Team values. 
								if ( Sql.ToBoolean(Application["CONFIG.inherit_assigned_user"]) )
								{
									new DynamicControl(this, "ASSIGNED_USER_ID").ID   = gASSIGNED_USER_ID;
									new DynamicControl(this, "ASSIGNED_TO"     ).Text = sASSIGNED_TO     ;
									new DynamicControl(this, "ASSIGNED_TO_NAME").Text = sASSIGNED_TO_NAME;
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									if ( Crm.Config.enable_dynamic_assignment() )
									{
										SplendidCRM._controls.UserSelect ctlUserSelect = FindControl("ASSIGNED_SET_NAME") as SplendidCRM._controls.UserSelect;
										if ( ctlUserSelect != null )
											ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true, true);
									}
								}
								if ( Sql.ToBoolean(Application["CONFIG.inherit_team"]) )
								{
									new DynamicControl(this, "TEAM_ID"  ).ID   = gTEAM_ID  ;
									new DynamicControl(this, "TEAM_NAME").Text = sTEAM_NAME;
									SplendidCRM._controls.TeamSelect ctlTeamSelect = FindControl("TEAM_SET_NAME") as SplendidCRM._controls.TeamSelect;
									if ( ctlTeamSelect != null )
										ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true, true);
								}
								// 08/04/2010 Paul.  Need to detect an Account ID change. 
								ViewState["LAST_B2C_CONTACT_ID"  ] = gB2C_CONTACT_ID ;
								ViewState["LAST_B2C_CONTACT_NAME"] = sPARENT_NAME;
								// 02/25/2008 Paul.  We need to pass the Account ID, not the Invoice ID. 
								ctlAllocationsView.LoadLineItems(gID, gACCOUNT_ID, gB2C_CONTACT_ID, gDuplicateID, null, null);
							}
							else if ( !Sql.IsEmptyGuid(gINVOICE_ID) )
							{
								// 04/14/2016 Paul.  New spPARENT_GetWithTeam procedure so that we can inherit Assigned To and Team values. 
								if ( Sql.ToBoolean(Application["CONFIG.inherit_assigned_user"]) )
								{
									new DynamicControl(this, "ASSIGNED_USER_ID").ID   = gASSIGNED_USER_ID;
									new DynamicControl(this, "ASSIGNED_TO"     ).Text = sASSIGNED_TO     ;
									new DynamicControl(this, "ASSIGNED_TO_NAME").Text = sASSIGNED_TO_NAME;
									// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
									if ( Crm.Config.enable_dynamic_assignment() )
									{
										SplendidCRM._controls.UserSelect ctlUserSelect = FindControl("ASSIGNED_SET_NAME") as SplendidCRM._controls.UserSelect;
										if ( ctlUserSelect != null )
											ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true, true);
									}
								}
								if ( Sql.ToBoolean(Application["CONFIG.inherit_team"]) )
								{
									new DynamicControl(this, "TEAM_ID"  ).ID   = gTEAM_ID  ;
									new DynamicControl(this, "TEAM_NAME").Text = sTEAM_NAME;
									SplendidCRM._controls.TeamSelect ctlTeamSelect = FindControl("TEAM_SET_NAME") as SplendidCRM._controls.TeamSelect;
									if ( ctlTeamSelect != null )
										ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true, true);
								}
								// 05/282/007 Paul.  spPARENT_Get will not return Invoices, so get the hard way.
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									string sSQL ;
									sSQL = "select *              " + ControlChars.CrLf
									     + "  from vwINVOICES_Edit" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Security.Filter(cmd, "Invoices", "edit");
										Sql.AppendParameter(cmd, gINVOICE_ID, "ID", false);
										con.Open();

										if ( bDebug )
											RegisterClientScriptBlock("vwINVOICES_Edit", Sql.ClientScriptBlock(cmd));

										using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
										{
											if ( rdr.Read() )
											{
												gACCOUNT_ID =Sql.ToGuid  (rdr["BILLING_ACCOUNT_ID"  ]);
												new DynamicControl(this, "ACCOUNT_ID"  ).ID   = Sql.ToGuid  (rdr["BILLING_ACCOUNT_ID"  ]);
												new DynamicControl(this, "ACCOUNT_NAME").Text = Sql.ToString(rdr["BILLING_ACCOUNT_NAME"]);
												// 08/04/2010 Paul.  Need to detect an Account ID change. 
												ViewState["LAST_ACCOUNT_ID"  ] = gACCOUNT_ID;
												ViewState["LAST_ACCOUNT_NAME"] = Sql.ToString(rdr["BILLING_ACCOUNT_NAME"]);
												
												// 05/01/2013 Paul.  Add Contacts field to support B2C. 
												gB2C_CONTACT_ID =Sql.ToGuid  (rdr["BILLING_CONTACT_ID"  ]);
												new DynamicControl(this, "B2C_CONTACT_ID"  ).ID   = Sql.ToGuid  (rdr["BILLING_CONTACT_ID"  ]);
												new DynamicControl(this, "B2C_CONTACT_NAME").Text = Sql.ToString(rdr["BILLING_CONTACT_NAME"]);
												ViewState["LAST_B2C_CONTACT_ID"  ] = gB2C_CONTACT_ID;
												ViewState["LAST_B2C_CONTACT_NAME"] = Sql.ToString(rdr["BILLING_CONTACT_NAME"]);

												// 02/25/2008 Paul.  We need to pass the Account ID, not the Invoice ID. 
												ctlAllocationsView.LoadLineItems(gID, gACCOUNT_ID, gB2C_CONTACT_ID, gDuplicateID, null, null);
												// 02/25/2008 Paul.  On record creation, prepopulate the allocated amount. 
												// 04/23/2008 Paul.  Make sure to convert to the current currency value. Use Amount Due. 
												new DynamicControl(this, "AMOUNT").Text = C10n.ToCurrency(Sql.ToDecimal(rdr["AMOUNT_DUE_USDOLLAR"])).ToString("0.00");
											}
										}
									}
								}
							}
						}
						PAYMENT_TYPE_Changed(null, null);
						// 11/10/2010 Paul.  Apply Business Rules. 
						this.ApplyEditViewNewEventRules(m_sMODULE + "." + LayoutEditView);
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
					// 08/05/2010 Paul.  We are having problems trying to trap the Acocunt change event. 
					// The AutoPostBack is submitting before AutoComplete finishes, so we need to lookup the account. 
					Guid   gACCOUNT_ID     = Guid.Empty;
					Guid   gB2C_CONTACT_ID = Guid.Empty;
					string sACCOUNT_NAME = new DynamicControl(this, "ACCOUNT_NAME").Text;
					if ( sACCOUNT_NAME != Sql.ToString(ViewState["LAST_ACCOUNT_NAME"]) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID          " + ControlChars.CrLf
							     + "  from vwACCOUNTS  " + ControlChars.CrLf
							     + " where NAME = @NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@NAME", sACCOUNT_NAME);
								gACCOUNT_ID = Sql.ToGuid(cmd.ExecuteScalar());
							}
						}
						// 08/04/2010 Paul.  Need to detect an Account ID change. 
						if ( gACCOUNT_ID != Sql.ToGuid(ViewState["LAST_ACCOUNT_ID"]) )
						{
							ViewState["LAST_ACCOUNT_ID"  ] = gACCOUNT_ID  ;
							ViewState["LAST_ACCOUNT_NAME"] = sACCOUNT_NAME;
							ctlAllocationsView.LoadLineItems(gID, gACCOUNT_ID, gB2C_CONTACT_ID, Guid.Empty, null, null);
							PAYMENT_TYPE_Changed(null, null);
						}
					}
					// 05/01/2013 Paul.  Add Contacts field to support B2C. 
					string sB2C_CONTACT_NAME = new DynamicControl(this, "B2C_CONTACT_NAME").Text;
					if ( sB2C_CONTACT_NAME != Sql.ToString(ViewState["LAST_B2C_CONTACT_NAME"]) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID          " + ControlChars.CrLf
							     + "  from vwCONTACTS  " + ControlChars.CrLf
							     + " where NAME = @NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@NAME", sB2C_CONTACT_NAME);
								gB2C_CONTACT_ID = Sql.ToGuid(cmd.ExecuteScalar());
							}
						}
						// 08/04/2010 Paul.  Need to detect an Contact ID change. 
						if ( gB2C_CONTACT_ID != Sql.ToGuid(ViewState["LAST_B2C_CONTACT_ID"]) )
						{
							ViewState["LAST_B2C_CONTACT_ID"  ] = gB2C_CONTACT_ID  ;
							ViewState["LAST_B2C_CONTACT_NAME"] = sB2C_CONTACT_NAME;
							ctlAllocationsView.LoadLineItems(gID, gACCOUNT_ID, gB2C_CONTACT_ID, Guid.Empty, null, null);
							PAYMENT_TYPE_Changed(null, null);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void InitControls()
		{
			try
			{
				CURRENCY_ID = tblMain.FindControl("CURRENCY_ID") as DropDownList;
				if ( CURRENCY_ID != null )
				{
					CURRENCY_ID.AutoPostBack = true;
					CURRENCY_ID.SelectedIndexChanged += new EventHandler(CURRENCY_ID_Changed);
				}
				// 12/10/2010 Paul.  Auto-adjust bank fee and account. 
				BANK_FEE = tblMain.FindControl("BANK_FEE") as TextBox;
				if ( BANK_FEE != null )
				{
					BANK_FEE.AutoPostBack = true;
					BANK_FEE.TextChanged += new EventHandler(BANK_FEE_TextChanged);
				}
				AMOUNT   = tblMain.FindControl("AMOUNT"  ) as TextBox;
				if ( AMOUNT != null )
				{
					AMOUNT.AutoPostBack = true;
					AMOUNT.TextChanged += new EventHandler(AMOUNT_TextChanged);
				}
				// 02/09/2008 Paul.  We need to enable the Charge Now button if the type is Credit Card. 
				PAYMENT_TYPE = tblMain.FindControl("PAYMENT_TYPE") as DropDownList;
				if ( PAYMENT_TYPE != null )
				{
					PAYMENT_TYPE.AutoPostBack = true;
					PAYMENT_TYPE.SelectedIndexChanged += new EventHandler(PAYMENT_TYPE_Changed);
				
					// 08/16/2010 Paul.  The CREDIT_CARD_NAME is appearing unexpectedly. Lets update the visibility on all postbacks. 
					// 10/27/2010 Paul.  Should not need to modify the visibility flag. 
					//bool bCreditCard = PAYMENT_TYPE.SelectedValue == "Credit Card";
					//new DynamicControl(this, "CREDIT_CARD_NAME").Visible = bCreditCard;
				}
				// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
				dtPaymentGateways = SplendidCache.PaymentGateways(Context);
				ctlDynamicButtons.AppendButton("Save"  , "", L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ), L10n.Term(".LBL_SAVE_BUTTON_TITLE"  ), "EditHeader");
				ctlDynamicButtons.AppendButton("Cancel", "", L10n.Term(".LBL_CANCEL_BUTTON_LABEL"), L10n.Term(".LBL_CANCEL_BUTTON_TITLE"), "EditHeader");
				if ( dtPaymentGateways.Rows.Count == 1 )
				{
					ctlDynamicButtons.AppendButton("Charge", String.Empty, L10n.Term("Payments.LBL_CHARGE_BUTTON_LABEL"), L10n.Term("Payments.LBL_CHARGE_BUTTON_TITLE"), "EditHeader");
				}
				else if ( dtPaymentGateways.Rows.Count > 1 )
				{
					foreach ( DataRow row in dtPaymentGateways.Rows )
					{
						Guid   gGATEWAY_ID   = Sql.ToGuid  (row["ID"  ]);
						string sGATEWAY_NAME = Sql.ToString(row["NAME"]);
						ctlDynamicButtons.AppendButton("Charge", gGATEWAY_ID.ToString(), L10n.Term("Payments.LBL_CHARGE_BUTTON_LABEL") + " - " + sGATEWAY_NAME, L10n.Term("Payments.LBL_CHARGE_BUTTON_TITLE") + " - " + sGATEWAY_NAME, "EditHeader");
					}
				}
				// 12/17/2015 Paul.  We need to manually add the charge for separate payment gateways. 
				else if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) )
				{
					ctlDynamicButtons.AppendButton("Charge", String.Empty, L10n.Term("Payments.LBL_CHARGE_BUTTON_LABEL"), L10n.Term("Payments.LBL_CHARGE_BUTTON_TITLE"), "EditHeader");
				}
				else if ( Sql.ToBoolean(Application["CONFIG.AuthorizeNet.Enabled"]) && !Sql.IsEmptyString(Application["CONFIG.AuthorizeNet.UserName"]) )
				{
					ctlDynamicButtons.AppendButton("Charge", String.Empty, L10n.Term("Payments.LBL_CHARGE_BUTTON_LABEL"), L10n.Term("Payments.LBL_CHARGE_BUTTON_TITLE"), "EditHeader");
				}
				else if ( !Sql.IsEmptyString(Application["CONFIG.PayPal.ClientID"]) && !Sql.IsEmptyString(Application["CONFIG.PayPal.ClientSecret"]) )
				{
					ctlDynamicButtons.AppendButton("Charge", String.Empty, L10n.Term("Payments.LBL_CHARGE_BUTTON_LABEL"), L10n.Term("Payments.LBL_CHARGE_BUTTON_TITLE"), "EditHeader");
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
			// 09/07/2015 Paul.  Prior to HeaderButtons, the events were wired in the UI. 
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Payments";
			// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
			m_sVIEW_NAME = "vw" + Crm.Modules.TableName(m_sMODULE) + "_Edit";
			// 03/15/2011 Paul.  Change menu to use main module. 
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);

				InitControls();
				// 08/16/2010 Paul.  The CREDIT_CARD_NAME is appearing unexpectedly. Lets update the visibility on all postbacks. 
				// 10/27/2010 Paul.  Should not need to modify the visibility flag. 
				//bool bCreditCard = new DynamicControl(this, "PAYMENT_TYPE").SelectedValue == "Credit Card";
				//new DynamicControl(this, "CREDIT_CARD_NAME").Visible = bCreditCard;
				// 11/10/2010 Paul.  Make sure to add the RulesValidator early in the pipeline. 
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}

