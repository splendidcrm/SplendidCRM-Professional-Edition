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

namespace SplendidCRM.Administration
{
	/// <summary>
	///		Summary description for UsersView.
	/// </summary>
	public class UsersView : SplendidControl
	{
		protected Label              lblError              ;
		protected HtmlGenericControl spnDynamicTeamsButtons;
		protected HtmlGenericControl spnDynamicTeamsMessage;
		// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
		protected HtmlGenericControl spnDynamicAssignmentButtons;
		protected HtmlGenericControl spnDynamicAssignmentMessage;

		public static void ASSIGNED_SETS_InitAllModules(object o)
		{
			HttpContext Context = o as HttpContext;
			HttpApplicationState Application = Context.Application;
			SplendidError.SystemMessage(Context, "Log", new StackTrace(true).GetFrame(0), "ASSIGNED_SETS_InitAllModules: Begin updating all modules to Dynamic Assignment.");
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							// 11/30/2011 Paul.  We need to use the command object so that we can increase the timeout. 
							using ( IDbCommand cmdASSIGNED_SETS_InitAllModules = SqlProcs.cmdASSIGNED_SETS_InitAllModules(con) )
							{
								cmdASSIGNED_SETS_InitAllModules.Transaction    = trn;
								cmdASSIGNED_SETS_InitAllModules.CommandTimeout = 0;
								cmdASSIGNED_SETS_InitAllModules.ExecuteNonQuery();
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
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
			}
			SplendidError.SystemMessage(Context, "Log", new StackTrace(true).GetFrame(0), "ASSIGNED_SETS_InitAllModules: Done updating all modules to Dynamic Assignment.");
		}

		public static void TEAM_SETS_InitAllModules(object o)
		{
			HttpContext Context = o as HttpContext;
			HttpApplicationState Application = Context.Application;
			SplendidError.SystemMessage(Context, "Log", new StackTrace(true).GetFrame(0), "TEAM_SETS_InitAllModules: Begin updating all modules to Dynamic Assignment.");
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							// 02/27/2020 Paul.  We need to use the command object so that we can increase the timeout. 
							using ( IDbCommand cmdTEAM_SETS_InitAllModules = SqlProcs.cmdTEAM_SETS_InitAllModules(con) )
							{
								cmdTEAM_SETS_InitAllModules.Transaction    = trn;
								cmdTEAM_SETS_InitAllModules.CommandTimeout = 0;
								cmdTEAM_SETS_InitAllModules.ExecuteNonQuery();
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
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
			}
			SplendidError.SystemMessage(Context, "Log", new StackTrace(true).GetFrame(0), "TEAM_SETS_InitAllModules: Done updating all modules to Dynamic Assignment.");
		}

