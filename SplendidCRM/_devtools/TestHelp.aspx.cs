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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for TestHelp.
	/// </summary>
	public class TestHelp : SplendidPage
	{
		protected DataTable dtMain    ;
		protected Label     lblCurrent;
		protected Label     lblStatus ;
		protected Label     lblErrors ;
		protected ListBox   lstFiles  ;
		protected DataGrid  grdMain   ;
		protected StringBuilder sbHelpScripts;

		void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !(SplendidCRM.Security.AdminUserAccess("Administration", "access") >= 0) )
				return;

			try
			{
				sbHelpScripts = new StringBuilder();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select vwTERMINOLOGY_HELP.NAME                                    " + ControlChars.CrLf
					     + "     , vwTERMINOLOGY_HELP.MODULE_NAME                             " + ControlChars.CrLf
					     + "     , vwTERMINOLOGY_HELP.DISPLAY_TEXT                            " + ControlChars.CrLf
					     + "     , vwMODULES.IS_ADMIN                                         " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY_HELP                                         " + ControlChars.CrLf
					     + " inner join vwMODULES                                             " + ControlChars.CrLf
					     + "         on vwMODULES.MODULE_NAME = vwTERMINOLOGY_HELP.MODULE_NAME" + ControlChars.CrLf
					     + " where vwTERMINOLOGY_HELP.LANG    = @LANG                         " + ControlChars.CrLf
					     + " order by vwMODULES.IS_ADMIN asc, vwTERMINOLOGY_HELP.MODULE_NAME, vwTERMINOLOGY_HELP.NAME asc" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LANG", L10n.NAME);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								//dt.Columns.Add("HELP_SCRIPT");
								foreach ( DataRow row in dt.Rows )
								{
									row["NAME"] = Sql.ToString(row["MODULE_NAME"]) + "." + Sql.ToString(row["NAME"]);
									string sDISPLAY_TEXT = Sql.ToString(row["DISPLAY_TEXT"]);
									int nStartScript = sDISPLAY_TEXT.IndexOf("<script type=\"text/javascript\">");
									if ( nStartScript >= 0 )
									{
										string sEndScript = "</script>";
										int nEndScript = sDISPLAY_TEXT.IndexOf(sEndScript, nStartScript);
										if ( nEndScript >= 0 )
										{
											string sHELP_SCRIPT  = sDISPLAY_TEXT.Substring(nStartScript, nEndScript + sEndScript.Length - nStartScript);
											//row["HELP_SCRIPT"] = sHELP_SCRIPT;
											sbHelpScripts.Append(sHELP_SCRIPT);
										}
									}
								}
								dt.AcceptChanges();
								
								DataView vwMain = new DataView(dt);
								vwMain.Sort = "IS_ADMIN asc, NAME asc";
								lstFiles.DataSource = vwMain;
								lstFiles.DataBind();
								//grdMain.DataSource = vwMain;
								//grdMain.DataBind();
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				lblStatus.Text = ex.Message;
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
