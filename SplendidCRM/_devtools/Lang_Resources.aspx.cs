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
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for Lang_Resources.
	/// </summary>
	public class Lang_Resources : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			Response.Buffer = false;
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			Response.Write("<html><body><pre>\r\n");

			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string strSpecifiedLang = Request["Lang"];
				con.Open();
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							for ( int i = 0 ; i < dt.Rows.Count && Response.IsClientConnected ; i++ )
							{
								string sLANG = Sql.ToString(dt.Rows[i]["Lang"]);
								Response.Write(sLANG + "\r\n");
								/* the following language packs have errors. 
								cn-ZH
								dk
								ge-CH
								ge-GE
								se
								sp-CO
								sp-VE
								tw-ZH
								*/
								if ( strSpecifiedLang == null || sLANG == strSpecifiedLang )
								{
									string sSQL ;
									sSQL = "select *                     " + ControlChars.CrLf
									     + "  from TERMINOLOGY           " + ControlChars.CrLf
									     + " where LANG = '" + sLANG + "'" + ControlChars.CrLf
									     + "   and LIST_NAME is null     " + ControlChars.CrLf
									     + " order by NAME               " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									using (DataTable dtLang = new DataTable() )
									{
										da.Fill(dtLang);
										XmlDocument docClean = new XmlDocument() ;
										// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
										// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
										// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
										docClean.XmlResolver = null;
									
										docClean.Load(Server.MapPath(".") + "\\Resources\\Resource-clean.resx");
										for ( int j = 0 ; j < dtLang.Rows.Count && Response.IsClientConnected ; j++ )
										{
											//Response.Write(dtLang.Rows[j]["LANG"].ToString() + " " + dtLang.Rows[j]["NAME"].ToString() + "\r\n");
											XmlElement elmData  = docClean.CreateElement("data");
											XmlAttribute attName = (XmlAttribute) docClean.CreateNode(XmlNodeType.Attribute, "name", "");
											attName.Value= dtLang.Rows[j]["NAME"].ToString();
											elmData.Attributes.Append(attName);
											XmlElement elmValue   = docClean.CreateElement("value");
											XmlElement elmComment = docClean.CreateElement("comment");
											XmlAttribute attPreserve = (XmlAttribute) docClean.CreateNode(XmlNodeType.Attribute, "space", "xml");
											attPreserve.Value= "preserve";
											elmValue.Attributes.Append(attPreserve);
											elmComment.Attributes.Append((XmlAttribute) attPreserve.Clone());
											docClean.DocumentElement.AppendChild(elmData);
											elmData.AppendChild(elmValue );
											elmData.AppendChild(elmComment);
											elmValue  .InnerText = dtLang.Rows[j]["DISPLAY_NAME"].ToString();
											elmComment.InnerText = dtLang.Rows[j]["MODULE_NAME" ].ToString();
										}
										if ( dt.Rows[i]["Lang"].ToString() == "en-US" )
											docClean.Save(Server.MapPath(".") + "\\Resources\\Resource.resx");
										else
											docClean.Save(Server.MapPath(".") + "\\Resources\\Resource." + dt.Rows[i]["Lang"].ToString() + ".resx");
									}
								}
							}
						}
					}
				}
			}
			Response.Write("</pre></body></html>\r\n");
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
