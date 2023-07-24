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
using System.Collections;
using System.Web.Services;
using System.ComponentModel;
using SplendidCRM;

namespace SplendidCRM.Products.ProductCatalog
{
	public class LineItem
	{
		public Guid    ID                 ;
		public string  NAME               ;
		public string  MFT_PART_NUM       ;
		public string  VENDOR_PART_NUM    ;
		public string  TAX_CLASS          ;
		// 12/16/2013 Paul.  Allow each product to have a default tax rate. 
		public Guid    TAXRATE_ID         ;
		public Decimal COST_PRICE         ;
		public Decimal COST_USDOLLAR      ;
		public Decimal LIST_PRICE         ;
		public Decimal LIST_USDOLLAR      ;
		public Decimal UNIT_PRICE         ;
		public Decimal UNIT_USDOLLAR      ;
		// 05/13/2009 Paul.  Carry forward the description from product to quote, order and invoice. 
		public string  DESCRIPTION        ;

		public LineItem()
		{
			ID                  = Guid.Empty  ;
			NAME                = String.Empty;
			MFT_PART_NUM        = String.Empty;
			VENDOR_PART_NUM     = String.Empty;
			TAX_CLASS           = String.Empty;
			// 12/16/2013 Paul.  Allow each product to have a default tax rate. 
			TAXRATE_ID          = Guid.Empty  ;
			COST_PRICE          = Decimal.Zero;
			COST_USDOLLAR       = Decimal.Zero;
			LIST_PRICE          = Decimal.Zero;
			LIST_USDOLLAR       = Decimal.Zero;
			UNIT_PRICE          = Decimal.Zero;
			UNIT_USDOLLAR       = Decimal.Zero;
			DESCRIPTION         = String.Empty;
		}
	}

