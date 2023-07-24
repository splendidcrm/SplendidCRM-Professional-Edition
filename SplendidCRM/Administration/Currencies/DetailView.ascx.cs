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

namespace SplendidCRM.Administration.Currencies
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected _controls.HeaderButtons         ctlDynamicButtons   ;
		protected CurrencyLayer.SystemCurrencyLog ctlSystemCurrencyLog;

		protected Guid          gID              ;
		protected HtmlTable     tblMain          ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Currencies.MakeDefault" )
				{
					string sNAME     = "default_currency";
					string sCATEGORY = "system";
					string sVALUE    = gID.ToString();
					SqlProcs.spCONFIG_Update(sCATEGORY, sNAME, sVALUE);
					Application["CONFIG." + sNAME] = sVALUE;
					Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Currencies.MakeBase" )
				{
					string sNAME     = "base_currency";
					string sCATEGORY = "system";
					string sVALUE    = gID.ToString();
					SqlProcs.spCONFIG_Update(sCATEGORY, sNAME, sVALUE);
					// 05/01/2016 Paul.  The conversion rate for the base currency is always 1.0. 
					string sISO4217 = new DynamicControl(this, "ISO4217").Text;
					SqlProcs.spCURRENCIES_UpdateRateByISO( sISO4217, 1.0f, Guid.Empty);
					Application["CONFIG." + sNAME] = sVALUE;
					Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Currencies.UpdateRate" )
				{
					StringBuilder sbErrors = new StringBuilder();
					string sISO4217 = new DynamicControl(this, "ISO4217").Text;
					float dRate = OrderUtils.GetCurrencyConversionRate(Application, sISO4217, sbErrors);
					if ( sbErrors.Length == 0 )
					{
						Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
					else
					{
						ctlDynamicButtons.ErrorText = sbErrors.ToString();
					}
				}
				else if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
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
							sSQL = "select *                " + ControlChars.CrLf
							     + "  from vwCURRENCIES_Edit" + ControlChars.CrLf
							     + " where 1 = 1            " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, gID, "ID", false);
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
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											
											this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
											ctlSystemCurrencyLog.DestinationISO4217 = Sql.ToString(rdr["ISO4217"]);
											
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, rdr);
											ctlDynamicButtons.ShowButton("Currencies.MakeDefault", !Sql.ToBoolean(rdr["IS_DEFAULT"]));
											ctlDynamicButtons.ShowButton("Currencies.MakeBase"   , !Sql.ToBoolean(rdr["IS_BASE"   ]));
											ctlDynamicButtons.ShowButton("Currencies.UpdateRate" , !Sql.ToBoolean(rdr["IS_BASE"   ]) && !Sql.IsEmptyString(Application["CONFIG.CurrencyLayer.AccessKey"]));
											
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
			m_sMODULE = "Currencies";
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

