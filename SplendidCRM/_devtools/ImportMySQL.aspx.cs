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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for ImportMySQL.
	/// </summary>
	public class ImportMySQL : System.Web.UI.Page
	{
		protected HtmlInputFile fileUNC;

		protected void Page_ItemCommand(Object sender, CommandEventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			// 12/22/2007 Paul.  Allow an admin to import data. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;
			if ( e.CommandName == "Upload" )
			{
				if ( fileUNC.PostedFile == null || Sql.IsEmptyString(fileUNC.PostedFile.FileName) )
				{
					throw(new Exception("File was not provided"));
				}
			
				Response.ContentType = "text/sql";
				// 08/06/2008 yxy21969.  Make sure to encode all URLs.
				// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
				Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, "ImportMySQL.sql"));
				Response.Write("set nocount on" + ControlChars.CrLf);
				Response.Write("GO" + ControlChars.CrLf);
				
				HttpPostedFile pstFile  = fileUNC.PostedFile;
				XmlDocument xml = new XmlDocument();
				// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
				// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
				// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
				xml.XmlResolver = null;
				xml.Load(pstFile.InputStream);
				foreach(XmlNode node in xml.DocumentElement.ChildNodes)
				{
					if ( node.NodeType == XmlNodeType.Element )
					{
						string sTableName = node.Name.ToUpper();
						StringBuilder  sbUpdate       = new StringBuilder();
						StringBuilder  sbInsertColumn = new StringBuilder();
						StringBuilder  sbInsertValues = new StringBuilder();
						sbUpdate.AppendLine("	update " + sTableName);
						sbInsertColumn.Append("	insert into " + sTableName);
						sbInsertValues.Append("	            " + Strings.Space(Math.Max(0, node.Name.Length - 6)) + "values");
						string sColumnLeader = "(";
						string sUpdateLeader = "	   set ";
						string sPrimaryKeyName  = String.Empty;
						string sPrimaryKeyValue = String.Empty;
						foreach(XmlNode nodeColumn in node.ChildNodes)
						{
							if ( node.NodeType == XmlNodeType.Element )
							{
								string sColumnName  = nodeColumn.Name.ToUpper();
								string sColumnValue = nodeColumn.InnerText.Replace("'", "''");
								if ( sColumnName == "ROLES_MODULES" && sColumnName == "MODULE_ID" )
									sColumnName = "MODULE";
								sbUpdate.Append(sUpdateLeader);
								sbInsertColumn.Append(sColumnLeader);
								sbInsertColumn.Append(nodeColumn.Name.ToUpper());
								sbInsertValues.Append(sColumnLeader);
								if ( (sColumnName == "ID" || sColumnName.EndsWith("_ID") || sColumnName.EndsWith("_BY")) && (nodeColumn.InnerText.Length > 0 && nodeColumn.InnerText.Length < 12) )
								{
									sColumnValue = "00000000-0000-0000-0000-";  // 42b109076e06
									// 07/31/2006 Paul.  Stop using VisualBasic library to increase compatibility with Mono. 
									sColumnValue += new string('0', 12 - nodeColumn.InnerText.Length);
									sColumnValue += nodeColumn.InnerText;
								}
								if ( sColumnName == "DO_NOT_CALL" || sColumnName == "EMAIL_OPT_OUT" || sColumnName == "DATE_DUE_FLAG" || sColumnName == "DATE_START_FLAG" || sColumnName == "IS_ADMIN" )
								{
									if ( sColumnValue == "off" )
										sColumnValue = "0";
									else if ( sColumnValue == "on" )
										sColumnValue = "1";
								}
								if ( sColumnValue.Length == 0 )
								{
									sbUpdate.Append(sColumnName + " = null");
									sbInsertValues.Append("null");
								}
								else
								{
									sbUpdate.Append(sColumnName + " = ");
									if ( sColumnName.StartsWith("AMOUNT") )
									{
										sbUpdate.Append(sColumnValue);
										sbInsertValues.Append(sColumnValue);
									}
									else
									{
										sbUpdate.Append("'");
										sbUpdate.Append(sColumnValue);
										sbUpdate.Append("'");
										sbInsertValues.Append("'");
										sbInsertValues.Append(sColumnValue);
										sbInsertValues.Append("'");
									}
								}
								if ( sColumnName == "ID" )
								{
									sPrimaryKeyName  = sColumnName ;
									sPrimaryKeyValue = sColumnValue;
								}
								sColumnLeader = ", ";
								sUpdateLeader = ControlChars.CrLf + "	     , ";
							}
						}
						// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
						sbUpdate.AppendLine();
						sbUpdate.AppendLine("	 where " + sPrimaryKeyName.Replace(" ", "") + " = '" + Sql.EscapeSQL(sPrimaryKeyValue) + "'");
						sbInsertColumn.AppendLine(")");
						sbInsertValues.AppendLine(")");
						if ( sPrimaryKeyName == String.Empty )
						{
							// Don't import CONFIG table. 
							//Response.Write(sbInsertColumn.ToString());
							//Response.Write(sbInsertValues.ToString());
						}
						else
						{
							if ( sTableName == "BUGS" || sTableName == "CASES" || sTableName == "CAMPAIGNS" || sTableName == "PROSPECTS" )
								Response.Write("set identity_insert " + sTableName + " on" + ControlChars.CrLf);
							// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
							Response.Write("if not exists(select * from " + node.Name.Replace(" ", "").ToUpper() + " where " + sPrimaryKeyName.Replace(" ", "") + " = '" + Sql.EscapeSQL(sPrimaryKeyValue) + "')" + ControlChars.CrLf);
							Response.Write(sbInsertColumn.ToString());
							Response.Write(sbInsertValues.ToString());
							Response.Write("else" + ControlChars.CrLf);
							Response.Write(sbUpdate.ToString());
							if ( sTableName == "BUGS" || sTableName == "CASES" || sTableName == "CAMPAIGNS" || sTableName == "PROSPECTS" )
								Response.Write("set identity_insert " + sTableName + " off" + ControlChars.CrLf);
							Response.Write("GO" + ControlChars.CrLf);
						}
					}
				}
				Response.End();
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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
