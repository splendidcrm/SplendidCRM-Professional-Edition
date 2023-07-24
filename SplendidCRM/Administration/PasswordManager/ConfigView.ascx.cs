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

namespace SplendidCRM.Administration.PasswordManager
{
	/// <summary>
	///		Summary description for ConfigView.
	/// </summary>
	public class ConfigView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected TextBox      PREFERRED_PASSWORD_LENGTH    ;
		protected TextBox      MINIMUM_LOWER_CASE_CHARACTERS;
		protected TextBox      MINIMUM_UPPER_CASE_CHARACTERS;
		protected TextBox      MINIMUM_NUMERIC_CHARACTERS   ;
		protected TextBox      MINIMUM_SYMBOL_CHARACTERS    ;
		protected TextBox      SYMBOL_CHARACTERS            ;
		protected TextBox      COMPLEXITY_NUMBER            ;
		protected TextBox      HISTORY_MAXIMUM              ;
		protected TextBox      LOGIN_LOCKOUT_COUNT          ;
		protected TextBox      EXPIRATION_DAYS              ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				try
				{
					if ( Page.IsValid )
					{
						Application["CONFIG.Password.PreferredPasswordLength"   ] = Sql.ToInteger(PREFERRED_PASSWORD_LENGTH    .Text);
						Application["CONFIG.Password.MinimumLowerCaseCharacters"] = Sql.ToInteger(MINIMUM_LOWER_CASE_CHARACTERS.Text);
						Application["CONFIG.Password.MinimumUpperCaseCharacters"] = Sql.ToInteger(MINIMUM_UPPER_CASE_CHARACTERS.Text);
						Application["CONFIG.Password.MinimumNumericCharacters"  ] = Sql.ToInteger(MINIMUM_NUMERIC_CHARACTERS   .Text);
						Application["CONFIG.Password.MinimumSymbolCharacters"   ] = Sql.ToInteger(MINIMUM_SYMBOL_CHARACTERS    .Text);
						Application["CONFIG.Password.SymbolCharacters"          ] = SYMBOL_CHARACTERS.Text.Trim();
						Application["CONFIG.Password.ComplexityNumber"          ] = Sql.ToInteger(COMPLEXITY_NUMBER            .Text);
						Application["CONFIG.Password.HistoryMaximum"            ] = Sql.ToInteger(HISTORY_MAXIMUM              .Text);
						Application["CONFIG.Password.LoginLockoutCount"         ] = Sql.ToInteger(LOGIN_LOCKOUT_COUNT          .Text);
						Application["CONFIG.Password.ExpirationDays"            ] = Sql.ToInteger(EXPIRATION_DAYS              .Text);

						SqlProcs.spCONFIG_Update("security", "Password.PreferredPasswordLength"   , Sql.ToString(Application["CONFIG.Password.PreferredPasswordLength"   ]));
						SqlProcs.spCONFIG_Update("security", "Password.MinimumLowerCaseCharacters", Sql.ToString(Application["CONFIG.Password.MinimumLowerCaseCharacters"]));
						SqlProcs.spCONFIG_Update("security", "Password.MinimumUpperCaseCharacters", Sql.ToString(Application["CONFIG.Password.MinimumUpperCaseCharacters"]));
						SqlProcs.spCONFIG_Update("security", "Password.MinimumNumericCharacters"  , Sql.ToString(Application["CONFIG.Password.MinimumNumericCharacters"  ]));
						SqlProcs.spCONFIG_Update("security", "Password.MinimumSymbolCharacters"   , Sql.ToString(Application["CONFIG.Password.MinimumSymbolCharacters"   ]));
						SqlProcs.spCONFIG_Update("security", "Password.SymbolCharacters"          , Sql.ToString(Application["CONFIG.Password.SymbolCharacters"          ]));
						SqlProcs.spCONFIG_Update("security", "Password.ComplexityNumber"          , Sql.ToString(Application["CONFIG.Password.ComplexityNumber"          ]));
						SqlProcs.spCONFIG_Update("security", "Password.HistoryMaximum"            , Sql.ToString(Application["CONFIG.Password.HistoryMaximum"            ]));
						SqlProcs.spCONFIG_Update("security", "Password.LoginLockoutCount"         , Sql.ToString(Application["CONFIG.Password.LoginLockoutCount"         ]));
						SqlProcs.spCONFIG_Update("security", "Password.ExpirationDays"            , Sql.ToString(Application["CONFIG.Password.ExpirationDays"            ]));
						Response.Redirect("../default.aspx");
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
			SetPageTitle(L10n.Term("Administration.LBL_MANAGE_PASSWORD_TITLE"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess("config", "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);

					PREFERRED_PASSWORD_LENGTH    .Text = Sql.ToString(Application["CONFIG.Password.PreferredPasswordLength"   ]);
					MINIMUM_LOWER_CASE_CHARACTERS.Text = Sql.ToString(Application["CONFIG.Password.MinimumLowerCaseCharacters"]);
					MINIMUM_UPPER_CASE_CHARACTERS.Text = Sql.ToString(Application["CONFIG.Password.MinimumUpperCaseCharacters"]);
					MINIMUM_NUMERIC_CHARACTERS   .Text = Sql.ToString(Application["CONFIG.Password.MinimumNumericCharacters"  ]);
					MINIMUM_SYMBOL_CHARACTERS    .Text = Sql.ToString(Application["CONFIG.Password.MinimumSymbolCharacters"   ]);
					SYMBOL_CHARACTERS            .Text = Sql.ToString(Application["CONFIG.Password.SymbolCharacters"          ]);
					COMPLEXITY_NUMBER            .Text = Sql.ToString(Application["CONFIG.Password.ComplexityNumber"          ]);
					HISTORY_MAXIMUM              .Text = Sql.ToString(Application["CONFIG.Password.HistoryMaximum"            ]);
					LOGIN_LOCKOUT_COUNT          .Text = Sql.ToString(Application["CONFIG.Password.LoginLockoutCount"         ]);
					EXPIRATION_DAYS              .Text = Sql.ToString(Application["CONFIG.Password.ExpirationDays"            ]);
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
			m_sMODULE = "PasswordManager";
			// 07/24/2010 Paul.  We need an admin flag for the areas that don't have a record in the Modules table. 
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
