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

namespace SplendidCRM.Administration.GetResponse
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
		protected TextBox      SECRET_API_KEY        ;
		protected TextBox      DEFAULT_CAMPAIGN_NAME ;

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
							Spring.Social.GetResponse.GetResponseSync.ValidateGetResponse(Application, SECRET_API_KEY.Text, sbErrors);

							if ( sbErrors.Length > 0 )
								ctlDynamicButtons.ErrorText = sbErrors.ToString();
							else
								ctlDynamicButtons.ErrorText = L10n.Term("GetResponse.LBL_TEST_SUCCESSFUL");
						}
						else if ( e.CommandName == "Save" )
						{
							Application["CONFIG.GetResponse.Enabled"            ] = ENABLED              .Checked;
							Application["CONFIG.GetResponse.VerboseStatus"      ] = VERBOSE_STATUS       .Checked;
							Application["CONFIG.GetResponse.Direction"          ] = DIRECTION            .SelectedValue;
							Application["CONFIG.GetResponse.ConflictResolution" ] = CONFLICT_RESOLUTION  .SelectedValue;
							Application["CONFIG.GetResponse.SyncModules"        ] = SYNC_MODULES         .SelectedValue;
							Application["CONFIG.GetResponse.SecretApiKey"       ] = SECRET_API_KEY       .Text;
							Application["CONFIG.GetResponse.DefaultCampaignName"] = DEFAULT_CAMPAIGN_NAME.Text;
							// 05/07/2015 Paul.  Clear the ID so that it will be updated on next run. 
							Application["CONFIG.GetResponse.DefaultCampaignID"  ] = String.Empty;
						
							SqlProcs.spCONFIG_Update("system", "GetResponse.Enabled"            , Sql.ToString(Application["CONFIG.GetResponse.Enabled"            ]));
							SqlProcs.spCONFIG_Update("system", "GetResponse.VerboseStatus"      , Sql.ToString(Application["CONFIG.GetResponse.VerboseStatus"      ]));
							SqlProcs.spCONFIG_Update("system", "GetResponse.Direction"          , Sql.ToString(Application["CONFIG.GetResponse.Direction"          ]));
							SqlProcs.spCONFIG_Update("system", "GetResponse.ConflictResolution" , Sql.ToString(Application["CONFIG.GetResponse.ConflictResolution" ]));
							SqlProcs.spCONFIG_Update("system", "GetResponse.SyncModules"        , Sql.ToString(Application["CONFIG.GetResponse.SyncModules"        ]));
							SqlProcs.spCONFIG_Update("system", "GetResponse.SecretApiKey"       , Sql.ToString(Application["CONFIG.GetResponse.SecretApiKey"       ]));
							SqlProcs.spCONFIG_Update("system", "GetResponse.DefaultCampaignName", Sql.ToString(Application["CONFIG.GetResponse.DefaultCampaignName"]));
#if !DEBUG
							SqlProcs.spSCHEDULERS_UpdateStatus("function::pollGetResponse", ENABLED.Checked ? "Active" : "Inactive");
#endif
							// 07/15/2017 Paul.  Instead of requiring that the user manually enable the user, do so automatically. 
							if ( Sql.ToBoolean(Application["CONFIG.GetResponse.Enabled"]) )
							{
								Guid gUSER_ID = Spring.Social.GetResponse.GetResponseSync.GetResponseUserID(Application);
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
			SetPageTitle(L10n.Term("GetResponse.LBL_MANAGE_GETRESPONSE_TITLE"));
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
					// 05/06/2015 Paul.  GetResponse does not support many contact fields (no separation for first/last name), so only send data. 
					//DIRECTION.DataSource = SplendidCache.List("getresponse_sync_direction");
					//DIRECTION.DataBind();
					DIRECTION.Items.Add(new ListItem(L10n.Term(".getresponse_sync_direction.from crm only"), "from crm only"));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(String.Empty                      ), String.Empty));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.remote"), "remote"    ));
					CONFLICT_RESOLUTION.Items.Add(new ListItem(L10n.Term(".sync_conflict_resolution.local" ), "local"     ));
					SYNC_MODULES.DataSource = SplendidCache.List("getresponse_sync_module");
					SYNC_MODULES.DataBind();
					
					ENABLED              .Checked = Sql.ToBoolean(Application["CONFIG.GetResponse.Enabled"            ]);
					VERBOSE_STATUS       .Checked = Sql.ToBoolean(Application["CONFIG.GetResponse.VerboseStatus"      ]);
					SECRET_API_KEY       .Text    = Sql.ToString (Application["CONFIG.GetResponse.SecretApiKey"       ]);
					DEFAULT_CAMPAIGN_NAME.Text    = Sql.ToString (Application["CONFIG.GetResponse.DefaultCampaignName"]);
					try
					{
						Utils.SetSelectedValue(DIRECTION, Sql.ToString(Application["CONFIG.GetResponse.Direction"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(CONFLICT_RESOLUTION, Sql.ToString(Application["CONFIG.GetResponse.ConflictResolution"]));
					}
					catch
					{
					}
					try
					{
						Utils.SetSelectedValue(SYNC_MODULES, Sql.ToString(Application["CONFIG.GetResponse.SyncModules"]));
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
			m_sMODULE = "GetResponse";
			SetAdminMenu(m_sMODULE);
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
			ctlFooterButtons .AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
		}
		#endregion
	}
}
