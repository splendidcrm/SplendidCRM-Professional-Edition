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

namespace SplendidCRM.Administration.ProductTemplates
{
	/// <summary>
	///		Summary description for RelatedProductTemplates.
	/// </summary>
	public class RelatedProductTemplates : SplendidControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected HtmlInputHidden txtPRODUCT_TEMPLATE_ID  ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "ProductTemplates.Edit":
					{
						Guid gPRODUCT_TEMPLATE_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/Administration/ProductTemplates/edit.aspx?ID=" + gPRODUCT_TEMPLATE_ID.ToString());
						break;
					}
					case "ProductTemplates.Remove":
					{
						Guid gPRODUCT_TEMPLATE_ID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spPRODUCT_PRODUCT_Delete(gID, gPRODUCT_TEMPLATE_ID);
						BindGrid();
						break;
					}
					default:
						throw(new Exception("Unknown command: " + e.CommandName));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void BindGrid()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
				     + "  from vwPRODUCTS_RELATED_PRODUCTS" + ControlChars.CrLf
				     + " where 1 = 1                      " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AppendParameter(cmd, gID, "PARENT_ID");
					cmd.CommandText += grdMain.OrderByClause("CHILD_NAME", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwPRODUCTS_RELATED_PRODUCTS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								grdMain.DataBind();
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			if ( SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 )
			{
				if ( !Sql.IsEmptyString(txtPRODUCT_TEMPLATE_ID.Value) )
				{
					try
					{
						SqlProcs.spPRODUCT_PRODUCT_MassUpdate(gID, txtPRODUCT_TEMPLATE_ID.Value);
						txtPRODUCT_TEMPLATE_ID.Value = String.Empty;
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
					}
				}
			}
			try
			{
				BindGrid();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}

			if ( !IsPostBack )
			{
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".RelatedProducts", Guid.Empty, gID);
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
			m_sMODULE = "ProductTemplates";
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("CHILD_ID"    );
			this.AppendGridColumns(grdMain, m_sMODULE + ".RelatedProducts", arrSelectFields);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".RelatedProducts", Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}
