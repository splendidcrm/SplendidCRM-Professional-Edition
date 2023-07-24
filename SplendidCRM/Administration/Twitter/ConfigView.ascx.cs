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

namespace SplendidCRM.Administration.Twitter
{
	/// <summary>
	///		Summary description for ConfigView.
	/// </summary>
	public class ConfigView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected CheckBox     ENABLE_TRACKING        ;
		protected TextBox      TWITTER_CONSUMER_KEY   ;
		protected TextBox      TWITTER_CONSUMER_SECRET;
		protected TextBox      ACCESS_TOKEN           ;
		protected TextBox      ACCESS_TOKEN_SECRET    ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				try
				{
					if ( Page.IsValid )
					{
						// 02/26/2015 Paul.  Provide a way to disable twitter without clearing values. 
						Application["CONFIG.Twitter.EnableTracking"   ] = ENABLE_TRACKING        .Checked;
						Application["CONFIG.Twitter.ConsumerKey"      ] = TWITTER_CONSUMER_KEY   .Text.Trim();
						Application["CONFIG.Twitter.ConsumerSecret"   ] = TWITTER_CONSUMER_SECRET.Text.Trim();
						Application["CONFIG.Twitter.AccessToken"      ] = ACCESS_TOKEN           .Text.Trim();
						Application["CONFIG.Twitter.AccessTokenSecret"] = ACCESS_TOKEN_SECRET    .Text.Trim();
						
						SqlProcs.spCONFIG_Update("system", "Twitter.EnableTracking"   , Sql.ToString(Application["CONFIG.Twitter.EnableTracking"   ]));
						SqlProcs.spCONFIG_Update("system", "Twitter.ConsumerKey"      , Sql.ToString(Application["CONFIG.Twitter.ConsumerKey"      ]));
						SqlProcs.spCONFIG_Update("system", "Twitter.ConsumerSecret"   , Sql.ToString(Application["CONFIG.Twitter.ConsumerSecret"   ]));
						SqlProcs.spCONFIG_Update("system", "Twitter.AccessToken"      , Sql.ToString(Application["CONFIG.Twitter.AccessToken"      ]));
						SqlProcs.spCONFIG_Update("system", "Twitter.AccessTokenSecret", Sql.ToString(Application["CONFIG.Twitter.AccessTokenSecret"]));
						Response.Redirect("../default.aspx");
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Start" )
			{
				try
				{
					TwitterManager.Instance.Start();
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Stop" )
			{
				try
				{
					TwitterManager.Instance.Stop();
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("../default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Twitter.LBL_MANAGE_TWITTER_TITLE"));
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

					// 02/26/2015 Paul.  Provide a way to disable twitter without clearing values. 
					ENABLE_TRACKING        .Checked = Sql.ToBoolean(Application["CONFIG.Twitter.EnableTracking"   ]);
					TWITTER_CONSUMER_KEY   .Text    = Sql.ToString (Application["CONFIG.Twitter.ConsumerKey"      ]);
					TWITTER_CONSUMER_SECRET.Text    = Sql.ToString (Application["CONFIG.Twitter.ConsumerSecret"   ]);
					ACCESS_TOKEN           .Text    = Sql.ToString (Application["CONFIG.Twitter.AccessToken"      ]);
					ACCESS_TOKEN_SECRET    .Text    = Sql.ToString (Application["CONFIG.Twitter.AccessTokenSecret"]);
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
			m_sMODULE = "Twitter";
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
