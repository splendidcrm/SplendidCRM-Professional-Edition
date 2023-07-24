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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Diagnostics;
using SplendidCRM._controls;

namespace SplendidCRM.EmailClient
{
	/// <summary>
	///		Summary description for ImportView.
	/// </summary>
	public class ImportView : SplendidControl
	{
		protected HtmlTable       tblMain                      ;
		protected Label           lblError                     ;

		public CommandEventHandler Command;

		public void ClearForm()
		{
			try
			{
				DropDownList    ctlPARENT_TYPE      = FindControl("PARENT_TYPE"     ) as DropDownList;
				TextBox         ctlPARENT_NAME      = FindControl("PARENT_NAME"     ) as TextBox;
				HtmlInputHidden ctlPARENT_ID        = FindControl("PARENT_ID"       ) as HtmlInputHidden;
				TextBox         ctlASSIGNED_TO      = FindControl("ASSIGNED_TO"     ) as TextBox;
				TextBox         ctlASSIGNED_TO_NAME = FindControl("ASSIGNED_TO_NAME") as TextBox;
				HtmlInputHidden ctlASSIGNED_USER_ID = FindControl("ASSIGNED_USER_ID") as HtmlInputHidden;
				TextBox         ctlTEAM_NAME        = FindControl("TEAM_NAME"       ) as TextBox;
				HtmlInputHidden ctlTEAM_ID          = FindControl("TEAM_ID"         ) as HtmlInputHidden;
				TeamSelect      ctlTeamSelect       = FindControl("TEAM_SET_NAME"   ) as TeamSelect;
				if ( ctlPARENT_TYPE      != null ) ctlPARENT_TYPE     .SelectedIndex = 0;
				if ( ctlPARENT_NAME      != null ) ctlPARENT_NAME     .Text  = String.Empty;
				if ( ctlPARENT_ID        != null ) ctlPARENT_ID       .Value = String.Empty;
				if ( ctlASSIGNED_TO      != null ) ctlASSIGNED_TO     .Text  = Security.USER_NAME;
				// 05/18/2014 Paul.  ASSIGNED_TO_NAME is a possible field name. 
				if ( ctlASSIGNED_TO_NAME != null ) ctlASSIGNED_TO_NAME.Text  = Security.FULL_NAME;
				if ( ctlASSIGNED_USER_ID != null ) ctlASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
				if ( ctlTEAM_NAME        != null ) ctlTEAM_NAME       .Text  = Security.TEAM_NAME;
				if ( ctlTEAM_ID          != null ) ctlTEAM_ID         .Value = Security.TEAM_ID.ToString();
				if ( ctlTeamSelect       != null ) ctlTeamSelect.LoadLineItems(Guid.Empty, true);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if ( !IsPostBack )
				{
					this.NotPostBack = true;
					this.AppendEditViewFields(m_sMODULE + ".ImportView", tblMain, null);
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
			// CODEGEN: This Meeting is required by the ASP.NET Web Form Designer.
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
			m_sMODULE = "EmailClient";
			if ( IsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + ".ImportView", tblMain, null);
			}
		}
		#endregion
	}
}

