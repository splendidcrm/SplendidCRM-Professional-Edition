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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Payments
{
	/// <summary>
	///		Summary description for PaymentTransactions.
	/// </summary>
	public class PaymentTransactions : SplendidControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Refund" )
				{
					DataTable dtPaymentGateways = SplendidCache.PaymentGateways(Context);
					Guid gPAYMENTS_TRANSACTION_ID = Sql.ToGuid(e.CommandArgument);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL;
						sSQL = "select *                      " + ControlChars.CrLf
						     + "  from vwPAYMENTS_TRANSACTIONS" + ControlChars.CrLf
						     + " where 1 = 1                  " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AppendParameter(cmd, gPAYMENTS_TRANSACTION_ID, "ID", false);
							con.Open();

							if ( bDebug )
								RegisterClientScriptBlock("vwINVOICES_Edit", Sql.ClientScriptBlock(cmd));

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
									{
										DataRow rdr = dt.Rows[0];
										Guid    gPAYMENT_ID        = Sql.ToGuid   (rdr["PAYMENT_ID"       ]);
										Guid    gCURRENCY_ID       = Sql.ToGuid   (rdr["CURRENCY_ID"      ]);
										Guid    gACCOUNT_ID        = Sql.ToGuid   (rdr["ACCOUNT_ID"       ]);
										Guid    gCREDIT_CARD_ID    = Sql.ToGuid   (rdr["CREDIT_CARD_ID"   ]);
										Guid    gINVOICE_ID        = Sql.ToGuid   (rdr["INVOICE_ID"       ]);
										string  sINVOICE_NUMBER    = Sql.ToString (rdr["INVOICE_NUMBER"   ]);
										Decimal dAMOUNT            = Sql.ToDecimal(rdr["AMOUNT"           ]);
										string  sDESCRIPTION       = Sql.ToString (rdr["DESCRIPTION"      ]);
										string sTRANSACTION_NUMBER = Sql.ToString(rdr["TRANSACTION_NUMBER"]);
										// 09/13/2013 Paul.  Add Contacts field to support B2C. 
										if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
											gACCOUNT_ID = Sql.ToGuid   (rdr["CONTACT_ID"]);
									
										// 09/13/2013 Paul.  Add support for PayTrace. 
										if ( Sql.ToBoolean(Context.Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.PayTrace.UserName"]) )
										{
											SplendidCRM.PayTraceUtils.Refund(Application, gPAYMENT_ID, gCURRENCY_ID, gACCOUNT_ID, gCREDIT_CARD_ID, sINVOICE_NUMBER, dAMOUNT, sTRANSACTION_NUMBER);
										}
										// 12/16/2013 Paul.  Add support for Authorize.Net
										else if ( Sql.ToBoolean(Context.Application["CONFIG.AuthorizeNet.Enabled"]) && !Sql.IsEmptyString(Context.Application["CONFIG.AuthorizeNet.UserName"]) )
										{
											SplendidCRM.AuthorizeNetUtils.Refund(Application, gPAYMENT_ID, gCURRENCY_ID, gACCOUNT_ID, gCREDIT_CARD_ID, sINVOICE_NUMBER, dAMOUNT, sTRANSACTION_NUMBER);
										}
										// 09/07/2015 Paul.  Add support for PayPal refund using REST API. 
										else if ( !Sql.IsEmptyString(PayPalCache.PayPalClientID(Application)) && !Sql.IsEmptyString(PayPalCache.PayPalClientSecret(Application)) )
										{
											SplendidCRM.PayPalRest.Refund(Application, gPAYMENT_ID, gCURRENCY_ID, gACCOUNT_ID, gCREDIT_CARD_ID, sINVOICE_NUMBER, dAMOUNT, sTRANSACTION_NUMBER);
										}
										else
										{
											// 10/19/2010 Paul.  Pull the gateway from the transaction so that we can process the refund using the same gateway. 
											string sPAYMENT_GATEWAY    = Sql.ToString (rdr["PAYMENT_GATEWAY"   ]);
											string sPaymentGateway          = Sql.ToString (Application["CONFIG.PaymentGateway"         ]);
											string sPaymentGateway_Login    = Sql.ToString (Application["CONFIG.PaymentGateway_Login"   ]);
											string sPaymentGateway_Password = Sql.ToString (Application["CONFIG.PaymentGateway_Password"]);
											bool   bPaymentGateway_TestMode = Sql.ToBoolean(Application["CONFIG.PaymentGateway_TestMode"]);
											// 04/21/2016 Paul.  Manual is a special gateway type that means that the refund will be processed manually. 
											if ( sPAYMENT_GATEWAY == "Manual" )
											{
												Guid gREFUND_PAYMENTS_TRANSACTION_ID = Guid.Empty;
												using ( IDbTransaction trn = Sql.BeginTransaction(con) )
												{
													try
													{
														SqlProcs.spPAYMENTS_TRANSACTIONS_InsertOnly
															( ref gREFUND_PAYMENTS_TRANSACTION_ID
															, gPAYMENT_ID
															, sPAYMENT_GATEWAY
															, "Refund"               // TRANSACTION_TYPE
															, dAMOUNT
															, gCURRENCY_ID
															, sINVOICE_NUMBER
															, sDESCRIPTION
															, Guid.Empty             // CREDIT_CARD_ID
															, gACCOUNT_ID
															, "Success"              // STATUS
															, trn
															);
														SqlProcs.spINVOICES_UpdateAmountDue(gINVOICE_ID, trn);
														trn.Commit();
													}
													catch(Exception ex)
													{
														trn.Rollback();
														throw(new Exception(ex.Message, ex.InnerException));
													}
												}
												BindGrid();
												return;
											}
											else if ( !Sql.IsEmptyString(sPAYMENT_GATEWAY) )
											{
												// 10/28/2010 Paul.  The login is included in the PAYMENT_GATEWAY field so we need to extract it. 
												DataView vwPaymentGateways = new DataView(dtPaymentGateways);
												if ( sPAYMENT_GATEWAY.Contains(":") )
												{
													string[] arrPAYMENT_GATEWAY = sPAYMENT_GATEWAY.Split(':');
													string   sGATEWAY = arrPAYMENT_GATEWAY[0].Trim();
													string   sLOGIN   = arrPAYMENT_GATEWAY[1].Trim();
													vwPaymentGateways.RowFilter = "GATEWAY = '" + sGATEWAY + "' and LOGIN = '" + sLOGIN + "'";
												}
												else
												{
													vwPaymentGateways.RowFilter = "GATEWAY = '" + sPAYMENT_GATEWAY + "'";
												}
												if ( vwPaymentGateways.Count > 0 )
												{
													sPaymentGateway          = Sql.ToString (vwPaymentGateways[0]["GATEWAY"  ]);
													sPaymentGateway_Login    = Sql.ToString (vwPaymentGateways[0]["LOGIN"    ]);
													sPaymentGateway_Password = Sql.ToString (vwPaymentGateways[0]["PASSWORD" ]);
													bPaymentGateway_TestMode = Sql.ToBoolean(vwPaymentGateways[0]["TEST_MODE"]);
												}
											}
											// 10/19/2010 Paul.  We don't use the simplified Refund method because the refund must go through the same gateway as the original charge. 
											SplendidCharge.CC.Charge(Application, gID, gCURRENCY_ID, gACCOUNT_ID, gCREDIT_CARD_ID, Request.UserHostAddress, sINVOICE_NUMBER, sDESCRIPTION, "Refund", dAMOUNT, sTRANSACTION_NUMBER, sPaymentGateway, sPaymentGateway_Login, sPaymentGateway_Password, bPaymentGateway_TestMode);
										}
										BindGrid();
									}
								}
							}
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

		private void BindGrid()
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					// 05/29/2008 Paul.  Build the list of fields to use in the select clause.
					// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
					m_sVIEW_NAME = "vwPAYMENTS_TRANSACTIONS";
					sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
					     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
					     + "  from " + m_sVIEW_NAME + ControlChars.CrLf
					     + " where 1 = 1"           + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID");
						// 04/26/2008 Paul.  Move Last Sort to the database.
						cmd.CommandText += grdMain.OrderByClause("DATE_ENTERED", "desc");

						if ( bDebug )
							RegisterClientScriptBlock("vwPAYMENTS_TRANSACTIONS", Sql.ClientScriptBlock(cmd));

						try
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									vwMain = dt.DefaultView;
									grdMain.DataSource = vwMain ;
									// 09/05/2005 Paul.  LinkButton controls will not fire an event unless the the grid is bound. 
									// 04/25/2008 Paul.  Enable sorting of sub panel. 
									// 04/26/2008 Paul.  Move Last Sort to the database.
									grdMain.DataBind();
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							ctlDynamicButtons.ErrorText = ex.Message;
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

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			BindGrid();

			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				// 04/28/2008 Paul.  Make use of dynamic buttons. 
				Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".PaymentTransactions", gASSIGNED_USER_ID, gID);
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Payments";
			// 04/26/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_ENTERED");
			arrSelectFields.Add("ID"          );
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			this.AppendGridColumns(grdMain, m_sMODULE + ".PaymentTransactions", arrSelectFields);
			// 04/28/2008 Paul.  Make use of dynamic buttons. 
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".PaymentTransactions", Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
