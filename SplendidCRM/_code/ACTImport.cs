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
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for ACTImport.
	/// </summary>
	public class ACTImport
	{
		#region ACT Blob methods
		public static void ACTBlobLookupTable(Stream stm, ref UInt32 uLookupTableSize, ref UInt32 uLookupTablePtr)
		{
			// 02/02/2010 Paul.  An ACT Blob file has the following signature. 
			// !@##@!Symantec--ACT!-3.0
			// 02/02/2010 Paul.  More research needs to be done, but the lookup table size/offset seems to be at 0x01d8. 
			stm.Seek(0x01d8, SeekOrigin.Begin);
			byte[] bySize   = new byte[4];
			byte[] byOffset = new byte[4];
			stm.Read(bySize  , 0, 4);
			stm.Read(byOffset, 0, 4);
			// 02/02/2010 Paul.  Convert from big edian to little edian. 
			Array.Reverse(bySize  );
			Array.Reverse(byOffset);
			// 02/03/2010 Paul.  The size is the number entries.  To get the number of bytes, multiply by 8. 
			uLookupTableSize = BitConverter.ToUInt32(bySize  , 0) * 8;
			uLookupTablePtr  = BitConverter.ToUInt32(byOffset, 0);
		}

		public static void ACTBlobExtendedLookupTable(Stream stm, ref UInt32 uExtendedTableType, ref UInt32 uExtendedTablePages, ref UInt32 uExtendedTablePtr)
		{
			// 02/03/2010 Paul.  The extended lookup table points to other 512 byte lookup tables. 
			// The extended pages are expected to be in one contiguous block. 
			stm.Seek(0x01C4, SeekOrigin.Begin);
			byte[] byType   = new byte[4];
			byte[] byOffset = new byte[4];
			stm.Read(byType  , 0, 4);
			stm.Read(byOffset, 0, 4);
			// 02/02/2010 Paul.  Convert from big edian to little edian. 
			Array.Reverse(byType  );
			Array.Reverse(byOffset);
			// 02/03/2010 Paul.  The size is the number of 512 byte pages.  To get the number of entries, multiply by 512/4. 
			// 4 is used because there are 4 bytes per pointer.  The size is always 512 bytes, so the size is not stored in the table. 
			// 04/14/2011 Paul.  The first 4 bytes are the TYPE and not SIZE.  Hard-code the size. 
			uExtendedTableType  = BitConverter.ToUInt32(byType  , 0);
			uExtendedTablePages = (0x200 / 4);
			uExtendedTablePtr   = BitConverter.ToUInt32(byOffset, 0);
		}

		public static UInt32 ACTBlobLookupOffset(string sLookupKey)
		{
			// 02/02/2010 Paul.  If the key is too short, then it is because the trailing spaces were trimmed. 
			if ( sLookupKey.Length < 6 )
				sLookupKey += Strings.Space(6 - sLookupKey.Length);
			char[] arrKey = sLookupKey.ToCharArray();
			byte by1 = Convert.ToByte(arrKey[arrKey.Length-1]);
			byte by2 = Convert.ToByte(arrKey[arrKey.Length-2]);
			byte by3 = Convert.ToByte(arrKey[arrKey.Length-3]);
			byte by4 = Convert.ToByte(arrKey[arrKey.Length-4]);
			// 05/09/2010 Paul.  by5 and by6 did not have the right index. 
			byte by5 = Convert.ToByte(arrKey[arrKey.Length-5]);
			byte by6 = Convert.ToByte(arrKey[arrKey.Length-6]);
			// 02/02/2010 Paul.  The byte values were all offset by 0x20 to make them readable, so undo this offset. 
			by1 -= 0x20;
			by2 -= 0x20;
			by3 -= 0x20;
			by4 -= 0x20;
			by5 -= 0x20;
			by6 -= 0x20;
			// 02/02/2010 Paul.  Not sure how to handle the top 3 bytes. This will be a problem for very large ACT databases. 
			if ( by6 > 0x00 || by5 > 0x00 || by4 > 0x00 )
				throw(new Exception("ACT Blob key is too large to handle: " + sLookupKey));
			UInt32 uLookupTableOffset = (Convert.ToUInt32(by3) * 0x8000 + Convert.ToUInt32(by2) * 0x0200 + Convert.ToUInt32(by1) * 0x08);
			return uLookupTableOffset;
		}

		public static UInt64 ACTDateValue(string sDateKey)
		{
			if ( sDateKey.Length < 6 )
				sDateKey += Strings.Space(6 - sDateKey.Length);
			char[] arrKey = sDateKey.ToCharArray();
			byte by1 = Convert.ToByte(arrKey[arrKey.Length-1]);
			byte by2 = Convert.ToByte(arrKey[arrKey.Length-2]);
			byte by3 = Convert.ToByte(arrKey[arrKey.Length-3]);
			byte by4 = Convert.ToByte(arrKey[arrKey.Length-4]);
			byte by5 = Convert.ToByte(arrKey[arrKey.Length-5]);
			byte by6 = Convert.ToByte(arrKey[arrKey.Length-6]);
			// 02/02/2010 Paul.  The byte values were all offset by 0x20 to make them readable, so undo this offset. 
			by1 -= 0x20;
			by2 -= 0x20;
			by3 -= 0x20;
			by4 -= 0x20;
			by5 -= 0x20;
			by6 -= 0x20;
			
			// 05/10/2010 Paul.  Not sure of the actual compression, so we just have to do the best we can. 
			// The following conversion only works for the date part.  The time part will be invalid. 
			by5 = (byte) (by6 << 6 | by5 & 0x3f);
			by4 = (byte) (by4 << 2 | (by3 >> 4 & 0x03));
			by3 = (byte) (by3 << 4);
			by2 = 0;
			by6 = 0;
			// 05/10/2010 Paul.  The resulting value is the time in seconds since January 1, 1970. 
			UInt64 uDate = (Convert.ToUInt64(by6) << 32) + (Convert.ToUInt64(by5) << 24) + (Convert.ToUInt64(by4) << 16) + (Convert.ToUInt64(by3) << 8) + (Convert.ToUInt64(by2));
			return uDate;
		}

		public static DateTime ACTTimestamp(string sDateKey)
		{
			DateTime dt = new DateTime(1970, 1, 1);
			try
			{
				dt = dt.AddSeconds(ACTDateValue(sDateKey));
				dt = new DateTime(dt.Year, dt.Month, dt.Day);
			}
			catch
			{
				dt = DateTime.MinValue;
			}
			return dt;
		}

		public static string ACTTimestampString(string sDateKey)
		{
			if ( !Sql.IsEmptyString(sDateKey) )
			{
				DateTime dt = ACTTimestamp(sDateKey);
				if ( dt == DateTime.MinValue )
					sDateKey = String.Empty;
				else
					sDateKey = dt.ToString("yyyy/MM/dd");
			}
			return sDateKey;
		}

		public static byte[] ACTDateArray(string sDateKey)
		{
			if ( sDateKey.Length < 6 )
				sDateKey += Strings.Space(6 - sDateKey.Length);
			char[] arrKey = sDateKey.ToCharArray();
			byte by1 = Convert.ToByte(arrKey[arrKey.Length-1]);
			byte by2 = Convert.ToByte(arrKey[arrKey.Length-2]);
			byte by3 = Convert.ToByte(arrKey[arrKey.Length-3]);
			byte by4 = Convert.ToByte(arrKey[arrKey.Length-4]);
			byte by5 = Convert.ToByte(arrKey[arrKey.Length-5]);
			byte by6 = Convert.ToByte(arrKey[arrKey.Length-6]);
			// 02/02/2010 Paul.  The byte values were all offset by 0x20 to make them readable, so undo this offset. 
			by1 -= 0x20;
			by2 -= 0x20;
			by3 -= 0x20;
			by4 -= 0x20;
			by5 -= 0x20;
			by6 -= 0x20;
			byte[] arr = new byte[6];
			arr[0] = by1;
			arr[1] = by2;
			arr[2] = by3;
			arr[3] = by4;
			arr[4] = by5;
			arr[5] = by6;
			return arr;
		}

		public static void ACTBlobValuePosition(Stream stm, UInt32 uLookupTableSize, UInt32 uLookupTablePtr, UInt32 uLookupTableOffset, ref UInt32 uValueSize, ref UInt32 uValuePosition)
		{
			uValueSize     = 0x0000;
			uValuePosition = 0x0000;
			
			UInt32 uLookupTableKeyPosition = uLookupTablePtr + uLookupTableOffset;
			// 02/03/2010 Paul.  If the offset is greater than the size, then we need to use the alternate list of lookup tables. 
			if ( uLookupTableOffset > uLookupTableSize )
			{
				UInt32 uExtendedTableType  = 0x0000;
				UInt32 uExtendedTablePages = 0x0000;
				UInt32 uExtendedTablePtr   = 0x0000;
				// 04/14/2011 Paul.  The first 4 bytes are the TYPE and not SIZE.  Hard-code the size. 
				ACTBlobExtendedLookupTable(stm, ref uExtendedTableType, ref uExtendedTablePages, ref uExtendedTablePtr);
				if ( uExtendedTableType > 0 && uExtendedTablePtr > 0 )
				{
					if ( uExtendedTableType == 0x01 )
					{
						// 04/14/2011 Paul.  For Type 1, the points are to a single level of strings. 
						UInt32 uExtendedOffset      = uLookupTableOffset - uLookupTableSize;
						// 04/16/2011 Paul.  The trick was to subtract the page offset before computing the page. 
						UInt32 uExtendedPage1Offset = uExtendedOffset % 0x200;
						UInt32 uExtendedPage1       = (uExtendedOffset - uExtendedPage1Offset) / 0x80;
						UInt32 uExtendedPage1Ptr    = uExtendedTablePtr + uExtendedPage1;

						stm.Seek(uExtendedPage1Ptr, SeekOrigin.Begin);
						byte[] byPagePosition1 = new byte[4];
						stm.Read(byPagePosition1 , 0, 4);
						Array.Reverse(byPagePosition1);
						uLookupTableKeyPosition = BitConverter.ToUInt32(byPagePosition1, 0);
						uLookupTableKeyPosition += uExtendedPage1Offset;
					}
					else if ( uExtendedTableType == 0x02 )
					{
						// 11/14/2010 Paul.  There seems to be two levels of pointers for the extended data. 
						UInt32 uExtendedOffset      = uLookupTableOffset - uLookupTableSize;
						UInt32 uExtendedPage1       = uExtendedOffset / (0x80 * 0x200);
						UInt32 uExtendedPage1Offset = uExtendedOffset % (0x80 * 0x200);
						if ( uExtendedPage1 < uExtendedTablePages )
						{
							UInt32 uExtendedPage1Ptr = uExtendedTablePtr + uExtendedPage1 * 4;
							stm.Seek(uExtendedPage1Ptr, SeekOrigin.Begin);
							byte[] byPagePosition1 = new byte[4];
							stm.Read(byPagePosition1 , 0, 4);
							Array.Reverse(byPagePosition1);
							UInt32 uExtendedLookupTablePtr1 = BitConverter.ToUInt32(byPagePosition1, 0);
							if ( uExtendedLookupTablePtr1 > 0 )
							{
								UInt32 uExtendedPage2       = uExtendedPage1Offset / 0x200;
								UInt32 uExtendedPage2Offset = uExtendedPage1Offset % 0x200;
								// 02/03/2010 Paul.  Each extended page can have 512/4 pointers, so lets make sure that the page does not exceed the bounds. 
								// The extended pages are expected to be in one contiguous block. 
								if ( uExtendedPage2 < 0x200 )
								{
									UInt32 uExtendedPage2Ptr = uExtendedLookupTablePtr1 + uExtendedPage2 * 4;
									stm.Seek(uExtendedPage2Ptr, SeekOrigin.Begin);
									byte[] byPagePosition2 = new byte[4];
									stm.Read(byPagePosition2 , 0, 4);
									Array.Reverse(byPagePosition2);
									// 02/03/2010 Paul.  Now that we have the extended page, we can treat it as a standard lookup table. 
									UInt32 uExtendedLookupTablePtr2 = BitConverter.ToUInt32(byPagePosition2, 0);
									if ( uExtendedLookupTablePtr2 > 0 )
									{
										uLookupTableKeyPosition = uExtendedLookupTablePtr2 + uExtendedPage2Offset;
									}
									else
									{
										// 11/14/2010 Paul.  If the pointer is NULL, then there is no data. 
										return;
									}
								}
								else
								{
									// 11/14/2010 Paul.  If the pointer is NULL, then there is no data. 
									return;
								}
							}
							else
							{
								// 11/14/2010 Paul.  If the pointer is NULL, then there is no data. 
								return;
							}
						}
						else
						{
							// 11/14/2010 Paul.  If the pointer is NULL, then there is no data. 
							return;
						}
					}
				}
				else
				{
					// 11/14/2010 Paul.  If the pointer is NULL, then there is no data. 
					return;
				}
			}
			stm.Seek(uLookupTableKeyPosition, SeekOrigin.Begin);
			
			// 02/02/2010 Paul.  Now that we are at the location of the key size and position, read and convert from big edian to little edian. 
			byte[] byValueSize     = new byte[4];
			byte[] byValuePosition = new byte[4];
			stm.Read(byValueSize     , 0, 4);
			stm.Read(byValuePosition , 0, 4);
			Array.Reverse(byValueSize    );
			Array.Reverse(byValuePosition);
			uValueSize     = BitConverter.ToUInt32(byValueSize    , 0);
			uValuePosition = BitConverter.ToUInt32(byValuePosition, 0);
		}

		public static string ACTBlobReadString(Stream stm, UInt32 uValueSize, UInt32 uValuePosition)
		{
			// 02/03/2010 Paul.  The maximum size is 32K, so if the size is larger, then there was an error. 
			// 04/14/2011 Paul.  0x0001 seems to be a deleted flag. 
			if ( uValueSize >= 0x8000 || uValueSize == 0x0000 || uValuePosition == 0x0000 || uValuePosition == 0x0001 )
				return String.Empty;
			stm.Seek(uValuePosition, SeekOrigin.Begin);
			StringBuilder sb = new StringBuilder();
			// 02/02/2010 Paul.  Position of 0x0001 seems to suggest that it was deleted. 
			while ( uValuePosition > 0x0001 && sb.Length < uValueSize )
			{
				if ( uValueSize - sb.Length < 256 )
				{
					byte[] byValue = new byte[uValueSize - sb.Length];
					stm.Read(byValue, 0, byValue.Length);
					// 02/02/2010 Paul.  ACT is a very old system, so we are going to assume that they are ASCII and not UTF8. 
					string sValue = UTF8Encoding.ASCII.GetString(byValue);
					sb.Append(sValue);
				}
				else
				{
					// 02/20/2010 Paul.  If the string is longer than 256, then it may be split into chuncks. 
					// The only way to check is to look at the last 4 bytes.  If the first is a NULL, then it is likely a pointer. 
					byte[] byValue = new byte[256 - 4];
					stm.Read(byValue, 0, byValue.Length);
					string sValue = UTF8Encoding.ASCII.GetString(byValue);
					sb.Append(sValue);
					// 02/20/2010 Paul.  Now read the last 4 bytes and check for next pointer. 
					byValue = new byte[4];
					stm.Read(byValue, 0, byValue.Length);
					if ( byValue[0] == 0x00 )
					{
						// This is a pointer to another block. 
						Array.Reverse(byValue);
						uValuePosition = BitConverter.ToUInt32(byValue, 0);
						stm.Seek(uValuePosition, SeekOrigin.Begin);
					}
					else
					{
						// This is not a pointer, so append these 4 bytes as a string, and continue reading. 
						sValue = UTF8Encoding.ASCII.GetString(byValue);
						sb.Append(sValue);
					}
					
				}
			}
			return sb.ToString();
		}

		public static string ACTBlobLookupString(Stream stm, UInt32 uLookupTableSize, UInt32 uLookupTablePtr, string sLookupKey)
		{
			if ( Sql.IsEmptyString(sLookupKey) )
				return String.Empty;
			UInt32 uLookupTableOffset = ACTBlobLookupOffset(sLookupKey);
			
			UInt32 uValueSize     = 0x0000;
			UInt32 uValuePosition = 0x0000;
			ACTBlobValuePosition(stm, uLookupTableSize, uLookupTablePtr, uLookupTableOffset, ref uValueSize, ref uValuePosition);
			// 04/16/2011 Paul.  We have noticed a number of strings with a trailing zero. 
			string sValue = ACTBlobReadString(stm, uValueSize, uValuePosition);
			if ( sValue.Length > 0 )
			{
				if ( sValue[sValue.Length-1] == '\0' )
					sValue = sValue.Substring(0, sValue.Length - 1);
			}
			return sValue;
		}
		#endregion

		public static XmlDocument ConvertActToXml(string sImportModule, Stream stm)
		{
			sImportModule = sImportModule.ToLower();
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
			xml.AppendChild(xml.CreateElement("xml"));
			List<String> arrIgnoreColumns = new List<String>();
			// 05/10/2010 Paul.  We can now convert CTIME and ETIME to the correct values. 
			//arrIgnoreColumns.Add("CTIME");
			//arrIgnoreColumns.Add("ETIME");
			arrIgnoreColumns.Add("MTIME");
			List<String> arrAdditionalColumns = new List<String>();
			arrAdditionalColumns.Add("EMAIL1");
			arrAdditionalColumns.Add("EMAIL2");
			string sTempPath        = Path.GetTempPath();
			string sActBlobPathName = String.Empty;
			sTempPath = Path.Combine(sTempPath, "Splendid");
			// 12/15/2019 Paul.  The Splendid folder may not exist. 
			if ( !Directory.Exists(sTempPath) )
			{
				Directory.CreateDirectory(sTempPath);
			}
			using ( ZipInputStream stmZip = new ZipInputStream(stm) )
			{
				ZipEntry theEntry = null;
				while ( (theEntry = stmZip.GetNextEntry()) != null )
				{
					string sFILENAME = Path.GetFileName(theEntry.Name);
					string sFILE_EXT = Path.GetExtension(sFILENAME);
					/*
					string sTempPathName = Path.Combine(sTempPath, sFILENAME);
					using ( FileStream stmDatabase = new FileStream(sTempPathName, FileMode.Create) )
					{
						using ( BinaryWriter mwtr = new BinaryWriter(stmDatabase) )
						{
							byte[] binBYTES = new byte[64*1024];
							while ( true )
							{
								int nReadBytes = stmZip.Read(binBYTES, 0, binBYTES.Length);
								if ( nReadBytes > 0 )
									mwtr.Write(binBYTES, 0, nReadBytes);
								else
									break;
							}
						}
					}
					*/
					sFILE_EXT = sFILE_EXT.ToLower();
					try
					{
						XmlDocument xml2 = null;
						// http://kb.sagesoftwareonline.com/cgi-bin/sagesoftwareonline.cfg/php/enduser/std_adp.php?p_faqid=11993&p_sid=IR2EIfTj&p_lva=12048
						switch ( sFILE_EXT )
						{
							// Activities.  Contains records for calls, meetings, and to-do activities. Activity records are identified by a Unique ID. 
							// Activity records are linked to contact records and to group records by the Unique ID of the contact or group. 
							// Activities are also linked to contacts by Relational table records.
							case ".adb":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, "activities"         , true, arrIgnoreColumns);  break;
							case ".adx":  break;
							// What Are the .ABC and ABU Files for ACT! 6.0 Databases?
							// http://kb.sagesoftwareonline.com/cgi-bin/sagesoftwareonline.cfg/php/enduser/std_adp.php?p_faqid=12653
							case ".abc":  break;
							// How Does the .ABU File Work and Why Does It Get Large?
							// http://kb.sagesoftwareonline.com/cgi-bin/sagesoftwareonline.cfg/php/enduser/std_adp.php?p_faqid=12756
							case ".abu":  break;
							// Binary Large Object.  Contains variable size data including attachment file and path names, database user information, 
							// drop-down list information, e-mail addresses, notes and history regarding text, recurring activity information, 
							// and synchronization settings. ACT! database tables contain ID numbers to point to the Binary Large Object Database file.
							case ".blb":  
								sActBlobPathName = Path.GetTempFileName();
								HttpContext.Current.Session["TempFile." + Guid.NewGuid().ToString()] = sActBlobPathName;
								SplendidImport.ZipSaveStream(stmZip, sActBlobPathName);
								break;
							// Contacts.  Contains contact information. Contact records are identified by a Unique ID. 
							// (Email addresses are not stored in this table.)
							// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
							case ".dbf":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, sImportModule, false, arrIgnoreColumns, arrAdditionalColumns);  break;
							// Sales drop-downs.  Referred to as the List table, or the Directory table. Contains drop-down information for the Product, 
							// Type and Main Competitor fields in Sales Opportunities. The Sales table stores a Unique ID for the field containing 
							// each of these three types of data. List records are identified and linked to sales records by the List table Unique ID field.
							case ".ddb":
								try
								{
									// 09/04/2010 Paul.  The terminology is not critical. 
									xml2 = SplendidImport.ConvertDBaseToXml(stmZip, "terminology"        , true, arrIgnoreColumns);
								}
								catch(Exception ex)
								{
									xml2 = null;
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Failed to open DBase file " + sFILENAME + ":" + ex.Message);
								}
								break;
							// Database Definitions.  Stores field definition information including mapping of ACT! field names to ACT! database schema names, 
							// index file structures, and references to drop-down list items in the Binary Large Object Database file.
							case ".ddf":
								try
								{
									// 09/04/2010 Paul.  The import maps are not critical. 
									xml2 = SplendidImport.ConvertDBaseToXml(stmZip, "import_maps"        , true, arrIgnoreColumns);
								}
								catch(Exception ex)
								{
									xml2 = null;
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Failed to open DBase file " + sFILENAME + ":" + ex.Message);
								}
								break;
							case ".ddx":  break;
							// E-mail address.  Contains contact E-mail address information. E-mail records are identified and linked to contact records 
							// by the Contact Unique ID field. Actual addresses are stored in the Binary Large Object Database file.
							case ".edb":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, "email_addresses"    , true, arrIgnoreColumns);  break;
							case ".edx":  break;
							// Groups.  Contains group record information. (Name, Address, SIC) Does not contain group membership data. Group records are 
							// linked to contact records by records in the Relational table.
							case ".gdb":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, "groups"             , true, arrIgnoreColumns);  break;
							case ".gdx":  break;
							// Notes/histories.  Contains Unique IDs for notes and histories. Actual history information is stored in the BLB so a pointer 
							// to that information can be found here as well.
							case ".hdb":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, "notes"              , true, arrIgnoreColumns);  break;
							case ".hdx":  break;
							case ".mdx":  break;
							// Activities, Alarms, Group members.  Used to maintain relation ships with contacts. Here, activities, alarms and groups are 
							// related to their respective contact record.
							// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
							case ".rel":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, sImportModule + "_activities", true, arrIgnoreColumns);  break;
							case ".rem":  break;
							case ".rex":  break;
							// Sales.  Holds Sales Opportunity information. Product, Type and Competitor fields are stored in the .DDB and are referenced 
							// here by their Unique ID, The Details field is an ID number to the .BLB file.
							case ".sdb":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, "opportunities"      , true, arrIgnoreColumns);  break;
							case ".sdx":  break;
							// Transactions.  Stores changes that have occurred in the database. These are then used during synchronization.
							// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
							case ".tdb":  xml2 = SplendidImport.ConvertDBaseToXml(stmZip, sImportModule + "_audit"     , true, arrIgnoreColumns);  break;
							case ".tdx":  break;
						}
						if ( xml2 != null )
						{
							if ( sFILE_EXT == ".dbf" )
							{
								string sCTIMEIndex = String.Empty;
								string sETIMEIndex = String.Empty;
								// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
								XmlNodeList nlContacts = xml2.DocumentElement.SelectNodes(sImportModule);
								if ( nlContacts.Count > 0 )
								{
									XmlNode xContactHeader = nlContacts[0];
									foreach ( XmlNode xTag in xContactHeader.ChildNodes )
									{
										if ( xTag.Name.StartsWith("ImportField") )
										{
											if ( xTag.InnerText == "CTIME" )
											{
												sCTIMEIndex = xTag.Name;
											}
											else if ( xTag.InnerText == "ETIME" )
											{
												sETIMEIndex = xTag.Name;
											}
										}
									}
									// 05/10/2010 Paul.  We need to convert the ACT timestamp to a valid date string. 
									for ( int nContact = 1; nContact < nlContacts.Count; nContact++ )
									{
										XmlNode xContact = nlContacts[nContact];
										string sTime = Sql.ToString(XmlUtil.SelectSingleNode(xContact, sCTIMEIndex));
										if ( !Sql.IsEmptyString(sTime) )
										{
											string sNewValue = ACTTimestampString(sTime);
											XmlUtil.SetSingleNode(xml, xContact, sCTIMEIndex, sNewValue);
										}
										sTime = Sql.ToString(XmlUtil.SelectSingleNode(xContact, sETIMEIndex));
										if ( !Sql.IsEmptyString(sTime) )
										{
											string sNewValue = ACTTimestampString(sTime);
											XmlUtil.SetSingleNode(xml, xContact, sETIMEIndex, sNewValue);
										}
									}
								}
							}
							else
							{
								// 05/10/2010 Paul.  We need to convert the ACT timestamp to a valid date string. 
								foreach ( XmlNode x in xml2.DocumentElement.ChildNodes )
								{
									foreach ( XmlNode y in x.ChildNodes )
									{
										if ( y.InnerText != null && (y.Name == "ctime" || y.Name == "etime") )
											y.InnerText = ACTTimestampString(y.InnerText);
									}
								}
							}
#if DEBUG
							string sTempPathName = Path.Combine(sTempPath, sFILENAME + ".xml");
							if ( File.Exists(sTempPathName) )
								File.Delete(sTempPathName);
							xml2.Save(sTempPathName);
#endif
							//xml.DocumentElement.InnerXml += xml2.DocumentElement.InnerXml;
							foreach ( XmlNode x in xml2.DocumentElement.ChildNodes )
							{
								xml.DocumentElement.AppendChild(xml.ImportNode(x, true));
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
				}
			}
			try
			{
				// 02/02/2010 Paul.  Read the blob data before merging records. 
				if ( File.Exists(sActBlobPathName) )
				{
					using ( FileStream stmBlob = File.OpenRead(sActBlobPathName) )
					{
						UInt32 uLookupTableSize = 0x0000;
						UInt32 uLookupTablePtr  = 0x0000;
						SplendidCRM.ACTImport.ACTBlobLookupTable(stmBlob, ref uLookupTableSize, ref uLookupTablePtr);
						
						XmlNodeList nlEmailAddresses = xml.DocumentElement.SelectNodes("email_addresses");
						foreach ( XmlNode xEmailAddress in nlEmailAddresses )
						{
							string sLogon = XmlUtil.SelectSingleNode(xEmailAddress, "logon");
							string sValue = SplendidCRM.ACTImport.ACTBlobLookupString(stmBlob, uLookupTableSize, uLookupTablePtr, sLogon);
							XmlUtil.SetSingleNode(xml, xEmailAddress, "email", sValue);
						}
						XmlNodeList nlNotes = xml.DocumentElement.SelectNodes("notes");
						foreach ( XmlNode xNote in nlNotes )
						{
							string sLogon = XmlUtil.SelectSingleNode(xNote, "regarding");
							string sValue = SplendidCRM.ACTImport.ACTBlobLookupString(stmBlob, uLookupTableSize, uLookupTablePtr, sLogon);
							XmlUtil.SetSingleNode(xml, xNote, "description", sValue);
						}
						XmlNodeList nlActivities = xml.DocumentElement.SelectNodes("activities");
						foreach ( XmlNode xActivity in nlActivities )
						{
							string sLogon = XmlUtil.SelectSingleNode(xActivity, "details");
							string sValue = SplendidCRM.ACTImport.ACTBlobLookupString(stmBlob, uLookupTableSize, uLookupTablePtr, sLogon);
							XmlUtil.SetSingleNode(xml, xActivity, "description", sValue);
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				string[] arrPhoneFields = new string[6];
				arrPhoneFields[0] = "PHONE_HOME"     ;
				arrPhoneFields[1] = "PHONE_MOBILE"   ;
				arrPhoneFields[2] = "PHONE_WORK"     ;
				arrPhoneFields[3] = "PHONE_OTHER"    ;
				arrPhoneFields[4] = "PHONE_FAX"      ;
				arrPhoneFields[5] = "ASSISTANT_PHONE";
				string[] arrDateFields = new string[7];
				arrDateFields[0] = "LAST_MEET" ;
				arrDateFields[1] = "LAST_REACH";
				arrDateFields[2] = "LAST_ATMPT";
				arrDateFields[3] = "LTTR_DATE" ;
				arrDateFields[4] = "BIRTHDATE" ;
				arrDateFields[5] = "ALT1REACH" ;
				arrDateFields[6] = "ALT2REACH" ;
				string[][] arrCombinedFields = new string[10][];
				arrCombinedFields[0] = new string[3] { "PRIMARY_ADDRESS_STREET", "PRIMARY_ADDRESS_STREET2", "\n"};
				arrCombinedFields[1] = new string[3] { "PRIMARY_ADDRESS_STREET", "PRIMARY_ADDRESS_STREET3", "\n"};
				arrCombinedFields[2] = new string[3] { "ALT_ADDRESS_STREET"    , "ALT_ADDRESS_STREET2"    , "\n"};
				arrCombinedFields[3] = new string[3] { "ALT_ADDRESS_STREET"    , "ALT_ADDRESS_STREET3"    , "\n"};
				arrCombinedFields[4] = new string[3] { "PHONE_WORK"            , "EXT"                    , " Ext "};
				arrCombinedFields[5] = new string[3] { "PHONE_FAX"             , "FAX_EXT"                , " Ext "};
				arrCombinedFields[6] = new string[3] { "PHONE_OTHER"           , "ALTEXT"                 , " Ext "};
				arrCombinedFields[7] = new string[3] { "ASSISTANT_PHONE"       , "ASST_EXT"               , " Ext "};
				arrCombinedFields[8] = new string[3] { "PHONE2"                , "PHONE2_EXT"             , " Ext "};
				arrCombinedFields[9] = new string[3] { "PHONE3"                , "PHONE3_EXT"             , " Ext "};
				string[] arrEmailFields = new string[2];
				arrEmailFields[0] = "EMAIL1";
				arrEmailFields[1] = "EMAIL2";
				string[] arrNameFields = new string[2];
				arrNameFields[0] = "LAST_NAME";
				arrNameFields[1] = "ACCOUNT_NAME";
				
				// 01/31/2010 Paul. ACT field names don't change, so manually map them to our fields. 
				// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
				XmlNodeList nlContacts = xml.DocumentElement.SelectNodes(sImportModule);
				if ( nlContacts.Count > 0 )
				{
					XmlNode xContactHeader = nlContacts[0];
					foreach ( XmlNode xTag in xContactHeader.ChildNodes )
					{
						if ( xTag.Name.StartsWith("ImportField") )
						{
							switch ( xTag.InnerText )
							{
								case "COMPANY"    :  xTag.InnerText = "ACCOUNT_NAME"              ;  break;
								// 05/09/2010 Paul.  Salutation seems to store a nick name. 
								case "SALUTATION" :  xTag.InnerText = "NICK_NAME"                 ;  break;
								case "FNAME"      :  xTag.InnerText = "FIRST_NAME"                ;  break;
								case "LNAME"      :  xTag.InnerText = "LAST_NAME"                 ;  break;
								case "ADDR1"      :  xTag.InnerText = "PRIMARY_ADDRESS_STREET"    ;  break;
								case "ADDR2"      :  xTag.InnerText = "PRIMARY_ADDRESS_STREET2"   ;  break;
								case "ADDR3"      :  xTag.InnerText = "PRIMARY_ADDRESS_STREET3"   ;  break;
								case "CITY"       :  xTag.InnerText = "PRIMARY_ADDRESS_CITY"      ;  break;
								case "STATE"      :  xTag.InnerText = "PRIMARY_ADDRESS_STATE"     ;  break;
								case "ZIP"        :  xTag.InnerText = "PRIMARY_ADDRESS_POSTALCODE";  break;
								case "COUNTRY"    :  xTag.InnerText = "PRIMARY_ADDRESS_COUNTRY"   ;  break;
								case "ALTADDR1"   :  xTag.InnerText = "ALT_ADDRESS_STREET"        ;  break;
								case "ALTADDR2"   :  xTag.InnerText = "ALT_ADDRESS_STREET2"       ;  break;
								case "ALTADDR3"   :  xTag.InnerText = "ALT_ADDRESS_STREET3"       ;  break;
								case "ALTCITY"    :  xTag.InnerText = "ALT_ADDRESS_CITY"          ;  break;
								case "ALTSTATE"   :  xTag.InnerText = "ALT_ADDRESS_STATE"         ;  break;
								case "ALTZIP"     :  xTag.InnerText = "ALT_ADDRESS_POSTALCODE"    ;  break;
								case "ALTCOUNTRY" :  xTag.InnerText = "ALT_ADDRESS_COUNTRY"       ;  break;
								case "HOME_PHONE" :  xTag.InnerText = "PHONE_HOME"                ;  break;
								case "MOBILPHONE" :  xTag.InnerText = "PHONE_MOBILE"              ;  break;
								case "PHONE"      :  xTag.InnerText = "PHONE_WORK"                ;  break;
								case "ALTPHONE"   :  xTag.InnerText = "PHONE_OTHER"               ;  break;
								case "FAX"        :  xTag.InnerText = "PHONE_FAX"                 ;  break;
								case "ASST_PHONE" :  xTag.InnerText = "ASSISTANT_PHONE"           ;  break;
								case "IDSTATUS"   :  xTag.InnerText = "LEAD_SOURCE"               ;  break;
								case "BIRTHDAY"   :  xTag.InnerText = "BIRTHDATE"                 ;  break;
								case "CREATOR"    :  xTag.InnerText = "CREATED_BY"                ;  break;
								case "OWNER"      :  xTag.InnerText = "ASSIGNED_TO"               ;  break;
								case "USER"       :  xTag.InnerText = "MODIFIED_BY"               ;  break;
								case "URL"        :  xTag.InnerText = "WEBSITE"                   ;  break;
								case "TICKERSYM"  :  xTag.InnerText = "TICKER_SYMBOL"             ;  break;
								case "CTIME"      :  xTag.InnerText = "DATE_ENTERED"              ;  break;
								case "ETIME"      :  xTag.InnerText = "DATE_MODIFIED"             ;  break;
							}
						}
						for ( int i = 0; i < arrPhoneFields.Length; i++ )
						{
							if ( xTag.InnerText == arrPhoneFields[i] )
								arrPhoneFields[i] = xTag.Name;
						}
						for ( int i = 0; i < arrDateFields.Length; i++ )
						{
							if ( xTag.InnerText == arrDateFields[i] )
								arrDateFields[i] = xTag.Name;
						}
						for ( int i = 0; i < arrCombinedFields.Length; i++ )
						{
							if ( xTag.InnerText == arrCombinedFields[i][0] )
								arrCombinedFields[i][0] = xTag.Name;
							else if ( xTag.InnerText == arrCombinedFields[i][1] )
								arrCombinedFields[i][1] = xTag.Name;
						}
						for ( int i = 0; i < arrEmailFields.Length; i++ )
						{
							if ( xTag.InnerText == arrEmailFields[i] )
								arrEmailFields[i] = xTag.Name;
						}
						for ( int i = 0; i < arrNameFields.Length; i++ )
						{
							if ( xTag.InnerText == arrNameFields[i] )
								arrNameFields[i] = xTag.Name;
						}
					}
				}
				System.Globalization.DateTimeFormatInfo dateInfo = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
				// 01/31/2010 Paul.  For each contact record, combine the fields based on the table above. 
				for ( int nContact = 1; nContact < nlContacts.Count; nContact++ )
				{
					XmlNode xContact = nlContacts[nContact];
					// 11/14/2010 Paul.  ACT supports a contact without a LAST_NAME, but we do not. 
					// In this case, use the ACCOUNT_NAME as the LAST_NAME. 
					//string sImportField000 = XmlUtil.SelectSingleNode(xContact, "ImportField000");
					string sLAST_NAME      = XmlUtil.SelectSingleNode(xContact, arrNameFields[0]);
					if ( Sql.IsEmptyString(sLAST_NAME) )
					{
						string sACCOUNT_NAME = XmlUtil.SelectSingleNode(xContact, arrNameFields[1]);
						XmlUtil.SetSingleNode(xml, xContact, arrNameFields[0], sACCOUNT_NAME);
					}
					// 01/31/2010 Paul.  The phone fields have strange junk at the end. 
					for ( int i = 0; i < arrPhoneFields.Length; i++ )
					{
						string sPhone = XmlUtil.SelectSingleNode(xContact, arrPhoneFields[i]);
						if ( !Sql.IsEmptyString(sPhone) )
						{
							// 02/03/2010 Paul.  Not sure what the phone flags mean, so just remove them. 
							// |1|0|    \"(--
							// |1|-1|    "(--
							int nFlagIndex = sPhone.IndexOf("|");
							if ( nFlagIndex >= 0 )
							{
								// 03/20/2011 Paul.  Don't subtract one from the index.  The Substring function does this by treating it as a length. 
								sPhone = sPhone.Substring(0, nFlagIndex);
							}
							sPhone = sPhone.Trim();
							XmlUtil.SetSingleNode(xml, xContact, arrPhoneFields[i], sPhone);
						}
					}
					for ( int i = 0; i < arrDateFields.Length; i++ )
					{
						string sDate = XmlUtil.SelectSingleNode(xContact, arrDateFields[i]);
						if ( !Sql.IsEmptyString(sDate) )
						{
							try
							{
								if ( sDate.Length == 8 )
								{
									DateTime dtNewDate = DateTime.ParseExact(sDate, "yyyyMMdd", dateInfo);
									XmlUtil.SetSingleNode(xml, xContact, arrDateFields[i], dtNewDate.ToString());
								}
								else if ( sDate.Length == 12 )
								{
									DateTime dtNewDate = DateTime.ParseExact(sDate, "yyyyMMddHHmm", dateInfo);
									XmlUtil.SetSingleNode(xml, xContact, arrDateFields[i], dtNewDate.ToString());
								}
							}
							catch
							{
							}
						}
					}
					for ( int i = 0; i < arrCombinedFields.Length; i++ )
					{
						// 02/04/2010 Paul.  Use trim to prevent appending if there is nothing to append. 
						string sAppend = Sql.ToString(XmlUtil.SelectSingleNode(xContact, arrCombinedFields[i][1])).TrimEnd();
						if ( !Sql.IsEmptyString(sAppend) )
						{
							string sNewValue = XmlUtil.SelectSingleNode(xContact, arrCombinedFields[i][0]);
							// 01/31/2010 Paul.  Use the defined separator. 
							sNewValue += arrCombinedFields[i][2];
							// 02/04/2010 Paul.  Now add the second field. 
							sNewValue += XmlUtil.SelectSingleNode(xContact, arrCombinedFields[i][1]);
							XmlUtil.SetSingleNode(xml, xContact, arrCombinedFields[i][0], sNewValue);
						}
					}
				}
				
				// 01/31/2010 Paul.  Move related nodes to the Contact record. 
				XmlNodeList nlEmailAddresses = xml.DocumentElement.SelectNodes("email_addresses");
				foreach ( XmlNode xEmailAddress in nlEmailAddresses )
				{
					string sContactID    = XmlUtil.SelectSingleNode(xEmailAddress, "contactid" );
					string sEmailAddress = XmlUtil.SelectSingleNode(xEmailAddress, "email"     );
					string sPrimaryEmail = XmlUtil.SelectSingleNode(xEmailAddress, "prm_status");
					// 07/30/2010 Paul.  Move EncaseXpathString() to XmlUtil.cs. 
					// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
					XmlNode xContact = xml.DocumentElement.SelectSingleNode(sImportModule + "[ImportField000=" + XmlUtil.EncaseXpathString(sContactID) + "]");
					if ( xContact != null )
					{
						if ( sPrimaryEmail == "1" )
							XmlUtil.SetSingleNode(xml, xContact, arrEmailFields[0], sEmailAddress);
						else
							XmlUtil.SetSingleNode(xml, xContact, arrEmailFields[1], sEmailAddress);
						xContact.AppendChild(xEmailAddress);
						//xContact.AppendChild(xEmailAddress.CloneNode(true));
						//xEmailAddress.ParentNode.RemoveChild(xEmailAddress);
					}
				}
				XmlNodeList nlNotes = xml.DocumentElement.SelectNodes("notes");
				foreach ( XmlNode xNote in nlNotes )
				{
					string sUserTime = XmlUtil.SelectSingleNode(xNote, "user_time");
					if ( !Sql.IsEmptyString(sUserTime) )
					{
						try
						{
							if ( sUserTime.Length == 8 )
							{
								DateTime dtNewDate = DateTime.ParseExact(sUserTime, "yyyyMMdd", dateInfo);
								XmlUtil.SetSingleNode(xml, xNote, "user_time", dtNewDate.ToString());
							}
							else if ( sUserTime.Length == 12 )
							{
								DateTime dtNewDate = DateTime.ParseExact(sUserTime, "yyyyMMddHHmm", dateInfo);
								XmlUtil.SetSingleNode(xml, xNote, "user_time", dtNewDate.ToString());
							}
						}
						catch
						{
						}
					}
					string sContactID = XmlUtil.SelectSingleNode(xNote, "contactid");
					// 07/30/2010 Paul.  Move EncaseXpathString() to XmlUtil.cs. 
					// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
					XmlNode xContact = xml.DocumentElement.SelectSingleNode(sImportModule + "[ImportField000=" + XmlUtil.EncaseXpathString(sContactID) + "]");
					if ( xContact != null )
					{
						xContact.AppendChild(xNote);
						//xContact.AppendChild(xNote.CloneNode(true));
						//xNote.ParentNode.RemoveChild(xNote);
					}
				}
				XmlNodeList nlActivities = xml.DocumentElement.SelectNodes("activities");
				foreach ( XmlNode xActivity in nlActivities )
				{
					string sStartTime = XmlUtil.SelectSingleNode(xActivity, "start_time");
					if ( !Sql.IsEmptyString(sStartTime) )
					{
						try
						{
							if ( sStartTime.Length == 8 )
							{
								DateTime dtNewDate = DateTime.ParseExact(sStartTime, "yyyyMMdd", dateInfo);
								XmlUtil.SetSingleNode(xml, xActivity, "start_time", dtNewDate.ToString());
							}
							else if ( sStartTime.Length == 12 )
							{
								DateTime dtNewDate = DateTime.ParseExact(sStartTime, "yyyyMMddHHmm", dateInfo);
								XmlUtil.SetSingleNode(xml, xActivity, "start_time", dtNewDate.ToString());
							}
						}
						catch
						{
						}
					}
					string sEndTime = XmlUtil.SelectSingleNode(xActivity, "end_time");
					if ( !Sql.IsEmptyString(sEndTime) )
					{
						try
						{
							if ( sEndTime.Length == 8 )
							{
								DateTime dtNewDate = DateTime.ParseExact(sEndTime, "yyyyMMdd", dateInfo);
								XmlUtil.SetSingleNode(xml, xActivity, "end_time", dtNewDate.ToString());
							}
							else if ( sEndTime.Length == 12 )
							{
								DateTime dtNewDate = DateTime.ParseExact(sEndTime, "yyyyMMddHHmm", dateInfo);
								XmlUtil.SetSingleNode(xml, xActivity, "end_time", dtNewDate.ToString());
							}
						}
						catch
						{
						}
					}
					string sContactID = XmlUtil.SelectSingleNode(xActivity, "schedlwith");
					// 07/30/2010 Paul.  Move EncaseXpathString() to XmlUtil.cs. 
					// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
					XmlNode xContact = xml.DocumentElement.SelectSingleNode(sImportModule + "[ImportField000=" + XmlUtil.EncaseXpathString(sContactID) + "]");
					if ( xContact != null )
					{
						xContact.AppendChild(xActivity);
						//xContact.AppendChild(xActivity.CloneNode(true));
						//xActivity.ParentNode.RemoveChild(xActivity);
					}
					else
					{
						// 02/05/2010 Paul.  If we can't find the Scheduled With, then try the Scheduled For. 
						sContactID = XmlUtil.SelectSingleNode(xActivity, "schedl_for");
						// 07/30/2010 Paul.  Move EncaseXpathString() to XmlUtil.cs. 
						// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
						xContact = xml.DocumentElement.SelectSingleNode(sImportModule + "[ImportField000=" + XmlUtil.EncaseXpathString(sContactID) + "]");
						if ( xContact != null )
						{
							xContact.AppendChild(xActivity);
							//xContact.AppendChild(xActivity.CloneNode(true));
							//xActivity.ParentNode.RemoveChild(xActivity);
						}
					}
				}
				// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
				XmlNodeList nlContactsActivities = xml.DocumentElement.SelectNodes(sImportModule + "_activities[type=1]");
				foreach ( XmlNode xContactsActivities in nlContactsActivities )
				{
					xContactsActivities.ParentNode.RemoveChild(xContactsActivities);
				}
				// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
				nlContactsActivities = xml.DocumentElement.SelectNodes(sImportModule + "_activities[type=0]");
				foreach ( XmlNode xContactsActivities in nlContactsActivities )
				{
					string sContactID = XmlUtil.SelectSingleNode(xContactsActivities, "field1");
					string sGroupID   = XmlUtil.SelectSingleNode(xContactsActivities, "field2");
					// 07/30/2010 Paul.  Move EncaseXpathString() to XmlUtil.cs. 
					// 11/14/2010 Paul.  Allow ACT database to be imported into Leads module. 
					XmlNode xContact = xml.DocumentElement.SelectSingleNode(sImportModule + "[ImportField000=" + XmlUtil.EncaseXpathString(sContactID) + "]");
					if ( xContact != null )
					{
						// 07/30/2010 Paul.  Move EncaseXpathString() to XmlUtil.cs. 
						XmlNode xGroup = xml.DocumentElement.SelectSingleNode("groups[unique_id=" + XmlUtil.EncaseXpathString(sGroupID) + "]");
						if ( xGroup != null )
						{
							XmlNode xProspectLists = xml.CreateElement("prospect_lists");
							xContact.AppendChild(xProspectLists);
							string sName        = XmlUtil.SelectSingleNode(xGroup, "grp_name"  );
							string sDescription = XmlUtil.SelectSingleNode(xGroup, "descrption");
							XmlUtil.SetSingleNode(xml, xProspectLists, "name"       , sName       );
							XmlUtil.SetSingleNode(xml, xProspectLists, "description", sDescription);
						}
					}
					xContactsActivities.ParentNode.RemoveChild(xContactsActivities);
				}
			}
			catch(Exception ex)
			{
				// 03/20/2011 Paul.  Show the error. 
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex.Message);
			}
			return xml;
		}
		
		// 07/30/2010 Paul.  Move EncaseXpathString() to XmlUtil.cs. 
	}
}
