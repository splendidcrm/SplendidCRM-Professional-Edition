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

namespace SplendidCRM.RevenueLineItems
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;

		protected Guid        gID            ;
		protected Guid        gOPPORTUNITY_ID;
		protected HtmlTable   tblMain        ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields("RevenueLineItems.EditView");
					if (Page.IsValid)
					{
						string sTABLE_NAME = Crm.Modules.TableName("RevenueLineItems");
						DataTable dtCustomFields = new DataTable();
						dtCustomFields.Columns.Add("NAME"    , Type.GetType("System.String"));
						dtCustomFields.Columns.Add("CsType"  , Type.GetType("System.String"));
						dtCustomFields.Columns.Add("MAX_SIZE", Type.GetType("System.Int32" ));
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                       " + ControlChars.CrLf
							     + "  from vwSqlColumns            " + ControlChars.CrLf
							     + " where ObjectName = @ObjectName" + ControlChars.CrLf
							     + "   and ColumnName <> 'ID_C'    " + ControlChars.CrLf
							     + " order by colid                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 02/20/2016 Paul.  Make sure to use upper case for Oracle. 
								Sql.AddParameter(cmd, "@ObjectName", Sql.MetadataName(cmd, sTABLE_NAME + "_CSTM"));
								using (DbDataAdapter da = dbf.CreateDataAdapter())
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									DataTable dtCSTM = new DataTable();
									da.Fill(dtCSTM);
									foreach (DataRow rowCSTM in dtCSTM.Rows)
									{
										DataRow row = dtCustomFields.NewRow();
										row["NAME"    ] = Sql.ToString (rowCSTM["ColumnName"]);
										row["CsType"  ] = Sql.ToString (rowCSTM["CsType"    ]);
										row["MAX_SIZE"] = Sql.ToInteger(rowCSTM["length"    ]);
										dtCustomFields.Rows.Add(row);
									}
								}
							}
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								sSQL = "select *                         " + ControlChars.CrLf
								     + "  from vwREVENUE_LINE_ITEMS      " + ControlChars.CrLf
								     + " where ID = @ID                  " + ControlChars.CrLf;
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
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
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
							
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									SqlProcs.spREVENUE_LINE_ITEMS_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "OPPORTUNITY_ID"      ).ID
										, new DynamicControl(this, rowCurrent, "LINE_GROUP_ID"       ).ID
										, new DynamicControl(this, rowCurrent, "LINE_ITEM_TYPE"      ).Text
										, new DynamicControl(this, rowCurrent, "POSITION"            ).IntegerValue
										, new DynamicControl(this, rowCurrent, "NAME"                ).Text
										, new DynamicControl(this, rowCurrent, "MFT_PART_NUM"        ).Text
										, new DynamicControl(this, rowCurrent, "VENDOR_PART_NUM"     ).Text
										, new DynamicControl(this, rowCurrent, "PRODUCT_TEMPLATE_ID" ).ID
										, new DynamicControl(this, rowCurrent, "TAX_CLASS"           ).Text
										, new DynamicControl(this, rowCurrent, "QUANTITY"            ).IntegerValue
										, new DynamicControl(this, rowCurrent, "COST_PRICE"          ).DecimalValue
										, new DynamicControl(this, rowCurrent, "LIST_PRICE"          ).DecimalValue
										, new DynamicControl(this, rowCurrent, "UNIT_PRICE"          ).DecimalValue
										, new DynamicControl(this, rowCurrent, "DESCRIPTION"         ).Text
										, new DynamicControl(this, rowCurrent, "PARENT_TEMPLATE_ID"  ).ID
										, new DynamicControl(this, rowCurrent, "DISCOUNT_ID"         ).ID
										, new DynamicControl(this, rowCurrent, "DISCOUNT_PRICE"      ).DecimalValue
										, new DynamicControl(this, rowCurrent, "PRICING_FORMULA"     ).Text
										, new DynamicControl(this, rowCurrent, "PRICING_FACTOR"      ).FloatValue
										, new DynamicControl(this, rowCurrent, "TAXRATE_ID"          ).ID
										, new DynamicControl(this, rowCurrent, "OPPORTUNITY_TYPE"    ).Text
										, new DynamicControl(this, rowCurrent, "LEAD_SOURCE"         ).Text
										, new DynamicControl(this, rowCurrent, "DATE_CLOSED"         ).DateValue
										, new DynamicControl(this, rowCurrent, "NEXT_STEP"           ).Text
										, new DynamicControl(this, rowCurrent, "SALES_STAGE"         ).Text
										, new DynamicControl(this, rowCurrent, "PROBABILITY"         ).FloatValue
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									trn.Commit();
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
						Response.Redirect("~/RevenueLineItems/view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if (e.CommandName == "Cancel")
			{
				Response.Redirect("~/RevenueLineItems/view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

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
							string sSQL;
							sSQL = "select *                         " + ControlChars.CrLf
							     + "  from vwREVENUE_LINE_ITEMS      " + ControlChars.CrLf
							     + " where ID = @ID                  " + ControlChars.CrLf;
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
											ctlDynamicButtons.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											Utils.UpdateTracker(Page, "RevenueLineItems", gID, ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, "Products", gID, ctlDynamicButtons.Title);

											gOPPORTUNITY_ID = Sql.ToGuid(rdr["OPPORTUNITY_ID"]);
											ViewState["OPPORTUNITY_ID"] = gOPPORTUNITY_ID;
											this.AppendEditViewFields("RevenueLineItems.EditView", tblMain, rdr);
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
										}
										else
										{
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
					}
				}
				else
				{
					gOPPORTUNITY_ID = Sql.ToGuid(ViewState["OPPORTUNITY_ID"]);
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
				}
			}
			catch (Exception ex)
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
			m_sMODULE = "Opportunities";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendEditViewFields("RevenueLineItems.EditView", tblMain, null);
			}
		}
		#endregion
	}
}

