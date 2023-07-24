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
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Web.Script.Serialization;
using System.Diagnostics;
using SplendidCRM._controls;

namespace SplendidCRM.Opportunities
{
	/// <summary>
	///		Summary description for EditLineItemsView.
	/// </summary>
	public class EditLineItemsView : SplendidControl
	{
		protected DataTable       dtLineItems           ;
		protected GridView        grdMain               ;
		protected DropDownList    CURRENCY_ID           ;
		protected Label           lblLineItemError      ;
		protected TextBox         SUBTOTAL              ;
		protected TextBox         DISCOUNT              ;
		protected TextBox         TOTAL                 ;
		protected HiddenField     DISCOUNT_USDOLLAR     ;
		protected bool            bAjaxAutoComplete     = false;
		protected bool            bEnableOptions        = false;
		protected HiddenField     hidPRODUCTS           ;
		protected Button          btnPRODUCT_OPTIONS    ;

		public DataTable LineItems
		{
			get { return dtLineItems; }
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
		}

		#region Line Item Editing
		protected bool FieldVisibility(string sLINE_ITEM_TYPE, string sFIELD_NAME)
		{
			bool bVisible = true;
			switch ( sFIELD_NAME )
			{
				case "QUANTITY"        :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "NAME"            :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "SELECT_NAME"     :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "DESCRIPTION"     :  bVisible = false; break;
				case "MFT_PART_NUM"    :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "TAX_CLASS"       :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "TAXRATE_ID"      :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "TAX"             :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "UNIT_PRICE"      :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "EXTENDED_PRICE"  :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "DISCOUNT_ID"     :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "DISCOUNT_NAME"   :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "DISCOUNT_PRICE"  :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "PRICING_FORMULA" :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "PRICING_FACTOR"  :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;

				case "OPPORTUNITY_TYPE":  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "LEAD_SOURCE"     :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "DATE_CLOSED"     :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "NEXT_STEP"       :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "SALES_STAGE"     :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "PROBABILITY"     :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
				case "btnComment"      :  bVisible = (sLINE_ITEM_TYPE != "Comment");  break;
			}
			return bVisible;
		}

		private int NextPosition(DataTable dt)
		{
			int nMaxPosition = 0;
			foreach ( DataRow row in dt.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					int nPosition = Sql.ToInteger(row["POSITION"]);
					if ( nPosition > nMaxPosition )
						nMaxPosition = nPosition;
				}
			}
			return nMaxPosition + 1;
		}

		// 11/18/2010 Paul.  Reverse the name of this function. 
		public static bool IsLineItemNotEmpty(DataRow row)
		{
			// 11/18/2010 Paul.  Include Discount and Subtotal in list of non-empty line items. 
			// 11/18/2010 Paul.  We do not need to check the DISCOUNT_ID as a discount cannot stand-alone. 
			if ( !Sql.IsEmptyString(row["NAME"]) || !Sql.IsEmptyGuid(row["PRODUCT_TEMPLATE_ID"]) || Sql.ToString(row["LINE_ITEM_TYPE"]) == "Subtotal" )
				return true;
			return false;
		}

