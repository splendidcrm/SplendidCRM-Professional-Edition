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

namespace SplendidCRM.Quotes
{
	/// <summary>
	///		Summary description for MassUpdate.
	/// </summary>
	public class MassUpdate : SplendidCRM.MassUpdate
	{
		// 11/10/2010 Paul.  Convert MassUpdate to dynamic buttons. 
		// 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. 
		protected _controls.MassUpdateButtons ctlDynamicButtons;

		protected HiddenField     txtSHIPPING_ACCOUNT_ID  ;
		protected TextBox         txtSHIPPING_ACCOUNT_NAME;
		protected HiddenField     txtSHIPPING_CONTACT_ID  ;
		protected TextBox         txtSHIPPING_CONTACT_NAME;
		protected HiddenField     txtBILLING_ACCOUNT_ID   ;
		protected TextBox         txtBILLING_ACCOUNT_NAME ;
		protected HiddenField     txtBILLING_CONTACT_ID   ;
		protected TextBox         txtBILLING_CONTACT_NAME ;
		protected DropDownList    lstPAYMENT_TERMS        ;
		protected DropDownList    lstQUOTE_STAGE          ;
		protected _controls.DatePicker ctlDATE_QUOTE_EXPECTED_CLOSED;
		protected _controls.DatePicker ctlORIGINAL_PO_DATE          ;
		public    CommandEventHandler Command ;
		protected _controls.TeamAssignedMassUpdate ctlTeamAssignedMassUpdate;
		// 05/13/2016 Paul.  Add Tags module. 
		protected _controls.TagMassUpdate          ctlTagMassUpdate;

		public Guid ASSIGNED_USER_ID
		{
			get
			{
				return ctlTeamAssignedMassUpdate.ASSIGNED_USER;
			}
		}

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

		public Guid SHIPPING_ACCOUNT_ID
		{
			get
			{
				return Sql.ToGuid(txtSHIPPING_ACCOUNT_ID.Value);
			}
		}

		public Guid SHIPPING_CONTACT_ID
		{
			get
			{
				return Sql.ToGuid(txtSHIPPING_CONTACT_ID.Value);
			}
		}

		public Guid BILLING_ACCOUNT_ID
		{
			get
			{
				return Sql.ToGuid(txtBILLING_ACCOUNT_ID.Value);
			}
		}

		public Guid BILLING_CONTACT_ID
		{
			get
			{
				return Sql.ToGuid(txtBILLING_CONTACT_ID.Value);
			}
		}

		public string PAYMENT_TERMS
		{
			get
			{
				return lstPAYMENT_TERMS.SelectedValue;
			}
		}

		public string QUOTE_STAGE
		{
			get
			{
				return lstQUOTE_STAGE.SelectedValue;
			}
		}

		public DateTime DATE_QUOTE_EXPECTED_CLOSED
		{
			get
			{
				// 07/09/2006 Paul.  Move the date conversion out of the MassUpdate control. 
				return ctlDATE_QUOTE_EXPECTED_CLOSED.Value;
			}
		}

		public DateTime ORIGINAL_PO_DATE
		{
			get
			{
				// 07/09/2006 Paul.  Move the date conversion out of the MassUpdate control. 
				return ctlORIGINAL_PO_DATE.Value;
			}
		}

		// 05/13/2016 Paul.  Add Tags module. 
		public string TAG_SET_NAME
		{
			get
			{
				return ctlTagMassUpdate.TAG_SET_NAME;
			}
		}

		public bool ADD_TAG_SET
		{
			get
			{
				return ctlTagMassUpdate.ADD_TAG_SET;
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
					int nACLACCESS_Delete = Security.GetUserAccess(m_sMODULE, "delete");
					int nACLACCESS_Edit   = Security.GetUserAccess(m_sMODULE, "edit"  );
					ctlDynamicButtons.ShowButton("MassUpdate", nACLACCESS_Edit   >= 0);
					ctlDynamicButtons.ShowButton("MassDelete", nACLACCESS_Delete >= 0);
					// 09/26/2017 Paul.  Add Archive access right. 
					int nACLACCESS_Archive = Security.GetUserAccess(m_sMODULE, "archive");
					ctlDynamicButtons.ShowButton("Archive.MoveData"   , (nACLACCESS_Archive >= ACL_ACCESS.ARCHIVE || Security.IS_ADMIN) && !ArchiveView() && ArchiveEnabled());
					ctlDynamicButtons.ShowButton("Archive.RecoverData", (nACLACCESS_Archive >= ACL_ACCESS.ARCHIVE || Security.IS_ADMIN) &&  ArchiveView() && ArchiveEnabled());

					// 02/15/2015 Paul.  Change from terminology payment_terms_dom to PaymentTerms list for QuickBooks Online. 
					lstPAYMENT_TERMS.DataSource = SplendidCache.List("PaymentTerms");
					lstPAYMENT_TERMS.DataBind();
					lstPAYMENT_TERMS.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstQUOTE_STAGE  .DataSource = SplendidCache.List("quote_stage_dom");
					lstQUOTE_STAGE  .DataBind();
					lstQUOTE_STAGE  .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
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
			m_sMODULE = "Quotes";
			// 09/26/2017 Paul.  Add Archive access right. 
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".MassUpdate" + (ArchiveView() ? ".ArchiveView" : String.Empty), Guid.Empty, null);
		}
		#endregion
	}
}
