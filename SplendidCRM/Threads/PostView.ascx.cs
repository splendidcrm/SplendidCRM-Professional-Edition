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

namespace SplendidCRM.Threads
{
	/// <summary>
	/// Summary description for PostView.
	/// </summary>
	public class PostView : SplendidControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected HiddenField  txtPOST_ID       ;
		protected Label        txtTITLE         ;
		protected Label        txtCREATED_BY    ;
		protected Label        txtDATE_ENTERED  ;
		protected Label        txtMODIFIED_BY   ;
		protected Label        txtDATE_MODIFIED ;
		protected Literal      txtDESCRIPTION   ;
		protected TableRow     trModified       ;

		public Guid POST_ID
		{
			get { return Sql.ToGuid(txtPOST_ID.Value); }
			set { txtPOST_ID.Value = value.ToString(); }
		}

		public string TITLE
		{
			get { return txtTITLE.Text; }
			set { txtTITLE.Text = value; }
		}

		public string CREATED_BY
		{
			get { return txtCREATED_BY.Text; }
			set { txtCREATED_BY.Text = value; }
		}

		public string DATE_ENTERED
		{
			get { return txtDATE_ENTERED.Text; }
			set { txtDATE_ENTERED.Text = value; }
		}

		public string MODIFIED_BY
		{
			get { return txtMODIFIED_BY.Text; }
			set { txtMODIFIED_BY.Text = value; }
		}

		public string DATE_MODIFIED
		{
			get { return txtDATE_MODIFIED.Text; }
			set { txtDATE_MODIFIED.Text = value; }
		}

		public string DESCRIPTION
		{
			get { return txtDESCRIPTION.Text; }
			set { txtDESCRIPTION.Text = value; }
		}

		public bool Modified
		{
			get { return trModified.Visible; }
			set { trModified.Visible = value; }
		}

		public bool ShowEdit
		{
			set { ctlDynamicButtons.ShowButton("Edit", value); }
		}

		public bool ShowDelete
		{
			set { ctlDynamicButtons.ShowButton("Delete", value); }
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				Guid gPOST_ID = POST_ID;
				if ( e.CommandName == "Reply" )
				{
					Response.Redirect("~/Posts/edit.aspx?REPLY_ID=" + gPOST_ID.ToString());
				}
				else if ( e.CommandName == "Quote" )
				{
					Response.Redirect("~/Posts/edit.aspx?QUOTE=1&REPLY_ID=" + gPOST_ID.ToString());
				}
				else if ( e.CommandName == "Edit" )
				{
					Response.Redirect("~/Posts/edit.aspx?ID=" + gPOST_ID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spPOSTS_Delete(gPOST_ID);
					Guid gID = Sql.ToGuid(Request["ID"]);
					int nListView = Sql.ToInteger(Request["ListView"]);
					Response.Redirect("view.aspx?ID=" + gID.ToString() + "&ListView=" + nListView.ToString());
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Posts";
			ctlDynamicButtons.AppendButtons("Threads.PostView", Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