		// 08/15/2010 Paul.  We are having a problem with the Update and Cancel buttons not working. 
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
				case "MoveUp":
				{
					DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					int nIndex = Sql.ToInteger(e.CommandArgument);
					if ( dtLineItems != null && nIndex > 0 && (nIndex + 1) < aCurrentRows.Length )
					{
						for ( int i = 0; i < dtLineItems.Columns.Count; i++ )
						{
							object oValue = aCurrentRows[nIndex - 1][i];
							aCurrentRows[nIndex - 1][i] = aCurrentRows[nIndex][i];
							aCurrentRows[nIndex][i] = oValue;
						}
						for ( int i = 0; i < aCurrentRows.Length; i++ )
						{
							aCurrentRows[i]["POSITION"] = i + 1;
						}
						grdMain.DataBind();
					}
					break;
				}
				case "MoveDown":
				{
					DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					int nIndex = Sql.ToInteger(e.CommandArgument);
					if ( dtLineItems != null && (nIndex + 2) < aCurrentRows.Length )
					{
						for ( int i = 0; i < dtLineItems.Columns.Count; i++ )
						{
							object oValue = aCurrentRows[nIndex + 1][i];
							aCurrentRows[nIndex + 1][i] = aCurrentRows[nIndex][i];
							aCurrentRows[nIndex][i] = oValue;
						}
						for ( int i = 0; i < aCurrentRows.Length; i++ )
						{
							aCurrentRows[i]["POSITION"] = i + 1;
						}
						grdMain.DataBind();
					}
					break;
				}
			}
		}

		// 08/15/2010 Paul.  Update the discount value when discount changes. 
		protected void DISCOUNT_ID_Changed(object sender, System.EventArgs e)
		{
			if ( dtLineItems != null )
			{
				GridViewRow gr = grdMain.Rows[grdMain.EditIndex];
				TextBox         txtQUANTITY            = gr.FindControl("QUANTITY"           ) as TextBox     ;
				TextBox         txtUNIT_PRICE          = gr.FindControl("UNIT_PRICE"         ) as TextBox     ;
				TextBox         txtEXTENDED_PRICE      = gr.FindControl("EXTENDED_PRICE"     ) as TextBox     ;
				DropDownList    lstDISCOUNT_ID         = gr.FindControl("DISCOUNT_ID"        ) as DropDownList;
				TextBox         txtDISCOUNT_PRICE      = gr.FindControl("DISCOUNT_PRICE"     ) as TextBox     ;
				// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
				DropDownList    lstPRICING_FORMULA     = gr.FindControl("PRICING_FORMULA"    ) as DropDownList;
				TextBox         txtPRICING_FACTOR      = gr.FindControl("PRICING_FACTOR"     ) as TextBox     ;

				// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
				Decimal nQUANTITY        = Sql.ToDecimal(txtQUANTITY      .Text         );
				Decimal dUNIT_PRICE      = Sql.ToDecimal(txtUNIT_PRICE    .Text         );
				Guid    gDISCOUNT_ID     = Sql.ToGuid   (lstDISCOUNT_ID   .SelectedValue);
				Decimal dDISCOUNT_VALUE  = Sql.ToDecimal(txtDISCOUNT_PRICE.Text         );
				string  sDISCOUNT_NAME   = String.Empty;
				string  sPRICING_FORMULA = String.Empty;
				float   fPRICING_FACTOR  = 0;
				// 08/15/2010 Paul.  In this area, we use the UNIT_PRICE instead of list or cost. 
				OrderUtils.DiscountValue(gDISCOUNT_ID, dUNIT_PRICE, dUNIT_PRICE, ref dDISCOUNT_VALUE, ref sDISCOUNT_NAME, ref sPRICING_FORMULA, ref fPRICING_FACTOR);
				// 08/15/2010 Paul.  The value we store is the discount amount and not the end-price. 
				txtDISCOUNT_PRICE.Text = (nQUANTITY * dDISCOUNT_VALUE).ToString("0.00");
				txtEXTENDED_PRICE.Text = (nQUANTITY * dUNIT_PRICE    ).ToString("0.00");
				txtPRICING_FACTOR.Text = fPRICING_FACTOR.ToString();
				try
				{
					// 08/19/2010 Paul.  Check the list before assigning the value. 
					Utils.SetValue(lstPRICING_FORMULA, sPRICING_FORMULA);
				}
				catch
				{
				}
			}
			UpdateTotals();
		}

		// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
		protected void PRICING_FORMULA_Changed(object sender, System.EventArgs e)
		{
			if ( dtLineItems != null )
			{
				GridViewRow gr = grdMain.Rows[grdMain.EditIndex];
				TextBox         txtQUANTITY            = gr.FindControl("QUANTITY"           ) as TextBox     ;
				TextBox         txtUNIT_PRICE          = gr.FindControl("UNIT_PRICE"         ) as TextBox     ;
				TextBox         txtEXTENDED_PRICE      = gr.FindControl("EXTENDED_PRICE"     ) as TextBox     ;
				DropDownList    lstDISCOUNT_ID         = gr.FindControl("DISCOUNT_ID"        ) as DropDownList;
				TextBox         txtDISCOUNT_PRICE      = gr.FindControl("DISCOUNT_PRICE"     ) as TextBox     ;
				DropDownList    lstPRICING_FORMULA     = gr.FindControl("PRICING_FORMULA"    ) as DropDownList;
				TextBox         txtPRICING_FACTOR      = gr.FindControl("PRICING_FACTOR"     ) as TextBox     ;
				
				// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
				Decimal nQUANTITY        = Sql.ToDecimal(txtQUANTITY      .Text         );
				Decimal dUNIT_PRICE      = Sql.ToDecimal(txtUNIT_PRICE    .Text         );
				string  sPRICING_FORMULA = lstPRICING_FORMULA.SelectedValue;
				float   fPRICING_FACTOR  = Sql.ToFloat  (txtPRICING_FACTOR.Text         );
				Decimal dDISCOUNT_VALUE  = Decimal.Zero;
				OrderUtils.DiscountValue(sPRICING_FORMULA, fPRICING_FACTOR, dUNIT_PRICE, dUNIT_PRICE, ref dDISCOUNT_VALUE);
				txtDISCOUNT_PRICE.Text = (nQUANTITY * dDISCOUNT_VALUE).ToString("0.00");
				txtEXTENDED_PRICE.Text = (nQUANTITY * dUNIT_PRICE    ).ToString("0.00");
				lstDISCOUNT_ID.SelectedIndex = 0;
			}
			UpdateTotals();
		}

		protected void btnPRODUCT_OPTIONS_Clicked(object sender, System.EventArgs e)
		{
			if ( dtLineItems != null )
			{
				string sPRODUCTS = hidPRODUCTS.Value;
				string[] arrPRODUCTS = sPRODUCTS.Split(',');
				if ( arrPRODUCTS.Length > 0 )
				{
					int nEditIndex = grdMain.EditIndex;
					DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
					DataRow row = aCurrentRows[nEditIndex];
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 07/14/2010 Paul.  Use the Parent to determine if we should update the record and move to the next record. 
						Guid gPARENT_TEMPLATE_ID  = Guid.Empty;
						// 07/15/2010 Paul.  Always create a new group. 
						// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
						Guid gLINE_GROUP_ID       = Guid.NewGuid();
						foreach ( string sPRODUCT in arrPRODUCTS )
						{
							string sSQL;
							sSQL = "select *                  " + ControlChars.CrLf
							     + "  from vwPRODUCT_CATALOG  " + ControlChars.CrLf
							     + " where ID = @ID           " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID.  It is returned from the popup separated by a pipe. 
								string[] arrPRODUCT_PARENT = sPRODUCT.Split('|');
								Guid gPRODUCT_TEMPLATE_ID = Sql.ToGuid(arrPRODUCT_PARENT[0]);
								gPARENT_TEMPLATE_ID  = Guid.Empty;
								if ( arrPRODUCT_PARENT.Length > 1 )
									gPARENT_TEMPLATE_ID = Sql.ToGuid(arrPRODUCT_PARENT[1]);
								Sql.AddParameter(cmd, "@ID", gPRODUCT_TEMPLATE_ID);

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
										if ( row == null )
										{
											nEditIndex++;
											row = dtLineItems.NewRow();
											dtLineItems.Rows.InsertAt(row, nEditIndex);
											// 08/04/2010 Paul.  Update the position. 
											row["POSITION"] = NextPosition(dtLineItems);
										}
										row["ID"                 ] = DBNull.Value;
										row["NAME"               ] = rdr["NAME"               ];
										row["MFT_PART_NUM"       ] = rdr["MFT_PART_NUM"       ];
										row["VENDOR_PART_NUM"    ] = rdr["VENDOR_PART_NUM"    ];
										row["PRODUCT_TEMPLATE_ID"] = gPRODUCT_TEMPLATE_ID      ;
										row["PARENT_TEMPLATE_ID" ] = gPARENT_TEMPLATE_ID       ;
										// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
										row["LINE_GROUP_ID"      ] = gLINE_GROUP_ID            ;
										row["TAX_CLASS"          ] = rdr["TAX_CLASS"          ];
										// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
										row["TAXRATE_ID"         ] = rdr["TAXRATE_ID"         ];
										row["QUANTITY"           ] = 1                         ;
										row["COST_USDOLLAR"      ] = rdr["COST_USDOLLAR"      ];
										row["LIST_USDOLLAR"      ] = rdr["LIST_USDOLLAR"      ];
										row["UNIT_USDOLLAR"      ] = rdr["UNIT_USDOLLAR"      ];
										row["DISCOUNT_ID"        ] = DBNull.Value;
										row["DISCOUNT_NAME"      ] = DBNull.Value;
										row["DISCOUNT_PRICE"     ] = DBNull.Value;
										row["DISCOUNT_USDOLLAR"  ] = DBNull.Value;
										row["PRICING_FORMULA"    ] = DBNull.Value;
										row["PRICING_FACTOR"     ] = DBNull.Value;
										row["DESCRIPTION"        ] = rdr["DESCRIPTION"        ];
										// 07/11/2010 Paul.  Now we update the row record based on the currency. 
										// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
										row["EXTENDED_USDOLLAR"  ] = Sql.ToDecimal(row["QUANTITY"]) * Sql.ToDecimal(row["UNIT_USDOLLAR"]);
										row["COST_PRICE"         ] = C10n.ToCurrency(Sql.ToDecimal(row["COST_USDOLLAR"    ]));
										row["LIST_PRICE"         ] = C10n.ToCurrency(Sql.ToDecimal(row["LIST_USDOLLAR"    ]));
										row["UNIT_PRICE"         ] = C10n.ToCurrency(Sql.ToDecimal(row["UNIT_USDOLLAR"    ]));
										row["EXTENDED_PRICE"     ] = C10n.ToCurrency(Sql.ToDecimal(row["EXTENDED_USDOLLAR"]));

										row["OPPORTUNITY_TYPE"   ] = DBNull.Value;
										row["LEAD_SOURCE"        ] = DBNull.Value;
										row["DATE_CLOSED"        ] = DBNull.Value;
										row["NEXT_STEP"          ] = DBNull.Value;
										row["SALES_STAGE"        ] = DBNull.Value;
										row["PROBABILITY"        ] = DBNull.Value;
										row["TAX"                ] = DBNull.Value;
										row["TAX_USDOLLAR"       ] = DBNull.Value;
										// 07/11/2010 Paul.  Clear the row as this will be our clue to insert a new row. 
										row = null;
									}
								}
							}
						}
						// 07/11/2010 Paul.  If more than one product was inserted, then jump to the end of the list. 
						if ( arrPRODUCTS.Length > 1 || !Sql.IsEmptyGuid(gPARENT_TEMPLATE_ID) )
						{
							aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
							// 11/18/2010 Paul.  Reverse the name of this function. 
							if ( aCurrentRows.Length == 0 || IsLineItemNotEmpty(aCurrentRows[aCurrentRows.Length-1]) )
							{
								DataRow rowNew = dtLineItems.NewRow();
								dtLineItems.Rows.Add(rowNew);
								// 08/04/2010 Paul.  Update the position. 
								rowNew["POSITION"] = NextPosition(dtLineItems);
								aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
							}
							// 03/30/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
							grdMain.EditIndex = aCurrentRows.Length - 1;
						}
						ViewState["LineItems"] = dtLineItems;
						grdMain.DataSource = dtLineItems;
						grdMain.DataBind();
						UpdateTotals();
					}
				}
			}
		}

		protected void grdMain_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if ( (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit )
			{
				DropDownList lstDISCOUNT_ID = e.Row.FindControl("DISCOUNT_ID") as DropDownList;
				if ( lstDISCOUNT_ID != null )
				{
					// 08/15/2010 Paul.  Add an empty discount so that it is optional. 
					DataTable dtDISCOUNTS = SplendidCache.Discounts().Copy();
					dtDISCOUNTS.Rows.InsertAt(dtDISCOUNTS.NewRow(), 0);
					// 08/15/2010 Paul.  We only want to allow the selection of discounts.  Markups are only for default values. 
					DataView vwDISCOUNTS = new DataView(dtDISCOUNTS);
					vwDISCOUNTS.RowFilter = "PRICING_FORMULA in ('PercentageDiscount', 'FixedDiscount') or ID is null";
					lstDISCOUNT_ID.DataSource = vwDISCOUNTS;
				}
				// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
				DropDownList lstPRICING_FORMULA = e.Row.FindControl("PRICING_FORMULA") as DropDownList;
				if ( lstPRICING_FORMULA != null )
				{
					lstPRICING_FORMULA.Items.Clear();
					lstPRICING_FORMULA.Items.Add(String.Empty);
					lstPRICING_FORMULA.Items.Add(new ListItem(Sql.ToString(L10n.Term(".pricing_formula_dom.", "PercentageDiscount")), "PercentageDiscount"));
					lstPRICING_FORMULA.Items.Add(new ListItem(Sql.ToString(L10n.Term(".pricing_formula_dom.", "FixedDiscount"     )), "FixedDiscount"     ));
				}

				DropDownList lstOPPORTUNITY_TYPE = e.Row.FindControl("OPPORTUNITY_TYPE") as DropDownList;
				if ( lstOPPORTUNITY_TYPE != null )
				{
					lstOPPORTUNITY_TYPE.DataSource = SplendidCache.List("opportunity_type_dom");
				}
				DropDownList lstLEAD_SOURCE = e.Row.FindControl("LEAD_SOURCE") as DropDownList;
				if ( lstLEAD_SOURCE != null )
				{
					lstLEAD_SOURCE.DataSource = SplendidCache.List("lead_source_dom");
				}
				DropDownList lstSALES_STAGE = e.Row.FindControl("SALES_STAGE") as DropDownList;
				if ( lstSALES_STAGE != null )
				{
					lstSALES_STAGE.DataSource = SplendidCache.List("sales_stage_dom");
				}
				if ( bAjaxAutoComplete )
				{
					// <ajaxToolkit:AutoCompleteExtender ID="autoNAME" TargetControlID="NAME" ServiceMethod="ItemNameList" ServicePath="~/Products/ProductCatalog/AutoComplete.asmx" MinimumPrefixLength="2" CompletionInterval="250" EnableCaching="true" CompletionSetCount="12" runat="server" />
					AjaxControlToolkit.AutoCompleteExtender autoNAME = new AjaxControlToolkit.AutoCompleteExtender();
					e.Row.Cells[1].Controls.Add(autoNAME);
					autoNAME.ID                   = "autoNAME";
					autoNAME.TargetControlID      = "NAME";
					autoNAME.ServiceMethod        = "ItemNameList";
					autoNAME.ServicePath          = "~/Products/ProductCatalog/AutoComplete.asmx";
					autoNAME.MinimumPrefixLength  = 2;
					autoNAME.CompletionInterval   = 250;
					autoNAME.EnableCaching        = true;
					// 12/09/2010 Paul.  Provide a way to customize the AutoComplete.CompletionSetCount. 
					autoNAME.CompletionSetCount   = Crm.Config.CompletionSetCount();

					// <ajaxToolkit:AutoCompleteExtender ID="autoMFT_PART_NUM" TargetControlID="MFT_PART_NUM" ServiceMethod="ItemNumberList" ServicePath="~/Products/ProductCatalog/AutoComplete.asmx" MinimumPrefixLength="2" CompletionInterval="250" EnableCaching="true" CompletionSetCount="12" runat="server" />
					AjaxControlToolkit.AutoCompleteExtender autoMFT_PART_NUM = new AjaxControlToolkit.AutoCompleteExtender();
					e.Row.Cells[2].Controls.Add(autoMFT_PART_NUM);
					autoMFT_PART_NUM.ID                   = "autoMFT_PART_NUM";
					autoMFT_PART_NUM.TargetControlID      = "MFT_PART_NUM";
					autoMFT_PART_NUM.ServiceMethod        = "ItemNumberList";
					autoMFT_PART_NUM.ServicePath          = "~/Products/ProductCatalog/AutoComplete.asmx";
					autoMFT_PART_NUM.MinimumPrefixLength  = 2;
					autoMFT_PART_NUM.CompletionInterval   = 250;
					autoMFT_PART_NUM.EnableCaching        = true;
					// 12/09/2010 Paul.  Provide a way to customize the AutoComplete.CompletionSetCount. 
					autoMFT_PART_NUM.CompletionSetCount   = Crm.Config.CompletionSetCount();
				}
			}
		}

		protected void grdMain_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if ( e.Row.RowType == DataControlRowType.DataRow )
			{
				DropDownList lstDISCOUNT_ID = e.Row.FindControl("DISCOUNT_ID") as DropDownList;
				if ( lstDISCOUNT_ID != null )
				{
					try
					{
						// 08/19/2010 Paul.  Check the list before assigning the value. 
						Utils.SetValue(lstDISCOUNT_ID, Sql.ToString(DataBinder.Eval(e.Row.DataItem, "DISCOUNT_ID")) );
					}
					catch
					{
					}
				}
				DropDownList lstPRICING_FORMULA = e.Row.FindControl("PRICING_FORMULA") as DropDownList;
				if ( lstPRICING_FORMULA != null )
				{
					try
					{
						// 08/19/2010 Paul.  Check the list before assigning the value. 
						Utils.SetValue(lstPRICING_FORMULA, Sql.ToString(DataBinder.Eval(e.Row.DataItem, "PRICING_FORMULA")) );
					}
					catch
					{
					}
				}
				DropDownList lstOPPORTUNITY_TYPE = e.Row.FindControl("OPPORTUNITY_TYPE") as DropDownList;
				if ( lstOPPORTUNITY_TYPE != null )
				{
					try
					{
						Utils.SetValue(lstOPPORTUNITY_TYPE, Sql.ToString(DataBinder.Eval(e.Row.DataItem, "OPPORTUNITY_TYPE")) );
					}
					catch
					{
					}
				}
				DropDownList lstLEAD_SOURCE = e.Row.FindControl("LEAD_SOURCE") as DropDownList;
				if ( lstLEAD_SOURCE != null )
				{
					try
					{
						Utils.SetValue(lstLEAD_SOURCE, Sql.ToString(DataBinder.Eval(e.Row.DataItem, "LEAD_SOURCE")) );
					}
					catch
					{
					}
				}
				DropDownList lstSALES_STAGE = e.Row.FindControl("SALES_STAGE") as DropDownList;
				if ( lstSALES_STAGE != null )
				{
					try
					{
						Utils.SetValue(lstSALES_STAGE, Sql.ToString(DataBinder.Eval(e.Row.DataItem, "SALES_STAGE")) );
					}
					catch
					{
					}
				}
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
				// 07/15/2010 Paul.  When a parent item is deleted, also delete all remaining items in the group. 
				// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
				Guid gPRODUCT_TEMPLATE_ID = Sql.ToGuid(aCurrentRows[e.RowIndex]["PRODUCT_TEMPLATE_ID"]);
				Guid gLINE_GROUP_ID       = Sql.ToGuid(aCurrentRows[e.RowIndex]["LINE_GROUP_ID"      ]);
				foreach ( DataRow row in aCurrentRows )
				{
					// 07/15/2010 Paul.  If the Parent ID matches the Product ID being deleted, then it is a child item. 
					// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
					// 05/06/0211 Paul.  LINE_GROUP_ID will be empty for old data. 
					if ( Sql.ToGuid(row["PARENT_TEMPLATE_ID"]) == gPRODUCT_TEMPLATE_ID && Sql.ToGuid(row["LINE_GROUP_ID"]) == gLINE_GROUP_ID && !Sql.IsEmptyGuid(gLINE_GROUP_ID) )
						row.Delete();
				}
				aCurrentRows[e.RowIndex].Delete();
				
				aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 02/04/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
				// 11/18/2010 Paul.  Reverse the name of this function. 
				if ( aCurrentRows.Length == 0 || IsLineItemNotEmpty(aCurrentRows[aCurrentRows.Length-1]) )
				{
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					// 08/04/2010 Paul.  Update the position. 
					rowNew["POSITION"] = NextPosition(dtLineItems);
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				}
				UpdateTotals();

				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
			}
		}

		protected void grdMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			if ( dtLineItems != null )
			{
				GridViewRow gr = grdMain.Rows[e.RowIndex];
				HiddenField     txtLINE_ITEM_TYPE      = gr.FindControl("LINE_ITEM_TYPE"     ) as HiddenField ;
				TextBox         txtNAME                = gr.FindControl("NAME"               ) as TextBox     ;
				TextBox         txtMFT_PART_NUM        = gr.FindControl("MFT_PART_NUM"       ) as TextBox     ;
				HiddenField     txtVENDOR_PART_NUM     = gr.FindControl("VENDOR_PART_NUM"    ) as HiddenField ;
				HiddenField     txtPRODUCT_TEMPLATE_ID = gr.FindControl("PRODUCT_TEMPLATE_ID") as HiddenField ;
				// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
				HiddenField     txtPARENT_TEMPLATE_ID  = gr.FindControl("PARENT_TEMPLATE_ID" ) as HiddenField ;
				// 07/15/2010 Paul.  Add GROUP_ID for options management. 
				// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
				HiddenField     txtLINE_GROUP_ID       = gr.FindControl("LINE_GROUP_ID"      ) as HiddenField ;
				TextBox         txtQUANTITY            = gr.FindControl("QUANTITY"           ) as TextBox     ;
				TextBox         txtUNIT_PRICE          = gr.FindControl("UNIT_PRICE"         ) as TextBox     ;
				HiddenField     txtUNIT_USDOLLAR       = gr.FindControl("UNIT_USDOLLAR"      ) as HiddenField ;
				TextBox         txtEXTENDED_PRICE      = gr.FindControl("EXTENDED_PRICE"     ) as TextBox     ;
				HiddenField     txtEXTENDED_USDOLLAR   = gr.FindControl("EXTENDED_USDOLLAR"  ) as HiddenField ;
				DropDownList    lstDISCOUNT_ID         = gr.FindControl("DISCOUNT_ID"        ) as DropDownList;
				TextBox         txtDISCOUNT_PRICE      = gr.FindControl("DISCOUNT_PRICE"     ) as TextBox     ;
				HiddenField     txtDISCOUNT_USDOLLAR   = gr.FindControl("DISCOUNT_USDOLLAR"  ) as HiddenField ;
				// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
				DropDownList    lstPRICING_FORMULA     = gr.FindControl("PRICING_FORMULA"    ) as DropDownList;
				TextBox         txtPRICING_FACTOR      = gr.FindControl("PRICING_FACTOR"     ) as TextBox     ;
				TextBox         txtDESCRIPTION         = gr.FindControl("DESCRIPTION"        ) as TextBox     ;
				
				DropDownList   lstOPPORTUNITY_TYPE     = gr.FindControl("OPPORTUNITY_TYPE"   ) as DropDownList;
				DropDownList   lstLEAD_SOURCE          = gr.FindControl("LEAD_SOURCE"        ) as DropDownList;
				DatePicker     ctlDATE_CLOSED          = gr.FindControl("DATE_CLOSED"        ) as DatePicker  ;
				TextBox        txtNEXT_STEP            = gr.FindControl("NEXT_STEP"          ) as TextBox     ;
				DropDownList   lstSALES_STAGE          = gr.FindControl("SALES_STAGE"        ) as DropDownList;
				TextBox        txtPROBABILITY          = gr.FindControl("PROBABILITY"        ) as TextBox     ;

				//DataRow row = dtLineItems.Rows[e.RowIndex];
				// 12/07/2007 garf.  If there are deleted rows in the set, then the index will be wrong.  Make sure to use the current rowset. 
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				DataRow row = aCurrentRows[e.RowIndex];
				// 03/30/2007 Paul.  The text controls are empty.  Use the Request object to read the data. 
				if ( txtLINE_ITEM_TYPE != null ) row["LINE_ITEM_TYPE"] = txtLINE_ITEM_TYPE.Value;
				if ( txtDESCRIPTION    != null ) row["DESCRIPTION"   ] = txtDESCRIPTION   .Text;
				// 03/31/2007 Paul.  The US Dollar values are computed from the price and are only used when changing currencies. 
				if ( txtNAME                != null ) row["NAME"               ] =               txtNAME               .Text;
				if ( txtMFT_PART_NUM        != null ) row["MFT_PART_NUM"       ] =               txtMFT_PART_NUM       .Text;
				if ( txtVENDOR_PART_NUM     != null ) row["VENDOR_PART_NUM"    ] =               txtVENDOR_PART_NUM    .Value;
				if ( txtPRODUCT_TEMPLATE_ID != null ) row["PRODUCT_TEMPLATE_ID"] = Sql.ToGuid   (txtPRODUCT_TEMPLATE_ID.Value);
				// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
				if ( txtPARENT_TEMPLATE_ID  != null ) row["PARENT_TEMPLATE_ID" ] = Sql.ToGuid   (txtPARENT_TEMPLATE_ID .Value);
				// 07/11/2010 Paul.  Add GROUP_ID. 
				// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
				if ( txtLINE_GROUP_ID       != null ) row["LINE_GROUP_ID"      ] = Sql.ToGuid   (txtLINE_GROUP_ID      .Value);
				row["TAX_CLASS"          ] = DBNull.Value;
				row["TAXRATE_ID"         ] = DBNull.Value;
				row["TAX"                ] = DBNull.Value;
				row["TAX_USDOLLAR"       ] = DBNull.Value;
				// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
				if ( txtQUANTITY            != null ) row["QUANTITY"           ] = Sql.ToDecimal(txtQUANTITY           .Text);
				if ( txtUNIT_PRICE          != null ) row["UNIT_PRICE"         ] = Sql.ToDecimal(txtUNIT_PRICE         .Text);
				if ( txtDISCOUNT_PRICE      != null ) row["DISCOUNT_PRICE"     ] = Sql.ToDecimal(txtDISCOUNT_PRICE     .Text);
				if ( lstDISCOUNT_ID != null )
				{
					row["DISCOUNT_ID"  ] = Sql.ToGuid(lstDISCOUNT_ID.SelectedValue);
					row["DISCOUNT_NAME"] = lstDISCOUNT_ID.SelectedItem.Text;
				}
				else
				{
					row["DISCOUNT_ID"  ] = DBNull.Value;
					row["DISCOUNT_NAME"] = DBNull.Value;
				}
				if ( lstPRICING_FORMULA != null )
					row["PRICING_FORMULA"    ] = Sql.ToString(lstPRICING_FORMULA.SelectedValue);
				else
					row["PRICING_FORMULA"    ] = DBNull.Value;
				if ( txtPRICING_FACTOR != null )
					row["PRICING_FACTOR"     ] = Sql.ToFloat(txtPRICING_FACTOR.Text);
				else
					row["PRICING_FACTOR"     ] = DBNull.Value;
					
				row["UNIT_USDOLLAR"      ] = C10n.FromCurrency(Sql.ToDecimal(txtUNIT_PRICE    .Text));
				// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
				row["EXTENDED_PRICE"     ] = Sql.ToDecimal(row["QUANTITY"]) * Sql.ToDecimal(row["UNIT_PRICE"]);
				row["EXTENDED_USDOLLAR"  ] = C10n.FromCurrency(Sql.ToDecimal(row["EXTENDED_PRICE"]));
				row["DISCOUNT_USDOLLAR"  ] = C10n.FromCurrency(Sql.ToDecimal(row["DISCOUNT_PRICE"]));
				if ( !Sql.IsEmptyString(row["PRICING_FORMULA"]) )
				{
					string  sPRICING_FORMULA = Sql.ToString (row["PRICING_FORMULA"]);
					float   fPRICING_FACTOR  = Sql.ToFloat  (row["PRICING_FACTOR" ]);
					Decimal dEXTENDED_PRICE  = Sql.ToDecimal(row["EXTENDED_PRICE" ]);
					Decimal dDISCOUNT_VALUE = Decimal.Zero;
					OrderUtils.DiscountValue(sPRICING_FORMULA, fPRICING_FACTOR, dEXTENDED_PRICE, dEXTENDED_PRICE, ref dDISCOUNT_VALUE);
					row["DISCOUNT_PRICE"   ] = dDISCOUNT_VALUE;
					row["DISCOUNT_USDOLLAR"] = C10n.FromCurrency(dDISCOUNT_VALUE);
				}
				else
				{
					row["PRICING_FACTOR"     ] = DBNull.Value;
				}
				row["TAX"             ] = DBNull.Value;
				row["TAX_USDOLLAR"    ] = DBNull.Value;
				if ( lstOPPORTUNITY_TYPE != null ) row["OPPORTUNITY_TYPE"] = lstOPPORTUNITY_TYPE.SelectedValue;
				if ( lstLEAD_SOURCE      != null ) row["LEAD_SOURCE"     ] = lstLEAD_SOURCE     .SelectedValue;
				if ( ctlDATE_CLOSED      != null ) row["DATE_CLOSED"     ] = ctlDATE_CLOSED     .Value        ;
				if ( txtNEXT_STEP        != null ) row["NEXT_STEP"       ] = txtNEXT_STEP       .Text         ;
				if ( lstSALES_STAGE      != null ) row["SALES_STAGE"     ] = lstSALES_STAGE     .SelectedValue;
				if ( txtPROBABILITY      != null ) row["PROBABILITY"     ] = Sql.ToFloat(txtPROBABILITY.Text) ;

				// 12/07/2007 Paul.  aCurrentRows is defined above. 
				//DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 03/30/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
				// 11/18/2010 Paul.  Reverse the name of this function. 
				if ( aCurrentRows.Length == 0 || IsLineItemNotEmpty(aCurrentRows[aCurrentRows.Length-1]) )
				{
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					// 08/04/2010 Paul.  Update the position. 
					rowNew["POSITION"] = NextPosition(dtLineItems);
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				}

				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 03/30/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
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
				// 07/14/2010 Paul.  We need to clear any temporary data. 
				DataRow row = aCurrentRows[grdMain.EditIndex];
				row["NAME"               ] = DBNull.Value;
				row["LINE_ITEM_TYPE"     ] = DBNull.Value;
				row["MFT_PART_NUM"       ] = DBNull.Value;
				row["VENDOR_PART_NUM"    ] = DBNull.Value;
				row["PRODUCT_TEMPLATE_ID"] = DBNull.Value;
				row["PARENT_TEMPLATE_ID" ] = DBNull.Value;
				// 07/15/2010 Paul.  Add GROUP_ID for options management. 
				// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
				row["LINE_GROUP_ID"      ] = DBNull.Value;
				row["TAX_CLASS"          ] = DBNull.Value;
				// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
				row["TAXRATE_ID"         ] = DBNull.Value;
				row["TAX"                ] = DBNull.Value;
				row["TAX_USDOLLAR"       ] = DBNull.Value;
				row["QUANTITY"           ] = DBNull.Value;
				row["COST_PRICE"         ] = DBNull.Value;
				row["COST_USDOLLAR"      ] = DBNull.Value;
				row["LIST_PRICE"         ] = DBNull.Value;
				row["LIST_USDOLLAR"      ] = DBNull.Value;
				row["UNIT_PRICE"         ] = DBNull.Value;
				row["UNIT_USDOLLAR"      ] = DBNull.Value;
				row["EXTENDED_PRICE"     ] = DBNull.Value;
				row["EXTENDED_USDOLLAR"  ] = DBNull.Value;
				row["DISCOUNT_ID"        ] = DBNull.Value;
				row["DISCOUNT_NAME"      ] = DBNull.Value;
				row["DISCOUNT_PRICE"     ] = DBNull.Value;
				row["DISCOUNT_USDOLLAR"  ] = DBNull.Value;
				row["PRICING_FORMULA"    ] = DBNull.Value;
				row["PRICING_FACTOR"     ] = DBNull.Value;
				row["DESCRIPTION"        ] = DBNull.Value;

				row["OPPORTUNITY_TYPE"   ] = DBNull.Value;
				row["LEAD_SOURCE"        ] = DBNull.Value;
				row["DATE_CLOSED"        ] = DBNull.Value;
				row["NEXT_STEP"          ] = DBNull.Value;
				row["SALES_STAGE"        ] = DBNull.Value;
				row["PROBABILITY"        ] = DBNull.Value;
				grdMain.DataBind();
				UpdateTotals();
			}
		}

		protected void CURRENCY_ID_Changed(object sender, System.EventArgs e)
		{
			// 03/31/2007 Paul.  When the currency changes, use the default exchange rate. 
			Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
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

			DISCOUNT.Text = C10n.ToCurrency(Sql.ToDecimal(DISCOUNT_USDOLLAR.Value)).ToString("0.00");
			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					row["UNIT_PRICE"    ] = C10n.ToCurrency(Sql.ToDecimal(row["UNIT_USDOLLAR"    ]));
					row["EXTENDED_PRICE"] = C10n.ToCurrency(Sql.ToDecimal(row["EXTENDED_USDOLLAR"]));
					row["DISCOUNT_PRICE"] = C10n.ToCurrency(Sql.ToDecimal(row["DISCOUNT_USDOLLAR"]));
				}
			}
			grdMain.DataBind();

			UpdateTotals();
		}

		// 01/05/2007 Paul.  We need to update the totals before saving. 
		public void UpdateTotals()
		{
			Double dSUBTOTAL = 0.0;
			// 08/13/2010 Paul.  Discount is now computed per line item. 
			Double dDISCOUNT = 0.0;
			Double dTOTAL    = 0.0;

			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
					// 11/18/2010 Paul.  We do not need to check the DISCOUNT_ID as a discount cannot stand-alone. 
					if ( !Sql.IsEmptyString(row["NAME"]) || !Sql.IsEmptyGuid(row["PRODUCT_TEMPLATE_ID"]) )
					{
						string  sLINE_ITEM_TYPE = Sql.ToString (row["LINE_ITEM_TYPE"]);
						// 02/10/2011 Paul.  Stop converting the Quantity to an integer. 
						float   nQUANTITY       = Sql.ToFloat  (row["QUANTITY"      ]);
						Double  dUNIT_PRICE     = Sql.ToDouble (row["UNIT_PRICE"    ]);
						Double  dDISCOUNT_PRICE = Sql.ToDouble (row["DISCOUNT_PRICE"]);
						dSUBTOTAL += dUNIT_PRICE * nQUANTITY;
						// 08/13/2010 Paul.  Discount is now computed per line item. 
						// 08/15/2010 Paul.  Discount already includes quantity. 
						dDISCOUNT += dDISCOUNT_PRICE;
					}
				}
			}
			dTOTAL = dSUBTOTAL - dDISCOUNT;
			SUBTOTAL.Text = Convert.ToDecimal(dSUBTOTAL).ToString("c");
			DISCOUNT.Text = Convert.ToDecimal(dDISCOUNT).ToString("0.00");
			TOTAL   .Text = Convert.ToDecimal(dTOTAL   ).ToString("c");
			// 03/31/2007 Paul.  We are using UNIT_PRICE, so the value will need to be converted to USD before stored in hidden fields. 
			DISCOUNT_USDOLLAR.Value = C10n.FromCurrency(Convert.ToDecimal(dDISCOUNT)).ToString("0.000");
		}
		#endregion

		// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
		public void LoadLineItems(Guid gID, Guid gDuplicateID, IDbConnection con, DataRow rdr)
		{
			if ( !IsPostBack )
			{
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

				CURRENCY_ID.DataSource = SplendidCache.Currencies();
				CURRENCY_ID.DataBind();
				try
				{
					// 03/15/2007 Paul.  Set the default currency to the current user currency. 
					// 08/19/2010 Paul.  Check the list before assigning the value. 
					Utils.SetValue(CURRENCY_ID, C10n.ID.ToString());
				}
				catch
				{
				}
				foreach ( DataControlField col in grdMain.Columns )
				{
					if ( !Sql.IsEmptyString(col.HeaderText) )
					{
						col.HeaderText = L10n.Term(m_sMODULE + col.HeaderText);
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
				if ( Sql.IsEmptyString(m_sMODULE) )
					throw(new Exception("EditLineItemsView: MODULE is undefined."));
				
				if ( (!Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID)) && (con != null) && (rdr != null) )
				{
					try
					{
						new DynamicControl(this, "CURRENCY_ID").SelectedValue = Sql.ToString(rdr["CURRENCY_ID"]);
					}
					catch
					{
					}
					string sSQL = String.Empty;
					sSQL = "select *                   " + ControlChars.CrLf
					     + "  from vwREVENUE_LINE_ITEMS" + ControlChars.CrLf
					     + " where 1 = 1               " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						if ( !Sql.IsEmptyGuid(gDuplicateID) )
						{
							Sql.AppendParameter(cmd, gDuplicateID, "OPPORTUNITY_ID", false);
						}
						else
						{
							Sql.AppendParameter(cmd, gID, "OPPORTUNITY_ID", false);
						}
						cmd.CommandText += " order by POSITION asc" + ControlChars.CrLf;

						if ( bDebug )
							RegisterClientScriptBlock("vwREVENUE_LINE_ITEMS", Sql.ClientScriptBlock(cmd));

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
							// 08/04/2010 Paul.  Update the position. 
							rowNew["POSITION"] = NextPosition(dtLineItems);

							ViewState["LineItems"] = dtLineItems;
							grdMain.DataSource = dtLineItems;
							// 03/27/2007 Paul.  Start with last line enabled for editing. 
							grdMain.EditIndex = dtLineItems.Rows.Count - 1;
							grdMain.DataBind();
							// 08/08/2015 Paul.  SUBTOTAL and DISCOUNT are not in the Opportunities table. 
							UpdateTotals();
						}
					}
				}
				else
				{
					dtLineItems = new DataTable();
					DataColumn colID                  = new DataColumn("ID"                 , Type.GetType("System.Guid"   ));
					DataColumn colLINE_GROUP_ID       = new DataColumn("LINE_GROUP_ID"      , Type.GetType("System.Guid"   ));
					DataColumn colLINE_ITEM_TYPE      = new DataColumn("LINE_ITEM_TYPE"     , Type.GetType("System.String" ));
					DataColumn colPOSITION            = new DataColumn("POSITION"           , Type.GetType("System.Int32"  ));
					DataColumn colNAME                = new DataColumn("NAME"               , Type.GetType("System.String" ));
					DataColumn colMFT_PART_NUM        = new DataColumn("MFT_PART_NUM"       , Type.GetType("System.String" ));
					DataColumn colVENDOR_PART_NUM     = new DataColumn("VENDOR_PART_NUM"    , Type.GetType("System.String" ));
					DataColumn colPRODUCT_TEMPLATE_ID = new DataColumn("PRODUCT_TEMPLATE_ID", Type.GetType("System.Guid"   ));
					// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
					DataColumn colPARENT_TEMPLATE_ID  = new DataColumn("PARENT_TEMPLATE_ID" , Type.GetType("System.Guid"   ));
					// 07/11/2010 Paul.  Add GROUP_ID. 
					// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
					//DataColumn colGROUP_ID            = new DataColumn("GROUP_ID"           , Type.GetType("System.Guid"   ));
					DataColumn colTAX_CLASS           = new DataColumn("TAX_CLASS"          , Type.GetType("System.String" ));
					// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
					DataColumn colTAXRATE_ID          = new DataColumn("TAXRATE_ID"         , Type.GetType("System.Guid"   ));
					DataColumn colTAX                 = new DataColumn("TAX"                , Type.GetType("System.Decimal"));
					DataColumn colTAX_USDOLLAR        = new DataColumn("TAX_USDOLLAR"       , Type.GetType("System.Decimal"));
					// 10/01/2012 Paul.  Quantity must be a decimal, otherwise it will round to an integer value. 
					DataColumn colQUANTITY            = new DataColumn("QUANTITY"           , Type.GetType("System.Decimal"));
					DataColumn colCOST_PRICE          = new DataColumn("COST_PRICE"         , Type.GetType("System.Decimal"));
					DataColumn colCOST_USDOLLAR       = new DataColumn("COST_USDOLLAR"      , Type.GetType("System.Decimal"));
					DataColumn colLIST_PRICE          = new DataColumn("LIST_PRICE"         , Type.GetType("System.Decimal"));
					DataColumn colLIST_USDOLLAR       = new DataColumn("LIST_USDOLLAR"      , Type.GetType("System.Decimal"));
					DataColumn colUNIT_PRICE          = new DataColumn("UNIT_PRICE"         , Type.GetType("System.Decimal"));
					DataColumn colUNIT_USDOLLAR       = new DataColumn("UNIT_USDOLLAR"      , Type.GetType("System.Decimal"));
					DataColumn colEXTENDED_PRICE      = new DataColumn("EXTENDED_PRICE"     , Type.GetType("System.Decimal"));
					DataColumn colEXTENDED_USDOLLAR   = new DataColumn("EXTENDED_USDOLLAR"  , Type.GetType("System.Decimal"));
					// 08/13/2010 Paul.  Add discount fields. 
					DataColumn colDISCOUNT_ID         = new DataColumn("DISCOUNT_ID"        , Type.GetType("System.Guid"   ));
					DataColumn colDISCOUNT_NAME       = new DataColumn("DISCOUNT_NAME"      , Type.GetType("System.String" ));
					DataColumn colDISCOUNT_PRICE      = new DataColumn("DISCOUNT_PRICE"     , Type.GetType("System.Decimal"));
					DataColumn colDISCOUNT_USDOLLAR   = new DataColumn("DISCOUNT_USDOLLAR"  , Type.GetType("System.Decimal"));
					DataColumn colPRICING_FORMULA     = new DataColumn("PRICING_FORMULA"    , Type.GetType("System.String" ));
					DataColumn colPRICING_FACTOR      = new DataColumn("PRICING_FACTOR"     , Type.GetType("System.Double" ));
					DataColumn colDESCRIPTION         = new DataColumn("DESCRIPTION"        , Type.GetType("System.String" ));
					
					DataColumn colOPPORTUNITY_TYPE    = new DataColumn("OPPORTUNITY_TYPE"   , Type.GetType("System.String"  ));
					DataColumn colLEAD_SOURCE         = new DataColumn("LEAD_SOURCE"        , Type.GetType("System.String"  ));
					DataColumn colDATE_CLOSED         = new DataColumn("DATE_CLOSED"        , Type.GetType("System.DateTime"));
					DataColumn colNEXT_STEP           = new DataColumn("NEXT_STEP"          , Type.GetType("System.String"  ));
					DataColumn colSALES_STAGE         = new DataColumn("SALES_STAGE"        , Type.GetType("System.String"  ));
					DataColumn colPROBABILITY         = new DataColumn("PROBABILITY"        , Type.GetType("System.Single"  ));
					
					dtLineItems.Columns.Add(colID                 );
					dtLineItems.Columns.Add(colLINE_GROUP_ID      );
					dtLineItems.Columns.Add(colLINE_ITEM_TYPE     );
					dtLineItems.Columns.Add(colPOSITION           );
					dtLineItems.Columns.Add(colNAME               );
					dtLineItems.Columns.Add(colMFT_PART_NUM       );
					dtLineItems.Columns.Add(colVENDOR_PART_NUM    );
					dtLineItems.Columns.Add(colPRODUCT_TEMPLATE_ID);
					// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
					dtLineItems.Columns.Add(colPARENT_TEMPLATE_ID );
					// 07/15/2010 Paul.  Add GROUP_ID for options management. 
					// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
					//dtLineItems.Columns.Add(colGROUP_ID           );
					dtLineItems.Columns.Add(colTAX_CLASS          );
					dtLineItems.Columns.Add(colTAX                );
					dtLineItems.Columns.Add(colTAX_USDOLLAR       );
					// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
					dtLineItems.Columns.Add(colTAXRATE_ID         );
					dtLineItems.Columns.Add(colQUANTITY           );
					dtLineItems.Columns.Add(colCOST_PRICE         );
					dtLineItems.Columns.Add(colCOST_USDOLLAR      );
					dtLineItems.Columns.Add(colLIST_PRICE         );
					dtLineItems.Columns.Add(colLIST_USDOLLAR      );
					dtLineItems.Columns.Add(colUNIT_PRICE         );
					dtLineItems.Columns.Add(colUNIT_USDOLLAR      );
					dtLineItems.Columns.Add(colEXTENDED_PRICE     );
					dtLineItems.Columns.Add(colEXTENDED_USDOLLAR  );
					dtLineItems.Columns.Add(colDISCOUNT_ID        );
					dtLineItems.Columns.Add(colDISCOUNT_NAME      );
					dtLineItems.Columns.Add(colDISCOUNT_PRICE     );
					dtLineItems.Columns.Add(colDISCOUNT_USDOLLAR  );
					dtLineItems.Columns.Add(colPRICING_FORMULA    );
					dtLineItems.Columns.Add(colPRICING_FACTOR     );
					dtLineItems.Columns.Add(colDESCRIPTION        );

					dtLineItems.Columns.Add(colOPPORTUNITY_TYPE   );
					dtLineItems.Columns.Add(colLEAD_SOURCE        );
					dtLineItems.Columns.Add(colDATE_CLOSED        );
					dtLineItems.Columns.Add(colNEXT_STEP          );
					dtLineItems.Columns.Add(colSALES_STAGE        );
					dtLineItems.Columns.Add(colPROBABILITY        );

					// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					// 08/04/2010 Paul.  Update the position. 
					rowNew["POSITION"] = 0;

					ViewState["LineItems"] = dtLineItems;
					grdMain.DataSource = dtLineItems;
					// 02/03/2007 Paul.  Start with last line enabled for editing. 
					grdMain.EditIndex = dtLineItems.Rows.Count - 1;
					grdMain.DataBind();

					UpdateTotals();
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( IsPostBack )
			{
				dtLineItems = ViewState["LineItems"] as DataTable;
				grdMain.DataSource = dtLineItems;
				// 03/31/2007 Paul.  Don't bind the grid, otherwise edits will be lost. 
				//grdMain.DataBind();
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
			// 07/11/2010 Paul.  Options allow multi-select. 
			bEnableOptions = Sql.ToBoolean(Application["CONFIG.ProductCatalog.EnableOptions"]);
			
			// 03/06/2016 Paul.  Change to Opportunities to make it easier to support HTML5. 
			m_sMODULE = "Opportunities";
			// 05/06/2010 Paul.  Move the ajax refence code to Page_Load as it only needs to be called once. 
			// 09/27/2010 Paul.  Move the AJAX detection logic to OnInit so that it will get called before LoadLineItems. 
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
				ServiceReference svc = new ServiceReference("~/Products/ProductCatalog/AutoComplete.asmx");
				ScriptReference  scr = new ScriptReference ("~/Products/ProductCatalog/AutoComplete.js"  );
				if ( !mgrAjax.Services.Contains(svc) )
					mgrAjax.Services.Add(svc);
				if ( !mgrAjax.Scripts.Contains(scr) )
					mgrAjax.Scripts.Add(scr);
			}
		}
		#endregion
	}
}

