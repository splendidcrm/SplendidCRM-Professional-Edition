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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;
using Microsoft.Reporting.WebForms;

namespace SplendidCRM.Reports
{
	/// <summary>
	/// Summary description for Render.
	/// </summary>
	public class Render : SplendidPage
	{
		protected Guid   gID       ;
		protected string sReportSQL;

		override protected bool AuthenticationRequired()
		{
			// 02/02/2010 Paul.  Use a flag to control if reports can be rendered externally. 
			if ( Sql.ToBoolean(Application["CONFIG.enable_external_reports"]) )
				return false;
			else
				return true;
		}

		// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
		// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
		public void RunReport(Guid gREPORT_ID, string sRDL, string sMODULE_NAME, string sFILENAME)
		{
			try
			{
				ReportViewer rdlViewer = new ReportViewer();
				// 01/24/2010 Paul.  Pass the context so that it can be used in the Validation call. 
				// 12/04/2010 Paul.  L10n is needed by the Rules Engine to allow translation of list terms. 
				// 04/13/2011 Paul.  A scheduled report does not have a Session, so we need to create a session using the same approach used for ExchangeSync. 
				// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
				// 03/24/2016 Paul.  We need an alternate way to provide parameters to render a report with a signature. 
				RdlUtil.LocalLoadReportDefinition(this.Context, null, null, L10n, T10n, rdlViewer, gREPORT_ID, sRDL, sMODULE_NAME, Guid.Empty, out sReportSQL);

				string    sMimeType   ;
				string    sEncoding   ;
				string    sExtension  ;
				string[]  arrStreamIDs;
				Warning[] warnings    ;
				// http://msdn2.microsoft.com/en-us/library/ms251839(VS.80).aspx
				string sRENDER_FORMAT = Sql.ToString(Request["RENDER_FORMAT"]);  // Excel, PDF, Image
				switch ( sRENDER_FORMAT.ToUpper() )
				{
					// 02/27/2013 Paul.  Word format is supported by ReportViewer 2012. 
					// http://msdn.microsoft.com/en-us/library/ms251671(v=vs.110).aspx
					// 09/13/2016 Paul.  Possible render formats "Excel" "EXCELOPENXML" "IMAGE" "PDF" "WORD" "WORDOPENXML". 
					// http://stackoverflow.com/questions/3494009/creating-a-custom-export-to-excel-for-reportviewer-rdlc
					// 05/07/2018 Paul.  Include all possible values. 
					case "WORD"        :  sRENDER_FORMAT = "WORDOPENXML" ;  break;
					case "WORDOPENXML" :  sRENDER_FORMAT = "WORDOPENXML" ;  break;
					case "EXCEL"       :  sRENDER_FORMAT = "EXCELOPENXML";  break;
					case "EXCELOPENXML":  sRENDER_FORMAT = "EXCELOPENXML";  break;
					case "IMAGE"       :  sRENDER_FORMAT = "Image"       ;  break;
					case "PDF"         :  sRENDER_FORMAT = "PDF"         ;  break;
					default            :  sRENDER_FORMAT = "PDF"         ;  break;
				}
				byte[] byPDF = rdlViewer.LocalReport.Render(sRENDER_FORMAT, "<DeviceInfo></DeviceInfo>", out sMimeType, out sEncoding, out sExtension, out arrStreamIDs, out warnings);
				Response.ContentType = sMimeType;
				// 08/06/2008 yxy21969.  Make sure to encode all URLs.
				// 12/20/2009 Paul.  Use our own encoding so that a space does not get converted to a +. 
				// 06/27/2010 Paul.  For Quotes, Orders and Invoices, we want to use the name of the records.  Otherwise, use the report name. 
				if ( Sql.IsEmptyString(sFILENAME) )
					sFILENAME = rdlViewer.LocalReport.DisplayName;
				// 08/22/2010 Paul.  Provide a way to disable the forced attachment. 
				if ( !Sql.ToBoolean(Request["DisableContentDisposition"]) )
					Response.AddHeader("Content-Disposition", "attachment;filename=" + Utils.ContentDispositionEncode(Request.Browser, sFILENAME + "." + sExtension));
				Response.OutputStream.Write(byPDF, 0, byPDF.Length);
				// 05/06/2009 Paul.  Not sure why we are getting an exception, but it does not seem to affect the output. 
				// Unable to evaluate expression because the code is optimized or a native frame is on top of the call stack.
				Response.Flush();
				//Response.End();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// 05/10/2009 Paul.  If there is an error, it is useful to dump the error. 
				// 08/17/2024 Paul.  The correct MIME type is text/plain. 
				Response.ContentType = "text/plain";
				Response.Clear();
				// 01/24/2010 Paul.  HTML should not be used in a text error. 
				string sError = Utils.ExpandException(ex);
				sError = sError.Replace("<br />" + ControlChars.CrLf, "  ");
				Response.Write(sError);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			try
			{
				if ( !IsPostBack )
				{
					gID = Sql.ToGuid(Request["ID"]);
					if ( !Sql.IsEmptyGuid(gID) )
					{
						string sRDL         = String.Empty;
						// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
						string sMODULE_NAME = String.Empty;
						string sFILENAME    = String.Empty;
						// 04/06/2011 Paul.  Cache reports. 
						DataTable dtReport = SplendidCache.Report(gID);
						if ( dtReport.Rows.Count > 0 )
						{
							DataRow rdr = dtReport.Rows[0];
							sRDL = Sql.ToString(rdr["RDL"]);
							// 01/19/2010 Paul.  The Module Name is needed in order to apply ACL Field Security. 
							sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
						}
						try
						{
							// 06/27/2010 Paul.  For Quotes, Orders and Invoices, we want to use the name of the records.  Otherwise, use the report name. 
							if ( sMODULE_NAME == "Quotes" || sMODULE_NAME == "Orders" || sMODULE_NAME == "Invoices" )
							{
								string sMODULE_FIELD_ID   = String.Empty;
								// 06/27/2010 Paul.  Use new TableName function. 
								string sMODULE_TABLE_NAME = Crm.Modules.TableName(Application, sMODULE_NAME);
								if ( sMODULE_TABLE_NAME.EndsWith("IES") )
									sMODULE_FIELD_ID = sMODULE_TABLE_NAME.Substring(0, sMODULE_TABLE_NAME.Length - 3) + "Y_ID";
								else if ( sMODULE_TABLE_NAME.EndsWith("S") )
									sMODULE_FIELD_ID = sMODULE_TABLE_NAME.Substring(0, sMODULE_TABLE_NAME.Length - 1) + "_ID";
								else
									sMODULE_FIELD_ID = sMODULE_TABLE_NAME + "_ID";
								// 01/05/2016 Paul.  Source could be multiple IDs, so we need to first check if a single ID. 
								Guid gSOURCE_ID = Guid.Empty;
								string[] arrSOURCE_IDs = Request.QueryString.GetValues(sMODULE_FIELD_ID);
								if ( arrSOURCE_IDs != null && arrSOURCE_IDs.Length == 1 )
									gSOURCE_ID = Sql.ToGuid(arrSOURCE_IDs[0]);
								
								if ( !Sql.IsEmptyGuid(gSOURCE_ID) )
								{
									DbProviderFactory dbf = DbProviderFactories.GetFactory();
									using ( IDbConnection con = dbf.CreateConnection() )
									{
										con.Open();
										string sSQL;
										sSQL = "select *                    "   + ControlChars.CrLf
										     + "  from vw" + sMODULE_TABLE_NAME + ControlChars.CrLf
										     + " where ID = @ID             "   + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Sql.AddParameter(cmd, "@ID", gSOURCE_ID);
											using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
											{
												if ( rdr.Read() )
												{
													DateTime dtDATE_MODIFIED = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
													string   sNAME           = Sql.ToString  (rdr["NAME"         ]);
													string   sNUMBER         = String.Empty;
													switch ( sMODULE_TABLE_NAME )
													{
														// 06/27/2010 Paul.  The # gets converted to a _ in a filename, so lets just remove it. 
														case "QUOTES"  :  sNUMBER = " " + Sql.ToString(rdr["QUOTE_NUM"  ]);  break;
														case "ORDERS"  :  sNUMBER = " " + Sql.ToString(rdr["ORDER_NUM"  ]);  break;
														case "INVOICES":  sNUMBER = " " + Sql.ToString(rdr["INVOICE_NUM"]);  break;
													}
													sFILENAME = sMODULE_NAME + sNUMBER + " - " + sNAME;
												}
											}
										}
									}
								}
							}
							if ( !Sql.IsEmptyString(sRDL) )
							{
								// 10/06/2012 Paul.  REPORT_ID is needed for sub-report caching. 
								RunReport(gID, sRDL, sMODULE_NAME, sFILENAME);
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							Response.Write(ex.Message);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
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
