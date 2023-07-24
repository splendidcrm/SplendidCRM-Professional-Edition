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
using System.Diagnostics;

namespace SplendidCRM.Administration.Terminology.Export
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Terminology : SplendidPage
	{
		// 11/19/2005 Paul.  Default to expiring everything. 
		override protected bool AuthenticationRequired()
		{
#if DEBUG
			return false;
#else
			return !Sql.ToBoolean(Application["CONFIG.enable_language_export"]);
#endif
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			// 07/09/2010 Paul.  This is a special page that may be publicly accessible. 
			// We need to allow end-users to download language packs from http://community.splendidcrm.com
			this.Visible = !AuthenticationRequired() || (SplendidCRM.Security.AdminUserAccess("Languages", "export") >= 0);
			if ( !this.Visible )
				return;
			
			Response.ExpiresAbsolute = new DateTime(1970, 1, 1);
			try
			{
				string sLANG = Sql.ToString(Request["LANG"]);

				StringBuilder sbXML = new StringBuilder();
				sbXML.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
				sbXML.AppendLine("<xml>");
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select NAME                         " + ControlChars.CrLf
					     + "     , LCID                         " + ControlChars.CrLf
					     + "     , ACTIVE                       " + ControlChars.CrLf
					     + "     , NATIVE_NAME                  " + ControlChars.CrLf
					     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
					     + "  from vwLANGUAGES                  " + ControlChars.CrLf
					     + " where lower(NAME) = @LANG          " + ControlChars.CrLf
					     + "  for xml raw('LANGUAGES'), elements" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LANG", sLANG);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							while ( rdr.Read() )
								sbXML.Append(Sql.ToString(rdr[0]));
						}
					}

					sSQL = "select NAME                                             " + ControlChars.CrLf
					     + "     , LANG                                             " + ControlChars.CrLf
					     + "     , MODULE_NAME                                      " + ControlChars.CrLf
					     + "     , LIST_NAME                                        " + ControlChars.CrLf
					     + "     , LIST_ORDER                                       " + ControlChars.CrLf
					     + "     , DISPLAY_NAME                                     " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY                                    " + ControlChars.CrLf
					     + " where lower(LANG) = @LANG                              " + ControlChars.CrLf
					     + " order by LANG, MODULE_NAME, LIST_NAME, LIST_ORDER, NAME" + ControlChars.CrLf
					     + "   for xml raw('TERMINOLOGY'), elements                 " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LANG", sLANG);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							while ( rdr.Read() )
								sbXML.Append(Sql.ToString(rdr[0]));
						}
					}
				}
				sbXML.AppendLine("</xml>");
				
				// 07/11/2011 Paul.  We are getting an unexplained "Object reference not set to an instance of an object", so make sure to clear the buffer. 
				Response.ContentType = "text/xml";
				Response.Clear();
				Response.Write(sbXML.ToString());
				Response.End();
			}
			catch(Exception ex)
			{
				// 07/11/2011 Paul.  Log the error instead of sending to the user. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				//Response.Write(ex.Message);
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

