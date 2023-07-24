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
using System.Diagnostics;
using AjaxControlToolkit;

namespace SplendidCRM.Invoices
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
		protected _controls.EditLineItemsView ctlEditLineItemsView;

		protected Guid            gID                   ;
		protected HtmlTable       tblMain               ;
		// 09/02/2012 Paul.  EditViews were combined into a single view. 
		//protected HtmlTable       tblAddress            ;
		protected HtmlTable       tblSummary            ;
		protected HtmlTable       tblDescription        ;
		protected PlaceHolder     plcSubPanel           ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 06/08/2006 Paul.  Redirect to parent if that is where the note was originated. 
			Guid   gPARENT_ID   = Sql.ToGuid(Request["PARENT_ID"]);
			string sMODULE      = String.Empty;
			string sPARENT_TYPE = String.Empty;
			string sPARENT_NAME = String.Empty;
			try
			{
				SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// The only possible error is a connection failure, so just ignore all errors. 
				gPARENT_ID = Guid.Empty;
			}
			// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveDuplicate" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					this.ValidateEditViewFields(m_sMODULE + ".EditAddress"    );
					this.ValidateEditViewFields(m_sMODULE + ".EditDescription");
					// 11/10/2010 Paul.  Apply Business Rules. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + LayoutEditView);
					this.ApplyEditViewValidationEventRules(m_sMODULE + ".EditAddress"    );
					this.ApplyEditViewValidationEventRules(m_sMODULE + ".EditDescription");
					
					// 04/19/2010 Paul.  We now need to validate the sub panels as they can contain an inline NewRecord control. 
					if ( plcSubPanel.Visible )
					{
						foreach ( Control ctl in plcSubPanel.Controls )
						{
							InlineEditControl ctlSubPanel = ctl as InlineEditControl;
							if ( ctlSubPanel != null )
							{
								ctlSubPanel.ValidateEditViewFields();
							}
						}
					}
					if ( Page.IsValid )
					{
						// 09/09/2009 Paul.  Use the new function to get the table name. 
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						DataTable dtCustomFields    = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
						// 05/25/2008 Paul.  Allow the line items table to contain custom fields. 
						DataTable dtCustomLineItems = SplendidCache.FieldsMetaData_UnvalidatedCustomFields(sTABLE_NAME + "_LINE_ITEMS");
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

							// 11/10/2010 Paul.  Apply Business Rules. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + LayoutEditView, rowCurrent);
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + ".EditAddress"    , rowCurrent);
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + ".EditDescription", rowCurrent);
							// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
							// Apply duplicate checking after PreSave business rules, but before trasnaction. 
							bool bDUPLICATE_CHECHING_ENABLED = Sql.ToBoolean(Application["CONFIG.enable_duplicate_check"]) && Sql.ToBoolean(Application["Modules." + m_sMODULE + ".DuplicateCheckingEnabled"]) && (e.CommandName != "SaveDuplicate");
							if ( bDUPLICATE_CHECHING_ENABLED )
							{
								if ( Utils.DuplicateCheck(Application, con, m_sMODULE, gID, this, rowCurrent) > 0 )
								{
									ctlDynamicButtons.ShowButton("SaveDuplicate", true);
									ctlFooterButtons .ShowButton("SaveDuplicate", true);
									throw(new Exception(L10n.Term(".ERR_DUPLICATE_EXCEPTION")));
								}
							}
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 01/05/2008 Paul.  Update the totals before saving. 
									ctlEditLineItemsView.UpdateTotals();
									// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
									// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
									// 08/06/2009 Paul.  INVOICE_NUM now uses our number sequence table. 
									// 02/27/2015 Paul.  Add SHIP_DATE to sync with QuickBooks. 
									// 11/29/2015 Paul.  If the name field is empty, generate from Account and first line item. 
									DataTable dtLineItems = ctlEditLineItemsView.LineItems;
									string sNAME = new DynamicControl(this, rowCurrent, "NAME").Text;
									if ( Sql.IsEmptyString(sNAME) )
									{
										string sLINE_ITEM_NAME = String.Empty;
										foreach ( DataRow row in dtLineItems.Rows )
										{
											if ( row.RowState != DataRowState.Deleted )
											{
												sLINE_ITEM_NAME = Sql.ToString (row["NAME"]);
												if ( Sql.IsEmptyString(sLINE_ITEM_NAME) )
													sLINE_ITEM_NAME = Sql.ToString (row["MFT_PART_NUM"]);
												break;
											}
										}
										// 04/25/2016 Paul.  Use template approach to naming. 
										sNAME = Sql.ToString(Application["CONFIG.Invoices.NameTemplate"]);
										if ( Sql.IsEmptyString(sNAME) )
										{
											if ( Sql.ToString(Application["CONFIG.BusinessMode"]) == "B2C" )
												sNAME = "(BILLING_CONTACT_NAME} - {LINE_ITEM_NAME}";
											else
												sNAME = "{BILLING_ACCOUNT_NAME} - {LINE_ITEM_NAME}";
										}
										System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("{([A-Za-z0-9_]+)}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
										foreach ( System.Text.RegularExpressions.Match match in regex.Matches(sNAME) )
										{
											if ( String.Compare(match.Groups[1].Value, "DATE_ENTERED", true) == 0 || String.Compare(match.Groups[1].Value, "DATE_MODIFIED", true) == 0 )
												sNAME = sNAME.Replace(match.Groups[0].Value, DateTime.Now.ToString("d"));
											else if ( String.Compare(match.Groups[1].Value, "LINE_ITEM_NAME", true) == 0 )
												sNAME = sNAME.Replace(match.Groups[0].Value, sLINE_ITEM_NAME);
											else
												sNAME = sNAME.Replace(match.Groups[0].Value, new DynamicControl(this, rowCurrent, match.Groups[1].Value).Text);
										}
									}
									SqlProcs.spINVOICES_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID"                ).ID
										, sNAME
										, new DynamicControl(this, rowCurrent, "QUOTE_ID"                        ).ID
										, new DynamicControl(this, rowCurrent, "ORDER_ID"                        ).ID
										, new DynamicControl(this, rowCurrent, "OPPORTUNITY_ID"                  ).ID
										, new DynamicControl(this, rowCurrent, "PAYMENT_TERMS"                   ).SelectedValue
										, new DynamicControl(this, rowCurrent, "INVOICE_STAGE"                   ).SelectedValue
										, new DynamicControl(this, rowCurrent, "PURCHASE_ORDER_NUM"              ).Text
										, new DynamicControl(this, rowCurrent, "DUE_DATE"                        ).DateValue
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "EXCHANGE_RATE"   ).FloatValue
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "CURRENCY_ID"     ).ID
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "TAXRATE_ID"      ).ID
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "SHIPPER_ID"      ).ID
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "SUBTOTAL"        ).DecimalValue
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "DISCOUNT"        ).DecimalValue
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "SHIPPING"        ).DecimalValue
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "TAX"             ).DecimalValue
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "TOTAL"           ).DecimalValue
										, new DynamicControl(ctlEditLineItemsView, rowCurrent, "AMOUNT_DUE"      ).DecimalValue
										, new DynamicControl(this, rowCurrent, "BILLING_ACCOUNT_ID"              ).ID
										, new DynamicControl(this, rowCurrent, "BILLING_CONTACT_ID"              ).ID
										, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_STREET"          ).Text
										, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_CITY"            ).Text
										, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_STATE"           ).Text
										, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_POSTALCODE"      ).Text
										, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_COUNTRY"         ).Text
										, new DynamicControl(this, rowCurrent, "SHIPPING_ACCOUNT_ID"             ).ID
										, new DynamicControl(this, rowCurrent, "SHIPPING_CONTACT_ID"             ).ID
										, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_STREET"         ).Text
										, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_CITY"           ).Text
										, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_STATE"          ).Text
										, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_POSTALCODE"     ).Text
										, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_COUNTRY"        ).Text
										, new DynamicControl(this, rowCurrent, "DESCRIPTION"                     ).Text
										, new DynamicControl(this, rowCurrent, "INVOICE_NUM"                     ).Text
										, new DynamicControl(this, rowCurrent, "TEAM_ID"                         ).ID
										, new DynamicControl(this, rowCurrent, "TEAM_SET_LIST"                   ).Text
										, new DynamicControl(this, rowCurrent, "SHIP_DATE"                       ).DateValue
										// 05/12/2016 Paul.  Add Tags module. 
										, new DynamicControl(this, rowCurrent, "TAG_SET_NAME"                    ).Text
										// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
										, new DynamicControl(this, rowCurrent, "ASSIGNED_SET_LIST"               ).Text
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									// 08/26/2010 Paul.  Add new record to tracker. 
									// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
									SqlProcs.spTRACKER_Update
										( Security.USER_ID
										, m_sMODULE
										, gID
										, new DynamicControl(this, rowCurrent, "NAME").Text
										, "save"
										, trn
										);
									
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
													SqlProcs.spINVOICES_LINE_ITEMS_Delete(gITEM_ID, trn);
											}
										}
									}
									// 12/28/2007 Paul.  Renumber the position. 
									int nPOSITION = 1;
									foreach ( DataRow row in dtLineItems.Rows )
									{
										if ( row.RowState != DataRowState.Deleted )
										{
											Guid    gITEM_ID             = Sql.ToGuid   (row["ID"                 ]);
											Guid    gLINE_GROUP_ID       = Sql.ToGuid   (row["LINE_GROUP_ID"      ]);
											string  sLINE_ITEM_TYPE      = Sql.ToString (row["LINE_ITEM_TYPE"     ]);
											//int     nPOSITION            = Sql.ToInteger(row["POSITION"           ]);
											string  sLINE_ITEM_NAME      = Sql.ToString (row["NAME"               ]);
											string  sMFT_PART_NUM        = Sql.ToString (row["MFT_PART_NUM"       ]);
											string  sVENDOR_PART_NUM     = Sql.ToString (row["VENDOR_PART_NUM"    ]);
											Guid    gPRODUCT_TEMPLATE_ID = Sql.ToGuid   (row["PRODUCT_TEMPLATE_ID"]);
											// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
											Guid    gPARENT_TEMPLATE_ID  = Sql.ToGuid   (row["PARENT_TEMPLATE_ID" ]);
											// 07/15/2010 Paul.  Add GROUP_ID for options management. 
											// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
											//Guid    gGROUP_ID            = Sql.ToGuid   (row["GROUP_ID"           ]);
											string  sTAX_CLASS           = Sql.ToString (row["TAX_CLASS"          ]);
											// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
											float   nQUANTITY            = Sql.ToFloat  (row["QUANTITY"           ]);
											Decimal dCOST_PRICE          = Sql.ToDecimal(row["COST_PRICE"         ]);
											Decimal dLIST_PRICE          = Sql.ToDecimal(row["LIST_PRICE"         ]);
											Decimal dUNIT_PRICE          = Sql.ToDecimal(row["UNIT_PRICE"         ]);
											string  sDESCRIPTION         = Sql.ToString (row["DESCRIPTION"        ]);
											// 08/13/2010 Paul.  New discount fields. 
											Guid    gDISCOUNT_ID         = Sql.ToGuid   (row["DISCOUNT_ID"        ]);
											Decimal dDISCOUNT_PRICE      = Sql.ToDecimal(row["DISCOUNT_PRICE"     ]);
											// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
											string  sPRICING_FORMULA     = Sql.ToString (row["PRICING_FORMULA"    ]);
											float   fPRICING_FACTOR      = Sql.ToFloat  (row["PRICING_FACTOR"     ]);
											// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
											Guid    gTAXRATE_ID          = Sql.ToGuid   (row["TAXRATE_ID"         ]);

											// 03/27/2007 Paul.  Only add if product is defined.  This will exclude the blank row. 
											// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
											// 08/16/2010 Paul.  Add support for comments. 
											// 11/18/2010 Paul.  Reverse the name of this function. 
											if ( _controls.EditLineItemsView.IsLineItemNotEmpty(row) )
											{
												// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
												// 07/15/2010 Paul.  Add GROUP_ID for options management. 
												// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
												// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
												// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
												SqlProcs.spINVOICES_LINE_ITEMS_Update
													( ref gITEM_ID        
													, gID                 
													, gLINE_GROUP_ID      
													, sLINE_ITEM_TYPE     
													, nPOSITION           
													, sLINE_ITEM_NAME     
													, sMFT_PART_NUM       
													, sVENDOR_PART_NUM    
													, gPRODUCT_TEMPLATE_ID
													, sTAX_CLASS          
													, nQUANTITY           
													, dCOST_PRICE         
													, dLIST_PRICE         
													, dUNIT_PRICE         
													, sDESCRIPTION        
													, gPARENT_TEMPLATE_ID 
													, gDISCOUNT_ID        
													, dDISCOUNT_PRICE     
													, sPRICING_FORMULA    
													, fPRICING_FACTOR     
 													, gTAXRATE_ID         
													, trn
													);
												// 05/25/2008 Paul.  Line item custom fields need to be updated. 
												SplendidDynamic.UpdateCustomFields(row, trn, gITEM_ID, sTABLE_NAME + "_LINE_ITEMS", dtCustomLineItems);
												nPOSITION++;
											}
										}
									}
									// 04/23/2008 Paul.  Update the Amount Due. 
									SqlProcs.spINVOICES_UpdateAmountDue(gID, trn);
									if ( plcSubPanel.Visible )
									{
										// 01/27/2010 Paul.  The SubPanel can now have state that needs to be saved. 
										foreach ( Control ctl in plcSubPanel.Controls )
										{
											InlineEditControl ctlSubPanel = ctl as InlineEditControl;
											if ( ctlSubPanel != null )
											{
												ctlSubPanel.Save(gID, m_sMODULE, trn);
											}
										}
									}
									trn.Commit();
									// 04/03/2012 Paul.  Just in case the name changes, clear the favorites. 
									SplendidCache.ClearFavorites();
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
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + ".EditAddress"    , rowCurrent);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + ".EditDescription", rowCurrent);
						}
						
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						else
						// 08/26/2010 Paul.  No longer redirect to parent so that it will be very easy to click the PDF button after saving. 
						//if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						//	Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
						//else
							Response.Redirect("view.aspx?ID=" + gID.ToString());
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
				// 11/12/2011 Paul.  Hidden fields for ORDER_ID and QUOTE_ID were removed, so make clear that they are not used. 
				Guid gORDER_ID = Sql.ToGuid(Request["ORDER_ID"]);
				Guid gQUOTE_ID = Sql.ToGuid(Request["QUOTE_ID"]);
				if ( !Sql.IsEmptyGuid(gPARENT_ID) )
					Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
				else if ( !Sql.IsEmptyGuid(gORDER_ID) )
					Response.Redirect("~/Orders/view.aspx?ID=" + gORDER_ID.ToString());
				else if ( !Sql.IsEmptyGuid(gQUOTE_ID) )
					Response.Redirect("~/Quotes/view.aspx?ID=" + gQUOTE_ID.ToString());
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void UpdateAccount(Guid gACCOUNT_ID, bool bUpdateBilling, bool bUpdateShipping)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				sSQL = "select *         " + ControlChars.CrLf
				     + "  from vwACCOUNTS" + ControlChars.CrLf
				     + " where ID = @ID  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gACCOUNT_ID);
					con.Open();
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							if ( bUpdateBilling )
							{
								new DynamicControl(this, "BILLING_ACCOUNT_ID"         ).ID   = Sql.ToGuid  (rdr["ID"                         ]);
								new DynamicControl(this, "BILLING_ACCOUNT_NAME"       ).Text = Sql.ToString(rdr["NAME"                       ]);
								new DynamicControl(this, "BILLING_ADDRESS_STREET"     ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STREET"     ]);
								new DynamicControl(this, "BILLING_ADDRESS_CITY"       ).Text = Sql.ToString(rdr["BILLING_ADDRESS_CITY"       ]);
								new DynamicControl(this, "BILLING_ADDRESS_STATE"      ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STATE"      ]);
								new DynamicControl(this, "BILLING_ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["BILLING_ADDRESS_POSTALCODE" ]);
								new DynamicControl(this, "BILLING_ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["BILLING_ADDRESS_COUNTRY"    ]);
								// 07/27/2010 Paul.  We need to use the ContextKey feature of AutoComplete to pass the Account Name to the Contact function. 
								AjaxControlToolkit.AutoCompleteExtender autoCONTACT_NAME = FindControl("autoBILLING_CONTACT_NAME") as AjaxControlToolkit.AutoCompleteExtender;
								if ( autoCONTACT_NAME != null )
								{
									if ( autoCONTACT_NAME.UseContextKey )
										autoCONTACT_NAME.ContextKey = Sql.ToString(rdr["NAME"]);
								}
							}
							if ( bUpdateShipping )
							{
								new DynamicControl(this, "SHIPPING_ACCOUNT_ID"        ).ID   = Sql.ToGuid  (rdr["ID"                         ]);
								new DynamicControl(this, "SHIPPING_ACCOUNT_NAME"      ).Text = Sql.ToString(rdr["NAME"                       ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_STREET"    ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_STREET"    ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_CITY"      ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_CITY"      ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_STATE"     ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_STATE"     ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_POSTALCODE").Text = Sql.ToString(rdr["SHIPPING_ADDRESS_POSTALCODE"]);
								new DynamicControl(this, "SHIPPING_ADDRESS_COUNTRY"   ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_COUNTRY"   ]);
								// 07/27/2010 Paul.  We need to use the ContextKey feature of AutoComplete to pass the Account Name to the Contact function. 
								AjaxControlToolkit.AutoCompleteExtender autoCONTACT_NAME = FindControl("autoSHIPPING_CONTACT_NAME") as AjaxControlToolkit.AutoCompleteExtender;
								if ( autoCONTACT_NAME != null )
								{
									if ( autoCONTACT_NAME.UseContextKey )
										autoCONTACT_NAME.ContextKey = Sql.ToString(rdr["NAME"]);
								}
							}
						}
						else
						{
							if ( bUpdateBilling )
							{
								// 07/27/2010 Paul.  We need to clear the UseContextKey if the account was not found. 
								AjaxControlToolkit.AutoCompleteExtender autoCONTACT_NAME = FindControl("autoBILLING_CONTACT_NAME") as AjaxControlToolkit.AutoCompleteExtender;
								if ( autoCONTACT_NAME != null )
								{
									if ( autoCONTACT_NAME.UseContextKey )
										autoCONTACT_NAME.ContextKey = String.Empty;
								}
							}
							if ( bUpdateShipping )
							{
								// 07/27/2010 Paul.  We need to clear the UseContextKey if the account was not found. 
								AjaxControlToolkit.AutoCompleteExtender autoCONTACT_NAME = FindControl("autoSHIPPING_CONTACT_NAME") as AjaxControlToolkit.AutoCompleteExtender;
								if ( autoCONTACT_NAME != null )
								{
									if ( autoCONTACT_NAME.UseContextKey )
										autoCONTACT_NAME.ContextKey = String.Empty;
								}
							}
						}
					}
				}
			}
		}

		private void UpdateContact(Guid gCONTACT_ID, bool bUpdateBilling, bool bUpdateShipping)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				sSQL = "select *         " + ControlChars.CrLf
				     + "  from vwCONTACTS" + ControlChars.CrLf
				     + " where ID = @ID  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gCONTACT_ID);
					con.Open();
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							// 05/28/2007 Paul.  The ACCOUNT_ID needs to be retrieved from contact record. 
							Guid gACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
							if ( bUpdateBilling )
							{
								new DynamicControl(this, "BILLING_CONTACT_ID"   ).ID   = Sql.ToGuid  (rdr["ID"  ]);
								new DynamicControl(this, "BILLING_CONTACT_NAME" ).Text = Sql.ToString(rdr["NAME"]);
								// 08/21/2010 Paul.  Update address if the contact changes. 
								DynamicControl ctlBILLING_ACCOUNT_ID  = new DynamicControl(this, "BILLING_ACCOUNT_ID" );
								// 08/22/2010 Paul.  Only update account if empty. 
								if ( !Sql.IsEmptyGuid(gACCOUNT_ID) && Sql.IsEmptyGuid(ctlBILLING_ACCOUNT_ID.ID) )
								{
									//new DynamicControl(this, "BILLING_ACCOUNT_ID"         ).ID   = Sql.ToGuid  (rdr["ACCOUNT_ID"                 ]);
									//new DynamicControl(this, "BILLING_ACCOUNT_NAME"       ).Text = Sql.ToString(rdr["ACCOUNT_NAME"               ]);
									UpdateAccount(gACCOUNT_ID, true, false);
								}
								if ( !Sql.IsEmptyString(rdr["PRIMARY_ADDRESS_STREET"]) )
								{
									new DynamicControl(this, "BILLING_ADDRESS_STREET"     ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_STREET"     ]);
									new DynamicControl(this, "BILLING_ADDRESS_CITY"       ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_CITY"       ]);
									new DynamicControl(this, "BILLING_ADDRESS_STATE"      ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_STATE"      ]);
									new DynamicControl(this, "BILLING_ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_POSTALCODE" ]);
									new DynamicControl(this, "BILLING_ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["PRIMARY_ADDRESS_COUNTRY"    ]);
								}
							}
							if ( bUpdateShipping )
							{
								new DynamicControl(this, "SHIPPING_CONTACT_ID"  ).ID   = Sql.ToGuid  (rdr["ID"  ]);
								new DynamicControl(this, "SHIPPING_CONTACT_NAME").Text = Sql.ToString(rdr["NAME"]);
								// 08/21/2010 Paul.  Update address if the contact changes. 
								DynamicControl ctlSHIPPING_ACCOUNT_ID = new DynamicControl(this, "SHIPPING_ACCOUNT_ID");
								// 08/22/2010 Paul.  Only update account if empty. 
								if ( !Sql.IsEmptyGuid(gACCOUNT_ID) && Sql.IsEmptyGuid(ctlSHIPPING_ACCOUNT_ID.ID) )
								{
									//new DynamicControl(this, "SHIPPING_ACCOUNT_ID"        ).ID   = Sql.ToGuid  (rdr["ACCOUNT_ID"            ]);
									//new DynamicControl(this, "SHIPPING_ACCOUNT_NAME"      ).Text = Sql.ToString(rdr["ACCOUNT_NAME"          ]);
									UpdateAccount(gACCOUNT_ID, false, true);
								}
								if ( !Sql.IsEmptyString(rdr["ALT_ADDRESS_STREET"]) )
								{
									new DynamicControl(this, "SHIPPING_ADDRESS_STREET"    ).Text = Sql.ToString(rdr["ALT_ADDRESS_STREET"    ]);
									new DynamicControl(this, "SHIPPING_ADDRESS_CITY"      ).Text = Sql.ToString(rdr["ALT_ADDRESS_CITY"      ]);
									new DynamicControl(this, "SHIPPING_ADDRESS_STATE"     ).Text = Sql.ToString(rdr["ALT_ADDRESS_STATE"     ]);
									new DynamicControl(this, "SHIPPING_ADDRESS_POSTALCODE").Text = Sql.ToString(rdr["ALT_ADDRESS_POSTALCODE"]);
									new DynamicControl(this, "SHIPPING_ADDRESS_COUNTRY"   ).Text = Sql.ToString(rdr["ALT_ADDRESS_COUNTRY"   ]);
								}
							}
						}
					}
				}
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
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					string sLOAD_MODULE     = "Invoices"  ;
					string sLOAD_MODULE_KEY = "INVOICE_ID";
					Guid gQUOTE_ID    = Sql.ToGuid(Request["QUOTE_ID"]);
					Guid gORDER_ID    = Sql.ToGuid(Request["ORDER_ID"]);
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					// 06/17/2008 Paul.  We need to save the DuplicateID so that we can make sure not to delete original line items. 
					ViewState["DuplicateID"] = gDuplicateID;
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) || !Sql.IsEmptyGuid(gQUOTE_ID) || !Sql.IsEmptyGuid(gORDER_ID) )
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
								if ( !Sql.IsEmptyGuid(gQUOTE_ID) )
								{
									// 04/28/2007 Paul.  Load the data from the QUOTES record. 
									sLOAD_MODULE     = "Quotes"  ;
									sLOAD_MODULE_KEY = "QUOTE_ID";
									m_sVIEW_NAME = "vwQUOTES_ConvertToInvoice";
									sSQL = "select *                        " + ControlChars.CrLf
									     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
									     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									// 04/28/2007 Paul.  Filter by the module we are loading. 
									Security.Filter(cmd, sLOAD_MODULE, "edit");
									Sql.AppendParameter(cmd, gQUOTE_ID, "ID", false);
									// 06/17/2008 Paul.  If converting from a quote, then make sure not to delete original line items. 
									ViewState["DuplicateID"] = gQUOTE_ID;
								}
								else if ( !Sql.IsEmptyGuid(gORDER_ID) )
								{
									// 04/28/2007 Paul.  Load the data from the ORDERS record. 
									sLOAD_MODULE     = "Orders"  ;
									sLOAD_MODULE_KEY = "ORDER_ID";
									m_sVIEW_NAME = "vwORDERS_ConvertToInvoice";
									sSQL = "select *                        " + ControlChars.CrLf
									     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
									     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									// 04/28/2007 Paul.  Filter by the module we are loading. 
									Security.Filter(cmd, sLOAD_MODULE, "edit");
									Sql.AppendParameter(cmd, gORDER_ID, "ID", false);
									// 06/17/2008 Paul.  If converting from a order, then make sure not to delete original line items. 
									ViewState["DuplicateID"] = gORDER_ID;
								}
								else
								{
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
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + ".EditAddress"    , rdr);
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + ".EditDescription", rdr);
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											ViewState["BILLING_ACCOUNT_ID" ] = Sql.ToGuid(rdr["BILLING_ACCOUNT_ID" ]);
											ViewState["SHIPPING_ACCOUNT_ID"] = Sql.ToGuid(rdr["SHIPPING_ACCOUNT_ID"]);
											// 08/21/2010 Paul.  Update address if the contact changes. 
											ViewState["BILLING_CONTACT_ID" ] = Sql.ToGuid(rdr["BILLING_CONTACT_ID" ]);
											ViewState["SHIPPING_CONTACT_ID"] = Sql.ToGuid(rdr["SHIPPING_CONTACT_ID"]);

											new DynamicControl(this, "QUOTE_ID").ID = Sql.ToGuid(rdr["QUOTE_ID"]);
											new DynamicControl(this, "ORDER_ID").ID = Sql.ToGuid(rdr["ORDER_ID"]);
											
											// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
											this.AppendEditViewRelationships(m_sMODULE + "." + LayoutEditView, plcSubPanel, Sql.IsEmptyGuid(Request["ID"]));
											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain       , rdr);
											// 09/02/2012 Paul.  EditViews were combined into a single view. 
											//this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , rdr);
											this.AppendEditViewFields(m_sMODULE + ".EditDescription", tblDescription, rdr);
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											
											// 08/04/2010 Paul.  Now that we have fixed the stored procedure to allow the INVOICE_NUM to be defined, 
											// we needed to make sure that the number gets reset when copying a record. 
											if ( !Sql.IsEmptyGuid(ViewState["DuplicateID"]) )
											{
												new DynamicControl(this, "INVOICE_NUM").Text = String.Empty;
											}
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											// 12/16/2008 Paul.  LoadLineItems will close the rdr, so get the date before loading the items. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											
											// 01/28/2010 Paul.  Use ViewState and Page.Items to be compatible with the DetailViews. 
											ViewState ["NAME"            ] = Sql.ToString(rdr["NAME"            ]);
											ViewState ["ASSIGNED_USER_ID"] = Sql.ToGuid  (rdr["ASSIGNED_USER_ID"]);
											Page.Items["NAME"            ] = ViewState ["NAME"            ];
											Page.Items["ASSIGNED_USER_ID"] = ViewState ["ASSIGNED_USER_ID"];
											if ( !Sql.IsEmptyGuid(gQUOTE_ID) )
											{
												new DynamicControl(this, "QUOTE_ID"  ).ID   = gQUOTE_ID;
												new DynamicControl(this, "QUOTE_NAME").Text = Sql.ToString(rdr["NAME"]);
												ctlEditLineItemsView.LoadLineItems(gQUOTE_ID, Guid.Empty, con, rdr, sLOAD_MODULE, sLOAD_MODULE_KEY);
											}
											else if ( !Sql.IsEmptyGuid(gORDER_ID) )
											{
												new DynamicControl(this, "ORDER_ID"  ).ID   = gORDER_ID;
												new DynamicControl(this, "ORDER_NAME").Text = Sql.ToString(rdr["NAME"]);
												ctlEditLineItemsView.LoadLineItems(gORDER_ID, Guid.Empty, con, rdr, sLOAD_MODULE, sLOAD_MODULE_KEY);
											}
											else
											{
												ctlEditLineItemsView.LoadLineItems(gID, gDuplicateID, con, rdr, sLOAD_MODULE, sLOAD_MODULE_KEY);
											}
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + ".EditAddress"    , rdr);
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + ".EditDescription", rdr);
										}
										else
										{
											ctlEditLineItemsView.LoadLineItems(gID, gDuplicateID, con, null, String.Empty, String.Empty);
											
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlFooterButtons .DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
											// 01/27/2010 Paul.  Hide any subpanel data. 
											plcSubPanel.Visible = false;
										}
									}
								}
							}
						}
					}
					else
					{
						// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
						this.AppendEditViewRelationships(m_sMODULE + "." + LayoutEditView, plcSubPanel, Sql.IsEmptyGuid(Request["ID"]));
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain       , null);
						// 09/02/2012 Paul.  EditViews were combined into a single view. 
						//this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
						this.AppendEditViewFields(m_sMODULE + ".EditDescription", tblDescription, null);
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

						// 06/08/2006 Paul.  Prepopulate the Account. 
						Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
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
								UpdateAccount(gPARENT_ID, true, true);
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
							}
							if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Contacts" )
							{
								UpdateContact(gPARENT_ID, true, true);
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
							}
							else if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Opportunities" )
							{
								new DynamicControl(this, "OPPORTUNITY_ID"   ).ID   = gPARENT_ID;
								new DynamicControl(this, "OPPORTUNITY_NAME" ).Text = sPARENT_NAME;
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
							}
						}
						ctlEditLineItemsView.LoadLineItems(gID, gDuplicateID, null, null, String.Empty, String.Empty);
						// 11/10/2010 Paul.  Apply Business Rules. 
						this.ApplyEditViewNewEventRules(m_sMODULE + "." + LayoutEditView);
						this.ApplyEditViewNewEventRules(m_sMODULE + ".EditAddress"    );
						this.ApplyEditViewNewEventRules(m_sMODULE + ".EditDescription");
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
					// 01/28/2010 Paul.  We need to restore the page items on each postback. 
					Page.Items["NAME"            ] = ViewState ["NAME"            ];
					Page.Items["ASSIGNED_USER_ID"] = ViewState ["ASSIGNED_USER_ID"];

					DynamicControl ctlBILLING_ACCOUNT_ID  = new DynamicControl(this, "BILLING_ACCOUNT_ID" );
					DynamicControl ctlSHIPPING_ACCOUNT_ID = new DynamicControl(this, "SHIPPING_ACCOUNT_ID");
					if ( Sql.ToGuid(ViewState["BILLING_ACCOUNT_ID" ]) != ctlBILLING_ACCOUNT_ID.ID )
					{
						UpdateAccount(ctlBILLING_ACCOUNT_ID.ID, true, true);
						ViewState["BILLING_ACCOUNT_ID" ] = ctlBILLING_ACCOUNT_ID.ID;
						ViewState["SHIPPING_ACCOUNT_ID"] = ctlBILLING_ACCOUNT_ID.ID;
					}
					if ( Sql.ToGuid(ViewState["SHIPPING_ACCOUNT_ID"]) != ctlSHIPPING_ACCOUNT_ID.ID )
					{
						UpdateAccount(ctlSHIPPING_ACCOUNT_ID.ID, false, true);
						ViewState["SHIPPING_ACCOUNT_ID"] = ctlSHIPPING_ACCOUNT_ID.ID;
					}

					// 08/21/2010 Paul.  Update address if the contact changes. 
					DynamicControl ctlBILLING_CONTACT_ID  = new DynamicControl(this, "BILLING_CONTACT_ID" );
					DynamicControl ctlSHIPPING_CONTACT_ID = new DynamicControl(this, "SHIPPING_CONTACT_ID");
					if ( Sql.ToGuid(ViewState["BILLING_CONTACT_ID" ]) != ctlBILLING_CONTACT_ID.ID )
					{
						UpdateContact(ctlBILLING_CONTACT_ID.ID, true, true);
						ViewState["BILLING_CONTACT_ID" ] = ctlBILLING_CONTACT_ID.ID;
						ViewState["SHIPPING_CONTACT_ID"] = ctlBILLING_CONTACT_ID.ID;
					}
					if ( Sql.ToGuid(ViewState["SHIPPING_CONTACT_ID"]) != ctlSHIPPING_CONTACT_ID.ID )
					{
						UpdateContact(ctlSHIPPING_CONTACT_ID.ID, false, true);
						ViewState["SHIPPING_CONTACT_ID"] = ctlSHIPPING_CONTACT_ID.ID;
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
			m_sMODULE = "Invoices";
			// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
			m_sVIEW_NAME = "vw" + Crm.Modules.TableName(m_sMODULE) + "_Edit";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 04/19/2010 Paul.  New approach to EditView Relationships will distinguish between New Record and Existing Record.
				// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
				this.AppendEditViewRelationships(m_sMODULE + "." + LayoutEditView, plcSubPanel, Sql.IsEmptyGuid(Request["ID"]));
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain       , null);
				// 09/02/2012 Paul.  EditViews were combined into a single view. 
				//this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
				this.AppendEditViewFields(m_sMODULE + ".EditDescription", tblDescription, null);
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
