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

namespace SplendidCRM.Administration.AuthorizeNet
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
		protected CheckBox     AUTHORIZENET_ENABLED  ;
		protected CheckBox     TEST_MODE             ;
		protected TextBox      USER_NAME             ;
		protected TextBox      TRANSACTION_KEY       ;
		protected RequiredFieldValidator reqUSER_NAME;
		protected RequiredFieldValidator reqTRANSACTION_KEY;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" )
			{
				try
				{
					USER_NAME      .Text = USER_NAME      .Text.Trim();
					TRANSACTION_KEY.Text = TRANSACTION_KEY.Text.Trim();
					
					string sTRANSACTION_KEY = TRANSACTION_KEY.Text;
					// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
					if ( sTRANSACTION_KEY == Sql.sEMPTY_PASSWORD )
					{
						sTRANSACTION_KEY = Sql.ToString (Application["CONFIG.AuthorizeNet.TransactionKey"]);
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
						string sENCRYPTED_TRANSACTION_KEY = Security.EncryptPassword(sTRANSACTION_KEY, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
						if ( Security.DecryptPassword(sENCRYPTED_TRANSACTION_KEY, gCREDIT_CARD_KEY, gCREDIT_CARD_IV) != sTRANSACTION_KEY )
							throw(new Exception("Decryption failed"));
						sTRANSACTION_KEY = sENCRYPTED_TRANSACTION_KEY;
					}
					reqUSER_NAME      .Enabled = true;
					reqTRANSACTION_KEY.Enabled = true;
					reqUSER_NAME      .Validate();
					reqTRANSACTION_KEY.Validate();
					if ( Page.IsValid )
					{
						if ( e.CommandName == "Test" )
						{
							string sResult = AuthorizeNetUtils.ValidateLogin(Application, USER_NAME.Text, sTRANSACTION_KEY, TEST_MODE.Checked);
							if ( Sql.IsEmptyString(sResult) )
							{
								ctlDynamicButtons.ErrorText = L10n.Term("AuthorizeNet.LBL_CONNECTION_SUCCESSFUL");
								// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
								TRANSACTION_KEY.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
							}
							else
							{
								ctlDynamicButtons.ErrorText = String.Format(L10n.Term("AuthorizeNet.ERR_FAILED_TO_CONNECT"), sResult);
							}
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.AuthorizeNet.Enabled"       ] = AUTHORIZENET_ENABLED.Checked;
							Application["CONFIG.AuthorizeNet.TestMode"      ] = TEST_MODE.Checked;
							Application["CONFIG.AuthorizeNet.UserName"      ] = USER_NAME.Text   ;
							Application["CONFIG.AuthorizeNet.TransactionKey"] = sTRANSACTION_KEY;
							SqlProcs.spCONFIG_Update("system", "AuthorizeNet.Enabled"       , Sql.ToString(Application["CONFIG.AuthorizeNet.Enabled"       ]));
							SqlProcs.spCONFIG_Update("system", "AuthorizeNet.TestMode"      , Sql.ToString(Application["CONFIG.AuthorizeNet.TestMode"      ]));
							SqlProcs.spCONFIG_Update("system", "AuthorizeNet.UserName"      , Sql.ToString(Application["CONFIG.AuthorizeNet.UserName"      ]));
							SqlProcs.spCONFIG_Update("system", "AuthorizeNet.TransactionKey", Sql.ToString(Application["CONFIG.AuthorizeNet.TransactionKey"]));
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
			SetPageTitle(L10n.Term("AuthorizeNet.LBL_AUTHORIZENET_SETTINGS"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				reqUSER_NAME      .DataBind();
				reqTRANSACTION_KEY.DataBind();
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);

					AUTHORIZENET_ENABLED.Checked = Sql.ToBoolean(Application["CONFIG.AuthorizeNet.Enabled"       ]);
					TEST_MODE           .Checked = Sql.ToBoolean(Application["CONFIG.AuthorizeNet.TestMode"      ]);
					USER_NAME           .Text    = Sql.ToString (Application["CONFIG.AuthorizeNet.UserName"      ]);
					string sTRANSACTION_KEY      = Sql.ToString (Application["CONFIG.AuthorizeNet.TransactionKey"]);
					if ( !Sql.IsEmptyString(sTRANSACTION_KEY) )
					{
						// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
						TRANSACTION_KEY.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
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
			m_sMODULE = "AuthorizeNet";
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
