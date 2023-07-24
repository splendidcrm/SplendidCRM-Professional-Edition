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

namespace SplendidCRM.Administration.Marketo
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
		protected DropDownList DIRECTION             ;
		protected DropDownList CONFLICT_RESOLUTION   ;
		protected DropDownList SYNC_MODULES          ;
		protected TextBox      OAUTH_ENDPOINT_URL    ;
		protected TextBox      OAUTH_IDENTITY_URL    ;
		protected TextBox      OAUTH_CLIENT_ID       ;
		protected TextBox      OAUTH_CLIENT_SECRET   ;
		protected TextBox      OAUTH_ACCESS_TOKEN    ;
		protected TextBox      OAUTH_SCOPE           ;
		protected TextBox      OAUTH_EXPIRES_AT      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" || e.CommandName == "Authorize" )
			{
				try
				{
					if ( Page.IsValid )
					{
						if ( e.CommandName == "Authorize" )
						{
							StringBuilder sbErrors = new StringBuilder();
							string sACCESS_TOKEN = String.Empty;
							string sEXPIRES_AT   = String.Empty;
							string sSCOPE        = String.Empty;
							Spring.Social.Marketo.MarketoSync.AuthorizeMarketo(Application, OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text, OAUTH_ENDPOINT_URL.Text,  OAUTH_IDENTITY_URL.Text, ref sACCESS_TOKEN, ref sEXPIRES_AT, ref sSCOPE, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
							{
								ctlDynamicButtons.ErrorText = L10n.Term("Marketo.LBL_AUTHORIZE_SUCCESSFUL");
								OAUTH_ACCESS_TOKEN.Text = sACCESS_TOKEN;
								OAUTH_EXPIRES_AT  .Text = sEXPIRES_AT  ;
								OAUTH_SCOPE       .Text = sSCOPE       ;
							}
						}
						else if ( e.CommandName == "Test" )
						{
							StringBuilder sbErrors = new StringBuilder();
							Spring.Social.Marketo.MarketoSync.ValidateMarketo(Application, OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text, OAUTH_ENDPOINT_URL.Text, OAUTH_IDENTITY_URL.Text, OAUTH_ACCESS_TOKEN.Text, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
								ctlDynamicButtons.ErrorText = L10n.Term("Marketo.LBL_TEST_SUCCESSFUL");
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.Marketo.Enabled"           ] = ENABLED            .Checked;
							Application["CONFIG.Marketo.VerboseStatus"     ] = VERBOSE_STATUS     .Checked;
							Application["CONFIG.Marketo.Direction"         ] = DIRECTION          .SelectedValue;
							Application["CONFIG.Marketo.ConflictResolution"] = CONFLICT_RESOLUTION.SelectedValue;
							Application["CONFIG.Marketo.SyncModules"       ] = SYNC_MODULES       .SelectedValue;
							Application["CONFIG.Marketo.EndpointURL"       ] = OAUTH_ENDPOINT_URL .Text;
							Application["CONFIG.Marketo.IdentityURL"       ] = OAUTH_IDENTITY_URL .Text;
							Application["CONFIG.Marketo.ClientID"          ] = OAUTH_CLIENT_ID    .Text;
							Application["CONFIG.Marketo.ClientSecret"      ] = OAUTH_CLIENT_SECRET.Text;
							Application["CONFIG.Marketo.OAuthAccessToken"  ] = OAUTH_ACCESS_TOKEN .Text;
							Application["CONFIG.Marketo.OAuthExpiresAt"    ] = OAUTH_EXPIRES_AT   .Text;
							Application["CONFIG.Marketo.OAuthScope"        ] = OAUTH_SCOPE        .Text;
						
							SqlProcs.spCONFIG_Update("system", "Marketo.Enabled"           , Sql.ToString(Application["CONFIG.Marketo.Enabled"           ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.VerboseStatus"     , Sql.ToString(Application["CONFIG.Marketo.VerboseStatus"     ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.Direction"         , Sql.ToString(Application["CONFIG.Marketo.Direction"         ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.ConflictResolution", Sql.ToString(Application["CONFIG.Marketo.ConflictResolution"]));
							SqlProcs.spCONFIG_Update("system", "Marketo.SyncModules"       , Sql.ToString(Application["CONFIG.Marketo.SyncModules"       ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.EndpointURL"       , Sql.ToString(Application["CONFIG.Marketo.EndpointURL"       ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.IdentityURL"       , Sql.ToString(Application["CONFIG.Marketo.IdentityURL"       ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.ClientID"          , Sql.ToString(Application["CONFIG.Marketo.ClientID"          ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.ClientSecret"      , Sql.ToString(Application["CONFIG.Marketo.ClientSecret"      ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.OAuthAccessToken"  , Sql.ToString(Application["CONFIG.Marketo.OAuthAccessToken"  ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.OAuthExpiresAt"    , Sql.ToString(Application["CONFIG.Marketo.OAuthExpiresAt"    ]));
							SqlProcs.spCONFIG_Update("system", "Marketo.OAuthScope"        , Sql.ToString(Application["CONFIG.Marketo.OAuthScope"        ]));
#if !DEBUG
							SqlProcs.spSCHEDULERS_UpdateStatus("function::pollMarketo", ENABLED.Checked ? "Active" : "Inactive");
#endif
							// 07/15/2017 Paul.  Instead of requiring that the user manually enable the user, do so automatically. 
							if ( Sql.ToBoolean(Application["CONFIG.Marketo.Enabled"]) )
							{
								Guid gUSER_ID = Spring.Social.Marketo.MarketoSync.MarketoUserID(Application);
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									string sSQL;
									sSQL = "select STATUS       " + ControlChars.CrLf
									     + "  from vwUSERS      " + ControlChars.CrLf
									     + " where ID = @ID     " + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@ID", gUSER_ID);
										string sSTATUS = Sql.ToString(cmd.ExecuteScalar());
										if ( sSTATUS != "Active" )
										{
											SqlProcs.spUSERS_UpdateStatus(gUSER_ID, "Active");
										}
									}
								}
							}
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
			SetPageTitle(L10n.Term("Marketo.LBL_MANAGE_MARKETO_TITLE"));
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
					DIRECTION.DataSource = SplendidCache.List("marketo_sync_direction");
					DIRECTION.DataBind();
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(String.Empty                      ), String.Empty));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.remote"), "remote"    ));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.local" ), "local"     ));
					SYNC_MODULES.DataSource = SplendidCache.List("marketo_sync_module");
					SYNC_MODULES.DataBind();
					
					ENABLED            .Checked = Sql.ToBoolean(Application["CONFIG.Marketo.Enabled"          ]);
					VERBOSE_STATUS     .Checked = Sql.ToBoolean(Application["CONFIG.Marketo.VerboseStatus"    ]);
					OAUTH_ENDPOINT_URL .Text    = Sql.ToString (Application["CONFIG.Marketo.EndpointURL"      ]);
					OAUTH_IDENTITY_URL .Text    = Sql.ToString (Application["CONFIG.Marketo.IdentityURL"      ]);
					OAUTH_CLIENT_ID    .Text    = Sql.ToString (Application["CONFIG.Marketo.ClientID"         ]);
					OAUTH_CLIENT_SECRET.Text    = Sql.ToString (Application["CONFIG.Marketo.ClientSecret"     ]);
					OAUTH_ACCESS_TOKEN .Text    = Sql.ToString (Application["CONFIG.Marketo.OAuthAccessToken" ]);
					OAUTH_SCOPE        .Text    = Sql.ToString (Application["CONFIG.Marketo.OAuthScope"       ]);
					OAUTH_EXPIRES_AT   .Text    = Sql.ToString (Application["CONFIG.Marketo.OAuthExpiresAt"   ]);
					try
					{
						Utils.SetSelectedValue(DIRECTION, Sql.ToString(Application["CONFIG.Marketo.Direction"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(CONFLICT_RESOLUTION, Sql.ToString(Application["CONFIG.Marketo.ConflictResolution"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(SYNC_MODULES, Sql.ToString(Application["CONFIG.Marketo.SyncModules"]));
					}
					catch
					{
					}
				}
				
				// 04/26/2016 Paul.  Instead of building the URL in the code-behind, just manually construct in JavaScript so that postback would not be necessary after changing Portal ID or Client ID. 
				//string sRedirectURL = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Import/OAuthLanding.aspx";
				//Spring.Social.Marketo.Connect.MarketoServiceProvider hubSpotServiceProvider = new Spring.Social.Marketo.Connect.MarketoServiceProvider(OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text);
				//Spring.Social.OAuth2.OAuth2Parameters parameters = new Spring.Social.OAuth2.OAuth2Parameters()
				//{
				//	RedirectUrl = sRedirectURL,
				//	Scope = "contacts-rw offline"
				//};
				//parameters.Add("InstanceURL", OAUTH_INSTANCE_URL.Text);
				//string authenticateUrl = hubSpotServiceProvider.OAuthOperations.BuildAuthorizeUrl(Spring.Social.OAuth2.GrantType.ImplicitGrant, parameters);
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
			m_sMODULE = "Marketo";
			SetAdminMenu(m_sMODULE);
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
			ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
		}
		#endregion
	}
}
