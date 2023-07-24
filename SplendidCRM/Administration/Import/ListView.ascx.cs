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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Xml;

namespace SplendidCRM.Administration.Import
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected Label         lblError       ;
		protected HtmlInputFile fileIMPORT     ;
		protected CheckBox      chkTruncate    ;
		protected Literal       lblImportErrors;
		protected RequiredFieldValidator reqFILENAME;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Next" )
			{
				if ( Page.IsValid )
				{
					try
					{
						HttpPostedFile pstIMPORT = fileIMPORT.PostedFile;
						if ( pstIMPORT != null )
						{
							if ( pstIMPORT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstIMPORT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstIMPORT.ContentType;
								if ( sFILE_MIME_TYPE == "text/xml" )
								{
									using ( MemoryStream mstm = new MemoryStream() )
									{
										using ( BinaryWriter mwtr = new BinaryWriter(mstm) )
										{
											using ( BinaryReader reader = new BinaryReader(pstIMPORT.InputStream) )
											{
												byte[] binBYTES = reader.ReadBytes(8*1024);
												while ( binBYTES.Length > 0 )
												{
													for(int i=0; i < binBYTES.Length; i++ )
													{
														// MySQL dump seems to dump binary 0 & 1 for byte values. 
														if ( binBYTES[i] == 0 )
															mstm.WriteByte(Convert.ToByte('0'));
														else if ( binBYTES[i] == 1 )
															mstm.WriteByte(Convert.ToByte('1'));
														else
															mstm.WriteByte(binBYTES[i]);
													}
													binBYTES = reader.ReadBytes(8*1024);
												}
											}
											mwtr.Flush();
											mstm.Seek(0, SeekOrigin.Begin);
											XmlDocument xml = new XmlDocument();
											// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
											// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
											// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
											xml.XmlResolver = null;
											xml.Load(mstm);
											try
											{
												// 09/30/2006 Paul.  Clear any previous error. 
												lblImportErrors.Text = "";
												SplendidImport.Import(xml, null, chkTruncate.Checked);
											}
											catch(Exception ex)
											{
												lblImportErrors.Text = ex.Message;
											}
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("Administration.LBL_IMPORT_DATABASE_ERROR")));
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
						return;
					}
				}
			}
			else if ( e.CommandName == "Back" )
			{
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Administration.LBL_MODULE_NAME"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			// 07/02/2006 Paul.  The required fields need to be bound manually. 
			reqFILENAME.DataBind();
			// 12/17/2005 Paul.  Don't buffer so that the connection can be kept alive. 
			Response.BufferOutput = false;
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			m_sMODULE = "Import";
			// 05/06/2010 Paul.  The menu will show the admin Module Name in the Six theme. 
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}

