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

namespace SplendidCRM.Administration.PayPalTransactions
{
	/// <summary>
	///		Summary description for ConfigView.
	/// </summary>
	public class ConfigView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected TextBox      USER_NAME  ;
		protected TextBox      PASSWORD   ;
		protected TextBox      PRIVATE_KEY;
		protected TextBox      CERTIFICATE;
		protected CheckBox     SANDBOX    ;
		// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
		protected TextBox      REST_CLIENT_ID    ;
		protected TextBox      REST_CLIENT_SECRET;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" )
			{
				try
				{
					USER_NAME.Text = USER_NAME.Text.Trim();
					PASSWORD .Text = PASSWORD .Text.Trim();
					
					if ( Page.IsValid )
					{
						if ( e.CommandName == "Save" )
						{
							Application["CONFIG.PayPal.APIUsername"    ] = USER_NAME  .Text;
							Application["CONFIG.PayPal.APIPassword"    ] = PASSWORD   .Text;
							Application["CONFIG.PayPal.X509PrivateKey" ] = PRIVATE_KEY.Text;
							Application["CONFIG.PayPal.X509Certificate"] = CERTIFICATE.Text;
							Application["CONFIG.PayPal.Sandbox"        ] = SANDBOX    .Checked;
							// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
							Application["CONFIG.PayPal.ClientID"       ] = REST_CLIENT_ID    .Text;
							Application["CONFIG.PayPal.ClientSecret"   ] = REST_CLIENT_SECRET.Text;
							SqlProcs.spCONFIG_Update("system", "PayPal.APIUsername"    , Sql.ToString(Application["CONFIG.PayPal.APIUsername"    ]));
							SqlProcs.spCONFIG_Update("system", "PayPal.APIPassword"    , Sql.ToString(Application["CONFIG.PayPal.APIPassword"    ]));
							SqlProcs.spCONFIG_Update("system", "PayPal.X509PrivateKey" , Sql.ToString(Application["CONFIG.PayPal.X509PrivateKey" ]));
							SqlProcs.spCONFIG_Update("system", "PayPal.X509Certificate", Sql.ToString(Application["CONFIG.PayPal.X509Certificate"]));
							SqlProcs.spCONFIG_Update("system", "PayPal.Sandbox"        , Sql.ToString(Application["CONFIG.PayPal.Sandbox"        ]));
							SqlProcs.spCONFIG_Update("system", "PayPal.ClientID"       , Sql.ToString(Application["CONFIG.PayPal.ClientID"       ]));
							SqlProcs.spCONFIG_Update("system", "PayPal.ClientSecret"   , Sql.ToString(Application["CONFIG.PayPal.ClientSecret"   ]));
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
			SetPageTitle(L10n.Term("PayPal.LBL_PAYPAL_SETTINGS"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);

					USER_NAME  .Text = Sql.ToString (Application["CONFIG.PayPal.APIUsername"    ]);
					PASSWORD   .Text = Sql.ToString (Application["CONFIG.PayPal.APIPassword"    ]);
					PRIVATE_KEY.Text = Sql.ToString (Application["CONFIG.PayPal.X509PrivateKey" ]);
					CERTIFICATE.Text = Sql.ToString (Application["CONFIG.PayPal.X509Certificate"]);
					SANDBOX.Checked  = Sql.ToBoolean(Application["CONFIG.PayPal.Sandbox"        ]);
					// 08/16/2015 Paul.  Add CARD_TOKEN for use with PayPal REST API. 
					REST_CLIENT_ID    .Text = Sql.ToString(Application["CONFIG.PayPal.ClientID"    ]);
					REST_CLIENT_SECRET.Text = Sql.ToString(Application["CONFIG.PayPal.ClientSecret"]);
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
			m_sMODULE = "PayPal";
			SetAdminMenu(m_sMODULE);
			if ( IsPostBack )
			{
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
			}
		}
		#endregion
	}
}
