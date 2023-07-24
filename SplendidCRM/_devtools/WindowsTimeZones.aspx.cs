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
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for WindowsTimeZones.
	/// http://www.codeproject.com/dotnet/WorldClock.asp?df=100&forumid=126704&exp=0&select=981883
	/// </summary>
	public class WindowsTimeZones : System.Web.UI.Page
	{
		[StructLayout( LayoutKind.Sequential )]
		private struct SYSTEMTIME
		{
			public UInt16 wYear        ;
			public UInt16 wMonth       ;
			public UInt16 wDayOfWeek   ;
			public UInt16 wDay         ;
			public UInt16 wHour        ;
			public UInt16 wMinute      ;
			public UInt16 wSecond      ;
			public UInt16 wMilliseconds;
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct TZI
		{
			public int        nBias         ;
			public int        nStandardBias ;
			public int        nDaylightBias ;
			public SYSTEMTIME dtStandardDate;
			public SYSTEMTIME dtDaylightDate;
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			// 04/08/2010 Paul.  Allow this page to be run on a production system. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;
			try
			{
				DataView vwTimeZones = new DataView(SplendidCache.Timezones());

				StringBuilder sbSQL = new StringBuilder();
				RegistryKey keyTimeZones = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones");
				if ( keyTimeZones != null )
				{
					Response.Write("<table border=1 cellspacing=0 cellpadding=4>" + ControlChars.CrLf);
					Response.Write("	<tr>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Name</th>" + ControlChars.CrLf);
					Response.Write("		<th>Display  Name</th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Name</th>" + ControlChars.CrLf);

					Response.Write("		<th>Bias         </th>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Bias</th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Bias</th>" + ControlChars.CrLf);

					Response.Write("		<th>Standard Date       </th>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Year       </th>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Month      </th>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Day        </th>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Day Of Week</th>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Hour       </th>" + ControlChars.CrLf);
					Response.Write("		<th>Standard Minute     </th>" + ControlChars.CrLf);

					Response.Write("		<th>Daylight Date       </th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Year       </th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Month      </th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Day        </th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Day Of Week</th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Hour       </th>" + ControlChars.CrLf);
					Response.Write("		<th>Daylight Minute     </th>" + ControlChars.CrLf);
					Response.Write("	</tr>" + ControlChars.CrLf);

					foreach ( string sTimeZone in keyTimeZones.GetSubKeyNames() )
					{
						RegistryKey keyTimeZone = keyTimeZones.OpenSubKey(sTimeZone);
						string sStandardName = keyTimeZone.GetValue("Std"    ).ToString();
						string sDisplayName  = keyTimeZone.GetValue("Display").ToString();
						string sDaylightName = keyTimeZone.GetValue("Dlt"    ).ToString();
						byte[] byTZI         = (byte[]) keyTimeZone.GetValue("TZI");

						TZI tzi ;
						GCHandle h = GCHandle.Alloc(byTZI, GCHandleType.Pinned);
						try
						{
							tzi = (TZI) Marshal.PtrToStructure( h.AddrOfPinnedObject(), typeof(TZI) );
						}
						finally
						{
							h.Free();
						}
						// 04/08/2010 Paul.  Do some checking to detect timezones that have not changed. 
						string sCommentMarker = "";
						vwTimeZones.RowFilter = "NAME = '" + sDisplayName.Replace("'", "''") + "'";
						if ( vwTimeZones.Count > 0 )
						{
							DataRowView row = vwTimeZones[0];
							string sNAME                  = Sql.ToString (row["NAME"                   ]);
							string sSTANDARD_NAME         = Sql.ToString (row["STANDARD_NAME"          ]);
							string sSTANDARD_ABBREVIATION = Sql.ToString (row["STANDARD_ABBREVIATION"  ]);
							string sDAYLIGHT_NAME         = Sql.ToString (row["DAYLIGHT_NAME"          ]);
							string sDAYLIGHT_ABBREVIATION = Sql.ToString (row["DAYLIGHT_ABBREVIATION"  ]);
							int    nBIAS                  = Sql.ToInteger(row["BIAS"                   ]);
							int    nSTANDARD_BIAS         = Sql.ToInteger(row["STANDARD_BIAS"          ]);
							int    nDAYLIGHT_BIAS         = Sql.ToInteger(row["DAYLIGHT_BIAS"          ]);
							int    nSTANDARD_YEAR         = Sql.ToInteger(row["STANDARD_YEAR"          ]);
							int    nSTANDARD_MONTH        = Sql.ToInteger(row["STANDARD_MONTH"         ]);
							int    nSTANDARD_WEEK         = Sql.ToInteger(row["STANDARD_WEEK"          ]);
							int    nSTANDARD_DAYOFWEEK    = Sql.ToInteger(row["STANDARD_DAYOFWEEK"     ]);
							int    nSTANDARD_HOUR         = Sql.ToInteger(row["STANDARD_HOUR"          ]);
							int    nSTANDARD_MINUTE       = Sql.ToInteger(row["STANDARD_MINUTE"        ]);
							int    nDAYLIGHT_YEAR         = Sql.ToInteger(row["DAYLIGHT_YEAR"          ]);
							int    nDAYLIGHT_MONTH        = Sql.ToInteger(row["DAYLIGHT_MONTH"         ]);
							int    nDAYLIGHT_WEEK         = Sql.ToInteger(row["DAYLIGHT_WEEK"          ]);
							int    nDAYLIGHT_DAYOFWEEK    = Sql.ToInteger(row["DAYLIGHT_DAYOFWEEK"     ]);
							int    nDAYLIGHT_HOUR         = Sql.ToInteger(row["DAYLIGHT_HOUR"          ]);
							int    nDAYLIGHT_MINUTE       = Sql.ToInteger(row["DAYLIGHT_MINUTE"        ]);
							if (  sSTANDARD_NAME         == sStandardName
							   && sDAYLIGHT_NAME         == sDaylightName
							   && nBIAS                  == tzi.nBias                    
							   && nSTANDARD_BIAS         == tzi.nStandardBias            
							   && nDAYLIGHT_BIAS         == tzi.nDaylightBias            
							   && nSTANDARD_YEAR         == tzi.dtStandardDate.wYear     
							   && nSTANDARD_MONTH        == tzi.dtStandardDate.wMonth    
							   && nSTANDARD_WEEK         == tzi.dtStandardDate.wDay      
							   && nSTANDARD_DAYOFWEEK    == tzi.dtStandardDate.wDayOfWeek
							   && nSTANDARD_HOUR         == tzi.dtStandardDate.wHour     
							   && nSTANDARD_MINUTE       == tzi.dtStandardDate.wMinute   
							   && nDAYLIGHT_YEAR         == tzi.dtDaylightDate.wYear     
							   && nDAYLIGHT_MONTH        == tzi.dtDaylightDate.wMonth    
							   && nDAYLIGHT_WEEK         == tzi.dtDaylightDate.wDay      
							   && nDAYLIGHT_DAYOFWEEK    == tzi.dtDaylightDate.wDayOfWeek
							   && nDAYLIGHT_HOUR         == tzi.dtDaylightDate.wHour     
							   && nDAYLIGHT_MINUTE       == tzi.dtDaylightDate.wMinute   
							   )
							{
								sCommentMarker = "--";
							}
						}

						sbSQL.Append(sCommentMarker);
						sbSQL.Append("exec dbo.spTIMEZONES_UpdateByName null");
						sbSQL.Append(", '" + sDisplayName .Replace("'", "''") + "'" + Strings.Space(61-sDisplayName.Length));
						sbSQL.Append(", '" + sStandardName.Replace("'", "''") + "'" + Strings.Space(31-sStandardName.Length));
						sbSQL.Append(", ''");
						sbSQL.Append(", '" + sDaylightName.Replace("'", "''") + "'" + Strings.Space(31-sDaylightName.Length));
						sbSQL.Append(", ''");
						sbSQL.Append(", " + tzi.nBias                     );
						sbSQL.Append(", " + tzi.nStandardBias             );
						sbSQL.Append(", " + tzi.nDaylightBias             );
						sbSQL.Append(", " + tzi.dtStandardDate.wYear      );
						sbSQL.Append(", " + tzi.dtStandardDate.wMonth     );
						sbSQL.Append(", " + tzi.dtStandardDate.wDay       );  // Week
						sbSQL.Append(", " + tzi.dtStandardDate.wDayOfWeek );
						sbSQL.Append(", " + tzi.dtStandardDate.wHour      );
						sbSQL.Append(", " + tzi.dtStandardDate.wMinute    );
						sbSQL.Append(", " + tzi.dtDaylightDate.wYear      );
						sbSQL.Append(", " + tzi.dtDaylightDate.wMonth     );
						sbSQL.Append(", " + tzi.dtDaylightDate.wDay       );  // Week
						sbSQL.Append(", " + tzi.dtDaylightDate.wDayOfWeek );
						sbSQL.Append(", " + tzi.dtDaylightDate.wHour      );
						sbSQL.Append(", " + tzi.dtDaylightDate.wMinute    );
						sbSQL.AppendLine();

						Response.Write("	<tr>" + ControlChars.CrLf);
						Response.Write("		<td>" + sDisplayName       + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + sStandardName      + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + sDaylightName      + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.nBias          + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.nStandardBias  + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.nDaylightBias  + "</td>" + ControlChars.CrLf);
						int nThisYear = DateTime.Today.Year;
						if ( tzi.dtStandardDate.wMonth > 0 )
						{
							DateTime dtStandardDate = new DateTime(nThisYear, tzi.dtStandardDate.wMonth, 1, tzi.dtStandardDate.wHour, tzi.dtStandardDate.wMinute, tzi.dtStandardDate.wSecond, tzi.dtStandardDate.wMilliseconds);
							dtStandardDate = dtStandardDate.AddDays(7 - (dtStandardDate.DayOfWeek - DayOfWeek.Sunday));  // First Sunday in the month. 
							dtStandardDate = dtStandardDate.AddDays(7 * (tzi.dtStandardDate.wDay - 1));  // Last Sunday, but might overflow.  5 means last Sunday. 
							while ( dtStandardDate.Month != tzi.dtStandardDate.wMonth )
								dtStandardDate = dtStandardDate.AddDays(-7);  // In case of overflow, subtract a week until the month matches. 
							Response.Write("		<td>" + dtStandardDate.ToString()     + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtStandardDate.wYear      + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtStandardDate.wMonth     + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtStandardDate.wDay       + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtStandardDate.wDayOfWeek + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtStandardDate.wHour      + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtStandardDate.wMinute    + "</td>" + ControlChars.CrLf);
						}
						else
						{
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
						}
						// Day is actually the week. 
						// Day of Week is typically 0 for Sunday, but Egypt has a 3. 
						if ( tzi.dtDaylightDate.wMonth > 0 )
						{
							DateTime dtDaylightDate = new DateTime(nThisYear, tzi.dtDaylightDate.wMonth, 1, tzi.dtDaylightDate.wHour, tzi.dtDaylightDate.wMinute, tzi.dtDaylightDate.wSecond, tzi.dtDaylightDate.wMilliseconds);
							dtDaylightDate = dtDaylightDate.AddDays(7 - (dtDaylightDate.DayOfWeek - DayOfWeek.Sunday));  // First Sunday in the month. 
							dtDaylightDate = dtDaylightDate.AddDays(7 * (tzi.dtDaylightDate.wDay - 1));  // Last Sunday, but might overflow.  5 means last Sunday. 
							while ( dtDaylightDate.Month != tzi.dtDaylightDate.wMonth )
								dtDaylightDate = dtDaylightDate.AddDays(-7);  // In case of overflow, subtract a week until the month matches. 
							Response.Write("		<td>" + dtDaylightDate.ToString()     + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtDaylightDate.wYear      + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtDaylightDate.wMonth     + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtDaylightDate.wDay       + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtDaylightDate.wDayOfWeek + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtDaylightDate.wHour      + "</td>" + ControlChars.CrLf);
							Response.Write("		<td>" + tzi.dtDaylightDate.wMinute    + "</td>" + ControlChars.CrLf);
						}
						else
						{
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
							Response.Write("		<td>&nbsp;</td>");
						}
						Response.Write("	</tr>" + ControlChars.CrLf);
					}
					sbSQL.AppendLine();
					Response.Write("</table>" + ControlChars.CrLf);
					Response.Write("<pre>");
					Response.Write(sbSQL.ToString());
					Response.Write("</pre>");
				}
			}
			catch(Exception ex)
			{
				Response.Write(ex.Message);
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
