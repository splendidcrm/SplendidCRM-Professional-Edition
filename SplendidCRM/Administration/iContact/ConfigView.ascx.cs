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

namespace SplendidCRM.Administration.iContact
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
		protected TextBox      API_APP_ID            ;
		protected TextBox      API_USERNAME          ;
		protected TextBox      API_PASSWORD          ;
		protected TextBox      ICONTACT_ACCOUNT_ID   ;
		protected TextBox      ICONTACT_CLIENT_FOLDER_ID;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" || e.CommandName == "GetAccount" )
			{
				try
				{
					if ( Page.IsValid )
					{
						if ( e.CommandName == "Test" )
						{
							StringBuilder sbErrors = new StringBuilder();
							Spring.Social.iContact.iContactSync.ValidateiContact(Application, API_APP_ID.Text, API_USERNAME.Text, API_PASSWORD.Text, ICONTACT_ACCOUNT_ID.Text, ICONTACT_CLIENT_FOLDER_ID.Text, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
								ctlDynamicButtons.ErrorText = L10n.Term("iContact.LBL_TEST_SUCCESSFUL");
						}
						else if ( e.CommandName == "GetAccount" )
						{
							StringBuilder sbErrors = new StringBuilder();
							string sAccountID      = String.Empty;
							string sClientFolderID = String.Empty;
							Spring.Social.iContact.iContactSync.GetAccount(Application, API_APP_ID.Text, API_USERNAME.Text, API_PASSWORD.Text, ref sAccountID, ref sClientFolderID, sbErrors);
							if ( sbErrors.Length > 0 )
							{
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							}
							else
							{
								ICONTACT_ACCOUNT_ID      .Text = sAccountID     ;
								ICONTACT_CLIENT_FOLDER_ID.Text = sClientFolderID;
								ctlDynamicButtons.ErrorText = L10n.Term("iContact.LBL_ACCOUNT_RETRIEVED");
							}
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.iContact.Enabled"               ] = ENABLED                  .Checked;
							Application["CONFIG.iContact.VerboseStatus"         ] = VERBOSE_STATUS           .Checked;
							Application["CONFIG.iContact.Direction"             ] = DIRECTION                .SelectedValue;
							Application["CONFIG.iContact.ConflictResolution"    ] = CONFLICT_RESOLUTION      .SelectedValue;
							Application["CONFIG.iContact.SyncModules"           ] = SYNC_MODULES             .SelectedValue;
							Application["CONFIG.iContact.ApiAppId"              ] = API_APP_ID               .Text;
							Application["CONFIG.iContact.ApiUsername"           ] = API_USERNAME             .Text;
							Application["CONFIG.iContact.ApiPassword"           ] = API_PASSWORD             .Text;
							Application["CONFIG.iContact.iContactAccountId"     ] = ICONTACT_ACCOUNT_ID      .Text;
							Application["CONFIG.iContact.iContactClientFolderId"] = ICONTACT_CLIENT_FOLDER_ID.Text;
						
							SqlProcs.spCONFIG_Update("system", "iContact.Enabled"               , Sql.ToString(Application["CONFIG.iContact.Enabled"               ]));
							SqlProcs.spCONFIG_Update("system", "iContact.VerboseStatus"         , Sql.ToString(Application["CONFIG.iContact.VerboseStatus"         ]));
							SqlProcs.spCONFIG_Update("system", "iContact.Direction"             , Sql.ToString(Application["CONFIG.iContact.Direction"             ]));
							SqlProcs.spCONFIG_Update("system", "iContact.ConflictResolution"    , Sql.ToString(Application["CONFIG.iContact.ConflictResolution"    ]));
							SqlProcs.spCONFIG_Update("system", "iContact.SyncModules"           , Sql.ToString(Application["CONFIG.iContact.SyncModules"           ]));
							SqlProcs.spCONFIG_Update("system", "iContact.ApiAppId"              , Sql.ToString(Application["CONFIG.iContact.ApiAppId"              ]));
							SqlProcs.spCONFIG_Update("system", "iContact.ApiUsername"           , Sql.ToString(Application["CONFIG.iContact.ApiUsername"           ]));
							SqlProcs.spCONFIG_Update("system", "iContact.ApiPassword"           , Sql.ToString(Application["CONFIG.iContact.ApiPassword"           ]));
							SqlProcs.spCONFIG_Update("system", "iContact.iContactAccountId"     , Sql.ToString(Application["CONFIG.iContact.iContactAccountId"     ]));
							SqlProcs.spCONFIG_Update("system", "iContact.iContactClientFolderId", Sql.ToString(Application["CONFIG.iContact.iContactClientFolderId"]));
#if !DEBUG
							SqlProcs.spSCHEDULERS_UpdateStatus("function::polliContact", ENABLED.Checked ? "Active" : "Inactive");
#endif
							// 07/15/2017 Paul.  Instead of requiring that the user manually enable the user, do so automatically. 
							if ( Sql.ToBoolean(Application["CONFIG.iContact.Enabled"]) )
							{
								Guid gUSER_ID = Spring.Social.iContact.iContactSync.iContactUserID(Application);
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
			SetPageTitle(L10n.Term("iContact.LBL_MANAGE_iContact_TITLE"));
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
					// 05/02/2015 Paul.  iContact does not provide a modification date, so we cannot do by-directional sync. 
					//DIRECTION.DataSource = SplendidCache.List("iContact_sync_direction");
					//DIRECTION.DataBind();
					DIRECTION.Items.Add(new ListItem(L10n.Term(".icontact_sync_direction.from crm only"), "from crm only"));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(String.Empty                      ), String.Empty));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.remote"), "remote"    ));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.local" ), "local"     ));
					SYNC_MODULES.DataSource = SplendidCache.List("icontact_sync_module");
					SYNC_MODULES.DataBind();
					
					ENABLED                  .Checked = Sql.ToBoolean(Application["CONFIG.iContact.Enabled"               ]);
					VERBOSE_STATUS           .Checked = Sql.ToBoolean(Application["CONFIG.iContact.VerboseStatus"         ]);
					API_APP_ID               .Text    = Sql.ToString (Application["CONFIG.iContact.ApiAppId"              ]);
					API_USERNAME             .Text    = Sql.ToString (Application["CONFIG.iContact.ApiUsername"           ]);
					API_PASSWORD             .Text    = Sql.ToString (Application["CONFIG.iContact.ApiPassword"           ]);
					ICONTACT_ACCOUNT_ID      .Text    = Sql.ToString (Application["CONFIG.iContact.iContactAccountId"     ]);
					ICONTACT_CLIENT_FOLDER_ID.Text    = Sql.ToString (Application["CONFIG.iContact.iContactClientFolderId"]);
					try
					{
						Utils.SetSelectedValue(DIRECTION, Sql.ToString(Application["CONFIG.iContact.Direction"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(CONFLICT_RESOLUTION, Sql.ToString(Application["CONFIG.iContact.ConflictResolution"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(SYNC_MODULES, Sql.ToString(Application["CONFIG.iContact.SyncModules"]));
					}
					catch
					{
					}
				}
				
				// 04/26/2016 Paul.  Instead of building the URL in the code-behind, just manually construct in JavaScript so that postback would not be necessary after changing Portal ID or Client ID. 
				//string sRedirectURL = Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) + "Import/OAuthLanding.aspx";
				//Spring.Social.iContact.Connect.iContactServiceProvider iContactServiceProvider = new Spring.Social.iContact.Connect.iContactServiceProvider(OAUTH_CLIENT_ID.Text, OAUTH_CLIENT_SECRET.Text);
				//Spring.Social.OAuth2.OAuth2Parameters parameters = new Spring.Social.OAuth2.OAuth2Parameters()
				//{
				//	RedirectUrl = sRedirectURL,
				//	Scope = "contacts-rw offline"
				//};
				//parameters.Add("portalId", APP_ID.Text);
				//string authenticateUrl = iContactServiceProvider.OAuthOperations.BuildAuthorizeUrl(Spring.Social.OAuth2.GrantType.ImplicitGrant, parameters);
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
			m_sMODULE = "iContact";
			SetAdminMenu(m_sMODULE);
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
			ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
		}
		#endregion
	}
}
