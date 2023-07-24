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
	/// </summary>
	public class WindowsCountries : System.Web.UI.Page
	{
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;

			Response.ContentType = "text/sql";
			// 08/06/2008 yxy21969.  Make sure to encode all URLs.
			// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
			Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, "TERMINOLOGY countries_dom.sql"));
			StringBuilder sbSQL = new StringBuilder();
			/*
			Response.Write("<table border=1 cellspacing=0 cellpadding=4>" + ControlChars.CrLf);
			Response.Write("	<tr>" + ControlChars.CrLf);
			Response.Write("		<th>Country Code</th>" + ControlChars.CrLf);
			Response.Write("		<th>Name        </th>" + ControlChars.CrLf);
			Response.Write("	</tr>" + ControlChars.CrLf);
			*/

			ArrayList lstExclusions = new ArrayList();
			lstExclusions.Add("United States");  // United States is automatic. 
			lstExclusions.Add("Congo (DRC)");
			lstExclusions.Add("INMARSAT");
			lstExclusions.Add("INMARSAT (Atlantic-East)");
			lstExclusions.Add("INMARSAT (Atlantic-West)");
			lstExclusions.Add("INMARSAT (Indian)");
			lstExclusions.Add("INMARSAT (Pacific)");
			lstExclusions.Add("International Freephone Service");
			lstExclusions.Add("Virgin Islands, British");
			lstExclusions.Add("Antigua and Barbuda");
			lstExclusions.Add("Bosnia and Herzegovina");
			lstExclusions.Add("S�o Tom� and Pr�ncipe");
			lstExclusions.Add("Serbia and Montenegro");
			lstExclusions.Add("St. Kitts and Nevis");
			lstExclusions.Add("St. Pierre and Miquelon");
			lstExclusions.Add("St. Vincent and the Grenadines");
			lstExclusions.Add("Trinidad and Tobago");
			lstExclusions.Add("Turks and Caicos Islands");
			lstExclusions.Add("Wallis and Futuna");
			lstExclusions.Add("Falkland Islands (Islas Malvinas)");
			lstExclusions.Add("Cocos (Keeling) Islands");
			lstExclusions.Add("Macedonia, Former Yugoslav Republic of");
			//lstExclusions.Add("");
			SortedList lst = new SortedList();
			lst.Add("Antigua", "102");
			lst.Add("Barbuda", "102");
			lst.Add("Bosnia", "387");
			lst.Add("Herzegovina", "387");
			lst.Add("S�o Tom�", "239");
			lst.Add("Pr�ncipe", "239");
			lst.Add("Serbia", "381");
			lst.Add("Montenegro", "381");
			lst.Add("St. Kitts ", "115");
			lst.Add("Nevis", "115");
			lst.Add("St. Pierre", "508");
			lst.Add("Miquelon", "508");
			lst.Add("St. Vincent", "116");
			lst.Add("Grenadines", "116");
			lst.Add("Trinidad", "117");
			lst.Add("Tobago", "117");
			lst.Add("Turks", "118");
			lst.Add("Caicos Islands", "118");
			lst.Add("Wallis", "681");
			lst.Add("Futuna", "681");
			lst.Add("Falkland Islands", "500");
			lst.Add("Cocos Islands", "6101");
			lst.Add("Macedonia", "389");
			//lst.Add("", "");
			RegistryKey keyCountries = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Telephony\Country List");
			if ( keyCountries != null )
			{
				int nMaxLength = 0;
				string sName;
				foreach ( string sCountryCode in keyCountries.GetSubKeyNames() )
				{
					RegistryKey keyCountry = keyCountries.OpenSubKey(sCountryCode);
					sName = keyCountry.GetValue("Name").ToString();
					if ( sName.IndexOf(" SAR") > 0 )
						sName = sName.Replace(" SAR", "");
					else if ( sName.IndexOf(", The") > 0 )
						sName = sName.Replace(", The", "");
					if ( !lstExclusions.Contains(sName) && !lst.ContainsKey(sName) )
						lst.Add(sName, sCountryCode);
					nMaxLength = Math.Max(nMaxLength, sName.Length);
				}
				int nCountryIndex = 1;
				sName = "United States";
				sbSQL.AppendLine("exec dbo.spTERMINOLOGY_InsertOnly '" + sName +"'" + Strings.Space(nMaxLength-sName.Length) + ", 'en-US', null, 'countries_dom', " + nCountryIndex.ToString("####") + ", '" + sName + "';");
				nCountryIndex++;
				foreach ( DictionaryEntry entry in lst )
				{
					sName        = Sql.ToString(entry.Key  );
					string sCountryCode = Sql.ToString(entry.Value);
					/*
					Response.Write("	<tr>" + ControlChars.CrLf);
					Response.Write("		<td>" + sCountryCode + "</td>" + ControlChars.CrLf);
					Response.Write("		<td>" + sName        + "</td>" + ControlChars.CrLf);
					Response.Write("	</tr>" + ControlChars.CrLf);
					*/
					sbSQL.AppendLine("exec dbo.spTERMINOLOGY_InsertOnly '" + sName +"'" + Strings.Space(nMaxLength-sName.Length) + ", 'en-US', null, 'countries_dom', " + nCountryIndex.ToString("####") + ", '" + sName + "';");
					nCountryIndex++;
				}
			}
			//Response.Write("</table>" + ControlChars.CrLf);
			//Response.Write("<pre>");
			Response.Write(sbSQL.ToString());
			//Response.Write("</pre>");
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
