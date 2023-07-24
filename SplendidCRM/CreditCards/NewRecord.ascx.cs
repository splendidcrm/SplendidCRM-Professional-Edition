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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.CreditCards
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : NewRecordControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;
		protected _controls.HeaderLeft     ctlHeaderLeft    ;

		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;
		protected Label           lblError                        ;
		protected Panel           pnlMain                         ;
		protected Panel           pnlEdit                         ;
		// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
		//private const string      sEMPTY_PASSWORD = "**********";

		public Guid ACCOUNT_ID
		{
			get
			{
				// 02/21/2010 Paul.  An EditView Inline will use the ViewState, and a NewRecord Inline will use the Request. 
				Guid gACCOUNT_ID = Sql.ToGuid(ViewState["ACCOUNT_ID"]);
				if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
					gACCOUNT_ID = Sql.ToGuid(Request["ACCOUNT_ID"]);
				return gACCOUNT_ID;
			}
			set
			{
				ViewState["ACCOUNT_ID"] = value;
			}
		}

		// 10/07/2010 Paul.  Add Contact field. 
		public Guid CONTACT_ID
		{
			get
			{
				Guid gCONTACT_ID = Sql.ToGuid(ViewState["CONTACT_ID"]);
				if ( Sql.IsEmptyGuid(gCONTACT_ID) )
					gCONTACT_ID = Sql.ToGuid(Request["CONTACT_ID"]);
				return gCONTACT_ID;
			}
			set
			{
				ViewState["CONTACT_ID"] = value;
			}
		}

		public override bool IsEmpty()
		{
			string sNAME = new DynamicControl(this, "NAME").Text;
			return Sql.IsEmptyString(sNAME);
		}

		public override void ValidateEditViewFields()
		{
			if ( !IsEmpty() )
			{
				this.ValidateEditViewFields(m_sMODULE + "." + sEditView);
				// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
				this.ApplyEditViewValidationEventRules(m_sMODULE + "." + sEditView);
			}
		}

		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			if ( IsEmpty() )
				return;
			
			string    sTABLE_NAME    = Crm.Modules.TableName(m_sMODULE);
			DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
			
			Guid gACCOUNT_ID = new DynamicControl(this, "ACCOUNT_ID").ID;
			if ( Sql.IsEmptyGuid(gACCOUNT_ID) )
				gACCOUNT_ID = this.ACCOUNT_ID;
			if ( sPARENT_TYPE == "Accounts" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gACCOUNT_ID = gPARENT_ID;
			// 10/07/2010 Paul.  Add Contact field. 
			Guid gCONTACT_ID = new DynamicControl(this, "CONTACT_ID").ID;
			if ( Sql.IsEmptyGuid(gCONTACT_ID) )
				gCONTACT_ID = this.CONTACT_ID;
			if ( sPARENT_TYPE == "Contacts" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gCONTACT_ID = gPARENT_ID;
			
			Guid gCREDIT_CARD_KEY = Sql.ToGuid(Application["CONFIG.CreditCardKey"]);
			Guid gCREDIT_CARD_IV  = Sql.ToGuid(Application["CONFIG.CreditCardIV" ]);
			// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
			string sCARD_TOKEN          = String.Empty;
			string sNAME                = new DynamicControl(this, "NAME"               ).Text.Trim();
			string sSECURITY_CODE       = new DynamicControl(this, "SECURITY_CODE"      ).Text.Trim();
			string sADDRESS_STREET      = new DynamicControl(this, "ADDRESS_STREET"     ).Text.Trim();
			string sADDRESS_CITY        = new DynamicControl(this, "ADDRESS_CITY"       ).Text.Trim();
			string sADDRESS_STATE       = new DynamicControl(this, "ADDRESS_STATE"      ).Text.Trim();
			string sADDRESS_POSTALCODE  = new DynamicControl(this, "ADDRESS_POSTALCODE" ).Text.Trim();
			string sADDRESS_COUNTRY     = new DynamicControl(this, "ADDRESS_COUNTRY"    ).Text.Trim();
			string sCARD_TYPE           = new DynamicControl(this, "CARD_TYPE"          ).SelectedValue;
			string sCARD_NUMBER         = new DynamicControl(this, "CARD_NUMBER"        ).Text.Trim();
			string sCARD_NUMBER_DISPLAY = new DynamicControl(this, "CARD_NUMBER_DISPLAY").Text.Trim();
			// 12/15/2015 Paul.  Add Bank routing. 
			string sBANK_NAME           = new DynamicControl(this, "BANK_NAME"          ).Text.Trim();
			string sBANK_ROUTING_NUMBER = new DynamicControl(this, "BANK_ROUTING_NUMBER").Text.Trim();
			// 12/15/2015 Paul.  Add EMAIL and PHONE for Authorize.Net. 
			string sEMAIL               = new DynamicControl(this, "EMAIL"              ).Text.Trim();
			string sPHONE               = new DynamicControl(this, "PHONE"              ).Text.Trim();
			
			// 02/25/2008 Paul.  We need to get the last 4 digits before the card number is encrypted. 
			if ( sCARD_NUMBER.Length > 4 )
				sCARD_NUMBER_DISPLAY = "****" + sCARD_NUMBER.Substring(sCARD_NUMBER.Length - 4, 4);
			else
				sCARD_NUMBER_DISPLAY = "****" + sCARD_NUMBER;
			string sENCRYPTED_CARD_NUMBER = Security.EncryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
			if ( Security.DecryptPassword(sENCRYPTED_CARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV) != sCARD_NUMBER )
				throw(new Exception("Decryption failed"));
			sCARD_NUMBER = sENCRYPTED_CARD_NUMBER;
			
			DateTime dtEXPIRATION_DATE = new DynamicControl(this, "EXPIRATION_DATE").DateValue;
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
				// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
				if ( new DynamicControl(this, "CARD_NUMBER").Text != Sql.sEMPTY_PASSWORD )
				{
					// 08/16/2015 Paul.  Only update the profile if the card number has changed. 
					string sCREDIT_CARD_NUMBER = Security.DecryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
					// 12/15/2015 Paul.  Add EMAIL and PHONE for Authorize.Net. 
					PayTraceUtils.UpdateCustomerProfile(Application, ref gID, sNAME, sCREDIT_CARD_NUMBER, sSECURITY_CODE, dtEXPIRATION_DATE.Month, dtEXPIRATION_DATE.Year, sADDRESS_STREET, sADDRESS_CITY, sADDRESS_STATE, sADDRESS_POSTALCODE, sADDRESS_COUNTRY, sEMAIL, sPHONE);
					// 09/13/2013 Paul.  We store the PayTrace Customer ID in the CARD_NUMBER field.  We use the Credit Card ID as the Customer ID. 
					sCARD_NUMBER = Security.EncryptPassword(gID.ToString(), gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
					sCARD_TOKEN  = gID.ToString();
					// 12/16/2015 Paul.  After updating the customer profile with the security code, clear it so that it is not stored in the database.
					sSECURITY_CODE = String.Empty;
				}
			}
			// 12/16/2013 Paul.  Add support for Authorize.Net
			else if ( Sql.ToBoolean(Application["CONFIG.AuthorizeNet.Enabled"]) && !Sql.IsEmptyString(Application["CONFIG.AuthorizeNet.UserName"]) )
			{
				// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
				if ( new DynamicControl(this, "CARD_NUMBER").Text != Sql.sEMPTY_PASSWORD )
				{
					string sCREDIT_CARD_NUMBER = Security.DecryptPassword(sCARD_NUMBER, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
					AuthorizeNetUtils.UpdateCustomerProfile(Application, ref sCARD_TOKEN, sNAME, sCREDIT_CARD_NUMBER, sSECURITY_CODE, sBANK_NAME, sBANK_ROUTING_NUMBER, dtEXPIRATION_DATE, sADDRESS_STREET, sADDRESS_CITY, sADDRESS_STATE, sADDRESS_POSTALCODE, sADDRESS_COUNTRY, sEMAIL, sPHONE);
					sCARD_NUMBER   = Security.EncryptPassword(sCARD_TOKEN, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
					sCARD_TOKEN    = String.Empty;
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
					sCARD_NUMBER   = Security.EncryptPassword(sCARD_TOKEN, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
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
					sCARD_TOKEN    = String.Empty;
					sSECURITY_CODE = String.Empty;
				}
			}
			
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
				, new DynamicControl(this, "IS_PRIMARY"         ).Checked
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
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "NewRecord" )
				{
					this.ValidateEditViewFields(m_sMODULE + "." + sEditView);
					// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + sEditView);
					if ( Page.IsValid )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + sEditView, null);
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 10/21/2010 Paul.  Make sure to pass the parent values. 
									Guid   gPARENT_ID   = this.PARENT_ID;
									string sPARENT_TYPE = this.PARENT_TYPE;
									Save(gPARENT_ID, sPARENT_TYPE, trn);
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									if ( bShowFullForm || bShowCancel )
										ctlFooterButtons.ErrorText = ex.Message;
									else
										lblError.Text = ex.Message;
									return;
								}
							}
							// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							DataRow rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + sEditView, rowCurrent);
						}
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						// 02/21/2010 Paul.  An error should not forward the command so that the error remains. 
						// In case of success, send the command so that the page can be rebuilt. 
						// 06/02/2010 Paul.  We need a way to pass the ID up the command chain. 
						else if ( Command != null )
							Command(sender, new CommandEventArgs(e.CommandName, gID.ToString()));
						else if ( !Sql.IsEmptyGuid(gID) )
							Response.Redirect("~/" + m_sMODULE + "/view.aspx?ID=" + gID.ToString());
					}
				}
				else if ( Command != null )
				{
					Command(sender, e);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				if ( bShowFullForm || bShowCancel )
					ctlFooterButtons.ErrorText = ex.Message;
				else
					lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/04/2006 Paul.  NewRecord should not be displayed if the user does not have edit rights. 
			// 01/02/2020 Paul.  Allow the NewRecord to be disabled per module using config table. 
			this.Visible = (!Sql.ToBoolean(Application["CONFIG." + m_sMODULE + ".DisableNewRecord"]) || sEditView != "NewRecord") && (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
				bool bIsPostBack = this.IsPostBack && !NotPostBack;
				if ( !bIsPostBack )
				{
					// 05/06/2010 Paul.  When the control is created out-of-band, we need to manually bind the controls. 
					if ( NotPostBack )
						this.DataBind();
					this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null);
					// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
					if ( EditViewLoad != null )
						EditViewLoad(this, null);
					// 06/03/2010 Paul.  Prefill the Opportunity and the Account. 
					// 06/03/2010 Paul.  Allow a CreditCard to be created from an Account. 
					Guid gACCOUNT_ID = ACCOUNT_ID;
					if ( !Sql.IsEmptyGuid(gACCOUNT_ID) )
					{
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
					Guid gCONTACT_ID = CONTACT_ID;
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
										new DynamicControl(this, "CONTACT_ID"         ).ID   = gCONTACT_ID;
										new DynamicControl(this, "CONTACT_NAME"       ).Text = Sql.ToString(rdr["NAME"                      ]);
										new DynamicControl(this, "NAME"               ).Text = Sql.ToString(rdr["NAME"                      ]);
										new DynamicControl(this, "ACCOUNT_ID"         ).ID   = Sql.ToGuid  (rdr["ACCOUNT_ID"                ]);
										new DynamicControl(this, "ACCOUNT_NAME"       ).Text = Sql.ToString(rdr["ACCOUNT_NAME"              ]);
										new DynamicControl(this, "ADDRESS_STREET"     ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_STREET"    ]);
										new DynamicControl(this, "ADDRESS_CITY"       ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_CITY"      ]);
										new DynamicControl(this, "ADDRESS_STATE"      ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_STATE"     ]);
										new DynamicControl(this, "ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_POSTALCODE"]);
										new DynamicControl(this, "ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_COUNTRY"   ]);
										ACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
									}
								}
							}
						}
					}
					// 02/21/2010 Paul.  When the Full Form buttons are used, we don't want the panel to have margins. 
					if ( bShowFullForm || bShowCancel || sEditView != "NewRecord" )
					{
						pnlMain.CssClass = "";
						pnlEdit.CssClass = "tabForm";
					}
					// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
					this.ApplyEditViewNewEventRules(m_sMODULE + "." + sEditView);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				if ( bShowFullForm || bShowCancel )
					ctlFooterButtons.ErrorText = ex.Message;
				else
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);

			ctlDynamicButtons.AppendButtons("NewRecord." + (bShowFullForm ? "FullForm" : (bShowCancel ? "WithCancel" : "SaveOnly")), Guid.Empty, Guid.Empty);
			ctlFooterButtons .AppendButtons("NewRecord." + (bShowFullForm ? "FullForm" : (bShowCancel ? "WithCancel" : "SaveOnly")), Guid.Empty, Guid.Empty);
			m_sMODULE = "CreditCards";
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( bIsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null, ctlFooterButtons.ButtonClientID("NewRecord"));
				// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
				if ( EditViewLoad != null )
					EditViewLoad(this, null);
				// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}
