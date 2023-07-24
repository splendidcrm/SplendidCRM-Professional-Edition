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
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SplendidCRM.Administration.ConstantContact
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
		protected TextBox      OAUTH_CLIENT_ID       ;
		protected TextBox      OAUTH_CLIENT_SECRET   ;
		protected TextBox      OAUTH_ACCESS_TOKEN    ;
		// 11/11/2019 Paul.  ConstantContact v3 API. 
		protected TextBox      OAUTH_REFRESH_TOKEN   ;
		protected TextBox      AUTHORIZATION_CODE    ;
		protected Button       btnGetAccessToken     ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 11/11/2019 Paul.  ConstantContact v3 API. 
			if ( e.CommandName == "GetAccessToken" )
			{
				try
				{
					// 11/11/2019 Paul.  ConstantContact v3 API. 
					// https://v3.developer.constantcontact.com/api_guide/server_flow.html
					HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create("https://idfed.constantcontact.com/as/token.oauth2");
					objRequest.Headers.Add("cache-control", "no-cache");
					objRequest.KeepAlive         = false;
					objRequest.AllowAutoRedirect = false;
					objRequest.Timeout           = 120000;  // 120 seconds
					objRequest.ContentType       = "application/x-www-form-urlencoded";
					objRequest.Method            = "POST";
					objRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(OAUTH_CLIENT_ID.Text + ":" + OAUTH_CLIENT_SECRET.Text));
					
					string sREDIRECT_URL = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Administration/ConstantContact/OAuthLanding.aspx";
					string sData = "grant_type=authorization_code&code=" + AUTHORIZATION_CODE.Text + "&redirect_uri=" + HttpUtility.UrlEncode(sREDIRECT_URL);
					objRequest.ContentLength = sData.Length;
					using ( StreamWriter stm = new StreamWriter(objRequest.GetRequestStream(), System.Text.Encoding.ASCII) )
					{
						stm.Write(sData);
					}
					
					string sResponse = String.Empty;
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
									JavaScriptSerializer json = new JavaScriptSerializer();
									Spring.Social.ConstantContact.RefreshToken token = json.Deserialize<Spring.Social.ConstantContact.RefreshToken>(sResponse);
									OAUTH_ACCESS_TOKEN.Text  = token.access_token ;
									OAUTH_REFRESH_TOKEN.Text = token.refresh_token;
									ctlDynamicButtons.ErrorText = L10n.Term("ConstantContact.LBL_CONNECTION_SUCCESSFUL");
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Save" || e.CommandName == "Test"  || e.CommandName == "RefreshToken" )
			{
				try
				{
					if ( Page.IsValid )
					{
						if ( e.CommandName == "RefreshToken" )
						{
							StringBuilder sbErrors = new StringBuilder();
							Spring.Social.ConstantContact.ConstantContactSync.RefreshAccessToken(Application, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
							{
								OAUTH_ACCESS_TOKEN .Text    = Sql.ToString (Application["CONFIG.ConstantContact.OAuthAccessToken" ]);
								OAUTH_REFRESH_TOKEN.Text    = Sql.ToString (Application["CONFIG.ConstantContact.OAuthRefreshToken"]);
								ctlDynamicButtons.ErrorText = L10n.Term("ConstantContact.LBL_CONNECTION_SUCCESSFUL");
							}
						}
						else if ( e.CommandName == "Test" )
						{
							StringBuilder sbErrors = new StringBuilder();
							// 11/11/2019 Paul.  Always refresh before we validate so that we do not need to worry about token expiration. 
							Spring.Social.ConstantContact.ConstantContactSync.RefreshAccessToken(Application, sbErrors);
							if ( sbErrors.Length == 0 )
							{
								OAUTH_ACCESS_TOKEN.Text = Sql.ToString (Application["CONFIG.ConstantContact.OAuthAccessToken"]);
								Spring.Social.ConstantContact.ConstantContactSync.ValidateConstantContact(Application, String.Empty, OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text, OAUTH_ACCESS_TOKEN.Text, sbErrors);
							}

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
								ctlDynamicButtons.ErrorText = L10n.Term("ConstantContact.LBL_TEST_SUCCESSFUL");
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.ConstantContact.Enabled"           ] = ENABLED            .Checked;
							Application["CONFIG.ConstantContact.VerboseStatus"     ] = VERBOSE_STATUS     .Checked;
							Application["CONFIG.ConstantContact.Direction"         ] = DIRECTION          .SelectedValue;
							Application["CONFIG.ConstantContact.ConflictResolution"] = CONFLICT_RESOLUTION.SelectedValue;
							Application["CONFIG.ConstantContact.SyncModules"       ] = SYNC_MODULES       .SelectedValue;
							Application["CONFIG.ConstantContact.ClientID"          ] = OAUTH_CLIENT_ID    .Text;
							Application["CONFIG.ConstantContact.ClientSecret"      ] = OAUTH_CLIENT_SECRET.Text;
							Application["CONFIG.ConstantContact.OAuthAccessToken"  ] = OAUTH_ACCESS_TOKEN .Text;
							// 11/11/2019 Paul.  ConstantContact v3 API. 
							Application["CONFIG.ConstantContact.OAuthRefreshToken" ] = OAUTH_REFRESH_TOKEN.Text;
							// 05/04/2015 Paul.  Saving the settings will reset the default list. 
							Application["CONFIG.ConstantContact.DefaultListID"     ] = String.Empty;
						
							SqlProcs.spCONFIG_Update("system", "ConstantContact.Enabled"           , Sql.ToString(Application["CONFIG.ConstantContact.Enabled"           ]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.VerboseStatus"     , Sql.ToString(Application["CONFIG.ConstantContact.VerboseStatus"     ]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.Direction"         , Sql.ToString(Application["CONFIG.ConstantContact.Direction"         ]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.ConflictResolution", Sql.ToString(Application["CONFIG.ConstantContact.ConflictResolution"]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.SyncModules"       , Sql.ToString(Application["CONFIG.ConstantContact.SyncModules"       ]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.ClientID"          , Sql.ToString(Application["CONFIG.ConstantContact.ClientID"          ]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.ClientSecret"      , Sql.ToString(Application["CONFIG.ConstantContact.ClientSecret"      ]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.OAuthAccessToken"  , Sql.ToString(Application["CONFIG.ConstantContact.OAuthAccessToken"  ]));
							SqlProcs.spCONFIG_Update("system", "ConstantContact.OAuthRefreshToken" , Sql.ToString(Application["CONFIG.ConstantContact.OAuthRefreshToken" ]));
#if !DEBUG
							SqlProcs.spSCHEDULERS_UpdateStatus("function::pollConstantContact", ENABLED.Checked ? "Active" : "Inactive");
#endif
							// 07/15/2017 Paul.  Instead of requiring that the user manually enable the user, do so automatically. 
							if ( Sql.ToBoolean(Application["CONFIG.ConstantContact.Enabled"]) )
							{
								Guid gUSER_ID = Spring.Social.ConstantContact.ConstantContactSync.ConstantContactUserID(Application);
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
			SetPageTitle(L10n.Term("ConstantContact.LBL_MANAGE_CONSTANTCONTACT_TITLE"));
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
					DIRECTION.DataSource = SplendidCache.List("constantcontact_sync_direction");
					DIRECTION.DataBind();
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(String.Empty                      ), String.Empty));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.remote"), "remote"    ));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.local" ), "local"     ));
					SYNC_MODULES.DataSource = SplendidCache.List("constantcontact_sync_module");
					SYNC_MODULES.DataBind();
					
					ENABLED            .Checked = Sql.ToBoolean(Application["CONFIG.ConstantContact.Enabled"          ]);
					VERBOSE_STATUS     .Checked = Sql.ToBoolean(Application["CONFIG.ConstantContact.VerboseStatus"    ]);
					OAUTH_CLIENT_ID    .Text    = Sql.ToString (Application["CONFIG.ConstantContact.ClientID"         ]);
					OAUTH_CLIENT_SECRET.Text    = Sql.ToString (Application["CONFIG.ConstantContact.ClientSecret"     ]);
					OAUTH_ACCESS_TOKEN .Text    = Sql.ToString (Application["CONFIG.ConstantContact.OAuthAccessToken" ]);
					// 11/11/2019 Paul.  ConstantContact v3 API. 
					OAUTH_REFRESH_TOKEN.Text    = Sql.ToString(Application["CONFIG.ConstantContact.OAuthRefreshToken" ]);
					try
					{
						Utils.SetSelectedValue(DIRECTION, Sql.ToString(Application["CONFIG.ConstantContact.Direction"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(CONFLICT_RESOLUTION, Sql.ToString(Application["CONFIG.ConstantContact.ConflictResolution"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(SYNC_MODULES, Sql.ToString(Application["CONFIG.ConstantContact.SyncModules"]));
					}
					catch
					{
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
			m_sMODULE = "ConstantContact";
			SetAdminMenu(m_sMODULE);
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
			ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
		}
		#endregion
	}
}
