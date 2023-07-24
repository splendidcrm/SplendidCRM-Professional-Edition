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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Diagnostics;

namespace SplendidCRM.FileBrowser
{
	public class Connector : SplendidPage
	{
		// 02/11/2009 Paul.  This page must be accessible without authentication. 
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			Response.CacheControl    = "no-cache";

			XmlDocument xml = new XmlDocument();
			xml.PreserveWhitespace = true;
			xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
			xml.AppendChild(xml.CreateWhitespace("\n"));
			XmlNode xConnector = xml.CreateElement("Connector");
			xml.AppendChild(xConnector);
			try
			{
				string sCommand       = Sql.ToString(Request["Command"      ]);
				string sResourceType  = Sql.ToString(Request["Type"         ]);
				string sCurrentFolder = Sql.ToString(Request["CurrentFolder"]);

				if ( !Security.IsAuthenticated() )
				{
					xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
					XmlUtil.SetSingleNodeAttribute(xml, "Error", "number", "1");
					XmlUtil.SetSingleNodeAttribute(xml, "Error", "text"  , "Authentication is required.");
					xConnector.AppendChild(xml.CreateWhitespace("\n"));
				}
				else if ( Sql.IsEmptyString(sCommand) || Sql.IsEmptyString(sResourceType) || Sql.IsEmptyString(sCurrentFolder) )
				{
					xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
					XmlUtil.SetSingleNodeAttribute(xml, "Error", "number", "1");
					XmlUtil.SetSingleNodeAttribute(xml, "Error", "text"  , "Invalid request.");
					xConnector.AppendChild(xml.CreateWhitespace("\n"));
				}
				else
				{
					string sSiteURL = Utils.MassEmailerSiteURL(Context.Application);
					string sFileURL = sSiteURL + "Images/EmailImage.aspx?ID=";
					switch ( sCommand )
					{
						case "FileUpload":
						{
							int nErrorNumber = 0;
							string sFileName  = String.Empty;
							string sCustomMsg = String.Empty;
							Guid   gImageID   = Guid.Empty;
							
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										FileWorkerUtils.LoadImage(ref gImageID, ref sFileName, trn);
										if ( Sql.IsEmptyGuid(gImageID) )
											nErrorNumber = 202;
										else
											sFileURL += gImageID.ToString();
										trn.Commit();
									}
									catch
									{
										trn.Rollback();
										throw;
									}
								}
							}
							
							Response.Write("<script type=\"text/javascript\">\n");
							Response.Write("window.parent.frames['frmUpload'].OnUploadCompleted(" + nErrorNumber.ToString() + ",'" + Sql.EscapeJavaScript(sFileURL) + "','" + Sql.EscapeJavaScript(sFileName) + "','" + Sql.EscapeJavaScript(sCustomMsg) + "');\n");
							Response.Write("</script>\n");
							return;
						}
						case "GetFolders":
						{
							XmlUtil.SetSingleNodeAttribute(xml, xConnector, "command"     , sCommand     );
							XmlUtil.SetSingleNodeAttribute(xml, xConnector, "resourceType", sResourceType);
							xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
							
							XmlUtil.SetSingleNodeAttribute(xml, "CurrentFolder", "path"        , sCurrentFolder);
							XmlUtil.SetSingleNodeAttribute(xml, "CurrentFolder", "url"         , sFileURL      );
							xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
							
							XmlUtil.SetSingleNode         (xml, "Folders"      , "");
							xConnector.AppendChild(xml.CreateWhitespace("\n"));
							break;
						}
						case "GetFoldersAndFiles":
						{
							XmlUtil.SetSingleNodeAttribute(xml, xConnector, "command"     , sCommand     );
							XmlUtil.SetSingleNodeAttribute(xml, xConnector, "resourceType", sResourceType);
							xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
							
							XmlUtil.SetSingleNodeAttribute(xml, "CurrentFolder", "path"        , sCurrentFolder);
							XmlUtil.SetSingleNodeAttribute(xml, "CurrentFolder", "url"         , sFileURL      );
							xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
							
							XmlUtil.SetSingleNode         (xml, "Folders"      , "");
							xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
							
							XmlUtil.SetSingleNode         (xml, "Files"        , "");
							xConnector.AppendChild(xml.CreateWhitespace("\n"));

							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL ;
								sSQL = "select *             " + ControlChars.CrLf
								     + "  from vwEMAIL_IMAGES" + ControlChars.CrLf
								     + " order by FILENAME   " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									using ( IDataReader rdr = cmd.ExecuteReader() )
									{
										XmlNode xFiles = xConnector.SelectSingleNode("Files");
										while ( rdr.Read() )
										{
											Guid   gID        = Sql.ToGuid  (rdr["ID"       ]);
											string sFILENAME  = Sql.ToString(rdr["FILENAME" ]);
											long   lFILE_SIZE = Sql.ToLong  (rdr["FILE_SIZE"]);
											XmlNode xFile  = xml.CreateElement("File" );
											XmlUtil.SetSingleNodeAttribute(xml, xFile, "name", sFILENAME);
											XmlUtil.SetSingleNodeAttribute(xml, xFile, "size", lFILE_SIZE.ToString());
											XmlUtil.SetSingleNodeAttribute(xml, xFile, "url", sFileURL + gID.ToString());
											xFiles.AppendChild(xml.CreateWhitespace("\n\t\t"));
											xFiles.AppendChild(xFile);
										}
										xFiles.AppendChild(xml.CreateWhitespace("\n\t"));
									}
								}
							}
							break;
						}
						case "CreateFolder":
						{
							xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
							XmlUtil.SetSingleNodeAttribute(xml, "Error", "number", "103");
							XmlUtil.SetSingleNodeAttribute(xml, "Error", "text"  , "Folders cannot be created.");
							xConnector.AppendChild(xml.CreateWhitespace("\n"));
							break;
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				xConnector.RemoveAll();
				xConnector.AppendChild(xml.CreateWhitespace("\n\t"));
				XmlUtil.SetSingleNodeAttribute(xml, "Error", "number", "1");
				XmlUtil.SetSingleNodeAttribute(xml, "Error", "text"  , ex.Message);
				xConnector.AppendChild(xml.CreateWhitespace("\n"));
			}
			Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
			Response.ContentType     = "text/xml";
			Response.Write(xml.OuterXml);
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

