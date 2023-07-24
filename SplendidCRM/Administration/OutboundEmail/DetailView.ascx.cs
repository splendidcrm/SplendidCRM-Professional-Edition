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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.OutboundEmail
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;

		// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
		//private const string  sEMPTY_PASSWORD = "**********";
		protected Guid        gID              ;
		protected HtmlTable   tblMain          ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Duplicate" )
				{
					Response.Redirect("edit.aspx?DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spOUTBOUND_EMAILS_Delete(gID);
					SplendidCache.ClearOutboundMail();
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Test" )
				{
					try
					{
						string sMAIL_SMTPUSER = new DynamicControl(this, "MAIL_SMTPUSER").Text;
						string sFROM_NAME     = new DynamicControl(this, "FROM_NAME"    ).Text;
						string sFROM_ADDR     = new DynamicControl(this, "FROM_ADDR"    ).Text;
						string sMAIL_SENDTYPE = new DynamicControl(this, "MAIL_SENDTYPE").Text;
						// 01/31/2017 Paul.  Add support for Exchange using Username/Password. 
						if ( String.Compare(sMAIL_SENDTYPE, "smtp", true) == 0 || String.Compare(sMAIL_SENDTYPE, "Exchange-Password", true) == 0 )
						{
							Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
							Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
							string sMAIL_SMTPPASS = Sql.ToString(ViewState["smtppass"]);
							if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
								sMAIL_SMTPPASS = Security.DecryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							
							if ( String.Compare(sMAIL_SENDTYPE, "smtp", true) == 0 )
							{
								string sMAIL_SMTPSERVER = new DynamicControl(this, "MAIL_SMTPSERVER").Text;
								int    nMAIL_SMTPPORT   = new DynamicControl(this, "MAIL_SMTPPORT"  ).IntegerValue;
								bool   bSmtpAuthReq     = Sql.ToBoolean(ViewState["smtpauth_req"]);
								bool   bSmtpSSL         = Sql.ToBoolean(ViewState["smtpssl"     ]);
								EmailUtils.SendTestMessage(Application, sMAIL_SMTPSERVER, nMAIL_SMTPPORT, bSmtpAuthReq, bSmtpSSL, sMAIL_SMTPUSER, sMAIL_SMTPPASS, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
								ctlDynamicButtons.ErrorText = "Send was successful.";
							}
							// 01/31/2017 Paul.  Add support for Exchange using Username/Password. 
							else if ( String.Compare(sMAIL_SENDTYPE, "Exchange-Password", true) == 0 )
							{
								string sENCRYPTED_EMAIL_PASSWORD = Sql.ToString(ViewState["smtppass"]);
								string sIMPERSONATED_TYPE        = Sql.ToString(Application["CONFIG.Exchange.ImpersonatedType"]);
								string sSERVER_URL               = Sql.ToString(Application["CONFIG.Exchange.ServerURL"       ]);
								ExchangeUtils.SendTestMessage(Application, sSERVER_URL, sMAIL_SMTPUSER, sENCRYPTED_EMAIL_PASSWORD, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
								ctlDynamicButtons.ErrorText = "Send was successful.";
							}
						}
						else if ( String.Compare(sMAIL_SENDTYPE, "Office365", true) == 0 )
						{
							//StringBuilder sbErrors = new StringBuilder();
							//string sOAuthClientID     = Sql.ToString(Application["CONFIG.Exchange.ClientID"    ]);
							//string sOAuthClientSecret = Sql.ToString(Application["CONFIG.Exchange.ClientSecret"]);
							//SplendidCRM.ActiveDirectory.Office365TestAccessToken(Application, sOAuthClientID, sOAuthClientSecret, gID, sbErrors);
							//ctlDynamicButtons.ErrorText = sbErrors.ToString();
							// 12/13/2020 Paul.  Move Office365 methods to Office365utils. 
							Office365Utils.SendTestMessage(Application, gID, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
							ctlDynamicButtons.ErrorText = "Send was successful.";
						}
						else if ( String.Compare(sMAIL_SENDTYPE, "GoogleApps", true) == 0 )
						{
							//StringBuilder sbErrors = new StringBuilder();
							//SplendidCRM.GoogleApps.TestAccessToken(Application, gID, sbErrors);
							//ctlDynamicButtons.ErrorText = sbErrors.ToString();
							GoogleApps.SendTestMessage(Application, gID, sFROM_ADDR, sFROM_NAME, sFROM_ADDR, sFROM_NAME);
							ctlDynamicButtons.ErrorText = "Send was successful.";
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
						return;
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwOUTBOUND_EMAILS_Edit" + ControlChars.CrLf
							     + " where ID = @ID              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											
											this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, rdr);
											ViewState["smtppass"    ] = Sql.ToString (rdr["MAIL_SMTPPASS"    ]);
											ViewState["smtpauth_req"] = Sql.ToBoolean(rdr["MAIL_SMTPAUTH_REQ"]);
											ViewState["smtpssl"     ] = Sql.ToBoolean(rdr["MAIL_SMTPSSL"     ]);
											
											bool bSmtp = Sql.ToString(rdr["MAIL_SENDTYPE"]) == "smtp";
											new DynamicControl(this, "MAIL_SMTPSERVER"  ).Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPPORT"    ).Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPSSL"     ).Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPUSER"    ).Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPSERVER_LABEL"  ).Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPPORT_LABEL"    ).Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPAUTH_REQ_LABEL").Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPSSL_LABEL"     ).Visible = bSmtp;
											new DynamicControl(this, "MAIL_SMTPUSER_LABEL"    ).Visible = bSmtp;
										}
										else
										{
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
					}
					else
					{
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
						ctlDynamicButtons.DisableAll();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "OutboundEmail";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			}
		}
		#endregion
	}
}

