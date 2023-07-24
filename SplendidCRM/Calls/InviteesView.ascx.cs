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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Calls
{
	/// <summary>
	///		Summary description for InviteesView.
	/// </summary>
	public class InviteesView : SplendidControl
	{
		protected DataView           vwMain         ;
		protected SplendidGrid       grdMain        ;
		protected Label              lblError       ;
		protected HtmlGenericControl divInvitees    ;
		protected SearchInvitees     ctlSearch      ;
		protected string[]           arrINVITEES    ;

		public CommandEventHandler Command ;

		// 07/23/2014 Paul.  Add meeting times so that we can display current user status. 
		protected DateTime           dtDATE_START = DateTime.MinValue;
		protected DateTime           dtDATE_END   = DateTime.MaxValue;

		public DateTime DATE_START
		{
			get
			{
				return dtDATE_START;
			}
			set
			{
				dtDATE_START = value;
			}
		}
		
		public DateTime DATE_END
		{
			get
			{
				return dtDATE_END;
			}
			set
			{
				dtDATE_END = value;
			}
		}

		public string[] INVITEES
		{
			get
			{
				return arrINVITEES;
			}
			set
			{
				arrINVITEES = value;
			}
		}

		public bool IsExistingInvitee(string sINVITEE_ID)
		{
			if ( arrINVITEES != null )
			{
				foreach(string s in arrINVITEES)
				{
					if ( s == sINVITEE_ID )
						return true;
				}
			}
			return false;
		}
		
		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					if ( Command != null )
						Command(this, e) ;
					// 04/11/2013 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					BindInvitees();
					ViewState["InviteesSearch"] = true;
				}
				else if ( e.CommandName == "Invitees.Add" )
				{
					if ( Command != null )
						Command(this, e) ;
					BindInvitees();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void BindInvitees()
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						//string sSQL;
						//sSQL = "select *                                                               " + ControlChars.CrLf
						//     + "  from vwACTIVITIES_Invitees                                           " + ControlChars.CrLf
						//     + " where (INVITEE_TYPE = 'Users' or ASSIGNED_USER_ID = @ASSIGNED_USER_ID)" + ControlChars.CrLf;
						
						// 04/01/2012 Paul.  Add Calls/Leads relationship. 
						// 04/01/2012 Paul.  Can't use vwACTIVITIES_Invitees as it will not apply Team Management rules. 
						// We need to use Security.Filter() for Contacts and Leads. 
						bool bTeamFilter = Crm.Config.enable_team_management();
						if ( bTeamFilter )
						{
							cmd.CommandText += "select ID          as ID                   " + ControlChars.CrLf;
							cmd.CommandText += "     , N'Users'    as INVITEE_TYPE         " + ControlChars.CrLf;
							cmd.CommandText += "     , FULL_NAME   as NAME                 " + ControlChars.CrLf;
							cmd.CommandText += "     , FIRST_NAME  as FIRST_NAME           " + ControlChars.CrLf;
							cmd.CommandText += "     , LAST_NAME   as LAST_NAME            " + ControlChars.CrLf;
							cmd.CommandText += "     , EMAIL1      as EMAIL                " + ControlChars.CrLf;
							cmd.CommandText += "     , PHONE_WORK  as PHONE                " + ControlChars.CrLf;
							cmd.CommandText += "     , null        as ASSIGNED_USER_ID     " + ControlChars.CrLf;
							cmd.CommandText += "  from vwTEAMS_ASSIGNED_TO_List            " + ControlChars.CrLf;
							cmd.CommandText += " where MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
						}
						else
						{
							cmd.CommandText += "select ID          as ID                   " + ControlChars.CrLf;
							cmd.CommandText += "     , N'Users'    as INVITEE_TYPE         " + ControlChars.CrLf;
							cmd.CommandText += "     , FULL_NAME   as NAME                 " + ControlChars.CrLf;
							cmd.CommandText += "     , FIRST_NAME  as FIRST_NAME           " + ControlChars.CrLf;
							cmd.CommandText += "     , LAST_NAME   as LAST_NAME            " + ControlChars.CrLf;
							cmd.CommandText += "     , EMAIL1      as EMAIL                " + ControlChars.CrLf;
							cmd.CommandText += "     , PHONE_WORK  as PHONE                " + ControlChars.CrLf;
							cmd.CommandText += "     , null        as ASSIGNED_USER_ID     " + ControlChars.CrLf;
							cmd.CommandText += "  from vwUSERS_ASSIGNED_TO_List            " + ControlChars.CrLf;
							cmd.CommandText += " where 1 = 1                               " + ControlChars.CrLf;
						}
						ctlSearch.SqlSearchClause(cmd);
						
						cmd.CommandText += "union all                                  " + ControlChars.CrLf;
						cmd.CommandText += "select ID               as ID              " + ControlChars.CrLf;
						cmd.CommandText += "     , N'Contacts'      as INVITEE_TYPE    " + ControlChars.CrLf;
						cmd.CommandText += "     , NAME             as NAME            " + ControlChars.CrLf;
						cmd.CommandText += "     , FIRST_NAME       as FIRST_NAME      " + ControlChars.CrLf;
						cmd.CommandText += "     , LAST_NAME        as LAST_NAME       " + ControlChars.CrLf;
						cmd.CommandText += "     , EMAIL1           as EMAIL           " + ControlChars.CrLf;
						cmd.CommandText += "     , PHONE_WORK       as PHONE           " + ControlChars.CrLf;
						cmd.CommandText += "     , ASSIGNED_USER_ID as ASSIGNED_USER_ID" + ControlChars.CrLf;
						cmd.CommandText += "  from vwCONTACTS                          " + ControlChars.CrLf;
						Security.Filter(cmd, "Contacts", "list");
						cmd.CommandText += "   and EMAIL1 is not null                  " + ControlChars.CrLf;
						ctlSearch.SqlSearchClause(cmd);
						
						cmd.CommandText += "union all                                  " + ControlChars.CrLf;
						cmd.CommandText += "select ID               as ID              " + ControlChars.CrLf;
						cmd.CommandText += "     , N'Leads'         as INVITEE_TYPE    " + ControlChars.CrLf;
						cmd.CommandText += "     , NAME             as NAME            " + ControlChars.CrLf;
						cmd.CommandText += "     , FIRST_NAME       as FIRST_NAME      " + ControlChars.CrLf;
						cmd.CommandText += "     , LAST_NAME        as LAST_NAME       " + ControlChars.CrLf;
						cmd.CommandText += "     , EMAIL1           as EMAIL           " + ControlChars.CrLf;
						cmd.CommandText += "     , PHONE_WORK       as PHONE           " + ControlChars.CrLf;
						cmd.CommandText += "     , ASSIGNED_USER_ID as ASSIGNED_USER_ID" + ControlChars.CrLf;
						cmd.CommandText += "  from vwLEADS                             " + ControlChars.CrLf;
						Security.Filter(cmd, "Leads", "list");
						cmd.CommandText += "   and EMAIL1 is not null" + ControlChars.CrLf;
						ctlSearch.SqlSearchClause(cmd);
						
						cmd.CommandText += " order by INVITEE_TYPE desc, LAST_NAME asc, FIRST_NAME asc" + ControlChars.CrLf;

						if ( bDebug )
							RegisterClientScriptBlock("vwACTIVITIES_Invitees", Sql.ClientScriptBlock(cmd));

						try
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									vwMain = dt.DefaultView;
									// 04/11/2013 Paul.  Watch for search while paginated as it can cause the CurrentPageIndex to be out of range. 
									if ( grdMain.CurrentPageIndex > vwMain.Count / grdMain.PageSize )
										grdMain.CurrentPageIndex = 0;
									grdMain.DataSource = vwMain ;
									grdMain.DataBind();
									divInvitees.Visible = true;
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							lblError.Text = ex.Message;
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

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( Sql.ToBoolean(ViewState["InviteesSearch"]) )
				BindInvitees();
			//if ( !IsPostBack )
			{
				// 02/21/2006 Paul.  Don't DataBind, otherwise it will cause the DropDownLists to loose their selected value. 
				// grdMain.DataBind() does not work.  Force the data bind by using L10nTranslate(). 
				grdMain.L10nTranslate();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This Contact is required by the ASP.NET Web Form Designer.
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
			ctlSearch.Command += new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}

