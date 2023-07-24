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
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.Undelete
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected TextBox              txtNAME       ;
		protected TextBox              txtAUDIT_TOKEN;
		protected DropDownList         lstMODULE_NAME;
		protected DropDownList         lstUSERS      ;
		protected _controls.DatePicker ctlSTART_DATE ;
		protected _controls.DatePicker ctlEND_DATE   ;
		protected Button               btnSearch     ;
		protected Button               btnClear      ;
		protected Button               btnUndelete   ;
		protected CheckBox             chkBackground ;
		
		public string MODULE_NAME
		{
			get { return lstMODULE_NAME.SelectedValue; }
		}

		public bool BackgroundOperation
		{
			get { return chkBackground.Checked; }
		}

		protected void lstMODULE_NAME_Changed(object sender, System.EventArgs e)
		{
			lstMODULE_NAME.Focus();
			if ( Command != null )
				Command(this, new CommandEventArgs("Search", null)) ;
		}

		protected void lstUSERS_Changed(object sender, System.EventArgs e)
		{
			if ( Command != null )
				Command(this, new CommandEventArgs("Search", null)) ;
		}

		public override void ClearForm()
		{
			txtNAME       .Text     = String.Empty;
			txtAUDIT_TOKEN.Text     = String.Empty;
			ctlSTART_DATE .DateText = String.Empty;
			ctlEND_DATE   .DateText = String.Empty;
			lstUSERS.SelectedIndex  = 0;
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, txtNAME.Text          , 200, Sql.SqlFilterMode.StartsWith, "NAME"            );
			Sql.AppendParameter(cmd, txtAUDIT_TOKEN.Text   , 200, Sql.SqlFilterMode.Exact     , "AUDIT_TOKEN"     );
			Sql.AppendParameter(cmd, lstUSERS                                                 , "MODIFIED_USER_ID");
			DateTime dtDateStart = DateTime.MinValue;
			DateTime dtDateEnd   = DateTime.MinValue;
			if ( !Sql.IsEmptyString(ctlSTART_DATE.DateText) )
			{
				dtDateStart = ctlSTART_DATE.Value;
			}
			if ( !Sql.IsEmptyString(ctlEND_DATE.DateText) )
			{
				dtDateEnd = ctlEND_DATE.Value;
			}
			if ( dtDateStart != DateTime.MinValue ||dtDateEnd != DateTime.MinValue )
				Sql.AppendParameter(cmd, dtDateStart, dtDateEnd, "AUDIT_DATE");
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				lstMODULE_NAME.DataSource = SplendidCache.AuditedModules();
				lstMODULE_NAME.DataBind();
				lstMODULE_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

				lstUSERS.DataSource = SplendidCache.ActiveUsers();
				lstUSERS.DataBind();
				lstUSERS.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
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
