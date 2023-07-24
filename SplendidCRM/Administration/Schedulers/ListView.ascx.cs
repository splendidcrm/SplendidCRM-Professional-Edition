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
using System.Xml;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.Schedulers
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Schedulers.Delete" )
				{
					Guid gID = Sql.ToGuid(e.CommandArgument);
					SqlProcs.spSCHEDULERS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Schedulers.Run" )
				{
					string sJOB = Sql.ToString(e.CommandArgument);
					SchedulerUtils.RunJob(HttpContext.Current, sJOB);
					// 01/20/2009 Paul.  Update last run. 
					vwMain.RowFilter = "JOB = '" + Sql.EscapeSQL(sJOB) + "'";
					if ( vwMain.Count > 0 )
					{
						Guid gID = Sql.ToGuid(vwMain[0]["ID"]);
						SqlProcs.spSCHEDULERS_UpdateLastRun(gID, DateTime.Now);
					}
					vwMain.RowFilter = String.Empty;
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("~/Administration/default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = Server.HtmlEncode(ex.Message);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.BufferOutput = true;
			SetPageTitle(L10n.Term("Schedulers.LBL_MODULE_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *               " + ControlChars.CrLf
					     + "     , '' as DATE_RANGE" + ControlChars.CrLf
					     + "  from vwSCHEDULERS    " + ControlChars.CrLf
					     + " where 1 = 1           " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string   sJOB_INTERVAL     = Sql.ToString  (row["JOB_INTERVAL"   ]);
									DateTime dtDATE_TIME_START = Sql.ToDateTime(row["DATE_TIME_START"]);
									DateTime dtDATE_TIME_END   = Sql.ToDateTime(row["DATE_TIME_END"  ]);
									DateTime dtLAST_RUN        = Sql.ToDateTime(row["LAST_RUN"       ]);
									row["JOB_INTERVAL"] = sJOB_INTERVAL + "<br>" + SchedulerUtils.CronDescription(L10n, sJOB_INTERVAL);
									if ( dtDATE_TIME_START != DateTime.MinValue )
										row["DATE_RANGE"] = T10n.FromServerTime(dtDATE_TIME_START).ToString() + "-";
									if ( dtDATE_TIME_END == DateTime.MinValue )
										row["DATE_RANGE"] += L10n.Term("Schedulers.LBL_PERENNIAL");
									else
										row["DATE_RANGE"] += T10n.FromServerTime(dtDATE_TIME_END).ToString();
									if ( dtLAST_RUN != DateTime.MinValue )
										row["LAST_RUN"] = T10n.FromServerTime(dtLAST_RUN);
									row["STATUS"] = L10n.Term(".scheduler_status_dom.", row["STATUS"]);
								}
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( !IsPostBack )
								{
									grdMain.SortColumn = "NAME";
									grdMain.SortOrder  = "asc" ;
									grdMain.ApplySort();
									grdMain.DataBind();
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
			m_sMODULE = "Schedulers";
			// 05/06/2010 Paul.  The menu will show the admin Module Name in the Six theme. 
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}

