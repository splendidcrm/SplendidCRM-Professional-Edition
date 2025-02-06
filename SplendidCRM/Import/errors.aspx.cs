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
	/// Summary description for Errors.
	/// </summary>
	public class Errors : SplendidPage
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string sSourceType        = Sql.ToString(Request["SourceType"     ]);
				string sProcessedFileID   = Sql.ToString(Request["ProcessedFileID"]);
				string sProcessedFileName = Sql.ToString(Session["TempFile." + sProcessedFileID]);
				string sProcessedPathName = Path.Combine(Path.GetTempPath(), sProcessedFileName);
				if ( File.Exists(sProcessedPathName) )
				{
					DataSet dsProcessed = new DataSet();
					dsProcessed.ReadXml(sProcessedPathName);
					if ( dsProcessed.Tables.Count == 1 )
					{
						DataTable dt = dsProcessed.Tables[0];
						if ( dt.Rows.Count > 0 )
						{
							for ( int i = dt.Rows.Count - 1; i >= 0; i-- )
							{
								DataRow row = dt.Rows[i];
								if ( Sql.ToBoolean(row["IMPORT_ROW_STATUS"]) )
									row.Delete();
							}
							dt.AcceptChanges();
							
							if ( sSourceType == "other" || sSourceType == "custom_delimited" )
							{
								Response.ContentType = "text/csv";
								Response.AddHeader("Content-Disposition", "attachment;filename=import_errors.csv");
								SplendidExport.ExportDelimited(Response.OutputStream, new DataView(dt), "", 0, dt.Rows.Count, ',' );
							}
							else if ( sSourceType == "other_tab" )
							{
								// 08/17/2024 Paul.  The correct MIME type is text/plain. 
								Response.ContentType = "text/plain";
								Response.AddHeader("Content-Disposition", "attachment;filename=import_errors.txt");
								SplendidExport.ExportDelimited(Response.OutputStream, new DataView(dt), "", 0, dt.Rows.Count, '\t' );
							}
							else  // excel
							{
								Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";  //"application/vnd.ms-excel";
								Response.AddHeader("Content-Disposition", "attachment;filename=import_errors.xlsx");
								SplendidExport.ExportExcelOpenXML(Response.OutputStream, new DataView(dt), "", 0, dt.Rows.Count);
							}
						}
						else
						{
							throw(new Exception(L10n.Term("Import.ERR_NO_ERRORS")));
						}
					}
					else
					{
						throw(new Exception(L10n.Term("Import.ERR_NO_PROCESSED_TABLE")));
					}
				}
				else
				{
					throw(new Exception(L10n.Term("Import.ERR_NO_PROCESSED_FILE") + " " + sProcessedFileID));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.ContentType = "text/plain";
				Response.AddHeader("Content-Disposition", "attachment;filename=export_errors.txt");
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
