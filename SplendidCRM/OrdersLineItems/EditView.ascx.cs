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

namespace SplendidCRM.OrdersLineItems
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons  ctlDynamicButtons;

		protected Guid        gID            ;
		protected Guid        gORDER_ID      ;
		protected HtmlTable   tblMain        ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields("OrdersLineItems.EditView");
					if (Page.IsValid)
					{
						// 09/09/2009 Paul.  Use the new function to get the table name. 
						string sTABLE_NAME = Crm.Modules.TableName("OrdersLineItems");
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
							// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								sSQL = "select *                         " + ControlChars.CrLf
								     + "  from vwORDERS_LINE_ITEMS_Detail" + ControlChars.CrLf
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
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												// 03/15/2014 Paul.  Dynamic Buttons is not used in this area. 
												//ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												//ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
											// It is possible that the record exists, but that ACL rules prevent it from being selected. 
											gID = Guid.Empty;
										}
									}
								}
							}

							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 05/21/2009 Paul.  Added serial number and support fields. 
									// 07/11/2010 Paul.  Add PARENT_TEMPLATE_ID. 
									// 07/15/2010 Paul.  Add GROUP_ID for options management. 
									// 08/13/2010 Paul.  Use LINE_GROUP_ID instead of GROUP_ID. 
									// 08/13/2010 Paul.  New discount fields. 
									// 08/17/2010 Paul.  Add PRICING fields so that they can be customized per line item. 
									// 10/20/2010 Paul.  Fixed missing S in DATE_SUPPORT_EXPIRES. 
									// 12/13/2013 Paul.  Allow each line item to have a separate tax rate. 
									SqlProcs.spORDERS_LINE_ITEMS_UpdateDetail
										( ref gID
										, new DynamicControl(this, rowCurrent, "ORDER_ID"            ).ID
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
										, new DynamicControl(this, rowCurrent, "SERIAL_NUMBER"       ).Text
										, new DynamicControl(this, rowCurrent, "ASSET_NUMBER"        ).Text
										, new DynamicControl(this, rowCurrent, "DATE_ORDER_SHIPPED"  ).DateValue
										, new DynamicControl(this, rowCurrent, "DATE_SUPPORT_EXPIRES").DateValue
										, new DynamicControl(this, rowCurrent, "DATE_SUPPORT_STARTS" ).DateValue
										, new DynamicControl(this, rowCurrent, "SUPPORT_NAME"        ).Text
										, new DynamicControl(this, rowCurrent, "SUPPORT_CONTACT"     ).Text
										, new DynamicControl(this, rowCurrent, "SUPPORT_TERM"        ).Text
										, new DynamicControl(this, rowCurrent, "SUPPORT_DESCRIPTION" ).Text
										, new DynamicControl(this, rowCurrent, "PARENT_TEMPLATE_ID"  ).ID
										, new DynamicControl(this, rowCurrent, "DISCOUNT_ID"         ).ID
										, new DynamicControl(this, rowCurrent, "DISCOUNT_PRICE"      ).DecimalValue
										, new DynamicControl(this, rowCurrent, "PRICING_FORMULA"     ).Text
										, new DynamicControl(this, rowCurrent, "PRICING_FACTOR"      ).FloatValue
										, new DynamicControl(this, rowCurrent, "TAXRATE_ID"          ).ID
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
						Response.Redirect("~/OrdersLineItems/view.aspx?ID=" + gID.ToString());
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
				Response.Redirect("~/OrdersLineItems/view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL;
							// 05/21/2009 Paul.  Added serial number and support fields. 
							sSQL = "select *                         " + ControlChars.CrLf
							     + "  from vwORDERS_LINE_ITEMS_Detail" + ControlChars.CrLf
							     + " where ID = @ID                  " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
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
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											// 10/18/2010 Paul.  Show this item in the last viewed of OrdersLineItems and Products. 
											Utils.UpdateTracker(Page, "OrdersLineItems", gID, ctlDynamicButtons.Title);
											Utils.UpdateTracker(Page, "Products", gID, ctlDynamicButtons.Title);

											gORDER_ID = Sql.ToGuid(rdr["ORDER_ID"]);
											ViewState["ORDER_ID"] = gORDER_ID;
											this.AppendEditViewFields("OrdersLineItems.EditView", tblMain, rdr);
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											// 10/10/2016 Paul.  Add buttons.  They must have been removed when ModuleHeader and DynamicButtons were combined. 
											ctlDynamicButtons.AppendButtons("OrdersLineItems.EditView", Guid.Empty, rdr);
										}
										else
										{
											// 11/25/2006 Paul.  If item is not visible, then don't allow save 
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											// 10/10/2016 Paul.  Add buttons.  They must have been removed when ModuleHeader and DynamicButtons were combined. 
											ctlDynamicButtons.AppendButtons("OrdersLineItems.EditView", Guid.Empty, null);
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
						this.AppendEditViewFields("OrdersLineItems.EditView", tblMain, null);
						// 10/10/2016 Paul.  Add buttons.  They must have been removed when ModuleHeader and DynamicButtons were combined. 
						ctlDynamicButtons.AppendButtons("OrdersLineItems.EditView", Guid.Empty, null);
					}
				}
				else
				{
					gORDER_ID = Sql.ToGuid(ViewState["ORDER_ID"]);
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
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
			m_sMODULE = "Orders";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields("OrdersLineItems.EditView", tblMain, null);
				// 10/10/2016 Paul.  Add buttons.  They must have been removed when ModuleHeader and DynamicButtons were combined. 
				ctlDynamicButtons.AppendButtons("OrdersLineItems.EditView", Guid.Empty, null);
			}
		}
		#endregion
	}
}

