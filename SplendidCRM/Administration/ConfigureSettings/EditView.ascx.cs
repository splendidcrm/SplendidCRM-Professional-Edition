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
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;

namespace SplendidCRM.Administration.ConfigureSettings
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		// 01/13/2010 Paul.  Add footer buttons. 
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected TextBox         txtNOTIFY_FROMNAME           ;
		protected TextBox         txtNOTIFY_FROMADDRESS        ;
		protected CheckBox        chkNOTIFY_ON                 ;
		protected CheckBox        chkNOTIFY_SEND_BY_DEFAULT    ;
		protected DropDownList    lstMAIL_SENDTYPE             ;
		protected TextBox         txtMAIL_SMTPSERVER           ;
		protected TextBox         txtMAIL_SMTPPORT             ;
		protected CheckBox        chkMAIL_SMTPAUTH_REQ         ;
		protected TextBox         txtMAIL_SMTPUSER             ;
		protected TextBox         txtMAIL_SMTPPASS             ;
		protected CheckBox        chkPORTAL_ON                 ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					try
					{
						SqlProcs.spCONFIG_Update("notify", "fromname"       , txtNOTIFY_FROMNAME       .Text);
						SqlProcs.spCONFIG_Update("notify", "fromaddress"    , txtNOTIFY_FROMADDRESS    .Text);
						SqlProcs.spCONFIG_Update("notify", "on"             , chkNOTIFY_ON             .Checked ? "1" : "0");
						SqlProcs.spCONFIG_Update("notify", "send_by_default", chkNOTIFY_SEND_BY_DEFAULT.Checked ? "1" : "0");
						SqlProcs.spCONFIG_Update("mail"  , "sendtype"       , lstMAIL_SENDTYPE         .SelectedValue      );
						SqlProcs.spCONFIG_Update("mail"  , "smtpserver"     , txtMAIL_SMTPSERVER       .Text);
						SqlProcs.spCONFIG_Update("mail"  , "smtpport"       , txtMAIL_SMTPPORT         .Text);
						SqlProcs.spCONFIG_Update("mail"  , "smtpauth_req"   , chkMAIL_SMTPAUTH_REQ     .Checked ? "1" : "0");
						SqlProcs.spCONFIG_Update("mail"  , "smtpuser"       , txtMAIL_SMTPUSER         .Text);
						SqlProcs.spCONFIG_Update("mail"  , "smtppass"       , txtMAIL_SMTPPASS         .Text);
						SqlProcs.spCONFIG_Update("portal", "on"             , chkPORTAL_ON             .Checked ? "1" : "0");
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
						return;
					}
					Response.Redirect("../default.aspx");
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("../default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList.Administration"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				if ( !IsPostBack )
				{
					// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

					lstMAIL_SENDTYPE.DataSource = SplendidCache.List("notifymail_sendtype");
					lstMAIL_SENDTYPE.DataBind();

					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select *        " + ControlChars.CrLf
						     + "  from vwCONFIG " + ControlChars.CrLf
						     + " where CATEGORY_NAME in ( 'notify_fromname'       " + ControlChars.CrLf
						     + "                        , 'notify_fromaddress'    " + ControlChars.CrLf
						     + "                        , 'notify_on'             " + ControlChars.CrLf
						     + "                        , 'notify_send_by_default'" + ControlChars.CrLf
						     + "                        , 'mail_smtpserver'       " + ControlChars.CrLf
						     + "                        , 'mail_smtpport'         " + ControlChars.CrLf
						     + "                        , 'mail_smtpauth_req'     " + ControlChars.CrLf
						     + "                        , 'mail_smtpuser'         " + ControlChars.CrLf
						     + "                        , 'mail_smtppass'         " + ControlChars.CrLf
						     + "                        )" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							con.Open();

							if ( bDebug )
								RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									string sCATEGORY_NAME = Sql.ToString(rdr["CATEGORY_NAME"]);
									switch ( sCATEGORY_NAME.ToUpper() )
									{
										case "NOTIFY_FROMNAME"       :  txtNOTIFY_FROMNAME       .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "NOTIFY_FROMADDRESS"    :  txtNOTIFY_FROMADDRESS    .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "NOTIFY_ON"             :  chkNOTIFY_ON             .Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "NOTIFY_SEND_BY_DEFAULT":  chkNOTIFY_SEND_BY_DEFAULT.Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "MAIL_SMTPSERVER"       :  txtMAIL_SMTPSERVER       .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "MAIL_SMTPPORT"         :  txtMAIL_SMTPPORT         .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "MAIL_SMTPAUTH_REQ"     :  chkMAIL_SMTPAUTH_REQ     .Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "MAIL_SMTPUSER"         :  txtMAIL_SMTPUSER         .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "MAIL_SMTPPASS"         :  txtMAIL_SMTPPASS         .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "PORTAL_ON"             :  chkPORTAL_ON             .Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "MAIL_SENDTYPE":
											try
											{
												// 08/19/2010 Paul.  Check the list before assigning the value. 
												Utils.SetSelectedValue(lstMAIL_SENDTYPE, Sql.ToString (rdr["VALUE"]));
											}
											catch(Exception ex)
											{
												SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
											}
											break;
									}
								}
							}
						}
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList.Administration") + " - " + ctlDynamicButtons.Title);
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
			m_sMODULE = "ConfigureSettings";
			// 05/06/2010 Paul.  The menu will show the admin Module Name in the Six theme. 
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}
