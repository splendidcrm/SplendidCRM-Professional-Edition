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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI;
using System.Diagnostics;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for ExportAll.
	/// </summary>
	public class ExportAll : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			// 12/22/2007 Paul.  Allow an admin to dump all their data. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;
			Response.ContentType = "text/sql";
			// 08/06/2008 yxy21969.  Make sure to encode all URLs.
			// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
			Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, "SplendidCRM Dump.sql"));
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							string[] aTableNames = new String[]
							{ "ACCOUNTS"
							, "BUGS"
							, "CALLS"
							, "CAMPAIGNS"
							, "CASES"
							, "CONFIG"
							, "CONTACTS"
							, "CURRENCIES"
							, "CUSTOM_FIELDS"
							, "DOCUMENTS"
							, "dtproperties"
							, "EMAIL_TEMPLATES"
							, "EMAILS"
							, "FEEDS"
							, "FIELDS_META_DATA"
							, "FILES"
							, "IFRAMES"
							, "IMPORT_MAPS"
							, "LEADS"
							, "MEETINGS"
							, "NOTES"
							, "OPPORTUNITIES"
							, "PROJECT"
							, "PROJECT_TASK"
							, "PROSPECT_LISTS"
							, "PROSPECTS"
							, "RELEASES"
							, "ROLES"
							, "TASKS"
							, "TERMINOLOGY"
							, "TIMEZONES"
							, "USERS"
							, "USERS_LAST_IMPORT"
							, "VCALS"
							, "VERSIONS"
							, "ACCOUNTS_BUGS"
							, "ACCOUNTS_CASES"
							, "ACCOUNTS_CONTACTS"
							, "ACCOUNTS_OPPORTUNITIES"
							, "CALLS_CONTACTS"
							, "CALLS_LEADS"
							, "CALLS_USERS"
							, "CASES_BUGS"
							, "CONTACTS_BUGS"
							, "CONTACTS_CASES"
							, "DOCUMENT_REVISIONS"
							, "EMAIL_MARKETING"
							, "EMAILMAN"
							, "EMAILMAN_SENT"
							, "EMAILS_ACCOUNTS"
							, "EMAILS_CASES"
							, "EMAILS_CONTACTS"
							, "EMAILS_OPPORTUNITIES"
							, "EMAILS_USERS"
							, "MEETINGS_CONTACTS"
							, "MEETINGS_LEADS"
							, "MEETINGS_USERS"
							, "OPPORTUNITIES_CONTACTS"
							, "PROJECT_RELATION"
							, "PROSPECT_LIST_CAMPAIGNS"
							, "PROSPECT_LISTS_PROSPECTS"
							, "ROLES_MODULES"
							, "ROLES_USERS"
							, "TRACKER"
							, "USERS_FEEDS"
							};
							if ( !Sql.IsEmptyString(Request.QueryString["Table"]) )
							{
								// 12/11/2005 Paul.  Need to allow for multiple tables, comma separated. 
								// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
								aTableNames = Sql.ToString(Request.QueryString["Table"]).Replace(" ", "").Split(',');
							}
							string sLANG = Sql.ToString(Request.QueryString["Lang"]) ;
							foreach ( string sTableName in aTableNames )
							{
								cmd.CommandText = "select * from " + Sql.EscapeSQL(sTableName) + " where 1 = 1" + ControlChars.CrLf;
								if ( String.Compare(sTableName, "TERMINOLOGY", true) == 0 && !Sql.IsEmptyString(sLANG) )
								{
									Sql.AppendParameter(cmd, sLANG, "LANG");
								}
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									StringBuilder sb = new StringBuilder();
									if ( Sql.IsOracle(cmd) )
										Response.Write("ALTER SESSION SET NLS_DATE_FORMAT='YYYY-MM-DD HH24:MI:SS';" + ControlChars.CrLf);
									while ( rdr.Read() )
									{
										if ( sb.Length == 0 )
										{
											sb.Append("insert into " + sTableName + "(");
											for ( int nColumn=0 ; nColumn < rdr.FieldCount ; nColumn++ )
											{
												if ( nColumn > 0 )
													sb.Append(", ");
												/*
												// 10/15/2005 Paul.  Table columns have been renamed in SugarCRM 3.5. 
												if ( rdr.GetName(nColumn) == "NUMBER" )
												{
													if ( sTableName == "BUGS" )
														sb.Append("BUG_NUMBER");
													else if ( sTableName == "CASES" )
														sb.Append("CASE_NUMBER");
													else if ( sTableName == "TRACKER" )
														sb.Append("TRACKER_NUMBER");
												}
												else
												*/
												{
													sb.Append(rdr.GetName(nColumn));
												}
											}
											sb.AppendLine(")");
										}
										Response.Write(sb.ToString());
									
										Response.Write(Strings.Space(6+sTableName.Length) + "values(");
										for ( int nColumn=0 ; nColumn < rdr.FieldCount ; nColumn++ )
										{
											if ( nColumn > 0 )
												Response.Write(", ");
											if ( rdr.IsDBNull(nColumn) )
												Response.Write("null");
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Boolean" ) ) Response.Write(rdr.GetBoolean (nColumn) ? "1" : "0" );
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Single"  ) ) Response.Write(rdr.GetDouble  (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Double"  ) ) Response.Write(rdr.GetDouble  (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int16"   ) ) Response.Write(rdr.GetInt16   (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int32"   ) ) Response.Write(rdr.GetInt32   (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int64"   ) ) Response.Write(rdr.GetInt64   (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Decimal" ) ) Response.Write(rdr.GetDecimal (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.DateTime") ) Response.Write("\'" + rdr.GetDateTime(nColumn).ToString("yyyy-MM-dd HH:mm:ss") + "\'");
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Guid"    ) ) Response.Write("\'" + rdr.GetGuid  (nColumn).ToString().ToUpper() + "\'");
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.String"  ) ) Response.Write("\'" + rdr.GetString(nColumn).Replace("\'", "\'\'") + "\'");
											else Response.Write("null");
										}
										Response.Write(");" + ControlChars.CrLf);
									}
									if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
										Response.Write("/" + ControlChars.CrLf + ControlChars.CrLf);
									if ( Sql.IsSQLServer(cmd) )
										Response.Write("GO" + ControlChars.CrLf + ControlChars.CrLf);
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message + ControlChars.CrLf);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
