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
	/// Summary description for TerminologyUsage.
	/// </summary>
	public class TerminologyUsage : System.Web.UI.Page
	{
		protected StringBuilder sb;

		void RecursiveReadAll(string strDirectory)
		{
			FileInfo objInfo = null;

			string[] arrFiles = Directory.GetFiles(strDirectory);
			for ( int i = 0 ; i < arrFiles.Length ; i++ )
			{
				objInfo = new FileInfo(arrFiles[i]);
				if ( (String.Compare(objInfo.Extension, ".cs", true) == 0 || String.Compare(objInfo.Extension, ".aspx", true) == 0 || String.Compare(objInfo.Extension, ".ascx", true) == 0 || String.Compare(objInfo.Extension, ".asmx", true) == 0 || String.Compare(objInfo.Extension, ".master", true) == 0 || String.Compare(objInfo.Extension, ".skin", true) == 0) && Response.IsClientConnected )
				{
					using (StreamReader rdr = objInfo.OpenText() )
					{
						sb.Append(rdr.ReadToEnd());
					}
				}
			}

			string[] arrDirectories = Directory.GetDirectories(strDirectory);
			for ( int i = 0 ; i < arrDirectories.Length ; i++ )
			{
				objInfo = new FileInfo(arrDirectories[i]);
				if ( (String.Compare(objInfo.Name, "_vti_cnf", true) != 0) && (String.Compare(objInfo.Name, "_sgbak", true) != 0) )
					RecursiveReadAll(objInfo.FullName);
			}
		}

		void Page_Load(object sender, System.EventArgs e)
		{
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;

			sb = new StringBuilder();
			RecursiveReadAll(Server.MapPath(".."));
			try
			{
				string sAllText = sb.ToString();
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
							sSQL = "select isnull(MODULE_NAME, '') + '.' + NAME             " + ControlChars.CrLf
							     + "  from vwTERMINOLOGY                                    " + ControlChars.CrLf
							     + " where LANG = 'en-US'                                   " + ControlChars.CrLf
							     + "   and LIST_NAME is null                                " + ControlChars.CrLf
							     + "   and NAME is not null                                 " + ControlChars.CrLf
							     + "   and isnull(MODULE_NAME, '') + '.' + NAME not in      " + ControlChars.CrLf
							     + "(      select DATA_LABEL        from EDITVIEWS_FIELDS   where DATA_LABEL        is not null" + ControlChars.CrLf
							     + " union select DATA_LABEL        from DETAILVIEWS_FIELDS where DATA_LABEL        is not null" + ControlChars.CrLf
							     + " union select HEADER_TEXT       from GRIDVIEWS_COLUMNS  where HEADER_TEXT       is not null" + ControlChars.CrLf
							     + " union select CONTROL_TEXT      from DYNAMIC_BUTTONS    where CONTROL_TEXT      is not null" + ControlChars.CrLf
							     + " union select CONTROL_TOOLTIP   from DYNAMIC_BUTTONS    where CONTROL_TOOLTIP   is not null" + ControlChars.CrLf
							     + " union select CONTROL_ACCESSKEY from DYNAMIC_BUTTONS    where CONTROL_ACCESSKEY is not null" + ControlChars.CrLf
							     + " union select DISPLAY_NAME      from SHORTCUTS          where DISPLAY_NAME      is not null" + ControlChars.CrLf
							     + " union select DISPLAY_NAME      from MODULES            where DISPLAY_NAME      is not null" + ControlChars.CrLf
							     + " union                                                  " + ControlChars.CrLf
							     + " select MODULE_NAME + '.LBL_' + COLUMNS.COLUMN_NAME     " + ControlChars.CrLf
							     + "   from      INFORMATION_SCHEMA.COLUMNS      COLUMNS    " + ControlChars.CrLf
							     + "  inner join MODULES                                    " + ControlChars.CrLf
							     + "          on MODULES.TABLE_NAME = COLUMNS.TABLE_NAME    " + ControlChars.CrLf
							     + "  union                                                 " + ControlChars.CrLf
							     + " select MODULE_NAME + '.LBL_LIST_' + COLUMNS.COLUMN_NAME" + ControlChars.CrLf
							     + "   from      INFORMATION_SCHEMA.COLUMNS      COLUMNS    " + ControlChars.CrLf
							     + "  inner join MODULES                                    " + ControlChars.CrLf
							     + "          on MODULES.TABLE_NAME = COLUMNS.TABLE_NAME    " + ControlChars.CrLf
							     + ")                                                       " + ControlChars.CrLf
							     + " order by 1                                             " + ControlChars.CrLf;
							cmd.CommandText = sSQL;
							using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
							{
								Response.Write("<html><body><pre>");
								while ( rdr.Read() )
								{
									string sTerm = Sql.ToString(rdr[0]);
									if ( sTerm[0] == '.' )
									{
										if ( !sAllText.Contains("\"" + sTerm + "\"") )
										{
											Response.Write("exec dbo.spTERMINOLOGY_DeleteTerm '" + sTerm + "';");
											Response.Write(ControlChars.CrLf);
										}
									}
									else
									{
										if ( !sAllText.Contains(sTerm) )
										{
											Response.Write("exec dbo.spTERMINOLOGY_DeleteTerm '" + sTerm + "';");
											Response.Write(ControlChars.CrLf);
										}
									}
								}
								Response.Write("</pre></body></html>");
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
