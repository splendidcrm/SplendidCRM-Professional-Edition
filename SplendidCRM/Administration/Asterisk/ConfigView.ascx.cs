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

namespace SplendidCRM.Administration.Asterisk
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
		protected TextBox      HOST_SERVER           ;
		protected TextBox      HOST_PORT             ;
		protected TextBox      USER_NAME             ;
		protected TextBox      PASSWORD              ;
		protected TextBox      FROM_TRUNK            ;
		protected TextBox      FROM_CONTEXT          ;
		protected CheckBox     LOG_MISSED_INCOMING_CALLS;
		protected CheckBox     LOG_MISSED_OUTGOING_CALLS;
		protected CheckBox     LOG_CALL_DETAILS         ;
		protected CheckBox     ORIGINATE_EXTENSION_FIRST;
		protected RequiredFieldValidator reqUSER_NAME;
		protected RequiredFieldValidator reqPASSWORD ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "Test" )
			{
				try
				{
					HOST_SERVER .Text = HOST_SERVER .Text.Trim();
					HOST_PORT   .Text = HOST_PORT   .Text.Trim();
					USER_NAME   .Text = USER_NAME   .Text.Trim();
					PASSWORD    .Text = PASSWORD    .Text.Trim();
					FROM_TRUNK  .Text = FROM_TRUNK  .Text.Trim();
					FROM_CONTEXT.Text = FROM_CONTEXT.Text.Trim();
					
					string sPASSWORD = PASSWORD.Text;
					// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
					if ( sPASSWORD == Sql.sEMPTY_PASSWORD )
					{
						sPASSWORD = Sql.ToString (Application["CONFIG.Asterisk.Password"]);
					}
					else
					{
						// 09/13/2013 Paul.  Make sure that the encryption keys have been created. 
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_KEY) )
						{
							gINBOUND_EMAIL_KEY = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("mail", "InboundEmailKey", gINBOUND_EMAIL_KEY.ToString());
							Application["CONFIG.InboundEmailKey"] = gINBOUND_EMAIL_KEY;
						}
						Guid gINBOUND_EMAIL_IV = Sql.ToGuid(Application["CONFIG.InboundEmailIV"]);
						if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_IV) )
						{
							gINBOUND_EMAIL_IV = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("mail", "InboundEmailIV", gINBOUND_EMAIL_IV.ToString());
							Application["CONFIG.InboundEmailIV"] = gINBOUND_EMAIL_IV;
						}
						string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sPASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sPASSWORD )
							throw(new Exception("Decryption failed"));
						sPASSWORD = sENCRYPTED_EMAIL_PASSWORD;
					}
					reqUSER_NAME .Enabled = true;
					reqPASSWORD  .Enabled = true;
					reqUSER_NAME .Validate();
					reqPASSWORD  .Validate();
					if ( Page.IsValid )
					{
						if ( e.CommandName == "Test" )
						{
							string sResult = AsteriskManager.Instance.ValidateLogin(HOST_SERVER.Text, Sql.ToInteger(HOST_PORT.Text), USER_NAME.Text, sPASSWORD);
							if ( Sql.IsEmptyString(sResult) )
								ctlDynamicButtons.ErrorText = L10n.Term("Asterisk.LBL_CONNECTION_SUCCESSFUL");
							else
								ctlDynamicButtons.ErrorText = String.Format(L10n.Term("Asterisk.ERR_FAILED_TO_CONNECT"), sResult);
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.Asterisk.Host"    ] = HOST_SERVER .Text;
							Application["CONFIG.Asterisk.Port"    ] = HOST_PORT   .Text;
							Application["CONFIG.Asterisk.UserName"] = USER_NAME   .Text;
							Application["CONFIG.Asterisk.Password"] = sPASSWORD        ;
							Application["CONFIG.Asterisk.Trunk"   ] = FROM_TRUNK  .Text;
							Application["CONFIG.Asterisk.Context" ] = FROM_CONTEXT.Text;
							Application["CONFIG.Asterisk.LogIncomingMissedCalls" ] = LOG_MISSED_INCOMING_CALLS.Checked;
							Application["CONFIG.Asterisk.LogOutgoingMissedCalls" ] = LOG_MISSED_OUTGOING_CALLS.Checked;
							Application["CONFIG.Asterisk.LogCallDetails"         ] = LOG_CALL_DETAILS.Checked;
							// 09/01/2015 Paul.  Allow Extension to be dialed first. 
							Application["CONFIG.Asterisk.OriginateExtensionFirst"] = ORIGINATE_EXTENSION_FIRST.Checked;
							
							SqlProcs.spCONFIG_Update("system", "Asterisk.Host"    , Sql.ToString(Application["CONFIG.Asterisk.Host"    ]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.Port"    , Sql.ToString(Application["CONFIG.Asterisk.Port"    ]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.UserName", Sql.ToString(Application["CONFIG.Asterisk.UserName"]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.Password", Sql.ToString(Application["CONFIG.Asterisk.Password"]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.Trunk"   , Sql.ToString(Application["CONFIG.Asterisk.Trunk"   ]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.Context" , Sql.ToString(Application["CONFIG.Asterisk.Context" ]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.LogIncomingMissedCalls" , Sql.ToString(Application["CONFIG.Asterisk.LogIncomingMissedCalls" ]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.LogOutgoingMissedCalls" , Sql.ToString(Application["CONFIG.Asterisk.LogOutgoingMissedCalls" ]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.LogCallDetails"         , Sql.ToString(Application["CONFIG.Asterisk.LogCallDetails"         ]));
							SqlProcs.spCONFIG_Update("system", "Asterisk.OriginateExtensionFirst", Sql.ToString(Application["CONFIG.Asterisk.OriginateExtensionFirst"]));
							
							AsteriskManager.Instance.Logout();
							if ( !Sql.IsEmptyString(HOST_SERVER.Text) && Sql.ToInteger(HOST_PORT.Text) > 0 && !Sql.IsEmptyString(USER_NAME.Text) && !Sql.IsEmptyString(sPASSWORD) )
								AsteriskManager.Instance.Login();
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
			SetPageTitle(L10n.Term("Asterisk.LBL_ASTERISK_SETTINGS"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				reqUSER_NAME .DataBind();
				reqPASSWORD  .DataBind();
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + ".EditView", Guid.Empty, null);

					HOST_SERVER .Text = Sql.ToString(Application["CONFIG.Asterisk.Host"    ]);
					HOST_PORT   .Text = Sql.ToString(Application["CONFIG.Asterisk.Port"    ]);
					USER_NAME   .Text = Sql.ToString(Application["CONFIG.Asterisk.UserName"]);
					FROM_TRUNK  .Text = Sql.ToString(Application["CONFIG.Asterisk.Trunk"   ]);
					FROM_CONTEXT.Text = Sql.ToString(Application["CONFIG.Asterisk.Context" ]);
					string sPASSWORD  = Sql.ToString(Application["CONFIG.Asterisk.Password"]);
					if ( !Sql.IsEmptyString(sPASSWORD) )
					{
						// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
						PASSWORD.Attributes.Add("value", Sql.sEMPTY_PASSWORD);
					}
					LOG_MISSED_INCOMING_CALLS.Checked = Sql.ToBoolean(Application["CONFIG.Asterisk.LogIncomingMissedCalls" ]);
					LOG_MISSED_OUTGOING_CALLS.Checked = Sql.ToBoolean(Application["CONFIG.Asterisk.LogOutgoingMissedCalls" ]);
					LOG_CALL_DETAILS.Checked          = Sql.ToBoolean(Application["CONFIG.Asterisk.LogCallDetails"         ]);
					// 09/01/2015 Paul.  Allow Extension to be dialed first. 
					ORIGINATE_EXTENSION_FIRST.Checked = Sql.ToBoolean(Application["CONFIG.Asterisk.OriginateExtensionFirst"]);
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
			m_sMODULE = "Asterisk";
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
