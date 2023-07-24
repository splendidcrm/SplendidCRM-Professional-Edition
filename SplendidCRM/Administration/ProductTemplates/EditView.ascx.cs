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

namespace SplendidCRM.Administration.ProductTemplates
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

		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveDuplicate" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
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
							// 11/11/2010 Paul.  The connection should be opened before getting the current values. 
							con.Open();
							// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *                       " + ControlChars.CrLf
								     + "  from vwPRODUCT_TEMPLATES_Edit" + ControlChars.CrLf;
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
									// 02/03/2009 Paul.  Add TEAM_ID for team management. 
									// 07/10/2010 Paul.  Add Options fields. 
									// 08/13/2010 Paul.  New discount fields. 
									// 09/20/2010 Paul.  PRICING_FACTOR is now a float. 
									// 05/21/2012 Paul.  Add QUICKBOOKS_ACCOUNT to support QuickBooks sync. 
									// 12/13/2013 Paul.  Allow each product to have a default tax rate. 
									// 10/21/2015 Paul.  Add min and max order fields for published data. 
									// 01/29/2019 Paul.  Add Tags module. 
									SqlProcs.spPRODUCT_TEMPLATES_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "NAME"                          ).Text
										, new DynamicControl(this, rowCurrent, "STATUS"                        ).SelectedValue
										, new DynamicControl(this, rowCurrent, "QUANTITY"                      ).IntegerValue
										, new DynamicControl(this, rowCurrent, "DATE_AVAILABLE"                ).DateValue
										, new DynamicControl(this, rowCurrent, "DATE_COST_PRICE"               ).DateValue
										, new DynamicControl(this, rowCurrent, "ACCOUNT_ID"                    ).ID
										, new DynamicControl(this, rowCurrent, "MANUFACTURER_ID"               ).ID
										, new DynamicControl(this, rowCurrent, "CATEGORY_ID"                   ).ID
										, new DynamicControl(this, rowCurrent, "TYPE_ID"                       ).ID
										, new DynamicControl(this, rowCurrent, "WEBSITE"                       ).Text
										, new DynamicControl(this, rowCurrent, "MFT_PART_NUM"                  ).Text
										, new DynamicControl(this, rowCurrent, "VENDOR_PART_NUM"               ).Text
										, new DynamicControl(this, rowCurrent, "TAX_CLASS"                     ).SelectedValue
										, new DynamicControl(this, rowCurrent, "WEIGHT"                        ).FloatValue
										, new DynamicControl(this, rowCurrent, "CURRENCY_ID"                   ).ID
										, new DynamicControl(this, rowCurrent, "COST_PRICE"                    ).DecimalValue
										, new DynamicControl(this, rowCurrent, "LIST_PRICE"                    ).DecimalValue
										, new DynamicControl(this, rowCurrent, "DISCOUNT_PRICE"                ).DecimalValue
										, new DynamicControl(this, rowCurrent, "PRICING_FACTOR"                ).FloatValue
										, new DynamicControl(this, rowCurrent, "PRICING_FORMULA"               ).SelectedValue
										, new DynamicControl(this, rowCurrent, "SUPPORT_NAME"                  ).Text
										, new DynamicControl(this, rowCurrent, "SUPPORT_CONTACT"               ).Text
										, new DynamicControl(this, rowCurrent, "SUPPORT_DESCRIPTION"           ).Text
										, new DynamicControl(this, rowCurrent, "SUPPORT_TERM"                  ).SelectedValue
										, new DynamicControl(this, rowCurrent, "DESCRIPTION"                   ).Text
										, new DynamicControl(this, rowCurrent, "TEAM_ID"                       ).ID
										, new DynamicControl(this, rowCurrent, "TEAM_SET_LIST"                 ).Text
										, new DynamicControl(this, rowCurrent, "MINIMUM_OPTIONS"               ).IntegerValue
										, new DynamicControl(this, rowCurrent, "MAXIMUM_OPTIONS"               ).IntegerValue
										, new DynamicControl(this, rowCurrent, "DISCOUNT_ID"                   ).ID
										, new DynamicControl(this, rowCurrent, "QUICKBOOKS_ACCOUNT"            ).Text
										, new DynamicControl(this, rowCurrent, "TAXRATE_ID"                    ).ID
										, new DynamicControl(this, rowCurrent, "MINIMUM_QUANTITY"              ).IntegerValue
										, new DynamicControl(this, rowCurrent, "MAXIMUM_QUANTITY"              ).IntegerValue
										, new DynamicControl(this, rowCurrent, "LIST_ORDER"                    ).IntegerValue
										// 01/29/2019 Paul.  Add Tags module. 
										, new DynamicControl(this, rowCurrent, "TAG_SET_NAME"               ).Text
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

		private void UpdateDiscount()
		{
			Decimal dCOST_PRICE      = new DynamicControl(this, "COST_PRICE"     ).DecimalValue;
			Decimal dLIST_PRICE      = new DynamicControl(this, "LIST_PRICE"     ).DecimalValue;
			Decimal dDISCOUNT_PRICE  = new DynamicControl(this, "DISCOUNT_PRICE" ).DecimalValue;
			Guid    gDISCOUNT_ID     = new DynamicControl(this, "DISCOUNT_ID"    ).ID;
			string  sPRICING_FORMULA = new DynamicControl(this, "PRICING_FORMULA").Text;
			// 02/21/2021 Paul.  Incorrect control name, was fPRICING_FACTOR. 
			float   fPRICING_FACTOR  = new DynamicControl(this, "PRICING_FACTOR").FloatValue;
			if ( !Sql.IsEmptyGuid(gDISCOUNT_ID) )
			{
				OrderUtils.DiscountPrice(gDISCOUNT_ID, dCOST_PRICE, dLIST_PRICE, ref dDISCOUNT_PRICE, ref sPRICING_FORMULA, ref fPRICING_FACTOR);
				new DynamicControl(this, "DISCOUNT_PRICE" ).Text = dDISCOUNT_PRICE.ToString("#,##0.00");
				new DynamicControl(this, "PRICING_FORMULA").Text = sPRICING_FORMULA;
				new DynamicControl(this, "PRICING_FACTOR" ).Text = fPRICING_FACTOR.ToString();
			}
			else if ( !Sql.IsEmptyString(sPRICING_FORMULA) )
			{
				OrderUtils.DiscountPrice(sPRICING_FORMULA, fPRICING_FACTOR, dCOST_PRICE, dLIST_PRICE, ref dDISCOUNT_PRICE);
				new DynamicControl(this, "DISCOUNT_PRICE").Text = dDISCOUNT_PRICE.ToString("#,##0.00");
			}
		}


		protected void COST_PRICE_TextChanged(object sender, EventArgs e)
		{
			UpdateDiscount();
			Decimal dCOST_PRICE = new DynamicControl(this, "COST_PRICE").DecimalValue;
			new DynamicControl(this, "COST_PRICE").Text = dCOST_PRICE.ToString("#,##0.00");
		}

		protected void LIST_PRICE_TextChanged(object sender, EventArgs e)
		{
			UpdateDiscount();
			Decimal dLIST_PRICE = new DynamicControl(this, "LIST_PRICE").DecimalValue;
			new DynamicControl(this, "LIST_PRICE").Text = dLIST_PRICE.ToString("#,##0.00");
		}

		protected void PRICING_FORMULA_SelectedIndexChanged(object sender, EventArgs e)
		{
			// 02/21/2021 Paul.  Clear DISCOUNT_PRICE before calling UpdateDiscount() as it may be changed. 
			new DynamicControl(this, "DISCOUNT_PRICE").Text = String.Empty;
			new DynamicControl(this, "DISCOUNT_ID"   ).Text = String.Empty;
			UpdateDiscount();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							// 08/10/2006 Paul.  Need to filter on ID, not 1 = 1.
							sSQL = "select *                       " + ControlChars.CrLf
							     + "  from vwPRODUCT_TEMPLATES_Edit" + ControlChars.CrLf
							     + " where ID = @ID                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
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
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											// 11/11/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
											
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											TextBox      COST_PRICE      = FindControl("COST_PRICE"     ) as TextBox;
											TextBox      LIST_PRICE      = FindControl("LIST_PRICE"     ) as TextBox;
											DropDownList PRICING_FORMULA = FindControl("PRICING_FORMULA") as DropDownList;
											if ( COST_PRICE != null )
											{
												COST_PRICE.AutoPostBack = true;
												COST_PRICE.TextChanged += new EventHandler(COST_PRICE_TextChanged);
											}
											if ( LIST_PRICE != null )
											{
												LIST_PRICE.AutoPostBack = true;
												LIST_PRICE.TextChanged += new EventHandler(LIST_PRICE_TextChanged);
											}
											if ( PRICING_FORMULA != null )
											{
												PRICING_FORMULA.AutoPostBack = true;
												PRICING_FORMULA.SelectedIndexChanged += new EventHandler(PRICING_FORMULA_SelectedIndexChanged);
											}
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											ViewState["DISCOUNT_ID"       ] = Sql.ToGuid    (rdr["DISCOUNT_ID"  ]);
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
						TextBox COST_PRICE = FindControl("COST_PRICE") as TextBox;
						TextBox LIST_PRICE = FindControl("LIST_PRICE") as TextBox;
						if ( COST_PRICE != null )
						{
							COST_PRICE.AutoPostBack = true;
							COST_PRICE.TextChanged += new EventHandler(COST_PRICE_TextChanged);
						}
						if ( LIST_PRICE != null )
						{
							LIST_PRICE.AutoPostBack = true;
							LIST_PRICE.TextChanged += new EventHandler(LIST_PRICE_TextChanged);
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
					
					DynamicControl ctlDISCOUNT_ID = new DynamicControl(this, "DISCOUNT_ID");
					if ( Sql.ToGuid(ViewState["DISCOUNT_ID"]) != ctlDISCOUNT_ID.ID )
					{
						UpdateDiscount();
						ViewState["DISCOUNT_ID"] = ctlDISCOUNT_ID.ID;
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
			m_sMODULE = "ProductTemplates";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				TextBox      COST_PRICE      = FindControl("COST_PRICE"     ) as TextBox;
				TextBox      LIST_PRICE      = FindControl("LIST_PRICE"     ) as TextBox;
				DropDownList PRICING_FORMULA = FindControl("PRICING_FORMULA") as DropDownList;
				if ( COST_PRICE != null )
				{
					COST_PRICE.AutoPostBack = true;
					COST_PRICE.TextChanged += new EventHandler(COST_PRICE_TextChanged);
				}
				if ( LIST_PRICE != null )
				{
					LIST_PRICE.AutoPostBack = true;
					LIST_PRICE.TextChanged += new EventHandler(LIST_PRICE_TextChanged);
				}
				if ( PRICING_FORMULA != null )
				{
					PRICING_FORMULA.AutoPostBack = true;
					PRICING_FORMULA.SelectedIndexChanged += new EventHandler(PRICING_FORMULA_SelectedIndexChanged);
				}
				
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
