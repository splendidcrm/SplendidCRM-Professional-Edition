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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SplendidCRM.Import
{
	/// <summary>
	/// Summary description for ExportFile.
	/// </summary>
	public class ExportFile : SplendidPage
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string sTempPath           = Path.GetTempPath();
				sTempPath = Path.Combine(sTempPath, "Splendid");
				string sExportFileID   = Sql.ToString(Request["FileID"]);
				string sExportPathName = Sql.ToString(Session["TempFile." + sExportFileID]);
				if ( !Sql.IsEmptyString(sExportPathName) )
				{
					if ( File.Exists(sExportPathName) )
					{
						Response.ContentType = System.Web.MimeMapping.GetMimeMapping(sExportPathName);
						Response.AddHeader("Content-Disposition", "attachment;filename=" + sExportFileID);
						const int nBLOCK_SIZE = 1024*1024;
						byte[] byData = new byte[nBLOCK_SIZE];
						using ( FileStream stm = File.OpenRead(sExportPathName) )
						{
							using ( BinaryWriter writer = new BinaryWriter(Response.OutputStream) )
							{
								int nOffset = 0;
								while ( nOffset < stm.Length )
								{
									int nReadSize = Math.Min(Convert.ToInt32(stm.Length) - nOffset, nBLOCK_SIZE);
									stm.Read(byData, 0, nReadSize);
									writer.Write(byData, 0, nReadSize);
									nOffset += nReadSize;
								}
							}
						}
					}
					else
					{
						throw(new Exception("File not found: " + sExportFileID));
					}
				}
				else
				{
					throw(new Exception("File not available in this session: " + sExportFileID));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.ContentType = "text/plain";
				Response.Write(ex.Message);
			}
			Response.End();
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
