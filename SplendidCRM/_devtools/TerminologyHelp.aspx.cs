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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI;
using System.Diagnostics;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for TerminologyHelp.
	/// </summary>
	public class TerminologyHelp : System.Web.UI.Page
	{
		protected void DumpTerminology(StringBuilder sb, IDbCommand cmd, string sProcedureName, int nNAME_MaxLength)
		{
			using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
			{
				int nCount = 0;
				while ( rdr.Read() )
				{
					// 04/29/2006 Paul.  DB2 is having a heap error SQL0954C.  
					// Increase the size of the heap and decrease the size of the procedure. 
					// db2 => connect to splendid
					// 1)	update db cfg for splendid using applheapsz 1024
					if ( nCount > 250 )
					{
						// 01/27/2007 Paul.  We have had one report of a Splendid Configuration Wizard timeout,
						// so break the terminology into transactions just as we do for Oracle and DB2. 
						sb.AppendLine("GO");
						sb.AppendLine("/* -- #if Oracle");
						sb.AppendLine("	COMMIT WORK;");
						sb.AppendLine("END;");
						sb.AppendLine("/");
						
						sb.AppendLine();
						sb.AppendLine("BEGIN");
						sb.AppendLine("-- #endif Oracle */");
						nCount = 0;
					}
					nCount++;
					sb.Append("exec dbo." + sProcedureName + " ");
					for ( int nColumn=0 ; nColumn < rdr.FieldCount ; nColumn++ )
					{
						if ( nColumn > 0 )
							sb.Append(", ");
						if ( rdr.IsDBNull(nColumn) )
							sb.Append("null");
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Boolean" ) ) sb.Append(rdr.GetBoolean (nColumn) ? "1" : "0" );
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Single"  ) ) sb.Append(rdr.GetDouble  (nColumn).ToString());
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Double"  ) ) sb.Append(rdr.GetDouble  (nColumn).ToString());
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int16"   ) ) sb.Append(rdr.GetInt16   (nColumn).ToString());
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int32"   ) ) sb.Append(rdr.GetInt32   (nColumn).ToString());
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int64"   ) ) sb.Append(rdr.GetInt64   (nColumn).ToString());
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Decimal" ) ) sb.Append(rdr.GetDecimal (nColumn).ToString());
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.DateTime") ) sb.Append("\'" + rdr.GetDateTime(nColumn).ToString("yyyy-MM-dd HH:mm:ss") + "\'");
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Guid"    ) ) sb.Append("\'" + rdr.GetGuid  (nColumn).ToString().ToUpper() + "\'");
						// 05/19/2008 Paul.  Unicode strings must be marked as such, otherwise unicode will go in as ???.
						// http://www.microsoft.com/globaldev/DrIntl/columns/001/default.mspx#E6B
						else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.String"  ) ) sb.Append("N\'" + rdr.GetString(nColumn).Replace("\'", "\'\'") + "\'");
						else sb.Append("null");
						// 11/21/2005 Paul.  Align the name field. 
						if ( nColumn == 0 )
						{
							string sNAME = rdr.GetString(nColumn);
							if ( nNAME_MaxLength - sNAME.Length > 0 )
								sb.Append(Strings.Space(nNAME_MaxLength - sNAME.Length));
						}
					}
					sb.AppendLine(";");
				}
				if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
				{
					sb.AppendLine("/");
					sb.AppendLine();
				}
				if ( Sql.IsSQLServer(cmd) )
				{
					sb.AppendLine("GO");
					sb.AppendLine();
				}
			}
		}

		protected string DumpAllTerms(IDbConnection con, string sLANG)
		{
			StringBuilder sb = new StringBuilder();
			if ( Sql.IsOracle(con) )
			{
				sb.AppendLine("ALTER SESSION SET NLS_DATE_FORMAT='YYYY-MM-DD HH24:MI:SS';");
				sb.AppendLine("BEGIN");
			}
			else
			{
				sb.AppendLine("/* -- #if IBM_DB2");
				sb.AppendLine("call dbo.spSqlDropProcedure('spTERMINOLOGY_HELP_Defaults_" + sLANG.Replace("-", "_") + "')");
				sb.AppendLine("/");
				sb.AppendLine();
				sb.AppendLine("Create Procedure dbo.spTERMINOLOGY_HELP_Defaults_" + sLANG.Replace("-", "_") + "()");
				sb.AppendLine("language sql");
				sb.AppendLine("  begin");
				sb.AppendLine("-- #endif IBM_DB2 */");
				sb.AppendLine();
				
				sb.AppendLine("/* -- #if Oracle");
				sb.AppendLine("BEGIN");
				sb.AppendLine("-- #endif Oracle */");
				sb.AppendLine();
				
				sb.AppendLine("-- Terminology generated from database [" + con.Database + "] on " + DateTime.Now.ToString() + ".");
				sb.AppendLine("print 'TERMINOLOGY_HELP " + sLANG + "';");
				sb.AppendLine("GO");
				sb.AppendLine();
				sb.AppendLine("set nocount on;");
				sb.AppendLine("GO");
				sb.AppendLine();
			}
			using ( IDbCommand cmd = con.CreateCommand() )
			{
				string sSQL;
				sSQL = "select NAME               " + ControlChars.CrLf
				     + "     , LCID               " + ControlChars.CrLf
				     + "     , ACTIVE             " + ControlChars.CrLf
				     + "     , NATIVE_NAME        " + ControlChars.CrLf
				     + "     , DISPLAY_NAME       " + ControlChars.CrLf
				     + "  from LANGUAGES          " + ControlChars.CrLf
				     + " where lower(NAME) = @NAME" + ControlChars.CrLf;
				cmd.CommandText = sSQL;
				// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
				Sql.AddParameter(cmd, "@NAME", sLANG.ToLower());
				DumpTerminology(sb, cmd, "spLANGUAGES_InsertOnly", sLANG.Length);

				sSQL = "select max(len(NAME))     " + ControlChars.CrLf
				     + "  from TERMINOLOGY_HELP   " + ControlChars.CrLf
				     + " where lower(LANG) = @LANG" + ControlChars.CrLf;
				cmd.CommandText = sSQL;
				cmd.Parameters.Clear();
				// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
				Sql.AddParameter(cmd, "@LANG", sLANG.ToLower());
				int nNAME_MaxLength = Sql.ToInteger(cmd.ExecuteScalar()) + 2;
				
				sSQL = "select NAME                      " + ControlChars.CrLf
				     + "     , LANG                      " + ControlChars.CrLf
				     + "     , MODULE_NAME               " + ControlChars.CrLf
				     + "     , DISPLAY_TEXT              " + ControlChars.CrLf
				     + "  from vwTERMINOLOGY_HELP        " + ControlChars.CrLf
				     + " where lower(LANG) = @LANG       " + ControlChars.CrLf
				     + " order by LANG, MODULE_NAME, NAME" + ControlChars.CrLf;
				cmd.CommandText = sSQL;
				cmd.Parameters.Clear();
				// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
				Sql.AddParameter(cmd, "@LANG", sLANG.ToLower());
				DumpTerminology(sb, cmd, "spTERMINOLOGY_HELP_InsertOnly", nNAME_MaxLength);
				sb.AppendLine();
				sb.AppendLine("set nocount off;");
				sb.AppendLine("GO");
				sb.AppendLine();
				if ( Sql.IsOracle(cmd) )
				{
					sb.AppendLine("	COMMIT WORK;");
					sb.AppendLine("END;");
					sb.AppendLine("/");
				}
				else
				{
					sb.AppendLine("/* -- #if Oracle");
					sb.AppendLine("	COMMIT WORK;");
					sb.AppendLine("END;");
					sb.AppendLine("/");
					sb.AppendLine("-- #endif Oracle */");

					sb.AppendLine();
					sb.AppendLine("/* -- #if IBM_DB2");
					sb.AppendLine("	commit;");
					sb.AppendLine("  end");
					sb.AppendLine("/");
					sb.AppendLine();

					sb.AppendLine("call dbo.spTERMINOLOGY_HELP_Defaults_" + sLANG.Replace("-", "_") + "()");
					sb.AppendLine("/");
					sb.AppendLine();

					sb.AppendLine("call dbo.spSqlDropProcedure('spTERMINOLOGY_HELP_Defaults_" + sLANG.Replace("-", "_") + "')");
					sb.AppendLine("/");
					sb.AppendLine("-- #endif IBM_DB2 */");
				}
			}
			return sb.ToString();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			// 05/24/2008 Paul.  Allow to be run remotely. 
			if ( !SplendidCRM.Security.IS_ADMIN )  // || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			try
			{
				string sLANG = Sql.ToString(Request.QueryString["Lang"]) ;
				if ( Sql.IsEmptyString(sLANG) )
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
								string sSQL;
								sSQL = "select *          " + ControlChars.CrLf
								     + "     , (select count(*) from vwTERMINOLOGY_HELP where LANG = vwLANGUAGES.NAME) as TERM_COUNT" + ControlChars.CrLf
								     + "  from vwLANGUAGES" + ControlChars.CrLf
								     + " order by NAME    " + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									Response.Write("<html><body><h1>Terminology Help</h1>" + ControlChars.CrLf);
									Response.Write("<table border=1 cellpadding=6 cellspacing=0>" + ControlChars.CrLf);
									while ( rdr.Read() )
									{
										Response.Write("<tr>" + ControlChars.CrLf);
										Response.Write("<td>" + Sql.ToString(rdr["DISPLAY_NAME"]) + "</td>" + ControlChars.CrLf);
										Response.Write("<td>" + Sql.ToString(rdr["NAME"        ]) + "</td>" + ControlChars.CrLf);
										Response.Write("<td>" + Sql.ToString(rdr["TERM_COUNT"  ]) + "</td>" + ControlChars.CrLf);
										Response.Write("<td><a href=\"TerminologyHelp.aspx?Lang=" + Sql.ToString(rdr["NAME"]) + "\">Export</a></td>" + ControlChars.CrLf);
										if ( Sql.ToString(rdr["NAME"]) != "en-US" )
											Response.Write("<td><a href=\"TranslateHelp.aspx?Lang="   + Sql.ToString(rdr["NAME"]) + "\">Translate</a></td>" + ControlChars.CrLf);
										Response.Write("</tr>" + ControlChars.CrLf);
									}
									Response.Write("</table>" + ControlChars.CrLf);
									Response.Write("</body></html>" + ControlChars.CrLf);
								}
							}
						}
					}
				}
				else
				{
					Response.Buffer = false;
					if ( String.Compare(sLANG, "all", true) != 0 )
					{
						Response.ContentType = "text/sql";
						// 08/06/2008 yxy21969.  Make sure to encode all URLs.
						// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
						Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, "TERMINOLOGY_HELP " + sLANG + ".3.sql"));
					}

					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						if ( String.Compare(sLANG, "all", true) == 0 )
						{
							string sSQL;
							sSQL = "select NAME       " + ControlChars.CrLf
							     + "  from vwLANGUAGES" + ControlChars.CrLf
							     + " order by NAME    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										foreach ( DataRow row in dt.Rows )
										{
											sLANG = Sql.ToString(row["NAME"]);
											Response.Write(sLANG + "<br/>");
											string sTERMINOLOGY = DumpAllTerms(con, sLANG);
											try
											{
												string sTerminologyPath = Server.MapPath("~/Terminology/TERMINOLOGY_HELP " + sLANG + ".3.sql");
												using ( StreamWriter stm = File.CreateText(sTerminologyPath) )
												{
													stm.Write(sTERMINOLOGY);
												}
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											}
										}
										Response.Write("***** Done<br/>");
									}
								}
							}
						}
						else
						{
							string sTERMINOLOGY = DumpAllTerms(con, sLANG);
							Response.Write(sTERMINOLOGY);
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

