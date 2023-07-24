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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Threads
{
	/// <summary>
	/// Summary description for ThreadView.
	/// </summary>
	public class ThreadView : SplendidControl
	{
		protected PlaceHolder  plcPosts;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Guid gID = Sql.ToGuid(Request["ID"]);
				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				//if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwTHREADS_POSTS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;

								// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
								Security.Filter(cmd, m_sMODULE, "list");
								cmd.CommandText += "   and THREAD_ID = @THREAD_ID" + ControlChars.CrLf;
								cmd.CommandText += " order by DATE_ENTERED asc   " + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@THREAD_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwTHREADS_POSTS", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);

										foreach ( DataRow rdr in dt.Rows )
										{
											PostView ctlPost = LoadControl("PostView.ascx") as PostView;
											plcPosts.Controls.Add(ctlPost);

											ctlPost.POST_ID       = Sql.ToGuid  (rdr["ID"              ]);
											ctlPost.TITLE         = Sql.ToString(rdr["TITLE"           ]);
											ctlPost.CREATED_BY    = Sql.ToString(rdr["CREATED_BY"      ]);
											ctlPost.DATE_ENTERED  = Sql.ToString(rdr["DATE_ENTERED"    ]);
											ctlPost.MODIFIED_BY   = Sql.ToString(rdr["MODIFIED_BY"     ]);
											ctlPost.DATE_MODIFIED = Sql.ToString(rdr["DATE_MODIFIED"   ]);
											ctlPost.DESCRIPTION   = Sql.ToString(rdr["DESCRIPTION_HTML"]);
											if ( Sql.ToDateTime(rdr["DATE_ENTERED"]) != Sql.ToDateTime(rdr["DATE_MODIFIED"]) )
												ctlPost.Modified = true;

											Guid gCREATED_BY_ID = Sql.ToGuid(rdr["CREATED_BY_ID"]);
											ctlPost.ShowEdit   = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0) || gCREATED_BY_ID == Security.USER_ID;
											ctlPost.ShowDelete = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0) || gCREATED_BY_ID == Security.USER_ID;
										}
									}
								}
							}
						}
					}
				}
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
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
			m_sMODULE = "Posts";
		}
		#endregion
	}
}
