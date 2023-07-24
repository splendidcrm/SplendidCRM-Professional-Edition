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
using System.Collections;
using System.Web.Services;
using System.ComponentModel;
using SplendidCRM;

namespace SplendidCRM.Administration.Teams
{
	public class Team
	{
		public Guid    ID  ;
		public string  NAME;

		public Team()
		{
			ID   = Guid.Empty  ;
			NAME = String.Empty;
		}
	}

	/// <summary>
	/// Summary description for AutoComplete
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.Web.Script.Services.ScriptService]
	[ToolboxItem(false)]
	public class AutoComplete : System.Web.Services.WebService
	{
		[WebMethod(EnableSession=true)]
		public Team TEAMS_TEAM_NAME_Get(string sNAME)
		{
			Team item = new Team();
			//try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 11/25/2006 Paul.  An admin can see all teams, but most users will only see the teams which they are assigned to. 
					if ( SplendidCRM.Security.AdminUserAccess("Teams", "list") >= 0 )
					{
						sSQL = "select ID            " + ControlChars.CrLf
						     + "     , NAME          " + ControlChars.CrLf
						     + "  from vwTEAMS_List  " + ControlChars.CrLf
						     + " where 1 = 1         " + ControlChars.CrLf;
					}
					// 09/18/2017 Paul.  Allow team hierarchy. 
					else if ( !Crm.Config.enable_team_hierarchy() )
					{
						sSQL = "select ID            " + ControlChars.CrLf
						     + "     , NAME          " + ControlChars.CrLf
						     + "  from vwTEAMS_MyList" + ControlChars.CrLf
						     + " where MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
					}
					// 09/18/2017 Paul.  Allow team hierarchy. 
					else
					{
						sSQL = "select ID            " + ControlChars.CrLf
						     + "     , NAME          " + ControlChars.CrLf
						     + "  from vwTEAMS       " + ControlChars.CrLf;
						if ( Sql.IsOracle(con) )
						{
							sSQL += " inner join table(fnTEAM_HIERARCHY_MEMBERSHIPS(@MEMBERSHIP_USER_ID)) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
							sSQL += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = vwTEAMS.ID" + ControlChars.CrLf;
						}
						else
						{
							string fnPrefix = (Sql.IsSQLServer(con) ? "dbo." : String.Empty);
							sSQL += " inner join " + fnPrefix + "fnTEAM_HIERARCHY_MEMBERSHIPS(@MEMBERSHIP_USER_ID) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
							sSQL += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = vwTEAMS.ID" + ControlChars.CrLf;
						}
						sSQL += " where 1 = 1       " + ControlChars.CrLf;
					}
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						if ( !(SplendidCRM.Security.AdminUserAccess("Teams", "list") >= 0) )
							Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
						// 07/12/2010 Paul.  Allow fuzzy searching during AutoComplete. 
						Sql.AppendParameter(cmd, sNAME, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "NAME");
						// 07/02/2007 Paul.  Sort is important so that the first match is selected. 
						cmd.CommandText += " order by NAME" + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.ID   = Sql.ToGuid   (rdr["ID"  ]);
								item.NAME = Sql.ToString (rdr["NAME"]);
							}
						}
					}
				}
				if ( Sql.IsEmptyGuid(item.ID) )
				{
					string sCULTURE = Sql.ToString (Session["USER_SETTINGS/CULTURE"]);
					L10N L10n = new L10N(sCULTURE);
					throw(new Exception(L10n.Term("Teams.ERR_TEAM_NOT_FOUND")));
				}
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return item;
		}

		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		// 03/29/2007 Paul.  In order for AutoComplete to work, the parameter names must be "prefixText" and "count". 
		[WebMethod(EnableSession=true)]
		public string[] TEAMS_TEAM_NAME_List(string prefixText, int count)
		{
			string[] arrItems = new string[0];
			try
			{
				if ( !Security.IsAuthenticated() )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					// 11/25/2006 Paul.  An admin can see all teams, but most users will only see the teams which they are assigned to. 
					if ( SplendidCRM.Security.AdminUserAccess("Teams", "list") >= 0 )
					{
						sSQL = "select NAME          " + ControlChars.CrLf
						     + "  from vwTEAMS_List  " + ControlChars.CrLf
						     + " where 1 = 1         " + ControlChars.CrLf;
					}
					// 09/18/2017 Paul.  Allow team hierarchy. 
					else if ( !Crm.Config.enable_team_hierarchy() )
					{
						sSQL = "select NAME          " + ControlChars.CrLf
						     + "  from vwTEAMS_MyList" + ControlChars.CrLf
						     + " where MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
					}
					// 09/18/2017 Paul.  Allow team hierarchy. 
					else
					{
						sSQL = "select NAME          " + ControlChars.CrLf
						     + "  from vwTEAMS       " + ControlChars.CrLf;
						if ( Sql.IsOracle(con) )
						{
							sSQL += " inner join table(fnTEAM_HIERARCHY_MEMBERSHIPS(@MEMBERSHIP_USER_ID)) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
							sSQL += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = vwTEAMS.ID" + ControlChars.CrLf;
						}
						else
						{
							string fnPrefix = (Sql.IsSQLServer(con) ? "dbo." : String.Empty);
							sSQL += " inner join " + fnPrefix + "fnTEAM_HIERARCHY_MEMBERSHIPS(@MEMBERSHIP_USER_ID) vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
							sSQL += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = vwTEAMS.ID" + ControlChars.CrLf;
						}
						sSQL += " where 1 = 1       " + ControlChars.CrLf;
					}
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						if ( !(SplendidCRM.Security.AdminUserAccess("Teams", "list") >= 0) )
							Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
						// 07/12/2010 Paul.  Allow fuzzy searching during AutoComplete. 
						Sql.AppendParameter(cmd, prefixText, (Sql.ToBoolean(Application["CONFIG.AutoComplete.Contains"]) ? Sql.SqlFilterMode.Contains : Sql.SqlFilterMode.StartsWith), "NAME");
						cmd.CommandText += " order by NAME" + ControlChars.CrLf;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(0, count, dt);
								arrItems = new string[dt.Rows.Count];
								for ( int i=0; i < dt.Rows.Count; i++ )
									arrItems[i] = Sql.ToString(dt.Rows[i]["NAME"]);
							}
						}
					}
				}
			}
			catch
			{
			}
			return arrItems;
		}

		// 09/03/2009 Paul.  The list can be retrived for the base module, or for a ModulePopup, 
		// so the field name can be NAME or TEAM_NAME. 
		[WebMethod(EnableSession=true)]
		public string[] TEAMS_NAME_List(string prefixText, int count)
		{
			return TEAMS_TEAM_NAME_List(prefixText, count);
		}

		// 04/28/2016 Paul.  Add parent team and custom fields. 
		[WebMethod(EnableSession=true)]
		public string[] TEAMS_PARENT_NAME_List(string prefixText, int count)
		{
			return TEAMS_TEAM_NAME_List(prefixText, count);
		}
	}
}

