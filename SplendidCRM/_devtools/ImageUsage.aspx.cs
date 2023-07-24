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
using System.Collections;
using System.Diagnostics;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for ImageUsage.
	/// </summary>
	public class ImageUsage : System.Web.UI.Page
	{
		protected StringBuilder sb;

		void RecursiveReadAll(string strDirectory)
		{
			FileInfo objInfo = null;

			string[] arrFiles = Directory.GetFiles(strDirectory);
			for ( int i = 0 ; i < arrFiles.Length ; i++ )
			{
				objInfo = new FileInfo(arrFiles[i]);
				if ( (String.Compare(objInfo.Extension, ".css", true) == 0 || String.Compare(objInfo.Extension, ".cs", true) == 0 || String.Compare(objInfo.Extension, ".aspx", true) == 0 || String.Compare(objInfo.Extension, ".ascx", true) == 0 || String.Compare(objInfo.Extension, ".master", true) == 0) && Response.IsClientConnected )
				{
					//Response.Write("<tr><td></td><td>" + objInfo.FullName + "</td></tr>" + ControlChars.CrLf);
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

			Response.Write("<html><body>Files Not Found<table border=1 cellpadding=0 cellspacing=0>");
			sb = new StringBuilder();
			RecursiveReadAll(Server.MapPath(".."));
			try
			{
				string sAllFiles = sb.ToString();
				sAllFiles = sAllFiles.ToLower();
				
				SortedList sorted = new SortedList();
				string[] arrFiles = Directory.GetFiles(Server.MapPath("../App_Themes/Sugar/images"));
				for ( int i = 0 ; i < arrFiles.Length ; i++ )
				{
					FileInfo objInfo = new FileInfo(arrFiles[i]);
					sorted.Add(objInfo.Name, objInfo.Name.Substring(0, objInfo.Name.Length - objInfo.Extension.Length));
				}
				
				DataTable dtShortcuts = new DataTable();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select IMAGE_NAME      " + ControlChars.CrLf
					     + "  from vwSHORTCUTS_Menu" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dtShortcuts);
						}
					}
				}
				DataRow row = dtShortcuts.NewRow();
				row["IMAGE_NAME"] = "company_logo.png";
				dtShortcuts.Rows.Add(row);

				DataView vwModules   = new DataView(SplendidCache.Modules());
				DataView vwShortcuts = new DataView(dtShortcuts);

				int j = 1;
				foreach ( string sName in sorted.GetKeyList() )
				{
					string sShortName = sorted[sName] as string;
					vwModules.RowFilter = "MODULE_NAME = '" + sShortName + "' or 'Create' + MODULE_NAME = '" + sShortName + "'";
					if ( vwModules.Count > 0 )
						continue;
					vwShortcuts.RowFilter = "IMAGE_NAME = '" + sName + "'";
					if ( vwShortcuts.Count > 0 )
						continue;
					if ( sName.StartsWith("mime-") )
						continue;
					if ( sAllFiles.IndexOf(sName.ToLower()) < 0 && sAllFiles.IndexOf("skinid=\"" + sShortName.ToLower() + "\"") < 0 )
					{
						//Response.Write("<tr>");
						//Response.Write("<td>attrib -r " + sName + "</td>");
						//Response.Write("</tr>");
						Response.Write("<tr>");
						Response.Write("<td>del " + sName + "</td>");
						Response.Write("</tr>");
						//Response.Write("<tr>");
						//Response.Write("<td>" + j.ToString() + "</td>");
						//Response.Write("<td>" + sName + "</td>");
						//Response.Write("<td>" + sShortName + "</td>" + ControlChars.CrLf);
						//Response.Write("</tr>");
						j++;
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message + ControlChars.CrLf);
			}
			Response.Write("</table></body></html>");
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
