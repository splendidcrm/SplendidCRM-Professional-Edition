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
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Diagnostics;

using DuoUniversal;

namespace SplendidCRM.Administration.Duo
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
		protected TextBox      CLIENT_ID             ;
		protected TextBox      CLIENT_SECRET         ;
		protected TextBox      API_HOST_URL          ;
		protected TextBox      REDIRECT_URL          ;

		private string GetDuoRedirectUrl()
		{
			string sServerScheme    = Sql.ToString(Application["ServerScheme"   ]);
			string sServerName      = Sql.ToString(Application["ServerName"     ]);
			string sApplicationPath = Sql.ToString(Application["ApplicationPath"]);
			string sServerPort      = Sql.ToString(Application["ServerPort"     ]);
			string sSiteURL         = sServerScheme + "://" + sServerName + sServerPort + sApplicationPath;
			if ( !sSiteURL.StartsWith("http") )
				sSiteURL = "http://" + sSiteURL;
			if ( !sSiteURL.EndsWith("/") )
				sSiteURL += "/";

			string sRedirectURL = sSiteURL;
			if ( sServerName != "localhost" )
			{
				sRedirectURL  = Crm.Config.SiteURL(Application);
			}
			sRedirectURL += "administration/duouniversal/default.aspx";
			return sRedirectURL;
		}

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
							string sRedirectURL = GetDuoRedirectUrl();
							Client duoClient = new DuoUniversal.ClientBuilder(CLIENT_ID.Text, CLIENT_SECRET.Text, API_HOST_URL.Text, sRedirectURL).Build();
							Task<bool> resultTask = Task.Run(() => duoClient.DoHealthCheck());
							if ( resultTask.Result )
							{
								string state = DuoUniversal.Client.GenerateState();
								Session["DuoUniversal.state"   ] = state;
								Session["DuoUniversal.username"] = Security.USER_NAME;
								string promptUri = duoClient.GenerateAuthUri(Security.USER_NAME, state);
								Response.Redirect(promptUri);
							}
							else
							{
								ctlDynamicButtons.ErrorText = L10n.Term("DuoUniversal.ERR_FAILED_HEALTH_CHECK");
							}
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.DuoUniversal.Enabled"      ] = ENABLED        .Checked;
							Application["CONFIG.DuoUniversal.ClientID"     ] = CLIENT_ID      .Text   ;
							Application["CONFIG.DuoUniversal.ClientSecret" ] = CLIENT_SECRET  .Text   ;
							Application["CONFIG.DuoUniversal.ApiHostURL"   ] = API_HOST_URL   .Text   ;
						
							SqlProcs.spCONFIG_Update("system", "DuoUniversal.Enabled"      , Sql.ToString(Application["CONFIG.DuoUniversal.Enabled"      ]));
							SqlProcs.spCONFIG_Update("system", "DuoUniversal.ClientID"     , Sql.ToString(Application["CONFIG.DuoUniversal.ClientID"     ]));
							SqlProcs.spCONFIG_Update("system", "DuoUniversal.ClientSecret" , Sql.ToString(Application["CONFIG.DuoUniversal.ClientSecret" ]));
							SqlProcs.spCONFIG_Update("system", "DuoUniversal.ApiHostURL"   , Sql.ToString(Application["CONFIG.DuoUniversal.ApiHostURL"   ]));
							Response.Redirect("../default.aspx");
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
					if ( ex.InnerException != null )
						ctlDynamicButtons.ErrorText = ex.Message + " " + ex.InnerException.Message;
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
			SetPageTitle(L10n.Term("DuoUniversal.LBL_MANAGE_DUO_UNIVERSAL_TITLE"));
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

					ENABLED       .Checked = Sql.ToBoolean(Application["CONFIG.DuoUniversal.Enabled"      ]);
					CLIENT_ID     .Text    = Sql.ToString (Application["CONFIG.DuoUniversal.ClientID"     ]);
					CLIENT_SECRET .Text    = Sql.ToString (Application["CONFIG.DuoUniversal.ClientSecret" ]);
					API_HOST_URL  .Text    = Sql.ToString (Application["CONFIG.DuoUniversal.ApiHostURL"   ]);
					REDIRECT_URL  .Text    = GetDuoRedirectUrl();

					string sDuoUniversalReceievedState = Sql.ToString(Request.QueryString["state"]);
					string sDuoUniversalReceivedCode   = Sql.ToString(Request.QueryString["code" ]);
					if ( !Sql.IsEmptyString(sDuoUniversalReceievedState) && !Sql.IsEmptyString(sDuoUniversalReceivedCode) )
					{
						string sDuoUniversalSessionState    = Sql.ToString(Session["DuoUniversal.state"   ]);
						string sDuoUniversalSessionUsername = Sql.ToString(Session["DuoUniversal.username"]);
						if ( !Sql.IsEmptyString(sDuoUniversalSessionUsername) && !Sql.IsEmptyString(sDuoUniversalSessionState) )
						{
							if ( sDuoUniversalSessionState == sDuoUniversalReceievedState )
							{
								string sRedirectURL = GetDuoRedirectUrl();
								Client duoClient = new DuoUniversal.ClientBuilder(CLIENT_ID.Text, CLIENT_SECRET.Text, API_HOST_URL.Text, sRedirectURL).Build();
								Task<IdToken> resultTask = Task.Run(() => duoClient.ExchangeAuthorizationCodeFor2faResult(sDuoUniversalReceivedCode, sDuoUniversalSessionUsername));
								IdToken token = resultTask.Result;
								if ( token.AuthResult.Result == "allow" )
								{
									ctlDynamicButtons.ErrorText = L10n.Term("DuoUniversal.LBL_TEST_SUCCESSFUL");
								}
								else
								{
									ctlDynamicButtons.ErrorText = L10n.Term("DuoUniversal.ERR_LOGIN_DENIED");
								}
							}
							else
							{
								ctlDynamicButtons.ErrorText = L10n.Term("DuoUniversal.ERR_INVALID_SESSION_STATE");
							}
						}
						else
						{
							ctlDynamicButtons.ErrorText = L10n.Term("DuoUniversal.ERR_LOGIN_SESSION_HAS_EXPIRED");
						}
						Session.Remove("DuoUniversal.state"   );
						Session.Remove("DuoUniversal.username");
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
			m_sMODULE = "DuoUniversal";
			// 07/24/2010 Paul.  We need an admin flag for the areas that don't have a record in the Modules table. 
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
