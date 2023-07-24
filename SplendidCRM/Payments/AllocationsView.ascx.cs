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

namespace SplendidCRM.Payments
{
	/// <summary>
	///		Summary description for AllocationsView.
	/// </summary>
	public class AllocationsView : SplendidControl
	{
		protected Guid            gACCOUNT_ID           ;
		// 05/01/2013 Paul.  Add Contacts field to support B2C. 
		protected Guid            gB2C_CONTACT_ID       ;
		protected DataTable       dtLineItems           ;
		protected GridView        grdMain               ;
		protected Label           lblLineItemError      ;
		protected DropDownList    CURRENCY_ID           ;
		protected TextBox         ALLOCATED             ;
		protected HiddenField     ALLOCATED_USDOLLAR    ;
		protected HiddenField     EXCHANGE_RATE         ;
		protected HiddenField     INVOICE_IDS           ;
		protected Button          btnINVOICES_CHANGED   ;
		protected bool            bAjaxAutoComplete     = false;

		public Decimal ALLOCATED_TOTAL
		{
			get { return Sql.ToDecimal(ALLOCATED.Text); }
		}

		public DataTable LineItems
		{
			get { return dtLineItems; }
		}

		#region Line Item Editing
		// 08/19/2010 Paul.  We are having a problem with the Update and Cancel buttons not working. 
		// Instead of using the default grid buttons, use our own. 
		protected void LINE_ITEM_Command(Object sender, CommandEventArgs e)
		{
			switch ( e.CommandName )
			{
				case "Update":
				{
					GridViewUpdateEventArgs arg = new GridViewUpdateEventArgs(grdMain.EditIndex);
					grdMain_RowUpdating(grdMain, arg);
					break;
				}
				case "Cancel":
				{
					GridViewCancelEditEventArgs arg = new GridViewCancelEditEventArgs(grdMain.EditIndex);
					grdMain_RowCancelingEdit(grdMain, arg);
					break;
				}
				case "Edit":
				{
					int nIndex = Sql.ToInteger(e.CommandArgument);
					GridViewEditEventArgs arg = new GridViewEditEventArgs(nIndex);
					grdMain_RowEditing(grdMain, arg);
					break;
				}
				case "Delete":
				{
					int nIndex = Sql.ToInteger(e.CommandArgument);
					GridViewDeleteEventArgs arg = new GridViewDeleteEventArgs(nIndex);
					grdMain_RowDeleting(grdMain, arg);
					break;
				}
			}
		}

		protected void grdMain_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if ( (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit )
			{
				if ( bAjaxAutoComplete )
				{
					// <ajaxToolkit:AutoCompleteExtender ID="autoNAME" TargetControlID="INVOICE_NAME" ServiceMethod="InvoiceNameList" ServicePath="~/Invoices/AutoComplete.asmx" MinimumPrefixLength="2" CompletionInterval="250" EnableCaching="true" CompletionSetCount="12" runat="server" />
					AjaxControlToolkit.AutoCompleteExtender auto = new AjaxControlToolkit.AutoCompleteExtender();
					e.Row.Cells[0].Controls.Add(auto);
					auto.ID                   = "autoNAME";
					auto.TargetControlID      = "INVOICE_NAME";
					auto.ServiceMethod        = "InvoiceNameList";
					auto.ServicePath          = "~/Invoices/AutoComplete.asmx";
					auto.MinimumPrefixLength  = 2;
					auto.CompletionInterval   = 250;
					auto.EnableCaching        = true;
					// 12/09/2010 Paul.  Provide a way to customize the AutoComplete.CompletionSetCount. 
					auto.CompletionSetCount   = Crm.Config.CompletionSetCount();
				}
			}
		}

		protected void grdMain_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if ( e.Row.RowType == DataControlRowType.DataRow )
			{
			}
		}

