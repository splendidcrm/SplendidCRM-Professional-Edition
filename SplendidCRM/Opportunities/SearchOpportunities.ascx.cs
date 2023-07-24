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

namespace SplendidCRM.Opportunities
{
	/// <summary>
	///		Summary description for SearchOpportunities.
	/// </summary>
	public class SearchOpportunities : SplendidControl
	{
		// 05/15/2016 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlListHeader;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected bool          bShowCheckboxColumn;

		// 05/15/2016 Paul.  Move UnifiedSearch() to Sql. 
		private void Page_Load(object sender, System.EventArgs e)
		{
			string sUnifiedSearch = Sql.ToString(Request["txtUnifiedSearch"]);
			// 11/02/2010 Paul.  Provide a way to have the search text supplied by the parent control. 
			if ( Page.Items.Contains("txtUnifiedSearch") )
			{
				bShowCheckboxColumn = true;
				sUnifiedSearch = Sql.ToString(Page.Items["txtUnifiedSearch"]);
			}
			if ( !Sql.IsEmptyString(sUnifiedSearch.Trim()) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					// 08/03/2010 Paul.  Reduce the select fields. 
					// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
					m_sVIEW_NAME = "vwOPPORTUNITIES_List";
					sSQL = "select " + Sql.FormatSelectFields(arrSelectFields) + ControlChars.CrLf
					     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
					     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						// 03/31/2008 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
						cmd.CommandText = sSQL;
						Security.Filter(cmd, m_sMODULE, "list");
						// 05/15/2016 Paul.  Move UnifiedSearch() to Sql. 
						cmd.CommandText += Sql.UnifiedSearch(m_sMODULE, sUnifiedSearch, cmd);
						// 08/09/2008 Paul.  Move Last Sort to the database.
						cmd.CommandText += grdMain.OrderByClause("NAME", "asc");

						if ( bDebug )
							RegisterClientScriptBlock("vwOPPORTUNITIES_List", Sql.ClientScriptBlock(cmd));

						try
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									// 03/10/2014 Paul.  Apply Business Rules to unified search. 
									this.ApplyGridViewRules(m_sMODULE + ".Search", dt);
									
									vwMain = dt.DefaultView;
									grdMain.DataSource = vwMain ;
									// 08/09/2009 Paul.  Always rebind so that pagination will not get lost when another panel gets the pagination request. 
									// 08/26/2010 Paul.  Hide panel if no results. 
									this.Visible = (dt.Rows.Count > 0);
									if ( this.Visible )
										grdMain.DataBind();
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							lblError.Text = ex.Message;
						}
					}
				}
				ctlListHeader.Visible = true;
			}
			else
			{
				ctlListHeader.Visible = false;
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
			// 12/28/2007 Paul.  UnifiedSearch should be customizable. 
			m_sMODULE = "Opportunities";
			// 08/03/2010 Paul.  Reduce the select fields. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("ID"  );
			arrSelectFields.Add("NAME");
			// 07/05/2012 Paul.  Add Edit button to grid. 
			arrSelectFields.Add("ASSIGNED_USER_ID");
			this.AppendGridColumns(grdMain, m_sMODULE + ".Search", arrSelectFields);
		}
		#endregion
	}
}

