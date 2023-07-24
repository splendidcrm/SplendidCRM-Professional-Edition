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
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.PaymentGateway
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;
		// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
		//private const string sEMPTY_PASSWORD = "**********";

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView);
					if ( Page.IsValid )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *                 " + ControlChars.CrLf
								     + "  from vwPAYMENT_GATEWAYS" + ControlChars.CrLf
								     + " where ID = @ID          " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											rowCurrent = dtCurrent.Rows[0];
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											gID = Guid.Empty;
										}
									}
								}
							}
							
							Guid gCREDIT_CARD_KEY = Sql.ToGuid(Application["CONFIG.CreditCardKey"]);
							if ( Sql.IsEmptyGuid(gCREDIT_CARD_KEY) )
							{
								gCREDIT_CARD_KEY = Guid.NewGuid();
								SqlProcs.spCONFIG_Update("system", "CreditCardKey", gCREDIT_CARD_KEY.ToString());
								Application["CONFIG.CreditCardKey"] = gCREDIT_CARD_KEY;
							}
							Guid gCREDIT_CARD_IV = Sql.ToGuid(Application["CONFIG.CreditCardIV"]);
							if ( Sql.IsEmptyGuid(gCREDIT_CARD_IV) )
							{
								gCREDIT_CARD_IV = Guid.NewGuid();
								SqlProcs.spCONFIG_Update("system", "CreditCardIV", gCREDIT_CARD_IV.ToString());
								Application["CONFIG.CreditCardIV"] = gCREDIT_CARD_IV;
							}
							
							string sNAME        = new DynamicControl(this, rowCurrent, "NAME"       ).Text;
							string sGATEWAY     = new DynamicControl(this, rowCurrent, "GATEWAY"    ).Text;
							string sLOGIN       = new DynamicControl(this, rowCurrent, "LOGIN"      ).Text;
							string sPASSWORD    = new DynamicControl(this, rowCurrent, "PASSWORD"   ).Text;
							bool   bTEST_MODE   = new DynamicControl(this, rowCurrent, "TEST_MODE"  ).Checked;
							string sDESCRIPTION = new DynamicControl(this, rowCurrent, "DESCRIPTION").Text;
							// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
							if ( sPASSWORD == Sql.sEMPTY_PASSWORD && rowCurrent != null )
							{
								sPASSWORD = Sql.ToString(rowCurrent["PASSWORD"]);
							}
							else
							{
								string sENCRYPTED_PASSWORD = Security.EncryptPassword(sPASSWORD, gCREDIT_CARD_KEY, gCREDIT_CARD_IV);
								if ( Security.DecryptPassword(sENCRYPTED_PASSWORD, gCREDIT_CARD_KEY, gCREDIT_CARD_IV) != sPASSWORD )
									throw(new Exception("Decryption failed"));
								sPASSWORD = sENCRYPTED_PASSWORD;
							}
							
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									SqlProcs.spPAYMENT_GATEWAYS_Update
										( ref gID
										, sNAME
										, sGATEWAY
										, sLOGIN
										, sPASSWORD
										, bTEST_MODE
										, sDESCRIPTION
										, trn
										);
									trn.Commit();
									
									if ( gID == Sql.ToGuid(Application["CONFIG.PaymentGateway_ID"]) || Sql.IsEmptyGuid(Application["CONFIG.PaymentGateway_ID"]) )
									{
										Application["CONFIG.PaymentGateway_ID"      ] = gID       ;
										Application["CONFIG.PaymentGateway"         ] = sGATEWAY  ;
										Application["CONFIG.PaymentGateway_Login"   ] = sLOGIN    ;
										Application["CONFIG.PaymentGateway_Password"] = sPASSWORD ;
										Application["CONFIG.PaymentGateway_TestMode"] = bTEST_MODE;
										SqlProcs.spCONFIG_Update("payments","PaymentGateway_ID"      , Sql.ToString(Application["CONFIG.PaymentGateway_ID"      ]));
										SqlProcs.spCONFIG_Update("payments","PaymentGateway"         , Sql.ToString(Application["CONFIG.PaymentGateway"         ]));
										SqlProcs.spCONFIG_Update("payments","PaymentGateway_Login"   , Sql.ToString(Application["CONFIG.PaymentGateway_Login"   ]));
										SqlProcs.spCONFIG_Update("payments","PaymentGateway_Password", Sql.ToString(Application["CONFIG.PaymentGateway_Password"]));
										SqlProcs.spCONFIG_Update("payments","PaymentGateway_TestMode", Sql.ToString(Application["CONFIG.PaymentGateway_TestMode"]));
									}
									// 10/19/2010 Paul.  Clear the PaymentGateways cache. 
									SplendidCache.ClearPaymentGateways();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									ctlDynamicButtons.ErrorText = ex.Message;
									return;
								}
							}
						}
						Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_MODULE_NAME"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
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
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                 " + ControlChars.CrLf
							     + "  from vwPAYMENT_GATEWAYS" + ControlChars.CrLf
							     + " where ID = @ID          " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
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
											SetPageTitle(L10n.Term(m_sMODULE + ".LBL_MODULE_NAME") + " - " + ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;

											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											// 11/08/2019 Paul.  Move sEMPTY_PASSWORD to Sql. 
											new DynamicControl(this, "PASSWORD").Text = Sql.sEMPTY_PASSWORD;
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
					}
				}
				else
				{
					// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(m_sMODULE + ".LBL_MODULE_NAME") + " - " + ctlDynamicButtons.Title);
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
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			m_sMODULE = "PaymentGateway";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}
