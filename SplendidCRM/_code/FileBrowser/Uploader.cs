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
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Diagnostics;

namespace SplendidCRM.FileBrowser
{
	public class Uploader : SplendidPage
	{
		// 02/11/2009 Paul.  This page must be accessible without authentication. 
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			Response.CacheControl    = "no-cache";

			string sCustomMsg = String.Empty;
			string sFileURL   = String.Empty;
			try
			{
				if ( !Security.IsAuthenticated() )
				{
					sCustomMsg = "Authentication is required.";
				}
				else
				{
					Guid   gImageID  = Guid.Empty;
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
						using ( IDbTransaction trn = Sql.BeginTransaction(con) )
						{
							try
							{
								string sFileName = String.Empty;
								FileWorkerUtils.LoadImage(ref gImageID, ref sFileName, trn);
								if ( Sql.IsEmptyGuid(gImageID) )
								{
									sCustomMsg = "Failed to upload message.";
								}
								else
								{
									sFileURL = Utils.MassEmailerSiteURL(Context.Application) + "Images/EmailImage.aspx?ID=" + gImageID.ToString();
								}
								trn.Commit();
							}
							catch
							{
								trn.Rollback();
								throw;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				sCustomMsg = ex.Message;
			}
			// 04/26/2012 Paul.  CKEditor 3.6.2 has a new technique for returning the uploaded image. 
			// http://stackoverflow.com/questions/9720734/image-upload-on-ckeditor-asp-net-4-response-to-upload-iframe-error
			Response.Write("<script type=\"text/javascript\">\n");
			Response.Write("window.parent.CKEDITOR.tools.callFunction(" + Request["CKEditorFuncNum"] + ",'" + Sql.EscapeJavaScript(sFileURL) + "','" + Sql.EscapeJavaScript(sCustomMsg) + "');\n");
			Response.Write("</script>\n");
			return;
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

