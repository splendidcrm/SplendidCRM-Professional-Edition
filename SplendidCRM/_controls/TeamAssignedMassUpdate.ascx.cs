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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for TeamAssignedMassUpdate.
	/// </summary>
	public class TeamAssignedMassUpdate : SplendidControl
	{
		protected bool         bShowAssigned      = true;
		protected HiddenField  ASSIGNED_USER_ID   ;
		protected HiddenField  TEAM_ID            ;
		protected TeamSelect   ctlTeamSelect      ;

		public bool ShowAssigned
		{
			get { return bShowAssigned; }
			set { bShowAssigned = value; }
		}

		public Guid ASSIGNED_USER
		{
			get
			{
				return Sql.ToGuid(ASSIGNED_USER_ID.Value);
			}
		}

		public Guid PRIMARY_TEAM_ID
		{
			get
			{
				if ( Crm.Config.enable_dynamic_teams() )
				{
					// 08/30/2009 Paul.  Use a separate call to get the primary so that we don't 
					// over-write the primary unless the user specifically wants that. 
					return ctlTeamSelect.PRIMARY_TEAM_ID;
				}
				return Sql.ToGuid(TEAM_ID.Value);
			}
		}

		public string TEAM_SET_LIST
		{
			get
			{
				if ( Crm.Config.enable_dynamic_teams() )
					return ctlTeamSelect.TEAM_SET_LIST;
				return String.Empty;
			}
		}

		public bool ADD_TEAM_SET
		{
			get
			{
				if ( Crm.Config.enable_dynamic_teams() )
					return ctlTeamSelect.ADD_TEAM_SET;
				return false;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 09/11/2007 Paul.  If neither the assigned nor the team are visbile, then hide the entire user control. 
			if ( !bShowAssigned && !Crm.Config.enable_team_management() )
				this.Visible = false;
			
			if ( !IsPostBack )
			{
				ctlTeamSelect.LoadLineItems(Guid.Empty, false);
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

