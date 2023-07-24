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
using System.Xml;
using System.Net;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Caching;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace SplendidCRM.Administration.Import
{
	[ServiceContract]
	[ServiceBehavior( IncludeExceptionDetailInFaults = true )]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Required )]
	public class Rest
	{
		public const string MODULE_NAME = "Import";

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public void ImportDatabase(bool Truncate, string FILE_MIME_TYPE, string FILE_DATA)
		{
			HttpContext          Context     = HttpContext.Current;
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			Cache                Cache       = HttpRuntime.Cache;
			
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() || SplendidCRM.Security.AdminUserAccess(MODULE_NAME, "edit") < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			if ( Sql.IsEmptyString(FILE_DATA) )
			{
				throw(new Exception("Missing FILE_DATA"));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			byte[] byFILE_DATA = new byte[] {};
			if ( !Sql.IsEmptyString(FILE_DATA) )
				byFILE_DATA = Convert.FromBase64String(FILE_DATA);
			using ( MemoryStream stm = new MemoryStream(byFILE_DATA) )
			{
				if ( FILE_MIME_TYPE == "text/xml" )
				{
					using ( MemoryStream mstm = new MemoryStream() )
					{
						using ( BinaryWriter mwtr = new BinaryWriter(mstm) )
						{
							using ( BinaryReader reader = new BinaryReader(stm) )
							{
								byte[] binBYTES = reader.ReadBytes(8*1024);
								while ( binBYTES.Length > 0 )
								{
									for(int i=0; i < binBYTES.Length; i++ )
									{
										// MySQL dump seems to dump binary 0 & 1 for byte values. 
										if ( binBYTES[i] == 0 )
											mstm.WriteByte(Convert.ToByte('0'));
										else if ( binBYTES[i] == 1 )
											mstm.WriteByte(Convert.ToByte('1'));
										else
											mstm.WriteByte(binBYTES[i]);
									}
									binBYTES = reader.ReadBytes(8*1024);
								}
							}
							mwtr.Flush();
							mstm.Seek(0, SeekOrigin.Begin);
							XmlDocument xml = new XmlDocument();
							// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
							// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
							// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
							xml.XmlResolver = null;
							xml.Load(mstm);
							SplendidImport.Import(xml, null, Truncate);
						}
					}
				}
				else
				{
					throw(new Exception(L10n.Term("Administration.LBL_IMPORT_DATABASE_ERROR")));
				}
			}
		}
	}
}
