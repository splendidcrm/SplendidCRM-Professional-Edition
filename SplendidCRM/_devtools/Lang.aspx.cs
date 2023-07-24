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
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for Lang.
	/// </summary>
	public class Lang : System.Web.UI.Page
	{
		void PrecompileDirectoryTree(string strDirectory)
		{
			if ( !Directory.Exists(strDirectory) )
				return;
			FileInfo objInfo;
			string[] arrFiles = Directory.GetFiles(strDirectory);
			for (int i = 0; i < arrFiles.Length; i++)
			{
				objInfo = new FileInfo(arrFiles[i]);
				if ( Response.IsClientConnected )
				{
					if ( objInfo.FullName.EndsWith(".lang.php") )
					{
						LanguagePackImport.InsertTerms(objInfo.FullName, false);
						Response.Write(objInfo.FullName + ControlChars.CrLf);
					}
					else if ( objInfo.FullName.EndsWith(".html") && objInfo.FullName.IndexOf(".help.") >= 0 )
					{
						LanguagePackImport.InsertHelp(objInfo.FullName, false);
						Response.Write(objInfo.FullName + ControlChars.CrLf);
					}
				}
			}

			string[] arrDirectories = Directory.GetDirectories(strDirectory);
			for (int i = 0; i < arrDirectories.Length; i++)
			{
				objInfo = new FileInfo(arrDirectories[i]);
				if (objInfo.Name != "_vti_cnf")
					PrecompileDirectoryTree(objInfo.FullName);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			Response.Buffer = false;
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			Response.ContentEncoding = System.Text.Encoding.UTF8;
			Response.Charset = "UTF-8";
			Response.Write("<html><body><pre>\r\n");

			PrecompileDirectoryTree(Server.MapPath("../LanguagePacks"));
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
