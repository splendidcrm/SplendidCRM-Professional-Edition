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

namespace SplendidCRM.Administration.ZipCodes
{
	public class ZipCode
	{
		public string  POSTALCODE;
		public string  CITY      ;
		public string  STATE     ;
		public string  COUNTRY   ;

		public ZipCode()
		{
			POSTALCODE = String.Empty;
			CITY       = String.Empty;
			STATE      = String.Empty;
			COUNTRY    = String.Empty;
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
		[WebMethod(EnableSession=true)]
		public ZipCode ZIPCODES_POSTALCODE_Get(string sNAME, string contextKey)
		{
			ZipCode item = new ZipCode();
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select ID        " + ControlChars.CrLf
					     + "     , NAME      " + ControlChars.CrLf
					     + "     , CITY      " + ControlChars.CrLf
					     + "     , STATE     " + ControlChars.CrLf
					     + "     , COUNTRY   " + ControlChars.CrLf
					     + "  from vwZIPCODES" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ZipCodes", "list");
						if ( !Sql.IsEmptyString(contextKey) )
						{
							if ( String.Compare(contextKey, "USA", true) == 0 || String.Compare(contextKey, "United States", true) == 0 )
								contextKey = "US";
							cmd.CommandText += "   and (COUNTRY is null or COUNTRY like @COUNTRY)" + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@COUNTRY", Sql.EscapeSQLLike(contextKey) + '%');
						}
						Sql.AppendParameter(cmd, sNAME, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "NAME");
						cmd.CommandText += " order by NAME" + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.POSTALCODE = Sql.ToString (rdr["NAME"   ]);
								item.CITY       = Sql.ToString (rdr["CITY"   ]);
								item.STATE      = Sql.ToString (rdr["STATE"  ]);
								item.COUNTRY    = Sql.ToString (rdr["COUNTRY"]);
							}
						}
					}
				}
				if ( Sql.IsEmptyString(item.POSTALCODE) && !Sql.IsEmptyString(sNAME) )
				{
					string sCULTURE = Sql.ToString (Session["USER_SETTINGS/CULTURE"]);
					L10N L10n = new L10N(sCULTURE);
					throw(new Exception(L10n.Term("ZipCodes.ERR_NOT_FOUND")));
				}
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return item;
		}

		[WebMethod(EnableSession=true)]
		public string[] ZIPCODES_POSTALCODE_List(string prefixText, int count, string contextKey)
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
					sSQL = "select distinct  " + ControlChars.CrLf
					     + "       NAME      " + ControlChars.CrLf
					     + "  from vwZIPCODES" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ZipCodes", "list");
						if ( !Sql.IsEmptyString(contextKey) )
						{
							if ( String.Compare(contextKey, "USA", true) == 0 || String.Compare(contextKey, "United States", true) == 0 )
								contextKey = "US";
							cmd.CommandText += "   and (COUNTRY is null or COUNTRY like @COUNTRY)" + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@COUNTRY", Sql.EscapeSQLLike(contextKey) + '%');
						}
						Sql.AppendParameter(cmd, prefixText, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "NAME");
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

		[WebMethod(EnableSession=true)]
		public string[] ZIPCODES_ADDRESS_POSTALCODE_List(string prefixText, int count, string contextKey)
		{
			return ZIPCODES_POSTALCODE_List(prefixText, count, contextKey);
		}

		[WebMethod(EnableSession=true)]
		public string[] ZIPCODES_ALT_ADDRESS_POSTALCODE_List(string prefixText, int count, string contextKey)
		{
			return ZIPCODES_POSTALCODE_List(prefixText, count, contextKey);
		}

		[WebMethod(EnableSession=true)]
		public string[] ZIPCODES_BILLING_ADDRESS_POSTALCODE_List(string prefixText, int count, string contextKey)
		{
			return ZIPCODES_POSTALCODE_List(prefixText, count, contextKey);
		}

		[WebMethod(EnableSession=true)]
		public string[] ZIPCODES_PRIMARY_ADDRESS_POSTALCODE_List(string prefixText, int count, string contextKey)
		{
			return ZIPCODES_POSTALCODE_List(prefixText, count, contextKey);
		}

		[WebMethod(EnableSession=true)]
		public string[] ZIPCODES_SHIPPING_ADDRESS_POSTALCODE_List(string prefixText, int count, string contextKey)
		{
			return ZIPCODES_POSTALCODE_List(prefixText, count, contextKey);
		}
	}
}

