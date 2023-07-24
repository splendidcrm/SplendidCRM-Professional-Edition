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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.CreditCards
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		// 01/13/2010 Paul.  Add footer buttons. 
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected Guid            gID                   ;
		protected HtmlTable       tblMain               ;
		// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
		//private const string      sEMPTY_PASSWORD = "**********";
		
		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			Guid gACCOUNT_ID = Sql.ToGuid(ViewState["ACCOUNT_ID"]);
			Guid gCONTACT_ID = Sql.ToGuid(ViewState["CONTACT_ID"]);
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					// 11/10/2010 Paul.  Apply Business Rules. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + LayoutEditView);
					if ( Page.IsValid )
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
												ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												ctlFooterButtons .ShowButton("SaveConcurrency", true);
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
							// 01/08/2008 Paul.  If the encryption key does not exist, then we must create it and we must save it back to the database. 
							// 01/08/2008 Paul.  SugarCRM uses blowfish for the inbound email encryption, but we will not since .NET 2.0 does not support blowfish natively. 
							// 02/13/2008 Paul.  Create separate keys for credit card numbers. 
							Guid gCREDIT_CARD_KEY = Sql.ToGuid(Application["CONFIG.CreditCardKey"]);
							if ( Sql.IsEmptyGuid(gCREDIT_CARD_KEY) )
							{
								gCREDIT_CARD_KEY = Guid.NewGuid();
								SqlProcs.spCONFIG_Update("system", "CreditCardKey", gCREDIT_CARD_KEY.ToString());
								Application["CONFIG.CreditCardKey"] = gCREDIT_CARD_KEY;
							}
							Guid gCREDIT_CARD_IV = Sql.ToGuid(Application["CONFIG.CreditCardIV"]);
							if ( Sql.IsEmptyGuid(gCREDIT_CARD_IV) )
							{
								gCREDIT_CARD_IV = Guid.NewGuid();
								SqlProcs.spCONFIG_Update("system", "CreditCardIV", gCREDIT_CARD_IV.ToString());
								Application["CONFIG.CreditCardIV"] = gCREDIT_CARD_IV;
							}
							// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
							string sCARD_TOKEN          = new DynamicControl(this, rowCurrent, "CARD_TOKEN"         ).Text.Trim();
							string sNAME                = new DynamicControl(this, rowCurrent, "NAME"               ).Text.Trim();
							string sSECURITY_CODE       = new DynamicControl(this, rowCurrent, "SECURITY_CODE"      ).Text.Trim();
							string sADDRESS_STREET      = new DynamicControl(this, rowCurrent, "ADDRESS_STREET"     ).Text.Trim();
							string sADDRESS_CITY        = new DynamicControl(this, rowCurrent, "ADDRESS_CITY"       ).Text.Trim();
							string sADDRESS_STATE       = new DynamicControl(this, rowCurrent, "ADDRESS_STATE"      ).Text.Trim();
							string sADDRESS_POSTALCODE  = new DynamicControl(this, rowCurrent, "ADDRESS_POSTALCODE" ).Text.Trim();
							string sADDRESS_COUNTRY     = new DynamicControl(this, rowCurrent, "ADDRESS_COUNTRY"    ).Text.Trim();
							string sCARD_TYPE           = new DynamicControl(this, rowCurrent, "CARD_TYPE"          ).SelectedValue;
							string sCARD_NUMBER         = new DynamicControl(this, rowCurrent, "CARD_NUMBER"        ).Text.Trim();
							string sCARD_NUMBER_DISPLAY = new DynamicControl(this, rowCurrent, "CARD_NUMBER_DISPLAY").Text.Trim();
							// 12/15/2015 Paul.  Add Bank routing. 
							string sBANK_NAME           = new DynamicControl(this, rowCurrent, "BANK_NAME"          ).Text.Trim();
							string sBANK_ROUTING_NUMBER = new DynamicControl(this, rowCurrent, "BANK_ROUTING_NUMBER").Text.Trim();
							// 12/15/2015 Paul.  Add EMAIL and PHONE for Authorize.Net. 
							string sEMAIL               = new DynamicControl(this, rowCurrent, "EMAIL"              ).Text.Trim();
							string sPHONE               = new DynamicControl(this, rowCurrent, "PHONE"              ).Text.Trim();
							if ( Sql.ToBoolean(Application["CONFIG.AuthorizeNet.Enabled"]) || Sql.ToString(Application["CONFIG.PaymentGateway.Library"]) == "nsoftware.InPayWeb" )
							{
								if ( Sql.IsEmptyString(sEMAIL) && Sql.IsEmptyString(sPHONE) )
								{
									throw(new Exception(L10n.Term("CreditCards.ERR_EMAIL_PHONE")));
								}
							}
							// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
							if ( sCARD_NUMBER == Sql.sEMPTY_PASSWORD && rowCurrent != null )
							{
								sCARD_NUMBER = Sql.ToString(rowCurrent["CARD_NUMBER"]);
								if ( !Sql.ToBoolean(rowCurrent["IS_ENCRYPTED"]) )
								{
									string sENCRYPTED_CARD_NUMBER = Security.EncryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									if ( Security.DecryptPassword(sENCRYPTED_CARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV) != sCARD_NUMBER )
										throw(new Exception("Decryption failed"));
									sCARD_NUMBER = sENCRYPTED_CARD_NUMBER;
								}
							}
							else
							{
								// 02/25/2008 Paul.  We need to get the last 4 digits before the card number is encrypted. 
								if ( sCARD_NUMBER.Length > 4 )
									sCARD_NUMBER_DISPLAY = "****" + sCARD_NUMBER.Substring(sCARD_NUMBER.Length - 4, 4);
								else
									sCARD_NUMBER_DISPLAY = "****" + sCARD_NUMBER;
								string sENCRYPTED_CARD_NUMBER = Security.EncryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
								if ( Security.DecryptPassword(sENCRYPTED_CARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV) != sCARD_NUMBER )
									throw(new Exception("Decryption failed"));
								sCARD_NUMBER = sENCRYPTED_CARD_NUMBER;
							}
							
							// 09/13/2013 Paul.  Validate the expiration date outside the datatabase transaction. 
							DateTime dtEXPIRATION_DATE = new DynamicControl(this, rowCurrent, "EXPIRATION_DATE").DateValue;
							// 03/12/2008 Paul.  Some customers may want a simplified mm/yy entry format. 
							if ( FindControl("EXPIRATION_MONTH") != null && FindControl("EXPIRATION_YEAR") != null )
							{
								int nMonth = new DynamicControl(this, "EXPIRATION_MONTH").IntegerValue;
								int nYear  = new DynamicControl(this, "EXPIRATION_YEAR" ).IntegerValue;
								if ( nMonth >= 1 && nMonth <= 12 && nYear >= 2000 && nYear <= 2030 )
								{
									dtEXPIRATION_DATE = new DateTime(nYear, nMonth, 1, 12, 0, 0);
								}
							}
							// 02/25/2008 Paul.  The expiration date should not be localized. 
							// It is actually safer to allow the localization to occur, just normalize the time. 
							dtEXPIRATION_DATE = new DateTime(dtEXPIRATION_DATE.Year, dtEXPIRATION_DATE.Month, dtEXPIRATION_DATE.Day, 12, 0, 0);
							
							// 09/13/2013 Paul.  Add support for PayTrace.  
							if ( Sql.ToBoolean(Application["CONFIG.PayTrace.Enabled"]) && !Sql.IsEmptyString(Application["CONFIG.PayTrace.UserName"]) )
							{

								if ( new DynamicControl(this, rowCurrent, "CARD_NUMBER").Text != Sql.sEMPTY_PASSWORD )
								{
									string sCREDIT_CARD_NUMBER = Security.DecryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									// 08/16/2015 Paul.  Only update the profile if the card number has changed. 
									// 12/15/2015 Paul.  Add EMAIL and PHONE for Authorize.Net. 
									PayTraceUtils.UpdateCustomerProfile(Application, ref gID, sNAME, sCREDIT_CARD_NUMBER, sSECURITY_CODE, dtEXPIRATION_DATE.Month, dtEXPIRATION_DATE.Year, sADDRESS_STREET, sADDRESS_CITY, sADDRESS_STATE, sADDRESS_POSTALCODE, sADDRESS_COUNTRY, sEMAIL, sPHONE);
									// 09/13/2013 Paul.  We store the PayTrace Customer ID in the CARD_NUMBER field.  We use the Credit Card ID as the Customer ID. 
									sCARD_NUMBER = Security.EncryptPassword(gID.ToString(), gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									sCARD_TOKEN  = gID.ToString();
									// 09/24/2013 Paul.  After updating the customer profile with the security code, clear it so that it is not stored in the database.
									sSECURITY_CODE = String.Empty;
								}
							}
							// 12/16/2013 Paul.  Add support for Authorize.Net
							else if ( Sql.ToBoolean(Application["CONFIG.AuthorizeNet.Enabled"]) && !Sql.IsEmptyString(Application["CONFIG.AuthorizeNet.UserName"]) )
							{
								// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
								if ( new DynamicControl(this, rowCurrent, "CARD_NUMBER").Text != Sql.sEMPTY_PASSWORD )
								{
									string sCREDIT_CARD_NUMBER = Security.DecryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									AuthorizeNetUtils.UpdateCustomerProfile(Application, ref sCARD_TOKEN, sNAME, sCREDIT_CARD_NUMBER, sSECURITY_CODE, sBANK_NAME, sBANK_ROUTING_NUMBER, dtEXPIRATION_DATE, sADDRESS_STREET, sADDRESS_CITY, sADDRESS_STATE, sADDRESS_POSTALCODE, sADDRESS_COUNTRY, sEMAIL, sPHONE);
									sCARD_NUMBER = Security.EncryptPassword(sCARD_TOKEN, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									sCARD_TOKEN  = String.Empty;
									sSECURITY_CODE = String.Empty;
								}
							}
							// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
							else if ( !Sql.IsEmptyString(Application["CONFIG.PayPal.ClientID"]) && !Sql.IsEmptyString(Application["CONFIG.PayPal.ClientSecret"]) )
							{
								// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
								if ( new DynamicControl(this, "CARD_NUMBER").Text != Sql.sEMPTY_PASSWORD )
								{
									string sCREDIT_CARD_NUMBER = Security.DecryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									// 12/15/2015 Paul.  Add EMAIL and PHONE for Authorize.Net. 
									PayPalRest.StoreCreditCard(Application, ref sCARD_TOKEN, sNAME, sCARD_TYPE, sCREDIT_CARD_NUMBER, sSECURITY_CODE, dtEXPIRATION_DATE.Month, dtEXPIRATION_DATE.Year, sADDRESS_STREET, sADDRESS_CITY, sADDRESS_STATE, sADDRESS_POSTALCODE, sADDRESS_COUNTRY, sEMAIL, sPHONE);
									sCARD_NUMBER = Security.EncryptPassword(sCARD_TOKEN, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									sSECURITY_CODE = String.Empty;
								}
							}
							// 12/15/2015 Paul.  We should always be saving a card token instead of a card number. 
							else
							{
								// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
								if ( new DynamicControl(this, "CARD_NUMBER").Text != Sql.sEMPTY_PASSWORD )
								{
									string sCREDIT_CARD_NUMBER = Security.DecryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									SplendidCharge.CC.StoreCreditCard(Application, ref sCARD_TOKEN, sNAME, sCARD_TYPE, sCREDIT_CARD_NUMBER, sSECURITY_CODE, sBANK_NAME, sBANK_ROUTING_NUMBER, dtEXPIRATION_DATE.Month, dtEXPIRATION_DATE.Year, sADDRESS_STREET, sADDRESS_CITY, sADDRESS_STATE, sADDRESS_POSTALCODE, sADDRESS_COUNTRY, sEMAIL, sPHONE);
									sCARD_NUMBER = Security.EncryptPassword(sCARD_TOKEN, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
									sSECURITY_CODE = String.Empty;
									sBANK_ROUTING_NUMBER = String.Empty;
								}
							}
							
							// 11/10/2010 Paul.  Apply Business Rules. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 01/30/2008 Paul.  Always save card number as encrypted. 
									// 10/07/2010 Paul.  Add Contact field. 
									// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
									// 12/15/2015 Paul.  Add EMAIL and PHONE for Authorize.Net. 
									SqlProcs.spCREDIT_CARDS_Update
										( ref gID
										, gACCOUNT_ID
										, sNAME
										, sCARD_TYPE
										, sCARD_NUMBER
										, sCARD_NUMBER_DISPLAY
										, sSECURITY_CODE
										, dtEXPIRATION_DATE
										, sBANK_NAME
										, sBANK_ROUTING_NUMBER
										, new DynamicControl(this, rowCurrent, "IS_PRIMARY"         ).Checked
										, true
										, sADDRESS_STREET
										, sADDRESS_CITY
										, sADDRESS_STATE
										, sADDRESS_POSTALCODE
										, sADDRESS_COUNTRY
										, gCONTACT_ID
										, sCARD_TOKEN
										, sEMAIL
										, sPHONE
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									// 08/16/2015 Paul.  There is no compelling reason to have a favorite credit card. 
									//// 08/26/2010 Paul.  Add new record to tracker. 
									//string sNAME = new DynamicControl(this, rowCurrent, "ACCOUNT_NAME").SelectedValue + " " + sCARD_NUMBER_DISPLAY;
									//// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
									//SqlProcs.spTRACKER_Update
									//	( Security.USER_ID
									//	, m_sMODULE
									//	, gID
									//	, sNAME
									//	, "save"
									//	, trn
									//	);
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									ctlDynamicButtons.ErrorText = ex.Message;
									return;
								}
							}
							// 11/10/2010 Paul.  Apply Business Rules. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
						}
						
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						else if ( !Sql.IsEmptyGuid(gCONTACT_ID) )
							Response.Redirect("~/Contacts/view.aspx?ID=" + gCONTACT_ID.ToString());
						else
							Response.Redirect("~/Accounts/view.aspx?ID=" + gACCOUNT_ID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( !Sql.IsEmptyGuid(gCONTACT_ID) )
					Response.Redirect("~/Contacts/view.aspx?ID=" + gCONTACT_ID.ToString());
				if ( !Sql.IsEmptyGuid(gACCOUNT_ID) )
					Response.Redirect("~/Accounts/view.aspx?ID=" + gACCOUNT_ID.ToString());
				else
					Response.Redirect("~/Accounts/default.aspx");
			}
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
				// 03/15/2011 Paul.  Change menu to use main module. 
				//if ( !Sql.IsEmptyGuid(Request["CONTACT_ID"]) )
				//	SetMenu("Contacts");
				//else
				//	SetMenu("Accounts");
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
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
											ctlDynamicButtons.Title = Sql.ToString(rdr["ACCOUNT_NAME"]) + " " + Sql.ToString(rdr["CARD_NUMBER_DISPLAY"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											ViewState["ACCOUNT_ID"] = Sql.ToGuid(rdr["ACCOUNT_ID"]);
											ViewState["CONTACT_ID"] = Sql.ToGuid(rdr["CONTACT_ID"]);
											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
											new DynamicControl(this, "CARD_NUMBER").Text = Sql.sEMPTY_PASSWORD;
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
										}
										else
										{
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlFooterButtons .DisableAll();
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
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

						Guid gACCOUNT_ID = Sql.ToGuid(Request["ACCOUNT_ID"]);
						if ( !Sql.IsEmptyGuid(gACCOUNT_ID) )
						{
							ViewState["ACCOUNT_ID"] = gACCOUNT_ID;
							// 10/07/2010 Paul.  Use Address information from Account. 
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL ;
								sSQL = "select *         " + ControlChars.CrLf
								     + "  from vwACCOUNTS" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, "Accounts", "view");
									Sql.AppendParameter(cmd, gACCOUNT_ID, "ID", false);
									
									if ( bDebug )
										RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
									
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											new DynamicControl(this, "ACCOUNT_ID"         ).ID   = gACCOUNT_ID;
											new DynamicControl(this, "ACCOUNT_NAME"       ).Text = Sql.ToString(rdr["NAME"                      ]);
											new DynamicControl(this, "ADDRESS_STREET"     ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STREET"    ]);
											new DynamicControl(this, "ADDRESS_CITY"       ).Text = Sql.ToString(rdr["BILLING_ADDRESS_CITY"      ]);
											new DynamicControl(this, "ADDRESS_STATE"      ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STATE"     ]);
											new DynamicControl(this, "ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["BILLING_ADDRESS_POSTALCODE"]);
											new DynamicControl(this, "ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["BILLING_ADDRESS_COUNTRY"   ]);
										}
									}
								}
							}
						}
						// 10/07/2010 Paul.  Add Contact field. 
						Guid gCONTACT_ID = Sql.ToGuid(Request["CONTACT_ID"]);
						if ( !Sql.IsEmptyGuid(gCONTACT_ID) )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL ;
								sSQL = "select *         " + ControlChars.CrLf
								     + "  from vwCONTACTS" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, "Contacts", "view");
									Sql.AppendParameter(cmd, gCONTACT_ID, "ID", false);
									
									if ( bDebug )
										RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
									
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											gACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
											new DynamicControl(this, "CONTACT_ID"         ).ID   = gCONTACT_ID;
											new DynamicControl(this, "CONTACT_NAME"       ).Text = Sql.ToString(rdr["NAME"                      ]);
											new DynamicControl(this, "NAME"               ).Text = Sql.ToString(rdr["NAME"                      ]);
											new DynamicControl(this, "ACCOUNT_ID"         ).ID   = gACCOUNT_ID;
											new DynamicControl(this, "ACCOUNT_NAME"       ).Text = Sql.ToString(rdr["ACCOUNT_NAME"              ]);
											new DynamicControl(this, "ADDRESS_STREET"     ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_STREET"    ]);
											new DynamicControl(this, "ADDRESS_CITY"       ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_CITY"      ]);
											new DynamicControl(this, "ADDRESS_STATE"      ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_STATE"     ]);
											new DynamicControl(this, "ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_POSTALCODE"]);
											new DynamicControl(this, "ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_COUNTRY"   ]);
											ViewState["ACCOUNT_ID"] = gACCOUNT_ID;
											ViewState["CONTACT_ID"] = gCONTACT_ID;
										}
									}
								}
							}
						}
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "CreditCards";
			// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
			m_sVIEW_NAME = "vw" + Crm.Modules.TableName(m_sMODULE) + "_Edit";
			// 12/09/2010 Paul.  The missing SetMenu() call is causing a "Failed to load viewstate" exception when a button is clicked. 
			// 03/15/2011 Paul.  Change menu to use main module. 
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				// 11/10/2010 Paul.  Make sure to add the RulesValidator early in the pipeline. 
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}
