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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.PayTrace
{
	/// <summary>
	///		Summary description for ConfigView.
	/// </summary>
	public class ConfigView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
		//private const string sEMPTY_PASSWORD = "**********";
		protected CheckBox     PAYTRACE_ENABLED      ;
		protected TextBox      USER_NAME             ;
		protected TextBox      PASSWORD              ;
		protected RequiredFieldValidator reqUSER_NAME;
		protected RequiredFieldValidator reqPASSWORD ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" )
			{
				try
				{
					USER_NAME   .Text = USER_NAME   .Text.Trim();
					PASSWORD    .Text = PASSWORD    .Text.Trim();
					
					string sPASSWORD = PASSWORD.Text;
					// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
					if ( sPASSWORD == Sql.sEMPTY_PASSWORD )
					{
						sPASSWORD = Sql.ToString (Application["CONFIG.PayTrace.Password"]);
					}
					else
					{
						// 09/13/2013 Paul.  Create separate keys for credit card numbers. 
						Guid gCREDIT_CARD_KEY = Sql.ToGuid(Application["CONFIG.CreditCardKey"]);
						if ( Sql.IsEmptyGuid(gCREDIT_CARD_KEY) )
						{
							gCREDIT_CARD_KEY = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("system", "CreditCardKey", gCREDIT_CARD_KEY.ToString());
							Application["CONFIG.CreditCardKey"] = gCREDIT_CARD_KEY;
						}
						Guid gCREDIT_CARD_IV = Sql.ToGuid(Application["CONFIG.CreditCardIV"]);
						if ( Sql.IsEmptyGuid(gCREDIT_CARD_IV) )
						{
							gCREDIT_CARD_IV = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("system", "CreditCardIV", gCREDIT_CARD_IV.ToString());
							Application["CONFIG.CreditCardIV"] = gCREDIT_CARD_IV;
						}
						string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sPASSWORD, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
						if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gCREDIT_CARD_KEY, gCREDIT_CARD_IV) != sPASSWORD )
							throw(new Exception("Decryption failed"));
						sPASSWORD = sENCRYPTED_EMAIL_PASSWORD;
					}
					reqUSER_NAME .Enabled = true;
					reqPASSWORD  .Enabled = true;
					reqUSER_NAME .Validate();
					reqPASSWORD  .Validate();
					if ( Page.IsValid )
					{
						if ( e.CommandName == "Test" )
						{
							string sResult = PayTraceUtils.ValidateLogin(Application, USER_NAME.Text, sPASSWORD);
							if ( Sql.IsEmptyString(sResult) )
							{
								ctlDynamicButtons.ErrorText = L10n.Term("PayTrace.LBL_CONNECTION_SUCCESSFUL");
								// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
								PASSWORD.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
							}
							else
							{
								ctlDynamicButtons.ErrorText = String.Format(L10n.Term("PayTrace.ERR_FAILED_TO_CONNECT"), sResult);
							}
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.PayTrace.Enabled" ] = PAYTRACE_ENABLED.Checked;
							Application["CONFIG.PayTrace.UserName"] = USER_NAME       .Text   ;
							Application["CONFIG.PayTrace.Password"] = sPASSWORD               ;
							SqlProcs.spCONFIG_Update("system", "PayTrace.Enabled" , Sql.ToString(Application["CONFIG.PayTrace.Enabled" ]));
							SqlProcs.spCONFIG_Update("system", "PayTrace.UserName", Sql.ToString(Application["CONFIG.PayTrace.UserName"]));
							SqlProcs.spCONFIG_Update("system", "PayTrace.Password", Sql.ToString(Application["CONFIG.PayTrace.Password"]));
							Response.Redirect("../default.aspx");
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
					return;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("../default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("PayTrace.LBL_PAYTRACE_SETTINGS"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				reqUSER_NAME .DataBind();
				reqPASSWORD  .DataBind();
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);

					PAYTRACE_ENABLED.Checked = Sql.ToBoolean(Application["CONFIG.PayTrace.Enabled" ]);
					USER_NAME       .Text    = Sql.ToString (Application["CONFIG.PayTrace.UserName"]);
					string sPASSWORD         = Sql.ToString (Application["CONFIG.PayTrace.Password" ]);
					if ( !Sql.IsEmptyString(sPASSWORD) )
					{
						// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
						PASSWORD.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
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
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "PayTrace";
			SetAdminMenu(m_sMODULE);
			if ( IsPostBack )
			{
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
			}
		}
		#endregion
	}
}
