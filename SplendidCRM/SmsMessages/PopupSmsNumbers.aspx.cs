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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.SmsMessages
{
	/// <summary>
	/// Summary description for PopupSmsNumbers.
	/// </summary>
	public class PopupSmsNumbers : SplendidPopup
	{
		protected _controls.SearchView     ctlSearchView    ;
		protected _controls.DynamicButtons ctlDynamicButtons;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					grdMain.CurrentPageIndex = 0;
					grdMain.DataBind();
				}
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
					arrSelectFields.AddFields(grdMain.SortColumn);
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
			SetPageTitle(L10n.Term("Contacts.LBL_LIST_FORM_TITLE"));
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dtCombined = new DataTable() )
							{
								arrSelectFields = new UniqueStringCollection();
								arrSelectFields.Add("ID"          );
								arrSelectFields.Add("NAME"        );
								arrSelectFields.Add("PHONE_MOBILE");
								arrSelectFields.Add("ACCOUNT_NAME");
								arrSelectFields.Add("SMS_OPT_IN"  );
								arrSelectFields.Add("MODULE_TYPE" );
								
								cmd.CommandText = "  from vwCONTACTS_SmsNumbers" + ControlChars.CrLf;
								Security.Filter(cmd, "Contacts", "list");
								grdMain.OrderByClause("NAME", "asc");
								ctlSearchView.SqlSearchClause(cmd);
								cmd.CommandText = "select " + Sql.FormatSelectFields(arrSelectFields)
								                + cmd.CommandText
								                + grdMain.OrderByClause();
								
								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "vwCONTACTS_SmsNumbers", Sql.ClientScriptBlock(cmd));
								da.Fill(dtCombined);
								
								cmd.Parameters.Clear();
								cmd.CommandText = "  from vwLEADS_SmsNumbers" + ControlChars.CrLf;
								Security.Filter(cmd, "Leads", "list");
								ctlSearchView.SqlSearchClause(cmd);
								cmd.CommandText = "select " + Sql.FormatSelectFields(arrSelectFields)
								                + cmd.CommandText
								                + grdMain.OrderByClause();
								
								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "vwLEADS_SmsNumbers", Sql.ClientScriptBlock(cmd));
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows)
									{
										DataRow rowNew = dtCombined.NewRow();
										foreach ( DataColumn col in dt.Columns )
										{
											if ( dtCombined.Columns.Contains(col.ColumnName) )
											{
												rowNew[col.ColumnName] = row[col.ColumnName];
											}
										}
										dtCombined.Rows.Add(rowNew);
									}
								}
								
								cmd.Parameters.Clear();
								cmd.CommandText = "  from vwPROSPECTS_SmsNumbers" + ControlChars.CrLf;
								Security.Filter(cmd, "Prospects", "list");
								ctlSearchView.SqlSearchClause(cmd);
								cmd.CommandText = "select " + Sql.FormatSelectFields(arrSelectFields)
								                + cmd.CommandText
								                + grdMain.OrderByClause();
								
								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "vwPROSPECTS_SmsNumbers", Sql.ClientScriptBlock(cmd));
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows)
									{
										DataRow rowNew = dtCombined.NewRow();
										foreach ( DataColumn col in dt.Columns )
										{
											if ( dtCombined.Columns.Contains(col.ColumnName) )
											{
												rowNew[col.ColumnName] = row[col.ColumnName];
											}
										}
										dtCombined.Rows.Add(rowNew);
									}
								}
								
								cmd.Parameters.Clear();
								cmd.CommandText = "  from vwUSERS_SmsNumbers" + ControlChars.CrLf;
								Security.Filter(cmd, "Users", "list");
								ctlSearchView.SqlSearchClause(cmd);
								cmd.CommandText = "select " + Sql.FormatSelectFields(arrSelectFields)
								                + cmd.CommandText
								                + grdMain.OrderByClause();
								
								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "vwUSERS_SmsNumbers", Sql.ClientScriptBlock(cmd));
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows)
									{
										DataRow rowNew = dtCombined.NewRow();
										foreach ( DataColumn col in dt.Columns )
										{
											if ( dtCombined.Columns.Contains(col.ColumnName) )
											{
												rowNew[col.ColumnName] = row[col.ColumnName];
											}
										}
										dtCombined.Rows.Add(rowNew);
									}
								}
								
								vwMain = dtCombined.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( !IsPostBack )
								{
									grdMain.DataBind();
								}
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
			ctlSearchView.Command += new CommandEventHandler(Page_Command);
			ctlDynamicButtons.AppendButtons("SmsMessages.PopupSmsNumbers", Guid.Empty, Guid.Empty);
			if ( !IsPostBack )
				ctlDynamicButtons.ShowButton("Clear", !Sql.ToBoolean(Request["ClearDisabled"]));
		}
		#endregion
	}
}

