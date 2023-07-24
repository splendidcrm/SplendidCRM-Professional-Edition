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
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Win32;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for WindowsTimeZones.
	/// http://www.codeproject.com/dotnet/WorldClock.asp?df=100&forumid=126704&exp=0&select=981883
	/// </summary>
	public class WindowsLanguages : System.Web.UI.Page
	{
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			StringBuilder sbSQL = new StringBuilder();
			Response.Write("<table border=1 cellspacing=0 cellpadding=4>" + ControlChars.CrLf);
			Response.Write("	<tr>" + ControlChars.CrLf);
			Response.Write("		<th>Name        </th>" + ControlChars.CrLf);
			Response.Write("		<th>LCID        </th>" + ControlChars.CrLf);
			Response.Write("		<th>Native Name</th>" + ControlChars.CrLf);
			Response.Write("		<th>Display Name</th>" + ControlChars.CrLf);
			Response.Write("	</tr>" + ControlChars.CrLf);

			CultureInfo[] aCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			foreach ( CultureInfo culture in aCultures)
			{
				Response.Write("	<tr>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.Name        + "</td>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.LCID        + "</td>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.NativeName  + "</td>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.DisplayName + "</td>" + ControlChars.CrLf);
				Response.Write("	</tr>" + ControlChars.CrLf);

				sbSQL.Append("--exec dbo.spLANGUAGES_InsertOnly null ");
				sbSQL.Append(", '" + culture.Name + "'" + Strings.Space(15-culture.Name.Length));
				sbSQL.Append(", " + Strings.Space(5-culture.LCID.ToString().Length) + culture.LCID );
				sbSQL.Append(", 0" );
				sbSQL.Append(", '" + culture.NativeName .Replace("'", "''") + "'");
				sbSQL.Append(", '" + culture.DisplayName.Replace("'", "''") + "'");
				sbSQL.AppendLine();

			}
			Response.Write("</table>" + ControlChars.CrLf);
			Response.Write("<pre>");
			Response.Write(sbSQL.ToString());
			Response.Write("</pre>");
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