		protected void grdMain_RowEditing(object sender, GridViewEditEventArgs e)
		{
			grdMain.EditIndex = e.NewEditIndex;
			if ( dtLineItems != null )
			{
				grdMain.DataSource = dtLineItems;
				grdMain.DataBind();
			}
		}

		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			if ( dtLineItems != null )
			{
				//dtLineItems.Rows.RemoveAt(e.RowIndex);
				//dtLineItems.Rows[e.RowIndex].Delete();
				// 08/07/2007 fhsakai.  There might already be deleted rows, so make sure to first obtain the current rows. 
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				aCurrentRows[e.RowIndex].Delete();
				
				aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 02/04/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				if ( aCurrentRows.Length == 0 || !Sql.IsEmptyGuid(aCurrentRows[aCurrentRows.Length-1]["INVOICE_ID"]) )
				{
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				}
				UpdateTotals();

				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
				UpdatePaymentAmount();
			}
		}

		protected void grdMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			if ( dtLineItems != null )
			{
				GridViewRow gr = grdMain.Rows[e.RowIndex];
				TextBox     txtINVOICE_NAME        = gr.FindControl("INVOICE_NAME"       ) as TextBox     ;
				HiddenField txtINVOICE_ID          = gr.FindControl("INVOICE_ID"         ) as HiddenField ;
				TextBox     txtAMOUNT_DUE          = gr.FindControl("AMOUNT_DUE"         ) as TextBox     ;
				HiddenField txtAMOUNT_DUE_USDOLLAR = gr.FindControl("AMOUNT_DUE_USDOLLAR") as HiddenField ;
				TextBox     txtAMOUNT              = gr.FindControl("AMOUNT"             ) as TextBox     ;
				HiddenField txtAMOUNT_USDOLLAR     = gr.FindControl("AMOUNT_USDOLLAR"    ) as HiddenField ;

				DataRow row = dtLineItems.Rows[e.RowIndex];
				// 03/31/2007 Paul.  The US Dollar values are computed from the price and are only used when changing currencies. 
				if ( txtINVOICE_NAME != null ) row["INVOICE_NAME"] =               txtINVOICE_NAME.Text;
				if ( txtINVOICE_ID   != null ) row["INVOICE_ID"  ] = Sql.ToGuid   (txtINVOICE_ID  .Value);
				// 02/07/2010 Paul.  Defensive programming, although the fields should always exist, but lets be safe. 
				if ( txtAMOUNT_DUE   != null )
				{
					row["AMOUNT_DUE"         ] = Sql.ToDecimal(txtAMOUNT_DUE  .Text);
					row["AMOUNT_DUE_USDOLLAR"] = C10n.FromCurrency(Sql.ToDecimal(txtAMOUNT_DUE.Text));
				}
				if ( txtAMOUNT       != null )
				{
					row["AMOUNT"             ] = Sql.ToDecimal(txtAMOUNT      .Text);
					row["AMOUNT_USDOLLAR"    ] = C10n.FromCurrency(Sql.ToDecimal(txtAMOUNT    .Text));
				}
				

				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 03/30/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				if ( aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length-1]["INVOICE_NAME"]) )
				{
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				}

				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 03/30/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
				UpdatePaymentAmount();
			}
		}

		protected void grdMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			grdMain.EditIndex = -1;
			if ( dtLineItems != null )
			{
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				grdMain.DataSource = dtLineItems;
				// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
				UpdatePaymentAmount();
			}
		}

		public void CURRENCY_ID_Changed(object sender, System.EventArgs e)
		{
			// 03/31/2007 Paul.  When the currency changes, use the default exchange rate. 
			Guid gCURRENCY_ID = C10n.ID;
			// 10/12/2010 Paul.  Currency field might not exist. 
			if ( CURRENCY_ID != null )
			{
				gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
				SetC10n(gCURRENCY_ID);
				// 04/30/2016 Paul.  If we are connected to the currency service, then now is a good time to check for changes. 
				if ( !Sql.IsEmptyString(Application["CONFIG.CurrencyLayer.AccessKey"]) )
				{
					StringBuilder sbErrors = new StringBuilder();
					float dRate = OrderUtils.GetCurrencyConversionRate(Application, C10n.ISO4217, sbErrors);
					if ( sbErrors.Length == 0 )
					{
						C10n.CONVERSION_RATE = dRate;
					}
				}
			}
			EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();

			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					row["AMOUNT"] = C10n.ToCurrency(Sql.ToDecimal(row["AMOUNT_USDOLLAR"]));
				}
			}
			grdMain.DataBind();

			UpdateTotals();
			UpdatePaymentAmount();
		}

		private void UpdateTotals()
		{
			Double dALLOCATED = 0.0;

			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					Double dAMOUNT = Sql.ToDouble (row["AMOUNT"]);
					dALLOCATED += dAMOUNT;
				}
			}
			ALLOCATED.Text           = Convert.ToDecimal(dALLOCATED).ToString("c");
			ALLOCATED_USDOLLAR.Value = C10n.FromCurrency(Convert.ToDecimal(dALLOCATED)).ToString("0.000");
		}

		private void UpdatePaymentAmount()
		{
			// 04/23/2008 Paul.  When the Allocated Total is updated, also update the Payment Amount. 
			// 02/22/2015 Paul.  We broke this code on 03/12/2014 when we enabled EditView div access to RulesWizard. 
			string sAMOUNT = C10n.ToCurrency(Sql.ToDecimal(ALLOCATED_USDOLLAR.Value)).ToString("0.00");
			SplendidControl ctlParent = (this.Parent as SplendidControl);
			if ( ctlParent == null )
				ctlParent = (this.Parent.Parent as SplendidControl);
			if ( ctlParent != null )
				new DynamicControl(ctlParent, "AMOUNT").Text = sAMOUNT;
			// 02/22/2015 Paul.  Not sure why amount is not getting updated, so post as a command to the parent. This is likely a .NET UpdatePanel issue. 
			ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "UpdatePaymentAmount", "document.getElementById('" + new DynamicControl(ctlParent, "AMOUNT").ClientID + "').value = '" + sAMOUNT + "';", true);
		}
		#endregion

		// 02/09/2008 Paul.  We need the ACCOUNT_ID so that the invoice popup will be specific to this account. 
		// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
		// 05/01/2013 Paul.  Add Contacts field to support B2C. 
		public void LoadLineItems(Guid gID, Guid gACCOUNT_ID, Guid gB2C_CONTACT_ID, Guid gDuplicateID, IDbConnection con, DataRow rdr)
		{
			this.gACCOUNT_ID = gACCOUNT_ID;
			this.gB2C_CONTACT_ID = gB2C_CONTACT_ID;
			ViewState["ACCOUNT_ID"] = gACCOUNT_ID;
			ViewState["B2C_CONTACT_ID"] = gB2C_CONTACT_ID;
			// 08/04/2010 Paul.  We need to allow a new payment to be created without first specifying an account or invoice. 
			//if ( !IsPostBack )
			{
				GetCurrencyControl();
				try
				{
					// 03/15/2007 Paul.  Set the default currency to the current user currency. 
					// 08/19/2010 Paul.  Check the list before assigning the value. 
					// 10/12/2010 Paul.  Currency field might not exist. 
					if ( CURRENCY_ID != null )
						Utils.SetValue(CURRENCY_ID, C10n.ID.ToString());
					EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
				}
				catch
				{
				}
				foreach ( DataControlField col in grdMain.Columns )
				{
					if ( !Sql.IsEmptyString(col.HeaderText) )
					{
						col.HeaderText = L10n.Term(col.HeaderText);
					}
					CommandField cf = col as CommandField;
					if ( cf != null )
					{
						cf.EditText   = L10n.Term(cf.EditText  );
						cf.DeleteText = L10n.Term(cf.DeleteText);
						cf.UpdateText = L10n.Term(cf.UpdateText);
						cf.CancelText = L10n.Term(cf.CancelText);
					}
				}

				if ( (!Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID)) && (con != null) && (rdr != null) )
				{
					try
					{
						// 08/19/2010 Paul.  Check the list before assigning the value. 
						// 10/12/2010 Paul.  Currency field might not exist. 
						if ( CURRENCY_ID != null )
							Utils.SetValue(CURRENCY_ID, Sql.ToString(rdr["CURRENCY_ID"]));
						// 05/26/2007 Paul.  Make sure to update the exchange rate. 
						EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
					}
					catch
					{
					}
					// 03/31/2007 Paul.  The exchange rate might be an old value. 
					float fEXCHANGE_RATE = Sql.ToFloat(rdr["EXCHANGE_RATE"]);
					if ( fEXCHANGE_RATE == 0.0f )
						fEXCHANGE_RATE = 1.0f;
					EXCHANGE_RATE.Value = fEXCHANGE_RATE.ToString();
					// 10/12/2010 Paul.  Currency field might not exist. 
					if ( CURRENCY_ID != null && CURRENCY_ID.Items.Count > 0 )
					{
						// 03/31/2007 Paul.  Replace the user currency with the form currency, but use the old exchange rate. 
						Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
						SetC10n(gCURRENCY_ID, fEXCHANGE_RATE);
						EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
					}
					// 05/26/2007 Paul.  ALLOCATED field is not returned, just TOTAL_ALLOCATED_USDOLLAR. 
					// Don't convert TOTAL_ALLOCATED_USDOLLAR in the hidden variable. 
					ALLOCATED.Text           = C10n.ToCurrency(Sql.ToDecimal(rdr["TOTAL_ALLOCATED_USDOLLAR"])).ToString("c");
					ALLOCATED_USDOLLAR.Value = Sql.ToDecimal(rdr["TOTAL_ALLOCATED_USDOLLAR"]).ToString("0.00");
					// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
					//rdr.Close();

					string sSQL;
					sSQL = "select *                  " + ControlChars.CrLf
					     + "  from vwPAYMENTS_INVOICES" + ControlChars.CrLf
					     + " where 1 = 1              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID", false);
						cmd.CommandText += " order by DATE_MODIFIED asc" + ControlChars.CrLf;

						if ( bDebug )
							RegisterClientScriptBlock("vwPAYMENTS_INVOICES", Sql.ClientScriptBlock(cmd));

						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							dtLineItems = new DataTable();
							da.Fill(dtLineItems);
							
							// 04/01/2007 Paul.  If we are duplicating a quote, then we must create new IDs for the line items. 
							// Otherwise, edits to the line items will change the old quote. 
							if ( !Sql.IsEmptyGuid(gDuplicateID) )
							{
								foreach ( DataRow row in dtLineItems.Rows )
								{
									row["ID"] = Guid.NewGuid();
								}
							}
							// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
							DataRow rowNew = dtLineItems.NewRow();
							dtLineItems.Rows.Add(rowNew);

							ViewState["LineItems"] = dtLineItems;
							grdMain.DataSource = dtLineItems;
							// 03/27/2007 Paul.  Start with last line enabled for editing. 
							grdMain.EditIndex = dtLineItems.Rows.Count - 1;
							grdMain.DataBind();
						}
					}
				}
				else
				{
					dtLineItems = new DataTable();
					DataColumn colID                  = new DataColumn("ID"                 , Type.GetType("System.Guid"   ));
					DataColumn colINVOICE_NAME        = new DataColumn("INVOICE_NAME"       , Type.GetType("System.String" ));
					DataColumn colINVOICE_ID          = new DataColumn("INVOICE_ID"         , Type.GetType("System.Guid"   ));
					DataColumn colAMOUNT_DUE          = new DataColumn("AMOUNT_DUE"         , Type.GetType("System.Decimal"));
					DataColumn colAMOUNT_DUE_USDOLLAR = new DataColumn("AMOUNT_DUE_USDOLLAR", Type.GetType("System.Decimal"));
					DataColumn colAMOUNT              = new DataColumn("AMOUNT"             , Type.GetType("System.Decimal"));
					DataColumn colAMOUNT_USDOLLAR     = new DataColumn("AMOUNT_USDOLLAR"    , Type.GetType("System.Decimal"));
					dtLineItems.Columns.Add(colID                 );
					dtLineItems.Columns.Add(colINVOICE_NAME       );
					dtLineItems.Columns.Add(colINVOICE_ID         );
					dtLineItems.Columns.Add(colAMOUNT_DUE         );
					dtLineItems.Columns.Add(colAMOUNT_DUE_USDOLLAR);
					dtLineItems.Columns.Add(colAMOUNT             );
					dtLineItems.Columns.Add(colAMOUNT_USDOLLAR    );
					// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
					DataRow rowNew = null;

					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection conInvoice = dbf.CreateConnection() )
					{
						conInvoice.Open();
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vwINVOICES_Edit" + ControlChars.CrLf
						     + " where 1 = 1          " + ControlChars.CrLf;
						using ( IDbCommand cmd = conInvoice.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
							Sql.AppendParameter(cmd, gPARENT_ID, "ID", false);

							if ( bDebug )
								RegisterClientScriptBlock("vwINVOICES_Edit", Sql.ClientScriptBlock(cmd));

							using ( IDataReader rdrInvoice = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdrInvoice.Read() )
								{
									rowNew = dtLineItems.NewRow();
									rowNew["INVOICE_NAME"       ] = Sql.ToString (rdrInvoice["NAME"               ]);
									rowNew["INVOICE_ID"         ] = Sql.ToGuid   (rdrInvoice["ID"                 ]);
									rowNew["AMOUNT_DUE"         ] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE"         ]);
									rowNew["AMOUNT_DUE_USDOLLAR"] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE_USDOLLAR"]);
									rowNew["AMOUNT"             ] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE"         ]);
									rowNew["AMOUNT_USDOLLAR"    ] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE_USDOLLAR"]);
									// 02/25/2008 Paul.  If AMOUNT_DUE has not been computed, then use the TOTAL. 
									if ( rdrInvoice["AMOUNT_DUE"] == DBNull.Value )
									{
										rowNew["AMOUNT_DUE"         ] = Sql.ToDecimal(rdrInvoice["TOTAL"         ]);
										rowNew["AMOUNT_DUE_USDOLLAR"] = Sql.ToDecimal(rdrInvoice["TOTAL_USDOLLAR"]);
										rowNew["AMOUNT"             ] = Sql.ToDecimal(rdrInvoice["TOTAL"         ]);
										rowNew["AMOUNT_USDOLLAR"    ] = Sql.ToDecimal(rdrInvoice["TOTAL_USDOLLAR"]);
									}
									dtLineItems.Rows.Add(rowNew);
								}
							}
						}
					}
					rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);

					ViewState["LineItems"] = dtLineItems;
					grdMain.DataSource = dtLineItems;
					// 02/03/2007 Paul.  Start with last line enabled for editing. 
					grdMain.EditIndex = dtLineItems.Rows.Count - 1;
					grdMain.DataBind();

					UpdateTotals();
				}
			}
		}

		protected DropDownList GetCurrencyControl()
		{
			try
			{
				if ( CURRENCY_ID == null )
				{
					// 02/07/2010 Paul.  Defensive programming, check for tblMain before using. 
					Control tblMain = Parent.FindControl("tblMain");
					if ( tblMain != null )
					{
						CURRENCY_ID = tblMain.FindControl("CURRENCY_ID") as DropDownList;
					}
				}
			}
			catch
			{
			}
			return CURRENCY_ID;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			GetCurrencyControl();
			if ( IsPostBack )
			{
				try
				{
					// 10/12/2010 Paul.  Currency field might not exist. 
					if ( CURRENCY_ID != null && CURRENCY_ID.Items.Count > 0 )
					{
						// 03/31/2007 Paul.  Replace the user currency with the form currency, but use the old exchange rate. 
						Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
						SetC10n(gCURRENCY_ID, Sql.ToFloat(EXCHANGE_RATE.Value));
						// 04/30/2016 Paul.  If we are connected to the currency service, then now is a good time to check for changes. 
						if ( !Sql.IsEmptyString(Application["CONFIG.CurrencyLayer.AccessKey"]) )
						{
							StringBuilder sbErrors = new StringBuilder();
							float dRate = OrderUtils.GetCurrencyConversionRate(Application, C10n.ISO4217, sbErrors);
							if ( sbErrors.Length == 0 )
							{
								C10n.CONVERSION_RATE = dRate;
							}
						}
						EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
					}
				}
				catch
				{
				}
				// 08/05/2010 Paul.  Need to save and restore the account. 
				gACCOUNT_ID = Sql.ToGuid(ViewState["ACCOUNT_ID"]);
				// 05/01/2013 Paul.  Add Contacts field to support B2C. 
				gB2C_CONTACT_ID = Sql.ToGuid(ViewState["B2C_CONTACT_ID"]);

				dtLineItems = ViewState["LineItems"] as DataTable;
				grdMain.DataSource = dtLineItems;
				// 03/31/2007 Paul.  Don't bind the grid, otherwise edits will be lost. 
				//grdMain.DataBind();
				if ( INVOICE_IDS.Value.Length > 0 )
				{
					DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					aCurrentRows[grdMain.EditIndex].Delete();
					
					DataRow rowNew = null;
					string[] arrINVOICE_IDs = INVOICE_IDS.Value.Split(',');
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection conInvoice = dbf.CreateConnection() )
					{
						conInvoice.Open();
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vwINVOICES_Edit" + ControlChars.CrLf
						     + " where 1 = 1          " + ControlChars.CrLf;
						using ( IDbCommand cmd = conInvoice.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AppendGuids(cmd, arrINVOICE_IDs, "ID");

							if ( bDebug )
								RegisterClientScriptBlock("vwINVOICES_Edit", Sql.ClientScriptBlock(cmd));

							using ( IDataReader rdrInvoice = cmd.ExecuteReader() )
							{
								while ( rdrInvoice.Read() )
								{
									rowNew = dtLineItems.NewRow();
									rowNew["INVOICE_NAME"       ] = Sql.ToString (rdrInvoice["NAME"               ]);
									rowNew["INVOICE_ID"         ] = Sql.ToGuid   (rdrInvoice["ID"                 ]);
									rowNew["AMOUNT_DUE"         ] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE"         ]);
									rowNew["AMOUNT_DUE_USDOLLAR"] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE_USDOLLAR"]);
									rowNew["AMOUNT"             ] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE"         ]);
									rowNew["AMOUNT_USDOLLAR"    ] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE_USDOLLAR"]);
									dtLineItems.Rows.Add(rowNew);
								}
							}
						}
					}
					rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);

					ViewState["LineItems"] = dtLineItems;
					grdMain.DataSource = dtLineItems;
					// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					grdMain.EditIndex = aCurrentRows.Length - 1;
					grdMain.DataBind();
					UpdateTotals();
					UpdatePaymentAmount();
					
					INVOICE_IDS.Value = String.Empty;
				}
			}
			
			// 05/06/2010 Paul.  Move the ajax refence code to Page_Load as it only needs to be called once. 
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			// 11/23/2009 Paul.  SplendidCRM 4.0 is very slow on Blackberry devices.  Lets try and turn off AJAX AutoComplete. 
			bAjaxAutoComplete = (mgrAjax != null);
			if ( this.IsMobile )
			{
				// 11/24/2010 Paul.  .NET 4 has broken the compatibility of the browser file system. 
				// We are going to minimize our reliance on browser files in order to reduce deployment issues. 
				bAjaxAutoComplete = Utils.AllowAutoComplete && (mgrAjax != null);
			}
			if ( bAjaxAutoComplete )
			{
				ServiceReference svc = new ServiceReference("~/Invoices/AutoComplete.asmx");
				// 04/23/2008 Paul.  We use the same AutoComplete service, but we display different values in the grid, so use a different auto-complete. 
				ScriptReference  scr = new ScriptReference ("~/Payments/AutoComplete.js"  );
				if ( !mgrAjax.Services.Contains(svc) )
					mgrAjax.Services.Add(svc);
				if ( !mgrAjax.Scripts.Contains(scr) )
					mgrAjax.Scripts.Add(scr);
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
			grdMain.RowCreated       += new GridViewRowEventHandler       (grdMain_RowCreated      );
			grdMain.RowDataBound     += new GridViewRowEventHandler       (grdMain_RowDataBound    );
			// 08/19/2010 Paul.  We are having a problem with the Update and Cancel buttons not working. 
			// Instead of using the default grid buttons, use our own. 
			//grdMain.RowEditing       += new GridViewEditEventHandler      (grdMain_RowEditing      );
			//grdMain.RowDeleting      += new GridViewDeleteEventHandler    (grdMain_RowDeleting     );
			//grdMain.RowUpdating      += new GridViewUpdateEventHandler    (grdMain_RowUpdating     );
			//grdMain.RowCancelingEdit += new GridViewCancelEditEventHandler(grdMain_RowCancelingEdit);

		}
		#endregion
	}
}
