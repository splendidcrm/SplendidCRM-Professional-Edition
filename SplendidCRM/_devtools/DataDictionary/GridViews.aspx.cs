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
using System.Xml;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI;
using System.Diagnostics;

namespace SplendidCRM._devtools.DataDictionary
{
	/// <summary>
	/// Summary description for GridViews.
	/// </summary>
	public class GridViews : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/24/2006 Paul.  Only a developer/administrator should see this. 
			// 01/27/2006 Paul.  Just require admin. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;
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
							string sSQL;
							string sNAME = Sql.ToString(Request.QueryString["NAME"]) ;
							if ( Sql.IsEmptyString(sNAME) )
							{
								sSQL = "select *          " + ControlChars.CrLf
								     + "  from vwGRIDVIEWS" + ControlChars.CrLf
								     + " order by NAME    " + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									Response.Write("<html><body><h1>GridViews</h1>");
									while ( rdr.Read() )
									{
										Response.Write("<a href=\"GridViews.aspx?NAME=" + rdr.GetString(rdr.GetOrdinal("NAME")) + "\">" + rdr.GetString(rdr.GetOrdinal("NAME")) + "</a><br>" + ControlChars.CrLf);
									}
									Response.Write("</body></html>");
								}
							}
							else
							{
								Response.ContentType = "text/xml";
								// 08/06/2008 yxy21969.  Make sure to encode all URLs.
								// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
								Response.AddHeader("Content-Disposition", "attachment;filename=" + SplendidCRM.Utils.ContentDispositionEncode(Request.Browser, sNAME + ".Mapping.xml"));

								XmlDocument xml = new XmlDocument();
								xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
								xml.AppendChild(xml.CreateElement("SplendidTest.Dictionary"));
								XmlAttribute aName = xml.CreateAttribute("Name");
								aName.Value = sNAME + ".Mapping.xml";
								xml.DocumentElement.Attributes.Append(aName);

								sSQL = "select DATA_FIELD               " + ControlChars.CrLf
								     + "     , URL_FIELD                " + ControlChars.CrLf
								     + "  from vwGRIDVIEWS_COLUMNS      " + ControlChars.CrLf
								     + " where GRID_NAME    = @GRID_NAME" + ControlChars.CrLf
								     + "   and DEFAULT_VIEW = 0         " + ControlChars.CrLf
								     + "   and DATA_FIELD is not null   " + ControlChars.CrLf
								     + " order by COLUMN_INDEX          " + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@GRID_NAME", sNAME);
								
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									while ( rdr.Read() )
									{
										string sDATA_FIELD = rdr.GetString(rdr.GetOrdinal("DATA_FIELD"));
										Utils.AppendDictionaryEntry(xml, sDATA_FIELD, "ctlListView_" + sDATA_FIELD);

										if ( !rdr.IsDBNull(rdr.GetOrdinal("URL_FIELD")) )
										{
											string sURL_FIELD = rdr.GetString(rdr.GetOrdinal("URL_FIELD"));
											if ( sDATA_FIELD != sURL_FIELD )
												Utils.AppendDictionaryEntry(xml, sURL_FIELD, "ctlListView_" + sURL_FIELD);
										}
									}
								}
								StringBuilder sb = new StringBuilder();
								if ( xml != null && xml.DocumentElement != null)
									XmlUtil.Dump(ref sb, "", xml.DocumentElement);
								Response.Write(sb.ToString());
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
