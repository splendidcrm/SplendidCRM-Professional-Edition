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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.TeamNotices
{
	/// <summary>
	///		Summary description for MyTeamNotices.
	/// </summary>
	public class MyTeamNotices : DashletControl
	{
		protected _controls.DashletHeader  ctlDashletHeader ;

		protected UniqueStringCollection arrSelectFields;
		protected DataView vwTeamNotices;
		protected Repeater ctlRepeater ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				// 07/10/2009 Paul.  Allow the dashlet to be removed. 
				if ( e.CommandName == "Remove" )
				{
					if ( !Sql.IsEmptyString(sDetailView) )
					{
						SqlProcs.spDASHLETS_USERS_InitDisable(Security.USER_ID, sDetailView, m_sMODULE, this.AppRelativeVirtualPath.Substring(0, this.AppRelativeVirtualPath.Length-5));
						SplendidCache.ClearUserDashlets(sDetailView);
						Response.Redirect(Page.AppRelativeVirtualPath + Request.Url.Query);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				//lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			bool bEnableTeamManagement = Crm.Config.enable_team_management();
			if ( !bEnableTeamManagement )
			{
				this.Visible = false;
				return;
			}
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
					     + "  from      vwTEAM_NOTICES_MyList" + ControlChars.CrLf;
					// 09/02/2009 Paul.  Add support for dynamic teams. 
					if ( Crm.Config.enable_dynamic_teams() )
					{
						sSQL += " inner join vwTEAM_SET_MEMBERSHIPS" + ControlChars.CrLf;
						sSQL += "         on vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID = TEAM_SET_ID" + ControlChars.CrLf;
						sSQL += "        and vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_USER_ID     = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
					}
					else
					{
						sSQL += " inner join vwTEAM_MEMBERSHIPS   " + ControlChars.CrLf;
						sSQL += "         on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID" + ControlChars.CrLf;
						sSQL += "        and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
					}
					sSQL += " where @CURRENT_DATE between DATE_START and DATE_END" + ControlChars.CrLf;
					sSQL += " order by DATE_START             " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
						Sql.AddParameter(cmd, "@CURRENT_DATE", T10n.ToServerTime(DateTime.Now));

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwTeamNotices = dt.DefaultView;
								ctlRepeater.DataSource = vwTeamNotices ;
								if ( !IsPostBack )
								{
									ctlRepeater.DataBind();
								}
							}
						}
					}
				}
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDashletHeader.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Home";
			// 02/08/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("NAME");
			arrSelectFields.Add("URL");
			arrSelectFields.Add("URL_TITLE");
			arrSelectFields.Add("DESCRIPTION");
			arrSelectFields.Add("DATE_START");
		}
		#endregion
	}
}
