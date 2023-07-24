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

namespace SplendidCRM.Administration.Google
{
	/// <summary>
	///		Summary description for ConfigView.
	/// </summary>
	public class ConfigView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected CheckBox     ENABLED               ;
		protected CheckBox     VERBOSE_STATUS        ;
		protected TextBox      API_KEY               ;
		protected TextBox      OAUTH_CLIENT_ID       ;
		protected TextBox      OAUTH_CLIENT_SECRET   ;
		protected CheckBox     PUSH_NOTIFICATIONS    ;
		protected TextBox      PUSH_NOTIFICATION_URL ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" || e.CommandName == "RefreshToken" )
			{
				try
				{
					if ( PUSH_NOTIFICATIONS.Checked )
					{
						if ( PUSH_NOTIFICATION_URL.Text.EndsWith("/") )
							PUSH_NOTIFICATION_URL.Text = PUSH_NOTIFICATION_URL.Text.Substring(0, PUSH_NOTIFICATION_URL.Text.Length - 1);
						if ( !PUSH_NOTIFICATION_URL.Text.ToLower().EndsWith("/googleoauth/google_webhook.aspx") )
							PUSH_NOTIFICATION_URL.Text += "/GoogleOAuth/Google_Webhook.aspx";
					}
					if ( Page.IsValid )
					{
						if ( e.CommandName == "RefreshToken" )
						{
							//StringBuilder sbErrors = new StringBuilder();
							//GoogleApps.RefreshAccessToken(Application, sbErrors);
							//
							//if ( sbErrors.Length > 0 )
							//	ctlDynamicButtons.ErrorText = sbErrors.ToString();
							//else
							//{
							//	ctlDynamicButtons.ErrorText = L10n.Term("GoogleApps.LBL_TEST_SUCCESSFUL");
							//}
						}
						else if ( e.CommandName == "Test" )
						{
							StringBuilder sbErrors = new StringBuilder();
							//GoogleApps.ValidateGoogleApps(Application, API_KEY.Text, OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text, OAUTH_ACCESS_TOKEN.Text, sbErrors);
							//Spring.Social.GoogleApps.GoogleAppsSync.RefreshAccessToken(Application, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
								ctlDynamicButtons.ErrorText = L10n.Term("GoogleApps.LBL_TEST_SUCCESSFUL");
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.GoogleApps.Enabled"            ] = ENABLED              .Checked;
							Application["CONFIG.GoogleApps.VerboseStatus"      ] = VERBOSE_STATUS       .Checked;
							Application["CONFIG.GoogleApps.ApiKey"             ] = API_KEY              .Text   ;
							Application["CONFIG.GoogleApps.ClientID"           ] = OAUTH_CLIENT_ID      .Text   ;
							Application["CONFIG.GoogleApps.ClientSecret"       ] = OAUTH_CLIENT_SECRET  .Text   ;
							Application["CONFIG.GoogleApps.PushNotifications"  ] = PUSH_NOTIFICATIONS   .Checked;
							Application["CONFIG.GoogleApps.PushNotificationURL"] = PUSH_NOTIFICATION_URL.Text   ;
						
							SqlProcs.spCONFIG_Update("system", "GoogleApps.Enabled"            , Sql.ToString(Application["CONFIG.GoogleApps.Enabled"            ]));
							SqlProcs.spCONFIG_Update("system", "GoogleApps.VerboseStatus"      , Sql.ToString(Application["CONFIG.GoogleApps.VerboseStatus"      ]));
							SqlProcs.spCONFIG_Update("system", "GoogleApps.ApiKey"             , Sql.ToString(Application["CONFIG.GoogleApps.ApiKey"             ]));
							SqlProcs.spCONFIG_Update("system", "GoogleApps.ClientID"           , Sql.ToString(Application["CONFIG.GoogleApps.ClientID"           ]));
							SqlProcs.spCONFIG_Update("system", "GoogleApps.ClientSecret"       , Sql.ToString(Application["CONFIG.GoogleApps.ClientSecret"       ]));
							SqlProcs.spCONFIG_Update("system", "GoogleApps.PushNotifications"  , Sql.ToString(Application["CONFIG.GoogleApps.PushNotifications"  ]));
							SqlProcs.spCONFIG_Update("system", "GoogleApps.PushNotificationURL", Sql.ToString(Application["CONFIG.GoogleApps.PushNotificationURL"]));
							// 09/15/2015 Paul.  Reset the Unauthorized flag. 
							GoogleSync.bUnauthorizedWebHook = false;
							
							// 09/15/2015 Paul.  Enable the ExchangeSync scheduled job. 
							SqlProcs.spSCHEDULERS_UpdateStatus("function::pollGoogleSync", "Active");
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
				Response.Redirect("../default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Google.LBL_MANAGE_GOOGLE_TITLE"));
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
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);

					ENABLED              .Checked = Sql.ToBoolean(Application["CONFIG.GoogleApps.Enabled"            ]);
					VERBOSE_STATUS       .Checked = Sql.ToBoolean(Application["CONFIG.GoogleApps.VerboseStatus"      ]);
					API_KEY              .Text    = Sql.ToString (Application["CONFIG.GoogleApps.ApiKey"             ]);
					OAUTH_CLIENT_ID      .Text    = Sql.ToString (Application["CONFIG.GoogleApps.ClientID"           ]);
					OAUTH_CLIENT_SECRET  .Text    = Sql.ToString (Application["CONFIG.GoogleApps.ClientSecret"       ]);
					PUSH_NOTIFICATIONS   .Checked = Sql.ToBoolean(Application["CONFIG.GoogleApps.PushNotifications"  ]);
					PUSH_NOTIFICATION_URL.Text    = Sql.ToString (Application["CONFIG.GoogleApps.PushNotificationURL"]);
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
			m_sMODULE = "Google";
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
