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
using System.IO;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SplendidCRM.Administration.HubSpot
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
		protected TextBox      OAUTH_PORTAL_ID       ;
		protected TextBox      OAUTH_CLIENT_ID       ;
		protected TextBox      OAUTH_CLIENT_SECRET   ;
		protected TextBox      OAUTH_ACCESS_TOKEN    ;
		protected TextBox      OAUTH_REFRESH_TOKEN   ;
		protected TextBox      OAUTH_EXPIRES_AT      ;
		protected TextBox      AUTHORIZATION_CODE    ;
		protected Button       btnGetAccessToken     ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 11/12/2019 Paul.  Update HubSpot OAuth. 
			if ( e.CommandName == "GetAccessToken" )
			{
				// 11/11/2019 Paul.  TLS12 is now requird. 
				if ( !ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) )
				{
					ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
				}
				try
				{
					// https://developers.hubspot.com/docs/methods/oauth2/get-access-and-refresh-tokens
					// https://legacydocs.hubspot.com/docs/methods/oauth2/oauth2-quickstart#step1directtohubspotsoauth20server
					// 09/26/2020 Paul.  URL changed to api.hubapi.com. 
					HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create("https://api.hubapi.com/oauth/v1/token");
					objRequest.Headers.Add("cache-control", "no-cache");
					objRequest.KeepAlive         = false;
					objRequest.AllowAutoRedirect = false;
					objRequest.Timeout           = 120000;  // 120 seconds
					objRequest.ContentType       = "application/x-www-form-urlencoded";
					objRequest.Method            = "POST";
					
					string sREDIRECT_URL = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Administration/HubSpot/OAuthLanding.aspx";
					string sData = "grant_type=authorization_code&client_id=" + OAUTH_CLIENT_ID.Text + "&client_secret=" + OAUTH_CLIENT_SECRET.Text + "&code=" + AUTHORIZATION_CODE.Text + "&redirect_uri=" + HttpUtility.UrlEncode(sREDIRECT_URL);
					objRequest.ContentLength = sData.Length;
					using ( StreamWriter stm = new StreamWriter(objRequest.GetRequestStream(), System.Text.Encoding.ASCII) )
					{
						stm.Write(sData);
					}
					
					string sResponse = String.Empty;
					// 11/12/2019 Paul.  Cannot connect. 
					// CloudFront: The distribution supports only cachable requests
					using ( HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse() )
					{
						if ( objResponse != null )
						{
							if ( objResponse.StatusCode != HttpStatusCode.OK && objResponse.StatusCode != HttpStatusCode.Found )
							{
								throw(new Exception(objResponse.StatusCode + " " + objResponse.StatusDescription));
							}
							else
							{
								using ( StreamReader stm = new StreamReader(objResponse.GetResponseStream()) )
								{
									sResponse = stm.ReadToEnd();
									// Access tokens expire after 6 hours,
									JavaScriptSerializer json = new JavaScriptSerializer();
									Spring.Social.HubSpot.RefreshToken token = json.Deserialize<Spring.Social.HubSpot.RefreshToken>(sResponse);
									OAUTH_ACCESS_TOKEN .Text = token.access_token ;
									OAUTH_REFRESH_TOKEN.Text = token.refresh_token;
									DateTime dtOAuthExpiresAt = DateTime.Now.AddHours(6);
									OAUTH_EXPIRES_AT.Text = dtOAuthExpiresAt.ToShortDateString() + " " + dtOAuthExpiresAt.ToShortTimeString();
								}
							}
						}
					}
				}
				catch(WebException ex)
				{
					string sResponse = String.Empty;
					using (Stream stream = ex.Response.GetResponseStream() )
					{
						using ( StreamReader reader = new StreamReader(stream) )
						{
							sResponse = reader.ReadToEnd();
						}
					}
					ctlDynamicButtons.ErrorText = ex.Message + " " + sResponse;
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Save" || e.CommandName == "Test" || e.CommandName == "RefreshToken" )
			{
				try
				{
					if ( Page.IsValid )
					{
						if ( e.CommandName == "RefreshToken" )
						{
							StringBuilder sbErrors = new StringBuilder();
							Spring.Social.HubSpot.HubSpotSync.RefreshAccessToken(Application, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
							{
								OAUTH_ACCESS_TOKEN .Text    = Sql.ToString (Application["CONFIG.HubSpot.OAuthAccessToken" ]);
								OAUTH_REFRESH_TOKEN.Text    = Sql.ToString (Application["CONFIG.HubSpot.OAuthRefreshToken"]);
								OAUTH_EXPIRES_AT   .Text    = Sql.ToString (Application["CONFIG.HubSpot.OAuthExpiresAt"   ]);
								ctlDynamicButtons.ErrorText = L10n.Term("HubSpot.LBL_TEST_SUCCESSFUL");
							}
						}
						else if ( e.CommandName == "Test" )
						{
							StringBuilder sbErrors = new StringBuilder();
							Spring.Social.HubSpot.HubSpotSync.ValidateHubSpot(Application, OAUTH_PORTAL_ID.Text, OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text, OAUTH_ACCESS_TOKEN.Text, sbErrors);
							//Spring.Social.HubSpot.HubSpotSync.RefreshAccessToken(Application, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
								ctlDynamicButtons.ErrorText = L10n.Term("HubSpot.LBL_TEST_SUCCESSFUL");
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.HubSpot.Enabled"           ] = ENABLED            .Checked;
							Application["CONFIG.HubSpot.VerboseStatus"     ] = VERBOSE_STATUS     .Checked;
							Application["CONFIG.HubSpot.Direction"         ] = DIRECTION          .SelectedValue;
							Application["CONFIG.HubSpot.ConflictResolution"] = CONFLICT_RESOLUTION.SelectedValue;
							Application["CONFIG.HubSpot.SyncModules"       ] = SYNC_MODULES       .SelectedValue;
							Application["CONFIG.HubSpot.PortalID"          ] = OAUTH_PORTAL_ID    .Text;
							Application["CONFIG.HubSpot.ClientID"          ] = OAUTH_CLIENT_ID    .Text;
							Application["CONFIG.HubSpot.ClientSecret"      ] = OAUTH_CLIENT_SECRET.Text;
							Application["CONFIG.HubSpot.OAuthAccessToken"  ] = OAUTH_ACCESS_TOKEN .Text;
							Application["CONFIG.HubSpot.OAuthRefreshToken" ] = OAUTH_REFRESH_TOKEN.Text;
							Application["CONFIG.HubSpot.OAuthExpiresAt"    ] = OAUTH_EXPIRES_AT   .Text;
						
							SqlProcs.spCONFIG_Update("system", "HubSpot.Enabled"           , Sql.ToString(Application["CONFIG.HubSpot.Enabled"           ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.VerboseStatus"     , Sql.ToString(Application["CONFIG.HubSpot.VerboseStatus"     ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.Direction"         , Sql.ToString(Application["CONFIG.HubSpot.Direction"         ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.ConflictResolution", Sql.ToString(Application["CONFIG.HubSpot.ConflictResolution"]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.SyncModules"       , Sql.ToString(Application["CONFIG.HubSpot.SyncModules"       ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.PortalID"          , Sql.ToString(Application["CONFIG.HubSpot.PortalID"          ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.ClientID"          , Sql.ToString(Application["CONFIG.HubSpot.ClientID"          ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.ClientSecret"      , Sql.ToString(Application["CONFIG.HubSpot.ClientSecret"      ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.OAuthAccessToken"  , Sql.ToString(Application["CONFIG.HubSpot.OAuthAccessToken"  ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.OAuthRefreshToken" , Sql.ToString(Application["CONFIG.HubSpot.OAuthRefreshToken" ]));
							SqlProcs.spCONFIG_Update("system", "HubSpot.OAuthExpiresAt"    , Sql.ToString(Application["CONFIG.HubSpot.OAuthExpiresAt"    ]));
#if !DEBUG
							SqlProcs.spSCHEDULERS_UpdateStatus("function::pollHubSpot", ENABLED.Checked ? "Active" : "Inactive");
#endif
							// 07/15/2017 Paul.  Instead of requiring that the user manually enable the user, do so automatically. 
							if ( Sql.ToBoolean(Application["CONFIG.HubSpot.Enabled"]) )
							{
								Guid gUSER_ID = Spring.Social.HubSpot.HubSpotSync.HubSpotUserID(Application);
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
			SetPageTitle(L10n.Term("HubSpot.LBL_MANAGE_HUBSPOT_TITLE"));
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
					DIRECTION.DataSource = SplendidCache.List("hubspot_sync_direction");
					DIRECTION.DataBind();
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(String.Empty                      ), String.Empty));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.remote"), "remote"    ));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.local" ), "local"     ));
					SYNC_MODULES.DataSource = SplendidCache.List("hubspot_sync_module");
					SYNC_MODULES.DataBind();
					
					ENABLED            .Checked = Sql.ToBoolean(Application["CONFIG.HubSpot.Enabled"          ]);
					VERBOSE_STATUS     .Checked = Sql.ToBoolean(Application["CONFIG.HubSpot.VerboseStatus"    ]);
					OAUTH_PORTAL_ID    .Text    = Sql.ToString (Application["CONFIG.HubSpot.PortalID"         ]);
					OAUTH_CLIENT_ID    .Text    = Sql.ToString (Application["CONFIG.HubSpot.ClientID"         ]);
					OAUTH_CLIENT_SECRET.Text    = Sql.ToString (Application["CONFIG.HubSpot.ClientSecret"     ]);
					OAUTH_ACCESS_TOKEN .Text    = Sql.ToString (Application["CONFIG.HubSpot.OAuthAccessToken" ]);
					OAUTH_REFRESH_TOKEN.Text    = Sql.ToString (Application["CONFIG.HubSpot.OAuthRefreshToken"]);
					OAUTH_EXPIRES_AT   .Text    = Sql.ToString (Application["CONFIG.HubSpot.OAuthExpiresAt"   ]);
					try
					{
						Utils.SetSelectedValue(DIRECTION, Sql.ToString(Application["CONFIG.HubSpot.Direction"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(CONFLICT_RESOLUTION, Sql.ToString(Application["CONFIG.HubSpot.ConflictResolution"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(SYNC_MODULES, Sql.ToString(Application["CONFIG.HubSpot.SyncModules"]));
					}
					catch
					{
					}
				}
				
				// 04/26/2016 Paul.  Instead of building the URL in the code-behind, just manually construct in JavaScript so that postback would not be necessary after changing Portal ID or Client ID. 
				//string sRedirectURL = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Import/OAuthLanding.aspx";
				//Spring.Social.HubSpot.Connect.HubSpotServiceProvider hubSpotServiceProvider = new Spring.Social.HubSpot.Connect.HubSpotServiceProvider(OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text);
				//Spring.Social.OAuth2.OAuth2Parameters parameters = new Spring.Social.OAuth2.OAuth2Parameters()
				//{
				//	RedirectUrl = sRedirectURL,
				//	Scope = "contacts-rw offline"
				//};
				//parameters.Add("portalId", OAUTH_PORTAL_ID.Text);
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
			m_sMODULE = "HubSpot";
			SetAdminMenu(m_sMODULE);
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
			ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
		}
		#endregion
	}
}
