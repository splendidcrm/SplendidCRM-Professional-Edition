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
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.CurrencyLayer
{
	/// <summary>
	///		Summary description for ConfigView.
	/// </summary>
	public class ConfigView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected CheckBox     LOG_CONVERSIONS       ;
		protected TextBox      ACCESS_KEY            ;
		protected TextBox      RATE_LIFETIME         ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" )
			{
				try
				{
					if ( Page.IsValid )
					{
						if ( e.CommandName == "Test" )
						{
							StringBuilder sbErrors = new StringBuilder();
							OrderUtils.GetCurrencyConversionRate(Application, ACCESS_KEY.Text, false, "USD", "EUR", sbErrors);
							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
								ctlDynamicButtons.ErrorText = L10n.Term("CurrencyLayer.LBL_TEST_SUCCESSFUL");
						}
						else if ( e.CommandName == "Save" )
						{
							if ( Sql.ToInteger(RATE_LIFETIME.Text.Trim()) <= 0 )
								RATE_LIFETIME.Text = "90";
							Application["CONFIG.CurrencyLayer.LogConversions"] = LOG_CONVERSIONS.Checked;
							Application["CONFIG.CurrencyLayer.AccessKey"     ] = ACCESS_KEY     .Text.Trim();
							Application["CONFIG.CurrencyLayer.RateLifetime"  ] = RATE_LIFETIME  .Text   ;
						
							SqlProcs.spCONFIG_Update("system", "CurrencyLayer.LogConversions", Sql.ToString(Application["CONFIG.CurrencyLayer.LogConversions"]));
							SqlProcs.spCONFIG_Update("system", "CurrencyLayer.AccessKey"     , Sql.ToString(Application["CONFIG.CurrencyLayer.AccessKey"     ]));
							SqlProcs.spCONFIG_Update("system", "CurrencyLayer.RateLifetime"  , Sql.ToString(Application["CONFIG.CurrencyLayer.RateLifetime"  ]));
							Response.Redirect("default.aspx");
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
				Response.Redirect("default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("CurrencyLayer.LBL_MANAGE_CURRENCYLAYER_TITLE"));
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
					LOG_CONVERSIONS.Checked = Sql.ToBoolean(Application["CONFIG.CurrencyLayer.LogConversions"]);
					ACCESS_KEY     .Text    = Sql.ToString (Application["CONFIG.CurrencyLayer.AccessKey"     ]);
					RATE_LIFETIME  .Text    = Sql.ToString (Application["CONFIG.CurrencyLayer.RateLifetime"  ]);
					if ( Sql.ToInteger(RATE_LIFETIME.Text.Trim()) <= 0 )
						RATE_LIFETIME.Text = "90";
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
			m_sMODULE = "CurrencyLayer";
			SetAdminMenu(m_sMODULE);
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
			ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
		}
		#endregion
	}
}