	/// <summary>
	/// Summary description for AutoComplete
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.Web.Script.Services.ScriptService]
	[ToolboxItem(false)]
	public class AutoComplete : System.Web.Services.WebService
	{
		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		[WebMethod(EnableSession=true)]
		public LineItem GetItemDetailsByNumber(Guid gCURRENCY_ID, string sMFT_PART_NUM)
		{
			LineItem item = new LineItem();
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 02/04/2007 Paul.  Use LIKE clause so that the user can abbreviate unique part numbers. 
					sSQL = "select *                " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						// 07/12/2010 Paul.  Allow fuzzy searching during AutoComplete. 
						Sql.AppendParameter(cmd, sMFT_PART_NUM, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "MFT_PART_NUM");
						// 07/02/2007 Paul.  Sort is important so that the first match is selected. 
						// 11/11/2008 Paul.  Sort must be applied to CommandText. 
						cmd.CommandText += " order by MFT_PART_NUM" + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.ID                  = Sql.ToGuid   (rdr["ID"                 ]);
								item.NAME                = Sql.ToString (rdr["NAME"               ]);
								item.MFT_PART_NUM        = Sql.ToString (rdr["MFT_PART_NUM"       ]);
								item.VENDOR_PART_NUM     = Sql.ToString (rdr["VENDOR_PART_NUM"    ]);
								item.TAX_CLASS           = Sql.ToString (rdr["TAX_CLASS"          ]);
								// 12/16/2013 Paul.  Allow each product to have a default tax rate. 
								item.TAXRATE_ID          = Sql.ToGuid   (rdr["TAXRATE_ID"         ]);
								item.COST_PRICE          = Sql.ToDecimal(rdr["COST_PRICE"         ]);
								item.COST_USDOLLAR       = Sql.ToDecimal(rdr["COST_USDOLLAR"      ]);
								item.LIST_PRICE          = Sql.ToDecimal(rdr["LIST_PRICE"         ]);
								item.LIST_USDOLLAR       = Sql.ToDecimal(rdr["LIST_USDOLLAR"      ]);
								item.UNIT_PRICE          = Sql.ToDecimal(rdr["UNIT_PRICE"         ]);
								item.UNIT_USDOLLAR       = Sql.ToDecimal(rdr["UNIT_USDOLLAR"      ]);
								// 05/13/2009 Paul.  Carry forward the description from product to quote, order and invoice. 
								item.DESCRIPTION         = Sql.ToString (rdr["DESCRIPTION"        ]);
								// 03/31/2007 Paul.  The price of the product may not be in the same currency as the order form. 
								// Make sure to convert to the specified currency. 
								if ( gCURRENCY_ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
								{
									// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
									Currency C10n = Currency.CreateCurrency(Application, gCURRENCY_ID);
									item.COST_PRICE          = C10n.ToCurrency(item.COST_USDOLLAR);
									item.LIST_PRICE          = C10n.ToCurrency(item.LIST_USDOLLAR);
									item.UNIT_PRICE          = C10n.ToCurrency(item.UNIT_USDOLLAR);
								}
							}
						}
					}
				}
				if ( Sql.IsEmptyGuid(item.ID) )
					throw(new Exception("Item not found"));
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return item;
		}

		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		[WebMethod(EnableSession=true)]
		public LineItem GetItemDetailsByName(Guid gCURRENCY_ID, string sNAME)
		{
			LineItem item = new LineItem();
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 02/04/2007 Paul.  Use LIKE clause so that the user can abbreviate unique part numbers. 
					sSQL = "select *                " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						// 07/12/2010 Paul.  Allow fuzzy searching during AutoComplete. 
						Sql.AppendParameter(cmd, sNAME, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "NAME");
						// 07/02/2007 Paul.  Sort is important so that the first match is selected. 
						// 11/11/2008 Paul.  Sort must be applied to CommandText. 
						cmd.CommandText += " order by NAME" + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.ID                  = Sql.ToGuid   (rdr["ID"                 ]);
								item.NAME                = Sql.ToString (rdr["NAME"               ]);
								item.MFT_PART_NUM        = Sql.ToString (rdr["MFT_PART_NUM"       ]);
								item.VENDOR_PART_NUM     = Sql.ToString (rdr["VENDOR_PART_NUM"    ]);
								item.TAX_CLASS           = Sql.ToString (rdr["TAX_CLASS"          ]);
								// 12/16/2013 Paul.  Allow each product to have a default tax rate. 
								item.TAXRATE_ID          = Sql.ToGuid   (rdr["TAXRATE_ID"         ]);
								item.COST_PRICE          = Sql.ToDecimal(rdr["COST_PRICE"         ]);
								item.COST_USDOLLAR       = Sql.ToDecimal(rdr["COST_USDOLLAR"      ]);
								item.LIST_PRICE          = Sql.ToDecimal(rdr["LIST_PRICE"         ]);
								item.LIST_USDOLLAR       = Sql.ToDecimal(rdr["LIST_USDOLLAR"      ]);
								item.UNIT_PRICE          = Sql.ToDecimal(rdr["UNIT_PRICE"         ]);
								item.UNIT_USDOLLAR       = Sql.ToDecimal(rdr["UNIT_USDOLLAR"      ]);
								// 05/13/2009 Paul.  Carry forward the description from product to quote, order and invoice. 
								item.DESCRIPTION         = Sql.ToString (rdr["DESCRIPTION"        ]);
								// 03/31/2007 Paul.  The price of the product may not be in the same currency as the order form. 
								// Make sure to convert to the specified currency. 
								if ( gCURRENCY_ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
								{
									// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
									Currency C10n = Currency.CreateCurrency(Application, gCURRENCY_ID);
									item.COST_PRICE          = C10n.ToCurrency(item.COST_USDOLLAR);
									item.LIST_PRICE          = C10n.ToCurrency(item.LIST_USDOLLAR);
									item.UNIT_PRICE          = C10n.ToCurrency(item.UNIT_USDOLLAR);
								}
							}
						}
					}
				}
				if ( Sql.IsEmptyGuid(item.ID) )
					throw(new Exception("Item not found"));
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return item;
		}

		// 03/03/2016 Paul.  HTML5 editor needs to get item  by ID. 
		[WebMethod(EnableSession=true)]
		public LineItem GetItemDetailsByID(Guid gCURRENCY_ID, string sID)
		{
			LineItem item = new LineItem();
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select *                " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						Sql.AppendParameter(cmd, Sql.ToGuid(sID), "ID");
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.ID                  = Sql.ToGuid   (rdr["ID"                 ]);
								item.NAME                = Sql.ToString (rdr["NAME"               ]);
								item.MFT_PART_NUM        = Sql.ToString (rdr["MFT_PART_NUM"       ]);
								item.VENDOR_PART_NUM     = Sql.ToString (rdr["VENDOR_PART_NUM"    ]);
								item.TAX_CLASS           = Sql.ToString (rdr["TAX_CLASS"          ]);
								item.TAXRATE_ID          = Sql.ToGuid   (rdr["TAXRATE_ID"         ]);
								item.COST_PRICE          = Sql.ToDecimal(rdr["COST_PRICE"         ]);
								item.COST_USDOLLAR       = Sql.ToDecimal(rdr["COST_USDOLLAR"      ]);
								item.LIST_PRICE          = Sql.ToDecimal(rdr["LIST_PRICE"         ]);
								item.LIST_USDOLLAR       = Sql.ToDecimal(rdr["LIST_USDOLLAR"      ]);
								item.UNIT_PRICE          = Sql.ToDecimal(rdr["UNIT_PRICE"         ]);
								item.UNIT_USDOLLAR       = Sql.ToDecimal(rdr["UNIT_USDOLLAR"      ]);
								item.DESCRIPTION         = Sql.ToString (rdr["DESCRIPTION"        ]);
								if ( gCURRENCY_ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
								{
									// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
									Currency C10n = Currency.CreateCurrency(Application, gCURRENCY_ID);
									item.COST_PRICE          = C10n.ToCurrency(item.COST_USDOLLAR);
									item.LIST_PRICE          = C10n.ToCurrency(item.LIST_USDOLLAR);
									item.UNIT_PRICE          = C10n.ToCurrency(item.UNIT_USDOLLAR);
								}
							}
						}
					}
				}
				if ( Sql.IsEmptyGuid(item.ID) )
					throw(new Exception("Item not found"));
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return item;
		}

		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		// 03/29/2007 Paul.  In order for AutoComplete to work, the parameter names must be "prefixText" and "count". 
		[WebMethod(EnableSession=true)]
		public string[] ItemNumberList(string prefixText, int count)
		{
			string[] arrItems = new string[0];
			try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					// 03/29/2007 Paul.  Use LIKE clause so that the user can abbreviate unique part numbers. 
					sSQL = "select MFT_PART_NUM     " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						// 07/12/2010 Paul.  Allow fuzzy searching during AutoComplete. 
						Sql.AppendParameter(cmd, prefixText, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "MFT_PART_NUM");
						// 11/11/2008 Paul.  Sort must be applied to CommandText. 
						cmd.CommandText += " order by MFT_PART_NUM" + ControlChars.CrLf;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(0, count, dt);
								arrItems = new string[dt.Rows.Count];
								for ( int i=0; i < dt.Rows.Count; i++ )
									arrItems[i] = Sql.ToString(dt.Rows[i]["MFT_PART_NUM"]);
							}
						}
					}
				}
			}
			catch
			{
			}
			return arrItems;
		}

		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		// 03/29/2007 Paul.  In order for AutoComplete to work, the parameter names must be "prefixText" and "count". 
		[WebMethod(EnableSession=true)]
		public string[] ItemNameList(string prefixText, int count)
		{
			string[] arrItems = new string[0];
			try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select NAME             " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						// 07/12/2010 Paul.  Allow fuzzy searching during AutoComplete. 
						Sql.AppendParameter(cmd, prefixText, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "NAME");
						// 11/11/2008 Paul.  Sort must be applied to CommandText. 
						cmd.CommandText += " order by NAME" + ControlChars.CrLf;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(0, count, dt);
								arrItems = new string[dt.Rows.Count];
								for ( int i=0; i < dt.Rows.Count; i++ )
									arrItems[i] = Sql.ToString(dt.Rows[i]["NAME"]);
							}
						}
					}
				}
			}
			catch
			{
			}
			return arrItems;
		}
	}
}

