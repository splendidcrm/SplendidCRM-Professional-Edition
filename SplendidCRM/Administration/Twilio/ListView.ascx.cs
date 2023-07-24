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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace SplendidCRM.Administration.Twilio
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected SearchBasic   ctlSearchBasic                     ;
		protected HtmlTable     tblMain                            ;
		protected DataGrid      grdMain                            ;
		protected Label         lblError                           ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Clear" )
				{
					ctlSearchBasic.ClearForm();
					Cache.Remove("Twilio.Messages");
					grdMain.CurrentPageIndex = 0;
				}
				else if ( e.CommandName == "Search" )
				{
					Cache.Remove("Twilio.Messages");
					grdMain.CurrentPageIndex = 0;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text += ex.Message;
			}
			try
			{
				Bind(true);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text += ex.Message;
			}
		}

		protected void OnPageIndexChanged(Object sender, DataGridPageChangedEventArgs e)
		{
			Cache.Remove("Twilio.Messages");
			grdMain.CurrentPageIndex = e.NewPageIndex;
			Bind(true);
		}

		private void Bind(bool bBind)
		{
			if ( !Sql.IsEmptyString(Context.Application["CONFIG.Twilio.AccountSID"]) && !Sql.IsEmptyString(Context.Application["CONFIG.Twilio.AuthToken"]) )
			{
				List<MessageResource> result = Cache.Get("Twilio.Messages") as List<MessageResource>;
				if ( result == null )
				{
					result = TwilioManager.ListMessages(Application, ctlSearchBasic.DATE_SENT, ctlSearchBasic.FROM_NUMBER, ctlSearchBasic.TO_NUMBER, grdMain.CurrentPageIndex);
					Cache.Insert("Twilio.Messages", result, null, DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
					//grdMain.AllowCustomPaging = true;
					//grdMain.VirtualItemCount  = result.Total;
					//grdMain.PageSize          = result.PageSize;
					grdMain.DataSource        = result;
				}
				if ( bBind )
				{
					grdMain.DataBind();
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				if ( !IsPostBack )
				{
					Cache.Remove("Twilio.Messages");
					ctlSearchBasic.FROM_NUMBER  = Sql.ToString(Application["CONFIG.Twilio.FromPhone"]);
				}
				Bind(!IsPostBack);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = Utils.ExpandException(ex);
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
			ctlSearchBasic.Command += new CommandEventHandler(Page_Command);
			grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(OnPageIndexChanged);
			m_sMODULE = "Twilio";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