		// 09/11/2007 Paul.  Provide quick access to team management flags. 
		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Teams.Enable"   )
				{
					SqlProcs.spCONFIG_Update("system", "enable_team_management", "true");
					Application["CONFIG.enable_team_management"] = true;
					// 09/21/2009 Paul.  When teams are enabled, we need to refresh the memberships. 
					SqlProcs.spTEAM_MEMBERSHIPS_RefreshAll();
				}
				else if ( e.CommandName == "Teams.Disable"  )
				{
					SqlProcs.spCONFIG_Update("system", "enable_team_management", "false");
					Application["CONFIG.enable_team_management"] = false;
				}
				else if ( e.CommandName == "Teams.Require"  )
				{
					SqlProcs.spCONFIG_Update("system", "require_team_management", "true");
					Application["CONFIG.require_team_management"] = true;
				}
				else if ( e.CommandName == "Teams.Optional" )
				{
					SqlProcs.spCONFIG_Update("system", "require_team_management", "false");
					Application["CONFIG.require_team_management"] = false;
				}
				// 08/29/2009 Paul.  Allow Dynamic Teams to be turned on or off. 
				else if ( e.CommandName == "Teams.Dynamic"  )
				{
					SqlProcs.spCONFIG_Update("system", "enable_dynamic_teams", "true");
					Application["CONFIG.enable_dynamic_teams"] = true;
					lblError.Text = L10n.Term("Users.LBL_UPDATING_MODULES_DYNAMIC_TEAMS");
					// 02/27/2020 Paul.  When enabling, we need to update any records that have TEAM_ID not null and TEAM_SET_ID null across all modules. 
					System.Threading.Thread t = new System.Threading.Thread(TEAM_SETS_InitAllModules);
					t.Start(this.Context);
					// 12/03/2017 Paul.  Return to prevent redirect. 
					Page.DataBind();
				}
				else if ( e.CommandName == "Teams.Singular" )
				{
					SqlProcs.spCONFIG_Update("system", "enable_dynamic_teams", "false");
					Application["CONFIG.enable_dynamic_teams"] = false;
				}
				// 01/01/2008 Paul.  We need a quick way to require user assignments across the system. 
				else if ( e.CommandName == "UserAssignement.Require"  )
				{
					SqlProcs.spCONFIG_Update("system", "require_user_assignment", "true");
					Application["CONFIG.require_user_assignment"] = true;
				}
				else if ( e.CommandName == "UserAssignement.Optional" )
				{
					SqlProcs.spCONFIG_Update("system", "require_user_assignment", "false");
					Application["CONFIG.require_user_assignment"] = false;
				}
				// 04/10/2009 Paul.  Make it easy to enable and disable admin delegation. 
				else if ( e.CommandName == "AdminDelegation.Enable"   )
				{
					SqlProcs.spCONFIG_Update("system", "allow_admin_roles", "true");
					Application["CONFIG.allow_admin_roles"] = true;
				}
				else if ( e.CommandName == "AdminDelegation.Disable"  )
				{
					SqlProcs.spCONFIG_Update("system", "allow_admin_roles", "false");
					Application["CONFIG.allow_admin_roles"] = false;
				}
				else if ( e.CommandName == "Teams.Hierarchical"   )
				{
					SqlProcs.spCONFIG_Update("system", "enable_team_hierarchy", "true");
					Application["CONFIG.enable_team_hierarchy"] = true;
				}
				else if ( e.CommandName == "Teams.NonHierarchical"  )
				{
					SqlProcs.spCONFIG_Update("system", "enable_team_hierarchy", "false");
					Application["CONFIG.enable_team_hierarchy"] = false;
				}
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				else if ( e.CommandName == "UserAssignement.Dynamic"  )
				{
					SqlProcs.spCONFIG_Update("system", "enable_dynamic_assignment", "true");
					Application["CONFIG.enable_dynamic_assignment"] = true;
					lblError.Text = L10n.Term("Users.LBL_UPDATING_MODULES_DYNAMIC_ASSIGNMENT");
					// 11/30/2017 Paul.  When enabling, we need to update any records that have ASSIGNED_USER_ID not null and ASSIGNED_SET_ID null across all modules. 
					System.Threading.Thread t = new System.Threading.Thread(ASSIGNED_SETS_InitAllModules);
					t.Start(this.Context);
					// 12/03/2017 Paul.  Return to prevent redirect. 
					Page.DataBind();
					return;
				}
				else if ( e.CommandName == "UserAssignement.Singular" )
				{
					SqlProcs.spCONFIG_Update("system", "enable_dynamic_assignment", "false");
					Application["CONFIG.enable_dynamic_assignment"] = false;
				}
				Response.Redirect("default.aspx");
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				// 09/22/2009 Paul.  Only show the dynamic teams if this is the Enterprise Edition. 
				// 11/06/2015 Paul.  Add support for the Ultimate edition. 
				string sServiceLevel = Sql.ToString(Application["CONFIG.service_level"]);
				spnDynamicTeamsButtons.Visible = (String.Compare(sServiceLevel, "Enterprise", true) == 0) || (String.Compare(sServiceLevel, "Ultimate", true) == 0);
				spnDynamicTeamsMessage.Visible = spnDynamicTeamsButtons.Visible;
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				spnDynamicAssignmentButtons.Visible = (String.Compare(sServiceLevel, "Enterprise", true) == 0) || (String.Compare(sServiceLevel, "Ultimate", true) == 0);
				spnDynamicAssignmentMessage.Visible = spnDynamicAssignmentButtons.Visible;
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
		}
		#endregion
	}
}
