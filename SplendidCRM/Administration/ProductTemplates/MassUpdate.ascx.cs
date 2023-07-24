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

namespace SplendidCRM.Administration.ProductTemplates
{
	/// <summary>
	///		Summary description for MassUpdate.
	/// </summary>
	public class MassUpdate : SplendidCRM.MassUpdate
	{
		// 11/10/2010 Paul.  Convert MassUpdate to dynamic buttons. 
		// 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. 
		protected _controls.MassUpdateButtons ctlDynamicButtons;

		protected HiddenField     txtACCOUNT_ID        ;
		protected TextBox         txtACCOUNT_NAME      ;
		// 05/23/2012 Paul.  QuickBooks requires the Product Type and the QuickBooks Account fields. 
		protected HiddenField     txtTYPE_ID           ;
		protected TextBox         txtTYPE_NAME         ;
		protected TextBox         txtQUICKBOOKS_ACCOUNT;
		protected DropDownList    lstSTATUS            ;
		protected DropDownList    lstTAX_CLASS         ;
		protected DropDownList    lstSUPPORT_TERM      ;
		protected _controls.DatePicker ctlDATE_COST_PRICE;
		protected _controls.DatePicker ctlDATE_AVAILABLE ;
		public    CommandEventHandler Command ;
		protected _controls.TeamAssignedMassUpdate ctlTeamAssignedMassUpdate;

		public Guid PRIMARY_TEAM_ID
		{
			get
			{
				return ctlTeamAssignedMassUpdate.PRIMARY_TEAM_ID;
			}
		}

		// 08/29/2009 Paul. Add support for dynamic teams. 
		public string TEAM_SET_LIST
		{
			get
			{
				return ctlTeamAssignedMassUpdate.TEAM_SET_LIST;
			}
		}

		public bool ADD_TEAM_SET
		{
			get
			{
				return ctlTeamAssignedMassUpdate.ADD_TEAM_SET;
			}
		}

		public Guid ACCOUNT_ID
		{
			get
			{
				return Sql.ToGuid(txtACCOUNT_ID.Value);
			}
		}

		// 05/23/2012 Paul.  QuickBooks requires the Product Type and the QuickBooks Account fields. 
		public Guid TYPE_ID
		{
			get
			{
				return Sql.ToGuid(txtTYPE_ID.Value);
			}
		}

		public string QUICKBOOKS_ACCOUNT
		{
			get
			{
				return txtQUICKBOOKS_ACCOUNT.Text;
			}
		}

		public string STATUS
		{
			get
			{
				return lstSTATUS.SelectedValue;
			}
		}

		public string TAX_CLASS
		{
			get
			{
				return lstTAX_CLASS.SelectedValue;
			}
		}

		public string SUPPORT_TERM
		{
			get
			{
				return lstSUPPORT_TERM.SelectedValue;
			}
		}

		public DateTime DATE_COST_PRICE
		{
			get
			{
				// 07/09/2006 Paul.  Move the date conversion out of the MassUpdate control. 
				return ctlDATE_COST_PRICE.Value;
			}
		}

		public DateTime DATE_AVAILABLE
		{
			get
			{
				// 07/09/2006 Paul.  Move the date conversion out of the MassUpdate control. 
				return ctlDATE_AVAILABLE.Value;
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// Command is handled by the parent. 
			if ( Command != null )
				Command(this, e) ;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if ( !IsPostBack )
				{
					// 06/02/2006 Paul.  Buttons should be hidden if the user does not have access. 
					int nACLACCESS_Delete = Security.AdminUserAccess(m_sMODULE, "delete");
					int nACLACCESS_Edit   = Security.AdminUserAccess(m_sMODULE, "edit"  );
					ctlDynamicButtons.ShowButton("MassUpdate", nACLACCESS_Edit   >= 0);
					ctlDynamicButtons.ShowButton("MassDelete", nACLACCESS_Delete >= 0);
					
					// 05/19/2012 Paul.  The visibility of the MassUpdate panel is controlled by the parent. 
					//this.Visible = !PrintView && ((nACLACCESS_Delete >= 0) || (nACLACCESS_Edit   >= 0));
					
					// 05/19/2012 Paul.  The list should be product_template_status_dom, not product_status_dom. 
					lstSTATUS      .DataSource = SplendidCache.List("product_template_status_dom");
					lstSTATUS      .DataBind();
					lstSTATUS      .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstTAX_CLASS   .DataSource = SplendidCache.List("tax_class_dom");
					lstTAX_CLASS   .DataBind();
					lstTAX_CLASS   .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstSUPPORT_TERM.DataSource = SplendidCache.List("support_term_dom");
					lstSUPPORT_TERM.DataBind();
					lstSUPPORT_TERM.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "ProductTemplates";
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".MassUpdate", Guid.Empty, null);
		}
		#endregion
	}
}
