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
using System.Web;
using System.Web.UI.WebControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for DetailButtons.
	/// </summary>
	public class DetailButtons : SplendidControl
	{
		protected Label  lblError    ;
		protected Button btnEdit     ;
		protected Button btnDuplicate;
		protected Button btnDelete   ;

		public CommandEventHandler Command;

		public void DisableAll()
		{
			btnEdit     .Enabled = false;
			btnDuplicate.Enabled = false;
			btnDelete   .Enabled = false;
		}

		public bool EnableEdit
		{
			get
			{
				return btnEdit.Enabled;
			}
			set
			{
				btnEdit.Enabled = value;
			}
		}

		public bool EnableDuplicate
		{
			get
			{
				return btnDuplicate.Enabled;
			}
			set
			{
				btnDuplicate.Enabled = value;
			}
		}

		public bool EnableDelete
		{
			get
			{
				return btnDelete.Enabled;
			}
			set
			{
				btnDelete.Enabled = value;
			}
		}

		public bool ShowEdit
		{
			get
			{
				return btnEdit.Visible;
			}
			set
			{
				btnEdit.Visible = value;
			}
		}

		public bool ShowDuplicate
		{
			get
			{
				return btnDuplicate.Visible;
			}
			set
			{
				btnDuplicate.Visible = value;
			}
		}

		public bool ShowDelete
		{
			get
			{
				return btnDelete.Visible;
			}
			set
			{
				btnDelete.Visible = value;
			}
		}

		public string ErrorText
		{
			get
			{
				return lblError.Text;
			}
			set
			{
				lblError.Text = value;
			}
		}

		// 04/27/2006 Paul.  This function should be virtual so that it could be 
		// over-ridden by LeadDetailButtons, or ProspectDetailButtons.
		public virtual void SetUserAccess(string sMODULE_NAME, Guid gASSIGNED_USER_ID)
		{
			// 05/22/2006 Paul.  Disable button if NOT Owner.
			int nACLACCESS_Delete = Security.GetUserAccess(sMODULE_NAME, "delete");
			if ( nACLACCESS_Delete == ACL_ACCESS.NONE || (nACLACCESS_Delete == ACL_ACCESS.OWNER && Security.USER_ID != gASSIGNED_USER_ID) )
			{
				btnDelete.Visible = false;
			}
			
			// 05/22/2006 Paul.  Disable button if NOT Owner.
			int nACLACCESS_Edit = Security.GetUserAccess(sMODULE_NAME, "edit");
			if ( nACLACCESS_Edit == ACL_ACCESS.NONE || (nACLACCESS_Edit == ACL_ACCESS.OWNER && Security.USER_ID != gASSIGNED_USER_ID) )
			{
				btnEdit.Visible      = false;
				btnDuplicate.Visible = false;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
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

