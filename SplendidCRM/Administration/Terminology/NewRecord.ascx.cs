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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.Terminology
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : SplendidControl
	{
		protected Label                      lblError           ;
		protected TextBox                    txtNAME            ;
		protected TextBox                    txtDISPLAY_NAME    ;
		protected DropDownList               lstLANGUAGE        ;
		protected DropDownList               lstMODULE_NAME     ;
		protected DropDownList               lstLIST_NAME       ;
		protected TextBox                    txtLIST_ORDER      ;
		protected RequiredFieldValidator     reqNAME            ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "NewRecord" )
			{
				reqNAME.Enabled = true;
				reqNAME.Validate();
				if ( Page.IsValid )
				{
					Guid gID = Guid.Empty;
					try
					{
						SqlProcs.spTERMINOLOGY_InsertOnly(txtNAME.Text, lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, lstLIST_NAME.SelectedValue, Sql.ToInteger(txtLIST_ORDER.Text), txtDISPLAY_NAME.Text);
						// 01/16/2006 Paul.  Update language cache. 
						if ( Sql.IsEmptyString(lstLIST_NAME.SelectedValue) )
							L10N.SetTerm(lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, txtNAME.Text, txtDISPLAY_NAME.Text);
						else
							L10N.SetTerm(lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, lstLIST_NAME.SelectedValue, txtNAME.Text, txtDISPLAY_NAME.Text);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
					if ( !Sql.IsEmptyGuid(gID) )
						Response.Redirect("default.aspx");
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;
			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();  // Need to bind so that Text of the Button gets updated. 
			reqNAME.ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Terminology.LBL_LIST_NAME") + "<br>";
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( !bIsPostBack )
			{
				// 05/06/2010 Paul.  When the control is created out-of-band, we need to manually bind the controls. 
				if ( NotPostBack )
					this.DataBind();
				// 01/12/2006 Paul.  Language cannot be null. 
				lstLANGUAGE.DataSource = SplendidCache.Languages();
				lstLANGUAGE.DataBind();

				DataTable dtModules = SplendidCache.Modules().Copy();
				dtModules.Rows.InsertAt(dtModules.NewRow(), 0);
				lstMODULE_NAME.DataSource = dtModules;
				lstMODULE_NAME.DataBind();
				// 01/12/2006 Paul.  Insert is failing, but I don't know why.  
				// It might be because the NewRecord control is loaded using LoadControl. 
				// Very odd as the Search Control is not having a problem inserting a value. 
				//lstMODULE_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

				DataTable dtPickLists = SplendidCache.TerminologyPickLists().Copy();
				dtPickLists.Rows.InsertAt(dtPickLists.NewRow(), 0);
				lstLIST_NAME.DataSource = dtPickLists;
				lstLIST_NAME.DataBind();
				//lstLIST_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

				try
				{
					// 01/12/2006 Paul.  Set default value to current language. 
					// 01/12/2006 Paul.  This is not working.  Use client-side script to select the default. 
					// 08/19/2010 Paul.  Check the list before assigning the value. 
					Utils.SetSelectedValue(lstLANGUAGE, L10N.NormalizeCulture(L10n.NAME));
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
				//this.DataBind();
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
			m_sMODULE = "Terminology";
		}
		#endregion
	}
}

